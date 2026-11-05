using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using HarmonyLib;

namespace FusionMod;

// BoardAction.SetDoom: radius five source world units, 1800 ash damage. SetPit destroys the
// destination stack and leaves a 180-second crater, rather than a 3x3 blast
// against plants. Reuse the existing shared radius damage for Doom Kelp.
internal static class DoomEffects
{
    [ThreadStatic] internal static (Zombie Zombie, uint Id)? Burning;
    internal static void AshDamage(Zombie zombie, int damage,
        DamageFlags flags = DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody)
    {
        var previous = Burning;
        Burning = (zombie, zombie.DataID);
        try { zombie.TakeDamage(damage, flags); }
        finally { Burning = previous; }
    }

    internal static void Damage(Board board, int row, float x, float y, int order, bool fire = false,
        int damage = 1800, bool global = false, int freezeTicks = 0, float radius = 5f / 1.35f * 80)
    {
        if (fire) { ReactivePots.Fire(board, row); PumpkinSupport.FireEnergy(board, row, damage); }
        var range = DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
            | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Underground;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || !SquashFusions.Eligible(board, zombie, range)) continue;
            if (fire)
            {
                if (zombie.mRow != row) continue;
            }
            else if (!global)
            {
                // Source GetColumnFromX divides by 1.35 world units per cell.
                // DoomKelp passes no crater and no plant destruction.
                var rect = zombie.GetZombieRect();
                float dx = x - Math.Clamp(x, rect.xMin, rect.xMax);
                float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
                if (dx * dx + dy * dy > radius * radius) continue;
            }
            targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !SquashFusions.Eligible(board, zombie, range)) continue;
            if (fire) zombie.RemoveColdEffects();
            if (freezeTicks > 0) IceInstantFusions.Freeze(zombie, freezeTicks);
            AshDamage(zombie, damage);
        }
        if (fire) NativeCombatVisuals.Fire(board, row, order);
        else board.mApp.AddTodParticle(x, y, order, ParticleEffect.Doom);
        board.mApp.SoundSystem.PlayFoley(fire ? FoleyType.JalapenoIgnite : FoleyType.Explosion);
        board.ShakeBoard(3, -4);
    }

    internal static void Explode(Board board, int col, int row, float x, float y, int order)
    {
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()
            || board.Pointer == CherryBombPlants.DisposingBoard) return;
        Damage(board, row, x, y, order);
        LeaveCrater(board, col, row);
    }

    internal static GridItem? LeaveCrater(Board board, int col, int row)
    {
        if (ReactivePots.PreventCrater(board, col, row)) return null;
        var victims = new List<(Plant Plant, uint Id)>();
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant != null && FusionPlant.From(plant) != FusionPlant.DoomNut
                && !plant.mDead && plant.mIsOnBoard && plant.mPlantCol == col
                && plant.mRow == row && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee)
                victims.Add((plant, plant.DataID));
        }
        foreach (var (plant, id) in victims)
            if (plant.DataID == id && plant.mBoard?.Pointer == board.Pointer && !plant.mDead) plant.Die();
        // Native crater owns terrain-specific presentation, blocking and saves.
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { } existing && !existing.mDead
                && existing.mGridItemType == GridItemType.Crater && existing.mGridX == col && existing.mGridY == row)
            { existing.mGridItemCounter = 18000; return existing; }
        var crater = board.AddACrater(col, row);
        if (crater != null) crater.mGridItemCounter = 18000;
        return crater;
    }
}

