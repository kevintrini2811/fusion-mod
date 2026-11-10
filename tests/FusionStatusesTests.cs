using FusionMod;
using Il2CppReloaded.Gameplay;
using Clock = Il2CppReloaded.Constants.Gameplay;
internal static class Program {
 static int checks;
 static void Check(bool value,string message) { ++checks; if(!value) throw new Exception(message); }
 static Zombie New(int rate) { Clock.TIME_FOR_UPDATE=1d/rate; var board=new Board(); var z=new Zombie{mBoard=board}; board.m_zombies.Add(z); return z; }
 static FusionNativeTimeRecords.Entry? Record(Zombie z)=>FusionNativeTimeRecords.Get(z.mBoard,FusionNativeTimeRecords.Kind.Status,z.DataID);
 static void Tick(Zombie z) {
  FusionTime.BeginTick(z.mBoard); FusionStatuses.PrepareBoard(z.mBoard);
  if(!z.mBoard.mPaused) { if(z.mButteredCounter>0)--z.mButteredCounter; if(z.mIceTrapCounter>0)--z.mIceTrapCounter; if(z.mChilledCounter>0)--z.mChilledCounter; }
  FusionStatuses.AfterUpdate(z); ++z.mBoard.mMainCounter;
 }
 static void Rate(Zombie z,int rate) { Clock.TIME_FOR_UPDATE=1d/rate; FusionStatuses.PrepareBoard(z.mBoard); }
 static void Main() {
  foreach(int rate in new[]{100,120}) {
   var z=New(rate); FusionStatuses.Butter(z,200); Check(z.mButteredCounter==2*rate,"two-second butter native boundary");
   for(int i=0;i<2*rate-1;i++)Tick(z); Check(z.mButteredCounter==1,"butter does not expire early"); Tick(z); Check(z.mButteredCounter==0&&Record(z)==null,"butter expiry clears record");
   z=New(rate);FusionStatuses.Freeze(z,400,1400);Check(z.mIceTrapCounter==4*rate&&z.mChilledCounter==14*rate,"explicit freeze and chill durations");
   z=New(rate);FusionStatuses.Chill(z);Check(z.mChilledCounter==10*rate,"native normal chill default1000 proven");
   z=New(rate);FusionStatuses.Chill(z,true);Check(z.mChilledCounter==20*rate,"native long chill default2000 proven");
  }
  var global=New(120); FusionStatuses.GlobalFreeze(new Plant{mBoard=global.mBoard});Check(global.mIceTrapCounter==480&&global.mChilledCounter==1680,"explicit Fusion native IceZombies scope converts only its own applications");
  var ordinary=New(120);new Plant{mBoard=ordinary.mBoard}.IceZombies();Check(ordinary.mIceTrapCounter==400&&ordinary.mChilledCounter==2000&&Record(ordinary)==null,"unmodified native IceZombies durations remain native");
  var implicitShot=New(120);var shot=new Projectile{mBoard=implicitShot.mBoard,Owned=true};FusionStatusImpactScope.Prefix(shot,implicitShot,out var impactState);implicitShot.ApplyChill(false);implicitShot.ApplyButter();Check(implicitShot.mChilledCounter==1200&&implicitShot.mButteredCounter==480,"known Fusion native impact adopts native chill/butter defaults");
  var unknown=new Projectile{mBoard=implicitShot.mBoard};FusionStatusImpactScope.Prefix(unknown,implicitShot,out var nestedState);implicitShot.ApplyChill(true);Check(implicitShot.mChilledCounter==2000,"nested unknown native impact retains native defaults");FusionStatusImpactScope.Finalizer(nestedState);implicitShot.ApplyChill(true);Check(implicitShot.mChilledCounter==2400,"nested impact finalizer restores owned parent scope");FusionStatusImpactScope.Finalizer(impactState);Check(FusionStatuses.ImpactTarget==null,"impact scope finalizer restores empty context");
  var impactVictim=New(120);var bystander=new Zombie{mBoard=impactVictim.mBoard,DataID=99,Pointer=(IntPtr)999};impactVictim.mBoard.m_zombies.Add(bystander);FusionStatuses.Butter(bystander);FusionStatusImpactScope.Prefix(new Projectile{mBoard=impactVictim.mBoard,Owned=true},impactVictim,out var targetScope);bystander.ApplyButter();Check(bystander.mButteredCounter==400&&Record(bystander)==null,"foreign same-board callback target is not attributed to primary impact");impactVictim.ApplyButter();Check(impactVictim.mButteredCounter==480,"actual primary impact target keeps owned native defaults");impactVictim.DataID++;impactVictim.ApplyButter();Check(impactVictim.mButteredCounter==400&&Record(impactVictim)==null,"reused impact target generation is not attributed");FusionStatusImpactScope.Finalizer(targetScope);
  var callbackOwner=New(100);var callbackTarget=new Zombie{mBoard=callbackOwner.mBoard,DataID=2,Pointer=(IntPtr)999};callbackOwner.mBoard.m_zombies.Add(callbackTarget);FusionStatuses.Butter(callbackTarget);callbackOwner.ButterCallback=()=>callbackTarget.ApplyButter();FusionStatuses.Butter(callbackOwner);Check(Record(callbackTarget)==null&&Record(callbackOwner)!=null,"Fusion scope binds board pointer and full owner ID; nested native writer on another zombie relinquishes provenance");
  var mixed=New(100);FusionStatuses.Butter(mixed,200);for(int i=0;i<37;i++)Tick(mixed);
  var units=Record(mixed)!.A;Rate(mixed,120);Check(mixed.mButteredCounter==196,"remaining163/100 seconds rounds up at120");
  for(int i=0;i<1000;i++){Rate(mixed,100);Rate(mixed,120);}Check(Record(mixed)!.A==units&&mixed.mButteredCounter==196,"repeated rate switches do not accumulate rounding");
  for(int i=0;i<120;i++)Tick(mixed);Rate(mixed,100);Check(mixed.mButteredCounter==63,"mixed rate decay preserves exact remaining seconds");
  FusionNativeTimeRecords.Forget(mixed.mBoard);Rate(mixed,120);Check(mixed.mButteredCounter==76,"saved native backing reconstructs exact provenance");
  var native=New(100);native.mButteredCounter=400;native.mChilledCounter=1000;native.mIceTrapCounter=500;Rate(native,120);Check(native.mButteredCounter==400&&native.mChilledCounter==1000&&native.mIceTrapCounter==500,"unmarked legacy/native states remain untouched");
  var takeover=New(100);FusionStatuses.Butter(takeover);takeover.ApplyButter();Check(Record(takeover)==null,"native same-value overwrite clears provenance");Rate(takeover,120);Check(takeover.mButteredCounter==400,"later native owner is not rescaled");
  takeover=New(100);FusionStatuses.Freeze(takeover);takeover.ApplyChill(false);Rate(takeover,120);Check(takeover.mIceTrapCounter==480&&takeover.mChilledCounter==1400,"native chill relinquishes only chill provenance");
  var longer=New(100);longer.mButteredCounter=900;FusionStatuses.Butter(longer,200);Check(longer.mButteredCounter==900&&Record(longer)==null,"short Fusion application does not acquire longer unowned native effect");
  var immune=New(120);immune.FreezeImmune=immune.ColdImmune=true;FusionStatuses.Freeze(immune);FusionStatuses.Butter(immune);Check(immune.mIceTrapCounter==0&&immune.mChilledCounter==0&&immune.mButteredCounter==0&&Record(immune)==null,"native immunity owns acceptance");
  var failed=New(120);failed.mBoard.m_gridItems.FailAllocation=true;FusionStatuses.Butter(failed,200);Check(failed.mButteredCounter==400&&Record(failed)==null,"failed provenance allocation preserves native result");
  var foreign=New(100);FusionStatuses.Butter(foreign);foreign.mButteredCounter=250;Rate(foreign,120);Check(foreign.mButteredCounter==250&&Record(foreign)==null,"unknown direct overwrite relinquishes provenance");
  var paused=New(100);FusionStatuses.Butter(paused,200);paused.mBoard.mPaused=true;for(int i=0;i<10;i++)Tick(paused);Check(paused.mButteredCounter==200,"pause preserves status");
  paused.mBoard.mPaused=false;paused.DataID++;FusionStatuses.PrepareBoard(paused.mBoard);Check(Record(paused)==null,"full entity generation prevents pool transfer");
  var sentinel=New(100);sentinel.mChilledCounter=-1;Rate(sentinel,120);Check(sentinel.mChilledCounter==-1,"unmarked permanent sentinel remains intact");
  Console.WriteLine($"Native Fusion statuses: {checks} checks passed; actual clock and persisted records, no game.");
 }
}
