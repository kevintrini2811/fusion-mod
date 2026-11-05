using Il2CppReloaded.Gameplay;
using LawnGlove;

namespace FusionMod;

internal static class FusionService
{
    internal static PlantingReason ShellTerrain(Board board, int col, int row, FusionPlant identity, Plant? replacing = null)
    {
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return PlantingReason.NotHere;
        var stack = board.GetPlantsOnLawn(col, row);
        if (stack.PumpkinPlant is { mDead: false } shell && shell.Pointer != replacing?.Pointer)
            return PlantingReason.NotHere;
        // Sea-shell remains on the accessory layer, but can float without a pad.
        var type = identity == FusionPlant.SeaShell && board.IsPoolSquare(col, row)
            ? SeedType.Seashroom : SeedType.Pumpkinshell;
        var hidden = new List<Plant>();
        if (replacing != null) hidden.Add(replacing);
        if (type == SeedType.Seashroom && stack.NormalPlant != null) hidden.Add(stack.NormalPlant);
        return ShroomSlots.CheckTerrain(board, col, row, type, hidden);
    }

    internal static bool CanStackCarried(Board board, Plant source, int col, int row)
    {
        if (source.mSeedType == SeedType.Pumpkinshell)
            return ShellTerrain(board, col, row, FusionPlant.From(source)) == PlantingReason.Ok;
        var stack = board.GetPlantsOnLawn(col, row);
        if ((stack.PumpkinPlant == null && stack.UnderPlant == null && stack.FlyingPlant == null)
            || stack.NormalPlant != null
            || source.mSeedType is SeedType.Lilypad or SeedType.Flowerpot or SeedType.InstantCoffee
            || AirPlants.Owns(source)) return false;
        var type = LawnGlove.PlantPlacementRules.TerrainType(source.mSeedType);
        if (source.mSeedType == SeedType.Cobcannon)
        {
            if (col >= 8 || board.GetPlantsOnLawn(col + 1, row).NormalPlant != null) return false;
            if (CheckNativePlacement(board, col + 1, row, type) != PlantingReason.Ok) return false;
        }
        return CheckNativePlacement(board, col, row, type) == PlantingReason.Ok;
    }

    [ThreadStatic] internal static bool CheckingNativePlacement;

    public static PlantingReason CheckNativePlacement(Board board, int col, int row, SeedType result)
    {
        bool previous = CheckingNativePlacement;
        CheckingNativePlacement = true;
        try { return board.CanPlantAt(col, row, result); }
        finally { CheckingNativePlacement = previous; }
    }

    public static PlantingReason CheckFusionPlacement(Board board, Plant target, FusionPlant fusion)
    {
        if (FusionPlant.IsPumpkinFusion(fusion))
            return FusionService.ShellTerrain(board, target.mPlantCol, target.mRow, fusion, target);
        if (AirPlants.Owns(fusion)) return AirPlants.Terrain(board, target.mPlantCol, target.mRow, target);
        var result = fusion == FusionPlant.Splash || fusion == FusionPlant.SeaNut || fusion == FusionPlant.SeaMine ? SeedType.Seashroom : fusion.NativeType;
        if (result == SeedType.Cobcannon)
        {
            if (target.mPlantCol >= 8) return PlantingReason.NotHere;
            if (FusionPlant.From(target) == (FusionPlant)SeedType.Kernelpult)
                // A cannon card still requires both native Kernel-pults.
                return CobPartner(board, target) == null ? PlantingReason.NotHere
                    : CheckNativePlacement(board, target.mPlantCol, target.mRow, result);
            if (FusionPlant.From(target) != (FusionPlant)SeedType.Cobcannon) return PlantingReason.NotHere;
            // Feeding a kernel to an existing cannon replaces its two-cell
            // footprint. Check both cells without hiding unrelated occupants.
            for (int col = target.mPlantCol; col <= target.mPlantCol + 1; ++col)
            {
                var reason = ShroomSlots.CheckTerrain(board, col, target.mRow, SeedType.Kernelpult, new[] { target });
                if (reason != PlantingReason.Ok) return reason;
            }
            return PlantingReason.Ok;
        }
        var occupants = ShroomSlots.Plants(board, target.mPlantCol, target.mRow);
        if (FusionPlant.IsFusionPot(fusion))
            // A support can fuse underneath an unrelated occupant. Hide the
            // normal layer only for the terrain query; never consume it.
            return ShroomSlots.CheckTerrain(board, target.mPlantCol, target.mRow, result,
                occupants.Append(target).DistinctBy(p => p.Pointer));
        if (occupants.Count > 1 && occupants.All(p => ShroomSlots.IsSmall(FusionPlant.From(p))))
            return ShroomSlots.CheckTerrain(board, target.mPlantCol, target.mRow, result, occupants);
        if (Plant.IsUpgrade(result) && target.IsUpgradableTo(result))
            return CheckNativePlacement(board, target.mPlantCol, target.mRow, result);

        // Ordinary results and upgrades with a different fusion ingredient
        // cannot natively occupy this cell. Exclude only the ingredient for a
        // synchronous terrain query, preserving supports and pumpkins. Reuse
        // the glove's upgrade-to-terrain mapping instead of duplicating it.
        bool wasSquished = target.mSquished;
        bool wasOnBoard = target.mIsOnBoard;
        try
        {
            target.mSquished = true;
            target.mIsOnBoard = false;
            return CheckNativePlacement(board, target.mPlantCol, target.mRow,
                PlantPlacementRules.TerrainType(result));
        }
        finally
        {
            target.mIsOnBoard = wasOnBoard;
            target.mSquished = wasSquished;
        }
    }

    private static bool IsLive(Plant? plant) => plant != null && !plant.mDead && plant.mIsOnBoard
        && !plant.mSquished && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee && !plant.NotOnGround();

    internal static void CompleteFusion(Plant fused, Plant target, FusionPlant incoming, Plant? source = null, bool activateIce = true)
    {
        // Capture recipe identities before consumption. A cherry marker on a
        // fused shooter is not a fresh Cherry Bomb ingredient.
        bool ice = activateIce && (incoming == (FusionPlant)SeedType.Iceshroom
            || FusionPlant.From(target) == (FusionPlant)SeedType.Iceshroom);
        bool cherry = incoming == (FusionPlant)SeedType.Cherrybomb
            || FusionPlant.From(target) == (FusionPlant)SeedType.Cherrybomb;
        if (!target.mDead) PlayerPlantingScope.Consume(target);
        if (source != null && !source.mDead) PlayerPlantingScope.Consume(source);
        if (FusionPlant.From(fused) == FusionPlant.SolarPult) fused.mBoard.AddSunMoney(100, 0);
        if (FusionPlant.From(fused) == FusionPlant.BloodSunflower)
            PlantLibrary.Plants.Unlock(PlantLibrary.Plants.Find(new PlantLibrary.PlantIdentity(
                FusionPlant.BloodSunflower.NativeType, FusionPlant.BloodSunflower.Marker))!, fused.mBoard);
        MineFusions.OnFusion(fused);
        ThreepeaterFusions.OnFusion(fused);
        if (FusionPlant.From(fused) == FusionPlant.ChompMine) fused.mBoard.AddSunMoney(75, 0);
        if (FusionPlant.From(fused) == FusionPlant.MelonPot) fused.mBoard.AddSunMoney(150, 0);
        if (FusionPlant.From(fused) == FusionPlant.TriPot) fused.mBoard.AddSunMoney(100, 0);
        if (FusionPlant.From(fused) == FusionPlant.BlackCherry) IceInstantFusions.OnFusion(fused);
        else if (cherry) CherryBombPlants.ExplodeFusion(fused);
        if (ice) IceInstantFusions.OnIceFusion(fused);
    }

    public static bool TryResolve(Board board, FusionPlant incoming, int col, int row,
        out Plant target, out FusionPlant result)
    {
        target = null!;
        result = SeedType.None;
        if (board.mApp == null || board.mApp.GameScene != GameScenes.Playing
            || board.mApp.GameMode == GameMode.ChallengeZenGarden
            || col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return false;
        var plans = ResolveAll(board, incoming, col, row);
        if (plans.Count == 0) return false;
        target = plans[0].Target;
        result = plans[0].Result;
        return true;
    }

    internal static Plant? CobPartner(Board board, Plant target)
    {
        if (target.mPlantCol >= 8 || FusionPlant.From(target) != (FusionPlant)SeedType.Kernelpult) return null;
        var right = board.GetPlantsOnLawn(target.mPlantCol + 1, target.mRow).NormalPlant;
        return IsLive(right) && FusionPlant.From(right!) == (FusionPlant)SeedType.Kernelpult ? right : null;
    }

    internal readonly record struct Plan(Plant Target, FusionPlant Result, Plant? Partner = null, List<Plant>? PuffIngredients = null);

    internal static List<Plan> ResolveAll(Board board, FusionPlant incoming, int col, int row, Plant? source = null)
    {
        var plans = new List<Plan>();
        if (board.mApp == null || board.mApp.GameScene != GameScenes.Playing
            || board.mApp.GameMode == GameMode.ChallengeZenGarden
            || col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return plans;
        var plants = ShroomSlots.Plants(board, col, row).Concat(AirPlants.At(board, col, row))
            .Where(p => p.Pointer != source?.Pointer).ToList();
        var layers = board.GetPlantsOnLawn(col, row);
        var pot = layers.UnderPlant;
        bool potRecipe = pot != null && pot.Pointer != source?.Pointer && IsLive(pot)
            && pot.mSeedType == SeedType.Flowerpot
            && FusionRecipes.TryGet(FusionPlant.From(pot), incoming, out _);
        // A support recipe belongs to the lower third, even on an empty pot.
        // The upper two thirds retain normal planting and occupant fusion.
        bool potArea = PlayerCursorScope.Position(board).Y >= board.GridToPixelY(col, row) + 80 * 2 / 3f;
        if (potRecipe && potArea)
        {
            if (FusionRecipes.TryGet(FusionPlant.From(pot!), incoming, out var fusion)
                && FusionRecipes.AcceptsIngredient(pot!, fusion)
                && (source == null || FusionRecipes.AcceptsIngredient(source, fusion)))
                plans.Add(new(pot!, fusion));
            return plans;
        }
        // A spread consumes the complete plain Puff-shroom cluster once and
        // leaves one native large mushroom. Mixed clusters cannot share its cell.
        if (plants.Count > 0 && plants.Any(p => FusionRecipes.IsPuffSpreading(FusionPlant.From(p), incoming)))
        {
            if (plants.All(p => IsLive(p) && FusionPlant.From(p) == (FusionPlant)SeedType.Puffshroom))
                plans.Add(new(plants[0], incoming, PuffIngredients: plants));
            return plans;
        }
        bool small = plants.Count > 0 && plants.All(p => ShroomSlots.IsSmall(FusionPlant.From(p)));
        int slot = small ? ShroomSlots.Hover(board, col, row) : 0;
        foreach (var plant in plants)
        {
            // Puff-shroom fusions feed every matching mushroom in the tile,
            // including a partially occupied tile. Other small plants keep
            // their selected-subfield behavior until the tile is full.
            if (small && plants.Count < 3 && FusionPlant.From(plant).NativeType != SeedType.Puffshroom
                && ShroomSlots.Slot(plant) != slot) continue;
            if (FusionRecipes.TryGet(FusionPlant.From(plant), incoming, out var fusion)
                && FusionRecipes.AcceptsIngredient(plant, fusion)
                && (source == null || FusionRecipes.AcceptsIngredient(source, fusion)))
                plans.Add(new Plan(plant, fusion, fusion == (FusionPlant)SeedType.Cobcannon ? CobPartner(board, plant) : null));
        }
        if (plans.Count == 0)
        {
            // Prefer the normal layer's recipe. One incoming card must not
            // pay for both a normal result and a support result on that tile.
            foreach (var support in new[] { layers.UnderPlant, layers.PumpkinPlant })
            {
            if (support?.mSeedType == SeedType.Flowerpot) continue;
            if (support != null && support.Pointer != source?.Pointer && IsLive(support)
                && FusionRecipes.TryGet(FusionPlant.From(support), incoming, out var fusion)
                && FusionRecipes.AcceptsIngredient(support, fusion)
                && (source == null || FusionRecipes.AcceptsIngredient(source, fusion)))
                { plans.Add(new(support, fusion)); break; }
            }
        }
        return plans;
    }

    // Allocate all results before consuming any ingredient. A failed allocation
    // removes only the new results and leaves every ingredient intact.
    internal static void CompleteBatch(Board board, FusionPlant incoming, List<Plan> plans, Plant? first = null, Plant? source = null)
    {
        var created = new List<Plant>();
        bool previous = CreatingBatch;
        CreatingBatch = true;
        try
        {
            for (int i = 0; i < plans.Count; ++i)
            {
                var plan = plans[i];
                var fused = i == 0 && first != null ? first : board.AddPlant(plan.Target.mPlantCol,
                    plan.Target.mRow, plan.Result.NativeType, plan.Result.Marker);
                if (fused == null || fused.mDead || !fused.mIsOnBoard)
                    throw new InvalidOperationException("Fusion result could not be created; ingredients retained.");
                created.Add(fused);
                if (ShroomSlots.IsSmall(FusionPlant.From(plan.Target)) && ShroomSlots.IsSmall(plan.Result))
                    ShroomSlots.Position(fused, plan.Target.mPlantCol, plan.Target.mRow, ShroomSlots.Slot(plan.Target));
            }
            foreach (var plan in plans)
                if (plan.PuffIngredients != null) PuffSpreading.Create(board, plan, created);
        }
        catch
        {
            foreach (var plant in created) PlayerPlantingScope.Consume(plant);
            throw;
        }
        finally { CreatingBatch = previous; }
        if (source != null && !source.mDead) PlayerPlantingScope.Consume(source);
        foreach (var plan in plans)
            if (plan.PuffIngredients != null)
                foreach (var puff in plan.PuffIngredients)
                    if (puff.Pointer != plan.Target.Pointer && !puff.mDead) PlayerPlantingScope.Consume(puff);
        for (int i = 0; i < plans.Count; ++i)
        {
            if (plans[i].Partner is { mDead: false } partner) PlayerPlantingScope.Consume(partner);
            // One incoming Ice-shroom can fuse several small plants. Spend its
            // global effect once; independently consumed target Ice-shrooms act normally.
            CompleteFusion(created[i], plans[i].Target, incoming,
                activateIce: i == 0 || incoming != (FusionPlant)SeedType.Iceshroom);
        }
    }

    [ThreadStatic] internal static bool CreatingBatch;

    private static Plant? FeedTarget(Board board, Plant source, int col, int row)
    {
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()
            || source.mBoard?.Pointer != board.Pointer || source.mDead
            || source.mOnBungeeState != PlantOnBungeeState.NotOnBungee
            || (source.mPlantCol == col && source.mRow == row)) return null;
        var lawn = board.GetPlantsOnLawn(col, row);
        var target = lawn.PumpkinPlant is { } shell && BucketPlants.CanFeed(shell, FusionPlant.From(source))
            ? shell : lawn.NormalPlant;
        return target != null && BucketPlants.CanFeed(target, FusionPlant.From(source)) ? target : null;
    }

    public static bool CanPlaceCarried(Board board, Plant source, int col, int row)
    {
        // Preview receives the temporarily detached source; commit receives it
        // restored. Do not require mIsOnBoard/!mSquished for this read-only query.
        if (FeedTarget(board, source, col, row) != null) return true;
        if (source.mDead || source.mBoard?.Pointer != board.Pointer
            || source.mOnBungeeState != PlantOnBungeeState.NotOnBungee
            || (source.mPlantCol == col && source.mRow == row)) return false;
        var plans = ResolveAll(board, FusionPlant.From(source), col, row, source);
        return plans.Count > 0
            ? plans.All(p => CheckFusionPlacement(board, p.Target, p.Result) == PlantingReason.Ok)
            : AirPlants.Owns(source) ? AirPlants.Terrain(board, col, row, source) == PlantingReason.Ok
            : FusionService.CanStackCarried(board, source, col, row)
                || ShroomSlots.CanAdd(board, FusionPlant.From(source), col, row, source);
    }

    public static bool TryPlaceCarried(Board board, Plant source, int col, int row)
    {
        if (!board.mPaused && IsLive(source) && FeedTarget(board, source, col, row) is { } fed)
        {
            BucketPlants.Feed(fed);
            PlayerPlantingScope.Consume(source);
            return true;
        }
        if (board.mPaused || !IsLive(source) || !CanPlaceCarried(board, source, col, row)) return false;
        var plans = ResolveAll(board, FusionPlant.From(source), col, row, source);
        if (plans.Count > 0)
        {
            CompleteBatch(board, FusionPlant.From(source), plans, source: source);
            return true;
        }
        if (AirPlants.Owns(source) || FusionService.CanStackCarried(board, source, col, row))
        { AirPlants.Move(board, source, col, row); return true; }
        if (ShroomSlots.TryPlacementSlot(board, FusionPlant.From(source), col, row, out int slot, source))
        {
            ShroomSlots.Position(source, col, row, slot);
            return true;
        }
        return false;
    }
}
