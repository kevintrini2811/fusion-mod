// Offline doubles. Never load native game code, profiles or saves.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int value) {} }
    public static class Priority { public const int First = 800; }
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HarmonyPatch : Attribute { public HarmonyPatch(Type type, string name) { } }
}
namespace Il2CppReloaded.Gameplay
{
    public enum ParticleEffect { PeaSplat }
    public sealed class App { public int Particles; public void AddTodParticle(float x, float y, int order, ParticleEffect effect) { ++Particles; } }
    public enum ZombieID : uint { None }
    public enum ZombieType { Normal, Gravestone, Target }
    public enum CursorType { Normal, Shovel }
    [Flags] public enum DamageRangeFlags { Ground = 1, Flying = 2 }
    [Flags] public enum DamageFlags { BypassesShield = 1 }
    public sealed class Cursor { public CursorType CursorType; }
    public sealed class Board
    {
        public IntPtr Pointer = (IntPtr)1;
        public bool mPaused;
        public int Light;
        public App mApp = new();
        public Plant? Carried;
        public Cursor mCursorObject = new();
        public List<Plant> m_plants = new();
        public GridPool m_gridItems;
        public Board() { m_gridItems = new(this); }
        public List<Zombie> m_zombies = new();
    }
    public sealed class Plant
    {
        private static uint next;
        public uint DataID = ++next;
        public IntPtr Pointer => (IntPtr)DataID;
        public Board mBoard = new();
        public SeedType mSeedType, mImitaterType = SeedType.None;
        public ZombieID mTargetZombieID;
        public int mPlantHealth, mPlantMaxHealth, mTargetX, mTargetY, mLaunchCounter,
            mLaunchRate, mStateCountdown, mStartRow, mShootingCounter, mDoSpecialCountdown, mRow, mPlantCol;
        public float mX, mY, mWidth = 80, mHeight = 80;
        public bool NotOnGround() => Bungee;
        public bool mDead, mIsAsleep, mIsOnBoard = true, Bungee;
        public void PlantInitialize() { } public void UpdateAbilities() { }
        public void UpdateShooting() { } public void Fire() { } public void MouseDown() { }
    }
    public readonly record struct Rect(float X)
    { public (float x, float y) center => (X, 40); public float xMin => X - 10; public float xMax => X + 10; public float yMin => 0; public float yMax => 80; }
    public sealed class Zombie
    {
        private static uint next;
        public uint DataID = ++next;
        public Board mBoard = null!;
        public int mRow, Damage, Hits, mIceTrapCounter, mChilledCounter, mRenderOrder;
        public bool IsDeadOrDying() => mDead;
        public void ApplyChill(bool value) => mChilledCounter = 1000;
        public float X = 500;
        public bool mDead, mMindControlled, Flying;
        public ZombieType mZombieType;
        public Action? OnDamage;
        public Rect GetZombieRect() => new(X);
        public void TakeDamage(int value, DamageFlags flags)
        { Damage += value; ++Hits; OnDamage?.Invoke(); }
    }
}
namespace LawnGlove
{
    public static class LawnGloveMod
    {
        public static Il2CppReloaded.Gameplay.Plant? GetCarriedPlant(Il2CppReloaded.Gameplay.Board board) => board.Carried;
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal static class ChomperFusions
    {
        internal static bool Active(Plant plant) => !plant.mBoard.mPaused && !plant.mDead
            && plant.mIsOnBoard && !plant.Bungee;
    }
    internal static class FumeSupport { internal const float Unit = 80f / 1.35f; }
    internal static class PumpkinFusions { internal static int LightLevel(Plant plant) => plant.mBoard.Light; }
    internal static class SquashFusions
    {
        internal static bool Eligible(Board board, Zombie zombie, DamageRangeFlags flags) =>
            zombie.mBoard == board && !zombie.mDead && !zombie.mMindControlled && !zombie.Flying;
    }
}
