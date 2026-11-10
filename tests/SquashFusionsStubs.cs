// Offline doubles. No game, Unity player, profile or save file is loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch() { } public HarmonyPatch(Type type, string method) { } }
}
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute
    { public HarmonyPriority(int priority) { } }
    public static class Priority { public const int First = 800, Last = 0; }
    public static class AccessTools
    { public static System.Reflection.MethodBase Method(Type type, string name) => type.GetMethod(name)!; }
}
namespace Il2CppReloaded.Services
{
    public enum FoleyType { JalapenoIgnite, Explosion }
    public sealed class SoundSystem { public int Calls; public void PlayFoley(FoleyType type) => ++Calls; }
}
namespace Il2CppReloaded.Gameplay
{

    public enum PlantState { Ready, ChomperBiting, ChomperBitingGotOne, ChomperBitingMissed, ChomperDigesting, Notready, SquashLook, SquashPreLaunch, SquashRising, SquashFalling, SquashDoneFalling }
    public enum PlantingReason { Ok, NotHere }
    public enum CursorType { Normal, Shovel }
    public enum CoinType { Sun }
    public enum CoinMotion { FromPlant }
    public enum PlantWeapon { Primary }
    public enum PlantOnBungeeState { NotOnBungee, OnBungee }
    public enum GameScenes { Playing, Loading }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public enum ZombieType { JalapenoHead, JackInTheBox, Normal, Zamboni, Catapult, Bobsled, RedeyeGargantuar, Gargantuar, Boss, Gravestone, Target }
    [Flags] public enum DamageRangeFlags { Ground=1, Flying=2, Submerged=4, Dog=8, OffGround=16, Underground=64 }
    [Flags] public enum DamageFlags { HitsShieldAndBody=2, DoesntLeaveBody=16 }
    public enum ParticleEffect { IceSparkle, Powie, Doom }
    public readonly record struct Rect(float xMin, float yMin, float xMax, float yMax);
    public static class Common
    {
        public static float GetRectOverlap(ref Rect a, ref Rect b) =>
            Math.Min(a.xMax, b.xMax) - Math.Max(a.xMin, b.xMin);
    }
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
        public int mMainCounter, mPlantsEaten;
        public App mApp = new();
        public List<Zombie> m_zombies = new();
        public List<Plant> m_plants = new();
        public GridPool m_gridItems;
        public Board() { m_gridItems = new(this); }
        public GridItem AddACrater(int col, int row) { var item = new GridItem { mGridX = col, mGridY = row, mGridItemType = GridItemType.Crater }; m_gridItems.Add(item); return item; }
        public List<Projectile> Projectiles = new();
        public Projectile AddProjectile(float x, float y, int order, int row, ProjectileType type)
        {
            var projectile = new Projectile { mBoard = this, mPosX = x, mPosY = y,
                mProjectileType = type, mRow = row };
            Projectiles.Add(projectile);
            return projectile;
        }
        public int Sun, Coins, Created, FailSpawn;
        public bool ThrowSpawn;
        public PlantingReason Placement;
        public Plant? LastCreated;
        public bool TakeSunMoney(int n, int player) { if (Sun < n) return false; Sun -= n; return true; }
        public void AddSunMoney(int n, int player) => Sun += n;
        public void AddCoin(float x, float y, CoinType type, CoinMotion motion) => ++Coins;
        public void ShakeBoard(int x, int y) { }
        public int PixelToGridX(float x, float y) => (int)(x / 80);
        public int GetNumRows() => 5;
        public void MouseDownWithTool() { }
        public Plant? AddPlant(int col, int row, SeedType type, SeedType marker)
        {
            if (ThrowSpawn) throw new InvalidOperationException("spawn failed");
            if (FailSpawn > 0) { --FailSpawn; return null; }
            ++Created;
            return LastCreated = new Plant { mBoard = this, mPlantCol = col, mRow = row,
                mSeedType = type, mImitaterType = marker };
        }
        public void KillAllPlantsInRadius() { }
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
        public int mPlantHealth = 300, mPlantMaxHealth = 300;
        public PlantOnBungeeState mOnBungeeState;
        public DamageRangeFlags GetDamageRangeFlags(PlantWeapon weapon) => DamageRangeFlags.Ground;
        public Zombie? FindSquashTarget() => null;
        public void DoSquashDamage() { }
        public void UpdateAbilities() { }
        public Rect GetPlantAttackRect(PlantWeapon weapon) => new(mX, mY, mX + mWidth, mY + mHeight);
        public Zombie? FindTargetZombie() => null;
        public int mLaunchCounter;
        public bool NotOnGround() => false;
        public void PlantInitialize() { }
        public void Die() { mDead = true; }
        public void Squish() { mSquished = true; }
    }
    public sealed class Zombie
    {
        public IntPtr Pointer = (IntPtr)3;
        public uint DataID = 1;
        public Board mBoard = null!;
        public ZombieType mZombieType;
        public float mPosY = 200;
        public bool Bouncing, Immobilized;
        public bool IsBouncingPogo() => Bouncing;
        public bool IsImmobilizied() => Immobilized;
        public bool CanBeChilled() => !ImmuneToFreeze;
        public bool CanBeFrozen() => !ImmuneToFreeze;
        public void ApplyChill(bool force) { if (!ImmuneToFreeze) mChilledCounter = 1000; }
        public void DieWithLoot() { mDead = true; }
        public int mRow = 2, Health = 10000, Damage, FreezeCalls, ColdRemovals;
        public int mIceTrapCounter, mChilledCounter, mYuckyFaceCounter;
        public bool mDead, mMindControlled, Dying, ImmuneToFreeze, FailControl;
        public Rect Rect = new(230, 200, 250, 280);
        public DamageRangeFlags Category = DamageRangeFlags.Ground;
        public Action? OnDamage;
        public bool IsDeadOrDying() => Dying || Health <= 0;
        public bool EffectedByDamage(DamageRangeFlags flags) => (flags & Category) != 0;
        public Rect GetZombieRect() => Rect;
        public void TakeDamage(int damage, DamageFlags flags)
        { Damage += damage; Health -= damage; OnDamage?.Invoke(); }
        public void HitIceTrap()
        {
            ++FreezeCalls;
            if (ImmuneToFreeze) return;
            // Native HitIceTrap has separate damage, suppressed by SnowPea.
            if (FusionMod.SnowPea.CreatingIceTrap != Pointer) TakeDamage(20, 0);
            mIceTrapCounter = Math.Max(mIceTrapCounter, 400);
            mChilledCounter = Math.Max(mChilledCounter, 1400);
        }
        public void RemoveColdEffects() { ++ColdRemovals; mIceTrapCounter = mChilledCounter = 0; }
        public void EatPlant() { }
        public void UpdateZombieJackInTheBox() { }
        public void UpdateActions() { }
        public void UpdateZombieJalapenoHead() { }
        public void StartMindControlled() { if (!FailControl) mMindControlled = true; }
    }
}
namespace Il2CppReloaded.Gameplay
{
    public enum ProjectileMotion { Straight, Star }
    public enum ProjectileType { Pea, PeashooterPea, Star, ZombiePea, Basketball, Fireball, PeashooterFireball }
    public sealed class Projectile
    {
        public Board mBoard = null!;
        public ProjectileMotion mMotionType;
        public ProjectileType mProjectileType;
        public DamageRangeFlags mDamageRangeFlags;
        public float mCobTargetX, mPosX, mPosY, mVelX, mVelY, mVelZ, mAccZ, mPosZ, mShadowY;
        public bool mDead;
        public int mRow;
        public void Die() => mDead = true;
        public void Update() { }
        public void CheckForCollision() { }
        public void UpdateLobMotion() { }
    }
}
namespace FusionMod
{
    internal static class GutsyShroom { internal static bool IsShot(Il2CppReloaded.Gameplay.Projectile p) => p.mCobTargetX == -40030; }
    internal static class FusionService
    {
        internal static Il2CppReloaded.Gameplay.PlantingReason CheckNativePlacement(
            Il2CppReloaded.Gameplay.Board board, int col, int row, Il2CppReloaded.Gameplay.SeedType type) => board.Placement;
    }
    internal static class ImportedPeaVisuals { internal static void EnsureRetaliationProjectile(Il2CppReloaded.Gameplay.Projectile p) { } }
    internal static class PlayerPlantingScope { internal static IntPtr ConsumingIngredient; }
    internal static class ChomperFusions
    {
        internal static bool Active(Il2CppReloaded.Gameplay.Plant p) => SquashFusions.Active(p) && !p.NotOnGround();
    }
    internal static class PoisonShooters
    {
        internal static int Bites;
        internal static void PoisonBite(Il2CppReloaded.Gameplay.Board b, Il2CppReloaded.Gameplay.Zombie z) => ++Bites;
    }
    internal static class ElementalNuts { internal static void Heal(Il2CppReloaded.Gameplay.Board board, bool fire, int row = -1) { } }
    internal static class SnowPea
    {
        [ThreadStatic] internal static IntPtr CreatingIceTrap;
        internal static Dictionary<IntPtr,int> Points = new();
        internal static void Hit(Il2CppReloaded.Gameplay.Zombie z, int points) { if (z.CanBeFrozen() && z.mIceTrapCounter == 0) Points[z.Pointer] = Points.GetValueOrDefault(z.Pointer) + points; }
    }
    internal static class CherryBombPlants
    {
        internal sealed record Death(Il2CppReloaded.Gameplay.Board Board, FusionPlant Identity, uint Id,
            int Col, int Row, float X, float Y, int Order);
        internal static IntPtr DisposingBoard;
        internal static bool CanTriggerDeath(Il2CppReloaded.Gameplay.Plant p) =>
            p.mBoard != null && p.mBoard.Pointer != DisposingBoard
            && PlayerPlantingScope.ConsumingIngredient != p.Pointer && !p.mDead && p.mIsOnBoard
            && p.mOnBungeeState == Il2CppReloaded.Gameplay.PlantOnBungeeState.NotOnBungee
            && p.mApp.GameScene == Il2CppReloaded.Gameplay.GameScenes.Playing
            && p.mApp.GameMode != Il2CppReloaded.Gameplay.GameMode.ChallengeZenGarden;
        internal static Death? Last;
        internal static void Explode(Death death) => Last = death;
    }
}

namespace Il2CppReloaded.Gameplay
{

}
namespace UnityEngine { public struct Rect { public float x, width; } }

namespace FusionMod { internal static class IceInstantFusions {
 internal static void Freeze(Il2CppReloaded.Gameplay.Zombie zombie,int ticks) {
  int old=zombie.mIceTrapCounter;var scope=SnowPea.CreatingIceTrap;SnowPea.CreatingIceTrap=zombie.Pointer;
  try {zombie.HitIceTrap();} finally {SnowPea.CreatingIceTrap=scope;}
  if(zombie.CanBeFrozen()&&zombie.mIceTrapCounter>0)zombie.mIceTrapCounter=Math.Max(old,ticks);
 }
} }

namespace FusionMod { internal static class PuffShooters { internal const float FlightHeight = 40; internal static bool Contains(Il2CppReloaded.Gameplay.Projectile p) => p.mCobTargetX is -460010 or -460017 or -460018; } }

namespace FusionMod { internal static class PuffSpecials {
 internal static bool IsNugget(Il2CppReloaded.Gameplay.Projectile p) => p.mCobTargetX == -470092;
 internal static bool IsIcicle(Il2CppReloaded.Gameplay.Projectile p) => p.mCobTargetX == -470018;
} }

namespace FusionMod { internal static class IronProjectiles { internal static bool IsSmall(Il2CppReloaded.Gameplay.Projectile p)=>p.mCobTargetX==-480090; } }

namespace FusionMod { internal static class SunflowerFusions { internal static bool IsShot(Il2CppReloaded.Gameplay.Projectile p)=>false; } }

namespace FusionMod { internal static class ScaredyProjectiles { internal static int Family(Il2CppReloaded.Gameplay.Projectile shot) => shot.mCobTargetX == -422040 ? 3 : 0; } }
namespace FusionMod { internal static class GlacierShroom { internal static bool Contains(Il2CppReloaded.Gameplay.Projectile shot) => shot.mProjectileType == Il2CppReloaded.Gameplay.ProjectileType.Pea && shot.mCobTargetX is <= -614000 and >= -614002; } }
