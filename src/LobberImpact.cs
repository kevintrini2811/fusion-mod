using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Shared original Thrower hit rules. Snapshot pool handles before callbacks;
// direct damage, splash budget, butter and money remain explicit inputs.
internal static class LobberImpact
{
    internal const float SplashRadius = 1.5f * ScaredyFusions.SourceUnit;
    internal static List<(Zombie Zombie, uint Id, bool Direct)> Targets(Projectile shot, Zombie? direct, bool melon)
    {
        var board = shot.mBoard;
        float x = shot.mPosX, y = shot.mPosY;
        int row = shot.mRow;
        var targets = new List<(Zombie Zombie, uint Id, bool Direct)>();
        if (direct != null && SquashFusions.Eligible(board, direct, DamageRangeFlags.Ground))
            targets.Add((direct, direct.DataID, true));
        if (melon)
            for (int i = 0; i < board.m_zombies.Count; ++i)
            {
                var zombie = board.m_zombies[i];
                if (zombie == null || zombie.Pointer == direct?.Pointer || Math.Abs(zombie.mRow - row) > 1
                    || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
                    || !SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground)) continue;
                var rect = zombie.GetZombieRect();
                float dx = x - Math.Clamp(x, rect.xMin, rect.xMax);
                float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
                if (dx * dx + dy * dy <= SplashRadius * SplashRadius)
                    targets.Add((zombie, zombie.DataID, false));
            }
        return targets;
    }

    internal static void Apply(Projectile shot, Zombie? direct, int damage,
        bool melon, bool butter, int money, bool boostedSplash)
    {
        var board = shot.mBoard;
        if (shot.mDead || board == null) return;
        uint shotId = shot.DataID;
        float x = shot.mPosX, y = shot.mPosY;
        var presentation = PlantLibrary.ProjectileImpact.Capture(shot);
        var targets = Targets(shot, direct, melon);
        shot.Die();
        if (!shot.mDead || shot.DataID != shotId || shot.mBoard?.Pointer != board.Pointer) return;
        int splash = boostedSplash ? damage * 2 / 3
            : (int)Math.Min(damage / 3, Math.Max(1, damage * 8L / Math.Max(1, targets.Count)));
        if (money > 0) SilverMoney.DropMoney(board, x, y, money);
        foreach (var (zombie, id, primary) in targets)
            if (zombie.DataID == id && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground))
            {
                zombie.TakeDamage(primary ? damage : splash, DamageFlags.BypassesShield);
                if (butter && zombie.DataID == id && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground))
                    zombie.ApplyButter();
            }
        PlantLibrary.Projectiles.EmitImpactEffect("fusion:" + (melon ? ParticleEffect.MelonSplash : butter ? ParticleEffect.ButterSplat : ParticleEffect.CabbageSplat), presentation);
    }
}
