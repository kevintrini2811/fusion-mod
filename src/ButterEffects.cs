using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Let native ApplyButter own immunity, visuals and interrupted zombie actions.
// Its ordinary four-second kernel duration is reduced to the source two seconds.
internal static class ButterEffects
{
    internal static void Apply(Zombie zombie, int duration = 200)
    {
        FusionStatuses.Butter(zombie, duration);
    }
}
