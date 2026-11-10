using Il2CppReloaded.Gameplay;
namespace Il2CppReloaded.Gameplay;
public enum GridItemType { ZenTool, Crater }
public enum GridItemState { Normal }
public sealed class GridPool : List<GridItem>
{
    private readonly Board board;
    public bool Full;
    public GridPool(Board value) => board = value;
    public GridItem? DataArrayAlloc()
    { if (Full) return null; var item = new GridItem { mBoard = board }; Add(item); return item; }
}
public sealed class GridItem
{
    public Board mBoard = null!;
    public bool mDead;
    public GridItemType mGridItemType;
    public GridItemState mGridItemState;
    public int mSunCount, mGridX, mGridY, mGridItemCounter;
    public float mX, mY, mPosX, mPosY;
    public void Init(App app, GridItemType type) => mGridItemType = type;
    public void GridItemDie() => mDead = true;
    public void Update() { }
    public void DrawGridItem() { }
}
