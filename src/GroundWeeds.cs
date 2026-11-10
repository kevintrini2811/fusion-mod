using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// These variants own their contact events. In particular, zero damage must
// never enter native Spikeweed's independent tire-destruction branch.
internal static class GroundWeeds
{
    internal const int AttackTicks = 67;
    internal static bool Target(Plant plant, Zombie zombie)
    {
        if (zombie.mRow != plant.mRow || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Dog)) return false;
        var attack = plant.GetPlantAttackRect(PlantWeapon.Primary);
        var rect = zombie.GetZombieRect();
        return rect.xMax >= attack.xMin && rect.xMin <= attack.xMax;
    }

    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsGroundWeed(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchCounter = plant.mStartRow = 0;
        plant.mTargetY = 300;
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant)) return;
        if (!FusionTime.SourceStep(plant, typeof(GroundWeeds))) return;
        bool butter = FusionPlant.From(plant) == FusionPlant.Butterweed;
        if (!butter)
            foreach (var (zombie, id) in Targets(plant))
                if (zombie.DataID == id && zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult)
                {
                    // Caltrop.KillCar owns an immediate vehicle kill, independent
                    // of its normal attack. CornCaltrop deliberately lacks it.
                    zombie.DieWithLoot();
                    plant.Die();
                    return;
                }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (butter && --plant.mTargetY <= 0)
        {
            plant.mTargetY = 300;
            foreach (var (zombie, id) in Targets(plant))
                if (zombie.DataID == id && Target(plant, zombie)) ButterEffects.Apply(zombie, 100);
        }
        if (plant.mStartRow == 0 && plant.mLaunchCounter == 0 && Targets(plant).Count != 0)
        {
            plant.mLaunchCounter = butter ? 100 : 150;
            plant.mStartRow = AttackTicks;
        }
        if (plant.mStartRow <= 0) return;
        int elapsed = AttackTicks - --plant.mStartRow;
        if (butter || elapsed != 42) return;
        foreach (var (zombie, id) in Targets(plant))
        {
            if (!ChomperFusions.Active(plant)) return;
            if (zombie.DataID != id || !Target(plant, zombie)) continue;
            zombie.TakeDamage(80, DamageFlags.BypassesShield);
            if (zombie.DataID == id && Target(plant, zombie)) SpringEffects.Launch(zombie);
        }
    }

    private static List<(Zombie Zombie, uint Id)> Targets(Plant plant)
    {
        var targets = new List<(Zombie, uint)>();
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && Target(plant, zombie))
                targets.Add((zombie, zombie.DataID));
        return targets;
    }

    internal static bool Slowed(Zombie zombie)
    {
        if (zombie.mBoard == null || !KelpEffects.Playing(zombie.mBoard)) return false;
        for (int i = 0; i < zombie.mBoard.m_plants.Count; ++i)
            if (zombie.mBoard.m_plants[i] is { } plant && FusionPlant.From(plant) == FusionPlant.Butterweed
                && ChomperFusions.Active(plant) && Target(plant, zombie)) return true;
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class GroundWeedInitializePatch
{ private static void Postfix(Plant __instance) => GroundWeeds.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class GroundWeedAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsGroundWeed(FusionPlant.From(__instance))) return true;
        GroundWeeds.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateSpikeweed))]
internal static class GroundWeedNativeAttackPatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsGroundWeed(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Zombie), nameof(Zombie.CanTargetPlant))]
internal static class GroundFusionTargetPatch
{
    internal static void Postfix(Zombie __instance, Plant thePlant, ref bool __result)
    {
        if (thePlant == null) return;
        var identity = FusionPlant.From(thePlant);
        if (identity == FusionPlant.SeaNut && thePlant.mStartRow >= 3
            || identity == FusionPlant.Butterweed && __instance.mZombieType is ZombieType.Zamboni or ZombieType.Catapult)
            __result = false;
    }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateZombieWalking))]
internal static class ButterweedWalkingPatch
{
    private static void Prefix(Zombie __instance, out (uint Id, float X, float Y, bool Slow) __state) =>
        __state = (__instance.DataID, __instance.mPosX, __instance.mPosY, GroundWeeds.Slowed(__instance));
    private static void Postfix(Zombie __instance, (uint Id, float X, float Y, bool Slow) __state)
    {
        if (!__state.Slow || __instance.DataID != __state.Id || __instance.mDead) return;
        __instance.mPosX = (__state.X + __instance.mPosX) * .5f;
        __instance.mPosY = (__state.Y + __instance.mPosY) * .5f;
        __instance.mX = (int)__instance.mPosX; __instance.mY = (int)__instance.mPosY;
    }
}
