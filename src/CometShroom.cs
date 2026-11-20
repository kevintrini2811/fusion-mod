using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

internal static class CometShroom
{
    private const int Marker = -451326, HitLimit = 10, Damage = 200;
    private const float Step = 4 * ScaredyFusions.SourceUnit / 100;
    private sealed record Flight(IntPtr Board, uint Id, Queue<uint> Targets)
    {
        internal int Remaining = HitLimit;
    }
    private static readonly Dictionary<IntPtr, Flight> Flights = new();

    internal static bool IsShot(Projectile shot) => shot.mProjectileType == ProjectileType.Pea
        && shot.mMotionType == ProjectileMotion.Star && shot.mCobTargetX == Marker;

    private static bool Eligible(Board board, Zombie? zombie) => zombie != null
        && zombie.mBoard?.Pointer == board.Pointer && !zombie.mDead && !zombie.IsDeadOrDying()
        && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target);

    internal static void Launch(CherryBombPlants.Death death)
    {
        var board = death.Board;
        var friends = new Queue<uint>();
        var enemies = new List<uint>();
        // JumpSun.Start filters and stably sorts the current board list on
        // allegiance. Later spawns do not join this flight.
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (!Eligible(board, zombie)) continue;
            if (zombie!.mMindControlled) friends.Enqueue(zombie.DataID);
            else enemies.Add(zombie.DataID);
        }
        foreach (uint id in enemies) friends.Enqueue(id);
        if (friends.Count > 0)
        {
            var shot = board.AddProjectile(death.X, death.Y, death.Order + 1, death.Row, ProjectileType.Pea);
            if (shot != null && !shot.mDead)
            {
                shot.mMotionType = ProjectileMotion.Star;
                shot.mCobTargetX = Marker;
                Flights[shot.Pointer] = new(board.Pointer, shot.DataID, friends);
                ImportedPeaVisuals.EnsureBloverSunProjectile(shot);
                return;
            }
        }
        Drop(board, death.X, death.Y, false);
    }

    private static void Drop(Board board, float x, float y, bool full) =>
        board.AddCoin(x, y, full ? CoinType.Sun : CoinType.SmallSun, CoinMotion.FromPlant);

    internal static void Tick(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board == null || board.mPaused || !ScaredContent.Playing(board)
            || !FusionTime.SourceStep(shot, typeof(CometShroom))) return;
        if (!Flights.TryGetValue(shot.Pointer, out var flight)
            || flight.Board != board.Pointer || flight.Id != shot.DataID)
        {
            shot.Die();
            return;
        }

        while (flight.Targets.TryPeek(out uint id))
        {
            var zombie = board.ZombieTryToGet((ZombieID)id);
            if (zombie == null || zombie.DataID != id || !Eligible(board, zombie))
            {
                flight.Targets.Dequeue();
                continue;
            }
            var rect = zombie.GetZombieRect();
            float dx = rect.center.x - shot.mPosX, dy = rect.center.y - .5f * ScaredyFusions.SourceUnit - shot.mPosY;
            float distance = MathF.Sqrt(dx * dx + dy * dy);
            float fraction = distance > Step ? Step / distance : 1;
            shot.mX = shot.mPosX += dx * fraction;
            shot.mY = shot.mPosY += dy * fraction;
            shot.mRow = zombie.mRow;
            shot.mShadowY = zombie.mPosY;
            shot.mRenderOrder = zombie.mRenderOrder + 1;
            if (distance > Step + .1f * ScaredyFusions.SourceUnit) return;

            // Consume the snapshot entry before damage callbacks can reenter.
            flight.Targets.Dequeue();
            bool friendly = zombie.mMindControlled;
            float x = shot.mPosX, y = shot.mPosY;
            int order = shot.mRenderOrder;
            bool last = --flight.Remaining == 0 || flight.Targets.Count == 0;
            if (last) shot.Die();
            zombie.TakeDamage(friendly ? Damage : Damage / 2, DamageFlags.BypassesShield);
            if (!ScaredContent.Playing(board)) return;
            Drop(board, x, y, friendly);
            board.mApp.AddTodParticle(x, y, order, ParticleEffect.StarSplat);
            board.mApp.SoundSystem.PlayFoley(FoleyType.Chime);
            PlantLibrary.Projectiles.PlaySound("fusion:Splat", board);
            if (last) Drop(board, x, y, false);
            return;
        }

        float endX = shot.mPosX, endY = shot.mPosY;
        shot.Die();
        Drop(board, endX, endY, false);
    }

    internal static void Forget(Projectile shot)
    {
        if (Flights.TryGetValue(shot.Pointer, out var flight) && flight.Id == shot.DataID)
            Flights.Remove(shot.Pointer);
    }

    internal static void ForgetBoard(Board board)
    {
        foreach (var pointer in Flights.Where(pair => pair.Value.Board == board.Pointer).Select(pair => pair.Key).ToArray())
            Flights.Remove(pointer);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class CometDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state)
    {
        __state = __runOriginal && FusionPlant.From(__instance) == FusionPlant.CometShroom
            && CherryBombPlants.CanTriggerDeath(__instance)
            ? new(__instance.mBoard, FusionPlant.CometShroom, __instance.DataID, __instance.mPlantCol,
                __instance.mRow, __instance.mX + __instance.mWidth / 2f,
                __instance.mY + __instance.mHeight / 2f, __instance.mRenderOrder)
            : null;
    }

    private static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id
            && __instance.mBoard?.Pointer == __state.Board.Pointer && ScaredContent.Playing(__state.Board))
            CometShroom.Launch(__state);
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class CometBitePatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Zombie __instance, Plant thePlant, bool __runOriginal)
    {
        if (!__runOriginal || FusionPlant.From(thePlant) != FusionPlant.CometShroom
            || !CherryBombPlants.CanTriggerDeath(thePlant) || __instance.mBoard?.Pointer != thePlant.mBoard.Pointer
            || __instance.mDead || __instance.IsDeadOrDying() || __instance.mMindControlled) return true;
        uint id = thePlant.DataID;
        HypnoPlants.MindControl(__instance);
        if (thePlant.DataID == id && !thePlant.mDead) thePlant.Die();
        return false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class CometFlightPatch
{
    private static bool Prefix(Projectile __instance, bool __runOriginal)
    {
        if (!__runOriginal || !CometShroom.IsShot(__instance)) return true;
        CometShroom.Tick(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class CometImpactPatch
{
    private static bool Prefix(Projectile __instance) => !CometShroom.IsShot(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class CometTorchwoodPatch
{
    private static bool Prefix(Projectile __instance) => !CometShroom.IsShot(__instance);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Die))]
internal static class CometProjectileDeathPatch
{
    private static void Prefix(Projectile __instance) => CometShroom.Forget(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class CometDisposePatch
{
    private static void Prefix(Board __instance) => CometShroom.ForgetBoard(__instance);
}
