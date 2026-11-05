// Offline doubles only. This project never loads game assemblies or profiles.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch() {} public HarmonyPatch(Type type, string method) {} }
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int value) {} }
    public static class Priority { public const int First = 800, Last = 0; }
    public static class AccessTools
    { public static System.Reflection.MethodBase Method(Type type, string name) => type.GetMethod(name)!; }
}
namespace Il2CppReloaded.Gameplay
{
    public enum ZombieType { Normal, Boss, Gravestone, Target, Zamboni, Catapult }
    public enum ProjectileType { Pea, ZombiePea, Basketball, Melon, Wintermelon, Fireball, PeashooterFireball }
    [Flags] public enum DamageRangeFlags { Ground = 1, Flying = 2, Submerged = 4, Dog = 8, OffGround = 16, Dying = 32, Underground = 64 }
    public enum GameScenes { Playing, Menu }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public enum GridItemType { ZenTool, Crater }
    public enum GridItemState { Normal }
    public enum ParticleEffect { Doom }
    [Flags] public enum DamageFlags { BypassesShield = 1, Spike = 2 }
    public readonly record struct Rect(float xMin, float xMax);
    public static class Common { public static int Rand(int range) => range / 2; }
    public sealed class Board
    {
        public IntPtr Pointer = (IntPtr)1;
        public bool mPaused;
        public App mApp = new();
        public bool FailPlant;
        public GridItems m_gridItems;
        public Board() { m_gridItems = new(this); }
        public Plant? AddPlant(int col, int row, SeedType type, SeedType marker)
        { if (FailPlant) return null; var p = new Plant { mBoard = this, mPlantCol = col, mRow = row, mSeedType = type, mImitaterType = marker }; m_plants.Add(p); return p; }
        public void KillAllZombiesInRadius(int row, float x, float y, float radius, int rows, bool burn, DamageRangeFlags range) {}
        public void ShakeBoard(int x, int y) {}
        public int mPlantsEaten, Rows = 5, mMainCounter;
        public List<Projectile> m_projectiles = new();
        public List<Plant> m_plants = new();
        public List<Zombie> m_zombies = new();
        public int Rand(int range) => range / 2;
        public int GetNumRows() => Rows;
        public Projectile AddProjectile() => new() { mBoard = this };
    }
    public sealed class Plant
    {
        static int next = 10;
        public IntPtr Pointer = (IntPtr)(++next);
        public uint DataID = (uint)next;
        public Board mBoard = null!;
        public SeedType mSeedType, mImitaterType = SeedType.None;
        public int mPlantCol, mRow, mPlantHealth = 300, mPlantMaxHealth = 300;
        public int mTargetX, mTargetY, mLaunchCounter, mShootingCounter, mStartRow, mEatenFlashCountdown, mWidth = 80;
        public float mX, mY;
        public int mHeight = 80;
        public void UpdateReanim() {}
        public void UpdateReanimColor() {}
        public void UpdateAbilities() {}
        public void Update() {}
        public static bool IsUpgrade(SeedType type) => false;
        public static int GetCost(App app, SeedType seed, SeedType marker) => seed == SeedType.Scaredyshroom ? 25 : seed == SeedType.Peashooter ? 100 : 50;
        public void DoSpecial() {}
        public bool mDead, mIsOnBoard = true, mIsAsleep, OffGround;
        public bool NotOnGround() => OffGround;
        public void Die() => mDead = true;
        public void Squish() => Die();
        public void Fire() {}
        public void UpdateShooter() {}
    }
    public sealed class Zombie
    {
        static int next = 100;
        public uint DataID = (uint)(++next);
        public IntPtr Pointer = (IntPtr)next;
        public Board mBoard = null!;
        public int mRow, Damage, mYuckyFaceCounter, mTargetRow = -1, mShieldHealth;
        public bool mDead, mMindControlled, mYuckyFace;
        public ZombieType mZombieType;
        public float X = 40;
        public int mBodyHealth = 100, mBodyMaxHealth = 300;
        public float mPosX => X;
        public void StartMindControlled() { mMindControlled = true; }
        public bool IsDeadOrDying() => mDead;
        public Rect GetZombieRect() => new(X - 10, X + 10);
        public void TakeDamage(int damage, DamageFlags flags) => Damage += damage;
        public void DieWithLoot() => mDead = true;
        public void EatPlant(Plant plant) => plant.mPlantHealth -= 4;
        public void StopEating() {}
        public void UpdateAnimSpeed() {}
        public void UpdateYuckyFace() {}
        public bool CanTargetPlant(Plant plant, ZombieAttackType type) => true;
    }
    public sealed class Projectile
    {
        static int next = 200;
        public uint DataID = (uint)(++next);
        public Board mBoard = null!;
        public bool mDead;
        public int mCobTargetRow;
        public ProjectileType mProjectileType;
        public void CheckForCollision() {}
        public void Update() {}
        public void ProjectileInitialize() {}
        public void DoImpact(Zombie zombie) {}
        public void UpdateLobMotion() {}
        public void DoSplashDamage(Zombie zombie) {}
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal static class ChomperFusions
    { internal static bool Active(Plant plant) => !plant.mDead && plant.mIsOnBoard && !plant.mBoard.mPaused; }
    internal static class SquashFusions
    { internal static bool Eligible(Board board, Zombie zombie, DamageRangeFlags flags) =>
        board.Pointer == zombie.mBoard.Pointer && !zombie.mDead && !zombie.mMindControlled; }
    internal static class PlayerPlantingScope { internal static void Consume(Plant plant) => plant.Die(); }
    internal static class ScaredyFusions { internal const float SourceUnit = 80f / 1.35f;
        internal static bool EnemyNear(Plant p, Zombie z, float halfWidth=1.25f) => !z.mDead && !z.mMindControlled && System.Math.Abs(p.mRow-z.mRow)<=1 && System.Math.Abs(p.mX-z.X)<80; }
    internal static class AirPlants { internal static bool Owns(Plant p) => FusionPlant.From(p)==FusionPlant.CamoShroom; }
    internal static class PotFusions { internal static IntPtr CostingHealth; internal static bool Protected(Plant plant) => false; }
    internal static class WeedFusions
    {
        internal sealed record Crush(Zombie Zombie, Board Board, uint ZombieId);
        internal static Crush? CurrentCrush;
    }
}

namespace Il2CppReloaded.Services
{
    public enum FoleyType { Explosion }
}
namespace Il2CppReloaded.Gameplay
{
    public sealed class App
    {
        public GameScenes GameScene = GameScenes.Playing;
        public GameMode GameMode;
        public Sound SoundSystem = new();
        public void AddTodParticle(float x, float y, int order, ParticleEffect effect) {}
    }
    public sealed class Sound
    { public void PlayFoley(Il2CppReloaded.Services.FoleyType type) {} }
    public sealed class GridItems : List<GridItem>
    {
        private readonly Board board;
        public bool Fail;
        public GridItems(Board board) { this.board = board; }
        public GridItem? DataArrayAlloc()
        { if (Fail) return null; var item = new GridItem { mBoard = board }; Add(item); return item; }
    }
    public sealed class GridItem
    {
        private static int nextPointer = 1000;
        public IntPtr Pointer = (IntPtr)(++nextPointer);
        public Board mBoard = null!;
        public GridItemType mGridItemType;
        public GridItemState mGridItemState;
        public SeedType mSeedType;
        public int mSunCount, mGridX, mGridY, mGridItemCounter, mTransparentCounter;
        public bool mDead;
        public float mX, mY, mPosX, mPosY;
        public void Init(App app, GridItemType type) { mGridItemType = type; }
        public void GridItemDie() { mDead = true; }
        public void Update() {}
        public void DrawGridItem() {}
    }
}

namespace FusionMod { internal static class IronProjectiles { internal static bool Contains(Il2CppReloaded.Gameplay.Projectile shot)=>false; internal static bool IsRed(Il2CppReloaded.Gameplay.Projectile shot)=>false; } }
