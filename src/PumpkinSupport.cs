using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Pumpkin owns the accessory layer. Its otherwise unused, serialized
// shooter fields own charge, heat and the independent copied attack schedule.
internal static class PumpkinSupport
{
    internal const int PodClipTicks = 42;
    internal const int PodEventTick = 25;
    [ThreadStatic] private static bool releasingIce;
    [ThreadStatic] private static IntPtr releasingFire;
    [ThreadStatic] internal static List<Projectile>? CopiedShots;
    internal static bool Owns(FusionPlant identity) => identity == FusionPlant.PumpkinPod
        || identity == FusionPlant.MelonShell || identity == FusionPlant.CryoPumpkin
        || identity == FusionPlant.PyroPumpkin || identity == FusionPlant.Chompkin
        || identity == FusionPlant.SunnyPumpkin || identity == FusionPlant.PumpkinMine;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!Owns(identity)) return;
        plant.mTargetX = plant.mTargetY = plant.mStartRow = plant.mShootingCounter = 0;
        plant.mTargetZombieID = 0;
        plant.mLaunchCounter = identity == FusionPlant.PumpkinPod ? 300
            : identity == FusionPlant.CryoPumpkin ? 500
            : identity == FusionPlant.Chompkin ? 100 : identity == FusionPlant.SunnyPumpkin ? 2500 : 0;
        if (identity == FusionPlant.Chompkin) plant.mTargetY = 750;
    }

    internal static Plant? Shell(Plant plant, FusionPlant identity)
    {
        var shell = plant.mBoard.GetPlantsOnLawn(plant.mPlantCol, plant.mRow).PumpkinPlant;
        return shell != null && FusionPlant.From(shell) == identity && ChomperFusions.Active(shell) ? shell : null;
    }

    internal static bool Pea(FusionPlant identity) => ShootingPots.Pea(identity)
        || identity == FusionPlant.Starshooter;

    internal static int Count(Plant shooter) => shooter.mSeedType == SeedType.Gatlingpea ? 4
        : shooter.mSeedType == SeedType.Splitpea ? 3 : shooter.mSeedType == SeedType.Repeater ? 2 : 1;

    private static Plant? Shooter(Plant shell, bool saved)
    {
        for (int i = 0; i < shell.mBoard.m_plants.Count; ++i)
        {
            var plant = shell.mBoard.m_plants[i];
            if (plant == null || plant.mPlantCol != shell.mPlantCol || plant.mRow != shell.mRow
                || !ChomperFusions.Active(plant) || plant.mIsAsleep || !Pea(FusionPlant.From(plant))
                || saved && plant.DataID != (uint)shell.mTargetZombieID) continue;
            return plant;
        }
        return null;
    }

    private static bool HasTarget(Plant plant) => FusionPlant.From(plant) == FusionPlant.Starshooter
        ? StarFusions.HasTarget(plant) : plant.FindTargetZombie(plant.mRow, PlantWeapon.Primary) != null
            || plant.mSeedType == SeedType.Splitpea && plant.FindTargetZombie(plant.mRow, PlantWeapon.Secondary) != null;

    internal static void Tick(Plant shell)
    {
        if (!Owns(FusionPlant.From(shell)) || !ChomperFusions.Active(shell)) return;
        if (!FusionTime.SourceStep(shell, typeof(PumpkinSupport))) return;
        var identity = FusionPlant.From(shell);
        if (identity == FusionPlant.Chompkin) { TickChompkin(shell); return; }
        if (identity == FusionPlant.PumpkinMine) { CheckMine(shell); return; }
        if (identity == FusionPlant.SunnyPumpkin)
        {
            if (shell.mStartRow > 0)
            {
                if (--shell.mStartRow == 0)
                {
                    SolarChomperPlants.Sun(shell);
                    foreach (var plant in Cell(shell).ToArray()) PlantShields.Add(plant, 50);
                }
            }
            if (shell.mLaunchCounter > 0 && --shell.mLaunchCounter > 0) return;
            shell.mLaunchCounter = 2375 + shell.mBoard.Rand(251);
            shell.mStartRow = 50;
            return;
        }
        if (identity == FusionPlant.CryoPumpkin)
        {
            if (shell.mLaunchCounter > 0 && --shell.mLaunchCounter > 0) return;
            shell.mLaunchCounter = 500;
            if (shell.mTargetX > 1) ReleaseIce(shell);
            return;
        }
        if (identity != FusionPlant.PumpkinPod) return;
        if (shell.mLaunchCounter > 0) --shell.mLaunchCounter;
        if (shell.mStartRow == 0)
        {
            if (shell.mLaunchCounter > 0) return;
            var shooter = Shooter(shell, false);
            if (shooter == null || !HasTarget(shooter)) return;
            shell.mTargetZombieID = (ZombieID)shooter.DataID;
            shell.mShootingCounter = Count(shooter);
            shell.mTargetY = shell.mShootingCounter;
            // PeaPumpkin.AttributeEvent applies a lower bound of three seconds.
            // The English claim of a six-second upper bound has no code branch.
            shell.mLaunchCounter = Math.Max(300, shooter.mLaunchRate * 2);
            shell.mStartRow = PodClipTicks;
        }
        if (PodClipTicks - shell.mStartRow == PodEventTick)
        {
            var shooter = Shooter(shell, true);
            if (shooter == null) { shell.mStartRow = shell.mShootingCounter = 0; return; }
            CopyShot(shell, shooter);
            --shell.mShootingCounter;
        }
        if (--shell.mStartRow == 0 && shell.mShootingCounter > 0) shell.mStartRow = PodClipTicks;
    }

    internal const int ChompTicks = 167;
    private static IEnumerable<Plant> Cell(Plant shell)
    {
        for (int i = 0; i < shell.mBoard.m_plants.Count; ++i)
            if (shell.mBoard.m_plants[i] is { } plant && plant.mPlantCol == shell.mPlantCol
                && plant.mRow == shell.mRow && ChomperFusions.Active(plant)) yield return plant;
    }
    private static Plant? Digesting(Plant shell) => Cell(shell).FirstOrDefault(plant =>
        plant.mSeedType == SeedType.Chomper
        && (plant.mState == PlantState.ChomperDigesting || FusionPlant.From(plant) == FusionPlant.Chompzilla)
        && plant.mStateCountdown > 50 && !plant.mIsAsleep);
    private static void TickChompkin(Plant shell)
    {
        // Production pauses at the three-bag cap; fertilizer changes only the
        // separate attribute clock. Both clocks and bag count survive loading.
        if (shell.mTargetX < 3 && --shell.mTargetY <= 0)
        { ++shell.mTargetX; shell.mTargetY = 4000; }
        if (shell.mStartRow > 0)
        {
            if (ChompTicks - shell.mStartRow == 50 && shell.mTargetX > 0
                && Digesting(shell) is { } target)
            {
                --shell.mTargetX;
                // Let the native digestion completion run, including each
                // variant's own reward; never overwrite its state or marker.
                target.mStateCountdown = 50;
                shell.mBoard.mApp.SoundSystem.PlayFoley(Il2CppReloaded.Services.FoleyType.Chomp);
            }
            --shell.mStartRow;
        }
        if (shell.mLaunchCounter > 0 && --shell.mLaunchCounter > 0) return;
        shell.mLaunchCounter = 100;
        if (shell.mStartRow == 0 && shell.mTargetX > 0 && Digesting(shell) != null)
            shell.mStartRow = ChompTicks;
    }
    internal static void CheckMine(Plant shell)
    {
        // Source ReplaceSprite remembers the current damaged band, even after
        // healing. A hit skipping the middle band earns only the lower blast.
        int band = shell.mPlantHealth < shell.mPlantMaxHealth / 3 ? 2
            : shell.mPlantHealth < shell.mPlantMaxHealth * 2L / 3
                && shell.mPlantHealth > shell.mPlantMaxHealth / 3 ? 1 : 0;
        if (band <= shell.mTargetX) return;
        shell.mTargetX = band;
        MineFusions.Blast(shell.mBoard, shell.mX + shell.mWidth / 2f,
            shell.mY + shell.mHeight / 2f, 1f * FumeVariants.Unit, 500, shell.mRenderOrder, row: shell.mRow);
    }

    internal static void CopyShot(Plant shell, Plant shooter)
    {
        // Preserve the real shooter's attack clock. Shot provenance, counters
        // such as Football Gatling's twelfth shot, and pot events remain its own.
        int launch = shooter.mLaunchCounter, shooting = shooter.mShootingCounter;
        var previous = CopiedShots;
        var shots = new List<Projectile>();
        CopiedShots = shots;
        try
        {
            if (FusionPlant.From(shooter) == FusionPlant.Starshooter) StarFusions.Fire(shooter);
            else
            {
                bool rear = shooter.mSeedType == SeedType.Splitpea && shell.mShootingCounter < shell.mTargetY;
                shooter.Fire(null!, shooter.mRow, rear ? PlantWeapon.Secondary : PlantWeapon.Primary);
            }
        }
        finally
        {
            CopiedShots = previous;
            shooter.mLaunchCounter = launch;
            shooter.mShootingCounter = shooting;
        }
        foreach (var shot in shots)
        {
            if (shot.mDead || shot.mBoard?.Pointer != shell.mBoard.Pointer) continue;
            // Same tile; move the shooter's muzzle to the original shell's
            // lower gun, without changing velocity, type or impact ownership.
            float dy = shell.mY + shell.mHeight / 2f - 10 - shot.mPosY;
            shot.mPosY += dy;
            shot.mShadowY += dy;
        }
    }

    internal static bool StoreIce(Board board, int col, int row)
    {
        if (releasingIce || board.mPaused || board.Pointer == CherryBombPlants.DisposingBoard) return false;
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var shell = board.m_plants[i];
            if (shell == null || FusionPlant.From(shell) != FusionPlant.CryoPumpkin
                || !ChomperFusions.Active(shell) || shell.mTargetX >= 3 || shell.mRow != row
                || col != shell.mPlantCol + 1) continue;
            ++shell.mTargetX;
            return true;
        }
        return false;
    }

    internal static void ReleaseIce(Plant shell)
    {
        if (shell.mTargetX <= 0) return;
        --shell.mTargetX; // Commit before callbacks can create another ice event.
        bool previous = releasingIce;
        releasingIce = true;
        try
        {
            IceInstantFusions.Ice(shell.mBoard, 400);
            ElementalNuts.Heal(shell.mBoard, false);
        }
        finally { releasingIce = previous; }
    }

    internal static void FireEnergy(Board board, int row, int damage)
    {
        if (board.mPaused || board.Pointer == CherryBombPlants.DisposingBoard) return;
        PhoenixProjectiles.IgniteRow(board, row);
        for (int i = 0; i < board.m_plants.Count; ++i)
            if (board.m_plants[i] is { } shell && shell.mRow == row
                && FusionPlant.From(shell) == FusionPlant.PyroPumpkin && ChomperFusions.Active(shell)
                && shell.Pointer != releasingFire)
                shell.mTargetX = (int)Math.Min(1000000, (long)shell.mTargetX + Math.Max(0, damage) / 10);
    }

    internal static void HeatedDeath(Board board, int row)
    {
        if (board.mPaused || board.Pointer == CherryBombPlants.DisposingBoard) return;
        for (int i = 0; i < board.m_plants.Count; ++i)
            if (board.m_plants[i] is { } shell && shell.mRow == row
                && FusionPlant.From(shell) == FusionPlant.PyroPumpkin && ChomperFusions.Active(shell)
                && shell.Pointer != releasingFire)
                // OnZombieDeath directly adds 60, unlike capped EatEnergy.
                shell.mTargetX = (int)Math.Min(int.MaxValue, (long)shell.mTargetX + 60);
    }

    internal static void ReleaseFire(Plant shell)
    {
        if (shell.mTargetX < 1800) return;
        int damage = shell.mTargetX;
        shell.mTargetX = 0;
        var previous = releasingFire;
        releasingFire = shell.Pointer;
        try
        {
            DoomEffects.Damage(shell.mBoard, shell.mRow, shell.mX, shell.mY, shell.mRenderOrder,
                fire: true, damage: damage);
            shell.mBoard.mIceTimer[shell.mRow] = 0;
            ElementalNuts.Heal(shell.mBoard, true, shell.mRow);
        }
        finally { releasingFire = previous; }

    }

    internal static bool Click(Board board, int x, int y, int clicks, int player)
    {
        if (clicks < 0 || player != 0 || board.mPaused || board.mCursorObject.CursorType != CursorType.Normal
            || board.mApp == null || board.mApp.GameScene != GameScenes.Playing
            || board.mApp.GameMode == GameMode.ChallengeZenGarden) return true;
        int col = board.PixelToGridX(x, y), row = board.PixelToGridY(x, y);
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return true;
        var shell = board.GetPlantsOnLawn(col, row).PumpkinPlant;
        if (shell == null || !ChomperFusions.Active(shell)) return true;
        var identity = FusionPlant.From(shell);
        if (identity == FusionPlant.CryoPumpkin) ReleaseIce(shell);
        else if (identity == FusionPlant.PyroPumpkin) ReleaseFire(shell);
        else return true;
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class PumpkinMineDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state)
    {
        __state = __runOriginal && FusionPlant.From(__instance) == FusionPlant.PumpkinMine
            && CherryBombPlants.CanTriggerDeath(__instance) && !__instance.NotOnGround()
            ? new(__instance.mBoard, FusionPlant.PumpkinMine, __instance.DataID,
                __instance.mPlantCol, __instance.mRow, __instance.mX + __instance.mWidth / 2f,
                __instance.mY + __instance.mHeight / 2f, __instance.mRenderOrder) : null;
    }
    private static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state == null || __instance.DataID != __state.Id || !__instance.mDead) return;
        MineFusions.Blast(__state.Board, __state.X, __state.Y, FumeVariants.Unit,
            1800, __state.Order, row: __state.Row);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PumpkinSupportInitializePatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Postfix(Plant __instance) => PumpkinSupport.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class PumpkinSupportTickPatch
{ internal static void Postfix(Plant __instance) => PumpkinSupport.Tick(__instance); }
[HarmonyPatch(typeof(Board), nameof(Board.MouseDown))]
internal static class PumpkinSupportClickPatch
{
    internal static bool Prefix(Board __instance, int x, int y, int theClickCount, int playerIndex) =>
        PumpkinSupport.Click(__instance, x, y, theClickCount, playerIndex);
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class PumpkinSupportBitePatch
{
    internal static void Prefix(Plant thePlant, out (Plant? Plant, uint Id, int Health, int Heat) __state) =>
        __state = (thePlant, thePlant?.DataID ?? 0, thePlant?.mPlantHealth ?? 0, thePlant?.mTargetX ?? 0);
    [HarmonyPriority(Priority.Last)]
    internal static void Postfix(Zombie __instance, (Plant? Plant, uint Id, int Health, int Heat) __state)
    {
        var plant = __state.Plant;
        if (plant == null || plant.DataID != __state.Id || plant.mPlantHealth >= __state.Health
            || plant.mBoard?.Pointer != __instance.mBoard?.Pointer || __instance.mDead
            || __instance.IsDeadOrDying() || __instance.mMindControlled || plant.mBoard!.mPaused) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.CryoPumpkin)
        {
            FusionStatuses.Chill(__instance, sourceTicks: 400);
            SnowPea.Hit(__instance, 5);
        }
        else if (identity == FusionPlant.PyroPumpkin)
        {
            float scale = __state.Health * 3L > plant.mPlantMaxHealth * 2L ? 1
                : __state.Health * 3L > plant.mPlantMaxHealth ? 2f / 3 : 1f / 3;
            __instance.TakeDamage((int)((20 + __state.Heat / 30f) * scale), DamageFlags.BypassesShield);
            Overheat.Apply(__instance);
        }
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class PyroPumpkinDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Plant __instance, bool __runOriginal, out (uint Id, int Heat) __state)
    {
        __state = __runOriginal && FusionPlant.From(__instance) == FusionPlant.PyroPumpkin
            && CherryBombPlants.CanTriggerDeath(__instance) && !__instance.NotOnGround()
            ? (__instance.DataID, __instance.mTargetX) : (0, 0);
    }
    internal static void Postfix(Plant __instance, (uint Id, int Heat) __state)
    {
        if (__state.Heat < 1800 || !__instance.mDead || __instance.DataID != __state.Id) return;
        __instance.mTargetX = __state.Heat;
        PumpkinSupport.ReleaseFire(__instance);
    }
}

// Both native ice entry points exist: DoSpecial contains an inlined loop.
[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class CryoPumpkinSpecialPatch
{
    [HarmonyPriority(Priority.First)]
    internal static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != (FusionPlant)SeedType.Iceshroom
            || !ChomperFusions.Active(__instance)
            || !PumpkinSupport.StoreIce(__instance.mBoard, __instance.mPlantCol, __instance.mRow)) return true;
        __instance.Die();
        return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.IceZombies))]
internal static class CryoPumpkinDirectIcePatch
{
    internal static bool Prefix(Plant __instance) => !PumpkinSupport.StoreIce(
        __instance.mBoard, __instance.mPlantCol, __instance.mRow);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.BurnRow))]
internal static class PyroPumpkinNativeFirePatch
{
    internal static void Postfix(Plant __instance, int theRow) => PumpkinSupport.FireEnergy(__instance.mBoard, theRow, 1800);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class PyroPumpkinSpecialFirePatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Plant __instance, bool __runOriginal, out (Board? Board, int Row) __state) =>
        __state = __runOriginal && FusionPlant.From(__instance) == (FusionPlant)SeedType.Jalapeno
            && ChomperFusions.Active(__instance) ? (__instance.mBoard, __instance.mRow) : (null, 0);
    internal static void Postfix((Board? Board, int Row) __state)
    {
        // Native DoSpecial inlines BurnRow in this exact Replanted build.
        if (__state.Board != null) PumpkinSupport.FireEnergy(__state.Board, __state.Row, 1800);
    }
}
