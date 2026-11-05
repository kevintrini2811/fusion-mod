using FusionMod;
using Il2CppReloaded.Gameplay;

void Check(bool condition, string message)
{ if (!condition) throw new Exception(message); }
var board = new Board();
var ground = new Plant { mSeedType = SeedType.Peashooter, mPlantCol = 2, mRow = 2 };
board.m_plants.Add(ground);
Check(AirPlants.Owns(new Plant { mSeedType = SeedType.Blover }), "Native flying plants use air placement");
Check(AirPlants.Owns(new Plant { mSeedType = SeedType.Scaredyshroom, Camo = true }), "Camo uses air placement");
Check(!AirPlants.Owns(ground), "Ground plants retain their placement layer");
Check(!AirPlants.Owns(new Plant { mSeedType = SeedType.InstantCoffee }), "Native coffee classification never creates a mod air occupant");
Check(AirPlants.Owns(new Plant { mSeedType = SeedType.Chomper, Fly = true }), "Flytrapper shares the air layer");
Check(!AirPlants.Owns(new Plant { mSeedType = SeedType.Chomper }), "Ordinary Chomper retains its ground layer");
Check(AirPlants.Terrain(board, 2, 2) == PlantingReason.Ok && ground.mIsOnBoard,
    "Air placement preserves the occupied ground layer");
Check(board.QueriedType == SeedType.Blover, "Air placement uses native flight rules");
var air = new Plant { mSeedType = SeedType.Scaredyshroom, Camo = true, mPlantCol = 2, mRow = 2 };
board.m_plants.Add(air);
Check(AirPlants.Terrain(board, 2, 2) == PlantingReason.NotHere, "Camo blocks a second air occupant");
Check(AirPlants.Terrain(board, 2, 2, air) == PlantingReason.Ok && air.mIsOnBoard,
    "Replacing air excludes only that instance");
air.mSeedType = SeedType.Blover; air.Camo = false;
Check(AirPlants.Terrain(board, 2, 2) == PlantingReason.NotHere, "Native flying occupant blocks Camo too");
air.mSeedType = SeedType.Chomper; air.Fly = true;
Check(AirPlants.Terrain(board, 2, 2) == PlantingReason.NotHere, "Flytrapper blocks a second air occupant");
Check(AirPlants.Terrain(board, 2, 2, air) == PlantingReason.Ok && ground.mIsOnBoard,
    "Flytrapper placement preserves the independent ground occupant");
board.ThrowTerrain = true;
try { AirPlants.Terrain(board, 2, 2, air); throw new Exception("Expected terrain exception"); }
catch (InvalidOperationException) { }
Check(air.mIsOnBoard && ground.mIsOnBoard, "Terrain exception preserves both occupants");
board.ThrowTerrain = false; board.Rule = PlantingReason.NotOnWater;
Check(AirPlants.Terrain(board, 2, 2, air) == PlantingReason.NotOnWater, "Native challenge restrictions remain authoritative");
board.Rule = PlantingReason.NeedsPot;
Check(AirPlants.Terrain(board, 2, 2, air) == PlantingReason.Ok, "Air needs no roof pot");
board.Rule = PlantingReason.Ok; board.Water = true;
Check(AirPlants.Terrain(board, 2, 2, air) == PlantingReason.Ok && board.QueriedType == SeedType.Seashroom,
    "Air validates water terrain without a lily pad");
Check(AirPlants.Terrain(board, -1, 2) == PlantingReason.NotHere, "Outside board rejected");
Console.WriteLine("Air placement checks passed.");
