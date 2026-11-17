// Native boundaries are simulated. The production map, native-record router,
// migration and load hooks are compiled unchanged by this executable.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] sealed class HarmonyPatch : Attribute
    { public HarmonyPatch() {} public HarmonyPatch(Type type, string method) {} }
    [AttributeUsage(AttributeTargets.Method)] sealed class HarmonyPriority : Attribute
    { public HarmonyPriority(int priority) {} }
    static class Priority { public const int First = 800, Last = 0; }
}
namespace Il2CppInterop.Runtime.InteropTypes.Arrays
{
    sealed class Il2CppStructArray<T>
    {
        readonly T[] values;
        public Il2CppStructArray(T[] values) { this.values = values.ToArray(); }
        public int Length => values.Length;
        public T this[int index] { get => values[index]; set => values[index] = value; }
        public T[] ToArray() => values.ToArray();
    }
}
namespace Il2CppReloaded.Services
{
    class NativeObject
    {
        static int next;
        public IntPtr Pointer = (IntPtr)Interlocked.Increment(ref next);
        public T? TryCast<T>() where T : class => this as T;
    }
    sealed class PlatformService : NativeObject
    {
        public NativeObject Platform = new Il2CppOOI.Platforms.Steam.SteamPlatform();
        public bool SaveFile() => true;
        public bool DeleteFile() => true;
    }
    sealed class Profile { public string mGuid = "9daa879e-4640-43be-b966-088899de8c55"; }
    sealed class Users { public Profile? ActiveUserProfile = new(); }
    sealed class SerializationService
    {
        public Il2CppReloaded.Gameplay.App? App;
        public NativeObject m_platformService = new PlatformService();
        public Users? m_userService = new();
        public List<Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<byte>> m_saveBuffer = new();
        public string FileName = "test.ig.dat";
        public int Category;
        public int GetCurrentSaveCategory() => Category;
        public string GetSaveFilename() => FileName;
        public bool Deserialize() => true;
        public void Serialize() {}
        public static class SaveHeader { public const int MAGIC_NUMBER = 0x1234567; }
    }
}
namespace Il2CppOOI.Platforms.Steam
{
    sealed class SteamPlatform : Il2CppReloaded.Services.NativeObject
    {
        public string Root = "";
        public string _platformFilePath(string file) => Path.Combine(Root, file);
    }
}
namespace Il2CppReloaded.Gameplay
{
    sealed class App { public Board? Board; }
    enum GridItemType { ZenTool, Other }
    enum GridItemState { Normal, Other }
    enum ProjectileType { Pea, Melon }
    enum ProjectileMotion { Straight, Lobbed, Star, Homing }
    sealed class NativeArray<T> where T : class
    {
        readonly Func<T, uint> id;
        readonly Func<T, bool> dead;
        public readonly List<T> Values = new();
        public Func<T>? Allocate;
        public int Allocations;
        public int Count => Values.Count;
        public T this[int index] => Values[index];
        public NativeArray(Func<T, uint> id, Func<T, bool> dead) { this.id = id; this.dead = dead; }
        public T? DataArrayTryToGet(uint key) => Values.FirstOrDefault(value => id(value) == key && !dead(value));
        public T? DataArrayAlloc() { ++Allocations; if (Values.Count >= 128 || Allocate == null) return null; var item = Allocate(); Values.Add(item); return item; }
    }
    sealed class Board : Il2CppReloaded.Services.NativeObject
    {
        public App mApp;
        public bool Playing = true;
        public int mMainCounter;
        public NativeArray<Projectile> m_projectiles = new(p => p.DataID, p => p.mDead);
        public NativeArray<GridItem> m_gridItems = new(p => p.DataID, p => p.mDead);
        public Board()
        {
            mApp = new() { Board = this };
            m_gridItems.Allocate = () => new() { mBoard = this, DataID = (uint)m_gridItems.Count + 1 };
        }
        public int GetNumRows() => 5;
        public Projectile AddProjectile(uint id)
        { var shot = new Projectile { DataID = id, mBoard = this }; m_projectiles.Values.Add(shot); return shot; }
        public void InitLevel() {} public void DisposeBoard() {} public void Serialize() {}
    }
    sealed class Controller
    {
        public bool Dead;
        public int Deaths;
        public void Die() { ++Deaths; Dead = true; }
    }
    sealed class Projectile : Il2CppReloaded.Services.NativeObject
    {
        public Board mBoard = null!;
        public uint DataID;
        public bool mDead;
        public float mX = 200, mY = 300, mCobTargetX;
        public int mRow = 2, mRenderOrder = 17, mCobTargetRow;
        public ProjectileType mProjectileType;
        public ProjectileMotion mMotionType;
        public float mVelX, mVelY, mVelZ, mAccZ, mPosZ, mShadowY;
        public int mProjectileAge, mWidth, mHeight, mTargetZombieID, mHitTorchwoodGridX, mPortalX;
        public Controller? mController = new();
        public int Initializations, VisualRemovals;
        public void Die() { mDead = true; mController?.Die(); }
        public void ProjectileInitialize(int x, int y, int order, int row, ProjectileType type)
        {
            if (mCobTargetX != 0 || mCobTargetRow != 0 || mProjectileType != ProjectileType.Pea || mMotionType != ProjectileMotion.Straight)
                throw new Exception("Fusion identity reached native controller creation.");
            if (mController is { Dead: false }) throw new Exception("Previous controller was not retired.");
            ++Initializations; mController = new();
            mX = x; mY = y; mRenderOrder = order; mRow = row; mProjectileType = type;
            mMotionType = ProjectileMotion.Straight;
            mVelX = mVelY = mVelZ = mAccZ = mPosZ = 0;
            mProjectileAge = mTargetZombieID = 0; mHitTorchwoodGridX = mPortalX = -1;
            mWidth = mHeight = 40; mShadowY = row * 100 + 67;
        }
    }
    sealed class GridItem : Il2CppReloaded.Services.NativeObject
    {
        public Board mBoard = null!;
        public uint DataID;
        public bool mDead, mVisible;
        public GridItemType mGridItemType;
        public GridItemState mGridItemState;
        public int mSunCount, mGridItemCounter, mGridX, mGridY, mX, mY;
        public float mPosX, mPosY, mGoalX, mGoalY;
        public void Init(App app, GridItemType type) { mBoard = app.Board!; mGridItemType = type; }
        public void GridItemDie() => mDead = true;
        public void Update() {} public void DrawGridItem() {}
    }
}
namespace FusionMod
{
    static class ScaredContent { public static bool Playing(Il2CppReloaded.Gameplay.Board board) => board.Playing; }
    static class FusionTime
    {
        public const double NativeTickSeconds = .01;
        public static bool CaptureResult = true;
        public static bool Capture(Il2CppReloaded.Gameplay.Board board) => CaptureResult;
    }
    // The marker partition itself is independently source reviewed; these tests
    // exercise lifecycle and routing without importing dozens of plant owners.
    static class FusionProjectileDomains
    {
        public static bool Marked(Il2CppReloaded.Gameplay.Projectile shot) => shot.mCobTargetX == -41001 || shot.mCobTargetX == -41021;
    }
    static class ProjectileCaches
    {
        public static readonly string[] Owners = { "Blaze", "Iron", "Rolling", "Split", "Solar", "Poison", "Visual" };
        static readonly Dictionary<(string, IntPtr), (IntPtr Board, uint Id)> entries = new();
        public static void Seed(Il2CppReloaded.Gameplay.Projectile shot)
        { foreach (string owner in Owners) entries[(owner, shot.Pointer)] = (shot.mBoard.Pointer, shot.DataID); }
        public static bool Any(Il2CppReloaded.Gameplay.Projectile shot) => Owners.Any(owner => entries.ContainsKey((owner, shot.Pointer)));
        public static void Forget(string owner, Il2CppReloaded.Gameplay.Projectile shot) => entries.Remove((owner, shot.Pointer));
        public static void ForgetBoard(string owner, Il2CppReloaded.Gameplay.Board board)
        { foreach (var key in entries.Where(e => e.Key.Item1 == owner && e.Value.Board == board.Pointer).Select(e => e.Key).ToArray()) entries.Remove(key); }
    }
    static class RollingProjectiles
    {
        public static void Forget(Il2CppReloaded.Gameplay.Projectile shot) { ProjectileCaches.Forget("Rolling", shot); ++shot.VisualRemovals; }
        public static void ForgetBoard(Il2CppReloaded.Gameplay.Board board) => ProjectileCaches.ForgetBoard("Rolling", board);
    }
    sealed class FusionMod
    {
        public static readonly FusionMod Instance = new();
        public readonly Log LoggerInstance = new();
        public sealed class Log { public void Error(string message) {} public void Msg(string message) {} }
    }
}

namespace FusionMod { static class BlazeShooters { public static void Forget(Il2CppReloaded.Gameplay.Projectile shot) => ProjectileCaches.Forget("Blaze", shot); public static void ForgetBoard(Il2CppReloaded.Gameplay.Board board) => ProjectileCaches.ForgetBoard("Blaze", board); } }

namespace FusionMod { static class IronProjectiles { public static void Forget(Il2CppReloaded.Gameplay.Projectile shot) => ProjectileCaches.Forget("Iron", shot); public static void ForgetBoard(Il2CppReloaded.Gameplay.Board board) => ProjectileCaches.ForgetBoard("Iron", board); } }

namespace FusionMod { static class SplitPeaProjectiles { public static void Forget(Il2CppReloaded.Gameplay.Projectile shot) => ProjectileCaches.Forget("Split", shot); public static void ForgetBoard(Il2CppReloaded.Gameplay.Board board) => ProjectileCaches.ForgetBoard("Split", board); } }

namespace FusionMod { static class SolarShooter { public static void Forget(Il2CppReloaded.Gameplay.Projectile shot) => ProjectileCaches.Forget("Solar", shot); public static void ForgetProjectiles(Il2CppReloaded.Gameplay.Board board) => ProjectileCaches.ForgetBoard("Solar", board); } }

namespace FusionMod { static class PoisonShooters { public static void Forget(Il2CppReloaded.Gameplay.Projectile shot) => ProjectileCaches.Forget("Poison", shot); public static void ForgetProjectiles(Il2CppReloaded.Gameplay.Board board) => ProjectileCaches.ForgetBoard("Poison", board); } }

namespace FusionMod { static class ImportedPeaVisuals { public static void ForgetProjectiles(Il2CppReloaded.Gameplay.Board board) => ProjectileCaches.ForgetBoard("Visual", board); } }
