using HarmonyLib;
using Il2CppReloaded.Gameplay;
using System.Runtime.InteropServices;

namespace FusionMod;

// Native Flowerpot owns support placement, low profile and serialization.
internal static class PotFusions
{
    [ThreadStatic] internal static IntPtr CostingHealth;
    // Solar Pot retains its already published behavior and field ownership.
    private static bool Managed(FusionPlant identity) => FusionPlant.IsFusionPot(identity)
        && identity != FusionPlant.SolarPot;

    internal static bool Protected(Plant? plant) => plant != null
        && plant.Pointer != CostingHealth && Managed(FusionPlant.From(plant))
        && plant.mTargetX > 0 && plant.mTargetX <= 50 && ChomperFusions.Active(plant)
        && PlayerPlantingScope.ConsumingIngredient != plant.Pointer;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!Managed(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = plant.mTargetY = 0;
        plant.mTargetX = 50;
        if (identity == FusionPlant.BloverPot) plant.mLaunchCounter = 100;
    }

    internal static void Tick(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!Managed(identity) || !ChomperFusions.Active(plant)
            || !FusionTime.SourceStep(plant, typeof(PotFusions))) return;
        if (Managed(identity)
            && ChomperFusions.Active(plant) && plant.mTargetX > 0) --plant.mTargetX;
        if (identity == FusionPlant.SpikePot) SpikePot.Tick(plant);
        if (FusionPlant.From(plant) != FusionPlant.SilverPot || !ChomperFusions.Active(plant)
            || plant.mIsAsleep
            || plant.mApp.GameMode is >= GameMode.PuzzleIZombie1 and <= GameMode.PuzzleIZombieEndless) return;
        if (plant.mShootingCounter > 0 && --plant.mShootingCounter == 50)
            SilverMoney.DropMoney(plant.mBoard, plant.mX, plant.mY, SilverMoney.MoneyUnits(plant));
        if (--plant.mLaunchCounter > 0) return;
        // Source attribute interval: 25 seconds, randomized by 0.90..1.10.
        plant.mLaunchCounter = 2250 + plant.mBoard.Rand(501);
        plant.mShootingCounter = 100;
    }

    internal static void Illuminate(Board board)
    {
        if (!board.StageHasFog() || board.mPaused || board.mApp.GameScene != GameScenes.Playing) return;
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant != null && FusionPlant.From(plant) == FusionPlant.LumosPot
                && ChomperFusions.Active(plant) && !plant.NotOnGround())
            {
                // Native fog samples both vertical edges of a lawn tile.
                // ClearFogAroundPlant(size: 0) clears nothing: its vertical
                // distance is abs(row - plantRow - 1). Own just this column
                // and these two edges, using native fade speed (6 per tick).
                var fog = board.mGridCelFog;
                if (fog == null) continue;
                var array = fog.Cast<Il2CppSystem.Array>();
                FadeFog(fog.Pointer, array.GetLength(0), array.GetLength(1), plant.mPlantCol, plant.mRow);
            }
        }
    }

    internal static void FadeFog(IntPtr array, int columns, int rows, int col, int row, int radius = 0)
    {
        // Supported native int[,] layout is proven by UpdateFog and
        // ClearFogAroundPlant: data +0x20, column-major rows, 12 by 7.
        // Interop exposes multidimensional arrays as Il2CppObjectBase.
        if (array == IntPtr.Zero || columns != 12 || rows != 7
            || col < 0 || col >= 9 || row < 0 || row >= 6 || radius is < 0 or > 2) return;
        for (int x = Math.Max(0, col - radius); x <= Math.Min(columns - 1, col + radius); ++x)
        for (int edge = Math.Max(0, row - radius); edge <= Math.Min(rows - 1, row + radius + 1); ++edge)
        {
            int offset = 0x20 + 4 * (x * rows + edge);
            Marshal.WriteInt32(array, offset, Math.Max(0, Marshal.ReadInt32(array, offset) - 6));
        }
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class PotBiteProtectionPatch
{
    [HarmonyPriority(Priority.First)]
    internal static bool Prefix(Plant thePlant) => !PotFusions.Protected(thePlant);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class PotCrushProtectionPatch
{
    [HarmonyPriority(Priority.First)]
    internal static bool Prefix(Plant __instance) => !PotFusions.Protected(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.FindCollisionTargetPlant))]
internal static class PotProjectileProtectionPatch
{
    internal static void Postfix(Projectile __instance, ref Plant? __result)
    {
        if (__instance.mDead || __result?.mBoard?.Pointer != __instance.mBoard?.Pointer
            || !PotFusions.Protected(__result)) return;
        // Both enemy pea collision and descending basketballs call this
        // helper before their inlined health write. Consume only the shot.
        __instance.Die();
        __result = null;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class PotDeathProtectionPatch
{
    [HarmonyPriority(Priority.First)]
    internal static bool Prefix(Plant __instance)
    {
        if (!PotFusions.Protected(__instance) || (__instance.mPlantHealth > 0
            && RetaliationNuts.RadiusBoard != __instance.mBoard.Pointer
            && RetaliationNuts.FireBoard != __instance.mBoard.Pointer)) return true;
        __instance.mPlantHealth = __instance.mPlantMaxHealth;
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PotInitializePatch
{
    private static void Postfix(Plant __instance) => PotFusions.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class PotAbilityPatch
{
    private static void Postfix(Plant __instance) => PotFusions.Tick(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class SilverPotShootingPatch
{
    // The common native ability path decrements this field for every seed.
    private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.SilverPot;
}

[HarmonyPatch(typeof(Board), "UpdateFog")]
internal static class PotFogPatch
{
    private static void Postfix(Board __instance) => PotFusions.Illuminate(__instance);
}
