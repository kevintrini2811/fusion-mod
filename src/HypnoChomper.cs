using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Chomper owns its bite animation, damage, target validation and digestion.
// mTargetX remembers an allied meal until digestion starts; mTargetY keeps an
// unallocated reward packet pending. Both fields survive native save/load.
internal static class HypnoChomper
{
    [ThreadStatic] private static (IntPtr Plant, uint Id, IntPtr Board)? alliedSearch;

    internal static bool SearchingAllies(Plant plant) => alliedSearch is { } search
        && search.Plant == plant.Pointer && search.Id == plant.DataID
        && search.Board == plant.mBoard?.Pointer;

    internal static void Initialize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.HypnoChomper)
            plant.mTargetX = plant.mTargetY = 0;
    }

    internal static Zombie? Target(Plant plant, int row, PlantWeapon weapon, Zombie? ordinary)
    {
        if (FusionPlant.From(plant) != FusionPlant.HypnoChomper || !ChomperFusions.Active(plant)
            || weapon != PlantWeapon.Primary || SearchingAllies(plant)) return ordinary;
        var previous = alliedSearch;
        Zombie? ally;
        alliedSearch = (plant.Pointer, plant.DataID, plant.mBoard.Pointer);
        try { ally = plant.FindTargetZombie(row, weapon); }
        finally { alliedSearch = previous; }
        // FindTargetZombie uses the leftmost attack rectangle, preserving the
        // first candidate on ties. Its second pass retains every native rule.
        return ally != null && (ordinary == null || (int)ally.GetZombieRect().xMin < (int)ordinary.GetZombieRect().xMin)
            ? ally : ordinary;
    }

    internal sealed record Meal(Plant Plant, uint PlantId, IntPtr Board, Zombie Zombie, uint ZombieId);

    internal static Meal? Capture(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.HypnoChomper || !ChomperFusions.Active(plant)
            || plant.mState != PlantState.ChomperBiting) return null;
        var target = plant.FindTargetZombie(plant.mRow, PlantWeapon.Primary);
        return target != null && target.mMindControlled && !target.mDead && !target.IsDeadOrDying()
            ? new(plant, plant.DataID, plant.mBoard.Pointer, target, target.DataID) : null;
    }

    internal static void Complete(Meal? meal)
    {
        if (meal == null || meal.Plant.DataID != meal.PlantId || meal.Plant.mBoard?.Pointer != meal.Board
            || !ChomperFusions.Active(meal.Plant) || meal.Plant.mState != PlantState.ChomperBitingGotOne
            || meal.Plant.mTargetX == 1 // The inner native entry may already have committed this meal.
            || meal.Zombie.DataID != meal.ZombieId || meal.Zombie.mBoard?.Pointer != meal.Board
            || !meal.Zombie.mDead) return;
        meal.Plant.mTargetX = meal.Plant.mTargetY = 1;
    }

    internal static void DropPacket(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.HypnoChomper || !ChomperFusions.Active(plant)
            || plant.mTargetY != 1) return;
        uint id = plant.DataID;
        var board = plant.mBoard;
        plant.mTargetY = 0; // Reserve before allocation: callbacks cannot duplicate the reward.
        var coin = board.AddCoin(plant.mX, plant.mY, CoinType.UsableSeedPacket, CoinMotion.FromPlant);
        if (coin == null || coin.mDead)
        {
            if (plant.DataID == id && plant.mBoard?.Pointer == board.Pointer && !plant.mDead) plant.mTargetY = 1;
            return;
        }
        coin.mUsableSeedType = SeedType.Hypnoshroom;
        if (coin.mController != null) coin.mController.SeedType = SeedType.Hypnoshroom;
    }

    internal static void AfterNative(Plant plant, PlantState before)
    {
        if (!FusionTime.IsSourcePulse(plant.mBoard)
            || FusionPlant.From(plant) != FusionPlant.HypnoChomper
            || before == PlantState.ChomperDigesting || plant.mState != PlantState.ChomperDigesting
            || plant.mTargetX != 1) return;
        // Native code owns ordinary digestion. Clearing the allied flag makes
        // this shortening idempotent when both patched entries are nested.
        plant.mStateCountdown = Math.Min(plant.mStateCountdown, 1000);
        plant.mTargetX = 0;
    }

    internal static bool Bite(Plant plant, Zombie zombie)
    {
        if (FusionPlant.From(plant) != FusionPlant.HypnoChomper) return false;
        if (!ChomperFusions.Active(plant) || !PerfumeShroom.Eligible(zombie)
            || zombie.mBoard?.Pointer != plant.mBoard.Pointer) return true;
        uint plantId = plant.DataID, zombieId = zombie.DataID;
        var board = plant.mBoard;
        var replacement = board.AddPlant(plant.mPlantCol, plant.mRow, SeedType.Chomper, SeedType.None);
        if (replacement == null || replacement.mDead || !replacement.mIsOnBoard) return true;
        if (plant.DataID != plantId || plant.mDead || plant.mBoard?.Pointer != board.Pointer)
        {
            PlayerPlantingScope.Consume(replacement);
            return true;
        }
        PlayerPlantingScope.Consume(plant);
        if (zombie.DataID == zombieId && zombie.mBoard?.Pointer == board.Pointer) HypnoPlants.MindControl(zombie);
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class HypnoChomperInitializePatch
{ private static void Postfix(Plant __instance) => HypnoChomper.Initialize(__instance); }

[HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
internal static class HypnoChomperTargetPatch
{
    private static void Postfix(Plant __instance, int theRow, PlantWeapon thePlantWeapon, ref Zombie? __result) =>
        __result = HypnoChomper.Target(__instance, theRow, thePlantWeapon, __result);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.GetDamageRangeFlags))]
internal static class HypnoChomperAlliedRangePatch
{
    private static void Postfix(Plant __instance, ref DamageRangeFlags __result)
    { if (HypnoChomper.SearchingAllies(__instance)) __result |= DamageRangeFlags.OnlyMindcontrolled; }
}

[HarmonyPatch]
internal static class HypnoChomperMealPatch
{
    // Replanted inlines the Chomper state machine inside UpdateAbilities.
    // Hook its actual caller as well as the standalone native entry point.
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() => new[] {
        typeof(Plant).GetMethod(nameof(Plant.UpdateAbilities))!,
        typeof(Plant).GetMethod(nameof(Plant.UpdateChomper))!
    };
    private static void Prefix(Plant __instance, bool __runOriginal,
        out (PlantState State, HypnoChomper.Meal? Meal, bool Eligible) __state) =>
        __state = (__instance.mState, __runOriginal && FusionTime.IsSourcePulse(__instance.mBoard)
            ? HypnoChomper.Capture(__instance) : null,
            __runOriginal && FusionTime.IsSourcePulse(__instance.mBoard));
    private static void Postfix(Plant __instance, (PlantState State, HypnoChomper.Meal? Meal, bool Eligible) __state)
    {
        if (!__state.Eligible) return;
        HypnoChomper.Complete(__state.Meal);
        if (ChomperFusions.Active(__instance)) HypnoChomper.AfterNative(__instance, __state.State);
        HypnoChomper.DropPacket(__instance);
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class HypnoChomperBitePatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Zombie __instance, Plant thePlant, bool __runOriginal) =>
        !__runOriginal || !HypnoChomper.Bite(thePlant, __instance);
}
