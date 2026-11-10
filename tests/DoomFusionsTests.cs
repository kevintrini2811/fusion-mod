using FusionMod;
using Il2CppReloaded.Gameplay;

static class DoomFusionsTests
{
    static void Check(bool result, string message) { if (!result) throw new Exception(message); }
    static Plant Plant(Board b,FusionPlant type,int col=3,int row=2) =>
        b.AddPlant(col,row,type.NativeType,type.Marker)!;
    static Zombie Zombie(Board b,float x=240,float y=200)
    { var z=new Zombie {mBoard=b,X=x,Y=y,mRow=2}; b.m_zombies.Add(z); return z; }
    static void Main()
    {
        Recipes(); Growth(); Fragments(); Ember(); DeathSafety();
        Console.WriteLine("Doom fusion cases passed.");
    }
    static void Recipes()
    {
        foreach (var (ingredient,result) in new[] { (SeedType.Garlic,FusionPlant.DoomGarlic),
            (SeedType.Wallnut,FusionPlant.DoomNut),(SeedType.Sunflower,FusionPlant.Doomflower),
            (SeedType.Potatomine,FusionPlant.CraterTot) })
        {
            Check(FusionRecipes.TryGet(ingredient,SeedType.Doomshroom,out var a) && a==result,"forward recipe");
            Check(FusionRecipes.TryGet(SeedType.Doomshroom,ingredient,out var z) && z==result,"reverse recipe");
        }
    }
    static void Growth()
    {
        var b=new Board(); var flower=Plant(b,FusionPlant.Doomflower);
        flower.mLaunchCounter=1; b.mPaused=true; DoomFusions.Tick(flower);
        Check(b.Sun==0 && flower.mLaunchCounter==1,"paused production");
        b.mPaused=false; DoomFusions.Tick(flower);
        var spore=b.m_plants.Single(p=>FusionPlant.From(p)==FusionPlant.DoomSeed);
        Check(b.Sun==25 && flower.mLaunchCounter==5000 && spore.mLaunchCounter==3000,"production and one growing spore");
        var target=Zombie(b,spore.mX+10,spore.mY+10);
        spore.mLaunchCounter=100; DoomFusions.Tick(spore); Check(!spore.mDead,"immature spore");
        DoomFusions.Fertilize(spore); Check(spore.mLaunchCounter==50,"fertilizer caps spore timer");
        spore.mLaunchCounter=1; DoomFusions.Tick(spore);
        Check(spore.mDead && target.Received==1800,"mature spore detonates");
        DoomFusions.Tick(spore);Check(target.Received==1800,"no replay");
        b.FailAllocation=true;flower.mLaunchCounter=1;DoomFusions.Tick(flower);
        Check(!flower.mDead,"failed spore allocation retains parent");
    }
    static void Fragments()
    {
        var b=new Board();var mine=Plant(b,FusionPlant.CraterTot);
        DoomFusions.Fertilize(mine);Check(mine.mState==PlantState.PotatoArmed,"fertilizer arms Crater-tot");
        DoomFusions.Detonate(mine);
        Check(b.Shots.Count==5 && b.Shots.Select(s=>(s.mCobTargetRow,s.mRow)).Distinct().Count()==5,"five different targets");
        var occupied=b.Shots[0];var occupant=Plant(b,(FusionPlant)SeedType.Sunflower,occupied.mCobTargetRow,occupied.mRow);
        var context=new object();PlayerPlantingScope.Current=context;
        foreach(var shot in b.Shots)
        {
            b.mPaused=true;float ticks=shot.mAccZ;PotatoSeedProjectiles.Move(shot);
            Check(shot.mAccZ==ticks,"paused flight");b.mPaused=false;
            for(int i=0;i<150;i++) PotatoSeedProjectiles.Move(shot);
        }
        Check(!occupant.mDead,"landing preserves new occupant");
        Check(b.m_plants.Count(p=>!p.mDead && p.mSeedType==SeedType.Potatomine)==4,"only four free landings");
        Check(b.m_plants.Where(p=>!p.mDead && p.mSeedType==SeedType.Potatomine).All(p=>p.mStateCountdown==50),"source landing fuse");
        Check(ReferenceEquals(context,PlayerPlantingScope.Current),"spawn scope restored");
        PlayerPlantingScope.Current=null;
    }
    static void Ember()
    {
        var b=new Board();var nut=Plant(b,FusionPlant.DoomNut);nut.mPlantHealth=3000;
        var z=Zombie(b);Check(EmberEffects.Apply(z),"ember allocation");
        foreach(var item in b.m_gridItems) item.mBoard=b;
        EmberEffects.Apply(z);Check(b.m_gridItems.Count==1,"repeated bites do not stack");
        var survivor=Zombie(b,260,200);survivor.Category=DamageRangeFlags.Flying;
        var death=EmberEffects.Capture(z);z.Dying=true;EmberEffects.Finish(death);
        Check(nut.mPlantHealth==3100 && survivor.Received==300,"delayed death heals nut and hits air");
        EmberEffects.Finish(death);Check(nut.mPlantHealth==3100 && survivor.Received==300,"nested death consumes marker once");
        var recycled=Zombie(b);EmberEffects.Apply(recycled);foreach(var item in b.m_gridItems)item.mBoard=b;
        var saved=b.m_gridItems.Last();recycled.DataID+=100;EmberEffects.Tick(saved);
        Check(saved.mDead,"pooled zombie cannot inherit marker");
    }
    static void DeathSafety()
    {
        var b=new Board();var nut=Plant(b,FusionPlant.DoomNut);var z=Zombie(b);
        DoomEffects.LeaveCrater(b,3,2);Check(!nut.mDead,"Doom-nut survives nuclear crater damage");
        Check(EmberEffects.Apply(z),"marker");foreach(var item in b.m_gridItems)item.mBoard=b;
        CherryBombPlants.DisposingBoard=b.Pointer;var death=EmberEffects.Capture(z);
        Check(death==null,"teardown has no explosion");CherryBombPlants.DisposingBoard=IntPtr.Zero;
    }
}
