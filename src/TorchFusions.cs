using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Native counters own the ignition cycle and summon credit across saves.
// Crossing is swept before movement; spawned peas cannot ignite again here.
internal static class TorchFusions
{
    internal static bool Owns(FusionPlant identity) => identity == FusionPlant.Nutwood || identity == FusionPlant.Squashwood
        || identity == FusionPlant.Torchthree || identity == FusionPlant.Sparkwood || identity == FusionPlant.Hypnowood
        || identity == FusionPlant.TorchDriftwood || identity == FusionPlant.Doomwood;
    private static bool Aquatic(FusionPlant identity) => identity == FusionPlant.TorchKelp
        || identity == FusionPlant.TorchDriftwood;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (Owns(identity))
        {
            plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.Nutwood ? 4000 : 300;
            plant.mTargetX = plant.mTargetY = plant.mShootingCounter = 0;
            if (identity == FusionPlant.Doomwood) plant.mTargetY = Doomwood.InitialEnergy;
            // The source Driftwood is a Torchwood with aquatic placement.
            if (identity == FusionPlant.TorchDriftwood) plant.mIsAsleep = false;
        }
        if (identity == FusionPlant.TorchKelp) plant.mTargetX = 0;
    }

    private static IEnumerable<Plant> Crossings(Projectile shot, bool aquaticOnly)
    {
        List<Plant>? candidates = null;
        bool backwards = shot.mMotionType == ProjectileMotion.Backwards;
        float dx = backwards ? -3.33f : shot.mVelX;
        foreach (var entry in TorchPlantIndex.For(shot.mBoard))
        {
            var plant = entry.Plant;
            if (!entry.IsCurrent(shot.mBoard)) continue;
            var identity = FusionPlant.From(plant);
            bool brazier = identity == FusionPlant.PumpkinBrazier;
            if (!(Aquatic(identity) || !aquaticOnly && (Owns(identity) || brazier))
                || !BeamShooters.Active(plant) || plant.NotOnGround()
                || plant.mRow != shot.mRow && !((brazier || identity == FusionPlant.Doomwood)
                    && shot.mMotionType is ProjectileMotion.Homing or ProjectileMotion.Star)
                || shot.mHitTorchwoodGridX == plant.mPlantCol) continue;
            float x = plant.mX + plant.mWidth / 2f;
            float y = plant.mY + plant.mHeight / 2f;
            if (Math.Min(shot.mPosX, shot.mPosX + dx) > x + 16
                || Math.Max(shot.mPosX, shot.mPosX + dx) < x - 16
                || Math.Abs(shot.mPosY + shot.mPosZ - y) > 60) continue;
            (candidates ??= new()).Add(plant);
        }
        if (candidates == null) return Array.Empty<Plant>();
        return dx >= 0 ? candidates.OrderBy(p => p.mPlantCol)
            : candidates.OrderByDescending(p => p.mPlantCol);
    }

    internal static void Convert(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board == null || board.mPaused || shot.mPosZ != 0) return;
        if (TorchPlantIndex.For(board).Length == 0) return;
        bool snow = shot.mProjectileType == ProjectileType.Snowpea && shot.mCobTargetX >= 0;
        bool hypno = HypnoShooters.Contains(shot) && !HypnoShooters.IsFire(shot);
        bool small = PuffShooters.Contains(shot);
        bool plain = shot.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea && shot.mCobTargetX >= 0;
        bool star = shot.mProjectileType == ProjectileType.Star && StarProjectiles.Family(shot) != 3
            && StarProjectiles.Family(shot) != 9 && CactusProjectiles.Family(shot) == 0;
        bool usual = snow || hypno || small || plain || star;
        if (!usual && !Doomwood.ExtraInput(shot)) return;
        foreach (var plant in Crossings(shot, aquaticOnly: small))
        {
            var identity = FusionPlant.From(plant);
            if (identity == FusionPlant.Doomwood)
            {
                if (Doomwood.Convert(plant, shot, snow, plain)) return;
                continue;
            }
            if (!usual) continue;
            if (Aquatic(identity))
            {
                if (hypno || star) continue;
                if (ConvertPea(shot, plant, snow, small)) CreditAquatic(plant, identity);
                return;
            }
            if (identity == FusionPlant.PumpkinBrazier)
            {
                if (hypno || star) continue;
                if (ConvertPea(shot, plant, snow, small: false) && ++plant.mTargetX > 50
                    && TorchweedFusions.SummonAhead(plant, FusionPlant.RoastedPumpkin))
                    plant.mTargetX = 0;
                return;
            }
            if (star)
            {
                if (identity != FusionPlant.Sparkwood) continue;
                StarProjectiles.Ignite(shot, plant.mPlantCol);
                return;
            }
            if (snow)
            {
                shot.ConvertToPea(plant.mPlantCol);
                if (identity == FusionPlant.Squashwood) CreditSquash(plant);
                return; // Squashwood counts thawing; other stumps do not.
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
            if (identity == FusionPlant.Squashwood) CreditSquash(plant);
            return;
        }
    }

    internal static bool ConvertPea(Projectile shot, Plant plant, bool snow, bool small)
    {
        FusionProjectileTime.AdoptConversion(shot);
        ImportedPeaVisuals.ForgetProjectile(shot);
        if (small)
        {
            shot.mCobTargetX = ChargeTorchFusions.SmallFire;
            shot.mCobTargetRow = 40;
            shot.mProjectileType = ProjectileType.Fireball;
            shot.mMotionType = ProjectileMotion.Star;
            shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
            ImportedPeaVisuals.EnsureChargeTorchProjectile(shot);
            shot.mBoard.mApp.SoundSystem.PlayFoley(FoleyType.FirePea);
        }
        else if (snow)
        {
            shot.ConvertToPea(plant.mPlantCol);
            if (shot.mProjectileType == ProjectileType.Snowpea) return false;
        }
        else
        {
            shot.ConvertToFireball(plant.mPlantCol);
            if (shot.mProjectileType is not (ProjectileType.Fireball or ProjectileType.PeashooterFireball)) return false;
        }
        shot.mHitTorchwoodGridX = plant.mPlantCol;
        return true;
    }

    internal static void IgniteKelp(Projectile shot)
    {
        if (KelpProjectiles.IsFire(shot)) return;
        var plant = Crossings(shot, aquaticOnly: true).FirstOrDefault();
        if (plant == null) return;
        ImportedPeaVisuals.ForgetProjectile(shot);
        KelpProjectiles.Ignite(shot, plant.mPlantCol);
        CreditAquatic(plant, FusionPlant.From(plant));
        shot.mBoard.mApp.SoundSystem.PlayFoley(FoleyType.FirePea);
    }

    private static void CreditAquatic(Plant plant, FusionPlant identity)
    {
        bool kelp = identity == FusionPlant.TorchKelp;
        if (++plant.mTargetX <= (kelp ? 100 : 50)) return;
        plant.mTargetX = 0; // Source spends the count even if placement fails.
        if (kelp)
        {
            TorchweedFusions.SummonAhead(plant, FusionPlant.RoastedKelp);
            return;
        }
        int col = plant.mPlantCol + 1;
        if (col >= 9) return;
        var child = plant.mBoard.GetPlantsOnLawn(col, plant.mRow).NormalPlant;
        // The immediate right cell grows an existing child up to eleven times.
        if (child != null && FusionPlant.From(child) == FusionPlant.RoastedSeaShroom && !child.mDead
            && child.mIsOnBoard && !child.NotOnGround())
            child.mShootingCounter = Math.Min(11, child.mShootingCounter + 1);
        else if (FusionService.CheckNativePlacement(plant.mBoard, col, plant.mRow, SeedType.Seashroom) == PlantingReason.Ok)
            TorchweedFusions.SummonAt(plant, FusionPlant.RoastedSeaShroom, col);
    }

    internal static void CreditSquash(Plant plant)
    {
        // SquashTorch consumes charge only after SetPlant finds a legal tile.
        if (++plant.mTargetY <= 50) return;
        if (TorchweedFusions.SummonAhead(plant, FusionPlant.RoastedSquash)) plant.mTargetY = 0;
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
{
    private static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (identity == FusionPlant.Doomwood) Doomwood.Tick(__instance);
        return !TorchFusions.Owns(identity);
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateTorchwood))]
internal static class TorchFusionNativePatch
{ private static bool Prefix(Plant __instance) => !TorchFusions.Owns(FusionPlant.From(__instance)); }
