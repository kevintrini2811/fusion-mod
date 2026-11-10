using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using LawnGlove;

namespace FusionMod;

// Keep native Fume-shroom placement, sleep/coffee, health, carrying and saves.
// UpdateAbilities calls UpdateShooter after its common sleep/bungee guards.
// mLaunchCounter stores cooldown; mStartRow stores the original clip phase.
// Fume-shroom has no native mStartRow writer. Neither field is a wall clock.
internal static class FumeFusions
{
    internal static int Duration(FusionPlant identity) => identity == FusionPlant.RayShroom || identity == FusionPlant.BlubberShroom ? 50 : identity == FusionPlant.StarburstShroom ? 59 : identity == FusionPlant.SootShroom ? 155 : identity == FusionPlant.FumeSpreader ? 102 : 100;
    internal static bool Active(Plant plant) => ChomperFusions.Active(plant) && !plant.mIsAsleep;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsFume(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.FootballShroom ? 3000 : identity == FusionPlant.WallopShroom ? 4000 : identity == FusionPlant.RustShroom ? 2000 : 300;
        plant.mLaunchRate = identity == FusionPlant.SootShroom ? 3000 : identity == FusionPlant.GlacierShroom ? 300 : 150;
        plant.mLaunchCounter = 0;
        plant.mStartRow = 0;
        if (identity == FusionPlant.BelchShroom) plant.mTargetX = plant.mTargetY = 0;
        if (FumeVariants.IsVariant(identity)) plant.mTargetX = plant.mTargetY = plant.mShootingCounter = 0;
        if (identity == FusionPlant.GutsyShroom) plant.mTargetX = 0;
        if (identity == FusionPlant.StarchShroom)
        {
            // Fume-shroom never writes target coordinates or mShootingCounter
            // outside Fire (suppressed below). All three fields serialize.
            plant.mTargetX = 1500;
            plant.mTargetY = plant.mShootingCounter = 0;
        }
    }

    internal static bool InRange(Plant plant, Zombie zombie, FusionPlant identity)
    {
        int rows = identity == FusionPlant.FumeSpreader ? 1 : 0;
        if (Math.Abs(zombie.mRow - plant.mRow) > rows
            || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || !SquashFusions.Eligible(plant.mBoard, zombie, plant.GetDamageRangeFlags(PlantWeapon.Primary))) return false;
        float x = plant.mX + plant.mWidth / 2f;
        var rect = zombie.GetZombieRect();
        float target = (rect.xMin + rect.xMax) / 2;
        // Source FumeShroom/FireFume use shadow.x through shadow.x + 7.
        // Mouse.GetColumnFromX divides source X by 1.35 per column.
        // ThreeFume and DoomFume remove the upper range limit entirely.
        return target >= x && (identity == FusionPlant.RayShroom || identity == FusionPlant.GlacierShroom || identity == FusionPlant.SootShroom || identity == FusionPlant.FumeSpreader
            || target <= x + 7f / 1.35f * 80);
    }

    internal static bool HasTarget(Plant plant, FusionPlant identity)
    {
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie != null && InRange(plant, zombie, identity)) return true;
        }
        return false;
    }

    internal static void Tick(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsFume(identity) || !Active(plant)) return;
        if (!FusionTime.SourceStep(plant, typeof(FumeFusions))) return;
        if (identity == FusionPlant.BelchShroom)
        {
            uint id = plant.DataID;
            EatingShrooms.TickBelch(plant);
            if (plant.DataID != id || !Active(plant)) return;
        }
        if (identity == FusionPlant.GlacierShroom) { GlacierShroom.Tick(plant); return; }
        if (identity == FusionPlant.RayShroom && plant.mTargetY > 0) --plant.mTargetY;
        if (identity == FusionPlant.BlubberShroom) { FumeSupport.TickBlubber(plant); return; }
        if (FumeVariants.IsVariant(identity)) { FumeVariants.Tick(plant, identity); return; }
        if (identity == FusionPlant.GutsyShroom) { GutsyShroom.Tick(plant); return; }
        if (identity == FusionPlant.StarchShroom)
        {
            if (plant.mTargetX > 0) --plant.mTargetX;
            if (plant.mShootingCounter > 0) --plant.mShootingCounter;
            if (--plant.mTargetY < 0)
            {
                plant.mShootingCounter = 12;
                plant.mTargetY = Math.Max(50, plant.mTargetX / 5);
            }
        }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (identity == FusionPlant.SootShroom || plant.mLaunchCounter > 0 || !(HasTarget(plant, identity) || identity == FusionPlant.TriggerShroom && TriggerShroom.HasCherry(plant))) return;
            Begin(plant, identity);
        }
        int elapsed = Duration(identity) - plant.mStartRow;
        // Standard fume clips emit at .35 seconds; StarFume emits at .333333.
        // ThreeFume.DelayShoot then waits 200 ms before its penetrating hit.
        if (elapsed == (identity == FusionPlant.RayShroom ? 25 : 35)) Smoke(plant, identity);
        if (identity == FusionPlant.FlameShroom)
        {
            // Original sustained flame: 33 one-damage hits at 50 Hz from
            // the emit event through the end of the one-second shoot clip.
            if (elapsed >= 35 && (elapsed - 35) % 2 == 0) Hit(plant, identity);
        }
        else if (elapsed == (identity == FusionPlant.BelchShroom ? 45 : identity == FusionPlant.RayShroom ? 25 : identity == FusionPlant.StarburstShroom ? 34 : identity == FusionPlant.FumeSpreader ? 55 : 35)) Hit(plant, identity);
        --plant.mStartRow;
    }

    private static void Begin(Plant plant, FusionPlant identity)
    {
        plant.mStartRow = Duration(identity);
        plant.mLaunchCounter = identity == FusionPlant.SootShroom ? 3000 : 150;
    }

    internal static bool Click(Plant plant, int clicks, int player)
    {
        if (FusionPlant.From(plant) != FusionPlant.SootShroom) return false;
        if (clicks >= 0 && player == 0 && Active(plant) && plant.mStartRow == 0
            && plant.mLaunchCounter == 0 && plant.mBoard.mCursorObject.CursorType == CursorType.Normal
            && LawnGloveMod.GetCarriedPlant(plant.mBoard) == null) Begin(plant, FusionPlant.SootShroom);
        return true;
    }

    internal static void Hit(Plant plant, FusionPlant identity)
    {
        if (identity == FusionPlant.StarburstShroom) { StarCaps.Fire(plant); return; }
        // Stable snapshot: damage/death callbacks can spawn or recycle zombies.
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie != null && InRange(plant, zombie, identity)) targets.Add((zombie, zombie.DataID));
        }
        if (identity == FusionPlant.TriggerShroom) TriggerShroom.Convert(plant);
        int bushDamage = identity == FusionPlant.BushShroom ? (int)Math.Min(int.MaxValue, 40L + plant.mTargetX) : 0;
        if (identity == FusionPlant.BushShroom) plant.mTargetX = 0;
        bool potato = identity == FusionPlant.StarchShroom && plant.mTargetX == 0;
        // Shoot1 consumes the charge even when its wave finds no targets.
        if (potato) plant.mTargetX = 1500;
        foreach (var (zombie, id) in targets)
        {
            if (!Active(plant) || zombie.DataID != id || !InRange(plant, zombie, identity)) continue;
            if (identity == FusionPlant.FootballShroom) { FootballFumeDamage.Hit(plant, zombie, id); continue; }
            if (potato)
            {
                // SmallBombPotato is a point overlap (range=0) in this row,
                // not a 3x3 mine. Each hit zombie supplies its own blast center.
                var rect = zombie.GetZombieRect();
                MineFusions.Blast(plant.mBoard, (rect.xMin + rect.xMax) / 2,
                    (rect.yMin + rect.yMax) / 2, 0, 300, plant.mRenderOrder, row: zombie.mRow);
                continue;
            }
            int damage = identity == FusionPlant.SootShroom ? 1800 : identity == FusionPlant.FlameShroom ? 1
                : identity == FusionPlant.GutsyShroom || identity == FusionPlant.FoulShroom || identity == FusionPlant.StarchShroom ? 30 : 40;
            if (identity == FusionPlant.BelchShroom) damage = plant.mTargetX > 0 ? 40 : 20;
            if (identity == FusionPlant.BushShroom) damage = bushDamage;
            if (identity == FusionPlant.RayShroom) damage = FumeSupport.RayDamage((int)MathF.Floor((zombie.GetZombieRect().center.x - plant.mBoard.GridToPixelX(0, zombie.mRow)) / 80f) - plant.mPlantCol, PumpkinFusions.LightLevel(plant));
            if (identity == FusionPlant.MorphShroom) damage = MetalArmor.FumeDamage(zombie, damage);
            if (identity == FusionPlant.RustShroom) damage = MetalArmor.RustDamage(zombie, 50);
            if (identity == FusionPlant.FumeSpreader) damage = ShootingPots.ThreeDamage(plant, damage);
            if (identity == FusionPlant.FumeSpreader && zombie.mRow == plant.mRow
                && (plant.mRow == 0 || plant.mRow == plant.mBoard.GetNumRows() - 1)) damage *= 2;
            if (identity == FusionPlant.FrostShroom) FusionStatuses.Chill(zombie);
            if (identity == FusionPlant.FlameShroom) zombie.RemoveColdEffects();
            if (identity == FusionPlant.PerfumeShroom && PerfumeShroom.TryHypnotize(plant, zombie)) continue;
            if (identity == FusionPlant.RayShroom) PlanternFusions.LaserHit(plant, zombie);
            if (zombie.mDead || zombie.IsDeadOrDying()) continue;
            zombie.TakeDamage(damage, DamageFlags.BypassesShield
                | (identity == FusionPlant.FlameShroom || identity == FusionPlant.SootShroom
                    ? DamageFlags.DoesntLeaveBody : (DamageFlags)0));
            if (zombie.DataID != id || !SquashFusions.Eligible(plant.mBoard, zombie, plant.GetDamageRangeFlags(PlantWeapon.Primary))) continue;
            if (identity == FusionPlant.BelchShroom && plant.mTargetX > 0)
                EatingShrooms.Knockback(zombie);
            if (identity == FusionPlant.FoulShroom) PoisonShooters.AddLevel(plant.mBoard, zombie);
            if (identity == FusionPlant.FrostShroom) SnowPea.Hit(zombie, 5);
            if (identity == FusionPlant.FlameShroom) Overheat.Apply(zombie);
        }
    }

    internal static void Smoke(Plant plant, FusionPlant identity)
    {
        var board = plant.mBoard;
        if (identity == FusionPlant.StarburstShroom || identity == FusionPlant.BlubberShroom) return;
        if (identity == FusionPlant.RayShroom) { ImportedPeaVisuals.Ray(plant); return; }
        if (identity == FusionPlant.BoostShroom)
        {
            for (float offset = 0; plant.mX + plant.mWidth / 2f + offset <= 900; offset += 80)
                ImportedPeaVisuals.FumeSmoke(plant, identity, offset: offset);
        }
        else
        {
            int first = identity == FusionPlant.FumeSpreader ? Math.Max(0, plant.mRow - 1) : plant.mRow;
            int last = identity == FusionPlant.FumeSpreader ? Math.Min(board.GetNumRows() - 1, plant.mRow + 1) : plant.mRow;
            for (int row = first; row <= last; ++row) ImportedPeaVisuals.FumeSmoke(plant, identity, row);
        }
        board.mApp.SoundSystem.PlayFoley(FoleyType.Fume);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class FumeInitializePatch
{
    internal static void Postfix(Plant __instance) => FumeFusions.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class FumeTickPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsFume(FusionPlant.From(__instance))) return true;
        FumeFusions.Tick(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class FumeNativeFirePatch
{
    // Native animation callbacks must not produce a second ordinary fume hit.
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsFume(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.MouseDown))]
internal static class SootClickPatch
{
    internal static bool Prefix(Plant __instance, int theClickCount, int playerIndex) =>
        !FumeFusions.Click(__instance, theClickCount, playerIndex);
}
