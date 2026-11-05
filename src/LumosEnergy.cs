using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Saved energy accounting shared by actual health writers and interception.
internal static class LumosEnergy
{
    internal static void Spend(Plant plant, int amount)
    {
        plant.mTargetX = Math.Max(0, plant.mTargetX - amount);
        if (plant.mTargetX == 0) plant.mTargetY = 500;
    }
    internal static int Absorb(Plant plant, int damage)
    {
        if (FusionPlant.From(plant) != FusionPlant.LumosUmbrella || damage <= 0) return damage;
        int saved = Math.Min(damage, plant.mTargetX / 5);
        Spend(plant, (int)Math.Min(int.MaxValue, (long)damage * 5));
        return damage - saved;
    }
}
