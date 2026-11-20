using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

internal static class OrcaShroom
{
    private const int Marker = -520303, BaseDamage = 600, FlightTicks = 150;

    internal static bool IsShot(Projectile shot) => shot.mProjectileType == ProjectileType.Melon
        && shot.mCobTargetX == Marker;

    internal static void Shoot(Plant plant)
    {
        var board = plant.mBoard;
        var target = SeaFusions.Target(plant, false);
        float x = plant.mX + plant.mWidth * .5f, y = plant.mY + plant.mHeight * .5f;
        var shot = board.AddProjectile(x, y - 10, plant.mRenderOrder + 1, plant.mRow, ProjectileType.Melon);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = Marker;
        shot.mCobTargetRow = BaseDamage;
        if (target == null) LobberFlight.LaunchAt(shot, x + 8 * SeaFusions.Unit, y, FlightTicks);
        else LobberFlight.Launch(shot, target, FlightTicks);
        ImportedPeaVisuals.EnsureDoomProjectile(shot);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Throw);
    }

    internal static void Fly(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board == null || board.mPaused || !ScaredContent.Playing(board)
            || !FusionTime.SourceStep(shot, typeof(OrcaShroom)) || !LobberFlight.AdvanceMelon(shot)) return;
        float x = shot.mPosX, y = shot.mPosY;
        int row = shot.mRow, order = shot.mRenderOrder;
        int damage = FumeProjectiles.Damage(shot, BaseDamage);
        uint id = shot.DataID;
        shot.Die();
        if (!shot.mDead || shot.DataID != id || shot.mBoard?.Pointer != board.Pointer
            || !ScaredContent.Playing(board)) return;
        DoomEffects.Damage(board, row, x, y, order, damage: damage, radius: 2 * SeaFusions.Unit, showEffects: false);
        if (!ScaredContent.Playing(board)) return;
        board.mApp.AddTodParticle(x, y, order, ParticleEffect.Powie);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Chime);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateLobMotion))]
internal static class OrcaShotFlightPatch
{
    private static bool Prefix(Projectile __instance, bool __runOriginal)
    {
        if (!__runOriginal || !OrcaShroom.IsShot(__instance)) return true;
        OrcaShroom.Fly(__instance);
        return false;
    }
}
