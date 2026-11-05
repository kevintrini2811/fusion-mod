using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Saved cob fields encode family/damage; native Star movement owns row changes
// and collision. No transient projectile state is required after loading.
internal static class StarProjectiles
{
    internal static readonly float[] Angles = { 30, -30, 180, 90, -90 };
    private const int Marker = -540000;
    internal static int Family(Projectile shot) => shot.mCobTargetX switch
    {
        Marker - 9 when shot.mProjectileType == ProjectileType.Star => 9,
        Marker - 1 when shot.mProjectileType == ProjectileType.Star => 1,
        Marker - 2 when shot.mProjectileType == ProjectileType.Star => 2,
        Marker - 3 when shot.mProjectileType == ProjectileType.Star => 3,
        Marker - 4 when shot.mProjectileType == ProjectileType.Pea => 4,
        Marker - 6 when shot.mProjectileType == ProjectileType.Star => 6,
        Marker - 5 when shot.mProjectileType == ProjectileType.Star => 5,
        Marker - 8 when shot.mProjectileType == ProjectileType.Star => 8,
        Marker - 7 when shot.mProjectileType == ProjectileType.Star => 7,
        _ => 0
    };

    internal static void Ignite(Projectile shot, int col)
    {
        int family = Family(shot);
        int damage = family == 0 ? 20 : Math.Max(0, shot.mCobTargetRow);
        ImportedPeaVisuals.ForgetProjectile(shot);
        shot.mVelZ = shot.mAccZ = shot.mPosZ = 0;
        shot.mTargetZombieID = (ZombieID)0;
        shot.mCobTargetX = Marker - 9;
        shot.mCobTargetRow = (int)Math.Min(int.MaxValue, (long)damage + 20);
        shot.mHitTorchwoodGridX = col;
        ImportedPeaVisuals.EnsureStarProjectile(shot);
    }

    internal static Projectile? Spawn(Board board, float x, float y, int order, int row,
        int family, float angle, float speed, int damage)
    {
        var shot = board.AddProjectile(x, y, order + 1, row, family == 4 ? ProjectileType.Pea : ProjectileType.Star);
        if (shot == null || shot.mDead) return null;
        shot.mCobTargetX = Marker - family;
        shot.mCobTargetRow = Math.Max(0, damage);
        shot.mMotionType = ProjectileMotion.Star;
        float radians = angle * MathF.PI / 180;
        shot.mVelX = MathF.Cos(radians) * speed;
        shot.mVelY = -MathF.Sin(radians) * speed;
        shot.mVelZ = shot.mAccZ = shot.mPosZ = 0;
        // Native UpdateNormalMotion hits land below 23px (Star) / 28px (Pea).
        shot.mShadowY = y + PuffShooters.FlightHeight;
        shot.mDamageRangeFlags = DamageRangeFlags.Ground;
        shot.mHitTorchwoodGridX = 0;
        ImportedPeaVisuals.EnsureStarProjectile(shot);
        return shot;
    }

    internal static void BeforeMove(Projectile shot)
    {
        int family = Family(shot);
        if (family == 0 || shot.mDead || shot.mBoard.mPaused) return;
        if (family == 2)
        {
            // IceStar's coroutine is cancelled when its source is destroyed.
            bool owner = false;
            for (int i = 0; i < shot.mBoard.m_plants.Count; ++i)
                if (shot.mBoard.m_plants[i] is { } plant && !plant.mDead
                    && plant.DataID == (uint)shot.mTargetZombieID
                    && FusionPlant.From(plant) == FusionPlant.WinterStarfruit) { owner = true; break; }
            shot.mVelX = shot.mVelY = 0;
            if (!owner) return;
            float seconds = shot.mProjectileAge * .01f;
            float angle = (shot.mHitTorchwoodGridX + seconds * 360) * MathF.PI / 180;
            float radius = seconds * 2 * (80f / 1.35f);
            shot.mVelX = shot.mVelZ + MathF.Cos(angle) * radius - shot.mPosX;
            shot.mVelY = shot.mAccZ - MathF.Sin(angle) * radius - shot.mPosY;
        }
        else if (family == 4 && shot.mHitTorchwoodGridX < 5
            && shot.mProjectileAge >= 20 + shot.mHitTorchwoodGridX * 50)
        {
            ++shot.mHitTorchwoodGridX;
            int damage = FumeProjectiles.Damage(shot, shot.mCobTargetRow);
            // Commit before allocation/callbacks. Children never emit children.
            for (int angle = -36; angle < 324; angle += 72)
                Spawn(shot.mBoard, shot.mPosX, shot.mPosY, shot.mRenderOrder, shot.mRow, 5, angle, 4, damage);
        }
    }

    internal static bool Impact(Projectile shot, Zombie? zombie)
    {
        int family = Family(shot);
        if (family == 0) return false;
        if (shot.mDead) return true;
        var board = shot.mBoard;
        uint id = zombie?.DataID ?? 0;
        int damage = FumeProjectiles.ModifiedDamage(shot, Math.Max(0, shot.mCobTargetRow));
        var flags = zombie == null ? (DamageFlags)0 : shot.GetDamageFlags(zombie);
        float x = shot.mPosX, y = shot.mPosY;
        int order = shot.mRenderOrder;
        shot.Die();
        if (zombie == null || !Live(board, zombie)) return true;
        if (family == 1) damage = (int)Math.Min(int.MaxValue, (long)damage + 5 * PoisonShooters.Level(zombie));
        if (family == 2)
        {
            if (BloverEffects.Reserve(zombie, true)) SnowPea.Hit(zombie, 10);
            if (zombie.DataID != id || !Live(board, zombie)) return true;
            if (zombie.mIceTrapCounter > 0) damage = (int)Math.Min(int.MaxValue, 4L * damage);
        }
        if (family is 3 or 9) zombie.RemoveColdEffects();
        if (family == 7) damage = MetalArmor.StarDamage(zombie, damage);
        if (family == 8) damage = MetalArmor.IronStarDamage(zombie, damage);
        // Reserve splash handles before direct damage can recycle the victim.
        var splash = new List<(Zombie Zombie, uint Id)>();
        if (family == 9)
            for (int i = 0; i < board.m_zombies.Count; ++i)
                if (board.m_zombies[i] is { } other && other.Pointer != zombie.Pointer && Live(board, other)
                    && other.mRow == zombie.mRow && Math.Abs(other.GetZombieRect().center.x - x) <= 80)
                    splash.Add((other, other.DataID));
        zombie.TakeDamage(damage, flags);
        foreach (var (other, otherId) in splash)
            if (other.DataID == otherId && Live(board, other))
            { other.RemoveColdEffects(); other.TakeDamage(damage / 3, flags); }

        if (zombie.DataID == id && Live(board, zombie))
        {
            if (family == 1 && BloverEffects.Reserve(zombie, false)) PoisonShooters.AddLevel(board, zombie);
            if (family == 2) zombie.ApplyChill(false);
        }
        board.mApp.AddTodParticle(x, y, order, family == 2 ? ParticleEffect.SnowpeaSplat
            : family == 3 ? ParticleEffect.Powie : ParticleEffect.PeaSplat);
        return true;
    }

    private static bool Live(Board board, Zombie zombie) => !zombie.mDead && !zombie.IsDeadOrDying()
        && !zombie.mMindControlled && zombie.mBoard?.Pointer == board.Pointer;
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class StarMotionPatch
{
    private static void Prefix(Projectile __instance) => StarProjectiles.BeforeMove(__instance);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class StarImpactPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Projectile __instance, Zombie theZombie) => !StarProjectiles.Impact(__instance, theZombie);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class StarProjectileVisualPatch
{
    private static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && StarProjectiles.Family(__instance) != 0)
            ImportedPeaVisuals.EnsureStarProjectile(__instance);
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class StarIgnitionPatch
{
    private static bool Prefix(Projectile __instance) => StarProjectiles.Family(__instance) == 0;
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToPea))]
internal static class StarThawPatch
{
    private static bool Prefix(Projectile __instance) => StarProjectiles.Family(__instance) == 0;
}
