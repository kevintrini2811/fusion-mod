namespace FusionMod;

// Existing plant suites isolate currency routing. The native collection,
// serialization and optional-mod paths require the user's integration checks.
internal static class LevelMoney
{
    internal static void Mark<T>(T coin, int units = 0) { }
    internal static int Units<T>(T coin) => 0;
}
internal static class OptionalDifficulty
{
    internal static int EyesChance<T>(T board) => 1;
}
