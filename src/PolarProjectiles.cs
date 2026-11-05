using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// A conversion changes the projectile class, preserving its flight and damage.
// Snow peas do not use the native saved cob target pair; the new pair replaces
// the old mod projectile tag only after its damage modifier has been captured.
internal static class PolarProjectiles
{
    private const float Marker = -31097;
    internal static bool IsPolar(Projectile shot) => shot.mProjectileType == ProjectileType.Snowpea
        && shot.mCobTargetX == Marker;
    internal static void Convert(Board board)
    {
        for (int i = 0; i < board.m_projectiles.Count; ++i)
        {
            var shot = board.m_projectiles[i];
            if (shot == null || shot.mDead || shot.mProjectileType != ProjectileType.Snowpea || IsPolar(shot)) continue;
            int damage = ScaredyProjectiles.Family(shot) == 3 ? ScaredyProjectiles.Damage(shot) : 20;
            damage = ShootingPots.ProjectileDamage(shot, damage);
            shot.mCobTargetRow = damage;
            shot.mCobTargetX = Marker;
            ImportedPeaVisuals.EnsurePolarProjectile(shot);
        }
    }
    internal static bool Impact(Projectile shot, Zombie? zombie)
    {
        if (!IsPolar(shot)) return false;
        if (shot.mDead) return true;
        var board = shot.mBoard;
        uint id = zombie?.DataID ?? 0;
        int damage = FumeProjectiles.ModifiedDamage(shot, Math.Max(0, shot.mCobTargetRow));
        float x = shot.mPosX, y = shot.mPosY;
        int order = shot.mRenderOrder;
        var flags = zombie == null ? (DamageFlags)0 : shot.GetDamageFlags(zombie);
        shot.Die();
        if (zombie == null || !Live(board, zombie) || zombie.mMindControlled) return true;
        if (BloverEffects.Reserve(zombie, true)) SnowPea.Hit(zombie, 20);
        if (zombie.DataID != id || !Live(board, zombie)) return true;
        bool multiply = zombie.mIceTrapCounter > 0 || !zombie.CanBeChilled();
        zombie.TakeDamage((int)Math.Min(int.MaxValue, damage * (multiply ? 4L : 1L)), flags);
        if (zombie.DataID == id && Live(board, zombie)) zombie.ApplyChill(false);
        board.mApp.AddTodParticle(x, y, order, ParticleEffect.SnowpeaSplat);
        return true;
    }
    private static bool Live(Board board, Zombie zombie) => zombie.mBoard?.Pointer == board.Pointer
        && !zombie.mDead && !zombie.IsDeadOrDying();
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class PolarImpactPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Projectile __instance, Zombie theZombie) => !PolarProjectiles.Impact(__instance, theZombie);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToPea))]
internal static class PolarThawPatch
{
    // TorchWood.OnTriggerEnter2D accepts source bullet IDs 0 and 15, not 97.
    private static bool Prefix(Projectile __instance) => !PolarProjectiles.IsPolar(__instance);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class PolarIgnitionPatch
{
    private static bool Prefix(Projectile __instance) => !PolarProjectiles.IsPolar(__instance);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class PolarVisualPatch
{
    private static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && PolarProjectiles.IsPolar(__instance)) ImportedPeaVisuals.EnsurePolarProjectile(__instance);
    }
}
