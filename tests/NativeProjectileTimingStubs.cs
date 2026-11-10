// Native boundary doubles: actual FusionTime and provenance storage are compiled.
namespace HarmonyLib {
 [AttributeUsage(AttributeTargets.Class,AllowMultiple=true)] internal sealed class HarmonyPatch:Attribute { public HarmonyPatch(){} public HarmonyPatch(Type t,string n){} }
 [AttributeUsage(AttributeTargets.Method)] internal sealed class HarmonyPriority:Attribute { public HarmonyPriority(int n){} }
 internal static class Priority { internal const int First=800,Last=0; }
}
namespace Il2CppReloaded.Constants { internal static class Gameplay { internal static double TIME_FOR_UPDATE=.01; } }
namespace Il2CppReloaded.Gameplay {
 internal enum ProjectileType { Pea,Cabbage,Butter,Melon }
 internal enum ProjectileMotion { Straight,Threepeater,Star,Lobbed,Homing }
 internal enum DamageRangeFlags { Ground }
 internal enum ZombieID:uint {}
 internal enum SeedType { None,Pea,Marker }
 internal enum GridItemType { ZenTool,Crater }
 internal enum GridItemState { Normal }
 internal sealed class App { public Board? Board; }
 internal sealed class Board {
  static int next; public IntPtr Pointer=(IntPtr)(++next);
  public App mApp; public bool Playing=true,mPaused; public int mMainCounter;
  public GridItems m_gridItems=new(); public List<Projectile> m_projectiles=new();
  public Board(){mApp=new(){Board=this};}
  public int GridToPixelX(int col,int row)=>col*80;
  public int GridToPixelY(int col,int row)=>row*100;
  public Projectile AddProjectile(){var p=new Projectile{mBoard=this};m_projectiles.Add(p);return p;}
  public void Serialize() {} public void UpdateGame(){} public void InitLevel(){} public void InitSurvivalStage(){} public void DisposeBoard(){}
 }
 internal sealed class GridItems:List<GridItem> { public bool FailAllocation; public GridItem? DataArrayAlloc(){if(FailAllocation)return null;var i=new GridItem();Add(i);return i;} }
 internal sealed class GridItem {
  static uint next; public uint DataID=++next; public Board mBoard=null!; public bool mDead;
  public GridItemType mGridItemType;public GridItemState mGridItemState;
  public int mGridX,mGridY,mGridItemCounter,mSunCount,mX,mY;public float mPosX,mPosY,mGoalX,mGoalY;
  public void Init(App app,GridItemType type){mBoard=app.Board!;mGridItemType=type;}
  public void GridItemDie()=>mDead=true;public void Update(){}public void DrawGridItem(){}
 }
 internal class Plant { public Board mBoard=null!;public IntPtr Pointer=(IntPtr)1;public uint DataID=1; public SeedType Marker;public void Fire(){} }
 internal sealed class Projectile:Plant {
  public bool mDead,Owned=true,CanonicalAge,CustomMotion;public int Flags=FusionMod.FusionProjectileTime.Ballistic;
  public int mProjectileAge,mRow,mHitTorchwoodGridX;
  public float mPosX,mPosY,mPosZ,mVelX,mVelY,mVelZ,mAccZ,mShadowY;
  public ProjectileMotion mMotionType;public ProjectileType mProjectileType;
  public DamageRangeFlags mDamageRangeFlags;public ZombieID mTargetZombieID;
  public void Update(){}public void UpdateNormalMotion(){}public void UpdateLobMotion(){}
 }
 internal sealed class Zombie:Plant {
  public int mButteredCounter,mIceTrapCounter;public bool mIsEating,Backwards,Chilled;public float mVelX;
  public Rect Bounds=new(490,190,510,210);public Rect GetZombieRect()=>Bounds;
  public bool IsWalkingBackwards()=>Backwards;public bool IsMovingAtChilledSpeed()=>Chilled;
 }
 internal readonly record struct Rect(float xMin,float yMin,float xMax,float yMax);
}
namespace Il2CppReloaded.Services { internal sealed class SerializationService { public Il2CppReloaded.Gameplay.App? App; public bool Deserialize()=>true; } }
namespace FusionMod {
 internal readonly record struct FusionPlant(Il2CppReloaded.Gameplay.SeedType Marker) { internal static FusionPlant From(Il2CppReloaded.Gameplay.Plant p)=>new(p.Marker); }
 internal static class ScaredContent { internal static bool Playing(Il2CppReloaded.Gameplay.Board? b)=>b?.Playing==true; }
 internal sealed class FusionMod { internal static FusionMod Instance=new();internal Log LoggerInstance=new();internal sealed class Log { internal int Errors;internal void Error(string s){++Errors;} } }
 // Identity partition is independently reviewed; these explicit test flags
 // exercise native age/motion and fully canonical owners without game assets.
 internal static class FusionProjectileDomains {
  internal static bool CanonicalAge(Il2CppReloaded.Gameplay.Projectile p)=>p.CanonicalAge;
  internal static bool CustomMotion(Il2CppReloaded.Gameplay.Projectile p)=>p.CustomMotion;
  internal static bool TryFlags(Il2CppReloaded.Gameplay.Projectile p,out int f){f=p.Flags;return p.Owned&&!p.CanonicalAge;}
 }
 internal static class PuffLobbers {internal const float Gravity=.058074f;}
 internal static class PuffShooters {internal const float FlightHeight=60;}
}
