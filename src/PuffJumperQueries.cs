using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class PuffJumperQueries
{
    internal static PlantQueryHide Air(Board board)
    {
        var hidden = new List<Plant>();
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant != null && plant.mBoard?.Pointer == board.Pointer && Squish.Airborne(plant)) hidden.Add(plant);
        }
        return new(board, hidden);
    }

    internal sealed class Owner
    {
        internal readonly Zombie Zombie;
        private readonly IntPtr pointer;
        private readonly uint id;
        private readonly ZombieType type;
        private readonly IntPtr boardPointer;
        internal readonly Board Board;
        internal int Depth;
        internal bool Armed, Finished;
        internal PlantQueryHide? Hide;
        internal Owner(Zombie zombie)
        { Zombie = zombie; pointer = zombie.Pointer; id = zombie.DataID; type = zombie.mZombieType; Board = zombie.mBoard; boardPointer = Board.Pointer; }
        internal bool MatchesIdentity(Zombie zombie) => zombie.Pointer == pointer && zombie.DataID == id
            && zombie.mZombieType == type && Board.Pointer == boardPointer && zombie.mBoard?.Pointer == boardPointer;
        internal bool Matches(Zombie zombie) => MatchesIdentity(zombie) && !zombie.mDead && ScaredContent.Playing(Board);
        internal void Restore() { Hide?.Dispose(); Hide = null; }
    }
}

// Both owners only select candidates. Keep all native ordering/fallback rules.
[HarmonyPatch]
internal static class PuffJumperSelectorScope
{
    internal static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.FindCatapultTarget)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.BossCanStompRow)) };
    internal static void Prefix(Zombie __instance, out PlantQueryHide? __state)
    { __state = !__instance.mDead && ScaredContent.Playing(__instance.mBoard) ? PuffJumperQueries.Air(__instance.mBoard) : null; }
    internal static void Finalizer(PlantQueryHide? __state) => __state?.Dispose();
}

[HarmonyPatch]
internal static class PuffJumperBungeeScope
{
    [ThreadStatic] internal static PuffJumperQueries.Owner? Current;
    internal static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.BungeeStealTarget)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateZombieBungee)) };
    internal static void Prefix(Zombie __instance, out PuffJumperQueries.Owner? __state)
    { __state = Current; Current = new(__instance); }
    internal static void Finalizer(PuffJumperQueries.Owner? __state) => Current = __state;
}

[HarmonyPatch(typeof(Board), nameof(Board.GetTopPlantAt))]
internal static class PuffJumperBungeeQuery
{
    internal sealed record Query(PuffJumperQueries.Owner Owner, PlantQueryHide? Hide);
    internal static void Prefix(Board __instance, int theGridX, int theGridY, PlantPriority thePriority,
        bool __runOriginal, out Query? __state)
    {
        __state = null;
        if (PuffJumperBungeeScope.Current is not { } owner) return;
        bool match = __runOriginal && owner.Depth == 0 && owner.Matches(owner.Zombie)
            && __instance.Pointer == owner.Board.Pointer && theGridX == owner.Zombie.mTargetCol
            && theGridY == owner.Zombie.mRow && thePriority == PlantPriority.BungeeOrder;
        ++owner.Depth;
        try { __state = new(owner, match ? PuffJumperQueries.Air(__instance) : null); }
        catch { --owner.Depth; throw; }
    }
    internal static void Finalizer(Query? __state)
    {
        if (__state == null) return;
        try { __state.Hide?.Dispose(); }
        finally { --__state.Owner.Depth; }
    }
}

// Native Stomp enumerates only between RemoveColdEffects and PlayZombieReanim.
// Restore before animation callbacks, including no-candidate/exception paths.
[HarmonyPatch(typeof(Zombie), nameof(Zombie.BossStompAttack))]
internal static class PuffJumperBossScope
{
    [ThreadStatic] internal static PuffJumperQueries.Owner? Current;
    internal static void Prefix(Zombie __instance, out PuffJumperQueries.Owner? __state)
    { __state = Current; Current = new(__instance); }
    internal static void Finalizer(PuffJumperQueries.Owner? __state)
    { try { Current?.Restore(); } finally { Current = __state; } }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.RemoveColdEffects))]
internal static class PuffJumperBossColdCheckpoint
{
    [HarmonyPriority(Priority.Last - 4000)]
    internal static void Postfix(Zombie __instance, bool __runOriginal)
    {
        var owner = PuffJumperBossScope.Current;
        if (!__runOriginal || owner == null || owner.Armed || owner.Finished || !owner.Matches(__instance)) return;
        owner.Armed = true;
        owner.Hide = PuffJumperQueries.Air(owner.Board);
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.PlayZombieReanim))]
internal static class PuffJumperBossAnimationCheckpoint
{
    [HarmonyPriority(Priority.First + 4000)]
    internal static void Prefix(Zombie __instance)
    {
        var owner = PuffJumperBossScope.Current;
        if (owner == null || !owner.Armed || owner.Finished || !owner.MatchesIdentity(__instance)) return;
        owner.Finished = true;
        owner.Restore();
    }
}
