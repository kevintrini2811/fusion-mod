// Other family suites isolate this collaborator; ActiveNutsTests owns its contract.
using Il2CppReloaded.Gameplay;
namespace FusionMod;
internal static class ActiveNuts
{
    internal static int HealLimit(Plant plant) => plant.mPlantMaxHealth;
}
