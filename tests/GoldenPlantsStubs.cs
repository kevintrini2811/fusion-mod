// Offline doubles. These never load the game, Unity or native assemblies.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch(Type type, string method) { } }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute
    { public HarmonyPriority(int priority) { } }
    public static class Priority { public const int First = 800; }
}
namespace UnityEngine.InputSystem
{
    public sealed class Key { public bool wasPressedThisFrame; }
    public sealed class Keyboard { public static Keyboard? current; public Key escapeKey = new(); }
}
namespace Il2CppReloaded.Services { public sealed class SerializationService { public void Serialize() { } } }
namespace LawnGlove
{
    public static class LawnGloveMod
    { public static Il2CppReloaded.Gameplay.Plant? Carried; public static Il2CppReloaded.Gameplay.Plant? GetCarriedPlant(Il2CppReloaded.Gameplay.Board board) => Carried; }
}
namespace Il2CppReloaded.Gameplay
{
    public enum PlantOnBungeeState { NotOnBungee, OnBungee }
    public enum GameScenes { Playing, Loading }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public enum CursorType { Normal, MoneySign, PlantFromBank }
    public enum GridItemType { ZenTool }
    public enum GridItemState { Normal }
    public enum CoinType { Silver, Gold, Diamond, Sun, LargeSun }
    public enum CoinMotion { FromPlant }
    public enum PlantingReason { Ok, NotHere }
    public sealed class App { public GameScenes GameScene; public GameMode GameMode; }
    public sealed class Sun { public int Amount; }
    public sealed class Cursor { public CursorType CursorType; }
    public sealed class Bank { public bool ContainsPoint(int x, int y) => y < 0; }
    public sealed class Layers { public Plant? NormalPlant; }
    public sealed class Pool<T> : List<T> where T : new()
    {
        public bool Fail;
        public T? DataArrayAlloc() { if (Fail) return default; var value = new T(); Add(value); return value; }
    }
    public sealed class Board
    {
        private static int next;
        public IntPtr Pointer = (IntPtr)(++next);
        public App mApp = new(); public bool mPaused, mShowCoinBank, FailPlant, FailCoin;
        public List<Plant> m_plants = new(); public Pool<GridItem> m_gridItems = new();
        public Cursor mCursorObject = new(); public Bank mSeedBank = new(); public Sun[] mSunMoney = { new() };
        public List<Coin> Coins = new();
        public Coin? AddCoin(float x, float y, CoinType type, CoinMotion motion)
        { if (FailCoin) return null; var c = new Coin { mBoard = this, mType = type }; Coins.Add(c); return c; }
        public void AddSunMoney(int theAmount, int playerIndex) { mSunMoney[playerIndex].Amount += theAmount; }
        public Plant? AddPlant(int col, int row, SeedType type, SeedType marker)
        { if (FailPlant) return null; var p = new Plant { mBoard = this, mSeedType = type, mImitaterType = marker, mPlantCol = col, mRow = row }; m_plants.Add(p); FusionMod.GoldenPlants.Initialize(p); return p; }
        public Layers GetPlantsOnLawn(int col, int row) => new() { NormalPlant = m_plants.FirstOrDefault(p => !p.mDead && p.mPlantCol == col && p.mRow == row) };
        public int PixelToGridX(int x, int y) => x; public int PixelToGridY(int x, int y) => y; public int GetNumRows() => 5;
        public void ClearCursor(bool ignoreControlType, int playerIndex) { mCursorObject.CursorType = CursorType.Normal; }
        public void ShowCoinBank() { mShowCoinBank = true; }
        public void UpdateGame() { } public void DisposeBoard() { } public void MouseDown() { }
    }
    public sealed class Plant
    {
        private static int next; public IntPtr Pointer = (IntPtr)(++next);
        public bool mDead, mIsOnBoard = true, mSquished, OffGround;
        public PlantOnBungeeState mOnBungeeState;
        public Board mBoard = new(); public SeedType mSeedType, mImitaterType = SeedType.None;
        public int mPlantHealth, mPlantMaxHealth, mLaunchCounter, mStartRow, mTargetX, mTargetY,
            mShootingCounter, mDoSpecialCountdown, mPlantCol, mRow;
        public float mX, mY;
        public bool NotOnGround() => OffGround;
        public void UpdateReanim() { } public void UpdateReanimColor() { }
        public void PlantInitialize() { } public void UpdateAbilities() { }
    }
    public sealed class Coin { public Board? mBoard; public CoinType mType; public void ScoreCoin() { } }
    public sealed class GridItem
    {
        public GridItemType mGridItemType; public GridItemState mGridItemState;
        public int mSunCount, mGridX, mGridY, mGridItemCounter; public float mX, mY; public bool mDead;
        public void Init(App app, GridItemType type) { mGridItemType = type; }
        public void GridItemDie() { mDead = true; } public void Update() { }
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal static class CherryBombPlants { internal static IntPtr DisposingBoard; }
    internal static class FusionService
    { internal static bool Fail; internal static PlantingReason CheckFusionPlacement(Board board, Plant target, FusionPlant result) => Fail ? PlantingReason.NotHere : PlantingReason.Ok; }
    internal static class PlayerPlantingScope { internal static void Consume(Plant plant) { plant.mDead = true; } }
    internal static class DebugMode
    { internal static bool Free; internal static bool Owns(Board board) => Free; internal static void CancelZombiePlacement() { } }
    internal sealed class GoldenBeanUi : IDisposable
    { internal static GoldenBeanUi? TryCreate() => null; internal void Update(bool visible, bool selected, bool debug, string money) { } public void Dispose() { } }
    internal sealed class Logger { internal void Error(string error) { } }
    internal sealed class FusionMod { internal static FusionMod Instance = new(); internal Logger LoggerInstance = new(); }
}

namespace FusionMod { internal static class BoardSpeed { internal static void Update(bool playing) { } internal static void Clear(bool restoreSelection = true) { } } }
