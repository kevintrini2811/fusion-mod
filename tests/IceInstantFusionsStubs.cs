// Offline doubles. No game, Unity player, profile or save is loaded.
namespace HarmonyLib {
 [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch:Attribute { public HarmonyPatch(){} public HarmonyPatch(Type t,string n){} }
 [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority:Attribute {public HarmonyPriority(int n){} }
 public static class Priority {public const int First=800, Last=0;}
 public static class AccessTools {public static System.Reflection.MethodBase Method(Type t,string n)=>t.GetMethod(n)!;}
}
namespace Il2CppReloaded.Services {}
namespace Il2CppReloaded.Gameplay {

 public enum ProjectileType {Pea,PeashooterPea,Fireball,PeashooterFireball,Star}
 public enum PlantOnBungeeState {NotOnBungee,OnBungee}
 public enum PlantState {Notready}
 public enum GameScenes {Playing,Loading}
 public enum GameMode {Adventure,ChallengeZenGarden}
 public enum GridItemType {Crater}
 public enum CoinType {Sun,SmallSun,UsableSeedPacket}
 public enum CoinMotion {FromPlant}
 public enum ParticleEffect {Powie,Doom,IceSparkle}
 public enum FoleyType {Cherrybomb,Explosion,JalapenoIgnite,Frozen}
 [Flags] public enum DamageRangeFlags {Ground=1,Flying=2,Submerged=4,Dog=8,OffGround=16,Dying=32,Underground=64}
 [Flags] public enum DamageFlags {HitsShieldAndBody=1,DoesntLeaveBody=2,BypassesShield=4}
 public readonly record struct Rect(float xMin,float yMin,float xMax,float yMax);
 public sealed class Sounds {public void PlayFoley(FoleyType t){} }
 public sealed class App {
  public GameScenes GameScene=GameScenes.Playing; public GameMode GameMode; public Sounds SoundSystem=new();
  public void AddTodParticle(float x,float y,int o,ParticleEffect p){} }
 public sealed class Board {
  public IntPtr Pointer=(IntPtr)1; public App mApp=new(); public bool mPaused; public int mMainCounter;
  public List<Plant> m_plants=new(); public List<Zombie> m_zombies=new(); public List<GridItem> m_gridItems=new(); public List<Coin> Coins=new();
  public int Rand(int n)=>0; public int GetNumRows()=>5;
  public GridItem AddACrater(int c,int r) {var x=new GridItem {mGridX=c,mGridY=r};m_gridItems.Add(x);return x;}
  public Coin AddCoin(float x,float y,CoinType t,CoinMotion m) {var c=new Coin {mType=t,mUsableSeedType=SeedType.None};Coins.Add(c);return c;}
  public void ShakeBoard(int x,int y){} public void UpdateGame(){} public void DisposeBoard(){}
  public void KillAllZombiesInRadius(int r,float x,float y,float radius,int rows,bool burn,DamageRangeFlags flags){}
 }
 public sealed class GridItem {public bool mDead;public GridItemType mGridItemType;public int mGridX,mGridY,mGridItemCounter,mSunCount; public SeedType mSeedType = SeedType.Wintermelon;}
 public sealed class Coin {public CoinType mType;public SeedType mUsableSeedType = SeedType.Wintermelon;public CoinController mController=new();}
 public sealed class CoinController {public SeedType SeedType = SeedType.Wintermelon;}
 public sealed class Plant {
  public IntPtr Pointer=(IntPtr)2; public uint DataID=2;public Board mBoard=new();public App mApp=>mBoard.mApp;
  public SeedType mSeedType = SeedType.Wintermelon,mImitaterType = SeedType.Wintermelon;public bool mDead,mIsOnBoard=true,mSquished,mIsAsleep,OffGround;
  public PlantOnBungeeState mOnBungeeState;public PlantState mState;public int mStateCountdown,mPlantHealth=300,mPlantMaxHealth=300;
  public int mWakeUpCounter,mLaunchCounter,mStartRow,mTargetX,mTargetY,mDoSpecialCountdown,mPlantCol=3,mRow=2,mRenderOrder;
  public float mX=200,mY=200,mWidth=80,mHeight=80;
  public bool NotOnGround()=>OffGround;
  public void IceZombies(){} public void PlantInitialize(){} public void UpdateAbilities(){} public void UpdateDoomShroom(){} public void UpdatePotato(){}public void DoSpecial(){}public void Fire(){} public void Squish(){}
  public void Die() {
   var patch=typeof(FusionMod.FrozenCherryDeathPatch);var args=new object?[]{this,true,null};
   patch.GetMethod("Prefix",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static)!.Invoke(null,args);
   mDead=true;mIsOnBoard=false;
   patch.GetMethod("Postfix",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static)!.Invoke(null,new[]{(object)this,args[2]});
  }
 }
 public sealed class Zombie {
  public IntPtr Pointer=(IntPtr)3;public uint DataID=3;public Board mBoard=null!;
  public int mRow=2,Health=10000,Damage,mIceTrapCounter,mChilledCounter;public float mPosX=240,mPosY=220;
  public bool mDead,mMindControlled,Immune;public Action? OnDamage;
  public Rect Rect=new(230,210,250,270);public DamageRangeFlags Category=DamageRangeFlags.Ground;
  public bool IsDeadOrDying()=>Health<=0;public bool EffectedByDamage(DamageRangeFlags r)=>(r&Category)!=0;
  public Rect GetZombieRect()=>Rect;public bool CanBeFrozen()=>!Immune;
  public void HitIceTrap(){if(!Immune)mIceTrapCounter=Math.Max(400,mIceTrapCounter);}
  public void ApplyChill(bool force){if(!Immune)mChilledCounter=1400;}
  public void RemoveColdEffects(){mIceTrapCounter=mChilledCounter=0;}
  public void TakeDamage(int damage,DamageFlags flags){Damage+=damage;Health-=damage;OnDamage?.Invoke();}
 }
 public sealed class Projectile {
  public IntPtr Pointer=(IntPtr)4;public uint DataID=4;public Board mBoard=null!;public bool mDead;public int mRow=2;
  public float mPosX=200,mPosY=230,mPosZ,mVelX=3,mCobTargetX;public ProjectileType mProjectileType;
  public void ProjectileInitialize(){}public void ConvertToFireball(){}public void Reset(){mCobTargetX=0;DataID++;mDead=false;}
  public void Update(){}public void DoImpact(){}public void Die(){FusionMod.CherryShooter.Forget(this);mDead=true;}
 }
}
namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class ChomperFusions {internal static bool Active(Plant p)=>!p.mDead&&p.mIsOnBoard&&!p.mSquished&&!p.NotOnGround()&&!p.mBoard.mPaused;}
 internal static class ElementalNuts {internal static void Heal(Board b,bool fire){} }
 internal static class FumeFusions {internal static bool Active(Plant p)=>!p.mDead&&p.mIsOnBoard&&!p.mSquished&&!p.NotOnGround()&&!p.mIsAsleep
  &&p.mOnBungeeState==PlantOnBungeeState.NotOnBungee&&!p.mBoard.mPaused&&p.mApp.GameScene==GameScenes.Playing&&p.mApp.GameMode!=GameMode.ChallengeZenGarden;}
 internal static class SquashFusions {internal static bool Eligible(Board b,Zombie z,DamageRangeFlags r)=>z.mBoard==b&&!z.mDead&&!z.IsDeadOrDying()&&!z.mMindControlled&&z.EffectedByDamage(r);}
 internal static class SnowPea {internal static IntPtr CreatingIceTrap;}
 internal static class SolarShooter {internal static List<int> Suns=new();internal static void DropSun(Board b,float x,float y,int value)=>Suns.Add(value);}
 internal static class ImportedPeaVisuals {internal static void RegisterCherryProjectile(Projectile p){} internal static void RegisterExplodeProjectile(Projectile p){}
  internal static void ForgetProjectile(Projectile p){}internal static void EnsureCherryProjectile(Projectile p){} }
 internal static class MineFusions {internal static void ScatterMines(CherryBombPlants.Death d){}internal static void OnDeath(CherryBombPlants.Death d){} }
 internal static class SolarChomperPlants {internal static void OnDeath(CherryBombPlants.Death d){} }
 internal static class PlayerPlantingScope {internal static IntPtr ConsumingIngredient;}
}
