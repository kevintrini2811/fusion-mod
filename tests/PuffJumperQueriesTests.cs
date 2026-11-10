using FusionMod;
using Il2CppReloaded.Gameplay;

static class Program
{
    static int checks;
    static void Check(bool value,string message) { ++checks; if(!value) throw new Exception(message); }
    static Plant Add(Board b,FusionPlant identity,int phase=Squish.Ground,bool onBoard=true)
    {
        var p=new Plant {mBoard=b,Identity=identity,mShootingCounter=8,mStartRow=phase,mIsOnBoard=onBoard,mPlantHealth=300};
        b.m_plants.Add(p);return p;
    }
    // Model the native candidate loop: rejection happens before selection, so
    // the next genuine occupant remains selectable rather than returning null.
    static Plant? NativeSelect(Board b)
    {
        for(int i=0;i<b.m_plants.Count;++i)
            if(!b.m_plants[i].mDead&&!b.m_plants[i].NotOnGround()) return b.m_plants[i];
        return null;
    }
    static void Selection()
    {
        var b=new Board();var air=Add(b,FusionPlant.Squish,Squish.Travel,false);
        var nibble=Add(b,FusionPlant.Nibble,Squish.Ground,false);
        var landed=Add(b,FusionPlant.Squish,Squish.Ground,false);
        var plain=Add(b,FusionPlant.Other);
        var z=new Zombie {mBoard=b};
        Check(NativeSelect(b)==air,"native unpatched Puff query observes airborne identity normally");
        PuffJumperSelectorScope.Prefix(z,out var scope);
        Check(air.mSquished&&!air.mIsOnBoard,"selector hides only airborne Squish");
        Check(NativeSelect(b)==nibble,"native fallback still selects detached Nibble");
        Check(!landed.mSquished&&!plain.mSquished,"landed Squish and plain Puff remain candidates");
        PuffJumperSelectorScope.Prefix(z,out var nested);
        PuffJumperSelectorScope.Finalizer(nested);
        Check(air.mSquished,"nested selector does not prematurely restore outer candidate");
        PuffJumperSelectorScope.Finalizer(scope);
        Check(!air.mSquished&&!air.mIsOnBoard,"normal return restores original detached flags");
        b.m_plants.Remove(nibble);
        PuffJumperSelectorScope.Prefix(z,out scope);
        Check(NativeSelect(b)==landed,"native fallback selects vulnerable landed Squish");
        try { throw new InvalidOperationException(); }
        catch(InvalidOperationException) { }
        finally { PuffJumperSelectorScope.Finalizer(scope); }
        Check(!air.mSquished,"exception finalizer restores flight candidate");
        var foreign=Add(new Board(),FusionPlant.Squish,Squish.Fall,false);b.m_plants.Add(foreign);
        using(var hide=PuffJumperQueries.Air(b)) Check(!foreign.mSquished,"foreign board never hidden");
    }
    static void Lifetime()
    {
        foreach(var mutation in new Action<Plant>[] {
            p=>p.DataID+=0x10000,p=>p.Pointer=(IntPtr)999,p=>p.mBoard=new Board(),
            p=>p.Identity=FusionPlant.Other,p=>p.mDead=true,p=>p.mPlantHealth=0,p=>p.mBoard.Pointer=(IntPtr)999 })
        {
            var b=new Board();var p=Add(b,FusionPlant.Squish,Squish.Hover);
            var hide=PuffJumperQueries.Air(b);mutation(p);hide.Dispose();hide.Dispose();
            Check(p.mSquished&&!p.mIsOnBoard,"expired generation/identity/board/death never restored");
        }
        var board=new Board();var plant=Add(board,FusionPlant.Other);
        using(var outer=new PlantQueryHide(board,new[]{plant,plant}))
        {
            using(var inner=new PlantQueryHide(board,new[]{plant})) Check(plant.mSquished,"nested explicit snapshot retains hide");
            Check(plant.mSquished&&!plant.mIsOnBoard,"inner restores outer flags exactly");
        }
        Check(!plant.mSquished&&plant.mIsOnBoard,"outer restores original flags once");
    }
    static void Placement()
    {
        var b=new Board();var nibble=Add(b,FusionPlant.Nibble,onBoard:false);
        var landed=Add(b,FusionPlant.Squish,onBoard:false);
        var occupant=Add(b,FusionPlant.Other);var support=Add(b,FusionPlant.Other);
        var neighbor=Add(b,FusionPlant.Nibble,onBoard:false);neighbor.mPlantCol++;
        var foreign=Add(new Board(),FusionPlant.Nibble,onBoard:false);b.m_plants.Add(foreign);
        PuffJumperPlacementScope.Prefix(b,3,2,out var scope);
        Check(nibble.mSquished&&landed.mSquished,"home placement releases detached retained pair");
        Check(!occupant.mSquished&&!support.mSquished&&!neighbor.mSquished&&!foreign.mSquished,
            "home placement preserves occupants supports neighbors and foreign boards");
        PuffJumperPlacementScope.Prefix(b,3,2,out var inner);PuffJumperPlacementScope.Finalizer(inner);
        Check(nibble.mSquished,"nested placement preserves outer hide");
        PuffJumperPlacementScope.Finalizer(scope);
        Check(!nibble.mSquished&&!nibble.mIsOnBoard&&!landed.mSquished,"placement exception restores retained state");
    }
    static void Bungee()
    {
        var b=new Board();var air=Add(b,FusionPlant.Squish,Squish.Travel,false);var native=Add(b,FusionPlant.Other);
        var z=new Zombie {mBoard=b,mTargetCol=3,mRow=2};
        void Query(int col,int row,PlantPriority priority,Board? board=null,bool run=true,bool expected=false)
        {
            PuffJumperBungeeQuery.Prefix(board??b,col,row,priority,run,out var state);
            Check(air.mSquished==expected,"only exact Bungee home query can hide air");
            if(expected) Check(NativeSelect(b)==native,"Bungee native fallback selects genuine ground occupant");
            PuffJumperBungeeQuery.Finalizer(state);
            Check(!air.mSquished,"Bungee query restored before gameplay resumes");
        }
        Query(3,2,PlantPriority.BungeeOrder);
        PuffJumperBungeeScope.Prefix(z,out var previous);
        Check(!air.mSquished,"Bungee owner marker does not hide plants across gameplay");
        Query(3,2,PlantPriority.BungeeOrder,expected:true);Query(4,2,PlantPriority.BungeeOrder);
        Query(3,1,PlantPriority.BungeeOrder);Query(3,2,PlantPriority.Normal);
        Query(3,2,PlantPriority.BungeeOrder,new Board());Query(3,2,PlantPriority.BungeeOrder,run:false);
        PuffJumperBungeeQuery.Prefix(b,3,2,PlantPriority.BungeeOrder,true,out var outer);
        PuffJumperBungeeQuery.Prefix(b,3,2,PlantPriority.BungeeOrder,true,out var inner);
        PuffJumperBungeeQuery.Finalizer(inner);Check(air.mSquished,"nested Bungee query cannot restore outer hide");
        PuffJumperBungeeQuery.Finalizer(outer);Check(!air.mSquished,"Bungee exception restores hide and depth");
        z.DataID+=0x10000;Query(3,2,PlantPriority.BungeeOrder);
        PuffJumperBungeeScope.Finalizer(previous);Check(PuffJumperBungeeScope.Current==null,"Bungee marker restored");
    }
    static void Boss()
    {
        var b=new Board();var air=Add(b,FusionPlant.Squish,Squish.Hover,false);
        var z=new Zombie {mBoard=b};var foreign=new Zombie {mBoard=b};
        PuffJumperBossColdCheckpoint.Postfix(z,true);Check(!air.mSquished,"cold outside Boss marker inert");
        PuffJumperBossScope.Prefix(z,out var previous);
        Check(!air.mSquished,"Boss entry marker leaves callback phase untouched");
        PuffJumperBossColdCheckpoint.Postfix(foreign,true);PuffJumperBossColdCheckpoint.Postfix(z,false);
        Check(!air.mSquished,"foreign owner or skipped cold cannot arm Boss selection");
        PuffJumperBossColdCheckpoint.Postfix(z,true);Check(air.mSquished,"Boss selection armed only after cold callback");
        PuffJumperBossColdCheckpoint.Postfix(z,true);
        PuffJumperBossAnimationCheckpoint.Prefix(foreign);Check(air.mSquished,"foreign animation cannot restore Boss query");
        PuffJumperBossAnimationCheckpoint.Prefix(z);Check(!air.mSquished,"Boss flags restored before animation callbacks");
        PuffJumperBossColdCheckpoint.Postfix(z,true);Check(!air.mSquished,"finished Boss context cannot rearm");
        PuffJumperBossScope.Finalizer(previous);Check(PuffJumperBossScope.Current==null,"Boss marker restored");
        PuffJumperBossScope.Prefix(z,out previous);PuffJumperBossColdCheckpoint.Postfix(z,true);
        PuffJumperBossScope.Finalizer(previous);Check(!air.mSquished,"no-candidate Boss path restores without animation checkpoint");
        foreach(bool changeScene in new[]{false,true})
        {
            PuffJumperBossScope.Prefix(z,out var old);
            PuffJumperBossColdCheckpoint.Postfix(z,true);
            if(changeScene) b.mApp.GameScene=GameScenes.Loading; else z.mDead=true;
            PuffJumperBossAnimationCheckpoint.Prefix(z);
            Check(!air.mSquished,"Boss animation restores existing hide even after owner death or scene change");
            PuffJumperBossScope.Finalizer(old);
            z.mDead=false;b.mApp.GameScene=GameScenes.Playing;
        }
        PuffJumperBossScope.Prefix(z,out previous);z.DataID+=0x10000;
        PuffJumperBossColdCheckpoint.Postfix(z,true);Check(!air.mSquished,"recycled Boss owner cannot arm");
        PuffJumperBossScope.Finalizer(previous);
    }
    static void Tools()
    {
        var b=new Board();var air=Add(b,FusionPlant.Squish,Squish.Travel,false);
        var lily=Add(b,FusionPlant.Other);lily.mSeedType=SeedType.Lilypad;
        foreach(var cursor in new[]{CursorType.Normal,CursorType.Shovel,CursorType.Glove})
        {
            b.CursorObjects[0].CursorType=cursor;
            PuffJumperToolQuery.Prefix(b,320,320,0,true,out var hide);
            Check(air.mSquished&&NativeSelect(b)==lily,"tool native selection and later postfix preserve Lily fallback");
            PuffJumperToolQuery.Prefix(b,320,320,0,true,out var inner);PuffJumperToolQuery.Finalizer(inner);
            Check(air.mSquished,"nested tool query preserves outer hide through postfixes");
            PuffJumperToolQuery.Finalizer(hide);Check(!air.mSquished,"tool return/exception restores flight state");
        }
        b.CursorObjects[0].CursorType=CursorType.PlantFromBank;
        PuffJumperToolQuery.Prefix(b,320,320,0,true,out var skipped);Check(skipped==null&&!air.mSquished,"unrelated tool remains native");
        PuffJumperToolQuery.Prefix(b,320,320,-1,true,out skipped);Check(skipped==null,"invalid tool player ignored");
        b.CursorObjects[0].CursorType=CursorType.Glove;
        PuffJumperToolQuery.Prefix(b,320,320,0,false,out skipped);Check(skipped==null,"foreign tool veto does not authorize hide");
        int x=(int)b.GridToPixelX(3,2)+40,y=(int)b.GridToPixelY(3,2)+40;
        void Query(int col=3,int row=2,PlantPriority priority=PlantPriority.DiggingOrder,Board? board=null,bool run=true,bool expected=false)
        {
            PuffJumperGloveQuery.Prefix(board??b,col,row,priority,run,out var state);
            Check(air.mSquished==expected,"only exact managed glove pickup query hides flight");
            if(expected) Check(NativeSelect(b)==lily,"glove native Lily fallback survives selection postfix window");
            PuffJumperGloveQuery.Finalizer(state);Check(!air.mSquished,"glove query restores before pickup gameplay");
        }
        Query();
        PuffJumperGloveClickScope.Prefix(b,x,y,1,0,out var previous);
        Check(!air.mSquished,"managed glove marker never holds flags through gameplay");
        Query(expected:true);Query(col:4);Query(row:1);Query(priority:PlantPriority.Normal);Query(board:new Board());Query(run:false);
        var z=new Zombie {mBoard=b};PuffJumperBungeeScope.Prefix(z,out var oldBungee);Query();PuffJumperBungeeScope.Finalizer(oldBungee);
        var shot=new Projectile {mBoard=b};PuffJumperProjectileCandidateScope.Prefix(shot,out var oldShot);Query();PuffJumperProjectileCandidateScope.Finalizer(oldShot);
        LawnGlove.LawnGloveMod.Carried=lily;Query();LawnGlove.LawnGloveMod.Carried=null;
        PuffJumperGloveClickScope.Prefix(b,x,y,-1,0,out var outer);Query();PuffJumperGloveClickScope.Finalizer(outer);
        Query(expected:true);
        PuffJumperGloveQuery.Prefix(b,3,2,PlantPriority.DiggingOrder,true,out var query);
        PuffJumperGloveQuery.Prefix(b,3,2,PlantPriority.DiggingOrder,true,out var nested);PuffJumperGloveQuery.Finalizer(nested);
        Check(air.mSquished,"nested glove query cannot steal outer restore");
        PuffJumperGloveQuery.Finalizer(query);Check(!air.mSquished,"glove exception restores query and nesting depth");
        PuffJumperGloveClickScope.Finalizer(previous);Check(PuffJumperGloveClickScope.Current==null,"managed glove marker restores after exception");
        PuffJumperGloveClickScope.Prefix(b,x,y,1,1,out previous);Query();PuffJumperGloveClickScope.Finalizer(previous);
        Check(PuffJumperGloveClickScope.TargetMethod().Name=="HandleBoardClick","managed owner binding uses installed glove method");
    }
    static void ControllerShovel()
    {
        var b=new Board();var air=Add(b,FusionPlant.Squish,Squish.Travel,false);
        var lily=Add(b,FusionPlant.Other);lily.mSeedType=SeedType.Lilypad;
        var landed=Add(b,FusionPlant.Squish,Squish.Ground,false);
        b.CursorObjects[0].CursorType=b.CursorObjects[1].CursorType=CursorType.PlantFromBank;
        const int x=320,y=320;
        void Query(bool expected=false,int qx=x,int qy=y,int player=0,Board? board=null,bool run=true)
        {
            PuffJumperToolQuery.Prefix(board??b,qx,qy,player,run,out var query);
            Check(air.mSquished==expected,"explicit shovel query uses exact board/player/pixels/depth");
            if(expected)
            {
                Check(NativeSelect(b)==lily,"PlantFromBank shovel retains native Lily fallback");
                Check(!landed.mSquished,"landed vulnerable Squish stays available");
            }
            PuffJumperToolQuery.Finalizer(query);
            Check(!air.mSquished&&!air.mIsOnBoard,"tool query restores before caller mutation");
        }
        Query();
        PuffJumperShovelClickScope.Prefix(b,x,y,1,CursorType.Shovel,0,true,out var old);
        Check(!air.mSquished,"shovel caller marker does not hide across gameplay");
        Query(qx:x+1);Query(qy:y+1);Query(player:1);Query(board:new Board());Query(run:false);
        var z=new Zombie {mBoard=b};PuffJumperBungeeScope.Prefix(z,out var oldBungee);Query();PuffJumperBungeeScope.Finalizer(oldBungee);
        var shot=new Projectile {mBoard=b};PuffJumperProjectileCandidateScope.Prefix(shot,out var oldShot);Query();PuffJumperProjectileCandidateScope.Finalizer(oldShot);
        PuffJumperBossScope.Prefix(z,out var oldBoss);Query();PuffJumperBossScope.Finalizer(oldBoss);
        Query(expected:true);Query();
        Check(!air.mSquished,"caller can damage or remove only after hidden flags restore");
        PuffJumperShovelClickScope.Finalizer(old);
        Check(PuffJumperShovelClickScope.Current==null,"shovel caller owner leaves no state");
        foreach(var setup in new Action[] {
            ()=>PuffJumperShovelClickScope.Prefix(b,x,y,0,CursorType.Shovel,0,true,out old),
            ()=>PuffJumperShovelClickScope.Prefix(b,x,y,-1,CursorType.Shovel,0,true,out old),
            ()=>PuffJumperShovelClickScope.Prefix(b,x,y,1,CursorType.Glove,0,true,out old),
            ()=>PuffJumperShovelClickScope.Prefix(b,x,y,1,CursorType.PlantFromBank,0,true,out old),
            ()=>PuffJumperShovelClickScope.Prefix(b,x,y,1,CursorType.Shovel,-1,true,out old),
            ()=>PuffJumperShovelClickScope.Prefix(b,x,y,1,CursorType.Shovel,2,true,out old),
            ()=>PuffJumperShovelClickScope.Prefix(b,x,y,1,CursorType.Shovel,0,false,out old),
            ()=>PuffJumperShovelClickScope.Prefix(b,-50,y,1,CursorType.Shovel,0,true,out old),
            ()=>PuffJumperShovelClickScope.Prefix(b,900,y,1,CursorType.Shovel,0,true,out old),
            ()=>PuffJumperShovelClickScope.Prefix(b,x,700,1,CursorType.Shovel,0,true,out old) })
        { setup();Query();PuffJumperShovelClickScope.Finalizer(old); }
        PuffJumperShovelClickScope.Prefix(b,x,y,1,CursorType.Shovel,1,true,out old);
        Query();Query(expected:true,player:1);PuffJumperShovelClickScope.Finalizer(old);
        PuffJumperShovelClickScope.Prefix(b,x,y,1,CursorType.Shovel,0,true,out old);
        PuffJumperToolQuery.Prefix(b,x+1,y,0,true,out var outerQuery);
        Query();PuffJumperToolQuery.Finalizer(outerQuery);Query(expected:true);
        PuffJumperShovelClickScope.Prefix(b,x,y,1,CursorType.Shovel,0,true,out var outerClick);
        PuffJumperToolQuery.Prefix(b,x,y,0,true,out var scope);
        try
        {
            Check(air.mSquished&&NativeSelect(b)==lily,"nested shovel caller owns its own query hide");
            PuffJumperToolQuery.Prefix(b,x,y,0,true,out var nested);
            PuffJumperToolQuery.Finalizer(nested);Check(air.mSquished,"nested query cannot restore outer hide");
            throw new InvalidOperationException("query callback");
        }
        catch(InvalidOperationException) { }
        finally { PuffJumperToolQuery.Finalizer(scope);PuffJumperShovelClickScope.Finalizer(outerClick); }
        Check(!air.mSquished&&PuffJumperShovelClickScope.Current?.Finished==true,"query exception restores flags and preceding caller owner");
        PuffJumperShovelClickScope.Finalizer(old);
        foreach(var mutation in new Action[] {()=>b.Pointer=(IntPtr)999,()=>b.mApp.GameScene=GameScenes.Loading,()=>b.Disposed=true})
        {
            var pointer=b.Pointer;PuffJumperShovelClickScope.Prefix(b,x,y,1,CursorType.Shovel,0,true,out old);
            mutation();Query();PuffJumperShovelClickScope.Finalizer(old);
            b.Pointer=pointer;b.mApp.GameScene=GameScenes.Playing;b.Disposed=false;
        }
        PuffJumperShovelClickScope.Prefix(b,x,y,1,CursorType.Shovel,0,true,out old);
        PuffJumperToolQuery.Prefix(b,x,y,0,true,out scope);air.DataID+=0x10000;
        PuffJumperToolQuery.Finalizer(scope);PuffJumperShovelClickScope.Finalizer(old);
        Check(air.mSquished&&!air.mIsOnBoard,"replaced actor never restored by shovel query");
    }
    public static void Main()
    {
        Selection();Lifetime();Placement();Bungee();Boss();Tools();ControllerShovel();
        Console.WriteLine($"PASS {checks} query scope checks using actual shared scopes and Squish flight predicates.");
    }
}
