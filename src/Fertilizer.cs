using HarmonyLib;
using System.Diagnostics.CodeAnalysis;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using LawnGlove;
using Il2CppSource.Controllers;
using UnityEngine;

namespace FusionMod;

// All clocks advance on Board.UpdateGame, the native 100 Hz simulation owner.
// No wall-clock timers and no Zen Garden inventory or profile mutations.
internal static class Fertilizer
{
    internal const int IntervalTicks = 7200;
    internal const int FirstDropTicks = 7050;
    internal const int LifetimeTicks = 3000;
    private const int BlinkTicks = 500;
    private const float HouseX = 45, HouseY = 160;
    private static Board? owner;
    private static readonly List<Drop> drops = new();
    private static Drop? held;
    private static int clock;

    private sealed class Drop
    {
        internal int Age;
        internal Reanimation? Visual;
        internal bool Initialized;
        internal Vector3 OriginalOffset;
        internal Vector2 OriginalScale;
        internal float OriginalRate;
        internal bool OriginalDieAfterPlay;
    }

    private static bool Playing(Board board) => board.mApp != null
        && board.mApp.GameScene == GameScenes.Playing
        && board.mApp.GameMode != GameMode.ChallengeZenGarden;

    internal static void Tick(Board board)
    {
        if (!Playing(board)) return;
        if (owner?.Pointer != board.Pointer)
        {
            Clear();
            owner = board;
        }
        if (board.mPaused) return;
        if (held != null && board.mCursorObject.CursorType != CursorType.Fertilizer) held = null;
        if (++clock >= FirstDropTicks && (clock - FirstDropTicks) % IntervalTicks == 0)
            drops.Add(new Drop());
        for (int i = drops.Count - 1; i >= 0; --i)
        {
            var drop = drops[i];
            if (drop != held && ++drop.Age > LifetimeTicks)
            {
                Destroy(drop);
                drops.RemoveAt(i);
                continue;
            }
            if (drop.Visual == null || drop.Visual.mDead)
            {
                drop.Initialized = false;
                drop.Visual = board.mApp.AddReanimation(HouseX, HouseY, 400000,
                    ReanimationType.ZengardenFertilizer);
            }
            var visual = drop.Visual;
            if (visual?.mController == null) continue;
            if (!drop.Initialized)
            {
                drop.OriginalOffset = visual.mController.VisualOffset;
                var scale = visual.mController.Scale;
                drop.OriginalScale = new Vector2(scale.x, scale.y);
                drop.OriginalRate = visual.mAnimRate;
                drop.OriginalDieAfterPlay = visual.mController.DieAfterPlay;
                drop.Initialized = true;
            }
            visual.mController.DieAfterPlay = false;
            visual.mAnimRate = 0;
            visual.mController.SetAnimationTimeOnTrack(0, CharacterAnimationTrack.Body);
            // Native feeding effect has a large offset above/right of its
            // plant. Remove that offset for the bag lying beside the house.
            visual.mController.VisualOffset = Vector3.zero;
            visual.OverrideScale(drop.OriginalScale.x, drop.OriginalScale.y);
            // Separate simultaneous uncollected bags; their individual clocks
            // remain independent, including after cancelling a pickup.
            visual.SetPosition(HouseX, DropY(drop, i));
            float alpha = drop == held ? 0 : drop.Age <= LifetimeTicks - BlinkTicks
                ? 1 : (drop.Age / 25 % 2 == 0 ? 1 : .25f);
            visual.mController.SetColorOverride(new Color(1, 1, 1, alpha), CharacterAnimationTrack.Body);
        }
    }

    internal static bool MouseDown(Board board, int x, int y, int clicks, int player)
    {
        if (owner?.Pointer != board.Pointer || !Playing(board)) return true;
        if (held != null && board.mCursorObject.CursorType != CursorType.Fertilizer) held = null;
        if (held != null)
        {
            if (clicks < 0)
            {
                held = null;
                board.ClearCursor(ignoreControlType: true, playerIndex: 0);
                return false;
            }
            if (board.mPaused || player != 0) return false;
            int col = board.PixelToGridX(x, y), row = board.PixelToGridY(x, y);
            if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return false;
            try
            {
                if (!Apply(board, col, row)) return false;
            }
            catch (Exception error)
            {
                FusionMod.Instance.LoggerInstance.Error($"Fertilizer could not be applied: {error}");
                return false;
            }
            var used = held;
            held = null;
            board.ClearCursor(ignoreControlType: true, playerIndex: 0);
            Destroy(used);
            drops.Remove(used);
            return false;
        }
        if (board.mPaused || player != 0 || clicks < 0) return true;
        for (int i = drops.Count - 1; i >= 0; --i)
        {
            if (Math.Abs(x - HouseX) > 50 || Math.Abs(y - (DropY(drops[i], i) - 20)) > 50) continue;
            board.ClearCursor(ignoreControlType: true, playerIndex: 0);
            held = drops[i];
            board.mCursorObject.CursorType = CursorType.Fertilizer;
            return false;
        }
        return true;
    }

    private static float DropY(Drop drop, int index)
    {
        float fall = Math.Min(drop.Age / 40f, 1);
        return HouseY + index * 48 - 70 * (1 - fall * fall);
    }

    private static bool Live([NotNullWhen(true)] Plant? plant) => plant != null && !plant.mDead && plant.mIsOnBoard
        && !plant.mSquished && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee
        && !plant.NotOnGround();

    internal static bool Apply(Board board, int col, int row)
    {
        // GetPlantsOnLawn exposes only one normal plant. Iterate the actual
        // board pool so all three small slots and every support are included.
        var plants = new List<Plant>();
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant != null && plant.mPlantCol == col && plant.mRow == row
                && Live(plant) && !GetCarried(board, plant)) plants.Add(plant);
        }
        if (plants.Count == 0) return false;
        var layers = board.GetPlantsOnLawn(col, row);
        var target = board.GetTopPlantAt(col, row, PlantPriority.DiggingOrder);
        if (!Live(target) || GetCarried(board, target)) target = plants[0];
        var effect = board.mApp.AddReanimation(target.mX, target.mY, target.mRenderOrder + 1,
            ReanimationType.ZengardenFertilizer);
        if (effect == null) return false;
        effect.mLoopType = ReanimLoopType.PlayOnceAndHold;
        var tools = new List<GridItem>();
        try
        {
            // Reserve all saved growth operations before healing or arming.
            // Each operation binds a DataID, so replacement plants cannot
            // inherit another plant's fertilizer during the animation.
            foreach (var plant in plants)
            {
                var identity = FusionPlant.From(plant);
                SeedType growth = SeedType.None;
                if (identity == (FusionPlant)SeedType.Wallnut
                    || identity == (FusionPlant)SeedType.Spikeweed
                    || identity == (FusionPlant)SeedType.Lilypad && !Live(layers.NormalPlant)
                    || identity == (FusionPlant)SeedType.Sunshroom && plant.mState != PlantState.SunshroomBig)
                    growth = plant.mSeedType;
                if (identity == FusionPlant.ScorchedThreepeater) growth = SeedType.Threepeater;
                if (identity == FusionPlant.SunsetShroom && plant.mStartRow < 2) growth = SeedType.Seashroom;
                if (identity == FusionPlant.FlameWallNut || identity == FusionPlant.FrostWallNut)
                    growth = identity.Marker;
                if (growth == SeedType.None) continue;
                var tool = board.m_gridItems.DataArrayAlloc();
                if (tool == null)
                {
                    foreach (var allocated in tools) allocated.GridItemDie();
                    effect.ReanimationDie();
                    return false;
                }
                tools.Add(tool);
                tool.Init(board.mApp, GridItemType.ZenTool);
                tool.mGridItemState = GridItemState.ZenToolFertilizer;
                tool.mGridX = col;
                tool.mGridY = row;
                tool.mSeedType = growth;
                // Counter is serialized at GridItem offset 0x80 and has no
                // native writer: our Update prefix owns combat feeding.
                tool.mGridItemCounter = unchecked((int)plant.DataID);
                tool.mPosX = target.mX;
                tool.mPosY = target.mY;
                tool.mX = tool.mY = -1000;
                tool.mRenderOrder = 800000;
                tool.mGridItemReanimID = board.mApp.ReanimationGetID(effect);
            }
        }
        catch
        {
            foreach (var tool in tools) tool.GridItemDie();
            effect.ReanimationDie();
            throw;
        }
        foreach (var plant in plants)
        {
            // Healing and damage visuals update immediately. Growth belongs
            // exclusively to animation completion.
            plant.mPlantHealth = Math.Max(plant.mPlantHealth, ActiveNuts.HealLimit(plant));
            plant.UpdateReanim();
            plant.UpdateReanimColor();
            var identity = FusionPlant.From(plant);
            // Source Fertilize.Upgrade caps positive attribute timers at .5s.
            // Silver variants have no fertilizer evolution into golden plants.
            if ((identity == FusionPlant.SilverIceShroom || identity == FusionPlant.SpicyStarfruit)
                && plant.mLaunchCounter > 50)
                plant.mLaunchCounter = 50;
            if ((FusionPlant.IsBeamShooter(identity) || (identity == FusionPlant.AuroraPlantern || identity == FusionPlant.BlacklightPlantern)) && plant.mTargetY > 50) plant.mTargetY = 50;
            UtilityMines.Fertilize(plant);
            TreasureMines.Fertilize(plant);
            GoldenPlants.Fertilize(plant);
            SeaFusions.Fertilize(plant);
            SeaNut.Fertilize(plant);
            if (identity == FusionPlant.Butterweed && plant.mTargetY > 50) plant.mTargetY = 50;
            SeaSupportFusions.Fertilize(plant);
            ChomperFusions.Fertilize(plant);
            EatingShrooms.Fertilize(plant);
            Spicicle.Fertilize(plant);
            PumpkinFusions.Fertilize(plant);
            IllumiNut.Fertilize(plant);
            FumeSupport.Fertilize(plant);
            DoomFusions.Fertilize(plant);
            DoomShooters.Fertilize(plant);
            SquashLobbers.Fertilize(plant);
            // Fertilizer instantly arms real underground potatoes in this cell.
            // Bucket, Cluster Mine and Chomp-mine have no buried growth phase.
            if ((identity == (FusionPlant)SeedType.Potatomine
                || identity == FusionPlant.PeatatoMine || identity == FusionPlant.SolarMine
                || identity == FusionPlant.IceMine || identity == FusionPlant.HypnoMine)
                && plant.mState is PlantState.Notready or PlantState.PotatoRising)
                MineFusions.GrowImmediately(plant);
        }
        return true;
    }

    internal static bool UpdateFeeding(GridItem tool)
    {
        var board = tool.mBoard;
        if (tool.mGridItemType != GridItemType.ZenTool
            || tool.mGridItemState != GridItemState.ZenToolFertilizer
            || board == null || board.mApp.GameMode == GameMode.ChallengeZenGarden) return true;
        if (!Playing(board) || board.mPaused || tool.mDead) return false;
        var effect = board.mApp.ReanimationTryToGet(tool.mGridItemReanimID);
        if (effect != null && !effect.mDead && (effect.mController == null
            || effect.mController.GetLoopCount(CharacterAnimationTrack.Body) == 0)) return false;
        // The native animation has ended. Resolve the current square again;
        // removed/carried plants and newly occupied lilies cannot be upgraded.
        try { Grow(board, tool.mGridX, tool.mGridY, tool.mSeedType, unchecked((uint)tool.mGridItemCounter)); }
        catch (Exception error)
        {
            FusionMod.Instance.LoggerInstance.Error($"Fertilizer growth failed: {error}");
            // Retain a usable bag when replacement allocation fails.
            if (owner?.Pointer == board.Pointer) drops.Add(new Drop());
        }
        finally
        {
            if (effect != null && !effect.mDead) effect.ReanimationDie();
            tool.GridItemDie();
        }
        return false;
    }

    private static void Grow(Board board, int col, int row, SeedType expected, uint expectedId)
    {
        var layers = board.GetPlantsOnLawn(col, row);
        Plant? plant = null;
        if (expectedId == 0) // Compatibility with feeding tools saved before 1.6.1.
            plant = expected == SeedType.Lilypad ? layers.UnderPlant : layers.NormalPlant;
        else
            for (int i = 0; i < board.m_plants.Count; ++i)
            {
                var candidate = board.m_plants[i];
                if (candidate != null && candidate.DataID == expectedId
                    && candidate.mPlantCol == col && candidate.mRow == row)
                { plant = candidate; break; }
            }
        var expectedIdentity = expected == SeedType.Threepeater ? FusionPlant.ScorchedThreepeater
            : expected == SeedType.Seashroom ? FusionPlant.SunsetShroom
            : expected == SeedType.Jalapeno ? FusionPlant.FlameWallNut
            : expected == SeedType.Iceshroom ? FusionPlant.FrostWallNut : (FusionPlant)expected;
        if (!Live(plant) || GetCarried(board, plant)
            || FusionPlant.From(plant) != expectedIdentity) return;
        if (expected == SeedType.Sunshroom)
        {
            if (plant.mState != PlantState.SunshroomBig)
            {
                plant.mState = PlantState.SunshroomSmall;
                plant.mStateCountdown = 0;
                plant.UpdateSunShroom();
            }
            return;
        }
        if (expected == SeedType.Seashroom)
        {
            SunflowerFusions.Fertilize(plant);
            return;
        }
        if (expected == SeedType.Lilypad && Live(layers.NormalPlant)) return;
        FusionPlant upgrade = expected == SeedType.Threepeater ? SeedType.Threepeater
            : expected == SeedType.Jalapeno ? FusionPlant.FlameTallNut
            : expected == SeedType.Iceshroom ? FusionPlant.FrostTallNut
            : expected == SeedType.Wallnut ? SeedType.Tallnut
            : expected == SeedType.Spikeweed ? SeedType.Spikerock
            : expected == SeedType.Lilypad ? SeedType.Cattail : SeedType.None;
        if (upgrade == SeedType.None
            || FusionService.CheckFusionPlacement(board, plant, upgrade) != PlantingReason.Ok) return;
        // Create before removing the healed original. Damage received during
        // feeding remains real; growth does not perform a second healing pass.
        var grown = board.AddPlant(col, row, upgrade.NativeType, upgrade.Marker);
        if (!Live(grown)) throw new InvalidOperationException("Fertilizer upgrade could not be created; original retained.");
        if (!plant.mDead) PlayerPlantingScope.Consume(plant);
    }

    private static bool GetCarried(Board board, Plant plant) =>
        LawnGloveMod.GetCarriedPlant(board)?.Pointer == plant.Pointer;

    private static void Destroy(Drop drop)
    {
        if (drop.Visual != null && !drop.Visual.mDead)
        {
            // Controllers are pooled. Restore every overridden property before
            // release so a later feeding effect cannot inherit a frozen bag.
            if (drop.Initialized && drop.Visual.mController != null)
            {
                var controller = drop.Visual.mController;
                controller.VisualOffset = drop.OriginalOffset;
                drop.Visual.OverrideScale(drop.OriginalScale.x, drop.OriginalScale.y);
                drop.Visual.mAnimRate = drop.OriginalRate;
                controller.DieAfterPlay = drop.OriginalDieAfterPlay;
                controller.SetColorOverride(Color.white, CharacterAnimationTrack.Body);
            }
            drop.Visual.ReanimationDie();
        }
        drop.Visual = null;
        drop.Initialized = false;
    }

    internal static void Clear()
    {
        if (held != null && owner?.mCursorObject?.CursorType == CursorType.Fertilizer) owner.ClearCursor(ignoreControlType: true, playerIndex: 0);
        held = null;
        foreach (var drop in drops) Destroy(drop);
        drops.Clear();
        owner = null;
        clock = 0;
    }

    internal static void PrepareSave()
    {
        if (held != null && owner?.mCursorObject?.CursorType == CursorType.Fertilizer) owner.ClearCursor(ignoreControlType: true, playerIndex: 0);
        held = null;
        foreach (var drop in drops) Destroy(drop);
    }

    internal static void BeforeDispose(Board board)
    {
        if (owner?.Pointer == board.Pointer) Clear();
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class FertilizerTickPatch
{
    private static void Postfix(Board __instance) => Fertilizer.Tick(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.MouseDown))]
internal static class FertilizerMousePatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Board __instance, int x, int y, int theClickCount, int playerIndex) =>
        Fertilizer.MouseDown(__instance, x, y, theClickCount, playerIndex);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class FertilizerDisposePatch
{
    private static void Prefix(Board __instance) => Fertilizer.BeforeDispose(__instance);
}

[HarmonyPatch(typeof(SerializationService), nameof(SerializationService.Serialize))]
internal static class FertilizerSavePatch
{
    // Remove transient pickup reanimations before native serialization, so
    // loading cannot restore a visible bag without its input/lifetime owner.
    private static void Prefix() => Fertilizer.PrepareSave();
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class FertilizerFeedingPatch
{
    // Combat feeding uses the native saved tool and animation, but must never
    // enter ZenGarden.DoFeedingTool (inventory/profile/garden progression).
    private static bool Prefix(GridItem __instance) => Fertilizer.UpdateFeeding(__instance);
}
