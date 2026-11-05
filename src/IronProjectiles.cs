using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Keep native targeting, flight, collision, impact effects and lifetime.
// Only the scoped hit damage and displacement belong to the iron projectile.
internal static class IronProjectiles
{
    internal const int SmallShot = -480090;
    internal static bool IsSmall(Projectile projectile) => projectile.mCobTargetX == SmallShot;
    private static readonly Dictionary<IntPtr, (IntPtr Board, uint Id)> shots = new();
    [ThreadStatic] internal static Plant? Firing;
    [ThreadStatic] internal static IntPtr Target;
    [ThreadStatic] internal static bool Pending;
    [ThreadStatic] internal static bool Red;
    internal static bool IsRed(Projectile projectile) => projectile.mCobTargetX == -450168;
    internal static bool Contains(Projectile projectile) => projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
        && (projectile.mCobTargetX is -450011 or -450168 or SmallShot || shots.TryGetValue(projectile.Pointer, out var shot)
        && shot.Board == projectile.mBoard?.Pointer && shot.Id == projectile.DataID);
    internal static void Track(Projectile projectile)
    {
        if (Firing == null || projectile.mBoard == null || projectile.mBoard.Pointer != Firing.mBoard?.Pointer || projectile.mDead
            || projectile.mProjectileType is not (ProjectileType.Pea or ProjectileType.PeashooterPea)) return;
        if (Contains(projectile)) return;
        if (FusionPlant.From(Firing) == FusionPlant.FootballGatling)
        {
            int count = (int)Firing.mTargetZombieID;
            count = count < 0 || count >= 11 ? 0 : count + 1;
            Firing.mTargetZombieID = count;
            projectile.mCobTargetX = count == 0 ? -450168 : -450011;
        }
        shots[projectile.Pointer] = (projectile.mBoard.Pointer, projectile.DataID);
        ImportedPeaVisuals.RegisterIronProjectile(projectile);
    }
    internal static void Forget(Projectile projectile)
    {
        shots.Remove(projectile.Pointer);
        ImportedPeaVisuals.ForgetProjectile(projectile);
    }
    internal static void ForgetBoard(Board board)
    {
        foreach (var key in shots.Where(e => e.Value.Board == board.Pointer).Select(e => e.Key).ToArray()) shots.Remove(key);
    }
    internal static void Clear() => shots.Clear();
    internal static void Knockback(Zombie zombie)
    {
        if (zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
            || !zombie.EffectedByDamage(DamageRangeFlags.Ground)
            || zombie.mZombieType is ZombieType.Boss or ZombieType.Zamboni or ZombieType.Catapult
                or ZombieType.Gargantuar or ZombieType.RedeyeGargantuar) return;
        zombie.mPosX = Math.Min(850, zombie.mPosX + 16);
        zombie.mX = (int)zombie.mPosX;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class IronFireScope
{
    private static void Prefix(Plant __instance, out Plant? __state)
    {
        __state = IronProjectiles.Firing;
        IronProjectiles.Firing = FusionPlant.From(__instance) is var identity
            && (identity == FusionPlant.Buckshooter || identity == FusionPlant.FootballGatling) ? __instance : null;
    }
    private static void Finalizer(Plant? __state) => IronProjectiles.Firing = __state;
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ProjectileInitialize))]
internal static class IronProjectileInitializePatch
{
    private static void Postfix(Projectile __instance) => IronProjectiles.Track(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class IronImpactScope
{
    private static void Prefix(Projectile __instance, Zombie theZombie, out (IntPtr Target, bool Pending, bool Red) __state)
    {
        __state = (IronProjectiles.Target, IronProjectiles.Pending, IronProjectiles.Red);
        IronProjectiles.Target = IntPtr.Zero;
        IronProjectiles.Pending = false;
        IronProjectiles.Red = IronProjectiles.IsRed(__instance);
        if (!IronProjectiles.Contains(__instance) || theZombie == null || theZombie.mDead || theZombie.IsDeadOrDying()) return;
        IronProjectiles.Target = theZombie.Pointer;
        IronProjectiles.Pending = true;
        if (IronProjectiles.Red) FootballPlants.Scare(theZombie);
    }
    private static void Finalizer((IntPtr Target, bool Pending, bool Red) __state)
    { IronProjectiles.Target = __state.Target; IronProjectiles.Pending = __state.Pending; IronProjectiles.Red = __state.Red; }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class IronDamagePatch
{
    private static void Prefix(Zombie __instance, ref int theDamage, out bool __state)
    {
        __state = IronProjectiles.Pending && IronProjectiles.Target == __instance.Pointer;
        if (!__state) return;
        IronProjectiles.Pending = false;
        theDamage = (int)Math.Min(int.MaxValue, 80L + (IronProjectiles.Red ? 0 : Math.Max(0, __instance.mShieldHealth)));
    }
    private static void Postfix(Zombie __instance, bool __state)
    { if (__state && !IronProjectiles.Red) IronProjectiles.Knockback(__instance); }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class IronTorchwoodPatch
{
    private static bool Prefix(Projectile __instance) => !IronProjectiles.Contains(__instance);
}

[HarmonyPatch]
internal static class IronProjectileCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), "Die"), AccessTools.Method(typeof(Projectile), "Reset") };
    private static void Prefix(Projectile __instance) => IronProjectiles.Forget(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class IronBoardCleanupPatch
{
    private static void Prefix(Board __instance) => IronProjectiles.ForgetBoard(__instance);
}

// AddProjectile contains an inlined initialization path on this game build.
[HarmonyPatch]
internal static class IronProjectileCreatedPatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        typeof(Board).GetMethods().Where(method => method.Name == nameof(Board.AddProjectile));
    private static void Postfix(Projectile __result)
    { if (__result != null) IronProjectiles.Track(__result); }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class IronProjectileRestorePatch
{
    private static void Postfix(Projectile __instance)
    { if (IronProjectiles.Contains(__instance)) ImportedPeaVisuals.EnsureIronProjectile(__instance); }
}
