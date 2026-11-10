// Offline adapters shared with the fertilizer regression project.
namespace Il2CppReloaded.Gameplay {
 public enum CoinType { SmallSun,Sun,LargeSun,Silver }
 public enum CoinMotion { FromPlant,Coin }
 public sealed class Coin { }
 public enum ProjectileType { Pea }
 public enum ProjectileMotion { Star }
 public enum PlantWeapon { Primary }
 public enum ParticleEffect { Powie }
 public enum ZombieType { Normal,Balloon }
 public partial class App { public void AddTodParticle(float x,float y,int order,ParticleEffect effect){} }
 public partial class Board {
  public int mFogBlownCountDown; public int Roll; public Queue<int> Rolls=new();
  public List<CoinType> Coins=new(); public List<Projectile> Shots=new(); public List<int> SmallSuns=new();
  public int Rand(int max)=>Rolls.Count>0?Rolls.Dequeue()%max:Roll%max;
  public Coin AddCoin(float x,float y,CoinType type,CoinMotion motion) { Coins.Add(type); return new Coin(); }
  public void ShakeBoard(int x,int y){}
 }
 public partial class Plant {
  public int mDoSpecialCountdown; public bool mIsAsleep;
  public void PlantInitialize(){} public void UpdateShooter(){} public void UpdateShooting(){} public void Fire(){} public void DoSpecial(){}
 }
 public partial class Zombie {
  public Board mBoard=null!; public bool mMindControlled; public ZombieType mZombieType;
  public int Damage,Thaws; public Action? OnDamage;
  public void RemoveColdEffects()=>++Thaws;
  public void DieNoLoot()=>mDead=true;
 }
 public class Projectile {
  public bool mDead; public Board mBoard=null!; public ProjectileType mProjectileType;
  public ProjectileMotion mMotionType; public float mCobTargetX,mPosX,mPosY,Angle,Speed;
  public void Die()=>mDead=true;
  public Plant? FindCollisionTargetPlant() => null;
  public DamageFlags GetDamageFlags(Zombie z)=>DamageFlags.HitsShieldAndBody;
  public void DoImpact(Zombie z){} public void ConvertToFireball(){} public void Update(){}
 }
}
namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class ChomperFusions {
  internal static void Fertilize(Plant plant) {}
  internal static bool Active(Plant p)=>!p.mDead&&p.mIsOnBoard&&!p.mSquished&&!p.NotOnGround()
   &&p.mOnBungeeState==PlantOnBungeeState.NotOnBungee&&!p.mBoard.mPaused
   &&p.mApp.GameScene==GameScenes.Playing&&p.mApp.GameMode!=GameMode.ChallengeZenGarden
   &&p.mBoard.Pointer!=CherryBombPlants.DisposingBoard;
 }
 internal static class CherryBombPlants {
  internal static IntPtr DisposingBoard;
  internal static bool CanTriggerDeath(Plant p)=>ChomperFusions.Active(p)&&PlayerPlantingScope.ConsumingIngredient!=p.Pointer;
 }
 internal static class ShroomSlots { internal static bool Live(Plant p)=>ChomperFusions.Active(p); }
 internal static class SquashFusions {
  internal static bool Eligible(Board b,Zombie z,DamageRangeFlags flags)=>!z.mDead&&!z.mMindControlled&&z.mBoard==b;
 }
 internal static class RetaliationProjectiles {
  internal static Projectile Spawn(Board b,float x,float y,int order,int row,bool star,float angle,float speed){
   var p=new Projectile{mBoard=b,mPosX=x,mPosY=y,Angle=angle,Speed=speed};b.Shots.Add(p);return p;
  }
 }
 internal static class ImportedPeaVisuals { internal static void EnsureBloverSunProjectile(Projectile p){} }
 internal static class SolarShooter { internal static void DropSun(Board b,float x,float y,int value)=>b.SmallSuns.Add(value); }
}

namespace FusionMod { internal static class SquashLobbers { internal static void Fertilize(Il2CppReloaded.Gameplay.Plant plant) { } } }

namespace FusionMod {
 internal static class EatingShrooms { internal static void Fertilize(Il2CppReloaded.Gameplay.Plant plant) { } }
 internal static class Spicicle { internal static void Fertilize(Il2CppReloaded.Gameplay.Plant plant) { } }
}
