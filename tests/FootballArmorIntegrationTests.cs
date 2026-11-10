using System.Reflection;
using System.Runtime.ExceptionServices;
using FusionMod;
using Il2CppReloaded.Gameplay;
using ZombieLibrary;

// Actual product API, scopes, Football damage owner and added-hat owner are
// linked by the project. Only native IL2CPP method bodies and Difficulty's
// outer division are modeled here; this is not a live Harmony/game test.
internal static class FootballArmorIntegrationTests
{
    private static int checks, entries, divisor = 1;
    private static bool veto, immune;
    private static Action? onEntry;
    private static readonly Assembly Product = typeof(Zombies).Assembly;
    private static void Check(bool value, string message)
    { ++checks; if (!value) throw new Exception(message); }
    private static MethodInfo Method(string type, string method) => Product.GetType("ZombieLibrary." + type)!
        .GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic)!;
    private static object? Invoke(string type, string method, object?[] args)
    {
        try { return Method(type, method).Invoke(null, args); }
        catch (TargetInvocationException error) when (error.InnerException != null)
        { ExceptionDispatchInfo.Capture(error.InnerException).Throw(); throw; }
    }
    private static object? EnterLibrary(string type, Zombie zombie, int damage, DamageFlags flags)
    {
        var method = Method(type, "Prefix");
        var parameters = method.GetParameters(); var args = new object?[parameters.Length];
        for (int i = 0; i < args.Length; ++i)
            args[i] = parameters[i].Name == "__state" ? null
                : parameters[i].ParameterType == typeof(Zombie) ? zombie
                : parameters[i].ParameterType == typeof(DamageFlags) ? flags : damage;
        Invoke(type, "Prefix", args);
        return args[Array.FindIndex(parameters, value => value.Name == "__state")];
    }
    private static void ExitLibrary(string type, object? state)
    {
        var method = Method(type, "Finalizer");
        var args = method.GetParameters().Select(value => value.Name == "__state" ? state : null).ToArray();
        Invoke(type, "Finalizer", args);
    }
    private static void NativeDamage(Zombie zombie, int damage, DamageFlags flags)
    {
        FootballFumeEntryPatch.Prefix(out var footballState);
        var independent = EnterLibrary("NativeIndependentAddedArmorEntryPatch", zombie, damage, flags);
        var hit = EnterLibrary("NativeBobsledHitScopePatch", zombie, damage, flags);
        try
        {
            ++entries;
            onEntry?.Invoke();
            if (immune) return;
            damage /= divisor; int remaining = damage;
            if (zombie.mShieldHealth > 0 && (flags & DamageFlags.BypassesShield) == 0)
            {
                int shieldResult = 0;
                if (FootballFumeShieldPatch.Prefix(zombie, ref shieldResult))
                { shieldResult = Math.Max(0, damage - zombie.mShieldHealth); zombie.mShieldHealth = Math.Max(0, zombie.mShieldHealth - damage); }
                remaining = (flags & DamageFlags.HitsShieldAndBody) != 0 ? damage : shieldResult;
            }
            if (remaining <= 0) return;
            if (zombie.mHelmType != HelmType.None)
            {
                int helmDamage = damage, result = 0;
                bool run = !veto && FootballFumeHelmPatch.Prefix(zombie, ref result);
                if (run)
                {
                    object?[] args = { zombie, helmDamage, flags, result };
                    run = (bool)Invoke("NativeArmoredBobsledDamagePatch", "Prefix", args)!;
                    helmDamage = (int)args[1]!; result = (int)args[3]!;
                }
                if (run)
                { result = Math.Max(0, helmDamage - zombie.mHelmHealth); zombie.mHelmHealth = Math.Max(0, zombie.mHelmHealth - helmDamage); }
                FootballFumeHelmPatch.Postfix(zombie, helmDamage, run, ref result);
                remaining = result;
            }
            if (remaining > 0 && FootballFumeBodyPatch.Prefix(zombie))
            {
                object?[] args = { zombie, remaining, flags };
                bool run = (bool)Invoke("NativeBobsledCrashedHatPatch", "Prefix", args)!;
                remaining = (int)args[1]!;
                if (run) zombie.mBodyHealth -= remaining;
            }
        }
        finally
        {
            ExitLibrary("NativeBobsledHitScopePatch", hit);
            ExitLibrary("NativeIndependentAddedArmorEntryPatch", independent);
            FootballFumeEntryPatch.Finalizer(null, footballState);
        }
    }
    private static (Plant Plant, Zombie Zombie) Make(int hat, bool crashed, bool shield = true)
    {
        var board = new Board(); var plant = new Plant { mBoard = board };
        var zombie = new Zombie { mBoard = board, mZombieType = ZombieType.Bobsled,
            mHelmType = HelmType.Bobsled, mHelmHealth = 500, mHelmMaxHealth = 500,
            mBodyHealth = 1000, mShieldHealth = shield ? 500 : 0 };
        var definition = Zombies.All.Single(entry => entry.Id == NativeArmorTypes.Id(ZombieType.Bobsled, true));
        Zombies.Apply(definition, zombie); zombie.mGraveY = hat;
        if (crashed) { zombie.mHelmType = HelmType.None; zombie.mHelmHealth = 0; }
        zombie.TakingDamage = (damage, flags) => NativeDamage(zombie, damage, flags);
        entries = 0; veto = immune = false; onEntry = null;
        return (plant, zombie);
    }
    internal static void Main()
    {
        NativeArmoredZombies.Register();
        foreach (int rate in new[] { 1, 2 }) foreach (int hat in new[] { 30, 200 })
        foreach (bool crashed in new[] { false, true }) foreach (bool shield in new[] { false, true })
        {
            divisor = rate; var (plant, zombie) = Make(hat, crashed, shield); int damage = 100 / rate;
            FootballFumeDamage.Hit(plant, zombie, zombie.DataID);
            Check(entries == 1, "Exactly one outer native damage entry / Difficulty division.");
            Check(zombie.mBodyHealth == 1000 - damage, "Body keeps independent scaled damage.");
            Check(zombie.mGraveY == Math.Max(0, hat - damage), "Actual added-hat owner receives independent scaled damage.");
            Check(zombie.mHelmHealth == (crashed ? 0 : 500 - damage), "Native sled helmet receives independent damage.");
            Check(zombie.mShieldHealth == (shield ? 500 - damage : 0), "Native shield receives independent damage.");
            Check(zombie.ChillCalls == 0 && FootballFumeDamage.Current == null, "No source6/nativeFreeze confusion or escaped Football scope.");
        }
        divisor = 1;
        {
            var (plant, zombie) = Make(200, false); veto = true;
            FootballFumeDamage.Hit(plant, zombie, zombie.DataID);
            Check(zombie.mBodyHealth == 1000 && zombie.mHelmHealth == 500, "Foreign veto remains authoritative.");
        }
        {
            var (plant, zombie) = Make(200, true); immune = true;
            FootballFumeDamage.Hit(plant, zombie, zombie.DataID);
            Check(zombie.mBodyHealth == 1000 && zombie.mGraveY == 200, "Native outer immunity does not become direct layer damage.");
        }
        foreach (bool crashed in new[] { false, true })
        {
            var (plant, zombie) = Make(200, crashed, false); bool nested = false;
            onEntry = () => { if (nested) return; nested = true; zombie.TakeDamage(30, DamageFlags.BypassesShield); };
            FootballFumeDamage.Hit(plant, zombie, zombie.DataID);
            Check(entries == 2 && zombie.mGraveY == 70 && zombie.mBodyHealth == 900,
                "Same-target nested ordinary hit absorbs into hat; outer body stays independent.");
            Check(zombie.mHelmHealth == (crashed ? 0 : 400), "Nested ordinary hit does not inherit Football native-helmet override.");
        }
        {
            var (plant, zombie) = Make(30, false); zombie.Dropping = (actor, _) => ++actor.DataID;
            FootballFumeDamage.Hit(plant, zombie, zombie.DataID);
            Check(zombie.mBodyHealth == 1000, "Full generation check blocks old payload after hat-drop callback recycles target.");
        }
        {
            var (plant, zombie) = Make(30, false); zombie.Dropping = (_, _) => throw new InvalidOperationException("drop callback");
            try { FootballFumeDamage.Hit(plant, zombie, zombie.DataID); throw new Exception("Exception was swallowed."); }
            catch (InvalidOperationException) { }
            Check(FootballFumeDamage.Current == null, "Exception unwinds Football scope.");
            zombie.Dropping = null; zombie.mGraveY = 200; zombie.mHelmType = HelmType.Bobsled; zombie.mHelmHealth = 500;
            zombie.TakeDamage(100, DamageFlags.BypassesShield);
            Check(zombie.mGraveY == 100 && zombie.mHelmHealth == 500 && zombie.mBodyHealth == 1000,
                "Exception unwinds Library scope; subsequent ordinary damage still absorbs normally.");
        }
        Console.WriteLine($"PASS {checks} Football / actual Zombie Library armor integration checks; native bodies modeled, no game loaded.");
    }
}

namespace ZombieLibrary
{
    internal static class NativeArmorVisuals
    {
        internal static void Observe(Zombie zombie) { }
        internal static UnityEngine.Sprite Portrait(ZombieType type, bool bucket) => new();
    }
}
