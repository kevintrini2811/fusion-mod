using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// HyponoFume.TrySetMindControl chooses control INSTEAD of the 40-damage hit.
// Native control is one-way: enemy zombies have no previous control levels.
internal static class PerfumeShroom
{
    internal static double Chance(long health, long maximum) => maximum <= 0 ? 0
        : 1 - Math.Max(.75, Math.Sqrt(Math.Clamp(2d * health / maximum, 0, 1)));

    internal static bool Eligible(Zombie zombie) => !zombie.mDead && !zombie.IsDeadOrDying()
        && !zombie.mMindControlled && zombie.mZombieType is not
            (ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target);

    internal static bool TryHypnotize(Plant plant, Zombie zombie)
    {
        if (!Eligible(zombie)) return false;
        double chance = Chance((long)Math.Max(0, zombie.mBodyHealth) + Math.Max(0, zombie.mHelmHealth),
            (long)Math.Max(0, zombie.mBodyMaxHealth) + Math.Max(0, zombie.mHelmMaxHealth));
        // One uniform draw per source attack, including the zero-chance case.
        if (plant.mBoard.Rand(1000000) >= chance * 1000000) return false;
        zombie.StartMindControlled();
        return zombie.mMindControlled;
    }

    internal static bool Bite(Plant plant, Zombie zombie)
    {
        if (FusionPlant.From(plant) != FusionPlant.PerfumeShroom || !FumeFusions.Active(plant)
            || !Eligible(zombie) || zombie.mBoard?.Pointer != plant.mBoard.Pointer) return false;
        // Allocate before consuming. The shared scope excludes fusion rewards,
        // death effects and unrelated plants; support and pumpkin survive.
        bool asleep = plant.mIsAsleep;
        var replacement = plant.mBoard.AddPlant(plant.mPlantCol, plant.mRow, SeedType.Fumeshroom, SeedType.None);
        if (replacement == null || replacement.mDead || !replacement.mIsOnBoard) return true;
        replacement.SetSleeping(asleep);
        PlayerPlantingScope.Consume(plant);
        zombie.StartMindControlled();
        return true;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class PerfumeBitePatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Zombie __instance, Plant thePlant, bool __runOriginal) =>
        !__runOriginal || !PerfumeShroom.Bite(thePlant, __instance);
}
