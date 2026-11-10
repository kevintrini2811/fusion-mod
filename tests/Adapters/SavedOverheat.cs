using Il2CppReloaded.Gameplay;
namespace FusionMod;
// Native item storage is exercised with its real owner in PumpkinSupportTests.
internal static class SavedOverheat
{
    private static readonly Dictionary<IntPtr, (IntPtr Board, uint Id)> effects = new();
    internal static object? Find(Zombie zombie) => effects.TryGetValue(zombie.Pointer, out var value)
        && value.Board == zombie.mBoard?.Pointer && value.Id == zombie.DataID ? zombie : null;
    internal static bool Add(Zombie zombie) { effects[zombie.Pointer] = (zombie.mBoard.Pointer, zombie.DataID); return true; }
    internal static void Remove(Zombie zombie) => effects.Remove(zombie.Pointer);
    // Simulate the native board disposing its serialized GridItems. Calling
    // Overheat.ForgetBoard only releases transient resources and keeps items.
    internal static void BoardDisposed(Board board)
    {
        foreach (var pointer in effects.Where(entry => entry.Value.Board == board.Pointer).Select(entry => entry.Key).ToArray())
            effects.Remove(pointer);
    }
}
