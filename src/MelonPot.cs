using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// mCobTargetRow is unused by ordinary melon flight. The saved marker follows
// the projectile, so moving/removing the pot cannot alter an airborne shot.
internal static class MelonPot
{
    internal const int ShotMarker = -1125;
    [ThreadStatic] internal static Plant? Shooter;
    internal sealed record Splash(Projectile Shot, uint Id, Zombie? Direct);
    [ThreadStatic] internal static Splash? CurrentSplash;
    internal static bool Boosted(Projectile shot) => shot.mCobTargetRow == ShotMarker
        && shot.mProjectileType is ProjectileType.Melon or ProjectileType.Wintermelon;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class MelonPotFireScope
{
    private static void Prefix(Plant __instance, out Plant? __state)
    {
        __state = MelonPot.Shooter;
        var identity = FusionPlant.From(__instance);
        MelonPot.Shooter = (identity == (FusionPlant)SeedType.Melonpult
            || identity == (FusionPlant)SeedType.Wintermelon)
            && PotBoosts.HasSupport(__instance, FusionPlant.MelonPot) ? __instance : null;
    }
    private static void Finalizer(Plant? __state) => MelonPot.Shooter = __state;
}

[HarmonyPatch]

internal static class MelonPotProjectilePatch
{
    private static System.Collections.Generic.IEnumerable<System.Reflection.MethodBase> TargetMethods() =>
        typeof(Board).GetMethods().Where(method => method.Name == nameof(Board.AddProjectile));
    private static void Postfix(Board __instance, Projectile? __result)
    {
        if (__result == null || __result.mDead || MelonPot.Shooter is not { } plant
            || plant.mBoard?.Pointer != __instance.Pointer
            || __result.mBoard?.Pointer != __instance.Pointer
            || __result.mProjectileType is not (ProjectileType.Melon or ProjectileType.Wintermelon)) return;
        __result.mCobTargetRow = MelonPot.ShotMarker;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoSplashDamage))]
internal static class MelonPotSplashScope
{
    private static void Prefix(Projectile __instance, Zombie theZombie, out MelonPot.Splash? __state)
    {
        __state = MelonPot.CurrentSplash;
        MelonPot.CurrentSplash = MelonPot.Boosted(__instance)
            ? new(__instance, __instance.DataID, theZombie) : null;
    }
    private static void Finalizer(MelonPot.Splash? __state) => MelonPot.CurrentSplash = __state;
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class MelonPotSplashDamagePatch
{
    private static void Prefix(Zombie __instance, ref int theDamage)
    {
        var splash = MelonPot.CurrentSplash;
        if (splash == null || splash.Shot.DataID != splash.Id
            || splash.Direct?.Pointer == __instance.Pointer
            || splash.Shot.mBoard?.Pointer != __instance.mBoard?.Pointer) return;
        // Native Melon/Wintermelon share 80 direct damage. Only secondary
        // damage changes; native hit tests, flags and winter chill still run.
        theDamage = 80 * 2 / 3;
    }
}
