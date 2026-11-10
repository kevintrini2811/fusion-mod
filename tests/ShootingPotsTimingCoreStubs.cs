// Native boundaries only. The production FusionTime and ShootingPots owners
// are compiled unchanged; no clock double is included in this core suite.
namespace Il2CppReloaded.Constants
{
    public static class Gameplay { public static double TIME_FOR_UPDATE = .01; }
}
namespace Il2CppReloaded.Gameplay
{
    public sealed partial class Board
    {
        public void UpdateGame() {}
        public void InitLevel() {}
        public void InitSurvivalStage() {}
        public void DisposeBoard() {}
    }
}
namespace Il2CppReloaded.Services
{
    public sealed class SerializationService
    {
        public sealed class LoadedApp { public Il2CppReloaded.Gameplay.Board? Board; }
        public LoadedApp? App;
        public bool Deserialize() => true;
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal static class ScaredContent
    { internal static bool Playing(Board board) => board.mApp.GameScene == GameScenes.Playing; }
    internal static class CattailFusions
    { internal static Plant? Firing; internal static void Track(Projectile shot) {} }
    internal static class TriLobberNativeFirePatch
    { internal static bool Prefix(Plant plant) => true; }
}
