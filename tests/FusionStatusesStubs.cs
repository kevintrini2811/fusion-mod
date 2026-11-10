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
        public List<Zombie> m_zombies = new();
        public Zombie? ZombieTryToGet(ZombieID id) => m_zombies.FirstOrDefault(z => z.DataID == (uint)id);
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
        public IntPtr Pointer = (IntPtr)100;
        public uint DataID = 1;
        public void IceZombies() { foreach(var z in mBoard.m_zombies) z.HitIceTrap(); }
    }
    internal sealed class Projectile : Plant { public bool Owned; public void DoImpact() {} }
    internal enum ZombieID : uint { None }
    internal enum ZombiePhase { ZombieNormal, ZombieDying }
    internal sealed class Zombie : Plant {
        internal bool Summoned = true;
        public bool mDead;
        public ZombiePhase mZombiePhase;
        public int mPhaseCounter, mButteredCounter, mChilledCounter, mIceTrapCounter;
        public bool ColdImmune, FreezeImmune;
        public Action? ButterCallback;
        public bool IsDeadOrDying() => mDead;
        public bool CanBeFrozen() => !FreezeImmune;
        public bool CanBeChilled() => !ColdImmune;
        public void ApplyButter() {
            FusionMod.FusionStatusNativeButterPatch.Prefix(this, out var state);
            if (!FreezeImmune) mButteredCounter = 400;
            ButterCallback?.Invoke();
            FusionMod.FusionStatusNativeButterPatch.Postfix(this, state);
        }
        public void ApplyChill(bool longer) {
            FusionMod.FusionStatusNativeChillPatch.Prefix(this, out var state);
            if (!ColdImmune) mChilledCounter = Math.Max(mChilledCounter, longer ? 2000 : 1000);
            FusionMod.FusionStatusNativeChillPatch.Postfix(this, longer, state);
        }
        public void HitIceTrap() {
            FusionMod.FusionStatusNativeIcePatch.Prefix(this, out var state);
            if (!ColdImmune) mChilledCounter = Math.Max(mChilledCounter, 2000);
            if (!FreezeImmune) mIceTrapCounter = 400;
            FusionMod.FusionStatusNativeIcePatch.Postfix(this, state);
        }
        public void RemoveColdEffects() { mIceTrapCounter = mChilledCounter = 0; }
        public void RemoveIceTrap() { mIceTrapCounter = 0; }

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
    internal static class FusionProjectileTime { internal static bool Owns(Il2CppReloaded.Gameplay.Projectile p) => p.Owned; }
    internal static class SnowPea { internal static IntPtr CreatingIceTrap; }
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
