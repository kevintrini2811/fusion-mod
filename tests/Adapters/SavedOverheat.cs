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
}
