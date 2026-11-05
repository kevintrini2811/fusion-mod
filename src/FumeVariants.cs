using HarmonyLib;
using Il2CppReloaded.Gameplay;
using LawnGlove;

namespace FusionMod;

// FumeFusions owns initialization and shooter dispatch. These variants own
// their saved ammunition/energy and original action phases, not animation events.
internal static class FumeVariants
{
    internal const float Unit = 80f / 1.35f;
    internal static bool IsVariant(FusionPlant identity) => identity == FusionPlant.BoostShroom
        || identity == FusionPlant.BlowtorchShroom || identity == FusionPlant.WallopShroom
        || identity == FusionPlant.CaltropShroom;
    internal static bool Driver(Zombie zombie) => zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult;
    internal static bool Target(Plant plant, Zombie zombie, float range) => zombie.mRow == plant.mRow
        && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)
        && zombie.GetZombieRect().center.x > plant.mX + plant.mWidth / 2f
        && zombie.GetZombieRect().center.x < plant.mX + plant.mWidth / 2f + range;

    private static Plant? Ammunition(Plant plant)
    {
        for (int i = 0; i < plant.mBoard.m_plants.Count; ++i)
        {
            var nut = plant.mBoard.m_plants[i];
            if (nut != null && FusionPlant.From(nut) == FusionPlant.NutShroom
                && ChomperFusions.Active(nut) && Math.Abs(nut.mPlantCol - plant.mPlantCol) <= 1
                && Math.Abs(nut.mRow - plant.mRow) <= 1 && nut.mPlantHealth > 0
                && LawnGloveMod.GetCarriedPlant(plant.mBoard)?.Pointer != nut.Pointer) return nut;
        }
        return null;
    }

    internal static void Tick(Plant plant, FusionPlant identity)
    {
        if (!FumeFusions.Active(plant)) return;
        if (identity == FusionPlant.CaltropShroom && PopContact(plant)) return;
        if (identity == FusionPlant.BoostShroom) plant.mTargetY = (plant.mTargetY + 1) % 2;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (identity == FusionPlant.BlowtorchShroom) Absorb(plant);
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0) return;
            if (identity == FusionPlant.BlowtorchShroom)
            {
                if (plant.mTargetX <= 0) return;
            }
            else
            {
                bool target = false;
                for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
                    if (plant.mBoard.m_zombies[i] is { } zombie && Target(plant, zombie,
                        identity == FusionPlant.CaltropShroom ? 7 * Unit : float.MaxValue)) { target = true; break; }
                if (!target) return;
                if (identity == FusionPlant.WallopShroom && plant.mTargetX == 0)
                {
                    var nut = Ammunition(plant);
                    if (nut == null) return;
                    plant.mTargetX = nut.mPlantHealth;
                    PlayerPlantingScope.Consume(nut);
                }
            }
            plant.mStartRow = Duration(identity);
            plant.mLaunchCounter = identity == FusionPlant.WallopShroom ? 270 + plant.mBoard.Rand(61)
                : identity == FusionPlant.BlowtorchShroom ? 100 : 150;
        }
        int elapsed = Duration(identity) - plant.mStartRow;
        if (identity == FusionPlant.BoostShroom && elapsed >= 35 && elapsed < 132 && plant.mTargetY == 0)
        {
            FumeProjectiles.Boost(plant);
            if (elapsed > 35 && (elapsed - 35) % 20 == 0) FumeFusions.Smoke(plant, identity);
        }
        if (identity == FusionPlant.CaltropShroom && elapsed == 50) FumeFusions.Smoke(plant, identity);
        if (elapsed == Event(identity))
        {
            if (identity == FusionPlant.BoostShroom || identity == FusionPlant.BlowtorchShroom)
                FumeFusions.Smoke(plant, identity);
            if (identity == FusionPlant.WallopShroom)
            {
                if (FumeProjectiles.LaunchNut(plant, (int)Math.Min(int.MaxValue, (long)plant.mTargetX + 20))) plant.mTargetX = 0;
            }
            else if (identity != FusionPlant.BlowtorchShroom) Hit(plant, identity, identity == FusionPlant.BoostShroom ? 20 : 40);
        }
        if (identity == FusionPlant.BlowtorchShroom && elapsed == Event(identity) + 15)
        {
            int damage = plant.mTargetX;
            plant.mTargetX = 0;
            Hit(plant, identity, damage);
        }
        --plant.mStartRow;
    }

    // Source clip durations/event ticks are recorded in the shared port report.
    internal static int Duration(FusionPlant identity) => identity == FusionPlant.BoostShroom ? 149
        : identity == FusionPlant.WallopShroom ? 110 : identity == FusionPlant.CaltropShroom ? 117 : 100;
    internal static int Event(FusionPlant identity) => identity == FusionPlant.CaltropShroom ? 57
        : identity == FusionPlant.WallopShroom ? 90 : 35;

    internal static void Hit(Plant plant, FusionPlant identity, int damage)
    {
        var board = plant.mBoard;
        float range = identity == FusionPlant.BlowtorchShroom ? 7 * Unit
            : identity == FusionPlant.CaltropShroom ? 7 * Unit : float.MaxValue;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && Target(plant, zombie, range)) targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (!FumeFusions.Active(plant)) return;
            if (zombie.DataID != id || !Target(plant, zombie, range)) continue;
            int extra = identity == FusionPlant.CaltropShroom && Driver(zombie)
                ? (int)(.3d * (Math.Max(0, zombie.mBodyMaxHealth) + Math.Max(0, zombie.mHelmMaxHealth)
                    + (long)Math.Max(0, zombie.mShieldMaxHealth))) : 0;
            if (identity == FusionPlant.BlowtorchShroom) zombie.RemoveColdEffects();
            zombie.TakeDamage(damage, DamageFlags.BypassesShield);
            if (extra > 0 && zombie.DataID == id && !zombie.mDead && !zombie.IsDeadOrDying())
                zombie.TakeDamage(extra, DamageFlags.BypassesShield);
        }
    }

    internal static bool PopContact(Plant plant)
    {
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || !Driver(zombie) || zombie.mRow != plant.mRow
                || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)) continue;
            var rect = zombie.GetZombieRect();
            float x = plant.mX + plant.mWidth / 2;
            if (Math.Abs(x - Math.Clamp(x, rect.xMin, rect.xMax)) > Unit / 2) continue;
            // Reuse native tire-puncture damage, including its vehicle death path.
            zombie.TakeDamage(1800, DamageFlags.BypassesShield | DamageFlags.Spike);
            plant.Die();
            return true;
        }
        return false;
    }

    private static void Absorb(Plant plant)
    {
        var shots = plant.mBoard.m_projectiles;
        float x = plant.mX + plant.mWidth / 2;
        for (int i = 0; i < shots.Count; ++i)
        {
            var shot = shots[i];
            // TorchFume accepts source bullet IDs 0 and 15 only. Other custom
            // pea-shaped bullets retain their distinct source identity.
            if (shot == null || shot.mDead || shot.mRow != plant.mRow || shot.mCobTargetX != 0
                || shot.mHitTorchwoodGridX == plant.mPlantCol
                || shot.mProjectileType is not (ProjectileType.Pea or ProjectileType.PeashooterPea or ProjectileType.Snowpea)
                || Math.Abs(shot.mPosX - x) > Math.Max(8, FumeProjectiles.Speed(shot) + 4)) continue;
            if (shot.mProjectileType == ProjectileType.Snowpea) { shot.ConvertToPea(plant.mPlantCol); continue; }
            int damage = FumeProjectiles.Damage(shot, ShootingPots.ProjectileDamage(shot, 20));
            plant.mTargetX = (int)Math.Min(300, (long)plant.mTargetX + Math.Max(0, damage));
            shot.Die();
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class CaltropFumeAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.CaltropShroom) return true;
        FumeVariants.Tick(__instance, FusionPlant.CaltropShroom);
        return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class CaltropFumeCrushPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.CaltropShroom) return true;
        return !FumeVariants.PopContact(__instance);
    }
}
