using LawnGlove;
using MelonLoader;

[assembly: MelonInfo(typeof(FusionMod.FusionMod), "Plant Fusion", "22.1.0", "Kevin / Codex")]
[assembly: MelonGame("PopCap Games", "PvZ Replanted")]
[assembly: MelonOptionalDependencies("DebugMod")]
[assembly: MelonAdditionalDependencies("LawnGlove", "PlantLibrary", "ZombieLibrary", "SpeedMod")]

namespace FusionMod;

public sealed class FusionMod : MelonMod
{
    internal static FusionMod Instance = null!;
    private static readonly Il2CppSystem.Action BeforeQuit = (Il2CppSystem.Action)(Action)DetachCameraTargets;

    public override void OnInitializeMelon()
    {
        Instance = this;
        MenuPresentation.Register(this);
        FusionAlmanac.RegisterLibrary();
        FusionZombies.Register();
        FusionAudio.RegisterLibrary();
        // Application.quitting runs before individual OnApplicationQuit
        // callbacks release native renderer assets. Melon's callback is later.
        UnityEngine.Application.quitting += BeforeQuit;
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

    public override void OnApplicationQuit() => DetachCameraTargets();

    private static void DetachCameraTargets()
    {
        // Native preview cameras outlive their render targets during shutdown.
        // Detach borrowed textures before Unity releases them; never release
        // textures owned by the native pipeline ourselves.
        foreach (var camera in UnityEngine.Object.FindObjectsOfType<UnityEngine.Camera>(true))
            if (camera != null && camera.targetTexture != null) camera.targetTexture = null;
    }

    internal static void DetachCameraTarget(UnityEngine.RenderTexture texture)
    {
        foreach (var camera in UnityEngine.Object.FindObjectsOfType<UnityEngine.Camera>(true))
            if (camera != null && camera.targetTexture?.Pointer == texture.Pointer) camera.targetTexture = null;
    }

    public override void OnDeinitializeMelon()
    {
        UnityEngine.Application.quitting -= BeforeQuit;
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
