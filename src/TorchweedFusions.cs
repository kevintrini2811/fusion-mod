using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// CaltropTorch owns a separate one-second fire clock, not native Spikeweed's
// melee attack. All gameplay state lives in saved native plant/projectile fields.
internal static class TorchweedFusions
{
    // Replanted updates plants once per board tick (currently 120 Hz).
    // Source fire is one second, independent of legacy 100-tick counters.
    internal static int DurationTicks(double seconds) => Math.Max(1, (int)Math.Round(seconds / Il2CppReloaded.Constants.Gameplay.TIME_FOR_UPDATE));
    internal static int FireTicks => DurationTicks(1);
    internal static int RoastedAttackTicks => DurationTicks(2d / 3);
    internal static int RoastedHitTicks => DurationTicks(1d / 3);
    internal static bool Roasted(Plant plant) => FusionPlant.From(plant) == FusionPlant.RoastedSpikeweed;
    internal static bool OwnsAbility(Plant plant) => Owns(plant) || Roasted(plant);
    internal static bool Owns(Plant plant) => FusionPlant.From(plant) == FusionPlant.Torchweed;

    internal static void Initialize(Plant plant)
    {
        if (!OwnsAbility(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchCounter = FireTicks;
        plant.mTargetX = plant.mTargetY = plant.mStartRow = plant.mShootingCounter = 0;
    }

    internal static bool Target(Plant plant, Zombie zombie)
    {
        var range = DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Dog;
        // Native EffectedByDamage categorizes BalloonPopping as OffGround before
        // checking Flying. Source Fall already exposes the body during this clip.
        if (Owns(plant) && zombie.mZombieType == ZombieType.Balloon && zombie.mZombiePhase == ZombiePhase.BalloonPopping)
            range |= DamageRangeFlags.OffGround;
        if (zombie.mRow != plant.mRow || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || !SquashFusions.Eligible(plant.mBoard, zombie, range)) return false;
        // SummonFire casts a square about the source shadow, then filters lane.
        float x = plant.mX + plant.mWidth / 2f;
        var rect = zombie.GetZombieRect();
        return rect.xMax >= x - ScaredyFusions.SourceUnit / 2 && rect.xMin <= x + ScaredyFusions.SourceUnit / 2;
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant)) return;
        if (Roasted(plant)) { TickRoasted(plant); return; }
        if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
        plant.mLaunchCounter = FireTicks;
        var board = plant.mBoard;
        uint plantId = plant.DataID;
        bool vehicleContact = false;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && Target(plant, zombie)) targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (plant.DataID != plantId || !ChomperFusions.Active(plant)) return;
            if (zombie.DataID != id || !Target(plant, zombie)) continue;
            board.mApp.SoundSystem.PlayFoley(FoleyType.FirePea);
            // Like Caltrop.KillCar, contact with a vehicle destroys both objects.
            if (zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult)
            {
                plant.mPlantHealth = 0;
                KillVehicle(zombie);
                vehicleContact = true;
                continue;
            }
            FireDamage(zombie);
            if (zombie.DataID == id && zombie.mBoard?.Pointer == board.Pointer && !zombie.mDead) zombie.RemoveColdEffects();
        }
        if (vehicleContact && plant.DataID == plantId && !plant.mDead) plant.Die();
    }

    internal static void FireDamage(Zombie zombie)
    {
        // Native TakeDamage owns modifiers and its inlined flying-armor writer.
        // Source Balloon.FirstArmorTakeDamage absorbs all overflow on this hit.
        bool armor = zombie.mZombieType == ZombieType.Balloon && zombie.IsFlying() && zombie.mFlyingHealth > 0;
        var previous = TorchweedBalloonArmor.Current;
        TorchweedBalloonArmor.Current = armor
            ? new(zombie.Pointer, zombie.mBoard.Pointer, zombie.DataID, TorchweedBalloonArmor.Depth + 1) : null;
        try
        {
            // DoesntLeaveBody suppresses native balloon popping, so reserve it
            // for body hits after the original flying armor has been consumed.
            zombie.TakeDamage(40, DamageFlags.BypassesShield | (armor ? (DamageFlags)0 : DamageFlags.DoesntLeaveBody));
        }
        finally { TorchweedBalloonArmor.Current = previous; }
    }

    internal static void KillVehicle(Zombie zombie)
    {
        // Source KillByCaltrop starts the vehicle-specific destruction, not a
        // numerical hit. Native death routines preserve tires, debris and phase.
        var board = zombie.mBoard;
        uint id = zombie.DataID;
        var flags = DamageFlags.BypassesShield | DamageFlags.Spike;
        if (zombie.mZombieType == ZombieType.Zamboni) zombie.ZamboniDeath(flags);
        else zombie.CatapultDeath(flags);
        if (zombie.DataID == id && zombie.mBoard?.Pointer == board?.Pointer) zombie.DropLoot();
    }

    internal static int RoastedInterval(Plant plant)
    {
        // Original Caltrop.Update adds a uniform [-.1,+.1] seconds after each
        // attack check, even when no target is present. Quantize only at the end.
        double jitter = (plant.mBoard.Rand(200001) - 100000) / 1000000d;
        return DurationTicks(1 + jitter);
    }

    internal static bool RoastedTarget(Plant plant, Zombie zombie) => !zombie.IsFlying() && Target(plant, zombie);

    internal static void TickRoasted(Plant plant)
    {
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        bool check = plant.mLaunchCounter == 0;
        if (check) plant.mLaunchCounter = RoastedInterval(plant);
        if (plant.mStartRow > 0)
        {
            if (--plant.mStartRow == RoastedAttackTicks - RoastedHitTicks) HitRoasted(plant);
            return;
        }
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || !RoastedTarget(plant, zombie)) continue;
            // Caltrop.OnTriggerEnter2D requests the same attack clip immediately
            // for a vehicle. Ordinary contact waits for the attack check.
            if (!check && zombie.mZombieType is not (ZombieType.Zamboni or ZombieType.Catapult)) continue;
            plant.mStartRow = RoastedAttackTicks;
            return;
        }
    }

    internal static void HitRoasted(Plant plant)
    {
        var board = plant.mBoard;
        uint plantId = plant.DataID;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && RoastedTarget(plant, zombie)) targets.Add((zombie, zombie.DataID));
        bool vehicle = false, hit = false;
        // AnimAttack calls KillCar before its ordinary damage loop. The source
        // sets its health to zero but finishes this event before the next Update.
        foreach (var (zombie, id) in targets)
        {
            if (plant.DataID != plantId || !ChomperFusions.Active(plant)) return;
            if (zombie.DataID != id || !RoastedTarget(plant, zombie)
                || zombie.mZombieType is not (ZombieType.Zamboni or ZombieType.Catapult)) continue;
            vehicle = true;
            plant.mPlantHealth = 0;
            KillVehicle(zombie);
        }
        foreach (var (zombie, id) in targets)
        {
            if (plant.DataID != plantId || !ChomperFusions.Active(plant)) return;
            if (zombie.DataID != id || !RoastedTarget(plant, zombie)) continue;
            zombie.TakeDamage(20, DamageFlags.BypassesShield | DamageFlags.Spike);
            hit = true;
        }
        if (hit) board.mApp.SoundSystem.PlayFoley(FoleyType.Splat);
        if (vehicle && plant.DataID == plantId && !plant.mDead) plant.Die();
    }

    internal static void Convert(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board == null || board.mPaused || shot.mPosZ != 0) return;
        bool small = PuffShooters.Contains(shot);
        bool rolling = RollingProjectiles.TryShot(shot, out var roll) && roll.Nut && !roll.SolarBall
            && shot.mCobTargetX != RollingProjectiles.IgnitedNut;
        if (!small && !rolling) return;
        List<Plant>? candidates = null;
        foreach (var entry in TorchPlantIndex.For(board))
        {
            var plant = entry.Plant;
            if (!entry.IsCurrent(board) || !Owns(plant) || !BeamShooters.Active(plant)
                || plant.NotOnGround() || plant.mRow != shot.mRow || shot.mHitTorchwoodGridX == plant.mPlantCol) continue;
            float x = plant.mX + plant.mWidth / 2f;
            if (Math.Min(shot.mPosX, shot.mPosX + shot.mVelX) <= x + 16
                && Math.Max(shot.mPosX, shot.mPosX + shot.mVelX) >= x - 16) (candidates ??= new()).Add(plant);
        }
        if (candidates == null) return;
        var first = shot.mVelX >= 0 ? candidates.OrderBy(p => p.mPlantCol).First() : candidates.OrderByDescending(p => p.mPlantCol).First();
        if (rolling) RollingProjectiles.Ignite(shot, roll);
        else
        {
            ImportedPeaVisuals.ForgetProjectile(shot);
            shot.mCobTargetX = ChargeTorchFusions.SmallFire;
            shot.mCobTargetRow = 40;
            shot.mProjectileType = ProjectileType.Fireball;
            shot.mMotionType = ProjectileMotion.Star;
            shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
            shot.mDamageRangeFlags = DamageRangeFlags.Ground;
            ImportedPeaVisuals.EnsureChargeTorchProjectile(shot);
        }
        shot.mHitTorchwoodGridX = first.mPlantCol;
        if (++first.mTargetX <= 100) return;
        first.mTargetX = 0; // Unlike Squashwood, consumes charge even on a full row.
        SummonAhead(first, FusionPlant.RoastedSpikeweed);
    }

    internal static bool SummonAhead(Plant plant, FusionPlant result)
    {
        var board = plant.mBoard;
        for (int col = plant.mPlantCol + 1; col < 9; ++col)
        {
            if (FusionService.CheckNativePlacement(board, col, plant.mRow, result.NativeType) != PlantingReason.Ok) continue;
            var previous = PlayerPlantingScope.Current;
            try
            {
                // This is a free source summon, never another seed transaction.
                PlayerPlantingScope.Current = null;
                var child = board.AddPlant(col, plant.mRow, result.NativeType, result.Marker);
                return child != null && !child.mDead && child.mIsOnBoard;
            }
            finally { PlayerPlantingScope.Current = previous; }
        }
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class TorchweedInitializePatch
{ private static void Postfix(Plant __instance) => TorchweedFusions.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class TorchweedAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!TorchweedFusions.OwnsAbility(__instance)) return true;
        TorchweedFusions.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateSpikeweed))]
internal static class TorchweedNativePatch
{ private static bool Prefix(Plant __instance) => !TorchweedFusions.OwnsAbility(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class TorchweedCrossingPatch
{ [HarmonyPriority(Priority.First + 1100)] private static void Prefix(Projectile __instance) => TorchweedFusions.Convert(__instance); }

// Synchronous hit scope only; no saved or pointer-only gameplay state. Native
// difficulty/resistance prefixes still run once on the original TakeDamage call.
internal static class TorchweedBalloonArmor
{
    internal readonly record struct Hit(IntPtr Zombie, IntPtr Board, uint Id, int Depth);
    [ThreadStatic] internal static Hit? Current;
    [ThreadStatic] internal static int Depth;
    internal static bool Overflow(Zombie zombie) => Current is { } hit && hit.Depth == Depth
        && hit.Zombie == zombie.Pointer && hit.Board == zombie.mBoard?.Pointer && hit.Id == zombie.DataID;
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class TorchweedBalloonDamageDepthPatch
{
    internal static void Prefix(Zombie __instance, out int? __state)
    {
        __state = null;
        if (TorchweedBalloonArmor.Current is not { } hit || hit.Zombie != __instance.Pointer) return;
        __state = TorchweedBalloonArmor.Depth;
        ++TorchweedBalloonArmor.Depth;
    }
    internal static void Finalizer(int? __state)
    { if (__state.HasValue) TorchweedBalloonArmor.Depth = __state.Value; }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeBodyDamage))]
internal static class TorchweedBalloonBodyOverflowPatch
{
    [HarmonyPriority(Priority.First + 2000)]
    internal static bool Prefix(Zombie __instance) => !TorchweedBalloonArmor.Overflow(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeHelmDamage))]
internal static class TorchweedBalloonHelmOverflowPatch
{
    [HarmonyPriority(Priority.First + 2000)]
    internal static bool Prefix(Zombie __instance, ref int __result)
    {
        if (!TorchweedBalloonArmor.Overflow(__instance)) return true;
        __result = 0;
        return false;
    }
}
