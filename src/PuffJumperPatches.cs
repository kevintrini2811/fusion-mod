using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Shared carrier integration and source ground-status eligibility. Each source
// plant owns its action machine, saved fields, movement and animation phase.
internal static class PuffJumpers
{
    internal static bool Retained(Plant plant) => NibbleShroom.Retained(plant) || Squish.Retained(plant);
    internal static bool DamageImmune(Plant plant) => NibbleShroom.DamageImmune(plant)
        || Squish.Airborne(plant) && ScaredContent.Playing(plant.mBoard) && !plant.mDead && !plant.mSquished;
    internal static bool PhysicalRetained(Plant plant) => !plant.mIsOnBoard
        && (NibbleShroom.Activated(plant) && NibbleShroom.Live(plant)
            || Squish.Retained(plant) && Squish.Live(plant) && !Squish.Airborne(plant));
    internal static bool GroundTarget(Board board, Zombie zombie) =>
        !(zombie.mZombiePhase is ZombiePhase.PolevaulterInVault or ZombiePhase.DolphinInJump
                or ZombiePhase.ImpGettingThrown or ZombiePhase.BalloonFlying or ZombiePhase.BalloonPopping
                or ZombiePhase.DiggerTunneling or ZombiePhase.DiggerTunnelingPauseWithoutAxe
                or ZombiePhase.DiggerRising or ZombiePhase.DiggerRiseWithoutAxe
                or ZombiePhase.PogoHighBounce1 or ZombiePhase.PogoHighBounce2 or ZombiePhase.PogoHighBounce3
                or ZombiePhase.PogoHighBounce4 or ZombiePhase.PogoHighBounce5 or ZombiePhase.PogoHighBounce6
                or ZombiePhase.PogoForwardBounce2 or ZombiePhase.PogoForwardBounce7
                or ZombiePhase.BungeeDiving or ZombiePhase.BungeeDivingScreaming or ZombiePhase.BungeeRising)
        && (SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)
        // Source Polo_jump is land status, unlike Polo_bigJump. Native range
        // flags all reject any positive Pogo height before testing those flags.
        // Admit only the ordinary bounce, retaining actual lifetime/hostility.
        || zombie.mZombieType == ZombieType.Pogo && zombie.mZombiePhase == ZombiePhase.PogoBouncing
            && zombie.mBoard?.Pointer == board.Pointer && !zombie.mDead && !zombie.IsDeadOrDying()
            && !zombie.mMindControlled);

}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PuffJumperInitializePatch
{
    internal static void Postfix(Plant __instance)
    { NibbleShroom.Initialize(__instance); Squish.Initialize(__instance); }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class PuffJumperAbilityPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (NibbleShroom.Owns(__instance)) { NibbleShroom.Tick(__instance); return false; }
        if (Squish.Owns(__instance)) { Squish.Tick(__instance); return false; }
        return true;
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.CanPlantAt))]
internal static class PuffJumperPlacementScope
{
    internal static void Prefix(Board __instance, int theGridX, int theGridY, out PlantQueryHide __state)
    {
        var hidden = new List<Plant>();
        for (int i = 0; i < __instance.m_plants.Count; ++i)
        {
            var plant = __instance.m_plants[i];
            if (plant != null && plant.mBoard?.Pointer == __instance.Pointer
                && plant.mPlantCol == theGridX && plant.mRow == theGridY && !plant.mIsOnBoard
                && PuffJumpers.Retained(plant)) hidden.Add(plant);
        }
        __state = new(__instance, hidden);
    }
    internal static void Finalizer(PlantQueryHide? __state) => __state?.Dispose();
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class PuffJumperCrushPatch
{
    internal static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal) return false;
        if (NibbleShroom.Owns(__instance) && NibbleShroom.Live(__instance)) return false;
        return !Squish.Owns(__instance) || !Squish.Crush(__instance);
    }
}

// FindCollisionTargetPlant does not consult NotOnGround before comparing each
// candidate rectangle. Represent only Squish's disabled collider as an empty
// bound during this exact query so the native loop can select another plant.
[HarmonyPatch(typeof(Projectile), nameof(Projectile.FindCollisionTargetPlant))]
internal static class PuffJumperProjectileCandidateScope
{
    internal sealed class Query
    {
        internal readonly Projectile Shot;
        internal readonly uint Id;
        internal readonly Board Board;
        internal readonly ProjectileType Type;
        internal Query(Projectile shot)
        { Shot = shot; Id = shot.DataID; Board = shot.mBoard; Type = shot.mProjectileType; }
        internal Plant? Candidate;
        internal uint CandidateId;
        internal FusionPlant CandidateIdentity;
        internal int TopDepth;
        internal bool Valid => Shot.DataID == Id && Shot.mBoard?.Pointer == Board.Pointer
            && !Shot.mDead && Shot.mProjectileType == Type && ScaredContent.Playing(Board);
        internal bool CandidateValid => Valid && Candidate is { } plant && plant.DataID == CandidateId
            && plant.mBoard?.Pointer == Board.Pointer && FusionPlant.From(plant) == CandidateIdentity
            && PuffJumpers.PhysicalRetained(plant);
    }
    [ThreadStatic] internal static Query? Current;
    internal static void Prefix(Projectile __instance, out Query? __state)
    { __state = Current; Current = new Query(__instance); }
    internal static void Finalizer(Query? __state) => Current = __state;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.GetPlantRect))]
internal static class PuffJumperProjectileCandidateBounds
{
    internal static void Postfix(Plant __instance, bool __runOriginal, ref UnityEngine.Rect __result)
    {
        if (PuffJumperProjectileCandidateScope.Current is not { } query) return;
        // Every native candidate supersedes the preceding candidate, including
        // foreign plants and disabled colliders. Never retain a prior overlap.
        query.Candidate = null;
        if (!__runOriginal || !query.Valid || query.TopDepth != 0
            || query.Board.Pointer != __instance.mBoard?.Pointer) return;
        if (Squish.Airborne(__instance))
            __result = new UnityEngine.Rect(-100000, -100000, 0, 0);
        else if (PuffJumpers.PhysicalRetained(__instance))
        { query.Candidate = __instance; query.CandidateId = __instance.DataID; query.CandidateIdentity = FusionPlant.From(__instance); }
    }
}

// Native projectile overlap is tested at the physical rectangle, but the final
// GetTopPlantAt resolves the immutable home cell. Rebind only that exact native
// claim; otherwise a replacement planted at home would receive the moving hit.
[HarmonyPatch(typeof(Board), nameof(Board.GetTopPlantAt))]
internal static class PuffJumperProjectileCandidateRemap
{
    internal sealed record Claim(PuffJumperProjectileCandidateScope.Query Query, Plant? Plant, uint Id, FusionPlant Identity);
    internal static void Prefix(Board __instance, int theGridX, int theGridY, PlantPriority thePriority,
        bool __runOriginal, out Claim? __state)
    {
        __state = null;
        if (PuffJumperProjectileCandidateScope.Current is not { } query) return;
        var candidate = query.Candidate;
        bool match = __runOriginal && query.TopDepth == 0 && query.Type is ProjectileType.Basketball or ProjectileType.ZombiePea && query.CandidateValid
            && __instance.Pointer == query.Board.Pointer && candidate!.mPlantCol == theGridX
            && candidate.mRow == theGridY && (int)thePriority == (query.Shot.mProjectileType == ProjectileType.Basketball ? 3 : 0);
        query.Candidate = null; // One use, also across foreign/nested claims.
        ++query.TopDepth;
        __state = new(query, match ? candidate : null, match ? query.CandidateId : 0, query.CandidateIdentity);
    }
    internal static void Postfix(bool __runOriginal, Claim? __state, ref Plant? __result)
    {
        if (!__runOriginal || __state is not { Plant: { } plant } claim) return;
        var query = claim.Query;
        if (ReferenceEquals(PuffJumperProjectileCandidateScope.Current, query) && query.TopDepth == 1
            && query.Valid && plant.DataID == claim.Id && plant.mBoard?.Pointer == query.Board.Pointer
            && FusionPlant.From(plant) == claim.Identity && PuffJumpers.PhysicalRetained(plant))
            __result = plant;
    }
    internal static void Finalizer(Claim? __state)
    { if (__state != null) --__state.Query.TopDepth; }
}
