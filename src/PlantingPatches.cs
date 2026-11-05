using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

[HarmonyPatch(typeof(Board), nameof(Board.CanPlantAt))]
internal static class FusionPlacementPatch
{
    private static void Postfix(Board __instance, int theGridX, int theGridY, SeedType theType,
        ref PlantingReason __result)
    {
        if (FusionService.CheckingNativePlacement) return;
        if (LawnGlove.LawnGloveMod.GetCarriedPlant(__instance) is { } carried
            && theType == carried.mSeedType
            && (FusionPlant.From(carried) == FusionPlant.Splash || FusionPlant.From(carried) == FusionPlant.SeaNut || FusionPlant.From(carried) == FusionPlant.SeaMine))
        {
            __result = FusionService.CheckNativePlacement(__instance, theGridX, theGridY, SeedType.Seashroom);
            return;
        }
        // Native Imitater morphing creates a fresh plant outside the player
        // scope. Keep its placement native, matching the existing exclusion.
        if (__instance.CursorObjects[PlayerCursorScope.PlayerIndex(__instance)] is
            { CursorType: CursorType.PlantFromBank, mImitaterType: not SeedType.None }) return;
        if (LawnGlove.LawnGloveMod.GetCarriedPlant(__instance) is { } shell
            && theType == shell.mSeedType && FusionPlant.From(shell) == FusionPlant.SeaShell)
        {
            __result = FusionService.ShellTerrain(__instance, theGridX, theGridY, FusionPlant.SeaShell);
            return;
        }
        var plans = FusionService.ResolveAll(__instance, theType, theGridX, theGridY);
        if (plans.Count == 0 && ShroomSlots.IsSmall(theType))
        {
            if (ShroomSlots.CanAdd(__instance, theType, theGridX, theGridY)) { __result = PlantingReason.Ok; return; }
            // Never let a native replacement plant into an occupied subfield.
            if (__result == PlantingReason.Ok && ShroomSlots.Plants(__instance, theGridX, theGridY).Count > 0)
                __result = PlantingReason.NotHere;
        }
        if (plans.Count == 0) return;
        // The result owns terrain validation. A land ingredient can be rejected
        // on water before the native occupancy check, including sea variants.
        foreach (var plan in plans)
        {
            __result = FusionService.CheckFusionPlacement(__instance, plan.Target, plan.Result);
            if (__result != PlantingReason.Ok) return;
        }
    }
}

// Only player planting may reinterpret an AddPlant call. Scripted spawns and
// save restoration must never fuse merely because another plant occupies a cell.
[HarmonyPatch(typeof(Board), "MouseDownWithPlant")]
internal static class PlayerPlantingScope
{
    internal sealed class Context
    {
        internal Board Board = null!;
        internal Plant? Ingredient;
        internal Plant? Partner;
        internal int X, Y;
        internal int PlayerIndex;
        internal bool AllowIngredientDeath;
        internal FusionPlant? CreatedFusion;
    }

    [ThreadStatic] internal static Context? Current;
    [ThreadStatic] internal static IntPtr ConsumingIngredient;
    internal static Board? ActiveBoard => Current?.Board;

    private static void Prefix(Board __instance, int x, int y, int playerIndex, out Context? __state)
    {
        __state = Current;
        Current = new Context { Board = __instance, X = x, Y = y, PlayerIndex = playerIndex };
        var cursor = __instance.CursorObjects[playerIndex];
        if (cursor == null || cursor.CursorType != CursorType.PlantFromBank
            || cursor.mImitaterType != SeedType.None) return;
        int col = __instance.PlantingPixelToGridX(x, y, cursor.mType);
        int row = __instance.PlantingPixelToGridY(x, y, cursor.mType);
        if (FusionService.TryResolve(__instance, cursor.mType, col, row, out var target, out var result)
            && FusionService.CheckFusionPlacement(__instance, target, result) == PlantingReason.Ok)
        {
            Current.Ingredient = target;
            if (result == (FusionPlant)SeedType.Cobcannon) Current.Partner = FusionService.CobPartner(__instance, target);
        }
    }

    private static void Finalizer(Context? __state) => Current = __state;

    internal static void Consume(Plant ingredient)
    {
        var context = Current;
        bool previous = context?.AllowIngredientDeath ?? false;
        var previousIngredient = ConsumingIngredient;
        ConsumingIngredient = ingredient.Pointer;
        if (context != null) context.AllowIngredientDeath = true;
        try { ingredient.Die(); }
        finally
        {
            ConsumingIngredient = previousIngredient;
            if (context != null) context.AllowIngredientDeath = previous;
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class DeferFusionIngredientDeathPatch
{
    private static bool Prefix(Plant __instance)
    {
        var context = PlayerPlantingScope.Current;
        // Native Wall-nut repair removes the existing nut before AddPlant.
        // Keep only this click's validated ingredient alive until the result
        // exists; ordinary repairs, shovelling and unrelated deaths stay native.
        return context == null || context.AllowIngredientDeath
            || (context.Ingredient?.Pointer != __instance.Pointer && context.Partner?.Pointer != __instance.Pointer);
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.AddPlant))]
internal static class FusionPlantCreationPatch
{
    internal sealed class Creation
    {
        internal FusionPlant Incoming;
        internal List<FusionService.Plan> Plans = new();
        internal int Slot;
        internal bool Small;
    }

    private static void Prefix(Board __instance, int theGridX, int theGridY,
        ref SeedType theSeedType, ref SeedType theImitaterType, out Creation? __state)
    {
        __state = null;
        if (FusionService.CreatingBatch || PlayerPlantingScope.ActiveBoard?.Pointer != __instance.Pointer
            || theImitaterType != SeedType.None) return;
        var incoming = (FusionPlant)theSeedType;
        var plans = FusionService.ResolveAll(__instance, incoming, theGridX, theGridY);
        if (plans.Count > 0 && plans.All(p => FusionService.CheckFusionPlacement(__instance, p.Target, p.Result) == PlantingReason.Ok))
        {
            __state = new Creation { Incoming = incoming, Plans = plans };
            theSeedType = plans[0].Result.NativeType;
            theImitaterType = plans[0].Result.Marker;
        }
        else if (ShroomSlots.TryPlacementSlot(__instance, incoming, theGridX, theGridY, out int slot))
            __state = new Creation { Incoming = incoming, Small = true, Slot = slot };
    }

    private static void Postfix(Board __instance, Plant __result, Creation? __state)
    {
        if (__state == null || __result == null || __result.mDead || !__result.mIsOnBoard) return;
        if (__state.Small)
        {
            ShroomSlots.Position(__result, __result.mPlantCol, __result.mRow, __state.Slot);
            return;
        }
        FusionService.CompleteBatch(__instance, __state.Incoming, __state.Plans, __result);
        if (PlayerPlantingScope.Current is { } context) context.CreatedFusion = FusionPlant.From(__result);
    }
}
