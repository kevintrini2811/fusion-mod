// Native scalar/container declarations for the real FusionNativeTimeRecords owner.
// These doubles do not simulate native serialization or replace the source clock.
namespace Il2CppReloaded.Gameplay
{
    public sealed partial class Board
    {
        public readonly ClockGridItems m_gridItems = new();
        public void Serialize() {}
    }
    public enum GridItemType { ZenTool }
    public enum GridItemState { Normal }
    public sealed class ClockGridItems : List<GridItem>
    {
        public GridItem DataArrayAlloc()
        { var item = new GridItem(); Add(item); return item; }
    }
    public sealed class GridItem
    {
        private static uint next;
        public uint DataID = ++next;
        public bool mDead;
        public GridItemType mGridItemType;
        public GridItemState mGridItemState;
        public int mGridX, mGridY, mGridItemCounter, mSunCount, mX, mY;
        public float mPosX, mPosY, mGoalX, mGoalY;
        public void Init(object app, GridItemType type) { mGridItemType = type; }
        public void GridItemDie() { mDead = true; }
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
