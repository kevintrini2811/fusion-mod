using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Only explicit Fusion applications acquire ownership. Unmarked saved/native
// statuses retain their native counter units and behavior.
internal static class FusionStatuses
{
    internal enum Effect { Butter = 1, Ice = 2, Chill = 4 }
    [ThreadStatic] private static (IntPtr Board, IntPtr Entity, uint Id)? applyingTo;
    [ThreadStatic] private static IntPtr globalFreezeBoard;
    internal sealed record ImpactOwner(IntPtr Board, IntPtr Entity, uint Id);
    [ThreadStatic] internal static ImpactOwner? ImpactTarget;
    private static bool ApplyingTo(Zombie z) => applyingTo is { } owner
        && owner.Board == z.mBoard?.Pointer && owner.Entity == z.Pointer && owner.Id == z.DataID;
    internal static bool OwnedImpact(Zombie z) => !ApplyingTo(z) && ImpactTarget is { } owner
        && owner.Board == z.mBoard?.Pointer && owner.Entity == z.Pointer && owner.Id == z.DataID;
    private static readonly Effect[] Effects = { Effect.Butter, Effect.Ice, Effect.Chill };
    private static int Counter(Zombie z, Effect effect) => effect == Effect.Butter ? z.mButteredCounter
        : effect == Effect.Ice ? z.mIceTrapCounter : z.mChilledCounter;
    private static void Counter(Zombie z, Effect effect, int value)
    {
        if (effect == Effect.Butter) z.mButteredCounter = value;
        else if (effect == Effect.Ice) z.mIceTrapCounter = value;
        else z.mChilledCounter = value;
    }
    private static float Units(FusionNativeTimeRecords.Entry entry, Effect effect) =>
        effect == Effect.Butter ? entry.A : effect == Effect.Ice ? entry.B : entry.C;
    private static void Units(FusionNativeTimeRecords.Entry entry, Effect effect, float value)
    {
        if (effect == Effect.Butter) entry.A = value;
        else if (effect == Effect.Ice) entry.B = value;
        else entry.C = value;
    }
    private static int Native(float units, int rate) => (int)Math.Ceiling((double)units * rate / 600 - 1e-9);
    private static FusionNativeTimeRecords.Entry? Record(Zombie z, bool create = false) => z.mBoard == null ? null
        : FusionNativeTimeRecords.Get(z.mBoard, FusionNativeTimeRecords.Kind.Status, z.DataID,
            create ? FusionNativeTimeRecords.CurrentRate : 0);

    // Native API calls outside our scope relinquish provenance even when a new
    // application writes exactly the same numeric counter as the previous one.
    internal static void NativeApplication(Zombie z, Effect effects)
    {
        if (ApplyingTo(z) || Record(z) is not { } entry) return;
        entry.Flags &= ~(int)effects;
        if (entry.Flags == 0) FusionNativeTimeRecords.Remove(entry);
    }

    private static void Observe(Zombie z, FusionNativeTimeRecords.Entry entry, bool allowDecay)
    {
        foreach (var effect in Effects)
        {
            if ((entry.Flags & (int)effect) == 0) continue;
            float units = Units(entry, effect);
            if (!float.IsFinite(units) || units <= 0) { entry.Flags &= ~(int)effect; continue; }
            int current = Counter(z, effect), expected = Native(units, entry.Rate);
            if (current <= 0 || current != expected && (!allowDecay || current != expected - 1))
            { entry.Flags &= ~(int)effect; continue; }
            if (current != expected) Units(entry, effect, Math.Max(0, units - 600 / entry.Rate));
        }
    }

    internal static void Prepare(Zombie z, bool allowDecay = false)
    {
        var entry = Record(z);
        if (entry == null) return;
        if (z.mDead || z.IsDeadOrDying()) { FusionNativeTimeRecords.Remove(entry); return; }
        Observe(z, entry, allowDecay);
        int rate = FusionNativeTimeRecords.CurrentRate;
        if (entry.Rate != rate)
        {
            foreach (var effect in Effects)
                if ((entry.Flags & (int)effect) != 0) Counter(z, effect, Native(Units(entry, effect), rate));
            entry.Rate = rate;
        }
        if (entry.Flags == 0) FusionNativeTimeRecords.Remove(entry);
    }
    internal static void AfterUpdate(Zombie z)
    {
        if (Record(z) is not { } entry) return;
        Observe(z, entry, true);
        if (entry.Flags == 0 || z.mDead || z.IsDeadOrDying()) FusionNativeTimeRecords.Remove(entry);
    }
    internal static void PrepareBoard(Board board)
    {
        if (!ScaredContent.Playing(board) || board.mPaused) return;
        foreach (var entry in FusionNativeTimeRecords.Entries(board, FusionNativeTimeRecords.Kind.Status))
        {
            var owner = board.ZombieTryToGet((ZombieID)entry.EntityId);
            if (owner == null || owner.DataID != entry.EntityId || owner.mBoard?.Pointer != board.Pointer)
                FusionNativeTimeRecords.Remove(entry);
            else Prepare(owner);
        }
    }
    private static void Set(Zombie z, Effect effect, int sourceTicks, int previous)
    {
        if (Counter(z, effect) <= 0) return; // Native immunity/controller owns acceptance.
        int desired = FusionTime.NativeTicks(sourceTicks);
        // A longer unowned native effect does not become Fusion-owned merely
        // because our shorter effect was offered to it.
        if (previous >= desired) { Counter(z, effect, previous); return; }
        var entry = Record(z, true);
        if (entry == null) return;
        Counter(z, effect, desired);
        entry.Flags |= (int)effect;
        Units(entry, effect, sourceTicks * 6f);
    }
    internal static void AdoptImpact(Zombie z, uint id, Effect effect, int sourceTicks, int previous)
    {
        if (z.DataID == id && OwnedImpact(z)) Set(z, effect, sourceTicks, previous);
    }
    internal static void Minimum(Zombie z, Effect effect, int sourceTicks)
    {
        Prepare(z, true);
        Set(z, effect, sourceTicks, Counter(z, effect));
    }
    internal static void Butter(Zombie z, int sourceTicks = 400)
    {
        Prepare(z, true);
        int previous = z.mButteredCounter;
        uint id = z.DataID; var board = z.mBoard?.Pointer;
        var previousScope = applyingTo;
        applyingTo = (z.mBoard?.Pointer ?? IntPtr.Zero, z.Pointer, id);
        try { z.ApplyButter(); }
        finally { applyingTo = previousScope; }
        if (z.DataID == id && z.mBoard?.Pointer == board) Set(z, Effect.Butter, sourceTicks, previous);
    }
    internal static void Chill(Zombie z, bool longer = false, int sourceTicks = 0)
    {
        Prepare(z, true);
        int previous = z.mChilledCounter;
        uint id = z.DataID; var board = z.mBoard?.Pointer;
        var previousScope = applyingTo;
        applyingTo = (z.mBoard?.Pointer ?? IntPtr.Zero, z.Pointer, id);
        try { z.ApplyChill(longer); }
        finally { applyingTo = previousScope; }
        if (z.DataID == id && z.mBoard?.Pointer == board)
            Set(z, Effect.Chill, sourceTicks > 0 ? sourceTicks : longer ? 2000 : 1000, previous);
    }
    internal static void GlobalFreeze(Plant plant)
    {
        PrepareBoard(plant.mBoard);
        var previous = globalFreezeBoard;
        globalFreezeBoard = plant.mBoard.Pointer;
        try { plant.IceZombies(); }
        finally { globalFreezeBoard = previous; }
    }
    internal static bool GlobalFreezeActive(Zombie z) => globalFreezeBoard != IntPtr.Zero
        && globalFreezeBoard == z.mBoard?.Pointer;
    internal static void AdoptGlobalFreeze(Zombie z, uint id, int ice, int chill)
    {
        if (!GlobalFreezeActive(z) || z.DataID != id) return;
        Set(z, Effect.Ice, 400, ice);
        Set(z, Effect.Chill, 1400, chill);
    }
    internal static void Freeze(Zombie z, int sourceTicks = 400, int chillTicks = 1400, bool suppressDamage = true)
    {
        Prepare(z, true);
        int ice = z.mIceTrapCounter, chill = z.mChilledCounter;
        uint id = z.DataID; var board = z.mBoard?.Pointer;
        var previous = SnowPea.CreatingIceTrap;
        if (suppressDamage) SnowPea.CreatingIceTrap = z.Pointer;
        var previousScope = applyingTo;
        applyingTo = (z.mBoard?.Pointer ?? IntPtr.Zero, z.Pointer, id);
        try { z.HitIceTrap(); }
        finally { applyingTo = previousScope; SnowPea.CreatingIceTrap = previous; }
        if (z.DataID != id || z.mBoard?.Pointer != board) return;
        Set(z, Effect.Ice, sourceTicks, ice);
        Set(z, Effect.Chill, chillTicks, chill);
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class FusionStatusBoardPatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Board __instance) => FusionStatuses.PrepareBoard(__instance);
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.Update))]
internal static class FusionStatusUpdatePatch
{
    internal static void Postfix(Zombie __instance) => FusionStatuses.AfterUpdate(__instance);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class FusionStatusImpactScope
{
    [HarmonyPriority(Priority.First)]
    internal static void Prefix(Projectile __instance, Zombie? theZombie, out FusionStatuses.ImpactOwner? __state)
    {
        __state = FusionStatuses.ImpactTarget;
        // Capture provenance before another impact prefix can retire the shot.
        var board = __instance.mBoard;
        FusionStatuses.ImpactTarget = board != null && theZombie != null && theZombie.mBoard?.Pointer == board.Pointer
            && FusionProjectileTime.Owns(__instance)
            ? new(board.Pointer, theZombie.Pointer, theZombie.DataID) : null;
    }
    internal static void Finalizer(FusionStatuses.ImpactOwner? __state) => FusionStatuses.ImpactTarget = __state;
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.ApplyButter))]
internal static class FusionStatusNativeButterPatch
{
    internal static void Prefix(Zombie __instance, out (uint Id, int Previous, bool Adopt) __state)
    {
        bool adopt = FusionStatuses.OwnedImpact(__instance);
        if (adopt) FusionStatuses.Prepare(__instance, true);
        __state = (__instance.DataID, __instance.mButteredCounter, adopt);
        if (!adopt) FusionStatuses.NativeApplication(__instance, FusionStatuses.Effect.Butter);
    }
    internal static void Postfix(Zombie __instance, (uint Id, int Previous, bool Adopt) __state)
    {
        if (__state.Adopt) FusionStatuses.AdoptImpact(__instance, __state.Id, FusionStatuses.Effect.Butter, 400, __state.Previous);
    }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.ApplyChill))]
internal static class FusionStatusNativeChillPatch
{
    internal static void Prefix(Zombie __instance, out (uint Id, int Previous, bool Adopt) __state)
    {
        bool adopt = FusionStatuses.OwnedImpact(__instance);
        if (adopt) FusionStatuses.Prepare(__instance, true);
        __state = (__instance.DataID, __instance.mChilledCounter, adopt);
        if (!adopt) FusionStatuses.NativeApplication(__instance, FusionStatuses.Effect.Chill);
    }
    internal static void Postfix(Zombie __instance, bool __0, (uint Id, int Previous, bool Adopt) __state)
    {
        if (__state.Adopt) FusionStatuses.AdoptImpact(__instance, __state.Id, FusionStatuses.Effect.Chill, __0 ? 2000 : 1000, __state.Previous);
    }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.HitIceTrap))]
internal static class FusionStatusNativeIcePatch
{
    internal static void Prefix(Zombie __instance, out (uint Id, int Ice, int Chill) __state)
    {
        __state = (__instance.DataID, __instance.mIceTrapCounter, __instance.mChilledCounter);
        if (!FusionStatuses.GlobalFreezeActive(__instance))
            FusionStatuses.NativeApplication(__instance, FusionStatuses.Effect.Ice | FusionStatuses.Effect.Chill);
    }
    internal static void Postfix(Zombie __instance, (uint Id, int Ice, int Chill) __state) =>
        FusionStatuses.AdoptGlobalFreeze(__instance, __state.Id, __state.Ice, __state.Chill);
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.RemoveColdEffects))]
internal static class FusionStatusRemoveColdPatch
{ internal static void Prefix(Zombie __instance) => FusionStatuses.NativeApplication(__instance, FusionStatuses.Effect.Ice | FusionStatuses.Effect.Chill); }
[HarmonyPatch(typeof(Zombie), nameof(Zombie.RemoveIceTrap))]
internal static class FusionStatusRemoveIcePatch
{ internal static void Prefix(Zombie __instance) => FusionStatuses.NativeApplication(__instance, FusionStatuses.Effect.Ice); }
