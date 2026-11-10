using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Saved marker ranges contain both projectile identity and remaining hits.
// mCobTargetRow snapshots damage; its sign snapshots Melon Pot's splash bonus.
internal static class LobberProjectiles
{
    private const int Taco = -520100, ButterTaco = -520110, Cob = -520140,
        ButterCob = -520160, Overripe = -520180, Cabbage = -520200, Salad = -520220, MetalMelon = -520240, ShellMelon = -520260, MagnetKernel = -520280, MagnetButter = -520300, IronMelon = -520320, DoomCabbage = -520340, BlubberButter = -520360, Solar = -520380, IronKernel = -520400, IronButter = -520420;

    internal static int Family(Projectile shot) => shot.mProjectileType switch
    {
        ProjectileType.Kernel when shot.mCobTargetX == IronKernel => 16,
        ProjectileType.Butter when shot.mCobTargetX == IronButter => 17,
        ProjectileType.Cabbage when shot.mCobTargetX is <= Solar - 1 and >= Solar - 6 => 15,
        ProjectileType.Cabbage when shot.mCobTargetX is <= Taco and >= Taco - 2 => 1,
        ProjectileType.Butter when shot.mCobTargetX is <= ButterTaco and >= ButterTaco - 2 => 2,
        ProjectileType.Melon when shot.mCobTargetX == Cob => 3,
        ProjectileType.Melon when shot.mCobTargetX == ButterCob => 4,
        ProjectileType.Melon when shot.mCobTargetX is <= Overripe and >= Overripe - 1 => 5,
        ProjectileType.Cabbage when shot.mCobTargetX is <= Cabbage and >= Cabbage - 2 => 6,
        ProjectileType.Melon when shot.mCobTargetX == Salad => 7,
        ProjectileType.Melon when shot.mCobTargetX == MetalMelon => 8,
        ProjectileType.Melon when shot.mCobTargetX == ShellMelon => 9,
        ProjectileType.Kernel when shot.mCobTargetX == MagnetKernel => 10,
        ProjectileType.Butter when shot.mCobTargetX == MagnetButter => 11,
        ProjectileType.Melon when shot.mCobTargetX == IronMelon => 12,
        ProjectileType.Cabbage when shot.mCobTargetX == DoomCabbage => 13,
        ProjectileType.Butter when shot.mCobTargetX == BlubberButter => 14,
        _ => 0
    };

    internal static int Remaining(Projectile shot) => Family(shot) == 6 ? Cabbage - (int)shot.mCobTargetX : 0;

    internal static bool AlreadyHit(Projectile shot, Zombie zombie) =>
        Family(shot) != 7 && ((uint)shot.mTargetZombieID == zombie.DataID
        || unchecked((uint)shot.mHitTorchwoodGridX) == zombie.DataID);

    internal static void Fire(Plant plant, int row, Zombie? target, bool edge, int ricochets)
    {
        var identity = FusionPlant.From(plant);
        bool butter = plant.mShootingCounter == 1;
        if (identity == FusionPlant.SolarPult) { FireSolar(plant, target); return; }
        int family = identity == FusionPlant.BoltPult ? butter ? 17 : 16 : identity == FusionPlant.DamagePult ? 13 : identity == FusionPlant.KernelMorph ? butter ? 11 : 10
            : identity == FusionPlant.MetalPult ? 12 : identity == FusionPlant.MelonMorph ? 8 : identity == FusionPlant.TacoPult ? butter ? 2 : 1
            : identity == FusionPlant.CobPult ? butter ? 4 : 3
            : identity == FusionPlant.OverripeMelon ? 5 : identity == FusionPlant.SaladPult ? 7 : 6;
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f
            + (edge ? .5f * ScaredyFusions.SourceUnit : 0), plant.mY + plant.mHeight / 2f - 10,
            plant.mRenderOrder + 1, row, family is 3 or 4 or 5 or 7 or 8 or 12 ? ProjectileType.Melon
                : family is 2 or 11 or 17 ? ProjectileType.Butter : family is 10 or 16 ? ProjectileType.Kernel : ProjectileType.Cabbage);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = family switch
        { 1 => Taco, 2 => ButterTaco, 3 => Cob, 4 => ButterCob, 5 => Overripe, 7 => Salad, 8 => MetalMelon, 10 => MagnetKernel, 11 => MagnetButter, 12 => IronMelon, 13 => DoomCabbage, 16 => IronKernel, 17 => IronButter, _ => Cabbage - ricochets };
        int damage = family switch { 1 or 6 => 40, 2 => 60, 3 or 7 => 140, 8 => 120, 4 => 160, 10 => 60, 11 => 80, 12 => 200, 13 => 1800, 17 => 100, _ => 80 };
        if (family == 6) damage = ShootingPots.ThreeDamage(plant, damage);
        shot.mCobTargetRow = family is 3 or 4 or 5 or 7 or 8 or 12 && PotBoosts.HasSupport(plant, FusionPlant.MelonPot)
            ? -damage : damage;
        Launch(shot, target, PuffLobbers.FlightTicks);
        if (family == 7) shot.mHitTorchwoodGridX = PotBoosts.ButterRange(plant);
    }

    internal static int SolarHits(int sun) => Math.Clamp(sun / 200 + 1, 2, 6);

    private static void FireSolar(Plant plant, Zombie? target)
    {
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f - 10, plant.mRenderOrder + 1, plant.mRow, ProjectileType.Cabbage);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = Solar - SolarHits(plant.mBoard.mSunMoney[0].Amount);
        shot.mCobTargetRow = 50;
        Launch(shot, target, PuffLobbers.FlightTicks);
        ImportedPeaVisuals.EnsureMixedLobProjectile(shot);
    }

    private static void SolarImpact(Projectile shot, Zombie? direct)
    {
        var board = shot.mBoard;
        if (direct == null)
        {
            float x = shot.mPosX, y = shot.mPosY;
            shot.Die();
            SolarShooter.DropSun(board, x, y, 5);
            return;
        }
        if (!SquashFusions.Eligible(board, direct, DamageRangeFlags.Ground) || AlreadyHit(shot, direct)) return;
        uint id = direct.DataID;
        int damage = FumeProjectiles.ModifiedDamage(shot, shot.mCobTargetRow);
        shot.mHitTorchwoodGridX = unchecked((int)(uint)shot.mTargetZombieID);
        shot.mTargetZombieID = (ZombieID)id;
        bool last = ++shot.mCobTargetX == Solar;
        if (last) shot.Die();
        else
        {
            shot.mVelX *= .4f;
            shot.mVelY *= .4f;
            shot.mVelZ = -Math.Max(FusionProjectileTime.SourceVelocity(PuffLobbers.Gravity), Math.Abs(shot.mVelZ) * .5f);
        }
        SolarShooter.DropSun(board, shot.mPosX, shot.mPosY, 5);
        if (direct.DataID == id && SquashFusions.Eligible(board, direct, DamageRangeFlags.Ground))
            direct.TakeDamage(damage, DamageFlags.BypassesShield);
    }

    internal static void FireBlubber(Plant plant, Zombie target)
    {
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f - 10, plant.mRenderOrder + 1, target.mRow, ProjectileType.Butter);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = BlubberButter;
        shot.mCobTargetRow = 40;
        Launch(shot, target, PuffLobbers.FlightTicks);
    }

    internal static void FireShell(Plant shell, Plant shooter, Zombie? target)
    {
        var shot = shell.mBoard.AddProjectile(shooter.mX + shooter.mWidth / 2f,
            shooter.mY + shooter.mHeight / 2f - 10, shooter.mRenderOrder + 1,
            shooter.mRow, ProjectileType.Melon);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = ShellMelon;
        shot.mCobTargetRow = PotBoosts.HasSupport(shooter, FusionPlant.MelonPot) ? -80 : 80;
        Launch(shot, target, PuffLobbers.FlightTicks);
    }

    internal static void Launch(Projectile shot, Zombie? target, int ticks)
    {
        LobberFlight.Launch(shot, target, ticks);
        ImportedPeaVisuals.EnsureMixedLobProjectile(shot);
        ImportedPeaVisuals.EnsureElementalLobProjectile(shot);
    }
    internal static void LaunchAt(Projectile shot, float x, float y, int ticks)
    {
        LobberFlight.LaunchAt(shot, x, y, ticks);
        ImportedPeaVisuals.EnsureMixedLobProjectile(shot);
        ImportedPeaVisuals.EnsureElementalLobProjectile(shot);
    }

    private static Zombie? RicochetTarget(Projectile shot)
    {
        Zombie? nearest = null;
        float distance = float.MaxValue;
        for (int i = 0; i < shot.mBoard.m_zombies.Count; ++i)
        {
            var zombie = shot.mBoard.m_zombies[i];
            if (zombie == null || Math.Abs(zombie.mRow - shot.mRow) != 1
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(shot.mBoard, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            float dx = (rect.xMin + rect.xMax) / 2 - shot.mPosX;
            float dy = (rect.yMin + rect.yMax) / 2 - shot.mPosY;
            float d = dx * dx + dy * dy;
            if (d < distance) { nearest = zombie; distance = d; }
        }
        return nearest;
    }

    internal static void Impact(Projectile shot, Zombie? direct)
    {
        int family = Family(shot);
        var board = shot.mBoard;
        if (family == 0 || shot.mDead || board == null) return;
        if (family == 15) { SolarImpact(shot, direct); return; }
        if (family == 13) { DamageImpact(shot, direct); return; }
        if (direct != null && (!SquashFusions.Eligible(board, direct, DamageRangeFlags.Ground)
            || AlreadyHit(shot, direct))) return;
        uint id = shot.DataID, directId = direct?.DataID ?? 0;
        int damage = Math.Abs(shot.mCobTargetRow);
        int remaining = Remaining(shot);
        bool melon = family is 3 or 4 or 5 or 7 or 8 or 9 or 12;
        var targets = LobberImpact.Targets(shot, direct, melon);
        float x = shot.mPosX, y = shot.mPosY;
        int order = shot.mRenderOrder, savedDamage = shot.mCobTargetRow;
        bool bounce = direct != null && (family is 1 or 2 && shot.mCobTargetX > (family == 1 ? Taco : ButterTaco) - 2
            || family == 5 && shot.mCobTargetX == Overripe);
        if (bounce)
        {
            --shot.mCobTargetX;
            shot.mHitTorchwoodGridX = unchecked((int)(uint)shot.mTargetZombieID);
            shot.mTargetZombieID = (ZombieID)direct!.DataID;
            shot.mVelX *= .5f;
            shot.mVelY *= .5f;
            shot.mVelZ = -Math.Max(FusionProjectileTime.SourceVelocity(PuffLobbers.Gravity), Math.Abs(shot.mVelZ) * .5f);
        }
        else shot.Die();
        if (shot.DataID != id || shot.mBoard?.Pointer != board.Pointer) return;
        int count = Math.Max(1, targets.Count);
        int splash = savedDamage < 0 ? (int)(damage * .67f)
            : Math.Min((int)(damage * .33f), (int)Math.Max(1, damage * 8L / count));
        // Original integer division precedes conversion to seconds.
        int splashButter = Math.Max(50, 4 / count * 100);
        foreach (var (zombie, targetId, primary) in targets)
        {
            if (zombie.DataID != targetId || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)) continue;
            int hitDamage = primary ? damage : splash;
            // Iron butter adds remaining type-1 armor before the native armor/body hit.
            if (family == 17) hitDamage = (int)Math.Min(int.MaxValue, (long)hitDamage + Math.Max(0, zombie.mHelmHealth));
            if (family is 8 or 12) hitDamage = MetalArmor.MelonDamage(zombie, hitDamage);
            zombie.TakeDamage(hitDamage, DamageFlags.BypassesShield);
            if (family is 2 or 4 or 11 or 14 or 17 && zombie.DataID == targetId
                && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground))
            {
                FusionStatuses.Butter(zombie, primary ? 400 : splashButter);
            }
            if (shot.DataID != id || shot.mBoard?.Pointer != board.Pointer || bounce && shot.mDead) return;
        }
        board.mApp.AddTodParticle(x, y, order, melon ? ParticleEffect.MelonSplash : ParticleEffect.CabbageSplat);
        if (family == 7 && direct != null)
        {
            SplitSalad(shot, board, x, y, order, savedDamage < 0, directId);
            return;
        }
        if (family != 6 || direct == null || remaining == 0) return;
        var target = RicochetTarget(shot);
        if (target == null) return;
        uint nextId = target.DataID;
        var next = board.AddProjectile(x, y, order, target.mRow, ProjectileType.Cabbage);
        if (next == null || next.mDead) return;
        next.mCobTargetX = Cabbage - (remaining - 1);
        next.mCobTargetRow = savedDamage;
        if (target.DataID != nextId || !SquashFusions.Eligible(board, target, DamageRangeFlags.Ground))
        { next.Die(); return; }
        Launch(next, target, 75);
    }
    internal static void DamageImpact(Projectile shot, Zombie? direct)
    {
        var board = shot.mBoard;
        uint shotId = shot.DataID, targetId = direct?.DataID ?? 0;
        float x = shot.mPosX, y = shot.mPosY;
        int row = shot.mRow, order = shot.mRenderOrder, damage = Math.Max(0, shot.mCobTargetRow);
        shot.Die();
        if (shot.DataID != shotId || shot.mBoard?.Pointer != board.Pointer || !shot.mDead) return;
        board.mApp.AddTodParticle(x, y, order, ParticleEffect.CabbageSplat);
        if (direct == null || !SquashFusions.Eligible(board, direct, DamageRangeFlags.Ground)) return;
        bool irradiated = EmberEffects.Has(direct);
        direct.TakeDamage(damage, DamageFlags.BypassesShield);
        // Bullet_cabbage_doom checks irradiation AFTER the direct hit.
        // A killed/recycled owner cannot donate an unrelated zombie's status.
        if (direct.DataID != targetId || direct.mBoard?.Pointer != board.Pointer || !(EmberEffects.Has(direct) || irradiated && (direct.mDead || direct.IsDeadOrDying()))) return;
        var targets = new List<(Zombie Zombie, uint Id)>();
        const float radius = 1.5f * 80 / 1.35f;
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.Pointer == direct.Pointer || Math.Abs(zombie.mRow - row) > 1
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= radius * radius) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground))
                zombie.TakeDamage(damage / 3, DamageFlags.BypassesShield);
        board.mApp.AddTodParticle(x, y, order, ParticleEffect.Powie);
    }

    private static void SplitSalad(Projectile parent, Board board, float x, float y, int order, bool boosted, uint directId)
    {
        bool butter = board.Rand(Math.Max(1, parent.mHitTorchwoodGridX)) == 0;
        float vx = parent.mVelX, vy = parent.mVelY, vz = parent.mVelZ, gravity = parent.mAccZ, z = parent.mPosZ;
        int row = parent.mRow;
        foreach (bool corn in new[] { false, true })
        {
            var next = board.AddProjectile(x, y, order, row, ProjectileType.Melon);
            if (next == null || next.mDead) continue;
            next.mCobTargetX = corn ? butter ? ButterCob : Cob : Overripe;
            int damage = corn ? butter ? 160 : 140 : 80;
            next.mCobTargetRow = boosted ? -damage : damage;
            next.mMotionType = ProjectileMotion.Lobbed;
            next.mDamageRangeFlags = DamageRangeFlags.Ground;
            next.mTargetZombieID = (ZombieID)directId;
            next.mHitTorchwoodGridX = 0;
            next.mVelX = vx * .5f;
            next.mVelY = vy * .5f;
            next.mVelZ = -Math.Max(FusionProjectileTime.SourceVelocity(PuffLobbers.Gravity), Math.Abs(vz) * .5f);
            next.mAccZ = gravity;
            next.mPosZ = z - .3f * ScaredyFusions.SourceUnit;
            next.mShadowY = next.mPosY + PuffShooters.FlightHeight;
            // These values were copied from a live native-rate parent.
            FusionProjectileTime.NativeLaunch(next);
            ImportedPeaVisuals.EnsureMixedLobProjectile(next);
        }
    }

}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class MixedLobberImpactPatch
{
    internal static bool Prefix(Projectile __instance, Zombie theZombie)
    {
        if (LobberProjectiles.Family(__instance) == 0) return true;
        LobberProjectiles.Impact(__instance, theZombie); return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.FindCollisionTarget))]
internal static class MixedLobberCollisionPatch
{
    internal static void Postfix(Projectile __instance, ref Zombie __result)
    {
        if (LobberProjectiles.Family(__instance) != 0 && __result != null
            && LobberProjectiles.AlreadyHit(__instance, __result)) __result = null!;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class MixedLobberVisualPatch
{
    internal static void Postfix(Projectile __instance)
    { if (!__instance.mDead && LobberProjectiles.Family(__instance) != 0) ImportedPeaVisuals.EnsureMixedLobProjectile(__instance); }
}
