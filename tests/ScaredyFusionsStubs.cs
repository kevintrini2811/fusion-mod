// Offline doubles. No Unity player or game assembly is loaded.
namespace HarmonyLib {
 public static class AccessTools { public static System.Reflection.MethodBase Method(Type t,string n)=>t.GetMethod(n)!; }
 [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute { public HarmonyPatch() {} public HarmonyPatch(Type t,string m) {} }
 [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int p) {} }
 public static class Priority { public const int First=800, Last=0; }
}
namespace Il2CppReloaded.Services { public enum FoleyType { Fume, Explosion, SpawnSun, Umbrella, Frozen } public sealed class SoundSystem { public void PlayFoley(FoleyType t) {} } }
namespace Il2CppReloaded.Gameplay {

 public enum PlantState { Notready, Ready, ScaredyshroomLowering, ScaredyshroomScared, ScaredyshroomRaising }
 public enum CoinType { Sun, Gold, SmallSun, Silver } public enum CoinMotion { FromPlant, Coin }
 public sealed class Coin {}
 public enum PlantWeapon { Primary }
 public enum CursorType { Normal, Shovel }
 public enum DamageRangeFlags { None = 0, Ground = 1, Flying = 2, Submerged = 4, Dog = 8, OffGround = 16, Dying = 32, Underground = 64 }
 [Flags] public enum DamageFlags { BypassesShield=1, HitsShieldAndBody=2, DoesntLeaveBody=16 }
 public enum ZombieType { Normal, Gravestone, Target, Boss, Zamboni, Catapult, Bungee, Imp }
 public enum ParticleEffect { FumeCloud, Powie, Doom, CabbageSplat, MelonSplash, ButterSplat, IceSparkle }
 public sealed class App { public Il2CppReloaded.Services.SoundSystem SoundSystem=new(); public int Particles; public void AddTodParticle(float x,float y,int o,ParticleEffect p) { ++Particles; } }
 public sealed class Cursor { public CursorType CursorType; }
 public sealed class Lawn { public Plant? PumpkinPlant, NormalPlant; }
 public sealed class Board { public Lawn GetPlantsOnLawn(int col,int row) => new() { NormalPlant = m_plants.FirstOrDefault(p => p.mPlantCol == col && p.mRow == row && p.mSeedType != SeedType.Pumpkinshell && !p.mDead), PumpkinPlant = m_plants.FirstOrDefault(p => p.mPlantCol == col && p.mRow == row && p.mSeedType == SeedType.Pumpkinshell && !p.mDead) }; public bool StarTarget, SpawnFails; public int mPlantsEaten, Suns; public List<(float X,CoinType Type)> Coins=new(); public int Rand(int n)=>n/2; public Coin AddCoin(float x,float y,CoinType t,CoinMotion m) { if(t==CoinType.Sun) ++Suns; Coins.Add((x,t)); return new(); } public List<Plant> m_plants=new(); public readonly List<Projectile> Projectiles=new(); public int GridToPixelX(int c,int r)=>c*80; public Projectile AddProjectile(float x,float y,int o,int r,ProjectileType t) { var p=new Projectile {mBoard=this,mProjectileType=t,mPosX=x,mPosY=y,mRow=r,mShadowY=GridToPixelY(0,r)+67}; Projectiles.Add(p);return p; } public IntPtr Pointer=(IntPtr)1; public bool mPaused; public int Rows=5; public App mApp=new(); public Cursor mCursorObject=new(); public Plant? Carried; public List<Zombie> m_zombies=new(); public int GetNumRows()=>Rows; public int GridToPixelY(int c,int r)=>r*100; }
 public sealed class Rect { public float xMin,xMax,yMin=200,yMax=280; public Rect(float x) { xMin=x-20;xMax=x+20; } }
 public sealed class Plant {
  private static int next; public IntPtr Pointer=(IntPtr)(++next); public Board mBoard=new(); public SeedType mSeedType = SeedType.Puffshroom,mImitaterType = SeedType.Puffshroom; public uint DataID=1; public PlantState mState; public int mPlantHealth,mPlantMaxHealth,mLaunchRate,mLaunchCounter,mStartRow,mTargetX,mTargetY,mShootingCounter,mRow=2,mPlantCol=2,mRenderOrder;
  public float mX=200,mY=200,mWidth=80,mHeight=80; public bool mDead,mIsAsleep,mIsOnBoard=true,mSquished,Bungee;
  public DamageRangeFlags GetDamageRangeFlags(PlantWeapon w)=>DamageRangeFlags.Ground;
  public bool NotOnGround()=>Bungee;
  public bool FindStarFruitTarget() => mBoard.StarTarget; public void Squish() {}
  public Action? OnDeath;
  public void Die(){
   FusionMod.BoomShroomDeathPatch.Prefix(this,true,out var death);
   mDead=true; OnDeath?.Invoke(); FusionMod.BoomShroomDeathPatch.Postfix(this,death);
  } public void UpdateAbilities() {} public void UpdateScaredyShroom(){} public void UpdateShooting(){} public void PlayIdleAnim(float rate) {} public void PlantInitialize() {} public void UpdateShooter() {} public void Fire() {} public void MouseDown() {}
 }
 public enum ZombieID : uint { Null = 0 }
 public enum ProjectileType { Star, ZombiePea, Basketball, Pea, PeashooterPea, Fireball, PeashooterFireball, Snowpea, Cabbage, Kernel, Butter, Melon, Wintermelon }
 public enum ProjectileMotion { Normal, Star, Lobbed }
 public sealed class Projectile {
  private static uint next; public uint DataID=++next; public IntPtr Pointer; public Board mBoard=new(); public bool mDead;
  public ProjectileType mProjectileType=ProjectileType.Pea; public ProjectileMotion mMotionType;
  public int mX,mRow,mRenderOrder,mCobTargetRow,mHitTorchwoodGridX; public ZombieID mTargetZombieID; public float mPosX; public float mCobTargetX,mVelX,mVelY,mVelZ,mAccZ,mPosZ,mShadowY,mPosY;
  public DamageRangeFlags mDamageRangeFlags;
  public void DoSplashDamage(Zombie zombie) {}
  public Zombie FindCollisionTarget()=>null!; public void Die(){mDead=true;} public void CheckForCollision(){} public void UpdateLobMotion(){} public void DoImpact(){} public void GetDamageFlags(){} public void ConvertToFireball(){} public void Update(){}
  // Native UpdateNormalMotion: Star movement, then pea ground-contact cutoff
  // at 0x6ca1a6 (28f). Other native projectile paths remain outside this double.
  public void UpdateNormalMotion() {
   if (mDead || mBoard.mPaused) return;
   if (mMotionType==ProjectileMotion.Star) {mPosX+=mVelX;mPosY+=mVelY;mShadowY+=mVelY;}
   if (mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea && mShadowY-mPosY<28) {mDead=true;return;}
   foreach(var z in mBoard.m_zombies) {
    if (z.mRow!=mRow || z.mDead || z.mMindControlled) continue;
    var rect=z.GetZombieRect();
    if(mPosX>=rect.xMin&&mPosX<=rect.xMax){z.TakeDamage(20,0);mDead=true;break;}
   }
  }
 }
 public sealed class Zombie {
        public bool IsWalkingBackwards() => mMindControlled;
        public int mChilledCounter;
        public bool IsMovingAtChilledSpeed() => mChilledCounter > 0;
  public Plant? BiteTarget; public Plant? FindPlantTarget(ZombieAttackType type)=>BiteTarget; public void AnimateChewSound() {}
  public float mVelX; public bool mIsEating; public int mButteredCounter; public void ApplyButter(){mButteredCounter=400;} public void DieWithLoot(){mDead=true;} public float mScaleZombie=1;
  public int mBodyHealth=270,mBodyMaxHealth=270,mHelmHealth,mHelmMaxHealth,mShieldHealth,mShieldMaxHealth,mFlyingHealth,mFlyingMaxHealth;
  public void StartMindControlled() { mMindControlled=true; }
  public int mIceTrapCounter; public IntPtr Pointer; public uint DataID; public Board? mBoard; public bool mDead,mMindControlled; public ZombieType mZombieType;
  public float mPosX; public int mX; public int mRow=2,Damage,Hits,PoisonLevels,FreezePoints,Chills,Heats,ColdClears; public float X=400; public DamageRangeFlags Category; public Action? OnDamage;
  public void CanTargetPlant(){} public void EatPlant(){} public bool IsDeadOrDying()=>mDead; public bool EffectedByDamage(DamageRangeFlags r)=>(r & Category)!=0; public Rect GetZombieRect()=>new(X){yMin=mRow*100,yMax=mRow*100+80};
  public void TakeDamage(int n,DamageFlags f) { Damage+=n; ++Hits; OnDamage?.Invoke(); }
  public void ApplyChill(bool b) { ++Chills; } public void RemoveColdEffects() { ++ColdClears; }
 }
}
namespace LawnGlove { public static class LawnGloveMod { public static Il2CppReloaded.Gameplay.Plant? GetCarriedPlant(Il2CppReloaded.Gameplay.Board b)=>b.Carried; } }
namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class ImportedPeaVisuals { internal static (float X,float Y) ScaredyMuzzle(Plant p)=>(p.mX+p.mWidth/2+15,p.mY+p.mHeight/2-10); internal static void EnsureSilverProjectile(Projectile p) {} internal static void EnsureIronProjectile(Projectile p) {} internal static void EnsurePuffLobProjectile(Projectile p) {} internal static void EnsurePuffProjectile(Projectile p) {} internal static void EnsurePuffSpecialProjectile(Projectile p) {} internal static void EnsureScaredyProjectile(Projectile p) {} internal static void EnsureRetaliationProjectile(Projectile p) {} }
 internal static class ChomperFusions { internal static bool Active(Plant p)=>!p.mBoard.mPaused&&!p.mDead&&p.mIsOnBoard&&!p.mSquished&&!p.Bungee; }
 internal static class SquashFusions { internal sealed record Landing(Board Board,float X,float Y,int Order,int Row); internal static bool Eligible(Board b,Zombie z,DamageRangeFlags r)=>z.mBoard?.Pointer==b.Pointer&&!z.mDead&&!z.mMindControlled&&z.EffectedByDamage(r); }
 internal static class PoisonShooters { internal static void AddLevel(Board b,Zombie z) { ++z.PoisonLevels; } }
 internal static class MineFusions {
  internal const DamageRangeFlags MineTargets=DamageRangeFlags.Ground;
  internal static readonly List<(float X,float Y,float Radius,int Damage,int? Row)> Blasts=new();
  internal static void Blast(Board board,float x,float y,float radius,int damage,int order,bool hypno=false,int? row=null,int rowRange=0)
   { Blasts.Add((x,y,radius,damage,row)); }
 }
 internal static class SnowPea { internal static void Hit(Zombie z,int points=10) { z.FreezePoints+=points; } }
 internal static class Overheat { internal static void Apply(Zombie z) { ++z.Heats; } }
}

namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class FumeFusions { internal static bool HasTarget(Plant p,FusionPlant identity) => p.mBoard.m_zombies.Any(z => InRange(p,z,identity));
  internal static bool InRange(Plant p,Zombie z,FusionPlant identity) => Math.Abs(z.mRow-p.mRow)<=(identity==FusionPlant.FumeSpreader?1:0) && z.X>=p.mX+p.mWidth/2 && SquashFusions.Eligible(p.mBoard,z,DamageRangeFlags.Ground);
 }
 internal static class IceInstantFusions { internal static CherryBombPlants.Death Capture(Plant p) => new(p.mBoard,FusionPlant.From(p),p.DataID,p.mPlantCol,p.mRow,p.mX+p.mWidth/2f,p.mY+p.mHeight/2f,p.mRenderOrder); internal static void Freeze(Zombie z,int ticks) { z.mIceTrapCounter=Math.Max(z.mIceTrapCounter,ticks); } }
 internal static class CherryBombPlants {
  internal sealed record Death(Board Board,FusionPlant Identity,uint Id,int Col,int Row,float X,float Y,int Order);
  internal static readonly List<int> Blasts=new(); internal static bool CanTriggerDeath(Plant p)=>!p.mDead&&p.mIsOnBoard&&!p.Bungee&&p.mBoard.Pointer!=DisposingBoard&&PlayerPlantingScope.ConsumingIngredient!=p.Pointer; internal static IntPtr DisposingBoard;
  internal static int Explode(Death d,int freezeTicks=0,float? sourceRadius=null,int damage=1800) { Blasts.Add(damage); return 0; }
 }
}

namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class GutsyShroom { internal static bool IsShot(Projectile p)=>false; }
 internal static class PlayerPlantingScope { internal static IntPtr ConsumingIngredient; }
 internal static class RetaliationNuts { internal static IntPtr JackBoard, RadiusBoard; internal sealed record Hit(Plant Plant, Board Board, uint Id, int Health, FusionPlant Identity,float X,float Y,int Row,int Order); }
}

namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class DoomEffects {
  internal static void AshDamage(Zombie zombie,int damage) => zombie.TakeDamage(damage, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody); internal static int Craters; internal static object LeaveCrater(Board b,int col,int row) { ++Craters; return new(); }
  internal static readonly List<(Board Board,int Row,int Damage,bool Fire)> Calls=new();
  internal static void Damage(Board b,int row,float x,float y,int order,bool fire=false,int damage=1800) { Calls.Add((b,row,damage,fire)); }
 }
}

namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class ShroomSlots {
  internal static List<Plant> Plants(Board board,int col,int row) => board.m_plants.Where(p=>!p.mDead&&p.mIsOnBoard&&p.mPlantCol==col&&p.mRow==row).ToList();
  internal static bool TrySpawn(Board board,FusionPlant identity,int col,int row) {
   if(board.SpawnFails) return false;
   var child=new Plant {mBoard=board,mPlantCol=col,mRow=row,mSeedType=identity.NativeType,mImitaterType=identity.Marker};
   PuffSpecials.Initialize(child);PuffSupport.Initialize(child);board.m_plants.Add(child);return true;
  }
 }
}

namespace FusionMod { internal static class IronProjectiles { internal const int SmallShot=-480090; internal static bool IsSmall(Il2CppReloaded.Gameplay.Projectile p)=>p.mCobTargetX==SmallShot; } }

namespace FusionMod { internal static class SunflowerFusions { internal static bool IsShot(Il2CppReloaded.Gameplay.Projectile p)=>false; } }

namespace FusionMod { internal static class ScaredContent { internal static bool Eyes(Il2CppReloaded.Gameplay.Zombie zombie) => false; } }

namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class CactusProjectiles {
  internal static Projectile? Spawn(Plant plant,float angle=0,bool burst=false) => plant.mBoard.AddProjectile(plant.mX,plant.mY,plant.mRenderOrder,plant.mRow,ProjectileType.Pea);
 }
 internal static class CherrySpreader {
  internal const float BlastRadius=1.5f/1.35f*80;
  internal static readonly Dictionary<Zombie,int> Bombs=new();
  internal static Projectile? AttachedTo(Board board,uint id)=>Bombs.Keys.Any(z=>z.mBoard==board&&z.DataID==id)?new Projectile():null;
  internal static bool Attach(Zombie zombie,int damage) { Bombs[zombie]=damage;return true; }
 }
}
