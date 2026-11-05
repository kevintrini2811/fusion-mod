using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Chomper owns swallowing and saved digestion for the two ordinary
// variants. Chompzilla bites repeatedly instead; its native Chomper counters
// store the bite phase and special cooldown without changing the save format.
internal static class ChomperFusions
{
    [ThreadStatic] internal static Board? PlantExplosionBoard;
    [ThreadStatic] internal static bool ApplyingCappedDamage;
    internal static bool Active(Plant plant) => plant.mBoard != null && !plant.mBoard.mPaused
        && plant.mBoard.Pointer != CherryBombPlants.DisposingBoard && !plant.mDead
        && plant.mIsOnBoard && !plant.mSquished && !plant.NotOnGround()
        && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee
        && plant.mApp != null && plant.mApp.GameScene == GameScenes.Playing
        && plant.mApp.GameMode != GameMode.ChallengeZenGarden;

    internal static void AfterNative(Plant plant, PlantState before)
    {
        if (!Active(plant)) return;
        var identity = FusionPlant.From(plant);
        SolarChomperPlants.AfterNative(plant, before);
        if (identity == FusionPlant.ChompNut && before == PlantState.ChomperDigesting
            && plant.mState != PlantState.ChomperDigesting)
            plant.mPlantHealth = Math.Min(plant.mPlantMaxHealth, plant.mPlantHealth + 1000);
        if (identity != FusionPlant.ChompShooter) return;
        if (before != PlantState.ChomperDigesting && plant.mState == PlantState.ChomperDigesting)
        {
            plant.mStateCountdown = 3000;
            plant.mLaunchCounter = 300;
        }
        if (plant.mState == PlantState.ChomperDigesting)
        {
            if (--plant.mLaunchCounter <= 0) plant.mLaunchCounter = 300;
            // Original additive shoot clip emits at .833 seconds.
            if (plant.mLaunchCounter == 217) RollingProjectiles.Spawn(plant, false);
        }
    }

    internal const int ChompzillaDigestionTicks = 4000;
    internal static void Fertilize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.Chompzilla && plant.mStateCountdown > 50)
            plant.mStateCountdown = 50;
    }

    internal static void UpdateChompzilla(Plant plant)
    {
        if (!Active(plant)) return;
        if (plant.mStateCountdown > 0) --plant.mStateCountdown;
        // Excess health decays by 5% of toughness per simulation second.
        if (plant.mPlantHealth > 4000 && plant.mBoard.mMainCounter % 100 == 0)
            plant.mPlantHealth = Math.Max(4000, plant.mPlantHealth - 200);
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mLaunchCounter == 0)
        {
            var target = plant.FindTargetZombie(plant.mRow, PlantWeapon.Primary);
            if (target == null) return;
            plant.mLaunchCounter = 175;
        }
        // Original AnimShoot at 1.65 seconds follows BiteEvent at 1.05.
        // Save the selected chunk until that event; target death cannot cancel it.
        if (plant.mLaunchCounter == 31 && plant.mTargetX is >= 1 and <= 3)
        {
            RollingProjectiles.Spawn(plant, plant.mTargetX == 3, plant.mTargetX);
            plant.mTargetX = 0;
        }
        // Original bite event is at 1.05 of its two-second clip. Map that
        // event to the 1.75-second attack cycle described by the almanac.
        if (plant.mLaunchCounter != 83) return;
        // Reacquire at the bite event. A dead/moved target must not be hit.
        var zombie = plant.FindTargetZombie(plant.mRow, PlantWeapon.Primary);
        if (zombie == null || zombie.mDead || zombie.IsDeadOrDying()) return;
        // SuperChomper.Bite checks the weak-target branch FIRST. Such a
        // bite neither consumes nor restarts the special digestion timer.
        bool weak = zombie.mHelmHealth <= 0 && zombie.mBodyHealth <= 500;
        bool special = !weak && plant.mStateCountdown == 0;
        int damage = special ? 1_000_000 : weak ? 500 : 200;
        int health = zombie.mBodyHealth + Math.Max(0, zombie.mHelmHealth) + Math.Max(0, zombie.mShieldHealth);
        zombie.TakeDamage(damage, DamageFlags.BypassesShield | DamageFlags.DoesntLeaveBody);
        int healing = special ? 16000 : weak ? 400 : Math.Min(damage, Math.Max(0, health));
        plant.mPlantHealth = (int)Math.Min(16000L, (long)plant.mPlantHealth + healing);
        if (special) plant.mStateCountdown = ChompzillaDigestionTicks;
        plant.mTargetX = weak ? 3 : plant.mBoard.Rand(2) + 1;
    }

    internal static void CappedDamage(Plant plant)
    {
        plant.mPlantHealth -= 4000 / 3;
        if (plant.mPlantHealth > 0) return;
        bool previous = ApplyingCappedDamage;
        ApplyingCappedDamage = true;
        try { plant.Die(); }
        finally { ApplyingCappedDamage = previous; }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class ChomperFusionInitializePatch
{
    private static void Postfix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (identity == FusionPlant.PeaNut || identity == FusionPlant.ChompNut || identity == FusionPlant.Chompzilla)
            __instance.mPlantHealth = __instance.mPlantMaxHealth = 4000;
        if (identity == FusionPlant.PeaNut) __instance.mLaunchRate = 150;
        if (identity == FusionPlant.Chompzilla)
        {
            __instance.mLaunchCounter = __instance.mTargetX = 0;
            __instance.mStateCountdown = 0;
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class ChomperFusionAbilitiesPatch
{
    private static bool Prefix(Plant __instance, out PlantState __state)
    {
        __state = __instance.mState;
        if (FusionPlant.From(__instance) != FusionPlant.Chompzilla) return true;
        ChomperFusions.UpdateChompzilla(__instance);
        return false;
    }
    private static void Postfix(Plant __instance, PlantState __state) => ChomperFusions.AfterNative(__instance, __state);
}

// Replanted represents crushing as an explicit removal, rather than numerical
// TakeDamage. Convert only live Chompzilla crushing to capped damage. Shovel,
// consumption, bungee and board teardown remain real removals.
[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class ChompzillaCrushPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.Chompzilla || !ChomperFusions.Active(__instance)) return true;
        ChomperFusions.CappedDamage(__instance);
        return false;
    }
}

// Jack-in-the-Box calls the Board radius helper, which calls Plant.Die.
// Scope that specific damage path: never reinterpret shovel/consumption/load.
[HarmonyPatch]
internal static class ChompzillaExplosionScope
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() =>
        typeof(Board).GetMethods().Where(m => m.Name == nameof(Board.KillAllPlantsInRadius));
    private static void Prefix(Board __instance, out Board? __state)
    { __state = ChomperFusions.PlantExplosionBoard; ChomperFusions.PlantExplosionBoard = __instance; }
    private static void Finalizer(Board? __state) => ChomperFusions.PlantExplosionBoard = __state;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class ChompzillaExplosionDamagePatch
{
    private static bool Prefix(Plant __instance)
    {
        if (ChomperFusions.ApplyingCappedDamage
            || ChomperFusions.PlantExplosionBoard?.Pointer != __instance.mBoard?.Pointer
            || FusionPlant.From(__instance) != FusionPlant.Chompzilla
            || PlayerPlantingScope.ConsumingIngredient == __instance.Pointer
            || !ChomperFusions.Active(__instance)) return true;
        ChomperFusions.CappedDamage(__instance);
        return false;
    }
}
