using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class ThreetatoMine
{
    internal const int RiseTicks = 200;
    internal static int Interval(float distance) => (int)MathF.Round((.3f + 1.2f * Math.Clamp(distance / (10 * ScaredyFusions.SourceUnit), 0, 1)) * 100);
    internal static void Tick(Plant plant)
    {
        if (!MineFusions.Active(plant)) return;
        if (plant.mStateCountdown > 0) --plant.mStateCountdown;
        if (plant.mTargetY > 0) --plant.mTargetY;
        if (plant.mLaunchRate > 0 && --plant.mLaunchRate == 0)
            PotatoProjectiles.Spawn(plant.mBoard, plant.mX + plant.mWidth / 2f, plant.mY + 20,
                plant.mRow, plant.mRenderOrder, 0, plant.mRow);
        if (plant.mShootingCounter > 0 && --plant.mShootingCounter == (int)MathF.Round(plant.mTargetX * .375f))
            Shoot(plant);
        if (plant.mState == PlantState.Notready && plant.mStateCountdown == 0)
        { plant.mState = PlantState.PotatoRising; plant.mStateCountdown = RiseTicks; plant.mStartRow = 0; }
        else if (plant.mState == PlantState.PotatoRising && plant.mStateCountdown == 0) plant.mState = PlantState.PotatoArmed;
        if (plant.mState != PlantState.PotatoArmed) return;
        var contact = MineFusions.Nearest(plant, true);
        if (contact != null && plant.mTargetY == 0 && contact.mZombieType is not (ZombieType.Zamboni or ZombieType.Catapult))
        {
            plant.mTargetY = 50;
            plant.mState = PlantState.Notready; plant.mStateCountdown = 1500;
            plant.mShootingCounter = plant.mLaunchRate = 0; plant.mStartRow = 1;
            Explode(plant);
            return;
        }
        var target = MineFusions.Nearest(plant, false);
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        bool any = target != null;
        if (!any)
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            {
                var z = plant.mBoard.m_zombies[i];
                if (z != null && Math.Abs(z.mRow - plant.mRow) <= 1 && !z.mMindControlled
                    && SquashFusions.Eligible(plant.mBoard, z, DamageRangeFlags.Ground)
                    && z.GetZombieRect().xMax >= plant.mX + plant.mWidth / 2f) { any = true; break; }
            }
        if (!any) return;
        int interval = target == null ? 150 : Interval(target.mPosX - plant.mX - plant.mWidth / 2f);
        plant.mLaunchCounter = Math.Min(plant.mLaunchCounter, interval);
        if (plant.mLaunchCounter > 0 || plant.mShootingCounter > 0) return;
        plant.mLaunchCounter = interval;
        plant.mTargetX = interval;
        plant.mShootingCounter = interval;
    }
    private static void Shoot(Plant plant)
    {
        for (int row = Math.Max(0, plant.mRow - 1); row <= Math.Min(plant.mBoard.GetNumRows() - 1, plant.mRow + 1); ++row)
        {
            float y = plant.mY + 20;
            PotatoProjectiles.Spawn(plant.mBoard, plant.mX + plant.mWidth / 2f, y, row, plant.mRenderOrder, 0, plant.mRow);
        }
        // Inherited ThreePeater.ExtraBullet returns a missing edge lane to
        // the own lane after 0.2 seconds. The saved clock cannot outlive burial.
        if (plant.mRow == 0 || plant.mRow == plant.mBoard.GetNumRows() - 1) plant.mLaunchRate = 20;
    }
    internal static void Explode(Plant plant)
    {
        var board = plant.mBoard;
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { mDead: false } z && z.mRow == plant.mRow && !z.mMindControlled
                && z.EffectedByDamage(MineFusions.MineTargets))
            {
                var r = z.GetZombieRect();
                if (Math.Abs(x - Math.Clamp(x, r.xMin, r.xMax)) <= ScaredyFusions.SourceUnit) targets.Add((z, z.DataID));
            }
        MineFusions.Blast(board, x, y, ScaredyFusions.SourceUnit, 5400, plant.mRenderOrder, row: plant.mRow);
        foreach (var (z, id) in targets)
            if (z.DataID == id && z.mBoard?.Pointer == board.Pointer && !z.mDead && !z.IsDeadOrDying()
                && z.mZombieType is not (ZombieType.Boss or ZombieType.Zamboni or ZombieType.Catapult))
            { z.mPosX = Math.Min(850, z.mPosX + 80); z.mX = (int)z.mPosX; z.StopEating(); }
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class ThreetatoCrushPatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal || FusionPlant.From(__instance) != FusionPlant.ThreetatoMine || !MineFusions.Active(__instance)
            || __instance.mState != PlantState.PotatoArmed) return;
        __instance.mState = PlantState.Notready;
        ThreetatoMine.Explode(__instance);
    }
}
