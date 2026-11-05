using HarmonyLib;
using Il2CppReloaded.Gameplay;
using LawnGlove;

namespace FusionMod;

// Native serialized counters own energy, clocks and complete pool handles.
// This family replaces native shooting; imported animation is visual only.
internal static class BeamShooters
{
    internal static int TargetCount(FusionPlant identity) => identity == FusionPlant.GatlingBeam ? 4
        : identity == FusionPlant.Rebeamer ? 2 : 1;
    internal static int ConsumeEnergy(int energy) => energy > 1 ? 1 + energy / 10 : 0;
    internal static int Damage(int energy) => 10 * (1 + ConsumeEnergy(energy));
    internal static int Charge(int energy, int count) => count * (1 + ConsumeEnergy(energy));

    internal static bool Active(Plant plant) => ChomperFusions.Active(plant) && !plant.mIsAsleep
        && LawnGloveMod.GetCarriedPlant(plant.mBoard)?.Pointer != plant.Pointer;

    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsBeamShooter(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mTargetX = 0;
        plant.mTargetY = 50;
        plant.mLaunchCounter = 0;
        ResetTargets(plant);
    }

    internal static uint TargetId(Plant plant, int slot) => slot switch
    {
        0 => (uint)plant.mTargetZombieID,
        1 => unchecked((uint)plant.mStartRow),
        2 => unchecked((uint)plant.mShootingCounter),
        _ => unchecked((uint)plant.mDoSpecialCountdown)
    };
    private static void SetTarget(Plant plant, int slot, uint id)
    {
        switch (slot)
        {
            case 0: plant.mTargetZombieID = (ZombieID)id; break;
            case 1: plant.mStartRow = unchecked((int)id); break;
            case 2: plant.mShootingCounter = unchecked((int)id); break;
            default: plant.mDoSpecialCountdown = unchecked((int)id); break;
        }
    }
    internal static void ResetTargets(Plant plant)
    {
        for (int i = 0; i < 4; ++i) SetTarget(plant, i, 0);
        plant.mLaunchRate = 0;
        plant.mLaunchCounter = plant.mStateCountdown = 0;
    }

    internal static bool ValidZombie(Plant plant, Zombie zombie) => zombie.mRow == plant.mRow
        && zombie.mBoard?.Pointer == plant.mBoard.Pointer
        && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)
        && zombie.GetZombieRect().center.x > plant.mX + plant.mWidth / 2f
        && zombie.GetZombieRect().center.x < 900;

    internal static Zombie? ZombieTarget(Plant plant, int slot)
    {
        uint id = TargetId(plant, slot);
        if (id == 0) return null;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && zombie.DataID == id
                && ValidZombie(plant, zombie)) return zombie;
        return null;
    }
    internal static bool ValidRelay(Plant source, Plant target) =>
        FusionPlant.From(target) == FusionPlant.SplitBeam && Active(target)
        && target.mBoard?.Pointer == source.mBoard.Pointer && target.mRow == source.mRow
        && target.mPlantCol > source.mPlantCol + 1;

    internal static Plant? RelayTarget(Plant source)
    {
        uint id = unchecked((uint)source.mLaunchRate);
        if (id == 0) return null;
        for (int i = 0; i < source.mBoard.m_plants.Count; ++i)
            if (source.mBoard.m_plants[i] is { } target && target.DataID == id
                && ValidRelay(source, target)) return target;
        return null;
    }
    private static Plant? FindRelay(Plant source)
    {
        Plant? nearest = null;
        for (int i = 0; i < source.mBoard.m_plants.Count; ++i)
        {
            var target = source.mBoard.m_plants[i];
            if (target != null && ValidRelay(source, target)
                && (nearest == null || target.mPlantCol < nearest.mPlantCol)) nearest = target;
        }
        return nearest;
    }
    private static Zombie? FindZombie(Plant plant, int slot, int count)
    {
        Zombie? nearest = null;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || !ValidZombie(plant, zombie)) continue;
            bool used = false;
            for (int other = 0; other < count; ++other)
                if (other != slot && TargetId(plant, other) == zombie.DataID) { used = true; break; }
            if (!used && (nearest == null || zombie.GetZombieRect().center.x < nearest.GetZombieRect().center.x))
                nearest = zombie;
        }
        return nearest;
    }

    // Four independent 5-bit countdowns fit in one saved native int.
    // A replacement target gets its own initial delay without delaying peers.
    private static int Timer(Plant plant, int slot) => plant.mLaunchCounter >> (slot * 5) & 31;
    private static void Timer(Plant plant, int slot, int ticks)
    {
        int shift = slot * 5;
        plant.mLaunchCounter = plant.mLaunchCounter & ~(31 << shift) | ticks << shift;
    }

    internal static void Tick(Plant plant)
    {
        if (!Active(plant)) return;
        if (--plant.mTargetY <= 0)
        {
            plant.mTargetX = Math.Clamp(plant.mTargetX + PumpkinFusions.LightLevel(plant), 0, 1000);
            plant.mTargetY = 50;
        }
        int count = TargetCount(FusionPlant.From(plant));
        var relay = RelayTarget(plant);
        if (relay == null && plant.mLaunchRate != 0) ResetTargets(plant);
        bool locked = false;
        for (int i = 0; i < count; ++i)
            if (ZombieTarget(plant, i) != null) locked = true;
        if (relay == null && !locked)
        {
            relay = FindRelay(plant);
            if (relay != null)
            {
                ResetTargets(plant);
                plant.mLaunchRate = unchecked((int)relay.DataID);
                Timer(plant, 0, 5);
            }
        }
        bool acquired = false;
        if (relay == null)
            for (int i = 0; i < count; ++i)
            {
                if (ZombieTarget(plant, i) != null) continue;
                SetTarget(plant, i, 0);
                var target = FindZombie(plant, i, count);
                Timer(plant, i, 0);
                if (target != null)
                {
                    SetTarget(plant, i, target.DataID);
                    Timer(plant, i, 10);
                    acquired = true;
                }
            }
        if (relay == null && !locked && !acquired)
        {
            plant.mLaunchCounter = plant.mStateCountdown = 0;
            return;
        }
        if (plant.mStateCountdown < int.MaxValue) ++plant.mStateCountdown;
        uint ownerId = plant.DataID;
        if (relay != null)
        {
            // ChargePlant subtracts deltaTime twice: first pulse at .05s,
            // then every .1s. TargetCount multiplies one energy expenditure.
            int timer = Math.Max(1, Timer(plant, 0)) - 1;
            Timer(plant, 0, timer > 0 ? timer : 10);
            if (timer > 0) return;
            int energy = plant.mTargetX;
            plant.mTargetX -= ConsumeEnergy(energy);
            relay.mTargetX = Math.Min(1000, relay.mTargetX + Charge(energy, count));
            return;
        }
        for (int slot = 0; slot < count; ++slot)
        {
            if (plant.DataID != ownerId || !Active(plant)) return;
            var zombie = ZombieTarget(plant, slot);
            if (zombie == null) continue;
            int timer = Math.Max(1, Timer(plant, slot)) - 1;
            Timer(plant, slot, timer > 0 ? timer : 20);
            if (timer > 0) continue;
            int energy = plant.mTargetX;
            plant.mTargetX -= ConsumeEnergy(energy);
            uint targetId = zombie.DataID;
            zombie.TakeDamage(Damage(energy), DamageFlags.BypassesShield);
            if (zombie.DataID == targetId)
                PlanternFusions.LaserHit(plant, zombie);
        }
    }

    internal static bool Click(Plant plant, int clicks, int player)
    {
        if (!FusionPlant.IsBeamShooter(FusionPlant.From(plant))) return false;
        if (clicks >= 0 && player == 0 && Active(plant)
            && plant.mBoard.mCursorObject.CursorType == CursorType.Normal
            && LawnGloveMod.GetCarriedPlant(plant.mBoard) == null) ResetTargets(plant);
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class BeamInitializePatch
{ private static void Postfix(Plant __instance) => BeamShooters.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class BeamAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsBeamShooter(FusionPlant.From(__instance))) return true;
        BeamShooters.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class BeamShootingPatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsBeamShooter(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class BeamFirePatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsBeamShooter(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.MouseDown))]
internal static class BeamClickPatch
{
    private static bool Prefix(Plant __instance, int theClickCount, int playerIndex) =>
        !BeamShooters.Click(__instance, theClickCount, playerIndex);
}
