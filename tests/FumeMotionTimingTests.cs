using FusionMod;
using Il2CppReloaded.Gameplay;
using Clock=Il2CppReloaded.Constants.Gameplay;
internal static class Program {
 static int checks;
 static void Check(bool value,string message){++checks;if(!value)throw new Exception(message);}
 static void Near(float value,float expected,string message)=>Check(Math.Abs(value-expected)<.003f,$"{message}: {value} != {expected}");
 static readonly System.Reflection.MethodInfo Motion=typeof(FumeProjectileMotionPatch).GetMethod("Prefix",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic)!;
 static Projectile Shot(Board board)=>new(){mBoard=board,mPosX=100,mRow=2,mCobTargetX=NutProjectiles.WallopMarker,mVelX=6*FumeVariants.Unit/100,mMotionType=ProjectileMotion.Star,mProjectileType=ProjectileType.Pea};
 static void Move(Projectile p,int rate,int ticks){Clock.TIME_FOR_UPDATE=1d/rate;for(int i=0;i<ticks;i++){FusionTime.BeginTick(p.mBoard);FumeProjectiles.MoveNut(p);++p.mBoard.mMainCounter;}}
 static void Main(){
  float sourceSpeed=6*FumeVariants.Unit/100;
  foreach(int rate in new[]{100,120}){
   var board=new Board();var shot=Shot(board);Move(shot,rate,rate);Near(shot.mPosX,100+sourceSpeed*100,"Wallop one simulated second");Near(shot.mVelX,sourceSpeed,"saved velocity stays canonical");
   var boosted=Shot(new Board());FumeProjectiles.Record(boosted,true)!.mGridY=3;Move(boosted,rate,rate);Near(boosted.mPosX,100+(sourceSpeed+3*.2f*FumeVariants.Unit/100)*100,"Wallop boost shares custom velocity domain");
   var native=new Projectile{mBoard=new Board(),mMotionType=ProjectileMotion.Straight};FumeProjectiles.Record(native,true)!.mGridY=3;Clock.TIME_FOR_UPDATE=1d/rate;
   for(int i=0;i<rate;i++){FusionTime.BeginTick(native.mBoard);Check((bool)Motion.Invoke(null,new object[]{native})!,"boost preserves native motion owner");++native.mBoard.mMainCounter;}
   Near(native.mPosX,3*.2f*FumeVariants.Unit,"native boost integrates one simulation second");
  }
  var mixed=Shot(new Board());Move(mixed,100,50);Move(mixed,120,60);Near(mixed.mPosX,100+sourceSpeed*100,"Wallop live rate switch preserves displacement");
  mixed.mBoard.mPaused=true;float paused=mixed.mPosX;Move(mixed,120,120);Near(mixed.mPosX,paused,"Wallop pause freezes movement");
  var restored=Shot(new Board());restored.mPosX=mixed.mPosX;restored.mVelX=mixed.mVelX;Move(restored,120,60);Near(restored.mPosX,mixed.mPosX+sourceSpeed*50,"saved custom velocity resumes without reinterpretation");
  Console.WriteLine($"Fume motion timing: {checks} checks passed; actual custom motion and central clock at100/120Hz.");
 }
}
