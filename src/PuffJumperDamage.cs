using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Replanted writes Plant HP inside native bite/projectile/board owners. Preserve
// their collision, sound and projectile retirement, veto only our captured HP
// loss before lethal Die, and restore nested scopes even on exceptions.
internal static class PuffJumperDamage
{
    internal sealed record Hit(Plant Plant, Board Board, uint Id, FusionPlant Identity, int Health, bool Bite, bool DirectDeath)
    {
        internal bool Valid => Plant.DataID == Id && Plant.mBoard?.Pointer == Board.Pointer
            && FusionPlant.From(Plant) == Identity && !Plant.mDead && !Plant.mSquished
            && ScaredContent.Playing(Board);
        internal void Restore()
        { if (Valid && Plant.mPlantHealth < Health) Plant.mPlantHealth = Health; }
    }
    [ThreadStatic] internal static List<Hit>? Current;

    internal static List<Hit> Capture(Board board, Plant? only = null, bool bite = false, bool directDeath = false)
    {
        var result = new List<Hit>();
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant == null || only != null && plant.Pointer != only.Pointer || plant.mPlantHealth <= 0
                || !PuffJumpers.DamageImmune(plant)) continue;
            result.Add(new(plant, board, plant.DataID, FusionPlant.From(plant), plant.mPlantHealth, bite, directDeath));
        }
        return result;
    }
    internal static void Finish(List<Hit>? previous)
    {
        try { if (Current != null && !ReferenceEquals(Current, previous)) foreach (var hit in Current) hit.Restore(); }
        finally { Current = previous; }
    }
    internal static bool BeforeDeath(Plant plant)
    {
        if (PlayerPlantingScope.ConsumingIngredient == plant.Pointer) return true;
        var hit = Current?.FirstOrDefault(hit => hit.Plant.Pointer == plant.Pointer && hit.Valid);
        if (hit == null || plant.mPlantHealth > 0 && !hit.DirectDeath) return true;
        hit.Restore();
        if (hit.Bite) --plant.mBoard.mPlantsEaten;
        return false;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class PuffJumperBiteDamageScope
{
    internal static void Prefix(Plant thePlant, out List<PuffJumperDamage.Hit>? __state)
    {
        __state = PuffJumperDamage.Current;
        if (thePlant?.mBoard != null && PuffJumpers.DamageImmune(thePlant))
            PuffJumperDamage.Current = PuffJumperDamage.Capture(thePlant.mBoard, thePlant, bite: true);
    }
    internal static void Finalizer(List<PuffJumperDamage.Hit>? __state) => PuffJumperDamage.Finish(__state);
}

[HarmonyPatch]
internal static class PuffJumperProjectileDamageScope
{
    internal static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.CheckForCollision)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.UpdateLobMotion)) };
    internal static void Prefix(Projectile __instance, out List<PuffJumperDamage.Hit>? __state)
    {
        __state = PuffJumperDamage.Current;
        if (!__instance.mDead && __instance.mBoard != null && __instance.mProjectileType == ProjectileType.Basketball)
            PuffJumperDamage.Current = PuffJumperDamage.Capture(__instance.mBoard);
    }
    internal static void Finalizer(List<PuffJumperDamage.Hit>? __state) => PuffJumperDamage.Finish(__state);
}

[HarmonyPatch]
internal static class PuffJumperExplosionDamageScope
{
    internal static IEnumerable<MethodBase> TargetMethods() => typeof(Board).GetMethods()
        .Where(method => method.Name == nameof(Board.KillAllPlantsInRadius));
    internal static void Prefix(Board __instance, out List<PuffJumperDamage.Hit>? __state)
    {
        __state = PuffJumperDamage.Current;
        PuffJumperDamage.Current = PuffJumperDamage.Capture(__instance, directDeath: true);
    }
    internal static void Finalizer(List<PuffJumperDamage.Hit>? __state) => PuffJumperDamage.Finish(__state);
}

// Native UpdateActions inlines Jalapeno-head row removal. Capture only that
// actual native damage owner, retaining its animation and all other zombies.
[HarmonyPatch]
internal static class PuffJumperFireDamageScope
{
    internal static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateZombieJalapenoHead)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateActions)) };
    internal static void Prefix(Zombie __instance, out List<PuffJumperDamage.Hit>? __state)
    {
        __state = PuffJumperDamage.Current;
        if (__instance.mZombieType == ZombieType.JalapenoHead && __instance.mBoard != null)
            PuffJumperDamage.Current = PuffJumperDamage.Capture(__instance.mBoard, directDeath: true);
    }
    internal static void Finalizer(List<PuffJumperDamage.Hit>? __state) => PuffJumperDamage.Finish(__state);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class PuffJumperLethalDamagePatch
{
    [HarmonyPriority(Priority.First + 4000)]
    internal static bool Prefix(Plant __instance, bool __runOriginal) =>
        !__runOriginal || PuffJumperDamage.BeforeDeath(__instance);
}
