using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The aquatic variants retain native kelp acquisition, DragUnder and saves.
// StartRow owns only the shooting clip; TargetX owns the completed drag count.
internal static class KelpSpreadFusions
{
    [ThreadStatic] internal static bool NativeTick;

    internal static void Update(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || !FusionTime.SourceStep(plant, typeof(KelpSpreadFusions))) return;
        if (plant.mStateCountdown > 0) --plant.mStateCountdown;
        bool grabbing = plant.mState == PlantState.TanglekelpGrabbing;
        if (!KelpTick(plant)) return;
        bool previous = NativeTick;
        NativeTick = true;
        try { plant.UpdateTanglekelp(); }
        finally { NativeTick = previous; }
        AfterNative(plant, grabbing);
    }

    internal static void AfterNative(Plant plant, bool grabbing)
    {
        if (FusionTime.IsSourcePulse(plant.mBoard) && !grabbing && plant.mState == PlantState.TanglekelpGrabbing
            && FusionPlant.From(plant) == FusionPlant.StrangleKelp) plant.mTargetY = 215;
    }

    internal static bool Shooter(FusionPlant identity) => identity == FusionPlant.KelpSpreader
        || identity == FusionPlant.SquashSpreader;
    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!Shooter(identity) && identity != FusionPlant.StrangleKelp && identity != FusionPlant.Squashweed) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchCounter = plant.mStartRow = plant.mTargetX = plant.mTargetY = 0;
    }

    internal static bool Target(Plant plant, Zombie zombie, bool water)
    {
        if (Math.Abs(zombie.mRow - plant.mRow) > 1
            || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || !SquashFusions.Eligible(plant.mBoard, zombie,
                water ? DamageRangeFlags.Ground | DamageRangeFlags.Submerged : DamageRangeFlags.Ground)) return false;
        // Source Threekelp.SearchZombie searches all three rows. Water limits
        // the on-hit Tangle status, not the shooter's target acquisition.
        return zombie.GetZombieRect().xMax > plant.mX
            && zombie.GetZombieRect().xMin < plant.mBoard.GridToPixelX(8, zombie.mRow) + 160;
    }

    internal static void ShootTick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep
            || plant.mState == PlantState.TanglekelpGrabbing) return;
        bool water = FusionPlant.From(plant) == FusionPlant.KelpSpreader;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0) return;
            bool target = false;
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
                if (plant.mBoard.m_zombies[i] is { } zombie && Target(plant, zombie, water)) { target = true; break; }
            if (!target) return;
            plant.mStartRow = 100;
            plant.mLaunchCounter = water ? 150 : 300;
        }
        int elapsed = 100 - plant.mStartRow;
        if (elapsed == 50)
            for (int row = Math.Max(0, plant.mRow - 1); row <= Math.Min(plant.mBoard.GetNumRows() - 1, plant.mRow + 1); ++row)
                Shoot(plant, row, water);
        // Both source shooters redirect a missing edge-row shot after .2s.
        if (elapsed == (water ? 50 : 70) && (plant.mRow == 0 || plant.mRow == plant.mBoard.GetNumRows() - 1))
            Shoot(plant, plant.mRow, water);
        --plant.mStartRow;
    }
    private static void Shoot(Plant plant, int row, bool water)
    {
        if (water) KelpProjectiles.Shoot(plant, row);
        else MiniSquashProjectiles.Spawn(plant, row);
    }

    private static void FinishVictim(Plant plant)
    {
        var victim = plant.mBoard.ZombieTryToGet(plant.mTargetZombieID);
        plant.mTargetZombieID = 0;
        if (victim != null && !victim.mDead && victim.mBoard?.Pointer == plant.mBoard.Pointer)
            victim.DieWithLoot();
    }

    internal static bool KelpTick(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if ((identity == FusionPlant.KelpSpreader || identity == FusionPlant.StrangleKelp)
            && !FusionTime.IsSourcePulse(plant.mBoard)) return false;
        if (identity == FusionPlant.KelpSpreader)
        {
            ShootTick(plant);
            if (!ChomperFusions.Active(plant)) return false;
            if (plant.mState == PlantState.TanglekelpGrabbing && plant.mStateCountdown == 0)
            {
                // Threekelp inherits Tanglekelp.Moving: the completed drag
                // removes both victim and plant. It has no reusable-grab upgrade.
                FinishVictim(plant);
                plant.Die();
                return false;
            }
            return true;
        }
        if (identity != FusionPlant.StrangleKelp) return true;
        if (!ChomperFusions.Active(plant)) return false;
        if (plant.mState != PlantState.TanglekelpGrabbing)
        {
            if (plant.mLaunchCounter > 0) { --plant.mLaunchCounter; return false; }
            return true;
        }
        if (plant.mTargetY > 0)
        {
            int elapsed = 215 - --plant.mTargetY;
            if (elapsed == 58)
                MiniSquashProjectiles.Area(plant.mBoard, plant.mRow, plant.mX + plant.mWidth * .5f,
                    plant.mY + plant.mHeight * .5f, 600, plant.mRenderOrder, sourceRadius: 1);
            plant.mStateCountdown = plant.mTargetY == 0 ? 84 : 100;
            return false;
        }
        if (plant.mStateCountdown != 0) return true;
        // Native UpdateTanglekelp would remove the plant before finishing the
        // victim. Finish that same saved DataID, then retain two more uses.
        var board = plant.mBoard;
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && zombie.DataID == (uint)plant.mTargetZombieID
                && !zombie.mDead && zombie.mBoard?.Pointer == board.Pointer)
            { zombie.DieWithLoot(); break; }
        plant.mTargetZombieID = 0;
        if (++plant.mTargetX >= 3) { plant.Die(); return false; }
        plant.mState = PlantState.Notready;
        plant.mLaunchCounter = 100;
        return false;
    }

    internal static void WeedTick(Plant plant)
    {
        if (!ChomperFusions.Active(plant)) return;
        var board = plant.mBoard;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && GroundWeeds.Target(plant, zombie))
                targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult)
            { zombie.DieWithLoot(); plant.Die(); return; }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0 && plant.mLaunchCounter == 0 && targets.Count > 0)
        { plant.mStartRow = 67; plant.mLaunchCounter = 150; }
        if (plant.mStartRow == 0 || 67 - --plant.mStartRow != 33) return;
        bool hit = false;
        foreach (var (zombie, id) in targets)
        {
            if (!ChomperFusions.Active(plant)) return;
            if (zombie.DataID != id || !GroundWeeds.Target(plant, zombie)) continue;
            hit = true;
            zombie.TakeDamage(20, DamageFlags.BypassesShield);
            if (zombie.DataID == id && GroundWeeds.Target(plant, zombie) && !KelpEffects.Immune(zombie))
            {
                zombie.mPosX = Math.Min(850, zombie.mPosX + .2f * ScaredyFusions.SourceUnit);
                zombie.mX = (int)zombie.mPosX;
            }
        }
        if (hit && ChomperFusions.Active(plant)) MiniSquashProjectiles.Spawn(plant, plant.mRow);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class KelpSpreadInitializePatch
{ private static void Postfix(Plant __instance) => KelpSpreadFusions.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateTanglekelp))]
internal static class KelpSpreadAbilityPatch
{
    private static bool Prefix(Plant __instance, out bool? __state)
    {
        __state = FusionTime.IsSourcePulse(__instance.mBoard)
            ? __instance.mState == PlantState.TanglekelpGrabbing : null;
        return KelpSpreadFusions.NativeTick || KelpSpreadFusions.KelpTick(__instance);
    }
    private static void Postfix(Plant __instance, bool? __state)
    {
        if (!KelpSpreadFusions.NativeTick && __state.HasValue
            && FusionTime.IsSourcePulse(__instance.mBoard)) KelpSpreadFusions.AfterNative(__instance, __state.Value);
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SquashSpreadAbilityPatch
{
    internal static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (identity == FusionPlant.KelpSpreader || identity == FusionPlant.StrangleKelp)
        {
            KelpSpreadFusions.Update(__instance);
            return false;
        }
        if (identity == FusionPlant.SquashSpreader)
        {
            if (FusionTime.SourceStep(__instance, typeof(KelpSpreadFusions))) KelpSpreadFusions.ShootTick(__instance);
            return false;
        }
        if (identity == FusionPlant.Squashweed)
        {
            if (FusionTime.SourceStep(__instance, typeof(KelpSpreadFusions))) KelpSpreadFusions.WeedTick(__instance);
            return false;
        }
        return true;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class KelpSpreadShootingPatch
{
    private static bool Prefix(Plant __instance) => !KelpSpreadFusions.Shooter(FusionPlant.From(__instance))
        && FusionPlant.From(__instance) != FusionPlant.StrangleKelp && FusionPlant.From(__instance) != FusionPlant.Squashweed;
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class KelpSpreadFirePatch
{ private static bool Prefix(Plant __instance) => !KelpSpreadFusions.Shooter(FusionPlant.From(__instance)); }
