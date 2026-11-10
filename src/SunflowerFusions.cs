using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Identity, production, growth and ability phases use serialized native fields.
// Native UpdateAbilities retains coffee/wake handling for the aquatic producer;
// its ordinary shooter is disabled. Instant plants replace that method entirely.
internal static class SunflowerFusions
{
    internal const float SolarShotMarker = -450010;
    internal static bool IsShot(Projectile shot) => shot.mProjectileType == ProjectileType.Pea
        && shot.mMotionType == ProjectileMotion.Star && shot.mCobTargetX == SolarShotMarker;
    internal static bool Producer(FusionPlant identity) => identity == FusionPlant.SilverSunflower || identity == FusionPlant.SolarPot || identity == FusionPlant.SunsetShroom;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsSunflowerFusion(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mDoSpecialCountdown = 0;
        plant.mLaunchCounter = plant.mShootingCounter = plant.mStartRow = plant.mTargetY = 0;
        // mTargetX is the remaining growth time; mStartRow is the size stage.
        plant.mTargetX = identity == FusionPlant.SunsetShroom ? 2000 : 0;
    }

    internal static float Brightness(Plant plant) => plant.mShootingCounter <= 0 ? 1
        : 1 + 3 * (1 - Math.Abs(plant.mShootingCounter - 50) / 50f);

    internal static void Fertilize(Plant plant)
    {
        // Fertilize.Upgrade reduces a positive attribute countdown to at most
        // 0.5 seconds. It does not skip both SunSeaShroom growth stages.
        if (FusionPlant.From(plant) == FusionPlant.SunsetShroom && plant.mStartRow < 2)
            plant.mTargetX = Math.Min(plant.mTargetX, 50);
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (!FusionTime.SourceStep(plant, typeof(SunflowerFusions))) return;
        var identity = FusionPlant.From(plant);
        if (Producer(identity))
        {
            if (identity == FusionPlant.SunsetShroom)
            {
                if (plant.mTargetY > 0) --plant.mTargetY;
                if (plant.mStartRow < 2 && --plant.mTargetX <= 0)
                {
                    ++plant.mStartRow;
                    plant.mTargetX = plant.mStartRow == 1 ? 4000 : 0;
                    plant.mTargetY = 100;
                }
            }
            if (plant.mApp.GameMode is >= GameMode.PuzzleIZombie1 and <= GameMode.PuzzleIZombieEndless) return;
            if (plant.mShootingCounter > 0 && --plant.mShootingCounter == 50)
            {
                var type = identity == FusionPlant.SilverSunflower ? CoinType.Sun
                    : identity == FusionPlant.SolarPot || plant.mStartRow == 0 ? CoinType.SmallSun
                    : plant.mStartRow == 1 ? CoinType.Sun : CoinType.LargeSun;
                plant.mBoard.AddCoin(plant.mX, plant.mY, type, CoinMotion.FromPlant);
            }
            if (--plant.mLaunchCounter > 0) return;
            int interval = identity == FusionPlant.SolarPot ? 2500 : 1500;
            // SunPot inherits MoneyPot's +/-10%; Producer uses +/-5%.
            int jitter = identity == FusionPlant.SolarPot ? 10 : 5;
            plant.mLaunchCounter = interval * (100 - jitter) / 100
                + plant.mBoard.Rand(interval * jitter / 50 + 1);
            plant.mShootingCounter = 100;
        }
        else if (identity == FusionPlant.SolarFlare)
        {
            // The original explode clip invokes AnimExplode at 2/3 second.
            if (++plant.mTargetY >= 67) Flare(plant);
        }
        else if (identity == FusionPlant.SolarBlover)
        {
            int age = ++plant.mTargetY;
            if (age < 49) return;
            if (age == 49) Blow(plant);
            // Source coroutine emits 10..720 degrees, one shot per fixed tick.
            if ((age - 49) % 2 != 0) return;
            if (plant.mStartRow == 72) { plant.Die(); return; }
            float angle = -10 * ++plant.mStartRow;
            float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
            var shot = RetaliationProjectiles.Spawn(plant.mBoard, x, y, plant.mRenderOrder,
                plant.mRow, false, angle, 5 * (80f / 1.35f) / 100);
            if (shot != null && !shot.mDead)
            {
                shot.mCobTargetX = SolarShotMarker;
                ImportedPeaVisuals.EnsureBloverSunProjectile(shot);
            }
        }
    }

    private static void Blow(Plant plant)
    {
        var board = plant.mBoard;
        board.mFogBlownCountDown = Math.Max(board.mFogBlownCountDown, 3000);
        var balloons = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && zombie.mZombieType == ZombieType.Balloon
                && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Flying))
                balloons.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in balloons)
            if (zombie.DataID == id && !zombie.mDead && !zombie.IsDeadOrDying()) zombie.DieNoLoot();
    }

    internal static void Flare(Plant plant)
    {
        if (plant.mTargetY < 0) return;
        var board = plant.mBoard;
        int row = plant.mRow, order = plant.mRenderOrder;
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        // Mark completion before any death/damage callback can reenter.
        plant.mTargetY = -1;
        plant.Die();
        var victims = new List<(Plant Plant, uint Id)>();
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var victim = board.m_plants[i];
            if (victim != null && victim.mRow == row && ShroomSlots.Live(victim))
                victims.Add((victim, victim.DataID));
        }
        var targets = new List<(Zombie Zombie, uint Id)>();
        var range = DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
            | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Underground;
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && zombie.mRow == row && SquashFusions.Eligible(board, zombie, range))
                targets.Add((zombie, zombie.DataID));
        int damage = 1800 + 500 * victims.Count;
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie, range)) continue;
            zombie.RemoveColdEffects();
            zombie.TakeDamage(damage, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
            board.AddCoin(x, y, CoinType.Sun, CoinMotion.FromPlant);
        }
        foreach (var (victim, id) in victims)
        {
            if (victim.DataID != id || victim.mBoard?.Pointer != board.Pointer || !ShroomSlots.Live(victim)) continue;
            victim.mPlantHealth -= 1800;
            // The shipped method rescues strictly negative health, not zero.
            if (board.Rand(3) == 0 && victim.mPlantHealth < 0) victim.mPlantHealth = 1;
            if (victim.mPlantHealth <= 0) victim.Die();
            else victim.UpdateReanim();
            board.AddCoin(victim.mX, victim.mY, CoinType.LargeSun, CoinMotion.FromPlant);
        }
        NativeCombatVisuals.Fire(board, row, order);
        board.mApp.SoundSystem.PlayFoley(FoleyType.JalapenoIgnite);
        board.ShakeBoard(3, -4);
    }

    internal static void Impact(Projectile shot, Zombie? zombie)
    {
        if (shot.mDead) return;
        var board = shot.mBoard;
        float x = shot.mPosX, y = shot.mPosY;
        uint? id = zombie?.DataID;
        bool hit = zombie != null && !zombie.mDead && !zombie.IsDeadOrDying()
            && board != null && zombie.mBoard?.Pointer == board.Pointer;
        var flags = hit ? shot.GetDamageFlags(zombie!) : (DamageFlags)0;
        // End the projectile before damage callbacks can reenter or recycle
        // it. Retain the marker until native Reset, including nested impacts.
        shot.Die();
        if (hit && zombie != null && zombie.DataID == id && !zombie.mDead && !zombie.IsDeadOrDying()
            && board != null && zombie.mBoard?.Pointer == board.Pointer)
        {
            zombie.TakeDamage(10, flags);
            SolarShooter.DropSun(board, x, y, 5);
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class SolarFlareDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Plant __instance, bool __runOriginal, out uint? __state)
    {
        __state = __runOriginal && __instance.mTargetY >= 0
            && FusionPlant.From(__instance) == FusionPlant.SolarFlare
            && CherryBombPlants.CanTriggerDeath(__instance) && !__instance.NotOnGround()
            ? __instance.DataID : null;
    }
    internal static void Postfix(Plant __instance, uint? __state)
    {
        if (__state == __instance.DataID && __instance.mDead) SunflowerFusions.Flare(__instance);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SunflowerFusionInitializePatch
{
    internal static void Postfix(Plant __instance) => SunflowerFusions.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SunflowerFusionTickPatch
{
    internal static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (identity != FusionPlant.SolarFlare && identity != FusionPlant.SolarBlover
            && identity != FusionPlant.SilverSunflower) return true;
        SunflowerFusions.Tick(__instance);
        return false;
    }
    internal static void Postfix(Plant __instance)
    {
        if (SunflowerFusions.Producer(FusionPlant.From(__instance))
            && FusionPlant.From(__instance) != FusionPlant.SilverSunflower) SunflowerFusions.Tick(__instance);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class SunsetNativeShooterPatch
{
    internal static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.SunsetShroom;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class SunsetNativeFirePatch
{
    internal static bool Prefix(Plant __instance) => !SunflowerFusions.Producer(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class SunflowerProducerShootingPatch
{
    // The common native ability path must not consume production animation ticks.
    internal static bool Prefix(Plant __instance) => !SunflowerFusions.Producer(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class SunflowerNativeSpecialPatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsSunflowerFusion(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class BloverSunImpactPatch
{
    internal static bool Prefix(Projectile __instance, Zombie theZombie)
    {
        if (!SunflowerFusions.IsShot(__instance)) return true;
        SunflowerFusions.Impact(__instance, theZombie);
        return false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class BloverSunTorchwoodPatch
{
    internal static bool Prefix(Projectile __instance) => !SunflowerFusions.IsShot(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class BloverSunVisualPatch
{
    internal static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && SunflowerFusions.IsShot(__instance)) ImportedPeaVisuals.EnsureBloverSunProjectile(__instance);
    }
}
