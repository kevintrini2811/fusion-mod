using FusionMod;
using Il2CppReloaded.Gameplay;

int checks = 0;
void Check(bool ok, string message) { ++checks; if (!ok) throw new Exception(message); }
Plant Twin() {
    var p = new Plant { mSeedType=SeedType.Marigold, mImitaterType=SeedType.Marigold };
    TwinMarigold.Initialize(p); return p;
}
void Tick(Plant p, int count) { for(int i=0;i<count;i++) TwinMarigold.Update(p); }
int NativeUnits(Coin c) => c.mType==CoinType.Silver?1:c.mType==CoinType.Gold?5:100;
int Collect(Coin c, bool fail=false) {
    TwinMarigoldCollectionScope.Prefix(c,out var previous);
    try {
        // Actual native order: Die, inlined denomination, UserService.AddCoins,
        // then board statistics. A GetCoinValue patch alone cannot pass this.
        TwinMarigold.Forget(c); c.mDead=true;
        if(fail) throw new InvalidOperationException("credit failure");
        int original=NativeUnits(c), amount=original;
        TwinMarigoldCreditPatch.Prefix(ref amount,out var delta);
        TwinMarigoldCreditPatch.Postfix(delta);
        c.mBoard.mCoinsCollected+=original;
        return amount;
    } finally { TwinMarigoldCollectionScope.Finalizer(previous); }
}

Check(FusionRecipes.TryGet(SeedType.Marigold,SeedType.Marigold,out var result)&&result==FusionPlant.TwinMarigold,"source recipe");
var p=Twin();
Check(FusionPlant.From(p)==result&&p.mSeedType==SeedType.Marigold,"saved identity");
Check(p.mPlantHealth==300&&p.mPlantMaxHealth==300,"source health");
Check(FusionPlant.HasImportedVisuals(result),"original artwork selected");
Check(!FusionRecipes.TryGet(result,SeedType.Marigold,out _),"no accidental third ingredient");
var distribution=Enumerable.Range(0,100).Select(TwinMarigold.Roll).GroupBy(t=>t).ToDictionary(g=>g.Key,g=>g.Count());
Check(distribution[CoinType.Gold]==20&&distribution[CoinType.Silver]==75&&distribution[CoinType.Diamond]==5,"exact source probabilities");
Check(TwinMarigold.Roll(19)==CoinType.Gold&&TwinMarigold.Roll(20)==CoinType.Silver&&TwinMarigold.Roll(94)==CoinType.Silver&&TwinMarigold.Roll(95)==CoinType.Diamond,"probability boundaries");
p.mBoard.Random.Enqueue(0); p.mBoard.Random.Enqueue(20); p.mBoard.Random.Enqueue(19);
Tick(p,1);
Check(p.mShootingCounter==100&&p.mLaunchCounter==1800&&p.mBoard.Coins.Count==0,"start brightening, minimum interval");
Tick(p,49);
Check(p.mBoard.Coins.Count==0,"no early coins");
Tick(p,1);
Check(p.mBoard.Coins.Count==2&&p.mBoard.Coins[0].mType==CoinType.Silver&&p.mBoard.Coins[1].mType==CoinType.Gold,"two independent rolls after half second");
Check(TwinMarigold.Brightness(p)==4,"peak glow");
Tick(p,50); Check(TwinMarigold.Brightness(p)==1,"glow resets");
Tick(p,1750); Check(p.mBoard.Coins.Count==4,"next pair after eighteen seconds");
var max=Twin(); max.mBoard.Random.Enqueue(400);Tick(max,1);
Check(max.mLaunchCounter==2200,"maximum interval");
foreach(var mode in new[]{"pause","carried","bungee","dead","squished","loading","garden","board-end","izombie"}) {
    var q=Twin();q.mLaunchCounter=10;q.mShootingCounter=51;
    if(mode=="pause")q.mBoard.mPaused=true;
    if(mode=="carried")q.OffGround=true;
    if(mode=="bungee")q.mOnBungeeState=PlantOnBungeeState.OnBungee;
    if(mode=="dead")q.mDead=true;
    if(mode=="squished")q.mSquished=true;
    if(mode=="loading")q.mApp.GameScene=GameScenes.Loading;
    if(mode=="garden")q.mApp.GameMode=GameMode.ChallengeZenGarden;
    if(mode=="board-end")ChomperFusions.DisposingBoard=q.mBoard.Pointer;
    if(mode=="izombie")q.mApp.GameMode=GameMode.PuzzleIZombie1;
    Tick(q,1); Check(q.mLaunchCounter==10&&q.mShootingCounter==51&&q.mBoard.Coins.Count==0,"inactive "+mode);
    ChomperFusions.DisposingBoard=IntPtr.Zero;
}
var normal=new Plant{mSeedType=SeedType.Marigold,mLaunchCounter=123,mShootingCounter=7};
TwinMarigold.Initialize(normal);Tick(normal,100);
Check(normal.mLaunchCounter==123&&normal.mShootingCounter==7&&normal.mBoard.Coins.Count==0,"normal Marigold unchanged");
var restored=Twin();restored.mLaunchCounter=600;restored.mShootingCounter=51;Tick(restored,1);
Check(restored.mBoard.Coins.Count==2&&restored.mLaunchCounter==599&&restored.mShootingCounter==50,"saved pending pair resumes once");
Tick(restored,1);Check(restored.mBoard.Coins.Count==2,"no duplicate pair after restored event");

foreach(var (roll,units) in new[]{(20,10),(0,30),(95,100)}) {
    var q=Twin();q.mLaunchCounter=100;q.mShootingCounter=51;q.mBoard.Random.Enqueue(roll);Tick(q,1);
    var c=q.mBoard.Coins[0];
    Check(Collect(c)==units&&q.mBoard.mCoinsCollected==units,"source money and statistics "+roll);
    Check(TwinMarigold.Collecting==null&&TwinMarigold.BeginCollection(c)==null,"scope and pickup cleanup "+roll);
}
Check(Collect(new Coin{mType=CoinType.Gold,mBoard=new Board()})==5,"ordinary money unchanged");
var pool=Twin();pool.mLaunchCounter=100;pool.mShootingCounter=51;Tick(pool,1);
var reused=pool.mBoard.Coins[0];reused.DataID++;
Check(TwinMarigold.BeginCollection(reused)==null,"pool identity prevents stale money");
var foreign=pool.mBoard.Coins[1];foreign.mBoard=new Board{Pointer=(IntPtr)2};
Check(TwinMarigold.BeginCollection(foreign)==null,"board identity prevents stale money");
var disposed=Twin();disposed.mShootingCounter=51;disposed.mLaunchCounter=100;Tick(disposed,1);TwinMarigold.ForgetBoard(disposed.mBoard);
Check(TwinMarigold.BeginCollection(disposed.mBoard.Coins[0])==null,"board cleanup");
var error=Twin();error.mShootingCounter=51;error.mLaunchCounter=100;Tick(error,1);
var outer=new TwinMarigold.Collection{Board=error.mBoard,Units=10};TwinMarigold.Collecting=outer;
try { Collect(error.mBoard.Coins[0],true); } catch(InvalidOperationException) {}
Check(ReferenceEquals(TwinMarigold.Collecting,outer)&&!outer.Credited&&error.mBoard.mCoinsCollected==0,"exception restores nested collection");
TwinMarigold.Collecting=null;TwinMarigold.Clear();
Console.WriteLine($"Twin Marigold: {checks} offline assertions passed.");

var productionOnly = new Plant { mSeedType = FusionPlant.TwinMarigold.NativeType,
    mImitaterType = FusionPlant.TwinMarigold.Marker, mShootingCounter = 1 };
Check(!TwinMarigoldShootingPatch.Prefix(productionOnly)
    && !TwinMarigoldFirePatch.Prefix(productionOnly), "production countdown cannot enter Marigold projectile path");
Check(TwinMarigoldShootingPatch.Prefix(new Plant()) && TwinMarigoldFirePatch.Prefix(new Plant()),
    "ordinary shooter paths remain available");
