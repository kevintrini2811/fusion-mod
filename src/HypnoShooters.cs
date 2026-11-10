using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Bullet_hypnoPea (153) summons a friendly PeaShooterZ on a lethal hit.
// Native shooters, PeaHead AI and collision own scheduling, armor and movement.
internal static class HypnoShooters
{
    // Both fields are saved/reset natively. Peas do not use cob coordinates;
    // PeaHead never uses the dancer/imp summoning counter during its lifetime.
    private const int PlantShot = -430030, AllyShot = -430020;
    internal const int Summoned = -430153, SummonedFire = -430154;
    private const int FireShot = -430060;
    [ThreadStatic] internal static Plant? FiringPlant;
    [ThreadStatic] internal static Zombie? FiringZombie;
    [ThreadStatic] internal static Impact? CurrentImpact;
    internal sealed class Impact
    {
        internal Zombie Zombie = null!;
        internal Board Board = null!;
        internal uint Id;
        internal int Row, Damage;
        internal float X;
        internal bool Fire;
        internal bool Hit, Finished;
    }

    internal static bool Contains(Projectile shot) => shot.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea or ProjectileType.Fireball
        && shot.mCobTargetX is PlantShot or AllyShot or FireShot;
    internal static bool IsSummoned(Zombie zombie) => zombie.mMindControlled
        && (zombie.mZombieType == ZombieType.PeaHead && zombie.mSummonCounter == Summoned
            || zombie.mZombieType == ZombieType.GatlingHead && zombie.mSummonCounter == SummonedFire);
    internal static bool IsFire(Projectile shot) => shot.mCobTargetX == FireShot;
    internal static bool FromAlly(Projectile shot) => shot.mCobTargetX == AllyShot || IsFire(shot) && shot.mCobTargetRow == 40;
    internal static void Ignite(Projectile shot, int col)
    {
        ImportedPeaVisuals.ForgetProjectile(shot);
        shot.mCobTargetX = FireShot; shot.mCobTargetRow = 60;
        // Source fire-hypno hits one target; native Fireball would add unintended splash.
        shot.mHitTorchwoodGridX = col;
        ImportedPeaVisuals.EnsureHypnoProjectile(shot);
    }
    internal static Zombie? SummonFire(Board board, int row, float x)
    {
        var ally = board.AddZombieInRow(ZombieType.GatlingHead, row, -1, false);
        if (ally == null || ally.mDead) return null;
        ally.mPosX = x; ally.mX = (int)x;
        ally.mSummonCounter = SummonedFire; ally.StartMindControlled();
        return ally;
    }

    internal static void Track(Projectile shot)
    {
        if (shot.mDead || shot.mBoard == null || shot.mProjectileType is not (ProjectileType.Pea or ProjectileType.PeashooterPea)) return;
        if (FiringPlant is { } plant && plant.mBoard?.Pointer == shot.mBoard.Pointer
            && !plant.mDead && FusionPlant.IsHypnoShooter(FusionPlant.From(plant))) shot.mCobTargetX = PlantShot;
        else if (FiringZombie is { } zombie && zombie.mBoard?.Pointer == shot.mBoard.Pointer
            && !zombie.mDead && IsSummoned(zombie))
        {
            shot.mCobTargetX = zombie.mSummonCounter == SummonedFire ? FireShot : AllyShot;
            if (IsFire(shot)) shot.mCobTargetRow = 40;
        }
        else return;
        ImportedPeaVisuals.EnsureHypnoProjectile(shot);
    }

    internal static Impact? BeginImpact(Projectile shot, Zombie? zombie)
    {
        var board = shot.mBoard;
        if (!Contains(shot) || shot.mDead || board == null || zombie == null || zombie.mDead
            || zombie.IsDeadOrDying() || zombie.mMindControlled || zombie.mBoard?.Pointer != board.Pointer) return null;
        return new() { Zombie = zombie, Board = board, Id = zombie.DataID, Row = zombie.mRow,
            X = zombie.mPosX, Fire = IsFire(shot), Damage = IsFire(shot) ? shot.mCobTargetRow : shot.mCobTargetX == PlantShot ? 30 : 20 };
    }

    internal static void AdjustDamage(Zombie zombie, ref int damage)
    {
        var impact = CurrentImpact;
        if (impact == null || impact.Hit || impact.Zombie.Pointer != zombie.Pointer
            || impact.Id != zombie.DataID || impact.Board.Pointer != zombie.mBoard?.Pointer) return;
        impact.Hit = true;
        damage = impact.Damage;
    }

    internal static void FinishImpact(Impact? impact)
    {
        if (impact == null || !impact.Hit || impact.Finished) return;
        impact.Finished = true;
        var victim = impact.Zombie;
        var board = impact.Board;
        if (victim.DataID != impact.Id || victim.mBoard?.Pointer != board.Pointer || victim.mMindControlled
            || !(victim.mDead || victim.IsDeadOrDying()) || board.Pointer == CherryBombPlants.DisposingBoard
            || victim.mZombieType is ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target) return;
        var ally = impact.Fire ? SummonFire(board, impact.Row, impact.X)
            : ZombieLibrary.Zombies.Spawn(FusionZombies.Hypno, board, impact.Row);
        if (ally == null || ally.mDead) return;
        ally.mPosX = impact.X;
        ally.mX = (int)impact.X;
        // Native PeaHead then fires every 150 simulation ticks, including after
        // loading. Its shots retain the same lethal-hit summoning behavior.
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class HypnoFireScope
{
    internal static void Prefix(Plant __instance, out (Plant? Plant, Zombie? Zombie) __state)
    {
        __state = (HypnoShooters.FiringPlant, HypnoShooters.FiringZombie);
        HypnoShooters.FiringZombie = null;
        HypnoShooters.FiringPlant = FusionPlant.IsHypnoShooter(FusionPlant.From(__instance)) ? __instance : null;
    }
    internal static void Finalizer((Plant? Plant, Zombie? Zombie) __state)
    { HypnoShooters.FiringPlant = __state.Plant; HypnoShooters.FiringZombie = __state.Zombie; }
}

[HarmonyPatch]
internal static class HypnoAllyFireScope
{
    // UpdateActions contains an inlined copy of UpdateZombiePeaHead.
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateZombiePeaHead)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.UpdateActions)) };
    internal static void Prefix(Zombie __instance, out (Plant? Plant, Zombie? Zombie) __state)
    {
        __state = (HypnoShooters.FiringPlant, HypnoShooters.FiringZombie);
        HypnoShooters.FiringPlant = null;
        HypnoShooters.FiringZombie = HypnoShooters.IsSummoned(__instance) ? __instance : null;
    }
    internal static void Finalizer((Plant? Plant, Zombie? Zombie) __state)
    { HypnoShooters.FiringPlant = __state.Plant; HypnoShooters.FiringZombie = __state.Zombie; }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ProjectileInitialize))]
internal static class HypnoProjectileInitializePatch
{
    internal static void Postfix(Projectile __instance) => HypnoShooters.Track(__instance);
}

[HarmonyPatch]
internal static class HypnoProjectileCreatedPatch
{
    private static IEnumerable<MethodBase> TargetMethods() =>
        typeof(Board).GetMethods().Where(method => method.Name == nameof(Board.AddProjectile));
    internal static void Postfix(Projectile __result)
    {
        if (__result != null) HypnoShooters.Track(__result);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class HypnoImpactScope
{
    internal static void Prefix(Projectile __instance, Zombie theZombie, out HypnoShooters.Impact? __state)
    {
        __state = HypnoShooters.CurrentImpact;
        HypnoShooters.CurrentImpact = HypnoShooters.BeginImpact(__instance, theZombie);
    }
    internal static void Postfix() => HypnoShooters.FinishImpact(HypnoShooters.CurrentImpact);
    internal static void Finalizer(HypnoShooters.Impact? __state) => HypnoShooters.CurrentImpact = __state;
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class HypnoDamagePatch
{
    [HarmonyPriority(Priority.First)]
    internal static void Prefix(Zombie __instance, ref int theDamage) => HypnoShooters.AdjustDamage(__instance, ref theDamage);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class HypnoTorchwoodPatch
{
    // Ordinary source TorchWood accepts types 0/15 only, never bullet 153.
    internal static bool Prefix(Projectile __instance) => !HypnoShooters.Contains(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class HypnoProjectileVisualPatch
{
    internal static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && HypnoShooters.Contains(__instance)) ImportedPeaVisuals.EnsureHypnoProjectile(__instance);
    }
}

[HarmonyPatch]
internal static class HypnoProjectileCleanupPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.Die)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.Reset)) };
    internal static void Prefix(Projectile __instance)
    {
        if (HypnoShooters.Contains(__instance)) ImportedPeaVisuals.ForgetProjectile(__instance);
    }
}
