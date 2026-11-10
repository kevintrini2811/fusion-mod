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
        public object mApp = new();
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
    internal enum SeedType { None=-1, Plantern, Imitater, Sunflower }
    internal sealed class SeedPacket {
        public IntPtr Pointer=(IntPtr)500; public Board mBoard=null!;
        public SeedType mPacketType=SeedType.Plantern,mImitaterType=SeedType.None;
        public int mRefreshCounter,mRefreshTime=1500,LastCounter=-1,OtherUpdates,ReadyEvents;
        public bool mActive,mRefreshing=true;
        public void Update() {}
    }
    internal class Plant {
        public static int GetRefreshTime(SeedType seed,SeedType imitater)=>1500;
        public Board mBoard = null!;
        public IntPtr Pointer = (IntPtr)100;
        public uint DataID = 1;
    }
    internal enum ProjectileMotion { Homing, Star, Straight }
    internal enum DamageRangeFlags { Ground }
    internal sealed class Projectile : Plant {
        public bool mDead, Owned = true;
        public ProjectileMotion mMotionType = ProjectileMotion.Homing;
        public uint mTargetZombieID;
        public DamageRangeFlags mDamageRangeFlags;
        public float mPosX, mPosY, mVelX, mVelY, mRotation;
        public int mWidth, mHeight, mProjectileAge, mRow;
        public void UpdateNormalMotion() {} public void CheckForCollision() {}
    }
    internal readonly record struct TargetPoint(float y);
    internal readonly record struct TargetRect(float Y) { public TargetPoint center => new(Y); }
    internal enum ZombiePhase { ZombieNormal, ZombieDying }
    internal sealed class Zombie : Plant {
        internal bool Summoned = true;
        public float TargetX = 1000, TargetY;
        public bool Targetable = true;
        public bool EffectedByDamage(DamageRangeFlags flags) => Targetable;
        public TargetRect GetZombieRect() => new(TargetY);
        public float ZombieTargetLeadX(float time) => TargetX;
        public bool mDead;
        public ZombiePhase mZombiePhase;
        public int mPhaseCounter;
        public void Update() {} public void UpdateActions() {} public void UpdateZombiePeaHead() {}
    }
}
namespace Il2CppReloaded.Services {
    internal sealed class App { public Il2CppReloaded.Gameplay.Board? Board; }
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
    }
}
