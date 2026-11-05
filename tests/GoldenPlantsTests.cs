using FusionMod;
using Il2CppReloaded.Gameplay;

static void Check(bool pass, string name) { if (!pass) throw new Exception(name); }
static Plant Add(Board b, FusionPlant identity) => b.AddPlant(2, 1, identity.NativeType, identity.Marker)!;
static void Money(Board b, int amount)
{
    Check(GoldenBean.Credit(b, amount), "fund level wallet");
}
static void Tick(Board b, int ticks) { for (int i = 0; i < ticks; ++i) GoldenPlants.Tick(b); }

var b = new Board(); var silver = Add(b, FusionPlant.SilverNut); Money(b, 1000);
foreach (var failure in new[] { "terrain", "allocation", "pause", "carried" })
{
    FusionService.Fail = failure == "terrain"; b.FailPlant = failure == "allocation";
    b.mPaused = failure == "pause"; LawnGlove.LawnGloveMod.Carried = failure == "carried" ? silver : null;
    Check(!GoldenBean.Use(b, silver) && !silver.mDead && GoldenBean.Balance(b) == 1000, "atomic upgrade: " + failure);
}
FusionService.Fail = b.FailPlant = b.mPaused = false; LawnGlove.LawnGloveMod.Carried = null;
Check(GoldenBean.Use(b, silver) && silver.mDead && GoldenBean.Balance(b) == 0, "paid upgrade");
var nut = b.m_plants.Last();
Check(FusionPlant.From(nut) == FusionPlant.GoldenNut && nut.mPlantHealth == 9000 && nut.mPlantMaxHealth == 9000, "free first ultimate");
Money(b, 2000); Check(!GoldenBean.Use(b, nut) && GoldenBean.Balance(b) == 2000, "cooldown retains money");
Tick(b, 500); nut.mPlantHealth = 1;
Check(GoldenBean.Use(b, nut) && nut.mPlantHealth == 10000 && GoldenBean.Balance(b) == 1000, "nut heals and increases maximum");
var c = new Board(); var a = Add(c, FusionPlant.GoldenSunflower); var other = Add(c, FusionPlant.GoldenSunflower);
Check(GoldenPlants.UpgradeCost(c, FusionPlant.SilverSunflower) == 12000, "living count doubles price");
Tick(c, 499); Check(a.mStartRow == 0, "initial production delay");
Tick(c, 1); Check(a.mStartRow == 50 && other.mStartRow == 50, "initial production");
c.mPaused = true; Tick(c, 1000); Check(a.mStartRow == 50 && a.mLaunchCounter == 1000, "pause stops saved clocks"); c.mPaused = false;
GoldenPlants.Fertilize(a); Check(a.mLaunchCounter == 50 && a.mStartRow == 50, "fertilizer accelerates without payout");
Tick(c, 50); Check(a.mStartRow == 100, "fertilized production");
a.mStartRow = 100; other.mStartRow = 50; Money(c, 2000);
Check(GoldenBean.Use(c, a) && GoldenBean.Balance(c) == 1000, "shared payout charged once");
Check(!GoldenBean.Use(c, other) && GoldenBean.Balance(c) == 1000, "no overlapping payout");
int before = c.Coins.Count; c.FailCoin = true; Tick(c, 1); Check(a.mStartRow == 100 && other.mStartRow == 50, "failed pickup keeps stored sun");
c.FailCoin = false; Tick(c, 20);
Check(a.mStartRow == 0 && other.mStartRow == 0 && c.Coins.Count - before == 3, "all banks pay once");
a.mStartRow = 9900; other.mStartRow = 9950; c.mSunMoney[0].Amount = 49975;
GoldenSunCapacityPatch.Prefix(c, 200, 0, out var saved);
Check(saved == 49975, "capacity captures original amount");
GoldenSunCapacityPatch.Postfix(c, 200, 0, saved);
Check(c.mSunMoney[0].Amount == 50000 && a.mStartRow == 10000 && other.mStartRow == 10000, "overflow fills newest available banks");
var ordinary = new Board(); Check(!GoldenPlants.IsGolden(FusionPlant.From(Add(ordinary, SeedType.Wallnut))), "native plants unchanged");
LawnGlove.LawnGloveMod.Carried = a;
Check(GoldenPlants.UpgradeCost(c, FusionPlant.SilverSunflower) == 12000, "carrying does not discount upgrades");
LawnGlove.LawnGloveMod.Carried = null;
var remainderBoard = new Board(); var remainderPlant = Add(remainderBoard, FusionPlant.GoldenSunflower);
remainderPlant.mStartRow = 25; Check(GoldenPlants.Ultimate(remainderPlant), "partial bank starts payout");
Tick(remainderBoard, 1);
Check(remainderBoard.mSunMoney[0].Amount == 25 && remainderBoard.Coins.Count == 0 && remainderPlant.mStartRow == 0, "partial overflow never rounds up");
Console.WriteLine("Golden plant transaction, timer, payout and capacity checks passed.");
