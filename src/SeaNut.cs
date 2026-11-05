using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The plant remains the saved rolling object. No transient projectile or
// animation callback owns movement, damage, growth or health.
internal static class SeaNut
{
    internal static void Initialize(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.SeaNut) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 2000;
        plant.mLaunchCounter = 1000;
        plant.mStartRow = plant.mTargetX = plant.mTargetY = 0;
        plant.mShootingCounter = 1000000;
        plant.mIsAsleep = false;
    }
    internal static float Scale(Plant plant) => Math.Max(1, plant.mShootingCounter / 1000000f);
    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant)) return;
        float target = Math.Max(1, (1 + Math.Clamp(plant.mStartRow, 0, 3) * .3f)
            * plant.mPlantHealth / Math.Max(1f, plant.mPlantMaxHealth));
        plant.mShootingCounter = (int)MathF.Round((Scale(plant) + (target - Scale(plant)) * .01f) * 1000000);
        if (plant.mStartRow < 3)
        {
            if (--plant.mLaunchCounter > 0) return;
            plant.mLaunchCounter = 1000;
            if (++plant.mStartRow < 3)
            {
                plant.mPlantHealth += 2000; plant.mPlantMaxHealth += 2000;
                plant.UpdateReanimColor();
            }
            return;
        }
        // The round clip's root travels 13.21 world units in one second with
        // ApplyRootMotion enabled. Status 25 means rolling, not movement speed.
        // Keep fractional pixels in a saved
        // accumulator; mX/mY remain the native controller's complete anchor.
        float speed = 13.21f * (80f / 1.35f);
        plant.mTargetX += (int)MathF.Round(speed * 10);
        int move = plant.mTargetX / 1000;
        plant.mTargetX %= 1000;
        plant.mX += move;
        plant.UpdateReanim();
        if (plant.mX > 900) { plant.Die(); return; }
        plant.mTargetY = (plant.mTargetY + 1) % 100;
        if (plant.mTargetY % 2 != 1) return;
        var targets = new List<(Zombie Zombie, uint Id)>();
        float x = plant.mX + plant.mWidth / 2f;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && zombie.mRow == plant.mRow
                && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
                && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Dog))
            {
                var rect = zombie.GetZombieRect();
                float radius = 30 * Scale(plant);
                if (rect.xMax >= x - radius - move && rect.xMin <= x + radius)
                    targets.Add((zombie, zombie.DataID));
            }
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Dog))
                zombie.TakeDamage(600, DamageFlags.BypassesShield);
    }
    internal static void Fertilize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.SeaNut && plant.mStartRow < 3 && plant.mLaunchCounter > 50)
            plant.mLaunchCounter = 50;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SeaNutInitializePatch
{ private static void Postfix(Plant __instance) => SeaNut.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SeaNutAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.SeaNut) return true;
        SeaNut.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class GroundFusionShootingPatch
{
    private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.SeaNut
        && !FusionPlant.IsGroundWeed(FusionPlant.From(__instance));
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class GroundFusionFirePatch
{
    private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.SeaNut
        && !FusionPlant.IsGroundWeed(FusionPlant.From(__instance));
}
