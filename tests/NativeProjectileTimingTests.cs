using FusionMod;
using Il2CppReloaded.Gameplay;
using Native=Il2CppReloaded.Constants.Gameplay;

int checks=0;
void Check(bool ok,string reason){++checks;if(!ok)throw new Exception(reason);}
void Near(double a,double b,string reason,double tolerance=.0001)=>Check(Math.Abs(a-b)<tolerance,reason+": "+a+" vs "+b);
void Rate(int rate)=>Native.TIME_FOR_UPDATE=1d/rate;
foreach(int rate in new[]{100,120}) {
 Rate(rate);var board=new Board();var shot=board.AddProjectile();shot.DataID=0x12340001;
 shot.mProjectileAge=250;shot.mVelX=4;shot.mVelY=-2;shot.mVelZ=-3;shot.mAccZ=.06f;
 FusionProjectileTime.Prepare(shot);
 Near(FusionProjectileTime.AgeSeconds(shot),2.5,"legacy marked age keeps elapsed source seconds");
 Near(shot.mVelX*rate,400,"source horizontal pixels per second");
 Near(shot.mAccZ*rate*rate,600,"source gravity pixels per second squared",.001);
 var r=FusionNativeTimeRecords.Get(board,FusionNativeTimeRecords.Kind.Projectile,shot.DataID)!;
 Check(r.Rate==rate,"native rate saved");
 FusionNativeTimeRecords.Forget(board);
 Check(ReferenceEquals(r,FusionNativeTimeRecords.Get(board,FusionNativeTimeRecords.Kind.Projectile,shot.DataID))==false,"saved record reconstructed independently of transient cache");
 int other=rate==100?120:100;Rate(other);FusionProjectileTime.Prepare(shot);
 Near(FusionProjectileTime.AgeSeconds(shot),2.5,"live rate switch preserves age");
 Near(shot.mVelX*other,400,"live rate switch preserves horizontal speed");
 Near(shot.mVelZ*other,-300,"live rate switch preserves vertical speed");
 Near(shot.mAccZ*other*other,600,"live rate switch preserves gravity",.002);
 Rate(rate);
 var ball=board.AddProjectile();ball.DataID=2;ball.mProjectileType=ProjectileType.Cabbage;
 LobberFlight.LaunchAt(ball,500,200,100);
 for(int i=0;i<rate;++i){ball.mVelZ+=ball.mAccZ;ball.mPosX+=ball.mVelX;ball.mPosY+=ball.mVelY;ball.mPosZ+=ball.mVelZ;}
 Near(ball.mPosX,500,"ballistic horizontal endpoint",.002);Near(ball.mPosY,200,"ballistic lane endpoint",.002);Near(ball.mPosZ,-35,"ballistic native collision plane",.002);
 var moving=board.AddProjectile();moving.DataID=3;var target=new Zombie{mBoard=board,mVelX=1};
 LobberFlight.Launch(moving,target,100);
 Near(moving.mVelX*rate,500-rate,"prediction uses actual native target displacement count",.002);
 var canonical=board.AddProjectile();canonical.DataID=4;canonical.CanonicalAge=true;canonical.mProjectileAge=250;canonical.mVelX=4;
 FusionProjectileTime.Prepare(canonical);Near(canonical.mVelX,4,"custom canonical motion never rebased");Near(FusionProjectileTime.AgeSeconds(canonical),2.5,"custom age uses source units");
 var stationaryFields=board.AddProjectile();stationaryFields.DataID=5;stationaryFields.Flags=FusionProjectileTime.Age;stationaryFields.mVelZ=123;stationaryFields.mAccZ=456;
 FusionProjectileTime.Prepare(stationaryFields);Near(stationaryFields.mVelZ,123,"orbit center Z field not velocity");Near(stationaryFields.mAccZ,456,"orbit center acceleration field not gravity");
 var fixedShot=board.AddProjectile();fixedShot.DataID=6;fixedShot.Flags=FusionProjectileTime.Age|FusionProjectileTime.FixedStep;
 FusionProjectileTime.BeforeNormalMotion(fixedShot);fixedShot.mPosX+=3.33f;
 Near(fixedShot.mPosX*rate,333,"literal native horizontal step corrected before original");
 var ordinary=board.AddProjectile();ordinary.DataID=7;ordinary.Owned=false;ordinary.mProjectileAge=17;ordinary.mVelX=4;
 FusionProjectileTime.Prepare(ordinary);Near(ordinary.mVelX,4,"unmarked native projectile untouched");Check(FusionNativeTimeRecords.Get(board,FusionNativeTimeRecords.Kind.Projectile,7)==null,"unmarked native does not acquire guessed origin");
 var crater=new GridItem{mBoard=board,mGridItemType=GridItemType.Crater};board.m_gridItems.Add(crater);
 FusionCraterTime.SetDuration(crater,18000);Check(crater.mGridItemCounter==rate*180,"Fusion crater lasts180 source seconds");
 crater.mGridItemCounter-=rate*30;Rate(other);FusionCraterTime.Prepare(crater);Check(crater.mGridItemCounter==other*150,"crater remaining time survives rate switch");
 var legacyCrater=new GridItem{mBoard=board,mGridItemType=GridItemType.Crater,mGridItemCounter=700};FusionCraterTime.Prepare(legacyCrater);Check(legacyCrater.mGridItemCounter==700,"unmarked legacy crater not attributed by guess");
 var demise=new GridItem{mBoard=board,mGridItemType=GridItemType.Crater,mGridItemCounter=1000,mSunCount=1000};FusionCraterTime.Prepare(demise);Check(demise.mGridItemCounter==1000,"Demise canonical crater remains canonical");
 board.m_gridItems.FailAllocation=true;var failed=new GridItem{mBoard=board,mGridItemType=GridItemType.Crater,mGridItemCounter=123};FusionCraterTime.SetDuration(failed,18000);Check(failed.mGridItemCounter==123,"allocation failure preserves previous crater state");
 board.m_gridItems.FailAllocation=false;shot.mDead=true;FusionProjectileTime.Cleanup(board);Check(FusionNativeTimeRecords.Get(board,FusionNativeTimeRecords.Kind.Projectile,shot.DataID)==null,"dead projectile provenance cleaned");
}
Rate(100);var stressBoard=new Board();var stress=stressBoard.AddProjectile();stress.DataID=987;stress.mProjectileAge=251;
FusionProjectileTime.Prepare(stress);
var stressCrater=new GridItem{mBoard=stressBoard,mGridItemType=GridItemType.Crater};stressBoard.m_gridItems.Add(stressCrater);FusionCraterTime.SetDuration(stressCrater,1);
for(int i=0;i<1000;++i){Rate(i%2==0?120:100);FusionProjectileTime.Prepare(stress);FusionCraterTime.Prepare(stressCrater);Near(FusionProjectileTime.AgeSeconds(stress),2.51,"age600 has no repeated-switch floor drift");}
Check(stressCrater.mGridItemCounter==1,"crater600 has no repeated-switch ceil drift");
Rate(120);FusionProjectileTime.Prepare(stress);int nativeAge=stress.mProjectileAge;
var read=FusionProjectileTime.BeginAgeRead(stress);Check(stress.mProjectileAge==251,"native motion source thresholds see canonical age");Near(FusionProjectileTime.AgeSeconds(stress),2.51,"source-age read scope preserves actual elapsed reader");FusionProjectileTime.EndAgeRead(read);Check(stress.mProjectileAge==nativeAge,"motion finalizer restores native age");
foreach(int rate in new[]{100,120}){
 Rate(rate);var b=new Board();var triple=b.AddProjectile();triple.DataID=102;triple.Flags=FusionProjectileTime.Age|FusionProjectileTime.Planar|FusionProjectileTime.FixedStep;triple.mMotionType=ProjectileMotion.Threepeater;triple.mVelY=3;
 FusionProjectileTime.Prepare(triple);
 for(int i=0;i<rate;++i){var scope=FusionProjectileTime.BeginThreepeater(triple);triple.mPosY+=triple.mVelY;triple.mVelY*=.97f;triple.mShadowY+=triple.mVelY;FusionProjectileTime.EndThreepeater(scope);}
 Near(triple.mPosY,3*(1-Math.Pow(.97,100))/.03,"native Threepeater trajectory matches source exponential damping",.003);
 Near(triple.mVelY*rate,300*Math.Pow(.97,100),"native Threepeater speed matches source elapsed time",.002);
}
Rate(120);var db=new Board();var dc=new GridItem{mBoard=db,mGridItemType=GridItemType.Crater,mSunCount=1000};db.m_gridItems.Add(dc);FusionCraterTime.SetDuration(dc,18000);dc.mSunCount=999;FusionCraterTime.Cleanup(db);Check(FusionNativeTimeRecords.Get(db,FusionNativeTimeRecords.Kind.Crater,dc.DataID)!=null,"Demise native crater provenance persists during independent freeze counter");Check(dc.mSunCount==999,"native lifetime never rescales Demise source freeze counter");dc.mDead=true;FusionCraterTime.Cleanup(db);Check(FusionNativeTimeRecords.Get(db,FusionNativeTimeRecords.Kind.Crater,dc.DataID)==null,"expired crater provenance removed");
Rate(100);var hb=new Board();var hp=hb.AddProjectile();hp.DataID=111;hp.Flags=FusionProjectileTime.Age;hp.mProjectileAge=251;hp.mVelX=4.8f;FusionProjectileTime.Prepare(hp);Rate(120);FusionProjectileTime.Prepare(hp);FusionProjectileTime.SourceMotionHandoff(hp,FusionProjectileTime.Age|FusionProjectileTime.Planar);Near(hp.mVelX,4,"custom potato velocity converts exactly once at native handoff");Near(FusionProjectileTime.AgeSeconds(hp),2.51,"native handoff preserves existing exact age");hp.Flags=FusionProjectileTime.Age|FusionProjectileTime.Planar;FusionProjectileTime.Prepare(hp);Near(hp.mVelX,4,"subsequent native prepare does not convert handoff again");
LobberFlight.LaunchAt(hp,500,100,100);Near(FusionProjectileTime.AgeSeconds(hp),2.51,"native ballistic relaunch retains exact sub-native age remainder");
var paused=hb.AddProjectile();paused.DataID=112;paused.mVelX=6;hb.mPaused=true;FusionProjectileTime.Prepare(paused);Near(paused.mVelX,6,"paused board does not prepare motion");hb.mPaused=false;
var nested=FusionProjectileTime.BeginAgeRead(hp);int canonicalView=hp.mProjectileAge;var inner=FusionProjectileTime.BeginAgeRead(hp);FusionProjectileTime.EndAgeRead(inner);Check(hp.mProjectileAge==canonicalView,"nested native age reader restores parent view");FusionProjectileTime.EndAgeRead(nested);Near(FusionProjectileTime.AgeSeconds(hp),2.51,"nested read leaves exact age unchanged");
var attach=hb.AddProjectile();attach.DataID=113;attach.Flags=FusionProjectileTime.Age|FusionProjectileTime.Planar;attach.mProjectileAge=1;attach.mMotionType=ProjectileMotion.Threepeater;attach.mVelY=3;FusionProjectileTime.Prepare(attach);
var ageAttach=FusionProjectileTime.BeginAgeRead(attach);var threeAttach=FusionProjectileTime.BeginThreepeater(attach);attach.CanonicalAge=true;attach.mProjectileAge=0;attach.mMotionType=ProjectileMotion.Straight;attach.mVelY=0;FusionProjectileTime.EndThreepeater(threeAttach);FusionProjectileTime.EndAgeRead(ageAttach);Check(attach.mProjectileAge==0&&attach.mVelY==0,"same-ID native impact attachment keeps new canonical age and velocity even when projected old age was zero");
var loadedBoard=new Board();var loaded=loadedBoard.AddProjectile();loaded.DataID=hp.DataID;loaded.Flags=hp.Flags;loaded.mProjectileAge=hp.mProjectileAge;loaded.mVelX=hp.mVelX;loaded.mVelY=hp.mVelY;loaded.mVelZ=hp.mVelZ;loaded.mAccZ=hp.mAccZ;
var savedRecord=FusionNativeTimeRecords.Get(hb,FusionNativeTimeRecords.Kind.Projectile,hp.DataID)!;
var snapshot=hb.m_gridItems.Single(i=>!i.mDead&&unchecked((uint)i.mGridX)==hp.DataID&&i.mGridItemCounter==savedRecord.Rate);
loadedBoard.m_gridItems.Add(new GridItem{mBoard=loadedBoard,mGridItemType=snapshot.mGridItemType,mGridItemState=snapshot.mGridItemState,mSunCount=snapshot.mSunCount,mGridX=snapshot.mGridX,mGridY=snapshot.mGridY,mGridItemCounter=snapshot.mGridItemCounter,mPosX=snapshot.mPosX,mPosY=snapshot.mPosY,mGoalX=snapshot.mGoalX,mGoalY=snapshot.mGoalY});
Rate(100);FusionProjectileTime.Prepare(loaded);Near(FusionProjectileTime.AgeSeconds(loaded),2.51,"fresh board saved native scalars recover exact projectile age across rate change");
Rate(100);var tb=new Board();var ts=tb.AddProjectile();ts.DataID=114;ts.Flags=FusionProjectileTime.Age|FusionProjectileTime.Planar|FusionProjectileTime.FixedStep;ts.mMotionType=ProjectileMotion.Threepeater;ts.mVelY=3;
foreach(var segment in new[]{(Hz:100,Ticks:50),(Hz:120,Ticks:60)}){Rate(segment.Hz);for(int i=0;i<segment.Ticks;++i){FusionProjectileTime.Prepare(ts);var scope=FusionProjectileTime.BeginThreepeater(ts);ts.mPosY+=ts.mVelY;ts.mVelY*=.97f;FusionProjectileTime.EndThreepeater(scope);++ts.mProjectileAge;}FusionNativeTimeRecords.Forget(tb);}
Near(ts.mPosY,3*(1-Math.Pow(.97,100))/.03,"Threepeater mixed-rate trajectory survives source/native rate changes and provenance cache reconstruction",.003);Near(FusionProjectileTime.AgeSeconds(ts),1,"Threepeater mixed-rate age accumulates actual elapsed second");
Rate(100);Console.WriteLine($"Native projectile/crater timing: {checks} checks passed; actual clock and provenance compiled.");
