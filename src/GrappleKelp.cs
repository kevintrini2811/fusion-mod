using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class GrappleKelp
{
    // Controller exit times include overlapping .25-second transitions. The
    // sinusoid clip runs at 1.5 speed: its events land at .5, .8333 and 1.1667s.
    internal const int AttackTicks = 185;
    internal static bool Is(Plant plant) => FusionPlant.From(plant) == FusionPlant.GrappleKelp;
    internal static bool InRange(Plant plant, Zombie zombie)
    {
        if (zombie.mRow != plant.mRow || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Submerged)) return false;
        var rect = zombie.GetZombieRect();
        float x = (rect.xMin + rect.xMax) * .5f, origin = plant.mX + plant.mWidth * .5f;
        return x >= origin && x <= origin + 7 * ScaredyFusions.SourceUnit;
    }
    internal static void Initialize(Plant plant)
    {
        if (!Is(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mStartRow = plant.mTargetX = 0;
    }

    internal static bool InBiteRange(Plant plant, Zombie zombie)
    {
        // FindPlantTarget accepts 20 pixels of horizontal attack overlap.
        // Keep that contact intact until the native chew check runs (every
        // four ticks, or eight while chilled). mIsEating can still be false.
        var attack = zombie.GetZombieAttackRect();
        var body = plant.GetPlantRect();
        return Common.GetRectOverlap(ref attack, ref body) >= 20;
    }
    internal static void Tick(Plant plant)
    {
        SeaPot.Advance(plant, TickOnce);
    }
    private static void TickOnce(Plant plant)
    {
        if (!Is(plant) || !ChomperFusions.Active(plant)) return;
        if (plant.mTargetX > 0) --plant.mTargetX;
        if (plant.mStartRow == 0)
        {
            if (plant.mTargetX > 0) return;
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
                if (plant.mBoard.m_zombies[i] is { } zombie && InRange(plant, zombie))
                {
                    plant.mStartRow = AttackTicks;
                    plant.mTargetX = 285 + plant.mBoard.Rand(31);
                    return;
                }
            return;
        }
        int elapsed = AttackTicks - --plant.mStartRow;
        if (elapsed == 50) Hit(plant, 0);
        else if (elapsed == 83) Hit(plant, 1);
        else if (elapsed == 117) Hit(plant, 2);
    }
    internal static void Hit(Plant plant, int stage)
    {
        var board = plant.mBoard;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && InRange(plant, zombie)) targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (!ChomperFusions.Active(plant)) return;
            if (zombie.DataID != id || !InRange(plant, zombie)) continue;
            KelpEffects.Add(zombie, 1000);
            if (zombie.DataID != id || !InRange(plant, zombie)) continue;
            zombie.TakeDamage(30, DamageFlags.BypassesShield);
            if (zombie.DataID != id || !InRange(plant, zombie)) continue;
            if (stage == 0) KelpEffects.Add(zombie, 1000);
            else if (stage == 1)
                zombie.TakeDamage((int)Math.Min(int.MaxValue, (long)5 * KelpEffects.Stacks(zombie)), DamageFlags.BypassesShield);
            else if (!KelpEffects.Immune(zombie) && !InBiteRange(plant, zombie))
            {
                zombie.mPosX = Math.Min(850, zombie.mPosX + .2f * ScaredyFusions.SourceUnit);
                zombie.mX = (int)zombie.mPosX;
            }
        }
    }
    internal static (string Clip, float Time, string? Previous, float PreviousTime, float Blend) Animation(Plant plant, float idle)
    {
        if (plant.mStartRow <= 0) return ("idle", idle, null, 0, 1);
        float elapsed = (AttackTicks - plant.mStartRow) * .01f;
        if (elapsed < .5f) return ("attack", elapsed, elapsed < .25f ? "idle" : null, idle, elapsed / .25f);
        if (elapsed < 4f / 3)
            return ("f(x)=sinx", (elapsed - .5f) * 1.5f, elapsed < .75f ? "attack" : null,
                Math.Min(elapsed, .74999f), (elapsed - .5f) / .25f);
        return ("suohuiqu", elapsed - 4f / 3, elapsed < 4f / 3 + .25f ? "f(x)=sinx" : null,
            Math.Min((elapsed - .5f) * 1.5f, 1.49999f), (elapsed - 4f / 3) / .25f);
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class GrappleInitializePatch
{
    private static void Postfix(Plant __instance) => GrappleKelp.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class GrappleAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!GrappleKelp.Is(__instance)) return true;
        GrappleKelp.Tick(__instance); return false;
    }
}
