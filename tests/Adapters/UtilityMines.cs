using Il2CppReloaded.Gameplay;
namespace FusionMod;
// Feeding suites isolate the utility mine mechanics in MineFusionsTests.
internal static class UtilityMines
{
    internal static void Fertilize(Plant plant) { }
}

// Mine fertilizer behavior is covered by the real debug-board mine suite.
internal static class TreasureMines
{
    internal static void Fertilize(Plant plant) { }
}
