using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class FumeProjectiles
{
    internal const float NutMarker = NutProjectiles.WallopMarker;
    private const int BoostMarker = -31010;
    internal static bool IsNut(Projectile shot) => shot.mProjectileType == ProjectileType.Pea && shot.mCobTargetX == NutMarker;
    internal static bool IsBoost(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == BoostMarker;

    internal static GridItem? Record(Projectile shot, bool create)
    {
        var board = shot.mBoard;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { mDead: false } item && IsBoost(item)
                && unchecked((uint)item.mGridX) == shot.DataID) return item;
        if (!create) return null;
        var record = board.m_gridItems.DataArrayAlloc();
        if (record == null) return null;
        try
        {
            record.Init(board.mApp, GridItemType.ZenTool);
            record.mGridItemState = GridItemState.Normal;
            record.mSunCount = BoostMarker;
            record.mSeedType = SeedType.None;
            record.mGridX = unchecked((int)shot.DataID);
            record.mGridY = record.mGridItemCounter = 0;
            record.mX = record.mY = record.mPosX = record.mPosY = -1000;
            return record;
        }
        catch { record.GridItemDie(); throw; }
    }

    internal static bool Straight(Projectile shot) => !shot.mDead && shot.mProjectileType != ProjectileType.ZombiePea
        && !CherrySpreader.IsAttached(shot) && !KelpProjectiles.Contains(shot)
        && NutProjectiles.Family(shot) != 3 && !RollingProjectiles.TryShot(shot, out _)
        && !PotatoProjectiles.Contains(shot)
        && (shot.mMotionType is ProjectileMotion.Straight or ProjectileMotion.Threepeater
            || shot.mMotionType == ProjectileMotion.Star && shot.mVelX > 0 && shot.mVelY == 0 && shot.mAccZ == 0);

    internal static void Boost(Plant plant)
    {
        var board = plant.mBoard;
        for (int i = 0; i < board.m_projectiles.Count; ++i)
        {
            var shot = board.m_projectiles[i];
            if (shot == null || !Straight(shot) || shot.mRow != plant.mRow
                || shot.mPosX <= plant.mX + plant.mWidth / 2f) continue;
            var record = Record(shot, true);
            if (record == null) continue; // Never grant an unsaved modifier.
            if (record.mGridItemCounter < int.MaxValue) ++record.mGridItemCounter;
            if (record.mGridY < int.MaxValue) ++record.mGridY;
        }
    }
    internal static bool Amplified(Projectile shot) => Record(shot, false)?.mSeedType == SeedType.Cobcannon;
    internal static int Damage(Projectile shot, int damage)
    {
        long result = (long)Math.Max(0, damage) + Math.Max(0, Record(shot, false)?.mGridItemCounter ?? 0);
        return (int)Math.Min(int.MaxValue, Amplified(shot) ? result * 7 / 4 : result);
    }
    internal static float ExtraSpeed(Projectile shot) => Math.Max(0, Record(shot, false)?.mGridY ?? 0)
        * (.2f * FumeVariants.Unit / 100);
    internal static float Speed(Projectile shot) => shot.mMotionType == ProjectileMotion.Star
        ? shot.mVelX + ExtraSpeed(shot) : 3.33f + ExtraSpeed(shot);

    internal static void TickRecord(GridItem item)
    {
        if (item.mBoard.mPaused) return;
        for (int i = 0; i < item.mBoard.m_projectiles.Count; ++i)
            if (item.mBoard.m_projectiles[i] is { mDead: false } shot
                && shot.DataID == unchecked((uint)item.mGridX)) return;
        item.GridItemDie();
    }

    internal static bool LaunchNut(Plant plant, int damage)
    {
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f + 20,
            plant.mY + plant.mHeight / 2f, plant.mRenderOrder + 1, plant.mRow, ProjectileType.Pea);
        if (shot == null || shot.mDead) return false;
        shot.mCobTargetX = NutMarker;
        shot.mCobTargetRow = Math.Max(0, damage);
        shot.mMotionType = ProjectileMotion.Star;
        shot.mVelX = 6 * FumeVariants.Unit / 100;
        shot.mVelY = shot.mVelZ = shot.mAccZ = shot.mPosZ = 0;
        shot.mShadowY = shot.mPosY + 12;
        shot.mDamageRangeFlags = DamageRangeFlags.Ground;
        ImportedPeaVisuals.EnsureWallopProjectile(shot);
        return true;
    }

    internal static void MoveNut(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board.mPaused) return;
        float oldX = shot.mPosX, x = oldX + Speed(shot);
        shot.mPosX = x;
        shot.mPosY += board.GetPosYBasedOnRow(x, shot.mRow) - board.GetPosYBasedOnRow(oldX, shot.mRow);
        shot.mShadowY = shot.mPosY + 12;
        if (x > 900 || x < -80) { shot.Die(); return; }
        var targets = new List<(Zombie Zombie, uint Id, float X)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != shot.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            if (rect.xMax >= oldX - 12 && rect.xMin <= x + 12) targets.Add((zombie, zombie.DataID, rect.xMin));
        }
        uint shotId = shot.DataID;
        foreach (var (zombie, id, _) in targets.OrderBy(target => target.X))
        {
            if (shot.mDead || shot.DataID != shotId || shot.mBoard?.Pointer != board.Pointer) return;
            if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)
                || !NutProjectiles.Remember(shot, zombie)) continue;
            // Source captures health before applying damage. Type-2 shields
            // are bypassed; body and type-1 helmet spend the penetration budget.
            long health = Math.Max(0, zombie.mBodyHealth) + (long)Math.Max(0, zombie.mHelmHealth);
            int damage = Damage(shot, shot.mCobTargetRow);
            zombie.TakeDamage(damage, DamageFlags.BypassesShield);
            if (shot.DataID != shotId || shot.mBoard?.Pointer != board.Pointer) return;
            if (damage <= health) { shot.Die(); return; }
            // Fold boost into the saved remainder, then clear its backing bonus
            // so a piercing hit never applies the same addition a second time.
            shot.mCobTargetRow = (int)(damage - health);
            if (Record(shot, false) is { } record) record.mGridItemCounter = 0;
        }
    }

    internal sealed class Hit
    {
        internal readonly Projectile Shot;
        internal readonly uint ShotId;
        internal readonly Zombie? Direct;
        internal readonly int Bonus;
        internal readonly bool Splash;
        internal bool Folded;
        internal Zombie? Target;
        internal uint TargetId;
        internal readonly bool Amplified;
        internal bool Pending;
        internal Hit(Projectile shot, Zombie? direct, int bonus, bool splash)
        {
            Shot = shot; ShotId = shot.DataID; Direct = Target = direct;
            Amplified = FumeProjectiles.Amplified(shot);
            TargetId = direct?.DataID ?? 0; Bonus = bonus; Splash = splash; Pending = direct != null;
        }
    }
    [ThreadStatic] internal static Hit? CurrentHit;
    internal static void ApplyBonus(Zombie zombie, ref int damage)
    {
        var hit = CurrentHit;
        if (hit == null || hit.Folded || !hit.Pending || hit.Shot.DataID != hit.ShotId || zombie.DataID != hit.TargetId
            || zombie.Pointer != hit.Target?.Pointer || hit.Shot.mBoard?.Pointer != zombie.mBoard?.Pointer) return;
        hit.Pending = false;
        int bonus = hit.Splash && hit.Direct?.Pointer != zombie.Pointer ? hit.Bonus / 3 : hit.Bonus;
        if (damage > 0) damage = (int)Math.Min(int.MaxValue, (hit.Amplified ? (long)damage * 7 / 4 : damage) + bonus);
    }
    internal static int ModifiedDamage(Projectile shot, int damage)
    {
        // Polar applies its fourfold multiplier to the whole original damage.
        // Its custom impact owns this addition before calling TakeDamage.
        if (CurrentHit?.Shot.Pointer == shot.Pointer) { CurrentHit.Pending = false; CurrentHit.Folded = true; }
        return Damage(shot, damage);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class FumeProjectileMotionPatch
{
    private static bool Prefix(Projectile __instance)
    {
        if (__instance.mDead || __instance.mBoard.mPaused) return true;
        if (FumeProjectiles.IsNut(__instance)) { FumeProjectiles.MoveNut(__instance); return false; }
        if (FumeProjectiles.Straight(__instance))
        {
            // Native Straight/Threepeater use a fixed X step, not mVelX.
            // Add the saved delta before native collision; preserve native Y,
            // shadow, terrain, pooling and the original motion classification.
            __instance.mPosX += FumeProjectiles.ExtraSpeed(__instance);
        }
        return true;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class WallopCollisionPatch
{
    private static bool Prefix(Projectile __instance) => !FumeProjectiles.IsNut(__instance);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class WallopIgnitionPatch
{
    private static bool Prefix(Projectile __instance) => !FumeProjectiles.IsNut(__instance);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class WallopProjectileVisualPatch
{
    private static void Postfix(Projectile __instance)
    { if (!__instance.mDead && FumeProjectiles.IsNut(__instance)) ImportedPeaVisuals.EnsureWallopProjectile(__instance); }
}
[HarmonyPatch]
internal static class BoostImpactScope
{
    private static IEnumerable<System.Reflection.MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.DoImpact)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.DoSplashDamage)) };
    [HarmonyPriority(Priority.First)]
    private static void Prefix(Projectile __instance, Zombie theZombie, System.Reflection.MethodBase __originalMethod,
        out FumeProjectiles.Hit? __state)
    {
        __state = FumeProjectiles.CurrentHit;
        FumeProjectiles.CurrentHit = !__instance.mDead
            ? new(__instance, theZombie, FumeProjectiles.Damage(__instance, 0),
                __originalMethod.Name == nameof(Projectile.DoSplashDamage)) : null;
    }
    private static void Finalizer(FumeProjectiles.Hit? __state) => FumeProjectiles.CurrentHit = __state;
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.GetDamageFlags))]
internal static class BoostTargetPatch
{
    private static void Postfix(Projectile __instance, Zombie theZombie)
    {
        if (theZombie == null || FumeProjectiles.CurrentHit is not { } hit
            || hit.Shot.Pointer != __instance.Pointer || hit.ShotId != __instance.DataID) return;
        hit.Target = theZombie; hit.TargetId = theZombie.DataID; hit.Pending = true;
    }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class BoostDamagePatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Zombie __instance, ref int theDamage) => FumeProjectiles.ApplyBonus(__instance, ref theDamage);
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class BoostItemPatch
{
    private static bool Prefix(GridItem __instance)
    {
        if (!FumeProjectiles.IsBoost(__instance)) return true;
        FumeProjectiles.TickRecord(__instance); return false;
    }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class BoostItemDrawPatch
{
    private static bool Prefix(GridItem __instance) => !FumeProjectiles.IsBoost(__instance);
}
