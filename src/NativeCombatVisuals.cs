using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class NativeCombatVisuals
{
    internal static void Fire(Board board, int row, int order)
    {
        for (int col = 0; col < 9; ++col)
        {
            var effect = board.mApp.AddReanimation(board.GridToPixelX(col, row) - 40,
                board.GridToPixelY(col, row), order, ReanimationType.JalapenoFire);
            if (effect != null) { effect.mLoopType = ReanimLoopType.PlayOnce; effect.mAnimRate = 24; }
        }
    }

}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeBodyDamage))]
internal static class FusionAshDeathPatch
{
    private static bool Prefix(Zombie __instance, int theDamage, bool __runOriginal)
    {
        if (!__runOriginal || DoomEffects.Burning is not { } hit || hit.Zombie.Pointer != __instance.Pointer
            || hit.Id != __instance.DataID || theDamage <= 0 || theDamage < __instance.mBodyHealth || __instance.mDead
            || __instance.mZombieType == ZombieType.Boss
            || __instance.mZombiePhase is ZombiePhase.ZombieBurned or ZombiePhase.ZombieMowered) return true;
        // Armor and modifiers have resolved. Burn before TakeBodyDamage drops
        // the head: native ApplyBurn needs that head to select the ash corpse.
        int health = __instance.mBodyHealth;
        __instance.mBodyHealth = 0;
        var previous = DoomEffects.Burning; DoomEffects.Burning = null;
        try { __instance.ApplyBurn(); }
        catch
        {
            if (!__instance.mDead && __instance.mZombiePhase != ZombiePhase.ZombieBurned)
                __instance.mBodyHealth = health;
            throw;
        }
        finally { DoomEffects.Burning = previous; }
        if (__instance.mZombiePhase == ZombiePhase.ZombieBurned || __instance.mDead) return false;
        __instance.mBodyHealth = health;
        return true;
    }
}
