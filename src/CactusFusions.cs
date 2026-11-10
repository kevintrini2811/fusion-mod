using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using LawnGlove;

namespace FusionMod;

// The native shooter routines are replaced for these identities only. Saved
// fields own mode, transition and attack phase; the renderer never emits hits.
internal static class CactusFusions
{
    internal const float Unit = 80f / 1.35f;
    internal static bool Owns(FusionPlant identity) => FusionPlant.IsCactusFusion(identity) || identity == FusionPlant.Starthorn || identity == FusionPlant.Thornweed;
    internal static bool Active(Plant plant) => ChomperFusions.Active(plant) && !plant.mIsAsleep
        && LawnGloveMod.GetCarriedPlant(plant.mBoard)?.Pointer != plant.Pointer;
    internal static int TransitionTicks(Plant plant) => FusionPlant.From(plant) == FusionPlant.SaguaroShroom ? 100
        : (FusionPlant.From(plant) == FusionPlant.MagnetCactus || FusionPlant.From(plant) == FusionPlant.DoomCactus || FusionPlant.From(plant) == FusionPlant.LumosCactus) && plant.mTargetX == 0 ? 69 : 67;
    internal static int EventTick(Plant plant) => FusionPlant.From(plant) == FusionPlant.SaguaroShroom
        ? plant.mTargetX == 0 ? 35 : 34 : plant.mTargetX == 0 ? 32 : 27;
    internal static int AttackTicks(Plant plant) => FusionPlant.From(plant) == FusionPlant.SaguaroShroom
        ? plant.mTargetX == 0 ? 100 : 84 : FusionPlant.From(plant) == FusionPlant.LumosCactus && plant.mTargetX != 0 ? 72 : 52;

    internal static void Initialize(Plant plant)
    {
        if (!Owns(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchRate = 150;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = 0;
        plant.mTargetX = plant.mTargetY = plant.mDoSpecialCountdown = 0;
        if (FusionPlant.From(plant) == FusionPlant.DoomCactus) plant.mShootingCounter = 21;
    }

    internal static bool InRange(Plant plant, Zombie zombie, bool airOnly = false)
    {
        if (zombie.mRow != plant.mRow || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || !SquashFusions.Eligible(plant.mBoard, zombie, airOnly ? DamageRangeFlags.Flying
                : DamageRangeFlags.Ground | DamageRangeFlags.Flying)) return false;
        float x = plant.mX + plant.mWidth / 2f;
        float target = zombie.GetZombieRect().center.x;
        return target >= x && (FusionPlant.From(plant) != FusionPlant.SaguaroShroom || target <= x + 6 * Unit);
    }

    internal static Zombie? Target(Plant plant, bool airOnly)
    {
        Zombie? target = null;
        float nearest = float.MaxValue;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || !InRange(plant, zombie, airOnly)) continue;
            float x = zombie.GetZombieRect().center.x;
            if (x < nearest) { target = zombie; nearest = x; }
        }
        return target;
    }

    internal static void Tick(Plant plant)
    {
        if (!Active(plant)) return;
        if (!FusionTime.SourceStep(plant, typeof(CactusFusions))) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.Starthorn) { TickStar(plant); return; }
        if (identity == FusionPlant.Thornweed) { TickThornweed(plant); return; }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (identity == FusionPlant.DoomCactus && plant.mDoSpecialCountdown > 0)
        {
            int burstElapsed = 184 - plant.mDoSpecialCountdown;
            // Ninety thorns spread across the original Start/StopShooting interval.
            if (burstElapsed >= 42 && burstElapsed < 150)
            {
                int before = (burstElapsed - 42) * 90 / 108, after = (burstElapsed - 41) * 90 / 108;
                for (int i = before; i < after; ++i)
                    CactusProjectiles.Spawn(plant, (plant.mBoard.Rand(9001) - 4500) / 100f, true);
            }
            --plant.mDoSpecialCountdown;
            return;
        }
        if (plant.mTargetY > 0) { --plant.mTargetY; return; }
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0) return;
            var target = Target(plant, identity != FusionPlant.SaguaroShroom || plant.mTargetX != 0);
            if (target == null && identity != FusionPlant.SaguaroShroom) target = Target(plant, false);
            if (target == null) return;
            int mode = identity == FusionPlant.SaguaroShroom ? plant.mTargetX : target.IsFlying() ? 1 : 0;
            if (plant.mTargetX != mode)
            {
                plant.mTargetX = mode;
                plant.mTargetY = TransitionTicks(plant);
                return;
            }
            plant.mStartRow = AttackTicks(plant);
            plant.mLaunchCounter = 150;
        }
        int elapsed = AttackTicks(plant) - plant.mStartRow;
        if (elapsed == EventTick(plant) || identity == FusionPlant.LumosCactus && plant.mTargetX != 0 && elapsed == 50) Fire(plant);
        if (plant.mStartRow > 0) --plant.mStartRow;
    }

    internal static void Fire(Plant plant)
    {
        if (!Active(plant)) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.DoomCactus)
        {
            int remaining = Math.Clamp(plant.mShootingCounter, 1, 21);
            if (plant.mBoard.Rand(remaining) == 0)
            {
                plant.mShootingCounter = 21;
                plant.mDoSpecialCountdown = 184;
                plant.mStartRow = 0;
            }
            else plant.mShootingCounter = remaining - 1;
            // UltimateCactus.Shoot1 still emits the triggering ordinary thorn.
        }
        if (identity == FusionPlant.LumosCactus && PumpkinFusions.LightLevel(plant) >= 2)
        { Laser(plant); return; }
        if (identity != FusionPlant.SaguaroShroom)
        { CactusProjectiles.Spawn(plant); return; }
        bool high = plant.mTargetX != 0;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie != null && InRange(plant, zombie, high)) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && InRange(plant, zombie, high))
                zombie.TakeDamage(high ? 160 : 40, DamageFlags.BypassesShield);
        ImportedPeaVisuals.FumeSmoke(plant, identity, profileKey: "31973");
        plant.mBoard.mApp.SoundSystem.PlayFoley(FoleyType.Fume);
    }


    internal static bool ThornweedTarget(Plant plant, Zombie zombie)
    {
        if (zombie.mRow != plant.mRow || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Flying)) return false;
        var attack = plant.GetPlantAttackRect(PlantWeapon.Primary);
        var target = zombie.GetZombieRect();
        return target.xMax >= attack.xMin && target.xMin <= attack.xMax;
    }
    private static void TickThornweed(Plant plant)
    {
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0) return;
            bool found = false;
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
                if (plant.mBoard.m_zombies[i] is { } zombie && ThornweedTarget(plant, zombie)) { found = true; break; }
            if (!found) return;
            plant.mStartRow = 67;
            plant.mLaunchCounter = 100;
        }
        if (67 - plant.mStartRow == 34) ThornweedAttack(plant);
        --plant.mStartRow;
    }
    internal static void ThornweedAttack(Plant plant)
    {
        if (!Active(plant)) return;
        var hits = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && ThornweedTarget(plant, zombie))
                hits.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in hits)
            if (zombie.DataID == id && ThornweedTarget(plant, zombie))
                zombie.TakeDamage(20, DamageFlags.BypassesShield | DamageFlags.Spike);
        foreach (float angle in new[] { 60f, 90f, 120f }) CactusProjectiles.Spawn(plant, angle, true);
    }

    internal static Zombie? StarTarget(Plant plant)
    {
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie != null && CactusProjectiles.AirTarget(plant.mBoard, zombie)) return zombie;
        }
        return null;
    }
    private static void TickStar(Plant plant)
    {
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || StarTarget(plant) == null) return;
            plant.mStartRow = 67;
            plant.mLaunchCounter = 150;
        }
        if (67 - plant.mStartRow == 42)
            foreach (float angle in StarProjectiles.Angles) CactusProjectiles.Star(plant, angle);
        --plant.mStartRow;
    }
    internal static void Laser(Plant plant)
    {
        bool air = plant.mTargetX != 0;
        int damage = 30 * (PumpkinFusions.LightLevel(plant) >= 3 ? 2 : 1) * (air ? 3 : 1);
        var hits = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie != null && InRange(plant, zombie, air)
                && (air || !zombie.IsFlying())) hits.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in hits)
            if (zombie.DataID == id && InRange(plant, zombie, air))
            {
                if (!air) PlanternFusions.LaserHit(plant, zombie);
                if (!zombie.mDead && !zombie.IsDeadOrDying()) zombie.TakeDamage(damage, DamageFlags.BypassesShield);
            }
    }

    internal static bool Click(Plant plant, int clicks, int player)
    {
        if (FusionPlant.From(plant) != FusionPlant.SaguaroShroom) return false;
        if (clicks >= 0 && player == 0 && Active(plant)
            && plant.mBoard.mCursorObject.CursorType == CursorType.Normal
            && LawnGloveMod.GetCarriedPlant(plant.mBoard) == null)
        {
            plant.mTargetX = plant.mTargetX == 0 ? 1 : 0;
            plant.mTargetY = TransitionTicks(plant);
            plant.mStartRow = 0;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class CactusInitializePatch
{ private static void Postfix(Plant __instance) => CactusFusions.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class CactusAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!CactusFusions.Owns(FusionPlant.From(__instance))) return true;
        CactusFusions.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class CactusShootingPatch
{ private static bool Prefix(Plant __instance) => !CactusFusions.Owns(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class CactusFirePatch
{ private static bool Prefix(Plant __instance) => !CactusFusions.Owns(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.MouseDown))]
internal static class CactusClickPatch
{
    private static bool Prefix(Plant __instance, int theClickCount, int playerIndex) =>
        !CactusFusions.Click(__instance, theClickCount, playerIndex);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class CactusNativeSpecialPatch
{
    private static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        return identity != FusionPlant.Starthorn && identity != FusionPlant.Thornweed
            && identity != FusionPlant.DoomCactus && identity != FusionPlant.LumosCactus;
    }
}
