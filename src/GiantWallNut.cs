using HarmonyLib;
using System.Reflection;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppSource.Controllers;
using UnityEngine;

namespace FusionMod;

// Reuse the game's GiantWallnut identity, controller, ground animation and save
// data. Its rolling behavior must also work outside the bowling challenge.
internal static class GiantWallNut
{
    private const float Size = 1.5f;
    private const float Speed = 0.45f;
    // Plant.UpdateBowling uses this logical board coordinate, not Widget.mWidth.
    private const float RightEdge = 800;
    private const int ContactDamage = 1800;

    private sealed class RollState
    {
        internal IntPtr Board;
        internal uint PlantId;
        internal readonly HashSet<uint> HitZombies = new();
    }

    private static readonly Dictionary<IntPtr, RollState> Rolls = new();

    internal static void Forget(Plant plant) => Rolls.Remove(plant.Pointer);

    internal static void ForgetBoard(Board board)
    {
        foreach (var key in Rolls.Where(entry => entry.Value.Board == board.Pointer).Select(entry => entry.Key).ToArray())
            Rolls.Remove(key);
    }

    private static RollState GetState(Plant plant)
    {
        uint id = plant.DataID;
        if (!Rolls.TryGetValue(plant.Pointer, out var state)
            || state.PlantId != id || state.Board != plant.mBoard.Pointer)
            Rolls[plant.Pointer] = state = new RollState { Board = plant.mBoard.Pointer, PlantId = id };
        return state;
    }

    internal static bool IsRoller(Plant plant) => plant.mSeedType == SeedType.GiantWallnut
        && plant.mBoard != null && plant.mApp != null
        && plant.mApp.GameMode != GameMode.ChallengeZenGarden;

    internal static void Update(Plant plant)
    {
        var board = plant.mBoard;
        var controller = plant.mController;
        if (plant.mDead || !plant.mIsOnBoard || plant.mSquished || board.mPaused
            || plant.mOnBungeeState != PlantOnBungeeState.NotOnBungee
            || controller == null || !controller) return;

        // Set absolute values, not a multiplier that grows each tick/load.
        controller.OverrideScale(Size, Size);
        plant.mWidth = plant.mHeight = 120;
        plant.mState = PlantState.Ready;
        // Slow the visual roll to match translation. Keep the established
        // translation speed independent of animation rate and visual size.
        if (!controller.IsAnimationPlayingOnTrack("_ground", CharacterAnimationTrack.Body))
            controller.PlayAnimationOnTrack("_ground", CharacterAnimationTrack.Body, 12 * Speed, ReanimLoopType.Loop);

        controller.SetAnimationRateOnTrack(12 * Speed, CharacterAnimationTrack.Body);

        float oldX = plant.mX;
        float groundStep = controller.GetGroundVelocity();
        // Native ground displacement includes visual scale and animation rate.
        float normalStep = MathF.Abs(groundStep / Size / Speed);
        plant.mX += normalStep * Speed;
        int col = board.PixelToGridX((int)plant.mX, (int)plant.mY);
        if (col >= 0 && col < 9) plant.mPlantCol = col;

        // Keep mRow and mY unchanged. A swept rectangle catches every contact,
        // including overlapping zombies and a frame that crosses a target.
        var hit = plant.GetPlantRect();
        hit.xMin = MathF.Min(hit.xMin, hit.xMin - (plant.mX - oldX));
        bool impacted = false;
        var state = GetState(plant);
        var zombies = board.m_zombies;
        for (int i = zombies.Count - 1; i >= 0; --i)
        {
            var zombie = zombies[i];
            if (zombie == null || zombie.mDead || zombie.IsDeadOrDying()
                || zombie.mRow != plant.mRow || !zombie.EffectedByDamage(DamageRangeFlags.Ground)
                || !hit.Overlaps(zombie.GetZombieRect()) || !state.HitZombies.Add((uint)zombie.DataID)) continue;
            // Native explosion damage (0x708, flags 0x12), applied only to
            // this contact. No explosion, burn effect or nearby-target search.
            // The per-roller ledger prevents repeated hits on survivors.
            zombie.TakeDamage(ContactDamage, DamageFlags.HitsShieldAndBody);
            impacted = true;
        }
        if (impacted) board.mApp.SoundSystem.PlayFoley(FoleyType.BowlingImpact);
        if (plant.mX > RightEdge) plant.Die();
    }
}

[HarmonyPatch]
internal static class GiantWallNutCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Plant), nameof(Plant.Die));
        yield return AccessTools.Method(typeof(Plant), nameof(Plant.Reset));
    }

    private static void Postfix(Plant __instance)
    {
        if (__instance.mDead || __instance.mSeedType != SeedType.GiantWallnut) GiantWallNut.Forget(__instance);
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class GiantWallNutBoardCleanupPatch
{
    private static void Prefix(Board __instance) => GiantWallNut.ForgetBoard(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class GiantWallNutUpdatePatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!GiantWallNut.IsRoller(__instance)) return true;
        GiantWallNut.Update(__instance);
        return false;
    }
}
