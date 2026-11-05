using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class ScaredyProjectiles
{
    // Native peas never use the serialized cob target. Encode family and the
    // damage at firing, so fear/death/load cannot change an airborne shot.
    internal static int Family(Projectile shot)
    {
        if (shot.mProjectileType is not (ProjectileType.Pea or ProjectileType.PeashooterPea or ProjectileType.Snowpea
            or ProjectileType.Fireball or ProjectileType.PeashooterFireball)) return 0;
        return shot.mCobTargetX switch
        {
            <= -420020 and >= -420050 => 1,
            -421040 => 2,
            -422040 => 3,
            -423060 or -423600 => 4,
            <= -424020 and >= -424100 => 5,
            -425040 => 6,
            _ => 0
        };
    }
    internal static int Damage(Projectile shot) => -(int)shot.mCobTargetX % 1000;

    internal static void Mark(Projectile shot, FusionPlant identity, int damage)
    {
        // AddProjectile initializes this to None. Native Fire normally sets it;
        // this shooter replaces Fire, so it owns the saved target mask.
        shot.mDamageRangeFlags = DamageRangeFlags.Ground;
        int family = identity == FusionPlant.HungryShroom ? 5 : identity == FusionPlant.SpeedyShroom ? 0 : identity == FusionPlant.BlastyShroom ? 1
            : identity == FusionPlant.ShiveryShroom ? 2 : identity == FusionPlant.FrenzyShroom ? 4 : 3;
        shot.mCobTargetX = -420000 - family * 1000 - damage;
        ImportedPeaVisuals.EnsureScaredyProjectile(shot);
    }

    internal sealed record Impact(Projectile Shot, uint ShotId, Zombie? Zombie = null, uint Id = 0);
    [ThreadStatic] internal static Impact? CurrentImpact;
    [ThreadStatic] internal static bool Pending;

    internal static void SelectTarget(Projectile shot, Zombie zombie)
    {
        if (CurrentImpact is not { } impact || impact.Shot.Pointer != shot.Pointer
            || impact.ShotId != shot.DataID || zombie == null || zombie.mDead || zombie.IsDeadOrDying()
            || zombie.mBoard?.Pointer != shot.mBoard?.Pointer) return;
        // Snowpea owns chill, freeze points and the frozen-target multiplier.
        // Its existing impact scope also handles Torchwood thawing correctly.
        CurrentImpact = impact with { Zombie = zombie, Id = zombie.DataID };
        Pending = true;
    }

    internal static void AdjustDamage(Zombie zombie, ref int damage)
    {
        var impact = CurrentImpact;
        if (!Pending || impact?.Zombie?.Pointer != zombie.Pointer || impact.Id != zombie.DataID
            || impact.Shot.DataID != impact.ShotId || impact.Shot.mBoard?.Pointer != zombie.mBoard?.Pointer) return;
        Pending = false;
        int source = Damage(impact.Shot);
        if (Family(impact.Shot) is 1 or 3 && impact.Shot.mProjectileType is ProjectileType.Fireball or ProjectileType.PeashooterFireball)
            // Native direct fire damage is 40, splash is 13. Preserve its
            // falloff while applying source Torchwood's additive twenty.
            damage = damage == 13 ? (source + 20) / 3 : damage * (source + 20) / 40;
        else damage = source;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class ScaredyImpactPatch
{
    internal static void Prefix(Projectile __instance, out (ScaredyProjectiles.Impact? Impact, bool Pending) __state)
    {
        __state = (ScaredyProjectiles.CurrentImpact, ScaredyProjectiles.Pending);
        ScaredyProjectiles.CurrentImpact = !__instance.mDead && ScaredyProjectiles.Family(__instance) != 0
            ? new(__instance, __instance.DataID) : null;
        ScaredyProjectiles.Pending = false;
    }
    internal static void Finalizer((ScaredyProjectiles.Impact? Impact, bool Pending) __state)
    { ScaredyProjectiles.CurrentImpact = __state.Impact; ScaredyProjectiles.Pending = __state.Pending; }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.GetDamageFlags))]
internal static class ScaredyDamageTargetPatch
{
    internal static void Postfix(Projectile __instance, Zombie theZombie) => ScaredyProjectiles.SelectTarget(__instance, theZombie);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class ScaredyDamagePatch
{
    [HarmonyPriority(Priority.First)]
    internal static void Prefix(Zombie __instance, ref int theDamage) => ScaredyProjectiles.AdjustDamage(__instance, ref theDamage);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class ScaredyTorchwoodPatch
{
    internal static bool Prefix(Projectile __instance) => ScaredyProjectiles.Family(__instance) is 0 or 1 or 3;
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class ScaredyProjectileVisualPatch
{
    internal static void Prefix(Projectile __instance)
    {
        if (!__instance.mDead && ScaredyProjectiles.Family(__instance) != 0)
            __instance.mDamageRangeFlags = DamageRangeFlags.Ground;
    }
    internal static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && ScaredyProjectiles.Family(__instance) != 0
            && __instance.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea or ProjectileType.Snowpea)
        {
            // Repair shots from older saves before the next native collision.
            __instance.mDamageRangeFlags = DamageRangeFlags.Ground;
            ImportedPeaVisuals.EnsureScaredyProjectile(__instance);
        }
    }
}
