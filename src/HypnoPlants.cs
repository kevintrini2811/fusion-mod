using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using LawnGlove;

namespace FusionMod;

internal static class HypnoPlants
{
    // Source CurrentFirstHealth/TotalFirstHealth include body and type-1 armor,
    // excluding held type-2 shields. Strictly below half, evaluated after a hit.
    internal static long Health(Zombie zombie) => Math.Max(0, zombie.mBodyHealth) + (long)Math.Max(0, zombie.mHelmHealth);
    internal static long MaxHealth(Zombie zombie) => Math.Max(0, zombie.mBodyMaxHealth) + (long)Math.Max(0, zombie.mHelmMaxHealth);
    internal static bool MindControl(Zombie zombie)
    {
        if (zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
            || zombie.mZombieType is ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target) return false;
        zombie.StartMindControlled();
        return zombie.mMindControlled;
    }

    internal static void MelonHit(Zombie zombie)
    {
        if (Health(zombie) < MaxHealth(zombie) / 2 && zombie.mBoard.Rand(4) == 0) MindControl(zombie);
    }

    internal static void Initialize(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.HypnoPepper) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mDoSpecialCountdown = plant.mLaunchCounter = plant.mStartRow = plant.mTargetY = 0;
    }

    internal static void TickPepper(Plant plant)
    {
        if (ChomperFusions.Active(plant) && !plant.mIsAsleep && ++plant.mTargetY >= 84) Explode(plant);
    }

    internal static void Explode(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mStartRow != 0) return;
        plant.mStartRow = 1;
        var board = plant.mBoard;
        int row = plant.mRow, order = plant.mRenderOrder;
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        uint plantId = plant.DataID;
        plant.Die();
        if (plant.DataID != plantId || !plant.mDead) return;
        var range = DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
            | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Underground;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && zombie.mRow == row
                && SquashFusions.Eligible(board, zombie, range)) targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie, range)) continue;
            if (Health(zombie) < 900 && MindControl(zombie)) continue;
            zombie.TakeDamage(1800, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
        }
        NativeCombatVisuals.Fire(board, row, order);
        board.mApp.SoundSystem.PlayFoley(FoleyType.JalapenoIgnite);
        board.ShakeBoard(3, -4);
    }

    internal static bool Bite(Plant plant, Zombie zombie)
    {
        if (FusionPlant.From(plant) != FusionPlant.HypnoMelon) return false;
        if (!ChomperFusions.Active(plant) || zombie.mBoard?.Pointer != plant.mBoard.Pointer
            || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled) return true;
        var plain = plant.mBoard.AddPlant(plant.mPlantCol, plant.mRow, SeedType.Melonpult, SeedType.None);
        if (plain == null || plain.mDead || !plain.mIsOnBoard) return true;
        // Allocate before consumption; preserve the support/pumpkin stack.
        PlayerPlantingScope.Consume(plant);
        MindControl(zombie);
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class HypnoPepperInitializePatch
{ private static void Postfix(Plant __instance) => HypnoPlants.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class HypnoPepperAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.HypnoPepper) return true;
        HypnoPlants.TickPepper(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class HypnoPepperSpecialPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.HypnoPepper) return true;
        HypnoPlants.Explode(__instance); return false;
    }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class HypnoMelonBitePatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Zombie __instance, Plant thePlant, bool __runOriginal) =>
        !__runOriginal || !HypnoPlants.Bite(thePlant, __instance);
}
