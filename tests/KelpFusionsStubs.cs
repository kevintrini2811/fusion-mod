// Offline doubles. No game, Unity player, profile or save is loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch() { } public HarmonyPatch(Type type, string method) { } }
}
namespace Il2CppReloaded.Gameplay
{
    public enum PlantRowType { Normal, Pool }
    public enum GameScenes { Playing, Menu }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public enum PlantingReason { Ok, NotHere }
    public enum ZombieType { Normal, Boss, Gravestone, Target, Zamboni, Catapult, SquashHead, Football, JackInTheBox, Dancer, BackupDancer, Yeti, Gargantuar, RedeyeGargantuar, Balloon, Bungee, DolphinRider, Pogo, Bobsled, Door, TrashCan, Imp }
    public enum ZombiePhase { ZombieNormal, BalloonFlying, DiggerTunneling, DiggerTunnelingPauseWithoutAxe, ImpGettingThrown, DancerRising, BalloonPopping, BungeeAtBottom, BungeeGrabbing, DolphinIntoPool, DolphinRiding, DolphinInJump }
    public enum ZombieHeight { ZombieNormal, InToPool, OutOfPool, DraggedUnder, Falling }
    public struct ZombieDrawPosition { public float BodyY, ImageOffsetY, ClipHeight; }
    public enum GridItemType { ZenTool }
    public enum GridItemState { Normal }
    public enum ProjectileType { Pea }
    public enum ProjectileMotion { Straight }
    public enum PlantState { Notready, TanglekelpGrabbing, SunshroomSmall, SunshroomBig }
    public enum ZombieID : uint { }
    public enum ParticleEffect { Powie }
    [Flags] public enum DamageFlags { BypassesShield = 1, Spike = 32, HitsShieldAndBody = 64, DoesntLeaveBody = 128 }
    [Flags] public enum DamageRangeFlags { Ground = 1, Submerged = 2, OffGround = 4 }
    public readonly record struct Rect(float xMin, float yMin, float xMax, float yMax);
    public static class Common
    {
        public static int GetRectOverlap(ref Rect first, ref Rect second) =>
            (int)(Math.Min(first.xMax, second.xMax) - Math.Max(first.xMin, second.xMin));
    }
    public sealed class App
    { public GameScenes GameScene = GameScenes.Playing; public GameMode GameMode; public Board Board = null!; public void AddTodParticle(float x, float y, int order, ParticleEffect effect) { } }
    public sealed class Pool<T> : List<T> where T : new()
    {
        public bool Fail;
        public T? DataArrayAlloc() { if (Fail) return default; var item = new T(); Add(item); return item; }
    }
    public sealed partial class Board
    {
        public IntPtr Pointer = (IntPtr)1;
        public App mApp;
        public bool mPaused, Occupied, FailPlant;
        public int mMainCounter;
        public Pool<GridItem> m_gridItems = new();
        public List<Plant> m_plants = new();
        public List<Zombie> m_zombies = new();
        public List<Projectile> m_projectiles = new();
        public Board() { mApp = new() { Board = this }; }
        public int Rand(int range) => range / 2;
        public int GetNumRows() => 6;
        public PlantRowType[] mPlantRow = new PlantRowType[6];
        public HashSet<int> BlockedRows = new();
        public bool RowCanHaveZombies(int row) => row >= 0 && row < 6 && !BlockedRows.Contains(row);
        public bool RowCanHaveZombieType(int row, ZombieType type) => RowCanHaveZombies(row);
        public Zombie? ZombieTryToGet(ZombieID id) => m_zombies.FirstOrDefault(z => z.DataID == (uint)id);
        public int PixelToGridX(int x, int y) => x / 80;
        public int PixelToGridX(float x, float y) => (int)x / 80;
        public int GridToPixelX(int col, int row) => col * 80;
        public bool Water;
        public HashSet<(int Col, int Row)> WaterCells = new();
        public bool IsPoolSquare(int col, int row) => Water || WaterCells.Contains((col, row));
        public float GetPosYBasedOnRow(float x, int row) => row * 100;
        public Plant? AddPlant(int col, int row, SeedType seed, SeedType marker)
        {
            if (FailPlant) return null;
            var plant = new Plant { mBoard = this, mPlantCol = col, mRow = row, mSeedType = seed, mImitaterType = marker };
            m_plants.Add(plant); FusionMod.TingleKelp.Initialize(plant); return plant;
        }
        public Projectile AddProjectile(float x, float y, int order, int row, ProjectileType type)
        {
            var shot = new Projectile { mBoard = this, mPosX = x, mPosY = y, mRow = row, mProjectileType = type };
            m_projectiles.Add(shot); return shot;
        }
    }
    public sealed partial class Plant
    {
        public Board mBoard = null!;
        public SeedType mSeedType, mImitaterType;
        public bool mDead, mIsOnBoard = true, mIsAsleep;
        public bool OffGround;
        public bool NotOnGround() => OffGround;
        public PlantState mState;
        public ZombieID mTargetZombieID;
        public int mLaunchCounter, mStateCountdown;
        public uint DataID = 1;
        public int mPlantHealth, mPlantMaxHealth, mStartRow, mTargetX, mTargetY;
        public int mPlantCol, mRow, mX = 80, mY = 100, mWidth = 80, mHeight = 80, mRenderOrder;
        public void PlantInitialize() { }
        public Rect GetPlantRect() => new(mX + 10, mY, mX + mWidth - 10, mY + mHeight);
        public void UpdateAbilities() { }
        public void PlayBodyReanim() { }
        public void UpdateShooting() { }
        public void Fire() { }
        public void UpdateTanglekelp() { }
        public void Squish() { mDead = true; }
        public void Die() => mDead = true;
        public void UpdateReanim() { }
        public void UpdateReanimColor() { }
    }
    public sealed partial class Zombie
    {
        private static uint next;
        public uint DataID = ++next;
        public Board mBoard = null!;
        public ZombieType mZombieType;
        public ZombiePhase mZombiePhase;
        public bool mDead, mMindControlled, mInPool = true;
        public int mBodyHealth = 1000, mBodyMaxHealth = 1000, mHelmMaxHealth, mRow = 1, mX, mY;
        public float mPosX = 120, mPosY = 100, mAltitude, mScaleZombie = 1;
        public ZombieHeight mZombieHeight;
        public int mShieldHealth;
        public bool mHasObject, mIsBobsledLeader;
        public ZombieID[]? mFollowerZombieID;
        public Il2CppSource.Controllers.ZombieController? mController;
        public void PogoBreak(int flags) => mHasObject = false;
        public bool IsBobsledTeamWithSled() => false;
        public void BobsledCrash() { }
        public int PoolChecks;
        public List<int> Damage = new();
        public bool IsDeadOrDying() => mDead || mBodyHealth <= 0;
        public Rect GetZombieRect() => new(mPosX - 10, mPosY, mPosX + 10, mPosY + 80);
        public Rect GetZombieAttackRect() => new(mPosX - 10, mPosY, mPosX + 10, mPosY + 80);
        public void TakeDamage(int value, DamageFlags flags) { Damage.Add(value); mBodyHealth -= value; }
        public void DieWithLoot() => mDead = true;
        public void UpdateZombieWalking() { }
        // Harmony target declarations used by the shared native pool adapter.
        public void LandFlyer(DamageFlags flags) { }
        public void UpdateZombieFalling() { }
        public void StartWalkAnim(int blend) { }
        public void EatPlant(Plant plant) { }
        public bool CanTargetPlant(Plant plant) => true;
        public void CheckForPool() { PoolChecks++; }
        public ZombieID SummonBackupDancer(int row, int x) => default;
        public void UpdateZombieBackupDancer() { }
        public ZombieDrawPosition GetDrawPos() => default;
        public void DrawReanim(ZombieDrawPosition draw, int group) { }
    }
    public sealed partial class GridItem
    {
        private static uint next;
        public uint DataID = ++next;
        public IntPtr Pointer => (IntPtr)DataID;
        public Board mBoard = null!;
        public bool mDead;
        public GridItemType mGridItemType;
        public GridItemState mGridItemState;
        public int mSunCount, mGridX, mGridY, mGridItemCounter;
        public float mX, mY, mPosX, mPosY;
        public void Init(App app, GridItemType type) { mBoard = app.Board; mGridItemType = type; }
        public void GridItemDie() => mDead = true;
        public void Update() { }
        public void DrawGridItem() { }
    }
    public sealed partial class Projectile
    {
        public Board mBoard = null!;
        public bool mDead;
        public ProjectileType mProjectileType;
        public ProjectileMotion mMotionType;
        public DamageRangeFlags mDamageRangeFlags;
        public int mCobTargetX, mCobTargetRow, mRow, mProjectileAge, mHitTorchwoodGridX, mX, mY, mRenderOrder;
        public uint DataID = 1;
        public ZombieID mTargetZombieID;
        public float mPosX, mPosY, mPosZ, mVelX, mVelY, mVelZ, mAccZ, mShadowY;
        public void Die() => mDead = true;
        public void Update() { }
        public void CheckForCollision() { }
        public void ConvertToFireball() { }
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal static class ChomperFusions
    { internal static bool Active(Plant p) => !p.mDead && p.mIsOnBoard && KelpEffects.Playing(p.mBoard); }
    internal static class SquashFusions
    { internal static bool Eligible(Board b, Zombie z, DamageRangeFlags flags) => !z.IsDeadOrDying() && !z.mMindControlled && z.mBoard == b; }
    internal static class ScaredyFusions { internal const float SourceUnit = 80f / 1.35f; }
    internal static class FusionService
    { internal static PlantingReason CheckNativePlacement(Board b, int col, int row, SeedType seed) => b.Occupied ? PlantingReason.NotHere : PlantingReason.Ok; }
    internal static class ImportedPeaVisuals { internal static void EnsureKelpProjectile(Projectile shot) { } internal static void EnsureMiniSquashProjectile(Projectile shot) { } }
    internal static class GroundWeeds
    { internal static bool Target(Plant p, Zombie z) => z.mRow == p.mRow && SquashFusions.Eligible(p.mBoard, z, DamageRangeFlags.Ground) && Math.Abs(z.mPosX - p.mX - 40) <= 40; }
    internal static class NutProjectiles
    { internal static bool Remember(Projectile shot, Zombie zombie, int ticks = 0) => true; }
    internal static class PotDefense { internal static int Damage(Plant plant, int damage) => damage; }
    internal static class WeedFusions
    {
        internal sealed record Crush(Zombie Zombie, Board Board, uint ZombieId);
        internal static Crush? CurrentCrush;
    }
}
namespace LawnGlove
{
    using Il2CppReloaded.Gameplay;
    internal static class PlayerPlantingScope { internal static void Consume(Plant p) => p.mDead = true; }
}

namespace FusionMod { internal static partial class ScaredContent { internal static bool Eyes(Il2CppReloaded.Gameplay.Zombie z) => false; } }

// The swimming clip patch only needs its native controller and rectangle boundary.
namespace UnityEngine
{
    public struct Rect
    {
        public float yMax;
        private float minimumY;
        public float yMin { readonly get => minimumY; set => minimumY = value; }
    }
}
namespace Il2CppSource.Controllers
{
    public sealed class ZombieController
    {
        public Il2CppReloaded.Gameplay.Zombie? m_zombie;
        public TestAnimationController? AnimationController;
        public void ClipRect(UnityEngine.Rect clipRect) { }
    }
    public sealed class TestAnimationController { public TestSkeletonAnimation? SkeletonAnimation; }
    public sealed class TestSkeletonAnimation { public TestSkeleton? skeleton; }
    public sealed class TestSkeleton { public TestSkeletonData Data = new(); }
    public sealed class TestSkeletonData { public object? FindAnimation(string name) => null; }
}

namespace Il2CppReloaded
{
    public static partial class Constants { public static partial class Gameplay { public const int BOARD_WIDTH = 800; } }
}
