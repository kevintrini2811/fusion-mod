namespace HarmonyLib {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal sealed class HarmonyPatch : Attribute { public HarmonyPatch() {} public HarmonyPatch(Type type, string name) {} }
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class HarmonyPriority : Attribute { public HarmonyPriority(int value) {} }
    internal static class AccessTools { internal static System.Reflection.MethodInfo Method(Type type, string name) => type.GetMethod(name)!; }
    internal static class Priority { internal const int First = 800, Last = 0; }
}
namespace Il2CppReloaded.Constants {
    internal static class Gameplay { internal static double TIME_FOR_UPDATE = .01; }
}
namespace Il2CppReloaded.Gameplay {
    internal sealed class Board {
        private static int next;
        public IntPtr Pointer = (IntPtr)(++next);
        public bool Playing = true, mPaused;
        public int mMainCounter;
        public Il2CppReloaded.Services.TestApp mApp = new();
        public List<Plant> m_plants = new(); public List<Zombie> m_zombies = new();
        public int mPlantsEaten; public bool Pool;
        public int PixelToGridX(float x,float y) => (int)(x / 80);
        public int GridToPixelX(int x,int y) => x*80;
        public float GetPosYBasedOnRow(float x,int row) => row*100 + x*.1f;
        public bool IsPoolSquare(int col,int row) => Pool;
        public Plant? GetTopPlantAt(int theGridX,int theGridY,PlantPriority thePriority) => null;
        public bool CanPlantAt() => true; public void KillAllPlantsInRadius() {}
        public GridItems m_gridItems = new();
        public Zombie? Target;
        public int PixelToGridY(int x, int y) => y / 100;
        public Zombie? ZombieTryToGet(uint id) => Target?.DataID == id ? Target : null;
        public void Serialize() {} public void UpdateGame() {} public void InitLevel() {}
        public void InitSurvivalStage() {} public void DisposeBoard() {}
    }
    internal enum GridItemType { ZenTool, Crater }
    internal enum GridItemState { Normal, Other }
    internal sealed class GridItems : List<GridItem> {
        internal bool FailAllocation;
        public GridItem? DataArrayAlloc() {
            if (FailAllocation) return null;
            var item = new GridItem(); Add(item); return item;
        }
    }
    internal sealed class GridItem {
        private static uint next;
        public uint DataID = ++next;
        public bool mDead;
        public GridItemType mGridItemType;
        public GridItemState mGridItemState;
        public int mGridX, mGridY, mGridItemCounter, mSunCount, mX, mY;
        public float mPosX, mPosY, mGoalX, mGoalY;
        public void Init(object app, GridItemType type) { mGridItemType = type; }
        public void GridItemDie() { mDead = true; }
        public void Update() {} public void DrawGridItem() {}
    }
    internal class Plant {
        public Board mBoard = null!;
        private static int nextPointer=100;
        public IntPtr Pointer = (IntPtr)(++nextPointer);
        public uint DataID = 1;
        public SeedType mSeedType = SeedType.Puffshroom, mImitaterType = SeedType.Chomper;
        public int mStartRow,mTargetX,mTargetY,mShootingCounter,mLaunchCounter,mStateCountdown,mDoSpecialCountdown;
        public int mPlantHealth=300,mPlantMaxHealth=300,mPlantCol,mRow,mWidth=80,mHeight=80,mRenderOrder;
        public float mX,mY; public bool mIsOnBoard=true,mDead,mSquished,mIsAsleep;
        public PlantOnBungeeState mOnBungeeState;
        public Action? OnReanim,OnDeath;
        public void UpdateReanim() => OnReanim?.Invoke();
        public void Die() { if(FusionMod.PuffJumperLethalDamagePatch.Prefix(this,true)) { mDead=true; OnDeath?.Invoke(); } }
        public void Squish() {} public void PlantInitialize() {} public void UpdateAbilities() {}
        public bool NotOnGround() => false;
        public UnityEngine.Rect GetPlantRect() => new(mX,mY,mWidth,mHeight);
    }
    internal enum PlantPriority { Normal=0, Projectile=3 }
    internal enum PlantOnBungeeState { NotOnBungee, OnBungee }
    internal enum ProjectileType { Basketball, ZombiePea, Pea }
    internal enum DamageFlags { BypassesShield }
    internal enum ZombieType { Normal, Pogo, JalapenoHead, Imp, Gargantuar, RedeyeGargantuar, Boss, Gravestone, Target }
    internal enum ProjectileMotion { Homing, Star, Straight }
    internal enum DamageRangeFlags { Ground }
    internal sealed class Projectile : Plant {
        public bool Owned = true; public ProjectileType mProjectileType;
        public Plant? FindCollisionTargetPlant() => null; public void UpdateLobMotion() {}
        public ProjectileMotion mMotionType = ProjectileMotion.Homing;
        public uint mTargetZombieID;
        public DamageRangeFlags mDamageRangeFlags;
        public float mPosX, mPosY, mVelX, mVelY, mRotation;
        public int mProjectileAge;
        public void UpdateNormalMotion() {} public void CheckForCollision() {}
    }
    internal readonly record struct TargetPoint(float y);
    
    internal enum ZombiePhase { ZombieNormal, ZombieDying, PogoBouncing, PolevaulterInVault, DolphinInJump,
        ImpGettingThrown,BalloonFlying,BalloonPopping,DiggerTunneling,DiggerTunnelingPauseWithoutAxe,
        DiggerRising,DiggerRiseWithoutAxe,PogoHighBounce1,PogoHighBounce2,PogoHighBounce3,PogoHighBounce4,
        PogoHighBounce5,PogoHighBounce6,PogoForwardBounce2,PogoForwardBounce7,BungeeDiving,BungeeDivingScreaming,BungeeRising }
    internal sealed class Zombie : Plant {
        internal bool Summoned = true;
        public float TargetX = 1000, TargetY;
        public bool Targetable = true, mMindControlled;
        public bool IsDeadOrDying() => mDead || mZombiePhase==ZombiePhase.ZombieDying;
        public bool EffectedByDamage(DamageRangeFlags flags) => Targetable;
        public UnityEngine.Rect GetZombieRect() => new(TargetX, TargetY, 40, 80);
        public ZombieType mZombieType; public int mBodyHealth=300,mBodyMaxHealth=300,mHelmHealth,mHelmMaxHealth,mShieldHealth;
        public int DamageTaken,Deaths; public DamageFlags LastFlags; public Action? OnDamage;
        public void TakeDamage(int damage,DamageFlags flags) { DamageTaken+=damage; LastFlags=flags; OnDamage?.Invoke(); }
        public void DieWithLoot() { ++Deaths; mDead=true; }
        public void EatPlant(Plant thePlant) {}
        public float ZombieTargetLeadX(float time) => TargetX;
        public ZombiePhase mZombiePhase;
        public int mPhaseCounter;
        public void UpdateZombieJalapenoHead() {} public void Update() {} public void UpdateActions() {} public void UpdateZombiePeaHead() {}
    }
}
namespace Il2CppReloaded.Services {
    internal sealed class App { private Il2CppReloaded.Gameplay.Board? board; internal bool ThrowOnBoard = false; public Il2CppReloaded.Gameplay.Board? Board { get => ThrowOnBoard ? throw new InvalidOperationException("board getter failed") : board; set => board = value; } }
    internal sealed class SerializationService {
        public App? App;
        public bool Deserialize() => true;
    }
}
namespace FusionMod {
    internal static class HypnoShooters {
        internal static bool IsSummoned(Il2CppReloaded.Gameplay.Zombie zombie) => zombie.Summoned;
    }
    internal sealed class FusionMod {
        internal static readonly FusionMod Instance = new();
        internal readonly Logger LoggerInstance = new();
        internal sealed class Logger {
            internal int Errors;
            internal void Error(string message) { ++Errors; }
        }
    }
    internal static class ScaredContent {
        internal static bool Playing(Il2CppReloaded.Gameplay.Board? board) => board?.Playing == true;
        internal static void InstantKill(Il2CppReloaded.Gameplay.Zombie zombie) {}
    }
}

namespace UnityEngine {
    internal struct Rect(float x,float y,float width,float height) {
        public float xMin=x,yMin=y; public float xMax=x+width,yMax=y+height;
        public Il2CppReloaded.Gameplay.TargetPoint center => new((yMin+yMax)/2);
    }
}
namespace Il2CppReloaded.Services {
    internal enum ParticleEffect { PlantingPool }
    internal enum FoleyType { ZombieSplash, Chomp }
    internal sealed class SoundSystem { internal int Chomps,Splashes; public Action? OnSound;
        internal void PlayFoley(FoleyType type) { if(type==FoleyType.Chomp) ++Chomps; else ++Splashes; OnSound?.Invoke(); } }
    internal sealed class TestApp { internal SoundSystem SoundSystem=new(); internal int Particles;
        internal void AddTodParticle(float x,float y,int order,ParticleEffect effect) { ++Particles; } }
}
namespace FusionMod {
    internal static class SquashFusions {
        internal static bool Eligible(Il2CppReloaded.Gameplay.Board board,Il2CppReloaded.Gameplay.Zombie zombie,Il2CppReloaded.Gameplay.DamageRangeFlags flags)
            => !zombie.mDead && zombie.mBoard.Pointer==board.Pointer && zombie.Targetable;
    }
    internal static class PlayerPlantingScope { internal static IntPtr ConsumingIngredient; }
    // Squish's own source machine has its independent fixture. This collaborator
    // exposes only the integration states needed by shared native query tests.
    internal static class Squish {
        internal static bool Owns(Il2CppReloaded.Gameplay.Plant p) => FusionPlant.From(p)==FusionPlant.Squish;
        internal static bool Airborne(Il2CppReloaded.Gameplay.Plant p) => Owns(p) && p.mStartRow==2;
        internal static bool Retained(Il2CppReloaded.Gameplay.Plant p) => Owns(p) && !p.mIsOnBoard;
        internal static bool Live(Il2CppReloaded.Gameplay.Plant p) => Owns(p) && ScaredContent.Playing(p.mBoard)
            && !p.mBoard.mPaused && !p.mDead && !p.mSquished && p.mPlantHealth>0;
        internal static void Initialize(Il2CppReloaded.Gameplay.Plant p) {} internal static void Tick(Il2CppReloaded.Gameplay.Plant p) {}
        internal static bool Crush(Il2CppReloaded.Gameplay.Plant p) => false;
    }
}

namespace PlantLibrary {
    internal readonly record struct Identity(Il2CppReloaded.Gameplay.SeedType NativeType,Il2CppReloaded.Gameplay.SeedType Marker);
    internal sealed record Definition(Identity Identity);
    internal static class Plants { internal static Definition? Find(Il2CppReloaded.Gameplay.Plant p) => null; }
}
