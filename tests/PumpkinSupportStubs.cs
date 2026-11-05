namespace Il2CppReloaded.Services { public enum FoleyType { Chomp } public sealed class SoundSystem { public void PlayFoley(FoleyType type) {} } }
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch() {} public HarmonyPatch(Type type, string method) {} }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int priority) {} }
    public static class Priority { public const int First=800, Last=0; }
    public static class AccessTools { public static System.Reflection.MethodInfo Method(Type type,string name) => type.GetMethod(name,System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic)!; }
}
namespace Il2CppReloaded.Gameplay
{
    public enum PlantState { Ready, ChomperDigesting }
    public enum PlantWeapon { Primary, Secondary }
    public enum ZombieID : uint { None }
    public enum CursorType { Normal, Other }
    public enum GameScenes { Playing, Other }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public enum PlantOnBungeeState { NotOnBungee, Other }
    [Flags] public enum DamageFlags { BypassesShield=1, HitsShieldAndBody=2 }
    [Flags] public enum DamageRangeFlags { Ground=1, Flying=2, Submerged=4, Dog=8, OffGround=16, Underground=32 }
    public enum ParticleEffect { Powie }
    public enum ZombieType { Normal, Boss, Gravestone, Target }
    public enum GridItemType { ZenTool }
    public enum GridItemState { Normal }
    public sealed class App { public Il2CppReloaded.Services.SoundSystem SoundSystem=new(); public GameMode GameMode; public GameScenes GameScene=GameScenes.Playing; public void AddTodParticle(float x,float y,int order,ParticleEffect effect) {} }
    public sealed class Cursor { public CursorType CursorType; }
    public sealed class Layers { public Plant? PumpkinPlant; }
    public sealed class Board
    {
        private static int next; public IntPtr Pointer=(IntPtr)(++next);
        public bool mPaused; public App mApp=new(); public GameMode GameMode;
        public Cursor mCursorObject=new(); public List<Plant> m_plants=new(); public List<Zombie> m_zombies=new();
        public ItemPool m_gridItems; public int[] mIceTimer=new int[6];
        public Board() { m_gridItems=new(this); }
        public int PixelToGridX(int x,int y)=>x/80; public int PixelToGridY(int x,int y)=>y/100;
        public int GetNumRows()=>5;
        public void MouseDown(int x,int y,int clicks,int player) {} public void DisposeBoard() {}
        public Layers GetPlantsOnLawn(int col,int row)=>new() { PumpkinPlant=m_plants.FirstOrDefault(p=>!p.mDead&&p.mIsOnBoard&&p.mPlantCol==col&&p.mRow==row&&p.mSeedType==SeedType.Pumpkinshell) };
        public int Rand(int max)=>0;
        public Projectile AddProjectile()=>new() { mBoard=this };
    }
    public sealed class Plant
    {
        private static uint next; public uint DataID=++next; public IntPtr Pointer=IntPtr.Zero;
        public SeedType mSeedType,mImitaterType=SeedType.None; public Board mBoard=null!;
        public App mApp=>mBoard.mApp; public int mStateCountdown; public PlantState mState;
        public int mPlantCol,mRow,mTargetX,mTargetY,mStartRow,mShootingCounter,mLaunchCounter,mLaunchRate=150;
        public int mPlantHealth=4000,mPlantMaxHealth=4000,mRenderOrder; public float mX,mY,mWidth=80,mHeight=80;
        public ZombieID mTargetZombieID; public bool mDead,mIsOnBoard=true,mIsAsleep,mSquished;
        public PlantOnBungeeState mOnBungeeState; public bool NotOnGround()=>false;
        public readonly List<PlantWeapon> Shots=new();
        public void Fire(Zombie target,int row,PlantWeapon weapon) { Shots.Add(weapon); mLaunchCounter=7; }
        public Zombie? FindTargetZombie(int row,PlantWeapon weapon)=>mBoard.m_zombies.FirstOrDefault(z=>!z.mDead&&z.mRow==row);
        public void Die()=>mDead=true;
        public void PlantInitialize() {} public void UpdateAbilities() {} public void DoSpecial() {} public void IceZombies() {} public void BurnRow(int row) {}
    }
    public sealed class Projectile { public bool mDead; public Board mBoard=null!; public float mPosY,mShadowY; }
    public sealed record Rect(float xMin,float yMin,float xMax,float yMax);
    public sealed class Zombie
    {
        private static uint next; public uint DataID=++next; public IntPtr Pointer; public Board mBoard=null!;
        public bool mDead,mMindControlled; public ZombieType mZombieType; public int mRow,mRenderOrder,mChilledCounter,mIceTrapCounter,Damage,Cryo;
        public bool IsDeadOrDying()=>mDead; public bool CanBeChilled()=>true;
        public bool EffectedByDamage(DamageRangeFlags range)=>true;
        public Rect GetZombieRect()=>new(100,100,140,180);
        public void RemoveColdEffects() { mChilledCounter=mIceTrapCounter=0; }
        public void ApplyChill(bool ice)=>mChilledCounter=1000;
        public void HitIceTrap()=>mIceTrapCounter=400;
        public void TakeDamage(int amount,DamageFlags flags)=>Damage+=amount;
        public void ApplyBurn() {} public void Update() {} public void EatPlant(Plant plant) {} public void DieWithLoot()=>mDead=true; public void DieNoLoot()=>mDead=true; public void Reset() {}
    }
    public sealed class ItemPool : List<GridItem>
    {
        private readonly Board board; public bool Fail;
        public ItemPool(Board board)=>this.board=board;
        public GridItem? DataArrayAlloc() { if(Fail)return null; var item=new GridItem {mBoard=board};Add(item);return item; }
    }
    public sealed class GridItem
    {
        public Board mBoard=null!; public GridItemType mGridItemType; public GridItemState mGridItemState;
        public SeedType mSeedType; public bool mDead; public int mSunCount,mGridX,mGridItemCounter;
        public float mX,mY,mPosX,mPosY;
        public void Init(App app,GridItemType type)=>mGridItemType=type;
        public void GridItemDie()=>mDead=true; public void Update() {} public void DrawGridItem() {}
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal static class ChomperFusions { internal static bool Active(Plant p)=>p.mBoard!=null&&!p.mBoard.mPaused&&!p.mDead&&p.mIsOnBoard&&!p.mSquished; }
    internal static class ShootingPots { internal static bool Pea(FusionPlant p)=>p.NativeType is SeedType.Peashooter or SeedType.Repeater or SeedType.Splitpea or SeedType.Gatlingpea; }
    internal static class StarFusions { internal static bool HasTarget(Plant p)=>p.mBoard.m_zombies.Count>0; internal static void Fire(Plant p)=>p.Fire(null!,p.mRow,PlantWeapon.Primary); }
    internal static class IceInstantFusions { internal static int Bursts; internal static bool Ice(Board b,int ticks) { ++Bursts;return true; } }
    internal static class ElementalNuts { internal static void Heal(Board b,bool fire,int row=-1) {} }
    internal static class SnowPea { internal static void Hit(Zombie z,int points)=>z.Cryo+=points; }
    internal static class CherryBombPlants { internal sealed record Death(Board Board,FusionPlant Identity,uint Id,int Col,int Row,float X,float Y,int Order); internal static IntPtr DisposingBoard=IntPtr.Zero; internal static bool CanTriggerDeath(Plant p)=>ChomperFusions.Active(p)&&p.mBoard.Pointer!=DisposingBoard; }
    internal static class DoomEffects { internal static int DamageReleased; internal static void Damage(Board b,int row,float x,float y,int order,bool fire,int damage) { DamageReleased=damage;PumpkinSupport.FireEnergy(b,row,damage);PumpkinSupport.HeatedDeath(b,row); } }
    internal static class PuffLobbers { internal static void Fire(Plant p,Zombie z) {} internal static Zombie? Target(Plant p)=>p.FindTargetZombie(p.mRow,PlantWeapon.Primary); }
    internal static class SilverLobbers { internal static void Fire(Plant p,Zombie z) {} }
    internal static class TriLobbers { internal static void Shoot(Plant p) {} }
    internal static class ElementalLobbers { internal static void Fire(Plant p) {} }
    internal static class SquashLobbers { internal static void Fire(Plant p) {} }
    internal static class LobberProjectiles { internal static int ShellShots; internal static void Fire(Plant p,int row,Zombie z,bool edge,int ricochets) {} internal static void FireShell(Plant shell,Plant p,Zombie? z)=>++ShellShots; }
}

namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class SolarChomperPlants { internal static void Sun(Plant plant) {} }
 internal static class FumeVariants { internal const float Unit=80f/1.35f; }
 internal static class MineFusions { internal static void Blast(Board board,float x,float y,float radius,int damage,int order,int? row=null) {} }
}
