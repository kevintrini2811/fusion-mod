// Other suites isolate aquatic behavior. StarFusionsTests compiles its owner.
using Il2CppReloaded.Gameplay;
namespace FusionMod;
internal static class SeaFusions
{
    internal static bool CanFeed(Plant plant, FusionPlant incoming) => false;
    internal static void Feed(Plant plant) { }
    internal static void Fertilize(Plant plant) { }
}

internal static class SeaSupportFusions
{
    internal static void Fertilize(Plant plant) { }
}
