using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Reuse the saved projectile modifier record. SeedType identifies relay state;
// render order holds the full umbrella DataID; PosX counts incoming flight ticks.
internal static class UmbrellaKale
{
    internal static bool Tick(Projectile shot)
    {
        if (shot.mDead || shot.mBoard.mPaused || shot.mMotionType != ProjectileMotion.Lobbed
            || shot.mProjectileType is not (ProjectileType.Cabbage or ProjectileType.Melon or ProjectileType.Wintermelon
                or ProjectileType.Kernel or ProjectileType.Butter)) return true;
        var record = FumeProjectiles.Record(shot, false);
        if (record == null || record.mSeedType == SeedType.None)
        {
            if (FusionProjectileTime.Owns(shot) ? FusionProjectileTime.AgeSeconds(shot) > .02f : shot.mProjectileAge > 2) return true;
            Plant? relay = null;
            for (int i = 0; i < shot.mBoard.m_plants.Count; ++i)
            {
                var plant = shot.mBoard.m_plants[i];
                if (plant == null || FusionPlant.From(plant) != FusionPlant.UmbrellaKale
                    || !ChomperFusions.Active(plant) || plant.NotOnGround() || plant.mRow != shot.mRow
                    || plant.mX + plant.mWidth / 2f <= shot.mPosX
                    || relay != null && relay.mX <= plant.mX) continue;
                relay = plant;
            }
            if (relay == null) return true;
            record = FumeProjectiles.Record(shot, true);
            if (record == null) return true;
            record.mSeedType = SeedType.Cabbagepult;
            record.mRenderOrder = unchecked((int)relay.DataID);
            record.mPosX = 80;
            LobberFlight.LaunchAt(shot, relay.mX + relay.mWidth / 2f, relay.mY + relay.mHeight / 2f, 80);
        }
        if (record.mSeedType is not (SeedType.Cabbagepult or SeedType.Umbrella)) return true;
        Plant? umbrella = null;
        for (int i = 0; i < shot.mBoard.m_plants.Count; ++i)
            if (shot.mBoard.m_plants[i] is { } candidate && candidate.DataID == unchecked((uint)record.mRenderOrder)
                && FusionPlant.From(candidate) == FusionPlant.UmbrellaKale && ChomperFusions.Active(candidate)
                && !candidate.NotOnGround()) { umbrella = candidate; break; }
        if (umbrella == null)
        {
            record.mSeedType = SeedType.Marigold;
            LobberFlight.Launch(shot, null, 80);
            return true;
        }
        if (record.mSeedType == SeedType.Cabbagepult
            && (!FusionTime.SourceStep(shot, typeof(UmbrellaKale)) || --record.mPosX > 0)) return true;
        shot.mPosX = umbrella.mX + umbrella.mWidth / 2f;
        shot.mPosY = umbrella.mY + umbrella.mHeight / 2f;
        shot.mPosZ = -60;
        shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
        shot.mVelX = shot.mVelY = shot.mVelZ = shot.mAccZ = 0;
        record.mSeedType = SeedType.Umbrella;
        var target = UmbrellaVariants.Target(umbrella);
        if (target == null) return false;
        // Commit once before native collision or damage callbacks can recycle it.
        record.mSeedType = SeedType.Cobcannon;
        LobberFlight.Launch(shot, target, 80);
        umbrella.mShootingCounter = UmbrellaShell.AnimationTicks;
        return true;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateLobMotion))]
internal static class UmbrellaKaleFlightPatch
{
    [HarmonyPriority(Priority.First + 1000)]
    private static bool Prefix(Projectile __instance) => UmbrellaKale.Tick(__instance);
}
