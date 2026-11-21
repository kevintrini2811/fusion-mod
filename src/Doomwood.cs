using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class Doomwood
{
    internal const int InitialEnergy = 6000;
    private const int MaxEnergy = 60000;
    internal const int PurplePea = 5, BurningDoomPea = 6, BurningDoom = 7;

    internal static void Tick(Plant plant)
    {
        if (plant.mTargetY > 0 && BeamShooters.Active(plant)
            && FusionTime.SourceStep(plant, typeof(Doomwood))) --plant.mTargetY;
    }

    internal static void Fertilize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.Doomwood && plant.mTargetY > 50)
            plant.mTargetY = 50;
    }

    internal static void Charge(Plant plant, int ticks) =>
        plant.mTargetY = Math.Min(MaxEnergy, Math.Max(0, plant.mTargetY) + ticks);

    internal static void ChargeRadius(Board board, float x, float y, int ticks = 3000, float units = 5)
    {
        float radius = units * DoomProjectiles.Unit;
        foreach (var entry in TorchPlantIndex.For(board))
        {
            var plant = entry.Plant;
            if (!entry.IsCurrent(board) || FusionPlant.From(plant) != FusionPlant.Doomwood
                || !BeamShooters.Active(plant)) continue;
            float dx = plant.mX + plant.mWidth / 2f - x, dy = plant.mY + plant.mHeight / 2f - y;
            if (dx * dx + dy * dy <= radius * radius) Charge(plant, ticks);
        }
    }

    internal static void PlanternPulse(Plant lamp)
    {
        var board = lamp.mBoard;
        foreach (var entry in TorchPlantIndex.For(board))
        {
            var plant = entry.Plant;
            if (entry.IsCurrent(board) && FusionPlant.From(plant) == FusionPlant.Doomwood
                && Math.Abs(plant.mRow - lamp.mRow) <= 1 && Math.Abs(plant.mPlantCol - lamp.mPlantCol) <= 1
                && BeamShooters.Active(plant)) Charge(plant, 300);
        }
    }

    internal static bool ExtraInput(Projectile shot) => DoomProjectiles.Family(shot) is 1 or 2
        || shot.mCobTargetX >= 0 && shot.mProjectileType is ProjectileType.Fireball or ProjectileType.PeashooterFireball;

    internal static bool Convert(Plant plant, Projectile shot, bool snow, bool plain)
    {
        int col = plant.mPlantCol;
        int family = DoomProjectiles.Family(shot);
        bool changed = false;
        if (plant.mTargetY == 0 || snow)
        {
            if (snow || plant.mTargetY == 0 && plain)
            {
                changed = TorchFusions.ConvertPea(shot, plant, snow, small: false);
                if (changed && plant.mTargetY == 0) ++plant.mTargetX;
            }
            if (plant.mTargetY == 0 && plant.mTargetX >= 61)
            {
                plant.mTargetX = 0;
                Charge(plant, InitialEnergy);
            }
            return changed;
        }
        if (family is 1 or 2)
        {
            int damage = (int)Math.Min(int.MaxValue, (long)shot.mCobTargetRow * (family == 1 ? 2 : 6));
            ImportedPeaVisuals.ForgetProjectile(shot);
            DoomProjectiles.Track(shot, family == 1 ? BurningDoomPea : BurningDoom, damage);
            plant.mTargetY = Math.Max(0, plant.mTargetY - 1000);
            shot.mBoard.mApp.SoundSystem.PlayFoley(Il2CppReloaded.Services.FoleyType.FirePea);
        }
        else
        {
            if (!plain && !ExtraInput(shot)) return false;
            if (plain && !TorchFusions.ConvertPea(shot, plant, snow: false, small: false)) return false;
            if (!plain)
            {
                FusionProjectileTime.AdoptConversion(shot);
                ImportedPeaVisuals.ForgetProjectile(shot);
                shot.mBoard.mApp.SoundSystem.PlayFoley(Il2CppReloaded.Services.FoleyType.FirePea);
            }
            DoomProjectiles.Track(shot, PurplePea, plain ? 50 : 70);
            ++plant.mTargetX;
        }
        shot.mHitTorchwoodGridX = col;
        return true;
    }

    internal static void PurpleImpact(Board board, Zombie direct, float x, float y, int order, int damage)
    {
        const float radius = DoomProjectiles.Unit;
        int row = direct.mRow;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != row || !DoomProjectiles.Live(board, zombie)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= radius * radius) targets.Add((zombie, zombie.DataID));
        }
        direct.TakeDamage((int)(damage * 2L / 3), DamageFlags.HitsShieldAndBody);
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !DoomProjectiles.Live(board, zombie)) continue;
            zombie.TakeDamage(damage / 3, DamageFlags.HitsShieldAndBody);
            if (zombie.DataID == id) EmberEffects.Apply(zombie, explode: false);
        }
        board.mApp.AddTodParticle(x, y, order, ParticleEffect.FireballDeath);
        board.mApp.SoundSystem.PlayFoley(Il2CppReloaded.Services.FoleyType.FirePea);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class DoomwoodNativeChargePatch
{
    [HarmonyPriority(Priority.First)]
    private static void Prefix(Plant __instance, out (Board Board, uint Id, float X, float Y)? __state) =>
        __state = FusionPlant.From(__instance) == (FusionPlant)SeedType.Doomshroom && ChomperFusions.Active(__instance)
            ? (__instance.mBoard, __instance.DataID, __instance.mX + __instance.mWidth / 2f,
                __instance.mY + __instance.mHeight / 2f) : null;

    private static void Postfix(Plant __instance, (Board Board, uint Id, float X, float Y)? __state)
    {
        if (__state is { } blast && __instance.mDead && __instance.DataID == blast.Id
            && __instance.mBoard?.Pointer == blast.Board.Pointer)
            Doomwood.ChargeRadius(blast.Board, blast.X, blast.Y);
    }
}
