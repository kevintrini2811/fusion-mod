using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Source bullet 177: direct damage, one poison level, then a miasma if
// poisoned. Native shooters/collision own shot timing, targets and reflection.
internal static class PoisonShooters
{
    internal const int GarlicTorchMarker = -622002, ToxicTorchMarker = -622003;
    private sealed record Shot(IntPtr Board, uint Id);
    private sealed class Poison
    {
        internal Zombie Zombie = null!;
        internal IntPtr Board;
        internal uint Id;
        internal int Level, Remaining;
        internal int DamageCountdown = 1;
    }
    private sealed record Cloud(Board Board, int Row, float X, float Y, int Order, int Started)
    {
        internal int NextPulse = Started + 100;
    }
    internal sealed class Impact
    {
        internal Zombie Zombie = null!;
        internal Board Board = null!;
        internal uint Id;
        internal int Row, Order;
        internal float X, Y;
        internal bool Hit, WasPoisoned;
    }
    private static readonly Dictionary<IntPtr, Shot> Shots = new();
    private static readonly Dictionary<IntPtr, Poison> Poisons = new();
    private static readonly List<Cloud> Clouds = new();
    private static readonly Dictionary<IntPtr, int> LastTicks = new();
    [ThreadStatic] internal static Plant? FiringPlant;
    [ThreadStatic] internal static Impact? CurrentImpact;

    internal static bool Contains(Projectile projectile) => projectile.mCobTargetX != ToxicTorchMarker && (projectile.mCobTargetX == GarlicTorchMarker || Shots.TryGetValue(projectile.Pointer, out var shot)
        && shot.Id == projectile.DataID && shot.Board == projectile.mBoard?.Pointer);

    internal static void Track(Projectile projectile)
    {
        var plant = FiringPlant;
        if (plant == null || !FusionPlant.IsPoisonShooter(FusionPlant.From(plant))
            || projectile.mDead || projectile.mBoard == null || plant.mBoard?.Pointer != projectile.mBoard.Pointer
            || projectile.mProjectileType is not (ProjectileType.Pea or ProjectileType.PeashooterPea)) return;
        if (!Contains(projectile)) Shots[projectile.Pointer] = new(projectile.mBoard.Pointer, projectile.DataID);
        ImportedPeaVisuals.RegisterPoisonProjectile(projectile);
    }

    private static bool LiveEnemy(Board board, Zombie zombie) => zombie.mBoard?.Pointer == board.Pointer
        && !zombie.mDead && !zombie.IsDeadOrDying() && !zombie.mMindControlled;

    private static bool CanPoison(Board board, Zombie zombie) => LiveEnemy(board, zombie)
        && zombie.mZombieType is not (ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target);

    private static Poison? State(Zombie zombie) => Poisons.TryGetValue(zombie.Pointer, out var poison)
        && poison.Id == zombie.DataID && poison.Board == zombie.mBoard?.Pointer ? poison : null;

    internal static int Level(Zombie zombie) => State(zombie)?.Level ?? 0;
    internal static bool IsPoisoned(Zombie zombie) => State(zombie)?.Remaining > 0;
    internal static (int Level, int Remaining, int Countdown)? Capture(Zombie zombie)
    {
        var state = State(zombie);
        return state == null ? null : (state.Level, state.Remaining, state.DamageCountdown);
    }
    internal static void Restore(Zombie zombie, int level, int remaining, int countdown)
    {
        if (State(zombie) != null || zombie.mBoard == null || !CanPoison(zombie.mBoard, zombie)) return;
        Poisons[zombie.Pointer] = new() { Zombie = zombie, Board = zombie.mBoard.Pointer,
            Id = zombie.DataID, Level = Math.Clamp(level, 0, 30), Remaining = Math.Max(0, remaining),
            DamageCountdown = Math.Clamp(countdown, 1, 100) };
        if (!LastTicks.TryGetValue(zombie.mBoard.Pointer, out int previous) || previous > zombie.mBoard.mMainCounter)
            LastTicks[zombie.mBoard.Pointer] = zombie.mBoard.mMainCounter;
    }
    internal static int CloudCount(Board board) => Clouds.Count(c => c.Board.Pointer == board.Pointer);

    internal static Impact? BeginImpact(Projectile projectile, Zombie? zombie)
    {
        if (!Contains(projectile) || projectile.mDead || zombie == null
            || !LiveEnemy(projectile.mBoard, zombie)) return null;
        var rect = zombie.GetZombieRect();
        return new() { Zombie = zombie, Board = projectile.mBoard, Id = zombie.DataID,
            Row = zombie.mRow, X = (rect.xMin + rect.xMax) / 2, Y = zombie.mPosY,
            Order = zombie.mRenderOrder, WasPoisoned = IsPoisoned(zombie) };
    }

    internal static void FinishImpact(Impact? impact)
    {
        if (impact == null || !impact.Hit || impact.Zombie.DataID != impact.Id
            || impact.Zombie.mBoard?.Pointer != impact.Board.Pointer) return;
        AddLevel(impact.Board, impact.Zombie);
        // A lethal hit on an already poisoned zombie also leaves a cloud.
        if (impact.WasPoisoned || IsPoisoned(impact.Zombie))
            MakeCloud(impact.Board, impact.Row, impact.X, impact.Y, impact.Order);
    }

    internal static void AddLevel(Board board, Zombie zombie)
    {
        if (!CanPoison(board, zombie)) return;
        if (!LastTicks.ContainsKey(board.Pointer)) LastTicks[board.Pointer] = board.mMainCounter;
        var poison = State(zombie);
        if (poison == null)
        {
            poison = new() { Zombie = zombie, Board = board.Pointer, Id = zombie.DataID };
            Poisons[zombie.Pointer] = poison;
        }
        // The sixth and later levels call SetPoison(5), which also increments
        // the level (up to 30) and extends the existing poison duration.
        if (poison.Level < 5) ++poison.Level;
        else ApplyPoison(board, zombie, 500);
    }

    internal static void ApplyPoison(Board board, Zombie zombie, int ticks)
    {
        if (!CanPoison(board, zombie)) return;
        var poison = State(zombie);
        if (poison == null)
        {
            poison = new() { Zombie = zombie, Board = board.Pointer, Id = zombie.DataID };
            Poisons[zombie.Pointer] = poison;
        }
        if (!LastTicks.ContainsKey(board.Pointer)) LastTicks[board.Pointer] = board.mMainCounter;
        poison.Remaining = (int)Math.Min(int.MaxValue, (long)poison.Remaining + ticks);
        poison.Level = Math.Min(30, poison.Level + 1);
    }

    internal static void PoisonBite(Board board, Zombie zombie) => ApplyPoison(board, zombie, 1000);

    internal static void TorchCloud(Board board, int row, float x, float y, int order) => MakeCloud(board, row, x, y, order);

    private static void MakeCloud(Board board, int row, float x, float y, int order)
    {
        // SetMiasma's non-sniper branch permits one stationary cloud per row.
        // Further hits neither move it nor restart its six-second lifetime.
        if (Clouds.Any(c => c.Board.Pointer == board.Pointer && c.Row == row)) return;
        if (!LastTicks.ContainsKey(board.Pointer)) LastTicks[board.Pointer] = board.mMainCounter;
        Clouds.Add(new(board, row, x, y, order, board.mMainCounter));
        ShowCloud(board, x, y, order);
    }

    private static void ShowCloud(Board board, float x, float y, int order) =>
        ImportedPeaVisuals.GarlicCloud(board, x, y, order);

    internal static int RowDamage(Board board, int row)
    {
        int shots = 0;
        var plants = board.m_plants;
        for (int i = 0; i < plants.Count; ++i)
        {
            var plant = plants[i];
            if (plant == null || !ChomperFusions.Active(plant)) continue;
            var identity = FusionPlant.From(plant);
            if (plant.mRow != row) continue;
            // Miasma.Update counts source type 1328 as three shots in the
            // plant's own row, independently of the destination of each pea.
            shots += identity == FusionPlant.StenchSpreader ? 3 : identity == FusionPlant.PoisonPea ? 1 : identity == FusionPlant.PoisonRepeater ? 2
                : identity == FusionPlant.SplitPoison ? 3 : identity == FusionPlant.GatlingPoison ? 4 : 0;
        }
        return shots * 20;
    }

    private static void Pulse(Cloud cloud)
    {
        var board = cloud.Board;
        int damage = RowDamage(board, cloud.Row);
        var targets = new List<(Zombie Zombie, uint Id)>();
        var zombies = board.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || !CanPoison(board, zombie) || zombie.mRow != cloud.Row
                || !zombie.EffectedByDamage(DamageRangeFlags.Ground | DamageRangeFlags.Flying
                    | DamageRangeFlags.Submerged | DamageRangeFlags.Dog | DamageRangeFlags.OffGround
                    | DamageRangeFlags.Underground)) continue;
            var rect = zombie.GetZombieRect();
            float dx = cloud.X - Math.Clamp(cloud.X, rect.xMin, rect.xMax);
            // 0.74 source cells; the native board uses 80 horizontal units/cell.
            if (dx * dx <= 59.2f * 59.2f) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !CanPoison(board, zombie)) continue;
            AddLevel(board, zombie);
            if (damage > 0) zombie.TakeDamage(damage, DamageFlags.BypassesShield);
        }
        ShowCloud(board, cloud.X, cloud.Y, cloud.Order);
    }

    internal static void Update(Board board)
    {
        if (board.mPaused || board.Pointer == CherryBombPlants.DisposingBoard || board.mApp == null
            || board.mApp.GameScene != GameScenes.Playing || board.mApp.GameMode == GameMode.ChallengeZenGarden) return;
        if (!LastTicks.TryGetValue(board.Pointer, out int previous)) return;
        int now = board.mMainCounter;
        if (now < previous) { ForgetBoard(board); return; }
        int elapsed = now - previous;
        if (elapsed == 0) return;
        LastTicks[board.Pointer] = now;
        // Snapshot: damage/death callbacks can create plants or clear states.
        foreach (var (key, poison) in Poisons.Where(p => p.Value.Board == board.Pointer).ToArray())
        {
            var zombie = poison.Zombie;
            // An earlier damage callback can reuse a later snapshot entry's
            // pool slot. Never erase the new occupant's independently added state.
            if (!Poisons.TryGetValue(key, out var current) || current != poison) continue;
            if (State(zombie) != poison || !CanPoison(board, zombie)) { Poisons.Remove(key); continue; }
            if (poison.Remaining <= 0) continue;
            // The source suspends duration while frozen, but keeps damage ticks.
            int activeTicks = zombie.mIceTrapCounter > 0 || EmberEffects.Has(zombie) ? elapsed : Math.Min(elapsed, poison.Remaining);
            poison.DamageCountdown -= activeTicks;
            if (zombie.mIceTrapCounter == 0 && !EmberEffects.Has(zombie)) poison.Remaining -= activeTicks;
            while (poison.DamageCountdown <= 0 && CanPoison(board, zombie) && State(zombie) == poison)
            {
                poison.DamageCountdown += 100;
                zombie.TakeDamage(poison.Level * 20, DamageFlags.BypassesShield);
            }
            if (poison.Remaining <= 0 && State(zombie) == poison)
                Poisons.Remove(key); // OnRemove resets levels.
        }
        foreach (var cloud in Clouds.Where(c => c.Board.Pointer == board.Pointer).ToArray())
        {
            // Source destroys only when lifeTimer > 6, permitting the t=6 pulse.
            if (now - cloud.Started > 600) { Clouds.Remove(cloud); continue; }
            if (now < cloud.NextPulse) continue;
            cloud.NextPulse = now + 100;
            Pulse(cloud);
        }
    }

    internal static void Forget(Projectile projectile)
    { Shots.Remove(projectile.Pointer); ImportedPeaVisuals.ForgetProjectile(projectile); }
    internal static void Forget(Zombie zombie) => Poisons.Remove(zombie.Pointer);
    internal static void ForgetBoard(Board board)
    {
        foreach (var key in Shots.Where(p => p.Value.Board == board.Pointer).Select(p => p.Key).ToArray()) Shots.Remove(key);
        foreach (var key in Poisons.Where(p => p.Value.Board == board.Pointer).Select(p => p.Key).ToArray()) Poisons.Remove(key);
        Clouds.RemoveAll(c => c.Board.Pointer == board.Pointer);
        LastTicks.Remove(board.Pointer);
    }
    internal static void Clear() { Shots.Clear(); Poisons.Clear(); Clouds.Clear(); LastTicks.Clear(); }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class PoisonFireScope
{
    internal static void Prefix(Plant __instance, out Plant? __state)
    { __state = PoisonShooters.FiringPlant; PoisonShooters.FiringPlant = FusionPlant.IsPoisonShooter(FusionPlant.From(__instance)) ? __instance : null; }
    internal static void Finalizer(Plant? __state) => PoisonShooters.FiringPlant = __state;
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ProjectileInitialize))]
internal static class PoisonProjectileInitializedPatch
{
    internal static void Postfix(Projectile __instance) => PoisonShooters.Track(__instance);
}

// Reassert the exact family after initializer/controller callbacks complete,
// matching the existing Hypno factory boundary. Tracking is idempotent.
[HarmonyPatch]
internal static class PoisonProjectileCreatedPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => typeof(Board).GetMethods()
        .Where(method => method.Name == nameof(Board.AddProjectile));
    internal static void Postfix(Projectile? __result)
    { if (__result != null) PoisonShooters.Track(__result); }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class PoisonImpactScope
{
    internal static void Prefix(Projectile __instance, Zombie theZombie, out PoisonShooters.Impact? __state)
    { __state = PoisonShooters.CurrentImpact; PoisonShooters.CurrentImpact = PoisonShooters.BeginImpact(__instance, theZombie); }
    internal static void Postfix() => PoisonShooters.FinishImpact(PoisonShooters.CurrentImpact);
    internal static void Finalizer(PoisonShooters.Impact? __state) => PoisonShooters.CurrentImpact = __state;
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class PoisonDamagePatch
{
    [HarmonyPriority(Priority.First)]
    internal static void Prefix(Zombie __instance, ref int theDamage)
    {
        var impact = PoisonShooters.CurrentImpact;
        if (impact == null || impact.Hit || impact.Zombie.Pointer != __instance.Pointer
            || impact.Id != __instance.DataID || impact.Board.Pointer != __instance.mBoard?.Pointer) return;
        impact.Hit = true;
        theDamage = 30;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class PoisonTorchwoodPatch
{
    // Ordinary source TorchWood accepts bullet 0/15, never poison bullet 177.
    internal static bool Prefix(Projectile __instance) => !PoisonShooters.Contains(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class PoisonTickPatch
{ internal static void Postfix(Board __instance) => PoisonShooters.Update(__instance); }

[HarmonyPatch]
internal static class PoisonProjectileCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.Die)), AccessTools.Method(typeof(Projectile), nameof(Projectile.Reset)) };
    internal static void Prefix(Projectile __instance) => PoisonShooters.Forget(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.Reset))]
internal static class PoisonZombieCleanupPatch
{ internal static void Prefix(Zombie __instance) => PoisonShooters.Forget(__instance); }

[HarmonyPatch]
internal static class PoisonZombieDeathCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.DieWithLoot)), AccessTools.Method(typeof(Zombie), nameof(Zombie.DieNoLoot)) };
    internal static void Postfix(Zombie __instance) { if (__instance.mDead) PoisonShooters.Forget(__instance); }
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class PoisonBoardCleanupPatch
{ internal static void Prefix(Board __instance) => PoisonShooters.ForgetBoard(__instance); }
