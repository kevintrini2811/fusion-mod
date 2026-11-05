using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Native Squash owns jumping, the landing event, removal and all save fields.
// Only acquisition range and the extra landing effects belong to the variants.
internal static class SquashFusions
{
    internal sealed record Landing(Board Board, uint Id, FusionPlant Identity,
        int Col, int Row, float X, float Y, int Order)
    { internal bool Swallowed; }
    [ThreadStatic] internal static Landing? CurrentLanding;

    internal static bool Active(Plant plant) => plant.mBoard != null && !plant.mBoard.mPaused
        && plant.mBoard.Pointer != CherryBombPlants.DisposingBoard && !plant.mDead
        && plant.mIsOnBoard && !plant.mSquished
        && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee
        && plant.mApp != null && plant.mApp.GameScene == GameScenes.Playing
        && plant.mApp.GameMode != GameMode.ChallengeZenGarden;

    internal static bool Eligible(Board board, Zombie zombie, DamageRangeFlags range) =>
        zombie.mBoard?.Pointer == board.Pointer && !zombie.mDead && !zombie.IsDeadOrDying()
        && !zombie.mMindControlled && zombie.EffectedByDamage(range);

    internal static Zombie? FindTarget(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        bool extended = identity == FusionPlant.CherrySquash || identity == FusionPlant.SpicySquash || identity == FusionPlant.DoomSquash;
        float x = plant.mX + plant.mWidth / 2f;
        // Almanac acquisition: half a cell behind / one ahead, or two behind
        // / three ahead. Native rectangles replace the source 2D colliders.
        float left = x - (extended ? 160 : identity == FusionPlant.Jawbreaker ? 0 : 40),
            right = x + (identity == FusionPlant.Jawbreaker ? 4f / 1.35f * 80 : extended ? 240 : 80);
        if (identity == FusionPlant.PotatoMash)
        {
            // PotatoSquash.Range is a 4x4 box centered on the native tile.
            left = x - 2 * ScaredyFusions.SourceUnit;
            right = x + 2 * ScaredyFusions.SourceUnit;
        }
        if (identity == FusionPlant.DoomSquash)
        {
            // Range=(8,8), centered two source world units ahead.
            left = x - 2f / 1.35f * 80;
            right = x + 6f / 1.35f * 80;
        }
        Zombie? target = null;
        float nearest = float.MaxValue;
        var zombies = plant.mBoard.m_zombies;
        var range = plant.GetDamageRangeFlags(PlantWeapon.Primary);
        if (identity == FusionPlant.Splash) range |= DamageRangeFlags.Submerged;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow || !Eligible(plant.mBoard, zombie, range)
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target) continue;
            var rect = zombie.GetZombieRect();
            if (rect.xMax < left || rect.xMin > right) continue;
            if (identity == FusionPlant.Jawbreaker && plant.mBoard.PixelToGridX(
                (rect.xMin + rect.xMax) / 2, zombie.mPosY) <= plant.mPlantCol) continue;
            float distance = Math.Abs((rect.xMin + rect.xMax) / 2 - x);
            if (distance >= nearest) continue;
            nearest = distance;
            target = zombie;
        }
        return target;
    }

    internal static void CrushSubmerged(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.Splash || !Active(plant)) return;
        // Leave the native leaf GetDamageRangeFlags unpatched: detouring it
        // crashes native Cactus targeting even when the postfix changes nothing.
        // Native DoSquashDamage still owns sound and ordinary recipients. Add
        // only underwater recipients excluded by its original damage range.
        var board = plant.mBoard;
        var range = plant.GetDamageRangeFlags(PlantWeapon.Primary);
        var attack = plant.GetPlantAttackRect(PlantWeapon.Primary);
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            if (board.m_zombies[i] is not { } zombie || zombie.mRow != plant.mRow
                || !Eligible(board, zombie, DamageRangeFlags.Submerged)
                || zombie.EffectedByDamage(range)) continue;
            var body = zombie.GetZombieRect();
            if (Common.GetRectOverlap(ref attack, ref body) > 0) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && Eligible(board, zombie, DamageRangeFlags.Submerged)
                && !zombie.EffectedByDamage(range))
                zombie.TakeDamage(1800, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
    }

    internal static Landing Capture(Plant plant) => new(plant.mBoard, plant.DataID,
        FusionPlant.From(plant), plant.mPlantCol, plant.mRow,
        plant.mX + plant.mWidth / 2f, plant.mY + plant.mHeight / 2f, plant.mRenderOrder);

    internal static (string Clip, float Time) Animation(Plant plant) => plant.mState switch
    {
        PlantState.SquashLook => (plant.mTargetX < plant.mX ? "lookleft" : "lookright",
            Math.Clamp((80 - plant.mStateCountdown) / 80f, 0, 1) * 1.49999f),
        PlantState.SquashPreLaunch => ("jump", Math.Clamp((45 - plant.mStateCountdown) / 45f, 0, 1) * .55f),
        PlantState.SquashRising => ("jump", .55f + Math.Clamp((50 - plant.mStateCountdown) / 50f, 0, 1) * .61665f),
        PlantState.SquashFalling => ("jumppop", Math.Clamp((10 - plant.mStateCountdown) / 15f, 0, .66666f)),
        PlantState.SquashDoneFalling => ("jumppop", .66667f
            + Math.Clamp((100 - plant.mStateCountdown) / 100f, 0, 1) * .83332f),
        _ => ("idle", (plant.mBoard.mMainCounter % 200) * .01f)
    };

    internal static void AfterLanding(Landing landing)
    {
        if (landing.Identity == FusionPlant.SolarSquash || landing.Identity == FusionPlant.Jawbreaker
            || landing.Identity == FusionPlant.Splash || landing.Identity == FusionPlant.PotatoMash) return;
        if (landing.Identity == FusionPlant.DoomSquash)
        {
            DoomEffects.Explode(landing.Board, landing.Board.PixelToGridX(landing.X, landing.Y),
                landing.Row, landing.X, landing.Y, landing.Order);
            return;
        }
        if (landing.Identity == FusionPlant.StarrySquash)
        {
            // StarSquash.AttackZombie calls the ordinary central crush, then
            // four radius-one circles offset by (+/-1.5, +/-0.5) source cells.
            foreach (float dx in new[] { -120f, 120f })
                foreach (float dy in new[] { -40f, 40f })
                    CrushCircle(landing, landing.X + dx, landing.Y + dy);
            return;
        }
        if (landing.Identity == FusionPlant.PeaSquash)
        {
            RetaliationProjectiles.Burst(landing);
            return;
        }
        if (landing.Identity == FusionPlant.HypnoSquash || landing.Identity == FusionPlant.IronSquash) return;
        if (landing.Identity == FusionPlant.CherrySquash)
        {
            CherryBombPlants.Explode(new(landing.Board, landing.Identity, landing.Id,
                landing.Col, landing.Row, landing.X, landing.Y, landing.Order));
            return;
        }
        ElementalBlast(landing, landing.Identity == FusionPlant.IcySquash);
    }

    internal static void ElementalBlast(Landing landing, bool ice)
    {
        if (!ice) ElementalNuts.Heal(landing.Board, true, landing.Row);
        var board = landing.Board;
        var range = DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
            | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Underground;
        var targets = new List<(Zombie Zombie, uint Id)>();
        var zombies = board.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || !Eligible(board, zombie, range)
                || Math.Abs(zombie.mRow - landing.Row) > (ice ? 1 : 0)) continue;
            var rect = zombie.GetZombieRect();
            float dx = landing.X - Math.Clamp(landing.X, rect.xMin, rect.xMax);
            float dy = landing.Y - Math.Clamp(landing.Y, rect.yMin, rect.yMax);
            // Actual IceSquash.AttackZombie uses radius 2, then row distance <= 1.
            if (ice && dx * dx + dy * dy > 160 * 160) continue;
            targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !Eligible(board, zombie, range)) continue;
            if (!ice) zombie.RemoveColdEffects();
            zombie.TakeDamage(ice ? 600 : 1800,
                DamageFlags.HitsShieldAndBody | (ice ? (DamageFlags)0 : DamageFlags.DoesntLeaveBody));
            if (!ice || zombie.DataID != id || !Eligible(board, zombie, range)) continue;
            // HitIceTrap owns immunity and the native freeze controller. Suppress
            // its independent 20 damage, as for the existing snow projectiles.
            var previous = SnowPea.CreatingIceTrap;
            SnowPea.CreatingIceTrap = zombie.Pointer;
            try { zombie.HitIceTrap(); }
            finally { SnowPea.CreatingIceTrap = previous; }
        }
        if (ice) board.mApp.AddTodParticle(landing.X, landing.Y, landing.Order, ParticleEffect.IceSparkle);
        else NativeCombatVisuals.Fire(board, landing.Row, landing.Order);
        if (!ice) board.mApp.SoundSystem.PlayFoley(FoleyType.JalapenoIgnite);
    }

    private static void CrushCircle(Landing landing, float x, float y)
    {
        var board = landing.Board;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || Math.Abs(zombie.mRow - landing.Row) > 1
                || !Eligible(board, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax);
            float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= 80 * 80) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && Eligible(board, zombie, DamageRangeFlags.Ground))
                zombie.TakeDamage(900, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
        board.mApp.AddTodParticle(x, y, landing.Order, ParticleEffect.Powie);
    }

    internal static bool ConvertCrush(Zombie zombie, int damage, DamageFlags flags)
    {
        var landing = CurrentLanding;
        if (landing?.Identity == FusionPlant.Jawbreaker && !landing.Swallowed && damage == 1800
            && flags == (DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody)
            && Eligible(landing.Board, zombie, DamageRangeFlags.Ground)
            && ChomperVariants.CanSwallow(zombie))
        {
            uint id = zombie.DataID;
            zombie.DieWithLoot();
            if (zombie.DataID == id && zombie.mDead)
            {
                landing.Swallowed = true;
                // The launch counter is unused by native Squash and serialized.
                var plants = landing.Board.m_plants;
                for (int i = 0; i < plants.Count; ++i)
                    if (plants[i] is { } plant && plant.DataID == landing.Id
                        && FusionPlant.From(plant) == FusionPlant.Jawbreaker) plant.mLaunchCounter = 1;
                return true;
            }
        }
        if (landing == null || landing.Identity != FusionPlant.HypnoSquash || damage != 1800
            || flags != (DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody)
            || !Eligible(landing.Board, zombie, DamageRangeFlags.Ground)
            || zombie.mZombieType is ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target) return false;
        // Ordinary source giants inherit SetMindControl too; do not invent a
        // giant immunity. Source-only bosses with overrides have no native port.
        zombie.StartMindControlled();
        return zombie.mMindControlled;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.FindSquashTarget))]
internal static class SquashFusionTargetPatch
{
    private static bool Prefix(Plant __instance, ref Zombie __result)
    {
        if (!FusionPlant.IsSquash(FusionPlant.From(__instance))) return true;
        __result = SquashFusions.Active(__instance) ? SquashFusions.FindTarget(__instance)! : null!;
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSquashDamage))]
internal static class SquashFusionLandingPatch
{
    internal static void Prefix(Plant __instance, out SquashFusions.Landing? __state)
    {
        __state = SquashFusions.CurrentLanding;
        SquashFusions.CurrentLanding = FusionPlant.IsSquash(FusionPlant.From(__instance))
            && SquashFusions.Active(__instance) ? SquashFusions.Capture(__instance) : null;
        SquashFusions.CrushSubmerged(__instance);
    }

    internal static void Postfix(Plant __instance)
    {
        var landing = SquashFusions.CurrentLanding;
        if (landing != null && __instance.DataID == landing.Id
            && __instance.mBoard?.Pointer == landing.Board.Pointer)
            SquashFusions.AfterLanding(landing);
    }

    internal static void Finalizer(SquashFusions.Landing? __state) => SquashFusions.CurrentLanding = __state;
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class HypnoSquashCrushPatch
{
    internal static bool Prefix(Zombie __instance, ref int theDamage, DamageFlags theDamageFlags)
    {
        if (SquashFusions.CurrentLanding?.Identity == FusionPlant.StarrySquash && theDamage == 1800
            && theDamageFlags == (DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody))
            theDamage = 900;
        if (SquashFusions.CurrentLanding?.Identity == FusionPlant.IronSquash && theDamage == 1800
            && theDamageFlags == (DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody))
            theDamage = 3240;
        return !SquashFusions.ConvertCrush(__instance, theDamage, theDamageFlags);
    }
}
