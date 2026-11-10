using Il2CppReloaded.Gameplay;
namespace FusionMod;
// Unrelated suites isolate pumpkin events; PumpkinSupportTests compiles their owner.
internal static class PumpkinSupport
{
    internal static void FireEnergy(Board board, int row, int damage) { }
    internal static void HeatedDeath(Board board, int row) { }
    internal static bool StoreIce(Board board, int col, int row) => false;
}
