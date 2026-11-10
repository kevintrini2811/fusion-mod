using System.Reflection;
using FusionMod;
using Il2CppReloaded.Gameplay;
using LawnGlove;

int checks = 0;
void Check(bool ok, string message) { ++checks; if (!ok) throw new Exception(message); }
void Call(Type t, string name, params object?[] args) => Board.Invoke(t, name, args);
void Hover(Board b, int slot, int col = 2, int row = 1)
{
    var o = ShroomSlots.Offset(slot);
    b.mApp.WidgetManager.LastMouseX[0] = b.GridToPixelX(col, row) + 40 + (int)o.X;
    b.mApp.WidgetManager.LastMouseY[0] = b.GridToPixelY(col, row) + 40 + (int)o.Y;
}
Plant Add(Board b, FusionPlant identity, int slot, int col = 2, int row = 1)
{
    var p = b.AddPlant(col, row, identity.NativeType, identity.Marker)!;
    ShroomSlots.Position(p, col, row, slot);
    return p;
}
Plant Click(Board b, SeedType seed, int slot, bool potBottom = true)
{
    Hover(b, slot);
    if (potBottom && b.GetPlantsOnLawn(2, 1).UnderPlant is { } pot
        && FusionRecipes.TryGet(FusionPlant.From(pot), seed, out _))
        b.mApp.WidgetManager.LastMouseY[0] = b.GridToPixelY(2, 1) + 70;
    b.mCursorObject.CursorType = CursorType.PlantFromBank;
    b.mCursorObject.mType = seed;
    object?[] scope = [b, b.mApp.WidgetManager.LastMouseX[0], b.mApp.WidgetManager.LastMouseY[0], 0, null];
    Call(typeof(PlayerPlantingScope), "Prefix", scope);
    try
    {
        Check(b.CanPlantAt(2, 1, seed) == PlantingReason.Ok, "Click placement validation");
        return b.AddPlant(2, 1, seed, SeedType.None)!;
    }
    finally { Call(typeof(PlayerPlantingScope), "Finalizer", scope[4]); }
}
List<Plant> Live(Board b) => b.m_plants.Where(p => !p.mDead && p.mIsOnBoard && !p.Support).ToList();

// Native depth also subtracts mX. Front-left/right offsets must never
// override the triangle's back-to-front order, including recomputation.
{
    var b = new Board();
    var back = Add(b, FusionPlant.PeaShroom, 0);
    var right = Add(b, FusionPlant.PeaShroom, 1);
    var left = Add(b, FusionPlant.PeaShroom, 2);
    Check(back.mRenderOrder < right.mRenderOrder && back.mRenderOrder < left.mRenderOrder,
        "Upper mushroom renders behind both lower mushrooms");
    Check(back.CalcRenderOrder() == back.mRenderOrder && right.CalcRenderOrder() == right.mRenderOrder
        && left.CalcRenderOrder() == left.mRenderOrder, "Native recomputation preserves slot depth");
    // Native SortingUtils.RemapSortingLayers truncates after compression.
    // Exercise every possible rounding phase, rather than logical order alone.
    int UnityOrder(int order) => (int)((order - 100000) * 65525f / 800000f) - 32763;
    for (int baseline = 300000; baseline < 350000; ++baseline)
        Check(UnityOrder(baseline + right.mRenderOrder - back.mRenderOrder) > UnityOrder(baseline)
            && UnityOrder(baseline + left.mRenderOrder - back.mRenderOrder) > UnityOrder(baseline),
            "Both front slots retain distinct Unity depth after native compression");
}

// Add an isolated Sun-shroom test fixture to exercise
// the real placement/recipe/batch paths without publishing an invented recipe.
var recipes = (Dictionary<(FusionPlant, FusionPlant), FusionPlant>)typeof(FusionRecipes)
    .GetField("Recipes", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
Check(!FusionRecipes.TryGet(SeedType.Sunshroom, SeedType.Iceshroom, out _), "Fixture is not a published recipe");
recipes.Add((SeedType.Sunshroom, SeedType.Iceshroom), FusionPlant.SundaeShroom);

// The source identity and the native saved coordinate must survive independently.
foreach (var identity in new FusionPlant[] { SeedType.Puffshroom, SeedType.Sunshroom, FusionPlant.SundaeShroom })
{
    var b = new Board();
    foreach (int slot in new[] { 2, 0, 1 })
    {
        Hover(b, slot);
        Check(ShroomSlots.CanAdd(b, identity, 2, 1), "Free selected subfield");
        var p = Add(b, identity, slot);
        Check(ShroomSlots.Slot(p) == slot, "Position round trip");
        Check(ShroomSlots.CanAdd(b, identity, 2, 1) == (Live(b).Count < 3),
            "Occupied subfield falls back only while capacity remains");
        Check(FusionPlant.From(p) == identity, "Variant identity retained");
    }
    Check(Live(b).Count == 3, "Exactly three plants");
    for (int slot = 0; slot < 3; ++slot) { Hover(b, slot); Check(!ShroomSlots.CanAdd(b, identity, 2, 1), "Fourth blocked"); }
}

var cards = new Board();
var first = Click(cards, SeedType.Sunshroom, 0);
var second = Click(cards, SeedType.Sunshroom, 1);
var third = Click(cards, SeedType.Sunshroom, 2);
Check(PlayerPlantingScope.Current == null, "Click scope restored");
var fused = Click(cards, SeedType.Iceshroom, 1);
Check(first.mDead && second.mDead && third.mDead, "All three matching ingredients consumed");
Check(Live(cards).Count == 3 && Live(cards).All(p => FusionPlant.From(p) == FusionPlant.SundaeShroom), "Three results, one incoming card");
Check(Live(cards).Select(ShroomSlots.Slot).Order().SequenceEqual(new[] { 0, 1, 2 }), "Results retain subfields");
Check(PlayerPlantingScope.Current == null && !FusionService.CreatingBatch && !FusionService.CheckingNativePlacement, "No leaked contexts");

// No recipe: use a free subfield, with identical preview/card/glove positions.
foreach (bool card in new[] { true, false })
{
    var b = new Board();
    var fallbackOccupant = Add(b, SeedType.Sunshroom, 0);
    var carried = card ? null : Add(b, SeedType.Sunshroom, 0, 5);
    Hover(b, 0);
    Check(ShroomSlots.TryPlacementSlot(b, SeedType.Sunshroom, 2, 1, out int free, carried)
        && free == 1, "Occupied hover selects first free subfield");
    Plant placed;
    if (card)
    {
        b.mCursorObject.CursorType = CursorType.PlantFromBank;
        b.mCursorObject.mType = SeedType.Sunshroom;
        var preview = new CursorPreview { mBoard = b, mGridX = 2, mGridY = 1,
            mX = b.GridToPixelX(2, 1), mY = b.GridToPixelY(2, 1) };
        Call(typeof(ShroomSeedPreviewPatch), "Postfix", preview);
        Check(preview.mX == b.GridToPixelX(2, 1) + ShroomSlots.Offset(free).X
            && preview.mY == b.GridToPixelY(2, 1) + ShroomSlots.Offset(free).Y, "Fallback preview matches free subfield");
        placed = Click(b, SeedType.Sunshroom, 0);
    }
    else
    {
        Check(FusionService.TryPlaceCarried(b, carried!, 2, 1), "Glove fallback succeeds");
        placed = carried!;
    }
    Check(!fallbackOccupant.mDead && ShroomSlots.Slot(fallbackOccupant) == 0 && ShroomSlots.Slot(placed) == free,
        "Fallback retains occupant and uses predicted slot");
    Add(b, SeedType.Sunshroom, 2);
    Check(!ShroomSlots.CanAdd(b, SeedType.Sunshroom, 2, 1), "Full tile rejects fallback");
}

// A valid recipe wins over a free slot, including when both ingredients are small.
// This isolated fixture exercises the priority without publishing a new recipe.
recipes.Add((SeedType.Puffshroom, SeedType.Sunshroom), FusionPlant.SundaeShroom);
foreach (bool card in new[] { true, false })
{
    var b = new Board();
    var target = Add(b, SeedType.Puffshroom, 0);
    var carried = card ? null : Add(b, SeedType.Sunshroom, 0, 5);
    Hover(b, 0);
    Check(FusionRecipes.TryGet(SeedType.Puffshroom, SeedType.Sunshroom, out var expected), "Small-plant recipe fixture");
    if (card) Click(b, SeedType.Sunshroom, 0);
    else Check(FusionService.TryPlaceCarried(b, carried!, 2, 1), "Glove fusion succeeds despite free slot");
    Check(target.mDead && (carried == null || carried.mDead)
        && Live(b).Count == 1 && FusionPlant.From(Live(b)[0]) == expected, "Fusion takes priority over fallback");
}
recipes.Remove((SeedType.Puffshroom, SeedType.Sunshroom));

var ordinary = new Board();
var pea = Add(ordinary, SeedType.Peashooter, 0);
var repeater = Click(ordinary, SeedType.Peashooter, 0);
Check(pea.mDead && FusionPlant.From(repeater) == (FusionPlant)SeedType.Repeater, "Ordinary card fusion unchanged");
var glovePea = Add(ordinary, SeedType.Peashooter, 0, 5);
Hover(ordinary, 0);
Check(FusionService.TryPlaceCarried(ordinary, glovePea, 2, 1) && glovePea.mDead
    && Live(ordinary).Count == 1 && FusionPlant.From(Live(ordinary)[0]) == (FusionPlant)SeedType.Splitpea, "Ordinary glove fusion unchanged");

var mixed = new Board();
var sun0 = Add(mixed, SeedType.Sunshroom, 0);
var puff1 = Add(mixed, FusionPlant.NutShroom, 1);
var sun2 = Add(mixed, SeedType.Sunshroom, 2);
Hover(mixed, 1);
Check(FusionService.ResolveAll(mixed, SeedType.Iceshroom, 2, 1).Count == 2, "Full cell skips incompatible recipe");
Click(mixed, SeedType.Iceshroom, 1);
Check(!puff1.mDead && sun0.mDead && sun2.mDead && Live(mixed).Count == 3, "Incompatible plant retained");

var partial = new Board();
var s0 = Add(partial, SeedType.Sunshroom, 0);
var s1 = Add(partial, SeedType.Sunshroom, 1);
Click(partial, SeedType.Iceshroom, 1);
Check(!s0.mDead && s1.mDead && Live(partial).Count == 2, "Partial cell fuses selected subfield only");
Hover(partial, 2);
Check(!FusionService.TryResolve(partial, SeedType.Iceshroom, 2, 1, out _, out _), "Empty subfield has no fusion target");

// Saving/loading needs no new field: recreate solely from native identity and coordinates.
var loaded = new Board();
foreach (var p in Live(cards))
    loaded.m_plants.Add(new Plant { mBoard = loaded, mPlantCol = p.mPlantCol, mRow = p.mRow,
        mX = p.mX, mY = p.mY, mSeedType = p.mSeedType, mImitaterType = p.mImitaterType });
Check(Live(loaded).Select(ShroomSlots.Slot).Order().SequenceEqual(new[] { 0, 1, 2 }), "Saved coordinates recover subfields");

var move = new Board();
var m0 = Add(move, SeedType.Sunshroom, 0);
var m1 = Add(move, SeedType.Puffshroom, 1);
var m2 = Add(move, SeedType.Sunshroom, 2);
move.mCursorObject.CursorType = CursorType.Glove;
for (int slot = 0; slot < 3; ++slot)
{
    Hover(move, slot);
    object?[] selection = [move, 2, 1, m0];
    Call(typeof(ShroomSelectionPatch), "Postfix", selection);
    Check(ShroomSlots.Slot((Plant)selection[3]!) == slot, "Glove selects hovered plant");
}
Hover(move, 2, 4, 1);
Check(FusionService.TryPlaceCarried(move, m1, 4, 1), "Move single plant to chosen destination subfield");
Check(m1.mPlantCol == 4 && ShroomSlots.Slot(m1) == 2 && m0.mPlantCol == 2 && m2.mPlantCol == 2, "Siblings remain in source cell");
var existing = Add(move, SeedType.Puffshroom, 0, 5);
Hover(move, 1, 5);
Check(FusionService.TryPlaceCarried(move, m1, 5, 1) && !existing.mDead, "Glove fills empty subfield on occupied cell");

var glove = new Board();
for (int slot = 0; slot < 3; ++slot) Add(glove, SeedType.Sunshroom, slot);
var ice = Add(glove, SeedType.Iceshroom, 0, 5);
Hover(glove, 0);
ice.mIsOnBoard = false; ice.mSquished = true;
Check(FusionService.CanPlaceCarried(glove, ice, 2, 1), "Detached glove preview permits batch");
Check(!Live(glove).Any(p => p.mDead), "Preview consumes nothing");
ice.mIsOnBoard = true; ice.mSquished = false;
Check(FusionService.TryPlaceCarried(glove, ice, 2, 1) && ice.mDead && Live(glove).Count == 3, "Glove consumes source once for three recipes");

// Failure at each allocation must retain all existing ingredients.
for (int fail = 1; fail <= 3; ++fail)
{
    var b = new Board();
    var ingredients = Enumerable.Range(0, 3).Select(slot => Add(b, SeedType.Sunshroom, slot)).ToArray();
    var source = Add(b, SeedType.Iceshroom, 0, 5);
    Hover(b, 0);
    b.FailAt = b.Attempts + fail;
    bool failed = false;
    try { FusionService.TryPlaceCarried(b, source, 2, 1); } catch (InvalidOperationException) { failed = true; }
    Check(failed && !source.mDead && ingredients.All(p => !p.mDead) && Live(b).Count == 4, "Batch allocation rollback");
    Check(!FusionService.CreatingBatch, "Allocation failure clears batch context");
}

for (int fail = 2; fail <= 3; ++fail)
{
    var b = new Board();
    var ingredients = Enumerable.Range(0, 3).Select(slot => Add(b, SeedType.Sunshroom, slot)).ToArray();
    b.FailAt = b.Attempts + fail;
    try { Click(b, SeedType.Iceshroom, 0); } catch (InvalidOperationException) { }
    Check(ingredients.All(p => !p.mDead) && Live(b).Count == 3, "Card batch allocation rollback");
    Check(PlayerPlantingScope.Current == null && !FusionService.CreatingBatch, "Failed card restores scope");
}

var scripted = new Board();
var original = Add(scripted, SeedType.Sunshroom, 0);
Hover(scripted, 0);
var spawn = scripted.AddPlant(2, 1, SeedType.Iceshroom, SeedType.None)!;
Check(!original.mDead && FusionPlant.From(spawn) == (FusionPlant)SeedType.Iceshroom, "Scripted AddPlant does not fuse");

var highlight = new Board();
var targets = Enumerable.Range(0, 3).Select(slot => Add(highlight, SeedType.Sunshroom, slot)).ToArray();
highlight.mCursorObject.CursorType = CursorType.PlantFromBank;
highlight.mCursorObject.mType = SeedType.Iceshroom;
Hover(highlight, 0);
foreach (var plant in targets)
{
    object?[] state = [plant, false];
    Call(typeof(FusionHighlightPatch), "Prefix", state);
    Check(plant.mHighlighted, "Every eligible batch target highlights");
    Call(typeof(FusionHighlightPatch), "Finalizer", plant, state[1]);
    Check(!plant.mHighlighted, "Highlight restores native state");
}

var terrain = new Board();
var t0 = Add(terrain, SeedType.Sunshroom, 0);
var t1 = Add(terrain, SeedType.Puffshroom, 1);
Hover(terrain, 2);
terrain.Terrain = PlantingReason.NotOnWater;
Check(!ShroomSlots.CanAdd(terrain, SeedType.Sunshroom, 2, 1), "Terrain remains authoritative");
terrain.ThrowTerrain = true;
try { ShroomSlots.CanAdd(terrain, SeedType.Sunshroom, 2, 1); } catch (InvalidOperationException) { }
Check(t0.mIsOnBoard && !t0.mSquished && t1.mIsOnBoard && !t1.mSquished && !FusionService.CheckingNativePlacement, "Terrain exception restores all siblings");
terrain.ThrowTerrain = false; terrain.Terrain = PlantingReason.Ok;
var support = Add(terrain, SeedType.Wallnut, 0); support.Support = true;
Check(ShroomSlots.CanAdd(terrain, SeedType.Sunshroom, 2, 1) && support.mIsOnBoard && !support.mDead, "Support retained");
terrain.mPaused = true;
Check(!FusionService.TryPlaceCarried(terrain, t0, 5, 1), "Paused glove blocked");
terrain.mApp.GameMode = GameMode.ChallengeZenGarden;
Check(!ShroomSlots.CanAdd(terrain, SeedType.Sunshroom, 2, 1), "Zen garden excluded");

// Native preview recalculates its anchor on each Update. Offset exactly once.
var previewBoard = new Board();
previewBoard.mCursorObject.CursorType = CursorType.PlantFromBank;
previewBoard.mCursorObject.mType = SeedType.Sunshroom;
foreach (int slot in new[] { 0, 1, 2, 1 })
{
    Hover(previewBoard, slot);
    var p = new CursorPreview { mBoard = previewBoard, mGridX = 2, mGridY = 1,
        mX = previewBoard.GridToPixelX(2, 1), mY = previewBoard.GridToPixelY(2, 1) };
    Call(typeof(ShroomSeedPreviewPatch), "Postfix", p);
    Check(p.mX == previewBoard.GridToPixelX(2, 1) + ShroomSlots.Offset(slot).X
        && p.mY == previewBoard.GridToPixelY(2, 1) + ShroomSlots.Offset(slot).Y && p.mController.Updates == 1, "Preview matches planting subfield");
}

var carriedPreview = new LawnGlove.CarriedPlant();
var carriedSmall = Add(previewBoard, SeedType.Puffshroom, 0, 5);
carriedPreview.gridAnchor.mBoard = previewBoard;
LawnGloveMod.Carried = carriedSmall;
foreach (int slot in new[] { 0, 1, 2 })
{
    Hover(previewBoard, slot);
    Call(typeof(ShroomGlovePreviewPatch), "Postfix", carriedPreview);
    var o = ShroomSlots.Offset(slot);
    Check(carriedPreview.gridAnchor.mX == previewBoard.GridToPixelX(2, 1) + o.X
        && carriedPreview.gridAnchor.mY == previewBoard.GridToPixelY(2, 1) + o.Y, "Glove preview matches destination subfield");
}
LawnGloveMod.Carried = null;
previewBoard.mCursorObject.mImitaterType = SeedType.Sunshroom;
Add(previewBoard, SeedType.Sunshroom, 0);
Hover(previewBoard, 1);
Check(previewBoard.CanPlantAt(2, 1, SeedType.Sunshroom) == PlantingReason.NotHere, "Imitater does not bypass native placement");
// Reproduce the screenshot: native pixel hit test chooses the right mushroom
// while the glove's subfield selection chooses the left mushroom.
var hoverBoard = new Board();
hoverBoard.mCursorObject.CursorType = CursorType.Glove;
var hoverPlants = Enumerable.Range(0, 3).Select(slot => Add(hoverBoard, SeedType.Puffshroom, slot)).ToArray();
foreach (int slot in new[] { 2, 0, 1, 2 })
{
    Hover(hoverBoard, slot);
    object?[] pickup = [hoverBoard, 2, 1, hoverPlants[1]];
    Call(typeof(ShroomSelectionPatch), "Postfix", pickup);
    object?[] hoverHit = [hoverBoard, hoverBoard.mApp.WidgetManager.LastMouseX[0],
        hoverBoard.mApp.WidgetManager.LastMouseY[0], hoverPlants[1]];
    Call(typeof(ShroomGloveHoverPatch), "Postfix", hoverHit);
    Check(ReferenceEquals(hoverHit[3], hoverPlants[slot]) && ReferenceEquals(hoverHit[3], pickup[3]),
        "Hover and pickup select the same mushroom, including left instead of native right");
}
hoverPlants[2].mDead = true;
Hover(hoverBoard, 2);
object?[] emptyHover = [hoverBoard, hoverBoard.mApp.WidgetManager.LastMouseX[0],
    hoverBoard.mApp.WidgetManager.LastMouseY[0], hoverPlants[1]];
Call(typeof(ShroomGloveHoverPatch), "Postfix", emptyHover);
Check(emptyHover[3] == null, "Empty subfield does not highlight a sibling");
foreach (var cursor in new[] { CursorType.PlantFromBank })
{
    hoverBoard.mCursorObject.CursorType = cursor;
    object?[] untouched = [hoverBoard, 240, 220, hoverPlants[1]];
    Call(typeof(ShroomGloveHoverPatch), "Postfix", untouched);
    Check(ReferenceEquals(untouched[3], hoverPlants[1]), "Other tools retain native hit test");
}
hoverBoard.mCursorObject.CursorType = CursorType.Glove;
LawnGloveMod.Carried = hoverPlants[0];
object?[] carriedHover = [hoverBoard, 240, 220, hoverPlants[1]];
Call(typeof(ShroomGloveHoverPatch), "Postfix", carriedHover);
Check(ReferenceEquals(carriedHover[3], hoverPlants[1]), "Carried fusion targets retain native hit test");
LawnGloveMod.Carried = null;
// Published Puff recipes exercise the same full-cell transaction and glove path.
foreach (var identity in new[] { FusionPlant.PeaShroom, FusionPlant.RepeatShroom,
    FusionPlant.SplitPeaShroom, FusionPlant.GatlingPeaShroom })
{
    var b = new Board();
    foreach (int slot in new[] {0, 1, 2}) Add(b, identity, slot);
    Check(Live(b).All(p => ShroomSlots.IsSmall(FusionPlant.From(p))), "Puff variants retain dense placement");
}
var puffBoard = new Board();
foreach (int slot in new[] {0, 1, 2}) Add(puffBoard, SeedType.Puffshroom, slot);
Click(puffBoard, SeedType.Peashooter, 2);
Check(Live(puffBoard).Count == 3 && Live(puffBoard).All(p => FusionPlant.From(p) == FusionPlant.PeaShroom), "One card fuses all three Puff-shrooms");
Check(Live(puffBoard).Select(ShroomSlots.Slot).Order().SequenceEqual(new[] {0, 1, 2}), "Puff batch retains slots");
var puffGlovePea = Add(puffBoard, SeedType.Peashooter, 0, 5);
Hover(puffBoard, 0);
Check(FusionService.TryPlaceCarried(puffBoard, puffGlovePea, 2, 1), "Glove upgrades three Pea-shrooms");
Check(puffGlovePea.mDead && Live(puffBoard).Count == 3 && Live(puffBoard).All(p => FusionPlant.From(p) == FusionPlant.RepeatShroom), "Single glove ingredient consumed for full batch");
var mixedPuffBoard = new Board();
Add(mixedPuffBoard, FusionPlant.PeaShroom, 0);
Add(mixedPuffBoard, FusionPlant.RepeatShroom, 1);
Add(mixedPuffBoard, FusionPlant.GatlingPeaShroom, 2);
Click(mixedPuffBoard, SeedType.Peashooter, 0);
Check(Live(mixedPuffBoard).Select(FusionPlant.From).ToHashSet().SetEquals(new[] {FusionPlant.RepeatShroom, FusionPlant.SplitPeaShroom, FusionPlant.GatlingPeaShroom}), "Each matching identity upgrades once; nonmatching sibling retained");
// A partial Puff tile must fuse as a group, independent of hovered subfield.
foreach (int count in new[] {1,2,3})
foreach (int hover in new[] {0,1,2})
{
    var b=new Board();
    var ingredients=Enumerable.Range(0,count).Select(slot=>Add(b,SeedType.Puffshroom,slot)).ToArray();
    Click(b,SeedType.Peashooter,hover);
    Check(ingredients.All(p=>p.mDead) && Live(b).Count==count
        && Live(b).All(p=>FusionPlant.From(p)==FusionPlant.PeaShroom), "Card fuses every Puff in partial/full tile");
    Check(Live(b).Select(ShroomSlots.Slot).Order().SequenceEqual(Enumerable.Range(0,count)), "Group fusion retains original slots");
    var source=Add(b,SeedType.Peashooter,0,5);
    Hover(b,hover);
    Check(FusionService.TryPlaceCarried(b,source,2,1) && source.mDead
        && Live(b).All(p=>FusionPlant.From(p)==FusionPlant.RepeatShroom), "Glove upgrades entire partial/full Puff tile");
}
var partialMixed=new Board();
var mixedBase=Add(partialMixed,SeedType.Puffshroom,0);
var mixedGatling=Add(partialMixed,FusionPlant.GatlingPeaShroom,1);
Click(partialMixed,SeedType.Peashooter,2);
Check(mixedBase.mDead && !mixedGatling.mDead && Live(partialMixed).Count==2
    && Live(partialMixed).Any(p=>FusionPlant.From(p)==FusionPlant.PeaShroom), "Partial tile retains incompatible Puff variant");
// Both inputs are existing small plants; a card on an occupied slot fuses
// all matching Puff ingredients while leaving incompatible Sun-shrooms alone.
foreach(bool card in new[]{true,false})
{
    var b=new Board();
    var a=Add(b,SeedType.Puffshroom,0);
    var c=Add(b,SeedType.Puffshroom,1);
    b.FailAt=b.Attempts+2;
    bool failed=false;
    var source=card?null:Add(b,SeedType.Peashooter,0,5);
    if(!card) b.FailAt=b.Attempts+2;
    try {
        if(card) Click(b,SeedType.Peashooter,0);
        else {Hover(b,0);FusionService.TryPlaceCarried(b,source!,2,1);}
    } catch(InvalidOperationException) {failed=true;}
    Check(failed && !a.mDead && !c.mDead && (source==null||!source.mDead), "Partial Puff batch rollback retains all ingredients");
    Check(Live(b).Count==(card?2:3), "Partial Puff rollback removes only results");
}
// New non-shooting Puff results use the same atomic group transaction.
foreach (var (ingredient, result) in new[] {
    (SeedType.Sunflower,(FusionPlant)SeedType.Sunshroom), (SeedType.Hypnoshroom,FusionPlant.GnomeShroom),
    (SeedType.Umbrella,FusionPlant.UmbrellaCap),
    (SeedType.Wallnut,FusionPlant.NutShroom), (SeedType.Jalapeno,FusionPlant.BellPepper),
    (SeedType.Doomshroom,FusionPlant.BoomShroom), (SeedType.Marigold,FusionPlant.PennyShroom),
    (SeedType.Iceshroom,FusionPlant.HailShroom), (SeedType.Snowpea,FusionPlant.IcicleShroom),
    (SeedType.Starfruit,FusionPlant.TwinkleShroom), (SeedType.Potatomine,FusionPlant.NuggetShroom),
    (SeedType.Cabbagepult,FusionPlant.SproutPult), (SeedType.Kernelpult,FusionPlant.BabyCorn),
    (SeedType.Melonpult,FusionPlant.SlicePult) })
foreach(int count in new[] {1,2,3})
{
    var b=new Board();
    var originals=Enumerable.Range(0,count).Select(slot=>Add(b,SeedType.Puffshroom,slot)).ToArray();
    Click(b,ingredient,2);
    Check(originals.All(p=>p.mDead) && Live(b).Count==count
        && Live(b).All(p=>FusionPlant.From(p)==result), "New Puff card batch for each occupancy");
    Check(Live(b).Select(ShroomSlots.Slot).Order().SequenceEqual(Enumerable.Range(0,count)), "New result preserves each slot");
    var gloveBoard=new Board();
    foreach(int slot in Enumerable.Range(0,count)) Add(gloveBoard,SeedType.Puffshroom,slot);
    var source=Add(gloveBoard,ingredient,0,6);
    Check(FusionService.TryPlaceCarried(gloveBoard,source,2,1) && source.mDead
        && Live(gloveBoard).Count==count && Live(gloveBoard).All(p=>FusionPlant.From(p)==result), "New Puff glove batch");
    var reverse=new Board();
    var baseIngredient=Add(reverse,ingredient,0);
    Click(reverse,SeedType.Puffshroom,0);
    Check(baseIngredient.mDead && Live(reverse).Count==1 && FusionPlant.From(Live(reverse)[0])==result, "Reverse card order");
    var reverseGlove=new Board();
    Add(reverseGlove,ingredient,0);
    var puff=Add(reverseGlove,SeedType.Puffshroom,0,6);
    Check(FusionService.TryPlaceCarried(reverseGlove,puff,2,1) && puff.mDead
        && Live(reverseGlove).Count==1 && FusionPlant.From(Live(reverseGlove)[0])==result, "Reverse glove order");
}
// Reproduction runs independently of cursor position or a player click.
foreach (int occupied in new[] { 1, 2, 3 })
{
    var b = new Board();
    foreach (int slot in Enumerable.Range(0, occupied)) Add(b, FusionPlant.NuggetShroom, slot);
    Hover(b, 0, 8, 4);
    bool spawned = ShroomSlots.TrySpawn(b, FusionPlant.NuggetShroom, 2, 1);
    Check(spawned == (occupied < 3), "Reproduction respects three-slot capacity");
    Check(Live(b).Count == Math.Min(3, occupied + 1), "Reproduction preserves occupants");
    Check(Live(b).Select(ShroomSlots.Slot).Distinct().Count() == Live(b).Count, "Reproduction selects a free slot");
    Check(Live(b).All(p => FusionPlant.From(p) == FusionPlant.NuggetShroom), "Reproduction never reinterprets marker as ingredient");
}
{
    var b = new Board(); Add(b, SeedType.Peashooter, 0);
    Check(!ShroomSlots.TrySpawn(b, FusionPlant.NuggetShroom, 2, 1), "No reproduction through a large occupant");
    b = new Board(); var originalNugget = Add(b, FusionPlant.NuggetShroom, 0);
    b.Terrain = PlantingReason.NotHere;
    Check(!ShroomSlots.TrySpawn(b, FusionPlant.NuggetShroom, 2, 1) && !originalNugget.mSquished && originalNugget.mIsOnBoard,
        "Invalid terrain retains occupant flags");
    b.Terrain = PlantingReason.Ok; b.FailAt = b.Attempts + 1;
    Check(!ShroomSlots.TrySpawn(b, FusionPlant.NuggetShroom, 2, 1) && Live(b).Count == 1,
        "Failed allocation retains original");
}
foreach (var (left, ingredient) in new[] { (FusionPlant.HailShroom, SeedType.Peashooter), (FusionPlant.PeaShroom, SeedType.Iceshroom) })
{
    var b = new Board(); Add(b, left, 0); Add(b, left, 1);
    Click(b, ingredient, 2);
    Check(Live(b).Count == 2 && Live(b).All(p => FusionPlant.From(p) == FusionPlant.IcicleShroom),
        "Alternative Icicle recipe uses common card batch");
    b = new Board(); Add(b, left, 0); var source = Add(b, ingredient, 0, 6);
    Check(FusionService.TryPlaceCarried(b, source, 2, 1) && source.mDead && FusionPlant.From(Live(b).Single()) == FusionPlant.IcicleShroom,
        "Alternative Icicle recipe uses common glove batch");
}
foreach (var (left, right) in new[] {
    (FusionPlant.PeaShroom, FusionPlant.Bucket),
    ((FusionPlant)SeedType.Puffshroom, FusionPlant.Buckshooter) })
foreach (bool reverseOrder in new[] { false, true })
{
    var b = new Board();
    var buckIngredient = Add(b, reverseOrder ? right : left, 0);
    var source = Add(b, reverseOrder ? left : right, 0, 6);
    Check(FusionService.TryPlaceCarried(b, source, 2, 1) && source.mDead && buckIngredient.mDead
        && Live(b).Count == 1 && FusionPlant.From(Live(b).Single()) == FusionPlant.BuckShroom,
        "Both Buck-shroom source recipes use atomic glove path in both directions");
}
// Support fusion uses the same card/glove resolver without removing occupants.
foreach (bool reverse in new[] { false, true })
{
    var board = new Board();
    var target = Add(board, reverse ? SeedType.Sunflower : SeedType.Flowerpot, 0);
    var supportResult = Click(board, reverse ? SeedType.Flowerpot : SeedType.Sunflower, 0);
    Check(FusionPlant.From(supportResult) == FusionPlant.SolarPot && target.mDead, "Both Solar Pot card orders");
    Check(board.GetPlantsOnLawn(2, 1).UnderPlant == supportResult, "Solar Pot occupies support layer");
    board = new Board();
    target = Add(board, reverse ? SeedType.Sunflower : SeedType.Flowerpot, 0);
    var carried = Add(board, reverse ? SeedType.Flowerpot : SeedType.Sunflower, 0, 4, 1);
    board.mApp.WidgetManager.LastMouseY[0] = board.GridToPixelY(2, 1) + 70;
    Check(FusionService.CanPlaceCarried(board, carried, 2, 1), "Both Solar Pot glove previews");
    Check(FusionService.TryPlaceCarried(board, carried, 2, 1) && target.mDead && carried.mDead,
        "Both Solar Pot glove commits");
    Check(FusionPlant.From(board.GetPlantsOnLawn(2, 1).UnderPlant!) == FusionPlant.SolarPot, "Glove result is support");
}
var layered = new Board();
var potIngredient = Add(layered, SeedType.Flowerpot, 0);
var occupant = Add(layered, SeedType.Cabbagepult, 0);
layered.mApp.WidgetManager.LastMouseY[0] = layered.GridToPixelY(2, 1) + 70;
var potPlans = FusionService.ResolveAll(layered, SeedType.Sunflower, 2, 1);
Check(potPlans.Count == 1 && potPlans[0].Target == potIngredient, "Find support below unrelated normal plant");
Check(FusionService.CheckFusionPlacement(layered, potIngredient, FusionPlant.SolarPot) == PlantingReason.Ok
    && occupant.mIsOnBoard && potIngredient.mIsOnBoard, "Support terrain query restores normal occupant");
layered.ThrowTerrain = true;
try { FusionService.CheckFusionPlacement(layered, potIngredient, FusionPlant.SolarPot); }
catch (InvalidOperationException) { }
Check(occupant.mIsOnBoard && !occupant.mSquished && potIngredient.mIsOnBoard && !potIngredient.mSquished
    && !FusionService.CheckingNativePlacement, "Support terrain failure restores both layers");
layered.ThrowTerrain = false;
var solarPot = Click(layered, SeedType.Sunflower, 0);
Check(FusionPlant.From(solarPot) == FusionPlant.SolarPot && potIngredient.mDead && !occupant.mDead
    && occupant.mIsOnBoard, "Support fusion retains unrelated occupant");
var priorityBoard = new Board();
var retainedPot = Add(priorityBoard, SeedType.Flowerpot, 0);
var normalIngredient = Add(priorityBoard, SeedType.Peashooter, 0);
var preferred = FusionService.ResolveAll(priorityBoard, SeedType.Sunflower, 2, 1);
Check(preferred.Count == 1 && preferred[0].Result == FusionPlant.SolarShooter
    && preferred[0].Target == normalIngredient, "One card selects normal recipe before support recipe");
var normalFusion = Click(priorityBoard, SeedType.Sunflower, 0, potBottom: false);
Check(FusionPlant.From(normalFusion) == FusionPlant.SolarShooter && normalIngredient.mDead && !retainedPot.mDead,
    "Normal fusion preserves support and produces only one result");
var allocationBoard = new Board();
var allocationPot = Add(allocationBoard, SeedType.Flowerpot, 0);
allocationBoard.FailAt = allocationBoard.Attempts + 1;
Check(Click(allocationBoard, SeedType.Sunflower, 0) == null && !allocationPot.mDead,
    "Failed support allocation retains ingredient");

// The 1.6.2 shovel fix intentionally resolves Normal/ Shovel through ToolHitTest.
foreach (var cursor in new[] { CursorType.Normal, CursorType.Shovel })
{
    hoverBoard.mCursorObject.CursorType = cursor;
    Hover(hoverBoard, 0);
    object?[] hit = [hoverBoard, hoverBoard.mApp.WidgetManager.LastMouseX[0],
        hoverBoard.mApp.WidgetManager.LastMouseY[0], hoverPlants[1]];
    Call(typeof(ShroomGloveHoverPatch), "Postfix", hit);
    Check(ReferenceEquals(hit[3], hoverPlants[0]), "Normal and shovel use selected subfield");
}
foreach (var (ingredient,result) in new[] {((FusionPlant)SeedType.Cabbagepult,FusionPlant.CabbagePot),
    ((FusionPlant)SeedType.Kernelpult,FusionPlant.ButterPot),((FusionPlant)SeedType.Umbrella,FusionPlant.UmbrellaPot),
    ((FusionPlant)SeedType.Wallnut,FusionPlant.NutPot),((FusionPlant)SeedType.Garlic,FusionPlant.GarlicPot),
    ((FusionPlant)SeedType.Spikeweed,FusionPlant.SpikePot),((FusionPlant)SeedType.Melonpult,FusionPlant.MelonPot)})
foreach(bool reverse in new[]{false,true})
{
    var board=new Board();
    var target=Add(board,reverse?ingredient:SeedType.Flowerpot,0);
    var createdSupport=Click(board,reverse?SeedType.Flowerpot:ingredient.NativeType,0);
    Check(FusionPlant.From(createdSupport)==result&&target.mDead,"New support recipes work in both card orders");
    Check(board.Sun == (result == FusionPlant.MelonPot ? 150 : 0), "Sun refund occurs after card fusion only");
    board=new Board(); target=Add(board,reverse?ingredient:SeedType.Flowerpot,0);
    var carried=Add(board,reverse?SeedType.Flowerpot:ingredient,0,5,1);
    board.mApp.WidgetManager.LastMouseY[0]=board.GridToPixelY(2,1)+70;
    Check(FusionService.TryPlaceCarried(board,carried,2,1)&&target.mDead&&carried.mDead,
        "New support recipes work in both glove directions");
    Check(FusionPlant.From(board.GetPlantsOnLawn(2,1).UnderPlant!)==result,"New pot owns support layer");
    Check(board.Sun == (result == FusionPlant.MelonPot ? 150 : 0), "Glove fusion refunds Sun exactly once");
}
var scriptedMelon = new Board();
Add(scriptedMelon, FusionPlant.MelonPot, 0);
Check(scriptedMelon.Sun == 0, "Scripted spawning cannot earn the fusion refund");
var cannonBoard=new Board();
var leftCorn=Add(cannonBoard,SeedType.Kernelpult,0);
Check(FusionService.CheckFusionPlacement(cannonBoard,leftCorn,SeedType.Cobcannon)==PlantingReason.NotHere,
    "Cannon result requires second kernel");
var rightCorn=Add(cannonBoard,SeedType.Kernelpult,0,3,1);
Check(FusionService.CheckFusionPlacement(cannonBoard,leftCorn,SeedType.Cobcannon)==PlantingReason.Ok,
    "Native cannon placement sees both kernels");
var cannonIngredient=Add(cannonBoard,SeedType.Cobcannon,0,5,1);
cannonBoard.FailAt=cannonBoard.Attempts+1;
try { FusionService.TryPlaceCarried(cannonBoard,cannonIngredient,2,1); } catch(InvalidOperationException) { }
Check(!leftCorn.mDead&&!rightCorn.mDead&&!cannonIngredient.mDead,"Failed cannon allocation keeps every ingredient");
cannonBoard.FailAt=0;
Check(FusionService.TryPlaceCarried(cannonBoard,cannonIngredient,2,1)&&leftCorn.mDead&&rightCorn.mDead&&cannonIngredient.mDead,
    "Successful cannon allocation consumes both kernels and incoming cannon");
var replacementBoard=new Board(); var oldCannon=Add(replacementBoard,SeedType.Cobcannon,0);
var obstruction=Add(replacementBoard,SeedType.Sunflower,0,3,1);
Check(FusionService.CheckFusionPlacement(replacementBoard,oldCannon,SeedType.Cobcannon)==PlantingReason.NotHere,
    "Replacement cannon cannot overwrite its right-hand neighbor");
obstruction.mDead=true;
Check(FusionService.CheckFusionPlacement(replacementBoard,oldCannon,SeedType.Cobcannon)==PlantingReason.Ok,
    "Replacement cannon validates both free terrain cells");
oldCannon.mPlantCol=8;
Check(FusionService.CheckFusionPlacement(replacementBoard,oldCannon,SeedType.Cobcannon)==PlantingReason.NotHere,
    "No cannon may extend beyond last column");

Console.WriteLine($"Shroom slots: {checks} assertions passed. Offline only.");

var pumpkinBoard=new Board();
var inner=Add(pumpkinBoard,SeedType.Peashooter,0);
var plainPumpkin=pumpkinBoard.AddPlant(2,1,SeedType.Pumpkinshell,SeedType.None)!;
var shellPlans=FusionService.ResolveAll(pumpkinBoard,SeedType.Cactus,2,1);
Check(shellPlans.Count==1&&shellPlans[0].Target==plainPumpkin&&shellPlans[0].Result==FusionPlant.ThornyPumpkin,
    "Shell recipe resolves underneath unrelated normal occupant");
FusionService.CompleteBatch(pumpkinBoard,SeedType.Cactus,shellPlans);
Check(!inner.mDead&&plainPumpkin.mDead,"Shell fusion preserves normal occupant");

// An aquatic fusion validates the result terrain even when its land ingredient
// was rejected before native occupancy checks. Both card directions still work.
foreach(var (land,result) in new[]{(SeedType.Squash,FusionPlant.Splash),(SeedType.Wallnut,FusionPlant.SeaNut)})
foreach(bool reverse in new[]{false,true}) {
 var waterBoard=new Board {TerrainForSeed=seed=>seed==SeedType.Seashroom?PlantingReason.Ok:PlantingReason.NotOnWater};
 var basePlant=Add(waterBoard,reverse?land:SeedType.Seashroom,0);
 var incoming=reverse?SeedType.Seashroom:land;
 Check(waterBoard.CanPlantAt(2,1,incoming)==PlantingReason.Ok,"Aquatic result accepts both card directions on water");
 Check(FusionService.CheckFusionPlacement(waterBoard,basePlant,result)==PlantingReason.Ok,"Aquatic result owns terrain query");
 waterBoard.TerrainForSeed=seed=>PlantingReason.NotOnWater;
 Check(waterBoard.CanPlantAt(2,1,incoming)==PlantingReason.NotOnWater,"Invalid aquatic terrain still rejects fusion");
 Check(!basePlant.mDead&&basePlant.mIsOnBoard&&!basePlant.mSquished,"Terrain checks never consume ingredient");
}

// A shared recipe must follow the pointer's layer, including an empty pot.
foreach (var supportType in new[] { (FusionPlant)SeedType.Flowerpot, FusionPlant.SolarPot })
{
    var supportedBoard = new Board();
    var gloveSupport = Add(supportedBoard, supportType, 0);
    var movingPlant = Add(supportedBoard, SeedType.Sunflower, 0, 5);
    Hover(supportedBoard, 0);
    Check(FusionService.CanPlaceCarried(supportedBoard, movingPlant, 2, 1), "Glove accepts ordinary plant above an empty native or fusion pot");
    Check(FusionService.TryPlaceCarried(supportedBoard, movingPlant, 2, 1)
        && movingPlant.mPlantCol == 2 && !gloveSupport.mDead, "Glove moves existing plant onto pot without consuming support");
    var blockedPlant = Add(supportedBoard, SeedType.Sunflower, 0, 6);
    Check(!FusionService.CanStackCarried(supportedBoard, blockedPlant, 2, 1), "Occupied normal layer remains blocked");
    var invalidBoard = new Board(); Add(invalidBoard, supportType, 0);
    var invalidPlant = Add(invalidBoard, SeedType.Sunflower, 0, 5);
    invalidBoard.Terrain = PlantingReason.NotOnWater;
    Check(!FusionService.CanStackCarried(invalidBoard, invalidPlant, 2, 1), "Support never bypasses native terrain restrictions");
}
var splitPotBoard = new Board();
var splitPot = Add(splitPotBoard, SeedType.Flowerpot, 0);
Hover(splitPotBoard, 0);
Check(FusionService.ResolveAll(splitPotBoard, SeedType.Sunflower, 2, 1).Count == 0,
    "Upper area of empty pot permits ordinary planting");
var upperSunflower = Click(splitPotBoard, SeedType.Sunflower, 0, potBottom: false);
Check(!splitPot.mDead && FusionPlant.From(upperSunflower) == (FusionPlant)SeedType.Sunflower,
    "Upper click plants on pot without consuming it");
var bothLayers = new Board();
var bottomPot = Add(bothLayers, SeedType.Flowerpot, 0);
var topPea = Add(bothLayers, SeedType.Peashooter, 0);
bothLayers.mApp.WidgetManager.LastMouseY[0] = bothLayers.GridToPixelY(2, 1) + 54;
var bottomPlan = FusionService.ResolveAll(bothLayers, SeedType.Sunflower, 2, 1);
Check(bottomPlan.Count == 1 && bottomPlan[0].Target == bottomPot && bottomPlan[0].Result == FusionPlant.SolarPot,
    "Lower third chooses pot even when occupant also has recipe");
bothLayers.mApp.WidgetManager.LastMouseY[0] = bothLayers.GridToPixelY(2, 1) + 53;
Check(FusionService.ResolveAll(bothLayers, SeedType.Sunflower, 2, 1).Single().Target == topPea,
    "Upper boundary chooses normal occupant");
bothLayers.mCursorObject.CursorType=CursorType.Glove;
bothLayers.mApp.WidgetManager.LastMouseY[0]=bothLayers.GridToPixelY(2,1)+54;
var potSelection=topPea;ShroomSelectionPatch.Postfix(bothLayers,2,1,ref potSelection);
Check(potSelection==bottomPot,"glove selects support in lower third despite normal occupant");
bothLayers.mApp.WidgetManager.LastMouseY[0]=bothLayers.GridToPixelY(2,1)+53;
potSelection=topPea;ShroomSelectionPatch.Postfix(bothLayers,2,1,ref potSelection);
Check(potSelection==topPea,"glove upper area retains normal occupant");
var coffeeBoard = new Board();
var sleepingMushrooms = Enumerable.Range(0, 3).Select(slot => Add(coffeeBoard, SeedType.Puffshroom, slot)).ToArray();
foreach (var mushroom in sleepingMushrooms) mushroom.mIsAsleep = true;
var coffeeOtherTile = Add(coffeeBoard, SeedType.Puffshroom, 0, 3); coffeeOtherTile.mIsAsleep = true;
var coffeeBean = Add(coffeeBoard, SeedType.InstantCoffee, 0);
object?[] coffeeState = [coffeeBean, false];
Call(typeof(TileCoffeePatch), "Prefix", coffeeState);
coffeeBean.mState = PlantState.DoingSpecial;
Call(typeof(TileCoffeePatch), "Postfix", coffeeBean, coffeeState[1]);
Check(sleepingMushrooms.All(p => p.mWakeUpCounter == 100) && coffeeOtherTile.mWakeUpCounter == 0,
    "One coffee wakes all three mushrooms only on its tile");

// A floating shell validates the aquatic terrain without consuming its inner plant.
var shellBoard = new Board { Water = true };
shellBoard.TerrainForSeed = seed => seed == SeedType.Seashroom ? PlantingReason.Ok : PlantingReason.NotHere;
var innerShellPlant = shellBoard.AddPlant(4, 1, SeedType.Peashooter, SeedType.None)!;
Check(FusionService.ShellTerrain(shellBoard, 4, 1, FusionPlant.SeaShell) == PlantingReason.Ok,
    "Sea-shell floats on water even around an existing normal occupant");
Check(innerShellPlant.mIsOnBoard && !innerShellPlant.mSquished && !innerShellPlant.mDead,
    "shell terrain query restores the inner plant");
var occupiedShell = shellBoard.AddPlant(4, 1, FusionPlant.SeaShell.NativeType, FusionPlant.SeaShell.Marker)!;
Check(FusionService.ShellTerrain(shellBoard, 4, 1, FusionPlant.SeaShell) == PlantingReason.NotHere,
    "floating shells never overwrite another live shell");

// Real recipe, placement and transaction paths for both source spread recipes.
foreach (var seed in new[] { SeedType.Fumeshroom, SeedType.Scaredyshroom })
foreach (int count in new[] { 1, 2, 3 })
{
    var b = new Board();
    var ingredients = Enumerable.Range(0, count).Select(slot => Add(b, SeedType.Puffshroom, slot)).ToArray();
    var result = Click(b, seed, 0);
    Check(ingredients.All(p => p.mDead), "Spread consumes every plain Puff-shroom");
    Check(FusionPlant.From(result) == (FusionPlant)seed && ShroomSlots.Plants(b, 2, 1).Count == 1,
        "Spread leaves exactly one unmarked native mushroom");
    foreach (var cell in PuffSpreading.Cells(2, 1, count, seed == SeedType.Fumeshroom))
        Check(ShroomSlots.Plants(b, cell.Col, cell.Row).Count == cell.Count, "Source spread geometry and count");
    Check(!FusionRecipes.TryGet(seed, SeedType.Puffshroom, out _), "Source requires Puff-shroom base");
}
{
    var b = new Board();
    var puffs = Enumerable.Range(0, 3).Select(slot => Add(b, SeedType.Puffshroom, slot)).ToArray();
    var blocker = Add(b, SeedType.Wallnut, 0, 3);
    Click(b, SeedType.Fumeshroom, 0);
    Check(!blocker.mDead && ShroomSlots.Plants(b, 4, 1).Count == 2 && ShroomSlots.Plants(b, 5, 1).Count == 1,
        "Blocked front cell does not cancel or redistribute later cells");
}
{
    var b = new Board();
    var puffs = Enumerable.Range(0, 3).Select(slot => Add(b, SeedType.Puffshroom, slot)).ToArray();
    b.FailAt = b.Attempts + 3;
    bool failed = false;
    try { Click(b, SeedType.Fumeshroom, 0); } catch (InvalidOperationException) { failed = true; }
    Check(failed && puffs.All(p => !p.mDead) && Live(b).Count == 3,
        "Offspring allocation failure rolls back central mushroom and earlier offspring");
    Check(!FusionService.CreatingBatch && PlayerPlantingScope.Current == null, "Failed spread restores contexts");
}
{
    var b = new Board();
    Add(b, SeedType.Sunshroom, 0); Add(b, SeedType.Puffshroom, 1);
    Check(FusionService.ResolveAll(b, SeedType.Fumeshroom, 2, 1).Count == 0, "Mixed clusters cannot become overlapping large plants");
    Check(!ShroomSlots.TrySpawn(b, SeedType.Puffshroom, 9, 0) && !ShroomSlots.TrySpawn(b, SeedType.Puffshroom, 0, -1),
        "Scripted spread clips board edges before native terrain access");
}
Console.WriteLine($"Passed {checks} checks including Puff-shroom spreading.");
