using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Money never uses the native seed-packet field. Store provenance and value
// there so collection after loading has the same destination and payout.
internal static class LevelMoney
{
    private const int Marker = -5211000;
    [ThreadStatic] internal static int Producing;
    [ThreadStatic] internal static int ZombieLoot;
    internal sealed record Collection(Board Board, int Amount);
    [ThreadStatic] internal static Collection? Collecting;
    internal static int NativeUnits(CoinType type) => type switch
    { CoinType.Silver => 1, CoinType.Gold => 5, CoinType.Diamond => 100, _ => 0 };
    internal static int Units(Coin coin)
    {
        int units = Marker - (int)coin.mUsableSeedType;
        return NativeUnits(coin.mType) > 0 && units is > 0 and <= 100 ? units : 0;
    }
    internal static void Mark(Coin? coin, int units = 0)
    {
        if (coin == null || coin.mDead || NativeUnits(coin.mType) == 0) return;
        if (units == 0) units = NativeUnits(coin.mType);
        coin.mUsableSeedType = (SeedType)(Marker - units);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateProductionPlant))]
internal static class PlantMoneyProductionScope
{
    private static void Prefix(out int __state) { __state = LevelMoney.Producing; ++LevelMoney.Producing; }
    private static void Finalizer(int __state) => LevelMoney.Producing = __state;
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.DropLoot))]
internal static class ZombieMoneyProductionScope
{
    private static void Prefix(out int __state) { __state = LevelMoney.ZombieLoot; ++LevelMoney.ZombieLoot; }
    private static void Finalizer(int __state) => LevelMoney.ZombieLoot = __state;
}
[HarmonyPatch(typeof(Board), nameof(Board.AddCoin))]
internal static class PlantMoneyBornPatch
{
    private static void Postfix(Coin __result)
    {
        if (LevelMoney.Producing > 0 && LevelMoney.ZombieLoot == 0) LevelMoney.Mark(__result);
    }
}
[HarmonyPatch(typeof(Coin), nameof(Coin.ScoreCoin))]
internal static class LevelMoneyCollectionScope
{
    private static void Prefix(Coin __instance, out LevelMoney.Collection? __state)
    {
        __state = LevelMoney.Collecting;
        int units = LevelMoney.Units(__instance);
        LevelMoney.Collecting = !__instance.mDead && units > 0 && __instance.mBoard is { } board
            && GoldenBean.Playing(board) ? new(board, units * 10) : null;
    }
    private static void Finalizer(LevelMoney.Collection? __state) => LevelMoney.Collecting = __state;
}
[HarmonyPatch(typeof(UserService), nameof(UserService.AddCoins))]
internal static class LevelMoneyCreditPatch
{
    [HarmonyPriority(Priority.Last)]
    private static bool Prefix() => LevelMoney.Collecting is not { } collection
        || !GoldenBean.Credit(collection.Board, collection.Amount);
}
