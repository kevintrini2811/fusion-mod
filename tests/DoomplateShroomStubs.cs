// Native boundary models only. Production clock, records, crater timing and
// Doomplate/Doom damage owners compile directly; no player or profile is loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] internal sealed class HarmonyPatch : Attribute
    { internal HarmonyPatch(Type type, string method) {} }
    [AttributeUsage(AttributeTargets.Method)] internal sealed class HarmonyPriority : Attribute
    { internal HarmonyPriority(int priority) {} }
    internal static class Priority { internal const int First = 800, Last = 0; }
}
namespace Il2CppReloaded.Constants
{ internal static class Gameplay { internal static double TIME_FOR_UPDATE = .01; } }
namespace Il2CppReloaded.Services
{
    internal sealed class LoadedApp { internal Il2CppReloaded.Gameplay.Board? Board; }
    internal sealed class SerializationService { internal LoadedApp? App; public bool Deserialize() => true; }
}
namespace Il2CppReloaded.Gameplay
{
    internal enum SeedType { None, Doomshroom, Tallnut, Bucket, Wallnut }
    internal enum GridItemType { ZenTool, Crater }
    internal enum GridItemState { Normal }
    internal enum PlantOnBungeeState { NotOnBungee, OnBungee }
    internal enum GameScenes { Playing, Loading }
    internal enum GameMode { Adventure, ChallengeZenGarden }
    internal enum ParticleEffect { Doom }
    internal enum FoleyType { Explosion, JalapenoIgnite }
    [Flags] internal enum DamageFlags { HitsShieldAndBody = 1, DoesntLeaveBody = 2 }
    [Flags] internal enum DamageRangeFlags { Ground = 1, Flying = 2, Submerged = 4, Dog = 8, OffGround = 16, Underground = 32 }
    internal readonly record struct Rect(float xMin, float yMin, float xMax, float yMax);
    internal sealed class Sounds { internal int Calls; internal void PlayFoley(FoleyType sound) => ++Calls; }
    internal sealed class App
    {
        internal GameScenes GameScene = GameScenes.Playing;
        internal GameMode GameMode;
        internal readonly Sounds SoundSystem = new();
        internal int Particles;
        internal void AddTodParticle(float x, float y, int order, ParticleEffect effect) => ++Particles;
    }
    internal sealed class Pool : List<GridItem>
    { internal GridItem DataArrayAlloc() { var item = new GridItem(); Add(item); return item; } }
    internal sealed class Board
    {
        private static int next;
        internal IntPtr Pointer = (IntPtr)(++next);
        internal bool mPaused;
        internal int mMainCounter, Craters;
        internal readonly App mApp = new();
        internal readonly Pool m_gridItems = new();
        internal readonly List<Plant> m_plants = new();
        internal readonly List<Zombie> m_zombies = new();
        internal readonly Queue<int> Random = new();
        internal int Rand(int limit) => Random.Count == 0 ? 2000 % limit : Random.Dequeue() % limit;
        internal int GetNumRows() => 5;
        internal GridItem AddACrater(int col, int row)
        {
            ++Craters;
            var item = new GridItem { mBoard = this, mGridItemType = GridItemType.Crater, mGridX = col, mGridY = row };
            m_gridItems.Add(item); return item;
        }
        internal void ShakeBoard(int x, int y) {}
        public void UpdateGame() {} public void InitLevel() {} public void InitSurvivalStage() {}
        public void DisposeBoard() { FusionMod.DoomplateDisposePatch.Prefix(this); } public void Serialize() {}
    }
    internal sealed class Plant
    {
        private static uint next;
        internal uint DataID = ++next;
        internal IntPtr Pointer => (IntPtr)DataID;
        internal Board mBoard = null!;
        internal App mApp => mBoard.mApp;
        internal FusionMod.FusionPlant Identity = FusionMod.FusionPlant.DoomplateShroom;
        internal int mPlantHealth, mPlantMaxHealth, mDoSpecialCountdown, mStartRow, mTargetX, mTargetY, mLaunchCounter;
        internal int mPlantCol = 3, mRow = 2, mRenderOrder = 100;
        internal float mX = 240, mY = 200, mWidth = 80, mHeight = 80;
        internal bool mDead, mIsOnBoard = true, mSquished, mIsAsleep, OffGround, Veto;
        internal float ShadowX = 271, ShadowY = 279;
        internal PlantOnBungeeState mOnBungeeState;
        internal Action? OnDie;
        internal bool NotOnGround() => OffGround;
        public void Die() { OnDie?.Invoke(); if (!Veto) { mDead = true; mIsOnBoard = false; } }
        public void PlantInitialize() {} public void UpdateAbilities() {} public void UpdateShooting() {} public void DoSpecial() {} public void Squish() {}
    }
    internal sealed class Zombie
    {
        private static uint next;
        internal uint DataID = ++next;
        internal IntPtr Pointer => (IntPtr)DataID;
        internal Board mBoard = null!;
        internal bool mDead, mMindControlled, Dying;
        internal int mBodyHealth = 1000, Received, mRow = 2;
        internal float X = 300, Y = 200;
        internal DamageFlags LastFlags;
        internal DamageRangeFlags Category = DamageRangeFlags.Ground;
        internal Action? OnDamage;
        internal bool IsDeadOrDying() => Dying || mBodyHealth <= 0;
        internal bool EffectedByDamage(DamageRangeFlags flags) => (flags & Category) != 0;
        internal Rect GetZombieRect() => new(X, Y, X + 30, Y + 60);
        internal void RemoveColdEffects() {}
        internal void TakeDamage(int damage, DamageFlags flags)
        { Received += damage; mBodyHealth -= damage; LastFlags = flags; OnDamage?.Invoke(); }
    }
    internal sealed class Projectile { internal Board mBoard = null!; internal IntPtr Pointer; internal uint DataID; }
    internal sealed class GridItem
    {
        private static uint next;
        internal uint DataID = ++next;
        internal Board mBoard = null!;
        internal bool mDead;
        internal GridItemType mGridItemType;
        internal GridItemState mGridItemState;
        internal int mGridX, mGridY, mGridItemCounter, mSunCount, mX, mY;
        internal float mPosX, mPosY, mGoalX, mGoalY;
        internal void Init(App app, GridItemType type) { mGridItemType = type; }
        internal void GridItemDie() { mDead = true; }
        public void Update() {} public void DrawGridItem() {}
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal readonly record struct FusionPlant(int Id)
    {
        internal static readonly FusionPlant DoomplateShroom = new(1239), Bucket = new(224), DoomNut = new(100), Other = new(0);
        internal static FusionPlant From(Plant plant) => plant.Identity;
    }
    internal static class MineFusions
    {
        internal static bool Active(Plant plant) => plant.mBoard != null && !plant.mBoard.mPaused
            && plant.mBoard.Pointer != CherryBombPlants.DisposingBoard && !plant.mDead
            && plant.mIsOnBoard && !plant.mSquished && !plant.NotOnGround()
            && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee
            && plant.mApp.GameScene == GameScenes.Playing && plant.mApp.GameMode != GameMode.ChallengeZenGarden;
    }
    internal static class SquashFusions
    {
        internal static bool Eligible(Board board, Zombie zombie, DamageRangeFlags flags) => zombie.mBoard == board
            && !zombie.mDead && !zombie.IsDeadOrDying() && !zombie.mMindControlled && zombie.EffectedByDamage(flags);
    }
    internal static class BucketDrops
    {
        internal readonly record struct Drop(Board Board, int Row, float X, float Y, FusionPlant Identity);
        internal static readonly List<Drop> Drops = new();
        internal static void Add(Board board, int row, float x, float y, FusionPlant identity) => Drops.Add(new(board, row, x, y, identity));
    }
    internal static class CherryBombPlants { internal static IntPtr DisposingBoard; }
    internal static class ReactivePots
    { internal static void Fire(Board board, int row) {} internal static bool PreventCrater(Board board, int col, int row) => false; }
    internal static class PumpkinSupport { internal static void FireEnergy(Board board, int row, int damage) {} }
    internal static class IceInstantFusions { internal static void Freeze(Zombie zombie, int ticks) {} }
    internal static class NativeCombatVisuals { internal static void Fire(Board board, int row, int order) {} }
    internal static class ImportedPeaVisuals
    { internal static (float X, float Y) DoomplateOrigin(Plant plant) => (plant.ShadowX, plant.ShadowY); }
    internal static class ScaredContent { internal static bool Playing(Board board) => board.mApp.GameScene == GameScenes.Playing; }
    internal sealed class FusionMod
    {
        internal static readonly FusionMod Instance = new();
        internal readonly Logger LoggerInstance = new();
        internal sealed class Logger { internal void Error(string message) { throw new Exception(message); } }
    }
}
