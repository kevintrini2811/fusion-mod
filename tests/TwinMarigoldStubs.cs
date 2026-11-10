// Offline doubles; no game, Unity or IL2CPP assemblies are loaded.
namespace HarmonyLib {
 [AttributeUsage(AttributeTargets.Class)] public sealed class HarmonyPatch : Attribute {
  public HarmonyPatch() {} public HarmonyPatch(Type type,string method) {} }
 public static class AccessTools { public static System.Reflection.MethodInfo Method(Type t,string n)=>t.GetMethod(n)!; }
}
namespace Il2CppReloaded.Services {
 public sealed class UserService { public void AddCoins(int theAmount) {} }
}
namespace Il2CppReloaded.Gameplay {

 public enum GameScenes { Playing, Loading }
 public enum GameMode { Adventure, ChallengeZenGarden, PuzzleIZombie1=20, PuzzleIZombieEndless=30 }
 public enum PlantOnBungeeState { NotOnBungee, OnBungee }
 public enum CoinType { Silver=1,Gold=2,Diamond=3 }
 public enum CoinMotion { Coin }
 public sealed class App { public GameScenes GameScene; public GameMode GameMode; }
 public sealed class Board {
  public IntPtr Pointer=(IntPtr)1; public bool mPaused; public int mCoinsCollected;
  public readonly Queue<int> Random=new(); public readonly List<Coin> Coins=new();
  public int Rand(int max) { var value=Random.Count==0?0:Random.Dequeue(); if(value<0||value>=max)throw new Exception("Invalid random bound"); return value; }
  public Coin AddCoin(float x,float y,CoinType type,CoinMotion motion) {
   var c=new Coin{Pointer=(IntPtr)(Coins.Count+1),DataID=(uint)(Coins.Count+1),mBoard=this,mType=type}; Coins.Add(c);return c; }
  public void DisposeBoard() {}
 }
 public sealed class Plant {
  public SeedType mSeedType = SeedType.Puffshroom,mImitaterType=SeedType.None; public int mPlantHealth=300,mPlantMaxHealth=300,mLaunchCounter,mShootingCounter,mX,mY;
  public bool mDead,mIsOnBoard=true,mSquished,OffGround;
  public PlantOnBungeeState mOnBungeeState; public Board mBoard=new(); public App mApp=new();
  public bool NotOnGround()=>OffGround; public void PlantInitialize(){} public void UpdateProductionPlant(){} public void UpdateShooting(){} public void Fire(){}
 }
 public sealed class Coin {
  public IntPtr Pointer; public uint DataID; public Board mBoard=null!; public CoinType mType; public bool mDead;
  public void ScoreCoin() {} public void Die(){} public void Reset(){}
 }
}
namespace FusionMod {
 internal static class ChomperFusions {
  internal static bool Active(Il2CppReloaded.Gameplay.Plant p)=>!p.mDead&&p.mIsOnBoard&&!p.mSquished&&!p.OffGround&&!p.mBoard.mPaused
   &&p.mBoard.Pointer!=DisposingBoard&&p.mOnBungeeState==0&&p.mApp.GameScene==0&&p.mApp.GameMode!=Il2CppReloaded.Gameplay.GameMode.ChallengeZenGarden;
  internal static IntPtr DisposingBoard;
 }
}
