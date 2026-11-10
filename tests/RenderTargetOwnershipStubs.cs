namespace MelonLoader {
 [AttributeUsage(AttributeTargets.Assembly)]public sealed class MelonInfo:Attribute{public MelonInfo(Type t,string n,string v,string a){}}
 [AttributeUsage(AttributeTargets.Assembly)]public sealed class MelonGame:Attribute{public MelonGame(string a,string b){}}
 [AttributeUsage(AttributeTargets.Assembly)]public sealed class MelonOptionalDependencies:Attribute{public MelonOptionalDependencies(params string[] n){}}
 [AttributeUsage(AttributeTargets.Assembly)]public sealed class MelonAdditionalDependencies:Attribute{public MelonAdditionalDependencies(params string[] n){}}
 public class MelonMod{public readonly Logger LoggerInstance=new();public virtual void OnInitializeMelon(){}public virtual void OnLateUpdate(){}public virtual void OnApplicationQuit(){}public virtual void OnDeinitializeMelon(){}}
 public sealed class Logger{public void Msg(string value){}}
}
namespace Il2CppSystem {public sealed class Action {
 readonly System.Action invoke;Action(System.Action a){invoke=a;}public void Invoke()=>invoke();
 public static explicit operator Action(System.Action a)=>new(a);
 public static Action? operator +(Action? a,Action? b)=>b==null?a:a==null?b:new(()=>{a.Invoke();b.Invoke();});
 public static Action? operator -(Action? a,Action? b)=>null;
}}
namespace HarmonyLib {
 [AttributeUsage(AttributeTargets.Class)]public sealed class HarmonyPatch:Attribute{public readonly Type Target;public readonly string Method;public HarmonyPatch(Type target,string method){Target=target;Method=method;}}
}
namespace UnityEngine {
 public class Object {
  private static int next;public readonly IntPtr Pointer=(IntPtr)(++next);
  public static readonly List<Camera> Cameras=new();public static T[] FindObjectsOfType<T>(bool inactive=false)where T:class=>Cameras.Cast<T>().ToArray();
 }
 public sealed class Camera:Object{public RenderTexture? targetTexture;}
 public sealed class RenderTexture:Object {
  public void Release()=>NativeHooks.Before(typeof(RenderTexture),nameof(Release),this);
  public static void ReleaseTemporary(RenderTexture value)=>NativeHooks.Before(typeof(RenderTexture),nameof(ReleaseTemporary),value);
 }
 public static class Application{public static Il2CppSystem.Action? quitting;public static void Quit()=>quitting?.Invoke();}
 internal static class NativeHooks {
  internal static void Before(Type owner,string method,object value){
   foreach(var type in typeof(NativeHooks).Assembly.GetTypes()){
    var patch=type.GetCustomAttributes(typeof(HarmonyLib.HarmonyPatch),false).Cast<HarmonyLib.HarmonyPatch>().SingleOrDefault();
    if(patch?.Target==owner&&patch.Method==method)type.GetMethod("Prefix",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static)!.Invoke(null,new[]{value});
   }
  }
 }
}
namespace LawnGlove {public static class OccupiedTilePlacement {public static System.Action? CanPlace,TryPlace;}}
namespace FusionMod {
 internal static class Assets {internal static int Releases;}
 internal static class FusionRecipes{internal const int Count=1;}
 internal static class FusionService{internal static void CanPlaceCarried(){}internal static void TryPlaceCarried(){}}
 internal static class MenuPresentation{internal static void Register(object m){}internal static void Dispose(){++Assets.Releases;}}
 internal static class FusionAlmanac{internal static void RegisterLibrary(){}}
 internal static class FusionAudio{internal static void RegisterLibrary(){}}
 internal static class FusionZombies{internal static void Register(){}internal static void Dispose(){++Assets.Releases;}}
 internal static class DebugBankPreviews{internal static void Clear(){++Assets.Releases;}}
 internal static class GoldenBean{internal static void UpdateUi(){}internal static void Clear(){++Assets.Releases;}}
 internal static class BucketPlants{internal static void UpdateCursor(){}internal static void Clear(){++Assets.Releases;}}
 internal static class BucketDrops{internal static void UpdateVisuals(){}internal static void Clear(){++Assets.Releases;}}
 internal static class ImportedPeaVisuals{internal static void UpdateFumeSmoke(){}internal static void Update(){}internal static void ClearEyesZombies(){}internal static void ClearHypnoHeads(){}internal static void ClearFumeSmoke(){}internal static void Dispose(){++Assets.Releases;}}
 internal static class IronProjectiles{internal static void Clear(){++Assets.Releases;}}
 internal static class Fertilizer{internal static void Clear(){++Assets.Releases;}}
 internal static class SolarShooter{internal static void Clear(){++Assets.Releases;}}
 internal static class TwinMarigold{internal static void Clear(){++Assets.Releases;}}
 internal static class Overheat{internal static void Clear(){++Assets.Releases;}}
 internal static class IceMineBlasts{internal static void Clear(){++Assets.Releases;}}
 internal static class BlazeShooters{internal static void Clear(){++Assets.Releases;}}
 internal static class PoisonShooters{internal static void Clear(){++Assets.Releases;}}
 internal static class RollingProjectiles{internal static void Clear(){++Assets.Releases;}}
}
