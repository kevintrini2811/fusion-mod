using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Kelp stacks outlive the slow. Both belong to the complete zombie DataID,
// persisted in an invisible native GridItem rather than a transient dictionary.
internal static class KelpEffects
{
    private const int Marker = -32000;
    internal static bool IsEffect(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == Marker;
    internal static bool Playing(Board board) => !board.mPaused
        && board.mApp.GameScene == GameScenes.Playing && board.mApp.GameMode != GameMode.ChallengeZenGarden;

    internal static GridItem? Find(Zombie zombie)
    {
        var items = zombie.mBoard.m_gridItems;
        for (int i = 0; i < items.Count; ++i)
            if (items[i] is { mDead: false } item && IsEffect(item)
                && item.mGridX == unchecked((int)zombie.DataID)) return item;
        return null;
    }
    internal static int Stacks(Zombie zombie) => Math.Max(0, Find(zombie)?.mGridY ?? 0);
    internal static bool Slowed(Zombie zombie) => Find(zombie)?.mGridItemCounter > 0;
    internal static bool Immune(Zombie zombie) => zombie.mZombieType is ZombieType.Boss
        or ZombieType.Gravestone or ZombieType.Target;

    private static GridItem? Create(Zombie zombie)
    {
        var item = zombie.mBoard.m_gridItems.DataArrayAlloc();
        if (item == null) return null;
        try
        {
            item.Init(zombie.mBoard.mApp, GridItemType.ZenTool);
            item.mGridItemState = GridItemState.Normal;
            item.mSunCount = Marker;
            item.mGridX = unchecked((int)zombie.DataID);
            item.mGridY = item.mGridItemCounter = 0;
            item.mX = item.mY = item.mPosX = item.mPosY = -1000;
            return item;
        }
        catch { item.GridItemDie(); throw; }
    }

    internal static int ThresholdDamage(int previousStacks, int maximum) => previousStacks > 5
        ? (int)(maximum * .01f) : 0;

    internal static bool Add(Zombie zombie, int ticks, bool suppressSeed = false)
    {
        if (Immune(zombie) || zombie.mMindControlled || zombie.mBoard == null) return false;
        var board = zombie.mBoard;
        uint id = zombie.DataID;
        // Bullet_kelp calls SetKelped after damage, including lethal damage.
        // Preserve its location before callbacks can recycle the zombie slot.
        int row = zombie.mRow, col = board.PixelToGridX((int)zombie.mPosX, (int)zombie.mPosY);
        var effect = Find(zombie) ?? Create(zombie);
        if (effect == null) return false;
        int previous = Math.Max(0, effect.mGridY);
        effect.mGridItemCounter = (int)Math.Min(int.MaxValue,
            (long)Math.Max(0, effect.mGridItemCounter) + Math.Max(0, ticks));
        // Commit before nested health writers. There is one increment per call.
        effect.mGridY = previous == int.MaxValue ? previous : previous + 1;
        int maximum = (int)Math.Min(int.MaxValue, (long)Math.Max(0, zombie.mBodyMaxHealth)
            + Math.Max(0, zombie.mHelmMaxHealth));
        if (suppressSeed) maximum /= 3;
        int damage = ThresholdDamage(previous, maximum);
        if (damage > 0 && !zombie.mDead && !zombie.IsDeadOrDying())
            zombie.TakeDamage(damage, DamageFlags.BypassesShield);
        // The source has two independent branches, not an else-if. At thirty
        // previous stacks, the one-percent hit precedes the full maximum hit.
        if (previous >= 30 && zombie.DataID == id && !zombie.mDead && !zombie.IsDeadOrDying())
            zombie.TakeDamage(maximum, DamageFlags.BypassesShield);
        if (zombie.DataID == id && zombie.mBoard?.Pointer == board.Pointer
            && zombie.mBodyHealth <= 0 && !suppressSeed) TingleKelp.Spawn(board, col, row);
        return true;
    }

    internal static void Tick(GridItem item)
    {
        var board = item.mBoard;
        if (board == null || !Playing(board)) return;
        var zombies = board.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
            if (zombies[i] is { mDead: false } zombie && zombie.DataID == unchecked((uint)item.mGridX)
                && !zombie.IsDeadOrDying())
            {
                if (item.mGridItemCounter > 0
                    && FusionTime.SourceStep(board, typeof(KelpEffects), item.Pointer, item.DataID)) --item.mGridItemCounter;
                return;
            }
        item.GridItemDie();
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class KelpEffectTickPatch
{
    private static bool Prefix(GridItem __instance)
    {
        if (!KelpEffects.IsEffect(__instance)) return true;
        KelpEffects.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class KelpEffectDrawPatch
{
    private static bool Prefix(GridItem __instance) => !KelpEffects.IsEffect(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateZombieWalking))]
internal static class KelpWalkingPatch
{
    // Walking can be driven by native velocity or controller root motion.
    // Scale its actual displacement, preserving chill and every native guard.
    private static void Prefix(Zombie __instance, out (uint Id, float X, float Y, bool Slow) __state) =>
        __state = (__instance.DataID, __instance.mPosX, __instance.mPosY,
            KelpEffects.Playing(__instance.mBoard) && KelpEffects.Slowed(__instance));
    private static void Postfix(Zombie __instance, (uint Id, float X, float Y, bool Slow) __state)
    {
        if (!__state.Slow || __instance.DataID != __state.Id || __instance.mDead) return;
        __instance.mPosX = (__state.X + __instance.mPosX) * .5f;
        __instance.mPosY = (__state.Y + __instance.mPosY) * .5f;
        __instance.mX = (int)__instance.mPosX;
        __instance.mY = (int)__instance.mPosY;
    }
}
