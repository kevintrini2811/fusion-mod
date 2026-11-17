using FusionMod;
using Il2CppReloaded.Gameplay;
using R = FusionMod.FusionNativeTimeRecords;

static class ControllerTests
{
    static int checks;
    static void Check(bool value, string name) { if (!value) throw new Exception(name); ++checks; }
    static void Refused<T>(Action action, string name) where T : Exception
    { try { action(); } catch (T) { ++checks; return; } throw new Exception(name); }
    static Board Board() { var board = new Board(); board.mApp = new() { Board = board }; return board; }
    static GridItem Carrier(Board board)
    {
        var entry = R.Get(board, R.Kind.Status, 1, 100)!;
        return board.m_gridItems.Values.Single();
    }
    static void Main()
    {
        var board = Board();
        var item = Carrier(board);
        var animation = item.mController!.m_animationController!;
        Refused<InvalidOperationException>(() => FusionRecordController.Prepare(item), "unregistered thread");
        FusionRecordController.Initialize();
        FusionRecordController.Prepare(item);
        Check(animation.Starts == 1 && animation.m_initialized && animation.m_animationState != null, "deferred Start runs before serialization");
        var group = item.mController.m_spriteGroups[0];
        Check(group.sprites != null && group.groupTransform != null, "own carrier has native reader scene references");
        FusionRecordController.Prepare(item);
        Check(animation.Starts == 1, "ready Start is never replayed");
        Check(ReferenceEquals(group.groupTransform, item.mController.m_spriteGroups[0].groupTransform), "ready group references are not recreated");

        var foreignBoard = Board();
        var foreign = foreignBoard.m_gridItems.DataArrayAlloc();
        foreign.Init(foreignBoard.mApp, GridItemType.ZenTool); foreign.mSunCount = 1;
        Refused<InvalidDataException>(() => FusionRecordController.Prepare(foreign), "foreign marker rejected");
        Check(foreign.mController!.m_animationController!.Starts == 0, "foreign animation untouched");
        FusionRecordController.PrepareLoaded(foreign.mController);
        Check(foreign.mController.m_spriteGroups[0].sprites == null && foreign.mController.m_spriteGroups[0].groupTransform == null,
            "foreign native controller groups remain untouched");
        var loaded = Carrier(Board());
        var loadedController = loaded.mController!;
        loadedController.m_spriteGroups[0] = new();
        loaded.mController = null;
        FusionRecordController.PrepareLoaded(loadedController);
        Check(loadedController.m_spriteGroups[0].sprites != null && loadedController.m_spriteGroups[0].groupTransform != null,
            "loaded own groups prepare before native mController assignment");
        var populated = loadedController.m_spriteGroups[0];
        populated.sprites!.Add(new UnityEngine.Sprite()); loadedController.m_spriteGroups[0] = populated;
        FusionRecordController.PrepareLoaded(loadedController);
        Check(loadedController.m_spriteGroups[0].spriteRenderer != null, "nonempty own sprite list receives its missing renderer");
        Check(loadedController.m_spriteGroups.Count == 1, "native group count remains unchanged");
        var legacy = foreignBoard.m_gridItems.DataArrayAlloc();
        legacy.Init(foreignBoard.mApp, GridItemType.ZenTool); legacy.mSunCount = -521095;
        FusionRecordController.PrepareLoaded(legacy.mController!);
        Check(legacy.mController!.m_spriteGroups[0].sprites == null, "legacy nonce is not an active carrier outside loading");
        FusionProjectileRecords.Loading = true;
        FusionRecordController.PrepareLoaded(legacy.mController);
        FusionProjectileRecords.Loading = false;
        Check(legacy.mController.m_spriteGroups[0].sprites != null, "authenticated legacy nonce can pass native load before retirement");
        var stale = Carrier(Board()); stale.mBoard.m_gridItems.Values.Clear();
        Refused<InvalidDataException>(() => FusionRecordController.Prepare(stale), "stale full native identity rejected");
        Check(stale.mController!.m_animationController!.Starts == 1, "new own allocation initialized exactly once");
        var dead = Carrier(Board()); dead.mDead = true;
        Refused<InvalidDataException>(() => FusionRecordController.Prepare(dead), "dead identity rejected");
        var missing = Carrier(Board()); missing.mController = null;
        Refused<IOException>(() => FusionRecordController.Prepare(missing), "missing controller rejected");
        var rebound = Carrier(Board()); rebound.mController!.m_gridItem = item;
        Refused<IOException>(() => FusionRecordController.Prepare(rebound), "foreign controller binding rejected");
        var partial = Carrier(Board()); partial.mController!.m_animationController!.m_animationState = null;
        Refused<IOException>(() => FusionRecordController.Prepare(partial), "partially initialized animation rejected");
        Check(partial.mController.m_animationController.Starts == 1, "partial Start never reset or replayed");
        var noAnimation = Carrier(Board()); noAnimation.mController!.m_animationController = null;
        FusionRecordController.Prepare(noAnimation); Check(true, "native no-animation branch preserved");
        Refused<InvalidOperationException>(() => Task.Run(() => FusionRecordController.Prepare(item)).GetAwaiter().GetResult(), "cross-thread native call rejected");

        FusionRecordController.Dispose();
        Check(R.InitializeController == null, "dispose unhooks only own allocator callback");
        Console.WriteLine($"PASS {checks} actual carrier/record assertions; native Unity/Spine execution is shimmed");
    }
}

namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] sealed class HarmonyPatch : Attribute { public HarmonyPatch(Type type, string method) {} }
}
namespace Il2CppReloaded.Services { sealed class SerializationService { public Il2CppReloaded.Gameplay.App? App; public bool Deserialize() => true; } }
namespace Il2CppReloaded.Gameplay
{
    static class Identity { static int value; public static IntPtr Next() => (IntPtr)Interlocked.Increment(ref value); }
    enum GridItemType { ZenTool = 9 }
    enum GridItemState { Normal }
    sealed class App { public Board Board = null!; }
    sealed class Board
    {
        public IntPtr Pointer = Identity.Next();
        public App mApp = null!; public int mMainCounter = 42;
        public Items m_gridItems;
        public Board() { m_gridItems = new(this); }
        public void DisposeBoard() {} public void InitLevel() {}
    }
    sealed class Items
    {
        readonly Board owner; public readonly List<GridItem> Values = new();
        public Items(Board owner) { this.owner = owner; }
        public int Count => Values.Count; public int Capacity => 128;
        public GridItem this[int index] => Values[index];
        public GridItem? DataArrayTryToGet(uint id) => Values.SingleOrDefault(value => value.DataID == id && !value.mDead);
        public GridItem DataArrayAlloc() { var value = new GridItem { DataID = (uint)Values.Count + 1, mBoard = owner }; Values.Add(value); return value; }
    }
    sealed class GridItem
    {
        public static bool FailStart;
        public IntPtr Pointer = Identity.Next();
        public uint DataID; public bool mDead, mVisible;
        public Board mBoard = null!; public GridItemType mGridItemType; public GridItemState mGridItemState;
        public int mSunCount, mGridItemCounter, mGridX, mGridY, mX, mY;
        public float mPosX, mPosY, mGoalX, mGoalY;
        public Il2CppSource.Controllers.GridItemController? mController;
        public void Init(App app, GridItemType type) { mBoard = app.Board; mGridItemType = type; mController = new(this); }
        public void GridItemDie() { mDead = true; }
        public void Update() {} public void DrawGridItem() {} public void PostUpdate() {}
    }
    sealed class SkeletonAnimation { public object? Skeleton = new(); }
    sealed class Animation
    {
        readonly List<string> events;
        public IntPtr Pointer = Identity.Next();
        public bool m_initialized; public int Starts;
        public object? m_animationState;
        public SkeletonAnimation? m_skeletonAnimation = new();
        public object? m_attachmentBones = new(), m_previousAnimations = new(), m_currentAnimations = new();
        public Animation(List<string> events) { this.events = events; }
        public void Start() { ++Starts; events.Add("Start"); if (GridItem.FailStart) throw new IOException("shim native Start failed"); m_initialized = true; m_animationState = new(); }
        public static bool operator !(Animation value) => false;
    }
}
namespace Il2CppSource.Controllers
{
    sealed class GridItemController
    {
        public IntPtr Pointer = Identity.Next();
        public GridItem m_gridItem; public Animation? m_animationController;
        public readonly List<string> Events = new();
        public UnityEngine.Transform transform = new UnityEngine.GameObject("controller").transform;
        public struct SpriteGroup
        {
            public Il2CppSystem.Collections.Generic.List<UnityEngine.Sprite>? sprites;
            public UnityEngine.SpriteRenderer? spriteRenderer;
            public UnityEngine.Transform? groupTransform;
        }
        public Il2CppSystem.Collections.Generic.List<SpriteGroup> m_spriteGroups = new() { new() };
        public GridItemController(GridItem item) { m_gridItem = item; m_animationController = new(Events); }
        public void Hide() { Events.Add("Hide"); }
        public void Serialize() { if (m_animationController is { m_animationState: null }) throw new NullReferenceException("unstarted shim animation"); Events.Add("Serialize"); }
        public static bool operator !(GridItemController value) => false;
    }
}
namespace Il2CppSystem.Collections.Generic { sealed class List<T> : System.Collections.Generic.List<T> { } }
namespace UnityEngine
{
    sealed class Sprite { }
    sealed class SpriteRenderer { public static bool operator !(SpriteRenderer value) => false; }
    sealed class Transform
    {
        public GameObject gameObject;
        public Transform(GameObject owner) { gameObject = owner; }
        public void SetParent(Transform parent, bool worldPositionStays) { }
        public static bool operator !(Transform value) => false;
    }
    sealed class GameObject
    {
        public Transform transform;
        public GameObject(string name) { transform = new(this); }
        public T AddComponent<T>() where T : new() => new();
    }
}
namespace FusionMod
{
    static class FusionTime
    {
        public const double NativeTickSeconds = 1d / 120;
        public static bool Capture(Board board) { var clock = R.Get(board, R.Kind.Clock, R.ClockEntity, 120)!; clock.Flags = board.mMainCounter; return true; }
    }
    static class ScaredContent { public static bool Playing(Board board) => true; }
    static class FusionProjectileRecords { public static bool Loading; public static bool HasSession(Board board) => true; public static long Generation(Board board) => 1; }
    static class FusionProjectileLoad { public static bool LegacyNonce(GridItem item) => item.mSunCount == -521095; }
    sealed class FusionMod { public static readonly FusionMod Instance = new(); public Log LoggerInstance = new(); public sealed class Log { public void Error(string text) {} } }
}
