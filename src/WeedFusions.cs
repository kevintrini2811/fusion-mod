using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Keep native low targeting, attack rectangles, one-second cadence, vehicle
// damage, terrain and saved attack state. UpdateAbilities inlines UpdateSpikeweed
// but both paths call DoRowAreaDamage at the actual contact event.
internal static class WeedFusions
{
    internal static int RockDamageStage(int health, int maximum) => maximum <= 0 ? 0
        : health <= maximum / 3 ? 2 : health <= (long)maximum * 2 / 3 ? 1 : 0;

    internal sealed record Attack(Board Board, uint PlantId, FusionPlant Identity);
    internal sealed record Crush(Zombie Zombie, Board Board, uint ZombieId);
    [ThreadStatic] internal static Attack? CurrentAttack;
    [ThreadStatic] internal static Crush? CurrentCrush;

    internal static bool Matches(Attack attack, Zombie zombie) =>
        zombie.mBoard?.Pointer == attack.Board.Pointer && !zombie.mDead
        && !zombie.IsDeadOrDying() && !zombie.mMindControlled;

    internal static void AfterHit(Zombie zombie, Attack? attack, uint id)
    {
        if (attack == null || zombie.DataID != id || !Matches(attack, zombie)) return;
        if (attack.Identity == FusionPlant.Iceweed || attack.Identity == FusionPlant.Icerock)
        {
            zombie.ApplyChill(false);
            if (attack.Identity == FusionPlant.Icerock && zombie.mChilledCounter > 0)
                zombie.mChilledCounter = Math.Max(zombie.mChilledCounter, 1500);
            SnowPea.Hit(zombie, attack.Identity == FusionPlant.Icerock ? 5 : 10);
        }
        else if (attack.Identity == FusionPlant.Scorchweed || attack.Identity == FusionPlant.Hearthrock) Overheat.Apply(zombie);
        else if (attack.Identity == FusionPlant.TangleWeed)
            // SetKelped(2, land:true): shared permanent stacks, a two-second
            // slow, one-third threshold damage and no aquatic seed drop.
            KelpEffects.Add(zombie, 200, suppressSeed: true);
    }

    internal static bool CrushNut(Plant plant)
    {
        var crush = CurrentCrush;
        if (FusionPlant.From(plant) != FusionPlant.SpikeNut || !ChomperFusions.Active(plant)
            || crush == null || crush.Zombie.DataID != crush.ZombieId
            || crush.Board.Pointer != plant.mBoard.Pointer
            || crush.Zombie.mBoard?.Pointer != crush.Board.Pointer
            || crush.Zombie.mMindControlled || crush.Zombie.mDead || crush.Zombie.IsDeadOrDying()
            || crush.Zombie.mZombieType is not (ZombieType.Zamboni or ZombieType.Catapult)) return false;
        // Charge once at real contact, before destroying the vehicle. Giant
        // hammers and boss attacks retain their native full crushing behavior.
        if (!PumpkinFusions.AbsorbCrush(plant)) plant.mPlantHealth -= PotDefense.Damage(plant, 500);
        if (plant.mPlantHealth <= 0) plant.Die();
        crush.Zombie.TakeDamage(1800, DamageFlags.BypassesShield | DamageFlags.Spike);
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoRowAreaDamage))]
internal static class WeedAttackPatch
{
    internal static bool Prefix(Plant __instance, ref int theDamage, DamageFlags theDamageFlags,
        out WeedFusions.Attack? __state)
    {
        __state = WeedFusions.CurrentAttack;
        WeedFusions.CurrentAttack = null;
        var identity = FusionPlant.From(__instance);
        if (!FusionPlant.IsWeed(identity) || (theDamageFlags & DamageFlags.Spike) == 0) return true;
        if (!ChomperFusions.Active(__instance)) return false;
        WeedFusions.CurrentAttack = new(__instance.mBoard, __instance.DataID, identity);
        if (identity == FusionPlant.TangleWeed) theDamage = 20;
        else if (identity != FusionPlant.Solarweed) theDamage = 40;
        return true;
    }

    internal static void Postfix(Plant __instance)
    {
        var attack = WeedFusions.CurrentAttack;
        if (attack?.Identity != FusionPlant.Solarweed || __instance.DataID != attack.PlantId
            || __instance.mBoard?.Pointer != attack.Board.Pointer) return;
        // One pickup per attack animation, including an attack that pops a
        // tire; never one pickup per zombie or a direct balance credit.
        SolarShooter.DropSun(attack.Board, __instance.mX, __instance.mY, 5);
    }

    internal static void Finalizer(WeedFusions.Attack? __state) => WeedFusions.CurrentAttack = __state;
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class WeedHitPatch
{
    internal static void Prefix(Zombie __instance, DamageFlags theDamageFlags,
        out (WeedFusions.Attack? Attack, uint Id) __state)
    {
        var attack = WeedFusions.CurrentAttack;
        __state = ((theDamageFlags & DamageFlags.Spike) != 0 && attack != null
            && WeedFusions.Matches(attack, __instance) ? attack : null, __instance.DataID);
    }
    internal static void Postfix(Zombie __instance, (WeedFusions.Attack? Attack, uint Id) __state) =>
        WeedFusions.AfterHit(__instance, __state.Attack, __state.Id);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.SquishAllInSquare))]
internal static class SpikeNutVehicleScope
{
    internal static void Prefix(Zombie __instance, out WeedFusions.Crush? __state)
    {
        __state = WeedFusions.CurrentCrush;
        WeedFusions.CurrentCrush = __instance.mBoard == null ? null
            : new(__instance, __instance.mBoard, __instance.DataID);
    }
    internal static void Finalizer(WeedFusions.Crush? __state) => WeedFusions.CurrentCrush = __state;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class SpikeNutVehiclePatch
{
    internal static bool Prefix(Plant __instance) => !WeedFusions.CrushNut(__instance);
}
