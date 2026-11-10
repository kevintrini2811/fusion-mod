// Native boundary declarations only; this suite compiles the production clock,
// registered identities, typed native gate and custom kelp owners unchanged.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute
    { public HarmonyPriority(int value) {} }
    public static class Priority { public const int First = 800, Last = 0; }
}
namespace Il2CppReloaded
{
    public static partial class Constants
    { public static partial class Gameplay { public static double TIME_FOR_UPDATE = .01; } }
}
namespace Il2CppReloaded.Gameplay
{
    public sealed partial class Board
    {
        public void Serialize() {} public void UpdateGame() {} public void InitLevel() {}
        public void InitSurvivalStage() {} public void DisposeBoard() {}
    }
    public sealed partial class Plant
    {
        private static uint next = 10;
        public int mShootingCounter;
        public IntPtr Pointer => (IntPtr)DataID;
        public Plant() { DataID = ++next; }
        public void UpdateShooter() {} public void UpdateProductionPlant() {}
        public void UpdateChomper() {} public void UpdateScaredyShroom() {}
    }
    public sealed partial class Zombie { public IntPtr Pointer => (IntPtr)DataID; }
    public sealed partial class Projectile { public IntPtr Pointer => (IntPtr)DataID; }
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
    internal static partial class ScaredContent
    {
        internal static bool Playing(Il2CppReloaded.Gameplay.Board? board) => board != null
            && board.mApp.GameScene == Il2CppReloaded.Gameplay.GameScenes.Playing;
    }
}

namespace Il2CppReloaded.TreeStateActivities
{
    public sealed class GameplayActivity { public void CreatePlantController() {} }
}

namespace Il2CppReloaded.Gameplay { public sealed partial class GridItem { public float mGoalX, mGoalY; } }
namespace FusionMod { internal sealed class FusionMod { internal static readonly FusionMod Instance=new(); internal readonly Logger LoggerInstance=new(); internal sealed class Logger { internal void Error(string message) {} } } }
