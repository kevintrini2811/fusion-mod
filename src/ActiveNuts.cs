using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Wall-nut/Tall-nut retain blocking, terrain and serialization. These native
// variants have no shooter. Saved counters belong exclusively to this owner.
internal static class ActiveNuts
{
    internal const int EnergyClipTicks = 75, StackClipTicks = 50, JumpTicks = 167;
    internal static bool Airborne(Plant plant) => FusionPlant.From(plant) == FusionPlant.SquashNut
        && plant.mStartRow is > 50 and <= 117;
    internal static int HealLimit(Plant plant) => FusionPlant.From(plant) == FusionPlant.ThreeNuts
        ? Math.Min(plant.mPlantMaxHealth, 12000 - Math.Clamp(plant.mTargetX, 0, 2) * 4000)
        : plant.mPlantMaxHealth;
    internal static int StoredEnergy(int damage, bool melon) =>
        (int)Math.Min(int.MaxValue, (long)damage * (damage > 1000 ? 4 : damage > 500 ? 3 : damage > 100 ? 2 : 1)
            * (melon ? 3 : 2) / 2);

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsActiveNut(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.MelonNut ? 8000
            : identity == FusionPlant.ThreeNuts ? 12000 : 4000;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = plant.mTargetX = plant.mTargetY = 0;
    }

    internal static void LoseHeads(Plant plant)
    {
        if (FusionPlant.From(plant) != FusionPlant.ThreeNuts) return;
        int stage = plant.mPlantHealth < 4000 ? 2 : plant.mPlantHealth < 8000 ? 1 : 0;
        if (stage <= plant.mTargetX) return;
        plant.mTargetX = stage;
        plant.mShootingCounter = 25;
    }

    internal static Zombie? Target(Plant plant, float radius = float.MaxValue)
    {
        Zombie? found = null;
        float distance = radius;
        float x = plant.mX + plant.mWidth / 2f;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            float d = Math.Abs((rect.xMin + rect.xMax) / 2 - x);
            if (d < distance) { distance = d; found = zombie; }
        }
        return found;
    }

    internal static void Tick(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsActiveNut(identity) || !ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (!FusionTime.SourceStep(plant, typeof(ActiveNuts))) return;
        if (FusionPlant.IsEnergyNut(identity))
        {
            if (plant.mTargetY > 0 && --plant.mLaunchCounter <= 0)
            {
                plant.mTargetY = Math.Max(0, plant.mTargetY - plant.mShootingCounter);
                plant.mLaunchCounter = 50;
            }
            if (plant.mStartRow == 0 && plant.mTargetX > 1000 && Target(plant) != null)
            {
                plant.mStartRow = EnergyClipTicks;
                plant.mTargetY = Math.Max(plant.mTargetY, plant.mTargetX);
                plant.mShootingCounter = Math.Max(1, plant.mTargetX / 10);
                plant.mLaunchCounter = 1;
            }
            if (plant.mStartRow > 0)
            {
                if (EnergyClipTicks - plant.mStartRow == 42 && NutProjectiles.Lob(plant, Target(plant), plant.mTargetX))
                    plant.mTargetX = 0;
                --plant.mStartRow;
            }
            return;
        }
        if (identity == FusionPlant.ThreeNuts)
        {
            LoseHeads(plant);
            if (plant.mShootingCounter > 0) --plant.mShootingCounter;
            if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
            if (plant.mStartRow == 0 && plant.mLaunchCounter == 0 && PuffLobbers.Target(plant) != null)
            {
                plant.mStartRow = StackClipTicks;
                plant.mLaunchCounter = 142 + plant.mBoard.Rand(16);
            }
            if (plant.mStartRow > 0)
            {
                if (StackClipTicks - plant.mStartRow == 25) NutProjectiles.Roll(plant);
                --plant.mStartRow;
            }
            return;
        }
        if (plant.mStartRow == 0 && Target(plant, ScaredyFusions.SourceUnit) != null)
            plant.mStartRow = JumpTicks;
        if (plant.mStartRow > 0)
        {
            if (JumpTicks - plant.mStartRow == 117) Land(plant);
            --plant.mStartRow;
        }
    }

    private static void Land(Plant plant)
    {
        var targets = new List<(Zombie Zombie, uint Id)>();
        float x = plant.mX + plant.mWidth / 2f;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            if (Math.Abs(x - Math.Clamp(x, rect.xMin, rect.xMax)) <= ScaredyFusions.SourceUnit)
                targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground))
                zombie.TakeDamage(160, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
        plant.mBoard.mApp.AddTodParticle(x, plant.mY + plant.mHeight / 2f, plant.mRenderOrder, ParticleEffect.DustSquash);
    }

    internal sealed class Hit
    {
        internal readonly Plant Plant;
        internal readonly Board Board;
        internal readonly uint Id;
        internal readonly int Health;
        internal readonly bool Bite;
        internal bool Applied;
        internal Hit(Plant plant, bool bite)
        { Plant = plant; Board = plant.mBoard; Id = plant.DataID; Health = plant.mPlantHealth; Bite = bite; }
        internal void Apply()
        {
            if (Applied || Plant.DataID != Id || Plant.mBoard?.Pointer != Board.Pointer
                || Plant.mPlantHealth >= Health) return;
            Applied = true;
            if (FusionPlant.IsEnergyNut(FusionPlant.From(Plant)))
            {
                int damage = Health - Plant.mPlantHealth;
                int absorbed = Math.Min(damage, Math.Max(0, Plant.mTargetY));
                Plant.mTargetY -= absorbed;
                Plant.mPlantHealth += absorbed;
                damage -= absorbed;
                int cap = FusionPlant.From(Plant) == FusionPlant.MelonNut ? 32000 : 4000;
                Plant.mTargetX = (int)Math.Min(cap, (long)Plant.mTargetX + StoredEnergy(damage,
                    FusionPlant.From(Plant) == FusionPlant.MelonNut));
            }
            LoseHeads(Plant);
        }
    }
    [ThreadStatic] internal static List<Hit>? Hits;
    internal static Hit? Capture(Plant plant, bool bite) => FusionPlant.IsActiveNut(FusionPlant.From(plant))
        && ChomperFusions.Active(plant) && plant.mPlantHealth > 0
        && Hits?.Any(hit => hit.Plant.Pointer == plant.Pointer) != true ? new(plant, bite) : null;
    internal static void Apply() { if (Hits != null) foreach (var hit in Hits) hit.Apply(); }
    internal static bool BeforeDeath(Plant plant)
    {
        if (PlayerPlantingScope.ConsumingIngredient == plant.Pointer || plant.mPlantHealth > 0) return true;
        var hit = Hits?.FirstOrDefault(hit => hit.Plant.Pointer == plant.Pointer && hit.Id == plant.DataID && !hit.Applied);
        if (hit == null) return true;
        hit.Apply();
        if (plant.mPlantHealth <= 0) return true;
        if (hit.Bite) --plant.mBoard.mPlantsEaten;
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class ActiveNutInitializePatch
{
    internal static void Postfix(Plant __instance) => ActiveNuts.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class ActiveNutTickPatch
{
    internal static void Postfix(Plant __instance) => ActiveNuts.Tick(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class ActiveNutFirePatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsActiveNut(FusionPlant.From(__instance));
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class ActiveNutBitePatch
{
    internal static bool Prefix(Plant thePlant, out List<ActiveNuts.Hit>? __state)
    {
        __state = ActiveNuts.Hits;
        if (thePlant != null && ActiveNuts.Capture(thePlant, true) is { } hit) ActiveNuts.Hits = new() { hit };
        return true;
    }
    [HarmonyPriority(Priority.Low)]
    internal static void Postfix(List<ActiveNuts.Hit>? __state)
    { if (!ReferenceEquals(ActiveNuts.Hits, __state)) ActiveNuts.Apply(); }
    internal static void Finalizer(List<ActiveNuts.Hit>? __state) => ActiveNuts.Hits = __state;
}
[HarmonyPatch]
internal static class ActiveNutProjectileHitPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.CheckForCollision)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.UpdateLobMotion)) };
    internal static void Prefix(Projectile __instance, out List<ActiveNuts.Hit>? __state)
    {
        __state = ActiveNuts.Hits;
        if (__instance.mDead || __instance.mBoard == null
            || __instance.mProjectileType is not (ProjectileType.ZombiePea or ProjectileType.Basketball)) return;
        var hits = new List<ActiveNuts.Hit>();
        for (int i = 0; i < __instance.mBoard.m_plants.Count; ++i)
            if (__instance.mBoard.m_plants[i] is { } plant && ActiveNuts.Capture(plant, false) is { } hit)
                hits.Add(hit);
        if (hits.Count > 0) ActiveNuts.Hits = hits;
    }
    [HarmonyPriority(Priority.Low)]
    internal static void Postfix(List<ActiveNuts.Hit>? __state)
    { if (!ReferenceEquals(ActiveNuts.Hits, __state)) ActiveNuts.Apply(); }
    internal static void Finalizer(List<ActiveNuts.Hit>? __state) => ActiveNuts.Hits = __state;
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class ActiveNutDeathPatch
{
    [HarmonyPriority(Priority.Low)]
    internal static bool Prefix(Plant __instance, bool __runOriginal) => !__runOriginal || ActiveNuts.BeforeDeath(__instance);
}

