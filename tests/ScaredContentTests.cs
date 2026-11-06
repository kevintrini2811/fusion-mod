using FusionMod;
using Il2CppReloaded.Gameplay;

// Run manually. These offline doubles never load game code or Unity.
static class ScaredContentTests
{
    private static void Check(bool condition, string message)
    { if (!condition) throw new Exception(message); }
    private static Zombie Eyes(Board board) => new() { mBoard=board,mZombieType=ZombieType.Normal,
        mGraveX=ScaredContent.ZombieMarker,mBodyHealth=270,mBodyMaxHealth=270,mHasHead=true };

    static void Main()
    {
        var board=new Board();
        foreach (var native in new[] { ZombieType.Normal, ZombieType.TrafficCone, ZombieType.Pail })
        for (uint roll=0;roll<100;roll++)
        {
            var zombie=new Zombie{mBoard=board,mZombieType=native,mHelmType=HelmType.None,mHelmHealth=200,mHelmMaxHealth=200};
            board.RandomValues.Enqueue(roll);ScaredContent.Spawn(zombie);
            Check(ScaredContent.Eyes(zombie)==(roll==0),"Exactly one of 100 outcomes replaces each eligible base zombie.");
        }
        var manualZombie=new Zombie{mBoard=board,mZombieType=ZombieType.Normal,mHelmType=HelmType.None};
        int randomCalls=board.RandomCalls;
        ScaredContent.SuppressReplacement=true;
        try { ScaredContent.Spawn(manualZombie); }
        finally { ScaredContent.SuppressReplacement=false; }
        Check(!ScaredContent.Eyes(manualZombie)&&board.RandomCalls==randomCalls,"An explicit normal debug card never consumes a replacement roll.");
        BucketDrops.Clear();
        var target=Eyes(board);target.mPosX=500;
        var hit=ScaredContent.BeforeDamage(target,DamageFlags.None);
        target.mBodyHealth=0;target.mDead=true;
        board.RandomValues.Enqueue(0);board.RandomValues.Enqueue(1200);
        ScaredContent.AfterDamage(target,hit);
        Check(BucketDrops.Count==1,"Ordinary lethal hit with zero drop roll drops a head.");
        ScaredContent.AfterDamage(target,hit);ScaredContent.Drop(target);
        Check(BucketDrops.Count==1,"Repeated death callbacks cannot duplicate a drop.");
        Check(board.m_gridItems.Any(i=>!i.mDead&&BucketDrops.IsSavedHead(i)),"Head has native serialized storage.");
        BucketDrops.Clear();
        target=Eyes(board);hit=ScaredContent.BeforeDamage(target,DamageFlags.DoesntLeaveBody);
        target.mBodyHealth=0;target.mDead=true;ScaredContent.AfterDamage(target,hit);
        Check(BucketDrops.Count==0,"Instant death never drops a fusion head.");
        target=Eyes(board);hit=ScaredContent.BeforeDamage(target,DamageFlags.DoesntLeaveBody);
        target.mBodyHealth=200;ScaredContent.AfterDamage(target,hit);
        hit=ScaredContent.BeforeDamage(target,DamageFlags.None);target.mBodyHealth=0;target.mDead=true;
        board.RandomValues.Enqueue(0);board.RandomValues.Enqueue(1200);ScaredContent.AfterDamage(target,hit);
        Check(BucketDrops.Count==1,"Surviving an explosion does not blacklist a later ordinary death.");
        BucketDrops.Clear();
        target=Eyes(board);hit=ScaredContent.BeforeDamage(target,DamageFlags.None);
        target.mBodyHealth=80;ScaredContent.AfterDamage(target,hit);
        Check(!target.mHasHead&&BucketDrops.Count==0,"Decapitation at source threshold 90 defers head drop until death.");
        target.mZombiePhase=ZombiePhase.ZombieDying;target.mDead=true;
        board.RandomValues.Enqueue(0);board.RandomValues.Enqueue(1200);ScaredContent.ZombieTick(target);
        Check(BucketDrops.Count==1,"Native delayed death completes pending ordinary drop.");
        BucketDrops.Clear();
        target=Eyes(board);hit=ScaredContent.BeforeDamage(target,DamageFlags.None);
        target.DataID++;target.mBodyHealth=0;target.mDead=true;ScaredContent.AfterDamage(target,hit);
        Check(BucketDrops.Count==0,"Recycled zombie handle cannot inherit a hit.");
        Check(FusionRecipes.TryGet(FusionPlant.EyesZombieHead,SeedType.Sunflower,out var result)
            && result==FusionPlant.BloodSunflower,"Only ordinary Sunflower is the head recipe ingredient.");
        Check(!FusionRecipes.TryGet(FusionPlant.EyesZombieHead,FusionPlant.SilverSunflower,out _),"Sunflower variants cannot substitute.");
        target=Eyes(new Board());target.mIsEating=true;
        target.mGraveY=1|32|(12<<8);
        Check(!ScaredContent.BiteReady(target),"Faster bite cannot fire before its first event at tick 13.");
        target.mGraveY=1|32|(13<<8);
        Check(ScaredContent.BiteReady(target)&&!ScaredContent.BiteReady(target),"One bite per original animation event.");
        target.mGraveY=1|32|128|(13<<8);
        Check(!ScaredContent.BiteReady(target),"Rage transition cannot deal bite damage.");
        target=Eyes(new Board());target.mGraveY=1;
        ScaredContent.Contact(target);
        Check(target.mIsEating,"Contact starts eating before waiting for first animation event.");
        ScaredContent.ZombieTick(target);
        Check(!ScaredContent.BiteReady(target),"Contact cannot bypass first-bite delay.");
        target.mGraveY=1|32|(34<<8);
        Check(ScaredContent.BiteReady(target)&&!ScaredContent.BiteReady(target),"Second faster event fires once at tick 34.");
        target=Eyes(new Board());target.mGraveY=1;target.mVelX=.4f;target.mScaleZombie=1;
        Check(ScaredRageWalkingPatch.Prefix(target,out var speed),"Open eyes allow native walking.");
        Check(target.mVelX==.8f&&target.mScaleZombie==2,"Both native movement paths receive double speed.");
        ScaredRageWalkingPatch.Finalizer(target,speed);
        Check(target.mVelX==.4f&&target.mScaleZombie==1,"Walking restores velocity and saved body size.");
        target.mGraveY=0;
        Check(ScaredRageWalkingPatch.Prefix(target,out speed)&&speed==null&&target.mVelX==.4f,"Closed eyes retain native speed.");
        target.mGraveY=1|128;
        Check(!ScaredRageWalkingPatch.Prefix(target,out speed)&&speed==null,"Opening animation still blocks walking.");
        var proximityBoard=new Board();
        var proximityZombie=Eyes(proximityBoard);proximityZombie.mPosX=500;proximityZombie.mRow=1;
        var proximityPlant=new Plant{mBoard=proximityBoard,mRow=1,mX=379};
        proximityBoard.Plants[(4,1)]=proximityPlant;
        proximityBoard.m_projectiles.Add(new Projectile{mBoard=proximityBoard,mRow=1,mPosX=450,mProjectileType=ProjectileType.Pea});
        ScaredContent.ZombieTick(proximityZombie);
        Check(!ScaredContent.Awakened(proximityZombie),"A projectile and a plant beyond one tile do not wake the eyes zombie.");
        proximityPlant.mX=380;
        ScaredContent.ZombieTick(proximityZombie);
        Check(ScaredContent.Awakened(proximityZombie)&&ScaredContent.Raging(proximityZombie),"A live plant at exactly one tile starts the opening animation.");
        var bitePlant=new Plant{mPlantHealth=100};
        var bite=new ScaredPlantBitePatch.Hit(target,bitePlant);
        bitePlant.mPlantHealth-=4;bite.Apply();
        Check(bitePlant.mPlantHealth==50,"Native four-point hit becomes exactly fifty before defenses.");
        bite.Apply();Check(bitePlant.mPlantHealth==50,"Damage supplement applies only once.");
        bitePlant=new Plant{mPlantHealth=3};bite=new ScaredPlantBitePatch.Hit(target,bitePlant);
        bitePlant.mPlantHealth-=4;bite.Apply();
        Check(bitePlant.mPlantHealth==-47,"Native lethal health writer exposes full bite to death defenses.");
        bitePlant=new Plant();bite=new ScaredPlantBitePatch.Hit(target,bitePlant);bite.Apply();
        Check(bitePlant.mPlantHealth==300&&!bite.Applied,"Rejected native hit causes no supplemental damage.");
        board=new Board();var plant=new Plant{mBoard=board,mSeedType=SeedType.Sunflower,mImitaterType=SeedType.Gravebuster};
        ScaredContent.Initialize(plant);
        var victim=new Plant{mBoard=board,mPlantCol=1,DataID=101};board.Plants[(1,0)]=victim;
        for(int i=0;i<999;i++)ScaredContent.PlantTick(plant);
        Check(!victim.mDead,"Infection cannot complete early.");
        board.mPaused=true;ScaredContent.PlantTick(plant);Check(plant.mTargetY==999,"Pause preserves infection progress.");
        board.mPaused=false;board.FailCreation=true;ScaredContent.PlantTick(plant);
        Check(!victim.mDead,"Failed result creation preserves ingredient.");
        board.FailCreation=false;ScaredContent.PlantTick(plant);
        Check(victim.mDead&&FusionPlant.From(board.LastCreated!)==FusionPlant.BloodSunflower,"Infection produces original variant.");
        board=new Board();plant=new Plant{mBoard=board,mSeedType=SeedType.Sunflower,mImitaterType=SeedType.Gravebuster};ScaredContent.Initialize(plant);
        for(int i=0;i<2499;i++)ScaredContent.PlantTick(plant);
        Check(board.Suns==0,"Sun production does not fire early.");ScaredContent.PlantTick(plant);
        Check(board.Suns==1,"Sun production uses 25 seconds.");
        Console.WriteLine("Scared content checks passed.");
    }
}
