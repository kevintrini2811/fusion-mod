using Il2CppReloaded.Gameplay;
namespace FusionMod;
// These suites isolate unrelated owners. DoomFusionsTests compiles the actual
// Doom owners and their persistence, damage and lifecycle cases.
internal static class DoomFusions { internal static void Fertilize(Plant plant) { } internal static void Detonate(Plant plant) { } }
internal static class EmberEffects { internal static readonly HashSet<object> Irradiated = new(); internal static bool Apply(object zombie) => Irradiated.Add(zombie); internal static bool Has(object zombie) => Irradiated.Contains(zombie); }
