using Il2CppReloaded.Gameplay;

namespace UnityEngine { internal static class OfflineNamespace { } }
namespace Il2CppReloaded.TreeStateActivities { public sealed class GameplayActivity { } }
namespace ReplantedModUi
{
    // Preserve the actual optional binding boundary while controlling presence
    // and session ownership. The production DebugIntegration source is compiled.
    internal static class OptionalModApi
    {
        internal static Board? SessionBoard;
        internal static bool Present => !Environment.GetCommandLineArgs().Contains("absent");
        internal static T? Bind<T>(string assembly, string owner, string name) where T : Delegate
        {
            if (!Present) return null;
            if (typeof(T) == typeof(Func<Board?, bool>))
                return (T)(Delegate)new Func<Board?, bool>(board => board != null
                    && SessionBoard != null && board.Pointer == SessionBoard.Pointer);
            return null;
        }
    }
}
namespace FusionMod
{
    internal static class DebugPlantCards
    {
        internal static bool Place(Board board, SeedType type, SeedType marker, bool ingredient, int col, int row) => false;
    }
    internal static class FusionAlmanac
    {
        internal static IEnumerable<(FusionPlant Plant, string Name, string Portrait)> DebugCatalog(bool ingredients)
            => Array.Empty<(FusionPlant, string, string)>();
    }
    internal static class PlantValue
    {
        internal static Dictionary<FusionPlant, int> Calculate(Board board) => new();
    }
}
