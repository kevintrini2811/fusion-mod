using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Native Marigold owns terrain, targeting, health and the serialized counters.
// Source Marigold invokes Produce twice after its half-second brightening.
internal static class TwinMarigold
{
    private sealed record Pickup(IntPtr Board, uint Id, CoinType Type, int Units);
    private static readonly Dictionary<IntPtr, Pickup> Pickups = new();
    internal sealed class Collection
    {
        internal Board Board = null!;
        internal int Units;
        internal bool Credited;
    }
    [ThreadStatic] internal static Collection? Collecting;

    internal static bool IsTwin(Plant plant) => FusionPlant.From(plant) == FusionPlant.TwinMarigold;

    internal static void Initialize(Plant plant)
    {
        if (!IsTwin(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchCounter = 0;
        plant.mShootingCounter = 0;
    }

    internal static CoinType Roll(int roll) => roll < 20 ? CoinType.Gold
        : roll < 95 ? CoinType.Silver : CoinType.Diamond;

    // Replanted stores money in tens: native 1/5/100 means 10/50/1000.
    internal static int Units(CoinType type) => type == CoinType.Silver ? 10
        : type == CoinType.Gold ? 30 : 100;

    internal static void Update(Plant plant)
    {
        if (!IsTwin(plant) || !ChomperFusions.Active(plant)
            || plant.mApp.GameMode is >= GameMode.PuzzleIZombie1 and <= GameMode.PuzzleIZombieEndless) return;
        if (plant.mShootingCounter > 0 && --plant.mShootingCounter == 50)
        {
            Produce(plant, -12);
            Produce(plant, 12);
        }
        if (--plant.mLaunchCounter > 0) return;
        // Source adds continuous Random.Range(-2, 2) to twenty seconds.
        plant.mLaunchCounter = 1800 + plant.mBoard.Rand(401);
        plant.mShootingCounter = 100;
    }

    private static void Produce(Plant plant, int offset)
    {
        var board = plant.mBoard;
        var type = Roll(board.Rand(100));
        var coin = board.AddCoin(plant.mX + offset, plant.mY, type, CoinMotion.Coin);
        LevelMoney.Mark(coin, Units(type));
        if (coin != null && !coin.mDead)
            Pickups[coin.Pointer] = new(board.Pointer, coin.DataID, type, Units(type));
    }

    internal static float Brightness(Plant plant) => plant.mShootingCounter <= 0 ? 1
        : 1 + 3 * (1 - Math.Abs(plant.mShootingCounter - 50) / 50f);

    internal static Collection? BeginCollection(Coin coin) => !coin.mDead && LevelMoney.Units(coin) > 0
        ? new Collection { Board = coin.mBoard, Units = LevelMoney.Units(coin) }
        : !coin.mDead && Pickups.TryGetValue(coin.Pointer, out var pickup)
        && pickup.Id == coin.DataID && pickup.Board == coin.mBoard?.Pointer && pickup.Type == coin.mType
        ? new Collection { Board = coin.mBoard, Units = pickup.Units } : null;

    internal static void Forget(Coin coin) => Pickups.Remove(coin.Pointer);
    internal static void ForgetBoard(Board board)
    {
        foreach (var key in Pickups.Where(p => p.Value.Board == board.Pointer).Select(p => p.Key).ToArray())
            Pickups.Remove(key);
    }
    internal static void Clear() => Pickups.Clear();
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class TwinMarigoldInitializePatch
{
    private static void Postfix(Plant __instance) => TwinMarigold.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateProductionPlant))]
internal static class TwinMarigoldProductionPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!TwinMarigold.IsTwin(__instance)) return true;
        TwinMarigold.Update(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Coin), nameof(Coin.ScoreCoin))]
internal static class TwinMarigoldCollectionScope
{
    internal static void Prefix(Coin __instance, out TwinMarigold.Collection? __state)
    {
        __state = TwinMarigold.Collecting;
        // ScoreCoin destroys the pickup before its inlined value/credit path.
        TwinMarigold.Collecting = TwinMarigold.BeginCollection(__instance);
    }
    internal static void Finalizer(TwinMarigold.Collection? __state) => TwinMarigold.Collecting = __state;
}

[HarmonyPatch(typeof(UserService), nameof(UserService.AddCoins))]
internal static class TwinMarigoldCreditPatch
{
    internal static void Prefix(ref int theAmount, out int __state)
    {
        __state = 0;
        var collection = TwinMarigold.Collecting;
        if (collection == null || collection.Credited) return;
        collection.Credited = true;
        __state = collection.Units - theAmount;
        theAmount = collection.Units;
    }
    internal static void Postfix(int __state)
    {
        // Native ScoreCoin adds its original value after AddCoins returns.
        if (TwinMarigold.Collecting is { } collection) collection.Board.mCoinsCollected += __state;
    }
}

[HarmonyPatch]
internal static class TwinMarigoldCoinCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Coin), "Die");
        yield return AccessTools.Method(typeof(Coin), "Reset");
    }
    private static void Prefix(Coin __instance) => TwinMarigold.Forget(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class TwinMarigoldBoardCleanupPatch
{
    private static void Prefix(Board __instance) => TwinMarigold.ForgetBoard(__instance);
}

// Production owns mShootingCounter. Native UpdateAbilities calls the general
// shooter for every seed; its final countdown tick otherwise calls Marigold.Fire.
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class TwinMarigoldShootingPatch
{
    internal static bool Prefix(Plant __instance) => !TwinMarigold.IsTwin(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class TwinMarigoldFirePatch
{
    internal static bool Prefix(Plant __instance) => !TwinMarigold.IsTwin(__instance);
}
