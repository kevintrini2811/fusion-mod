using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// ThreePeater.Shoot1 emits at the middle head's .5-second animation event.
// Edge rows redirect the missing bullet into the own row after .2 seconds.
// mStartRow and mLaunchCounter are native saved/reset fields. Native Fire only
// reads this plant's target fields; its +0xd4 writer belongs to a projectile.
internal static class ThreepeaterFusions
{
    [ThreadStatic] internal static Plant? Firing;
    [ThreadStatic] internal static Projectile? Created;
    // Pea/Fireball never use the native saved Cob target coordinate. Native
    // initialization and Reset clear it; conversion leaves it intact.
    private const float ScorchedMarker = -40040;
    internal sealed record Impact(Projectile Projectile, Zombie Zombie, IntPtr Board, uint Id);
    [ThreadStatic] internal static Impact? CurrentImpact;
    [ThreadStatic] internal static bool Pending;

    internal static bool IsScorchedShot(Projectile projectile) => projectile.mCobTargetX == ScorchedMarker
        && projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
            or ProjectileType.Fireball or ProjectileType.PeashooterFireball;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsThreepeater(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mStartRow = 0;
        plant.mLaunchRate = identity == FusionPlant.SnowThreepeater ? 100 : 150;
        plant.mLaunchCounter = 0;
        if (identity == FusionPlant.Multipeater) plant.mTargetX = plant.mTargetY = 0;
    }

    internal static int[] Rows(Plant plant, bool requireTarget = false)
    {
        var board = plant.mBoard;
        var identity = FusionPlant.From(plant);
        int mode = plant.mStartRow > 0 ? plant.mTargetX : plant.mTargetY;
        var nearest = new Dictionary<int, float>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                || !SquashFusions.Eligible(board, zombie, plant.GetDamageRangeFlags(PlantWeapon.Primary))) continue;
            float x = zombie.GetZombieRect().xMin;
            if (x <= plant.mX || x > board.GridToPixelX(8, zombie.mRow) + 160) continue;
            if (identity == FusionPlant.Multipeater)
            {
                if (mode == 2 && zombie.mRow != plant.mRow
                    || mode == 0 && Math.Abs(zombie.mRow - plant.mRow) > 1) continue;
            }
            else if (identity != FusionPlant.SnowThreepeater && Math.Abs(zombie.mRow - plant.mRow) > 1) continue;
            if (!nearest.TryGetValue(zombie.mRow, out float previous) || x < previous) nearest[zombie.mRow] = x;
        }
        if (nearest.Count == 0 && requireTarget) return Array.Empty<int>();
        if (identity == FusionPlant.Multipeater && mode != 0)
            return mode == 2 ? new[] { plant.mRow } : Enumerable.Range(0, board.GetNumRows()).ToArray();
        if (identity != FusionPlant.SnowThreepeater)
            return Enumerable.Range(Math.Max(0, plant.mRow - 1),
                Math.Min(board.GetNumRows() - 1, plant.mRow + 1) - Math.Max(0, plant.mRow - 1) + 1).ToArray();
        // IceThreePeater.Shoot1 orders each row's leftmost enemy, then fills
        // remaining slots with own / previous / next row, without duplicates.
        var rows = nearest.OrderBy(p => p.Value).ThenBy(p => p.Key).Take(3).Select(p => p.Key).ToList();
        foreach (int row in new[] { plant.mRow, plant.mRow - 1, plant.mRow + 1 })
            if (rows.Count < 3 && row >= 0 && row < board.GetNumRows() && !rows.Contains(row)) rows.Add(row);
        return rows.ToArray();
    }

    internal static void Tick(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsThreepeater(identity) || identity == FusionPlant.CharredThreepeater
            || !ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        if (!FusionTime.SourceStep(plant, typeof(ThreepeaterFusions))) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0) return;
            if (Rows(plant, requireTarget: true).Length == 0)
            {
                if (identity == FusionPlant.WeedSpreader) plant.mLaunchCounter = plant.mLaunchRate + plant.mBoard.Rand(21) - 10;
                return;
            }
            if (identity == FusionPlant.Multipeater) plant.mTargetX = plant.mTargetY;
            plant.mStartRow = identity == FusionPlant.Multipeater ? MultiDuration(plant) : 100;
            plant.mLaunchCounter = plant.mLaunchRate + (identity == FusionPlant.WeedSpreader ? plant.mBoard.Rand(21) - 10 : 0);
        }
        if (identity == FusionPlant.Multipeater)
        {
            int time = MultiDuration(plant) - plant.mStartRow;
            // Single-row mode emits six distinct peas, not two stacks of three.
            bool fire = plant.mTargetX switch
            {
                1 => time == 30,
                2 => time is 42 or 50 or 58 or 67 or 75 or 83,
                _ => time is 42 or 67
            };
            if (fire)
            {
                foreach (int row in Rows(plant))
                    Shoot(plant, row);
            }
            // Normal mode returns missing edge-row bullets to the own row.
            if (plant.mTargetX == 0 && time is 62 or 87
                && (plant.mRow == 0 || plant.mRow == plant.mBoard.GetNumRows() - 1)) Shoot(plant, plant.mRow);
            --plant.mStartRow;
            return;
        }
        int elapsed = 100 - plant.mStartRow;
        if (elapsed == 50)
            foreach (int row in Rows(plant)) Shoot(plant, row);
        if (elapsed == 70 && identity != FusionPlant.SnowThreepeater
            && (plant.mRow == 0 || plant.mRow == plant.mBoard.GetNumRows() - 1)) Shoot(plant, plant.mRow);
        --plant.mStartRow;
    }

    internal static Projectile? Shoot(Plant plant, int row)
    {
        if (FusionPlant.From(plant) == FusionPlant.WeedSpreader)
        {
            // ThreeSpike suppresses only the two outer shots into water.
            // Missing board-edge shots still return to the own row after .2s.
            if (row != plant.mRow && plant.mBoard.IsPoolSquare(plant.mPlantCol, row)) return null;
            return RollingProjectiles.SpawnWeed(plant, row);
        }
        var old = Firing;
        var oldCreated = Created;
        Firing = plant;
        Created = null;
        try
        {
            // Reuse native Fire for projectile initialization, row collision,
            // normal/Torchwood motion, sound and source-compatible pea rules.
            ShootingPots.FireShot(plant, row);
            if (Created is not { } projectile || projectile.mDead) return null;
            if ((FusionPlant.From(plant) == FusionPlant.SnowThreepeater
                || FusionPlant.From(plant) == FusionPlant.Multipeater) && row != plant.mRow)
            {
                // Native Threepeater motion decays vertical velocity by .97
                // each tick. Scale its trajectory for arbitrarily distant rows.
                float adjacentY = plant.mBoard.GridToPixelY(plant.mPlantCol,
                    plant.mRow + Math.Sign(row - plant.mRow));
                float originY = plant.mBoard.GridToPixelY(plant.mPlantCol, plant.mRow);
                float targetY = plant.mBoard.GridToPixelY(plant.mPlantCol, row);
                float factor = (targetY - originY) / (adjacentY - originY);
                projectile.mVelY *= factor;
                // Native Fire offsets the shadow against the curved flight.
                projectile.mShadowY -= (targetY - originY) - (adjacentY - originY);
            }
            if (FusionPlant.From(plant) == FusionPlant.PhoenixThreepeater)
                PhoenixProjectiles.Mark(projectile);
            else if (FusionPlant.From(plant) == FusionPlant.ScorchedThreepeater)
            {
                projectile.mCobTargetX = ScorchedMarker;
                ImportedPeaVisuals.RegisterScorchedProjectile(projectile);
            }
            else if (FusionPlant.From(plant) == FusionPlant.CherrySpreader)
                CherrySpreader.Track(projectile);
            else if (FusionPlant.From(plant) == FusionPlant.Multipeater && plant.mTargetX == 1)
                projectile.mCobTargetX = -40100 - plant.mBoard.GetNumRows();
            return projectile;
        }
        finally { Firing = old; Created = oldCreated; }
    }

    internal static int MultiDuration(Plant plant) => plant.mTargetX == 1 ? 50 : 100;

    internal static bool ClickMulti(Plant plant, int clicks, int player)
    {
        if (FusionPlant.From(plant) != FusionPlant.Multipeater) return false;
        if (clicks >= 0 && player == 0 && ChomperFusions.Active(plant)
            && plant.mBoard.mCursorObject.CursorType == CursorType.Normal
            && LawnGlove.LawnGloveMod.GetCarriedPlant(plant.mBoard) == null)
            plant.mTargetY = (Math.Clamp(plant.mTargetY, 0, 2) + 1) % 3;
        return true;
    }

    internal static int MultiDamage(Projectile shot, int damage)
    {
        int rows = (int)(-shot.mCobTargetX - 40100);
        if (damage <= 0 || rows is < 1 or > 6 || shot.mCobTargetX != -40100 - rows) return damage;
        // Distribute missing coverage bullets. Torchwood adds its own 20 HP.
        int bonus = 120 / rows - 20;
        bool fire = shot.mProjectileType is ProjectileType.Fireball or ProjectileType.PeashooterFireball;
        return fire && damage < 20 ? (damage * 3 + bonus + 1) / 3 : damage + bonus;
    }

    internal static bool IsMultiShot(Projectile shot) => shot.mCobTargetX <= -40101 && shot.mCobTargetX >= -40106
        && shot.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
            or ProjectileType.Fireball or ProjectileType.PeashooterFireball;

    internal static void OnFusion(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.PhoenixThreepeater)
        { PhoenixProjectiles.BurnBoard(plant.mBoard, plant.mX, plant.mY, plant.mRenderOrder); return; }
        if (FusionPlant.From(plant) != FusionPlant.ScorchedThreepeater) return;
        // CreatePlant.UniqueEvent: 1800 ash damage in own and adjacent rows.
        // Invoke only after successful ingredient consumption, never on load.
        for (int row = Math.Max(0, plant.mRow - 1); row <= Math.Min(plant.mBoard.GetNumRows() - 1, plant.mRow + 1); ++row)
        {
            ElementalNuts.Heal(plant.mBoard, true, row);
            DoomEffects.Damage(plant.mBoard, row, plant.mX, plant.mBoard.GridToPixelY(plant.mPlantCol, row),
                plant.mRenderOrder, fire: true);
        }
    }


}

[HarmonyPatch(typeof(Plant), nameof(Plant.MouseDown))]
internal static class MultipeaterClickPatch
{
    private static bool Prefix(Plant __instance, int theClickCount, int playerIndex) =>
        !ThreepeaterFusions.ClickMulti(__instance, theClickCount, playerIndex);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class ThreepeaterInitializePatch
{
    internal static void Postfix(Plant __instance) => ThreepeaterFusions.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooter))]
internal static class ThreepeaterTickPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsThreepeater(FusionPlant.From(__instance))) return true;
        ThreepeaterFusions.Tick(__instance);
        return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class ThreepeaterNativeFirePatch
{
    internal static bool Prefix(Plant __instance) => !FusionPlant.IsThreepeater(FusionPlant.From(__instance))
        || ThreepeaterFusions.Firing?.Pointer == __instance.Pointer;
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ProjectileInitialize))]
internal static class ThreepeaterProjectilePatch
{
    internal static void Postfix(Projectile __instance)
    {
        if (ThreepeaterFusions.Firing is { } plant && plant.mBoard?.Pointer == __instance.mBoard?.Pointer)
            ThreepeaterFusions.Created = __instance;
    }
}
[HarmonyPatch]
internal static class ScorchedImpactPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.DoImpact)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.DoSplashDamage)) };
    internal static void Prefix(Projectile __instance, Zombie theZombie,
        out (ThreepeaterFusions.Impact? Impact, bool Pending) __state)
    {
        __state = (ThreepeaterFusions.CurrentImpact, ThreepeaterFusions.Pending);
        ThreepeaterFusions.CurrentImpact = (ThreepeaterFusions.IsScorchedShot(__instance)
            || ThreepeaterFusions.IsMultiShot(__instance)) && !__instance.mDead
            && theZombie != null && !theZombie.mDead && !theZombie.IsDeadOrDying()
            && theZombie.mBoard?.Pointer == __instance.mBoard?.Pointer
            ? new(__instance, theZombie, __instance.mBoard!.Pointer, theZombie.DataID) : null;
        ThreepeaterFusions.Pending = ThreepeaterFusions.CurrentImpact != null;
    }
    internal static void Finalizer((ThreepeaterFusions.Impact? Impact, bool Pending) __state)
    { ThreepeaterFusions.CurrentImpact = __state.Impact; ThreepeaterFusions.Pending = __state.Pending; }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.GetDamageFlags))]
internal static class ScorchedDamageTargetPatch
{
    internal static void Postfix(Projectile __instance, Zombie theZombie)
    {
        // Both native direct and splash paths call this immediately before
        // each actual TakeDamage. Arm one write for that exact pool occupant.
        if (ThreepeaterFusions.CurrentImpact?.Projectile.Pointer != __instance.Pointer
            || theZombie == null || theZombie.mBoard?.Pointer != __instance.mBoard?.Pointer) return;
        ThreepeaterFusions.CurrentImpact = new(__instance, theZombie, __instance.mBoard!.Pointer, theZombie.DataID);
        ThreepeaterFusions.Pending = true;
    }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class ScorchedDamagePatch
{
    [HarmonyPriority(Priority.First)]
    internal static void Prefix(Zombie __instance, ref int theDamage)
    {
        var impact = ThreepeaterFusions.CurrentImpact;
        if (!ThreepeaterFusions.Pending || impact == null || impact.Zombie.Pointer != __instance.Pointer
            || impact.Id != __instance.DataID || impact.Board != __instance.mBoard?.Pointer) return;
        ThreepeaterFusions.Pending = false;
        if (ThreepeaterFusions.IsMultiShot(impact.Projectile))
        { theDamage = ThreepeaterFusions.MultiDamage(impact.Projectile, theDamage); return; }
        // Source TorchWood adds 20 (FirePeas), rather than multiplying the
        // already strengthened 40-damage pea. Native fire splash is one third.
        theDamage = impact.Projectile.mProjectileType is ProjectileType.Fireball or ProjectileType.PeashooterFireball
            ? checked((theDamage * 3 + 1) / 2) : checked(theDamage * 2);
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class ScorchedProjectileVisualPatch
{
    // Restore the imported pea after loading. Converted fireballs retain their
    // native fire rendering while preserving the saved damage multiplier.
    internal static void Postfix(Projectile __instance)
    {
        if (!__instance.mDead && ThreepeaterFusions.IsScorchedShot(__instance)
            && __instance.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea)
            ImportedPeaVisuals.EnsureScorchedProjectile(__instance);
    }
}
