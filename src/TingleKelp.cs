using HarmonyLib;
using Il2CppReloaded.Gameplay;
using LawnGlove;

namespace FusionMod;

internal static class TingleKelp
{
    internal const int ExplodeTicks = 67;
    internal static bool Is(Plant plant) => FusionPlant.From(plant) == FusionPlant.TingleKelp;
    internal static void Initialize(Plant plant)
    {
        if (!Is(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mStartRow = plant.mTargetX = plant.mTargetY = 0;
    }
    internal static bool Near(Plant plant, Zombie zombie)
    {
        if (zombie.mRow != plant.mRow || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Submerged)) return false;
        var rect = zombie.GetZombieRect();
        float center = plant.mX + plant.mWidth * .5f;
        return Math.Abs(center - (rect.xMin + rect.xMax) * .5f) <= .7f * ScaredyFusions.SourceUnit;
    }
    internal static void Tick(Plant plant)
    {
        SeaPot.Advance(plant, TickOnce);
    }
    private static void TickOnce(Plant plant)
    {
        if (!Is(plant) || !ChomperFusions.Active(plant)) return;
        if (plant.mStartRow == 0)
        {
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
                if (plant.mBoard.m_zombies[i] is { } zombie && Near(plant, zombie))
                { plant.mStartRow = ExplodeTicks; break; }
            return;
        }
        if (plant.mStartRow > 0 && --plant.mStartRow == 0) Explode(plant);
    }
    internal static void Explode(Plant plant)
    {
        if (!Is(plant) || plant.mDead || plant.mTargetX != 0) return;
        plant.mTargetX = 1;
        KelpProjectiles.Burst(plant);
        // Consume suppresses ordinary death rewards and native kelp drowning.
        PlayerPlantingScope.Consume(plant);
    }
    internal static bool Spawn(Board board, int col, int row)
    {
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()
            || !KelpEffects.Playing(board) || FusionService.CheckNativePlacement(board, col, row, SeedType.Tanglekelp) != PlantingReason.Ok)
            return false;
        var plant = board.AddPlant(col, row, SeedType.Tanglekelp, FusionPlant.TingleKelp.Marker);
        return plant != null && !plant.mDead && plant.mIsOnBoard;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class TingleInitializePatch
{
    private static void Postfix(Plant __instance) => TingleKelp.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateTanglekelp))]
internal static class TingleAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!TingleKelp.Is(__instance)) return true;
        TingleKelp.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class TingleCrushPatch
{
    private static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal || !TingleKelp.Is(__instance)) return true;
        TingleKelp.Explode(__instance); return false;
    }
}
