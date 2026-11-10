using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Saved projectile markers own identity; target row owns attack damage and the
// Torchwood field owns the complete Coldslaw source handle. Native lob motion
// owns movement, collision and persistence. No transient projectile registry.
internal static class ElementalLobbers
{
    private const int Cold = -530001, Hot = -530002, SmallHot = -530003,
        Kernel = -530004, Butter = -530005, GarlicCabbage = -530100,
        GarlicKernel = -530110, GarlicButter = -530111, GarlicMelon = -530112, FireMelon = -530113, HypnoMelon = -530114;

    internal static bool Owns(FusionPlant identity) => identity == FusionPlant.Coldslaw
        || identity == FusionPlant.KimchiPult || identity == FusionPlant.Cobsicle
        || identity == FusionPlant.GarbagePult || identity == FusionPlant.ClovePult
        || identity == FusionPlant.GarlicPult || identity == FusionPlant.SummerMelon || identity == FusionPlant.HypnoMelon;

    internal static int Family(Projectile shot) => shot.mProjectileType switch
    {
        ProjectileType.Cabbage when shot.mCobTargetX == Cold => 1,
        ProjectileType.Cabbage when shot.mCobTargetX == Hot => 2,
        ProjectileType.Cabbage when shot.mCobTargetX == SmallHot => 3,
        ProjectileType.Kernel when shot.mCobTargetX == Kernel => 4,
        ProjectileType.Butter when shot.mCobTargetX == Butter => 5,
        ProjectileType.Cabbage when shot.mCobTargetX == GarlicCabbage || shot.mCobTargetX == GarlicCabbage - 1 => 6,
        ProjectileType.Kernel when shot.mCobTargetX == GarlicKernel => 7,
        ProjectileType.Butter when shot.mCobTargetX == GarlicButter => 8,
        ProjectileType.Melon when shot.mCobTargetX == GarlicMelon => 9,
        ProjectileType.Melon when shot.mCobTargetX == FireMelon => 10,
        ProjectileType.Melon when shot.mCobTargetX == HypnoMelon => 11,
        _ => 0
    };

    internal static void Tick(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        bool fast = identity == FusionPlant.Coldslaw && plant.mTargetX > 150;
        if (plant.mTargetX > 0) --plant.mTargetX;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || PuffLobbers.Target(plant) == null) return;
            plant.mLaunchCounter = fast ? 19 + plant.mBoard.Rand(3) : 285 + plant.mBoard.Rand(31);
            plant.mStartRow = LobberFusions.ShootTicks;
            plant.mShootingCounter = (identity == FusionPlant.Cobsicle || identity == FusionPlant.ClovePult)
                && plant.mBoard.Rand(PotBoosts.ButterRange(plant)) == 0 ? 1 : 0;
        }
        int elapsed = LobberFusions.ShootTicks - plant.mStartRow;
        int rate = fast ? 3 : 1;
        int eventTick = identity == FusionPlant.Cobsicle || identity == FusionPlant.ClovePult
            || identity == FusionPlant.GarlicPult || identity == FusionPlant.SummerMelon
            || identity == FusionPlant.HypnoMelon ? 34 : 32;
        // Crossing, not equality: the source triples Animator speed for 3 s.
        if (elapsed <= eventTick && elapsed + rate > eventTick) Fire(plant);
        plant.mStartRow = Math.Max(0, plant.mStartRow - rate);
        if (plant.mStartRow == 0) plant.mShootingCounter = 0;
    }

    internal static void Fire(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.GarbagePult || identity == FusionPlant.ClovePult
            || identity == FusionPlant.GarlicPult || identity == FusionPlant.SummerMelon || identity == FusionPlant.HypnoMelon)
        { FireSeasoned(plant, identity); return; }
        bool butter = identity == FusionPlant.Cobsicle && plant.mShootingCounter == 1;
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f - 10, plant.mRenderOrder + 1, plant.mRow,
            identity == FusionPlant.Cobsicle ? butter ? ProjectileType.Butter : ProjectileType.Kernel : ProjectileType.Cabbage);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = identity == FusionPlant.Coldslaw ? Cold
            : identity == FusionPlant.KimchiPult ? Hot : butter ? Butter : Kernel;
        shot.mCobTargetRow = identity == FusionPlant.Coldslaw ? 40
            : identity == FusionPlant.KimchiPult ? 60 : butter ? 40 : 20;
        LobberProjectiles.Launch(shot, PuffLobbers.Target(plant), PuffLobbers.FlightTicks);
        shot.mHitTorchwoodGridX = identity == FusionPlant.Coldslaw ? unchecked((int)plant.DataID) : 0;
    }

    private static bool Eligible(Board board, Zombie zombie) =>
        zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground);

    internal static void Impact(Projectile shot, Zombie? direct)
    {
        var board = shot.mBoard;
        int family = Family(shot);
        if (family == 0 || shot.mDead || board == null) return;
        if (family >= 6) { ImpactSeasoned(shot, direct, family); return; }
        if (direct != null && (!Eligible(board, direct) || (uint)shot.mTargetZombieID == direct.DataID)) return;
        uint shotId = shot.DataID, targetId = direct?.DataID ?? 0;
        int damage = shot.mCobTargetRow;
        float x = shot.mPosX, y = shot.mPosY;
        int row = shot.mRow, order = shot.mRenderOrder;
        int column = direct == null ? 0 : board.PixelToGridX(
            (direct.GetZombieRect().xMin + direct.GetZombieRect().xMax) / 2, y);
        bool frozen = false;
        if (direct != null)
        {
            if (family == 1)
            {
                SnowPea.Hit(direct);
                frozen = direct.mIceTrapCounter > 0;
            }
            else if (family is 4 or 5) frozen = direct.mIceTrapCounter > 0;
            else Overheat.Apply(direct);
        }
        bool bounce = family == 1 && direct != null && frozen;
        if (bounce)
        {
            shot.mTargetZombieID = (ZombieID)targetId;
            // First hit: x *= .4, y *= -.5. Later hits reflect vertical
            // velocity only. mCobTargetRow remains the original attack value.
            if (shot.mVelZ > 0)
            {
                if (shot.mCobTargetRow > 0) { shot.mVelX *= .4f; shot.mVelY *= .4f; shot.mVelZ *= -.5f; }
                else shot.mVelZ *= -1;
            }
            shot.mCobTargetRow = -Math.Abs(damage);
            BoostOwner(shot);
        }
        else shot.Die();
        if (shot.DataID != shotId || shot.mBoard?.Pointer != board.Pointer) return;
        if (direct != null && direct.DataID == targetId && Eligible(board, direct))
        {
            direct.TakeDamage(Math.Abs(damage) * (frozen ? 4 : 1), DamageFlags.BypassesShield);
            if (direct.DataID == targetId && Eligible(board, direct))
            {
                if (family is 1 or 4)
                {
                    FusionStatuses.Chill(direct);
                    if (family == 4) SnowPea.Hit(direct);
                }
                else if (family == 5) ChomperVariants.Freeze(direct, 400);
            }
        }
        if (shot.DataID != shotId || shot.mBoard?.Pointer != board.Pointer || bounce && shot.mDead) return;
        board.mApp.AddTodParticle(x, y, order, ParticleEffect.CabbageSplat);
        if (family == 2 && direct != null) Split(board, x, y, row, column, order, Math.Abs(damage) / 3);
    }

    private static void FireSeasoned(Plant plant, FusionPlant identity)
    {
        bool corn = identity == FusionPlant.ClovePult, butter = corn && plant.mShootingCounter == 1;
        bool melon = identity == FusionPlant.GarlicPult || identity == FusionPlant.SummerMelon || identity == FusionPlant.HypnoMelon;
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f - 10, plant.mRenderOrder + 1, plant.mRow,
            melon ? ProjectileType.Melon : corn ? butter ? ProjectileType.Butter : ProjectileType.Kernel : ProjectileType.Cabbage);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = identity == FusionPlant.GarbagePult ? GarlicCabbage
            : corn ? butter ? GarlicButter : GarlicKernel
            : identity == FusionPlant.GarlicPult ? GarlicMelon : identity == FusionPlant.HypnoMelon ? HypnoMelon : FireMelon;
        int damage = identity == FusionPlant.SummerMelon || identity == FusionPlant.HypnoMelon ? 120 : melon ? 100 : butter ? 60 : 40;
        shot.mCobTargetRow = melon && PotBoosts.HasSupport(plant, FusionPlant.MelonPot) ? -damage : damage;
        LobberProjectiles.Launch(shot, PuffLobbers.Target(plant), PuffLobbers.FlightTicks);
    }

    internal static bool AlreadyHit(Projectile shot, Zombie zombie) =>
        (uint)shot.mTargetZombieID == zombie.DataID
        || Family(shot) == 6 && unchecked((uint)shot.mHitTorchwoodGridX) == zombie.DataID;

    private static void ImpactSeasoned(Projectile shot, Zombie? direct, int family)
    {
        var board = shot.mBoard;
        if (direct != null && (!Eligible(board, direct) || AlreadyHit(shot, direct))) return;
        uint id = shot.DataID;
        int damage = Math.Abs(shot.mCobTargetRow), order = shot.mRenderOrder;
        float x = shot.mPosX, y = shot.mPosY;
        bool melon = family >= 9;
        var targets = LobberImpact.Targets(shot, direct, melon);
        // A pre-existing garlic level refunds this hit's penetration before
        // damage or AddPoisonLevel can change the target's status.
        bool free = direct != null && PoisonShooters.Level(direct) > 0;
        bool bounce = family == 6 && direct != null && (free || shot.mCobTargetX == GarlicCabbage);
        int splash = shot.mCobTargetRow < 0 ? (int)(damage * .67f)
            : Math.Min((int)(damage * .33f), (int)Math.Max(1, damage * 8L / Math.Max(1, targets.Count)));
        if (bounce)
        {
            if (!free) --shot.mCobTargetX;
            shot.mHitTorchwoodGridX = unchecked((int)(uint)shot.mTargetZombieID);
            shot.mTargetZombieID = (ZombieID)direct!.DataID;
            if (shot.mVelZ > 0)
            {
                if (shot.mCobTargetRow > 0)
                { shot.mVelX *= .4f; shot.mVelY *= .4f; shot.mVelZ *= -.5f; }
                else shot.mVelZ *= -1;
            }
            // Cabbage does not use the sign for melon support. Preserve its
            // first/later bounce mode independently of the refunded hit count.
            shot.mCobTargetRow = -damage;
        }
        else shot.Die();
        if (shot.DataID != id || shot.mBoard?.Pointer != board.Pointer) return;
        foreach (var (zombie, targetId, primary) in targets)
        {
            if (zombie.DataID != targetId || !Eligible(board, zombie)) continue;
            int bonus = family >= 10 ? 0 : PoisonShooters.Level(zombie) * (primary ? 20 : 5);
            zombie.TakeDamage((primary ? damage : splash) + bonus, DamageFlags.BypassesShield);
            if (zombie.DataID == targetId && Eligible(board, zombie))
            {
                if (family == 11) HypnoPlants.MelonHit(zombie);
                else if (family == 10)
                {
                    if (BloverEffects.ReserveOverheat(zombie)) Overheat.Apply(zombie);
                }
                else if (family == 8)
                {
                    FusionStatuses.Butter(zombie, 400);
                    if (BloverEffects.Reserve(zombie, false)) PoisonShooters.ApplyPoison(board, zombie, 1000);
                }
                else if (BloverEffects.Reserve(zombie, false)) PoisonShooters.AddLevel(board, zombie);
            }
            if (shot.DataID != id || shot.mBoard?.Pointer != board.Pointer || bounce && shot.mDead) return;
        }
        board.mApp.AddTodParticle(x, y, order, melon ? ParticleEffect.MelonSplash : ParticleEffect.CabbageSplat);
    }

    private static void BoostOwner(Projectile shot)
    {
        uint owner = unchecked((uint)shot.mHitTorchwoodGridX);
        for (int i = 0; i < shot.mBoard.m_plants.Count; ++i)
        {
            var plant = shot.mBoard.m_plants[i];
            if (plant == null || plant.DataID != owner || FusionPlant.From(plant) != FusionPlant.Coldslaw
                || !ChomperFusions.Active(plant) || plant.mTargetX != 0) continue;
            // 3 seconds active, then 1.5 seconds recovery. Fertilizer must not
            // reset this saved clock or shorten its retrigger protection.
            plant.mTargetX = 450;
            plant.mLaunchCounter = 20;
        }
    }

    private static void Split(Board board, float x, float y, int row, int col, int order, int damage)
    {
        for (int r = Math.Max(0, row - 1); r <= Math.Min(board.GetNumRows() - 1, row + 1); ++r)
            for (int c = Math.Max(0, col - 1); c <= Math.Min(8, col + 1); ++c)
            {
                var next = board.AddProjectile(x, y, order, r, ProjectileType.Cabbage);
                if (next == null || next.mDead) continue;
                next.mCobTargetX = SmallHot;
                next.mCobTargetRow = damage;
                LobberProjectiles.LaunchAt(next, board.GridToPixelX(c, r) + 40,
                    board.GridToPixelY(c, r) + 30, 80);
            }
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class ElementalLobberImpactPatch
{
    internal static bool Prefix(Projectile __instance, Zombie theZombie)
    {
        if (ElementalLobbers.Family(__instance) == 0) return true;
        ElementalLobbers.Impact(__instance, theZombie); return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.FindCollisionTarget))]
internal static class ElementalLobberCollisionPatch
{
    internal static void Postfix(Projectile __instance, ref Zombie __result)
    {
        if (ElementalLobbers.Family(__instance) != 0 && __result != null
            && ElementalLobbers.AlreadyHit(__instance, __result)) __result = null!;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class ElementalLobberVisualPatch
{
    internal static void Postfix(Projectile __instance)
    { if (!__instance.mDead && ElementalLobbers.Family(__instance) != 0) ImportedPeaVisuals.EnsureElementalLobProjectile(__instance); }
}
