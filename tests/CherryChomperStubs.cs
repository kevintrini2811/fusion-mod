// Minimal offline doubles. No Unity, IL2CPP, MelonLoader or game code is loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HarmonyPatch(Type type, string method) : Attribute
    {
        public Type TargetType { get; } = type;
        public string TargetMethod { get; } = method;
    }
}

namespace Il2CppReloaded.Gameplay
{
    public enum PlantState { Ready, ChomperBiting, ChomperBitingGotOne, ChomperBitingMissed, ChomperDigesting, ChomperSwallowing }
    public enum PlantOnBungeeState { NotOnBungee, OnBungee }
    public enum GameScenes { Playing, Loading }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public sealed class App
    {
        public GameScenes GameScene = GameScenes.Playing;
        public GameMode GameMode = GameMode.Adventure;
    }
    public sealed class Board
    {
        public IntPtr Pointer = (IntPtr)1;
        public App? mApp = new();
    }
    public sealed class Plant
    {
        public PlantState mState;
        public bool Special = true, mDead, mIsOnBoard = true, mSquished, OffGround;
        public PlantOnBungeeState mOnBungeeState;
        public Board? mBoard = new();
        public uint DataID = 7;
        public int mPlantCol = 2, mRow = 1, mWidth = 80, mHeight = 80, mRenderOrder = 100;
        public float mX = 160, mY = 80;
        public bool NotOnGround() => OffGround;
        public void UpdateAbilities() { }
        public void UpdateChomper() { }
    }
}

namespace FusionMod
{
    using Il2CppReloaded.Gameplay;
    internal readonly record struct FusionPlant(bool Special)
    {
        internal static FusionPlant CherryChomper => new(true);
        internal static FusionPlant From(Plant plant) => new(plant.Special);
    }
    internal static class CherryBombPlants
    {
        internal sealed record Death(Board Board, FusionPlant Identity, uint Id,
            int Col, int Row, float X, float Y, int Order);
        internal static IntPtr DisposingBoard;
        internal static readonly List<Death> Explosions = new();
        internal static void Explode(Death death) => Explosions.Add(death);
    }
}
