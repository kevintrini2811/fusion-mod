using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// BucketDoom's explode clip emits at 2/3 seconds with controller speed .7.
// The saved countdown stays in source centiseconds; native animation is visual.
internal static class DoomplateShroom
{
    internal const int FuseTicks = 96, Damage = 3240;
    internal const float SourceUnit = 80f / 1.35f;
    private sealed class Detonation
    {
        internal readonly IntPtr Board;
        internal bool Invalidated;
        internal Detonation(IntPtr board) { Board = board; }
    }
    private static readonly List<Detonation> detonations = new();

    // Only synchronous, currently executing Doomplate payloads are retained.
    // Dispose completion cannot revive one when the shared pointerguard resets.
    internal static void BeforeDispose(Board board)
    {
        foreach (var detonation in detonations)
            if (detonation.Board == board.Pointer) detonation.Invalidated = true;
    }
    internal static bool Owns(Plant plant) => FusionPlant.From(plant) == FusionPlant.DoomplateShroom;

    internal static void Initialize(Plant plant)
    {
        if (!Owns(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mDoSpecialCountdown = plant.mStartRow = plant.mTargetX = plant.mTargetY = 0;
        plant.mLaunchCounter = FuseTicks;
    }

    internal static void Tick(Plant plant)
    {
        if (!Owns(plant) || !MineFusions.Active(plant) || plant.mIsAsleep || plant.mStartRow != 0
            || !FusionTime.SourceStep(plant, typeof(DoomplateShroom))) return;
        if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
        Detonate(plant);
    }

    internal static void Detonate(Plant plant, bool crushed = false)
    {
        if (!Owns(plant) || !MineFusions.Active(plant) || plant.mIsAsleep && !crushed || plant.mStartRow != 0) return;
        var board = plant.mBoard;
        IntPtr boardPointer = board.Pointer, pointer = plant.Pointer;
        uint id = plant.DataID;
        int col = plant.mPlantCol, row = plant.mRow, order = plant.mRenderOrder;
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        var dropOrigin = ImportedPeaVisuals.DoomplateOrigin(plant);
        plant.mStartRow = 1; // Commit before Die can reenter the ability.
        var detonation = new Detonation(boardPointer);
        detonations.Add(detonation);
        try
        {
            bool OwnsPayload() => !detonation.Invalidated && board.Pointer == boardPointer
                && boardPointer != CherryBombPlants.DisposingBoard && ScaredContent.Playing(board);
            plant.Die();
            // Read only identity until the old native generation is confirmed.
            if (detonation.Invalidated || plant.Pointer != pointer || plant.DataID != id
                || plant.mBoard?.Pointer != boardPointer || !plant.mDead || board.Pointer != boardPointer
                || board.Pointer == CherryBombPlants.DisposingBoard) return;

            // The original callback scatters each bucket around the plant's shadow,
            // independently in X and Y, rather than around the killed zombie.
            DoomEffects.Damage(board, row, x, y, order, damage: Damage,
                confirmedKill: () =>
                {
                    if (!OwnsPayload()) return;
                    BucketDrops.Add(board, row,
                        dropOrigin.X + (board.Rand(4001) / 1000f - 2) * SourceUnit,
                        dropOrigin.Y + (board.Rand(4001) / 1000f - 2) * SourceUnit, FusionPlant.Bucket);
                }, ownsPayload: OwnsPayload);
            if (OwnsPayload()) DoomEffects.LeaveCrater(board, col, row, ownsPayload: OwnsPayload);
        }
        finally { detonations.Remove(detonation); }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class DoomplateInitializePatch
{ private static void Postfix(Plant __instance) => DoomplateShroom.Initialize(__instance); }

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class DoomplateAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!DoomplateShroom.Owns(__instance)) return true;
        DoomplateShroom.Tick(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class DoomplateShootingPatch
{ private static bool Prefix(Plant __instance) => !DoomplateShroom.Owns(__instance); }

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class DoomplateSpecialPatch
{ private static bool Prefix(Plant __instance) => !DoomplateShroom.Owns(__instance); }

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class DoomplateCrushPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (!DoomplateShroom.Owns(__instance) || !MineFusions.Active(__instance)) return true;
        // DoomShroom.OnCrash dispatches the virtual AnimExplode immediately,
        // including a sleeping mushroom, rather than waiting for its clip.
        DoomplateShroom.Detonate(__instance, crushed: true);
        return false;
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class DoomplateDisposePatch
{
    [HarmonyPriority(Priority.First + 2000)]
    internal static void Prefix(Board __instance) => DoomplateShroom.BeforeDispose(__instance);
}
