using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

internal static class DoomPult
{
    private const int ShotMarker = -520440;
    private const int CraterMarker = -523408;
    private const int BaseDamage = 1800;

    internal static bool IsShot(Projectile shot) => shot.mProjectileType == ProjectileType.Melon
        && shot.mCobTargetX == ShotMarker;

    internal static void Shoot(Plant plant, Zombie? target)
    {
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth * .5f,
            plant.mY + plant.mHeight * .5f - 10, plant.mRenderOrder + 1, plant.mRow, ProjectileType.Melon);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = ShotMarker;
        shot.mCobTargetRow = PotBoosts.HasSupport(plant, FusionPlant.MelonPot) ? -BaseDamage : BaseDamage;
        LobberFlight.Launch(shot, target, PuffLobbers.FlightTicks);
        ImportedPeaVisuals.EnsureDoomMelonProjectile(shot);
    }

    internal static void Fly(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board == null || board.mPaused || !ScaredContent.Playing(board)
            || !FusionTime.SourceStep(shot, typeof(DoomPult))) return;
        // The shared solver reaches the target at the native Melon impact
        // height. Confirm land on that step without the native contact hit.
        if (LobberFlight.AdvanceMelon(shot)) Land(shot);
    }

    private static void Land(Projectile shot)
    {
        var board = shot.mBoard;
        float x = shot.mPosX, y = shot.mPosY;
        int row = shot.mRow, order = shot.mRenderOrder, col = board.PixelToGridX(x, y);
        bool melonPot = shot.mCobTargetRow < 0;
        int damage = FumeProjectiles.Damage(shot, BaseDamage);
        uint id = shot.DataID;
        shot.Die();
        if (!shot.mDead || shot.DataID != id || shot.mBoard?.Pointer != board.Pointer
            || !ScaredContent.Playing(board) || col is < 0 or >= 9 || row < 0 || row >= board.GetNumRows()) return;

        bool fallback = board.IsPoolSquare(col, row) || HasCrater(board, col, row);
        if (fallback)
        {
            DoomEffects.Damage(board, row, x, y, order, damage: damage / 18,
                radius: (melonPot ? 2f : 1.5f) * ScaredyFusions.SourceUnit, showEffects: false);
            if (!ScaredContent.Playing(board)) return;
            board.mApp.AddTodParticle(x, y, order, ParticleEffect.Powie);
            board.mApp.SoundSystem.PlayFoley(FoleyType.Chime);
            return;
        }
        DoomEffects.Damage(board, row, x, y, order, damage: damage);
        if (ScaredContent.Playing(board) && DoomEffects.LeaveCrater(board, col, row,
                () => ScaredContent.Playing(board)) is { } crater)
            crater.mSunCount = CraterMarker;
    }

    private static bool HasCrater(Board board, int col, int row)
    {
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { mDead: false, mGridItemType: GridItemType.Crater } crater
                && crater.mGridX == col && crater.mGridY == row) return true;
        return false;
    }

    internal static void TickCrater(GridItem crater)
    {
        if (crater.mDead || crater.mGridItemType != GridItemType.Crater || crater.mSunCount != CraterMarker) return;
        var board = crater.mBoard;
        if (board == null || board.mPaused || !ScaredContent.Playing(board)
            || !FusionTime.SourceStep(board, typeof(DoomPult), crater.Pointer, crater.DataID)) return;
        float x = board.GridToPixelX(crater.mGridX, crater.mGridY) + 40;
        float y = board.GridToPixelY(crater.mGridX, crater.mGridY) + 55;
        const float radius = ScaredyFusions.SourceUnit * .5f;
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != crater.mGridY
                || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= radius * radius) EmberEffects.Apply(zombie, explode: false);
        }
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateLobMotion))]
internal static class DoomPultFlightPatch
{
    private static bool Prefix(Projectile __instance, bool __runOriginal)
    {
        if (!__runOriginal || !DoomPult.IsShot(__instance)) return true;
        DoomPult.Fly(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class DoomPultProjectileVisualPatch
{
    private static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && DoomPult.IsShot(__instance)) ImportedPeaVisuals.EnsureDoomMelonProjectile(__instance);
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class DoomPultCraterPatch
{
    private static void Postfix(GridItem __instance) => DoomPult.TickCrater(__instance);
}
