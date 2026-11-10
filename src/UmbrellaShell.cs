using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The native Umbrella owns bungee/catapult protection, terrain and save fields.
// CornUmbrella's contact pulse and NutUmbrella's damage are source-only behavior.
internal static class UmbrellaShell
{
    internal const float SourceUnit = 80f / 1.35f;
    internal const int CooldownTicks = 100;
    internal const int AnimationTicks = 84; // Original one-second block at speed 1.2.

    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsContactUmbrella(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = FusionPlant.From(plant) == FusionPlant.UmbrellaShell ? 4000 : 1000;
        plant.mLaunchCounter = plant.mShootingCounter = 0;
    }

    internal static bool CanHit(Plant plant, Zombie zombie) => !zombie.mDead
        && !zombie.IsDeadOrDying() && !zombie.mMindControlled
        && zombie.mBoard?.Pointer == plant.mBoard.Pointer
        && zombie.EffectedByDamage(DamageRangeFlags.Ground)
        && zombie.mZombieType is not (ZombieType.Boss or ZombieType.Zamboni or ZombieType.Catapult);

    internal static void Update(Plant plant)
    {
        if (!FusionPlant.IsContactUmbrella(FusionPlant.From(plant)) || !ChomperFusions.Active(plant)) return;
        if (!FusionTime.SourceStep(plant, typeof(UmbrellaShell))) return;
        if (plant.mShootingCounter > 0) --plant.mShootingCounter;
        if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
        if (FusionPlant.From(plant) == FusionPlant.UmbrellaThorn) { Thorn(plant); return; }
        var board = plant.mBoard;
        uint plantId = plant.DataID;
        bool clove = FusionPlant.From(plant) == FusionPlant.UmbrellaClove;
        float x = plant.mX + plant.mWidth / 2f;
        float y = plant.mY + plant.mHeight / 2f - (clove ? .5f * SourceUnit : 0);
        float radius = clove ? 2 * SourceUnit : 160;
        // Source contact collider is 2.355 units wide. Require contact in the
        // same row before its radius-two pulse, using native target rectangles.
        // Clove retains its own wider collider and source-unit circle.
        bool contact = false;
        var zombies = plant.mBoard.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow || !CanHit(plant, zombie)) continue;
            var rect = zombie.GetZombieRect();
            float left = clove ? x + (-.175154924f - 2.75155258f / 2) * SourceUnit : x - 82;
            float right = clove ? x + (-.175154924f + 2.75155258f / 2) * SourceUnit : x + 107;
            if (rect.xMax >= left && rect.xMin <= right) { contact = true; break; }
        }
        if (!contact) return;
        plant.mLaunchCounter = CooldownTicks;
        plant.mShootingCounter = AnimationTicks;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || clove && zombie.mRow != plant.mRow || !CanHit(plant, zombie)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax);
            float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= radius * radius) targets.Add((zombie, zombie.DataID));
        }
        int pulseDamage = FusionPlant.From(plant) == FusionPlant.UmbrellaKale ? 0
            : FusionPlant.From(plant) == FusionPlant.UmbrellaRind ? (int)Math.Min(int.MaxValue, 80L + plant.mTargetX) : 40;
        if (FusionPlant.From(plant) == FusionPlant.UmbrellaRind) plant.mTargetX = 0;
        int count = 0;
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !CanHit(plant, zombie)) continue;
            ++count;
            if (pulseDamage > 0) zombie.TakeDamage(pulseDamage, DamageFlags.BypassesShield);
            if (zombie.DataID != id || !CanHit(plant, zombie)) continue;
            if (FusionPlant.From(plant) == FusionPlant.UmbrellaClove)
            {
                zombie.StopEating();
                zombie.mYuckyFace = true;
                zombie.mYuckyFaceCounter = 169;
            }
            if (FusionPlant.From(plant) == FusionPlant.UmbrellaHusk) ButterEffects.Apply(zombie, PotBoosts.HasSupport(plant, FusionPlant.UmbrellaPot) ? 400 : 200);
            // Source Object equality against null selects two units without a
            // pot, three with a pot. Do not invert the absent-support branch.
            float push = FusionPlant.From(plant) == FusionPlant.UmbrellaClove
                ? (PotBoosts.HasSupport(plant, FusionPlant.UmbrellaPot) ? 4 : 2) * SourceUnit
                : PotBoosts.HasSupport(plant, FusionPlant.UmbrellaPot) ? 240 : 160;
            if (zombie.mIsEating) zombie.StopEating();
            zombie.mPosX = Math.Min(850, zombie.mPosX + push);
            zombie.mX = (int)zombie.mPosX;
            zombie.UpdateZombiePosition();
        }
        // Inherited CornUmbrella.DamageHealth charges once per pulse, by count.
        if (count == 0 || plant.DataID != plantId || plant.mBoard?.Pointer != board.Pointer
            || !ChomperFusions.Active(plant)) return;
        // MoneyUmbrella overrides the inherited contact effect. It pays no
        // money for touching zombies and does not spend its own health.
        if (FusionPlant.From(plant) == FusionPlant.SilverUmbrella) return;
        plant.mPlantHealth -= LumosEnergy.Absorb(plant, count == 1 ? 100 : count <= 4 ? 150 : 250);
        if (plant.mPlantHealth <= 0) plant.Die();
    }

    // CactusUmbrella searches a radius-one elevated circle in its own row.
    // It never runs the ground contact damage inherited by the other umbrellas.
    internal static void Thorn(Plant plant)
    {
        var board = plant.mBoard;
        uint plantId = plant.DataID;
        float x = plant.mX + plant.mWidth / 2f;
        float y = plant.mY + plant.mHeight / 2f - .5f * SourceUnit;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mDead || zombie.IsDeadOrDying() || zombie.mMindControlled
                || zombie.mBoard?.Pointer != board.Pointer || zombie.mRow != plant.mRow || !zombie.IsFlying()) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax);
            float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= SourceUnit * SourceUnit)
                targets.Add((zombie, zombie.DataID));
        }
        if (targets.Count == 0) return;
        plant.mLaunchCounter = CooldownTicks;
        plant.mShootingCounter = AnimationTicks;
        // Source pays once before the knockback callbacks, even if this kills it.
        plant.mPlantHealth -= targets.Count == 1 ? 100 : targets.Count <= 4 ? 150 : 250;
        if (plant.mPlantHealth <= 0) plant.Die();
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || zombie.mDead || zombie.IsDeadOrDying()
                || zombie.mMindControlled || zombie.mBoard?.Pointer != board.Pointer) continue;
            if (zombie.mZombieType == ZombieType.Balloon) zombie.LandFlyer(DamageFlags.BypassesShield);
            if (zombie.DataID != id || zombie.mDead || zombie.IsDeadOrDying()
                || plant.DataID != plantId || plant.mBoard?.Pointer != board.Pointer) continue;
            zombie.mPosX = Math.Min(850, zombie.mPosX
                + (PotBoosts.HasSupport(plant, FusionPlant.UmbrellaPot) ? 3 : 2) * SourceUnit);
            zombie.mX = (int)zombie.mPosX;
            zombie.UpdateZombiePosition();
        }
    }

}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class UmbrellaShellInitializePatch
{
    private static void Postfix(Plant __instance) => UmbrellaShell.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class UmbrellaShellAbilitiesPatch
{
    private static void Postfix(Plant __instance) => UmbrellaShell.Update(__instance);
}
