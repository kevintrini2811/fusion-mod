// Offline doubles only; no game assemblies are loaded.
using System.Reflection;
namespace HarmonyLib {
 [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch:Attribute { public HarmonyPatch(){} public HarmonyPatch(Type t,string n){} }
 [AttributeUsage(AttributeTargets.Method)] public sealed class HarmonyPriority:Attribute { public HarmonyPriority(int n){} }
 public static class Priority { public const int First=800,Last=0; }
 public static class AccessTools { public static MethodInfo Method(Type t,string n)=>t.GetMethod(n)!; }
}
namespace UnityEngine {
 public struct Vector3 { public float x,y,z; public static Vector3 zero=>new(); }
 public struct Vector2 { public float x,y; public Vector2(float a,float b){x=a;y=b;} }
 public struct Color { public Color(float r,float g,float b,float a){} public static Color white=>new(); }
}
namespace Il2CppSource.Controllers {
 public enum CharacterAnimationTrack { Body }
 public class Controller {
  public UnityEngine.Vector3 VisualOffset; public bool DieAfterPlay;
  public int Loops; public float Rate=12, Size=1; public UnityEngine.Vector2 Scale=>new(Size,Size); public string Clip="";
  public void SetAnimationTimeOnTrack(float n,CharacterAnimationTrack t){}
  public void SetColorOverride(UnityEngine.Color c,CharacterAnimationTrack t){}
  public int GetLoopCount(CharacterAnimationTrack t)=>Loops;
  public void OverrideScale(float x,float y)=>Size=x;
  public bool IsAnimationPlayingOnTrack(string c,CharacterAnimationTrack t)=>Clip==c;
  public void PlayAnimationOnTrack(string c,CharacterAnimationTrack t,float r,Il2CppReloaded.Gameplay.ReanimLoopType l){Clip=c;Rate=r;}
  public void SetAnimationRateOnTrack(float r,CharacterAnimationTrack t)=>Rate=r;
  public float GetGroundVelocity()=>4*Size*Rate/12;
  public static implicit operator bool(Controller c)=>c!=null;
 }
}
namespace Il2CppReloaded.Services { public class SerializationService { public void Serialize(){} } }
namespace Il2CppReloaded.TreeStateActivities { public class GameplayActivity { public void CreatePlantController(){} } }
namespace LawnGlove { public static class LawnGloveMod {
 public static Il2CppReloaded.Gameplay.Plant? Carried;
 public static Il2CppReloaded.Gameplay.Plant? GetCarriedPlant(Il2CppReloaded.Gameplay.Board b)=>Carried;
} }
namespace Il2CppReloaded.Gameplay {


 public enum GameScenes { Playing, Loading }
 public enum GameMode { Adventure, ChallengeZenGarden, PuzzleIZombie1=50,PuzzleIZombieEndless=60 }
 public enum CursorType { Normal, Fertilizer }
 public enum PlantState { Ready,Notready,PotatoRising,PotatoArmed,SunshroomSmall,SunshroomBig }
 public enum PlantOnBungeeState { NotOnBungee, OnBungee }
 public enum PlantPriority { DiggingOrder }
 public enum PlantingReason { Ok, NotHere }
 public enum ReanimationType { ZengardenFertilizer }
 public enum ReanimLoopType { PlayOnceAndHold, Loop }
 public enum GridItemType { ZenTool }
 public enum GridItemState { ZenToolFertilizer }
 [Flags] public enum DamageRangeFlags { Ground=1,Flying=2,Submerged=4,Dog=8,OffGround=16,Underground=32 }
 [Flags] public enum DamageFlags { HitsShieldAndBody=2,DoesntLeaveBody=16 }
 public enum FoleyType { BowlingImpact,JalapenoIgnite }
 public class Rect { public float xMin,xMax; public bool Overlaps(Rect r)=>false; }
 public class Sound { public void PlayFoley(FoleyType f){} }
 public class Cursor { public CursorType CursorType; }
 public partial class App {
  public GameScenes GameScene=GameScenes.Playing; public GameMode GameMode;
  public Sound SoundSystem=new(); public List<Reanimation> Effects=new(); public bool FailEffect;
  public Reanimation? AddReanimation(float x,float y,int order,ReanimationType type){if(FailEffect)return null;var r=new Reanimation();Effects.Add(r);return r;}
  public int ReanimationGetID(Reanimation r)=>Effects.IndexOf(r)+1;
  public Reanimation? ReanimationTryToGet(int id)=>id>0&&id<=Effects.Count?Effects[id-1]:null;
 }
 public class Reanimation {
  public bool mDead; public float mAnimRate; public ReanimLoopType mLoopType;
  public Il2CppSource.Controllers.Controller mController=new();
  public void SetPosition(float x,float y){}
  public void ReanimationDie()=>mDead=true;
  public void OverrideScale(float x,float y)=>mController.OverrideScale(x,y);
 }
 public class Layers { public Plant? NormalPlant,UnderPlant,PumpkinPlant,FlyingPlant; }
 public partial class Board {
  static long next; public IntPtr Pointer=(IntPtr)(++next); public bool mPaused;
  public App mApp=new(); public Cursor mCursorObject=new(); public List<Plant> m_plants=new(); public List<Zombie> m_zombies=new();
  public GridPool m_gridItems; public Board(){m_gridItems=new(this);}
  public Layers GetPlantsOnLawn(int col,int row){var p=m_plants.Where(p=>!p.mDead&&p.mPlantCol==col&&p.mRow==row);return new(){
   NormalPlant=p.FirstOrDefault(p=>p.mSeedType is not (SeedType.Pumpkinshell or SeedType.Lilypad or SeedType.Flowerpot)),
   UnderPlant=p.FirstOrDefault(p=>p.mSeedType is SeedType.Lilypad or SeedType.Flowerpot),PumpkinPlant=p.FirstOrDefault(p=>p.mSeedType==SeedType.Pumpkinshell)};}
  public Plant? GetTopPlantAt(int c,int r,PlantPriority p)=>GetPlantsOnLawn(c,r).PumpkinPlant??GetPlantsOnLawn(c,r).NormalPlant??GetPlantsOnLawn(c,r).UnderPlant;
  public Plant? AddPlant(int c,int r,SeedType seed,SeedType marker){var p=new Plant {mBoard=this,mSeedType=seed,mImitaterType=marker,mPlantCol=c,mRow=r};m_plants.Add(p);return p;}
  public int PixelToGridX(int x,int y)=>x/80; public int PixelToGridY(int x,int y)=>y/80; public int GetNumRows()=>5;
  public void ClearCursor(bool ignoreControlType=false,int playerIndex=0)=>mCursorObject.CursorType=CursorType.Normal;
  public void UpdateGame(){} public void MouseDown(){} public void DisposeBoard(){}
 }
 public partial class Plant {
  static uint next; public uint DataID=++next; public IntPtr Pointer= (IntPtr)next;
  public Board mBoard=null!; public App mApp=>mBoard.mApp;
  public SeedType mSeedType = SeedType.Wintermelon,mImitaterType=SeedType.None;
  public bool mDead,mSquished,mIsOnBoard=true; public bool Airborne;
  public PlantOnBungeeState mOnBungeeState; public int mPlantCol,mRow,mRenderOrder,mWidth=80,mHeight=80;
  public float mX,mY; public int mPlantHealth=10,mPlantMaxHealth=300;
  public PlantState mState; public int mStateCountdown,mTargetX,mTargetY,mStartRow,mShootingCounter,mLaunchCounter;
  public Il2CppSource.Controllers.Controller mController=new();
  public bool NotOnGround()=>Airborne;
  public void UpdateReanim(){} public void UpdateReanimColor(){}
  public void UpdateSunShroom(){if(mStateCountdown==0)mState=PlantState.SunshroomBig;}
  public Rect GetPlantRect()=>new(); public void Die()=>mDead=true; public void Reset(){}
  public void UpdateAbilities(){}
 }
 public partial class Zombie { public uint DataID; public bool mDead; public int mRow; public bool IsDeadOrDying()=>mDead;public bool EffectedByDamage(DamageRangeFlags f)=>true;public Rect GetZombieRect()=>new();public void TakeDamage(int n,DamageFlags f){Damage+=n;OnDamage?.Invoke();} }
 public class GridPool {
  readonly Board board; public List<GridItem> Items=new();public int FailAfter=int.MaxValue;
  public GridPool(Board b)=>board=b;
  public GridItem? DataArrayAlloc(){if(Items.Count>=FailAfter)return null;var t=new GridItem{mBoard=board};Items.Add(t);return t;}
 }
 public class GridItem {
  public Board mBoard=null!;public GridItemType mGridItemType; public GridItemState mGridItemState;
  public int mGridX,mGridY,mGridItemCounter,mGridItemReanimID,mX,mY,mRenderOrder;public float mPosX,mPosY;public SeedType mSeedType = SeedType.Wintermelon;public bool mDead;
  public void Init(App a,GridItemType t)=>mGridItemType=t;public void GridItemDie()=>mDead=true;public void Update(){}
 }
}
namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class MineFusions { public static void GrowImmediately(Plant p)=>p.mState=PlantState.PotatoArmed; }
 internal static class FusionService { public static PlantingReason CheckFusionPlacement(Board b,Plant p,FusionPlant f)=>PlantingReason.Ok; }
 internal static class PlayerPlantingScope { public static IntPtr ConsumingIngredient; public static void Consume(Plant p)=>p.Die(); }
 public class Log { public void Error(string s)=>throw new Exception(s); }
 public class FusionMod { public static FusionMod Instance=new(); public Log LoggerInstance=new(); }
}
