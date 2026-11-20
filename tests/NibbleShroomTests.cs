using FusionMod;
using Il2CppReloaded.Gameplay;
using NativeClock = Il2CppReloaded.Constants.Gameplay;

static class Program
{
    static int checks;
    static void Check(bool ok,string text) { ++checks; if(!ok) throw new Exception(text); }
    static void Near(double a,double b,string text) => Check(Math.Abs(a-b)<.003,$"{text}: {a} != {b}");
    static Plant New(Board? board=null) { board??=new(); var p=new Plant {mBoard=board}; board.m_plants.Add(p); NibbleShroom.Initialize(p); return p; }
    static void Step(Plant p,int ticks) { for(int i=0;i<ticks;++i) { FusionTime.BeginTick(p.mBoard); NibbleShroom.Tick(p); ++p.mBoard.mMainCounter; } }
    static Zombie Target(Plant p,float offset=60) { var z=new Zombie {mBoard=p.mBoard,TargetX=p.mX+40+offset,mRow=p.mRow}; p.mBoard.m_zombies.Add(z); return z; }
    static void Activate(Plant p) { Target(p); while(!NibbleShroom.Activated(p)) Step(p,1); p.mBoard.m_zombies.Clear(); }
    static void Frequency(double period,int ticks)
    {
        NativeClock.TIME_FOR_UPDATE=period;
        var p=New(); Activate(p); float x=p.mX,y=p.mY;
        Step(p,ticks);
        Check(NibbleShroom.Phase(p)==0,"exact rational animation loop");
        Near(p.mX-x,1.5*NibbleShroom.Unit,"three loops advance one and half source units");
        Near(p.mY-y,(p.mX-x)*.1,"roof projection preserves initial slot offset");
        Check(!p.mIsOnBoard && !p.mDead && p.mPlantCol==0,"detached live pool owner retains original cell");
        Check(p.mBoard.mApp.SoundSystem.Chomps==3,"one landing per source loop");
        Check(p.mTargetX is >=0 and <=5 && p.mStartRow<=113,"canonical centiseconds with explicit residue");
    }
    static void ClockAndSave()
    {
        NativeClock.TIME_FOR_UPDATE=1d/120; var p=New(); Activate(p); Step(p,17);
        int phase=NibbleShroom.Phase(p); float x=p.mX;
        FusionTime.BeginTick(p.mBoard); NibbleShroom.Tick(p); int once=NibbleShroom.Phase(p);
        NibbleShroom.Tick(p); Check(NibbleShroom.Phase(p)==once,"native owner deduplicated");
        Check(FusionTime.SourceStep(p,typeof(Program))==FusionTime.IsSourcePulse(p.mBoard),"other owner independent");
        ++p.mBoard.mMainCounter;
        p.mBoard.mPaused=true; Step(p,50); Check(NibbleShroom.Phase(p)==once,"pause advances no phase");
        p.mBoard.mPaused=false;
        FusionTime.Capture(p.mBoard); FusionTime.Reset(p.mBoard); FusionNativeTimeRecords.Forget(p.mBoard);
        var saved=New(p.mBoard); saved.mShootingCounter=p.mShootingCounter; saved.mStartRow=p.mStartRow; saved.mTargetX=p.mTargetX;
        saved.mIsOnBoard=false; saved.mX=p.mX; saved.mY=p.mY; saved.DataID=0xf0000011;
        NativeClock.TIME_FOR_UPDATE=.01; Step(saved,1);
        Check(NibbleShroom.Phase(saved)==(once+6)%680,"save fields reconstruct across rate switch");
        Check(p.mX==x,"inactive half of loop has no motion");
        FusionTime.BeginLoad(); int before=NibbleShroom.Phase(saved); NibbleShroom.Tick(saved);
        Check(NibbleShroom.Phase(saved)==before,"deserialize does not run actions"); FusionTime.EndLoad(saved.mBoard,true);
        FusionTime.BeginTick(saved.mBoard); bool first=FusionTime.NativeStep(saved,typeof(Program));
        Check(first,"new owner consumes current generation"); ++saved.DataID;
        Check(FusionTime.NativeStep(saved,typeof(Program)),"recycled full generation has independent claim");
    }
    static void TargetsAndLanding()
    {
        NativeClock.TIME_FOR_UPDATE=.01;
        var p=New(); var z=Target(p); z.mBodyHealth=99; z.mBodyMaxHealth=300; z.mShieldHealth=9999;
        Check(NibbleShroom.Target(p,z,1.5f),"ordinary same-row target accepted");
        foreach(var excluded in Enum.GetValues<ZombiePhase>().Where(v=>v!=ZombiePhase.ZombieNormal && v!=ZombiePhase.ZombieDying && v!=ZombiePhase.PogoBouncing))
        { z.mZombiePhase=excluded;Check(!NibbleShroom.Target(p,z,1.5f),$"source flight state excluded: {excluded}"); }
        z.mZombieType=ZombieType.Pogo;z.mZombiePhase=ZombiePhase.PogoBouncing;z.Targetable=false;
        Check(NibbleShroom.Target(p,z,1.5f),"source ordinary Pogo bounce survives native range-height veto");
        z.mMindControlled=true;Check(!NibbleShroom.Target(p,z,1.5f),"Pogo adapter retains hostility guard");z.mMindControlled=false;
        z.mDead=true;Check(!NibbleShroom.Target(p,z,1.5f),"Pogo adapter retains dead guard");z.mDead=false;
        var ownBoard=z.mBoard;z.mBoard=new Board();Check(!NibbleShroom.Target(p,z,1.5f),"Pogo adapter retains board guard");z.mBoard=ownBoard;
        z.mZombiePhase=ZombiePhase.PogoHighBounce1;z.Targetable=true;
        Check(!NibbleShroom.Target(p,z,1.5f),"source high Pogo excluded even when native range accepts");
        z.mZombieType=ZombieType.Normal;z.Targetable=true;
        z.mZombiePhase=ZombiePhase.ZombieNormal;z.mRow=1;Check(!NibbleShroom.Target(p,z,1.5f),"neighbor row excluded");z.mRow=0;
        Check(NibbleShroom.Swallow(z),"strict threshold ignores shield"); z.mBodyHealth=100;
        Check(!NibbleShroom.Swallow(z),"one third boundary does not swallow"); z.mHelmHealth=1;z.mHelmMaxHealth=300;
        Check(NibbleShroom.Swallow(z),"first armor participates"); z.mZombieType=ZombieType.Gargantuar;
        Check(!NibbleShroom.Swallow(z),"Garg excluded even weak");z.mZombieType=ZombieType.Imp;z.mBodyHealth=300;
        Check(NibbleShroom.Swallow(z),"Imp swallowed at full health");
        z.mZombieType=ZombieType.Normal;z.mHelmHealth=z.mHelmMaxHealth=0;
        p.mShootingCounter=1;p.mIsOnBoard=false;
        NibbleShroom.Land(p,p.DataID,p.mBoard);Check(z.DamageTaken==200 && z.LastFlags==DamageFlags.BypassesShield,"native shieldless damage");
        z.mBodyHealth=99;NibbleShroom.Land(p,p.DataID,p.mBoard);Check(z.Deaths==1,"weak target dies with loot");
        var pool=New();pool.mBoard.Pool=true;pool.mShootingCounter=1;pool.mIsOnBoard=false;
        var poolTarget=Target(pool);NibbleShroom.Land(pool,pool.DataID,pool.mBoard);
        Check(pool.mDead && poolTarget.DamageTaken==0 && pool.mBoard.mApp.Particles==1,"source original water cell dies before attack, including support adaptation");
        var stale=New(); stale.mShootingCounter=1; var target=Target(stale);
        stale.mBoard.mApp.SoundSystem.OnSound=()=>++stale.DataID;
        NibbleShroom.Land(stale,stale.DataID,stale.mBoard);Check(target.DamageTaken==0,"recycled owner after callback cannot attack");
        var replacement=New();replacement.mShootingCounter=1;var first=Target(replacement);var second=Target(replacement);
        first.OnDamage=()=>++second.DataID;
        NibbleShroom.Land(replacement,replacement.DataID,replacement.mBoard);Check(second.DamageTaken==0,"recycled target generation excluded");
    }
    static void Damage()
    {
        var ordinary=New();ordinary.mImitaterType=SeedType.None;
        PuffJumperBiteDamageScope.Prefix(ordinary,out var ordinaryPrevious);ordinary.mPlantHealth=0;ordinary.Die();
        PuffJumperBiteDamageScope.Finalizer(ordinaryPrevious);Check(ordinary.mDead,"ordinary plants retain native lethal damage");
        var inactive=New();PuffJumperBiteDamageScope.Prefix(inactive,out ordinaryPrevious);inactive.mPlantHealth=0;inactive.Die();
        PuffJumperBiteDamageScope.Finalizer(ordinaryPrevious);Check(inactive.mDead,"inactive Nibble remains damageable");
        var p=New();p.mShootingCounter=1;p.mIsOnBoard=false;
        PuffJumperBiteDamageScope.Prefix(p,out var old);p.mPlantHealth=-20;++p.mBoard.mPlantsEaten;p.Die();
        Check(!p.mDead && p.mPlantHealth==300 && p.mBoard.mPlantsEaten==0,"lethal bite preserves collider and vetoes HP writer");
        PuffJumperBiteDamageScope.Finalizer(old);Check(PuffJumperDamage.Current==null,"bite scope restored");
        PuffJumperBiteDamageScope.Prefix(p,out old);p.mPlantHealth=200;
        PuffJumperExplosionDamageScope.Prefix(p.mBoard,out var nested);p.Die();
        Check(!p.mDead,"direct radius death vetoed");PuffJumperExplosionDamageScope.Finalizer(nested);PuffJumperBiteDamageScope.Finalizer(old);
        Check(p.mPlantHealth==300,"nested health scopes restore original health");
        PuffJumperBiteDamageScope.Prefix(p,out old);++p.DataID;p.mPlantHealth=10;PuffJumperBiteDamageScope.Finalizer(old);
        Check(p.mPlantHealth==10,"recycled health never restored");
        p.mPlantHealth=300;PuffJumperBiteDamageScope.Prefix(p,out old);p.mBoard.Playing=false;p.mPlantHealth=12;PuffJumperBiteDamageScope.Finalizer(old);
        Check(p.mPlantHealth==12,"disposed board health never restored");p.mBoard.Playing=true;
        PuffJumperExplosionDamageScope.Prefix(p.mBoard,out old);PlayerPlantingScope.ConsumingIngredient=p.Pointer;p.Die();
        Check(p.mDead,"ingredient removal remains legal");PlayerPlantingScope.ConsumingIngredient=default;PuffJumperExplosionDamageScope.Finalizer(old);
    }
    static (bool Native, Plant? Target) Select(Projectile shot, bool runOriginal = true, Plant? initial = null)
    {
        PuffJumperProjectileCandidateScope.Prefix(shot, out var previous);
        var result = initial;
        try
        {
            bool native = PuffJumperProjectilePhysicalSearch.Prefix(shot, runOriginal, ref result);
            return (native, result);
        }
        finally { PuffJumperProjectileCandidateScope.Finalizer(previous); }
    }

    static void Query()
    {
        var moving = New();
        moving.mShootingCounter = 1; moving.mIsOnBoard = false; moving.mX = 300;
        var board = moving.mBoard;
        var sentinel = New(board);
        sentinel.mImitaterType = SeedType.Peashooter;
        var shot = new Projectile { mBoard = board, mProjectileType = ProjectileType.Basketball,
            DataID = 0xf0010022, mX = 320, mY = 1000 };
        board.TopLookup = (col, row, priority) =>
        {
            Check(col == moving.mPlantCol && row == moving.mRow && priority == PlantPriority.CatapultOrder,
                "basketball uses the native candidate home cell and catapult priority");
            return (sentinel, true);
        };
        foreach (bool pool in new[] { false, true })
        {
            board.Pool = pool;
            var result = Select(shot);
            Check(!result.Native && ReferenceEquals(result.Target, moving),
                "horizontal geometry selects physical Nibble on either ground or pool");
        }
        shot.mX = 372;
        Check(Select(shot) == (false, null), "exactly eight pixels of horizontal overlap miss");
        shot.mX = 371;
        Check(ReferenceEquals(Select(shot).Target, moving), "nine pixels of horizontal overlap hit");
        shot.mX = 320;

        board.m_plants.Remove(sentinel);
        board.m_plants.Insert(0, sentinel);
        sentinel.mX = 310;
        Check(ReferenceEquals(Select(shot).Target, sentinel), "first matching ordinary rectangle owns its home claim");
        int homeLookups = 0;
        board.TopLookup = (_, _, _) => { ++homeLookups; return (null, true); };
        Check(Select(shot) == (false, null) && homeLookups == 1,
            "null result from the first overlap terminates the search before a later matching rectangle");
        board.TopLookup = (_, _, _) => (sentinel, true);
        sentinel.mX = 0;
        var air = New(board);
        air.mImitaterType = SeedType.Squash; air.mStartRow = 2; air.mX = 310;
        var rolling = New(board);
        rolling.mImitaterType = SeedType.None; rolling.RollingPepper = true; rolling.mX = 310;
        board.m_plants.Remove(air); board.m_plants.Insert(0, air);
        board.m_plants.Remove(rolling); board.m_plants.Insert(0, rolling);
        Check(ReferenceEquals(Select(shot).Target, moving), "airborne Squish and rolling Pepper yield to next collider");
        Check(air.mX == 310 && rolling.mX == 310, "rectangle search changes no plant position");

        moving.OnRect = () => ++moving.DataID;
        Check(Select(shot) == (false, null), "a recycled rectangle candidate cannot claim the hit");
        moving.OnRect = null;
        board.TopLookup = (_, _, _) => { ++shot.DataID; return (sentinel, true); };
        Check(ReferenceEquals(Select(shot).Target, sentinel), "recycled projectile cannot rebind native result");
        board.TopLookup = (_, _, _) => (sentinel, false);
        Check(ReferenceEquals(Select(shot).Target, sentinel), "foreign veto of home lookup retains its result");

        Check(Select(shot, false, sentinel) == (true, sentinel), "earlier query veto is preserved");
        shot.mRow = 1;
        Check(Select(shot, initial: sentinel) == (true, sentinel), "unaffected rows use original query");
        shot.mRow = 0;
        PuffJumperProjectileCandidateScope.Prefix(shot, out var previous);
        var outer = PuffJumperProjectileCandidateScope.Current;
        _ = Select(shot);
        Check(ReferenceEquals(PuffJumperProjectileCandidateScope.Current, outer), "nested query restores parent");
        PuffJumperProjectileCandidateScope.Finalizer(previous);
        Check(PuffJumperProjectileCandidateScope.Current == null, "query releases the outer scope");

        Check(!moving.NotOnGround(), "moving Nibble keeps native physical ground query");
        PuffJumperPlacementScope.Prefix(board, moving.mPlantCol, moving.mRow, out var hidden);
        Check(moving.mSquished && !moving.mIsOnBoard, "placement query hides detached source slot");
        PuffJumperPlacementScope.Finalizer(hidden);
        Check(!moving.mSquished && !moving.mIsOnBoard, "placement restores detached source state");
    }

    static void SquishQuery()
    {
        var moving = New();
        moving.mImitaterType = SeedType.Squash; moving.mIsOnBoard = false; moving.mX = 300;
        var board = moving.mBoard;
        var sentinel = New(board);
        sentinel.mImitaterType = SeedType.None;
        var shot = new Projectile { mBoard = board, mProjectileType = ProjectileType.Basketball,
            DataID = 0xf0010033, mX = 320 };
        board.TopLookup = (_, _, _) => (sentinel, true);
        Check(ReferenceEquals(Select(shot).Target, moving), "grounded displaced Squish receives physical hit");
        moving.mStartRow = 2;
        Check(Select(shot) == (false, null), "airborne Squish suppresses its collider");
        moving.mStartRow = 0;
        board.TopLookup = (_, _, _) => { moving.mStartRow = 2; return (sentinel, true); };
        Check(ReferenceEquals(Select(shot).Target, sentinel), "takeoff during home lookup invalidates claim");
        moving.mStartRow = 0;
        board.TopLookup = (_, _, _) => { moving.mImitaterType = SeedType.Chomper; return (sentinel, true); };
        Check(ReferenceEquals(Select(shot).Target, sentinel), "a changed identity cannot inherit a claim");
        moving.mImitaterType = SeedType.Squash;

        shot.mProjectileType = ProjectileType.ZombiePea;
        var excludedSeeds = new[] { SeedType.Puffshroom, SeedType.Sunshroom, SeedType.Potatomine,
            SeedType.Spikeweed, SeedType.Spikerock, SeedType.Lilypad };
        sentinel.RollingPepper = true;
        foreach (var seed in excludedSeeds)
        {
            sentinel.mSeedType = seed;
            Check(Select(shot) == (true, null), "ZombiePea retains original query when only excluded plants are present");
        }

        sentinel.RollingPepper = false;
        sentinel.mX = 320; sentinel.mPlantCol = 4;
        sentinel.mSeedType = SeedType.Tallnut;
        var rolling = New(board);
        rolling.mSeedType = SeedType.Tallnut; rolling.RollingPepper = true; rolling.mX = 310;
        board.m_plants.Remove(rolling); board.m_plants.Insert(0, rolling);
        board.TopLookup = (col, row, priority) =>
        {
            Check(col == 4 && row == 0 && priority == PlantPriority.EatingOrder,
                "ZombiePea respects its seed exclusions and native eating priority");
            return (sentinel, true);
        };
        var excluded = New(board);
        excluded.mImitaterType = SeedType.None; excluded.mX = 315; excluded.mPlantCol = 2;
        board.m_plants.Remove(excluded); board.m_plants.Insert(1, excluded);
        foreach (var seed in excludedSeeds)
        {
            excluded.mSeedType = seed;
            Check(Select(shot) == (false, sentinel), "ZombiePea keeps ordinary fallback after excluded and rolling plants");
        }
    }

    static void Main() { Frequency(.01,340); Frequency(1d/120,408); ClockAndSave();TargetsAndLanding();Damage();Query();SquishQuery();Console.WriteLine($"Nibble-shroom: {checks} checks passed."); }
}
