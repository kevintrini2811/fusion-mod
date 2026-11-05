using Il2CppReloaded.Services;
// Offline doubles: no game assembly, Unity or IL2CPP is loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HarmonyPatch : Attribute
    { public HarmonyPatch() {} public HarmonyPatch(Type type, string method) {} }
    [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int priority) {} }
    public static class Priority { public const int First=800, Last=0; }
    public static class AccessTools
    { public static System.Reflection.MethodInfo Method(Type type, string name) => type.GetMethod(name)!; }
}
namespace Il2CppReloaded.Gameplay
{

    public enum PlantState { Notready, PotatoRising, PotatoArmed, PotatoMashed, Ready, ChomperBiting, ChomperDigesting, ChomperSwallowing, UmbrellaReflecting, TanglekelpGrabbing }
    public enum PlantOnBungeeState { NotOnBungee, OnBungee }
    public enum GameScenes { Playing, Loading }
    public enum GameMode { Adventure, ChallengeZenGarden, PuzzleIZombie1=20, PuzzleIZombieEndless=30 }
    public enum CursorType { Normal, PlantFromBank }
    public enum PlantingReason { Ok, NotHere }
    public sealed class Layers { public Plant? NormalPlant; }
    public sealed class Cursor { public CursorType CursorType; }
    public enum PlantWeapon { Primary }
    public enum ProjectileType { Pea, PeashooterPea, ZombiePea, Basketball }
    public enum ProjectileMotion { Straight, Threepeater }
    [Flags] public enum DamageRangeFlags { Flying=16, OffGround=32, Ground=1, Submerged=2, Dog=4, Underground=8 }
    [Flags] public enum DamageFlags { BypassesShield=1, HitsShieldAndBody=2, DoesntLeaveBody=16 }
    public enum ZombieType { Balloon, Normal, Boss, Zamboni, Catapult, Gargantuar, RedeyeGargantuar, Gravestone, Target }
    public enum ReanimLoopType { Loop }
    public enum CoinType { Sun, Silver }
    public enum CoinMotion { FromPlant, Coin }
    public enum ParticleEffect { IceSparkle, Powie, PotatoMine, PeaSplat }
    public sealed class App { public SoundSystem SoundSystem=new(); public void AddTodParticle(float x,float y,int order,ParticleEffect effect) {} public GameScenes GameScene; public GameMode GameMode; }
    public readonly record struct Rect(float xMin, float xMax, float yMin, float yMax);
    public sealed class Board
    {
        public IntPtr Pointer = (IntPtr)1;
        public bool mPaused;
        public readonly List<Plant> m_plants = new();
        public List<Projectile> m_projectiles => Projectiles;
        public Cursor mCursorObject = new();
        public Layers Layers = new();
        public Layers GetPlantsOnLawn(int col, int row) => Layers.NormalPlant != null ? Layers
            : new Layers { NormalPlant = m_plants.FirstOrDefault(p => !p.mDead && p.mPlantCol == col && p.mRow == row) };
        public HashSet<(int Col, int Row)> BlockedCells = new();
        public bool SawPlayerContext, FailSpawn;
        public int GetNumRows() => 5;
        public int PixelToGridX(int x, int y) => x / 80;
        public int PixelToGridY(int x, int y) => y / 80;
        public void MouseDown() {}
        public bool CanTakeSunMoney(int amount, int player) => true;
        public bool TakeSunMoney(int amount, int player) => true;
        public Plant AddPlant(int col,int row,SeedType type,SeedType marker) {
            SawPlayerContext |= FusionMod.PlayerPlantingScope.Current != null;
            if (FailSpawn) throw new InvalidOperationException("spawn failure");
            var plant = new Plant {mBoard=this,mSeedType=type,mImitaterType=marker,mPlantCol=col,mRow=row};
            m_plants.Add(plant); return plant;
        }
        public int mMainCounter = 1, SunMoney, Coins, Particles;
        public App mApp=>App;
        public int Rand(int max)=>0;
        public void AddSunMoney(int amount,int playerIndex)=>SunMoney+=amount;
        public int AddCoin(float x,float y,CoinType type,CoinMotion motion)=>++Coins;
        public readonly List<Zombie> m_zombies = new();
        public readonly List<Projectile> Projectiles = new();
        public App App = new();
        public Func<float,float> Ground = _ => 0;
        public float GetPosYBasedOnRow(float x, int row) => Ground(x);
        public Projectile AddProjectile(float x, float y, int order, int row, ProjectileType type)
        {
            var p = new Projectile { mBoard=this, mPosX=x, mPosY=y, mRow=row,
                mShadowY = GridToPixelY(0, row) + 67, Pointer=(IntPtr)(Projectiles.Count+10), DataID=(uint)(Projectiles.Count+10) };
            Projectiles.Add(p); return p;
        }
        public void UpdateGame() { }
        public Plant? FindUmbrellaPlant(int col, int row) => null;
        public int GridToPixelX(int col, int row) => col * 80;
        public int GridToPixelY(int col, int row) => row * 80;
        public void DisposeBoard() { }
        public void KillAllPlantsInRadius(float x, float y, int radius) { }
    }
    public sealed class Plant
    {
        public Il2CppSource.Controllers.PlantController mController = new();
        public SeedType mSeedType = SeedType.Puffshroom, mImitaterType = SeedType.None;
        public PlantState mState;
        public PlantOnBungeeState mOnBungeeState;
        public bool mDead, mSquished, OffGround, mIsAsleep, mIsOnBoard=true;
        public Board mBoard = new();
        public App mApp => mBoard.App;
        public IntPtr Pointer=(IntPtr)3;
        public uint DataID=3;
        public int mPlantHealth=300, mPlantMaxHealth=300, mLaunchCounter, mLaunchRate, mStateCountdown, mShootingCounter;
        public int mPlantCol, mStartRow, mTargetX, mTargetY;
        public int mX, mY, mHeight=80, mWidth=80, mRow, mRenderOrder;
        public uint mTargetZombieID;
        public Zombie? Target;
        public int GrabUpdates;
        public void UpdateTanglekelp() => ++GrabUpdates;
        public Zombie? FindTargetZombie(int row, PlantWeapon weapon) => Target;
        public bool NotOnGround() => OffGround;
        public void Die() {
            mDead=true;
            if (FusionMod.FusionPlant.IsBurstMine(FusionMod.FusionPlant.From(this)) || FusionMod.FusionPlant.From(this) == FusionMod.FusionPlant.LandMine)
                FusionMod.MineFusions.OnDeath(new(mBoard,FusionMod.FusionPlant.From(this),DataID,mPlantCol,mRow,mX+40,mY+40,0,mStartRow));
        }
        public void PlantInitialize() { }
        public void UpdateProductionPlant() { }
        public void PlayBodyReanim(string track, ReanimLoopType loop, int blend, float rate) {}
        public void UpdatePotato() { if (mState == PlantState.PotatoRising) mState = PlantState.PotatoArmed; }
        public void UpdateAbilities() { }
        public void Squish() { }
        public void Fire() { }
        public void UpdateShooting() { }
    }
    public sealed class Projectile
    {
        public IntPtr Pointer;
        public uint DataID;
        public Board mBoard=null!;
        public bool mDead;
        public int mRow,mProjectileAge,mRenderOrder;
        public float mCobTargetX,mPosX,mPosY,mPosZ,mVelX,mVelY,mVelZ,mAccZ,mShadowY,mRotation;
        public ProjectileType mProjectileType;
        public ProjectileMotion mMotionType;
        public DamageRangeFlags mDamageRangeFlags;
        public void Die() => mDead=true;
        public void Reset() { mCobTargetX = 0; mDead = false; }
        public void UpdateNormalMotion()
        {
            mPosX += 3.33f;
            mPosY += mVelY;
            if (mMotionType == ProjectileMotion.Threepeater)
            {
                mVelY *= .97f;
                mShadowY += mVelY;
            }
            if (mShadowY - mPosY < 28) mDead = true;
        }
        public void UpdateLobMotion() { }
        public Plant? FindCollisionTargetPlant() => null;
        public void CheckForCollision() {} public void ConvertToFireball() {}
    }
    public sealed class Zombie
    { public bool mIsEating; public void UpdateZombiePosition() {}
        public int mButteredCounter;
        public bool ButterImmune;
        public void ApplyButter() { if (!ButterImmune) mButteredCounter = 400; }

        public IntPtr Pointer = (IntPtr)7;
        public int mIceTrapCounter;
        public bool FailControl, FreezeImmune;
        public void StartMindControlled() { if (!FailControl) mMindControlled = true; }
        public bool CanBeFrozen() => !FreezeImmune;
        public void HitIceTrap() { if (!FreezeImmune) mIceTrapCounter = 400; }
        public uint DataID;
        public bool mDead,mMindControlled,Eligible=true;
        public bool Flying, mYuckyFace;
        public int mYuckyFaceCounter;
        public void StopEating() { }
        public bool IsFlying() => Flying;
        public void LandFlyer(DamageFlags flags) => Flying = false;
        public int mRow,mBodyHealth=2000,mHelmHealth,mShieldHealth,mX,Hits,Damage;
        public float mPosX,mPosY; public Board mBoard=null!;
        public ZombieType mZombieType;
        // Native EatPlant: 0x6f0fd8..0x6f0fe5 rejects armed Potato Mines;
        // 0x6f10d6 subtracts four health after that guard.
        public void EatPlant(Plant plant)
        {
            if (plant.mSeedType==SeedType.Potatomine && plant.mState!=PlantState.Notready) return;
            plant.mPlantHealth-=4;
            if (plant.mPlantHealth<=0) plant.Die();
        }
        public bool IsDeadOrDying() => mBodyHealth<=0;
        public bool EffectedByDamage(DamageRangeFlags flags) => Eligible;
        public Rect GetZombieRect() => new(mPosX,mPosX+30,mPosY,mPosY+80);
        public void DieWithLoot() { mDead = true; }
        public void TakeDamage(int amount, DamageFlags flags) { ++Hits; Damage+=amount; mBodyHealth-=amount; }
    }
}
namespace FusionMod
{
    internal static class ChomperFusions { internal static Il2CppReloaded.Gameplay.Board? PlantExplosionBoard=null;
        internal static bool Active(Il2CppReloaded.Gameplay.Plant p) => MineFusions.Active(p); }
    internal static class SolarShooter { internal static int CustomSun; internal static void DropSun(Il2CppReloaded.Gameplay.Board b,float x,float y,int value) => CustomSun += value; }
    internal static class FusionService { internal static Il2CppReloaded.Gameplay.PlantingReason CheckNativePlacement(Il2CppReloaded.Gameplay.Board b,int c,int r,Il2CppReloaded.Gameplay.SeedType t) => b.BlockedCells.Contains((c,r)) ? Il2CppReloaded.Gameplay.PlantingReason.NotHere : Il2CppReloaded.Gameplay.PlantingReason.Ok; }
    internal static class SnowPea { internal static IntPtr CreatingIceTrap; }
    internal static class CherryBombPlants {
        internal static IntPtr DisposingBoard=IntPtr.Zero;
        internal static bool CanTriggerDeath(Il2CppReloaded.Gameplay.Plant plant) => MineFusions.Active(plant) && PlayerPlantingScope.ConsumingIngredient != plant.Pointer;
        internal sealed record Death(Il2CppReloaded.Gameplay.Board Board,FusionPlant Identity,uint Id,int Col,int Row,float X,float Y,int Order,int Stage=0); }
    internal static class PlayerPlantingScope { internal static IntPtr ConsumingIngredient=IntPtr.Zero; internal static object? Current=null; }
    internal static class ImportedPeaVisuals
    {
        internal static (float X,float Y) PeatatoMuzzle(Il2CppReloaded.Gameplay.Plant p) => (p.mX+p.mWidth/2f,p.mY+p.mHeight-25);
        internal static void EnsureRollingProjectile(Il2CppReloaded.Gameplay.Projectile shot, bool nut) { }
        internal static void EnsurePotatoProjectile(Il2CppReloaded.Gameplay.Projectile p) {}
        internal static void RegisterPotatoProjectile(Il2CppReloaded.Gameplay.Projectile p) {}
        internal static void RegisterSolarNutBall(Il2CppReloaded.Gameplay.Projectile p) {}
        internal static void RegisterRollingProjectile(Il2CppReloaded.Gameplay.Projectile p,bool nut) { }
        internal static void ForgetProjectile(Il2CppReloaded.Gameplay.Projectile p) { }
    }
}

namespace Il2CppReloaded.Services { public enum FoleyType { Explosion } public sealed class SoundSystem { public void PlayFoley(FoleyType type) {} } }
namespace Il2CppSource.Controllers {
    public enum CharacterAnimationTrack { Body }
    public sealed class PlantController { public void SetLoopCount(int count, CharacterAnimationTrack track) {} }
}

namespace FusionMod { internal static class SquashFusions { internal static bool Eligible(Il2CppReloaded.Gameplay.Board board, Il2CppReloaded.Gameplay.Zombie zombie, Il2CppReloaded.Gameplay.DamageRangeFlags flags) => zombie.mBoard.Pointer == board.Pointer && !zombie.mDead && !zombie.IsDeadOrDying() && !zombie.mMindControlled && zombie.EffectedByDamage(flags); } }

namespace FusionMod
{
    internal static class DoomEffects
    {
        internal static void AshDamage(Il2CppReloaded.Gameplay.Zombie zombie, int damage) =>
            zombie.TakeDamage(damage, Il2CppReloaded.Gameplay.DamageFlags.HitsShieldAndBody
                | Il2CppReloaded.Gameplay.DamageFlags.DoesntLeaveBody);
    }
}

namespace FusionMod { internal static class TreasureMines { internal static bool Owns(Il2CppReloaded.Gameplay.Plant p) => FusionPlant.From(p) == FusionPlant.MarigoldMine; } }

namespace FusionMod
{
    // Conversion is covered by native projectile matrix tests. Mine tests can
    // simulate an owner transfer without importing unrelated plant families.
    internal static class SupportTorchFusions
    {
        internal static Action<Il2CppReloaded.Gameplay.Projectile>? OnConvert;
        internal static void Convert(Il2CppReloaded.Gameplay.Projectile shot) => OnConvert?.Invoke(shot);
    }
}
