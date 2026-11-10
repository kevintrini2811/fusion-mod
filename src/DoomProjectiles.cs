using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// All live state resides in native serialized projectile fields. Neither a
// producer pointer nor a transient dictionary is needed when loading a shot.
internal static class DoomProjectiles
{
    private const int Marker = -580000;
    internal const float Unit = 80f / 1.35f;
    internal static int Family(Projectile shot) =>
        shot.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
        && shot.mCobTargetX is <= Marker - 1 and >= Marker - 4 ? Marker - (int)shot.mCobTargetX : 0;

    internal static void Track(Projectile shot, int family, int damage)
    {
        shot.mCobTargetX = Marker - family;
        shot.mCobTargetRow = damage;
        ImportedPeaVisuals.EnsureDoomProjectile(shot);
    }

    internal static bool Impact(Projectile shot, Zombie? zombie)
    {
        int family = Family(shot);
        if (family == 0) return false;
        if (shot.mDead || family == 4) return true;
        var board = shot.mBoard;
        float x = shot.mPosX, y = shot.mPosY;
        int row = shot.mRow, order = shot.mRenderOrder;
        int damage = FumeProjectiles.ModifiedDamage(shot, Math.Max(0, shot.mCobTargetRow));
        uint id = zombie?.DataID ?? 0;
        shot.Die(); // Commit before callbacks, even if the hit is lethal.
        if (zombie == null || !Live(board, zombie)) return true;
        // Bullet_doom.HitZombie: the enlarged shot runs its radius-three
        // explosion before the ordinary shield-bypassing direct hit.
        if (family == 3) Blast(board, x, y, row, order, damage);
        if (zombie.DataID == id && Live(board, zombie)) zombie.TakeDamage(damage, DamageFlags.BypassesShield);
        board.mApp.AddTodParticle(x, y, order, ParticleEffect.Powie);
        return true;
    }

    private static bool Live(Board board, Zombie zombie) => !zombie.mDead && !zombie.IsDeadOrDying()
        && !zombie.mMindControlled && zombie.mBoard?.Pointer == board.Pointer;

    internal static void Blast(Board board, float x, float y, int row, int order, int damage)
    {
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || !Live(board, zombie)
                || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Flying)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= 9 * Unit * Unit) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && Live(board, zombie)) DoomEffects.AshDamage(zombie, damage, DamageFlags.BypassesShield);
        board.mApp.AddTodParticle(x, y, order, ParticleEffect.Doom);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Explosion);
        board.ShakeBoard(3, -4);
    }

    internal static Projectile? Meteor(Plant plant)
    {
        var board = plant.mBoard;
        // BigStar.Start chooses the centre row (or the midpoint of the two
        // centre rows). The plant meteor keeps its prefab (-5.7,9),
        // +7 units/s and g=-9.8. (15,9), -7 belongs to hostile meteors.
        float ground = (board.GridToPixelY(0, (board.GetNumRows() - 1) / 2)
            + board.GridToPixelY(0, board.GetNumRows() / 2)) * .5f + 50;
        float x = board.GridToPixelX(0, 0) - .1f * Unit;
        float yUnit = (board.GridToPixelY(0, 1) - board.GridToPixelY(0, 0))
            / (board.GetNumRows() == 6 ? 1.45f : 1.67f);
        float y = board.GridToPixelY(0, 0) + 50 - 6.2f * yUnit;
        var shot = board.AddProjectile(x, y, plant.mRenderOrder + 1,
            board.GetNumRows() / 2, ProjectileType.Pea);
        if (shot == null || shot.mDead) return null;
        shot.mMotionType = ProjectileMotion.Star;
        shot.mVelX = 7 * Unit / 100;
        shot.mVelY = 0;
        shot.mAccZ = 9.8f * yUnit / 10000;
        shot.mVelZ = ground; // Landing plane, unused by this custom motion.
        shot.mPosZ = 0;
        shot.mShadowY = ground;
        shot.mDamageRangeFlags = DamageRangeFlags.Ground | DamageRangeFlags.Flying;
        Track(shot, 4, 1800);
        return shot;
    }

    internal static void TickMeteor(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board.mPaused || board.mApp.GameScene != GameScenes.Playing
            || board.Pointer == CherryBombPlants.DisposingBoard) return;
        if (!FusionTime.SourceStep(shot, typeof(DoomProjectiles))) return;
        ++shot.mProjectileAge;
        shot.mVelY += shot.mAccZ;
        shot.mPosX += shot.mVelX;
        shot.mPosY += shot.mVelY;
        shot.mX = shot.mPosX;
        shot.mY = shot.mPosY;
        if (shot.mPosY < shot.mVelZ) return;
        float x = shot.mPosX, y = shot.mVelZ;
        int order = shot.mRenderOrder, row = shot.mRow;
        int damage = Math.Max(0, shot.mCobTargetRow);
        shot.Die();
        Blast(board, x, y, row, order, damage);
        // BigStar.CreateStars: five rings, half a source unit apart, 36
        // stars per ring. Every star moves away at its original angle.
        for (int ring = 1; ring <= 5; ++ring)
        for (int angle = 0; angle < 360; angle += 10)
        {
            float radians = angle * MathF.PI / 180;
            StarProjectiles.Spawn(board, x + MathF.Cos(radians) * ring * .5f * Unit,
                y - MathF.Sin(radians) * ring * .5f * Unit, order, row, 6, angle, 4, 20);
        }
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class DoomProjectileImpactPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Projectile __instance, Zombie theZombie) => !DoomProjectiles.Impact(__instance, theZombie);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class DoomProjectileUpdatePatch
{
    private static bool Prefix(Projectile __instance)
    {
        if (DoomProjectiles.Family(__instance) != 4) return true;
        DoomProjectiles.TickMeteor(__instance); return false;
    }
    private static void Postfix(Projectile __instance)
    { if (!__instance.mDead && DoomProjectiles.Family(__instance) != 0) ImportedPeaVisuals.EnsureDoomProjectile(__instance); }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class DoomProjectileIgnitionPatch
{ private static bool Prefix(Projectile __instance) => DoomProjectiles.Family(__instance) == 0; }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToPea))]
internal static class DoomProjectileThawPatch
{ private static bool Prefix(Projectile __instance) => DoomProjectiles.Family(__instance) == 0; }
