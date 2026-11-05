// Offline doubles. Never loads a game, Unity player, profile or save.
namespace HarmonyLib {
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute {
        public HarmonyPatch() {} public HarmonyPatch(Type type, string method) {} }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute {
        public int Value; public HarmonyPriority(int value) => Value=value; }
    public static class Priority { public const int First=800; }
    public static class AccessTools { public static System.Reflection.MethodBase Method(Type type,string name) => type.GetMethod(name)!; }
}
namespace Il2CppReloaded.Gameplay {

    public enum ProjectileType { Pea, PeashooterPea, Snowpea, Fireball }
    public enum CursorType { Normal, Shovel, PlantFromBank, Glove }
    public enum PlantOnBungeeState { NotOnBungee, OnBungee }
    public enum GameScenes { Playing, Loading }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public enum ZombieType { Normal, Boss, Gravestone, Target }
    public enum PlantingReason { Ok, NotHere }
    [Flags] public enum DamageRangeFlags { Ground=1, Flying=2, Submerged=4, Dog=8, OffGround=16, Underground=64 }
    [Flags] public enum DamageFlags { HitsShieldAndBody=1, BypassesShield=2 }
    public enum ParticleEffect { Powie, FumeCloud }
    public readonly record struct Rect(float xMin,float yMin,float xMax,float yMax);
    public sealed class Cursor { public CursorType CursorType; }
    public sealed class App { public GameScenes GameScene=GameScenes.Playing; public GameMode GameMode;
        public void AddTodParticle(float x,float y,int order,ParticleEffect type) {} }
    public sealed class Board {
        public IntPtr Pointer=(IntPtr)1; public bool mPaused; public App mApp=new();
        public Cursor mCursorObject=new(); public List<Zombie> m_zombies=new(); public List<Plant> m_plants=new(); public int mMainCounter;
        public Plant? Carried, Created; public bool FailCreate, BlockPlacement;
        public Plant? AddPlant(int col,int row,SeedType seed,SeedType marker) {
            if(FailCreate)return null;
            Created=new Plant { Pointer=(IntPtr)100,DataID=100,mBoard=this,mSeedType=seed,mImitaterType=marker,mPlantCol=col,mRow=row };
            FusionMod.ShooterGrowth.Initialize(Created); return Created;
        }
        public void DisposeBoard() {} public void UpdateGame() {}
        public Projectile AddProjectile() => new() { mBoard = this };
    }
    public sealed class Plant {
        public IntPtr Pointer=(IntPtr)2; public uint DataID=2; public Board mBoard=new();
        public App mApp => mBoard.mApp;
        public SeedType mSeedType = SeedType.Puffshroom,mImitaterType = SeedType.Puffshroom; public int mStartRow,mPlantCol,mRow;
        public int mPlantHealth=300,mPlantMaxHealth=300; public bool mDead,mIsOnBoard=true,mSquished,OffGround;
        public PlantOnBungeeState mOnBungeeState;
        public bool NotOnGround()=>OffGround;
        public void Fire() {} public void PlantInitialize() {} public void UpdateAbilities() {} public void MouseDown() {}
    }
    public sealed class Projectile {
        private static uint next=300; public uint DataID=++next; public IntPtr Pointer=(IntPtr)(++next); public Board mBoard=null!;
        public ProjectileType mProjectileType; public float mCobTargetX; public bool mDead;
        public void ProjectileInitialize() {} public void ConvertToFireball() {}
        public void Die() {mDead=true;} public void Reset() {DataID++;mDead=false;}
        public void DoImpact(Zombie? zombie) {
            FusionMod.BlazeImpactScope.Prefix(this,zombie!,out var old);
            FusionMod.PoisonImpactScope.Prefix(this,zombie!,out var poisonOld);
            try { if(zombie!=null)zombie.TakeDamage(20,DamageFlags.HitsShieldAndBody); FusionMod.PoisonImpactScope.Postfix(); }
            finally {FusionMod.BlazeImpactScope.Finalizer(old); FusionMod.PoisonImpactScope.Finalizer(poisonOld);}
        }
    }
    public sealed class Zombie { public void ApplyBurn() {} public void Update() {}
        private static uint next=400; public uint DataID=++next; public IntPtr Pointer=(IntPtr)(++next); public Board mBoard=null!;
        public int Health=10000,Damage,mRow,mRenderOrder,mChilledCounter,mIceTrapCounter;
        public float mPosY; public bool mDead,mMindControlled,Dying; public ZombieType mZombieType;
        public bool IsDeadOrDying()=>Dying||Health<=0;
        public float X; public Rect GetZombieRect()=>new(X,0,X+20,20);
        public bool EffectedByDamage(DamageRangeFlags range)=>true;
        public Action? OnDamage;
        public void TakeDamage(int damage,DamageFlags flags) {
            FusionMod.BlazeDamagePatch.Prefix(this,ref damage);
            FusionMod.PoisonDamagePatch.Prefix(this,ref damage);
            FusionMod.OverheatDamagePatch.Prefix(this,ref damage);
            Damage+=damage; Health-=damage; OnDamage?.Invoke();
        }
        public void RemoveColdEffects(){mChilledCounter=mIceTrapCounter=0;}
        public void ApplyChill(){} public void HitIceTrap(){} public void DieWithLoot(){} public void DieNoLoot(){} public void Reset(){}
    }
}
namespace LawnGlove {
    public static class LawnGloveMod {public static Il2CppReloaded.Gameplay.Plant? GetCarriedPlant(Il2CppReloaded.Gameplay.Board board)=>board.Carried;}
}
namespace FusionMod {
    using Il2CppReloaded.Gameplay;
    internal static class ChomperFusions {
        internal static bool Active(Plant p)=>p.mBoard!=null&&!p.mBoard.mPaused&&!p.mDead&&p.mIsOnBoard&&!p.mSquished&&!p.NotOnGround()
            &&p.mOnBungeeState==PlantOnBungeeState.NotOnBungee&&p.mApp.GameScene==GameScenes.Playing&&p.mApp.GameMode!=GameMode.ChallengeZenGarden
            &&p.mBoard.Pointer!=CherryBombPlants.DisposingBoard;
    }
    internal static class CherryBombPlants {internal static IntPtr DisposingBoard;}
    internal static class ImportedPeaVisuals { internal static void GarlicCloud(Il2CppReloaded.Gameplay.Board board,float x,float y,int order) {}internal static void RegisterPoisonProjectile(Projectile p)=>++Registrations; internal static int Registrations; internal static void RegisterBlazeProjectile(Projectile p)=>++Registrations; internal static void ForgetProjectile(Projectile p){} }
    internal static class FusionService {internal static PlantingReason CheckFusionPlacement(Board b,Plant p,FusionPlant r)=>b.BlockPlacement?PlantingReason.NotHere:PlantingReason.Ok;}
    internal static class PlayerPlantingScope {internal static void Consume(Plant p)=>p.mDead=true;}
}
