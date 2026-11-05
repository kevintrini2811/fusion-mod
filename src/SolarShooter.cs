using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Peashooter owns targeting and shooting. Extra state never overwrites
// unrelated native counters or changes the save stream format.
internal static class SolarShooter
{
    private sealed class Producer
    {
        internal IntPtr Board;
        internal uint Id;
        internal int Countdown;
        internal int Shots;
        internal int Glow;
        internal bool FireReported;
        internal bool ProjectileReported;
    }
    internal sealed record Shot(IntPtr Board, uint Id, bool DropsSun);
    private static readonly Dictionary<IntPtr, Producer> Producers = new();
    private static readonly Dictionary<IntPtr, Shot> Shots = new();
    private static readonly Dictionary<IntPtr, (IntPtr Board, uint Id, int Value)> Suns = new();
    [ThreadStatic] internal static Plant? FiringPlant;
    [ThreadStatic] internal static IntPtr ImpactZombie;
    [ThreadStatic] internal static bool DamagePending;
    [ThreadStatic] internal static IntPtr ScoringBoard;
    [ThreadStatic] internal static int ScoringValue;

    private static Producer State(Plant plant)
    {
        if (!Producers.TryGetValue(plant.Pointer, out var state)
            || state.Id != plant.DataID || state.Board != plant.mBoard.Pointer)
            Producers[plant.Pointer] = state = new Producer
            {
                Board = plant.mBoard.Pointer, Id = plant.DataID,
                Countdown = 400 + plant.mBoard.Rand(301)
            };
        return state;
    }

    internal static bool IsSolar(Plant plant) => FusionPlant.From(plant) == FusionPlant.SolarShooter;

    internal static void Update(Plant plant)
    {
        var board = plant.mBoard;
        if (!IsSolar(plant) || board == null || plant.mApp == null
            || plant.mApp.GameScene != GameScenes.Playing || board.mPaused
            || plant.mApp.GameMode == GameMode.ChallengeZenGarden
            || plant.mApp.GameMode is >= GameMode.PuzzleIZombie1 and <= GameMode.PuzzleIZombieEndless
            || plant.mDead || !plant.mIsOnBoard || plant.mSquished || plant.NotOnGround()
            || plant.mOnBungeeState != PlantOnBungeeState.NotOnBungee) return;
        var state = State(plant);
        if (state.Glow > 0) --state.Glow;
        // Production is independent of targets and of the native launch timer.
        if (--state.Countdown == 50) state.Glow = 120;
        if (state.Countdown > 0) return;
        board.AddCoin(plant.mX, plant.mY, CoinType.Sun, CoinMotion.FromPlant);
        state.Countdown = (25 + board.Rand(5) - 2) * 100;
    }

    internal static float Brightness(Plant plant)
    {
        if (!Producers.TryGetValue(plant.Pointer, out var state) || state.Id != plant.DataID
            || state.Board != plant.mBoard?.Pointer || state.Glow <= 0) return 1;
        return 1 + 3 * (1 - Math.Abs(state.Glow - 60) / 60f);
    }

    internal static void ObserveFire(Plant plant)
    {
        var state = State(plant);
        if (state.FireReported) return;
        state.FireReported = true;
        FusionMod.Instance.LoggerInstance.Msg($"SolarShooter fire scope ready: plant={(uint)plant.DataID}, board={plant.mBoard.Pointer}.");
    }

    internal static void Track(Board board, Projectile projectile)
    {
        var plant = FiringPlant;
        if (plant == null || plant.mBoard?.Pointer != board.Pointer || projectile == null
            || projectile.mDead) return;
        // Replanted's native Peashooter emits its dedicated PeashooterPea (14).
        // Pea (0) belongs to other native pea shooters. Both are ordinary peas;
        // only this Solar firing scope may attach the solar provenance.
        if (projectile.mProjectileType is not (ProjectileType.Pea or ProjectileType.PeashooterPea)) return;
        if (Shots.TryGetValue(projectile.Pointer, out var old)
            && old.Id == projectile.DataID && old.Board == board.Pointer) return;
        var state = State(plant);
        state.Shots = (state.Shots + 1) % 3;
        Shots[projectile.Pointer] = new Shot(board.Pointer, projectile.DataID, state.Shots == 0);
        ImportedPeaVisuals.RegisterSolarProjectile(projectile);
        if (!state.ProjectileReported)
        {
            state.ProjectileReported = true;
            FusionMod.Instance.LoggerInstance.Msg($"SolarShooter projectile registered after initialization: plant={(uint)plant.DataID}, projectile={(uint)projectile.DataID}, type={projectile.mProjectileType}, board={board.Pointer}.");
        }
    }

    internal static bool TryShot(Projectile projectile, out Shot shot) =>
        Shots.TryGetValue(projectile.Pointer, out shot!) && shot.Id == projectile.DataID
        && shot.Board == projectile.mBoard?.Pointer;

    internal static void DropSun(Projectile projectile)
    {
        // A distinct physical pickup keeps native collection and Sun Collector working.
        DropSun(projectile.mBoard, projectile.mPosX, projectile.mPosY, 5);
    }

    internal static void DropSun(Board board, float x, float y, int value)
    {
        var coin = board.AddCoin(x, y, CoinType.SmallSun, CoinMotion.FromPlant);
        if (coin != null)
        {
            // Small Sun never uses the serialized usable-seed field. Preserve
            // five-sun pickups across native saves instead of only in a cache.
            if (value == 5) coin.mUsableSeedType = SeedType.Sunflower;
            else Suns[coin.Pointer] = (board.Pointer, coin.DataID, value);
        }
    }

    internal static int CustomSunValue(Coin coin) => coin.mType == CoinType.SmallSun
        && coin.mUsableSeedType == SeedType.Sunflower ? 5 : Suns.TryGetValue(coin.Pointer, out var sun)
        && sun.Id == coin.DataID && sun.Board == coin.mBoard?.Pointer && coin.mType == CoinType.SmallSun
        ? sun.Value : 0;
    internal static void Forget(Plant plant) => Producers.Remove(plant.Pointer);
    internal static void Forget(Projectile projectile)
    { Shots.Remove(projectile.Pointer); ImportedPeaVisuals.ForgetProjectile(projectile); }
    internal static void Forget(Coin coin) => Suns.Remove(coin.Pointer);
    internal static void ForgetBoard(Board board)
    {
        foreach (var key in Producers.Where(e => e.Value.Board == board.Pointer).Select(e => e.Key).ToArray()) Producers.Remove(key);
        foreach (var key in Shots.Where(e => e.Value.Board == board.Pointer).Select(e => e.Key).ToArray()) Shots.Remove(key);
        foreach (var key in Suns.Where(e => e.Value.Board == board.Pointer).Select(e => e.Key).ToArray()) Suns.Remove(key);
    }
    internal static void Clear() { Producers.Clear(); Shots.Clear(); Suns.Clear(); }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SolarProductionPatch
{
    private static void Postfix(Plant __instance) => SolarShooter.Update(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class SolarFireScope
{
    private static void Prefix(Plant __instance, out Plant? __state)
    {
        __state = SolarShooter.FiringPlant;
        SolarShooter.FiringPlant = SolarShooter.IsSolar(__instance) ? __instance : null;
        if (SolarShooter.FiringPlant != null) SolarShooter.ObserveFire(__instance);
    }
    private static void Finalizer(Plant? __state) => SolarShooter.FiringPlant = __state;
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ProjectileInitialize))]
internal static class SolarProjectileInitializedPatch
{
    // Both native AddProjectile overloads assign Board and then call this
    // initializer directly. Capture the initialized object itself; do not rely
    // on overload return wrappers for source provenance and original visuals.
    private static void Postfix(Projectile __instance)
    {
        if (__instance.mBoard != null) SolarShooter.Track(__instance.mBoard, __instance);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class SolarImpactScope
{
    private static void Prefix(Projectile __instance, Zombie theZombie, out (IntPtr Zombie, bool Pending) __state)
    {
        __state = (SolarShooter.ImpactZombie, SolarShooter.DamagePending);
        SolarShooter.ImpactZombie = IntPtr.Zero;
        SolarShooter.DamagePending = false;
        if (!SolarShooter.TryShot(__instance, out var shot) || theZombie == null
            || theZombie.mDead || theZombie.IsDeadOrDying()) return;
        SolarShooter.ImpactZombie = theZombie.Pointer;
        SolarShooter.DamagePending = true;
        if (shot.DropsSun) SolarShooter.DropSun(__instance);
    }
    private static void Finalizer((IntPtr Zombie, bool Pending) __state)
    { SolarShooter.ImpactZombie = __state.Zombie; SolarShooter.DamagePending = __state.Pending; }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class SolarDamagePatch
{
    private static void Prefix(Zombie __instance, ref int theDamage)
    {
        if (!SolarShooter.DamagePending || SolarShooter.ImpactZombie != __instance.Pointer) return;
        SolarShooter.DamagePending = false;
        theDamage = 30;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class SolarTorchwoodPatch
{
    // Original solar bullets are a separate bullet type, not combustible peas.
    private static bool Prefix(Projectile __instance) => !SolarShooter.TryShot(__instance, out _);
}

[HarmonyPatch(typeof(Coin), nameof(Coin.GetSunValue))]
internal static class SolarSunValuePatch
{
    private static void Postfix(Coin __instance, ref int __result)
    { if (SolarShooter.CustomSunValue(__instance) is > 0 and var value) __result = value; }
}

[HarmonyPatch(typeof(Coin), nameof(Coin.ScoreCoin))]
internal static class SolarSunScoreScope
{
    private static void Prefix(Coin __instance, out (IntPtr Board, int Value) __state)
    {
        __state = (SolarShooter.ScoringBoard, SolarShooter.ScoringValue);
        // ScoreCoin inlines GetSunValue and calls Die before crediting money.
        // Capture provenance first, before normal lifecycle cleanup removes it.
        SolarShooter.ScoringValue = SolarShooter.CustomSunValue(__instance);
        SolarShooter.ScoringBoard = SolarShooter.ScoringValue > 0 ? __instance.mBoard.Pointer : IntPtr.Zero;
    }
    private static void Finalizer((IntPtr Board, int Value) __state)
    { SolarShooter.ScoringBoard = __state.Board; SolarShooter.ScoringValue = __state.Value; }
}

[HarmonyPatch(typeof(Board), nameof(Board.AddSunMoney))]
internal static class SolarSunCreditPatch
{
    private static void Prefix(Board __instance, ref int theAmount)
    {
        if (SolarShooter.ScoringBoard != __instance.Pointer) return;
        SolarShooter.ScoringBoard = IntPtr.Zero;
        theAmount = SolarShooter.ScoringValue;
    }
}

[HarmonyPatch]
internal static class SolarCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        foreach (var type in new[] { typeof(Plant), typeof(Projectile), typeof(Coin) })
            foreach (var name in new[] { "Die", "Reset" }) yield return AccessTools.Method(type, name);
    }
    private static void Prefix(object __instance)
    {
        if (__instance is Plant plant) SolarShooter.Forget(plant);
        else if (__instance is Projectile projectile) SolarShooter.Forget(projectile);
        else if (__instance is Coin coin) SolarShooter.Forget(coin);
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class SolarBoardCleanupPatch
{
    private static void Prefix(Board __instance) => SolarShooter.ForgetBoard(__instance);
}
