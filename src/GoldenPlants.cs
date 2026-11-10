using HarmonyLib;
using System.Diagnostics.CodeAnalysis;
using Il2CppReloaded.Gameplay;
using LawnGlove;

namespace FusionMod;

// Saved native fields own production, the sun bank and ultimate cooldowns.
// These two source plants have no projectiles. Native terrain and biting remain.
internal static class GoldenPlants
{
    internal static bool IsGolden(FusionPlant identity) => identity == FusionPlant.GoldenNut
        || identity == FusionPlant.GoldenSunflower;

    internal static bool Live([NotNullWhen(true)] Plant? plant) => plant != null && !plant.mDead && plant.mIsOnBoard
        && !plant.mSquished && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee
        && !plant.NotOnGround() && LawnGloveMod.GetCarriedPlant(plant.mBoard)?.Pointer != plant.Pointer;

    internal static IEnumerable<Plant> Sunflowers(Board board)
    {
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (Live(plant) && FusionPlant.From(plant) == FusionPlant.GoldenSunflower) yield return plant;
        }
    }

    internal static int SunflowerCount(Board board)
    {
        int count = 0;
        for (int i = 0; i < board.m_plants.Count; ++i)
            if (board.m_plants[i] is { mDead: false, mSquished: false } plant
                && FusionPlant.From(plant) == FusionPlant.GoldenSunflower) ++count;
        return count;
    }

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!IsGolden(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.GoldenNut ? 8000 : 300;
        plant.mLaunchCounter = 500;
        plant.mStartRow = plant.mTargetX = plant.mTargetY = plant.mShootingCounter = 0;
        plant.mDoSpecialCountdown = 0;
    }

    internal static void Tick(Board board)
    {
        if (!GoldenBean.Playing(board) || board.mPaused) return;
        if (!FusionTime.SourceStep(board, typeof(GoldenPlants))) return;
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (!Live(plant) || !IsGolden(FusionPlant.From(plant))) continue;
            if (plant.mTargetY > 0) --plant.mTargetY;
            if (plant.mShootingCounter > 0) --plant.mShootingCounter;
            if (FusionPlant.From(plant) != FusionPlant.GoldenSunflower) continue;
            if (--plant.mLaunchCounter <= 0)
            {
                plant.mLaunchCounter = 1000;
                plant.mStartRow = Math.Min(10000, plant.mStartRow + 50);
                plant.mShootingCounter = 100;
            }
            // The source coroutine releases one 50-sun pickup every .1 s.
            // Reserve sun only after allocating its pickup; failed allocation retries.
            if (plant.mTargetX > 0 && --plant.mTargetX == 0 && plant.mStartRow > 0)
            {
                if (plant.mStartRow < 50)
                {
                    // Native Replanted sources can overflow by amounts below
                    // 50. Credit that remainder exactly, without rounding up.
                    int remainder = plant.mStartRow;
                    plant.mStartRow = 0;
                    board.AddSunMoney(remainder, 0);
                }
                else
                {
                    var coin = board.AddCoin(plant.mX, plant.mY, CoinType.LargeSun, CoinMotion.FromPlant);
                    if (coin != null) plant.mStartRow -= 50;
                    if (plant.mStartRow > 0) plant.mTargetX = 10;
                }
            }
        }
    }

    internal static bool Ultimate(Plant plant)
    {
        if (!Live(plant) || plant.mTargetY > 0) return false;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.GoldenNut)
        {
            if (plant.mPlantMaxHealth < 1_000_000_000) plant.mPlantMaxHealth += 1000;
            plant.mPlantHealth = plant.mPlantMaxHealth;
            plant.mTargetY = 500;
        }
        else if (identity == FusionPlant.GoldenSunflower)
        {
            // Repeated activations cannot start a second payout coroutine.
            var producers = Sunflowers(plant.mBoard).ToArray();
            if (producers.Any(p => p.mTargetX > 0)) return false;
            foreach (var producer in producers)
                if (producer.mStartRow > 0) producer.mTargetX = 1;
            // Source cooldown is the selected bank's count * .1 seconds.
            plant.mTargetY = (plant.mStartRow / 50) * 10;
        }
        else return false;
        plant.mShootingCounter = 100;
        return true;
    }

    internal static void Fertilize(Plant plant)
    {
        // Fertilize.Upgrade caps positive attribute timers at .5 seconds.
        // It neither evolves silver plants nor gives a free paid ultimate.
        if (FusionPlant.From(plant) == FusionPlant.GoldenSunflower && plant.mLaunchCounter > 50)
            plant.mLaunchCounter = 50;
    }

    internal static int UpgradeCost(Board board, FusionPlant identity)
    {
        if (identity == FusionPlant.SilverNut) return 1000;
        if (identity != FusionPlant.SilverSunflower) return 0;
        int count = SunflowerCount(board);
        return count >= 20 ? int.MaxValue : (int)Math.Min(int.MaxValue, 3000L << count);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class GoldenInitializePatch
{
    private static void Postfix(Plant __instance) => GoldenPlants.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class GoldenAbilityPatch
{
    // Golden production has one simulation owner. Never run native Sunflower's
    // immediate production in addition to the saved personal sun bank.
    private static bool Prefix(Plant __instance) => !GoldenPlants.IsGolden(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Board), nameof(Board.AddSunMoney))]
internal static class GoldenSunCapacityPatch
{
    internal static void Prefix(Board __instance, int theAmount, int playerIndex, out int? __state)
    {
        __state = theAmount > 0 && playerIndex == 0 && (GoldenPlants.SunflowerCount(__instance) > 0 || __instance.mSunMoney[playerIndex].Amount > 9990)
            ? __instance.mSunMoney[playerIndex].Amount : null;
    }

    internal static void Postfix(Board __instance, int theAmount, int playerIndex, int? __state)
    {
        if (!__state.HasValue) return;
        long total = (long)__state.Value + theAmount;
        __instance.mSunMoney[playerIndex].Amount = (int)Math.Min(50000, total);
        int excess = (int)Math.Min(int.MaxValue, Math.Max(0, total - 50000));
        foreach (var plant in GoldenPlants.Sunflowers(__instance).Reverse())
        {
            int stored = Math.Min(excess, 10000 - Math.Clamp(plant.mStartRow, 0, 10000));
            plant.mStartRow += stored;
            excess -= stored;
            if (excess == 0) break;
        }
    }
}
