using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Chomper owns target selection, swallowing and saved digestion states.
// Nut timers and permanent damage flags use otherwise unused native fields.
internal static class SolarChomperPlants
{
    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsSolarNut(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 4000;
        plant.mLaunchCounter = 1500;
        plant.mShootingCounter = 0;
    }

    internal static void AfterNative(Plant plant, PlantState before)
    {
        if (!FusionTime.IsSourcePulse(plant.mBoard)) return;
        var identity = FusionPlant.From(plant);
        if (identity != FusionPlant.ChompMine && identity != FusionPlant.SolarChomper) return;
        if (!FusionTime.SourceStep(plant, typeof(SolarChomperPlants))) return;
        if (before != PlantState.ChomperDigesting && plant.mState == PlantState.ChomperDigesting)
            plant.mStateCountdown = identity == FusionPlant.ChompMine ? 2000 : 4000;
        if (before != PlantState.ChomperDigesting || plant.mState == PlantState.ChomperDigesting) return;
        if (identity == FusionPlant.SolarChomper)
            for (int i = 0; i < 4; ++i) Sun(plant);
        else CreateMine(plant);
    }

    internal static void CreateMine(Plant plant)
    {
        var board = plant.mBoard;
        int col = plant.mPlantCol + 1, row = plant.mRow;
        if (col >= 9 || FusionService.CheckNativePlacement(board, col, row, SeedType.Potatomine) != PlantingReason.Ok) return;
        var previous = PlayerPlantingScope.Current;
        PlayerPlantingScope.Current = null;
        try
        {
            var mine = board.AddPlant(col, row, SeedType.Potatomine, SeedType.None);
            if (mine != null && !mine.mDead && mine.mIsOnBoard) MineFusions.GrowImmediately(mine);
        }
        finally { PlayerPlantingScope.Current = previous; }
    }

    internal static void Sun(Plant plant) =>
        plant.mBoard.AddCoin(plant.mX, plant.mY, CoinType.Sun, CoinMotion.FromPlant);

    internal static void UpdateNut(Plant plant)
    {
        if (!ChomperFusions.Active(plant)) return;
        if (!FusionTime.SourceStep(plant, typeof(SolarChomperPlants))) return;
        // Source ReplaceSprite rewards the current band once; healing never
        // rearms it and jumping straight to the lowest band earns one reward.
        int bit = plant.mPlantHealth < plant.mPlantMaxHealth / 3 ? 2
            : plant.mPlantHealth < (long)plant.mPlantMaxHealth * 2 / 3
                && plant.mPlantHealth > plant.mPlantMaxHealth / 3 ? 1 : 0;
        if (bit != 0 && (plant.mShootingCounter & bit) == 0)
        { plant.mShootingCounter |= bit; Sun(plant); }
        if (plant.mApp.GameMode is >= GameMode.PuzzleIZombie1 and <= GameMode.PuzzleIZombieEndless) return;
        if (--plant.mLaunchCounter > 0) return;
        Sun(plant);
        plant.mLaunchCounter = (15 + plant.mBoard.Rand(5) - 2) * 100;
    }

    internal static void OnDeath(CherryBombPlants.Death death)
    {
        if (death.Identity == FusionPlant.ChompMine)
            MineFusions.Blast(death.Board, death.X, death.Y, 80, 1800, death.Order);
        else
            for (int i = 0; i < 2; ++i)
                death.Board.AddCoin(death.X, death.Y, CoinType.Sun, CoinMotion.FromPlant);
    }

    internal static void Hit(Plant plant)
    {
        plant.mPlantHealth -= 50;
        SolarShooter.DropSun(plant.mBoard, plant.mX, plant.mY, 15);
        if (plant.mPlantHealth <= 0) plant.Die();
    }

    internal sealed record DamageObservation(Plant Plant, uint Id, IntPtr Board, int Health)
    { internal bool Completed; }
    internal static DamageObservation? Observe(Plant? plant) => plant != null
        && FusionPlant.From(plant) == FusionPlant.TwinSolarNut && ChomperFusions.Active(plant)
        ? new(plant, plant.DataID, plant.mBoard.Pointer, plant.mPlantHealth) : null;
    internal static DamageObservation? BeginDamage(Plant? plant)
    {
        var saved = Observe(plant);
        // Native writers may call Die before returning. Reserve health during
        // the synchronous damage path so only the source's fixed 50 can kill.
        if (saved != null) saved.Plant.mPlantHealth = int.MaxValue;
        return saved;
    }
    internal static void Restore(DamageObservation? saved)
    {
        if (saved == null || saved.Completed) return;
        saved.Completed = true;
        if (saved.Plant.DataID == saved.Id && saved.Plant.mBoard?.Pointer == saved.Board)
            saved.Plant.mPlantHealth = saved.Health;
    }
    internal static void Finish(DamageObservation? saved)
    {
        if (saved == null || saved.Plant.DataID != saved.Id || saved.Plant.mBoard?.Pointer != saved.Board
            || saved.Completed) return;
        bool hit = !saved.Plant.mDead && saved.Plant.mPlantHealth < int.MaxValue;
        Restore(saved);
        if (hit) Hit(saved.Plant);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SolarNutInitializePatch
{
    private static void Postfix(Plant __instance) => SolarChomperPlants.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SolarNutAbilitiesPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsSolarNut(FusionPlant.From(__instance))) return true;
        SolarChomperPlants.UpdateNut(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class TwinSolarNutBitePatch
{
    private static void Prefix(Plant thePlant, out SolarChomperPlants.DamageObservation? __state) =>
        __state = SolarChomperPlants.BeginDamage(thePlant);
    private static void Postfix(SolarChomperPlants.DamageObservation? __state) => SolarChomperPlants.Finish(__state);
    private static void Finalizer(SolarChomperPlants.DamageObservation? __state) => SolarChomperPlants.Restore(__state);
}

// Zombie peas subtract health directly in CheckForCollision, bypassing a
// plant damage method. Observe that actual writer, after native hit selection.
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class TwinSolarNutProjectilePatch
{
    private static void Prefix(Projectile __instance, out List<SolarChomperPlants.DamageObservation>? __state)
    {
        __state = null;
        if (__instance.mProjectileType != ProjectileType.ZombiePea || __instance.mBoard == null) return;
        __state = new();
        var plants = __instance.mBoard.m_plants;
        for (int i = 0; i < plants.Count; ++i)
            if (SolarChomperPlants.BeginDamage(plants[i]) is { } saved) __state.Add(saved);
    }
    private static void Postfix(List<SolarChomperPlants.DamageObservation>? __state)
    { if (__state != null) foreach (var saved in __state) SolarChomperPlants.Finish(saved); }
    private static void Finalizer(List<SolarChomperPlants.DamageObservation>? __state)
    { if (__state != null) foreach (var saved in __state) SolarChomperPlants.Restore(saved); }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class TwinSolarNutCrushPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (SolarChomperPlants.Observe(__instance) == null) return true;
        SolarChomperPlants.Hit(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class TwinSolarNutExplosionPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (ChomperFusions.PlantExplosionBoard?.Pointer != __instance.mBoard?.Pointer
            || PlayerPlantingScope.ConsumingIngredient == __instance.Pointer
            || __instance.mPlantHealth <= 0 || SolarChomperPlants.Observe(__instance) == null) return true;
        SolarChomperPlants.Hit(__instance);
        return false;
    }
}

// Preserve native collision iteration, overlap threshold and pumpkin priority.
// Only this read-only lookup exposes Chomp-mine as the native low category.
[HarmonyPatch(typeof(Projectile), nameof(Projectile.FindCollisionTargetPlant))]
internal static class ChompMineLowCollisionScope
{
    internal sealed record Saved(Plant Plant, uint Id, IntPtr Board);
    private static void Prefix(Projectile __instance, out List<Saved>? __state)
    {
        __state = null;
        if (__instance.mProjectileType != ProjectileType.ZombiePea || __instance.mBoard == null) return;
        __state = new();
        var plants = __instance.mBoard.m_plants;
        for (int i = 0; i < plants.Count; ++i)
        {
            var plant = plants[i];
            if (plant == null || plant.mDead || FusionPlant.From(plant) != FusionPlant.ChompMine) continue;
            __state.Add(new(plant, plant.DataID, plant.mBoard.Pointer));
            plant.mSeedType = SeedType.Potatomine;
        }
    }
    private static void Finalizer(List<Saved>? __state)
    {
        if (__state == null) return;
        foreach (var saved in __state)
            if (saved.Plant.DataID == saved.Id && saved.Plant.mBoard?.Pointer == saved.Board)
                saved.Plant.mSeedType = SeedType.Chomper;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
internal static class ChompMineRangePatch
{
    private static void Postfix(Plant __instance, ref Zombie? __result)
    {
        // PotatoChomper.ChompRange is half the base value 2: one source cell.
        if (__result != null && FusionPlant.From(__instance) == FusionPlant.ChompMine
            && __result.GetZombieRect().xMin > __instance.mX + __instance.mWidth / 2f + 80)
            __result = null;
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.MouseDown))]
internal static class TwinSolarNutClickPatch
{
    private static bool Prefix(Board __instance, int x, int y, int theClickCount, int playerIndex)
    {
        if (playerIndex != 0 || theClickCount < 0 || __instance.mPaused
            || __instance.mCursorObject.CursorType != CursorType.Normal) return true;
        int col = __instance.PixelToGridX(x, y), row = __instance.PixelToGridY(x, y);
        if (col < 0 || col >= 9 || row < 0 || row >= __instance.GetNumRows()) return true;
        var plant = __instance.GetPlantsOnLawn(col, row).NormalPlant;
        if (SolarChomperPlants.Observe(plant) == null) return true;
        if (!__instance.CanTakeSunMoney(500, 0)) return false;
        // Build the ball before charging or healing. An invalid front square
        // or failed allocation retains sun and the existing plant.
        if (!RollingProjectiles.ChargeSolarBall(plant!)) return false;
        if (!__instance.TakeSunMoney(500, 0)) return false;
        plant!.mPlantHealth = (int)Math.Min(int.MaxValue, (long)plant.mPlantHealth + plant.mPlantMaxHealth);
        return false;
    }
}
