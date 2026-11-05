using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class SilverMoney
{
    // Native money has a ten-coin minimum. Carry the half-unit in a saved
    // plant field: alternate 20/30 instead of rounding every 25-coin payout.
    internal static int MoneyUnits(Plant plant)
    {
        int remainder = plant.mTargetY == 5 ? 5 : 0;
        int amount = 25 + remainder;
        plant.mTargetY = amount % 10;
        return amount / 10;
    }

    internal static void DropMoney(Board board, float x, float y, int units)
    {
        for (int i = 0; i < units; ++i)
            LevelMoney.Mark(board.AddCoin(x + (i - 1) * 10, y, CoinType.Silver, CoinMotion.Coin));
    }

}
