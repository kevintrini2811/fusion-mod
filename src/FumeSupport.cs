using HarmonyLib;
using Il2CppReloaded.Gameplay;
using LawnGlove;

namespace FusionMod;

// Saved native counters own energy, production and action phases. The source
// animations are visual only; no animation event can create a second attack.
internal static class FumeSupport
{
    internal const float Unit = 80f / 1.35f;
    internal static bool Reactive(FusionPlant identity) => identity == FusionPlant.BlubberShroom
        || identity == FusionPlant.BushShroom;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (identity != FusionPlant.RayShroom && identity != FusionPlant.SunriseShroom
            && !Reactive(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.BushShroom ? 1000 : 300;
        plant.mTargetX = plant.mTargetY = plant.mStartRow = plant.mShootingCounter = 0;
        plant.mLaunchCounter = identity == FusionPlant.SunriseShroom ? 400 + plant.mBoard.Rand(301) : 0;
    }

    internal static int RayDamage(int columns, int light) =>
        (int)(10 + 80 * Math.Clamp(1 - columns / (light >= 3 ? 10f : 5f), 0, 1)) + 10 * light / 2;

    internal static bool BlubberTarget(Plant plant, Zombie zombie)
    {
        if (Math.Abs(plant.mRow - zombie.mRow) > 1
            || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)) return false;
        var rect = zombie.GetZombieRect();
        float dx = rect.center.x - (plant.mX + plant.mWidth / 2f);
        float dy = rect.center.y - (plant.mY + plant.mHeight / 2f);
        return dx * dx + dy * dy <= 9 * Unit * Unit;
    }

    internal static bool Trigger(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.BlubberShroom || !FumeFusions.Active(plant)) return false;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && BlubberTarget(plant, zombie))
            {
                if (plant.mStartRow == 0) plant.mStartRow = 50;
                return true;
            }
        return false;
    }

    internal static void Butter(Plant plant)
    {
        var board = plant.mBoard;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && BlubberTarget(plant, zombie))
                targets.Add((zombie, zombie.DataID));
        if (targets.Count == 0) return;
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !BlubberTarget(plant, zombie)) continue;
            LobberProjectiles.FireBlubber(plant, zombie);
        }
        // CornFume pays once per nonempty animation event, not per target.
        plant.mPlantMaxHealth = Math.Max(1, plant.mPlantMaxHealth - 30);
        plant.mPlantHealth -= 30;
        if (plant.mPlantHealth <= 0) plant.Die();
    }

    internal static void TickBlubber(Plant plant)
    {
        if (plant.mStartRow <= 0) return;
        // Original 0.5-second clip invokes AnimShoot at its event time.
        if (plant.mStartRow == 25) Butter(plant);
        if (!plant.mDead && plant.mStartRow > 0) --plant.mStartRow;
    }

    internal static void TickBush(Plant plant)
    {
        if (!FumeFusions.Active(plant)) return;
        if (!FusionTime.SourceStep(plant, typeof(FumeSupport))) return;
        if (plant.mShootingCounter > 0) --plant.mShootingCounter;
        if (plant.mTargetY > 0 && --plant.mTargetY > 0) return;
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        var targets = new List<(Zombie Zombie, uint Id)>();
        bool contact = false;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || !UmbrellaShell.CanHit(plant, zombie)) continue;
            var rect = zombie.GetZombieRect();
            if (zombie.mRow == plant.mRow && rect.xMax >= x - 82 && rect.xMin <= x + 107) contact = true;
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= 4 * Unit * Unit) targets.Add((zombie, zombie.DataID));
        }
        if (!contact) return;
        plant.mTargetY = 100;
        plant.mShootingCounter = 84;
        int count = 0;
        uint plantId = plant.DataID;
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !UmbrellaShell.CanHit(plant, zombie)) continue;
            ++count;
            Energy(plant, 40);
            zombie.TakeDamage(40, DamageFlags.BypassesShield);
            if (zombie.DataID != id || !UmbrellaShell.CanHit(plant, zombie)) continue;
            if (zombie.mIsEating) zombie.StopEating();
            zombie.mPosX = Math.Min(850, zombie.mPosX
                + (PotBoosts.HasSupport(plant, FusionPlant.UmbrellaPot) ? 3 : 2) * Unit);
            zombie.mX = (int)zombie.mPosX;
            zombie.UpdateZombiePosition();
        }
        if (count == 0 || plant.DataID != plantId || !FumeFusions.Active(plant)) return;
        // Source marks these self-hits: damage reduction applies, energy does not.
        plant.mPlantHealth -= count == 1 ? 100 : count <= 4 ? 150 : 250;
        if (plant.mPlantHealth <= 0) plant.Die();
    }

    internal static void Energy(Plant plant, int amount)
    {
        if (amount > 0) plant.mTargetX = (int)Math.Min(int.MaxValue, (long)Math.Max(0, plant.mTargetX) + amount);
    }

    internal static void Fertilize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.SunriseShroom && plant.mLaunchCounter > 50)
            plant.mLaunchCounter = 50;
    }

    internal static void Produce(Plant plant)
    {
        void Sun()
        {
            var coin = plant.mBoard.AddCoin(plant.mX, plant.mY, CoinType.Sun, CoinMotion.FromPlant);
            // Source CreateSun uses the non-bankable production category.
            if (coin != null) coin.mUsableSeedType = SeedType.Imitater;
        }
        Sun();
        if (plant.mTargetX <= 25) return;
        while (plant.mTargetX >= 25)
        {
            plant.mTargetX -= 25;
            Sun();
        }
    }

    internal static void TickSunrise(Plant plant)
    {
        if (!FumeFusions.Active(plant)
            || plant.mApp.GameMode is >= GameMode.PuzzleIZombie1 and <= GameMode.PuzzleIZombieEndless) return;
        if (!FusionTime.SourceStep(plant, typeof(FumeSupport))) return;
        if (plant.mShootingCounter > 0 && --plant.mShootingCounter == 50) Produce(plant);
        if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
        plant.mLaunchCounter = 1900 + plant.mBoard.Rand(201);
        plant.mShootingCounter = 100;
    }

    // CoinSun chooses a bank once at creation. Use the first simulation tick,
    // after custom five-/fifteen-sun values have been attached by their owners.
    // Sun coins never use seed-packet type or drop count for their native AI.
    // Both fields serialize, including the complete destination DataID.
    internal static bool Sun(Coin coin)
    {
        if (coin.mDead || !coin.IsSun() || coin.mBoard == null) return false;
        var board = coin.mBoard;
        if (board.mPaused || board.mApp.GameScene != GameScenes.Playing) return coin.mUsableSeedType == SeedType.Fumeshroom;
        if (coin.mUsableSeedType != SeedType.Fumeshroom)
        {
            if (coin.mUsableSeedType == SeedType.Imitater || coin.mIsBeingCollected) return false;
            coin.mUsableSeedType = SeedType.Imitater;
            Plant? bank = null;
            int col = board.PixelToGridX((int)coin.mPosX, (int)coin.mPosY);
            int row = board.PixelToGridY((int)coin.mPosX, (int)coin.mPosY);
            for (int i = 0; i < board.m_plants.Count; ++i)
            {
                var plant = board.m_plants[i];
                if (plant != null && FusionPlant.From(plant) == FusionPlant.SunriseShroom
                    && FumeFusions.Active(plant) && Math.Abs(plant.mPlantCol - col) <= 2
                    && Math.Abs(plant.mRow - row) <= 2) bank = plant;
            }
            if (bank == null) return false;
            coin.mUsableSeedType = SeedType.Fumeshroom;
            coin.mTimesDropped = unchecked((int)bank.DataID);
            coin.mDisappearCounter = coin.GetSunValue();
        }
        Plant? owner = null;
        for (int i = 0; i < board.m_plants.Count; ++i)
            if (board.m_plants[i] is { } plant && plant.DataID == unchecked((uint)coin.mTimesDropped)
                && !plant.mDead && FusionPlant.From(plant) == FusionPlant.SunriseShroom) { owner = plant; break; }
        if (owner == null) { coin.Die(); return true; }
        if (!FumeFusions.Active(owner)) return true;
        float x = owner.mX + owner.mWidth / 2f, y = owner.mY + owner.mHeight / 2f;
        float dx = x - coin.mPosX, dy = y - coin.mPosY;
        if (dx * dx + dy * dy <= 18 * 18)
        {
            int value = coin.mDisappearCounter;
            uint id = coin.DataID;
            coin.Die();
            if (coin.mDead && coin.DataID == id) Energy(owner, (int)(value * 1.3f));
        }
        else
        {
            float blend = 1 - MathF.Pow(.95f, (float)(FusionTime.StepSeconds(board) / .01));
            coin.mPosX += dx * blend;
            coin.mPosY += dy * blend;
            coin.mX = (int)coin.mPosX;
            coin.mY = (int)coin.mPosY;
        }
        return true;
    }

    internal static bool Click(Plant plant, int clicks, int player)
    {
        if (FusionPlant.From(plant) != FusionPlant.BlubberShroom) return false;
        if (clicks >= 0 && player == 0 && plant.mBoard.mCursorObject.CursorType == CursorType.Normal
            && LawnGloveMod.GetCarriedPlant(plant.mBoard) == null) Trigger(plant);
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class FumeSupportInitializePatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(Plant __instance) => FumeSupport.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class FumeSupportAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.SunriseShroom) return true;
        FumeSupport.TickSunrise(__instance);
        return false;
    }
    private static void Postfix(Plant __instance)
    {
        if (FusionPlant.From(__instance) == FusionPlant.BushShroom)
        {
            FumeFusions.Tick(__instance);
            FumeSupport.TickBush(__instance);
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class FumeSupportShootingPatch
{
    private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.BushShroom
        && FusionPlant.From(__instance) != FusionPlant.SunriseShroom;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class SunriseFirePatch
{ private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.SunriseShroom; }

[HarmonyPatch(typeof(Plant), nameof(Plant.MouseDown))]
internal static class BlubberClickPatch
{
    private static bool Prefix(Plant __instance, int theClickCount, int playerIndex) =>
        !FumeSupport.Click(__instance, theClickCount, playerIndex);
}

[HarmonyPatch(typeof(Coin), nameof(Coin.Update))]
internal static class SunriseCoinPatch
{ private static bool Prefix(Coin __instance) => !FumeSupport.Sun(__instance); }

[HarmonyPatch(typeof(Coin), nameof(Coin.Collect))]
internal static class SunriseCoinCollectionPatch
{
    private static bool Prefix(Coin __instance) => !__instance.IsSun()
        || __instance.mUsableSeedType != SeedType.Fumeshroom;
}
