using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Scaredy owns sleep, ground placement, saves and fear poses. Saved mTargetX
// owns the current attack interval, mTargetY the active action's remaining time.
internal static class FrenzyShroom
{
    internal static int ShootTicks(Plant plant) => Math.Min(50, Math.Max(20, plant.mTargetX));

    internal static bool HasTarget(Plant plant)
    {
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && zombie.mRow == plant.mRow
                && FumeFusions.InRange(plant, zombie, FusionPlant.FumeSpreader)) return true;
        return false;
    }

    internal static void Tick(Plant plant)
    {
        if (!ScaredyFusions.Active(plant)) return;
        bool near = false;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && ScaredyFusions.Near(plant, zombie)) { near = true; break; }
        // GetNearZombies invokes ScaredEvent immediately, independently of the
        // scared animation. A passing adjacent-lane zombie must not outrun it.
        if (near && plant.mTargetX <= 20)
        {
            var death = IceInstantFusions.Capture(plant);
            plant.Die();
            if (plant.mDead && plant.DataID == death.Id && plant.mBoard?.Pointer == death.Board.Pointer)
            {
                DoomEffects.Damage(death.Board, death.Row, death.X, death.Y, death.Order);
                DoomEffects.LeaveCrater(death.Board, death.Col, death.Row);
            }
            return;
        }
        if (near) plant.mTargetX = 150;
        if (near && !ScaredyFusions.Afraid(plant))
        {
            plant.mState = PlantState.ScaredyshroomLowering;
            plant.mTargetY = 100;
            plant.mStartRow = 0;
        }
        if (!near && ScaredyFusions.Afraid(plant))
        {
            plant.mState = PlantState.ScaredyshroomRaising;
            plant.mTargetY = 100;
        }
        if (plant.mState == PlantState.ScaredyshroomLowering)
        {
            if (--plant.mTargetY <= 0) plant.mState = PlantState.ScaredyshroomScared;
            return;
        }
        if (plant.mState == PlantState.ScaredyshroomScared) return;
        if (plant.mState == PlantState.ScaredyshroomRaising)
        {
            if (--plant.mTargetY <= 0) plant.mState = PlantState.Notready;
            return;
        }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || !HasTarget(plant)) return;
            plant.mStartRow = ShootTicks(plant);
            plant.mTargetY = plant.mStartRow; // Snapshot duration before Shoot1 shortens interval.
            plant.mLaunchCounter = plant.mTargetX;
        }
        int elapsed = plant.mTargetY - plant.mStartRow;
        if (elapsed == plant.mTargetY * 3 / 5)
        {
            plant.mTargetX = Math.Max(20, plant.mTargetX - 10);
            // Source synchronizes attackDamage with DoomPeashooter's base and
            // permanent bonus in the immediately adjacent right-hand cell.
            var right = plant.mPlantCol < 8
                ? plant.mBoard.GetPlantsOnLawn(plant.mPlantCol + 1, plant.mRow).NormalPlant : null;
            if (right != null && ChomperFusions.Active(right) && FusionPlant.From(right) == FusionPlant.DoomPea)
                plant.mShootingCounter = 40 + Math.Clamp(right.mTargetX, 0, 60);
            plant.mLaunchCounter = Math.Max(1, plant.mTargetX - elapsed);
            var muzzle = ImportedPeaVisuals.ScaredyMuzzle(plant);
            var shot = plant.mBoard.AddProjectile(muzzle.X + .1f * ScaredyFusions.SourceUnit,
                muzzle.Y, plant.mRenderOrder + 1, plant.mRow, ProjectileType.Pea);
            if (shot != null && !shot.mDead)
                ScaredyProjectiles.Mark(shot, FusionPlant.FrenzyShroom, plant.mShootingCounter);
        }
        --plant.mStartRow;
    }
}
