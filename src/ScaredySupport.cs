using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class ScaredySupport
{
    // Half-ticks preserve ScaredySun's continuous interval reduction of .5 s/s.
    // Fear resets the interval, not the running production or brightening.
    internal static void ProduceTick(Plant plant, bool near)
    {
        plant.mTargetX = near ? 5000 : Math.Max(2000, plant.mTargetX - 1);
        if (plant.mShootingCounter > 0 && --plant.mShootingCounter == 60)
            plant.mBoard.AddCoin(plant.mX, plant.mY, CoinType.Sun, CoinMotion.FromPlant);
        if (plant.mLaunchCounter != 0 || plant.mShootingCounter != 0) return;
        plant.mShootingCounter = 360;
        int interval = plant.mTargetX / 2;
        int spread = interval / 20;
        plant.mLaunchCounter = interval - spread + plant.mBoard.Rand(2 * spread + 1);
    }

    internal static float Brightness(Plant plant) => plant.mShootingCounter <= 0 ? 1
        : plant.mShootingCounter > 60 ? 1 + (360 - plant.mShootingCounter) * .01f
        : 1 + plant.mShootingCounter * .05f;

    internal static bool SturdyTick(Plant plant, bool near)
    {
        if (plant.mTargetX < 0) ++plant.mTargetX;
        else if (!near) plant.mTargetX = 0;
        else if (++plant.mTargetX > 1000)
        {
            plant.mTargetX = -500;
            plant.mState = PlantState.ScaredyshroomRaising;
            plant.mTargetY = 100;
            // ScaredyNut invokes Action after .5 s, rather than via an event.
            plant.mStartRow = 51;
        }
        if (plant.mStartRow > 0 && --plant.mStartRow == 0) Repel(plant);
        return plant.mDead;
    }

    private static void Repel(Plant plant)
    {
        var board = plant.mBoard;
        uint plantId = plant.DataID;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && ScaredyFusions.Near(plant, zombie))
                targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (plant.DataID != plantId || plant.mBoard?.Pointer != board.Pointer || !ScaredyFusions.Active(plant)) break;
            if (zombie.DataID != id || !ScaredyFusions.Near(plant, zombie)) continue;
            zombie.TakeDamage(20, DamageFlags.HitsShieldAndBody);
            if (plant.DataID != plantId || plant.mBoard?.Pointer != board.Pointer || !ScaredyFusions.Active(plant)) break;
            if (zombie.DataID != id || !ScaredyFusions.Near(plant, zombie)) continue;
            // Match the existing umbrella knockback adapter, using the source
            // ScaredyNut distance of two units instead of three.
            zombie.mPosX = Math.Min(850, zombie.mPosX + 2 * ScaredyFusions.SourceUnit);
            zombie.mX = (int)zombie.mPosX;
        }
    }

    internal static bool BiteProtected(Plant plant)
    {
        if (!ChomperFusions.Active(plant)) return false;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.CamoShroom) return true;
        // The source predicate also protects ordinary scaredy mushrooms and
        // ScaredySun, but only while their scared animator flag is set.
        if (plant.mSeedType != SeedType.Scaredyshroom || !ScaredyFusions.Afraid(plant)) return false;
        for (int i = 0; i < plant.mBoard.m_plants.Count; ++i)
        {
            var camo = plant.mBoard.m_plants[i];
            if (camo != null && FusionPlant.From(camo) == FusionPlant.CamoShroom
                && ScaredyFusions.Active(camo) && Math.Abs(camo.mRow - plant.mRow) <= 1
                && Math.Abs(camo.mPlantCol - plant.mPlantCol) <= 1) return true;
        }
        return false;
    }

    internal sealed class Hit
    {
        internal readonly Plant Plant;
        internal readonly Board Board;
        internal readonly uint Id;
        internal readonly int Health;
        internal readonly bool Bite;
        internal bool Applied;
        internal Hit(Plant plant, bool bite)
        { Plant = plant; Board = plant.mBoard; Id = plant.DataID; Health = plant.mPlantHealth; Bite = bite; }
    }

    [ThreadStatic] internal static List<Hit>? Hits;

    internal static Hit? Capture(Plant plant, bool bite) =>
        FusionPlant.From(plant) == FusionPlant.SturdyShroom && ScaredyFusions.Active(plant)
        && ScaredyFusions.Afraid(plant) && plant.mPlantHealth > 0
        && !(Hits?.Any(h => h.Plant.Pointer == plant.Pointer && h.Id == plant.DataID) ?? false)
        ? new(plant, bite) : null;

    internal static void Apply(Hit hit)
    {
        var plant = hit.Plant;
        if (hit.Applied || plant.DataID != hit.Id || plant.mBoard?.Pointer != hit.Board.Pointer) return;
        hit.Applied = true;
        if (plant.mPlantHealth < hit.Health)
            plant.mPlantHealth = hit.Health - (int)(((long)hit.Health - plant.mPlantHealth) / 2);
    }

    internal static bool BeforeDeath(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.SturdyShroom && ChomperFusions.Active(plant)
            && PlayerPlantingScope.ConsumingIngredient != plant.Pointer
            && RetaliationNuts.JackBoard == plant.mBoard.Pointer
            && RetaliationNuts.RadiusBoard == plant.mBoard.Pointer && plant.mPlantHealth > 0)
        {
            // Native jack explosions remove plants directly. As with the
            // existing Iron Squash adapter, translate that path into the
            // source's 1800 damage before deciding whether this plant dies.
            plant.mPlantHealth -= ScaredyFusions.Afraid(plant) ? 900 : 1800;
            if (plant.mPlantHealth > 0) { --plant.mBoard.mPlantsEaten; return false; }
        }
        var hit = Hits?.FirstOrDefault(h => h.Plant.Pointer == plant.Pointer && h.Id == plant.DataID);
        if (hit == null || hit.Applied || plant.mPlantHealth > 0) return true;
        Apply(hit);
        if (plant.mPlantHealth <= 0) return true;
        // EatPlant increments before Die. Restore only the suppressed death.
        if (hit.Bite) --hit.Board.mPlantsEaten;
        return false;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.CanTargetPlant))]
internal static class CamoTargetPatch
{
    internal static void Postfix(Plant thePlant, ZombieAttackType theAttackType, ref bool __result)
    {
        if (thePlant == null || !__result) return;
        if (FusionPlant.From(thePlant) == FusionPlant.CamoShroom
            || theAttackType == ZombieAttackType.Chew && ScaredySupport.BiteProtected(thePlant)) __result = false;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class ScaredySupportBitePatch
{
    internal static bool Prefix(Plant thePlant, out List<ScaredySupport.Hit>? __state)
    {
        __state = ScaredySupport.Hits;
        if (thePlant == null) return true;
        if (ScaredySupport.BiteProtected(thePlant)) return false;
        var hit = ScaredySupport.Capture(thePlant, true);
        if (hit != null) ScaredySupport.Hits = new() { hit };
        return true;
    }
    internal static void Postfix(List<ScaredySupport.Hit>? __state)
    { if (ScaredySupport.Hits != null && !ReferenceEquals(ScaredySupport.Hits, __state))
        foreach (var hit in ScaredySupport.Hits) ScaredySupport.Apply(hit); }
    internal static void Finalizer(List<ScaredySupport.Hit>? __state) => ScaredySupport.Hits = __state;
}

[HarmonyPatch]
internal static class SturdyProjectileDamagePatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.CheckForCollision)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.UpdateLobMotion)) };
    internal static void Prefix(Projectile __instance, out List<ScaredySupport.Hit>? __state)
    {
        __state = ScaredySupport.Hits;
        if (__instance.mDead || __instance.mBoard == null
            || __instance.mProjectileType is not (ProjectileType.ZombiePea or ProjectileType.Basketball)) return;
        var hits = new List<ScaredySupport.Hit>();
        for (int i = 0; i < __instance.mBoard.m_plants.Count; ++i)
            if (__instance.mBoard.m_plants[i] is { } plant && ScaredySupport.Capture(plant, false) is { } hit)
                hits.Add(hit);
        if (hits.Count > 0) ScaredySupport.Hits = hits;
    }
    internal static void Postfix(List<ScaredySupport.Hit>? __state)
    { if (ScaredySupport.Hits != null && !ReferenceEquals(ScaredySupport.Hits, __state))
        foreach (var hit in ScaredySupport.Hits) ScaredySupport.Apply(hit); }
    internal static void Finalizer(List<ScaredySupport.Hit>? __state) => ScaredySupport.Hits = __state;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class SturdyLethalDamagePatch
{
    [HarmonyPriority(Priority.Last)]
    internal static bool Prefix(Plant __instance, bool __runOriginal) =>
        !__runOriginal || ScaredySupport.BeforeDeath(__instance);
}
