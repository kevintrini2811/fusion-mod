using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class MelonShell
{
    internal sealed class Attack
    {
        internal readonly Plant Plant;
        internal readonly uint Id;
        internal Projectile? First;
        internal Attack(Plant plant) { Plant = plant; Id = plant.DataID; }
    }
    [ThreadStatic] internal static Attack? Current;

    internal static bool Lobber(FusionPlant identity) => identity == (FusionPlant)SeedType.Cabbagepult
        || identity == (FusionPlant)SeedType.Kernelpult || identity == (FusionPlant)SeedType.Melonpult
        || identity == (FusionPlant)SeedType.Wintermelon || FusionPlant.IsMixedLobber(identity)
        || FusionPlant.IsPuffLobber(identity) || FusionPlant.IsSilverLobber(identity)
        || FusionPlant.IsTriLobber(identity);

    internal static Attack? Begin(Plant plant) => Current != null || !ChomperFusions.Active(plant)
        || plant.mIsAsleep || !Lobber(FusionPlant.From(plant)) ? null : new(plant);

    internal static void Finish(Attack? frame)
    {
        if (frame?.First == null || frame.Plant.DataID != frame.Id || !ChomperFusions.Active(frame.Plant)) return;
        var shell = PumpkinSupport.Shell(frame.Plant, FusionPlant.MelonShell);
        if (shell == null) return;
        LobberProjectiles.FireShell(shell, frame.Plant, PuffLobbers.Target(frame.Plant));
    }
}

[HarmonyPatch]
internal static class MelonShellAttackPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Plant), nameof(Plant.Fire)),
        AccessTools.Method(typeof(PuffLobbers), nameof(PuffLobbers.Fire)),
        AccessTools.Method(typeof(SilverLobbers), nameof(SilverLobbers.Fire)),
        AccessTools.Method(typeof(TriLobbers), nameof(TriLobbers.Shoot)),
        AccessTools.Method(typeof(LobberProjectiles), nameof(LobberProjectiles.Fire)),
        AccessTools.Method(typeof(ElementalLobbers), nameof(ElementalLobbers.Fire)),
        AccessTools.Method(typeof(SquashLobbers), nameof(SquashLobbers.Fire)) };

    internal static void Prefix(object[] __args, object? __instance,
        out (MelonShell.Attack? Previous, MelonShell.Attack? Frame) __state)
    {
        var plant = __instance as Plant ?? __args[0] as Plant;
        var frame = plant == null ? null : MelonShell.Begin(plant);
        __state = (MelonShell.Current, frame);
        if (frame != null) MelonShell.Current = frame;
    }
    internal static void Postfix((MelonShell.Attack? Previous, MelonShell.Attack? Frame) __state)
    {
        if (__state.Frame == null) return;
        MelonShell.Current = __state.Previous;
        MelonShell.Finish(__state.Frame);
    }
    internal static void Finalizer((MelonShell.Attack? Previous, MelonShell.Attack? Frame) __state) =>
        MelonShell.Current = __state.Previous;
}

[HarmonyPatch]
internal static class PumpkinSupportProjectilePatch
{
    private static IEnumerable<MethodBase> TargetMethods() => typeof(Board).GetMethods()
        .Where(method => method.Name == nameof(Board.AddProjectile));
    internal static void Postfix(Board __instance, Projectile? __result)
    {
        if (__result == null || __result.mDead) return;
        if (MelonShell.Current is { } frame && frame.Plant.mBoard?.Pointer == __instance.Pointer)
            frame.First ??= __result;
        PumpkinSupport.CopiedShots?.Add(__result);
    }
}
