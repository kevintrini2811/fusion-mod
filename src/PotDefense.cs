using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Observe actual native health writers. Synchronous hit frames are not saved.
// Fractional damage belongs to the support's saved, otherwise unused mTargetY.
internal static class PotDefense
{
    internal sealed class Hit
    {
        internal readonly Plant Plant, Pot;
        internal readonly uint Id, PotId;
        internal readonly int Health, Numerator, Denominator;
        internal bool Applied;
        internal Hit(Plant plant, Plant pot, int numerator, int denominator)
        {
            Plant = plant; Pot = pot; Id = plant.DataID; PotId = pot.DataID;
            Health = plant.mPlantHealth; Numerator = numerator; Denominator = denominator;
        }
        internal bool Valid => Plant.DataID == Id && Pot.DataID == PotId
            && Plant.mBoard?.Pointer == Pot.mBoard?.Pointer;
        internal int Reduce(int damage)
        {
            long value = (long)damage * Numerator + Math.Clamp(Pot.mTargetY, 0, Denominator - 1);
            Pot.mTargetY = (int)(value % Denominator);
            return (int)(value / Denominator);
        }
        internal void Apply()
        {
            if (Applied || !Valid || Plant.mPlantHealth >= Health) return;
            Applied = true;
            Plant.mPlantHealth = Health - Reduce(Health - Plant.mPlantHealth);
        }
    }

    [ThreadStatic] internal static List<Hit>? Current;
    internal const int GarlicDirection = -1130;
    [ThreadStatic] internal static int? RowRandom;

    internal static Plant? Support(Plant plant, FusionPlant identity) => PotBoosts.FindSupport(plant, identity);

    internal static bool Garlic(FusionPlant identity) => identity == (FusionPlant)SeedType.Garlic
        || identity == FusionPlant.GarlicPot || identity == FusionPlant.SilverGarlic
        || identity == FusionPlant.FoulNut || identity == FusionPlant.FoulShroom
        || identity == FusionPlant.StenchSpreader || FusionPlant.IsPoisonShooter(identity);

    internal static Hit? Capture(Plant plant, bool bite)
    {
        if (plant.mPlantHealth <= 0 || Current?.Any(hit => hit.Plant.Pointer == plant.Pointer) == true) return null;
        // One source garlic bite costs exactly ten HP and starts disgust.
        if (bite && Garlic(FusionPlant.From(plant))
            && Support(plant, FusionPlant.GarlicPot) is { } garlic) return new(plant, garlic, 0, 1);
        if (FusionPlant.From(plant) == FusionPlant.SpikeNut
            && Support(plant, FusionPlant.SpikePot) is { } spike) return new(plant, spike, 1, 2);
        return Support(plant, FusionPlant.NutPot) is { } nut ? new(plant, nut, 4, 5) : null;
    }

    internal static void Apply(List<Hit>? hits)
    { if (hits != null) foreach (var hit in hits) hit.Apply(); }

    internal static int Damage(Plant plant, int damage)
    {
        var hit = Capture(plant, false);
        if (hit == null) return damage;
        return hit.Reduce(damage);
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class PotDefenseBitePatch
{
    [HarmonyPriority(Priority.First)]
    private static void Prefix(Plant thePlant, out List<PotDefense.Hit>? __state)
    {
        __state = PotDefense.Current;
        PotDefense.Current = thePlant != null && PotDefense.Capture(thePlant, true) is { } hit
            ? new() { hit } : null;
    }
    [HarmonyPriority(Priority.First)]
    private static void Postfix(Zombie __instance)
    {
        var hits = PotDefense.Current;
        PotDefense.Apply(hits);
        if (hits == null || __instance.mDead || __instance.mMindControlled) return;
        foreach (var hit in hits)
        {
            if (!hit.Applied || hit.Numerator != 0 || !hit.Valid || !ChomperFusions.Active(hit.Plant)) continue;
            hit.Plant.mPlantHealth -= 10;
            hit.Plant.mEatenFlashCountdown = 50;
            __instance.StopEating();
            __instance.mYuckyFace = true;
            __instance.mYuckyFaceCounter = 0;
            __instance.mTargetRow = PotDefense.GarlicDirection;
            __instance.UpdateAnimSpeed();
            if (hit.Plant.mPlantHealth <= 0) { ++hit.Plant.mBoard.mPlantsEaten; hit.Plant.Die(); }
        }
    }
    private static void Finalizer(List<PotDefense.Hit>? __state) => PotDefense.Current = __state;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class PotDefenseDeathPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal || PotDefense.Current == null) return true;
        var hit = PotDefense.Current.FirstOrDefault(hit => hit.Plant.Pointer == __instance.Pointer && !hit.Applied);
        if (hit == null || !hit.Valid || __instance.mPlantHealth > 0) return true;
        hit.Apply();
        if (__instance.mPlantHealth <= 0) return true;
        // EatPlant increments this immediately before its lethal Die call.
        --__instance.mBoard.mPlantsEaten;
        return false;
    }
}

[HarmonyPatch]
internal static class PotDefenseProjectilePatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.CheckForCollision)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.UpdateLobMotion)) };
    private static void Prefix(Projectile __instance, out List<PotDefense.Hit>? __state)
    {
        __state = null;
        if (__instance.mDead || __instance.mBoard == null
            || __instance.mProjectileType is not (ProjectileType.ZombiePea or ProjectileType.Basketball)) return;
        __state = new();
        var plants = __instance.mBoard.m_plants;
        for (int i = 0; i < plants.Count; ++i)
            if (plants[i] is { } plant && PotDefense.Capture(plant, false) is { } hit) __state.Add(hit);
    }
    [HarmonyPriority(Priority.First)]
    private static void Postfix(List<PotDefense.Hit>? __state) => PotDefense.Apply(__state);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateYuckyFace))]
internal static class GarlicPotRowScope
{
    private static void Prefix(Zombie __instance, out int? __state)
    {
        __state = PotDefense.RowRandom;
        PotDefense.RowRandom = null;
        if (__instance.mTargetRow != PotDefense.GarlicDirection
            || __instance.mYuckyFaceCounter is < 20 or > 169 || __instance.mZombieType == ZombieType.Boss) return;
        int row = __instance.mRow, last = __instance.mBoard.GetNumRows() - 1;
        int above = Math.Abs(2 * (row - 1) - last), below = Math.Abs(2 * (row + 1) - last);
        // Native code checks adjacent rows and water compatibility first.
        // It draws this value only when both adjacent rows are legal.
        if (above != below) PotDefense.RowRandom = above < below ? 1 : 0;
    }
    private static void Postfix(Zombie __instance)
    {
        if (__instance.mTargetRow == PotDefense.GarlicDirection
            && (__instance.mYuckyFaceCounter == 0 || __instance.mYuckyFaceCounter >= 170))
            __instance.mTargetRow = -1;
    }
    private static void Finalizer(int? __state) => PotDefense.RowRandom = __state;
}

[HarmonyPatch]
internal static class GarlicPotRowRandomPatch
{
    private static MethodBase TargetMethod() => typeof(Common).GetMethod(nameof(Common.Rand), new[] { typeof(int) })!;
    private static bool Prefix(int range, ref int __result)
    {
        if (range != 2 || PotDefense.RowRandom is not { } value) return true;
        __result = value;
        return false;
    }
}
