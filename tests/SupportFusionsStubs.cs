// Offline doubles for production SupportFusions; no game code is loaded.
namespace HarmonyLib {
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute { public HarmonyPatch(Type t,string n) {} }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int n) {} }
    public static class Priority { public const int First=800; }
}
namespace Il2CppReloaded.Gameplay {


    public enum ProjectileType { Pea,Star }
    public enum CursorType { Normal,Shovel,Glove }
    public enum PlantOnBungeeState { NotOnBungee,OnBungee }
    public enum GameScenes { Playing,Loading }
    public enum GameMode { Adventure,ChallengeZenGarden }
    public enum ZombieType { Normal,Boss,Gravestone,Target,Zamboni,Catapult,Bungee,Balloon,Gargantuar }
    public enum ParticleEffect { FumeCloud }
    [Flags] public enum DamageRangeFlags { Ground=1,Flying=2,Submerged=4,Dog=8,OffGround=16,Underground=64,OnlyMindcontrolled=128 }
    [Flags] public enum DamageFlags { HitsShieldAndBody=1,DoesntLeaveBody=2 }
    public readonly record struct Rect(float xMin,float yMin,float xMax,float yMax);
    public class App { public GameScenes GameScene=GameScenes.Playing; public GameMode GameMode;
        public int Effects; public void AddTodParticle(float x,float y,int o,ParticleEffect p)=>++Effects; }
    public class Cursor { public CursorType CursorType; }
    public class Lawn { public Plant? PumpkinPlant; }
    public class Board {
        public IntPtr Pointer=(IntPtr)1; public bool mPaused; public int mFogBlownCountDown,Sun=100;
        public App mApp=new(); public Cursor mCursorObject=new(); public Lawn Lawn=new();
        public List<Zombie> m_zombies=new(); public List<Projectile> Shots=new(); public Plant? Carried,Created;
        public bool FailCreate,FailTakeSun; public int Takes,Allocations;
        public Lawn GetPlantsOnLawn(int c,int r)=>Lawn;
        public bool CanTakeSunMoney(int n,int p)=>Sun>=n;
        public bool TakeSunMoney(int n,int p){if(FailTakeSun||Sun<n)return false;Sun-=n;++Takes;return true;}
        public Plant? AddPlant(int c,int r,SeedType t,SeedType m){++Allocations;if(FailCreate)return null;
            return Created=new(){mBoard=this,mPlantCol=c,mRow=r,mSeedType=t,mImitaterType=m};}
    }
    public class Plant {
        static int next=10; public IntPtr Pointer=(IntPtr)(++next); public uint DataID=(uint)next;
        public Board mBoard=new(); public App mApp=>mBoard.mApp;
        public SeedType mSeedType = SeedType.Puffshroom,mImitaterType = SeedType.Puffshroom; public int mPlantHealth=300,mPlantMaxHealth=300;
        public int mDoSpecialCountdown,mStartRow,mTargetX,mTargetY,mLaunchCounter,mPlantCol,mRow,mWidth=80,mHeight=80,mRenderOrder;
        public float mX,mY; public bool mDead,mIsOnBoard=true,mSquished,mIsAsleep,OffGround;
        public PlantOnBungeeState mOnBungeeState;
        public bool NotOnGround()=>OffGround; public void Die()=>mDead=true;
        public void PlantInitialize(){} public void UpdateAbilities(){} public void UpdateShooter(){}
        public void DoSpecial(){} public void MouseDown(){} public void FindTargetZombie(){}
    }
    public class Projectile {
        public Board mBoard=null!; public ProjectileType mProjectileType; public float mCobTargetX,Angle,Speed; public bool mDead;
        public void DoImpact(){}
    }
    public class Zombie {
        static int next=100; public IntPtr Pointer=(IntPtr)(++next); public uint DataID=(uint)next;
        public Board mBoard=null!; public bool mDead,mMindControlled,mYuckyFace,Dying,Excluded; public ZombieType mZombieType;
        public int mRow,mTargetRow=-1,mYuckyFaceCounter,mRenderOrder,Damage,Stops; public float mPosX,mPosY;
        public bool IsDeadOrDying()=>Dying; public Rect GetZombieRect()=>new(mPosX,0,mPosX+20,20);
        public bool EffectedByDamage(DamageRangeFlags f)=>!Excluded
            &&mMindControlled==f.HasFlag(DamageRangeFlags.OnlyMindcontrolled);
        public Action? OnDamage;
        public void TakeDamage(int n,DamageFlags f){Damage+=n;OnDamage?.Invoke();}
        public void StopEating()=>++Stops; public void StartMindControlled()=>mMindControlled=true;
        public void EatPlant(){} public void CanTargetPlant(){}
    }
}
namespace LawnGlove { public static class LawnGloveMod {
    public static Il2CppReloaded.Gameplay.Plant? GetCarriedPlant(Il2CppReloaded.Gameplay.Board b)=>b.Carried; } }
namespace FusionMod {
    using Il2CppReloaded.Gameplay;
    // Passive pot behavior has its own production-source adapter suite.
    internal static class PotBoosts { internal static bool HasSupport(Plant plant, FusionPlant support) => false; }
    internal static class PotDefense { internal const int GarlicDirection = -1130; }
    internal static class CherryBombPlants {internal static IntPtr DisposingBoard;}
    internal static class ChomperFusions { internal static bool Active(Plant p)=>!p.mDead&&!p.mBoard.mPaused
        &&p.mIsOnBoard&&!p.mSquished&&!p.OffGround&&p.mOnBungeeState==PlantOnBungeeState.NotOnBungee
        &&p.mApp.GameScene==GameScenes.Playing&&p.mApp.GameMode!=GameMode.ChallengeZenGarden
        &&p.mBoard.Pointer!=CherryBombPlants.DisposingBoard; }
    internal static class SquashFusions { internal static bool Eligible(Board b,Zombie z,DamageRangeFlags f)=>
        b.Pointer==z.mBoard.Pointer&&!z.mDead&&!z.IsDeadOrDying()&&!z.mMindControlled&&z.EffectedByDamage(f); }
    internal static class PlayerPlantingScope { internal static void Consume(Plant p)=>p.Die(); }
    internal static class RetaliationProjectiles {
        internal static Projectile? Spawn(Board b,float x,float y,int o,int r,bool star,float angle,float speed){
            var p=new Projectile{mBoard=b,mProjectileType=ProjectileType.Star,Angle=angle,Speed=speed};b.Shots.Add(p);return p;}
    }
}

namespace FusionMod { internal static class ImportedPeaVisuals { internal static void ShowFalloutSplat(Il2CppReloaded.Gameplay.Zombie zombie) {} } }
