using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class TriggerShroom
{
    private static bool Eligible(Plant plant, Projectile shot) => !shot.mDead
        && shot.mBoard?.Pointer == plant.mBoard.Pointer && shot.mRow == plant.mRow
        && shot.mMotionType == ProjectileMotion.Straight
        && CherryShooter.TryShot(shot) && !CherryShooter.IsExplosive(shot)
        && shot.mPosX >= plant.mX + plant.mWidth / 2f
        && shot.mPosX <= plant.mX + plant.mWidth / 2f + 7f * ScaredyFusions.SourceUnit;

    internal static bool HasCherry(Plant plant)
    {
        for (int i = 0; i < plant.mBoard.m_projectiles.Count; ++i)
            if (plant.mBoard.m_projectiles[i] is { } shot && Eligible(plant, shot)) return true;
        return false;
    }

    internal static void Convert(Plant plant)
    {
        int count = 0;
        for (int i = 0; i < plant.mBoard.m_projectiles.Count; ++i)
        {
            var shot = plant.mBoard.m_projectiles[i];
            if (shot == null || !Eligible(plant, shot)) continue;
            CherryShooter.Catalyze(shot);
            if (++count == 4) break;
        }
    }
}
