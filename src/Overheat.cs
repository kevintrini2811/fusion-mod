using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Fusion's permanent JalaEffect: +50% incoming damage, cleared by cold/freeze,
// and a one-cell, 100-damage death explosion that propagates the effect.
internal static class Overheat
{
    internal sealed record Death(Board Board, uint Id, int Row, float X, float Y, int Order);

    internal static bool Has(Zombie zombie) => SavedOverheat.Find(zombie) != null;

    internal static void Apply(Zombie zombie)
    {
        if (zombie.mBoard == null || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
            || zombie.mZombieType is ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target) return;
        if (SavedOverheat.Add(zombie)) zombie.RemoveColdEffects();
    }

    internal static int Damage(Zombie zombie, int damage) => damage <= 0 || !Has(zombie) ? damage
        : (int)Math.Min(int.MaxValue, damage + (long)damage / 2);

    internal static Death? Capture(Zombie zombie)
    {
        var board = zombie.mBoard;
        if (!Has(zombie) || zombie.mDead || board == null || board.mPaused
            || board.Pointer == CherryBombPlants.DisposingBoard
            || board.mApp == null || board.mApp.GameScene != GameScenes.Playing
            || board.mApp.GameMode == GameMode.ChallengeZenGarden) return null;
        var rect = zombie.GetZombieRect();
        return new(board, zombie.DataID, zombie.mRow,
            (rect.xMin + rect.xMax) / 2, (rect.yMin + rect.yMax) / 2, zombie.mRenderOrder);
    }

    internal static Death? BeginDeath(Zombie zombie)
    {
        var death = Capture(zombie);
        Forget(zombie); // DieWithLoot calls DieNoLoot; consume before nesting.
        return death;
    }

    internal static void FinishDeath(Zombie zombie, Death? death)
    {
        if (death == null || !zombie.mDead || zombie.DataID != death.Id
            || zombie.mBoard?.Pointer != death.Board.Pointer) return;
        PumpkinSupport.HeatedDeath(death.Board, death.Row);
        Explode(death);
    }

    internal static void Explode(Death death)
    {
        var range = DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
            | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Underground;
        var targets = new List<(Zombie Zombie, uint Id)>();
        var zombies = death.Board.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || zombie.DataID == death.Id || zombie.mBoard?.Pointer != death.Board.Pointer
                || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
                || Math.Abs(zombie.mRow - death.Row) > 1 || !zombie.EffectedByDamage(range)) continue;
            var rect = zombie.GetZombieRect();
            float dx = death.X - Math.Clamp(death.X, rect.xMin, rect.xMax);
            float dy = death.Y - Math.Clamp(death.Y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= 80 * 80) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || zombie.mBoard?.Pointer != death.Board.Pointer
                || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled) continue;
            // Source applies damage first, then heats survivors. Already heated
            // victims receive 150 and may trigger their own death explosion.
            zombie.TakeDamage(100, DamageFlags.HitsShieldAndBody);
            if (zombie.DataID == id && zombie.mBoard?.Pointer == death.Board.Pointer) Apply(zombie);
        }
        death.Board.mApp.AddTodParticle(death.X, death.Y, death.Order, ParticleEffect.Powie);
    }

    internal static void Forget(Zombie zombie) => SavedOverheat.Remove(zombie);
    internal static void RestoreFailedDeath(Zombie zombie, Death? death)
    {
        if (death != null && !zombie.mDead && zombie.DataID == death.Id
            && zombie.mBoard?.Pointer == death.Board.Pointer)
            SavedOverheat.Add(zombie);
    }
    // Native board disposal owns the serialized items. No transient status map.
    internal static void ForgetBoard(Board board) { }
    internal static void Clear() { }

}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class OverheatDamagePatch
{
    internal static void Prefix(Zombie __instance, ref int theDamage) => theDamage = Overheat.Damage(__instance, theDamage);
}

[HarmonyPatch]
internal static class OverheatColdPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Zombie), nameof(Zombie.ApplyChill));
        yield return AccessTools.Method(typeof(Zombie), nameof(Zombie.HitIceTrap));
    }
    internal static void Postfix(Zombie __instance)
    {
        if (__instance.mChilledCounter > 0 || __instance.mIceTrapCounter > 0) Overheat.Forget(__instance);
    }
}

[HarmonyPatch]
internal static class OverheatDeathPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Zombie), nameof(Zombie.DieWithLoot));
        yield return AccessTools.Method(typeof(Zombie), nameof(Zombie.DieNoLoot));
    }
    internal static void Prefix(Zombie __instance, out Overheat.Death? __state) => __state = Overheat.BeginDeath(__instance);
    internal static void Postfix(Zombie __instance, Overheat.Death? __state)
    {
        Overheat.FinishDeath(__instance, __state);
    }
    internal static void Finalizer(Zombie __instance, Overheat.Death? __state) =>
        Overheat.RestoreFailedDeath(__instance, __state);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.Reset))]
internal static class OverheatResetPatch
{
    private static void Prefix(Zombie __instance) => Overheat.Forget(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class OverheatBoardCleanupPatch
{
    private static void Prefix(Board __instance) => Overheat.ForgetBoard(__instance);
}

// ApplyBurn and native health writers can contain inlined removal. A nested
// ordinary death consumes its item first, so this audit cannot double-credit.
[HarmonyPatch]
internal static class OverheatInlinedDeathPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.TakeDamage)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.ApplyBurn)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.Update)) };
    internal static void Prefix(Zombie __instance, out Overheat.Death? __state) => __state = Overheat.Capture(__instance);
    internal static void Postfix(Zombie __instance, Overheat.Death? __state)
    {
        if (__state == null || !__instance.mDead || !Overheat.Has(__instance)) return;
        Overheat.Forget(__instance);
        Overheat.FinishDeath(__instance, __state);
    }
}

