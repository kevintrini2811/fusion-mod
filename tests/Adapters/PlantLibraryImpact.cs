// Offline presentation adapter preserving the existing particle assertions.
using Il2CppReloaded.Gameplay;
namespace PlantLibrary;
public sealed record ProjectileImpact(Board Board, float X, float Y, int RenderOrder) {
    public static ProjectileImpact Capture(Projectile shot) => new(shot.mBoard, shot.mPosX, shot.mPosY, shot.mRenderOrder);
}
public static class Projectiles {
    public static void PlaySound(string id, Board board) { }
    public static void EmitImpactEffect(string id, ProjectileImpact impact) =>
        impact.Board.mApp.AddTodParticle(impact.X, impact.Y, impact.RenderOrder,
            id == "fusion:potato-impact" ? Enum.Parse<ParticleEffect>("PeaSplat") : Enum.Parse<ParticleEffect>(id["fusion:".Length..]));
}
