using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class SnowPea
{
    private sealed class FreezeState
    {
        internal IntPtr Board;
        internal uint ZombieId;
        internal int Points;
        internal int Threshold = 100;
    }

    internal sealed class Impact
    {
        internal IntPtr Zombie;
        internal bool Multiply;
        internal bool Pending = true;
    }

    private static readonly Dictionary<IntPtr, FreezeState> States = new();
    [ThreadStatic] internal static Impact? CurrentImpact;
    [ThreadStatic] internal static IntPtr CreatingIceTrap;

    internal static (int Points, int Threshold)? Capture(Zombie zombie) =>
        States.TryGetValue(zombie.Pointer, out var state) && state.ZombieId == zombie.DataID
            && state.Board == zombie.mBoard?.Pointer ? (state.Points, state.Threshold) : null;
    internal static void Restore(Zombie zombie, int points, int threshold)
    {
        if (Capture(zombie) != null || zombie.mBoard == null || zombie.mDead || zombie.IsDeadOrDying()) return;
        int limit = Math.Clamp(threshold, 100, 1000);
        States[zombie.Pointer] = new() { Board = zombie.mBoard.Pointer, ZombieId = zombie.DataID,
            Points = Math.Clamp(points, 0, limit - 1), Threshold = limit };
    }

    internal static void Hit(Zombie zombie, int points = 10)
    {
        if (zombie.mDead || zombie.IsDeadOrDying() || zombie.mBoard == null
            || !zombie.CanBeFrozen() || zombie.mIceTrapCounter > 0) return;
        uint id = zombie.DataID;
        if (!States.TryGetValue(zombie.Pointer, out var state)
            || state.ZombieId != id || state.Board != zombie.mBoard.Pointer)
            States[zombie.Pointer] = state = new FreezeState
            {
                Board = zombie.mBoard.Pointer, ZombieId = id
            };

        state.Points += points;
        if (state.Points < state.Threshold) return;
        state.Points = 0;
        state.Threshold = Math.Min(1000, state.Threshold + 100);

        // HitIceTrap owns freeze immunity, animation rate, controller and sound.
        // Its independent 20 Ice-shroom damage must not become an extra pea hit.
        FusionStatuses.Freeze(zombie);
    }

    internal static void ClearPoints(Zombie zombie)
    {
        if (States.TryGetValue(zombie.Pointer, out var state)) state.Points = 0;
    }

    internal static void Forget(Zombie zombie) => States.Remove(zombie.Pointer);

    internal static void ForgetBoard(Board board)
    {
        foreach (var key in States.Where(entry => entry.Value.Board == board.Pointer)
                     .Select(entry => entry.Key).ToArray())
            States.Remove(key);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class SnowPeaImpactPatch
{
    private static void Prefix(Projectile __instance, Zombie theZombie, out SnowPea.Impact? __state)
    {
        __state = SnowPea.CurrentImpact;
        SnowPea.CurrentImpact = null;
        // Check the current projectile type: Torchwood-thawed peas stay ordinary.
        // No owner ledger is needed, so existing and loaded Snow Peas work too.
        if (__instance.mDead || __instance.mProjectileType != ProjectileType.Snowpea || theZombie == null
            || theZombie.mDead || theZombie.IsDeadOrDying()) return;
        SnowPea.Hit(theZombie);
        SnowPea.CurrentImpact = new SnowPea.Impact
        {
            Zombie = theZombie.Pointer, Multiply = theZombie.mIceTrapCounter > 0
                || PuffSpecials.IsIcicle(__instance) && !theZombie.CanBeChilled()
        };
    }

    private static void Finalizer(SnowPea.Impact? __state) => SnowPea.CurrentImpact = __state;
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class SnowPeaDamagePatch
{
    private static bool Prefix(Zombie __instance, ref int theDamage)
    {
        if (SnowPea.CreatingIceTrap == __instance.Pointer) return false;
        var impact = SnowPea.CurrentImpact;
        if (impact == null || !impact.Pending || impact.Zombie != __instance.Pointer) return true;
        impact.Pending = false;
        if (impact.Multiply) theDamage = checked(theDamage * 4);
        return true;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.HitIceTrap))]
internal static class FusionFreezeDurationPatch
{
    // Clearing Cryo points is event bookkeeping. Native freezes retain their
    // native durations; only FusionStatuses.Freeze supplies Fusion durations.
    private static void Postfix(Zombie __instance) => SnowPea.ClearPoints(__instance);
}

[HarmonyPatch]
internal static class SnowPeaThawPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Zombie), nameof(Zombie.RemoveIceTrap));
        yield return AccessTools.Method(typeof(Zombie), nameof(Zombie.RemoveColdEffects));
    }

    private static void Postfix(Zombie __instance) => SnowPea.ClearPoints(__instance);
}

[HarmonyPatch]
internal static class SnowPeaZombieCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Zombie), nameof(Zombie.DieNoLoot));
        yield return AccessTools.Method(typeof(Zombie), nameof(Zombie.DieWithLoot));
        yield return AccessTools.Method(typeof(Zombie), nameof(Zombie.Reset));
    }

    private static void Prefix(Zombie __instance) => SnowPea.Forget(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class SnowPeaBoardCleanupPatch
{
    private static void Prefix(Board __instance) => SnowPea.ForgetBoard(__instance);
}
