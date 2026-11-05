using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// The native plant stores attack/fuse progress. Imported animation never fires
// gameplay events. Native placement, supports, health and save/load stay shared.
internal static class StarFusions
{
    internal static int ShootTicks(FusionPlant identity) => identity == FusionPlant.Starshooter ? 50 : 61;
    internal static int EventTick(FusionPlant identity) => identity == FusionPlant.Starshooter ? 34 : 38;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsStarFusion(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.IronStar ? 2000 : 300;
        plant.mDoSpecialCountdown = plant.mStartRow = plant.mShootingCounter = 0;
        plant.mTargetX = plant.mTargetY = 0;
        plant.mLaunchCounter = identity == FusionPlant.SpicyStarfruit ? 100 : 0;
    }

    internal static bool HasTarget(Plant plant)
    {
        bool pea = FusionPlant.From(plant) == FusionPlant.Starshooter;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)) continue;
            if (pea)
            {
                if (zombie.mRow == plant.mRow && zombie.GetZombieRect().xMax >= plant.mX + 40) return true;
            }
            else if (FusionPlant.From(plant) == FusionPlant.WinterStarfruit || StarRay(plant, zombie)) return true;
        }
        return false;
    }

    internal static bool HasStarTarget(Plant plant)
    {
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie != null && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
                && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground) && StarRay(plant, zombie)) return true;
        }
        return false;
    }

    private static bool StarRay(Plant plant, Zombie zombie)
    {
        var rect = zombie.GetZombieRect();
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        // Original five infinite rays. Native rectangles replace source 2D
        // colliders; test the finite X interval against each diagonal ray.
        if (rect.xMin <= x && rect.xMax >= x || zombie.mRow == plant.mRow && rect.xMin <= x) return true;
        float left = Math.Max(x, rect.xMin), right = rect.xMax;
        if (right < left) return false;
        foreach (float slope in new[] { -.5f, .5f })
        {
            float a = y + (left - x) * slope, b = y + (right - x) * slope;
            if (Math.Max(a, b) >= rect.yMin - 10 && Math.Min(a, b) <= rect.yMax + 10) return true;
        }
        return false;
    }

    internal static void Tick(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsStarFusion(identity) || !ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (identity == FusionPlant.SpicyStarfruit)
        {
            if (plant.mLaunchCounter == 0)
            {
                var death = CaptureSpicyDeath(plant);
                if (death != null)
                {
                    plant.mDoSpecialCountdown = 1;
                    plant.Die();
                    if (plant.mDead) Explode(death);
                    else plant.mDoSpecialCountdown = 0;
                }
            }
            return;
        }
        if (plant.mStartRow == 0 && plant.mLaunchCounter == 0 && HasTarget(plant))
        {
            plant.mLaunchCounter = 143 + plant.mBoard.Rand(15);
            plant.mStartRow = ShootTicks(identity);
        }
        if (plant.mStartRow == 0) return;
        int elapsed = ShootTicks(identity) - plant.mStartRow + 1;
        --plant.mStartRow;
        if (elapsed == EventTick(identity)) Fire(plant);
    }

    internal static void Fire(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        if (identity == FusionPlant.Starshooter)
        {
            StarProjectiles.Spawn(plant.mBoard, x + 20, y, plant.mRenderOrder, plant.mRow, 4, 0, 3.33f, 20);
            return;
        }
        if (identity == FusionPlant.WinterStarfruit)
        {
            for (int angle = 0; angle < 360; angle += 72)
            {
                var shot = StarProjectiles.Spawn(plant.mBoard, x, y, plant.mRenderOrder, plant.mRow, 2, angle, 0, 20);
                if (shot == null) continue;
                // Star motion never integrates Z velocity or acceleration.
                // Those serialized fields retain the fixed spiral origin.
                shot.mVelZ = x;
                shot.mAccZ = y;
                shot.mHitTorchwoodGridX = angle;
                shot.mTargetZombieID = (ZombieID)plant.DataID;
            }
        }
        else if (identity == FusionPlant.IronStar)
            foreach (float angle in StarProjectiles.Angles)
                StarProjectiles.Spawn(plant.mBoard, x, y, plant.mRenderOrder, plant.mRow, 8,
                    angle, angle == 180 ? 4.8f : 4, 80);
        else if (identity == FusionPlant.Starmorph)
            foreach (float angle in StarProjectiles.Angles)
                StarProjectiles.Spawn(plant.mBoard, x, y, plant.mRenderOrder, plant.mRow, 7,
                    angle, angle == 180 ? 4.8f : 4, 40);
        else if (identity == FusionPlant.Starlic)
            foreach (float angle in StarProjectiles.Angles)
                StarProjectiles.Spawn(plant.mBoard, x, y, plant.mRenderOrder, plant.mRow, 1,
                    angle, angle == 180 ? 4.8f : 4, 30);
    }

    internal static CherryBombPlants.Death? CaptureSpicyDeath(Plant plant) =>
        FusionPlant.From(plant) == FusionPlant.SpicyStarfruit && plant.mDoSpecialCountdown == 0
        && CherryBombPlants.CanTriggerDeath(plant) && !plant.NotOnGround()
        ? new(plant.mBoard, FusionPlant.SpicyStarfruit, plant.DataID,
            plant.mPlantCol, plant.mRow, plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f, plant.mRenderOrder) : null;

    internal static void FinishSpicyDeath(Plant plant, CherryBombPlants.Death? death)
    {
        if (death == null || !plant.mDead || plant.DataID != death.Id || plant.mDoSpecialCountdown != 0) return;
        // The timer owns its completion too. Native death entry paths can be
        // inlined; the saved latch prevents a hooked death from firing twice.
        plant.mDoSpecialCountdown = 1;
        Explode(death);
    }

    internal static void Explode(CherryBombPlants.Death death)
    {
        var board = death.Board;
        // Native DoFwoosh owns the complete row fire artwork (including roof
        // placement), but does not apply damage. Keep source damage below.
        board.DoFwoosh(death.Row);
        if (board.mIceTimer != null && death.Row >= 0 && death.Row < board.mIceTimer.Length)
            board.mIceTimer[death.Row] = Math.Min(board.mIceTimer[death.Row], 20);
        // Reuse the existing Jalapeno landing path, including cold removal,
        // elemental-nut healing, hot-pot activation and native ash damage.
        SquashFusions.ElementalBlast(new(board, death.Id, death.Identity,
            death.Col, death.Row, death.X, death.Y, death.Order), false);
        foreach (float angle in StarProjectiles.Angles)
            for (int spread = -20; spread <= 20; spread += 10)
                StarProjectiles.Spawn(board, death.X, death.Y, death.Order, death.Row, 3,
                    angle + spread, 6, 100);
    }

    internal static bool Bite(Plant plant, Zombie zombie)
    {
        if (FusionPlant.From(plant) != FusionPlant.Starlic) return false;
        if (!ChomperFusions.Active(plant) || !SupportFusions.CanRedirect(plant, zombie)) return true;
        // GarlicStar.OnEat calls EatGarlic(plant, 1, true). Ordinary source
        // cost is 15 HP; the adventure-only cost upgrade is not imported.
        plant.mPlantHealth -= PotBoosts.HasSupport(plant, FusionPlant.GarlicPot) ? 10 : 15;
        plant.mEatenFlashCountdown = 50;
        zombie.StopEating();
        zombie.mYuckyFace = true;
        zombie.mYuckyFaceCounter = 70; // One second before native row selection.
        zombie.mTargetRow = PotBoosts.HasSupport(plant, FusionPlant.GarlicPot) ? PotDefense.GarlicDirection : -1;
        zombie.UpdateAnimSpeed();
        if (plant.mPlantHealth <= 0) { ++plant.mBoard.mPlantsEaten; plant.Die(); }
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class StarInitializePatch
{
    private static void Postfix(Plant __instance) => StarFusions.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class StarAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsStarFusion(FusionPlant.From(__instance))) return true;
        StarFusions.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class StarShootingPatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsStarFusion(FusionPlant.From(__instance));
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class StarNativeFirePatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsStarFusion(FusionPlant.From(__instance));
}
[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class StarSpecialPatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsStarFusion(FusionPlant.From(__instance));
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class SpicyStarDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state)
    {
        __state = __runOriginal ? StarFusions.CaptureSpicyDeath(__instance) : null;
    }
    private static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        StarFusions.FinishSpicyDeath(__instance, __state);
    }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class StarlicBitePatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Zombie __instance, Plant thePlant, bool __runOriginal) =>
        !__runOriginal || !StarFusions.Bite(thePlant, __instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.StarFruitFire))]
internal static class StarAnimationFirePatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsStarFusion(FusionPlant.From(__instance));
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class StarShooterClockPatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsStarFusion(FusionPlant.From(__instance));
}
