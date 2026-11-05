using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native bites and enemy projectiles write HP directly. Observe those writers,
// including their synchronous lethal Die call, rather than polling lost HP.
internal static class ReactiveFumeDamage
{
    internal static bool Invulnerable(Plant plant) => (FusionPlant.From(plant) == FusionPlant.DoomCactus
        && plant.mDoSpecialCountdown > 0 || FusionPlant.From(plant) == FusionPlant.SeaNut && plant.mStartRow >= 3)
        && FumeFusions.Active(plant)
        && LawnGlove.LawnGloveMod.GetCarriedPlant(plant.mBoard)?.Pointer != plant.Pointer;
    internal sealed class Hit
    {
        internal readonly Plant Plant;
        internal readonly Board Board;
        internal readonly uint Id;
        internal readonly int Health;
        internal readonly bool Bite;
        internal bool Applied;
        internal Hit(Plant plant, bool bite)
        { Plant = plant; Board = plant.mBoard; Id = plant.DataID; Health = plant.mPlantHealth; Bite = bite; }
        internal bool Valid => Plant.DataID == Id && Plant.mBoard?.Pointer == Board.Pointer;
        internal void Apply()
        {
            if (Applied || !Valid || Plant.mPlantHealth >= Health) return;
            Applied = true;
            int damage = Health - Plant.mPlantHealth;
            if (Invulnerable(Plant)) { Plant.mPlantHealth = Health; return; }
            damage = LumosEnergy.Absorb(Plant, PlantShields.Absorb(Plant, damage));
            Plant.mPlantHealth = Health - damage;
            if (damage == 0) return;
            if (Spicicle.Guard(Plant) != null)
            {
                Plant.mPlantHealth = Health;
                Spicicle.Redirect(Plant, damage);
                return;
            }
            if (FusionPlant.From(Plant) == FusionPlant.BushShroom)
            {
                FumeSupport.Energy(Plant, Math.Min(damage / 2, Plant.mPlantMaxHealth));
                Plant.mPlantHealth = Health - Math.Max(1, (int)MathF.Round(damage * .1f));
            }
            else FumeSupport.Trigger(Plant);
            if (Valid && FusionPlant.From(Plant) == FusionPlant.PumpkinMine) PumpkinSupport.CheckMine(Plant);
        }
    }
    [ThreadStatic] internal static List<Hit>? Current;
    internal static Hit? Capture(Plant plant, bool bite) =>
        ((FumeSupport.Reactive(FusionPlant.From(plant)) || FusionPlant.From(plant) == FusionPlant.PumpkinMine || Invulnerable(plant)) && FumeFusions.Active(plant)
            || Spicicle.Guard(plant) != null || PlantShields.Amount(plant) > 0 || FusionPlant.From(plant) == FusionPlant.LumosUmbrella && FumeFusions.Active(plant)) && plant.mPlantHealth > 0 ? new(plant, bite) : null;
    internal static void Apply(List<Hit>? hits)
    { if (hits != null) foreach (var hit in hits) hit.Apply(); }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class ReactiveFumeBiteScope
{
    private static void Prefix(Plant thePlant, out List<ReactiveFumeDamage.Hit>? __state)
    {
        __state = ReactiveFumeDamage.Current;
        ReactiveFumeDamage.Current = thePlant != null && ReactiveFumeDamage.Capture(thePlant, true) is { } hit
            ? new() { hit } : null;
    }
    [HarmonyPriority(Priority.First + 300)]
    private static void Postfix() => ReactiveFumeDamage.Apply(ReactiveFumeDamage.Current);
    private static void Finalizer(List<ReactiveFumeDamage.Hit>? __state) => ReactiveFumeDamage.Current = __state;
}

[HarmonyPatch]
internal static class ReactiveFumeProjectileScope
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.CheckForCollision)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.UpdateLobMotion)) };
    private static void Prefix(Projectile __instance, out List<ReactiveFumeDamage.Hit>? __state)
    {
        __state = ReactiveFumeDamage.Current;
        ReactiveFumeDamage.Current = null;
        if (__instance.mDead || __instance.mBoard == null
            || __instance.mProjectileType is not (ProjectileType.ZombiePea or ProjectileType.Basketball)) return;
        var hits = new List<ReactiveFumeDamage.Hit>();
        var plants = __instance.mBoard.m_plants;
        for (int i = 0; i < plants.Count; ++i)
            if (plants[i] is { } plant && ReactiveFumeDamage.Capture(plant, false) is { } hit) hits.Add(hit);
        ReactiveFumeDamage.Current = hits;
    }
    [HarmonyPriority(Priority.First + 300)]
    private static void Postfix() => ReactiveFumeDamage.Apply(ReactiveFumeDamage.Current);
    private static void Finalizer(List<ReactiveFumeDamage.Hit>? __state) => ReactiveFumeDamage.Current = __state;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class ReactiveFumeDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    private static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal) return true;
        if (ReactiveFumeDamage.Invulnerable(__instance)
            && (RetaliationNuts.RadiusBoard == __instance.mBoard?.Pointer
                || RetaliationNuts.FireBoard == __instance.mBoard?.Pointer)
            && PlayerPlantingScope.ConsumingIngredient != __instance.Pointer)
        {
            --__instance.mBoard.mPlantsEaten;
            return false;
        }
        // Native jack explosions remove plants directly, bypassing HP writers.
        // Source explosions deal 1800 before Bush-shroom's reduction.
        if (FusionPlant.From(__instance) == FusionPlant.BushShroom && __instance.mPlantHealth > 0
            && RetaliationNuts.JackBoard == __instance.mBoard?.Pointer
            && RetaliationNuts.RadiusBoard == __instance.mBoard?.Pointer
            && PlayerPlantingScope.ConsumingIngredient != __instance.Pointer
            && ReactiveFumeDamage.Capture(__instance, false) is { } blast)
        {
            __instance.mPlantHealth -= 1800;
            blast.Apply();
            if (__instance.mPlantHealth <= 0) return true;
            --blast.Board.mPlantsEaten;
            return false;
        }
        if (ReactiveFumeDamage.Current == null || __instance.mPlantHealth > 0) return true;
        var hit = ReactiveFumeDamage.Current.FirstOrDefault(h => h.Plant.Pointer == __instance.Pointer && !h.Applied);
        if (hit == null || !hit.Valid) return true;
        hit.Apply();
        if (__instance.mPlantHealth <= 0) return true;
        if (hit.Bite) --hit.Board.mPlantsEaten;
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class DoomCactusCrushPatch
{
    private static bool Prefix(Plant __instance) => !ReactiveFumeDamage.Invulnerable(__instance)
        || PlayerPlantingScope.ConsumingIngredient == __instance.Pointer;
}
