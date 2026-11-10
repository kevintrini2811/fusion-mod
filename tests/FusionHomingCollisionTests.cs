using FusionMod;
using Il2CppReloaded.Gameplay;
using Clock=Il2CppReloaded.Constants.Gameplay;
internal static class Program {
 static int checks;
 static void Check(bool value,string message){++checks;if(!value)throw new Exception(message);}
 static Projectile Shot(int rate,uint identity=1){Clock.TIME_FOR_UPDATE=1d/rate;var b=new Board{Target=new Zombie{DataID=9,TargetX=1000}};var p=new Projectile{mBoard=b,DataID=identity,Pointer=(IntPtr)(100+identity),mTargetZombieID=9,mVelX=200f/rate};FusionNativeTimeRecords.Get(b,FusionNativeTimeRecords.Kind.Projectile,p.DataID,rate);FusionTime.BeginTick(b);return p;}
 // Branch model from native CheckForCollision RVA0x6c6b3e/0x6c6d0b:
 // Homing tests only the saved target; ordinary Star searches generic targets.
 static uint CollisionBranch(Projectile p,bool savedOverlaps,uint genericTarget){
  if(p.mMotionType!=ProjectileMotion.Homing)return genericTarget;
  var z=p.mBoard.ZombieTryToGet(p.mTargetZombieID);
  return z!=null&&z.EffectedByDamage(p.mDamageRangeFlags)&&savedOverlaps?z.DataID:0;
 }
 static int Priority(Type type,string name)=> (int)type.GetMethod(name,System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static)!.GetCustomAttributesData().Single(a=>a.AttributeType.Name=="HarmonyPriority").ConstructorArguments[0].Value!;
 static void Main(){
  foreach(int rate in new[]{100,120}){
   var p=Shot(rate);var state=FusionHomingTime.Prepare(p);Check(CollisionBranch(p,true,10)==10,"unrestored Star counterexample selects bystander");FusionHomingTime.BeforeCollision(p);Check(CollisionBranch(p,true,10)==9,"restored Homing selects actual saved target");Check(CollisionBranch(p,false,10)==0,"bystander cannot intercept outside saved target rectangle");p.mBoard.Target=null;Check(CollisionBranch(p,true,10)==0,"missing saved target cannot become arbitrary generic target");FusionHomingTime.Restore(p,state);Check(p.mMotionType==ProjectileMotion.Homing,"collision identity remains saved after finalizer");
   p=Shot(rate);state=FusionHomingTime.Prepare(p);FusionHomingTime.BeforeCollision(p);p.mMotionType=ProjectileMotion.Star;FusionHomingTime.Restore(p,state);Check(p.mMotionType==ProjectileMotion.Star,"explicit impact conversion survives outer finalizer");
   p=Shot(rate);state=FusionHomingTime.Prepare(p);p.DataID++;FusionHomingTime.BeforeCollision(p);Check(p.mMotionType==ProjectileMotion.Star,"collision restoration does not write recycled generation");FusionHomingTime.Restore(p,state);
   p=Shot(rate);state=FusionHomingTime.Prepare(p);p.mBoard=new Board();FusionHomingTime.BeforeCollision(p);Check(p.mMotionType==ProjectileMotion.Star,"collision restoration does not cross board identity");FusionHomingTime.Restore(p,state);
  }
  var outer=Shot(100);var os=FusionHomingTime.Prepare(outer);var inner=Shot(100,2);var ins=FusionHomingTime.Prepare(inner);FusionHomingTime.BeforeCollision(outer);Check(outer.mMotionType==ProjectileMotion.Star,"inner scope does not restore unrelated outer shot");FusionHomingTime.BeforeCollision(inner);Check(inner.mMotionType==ProjectileMotion.Homing,"inner collision restores only its own identity");FusionHomingTime.Restore(inner,ins);FusionHomingTime.BeforeCollision(outer);Check(outer.mMotionType==ProjectileMotion.Homing,"nested finalizer restores outer collision context");FusionHomingTime.Restore(outer,os);
  var skipped=Shot(100);FusionHomingTimePatch.Prefix(skipped,false,out var noState);Check(noState==null&&skipped.mMotionType==ProjectileMotion.Homing,"skipped native motion creates no substitution scope");
  Check(Priority(typeof(FusionHomingTimePatch),"Prefix")==-3000,"Homing preparation follows default acquisition and age read scopes");
  Check(Priority(typeof(FusionHomingCollisionIdentityPatch),"Prefix")>1800,"collision identity restoration precedes existing First+1000 conversion prefixes");
  Console.WriteLine($"Homing collision boundary: {checks} checks passed; native branch model and actual owner/clock/records.");
 }
}
namespace FusionMod {internal static class FusionProjectileTime {internal static bool Owns(Projectile p)=>p.Owned;internal static float AgeSeconds(Projectile p)=>(float)p.mProjectileAge/FusionNativeTimeRecords.Get(p.mBoard,FusionNativeTimeRecords.Kind.Projectile,p.DataID)!.Rate;}}
