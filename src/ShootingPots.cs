using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The support owns the event count. Flight owns its saved damage modifier.
// Peas never use mCobTargetRow; Cherry-spreader consumes it on attachment.
internal static class ShootingPots
{
    private const int Boost = -27010, Half = -27020;
    internal const int BonusDelayTicks = 20;
    [ThreadStatic] internal static Fire? Firing;
    [ThreadStatic] private static bool halfExtra, scatter;
    internal sealed class Fire
    {
        internal readonly Plant Plant;
        internal readonly uint Id;
        internal readonly PlantWeapon Weapon;
        internal Projectile? Shot;
        internal bool Finished;
        internal bool AwaitingNativeHook;
        internal Fire(Plant plant, PlantWeapon weapon) { Plant = plant; Id = plant.DataID; Weapon = weapon; }
    }
    internal readonly record struct Scope(Fire? Previous, Fire? Current);

    private static void Trace(Plant plant, string message)
    {
        if (FusionPlant.From(plant) == (FusionPlant)SeedType.Threepeater)
            DebugMode.TraceShootingPot(plant, message);
    }

    // PeaPumpkin.PeaPlant is an explicit source whitelist, not every shooter.
    internal static bool Pea(FusionPlant identity) => identity == (FusionPlant)SeedType.Peashooter
        || identity == (FusionPlant)SeedType.Repeater || identity == (FusionPlant)SeedType.Splitpea
        || identity == (FusionPlant)SeedType.Gatlingpea || identity == (FusionPlant)SeedType.Snowpea
        || FusionPlant.IsCherryShooter(identity) || FusionPlant.IsBlazeShooter(identity)
        || FusionPlant.IsPoisonShooter(identity) || FusionPlant.IsHypnoShooter(identity)
        || FusionPlant.IsSnowShooter(identity) && identity != FusionPlant.SnowThreepeater
        || identity == FusionPlant.Buckshooter || identity == FusionPlant.FootballGatling;

    internal static int ThreeDamage(Plant plant, int damage) =>
        PotBoosts.HasSupport(plant, FusionPlant.TriPot) ? Scale(damage, Boost) : damage;

    private static bool PeaProjectile(Projectile shot) => shot.mProjectileType is ProjectileType.Pea
        or ProjectileType.PeashooterPea or ProjectileType.Snowpea
        or ProjectileType.Fireball or ProjectileType.PeashooterFireball;

    private static int Scale(int damage, int marker) => damage <= 0 ? damage
        : marker == Half ? Math.Max(1, (int)Math.Round(damage * .5d))
        : marker == Boost ? (int)Math.Min(int.MaxValue, Math.Round(damage * 1.2d)) : damage;

    internal static int ProjectileDamage(Projectile shot, int damage) => Scale(damage, shot.mCobTargetRow);

    internal static void Fired(Fire frame)
    {
        var plant = frame.Plant;
        var shot = frame.Shot;
        if (shot == null || shot.mDead || !PeaProjectile(shot) || plant.DataID != frame.Id
            || !ChomperFusions.Active(plant) || plant.mIsAsleep
            || shot.mBoard?.Pointer != plant.mBoard.Pointer)
        {
            Trace(plant, $"rejected: shot={(shot == null ? (uint?)null : (uint)shot.DataID)}, type={shot?.mProjectileType}, dead={shot?.mDead}, active={ChomperFusions.Active(plant)}, asleep={plant.mIsAsleep}");
            return;
        }
        var identity = FusionPlant.From(plant);
        if (halfExtra) shot.mCobTargetRow = Half;
        else if ((identity == (FusionPlant)SeedType.Threepeater || FusionPlant.IsThreepeater(identity))
            && PotBoosts.HasSupport(plant, FusionPlant.TriPot))
            shot.mCobTargetRow = Boost;
        if (scatter)
        {
            shot.mPosX += (plant.mBoard.Rand(801) - 300) * (ScaredyFusions.SourceUnit / 1000);
            float dy = (plant.mBoard.Rand(601) - 300) * (ScaredyFusions.SourceUnit / 1000);
            shot.mPosY += dy;
            shot.mShadowY += dy;
        }
        if (halfExtra || frame.Weapon != PlantWeapon.Primary) return;
        bool three = identity == (FusionPlant)SeedType.Threepeater || FusionPlant.IsThreepeater(identity);
        if (!Pea(identity) && !three) return;
        var pot = (three ? null : PotBoosts.FindSupport(plant, FusionPlant.PeaPot))
            ?? PotBoosts.FindSupport(plant, FusionPlant.TriPot);
        if (pot == null)
        {
            Trace(plant, $"support missing: cell={plant.mPlantCol},{plant.mRow}");
            return;
        }
        int limit = FusionPlant.From(pot) == FusionPlant.PeaPot ? 30 : 6;
        Trace(plant, $"shot={(uint)shot.DataID}, row={shot.mRow}, pot={(uint)pot.DataID}, count={pot.mTargetY + 1}/{limit}");
        if (++pot.mTargetY < limit) return;
        pot.mTargetY = 0;
        if (limit == 30)
        {
            bool old = scatter;
            scatter = true;
            try
            {
                // AnimShoot raises event 5 itself: these five attacks count
                // towards the next trigger, exactly as the source recursion.
                for (int i = 0; i < 5 && ChomperFusions.Active(plant); ++i)
                    FireShot(plant, plant.mRow);
            }
            finally { scatter = old; }
        }
        else if (identity == (FusionPlant)SeedType.Threepeater)
        {
            // These fields are unused by native Flowerpot. Keep the delay,
            // queued pairs and full shooter handle in the pot's native save.
            if ((uint)pot.mTargetZombieID != (uint)plant.DataID)
            {
                pot.mStartRow = pot.mLaunchCounter = 0;
                pot.mTargetZombieID = (ZombieID)(uint)plant.DataID;
            }
            if (++pot.mLaunchCounter == 1)
                pot.mStartRow = unchecked(pot.mBoard.mMainCounter + BonusDelayTicks);
            Trace(plant, $"bonus scheduled: due={pot.mStartRow}, pairs={pot.mLaunchCounter}");
        }
        else
        {
            Extra(plant, Math.Max(0, plant.mRow - 1));
            Extra(plant, Math.Min(plant.mBoard.GetNumRows() - 1, plant.mRow + 1));
        }
    }

    internal static void Tick(Plant pot)
    {
        if (FusionPlant.From(pot) != FusionPlant.TriPot || pot.mLaunchCounter <= 0
            || !ChomperFusions.Active(pot)) return;
        uint sourceId = (uint)pot.mTargetZombieID, potId = pot.DataID;
        Plant? source = null;
        for (int i = 0; i < pot.mBoard.m_plants.Count; ++i)
            if (pot.mBoard.m_plants[i] is { } candidate && (uint)candidate.DataID == sourceId)
            { source = candidate; break; }
        if (source == null || !Supported(source, pot, sourceId, potId))
        {
            pot.mLaunchCounter = pot.mStartRow = 0;
            pot.mTargetZombieID = 0;
            return;
        }
        if (unchecked(pot.mBoard.mMainCounter - pot.mStartRow) < 0) return;
        // Consume before firing: callbacks cannot emit the same pair twice.
        --pot.mLaunchCounter;
        pot.mStartRow = pot.mLaunchCounter > 0
            ? unchecked(pot.mBoard.mMainCounter + BonusDelayTicks) : 0;
        if (pot.mLaunchCounter == 0) pot.mTargetZombieID = 0;
        Extra(source, Math.Max(0, source.mRow - 1));
        if (Supported(source, pot, sourceId, potId))
            Extra(source, Math.Min(source.mBoard.GetNumRows() - 1, source.mRow + 1));
    }

    private static bool Supported(Plant source, Plant pot, uint sourceId, uint potId) =>
        (uint)source.DataID == sourceId && (uint)pot.DataID == potId
        && FusionPlant.From(source) == (FusionPlant)SeedType.Threepeater
        && ChomperFusions.Active(source) && !source.mIsAsleep && ChomperFusions.Active(pot)
        && PotBoosts.FindSupport(source, FusionPlant.TriPot)?.Pointer == pot.Pointer;

    private static void Extra(Plant plant, int row)
    {
        if (!ChomperFusions.Active(plant)) return;
        bool old = halfExtra;
        var outer = Firing;
        halfExtra = true;
        // Capture the nested shot without retaining a pointer across ticks.
        Projectile? extra = null;
        try
        {
            extra = FusionPlant.IsThreepeater(FusionPlant.From(plant))
                ? ThreepeaterFusions.Shoot(plant, row) : FireShot(plant, row).Shot;
        }
        finally { halfExtra = old; Firing = outer; }
        Trace(plant, $"extra: row={row}, shot={(extra == null ? (uint?)null : (uint)extra.DataID)}, type={extra?.mProjectileType}, dead={extra?.mDead}");
        if (extra is not { } shot || shot.mDead || row == plant.mRow
            || shot.mMotionType == ProjectileMotion.Threepeater) return;
        // Proven native Threepeater motion: +/-3, .97 decay, opposite shadow
        // offset. The source also changes only motion and destination row.
        shot.mMotionType = ProjectileMotion.Threepeater;
        shot.mVelY = Math.Sign(row - plant.mRow) * 3;
        shot.mShadowY -= plant.mBoard.GridToPixelY(plant.mPlantCol, row)
            - plant.mBoard.GridToPixelY(plant.mPlantCol, plant.mRow);
    }

    internal static Fire FireShot(Plant plant, int row)
    {
        var previous = Firing;
        var frame = new Fire(plant, PlantWeapon.Primary) { AwaitingNativeHook = true };
        Firing = frame;
        try
        {
            plant.Fire(null!, row, PlantWeapon.Primary);
            Finish(frame);
            return frame;
        }
        finally { Firing = previous; }
    }

    internal static void Finish(Fire frame)
    {
        if (frame.Finished) return;
        frame.Finished = true;
        Fired(frame);
    }

    internal static void Capture(Projectile shot)
    {
        if (!shot.mDead && Firing is { } frame
            && frame.Plant.mBoard?.Pointer == shot.mBoard?.Pointer) frame.Shot = shot;
    }

    internal sealed class Hit
    {
        internal readonly Projectile Shot;
        internal readonly uint Id;
        internal readonly int Marker;
        internal readonly Zombie? Direct;
        internal readonly bool Splash;
        internal Zombie? Target;
        internal uint TargetId;
        internal bool Pending;
        internal Hit(Projectile shot, Zombie? direct, bool splash)
        { Shot = shot; Id = shot.DataID; Marker = shot.mCobTargetRow; Direct = direct; Splash = splash; }
    }
    [ThreadStatic] internal static Hit? CurrentHit;

    internal static Hit? Begin(Projectile shot, Zombie? direct, bool splash) => !shot.mDead
        && PeaProjectile(shot) && shot.mCobTargetRow is Boost or Half
        && !CherrySpreader.IsFlying(shot) ? new(shot, direct, splash) : null;

    internal static void Damage(Zombie zombie, ref int damage)
    {
        var hit = CurrentHit;
        if (hit == null || !hit.Pending || hit.Target?.Pointer != zombie.Pointer
            || hit.TargetId != zombie.DataID || hit.Shot.DataID != hit.Id
            || hit.Shot.mBoard?.Pointer != zombie.mBoard?.Pointer) return;
        hit.Pending = false;
        if (damage <= 0) return;
        if (IronProjectiles.Contains(hit.Shot))
        {
            int shield = IronProjectiles.IsRed(hit.Shot) ? 0 : Math.Max(0, zombie.mShieldHealth);
            damage = (int)Math.Min(int.MaxValue, (long)Scale(80, hit.Marker) + shield);
            return;
        }
        bool fire = hit.Shot.mProjectileType is ProjectileType.Fireball or ProjectileType.PeashooterFireball;
        if (fire)
        {
            // Torchwood adds 20 after the plant's attack modifier. Splash is
            // one third of that resulting fire damage, with integer rounding.
            bool secondary = hit.Splash && hit.Direct?.Pointer != zombie.Pointer;
            int full = secondary ? damage * 3 : damage;
            int scaled = Scale(Math.Max(0, full - 20), hit.Marker) + 20;
            damage = secondary ? Math.Max(1, scaled / 3) : scaled;
        }
        else damage = Scale(damage, hit.Marker);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class ShootingPotAbilityPatch
{
    internal static void Postfix(Plant __instance) => ShootingPots.Tick(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class ShootingPotFireScope
{
    [HarmonyPriority(Priority.First)]
    internal static void Prefix(Plant __instance, PlantWeapon thePlantWeapon, out ShootingPots.Scope __state)
    {
        var previous = ShootingPots.Firing;
        var frame = previous != null && previous.AwaitingNativeHook && !previous.Finished
            && previous.Plant.Pointer == __instance.Pointer && previous.Weapon == thePlantWeapon
            ? previous : new ShootingPots.Fire(__instance, thePlantWeapon);
        frame.AwaitingNativeHook = false;
        __state = new(previous, frame);
        ShootingPots.Firing = frame;
    }
    [HarmonyPriority(Priority.Last)]
    internal static void Postfix(ShootingPots.Scope __state)
    { if (__state.Current is { } frame) ShootingPots.Finish(frame); }
    internal static void Finalizer(ShootingPots.Scope __state)
    { if (__state.Current != null) ShootingPots.Firing = __state.Previous; }
}

[HarmonyPatch]
internal static class ShootingPotProjectilePatch
{
    private static IEnumerable<MethodBase> TargetMethods() => typeof(Board).GetMethods()
        .Where(method => method.Name == nameof(Board.AddProjectile));
    internal static void Postfix(Board __instance, Projectile? __result)
    {
        if (__result != null && __result.mBoard?.Pointer == __instance.Pointer) ShootingPots.Capture(__result);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ProjectileInitialize))]
internal static class ShootingPotProjectileInitializedPatch
{
    internal static void Postfix(Projectile __instance) => ShootingPots.Capture(__instance);
}

[HarmonyPatch]
internal static class ShootingPotImpactScope
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.DoImpact)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.DoSplashDamage)) };
    [HarmonyPriority(Priority.First)]
    internal static void Prefix(Projectile __instance, Zombie theZombie, MethodBase __originalMethod,
        out ShootingPots.Hit? __state)
    {
        __state = ShootingPots.CurrentHit;
        ShootingPots.CurrentHit = ShootingPots.Begin(__instance, theZombie,
            __originalMethod.Name == nameof(Projectile.DoSplashDamage));
    }
    internal static void Finalizer(ShootingPots.Hit? __state) => ShootingPots.CurrentHit = __state;
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.GetDamageFlags))]
internal static class ShootingPotDamageTargetPatch
{
    internal static void Postfix(Projectile __instance, Zombie theZombie)
    {
        if (theZombie == null || ShootingPots.CurrentHit is not { } hit
            || hit.Shot.Pointer != __instance.Pointer || hit.Id != __instance.DataID
            || theZombie.mBoard?.Pointer != __instance.mBoard?.Pointer) return;
        hit.Target = theZombie; hit.TargetId = theZombie.DataID; hit.Pending = true;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class ShootingPotDamagePatch
{
    // Family damage replacements run first; recipient damage reduction last.
    [HarmonyPriority(Priority.Last + 1)]
    internal static void Prefix(Zombie __instance, ref int theDamage) => ShootingPots.Damage(__instance, ref theDamage);
}
