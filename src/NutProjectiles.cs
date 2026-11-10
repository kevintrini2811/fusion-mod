using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Projectile markers and damage serialize natively. Rolling target history uses
// native hidden GridItems, bound to the complete projectile and zombie DataIDs.
internal static class NutProjectiles
{
    internal const float WallopMarker = -550171;
    private const int Cabbage = -530100, Melon = -530110, RollMarker = -530120, HitMarker = -31000;
    internal static int Family(Projectile shot) => shot.mProjectileType == ProjectileType.Cabbage && shot.mCobTargetX == Cabbage ? 1
        : shot.mProjectileType == ProjectileType.Melon && shot.mCobTargetX == Melon ? 2
        : shot.mProjectileType == ProjectileType.Pea && shot.mCobTargetX is <= RollMarker and >= RollMarker - 7 ? 3 : 0;
    internal static bool Lob(Plant plant, Zombie? target, int damage)
    {
        bool melon = FusionPlant.From(plant) == FusionPlant.MelonNut;
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f - 10, plant.mRenderOrder + 1, plant.mRow,
            melon ? ProjectileType.Melon : ProjectileType.Cabbage);
        if (shot == null || shot.mDead) return false;
        shot.mCobTargetX = melon ? Melon : Cabbage;
        shot.mCobTargetRow = damage;
        LobberProjectiles.Launch(shot, target, PuffLobbers.FlightTicks);
        ImportedPeaVisuals.EnsureNutProjectile(shot);
        return true;
    }
    internal static void Roll(Plant plant)
    {
        int stage = Math.Clamp(plant.mTargetX, 0, 2);
        var shot = plant.mBoard.AddProjectile(plant.mX + 55, plant.mY + plant.mHeight - 10,
            plant.mRenderOrder + 1, plant.mRow, ProjectileType.Pea);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = RollMarker - (3 + stage * 2);
        shot.mCobTargetRow = 80 - stage * 20;
        shot.mMotionType = ProjectileMotion.Straight;
        shot.mDamageRangeFlags = DamageRangeFlags.Ground;
        shot.mVelX = 2.5f;
        shot.mPosZ = -(80 - stage * 25);
        shot.mVelY = shot.mVelZ = 0;
        shot.mAccZ = .1f;
        shot.mHitTorchwoodGridX = 0;
        ImportedPeaVisuals.EnsureNutProjectile(shot);
    }
    internal static bool IsHistory(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == HitMarker;
    internal static bool Remember(Projectile shot, Zombie zombie, int ticks = 0)
    {
        var items = shot.mBoard.m_gridItems;
        for (int i = 0; i < items.Count; ++i)
            if (items[i] is { mDead: false } entry && IsHistory(entry)
                && entry.mGridX == unchecked((int)shot.DataID) && entry.mGridY == unchecked((int)zombie.DataID)) return false;
        var item = items.DataArrayAlloc();
        if (item == null) { shot.Die(); return false; }
        try
        {
            item.Init(shot.mBoard.mApp, GridItemType.ZenTool);
            item.mGridItemState = GridItemState.Normal;
            item.mSunCount = HitMarker;
            item.mGridX = unchecked((int)shot.DataID);
            item.mGridY = unchecked((int)zombie.DataID);
            item.mGridItemCounter = ticks;
            item.mX = item.mY = item.mPosX = item.mPosY = -1000;
            return true;
        }
        catch { item.GridItemDie(); shot.Die(); throw; }
    }
    internal static void TickHistory(GridItem item)
    {
        var board = item.mBoard;
        if (board == null || board.mPaused) return;
        if (item.mGridItemCounter > 0
            && FusionTime.SourceStep(board, typeof(NutProjectiles), item.Pointer, item.DataID)
            && --item.mGridItemCounter == 0)
        { item.GridItemDie(); return; }
        for (int i = 0; i < board.m_projectiles.Count; ++i)
            if (board.m_projectiles[i] is { mDead: false } shot
                && shot.DataID == unchecked((uint)item.mGridX)) return;
        item.GridItemDie();
    }
    internal static void Move(Projectile shot)
    {
        if (shot.mDead || shot.mBoard.mPaused) return;
        if (!FusionTime.SourceStep(shot, typeof(NutProjectiles))) return;
        var board = shot.mBoard;
        float oldX = shot.mPosX, x = oldX + shot.mVelX;
        float slope = board.GetPosYBasedOnRow(x, shot.mRow) - board.GetPosYBasedOnRow(oldX, shot.mRow);
        if (x > 900 || x < -80 || slope < -.01f && shot.mPosZ >= 0) { shot.Die(); return; }
        shot.mPosX = x;
        shot.mPosY += slope;
        shot.mVelZ += shot.mAccZ;
        shot.mPosZ = Math.Min(0, shot.mPosZ + shot.mVelZ);
        if (shot.mPosZ == 0) { shot.mVelZ = shot.mAccZ = 0; shot.mVelX = 1.25f; }
        shot.mShadowY = shot.mPosY + 10;
        shot.mRotation += .12f;
        uint id = shot.DataID;
        var targets = new List<(Zombie Zombie, uint Id, float X)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != shot.mRow
                || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            if (rect.xMax >= oldX - 12 && rect.xMin <= x + 12)
                targets.Add((zombie, zombie.DataID, rect.xMin));
        }
        foreach (var (zombie, targetId, _) in targets.OrderBy(target => target.X))
        {
            if (shot.mDead || shot.DataID != id || shot.mBoard?.Pointer != board.Pointer) return;
            if (zombie.DataID != targetId || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)
                || !Remember(shot, zombie)) continue;
            int remaining = (int)(RollMarker - shot.mCobTargetX);
            shot.mCobTargetX = RollMarker - (remaining - 1);
            zombie.TakeDamage(shot.mCobTargetRow, DamageFlags.BypassesShield);
            if (shot.DataID != id || shot.mBoard?.Pointer != board.Pointer) return;
            if (remaining <= 1) { shot.Die(); return; }
        }
    }
    internal static void Impact(Projectile shot, Zombie? zombie) =>
        LobberImpact.Apply(shot, zombie, Math.Max(0, shot.mCobTargetRow), Family(shot) == 2, false, 0, false);
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class NutLobImpactPatch
{
    internal static bool Prefix(Projectile __instance, Zombie theZombie)
    {
        int family = NutProjectiles.Family(__instance);
        if (family is 1 or 2) { NutProjectiles.Impact(__instance, theZombie); return false; }
        return family != 3;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class StackNutMotionPatch
{
    internal static bool Prefix(Projectile __instance)
    {
        if (NutProjectiles.Family(__instance) != 3) return true;
        NutProjectiles.Move(__instance); return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class StackNutCollisionPatch
{
    internal static bool Prefix(Projectile __instance) => NutProjectiles.Family(__instance) != 3;
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class StackNutTorchPatch
{
    internal static bool Prefix(Projectile __instance) => NutProjectiles.Family(__instance) != 3;
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class NutProjectileRestorePatch
{
    internal static void Postfix(Projectile __instance)
    { if (!__instance.mDead && NutProjectiles.Family(__instance) != 0) ImportedPeaVisuals.EnsureNutProjectile(__instance); }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class NutProjectileHistoryPatch
{
    internal static bool Prefix(GridItem __instance)
    {
        if (!NutProjectiles.IsHistory(__instance)) return true;
        NutProjectiles.TickHistory(__instance); return false;
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class NutProjectileHistoryDrawPatch
{
    internal static bool Prefix(GridItem __instance) => !NutProjectiles.IsHistory(__instance);
}
