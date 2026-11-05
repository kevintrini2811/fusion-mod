using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// A distinct cherry variant. Native Peashooter still owns target acquisition,
// firing animation, pause, carrying and the persisted launch countdown.
internal static class ExplodeOShooter
{
    internal static bool IsExplode(Plant plant) => FusionPlant.From(plant) == FusionPlant.ExplodeOShooter;

    internal static void Detonate(Projectile projectile)
    {
        var board = projectile.mBoard;
        if (board == null || projectile.mDead) return;
        int damage = CherryShooter.ExplosionDamage(projectile);
        float x = projectile.mPosX, y = projectile.mPosY;
        int row = projectile.mRow, order = projectile.mRenderOrder;
        // Remove provenance before damage callbacks: a reentrant impact must
        // never detonate the same projectile twice. Die restores pooled art.
        CherryShooter.Forget(projectile);
        projectile.Die();
        CherryBombPlants.HealNuts(board, x, y, row, 80);
        // Replanted cells are 80 pixels wide. Use the native attack rectangles
        // and ground eligibility, with an actual circle rather than a 3x3 box.
        var targets = new List<(Zombie Zombie, uint Id)>();
        var zombies = board.m_zombies;
        for (int i = zombies.Count - 1; i >= 0; --i)
        {
            var zombie = zombies[i];
            if (zombie == null || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
                || Math.Abs(zombie.mRow - row) > 1
                || !zombie.EffectedByDamage(DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax);
            float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy > 80f * 80f) continue;
            targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)) continue;
            // Explosion damage hits shield and body without distance falloff.
            // No extra native pea hit, no plant damage and no ladder deletion.
            zombie.TakeDamage(damage, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
        }
        var explosion = board.mApp.AddTodParticle(x, y, order, ParticleEffect.Powie);
        // Scale this particle instance only; damage radius remains 80 pixels.
        if (explosion != null) explosion.OverrideScale(null, .9f);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Cherrybomb);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class ExplodeInitializePatch
{
    private static void Postfix(Plant __instance)
    {
        if (!ExplodeOShooter.IsExplode(__instance)) return;
        __instance.mLaunchRate = 300;
        // Keep native randomized first-shot phase, scaled to this interval.
        __instance.mLaunchCounter = Math.Clamp(__instance.mLaunchCounter * 2, 1, 300);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class FixedShooterIntervalPatch
{
    private static void Prefix(Plant __instance, out bool __state)
    {
        __state = (ExplodeOShooter.IsExplode(__instance) || FusionPlant.From(__instance) == FusionPlant.PeaNut)
            && __instance.mLaunchCounter <= 1;
    }
    private static void Postfix(Plant __instance, bool __state)
    {
        // Native reset subtracts up to 14 ticks of jitter. This fusion's source
        // interval is three seconds: retain the native fire path, reset to 300.
        if (__state && !__instance.mDead)
            __instance.mLaunchCounter = FusionPlant.From(__instance) == FusionPlant.PeaNut ? 150 : 300;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class ExplodeImpactPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Projectile __instance)
    {
        if (!CherryShooter.IsExplosive(__instance)) return true;
        ExplodeOShooter.Detonate(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(SeedPacket), nameof(SeedPacket.WasPlanted))]
internal static class ExplodeFusionCooldownPatch
{
    private static void Postfix(SeedPacket __instance)
    {
        var context = PlayerPlantingScope.Current;
        if (context?.CreatedFusion != FusionPlant.ExplodeOShooter
            || __instance.mBoard?.Pointer != context.Board.Pointer
            || __instance.mPacketType != SeedType.Cherrybomb || !__instance.mRefreshing
            || __instance.mRefreshTime <= 0) return;
        // Credit 85% of this one recharge. Leave native refresh duration intact,
        // including speed changes, serialization and the next ordinary use.
        __instance.mRefreshCounter = Math.Max(__instance.mRefreshCounter,
            (int)((long)__instance.mRefreshTime * 85 / 100));
    }
}
