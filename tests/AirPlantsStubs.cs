using Il2CppReloaded.Gameplay;
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute { public HarmonyPatch(Type type, string method) { } }
}
namespace Il2CppReloaded.Gameplay
{
    public enum SeedType { None, Peashooter, Blover, Scaredyshroom, Pumpkinshell, Flowerpot, Lilypad, InstantCoffee, Seashroom, Chomper }
    public enum PlantingReason { Ok, NotHere, NeedsPot, NotOnWater }
    public sealed class Zombie { public bool CanTargetPlant(Plant plant) => true; }
    public sealed class PlantsOnLawn { public Plant? NormalPlant, FlyingPlant; }
    public sealed class Plant
    {
        private static int next;
        public IntPtr Pointer = (IntPtr)(++next);
        public SeedType mSeedType;
        public bool SquishAirborne;
        public bool Camo, Fly, mDead, mIsOnBoard = true;
        public int mPlantCol, mRow, mRenderOrder, mShadowRenderOrder;
        public float mX, mY;
        public static bool IsFlying(SeedType type) => type == SeedType.InstantCoffee;
        public int CalcRenderOrder() => mRow * 100;
        public void UpdateReanim() { }
    }
    public sealed class Board
    {
        public List<Plant> m_plants = new();
        public PlantingReason Rule;
        public bool ThrowTerrain;
        public bool Water;
        public bool IsPoolSquare(int col, int row) => Water;
        public SeedType QueriedType;
        public int GetNumRows() => 6;
        public float GridToPixelX(int col, int row) => col * 80;
        public float GridToPixelY(int col, int row) => row * 100;
        public PlantsOnLawn GetPlantsOnLawn(int col, int row) => new();
        public PlantingReason CanPlantAt(int col, int row, SeedType type)
        {
            QueriedType = type;
            if (ThrowTerrain) throw new InvalidOperationException("terrain");
            if (Rule != PlantingReason.Ok) return Rule;
            var occupants = m_plants.Where(p => p.mIsOnBoard && !p.mDead && p.mPlantCol == col && p.mRow == row);
            return occupants.Any()
                ? PlantingReason.NotHere : PlantingReason.Ok;
        }
    }
}
namespace LawnGlove
{
    public static class LawnGloveMod { public static Plant? GetCarriedPlant(Board board) => null; }
}
namespace FusionMod
{
    internal static class Squish { internal static bool Airborne(Plant p) => p.SquishAirborne; }
    internal readonly record struct FusionPlant(SeedType Type, bool Camo = false, bool Fly = false)
    {
        internal SeedType NativeType => Type;
        internal static FusionPlant CamoShroom = new(SeedType.Scaredyshroom, true);
        internal static FusionPlant Flytrapper = new(SeedType.Chomper, false, true);
        internal static FusionPlant From(Plant plant) => new(plant.mSeedType, plant.Camo, plant.Fly);
        public static implicit operator FusionPlant(SeedType type) => new(type);
    }
    internal static class ShroomSlots
    {
        internal static List<Plant> Plants(Board board, int col, int row) => board.m_plants
            .Where(p => p.mPlantCol == col && p.mRow == row && Live(p) && !AirPlants.Owns(p)
                && p.mSeedType is not (SeedType.Pumpkinshell or SeedType.Flowerpot or SeedType.Lilypad or SeedType.InstantCoffee)).ToList();
        internal static bool Live(Plant plant) => !plant.mDead && plant.mIsOnBoard;
        internal static PlantingReason CheckTerrain(Board board, int col, int row, SeedType type, IEnumerable<Plant> hidden)
        {
            var states = hidden.Select(p => (Plant: p, OnBoard: p.mIsOnBoard)).ToArray();
            try
            {
                foreach (var state in states) state.Plant.mIsOnBoard = false;
                return board.CanPlantAt(col, row, type);
            }
            finally { foreach (var state in states) state.Plant.mIsOnBoard = state.OnBoard; }
        }
    }
    internal static class FusionService
    {
        internal static bool CheckingNativePlacement;
        internal static List<int> ResolveAll(Board board, FusionPlant type, int col, int row, Plant? source = null) => new();
    }
}
