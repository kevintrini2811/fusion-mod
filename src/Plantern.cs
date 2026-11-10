using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Plantern already clears the original 5x7 fog footprint without corners.
// PumpkinFusions.LightLevel already supplies its additive 3x3 level-one light.
// Source Fusion 4.0.5.2 differs in its 15-second recharge (25 sun is native).
[HarmonyPatch(typeof(Plant), nameof(Plant.GetRefreshTime))]
internal static class PlanternRechargePatch
{
    internal static bool Matches(SeedType seed, SeedType imitater) => seed == SeedType.Plantern
        || seed == SeedType.Imitater && imitater == SeedType.Plantern;

    private static void Postfix(SeedType theSeedType, SeedType theImitaterType, ref int __result)
    {
        if (Matches(theSeedType, theImitaterType))
            __result = 1500;
    }
}

// The published Plantern recharge fields stay canonical source centiseconds.
// Native packet rendering, morphing and other work still run every native tick.
internal static class PlanternPacketClock
{
    internal sealed record Scope(IntPtr Board, IntPtr Packet, SeedType Seed, SeedType Imitater, int Counter);
    internal static Scope? Before(SeedPacket packet)
    {
        var board = packet.mBoard;
        if (board == null || !ScaredContent.Playing(board) || !PlanternRechargePatch.Matches(packet.mPacketType, packet.mImitaterType)
            || packet.mRefreshTime != 1500 || packet.mActive || !packet.mRefreshing
            || packet.mRefreshCounter < 0 || packet.mRefreshCounter > packet.mRefreshTime
            || FusionTime.SourceStep(board, typeof(PlanternPacketClock), packet.Pointer)) return null;
        var scope = new Scope(board.Pointer, packet.Pointer, packet.mPacketType, packet.mImitaterType, packet.mRefreshCounter);
        // Native Update increments before testing >mRefreshTime. Cancelling that
        // single increment prevents premature completion and its side effects.
        --packet.mRefreshCounter;
        return scope;
    }
    internal static void After(SeedPacket packet, Scope? scope)
    {
        if (scope != null && packet.Pointer == scope.Packet && packet.mBoard?.Pointer == scope.Board
            && packet.mPacketType == scope.Seed && packet.mImitaterType == scope.Imitater
            && packet.mRefreshTime == 1500 && packet.mRefreshing && !packet.mActive
            && packet.mRefreshCounter == scope.Counter - 1)
            packet.mRefreshCounter = scope.Counter; // Native early return or an exception before its increment.
    }
}
[HarmonyPatch(typeof(SeedPacket), nameof(SeedPacket.Update))]
internal static class PlanternPacketClockPatch
{
    [HarmonyPriority(Priority.Last - 1000)]
    private static void Prefix(SeedPacket __instance, out PlanternPacketClock.Scope? __state) =>
        __state = PlanternPacketClock.Before(__instance);
    private static void Finalizer(SeedPacket __instance, PlanternPacketClock.Scope? __state) =>
        PlanternPacketClock.After(__instance, __state);
}
