using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using ZombieLibrary;

namespace FusionMod;

internal static class FusionZombies
{
    private static T Retain<T>(T asset) where T : UnityEngine.Object
    { asset.hideFlags |= UnityEngine.HideFlags.DontUnloadUnusedAsset; return asset; }

    internal static ZombieDefinition Eyes = null!, Hypno = null!;
    private static ReplantedModUi.NativeZombiePortraits? portraits;
    private static UnityEngine.Sprite? eyesPortrait;
    private static readonly Dictionary<string, UnityEngine.Sprite> thumbnails = new();
    private static UnityEngine.Sprite Thumbnail(string key, bool duck)
    {
        string file = key + (duck ? "-duck" : "") + ".png";
        string resource = key == "hypno-portrait" ? "FusionMod.Assets.hypno-zombie.png" : "FusionMod.ZombieAlmanac." + file;
        if (thumbnails.TryGetValue(file, out var cached)) return cached;
        using var stream = typeof(FusionZombies).Assembly.GetManifestResourceStream(resource)
            ?? throw new InvalidDataException("Missing zombie thumbnail: " + file);
        using var bytes = new MemoryStream(); stream.CopyTo(bytes);
        var texture = Retain(new UnityEngine.Texture2D(2, 2, UnityEngine.TextureFormat.RGBA32, false));
        if (!UnityEngine.ImageConversion.LoadImage(texture, bytes.ToArray(), true))
        { UnityEngine.Object.Destroy(texture); throw new InvalidDataException("Invalid zombie thumbnail: " + file); }
        var sprite = Retain(UnityEngine.Sprite.Create(texture,
            new UnityEngine.Rect(0, 0, texture.width, texture.height), new UnityEngine.Vector2(.5f, .5f), 100));
        sprite.name = resource;
        thumbnails.Add(file, sprite); return sprite;
    }
    private static UnityEngine.Sprite EyesPortrait()
    {
        if (eyesPortrait != null) return eyesPortrait;
        using var stream = typeof(FusionZombies).Assembly.GetManifestResourceStream("FusionMod.DebugCards.eyes-zombie.png")!;
        using var bytes = new MemoryStream(); stream.CopyTo(bytes);
        var texture = Retain(new UnityEngine.Texture2D(2, 2, UnityEngine.TextureFormat.RGBA32, false));
        if (!UnityEngine.ImageConversion.LoadImage(texture, bytes.ToArray(), true))
        { UnityEngine.Object.Destroy(texture); throw new InvalidDataException("Invalid eyes zombie portrait."); }
        return eyesPortrait = Retain(UnityEngine.Sprite.Create(texture,
            new UnityEngine.Rect(0, 0, texture.width, texture.height), new UnityEngine.Vector2(.5f, .5f), 100));
    }
    internal static void Dispose()
    {
        portraits?.Dispose(); portraits = null;
        if (eyesPortrait != null) { UnityEngine.Object.Destroy(eyesPortrait.texture); UnityEngine.Object.Destroy(eyesPortrait); }
        eyesPortrait = null;
        foreach (var sprite in thumbnails.Values) { UnityEngine.Object.Destroy(sprite.texture); UnityEngine.Object.Destroy(sprite); }
        thumbnails.Clear();
    }

    internal static void Register()
    {
        var mod = new ModCatalog("fusion", "Plant Fusion");
        Zombies.RegisterCatalog(mod);
        Zombies.RegisterSound("fusion:eyes-rage", FoleyType.NewspaperRarrgh);
        Eyes = Zombies.RegisterZombie(mod, "eyes", new(ZombieType.Normal, MarkerField.GraveX, ScaredContent.ZombieMarker),
            new("Eyes Zombie", "Wakes near plants or enemies, then attacks rapidly. Ordinary deaths have a 1% chance to drop an Eyes Zombie Head.", "Health: 270"),
            new("Augenzombie", "Erwacht nahe Pflanzen oder Gegnern und greift schnell an. Bei gewöhnlichem Tod besteht 1 % Chance auf einen Augenzombie-Kopf.", "Lebenspunkte: 270"), rubberDuck: true);
        Eyes.Initialize = zombie => { zombie.mGraveY = 0; zombie.mBodyHealth = zombie.mBodyMaxHealth = 270; };
        Eyes.Tick = ScaredContent.ZombieTick;
        Eyes.RefreshVisuals = ImportedPeaVisuals.UpdateEyesZombies;
        Eyes.PlacePoolDuck = ImportedPeaVisuals.PlaceEyesPoolDuck;
        Eyes.Portrait = _ => EyesPortrait();
        Eyes.Thumbnail = (_, duck) => Thumbnail("eyes", duck);
        Hypno = Zombies.RegisterZombie(mod, "hypno-pea", new(ZombieType.PeaHead, MarkerField.SummonCounter, HypnoShooters.Summoned),
            new("Hypno Pea Zombie", "A friendly pea zombie summoned by a lethal hypno pea. Its lethal peas summon further allies.", "Health: 270; damage: 20; shot interval: 150 ticks"),
            new("Hypnoerbsen-Zombie", "Verbündeter Erbsenzombie nach tödlichem Hypnoerbsen-Treffer. Tödliche Erbsen erschaffen weitere Verbündete.", "Lebenspunkte: 270; Schaden: 20; Schussabstand: 150 Ticks"), rubberDuck: true);
        Hypno.Initialize = zombie => {
            zombie.mBodyHealth = zombie.mBodyMaxHealth = 270;
            zombie.mPhaseCounter = 50 + zombie.mBoard.Rand(101);
            zombie.StartMindControlled();
        };
        Hypno.Portrait = _ => Thumbnail("hypno-portrait", false);
        Hypno.Thumbnail = (_, duck) => Thumbnail("hypno", duck);
        // These native rigs already had Fusion's pool support. The library now
        // owns their water entry, rendering, clipping and dancer lifecycle.
        foreach (var (type, english, german) in new[] {
            (ZombieType.Newspaper, "Newspaper Zombie", "Zeitungszombie"),
            (ZombieType.Door, "Screen Door Zombie", "Fliegengitterzombie"),
            (ZombieType.Bobsled, "Zombie Bobsled Team", "Zombie-Bobteam"),
            (ZombieType.Balloon, "Balloon Zombie", "Ballonzombie"),
            (ZombieType.Pogo, "Pogo Zombie", "Pogozombie"),
            (ZombieType.Ladder, "Ladder Zombie", "Leiterzombie"),
            (ZombieType.Imp, "Imp", "Wicht"),
            (ZombieType.Target, "Target Zombie", "Zombie-Ziel"),
            (ZombieType.TrashCan, "Trash Can Zombie", "Mülleimerzombie"),
            (ZombieType.Football, "Football Zombie", "Football-Zombie"),
            (ZombieType.JackInTheBox, "Jack-in-the-Box Zombie", "Springteufel-Zombie"),
            (ZombieType.Dancer, "Dancing Zombie", "Tänzer-Zombie"),
            (ZombieType.BackupDancer, "Backup Dancer", "Background-Tänzer"),
            (ZombieType.Yeti, "Zombie Yeti", "Zombie-Yeti"),
            (ZombieType.Gargantuar, "Gargantuar", "Gargantuar"),
            (ZombieType.RedeyeGargantuar, "Giga-gargantuar", "Giga-Gargantuar"),
            (ZombieType.SquashHead, "Squash Zombie", "Kürbis-Zombie") })
        {
            var duck = Zombies.RegisterDuckVariant(mod, "duck-" + type, type,
                new(english, "Native zombie with a ducky tube. Retains native health, attacks and behavior."),
                new(german, "Nativer Zombie mit Gummiente. Lebenspunkte, Angriffe und Verhalten bleiben erhalten."));
            duck.Portrait = _ => (portraits ??= new ReplantedModUi.NativeZombiePortraits()).Get(type);
            duck.Thumbnail = (_, swimming) => Thumbnail(((int)type).ToString(), swimming);
        }
    }
}
