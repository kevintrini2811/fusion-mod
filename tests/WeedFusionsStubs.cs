// Offline doubles. No game, Unity player, profile or save file is loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch() { } public HarmonyPatch(Type type, string method) { } }
    public static class AccessTools { public static System.Reflection.MethodBase Method(Type type, string name) => type.GetMethod(name)!; }
}
namespace Il2CppReloaded.Services
{
    public enum FoleyType { JalapenoIgnite }
    public sealed class SoundSystem { public int Calls; public void PlayFoley(FoleyType type) => ++Calls; }
}
namespace Il2CppReloaded.Gameplay
{

    public enum PlantState { Notready, SquashLook, SquashPreLaunch, SquashRising, SquashFalling, SquashDoneFalling }
    public enum PlantWeapon { Primary }
    public enum PlantOnBungeeState { NotOnBungee, OnBungee }
    public enum GameScenes { Playing, Loading }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public enum ZombieType { Normal, Gargantuar, Boss, Gravestone, Target, Zamboni, Catapult, Balloon, Imp, RedeyeGargantuar, Bungee }
    [Flags] public enum DamageRangeFlags { Ground=1, Flying=2, Submerged=4, Dog=8, OffGround=16, Underground=64 }
    [Flags] public enum DamageFlags { BypassesShield=1, HitsShieldAndBody=2, DoesntLeaveBody=16, Spike=32 }
    public enum ParticleEffect { IceSparkle, Powie }
    public readonly record struct Rect(float xMin, float yMin, float xMax, float yMax);
    public sealed class App
    {
        public GameScenes GameScene = GameScenes.Playing;
        public GameMode GameMode;
        public Il2CppReloaded.Services.SoundSystem SoundSystem = new();
        public int Particles;
        public void AddTodParticle(float x, float y, int order, ParticleEffect type) => ++Particles;
    }
    public sealed class Board
    {
        public IntPtr Pointer = (IntPtr)1;
        public bool mPaused;
        public int mMainCounter;
        public App mApp = new();
        public List<Zombie> m_zombies = new();
        public List<Plant> m_plants = new();
        public Pool<GridItem> m_gridItems;
        public Board() { m_gridItems = new(() => new GridItem { mBoard = this }); }
        public void DisposeBoard() { }
    }
    public sealed class Plant
    {
        public IntPtr Pointer = (IntPtr)2;
        public uint DataID = 1;
        public Board mBoard = new();
        public App mApp => mBoard.mApp;
        public SeedType mSeedType = SeedType.Squash, mImitaterType = SeedType.Puffshroom;
        public PlantState mState;
        public int mPlantCol, mRow = 2, mRenderOrder, mStateCountdown, mTargetX, mTargetY, mStartRow;
        public float mX = 200, mY = 200, mWidth = 80, mHeight = 80;
        public bool mIsAsleep;
        public bool mDead, mIsOnBoard = true, mSquished;
        public int mPlantHealth = 300, mPlantMaxHealth = 300;
        public PlantOnBungeeState mOnBungeeState;
        public bool OffGround;
        public bool NotOnGround() => OffGround;
        public void Die() => mDead = true;
        public void Squish() { if (FusionMod.SpikeNutVehiclePatch.Prefix(this)) mSquished = true; }
        public void DoRowAreaDamage(int theDamage, DamageFlags theDamageFlags) {
            bool run = FusionMod.WeedAttackPatch.Prefix(this, ref theDamage, theDamageFlags, out var previous);
            try {
                if (run) {
                    foreach (var z in mBoard.m_zombies.ToArray()) {
                        if (z.mDead || z.mMindControlled || z.mRow != mRow || z.Rect.xMax < mX
                            || z.Rect.xMin > mX + mWidth || z.Category != DamageRangeFlags.Ground) continue;
                        int damage = theDamage;
                        if (z.mZombieType is ZombieType.Zamboni or ZombieType.Catapult) { damage = 1800; Die(); }
                        z.TakeDamage(damage, theDamageFlags);
                    }
                }
                FusionMod.WeedAttackPatch.Postfix(this);
            } finally { FusionMod.WeedAttackPatch.Finalizer(previous); }
        }
        public int mLaunchCounter, mShootingCounter;
        public void UpdateAbilities() { }
        public void UpdateReanim() { }
        public void UpdateReanimColor() { }
        public void UpdateSpikeweed() { }
        public void UpdateShooting() { }
        public void Fire() { }
        public void SetSleeping(bool value) { mIsAsleep = value; }
        public Rect GetPlantAttackRect(PlantWeapon weapon) => new(mX, mY, mX + mWidth, mY + mHeight);
        public void PlantInitialize() { }
        public void BurnRow(int theRow) { }
        public void DoSpecial() { }
        public void IceZombies() { }
        public DamageRangeFlags GetDamageRangeFlags(PlantWeapon weapon) => DamageRangeFlags.Ground;
        public Zombie? FindSquashTarget() => null;
        public void DoSquashDamage() { }
    }
    public sealed class Zombie
    { public void ApplyBurn() {} public void Update() {}
        public float mAltitude, mPosX, mPosY; public int mX, mY;
        public void StopEating() { }
        public void UpdateZombieWalking() { }
        public void UpdateActions() { }
        public void UpdatePlaying() { }
        public void CheckSquish() { }
        public bool CanTargetPlant(Plant plant) => true;
        public int mButteredCounter;
        public bool ButterImmune;
        public void ApplyButter() { if (!ButterImmune) mButteredCounter = 400; }

        public bool IsFlying() => false;
        public void LandFlyer(DamageFlags flags) { }
        public IntPtr Pointer = (IntPtr)3;
        public uint DataID = 1;
        public Board mBoard = null!;
        public ZombieType mZombieType;
        public int mRenderOrder, mRow = 2, Health = 10000, Damage, FreezeCalls, ColdRemovals;
        public int mIceTrapCounter, mChilledCounter;
        public bool mDead, mMindControlled, Dying, ImmuneToFreeze, FailControl;
        public Rect Rect = new(230, 200, 250, 280);
        public DamageRangeFlags Category = DamageRangeFlags.Ground;
        public Action? OnDamage;
        public bool IsDeadOrDying() => Dying || Health <= 0;
        public bool EffectedByDamage(DamageRangeFlags flags) => (flags & Category) != 0;
        public Rect GetZombieRect() => Rect;
        public void TakeDamage(int damage, DamageFlags flags)
        {
            FusionMod.WeedHitPatch.Prefix(this, flags, out var hit);
            int amount = damage;
            if (!FusionMod.SnowPeaDamagePatchForTests.Apply(this, ref amount)) return;
            FusionMod.OverheatDamagePatch.Prefix(this, ref amount);
            Damage += amount; Health -= amount; OnDamage?.Invoke();
            if (Health <= 0 && !mDead) DieWithLoot();
            FusionMod.WeedHitPatch.Postfix(this, hit);
        }
        public bool ImmuneToChill; public bool CanBeChilled() => !ImmuneToChill; public bool CanBeFrozen() => !ImmuneToFreeze;
        public void ApplyChill(bool ice) {
            if (!ImmuneToFreeze) mChilledCounter = Math.Max(mChilledCounter, 1000);
            FusionMod.OverheatColdPatch.Postfix(this);
        }
        public bool SkipBite;
        public int BiteDamage = 4;
        public void EatPlant(Plant p) {
            FusionMod.ElementalNutBitePatch.Prefix(this, p, out var before);
            // Native path: skipped bites (ladder, odd chilled tick, etc.) have no HP write.
            if (!SkipBite) { p.mPlantHealth -= BiteDamage; if (p.mPlantHealth <= 0) p.mDead = true; }
            FusionMod.ElementalNutBitePatch.Postfix(this, p, before);
        }
        public void DieWithLoot() {
            FusionMod.OverheatDeathPatch.Prefix(this, out var before);
            DieNoLoot();
            FusionMod.OverheatDeathPatch.Postfix(this, before);
        }
        public void DieNoLoot() {
            FusionMod.OverheatDeathPatch.Prefix(this, out var before);
            mDead = true;
            FusionMod.OverheatDeathPatch.Postfix(this, before);
        }
        public void SquishAllInSquare(int x, int y, int attackType) {
            FusionMod.SpikeNutVehicleScope.Prefix(this, out var previous);
            try { foreach (var p in mBoard.m_plants.Where(p => p.mPlantCol==x && p.mRow==y).ToArray()) p.Squish(); }
            finally { FusionMod.SpikeNutVehicleScope.Finalizer(previous); }
        }
        public void Reset() { FusionMod.Overheat.Forget(this); FusionMod.SnowPea.Forget(this); }
        public void HitIceTrap()
        {
            ++FreezeCalls;
            if (ImmuneToFreeze) return;
            // Native HitIceTrap has separate damage, suppressed by SnowPea.
            if (FusionMod.SnowPea.CreatingIceTrap != Pointer) TakeDamage(20, 0);
            mIceTrapCounter = Math.Max(mIceTrapCounter, 400);
            mChilledCounter = Math.Max(mChilledCounter, 1400);
            FusionMod.SnowPea.ClearPoints(this);
            FusionMod.OverheatColdPatch.Postfix(this);
        }
        public void RemoveColdEffects() { ++ColdRemovals; mIceTrapCounter = mChilledCounter = 0; FusionMod.SnowPea.ClearPoints(this); }
        public void RemoveIceTrap() { mIceTrapCounter = 0; FusionMod.SnowPea.ClearPoints(this); }
        public void StartMindControlled() { if (!FailControl) mMindControlled = true; }
    }
}
namespace Il2CppReloaded.Gameplay {
    public enum ProjectileType { Pea, Snowpea }
    public sealed class Projectile { public bool mDead = false; public float mCobTargetX; public ProjectileType mProjectileType; public void DoImpact(Zombie theZombie) { } }
}
namespace FusionMod {
    internal static class CherryBombPlants { internal static IntPtr DisposingBoard; }
    internal static class ChomperFusions {
        internal static bool Active(Il2CppReloaded.Gameplay.Plant p) => p.mBoard != null && !p.mBoard.mPaused
            && p.mBoard.Pointer != CherryBombPlants.DisposingBoard && !p.mDead && p.mIsOnBoard
            && !p.mSquished && !p.NotOnGround() && p.mOnBungeeState == Il2CppReloaded.Gameplay.PlantOnBungeeState.NotOnBungee
            && p.mApp.GameScene == Il2CppReloaded.Gameplay.GameScenes.Playing
            && p.mApp.GameMode != Il2CppReloaded.Gameplay.GameMode.ChallengeZenGarden;
    }
    internal static class SnowPeaDamagePatchForTests {
        internal static bool Apply(Il2CppReloaded.Gameplay.Zombie z, ref int damage) => SnowPea.CreatingIceTrap != z.Pointer;
    }
}

namespace FusionMod {
    internal static class SolarShooter {
        internal static List<(Il2CppReloaded.Gameplay.Board Board, int Value)> Suns = new();
        internal static void DropSun(Il2CppReloaded.Gameplay.Board b, float x, float y, int value) => Suns.Add((b, value));
    }
}

namespace FusionMod { internal static class PuffSpecials {
 internal static bool IsNugget(Il2CppReloaded.Gameplay.Projectile p) => p.mCobTargetX == -470092;
 internal static bool IsIcicle(Il2CppReloaded.Gameplay.Projectile p) => p.mCobTargetX == -470018;
} }

// This suite exercises native vehicle handling without passive pot supports.
namespace FusionMod { internal static class PotDefense {
 internal static int Damage(Il2CppReloaded.Gameplay.Plant plant, int damage) => damage;
} }

namespace Il2CppReloaded.Gameplay {
    public enum GridItemType { ZenTool }
    public enum GridItemState { Normal }
    public sealed class Pool<T> : List<T> {
        private readonly Func<T> create;
        public bool Fail;
        public Pool(Func<T> create) { this.create = create; }
        public T? DataArrayAlloc() { if (Fail) return default; var item = create(); Add(item); return item; }
    }
    public sealed class GridItem {
        private static uint next;
        public uint DataID = ++next;
        public IntPtr Pointer = (IntPtr)next;
        public Board mBoard = null!;
        public GridItemType mGridItemType;
        public GridItemState mGridItemState;
        public int mSunCount, mGridX, mGridItemCounter, mX, mY;
        public float mPosX, mPosY;
        public bool mDead;
        public void Init(App app, GridItemType kind) { mGridItemType = kind; }
        public void GridItemDie() { mDead = true; }
        public void Update() { }
        public void DrawGridItem() { }
    }
}
namespace FusionMod {
    internal static class KelpEffects {
        internal static int Calls,Ticks; internal static bool Land;
        internal static bool Add(Il2CppReloaded.Gameplay.Zombie z,int ticks,bool suppressSeed=false) { ++Calls;Ticks=ticks;Land=suppressSeed;return true; }
        internal static bool Playing(Il2CppReloaded.Gameplay.Board b) => !b.mPaused
            && b.mApp.GameScene == Il2CppReloaded.Gameplay.GameScenes.Playing
            && b.mApp.GameMode != Il2CppReloaded.Gameplay.GameMode.ChallengeZenGarden;
    }
    internal static class SquashFusions {
        internal static bool Eligible(Il2CppReloaded.Gameplay.Board b, Il2CppReloaded.Gameplay.Zombie z,
            Il2CppReloaded.Gameplay.DamageRangeFlags range) => z.mBoard == b && !z.mDead && !z.IsDeadOrDying()
            && !z.mMindControlled && z.EffectedByDamage(range);
    }
}
