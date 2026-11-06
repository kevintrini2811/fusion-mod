using System.Reflection;
using System.Text.Json;
using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;
using Il2CppSource.Controllers;
using LawnGlove;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace FusionMod;

// The imported prefab is visual data only. Never instantiate the other game's
// scripts, colliders or animation events: native shooter still owns gameplay.
internal static partial class ImportedPeaVisuals
{
    private sealed record Model(Node[] Nodes, Dictionary<int, SpriteData> Sprites,
        Dictionary<string, Clip> Clips, float IdleSpeed, bool ShootAdditive, int GroundNode, Dictionary<string, int[]>? DamageSprites = null,
        Skin[]? Skins = null, Affine[]? Affines = null);
    private sealed record Affine(int Node, int Outer, int Inner, float[] Values);
    private sealed record Node(string Name, string Path, int Parent, bool Active,
        float[] Position, float[] Rotation, float[] Scale, int Sprite, int Order,
        bool FlipX, bool FlipY, float[] Color, bool Enabled = true,
        int MaskSprite = -1, float MaskCutoff = .2f, int MaskInteraction = 0);
    private sealed record SpriteData(string File, float[] Pivot, float PixelsPerUnit);
    private sealed record Clip(float Duration, Binding[] Bindings);
    private sealed record Binding(int Node, int Attribute, Key[][] Channels);
    private sealed record Key(float Time, float[] Coefficients);

    private sealed class Assets
    {
        internal readonly FusionPlant Identity;
        internal readonly string Name;
        internal Model? model;
        internal Bounds PuffPreviewBounds;
        internal float PuffPreviewScale;
        internal float NativeTallHeight;

        internal Assets(FusionPlant identity, string name) { Identity = identity; Name = name; }
        internal Material? spriteMaterial;
        internal readonly Dictionary<int, Sprite> Sprites = new();
        internal readonly List<Texture2D> Textures = new();

        internal Stream Resource(string name) => typeof(ImportedPeaVisuals).Assembly
            .GetManifestResourceStream($"FusionMod.{Name}.{name}")
            ?? throw new InvalidDataException($"Missing embedded Fusion asset: {name}");

        internal void Load(Board board, PreviewController? preview)
        {
            if (model != null && spriteMaterial != null && spriteMaterial
                && Sprites.Values.All(sprite => sprite != null && sprite)) return;
            ReleaseAssets();
            // Reuse the actual game's loaded sprite material. No shader lookup,
            // catalog enumeration, generic asset request or manual bundle loading.
            PreviewController? ownedPreview = null;
            try
            {
                var template = preview ?? board.mCursorObject?.mPreviewController;
                if (FusionPlant.IsPuffVariant(Identity) || Identity.NativeType == SeedType.Tallnut || template == null || !template)
                {
                    // This is the same native factory already used by LawnGlove.
                    // The temporary controller is ours, not the cursor's controller.
                    ownedPreview = board.mApp.CreatePreviewController(Identity.NativeType, board.mCursorObject);
                    template = ownedPreview;
                    if (ownedPreview != null && ownedPreview) ownedPreview.HidePreview();
                }
                var renderer = template?.m_previewDrawer?.m_previewSprite;
                if (FusionPlant.IsPuffVariant(Identity))
                {
                    // Always measure a real Puff-shroom preview. The cursor
                    // may currently show the incoming Peashooter ingredient.
                    var original = template!.m_previewDrawer.CurrentSprite;
                    if (original.Sprite == null || !original.Sprite || !float.IsFinite(original.Scale) || original.Scale <= 0)
                        throw new InvalidDataException("Native Puff-shroom preview has no usable sprite or scale.");
                    PuffPreviewBounds = original.Sprite.bounds;
                    PuffPreviewScale = original.Scale;
                }
                if (Identity.NativeType == SeedType.Tallnut)
                {
                    var original = template!.m_previewDrawer.CurrentSprite;
                    if (original.Sprite == null || !original.Sprite || !float.IsFinite(original.Scale) || original.Scale <= 0)
                        throw new InvalidDataException($"Native {Identity.NativeType} preview has no usable sprite or scale.");
                    NativeTallHeight = original.Sprite.bounds.size.y * original.Scale;
                }
                var sourceMaterial = renderer?.sharedMaterial;
                if (sourceMaterial == null || !sourceMaterial || sourceMaterial.shader == null
                    || !sourceMaterial.shader || !sourceMaterial.shader.isSupported)
                    throw new InvalidOperationException("Native plant preview has no supported sprite material.");
                spriteMaterial = new Material(sourceMaterial)
                {
                    name = $"Fusion{Name}-NativeSprite",
                    hideFlags = HideFlags.HideAndDontSave
                };
                spriteMaterial.SetColor("_Color", Color.white);
                FusionMod.Instance.LoggerInstance.Msg($"{Name} uses native preview material: {spriteMaterial.shader.name}.");
                using var stream = Resource("model.json");
                var loaded = JsonSerializer.Deserialize<Model>(stream)
                    ?? throw new InvalidDataException($"Invalid {Name} visual model.");
                if (loaded.GroundNode >= 0)
                {
                    // The native preview shader blends the texture's pixels, unlike
                    // SpriteRenderer.color which only multiplies those pixels.
                    // Enable its existing variant once; each plant owns its values.
                    if (!spriteMaterial.HasProperty("_BlendColor") || !spriteMaterial.HasProperty("_BlendStrength"))
                        throw new InvalidOperationException("Native sprite material has no colour-blend support.");
                    spriteMaterial.EnableKeyword("_BLEND_ON");
                    spriteMaterial.SetFloat("_BlendOn", 1);
                    spriteMaterial.SetFloat("_BlendStrength", 0);
                }
                foreach (var (id, data) in loaded.Sprites)
                {
                    using var image = Resource(data.File);
                    using var buffer = new MemoryStream();
                    image.CopyTo(buffer);
                    var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    Textures.Add(texture);
                    if (!ImageConversion.LoadImage(texture, buffer.ToArray(), false))
                        throw new InvalidDataException($"Cannot decode Fusion sprite: {data.File}");
                    texture.name = $"Fusion{Name}-{id}";
                    texture.hideFlags = HideFlags.HideAndDontSave;
                    texture.filterMode = FilterMode.Bilinear;
                    texture.wrapMode = TextureWrapMode.Clamp;
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(data.Pivot[0], data.Pivot[1]), data.PixelsPerUnit,
                        0, SpriteMeshType.FullRect);
                    sprite.hideFlags = HideFlags.HideAndDontSave;
                    Sprites.Add(id, sprite);
                }
                PrepareSkinGeometry(loaded, Sprites);
                model = loaded;
            }
            catch { ReleaseAssets(); throw; }
            finally
            {
                if (ownedPreview != null && ownedPreview) ownedPreview.Die();
            }
        }

        internal void ReleaseAssets()
        {
            foreach (var sprite in Sprites.Values) if (sprite) Object.Destroy(sprite);
            Sprites.Clear();
            foreach (var texture in Textures) if (texture) Object.Destroy(texture);
            Textures.Clear();
            if (spriteMaterial != null && spriteMaterial) Object.Destroy(spriteMaterial);
            spriteMaterial = null;
            model = null;
        }

    }

    // <generated-fusion-models>
    private static readonly Assets[] Models =
    {
        new(FusionPlant.BuckShroom, "BuckShroom"),
        new(FusionPlant.SproutPult, "SproutPult"),
        new(FusionPlant.BabyCorn, "BabyCorn"),
        new(FusionPlant.SlicePult, "SlicePult"),
        new(FusionPlant.GnomeShroom, "GnomeShroom"),
        new(FusionPlant.UmbrellaCap, "UmbrellaCap"),
        new(FusionPlant.HailShroom, "HailShroom"),
        new(FusionPlant.IcicleShroom, "IcicleShroom"),
        new(FusionPlant.TwinkleShroom, "TwinkleShroom"),
        new(FusionPlant.NuggetShroom, "NuggetShroom"),
        new(FusionPlant.NutShroom, "NutShroom"),
        new(FusionPlant.BellPepper, "BellPepper"),
        new(FusionPlant.BoomShroom, "BoomShroom"),
        new(FusionPlant.PennyShroom, "PennyShroom"),
        new(FusionPlant.SunnyShroom, "SunnyShroom"),
        new(FusionPlant.StarryShroom, "StarryShroom"),
        new(FusionPlant.SturdyShroom, "SturdyShroom"),
        new(FusionPlant.CamoShroom, "CamoShroom"),
        new(FusionPlant.PeaShroom, "PeaShroom"),
        new(FusionPlant.RepeatShroom, "RepeatShroom"),
        new(FusionPlant.SplitPeaShroom, "SplitPeaShroom"),
        new(FusionPlant.GatlingPeaShroom, "GatlingPeaShroom"),
        new(FusionPlant.SpeedyShroom, "SpeedyShroom"),
        new(FusionPlant.BlastyShroom, "BlastyShroom"),
        new(FusionPlant.ShiveryShroom, "ShiveryShroom"),
        new(FusionPlant.SpuddyShroom, "SpuddyShroom"),
        new(FusionPlant.BlackCherry, "BlackCherry"),
        new(FusionPlant.FrozenCherry, "FrozenCherry"),
        new(FusionPlant.DemiseShroom, "DemiseShroom"),
        new(FusionPlant.SundaeShroom, "SundaeShroom"),
        new(FusionPlant.DoomSquash, "DoomSquash"),
        new(FusionPlant.Jawbreaker, "Jawbreaker"),
        new(FusionPlant.DoomChomper, "DoomChomper"),
        new(FusionPlant.Frostbiter, "Frostbiter"),
        new(FusionPlant.FrostShroom, "FrostShroom"),
        new(FusionPlant.FlameShroom, "FlameShroom"),
        new(FusionPlant.SootShroom, "SootShroom"),
        new(FusionPlant.FumeSpreader, "FumeSpreader"),
        new(FusionPlant.FoulShroom, "FoulShroom"),
        new(FusionPlant.GloomShroom, "GloomShroom"),
        new(FusionPlant.FrostGloomShroom, "FrostGloomShroom"),
        new(FusionPlant.FlameGloomShroom, "FlameGloomShroom"),
        new(FusionPlant.GutsyShroom, "GutsyShroom"),
        new(FusionPlant.StarchShroom, "StarchShroom"),
        new(FusionPlant.Icerock, "Icerock"),
        new(FusionPlant.Hearthrock, "Hearthrock"),
        new(FusionPlant.StarrySquash, "StarrySquash"),
        new(FusionPlant.SolarSquash, "SolarSquash"),
        new(FusionPlant.SpicyKelp, "SpicyKelp"),
        new(FusionPlant.DoomKelp, "DoomKelp"),
        new(FusionPlant.ButterNut, "ButterNut"),
        new(FusionPlant.UmbrellaHusk, "UmbrellaHusk"),
        new(FusionPlant.Torchflower, "Torchflower"),
        new(FusionPlant.SilverNut, "SilverNut"),
        new(FusionPlant.SilverGarlic, "SilverGarlic"),
        new(FusionPlant.SilverIceShroom, "SilverIceShroom"),
        new(FusionPlant.SilverSunflower, "SilverSunflower"),
        new(FusionPlant.SilverDoomShroom, "SilverDoomShroom"),
        new(FusionPlant.SilverHypnoShroom, "SilverHypnoShroom"),
        new(FusionPlant.SilverCabbage, "SilverCabbage"),
        new(FusionPlant.SilverMelon, "SilverMelon"),
        new(FusionPlant.SolarPot, "SolarPot"),
        new(FusionPlant.SunsetShroom, "SunsetShroom"),
        new(FusionPlant.SolarFlare, "SolarFlare"),
        new(FusionPlant.SolarBlover, "SolarBlover"),
        new(FusionPlant.LandMine, "LandMine"),
        new(FusionPlant.FoulNut, "FoulNut"),
        new(FusionPlant.StarNut, "StarNut"),
        new(FusionPlant.PeaSquash, "PeaSquash"),
        new(FusionPlant.IronSquash, "IronSquash"),
        new(FusionPlant.StenchSpreader, "StenchSpreader"),
        new(FusionPlant.SnowThreepeater, "SnowThreepeater"),
        new(FusionPlant.CherrySpreader, "CherrySpreader"),
        new(FusionPlant.ScorchedThreepeater, "ScorchedThreepeater"),
        new(FusionPlant.CharredThreepeater, "CharredThreepeater"),
        new(FusionPlant.FalloutBlover, "FalloutBlover"),
        new(FusionPlant.Solarfruit, "Solarfruit"),
        new(FusionPlant.HypnoGarlic, "HypnoGarlic"),
        new(FusionPlant.Flytrapper, "Flytrapper"),
        new(FusionPlant.HypnoPea, "HypnoPea"),
        new(FusionPlant.HypnoRepeater, "HypnoRepeater"),
        new(FusionPlant.SplitHypno, "SplitHypno"),
        new(FusionPlant.GatlingHypno, "GatlingHypno"),
        new(FusionPlant.PoisonPea, "PoisonPea"),
        new(FusionPlant.PoisonRepeater, "PoisonRepeater"),
        new(FusionPlant.SplitPoison, "SplitPoison"),
        new(FusionPlant.GatlingPoison, "GatlingPoison"),
        new(FusionPlant.ThornNut, "ThornNut"),
        new(FusionPlant.IceMine, "IceMine"),
        new(FusionPlant.HypnoMine, "HypnoMine"),
        new(FusionPlant.ChipMine, "ChipMine"),
        new(FusionPlant.BlazePea, "BlazePea"),
        new(FusionPlant.BlazeRepeater, "BlazeRepeater"),
        new(FusionPlant.SplitBlaze, "SplitBlaze"),
        new(FusionPlant.GatlingBlaze, "GatlingBlaze"),
        new(FusionPlant.Solarweed, "Solarweed"),
        new(FusionPlant.Iceweed, "Iceweed"),
        new(FusionPlant.Scorchweed, "Scorchweed"),
        new(FusionPlant.SpikeNut, "SpikeNut"),
        new(FusionPlant.FlameWallNut, "FlameWallNut"),
        new(FusionPlant.FrostWallNut, "FrostWallNut"),
        new(FusionPlant.FlameTallNut, "FlameTallNut"),
        new(FusionPlant.FrostTallNut, "FrostTallNut"),
        new(FusionPlant.CherrySquash, "CherrySquash"),
        new(FusionPlant.SpicySquash, "SpicySquash"),
        new(FusionPlant.IcySquash, "IcySquash"),
        new(FusionPlant.HypnoSquash, "HypnoSquash"),
        new(FusionPlant.TwinMarigold, "TwinMarigold"),
        new(FusionPlant.UmbrellaShell, "UmbrellaShell"),
        new(FusionPlant.ChompMine, "ChompMine"),
        new(FusionPlant.SolarChomper, "SolarChomper"),
        new(FusionPlant.SolarNut, "SolarNut"),
        new(FusionPlant.TwinSolarNut, "TwinSolarNut"),
        new(FusionPlant.PeatatoMine, "PeatatoMine"),
        new(FusionPlant.SolarMine, "SolarMine"),
        new(FusionPlant.MineNut, "MineNut"),
        new(FusionPlant.Bucket, "Bucket"),
        new(FusionPlant.FootballHelmet, "FootballHelmet"),
        new(FusionPlant.FootballNut, "FootballNut"),
        new(FusionPlant.FootballGatling, "FootballGatling"),
        new(FusionPlant.BuckNut, "BuckNut"),
        new(FusionPlant.Buckshooter, "Buckshooter"),
        new(FusionPlant.PeaNut, "PeaNut"),
        new(FusionPlant.ChompShooter, "ChompShooter"),
        new(FusionPlant.ChompNut, "ChompNut"),
        new(FusionPlant.Chompzilla, "Chompzilla"),
        new(FusionPlant.CherryChomper, "CherryChomper"),
        new(FusionPlant.CherryNut, "CherryNut"),
        new(FusionPlant.SolarBomb, "SolarBomb"),
        new(FusionPlant.ClusterMine, "ClusterMine"),
        new(FusionPlant.SnowRepeater, "SnowRepeater"),
        new(FusionPlant.SnowSplit, "SnowSplit"),
        new(FusionPlant.GatlingSnow, "GatlingSnow"),
        new(FusionPlant.SolarShooter, "SolarShooter"),
        new(FusionPlant.CherryShooter, "CherryShooter"),
        new(FusionPlant.Cherrypeater, "Cherrypeater"),
        new(FusionPlant.SplitCherry, "SplitCherry"),
        new(FusionPlant.GatlingCherry, "GatlingCherry"),
        new(FusionPlant.ExplodeOShooter, "ExplodeOShooter"),
        new(FusionPlant.SilverPot, "SilverPot"),
        new(FusionPlant.SilverUmbrella, "SilverUmbrella"),
        new(FusionPlant.SilverKernel, "SilverKernel"),
        new(FusionPlant.LumosPot, "LumosPot"),
        new(FusionPlant.CabbagePot, "CabbagePot"),
        new(FusionPlant.ButterPot, "ButterPot"),
        new(FusionPlant.UmbrellaPot, "UmbrellaPot"),
        new(FusionPlant.NutPot, "NutPot"),
        new(FusionPlant.GarlicPot, "GarlicPot"),
        new(FusionPlant.SpikePot, "SpikePot"),
        new(FusionPlant.MelonPot, "MelonPot"),
        new(FusionPlant.CherryPot, "CherryPot"),
        new(FusionPlant.HotPot, "HotPot"),
        new(FusionPlant.HypnoPot, "HypnoPot"),
        new(FusionPlant.DoomPot, "DoomPot"),
        new(FusionPlant.PeaPot, "PeaPot"),
        new(FusionPlant.TriPot, "TriPot"),
        new(FusionPlant.KernelTriPult, "KernelTriPult"),
        new(FusionPlant.MelonTriPult, "MelonTriPult"),
        new(FusionPlant.TacoPult, "TacoPult"),
        new(FusionPlant.CobPult, "CobPult"),
        new(FusionPlant.OverripeMelon, "OverripeMelon"),
        new(FusionPlant.CabbageTriPult, "CabbageTriPult"),
        new(FusionPlant.Coldslaw, "Coldslaw"),
        new(FusionPlant.KimchiPult, "KimchiPult"),
        new(FusionPlant.Cobsicle, "Cobsicle"),
        new(FusionPlant.SaladPult, "SaladPult"),
        new(FusionPlant.CabbageNut, "CabbageNut"),
        new(FusionPlant.MelonNut, "MelonNut"),
        new(FusionPlant.ThreeNuts, "ThreeNuts"),
        new(FusionPlant.SquashNut, "SquashNut"),
        new(FusionPlant.TingleKelp, "TingleKelp"),
        new(FusionPlant.GrappleKelp, "GrappleKelp"),
        new(FusionPlant.TangleNut, "TangleNut"),
        new(FusionPlant.SpikeKelp, "SpikeKelp"),
        new(FusionPlant.EyesZombieHead, "EyesZombieHead"),
        new(FusionPlant.BloodSunflower, "BloodSunflower"),
        new(FusionPlant.Cyclover, "Cyclover"),
        new(FusionPlant.Disguster, "Disguster"),
        new(FusionPlant.BreezeBlover, "BreezeBlover"),
        new(FusionPlant.MindBlover, "MindBlover"),
        new(FusionPlant.BoostShroom, "BoostShroom"),
        new(FusionPlant.BlowtorchShroom, "BlowtorchShroom"),
        new(FusionPlant.WallopShroom, "WallopShroom"),
        new(FusionPlant.CaltropShroom, "CaltropShroom"),
        new(FusionPlant.GarbagePult, "GarbagePult"),
        new(FusionPlant.ClovePult, "ClovePult"),
        new(FusionPlant.GarlicPult, "GarlicPult"),
        new(FusionPlant.SummerMelon, "SummerMelon"),
        new(FusionPlant.Starlic, "Starlic"),
        new(FusionPlant.WinterStarfruit, "WinterStarfruit"),
        new(FusionPlant.SpicyStarfruit, "SpicyStarfruit"),
        new(FusionPlant.Starshooter, "Starshooter"),
        new(FusionPlant.FrostCattail, "FrostCattail"),
        new(FusionPlant.FlameCattail, "FlameCattail"),
        new(FusionPlant.HypnoMelon, "HypnoMelon"),
        new(FusionPlant.HypnoPepper, "HypnoPepper"),
        new(FusionPlant.MinnowShroom, "MinnowShroom"),
        new(FusionPlant.UrchinShroom, "UrchinShroom"),
        new(FusionPlant.SharkShroom, "SharkShroom"),
        new(FusionPlant.SeaStar, "SeaStar"),
        new(FusionPlant.BeaconShroom, "BeaconShroom"),
        new(FusionPlant.JellyShroom, "JellyShroom"),
        new(FusionPlant.SwellShroom, "SwellShroom"),
        new(FusionPlant.HermitShroom, "HermitShroom"),
        new(FusionPlant.StarryPumpkin, "StarryPumpkin"),
        new(FusionPlant.ThornyPumpkin, "ThornyPumpkin"),
        new(FusionPlant.PuffKin, "PuffKin"),
        new(FusionPlant.FumingPumpkin, "FumingPumpkin"),
        new(FusionPlant.CherryPumpkin, "CherryPumpkin"),
        new(FusionPlant.StinkyPumpkin, "StinkyPumpkin"),
        new(FusionPlant.ScarelessPumpkin, "ScarelessPumpkin"),
        new(FusionPlant.NutShell, "NutShell"),
        new(FusionPlant.WindyPumpkin, "WindyPumpkin"),
        new(FusionPlant.SpikyPumpkin, "SpikyPumpkin"),
        new(FusionPlant.JackOPlantern, "JackOPlantern"),
        new(FusionPlant.SeaShell, "SeaShell"),
        new(FusionPlant.LettucePult, "LettucePult"),
        new(FusionPlant.ButternutPult, "ButternutPult"),
        new(FusionPlant.GourdPult, "GourdPult"),
        new(FusionPlant.SpringMelon, "SpringMelon"),
        new(FusionPlant.DormantShroom, "DormantShroom"),
        new(FusionPlant.MorphShroom, "MorphShroom"),
        new(FusionPlant.MelonMorph, "MelonMorph"),
        new(FusionPlant.Starmorph, "Starmorph"),
        new(FusionPlant.SaguaroShroom, "SaguaroShroom"),
        new(FusionPlant.IceCactus, "IceCactus"),
        new(FusionPlant.MagnetCactus, "MagnetCactus"),
        new(FusionPlant.IllumiNut, "IllumiNut"),
        new(FusionPlant.DoomGarlic, "DoomGarlic"),
        new(FusionPlant.DoomNut, "DoomNut"),
        new(FusionPlant.Doomflower, "Doomflower"),
        new(FusionPlant.CraterTot, "CraterTot"),
        new(FusionPlant.DoomSeed, "DoomSeed"),
        new(FusionPlant.PumpkinPod, "PumpkinPod"),
        new(FusionPlant.MelonShell, "MelonShell"),
        new(FusionPlant.CryoPumpkin, "CryoPumpkin"),
        new(FusionPlant.PyroPumpkin, "PyroPumpkin"),
        new(FusionPlant.KernelMorph, "KernelMorph"),
        new(FusionPlant.RustShroom, "RustShroom"),
        new(FusionPlant.MetalPult, "MetalPult"),
        new(FusionPlant.IronStar, "IronStar"),
        new(FusionPlant.DoomPea, "DoomPea"),
        new(FusionPlant.GatlingDoom, "GatlingDoom"),
        new(FusionPlant.DoomSpreader, "DoomSpreader"),
        new(FusionPlant.DeathStar, "DeathStar"),
        new(FusionPlant.UmbrellaClove, "UmbrellaClove"),
        new(FusionPlant.UmbrellaThorn, "UmbrellaThorn"),
        new(FusionPlant.DamagePult, "DamagePult"),
        new(FusionPlant.StarburstShroom, "StarburstShroom"),
        new(FusionPlant.RayShroom, "RayShroom"),
        new(FusionPlant.BlubberShroom, "BlubberShroom"),
        new(FusionPlant.SunriseShroom, "SunriseShroom"),
        new(FusionPlant.BushShroom, "BushShroom"),
        new(FusionPlant.Beamshooter, "Beamshooter"),
        new(FusionPlant.Rebeamer, "Rebeamer"),
        new(FusionPlant.SplitBeam, "SplitBeam"),
        new(FusionPlant.GatlingBeam, "GatlingBeam"),
        new(FusionPlant.Starthorn, "Starthorn"),
        new(FusionPlant.DoomCactus, "DoomCactus"),
        new(FusionPlant.LumosCactus, "LumosCactus"),
        new(FusionPlant.Thornweed, "Thornweed"),
        new(FusionPlant.Butterweed, "Butterweed"),
        new(FusionPlant.Springweed, "Springweed"),
        new(FusionPlant.Splash, "Splash"),
        new(FusionPlant.SeaNut, "SeaNut"),
        new(FusionPlant.StrangleKelp, "StrangleKelp"),
        new(FusionPlant.KelpSpreader, "KelpSpreader"),
        new(FusionPlant.SquashSpreader, "SquashSpreader"),
        new(FusionPlant.Squashweed, "Squashweed"),
        new(FusionPlant.GoldenNut, "GoldenNut"),
        new(FusionPlant.GoldenSunflower, "GoldenSunflower"),
        new(FusionPlant.TangleMine, "TangleMine"),
        new(FusionPlant.KelpMine, "KelpMine"),
        new(FusionPlant.SeaMine, "SeaMine"),
        new(FusionPlant.PotatoMasher, "PotatoMasher"),
        new(FusionPlant.PotatoMash, "PotatoMash"),
        new(FusionPlant.PerfumeShroom, "PerfumeShroom"),
        new(FusionPlant.TangleWeed, "TangleWeed"),
        new(FusionPlant.Glowver, "Glowver"),
        new(FusionPlant.FrenzyShroom, "FrenzyShroom"),
        new(FusionPlant.PhoenixThreepeater, "PhoenixThreepeater"),
        new(FusionPlant.SolarPult, "SolarPult"),
        new(FusionPlant.Triplantern, "Triplantern"),
        new(FusionPlant.AuroraPlantern, "AuroraPlantern"),
        new(FusionPlant.BloverPot, "BloverPot"),
        new(FusionPlant.ScaredyPot, "ScaredyPot"),
        new(FusionPlant.Multipeater, "Multipeater"),
        new(FusionPlant.SeaPot, "SeaPot"),
        new(FusionPlant.CabbageDropper, "CabbageDropper"),
        new(FusionPlant.ButterFly, "ButterFly"),
        new(FusionPlant.CherryPop, "CherryPop"),
        new(FusionPlant.BallCactus, "BallCactus"),
        new(FusionPlant.BelchShroom, "BelchShroom"),
        new(FusionPlant.HungryShroom, "HungryShroom"),
        new(FusionPlant.TriggerShroom, "TriggerShroom"),
        new(FusionPlant.Spicicle, "Spicicle"),
        new(FusionPlant.Chompkin, "Chompkin"),
        new(FusionPlant.PumpkinMine, "PumpkinMine"),
        new(FusionPlant.SunnyPumpkin, "SunnyPumpkin"),
        new(FusionPlant.GlacierShroom, "GlacierShroom"),
        new(FusionPlant.ThreetatoMine, "ThreetatoMine"),
        new(FusionPlant.MarigoldMine, "MarigoldMine"),
        new(FusionPlant.UmbrellaKale, "UmbrellaKale"),
        new(FusionPlant.UmbrellaRind, "UmbrellaRind"),
        new(FusionPlant.LumosUmbrella, "LumosUmbrella"),
        new(FusionPlant.IcePot, "IcePot"),
        new(FusionPlant.RoastedSquash, "RoastedSquash"),
        new(FusionPlant.RoastedWallNut, "RoastedWallNut"),
        new(FusionPlant.RoastedPumpkin, "RoastedPumpkin"),
        new(FusionPlant.RoastedPotato, "RoastedPotato"),
        new(FusionPlant.TideShroom, "TideShroom"),
        new(FusionPlant.BlacklightPlantern, "BlacklightPlantern"),
        new(FusionPlant.Scorchwood, "Scorchwood"),
        new(FusionPlant.Stenchwood, "Stenchwood"),
        new(FusionPlant.Nutwood, "Nutwood"),
        new(FusionPlant.Torchthree, "Torchthree"),
        new(FusionPlant.Sparkwood, "Sparkwood"),
        new(FusionPlant.Hypnowood, "Hypnowood"),
        new(FusionPlant.HotPotato, "HotPotato"),
        new(FusionPlant.Boomwood, "Boomwood"),
    };
    // </generated-fusion-models>
    private static GameObject? bucketCursor;
    internal static void ClearBucketCursor()
    {
        if (bucketCursor != null && bucketCursor) Object.Destroy(bucketCursor);
        bucketCursor = null;
    }
    private static int DropSprite(Assets assets) => assets.model!.Nodes.First(n => n.Name == "Body").Sprite;
    internal static void ShowBucketCursor(Board board, FusionPlant identity = default)
    {
        var cursor = board.mCursorObject?.mController;
        if (cursor == null || !cursor) return;
        if (bucketCursor != null && bucketCursor) return;
        var assets = Models.First(a => a.Identity == (identity == default ? FusionPlant.Bucket : identity));
        assets.Load(board, null);
        bucketCursor = new GameObject("Fusion-BucketCursor") { layer = cursor.gameObject.layer };
        bucketCursor.transform.SetParent(cursor.transform, false);
        // Original BucketPlant Body; cursor uses the same 2.82 world units
        // per board pixel as native plant controllers.
        var renderer = bucketCursor.AddComponent<SpriteRenderer>();
        renderer.sprite = assets.Sprites[DropSprite(assets)];
        renderer.sharedMaterial = assets.spriteMaterial;
        renderer.sortingOrder = 32760;
        bucketCursor.transform.localScale = Vector3.one * (55f * 2.82f / renderer.sprite.bounds.size.x);
    }
    // Mesh.bounds contains Spine's enormous culling sentinels. Only actual
    // finite artwork vertices may determine imported plant/head dimensions.
    private static Bounds? VisibleMeshBounds(MeshRenderer mesh, Transform parent)
    {
        var geometry = mesh.GetComponent<MeshFilter>()?.sharedMesh;
        if (geometry == null) return null;
        Bounds? result = null;
        foreach (var vertex in geometry.vertices)
        {
            if (!float.IsFinite(vertex.x) || !float.IsFinite(vertex.y)
                || Math.Abs(vertex.x) > 20000 || Math.Abs(vertex.y) > 20000) continue;
            var point = parent.InverseTransformPoint(mesh.transform.TransformPoint(vertex));
            if (result == null) result = new Bounds(point, Vector3.zero);
            else { var bounds = result.Value; bounds.Encapsulate(point); result = bounds; }
        }
        return result is { } box && box.size.x > 0 && box.size.y > 0 ? result : null;
    }

    private static readonly Dictionary<IntPtr, View> Views = new();
    [ThreadStatic] internal static Plant? PreviewIngredient;

    internal static void Register(Plant plant, PlantController controller)
    {
        if (controller == null || !controller) return;
        Add(new View(plant, controller, false));
    }

    internal static (float X, float Y) PeatatoMuzzle(Plant plant)
    {
        if (plant.mController != null && Views.TryGetValue(plant.mController.Pointer, out var view)
            && view.IsValid && view.ShootPoint() is { } local)
            return (plant.mX + plant.mWidth / 2f + local.x / 2.82f,
                plant.mY + plant.mHeight - local.y / 2.82f);
        return (plant.mX + plant.mWidth / 2f,
            plant.mY + (plant.mState == PlantState.PotatoArmed ? -5 : plant.mHeight - 25));
    }

    internal static void CaptureHighlight(Plant plant)
    {
        var controller = plant.mController;
        if (controller != null && Views.TryGetValue(controller.Pointer, out var view)
            && view.IsValid) view.Highlighted = plant.mHighlighted;
    }

    internal static void RegisterPreview(PreviewController controller)
    {
        if (PreviewIngredient != null && controller != null && controller)
            Add(new View(PreviewIngredient, controller, true));
    }

    internal static void RegisterDebugPreview(PreviewController controller, Board board, FusionPlant identity)
    {
        if (!FusionPlant.HasImportedVisuals(identity) || controller == null || !controller) return;
        // Detached visual identity only: no PlantInitialize, pool insertion,
        // gameplay abilities, saving, loot or recipe side effects.
        var presentation = new Plant
        {
            mApp = board.mApp, mBoard = board, mSeedType = identity.NativeType,
            mImitaterType = identity.Marker, mVisible = true,
            mPlantHealth = 300, mPlantMaxHealth = 300, mState = PlantState.Ready
        };
        Add(new View(presentation, controller, true));
    }

    private static void Add(View view)
    {
        if (Views.Remove(view.Controller.Pointer, out var previous)) previous.Dispose();
        Views[view.Controller.Pointer] = view;
    }

    internal static void Forget(Plant plant)
    {
        foreach (var entry in Views.Where(e => e.Value.Plant.Pointer == plant.Pointer).ToArray())
        { entry.Value.Dispose(); Views.Remove(entry.Key); }
    }

    internal static void ForgetController(ReloadedController controller)
    {
        if (Views.Remove(controller.Pointer, out var view)) view.Dispose();
    }

    internal static void ForgetBoard(Board board)
    {
        ClearFalloutSplats(board);
        ReportedProjectileBoards.RemoveWhere(entry => entry.Board == board.Pointer);
        foreach (var entry in ImportedProjectiles.Where(e => e.Value.Board == board.Pointer).ToArray())
        { entry.Value.Dispose(); ImportedProjectiles.Remove(entry.Key); }
        foreach (var entry in Views.Where(e => e.Value.Board == board.Pointer).ToArray())
        { entry.Value.Dispose(); Views.Remove(entry.Key); }
    }

    internal static void Update()
    {
        UpdateFalloutSplats();
        UpdateImportedProjectiles();
        foreach (var (key, view) in Views.ToArray())
        {
            try
            {
                if (view.IsValid) { view.Update(); continue; }
            }
            catch (Exception error)
            {
                // Restore native rendering on failure and report once per view.
                FusionMod.Instance.LoggerInstance.Error($"Fusion pea art failed: {error}");
            }
            view.Dispose();
            Views.Remove(key);
        }
    }

    internal static void Dispose()
    {
        ClearFalloutSplats();
        foreach (var view in Views.Values) view.Dispose();
        Views.Clear();
        foreach (var view in ImportedProjectiles.Values) view.Dispose();
        ImportedProjectiles.Clear();
        ReportedProjectileBoards.Clear();
        ClearBucketCursor();
        SpeedyProjectileAssets.ReleaseAssets();
        ThawedShiveryProjectileAssets.ReleaseAssets();
        CherrySporeAssets.ReleaseAssets();
        PotatoSporeAssets.ReleaseAssets();
        ShiveryProjectileAssets.ReleaseAssets();
        PlainPeaProjectileAssets.ReleaseAssets();
        PlainStarProjectileAssets.ReleaseAssets();
        StarCapProjectileAssets.ReleaseAssets();
        DoomCabbageProjectileAssets.ReleaseAssets();
        PhoenixProjectileAssets.ReleaseAssets();
        SolarCabbageProjectileAssets.ReleaseAssets();
        DoomPeaProjectileAssets.ReleaseAssets();
        DoomProjectileAssets.ReleaseAssets();
        BigDoomProjectileAssets.ReleaseAssets();
        DoomMeteorProjectileAssets.ReleaseAssets();
        HypnoProjectileAssets.ReleaseAssets();
        HypnoFireProjectileAssets.ReleaseAssets();
        PoisonProjectileAssets.ReleaseAssets();
        StenchFireProjectileAssets.ReleaseAssets();
        ScorchProjectileAssets.ReleaseAssets();
        FireCherryProjectileAssets.ReleaseAssets();
        CherryDroneProjectileAssets.ReleaseAssets();
        SmallFireProjectileAssets.ReleaseAssets();
        PotatoFireProjectileAssets.ReleaseAssets();
        FireNutProjectileAssets.ReleaseAssets();
        PotatoProjectileAssets.ReleaseAssets();
        IronProjectileAssets.ReleaseAssets();
        SmallCabbageProjectileAssets.ReleaseAssets();
        SmallIronProjectileAssets.ReleaseAssets();
        SmallKernelProjectileAssets.ReleaseAssets();
        SmallButterProjectileAssets.ReleaseAssets();
        SmallMelonProjectileAssets.ReleaseAssets();
        SilverCabbageProjectileAssets.ReleaseAssets();
        SilverMelonProjectileAssets.ReleaseAssets();
        SilverKernelProjectileAssets.ReleaseAssets();
        SilverButterProjectileAssets.ReleaseAssets();
        ColdslawProjectileAssets.ReleaseAssets();
        KimchiProjectileAssets.ReleaseAssets();
        IceKernelProjectileAssets.ReleaseAssets();
        IceButterProjectileAssets.ReleaseAssets();
        SaladProjectileAssets.ReleaseAssets();
        GarlicCabbageProjectileAssets.ReleaseAssets();
        GarlicKernelProjectileAssets.ReleaseAssets();
        GarlicButterProjectileAssets.ReleaseAssets();
        GarlicMelonProjectileAssets.ReleaseAssets();
        FireMelonProjectileAssets.ReleaseAssets();
        HypnoMelonProjectileAssets.ReleaseAssets();
        IceTrackProjectileAssets.ReleaseAssets();
        FireTrackProjectileAssets.ReleaseAssets();
        SeaSporeProjectileAssets.ReleaseAssets();
        SeaSpikeProjectileAssets.ReleaseAssets();
        DoomCactusProjectileAssets.ReleaseAssets();
        LightSpikeProjectileAssets.ReleaseAssets();
        LightPierceProjectileAssets.ReleaseAssets();
        CactusStarProjectileAssets.ReleaseAssets();
        CactusSpikeProjectileAssets.ReleaseAssets();
        IceCactusProjectileAssets.ReleaseAssets();
        MagnetCactusProjectileAssets.ReleaseAssets();
        SeaStarProjectileAssets.ReleaseAssets();

        TacoProjectileAssets.ReleaseAssets();
        ButterTacoProjectileAssets.ReleaseAssets();
        CobProjectileAssets.ReleaseAssets();
        ButterCobProjectileAssets.ReleaseAssets();
        OverripeProjectileAssets.ReleaseAssets();
        TriCabbageProjectileAssets.ReleaseAssets();
        CabbageProjectileAssets.ReleaseAssets();
        KernelProjectileAssets.ReleaseAssets();
        ButterProjectileAssets.ReleaseAssets();
        MelonProjectileAssets.ReleaseAssets();
        NutProjectileAssets.ReleaseAssets();
        ChompProjectileAssets.ReleaseAssets();
        SolarNutBallAssets.ReleaseAssets();
        SolarProjectileAssets.ReleaseAssets();
        CherryProjectileAssets.ReleaseAssets();
        CherrySpreaderProjectileAssets.ReleaseAssets();
        CherrySpreaderBombProjectileAssets.ReleaseAssets();
        KelpProjectileAssets.ReleaseAssets();
        MiniSquashProjectileAssets.ReleaseAssets();
        ExplodeProjectileAssets.ReleaseAssets();
        foreach (var assets in Models) assets.ReleaseAssets();
    }

    private static Vector3 Vector(float[] v) => new(v[0], v[1], v[2]);

    private static float Evaluate(Key[] keys, float time)
    {
        int low = 0, high = keys.Length - 1;
        while (low < high)
        {
            int mid = (low + high + 1) / 2;
            if (keys[mid].Time <= time) low = mid; else high = mid - 1;
        }
        var key = keys[low];
        float dt = Math.Max(0, time - key.Time);
        var c = key.Coefficients;
        return ((c[0] * dt + c[1]) * dt + c[2]) * dt + c[3];
    }

    private static void Pose(Model data, Transform[] transforms, float idleTime, float? shooting,
        SpriteRenderer?[]? renderers = null, Dictionary<int, Sprite>? sprites = null, string baseClip = "idle", string shootClip = "shoot")
    {
        for (int i = 0; i < data.Nodes.Length; ++i)
        {
            var node = data.Nodes[i]; var transform = transforms[i];
            transform.localPosition = Vector(node.Position);
            transform.localRotation = new Quaternion(node.Rotation[0], node.Rotation[1], node.Rotation[2], node.Rotation[3]);
            transform.localScale = Vector(node.Scale);
            transform.gameObject.SetActive(node.Active);
            if (renderers?[i] is { } part)
            {
                part.enabled = node.Enabled;
                if (data.Affines != null && sprites != null && node.Sprite >= 0) part.sprite = sprites[node.Sprite];
            }
        }
        Apply(transforms, data.Clips[baseClip], idleTime % data.Clips[baseClip].Duration, false);
        if (data.Clips.ContainsKey("head1"))
        {
            // Each original head is an independent additive layer. Its idle
            // and shooting states are alternatives, not two stacked clips.
            for (int head = 1; head <= 3; ++head)
            {
                var clip = data.Clips[$"{(shooting.HasValue ? "shoot" : "head")}{head}"];
                Apply(transforms, clip, shooting ?? idleTime % clip.Duration, true);
            }
        }
        else if (shooting.HasValue) Apply(transforms, data.Clips[shootClip], shooting.Value, data.ShootAdditive);
        if (data.Clips.TryGetValue("back", out var back))
            Apply(transforms, back, idleTime % back.Duration, true);
        PoseAffines(data, transforms, baseClip, idleTime);
        if (renderers != null && sprites != null)
        {
            for (int i = 0; i < renderers.Length; ++i)
                if (renderers[i] is { } renderer) renderer.sortingOrder = data.Nodes[i].Order;
            foreach (var binding in data.Clips[baseClip].Bindings.Where(b => b.Attribute is 5 or 6 or 12))
                if (renderers[binding.Node] is { } renderer)
                {
                    float value = Evaluate(binding.Channels[0], idleTime % data.Clips[baseClip].Duration);
                    if (binding.Attribute == 5) renderer.sprite = sprites[(int)value];
                    else if (binding.Attribute == 12) renderer.sortingOrder = (int)value;
                    else renderer.enabled = value >= .5f;
                }
        }
    }

    private static float AnimatedAlpha(Model model, int node, string baseClip, float time, string shootClip, float? shooting)
    {
        float alpha = model.Nodes[node].Color[3];
        foreach (var binding in model.Clips[baseClip].Bindings)
            if (binding.Node == node && binding.Attribute == 7)
                alpha = Evaluate(binding.Channels[0], time % model.Clips[baseClip].Duration);
        if (model.Clips.ContainsKey("head1"))
        {
            // The three-head renderer uses shoot1/2/3, never a single shoot
            // clip. Alpha must follow the same active layers as Pose.
            for (int head = 1; head <= 3; ++head)
            {
                var clip = model.Clips[$"{(shooting.HasValue ? "shoot" : "head")}{head}"];
                foreach (var binding in clip.Bindings)
                    if (binding.Node == node && binding.Attribute == 7)
                        alpha = Evaluate(binding.Channels[0], shooting ?? time % clip.Duration);
            }
        }
        else if (shooting.HasValue)
            foreach (var binding in model.Clips[shootClip].Bindings)
                if (binding.Node == node && binding.Attribute == 7)
                    alpha = Evaluate(binding.Channels[0], shooting.Value);
        return Math.Clamp(alpha, 0, 1);
    }

    private static void Apply(Transform[] transforms, Clip clip, float time, bool additive)
    {
        foreach (var binding in clip.Bindings)
        {
            var transform = transforms[binding.Node]; var channels = binding.Channels;
            if (binding.Attribute == 0)
            { transform.gameObject.SetActive(Evaluate(channels[0], time) >= 0.5f); continue; }
            if (binding.Attribute >= 5) continue; // Sprite references are applied by the visual owner.
            var value = new Vector3(Evaluate(channels[0], time), Evaluate(channels[1], time), Evaluate(channels[2], time));
            var reference = additive ? new Vector3(Evaluate(channels[0], 0),
                Evaluate(channels[1], 0), Evaluate(channels[2], 0)) : Vector3.zero;
            switch (binding.Attribute)
            {
                case 1:
                    transform.localPosition = additive ? transform.localPosition + value - reference : value;
                    break;
                case 3:
                    transform.localScale = additive ? Vector3.Scale(transform.localScale,
                        new Vector3(value.x / reference.x, value.y / reference.y, value.z / reference.z)) : value;
                    break;
                case 4:
                    transform.localRotation = additive ? transform.localRotation
                        * Quaternion.Inverse(Quaternion.Euler(reference)) * Quaternion.Euler(value)
                        : Quaternion.Euler(value);
                    break;
            }
        }
    }


    private sealed partial class View : IDisposable
    {
        internal readonly Plant Plant;
        internal bool Highlighted;
        internal readonly ReloadedController Controller;
        internal readonly IntPtr Board;
        private readonly uint id;
        private readonly bool preview;
        private readonly Assets assets;
        private readonly MaterialPropertyBlock colorEffects = new();
        private Model model => assets.model!;
        private GameObject? root;
        private Vector3 doomGround;
        private SortingGroup? sorting;
        private Renderer? native;
        private bool nativeWasEnabled;
        internal Vector3? ShootPoint()
        {
            if (root == null || model == null) return null;
            int index = Array.FindIndex(model.Nodes, node => node.Path == "PotatoMine_light1/Shoot");
            return index >= 0 && index < transforms.Length
                ? root.transform.parent.InverseTransformPoint(transforms[index].position) : null;
        }

        private bool ownsNative;
        private Transform[] transforms = Array.Empty<Transform>();
        private SpriteRenderer?[] renderers = Array.Empty<SpriteRenderer?>();
        private readonly List<(Renderer Renderer, bool Enabled)> shadows = new();
        private readonly Dictionary<IntPtr, (Renderer Renderer, bool Enabled)> nativeParts = new();

        internal View(Plant plant, ReloadedController controller, bool preview)
        {
            Plant = plant; Controller = controller; this.preview = preview;
            assets = Models.Single(data => data.Identity == FusionPlant.From(plant));
            id = plant.DataID; Board = plant.mBoard?.Pointer ?? IntPtr.Zero;
        }

        internal bool IsValid => Controller && !Plant.mDead && Plant.DataID == id
            && Plant.mBoard?.Pointer == Board && FusionPlant.From(Plant) == assets.Identity
            && (preview || Plant.mController?.Pointer == Controller.Pointer);

        private bool Create()
        {
            // During save restoration the controller is created in Loading.
            // Wait until the game has initialized the ordinary cursor preview.
            if (Plant.mApp == null || Plant.mApp.GameScene != GameScenes.Playing) return false;
            // Spine localBounds are culling bounds (over 300 million units in
            // Replanted), not artwork dimensions. Never use them for placement.
            native = preview ? Controller.GetComponentInChildren<SpriteRenderer>(true)
                : Controller.Cast<PlantController>().m_meshRenderer;
            if (native == null || !native) return false;
            // Match CoreLib's SpriteRendererPlantSkin: attach to the skin's
            // ground-origin transform, alongside the native skeleton.
            // SplitPea and GatlingPea have no skin controller. Their native
            // mesh transform is also m_renderOffset (*Pea_Render), the
            // same ground-origin anchor used by its animation controller.
            var skin = Controller.m_skinController;
            var parent = !preview && skin != null && skin ? skin.transform : native.transform;
            if (parent == null || !parent) return false;
            // Measure the actual native Gatling pose in the same parent space.
            // Preview art is smaller than its animated board model. Defer until
            // Spine has produced geometry; culling bounds contain sentinels.
            Bounds? nativeGatling = !preview && assets.Identity.NativeType == SeedType.Gatlingpea
                ? VisibleMeshBounds(native.Cast<MeshRenderer>(), parent) : null;
            if (!preview && assets.Identity.NativeType == SeedType.Gatlingpea && nativeGatling == null) return false;
            bool puff = FusionPlant.IsPuffVariant(assets.Identity);
            assets.Load(Plant.mBoard, preview ? Controller.Cast<PreviewController>() : null);
            var data = model!;
            root = new GameObject($"Fusion-{assets.Name}Prefab") { layer = native.gameObject.layer };
            root.transform.SetParent(parent, false);
            sorting = root.AddComponent<SortingGroup>();
            transforms = new Transform[data.Nodes.Length];
            renderers = new SpriteRenderer?[data.Nodes.Length];
            for (int i = 0; i < data.Nodes.Length; ++i)
            {
                var node = data.Nodes[i];
                var go = new GameObject(node.Name) { layer = native.gameObject.layer };
                transforms[i] = go.transform;
                go.transform.SetParent(node.Parent < 0 ? root.transform : transforms[node.Parent], false);
                if (node.MaskSprite >= 0)
                {
                    var mask = go.AddComponent<SpriteMask>();
                    mask.sprite = assets.Sprites[node.MaskSprite];
                    mask.alphaCutoff = node.MaskCutoff;
                    mask.isCustomRangeActive = true;
                    mask.frontSortingLayerID = mask.backSortingLayerID = 0;
                    mask.backSortingOrder = -32768;
                    mask.frontSortingOrder = 32767;
                }
                if (node.Sprite < 0) continue;
                var renderer = go.AddComponent<SpriteRenderer>();
                renderers[i] = renderer;
                renderer.sharedMaterial = assets.spriteMaterial;
                renderer.sprite = assets.Sprites[node.Sprite];
                renderer.sortingOrder = node.Order;
                renderer.maskInteraction = (SpriteMaskInteraction)node.MaskInteraction;
                renderer.flipX = node.FlipX;
                renderer.flipY = node.FlipY;
                renderer.color = new Color(node.Color[0], node.Color[1], node.Color[2], node.Color[3]);
            }
            Pose(0, null);
            // Bounds in the original prefab's coordinate system, including all
            // original pivots, hierarchy transforms and its 0.23 root scale.
            bool first = true;
            Bounds bounds = default;
            // Puff variants fit the native Puff-shroom's complete artwork.
            // Other plants retain their established shadow normalization.
            foreach (var renderer in data.GroundNode < 0 || !data.Nodes[data.GroundNode].Active || assets.Identity == FusionPlant.GrappleKelp || puff || assets.Identity.NativeType is SeedType.Tallnut or SeedType.Gatlingpea || assets.Identity == FusionPlant.CaltropShroom
                || assets.Identity == FusionPlant.Starthorn || assets.Identity == FusionPlant.Thornweed || FusionPlant.IsElementalCattail(assets.Identity) || FusionPlant.IsSeaFusion(assets.Identity) || assets.Identity == FusionPlant.PuffKin || assets.Identity == FusionPlant.SpikyPumpkin
                ? renderers : new[] { renderers[data.GroundNode] })
            {
                if (renderer == null || renderer.sprite == null) continue;
                bool active = true;
                // A grid preview's native parent starts hidden. Ignore that
                // parent when measuring the original prefab's active parts.
                for (var part = renderer.transform; part.Pointer != root.transform.Pointer; part = part.parent)
                    if (!part.gameObject.activeSelf) { active = false; break; }
                if (!active) continue;
                // Sprite bounds exist even before a hidden preview's renderer
                // has participated in a frame. World renderer bounds may not.
                var local = renderer.sprite.bounds;
                for (int corner = 0; corner < 8; ++corner)
                {
                    var point = local.center + Vector3.Scale(local.extents,
                        new Vector3((corner & 1) == 0 ? -1 : 1, (corner & 2) == 0 ? -1 : 1,
                            (corner & 4) == 0 ? -1 : 1));
                    if (renderer.flipX) point.x = -point.x;
                    if (renderer.flipY) point.y = -point.y;
                    var p = root.transform.InverseTransformPoint(renderer.transform.TransformPoint(point));
                    if (first) { bounds = new Bounds(p, Vector3.zero); first = false; }
                    else bounds.Encapsulate(p);
                }
            }
            if (first || bounds.size.x <= 0 || bounds.size.y <= 0)
                throw new InvalidDataException($"Original {assets.Name} prefab has no visible bounds.");
            // ReloadedController maps one logical board pixel to 2.82 render
            // units. An ordinary plant occupies an 80x80 logical box. Keep this
            // conversion in the skin's local space so native scale/offsets apply
            // once, including roof, pot and glove movement.
            const float plantSize = 80f * 2.82f;
            var target = puff
                ? PuffArtworkSizing.Target(assets.PuffPreviewBounds, assets.PuffPreviewScale, preview)
                : preview ? native.Cast<SpriteRenderer>().sprite.bounds
                    : new Bounds(new Vector3(0, plantSize * 0.5f, 0), new Vector3(plantSize, plantSize, 0));
            if (!float.IsFinite(target.size.x) || !float.IsFinite(target.size.y)
                || target.size.x <= 0 || target.size.y <= 0)
                throw new InvalidDataException($"Native {assets.Name} preview has no usable sprite bounds.");
            // Keep the stable sprite reference; native Spine mesh geometry
            // includes enormous sentinel coordinates and is not artwork data.
            float scale = FusionPlant.IsBeamShooter(assets.Identity) || FusionPlant.IsDoomFusion(assets.Identity) || FusionPlant.IsCactusFusion(assets.Identity) || assets.Identity == FusionPlant.IllumiNut || assets.Identity == FusionPlant.SunriseShroom || FusionPlant.IsFume(assets.Identity) || FusionPlant.IsScaredy(assets.Identity) || assets.Identity == FusionPlant.SpikyPumpkin
                ? plantSize / 1.35f : target.size.x / bounds.size.x;
            if (assets.Identity.NativeType == SeedType.Tallnut && assets.NativeTallHeight > 0)
                scale = assets.NativeTallHeight / bounds.size.y;
            // Match the live native body's height, not its shadow or card art.
            if (assets.Identity.NativeType == SeedType.Gatlingpea)
                scale = (preview ? target.size.y : nativeGatling!.Value.size.y) / bounds.size.y;
            // Air lobber shadows are only 0.88 source units wide. Fitting them
            // to a full tile inflates their 0.4-scaled bodies by over 50 percent.
            if (FusionPlant.IsAirLobber(assets.Identity))
                scale = (preview ? target.size.x : plantSize) / 1.35f;
            // Pot shadows are deliberately narrow. Fitting them to an entire
            // tile enlarged the whole model, including the visible pot body.
            if (FusionPlant.IsFusionPot(assets.Identity))
                scale = (preview ? target.size.x : plantSize) / 1.35f;
            // Sea support models have no shadow reference. Their full animated
            // geometry changes with growth; use the source unit conversion.
            if (FusionPlant.IsSeaSupport(assets.Identity) || assets.Identity == FusionPlant.SunsetShroom || assets.Identity == FusionPlant.TideShroom || assets.Identity == FusionPlant.GrappleKelp || assets.Identity == FusionPlant.Sparkwood)
                scale = plantSize / 1.35f;
            // DoomSeed is a small spawned spore, not a full-size Doom-shroom.
            // Keep its wrapper scale stable; only its source root grows later.
            if (assets.Identity == FusionPlant.DoomSeed) scale *= .5f;
            if (!float.IsFinite(scale) || scale <= 0)
                throw new InvalidDataException($"Original {assets.Name} artwork has an invalid scale.");
            root.transform.localScale = Vector3.one * scale;
            root.transform.localPosition = new Vector3(target.center.x - bounds.center.x * scale,
                target.min.y - bounds.min.y * scale, 0);
            if (assets.Identity == FusionPlant.DoomSeed)
            {
                var ground = renderers[data.GroundNode]!;
                doomGround = root.transform.parent.InverseTransformPoint(
                    ground.transform.TransformPoint(ground.sprite.bounds.center));
            }
            if (assets.Identity == FusionPlant.SpikyPumpkin)
            {
                // This underground prefab has an empty Shadow transform.
                // Anchor that original ground point, not the buried body's bottom.
                int ground = Array.FindIndex(data.Nodes, node => node.Path == "Shadow");
                var origin = root.transform.InverseTransformPoint(transforms[ground].position);
                root.transform.localPosition = new Vector3(target.center.x - origin.x * scale,
                    target.min.y - origin.y * scale, 0);
            }
            FusionMod.Instance.LoggerInstance.Msg($"{assets.Name} original art ready: preview={preview}, "
                + $"shader={assets.spriteMaterial!.shader.name}, sprites={renderers.Count(r => r != null)}, "
                + $"layer={root.layer}, scale={scale}, artworkBox={target.size}, "
                + $"sizeReference={(puff ? "puff-sprite-1.5x" : "preview-or-footprint")}, "
                + $"parent={parent.name}, localPosition={root.transform.localPosition}, "
                + $"onBoard={Plant.mIsOnBoard}, visible={Plant.mVisible}, "
                + $"parentActive={parent.gameObject.activeInHierarchy}.");
            nativeWasEnabled = native.enabled;
            native.enabled = false;
            ownsNative = true;
            if (!preview)
            {
                var shadow = Controller.m_shadowController;
                if (shadow != null && shadow)
                    foreach (var renderer in shadow.GetComponentsInChildren<Renderer>(true))
                    { shadows.Add((renderer, renderer.enabled)); renderer.enabled = false; }
            }
            return true;
        }

        private void AlignKelpGrab()
        {
            var zombie = Plant.mBoard.ZombieTryToGet(Plant.mTargetZombieID);
            if (zombie == null || zombie.mDead || zombie.mBoard?.Pointer != Plant.mBoard.Pointer
                || zombie.mController == null || !zombie.mController) return;
            int group = Array.FindIndex(model!.Nodes, node => node.Path == "Grab");
            if (group < 0) return;
            bool first = true;
            Bounds bounds = default;
            for (int i = 0; i < model.Nodes.Length; ++i)
            {
                if (!model.Nodes[i].Path.StartsWith("Grab/grabs/", StringComparison.Ordinal)
                    || renderers[i] is not { } renderer || !renderer.gameObject.activeInHierarchy
                    || renderer.sprite == null) continue;
                var sprite = renderer.sprite.bounds;
                for (int corner = 0; corner < 4; ++corner)
                {
                    var point = sprite.center + Vector3.Scale(sprite.extents,
                        new Vector3((corner & 1) == 0 ? -1 : 1, (corner & 2) == 0 ? -1 : 1, 0));
                    point = renderer.transform.TransformPoint(point);
                    if (first) { bounds = new Bounds(point, Vector3.zero); first = false; }
                    else bounds.Encapsulate(point);
                }
            }
            if (first) return;
            // The source script moves this independent group to its victim.
            // Ignore the prefab's scene position, using actual sprite bounds.
            var target = zombie.mController.transform.position;
            transforms[group].position += new Vector3(target.x - bounds.center.x, target.y - bounds.min.y, 0);
        }

        private void Pose(float idleTime, float? shooting) => ImportedPeaVisuals.Pose(model, transforms, idleTime, shooting, renderers, assets.Sprites);

        private void ApplySprites(Clip clip, float time)
        {
            foreach (var binding in clip.Bindings.Where(b => b.Attribute == 5))
                if (renderers[binding.Node] is { } renderer)
                    renderer.sprite = assets.Sprites[(int)Evaluate(binding.Channels[0], time % clip.Duration)];
        }

        private readonly SpeedMod.VisualClock visualClock = new();

        internal void Update()
        {
            if (root == null && !Create()) return;
            bool visible = preview || (Plant.mIsOnBoard && Plant.mVisible && !Plant.mSquished);
            root!.SetActive(visible);
            native!.enabled = false;
            if (!preview)
            {
                // Native render separation can allocate extra Spine meshes
                // later (e.g. around a pumpkin). Hide those as well as the main
                // mesh, without touching the imported SpriteRenderers.
                foreach (var part in Controller.GetComponentsInChildren<MeshRenderer>(true))
                {
                    if (part.Pointer == native.Pointer) continue;
                    if (!nativeParts.ContainsKey(part.Pointer))
                        nativeParts[part.Pointer] = (part, part.enabled);
                    part.enabled = false;
                }
            }
            foreach (var shadow in shadows) if (shadow.Renderer) shadow.Renderer.enabled = false;
            sorting!.sortingLayerID = native.sortingLayerID;
            sorting.sortingOrder = native.sortingOrder;
            if (!visible) return;
            float? shoot = null;
            if (!preview && !FusionPlant.IsMine(assets.Identity) && model.Clips.ContainsKey("shoot"))
            {
                var controller = Controller.Cast<PlantController>();
                var track = CharacterAnimationTrack.Head;
                var animation = "shooting";
                if (assets.Identity.NativeType == SeedType.Splitpea
                    && !controller.IsAnimationPlayingOnTrack("shooting", track))
                {
                    track = CharacterAnimationTrack.Head2;
                    animation = "splitpea_shooting";
                }
                if (controller.IsAnimationPlayingOnTrack(animation, track))
                    shoot = Math.Clamp(controller.GetAnimationPercentOnTrack(track), 0, 1)
                        * model.Clips["shoot"].Duration;
            }
            // mAnimCounter is a short native ping-pong frame counter, not an
            // elapsed clock. Board simulation ticks cover the full source clip,
            // respect pause/game speed and survive saves without extra state.
            float idle = ((long)Plant.mBoard.mMainCounter + (id & 0xff)) * 0.01f * model!.IdleSpeed;
            string baseClip = "idle";
            if (!preview && assets.Identity == FusionPlant.TideShroom && Plant.mStartRow != 0)
            { baseClip = Plant.mTargetX < 100 ? "blow" : "blowloop"; idle = Plant.mTargetX < 100 ? Plant.mTargetX * .01f : idle; }
            if (FusionPlant.IsSilverLobber(assets.Identity))
                shoot = !preview && Plant.mStartRow > 0
                    ? (SilverLobbers.ShootTicks - Plant.mStartRow) * .01f : null;
            if (FusionPlant.IsPuffShooter(assets.Identity))
                shoot = !preview && Plant.mStartRow > 0
                    ? (PuffShooters.Duration(assets.Identity) - Plant.mStartRow) * .01f : null;
            if (assets.Identity == FusionPlant.BallCactus)
                shoot = !preview && Plant.mStartRow > 0 ? (50 - Plant.mStartRow) * .01f : null;
            if (FusionPlant.IsPuffSpecial(assets.Identity))
                shoot = !preview && assets.Identity != FusionPlant.HailShroom && Plant.mStartRow > 0
                    ? (PuffSpecials.ShootTicks - Plant.mStartRow) * .01f : null;
            if (FusionPlant.IsElementalCattail(assets.Identity))
                shoot = !preview && Plant.mStartRow > 0
                    ? (CattailFusions.ShootTicks - Plant.mStartRow) * .01f : null;
            if (FusionPlant.IsDoomShooter(assets.Identity))
                shoot = !preview && Plant.mStartRow > 0
                    ? ((Plant.mTargetY > 0 ? Plant.mTargetY : DoomShooters.ShootTicks) - Plant.mStartRow)
                        / (float)(Plant.mTargetY > 0 ? Plant.mTargetY : DoomShooters.ShootTicks) : null;
            if (FusionPlant.IsAirLobber(assets.Identity))
                shoot = !preview && Plant.mStartRow > 0
                    ? (AirLobbers.ShootTicks - Plant.mStartRow) * .01f : null;
            string shootClip = "shoot";
            bool loopShooting = false;
            if (FusionPlant.IsBeamShooter(assets.Identity))
            {
                shoot = null;
                if (!preview && Plant.mLaunchCounter > 0)
                {
                    float elapsed = Plant.mStateCountdown * .01f;
                    if (elapsed >= model.Clips["shoot"].Duration)
                    {
                        shootClip = "shooting";
                        elapsed -= model.Clips["shoot"].Duration;
                    }
                    loopShooting = true;
                    shoot = elapsed;
                }
            }
            if (assets.Identity == FusionPlant.DeathStar)
            {
                shootClip = "shake";
                loopShooting = true;
                shoot = preview ? null : idle;
            }
            if (FusionPlant.IsPuffLobber(assets.Identity))
                shoot = !preview && Plant.mStartRow > 0
                    ? (PuffLobbers.ShootTicks - Plant.mStartRow) * .01f : null;
            if (FusionPlant.IsTriLobber(assets.Identity))
                shoot = !preview && Plant.mStartRow > 0
                    ? (TriLobbers.ShootTicks - Plant.mStartRow) * .01f : null;
            if (FusionPlant.IsMixedLobber(assets.Identity))
                shoot = !preview && Plant.mStartRow > 0
                    ? (LobberFusions.Duration(assets.Identity) - Plant.mStartRow) * .01f : null;
            if ((assets.Identity == FusionPlant.TacoPult || assets.Identity == FusionPlant.CobPult || assets.Identity == FusionPlant.Cobsicle || assets.Identity == FusionPlant.ClovePult || assets.Identity == FusionPlant.ButternutPult || assets.Identity == FusionPlant.KernelMorph)
                && !preview && Plant.mShootingCounter == 1) shootClip = "shoot2";
            if (assets.Identity == FusionPlant.KernelTriPult && !preview && Plant.mShootingCounter == 1)
                shootClip = "shoot2";
            if (assets.Identity == FusionPlant.BabyCorn && !preview && Plant.mShootingCounter == 1)
                shootClip = "shoot2";
            if (assets.Identity == FusionPlant.SilverKernel && !preview && Plant.mShootingCounter == 1)
                shootClip = "shoot2";
            float baseTime = idle;
            if (assets.Identity == FusionPlant.SpringMelon && !preview)
            {
                shoot = null;
                baseClip = Plant.mStartRow > 0 ? "shoot" : Plant.mTargetY > 0 ? "wait" : "idle2";
                baseTime = Plant.mStartRow > 0 ? (100 - Plant.mStartRow) * .01f
                    : Plant.mTargetY > 0 ? (100 - Plant.mTargetY) * .01f : idle;
                baseTime = Math.Min(baseTime, model.Clips[baseClip].Duration - .00001f);
            }
            if (assets.Identity == FusionPlant.SunsetShroom && !preview)
            {
                shoot = null;
                baseClip = Plant.mTargetY > 0 ? (Plant.mStartRow == 1 ? "grow1" : "grow2")
                    : Plant.mStartRow == 0 ? "idle" : Plant.mStartRow == 1 ? "idle1" : "idle2";
                baseTime = Plant.mTargetY > 0 ? Math.Min((100 - Plant.mTargetY) * .01f,
                    model.Clips[baseClip].Duration - .00001f) : idle;
            }
            if (assets.Identity == FusionPlant.Spicicle && !preview && Plant.mPlantHealth < Plant.mPlantMaxHealth / 5)
            {
                shootClip = "shake";
                shoot = idle;
                loopShooting = true;
            }
            if (assets.Identity == FusionPlant.HypnoPepper)
            {
                shoot = null;
                if (!preview)
                {
                    baseClip = "explode";
                    baseTime = Math.Min(Plant.mTargetY * .008f, model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (assets.Identity == FusionPlant.SolarFlare && !preview)
            {
                shoot = null;
                baseClip = "explode";
                baseTime = Math.Min(Plant.mTargetY * .01f, model.Clips[baseClip].Duration - .00001f);
            }
            if (assets.Identity == FusionPlant.SolarBlover && !preview)
            {
                shoot = null;
                baseClip = Plant.mTargetY >= 69 ? "blowloop" : "blow";
                baseTime = baseClip == "blowloop" ? idle
                    : Math.Min(Plant.mTargetY * .01f, model.Clips[baseClip].Duration - .00001f);
            }
            if (FusionPlant.IsPuffSupport(assets.Identity))
            {
                shoot = null;
                if (!preview && assets.Identity == FusionPlant.BellPepper && Plant.mStartRow > 0)
                {
                    baseClip = "explode";
                    baseTime = Math.Min((PuffSupport.BellFuse - Plant.mStartRow) * .01f,
                        model.Clips[baseClip].Duration - .00001f);
                }
                if (!preview && assets.Identity == FusionPlant.PennyShroom
                    && Plant.mTargetX == 0)
                {
                    baseClip = Plant.mTargetY > 0 ? "grow" : "idle2";
                    baseTime = Plant.mTargetY > 0 ? (PuffSupport.GrowTicks - Plant.mTargetY) * .01f : idle;
                }
                if (!preview && assets.Identity == FusionPlant.UmbrellaCap && Plant.mStartRow > 0)
                {
                    baseClip = "block";
                    baseTime = (PuffSupport.BlockTicks - Plant.mStartRow) * .01f;
                }
            }
            if (!preview && assets.Identity == FusionPlant.DemiseShroom && !Plant.mIsAsleep)
            {
                baseClip = "explode";
                baseTime = Math.Min((96 - Plant.mLaunchCounter) * .007f, model.Clips[baseClip].Duration);
            }
            if (FusionPlant.IsScaredy(assets.Identity))
            {
                shoot = null;
                if (!preview)
                {
                    baseClip = Plant.mState == PlantState.ScaredyshroomLowering ? "scared"
                        : Plant.mState == PlantState.ScaredyshroomScared ? "scaredidle"
                        : Plant.mState == PlantState.ScaredyshroomRaising ? "scaredgrow"
                        : Plant.mStartRow > 0 ? "shoot" : "idle";
                    baseTime = baseClip == "scared" ? (100 - Plant.mTargetY) * .01f
                        : baseClip == "scaredgrow" ? (ScaredyFusions.GrowTicks(assets.Identity) - Plant.mTargetY) * .01f
                        : baseClip == "shoot" ? (ScaredyFusions.ShootTicks(assets.Identity) - Plant.mStartRow) * .01f : idle;
                    if (assets.Identity == FusionPlant.FrenzyShroom && baseClip == "shoot")
                        baseTime = (Plant.mTargetY - Plant.mStartRow) * .5f / Math.Max(1, Plant.mTargetY);
                    if (baseClip is "scared" or "scaredgrow" or "shoot")
                        baseTime = Math.Min(baseTime, model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (FusionPlant.IsFume(assets.Identity))
            {
                shoot = !preview && Plant.mStartRow > 0
                    ? ((FumeVariants.IsVariant(assets.Identity) ? FumeVariants.Duration(assets.Identity)
                        : FumeFusions.Duration(assets.Identity)) - Plant.mStartRow) * .01f : null;
                if (!preview && assets.Identity == FusionPlant.SootShroom && !shoot.HasValue
                    && Plant.mLaunchCounter > 0) baseClip = "rest";
            }
            if (assets.Identity == FusionPlant.BelchShroom && !preview && Plant.mTargetY > 0)
            {
                shoot = null;
                baseClip = "chomp";
                baseTime = (EatingShrooms.ChompTicks - Plant.mTargetY) * .01f;
            }
            if (!preview && assets.Identity == FusionPlant.BushShroom)
            {
                if (Plant.mShootingCounter > 0 || Plant.mState == PlantState.UmbrellaReflecting)
                {
                    shoot = null;
                    baseClip = "block";
                    baseTime = Plant.mShootingCounter > 0 ? (84 - Plant.mShootingCounter) * .012f : 0;
                }
            }
            if (assets.Identity == FusionPlant.CaltropShroom) shootClip = "attack";
            if (assets.Identity == FusionPlant.GutsyShroom)
            {
                shoot = !preview && Plant.mStartRow > 0 ? GutsyShroom.AnimationTime(Plant) : null;
                shootClip = !preview && Plant.mTargetX == 1 ? "shoot1" : "shoot";
            }
            if (FusionPlant.IsGloom(assets.Identity))
            {
                shoot = null;
                if (!preview && Plant.mStartRow > 0)
                {
                    baseClip = GloomFusions.Attack(assets.Identity);
                    baseTime = Math.Min((GloomFusions.Duration(assets.Identity) - Plant.mStartRow) * .01f
                        * GloomFusions.Speed(assets.Identity), model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (assets.Identity == FusionPlant.CraterTot)
            {
                baseClip = preview || Plant.mState == PlantState.PotatoArmed ? "idle"
                    : Plant.mState == PlantState.PotatoRising ? "rise" : "unrised";
                baseTime = baseClip == "rise" ? Math.Min((50 - Plant.mStateCountdown) * .01f, model.Clips[baseClip].Duration - .00001f) : idle;
            }
            if ((assets.Identity == FusionPlant.Flytrapper || assets.Identity == FusionPlant.DoomChomper || assets.Identity == FusionPlant.Frostbiter || assets.Identity == FusionPlant.ChompMine || assets.Identity == FusionPlant.SolarChomper || assets.Identity == FusionPlant.CherryChomper || assets.Identity == FusionPlant.ChompNut || assets.Identity == FusionPlant.ChompShooter) && !preview)
            {
                // Source states replace the base layer. Native saved states
                // and animation progress own timing, including pause/load.
                var state = Plant.mState;
                string? clip = state == PlantState.ChomperDigesting ? "chew"
                    : state == PlantState.ChomperSwallowing ? "swallow"
                    : state == PlantState.ChomperBiting || state == PlantState.ChomperBitingGotOne
                        || state == PlantState.ChomperBitingMissed ? "bite" : null;
                if (clip != null && model.Clips.ContainsKey(clip))
                {
                    baseClip = clip;
                    baseTime = clip == "chew" ? idle
                        : Math.Clamp(Controller.Cast<PlantController>()
                            .GetAnimationPercentOnTrack(CharacterAnimationTrack.Body), 0, 1)
                            * Math.Max(0, model.Clips[clip].Duration - 0.00001f);
                }
            }
            if (FusionPlant.IsCactusFusion(assets.Identity))
            {
                shoot = null;
                bool high = !preview && Plant.mTargetX != 0;
                baseClip = high ? "idle2" : "idle";
                baseTime = idle;
                if (!preview && assets.Identity == FusionPlant.DoomCactus && Plant.mDoSpecialCountdown > 0)
                {
                    baseClip = "superskill";
                    baseTime = Math.Min((184 - Plant.mDoSpecialCountdown) * .01f,
                        model.Clips[baseClip].Duration - .00001f);
                }
                else if (!preview && Plant.mTargetY > 0)
                {
                    baseClip = assets.Identity == FusionPlant.SaguaroShroom
                        ? high ? "1to2" : "2to1" : high ? "rise" : "lower";
                    baseTime = Math.Min((CactusFusions.TransitionTicks(Plant) - Plant.mTargetY) * .01f,
                        model.Clips[baseClip].Duration - .00001f);
                }
                else if (!preview && Plant.mStartRow > 0)
                {
                    baseClip = assets.Identity == FusionPlant.SaguaroShroom && !high ? "shoot" : high ? "shoot2" : "shoot1";
                    baseTime = Math.Min((CactusFusions.AttackTicks(Plant) - Plant.mStartRow) * .01f,
                        model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (assets.Identity == FusionPlant.Starthorn || assets.Identity == FusionPlant.Thornweed)
            {
                shoot = null;
                if (!preview && Plant.mStartRow > 0)
                {
                    baseClip = assets.Identity == FusionPlant.Thornweed ? "attack" : "shoot";
                    baseTime = Math.Min((67 - Plant.mStartRow) * .01f, model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (FusionPlant.IsWindBlover(assets.Identity))
            {
                shoot = null;
                if (!preview && assets.Identity == FusionPlant.Cyclover && Plant.mTargetY > 0)
                {
                    baseClip = "blow";
                    baseTime = Math.Min((BloverFusions.CycloneTicks - Plant.mTargetY) * .01f,
                        model.Clips[baseClip].Duration - .00001f);
                }
                else if (assets.Identity != FusionPlant.Cyclover)
                {
                    baseClip = !preview && Plant.mTargetX >= 69 ? "blowloop" : "blow";
                    baseTime = preview ? 0 : baseClip == "blowloop" ? idle
                        : Math.Min(Plant.mTargetX * .01f, model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (!preview && assets.Identity == FusionPlant.FalloutBlover)
            {
                shoot = null;
                baseClip = Plant.mTargetY >= 69 ? "blowloop" : "blow";
                baseTime = baseClip == "blowloop" ? idle
                    : Math.Min(Plant.mTargetY * .01f, model.Clips[baseClip].Duration - .00001f);
            }
            if (FusionPlant.IsSeaSupport(assets.Identity))
            {
                shoot = null;
                if (assets.Identity == FusionPlant.BeaconShroom)
                {
                    baseClip = !preview && Plant.mShootingCounter > 0 ? "idle2" : "idle";
                    if (!preview && Plant.mTargetY > 0)
                    { baseClip = "grow"; baseTime = (100 - Plant.mTargetY) * .01f; }
                }
                else if (assets.Identity == FusionPlant.JellyShroom)
                {
                    baseClip = "idle";
                    // Original eat layer overrides its keyed idle transforms.
                    shootClip = "eat";
                    if (!preview && Plant.mTargetY > 0) shoot = (100 - Plant.mTargetY) * .01f;
                }
                else if (assets.Identity == FusionPlant.SwellShroom)
                {
                    baseClip = !preview && Plant.mTargetY > 0 ? "freezed" : "idle";
                    if (baseClip == "freezed") baseTime = Math.Min((SeaSupportFusions.SwellTicks - Plant.mTargetY) * .01f,
                        model.Clips[baseClip].Duration - .00001f);
                }
                else
                {
                    int mode = preview ? 0 : Plant.mShootingCounter;
                    baseClip = mode == 1 ? "sea" : mode == 2 ? "jihuo" : mode == 4 ? "suohuiqu"
                        : mode == 3 ? Plant.mStartRow > 0 ? "shoot" : "idle" : "scaredidle";
                    if (mode is 1 or 2 or 4)
                        baseTime = Math.Min(((mode == 1 ? SeaSupportFusions.FeedTicks : SeaSupportFusions.ActivateTicks)
                            - Plant.mTargetY) * .01f, model.Clips[baseClip].Duration - .00001f);
                    else if (mode == 3 && Plant.mStartRow > 0)
                        baseTime = Math.Min((SeaSupportFusions.ShotTicks - Plant.mStartRow) * .01f,
                            model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (FusionPlant.IsSeaFusion(assets.Identity))
            {
                shoot = null;
                bool grown = !preview && Plant.mShootingCounter > 0 && assets.Identity != FusionPlant.MinnowShroom;
                baseClip = grown && assets.Identity != FusionPlant.SharkShroom ? "idle2" : "idle";
                if (!preview && Plant.mTargetY != 0)
                {
                    baseClip = Plant.mTargetY > 0 ? "grow" : "eat";
                    baseTime = Math.Min((100 - Math.Abs(Plant.mTargetY)) * .01f, model.Clips[baseClip].Duration - .00001f);
                }
                else if (!preview && Plant.mStartRow > 0)
                {
                    bool matureAttack = (Plant.mDoSpecialCountdown & 1) != 0;
                    bool air = (Plant.mDoSpecialCountdown & 2) != 0;
                    baseClip = assets.Identity == FusionPlant.UrchinShroom
                        ? (air ? "shoot2" : "shoot") + (matureAttack ? "_grown" : "")
                        : assets.Identity == FusionPlant.SeaStar && matureAttack ? "shoot2" : "shoot";
                    baseTime = Math.Min((SeaFusions.Duration(Plant) - Plant.mStartRow) * .01f,
                        model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (FusionPlant.IsStarFusion(assets.Identity))
            {
                shoot = null;
                if (assets.Identity == FusionPlant.SpicyStarfruit)
                {
                    baseClip = "shake";
                    baseTime = idle;
                }
                else if (!preview && Plant.mStartRow > 0)
                {
                    float rate = assets.Identity == FusionPlant.Starshooter ? 2 : 1.1f;
                    float time = (StarFusions.ShootTicks(assets.Identity) - Plant.mStartRow) * .01f * rate;
                    if (assets.Identity == FusionPlant.Starshooter) shoot = time;
                    else { baseClip = "shoot"; baseTime = time; }
                }
            }
            if (!preview && assets.Identity == FusionPlant.Solarfruit)
            {
                shoot = null;
                if (Plant.mStartRow > 0)
                {
                    baseClip = "shoot";
                    baseTime = Math.Min((SupportFusions.SolarShotTicks - Plant.mStartRow) * .02f,
                        model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (assets.Identity == FusionPlant.Chompkin)
            {
                shoot = null;
                if (!preview && Plant.mStartRow > 0)
                {
                    baseClip = "chomp";
                    baseTime = (PumpkinSupport.ChompTicks - Plant.mStartRow) * .01f;
                }
            }
            if (assets.Identity == FusionPlant.PumpkinPod)
                shoot = !preview && Plant.mStartRow > 0 ? (PumpkinSupport.PodClipTicks - Plant.mStartRow) * .01f : null;
            if (assets.Identity == FusionPlant.FumingPumpkin)
                shoot = !preview && Plant.mStartRow > 0 ? (75 - Plant.mStartRow) * .01f : null;
            if (!preview && assets.Identity == FusionPlant.Chompzilla && Plant.mLaunchCounter > 0)
            {
                baseClip = "bite";
                baseTime = (175 - Plant.mLaunchCounter) / 175f * (model.Clips[baseClip].Duration - .00001f);
            }
            if (!preview && assets.Identity == FusionPlant.ChompShooter && Plant.mState == PlantState.ChomperDigesting)
                shoot = (300 - Plant.mLaunchCounter) * .01f < model.Clips["shoot"].Duration
                    ? (300 - Plant.mLaunchCounter) * .01f : null;
            if (!preview && FusionPlant.IsContactUmbrella(assets.Identity))
            {
                if (Plant.mShootingCounter > 0)
                {
                    baseClip = "block";
                    baseTime = Math.Min((UmbrellaShell.AnimationTicks - Plant.mShootingCounter) * .012f, .99999f);
                }
                else if (Plant.mState == PlantState.UmbrellaReflecting)
                {
                    baseClip = "block";
                    baseTime = Math.Clamp(Controller.Cast<PlantController>()
                        .GetAnimationPercentOnTrack(CharacterAnimationTrack.Body), 0, .99999f);
                }
            }
            if (FusionPlant.IsMine(assets.Identity) && assets.Identity != FusionPlant.ChipMine && assets.Identity != FusionPlant.LandMine && assets.Identity != FusionPlant.SeaMine)
            {
                baseClip = assets.Identity == FusionPlant.MineNut || (!preview && Plant.mState == PlantState.PotatoArmed)
                    ? "armed" : !preview && Plant.mState == PlantState.PotatoRising ? "rise" : "idle";
                baseTime = baseClip == "rise" ? (MineFusions.RiseDuration(assets.Identity) - Plant.mStateCountdown) * .01f : idle;
                if (assets.Identity == FusionPlant.ThreetatoMine)
                {
                    baseClip = !preview && Plant.mState == PlantState.PotatoArmed ? "idle2" : !preview && Plant.mState == PlantState.PotatoRising ? "rise" : "idle";
                    if (!preview && Plant.mState == PlantState.Notready && Plant.mStartRow == 1 && Plant.mStateCountdown > 1300)
                    { baseClip = "down"; baseTime = (1500 - Plant.mStateCountdown) * .01f; }
                    if (!preview && Plant.mShootingCounter > 0) shoot = (Plant.mTargetX - Plant.mShootingCounter) / (float)Math.Max(1, Plant.mTargetX) * (2f / 3);
                }
                if (!preview && assets.Identity == FusionPlant.PeatatoMine && Plant.mShootingCounter > 0)
                    shoot = (MineFusions.ShootTicks - Plant.mShootingCounter) * .01f;
            }
            if (FusionPlant.IsMine(assets.Identity))
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (renderers[i] is { } renderer && model.Nodes[i].Sprite >= 0)
                        renderer.sprite = assets.Sprites[model.Nodes[i].Sprite];
            if (assets.Identity == FusionPlant.ThornNut && !preview)
            {
                bool raised = Plant.mLaunchCounter == 1;
                baseClip = Plant.mShootingCounter > 0 ? raised ? "rise" : "unrise" : raised ? "idle2" : "idle";
                baseTime = Plant.mShootingCounter > 0
                    ? Math.Max(0, (raised ? 58 : 50) - Plant.mShootingCounter) * .01f : idle;
            }
            if (assets.Identity == FusionPlant.TangleMine && !preview)
            {
                baseClip = Plant.mState == PlantState.TanglekelpGrabbing ? "grab"
                    : Plant.mState == PlantState.PotatoArmed ? "armed"
                    : Plant.mState == PlantState.PotatoRising ? "rise" : "idle";
                baseTime = Plant.mState == PlantState.TanglekelpGrabbing ? (100 - Plant.mStateCountdown) * .01f
                    : Plant.mState == PlantState.PotatoRising ? (MineFusions.RiseTicks - Plant.mStateCountdown) * .01f : idle;
            }
            if (assets.Identity == FusionPlant.KelpMine && !preview && Plant.mState == PlantState.TanglekelpGrabbing)
            { baseClip = "grab"; baseTime = (100 - Plant.mStateCountdown) * .01f; }
            if (assets.Identity == FusionPlant.RoastedPotato)
            { baseClip = "armed"; baseTime = idle; shoot = null; }
            if (assets.Identity == FusionPlant.PotatoMasher)
            {
                baseClip = preview || Plant.mState == PlantState.PotatoArmed ? "idle2"
                    : Plant.mState == PlantState.PotatoRising ? "rise" : "idle";
                baseTime = Plant.mState == PlantState.PotatoRising ? (50 - Plant.mStateCountdown) * .01f : idle;
            }
            if ((FusionPlant.IsSquash(assets.Identity) || assets.Identity == FusionPlant.RoastedSquash) && !preview)
                (baseClip, baseTime) = SquashFusions.Animation(Plant);
            if (!preview && FusionPlant.IsRock(assets.Identity) && Plant.mState == PlantState.SpikeweedAttacking)
            {
                baseClip = "attack";
                // Match the two original events (1/3 and .7 seconds) to
                // native contact countdowns 70 and 32 without emitting hits.
                float elapsed = 100 - Plant.mStateCountdown;
                baseTime = elapsed <= 30 ? elapsed / 30f / 3
                    : elapsed <= 68 ? 1f / 3 + (elapsed - 30) / 38 * (.7f - 1f / 3)
                    : .7f + (elapsed - 68) / 32 * .2f;
                baseTime = Math.Clamp(baseTime, 0, model.Clips[baseClip].Duration - .00001f);
            }
            else if (!preview && FusionPlant.IsWeed(assets.Identity) && Plant.mState == PlantState.SpikeweedAttacking && Plant.mStateCountdown <= 50)
            {
                baseClip = "attack";
                // Original event lies halfway through attack; native contact is
                // at countdown 25. Fit the clip into the final half-second.
                baseTime = Math.Clamp((50 - Plant.mStateCountdown) / 50f * model.Clips[baseClip].Duration,
                    0, model.Clips[baseClip].Duration - .00001f);
            }
            if ((FusionPlant.IsThreepeater(assets.Identity) || assets.Identity == FusionPlant.SquashSpreader) && !preview && Plant.mStartRow > 0)
                shoot = (100 - Plant.mStartRow) * .01f;
            if (assets.Identity == FusionPlant.Multipeater)
            {
                int mode = preview ? 0 : Plant.mStartRow > 0 ? Plant.mTargetX : Plant.mTargetY;
                mode = Math.Clamp(mode, 0, 2);
                baseClip = mode == 0 ? "idle" : mode == 1 ? "idle3" : "idle2";
                shootClip = mode == 0 ? "shoot1" : mode == 1 ? "shoot3" : "shoot2";
                baseTime = idle;
                shoot = !preview && Plant.mStartRow > 0
                    ? (ThreepeaterFusions.MultiDuration(Plant) - Plant.mStartRow) * .01f : null;
                if (shoot >= model.Clips[shootClip].Duration) shoot = null;
            }
            if (assets.Identity == FusionPlant.SpikePot && !preview && Plant.mStartRow > 0)
            {
                baseClip = "attack";
                baseTime = Math.Min((SpikePot.AttackTicks - Plant.mStartRow) * .01f,
                    model.Clips[baseClip].Duration - .00001f);
            }
            if (FusionPlant.IsEnergyNut(assets.Identity))
                shoot = !preview && Plant.mStartRow > 0 ? (ActiveNuts.EnergyClipTicks - Plant.mStartRow) * .01f : null;
            if (assets.Identity == FusionPlant.ThreeNuts)
            {
                int stage = preview ? 0 : Math.Clamp(Plant.mTargetX, 0, 2);
                baseClip = stage == 0 ? "idle" : stage == 1 ? "idle2" : "idle3";
                baseTime = idle;
                if (!preview && Plant.mShootingCounter > 0)
                {
                    baseClip = stage == 1 ? "fall1" : "fall2";
                    baseTime = Math.Min((25 - Plant.mShootingCounter) * .01f, model.Clips[baseClip].Duration - .00001f);
                }
                shoot = !preview && Plant.mStartRow > 0 ? (ActiveNuts.StackClipTicks - Plant.mStartRow) * .01f : null;
            }
            if (assets.Identity == FusionPlant.SquashNut)
            {
                shoot = null;
                if (!preview && Plant.mStartRow > 0)
                {
                    baseClip = "jump";
                    baseTime = Math.Min((ActiveNuts.JumpTicks - Plant.mStartRow) * .01f, model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (assets.Identity == FusionPlant.TingleKelp)
            {
                shoot = null;
                if (!preview && Plant.mStartRow > 0)
                {
                    baseClip = "explode";
                    baseTime = Math.Min((TingleKelp.ExplodeTicks - Plant.mStartRow) * .01f,
                        model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (assets.Identity == FusionPlant.GrappleKelp)
            {
                shoot = null;
                var animation = preview ? (Clip: "idle", Time: idle, Previous: (string?)null, PreviousTime: 0f, Blend: 1f)
                    : GrappleKelp.Animation(Plant, idle);
                baseClip = animation.Clip;
                baseTime = Math.Min(animation.Time, model.Clips[baseClip].Duration - .00001f);
            }
            if (assets.Identity == FusionPlant.SpikeKelp)
            {
                shoot = null;
                if (!preview && Plant.mStartRow > 0)
                {
                    baseClip = "attack";
                    baseTime = Math.Min((SpikeKelp.AttackTicks - Plant.mStartRow) * .01f,
                        model.Clips[baseClip].Duration - .00001f);
                }
            }
            if (FusionPlant.IsGroundWeed(assets.Identity))
            {
                shoot = null;
                baseClip = !preview && Plant.mStartRow > 0 ? "attack" : "idle";
                baseTime = baseClip == "attack" ? Math.Min((67 - Plant.mStartRow) * .01f,
                    model.Clips[baseClip].Duration - .00001f) : idle;
            }
            if (assets.Identity == FusionPlant.SeaNut)
            {
                shoot = null;
                baseClip = !preview && Plant.mStartRow >= 3 ? "round" : "idle";
                baseTime = baseClip == "round" ? Plant.mTargetY * .01f : idle;
            }
            if (assets.Identity == FusionPlant.Squashweed)
            {
                shoot = null;
                baseClip = !preview && Plant.mStartRow > 0 ? "attack" : "idle";
                baseTime = baseClip == "attack" ? Math.Min((67 - Plant.mStartRow) * .01f,
                    model.Clips[baseClip].Duration - .00001f) : idle;
            }
            if (assets.Identity == FusionPlant.KelpSpreader)
                shoot = !preview && Plant.mState != PlantState.TanglekelpGrabbing && Plant.mStartRow > 0
                    ? (100 - Plant.mStartRow) * .01f : null;
            if (assets.Identity == FusionPlant.StrangleKelp && !preview && Plant.mTargetY > 0)
            {
                shoot = null;
                baseClip = "searchzombie";
                baseTime = Math.Min((215 - Plant.mTargetY) * .01f, model.Clips[baseClip].Duration - .00001f);
            }
            if (!preview)
            {
                baseTime = visualClock.Sample("base:" + baseClip, baseTime);
                if (shoot.HasValue) shoot = visualClock.Sample("shoot:" + shootClip, shoot.Value);
            }
            if (loopShooting && shoot.HasValue) shoot %= model.Clips[shootClip].Duration;
            ImportedPeaVisuals.Pose(model, transforms, baseTime, shoot, renderers, assets.Sprites, baseClip, shootClip);
            if (!preview) idle = visualClock.Sample("overlays", idle);
            if (assets.Identity == FusionPlant.SeaNut && !preview)
            {
                transforms[0].localScale *= SeaNut.Scale(Plant);
                // The original round clip translates its root across the lawn.
                // Saved native mX owns that travel here, exactly once.
                if (Plant.mStartRow >= 3) transforms[0].localPosition = Vector(model.Nodes[0].Position);
            }
            if (assets.Identity == FusionPlant.MinnowShroom)
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (model.Nodes[i].Path == "SeaShroom_body_2")
                        transforms[i].gameObject.SetActive(!preview && Plant.mShootingCounter >= 3);

            if (assets.Identity == FusionPlant.GrappleKelp && !preview)
            {
                var animation = GrappleKelp.Animation(Plant, idle);
                BlendGrapple(animation.Previous, animation.PreviousTime, baseClip, baseTime, animation.Blend);
            }
            if (assets.Identity == FusionPlant.TingleKelp || assets.Identity == FusionPlant.GrappleKelp
                || assets.Identity == FusionPlant.SpikeKelp)
            {
                var water = model.Clips["water"];
                Apply(transforms, water, idle % water.Duration, true);
                ApplySprites(water, idle % water.Duration);
            }
            if (assets.Identity == FusionPlant.DoomSeed && !preview)
            {
                // Grow the source prefab around its ground point, never the
                // imported wrapper: the prefab carries a scene X of 1.2865.
                var ground = renderers[model.GroundNode]!;
                float growth = 1f + 1.5f * (1 - Math.Clamp(Plant.mLaunchCounter / 3000f, 0, 1));
                var scale = model.Nodes[0].Scale;
                transforms[0].localScale = new Vector3(scale[0], scale[1], scale[2]) * growth;
                root!.transform.position += root.transform.parent.TransformPoint(doomGround)
                    - ground.transform.TransformPoint(ground.sprite.bounds.center);
            }
            if (assets.Identity == FusionPlant.CraterTot && !preview && Plant.mTargetY > 0)
                ApplySprites(model.Clips["flash"], visualClock.Sample("crater-flash", Math.Min((20 - Plant.mTargetY) * .01f,
                    model.Clips["flash"].Duration - .00001f)));
            UpdateSkins();
            if (assets.Identity == FusionPlant.TangleNut)
            {
                int stage = preview ? 0 : ElementalNuts.DamageStage(Plant.mPlantHealth, Plant.mPlantMaxHealth);
                for (int i = 0; i < model.Nodes.Length; ++i)
                    switch (model.Nodes[i].Name)
                    {
                        case "Wallnut": transforms[i].gameObject.SetActive(stage == 0); break;
                        case "Wallnut_cracked": transforms[i].gameObject.SetActive(stage == 1); break;
                        case "Wallnut_cracked2": transforms[i].gameObject.SetActive(stage == 2); break;
                        case "Crack1": transforms[i].gameObject.SetActive(stage == 0); break;
                        case "Crack2": transforms[i].gameObject.SetActive(stage < 2); break;
                    }
            }
            if (assets.Identity == FusionPlant.SpuddyShroom)
            {
                // ScaredyPotato.Start and State_scaredyPotato switch the two
                // original fuse lights through transform.GetChild(0).GetChild(0/1).
                bool burning = preview || Plant.mTargetX > 0;
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (model.Nodes[i].Path == "ScaredyShroom_head/light1") transforms[i].gameObject.SetActive(burning);
                    else if (model.Nodes[i].Path == "ScaredyShroom_head/light2") transforms[i].gameObject.SetActive(!burning);
            }
            if (assets.Identity == FusionPlant.FlameGloomShroom)
            {
                var fire = model.Clips["idle_fire"];
                Apply(transforms, fire, idle % fire.Duration, true);
                ApplySprites(fire, idle % fire.Duration);
            }
            if (assets.Identity == FusionPlant.StarchShroom && !preview && Plant.mShootingCounter > 0)
                ApplySprites(model.Clips["flash"], visualClock.Sample("starch-flash", (12 - Plant.mShootingCounter) * .01f));
            if (FusionPlant.IsRock(assets.Identity))
            {
                int damage = WeedFusions.RockDamageStage(Plant.mPlantHealth, Plant.mPlantMaxHealth);
                foreach (var (node, index) in model.Nodes.Select((node, index) => (node, index)))
                    if (node.Name == "Spikerock_bigspike3") transforms[index].gameObject.SetActive(damage == 0);
                    else if (node.Name == "Spikerock_bigspike2") transforms[index].gameObject.SetActive(damage < 2);
            }
            if (assets.Identity == FusionPlant.SolarSquash)
            {
                var sun = model.Clips["sun"];
                Apply(transforms, sun, idle % sun.Duration, true);
                ApplySprites(sun, idle % sun.Duration);
            }
            if ((FusionPlant.IsKelp(assets.Identity) || assets.Identity == FusionPlant.StrangleKelp || assets.Identity == FusionPlant.KelpSpreader)
                && !preview && Plant.mState == PlantState.TanglekelpGrabbing
                && (assets.Identity != FusionPlant.StrangleKelp || Plant.mTargetY == 0))
            {
                var grab = model.Clips["grab"];
                float time = assets.Identity == FusionPlant.StrangleKelp
                    ? Math.Clamp((83 - Plant.mStateCountdown) * .01f, 0, grab.Duration - .00001f)
                    : Math.Clamp((100 - Plant.mStateCountdown) / 100f, 0, .99999f) * grab.Duration;
                time = visualClock.Sample("grab-overlay", time);
                Apply(transforms, grab, time, true);
                ApplySprites(grab, time);
                AlignKelpGrab();
            }
            if (FusionPlant.IsFlameNut(assets.Identity) || assets.Identity == FusionPlant.HotPot
                || assets.Identity == FusionPlant.CabbageTriPult || assets.Identity == FusionPlant.Coldslaw
                || assets.Identity == FusionPlant.KimchiPult || assets.Identity == FusionPlant.SummerMelon
                || assets.Identity == FusionPlant.FlameCattail || assets.Identity == FusionPlant.DamagePult)
            {
                // The source fire layer runs continuously, additively at weight 1.
                var fire = model.Clips["fire"];
                float fireTime = idle * (assets.Identity == FusionPlant.FlameCattail ? 1.5f : 1);
                Apply(transforms, fire, fireTime % fire.Duration, true);
                ApplySprites(fire, fireTime % fire.Duration);
            }
            if (assets.Identity == FusionPlant.DoomNut || assets.Identity == FusionPlant.IllumiNut || FusionPlant.IsRetaliatingNut(assets.Identity) && assets.Identity != FusionPlant.ThornNut)
            {
                int damage = ElementalNuts.DamageStage(Plant.mPlantHealth, Plant.mPlantMaxHealth);
                for (int i = 0; i < model.Nodes.Length; ++i)
                {
                    var name = model.Nodes[i].Name;
                    if (name is "Wallnut_body" or "tnf1") transforms[i].gameObject.SetActive(damage == 0);
                    else if (name is "Wallnut_cracked1" or "tnf2") transforms[i].gameObject.SetActive(damage == 1);
                    else if (name is "Wallnut_cracked2" or "tnf3") transforms[i].gameObject.SetActive(damage == 2);
                }
            }
            if (assets.Identity == FusionPlant.ThornNut)
            {
                int damage = preview ? 0 : ElementalNuts.DamageStage(Plant.mPlantHealth, Plant.mPlantMaxHealth);
                for (int i = 0; i < model.Nodes.Length; ++i)
                {
                    var name = model.Nodes[i].Name;
                    // Cracked sprites are children of the animated intact
                    // sprite. Keep the parent active to retain its transform.
                    if (name is "Wallnut_body" or "A1" or "CactusNut_stem")
                    {
                        if (name == "Wallnut_body") transforms[i].gameObject.SetActive(true);
                        renderers[i]!.enabled = damage == 0;
                    }
                    else if (name is "Wallnut_cracked1" or "B1" or "CactusNut_stem_1")
                        transforms[i].gameObject.SetActive(damage == 1);
                    else if (name is "Wallnut_cracked2" or "C1" or "CactusNut_stem_2")
                        transforms[i].gameObject.SetActive(damage == 2);
                }
            }
            if ((FusionPlant.IsSquash(assets.Identity) || assets.Identity == FusionPlant.RoastedSquash) && !preview)
            {
                // Native mX/mY already carry the entire jump trajectory. Keep
                // source squash/stretch and facial curves without adding its
                // scripted downward Body travel a second time.
                if (baseClip == "jumppop")
                    for (int i = 0; i < model.Nodes.Length; ++i)
                        if (model.Nodes[i].Path == "Squash_body")
                        {
                            var position = transforms[i].localPosition;
                            position.y = model.Nodes[i].Position[1];
                            transforms[i].localPosition = position;
                        }
                if (Plant.mState is PlantState.SquashRising or PlantState.SquashFalling or PlantState.SquashDoneFalling)
                    renderers[model.GroundNode]!.enabled = false;
            }
            if ((assets.Identity == FusionPlant.HypnoMine || assets.Identity == FusionPlant.IceMine || assets.Identity == FusionPlant.TangleMine)
                && !preview && Plant.mShootingCounter > 0)
                ApplySprites(model.Clips["flash"], ((assets.Identity == FusionPlant.IceMine ? 20 : 8)
                    - Plant.mShootingCounter) * .01f);
            if (assets.Identity == FusionPlant.PotatoMasher && !preview && Plant.mState == PlantState.PotatoArmed)
                ApplySprites(model.Clips["flash"], idle % model.Clips["flash"].Duration);
            if (assets.Identity == FusionPlant.LandMine || assets.Identity == FusionPlant.SeaMine)
            {
                // Scripted source growth runs at 0.2 scale units per second.
                // The view changes scale only; native placement keeps its anchor.
                transforms[0].localScale *= preview ? 1 : MineFusions.LandMineScale(Plant);
                bool flash = !preview && Plant.mShootingCounter > 0;
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (model.Nodes[i].Name == "PotatoMine_light") transforms[i].gameObject.SetActive(!flash);
                    else if (model.Nodes[i].Name == "PotatoMine_light_1") transforms[i].gameObject.SetActive(flash);
            }
            if (assets.Identity == FusionPlant.MarigoldMine)
            {
                baseClip = preview || Plant.mState == PlantState.Notready ? "idle" : Plant.mState == PlantState.PotatoRising ? "rise" : Plant.mState == PlantState.PotatoArmed ? "armed" : "unrise";
                baseTime = preview || Plant.mState == PlantState.Notready ? idle : ((Plant.mState == PlantState.PotatoArmed ? TreasureMines.GiveTicks : TreasureMines.RiseTicks) - Plant.mStateCountdown) * .01f;
            }
            if (assets.Identity == FusionPlant.MarigoldMine && !preview && Plant.mShootingCounter > 0)
                ApplySprites(model.Clips["flash"], (8 - Plant.mShootingCounter) * .01f);
            if (assets.Identity == FusionPlant.SolarMine && !preview && Plant.mShootingCounter > 0)
                ApplySprites(model.Clips["flash"], (100 - Plant.mShootingCounter) * .01f);
            if (assets.Identity == FusionPlant.MineNut)
            {
                int damage = Plant.mPlantHealth < Plant.mPlantMaxHealth / 3 ? 2
                    : Plant.mPlantHealth < (long)Plant.mPlantMaxHealth * 2 / 3 ? 1 : 0;
                for (int i = 0; i < model.Nodes.Length; ++i)
                {
                    if (model.Nodes[i].Name == "PotatoMine_body") transforms[i].gameObject.SetActive(damage == 0);
                    else if (model.Nodes[i].Name == "PotatoMine_body2") transforms[i].gameObject.SetActive(damage == 1);
                    else if (model.Nodes[i].Name == "PotatoMine_body3") transforms[i].gameObject.SetActive(damage == 2);
                }
                if (!preview && Plant.mLaunchCounter > 0)
                    ApplySprites(model.Clips["flash"], (100 - Plant.mLaunchCounter) * .01f);
            }
            if (FusionPlant.IsPumpkinFusion(assets.Identity))
            {
                int stage = Plant.mPlantHealth * 3L > Plant.mPlantMaxHealth * 2L ? 0
                    : Plant.mPlantHealth * 3L > Plant.mPlantMaxHealth ? 1 : 2;
                for (int i = 0; i < model.Nodes.Length; ++i)
                {
                    var name = model.Nodes[i].Name;
                    if (assets.Identity == FusionPlant.PuffKin && name == "B2") transforms[i].gameObject.SetActive(stage == 0);
                    else if (assets.Identity == FusionPlant.PuffKin && name == "C1") transforms[i].gameObject.SetActive(stage <= 1);
                    else if (name == "pumpkin_damage0") transforms[i].gameObject.SetActive(stage == 0);
                    else if (name == "pumpkin_damage1") transforms[i].gameObject.SetActive(stage == 1);
                    else if (name == "pumpkin_damage2") transforms[i].gameObject.SetActive(stage == 2);
                    if (assets.Identity == FusionPlant.Chompkin && name.StartsWith("Chomper_stomach", StringComparison.Ordinal))
                        transforms[i].gameObject.SetActive(preview || int.Parse(name[^1..]) <= Plant.mTargetX);
                    if (assets.Identity == FusionPlant.PumpkinMine && name == "pumpkin_back2")
                        transforms[i].gameObject.SetActive(stage == 2);
                    if (assets.Identity == FusionPlant.PumpkinMine && name == "pumpkin_back" && renderers[i] != null)
                        transforms[i].gameObject.SetActive(stage != 2);
                    if (assets.Identity == FusionPlant.CryoPumpkin && name == "Pumpkin_back"
                        || assets.Identity == FusionPlant.PyroPumpkin && name == "pumpkin_back")
                        renderers[i]!.enabled = stage == 0;
                    if (assets.Identity == FusionPlant.CryoPumpkin && name == "Pumpkin_back2"
                        || assets.Identity == FusionPlant.PyroPumpkin && name == "pumpkin_back_1")
                        transforms[i].gameObject.SetActive(stage == 1);
                    if (assets.Identity == FusionPlant.CryoPumpkin && name == "Pumpkin_back3"
                        || assets.Identity == FusionPlant.PyroPumpkin && name == "pumpkin_back_2")
                        transforms[i].gameObject.SetActive(stage == 2);
                }
            }
            if (assets.Identity == FusionPlant.PyroPumpkin)
            {
                Apply(transforms, model.Clips["fire"], idle % model.Clips["fire"].Duration, false);
                ApplySprites(model.Clips["fire"], idle % model.Clips["fire"].Duration);
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (model.Nodes[i].Name == "1_1") transforms[i].gameObject.SetActive(Plant.mPlantHealth * 3L > Plant.mPlantMaxHealth);
            }
            if (assets.Identity == FusionPlant.PyroPumpkin)
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (model.Nodes[i].Name == "1_1" && renderers[i] != null) renderers[i]!.sortingOrder = 0;
            if (FusionPlant.IsContactUmbrella(assets.Identity))
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (renderers[i] != null && model.Nodes[i].Path.StartsWith("leaves/", StringComparison.Ordinal))
                        renderers[i]!.sortingOrder += 32;
            if (!preview && FusionPlant.IsPumpkinFusion(assets.Identity))
            {
                // A single SortingGroup cannot enclose another controller.
                // Place the source back and face on either side of its occupant.
                sorting.enabled = false;
                var inner = Plant.mBoard.GetPlantsOnLawn(Plant.mPlantCol, Plant.mRow).NormalPlant;
                int middle = inner?.mController?.m_meshRenderer?.sortingOrder ?? native.sortingOrder;
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (renderers[i] != null)
                    {
                        var path = model.Nodes[i].Path;
                        bool back = path.Contains("back", StringComparison.OrdinalIgnoreCase)
                            || i == model.GroundNode || model.Nodes[i].Name == "shadowimage";
                        // Fire attached to the back also stays behind the face.
                        renderers[i]!.sortingOrder = middle + (back ? -32 : 32) + renderers[i]!.sortingOrder;
                    }
            }
            if (assets.Identity == FusionPlant.NutShell)
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (model.Nodes[i].Name == "WallNut")
                        transforms[i].localScale = Vector3.one * (preview ? .01f
                            : Math.Clamp(.01f + (PumpkinFusions.KernelGrowthTicks - Plant.mLaunchCounter) / 5000f, .01f, 1f));
            if (assets.Identity == FusionPlant.ScaredyPot)
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (model.Nodes[i].Name == "eye")
                        transforms[i].gameObject.SetActive(preview || !OffensivePots.HasRecipient(Plant));
            if (assets.Identity == FusionPlant.SeaPot)
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (model.Nodes[i].Path == "Pot_top/Pot_top_1")
                        transforms[i].gameObject.SetActive(!preview && SeaPot.Wet(Plant));
            if (assets.Identity == FusionPlant.Multipeater)
            {
                Apply(transforms, model.Clips["fly"], idle % model.Clips["fly"].Duration, true);
                ApplySprites(model.Clips["fly"], idle % model.Clips["fly"].Duration);
            }
            if (assets.Identity == FusionPlant.BlowtorchShroom)
                ApplySprites(model.Clips["fire"], idle % model.Clips["fire"].Duration);
            if (assets.Identity == FusionPlant.FlameShroom && shoot is >= .35f)
                ApplySprites(model.Clips["fire"], shoot.Value - .35f);
            if (assets.Identity == FusionPlant.SolarBomb && !preview)
            {
                Apply(transforms, model.Clips["Bomb"], Math.Clamp(1 - Plant.mDoSpecialCountdown / 100f, 0, 1), false);
                Apply(transforms, model.Clips["shake"], idle % model.Clips["shake"].Duration, true);
            }
            if (assets.Identity == FusionPlant.BlackCherry || assets.Identity == FusionPlant.SolarFlare || assets.Identity == FusionPlant.HypnoPepper)
                Apply(transforms, model.Clips["shake"], idle % model.Clips["shake"].Duration, true);
            if (FusionPlant.IsEnergyNut(assets.Identity) || assets.Identity == FusionPlant.SquashNut || assets.Identity == FusionPlant.SilverNut || assets.Identity == FusionPlant.GoldenNut || assets.Identity == FusionPlant.SilverGarlic || assets.Identity == FusionPlant.HypnoGarlic || assets.Identity == FusionPlant.DoomGarlic || FusionPlant.IsReactiveNut(assets.Identity) || FusionPlant.IsSolarNut(assets.Identity) || assets.Identity == FusionPlant.BuckNut || assets.Identity == FusionPlant.CherryNut || assets.Identity == FusionPlant.PeaNut
                || assets.Identity == FusionPlant.Chompzilla)
            {
                // These are original alternate bodies, not additional layers.
                int damage = Plant.mPlantMaxHealth <= 0 ? 0
                    : Plant.mPlantHealth < Plant.mPlantMaxHealth / 3 ? 2
                    : Plant.mPlantHealth < (long)Plant.mPlantMaxHealth * 2 / 3 ? 1 : 0;
                for (int i = 0; i < model.Nodes.Length; ++i)
                {
                    var name = model.Nodes[i].Name;
                    if (name == "Garlic_body1") transforms[i].gameObject.SetActive(damage == 0);
                    else if (name == "Garlic_body2") transforms[i].gameObject.SetActive(damage == 1);
                    else if (name == "Garlic_body3") transforms[i].gameObject.SetActive(damage == 2);
                    else if (name == "Wallnut_body") transforms[i].gameObject.SetActive(damage == 0);
                    else if (name == "Wallnut_cracked1") transforms[i].gameObject.SetActive(damage == 1);
                    else if (name == "Wallnut_cracked2") transforms[i].gameObject.SetActive(damage == 2);
                    else if (name is "Chomper_body" or "Chomper_face_upper") transforms[i].gameObject.SetActive(damage == 0);
                    else if (name is "Chomper_body1" or "Chomper_face_upper1") transforms[i].gameObject.SetActive(damage == 1);
                    else if (name is "Chomper_body2" or "Chomper_face_upper2") transforms[i].gameObject.SetActive(damage == 2);
                }
            }
            if (assets.Identity == FusionPlant.UmbrellaShell)
            {
                // ReplaceSprite changes the three children of each original
                // leaf. Exact third boundaries keep the previous source pose.
                int damage = Plant.mPlantHealth > (long)Plant.mPlantMaxHealth * 2 / 3 ? 0
                    : Plant.mPlantHealth < Plant.mPlantMaxHealth / 3 ? 2
                    : Plant.mPlantHealth < (long)Plant.mPlantMaxHealth * 2 / 3
                        && Plant.mPlantHealth > Plant.mPlantMaxHealth / 3 ? 1 : -1;
                if (damage >= 0)
                    for (int i = 0; i < model.Nodes.Length; ++i)
                    {
                        var node = model.Nodes[i];
                        if (node.Parent < 0 || !model.Nodes[node.Parent].Name.StartsWith("Umbrellaleaf_leaf")) continue;
                        transforms[i].gameObject.SetActive(damage == (node.Name.EndsWith("_2") ? 2 : node.Name.EndsWith("_1") ? 1 : 0));
                    }
            }
            if (assets.Identity == FusionPlant.FoulShroom)
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (model.Nodes[i].Name is "eye1" or "eye2")
                    {
                        var rotation = model.Nodes[i].Rotation;
                        transforms[i].localRotation = new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);
                    }
            if (assets.Identity == FusionPlant.FootballNut)
            {
                int damage = Plant.mPlantHealth <= Plant.mPlantMaxHealth / 3 ? 2
                    : Plant.mPlantHealth <= (long)Plant.mPlantMaxHealth * 2 / 3 ? 1 : 0;
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (model.Nodes[i].Name is "tnf1" or "tnf2" or "tnf3")
                        transforms[i].gameObject.SetActive(model.Nodes[i].Name == $"tnf{damage + 1}");
            }
            if (assets.Identity == FusionPlant.Bucket || assets.Identity == FusionPlant.FootballHelmet)
            {
                int damage = Plant.mPlantHealth <= Plant.mPlantMaxHealth / 3 ? 2
                    : Plant.mPlantHealth <= (long)Plant.mPlantMaxHealth * 2 / 3 ? 1 : 0;
                for (int i = 0; i < model.Nodes.Length; ++i)
                {
                    if (model.Nodes[i].Name == "Body") renderers[i]!.enabled = damage == 0;
                    else if (model.Nodes[i].Name == "Body_1") transforms[i].gameObject.SetActive(damage == 1);
                    else if (model.Nodes[i].Name == "Body_2") transforms[i].gameObject.SetActive(damage == 2);
                }
            }
            if (assets.Identity == FusionPlant.WallopShroom)
            {
                int stage = WeedFusions.RockDamageStage(Plant.mPlantHealth, Plant.mPlantMaxHealth);
                for (int i = 0; i < model.Nodes.Length; ++i)
                    foreach (var part in new[] { "FumeShroom_head", "FumeShroom_body" })
                        if (model.Nodes[i].Name == part && renderers[i] is { } renderer) renderer.enabled = stage == 0;
                        else if (model.Nodes[i].Name == part + "_1" || model.Nodes[i].Name == part + "_2")
                            transforms[i].gameObject.SetActive(model.Nodes[i].Name == part + "_" + stage);
            }
            if (model.DamageSprites != null)
            {
                int damage = Plant.mPlantHealth < Plant.mPlantMaxHealth / 3 ? 2
                    : Plant.mPlantHealth < (long)Plant.mPlantMaxHealth * 2 / 3 ? 1 : 0;
                for (int i = 0; i < model.Nodes.Length; ++i)
                    if (renderers[i] is { } renderer && model.DamageSprites.TryGetValue(model.Nodes[i].Name, out var sprites))
                        renderer.sprite = assets.Sprites[damage == 0 ? model.Nodes[i].Sprite : sprites[damage - 1]];
            }
            Color tint = Color.white;
            Color additive = Color.clear;
            if (preview) tint = native.Cast<SpriteRenderer>().color;
            else
            {
                var effects = Controller.m_materialEffectController;
                if (effects != null && effects)
                {
                    tint = effects.m_overrideColor;
                    if (effects.m_enableOverlayColor)
                        tint = Color.Lerp(tint, effects.m_overlayColor, effects.m_overlayColor.a);
                    if (effects.m_enableAdditiveColor) additive = effects.m_additiveColor;
                }
            }
            for (int i = 0; i < renderers.Length; ++i)
                if (renderers[i] != null)
                {
                    var color = model.Nodes[i].Color;
                    float brightness = !preview && assets.Identity == FusionPlant.SunnyPumpkin && Plant.mStartRow > 0 && i != model.GroundNode
                        ? 1 + 3 * (1 - Math.Abs(Plant.mStartRow - 25) / 25f)
                        : !preview && assets.Identity == FusionPlant.SunriseShroom && i != model.GroundNode ? SunflowerFusions.Brightness(Plant)
                        : !preview && (SunflowerFusions.Producer(assets.Identity) || assets.Identity == FusionPlant.GoldenSunflower)
                        && i != model.GroundNode ? SunflowerFusions.Brightness(Plant)
                        : !preview && assets.Identity == FusionPlant.PennyShroom
                        && i != model.GroundNode ? PuffSupport.Brightness(Plant)
                        : !preview && assets.Identity == FusionPlant.SunnyShroom
                        && i != model.GroundNode ? ScaredySupport.Brightness(Plant)
                        : !preview && assets.Identity == FusionPlant.SundaeShroom
                        && i != model.GroundNode ? IceInstantFusions.Brightness(Plant)
                        : !preview && assets.Identity == FusionPlant.SolarShooter
                        && i != model.GroundNode ? SolarShooter.Brightness(Plant)
                        : !preview && (assets.Identity == FusionPlant.TwinMarigold || assets.Identity == FusionPlant.SilverPot) && i != model.GroundNode
                            ? TwinMarigold.Brightness(Plant)
                            : !preview && assets.Identity == FusionPlant.Torchflower && i != model.GroundNode
                                ? Torchflower.Brightness(Plant) : 1;
                    renderers[i]!.color = new Color(color[0] * tint.r * brightness, color[1] * tint.g * brightness,
                        color[2] * tint.b * brightness, AnimatedAlpha(model, i, baseClip, baseTime, shootClip, shoot) * tint.a);
                    // Keep effect values per renderer, never on the shared
                    // material: one valid fusion target must not tint its peers.
                    // Reset strength every frame, including cancel/tool changes.
                    renderers[i]!.GetPropertyBlock(colorEffects);
                    colorEffects.SetColor("_BlendColor", additive);
                    // Soften selection only; damage flashes keep their strength.
                    float strength = additive.a * (Highlighted ? .5f : 1f);
                    colorEffects.SetFloat("_BlendStrength", i == model.GroundNode ? 0 : strength);
                    renderers[i]!.SetPropertyBlock(colorEffects);
                }
            UpdateRay();
        }

        public void Dispose()
        {
            ReleaseSkins();
            // Laser sprite belongs to shared FumeEffects assets.
            if (root != null && root) { root.SetActive(false); Object.Destroy(root); }
            root = null;
            if (ownsNative && native != null && native) native.enabled = nativeWasEnabled;
            ownsNative = false;
            foreach (var shadow in shadows) if (shadow.Renderer) shadow.Renderer.enabled = shadow.Enabled;
            shadows.Clear();
            foreach (var part in nativeParts.Values) if (part.Renderer) part.Renderer.enabled = part.Enabled;
            nativeParts.Clear();
        }
    }
}

// Capture the actual ingredient while the glove constructs its two previews.
// GetCarriedPlant is intentionally not yet published during that constructor.
[HarmonyPatch]
internal static class SnowRepeaterGlovePreviewScope
{
    private static MethodBase TargetMethod() => AccessTools.Constructor(
        typeof(LawnGloveMod).Assembly.GetType("LawnGlove.CarriedPlant", true), new[] { typeof(Board), typeof(Plant) });

    private static void Prefix(Plant plant, out Plant? __state)
    {
        __state = ImportedPeaVisuals.PreviewIngredient;
        ImportedPeaVisuals.PreviewIngredient = FusionPlant.HasImportedVisuals(FusionPlant.From(plant)) ? plant : null;
    }

    private static void Finalizer(Plant? __state) => ImportedPeaVisuals.PreviewIngredient = __state;
}

[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.CreatePreviewController),
    new[] { typeof(SeedType), typeof(ReloadedObject) })]
internal static class SnowRepeaterPreviewPatch
{
    private static void Postfix(PreviewController __result) => ImportedPeaVisuals.RegisterPreview(__result);
}

[HarmonyPatch]
internal static class ImportedPeaVisualPlantCleanup
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Plant), nameof(Plant.Die));
        yield return AccessTools.Method(typeof(Plant), nameof(Plant.Reset));
    }
    private static void Prefix(Plant __instance) => ImportedPeaVisuals.Forget(__instance);
}

[HarmonyPatch(typeof(ReloadedController), nameof(ReloadedController.Die))]
internal static class ImportedPeaVisualControllerCleanup
{
    private static void Prefix(ReloadedController __instance) => ImportedPeaVisuals.ForgetController(__instance);
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class ImportedPeaVisualBoardCleanup
{
    private static void Prefix(Board __instance) => ImportedPeaVisuals.ForgetBoard(__instance);
}
