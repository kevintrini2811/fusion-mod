// Only external boundaries are doubled. Native bodies below are explicit
// counter/state models; actual native animation and bite execution need the game.
namespace Il2CppReloaded.Constants
{ public static class Gameplay { public static double TIME_FOR_UPDATE = .01; } }
namespace Il2CppReloaded.Gameplay
{
    public sealed partial class Board
    {
        public App mApp => App;
        public void UpdateGame() {} public void InitLevel() {}
        public void InitSurvivalStage() {} public void DisposeBoard() {}
    }
    public sealed partial class Plant
    {
        public void UpdateShooter() {} public void UpdateShooting() {}
        public void UpdateProductionPlant() {} public void UpdateTanglekelp() {}
        public void UpdateScaredyShroom() {}
    }
    public sealed class Projectile
    { public Board mBoard = null!; public IntPtr Pointer; public uint DataID; }
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
    public readonly partial record struct FusionPlant
    {
        internal static readonly FusionPlant Torchweed = new(90), RoastedSpikeweed = new(91);
        internal static bool HasImportedVisuals(FusionPlant identity) => identity != Other;
        internal static bool IsPuffJumper(FusionPlant identity) => false;
    }
    internal static class ScaredContent
    {
        internal static bool Playing(Il2CppReloaded.Gameplay.Board? board) => board != null
            && board.mApp.GameScene == Il2CppReloaded.Gameplay.GameScenes.Playing;
    }
    internal static class SeaPot { internal static bool AdvancingNative; }
}
