using System.Reflection;

namespace FusionMod;

// Isolation double for owner behavior tests: each direct owner invocation is
// one canonical 100-Hz source quantum. This does not simulate the production
// scheduler, pause handling, native tick-rate changes, or pulse deduplication.
// Production FusionTime is tested separately without FusionIdentity.projitems.
internal static class FusionTime
{
    internal const double SourceTickSeconds = .01;
    internal static double NativeTickSeconds => .01;
    internal static bool SourceStep<T>(T entity, object owner, IntPtr pointer = default, uint id = 0) => true;
    internal static bool IsSourcePulse<T>(T board) => true;
    internal static int NativeTicks(int sourceTicks) => sourceTicks;
    internal static int NativeTicksFromSeconds(double seconds) => (int)Math.Ceiling(seconds * 100 - 1e-9);
    internal static float NativeSeconds(int nativeTicks) => nativeTicks * .01f;
    internal static float SourceSeconds(int sourceTicks) => sourceTicks * .01f;
    internal static double StepSeconds<T>(T board) => .01;
    internal static double ElapsedSeconds<T>(T board) => SourceTicks(board) * .01;
    internal static long SourceTicks<T>(T board)
    {
        if (board == null) return 0;
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        object value = board;
        var type = value.GetType();
        // Some isolated owners pass an entity rather than its board. Reflection
        // keeps this double independent of the intentionally minimal stubs.
        var counter = type.GetField("mMainCounter", flags)?.GetValue(value)
            ?? type.GetProperty("mMainCounter", flags)?.GetValue(value);
        if (counter != null) return Convert.ToInt64(counter);
        var parent = type.GetField("mBoard", flags)?.GetValue(value)
            ?? type.GetProperty("mBoard", flags)?.GetValue(value);
        return parent == null || ReferenceEquals(parent, value) ? 0 : SourceTicks(parent);
    }
}

// Identity-rate collaborators for owner-isolation suites. Actual native
// boundaries have dedicated projects that compile their production sources.
internal static class FusionProjectileTime
{
    internal const int Age = 1, Planar = 2, Vertical = 4, Gravity = 8, FixedStep = 16;
    internal const int Ballistic = Age | Planar | Vertical | Gravity;
    internal static float SourceVelocity(float value) => value;
    internal static float SourceGravity(float value) => value;
    internal static bool Owns<T>(T shot) => false;
    internal static void NativeLaunch<T>(T shot, int flags = Ballistic) { }
    internal static void SourceLaunch<T>(T shot, int flags) { }
    internal static void SourceMotionHandoff<T>(T shot, int flags) { }
    internal static void AdoptConversion<T>(T shot) { }
    internal static float AgeSeconds<T>(T shot)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        object? value = shot;
        return value == null ? 0 : Convert.ToSingle(value.GetType().GetField("mProjectileAge", flags)?.GetValue(value) ?? 0) * .01f;
    }
}
internal static class FusionProjectileDomains
{
    internal static bool CustomMotion<T>(T shot) => false;
}
internal static class FusionCraterTime
{
    internal static void SetDuration<T>(T crater, int sourceTicks)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        object? value = crater;
        value?.GetType().GetField("mGridItemCounter", flags)?.SetValue(value, sourceTicks);
    }
}

internal static class FusionStatuses
{
    internal enum Effect { Butter = 1, Ice = 2, Chill = 4 }
    private const BindingFlags Fields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static void Invoke<T>(T zombie, string name, object[] args)
    {
        object? value = zombie;
        value?.GetType().GetMethods(Fields).FirstOrDefault(method => method.Name == name
            && method.GetParameters().Length == args.Length)?.Invoke(value, args);
    }
    private static FieldInfo? Counter<T>(T zombie, Effect effect) => zombie?.GetType().GetField(
        effect == Effect.Butter ? "mButteredCounter" : effect == Effect.Ice ? "mIceTrapCounter" : "mChilledCounter", Fields);
    private static int Read<T>(T zombie, Effect effect) => Convert.ToInt32(Counter(zombie, effect)?.GetValue(zombie) ?? 0);
    private static void Accepted<T>(T zombie, Effect effect, int sourceTicks, int previous)
    {
        if (Read(zombie, effect) > 0) Counter(zombie, effect)?.SetValue(zombie, Math.Max(previous, sourceTicks));
    }
    internal static void Butter<T>(T zombie, int sourceTicks = 400)
    {
        int previous = Read(zombie, Effect.Butter);
        Invoke(zombie, "ApplyButter", Array.Empty<object>());
        Accepted(zombie, Effect.Butter, sourceTicks, previous);
    }
    internal static void GlobalFreeze<T>(T plant) => Invoke(plant, "IceZombies", Array.Empty<object>());
    internal static void Chill<T>(T zombie, bool longer = false, int sourceTicks = 0)
    {
        int previous = Read(zombie, Effect.Chill);
        Invoke(zombie, "ApplyChill", new object[] { longer });
        Accepted(zombie, Effect.Chill, sourceTicks > 0 ? sourceTicks : longer ? 2000 : 1000, previous);
    }
    internal static void Freeze<T>(T zombie, int sourceTicks = 400, int chillTicks = 1400, bool suppressDamage = true)
    {
        int ice = Read(zombie, Effect.Ice), chill = Read(zombie, Effect.Chill);
        object? value = zombie;
        var suppressor = typeof(FusionStatuses).Assembly.GetType("FusionMod.SnowPea")?.GetField("CreatingIceTrap", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        object? previous = suppressor?.GetValue(null);
        if (suppressDamage && suppressor != null && value != null)
            suppressor.SetValue(null, value.GetType().GetField("Pointer", Fields)?.GetValue(value)
                ?? value.GetType().GetProperty("Pointer", Fields)?.GetValue(value) ?? IntPtr.Zero);
        try { Invoke(zombie, "HitIceTrap", Array.Empty<object>()); }
        finally { if (suppressDamage && suppressor != null) suppressor.SetValue(null, previous); }
        Accepted(zombie, Effect.Ice, sourceTicks, ice);
        Accepted(zombie, Effect.Chill, chillTicks, chill);
    }
    internal static void Minimum<T>(T zombie, Effect effect, int sourceTicks)
    {
        object? value = zombie;
        var field = value?.GetType().GetField(effect == Effect.Butter ? "mButteredCounter"
            : effect == Effect.Ice ? "mIceTrapCounter" : "mChilledCounter", Fields);
        if (field != null) field.SetValue(value, Math.Max(Convert.ToInt32(field.GetValue(value)), sourceTicks));
    }
}
