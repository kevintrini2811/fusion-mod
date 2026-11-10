using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class SilverUmbrella
{
    internal sealed class Reflection
    {
        internal readonly Projectile Shot;
        internal readonly uint ShotId;
        internal readonly Board Board;
        internal Plant? Umbrella;
        internal uint UmbrellaId;
        internal Reflection(Projectile shot)
        {
            Shot = shot;
            ShotId = shot.DataID;
            Board = shot.mBoard;
        }
    }

    [ThreadStatic] internal static Reflection? Current;

    internal static void Found(Plant? plant)
    {
        var reflection = Current;
        // UpdateLobMotion defers while the umbrella opens. Only the reflecting
        // branch destroys this basketball; bungees do not enter this scope.
        if (reflection == null || plant == null
            || (FusionPlant.From(plant) != FusionPlant.SilverUmbrella && FusionPlant.From(plant) != FusionPlant.BushShroom)
            || plant.mState != PlantState.UmbrellaReflecting
            || !ChomperFusions.Active(plant) || plant.mBoard?.Pointer != reflection.Board.Pointer) return;
        reflection.Umbrella = plant;
        reflection.UmbrellaId = plant.DataID;
    }

    internal static void Finish(Reflection? reflection)
    {
        if (reflection?.Umbrella is not { } plant
            || !reflection.Shot.mDead || reflection.Shot.DataID != reflection.ShotId
            || reflection.Shot.mBoard?.Pointer != reflection.Board.Pointer
            || plant.DataID != reflection.UmbrellaId || !ChomperFusions.Active(plant)
            || plant.mBoard?.Pointer != reflection.Board.Pointer) return;
        if (FusionPlant.From(plant) == FusionPlant.BushShroom) { FumeSupport.Energy(plant, 75); return; }
        SilverMoney.DropMoney(reflection.Board, plant.mX, plant.mY, SilverMoney.MoneyUnits(plant));
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateLobMotion))]
internal static class SilverUmbrellaReflectionScope
{
    private static void Prefix(Projectile __instance, out SilverUmbrella.Reflection? __state)
    {
        __state = SilverUmbrella.Current;
        SilverUmbrella.Current = !__instance.mDead && __instance.mBoard != null
            && __instance.mProjectileType == ProjectileType.Basketball
            ? new SilverUmbrella.Reflection(__instance) : null;
    }

    private static void Finalizer(Exception? __exception, SilverUmbrella.Reflection? __state)
    {
        var current = SilverUmbrella.Current;
        SilverUmbrella.Current = __state;
        if (__exception == null) SilverUmbrella.Finish(current);
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.FindUmbrellaPlant))]
internal static class SilverUmbrellaTargetPatch
{
    private static void Postfix(Plant? __result) => SilverUmbrella.Found(__result);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class SilverUmbrellaShootingPatch
{
    private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.SilverUmbrella;
}
