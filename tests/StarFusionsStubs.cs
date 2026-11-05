// Offline collaborators. No native game assemblies are loaded by this suite.
namespace HarmonyLib {
    [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute { public HarmonyPatch() {} public HarmonyPatch(Type type, string name) {} }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int priority) {} }
    public static class AccessTools { public static System.Reflection.MethodInfo Method(Type t,string n) => t.GetMethod(n)!; }
    public static class Priority { public const int First = 800, Last = 0; }
}
namespace Il2CppReloaded.Services { public enum FoleyType { Fume, Frozen, Splat } }
namespace Il2CppReloaded.Gameplay {
    public enum PlantWeapon { Primary }
    public enum ProjectileType { Pea, Star, Spike, ZombiePea, Basketball }
    public enum ProjectileMotion { Straight, Star }
    public enum ZombieID : uint { }
    public enum ZombieType { Catapult, Balloon = 30, Zamboni = 12, Normal = 0, Gravestone = 99, Target }
    public enum ParticleEffect { PeaSplat, SnowpeaSplat, Powie, FumeCloud, PoolSplash, IceSparkle }
    [Flags] public enum DamageRangeFlags { Ground = 1, Flying = 2, Dog = 4 }
    [Flags] public enum DamageFlags { None = 0, BypassesShield = 1, Spike = 2 }
    public readonly record struct Point(float x,float y);
    public readonly record struct Rect(float xMin, float yMin, float xMax, float yMax) { public Point center => new((xMin+xMax)/2,(yMin+yMax)/2); }
    public class SoundSystem { public void PlayFoley(Il2CppReloaded.Services.FoleyType type) {} }
    public enum PlantingReason { Ok, NotHere, OnlyInPool }
    public enum CoinType { UsableSeedPacket }
    public enum CoinMotion { FromPlant }
    public class CoinController { public SeedType SeedType; }
    public class Coin { public SeedType mUsableSeedType; public CoinController? mController; }
    public class Lawn { public Plant? NormalPlant,PumpkinPlant; }
    public enum GameScenes { Playing }
    public class App { public GameScenes GameScene = GameScenes.Playing; public SoundSystem SoundSystem = new(); public int Effects; public void AddTodParticle(float x,float y,int order,ParticleEffect effect) => ++Effects; }
    public class Board {
        public IntPtr Pointer = (IntPtr)1;
        public bool mPaused, FailAllocation, ReserveFailure;
        public Cursor mCursorObject = new();
        public int mMainCounter, mPlantsEaten, RowBlasts, Fwooshes;
        public int[] mIceTimer = new int[5];
        public void DoFwoosh(int row) => ++Fwooshes;
        public App mApp = new();
        public List<Plant> m_plants = new(); public List<Zombie> m_zombies = new(); public List<Projectile> m_projectiles = new();
        public int GetNumRows() => 5;
        public int GridToPixelX(int col,int row) => col*80; public int GridToPixelY(int col,int row) => row*100;
        public int Cards;
        public Coin? AddCoin(float x,float y,CoinType type,CoinMotion motion) { if(FailAllocation)return null; ++Cards;return new Coin(); }
        public Lawn GetPlantsOnLawn(int col,int row) => new() { NormalPlant=m_plants.FirstOrDefault(p=>!p.mDead&&p.mPlantCol==col&&p.mRow==row&&p.mSeedType!=SeedType.Pumpkinshell),PumpkinPlant=m_plants.FirstOrDefault(p=>!p.mDead&&p.mPlantCol==col&&p.mRow==row&&p.mSeedType==SeedType.Pumpkinshell) };
        public bool StageHasFog() => true;
        public FogArray? mGridCelFog;
        public Zombie? AddZombieAtCell(ZombieType type,int col,int row) {
            if(FailAllocation)return null;
            var z=new Zombie {mBoard=this,mZombieType=type,mRow=row,X=col*80};m_zombies.Add(z);return z;
        }
        public void CanPlantAt() {}
        public Plant? AddPlant(int col,int row,SeedType type,SeedType marker) {
            if(FailAllocation) return null;
            var plant=new Plant {mBoard=this,mPlantCol=col,mRow=row,mSeedType=type,mImitaterType=marker};m_plants.Add(plant);return plant;
        }
        public int RandomValue; public int Rand(int range) => Math.Clamp(RandomValue, 0, range - 1);
        public Projectile? AddProjectile(float x,float y,int order,int row,ProjectileType type) {
            if (FailAllocation) return null;
            var shot = new Projectile { mBoard=this,mPosX=x,mPosY=y,mRenderOrder=order,mRow=row,mProjectileType=type };
            m_projectiles.Add(shot); return shot;
        }
    }
    public class FogArray { public IntPtr Pointer; public T Cast<T>() where T:new()=>new(); }
    public enum CursorType { Normal, Shovel }
    public class Cursor { public CursorType CursorType; }
    public class Plant {
        private static uint next = 100; public uint DataID = ++next; public IntPtr Pointer => (IntPtr)DataID;
        public Board mBoard = null!; public SeedType mSeedType,mImitaterType;
        public int mPlantHealth=300,mPlantMaxHealth=300,mDoSpecialCountdown,mStartRow,mShootingCounter,mTargetX,mTargetY,mLaunchCounter,mLaunchRate;
        public int mPlantCol,mRow,mWidth=80,mHeight=80,mRenderOrder,mEatenFlashCountdown;
        public float mX,mY; public bool mDead,mIsAsleep,mIsOnBoard=true,Carried;
        public Rect GetPlantAttackRect(PlantWeapon weapon) => new(mX - 20,mY,mX + 100,mY + 80);
        public bool NotOnGround() => Carried; public void Die() => mDead=true;
        public void MouseDown() {} public void Squish() {} public void SpikeRockTakeDamage() {} public void PlantInitialize() {} public void UpdateAbilities() {} public void StarFruitFire() {} public void UpdateShooter() {} public void UpdateShooting() {} public void Fire() {} public void DoSpecial() {}
    }
    public class Projectile {
        private static uint next = 1000; public uint DataID=++next; public IntPtr Pointer => (IntPtr)DataID;
        public Board mBoard=null!; public ProjectileType mProjectileType; public ProjectileMotion mMotionType;
        public float mCobTargetX,mVelX,mVelY,mVelZ,mAccZ,mPosX,mPosY,mPosZ,mShadowY;
        public int mCobTargetRow,mProjectileAge,mRow,mRenderOrder,mHitTorchwoodGridX,Bonus;
        public ZombieID mTargetZombieID; public DamageRangeFlags mDamageRangeFlags; public bool mDead;
        public void FindCollisionTargetPlant() {} public void CheckForCollision() {} public void UpdateLobMotion() {}
        public void Die() => mDead=true; public void UpdateNormalMotion() {} public void DoImpact() {} public void Update() {}
        public void ConvertToFireball() {} public void ConvertToPea() {} public DamageFlags GetDamageFlags(Zombie zombie) => 0;
    }
    public class Zombie {
        public int Shrinks;
        private static uint next = 2000; public HelmType mHelmType; public ShieldType mShieldType; public int mHelmHealth, mShieldHealth; public uint DataID=++next; public IntPtr Pointer => (IntPtr)DataID;
        public Board mBoard=null!; public ZombieType mZombieType;
        public bool mDead,mMindControlled,mYuckyFace,Dying,Chilled,ColdRemoved,Immune,Flying; public bool IsFlying() => Flying; public Action? OnDamage;
        public int mRow,mIceTrapCounter,mYuckyFaceCounter,mTargetRow,Damage,Poison,Frost,Embers;
        public float X=200,Y=40; public Rect GetZombieRect() => new(X,Y,X+40,Y+80);
        public bool IsDeadOrDying() => Dying; public void StopEating() {} public void UpdateAnimSpeed() {} public void EatPlant() {} public void CanTargetPlant() {}
        public void ApplyChill(bool ice) => Chilled=true; public void RemoveColdEffects() { ColdRemoved=true; mIceTrapCounter=0; }
        public void LandFlyer(DamageFlags flags) { Flying=false; }
        public void TakeDamage(int damage,DamageFlags flags) { Damage+=damage; OnDamage?.Invoke(); }
    }
}
namespace FusionMod {
    using Il2CppReloaded.Gameplay;
    internal static class HypnoPlants { internal static bool MindControl(Zombie z) { if(z.mDead||z.Dying||z.mMindControlled)return false;z.mMindControlled=true;return true; } }
    internal static class IceInstantFusions { internal static void Freeze(Zombie z,int ticks) { if(!z.Immune&&!z.mDead&&!z.Dying)z.mIceTrapCounter=Math.Max(z.mIceTrapCounter,ticks); } }
    internal static class PotFusions { internal static void FadeFog(IntPtr array,int cols,int rows,int col,int row,int radius=0) { } }
    internal static class PuffShooters { internal const float FlightHeight = 40; }
    internal static class PlayerPlantingScope { internal static IntPtr ConsumingIngredient = IntPtr.Zero; internal static object? Current; internal static Il2CppReloaded.Gameplay.Board? ActiveBoard = null; internal static void Consume(Plant plant) => plant.Die(); }
    internal static class FusionService {
        internal static bool CreatingBatch = false,CheckingNativePlacement = false;
        internal static PlantingReason CheckNativePlacement(Board board,int col,int row,SeedType type) => PlantingReason.Ok;
        internal record Plan(Il2CppReloaded.Gameplay.Plant Target,FusionPlant Result);
        internal static List<Plan> ResolveAll(Il2CppReloaded.Gameplay.Board board,FusionPlant incoming,int col,int row) {
            var p=board.GetPlantsOnLawn(col,row).NormalPlant;
            return p!=null&&FusionRecipes.TryGet(FusionPlant.From(p),incoming,out var result)?new(){new(p,result)}:new();
        }
        internal static Il2CppReloaded.Gameplay.PlantingReason CheckFusionPlacement(Il2CppReloaded.Gameplay.Board b,Il2CppReloaded.Gameplay.Plant p,FusionPlant result) => Il2CppReloaded.Gameplay.PlantingReason.Ok;
        internal static void CompleteBatch(Il2CppReloaded.Gameplay.Board board,FusionPlant incoming,List<Plan> plans) {
            if(board.FailAllocation)throw new InvalidOperationException("allocation");
            foreach(var plan in plans) plan.Target.Die();
        }
    }

    internal static class ChomperFusions { internal static bool Active(Plant p) => !p.mDead&&!p.Carried&&p.mIsOnBoard&&!p.mBoard.mPaused; }
    internal static class CherryBombPlants {
        internal sealed record Death(Board Board,FusionPlant Identity,uint Id,int Col,int Row,float X,float Y,int Order);
        internal static bool CanTriggerDeath(Plant p) => !p.mDead&&p.mIsOnBoard;
    }
    internal static class SquashFusions {
        internal sealed record Landing(Board Board,uint Id,FusionPlant Identity,int Col,int Row,float X,float Y,int Order);
        internal static void ElementalBlast(Landing hit,bool ice) => ++hit.Board.RowBlasts;
        internal static bool Eligible(Board board,Zombie z,DamageRangeFlags flags) => z.mBoard==board&&!z.mDead&&!z.Dying&&!z.mMindControlled && (flags & (z.Flying ? DamageRangeFlags.Flying : DamageRangeFlags.Ground)) != 0;
    }
    internal static class SupportFusions { internal static bool CanRedirect(Plant p,Zombie z) => z.mBoard==p.mBoard&&!z.mDead&&!z.mMindControlled&&!z.mYuckyFace; }
    internal static class PotBoosts { internal static bool HasSupport(Plant p,FusionPlant identity) => p.mTargetY==1; }
    internal static class WeedFusions {
        internal sealed record Attack(Board Board,uint PlantId,FusionPlant Identity);
        internal sealed record Crush(Board Board) { internal Zombie Zombie = new(); internal uint ZombieId; }
        internal static Attack? CurrentAttack = null;
        internal static Crush? CurrentCrush = null;
    }
    internal static class PotDefense { internal const int GarlicDirection = -1130; internal static int Damage(Plant p,int damage) => damage; }
    internal static class FumeProjectiles { internal static float Speed(Projectile p) => 3.33f; internal static int Damage(Projectile p,int damage) => damage+p.Bonus;
        internal static int ModifiedDamage(Projectile p,int damage) => Damage(p,damage); }
    internal static class BloverEffects { internal static bool Reserve(Zombie z,bool cryo) => !z.mBoard.ReserveFailure; }
    internal static class SnowPea { internal static void Hit(Zombie z,int points) { if(!z.Immune&&z.mIceTrapCounter==0) { z.Frost+=points; if(z.Frost>=100) z.mIceTrapCounter=400; } } }
    internal static class PoisonShooters { internal static void PoisonBite(Board board,Zombie zombie) => ++zombie.Poison; internal static int Level(Zombie z) => z.Poison; internal static void AddLevel(Board board,Zombie z) => ++z.Poison; }
    internal static class ImportedPeaVisuals { internal static void ForgetProjectile(Projectile p) {} internal static void FumeSmoke(Plant p,FusionPlant identity,int? row=null,float offset=0,float degrees=0,string? profileKey=null) {} internal static void EnsureCactusProjectile(Projectile p) {} internal static void EnsureStarProjectile(Projectile p) {} internal static void EnsureSeaProjectile(Projectile p) {} }
}

namespace Il2CppSystem { public class Array { public int GetLength(int dimension) => dimension==0?12:7; } }

namespace FusionMod { internal static class CherryShooter { internal static bool IsExplosive(Il2CppReloaded.Gameplay.Projectile p) => p.mCobTargetX == -41010; } }

namespace LawnGlove { public static class LawnGloveMod { public static Il2CppReloaded.Gameplay.Plant? GetCarriedPlant(Il2CppReloaded.Gameplay.Board board) => board.m_plants.FirstOrDefault(p=>p.Carried); } }

namespace FusionMod {
 internal static class PuffLobbers { internal static void Shrink(Il2CppReloaded.Gameplay.Zombie zombie) { zombie.Shrinks++; } }
}
namespace PlantLibrary {
 using Il2CppReloaded.Gameplay;
 public sealed record ProjectileImpact(Board Board,float X,float Y,int Row,int RenderOrder,ProjectileType NativeType) {
  public static ProjectileImpact Capture(Projectile shot)=>new(shot.mBoard,shot.mPosX,shot.mPosY,shot.mRow,shot.mRenderOrder,shot.mProjectileType);
 }
 public static class Projectiles {
  public static ProjectileImpact? LastImpact;
  public static void EmitImpactEffect(string id,ProjectileImpact impact) { LastImpact=impact;impact.Board.mApp.AddTodParticle(impact.X,impact.Y,impact.RenderOrder,ParticleEffect.PeaSplat); }
  public static void PlaySound(string id,Board board) {}
 }
}
