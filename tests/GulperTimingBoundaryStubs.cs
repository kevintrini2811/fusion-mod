// Only absent native boundaries for the existing real-clock Chomper suite.
namespace UnityEngine { public struct Rect { public float x, width; } }
namespace Il2CppReloaded.Services
{
    public enum FoleyType { Chomp }
    public sealed class SoundSystem { public void PlayFoley(FoleyType sound) { } }
}
namespace Il2CppReloaded.Gameplay
{
    public sealed partial class App { public Il2CppReloaded.Services.SoundSystem SoundSystem = new(); }
    public sealed partial class Board { public bool mOnlyBurnedZombies = true; }
    public sealed partial class Plant
    {
        public int mShootingCounter, mWidth = 80;
        public bool mIsAsleep;
        public UnityEngine.Rect GetPlantAttackRect(PlantWeapon weapon) => new();
    }
    public enum ZombieType { Normal, Gargantuar, RedeyeGargantuar }
    public sealed partial class Zombie
    {
        public ZombieType mZombieType;
        public Action? OnDie;
        public bool VetoDeath;
        public void DieNoLoot() { if (!VetoDeath) mDead = true; OnDie?.Invoke(); }
    }
}
namespace FusionMod
{
    public readonly partial record struct FusionPlant
    { internal static readonly FusionPlant GulperShroom = new(6); }
    internal static class SquashFusions
    {
        internal static bool Eligible(Il2CppReloaded.Gameplay.Board board, Il2CppReloaded.Gameplay.Zombie zombie,
            Il2CppReloaded.Gameplay.DamageRangeFlags flags) => zombie.mBoard?.Pointer == board.Pointer
                && !zombie.mDead && !zombie.IsDeadOrDying() && !zombie.mMindControlled;
    }
}
