// Native boundary models only. The real Squish leaf, clock and clock-record owners compile directly.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] internal sealed class HarmonyPatch : Attribute
    { internal HarmonyPatch() {} internal HarmonyPatch(Type type, string method) {} }
    [AttributeUsage(AttributeTargets.Method)] internal sealed class HarmonyPriority : Attribute
    { internal HarmonyPriority(int priority) {} }
    internal static class AccessTools { internal static System.Reflection.MethodInfo Method(string name) => typeof(LawnGlove.LawnGloveMod).GetMethod(name.Split(':')[1],System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!; internal static System.Reflection.MethodInfo Method(Type type,string name) => type.GetMethod(name,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Public)!; }
    internal static class Priority { internal const int First = 800, Last = 0; }
}
namespace UnityEngine { internal readonly record struct Rect(float x, float y, float width, float height); }
namespace Il2CppReloaded.Constants
{ internal static class Gameplay { internal static double TIME_FOR_UPDATE = .01; } }
namespace Il2CppReloaded.Services
{
    internal sealed class LoadedApp { internal Il2CppReloaded.Gameplay.Board? Board; }
    internal sealed class SerializationService { internal LoadedApp? App; public bool Deserialize() => true; }
}
namespace Il2CppReloaded.Gameplay
{
    internal enum SeedType { None, Doomshroom, Tallnut, Bucket, Wallnut, Puffshroom, Squash, Lilypad }
    internal enum ZombieType { Pogo, Normal, Gargantuar, RedeyeGargantuar, Boss, Zamboni, Catapult, Gravestone, Target }
    internal enum ZombiePhase { ZombieNormal, ZombieDying, PogoBouncing, PolevaulterInVault, DolphinInJump,
        ImpGettingThrown,BalloonFlying,BalloonPopping,DiggerTunneling,DiggerTunnelingPauseWithoutAxe,
        DiggerRising,DiggerRiseWithoutAxe,PogoHighBounce1,PogoHighBounce2,PogoHighBounce3,PogoHighBounce4,
        PogoHighBounce5,PogoHighBounce6,PogoForwardBounce2,PogoForwardBounce7,BungeeDiving,BungeeDivingScreaming,BungeeRising }
    internal enum PlantPriority { Normal, DiggingOrder=1, BungeeOrder=2 }
    internal enum CursorType { Normal, Shovel, Glove, PlantFromBank }
    internal sealed class Cursor { internal CursorType CursorType; }
    internal enum ProjectileType { Basketball, ZombiePea }
    internal enum GridItemType { ZenTool, Crater }
    internal enum GridItemState { Normal }
    internal enum PlantOnBungeeState { NotOnBungee, OnBungee }
    internal enum GameScenes { Playing, Loading }
    internal enum GameMode { Adventure, ChallengeZenGarden }
    internal enum ParticleEffect { Doom, PlantingPool }
    internal enum FoleyType { Explosion, JalapenoIgnite, Thump, ZombieSplash }
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
    // SDK DataArray exposes Count/indexing, not IEnumerable or LINQ.
    internal sealed class DataArray<T>
    {
        private readonly List<T> items = new();
        internal int Count => items.Count;
        internal T this[int index] => items[index];
        internal void Add(T item) => items.Add(item);
        internal bool Remove(T item) => items.Remove(item);
    }
    internal sealed class Pool : List<GridItem>
    { internal GridItem DataArrayAlloc() { var item = new GridItem(); Add(item); return item; } }
    internal sealed class Board
    {
        private static int next;
        internal IntPtr Pointer = (IntPtr)(++next);
        internal bool mPaused, Disposed;
        internal Cursor[] CursorObjects = {new(),new()};
        internal readonly HashSet<(int,int)> Water = new();
        internal readonly Dictionary<(int,int), Plant> Supports = new();
        internal float GridToPixelX(int col,int row) => 40 + col * 80;
        internal float GridToPixelY(int col,int row) => 80 + row * 100;
        internal float GetPosYBasedOnRow(float x,int row) => GridToPixelY(0,row);
        internal int PixelToGridX(float x,float y) => (int)MathF.Floor((x - 40) / 80);
        internal bool IsPoolSquare(int col,int row) => Water.Contains((col,row));
        internal Layers GetPlantsOnLawn(int col,int row) => new() { UnderPlant = Supports.GetValueOrDefault((col,row)) };
        internal int mMainCounter, Craters;
        internal readonly App mApp = new();
        internal readonly Pool m_gridItems = new();
        internal readonly DataArray<Plant> m_plants = new();
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
        internal int PixelToGridY(int x,int y) => (y-80)/100;
        internal void ToolHitTest() {}
        internal void MouseDownWithTool(int x,int y,int theClickCount,CursorType theCursorType,int playerIndex) {}
        internal void CanPlantAt() {}
        internal Plant? GetTopPlantAt() => null;
        internal void ShakeBoard(int x, int y) {}
        public void UpdateGame() {} public void InitLevel() {} public void InitSurvivalStage() {}
        public void DisposeBoard() { Disposed = true; FusionMod.FusionTime.Dispose(this); } public void Serialize() {}
    }
    internal sealed class Layers { internal Plant? UnderPlant; }
    internal sealed class Plant
    {
        private static uint next;
        internal uint DataID = ++next;
        internal IntPtr Pointer = (IntPtr)next;
        internal Board mBoard = null!;
        internal App mApp => mBoard.mApp;
        internal FusionMod.FusionPlant Identity = FusionMod.FusionPlant.Squish;
        internal int mPlantHealth, mPlantMaxHealth, mDoSpecialCountdown, mStartRow, mTargetX, mTargetY, mLaunchCounter;
        internal int mShootingCounter, mLaunchRate, mStateCountdown, ReanimCalls;
        internal SeedType mSeedType = SeedType.Puffshroom;
        internal Action? OnReanim;
        internal void UpdateReanim() { ++ReanimCalls; OnReanim?.Invoke(); }
        internal int mPlantCol = 3, mRow = 2, mRenderOrder = 100;
        internal float mX = 240, mY = 200, mWidth = 80, mHeight = 80;
        internal bool mDead, mIsOnBoard = true, mSquished, mIsAsleep, OffGround, Veto;
        internal float ShadowX = 271, ShadowY = 279;
        internal PlantOnBungeeState mOnBungeeState;
        internal Action? OnDie;
        internal UnityEngine.Rect GetPlantRect() => new(mX,mY,mWidth,mHeight);
        internal bool NotOnGround() => OffGround || mSquished;
        public void Die() { OnDie?.Invoke(); if (!Veto) { mDead = true; mIsOnBoard = false; } }
        public void PlantInitialize() {} public void UpdateAbilities() {} public void UpdateShooting() {} public void DoSpecial() {} public void Squish() {}
    }
    internal sealed class Zombie
    {
        private static uint next;
        internal uint DataID = ++next;
        internal IntPtr Pointer = (IntPtr)next;
        internal Board mBoard = null!;
        internal bool mDead, mMindControlled, Dying, mIsEating, Chilled;
        internal ZombieType mZombieType;
        internal ZombiePhase mZombiePhase;
        internal float Height;
        internal int mIceTrapCounter, mButteredCounter;
        internal float mPosX, mPosY, mWidth = 30, mHeight = 60, mVelX;
        internal bool IsMovingAtChilledSpeed() => Chilled;
        internal int mBodyHealth = 1000, Received, mRow = 2;
        internal float X = 300, Y = 200;
        internal DamageFlags LastFlags;
        internal DamageRangeFlags Category = DamageRangeFlags.Ground;
        internal Action? OnDamage;
        internal bool IsDeadOrDying() => Dying || mBodyHealth <= 0;
        internal bool EffectedByDamage(DamageRangeFlags flags) => !(mZombieType == ZombieType.Pogo && Height > 0) && (flags & Category) != 0;
        internal Rect GetZombieRect() => new(mPosX, mPosY, mPosX + mWidth, mPosY + mHeight);
        internal int mTargetCol=3;
        internal void RemoveColdEffects() {}
        internal void FindCatapultTarget() {} internal void BossCanStompRow() {}
        internal void BungeeStealTarget() {} internal void UpdateZombieBungee() {}
        internal void BossStompAttack() {} internal void PlayZombieReanim() {}
        internal void TakeDamage(int damage, DamageFlags flags)
        { Received += damage; mBodyHealth -= damage; LastFlags = flags; OnDamage?.Invoke(); }
    }
    internal sealed class Projectile { internal Board mBoard = null!; internal IntPtr Pointer; internal uint DataID; internal bool mDead; internal ProjectileType mProjectileType; internal Plant? FindCollisionTargetPlant() => null; }
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
        internal static readonly FusionPlant Squish = new(1370), Nibble = new(1369), DoomplateShroom = new(1239), Bucket = new(224), DoomNut = new(100), Other = new(0);
        internal static FusionPlant From(Plant plant) => plant.Identity;
    }
    // Other leaf is a boundary; the real shared owner and Squish compile directly.
    internal static class NibbleShroom
    {
        internal static bool Owns(Plant p) => p.Identity==FusionPlant.Nibble;
        internal static bool Retained(Plant p) => Owns(p)&&!p.mDead&&!p.mSquished;
        internal static bool Activated(Plant p) => Owns(p);
        internal static bool Live(Plant p) => Retained(p);
        internal static bool DamageImmune(Plant p) => false;
        internal static void Initialize(Plant p) {}
        internal static void Tick(Plant p) {}
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
    internal static class ScaredContent { internal static bool Playing(Board board) => !board.Disposed && board.mApp.GameScene == GameScenes.Playing && board.mApp.GameMode != GameMode.ChallengeZenGarden; }

    internal static class ShroomSlots
    {
        internal static (float X,float Y) Offset(int slot) => slot switch { 1 => (24,12), 2 => (-24,12), _ => (0,-16) };
        internal static int Slot(Plant plant)
        {
            float x = plant.mX - plant.mBoard.GridToPixelX(plant.mPlantCol,plant.mRow);
            float y = plant.mY - plant.mBoard.GridToPixelY(plant.mPlantCol,plant.mRow);
            return Enumerable.Range(0,3).OrderBy(i => MathF.Pow(x-Offset(i).X,2)+MathF.Pow(y-Offset(i).Y,2)).First();
        }
        internal static bool IsSmall(FusionPlant identity) => identity != FusionPlant.Other;
        internal static List<Plant> Plants(Board board,int col,int row)
        {
            var result=new List<Plant>();
            for(int i=0;i<board.m_plants.Count;++i) {
                var p=board.m_plants[i];
                if(!p.mDead&&p.mIsOnBoard&&p.mPlantCol==col&&p.mRow==row) result.Add(p);
            }
            return result;
        }
        internal static bool CanAdd(Board board,FusionPlant identity,int col,int row,Plant? source=null)
        { var others=Plants(board,col,row).Where(p=>!ReferenceEquals(p,source)).ToArray(); return others.Length<3 && others.All(p=>IsSmall(p.Identity)); }
    }
    internal static class WeedFusions
    { internal sealed record Crush(Zombie Zombie,Board Board,uint ZombieId); internal static Crush? CurrentCrush; }
    internal static class PumpkinFusions
    { internal static Func<Plant,bool>? Absorbing; internal static bool AbsorbCrush(Plant p) => Absorbing?.Invoke(p) ?? false; }
    internal static class PotDefense
    { internal static int Numerator=1,Denominator=1; internal static int Damage(Plant p,int damage) => damage*Numerator/Denominator; }
    internal sealed class FusionMod
    {
        internal static readonly FusionMod Instance = new();
        internal readonly Logger LoggerInstance = new();
        internal sealed class Logger { internal void Error(string message) { throw new Exception(message); } }
    }
}

namespace LawnGlove {
    internal sealed class LawnGloveMod {
        internal static Il2CppReloaded.Gameplay.Plant? Carried;
        internal static Il2CppReloaded.Gameplay.Plant? GetCarriedPlant(Il2CppReloaded.Gameplay.Board b) => Carried;
        internal bool HandleBoardClick(Il2CppReloaded.Gameplay.Board board,int x,int y,int clicks,int player) => true;
    }
}
