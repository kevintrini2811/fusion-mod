using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Fog remains owned by native Plantern. Saved grid records remember the full
// plant/zombie handles already affected by Aurora's once-per-target freeze.
internal static class PlanternFusions
{
    private const int FreezeMarker = -26138;
    internal static bool Owns(FusionPlant identity) => identity == FusionPlant.Triplantern
        || identity == FusionPlant.AuroraPlantern || identity == FusionPlant.BlacklightPlantern;
    internal static bool IsMemory(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == FreezeMarker;
    internal static void Initialize(Plant plant)
    {
        if (!Owns(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mTargetY = 100;
    }
    internal static void MemoryTick(GridItem item)
    {
        if (item.mBoard.mPaused) return;
        bool plant = false, zombie = false;
        for (int i = 0; i < item.mBoard.m_plants.Count; ++i)
            if (item.mBoard.m_plants[i] is { } p && p.DataID == unchecked((uint)item.mGridX)
                && !p.mDead && FusionPlant.From(p) == FusionPlant.AuroraPlantern) { plant = true; break; }
        for (int i = 0; i < item.mBoard.m_zombies.Count; ++i)
            if (item.mBoard.m_zombies[i] is { } z && z.DataID == unchecked((uint)item.mGridY)
                && !z.mDead && !z.IsDeadOrDying()) { zombie = true; break; }
        if (!plant || !zombie) item.GridItemDie();
    }
    private static bool Remember(Plant plant, Zombie zombie)
    {
        var board = plant.mBoard;
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { } item && !item.mDead && IsMemory(item)
                && item.mGridX == unchecked((int)plant.DataID) && item.mGridY == unchecked((int)zombie.DataID)) return false;
        var memory = board.m_gridItems.DataArrayAlloc();
        if (memory == null) return false;
        try
        {
            memory.Init(board.mApp, GridItemType.ZenTool);
            memory.mGridItemState = GridItemState.Normal;
            memory.mSunCount = FreezeMarker;
            memory.mGridX = unchecked((int)plant.DataID);
            memory.mGridY = unchecked((int)zombie.DataID);
            memory.mX = memory.mY = memory.mPosX = memory.mPosY = -1000;
            return true;
        }
        catch { memory.GridItemDie(); throw; }
    }
    internal static void Tick(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.AuroraPlantern || !BeamShooters.Active(plant)
            || plant.NotOnGround()) return;
        if (!FusionTime.SourceStep(plant, typeof(PlanternFusions))) return;
        if (plant.mTargetY > 0 && --plant.mTargetY > 0) return;
        plant.mTargetY = 100;
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        var hits = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || Math.Abs(zombie.mRow - plant.mRow) > 1
                || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Flying)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= 2.25f * FumeSupport.Unit * FumeSupport.Unit) hits.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in hits)
        {
            if (zombie.DataID != id || zombie.mDead || zombie.IsDeadOrDying()) continue;
            // 4.0.5.2 executable uses radius 1.5 and SetFreeze(5), unlike its
            // older almanac. Do not shorten an already longer freeze.
            if (zombie.mIceTrapCounter > 0 && Remember(plant, zombie))
                FusionStatuses.Minimum(zombie, FusionStatuses.Effect.Ice, 500);
            Chill(zombie);
            zombie.TakeDamage(20, DamageFlags.BypassesShield);
        }
    }
    private static void Chill(Zombie zombie)
    {
        FusionStatuses.Chill(zombie, sourceTicks: 150);
    }
    internal static IEnumerable<Plant> Crossed(Plant source, float endX)
    {
        for (int i = 0; i < source.mBoard.m_plants.Count; ++i)
        {
            var p = source.mBoard.m_plants[i];
            if (p != null && Owns(FusionPlant.From(p)) && BeamShooters.Active(p) && !p.NotOnGround()
                && p.mRow == source.mRow && p.mPlantCol > source.mPlantCol
                && p.mX + p.mWidth / 2f <= endX) yield return p;
        }
    }
    internal static void LaserHit(Plant source, Zombie zombie)
    {
        float endX = zombie.GetZombieRect().center.x;
        bool cold = false;
        int extraRays = 0;
        uint id = zombie.DataID;
        foreach (var p in Crossed(source, endX))
            if (FusionPlant.From(p) == FusionPlant.Triplantern) extraRays += 2;
            else if (FusionPlant.From(p) == FusionPlant.AuroraPlantern) cold = true;
        if (Crossed(source, endX).Any(p => FusionPlant.From(p) == FusionPlant.BlacklightPlantern))
        {
            var rect = zombie.GetZombieRect();
            var hits = new List<(Zombie Zombie, uint Id)>();
            for (int i = 0; i < source.mBoard.m_zombies.Count; ++i)
            {
                var target = source.mBoard.m_zombies[i];
                if (target == null || !SquashFusions.Eligible(source.mBoard, target, DamageRangeFlags.Ground)) continue;
                var bounds = target.GetZombieRect();
                float dx = bounds.center.x - rect.center.x, dy = bounds.center.y - rect.center.y;
                if (dx * dx + dy * dy <= FumeSupport.Unit * FumeSupport.Unit) hits.Add((target, target.DataID));
            }
            foreach (var (target, targetId) in hits)
                if (target.DataID == targetId && !target.mDead) target.TakeDamage(10, DamageFlags.BypassesShield);
            source.mBoard.mApp.AddTodParticle(rect.center.x, rect.center.y, zombie.mRenderOrder, ParticleEffect.PeaSplat);
        }
        if (cold && zombie.DataID == id && !zombie.mDead) Chill(zombie);
        for (int i = 0; i < extraRays && zombie.DataID == id && !zombie.mDead && !zombie.IsDeadOrDying(); ++i)
            zombie.TakeDamage(30, DamageFlags.BypassesShield);
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PlanternInitializePatch
{ private static void Postfix(Plant __instance) => PlanternFusions.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class PlanternAbilityPatch
{ private static void Postfix(Plant __instance) => PlanternFusions.Tick(__instance); }
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class PlanternMemoryTickPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(GridItem __instance)
    { if (!PlanternFusions.IsMemory(__instance)) return true; PlanternFusions.MemoryTick(__instance); return false; }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class PlanternMemoryDrawPatch
{ private static bool Prefix(GridItem __instance) => !PlanternFusions.IsMemory(__instance); }
