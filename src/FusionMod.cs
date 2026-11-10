using LawnGlove;
using MelonLoader;

[assembly: MelonInfo(typeof(FusionMod.FusionMod), "Plant Fusion", "23.3.2", "Kevin / Codex")]
[assembly: MelonGame("PopCap Games", "PvZ Replanted")]
[assembly: MelonOptionalDependencies("DebugMod", "SpeedMod", "DifficultyMod")]
[assembly: MelonAdditionalDependencies("LawnGlove", "PlantLibrary", "ZombieLibrary")]

namespace FusionMod;

public sealed class FusionMod : MelonMod
{
    internal static FusionMod Instance = null!;

    public override void OnInitializeMelon()
    {
        Instance = this;
        MenuPresentation.Register(this);
        FusionAlmanac.RegisterLibrary();
        FusionZombies.Register();
        FusionAudio.RegisterLibrary();
        OccupiedTilePlacement.CanPlace += FusionService.CanPlaceCarried;
        OccupiedTilePlacement.TryPlace += FusionService.TryPlaceCarried;
        LoggerInstance.Msg($"Plant Fusion ready: {FusionRecipes.Count} recipe(s); seed planting and Lawn Glove supported.");
    }

    public override void OnLateUpdate()
    {
        GoldenBean.UpdateUi();
        BucketPlants.UpdateCursor();
        BucketDrops.UpdateVisuals();
        ImportedPeaVisuals.UpdateFumeSmoke();
        ImportedPeaVisuals.Update();
    }

    public override void OnDeinitializeMelon()
    {
        MenuPresentation.Dispose();
        DebugBankPreviews.Clear();
        FusionZombies.Dispose();
        OccupiedTilePlacement.CanPlace -= FusionService.CanPlaceCarried;
        OccupiedTilePlacement.TryPlace -= FusionService.TryPlaceCarried;
        BucketPlants.Clear();
        BucketDrops.Clear();
        ImportedPeaVisuals.ClearEyesZombies();
        ImportedPeaVisuals.ClearHypnoHeads();
        ImportedPeaVisuals.ClearFumeSmoke();
        IronProjectiles.Clear();
        Fertilizer.Clear();
        GoldenBean.Clear();
        ImportedPeaVisuals.Dispose();
        SolarShooter.Clear();
        TwinMarigold.Clear();
        Overheat.Clear();
        IceMineBlasts.Clear();
        BlazeShooters.Clear();
        PoisonShooters.Clear();
        RollingProjectiles.Clear();
    }
}
