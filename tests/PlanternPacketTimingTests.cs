using FusionMod;
using Il2CppReloaded.Gameplay;
using Clock=Il2CppReloaded.Constants.Gameplay;
internal static class Program {
 static int checks;
 static void Check(bool value,string message){++checks;if(!value)throw new Exception(message);}
 // Pinned SeedPacket.Update0x6d6ac0: maincounter guard, inc at0x6d6c55,
 // strict >refreshTime completion at0x6d6c5a. Non-recharge work is independent.
 static void Native(SeedPacket p){if(p.LastCounter==p.mBoard.mMainCounter)return;p.LastCounter=p.mBoard.mMainCounter;++p.OtherUpdates;if(!p.mActive&&p.mRefreshing&&++p.mRefreshCounter>p.mRefreshTime){p.mRefreshCounter=0;p.mRefreshing=false;p.mActive=true;++p.ReadyEvents;}}
 static void Call(SeedPacket p){var scope=PlanternPacketClock.Before(p);try{Native(p);}finally{PlanternPacketClock.After(p,scope);}}
 static void Tick(SeedPacket p){FusionTime.BeginTick(p.mBoard);Call(p);int once=p.mRefreshCounter;Call(p);Check(p.mRefreshCounter==once,"duplicate native packet call never changes source progress");++p.mBoard.mMainCounter;}
 static void Advance(SeedPacket p,int rate,int ticks){Clock.TIME_FOR_UPDATE=1d/rate;for(int i=0;i<ticks;i++)Tick(p);}
 static SeedPacket New()=>new(){mBoard=new Board()};
 static void Main(){
  foreach(int rate in new[]{100,120})foreach(bool imitate in new[]{false,true}){
   var p=New();if(imitate){p.mPacketType=SeedType.Imitater;p.mImitaterType=SeedType.Plantern;}Advance(p,rate,15*rate);Check(p.mRefreshCounter==1500&&p.mRefreshing&&!p.mActive,"15 simulated seconds retain native strict completion threshold");Check(p.OtherUpdates==15*rate,"native non-recharge packet work runs every native step");
   for(int i=0;i<2&&!p.mActive;i++)Tick(p);Check(p.mActive&&p.ReadyEvents==1,"ready transition occurs exactly once on next canonical source step");
  }
  var legacy=New();legacy.mRefreshCounter=321;Advance(legacy,120,60);Check(legacy.mRefreshCounter==371&&legacy.mRefreshTime==1500,"published saved fields remain canonical without schema conversion");Advance(legacy,100,50);Check(legacy.mRefreshCounter==421,"live120to100 change preserves remaining progress");Advance(legacy,120,60);Check(legacy.mRefreshCounter==471,"live100to120 change preserves remaining progress");
  legacy.mBoard.mPaused=true;Advance(legacy,120,120);Check(legacy.mRefreshCounter==471,"pause does not advance owned recharge even if called");legacy.mBoard.mPaused=false;
  var native=New();native.mPacketType=SeedType.Sunflower;Advance(native,120,120);Check(native.mRefreshCounter==120,"unrelated native recharge remains native");
  var challenge=New();challenge.mRefreshTime=8000;Advance(challenge,120,120);Check(challenge.mRefreshCounter==120,"native mode-specific duration is not treated as authored1500");
  var interrupted=New();Clock.TIME_FOR_UPDATE=1d/120;FusionTime.BeginTick(interrupted.mBoard);var scope=PlanternPacketClock.Before(interrupted);Check(interrupted.mRefreshCounter==-1,"only private native increment boundary is temporarily offset");PlanternPacketClock.After(interrupted,scope);Check(interrupted.mRefreshCounter==0,"early-return or exception restores unpublished temporary value");
  scope=PlanternPacketClock.Before(interrupted);interrupted.mPacketType=SeedType.Sunflower;interrupted.mRefreshCounter=17;PlanternPacketClock.After(interrupted,scope);Check(interrupted.mRefreshCounter==17,"native identity/state change survives finalizer");
  var ready=New();ready.mActive=true;ready.mRefreshing=false;Check(PlanternPacketClock.Before(ready)==null,"ready packet has no custom recharge scope");
  var malformed=New();malformed.mRefreshCounter=-1;Check(PlanternPacketClock.Before(malformed)==null&&malformed.mRefreshCounter==-1,"negative native sentinel is untouched");
  Console.WriteLine($"Plantern packet time: {checks} checks passed; actual clock and own recharge boundary at100/120Hz.");
 }
}
