// Family suites isolate the new collaborator. DefensivePotsTests compiles its
// actual implementation and checks the effects independently.
using Il2CppReloaded.Gameplay;
namespace FusionMod;
internal static class ReactivePots
{
    internal static void Heal(Plant plant, int amount) => plant.mPlantHealth = Math.Min(plant.mPlantMaxHealth, plant.mPlantHealth + amount);
    internal static void Cherry(Board board, float x, float y, int row, float radius) { }
    internal static void Fire(Board board, int row) { }
    internal static bool PreventCrater(Board board, int col, int row) => false;
}
