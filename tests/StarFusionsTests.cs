using FusionMod;
using Il2CppReloaded.Gameplay;

static class Program
{
    static void Check(bool value,string message) { if(!value) throw new Exception(message); }
    static Plant Plant(Board board,FusionPlant type) {
        var p=new Plant { mBoard=board,mSeedType=type.NativeType,mImitaterType=type.Marker };
        board.m_plants.Add(p); StarFusions.Initialize(p); return p;
    }
    static Zombie Zombie(Board board) { var z=new Zombie {mBoard=board};board.m_zombies.Add(z);return z; }
    static Projectile Restore(Projectile old) {
        var copy=new Projectile();
        foreach(var field in typeof(Projectile).GetFields()) if(!field.IsStatic) field.SetValue(copy,field.GetValue(old));
        return copy;
    }
    // Native UpdateNormalMotion (0x6ca1d2 / 0x6ca1a6) removes Star
    // below 23px and Pea below 28px, even when Z itself is zero.
    static void NativeFlightTick(Projectile shot) {
        shot.mPosX += shot.mVelX;
        shot.mPosY += shot.mVelY;
        shot.mShadowY += shot.mVelY;
        if (shot.mShadowY - shot.mPosY < (shot.mProjectileType == ProjectileType.Star ? 23 : 28)) shot.Die();
    }
    static void Main(string[] args)
    {
        var ironBoard=new Board();var iron=Plant(ironBoard,FusionPlant.IronStar);
        Check(iron.mPlantMaxHealth==2000,"iron star toughness");
        StarFusions.Fire(iron);Check(ironBoard.m_projectiles.Count==5,"five iron stars");
        foreach(var flying in ironBoard.m_projectiles) {
            float startX=flying.mPosX,startY=flying.mPosY;
            NativeFlightTick(flying);
            Check(!flying.mDead&&(flying.mPosX!=startX||flying.mPosY!=startY),"iron stars survive native first flight tick");
        }
        var ironTarget=Zombie(ironBoard);ironTarget.mShieldHealth=500;
        var ironShot=Restore(ironBoard.m_projectiles[0]);
        Check(StarProjectiles.Family(ironShot)==8,"saved iron star identity");
        iron.mDead=true;StarProjectiles.Impact(ironShot,ironTarget);
        Check(ironTarget.Damage==580,"iron star adds full shield before damage, survives owner death");
        StarProjectiles.Impact(ironShot,ironTarget);Check(ironTarget.Damage==580,"iron star impacts once");
        var morphBoard=new Board();var morph=Plant(morphBoard,FusionPlant.Starmorph);
        StarFusions.Fire(morph);Check(morphBoard.m_projectiles.Count==5,"five metal stars");
        var armored=Zombie(morphBoard);armored.mHelmType=HelmType.Pail;armored.mHelmHealth=100;
        var metalShot=Restore(morphBoard.m_projectiles[0]);
        Check(StarProjectiles.Family(metalShot)==7,"saved metal star identity");
        StarProjectiles.Impact(metalShot,armored);Check(armored.Damage==80,"metal star equipment bonus");
        var driver=Zombie(morphBoard);driver.mZombieType=ZombieType.Zamboni;
        StarProjectiles.Impact(morphBoard.m_projectiles[1],driver);Check(driver.Damage==40,"metal body alone gives no star bonus");
        var dormant=Plant(morphBoard,FusionPlant.DormantShroom);dormant.mDoSpecialCountdown=100;
        DormantShroomInitializePatch.Postfix(dormant);
        Check(dormant.mDoSpecialCountdown==0&&!DormantShroomAbilityPatch.Prefix(dormant)
            &&!DormantShroomSpecialPatch.Prefix(dormant),"dormant never schedules or executes explosion");
        Check(DormantShroomSpecialPatch.Prefix(Plant(morphBoard,SeedType.Doomshroom)),"native doom stays native");
        foreach(var pair in new[]{(SeedType.Doomshroom,FusionPlant.DormantShroom),(SeedType.Fumeshroom,FusionPlant.MorphShroom),
            (SeedType.Melonpult,FusionPlant.MelonMorph),(SeedType.Starfruit,FusionPlant.Starmorph)}) {
            Check(FusionRecipes.TryGet(pair.Item1,SeedType.Magnetshroom,out var forward)&&forward==pair.Item2,"metal recipe");
            Check(FusionRecipes.TryGet(SeedType.Magnetshroom,pair.Item1,out var reverse)&&reverse==pair.Item2,"reverse metal recipe");
        }
        CactusCases.Run(); CactusNewCases.Run();
        if (args.Contains("--cactus-only")) { Console.WriteLine("Cactus regression cases passed."); return; }
        SeaFusionsCases.Run();
        SeaSupportCases.Run();
        var recipes=new[]{(SeedType.Garlic,FusionPlant.Starlic),(SeedType.Iceshroom,FusionPlant.WinterStarfruit),
            (SeedType.Jalapeno,FusionPlant.SpicyStarfruit),(SeedType.Peashooter,FusionPlant.Starshooter)};
        foreach(var (ingredient,result) in recipes) {
            Check(FusionRecipes.TryGet(SeedType.Starfruit,ingredient,out var a)&&a==result,"forward recipe");
            Check(FusionRecipes.TryGet(ingredient,SeedType.Starfruit,out var b)&&b==result,"reverse recipe");
        }
        var board=new Board();var plant=Plant(board,FusionPlant.Starlic);var zombie=Zombie(board);
        for(int i=0;i<37;++i) StarFusions.Tick(plant);
        Check(board.m_projectiles.Count==0,"no shot before original event");
        board.mPaused=true;int phase=plant.mStartRow;StarFusions.Tick(plant);
        Check(plant.mStartRow==phase,"pause preserves phase");board.mPaused=false;StarFusions.Tick(plant);
        Check(board.m_projectiles.Count==5,"five stars at original event");
        var shot=board.m_projectiles[0];zombie.Poison=3;StarProjectiles.Impact(shot,zombie);
        Check(zombie.Damage==45&&zombie.Poison==4,"poison bonus precedes new point");
        StarProjectiles.Impact(shot,zombie);Check(zombie.Damage==45,"dead shot cannot hit twice");
        board.ReserveFailure=true;shot=StarProjectiles.Spawn(board,0,0,0,0,1,0,4,30)!;
        StarProjectiles.Impact(shot,zombie);Check(zombie.Poison==4,"no unsaved status on allocation failure");board.ReserveFailure=false;

        board=new Board();plant=Plant(board,FusionPlant.WinterStarfruit);StarFusions.Fire(plant);
        Check(board.m_projectiles.Count==5,"five spiral arms");shot=Restore(board.m_projectiles[0]);shot.mProjectileAge=25;
        StarProjectiles.BeforeMove(shot);
        Check(Math.Abs(shot.mVelX)<.001f&&Math.Abs(shot.mVelY+25*.01f*2*80/1.35f)<.001f,"saved quarter-turn spiral position");
        plant.DataID+=100;StarProjectiles.BeforeMove(shot);Check(shot.mVelX==0&&shot.mVelY==0,"recycled source cannot resume spiral");
        zombie=Zombie(board);zombie.Frost=90;shot=StarProjectiles.Spawn(board,0,0,0,0,2,0,0,20)!;shot.Bonus=5;
        StarProjectiles.Impact(shot,zombie);Check(zombie.Damage==100&&zombie.Frost==100&&zombie.Chilled,"threshold hit multiplies complete damage");
        zombie=Zombie(board);zombie.Immune=true;shot=StarProjectiles.Spawn(board,0,0,0,0,2,0,0,20)!;
        StarProjectiles.Impact(shot,zombie);Check(zombie.Damage==20,"immunity alone is not frozen");

        board=new Board();plant=Plant(board,FusionPlant.Starshooter);StarFusions.Fire(plant);shot=board.m_projectiles[0];
        shot.mProjectileAge=19;StarProjectiles.BeforeMove(shot);Check(board.m_projectiles.Count==1,"no early split");
        for(int wave=0;wave<5;++wave) {
            shot=Restore(shot);shot.mProjectileAge=20+50*wave;StarProjectiles.BeforeMove(shot);
            Check(shot.mHitTorchwoodGridX==wave+1,"saved wave count resumes exactly once");
            StarProjectiles.BeforeMove(shot);Check(board.m_projectiles.Count==1+5*(wave+1),"one emission per threshold");
        }
        shot.mProjectileAge=500;StarProjectiles.BeforeMove(shot);Check(board.m_projectiles.Count==26,"five emissions maximum");
        var child=board.m_projectiles[1];child.mProjectileAge=500;StarProjectiles.BeforeMove(child);Check(board.m_projectiles.Count==26,"children do not emit");
        shot.mDead=true;StarProjectiles.BeforeMove(shot);Check(board.m_projectiles.Count==26,"dead parent does not emit");
        board.FailAllocation=true;Check(StarProjectiles.Spawn(board,0,0,0,0,4,0,4,20)==null,"failed projectile allocation is safe");

        board=new Board();plant=Plant(board,FusionPlant.SpicyStarfruit);
        board.mIceTimer[0]=600;
        for(int i=0;i<99;++i) StarFusions.Tick(plant);Check(!plant.mDead,"fuse lasts one second");
        board.mPaused=true;StarFusions.Tick(plant);Check(!plant.mDead,"paused fuse");board.mPaused=false;
        StarFusions.Tick(plant);Check(plant.mDead,"fuse expires");
        StarFusions.FinishSpicyDeath(plant, new(board,FusionPlant.SpicyStarfruit,plant.DataID,0,0,40,40,0));
        Check(board.RowBlasts==1&&board.Fwooshes==1&&board.mIceTimer[0]==20&&board.m_projectiles.Count==25,"one visible row fire, melting ice and 25 stars");
        Check(board.m_projectiles.All(p=>StarProjectiles.Family(p)==3&&p.mCobTargetRow==100),"fire stars retain source damage");
        foreach(var flying in board.m_projectiles) NativeFlightTick(flying);
        Check(board.m_projectiles.All(p=>!p.mDead),"all 25 death stars survive native ground-height gate");
        plant=Plant(board,FusionPlant.Starlic);zombie=Zombie(board);StarFusions.Bite(plant,zombie);
        Check(plant.mPlantHealth==285&&zombie.mYuckyFace&&zombie.mYuckyFaceCounter==70,"garlic bite cost and delay");
        // Sparkwood replaces elemental properties, retains saved damage and
        // direction, and applies the normal one-third splash after a hit.
        board = new Board();
        var incoming = StarProjectiles.Spawn(board,200,40,0,0,1,30,4,30)!;
        float vx=incoming.mVelX,vy=incoming.mVelY;
        incoming.mVelZ=200;incoming.mAccZ=100;
        StarProjectiles.Ignite(incoming,3);
        Check(StarProjectiles.Family(incoming)==9&&incoming.mCobTargetRow==50,"ignition adds 20 and removes original star traits");
        Check(incoming.mVelX==vx&&incoming.mVelY==vy&&incoming.mVelZ==0&&incoming.mAccZ==0&&incoming.mHitTorchwoodGridX==3,"ignition preserves motion and crossing ownership");
        var restored=Restore(incoming);
        Check(StarProjectiles.Family(restored)==9&&restored.mCobTargetRow==50,"ignited stars reconstruct from saved fields");
        var victim=Zombie(board);var neighbor=Zombie(board);neighbor.X=240;
        StarProjectiles.Impact(incoming,victim);
        Check(victim.Damage==50&&neighbor.Damage==16&&victim.Poison==0,"fire damage and splash replace poison");
        if (!args.Contains("--stars-only")) PumpkinCases.Run();
        Console.WriteLine("Star fusion contracts passed.");
    }
}
