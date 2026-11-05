using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Wall-nut/Tall-nut retain terrain, blocking, vault prevention, health
// and serialization. Retaliation only follows a bite that actually removed HP.
internal static class ElementalNuts
{
    internal sealed record Bite(Board Board, uint PlantId, uint ZombieId,
        FusionPlant Identity, int Health);

    internal static int DamageStage(int health, int maximum) => maximum <= 0 ? 0
        : health < maximum / 3 ? 2 : health < (long)maximum * 2 / 3 ? 1 : 0;

    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsRetaliatingNut(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = plant.mSeedType == SeedType.Tallnut ? 8000 : 4000;
        if (FusionPlant.From(plant) == FusionPlant.ThornNut)
            plant.mLaunchCounter = plant.mShootingCounter = 0;
    }

    internal static Bite? BeforeBite(Zombie zombie, Plant plant) =>
        FusionPlant.IsRetaliatingNut(FusionPlant.From(plant)) && ChomperFusions.Active(plant)
        && !plant.NotOnGround() && plant.mPlantHealth > 0
        && zombie.mBoard?.Pointer == plant.mBoard.Pointer && !zombie.mMindControlled
        && !zombie.mDead && !zombie.IsDeadOrDying()
        ? new(plant.mBoard, plant.DataID, zombie.DataID, FusionPlant.From(plant), plant.mPlantHealth) : null;

    internal static void AfterBite(Zombie zombie, Plant plant, Bite? bite)
    {
        // The native lethal bite calls Plant.Die before returning. Its HP and
        // ID remain valid; do not discard the last bite because mDead is set.
        if (bite == null || plant.DataID != bite.PlantId || zombie.DataID != bite.ZombieId
            || plant.mBoard?.Pointer != bite.Board.Pointer || zombie.mBoard?.Pointer != bite.Board.Pointer
            || plant.mPlantHealth >= bite.Health || zombie.mDead || zombie.IsDeadOrDying()
            || zombie.mMindControlled) return;
        int lost = (int)Math.Min(int.MaxValue, (long)bite.Health - plant.mPlantHealth);
        if (bite.Identity == FusionPlant.SpikeNut || bite.Identity == FusionPlant.ThornNut)
            zombie.TakeDamage(lost, DamageFlags.HitsShieldAndBody);
        if (bite.Identity == FusionPlant.SpikeNut || bite.Identity == FusionPlant.ThornNut) return;
        int count = BiteEvents.Count(plant, zombie, lost);
        for (int i = 0; i < count && zombie.DataID == bite.ZombieId
            && zombie.mBoard?.Pointer == bite.Board.Pointer && !zombie.mDead && !zombie.IsDeadOrDying(); ++i)
        {
            if (bite.Identity == FusionPlant.ButterNut)
                ButterEffects.Apply(zombie);
            else if (FusionPlant.IsFlameNut(bite.Identity))
            {
                // Source EatEffect applies the 20 damage before SetJalaed.
                zombie.TakeDamage(20, DamageFlags.HitsShieldAndBody);
                if (zombie.DataID == bite.ZombieId && zombie.mBoard?.Pointer == bite.Board.Pointer)
                    Overheat.Apply(zombie);
            }
            else
            {
                zombie.ApplyChill(false);
                SnowPea.Hit(zombie); // Shared ten points and escalating freeze threshold.
            }
        }
    }

    internal static bool BalloonNear(Plant plant, Zombie zombie, float radius) =>
        zombie.mBoard?.Pointer == plant.mBoard.Pointer && !zombie.mDead && !zombie.IsDeadOrDying()
        && !zombie.mMindControlled && zombie.mZombieType == ZombieType.Balloon && zombie.IsFlying()
        && zombie.mRow == plant.mRow
        && zombie.GetZombieRect().xMax >= plant.mX + plant.mWidth / 2f - radius
        && zombie.GetZombieRect().xMin <= plant.mX + plant.mWidth / 2f + radius;

    internal static void UpdateThorn(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.ThornNut || !ChomperFusions.Active(plant)
            || plant.NotOnGround()) return;
        bool raised = false;
        var balloons = new List<(Zombie Zombie, uint Id)>();
        var zombies = plant.mBoard.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || !BalloonNear(plant, zombie, 120)) continue;
            raised = true;
            if (BalloonNear(plant, zombie, 25)) balloons.Add((zombie, zombie.DataID));
        }
        int mode = raised ? 1 : 0;
        if (plant.mLaunchCounter != mode)
        {
            plant.mLaunchCounter = mode;
            plant.mShootingCounter = raised ? 58 : 50;
        }
        else if (plant.mShootingCounter > 0) --plant.mShootingCounter;
        foreach (var (zombie, id) in balloons)
            if (zombie.DataID == id && BalloonNear(plant, zombie, 25))
                zombie.LandFlyer(DamageFlags.HitsShieldAndBody);
    }

    internal static void Heal(Board board, bool fire, int row = -1)
    {
        if (fire) ReactivePots.Fire(board, row);
        var plants = board.m_plants;
        for (int i = 0; i < plants.Count; ++i)
        {
            var plant = plants[i];
            if (plant == null || !ChomperFusions.Active(plant) || plant.NotOnGround()
                || plant.mBoard?.Pointer != board.Pointer || (fire && plant.mRow != row)
                || FusionPlant.From(plant) != (fire ? FusionPlant.FlameTallNut : FusionPlant.FrostTallNut)) continue;
            plant.mPlantHealth = (int)Math.Min(plant.mPlantMaxHealth,
                (long)plant.mPlantHealth + (fire ? 3000 : 1000));
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class ElementalNutInitializePatch
{
    private static void Postfix(Plant __instance) => ElementalNuts.Initialize(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class ElementalNutBitePatch
{
    internal static void Prefix(Zombie __instance, Plant thePlant, out ElementalNuts.Bite? __state) =>
        __state = thePlant == null ? null : ElementalNuts.BeforeBite(__instance, thePlant);
    internal static void Postfix(Zombie __instance, Plant thePlant, ElementalNuts.Bite? __state) =>
        ElementalNuts.AfterBite(__instance, thePlant, __state);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.BurnRow))]
internal static class FlameTallNutHealPatch
{
    private static void Postfix(Plant __instance, int theRow)
    {
        if (__instance.mBoard != null && __instance.mBoard.Pointer != CherryBombPlants.DisposingBoard)
            ElementalNuts.Heal(__instance.mBoard, true, theRow);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class FrostTallNutSpecialHealPatch
{
    // DoSpecial contains an inlined IceZombies loop in this native build.
    internal static void Prefix(Plant __instance, bool __runOriginal, out Board? __state) =>
        __state = __runOriginal && __instance.mSeedType == SeedType.Iceshroom && ChomperFusions.Active(__instance)
            ? __instance.mBoard : null;
    internal static void Postfix(Board? __state)
    {
        if (__state != null) ElementalNuts.Heal(__state, false);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.IceZombies))]
internal static class FrostTallNutDirectHealPatch
{
    private static void Postfix(Plant __instance, bool __runOriginal)
    {
        if (__runOriginal && __instance.mBoard != null && __instance.mBoard.Pointer != CherryBombPlants.DisposingBoard)
            ElementalNuts.Heal(__instance.mBoard, false);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class ThornNutAbilityPatch
{
    private static void Postfix(Plant __instance) => ElementalNuts.UpdateThorn(__instance);
}
