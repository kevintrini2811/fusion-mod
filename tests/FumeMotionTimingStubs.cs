namespace HarmonyLib {
 [AttributeUsage(AttributeTargets.Class)] internal sealed class HarmonyPatch:Attribute {public HarmonyPatch(){}public HarmonyPatch(Type t,string m){}}
 [AttributeUsage(AttributeTargets.Method)] internal sealed class HarmonyPriority:Attribute {public HarmonyPriority(int p){}}
 internal static class Priority {public const int First=800,Last=0;}
 internal static class AccessTools {public static System.Reflection.MethodInfo Method(Type t,string n)=>t.GetMethod(n)!;}
}
namespace Il2CppReloaded.Constants {internal static class Gameplay {public static double TIME_FOR_UPDATE=.01;}}
namespace Il2CppReloaded.Services {internal sealed class App {public Il2CppReloaded.Gameplay.Board? Board;} internal sealed class SerializationService {public App? App;public bool Deserialize()=>true;}}
namespace Il2CppReloaded.Gameplay {
 internal enum ProjectileType {Pea,ZombiePea}
 internal enum ProjectileMotion {Straight,Threepeater,Star}
 internal enum DamageRangeFlags {Ground}
 internal enum DamageFlags {BypassesShield}
 internal enum GridItemType {ZenTool}
 internal enum GridItemState {Normal}
 internal enum SeedType {None,Cobcannon}
 internal enum ZombieType {Normal,Gravestone,Target}
 internal sealed class Board {
  static int next;public IntPtr Pointer=(IntPtr)(++next);public bool mPaused;public int mMainCounter;public object mApp=new();
  public List<Projectile> m_projectiles=new();public List<Zombie> m_zombies=new();public Pool m_gridItems=new();
  public float GetPosYBasedOnRow(float x,int row)=>row*100;
  public Projectile AddProjectile(float x,float y,int order,int row,ProjectileType type){var p=new Projectile{mBoard=this,mPosX=x,mPosY=y,mRow=row,mProjectileType=type};m_projectiles.Add(p);return p;}
  public void Serialize() {} public void UpdateGame(){}public void InitLevel(){}public void InitSurvivalStage(){}public void DisposeBoard(){}
 }
 internal sealed class Pool:List<GridItem>{public GridItem DataArrayAlloc(){var i=new GridItem();Add(i);return i;}}
 internal sealed class GridItem {
  static uint next;public uint DataID=++next;public float mGoalX,mGoalY;
  public Board mBoard=null!;public bool mDead;public GridItemType mGridItemType;public GridItemState mGridItemState;public SeedType mSeedType;
  public int mSunCount,mGridX,mGridY,mGridItemCounter;public float mX,mY,mPosX,mPosY;
  public void Init(object app,GridItemType t){mGridItemType=t;}public void GridItemDie()=>mDead=true;public void Update(){}public void DrawGridItem(){}
 }
 internal class Plant {public Board mBoard=null!;public IntPtr Pointer=(IntPtr)10;public uint DataID=10;public float mX,mY,mWidth=80,mHeight=80;public int mRow,mRenderOrder;}
 internal sealed class Projectile:Plant {
  static uint next;public Projectile(){DataID=++next;Pointer=(IntPtr)(100+next);}
  public bool mDead;public ProjectileType mProjectileType;public ProjectileMotion mMotionType;public DamageRangeFlags mDamageRangeFlags;
  public float mPosX,mPosY,mVelX,mVelY,mVelZ,mAccZ,mPosZ,mShadowY,mCobTargetX;public int mCobTargetRow;
  public void Die()=>mDead=true;public void UpdateNormalMotion(){}public void CheckForCollision(){}public void ConvertToFireball(){}public void Update(){}public void DoImpact(){}public void DoSplashDamage(){}public void GetDamageFlags(){}
 }
 internal sealed class Zombie:Plant {public bool mDead;public ZombieType mZombieType;public int mBodyHealth=100,mHelmHealth;public Rect GetZombieRect()=>new();public void TakeDamage(int d,DamageFlags f){mBodyHealth-=d;} }
 internal sealed class Rect {public float xMin=1000,xMax=1040;}
}
namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class ScaredContent {public static bool Playing(Board? b)=>b!=null;}
 internal static class FumeVariants {public const float Unit=80f/1.35f;}
 internal static class FusionProjectileTime {public static float SourceVelocity(float value)=>value*(float)(100*FusionTime.NativeTickSeconds);}
 internal static class NutProjectiles {public const int WallopMarker=-100;public static int Family(Projectile p)=>0;public static bool Remember(Projectile p,Zombie z)=>true;}
 internal static class CherrySpreader {public static bool IsAttached(Projectile p)=>false;}
 internal static class KelpProjectiles {public static bool Contains(Projectile p)=>false;}
 internal static class PotatoProjectiles {public static bool Contains(Projectile p)=>false;}
 internal static class RollingProjectiles {public static bool TryShot(Projectile p,out object? shot){shot=null;return false;}}
 internal static class SquashFusions {public static bool Eligible(Board b,Zombie z,DamageRangeFlags r)=>!z.mDead;}
 internal static class ImportedPeaVisuals {public static void EnsureWallopProjectile(Projectile p){}}
}

namespace FusionMod { internal sealed class FusionMod { internal static readonly FusionMod Instance=new(); internal readonly Logger LoggerInstance=new(); internal sealed class Logger { internal void Error(string message) {} } } }
