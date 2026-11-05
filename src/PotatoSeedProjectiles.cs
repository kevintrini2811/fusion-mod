using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// PotatoSeed's coroutine flies for 1.5 seconds, then creates a native mine
// with AttributeCountdown .5. Native projectile fields save the entire flight.
internal static class PotatoSeedProjectiles
{
    private const float Marker = -526002;
    internal const int FlightTicks = 150;
    internal static bool Contains(Projectile shot) => shot.mProjectileType == ProjectileType.Pea
        && shot.mCobTargetX == Marker;

    internal static void Scatter(CherryBombPlants.Death death)
    {
        var board = death.Board;
        var cells = new List<(int Col, int Row)>();
        for (int col = 0; col < 9; ++col)
        for (int row = 0; row < board.GetNumRows(); ++row)
            if (DoomFusions.Empty(board, col, row, (FusionPlant)SeedType.Potatomine)) cells.Add((col, row));
        for (int i = 0; i < 5 && cells.Count > 0; ++i)
        {
            int index = board.Rand(cells.Count);
            var (col, row) = cells[index]; cells.RemoveAt(index);
            var shot = board.AddProjectile(death.X, death.Y, death.Order + 1, row, ProjectileType.Pea);
            if (shot == null || shot.mDead) continue;
            shot.mCobTargetX = Marker;
            shot.mCobTargetRow = col;
            shot.mMotionType = ProjectileMotion.Straight;
            shot.mAccZ = FlightTicks;
            shot.mPosZ = 0;
            shot.mVelZ = .08f;
            shot.mVelX = (board.GridToPixelX(col, row) + 40 - death.X) / FlightTicks;
            shot.mVelY = (board.GridToPixelY(col, row) + 40 - death.Y) / FlightTicks
                - shot.mVelZ * (FlightTicks - 1) / 2;
            shot.mDamageRangeFlags = 0;
            ImportedPeaVisuals.EnsurePotatoSeedProjectile(shot);
        }
    }

    internal static void Move(Projectile shot)
    {
        if (shot.mDead || shot.mBoard.mPaused) return;
        ImportedPeaVisuals.EnsurePotatoSeedProjectile(shot);
        shot.mPosX += shot.mVelX;
        shot.mPosY += shot.mVelY;
        shot.mVelY += shot.mVelZ;
        shot.mShadowY = shot.mPosY;
        if (--shot.mAccZ > 0) return;
        int col = shot.mCobTargetRow, row = shot.mRow;
        var board = shot.mBoard;
        shot.Die(); // No duplicate landing, including an allocation callback.
        var mine = DoomFusions.Spawn(board, col, row, (FusionPlant)SeedType.Potatomine);
        if (mine == null) return;
        // The source explicitly sets .5 s, rather than a fresh 15-second fuse.
        mine.mState = PlantState.Notready;
        mine.mStateCountdown = 50;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class PotatoSeedMotionPatch
{
    private static bool Prefix(Projectile __instance)
    {
        if (!PotatoSeedProjectiles.Contains(__instance)) return true;
        PotatoSeedProjectiles.Move(__instance); return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class PotatoSeedCollisionPatch
{ private static bool Prefix(Projectile __instance) => !PotatoSeedProjectiles.Contains(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class PotatoSeedTorchwoodPatch
{ private static bool Prefix(Projectile __instance) => !PotatoSeedProjectiles.Contains(__instance); }
