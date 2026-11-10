using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// The native Homing branch hardcodes a two-pixel speed and age*.001 steering.
// Compute that owned steering in source units, then let native Star motion run
// the resulting vector, row lookup and collision checks at the actual rate.
internal static class FusionHomingTime
{
    internal sealed record Scope(IntPtr Board, IntPtr Pointer, uint Id, Scope? Previous)
    { internal bool CollisionRestored; }
    [ThreadStatic] private static Scope? current;

    private static (float X, float Y) Normalize(float x, float y)
    {
        float length = MathF.Sqrt(x * x + y * y);
        return length > .00001f ? (x / length, y / length) : (0, 0);
    }

    internal static Scope? Prepare(Projectile shot)
    {
        if (shot.mMotionType != ProjectileMotion.Homing || shot.mDead || shot.mBoard == null
            || !ScaredContent.Playing(shot.mBoard) || shot.mBoard.mPaused
            || !FusionProjectileTime.Owns(shot)) return null;
        var record = FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile, shot.DataID);
        if (record == null) return null; // No guessed conversion after failed provenance allocation.
        var target = shot.mBoard.ZombieTryToGet(shot.mTargetZombieID);
        if (target != null && target.EffectedByDamage(shot.mDamageRangeFlags)
            && FusionTime.SourceStep(shot, typeof(FusionHomingTime)))
        {
            var rect = target.GetZombieRect();
            var direction = Normalize(target.ZombieTargetLeadX(0) - (shot.mPosX + shot.mWidth * .5f),
                rect.center.y - (shot.mPosY + shot.mHeight * .5f));
            float scale = 100f / record.Rate;
            float sourceAge = (float)Math.Floor(FusionProjectileTime.AgeSeconds(shot) * 100d + 1e-5);
            var velocity = Normalize(shot.mVelX / scale + direction.X * sourceAge * .001f,
                shot.mVelY / scale + direction.Y * sourceAge * .001f);
            shot.mVelX = velocity.X * 2 * scale;
            shot.mVelY = velocity.Y * 2 * scale;
            shot.mRotation = -MathF.Atan2(shot.mVelY, shot.mVelX);
        }
        // Native Homing always refreshes the endpoint's row. Star omits that
        // lookup for an exactly horizontal vector, so supply only that case.
        if (shot.mVelY == 0)
            shot.mRow = shot.mBoard.PixelToGridY((int)(shot.mPosX + shot.mVelX), (int)shot.mPosY);
        var state = current = new Scope(shot.mBoard.Pointer, shot.Pointer, shot.DataID, current);
        shot.mMotionType = ProjectileMotion.Star;
        return state;
    }

    internal static void BeforeCollision(Projectile shot)
    {
        // CheckForCollision has a distinct Homing-only target branch. Restore
        // identity before any collision prefix or the native collision body.
        if (current is { } state && shot.Pointer == state.Pointer && shot.DataID == state.Id
            && shot.mBoard?.Pointer == state.Board && shot.mMotionType == ProjectileMotion.Star)
        { shot.mMotionType = ProjectileMotion.Homing; state.CollisionRestored = true; }
    }

    internal static void Restore(Projectile shot, Scope? state)
    {
        if (state != null) current = state.Previous;
        if (state is { CollisionRestored: false } && shot.Pointer == state.Pointer && shot.DataID == state.Id
            && shot.mBoard?.Pointer == state.Board && shot.mMotionType == ProjectileMotion.Star)
            shot.mMotionType = ProjectileMotion.Homing;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class FusionHomingTimePatch
{
    // Target acquisition by existing Fusion prefixes runs first. Motion-age
    // scopes expose their exact saved seconds through AgeSeconds during this call.
    [HarmonyPriority(Priority.Last - 3000)]
    internal static void Prefix(Projectile __instance, bool __runOriginal, out FusionHomingTime.Scope? __state) =>
        __state = __runOriginal ? FusionHomingTime.Prepare(__instance) : null;
    internal static void Finalizer(Projectile __instance, FusionHomingTime.Scope? __state) =>
        FusionHomingTime.Restore(__instance, __state);
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class FusionHomingCollisionIdentityPatch
{
    [HarmonyPriority(Priority.First + 5000)]
    internal static void Prefix(Projectile __instance) => FusionHomingTime.BeforeCollision(__instance);
}
