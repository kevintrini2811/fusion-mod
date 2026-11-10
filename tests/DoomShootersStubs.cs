// Offline doubles only. These tests never load the game or profiles.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute { public HarmonyPatch(Type type, string name) { } }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int value) { } }
    public static class Priority { public const int First = 800; }
}
namespace Il2CppReloaded.Services
{
    public enum FoleyType { Explosion }
    public sealed class SoundSystem { public void PlayFoley(FoleyType type) { } }
}
namespace Il2CppReloaded.Gameplay
{
    public enum ZombieType { Normal, Gravestone, Target }
    public enum GameScenes { Playing, Loading }
    public enum PlantWeapon { Primary }
    public enum ProjectileType { Pea, PeashooterPea, Star }
    public enum ProjectileMotion { Straight, Star }
    [Flags] public enum DamageRangeFlags { Ground = 1, Flying = 2 }
    [Flags] public enum DamageFlags { BypassesShield = 1 }
    public enum ParticleEffect { Powie, Doom }
    public readonly record struct Rect(float xMin, float yMin, float xMax, float yMax);
    public sealed class App
    {
        public GameScenes GameScene = GameScenes.Playing;
        public Il2CppReloaded.Services.SoundSystem SoundSystem = new();
        public void AddTodParticle(float x, float y, int order, ParticleEffect effect) { }
    }
    public sealed class Lawn { public Plant? NormalPlant; }
    public sealed class Board
    {
        public List<Plant> m_plants = new();
        public Lawn GetPlantsOnLawn(int col, int row) => new() { NormalPlant = m_plants.FirstOrDefault(p => !p.mDead && p.mPlantCol == col && p.mRow == row) };
        public IntPtr Pointer = (IntPtr)1; public bool mPaused, FailCreate; public int Rows = 5;
        public App mApp = new(); public List<Zombie> m_zombies = new(); public List<Projectile> Shots = new();
        public int GetNumRows() => Rows;
        public int GridToPixelX(int col, int row) => col * 80;
        public int GridToPixelY(int col, int row) => row * 100;
        public void ShakeBoard(int x, int y) { }
        public Projectile? AddProjectile(float x, float y, int order, int row, ProjectileType type)
        {
            if (FailCreate) return null;
            var shot = new Projectile { mBoard = this, mPosX = x, mPosY = y, mRenderOrder = order, mRow = row, mProjectileType = type };
            Shots.Add(shot); return shot;
        }
    }
    public sealed class Plant
    {
        public IntPtr Pointer = (IntPtr)2; public Board mBoard = new();
        public SeedType mSeedType, mImitaterType = SeedType.None;
        public int mPlantHealth, mPlantMaxHealth, mLaunchRate, mLaunchCounter, mStartRow,
            mShootingCounter, mPlantCol, mTargetX, mTargetY, mDoSpecialCountdown, mRow, mRenderOrder;
        public float mX; public bool mDead, mIsAsleep, Carried;
        public void PlantInitialize() { } public void UpdateAbilities() { } public void UpdateShooting() { }
        public void Die() => mDead = true;
        public Action? OnFire;
        public void Fire(Zombie target, int row, PlantWeapon weapon)
        {
            OnFire?.Invoke();
            FusionMod.DoomShooters.Created = mBoard.AddProjectile(mX + 40, mRow * 100 + 50,
                mRenderOrder, row, ProjectileType.Pea);
        }
    }
    public sealed class Projectile
    {
        public Board mBoard = null!; public ProjectileType mProjectileType; public ProjectileMotion mMotionType;
        public bool mDead; public float mCobTargetX, mPosX, mPosY, mPosZ, mVelX, mVelY, mVelZ, mAccZ, mX, mY, mShadowY;
        public int mCobTargetRow, mRow, mRenderOrder, mProjectileAge; public DamageRangeFlags mDamageRangeFlags;
        public void Die() => mDead = true;
        public void ProjectileInitialize() { } public void DoImpact() { } public void Update() { }
        public void ConvertToFireball() { } public void ConvertToPea() { }
    }
    public sealed class Zombie
    {
        public Board mBoard = null!; public uint DataID; public int mRow, Damage, Health = 10000;
        public bool mDead, mMindControlled, Flying; public ZombieType mZombieType;
        public float X, Y; public DamageFlags Flags; public Action? OnDamage;
        public Rect GetZombieRect() => new(X, Y, X + 20, Y + 20);
        public bool IsDeadOrDying() => Health <= 0;
        public void TakeDamage(int value, DamageFlags flags) { Damage += value; Health -= value; Flags = flags; OnDamage?.Invoke(); }
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal static class DoomEffects
    {
        internal static void AshDamage(Il2CppReloaded.Gameplay.Zombie zombie, int damage,
            Il2CppReloaded.Gameplay.DamageFlags flags) => zombie.TakeDamage(damage, flags);
    }
    internal static class ChomperFusions
    {
        internal static bool Active(Plant plant) => !plant.mDead && !plant.Carried && !plant.mBoard.mPaused
            && plant.mBoard.mApp.GameScene == GameScenes.Playing;
    }
    internal static class SquashFusions
    {
        internal static bool Eligible(Board board, Zombie zombie, DamageRangeFlags flags) =>
            zombie.mBoard == board && !zombie.mDead && !zombie.IsDeadOrDying() && !zombie.mMindControlled
            && (flags & (zombie.Flying ? DamageRangeFlags.Flying : DamageRangeFlags.Ground)) != 0;
    }
    internal static class FumeProjectiles { internal static int ModifiedDamage(Projectile shot, int damage) => damage; }
    internal static class CherryBombPlants { internal static IntPtr DisposingBoard = IntPtr.Zero; }
    internal static class ImportedPeaVisuals { internal static void EnsureDoomProjectile(Projectile shot) { } }
    internal static class StarProjectiles
    {
        internal static Projectile? Spawn(Board board, float x, float y, int order, int row, int family, float angle, float speed, int damage)
        { var shot = board.AddProjectile(x, y, order, row, ProjectileType.Star); if (shot != null) { shot.mCobTargetRow = damage; shot.mCobTargetX = family; } return shot; }
    }
}
