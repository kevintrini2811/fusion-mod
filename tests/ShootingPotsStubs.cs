namespace Il2CppReloaded.Services { public enum FoleyType { JalapenoIgnite } }
namespace LawnGlove { public static class PlayerPlantingScope { public static void Consume(Il2CppReloaded.Gameplay.Plant plant) => plant.Die(); } }
namespace FusionMod
{
    internal static class DebugMode
    {
        internal static void TraceShootingPot(Il2CppReloaded.Gameplay.Plant plant, string message) { }
    }
    internal static class ThreepeaterFusions
    {
        internal static Il2CppReloaded.Gameplay.Projectile? Shoot(Il2CppReloaded.Gameplay.Plant plant, int row)
            => ShootingPots.FireShot(plant, row).Shot;
    }
}
// Isolated native doubles. No game, Unity runtime, profile or save is loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch() { } public HarmonyPatch(Type type, string method) { } }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute
    { public HarmonyPriority(int value) { } }
    public static class Priority { public const int First = 800, Last = 0; }
    public static class AccessTools
    { public static System.Reflection.MethodBase Method(Type type, string name) => type.GetMethod(name)!; }
}
namespace Il2CppReloaded.Gameplay
{
    public enum ProjectileType { Pea, PeashooterPea, Snowpea, Fireball, PeashooterFireball, Kernel, Butter, Melon, Cabbage, Spike }
    public enum ProjectileMotion { Straight, Threepeater, Lobbed }
    public enum PlantWeapon { Primary, Secondary }
    [Flags] public enum DamageRangeFlags { Ground = 1, Flying=2, Submerged=4, Dog=8, OffGround=16, Underground=32 }
    [Flags] public enum DamageFlags { BypassesShield = 1, HitsShieldAndBody=2, DoesntLeaveBody=4 }
    public enum ParticleEffect { ButterSplat, MelonSplash, CabbageSplat, Powie }
    public enum ZombieType { Zamboni = 12, Normal = 0, Gravestone = 99, Target, Boss }
    public enum ZombieID : uint { None }
    public sealed class Sound { public void PlayFoley(Il2CppReloaded.Services.FoleyType type) {} }
    public enum GameScenes { Playing, Other }
    public sealed class App { public GameScenes GameScene = GameScenes.Playing; public Sound SoundSystem=new(); public void AddTodParticle(float x, float y, int order, ParticleEffect effect) { } }
    public readonly record struct Rect(float xMin, float yMin, float xMax, float yMax);
    public static class Common { public static int Rand(int range) => 1 % range; }
    public sealed class Board
    {
        public IntPtr Pointer = (IntPtr)1;
        public bool mPaused, FailAllocation;
        public int Rows = 5, Money;
        public sealed class SunMoney { public int Amount; }
        public SunMoney[] mSunMoney = {new()};
        public App mApp = new();
        public List<Plant> m_plants = new();
        public List<Zombie> m_zombies = new();
        public List<Projectile> Shots = new();
        public Queue<int> Rolls = new();
        public int mMainCounter;
        public bool BypassFireHooks, BypassFactoryHook;
        public int Rand(int range) => (Rolls.Count > 0 ? Rolls.Dequeue() : 1) % range;
        public void ShakeBoard(int x,int y) {}
        public Plant? AddPlant(int col,int row,SeedType type,SeedType marker) {
            if (FailAllocation) return null;
            var p=new Plant {mBoard=this,mPlantCol=col,mRow=row,mSeedType=type,mImitaterType=marker};m_plants.Add(p);return p;
        }
        public int GetNumRows() => Rows;
        public bool Water; public bool IsPoolSquare(int col, int row) => Water;
        public int PixelToGridX(float x, float y) => (int)Math.Floor(x / 80);
        public int GridToPixelX(int col, int row) => col * 80;
        public int GridToPixelY(int col, int row) => row * 100;
        public Projectile? AddProjectile(float x, float y, int order, int row, ProjectileType type)
        {
            if (FailAllocation) return null;
            var shot = new Projectile { mBoard = this, mPosX = x, mPosY = y,
                mShadowY = y + 40, mRow = row, mProjectileType = type };
            Shots.Add(shot);
            FusionMod.ShootingPotProjectileInitializedPatch.Postfix(shot);
            if (!BypassFactoryHook) FusionMod.ShootingPotProjectilePatch.Postfix(this, shot);
            FusionMod.CattailFusions.Track(shot);
            return shot;
        }
    }
    public sealed class Plant
    {
        private static uint next;
        public uint DataID = ++next;
        public IntPtr Pointer => (IntPtr)DataID;
        public Board mBoard = null!;
        public SeedType mSeedType, mImitaterType = SeedType.None;
        public int mPlantCol = 1, mRow = 2, mPlantHealth = 300, mPlantMaxHealth = 300;
        public int mDoSpecialCountdown, mLaunchRate, mLaunchCounter, mStartRow, mShootingCounter, mTargetX, mTargetY, mRenderOrder;
        public ZombieID mTargetZombieID;
        public float mX = 80, mY = 200, mWidth = 80, mHeight = 80;
        public bool mDead, mIsOnBoard = true, mSquished, mIsAsleep, OffGround;
        public bool NotOnGround() => OffGround;
        public static bool IsUpgrade(SeedType type) => false;
        public void Die() => mDead=true;
        public void DoSpecial() {}
        public void UpdateAbilities() {}
        public Zombie? FindTargetZombie(int row,PlantWeapon weapon) => mBoard.m_zombies.FirstOrDefault(z => !z.mDead&&!z.mMindControlled);
        public void PlantInitialize() { }
        public void UpdateShooter() { }
        public void UpdateShooting() { }
        public void FindTargetAndFire() { }
        public void Fire(Zombie? zombie, int row, PlantWeapon weapon)
        {
            var previous=FusionMod.CattailFusions.Firing;
            FusionMod.CattailFusions.Firing=FusionMod.FusionPlant.IsElementalCattail(FusionMod.FusionPlant.From(this))?this:null;
            FusionMod.ShootingPots.Scope old = default;
            if (!mBoard.BypassFireHooks) FusionMod.ShootingPotFireScope.Prefix(this, weapon, out old);
            try
            {
                if (!FusionMod.TriLobberNativeFirePatch.Prefix(this)) return;
                var shot = mBoard.AddProjectile(mX + 40, mY + 30, 0, row, mSeedType==SeedType.Cattail?ProjectileType.Spike:ProjectileType.Pea);
                // Native Fire already configures Threepeater's lane transition.
                if (shot != null && mSeedType == SeedType.Threepeater && row != mRow)
                {
                    shot.mMotionType = ProjectileMotion.Threepeater;
                    shot.mVelY = Math.Sign(row - mRow) * 3;
                    shot.mShadowY -= mBoard.GridToPixelY(mPlantCol, row) - mBoard.GridToPixelY(mPlantCol, mRow);
                }
                if (!mBoard.BypassFireHooks) FusionMod.ShootingPotFireScope.Postfix(old);
            }
            finally { if (!mBoard.BypassFireHooks) FusionMod.ShootingPotFireScope.Finalizer(old); FusionMod.CattailFusions.Firing=previous; }
        }
    }
    public sealed class Projectile
    {
        private static uint next = 1000;
        public uint DataID = ++next;
        public IntPtr Pointer => (IntPtr)DataID;
        public Board mBoard = null!;
        public bool mDead;
        public ProjectileType mProjectileType;
        public ProjectileMotion mMotionType;
        public DamageRangeFlags mDamageRangeFlags;
        public ZombieID mTargetZombieID;
        public int mRow, mCobTargetRow, mRenderOrder, mHitTorchwoodGridX;
        public float mCobTargetX, mPosX, mPosY, mPosZ, mVelX, mVelY, mVelZ, mAccZ, mShadowY;
        public void Die() => mDead = true;
        public void ProjectileInitialize() { }
        public void Update() { }
        public void UpdateLobMotion() {}
        public void DoImpact(Zombie zombie) { }
        public Zombie? FindCollisionTarget() => null;
        public void DoSplashDamage(Zombie zombie) { }
        public DamageFlags GetDamageFlags(Zombie zombie)
        { FusionMod.ShootingPotDamageTargetPatch.Postfix(this, zombie); return DamageFlags.BypassesShield; }
    }
    public sealed class Zombie
    {
        public bool IsWalkingBackwards() => mMindControlled;
        public bool IsMovingAtChilledSpeed() => mChilledCounter > 0;
        private static uint next = 500;
        public HelmType mHelmType; public ShieldType mShieldType; public uint DataID = ++next;
        public IntPtr Pointer => (IntPtr)DataID;
        public Board mBoard = null!;
        public int mBodyHealth=3000,mBodyMaxHealth=3000,mHelmHealth,mHelmMaxHealth;
        public bool FailControl;
        public void StartMindControlled() {if(!FailControl)mMindControlled=true;}
        public void EatPlant(Plant plant) {}
        public int mRow = 2, mShieldHealth, Damage, Buttered, mButteredCounter, mIceTrapCounter, mChilledCounter, FreezePoints;
        public float X = 400, mVelX;
        public bool mIsEating;
        public bool mDead, mMindControlled, Irradiated;
        public Action? OnDamage;
        public ZombieType mZombieType;
        public bool IsDeadOrDying() => mDead;
        public Rect GetZombieRect() => new(X - 10, mRow * 100 + 10, X + 10, mRow * 100 + 50);
        public void TakeDamage(int damage, DamageFlags flags) { Damage += damage; OnDamage?.Invoke(); }
        public void ApplyChill(bool value) => mChilledCounter = 1000;
        public void RemoveColdEffects() { mChilledCounter = mIceTrapCounter = 0; }
        public void ApplyButter() { ++Buttered; mButteredCounter = 400; }
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal static class ChomperFusions
    { internal static bool Active(Plant plant) => !plant.mDead && plant.mIsOnBoard && !plant.mSquished && !plant.OffGround && !plant.mBoard.mPaused; }
    internal static class ScaredyFusions { internal const float SourceUnit = 80f / 1.35f; }
    internal static class SquashFusions
    { internal static bool Eligible(Board board, Zombie zombie, DamageRangeFlags flags) => zombie.mBoard == board && !zombie.mDead && !zombie.mMindControlled; }
    internal static class PuffShooters { internal const float FlightHeight = 40; }
    internal static class PuffLobbers
    {
        internal const int FlightTicks = 150;
        internal const float Gravity = 9.8f * ScaredyFusions.SourceUnit / 10000;
        internal static Zombie? Target(Plant plant, int? row = null) => plant.mBoard.m_zombies
            .Where(zombie => zombie.mRow == (row ?? plant.mRow) && zombie.X >= plant.mX
                && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)).OrderBy(zombie => zombie.X).FirstOrDefault();
    }
    internal static class CherrySpreader { internal static bool IsFlying(Projectile shot) => shot.mCobTargetX == -41020; }
    internal static class IronProjectiles
    {
        internal static bool Contains(Projectile shot) => shot.mCobTargetX is -450011 or -450168;
        internal static bool IsRed(Projectile shot) => shot.mCobTargetX == -450168;
    }
    internal static class SilverMoney
    { internal static void DropMoney(Board board, float x, float y, int money) => board.Money += money; }
    internal static class ImportedPeaVisuals { internal static void EnsureSquashLobProjectile(Projectile shot) {} internal static void EnsureCattailProjectile(Projectile shot) {} internal static void EnsureTriLobProjectile(Projectile shot) { } internal static void EnsureMixedLobProjectile(Projectile shot) { } internal static void EnsureElementalLobProjectile(Projectile shot) { } }
    internal static class SnowPea
    {
        internal static void Hit(Zombie zombie, int points = 10)
        { if (zombie.mIceTrapCounter == 0 && (zombie.FreezePoints += points) >= 100) zombie.mIceTrapCounter = 400; }
    }
    internal static class Overheat
    {
        internal static readonly HashSet<uint> Heated = new();
        internal static void Apply(Zombie zombie) => Heated.Add(zombie.DataID);
        internal static bool Has(Zombie zombie) => Heated.Contains(zombie.DataID);
    }
    internal static class PoisonShooters
    {
        internal static readonly Dictionary<uint, int> Levels = new(), Durations = new();
        internal static int Level(Zombie zombie) => Levels.GetValueOrDefault(zombie.DataID);
        internal static void AddLevel(Board board, Zombie zombie) => Levels[zombie.DataID] = Level(zombie) + 1;
        internal static void ApplyPoison(Board board, Zombie zombie, int ticks)
        { AddLevel(board, zombie); Durations[zombie.DataID] = ticks; }
    }
    internal static class BloverEffects
    {
        internal static bool Reserve(Zombie zombie, bool cryo) => true;
        internal static bool ReserveOverheat(Zombie zombie) => true;
    }
    internal static class ChomperVariants
    { internal static void Freeze(Zombie zombie, int ticks) => zombie.mIceTrapCounter = Math.Max(zombie.mIceTrapCounter, ticks); }

}

namespace FusionMod { internal static class SolarShooter { internal static int Sun; internal static void DropSun(Il2CppReloaded.Gameplay.Board board,float x,float y,int value)=>Sun+=value; } }

namespace FusionMod { internal static class FumeProjectiles { internal static int ModifiedDamage(Il2CppReloaded.Gameplay.Projectile shot,int damage)=>damage; } }
