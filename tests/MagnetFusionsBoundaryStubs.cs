// Native equipment removal, visuals and integration owners are isolated here.
// FusionTime and the complete MagnetFusions source execute in the tests.
namespace Il2CppReloaded.Gameplay {
    internal enum PlantState { Ready, MagnetshroomSucking }
    internal enum HelmType { None, Pail, Football, Bobsled }
    internal enum ShieldType { None, Door, Ladder, Garbage }
    internal enum MagnetItemType { None, Pail1, FootballHelmet1, LadderPlaced }
    internal enum ZombieType { Normal, Pail, Football, Pogo, JackInTheBox, Digger, Bobsled, Boss, Bungee, Snorkel }
    [Flags] internal enum DamageFlags { None, BypassesShield = 1 }
    internal enum ParticleEffect { Powie }
    internal sealed class MagnetItem {
        public MagnetItemType ItemType; public float PosX, PosY, DestOffsetX, DestOffsetY;
    }
    internal sealed partial class Board {
        public readonly List<Zombie> m_zombies = new();
        internal readonly List<(float X, float Y, int Damage)> Blasts = new();
        internal int Winds, Effects;
    }
    internal partial class Plant {
        public FusionMod.FusionPlant Identity;
        public bool mDead, mIsAsleep;
        public int mPlantHealth = 300, mPlantMaxHealth = 300, mDoSpecialCountdown, mStateCountdown,
            mShootingCounter, mLaunchCounter, mLaunchRate, mTargetX, mTargetY, mStartRow,
            mX = 100, mY = 100, mWidth = 80, mHeight = 80, mRow = 2, mPlantCol, mRenderOrder, Level;
        public PlantState mState;
        public MagnetItem[] mMagnetItems = { new() };
        public Action? OnCapture;
        public void PlantInitialize() {} public void UpdateMagnetShroom() {} public void UpdateAbilities() {} public void UpdateShooting() {}
        public void Die() { mDead = true; }
        public void MagnetShroomAttactItem(Zombie zombie) {
            if (zombie.mHelmType is HelmType.Pail or HelmType.Football) {
                mMagnetItems[0].ItemType = zombie.mHelmType == HelmType.Pail ? MagnetItemType.Pail1 : MagnetItemType.FootballHelmet1;
                zombie.mHelmType = HelmType.None; zombie.mHelmHealth = 0;
            } else if (zombie.mShieldHealth > 0) {
                mMagnetItems[0].ItemType = MagnetItemType.LadderPlaced;
                zombie.mShieldHealth = 0; zombie.mShieldType = ShieldType.None;
            }
            mMagnetItems[0].PosX = zombie.TargetX; mMagnetItems[0].PosY = zombie.TargetY;
            mMagnetItems[0].DestOffsetX = 40; mMagnetItems[0].DestOffsetY = 10;
            OnCapture?.Invoke();
        }
    }
    internal sealed partial class Zombie {
        public ZombieType mZombieType; public HelmType mHelmType; public ShieldType mShieldType;
        public int mHelmHealth, mHelmMaxHealth, mShieldHealth, mGraveY, BodyHealth = 1000;
        internal bool AddedBucket;
        public bool mHasHead = true, mHasObject, mMindControlled, mInPool, mIsEating;
        public Action? OnDamage;
        public bool IsDeadOrDying() => mDead || BodyHealth <= 0;
        public void TakeDamage(int damage, DamageFlags flags) {
            if ((flags & DamageFlags.BypassesShield) == 0 && mShieldHealth > 0) {
                int absorbed = Math.Min(damage, mShieldHealth); mShieldHealth -= absorbed; damage -= absorbed;
            }
            // Isolating damage adapter: the real added-Bobsled owner reads
            // mGraveY first, then native damage reads the restored sled slot.
            if (mZombieType == ZombieType.Bobsled && AddedBucket && mGraveY > 0) {
                int absorbed = Math.Min(damage, mGraveY); mGraveY -= absorbed; damage -= absorbed;
            }
            if (mHelmHealth > 0) { int absorbed = Math.Min(damage, mHelmHealth); mHelmHealth -= absorbed; damage -= absorbed; }
            BodyHealth -= damage; OnDamage?.Invoke();
        }
    }
    internal sealed partial class GridItem { public IntPtr Pointer = (IntPtr)77; }
    internal static class ParticleBoundary {
        public static void AddTodParticle(this object app, float x, float y, int order, ParticleEffect effect) {}
    }
}
namespace FusionMod {
    using Il2CppReloaded.Gameplay;
    internal readonly record struct FusionPlant(int Id) {
        internal static readonly FusionPlant MagnetBlover = new(1), LumosMagnet = new(2), AnchorShroom = new(3), GrenadierMagnet = new(4);
        internal static FusionPlant From(Plant plant) => plant.Identity;
        internal static bool IsMagnetFusion(FusionPlant identity) => identity.Id is 1 or 2 or 3 or 4;
    }
    internal static class CherryBombPlants {
        internal sealed record Death(Board Board, FusionPlant Identity, uint Id, int Col, int Row, float X, float Y, int Order);
        internal static void Explode(Death impact, float sourceRadius, int damage) => impact.Board.Blasts.Add((impact.X, impact.Y, damage));
    }
    internal static class ImportedPeaVisuals { internal static void ShowMagnetLight(Plant plant, float x, float y) {} }
    internal static class BucketPlants { internal static bool Held; internal static int Clears; internal static bool Holding(Plant plant) => Held; internal static void Clear() { Held = false; ++Clears; } }
    internal static class PumpkinFusions { internal static int LightLevel(Plant plant) => plant.Level; }
    internal static class ChomperFusions {
        internal static bool Active(Plant plant) => plant.mBoard.Playing && !plant.mBoard.mPaused && !plant.mDead;
    }
    internal static class BloverFusions {
        internal static bool Live(Board board, Zombie zombie) => zombie.mBoard == board && !zombie.IsDeadOrDying();
        internal static void Blow(Plant plant) { ++plant.mBoard.Winds; }
    }
    internal static class BucketDrops {
        internal static bool TakeForMagnet(Plant plant, float radius, int rows, out int row) { row = 0; return false; }
        internal sealed record MagnetCapture(Board Board, uint Id);
        internal static MagnetCapture? CaptureMagnet(Plant plant) => null;
        internal static void ReleaseFromMagnet(Plant plant, MagnetCapture capture) {}
    }
}

namespace ZombieLibrary {
    internal static class NativeArmorTypes { internal const int BucketHealth = 1100; }
    internal static class NativeArmoredZombies {
        internal static bool TryKind(Il2CppReloaded.Gameplay.Zombie zombie, out bool bucket) {
            bucket = zombie.AddedBucket; return zombie.AddedBucket;
        }
    }
}
