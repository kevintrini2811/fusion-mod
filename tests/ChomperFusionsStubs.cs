// Offline doubles: no game assembly, Unity or IL2CPP is loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch() {} public HarmonyPatch(Type type, string method) {} }
    public static class AccessTools
    { public static System.Reflection.MethodInfo Method(Type type, string name) => type.GetMethod(name)!; }
}
namespace Il2CppReloaded.Gameplay
{

    public enum PlantState { Ready, ChomperBiting, ChomperDigesting, ChomperSwallowing }
    public enum PlantOnBungeeState { NotOnBungee, OnBungee }
    public enum GameScenes { Playing, Loading }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public enum CursorType { Normal, PlantFromBank }
    public enum PlantingReason { Ok, NotHere }
    public sealed class Layers { public Plant? NormalPlant; }
    public sealed class Cursor { public CursorType CursorType; }
    public enum PlantWeapon { Primary }
    public enum ProjectileType { Pea, PeashooterPea, ZombiePea }
    public enum ProjectileMotion { Straight }
    public enum DamageRangeFlags { Ground }
    [Flags] public enum DamageFlags { BypassesShield=1, DoesntLeaveBody=16 }
    public enum ZombieType { Normal, Boss, Zamboni, Catapult, Gargantuar, RedeyeGargantuar }
    public sealed class App { public GameScenes GameScene; public GameMode GameMode; }
    public readonly record struct Rect(float xMin, float xMax);
    public sealed class Board
    {
        public IntPtr Pointer = (IntPtr)1;
        public bool mPaused;
        public readonly List<Plant> m_plants = new();
        public List<Projectile> m_projectiles => Projectiles;
        public Cursor mCursorObject = new();
        public Layers Layers = new();
        public Layers GetPlantsOnLawn(int col, int row) => Layers;
        public int GetNumRows() => 5;
        public int PixelToGridX(int x, int y) => x / 80;
        public int PixelToGridY(int x, int y) => y / 80;
        public void MouseDown() {}
        public bool CanTakeSunMoney(int amount, int player) => true;
        public bool TakeSunMoney(int amount, int player) => true;
        public Plant AddPlant(int col,int row,SeedType type,SeedType marker) {
            var plant = new Plant {mBoard=this,mSeedType=type,mImitaterType=marker,mPlantCol=col,mRow=row};
            m_plants.Add(plant); return plant;
        }
        public int Rand(int range) => 0;
        public int mMainCounter = 1;
        public readonly List<Zombie> m_zombies = new();
        public readonly List<Projectile> Projectiles = new();
        public App App = new();
        public Func<float,float> Ground = _ => 0;
        public float GetPosYBasedOnRow(float x, int row) => Ground(x);
        public Projectile AddProjectile(float x, float y, int order, int row, ProjectileType type)
        {
            var p = new Projectile { mBoard=this, mPosX=x, mPosY=y, mRow=row,
                Pointer=(IntPtr)(Projectiles.Count+10), DataID=(uint)(Projectiles.Count+10) };
            Projectiles.Add(p); return p;
        }
        public void DisposeBoard() { }
        public void KillAllPlantsInRadius(float x, float y, int radius) { }
    }
    public sealed class Plant
    {
        public SeedType mSeedType = SeedType.Puffshroom, mImitaterType = SeedType.None;
        public PlantState mState;
        public PlantOnBungeeState mOnBungeeState;
        public bool mDead, mSquished, OffGround, mIsOnBoard=true;
        public Board mBoard = new();
        public App mApp => mBoard.App;
        public IntPtr Pointer=(IntPtr)3;
        public uint DataID=3;
        public int mPlantHealth=300, mPlantMaxHealth=300, mLaunchCounter, mLaunchRate, mStateCountdown, mTargetX;
        public int mPlantCol;
        public int mX, mY, mHeight=80, mRow, mRenderOrder;
        public Zombie? Target;
        public Zombie? FindTargetZombie(int row, PlantWeapon weapon) => Target;
        public bool NotOnGround() => OffGround;
        public void Die() => mDead=true;
        public void PlantInitialize() { }
        public void UpdateAbilities() { }
        public void Squish() { }
        public void Fire() { }
    }
    public sealed class Projectile
    {
        public IntPtr Pointer;
        public uint DataID;
        public Board mBoard=null!;
        public bool mDead;
        public int mRow, mCobTargetRow;
        public float mCobTargetX,mPosX,mPosY,mPosZ,mVelX,mVelY,mVelZ,mAccZ,mShadowY,mRotation;
        public ProjectileType mProjectileType;
        public ProjectileMotion mMotionType;
        public DamageRangeFlags mDamageRangeFlags;
        public void Die() => mDead=true;
        public void Reset() { }
        public void Update() {} public void UpdateNormalMotion() { }
        public void CheckForCollision() {} public void ConvertToFireball() {}
    }
    public sealed class Zombie
    {
        public uint DataID;
        public bool mDead,mMindControlled,Eligible=true;
        public int mRow,mBodyHealth=2000,mHelmHealth,mShieldHealth,mX,Hits,Damage;
        public float mPosX;
        public ZombieType mZombieType;
        public bool IsDeadOrDying() => mBodyHealth<=0;
        public bool EffectedByDamage(DamageRangeFlags flags) => Eligible;
        public Rect GetZombieRect() => new(mPosX,mPosX+30);
        public void TakeDamage(int amount, DamageFlags flags) { ++Hits; Damage+=amount; mBodyHealth-=amount; }
    }
}
namespace FusionMod
{
    internal static class FusionService { internal static Il2CppReloaded.Gameplay.PlantingReason CheckNativePlacement(Il2CppReloaded.Gameplay.Board b,int c,int r,Il2CppReloaded.Gameplay.SeedType t) => Il2CppReloaded.Gameplay.PlantingReason.Ok; }
    internal static class SolarChomperPlants { internal static void AfterNative(Il2CppReloaded.Gameplay.Plant plant, Il2CppReloaded.Gameplay.PlantState before) {} }
    internal static class CherryBombPlants { internal static IntPtr DisposingBoard; }
    internal static class PlayerPlantingScope { internal static IntPtr ConsumingIngredient; }
    internal static class ImportedPeaVisuals
    {
        internal static void EnsureRollingProjectile(Il2CppReloaded.Gameplay.Projectile projectile, bool nut) { }
        internal static void RegisterSolarNutBall(Il2CppReloaded.Gameplay.Projectile p) {}
        internal static void RegisterRollingProjectile(Il2CppReloaded.Gameplay.Projectile p,bool nut) { }
        internal static void ForgetProjectile(Il2CppReloaded.Gameplay.Projectile p) { }
    }
}
