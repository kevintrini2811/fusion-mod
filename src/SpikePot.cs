using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class SpikePot
{
    internal const int AttackTicks = 67, EventTick = 33;
    private static bool Target(Plant plant, Zombie zombie) => zombie.mBoard?.Pointer == plant.mBoard.Pointer
        && zombie.mRow == plant.mRow
        && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)
        && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target or ZombieType.Boss)
        && Math.Abs(plant.mX + plant.mWidth / 2f - Math.Clamp(plant.mX + plant.mWidth / 2f,
            zombie.GetZombieRect().xMin, zombie.GetZombieRect().xMax)) <= ScaredyFusions.SourceUnit;

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        var board = plant.mBoard;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && Target(plant, zombie)) targets.Add((zombie, zombie.DataID));
        // Source KillCar is separate from the animation's 20 damage event.
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && Target(plant, zombie)
                && zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult)
            {
                // Commit our removal before callbacks or pooled allocations.
                PlayerPlantingScope.Consume(plant);
                zombie.DieWithLoot();
                return;
            }
        if (plant.mStartRow == 0 && plant.mLaunchCounter == 0 && targets.Count > 0)
        {
            plant.mStartRow = AttackTicks;
            plant.mLaunchCounter = 90 + board.Rand(21);
        }
        if (plant.mStartRow <= 0) return;
        if (AttackTicks - plant.mStartRow == EventTick)
            foreach (var (zombie, id) in targets)
                if (zombie.DataID == id && Target(plant, zombie))
                    zombie.TakeDamage(20, DamageFlags.BypassesShield | DamageFlags.Spike);
        --plant.mStartRow;
    }

    internal static bool Crush(Plant plant)
    {
        var crush = WeedFusions.CurrentCrush;
        if (FusionPlant.From(plant) != FusionPlant.SpikePot || !ChomperFusions.Active(plant)
            || PotFusions.Protected(plant) || crush == null || crush.Zombie.DataID != crush.ZombieId
            || crush.Board.Pointer != plant.mBoard.Pointer || !Target(plant, crush.Zombie)
            || crush.Zombie.mZombieType is not (ZombieType.Zamboni or ZombieType.Catapult)) return false;
        PlayerPlantingScope.Consume(plant);
        crush.Zombie.DieWithLoot();
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class SpikePotVehiclePatch
{
    private static bool Prefix(Plant __instance, bool __runOriginal) => !__runOriginal || !SpikePot.Crush(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.CanTargetPlant))]
internal static class SpikePotTargetPatch
{
    private static void Postfix(Plant thePlant, ZombieAttackType theAttackType, ref bool __result)
    {
        if (theAttackType == ZombieAttackType.Chew && thePlant != null
            && FusionPlant.From(thePlant) == FusionPlant.SpikePot) __result = false;
    }
}
