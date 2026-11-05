using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class SpikeKelp
{
    internal const int AttackTicks = 67;
    internal static bool Is(Plant plant) => FusionPlant.From(plant) == FusionPlant.SpikeKelp;
    internal static bool Target(Plant plant, Zombie zombie)
    {
        if (zombie.mRow != plant.mRow || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || zombie.mZombiePhase is ZombiePhase.BalloonFlying or ZombiePhase.DiggerTunneling
                or ZombiePhase.DiggerTunnelingPauseWithoutAxe or ZombiePhase.ImpGettingThrown
            || !SquashFusions.Eligible(plant.mBoard, zombie,
                DamageRangeFlags.Ground | DamageRangeFlags.Submerged | DamageRangeFlags.OffGround)) return false;
        var rect = zombie.GetZombieRect();
        float x = plant.mX + plant.mWidth * .5f;
        return Math.Abs(x - Math.Clamp(x, rect.xMin, rect.xMax)) <= .5f * ScaredyFusions.SourceUnit;
    }
    internal static void Initialize(Plant plant)
    {
        if (!Is(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mStartRow = plant.mTargetX = 0;
    }
    internal static void Tick(Plant plant)
    {
        SeaPot.Advance(plant, TickOnce);
    }
    private static void TickOnce(Plant plant)
    {
        if (!Is(plant) || !ChomperFusions.Active(plant)) return;
        if (plant.mTargetX > 0) --plant.mTargetX;
        if (plant.mStartRow == 0)
        {
            if (plant.mTargetX > 0) return;
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
                if (plant.mBoard.m_zombies[i] is { } zombie && Target(plant, zombie))
                {
                    plant.mStartRow = AttackTicks;
                    plant.mTargetX = 95 + plant.mBoard.Rand(11);
                    return;
                }
            return;
        }
        if (AttackTicks - --plant.mStartRow == 33) Hit(plant);
    }
    internal static void Hit(Plant plant)
    {
        var board = plant.mBoard;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && Target(plant, zombie)) targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (!ChomperFusions.Active(plant)) return;
            if (zombie.DataID != id || !Target(plant, zombie)) continue;
            bool vehicle = zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult;
            zombie.TakeDamage(20, DamageFlags.BypassesShield | DamageFlags.Spike);
            if (zombie.DataID == id && zombie.mBoard?.Pointer == board.Pointer)
                KelpEffects.Add(zombie, 200);
            // Original damage is owed even when the attack kills the driver.
            if (vehicle)
            {
                plant.mPlantHealth -= PotDefense.Damage(plant, 10);
                if (plant.mPlantHealth <= 0) { plant.Die(); return; }
                plant.UpdateReanim(); plant.UpdateReanimColor();
            }
        }
    }
    internal static bool DriverCrush(Plant plant)
    {
        var frame = WeedFusions.CurrentCrush;
        return frame != null && frame.Zombie.DataID == frame.ZombieId
            && frame.Board.Pointer == plant.mBoard.Pointer
            && frame.Zombie.mBoard?.Pointer == frame.Board.Pointer
            && frame.Zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SpikeKelpInitializePatch
{
    private static void Postfix(Plant __instance) => SpikeKelp.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateTanglekelp))]
internal static class SpikeKelpAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!SpikeKelp.Is(__instance)) return true;
        SpikeKelp.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class SpikeKelpDriverPatch
{
    private static bool Prefix(Plant __instance) => !SpikeKelp.Is(__instance) || !SpikeKelp.DriverCrush(__instance);
}
