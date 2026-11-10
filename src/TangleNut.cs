using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class TangleNut
{
    internal static bool Is(Plant plant) => FusionPlant.From(plant) == FusionPlant.TangleNut;
    internal static void Initialize(Plant plant)
    {
        if (!Is(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 4000;
        plant.mTargetY = 0;
    }
    internal static void Tick(Plant plant)
    {
        if (ChomperFusions.Active(plant) && FusionTime.SourceStep(plant, typeof(TangleNut))
            && plant.mTargetY > 0) --plant.mTargetY;
    }
    internal static void Crush(Plant plant)
    {
        var frame = WeedFusions.CurrentCrush;
        if (!ChomperFusions.Active(plant) || plant.mTargetY > 0 || frame == null
            || frame.Zombie.DataID != frame.ZombieId || frame.Board.Pointer != plant.mBoard.Pointer
            || frame.Zombie.mBoard?.Pointer != frame.Board.Pointer
            || frame.Zombie.mDead || frame.Zombie.IsDeadOrDying() || !frame.Zombie.mInPool) return;
        plant.mTargetY = 2;
        KelpEffects.Add(frame.Zombie, 100);
        plant.mPlantHealth -= PotDefense.Damage(plant, 50);
        if (plant.mPlantHealth <= 0) plant.Die();
        else { plant.UpdateReanim(); plant.UpdateReanimColor(); }
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class TangleNutInitializePatch
{
    private static void Postfix(Plant __instance) => TangleNut.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateTanglekelp))]
internal static class TangleNutAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!TangleNut.Is(__instance)) return true;
        TangleNut.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.CanTargetPlant))]
internal static class EdibleKelpTargetPatch
{
    // Native CanTargetPlant explicitly rejects Tanglekelp before its ordinary
    // terrain, pumpkin and chew rules. Use an edible aquatic plant, with no
    // fusion marker, for that query. Preserve identity across nested queries
    // and exceptions; never restore it into a recycled plant slot.
    internal static void Prefix(Plant thePlant,
        out (Plant? Plant, uint Id, SeedType Seed, SeedType Marker) __state)
    {
        __state = (null, 0, SeedType.None, SeedType.None);
        if (thePlant != null && (TangleNut.Is(thePlant) || GrappleKelp.Is(thePlant)))
        {
            __state = (thePlant, thePlant.DataID, thePlant.mSeedType, thePlant.mImitaterType);
            thePlant.mSeedType = SeedType.Seashroom;
            thePlant.mImitaterType = SeedType.None;
        }
    }
    internal static void Finalizer((Plant? Plant, uint Id, SeedType Seed, SeedType Marker) __state)
    {
        if (__state.Plant == null || __state.Plant.DataID != __state.Id) return;
        __state.Plant.mSeedType = __state.Seed;
        __state.Plant.mImitaterType = __state.Marker;
    }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class TangleNutBitePatch
{
    private static void Prefix(Zombie __instance, Plant thePlant,
        out (Plant? Plant, uint Eater, uint PlantId, int Health) __state) =>
        __state = thePlant != null && TangleNut.Is(thePlant) && ChomperFusions.Active(thePlant)
            ? (thePlant, __instance.DataID, thePlant.DataID, thePlant.mPlantHealth) : (null, 0, 0, 0);
    private static void Postfix(Zombie __instance, (Plant? Plant, uint Eater, uint PlantId, int Health) __state)
    {
        var plant = __state.Plant;
        if (plant != null && plant.DataID == __state.PlantId && __instance.DataID == __state.Eater
            && plant.mBoard?.Pointer == __instance.mBoard?.Pointer && plant.mPlantHealth < __state.Health)
            KelpEffects.Add(__instance, 500);
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class TangleNutCrushPatch
{
    private static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal || !TangleNut.Is(__instance)) return true;
        TangleNut.Crush(__instance); return false;
    }
}
