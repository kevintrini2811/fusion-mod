using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Three hits include the first impact. Native projectile fields and the shared
// saved hit history survive loading; animation never owns damage or movement.
internal static class PhoenixProjectiles
{
    private const int Marker = -590000;
    internal static bool Contains(Projectile shot) => shot.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
        && shot.mCobTargetX is <= Marker - 1 and >= Marker - 3;
    internal static void Mark(Projectile shot)
    {
        shot.mCobTargetX = Marker - 3;
        shot.mCobTargetRow = 160;
        ImportedPeaVisuals.EnsurePhoenixProjectile(shot);
    }
    internal static bool Impact(Projectile shot, Zombie? zombie)
    {
        if (!Contains(shot)) return false;
        if (shot.mDead) return true;
        if (zombie == null) { shot.Die(); return true; }
        var board = shot.mBoard;
        if (!SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)
            || !NutProjectiles.Remember(shot, zombie)) return true;
        uint id = zombie.DataID;
        int damage = FumeProjectiles.ModifiedDamage(shot, shot.mCobTargetRow);
        bool last = ++shot.mCobTargetX == Marker;
        if (last) shot.Die(); // Commit before damage/death callbacks.
        zombie.TakeDamage(damage, DamageFlags.BypassesShield);
        if (zombie.DataID == id && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground))
            Overheat.Apply(zombie);
        return true;
    }
    internal static void BurnBoard(Board board, float x, float y, int order)
    {
        for (int row = 0; row < board.GetNumRows(); ++row)
        {
            ElementalNuts.Heal(board, true, row);
            DoomEffects.Damage(board, row, x, y, order, fire: true);
        }
    }
    internal static void IgniteRow(Board board, int row)
    {
        if (board == null || board.Pointer == CherryBombPlants.DisposingBoard) return;
        var targets = new List<(Plant Plant, uint Id)>();
        for (int i = 0; i < board.m_plants.Count; ++i)
            if (board.m_plants[i] is { } plant && plant.mRow == row
                && FusionPlant.From(plant) == FusionPlant.CharredThreepeater && ChomperFusions.Active(plant))
                targets.Add((plant, plant.DataID));
        foreach (var (plant, id) in targets)
        {
            if (plant.DataID != id || FusionPlant.From(plant) != FusionPlant.CharredThreepeater
                || !ChomperFusions.Active(plant)
                || FusionService.CheckFusionPlacement(board, plant, FusionPlant.PhoenixThreepeater) != PlantingReason.Ok) continue;
            var grown = board.AddPlant(plant.mPlantCol, plant.mRow,
                FusionPlant.PhoenixThreepeater.NativeType, FusionPlant.PhoenixThreepeater.Marker);
            if (grown == null || grown.mDead) continue;
            PlayerPlantingScope.Consume(plant);
            ThreepeaterFusions.OnFusion(grown);
        }
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class PhoenixImpactPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Projectile __instance, Zombie theZombie) => !PhoenixProjectiles.Impact(__instance, theZombie);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class PhoenixVisualPatch
{
    private static void Postfix(Projectile __instance)
    { if (!__instance.mDead && PhoenixProjectiles.Contains(__instance)) ImportedPeaVisuals.EnsurePhoenixProjectile(__instance); }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class PhoenixTorchwoodPatch
{ private static bool Prefix(Projectile __instance) => !PhoenixProjectiles.Contains(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class PhoenixDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state)
    {
        __state = __runOriginal && FusionPlant.From(__instance) == FusionPlant.PhoenixThreepeater
            && CherryBombPlants.CanTriggerDeath(__instance) && !__instance.NotOnGround()
            ? new(__instance.mBoard, FusionPlant.PhoenixThreepeater, __instance.DataID,
                __instance.mPlantCol, __instance.mRow, __instance.mX, __instance.mY, __instance.mRenderOrder) : null;
    }
    private static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id)
            PhoenixProjectiles.BurnBoard(__state.Board, __state.X, __state.Y, __state.Order);
    }
}
