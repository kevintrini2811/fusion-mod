using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using UnityEngine;

namespace FusionMod;

internal static class AirRaidFusions
{
    private const int CactusInterval = 150, ShotGap = 10;
    private const int CherryInterval = 1500, OffsetPrecision = 256;
    private const float CherrySearchRadius = 3 * BloverFusions.Unit;
    private const float CherryContactRadius = .5f * BloverFusions.Unit;
    internal const int CactusShootTicks = 50;

    internal static bool Owns(FusionPlant identity) =>
        identity == FusionPlant.CactusDrone || identity == FusionPlant.CherryDrone;

    internal static void Initialize(Plant plant)
    {
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mDoSpecialCountdown = plant.mLaunchCounter = plant.mShootingCounter = plant.mStartRow = 0;
        ResetFlight(plant);
    }

    internal static void ResetFlight(Plant plant)
    {
        plant.mTargetZombieID = 0;
        plant.mTargetX = plant.mTargetY = 0;
    }

    internal static void Fertilize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.CherryDrone && plant.mLaunchCounter > 50)
            plant.mLaunchCounter = 50;
    }

    internal static Vector2 FlightOffset(Plant plant) =>
        new(plant.mTargetX / (float)OffsetPrecision, plant.mTargetY / (float)OffsetPrecision);

    internal static void Tick(Plant plant, FusionPlant identity)
    {
        if (!CactusFusions.Active(plant) || !FusionTime.SourceStep(plant, typeof(AirRaidFusions))) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (identity == FusionPlant.CactusDrone) TickCactus(plant);
        else TickCherry(plant);
    }

    private static void TickCactus(Plant plant)
    {
        if (plant.mStartRow == 0)
        {
            plant.mStartRow = 1;
            BloverFusions.Blow(plant);
        }
        if (plant.mShootingCounter > 0) --plant.mShootingCounter;
        if (plant.mLaunchCounter == CactusInterval - ShotGap)
        {
            FireCactus(plant);
            return;
        }
        if (plant.mLaunchCounter != 0) return;

        var board = plant.mBoard;
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mZombieType != ZombieType.Balloon || !zombie.IsFlying()
                || zombie.mBlowingAway || !CactusFusions.InRange(plant, zombie, true)) continue;
            plant.mLaunchCounter = CactusInterval;
            plant.mShootingCounter = CactusShootTicks;
            FireCactus(plant);
            return;
        }
    }

    private static void FireCactus(Plant plant)
    {
        if (CactusProjectiles.Spawn(plant) != null)
            plant.mApp.SoundSystem.PlayFoley(FoleyType.Throw);
    }

    private static bool Eligible(Plant plant, Zombie zombie) => Math.Abs(zombie.mRow - plant.mRow) <= 1
        && !zombie.IsFlying() && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground);

    private static Zombie? Acquire(Plant plant, Vector2 origin)
    {
        Zombie? target = null;
        float closest = CherrySearchRadius * CherrySearchRadius;
        var zombies = plant.mBoard.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || !Eligible(plant, zombie)) continue;
            var rect = zombie.GetZombieRect();
            float dx = origin.x - Math.Clamp(origin.x, rect.xMin, rect.xMax);
            float dy = origin.y - Math.Clamp(origin.y, rect.yMin, rect.yMax);
            float distance = dx * dx + dy * dy;
            if (distance > closest || (target != null && distance == closest)) continue;
            closest = distance;
            target = zombie;
        }
        return target;
    }

    private static void TickCherry(Plant plant)
    {
        var board = plant.mBoard;
        var origin = new Vector2(plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f - plant.mHeight * .52f);
        var target = plant.mTargetZombieID == 0 ? null : board.ZombieTryToGet(plant.mTargetZombieID);
        if (target != null && !Eligible(plant, target)) target = null;
        if (target == null && plant.mLaunchCounter == 0) target = Acquire(plant, origin);
        plant.mTargetZombieID = (ZombieID)(target?.DataID ?? 0);

        var offset = FlightOffset(plant);
        var destination = target == null ? origin : target.GetZombieRect().center;
        // The source head uses Lerp at five times the frame delta. Run the
        // equivalent movement in fixed source steps and keep the air slot anchored.
        offset += (destination - origin - offset) * .05f;
        plant.mTargetX = (int)MathF.Round(offset.x * OffsetPrecision);
        plant.mTargetY = (int)MathF.Round(offset.y * OffsetPrecision);
        if ((destination - origin - offset).sqrMagnitude >= CherryContactRadius * CherryContactRadius) return;
        if (target == null)
        {
            ResetFlight(plant);
            return;
        }

        // Commit the cooldown and clear the old generation before damage can
        // run a death callback or recycle the target object.
        plant.mTargetZombieID = 0;
        plant.mLaunchCounter = CherryInterval;
        var impact = origin + offset;
        CherryBombPlants.Explode(new(board, FusionPlant.CherryDrone, plant.DataID, plant.mPlantCol,
            target.mRow, impact.x, impact.y, plant.mRenderOrder), sourceRadius: 80, damage: 300);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class AirRaidInitializePatch
{
    private static void Postfix(Plant __instance)
    {
        if (AirRaidFusions.Owns(FusionPlant.From(__instance))) AirRaidFusions.Initialize(__instance);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class AirRaidAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (!AirRaidFusions.Owns(identity)) return true;
        AirRaidFusions.Tick(__instance, identity);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class AirRaidSpecialPatch
{
    private static bool Prefix(Plant __instance) => !AirRaidFusions.Owns(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class AirRaidShootingPatch
{
    private static bool Prefix(Plant __instance) => !AirRaidFusions.Owns(FusionPlant.From(__instance));
}
