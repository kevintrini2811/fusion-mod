using FusionMod;
using UnityEngine;
int checks=0;
void Close(float actual,float expected,string message){++checks;if(!float.IsFinite(actual)||Math.Abs(actual-expected)>.001f)throw new Exception(message);}
// Cached sprite scale .2 produces the previous logged 100 x 99 world target.
// The corrected target must be 150 x 148.5, regardless of native mesh data.
var sprite=new Bounds(new Vector3(12,40,0),new Vector3(500,495,1));
var world=PuffArtworkSizing.Target(sprite,.2f,false);
Close(world.size.x,150,"Enlarge logged world width by 50 percent");
Close(world.size.y,148.5f,"Preserve artwork aspect ratio");
Close(world.min.y,0,"World bottom remains at ground anchor");
Close(world.center.x,0,"World horizontal centre remains at anchor");
var preview=PuffArtworkSizing.Target(sprite,.2f,true);
Close(preview.size.x*.2f,world.size.x,"Preview parent applies cached scale exactly once");
Close(preview.size.y*.2f,world.size.y,"Preview and world share enlargement");
Close(preview.min.y,sprite.min.y,"Preview bottom remains anchored");
Close(preview.center.x,sprite.center.x,"Preview preserves native horizontal centre");
foreach(float scale in new[]{.1f,.5f,1f,2.82f}){
 var target=PuffArtworkSizing.Target(sprite,scale,false);
 Close(target.size.x,750*scale,"World scale comes only from cached sprite");
 Close(target.min.y,0,"All world scales preserve ground anchor");
 Close(PuffArtworkSizing.Target(sprite,scale,true).size.x,750,"Preview never applies cached scale twice");
}
Console.WriteLine($"{checks} Puff sizing checks passed; no game code loaded.");
