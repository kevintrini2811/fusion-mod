using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;
using ZombieLibrary;

namespace FusionMod;

internal static class HypnoticTransmutations
{
    internal const int CatalystMarker = -430133;
    private const int CryonicMarker = -430104;
    internal const int CatalystFirstShot = 300, CatalystInterval = 600;
    private const int FrostBodyHealth = 270, FrostArmorHealth = 2200;

    internal static bool IsCatalyst(Zombie zombie) => zombie.mMindControlled
        && zombie.mZombieType == ZombieType.PeaHead && zombie.mSummonCounter == CatalystMarker;

    internal static bool IsCryonic(Zombie zombie) => zombie.mSummonCounter == CryonicMarker
        && zombie.mZombieType is ZombieType.TallnutHead or ZombieType.Normal;

    private static Zombie? Spawn(Board board, Zombie biter, bool water, bool cryonic)
    {
        if (water) return board.AddZombieInRow(ZombieType.Pail, biter.mRow, -1, false);
        if (!cryonic) return Zombies.Spawn(FusionZombies.Catalyst, board, biter.mRow);
        // Reuse the native Zombotany Tall-nut's head, armor damage animation
        // and loss of its helmet. Only the frost state needs a saved marker.
        var ally = board.AddZombieInRow(ZombieType.TallnutHead, biter.mRow, -1, false);
        if (ally == null || ally.mDead) return null;
        ally.mSummonCounter = CryonicMarker;
        ally.mBodyHealth = ally.mBodyMaxHealth = FrostBodyHealth;
        ally.mHelmHealth = ally.mHelmMaxHealth = FrostArmorHealth;
        return ally;
    }

    internal static bool Bite(Zombie biter)
    {
        var board = biter.mBoard;
        if (board == null || board.mPaused || !ScaredContent.Playing(board) || biter.mMindControlled
            || biter.mDead || biter.IsDeadOrDying() || !biter.mHasHead) return false;
        var plant = biter.FindPlantTarget(ZombieAttackType.Chew);
        if (plant == null || plant.mBoard?.Pointer != board.Pointer || plant.mIsAsleep
            || !ChomperFusions.Active(plant)) return false;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.CurseShroom || identity == FusionPlant.ZodiacShroom)
        {
            if (biter.mZombieType is not (ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target))
                HypnoBiteEffects.Attach(plant, biter, identity == FusionPlant.CurseShroom);
            return true;
        }
        bool cryonic = identity == FusionPlant.CryonicShroom;
        if (!cryonic && identity != FusionPlant.CatalystShroom) return false;
        if (biter.mZombieType is ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target) return true;

        uint plantId = plant.DataID, biterId = biter.DataID;
        float x = biter.mPosX;
        var center = biter.GetZombieRect().center;
        int col = Math.Clamp(board.PixelToGridX((int)center.x, (int)center.y), 0, 8);
        bool water = board.IsPoolSquare(col, biter.mRow);
        var ally = Spawn(board, biter, water, cryonic);
        if (ally == null || ally.mDead) return true;
        uint allyId = ally.DataID;
        ally.mPosX = ally.mX = x;
        if (water) ally.CheckForPool();
        // Commit the replacement only after allocation and the native team
        // change succeed. A difficulty modifier may resist hypnosis.
        if (!HypnoPlants.MindControl(ally) || !StillOwns(plant, plantId, board)
            || biter.DataID != biterId || biter.mBoard?.Pointer != board.Pointer
            || biter.mDead || biter.IsDeadOrDying() || biter.mMindControlled)
        {
            if (ally.DataID == allyId && ally.mBoard?.Pointer == board.Pointer && !ally.mDead) ally.DieNoLoot();
            return true;
        }
        if (cryonic) IceInstantFusions.OnIceFusion(plant);
        if (StillOwns(plant, plantId, board)) PlayerPlantingScope.Consume(plant);
        if (biter.DataID == biterId && biter.mBoard?.Pointer == board.Pointer
            && !biter.mDead && !biter.IsDeadOrDying()) biter.DieNoLoot();
        return true;
    }

    private static bool StillOwns(Plant plant, uint id, Board board) => plant.DataID == id
        && plant.mBoard?.Pointer == board.Pointer && ChomperFusions.Active(plant);

    internal static void FireDamage(Zombie zombie) =>
        zombie.TakeBodyDamage((FrostBodyHealth + FrostArmorHealth) / 3, (DamageFlags)0);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.AnimateChewSound))]
internal static class HypnoticTransmutationBitePatch
{
    private static bool Prefix(Zombie __instance, bool __runOriginal) =>
        !__runOriginal || !HypnoticTransmutations.Bite(__instance);
}

[HarmonyPatch]
internal static class CryonicColdImmunityPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Zombie), nameof(Zombie.ApplyChill)),
        AccessTools.Method(typeof(Zombie), nameof(Zombie.HitIceTrap))
    };

    private static bool Prefix(Zombie __instance) => !HypnoticTransmutations.IsCryonic(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.ApplyBurn))]
internal static class CryonicFireDamagePatch
{
    private static bool Prefix(Zombie __instance, bool __runOriginal)
    {
        if (!__runOriginal || !HypnoticTransmutations.IsCryonic(__instance)) return true;
        if (!__instance.mDead && !__instance.IsDeadOrDying()) HypnoticTransmutations.FireDamage(__instance);
        return false;
    }
}
