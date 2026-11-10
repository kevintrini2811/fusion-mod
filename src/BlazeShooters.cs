using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native shooters own targeting, shot counts, collision and the saved launch
// phase. JalaPeashooter selects bullet 175; Bullet_pea_jala heats BEFORE damage.
internal static class BlazeShooters
{
    internal sealed record Shot(IntPtr Board, uint Id);
    internal sealed record Impact(Zombie Zombie, IntPtr Board, uint Id);
    private static readonly Dictionary<IntPtr, Shot> Shots = new();
    [ThreadStatic] internal static Plant? FiringPlant;
    [ThreadStatic] internal static Impact? CurrentImpact;
    [ThreadStatic] internal static bool DamagePending;

    internal static bool Contains(Projectile projectile) => Shots.TryGetValue(projectile.Pointer, out var shot)
        && shot.Id == projectile.DataID && shot.Board == projectile.mBoard?.Pointer;

    internal static void Track(Projectile projectile)
    {
        var plant = FiringPlant;
        if (plant == null || !FusionPlant.IsBlazeShooter(FusionPlant.From(plant))
            || projectile.mDead || projectile.mBoard == null || plant.mBoard?.Pointer != projectile.mBoard.Pointer
            || projectile.mProjectileType is not (ProjectileType.Pea or ProjectileType.PeashooterPea)
            || Contains(projectile)) return;
        Shots[projectile.Pointer] = new(projectile.mBoard.Pointer, projectile.DataID);
        ImportedPeaVisuals.RegisterBlazeProjectile(projectile);
    }

    internal static Impact? BeginImpact(Projectile projectile, Zombie? zombie)
    {
        if (!Contains(projectile) || projectile.mDead || zombie == null || zombie.mDead
            || zombie.IsDeadOrDying() || zombie.mMindControlled
            || zombie.mBoard?.Pointer != projectile.mBoard.Pointer) return null;
        Overheat.Apply(zombie);
        return new(zombie, projectile.mBoard.Pointer, zombie.DataID);
    }

    internal static void Forget(Projectile projectile)
    {
        Shots.Remove(projectile.Pointer);
        ImportedPeaVisuals.ForgetProjectile(projectile);
    }
    internal static void ForgetBoard(Board board)
    {
        foreach (var key in Shots.Where(p => p.Value.Board == board.Pointer).Select(p => p.Key).ToArray())
            Shots.Remove(key);
    }
    internal static void Clear() => Shots.Clear();
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class BlazeFireScope
{
    internal static void Prefix(Plant __instance, out Plant? __state)
    {
        __state = BlazeShooters.FiringPlant;
        BlazeShooters.FiringPlant = FusionPlant.IsBlazeShooter(FusionPlant.From(__instance)) ? __instance : null;
    }
    internal static void Finalizer(Plant? __state) => BlazeShooters.FiringPlant = __state;
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ProjectileInitialize))]
internal static class BlazeProjectileInitializedPatch
{
    internal static void Postfix(Projectile __instance) => BlazeShooters.Track(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class BlazeImpactScope
{
    internal static void Prefix(Projectile __instance, Zombie theZombie,
        out (BlazeShooters.Impact? Impact, bool Pending) __state)
    {
        __state = (BlazeShooters.CurrentImpact, BlazeShooters.DamagePending);
        BlazeShooters.CurrentImpact = BlazeShooters.BeginImpact(__instance, theZombie);
        BlazeShooters.DamagePending = BlazeShooters.CurrentImpact != null;
    }
    internal static void Finalizer((BlazeShooters.Impact? Impact, bool Pending) __state)
    { BlazeShooters.CurrentImpact = __state.Impact; BlazeShooters.DamagePending = __state.Pending; }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class BlazeDamagePatch
{
    [HarmonyPriority(Priority.First)]
    internal static void Prefix(Zombie __instance, ref int theDamage)
    {
        var impact = BlazeShooters.CurrentImpact;
        if (!BlazeShooters.DamagePending || impact == null || impact.Zombie.Pointer != __instance.Pointer
            || impact.Id != __instance.DataID || impact.Board != __instance.mBoard?.Pointer) return;
        BlazeShooters.DamagePending = false;
        theDamage = 30;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class BlazeTorchwoodPatch
{
    // Source TorchWood accepts plain/snow peas only. Blaze ignition requires
    // SuperTorch, a separate unavailable result, never ordinary Torchwood.
    internal static bool Prefix(Projectile __instance) => !BlazeShooters.Contains(__instance);
}

[HarmonyPatch]
internal static class BlazeProjectileCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.Die)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.Reset)) };
    internal static void Prefix(Projectile __instance) => BlazeShooters.Forget(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class BlazeBoardCleanupPatch
{
    internal static void Prefix(Board __instance) => BlazeShooters.ForgetBoard(__instance);
}
