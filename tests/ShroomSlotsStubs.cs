// Offline doubles. No game, Unity or IL2CPP code is loaded.
using System.Reflection;
namespace HarmonyLib {
 [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch:Attribute { public HarmonyPatch(){} public HarmonyPatch(Type t,string n){} }
 public static class AccessTools {
  public static MethodInfo Method(string name) { var a=name.Split(':'); return typeof(AccessTools).Assembly.GetType(a[0])!.GetMethod(a[1],BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.Instance)!; }
  public static FieldInfo Field(string name) { var a=name.Split(':'); return typeof(AccessTools).Assembly.GetType(a[0])!.GetField(a[1],BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)!; }
 }
}
namespace Il2CppSource.Controllers { public class PreviewController { public int Updates; public void UpdatePosition()=>++Updates; } }
namespace Il2CppReloaded.Gameplay {

 public enum PlantState { Notready,DoingSpecial }
 public enum CursorType { Normal,Glove,PlantFromBank,Shovel }
 public enum GameScenes { Playing,Loading } public enum GameMode { Adventure,ChallengeZenGarden }
 public enum PlantingReason { Ok,NotHere,NotOnWater,NeedsSleeping }
 public enum PlantOnBungeeState { NotOnBungee,OnBungee }
 public class WidgetManager { public int[] LastMouseX=[0],LastMouseY=[0]; }
 public class App { public WidgetManager WidgetManager=new(); public GameScenes GameScene=GameScenes.Playing; public GameMode GameMode; }
 public class Cursor { public CursorType CursorType; public SeedType mType = SeedType.Puffshroom,mImitaterType=SeedType.None; }
 public class ReloadedObject { public Board mBoard=null!; public float mX,mY; public bool mVisible=true; }
 public class CursorPreview:ReloadedObject { public int PlayerIndex,mGridX,mGridY; public Il2CppSource.Controllers.PreviewController mController=new(); public void Update(){} }
 public class Lawn { public Plant? NormalPlant,UnderPlant,PumpkinPlant,FlyingPlant; }
 public class Board {
        public bool Water; public bool IsPoolSquare(int col, int row) => Water;
  public IntPtr Pointer=(IntPtr)1; public App mApp=new(); public bool mPaused;
  public int mX,mY; public Cursor mCursorObject=new(); public Cursor[] CursorObjects; public List<Plant> m_plants=new();
  public int Attempts,FailAt,Sun; public bool ThrowTerrain; public PlantingReason Terrain=PlantingReason.Ok;
  public Func<SeedType,PlantingReason>? TerrainForSeed;
  public Board(){CursorObjects=[mCursorObject];}
  public int GetNumRows()=>5;
  public int GridToPixelX(int c,int r)=>c*80+40; public int GridToPixelY(int c,int r)=>r*100+80;
  public int PixelToGridX(int x,int y)=>(x-40)/80; public int PixelToGridY(int x,int y)=>(y-80)/100;
  public int PlantingPixelToGridX(int x,int y,SeedType t)=>PixelToGridX(x,y);
  public int PlantingPixelToGridY(int x,int y,SeedType t)=>PixelToGridY(x,y);
  public Lawn GetPlantsOnLawn(int c,int r)=>new(){NormalPlant=m_plants.FirstOrDefault(p=>p.mPlantCol==c&&p.mRow==r&&!p.mDead&&p.mIsOnBoard&&!p.mSquished&&!p.Support&&p.mSeedType!=SeedType.Flowerpot&&p.mSeedType!=SeedType.Pumpkinshell),PumpkinPlant=m_plants.FirstOrDefault(p=>p.mPlantCol==c&&p.mRow==r&&!p.mDead&&p.mIsOnBoard&&!p.mSquished&&p.mSeedType==SeedType.Pumpkinshell),UnderPlant=m_plants.FirstOrDefault(p=>p.mPlantCol==c&&p.mRow==r&&!p.mDead&&p.mIsOnBoard&&!p.mSquished&&p.mSeedType==SeedType.Flowerpot)};
  public PlantingReason CanPlantAt(int c,int r,SeedType t){
   if(ThrowTerrain)throw new InvalidOperationException("terrain");
   var lawn=GetPlantsOnLawn(c,r);
   var nativeTerrain = TerrainForSeed?.Invoke(t) ?? Terrain;
   if(t==SeedType.Cobcannon) return nativeTerrain!=PlantingReason.Ok ? nativeTerrain
    : c<8 && lawn.NormalPlant?.mSeedType==SeedType.Kernelpult
      && GetPlantsOnLawn(c+1,r).NormalPlant?.mSeedType==SeedType.Kernelpult ? PlantingReason.Ok : PlantingReason.NotHere;
   var reason=nativeTerrain!=PlantingReason.Ok?nativeTerrain:t==SeedType.Flowerpot&&lawn.UnderPlant!=null?PlantingReason.NotHere:lawn.NormalPlant!=null?PlantingReason.NotHere:PlantingReason.Ok;
   object?[] args=[this,c,r,t,reason]; Invoke(typeof(FusionMod.FusionPlacementPatch),"Postfix",args);return (PlantingReason)args[4]!;
  }
  public Plant? AddPlant(int c,int r,SeedType t,SeedType m){
   object?[] args=[this,c,r,t,m,null]; Invoke(typeof(FusionMod.FusionPlantCreationPatch),"Prefix",args);
   ++Attempts;if(FailAt==Attempts)return null;
   var plant=new Plant{mBoard=this,mPlantCol=c,mRow=r,mX=GridToPixelX(c,r),mY=GridToPixelY(c,r),mSeedType=(SeedType)args[3]!,mImitaterType=(SeedType)args[4]!};m_plants.Add(plant);
   Invoke(typeof(FusionMod.FusionPlantCreationPatch),"Postfix",[this,plant,args[5]]);return plant;
  }
  public void AddSunMoney(int n,int p)=>Sun+=n;
  public Plant? GetTopPlantAt(int c,int r)=>GetPlantsOnLawn(c,r).NormalPlant;
  public Plant? ToolHitTest(int x,int y,int playerIndex)=>GetTopPlantAt(PixelToGridX(x,y),PixelToGridY(x,y));
  public void MouseDownWithPlant(){}
  public static void Invoke(Type t,string n,object?[] args){try{t.GetMethod(n,BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,args);}catch(TargetInvocationException e){throw e.InnerException!;}}
 }
 public class Plant {
  static int next=10; public IntPtr Pointer=(IntPtr)(++next); public Board mBoard=null!;
  public int mPlantCol,mRow,mPlantHealth=300,mPlantMaxHealth=300,mRenderOrder,mShadowRenderOrder;
  public float mX,mY; public SeedType mSeedType = SeedType.Puffshroom,mImitaterType=SeedType.None;
  public App mApp=>mBoard.mApp; public PlantState mState; public bool mIsAsleep; public int mWakeUpCounter; public void DoSpecial(){}
  public bool mDead,mIsOnBoard=true,mSquished,OffGround,Support,mHighlighted; public PlantOnBungeeState mOnBungeeState;
  public bool NotOnGround()=>OffGround; public static bool IsUpgrade(SeedType t)=>false; public bool IsUpgradableTo(SeedType t)=>false;
  public int CalcRenderOrder()=>mRow*100; public void UpdateReanim(){} public void UpdateReanimColor(){}
  public void Die()=>mDead=true;
 }
}
namespace LawnGlove {
 using Il2CppReloaded.Gameplay;
 public static class LawnGloveMod { public static Plant? Carried; public static Plant? GetCarriedPlant(Board b)=>Carried; }
 public static class PlantPlacementRules { public static SeedType TerrainType(SeedType t)=>t; }
 internal static class PlantMover { internal static bool CanPlace(Board board,Plant plant,int col,int row)=>false; internal static bool TryMove(Board board,Plant plant,int col,int row)=>false; }
 internal class CarriedPlant { public ReloadedObject gridAnchor=new(); public Il2CppSource.Controllers.PreviewController gridPreview=new(); public void UpdatePreview(){} }
}
namespace FusionMod {
 using Il2CppReloaded.Gameplay;
 internal static class BucketPlants { internal static bool CanFeed(Plant p,FusionPlant i)=>false; internal static void Feed(Plant p){} internal static bool IsTarget(Plant p)=>false; }
 internal static class ChomperFusions { internal static bool Active(Plant p)=>!p.mDead&&p.mIsOnBoard&&!p.mBoard.mPaused; }
 internal static class MineFusions { internal static void OnFusion(Plant p){} }
 internal static class ThreepeaterFusions { internal static void OnFusion(Plant p){} }
 internal static class IceInstantFusions { internal static void OnIceFusion(Plant p){} internal static void OnFusion(Plant p){} }
 internal static class CherryBombPlants { internal static void ExplodeFusion(Plant p){} }
 internal static class ImportedPeaVisuals { internal static void CaptureHighlight(Plant p){} }
}
