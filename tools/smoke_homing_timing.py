"""Representative actual native Homing test on an already-running empty Night debug board."""
import json
import math
from pathlib import Path
import sys
import time
PROJECT=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(PROJECT.parent/"debug-mod/tools"))
from debug_api import GAME,call
API=GAME/"UserData/DebugModApi"
OUT=PROJECT/"references/tick-rate-migration/runtime/homing-runtime.json"
events=[]
def api(command,**args):
    response=call(API,command,args);events.append(dict(command=command,args=args,response=response))
    assert response["ok"],response
    return response["result"]
def wait(command,predicate,timeout=30):
    end=time.monotonic()+timeout
    while time.monotonic()<end:
        result=api(command)
        if match:=predicate(result):return match
        time.sleep(.02)
    raise AssertionError((command,result))
def main():
    state=api("status");assert state["debug"] and state["scene"]=="Playing"
    board=state["board"]
    assert not api("plants")["items"] and not api("zombies")["items"] and not api("projectiles")["items"]
    original=dict(speed=api("speed")["selected"],paused=state["paused"],timing=api("debug.timing"),settings=api("settings"))
    assert not original["timing"]["overridden"]
    catalog=api("catalog.plants")["registered"]
    plants=set();zombies=set();checks=[];cleanup=[]
    def place(seed,marker,col,row):
        card=next(p["index"] for p in catalog if(p["type"],p["marker"])==(seed,marker))
        result=api("plant.place",registered=card,col=col,row=row)["plants"]
        plants.update(p["DataID"] for p in result)
    try:
        api("pause",paused=True);api("speed",index=0);api("debug.timing",board=board,hz=100)
        blocked={(i["mGridX"],i["mGridY"]) for i in api("debug.items")["items"] if i["mGridItemType"]!="ZenTool"}
        row=next(r for r in range(5) if all((col,r) not in blocked for col in (0,2,7)))
        place("Peashooter","ExplodeONut",0,row)
        place("Torchwood","Cherrybomb",2,row)
        target=api("zombie.place",type="Normal",col=7,row=row)["native"];zombies.add(target["DataID"])
        api("debug.zombie.set",board=board,id=target["DataID"],health=1000000,freezeTicks=1000000)
        api("pause",paused=False)
        initial=wait("projectiles",lambda p:next((x for x in p["items"] if x["mCobTargetX"]==-624012 and x["mMotionType"]=="Homing"),None))
        api("pause",paused=True)
        identifier=initial["DataID"]
        before=next(p for p in api("projectiles")["items"] if p["DataID"]==identifier)
        assert len(api("zombies")["items"]) == 1, "One actual native target is required."
        assert abs(math.hypot(before["mVelX"],before["mVelY"])*100-200)<.02,before
        first_tick=api("fps")["simulationTick"];time.sleep(.25)
        paused=next(p for p in api("projectiles")["items"] if p["DataID"]==identifier)
        assert first_tick==api("fps")["simulationTick"] and all(before[k]==paused[k] for k in ("mPosX","mPosY","mProjectileAge"))
        checks.append(dict(test="native Homing identity and pause; exactly one target",before=before,paused=paused))
        api("debug.timing",board=board,hz=120)
        start=api("fps")["simulationTick"];api("pause",paused=False)
        wait("fps",lambda f:f["simulationTick"]-start>=1)
        api("pause",paused=True)
        after=next(p for p in api("projectiles")["items"] if p["DataID"]==identifier)
        assert after["mMotionType"]=="Homing",after
        assert abs(math.hypot(after["mVelX"],after["mVelY"])*120-200)<.02,after
        assert after["mPosX"]>before["mPosX"],(before,after)
        checks.append(dict(test="actual Homing100->120 native speed and direction",before=before,after=after))
        health=next(z["native"]["mBodyHealth"] for z in api("zombies")["items"] if z["native"]["DataID"]==target["DataID"])
        api("pause",paused=False)
        hit=wait("zombies",lambda z:next((x["native"] for x in z["items"] if x["native"]["DataID"]==target["DataID"] and x["native"]["mBodyHealth"]<health),None))
        api("pause",paused=True)
        assert not any(p["DataID"]==identifier for p in api("projectiles")["items"]),"The observed Homing projectile must be consumed by its hit."
        checks.append(dict(test="actual native Homing target hit",target=hit,healthBefore=health,damage=health-hit["mBodyHealth"]))
    finally:
        assert api("status")["board"]==board
        for cmd,args in [("pause",dict(paused=True)),("debug.timing",dict(board=board,reset=True))]:
            try:api(cmd,**args)
            except Exception as e:cleanup.append(str(e))
        for p in api("plants")["items"]:
            if p["DataID"] in plants:api("plant.remove",board=board,id=p["DataID"])
        for z in api("zombies")["items"]:
            if z["native"]["DataID"] in zombies:api("zombie.remove",board=board,id=z["native"]["DataID"])
        try:
            api("pause",paused=False);wait("projectiles",lambda p:not p["items"])
            api("speed",index=original["speed"]);api("pause",paused=original["paused"])
            restored=dict(timing=api("debug.timing"),settings=api("settings"))
            assert restored["timing"]["hz"]==original["timing"]["hz"] and not restored["timing"]["overridden"]
            assert restored["settings"]==original["settings"]
        except Exception as e:cleanup.append(str(e));restored=dict(error=str(e))
        OUT.parent.mkdir(parents=True,exist_ok=True);OUT.write_text(json.dumps(dict(events=events,checks=checks,original=original,restored=restored,cleanupErrors=cleanup,inspectionLimit="The generic native snapshot omits the structured target handle. Actual mode/velocity and one-target hit are live evidence; saved-target collision branch is verified separately offline."),indent=2))
        assert not cleanup,cleanup
    print(json.dumps(dict(checks=len(checks),evidence=str(OUT))))
if __name__=="__main__":main()
