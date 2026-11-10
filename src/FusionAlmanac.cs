using System.Text.Json;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// This is a view of the implemented recipe results. It never registers fake
// SeedTypes, creates gameplay plants, unlocks entries or writes user profiles.
internal static class FusionAlmanac
{
    private sealed record EntryText(string Name, string Info, string Flavor, string Notes, string[] Recipes);
    private sealed record Entry(int SourceId, string Identity, string CanonicalName,
        EntryText English, EntryText German, string[] Recipes, string Portrait);

    private static Entry[]? entries;
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Board, Dictionary<FusionPlant, int>> Costs = new();
    // These drops are ingredients without a recipe result, so they deliberately
    // stay outside the result catalog. Reuse their exported original body sprites.
    private static readonly Dictionary<FusionPlant, (int Id, string English, string German, string Portrait)> IngredientOnly = new()
    {
        [FusionPlant.Bucket] = (224, "Bucket", "Eimer", "FusionMod.Bucket.sprite-15928.png"),
        [FusionPlant.FootballHelmet] = (225, "Football Helmet", "Football-Helm", "FusionMod.FootballHelmet.sprite-15508.png"),
        [FusionPlant.EyesZombieHead] = (226, "Eyes Zombie Head", "Augen-Zombie-Kopf", "FusionMod.EyesZombieHead.sprite-4582.png")
    };

    internal static bool GermanGameLanguage() => PlantLibrary.PlantLocalization.German;
    internal static class GameLocalization
    {
        internal static string? LanguageLabel() => PlantLibrary.PlantLocalization.LanguageLabel();
        internal static string Localize(string key) => PlantLibrary.PlantLocalization.Localize(key);
    }

    private static Stream Resource(string file) => typeof(FusionAlmanac).Assembly
        .GetManifestResourceStream(file)
        ?? throw new InvalidDataException($"Missing fusion almanac resource: {file}");

    private static FusionPlant CatalogPlant(Entry entry)
    {
        if (entry.Identity.StartsWith("SeedType.", StringComparison.Ordinal))
            return new FusionPlant(Enum.Parse<SeedType>(entry.Identity[9..]));
        const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic;
        string name = entry.Identity[12..];
        return (FusionPlant)(typeof(FusionPlant).GetField(name, flags)?.GetValue(null)
            ?? typeof(FusionPlant).GetProperty(name, flags)?.GetValue(null)
            ?? throw new InvalidDataException($"Unknown almanac identity: {entry.Identity}"));
    }

    private static void LoadCatalog()
    {
        if (entries != null) return;
        using var stream = Resource("FusionMod.Almanac.catalog.json");
        entries = JsonSerializer.Deserialize<Entry[]>(stream)
            ?? throw new InvalidDataException("Invalid fusion almanac catalog.");
        if (entries.Length == 0) throw new InvalidDataException("Empty fusion almanac catalog.");
    }

    internal static IEnumerable<(FusionPlant Plant, string Name, string Portrait)> DebugCatalog(bool ingredients)
    {
        LoadCatalog();
        bool german = GermanGameLanguage();
        if (ingredients)
            return IngredientOnly.Select(pair => (pair.Key,
                german ? pair.Value.German : pair.Value.English, pair.Value.Portrait));
        return entries!.Select(entry => (CatalogPlant(entry),
            german ? entry.German.Name : entry.English.Name, $"FusionMod.Almanac.{entry.Portrait}"));
    }

    internal static void RegisterLibrary()
    {
        LoadCatalog();
        var owner = new PlantLibrary.ModCatalog("fusion", "Plant Fusion", Resource) {
            Related = plant => FusionRecipes.Partners(new FusionPlant(plant.NativeType, plant.Marker))
                .Select(pair => (new PlantLibrary.PlantIdentity(pair.Result.NativeType, pair.Result.Marker),
                    new PlantLibrary.PlantIdentity(pair.Partner.NativeType, pair.Partner.Marker))),
            CostNotes = german => german ? "Kosten und Aufladezeit: eingehende Zutatenkarte. Handschuh: dessen Kostenregeln."
                : "Cost and recharge: the incoming ingredient's seed packet. Glove: its existing cost rules."
        };
        foreach (var pair in IngredientOnly) owner.Ingredients.Add(new(pair.Key.NativeType, pair.Key.Marker),
            new(pair.Value.Id, pair.Value.English, pair.Value.German, pair.Value.Portrait));
        foreach (var entry in entries!) {
            var identity = CatalogPlant(entry);
            var text = new PlantLibrary.Entry(entry.SourceId, new(identity.NativeType, identity.Marker),
                new(entry.English.Name, entry.English.Info, entry.English.Flavor, entry.English.Notes, entry.English.Recipes),
                new(entry.German.Name, entry.German.Info, entry.German.Flavor, entry.German.Notes, entry.German.Recipes),
                $"FusionMod.Almanac.{entry.Portrait}");
            var plant = PlantLibrary.Plants.RegisterPlant(owner, entry.Identity, text,
                board => Costs.GetValue(board, PlantValue.Calculate)[identity],
                identity == FusionPlant.BloodSunflower ? PlantLibrary.UnlockPolicy.Explicit : PlantLibrary.UnlockPolicy.Never);
            plant.CanPlant = (board, col, row) => DebugPlantCards.Placement(board, identity, col, row);
        }
        PlantLibrary.Plants.RegisterCatalog(owner);
    }
}
