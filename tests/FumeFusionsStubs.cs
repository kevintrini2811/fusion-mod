// Offline doubles. No Unity player or game assembly is loaded.
namespace HarmonyLib {
 public static class AccessTools { public static System.Reflection.MethodInfo Method(Type t,string m)=>t.GetMethod(m)!; }
 [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute { public HarmonyPatch() {} public HarmonyPatch(Type t,string m) {} }
 [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int p) {} }
 public static class Priority { public const int First=800,Last=0; }
}
namespace Il2CppReloaded.Services { public enum FoleyType { Fume, Explosion, Chomp, JalapenoIgnite, Frozen } public sealed class SoundSystem { public void PlayFoley(FoleyType t) {} } }
namespace Il2CppReloaded.Gameplay {

 public enum PlantWeapon { Primary }
 public enum CursorType { Normal, Shovel }
 public enum DamageRangeFlags { Ground, Flying, Submerged=4, Dog=8, OffGround=16, Underground=32 }
 [Flags] public enum DamageFlags { HitsShieldAndBody=2, Freeze=4, Spike=32, BypassesShield=1, DoesntLeaveBody=16 }
 public enum GameScenes { Playing }
 public enum GameMode { Adventure, ChallengeZenGarden, PuzzleIZombie1=10, PuzzleIZombieEndless=20 }
 public enum PlantState { Idle, UmbrellaReflecting }
 public enum CoinType { Sun, SmallSun }
 public enum CoinMotion { FromPlant }
 public sealed class Coin { public uint DataID=1; public Board mBoard=new(); public bool mDead,mIsBeingCollected; public SeedType mUsableSeedType=SeedType.None; public int mTimesDropped,mDisappearCounter,mX,mY; public float mPosX,mPosY; public int Value=25; public bool IsSun()=>true; public int GetSunValue()=>Value; public void Die()=>mDead=true; public void Collect(){} public void Update(){} }
 public enum ZombieType { Boss, Zamboni, Catapult, Normal, Gravestone, Target, Gargantuar, RedeyeGargantuar }
 public enum ParticleEffect { FumeCloud, GloomCloud, Powie, IceSparkle }
 public sealed class TodParticleSystem { public void OverrideColor(string? name,UnityEngine.Color color) {} }
 public sealed class App { public GameScenes GameScene; public GameMode GameMode; public Il2CppReloaded.Services.SoundSystem SoundSystem=new(); public int Particles; public TodParticleSystem AddTodParticle(float x,float y,int o,ParticleEffect p) { ++Particles; return new(); } }
 public sealed class Cursor { public CursorType CursorType; }
 public sealed class Board {
  public Board() { m_gridItems=new(this); }
  public readonly List<Projectile> Projectiles=new(); public List<Projectile> m_projectiles=>Projectiles;
  public readonly List<Plant> m_plants=new(); public GridPool m_gridItems;
  public int mPlantsEaten; public readonly List<Coin> Coins=new(); public Coin AddCoin(float x,float y,CoinType t,CoinMotion m){var c=new Coin {mBoard=this,mPosX=x,mPosY=y};Coins.Add(c);return c;}
  public int PixelToGridX(int x,int y)=>x/80; public int PixelToGridY(int x,int y)=>y/100;
  public bool SpawnFails; public int? RandomValue; public int Rand(int n)=>RandomValue??n/2;
  public int GridToPixelX(int c,int r)=>c*80;
  public Projectile AddProjectile(float x,float y,int o,int r,ProjectileType t) {
   if(SpawnFails)return null!; var p=new Projectile {mBoard=this,mProjectileType=t,mPosX=x,mPosY=y,mRow=r}; Projectiles.Add(p);return p;
  }
  public Plant? AddPlant(int col,int row,SeedType type,SeedType marker) { if(SpawnFails)return null; var p=new Plant {mBoard=this,mPlantCol=col,mRow=row,mSeedType=type,mImitaterType=marker};m_plants.Add(p);return p; }
  public float GetPosYBasedOnRow(float x,int row)=>row*100;
  public IntPtr Pointer=(IntPtr)1; public bool mPaused; public int Rows=5; public App mApp=new(); public Cursor mCursorObject=new(); public Plant? Carried; public List<Zombie> m_zombies=new(); public int GetNumRows()=>Rows; public int GridToPixelY(int c,int r)=>r*100;
 }
 public enum GridItemType { ZenTool }
 public enum GridItemState { Normal }
 public sealed class GridPool : List<GridItem> {
  readonly Board board; public bool Full; public GridPool(Board b) {board=b;}
  public GridItem? DataArrayAlloc(){if(Full)return null;var item=new GridItem{mBoard=board};Add(item);return item;}
 }
 public sealed class GridItem {
        private static uint nextGridItem;
        public uint DataID = ++nextGridItem;
        public IntPtr Pointer = (IntPtr)nextGridItem;
  public Board mBoard=null!; public bool mDead; public GridItemType mGridItemType; public GridItemState mGridItemState;
  public SeedType mSeedType; public int mSunCount,mGridX,mGridY,mGridItemCounter,mRenderOrder; public float mX,mY,mPosX,mPosY;
  public void Init(App app,GridItemType type){mGridItemType=type;} public void GridItemDie()=>mDead=true;
  public void Update(){} public void DrawGridItem(){}
 }
 public sealed class Rect { public (float x,float y) center => ((xMin+xMax)/2,(yMin+yMax)/2); public float xMin,xMax,yMin=200,yMax=280; public Rect(float x) { xMin=x-20;xMax=x+20; } }
 public sealed class Plant {
  static uint next; public uint DataID=++next; public IntPtr Pointer= (IntPtr)next; public void Die()=>mDead=true; public void DoSpecial(){} public void UpdateAbilities(){} public void Squish(){}
  public Board mBoard=new(); public SeedType mSeedType = SeedType.Puffshroom,mImitaterType = SeedType.Puffshroom; public int mDoSpecialCountdown,mPlantHealth,mPlantMaxHealth,mLaunchRate,mLaunchCounter,mStartRow,mTargetX,mTargetY,mShootingCounter,mRow=2,mPlantCol=2,mRenderOrder;
  public App mApp=>mBoard.mApp; public PlantState mState; public void UpdateShooting(){}
  public float mX=200,mY=200,mWidth=80,mHeight=80; public bool mDead,mIsAsleep,mIsOnBoard=true,mSquished,Bungee;
  public DamageRangeFlags GetDamageRangeFlags(PlantWeapon w)=>DamageRangeFlags.Ground;
  public void SetSleeping(bool value)=>mIsAsleep=value; public void PlantInitialize() {} public void UpdateShooter() {} public void Fire() {} public void MouseDown() {}
 }
 public enum ZombieID : uint { None }
 public enum ProjectileType { Pea, PeashooterPea, Snowpea, Fireball, ZombiePea, Cabbage, Melon, Butter, Basketball }
 public enum ProjectileMotion { Straight, Normal=Straight, Star, Threepeater }
 public sealed class Projectile {
  private static uint next; public uint DataID=++next; public IntPtr Pointer=(IntPtr)next; public Board mBoard=new(); public bool mDead;
  public ProjectileType mProjectileType; public ProjectileMotion mMotionType;
  public ZombieID mTargetZombieID; public int mCobTargetRow,mRow,mRenderOrder,mHitTorchwoodGridX=-1; public float mPosX,mRotation;
  public float mCobTargetX,mVelX,mVelY,mVelZ,mAccZ,mPosZ,mShadowY,mPosY;
  public DamageRangeFlags mDamageRangeFlags;
  public void Die()=>mDead=true; public void ConvertToPea(int col){mProjectileType=ProjectileType.Pea;mHitTorchwoodGridX=col;} public void UpdateLobMotion(){} public void CheckForCollision(){} public void UpdateNormalMotion(){} public void DoSplashDamage(){} public void DoImpact(){} public void GetDamageFlags(){} public void ConvertToFireball(){} public void Update(){}
 }
 public sealed class Zombie { public bool mIsEating; public void StopEating() { mIsEating=false; } public void UpdateZombiePosition() {}
        public bool IsWalkingBackwards() => mMindControlled;
        public int mChilledCounter;
        public bool IsMovingAtChilledSpeed() => mChilledCounter > 0;
  public HelmType mHelmType; public ShieldType mShieldType; public IntPtr Pointer; public uint DataID; public Board? mBoard; public bool mDead,mMindControlled; public ZombieType mZombieType;
  public int mBodyHealth=1000000,mBodyMaxHealth=1000000,mHelmHealth,mHelmMaxHealth,mShieldHealth,mShieldMaxHealth; public int mRow=2,Damage,Hits,PoisonLevels,FreezePoints,Chills,Heats,ColdClears; public float mPosX=400; public int mX; public float X=400; public DamageRangeFlags Category; public Action? OnDamage;
  public void DieWithLoot()=>mDead=true; public bool IsDeadOrDying()=>mDead; public bool EffectedByDamage(DamageRangeFlags r)=>r==Category; public Rect GetZombieRect()=>new(X){yMin=mRow*100,yMax=mRow*100+80};
  public int AdditionalHatHealth; public int ShieldHits,HelmHits,EntryHits; public bool Immune,DamageThrows,CancelHelmet; public int DamageDivisor=1; public DamageFlags LastLayerFlags; public Action? OnEntry;
  public void TakeDamage(int n,DamageFlags f) {
   FusionMod.FootballFumeEntryPatch.Prefix(out var state);
   ZombieLibrary.Zombies.Enter();
   try {
    if(FusionMod.FootballFumeDamage.Current!=null) {
     ++EntryHits; OnEntry?.Invoke(); if(DamageThrows)throw new InvalidOperationException("fixture damage exception");if(Immune)return;n/=DamageDivisor;
     int remaining=n;
     if(mShieldHealth>0&&(f&DamageFlags.BypassesShield)==0) {remaining=TakeShieldDamage(n,f);if((f&DamageFlags.HitsShieldAndBody)!=0)remaining=n;}
     if(remaining<=0)return;
     if(mHelmHealth>0)remaining=TakeHelmDamage(n,f);
     if(remaining>0)TakeBodyDamage(remaining,f);
    } else { Damage+=n; mBodyHealth-=n; ++Hits; OnDamage?.Invoke(); }
   } finally { ZombieLibrary.Zombies.Exit(); FusionMod.FootballFumeEntryPatch.Finalizer(null,state); }
  }
  public int TakeShieldDamage(int n,DamageFlags f) {
   int left=0;if(!FusionMod.FootballFumeShieldPatch.Prefix(this,ref left))return left;
   LastLayerFlags=f;++ShieldHits;left=Math.Max(0,n-mShieldHealth);mShieldHealth=Math.Max(0,mShieldHealth-n);OnDamage?.Invoke();return left;
  }
  public int TakeHelmDamage(int n,DamageFlags f) {
   // An isolated external helmet prefix fixture: the real Library owns its
   // own fields and hook. No product code reads this synthetic hat field.
   int left=0;bool original=!CancelHelmet&&FusionMod.FootballFumeHelmPatch.Prefix(this,ref left);
   if(original&&AdditionalHatHealth>0){int absorbed=Math.Min(n,AdditionalHatHealth);AdditionalHatHealth-=absorbed;if(!ZombieLibrary.Zombies.IsIndependent(this)){n-=absorbed;original=n>0;}}
   if(original){LastLayerFlags=f;++HelmHits;left=Math.Max(0,n-mHelmHealth);mHelmHealth=Math.Max(0,mHelmHealth-n);OnDamage?.Invoke();}
   FusionMod.FootballFumeHelmPatch.Postfix(this,n,original,ref left);return left;
  }
  public void TakeBodyDamage(int n,DamageFlags f) {
   if(!FusionMod.FootballFumeBodyPatch.Prefix(this))return;
   LastLayerFlags=f;Damage+=n;mBodyHealth-=n;++Hits;OnDamage?.Invoke();
  }
  public void StartMindControlled()=>mMindControlled=true; public void EatPlant(){} public void ApplyChill(bool b) { ++Chills; } public void RemoveColdEffects() { ++ColdClears; }
 }
}
namespace LawnGlove { public static class LawnGloveMod { public static Il2CppReloaded.Gameplay.Plant? GetCarriedPlant(Il2CppReloaded.Gameplay.Board b)=>b.Carried; } }
namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class PuffShooters { internal const float FlightHeight=40; }
 internal static class ImportedPeaVisuals { internal static (float X,float Y) ScaredyMuzzle(Plant p)=>(p.mX,p.mY); internal static int IceSmoke,NormalSmoke; internal static FusionPlant LastSmokeIdentity; internal static void FumeSmoke(Plant p,FusionPlant identity,int? row=null,float offset=0) {LastSmokeIdentity=identity;if(identity==FusionPlant.FrostShroom)++IceSmoke;else ++NormalSmoke;} internal static void Ray(Plant p)=>p.mTargetY=30; internal static void EnsureStarCapProjectile(Projectile p) {} internal static void EnsureGutsyProjectile(Projectile p) {} internal static void EnsureWallopProjectile(Projectile p) {} internal static void EnsureNutProjectile(Projectile p) {} }
 internal static class PlayerPlantingScope { internal static IntPtr ConsumingIngredient=IntPtr.Zero; internal static void Consume(Plant p)=>p.Die(); }
 internal static class PuffLobbers { internal const int FlightTicks=100; }
 internal static class LobberProjectiles { internal static int Butters; internal static void FireBlubber(Plant p,Zombie z){++Butters;} internal static void Launch(Projectile p,Zombie? z,int ticks){} }
 internal static class LobberImpact { internal static void Apply(Projectile p,Zombie? z,int d,bool a,bool b,int c,bool e){} }
 internal static class ChomperFusions { internal static bool Active(Plant p)=>!p.mBoard.mPaused&&!p.mDead&&p.mIsOnBoard&&!p.mSquished&&!p.Bungee; }
 internal static class SquashFusions { internal static bool Eligible(Board b,Zombie z,DamageRangeFlags r)=>z.mBoard?.Pointer==b.Pointer&&!z.mDead&&!z.mMindControlled&&z.EffectedByDamage(r); }
 internal static class PoisonShooters { internal static void AddLevel(Board b,Zombie z) { ++z.PoisonLevels; } }
 internal static class MineFusions {
  internal static readonly List<(float X,float Y,float Radius,int Damage,int? Row)> Blasts=new();
  internal static void Blast(Board board,float x,float y,float radius,int damage,int order,bool hypno=false,int? row=null,int rowRange=0)
   { Blasts.Add((x,y,radius,damage,row)); }
 }
 internal static class SnowPea { internal static void Hit(Zombie z,int points=10) { z.FreezePoints+=points; } }
 internal static class Overheat { internal static void Apply(Zombie z) { ++z.Heats; } }
}

namespace FusionMod {
 internal static class CherrySpreader { internal static bool IsAttached(Il2CppReloaded.Gameplay.Projectile shot)=>shot.mCobTargetX==-41021; }
 internal static class KelpProjectiles { internal static bool Contains(Il2CppReloaded.Gameplay.Projectile shot)=>shot.mCobTargetX==-540020; }
 internal static class PotatoProjectiles { internal static bool Contains(Il2CppReloaded.Gameplay.Projectile shot)=>false; }
}

namespace UnityEngine { public readonly record struct Color(float r,float g,float b,float a); }

namespace FusionMod {
 internal static class StarProjectiles {
  internal static readonly float[] Angles = {30,-30,180,90,-90};
  internal static readonly List<(float Angle,int Damage)> Children = new();
  internal static Il2CppReloaded.Gameplay.Projectile? Spawn(Il2CppReloaded.Gameplay.Board board,float x,float y,int order,int row,int family,float angle,float speed,int damage) {
   Children.Add((angle,damage)); return null;
  }
 }
}

namespace FusionMod {
 internal static class PumpkinFusions { internal static int Light=0; internal static int LightLevel(Il2CppReloaded.Gameplay.Plant p)=>Light; }
 internal static class RetaliationNuts { internal static IntPtr JackBoard=IntPtr.Zero,RadiusBoard=IntPtr.Zero,FireBoard=IntPtr.Zero; }
 internal static class UmbrellaShell { internal static bool CanHit(Il2CppReloaded.Gameplay.Plant p,Il2CppReloaded.Gameplay.Zombie z)=>SquashFusions.Eligible(p.mBoard,z,Il2CppReloaded.Gameplay.DamageRangeFlags.Ground); }
 internal static class PotBoosts { internal static bool HasSupport(Il2CppReloaded.Gameplay.Plant p,FusionPlant f)=>false; }
}

namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class ChomperVariants { internal static bool CanSwallow(Zombie z)=>!z.mDead&&!z.mMindControlled&&z.mZombieType is not (ZombieType.Boss or ZombieType.Gargantuar or ZombieType.RedeyeGargantuar); }
 internal static class ScaredyFusions { internal const float SourceUnit=80f/1.35f; internal static bool Active(Plant p)=>ChomperFusions.Active(p)&&!p.mIsAsleep; internal static bool Near(Plant p,Zombie z,float range)=>!z.mDead&&!z.mMindControlled&&Math.Abs(z.mRow-p.mRow)<=1&&Math.Abs(z.X-p.mX-p.mWidth/2)<=range*SourceUnit; }
 internal static class RollingProjectiles { internal static int Chunk(Projectile p)=>0; internal static bool TryShot(Projectile p,out object? value){value=null;return false;} internal static int HungryShots; internal static void Spawn(Plant p,bool knockback,int chunk,bool hungry=false){if(hungry)++HungryShots;} }
 internal static class ScaredyProjectiles { internal static void Mark(Projectile p,FusionPlant f,int damage){p.mCobTargetRow=damage;} }
 internal static class CherryShooter { internal static bool TryShot(Projectile p)=>p.mCobTargetX is -41001 or -41011; internal static bool IsExplosive(Projectile p)=>p.mCobTargetX==-41011; internal static void Catalyze(Projectile p)=>p.mCobTargetX=-41011; }
 internal static class CherryBombPlants { internal sealed record Death(Board Board,FusionPlant Identity,uint Id,int Col,int Row,float X,float Y,int Order); internal static bool CanTriggerDeath(Plant p)=>ChomperFusions.Active(p)&&PlayerPlantingScope.ConsumingIngredient!=p.Pointer; }
 internal static class IceInstantFusions { internal static CherryBombPlants.Death Capture(Plant p)=>new(p.mBoard,FusionPlant.From(p),p.DataID,p.mPlantCol,p.mRow,p.mX,p.mY,p.mRenderOrder); internal static void Freeze(Zombie z,int ticks){} }
 internal static class ElementalNuts { internal static void Heal(Board b,bool fire,int? row=null){} }
 internal static class PumpkinSupport { internal static void CheckMine(Plant p) {} internal static bool StoreIce(Board b,int col,int row)=>false; }
}

namespace FusionMod {
 internal static class GlacierShroom { internal static void Tick(Il2CppReloaded.Gameplay.Plant plant) {} }
}

// Public API isolation double. Real added-armor fields and Harmony routing are
// covered by the Zombie Library suite; this fixture checks Fusion's API use.
namespace ZombieLibrary {
 using Il2CppReloaded.Gameplay;
 internal static class Zombies {
  static Zombie? target; static uint id; static Board? board; static int depth;
  internal static int Calls;
  internal static bool IsIndependent(Zombie zombie)=>ReferenceEquals(target,zombie)&&id==zombie.DataID&&ReferenceEquals(board,zombie.mBoard)&&depth==1;
  internal static void Enter(){if(target!=null)++depth;}
  internal static void Exit(){if(target!=null)--depth;}
  internal static void TakeDamageWithIndependentAddedArmor(Zombie zombie,int damage,DamageFlags flags) {
   ++Calls;var previous=(target,id,board,depth);target=zombie;id=zombie.DataID;board=zombie.mBoard;depth=0;
   try {zombie.TakeDamage(damage,flags);}finally{(target,id,board,depth)=previous;}
  }
 }
}
