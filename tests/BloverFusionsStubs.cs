namespace Il2CppReloaded.Services {
    public sealed class SerializationService {
        public Il2CppReloaded.Gameplay.App? App;
        public bool Deserialize() => true;
    }
}
namespace Il2CppReloaded.Services { public enum FoleyType { Floop } public sealed class SoundSystem { public void PlayFoley(FoleyType sound) {} } }
// Offline doubles. Compiling this suite never executes game code.
namespace HarmonyLib {
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute { public HarmonyPatch(Type type,string method){} }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int value){} }
    public static class Priority { public const int First=800; }
}
namespace Il2CppReloaded.Gameplay {
    public enum GameScenes { Playing, Loading }
    public enum GameMode { Adventure, ChallengeZenGarden }
    public enum PlantOnBungeeState { NotOnBungee, OnBungee }
    public enum ZombieType { Normal,Boss,Gravestone,Target,Balloon,Bungee,Zamboni,Catapult,Bobsled,Gargantuar,RedeyeGargantuar }
    public enum ProjectileType { Pea,Snowpea }
    public enum ParticleEffect { FumeCloud,IceSparkle,SnowpeaSplat }
    public enum GridItemType { ZenTool }
    public enum GridItemState { Normal }
    [Flags] public enum DamageRangeFlags { Ground=1,Flying=2,Submerged=4,OffGround=8,Dog=16,OnlyMindcontrolled=32 }
    [Flags] public enum DamageFlags { BypassesShield=1,HitsShieldAndBody=2 }
    public class App { public Board? Board; public Il2CppReloaded.Services.SoundSystem SoundSystem=new();
        public GameScenes GameScene=GameScenes.Playing; public GameMode GameMode;
        public void AddTodParticle(float x,float y,int order,ParticleEffect kind){}
    }
    public class Board {
        public IntPtr Pointer=(IntPtr)1; public bool mPaused; public App mApp=new();
        public int mFogBlownCountDown,mMainCounter; public List<Zombie> m_zombies=new();
        public List<Projectile> m_projectiles=new(); public List<Plant> m_plants=new();
        public GridItems m_gridItems;
        public Board(){m_gridItems=new(this);}
        public int PixelToGridX(int x,int y)=>x/80; public int Rand(int range)=>range/2;
        public void UpdateGame(){} public void Serialize(){}
    }
    public class Plant {
        public Board mBoard=new(); public App mApp=>mBoard.mApp; public IntPtr Pointer=(IntPtr)1;
        public uint DataID=1; public SeedType mSeedType,mImitaterType;
        public int mPlantHealth,mPlantMaxHealth,mDoSpecialCountdown,mLaunchCounter,mStartRow,mTargetX,mTargetY;
        public int mPlantCol,mRow,mWidth=80,mHeight=80,mRenderOrder,mX,mY;
        public bool mDead,mIsOnBoard=true,mSquished,OffGround,mIsAsleep;
        public PlantOnBungeeState mOnBungeeState; public bool NotOnGround()=>OffGround;
        public void Die()=>mDead=true; public void PlantInitialize(){} public void UpdateAbilities(){}
        public void UpdateShooting(){} public void DoSpecial(){}
    }
    public class Zombie {
        static uint next=10; public uint DataID=++next; public IntPtr Pointer=IntPtr.Zero; public Board mBoard=null!;
        public ZombieType mZombieType; public int mRow,mX,mIceTrapCounter,mBodyMaxHealth=270,mHelmMaxHealth=100;
        public float mPosX,mPosY,mAltitude; public bool mDead,Dying,mMindControlled,mBlowingAway,mYuckyFace,Air,Excluded;
        public int Damage,Chills,Redirects,mRenderOrder; public bool CanChill=true; public Action? OnDamage;
        public bool IsFlying()=>Air; public bool IsDeadOrDying()=>Dying; public bool CanBeChilled()=>CanChill;
        public bool EffectedByDamage(DamageRangeFlags flags)=>!Excluded;
        public void TakeDamage(int value,DamageFlags flags){Damage+=value;OnDamage?.Invoke();}
        public void ApplyChill(bool ice)=>++Chills; public void EatPlant(){} public void Deserialize(){}
    }
    public class Projectile {
        static uint next=100; public uint DataID=++next; public IntPtr Pointer=(IntPtr)1; public Board mBoard=null!;
        public ProjectileType mProjectileType=ProjectileType.Snowpea; public float mCobTargetX,mPosX,mPosY;
        public int mCobTargetRow,mRenderOrder; public bool mDead;
        public void Die()=>mDead=true; public DamageFlags GetDamageFlags(Zombie zombie)=>DamageFlags.HitsShieldAndBody;
        public void DoImpact(){} public void ConvertToPea(){} public void ConvertToFireball(){} public void Update(){}
    }
    public class GridItems(Board board) : List<GridItem> {
        public bool Fail;
        public GridItem? DataArrayAlloc(){if(Fail)return null;var item=new GridItem{mBoard=board};Add(item);return item;}
    }
    public class GridItem {
        public Board mBoard=null!; public bool mDead; public GridItemType mGridItemType; public GridItemState mGridItemState;
        public SeedType mSeedType; public int mSunCount,mGridX,mGridY,mGridItemCounter,mRenderOrder;
        public float mX,mY,mPosX,mPosY;
        public void Init(App app,GridItemType type)=>mGridItemType=type;
        public void GridItemDie()=>mDead=true; public void Update(){} public void DrawGridItem(){}
    }
}
namespace FusionMod {
    using Il2CppReloaded.Gameplay;
    internal static class CherryBombPlants { internal static IntPtr DisposingBoard = IntPtr.Zero; }
    internal static class ChomperFusions {
        internal static bool Active(Plant p)=>BloverFusions.Playing(p.mBoard)&&!p.mDead&&p.mIsOnBoard&&!p.mSquished
            &&!p.NotOnGround()&&p.mOnBungeeState==PlantOnBungeeState.NotOnBungee;
    }
    internal static class SupportFusions { internal static void Redirect(Zombie z){z.mYuckyFace=true;++z.Redirects;} }
    internal static class Overheat {
        internal static readonly HashSet<Zombie> States = new();
        internal static bool Has(Zombie zombie) => States.Contains(zombie);
        internal static void Apply(Zombie zombie) => States.Add(zombie);
        internal static void Forget(Zombie zombie) => States.Remove(zombie);
    }
    internal static class PoisonShooters {
        internal static Dictionary<Zombie,(int Level,int Remaining,int Countdown)> States=new();
        internal static int Level(Zombie z)=>Capture(z)?.Level??0;
        internal static void AddLevel(Board board,Zombie z){var s=Capture(z)??(0,0,1);States[z]=(Math.Min(30,s.Level+1),s.Remaining+(s.Level>=5?500:0),s.Countdown);}
        internal static (int Level,int Remaining,int Countdown)? Capture(Zombie z)=>States.TryGetValue(z,out var s)?s:null;
        internal static void Restore(Zombie z,int level,int remaining,int countdown){if(!States.ContainsKey(z))States[z]=(level,remaining,countdown);}
        internal static void Forget(Zombie z)=>States.Remove(z);
        internal static void ForgetBoard(Board board) {
            foreach(var zombie in States.Keys.Where(z=>z.mBoard==board).ToArray()) States.Remove(zombie);
        }
    }
    internal static class SnowPea {
        internal static Dictionary<Zombie,(int Points,int Threshold)> States=new();
        internal static (int Points,int Threshold)? Capture(Zombie z)=>States.TryGetValue(z,out var s)?s:null;
        internal static void Restore(Zombie z,int points,int threshold){if(!States.ContainsKey(z))States[z]=(points,threshold);}
        internal static void Hit(Zombie z,int points){if(z.mIceTrapCounter>0)return;var s=Capture(z)??(0,100);
            int p=s.Points+points,t=s.Threshold;if(p>=t){p=0;t=Math.Min(1000,t+100);z.mIceTrapCounter=400;}States[z]=(p,t);}
        internal static void Forget(Zombie z)=>States.Remove(z);
        internal static void ForgetBoard(Board board) {
            foreach(var zombie in States.Keys.Where(z=>z.mBoard==board).ToArray()) States.Remove(zombie);
        }
    }
    internal static class ScaredyProjectiles { internal static int Family(Projectile s)=>s.mCobTargetX==-422040?3:0; internal static int Damage(Projectile s)=>40; }
    internal static class ImportedPeaVisuals { internal static void FumeSmoke(Plant plant,FusionPlant identity) {} internal static void EnsurePolarProjectile(Projectile s){} }
}

namespace FusionMod { internal static class FumeProjectiles { internal static int ModifiedDamage(Il2CppReloaded.Gameplay.Projectile shot,int damage)=>damage; } }
