using Il2CppReloaded.Gameplay;

namespace FusionMod;

// One valuation for chooser prices and cost-dependent plant abilities.
internal static class PlantValue
{
    internal static Dictionary<FusionPlant, int> Calculate(Board value)
    {
        var recipes = FusionRecipes.Entries.ToArray();
        // A recipe that feeds its existing result (Wall-nut + Tall-nut, for
        // example) cannot define that result's initial construction price.
        var results = recipes.Where(recipe => recipe.Result != recipe.Ingredients.Existing
                && recipe.Result != recipe.Ingredients.Incoming).Select(recipe => recipe.Result).ToHashSet();
        var costs = new Dictionary<FusionPlant, int>();
        for (int i = 0; i < (int)SeedType.NumSeedTypes; ++i)
            if (!results.Contains((SeedType)i))
                costs[(SeedType)i] = Plant.GetCost(value.mApp, (SeedType)i, SeedType.None);
        foreach (var item in new[] { FusionPlant.Bucket, FusionPlant.FootballHelmet, FusionPlant.EyesZombieHead }) costs[item] = 100;
        foreach (var item in new[] { FusionPlant.RoastedSquash, FusionPlant.RoastedWallNut, FusionPlant.RoastedPumpkin, FusionPlant.RoastedPotato })
            costs[item] = Plant.GetCost(value.mApp, item.NativeType, SeedType.None);
        // Relax the recipe graph. A minimum over all available paths handles
        // alternative recipes and cycles without recursive self-dependence.
        for (int pass = 0; pass <= recipes.Length; ++pass)
        {
            bool changed = false;
            foreach (var recipe in recipes)
            {
                if (!costs.TryGetValue(recipe.Ingredients.Existing, out int left)
                    || !costs.TryGetValue(recipe.Ingredients.Incoming, out int right)) continue;
                int sum = (int)Math.Min(int.MaxValue, (long)left + right);
                if (!costs.TryGetValue(recipe.Result, out int previous) || sum < previous)
                { costs[recipe.Result] = sum; changed = true; }
            }
            if (!changed) break;
        }
        // These are paid tool upgrades, not plant-pair recipes.
        costs[FusionPlant.GoldenNut] = costs[FusionPlant.SilverNut] + 1000;
        costs[FusionPlant.GoldenSunflower] = costs[FusionPlant.SilverSunflower] + 3000;
        return costs;
    }
}
