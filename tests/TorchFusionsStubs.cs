// Offline adapters. No Unity, IL2CPP or game process is loaded.
namespace HarmonyLib {
 [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute { public HarmonyPatch(Type t,string n) {} }
 [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int n) {} }
 public static class Priority { public const int First=800; }
}
namespace Il2CppReloaded.Gameplay {
 public enum DamageRangeFlags { Ground=1 }
 public enum ProjectileType { Pea,PeashooterPea,Snowpea,Star,Fireball }
 public enum ProjectileMotion { Straight,Threepeater }
 public enum PlantingReason { Ok,NotHere }
 public sealed class CountingPlants : List<Plant> {
  public int Reads;
  public new Plant this[int index] { get { ++Reads; return base[index]; } set => base[index]=value; }
 }
 public sealed class Board {
  private static long next;public IntPtr Pointer=(IntPtr)(++next);public int mMainCounter;public bool mPaused,Full,FailPlant; public readonly CountingPlants m_plants=new(); public readonly List<Projectile> Shots=new();
  public void DisposeBoard(){}
  public int GetNumRows()=>6;
  public Projectile AddProjectile(float x,float y,int order,int row,ProjectileType type) { var p=new Projectile {mBoard=this,mPosX=x,mPosY=y,mRow=row,mProjectileType=type};Shots.Add(p);return p; }
  public Plant? AddPlant(int col,int row,SeedType seed,SeedType marker) { if(FailPlant)return null;var p=new Plant {mBoard=this,mPlantCol=col,mRow=row,mSeedType=seed,mImitaterType=marker};m_plants.Add(p);return p; }
 }
 public sealed class Plant {
  private static uint next;public uint DataID=++next;public void Die()=>mDead=true;
  public Board mBoard=null!;public SeedType mSeedType,mImitaterType;public bool mDead,mIsOnBoard=true;
  public int mPlantHealth,mPlantMaxHealth,mTargetX,mTargetY,mShootingCounter,mPlantCol,mRow,mRenderOrder;
  public float mX,mY,mWidth=80,mHeight=80;public bool NotOnGround()=>false;
  public void PlantInitialize(){} public void Fire(){} public void UpdateAbilities(){} public void UpdateTorchwood(){}
 }
 public sealed class Projectile {
  public Board mBoard=null!;public bool mDead;public DamageRangeFlags mDamageRangeFlags;public int mRow,mHitTorchwoodGridX=-1,mRenderOrder;public float mCobTargetX,mCobTargetRow,mPosX,mPosY,mPosZ,mVelX=4,mVelY,mShadowY;
  public ProjectileType mProjectileType;public ProjectileMotion mMotionType;
  public void Die()=>mDead=true;public void ConvertToPea(int col){mProjectileType=ProjectileType.Pea;mHitTorchwoodGridX=col;}public void ConvertToFireball(int col){mProjectileType=ProjectileType.Fireball;mHitTorchwoodGridX=col;}
  public void CheckForCollision(){}
 }
 public sealed class Zombie {public Board mBoard=null!;public bool mDead,mMindControlled;public bool IsDeadOrDying()=>false;public void EatPlant(){} }
}
namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class BeamShooters {internal static bool Active(Plant p)=>!p.mDead&&!p.mBoard.mPaused;}
 internal static class ChomperFusions {internal static bool Active(Plant p)=>BeamShooters.Active(p);}
 internal static class ImportedPeaVisuals {internal static void ForgetProjectile(Projectile p){} }
 internal static class PuffShooters {internal const int FlightHeight=40;}
 internal static class CactusProjectiles {internal static int Family(Projectile p)=>p.mCobTargetX==-1?5:0;}
 internal static class StarProjectiles {internal static int Family(Projectile p)=>(int)p.mCobTargetRow;internal static void Ignite(Projectile p,int col){p.mCobTargetX=-9;p.mHitTorchwoodGridX=col;}}
 internal static class HypnoShooters {
  internal static int Summons;internal static bool Contains(Projectile p)=>p.mCobTargetX is -30 or -60 or -20;internal static bool IsFire(Projectile p)=>p.mCobTargetX==-60;internal static bool FromAlly(Projectile p)=>p.mCobTargetX==-20;
  internal static void Ignite(Projectile p,int col){p.mCobTargetX=-60;p.mHitTorchwoodGridX=col;}
  internal static Zombie SummonFire(Board b,int row,float x){++Summons;return new Zombie {mBoard=b};}
 }
 internal static class FusionService {internal static PlantingReason CheckNativePlacement(Board b,int c,int r,SeedType t)=>b.Full?PlantingReason.NotHere:PlantingReason.Ok;}
 internal static class HypnoPlants {internal static void MindControl(Zombie z)=>z.mMindControlled=true;}
 internal static class PlayerPlantingScope {internal static void Consume(Plant p)=>p.mDead=true;}
}
