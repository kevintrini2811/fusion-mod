"""Exercise native water transitions and profile isolation through Debug Mod."""
import json
from pathlib import Path
import shutil
import sys
import time

PROJECT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT.parent / "debug-mod/tools"))
from debug_api import call, GAME

OUT = PROJECT / "references/support-ducks-22-0-0"
ROOT = GAME / "UserData/DebugModApi"
events = []

def api(command, **args):
    response = call(ROOT, command, args)
    events.append(dict(command=command, args=args, response=response))
    assert response["ok"], response
    return response["result"]

def run_ducks():
    status = api("status")
    assert status["debug"] and status["scene"] == "Playing"
    board = status["board"]
    api("pause", paused=True)
    while live := api("zombies")["items"]:
        api("zombie.remove", board=board, id=live[-1]["native"]["DataID"])
    sightings = GAME / "UserData/ZombieLibrary/sightings.json"
    before = sightings.read_bytes() if sightings.exists() else None
    for index, kind in enumerate(["Newspaper", "Door", "Bobsled", "Balloon", "Pogo", "Ladder", "Imp", "Target", "TrashCan"]):
        result = api("zombie.place", type=kind, col=2 + index % 6, row=2 + index // 6)
        print(kind, result["native"]["DataID"], flush=True)
    api("pause", paused=False)
    time.sleep(4)
    zombies = [item["native"] for item in api("zombies")["items"]]
    print([(z["mZombieType"], z["mInPool"], z["mHasObject"], z["mZombiePhase"]) for z in zombies], flush=True)
    shutil.copy2(api("screenshot")["path"], OUT / "ducks-first.png")
    assert (sightings.read_bytes() if sightings.exists() else None) == before, "Debug sightings changed permanent data"
    for kind in ["Newspaper", "Door", "Pogo", "Ladder", "Imp", "Target", "TrashCan"]:
        assert any(z["mZombieType"] == kind and z["mInPool"] for z in zombies), kind
    assert len([z for z in zombies if z["mZombieType"] == "Bobsled"]) == 4
    assert all(z["mInPool"] for z in zombies if z["mZombieType"] == "Bobsled"), "All bobsled members must enter water"
    assert all(not z["mHasObject"] for z in zombies if z["mZombieType"] == "Pogo")
    assert any(z["mZombieType"] == "Balloon" and not z["mDead"] for z in zombies)
    print("PASS duck swimming, balloon survival, pogo break, persistent sightings unchanged", flush=True)

def wait(command, predicate, seconds=90):
    end = time.monotonic() + seconds
    while time.monotonic() < end:
        value = api(command)
        if predicate(value): return value
        time.sleep(.1)
    raise AssertionError((command, value))

def run_plants():
    state = wait("status", lambda s: s["debug"] and s["scene"] == "Playing")
    board = state["board"]
    api("difficulty", board=board, value=3)
    def plants(): return api("plants")["items"]
    def clear():
        api("pause", paused=True)
        while live := api("zombies")["items"]:
            api("zombie.remove", board=board, id=live[-1]["native"]["DataID"])
        for item in plants(): api("plant.remove", board=board, id=item["DataID"])
        api("pause", paused=False)
        time.sleep(.3)
        api("pause", paused=True)
    def place(seed, col=2, row=0):
        return api("plant.place", type=seed, col=col, row=row)["plants"]
    def fusion(left, right, row=0, col=2):
        place(left, col, row); result=place(right, col, row)
        print("FUSION", left, right, [(p["mSeedType"],p["mImitaterType"]) for p in result], flush=True)
        return result[-1]
    catalog=api("catalog.plants")["registered"]
    for left,right,native,marker in [("Torchwood","Jalapeno","Torchwood","Jalapeno"),("Torchwood","Garlic","Torchwood","Garlic"),("Plantern","Doomshroom","Plantern","Doomshroom"),("Seashroom","Blover","Seashroom","Blover")]:
        clear(); row=2 if native=="Seashroom" else 0
        p=fusion(left,right,row)
        assert p["mSeedType"]==native and p["mImitaterType"]==marker
        api("debug.plant.set",board=board,id=p["DataID"],health=100)
        api("debug.fertilize",board=board,col=2,row=row)
        assert next(x for x in plants() if x["DataID"]==p["DataID"])["mPlantHealth"]==300
        api("pause",paused=False);time.sleep(.5)
        shutil.copy2(api("screenshot")["path"],OUT/("plant-"+marker+".png"))
    for left,right,native,marker in [("Jalapeno","Torchwood","Torchwood","Jalapeno"),("Garlic","Torchwood","Torchwood","Garlic"),("Doomshroom","Plantern","Plantern","Doomshroom")]:
        clear(); row=2 if native=="Seashroom" else 0
        if row==2: place("Lilypad",2,row)
        p=fusion(left,right,row)
        assert any(x["mSeedType"]==native and x["mImitaterType"]==marker for x in plants())
    for marker,expected in [("Jalapeno",-622001),("Garlic",-622002),("double-garlic",-622003)]:
        clear(); place("Peashooter",0,0)
        fusion("Torchwood","Jalapeno" if marker=="Jalapeno" else "Garlic",0,2)
        if marker=="double-garlic": fusion("Torchwood","Garlic",0,3)
        z=api("zombie.place",type="Normal",col=7,row=0)["native"]
        api("debug.zombie.set",board=board,id=z["DataID"],health=100000,freezeTicks=100000)
        api("pause",paused=False)
        shot=wait("projectiles",lambda s:any(x["mCobTargetX"]==expected for x in s["items"]),20)
        print("SHOT", marker,[(x["mCobTargetX"],x["mCobTargetRow"],x["mProjectileType"]) for x in shot["items"]],flush=True)
        shutil.copy2(api("screenshot")["path"],OUT/("shot-"+marker+".png"))
        time.sleep(4)
        after=next(x["native"] for x in api("zombies")["items"] if x["native"]["DataID"]==z["DataID"])
        assert after["mBodyHealth"]<100000
    clear()
    p=fusion("Torchwood","Jalapeno",0,2)
    z=api("zombie.place",type="Normal",col=6,row=0)["native"]
    api("debug.zombie.set",board=board,id=z["DataID"],health=10000,freezeTicks=100000)
    api("plant.remove",board=board,id=p["DataID"])
    after=next(x["native"] for x in api("zombies")["items"] if x["native"]["DataID"]==z["DataID"])
    assert after["mBodyHealth"]==8200, after["mBodyHealth"]
    print("PASS four recipes, fertilizer, three projectile families, damage, Scorchwood death fire",flush=True)
    api("pause",paused=False)

if __name__ == "__main__":
    OUT.mkdir(parents=True, exist_ok=True)
    mode = sys.argv[1] if len(sys.argv)>1 else "ducks"
    try:
        run_plants() if mode=="plants" else run_ducks()
    finally:
        (OUT / (mode+"-runtime.json")).write_text(json.dumps(events, indent=2) + "\n")
