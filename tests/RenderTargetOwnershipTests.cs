using FusionMod;
using UnityEngine;
internal static class Program {
 static int checks;
 static void Check(bool value,string message){++checks;if(!value)throw new Exception(message);}
 static void Main(){
  var native=new RenderTexture();var foreign=new RenderTexture();var a=new Camera{targetTexture=native};var b=new Camera{targetTexture=foreign};UnityEngine.Object.Cameras.AddRange(new[]{a,b});
  var mod=new FusionMod.FusionMod();mod.OnInitializeMelon();mod.OnLateUpdate();
  Check(ReferenceEquals(a.targetTexture,native)&&ReferenceEquals(b.targetTexture,foreign),"initialization preserves borrowed and foreign camera targets");
  native.Release();Check(ReferenceEquals(a.targetTexture,native)&&ReferenceEquals(b.targetTexture,foreign),"native Release has no global camera-detachment prefix");
  RenderTexture.ReleaseTemporary(foreign);Check(ReferenceEquals(a.targetTexture,native)&&ReferenceEquals(b.targetTexture,foreign),"foreign temporary release has no cross-owner camera mutation");
  Application.Quit();Check(ReferenceEquals(a.targetTexture,native)&&ReferenceEquals(b.targetTexture,foreign),"Unity quitting event leaves native resources to their owner");
  mod.OnApplicationQuit();Check(ReferenceEquals(a.targetTexture,native)&&ReferenceEquals(b.targetTexture,foreign),"Melon quit callback leaves native and foreign targets intact");
  mod.OnDeinitializeMelon();Check(ReferenceEquals(a.targetTexture,native)&&ReferenceEquals(b.targetTexture,foreign),"own deinitialization does not change other owners camera targets");
  Check(Assets.Releases==16,"existing own resource cleanup remains reachable");
  Console.WriteLine($"Render-target ownership: {checks} checks passed; actual Fusion lifecycle, modeled native releases and foreign cameras.");
 }
}
