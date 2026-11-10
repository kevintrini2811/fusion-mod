using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

internal static class Spicicle
{
    internal static Plant? Guard(Plant target)
    {
        if (!ChomperFusions.Active(target) || FusionPlant.From(target) == FusionPlant.Spicicle) return null;
        for (int i = 0; i < target.mBoard.m_plants.Count; ++i)
        {
            var plant = target.mBoard.m_plants[i];
            if (plant != null && FusionPlant.From(plant) == FusionPlant.Spicicle
                && FumeFusions.Active(plant) && plant.mPlantHealth > 0) return plant;
        }
        return null;
    }

    internal static bool Redirect(Plant target, int damage)
    {
        if (damage <= 0 || Guard(target) is not { } guard) return false;
        guard.mPlantHealth -= damage;
        if (guard.mPlantHealth <= 0) guard.Die();
        return true;
    }

    internal static void Fertilize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.Spicicle && plant.mTargetX > 50) plant.mTargetX = 50;
    }

    internal static void Initialize(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.Spicicle) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 32000;
        plant.mDoSpecialCountdown = 0;
        plant.mTargetX = 100;
        plant.mTargetY = plant.mStartRow = plant.mLaunchCounter = plant.mShootingCounter = 0;
    }

    internal static void Tick(Plant plant)
    {
        if (!FumeFusions.Active(plant)) return;
        if (!FusionTime.SourceStep(plant, typeof(Spicicle))) return;
        if (--plant.mTargetX > 0) return;
        plant.mTargetX = 100;
        plant.mPlantHealth -= plant.mPlantMaxHealth / 5;
        if (plant.mPlantHealth <= 0) plant.Die();
    }

    internal static void Explode(CherryBombPlants.Death death)
    {
        var board = death.Board;
        var targets = new List<(Zombie Zombie, uint Id)>();
        var range = DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
            | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Underground;
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && zombie.mRow == death.Row
                && SquashFusions.Eligible(board, zombie, range)) targets.Add((zombie, zombie.DataID));
        ElementalNuts.Heal(board, true, death.Row);
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie, range)) continue;
            zombie.RemoveColdEffects();
            zombie.TakeDamage(1800, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
        }
        NativeCombatVisuals.Fire(board, death.Row, death.Order);
        board.mApp.SoundSystem.PlayFoley(FoleyType.JalapenoIgnite);
        // Source orders the row fire BEFORE global ice. Re-snapshot after fire
        // callbacks; newly generated enemies must receive the following freeze.
        targets.Clear();
        if (!PumpkinSupport.StoreIce(board, death.Col, death.Row))
        {
            for (int i = 0; i < board.m_zombies.Count; ++i)
                if (board.m_zombies[i] is { } zombie && SquashFusions.Eligible(board, zombie, range))
                    targets.Add((zombie, zombie.DataID));
            foreach (var (zombie, id) in targets)
            {
                if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie, range)) continue;
                IceInstantFusions.Freeze(zombie, 400);
                zombie.TakeDamage(20, DamageFlags.BypassesShield);
            }
            ElementalNuts.Heal(board, false);
        }
        board.mApp.AddTodParticle(death.X, death.Y, death.Order, ParticleEffect.IceSparkle);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Frozen);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SpicicleInitializePatch
{ private static void Postfix(Plant __instance) => Spicicle.Initialize(__instance); }

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SpicicleTickPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.Spicicle) return true;
        Spicicle.Tick(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class SpicicleSpecialPatch
{ private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.Spicicle; }

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class SpicicleCrushGuardPatch
{
    [HarmonyPriority(Priority.First + 300)]
    private static bool Prefix(Plant __instance, bool __runOriginal) => !__runOriginal
        || !Spicicle.Redirect(__instance, 900);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class SpicicleDeathPatch
{
    [HarmonyPriority(Priority.First + 300)]
    private static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal || PlayerPlantingScope.ConsumingIngredient == __instance.Pointer) return true;
        // Apply pending numeric loss before the native lethal removal, while
        // the target's saved identity and health snapshot still belong to it.
        var hit = ReactiveFumeDamage.Current?.FirstOrDefault(h => h.Plant.Pointer == __instance.Pointer && !h.Applied);
        if (hit != null && hit.Valid && __instance.mPlantHealth < hit.Health && Spicicle.Guard(__instance) != null)
        {
            hit.Apply();
            if (__instance.mPlantHealth > 0)
            {
                if (hit.Bite) --__instance.mBoard.mPlantsEaten;
                return false;
            }
        }
        if ((RetaliationNuts.RadiusBoard == __instance.mBoard?.Pointer
                || RetaliationNuts.FireBoard == __instance.mBoard?.Pointer)
            && Spicicle.Redirect(__instance, 1800))
        {
            --__instance.mBoard.mPlantsEaten;
            return false;
        }
        return true;
    }

}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class SpicicleDeathEffectPatch
{
    [HarmonyPriority(Priority.Last - 100)]
    private static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state)
    {
        __state = __runOriginal && FusionPlant.From(__instance) == FusionPlant.Spicicle
            && CherryBombPlants.CanTriggerDeath(__instance) ? IceInstantFusions.Capture(__instance) : null;
    }

    private static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id
            && __instance.mBoard?.Pointer == __state.Board.Pointer) Spicicle.Explode(__state);
    }
}
