using System.Reflection;
using UnityEngine;

namespace FusionMod;

// Bind only to an already-loaded optional mod. Delegates keep reflection out
// of animation callbacks; without Speed Mod, native animation time is unchanged.
internal static class OptionalSpeed
{
    private static readonly Assembly? assembly = AppDomain.CurrentDomain.GetAssemblies()
        .FirstOrDefault(value => value.GetName().Name == "SpeedMod");
    internal static readonly Type? ClockType = assembly?.GetType("SpeedMod.VisualClock");
    private static readonly Func<bool>? reduced = ClockType?.GetProperty("Reduced")?.GetMethod
        ?.CreateDelegate<Func<bool>>();
    private static readonly Func<RectTransform?>? slowdown = assembly?.GetType("SpeedMod.BoardSpeed")
        ?.GetProperty("SlowdownAnchor")?.GetMethod?.CreateDelegate<Func<RectTransform?>>();

    internal static bool Reduced => reduced?.Invoke() ?? false;
    internal static RectTransform? SlowdownAnchor => slowdown?.Invoke();
}

internal sealed class FusionVisualClock
{
    private readonly Func<string, float, float>? sample;

    internal FusionVisualClock()
    {
        var type = OptionalSpeed.ClockType;
        var method = type?.GetMethod("Sample", new[] { typeof(string), typeof(float) });
        if (method?.ReturnType == typeof(float) && type!.GetConstructor(Type.EmptyTypes) != null)
            sample = method.CreateDelegate<Func<string, float, float>>(Activator.CreateInstance(type));
    }

    public float Sample(string track, float seconds) => sample?.Invoke(track, seconds) ?? seconds;
}
