using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Only the unrelated plant activity predicate is isolated. The Football damage
// owner, public Zombie Library API, scopes and added-armor owner are production.
internal static class FumeFusions
{
    internal static bool Active(Plant plant) => !plant.mDead && !plant.mIsAsleep
        && plant.mIsOnBoard && !plant.mBoard.mPaused;
}
