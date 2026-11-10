using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Native plants own health, death, placement and save fields. These callbacks
// add the Fusion-only effects without changing native plant identities.
internal static class CherryBombPlants
{
    internal sealed record Death(Board Board, FusionPlant Identity, uint Id,
        int Col, int Row, float X, float Y, int Order, int Stage = 0);
    [ThreadStatic] internal static IntPtr DisposingBoard;

    internal static bool CanTriggerDeath(Plant plant)
    {
        var board = plant.mBoard;
        return board != null && board.Pointer != DisposingBoard
            && PlayerPlantingScope.ConsumingIngredient != plant.Pointer
            && !plant.mDead && plant.mIsOnBoard
            && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee
            && board.mApp != null && board.mApp.GameScene == GameScenes.Playing
            && board.mApp.GameMode != GameMode.ChallengeZenGarden;
    }

    internal static void ExplodeFusion(Plant fused)
    {
        var board = fused.mBoard;
        float x = fused.mX + fused.mWidth / 2f, y = fused.mY + fused.mHeight / 2f;
        // Match native Cherry Bomb DoSpecial: radius 115, row range 1,
        // burning damage and all seven native target categories (0x7f).
        // Use the destination even when the glove carries the Cherry Bomb.
        HealNuts(board, x, y, fused.mRow, 115);
        board.KillAllZombiesInRadius(fused.mRow, x, y, 115, 1, true,
            DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
            | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Dying | DamageRangeFlags.Underground);
        board.mApp.AddTodParticle(x, y, 400000, ParticleEffect.Powie);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Cherrybomb);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Explosion);
        board.ShakeBoard(3, -4);
    }

    internal static void HealNuts(Board board, float x, float y, int row, float radius)
    {
        ReactivePots.Cherry(board, x, y, row, radius);
        var plants = board.m_plants;
        for (int i = 0; i < plants.Count; ++i)
        {
            var plant = plants[i];
            if (plant == null || plant.mDead || !plant.mIsOnBoard || plant.mSquished
                || plant.mOnBungeeState != PlantOnBungeeState.NotOnBungee || plant.NotOnGround()
                || FusionPlant.From(plant) != FusionPlant.CherryNut || Math.Abs(plant.mRow - row) > 1) continue;
            float dx = plant.mX + plant.mWidth / 2f - x;
            float dy = plant.mY + plant.mHeight / 2f - y;
            if (dx * dx + dy * dy <= radius * radius)
                plant.mPlantHealth = (int)Math.Min(plant.mPlantMaxHealth, (long)plant.mPlantHealth + 200);
        }
    }

    internal static int Explode(Death death, int freezeTicks = 0, float? sourceRadius = null, int damage = 1800)
    {
        var board = death.Board;
        bool mine = death.Identity == FusionPlant.ClusterMine;
        bool solar = death.Identity == FusionPlant.SolarBomb;
        var damageRange = mine
            ? DamageRangeFlags.Ground | DamageRangeFlags.Submerged | DamageRangeFlags.Dog | DamageRangeFlags.Underground
            : DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
                | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Dying | DamageRangeFlags.Underground;
        // Source cherry radius 2 / potato radius 1; one board cell is 80 pixels.
        float radius = sourceRadius ?? (mine ? 80 : 160);
        if (!mine) HealNuts(board, death.X, death.Y, death.Row, radius);
        else ReactivePots.Cherry(board, death.X, death.Y, death.Row, radius);
        // Snapshot references and IDs before damage callbacks can spawn/recycle
        // zombies. A shield is not an extra zombie and earns no extra sun.
        var targets = new List<(Zombie Zombie, uint Id)>();
        var zombies = board.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            var zombie = zombies[i];
            if (zombie == null || zombie.mDead || zombie.IsDeadOrDying()
                || Math.Abs(zombie.mRow - death.Row) > (mine ? 0 : 1)
                || !zombie.EffectedByDamage(damageRange)) continue;
            var rect = zombie.GetZombieRect();
            float dx = death.X - Math.Clamp(death.X, rect.xMin, rect.xMax);
            float dy = death.Y - Math.Clamp(death.Y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= radius * radius) targets.Add((zombie, zombie.DataID));
        }
        int hit = 0;
        foreach (var (zombie, id) in targets)
        {
            if (zombie.mDead || zombie.DataID != id || zombie.mBoard?.Pointer != board.Pointer
                || zombie.IsDeadOrDying()) continue;
            zombie.TakeDamage(damage, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
            ++hit;
            if (freezeTicks > 0 && zombie.DataID == id && !zombie.mDead && !zombie.IsDeadOrDying())
                IceInstantFusions.Freeze(zombie, freezeTicks);
            // Per damaged zombie, including survivors. Native radius helper
            // returns kills, so its return value cannot implement this rule.
            if (solar) board.AddCoin(death.X, death.Y, CoinType.Sun, CoinMotion.FromPlant);
        }
        board.mApp.AddTodParticle(death.X, death.Y, death.Order, ParticleEffect.Powie);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Cherrybomb);
        if (mine) MineFusions.ScatterMines(death);
        return hit;
    }


}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class CherryBombPlantInitializePatch
{
    private static void Postfix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (identity == FusionPlant.CherryNut)
            __instance.mPlantHealth = __instance.mPlantMaxHealth = 4000;
        else if (identity == FusionPlant.ClusterMine)
        {
            // CherryMine derives directly from source Plant, not PotatoMine.
            // It is an edible low plant; only its death explodes and scatters.
            __instance.mState = PlantState.Notready;
            __instance.mStateCountdown = 0;
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdatePotato))]
internal static class ClusterMineAbilityPatch
{
    private static bool Prefix(Plant __instance) => FusionPlant.From(__instance) != FusionPlant.ClusterMine;
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class CherryBombSpecialPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (__instance.mDead) return true;
        if (FusionPlant.From(__instance) == FusionPlant.SolarBomb)
        {
            // The native Cherry Bomb's one-second saved fuse calls DoSpecial.
            // Its death callback also covers destruction before the fuse ends.
            __instance.Die();
            return false;
        }
        if (__instance.mSeedType == SeedType.Cherrybomb && __instance.mBoard != null)
            CherryBombPlants.HealNuts(__instance.mBoard, __instance.mX + __instance.mWidth / 2f,
                __instance.mY + __instance.mHeight / 2f, __instance.mRow, 115);
        return true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class CherryBombPlantDeathPatch
{
    // Run after the deferred-ingredient decision, and capture before native
    // death clears on-board flags. Execute effects only after successful death.
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state)
    {
        __state = null;
        var board = __instance.mBoard;
        if (!__runOriginal || !CherryBombPlants.CanTriggerDeath(__instance) || __instance.NotOnGround()) return;
        var identity = FusionPlant.From(__instance);
        if (!FusionPlant.IsSolarNut(identity) && identity != FusionPlant.ChompMine && !FusionPlant.IsMine(identity) && identity != FusionPlant.CherryNut && identity != FusionPlant.ClusterMine && identity != FusionPlant.SolarBomb) return;
        __state = new(board, identity, __instance.DataID, __instance.mPlantCol, __instance.mRow,
            __instance.mX + __instance.mWidth / 2f, __instance.mY + __instance.mHeight / 2f, __instance.mRenderOrder, __instance.mStartRow);
    }

    private static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id)
        {
            if (FusionPlant.IsSolarNut(__state.Identity) || __state.Identity == FusionPlant.ChompMine) SolarChomperPlants.OnDeath(__state);
            else if (FusionPlant.IsMine(__state.Identity)) MineFusions.OnDeath(__state);
            else CherryBombPlants.Explode(__state);
        }
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class CherryBombBoardDisposeScope
{
    private static void Prefix(Board __instance, out IntPtr __state)
    {
        __state = CherryBombPlants.DisposingBoard;
        CherryBombPlants.DisposingBoard = __instance.Pointer;
    }
    private static void Finalizer(IntPtr __state) => CherryBombPlants.DisposingBoard = __state;
}
