// Offline boundary doubles. Native targeting and animation require the game test.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch(Type type, string method) {} public HarmonyPatch() {} }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute
    { public HarmonyPriority(int priority) {} }
    public static class Priority { public const int First = 800, Last = 0; }
}
namespace Il2CppReloaded.Gameplay
{
    public enum SeedType { None, Chomper, Hypnoshroom }
    public enum PlantState { Ready, ChomperBiting, ChomperBitingGotOne, ChomperBitingMissed, ChomperDigesting, ChomperSwallowing }
    public enum PlantWeapon { Primary, Secondary }
    public enum PlantOnBungeeState { NotOnBungee, OnBungee }
    public enum GameScenes { Playing, Loading }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public enum CoinType { UsableSeedPacket }
    public enum CoinMotion { FromPlant }
    [Flags] public enum DamageRangeFlags { Ground = 1, OnlyMindcontrolled = 128 }
    [Flags] public enum DamageFlags { BypassesShield = 1, DoesntLeaveBody = 2 }
    public readonly record struct Rect(float xMin);
    public sealed partial class App { public GameScenes GameScene; public GameMode GameMode; }
    public sealed partial class Board
    {
        public IntPtr Pointer = (IntPtr)1;
        public bool mPaused, FailCoin, FailPlant;
        public int mMainCounter = 1;
        public readonly App App = new();
        public readonly List<Coin> Coins = new();
        public readonly List<Plant> Plants = new();
        public Action? OnAddPlant;
        public Action? OnAddCoin;
        public int Rand(int range) => 0;
        public Coin? AddCoin(float x, float y, CoinType type, CoinMotion motion)
        {
            OnAddCoin?.Invoke();
            if (FailCoin) return null;
            var coin = new Coin(); Coins.Add(coin); return coin;
        }
        public Plant? AddPlant(int col, int row, SeedType type, SeedType marker)
        {
            OnAddPlant?.Invoke();
            if (FailPlant) return null;
            var plant = new Plant { mBoard = this, mPlantCol = col, mRow = row };
            Plants.Add(plant); return plant;
        }
        public void KillAllPlantsInRadius() {}
    }
    public sealed class CoinController { public SeedType SeedType; }
    public sealed class Coin { public bool mDead; public SeedType mUsableSeedType; public CoinController? mController = new(); }
    public sealed partial class Plant
    {
        public FusionMod.FusionPlant Identity = FusionMod.FusionPlant.HypnoChomper;
        public IntPtr Pointer = (IntPtr)2;
        public uint DataID = 3;
        public Board mBoard = new();
        public App mApp => mBoard.App;
        public bool mDead, mSquished, OffGround, mIsOnBoard = true;
        public PlantOnBungeeState mOnBungeeState;
        public PlantState mState;
        public int mStateCountdown, mTargetX, mTargetY, mLaunchCounter, mLaunchRate,
            mPlantHealth = 300, mPlantMaxHealth = 300, mRow, mPlantCol, mX, mY, mHeight = 80, mRenderOrder;
        public Zombie? Target;
        public Func<Zombie?>? Search;
        public Zombie? FindTargetZombie(int row, PlantWeapon weapon) => Search != null ? Search() : Target;
        public DamageRangeFlags GetDamageRangeFlags(PlantWeapon weapon) => DamageRangeFlags.Ground;
        public bool NotOnGround() => OffGround;
        public void Die() => mDead = true;
        public void Squish() {}
        public void PlantInitialize() {}
        public void UpdateAbilities() {}
        public void UpdateChomper() {}
    }
    public sealed partial class Zombie
    {
        public IntPtr Pointer = (IntPtr)4;
        public uint DataID = 5;
        public Board? mBoard;
        public bool mDead, mMindControlled;
        public int mBodyHealth = 270, mHelmHealth, mShieldHealth;
        public float X;
        public bool IsDeadOrDying() => mDead || mBodyHealth <= 0;
        public Rect GetZombieRect() => new(X);
        public void TakeDamage(int damage, DamageFlags flags) => mBodyHealth -= damage;
        public void EatPlant(Plant plant) {}
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    public readonly partial record struct FusionPlant(int Value)
    {
        public static readonly FusionPlant Other = new(0), HypnoChomper = new(1),
            Chompzilla = new(2), ChompNut = new(3), ChompShooter = new(4), PeaNut = new(5);
        internal static FusionPlant From(Plant plant) => plant.Identity;
    }
    internal static class SolarChomperPlants { internal static void AfterNative(Plant plant, PlantState before) {} }
    internal static class CherryBombPlants { internal static IntPtr DisposingBoard = IntPtr.Zero; }
    internal static class RollingProjectiles { internal static int Spawns; internal static void Spawn(Plant plant, bool large, int chunk = 0) { ++Spawns; } }
    internal static class PlayerPlantingScope
    {
        internal static IntPtr ConsumingIngredient = IntPtr.Zero;
        internal static void Consume(Plant plant) => plant.Die();
    }
    internal static class PerfumeShroom
    { internal static bool Eligible(Zombie zombie) => !zombie.IsDeadOrDying() && !zombie.mMindControlled; }
    internal static class HypnoPlants
    {
        internal static bool MindControl(Zombie zombie)
        { if (!PerfumeShroom.Eligible(zombie)) return false; zombie.mMindControlled = true; return true; }
    }
}
