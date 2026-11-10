using Il2CppReloaded.Gameplay;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace FusionMod;

internal static partial class ImportedPeaVisuals
{
    private static readonly Assets GlacierProjectileAssets = new(FusionPlant.GlacierShroom, "GlacierProjectile");
    private static readonly Assets WeedProjectileAssets = new(FusionPlant.WeedSpreader, "WeedSpreaderProjectile");
    internal static void EnsureWeedProjectile(Projectile shot) => RegisterProjectile(shot, WeedProjectileAssets);
    internal static void EnsureUmbrellaProjectile(Projectile shot)
    {
        if (UmbrellaVariants.ReflectedPea(shot)) RegisterProjectile(shot, PlainPeaProjectileAssets);
        else if (UmbrellaVariants.ReflectedMelon(shot)) RegisterProjectile(shot, MelonProjectileAssets);
    }
    internal static void EnsureGlacierProjectile(Projectile projectile) => RegisterProjectile(projectile, GlacierProjectileAssets);
    private static readonly Assets PhoenixProjectileAssets = new(FusionPlant.PhoenixThreepeater, "PhoenixProjectile");
    private static readonly Assets SolarCabbageProjectileAssets = new(FusionPlant.SolarPult, "SolarCabbageProjectile");
    internal static void EnsurePhoenixProjectile(Projectile projectile)
    {
        RegisterProjectile(projectile, PhoenixProjectileAssets);
    }
    private static readonly Assets DoomPeaProjectileAssets = new(FusionPlant.DoomPea, "DoomPeaProjectile");
    private static readonly Assets DoomProjectileAssets = new(FusionPlant.GatlingDoom, "DoomProjectile");
    private static readonly Assets BigDoomProjectileAssets = new(FusionPlant.GatlingDoom, "BigDoomProjectile");
    private static readonly Assets DoomMeteorProjectileAssets = new(FusionPlant.DeathStar, "DoomMeteorProjectile");
    internal static void EnsureDoomProjectile(Projectile projectile)
    {
        var assets = DoomProjectiles.Family(projectile) switch
        { 1 => DoomPeaProjectileAssets, 2 => DoomProjectileAssets, 3 => BigDoomProjectileAssets,
            4 => DoomMeteorProjectileAssets, _ => null };
        if (assets == null) return;
        RegisterProjectile(projectile, assets);
    }
    private static readonly Assets PotatoSeedProjectileAssets = new(FusionPlant.CraterTot, "PotatoSeedProjectile");
    internal static void EnsurePotatoSeedProjectile(Projectile projectile)
    {
        RegisterProjectile(projectile, PotatoSeedProjectileAssets);
    }
    private static readonly Assets IceCactusProjectileAssets = new(FusionPlant.IceCactus, "IceCactusProjectile");
    private static readonly Assets MagnetCactusProjectileAssets = new(FusionPlant.MagnetCactus, "MagnetCactusProjectile");
    private static readonly Assets DoomCactusProjectileAssets = new(FusionPlant.DoomCactus, "DoomCactusProjectile");
    private static readonly Assets LightSpikeProjectileAssets = new(FusionPlant.LumosCactus, "LightSpikeProjectile");
    private static readonly Assets LightPierceProjectileAssets = new(FusionPlant.LumosCactus, "LightPierceProjectile");
    private static readonly Assets CactusSpikeProjectileAssets = new(FusionPlant.Thornweed, "CactusSpikeProjectile");
    private static readonly Assets CactusStarProjectileAssets = new(FusionPlant.Starthorn, "CactusStarProjectile");
    internal static void EnsureCactusProjectile(Projectile projectile)
    {
        var assets = CactusProjectiles.Family(projectile) switch
        { 1 => IceCactusProjectileAssets, 2 => MagnetCactusProjectileAssets, 3 => DoomCactusProjectileAssets,
            4 => LightPierceProjectileAssets, 6 => LightSpikeProjectileAssets,
            5 => CactusStarProjectileAssets, 7 => CactusSpikeProjectileAssets, 8 => CactusSpikeProjectileAssets, _ => null };
        if (assets == null) return;
        RegisterProjectile(projectile, assets);
    }
    private static readonly Assets LettuceProjectileAssets = new(FusionPlant.LettucePult, "LettuceProjectile");
    private static readonly Assets LittleLettuceProjectileAssets = new(FusionPlant.LettucePult, "LittleLettuceProjectile");
    private static readonly Assets ButternutProjectileAssets = new(FusionPlant.ButternutPult, "ButternutProjectile");
    private static readonly Assets ButterSquashProjectileAssets = new(FusionPlant.ButternutPult, "ButterSquashProjectile");
    private static readonly Assets GourdProjectileAssets = new(FusionPlant.GourdPult, "GourdProjectile");
    internal static void EnsureSquashLobProjectile(Projectile projectile)
    {
        var assets = SquashLobbers.Family(projectile) switch
        {
            1 => LettuceProjectileAssets, 2 => LittleLettuceProjectileAssets,
            3 or 8 => ButternutProjectileAssets, 4 or 9 => ButterSquashProjectileAssets,
            5 or 6 => GourdProjectileAssets, 7 => MelonProjectileAssets, _ => null
        };
        if (assets == null) return;
        RegisterProjectile(projectile, assets);
    }
    private static readonly Assets WallopProjectileAssets = new(FusionPlant.WallopShroom, "WallopProjectile");
    internal static void EnsureWallopProjectile(Projectile projectile)
    {
        RegisterProjectile(projectile, WallopProjectileAssets);
    }

    private static readonly Assets MiniSquashProjectileAssets = new(FusionPlant.SquashSpreader, "MiniSquashProjectile");
    internal static void EnsureMiniSquashProjectile(Projectile projectile)
    {
        RegisterProjectile(projectile, MiniSquashProjectileAssets);
    }
    private static readonly Assets KelpProjectileAssets = new(FusionPlant.TingleKelp, "KelpProjectile");
    internal static void EnsureKelpProjectile(Projectile projectile)
    {
        if (KelpProjectiles.Contains(projectile)) RegisterProjectile(projectile, KelpProjectileAssets);
    }
    private static readonly HashSet<(IntPtr Board, string Name)> ReportedProjectileBoards = new();
    private static readonly Assets PolarProjectileAssets = new(FusionPlant.BreezeBlover, "PolarProjectile");
    private static readonly Assets BloverSunProjectileAssets = new(FusionPlant.SolarBlover, "BloverSunProjectile");
    private static readonly Assets SmallCabbageProjectileAssets = new(FusionPlant.SproutPult, "SmallCabbageProjectile");
    private static readonly Assets SmallIronProjectileAssets = new(FusionPlant.BuckShroom, "SmallIronProjectile");
    private static readonly Assets SmallKernelProjectileAssets = new(FusionPlant.BabyCorn, "SmallKernelProjectile");
    private static readonly Assets SmallButterProjectileAssets = new(FusionPlant.BabyCorn, "SmallButterProjectile");
    private static readonly Assets SmallMelonProjectileAssets = new(FusionPlant.SlicePult, "SmallMelonProjectile");
    private static readonly Assets SilverCabbageProjectileAssets = new(FusionPlant.SilverCabbage, "SilverCabbageProjectile");
    private static readonly Assets SilverKernelProjectileAssets = new(FusionPlant.SilverKernel, "SilverKernelProjectile");
    private static readonly Assets SilverButterProjectileAssets = new(FusionPlant.SilverKernel, "SilverButterProjectile");
    private static readonly Assets SilverMelonProjectileAssets = new(FusionPlant.SilverMelon, "SilverMelonProjectile");
    private static readonly Assets PuffPeaProjectileAssets = new(FusionPlant.PeaShroom, "PuffPeaProjectile");
    private static readonly Assets IcicleProjectileAssets = new(FusionPlant.IcicleShroom, "IcicleProjectile");
    private static readonly Assets FrenzyProjectileAssets = new(FusionPlant.FrenzyShroom, "FrenzyProjectile");
    private static readonly Assets SpeedyProjectileAssets = new(FusionPlant.SpeedyShroom, "SpeedyProjectile");
    private static readonly Assets ThawedShiveryProjectileAssets = new(FusionPlant.ShiveryShroom, "PlainPeaProjectile");
    private static readonly Assets CherrySporeAssets = new(FusionPlant.BlastyShroom, "CherrySpore");
    private static readonly Assets PotatoSporeAssets = new(FusionPlant.SpuddyShroom, "PotatoSpore");
    private static readonly Assets ShiveryProjectileAssets = new(FusionPlant.ShiveryShroom, "ShiverySporeProjectile");
    private static readonly Assets GutsyProjectileAssets = new(FusionPlant.GutsyShroom, "GutsyProjectile");
    private static readonly Assets PlainPeaProjectileAssets = new(FusionPlant.PeaSquash, "PlainPeaProjectile");
    private static readonly Assets PlainStarProjectileAssets = new(FusionPlant.StarNut, "PlainStarProjectile");
    private static readonly Assets HypnoFireProjectileAssets = new(FusionPlant.Hypnowood, "HypnoFireProjectile");
    private static readonly Assets HypnoProjectileAssets = new(FusionPlant.HypnoPea, "HypnoProjectile");
    private static readonly Assets CherryDroneProjectileAssets = new(FusionPlant.Boomwood, "CherryDroneProjectile");
    private static readonly Assets FireCherryProjectileAssets = new(FusionPlant.Boomwood, "FireCherryProjectile");
    private static readonly Assets SmallFireProjectileAssets = new(FusionPlant.HotPotato, "SmallFireProjectile");
    private static readonly Assets PotatoFireProjectileAssets = new(FusionPlant.HotPotato, "PotatoFireProjectile");
    internal static void EnsureChargeTorchProjectile(Projectile shot)
    {
        if (ChargeTorchFusions.IsFireCherry(shot)) RegisterProjectile(shot, shot.mMotionType == ProjectileMotion.Puff ? CherryDroneProjectileAssets : FireCherryProjectileAssets);
        else if (shot.mCobTargetX == ChargeTorchFusions.SmallFire) RegisterProjectile(shot, SmallFireProjectileAssets);
        else if (shot.mCobTargetX == ChargeTorchFusions.PotatoFire) RegisterProjectile(shot, PotatoFireProjectileAssets);
    }
    private static readonly Assets ScorchProjectileAssets = new(FusionPlant.Scorchwood, "ScorchProjectile");
    private static readonly Assets StenchFireProjectileAssets = new(FusionPlant.Stenchwood, "StenchFireProjectile");
    internal static void EnsureTorchProjectile(Projectile shot)
    {
        if (shot.mCobTargetX == SupportTorchFusions.Scorch) RegisterProjectile(shot, ScorchProjectileAssets);
        else if (shot.mCobTargetX == SupportTorchFusions.Stench) RegisterProjectile(shot, StenchFireProjectileAssets);
        else if (shot.mCobTargetX == SupportTorchFusions.Garlic) RegisterPoisonProjectile(shot);
    }
    private static readonly Assets PoisonProjectileAssets = new(FusionPlant.PoisonPea, "PoisonProjectile");
    private static readonly Assets BlazeProjectileAssets = new(FusionPlant.BlazePea, "BlazeProjectile");
    private static readonly Assets PotatoProjectileAssets = new(FusionPlant.PeatatoMine, "PotatoProjectile");
    private static readonly Assets HelmetProjectileAssets = new(FusionPlant.FootballGatling, "HelmetProjectile");
    private static readonly Assets IronProjectileAssets = new(FusionPlant.Buckshooter, "IronProjectile");
    private static readonly Assets SolarNutBallAssets = new(FusionPlant.TwinSolarNut, "SolarNutBall");
    private static readonly Assets FireNutProjectileAssets = new(FusionPlant.HotPotato, "FireNutProjectile");
    private static readonly Assets NutProjectileAssets = new(FusionPlant.PeaNut, "NutProjectile");
    private static readonly Assets ZombieBlockAssets = new(FusionPlant.Chompzilla, "ZombieBlockProjectile");
    private static readonly Assets ZombieBlock2Assets = new(FusionPlant.Chompzilla, "ZombieBlock2Projectile");
    private static readonly Assets ZombieBlock3Assets = new(FusionPlant.Chompzilla, "ZombieBlock3Projectile");
    private static readonly Assets ChompProjectileAssets = new(FusionPlant.ChompShooter, "ChompProjectile");
    private static readonly Assets ExplodeProjectileAssets = new(FusionPlant.ExplodeOShooter, "ExplodeProjectile");
    private static readonly Assets CherryProjectileAssets = new(FusionPlant.CherryShooter, "CherryProjectile");
    private static readonly Assets CherrySpreaderProjectileAssets = new(FusionPlant.CherrySpreader, "CherrySpreaderProjectile");
    private static readonly Assets CherrySpreaderBombProjectileAssets = new(FusionPlant.CherrySpreader, "CherrySpreaderBombProjectile");
    private static readonly Assets SolarProjectileAssets = new(FusionPlant.SolarShooter, "SolarProjectile");
    private static readonly Assets KernelProjectileAssets = new(FusionPlant.KernelTriPult, "KernelProjectile");
    private static readonly Assets ButterProjectileAssets = new(FusionPlant.KernelTriPult, "ButterProjectile");
    private static readonly Assets MelonProjectileAssets = new(FusionPlant.MelonTriPult, "MelonProjectile");
    private static readonly Assets TacoProjectileAssets = new(FusionPlant.TacoPult, "TacoProjectile");
    private static readonly Assets ButterTacoProjectileAssets = new(FusionPlant.TacoPult, "ButterTacoProjectile");
    private static readonly Assets CobProjectileAssets = new(FusionPlant.CobPult, "CobProjectile");
    private static readonly Assets ButterCobProjectileAssets = new(FusionPlant.CobPult, "ButterCobProjectile");
    private static readonly Assets OverripeProjectileAssets = new(FusionPlant.OverripeMelon, "OverripeProjectile");
    private static readonly Assets TriCabbageProjectileAssets = new(FusionPlant.CabbageTriPult, "TriCabbageProjectile");
    private static readonly Assets CabbageProjectileAssets = new(FusionPlant.CabbageTriPult, "CabbageProjectile");
    private static readonly Assets ColdslawProjectileAssets = new(FusionPlant.Coldslaw, "ColdslawProjectile");
    private static readonly Assets KimchiProjectileAssets = new(FusionPlant.KimchiPult, "KimchiProjectile");
    private static readonly Assets IceKernelProjectileAssets = new(FusionPlant.Cobsicle, "IceKernelProjectile");
    private static readonly Assets IceButterProjectileAssets = new(FusionPlant.Cobsicle, "IceButterProjectile");
    private static readonly Assets SaladProjectileAssets = new(FusionPlant.SaladPult, "SaladProjectile");
    private static readonly Assets GarlicCabbageProjectileAssets = new(FusionPlant.GarbagePult, "GarlicCabbageProjectile");
    private static readonly Assets GarlicKernelProjectileAssets = new(FusionPlant.ClovePult, "GarlicKernelProjectile");
    private static readonly Assets GarlicButterProjectileAssets = new(FusionPlant.ClovePult, "GarlicButterProjectile");
    private static readonly Assets GarlicMelonProjectileAssets = new(FusionPlant.GarlicPult, "GarlicMelonProjectile");
    private static readonly Assets HypnoMelonProjectileAssets = new(FusionPlant.HypnoMelon, "HypnoMelonProjectile");
    private static readonly Assets IceTrackProjectileAssets = new(FusionPlant.FrostCattail, "IceTrackProjectile");
    private static readonly Assets FireTrackProjectileAssets = new(FusionPlant.FlameCattail, "FireTrackProjectile");
    private static readonly Assets FireMelonProjectileAssets = new(FusionPlant.SummerMelon, "FireMelonProjectile");
    private static readonly Assets GarlicStarProjectileAssets = new(FusionPlant.Starlic, "GarlicStarProjectile");
    private static readonly Assets IceStarProjectileAssets = new(FusionPlant.WinterStarfruit, "IceStarProjectile");
    private static readonly Assets FireStarProjectileAssets = new(FusionPlant.SpicyStarfruit, "FireStarProjectile");
    private static readonly Assets StarPeaProjectileAssets = new(FusionPlant.Starshooter, "StarPeaProjectile");

    private static readonly Assets SeaSporeProjectileAssets = new(FusionPlant.MinnowShroom, "SeaSporeProjectile");
    private static readonly Assets SeaSpikeProjectileAssets = new(FusionPlant.UrchinShroom, "SeaSpikeProjectile");
    private static readonly Assets SeaStarProjectileAssets = new(FusionPlant.SeaStar, "SeaStarProjectile");
    internal static void EnsureSeaProjectile(Projectile projectile)
    {
        var family = SeaProjectiles.Family(projectile);
        if (family != 0) RegisterProjectile(projectile, family == 3 ? SeaSpikeProjectileAssets : SeaSporeProjectileAssets);
    }

    private static readonly Assets IronStarProjectileAssets = new(FusionPlant.IronStar, "IronStarProjectile");
    private static readonly Assets IronMelonProjectileAssets = new(FusionPlant.MetalPult, "IronMelonProjectile");
    private static readonly Assets IronKernelProjectileAssets = new(FusionPlant.BoltPult, "IronKernelProjectile");
    private static readonly Assets IronButterProjectileAssets = new(FusionPlant.BoltPult, "IronButterProjectile");
    private static readonly Assets MagnetKernelProjectileAssets = new(FusionPlant.KernelMorph, "MagnetKernelProjectile");
    private static readonly Assets MagnetButterProjectileAssets = new(FusionPlant.KernelMorph, "MagnetButterProjectile");
    private static readonly Assets MetalStarProjectileAssets = new(FusionPlant.Starmorph, "MetalStarProjectile");
    private static readonly Assets MetalMelonProjectileAssets = new(FusionPlant.MelonMorph, "MetalMelonProjectile");

    private static readonly Assets StarCapProjectileAssets = new(FusionPlant.StarburstShroom, "StarCapProjectile");
    private static readonly Assets DoomCabbageProjectileAssets = new(FusionPlant.DamagePult, "DoomCabbageProjectile");

    internal static void EnsureStarCapProjectile(Projectile projectile)
    {
        if (!StarCaps.Contains(projectile)) return;
        RegisterProjectile(projectile, StarCapProjectileAssets);
    }

    internal static void EnsureStarProjectile(Projectile projectile)
    {
        var assets = StarProjectiles.Family(projectile) switch {
            1 => GarlicStarProjectileAssets, 2 => IceStarProjectileAssets,
            3 or 9 => FireStarProjectileAssets, 4 => StarPeaProjectileAssets,
            5 => PlainStarProjectileAssets, 6 => SeaStarProjectileAssets, 7 => MetalStarProjectileAssets, 8 => IronStarProjectileAssets, _ => null };
        if (assets == null) return;
        RegisterProjectile(projectile, assets);
    }

    private static readonly Dictionary<IntPtr, ProjectileView> ImportedProjectiles = new();

    internal static void EnsureCattailProjectile(Projectile projectile)
    {
        var assets = CattailFusions.Family(projectile) switch
        { 1 => IceTrackProjectileAssets, 2 => FireTrackProjectileAssets, _ => null };
        if (assets != null) RegisterProjectile(projectile, assets);
    }

    internal static void EnsureElementalLobProjectile(Projectile projectile)
    {
        var assets = ElementalLobbers.Family(projectile) switch
        {
            1 => ColdslawProjectileAssets, 2 or 3 => KimchiProjectileAssets,
            4 => IceKernelProjectileAssets, 5 => IceButterProjectileAssets,
            6 => GarlicCabbageProjectileAssets,
            7 => GarlicKernelProjectileAssets,
            8 => GarlicButterProjectileAssets,
            9 => GarlicMelonProjectileAssets,
            10 => FireMelonProjectileAssets,
            11 => HypnoMelonProjectileAssets,
            _ => null
        };
        if (assets != null) RegisterProjectile(projectile, assets);
    }

    internal static void EnsureNutProjectile(Projectile projectile)
    {
        var assets = NutProjectiles.Family(projectile) switch
        { 1 => CabbageProjectileAssets, 2 => MelonProjectileAssets, 3 => NutProjectileAssets, _ => null };
        if (assets != null) RegisterProjectile(projectile, assets);
    }

    internal static void EnsureMixedLobProjectile(Projectile projectile)
    {
        var assets = LobberProjectiles.Family(projectile) switch
        {
            1 => TacoProjectileAssets, 2 => ButterTacoProjectileAssets, 3 => CobProjectileAssets,
            4 => ButterCobProjectileAssets, 5 => OverripeProjectileAssets,
            7 => SaladProjectileAssets, 8 => MetalMelonProjectileAssets, 9 => MelonProjectileAssets,
            10 => MagnetKernelProjectileAssets, 11 => MagnetButterProjectileAssets, 12 => IronMelonProjectileAssets, 13 => DoomCabbageProjectileAssets,
            14 => ButterProjectileAssets, 15 => SolarCabbageProjectileAssets,
            16 => IronKernelProjectileAssets, 17 => IronButterProjectileAssets,
            6 => LobberProjectiles.Remaining(projectile) == 0 ? CabbageProjectileAssets : TriCabbageProjectileAssets,
            _ => null
        };
        if (assets != null) RegisterProjectile(projectile, assets);
    }

    internal static void EnsureTriLobProjectile(Projectile projectile)
    {
        var assets = TriLobbers.Family(projectile) switch
        { 1 => KernelProjectileAssets, 2 => ButterProjectileAssets, 3 => MelonProjectileAssets, _ => null };
        if (assets != null) RegisterProjectile(projectile, assets);
    }

    internal static void EnsureSilverProjectile(Projectile projectile)
    {
        var assets = SilverLobbers.Family(projectile) switch
        {
            1 => SilverCabbageProjectileAssets, 2 => SilverMelonProjectileAssets,
            3 => SilverKernelProjectileAssets, 4 => SilverButterProjectileAssets, _ => null
        };
        if (assets != null) RegisterProjectile(projectile, assets);
    }

    internal static void EnsurePolarProjectile(Projectile projectile)
    {
        RegisterProjectile(projectile, PolarProjectileAssets);
    }

    internal static void EnsureBloverSunProjectile(Projectile projectile)
    {
        RegisterProjectile(projectile, BloverSunProjectileAssets);
    }

    internal static void EnsurePuffLobProjectile(Projectile projectile)
    {
        var assets = PuffLobbers.Family(projectile) switch
        {
            1 => SmallCabbageProjectileAssets, 2 => SmallKernelProjectileAssets,
            3 => SmallButterProjectileAssets, 4 => SmallMelonProjectileAssets, _ => null
        };
        if (assets != null) RegisterProjectile(projectile, assets);
    }

    internal static void EnsurePuffProjectile(Projectile projectile)
    {
        RegisterProjectile(projectile, PuffPeaProjectileAssets);
    }

    internal static void EnsurePuffSpecialProjectile(Projectile projectile)
    {
        var assets = PuffSpecials.IsIcicle(projectile) ? IcicleProjectileAssets : PotatoSporeAssets;
        RegisterProjectile(projectile, assets);
    }

    internal static void EnsureScaredyProjectile(Projectile projectile)
    {
        var assets = ScaredyProjectiles.Family(projectile) switch
        {
            1 when projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea => SpeedyProjectileAssets,
            2 => CherrySporeAssets,
            3 when projectile.mProjectileType == ProjectileType.Snowpea => ShiveryProjectileAssets,
            3 when projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea => ThawedShiveryProjectileAssets,
            4 => PotatoSporeAssets,
            5 => FrenzyProjectileAssets,
            6 => GutsyProjectileAssets,
            _ => null
        };
        if (assets == null) return;
        // The factory may already have registered a generic pea view before
        // the shooter assigns its saved family. Require the actual asset too.
        RegisterProjectile(projectile, assets);
    }

    internal static void EnsureGutsyProjectile(Projectile projectile)
    {
        RegisterProjectile(projectile, GutsyProjectileAssets);
    }
    internal static void RegisterScorchedProjectile(Projectile projectile) => RegisterProjectile(projectile, PlainPeaProjectileAssets);
    internal static void EnsureScorchedProjectile(Projectile projectile)
    {
        RegisterScorchedProjectile(projectile);
    }
    internal static void EnsureHypnoProjectile(Projectile projectile)
    {
        RegisterProjectile(projectile, HypnoShooters.IsFire(projectile) ? HypnoFireProjectileAssets : HypnoProjectileAssets);
    }
    internal static void RegisterPoisonProjectile(Projectile projectile) => RegisterProjectile(projectile, PoisonProjectileAssets);
    internal static void RegisterBlazeProjectile(Projectile projectile) => RegisterProjectile(projectile, BlazeProjectileAssets);
    internal static void RegisterSolarNutBall(Projectile projectile) => RegisterProjectile(projectile, SolarNutBallAssets);
    internal static void RegisterPotatoProjectile(Projectile projectile) => RegisterProjectile(projectile, PotatoProjectileAssets);
    internal static void EnsurePotatoProjectile(Projectile projectile) => RegisterProjectile(projectile, PotatoProjectileAssets);
    internal static void RegisterIronProjectile(Projectile projectile) => RegisterProjectile(projectile,
        IronProjectiles.IsSmall(projectile) ? SmallIronProjectileAssets
            : IronProjectiles.IsRed(projectile) ? HelmetProjectileAssets : IronProjectileAssets);
    internal static void EnsureIronProjectile(Projectile projectile)
    {
        RegisterIronProjectile(projectile);
    }
    private static Assets RollingAssets(Projectile projectile, bool nut) => projectile.mCobTargetX == RollingProjectiles.IgnitedNut ? FireNutProjectileAssets :
        RollingProjectiles.Chunk(projectile) switch
        { 1 => ZombieBlockAssets, 2 => ZombieBlock2Assets, 3 => ZombieBlock3Assets,
            _ => RollingProjectiles.TryShot(projectile, out var shot) && shot.SolarBall ? SolarNutBallAssets
                : nut ? NutProjectileAssets : ChompProjectileAssets };
    internal static void RegisterRollingProjectile(Projectile projectile, bool nut) =>
        RegisterProjectile(projectile, RollingAssets(projectile, nut));
    internal static void EnsureRollingProjectile(Projectile projectile, bool nut)
    {
        var assets = RollingAssets(projectile, nut);
        // A valid generic pea/chomper view is insufficient. The saved chunk
        // chooses exactly one original prefab, including after pool reuse/load.
        RegisterProjectile(projectile, assets);
    }
    internal static void RegisterExplodeProjectile(Projectile projectile) => RegisterProjectile(projectile, ExplodeProjectileAssets);
    internal static void RegisterSolarProjectile(Projectile projectile) => RegisterProjectile(projectile, SolarProjectileAssets);
    internal static void RegisterCherryProjectile(Projectile projectile) => RegisterProjectile(projectile, CherryProjectileAssets);
    internal static void EnsureCherrySpreaderProjectile(Projectile projectile)
    {
        RegisterProjectile(projectile, CherrySpreader.IsAttached(projectile)
            ? CherrySpreaderBombProjectileAssets : CherrySpreaderProjectileAssets);
    }
    internal static void EnsureCherryProjectile(Projectile projectile)
    {
        if (CherryShooter.IsExplosive(projectile)) RegisterExplodeProjectile(projectile);
        else RegisterCherryProjectile(projectile);
    }

    internal static void EnsureRetaliationProjectile(Projectile projectile)
    {
        if (projectile.mDead || projectile.mBoard == null || StarProjectiles.Family(projectile) != 0) return;
        var assets = RetaliationProjectiles.IsPea(projectile) ? PlainPeaProjectileAssets
            : RetaliationProjectiles.IsStar(projectile) ? PlainStarProjectileAssets : null;
        if (assets == null) return;
        RegisterProjectile(projectile, assets);
    }

    internal static void EnsureAirLobProjectile(Projectile projectile)
    {
        if (projectile.mDead || !AirLobbers.IsShot(projectile)) return;
        RegisterProjectile(projectile, projectile.mProjectileType == ProjectileType.Cabbage ? CabbageProjectileAssets
            : projectile.mProjectileType == ProjectileType.Butter ? ButterProjectileAssets : KernelProjectileAssets);
    }

    internal static bool HasProjectileArt(Projectile projectile) => ImportedProjectiles.TryGetValue(projectile.Pointer, out var view) && view.IsValid;

    private static void RegisterProjectile(Projectile projectile, Assets assets)
    {
        if (ImportedProjectiles.TryGetValue(projectile.Pointer, out var current) && current.IsValid && current.Uses(assets)) return;
        ForgetProjectile(projectile);
        ImportedProjectiles[projectile.Pointer] = new ProjectileView(projectile, assets);
        PlantLibrary.Projectiles.NotifyBorn(projectile);
    }
    internal static void ForgetProjectile(Projectile projectile)
    { if (ImportedProjectiles.Remove(projectile.Pointer, out var view)) view.Dispose(); }

    private static void UpdateImportedProjectiles()
    {
        foreach (var (key, view) in ImportedProjectiles.ToArray())
        {
            try { if (view.IsValid) { view.Update(); continue; } }
            catch (Exception error) { FusionMod.Instance.LoggerInstance.Error($"Imported projectile art failed: {error}"); }
            view.Dispose(); ImportedProjectiles.Remove(key);
        }
    }

    private sealed class ProjectileView : IDisposable
    {
        private readonly Projectile projectile;
        private readonly Assets assets;
        private readonly uint id;
        internal readonly IntPtr Board;
        private GameObject? root;
        private Transform[] transforms = Array.Empty<Transform>();
        private readonly List<(Renderer Renderer, bool Enabled)> native = new();
        private SortingGroup? sorting;
        private Renderer? anchor;

        internal ProjectileView(Projectile projectile, Assets assets)
        { this.assets = assets; this.projectile = projectile; id = projectile.DataID; Board = projectile.mBoard.Pointer; }
        internal bool Uses(Assets expected) => ReferenceEquals(assets, expected);
        internal bool IsValid => !projectile.mDead && projectile.DataID == id
            && projectile.mBoard?.Pointer == Board && (assets == PhoenixProjectileAssets ? PhoenixProjectiles.Contains(projectile)
                : assets == GlacierProjectileAssets ? GlacierShroom.Contains(projectile)
                : assets == SolarCabbageProjectileAssets ? LobberProjectiles.Family(projectile) == 15
                : assets == DoomPeaProjectileAssets ? DoomProjectiles.Family(projectile) == 1
                : assets == DoomProjectileAssets ? DoomProjectiles.Family(projectile) == 2
                : assets == BigDoomProjectileAssets ? DoomProjectiles.Family(projectile) == 3
                : assets == DoomMeteorProjectileAssets ? DoomProjectiles.Family(projectile) == 4
                : assets == PotatoSeedProjectileAssets ? PotatoSeedProjectiles.Contains(projectile)
                : assets == DoomCactusProjectileAssets ? CactusProjectiles.Family(projectile) == 3
                : assets == LightPierceProjectileAssets ? CactusProjectiles.Family(projectile) == 4
                : assets == LightSpikeProjectileAssets ? CactusProjectiles.Family(projectile) == 6
                : assets == CactusSpikeProjectileAssets ? CactusProjectiles.Family(projectile) is 7 or 8
                : assets == CactusStarProjectileAssets ? CactusProjectiles.Family(projectile) == 5
                : assets == IceCactusProjectileAssets ? CactusProjectiles.Family(projectile) == 1
                : assets == MagnetCactusProjectileAssets ? CactusProjectiles.Family(projectile) == 2
                : assets == GarlicStarProjectileAssets ? StarProjectiles.Family(projectile) == 1
                : assets == IceStarProjectileAssets ? StarProjectiles.Family(projectile) == 2
                : assets == FireStarProjectileAssets ? StarProjectiles.Family(projectile) is 3 or 9
                : assets == IronStarProjectileAssets ? StarProjectiles.Family(projectile) == 8
                : assets == IronMelonProjectileAssets ? LobberProjectiles.Family(projectile) == 12
                : assets == IronKernelProjectileAssets ? LobberProjectiles.Family(projectile) == 16
                : assets == IronButterProjectileAssets ? LobberProjectiles.Family(projectile) == 17
                : assets == MagnetKernelProjectileAssets ? LobberProjectiles.Family(projectile) == 10
                : assets == MagnetButterProjectileAssets ? LobberProjectiles.Family(projectile) == 11
                : assets == MetalStarProjectileAssets ? StarProjectiles.Family(projectile) == 7
                : assets == MetalMelonProjectileAssets ? LobberProjectiles.Family(projectile) == 8
                : assets == StarPeaProjectileAssets ? StarProjectiles.Family(projectile) == 4
                : assets == PolarProjectileAssets ? PolarProjectiles.IsPolar(projectile) : assets == BloverSunProjectileAssets ? SunflowerFusions.IsShot(projectile)
                : assets == LettuceProjectileAssets ? SquashLobbers.Family(projectile) == 1
                : assets == LittleLettuceProjectileAssets ? SquashLobbers.Family(projectile) == 2
                : assets == ButternutProjectileAssets ? SquashLobbers.Family(projectile) is 3 or 8
                : assets == ButterSquashProjectileAssets ? SquashLobbers.Family(projectile) is 4 or 9
                : assets == GourdProjectileAssets ? SquashLobbers.Family(projectile) is 5 or 6
                : assets == SmallCabbageProjectileAssets ? PuffLobbers.Family(projectile) == 1
                : assets == SmallIronProjectileAssets ? IronProjectiles.IsSmall(projectile)
                : assets == SmallKernelProjectileAssets ? PuffLobbers.Family(projectile) == 2
                : assets == SmallButterProjectileAssets ? PuffLobbers.Family(projectile) == 3
                : assets == SmallMelonProjectileAssets ? PuffLobbers.Family(projectile) == 4
                : assets == SilverCabbageProjectileAssets ? SilverLobbers.Family(projectile) == 1
                : assets == SilverMelonProjectileAssets ? SilverLobbers.Family(projectile) == 2
                : assets == SilverKernelProjectileAssets ? SilverLobbers.Family(projectile) == 3
                : assets == SilverButterProjectileAssets ? SilverLobbers.Family(projectile) == 4
                : assets == TacoProjectileAssets ? LobberProjectiles.Family(projectile) == 1
                : assets == ButterTacoProjectileAssets ? LobberProjectiles.Family(projectile) == 2
                : assets == CobProjectileAssets ? LobberProjectiles.Family(projectile) == 3
                : assets == ButterCobProjectileAssets ? LobberProjectiles.Family(projectile) == 4
                : assets == OverripeProjectileAssets ? LobberProjectiles.Family(projectile) == 5
                : assets == TriCabbageProjectileAssets ? LobberProjectiles.Family(projectile) == 6 && LobberProjectiles.Remaining(projectile) > 0
                : assets == CabbageProjectileAssets ? NutProjectiles.Family(projectile) == 1 || LobberProjectiles.Family(projectile) == 6 && LobberProjectiles.Remaining(projectile) == 0
                : assets == StarCapProjectileAssets ? StarCaps.Contains(projectile)
                : assets == DoomCabbageProjectileAssets ? LobberProjectiles.Family(projectile) == 13
                : assets == ColdslawProjectileAssets ? ElementalLobbers.Family(projectile) == 1
                : assets == KimchiProjectileAssets ? ElementalLobbers.Family(projectile) is 2 or 3
                : assets == IceKernelProjectileAssets ? ElementalLobbers.Family(projectile) == 4
                : assets == IceButterProjectileAssets ? ElementalLobbers.Family(projectile) == 5
                : assets == GarlicCabbageProjectileAssets ? ElementalLobbers.Family(projectile) == 6
                : assets == GarlicKernelProjectileAssets ? ElementalLobbers.Family(projectile) == 7
                : assets == GarlicButterProjectileAssets ? ElementalLobbers.Family(projectile) == 8
                : assets == GarlicMelonProjectileAssets ? ElementalLobbers.Family(projectile) == 9
                : assets == HypnoMelonProjectileAssets ? ElementalLobbers.Family(projectile) == 11
                : assets == IceTrackProjectileAssets ? CattailFusions.Family(projectile) == 1
                : assets == FireTrackProjectileAssets ? CattailFusions.Family(projectile) == 2
                : assets == FireMelonProjectileAssets ? ElementalLobbers.Family(projectile) == 10
                : assets == SaladProjectileAssets ? LobberProjectiles.Family(projectile) == 7
                : assets == KernelProjectileAssets ? TriLobbers.Family(projectile) == 1
                : assets == ButterProjectileAssets ? TriLobbers.Family(projectile) == 2 || LobberProjectiles.Family(projectile) == 14
                : assets == MelonProjectileAssets ? UmbrellaVariants.ReflectedMelon(projectile) || LobberProjectiles.Family(projectile) == 9 || SquashLobbers.Family(projectile) == 7 || NutProjectiles.Family(projectile) == 2 || TriLobbers.Family(projectile) == 3
                : assets == PuffPeaProjectileAssets ? PuffShooters.Contains(projectile) : assets == SpeedyProjectileAssets ? ScaredyProjectiles.Family(projectile) == 1 && projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
                : assets == CherrySporeAssets ? ScaredyProjectiles.Family(projectile) == 2
                : assets == FrenzyProjectileAssets ? ScaredyProjectiles.Family(projectile) == 5
                : assets == IcicleProjectileAssets ? PuffSpecials.IsIcicle(projectile)
                : assets == PotatoSporeAssets ? ScaredyProjectiles.Family(projectile) == 4 || PuffSpecials.IsNugget(projectile)
                : assets == ShiveryProjectileAssets ? ScaredyProjectiles.Family(projectile) == 3 && projectile.mProjectileType == ProjectileType.Snowpea
                : assets == ThawedShiveryProjectileAssets ? ScaredyProjectiles.Family(projectile) == 3 && projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea
                : assets == GutsyProjectileAssets ? GutsyShroom.IsShot(projectile) || ScaredyProjectiles.Family(projectile) == 6 : assets == PlainPeaProjectileAssets ? UmbrellaVariants.ReflectedPea(projectile) || RetaliationProjectiles.IsPea(projectile) || (ThreepeaterFusions.IsScorchedShot(projectile)
                && projectile.mProjectileType is ProjectileType.Pea or ProjectileType.PeashooterPea) : assets == PlainStarProjectileAssets ? RetaliationProjectiles.IsStar(projectile) || StarProjectiles.Family(projectile) == 5 : assets == HypnoFireProjectileAssets ? HypnoShooters.IsFire(projectile)
                : assets == HypnoProjectileAssets ? HypnoShooters.Contains(projectile) && !HypnoShooters.IsFire(projectile)
                : assets == PotatoProjectileAssets ? PotatoProjectiles.Contains(projectile)
                : assets == CherryDroneProjectileAssets ? ChargeTorchFusions.IsFireCherry(projectile) && projectile.mMotionType == ProjectileMotion.Puff
                : assets == FireCherryProjectileAssets ? ChargeTorchFusions.IsFireCherry(projectile) && projectile.mMotionType != ProjectileMotion.Puff
                : assets == SmallFireProjectileAssets ? projectile.mCobTargetX == ChargeTorchFusions.SmallFire
                : assets == PotatoFireProjectileAssets ? projectile.mCobTargetX == ChargeTorchFusions.PotatoFire
                : assets == ScorchProjectileAssets ? projectile.mCobTargetX == SupportTorchFusions.Scorch
                : assets == StenchFireProjectileAssets ? projectile.mCobTargetX == SupportTorchFusions.Stench
                : assets == PoisonProjectileAssets ? PoisonShooters.Contains(projectile) : assets == BlazeProjectileAssets ? BlazeShooters.Contains(projectile) : assets == HelmetProjectileAssets ? IronProjectiles.IsRed(projectile) && IronProjectiles.Contains(projectile) : assets == IronProjectileAssets ? !IronProjectiles.IsRed(projectile) && IronProjectiles.Contains(projectile) : assets == SolarNutBallAssets ? RollingProjectiles.TryShot(projectile, out var solar) && solar.SolarBall
                : assets == WeedProjectileAssets ? RollingProjectiles.IsWeed(projectile)
                : assets == FireNutProjectileAssets ? projectile.mCobTargetX == RollingProjectiles.IgnitedNut
                : assets == NutProjectileAssets ? NutProjectiles.Family(projectile) == 3 || RollingProjectiles.TryShot(projectile, out var nut) && nut.Nut && !nut.SolarBall
                : assets == ChompProjectileAssets ? RollingProjectiles.Chunk(projectile) == 0 && RollingProjectiles.TryShot(projectile, out var chomp) && !chomp.Nut && !chomp.Weed : assets == SolarProjectileAssets ? SolarShooter.TryShot(projectile, out _)
                : assets == SeaSporeProjectileAssets ? SeaProjectiles.Family(projectile) is 1 or 2
                : assets == SeaSpikeProjectileAssets ? SeaProjectiles.Family(projectile) == 3
                : assets == SeaStarProjectileAssets ? StarProjectiles.Family(projectile) == 6
                : assets == ZombieBlockAssets ? RollingProjectiles.Chunk(projectile) == 1
                : assets == ZombieBlock2Assets ? RollingProjectiles.Chunk(projectile) == 2
                : assets == ZombieBlock3Assets ? RollingProjectiles.Chunk(projectile) == 3
                : assets == WallopProjectileAssets ? FumeProjectiles.IsNut(projectile)
                : assets == MiniSquashProjectileAssets ? MiniSquashProjectiles.Contains(projectile)
                : assets == KelpProjectileAssets ? KelpProjectiles.Contains(projectile)
                : assets == ExplodeProjectileAssets ? CherryShooter.IsExplosive(projectile)
                : assets == CherrySpreaderProjectileAssets ? CherrySpreader.IsFlying(projectile)
                : assets == CherrySpreaderBombProjectileAssets ? CherrySpreader.IsAttached(projectile)
                : CherryShooter.TryShot(projectile));

        private bool poseReady;

        internal void Update()
        {
            if (root != null && assets == PuffPeaProjectileAssets)
                root.transform.localScale = Vector3.one * (80f * 2.82f / 1.35f
                    * (projectile.mCobTargetX == PuffShooters.ReboundedShot ? .5f : .3f));
            var controller = projectile.mController;
            if (controller == null || !controller) return;
            if (root == null)
            {
                assets.Load(projectile.mBoard, null);
                var model = assets.model!;
                anchor = controller.GetComponentInChildren<Renderer>(true);
                if (anchor == null || !anchor) return;
                root = new GameObject($"Fusion-{assets.Name}") { layer = anchor.gameObject.layer };
                root.transform.SetParent(controller.transform, false);
                // Use each shooter's measured original ground reference.
                // Explode-o-shooter: .88 * 6.70833397 * .24 = 1.4168.
                root.transform.localScale = Vector3.one * (80f * 2.82f * (assets == PuffPeaProjectileAssets
                    ? projectile.mCobTargetX == PuffShooters.ReboundedShot ? .5f : .3f : assets == IcicleProjectileAssets || PuffSpecials.IsNugget(projectile) ? .3f : 1) / (assets == PuffPeaProjectileAssets || assets == IcicleProjectileAssets || PuffSpecials.IsNugget(projectile) || assets == GutsyProjectileAssets || assets == SpeedyProjectileAssets || assets == ThawedShiveryProjectileAssets || assets == CherrySporeAssets || assets == PotatoSporeAssets || assets == ShiveryProjectileAssets ? 1.35f : assets == StarCapProjectileAssets ? 1.35f : assets == PlainPeaProjectileAssets ? 1.566913f : assets == PlainStarProjectileAssets ? 1.50443746f : assets == SolarNutBallAssets ? 1.517076f
                    : assets == NutProjectileAssets || assets == FireNutProjectileAssets ? 1.50443746f
                    : assets == ChompProjectileAssets ? 1.53014407f : assets == SolarProjectileAssets ? .88f * 7.4329915f * .23f
                    : assets == PolarProjectileAssets ? 1.566913f : assets == BloverSunProjectileAssets ? 1.16160005f
                    : assets == CherrySpreaderProjectileAssets || assets == CherrySpreaderBombProjectileAssets ? 1.566913f
                    : assets == HypnoProjectileAssets ? .88f * 7.1613f * .23f
                    : assets == IronProjectileAssets ? .88f * 7f * .23f : 1.4168f));
                if (PuffLobbers.Family(projectile) != 0 || IronProjectiles.IsSmall(projectile))
                    root.transform.localScale = Vector3.one * (80f * 2.82f * .3f / 1.35f);
                if (assets == FrenzyProjectileAssets)
                    root.transform.localScale = Vector3.one * (80f * 2.82f / 1.35f);
                if (SquashLobbers.Family(projectile) != 0 || SilverLobbers.Family(projectile) != 0 || TriLobbers.Family(projectile) != 0 || LobberProjectiles.Family(projectile) != 0 || ElementalLobbers.Family(projectile) != 0 || NutProjectiles.Family(projectile) is 1 or 2)
                    root.transform.localScale = Vector3.one * (80f * 2.82f / 1.35f);
                if (ElementalLobbers.Family(projectile) == 3) root.transform.localScale *= .5f;
                if (assets == MiniSquashProjectileAssets || CactusProjectiles.Family(projectile) != 0 || SeaProjectiles.Family(projectile) != 0 || assets == SeaStarProjectileAssets || CattailFusions.Family(projectile) != 0 || (StarProjectiles.Family(projectile) is >= 1 and <= 4 or 7) || assets == WallopProjectileAssets || assets == KelpProjectileAssets)
                    root.transform.localScale = Vector3.one * (80f * 2.82f / 1.35f);
                if (assets == WeedProjectileAssets || assets == PhoenixProjectileAssets || StarCaps.Contains(projectile) || DoomProjectiles.Family(projectile) != 0 || assets == PotatoSeedProjectileAssets || assets == ZombieBlockAssets || assets == ZombieBlock2Assets || assets == ZombieBlock3Assets)
                    root.transform.localScale = Vector3.one * (80f * 2.82f / 1.35f);
                sorting = root.AddComponent<SortingGroup>();
                transforms = new Transform[model.Nodes.Length];
                for (int i = 0; i < model.Nodes.Length; ++i)
                {
                    var node = model.Nodes[i];
                    var go = new GameObject(node.Name) { layer = anchor.gameObject.layer };
                    transforms[i] = go.transform;
                    go.transform.SetParent(node.Parent < 0 ? root.transform : transforms[node.Parent], false);
                    if (node.Sprite < 0) continue;
                    var renderer = go.AddComponent<SpriteRenderer>();
                    renderer.sharedMaterial = assets.spriteMaterial;
                    renderer.sprite = assets.Sprites[node.Sprite];
                    renderer.sortingOrder = node.Order;
                    renderer.flipX = node.FlipX; renderer.flipY = node.FlipY;
                    renderer.color = new Color(node.Color[0], node.Color[1], node.Color[2], node.Color[3]);
                }
                Pose(model, transforms, FusionProjectileTime.AgeSeconds(projectile), null);
                if (ReportedProjectileBoards.Add((Board, assets.Name)))
                    FusionMod.Instance.LoggerInstance.Msg($"{assets.Name} original art ready: shader={assets.spriteMaterial!.shader.name}, sprites={model.Nodes.Count(n => n.Sprite >= 0)}, parent={controller.name}, scale={root.transform.localScale}.");
                // Hide native rendering only after original art has been built.
                foreach (var renderer in controller.GetComponentsInChildren<Renderer>(true))
                {
                    if (renderer.transform.IsChildOf(root.transform)) continue;
                    native.Add((renderer, renderer.enabled)); renderer.enabled = false;
                }
            }
            if (SquashLobbers.Family(projectile) != 0)
            {
                // A live view survives the flight/landing transition. Reapply
                // phase scale here as well as when reconstructing a saved view.
                float size = 80f * 2.82f / 1.35f;
                if (SquashLobbers.Family(projectile) is 1 or 2)
                    size *= MathF.Pow(.5f, SquashLobbers.Generation(projectile));
                root!.transform.localScale = Vector3.one * size;
                if (SquashLobbers.Family(projectile) is 6 or 8 or 9)
                    root.transform.localScale = Vector3.Scale(root.transform.localScale, new Vector3(1, .5f, 1));
            }
            bool animatePose = !OptionalSpeed.Reduced || !poseReady;
            if (animatePose && assets == DoomMeteorProjectileAssets)
                root!.transform.localRotation = Quaternion.Euler(0, 0, -FusionProjectileTime.AgeSeconds(projectile) * 180);
            sorting!.sortingLayerID = anchor!.sortingLayerID;
            sorting.sortingOrder = anchor.sortingOrder;
            // Native controller/animation updates can enable their renderers
            // again. The imported view owns rendering for its whole lifetime.
            foreach (var part in native)
                if (part.Renderer) part.Renderer.enabled = false;
            if (animatePose)
            {
                if (assets == SolarNutBallAssets && RollingProjectiles.TryShot(projectile, out var ballState))
                    Pose(assets.model!, transforms, ballState.AnimationTime, null,
                        baseClip: ballState.ChargeTicks > 0 ? "idle_big" : "Round");
                else if (assets == CherrySpreaderBombProjectileAssets && projectile.mClickBackoffCounter > 0)
                    Pose(assets.model!, transforms, (17 - projectile.mClickBackoffCounter) * .01f, null, baseClip: "charge");
                else if (assets == LittleLettuceProjectileAssets)
                    Pose(assets.model!, transforms, projectile.mHitTorchwoodGridX / 50000f,
                        Math.Min(projectile.mHitTorchwoodGridX / 100000f, 2.13333f));
                else Pose(assets.model!, transforms, FusionProjectileTime.AgeSeconds(projectile), null);
            }
            if (assets == SolarNutBallAssets && RollingProjectiles.TryShot(projectile, out var solarBall))
            {
                // Original shadow: .88 * 5.746500015 * .3 = 1.517076.
                root!.transform.localScale = Vector3.one * (80f * 2.82f / 1.517076f * solarBall.Scale / .3f);
                // Round translates its root by eight source units per loop.
                // Unity consumes that as root motion; the native projectile
                // owns movement here. Do not replay it as a local visual offset
                // (which jumps back every loop). Prefab scene position is not
                // a projectile offset either.
                transforms[0].localPosition = Vector3.zero;
                // The projectile controller is a pea's centre anchor. Pin the
                // original ground shadow to the native shadow position rather
                // than placing this much larger body at the pea's centre.
                var groundNode = assets.model!.GroundNode;
                var groundSprite = assets.Sprites[assets.model.Nodes[groundNode].Sprite];
                var ground = root.transform.InverseTransformPoint(
                    transforms[groundNode].TransformPoint(groundSprite.bounds.center));
                var offset = Vector3.Scale(ground, root.transform.localScale);
                root.transform.localPosition = new Vector3(-offset.x,
                    (projectile.mPosY - projectile.mShadowY) * 2.82f - offset.y, 0);
            }
            if (assets == WeedProjectileAssets || assets == FireNutProjectileAssets || ChargeTorchFusions.IsLowFire(projectile) || ChargeTorchFusions.IsFireCherry(projectile) || assets == MiniSquashProjectileAssets || DoomProjectiles.Family(projectile) != 0 || CactusProjectiles.Family(projectile) != 0 || assets == ZombieBlockAssets || assets == ZombieBlock2Assets || assets == ZombieBlock3Assets || SeaProjectiles.Family(projectile) != 0 || CattailFusions.Family(projectile) != 0 || StarProjectiles.Family(projectile) != 0 || assets == BloverSunProjectileAssets || assets == HypnoProjectileAssets || PuffLobbers.Family(projectile) != 0 || SilverLobbers.Family(projectile) != 0 || TriLobbers.Family(projectile) != 0 || SquashLobbers.Family(projectile) != 0 || LobberProjectiles.Family(projectile) != 0 || ElementalLobbers.Family(projectile) >= 6 || IronProjectiles.IsSmall(projectile))
                // The prefab's +0.4185 X is its scene position, not a muzzle
                // offset. Native simulation owns the projectile's world anchor.
                transforms[0].localPosition = Vector3.zero;
            if (animatePose)
            {
                if (CattailFusions.Family(projectile) != 0)
                    for (int i = 0; i < transforms.Length; ++i)
                        if (assets.model!.Nodes[i].Sprite >= 0 && assets.model.Nodes[i].Name != "Shadow")
                            transforms[i].localRotation *= Quaternion.Euler(0, 0,
                                MathF.Atan2(-projectile.mVelY, projectile.mVelX) * 180 / MathF.PI);
                if (assets == IceStarProjectileAssets)
                    transforms[0].localRotation *= Quaternion.Euler(0, 0,
                        projectile.mHitTorchwoodGridX + FusionProjectileTime.AgeSeconds(projectile) * 360 + 90);
                if (assets == IronStarProjectileAssets || assets == MetalStarProjectileAssets || assets == PlainStarProjectileAssets || assets == GarlicStarProjectileAssets || assets == FireStarProjectileAssets || assets == IceStarProjectileAssets)
                {
                    // Bullet_star.Update rotates only its visible child by
                    // min(source speed, 12) * 30 degrees/s; the shadow stays level.
                    float speed = assets == IceStarProjectileAssets ? 10 : MathF.Sqrt(projectile.mVelX * projectile.mVelX + projectile.mVelY * projectile.mVelY) * 1.25f;
                    for (int i = 0; i < transforms.Length; ++i)
                        if (assets.model!.Nodes[i].Name is "ProjectileStar" or "zidan")
                            transforms[i].localRotation *= Quaternion.Euler(0, 0,
                                Math.Min(speed, 12) * 30 * FusionProjectileTime.AgeSeconds(projectile));
                }
                if (assets == SmallCabbageProjectileAssets)
                    // Bullet_sunCabbage.FixedUpdate rotates its sprite at -500
                    // degrees/second. The shadow and native motion stay separate.
                    for (int i = 0; i < transforms.Length; ++i)
                        if (assets.model!.Nodes[i].Name == "Sprite")
                            transforms[i].localRotation *= Quaternion.Euler(0, 0,
                                -500 * FusionProjectileTime.AgeSeconds(projectile));
                if (assets == DoomCabbageProjectileAssets || assets == SilverCabbageProjectileAssets || assets == TriCabbageProjectileAssets || assets == CabbageProjectileAssets
                    || assets == ColdslawProjectileAssets || assets == KimchiProjectileAssets || assets == GarlicCabbageProjectileAssets)
                    // Inherited Bullet_cabbage.RotateUpdate: 300 degrees/s,
                    // clockwise for rightward throws; the shadow stays level.
                    for (int i = 0; i < transforms.Length; ++i)
                        if (assets.model!.Nodes[i].Name == "Sprite")
                            transforms[i].localRotation *= Quaternion.Euler(0, 0,
                                (projectile.mVelX > 0 ? -300 : 300) * FusionProjectileTime.AgeSeconds(projectile));
                if (assets == FireNutProjectileAssets || assets == NutProjectileAssets || assets == ChompProjectileAssets || assets == ZombieBlockAssets || assets == ZombieBlock2Assets || assets == ZombieBlock3Assets)
                    foreach (var transform in transforms.Where((_, i) => assets.model!.Nodes[i].Sprite >= 0
                        && assets.model.Nodes[i].Name != "Shadow"))
                        transform.localRotation *= Quaternion.Euler(0, 0, -projectile.mRotation * Mathf.Rad2Deg);
            }
            poseReady = true;
            if (assets == MiniSquashProjectileAssets || assets == ZombieBlockAssets || assets == ZombieBlock2Assets || assets == ZombieBlock3Assets || assets == KelpProjectileAssets || assets == NutProjectileAssets || NutProjectiles.Family(projectile) is 1 or 2 || PuffLobbers.Family(projectile) != 0 || SilverLobbers.Family(projectile) != 0 || TriLobbers.Family(projectile) != 0 || SquashLobbers.Family(projectile) != 0 || LobberProjectiles.Family(projectile) != 0 || ElementalLobbers.Family(projectile) >= 6 || IronProjectiles.IsSmall(projectile))
                for (int i = 0; i < transforms.Length; ++i)
                {
                    if (assets.model!.Nodes[i].Name != "Shadow") continue;
                    // The native controller follows the airborne body. Keep
                    // its imported shadow on the lane throughout the fall.
                    float scale = root!.transform.localScale.y;
                    for (var parent = transforms[i].parent; parent != root.transform; parent = parent.parent)
                        scale *= parent.localScale.y;
                    var position = transforms[i].localPosition;
                    position.y = (projectile.mPosZ - 10) * 2.82f / scale;
                    transforms[i].localPosition = position;
                }
        }
        public void Dispose()
        {
            foreach (var part in native) if (part.Renderer) part.Renderer.enabled = part.Enabled;
            native.Clear();
            if (root != null && root) Object.Destroy(root);
            root = null;
        }
    }
}
