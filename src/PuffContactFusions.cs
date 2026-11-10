using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Puff-shroom owns terrain, sleep and three subfields. All clocks stay in
// serialized native fields; the imported animation never runs source scripts.
internal static class PuffContactFusions
{
    internal const int BallShootTicks = 50;
    // Only collider 124534 enables callbacks; the wide body collider does not.
    internal const float CherryContactOffset = .78008616f * .15f * ScaredyFusions.SourceUnit;
    internal const float CherryContactHalfWidth = 1.7313302f * .15f * .5f * ScaredyFusions.SourceUnit;
    internal static void Initialize(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.BallCactus) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchRate = 100;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = 0;
        plant.mTargetX = plant.mTargetY = 0;
    }

    internal static bool BallTarget(Plant plant, Zombie zombie)
    {
        if (zombie.mRow != plant.mRow || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground | DamageRangeFlags.Flying)) return false;
        var rect = zombie.GetZombieRect();
        return (rect.xMin + rect.xMax) / 2f > plant.mX + plant.mWidth / 2f;
    }

    internal static void TickBall(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (!FusionTime.SourceStep(plant, typeof(PuffContactFusions))) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0) return;
            bool target = false;
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
                if (plant.mBoard.m_zombies[i] is { } zombie && BallTarget(plant, zombie)) { target = true; break; }
            if (!target) return;
            plant.mStartRow = BallShootTicks;
            plant.mLaunchCounter = 95 + plant.mBoard.Rand(11);
        }
        // CactusPuff's original shoot clip calls Shoot1 at .25 seconds.
        if (plant.mStartRow == 25) CactusProjectiles.Spawn(plant);
        --plant.mStartRow;
    }

    internal static bool Contact(Plant plant, Zombie? bite = null)
    {
        if (FusionPlant.From(plant) != FusionPlant.CherryPop || !ChomperFusions.Active(plant) || plant.mIsAsleep) return false;
        if (bite != null)
        {
            if (!PuffSupport.GroundEnemy(plant.mBoard, bite, plant.mRow)) return false;
            plant.Die();
            return true;
        }
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || !PuffSupport.GroundEnemy(plant.mBoard, zombie, plant.mRow)) continue;
            var rect = zombie.GetZombieRect();
            float x = plant.mX + plant.mWidth / 2f + CherryContactOffset;
            if (Math.Abs(x - Math.Clamp(x, rect.xMin, rect.xMax)) > CherryContactHalfWidth) continue;
            plant.Die();
            return true;
        }
        return false;
    }

    internal static void Explode(CherryBombPlants.Death death)
    {
        var board = death.Board;
        var range = DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
            | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Dying | DamageRangeFlags.Underground;
        var candidates = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || !SquashFusions.Eligible(board, zombie, range)
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || Math.Abs(zombie.mRow - death.Row) > 1 || CherrySpreader.AttachedTo(board, zombie.DataID) != null) continue;
            var rect = zombie.GetZombieRect();
            float dx = death.X - Math.Clamp(death.X, rect.xMin, rect.xMax);
            float dy = death.Y - Math.Clamp(death.Y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= CherrySpreader.BlastRadius * CherrySpreader.BlastRadius)
                candidates.Add((zombie, zombie.DataID));
        }
        if (candidates.Count > 0)
        {
            var (target, id) = candidates[board.Rand(candidates.Count)];
            if (target.DataID == id) CherrySpreader.Attach(target, 900);
        }
        // Attach BEFORE cherry damage: the initial explosion also charges the
        // new bomb by damage/4. The shared cherry damage scope owns that rule.
        CherryBombPlants.Explode(death, sourceRadius: CherrySpreader.BlastRadius, damage: 900);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PuffContactInitializePatch
{ private static void Postfix(Plant __instance) => PuffContactFusions.Initialize(__instance); }

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class CherryPopContactPatch
{
    [HarmonyPriority(Priority.First + 200)]
    private static bool Prefix(Zombie __instance, Plant thePlant) =>
        thePlant == null || !PuffContactFusions.Contact(thePlant, __instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class CherryPopDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state)
    {
        __state = __runOriginal && FusionPlant.From(__instance) == FusionPlant.CherryPop
            && CherryBombPlants.CanTriggerDeath(__instance) && !__instance.NotOnGround()
            ? IceInstantFusions.Capture(__instance) : null;
    }
    private static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id
            && __instance.mBoard?.Pointer == __state.Board.Pointer) PuffContactFusions.Explode(__state);
    }
}
