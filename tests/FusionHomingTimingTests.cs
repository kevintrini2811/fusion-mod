using FusionMod;
using Il2CppReloaded.Gameplay;
using Clock = Il2CppReloaded.Constants.Gameplay;

static class Program
{
    private static int checks;
    private static void Check(bool condition, string message) { ++checks; if (!condition) throw new Exception(message); }
    private static (Board, Projectile) Head(double period, float targetY = 0)
    {
        Clock.TIME_FOR_UPDATE = period;
        var board = new Board { Target = new Zombie { DataID = 9, TargetY = targetY } };
        var shot = new Projectile { mBoard = board, mVelX = (float)(200 * period), mTargetZombieID = 9 };
        FusionNativeTimeRecords.Get(board, FusionNativeTimeRecords.Kind.Projectile, shot.DataID, (int)Math.Round(1 / period));
        return (board, shot);
    }
    private static void Step(Board board, Projectile shot)
    {
        FusionTime.BeginTick(board); ++shot.mProjectileAge;
        var state = FusionHomingTime.Prepare(shot);
        Check(shot.mMotionType == ProjectileMotion.Star, "owned homing supplies a vector to native motion");
        shot.mPosX += shot.mVelX; shot.mPosY += shot.mVelY; // Native Star boundary model.
        FusionHomingTime.BeforeCollision(shot);
        Check(shot.mMotionType == ProjectileMotion.Homing, "native collision sees Homing target and terrain semantics");
        FusionHomingTime.Restore(shot, state);
        Check(shot.mMotionType == ProjectileMotion.Homing, "native collision keeps saved Homing identity afterward");
        ++board.mMainCounter;
    }
    public static void Main()
    {
        foreach (double period in new[] { .01, 1d / 120 }) {
            var (board, shot) = Head(period);
            for (int i = 0; i < (int)Math.Round(1 / period); ++i) Step(board, shot);
            Check(Math.Abs(shot.mPosX - 200) < .001 && Math.Abs(shot.mPosY) < .001, "two source pixels per centisecond at either native rate");
            Check(Math.Abs(Math.Sqrt(shot.mVelX * shot.mVelX + shot.mVelY * shot.mVelY) / period - 200) < .001, "native velocity magnitude retains correct units");
        }
        var (b, p) = Head(.01, 100);
        Step(b, p);
        Check(p.mVelY > 0 && p.mRotation < 0, "source steering follows native target center and rotation sign");
        float x = p.mVelX, y = p.mVelY;
        var record = FusionNativeTimeRecords.Get(b, FusionNativeTimeRecords.Kind.Projectile, p.DataID)!;
        Clock.TIME_FOR_UPDATE = 1d / 120; record.Rate = 120; p.mVelX *= 100f / 120; p.mVelY *= 100f / 120;
        b.mMainCounter++; FusionTime.BeginTick(b); // First rate-switched sample has no source pulse.
        var scope = FusionHomingTime.Prepare(p);
        Check(Math.Abs(p.mVelX - x * 100f / 120) < 1e-6 && Math.Abs(p.mVelY - y * 100f / 120) < 1e-6, "skipped source steering preserves direction while native movement remains enabled");
        FusionHomingTime.Restore(p, scope);
        b.mPaused = true;
        Check(FusionHomingTime.Prepare(p) == null && p.mMotionType == ProjectileMotion.Homing, "pause does not touch motion");
        b.mPaused = false; p.Owned = false;
        Check(FusionHomingTime.Prepare(p) == null, "unchanged native homing is untouched");
        p.Owned = true;
        scope = FusionHomingTime.Prepare(p); p.DataID++;
        FusionHomingTime.Restore(p, scope);
        Check(p.mMotionType == ProjectileMotion.Star, "pool generation replacement receives no stale finalizer write");
        var (oldBoard, migrating) = Head(.01);
        scope = FusionHomingTime.Prepare(migrating); migrating.mBoard = new Board();
        FusionHomingTime.Restore(migrating, scope);
        Check(migrating.mMotionType == ProjectileMotion.Star, "board replacement receives no stale finalizer write");
        p.mMotionType = ProjectileMotion.Homing;
        Check(FusionHomingTime.Prepare(p) == null, "absent provenance does not modify a new target generation");
        var (horizontalBoard, horizontal) = Head(.01);
        horizontal.mRow = 3;
        var horizontalScope = FusionHomingTime.Prepare(horizontal);
        Check(horizontal.mRow == 0, "horizontal Homing refreshes endpoint row before native collision like the native Homing branch");
        FusionHomingTime.Restore(horizontal, horizontalScope);
        var (conversionBoard, converted) = Head(.01);
        var conversionScope = FusionHomingTime.Prepare(converted);
        FusionHomingTime.BeforeCollision(converted);
        converted.mMotionType = ProjectileMotion.Star; // A real impact intentionally changes motion.
        FusionHomingTime.Restore(converted, conversionScope);
        Check(converted.mMotionType == ProjectileMotion.Star, "later real collision motion changes survive the movement finalizer");
        var (missing, straight) = Head(.01); missing.Target = null;
        Step(missing, straight);
        Check(Math.Abs(straight.mPosX - 2) < 1e-6, "missing target retains prior native flight vector");
        Console.WriteLine($"PASS {checks} actual Homing owner assertions; native vector movement/collision is an explicit boundary model.");
    }
}
namespace FusionMod {
    internal static class FusionProjectileTime {
        internal static bool Owns(Projectile shot) => shot.Owned;
        internal static float AgeSeconds(Projectile shot) => (float)shot.mProjectileAge /
            FusionNativeTimeRecords.Get(shot.mBoard, FusionNativeTimeRecords.Kind.Projectile, shot.DataID)!.Rate;
    }
}
