using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Sunflower owns terrain and saved identity. Production consumes friendly
// projectiles, never converts them into damaging fireballs.
internal static class Torchflower
{
    internal static void Initialize(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.Torchflower) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        // Retain the native initial production delay; subsequent cycles are 25 s.
        plant.mShootingCounter = 0;
    }

    internal static bool CanMelt(Plant plant, Projectile projectile)
    {
        if (projectile.mDead || projectile.mBoard?.Pointer != plant.mBoard.Pointer
            || projectile.mProjectileType is ProjectileType.ZombiePea or ProjectileType.Basketball) return false;
        float dx = projectile.mPosX - (plant.mX + plant.mWidth / 2f);
        float dy = projectile.mPosY + projectile.mPosZ - (plant.mY + plant.mHeight / 2f);
        // Actual TorchSunflower.ProduceSun queries radius 2, despite almanac 1.5.
        return dx * dx + dy * dy <= 160 * 160;
    }

    internal static void Produce(Plant plant)
    {
        var board = plant.mBoard;
        board.AddCoin(plant.mX, plant.mY, CoinType.Sun, CoinMotion.FromPlant);
        var targets = new List<(Projectile Projectile, uint Id)>();
        var projectiles = board.m_projectiles;
        for (int i = 0; i < projectiles.Count; ++i)
            if (projectiles[i] is { } projectile && CanMelt(plant, projectile))
                targets.Add((projectile, projectile.DataID));
        foreach (var (projectile, id) in targets)
        {
            if (projectile.DataID != id || !CanMelt(plant, projectile)) continue;
            // Reuse physical five-sun pickups and the existing collection hooks.
            SolarShooter.DropSun(board, projectile.mPosX, projectile.mPosY + projectile.mPosZ, 5);
            projectile.Die();
        }
    }

    internal static float Brightness(Plant plant) => plant.mShootingCounter <= 0 ? 1
        : 1 + 3 * (1 - Math.Abs(plant.mShootingCounter - 50) / 50f);

    internal static void Update(Plant plant)
    {
        if (!ChomperFusions.Active(plant)
            || plant.mApp.GameMode is >= GameMode.PuzzleIZombie1 and <= GameMode.PuzzleIZombieEndless) return;
        if (plant.mShootingCounter > 0 && --plant.mShootingCounter == 50) Produce(plant);
        if (--plant.mLaunchCounter > 0) return;
        // Actual ProducerUpdate schedules ProduceSun after 0.5 s and samples
        // Random.Range(0.95, 1.05) times the 25-second production rate.
        plant.mLaunchCounter = 2375 + plant.mBoard.Rand(251);
        plant.mShootingCounter = 100;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class TorchflowerInitializePatch
{
    internal static void Postfix(Plant __instance) => Torchflower.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateProductionPlant))]
internal static class TorchflowerProductionPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.Torchflower) return true;
        Torchflower.Update(__instance);
        return false;
    }
}
