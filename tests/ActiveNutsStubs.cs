// Offline doubles; no game process, Unity player, profile or save is loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch() { } public HarmonyPatch(Type type, string method) { } }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute
    { public HarmonyPriority(int priority) { } }
    public static class Priority { public const int Low = 200; }
    public static class AccessTools
    { public static System.Reflection.MethodBase Method(Type type, string method) => type.GetMethod(method)!; }
}
namespace Il2CppReloaded.Gameplay
{
    public enum ProjectileType { Pea, PeashooterPea, Cabbage, Melon, ZombiePea, Basketball }
    public enum ProjectileMotion { Straight, Lobbed }
    public enum ZombieType { Normal, Gravestone, Target }
    public enum GridItemType { ZenTool }
    public enum GridItemState { Normal }
    [Flags] public enum DamageRangeFlags { Ground = 1 }
    [Flags] public enum DamageFlags { BypassesShield = 1, HitsShieldAndBody = 2, DoesntLeaveBody = 16 }
    public enum ParticleEffect { DustSquash }
    public readonly record struct Rect(float xMin, float yMin, float xMax, float yMax);
    public sealed class App { public void AddTodParticle(float x, float y, int order, ParticleEffect effect) { } }
    public sealed class Pool<T> : List<T> where T : new()
    {
        public bool Fail;
        public T? DataArrayAlloc() { if (Fail) return default; var item = new T(); Add(item); return item; }
    }
    public sealed class Board
    {
        public IntPtr Pointer = (IntPtr)1;
        public App mApp = new();
        public bool mPaused, FailProjectile;
        public int mPlantsEaten;
        public List<Plant> m_plants = new();
        public List<Zombie> m_zombies = new();
        public List<Projectile> m_projectiles = new();
        public Pool<GridItem> m_gridItems = new();
        public int Rand(int range) => 0;
        public float GetPosYBasedOnRow(float x, int row) => row * 100;
        public Projectile? AddProjectile(float x, float y, int order, int row, ProjectileType type)
        {
            if (FailProjectile) return null;
            var shot = new Projectile { mBoard = this, mPosX = x, mPosY = y, mRow = row, mProjectileType = type };
            m_projectiles.Add(shot); return shot;
        }
    }
    public sealed class Plant
    {
        static uint next;
        public uint DataID = ++next;
        public IntPtr Pointer => (IntPtr)DataID;
        public Board mBoard = new();
        public SeedType mSeedType, mImitaterType;
        public bool mDead, mIsAsleep, mIsOnBoard = true;
        public int mPlantHealth, mPlantMaxHealth, mLaunchCounter, mStartRow, mShootingCounter, mTargetX, mTargetY;
        public int mRow = 2, mRenderOrder;
        public float mX = 80, mY = 200, mWidth = 80, mHeight = 80;
        public void PlantInitialize() { }
        public void UpdateAbilities() { }
        public void Fire() { }
        public void Die() => mDead = true;
    }
    public sealed class Zombie
    {
        public bool IsWalkingBackwards() => mMindControlled;
        public int mChilledCounter;
        public bool IsMovingAtChilledSpeed() => mChilledCounter > 0;
        static uint next;
        public uint DataID = ++next;
        public Board mBoard = new();
        public int mRow = 2, Health = 10000, Hits;
        public float X = 160;
        public bool mDead, mMindControlled;
        public ZombieType mZombieType;
        public Rect GetZombieRect() => new(X, 200, X + 40, 280);
        public void TakeDamage(int damage, DamageFlags flags) { Health -= damage; ++Hits; }
        public void EatPlant(Plant plant) { }
        public bool CanTargetPlant(Plant plant, ZombieAttackType attack) => true;
    }
    public sealed class Projectile
    {
        static uint next;
        public uint DataID = ++next;
        public Board mBoard = new();
        public bool mDead;
        public ProjectileType mProjectileType;
        public ProjectileMotion mMotionType;
        public DamageRangeFlags mDamageRangeFlags;
        public float mCobTargetX, mPosX, mPosY, mPosZ, mVelX, mVelY, mVelZ, mAccZ, mShadowY, mRotation;
        public int mCobTargetRow, mRow, mHitTorchwoodGridX;
        public void Die() => mDead = true;
        public void DoImpact(Zombie zombie) { }
        public void Update() { }
        public void UpdateLobMotion() { }
        public void UpdateNormalMotion() { }
        public void CheckForCollision() { }
        public Plant? FindCollisionTargetPlant() => null;
        public void ConvertToFireball() { }
    }
    public sealed class GridItem
    {
        private static uint nextGridItem;
        public uint DataID = ++nextGridItem;
        public IntPtr Pointer = (IntPtr)nextGridItem;
        public bool mDead;
        public GridItemType mGridItemType;
        public GridItemState mGridItemState;
        public int mGridItemCounter; public int mSunCount, mGridX, mGridY;
        public float mX, mY, mPosX, mPosY;
        public Board mBoard = new();
        public void Init(App app, GridItemType type) => mGridItemType = type;
        public void GridItemDie() => mDead = true;
        public void Update() { }
        public void DrawGridItem() { }
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal static class ChomperFusions
    { internal static bool Active(Plant plant) => !plant.mDead && !plant.mBoard.mPaused && plant.mIsOnBoard; }
    internal static class SquashFusions
    { internal static bool Eligible(Board board, Zombie zombie, DamageRangeFlags flags) => !zombie.mDead && !zombie.mMindControlled && zombie.mBoard == board; }
    internal static class ScaredyFusions { internal const float SourceUnit = 80f / 1.35f; }
    internal static class PuffLobbers
    {
        internal const int FlightTicks = 75;
        internal static Zombie? Target(Plant plant) => plant.mBoard.m_zombies.FirstOrDefault(z => z.X >= plant.mX && SquashFusions.Eligible(plant.mBoard, z, DamageRangeFlags.Ground));
    }
    internal static class LobberProjectiles
    { internal static void Launch(Projectile shot, Zombie? zombie, int ticks) => shot.mMotionType = ProjectileMotion.Lobbed; }
    internal static class LobberImpact
    { internal static void Apply(Projectile shot, Zombie? zombie, int damage, bool melon, bool butter, int money, bool boost) { zombie?.TakeDamage(damage, DamageFlags.BypassesShield); shot.Die(); } }
    internal static class ImportedPeaVisuals { internal static void EnsureNutProjectile(Projectile shot) { } }
    internal static class PlayerPlantingScope { internal static IntPtr ConsumingIngredient = IntPtr.Zero; }
}
