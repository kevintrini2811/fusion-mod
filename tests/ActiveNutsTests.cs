using FusionMod;
using Il2CppReloaded.Gameplay;

static void Check(bool value, string contract)
{ if (!value) throw new Exception(contract); }
static Plant NewPlant(FusionPlant identity, Board? board = null)
{
    var plant = new Plant { mBoard = board ?? new(), mSeedType = identity.NativeType, mImitaterType = identity.Marker };
    plant.mBoard.m_plants.Add(plant); ActiveNuts.Initialize(plant); return plant;
}
static Zombie Target(Board board, float x = 160)
{ var zombie = new Zombie { mBoard = board, X = x }; board.m_zombies.Add(zombie); return zombie; }

Check(ActiveNuts.StoredEnergy(100, false) == 100, "100 damage remains unmultiplied");
Check(ActiveNuts.StoredEnergy(101, false) == 202, "multiplier uses strict threshold");
Check(ActiveNuts.StoredEnergy(501, false) == 1503, "third multiplier");
Check(ActiveNuts.StoredEnergy(1001, true) == 6006, "melon multiplies energy by 1.5");
var cabbage = NewPlant(FusionPlant.CabbageNut);
var hit = new ActiveNuts.Hit(cabbage, false);
cabbage.mPlantHealth -= 501; hit.Apply(); hit.Apply();
Check(cabbage.mTargetX == 1503, "one numeric injury stored exactly once");
Target(cabbage.mBoard);
ActiveNuts.Tick(cabbage);
Check(cabbage.mStartRow == 74 && cabbage.mTargetY == 1503, "charge starts clip and shield together");
for (int i = 0; i < 42; ++i) ActiveNuts.Tick(cabbage);
Check(cabbage.mTargetX == 0 && cabbage.mBoard.m_projectiles.Single().mCobTargetRow == 1503, "shoot event snapshots charge");
var shielded = NewPlant(FusionPlant.MelonNut); shielded.mPlantHealth = 5; shielded.mTargetY = 50;
ActiveNuts.Hits = new() { new(shielded, true) }; shielded.mPlantHealth -= 10; shielded.mBoard.mPlantsEaten = 1;
Check(!ActiveNuts.BeforeDeath(shielded) && shielded.mPlantHealth == 5 && shielded.mTargetY == 40
    && shielded.mBoard.mPlantsEaten == 0, "shield saves lethal bite before Die");
ActiveNuts.Apply(); Check(shielded.mTargetY == 40 && shielded.mTargetX == 0, "shield damage produces no energy");
ActiveNuts.Hits = null;
var stack = NewPlant(FusionPlant.ThreeNuts); stack.mPlantHealth = 7999; ActiveNuts.LoseHeads(stack);
Check(stack.mTargetX == 1 && ActiveNuts.HealLimit(stack) == 8000, "fertilizer cannot restore first lost head");
stack.mPlantHealth = 3999; ActiveNuts.LoseHeads(stack); stack.mPlantHealth = ActiveNuts.HealLimit(stack);
Check(stack.mTargetX == 2 && stack.mPlantHealth == 4000, "second head remains lost after healing");
Target(stack.mBoard, 150); Target(stack.mBoard, 150);
NutProjectiles.Roll(stack); var roll = stack.mBoard.m_projectiles.Single();
Check(roll.mCobTargetRow == 40, "last head launches 40 damage");
for (int i = 0; i < 20 && !roll.mDead; ++i) NutProjectiles.Move(roll);
Check(stack.mBoard.m_zombies.All(z => z.Hits == 1), "overlapping zombies cannot alternate repeat hits");
Check(stack.mBoard.m_gridItems.Count == 2, "full DataID history for both targets");
var history = stack.mBoard.m_gridItems[0]; history.mBoard = stack.mBoard;
roll.Die(); NutProjectiles.TickHistory(history); Check(history.mDead, "orphan history removed");
var failed = NewPlant(FusionPlant.ThreeNuts); Target(failed.mBoard, 150); failed.mBoard.m_gridItems.Fail = true;
NutProjectiles.Roll(failed); var noHistory = failed.mBoard.m_projectiles.Single();
for (int i = 0; i < 20 && !noHistory.mDead; ++i) NutProjectiles.Move(noHistory);
Check(noHistory.mDead && failed.mBoard.m_zombies.Single().Hits == 0, "allocation failure cannot produce untracked hits");
var hopper = NewPlant(FusionPlant.SquashNut); var close = Target(hopper.mBoard, 120);
for (int i = 0; i < 50; ++i) ActiveNuts.Tick(hopper);
Check(ActiveNuts.Airborne(hopper), "jump disables blocking after original event");
for (int i = 0; i < 68; ++i) ActiveNuts.Tick(hopper);
Check(!ActiveNuts.Airborne(hopper) && close.Health == 9840, "landing restores blocking and crushes once");
int phase = hopper.mStartRow; hopper.mBoard.mPaused = true; ActiveNuts.Tick(hopper);
Check(hopper.mStartRow == phase, "pause freezes phase");
var timed = NewPlant(FusionPlant.ThreeNuts); var timedTarget = Target(timed.mBoard, 150);
NutProjectiles.Roll(timed); var timedShot = timed.mBoard.m_projectiles.Single();
Check(NutProjectiles.Remember(timedShot, timedTarget, 15), "first timed contact stored");
var timedHistory = timed.mBoard.m_gridItems.Single(); timedHistory.mBoard = timed.mBoard;
Check(!NutProjectiles.Remember(timedShot, timedTarget, 15), "same target suppressed during timeout");
timed.mBoard.mPaused = true; NutProjectiles.TickHistory(timedHistory);
Check(timedHistory.mGridItemCounter == 15, "pause freezes timed history");
timed.mBoard.mPaused = false;
for (int i = 0; i < 14; ++i) NutProjectiles.TickHistory(timedHistory);
Check(!timedHistory.mDead && timedHistory.mGridItemCounter == 1, "history persists fourteen ticks");
NutProjectiles.TickHistory(timedHistory);
Check(timedHistory.mDead && NutProjectiles.Remember(timedShot, timedTarget, 15), "contact allowed after fifteen ticks");
Console.WriteLine("Active nut contracts passed.");
