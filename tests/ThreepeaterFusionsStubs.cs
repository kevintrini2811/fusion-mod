// Offline doubles. Never loads a game, Unity player, profile or save.
namespace HarmonyLib {
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute {
        public HarmonyPatch() {} public HarmonyPatch(Type type, string method) {} }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute {
        public int Value; public HarmonyPriority(int value) => Value=value; }
    public static class Priority { public const int First=800, Last=0; }
    public static class AccessTools { public static System.Reflection.MethodBase Method(Type type,string name) => type.GetMethod(name)!; }
}
namespace Il2CppReloaded.Gameplay {

    public enum ProjectileType { Pea, PeashooterPea, Snowpea, Fireball, PeashooterFireball }
    public enum ProjectileMotion { Straight, Threepeater }
    public enum ZombieID : uint { None }
    public enum CursorType { Normal, Shovel, PlantFromBank, Glove }
    public enum PlantOnBungeeState { NotOnBungee, OnBungee }
    public enum GameScenes { Playing, Loading }
    public enum GameMode { Adventure, ChallengeZenGarden, PuzzleIZombie1, PuzzleIZombieEndless }
    public enum ZombieType { Normal, Boss, Gravestone, Target }
    public enum PlantWeapon { Primary }
    public enum PlantingReason { Ok, NotHere }
    [Flags] public enum DamageRangeFlags { Ground=1, Flying=2, Submerged=4, Dog=8, OffGround=16, Underground=64 }
    [Flags] public enum DamageFlags { HitsShieldAndBody=1, BypassesShield=2 }
    public enum ParticleEffect { Powie, FumeCloud }
    public readonly record struct Rect(float xMin,float yMin,float xMax,float yMax);
    public sealed class Cursor { public CursorType CursorType; }
    public sealed class App { public GameScenes GameScene=GameScenes.Playing; public GameMode GameMode;
        public Il2CppReloaded.Services.SoundSystem SoundSystem=new();
        public void AddTodParticle(float x,float y,int order,ParticleEffect type) {} }
    public sealed class Board {
        public IntPtr Pointer=(IntPtr)1; public bool mPaused; public App mApp=new();
        public int Rand(int range)=>0;
        public Projectile AddProjectile() => new() { mBoard = this };
        public Projectile AddProjectile(float x,float y,int order,int row,ProjectileType type) { var shot=new Projectile {mBoard=this,mPosX=x,mPosY=y,mRenderOrder=order,mRow=row,mProjectileType=type}; Shots.Add(shot); return shot; }
        public List<Coin> Coins=new();
        public Coin AddCoin(float x,float y,CoinType type,CoinMotion motion) {var c=new Coin {mBoard=this,mType=type};Coins.Add(c);return c;}
        public void AddSunMoney(int amount,int playerIndex) {}
        public Cursor mCursorObject=new(); public List<Zombie> m_zombies=new(); public List<Plant> m_plants=new(); public int mMainCounter;
        public int NumRows=5; public List<Projectile> Shots=new();
        public List<Projectile> m_projectiles=>Shots;
        public Zombie? ZombieTryToGet(ZombieID id)=>m_zombies.FirstOrDefault(z=>z.DataID==(uint)id);
        public int GetNumRows()=>NumRows;
        public int GridToPixelX(int col,int row)=>col*80;
        public int GridToPixelY(int col,int row)=>row*100;
        public Plant? Carried, Created; public bool FailCreate, BlockPlacement;
        public Plant? AddPlant(int col,int row,SeedType seed,SeedType marker) {
            if(FailCreate)return null;
            Created=new Plant { Pointer=(IntPtr)100,DataID=100,mBoard=this,mSeedType=seed,mImitaterType=marker,mPlantCol=col,mRow=row };
            FusionMod.ShooterGrowth.Initialize(Created); return Created;
        }
        public void DisposeBoard() {} public void UpdateGame() {}
    }
    public sealed class Plant {
        public IntPtr Pointer=(IntPtr)2; public uint DataID=2; public Board mBoard=new();
        public App mApp => mBoard.mApp;
        public SeedType mSeedType = SeedType.Puffshroom,mImitaterType = SeedType.Puffshroom; public int mStartRow,mPlantCol,mRow,mLaunchCounter,mLaunchRate; public int mRenderOrder; public float mX,mY; public int mWidth=80,mHeight=80; public bool mIsAsleep;
        public int mPlantHealth=300,mPlantMaxHealth=300,mTargetX,mTargetY; public bool mDead,mIsOnBoard=true,mSquished,OffGround;
        public PlantOnBungeeState mOnBungeeState;
        public bool NotOnGround()=>OffGround;
        public DamageRangeFlags GetDamageRangeFlags(PlantWeapon weapon)=>DamageRangeFlags.Ground;
        public void Die()=>mDead=true;
        public void Reset() {}
        public void UpdateShooter() {}
        public void Fire(Zombie? target,int row,PlantWeapon weapon) {
            if(!FusionMod.ThreepeaterNativeFirePatch.Prefix(this))return;
            FusionMod.PoisonFireScope.Prefix(this,out var poisonOld);
            try {
                var p=new Projectile {mBoard=mBoard,mProjectileType=FusionMod.FusionPlant.IsSnowShooter(FusionMod.FusionPlant.From(this)) ? ProjectileType.Snowpea:ProjectileType.Pea,
                    mRow=row,mVelY=Math.Sign(row-mRow)*3,mShadowY=mRow*100-(row-mRow)*100};
                FusionMod.ThreepeaterProjectilePatch.Postfix(p);
                FusionMod.PoisonShooters.Track(p);
                mBoard.Shots.Add(p);
            } finally {FusionMod.PoisonFireScope.Finalizer(poisonOld);}
        } public void PlantInitialize() {} public void UpdateAbilities() {} public void MouseDown() {} public void DoSpecial() {}
    }
    public sealed class Projectile {
        private static uint next=300; public uint DataID=++next; public IntPtr Pointer=(IntPtr)(++next); public Board mBoard=null!;
        public ProjectileType mProjectileType; public ProjectileMotion mMotionType; public bool mDead;
        public float mVelX,mVelY,mVelZ,mAccZ,mShadowY,mCobTargetX,mPosX,mPosY,mPosZ;
        public float mX,mY;
        public int mRow,mCobTargetRow,mProjectileAge,mClickBackoffCounter,mRenderOrder;
        public ZombieID mTargetZombieID;
        public void Update() {
            if(FusionMod.CherrySpreaderUpdatePatch.Prefix(this))++mProjectileAge;
            FusionMod.CherrySpreaderUpdatePatch.Postfix(this);
        }
        public void ProjectileInitialize() {} public void ConvertToFireball() {}
        public void Die() {FusionMod.ImportedPeaVisuals.ForgetProjectile(this);mDead=true;}
        public void Reset() {FusionMod.CherrySpreaderCleanupPatch.Prefix(this);DataID++;mDead=false;mCobTargetX=0;mCobTargetRow=0;mTargetZombieID=0;mProjectileAge=0;}
        public void DoSplashDamage(Zombie zombie) {}
        public DamageFlags GetDamageFlags(Zombie zombie) {FusionMod.ScorchedDamageTargetPatch.Postfix(this,zombie);return DamageFlags.HitsShieldAndBody;}
        public void DoImpact(Zombie? zombie) {
            FusionMod.CherrySpreaderProjectileDamageScope.Prefix(this,out var cherryBoard);
            try {
            if(!FusionMod.CherrySpreaderImpactPatch.Prefix(this,zombie!))return;
            FusionMod.CherryImpactScope.Prefix(this,zombie!,out var cherryOld);
            FusionMod.ScorchedImpactPatch.Prefix(this,zombie!,out var scorchedOld);
            FusionMod.BlazeImpactScope.Prefix(this,zombie!,out var old);
            FusionMod.PoisonImpactScope.Prefix(this,zombie!,out var poisonOld);
            try { if(zombie!=null)FusionMod.ScorchedDamageTargetPatch.Postfix(this,zombie); if(zombie!=null)zombie.TakeDamage(mProjectileType==ProjectileType.Fireball ? 40 : 20,DamageFlags.HitsShieldAndBody); FusionMod.PoisonImpactScope.Postfix(); }
            finally {FusionMod.ScorchedImpactPatch.Finalizer(scorchedOld); FusionMod.BlazeImpactScope.Finalizer(old); FusionMod.PoisonImpactScope.Finalizer(poisonOld);FusionMod.CherryImpactScope.Finalizer(cherryOld);}
            } finally {FusionMod.CherrySpreaderProjectileDamageScope.Finalizer(cherryBoard);}
        }
    }
    public sealed class Zombie { public void ApplyBurn() {} public void Update() {}
        private static uint next=400; public uint DataID=++next; public IntPtr Pointer=(IntPtr)(++next); public Board mBoard=null!;
        public int Health=10000,Damage,mRow,mRenderOrder,mChilledCounter,mIceTrapCounter;
        public float mPosX,mPosY; public bool mDead,mMindControlled,Dying,Flying; public ZombieType mZombieType;
        public bool IsFlying()=>Flying;
        public bool IsDeadOrDying()=>Dying||Health<=0;
        public float X; public Rect GetZombieRect()=>new(X,mRow*100,X+20,mRow*100+20);
        public bool EffectedByDamage(DamageRangeFlags range)=>Flying ? (range&DamageRangeFlags.Flying)!=0 : (range&DamageRangeFlags.Ground)!=0;
        public Action? OnDamage;
        public void TakeDamage(int damage,DamageFlags flags) {
            FusionMod.ScorchedDamagePatch.Prefix(this,ref damage);
            FusionMod.BlazeDamagePatch.Prefix(this,ref damage);
            FusionMod.PoisonDamagePatch.Prefix(this,ref damage);
            FusionMod.OverheatDamagePatch.Prefix(this,ref damage);
            FusionMod.CherryDamagePatch.Prefix(this,ref damage);
            FusionMod.CherrySpreaderChargePatch.Prefix(this,damage,true,out var cherryBoard);
            try {Damage+=damage; Health-=damage; OnDamage?.Invoke();}
            finally {FusionMod.CherrySpreaderChargePatch.Finalizer(cherryBoard);}
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
    internal static class CherryBombPlants {internal static IntPtr DisposingBoard;
        internal sealed record Death(Board Board, FusionPlant Identity=default, uint Id=0, int Col=0, int Row=0, float X=0, float Y=0, int Order=0);
        internal static bool CanTriggerDeath(Plant p)=>ChomperFusions.Active(p);
        internal static int Explode(Death death)=>0;
        internal static void ExplodeFusion(Plant plant){} }
    internal static class ExplodeOShooter {internal static void Detonate(Projectile shot){} }
    internal static class SquashFusions {
        internal static bool Eligible(Board b,Zombie z,DamageRangeFlags range)=>z.mBoard==b&&!z.mDead&&!z.IsDeadOrDying()&&!z.mMindControlled&&z.EffectedByDamage(range);
    }
    internal static class ImportedPeaVisuals { internal static void GarlicCloud(Il2CppReloaded.Gameplay.Board board,float x,float y,int order) {}
        internal static void EnsurePhoenixProjectile(Projectile p) {}
        internal static void RegisterSolarProjectile(Projectile p) {}
        internal static void EnsureCherrySpreaderProjectile(Projectile p) {++Registrations;}
        internal static void RegisterExplodeProjectile(Projectile p) {} internal static void RegisterCherryProjectile(Projectile p) {} internal static void EnsureCherryProjectile(Projectile p) {}
        internal static void RegisterScorchedProjectile(Projectile p) {}
        internal static void EnsureScorchedProjectile(Projectile p) {}internal static void RegisterPoisonProjectile(Projectile p)=>++Registrations; internal static int Registrations; internal static void RegisterBlazeProjectile(Projectile p)=>++Registrations; internal static void ForgetProjectile(Projectile p){} }
    internal static class FusionService {internal static PlantingReason CheckFusionPlacement(Board b,Plant p,FusionPlant r)=>b.BlockPlacement?PlantingReason.NotHere:PlantingReason.Ok;}
    internal static class PlayerPlantingScope {internal static void Consume(Plant p)=>p.mDead=true;}
}

namespace Il2CppReloaded.Services {
    public enum FoleyType { Cherrybomb }
    public sealed class SoundSystem {public void PlayFoley(FoleyType type) {}}
}

namespace FusionMod {
    internal static class ElementalNuts { internal static void Heal(Il2CppReloaded.Gameplay.Board b,bool fire,int row) {} }
    internal static class DoomEffects {
        internal static List<int> Rows=new();
        internal static void Damage(Il2CppReloaded.Gameplay.Board board,int row,float x,float y,int order,bool fire=false) {Rows.Add(row);}
    }
}

namespace FusionMod { internal static class FumeProjectiles { internal static int ModifiedDamage(Il2CppReloaded.Gameplay.Projectile shot,int damage)=>damage; } }

namespace Il2CppReloaded.Gameplay {
    public enum CoinType { SmallSun, Sun }
    public enum CoinMotion { FromPlant }
    public sealed class Coin {
        private static uint next=700;
        public uint DataID=++next; public IntPtr Pointer=>(IntPtr)DataID;
        public Board mBoard=null!; public CoinType mType;
        public SeedType mUsableSeedType=SeedType.None;
        public int GetSunValue()=>15; public void ScoreCoin() {} public void Die() {} public void Reset() {}
    }
}
namespace FusionMod {
    internal static partial class ShootingPots {
        internal static void FireShot(Il2CppReloaded.Gameplay.Plant plant, int row)
            => plant.Fire(null!, row, Il2CppReloaded.Gameplay.PlantWeapon.Primary);
    }
    internal sealed class FusionMod {
        internal static FusionMod Instance=new(); internal Logger LoggerInstance=new();
        internal sealed class Logger { internal void Msg(string text) {} }
    }
    internal static class NutProjectiles {
        private static HashSet<(uint,uint)> hits=new();
        internal static bool Remember(Il2CppReloaded.Gameplay.Projectile shot, Il2CppReloaded.Gameplay.Zombie zombie)=>hits.Add((shot.DataID,zombie.DataID));
    }
}
