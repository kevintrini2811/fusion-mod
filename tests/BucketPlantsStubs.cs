// Offline doubles only. No game, Unity, or IL2CPP assembly is loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch() {} public HarmonyPatch(Type type, string name) {} }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int priority) {} }
    public static class Priority { public const int First=800, Last=0; }
    public static class AccessTools
    { public static System.Reflection.MethodInfo Method(Type type,string name) => type.GetMethod(name)!; }
}
namespace Il2CppReloaded.Services { public enum FoleyType { NewspaperRarrgh } public class SoundSystem { public void PlayFoley(FoleyType type) {} } public class SerializationService { public void Serialize() {} } }
namespace Il2CppReloaded.Gameplay
{

    public enum CursorType { Normal, Glove, Shovel }
    public enum MagnetItemType { None, Pail1, Pail2, Pail3, FootballHelmet1, FootballHelmet2, FootballHelmet3, Door1 }
    public enum PlantOnBungeeState { NotOnBungee, OnBungee }
    public enum PlantState { Ready, Notready }
    public enum GameScenes { Playing, Loading }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public enum PlantingReason { Ok, NotHere }
    public enum ProjectileType { Pea, PeashooterPea, Cabbage, ZombiePea, Basketball }
    public enum ZombiePhase { ZombieNormal, ZombieDying }
    public enum CoinType { Sun }
    public enum CoinMotion { FromPlant }
    public enum GridItemType { ZenTool }
    public enum GridItemState { Normal }
    public enum DamageRangeFlags { Ground }
    [Flags] public enum DamageFlags { None=0, DoesntLeaveBody=16 }
    public enum HelmType { None, Pail, Football }
    public enum ZombieType { Normal, Boss, Zamboni, Catapult, Gargantuar, RedeyeGargantuar, Football }
    public class WidgetManager { public int[] LastMouseX=[0], LastMouseY=[0]; }
    public class App { public Il2CppReloaded.Services.SoundSystem SoundSystem=new(); public WidgetManager WidgetManager=new(); public GameScenes GameScene; public GameMode GameMode; }
    public class Cursor { public CursorType CursorType; }
    public class MagnetItem { public MagnetItemType ItemType; }
    public class Lawn { public Plant? NormalPlant,UnderPlant,PumpkinPlant,FlyingPlant; }
    public class Board
    {
        public bool Water; public bool IsPoolSquare(int col, int row) => Water;
        public IntPtr Pointer=(IntPtr)1;
        public App mApp=new(); public bool mPaused, FailCreation;
        public Cursor mCursorObject=new(); public int mX,mY;
        public List<Plant> m_plants=>Plants.Values.ToList();
        public List<Zombie> m_zombies=new(); public List<Projectile> m_projectiles=new();
        public GridItems m_gridItems=new(); public int Suns;
        public void AddCoin(float x,float y,CoinType type,CoinMotion motion)=>++Suns;
        public int Rand(int maximum)=>(int)(Rand()%(uint)maximum);
        public int GridToPixelX(int col,int row)=>col*80; public int GridToPixelY(int col,int row)=>row*80;
        public Dictionary<(int,int),Plant> Plants=new();
        public PlantingReason Terrain=PlantingReason.Ok;
        public int Created, RandomCalls, SunMoney, mPlantsEaten;
        public void AddSunMoney(int amount, int player) => SunMoney += amount;
        public Queue<uint> RandomValues=new();
        public uint Rand() { ++RandomCalls; return RandomValues.Count==0 ? 0 : RandomValues.Dequeue(); }
        public float GetPosYBasedOnRow(float x,int row)=>row*80;
        public Plant? LastCreated;
        public int GetNumRows()=>5;
        public Lawn GetPlantsOnLawn(int col,int row)=>new(){NormalPlant=Plants.GetValueOrDefault((col,row))};
        public int PixelToGridX(int x,int y)=>x; public int PixelToGridY(int x,int y)=>y;
        public void ClearCursor()=>mCursorObject.CursorType=CursorType.Normal;
        public PlantingReason CanPlantAt(int col,int row,SeedType type)=>
            Plants.TryGetValue((col,row),out var p)&&p.mIsOnBoard&&!p.mSquished&&!p.mDead ? PlantingReason.NotHere : Terrain;
        public Plant? AddPlant(int col,int row,SeedType type,SeedType marker)
        {
            if(FailCreation) return null;
            ++Created;
            return LastCreated=Plants[(col,row)]=new(){mBoard=this,mPlantCol=col,mRow=row,mSeedType=type,mImitaterType=marker};
        }
        public Projectile AddProjectile()=>new();
        public void MouseDown(){} public void DisposeBoard(){} public void UpdateGame(){}
        public void AddZombieInRow(){}
    }
    public class Plant
    {
        public IntPtr Pointer=(IntPtr)3; public uint DataID=3;
        public Board mBoard=new(); public SeedType mSeedType = SeedType.Puffshroom, mImitaterType=SeedType.None;
        public bool mDead,mIsOnBoard=true,mSquished,OffGround,mIsAsleep;
        public int mLaunchCounter,mTargetX,mTargetY,mShootingCounter;
        public int mTargetZombieID;
        public int mStartRow;
        public int mPlantHealth=300,mPlantMaxHealth=300,mPlantCol,mRow,mLaunchRate,mStateCountdown;
        public App mApp=>mBoard.mApp;
        public PlantState mState; public PlantOnBungeeState mOnBungeeState;
        public MagnetItem[] mMagnetItems=[new()];
        public float mX,mY; public int mRenderOrder,mShadowRenderOrder;
        public int CalcRenderOrder()=>mRow*100; public void UpdateReanim(){}
        public bool NotOnGround()=>OffGround;
        public static bool IsUpgrade(SeedType type)=>false;
        public bool IsUpgradableTo(SeedType type)=>false;
        public void Die()=>mDead=true;
        public void PlantInitialize(){} public void UpdatePotato(){} public void UpdateAbilities(){} public void Fire(){}
    }
    public class Projectile
    {
        public IntPtr Pointer=(IntPtr)5; public uint DataID=5;
        public Board mBoard=new(); public bool mDead; public ProjectileType mProjectileType;
        public float mCobTargetX; public void Update(){}
        public int mRow; public float mPosX;
        public void ProjectileInitialize(){} public void DoImpact(){} public void ConvertToFireball(){}
        public void Die(){} public void Reset(){}
    }
    public class Zombie
    {
        public IntPtr Pointer=(IntPtr)7;
        public bool mDead,mMindControlled,Dying,Eligible=true;
        public uint DataID=7;
        public Board mBoard=new();
        public HelmType mHelmType=HelmType.Pail;
        public int mHelmHealth=1100,mBodyHealth=200,mHeight=115,mRow=1;
        public int mSummonCounter,mButteredCounter,mIceTrapCounter,mChilledCounter; public float mVelX=1,mScaleZombie=1;
        public void StartEating()=>mIsEating=true; public void StopEating()=>mIsEating=false; public void StartWalkAnim(int blend){} public void DieNoLoot()=>mDead=true;
        public void UpdateZombieWalking(){} public void FindPlantTarget(){} public bool IsWalkingBackwards()=>false;
        public int mShieldHealth,mX; public float mPosX,mPosY;
        public ZombieType mZombieType;
        public int mGraveX,mGraveY,mBodyMaxHealth,mZombieAge; public bool mHasHead=true,mIsEating;
        public ZombiePhase mZombiePhase;
        public void DropHead(DamageFlags flags) {mHasHead=false;Dying=true;}
        public void ZombieInitialize(){} public void Reset(){} public void Update(){}
        public void DieWithLoot(){} public void MowDown(){} public void ApplyBurn(){}
        public void CheckIfPreyCaught(){} public void EatPlant(Plant plant){} public void EatZombie(Zombie zombie){}
        public bool IsDeadOrDying()=>Dying;
        public bool EffectedByDamage(DamageRangeFlags flags)=>Eligible;
        public void TakeDamage(){}
    }
    public class GridItems : List<GridItem>
    {
        private uint next=1;
        public GridItem DataArrayAlloc() {var item=new GridItem{DataID=next++,Pointer=(IntPtr)next};Add(item);return item;}
    }
    public class GridItem
    {
        public GridItemType mGridItemType; public GridItemState mGridItemState;
        public uint DataID; public IntPtr Pointer; public bool mDead;
        public int mSunCount,mGridX,mGridY,mGridItemCounter,mX,mY;
        public float mPosX,mPosY,mGoalX,mGoalY;
        public void Init(App app,GridItemType type)=>mGridItemType=type;
        public void GridItemDie()=>mDead=true;
        public void Update(){} public void DrawGridItem(){}
    }
}
namespace LawnGlove
{
    using Il2CppReloaded.Gameplay;
    public class LawnGloveMod { public void Toggle() {} public void Activate() {} }
    public static class PlantPlacementRules { public static SeedType TerrainType(SeedType type)=>type; }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal static class ImportedPeaVisuals
    {
        internal static void EnsureEyesZombie(Zombie z){} internal static void ClearEyesZombies(Board? b=null){}
        internal sealed class BucketDropView : IDisposable
        {
            internal BucketDropView(Board board, FusionPlant identity = default) {} internal void Update(float x,float y,float alpha) {}
            public void Dispose() {}
        }
        internal static void ClearBucketCursor(){} internal static void ShowBucketCursor(Board b, FusionPlant identity = default){}
        internal static void EnsureIronProjectile(Projectile p){} internal static void RegisterIronProjectile(Projectile p){} internal static void ForgetProjectile(Projectile p){}
    }
    internal static class ChomperFusions
    {
        internal static bool Active(Plant p)=>p.mBoard!=null&&!p.mBoard.mPaused&&!p.mDead&&p.mIsOnBoard
            &&!p.mSquished&&!p.NotOnGround()&&p.mOnBungeeState==PlantOnBungeeState.NotOnBungee
            &&p.mApp.GameScene==GameScenes.Playing&&p.mApp.GameMode!=GameMode.ChallengeZenGarden;
    }
    internal static class PlayerPlantingScope { internal sealed class Context { internal Board Board=new(); internal int X=0,Y=0,PlayerIndex=0; } internal static Context? Current=null; internal static void Consume(Plant p)=>p.Die(); }
    internal static class CherryBombPlants { internal static IntPtr DisposingBoard; internal static void ExplodeFusion(Plant p){} }
    public class Logger { public void Error(string s){} public void Msg(string s){} }
    public class FusionMod { public static FusionMod Instance=new(); public Logger LoggerInstance=new(); }
}

namespace FusionMod { internal static class MineFusions { internal static void OnFusion(Il2CppReloaded.Gameplay.Plant plant) {} } }

namespace FusionMod { internal static class ThreepeaterFusions { internal static void OnFusion(Il2CppReloaded.Gameplay.Plant plant) {} } }

namespace FusionMod { internal static class IceInstantFusions { internal static void OnIceFusion(Il2CppReloaded.Gameplay.Plant plant) {} internal static void OnFusion(Il2CppReloaded.Gameplay.Plant plant) {} } }

namespace FusionMod { internal static class PuffSupport { internal static bool BiteGnome(Il2CppReloaded.Gameplay.Plant plant,Il2CppReloaded.Gameplay.Zombie zombie) => false; } }
