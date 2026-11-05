"""Export Scared original sprite hierarchies, affine material animation and clips.

Reads Unity data only. Source scripts and animation events never run.
The existing curve decoder and sprite/pivot exporter remain authoritative.
"""
import argparse
import fcntl
import json
from pathlib import Path
import zlib
import tempfile
import UnityPy
from export_snow_repeater import SourceAssets, vector
from source_index import PROJECT, atomic_json, sha256
from scared_content import verify_source

PROPERTIES = {0x80000000 | (zlib.crc32(n.encode()) & 0xfffffff): (n, i)
              for i, n in enumerate(("_AngleX", "_AngleY", "_ScaleX", "_ScaleY", "_Alpha", "_IsVisible"))}
PREFABS = {"eyes-zombie": 9080, "blood-sunflower": 8185}


def export(name, prefab, assets, source, fingerprints, root=PROJECT):
    output = root / "assets" / name
    reference = root / "references" / name
    output.mkdir(exist_ok=True); reference.mkdir(exist_ok=True)
    nodes, paths, sprites, affines, evidence = [], {}, {}, [], []
    objects, read = assets.objects, assets.read
    def sprite(i):
        if i in sprites:return
        if i == 0:
            from PIL import Image
            from io import BytesIO
            stream=BytesIO();Image.new("RGBA",(1,1)).save(stream,format="PNG");png=stream.getvalue();meta=dict(File="sprite-0.png",Pivot=[.5,.5],PixelsPerUnit=100);proof=dict(path_id=0,reason="Native null sprite")
        else:meta, png, proof = assets.sprite(i)
        (output/meta["File"]).write_bytes(png); sprites[i]=meta; evidence.append(proof)
    def visit(tid, parent=-1, path=""):
        t=read(tid);go=read(t["m_GameObject"]["m_PathID"])
        if parent>=0:path=(path+"/"+go["m_Name"]).lstrip("/")
        i=len(nodes);paths[zlib.crc32(path.encode())]=i
        node=dict(Name=go["m_Name"],Path=path,Parent=parent,Active=go["m_IsActive"],Position=vector(t["m_LocalPosition"],"xyz"),Rotation=vector(t["m_LocalRotation"],"xyzw"),Scale=vector(t["m_LocalScale"],"xyz"),Sprite=-1,Order=0,FlipX=False,FlipY=False,Color=[1,1,1,1],Enabled=True)
        nodes.append(node)
        for ref in go["m_Component"]:
            ob=objects[ref["component"]["m_PathID"]]
            if ob.type.name!="SpriteRenderer":continue
            r=read(ob.path_id);sid=r["m_Sprite"]["m_PathID"]
            sprite(sid);node.update(Sprite=sid,Order=r["m_SortingOrder"],FlipX=r["m_FlipX"],FlipY=r["m_FlipY"],Color=vector(r["m_Color"],"rgba"),Enabled=r["m_Enabled"])
            material=read(r["m_Materials"][0]["m_PathID"])
            floats=dict(material["m_SavedProperties"]["m_Floats"])
            if "_AngleX" in floats:
                affines.append(dict(Node=i,Outer=-1,Inner=-1,Values=[floats.get(n,default) for n,default in [("_AngleX",0),("_AngleY",0),("_ScaleX",1),("_ScaleY",1)]]))
                node["Color"][3]*=floats.get("_Alpha",1)
                node["Enabled"] &=floats.get("_IsVisible",0)>-.5
        for c in t["m_Children"]:visit(c["m_PathID"],i,path)
    transform,controller=assets.prefab(prefab);visit(transform)
    inner={}
    for a in affines:
        node=nodes[a["Node"]];outer=len(nodes);inside=outer+1
        empty=dict(Name="AffineRotation",Path=node["Path"]+"/@affine",Parent=a["Node"],Active=True,Position=[0,0,0],Rotation=[0,0,0,1],Scale=[1,1,1],Sprite=-1,Order=0,FlipX=False,FlipY=False,Color=[1,1,1,1],Enabled=True)
        nodes.append(empty)
        leaf=dict(node,Name=node["Name"]+"-sprite",Path=node["Path"]+"/@sprite",Parent=outer,Position=[0,0,0],Rotation=[0,0,0,1],Scale=[1,1,1])
        nodes.append(leaf);node["Sprite"]=-1;a.update(Outer=outer,Inner=inside);inner[a["Node"]]=inside
    clips={};clip_proof=[];ignored=[]
    for r in read(controller)["m_AnimationClips"]:
        cid=r["m_PathID"];c=read(cid);chs=[[dict(Time=t,Coefficients=list(v)) for t,v in ch] for ch in assets.channels(cid)];bindings=[];cursor=0
        for b in c["m_ClipBindingConstant"]["genericBindings"]:
            kind,attr=b["typeID"],b["attribute"];width=3 if kind==4 else 1
            if b["path"] not in paths:
                ignored.append(dict(clip=cid,binding=b,reason="Transform absent from original prefab; Unity ignores binding"));cursor+=width;continue
            n=paths[b["path"]];target=n
            if kind==4:
                assert attr==1
            elif kind==25:
                assert b["customType"]==22
                prop,index=PROPERTIES[attr];attr=8+index if index<4 else 7 if index==4 else 6
                if index>=4:target=inner.get(n,n)
                if index==5:
                    for k in chs[cursor]:k["Coefficients"][3]+=1
            elif kind==212 and b["isPPtrCurve"]:
                attr=5;target=inner.get(n,n)
                for k in chs[cursor]:
                    sid=c["m_ClipBindingConstant"]["pptrCurveMapping"][int(k["Coefficients"][3])]["m_PathID"]
                    sprite(sid);k["Coefficients"][3]=sid
            else:raise ValueError(f"Unsupported source binding {b}")
            bindings.append(dict(Node=target,Attribute=attr,Channels=chs[cursor:cursor+width]));cursor+=width
        assert cursor==len(chs)
        clip_name=c["m_Name"].removeprefix("anim_")
        assert clip_name not in clips
        clips[clip_name]=dict(Duration=c["m_MuscleClip"]["m_StopTime"],Bindings=bindings)
        clip_proof.append(dict(id=cid,name=c["m_Name"],events=c["m_Events"]))
    ground=next(i for i,n in enumerate(nodes) if n["Name"]=="Shadow")
    ground=inner.get(ground,ground)
    model=dict(Nodes=nodes,Sprites=sprites,Clips=clips,IdleSpeed=1,ShootAdditive=False,GroundNode=ground,Affines=affines)
    atomic_json(output/"model.json",model)
    atomic_json(reference/"visual-assets.json",dict(source=str(source),sha256=fingerprints,prefab_path_id=prefab,animator_controller_path_id=controller,clips=clip_proof,ignored_bindings=ignored,sprites=evidence,files={p.name:sha256(p) for p in output.iterdir() if p.is_file()}))
    print(name,len(nodes),len(sprites),list(clips))
    return model


def main():
    parser=argparse.ArgumentParser(description=__doc__);parser.add_argument("--source",type=Path,default=Path("/mnt/NFS/Multimedia/Downloads"));args=parser.parse_args()
    paths=[args.source/"GameAssembly.dll",args.source/"PVZ-Scared_Data/resources.assets",args.source/"PVZ-Scared_Data/resources.assets.resS",args.source/"PVZ-Scared_Data/il2cpp_data/Metadata/global-metadata.dat"]
    fingerprints=verify_source(args.source)
    environment=UnityPy.load(str(paths[1]));assets=SourceAssets({o.path_id:o for o in environment.objects if o.assets_file.name=="resources.assets"})
    with (PROJECT/".cache/export.lock").open("a") as lock, tempfile.TemporaryDirectory(dir=PROJECT/".cache") as temporary:
        fcntl.flock(lock,fcntl.LOCK_EX|fcntl.LOCK_NB)
        root=Path(temporary);(root/"assets").mkdir();(root/"references").mkdir()
        for name,prefab in PREFABS.items():export(name,prefab,assets,args.source,fingerprints,root)
        # The closed-eye original head is a loose ingredient, never a plant.
        meta,png,proof=assets.sprite(4582);out=root/"assets/eyes-zombie-head";out.mkdir(exist_ok=True)
        (out/meta["File"]).write_bytes(png)
        model=dict(Nodes=[dict(Name="Body",Path="",Parent=-1,Active=True,Position=[0,0,0],Rotation=[0,0,0,1],Scale=[1,1,1],Sprite=4582,Order=0,FlipX=False,FlipY=False,Color=[1,1,1,1])],Sprites={4582:meta},Clips={"idle":dict(Duration=1,Bindings=[])},IdleSpeed=1,ShootAdditive=False,GroundNode=0)
        atomic_json(out/"model.json",model);ref=root/"references/eyes-zombie-head";ref.mkdir(exist_ok=True);atomic_json(ref/"visual-assets.json",dict(source=str(args.source),sha256=fingerprints,sprites=[proof],files={p.name:sha256(p) for p in out.iterdir()}))
        if any(sha256(p)!=v for p,v in fingerprints.items()):raise ValueError("Source changed during export")
        for slug in (*PREFABS,"eyes-zombie-head"):
            destination=PROJECT/"assets"/slug;destination.mkdir(exist_ok=True)
            generated={p.name for p in (root/"assets"/slug).iterdir()}
            for path in (root/"assets"/slug).iterdir():path.replace(destination/path.name)
            for path in destination.glob("sprite-*.png"):
                if path.name not in generated:path.unlink()
            destination=PROJECT/"references"/slug;destination.mkdir(exist_ok=True)
            (root/"references"/slug/"visual-assets.json").replace(destination/"visual-assets.json")
if __name__=="__main__":main()
