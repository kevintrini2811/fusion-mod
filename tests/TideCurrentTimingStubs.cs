namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] internal sealed class HarmonyPatch : Attribute
    { internal HarmonyPatch(Type type, string name) {} }
    [AttributeUsage(AttributeTargets.Method)] internal sealed class HarmonyPriority : Attribute
    { internal HarmonyPriority(int priority) {} }
    internal static class Priority { internal const int First = 800, Last = 0; }
}
namespace Il2CppReloaded.Constants
{ internal static class Gameplay { internal static double TIME_FOR_UPDATE = .01; } }
namespace Il2CppReloaded.Gameplay
{
    internal sealed partial class Board
    {
        private static int next;
        internal IntPtr Pointer = (IntPtr)(++next); internal int mMainCounter;
        internal bool mPaused; internal List<Plant> m_plants = new(); internal List<Zombie> m_zombies = new();
        public void Serialize() {} public void UpdateGame() {} public void InitLevel() {} public void InitSurvivalStage() {} public void DisposeBoard() {}
    }
    internal class Plant
    {
        private static uint next;
        internal uint DataID = ++next; internal IntPtr Pointer => (IntPtr)DataID;
        internal Board mBoard = null!; internal int mRow, mTargetX, mStartRow;
        internal bool Tide = true, mDead, OffGround;
        internal bool NotOnGround() => OffGround;
    }
    internal sealed class Projectile : Plant {}
    internal sealed class Zombie : Plant
    {
        internal bool mInPool = true, mMindControlled, Backwards, Chilled;
        internal float mPosX = 500, mVelX, mX = 500; internal int mIceTrapCounter, mButteredCounter;
        public void UpdateZombieWalking() {}
    }
}
namespace Il2CppReloaded.Services
{
    internal sealed class SerializationService
    {
        internal sealed class LoadedApp { internal Il2CppReloaded.Gameplay.Board? Board; }
        internal LoadedApp? App; public bool Deserialize() => true;
    }
}
namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal readonly record struct FusionPlant(bool Tide)
    { internal static readonly FusionPlant TideShroom = new(true); internal static FusionPlant From(Plant plant) => new(plant.Tide); }
    internal static class BeamShooters
    { internal static bool Active(Plant plant) => !plant.mDead && !plant.mBoard.mPaused; }
    internal static class BloverFusions
    {
        internal static int Blows;
        internal static bool Live(Board board, Zombie zombie) => !zombie.mDead && zombie.mBoard == board;
        internal static void Blow(Plant plant) => ++Blows;
        internal static void Move(Zombie zombie, float amount) { zombie.mPosX += amount; zombie.mX = (int)zombie.mPosX; }
    }
    internal static class ScaredContent
    { internal static bool Playing(Board? board) => board != null; }
}

namespace Il2CppReloaded.Gameplay
{
    internal sealed partial class Board
    { internal readonly object mApp = new(); internal readonly GridItems m_gridItems = new(); }
    internal enum GridItemType { ZenTool }
    internal enum GridItemState { Normal }
    internal sealed class GridItems : List<GridItem>
    { internal GridItem DataArrayAlloc() { var item = new GridItem(); Add(item); return item; } }
    internal sealed class GridItem
    {
        private static uint next;
        internal uint DataID = ++next;
        internal bool mDead;
        internal GridItemType mGridItemType;
        internal GridItemState mGridItemState;
        internal int mGridX, mGridY, mGridItemCounter, mSunCount, mX, mY;
        internal float mPosX, mPosY, mGoalX, mGoalY;
        internal void Init(object app, GridItemType type) { mGridItemType = type; }
        internal void GridItemDie() { mDead = true; }
        public void Update() {} public void DrawGridItem() {}
    }
}
namespace FusionMod
{
    internal sealed class FusionMod
    {
        internal static readonly FusionMod Instance = new();
        internal readonly Logger LoggerInstance = new();
        internal sealed class Logger { internal void Error(string message) {} }
    }
}
