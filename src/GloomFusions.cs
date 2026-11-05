using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// The three source controllers share four penetrating pulses. Their different
// attack speeds are significant: the actual clip events own the hit schedule.
// Native Gloom-shroom owns terrain, coffee, blocking and serialized identity.
internal static class GloomFusions
{
    internal static int Duration(FusionPlant identity) => identity == FusionPlant.GloomShroom ? 145 : identity == FusionPlant.FrostGloomShroom ? 156 : 154;
    internal static float Speed(FusionPlant identity) => identity == FusionPlant.GloomShroom ? 1.4f : 1.3f;
    internal static string Attack(FusionPlant identity) => identity == FusionPlant.GloomShroom ? "attack"
        : identity == FusionPlant.FrostGloomShroom ? "attack_ice" : "attack_fire";

    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsGloom(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchRate = 190;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = 0;
    }

    internal static bool InRange(Plant plant, Zombie zombie)
    {
        if (Math.Abs(zombie.mRow - plant.mRow) > 1
            || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || !SquashFusions.Eligible(plant.mBoard, zombie, plant.GetDamageRangeFlags(PlantWeapon.Primary))) return false;
        var rect = zombie.GetZombieRect();
        float x = plant.mX + plant.mWidth / 2f;
        float y = plant.mY + plant.mHeight / 2f;
        float dx = x - Math.Clamp(x, rect.xMin, rect.xMax);
        float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
        // GloomShroom.Range = 2 source units; columns are 1.35 units apart.
        const float radius = 2f / 1.35f * 80;
        return dx * dx + dy * dy <= radius * radius;
    }

    internal static void Tick(Plant plant)
    {
        if (!FumeFusions.Active(plant)) return;
        var identity = FusionPlant.From(plant);
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0) return;
            bool target = false;
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
                if (plant.mBoard.m_zombies[i] is { } zombie && InRange(plant, zombie)) { target = true; break; }
            if (!target) return;
            plant.mStartRow = Duration(identity);
            plant.mLaunchCounter = 190;
        }
        int elapsed = Duration(identity) - plant.mStartRow;
        bool pulse = identity == FusionPlant.GloomShroom ? elapsed is 43 or 62 or 81 or 100
            : elapsed is 47 or 67 or 88 or 108;
        if (pulse) Hit(plant, identity);
        --plant.mStartRow;
    }

    internal static void Hit(Plant plant, FusionPlant identity)
    {
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && InRange(plant, zombie)) targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (!FumeFusions.Active(plant) || zombie.DataID != id || !InRange(plant, zombie)) continue;
            bool fire = identity == FusionPlant.FlameGloomShroom;
            if (identity == FusionPlant.FrostGloomShroom) zombie.ApplyChill(false);
            if (fire) zombie.RemoveColdEffects();
            zombie.TakeDamage(identity == FusionPlant.GloomShroom ? 20 : 40,
                DamageFlags.BypassesShield | (fire ? DamageFlags.DoesntLeaveBody : (DamageFlags)0));
            if (zombie.DataID != id || !InRange(plant, zombie)) continue;
            if (identity == FusionPlant.FrostGloomShroom) SnowPea.Hit(zombie, 5);
            if (fire) Overheat.Apply(zombie);
        }
        ImportedPeaVisuals.FumeSmoke(plant, identity);
        plant.mBoard.mApp.SoundSystem.PlayFoley(FoleyType.Fume);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class GloomInitializePatch
{
    internal static void Postfix(Plant __instance) => GloomFusions.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class GloomTickPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsGloom(FusionPlant.From(__instance))) return true;
        GloomFusions.Tick(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class GloomNativeFirePatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsGloom(FusionPlant.From(__instance));
}
