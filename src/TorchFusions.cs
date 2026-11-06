using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native counters own the ignition cycle and summon credit across saves.
// Crossing is swept before movement; spawned peas cannot ignite again here.
internal static class TorchFusions
{
    internal static bool Owns(FusionPlant identity) => identity == FusionPlant.Nutwood
        || identity == FusionPlant.Torchthree || identity == FusionPlant.Sparkwood || identity == FusionPlant.Hypnowood;

    internal static void Initialize(Plant plant)
    {
        if (!Owns(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = FusionPlant.From(plant) == FusionPlant.Nutwood ? 4000 : 300;
        plant.mTargetX = plant.mTargetY = plant.mShootingCounter = 0;
    }

    internal static void Convert(Projectile shot)
    {
        if (shot.mDead || shot.mBoard == null || shot.mBoard.mPaused || shot.mPosZ != 0) return;
        bool snow = shot.mProjectileType == ProjectileType.Snowpea && shot.mCobTargetX >= 0;
        bool hypno = HypnoShooters.Contains(shot) && !HypnoShooters.IsFire(shot);
        bool plain = shot.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea && shot.mCobTargetX >= 0;
        bool star = shot.mProjectileType == ProjectileType.Star && StarProjectiles.Family(shot) != 3
            && StarProjectiles.Family(shot) != 9 && CactusProjectiles.Family(shot) == 0;
        if (!snow && !hypno && !plain && !star) return;
        var candidates = new List<Plant>();
        for (int i = 0; i < shot.mBoard.m_plants.Count; ++i)
        {
            var plant = shot.mBoard.m_plants[i];
            if (plant == null || !Owns(FusionPlant.From(plant)) || !BeamShooters.Active(plant) || plant.NotOnGround()
                || plant.mRow != shot.mRow || shot.mHitTorchwoodGridX == plant.mPlantCol) continue;
            float x = plant.mX + plant.mWidth / 2f;
            float y = plant.mY + plant.mHeight / 2f;
            if (Math.Min(shot.mPosX, shot.mPosX + shot.mVelX) > x + 16
                || Math.Max(shot.mPosX, shot.mPosX + shot.mVelX) < x - 16
                || Math.Abs(shot.mPosY - y) > 60) continue;
            candidates.Add(plant);
        }
        foreach (var plant in shot.mVelX >= 0 ? candidates.OrderBy(p => p.mPlantCol) : candidates.OrderByDescending(p => p.mPlantCol))
        {
            var identity = FusionPlant.From(plant);
            if (star)
            {
                if (identity != FusionPlant.Sparkwood) continue;
                StarProjectiles.Ignite(shot, plant.mPlantCol);
                return;
            }
            if (snow)
            {
                shot.ConvertToPea(plant.mPlantCol);
                return; // Thawing does not count as ignition.
            }
            if (hypno)
            {
                if (identity != FusionPlant.Hypnowood || HypnoShooters.FromAlly(shot)) continue;
                HypnoShooters.Ignite(shot, plant.mPlantCol);
                Credit(plant, 4);
                return;
            }
            if (identity == FusionPlant.Torchthree)
            {
                int cycle = Math.Clamp(plant.mTargetX, 0, 2);
                plant.mTargetX = (cycle + 1) % 3;
                // Capture all input fields before Die can return the pool slot.
                var board = shot.mBoard;
                float x = shot.mPosX, y = shot.mPosY;
                int row = shot.mRow, order = shot.mRenderOrder;
                shot.Die();
                if (cycle != 1) Spawn(board, x, y, row, row, order, plant.mPlantCol);
                if (cycle != 0)
                    foreach (int direction in new[] { -1, 1 })
                    {
                        int destination = Math.Clamp(row + direction, 0, board.GetNumRows() - 1);
                        Spawn(board, x + (destination == row ? 8 : 0), y, row, destination, order, plant.mPlantCol);
                    }
                return;
            }
            ImportedPeaVisuals.ForgetProjectile(shot);
            shot.ConvertToFireball(plant.mPlantCol);
            shot.mHitTorchwoodGridX = plant.mPlantCol;
            if (identity == FusionPlant.Nutwood || identity == FusionPlant.Hypnowood) Credit(plant, 1);
            return;
        }
    }

    private static void Spawn(Board board, float x, float y, int origin, int row, int order, int col)
    {
        var child = board.AddProjectile(x, y, order, row, ProjectileType.Fireball);
        if (child == null || child.mDead) return;
        child.mDamageRangeFlags = DamageRangeFlags.Ground;
        child.mHitTorchwoodGridX = col;
        child.mMotionType = origin == row ? ProjectileMotion.Straight : ProjectileMotion.Threepeater;
        child.mVelX = 3.33f;
        child.mVelY = Math.Sign(row - origin) * 3;
        child.mShadowY = y + PuffShooters.FlightHeight;
    }

    private static void Credit(Plant plant, int amount)
    {
        plant.mTargetY += amount;
        if (FusionPlant.From(plant) == FusionPlant.Hypnowood)
        {
            // Source OnFirePea compares strictly greater than 60, not >= 60.
            if (plant.mTargetY <= 60) return;
            var ally = HypnoShooters.SummonFire(plant.mBoard, plant.mRow, plant.mX + plant.mWidth / 2f);
            if (ally != null) plant.mTargetY = 0;
        }
        else if (plant.mTargetY >= 50)
        {
            // Source scans forward until it can place a roasted nut. A full
            // row keeps its credit and retries after the next ignition.
            for (int col = plant.mPlantCol + 1; col < 9; ++col)
            {
                if (FusionService.CheckNativePlacement(plant.mBoard, col, plant.mRow, SeedType.Wallnut) != PlantingReason.Ok) continue;
                var nut = plant.mBoard.AddPlant(col, plant.mRow, FusionPlant.RoastedWallNut.NativeType, FusionPlant.RoastedWallNut.Marker);
                if (nut != null && !nut.mDead && nut.mIsOnBoard) plant.mTargetY = 0;
                break;
            }
        }
    }

    internal static bool Bite(Plant plant, Zombie zombie)
    {
        if (FusionPlant.From(plant) != FusionPlant.Hypnowood || !ChomperFusions.Active(plant)
            || zombie.mBoard?.Pointer != plant.mBoard.Pointer || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled) return false;
        var replacement = plant.mBoard.AddPlant(plant.mPlantCol, plant.mRow, SeedType.Torchwood, SeedType.None);
        if (replacement == null || replacement.mDead || !replacement.mIsOnBoard) return false;
        HypnoPlants.MindControl(zombie);
        PlayerPlantingScope.Consume(plant);
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class TorchFusionInitializePatch
{ private static void Postfix(Plant __instance) => TorchFusions.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class TorchFusionFirePatch
{ private static bool Prefix(Plant __instance) => !TorchFusions.Owns(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class TorchFusionCrossingPatch
{ [HarmonyPriority(Priority.First + 900)] private static void Prefix(Projectile __instance) => TorchFusions.Convert(__instance); }
[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class HypnowoodBitePatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Zombie __instance, Plant thePlant, bool __runOriginal) =>
        !__runOriginal || thePlant == null || !TorchFusions.Bite(thePlant, __instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class TorchFusionAbilityPatch
{ private static bool Prefix(Plant __instance) => !TorchFusions.Owns(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateTorchwood))]
internal static class TorchFusionNativePatch
{ private static bool Prefix(Plant __instance) => !TorchFusions.Owns(FusionPlant.From(__instance)); }
