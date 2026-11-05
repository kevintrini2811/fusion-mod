using System.Reflection;
using FusionMod;
using Il2CppReloaded.Gameplay;

internal static class PumpkinCases
{
    private static void Check(bool value, string message)
    { if (!value) throw new Exception(message); }
    private static Plant Plant(Board board, FusionPlant identity)
    {
        var plant = new Plant { mBoard = board, mSeedType = identity.NativeType, mImitaterType = identity.Marker };
        board.m_plants.Add(plant);
        PumpkinFusions.Initialize(plant);
        return plant;
    }
    internal static void Run()
    {
        foreach (var (ingredient, result) in new[] {
            (SeedType.Peashooter, FusionPlant.PumpkinPod), (SeedType.Melonpult, FusionPlant.MelonShell),
            (SeedType.Iceshroom, FusionPlant.CryoPumpkin), (SeedType.Jalapeno, FusionPlant.PyroPumpkin),
            (SeedType.Starfruit, FusionPlant.StarryPumpkin), (SeedType.Cactus, FusionPlant.ThornyPumpkin),
            (SeedType.Puffshroom, FusionPlant.PuffKin), (SeedType.Fumeshroom, FusionPlant.FumingPumpkin) })
        {
            Check(FusionRecipes.TryGet(SeedType.Pumpkinshell, ingredient, out var a) && a == result, "Pumpkin recipe");
            Check(FusionRecipes.TryGet(ingredient, SeedType.Pumpkinshell, out var b) && b == result, "Reverse pumpkin recipe");
            var saved = Plant(new(), result);
            Check(FusionPlant.From(saved) == result, "Pumpkin save identity");
        }
        foreach (var identity in new[] { FusionPlant.PumpkinPod, FusionPlant.CryoPumpkin })
        {
            var plant = Plant(new(), identity); plant.mLaunchCounter = 500; plant.mTargetX = 3;
            PumpkinFusions.Fertilize(plant);
            Check(plant.mLaunchCounter == 50 && plant.mTargetX == 3, "Fertilizer caps timer, preserves charge");
        }
        var hot = Plant(new(), FusionPlant.PyroPumpkin); hot.mTargetX = 1800;
        PumpkinFusions.Fertilize(hot); Check(hot.mTargetX == 1800, "Fertilizer preserves heat");
        var feedBoard = new Board();
        var seaShell = Plant(feedBoard, FusionPlant.SeaShell);
        seaShell.mPlantHealth = 1000;
        var enclosed = Plant(feedBoard, SeedType.Peashooter);
        Check(SeaFusions.FeedTarget(feedBoard, 0, 0) == seaShell, "Sea-shell receives feed before the interior");
        Check(SeaFusions.CanFeed(seaShell, SeedType.Seashroom), "Sea-shell accepts normal Sea-shroom");
        SeaFusions.Feed(seaShell);
        Check(seaShell.mPlantHealth == 2333 && !enclosed.mDead, "Feeding repairs one third without replacing interior");
        seaShell.mPlantHealth = 3900; SeaFusions.Feed(seaShell);
        Check(seaShell.mPlantHealth == 4000, "Repair caps at max health");
        feedBoard.mPaused = true;
        Check(!SeaFusions.CanFeed(seaShell, SeedType.Seashroom), "Pause blocks feeding");

        var lightBoard = new Board();
        var lantern = Plant(lightBoard, FusionPlant.JackOPlantern); lantern.mPlantHealth = 3000;
        lantern.mLaunchCounter = 1; PumpkinFusions.Tick(lantern);
        Check(lantern.mPlantHealth == 3000, "Own level-one light grants no healing");
        var plantern = Plant(lightBoard, SeedType.Plantern); plantern.mPlantCol = 1;
        lantern.mLaunchCounter = 1; PumpkinFusions.Tick(lantern);
        Check(lantern.mPlantHealth == 3150, "Overlapping light adds one healing level");
        plantern.Carried = true;
        Check(PumpkinFusions.LightLevel(lantern) == 1, "Carried source contributes no light");
        PumpkinFusions.Fertilize(lantern);
        Check(lantern.mLaunchCounter == 50, "Fertilizer caps lantern attribute timer");

        var windBoard = new Board(); var wind = Plant(windBoard, FusionPlant.WindyPumpkin);
        wind.mLaunchCounter = 1; PumpkinFusions.Tick(wind);
        Check(windBoard.GetPlantsOnLawn(0, 0).NormalPlant?.mSeedType == SeedType.Blover, "Blover uses empty interior");
        wind.mLaunchCounter = 1; PumpkinFusions.Tick(wind);
        Check(windBoard.GetPlantsOnLawn(1, 0).NormalPlant?.mSeedType == SeedType.Blover, "Occupied interior scans forward");
        wind.mLaunchCounter = 2000; PumpkinFusions.Fertilize(wind);
        Check(wind.mLaunchCounter == 50, "Fertilizer caps Blover timer");
        wind.mBoard.mPaused = true; PumpkinFusions.Tick(wind);
        Check(wind.mLaunchCounter == 50, "Pause preserves Blover timer");

        var crushBoard = new Board(); var spiky = Plant(crushBoard, FusionPlant.SpikyPumpkin);
        var weed = Plant(crushBoard, SeedType.Spikeweed);
        Check(PumpkinFusions.AbsorbCrush(weed) && spiky.mPlantHealth == 3500 && !weed.mDead,
            "Shell pays 500 for enclosed weed");
        Check(PumpkinFusions.AbsorbCrush(spiky) && spiky.mPlantHealth == 3500, "Same tick cannot double-charge shell");
        ++crushBoard.mMainCounter; PumpkinFusions.AbsorbCrush(spiky);
        Check(spiky.mPlantHealth == 3000, "Next tick permits another crush");
        var ordinaryInterior = Plant(crushBoard, SeedType.Peashooter);
        Check(!PumpkinFusions.AbsorbCrush(ordinaryInterior), "Ordinary interiors retain native crushing");
        spiky.mPlantHealth = 400; ++crushBoard.mMainCounter;
        PumpkinFusions.AbsorbCrush(weed);
        Check(spiky.mDead && !weed.mDead, "Final shell charge protects weed before shell dies");

        var nutBoard = new Board();
        var nutShell = Plant(nutBoard, FusionPlant.NutShell);
        Check(nutShell.mPlantMaxHealth == 8000 && nutShell.mTargetX == 3, "Nut Shell HP and saved card budget");
        var damaged = Plant(nutBoard, FusionPlant.ThornyPumpkin); damaged.mPlantCol = 1; damaged.mPlantHealth = 3500;
        var damaged2 = Plant(nutBoard, FusionPlant.PuffKin); damaged2.mRow = 1; damaged2.mPlantHealth = 3000;
        nutShell.mLaunchCounter = 1; PumpkinFusions.Tick(nutShell);
        Check(damaged.mPlantHealth == 4000 && damaged2.mPlantHealth == 3500 && nutShell.mTargetX == 3,
            "One 1000 HP pool spreads across other shells within 3x3 without spending a card");
        damaged2.mPlantHealth = damaged2.mPlantMaxHealth;
        for (int i = 0; i < 4; ++i) { nutShell.mLaunchCounter = 1; PumpkinFusions.Tick(nutShell); }
        Check(nutBoard.Cards == 3 && nutShell.mTargetX == 0, "At most three usable Wall-nut cards");
        damaged.mPlantHealth = 3000; nutShell.mLaunchCounter = 1; PumpkinFusions.Tick(nutShell);
        Check(damaged.mPlantHealth == 4000, "Healing continues after card budget is exhausted");
        nutShell.mLaunchCounter = 777; PumpkinFusions.Fertilize(nutShell);
        Check(nutShell.mLaunchCounter == 777 && nutShell.mTargetX == 0, "Fertilizer preserves kernel growth and spent card budget");
        nutShell.mBoard.mPaused = true; PumpkinFusions.Tick(nutShell);
        Check(nutShell.mLaunchCounter == 777, "Pause preserves kernel growth");
        var retryBoard = new Board { FailAllocation = true };
        var retry = Plant(retryBoard, FusionPlant.NutShell); retry.mLaunchCounter = 1; PumpkinFusions.Tick(retry);
        Check(retry.mTargetX == 3 && retry.mLaunchCounter == 0, "Failed card allocation preserves ripe kernel and budget");
        retryBoard.FailAllocation = false; PumpkinFusions.Tick(retry);
        Check(retry.mTargetX == 2 && retryBoard.Cards == 1, "Retry produces exactly one usable card");

        var cherryBoard = new Board(); var cherry = Plant(cherryBoard, FusionPlant.CherryPumpkin);
        cherry.mPlantHealth = 3500;
        PumpkinFusions.Cherry(cherryBoard, 40, 40, 0, 80);
        Check(cherry.mPlantHealth == 3700, "Cherry explosion heals 200");
        var explosive = new Projectile { mBoard = cherryBoard, mRow = 0, mPosX = 17, mCobTargetX = -41010 };
        Check(PumpkinFusions.AbsorbCherry(explosive) && explosive.mDead && cherry.mPlantHealth == 3900,
            "Explosive cherry is absorbed before splash and heals once");
        Check(!PumpkinFusions.AbsorbCherry(explosive), "Dead cherry cannot heal twice");
        var ordinary = new Projectile { mBoard = cherryBoard, mRow = 0, mPosX = 17 };
        Check(!PumpkinFusions.AbsorbCherry(ordinary), "Ordinary peas pass through Cherry Pumpkin");
        Check(Plant(new(), FusionPlant.ScarelessPumpkin).mPlantMaxHealth == 2000, "Scareless Pumpkin has 2000 HP");

        var board = new Board();
        var shell = Plant(board, FusionPlant.PuffKin);
        Check(shell.mPlantHealth == 1000 && shell.mPlantMaxHealth == 4000, "Puff-kin starts at quarter HP");
        for (int i = 0; i < 299; ++i) PumpkinFusions.Tick(shell);
        Check(shell.mPlantHealth == 1000, "No early regeneration");
        PumpkinFusions.Tick(shell);
        Check(shell.mPlantHealth == 1050, "Regenerate 50 every three seconds");
        shell.mPlantHealth = 1333;
        var hit = PumpkinFusions.Capture(shell)!; shell.mPlantHealth -= 20; hit.Apply();
        Check(shell.mPlantHealth == 1313, "No armor below one third");
        shell.mPlantHealth = 1334;
        hit = PumpkinFusions.Capture(shell)!; shell.mPlantHealth -= 20; hit.Apply();
        Check(shell.mPlantHealth == 1324, "Armor uses pre-hit health");
        var inner = Plant(board, SeedType.Peashooter);
        shell.mPlantHealth = 2666;
        Check(PumpkinFusions.Capture(inner) == null, "No interception below two thirds");
        shell.mPlantHealth = 2667;
        hit = PumpkinFusions.Capture(inner)!;
        inner.mPlantHealth -= 8000; hit.Apply();
        Check(inner.mPlantHealth == 300 && shell.mDead, "Lethal interception preserves inner plant and spends shell HP");
        hit.Apply();
        Check(inner.mPlantHealth == 300, "Nested death/postfix does not apply twice");
        shell = Plant(new(), FusionPlant.PuffKin);
        shell.mLaunchCounter = 200; PumpkinFusions.Fertilize(shell);
        Check(shell.mLaunchCounter == 50, "Fertilizer caps positive attribute timer");
        shell.mLaunchCounter = 12; PumpkinFusions.Fertilize(shell);
        Check(shell.mLaunchCounter == 12, "Fertilizer never lengthens timer");
        shell.mBoard.mPaused = true; PumpkinFusions.Tick(shell);
        Check(shell.mLaunchCounter == 12, "Paused regeneration");
        shell.mBoard.mPaused = false; shell.Carried = true; PumpkinFusions.Tick(shell);
        Check(shell.mLaunchCounter == 12, "Carrying suspends regeneration");
        shell.Carried = false;
        hit = PumpkinFusions.Capture(shell)!;
        ++shell.DataID; shell.mPlantHealth -= 20; hit.Apply();
        Check(shell.mPlantHealth == 980, "Recycled plant rejects stale hit");

        board = new(); var fume = Plant(board, FusionPlant.FumingPumpkin); fume.mRow = 2; fume.mPlantCol = 2;
        var target = new Zombie { mBoard = board, mRow = 0, X = 160 }; board.m_zombies.Add(target);
        var wrongColumn = new Zombie { mBoard = board, mRow = 2, X = 240 }; board.m_zombies.Add(wrongColumn);
        for (int i = 0; i <= 63; ++i) PumpkinFusions.Tick(fume);
        Check(target.Damage == 60 && wrongColumn.Damage == 0, "Three vertical penetrating pulses, own column only");
        var second = new Zombie { mBoard = board, mRow = 4, X = 160 }; board.m_zombies.Add(second);
        fume.mStartRow = 33; // Saved phase immediately before the first hit.
        PumpkinFusions.Tick(fume); PumpkinFusions.Tick(fume);
        Check(second.Damage == 20, "Loaded phase emits only pending pulse");

        board = new(); shell = Plant(board, FusionPlant.ThornyPumpkin);
        var eater = new Zombie { mBoard = board };
        var state = ((List<PumpkinFusions.Hit>?)null, (Plant?)shell, shell.DataID, shell.mPlantHealth);
        shell.mPlantHealth = 2;
        state = (null, shell, shell.DataID, shell.mPlantHealth);
        shell.mPlantHealth -= 4;
        typeof(PumpkinBitePatch).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, new object?[] { eater, state });
        Check(eater.Damage == 4, "Lethal native partial bite reflects its actual four HP loss");
        board = new(); shell = Plant(board, FusionPlant.StinkyPumpkin); eater = new Zombie { mBoard = board };
        state = (null, shell, shell.DataID, shell.mPlantHealth); shell.mPlantHealth -= 10;
        typeof(PumpkinBitePatch).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, new object?[] { eater, state });
        Check(eater.Poison == 1 && eater.mYuckyFace && eater.Damage == 0,
            "Stinky bite adds poison and starts native delayed garlic lane change");

    }
}
