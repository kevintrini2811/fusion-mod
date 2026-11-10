using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// One owner for nut/chunk movement, ground eligibility and once-per-target
// piercing. Native Projectile.Update still owns age, render position and pool.
internal static class RollingProjectiles
{
    internal sealed class Shot
    {
        internal IntPtr Board;
        internal uint Id;
        internal bool Nut;
        internal bool Weed;
        internal int Chunk;
        internal bool SolarBall;
        internal uint SourceId;
        internal int ChargeTicks, Power = 1, Damage = 600, HitTicks;
        internal float Scale = 1;
        internal float AnimationTime;
        internal float AnimationSpeed => Math.Max(1, 2 - .1f * (Power - 1));
        internal bool Knockback;
        internal readonly HashSet<uint> Hit = new();
    }
    private static readonly Dictionary<IntPtr, Shot> Shots = new();

    internal static bool TryShot(Projectile projectile, out Shot shot)
    {
        if (Shots.TryGetValue(projectile.Pointer, out shot!) && shot.Id == projectile.DataID
            && shot.Board == projectile.mBoard?.Pointer) return true;
        int chunk = Chunk(projectile);
        bool ignitedNut = projectile.mCobTargetX == IgnitedNut;
        bool weed = IsWeed(projectile);
        if (chunk == 0 && !ignitedNut && !weed || projectile.mBoard == null) return false;
        // Saved markers reconstruct original chunk graphics and knockback on load.
        shot = new Shot { Board = projectile.mBoard.Pointer, Id = projectile.DataID,
            Nut = ignitedNut, Weed = weed, Chunk = chunk, Knockback = chunk == 3 && !Hungry(projectile) };
        Shots[projectile.Pointer] = shot;
        if (weed) ImportedPeaVisuals.EnsureWeedProjectile(projectile);
        else ImportedPeaVisuals.RegisterRollingProjectile(projectile, false);
        return true;
    }
    private const int WeedMarker = -625001;
    internal static bool IsWeed(Projectile shot) => shot.mProjectileType == ProjectileType.Pea && shot.mCobTargetX == WeedMarker;

    internal static Projectile? SpawnWeed(Plant plant, int row)
    {
        const float unit = 80f / 1.35f;
        // Original Shoot minus Shadow: (.6744, .0616), including prefab scale.
        float x = plant.mX + plant.mWidth / 2f + .7744f * unit;
        var shot = plant.mBoard.AddProjectile(x,
            plant.mBoard.GetPosYBasedOnRow(x, plant.mRow) + 80 - .0616f * unit,
            plant.mRenderOrder + 1, row, ProjectileType.Pea);
        if (shot == null || shot.mDead) return null;
        shot.mCobTargetX = WeedMarker;
        shot.mCobTargetRow = row == plant.mRow ? 0 : 1;
        shot.mMotionType = ProjectileMotion.Straight;
        shot.mDamageRangeFlags = DamageRangeFlags.Ground | DamageRangeFlags.Dog;
        shot.mVelX = 6 * unit / 100;
        shot.mVelY = shot.mVelZ = shot.mAccZ = shot.mPosZ = 0;
        shot.mShadowY = shot.mPosY;
        Shots[shot.Pointer] = new Shot { Board = shot.mBoard.Pointer, Id = shot.DataID, Weed = true };
        ImportedPeaVisuals.EnsureWeedProjectile(shot);
        return shot;
    }
    internal const int IgnitedNut = -624013;
    internal static void Ignite(Projectile projectile, Shot shot)
    {
        int damage = projectile.mCobTargetX == IgnitedNut ? projectile.mCobTargetRow : 20;
        projectile.mCobTargetX = IgnitedNut;
        projectile.mCobTargetRow = (int)Math.Min(int.MaxValue, (long)damage * 2);
        shot.Hit.Clear();
    }
    internal static bool Hungry(Projectile projectile) => projectile.mCobTargetX is >= -31203 and <= -31201;
    internal static int Chunk(Projectile projectile) => projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
        && projectile.mCobTargetX is >= -31103 and <= -31101 ? -(int)projectile.mCobTargetX - 31100
            : projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea && Hungry(projectile)
                ? -(int)projectile.mCobTargetX - 31200 : 0;

    internal static void Spawn(Plant plant, bool knockback, int chunk = 0, bool hungry = false)
    {
        bool nut = FusionPlant.From(plant) == FusionPlant.PeaNut;
        var projectile = plant.mBoard.AddProjectile(plant.mX + 55f,
            plant.mY + plant.mHeight - (nut ? 10f : 18f),
            plant.mRenderOrder + 1, plant.mRow, ProjectileType.Pea);
        if (projectile == null || projectile.mDead) return;
        if (FusionPlant.From(plant) == FusionPlant.Chompzilla)
            projectile.mCobTargetX = -(31100 + (chunk is >= 1 and <= 3 ? chunk : knockback ? 3 : plant.mBoard.Rand(2) + 1));
        if (hungry) projectile.mCobTargetX = -(31200 + chunk);
        Track(projectile, nut, knockback, nut ? Math.Max(0, plant.mHeight - 35f) : 0);
    }

    internal static bool ChargeSolarBall(Plant plant)
    {
        var existing = plant.mBoard.m_projectiles;
        for (int i = 0; i < existing.Count; ++i)
        {
            var projectile = existing[i];
            if (projectile == null || projectile.mDead || !TryShot(projectile, out var shot)
                || !shot.SolarBall || shot.SourceId != plant.DataID || shot.ChargeTicks <= 0) continue;
            shot.Damage += Math.Max(60, (int)(600 * MathF.Exp((shot.Power - 1) * -.3f)));
            ++shot.Power;
            shot.Scale += .1f * MathF.Exp(-2 * shot.Scale);
            shot.ChargeTicks = 100;
            return true;
        }
        int col = plant.mPlantCol + 1;
        if (col >= 9 || plant.mBoard.GetPlantsOnLawn(col, plant.mRow).NormalPlant is { mDead: false }
            || FusionService.CheckNativePlacement(plant.mBoard, col, plant.mRow, SeedType.Wallnut) != PlantingReason.Ok) return false;
        var ball = plant.mBoard.AddProjectile(plant.mX + 120, plant.mY + plant.mHeight - 10,
            plant.mRenderOrder + 1, plant.mRow, ProjectileType.Pea);
        if (ball == null || ball.mDead) return false;
        Track(ball, true);
        var state = Shots[ball.Pointer];
        state.SolarBall = true;
        state.Scale = .3f; // Original BigSunNut root scale; Larger uses this value.
        state.SourceId = plant.DataID;
        state.ChargeTicks = 100;
        ball.mRotation = 0;
        ImportedPeaVisuals.RegisterSolarNutBall(ball);
        return true;
    }

    internal static void Track(Projectile projectile, bool nut, bool knockback = false, float dropHeight = 0)
    {
        Forget(projectile);
        Shots[projectile.Pointer] = new Shot { Board = projectile.mBoard.Pointer,
            Id = projectile.DataID, Nut = nut, Knockback = knockback, Chunk = Chunk(projectile) };
        projectile.mMotionType = ProjectileMotion.Straight;
        projectile.mVelX = nut ? 2.5f : 3.33f;
        projectile.mVelY = projectile.mVelZ = projectile.mAccZ = projectile.mPosZ = 0;
        if (nut)
        {
            // Replanted uses negative Z above the lane. Keep the ground
            // baseline in PosY so Update and the controller retain ownership.
            projectile.mPosZ = -dropHeight;
            projectile.mAccZ = .1f;
        }
        projectile.mShadowY = projectile.mPosY + (nut ? 10 : 12);
        projectile.mDamageRangeFlags = DamageRangeFlags.Ground;
        ImportedPeaVisuals.RegisterRollingProjectile(projectile, nut);
    }

    internal static void Move(Projectile projectile, Shot shot)
    {
        if (shot.Weed) { MoveWeed(projectile); return; }
        var board = projectile.mBoard;
        TorchweedFusions.Convert(projectile);
        ChargeTorchFusions.Convert(projectile);
        if (!FusionTime.SourceStep(projectile, typeof(RollingProjectiles))) return;
        if (shot.SolarBall && shot.ChargeTicks > 0) { --shot.ChargeTicks; return; }
        if (shot.SolarBall) shot.AnimationTime += .01f * shot.AnimationSpeed;
        float oldX = projectile.mPosX, x = oldX + projectile.mVelX;
        float slope = board.GetPosYBasedOnRow(x, projectile.mRow) - board.GetPosYBasedOnRow(oldX, projectile.mRow);
        if (x > 900 || x < -80 || (slope < -.01f && (!shot.Nut || projectile.mPosZ >= 0)))
        { projectile.Die(); return; }
        projectile.mPosX = x;
        projectile.mPosY += slope;
        if (shot.SolarBall) projectile.mShadowY += slope;
        if (shot.Nut)
        {
            projectile.mVelZ += projectile.mAccZ;
            projectile.mPosZ = Math.Min(0, projectile.mPosZ + projectile.mVelZ);
            if (projectile.mPosZ == 0)
            {
                projectile.mVelZ = projectile.mAccZ = 0;
                projectile.mVelX = 1.25f;
            }
        }
        if (!shot.SolarBall) projectile.mRotation += .12f;
        if (shot.SolarBall && shot.HitTicks-- > 0) return;
        if (shot.SolarBall) shot.HitTicks = 1;
        var zombies = board.m_zombies;
        for (int i = zombies.Count - 1; i >= 0; --i)
        {
            var zombie = zombies[i];
            if (zombie == null || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
                || zombie.mRow != projectile.mRow || !zombie.EffectedByDamage(DamageRangeFlags.Ground)
                || !shot.SolarBall && shot.Chunk == 0 && shot.Hit.Contains(zombie.DataID)) continue;
            var rect = zombie.GetZombieRect();
            float radius = 12 * (shot.SolarBall ? shot.Scale / .3f : 1);
            if (rect.xMax < oldX - radius || rect.xMin > x + radius) continue;
            if (shot.Chunk != 0)
            {
                if (!NutProjectiles.Remember(projectile, zombie)) continue;
            }
            else shot.Hit.Add(zombie.DataID);
            zombie.TakeDamage(shot.SolarBall ? shot.Damage : shot.Nut ? (projectile.mCobTargetX == IgnitedNut ? projectile.mCobTargetRow : 20) : Hungry(projectile) ? 40 : 80, DamageFlags.BypassesShield);
            // Move only a living ground target. Preserve its native state,
            // row, armour and movement controller; never move bosses/vehicles.
            if (shot.Knockback && !zombie.mDead && !zombie.IsDeadOrDying()
                && zombie.mZombieType is not (ZombieType.Boss or ZombieType.Zamboni or ZombieType.Catapult
                    or ZombieType.Gargantuar or ZombieType.RedeyeGargantuar))
            {
                zombie.mPosX = Math.Min(850, zombie.mPosX + 40);
                zombie.mX = (int)zombie.mPosX;
            }
        }
    }

    private static void MoveWeed(Projectile projectile)
    {
        if (!FusionTime.SourceStep(projectile, typeof(RollingProjectiles))) return;
        var board = projectile.mBoard;
        uint id = projectile.DataID;
        float oldX = projectile.mPosX, oldY = projectile.mPosY;
        projectile.mPosX += projectile.mVelX;
        // MoveWay_right2 follows the destination lane with velocity
        // (rowY + 1 - currentY) * Speed. Own-lane shots retain straight motion.
        float slope = board.GetPosYBasedOnRow(projectile.mPosX, projectile.mRow)
            - board.GetPosYBasedOnRow(oldX, projectile.mRow);
        projectile.mPosY += slope;
        if (projectile.mCobTargetRow == 1)
            projectile.mPosY += (board.GetPosYBasedOnRow(projectile.mPosX, projectile.mRow)
                + 80 - 80f / 1.35f - projectile.mPosY) * .06f;
        projectile.mShadowY = projectile.mPosY;
        if (projectile.mPosX > 900 || projectile.mPosX < -80 || slope < -.01f)
        {
            board.mApp.SoundSystem.PlayFoley(Il2CppReloaded.Services.FoleyType.Splat);
            projectile.Die(); return;
        }
        const float unit = 80f / 1.35f;
        // Actual 73026 BoxCollider2D, including its .3 root scale.
        const float offsetX = .2138571f * unit, offsetY = -.4948555f * unit;
        const float halfX = .6282458f * unit / 2, halfY = .3547076f * unit / 2;
        var targets = new List<(Zombie Zombie, uint Id, float X)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != projectile.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(board, zombie, projectile.mDamageRangeFlags)) continue;
            var rect = zombie.GetZombieRect();
            if (rect.xMax < oldX + offsetX - halfX || rect.xMin > projectile.mPosX + offsetX + halfX
                || rect.yMax < Math.Min(oldY, projectile.mPosY) + offsetY - halfY
                || rect.yMin > Math.Max(oldY, projectile.mPosY) + offsetY + halfY) continue;
            targets.Add((zombie, zombie.DataID, rect.xMin));
        }
        foreach (var (zombie, zombieId, _) in targets.OrderBy(target => target.X))
        {
            if (projectile.mDead || projectile.DataID != id || projectile.mBoard?.Pointer != board.Pointer) return;
            if (zombie.DataID != zombieId || !SquashFusions.Eligible(board, zombie, projectile.mDamageRangeFlags)
                || !NutProjectiles.Remember(projectile, zombie)) continue;
            bool vehicle = zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult;
            int extra = vehicle ? (int)(Math.Max(0L, zombie.mBodyMaxHealth) * .3f) : 0;
            // Source first applies 5 piercing damage, then the vehicle bonus.
            zombie.TakeDamage(5, DamageFlags.BypassesShield);
            if (projectile.DataID != id || projectile.mBoard?.Pointer != board.Pointer) return;
            PlantLibrary.Projectiles.PlaySound("fusion:Splat", board);
            if (!vehicle) continue;
            if (zombie.DataID == zombieId && zombie.mBoard?.Pointer == board.Pointer
                && !zombie.mDead && !zombie.IsDeadOrDying()) zombie.TakeDamage(extra, DamageFlags.BypassesShield);
            if (projectile.DataID == id && projectile.mBoard?.Pointer == board.Pointer && !projectile.mDead)
                projectile.Die();
            return;
        }
    }

    internal static void Forget(Projectile projectile)
    {
        Shots.Remove(projectile.Pointer);
        ImportedPeaVisuals.ForgetProjectile(projectile);
    }
    internal static void ForgetBoard(Board board)
    {
        foreach (var key in Shots.Where(e => e.Value.Board == board.Pointer).Select(e => e.Key).ToArray())
            Shots.Remove(key);
    }
    internal static void Clear() => Shots.Clear();
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class PeaNutFirePatch
{
    private static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.PeaNut) return true;
        if (ChomperFusions.Active(__instance)) RollingProjectiles.Spawn(__instance, false);
        return false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class RollingMotionPatch
{
    private static bool Prefix(Projectile __instance)
    {
        if (!RollingProjectiles.TryShot(__instance, out var shot)) return true;
        if (!__instance.mDead)
        {
            if (shot.Weed) ImportedPeaVisuals.EnsureWeedProjectile(__instance);
            else ImportedPeaVisuals.EnsureRollingProjectile(__instance, shot.Nut);
        }
        if (!__instance.mDead && !__instance.mBoard.mPaused) RollingProjectiles.Move(__instance, shot);
        return false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class SolarBallCollisionPatch
{
    private static bool Prefix(Projectile __instance) =>
        !RollingProjectiles.TryShot(__instance, out _);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class SolarBallTorchwoodPatch
{
    private static bool Prefix(Projectile __instance) =>
        !RollingProjectiles.TryShot(__instance, out _);
}

[HarmonyPatch]
internal static class RollingCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        new[] { AccessTools.Method(typeof(Projectile), "Die"), AccessTools.Method(typeof(Projectile), "Reset") };
    private static void Prefix(Projectile __instance) => RollingProjectiles.Forget(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class RollingBoardCleanupPatch
{
    private static void Prefix(Board __instance) => RollingProjectiles.ForgetBoard(__instance);
}

