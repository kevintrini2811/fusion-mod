// Offline doubles. No game, Unity player, profile or save file is loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int value) {} }
    public static class Priority { public const int First = 800; }
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
    public enum ZombieType { Normal, Gargantuar, Boss, Gravestone, Target , Balloon }
    [Flags] public enum DamageRangeFlags { Ground=1, Flying=2, Submerged=4, Dog=8, OffGround=16, Underground=64 }
    [Flags] public enum DamageFlags { HitsShieldAndBody=2, DoesntLeaveBody=16 }
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
        public GridPool m_gridItems;
        public Board() { m_gridItems = new(this); }
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
        public int mPlantCol, mRow = 2, mRenderOrder, mStateCountdown, mTargetX;
        public float mX = 200, mY = 200, mWidth = 80, mHeight = 80;
        public bool mDead, mIsOnBoard = true, mSquished;
        public int mLaunchCounter, mShootingCounter;
        public void UpdateAbilities() { }
        public int mPlantHealth = 300, mPlantMaxHealth = 300;
        public PlantOnBungeeState mOnBungeeState;
        public bool OffGround;
        public bool NotOnGround() => OffGround;
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
        public int mButteredCounter;
        public bool ButterImmune;
        public void ApplyButter() { if (!ButterImmune) mButteredCounter = 400; }

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
            int amount = damage;
            if (!FusionMod.SnowPeaDamagePatchForTests.Apply(this, ref amount)) return;
            FusionMod.OverheatDamagePatch.Prefix(this, ref amount);
            Damage += amount; Health -= amount; OnDamage?.Invoke();
            if (Health <= 0 && !mDead) DieWithLoot();
        }
        public bool Flying = true;
        public bool IsFlying() => Flying;
        public void LandFlyer(DamageFlags flags) => Flying = false;
        public bool ImmuneToChill; public bool CanBeChilled() => !ImmuneToChill; public bool CanBeFrozen() => !ImmuneToFreeze;
        public void ApplyChill(bool ice) {
            if (!ImmuneToFreeze) mChilledCounter = Math.Max(mChilledCounter, 1000);
            FusionMod.OverheatColdPatch.Postfix(this);
        }
        public bool SkipBite;
        public void EatPlant(Plant p) {
            FusionMod.ElementalNutBitePatch.Prefix(this, p, out var before);
            // Native path: skipped bites (ladder, odd chilled tick, etc.) have no HP write.
            if (!SkipBite) { p.mPlantHealth -= 4; if (p.mPlantHealth <= 0) p.mDead = true; }
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
        internal static bool Apply(Il2CppReloaded.Gameplay.Zombie z, ref int damage) {
            object?[] values = [z, damage];
            bool result = (bool)typeof(SnowPeaDamagePatch).GetMethod("Prefix", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.Invoke(null, values)!;
            damage = (int)values[1]!;
            return result;
        }
    }
}

namespace FusionMod { internal static class PuffSpecials {
 internal static bool IsNugget(Il2CppReloaded.Gameplay.Projectile p) => p.mCobTargetX == -470092;
 internal static bool IsIcicle(Il2CppReloaded.Gameplay.Projectile p) => p.mCobTargetX == -470018;
} }
