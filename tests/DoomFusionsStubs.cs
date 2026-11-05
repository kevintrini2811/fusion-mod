// Offline doubles. No Unity player, game process, profile or save is loaded.
namespace HarmonyLib {
 [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute {
  public HarmonyPatch() {} public HarmonyPatch(Type type, string name) {} }
 [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int n) {} }
 public static class Priority { public const int First = 800, Last = 0; }
 public static class AccessTools { public static System.Reflection.MethodBase Method(Type t, string n) => t.GetMethod(n)!; }
}
namespace Il2CppReloaded.Services { }
namespace Il2CppReloaded.Gameplay {
 public enum PlantState { Notready, PotatoRising, PotatoArmed }
 public enum PlantOnBungeeState { NotOnBungee, OnBungee }
 public enum GameScenes { Playing, Loading }
 public enum GameMode { Adventure, ChallengeZenGarden, PuzzleIZombie1, PuzzleIZombieEndless }
 public enum PlantingReason { Ok, NotHere }
 public enum GridItemType { ZenTool, Crater }
 public enum GridItemState { Normal }
 public enum CoinType { Sun }
 public enum CoinMotion { FromPlant }
 public enum ProjectileType { Pea }
 public enum ProjectileMotion { Straight }
 public enum ZombieType { Normal, Boss, Gravestone, Target }
 public enum ParticleEffect { Powie, Doom }
 public enum FoleyType { Explosion, JalapenoIgnite }
 [Flags] public enum DamageRangeFlags { Ground=1, Flying=2, Submerged=4, Dog=8, OffGround=16, Underground=32 }
 [Flags] public enum DamageFlags { HitsShieldAndBody=1, DoesntLeaveBody=2 }
 public readonly record struct Rect(float xMin, float yMin, float xMax, float yMax);
 public sealed class Sounds { public void PlayFoley(FoleyType type) {} }
 public sealed class App {
  public GameScenes GameScene = GameScenes.Playing; public GameMode GameMode; public Sounds SoundSystem = new();
  public void AddTodParticle(float x,float y,int order,ParticleEffect effect) { }
 }
 public sealed class Pool<T> : List<T> where T : new() {
  public T DataArrayAlloc() { var item=new T(); Add(item); return item; }
 }
 public sealed class Board {
  public IntPtr Pointer=(IntPtr)1; public bool mPaused; public App mApp=new(); public int mPlantsEaten, Sun;
  public List<Plant> m_plants=new(); public List<Zombie> m_zombies=new(); public Pool<GridItem> m_gridItems=new();
  public List<Projectile> Shots=new(); public bool FailAllocation;
  public int Rand(int n)=>0; public int GetNumRows()=>5;
  public (Plant? NormalPlant, Plant? PumpkinPlant) GetPlantsOnLawn(int col,int row) =>
   (m_plants.FirstOrDefault(p=>!p.mDead && p.mIsOnBoard && p.mPlantCol==col && p.mRow==row),null);
  public Plant? AddPlant(int col,int row,SeedType type,SeedType marker) {
   if (FailAllocation) return null;
   var p=new Plant { mBoard=this,mPlantCol=col,mRow=row,mSeedType=type,mImitaterType=marker,
    mX=GridToPixelX(col,row),mY=GridToPixelY(col,row) };
   m_plants.Add(p); FusionMod.DoomFusions.Initialize(p); return p;
  }
  public Projectile AddProjectile(float x,float y,int order,int row,ProjectileType type) {
   var p=new Projectile {mBoard=this,mPosX=x,mPosY=y,mRow=row,mProjectileType=type};Shots.Add(p);return p;
  }
  public int GridToPixelX(int col,int row)=>80*col; public int GridToPixelY(int col,int row)=>100*row;
  public void AddCoin(float x,float y,CoinType type,CoinMotion motion)=>Sun+=25;
  public GridItem AddACrater(int col,int row) {var g=new GridItem {mGridItemType=GridItemType.Crater,mGridX=col,mGridY=row,mBoard=this};m_gridItems.Add(g);return g;}
  public void ShakeBoard(int x,int y) { }
 }
 public sealed class Plant {
  private static uint next; public uint DataID=++next; public IntPtr Pointer=IntPtr.Zero;
  public Board mBoard=null!; public App mApp=>mBoard.mApp;
  public SeedType mSeedType,mImitaterType; public bool mDead,mIsOnBoard=true,mIsAsleep,mSquished;
  public PlantOnBungeeState mOnBungeeState; public PlantState mState;
  public int mPlantHealth=300,mPlantMaxHealth=300,mDoSpecialCountdown,mStartRow,mTargetX,mTargetY,mLaunchCounter,mStateCountdown;
  public int mPlantCol,mRow,mRenderOrder; public float mX,mY,mWidth=80,mHeight=80;
  public Plant() {Pointer=(IntPtr)DataID;}
  public bool NotOnGround()=>false;
  public void PlantInitialize() {} public void UpdateAbilities() {} public void UpdateShooting() {} public void DoSpecial() {} public void Squish() {}
  public void Die() {mDead=true;mIsOnBoard=false;}
 }
 public sealed class Zombie {
  private static uint next; public uint DataID=++next; public IntPtr Pointer; public Board mBoard=null!;
  public bool mDead,mMindControlled,Dying; public ZombieType mZombieType; public int mBodyHealth=10000,mRow,mRenderOrder,mYuckyFaceCounter;
  public float X,Y; public int Received; public DamageRangeFlags Category=DamageRangeFlags.Ground;
  public Zombie() {Pointer=(IntPtr)DataID;}
  public bool IsDeadOrDying()=>Dying || mBodyHealth<=0;
  public Rect GetZombieRect()=>new(X,Y,X+30,Y+60);
  public bool EffectedByDamage(DamageRangeFlags flags)=>(flags&Category)!=0;
  public void TakeDamage(int damage,DamageFlags flags) {Received+=damage;mBodyHealth-=damage;}
  public void RemoveColdEffects() {} public void ApplyBurn() {} public void DieWithLoot() {} public void DieNoLoot() {} public void Update() {} public void EatPlant() {}
 }
 public sealed class GridItem {
  public Board mBoard=null!; public bool mDead; public GridItemType mGridItemType; public GridItemState mGridItemState;
  public int mSunCount,mGridX,mGridY,mGridItemCounter; public SeedType mSeedType;public float mX,mY,mPosX,mPosY;
  public void Init(App app,GridItemType type) {mGridItemType=type;}
  public void GridItemDie()=>mDead=true; public void Update(){} public void DrawGridItem(){}
 }
 public sealed class Projectile {
  public Board mBoard=null!; public bool mDead; public ProjectileType mProjectileType;public ProjectileMotion mMotionType;
  public float mCobTargetX,mAccZ,mPosZ,mVelZ,mVelX,mVelY,mPosX,mPosY,mShadowY; public int mCobTargetRow,mRow;
  public DamageRangeFlags mDamageRangeFlags;
  public void Die()=>mDead=true; public void UpdateNormalMotion(){} public void CheckForCollision(){} public void ConvertToFireball(){}
 }
}
namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class PlayerPlantingScope {internal static object? Current;internal static IntPtr ConsumingIngredient=IntPtr.Zero;}
 internal static class FusionService {
  internal static PlantingReason CheckNativePlacement(Board b,int c,int r,SeedType t)=>
   b.m_gridItems.Any(g=>!g.mDead && g.mGridItemType==GridItemType.Crater && g.mGridX==c && g.mGridY==r) ? PlantingReason.NotHere : PlantingReason.Ok;
 }
 internal static class CherryBombPlants {
  internal static IntPtr DisposingBoard;
  internal sealed record Death(Board Board,FusionPlant Identity,uint Id,int Col,int Row,float X,float Y,int Order,int Stage=0);
  internal static bool CanTriggerDeath(Plant p)=>!p.mDead && p.mIsOnBoard && p.mBoard.Pointer!=DisposingBoard
   && p.Pointer!=PlayerPlantingScope.ConsumingIngredient;
 }
 internal static class IceInstantFusions {
  internal static CherryBombPlants.Death Capture(Plant p)=>new(p.mBoard,FusionPlant.From(p),p.DataID,p.mPlantCol,p.mRow,p.mX+40,p.mY+40,p.mRenderOrder);
  internal static bool Eligible(Board b,Zombie z)=>!z.mDead && !z.IsDeadOrDying() && !z.mMindControlled;
  internal static void Freeze(Zombie z,int ticks) { }
 }
 internal static class MineFusions {
  internal static bool Active(Plant p)=>p.mIsOnBoard && !p.mDead && !p.mBoard.mPaused && p.mApp.GameScene==GameScenes.Playing;
  internal static Zombie? Nearest(Plant p,bool contact)=>p.mBoard.m_zombies.FirstOrDefault(z=>!z.mDead && !z.IsDeadOrDying() && z.mRow==p.mRow && Math.Abs(z.X-p.mX)<50);
  internal static void Blast(Board b,float x,float y,float radius,int damage,int order,bool ash=false) {}
 }
 internal static class RetaliationNuts {internal static IntPtr RadiusBoard=IntPtr.Zero,JackBoard=IntPtr.Zero;}
 internal static class ReactivePots {
  internal static void Heal(Plant p,int n)=>p.mPlantHealth=Math.Min(p.mPlantMaxHealth,p.mPlantHealth+n);
  internal static void Fire(Board b,int row) {} internal static bool PreventCrater(Board b,int c,int r)=>false;
 }
 internal static class SquashFusions {
  internal static bool Eligible(Board b,Zombie z,DamageRangeFlags flags)=>!z.mDead && !z.IsDeadOrDying() && !z.mMindControlled && z.EffectedByDamage(flags);
 }
 internal static class ImportedPeaVisuals {internal static void EnsurePotatoSeedProjectile(Projectile p) {}}
 internal static class ScaredContent {internal static bool Playing(Board b)=>b.mApp.GameScene==GameScenes.Playing && b.Pointer!=CherryBombPlants.DisposingBoard;}
 internal static class BloverEffects {internal static bool Reserve(Zombie z,bool cryo)=>true;}
 internal static class PoisonShooters {internal static void PoisonBite(Board b,Zombie z) {}}
 internal static class BucketPlants {internal static bool IsDropIngredient(FusionPlant p)=>false;}
}
