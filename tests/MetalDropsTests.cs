using System.Collections;
using System.Text.Json;
using FusionMod;
using Il2CppReloaded.Gameplay;
using ReplantedModUi;

int assertions = 0;
void Check(bool condition, string message)
{ ++assertions; if (!condition) throw new Exception(message); }
JsonElement Query(Board board) => JsonSerializer.SerializeToElement(DebugFusionIntegration.MetalDrops(board));
BucketDrops.Clear();
var board = new Board { Pointer = (IntPtr)101 };
var foreign = new Board { Pointer = (IntPtr)102 };
OptionalModApi.SessionBoard = board;
BucketDrops.Add(board, 1, 100, 200, FusionPlant.Bucket);
BucketDrops.Add(board, 2, 200, 300, FusionPlant.FootballHelmet);
BucketDrops.Add(board, 3, 300, 400, FusionPlant.EyesZombieHead);
BucketDrops.Add(foreign, 4, 400, 500, FusionPlant.Bucket);
var original = BucketDrops.Snapshot(board);
Check(original.Count == 3 && BucketDrops.Snapshot(foreign).Count == 1, "owner snapshots isolate boards");
Check(original[0].Identity == FusionPlant.Bucket && original[1].Identity == FusionPlant.FootballHelmet
    && original[2].Identity == FusionPlant.EyesZombieHead, "exact drop identities");
Check(original[0].Row == 1 && original[0].X == 100 && original[0].Y == 145 && original[0].Age == 0,
    "owner snapshots use displayed position and actual age");
Check(original is IList list && list.IsReadOnly, "owner snapshot collection cannot be changed");
Check(original.Select(drop => drop.SpawnX).SequenceEqual(new float?[] { 100, 200, 300 })
    && original.Select(drop => drop.SpawnGroundY).SequenceEqual(new float?[] { 200, 300, 400 }),
    "all fresh drop identities expose materialized origin, not raised display Y");
Check(typeof(BucketDrops.Drop).GetField("SpawnX", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.IsInitOnly
    && typeof(BucketDrops.Drop).GetField("SpawnGroundY", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.IsInitOnly,
    "owner origin fields are immutable");
int nativeItems = board.m_gridItems.Count;
int random = board.RandomCalls, count = BucketDrops.Count;
var beforeItem = board.m_gridItems.Single();
float height = beforeItem.mGoalX, velocity = beforeItem.mGoalY;
var first = Query(board);
for (int i = 0; i < 5; ++i) Query(board);
Check(board.m_gridItems.Count == nativeItems && board.RandomCalls == random && BucketDrops.Count == count && beforeItem.mGoalX == height
    && beforeItem.mGoalY == velocity && original.All(drop => drop.Age == 0),
    "queries do not randomize, tick, mutate persistence or consume drops");
if (OptionalModApi.Present)
{
    Check(first.GetArrayLength() == 3, "debug integration returns owned drops");
    foreach (var (index, identity) in new[] { (0, FusionPlant.Bucket), (1, FusionPlant.FootballHelmet), (2, FusionPlant.EyesZombieHead) })
    {
        var entry = first[index];
        Check(entry.GetProperty("type").GetString() == identity.NativeType.ToString()
            && entry.GetProperty("marker").GetString() == identity.Marker.ToString(), "wire identity names");
        Check(entry.EnumerateObject().Count() == 8, "wire contract adds only two nullable read-only origin fields");
    }
    Check(first[0].GetProperty("row").GetInt32() == 1 && first[0].GetProperty("x").GetSingle() == 100
        && first[0].GetProperty("y").GetSingle() == 145 && first[0].GetProperty("age").GetInt32() == 0,
        "serialized row, position and age");
    Check(first[0].GetProperty("spawnX").GetSingle() == 100
        && first[0].GetProperty("spawnGroundY").GetSingle() == 200,
        "wire origin is exact materialized ground coordinate");
}
else Check(first.GetArrayLength() == 0, "missing optional Debug Mod produces no data");
Check(Query(foreign).GetArrayLength() == 0, "foreign board cannot expose its drops through debug integration");
Check(DebugFusionIntegration.MetalDrops(null!).Length == 0 && BucketDrops.Snapshot(null).Count == 0,
    "missing board produces no data");
var app = board.mApp;
board.mApp = null!;
Check(BucketDrops.Snapshot(board).Count == 0 && Query(board).GetArrayLength() == 0, "missing board app is hidden");
board.mApp = app;
board.mPaused = true;
var paused = Query(board);
BucketDrops.Tick(board);
Check(paused.GetArrayLength() == (OptionalModApi.Present ? 3 : 0)
    && BucketDrops.Snapshot(board).All(drop => drop.Age == 0), "paused debug board remains observable without aging");
board.mPaused = false;
board.DropSlope = .25f;
BucketDrops.Tick(board);
Check(BucketDrops.Snapshot(board).All(drop => drop.Age == 1) && original.All(drop => drop.Age == 0),
    "materialized snapshots retain old values after real tick");
Check(first.GetArrayLength() == (OptionalModApi.Present ? 3 : 0), "wire result retains old materialization");
var moved = BucketDrops.Snapshot(board);
Check(moved[0].X != original[0].X && moved[0].Y != original[0].Y && beforeItem.mPosY != 400,
    "actual owner tick moves X, ground Y and height on a sloped row");
Check(moved.Select(drop => drop.SpawnX).SequenceEqual(original.Select(drop => drop.SpawnX))
    && moved.Select(drop => drop.SpawnGroundY).SequenceEqual(original.Select(drop => drop.SpawnGroundY)),
    "physics never changes materialized origin");
var stable = moved.ToArray(); nativeItems = board.m_gridItems.Count; random = board.RandomCalls;
for (int i = 0; i < 5; ++i) { Query(board); BucketDrops.Snapshot(board); }
Check(BucketDrops.Snapshot(board).SequenceEqual(stable) && board.m_gridItems.Count == nativeItems && board.RandomCalls == random,
    "repeated readers preserve age, positions, origins and native allocation count");
board.mApp.GameScene = GameScenes.Loading;
Check(BucketDrops.Snapshot(board).Count == 0 && Query(board).GetArrayLength() == 0, "non-playing board is hidden");
board.mApp.GameScene = GameScenes.Playing;
board.mApp.GameMode = GameMode.ChallengeZenGarden;
Check(BucketDrops.Snapshot(board).Count == 0 && Query(board).GetArrayLength() == 0, "Zen Garden is hidden");
board.mApp.GameMode = GameMode.Adventure;
CherryBombPlants.DisposingBoard = board.Pointer;
Check(BucketDrops.Snapshot(board).Count == 0 && Query(board).GetArrayLength() == 0, "board teardown is hidden");
CherryBombPlants.DisposingBoard = IntPtr.Zero;
OptionalModApi.SessionBoard = null;
Check(Query(board).GetArrayLength() == 0, "ended debug session hides prior board");
OptionalModApi.SessionBoard = board;
BucketDrops.Clear(board);
Check(Query(board).GetArrayLength() == 0 && BucketDrops.Snapshot(foreign).Count == 1,
    "board cleanup does not remove foreign drops");
// A read must not materialize a persisted head that the owner has not restored.
var saved = board.m_gridItems.DataArrayAlloc();
saved.mGridItemType = GridItemType.ZenTool; saved.mGridItemState = GridItemState.Normal;
saved.mSunCount = -521013; saved.mGridX = 1;
saved.mGridY = BitConverter.SingleToInt32Bits(1f); saved.mPosX = 600; saved.mPosY = 400;
saved.mGoalX = 55; saved.mGoalY = -3.2f; saved.mGridItemCounter = 70;
nativeItems = board.m_gridItems.Count;
Check(BucketDrops.Snapshot(board).Count == 0 && Query(board).GetArrayLength() == 0 && !saved.mDead,
    "query never invokes owner restore or removes saved items");
Check(board.m_gridItems.Count == nativeItems && saved.mGridItemCounter == 70 && saved.mPosX == 600,
    "read does not allocate native items or change unmaterialized save state");
BucketDrops.Tick(board);
var restored = BucketDrops.Snapshot(board).Single();
Check(restored.Identity == FusionPlant.EyesZombieHead && restored.Age == 71 && restored.X == 601,
    "only real owner tick restores saved head and preserves existing motion semantics");
Check(restored.SpawnX == null && restored.SpawnGroundY == null,
    "restored head origin remains explicitly unknown instead of inferred from saved moving position");
for (int i = 0; i < 10; ++i) BucketDrops.Tick(board);
var restoredMoved = BucketDrops.Snapshot(board).Single();
Check(restoredMoved.X != restored.X && restoredMoved.SpawnX == null && restoredMoved.SpawnGroundY == null,
    "later restored-head physics never invents origin");
if (OptionalModApi.Present)
    Check(Query(board)[0].GetProperty("spawnX").ValueKind == JsonValueKind.Null
        && Query(board)[0].GetProperty("spawnGroundY").ValueKind == JsonValueKind.Null,
        "unknown restored origins serialize as JSON null");
BucketDrops.Clear();
var clampedBoard = new Board { Pointer = (IntPtr)103 };
BucketDrops.Add(clampedBoard, 1, -500, 123, FusionPlant.Bucket);
BucketDrops.Add(clampedBoard, 2, 2000, 456, FusionPlant.FootballHelmet);
var clamped = BucketDrops.Snapshot(clampedBoard);
Check(clamped[0].X == 30 && clamped[0].SpawnX == 30 && clamped[0].SpawnGroundY == 123
    && clamped[1].X == 770 && clamped[1].SpawnX == 770 && clamped[1].SpawnGroundY == 456,
    "new origins are captured after both actual initial board clamps");
BucketDrops.Clear();
var damagedZombie = new Zombie { mBoard = clampedBoard, mPosX = -500, mPosY = 100,
    mHelmType = HelmType.Pail, mHelmHealth = 500, mBodyHealth = 500 };
var beforeDamage = BucketDrops.BeforeDamage(damagedZombie, DamageFlags.None);
damagedZombie.mHelmHealth = 0;
BucketDrops.AfterDamage(damagedZombie, beforeDamage);
var fromDamage = BucketDrops.Snapshot(clampedBoard).Single();
Check(fromDamage.SpawnX == 30 && fromDamage.SpawnGroundY == damagedZombie.mPosY + damagedZombie.mHeight - 20,
    "actual armor-loss materialization captures its own post-clamp origin");
BucketDrops.Clear();
Console.WriteLine($"Metal drop snapshots ({(OptionalModApi.Present ? "present" : "absent")}): {assertions} assertions passed.");
