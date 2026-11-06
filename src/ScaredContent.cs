using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

internal static class ScaredContent
{
    // These grave coordinates are unused by walking Normal zombies and are
    // serialized by native Zombie.Serialize/Deserialize. Never mark grave rises.
    internal const int ZombieMarker = -521012;
    [ThreadStatic] internal static bool SuppressReplacement;
    private const int Awake = 1, DropAttempted = 2, Instant = 4, OrdinaryDeath = 8;
    private const int Death = 16, Eating = 32, Bitten = 64, Rage = 128;
    internal static bool Eyes(Zombie zombie) => zombie.mZombieType is (ZombieType.Normal or ZombieType.TrafficCone or ZombieType.Pail) && zombie.mGraveX == ZombieMarker;
    internal static bool Awakened(Zombie zombie) => Eyes(zombie) && (zombie.mGraveY & Awake) != 0;
    internal static bool Raging(Zombie zombie) => (zombie.mGraveY & Rage) != 0;
    internal const int EatAnimationRate = 6;
    internal const float WalkSpeedMultiplier = 2f, WalkAnimationRate = 3f;
    private static bool BiteTick(int tick) => tick * EatAnimationRate >= 75
        && (tick * EatAnimationRate - 75) % 125 < EatAnimationRate;
    internal static float AnimationTime(Zombie zombie) => (zombie.mGraveY >> 8) * .01f;
    internal static bool Playing(Board? board) => board != null && board.mApp != null
        && board.mApp.GameScene == GameScenes.Playing && board.mApp.GameMode != GameMode.ChallengeZenGarden
        && board.Pointer != CherryBombPlants.DisposingBoard;

    internal static void Spawn(Zombie zombie)
    {
        var board = zombie.mBoard;
        if (SuppressReplacement || ZombieLibrary.Zombies.IsExplicitSpawn || !Playing(board) || zombie.mDead || zombie.mZombieType is not (ZombieType.Normal or ZombieType.TrafficCone or ZombieType.Pail) || zombie.mMindControlled
            || zombie.mGraveX != 0
            || zombie.mShieldHealth > 0
            || zombie.mZombiePhase != ZombiePhase.ZombieNormal || board!.Rand(100) >= OptionalDifficulty.EyesChance(board)) return;
        ZombieLibrary.Zombies.Apply(zombie.mZombieType == ZombieType.TrafficCone ? FusionZombies.EyesCone
            : zombie.mZombieType == ZombieType.Pail ? FusionZombies.EyesBucket : FusionZombies.Eyes, zombie);
    }

    internal static void ZombieTick(Zombie zombie)
    {
        var board = zombie.mBoard;
        if (!Eyes(zombie) || !Playing(board) || board!.mPaused) return;
        if (zombie.mDead)
        {
            if (zombie.mZombiePhase == ZombiePhase.ZombieDying) Drop(zombie);
            return;
        }
        ImportedPeaVisuals.EnsureEyesZombie(zombie);
        if (zombie.mZombiePhase == ZombiePhase.RisingFromGrave) return;
        if (zombie.mButteredCounter > 0 || zombie.mIceTrapCounter > 0) return;
        if (zombie.mChilledCounter > 0 && (zombie.mZombieAge & 1) != 0) return;
        if (zombie.mZombiePhase == ZombiePhase.ZombieDying && (zombie.mGraveY & Death) == 0)
            zombie.mGraveY = (zombie.mGraveY & 255 & ~(Rage | Eating)) | Death;
        if (Raging(zombie) && (zombie.mGraveY >> 8) >= 108)
        {
            zombie.mGraveY = zombie.mGraveY & 255 & ~Rage;
            // User excludes original audio. Native rage cue follows game volume/mute.
            ZombieLibrary.Zombies.PlaySound("fusion:eyes-rage", zombie);
        }
        if (!Raging(zombie) && zombie.mZombiePhase != ZombiePhase.ZombieDying
            && zombie.mIsEating != ((zombie.mGraveY & Eating) != 0))
            zombie.mGraveY = (zombie.mGraveY & 255 & ~(Eating | Bitten)) | (zombie.mIsEating ? Eating : 0);
        if (zombie.mGraveY <= int.MaxValue - 256) zombie.mGraveY += 256;
        if (!BiteTick(zombie.mGraveY >> 8)) zombie.mGraveY &= ~Bitten;
        if (!Awakened(zombie) && !zombie.IsDeadOrDying())
        {
            bool near = false;
            float direction = zombie.mMindControlled ? 1 : -1;
            if (!zombie.mMindControlled)
                for (int i = 0; i < board.m_plants.Count && !near; ++i)
                {
                    var plant = board.m_plants[i];
                    if (plant == null || !ShroomSlots.Live(plant) || plant.mRow != zombie.mRow) continue;
                    float distance = direction * (plant.mX + 40 - zombie.mPosX);
                    near = distance >= 0 && distance <= 80;
                }
            for (int i = 0; i < board.m_zombies.Count && !near; ++i)
            {
                var target = board.m_zombies[i];
                if (target == null || target.mDead || target.IsDeadOrDying() || target.mRow != zombie.mRow
                    || target.mMindControlled == zombie.mMindControlled) continue;
                float distance = direction * (target.mPosX - zombie.mPosX);
                near = distance >= 0 && distance <= 80;
            }
            if (near) zombie.mGraveY = (zombie.mGraveY & 255) | Awake | Rage;
        }
    }

    internal static void Contact(Zombie zombie)
    {
        // Native EatPlant/EatZombie normally starts eating only after damage.
        // Our animation delay must start that state before waiting for its event.
        if (Awakened(zombie) && !Raging(zombie) && Playing(zombie.mBoard) && !zombie.mBoard.mPaused
            && !zombie.mDead && !zombie.IsDeadOrDying() && !zombie.mIsEating)
            zombie.StartEating();
    }
    internal static bool BiteReady(Zombie zombie)
    {
        if (!Awakened(zombie) || Raging(zombie) || !Playing(zombie.mBoard) || zombie.mBoard.mPaused
            || zombie.mDead || zombie.IsDeadOrDying() || zombie.mButteredCounter > 0 || zombie.mIceTrapCounter > 0
            || (zombie.mGraveY & (Eating | Bitten)) != Eating) return false;
        int tick = zombie.mGraveY >> 8;
        if (!BiteTick(tick)) return false;
        zombie.mGraveY |= Bitten;
        return true;
    }
    internal sealed record BiteTarget(Zombie Zombie, uint Id);
    [ThreadStatic] internal static BiteTarget? CurrentBite;

    internal static void InstantKill(Zombie zombie)
    {
        if (Eyes(zombie)) zombie.mGraveY = (zombie.mGraveY | Instant) & ~OrdinaryDeath;
    }
    internal sealed record Damage(Board Board, uint Id, bool Alive);
    internal static Damage? BeforeDamage(Zombie zombie, DamageFlags flags)
    {
        if (!Eyes(zombie) || !Playing(zombie.mBoard) || zombie.mBoard.mPaused || zombie.mDead) return null;
        if ((flags & DamageFlags.DoesntLeaveBody) != 0) InstantKill(zombie);
        return new(zombie.mBoard, zombie.DataID, !zombie.IsDeadOrDying());
    }
    internal static void AfterDamage(Zombie zombie, Damage? damage)
    {
        if (damage == null || !Eyes(zombie) || zombie.DataID != damage.Id
            || zombie.mBoard?.Pointer != damage.Board.Pointer || !damage.Alive) return;
        if (!zombie.mDead && !zombie.IsDeadOrDying() && zombie.mBodyHealth > 0)
        {
            zombie.mGraveY &= ~Instant;
            if (zombie.mHasHead && zombie.mBodyHealth <= 90) zombie.DropHead((DamageFlags)0);
        }
        if ((zombie.mGraveY & Instant) != 0) return;
        if (zombie.mDead || zombie.IsDeadOrDying() || zombie.mBodyHealth <= 0)
        {
            zombie.mGraveY |= OrdinaryDeath;
            if (zombie.mDead || zombie.mBodyHealth <= 0) Drop(zombie);
        }
    }
    internal static void Drop(Zombie zombie)
    {
        if (!Eyes(zombie) || (zombie.mGraveY & (DropAttempted | Instant | OrdinaryDeath)) != OrdinaryDeath
            || !Playing(zombie.mBoard) || zombie.mBoard.mPaused) return;
        zombie.mGraveY |= DropAttempted; // Reserve before RNG and presentation callbacks.
        if (zombie.mBoard.Rand(100) == 0)
            BucketDrops.Add(zombie.mBoard, zombie.mRow, zombie.mPosX + 40,
                zombie.mPosY + zombie.mHeight - 20, FusionPlant.EyesZombieHead);
    }

    internal static void Initialize(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.BloodSunflower) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchCounter = 2500;
        plant.mTargetX = plant.mTargetY = plant.mShootingCounter = 0;
    }
    internal static void PlantTick(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.BloodSunflower || !ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        var board = plant.mBoard;
        if (plant.mShootingCounter > 0) --plant.mShootingCounter;
        if (--plant.mLaunchCounter <= 0)
        {
            board.AddCoin(plant.mX, plant.mY, CoinType.Sun, CoinMotion.FromPlant);
            plant.mLaunchCounter = 2500;
            plant.mShootingCounter = 50;
        }
        // Source GetPlants(col + 1, row) infects the first plant to the right.
        // Full DataID prevents progress transferring to a replacement plant.
        var target = plant.mPlantCol < 8 ? board.GetPlantsOnLawn(plant.mPlantCol + 1, plant.mRow).NormalPlant : null;
        if (target == null || !ShroomSlots.Live(target) || FusionPlant.From(target) == FusionPlant.BloodSunflower)
        { plant.mTargetX = plant.mTargetY = 0; return; }
        int id = unchecked((int)target.DataID);
        if (plant.mTargetX != id) { plant.mTargetX = id; plant.mTargetY = 0; }
        if (++plant.mTargetY < 1000) return;
        if (FusionService.CheckFusionPlacement(board, target, FusionPlant.BloodSunflower) != PlantingReason.Ok) return;
        var created = board.AddPlant(target.mPlantCol, target.mRow, FusionPlant.BloodSunflower.NativeType, FusionPlant.BloodSunflower.Marker);
        if (created == null || created.mDead || !created.mIsOnBoard) return;
        PlayerPlantingScope.Consume(target);
        plant.mTargetX = plant.mTargetY = 0;
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.AddZombieInRow))]
internal static class ScaredZombieInitializePatch
{
    // This is the fresh row-spawn boundary, after complete native initialization.
    // Loading, almanac previews, grave rises and bungee cargo do not enter it.
    private static void Postfix(Zombie? __result)
    { if (__result != null) ScaredContent.Spawn(__result); }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class ScaredZombieDamagePatch
{
    [HarmonyPriority(Priority.First)]
    private static void Prefix(Zombie __instance, ref int theDamage, DamageFlags theDamageFlags, out ScaredContent.Damage? __state)
    {
        if (theDamage > 0 && ScaredContent.CurrentBite is { } bite && bite.Zombie.Pointer == __instance.Pointer
            && bite.Id == __instance.DataID) theDamage = 50;
        __state = ScaredContent.BeforeDamage(__instance, theDamageFlags);
    }
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(Zombie __instance, ScaredContent.Damage? __state) => ScaredContent.AfterDamage(__instance, __state);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class ScaredPlantBitePatch
{
    internal sealed class Hit
    {
        internal readonly uint ZombieId, PlantId;
        internal readonly int Age, Health;
        internal readonly Plant Plant;
        internal bool Applied;
        internal Hit(Zombie zombie, Plant plant)
        { ZombieId = zombie.DataID; Age = zombie.mZombieAge; Plant = plant; PlantId = plant.DataID; Health = plant.mPlantHealth; }
        internal void Apply()
        {
            if (Applied || Plant.DataID != PlantId || Plant.mDead) return;
            int damage = Health - Plant.mPlantHealth;
            if (damage <= 0) return; // Native target guards rejected this bite.
            Applied = true;
            if (damage < 50) Plant.mPlantHealth -= 50 - damage;
        }
    }
    [ThreadStatic] internal static Hit? Current;
    internal sealed record State(Hit? Previous, Hit Hit);
    private static bool Prefix(Zombie __instance, Plant thePlant, out State? __state)
    {
        __state = null;
        if (!ScaredContent.Eyes(__instance)) return true;
        ScaredContent.Contact(__instance);
        if (!ScaredContent.BiteReady(__instance)) return false;
        // Eyes owns the bite clock; apply Gnome's bite only at that same event.
        // Its prefix can otherwise lose ordering against this replacement AI.
        if (PuffSupport.BiteGnome(thePlant, __instance)) return false;
        var hit = new Hit(__instance, thePlant);
        __state = new(Current, hit);
        Current = hit;
        // Source animation owns the bite clock. Restore native age in Finalizer.
        __instance.mZombieAge &= ~1;
        return true;
    }
    [HarmonyPriority(Priority.First + 100)]
    private static void Postfix(State? __state)
    {
        if (__state is not { Hit: var hit } || hit.Applied) return;
        hit.Apply();
        if (!hit.Applied || hit.Plant.mPlantHealth > 0) return;
        // Die's existing defense scopes can rescue a lethal hit and undo this
        // counter. Their snapshots include the complete 50-point bite.
        ++hit.Plant.mBoard.mPlantsEaten;
        hit.Plant.Die();
    }
    private static void Finalizer(Zombie __instance, State? __state)
    {
        if (__state is not { } state) return;
        Current = state.Previous;
        if (__instance.DataID == state.Hit.ZombieId) __instance.mZombieAge = state.Hit.Age;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class ScaredBiteDeathPatch
{
    // Native four-point lethal hits enter Die before EatPlant's postfix.
    // Complete damage before PotDefense and ActiveNuts inspect health.
    [HarmonyPriority(Priority.First + 100)]
    private static void Prefix(Plant __instance)
    {
        if (ScaredPlantBitePatch.Current is { } hit && hit.Plant.Pointer == __instance.Pointer)
            hit.Apply();
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateZombieWalking))]
internal static class ScaredRageWalkingPatch
{
    internal sealed record Speed(uint Id, float Velocity, float Scale);
    internal static bool Prefix(Zombie __instance, out Speed? __state)
    {
        __state = null;
        if (!ScaredContent.Eyes(__instance)) return true;
        if (ScaredContent.Raging(__instance)) return false;
        if (!ScaredContent.Awakened(__instance) || !ScaredContent.Playing(__instance.mBoard)
            || __instance.mBoard.mPaused) return true;
        // Native walking either uses velocity or animation travel * scale.
        // Scale both inputs only inside this call; preserve native slowing,
        // direction and immobilization, without changing the saved body size.
        __state = new(__instance.DataID, __instance.mVelX, __instance.mScaleZombie);
        __instance.mVelX *= ScaredContent.WalkSpeedMultiplier;
        __instance.mScaleZombie *= ScaredContent.WalkSpeedMultiplier;
        return true;
    }
    internal static void Finalizer(Zombie __instance, Speed? __state)
    {
        if (__state == null || __instance.DataID != __state.Id) return;
        __instance.mVelX = __state.Velocity;
        __instance.mScaleZombie = __state.Scale;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatZombie))]
internal static class ScaredZombieBitePatch
{
    internal sealed record State(ScaredContent.BiteTarget? Previous, uint Id, int Age);
    private static bool Prefix(Zombie __instance, Zombie theZombie, out State? __state)
    {
        __state = null;
        if (!ScaredContent.Eyes(__instance)) return true;
        ScaredContent.Contact(__instance);
        if (!ScaredContent.BiteReady(__instance)) return false;
        __state = new(ScaredContent.CurrentBite, __instance.DataID, __instance.mZombieAge);
        ScaredContent.CurrentBite = new(theZombie, theZombie.DataID);
        __instance.mZombieAge &= ~1;
        return true;
    }
    private static void Finalizer(Zombie __instance, State? __state)
    {
        if (__state is not { } state) return;
        ScaredContent.CurrentBite = state.Previous;
        if (__instance.DataID == state.Id) __instance.mZombieAge = state.Age;
    }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.DieWithLoot))]
internal static class ScaredZombieDeathPatch
{
    // In this native build DieWithLoot is the direct chomper/kelp/mower/hammer
    // route. Ordinary health deaths use TakeBodyDamage and UpdateDeath instead.
    private static void Prefix(Zombie __instance) => ScaredContent.InstantKill(__instance);
}
[HarmonyPatch]
internal static class ScaredInstantKillPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.MowDown)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.ApplyBurn)) };
    private static void Prefix(Zombie __instance) => ScaredContent.InstantKill(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class ScaredPlantInitializePatch
{
    private static void Postfix(Plant __instance) => ScaredContent.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class ScaredPlantTickPatch
{
    private static void Postfix(Plant __instance) => ScaredContent.PlantTick(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class ScaredSunflowerProductionPatch
{
    private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.BloodSunflower;
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.CheckIfPreyCaught))]
internal static class ScaredPreyClockPatch
{
    internal sealed record State(uint Id, int Age);
    private static void Prefix(Zombie __instance, out State? __state)
    {
        __state = null;
        if (!ScaredContent.Eyes(__instance) || !ScaredContent.Playing(__instance.mBoard)
            || __instance.mBoard.mPaused) return;
        // Native prey checks run only when age % (chilled ? 8 : 4) == 0.
        // A one-tick source bite event otherwise disappears between checks.
        // Keep native target/phase guards; the saved animation clock still owns
        // bite rate, chill, butter, ice and the one-bite-per-event reservation.
        __state = new(__instance.DataID, __instance.mZombieAge);
        __instance.mZombieAge &= ~7;
    }
    private static void Finalizer(Zombie __instance, State? __state)
    {
        if (__state != null && __instance.DataID == __state.Id)
            __instance.mZombieAge = __state.Age;
    }
}
