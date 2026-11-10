using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Native Fire already plays its sound. Scripted projectiles play once when
// their original artwork is registered at birth, never during load or conversion.
internal static class FusionAudio
{
    internal static void RegisterLibrary()
    {
        foreach (var sound in new[] { FoleyType.Puff, FoleyType.Throw, FoleyType.MelonImpact, FoleyType.KernelSplat, FoleyType.Butter, FoleyType.Splat })
            PlantLibrary.Projectiles.RegisterSound("fusion:" + sound, board => board.mApp.SoundSystem.PlayFoley(sound));
        foreach (var type in new[] { ProjectileType.Melon, ProjectileType.Kernel, ProjectileType.Butter, ProjectileType.Pea }) {
            var sound = type == ProjectileType.Melon ? FoleyType.MelonImpact : type == ProjectileType.Kernel ? FoleyType.KernelSplat
                : type == ProjectileType.Butter ? FoleyType.Butter : FoleyType.Splat;
            PlantLibrary.Projectiles.RegisterProjectile(new("fusion:imported:" + type,
                shot => !ChargeTorchFusions.IsFireCherry(shot) && !ChargeTorchFusions.IsLowFire(shot) && CactusProjectiles.Family(shot) != 8 && !GlacierShroom.Contains(shot) && ImportedPeaVisuals.HasProjectileArt(shot) && (type == ProjectileType.Pea
                    ? shot.mProjectileType is not (ProjectileType.Melon or ProjectileType.Kernel or ProjectileType.Butter)
                    : shot.mProjectileType == type), Born, ImpactSound: "fusion:" + sound, ImpactOnlyWhenOriginalSkipped: true));
        }
        PlantLibrary.Projectiles.RegisterProjectile(new("fusion:fire-cherry", ChargeTorchFusions.IsFireCherry, Born));
        PlantLibrary.Projectiles.RegisterProjectile(new("fusion:low-fire", ChargeTorchFusions.IsLowFire));
        PlantLibrary.Projectiles.RegisterImpactEffect("fusion:potato-impact",
            impact => impact.Board.mApp.AddTodParticle(impact.X, impact.Y, impact.RenderOrder, ParticleEffect.PeaSplat));
        PlantLibrary.Projectiles.RegisterImpactEffect("fusion:glacier-impact",
            impact => impact.Board.mApp.AddTodParticle(impact.X, impact.Y, impact.RenderOrder, ParticleEffect.SnowpeaSplat));
        PlantLibrary.Projectiles.RegisterProjectile(new("fusion:glacier", GlacierShroom.Contains,
            shot => PlantLibrary.Projectiles.PlaySound("fusion:Puff", shot.mBoard)));
        foreach (var effect in new[] { ParticleEffect.MelonSplash, ParticleEffect.ButterSplat, ParticleEffect.CabbageSplat })
            PlantLibrary.Projectiles.RegisterImpactEffect("fusion:" + effect,
                impact => impact.Board.mApp.AddTodParticle(impact.X, impact.Y, impact.RenderOrder, effect));
        PlantLibrary.Projectiles.RegisterImpactEffect("fusion:ball-cactus-impact",
            impact => impact.Board.mApp.AddTodParticle(impact.X, impact.Y, impact.RenderOrder, ParticleEffect.PeaSplat));
        // Collision owns the captured effect and sound, including its direct
        // swept-collision path. Automatic impact dispatch must not repeat it.
        PlantLibrary.Projectiles.RegisterProjectile(new("fusion:ball-cactus",
            shot => CactusProjectiles.Family(shot) == 8, Born));
    }
    internal static void Born(Projectile shot)
    {
        if (shot.mDead || shot.mProjectileAge != 0 || ShootingPots.Firing != null || HypnoShooters.FiringZombie != null) return;
        var sound = PuffShooters.Contains(shot) || PuffSpecials.IsIcicle(shot) || PuffSpecials.IsNugget(shot)
            || GutsyShroom.IsShot(shot) || ScaredyProjectiles.Family(shot) != 0 ? FoleyType.Puff : FoleyType.Throw;
        PlantLibrary.Projectiles.PlaySound("fusion:" + sound, shot.mBoard);
    }
}
