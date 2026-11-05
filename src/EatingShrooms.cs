using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Fume/scaredy own the shooter and fear phases. Native saved fields store
// digestion and the independent chomp clip; loading never repeats a swallow.
internal static class EatingShrooms
{
    internal const int DigestionTicks = 4000, ChompTicks = 75;
    internal static void Fertilize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if ((identity == FusionPlant.BelchShroom || identity == FusionPlant.HungryShroom) && plant.mTargetX > 50)
            plant.mTargetX = 50;
    }

    internal static void TickBelch(Plant plant)
    {
        if (plant.mTargetX > 0) --plant.mTargetX;
        if (plant.mTargetY > 0)
        {
            // Original chomp animation owns the bite event. Reacquire there,
            // rather than retaining an unsafe pointer across simulation ticks.
            if (plant.mTargetY == 20) SwallowBelch(plant);
            --plant.mTargetY;
        }
        else if (plant.mTargetX == 0 && BelchTarget(plant) != null) plant.mTargetY = ChompTicks;
    }

    private static Zombie? BelchTarget(Plant plant)
    {
        Zombie? target = null;
        float nearest = float.MaxValue;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || !ChomperVariants.CanSwallow(zombie)
                || !FumeVariants.Target(plant, zombie, 2 * ScaredyFusions.SourceUnit)) continue;
            float x = zombie.GetZombieRect().center.x;
            if (x < nearest) { nearest = x; target = zombie; }
        }
        return target;
    }

    private static void SwallowBelch(Plant plant)
    {
        var zombie = BelchTarget(plant);
        if (zombie == null) return;
        // Commit the plant timer before death/loot callbacks can reuse pools.
        plant.mTargetX = DigestionTicks;
        var board = plant.mBoard;
        zombie.DieWithLoot();
        board.mApp.SoundSystem.PlayFoley(FoleyType.Chomp);
    }

    internal static void Fear(Plant plant)
    {
        if (plant.mTargetX > 0) return;
        // Source charges the cooldown even if none of the three attempts eat.
        plant.mTargetX = DigestionTicks;
        var board = plant.mBoard;
        uint plantId = plant.DataID;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && ChomperVariants.CanSwallow(zombie)
                && ScaredyFusions.Near(plant, zombie, 1.5f)) targets.Add((zombie, zombie.DataID));
        int eaten = 0;
        foreach (var (zombie, id) in targets)
        {
            if (plant.DataID != plantId || !ScaredyFusions.Active(plant)) return;
            if (zombie.DataID != id || !ChomperVariants.CanSwallow(zombie)
                || !ScaredyFusions.Near(plant, zombie, 1.5f)) continue;
            zombie.DieWithLoot();
            if (++eaten == 3) break;
        }
        board.mApp.SoundSystem.PlayFoley(FoleyType.Chomp);
    }

    internal static void Knockback(Zombie zombie)
    {
        if (zombie.mZombieType is ZombieType.Boss or ZombieType.Zamboni or ZombieType.Catapult
            or ZombieType.Gargantuar or ZombieType.RedeyeGargantuar) return;
        zombie.mPosX = Math.Min(850, zombie.mPosX + .2f * ScaredyFusions.SourceUnit);
        zombie.mX = (int)zombie.mPosX;
    }

    internal static void ShootHungry(Plant plant)
    {
        plant.mLaunchCounter = 120;
        if (plant.mTargetX > 0) RollingProjectiles.Spawn(plant, false, plant.mBoard.Rand(2) + 1, hungry: true);
        else
        {
            var muzzle = ImportedPeaVisuals.ScaredyMuzzle(plant);
            var shot = plant.mBoard.AddProjectile(muzzle.X, muzzle.Y,
                plant.mRenderOrder + 1, plant.mRow, ProjectileType.Pea);
            if (shot != null && !shot.mDead) ScaredyProjectiles.Mark(shot, FusionPlant.HungryShroom, 40);
        }
    }
}
