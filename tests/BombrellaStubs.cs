using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class Bombrella
{
    internal static Plant? Guard(Plant plant) => null;
    internal static bool TryProtect(Plant plant) => false;
    internal static void Repel(Plant plant, Zombie zombie) { }
    internal static void ChargeFromCherryHeal(Plant plant, int previousHealth) { }
}

internal static partial class OrcaShroom
{
    internal static Plant? Launched;
    internal static void LaunchUmbrella(Plant plant) => Launched = plant;
}
