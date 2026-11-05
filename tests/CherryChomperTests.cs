using System.Reflection;
using FusionMod;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

var type = typeof(CherryChomperSwallowPatch);
var prefix = type.GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static)!;
var postfix = type.GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static)!;
int assertions = 0;
void Require(bool condition, string message)
{
    ++assertions;
    if (!condition) throw new InvalidOperationException(message);
}
void Tick(Plant plant, PlantState after)
{
    object?[] args = [plant, null];
    prefix.Invoke(null, args);
    // Simulate the native owner's observed state transition, not game code.
    plant.mState = after;
    postfix.Invoke(null, [plant, args[1]]);
}

var target = type.GetCustomAttribute<HarmonyPatch>()!;
Require(target.TargetType == typeof(Plant) && target.TargetMethod == nameof(Plant.UpdateAbilities),
    "Must patch UpdateAbilities: native caller inlines UpdateChomper.");
var plant = new Plant { mState = PlantState.ChomperBiting };
Tick(plant, PlantState.ChomperBitingGotOne);
Require(CherryBombPlants.Explosions.Count == 1, "Successful swallow must explode once.");
var effect = CherryBombPlants.Explosions[0];
Require(effect.Board == plant.mBoard && effect.Id == plant.DataID && effect.Identity == FusionPlant.CherryChomper,
    "Explosion must retain board and identity.");
Require(effect.Col == 3 && effect.Row == 1 && effect.X == 280 && effect.Y == 120,
    "Explosion must be centered one cell ahead.");
Tick(plant, PlantState.ChomperBitingGotOne);
Tick(plant, PlantState.ChomperDigesting);
Tick(plant, PlantState.ChomperDigesting);
Tick(plant, PlantState.ChomperSwallowing);
Tick(plant, PlantState.Ready);
Require(CherryBombPlants.Explosions.Count == 1, "Digestion and unchanged/loaded states must not repeat explosion.");
foreach (var state in new[] { PlantState.ChomperBitingMissed, PlantState.Ready })
{
    CherryBombPlants.Explosions.Clear();
    Tick(new Plant { mState = PlantState.ChomperBiting }, state);
    Require(CherryBombPlants.Explosions.Count == 0, "Missed or non-swallowing bite must not explode.");
}
Action<Plant>[] invalid = [p => p.Special = false, p => p.mDead = true, p => p.mIsOnBoard = false,
    p => p.mSquished = true, p => p.OffGround = true, p => p.mOnBungeeState = PlantOnBungeeState.OnBungee,
    p => p.mBoard = null, p => p.mBoard!.mApp = null,
    p => p.mBoard!.mApp!.GameScene = GameScenes.Loading,
    p => p.mBoard!.mApp!.GameMode = GameMode.ChallengeZenGarden];
foreach (var invalidate in invalid)
{
    CherryBombPlants.Explosions.Clear();
    plant = new Plant { mState = PlantState.ChomperBiting };
    invalidate(plant);
    Tick(plant, PlantState.ChomperBitingGotOne);
    Require(CherryBombPlants.Explosions.Count == 0, "Invalid/native/preview plant must not explode.");
}
CherryBombPlants.Explosions.Clear();
plant = new Plant { mState = PlantState.ChomperBiting };
CherryBombPlants.DisposingBoard = plant.mBoard!.Pointer;
Tick(plant, PlantState.ChomperBitingGotOne);
Require(CherryBombPlants.Explosions.Count == 0, "Board teardown must not explode.");
CherryBombPlants.DisposingBoard = IntPtr.Zero;
Console.WriteLine($"Cherry Chomper: {assertions} offline assertions passed; no game code loaded.");
