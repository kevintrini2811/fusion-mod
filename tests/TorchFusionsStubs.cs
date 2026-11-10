// Offline adapters. No Unity, IL2CPP or game process is loaded.
namespace HarmonyLib {
 [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute { public HarmonyPatch() {} public HarmonyPatch(Type t,string n) {} }
 [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int n) {} }
 public static class AccessTools {public static System.Reflection.MethodInfo Method(Type t,string name)=>t.GetMethod(name)!;}
 public static class Priority { public const int First=800; }
}
namespace Il2CppReloaded.Gameplay {
 [Flags] public enum DamageRangeFlags { Ground=1,Flying=2,Dog=4,Submerged=8,OffGround=16 }
 [Flags] public enum DamageFlags { BypassesShield=1,DoesntLeaveBody=0x10,Spike=0x20 }
 public enum ZombieType { Normal,Gravestone,Target,Zamboni,Catapult,Balloon,Boss,Gargantuar,RedeyeGargantuar }
 public enum ZombiePhase { BalloonFlying,BalloonPopping,BalloonWalking }
 public struct Vector2 {public float x,y;public Vector2(float a,float b){x=a;y=b;}}
 public struct Rect { public Vector2 center=>new((xMin+xMax)/2,0);public float yMin=>0;public float yMax=>80;public float xMin,xMax; public Rect(float left,float right){xMin=left;xMax=right;} }
 public enum ProjectileType { Pea,PeashooterPea,Snowpea,Star,Fireball }
 public enum ProjectileMotion { Straight,Threepeater,Star,Puff,Homing }
 public enum ZombieID:uint {}
 public enum PlantState { Notready }
 public enum PlantingReason { Ok,NotHere }
 public sealed class CountingPlants : List<Plant> {
  public int Reads;
  public new Plant this[int index] { get { ++Reads; return base[index]; } set => base[index]=value; }
 }
 public sealed class Board {
  private static long next;public IntPtr Pointer=(IntPtr)(++next);public int RandomValue=100000;public int Rand(int maximum)=>Math.Clamp(RandomValue,0,maximum-1);public int mMainCounter;public bool mPaused,Full,FailPlant; public readonly CountingPlants m_plants=new(); public readonly List<Projectile> Shots=new();public readonly List<Zombie> m_zombies=new();public App mApp=new();
  public List<Projectile> m_projectiles=>Shots;public float GetPosYBasedOnRow(float x,int row)=>0;public Layers GetPlantsOnLawn(int col,int row)=>new();
  public void DisposeBoard(){}
  public int GetNumRows()=>6;
  public Projectile AddProjectile(float x,float y,int order,int row,ProjectileType type) { var p=new Projectile {mBoard=this,mPosX=x,mPosY=y,mRow=row,mProjectileType=type};Shots.Add(p);return p; }
  public Plant? AddPlant(int col,int row,SeedType seed,SeedType marker) { if(FailPlant)return null;var p=new Plant {mBoard=this,mPlantCol=col,mRow=row,mSeedType=seed,mImitaterType=marker};m_plants.Add(p);return p; }
 }
 public sealed class Layers {public Plant? NormalPlant;}
 public sealed class Plant {
  private static uint next;public uint DataID=++next;public void Die()=>mDead=true;
  public Board mBoard=null!;public SeedType mSeedType,mImitaterType;public bool mDead,mIsOnBoard=true;
  public PlantState mState;public int mStateCountdown;public int mDoSpecialCountdown, mLaunchCounter,mStartRow,mPlantHealth,mPlantMaxHealth,mTargetX,mTargetY,mShootingCounter,mPlantCol,mRow,mRenderOrder;
  public float mX,mY,mWidth=80,mHeight=80;public bool NotOnGround()=>false;
  public void PlantInitialize(){} public void Fire(){} public void UpdateAbilities(){} public void UpdateTorchwood(){} public void UpdateSpikeweed(){}
 }
 public sealed class Projectile {
  private static uint next;public uint DataID=++next;public IntPtr Pointer=(IntPtr)next;public ZombieID mTargetZombieID;public int mLastPortalX,mProjectileAge;
  public Board mBoard=null!;public bool mDead;public DamageRangeFlags mDamageRangeFlags;public int mRow,mHitTorchwoodGridX=-1,mRenderOrder;public int mCobTargetRow;public float mCobTargetX,mPosX,mPosY,mPosZ,mVelX=4,mVelY,mShadowY,mVelZ,mAccZ,mRotation;
  public ProjectileType mProjectileType;public ProjectileMotion mMotionType;
  public void Die()=>mDead=true;public void ConvertToPea(int col){mProjectileType=ProjectileType.Pea;mHitTorchwoodGridX=col;}public void ConvertToFireball(int col){mProjectileType=ProjectileType.Fireball;mHitTorchwoodGridX=col;}
  public void Reset(){}public void Update(){}public void UpdateNormalMotion(){}
  public void CheckForCollision(){}public void DoImpact(Zombie zombie){}
 }
 public sealed class Zombie {
  public int mBodyMaxHealth=2000;
  private static uint next; private static long pointer;
  public uint DataID=++next; public IntPtr Pointer=(IntPtr)(++pointer);
  public Board mBoard=null!; public bool mDead,mMindControlled,Flying,Popping,OffGround;
  public ZombiePhase mZombiePhase=>Popping?ZombiePhase.BalloonPopping:Flying?ZombiePhase.BalloonFlying:ZombiePhase.BalloonWalking;
  public int mFlyingHealth=20,mHelmHealth,mRow,Health=1000,ColdClears,Landings,VehicleDeaths,LootDrops;
  public int DamageBonus,DamageDivisor=1; public Action? ArmorCallback;
  public int mX,mRenderOrder;public float mPosY;public float mPosX {get=>X;set=>X=value;}
  public bool EffectedByDamage(DamageRangeFlags flags)=>(Popping||OffGround)?(flags&DamageRangeFlags.OffGround)!=0:Flying?(flags&DamageRangeFlags.Flying)!=0:(flags&DamageRangeFlags.Ground)!=0;
  public float X; public ZombieType mZombieType;
  public bool IsDeadOrDying()=>mDead; public void EatPlant(){} public Rect GetZombieRect()=>new(X,X+20);
  public void TakeDamage(int value,DamageFlags flags) {
   FusionMod.TorchweedBalloonDamageDepthPatch.Prefix(this,out var previous);
   try {
    value=(value+DamageBonus)/DamageDivisor;
    if(mZombieType==ZombieType.Balloon&&IsFlying())value=TakeFlyingDamage(value,flags);
    if(value<=0)return;
    if(mHelmHealth>0)value=TakeHelmDamage(value,flags);
    if(value>0)TakeBodyDamage(value,flags);
   } finally {FusionMod.TorchweedBalloonDamageDepthPatch.Finalizer(previous);}
  }
  public int TakeFlyingDamage(int value,DamageFlags flags) {
   int used=Math.Min(value,mFlyingHealth);mFlyingHealth-=used;
   if(mFlyingHealth==0)LandFlyer(flags);
   var callback=ArmorCallback;ArmorCallback=null;callback?.Invoke();
   return value-used;
  }
  public int TakeHelmDamage(int value,DamageFlags flags) {
   int result=0;if(!FusionMod.TorchweedBalloonHelmOverflowPatch.Prefix(this,ref result))return result;
   int used=Math.Min(value,mHelmHealth);mHelmHealth-=used;return value-used;
  }
  public void TakeBodyDamage(int value,DamageFlags flags) {
   if(FusionMod.TorchweedBalloonBodyOverflowPatch.Prefix(this))Health-=value;
  }
  public void RemoveColdEffects()=>++ColdClears; public void DieWithLoot()=>mDead=true;
  public void ZamboniDeath(DamageFlags flags){mDead=true;++VehicleDeaths;}
  public void CatapultDeath(DamageFlags flags){mDead=true;++VehicleDeaths;}
  public void DropLoot()=>++LootDrops; public bool IsFlying()=>Flying||Popping;
  public void LandFlyer(DamageFlags flags){if(Flying&&(flags&DamageFlags.DoesntLeaveBody)==0){Flying=false;Popping=true;++Landings;}}
 }
}
namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class BeamShooters {internal static bool Active(Plant p)=>!p.mDead&&!p.mBoard.mPaused;}
 internal static class ChomperFusions {internal static bool Active(Plant p)=>BeamShooters.Active(p);}
 internal static class ImportedPeaVisuals {internal static void EnsureWeedProjectile(Projectile p){}internal static void ForgetProjectile(Projectile p){} internal static void EnsureChargeTorchProjectile(Projectile p){} internal static void RegisterRollingProjectile(Projectile p,bool nut){}internal static void RegisterSolarNutBall(Projectile p){}internal static void EnsureRollingProjectile(Projectile p,bool nut){} }
 internal static class PuffShooters {internal const int FlightHeight=40;internal static bool Contains(Projectile p)=>p.mCobTargetX==-460010;}
 internal static class CactusProjectiles {internal static int Family(Projectile p)=>p.mCobTargetX==-1?5:0;}
 internal static class StarProjectiles {internal static int Family(Projectile p)=>(int)p.mCobTargetRow;internal static void Ignite(Projectile p,int col){p.mCobTargetX=-9;p.mHitTorchwoodGridX=col;}}
 internal static class HypnoShooters {
  internal static int Summons;internal static bool Contains(Projectile p)=>p.mCobTargetX is -30 or -60 or -20;internal static bool IsFire(Projectile p)=>p.mCobTargetX==-60;internal static bool FromAlly(Projectile p)=>p.mCobTargetX==-20;
  internal static void Ignite(Projectile p,int col){p.mCobTargetX=-60;p.mHitTorchwoodGridX=col;}
  internal static Zombie SummonFire(Board b,int row,float x){++Summons;return new Zombie {mBoard=b};}
 }
 internal static class FusionService {internal static PlantingReason CheckNativePlacement(Board b,int c,int r,SeedType t)=>b.Full?PlantingReason.NotHere:PlantingReason.Ok;}
 internal static class HypnoPlants {internal static void MindControl(Zombie z)=>z.mMindControlled=true;}
 internal static class PlayerPlantingScope {internal static object? Current;internal static void Consume(Plant p)=>p.mDead=true;}
}

namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class ScaredyFusions {internal const float SourceUnit=80/1.35f;}
 internal static class SquashFusions {internal static bool Eligible(Board b,Zombie z,DamageRangeFlags flags)=>z.mBoard==b&&!z.mDead&&!z.mMindControlled&&(!(z.Popping||z.OffGround)||(flags&DamageRangeFlags.OffGround)!=0);}

}

namespace Il2CppReloaded.Services { public enum FoleyType {FirePea,Splat} }
namespace Il2CppReloaded.Gameplay {
 public sealed class App {public Sound SoundSystem=new();}
 public sealed class Sound {public void PlayFoley(Il2CppReloaded.Services.FoleyType sound) {}}
}

namespace Il2CppReloaded.Constants {public static class Gameplay {public static double TIME_FOR_UPDATE=1d/120d;}}

namespace PlantLibrary {public static class Projectiles {public static void PlaySound(string sound,Il2CppReloaded.Gameplay.Board board){} public static void NotifyBorn(Il2CppReloaded.Gameplay.Projectile p){}}}
namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class PotatoProjectiles {internal static bool Contains(Projectile p)=>false;}
 internal static class CherryShooter {internal static bool IsExplosive(Projectile p)=>false;internal static int ExplosionDamage(Projectile p)=>1800;}
 internal static class ExplodeOShooter {internal static void Detonate(Projectile p,int damage){}}
 internal static class BloverFusions {internal static bool Live(Board b,Zombie z)=>z.mBoard==b&&!z.mDead;}
 internal static class CherryBombPlants {
  internal sealed record Death(Board Board,FusionPlant Identity,uint Id,float X,float Y,int Row,int Order);
  internal static bool CanTriggerDeath(Plant p)=>false;
  internal static void Explode(Death d){}
 }
 internal static class IceInstantFusions {internal static CherryBombPlants.Death Capture(Plant p)=>new(p.mBoard,FusionPlant.From(p),p.DataID,p.mX,p.mY,p.mRow,p.mRenderOrder);}
 internal static class MineFusions {internal static void Blast(Board b,float x,float y,int radius,int damage,int order,int row){}}
 internal static class NutProjectiles {internal static bool Remember(Projectile p,Zombie z)=>true;}
}
