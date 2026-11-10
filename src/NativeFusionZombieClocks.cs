using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Only summoned Fusion heads own this canonical source clock. Native Update
// still owns locomotion, statuses and body animation on every native step.
internal static class NativeFusionZombieClocks
{
    internal sealed record UpdateState(IntPtr Board, IntPtr Pointer, uint Id, int Phase, bool Advance, UpdateState? Previous);
    [ThreadStatic] private static UpdateState? current;
    internal sealed record ActionState(IntPtr Board, IntPtr Pointer, uint Id, int Phase);

    private static bool Owns(Zombie zombie) => HypnoShooters.IsSummoned(zombie)
        && !zombie.mDead && zombie.mZombiePhase == ZombiePhase.ZombieNormal
        && ScaredContent.Playing(zombie.mBoard) && !zombie.mBoard.mPaused;

    internal static UpdateState? BeforeUpdate(Zombie zombie)
    {
        if (!Owns(zombie)) return null;
        return current = new(zombie.mBoard.Pointer, zombie.Pointer, zombie.DataID, zombie.mPhaseCounter,
            FusionTime.SourceStep(zombie, typeof(NativeFusionZombieClocks)), current);
    }

    internal static void AfterUpdate(Zombie zombie, UpdateState? state)
    {
        if (state == null) return;
        current = state.Previous;
        if (state.Advance || zombie.Pointer != state.Pointer || zombie.DataID != state.Id
            || zombie.mBoard?.Pointer != state.Board || !Owns(zombie)) return;
        // The native writer at 0x706595 decrements a positive head phase unless
        // frozen or buttered. Undo exactly that write; preserve unrelated writes.
        if (state.Phase > 0 && zombie.mPhaseCounter == state.Phase - 1)
            zombie.mPhaseCounter = state.Phase;
    }

    internal static ActionState? BeforeActions(Zombie zombie)
    {
        if (!Owns(zombie)) return null;
        bool advance = current is { } scope && scope.Pointer == zombie.Pointer && scope.Id == zombie.DataID
            && scope.Board == zombie.mBoard.Pointer
            ? scope.Advance : FusionTime.IsSourcePulse(zombie.mBoard);
        if (advance) return null;
        var state = new ActionState(zombie.mBoard.Pointer, zombie.Pointer, zombie.DataID, zombie.mPhaseCounter);
        // Both the inline and standalone head branch compare 0 (fire) and 35
        // (pose). A scoped negative sentinel bypasses only those head events.
        zombie.mPhaseCounter = -1;
        return state;
    }

    internal static void AfterActions(Zombie zombie, ActionState? state)
    {
        if (state != null && zombie.Pointer == state.Pointer && zombie.DataID == state.Id
            && zombie.mBoard?.Pointer == state.Board && zombie.mPhaseCounter == -1)
            zombie.mPhaseCounter = state.Phase;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.Update))]
internal static class NativeFusionZombieUpdatePatch
{
    [HarmonyPriority(Priority.First + 3000)]
    internal static void Prefix(Zombie __instance, out NativeFusionZombieClocks.UpdateState? __state) =>
        __state = NativeFusionZombieClocks.BeforeUpdate(__instance);
    internal static void Finalizer(Zombie __instance, NativeFusionZombieClocks.UpdateState? __state) =>
        NativeFusionZombieClocks.AfterUpdate(__instance, __state);
}

[HarmonyPatch]
internal static class NativeFusionZombieActionPatch
{
    internal static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateActions)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateZombiePeaHead))
    };
    [HarmonyPriority(Priority.First + 3000)]
    internal static void Prefix(Zombie __instance, out NativeFusionZombieClocks.ActionState? __state) =>
        __state = NativeFusionZombieClocks.BeforeActions(__instance);
    internal static void Finalizer(Zombie __instance, NativeFusionZombieClocks.ActionState? __state) =>
        NativeFusionZombieClocks.AfterActions(__instance, __state);
}
