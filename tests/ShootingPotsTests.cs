using FusionMod;
using Il2CppReloaded.Gameplay;

static class Program
{
    static void Equal<T>(T expected, T actual, string message)
    { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"{message}: expected {expected}, got {actual}"); }
    static Plant Add(Board board, FusionPlant identity, int row = 2)
    {
        var plant = new Plant { mBoard = board, mSeedType = identity.NativeType,
            mImitaterType = identity.Marker, mRow = row, mY = row * 100 };
        board.m_plants.Add(plant); return plant;
    }
    static void TickPot(Board board, Plant pot, int ticks)
    {
        for (int i = 0; i < ticks; ++i)
        {
            if (!board.mPaused) ++board.mMainCounter;
            ShootingPotAbilityPatch.Postfix(pot);
        }
    }
    static int Damage(Projectile shot, Zombie zombie, int value)
    {
        var old = ShootingPots.CurrentHit;
        try
        {
            ShootingPots.CurrentHit = ShootingPots.Begin(shot, zombie, false);
            shot.GetDamageFlags(zombie);
            ShootingPots.Damage(zombie, ref value); return value;
        }
        finally { ShootingPots.CurrentHit = old; }
    }
    static void Recipes()
    {
        foreach (var (a, b, result) in new (FusionPlant, FusionPlant, FusionPlant)[] {
            (SeedType.Flowerpot, SeedType.Peashooter, FusionPlant.PeaPot),
            (SeedType.Flowerpot, SeedType.Threepeater, FusionPlant.TriPot),
            (SeedType.Kernelpult, SeedType.Threepeater, FusionPlant.KernelTriPult),
            (SeedType.Melonpult, SeedType.Threepeater, FusionPlant.MelonTriPult) })
        {
            Equal(true, FusionRecipes.TryGet(a, b, out var direct), "recipe"); Equal(result, direct, "result");
            Equal(true, FusionRecipes.TryGet(b, a, out var reversed), "reverse recipe"); Equal(result, reversed, "reverse result");
            Equal(result, FusionPlant.From(Add(new Board(), result)), "saved identity");
        }
        foreach (var (a, b, result) in new (FusionPlant, FusionPlant, FusionPlant)[] {
            (SeedType.Cabbagepult, SeedType.Kernelpult, FusionPlant.TacoPult),
            (SeedType.Kernelpult, SeedType.Melonpult, FusionPlant.CobPult),
            (SeedType.Cabbagepult, SeedType.Melonpult, FusionPlant.OverripeMelon),
            (SeedType.Cabbagepult, SeedType.Threepeater, FusionPlant.CabbageTriPult) })
        {
            Equal(true, FusionRecipes.TryGet(a, b, out var direct), "new recipe"); Equal(result, direct, "new result");
            Equal(true, FusionRecipes.TryGet(b, a, out var reverse), "new reverse"); Equal(result, reverse, "new reverse result");
            Equal(result, FusionPlant.From(Add(new Board(), result)), "new saved identity");
        }
    }
    static void Pots()
    {
        var board = new Board(); var plant = Add(board, SeedType.Peashooter); var pot = Add(board, FusionPlant.PeaPot);
        for (int i = 0; i < 30; ++i) plant.Fire(null, 2, PlantWeapon.Primary);
        Equal(35, board.Shots.Count, "five additional attacks"); Equal(5, pot.mTargetY, "additional AnimShoot events count");
        for (int i = 0; i < 25; ++i) plant.Fire(null, 2, PlantWeapon.Primary);
        Equal(65, board.Shots.Count, "next trigger includes previous bonus attacks");
        Equal(5, pot.mTargetY, "event counter persists on support");
        plant.Fire(null, 2, PlantWeapon.Secondary); Equal(5, pot.mTargetY, "secondary event excluded");
        board.mPaused = true; plant.Fire(null, 2, PlantWeapon.Primary); Equal(5, pot.mTargetY, "pause");
        board = new Board(); plant = Add(board, SeedType.Peashooter, 0); pot = Add(board, FusionPlant.TriPot, 0);
        for (int i = 0; i < 6; ++i) plant.Fire(null, 0, PlantWeapon.Primary);
        Equal(8, board.Shots.Count, "six attacks grant two lane shots");
        Equal(0, board.Shots[^2].mRow, "top edge compensation"); Equal(1, board.Shots[^1].mRow, "lower adjacent row");
        var enemy = new Zombie { mBoard = board, mRow = 1 };
        Equal(10, Damage(board.Shots[^1], enemy, 20), "half damage");
        board.Shots[^1].mProjectileType = ProjectileType.Fireball;
        Equal(30, Damage(board.Shots[^1], enemy, 40), "torchwood adds after halving");
        board.Shots[^1].mProjectileType = ProjectileType.Pea; board.Shots[^1].mCobTargetX = -450011;
        enemy.mShieldHealth = 100; Equal(140, Damage(board.Shots[^1], enemy, 180), "iron shield bonus stays intact");
        Equal(0, pot.mTargetY, "lane shots do not raise events");
        board = new Board(); plant = Add(board, SeedType.Threepeater); pot = Add(board, FusionPlant.TriPot);
        plant.Fire(null, 2, PlantWeapon.Primary); enemy = new Zombie { mBoard = board };
        Equal(24, Damage(board.Shots[0], enemy, 20), "threepeater bonus");
        board.Shots[0].mProjectileType = ProjectileType.Fireball;
        Equal(44, Damage(board.Shots[0], enemy, 40), "boost before torchwood flat addition");
        for (int i = 0; i < 5; ++i) plant.Fire(null, 2, PlantWeapon.Primary);
        Equal(6, board.Shots.Count, "native threepeater schedules bonus instead of overlapping main volley");
        TickPot(board, pot, 20);
        Equal(8, board.Shots.Count, "native threepeater grants two extra attacks after six shots");
        Equal(0, pot.mTargetY, "native threepeater bonus attacks do not count themselves");
        Equal(1, board.Shots[^2].mRow, "upper bonus row");
        Equal(3, board.Shots[^1].mRow, "lower bonus row");
        Equal(plant.mY + 70 + 100, board.Shots[^2].mShadowY, "native upper shadow shifted exactly once");
        Equal(plant.mY + 70 - 100, board.Shots[^1].mShadowY, "native lower shadow shifted exactly once");
        foreach (bool bypass in new[] { false, true })
        {
            board = new Board { BypassFireHooks = bypass, BypassFactoryHook = true };
            plant = Add(board, SeedType.Threepeater); pot = Add(board, FusionPlant.TriPot);
            for (int i = 0; i < 6; ++i) ShootingPots.FireShot(plant, plant.mRow);
            TickPot(board, pot, 20);
            Equal(8, board.Shots.Count, "managed shots count with and without Fire detours");
            Equal(0, pot.mTargetY, "managed helper never double-counts a detour");
            Equal(10, Damage(board.Shots[^1], new Zombie { mBoard = board }, 20), "managed extra retains half damage");
            Equal<ShootingPots.Fire?>(null, ShootingPots.Firing, "managed shot restores firing scope");
        }
        board = new Board(); plant = Add(board, SeedType.Threepeater); pot = Add(board, FusionPlant.TriPot);
        ShootingPotFireScope.Prefix(plant, PlantWeapon.Primary, out var nativeScope);
        board.AddProjectile(120, 230, 0, plant.mRow, ProjectileType.Pea);
        ShootingPots.Firing = null;
        ShootingPotFireScope.Postfix(nativeScope);
        ShootingPotFireScope.Postfix(nativeScope);
        ShootingPotFireScope.Finalizer(nativeScope);
        Equal(1, pot.mTargetY, "native completion owns its captured frame despite changed ambient state");
        Equal<ShootingPots.Fire?>(null, ShootingPots.Firing, "native completion restores previous scope");
        board = new Board(); plant = Add(board, SeedType.Threepeater); pot = Add(board, FusionPlant.TriPot);
        ShootingPotFireScope.Prefix(plant, PlantWeapon.Primary, out var outerScope);
        board.AddProjectile(120, 230, 0, plant.mRow, ProjectileType.Pea);
        ShootingPotFireScope.Prefix(plant, PlantWeapon.Primary, out var innerScope);
        board.AddProjectile(120, 230, 0, plant.mRow, ProjectileType.Pea);
        ShootingPotFireScope.Postfix(innerScope); ShootingPotFireScope.Finalizer(innerScope);
        ShootingPotFireScope.Postfix(outerScope); ShootingPotFireScope.Finalizer(outerScope);
        Equal(2, pot.mTargetY, "nested native shots from the same plant each own a count");
        board = new Board(); plant = Add(board, SeedType.Threepeater, 1); pot = Add(board, FusionPlant.TriPot, 1);
        for (int volley = 0; volley < 2; ++volley)
            foreach (int row in new[] { 2, 1, 0 }) plant.Fire(null, row, PlantWeapon.Primary);
        Equal(ShootingPots.BonusDelayTicks, pot.mStartRow, "new bonus saves a relative source delay");
        TickPot(board, pot, 19);
        Equal(6, board.Shots.Count, "no extra peas before twenty simulation ticks");
        board.mPaused = true; TickPot(board, pot, 100);
        Equal(6, board.Shots.Count, "pause preserves pending bonus");
        board.mPaused = false; TickPot(board, pot, 1);
        Equal(8, board.Shots.Count, "bonus pair appears exactly twenty active ticks later");
        Equal(0, board.Shots[6].mRow, "delayed upper row"); Equal(2, board.Shots[7].mRow, "delayed lower row");
        Equal(10, Damage(board.Shots[6], new Zombie { mBoard = board }, 20), "delayed bonus keeps half damage");
        ShootingPotAbilityPatch.Postfix(pot);
        Equal(8, board.Shots.Count, "same-tick repeat cannot duplicate a consumed bonus");
        Equal(0, pot.mTargetY, "delayed extras do not count towards next trigger");

        board = new Board(); plant = Add(board, SeedType.Threepeater); pot = Add(board, FusionPlant.TriPot);
        for (int i = 0; i < 12; ++i) plant.Fire(null, plant.mRow, PlantWeapon.Primary);
        TickPot(board, pot, 20); Equal(14, board.Shots.Count, "first queued bonus pair");
        TickPot(board, pot, 20); Equal(16, board.Shots.Count, "second trigger is retained instead of overwritten");

        board = new Board(); plant = Add(board, SeedType.Threepeater); pot = Add(board, FusionPlant.TriPot);
        for (int i = 0; i < 6; ++i) plant.Fire(null, plant.mRow, PlantWeapon.Primary);
        plant.DataID++;
        TickPot(board, pot, 20);
        Equal(6, board.Shots.Count, "reused shooter slot never inherits pending bonus");
        Equal(0, pot.mLaunchCounter, "stale source cancels pending pair");

        board = new Board(); plant = Add(board, SeedType.Threepeater); pot = Add(board, FusionPlant.TriPot);
        for (int i = 0; i < 6; ++i) plant.Fire(null, plant.mRow, PlantWeapon.Primary);
        ++plant.mPlantCol; TickPot(board, pot, 20);
        Equal(6, board.Shots.Count, "moving shooter off pot cancels pending pair");

        board = new Board(); plant = Add(board, SeedType.Threepeater); pot = Add(board, FusionPlant.TriPot);
        for (int i = 0; i < 6; ++i) plant.Fire(null, plant.mRow, PlantWeapon.Primary);
        TickPot(board, pot, 7);
        board.m_plants.Remove(pot);
        var loadedPot = Add(board, FusionPlant.TriPot);
        loadedPot.mLaunchCounter = pot.mLaunchCounter; loadedPot.mStartRow = pot.mStartRow;
        loadedPot.mDoSpecialCountdown = pot.mDoSpecialCountdown;
        loadedPot.mTargetZombieID = pot.mTargetZombieID;
        board.mMainCounter = 100000; // New saved delays do not depend on a board epoch.
        TickPot(board, loadedPot, 12); Equal(6, board.Shots.Count, "restored saved fields retain remaining delay");
        TickPot(board, loadedPot, 1); Equal(8, board.Shots.Count, "restored fields emit owed pair once");

        foreach (int nativeCounter in new[] { 400, int.MaxValue - 5 })
        {
            board = new Board { mMainCounter = nativeCounter };
            plant = Add(board, SeedType.Threepeater); pot = Add(board, FusionPlant.TriPot);
            // Legacy save fixture: no unit tag, absolute native board deadline.
            pot.mLaunchCounter = 1;
            pot.mTargetZombieID = (ZombieID)plant.DataID;
            pot.mStartRow = unchecked(nativeCounter + 13);
            pot.mDoSpecialCountdown = 0;
            TickPot(board, pot, 12);
            Equal(0, board.Shots.Count, "legacy absolute delay preserves remaining time, including native wrap");
            Equal(ShootingPots.RelativeBonusDelay, pot.mDoSpecialCountdown, "legacy deadline gains persistent relative-time tag");
            Equal(1, pot.mStartRow, "legacy migration does not subtract its first native step twice");
            TickPot(board, pot, 1);
            Equal(2, board.Shots.Count, "legacy migrated pair fires once at original remaining boundary");
        }
    }
    static void Lobbers()
    {
        var board = new Board(); var kernel = Add(board, FusionPlant.KernelTriPult);
        TriLobbers.Initialize(kernel); board.m_zombies.Add(new Zombie { mBoard = board });
        kernel.mShootingCounter = 0; TriLobbers.Shoot(kernel);
        Equal(3, board.Shots.Count, "first kernel volley"); Equal(1, kernel.mTargetY, "non-butter series");
        TriLobbers.Shoot(kernel); Equal(7, board.Shots.Count, "second volley gains one");
        kernel.mShootingCounter = 1; TriLobbers.Shoot(kernel);
        Equal(11, board.Shots.Count, "butter uses existing extra count"); Equal(0, kernel.mTargetY, "butter resets series");
        Equal(ProjectileType.Butter, board.Shots[^1].mProjectileType, "butter extra projectile");
        var enemy = board.m_zombies[0]; TriLobbers.Impact(board.Shots[^1], enemy);
        Equal(40, enemy.Damage, "butter damage"); Equal(1, enemy.Buttered, "butter status");
        board = new Board(); var melon = Add(board, FusionPlant.MelonTriPult, 0); Add(board, FusionPlant.TriPot, 0);
        TriLobbers.Initialize(melon); board.m_zombies.Add(new Zombie { mBoard = board, mRow = 0 });
        TriLobbers.Tick(melon); Equal(0, board.Shots.Count, "wait for original animation event");
        for (int i = 0; i < 24; ++i) TriLobbers.Tick(melon);
        Equal(0, board.Shots.Count, "not before 0.25 seconds");
        TriLobbers.Tick(melon); Equal(3, board.Shots.Count, "quarter-second emission");
        Equal(2, board.Shots.Count(shot => shot.mRow == 0), "lobber edge compensation");
        Equal(96, board.Shots[0].mCobTargetRow, "threepeater lineage attack bonus");
        int phase = melon.mStartRow; board.mPaused = true; TriLobbers.Tick(melon); Equal(phase, melon.mStartRow, "paused phase");
        board.mPaused = false; var direct = board.m_zombies[0]; var splash = new Zombie { mBoard = board, mRow = 1, X = direct.X };
        board.m_zombies.Add(splash); var shot = board.Shots[0]; shot.mPosX = direct.X; shot.mPosY = 80;
        TriLobbers.Impact(shot, direct); Equal(96, direct.Damage, "melon direct damage"); Equal(32, splash.Damage, "melon splash");
        TriLobbers.Impact(shot, direct); Equal(96, direct.Damage, "no repeated impact");
        Equal(null, ShootingPots.Firing, "fire scope restored"); Equal(null, ShootingPots.CurrentHit, "hit scope restored");
    }
    static Zombie Enemy(Board board, int row = 2, float x = 400)
    {
        var zombie = new Zombie { mBoard = board, mRow = row, X = x };
        board.m_zombies.Add(zombie); return zombie;
    }
    static void MixedLobbers()
    {
        foreach (var identity in new[] { FusionPlant.TacoPult, FusionPlant.CobPult, FusionPlant.OverripeMelon })
        {
            var board = new Board(); var plant = Add(board, identity); Enemy(board);
            LobberFusions.Initialize(plant); board.Rolls.Enqueue(1); board.Rolls.Enqueue(0);
            int eventTick = identity == FusionPlant.TacoPult ? 32 : 34;
            LobberFusions.Tick(plant);
            for (int i = 1; i < eventTick; ++i) LobberFusions.Tick(plant);
            Equal(0, board.Shots.Count, "no early emission");
            int phase = plant.mStartRow, choice = plant.mShootingCounter;
            board.mPaused = true; LobberFusions.Tick(plant); Equal(phase, plant.mStartRow, "pause preserves phase");
            board.mPaused = false; LobberFusions.Tick(plant);
            Equal(1, board.Shots.Count, "original event produces one projectile");
            Equal(choice, plant.mShootingCounter, "butter choice remains saved");
            Equal(identity == FusionPlant.TacoPult ? 60 : identity == FusionPlant.CobPult ? 160 : 80,
                board.Shots[0].mCobTargetRow, "current source damage");
            plant.OffGround = true; phase = plant.mStartRow; LobberFusions.Tick(plant);
            Equal(phase, plant.mStartRow, "carried plant does not advance");
        }
        var lostBoard = new Board(); var lostPlant = Add(lostBoard, FusionPlant.TacoPult);
        var lostTarget = Enemy(lostBoard); LobberFusions.Initialize(lostPlant); LobberFusions.Tick(lostPlant);
        lostTarget.mDead = true;
        for (int i = 0; i < 32; ++i) LobberFusions.Tick(lostPlant);
        Equal(1, lostBoard.Shots.Count, "begun attack still emits after target loss");
        var pots = new Board();
        foreach (var identity in new[] { FusionPlant.TacoPult, FusionPlant.OverripeMelon, FusionPlant.CabbageTriPult })
        {
            var plant = Add(pots, identity); Add(pots, FusionPlant.CabbagePot);
            Equal(true, PotBoosts.FasterCabbage(plant), "new cabbage support recipient");
            plant.mLaunchCounter = 300; CabbagePotIntervalPatch.Postfix(plant, 0);
            Equal(200, plant.mLaunchCounter, "support scales newly scheduled interval once");
        }
    }
    static void BouncingLobbers()
    {
        var board = new Board(); var plant = Add(board, FusionPlant.TacoPult);
        var first = Enemy(board); var second = Enemy(board, x: 450); var third = Enemy(board, x: 500);
        LobberProjectiles.Fire(plant, 2, first, false, 0); var shot = board.Shots[^1];
        shot.mVelX = 10; shot.mVelZ = 8;
        LobberProjectiles.Impact(shot, first); Equal(40, first.Damage, "first taco hit");
        Equal(false, shot.mDead, "first bounce retained"); Equal(5f, shot.mVelX, "horizontal halving");
        Equal(-4f, shot.mVelZ, "reflected half-height arc");
        LobberProjectiles.Impact(shot, first); Equal(40, first.Damage, "same target excluded");
        LobberProjectiles.Impact(shot, second); Equal(40, second.Damage, "second taco hit");
        LobberProjectiles.Impact(shot, first); Equal(40, first.Damage, "both old handles excluded");
        LobberProjectiles.Impact(shot, third); Equal(40, third.Damage, "third taco hit");
        Equal(true, shot.mDead, "three total hits");
        plant.mShootingCounter = 1; LobberProjectiles.Fire(plant, 2, first, false, 0);
        shot = board.Shots[^1]; first.mButteredCounter = 500;
        LobberProjectiles.Impact(shot, first); Equal(100, first.Damage, "butter taco damage");
        Equal(500, first.mButteredCounter, "butter never shortens existing status");
        LobberProjectiles.Impact(shot, second); Equal(400, second.mButteredCounter, "four-second butter");
        LobberProjectiles.Impact(shot, null); Equal(true, shot.mDead, "landing ends butter taco");
    }
    static void MixedMelons()
    {
        var board = new Board(); var plant = Add(board, FusionPlant.CobPult); plant.mShootingCounter = 1;
        var direct = Enemy(board); var other = Enemy(board, x: 430);
        LobberProjectiles.Fire(plant, 2, direct, false, 0); var shot = board.Shots[^1];
        shot.mPosX = 400; shot.mPosY = 230;
        LobberProjectiles.Impact(shot, direct);
        Equal(160, direct.Damage, "butter cob direct"); Equal(52, other.Damage, "original 0.33 splash");
        Equal(400, direct.mButteredCounter, "direct butter duration"); Equal(200, other.mButteredCounter, "two-target butter dilution");
        LobberProjectiles.Impact(shot, direct); Equal(160, direct.Damage, "dead shot cannot hit twice");
        board = new Board(); plant = Add(board, FusionPlant.CobPult); Add(board, FusionPlant.MelonPot);
        direct = Enemy(board); other = Enemy(board, x: 430);
        LobberProjectiles.Fire(plant, 2, direct, false, 0); shot = board.Shots[^1];
        Equal(-140, shot.mCobTargetRow, "source base damage and saved pot bonus");
        board.m_plants.RemoveAt(1); shot.mPosX = 400; shot.mPosY = 230;
        LobberProjectiles.Impact(shot, direct); Equal(93, other.Damage, "pot removal preserves 0.67 splash snapshot");
        board = new Board(); plant = Add(board, FusionPlant.OverripeMelon);
        direct = Enemy(board); other = Enemy(board, x: 430);
        LobberProjectiles.Fire(plant, 2, direct, false, 0); shot = board.Shots[^1];
        shot.mPosX = 400; shot.mPosY = 230;
        LobberProjectiles.Impact(shot, direct); Equal(80, direct.Damage, "overripe direct");
        Equal(26, other.Damage, "overripe first splash"); Equal(false, shot.mDead, "one physical bounce");
        LobberProjectiles.Impact(shot, other); Equal(106, other.Damage, "overripe second direct");
        Equal(true, shot.mDead, "overripe ends after second hit");
        board = new Board(); plant = Add(board, FusionPlant.CobPult); plant.mShootingCounter = 1;
        direct = Enemy(board); other = Enemy(board, x: 420); var third = Enemy(board, x: 440);
        LobberProjectiles.Fire(plant, 2, direct, false, 0); shot = board.Shots[^1]; shot.mPosX = 400; shot.mPosY = 230;
        LobberProjectiles.Impact(shot, direct); Equal(100, other.mButteredCounter, "three-target butter uses integer division");
        board = new Board(); plant = Add(board, FusionPlant.CobPult); plant.mShootingCounter = 1;
        direct = Enemy(board); other = Enemy(board, x: 420); third = Enemy(board, x: 440);
        Enemy(board, x: 450); Enemy(board, x: 460);
        LobberProjectiles.Fire(plant, 2, direct, false, 0); shot = board.Shots[^1]; shot.mPosX = 400; shot.mPosY = 230;
        LobberProjectiles.Impact(shot, direct); Equal(50, third.mButteredCounter, "butter minimum half second");
    }
    static void CabbageRicochets()
    {
        var board = new Board(); var plant = Add(board, FusionPlant.CabbageTriPult, 0); Add(board, FusionPlant.TriPot, 0);
        var direct = Enemy(board, 0); var adjacent = Enemy(board, 1, 420);
        TriLobbers.Initialize(plant); TriLobbers.Shoot(plant);
        Equal(3, board.Shots.Count, "three cabbage shots at edge");
        Equal(2, board.Shots.Count(s => s.mRow == 0), "cabbage edge compensation");
        var central = board.Shots[^1]; central.mPosX = 400; central.mPosY = 30;
        Equal(2, LobberProjectiles.Remaining(central), "central keeps two ricochets");
        LobberProjectiles.Impact(central, direct); Equal(48, direct.Damage, "tri pot cabbage damage");
        Equal(4, board.Shots.Count, "only one adjacent target selected");
        var rebound = board.Shots[^1]; Equal(1, rebound.mRow, "ricochet changes row");
        Equal(1, LobberProjectiles.Remaining(rebound), "central retains one more ricochet");
        rebound.mPosX = 420; rebound.mPosY = 130;
        LobberProjectiles.Impact(rebound, adjacent); Equal(5, board.Shots.Count, "central second ricochet");
        var last = board.Shots[^1]; Equal(0, LobberProjectiles.Remaining(last), "last uses regular cabbage");
        LobberProjectiles.Impact(last, direct); Equal(5, board.Shots.Count, "regular cabbage does not split");
        board.FailAllocation = true; LobberProjectiles.Impact(board.Shots[0], direct);
        Equal(5, board.Shots.Count, "failed rebound allocation does not duplicate damage source");
    }
    static void ReusedTargets()
    {
        var board = new Board(); var plant = Add(board, FusionPlant.CobPult); plant.mShootingCounter = 1;
        var direct = Enemy(board); var splash = Enemy(board, x: 430);
        LobberProjectiles.Fire(plant, 2, direct, false, 0); var shot = board.Shots[^1];
        shot.mPosX = 400; shot.mPosY = 230;
        direct.OnDamage = () => { direct.DataID += 10000; splash.DataID += 10000; };
        LobberProjectiles.Impact(shot, direct);
        Equal(0, direct.Buttered, "reused direct pool slot receives no butter");
        Equal(0, splash.Damage, "reused splash pool slot receives no damage");
        board = new Board(); plant = Add(board, FusionPlant.CabbageTriPult);
        direct = Enemy(board); Enemy(board, 1); TriLobbers.Initialize(plant); TriLobbers.Shoot(plant);
        shot = board.Shots[^1]; direct.OnDamage = () => shot.DataID += 10000;
        LobberProjectiles.Impact(shot, direct);
        Equal(3, board.Shots.Count, "reused projectile slot cannot create a ricochet");
    }
    static void ElementalContracts()
    {
        foreach (var (a, b, result) in new (FusionPlant, FusionPlant, FusionPlant)[] {
            (SeedType.Cabbagepult, SeedType.Iceshroom, FusionPlant.Coldslaw),
            (SeedType.Cabbagepult, SeedType.Jalapeno, FusionPlant.KimchiPult),
            (SeedType.Kernelpult, SeedType.Iceshroom, FusionPlant.Cobsicle),
            (SeedType.Cabbagepult, FusionPlant.CobPult, FusionPlant.SaladPult),
            (SeedType.Kernelpult, FusionPlant.OverripeMelon, FusionPlant.SaladPult),
            (SeedType.Melonpult, FusionPlant.TacoPult, FusionPlant.SaladPult) })
        {
            Equal(true, FusionRecipes.TryGet(a, b, out var direct), "elemental recipe");
            Equal(result, direct, "elemental result");
            Equal(true, FusionRecipes.TryGet(b, a, out var reverse), "elemental reverse recipe");
            Equal(result, reverse, "elemental reverse result");
        }
        var board = new Board(); var plant = Add(board, FusionPlant.Coldslaw);
        var zombie = Enemy(board); zombie.FreezePoints = 90;
        LobberFusions.Initialize(plant); ElementalLobbers.Fire(plant);
        var shot = board.Shots[^1]; shot.mVelZ = 4;
        ElementalLobbers.Impact(shot, zombie);
        Equal(160, zombie.Damage, "cabbage freezes before quadrupling");
        Equal(false, shot.mDead, "frozen cabbage continues");
        Equal(450, plant.mTargetX, "saved activity and recovery");
        Equal(-2f, shot.mVelZ, "first frozen rebound halves vertical speed");
        var other = Enemy(board, x: 500); other.mIceTrapCounter = 400; shot.mVelZ = 3;
        ElementalLobbers.Impact(shot, other);
        Equal(160, other.Damage, "later rebound retains original attack damage");
        Equal(-3f, shot.mVelZ, "later frozen rebound reflects without halving");
        Equal(450, plant.mTargetX, "no refresh during active or recovery state");
        ElementalLobbers.Impact(shot, other); Equal(160, other.Damage, "same target cannot hit twice consecutively");
        board.mPaused = true; LobberFusions.Tick(plant); Equal(450, plant.mTargetX, "pause preserves clock");
        board.mPaused = false; plant.mTargetX = 150; plant.mLaunchCounter = 100;
        for (int i = 0; i < 150; ++i) LobberFusions.Tick(plant);
        Equal(0, plant.mTargetX, "recovery finishes");
        board = new Board(); plant = Add(board, FusionPlant.Cobsicle); zombie = Enemy(board);
        zombie.FreezePoints = 90; ElementalLobbers.Fire(plant); shot = board.Shots[^1];
        ElementalLobbers.Impact(shot, zombie);
        Equal(20, zombie.Damage, "kernel adds freeze points after damage");
        Equal(400, zombie.mIceTrapCounter, "kernel completes freeze threshold");
        plant.mShootingCounter = 1; ElementalLobbers.Fire(plant); shot = board.Shots[^1];
        ElementalLobbers.Impact(shot, zombie); Equal(180, zombie.Damage, "frozen butter deals 40 times four");
        board = new Board(); plant = Add(board, FusionPlant.KimchiPult); zombie = Enemy(board);
        ElementalLobbers.Fire(plant); shot = board.Shots[^1]; shot.mPosX = 400;
        ElementalLobbers.Impact(shot, zombie); Equal(10, board.Shots.Count, "nine fire fragments");
        Equal(9, board.Shots.Count(s => ElementalLobbers.Family(s) == 3), "all fragments marked small");
        var small = board.Shots[^1]; ElementalLobbers.Impact(small, zombie);
        Equal(80, zombie.Damage, "fragment retains one third damage");
        Equal(10, board.Shots.Count, "fragments never recursively split");
        ElementalLobbers.Fire(plant); shot = board.Shots[^1];
        ElementalLobbers.Impact(shot, null); Equal(11, board.Shots.Count, "landing does not split");
        board = new Board(); plant = Add(board, FusionPlant.SaladPult); zombie = Enemy(board);
        var butterPot = Add(board, FusionPlant.ButterPot); Add(board, FusionPlant.MelonPot);
        LobberProjectiles.Fire(plant, 2, zombie, false, 0); shot = board.Shots[^1];
        butterPot.mDead = true; board.Rolls.Enqueue(2);
        LobberProjectiles.Impact(shot, zombie);
        Equal(3, board.Shots.Count, "salad creates exactly two successors");
        Equal(4, LobberProjectiles.Family(board.Shots[^1]), "saved fifty percent butter choice survives pot removal");
        Equal(-160, board.Shots[^1].mCobTargetRow, "salad descendants retain melon pot bonus");
        board = new Board(); plant = Add(board, FusionPlant.SaladPult); zombie = Enemy(board);
        LobberProjectiles.Fire(plant, 2, zombie, false, 0); shot = board.Shots[^1];
        LobberProjectiles.Impact(shot, null); Equal(1, board.Shots.Count, "salad landing never splits");
        LobberFusions.Initialize(plant); board.Rolls.Enqueue(25); board.Rolls.Enqueue(0);
        for (int i = 0; i <= 45; ++i) LobberFusions.Tick(plant);
        Equal(10, plant.mLaunchCounter, "salad repeat selected at attack event");
        Equal(2, board.Shots.Count, "salad emits once at 0.45 seconds");
        board.FailAllocation = true; ElementalLobbers.Fire(Add(board, FusionPlant.Coldslaw));
        Equal(2, board.Shots.Count, "failed allocation creates no phantom shot");
    }
    static void MoveToImpact(Projectile shot, Zombie target)
    {
        // These adapters invoke Impact directly instead of advancing native flight.
        var rect = target.GetZombieRect();
        shot.mPosX = (rect.xMin + rect.xMax) / 2;
        shot.mPosY = (rect.yMin + rect.yMax) / 2;
    }
    static void GarlicAndSummer()
    {
        foreach (var (a, b, result) in new (FusionPlant, FusionPlant, FusionPlant)[] {
            (SeedType.Cabbagepult, SeedType.Garlic, FusionPlant.GarbagePult),
            (SeedType.Kernelpult, SeedType.Garlic, FusionPlant.ClovePult),
            (SeedType.Melonpult, SeedType.Garlic, FusionPlant.GarlicPult),
            (SeedType.Melonpult, SeedType.Jalapeno, FusionPlant.SummerMelon) })
        {
            Equal(true, FusionRecipes.TryGet(a, b, out var direct), "seasoned recipe"); Equal(result, direct, "seasoned result");
            Equal(true, FusionRecipes.TryGet(b, a, out var reverse), "seasoned reverse"); Equal(result, reverse, "seasoned reverse result");
            Equal(result, FusionPlant.From(Add(new Board(), result)), "seasoned saved identity");
        }
        var board = new Board(); var plant = Add(board, FusionPlant.GarbagePult); var zombie = Enemy(board);
        ElementalLobbers.Fire(plant); var shot = board.Shots[^1]; shot.mVelZ = 4;
        ElementalLobbers.Impact(shot, zombie);
        Equal(false, shot.mDead, "first unpoisoned hit bounces"); Equal(40, zombie.Damage, "first garlic damage");
        Equal(-2f, shot.mVelZ, "first garlic bounce halves downward velocity");
        Equal(1, PoisonShooters.Level(zombie), "garlic added after damage");
        var second = Enemy(board); PoisonShooters.Levels[second.DataID] = 3;
        float marker = shot.mCobTargetX;
        float horizontal = shot.mVelX; shot.mVelZ = 3;
        ElementalLobbers.Impact(shot, second);
        Equal(horizontal, shot.mVelX, "later garlic bounce keeps horizontal velocity");
        Equal(-3f, shot.mVelZ, "later garlic bounce reflects vertical velocity");
        Equal(100, second.Damage, "twenty per old garlic point"); Equal(marker, shot.mCobTargetX, "poisoned hit refunds bounce");
        var last = Enemy(board); ElementalLobbers.Impact(shot, last);
        Equal(true, shot.mDead, "next unpoisoned hit consumes shot");
        plant = Add(board, FusionPlant.ClovePult); plant.mShootingCounter = 1;
        ElementalLobbers.Fire(plant); shot = board.Shots[^1]; zombie = Enemy(board);
        PoisonShooters.Levels[zombie.DataID] = 2;
        ElementalLobbers.Impact(shot, zombie);
        Equal(100, zombie.Damage, "garlic butter base and old-level bonus");
        Equal(400, zombie.mButteredCounter, "garlic butter four seconds"); Equal(1000, PoisonShooters.Durations[zombie.DataID], "garlic butter ten seconds poison");
        Equal(3, PoisonShooters.Level(zombie), "butter adds one point only");
        board = new Board(); plant = Add(board, FusionPlant.GarlicPult); zombie = Enemy(board); var splash = Enemy(board, 1);
        PoisonShooters.Levels[zombie.DataID] = 2; PoisonShooters.Levels[splash.DataID] = 3;
        ElementalLobbers.Fire(plant); shot = board.Shots[^1]; MoveToImpact(shot, zombie); ElementalLobbers.Impact(shot, zombie);
        Equal(140, zombie.Damage, "melon direct garlic bonus"); Equal(48, splash.Damage, "melon splash five per garlic point");
        Equal(4, PoisonShooters.Level(splash), "splash adds garlic");
        board = new Board(); plant = Add(board, FusionPlant.SummerMelon); zombie = Enemy(board); splash = Enemy(board, 1);
        ElementalLobbers.Fire(plant); shot = board.Shots[^1]; MoveToImpact(shot, zombie); ElementalLobbers.Impact(shot, zombie);
        Equal(120, zombie.Damage, "summer direct base"); Equal(39, splash.Damage, "summer splash");
        Equal(true, Overheat.Heated.Contains(zombie.DataID), "direct heated"); Equal(true, Overheat.Heated.Contains(splash.DataID), "splash heated");
        board.FailAllocation = true; int count = board.Shots.Count; ElementalLobbers.Fire(plant);
        Equal(count, board.Shots.Count, "seasoned failed allocation");
    }
    static void HypnoAndCattails()
    {
        foreach (var (a,b,result) in new (FusionPlant,FusionPlant,FusionPlant)[] {
            (SeedType.Cattail,SeedType.Iceshroom,FusionPlant.FrostCattail),
            (SeedType.Cattail,SeedType.Jalapeno,FusionPlant.FlameCattail),
            (SeedType.Melonpult,SeedType.Hypnoshroom,FusionPlant.HypnoMelon),
            (SeedType.Jalapeno,SeedType.Hypnoshroom,FusionPlant.HypnoPepper) }) {
            Equal(true,FusionRecipes.TryGet(a,b,out var resultA),"new recipe"); Equal(result,resultA,"new result");
            Equal(true,FusionRecipes.TryGet(b,a,out var resultB),"new reverse"); Equal(result,resultB,"new reverse result");
            Equal(result,FusionPlant.From(Add(new Board(),result)),"new saved identity");
        }
        var board=new Board(); var plant=Add(board,FusionPlant.FrostCattail);var zombie=Enemy(board,0);
        CattailFusions.Initialize(plant);
        for(int i=0;i<33;i++) CattailFusions.Tick(plant);
        Equal(0,board.Shots.Count,"no shot before original event");
        CattailFusions.Tick(plant); Equal(1,board.Shots.Count,"first shot at tick 34, other lane");
        int phase=plant.mStartRow; board.mPaused=true;CattailFusions.Tick(plant);Equal(phase,plant.mStartRow,"pause retains phase");board.mPaused=false;
        for(int i=34;i<67;i++)CattailFusions.Tick(plant);
        Equal(2,board.Shots.Count,"second shot at tick 67");
        var shot=board.Shots[0];Equal(1,CattailFusions.Family(shot),"saved ice marker");
        foreach(var (chill,freeze,expected) in new[]{(0,0,40),(100,0,80),(100,400,160)}) {
            zombie.mChilledCounter=chill;zombie.mIceTrapCounter=freeze;
            CattailFusions.Current=new(shot,shot.DataID,zombie,zombie.DataID);
            int damage=20;CattailFusions.Damage(zombie,ref damage);Equal(expected,damage,"cold status multiplier");
            int nested=7;CattailFusions.Damage(zombie,ref nested);Equal(7,nested,"nested damage not overwritten");
        }
        plant=Add(board,FusionPlant.FlameCattail);CattailFusions.Firing=plant;
        shot=board.AddProjectile(0,0,0,2,ProjectileType.Spike)!;CattailFusions.Firing=null;
        Overheat.Apply(zombie);CattailFusions.Current=new(shot,shot.DataID,zombie,zombie.DataID);
        int hot=20;CattailFusions.Damage(zombie,ref hot);Equal(120,hot,"threefold enflamed base");
        CattailFusions.Current=new(shot,shot.DataID,zombie,zombie.DataID);++shot.DataID;
        hot=17;CattailFusions.Damage(zombie,ref hot);Equal(17,hot,"recycled shot rejected");CattailFusions.Current=null;
        board=new Board();plant=Add(board,FusionPlant.HypnoMelon);zombie=Enemy(board);var splash=Enemy(board,1);
        zombie.mBodyHealth=1499;splash.mBodyHealth=1499;board.Rolls.Enqueue(0);board.Rolls.Enqueue(0);
        ElementalLobbers.Fire(plant);shot=board.Shots[^1];
        MoveToImpact(shot,zombie);
        ElementalLobbers.Impact(shot,zombie);
        Equal(120,zombie.Damage,"hypno direct damage");Equal(39,splash.Damage,"hypno splash damage");
        Equal(true,zombie.mMindControlled,"direct hypnosis");Equal(true,splash.mMindControlled,"splash hypnosis");
        zombie=Enemy(board);zombie.mBodyHealth=1500;board.Rolls.Enqueue(0);HypnoPlants.MelonHit(zombie);
        Equal(false,zombie.mMindControlled,"strict half-health threshold");
        board.FailAllocation=true;Equal(true,HypnoPlants.Bite(plant,zombie),"bite handled");Equal(false,plant.mDead,"failed replacement retains plant");
        board.FailAllocation=false;HypnoPlants.Bite(plant,zombie);Equal(true,plant.mDead,"bite consumes only after replacement");
        Equal(SeedType.Melonpult,board.m_plants[^1].mSeedType,"ordinary melon restored");Equal(true,zombie.mMindControlled,"bite hypnosis");
        board=new Board();plant=Add(board,FusionPlant.HypnoPepper);zombie=Enemy(board);zombie.mBodyHealth=899;
        var armored=Enemy(board);armored.mBodyHealth=800;armored.mHelmHealth=100;
        var immune=Enemy(board);immune.mBodyHealth=100;immune.FailControl=true;
        var other=Enemy(board,1);HypnoPlants.Initialize(plant);
        for(int i=0;i<83;i++)HypnoPlants.TickPepper(plant);Equal(false,plant.mDead,"source 0.8 animation speed");
        HypnoPlants.TickPepper(plant);Equal(true,plant.mDead,"pepper commits once at tick 84");
        Equal(true,zombie.mMindControlled,"below 900 hypnotized");Equal(0,zombie.Damage,"successful hypnosis takes no damage");
        Equal(false,armored.mMindControlled,"helmet counts toward 900");Equal(1800,armored.Damage,"threshold fallback");
        Equal(1800,immune.Damage,"failed hypnosis still takes ash damage");Equal(0,other.Damage,"other lanes unaffected");
        HypnoPlants.Explode(plant);Equal(1800,immune.Damage,"reentry cannot repeat explosion");
    }
    public static void Main()
    {
        BoltPultCases.Run();
        SolarLobberCases();
        var blubberBoard=new Board();var blubber=Add(blubberBoard,FusionPlant.BlubberShroom);
        var butterTarget=Enemy(blubberBoard);
        LobberProjectiles.FireBlubber(blubber,butterTarget);
        var blubberShot=blubberBoard.Shots.Last();
        Equal(14,LobberProjectiles.Family(blubberShot),"blubber butter saved family");
        blubber.mDead=true;LobberProjectiles.Impact(blubberShot,butterTarget);
        Equal(40,butterTarget.Damage,"butter survives source plant death");
        Equal(400,butterTarget.mButteredCounter,"four-second blubber butter");
        LobberProjectiles.Impact(blubberShot,butterTarget);
        Equal(40,butterTarget.Damage,"blubber butter cannot impact twice");

        var kernelBoard=new Board();var kernel=Add(kernelBoard,FusionPlant.KernelMorph);
        LobberFusions.Initialize(kernel);var kernelTarget=Enemy(kernelBoard);
        kernel.mShootingCounter=0;LobberProjectiles.Fire(kernel,2,kernelTarget,false,0);
        var kernelShot=kernelBoard.Shots.Last();Equal(10,LobberProjectiles.Family(kernelShot),"magnetic kernel saved identity");
        LobberProjectiles.Impact(kernelShot,kernelTarget);Equal(60,kernelTarget.Damage,"magnetic kernel damage");
        kernel.mShootingCounter=1;LobberProjectiles.Fire(kernel,2,kernelTarget,false,0);
        var butterShot=kernelBoard.Shots.Last();Equal(11,LobberProjectiles.Family(butterShot),"magnetic butter saved identity");
        LobberProjectiles.Impact(butterShot,kernelTarget);Equal(140,kernelTarget.Damage,"magnetic butter damage");
        Equal(400,kernelTarget.mButteredCounter,"four second magnetic butter");
        var ironBoard=new Board();var iron=Add(ironBoard,FusionPlant.MetalPult);LobberFusions.Initialize(iron);
        Equal(2000,iron.mPlantMaxHealth,"iron melon toughness");
        var ironTarget=Enemy(ironBoard);ironTarget.mHelmHealth=100;
        var ironSplash=Enemy(ironBoard,x:430);ironSplash.mHelmHealth=100;
        LobberProjectiles.Fire(iron,2,ironTarget,false,0);var ironShot=ironBoard.Shots.Single();
        ironShot.mPosX=400;ironShot.mPosY=230;Equal(12,LobberProjectiles.Family(ironShot),"iron melon saved family");
        LobberProjectiles.Impact(ironShot,ironTarget);Equal(400,ironTarget.Damage,"iron melon direct armor bonus");
        Equal(132,ironSplash.Damage,"iron melon splash recipient armor bonus");
        var metalBoard=new Board();var morph=Add(metalBoard,FusionPlant.MelonMorph);
        LobberFusions.Initialize(morph);
        var cone=new Zombie {mBoard=metalBoard,mHelmType=HelmType.TrafficCone,mHelmHealth=100};
        var door=new Zombie {mBoard=metalBoard,mShieldType=ShieldType.Door,mShieldHealth=100,X=420};
        metalBoard.m_zombies.Add(cone);metalBoard.m_zombies.Add(door);
        LobberProjectiles.Fire(morph,morph.mRow,cone,false,0);
        var metalShot=metalBoard.Shots[0];metalShot.mPosX=400;metalShot.mPosY=230;
        Equal(8,LobberProjectiles.Family(metalShot),"saved metal melon family");
        LobberProjectiles.Impact(metalShot,cone);
        Equal(240,cone.Damage,"cone doubles direct damage");
        Equal(39,door.Damage,"door alone does not double original .33 splash");
        LobberProjectiles.Impact(metalShot,cone);Equal(240,cone.Damage,"no duplicate impact");
        DamageCabbage(); ShellMelon(); SquashLobberCases.Run(); HypnoAndCattails(); Recipes(); Pots(); Lobbers(); MixedLobbers(); BouncingLobbers(); MixedMelons(); CabbageRicochets(); ReusedTargets(); ElementalContracts(); GarlicAndSummer();
        Console.WriteLine("Shooting pot and lobber contracts passed.");
    }

    static void ShellMelon()
    {
    var board = new Board();
    var shell = new Plant { mBoard = board, mSeedType = SeedType.Pumpkinshell, mImitaterType = SeedType.Melonpult };
    var shooter = new Plant { mBoard = board, mSeedType = SeedType.Cabbagepult };
    LobberProjectiles.FireShell(shell, shooter, null);
    var shot = board.Shots.Last();
    Equal(9, LobberProjectiles.Family(shot), "Melon Shell saved marker");
    Equal(80, shot.mCobTargetRow, "Melon Shell damage");
}

// Doom cabbage retains damage after its producer disappears.
static void DamageCabbage()
{
    var board = new Board();
    var plant = new Plant { mBoard=board,mSeedType=SeedType.Cabbagepult,mImitaterType=SeedType.Doomshroom };
    LobberFusions.Initialize(plant);
    Equal(1500,plant.mLaunchRate,"Damage-pult interval");
    var direct = new Zombie {mBoard=board,mRow=2,X=400};
    var nearby = new Zombie {mBoard=board,mRow=2,X=420};
    var far = new Zombie {mBoard=board,mRow=2,X=700};
    board.m_zombies.AddRange(new[]{direct,nearby,far});
    EmberEffects.Irradiated.Add(direct);
    LobberProjectiles.Fire(plant,2,direct,false,0);
    var shot=board.Shots.Last();shot.mPosX=400;shot.mPosY=230;
    plant.mDead=true;LobberProjectiles.Impact(shot,direct);
    Equal(1800,direct.Damage,"irradiated direct target is excluded from splash");
    Equal(600,nearby.Damage,"doom cabbage constant splash");Equal(0,far.Damage,"doom cabbage radius");
    Equal(true,shot.mDead,"doom cabbage consumed once");
    LobberProjectiles.Impact(shot,direct);Equal(1800,direct.Damage,"dead projectile cannot hit twice");
    EmberEffects.Irradiated.Clear();
}

    static void SolarLobberCases()
    {
        Equal(2,LobberProjectiles.SolarHits(0),"solar minimum includes first impact and one bounce");
        Equal(2,LobberProjectiles.SolarHits(200),"original integer formula at 200 Sun");
        Equal(3,LobberProjectiles.SolarHits(400),"third hit at 400 Sun");
        Equal(6,LobberProjectiles.SolarHits(50000),"solar hit cap");
        var board=new Board();var plant=Add(board,FusionPlant.SolarPult);
        var target=new Zombie{mBoard=board,X=400};board.m_zombies.Add(target);
        board.mSunMoney[0].Amount=400;
        LobberProjectiles.Fire(plant,2,target,false,0);var shot=board.Shots.Last();
        Equal(15,LobberProjectiles.Family(shot),"solar projectile identity");
        float z=shot.mPosZ,v=shot.mVelZ;
        for(int tick=0;tick<PuffLobbers.FlightTicks;++tick){v+=shot.mAccZ;z+=v;}
        Equal(true,Math.Abs(z+35)<.01f,"planned cabbage arc reaches native impact height");
        SolarShooter.Sun=0;LobberProjectiles.Impact(shot,target);
        Equal(50,target.Damage,"source solar cabbage damage");Equal(5,SolarShooter.Sun,"five Sun per impact");
        LobberProjectiles.Impact(shot,target);Equal(50,target.Damage,"no repeated collision with same occupant");
        LobberProjectiles.Impact(shot,null!);Equal(10,SolarShooter.Sun,"five Sun on landing");
        Equal(true,shot.mDead,"landing ends solar cabbage");
    }

}
