using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The native normal-motion path owns flight, age and pooling. Saved marker,
// damage and two previous handles are sufficient for at most three pierces.
internal static class CactusProjectiles
{
    private const int Ice = -570100, Magnet = -570110, Doom = -570120, Light = -570130, StarMarker = -570140, LightSingle = -570150, Thorn = -570160, Ball = -570170;
    internal static int Family(Projectile shot) => shot.mProjectileType == ProjectileType.Star && shot.mCobTargetX == StarMarker ? 5
        : shot.mProjectileType != ProjectileType.Spike ? 0
        : shot.mCobTargetX is <= Ice and >= Ice - 2 ? 1 : shot.mCobTargetX == Magnet ? 2
        : shot.mCobTargetX is <= Doom and >= Doom - 2 ? 3
        : shot.mCobTargetX is <= Light and >= Light - 2 ? 4 : shot.mCobTargetX == LightSingle ? 6 : shot.mCobTargetX == Thorn ? 7 : shot.mCobTargetX == Ball ? 8 : 0;
    internal static int Remaining(Projectile shot) => Family(shot) switch
    { 1 => Ice - (int)shot.mCobTargetX + 1, 3 => Doom - (int)shot.mCobTargetX + 1,
        4 => Light - (int)shot.mCobTargetX + 1, _ => 1 };
    internal static bool AlreadyHit(Projectile shot, Zombie zombie) =>
        (uint)shot.mTargetZombieID == zombie.DataID || unchecked((uint)shot.mHitTorchwoodGridX) == zombie.DataID;

    internal static Projectile? Spawn(Plant plant, float angle = 0, bool burst = false)
    {
        var identity = FusionPlant.From(plant);
        bool ice = identity == FusionPlant.IceCactus, thorn = identity == FusionPlant.Thornweed;
        bool doom = identity == FusionPlant.DoomCactus, light = identity == FusionPlant.LumosCactus;
        bool ball = identity == FusionPlant.BallCactus;
        bool air = !ball && plant.mTargetX != 0;
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f + (thorn ? 0 : 15),
            thorn ? plant.mY + plant.mHeight - 10 : plant.mY + plant.mHeight / 2f - (air ? CactusFusions.Unit : 0),
            plant.mRenderOrder + 1, plant.mRow, ProjectileType.Spike);
        if (shot == null || shot.mDead) return null;
        shot.mCobTargetX = ball ? Ball : thorn ? Thorn : ice ? Ice - 2 : doom ? Doom - 2 : light ? (PumpkinFusions.LightLevel(plant) >= 1 ? Light - 2 : LightSingle) : Magnet;
        shot.mCobTargetRow = ball ? 30 : ice || thorn ? 20 : light ? 30 : 40;
        shot.mTargetZombieID = 0;
        shot.mHitTorchwoodGridX = 0;
        shot.mMotionType = ProjectileMotion.Star;
        float radians = angle * MathF.PI / 180;
        shot.mVelX = MathF.Cos(radians) * 10 * CactusFusions.Unit / 100;
        shot.mVelY = -MathF.Sin(radians) * 10 * CactusFusions.Unit / 100;
        shot.mVelZ = shot.mAccZ = shot.mPosZ = 0;
        shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
        shot.mDamageRangeFlags = burst || ball ? DamageRangeFlags.Ground | DamageRangeFlags.Flying
            : air ? DamageRangeFlags.Flying : DamageRangeFlags.Ground;
        ImportedPeaVisuals.EnsureCactusProjectile(shot);
        return shot;
    }

    internal static bool AirTarget(Board board, Zombie zombie) => zombie.IsFlying()
        && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Flying);

    internal static Projectile? Star(Plant plant, float angle)
    {
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f, plant.mRenderOrder + 1, plant.mRow, ProjectileType.Star);
        if (shot == null || shot.mDead) return null;
        shot.mCobTargetX = StarMarker;
        shot.mCobTargetRow = 20;
        shot.mTargetZombieID = (ZombieID)(CactusFusions.StarTarget(plant)?.DataID ?? 0);
        shot.mHitTorchwoodGridX = 0;
        shot.mMotionType = ProjectileMotion.Star;
        shot.mVelX = MathF.Cos(angle * MathF.PI / 180) * 4;
        shot.mVelY = -MathF.Sin(angle * MathF.PI / 180) * 4;
        shot.mVelZ = shot.mAccZ = shot.mPosZ = 0;
        shot.mDamageRangeFlags = DamageRangeFlags.Flying;
        shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
        ImportedPeaVisuals.EnsureCactusProjectile(shot);
        return shot;
    }
    internal static void BeforeMove(Projectile shot)
    {
        if (Family(shot) != 5 || shot.mDead || shot.mBoard.mPaused) return;
        Zombie? target = null;
        for (int i = 0; i < shot.mBoard.m_zombies.Count; ++i)
        {
            var candidate = shot.mBoard.m_zombies[i];
            if (candidate == null || !AirTarget(shot.mBoard, candidate)) continue;
            target ??= candidate;
            if (candidate.DataID == (uint)shot.mTargetZombieID) { target = candidate; break; }
        }
        shot.mTargetZombieID = (ZombieID)(target?.DataID ?? 0);
        if (target == null) return;
        var rect = target.GetZombieRect();
        float dx = rect.center.x - shot.mPosX, dy = rect.center.y - shot.mPosY;
        float distance = MathF.Sqrt(dx * dx + dy * dy);
        if (distance < .001f) return;
        float step = Math.Min(4, distance);
        shot.mVelX = dx / distance * step;
        shot.mVelY = dy / distance * step;
    }
    internal static bool Eligible(Projectile shot, Zombie zombie) =>
        zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && (Family(shot) == 5 ? AirTarget(shot.mBoard, zombie)
            : SquashFusions.Eligible(shot.mBoard, zombie, shot.mDamageRangeFlags)
                && !AlreadyHit(shot, zombie))
        && (Family(shot) is 3 or 5 or 7 || zombie.mRow == shot.mRow);

    // A segment against an expanded rectangle handles every cone direction,
    // homing turns and fast flight without a row approximation or tunnelling.
    internal static bool Swept(Projectile shot, float minX, float minY, float maxX, float maxY)
    {
        float startX = shot.mPosX - shot.mVelX, startY = shot.mPosY - shot.mVelY;
        float lo = 0, hi = 1;
        bool Axis(float start, float delta, float min, float max)
        {
            if (Math.Abs(delta) < .00001f) return start >= min && start <= max;
            float a = (min - start) / delta, b = (max - start) / delta;
            lo = Math.Max(lo, Math.Min(a, b)); hi = Math.Min(hi, Math.Max(a, b));
            return lo <= hi;
        }
        return Axis(startX, shot.mVelX, minX - 4, maxX + 4)
            && Axis(startY, shot.mVelY, minY - 4, maxY + 4);
    }

    internal static void Collide(Projectile shot)
    {
        if (shot.mDead || shot.mBoard == null || shot.mBoard.mPaused) return;
        // Swept X interval prevents fast spikes tunnelling through narrow
        // targets. Ground/air flags retain the chosen source attack lane.
        var hits = new List<(Zombie Zombie, uint Id, float X)>();
        for (int i = 0; i < shot.mBoard.m_zombies.Count; ++i)
        {
            var zombie = shot.mBoard.m_zombies[i];
            if (zombie == null || !Eligible(shot, zombie)) continue;
            var rect = zombie.GetZombieRect();
            if (Family(shot) is 3 or 5 or 7 ? Swept(shot, rect.xMin, rect.yMin, rect.xMax, rect.yMax)
                : rect.xMax >= shot.mPosX - Math.Abs(shot.mVelX) - 4 && rect.xMin <= shot.mPosX + 4)
                hits.Add((zombie, zombie.DataID, rect.xMin));
        }
        foreach (var (zombie, id, _) in hits.OrderBy(hit => hit.X))
        {
            if (shot.mDead) break;
            if (zombie.DataID == id && Eligible(shot, zombie)) Impact(shot, zombie);
        }
    }

    internal static bool Impact(Projectile shot, Zombie? zombie)
    {
        int family = Family(shot);
        if (family == 0) return false;
        if (shot.mDead) return true;
        if (zombie == null) { shot.Die(); return true; }
        if (!Eligible(shot, zombie)) return true;
        var impact = family == 8 ? PlantLibrary.ProjectileImpact.Capture(shot) : null;
        var board = shot.mBoard;
        uint id = zombie.DataID;
        float x = shot.mPosX, y = shot.mPosY;
        int order = shot.mRenderOrder;
        bool air = zombie.IsFlying();
        int damage = FumeProjectiles.ModifiedDamage(shot, Math.Max(0, shot.mCobTargetRow));
        var flags = shot.GetDamageFlags(zombie);
        // Commit hit history before status, damage or pool callbacks.
        if (family != 5)
        {
            shot.mHitTorchwoodGridX = unchecked((int)(uint)shot.mTargetZombieID);
            shot.mTargetZombieID = (ZombieID)id;
        }
        if (family is 2 or 5 || family == 1 && !air || Remaining(shot) <= 1) shot.Die();
        else ++shot.mCobTargetX;
        if (family == 1)
        {
            SnowPea.Hit(zombie, 10);
            damage = (int)Math.Min(int.MaxValue, damage * (air ? 4L : 1L)
                * (zombie.mIceTrapCounter > 0 ? 4L : 1L));
            zombie.ApplyChill(false);
        }
        else if (family == 2) damage = MetalArmor.StarDamage(zombie, damage);
        zombie.TakeDamage(damage, flags);
        if (family == 8 && zombie.DataID == id && zombie.mBoard?.Pointer == board.Pointer
            && !zombie.mDead && !zombie.IsDeadOrDying()) PuffLobbers.Shrink(zombie);
        if (family == 3 && zombie.DataID == id && zombie.mBoard?.Pointer == board.Pointer) EmberEffects.Apply(zombie);
        if (zombie.DataID == id && zombie.mBoard?.Pointer == board.Pointer
            && !zombie.mDead && !zombie.IsDeadOrDying() && family != 8 && air && zombie.IsFlying()
            && zombie.mZombieType == ZombieType.Balloon)
            zombie.LandFlyer(flags);
        if (impact != null)
        {
            PlantLibrary.Projectiles.EmitImpactEffect("fusion:ball-cactus-impact", impact);
            PlantLibrary.Projectiles.PlaySound("fusion:" + Il2CppReloaded.Services.FoleyType.Splat, board);
        }
        else board.mApp.AddTodParticle(x, y, order,
            family == 1 ? ParticleEffect.SnowpeaSplat : ParticleEffect.PeaSplat);
        return true;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class CactusCollisionPatch
{
    private static bool Prefix(Projectile __instance)
    {
        if (CactusProjectiles.Family(__instance) == 0) return true;
        CactusProjectiles.Collide(__instance); return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class CactusImpactPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Projectile __instance, Zombie theZombie) => !CactusProjectiles.Impact(__instance, theZombie);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class CactusProjectileVisualPatch
{
    private static void Postfix(Projectile __instance)
    { if (!__instance.mDead && CactusProjectiles.Family(__instance) != 0) ImportedPeaVisuals.EnsureCactusProjectile(__instance); }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class CactusIgnitionPatch
{ private static bool Prefix(Projectile __instance) => CactusProjectiles.Family(__instance) == 0; }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToPea))]
internal static class CactusThawPatch
{ private static bool Prefix(Projectile __instance) => CactusProjectiles.Family(__instance) == 0; }

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class CactusHomingMotionPatch
{ private static void Prefix(Projectile __instance) => CactusProjectiles.BeforeMove(__instance); }
