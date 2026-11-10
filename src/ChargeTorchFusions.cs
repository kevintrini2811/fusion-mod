using HarmonyLib;
using Il2CppReloaded.Gameplay;
using PlantLibrary;

namespace FusionMod;

// Native plant counters and projectile fields preserve charge, ownership and
// stored cherries. No pointer-only gameplay state survives pool reuse.
internal static class ChargeTorchFusions
{
    internal const int SmallFire = -624010, PotatoFire = -624011, FireCherry = -624012;
    internal static bool Owns(FusionPlant identity) => identity == FusionPlant.HotPotato || identity == FusionPlant.Boomwood;
    internal static bool IsFireCherry(Projectile shot) => shot.mProjectileType == ProjectileType.Fireball && shot.mCobTargetX == FireCherry;
    internal static bool IsLowFire(Projectile shot) => shot.mProjectileType == ProjectileType.Fireball && shot.mCobTargetX is SmallFire or PotatoFire;

    internal static void Initialize(Plant plant)
    {
        if (!Owns(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mTargetX = plant.mTargetY = plant.mShootingCounter = 0;
        plant.mState = PlantState.Notready; // Hot Potato is edible, with no contact fuse or underground growth.
        plant.mStateCountdown = 0;
    }

    internal static void Convert(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board == null || board.mPaused || shot.mPosZ != 0) return;
        var indexed = TorchPlantIndex.For(board);
        if (indexed.Length == 0) return;
        bool potato = PotatoProjectiles.Contains(shot), small = PuffShooters.Contains(shot);
        bool rolling = RollingProjectiles.TryShot(shot, out var roll) && roll.Nut && !roll.SolarBall;
        bool explosive = CherryShooter.IsExplosive(shot);
        bool snow = shot.mProjectileType == ProjectileType.Snowpea && shot.mCobTargetX >= 0;
        bool plain = shot.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea && shot.mCobTargetX >= 0 && !rolling;
        if (!potato && !small && !rolling && !explosive && !snow && !plain) return;
        var stumps = new List<Plant>();
        foreach (var entry in indexed)
        {
            var plant = entry.Plant;
            if (!entry.IsCurrent(board) || !Owns(FusionPlant.From(plant)) || !BeamShooters.Active(plant)
                || plant.NotOnGround() || plant.mRow != shot.mRow || shot.mHitTorchwoodGridX == plant.mPlantCol) continue;
            float x = plant.mX + plant.mWidth / 2f;
            if (Math.Min(shot.mPosX, shot.mPosX + shot.mVelX) <= x + 16
                && Math.Max(shot.mPosX, shot.mPosX + shot.mVelX) >= x - 16) stumps.Add(plant);
        }
        foreach (var plant in shot.mVelX >= 0 ? stumps.OrderBy(p => p.mPlantCol) : stumps.OrderByDescending(p => p.mPlantCol))
        {
            if (FusionPlant.From(plant) == FusionPlant.HotPotato)
            {
                if (rolling)
                {
                    RollingProjectiles.Ignite(shot, roll);
                    shot.mHitTorchwoodGridX = plant.mPlantCol;
                    CreditMine(plant, 1); return;
                }
                if (!potato && !small) continue;
                FusionProjectileTime.AdoptConversion(shot);
                ImportedPeaVisuals.ForgetProjectile(shot);
                shot.mCobTargetX = potato ? PotatoFire : SmallFire;
                shot.mCobTargetRow = potato ? 50 : 40;
                shot.mProjectileType = ProjectileType.Fireball;
                // Preserve arbitrary potato/puff direction; Star motion is the
                // native saved straight-vector path, without diagonal lane steering.
                shot.mMotionType = ProjectileMotion.Star;
                if (potato) FusionProjectileTime.SourceMotionHandoff(shot, FusionProjectileTime.Age | FusionProjectileTime.Planar);
                shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
                shot.mDamageRangeFlags = DamageRangeFlags.Ground;
                shot.mHitTorchwoodGridX = plant.mPlantCol;
                CreditMine(plant, potato ? 8 : 1);
                ImportedPeaVisuals.EnsureChargeTorchProjectile(shot);
                return;
            }
            if (explosive)
            {
                int damage = CherryShooter.ExplosionDamage(shot);
                // A full rack leaves the original explosive cherry intact.
                if (SummonCherry(plant, damage)) shot.Die();
                return;
            }
            if (!plain && !snow) continue;
            FusionProjectileTime.AdoptConversion(shot);
            ImportedPeaVisuals.ForgetProjectile(shot);
            if (snow) shot.ConvertToPea(plant.mPlantCol);
            else shot.ConvertToFireball(plant.mPlantCol);
            shot.mHitTorchwoodGridX = plant.mPlantCol;
            if (++plant.mTargetX > 30) SummonCherry(plant, 300);
            return;
        }
    }

    private static void CreditMine(Plant plant, int amount)
    {
        plant.mTargetX += amount;
        if (plant.mTargetX <= 100) return;
        plant.mTargetX = 0; // The source consumes charge even if the row is full.
        for (int col = plant.mPlantCol + 1; col < 9; ++col)
        {
            if (FusionService.CheckNativePlacement(plant.mBoard, col, plant.mRow, SeedType.Potatomine) != PlantingReason.Ok) continue;
            var previous = PlayerPlantingScope.Current;
            try
            {
                PlayerPlantingScope.Current = null;
                plant.mBoard.AddPlant(col, plant.mRow, FusionPlant.RoastedPotato.NativeType, FusionPlant.RoastedPotato.Marker);
            }
            finally { PlayerPlantingScope.Current = previous; }
            return;
        }
    }

    private static bool SummonCherry(Plant plant, int damage)
    {
        var shots = plant.mBoard.m_projectiles;
        int slot = Enumerable.Range(0, 3).FirstOrDefault(candidate => !Enumerable.Range(0, shots.Count).Any(i =>
            shots[i] is { mDead: false } shot && IsFireCherry(shot)
            && unchecked((uint)shot.mLastPortalX) == plant.DataID && shot.mHitTorchwoodGridX == candidate), -1);
        if (slot < 0) return false;
        var cherry = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f, plant.mY,
            plant.mRenderOrder + 1, plant.mRow, ProjectileType.Fireball);
        if (cherry == null || cherry.mDead) return false;
        cherry.mCobTargetX = FireCherry;
        cherry.mCobTargetRow = damage;
        cherry.mLastPortalX = unchecked((int)plant.DataID);
        cherry.mHitTorchwoodGridX = slot;
        cherry.mMotionType = ProjectileMotion.Puff;
        cherry.mDamageRangeFlags = DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged;
        cherry.mVelX = cherry.mVelY = cherry.mVelZ = cherry.mAccZ = cherry.mPosZ = 0;
        cherry.mShadowY = cherry.mPosY + PuffShooters.FlightHeight;
        plant.mTargetX = 0;
        Projectiles.NotifyBorn(cherry);
        ImportedPeaVisuals.EnsureChargeTorchProjectile(cherry);
        return true;
    }

    internal static bool MoveCherry(Projectile shot)
    {
        if (!IsFireCherry(shot)) return true;
        ImportedPeaVisuals.EnsureChargeTorchProjectile(shot);
        if (shot.mDead || shot.mBoard.mPaused) return false;
        Zombie? target = null;
        float nearest = float.MaxValue;
        for (int i = 0; i < shot.mBoard.m_zombies.Count; ++i)
        {
            var zombie = shot.mBoard.m_zombies[i];
            if (zombie == null || !BloverFusions.Live(shot.mBoard, zombie)
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || zombie.mMindControlled || zombie.mPosX >= 900
                || !zombie.EffectedByDamage(shot.mDamageRangeFlags)) continue;
            var rect = zombie.GetZombieRect();
            float dx = rect.center.x - shot.mPosX, dy = rect.center.y - shot.mPosY;
            float distance = dx * dx + dy * dy;
            if (distance < nearest) { target = zombie; nearest = distance; }
        }
        // Native Homing only steers toward its saved handle; it does not acquire
        // a target. Refresh that handle when the previous zombie disappears.
        shot.mTargetZombieID = (ZombieID)(target?.DataID ?? 0);
        if (target != null)
        {
            if (shot.mMotionType != ProjectileMotion.Homing) shot.mVelX = FusionProjectileTime.SourceVelocity(3.33f);
            shot.mMotionType = ProjectileMotion.Homing;
            return true;
        }
        if (shot.mMotionType == ProjectileMotion.Homing) return true;
        Plant? owner = null;
        for (int i = 0; i < shot.mBoard.m_plants.Count; ++i)
            if (shot.mBoard.m_plants[i] is { mDead: false } plant && plant.DataID == unchecked((uint)shot.mLastPortalX)
                && FusionPlant.From(plant) == FusionPlant.Boomwood) { owner = plant; break; }
        if (owner == null) { shot.Die(); return false; }
        float angle = FusionProjectileTime.AgeSeconds(shot) * 3 + shot.mHitTorchwoodGridX * MathF.PI * 2 / 3;
        shot.mPosX = owner.mX + owner.mWidth / 2f + MathF.Cos(angle) * 25;
        shot.mPosY = owner.mY + MathF.Sin(angle) * 15;
        shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class ChargeTorchInitializePatch
{ private static void Postfix(Plant __instance) => ChargeTorchFusions.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class ChargeTorchAbilityPatch
{ private static bool Prefix(Plant __instance) => !ChargeTorchFusions.Owns(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class ChargeTorchFirePatch
{ private static bool Prefix(Plant __instance) => !ChargeTorchFusions.Owns(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateTorchwood))]
internal static class ChargeTorchNativePatch
{ private static bool Prefix(Plant __instance) => !ChargeTorchFusions.Owns(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class ChargeTorchCrossingPatch
{
    [HarmonyPriority(Priority.First + 1000)]
    private static bool Prefix(Projectile __instance)
    {
        ChargeTorchFusions.Convert(__instance);
        return !__instance.mDead && !(ChargeTorchFusions.IsFireCherry(__instance) && __instance.mMotionType == ProjectileMotion.Puff);
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class ChargeTorchMotionPatch
{ private static bool Prefix(Projectile __instance) => ChargeTorchFusions.MoveCherry(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class ChargeTorchVisualPatch
{ private static void Postfix(Projectile __instance) { if (!__instance.mDead) ImportedPeaVisuals.EnsureChargeTorchProjectile(__instance); } }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class FireCherryImpactPatch
{
    [HarmonyPriority(Priority.First + 1000)]
    private static bool Prefix(Projectile __instance)
    {
        if (!ChargeTorchFusions.IsFireCherry(__instance)) return true;
        ExplodeOShooter.Detonate(__instance, __instance.mCobTargetRow);
        return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class ChargeTorchDeathPatch
{
    private static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state) =>
        __state = __runOriginal && ChargeTorchFusions.Owns(FusionPlant.From(__instance)) && CherryBombPlants.CanTriggerDeath(__instance)
            ? IceInstantFusions.Capture(__instance) : null;
    private static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state == null || !__instance.mDead || __instance.DataID != __state.Id) return;
        if (__state.Identity == FusionPlant.Boomwood) CherryBombPlants.Explode(__state);
        else MineFusions.Blast(__state.Board, __state.X, __state.Y, 80, 1800, __state.Order, row: __state.Row);
    }
}
