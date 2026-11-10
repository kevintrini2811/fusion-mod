using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class ChomperVariants
{
    internal sealed record Bite(Plant Plant, uint Id, PlantState Before);
    [ThreadStatic] internal static Bite? Current;
    [ThreadStatic] internal static bool ApplyingEffect;

    internal static bool CanSwallow(Zombie zombie) => !zombie.mDead && !zombie.IsDeadOrDying()
        && !zombie.mMindControlled && zombie.mZombieType is not (ZombieType.Gargantuar
            or ZombieType.RedeyeGargantuar or ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target);

    internal static void AfterNative(Plant plant, PlantState before)
    {
        if (!ChomperFusions.Active(plant) || !FusionTime.IsSourcePulse(plant.mBoard)) return;
        var identity = FusionPlant.From(plant);
        if (identity != FusionPlant.DoomChomper && identity != FusionPlant.Frostbiter) return;
        if (!FusionTime.SourceStep(plant, typeof(ChomperVariants))) return;
        if (identity == FusionPlant.DoomChomper)
        {
            if (before != PlantState.ChomperDigesting && plant.mState == PlantState.ChomperDigesting)
                plant.mStateCountdown = 6000;
            if (before == PlantState.ChomperBiting && plant.mState == PlantState.ChomperBitingGotOne)
                DoomEffects.Explode(plant.mBoard, plant.mPlantCol + 1, plant.mRow,
                    plant.mX + plant.mWidth / 2f + 80, plant.mY + plant.mHeight / 2f, plant.mRenderOrder);
        }
        else if (identity == FusionPlant.Frostbiter && before == PlantState.ChomperBiting
            && plant.mState is PlantState.ChomperBitingGotOne or PlantState.ChomperBitingMissed)
        {
            bool previous = ApplyingEffect;
            ApplyingEffect = true;
            try { FrostBite(plant, plant.mState == PlantState.ChomperBitingGotOne); }
            finally { ApplyingEffect = previous; }
        }
    }

    internal static void Freeze(Zombie zombie, int duration)
    {
        FusionStatuses.Freeze(zombie, duration, duration + 1000);
    }

    internal static bool InFrostPulse(Plant plant, Zombie zombie)
    {
        if (Math.Abs(zombie.mRow - plant.mRow) > 1) return false;
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        var rect = zombie.GetZombieRect();
        float dx = x - Math.Clamp(x, rect.xMin, rect.xMax);
        float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
        float radius = 2f / 1.35f * 80;
        return dx * dx + dy * dy <= radius * radius;
    }

    internal static void FrostBite(Plant plant, bool swallowed)
    {
        var board = plant.mBoard;
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        bool freeze = swallowed && !PumpkinSupport.StoreIce(board, plant.mPlantCol, plant.mRow);
        var range = DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
            | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Underground;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && SquashFusions.Eligible(board, zombie, range))
                targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie, range)) continue;
            if (freeze)
            {
                // CreateFreeze applies 20 damage as well as the one-second freeze.
                Freeze(zombie, 100);
                zombie.TakeDamage(20, DamageFlags.HitsShieldAndBody);
            }
            if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie, range)
                || Math.Abs(zombie.mRow - plant.mRow) > 1) continue;
            // Acquisition and the actual pulse share the same reach.
            if (!InFrostPulse(plant, zombie)) continue;
            FusionStatuses.Chill(zombie, sourceTicks: 1500);
            // The actual call passes 0x33 (51), despite the almanac saying 50.
            SnowPea.Hit(zombie, 51);
            zombie.TakeDamage(40, DamageFlags.HitsShieldAndBody);
        }
        board.mApp.AddTodParticle(x, y, plant.mRenderOrder, ParticleEffect.IceSparkle);
        if (freeze) ElementalNuts.Heal(board, false);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class ChomperVariantsAbilitiesPatch
{
    internal static void Prefix(Plant __instance, bool __runOriginal, out ChomperVariants.Bite? __state)
    {
        __state = ChomperVariants.Current;
        var identity = FusionPlant.From(__instance);
        ChomperVariants.Current = (identity == FusionPlant.Frostbiter || identity == FusionPlant.DoomChomper)
            && __runOriginal && ChomperFusions.Active(__instance)
            && FusionTime.IsSourcePulse(__instance.mBoard) ? new(__instance, __instance.DataID, __instance.mState) : null;
    }
    internal static void Postfix(Plant __instance)
    {
        var bite = ChomperVariants.Current;
        if (bite != null && bite.Plant.Pointer == __instance.Pointer && bite.Id == __instance.DataID)
            ChomperVariants.AfterNative(__instance, bite.Before);
    }
    internal static void Finalizer(ChomperVariants.Bite? __state) => ChomperVariants.Current = __state;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
internal static class FrostbiterTargetPatch
{
    internal static void Postfix(Plant __instance, ref Zombie __result)
    {
        if (FusionPlant.From(__instance) != FusionPlant.Frostbiter || __result == null) return;
        // A frozen target can be swallowed at the longer native chomp reach.
        // An unfrozen target must already be within the damaging frost pulse.
        if (__instance.mState == PlantState.ChomperBiting && __instance.mStateCountdown == 0)
        {
            if (__result.mIceTrapCounter <= 0) __result = null!;
        }
        else if (!ChomperVariants.InFrostPulse(__instance, __result)
            && !(__result.mIceTrapCounter > 0 && ChomperVariants.CanSwallow(__result)))
            __result = null!;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class FrostbiterNativeBiteDamagePatch
{
    internal static bool Prefix(Zombie __instance, int theDamage, DamageFlags theDamageFlags)
    {
        // Native giants receive 40 for a failed bite. The source radius pulse
        // replaces that hit, so never add a second direct 40 to the giant.
        var bite = ChomperVariants.Current;
        return ChomperVariants.ApplyingEffect || bite == null || bite.Before != PlantState.ChomperBiting
            || FusionPlant.From(bite.Plant) != FusionPlant.Frostbiter || theDamage != 40
            || theDamageFlags != 0 || __instance.mBoard?.Pointer != bite.Plant.mBoard.Pointer;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.GetPlantAttackRect))]
internal static class ChomperVariantsRangePatch
{
    internal static void Postfix(Plant __instance, ref UnityEngine.Rect __result)
    {
        // Source ChomperSearchZombie casts from shadow.x + 1; the constructor
        // sets range 2 and IceChomper.get_ChompRange adds 1. Native FindTargetZombie
        // calls this rectangle helper, so retain native rows/height and filters.
        var identity = FusionPlant.From(__instance);
        if (identity != FusionPlant.DoomChomper && identity != FusionPlant.Frostbiter) return;
        float unit = 80f / 1.35f;
        __result.x = __instance.mX + __instance.mWidth / 2f + unit;
        __result.width = (identity == FusionPlant.Frostbiter ? 3 : 2) * unit;
    }
}
