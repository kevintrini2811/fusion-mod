// Offline API adapter. It does not load PlantLibrary, Unity or the game.
using Il2CppReloaded.Gameplay;
namespace PlantLibrary;
public readonly record struct PlantIdentity(SeedType NativeType, SeedType Marker = SeedType.None);
public sealed record PlantDefinition(PlantIdentity Identity);
public static class Plants {
    public static readonly Dictionary<PlantIdentity, PlantDefinition> Registered = new() {
        [new(SeedType.Sunflower, SeedType.Gravebuster)] = new(new(SeedType.Sunflower, SeedType.Gravebuster))
    };
    public static readonly HashSet<(Board Board, PlantIdentity Identity)> Unlocks = new();
    public static PlantDefinition? Find(Plant plant) => Find(new PlantIdentity(plant.mSeedType, plant.mImitaterType));
    public static PlantDefinition? Find(PlantIdentity identity) => Registered.GetValueOrDefault(identity);
    public static void Unlock(PlantDefinition plant, Board board) => Unlocks.Add((board, plant.Identity));
}
