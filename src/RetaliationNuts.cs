using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Wall-nut owns blocking, health and save identity. Observe actual HP writers:
// bites, zombie peas, basketballs and combat removal. Never fire on a shovel,
// fusion consumption, loading or teardown.
internal static class RetaliationNuts
{
    internal sealed record Hit(Plant Plant, Board Board, uint Id, int Health,
        FusionPlant Identity, float X, float Y, int Row, int Order);
    [ThreadStatic] internal static Hit? CurrentBite;
    [ThreadStatic] internal static IntPtr RadiusBoard;
    [ThreadStatic] internal static IntPtr JackBoard;
    [ThreadStatic] internal static IntPtr FireBoard;

    internal static Hit? Capture(Plant plant) => FusionPlant.IsReactiveNut(FusionPlant.From(plant))
        && ChomperFusions.Active(plant) && plant.mPlantHealth > 0
        ? new(plant, plant.mBoard, plant.DataID, plant.mPlantHealth, FusionPlant.From(plant),
            plant.mX + plant.mWidth / 2f, plant.mY + plant.mHeight / 2f,
            plant.mRow, plant.mRenderOrder) : null;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (FusionPlant.IsReactiveNut(identity))
        {
            plant.mPlantHealth = plant.mPlantMaxHealth = 4000;
            // Wall-nut has no launch routine. This saved field records the HP
            // already acknowledged by retaliation, including a delayed death.
            plant.mLaunchCounter = plant.mPlantHealth;
        }
        else if (identity == FusionPlant.IronSquash)
            plant.mPlantHealth = plant.mPlantMaxHealth = 1000;
    }

    internal static void AfterHit(Hit? hit, Zombie? eater = null)
    {
        if (hit == null || hit.Plant.DataID != hit.Id || hit.Plant.mBoard?.Pointer != hit.Board.Pointer
            || hit.Plant.mPlantHealth >= hit.Health) return;
        int count = eater == null ? 1 : BiteEvents.Count(hit.Plant, eater,
            (int)Math.Min(int.MaxValue, (long)hit.Health - hit.Plant.mPlantHealth));
        if (count == 0) { hit.Plant.mLaunchCounter = hit.Plant.mPlantHealth; return; }
        for (int i = 0; i < count; ++i)
        {
            if (hit.Identity == FusionPlant.StarNut)
            {
                hit.Plant.mLaunchCounter = hit.Plant.mPlantHealth;
                RetaliationProjectiles.Stars(hit);
            }
            else if (eater != null && eater.mBoard?.Pointer == hit.Board.Pointer && !eater.mDead
                && !eater.IsDeadOrDying() && !eater.mMindControlled)
            {
                PoisonShooters.PoisonBite(hit.Board, eater);
                // Reuse the native saved disgust/row-change sequence, including
                // terrain checks and immunity while frozen, rather than teleporting.
                if (eater.mYuckyFaceCounter == 0) eater.mYuckyFaceCounter = 1;
            }
        }
    }

    internal static List<Hit>? ProjectileHits(Projectile projectile)
    {
        if (projectile.mDead || projectile.mBoard == null
            || projectile.mProjectileType is not (ProjectileType.ZombiePea or ProjectileType.Basketball)) return null;
        var hits = new List<Hit>();
        var plants = projectile.mBoard.m_plants;
        for (int i = 0; i < plants.Count; ++i)
            if (plants[i] is { } plant && FusionPlant.From(plant) == FusionPlant.StarNut
                && Capture(plant) is { } hit) hits.Add(hit);
        return hits;
    }

    internal static bool BeforeDeath(Plant plant)
    {
        var board = plant.mBoard;
        if (board == null || !ChomperFusions.Active(plant)
            || PlayerPlantingScope.ConsumingIngredient == plant.Pointer) return true;
        if (FusionPlant.From(plant) == FusionPlant.IronSquash && JackBoard == board.Pointer
            && RadiusBoard == board.Pointer && plant.mPlantHealth > 0)
        {
            // Native jack explosions call Die directly. Source cherry damage
            // is 1800; iron resistance halves only this explosion category.
            plant.mPlantHealth -= 900;
            if (plant.mPlantHealth > 0)
            {
                // Both native radius overloads increment this immediately
                // before Die. A surviving plant must not count as eaten.
                --board.mPlantsEaten;
                return false;
            }
        }
        if (FusionPlant.From(plant) != FusionPlant.StarNut
            || CurrentBite?.Plant.Pointer == plant.Pointer) return true;
        bool combat = RadiusBoard == board.Pointer || FireBoard == board.Pointer
            || (plant.mPlantHealth <= 0 && plant.mLaunchCounter != plant.mPlantHealth);
        if (combat)
        {
            var hit = new Hit(plant, board, plant.DataID, plant.mPlantHealth, FusionPlant.StarNut,
                plant.mX + plant.mWidth / 2f, plant.mY + plant.mHeight / 2f, plant.mRow, plant.mRenderOrder);
            plant.mLaunchCounter = plant.mPlantHealth;
            RetaliationProjectiles.Stars(hit);
        }
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class RetaliationInitializePatch
{
    internal static void Postfix(Plant __instance) => RetaliationNuts.Initialize(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class RetaliationBitePatch
{
    internal static void Prefix(Zombie __instance, Plant thePlant, out (RetaliationNuts.Hit? Previous, RetaliationNuts.Hit? Hit, uint Eater) __state)
    {
        var hit = thePlant == null ? null : RetaliationNuts.Capture(thePlant);
        __state = (RetaliationNuts.CurrentBite, hit, __instance.DataID);
        RetaliationNuts.CurrentBite = hit;
    }
    internal static void Postfix(Zombie __instance, (RetaliationNuts.Hit? Previous, RetaliationNuts.Hit? Hit, uint Eater) __state) =>
        RetaliationNuts.AfterHit(__state.Hit, __instance.DataID == __state.Eater ? __instance : null);
    internal static void Finalizer((RetaliationNuts.Hit? Previous, RetaliationNuts.Hit? Hit, uint Eater) __state) =>
        RetaliationNuts.CurrentBite = __state.Previous;
}

[HarmonyPatch]
internal static class RetaliationProjectileHitPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.CheckForCollision)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.UpdateLobMotion)) };
    internal static void Prefix(Projectile __instance, out List<RetaliationNuts.Hit>? __state) =>
        __state = RetaliationNuts.ProjectileHits(__instance);
    internal static void Postfix(List<RetaliationNuts.Hit>? __state)
    { if (__state != null) foreach (var hit in __state) RetaliationNuts.AfterHit(hit); }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class RetaliationDeathPatch
{
    // Respect earlier fusion/death suppression; a skipped Die is not damage.
    [HarmonyPriority(Priority.Last)]
    internal static bool Prefix(Plant __instance, bool __runOriginal) =>
        !__runOriginal || RetaliationNuts.BeforeDeath(__instance);
}

[HarmonyPatch]
internal static class RetaliationRadiusScope
{
    private static IEnumerable<MethodBase> TargetMethods() => typeof(Board).GetMethods()
        .Where(method => method.Name == nameof(Board.KillAllPlantsInRadius));
    internal static void Prefix(Board __instance, out IntPtr __state)
    { __state = RetaliationNuts.RadiusBoard; RetaliationNuts.RadiusBoard = __instance.Pointer; }
    internal static void Finalizer(IntPtr __state) => RetaliationNuts.RadiusBoard = __state;
}

[HarmonyPatch]
internal static class IronSquashExplosionScope
{
    // UpdateActions inlines the jack routine, including KillAllPlantsInRadius.
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateZombieJackInTheBox)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateActions)) };
    internal static void Prefix(Zombie __instance, out IntPtr __state)
    { __state = RetaliationNuts.JackBoard; RetaliationNuts.JackBoard = __instance.mZombieType == ZombieType.JackInTheBox
        ? __instance.mBoard?.Pointer ?? IntPtr.Zero : IntPtr.Zero; }
    internal static void Finalizer(IntPtr __state) => RetaliationNuts.JackBoard = __state;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class RetaliationSquishScope
{
    internal static void Prefix(Plant __instance, out RetaliationNuts.Hit? __state) =>
        __state = FusionPlant.From(__instance) == FusionPlant.StarNut ? RetaliationNuts.Capture(__instance) : null;
    internal static void Postfix(Plant __instance, RetaliationNuts.Hit? __state)
    {
        if (__state != null && __instance.DataID == __state.Id
            && __instance.mBoard?.Pointer == __state.Board.Pointer && __instance.mSquished)
            RetaliationProjectiles.Stars(__state);
    }
}

[HarmonyPatch]
internal static class StarNutFireScope
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateZombieJalapenoHead)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateActions)) };
    internal static void Prefix(Zombie __instance, out IntPtr __state)
    {
        __state = RetaliationNuts.FireBoard;
        RetaliationNuts.FireBoard = __instance.mZombieType == ZombieType.JalapenoHead
            ? __instance.mBoard?.Pointer ?? IntPtr.Zero : IntPtr.Zero;
    }
    internal static void Finalizer(IntPtr __state) => RetaliationNuts.FireBoard = __state;
}
