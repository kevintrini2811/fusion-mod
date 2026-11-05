using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppSource.Controllers;

namespace FusionMod;

// Potato Mine owns the low target/terrain category. Serialized native fields
// store the variant ability timers and permanent damage flags.
internal static class MineFusions
{
    [ThreadStatic] internal static (IntPtr Board, uint Id)? CurrentDetonation;
    internal const int RiseTicks = 84, ShootTicks = 100;
    internal const DamageRangeFlags MineTargets = DamageRangeFlags.Ground | DamageRangeFlags.Submerged
        | DamageRangeFlags.Dog | DamageRangeFlags.Underground;

    internal static int RiseDuration(FusionPlant identity) => identity == FusionPlant.ThreetatoMine ? ThreetatoMine.RiseTicks : identity == FusionPlant.IceMine ? 100 : RiseTicks;

    internal static bool Active(Plant plant) => plant.mBoard != null && !plant.mBoard.mPaused
        && plant.mBoard.Pointer != CherryBombPlants.DisposingBoard && !plant.mDead
        && plant.mIsOnBoard && !plant.mSquished && !plant.NotOnGround()
        && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee
        && plant.mApp != null && plant.mApp.GameScene == GameScenes.Playing
        && plant.mApp.GameMode != GameMode.ChallengeZenGarden;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsMine(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.MineNut ? 4000 : 300;
        plant.mState = identity == FusionPlant.MineNut ? PlantState.PotatoArmed : PlantState.Notready;
        plant.mStateCountdown = identity == FusionPlant.MineNut ? 0 : 1400 + plant.mBoard.Rand(201);
        plant.mLaunchCounter = identity == FusionPlant.SolarMine ? 1500 : identity == FusionPlant.PeatatoMine ? 150 : 0;
        plant.mShootingCounter = 0;
        if (identity == FusionPlant.ThreetatoMine)
        { plant.mTargetX = plant.mTargetY = plant.mStartRow = plant.mLaunchRate = 0; plant.mStateCountdown = 1500; }
        if ((identity == FusionPlant.LandMine || identity == FusionPlant.SeaMine))
        {
            if (identity == FusionPlant.SeaMine) plant.mIsAsleep = false;
            // SeaMine is ready immediately; its attribute timer grows it twice.
            plant.mState = PlantState.PotatoArmed;
            plant.mStateCountdown = 2900 + plant.mBoard.Rand(201);
            plant.mStartRow = plant.mTargetX = 0;
        }
    }

    internal static void GrowImmediately(Plant plant)
    {
        // This ready mine has a separate growth clock, not a buried fuse.
        if ((FusionPlant.From(plant) == FusionPlant.LandMine || FusionPlant.From(plant) == FusionPlant.SeaMine)) return;
        plant.mStateCountdown = 0;
        if (FusionPlant.From(plant) == (FusionPlant)SeedType.Potatomine)
        {
            // Native rise completion replaces the buried controller with the
            // armed controller and attaches its light. Setting Armed alone
            // would leave the underground artwork/controller behind.
            plant.mState = PlantState.PotatoRising;
            plant.mController.SetLoopCount(1, CharacterAnimationTrack.Body);
            plant.UpdatePotato();
        }
        else
        {
            bool rising = plant.mState != PlantState.PotatoArmed;
            plant.mState = PlantState.PotatoArmed;
            if (rising && FusionPlant.From(plant) == FusionPlant.IceMine)
                IceMineBlasts.Start(plant.mBoard, plant.mPlantCol, plant.mRow, plant.mRenderOrder);
        }
    }

    internal static void OnFusion(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        int refund = identity == FusionPlant.PeatatoMine ? 50 : identity == FusionPlant.SolarMine ? 25 : 0;
        if (refund != 0) plant.mBoard.AddSunMoney(refund, 0);
    }

    internal static int FlashInterval(float distance) => distance > 480 ? 600 : distance < 80 ? 20
        : (int)(distance * .625f);

    internal static int ShotInterval(float distance) =>
        (int)MathF.Round((1.5f - Math.Clamp((480 - distance) / 400, 0, 1)) * 100);

    internal static Zombie? Nearest(Plant plant, bool contact)
    {
        Zombie? nearest = null;
        float closest = float.MaxValue, center = plant.mX + plant.mWidth / 2f;
        float contactReach = (FusionPlant.From(plant) == FusionPlant.LandMine || FusionPlant.From(plant) == FusionPlant.SeaMine) ? 24 * LandMineScale(plant) : 25;
        var zombies = plant.mBoard.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
                || zombie.mRow != plant.mRow || !zombie.EffectedByDamage(contact ? MineTargets : DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            float distance = rect.xMin - center;
            if (contact ? rect.xMax < center - contactReach || distance > contactReach : rect.xMax < center || distance > 800) continue;
            if (distance < closest) { closest = distance; nearest = zombie; }
        }
        return nearest;
    }

    internal static void Update(Plant plant)
    {
        if (!Active(plant)) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.ThreetatoMine) { ThreetatoMine.Tick(plant); return; }
        if ((identity == FusionPlant.LandMine || identity == FusionPlant.SeaMine))
        {
            UpdateLandMine(plant);
            return;
        }
        if (identity == FusionPlant.MineNut)
        {
            // Source Broken chooses the current band, not every skipped band.
            int bit = plant.mPlantHealth < plant.mPlantMaxHealth / 3 ? 2
                : plant.mPlantHealth < (long)plant.mPlantMaxHealth * 2 / 3
                    && plant.mPlantHealth > plant.mPlantMaxHealth / 3 ? 1 : 0;
            if (bit != 0 && (plant.mShootingCounter & bit) == 0)
            {
                plant.mShootingCounter |= bit;
                Blast(plant.mBoard, plant.mX + plant.mWidth / 2f,
                    plant.mY + plant.mHeight / 2f, 80, 500, plant.mRenderOrder);
                plant.mLaunchCounter = 100;
            }
            if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
            return;
        }
        // JalaMine derives from source Plant: edible, no contact fuse or rise.
        if (identity == FusionPlant.ChipMine) return;
        if (plant.mShootingCounter > 0) --plant.mShootingCounter;
        if (plant.mStateCountdown > 0) --plant.mStateCountdown;
        if (plant.mState == PlantState.Notready && plant.mStateCountdown == 0)
        { plant.mState = PlantState.PotatoRising; plant.mStateCountdown = RiseDuration(identity); }
        else if (plant.mState == PlantState.PotatoRising && plant.mStateCountdown == 0)
        {
            plant.mState = PlantState.PotatoArmed;
        }
        if (identity == FusionPlant.IceMine && plant.mState == PlantState.PotatoRising
            && plant.mStateCountdown == 50)
            IceMineBlasts.Start(plant.mBoard, plant.mPlantCol, plant.mRow, plant.mRenderOrder);
        if (plant.mState == PlantState.PotatoArmed && Nearest(plant, true) != null)
        {
            plant.mState = PlantState.PotatoMashed;
            if (!FusionPlant.IsBurstMine(identity))
                Blast(plant.mBoard, plant.mX + plant.mWidth / 2f,
                    plant.mY + plant.mHeight / 2f, 80, 1800, plant.mRenderOrder);
            var previous = CurrentDetonation;
            CurrentDetonation = (plant.mBoard.Pointer, plant.DataID);
            try { plant.Die(); }
            finally { CurrentDetonation = previous; }
            return;
        }
        if (FusionPlant.IsBurstMine(identity))
        {
            if (plant.mState == PlantState.PotatoArmed)
            {
                var target = Nearest(plant, false);
                float distance = target == null ? 1200 : target.mPosX - plant.mX - plant.mWidth / 2f;
                if (++plant.mLaunchCounter > FlashInterval(distance))
                {
                    plant.mLaunchCounter = 0;
                    plant.mShootingCounter = identity == FusionPlant.IceMine ? 20 : 8;
                }
            }
            return;
        }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (identity == FusionPlant.SolarMine)
        {
            if (plant.mApp.GameMode is >= GameMode.PuzzleIZombie1 and <= GameMode.PuzzleIZombieEndless) return;
            if (plant.mLaunchCounter > 0) return;
            plant.mBoard.AddCoin(plant.mX, plant.mY, CoinType.Sun, CoinMotion.FromPlant);
            plant.mLaunchCounter = (15 + plant.mBoard.Rand(5) - 2) * 100;
            plant.mShootingCounter = 100;
        }
        else
        {
            var target = Nearest(plant, false);
            if (target == null) return;
            int interval = ShotInterval(target.mPosX - (plant.mX + plant.mWidth / 2f));
            plant.mLaunchCounter = Math.Min(plant.mLaunchCounter, interval);
            if (plant.mLaunchCounter > 0) return;
            var muzzle = ImportedPeaVisuals.PeatatoMuzzle(plant);
            PotatoProjectiles.Spawn(plant.mBoard, muzzle.X, muzzle.Y, plant.mRow, plant.mRenderOrder, 0);
            plant.mLaunchCounter = interval;
            plant.mShootingCounter = ShootTicks;
        }
    }

    internal static float LandMineScale(Plant plant) =>
        (0.15f + 0.05f * Math.Clamp(plant.mStartRow, 0, 2)
            - 0.002f * Math.Clamp(plant.mTargetX, 0, 25)) / 0.15f;

    private static void UpdateLandMine(Plant plant)
    {
        if (plant.mShootingCounter > 0) --plant.mShootingCounter;
        if (plant.mTargetX > 0) --plant.mTargetX;
        if (plant.mStartRow < 2 && --plant.mStateCountdown <= 0)
        {
            ++plant.mStartRow;
            plant.mTargetX = 25;
            plant.mStateCountdown = plant.mStartRow < 2 ? 2900 + plant.mBoard.Rand(201) : 0;
        }
        if (++plant.mLaunchCounter > 200)
        {
            plant.mLaunchCounter = 0;
            plant.mShootingCounter = 10;
        }
        if (Nearest(plant, true) == null) return;
        plant.mState = PlantState.PotatoMashed;
        var previous = CurrentDetonation;
        CurrentDetonation = (plant.mBoard.Pointer, plant.DataID);
        try { plant.Die(); }
        finally { CurrentDetonation = previous; }
    }

    internal static void Blast(Board board, float x, float y, float radius, int damage, int order, bool hypno = false, int? row = null, int rowRange = 0, bool ash = false)
    {
        var targets = new List<(Zombie Zombie, uint Id)>();
        var zombies = board.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
                || (row.HasValue && Math.Abs(zombie.mRow - row.Value) > rowRange)
                || !zombie.EffectedByDamage(MineTargets)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= radius * radius) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
            if (!zombie.mDead && zombie.DataID == id && zombie.mBoard?.Pointer == board.Pointer && !zombie.IsDeadOrDying())
            {
                if (hypno && zombie.mZombieType is not (ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target))
                    zombie.StartMindControlled();
                if (zombie.DataID == id && zombie.mBoard?.Pointer == board.Pointer
                    && !zombie.mDead && !zombie.IsDeadOrDying() && !zombie.mMindControlled)
                {
                    if (ash) DoomEffects.AshDamage(zombie, damage);
                    else zombie.TakeDamage(damage, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
                }
            }
        board.mApp.AddTodParticle(x, y, order, ParticleEffect.PotatoMine);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Explosion);
    }

    internal static IEnumerable<(int Col, int Row)> ScatterCells(CherryBombPlants.Death death, bool rowOnly)
    {
        if (rowOnly)
        {
            // JalaMine.OnDie alternates +1,-1,+2,-2; origin is excluded.
            for (int distance = 1; distance <= 8; ++distance)
                foreach (int direction in new[] { 1, -1 })
                {
                    int col = death.Col + distance * direction;
                    if (col >= 0 && col < 9) yield return (col, death.Row);
                }
        }
        else
            for (int row = Math.Max(0, death.Row - 1); row <= Math.Min(death.Board.GetNumRows() - 1, death.Row + 1); ++row)
                for (int col = Math.Max(0, death.Col - 1); col <= Math.Min(8, death.Col + 1); ++col)
                    yield return (col, row);
    }

    internal static void ScatterMines(CherryBombPlants.Death death, bool rowOnly = false)
    {
        var board = death.Board;
        var context = PlayerPlantingScope.Current;
        PlayerPlantingScope.Current = null;
        try
        {
            int created = 0;
            foreach (var (col, row) in ScatterCells(death, rowOnly))
            {
                var occupant = board.GetPlantsOnLawn(col, row).NormalPlant;
                if (occupant != null && !occupant.mDead) continue;
                if (FusionService.CheckNativePlacement(board, col, row, SeedType.Potatomine) != PlantingReason.Ok) continue;
                var plant = board.AddPlant(col, row, SeedType.Potatomine, SeedType.None);
                if (plant == null || plant.mDead) continue;
                plant.mState = PlantState.Notready;
                plant.mStateCountdown = 500;
                if (++created == 7 && rowOnly) break;
            }
        }
        finally { PlayerPlantingScope.Current = context; }
    }

    internal static void OnDeath(CherryBombPlants.Death death)
    {
        if ((death.Identity == FusionPlant.LandMine || death.Identity == FusionPlant.SeaMine))
        {
            if (CurrentDetonation == (death.Board.Pointer, death.Id))
                Blast(death.Board, death.X, death.Y, death.Stage >= 2 ? 160 : 80,
                    death.Stage == 0 ? 900 : 1800, death.Order, row: death.Row, rowRange: death.Stage >= 2 ? 1 : 0);
        }
        else if (FusionPlant.IsBurstMine(death.Identity))
        {
            // IceMine's ordinary 1800 explosion occurs on armed contact/crash;
            // its ice burst accompanies that explosion, not an underground bite.
            if (death.Identity == FusionPlant.ChipMine || CurrentDetonation == (death.Board.Pointer, death.Id))
            {
                if (death.Identity == FusionPlant.IceMine)
                    IceMineBlasts.Start(death.Board, death.Col, death.Row, death.Order);
                Blast(death.Board, death.X, death.Y, 80, 1800, death.Order,
                    death.Identity == FusionPlant.HypnoMine,
                    death.Identity == FusionPlant.HypnoMine ? death.Row : null);
            }
            if (death.Identity == FusionPlant.ChipMine) ScatterMines(death, true);
        }
        else if (death.Identity == FusionPlant.MineNut)
            Blast(death.Board, death.X, death.Y, 80, 1800, death.Order);
        else if (death.Identity == FusionPlant.SolarMine)
        {
            if (CurrentDetonation != (death.Board.Pointer, death.Id))
                Blast(death.Board, death.X, death.Y, 80, 1800, death.Order);
            death.Board.AddCoin(death.X, death.Y, CoinType.Sun, CoinMotion.FromPlant);
            death.Board.AddCoin(death.X, death.Y, CoinType.Sun, CoinMotion.FromPlant);
        }
        else if (death.Identity == FusionPlant.PeatatoMine)
            for (int angle = 0; angle < 360; angle += 10)
                PotatoProjectiles.Spawn(death.Board, death.X, death.Y, death.Row, death.Order, angle);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class MineInitializePatch
{
    private static void Postfix(Plant __instance) => MineFusions.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class MineAbilitiesPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsMine(FusionPlant.From(__instance))) return true;
        MineFusions.Update(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdatePotato))]
internal static class MinePotatoPatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsMine(FusionPlant.From(__instance));
}

// EatPlant otherwise returns before subtracting health whenever a Potato Mine
// is not Notready. Keep its low projectile category and identity intact; expose
// the edible state only during the native bite, including its native death path.
[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class MineBiteScope
{
    internal sealed record SavedState(Plant Plant, uint Id, IntPtr Board, PlantState State);

    internal static void Prefix(Plant thePlant, out SavedState? __state)
    {
        __state = null;
        if (thePlant == null || !(FusionPlant.IsMine(FusionPlant.From(thePlant))
                || FusionPlant.From(thePlant) == FusionPlant.CraterTot || UtilityMines.Managed(FusionPlant.From(thePlant)) || TreasureMines.Owns(thePlant))
            || !MineFusions.Active(thePlant) || thePlant.mState == PlantState.Notready) return;
        __state = new(thePlant, thePlant.DataID, thePlant.mBoard.Pointer, thePlant.mState);
        thePlant.mState = PlantState.Notready;
    }

    internal static void Finalizer(SavedState? __state)
    {
        if (__state != null && __state.Plant.DataID == __state.Id
            && __state.Plant.mBoard?.Pointer == __state.Board)
            __state.Plant.mState = __state.State;
    }
}

// Source OnCrash detonates armed PotatoMine derivatives. Native Squish calls
// Die; preserve this intent separately from ordinary damage and shoveling.
[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class BurstMineCrushScope
{
    internal static void Prefix(Plant __instance, out (IntPtr Board, uint Id)? __state)
    {
        __state = MineFusions.CurrentDetonation;
        var identity = FusionPlant.From(__instance);
        if ((identity == FusionPlant.IceMine || identity == FusionPlant.HypnoMine || (identity == FusionPlant.LandMine || identity == FusionPlant.SeaMine))
            && MineFusions.Active(__instance) && __instance.mState == PlantState.PotatoArmed)
            MineFusions.CurrentDetonation = (__instance.mBoard.Pointer, __instance.DataID);
    }
    internal static void Finalizer((IntPtr Board, uint Id)? __state) => MineFusions.CurrentDetonation = __state;
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class ArmedMineContactPatch
{
    [HarmonyPriority(Priority.First + 200)]
    private static bool Prefix(Plant thePlant)
    {
        if (thePlant == null || !MineFusions.Active(thePlant) || thePlant.mState != PlantState.PotatoArmed) return true;
        var identity = FusionPlant.From(thePlant);
        if (identity == FusionPlant.CraterTot) { DoomFusions.Detonate(thePlant); return false; }
        if (identity == FusionPlant.TangleMine) { thePlant.UpdateTanglekelp(); return false; }
        return true;
    }
}
