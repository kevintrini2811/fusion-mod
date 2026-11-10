using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Keep the real TakeDamage entry, including native immunity/balloon checks and
// external damage scaling. Only this source attack restores full damage after
// the native helmet stage; numeric source attack type 6 is not a native flag.
internal static class FootballFumeDamage
{
    internal sealed class Scope
    {
        private readonly Plant plant;
        private readonly Zombie zombie;
        private readonly uint zombieId;
        private readonly Board board;
        private readonly uint plantId;
        internal readonly IntPtr Target;
        internal int Depth;
        internal Scope(Plant plant, Zombie zombie, uint zombieId)
        {
            this.plant = plant;
            this.zombie = zombie;
            this.zombieId = zombieId;
            board = plant.mBoard;
            plantId = plant.DataID;
            Target = zombie.Pointer;
        }
        internal bool Valid => plant.DataID == plantId && plant.mBoard?.Pointer == board.Pointer
            && FumeFusions.Active(plant) && zombie.DataID == zombieId
            && zombie.mBoard?.Pointer == board.Pointer && !zombie.mDead
            && !zombie.IsDeadOrDying();
    }
    internal static Scope? Current;
    internal static bool Applies(Zombie zombie) => Current is { Depth: 1 } scope && scope.Target == zombie.Pointer;
    internal static void Hit(Plant plant, Zombie zombie, uint zombieId)
    {
        var previous = Current;
        var scope = new Scope(plant, zombie, zombieId);
        if (!scope.Valid) return;
        Current = scope;
        try { ZombieLibrary.Zombies.TakeDamageWithIndependentAddedArmor(zombie, 100, DamageFlags.HitsShieldAndBody); }
        finally { Current = previous; }
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class FootballFumeEntryPatch
{
    [HarmonyPriority(int.MaxValue)]
    internal static void Prefix(out FootballFumeDamage.Scope? __state)
    {
        __state = FootballFumeDamage.Current;
        if (__state != null) ++__state.Depth;
    }
    internal static Exception? Finalizer(Exception? __exception, FootballFumeDamage.Scope? __state)
    {
        if (__state != null) --__state.Depth;
        return __exception;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeShieldDamage))]
internal static class FootballFumeShieldPatch
{
    internal static bool Prefix(Zombie __instance, ref int __result)
    {
        if (!FootballFumeDamage.Applies(__instance) || FootballFumeDamage.Current!.Valid) return true;
        __result = 0;
        return false;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeHelmDamage))]
internal static class FootballFumeHelmPatch
{
    internal static bool Prefix(Zombie __instance, ref int __result)
    {
        if (!FootballFumeDamage.Applies(__instance) || FootballFumeDamage.Current!.Valid) return true;
        __result = 0;
        return false;
    }
    internal static void Postfix(Zombie __instance, int __0, bool __runOriginal, ref int __result)
    {
        if (__runOriginal && FootballFumeDamage.Applies(__instance))
            __result = FootballFumeDamage.Current!.Valid ? __0 : 0;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeBodyDamage))]
internal static class FootballFumeBodyPatch
{
    internal static bool Prefix(Zombie __instance) => !FootballFumeDamage.Applies(__instance)
        || FootballFumeDamage.Current!.Valid;
}
