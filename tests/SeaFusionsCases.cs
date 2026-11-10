using FusionMod;
using Il2CppReloaded.Gameplay;

internal static class SeaFusionsCases
{
    private static void Check(bool value, string message)
    { if (!value) throw new Exception("Sea: " + message); }
    private static Plant New(Board board, FusionPlant identity)
    {
        var plant = new Plant { mBoard=board, mSeedType=identity.NativeType, mImitaterType=identity.Marker };
        board.m_plants.Add(plant); SeaFusions.Initialize(plant); return plant;
    }
    private static void Tick(Plant plant, int count)
    { for (int i=0;i<count;++i) SeaFusions.Tick(plant); }
    internal static void Run()
    {
        foreach (var (ingredient, result) in new[] {
            (SeedType.Puffshroom,FusionPlant.MinnowShroom), (SeedType.Cactus,FusionPlant.UrchinShroom),
            (SeedType.Fumeshroom,FusionPlant.SharkShroom), (SeedType.Starfruit,FusionPlant.SeaStar) })
        {
            Check(FusionRecipes.TryGet(SeedType.Seashroom,ingredient,out var forward)&&forward==result,"forward recipe");
            Check(FusionRecipes.TryGet(ingredient,SeedType.Seashroom,out var reverse)&&reverse==result,"reverse recipe");
            var p=New(new(),result);Check(FusionPlant.From(p)==result,"saved identity");
        }
        var board=new Board(); var plant=New(board,FusionPlant.MinnowShroom);
        SeaFusions.Fire(plant);Check(board.m_projectiles.Count==2,"initial colony fires twice");
        Tick(plant,1499);Check(plant.mShootingCounter==2,"no premature colony growth");
        board.mPaused=true;Tick(plant,100);Check(plant.mTargetX==1,"paused growth");board.mPaused=false;
        Tick(plant,1);Check(plant.mShootingCounter==3&&board.Cards==0,"first interval grows without a card");
        SeaFusions.Fire(plant);Check(board.m_projectiles.Count==5,"grown colony fires three times");
        Tick(plant,1500);Check(board.Cards==1,"later interval drops usable card");
        plant.mTargetX=400;SeaFusions.Fertilize(plant);Check(plant.mTargetX==50,"fertilizer accelerates reproduction");
        plant.mTargetX=17;SeaFusions.Fertilize(plant);Check(plant.mTargetX==17,"fertilizer preserves shorter timer");
        plant.mTargetX=0;SeaFusions.Fertilize(plant);Check(plant.mTargetX==0,"fertilizer never reactivates completed timer");

        board=new();plant=New(board,FusionPlant.UrchinShroom);Tick(plant,11999);
        Check(plant.mShootingCounter==0,"urchin needs 120 seconds");Tick(plant,1);
        Check(plant.mShootingCounter==1&&plant.mTargetY==100,"urchin growth clip");Tick(plant,100);
        var ground=new Zombie{mBoard=board,mRow=0,X=300};board.m_zombies.Add(ground);
        Tick(plant,110);Check(board.m_projectiles.Count==4,"grown urchin four original events");
        var shot=board.m_projectiles[0];Check(SeaProjectiles.Family(shot)==2,"urchin homing spore");
        SeaProjectiles.BeforeMove(shot);Check(shot.mTargetZombieID==(ZombieID)ground.DataID,"same-row homing target");
        ground.mRow=1;shot.mTargetZombieID=0;shot.mVelX=3;shot.mVelY=0;SeaProjectiles.BeforeMove(shot);
        Check(shot.mTargetZombieID==0,"no target in other lane");
        board=new();plant=New(board,FusionPlant.UrchinShroom);
        board.m_zombies.Add(new(){mBoard=board,Flying=true,mRow=0,X=300});
        Tick(plant,31);Check(board.m_projectiles.Count==1&&SeaProjectiles.Family(board.m_projectiles[0])==3,"air-only spike selection");
        Check(board.m_projectiles[0].mDamageRangeFlags==DamageRangeFlags.Flying,"spike cannot hit ground");
        shot=board.m_projectiles[0];shot.mCobTargetX=0;Check(SeaProjectiles.Family(shot)==0,"pool reuse clears family");

        board=new();plant=New(board,FusionPlant.SeaStar);Tick(plant,6000);
        Check(plant.mShootingCounter==1,"sea star matures after sixty seconds");Tick(plant,100);
        board.m_zombies.Add(new(){mBoard=board,mRow=0,X=0});Tick(plant,84);
        Check(board.m_projectiles.Count==10,"mature sea star emits two complete five-star volleys");
        Check(board.m_projectiles.All(p=>StarProjectiles.Family(p)==6),"original sea-star projectile family");
        var target=board.m_zombies[0];StarProjectiles.Impact(board.m_projectiles[0],target);
        Check(target.Damage==20,"sea star normal damage without unrelated status");

        board=new();plant=New(board,FusionPlant.SharkShroom);plant.mPlantHealth=123;
        Check(SeaFusions.CanFeed(plant,SeedType.Seashroom)&&!SeaFusions.CanFeed(plant,SeedType.Puffshroom),"feeding identity");
        SeaFusions.Feed(plant);Check(plant.mTargetX==1000&&plant.mPlantHealth==123,"feed grants range without healing");
        var far=new Zombie{mBoard=board,mRow=0,X=700};board.m_zombies.Add(far);
        Check(SeaFusions.Target(plant,false)==far,"fed range reaches far end");
        SeaFusions.Fire(plant);Check(far.Damage==20,"fed smoke deals piercing damage at range");
        SeaFusions.Fertilize(plant);Check(plant.mTargetX==50,"fertilizer caps current range buff");Tick(plant,50);
        Check(SeaFusions.Target(plant,false)==null,"range restored on expiry");
        plant.Carried=true;int timer=plant.mTargetY;Tick(plant,50);Check(timer==plant.mTargetY,"carrying pauses animation");
        Check(!SeaFusions.CanFeed(plant,SeedType.Seashroom),"cannot feed carried plant");

        board=new();
        var low = new[]{New(board,FusionPlant.MinnowShroom),New(board,FusionPlant.UrchinShroom),New(board,FusionPlant.SeaStar)};
        var tall = New(board,FusionPlant.SharkShroom);
        shot=new Projectile{mBoard=board,mProjectileType=ProjectileType.ZombiePea};
        SeaLowCollisionScope.Prefix(shot,out var mask);
        Check(low.All(p=>p.mSeedType==SeedType.Potatomine)&&tall.mSeedType==SeedType.Seashroom,"only low species evade straight shots");
        SeaLowCollisionScope.Finalizer(mask);
        Check(low.All(p=>p.mSeedType==SeedType.Seashroom),"restore saved identities after lookup");
        shot.mProjectileType=ProjectileType.Basketball;SeaLowCollisionScope.Prefix(shot,out mask);
        Check(mask==null,"descending basketballs still collide");

        board=new();plant=New(board,FusionPlant.MinnowShroom);
        var forwardPlant=New(board,FusionPlant.SharkShroom);forwardPlant.mPlantCol=1;
        SeaFusions.Reproduce(plant);Check(forwardPlant.mTargetX==1000&&board.Cards==0,"automatic forward feeding");
        forwardPlant.mSeedType=SeedType.Cactus;forwardPlant.mImitaterType=SeedType.None;
        board.FailAllocation=true;bool failed=false;
        try { SeaFusions.Reproduce(plant); } catch(InvalidOperationException) { failed=true; }
        Check(failed&&!forwardPlant.mDead&&PlayerPlantingScope.Current==null,"allocation failure retains forward ingredient and restores context");
    }
}
