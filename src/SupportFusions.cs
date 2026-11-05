using HarmonyLib;
using Il2CppReloaded.Gameplay;
using LawnGlove;

namespace FusionMod;

// Original ability events run on simulation ticks, never on rendering clocks.
// The native saved fields below have no competing writers on these variants:
// their ability/shooter paths are replaced, and DoSpecial is suppressed for
// Fallout Blover. Flytrapper retains the native Chomper state machine.
internal static class SupportFusions
{
    internal const int SolarShotTicks = 34, SolarEventTick = 21;
    internal const float SolarShotMarker = -440100;
    private const float Unit = 80f / 1.35f;
    internal static bool IsSolarShot(Projectile shot) => shot.mProjectileType == ProjectileType.Star
        && shot.mCobTargetX == SolarShotMarker;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsSupportFusion(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        if (identity == FusionPlant.Flytrapper) return;
        plant.mDoSpecialCountdown = 0;
        plant.mStartRow = plant.mTargetX = plant.mTargetY = 0;
        plant.mLaunchCounter = identity == FusionPlant.FalloutBlover ? 49
            : identity == FusionPlant.HypnoGarlic ? 50 : 0;
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.FalloutBlover)
        {
            ++plant.mTargetY;
            if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
            bool first = plant.mStartRow == 0;
            // Blover.AnimBlow invokes the overridden BlowEffect immediately.
            // KeepEffect then invokes it ten more times at one-second intervals.
            plant.mStartRow = 1;
            if (first) plant.mBoard.mFogBlownCountDown = Math.Max(plant.mBoard.mFogBlownCountDown, 3000);
            if (!first) ++plant.mTargetX;
            plant.mLaunchCounter = 100;
            FalloutPulse(plant);
            if (!plant.mDead && plant.mTargetX >= 10) plant.Die();
        }
        else if (identity == FusionPlant.Solarfruit)
        {
            if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
            if (plant.mStartRow > 0)
            {
                int elapsed = SolarShotTicks - plant.mStartRow + 1;
                --plant.mStartRow;
                // Original controller plays the .666667-second clip at speed 2.
                if (elapsed == SolarEventTick) SolarStars(plant);
            }
            if (plant.mLaunchCounter == 0 && HasStarryPumpkin(plant)) ShootSolar(plant);
        }
        else if (identity == FusionPlant.HypnoGarlic)
        {
            if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
            plant.mLaunchCounter = 50;
            // AttributeEvent: radius one source unit, same row, friendly team.
            var board = plant.mBoard;
            for (int i = 0; i < board.m_zombies.Count; ++i)
            {
                var zombie = board.m_zombies[i];
                if (zombie == null || !zombie.mMindControlled || !CanRedirect(plant, zombie)) continue;
                float x = plant.mX + plant.mWidth / 2f;
                var rect = zombie.GetZombieRect();
                if (Math.Abs(x - Math.Clamp(x, rect.xMin, rect.xMax)) <= Unit) Redirect(zombie);
            }
        }
    }

    internal static void FalloutPulse(Plant plant)
    {
        var board = plant.mBoard;
        var range = DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
            | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Underground;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie != null && zombie.mZombieType is not (ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target)
                && SquashFusions.Eligible(board, zombie, range)) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie, range)) continue;
            ImportedPeaVisuals.ShowFalloutSplat(zombie);
            zombie.TakeDamage(180, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
        }
    }

    private static bool HasStarryPumpkin(Plant plant)
    {
        var shell = plant.mBoard.GetPlantsOnLawn(plant.mPlantCol, plant.mRow).PumpkinPlant;
        // The conditional ability exists even though this accessory's own
        // recipe remains unavailable. Its save pair is reserved centrally.
        return shell != null && !shell.mDead && shell.mIsOnBoard && !shell.mSquished
            && new FusionPlant(shell.mSeedType, shell.mImitaterType) == FusionPlant.StarryPumpkin;
    }

    internal static bool ShootSolar(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.Solarfruit || !ChomperFusions.Active(plant)
            || plant.mIsAsleep || plant.mLaunchCounter > 0 || plant.mStartRow > 0) return false;
        var board = plant.mBoard;
        // Both actual SunStar entry points compare sunlight strictly > 25.
        if (!board.CanTakeSunMoney(26, 0) || !board.TakeSunMoney(25, 0)) return false;
        plant.mStartRow = SolarShotTicks;
        plant.mLaunchCounter = 35;
        return true;
    }

    internal static void SolarStars(Plant plant)
    {
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        foreach (float angle in new[] { 30f, -30f, 180f, 90f, -90f })
        {
            var shot = RetaliationProjectiles.Spawn(plant.mBoard, x, y, plant.mRenderOrder,
                plant.mRow, true, angle, angle == 180 ? 4.8f : 4);
            if (shot != null && !shot.mDead) shot.mCobTargetX = SolarShotMarker;
        }
    }

    internal static bool CanRedirect(Plant plant, Zombie zombie) => !zombie.mDead && !zombie.IsDeadOrDying()
        && zombie.mBoard?.Pointer == plant.mBoard.Pointer && zombie.mRow == plant.mRow
        && !zombie.mYuckyFace && zombie.EffectedByDamage(DamageRangeFlags.Ground | DamageRangeFlags.Dog
            | (zombie.mMindControlled ? DamageRangeFlags.OnlyMindcontrolled : 0))
        && zombie.mZombieType is not (ZombieType.Boss or ZombieType.Gravestone
            or ZombieType.Target or ZombieType.Zamboni or ZombieType.Catapult or ZombieType.Bungee);

    internal static void Redirect(Zombie zombie)
    {
        // Reuse native legal-row selection, water/land compatibility, movement
        // and serialization. Start immediately before its row-selection event.
        zombie.StopEating();
        zombie.mYuckyFace = true;
        zombie.mYuckyFaceCounter = 169;
    }

    internal static bool BiteGarlic(Plant plant, Zombie zombie)
    {
        if (FusionPlant.From(plant) != FusionPlant.HypnoGarlic) return false;
        if (!ChomperFusions.Active(plant) || zombie.mMindControlled || !CanRedirect(plant, zombie)) return true;
        bool center = PotBoosts.HasSupport(plant, FusionPlant.GarlicPot);
        var garlic = plant.mBoard.AddPlant(plant.mPlantCol, plant.mRow, SeedType.Garlic, SeedType.None);
        if (garlic == null || garlic.mDead || !garlic.mIsOnBoard) return true;
        // Source EatEffect replaces the plant with a fresh ordinary Garlic,
        // rather than applying a bite to the old plant or preserving its HP.
        garlic.mPlantHealth = garlic.mPlantMaxHealth = 300;
        PlayerPlantingScope.Consume(plant);
        zombie.StartMindControlled();
        Redirect(zombie);
        if (center) zombie.mTargetRow = PotDefense.GarlicDirection;
        return true;
    }

    internal static Zombie? FlyTarget(Plant plant)
    {
        if (!ChomperFusions.Active(plant)) return null;
        Zombie? result = null;
        float nearest = float.MaxValue;
        float start = plant.mX + plant.mWidth / 2f + Unit;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow || zombie.mBoard?.Pointer != plant.mBoard.Pointer
                || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
                || zombie.mZombieType is ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target
                    or ZombieType.Zamboni or ZombieType.Catapult or ZombieType.Bungee) continue;
            if (!zombie.EffectedByDamage(DamageRangeFlags.Ground | DamageRangeFlags.Flying
                | DamageRangeFlags.OffGround | DamageRangeFlags.Dog)) continue;
            var rect = zombie.GetZombieRect();
            if (rect.xMax < start || rect.xMin > start + Unit || rect.xMin >= nearest) continue;
            nearest = rect.xMin;
            result = zombie;
        }
        return result;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SupportInitializePatch
{
    internal static void Postfix(Plant __instance) => SupportFusions.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SupportTickPatch
{
    internal static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (!FusionPlant.IsSupportFusion(identity) || identity == FusionPlant.Flytrapper) return true;
        SupportFusions.Tick(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class SolarfruitNativeShooterPatch
{
    internal static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.Solarfruit;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class FalloutNativeSpecialPatch
{
    internal static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.FalloutBlover;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.MouseDown))]
internal static class SolarfruitClickPatch
{
    internal static bool Prefix(Plant __instance, int theClickCount, int playerIndex)
    {
        if (FusionPlant.From(__instance) != FusionPlant.Solarfruit || theClickCount < 0 || playerIndex != 0
            || __instance.mBoard.mCursorObject.CursorType != CursorType.Normal
            || LawnGloveMod.GetCarriedPlant(__instance.mBoard) != null) return true;
        SupportFusions.ShootSolar(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class HypnoGarlicBitePatch
{
    internal static bool Prefix(Zombie __instance, Plant thePlant) =>
        thePlant == null || !SupportFusions.BiteGarlic(thePlant, __instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
internal static class FlytrapperTargetPatch
{
    internal static bool Prefix(Plant __instance, ref Zombie __result)
    {
        if (FusionPlant.From(__instance) != FusionPlant.Flytrapper) return true;
        __result = SupportFusions.FlyTarget(__instance)!;
        return false;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.CanTargetPlant))]
internal static class FlytrapperFlyingPatch
{
    internal static void Postfix(Plant thePlant, ref bool __result)
    {
        if (thePlant != null && FusionPlant.From(thePlant) == FusionPlant.Flytrapper) __result = false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class SolarfruitImpactPatch
{
    [ThreadStatic] internal static Projectile? Current;
    [ThreadStatic] internal static Zombie? Victim;
    [ThreadStatic] internal static uint VictimId;
    [ThreadStatic] internal static bool Pending;
    internal static void Prefix(Projectile __instance, Zombie theZombie,
        out (Projectile? Shot, Zombie? Zombie, uint Id, bool Pending) __state)
    {
        __state = (Current, Victim, VictimId, Pending);
        Current = SupportFusions.IsSolarShot(__instance) && !__instance.mDead ? __instance : null;
        Victim = theZombie;
        VictimId = theZombie?.DataID ?? 0;
        Pending = Current != null;
    }
    internal static void Finalizer((Projectile? Shot, Zombie? Zombie, uint Id, bool Pending) __state)
    { Current = __state.Shot; Victim = __state.Zombie; VictimId = __state.Id; Pending = __state.Pending; }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class SolarfruitDamagePatch
{
    [HarmonyPriority(Priority.First)]
    internal static void Prefix(Zombie __instance, ref int theDamage)
    {
        if (!SolarfruitImpactPatch.Pending || SolarfruitImpactPatch.Current is not { } shot
            || SolarfruitImpactPatch.Victim?.Pointer != __instance.Pointer
            || SolarfruitImpactPatch.VictimId != __instance.DataID
            || shot.mBoard?.Pointer != __instance.mBoard?.Pointer) return;
        SolarfruitImpactPatch.Pending = false;
        theDamage = 100;
    }
}
