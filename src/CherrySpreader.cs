using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// The attached cherry remains a native, serialized projectile. Its target ID,
// damage and age therefore survive loading without a parallel status table.
internal static class CherrySpreader
{
    private const float FlyingMarker = -41020, AttachedMarker = -41021;
    internal const int RipeningTicks = 600;
    internal const float BlastRadius = 1.5f / 1.35f * 80;
    [ThreadStatic] internal static Board? CherryDamageBoard;

    internal static bool IsFlying(Projectile shot) => shot.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
        && shot.mCobTargetX == FlyingMarker;
    internal static bool IsAttached(Projectile shot) => shot.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
        && shot.mCobTargetX == AttachedMarker;

    internal static void Track(Projectile shot)
    {
        if (shot.mDead || shot.mBoard == null
            || shot.mProjectileType is not (ProjectileType.Pea or ProjectileType.PeashooterPea)) return;
        shot.mCobTargetX = FlyingMarker;
        ImportedPeaVisuals.EnsureCherrySpreaderProjectile(shot);
    }

    internal static Projectile? AttachedTo(Board board, uint target)
    {
        for (int i = 0; i < board.m_projectiles.Count; ++i)
        {
            var shot = board.m_projectiles[i];
            if (shot != null && !shot.mDead && shot.mBoard?.Pointer == board.Pointer
                && IsAttached(shot) && (uint)shot.mTargetZombieID == target) return shot;
        }
        return null;
    }

    internal static void Charge(Zombie zombie, int damage)
    {
        if (damage <= 0 || zombie.mBoard == null || zombie.mDead || zombie.IsDeadOrDying()
            || zombie.mMindControlled) return;
        var bomb = AttachedTo(zombie.mBoard, zombie.DataID);
        if (bomb == null) return;
        // TimeBomb.Charge uses integer division per hit, not accumulated
        // fractional quarters. Preserve that rounding for small splash hits.
        bomb.mCobTargetRow = (int)Math.Min(int.MaxValue, (long)bomb.mCobTargetRow + damage / 4);
        bomb.mClickBackoffCounter = 17;
    }

    internal static void Impact(Projectile shot, Zombie? zombie)
    {
        if (shot.mDead) return;
        int damage = FumeProjectiles.ModifiedDamage(shot, ShootingPots.ProjectileDamage(shot, 20));
        var board = shot.mBoard;
        if (board == null || zombie == null || zombie.mBoard?.Pointer != board.Pointer
            || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled)
        { shot.Die(); return; }
        if (AttachedTo(board, zombie.DataID) != null)
        {
            var flags = shot.GetDamageFlags(zombie);
            uint id = zombie.DataID;
            // Finish this projectile before callbacks can reenter its impact.
            shot.Die();
            if (zombie.DataID != id || zombie.mBoard?.Pointer != board.Pointer
                || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled) return;
            // These projectiles explicitly charge an existing bomb, then deal
            // 20 direct damage. The first hit only installs its 20-damage bomb.
            Charge(zombie, damage);
            var old = CherryDamageBoard;
            CherryDamageBoard = null;
            try { zombie.TakeDamage(damage, flags); }
            finally { CherryDamageBoard = old; }
            return;
        }
        Attach(shot, zombie, damage);
    }

    internal static bool Attach(Zombie zombie, int damage)
    {
        if (zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
            || AttachedTo(zombie.mBoard, zombie.DataID) != null) return false;
        var shot = zombie.mBoard.AddProjectile(zombie.mPosX, zombie.mPosY,
            zombie.mRenderOrder + 1, zombie.mRow, ProjectileType.Pea);
        if (shot == null || shot.mDead) return false;
        Attach(shot, zombie, damage);
        return true;
    }

    private static void Attach(Projectile shot, Zombie zombie, int damage)
    {
        shot.mCobTargetX = AttachedMarker;
        shot.mCobTargetRow = damage;
        shot.mTargetZombieID = (ZombieID)zombie.DataID;
        shot.mProjectileAge = shot.mClickBackoffCounter = 0;
        shot.mMotionType = ProjectileMotion.Straight;
        shot.mVelX = shot.mVelY = shot.mVelZ = shot.mAccZ = 0;
        Follow(shot, zombie);
        ImportedPeaVisuals.EnsureCherrySpreaderProjectile(shot);
    }

    private static void Follow(Projectile shot, Zombie zombie)
    {
        var rect = zombie.GetZombieRect();
        shot.mPosX = (rect.xMin + rect.xMax) / 2;
        shot.mPosY = (rect.yMin + rect.yMax) / 2;
        shot.mPosZ = 0;
        shot.mRow = zombie.mRow;
        shot.mX = shot.mPosX;
        shot.mY = shot.mPosY;
        shot.mShadowY = zombie.mPosY;
        shot.mRenderOrder = zombie.mRenderOrder + 1;
    }

    internal static void Tick(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board == null || board.mPaused || board.Pointer == CherryBombPlants.DisposingBoard
            || board.mApp == null || board.mApp.GameScene != GameScenes.Playing) return;
        var target = board.ZombieTryToGet(shot.mTargetZombieID);
        // Source TimeBomb is removed when its owner disappears or dies. Never
        // transfer an old attachment to the next occupant of a pooled zombie.
        if (target == null || target.DataID != (uint)shot.mTargetZombieID || target.mBoard?.Pointer != board.Pointer
            || target.mDead || target.IsDeadOrDying() || target.mMindControlled)
        { shot.Die(); return; }
        Follow(shot, target);
        if (!FusionTime.SourceStep(shot, typeof(CherrySpreader))) return;
        if (shot.mClickBackoffCounter > 0) --shot.mClickBackoffCounter;
        if (++shot.mProjectileAge > RipeningTicks) Detonate(shot, target);
    }

    private static void Detonate(Projectile shot, Zombie owner)
    {
        var board = shot.mBoard;
        bool airborne = owner.IsFlying();
        var range = airborne ? DamageRangeFlags.Flying : DamageRangeFlags.Ground;
        float x = shot.mPosX, y = shot.mPosY;
        int row = shot.mRow, damage = shot.mCobTargetRow, order = shot.mRenderOrder;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || Math.Abs(zombie.mRow - row) > 1
                || zombie.IsFlying() != airborne || !SquashFusions.Eligible(board, zombie, range)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax);
            float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= BlastRadius * BlastRadius) targets.Add((zombie, zombie.DataID));
        }
        shot.Die();
        ReactivePots.Cherry(board, x, y, row, BlastRadius);
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && zombie.IsFlying() == airborne && SquashFusions.Eligible(board, zombie, range))
                zombie.TakeDamage(damage, DamageFlags.HitsShieldAndBody);
        // Physical explosion: retain bodies and cold effects; no plant damage,
        // ash, crater or extra native pea hit. Native particles replace VFX.
        board.mApp.AddTodParticle(x, y, order, ParticleEffect.Powie);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Cherrybomb);
    }

    internal static void Forget(Projectile shot)
    {
        if (!IsFlying(shot) && !IsAttached(shot)) return;
        shot.mCobTargetX = 0;
        shot.mCobTargetRow = 0;
        shot.mTargetZombieID = (ZombieID)0;
        ImportedPeaVisuals.ForgetProjectile(shot);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class CherrySpreaderImpactPatch
{
    [HarmonyPriority(Priority.First)]
    internal static bool Prefix(Projectile __instance, Zombie theZombie)
    {
        if (CherrySpreader.IsAttached(__instance)) return false;
        if (!CherrySpreader.IsFlying(__instance)) return true;
        CherrySpreader.Impact(__instance, theZombie);
        return false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class CherrySpreaderUpdatePatch
{
    internal static bool Prefix(Projectile __instance)
    {
        if (!CherrySpreader.IsAttached(__instance)) return true;
        CherrySpreader.Tick(__instance);
        return false;
    }
    internal static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && (CherrySpreader.IsFlying(__instance) || CherrySpreader.IsAttached(__instance)))
            ImportedPeaVisuals.EnsureCherrySpreaderProjectile(__instance);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class CherrySpreaderTorchwoodPatch
{
    internal static bool Prefix(Projectile __instance) =>
        !CherrySpreader.IsFlying(__instance) && !CherrySpreader.IsAttached(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Reset))]
internal static class CherrySpreaderCleanupPatch
{
    // Keep the marker on a dead slot until Reset so a reentrant impact still
    // reaches the dead-projectile guard. Existing Die cleanup releases its art.
    internal static void Prefix(Projectile __instance) => CherrySpreader.Forget(__instance);
}

[HarmonyPatch]
internal static class CherrySpreaderProjectileDamageScope
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.DoImpact)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.DoSplashDamage)) };
    [HarmonyPriority(Priority.First + 100)]
    internal static void Prefix(Projectile __instance, out Board? __state)
    {
        __state = CherrySpreader.CherryDamageBoard;
        CherrySpreader.CherryDamageBoard = CherryShooter.TryShot(__instance) ? __instance.mBoard : null;
    }
    internal static void Finalizer(Board? __state) => CherrySpreader.CherryDamageBoard = __state;
}

[HarmonyPatch]
internal static class CherrySpreaderExplosionDamageScope
{
    // The shared managed explosion owners cover their actual native damage
    // calls, including Cherry Chomper, Cherry Nut and Solar Bomb. No global
    // inference from ash flags can distinguish a cherry from a Doom explosion.
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(CherryBombPlants), nameof(CherryBombPlants.Explode)),
        AccessTools.Method(typeof(CherryBombPlants), nameof(CherryBombPlants.ExplodeFusion)),
        AccessTools.Method(typeof(ExplodeOShooter), nameof(ExplodeOShooter.Detonate)) };
    internal static void Prefix(object[] __args, out Board? __state)
    {
        __state = CherrySpreader.CherryDamageBoard;
        CherrySpreader.CherryDamageBoard = __args[0] switch {
            CherryBombPlants.Death death => death.Board,
            Plant plant => plant.mBoard,
            Projectile shot => shot.mBoard,
            _ => null };
    }
    internal static void Finalizer(Board? __state) => CherrySpreader.CherryDamageBoard = __state;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class CherrySpreaderNativeBombScope
{
    [HarmonyPriority(Priority.First + 100)]
    internal static void Prefix(Plant __instance, out Board? __state)
    {
        __state = CherrySpreader.CherryDamageBoard;
        CherrySpreader.CherryDamageBoard = FusionPlant.From(__instance) == (FusionPlant)SeedType.Cherrybomb
            ? __instance.mBoard : null;
    }
    internal static void Finalizer(Board? __state) => CherrySpreader.CherryDamageBoard = __state;
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class CherrySpreaderChargePatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Zombie __instance, int theDamage, bool __runOriginal, out Board? __state)
    {
        __state = CherrySpreader.CherryDamageBoard;
        // Temporarily clear provenance so unrelated damage from a nested
        // callback cannot inherit this explosion or projectile's ownership.
        CherrySpreader.CherryDamageBoard = null;
        if (__runOriginal && __state != null && __instance.mBoard?.Pointer == __state.Pointer)
            CherrySpreader.Charge(__instance, theDamage);
    }
    internal static void Finalizer(Board? __state) => CherrySpreader.CherryDamageBoard = __state;
}
