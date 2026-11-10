// Other family suites isolate this support collaborator. ShootingPotsTests
// compiles the production implementation and verifies its own state contract.
using Il2CppReloaded.Gameplay;
namespace FusionMod;
internal static partial class ShootingPots
{
    internal static int ThreeDamage(Plant plant, int damage) => damage;
    internal static int ProjectileDamage(object shot, int damage) => damage;
}
