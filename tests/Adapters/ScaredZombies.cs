// Library boundary double. The real registry has its own independent suite.
namespace FusionMod
{
    internal static class PuffSpreading { internal static void Create(Il2CppReloaded.Gameplay.Board board, FusionService.Plan plan, System.Collections.Generic.List<Il2CppReloaded.Gameplay.Plant> created) {} }
    internal static class FusionZombies { internal static readonly object Eyes = new(), EyesCone = new(), EyesBucket = new(); }
}
namespace ZombieLibrary
{
    internal static class Zombies
    {
        internal static bool IsExplicitSpawn => false;
        internal static void Apply(object definition, Il2CppReloaded.Gameplay.Zombie zombie)
        {
            zombie.mGraveX = FusionMod.ScaredContent.ZombieMarker;
            zombie.mGraveY = 0;
            zombie.mBodyHealth = zombie.mBodyMaxHealth = 270;
        }
        internal static void PlaySound(string id, Il2CppReloaded.Gameplay.Zombie zombie) =>
            zombie.mBoard.mApp.SoundSystem.PlayFoley(Il2CppReloaded.Services.FoleyType.NewspaperRarrgh);
    }
}
