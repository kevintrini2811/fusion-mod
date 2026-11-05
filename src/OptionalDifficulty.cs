using System.Reflection;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Inspect only an already-loaded optional mod. Never load its DLL or retain
// a hard assembly reference; absent/older versions retain the 1% base rate.
internal static class OptionalDifficulty
{
    private const BindingFlags Flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    internal static int EyesChance(Board board)
    {
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == "DifficultyMod")?.GetType("DifficultyMod.Mod");
        if (type?.GetField("Instance", Flags)?.GetValue(null) is not { } mod) return 1;
        if (type.GetField("InRun", Flags)?.GetValue(mod) is not true
            || type.GetField("App", Flags)?.GetValue(mod) is not Il2CppReloaded.TreeStateActivities.GameplayActivity app
            || app.Pointer != board.mApp?.Pointer) return 1;
        return Convert.ToInt32(type.GetField("Active", Flags)?.GetValue(mod)) == 5 ? 33 : 1;
    }
}
