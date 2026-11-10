"""Runtime contracts for charge torches and reported regressions; temporary debug boards only."""
import json
from pathlib import Path
import sys
import time
import shutil

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT.parent / "debug-mod/tools"))
from debug_api import call, GAME
API = GAME / "UserData/DebugModApi"
OUT = ROOT / "references/charge-torches-22-2-0"
EVENTS, CHECKS = [], []

def api(command, **args):
    value = call(API, command, args)
    if command not in ("status", "plants", "zombies", "projectiles", "fps"):
        EVENTS.append(dict(command=command, args=args, response=value))
    assert value["ok"], value
    return value["result"]

def wait(command, predicate, seconds=20, **args):
    end = time.monotonic() + seconds
    last = None
    while time.monotonic() < end:
        last = api(command, **args)
        if value := predicate(last): return value
        time.sleep(.03)
    raise AssertionError((command, last))

def main():
    status = api("status")
    assert status["debug"] and status["scene"] == "Playing", status
    board = status["board"]
    api("difficulty", board=board, value=3)
    catalog = api("catalog.plants")["registered"]
    graves = {(g["mGridX"], g["mGridY"]) for g in api("debug.items")["items"] if g["mGridItemType"] == "Gravestone"}
    def plants(): return api("plants")["items"]
    def zombies(): return api("zombies")["items"]
    def place(seed, col=2, row=1, marker=None, **extra):
        args = dict(col=col, row=row, **extra)
        if marker:
            args["registered"] = next(p["index"] for p in catalog if p["type"] == seed and p["marker"] == marker)
        else: args["type"] = seed
        return api("plant.place", **args)["plants"]
    def target(col=8, row=1, health=1000000):
        z = api("zombie.place", type="Normal", col=col, row=row)["native"]
        api("debug.zombie.set", board=board, id=z["DataID"], health=health, freezeTicks=1000000)
        return z
    def clear():
        api("pause", paused=True)
        for z in zombies(): api("zombie.remove", board=board, id=z["native"]["DataID"])
        for p in plants(): api("plant.remove", board=board, id=p["DataID"])
        api("speed", index=6)
        api("pause", paused=False)
        wait("projectiles", lambda v: not v["items"], 15)
        api("pause", paused=True)
        api("speed", index=0)
    def pause_shot(tag, model):
        shot = wait("projectiles", lambda v: next((p for p in v["items"] if p["mCobTargetX"] == tag
            and any(model in r["path"] and r["enabled"] and r["active"] for r in p["visuals"])), None), 25, visuals=True)
        api("pause", paused=True)
        CHECKS.append(dict(projectile=shot))
        return shot

    # Exercise the real native button, then allow multiple render frames for conflicting writers.
    api("pause", paused=False); api("speed", index=0)
    for index in range(1, 7):
        value = api("speed", next=True)
        time.sleep(.15)
        persisted = api("speed")
        assert value["selected"] == index and persisted["selected"] == index, (value, persisted)
        CHECKS.append(dict(speed=persisted))
    api("speed", next=True)
    assert api("speed")["selected"] == 0
    print("PASS native speed button through 20x", flush=True)

    for a, b, seed, marker in (("Potatomine", "Torchwood", "Potatomine", "Torchwood"),
                              ("Torchwood", "Cherrybomb", "Torchwood", "Cherrybomb")):
        for first, second in ((a,b),(b,a)):
            clear(); place(first); result = place(second)
            p = next(p for p in result if p["mSeedType"] == seed and p["mImitaterType"] == marker)
            api("debug.plant.set", board=board, id=p["DataID"], health=100)
            api("debug.fertilize", board=board, col=2, row=1)
            healed = next(q for q in plants() if q["DataID"] == p["DataID"])
            assert healed["mPlantHealth"] == healed["mPlantMaxHealth"] == 300, healed
            assert healed["mTargetX"] == p["mTargetX"], healed
            CHECKS.append(dict(recipe=[first,second], plant=healed))
        print("PASS recipe both orders and fertilizer", seed, marker, flush=True)

    # Both new passive plants must apply their complete death explosion.
    for seed, marker in (("Potatomine", "Torchwood"),("Torchwood", "Cherrybomb")):
        clear(); place(seed, marker=marker); z = target(col=2, health=5000)
        p = next(p for p in plants() if p["mImitaterType"] == marker)
        api("plant.remove", board=board, id=p["DataID"])
        after = next(q["native"] for q in zombies() if q["native"]["DataID"] == z["DataID"])
        assert after["mBodyHealth"] == 3200, after
        CHECKS.append(dict(death=[seed,marker], before=z, after=after))
    print("PASS both death explosions: 1800 damage", flush=True)

    # Ground collision on the actual split fireballs, including the neighboring row.
    clear(); place("Torchwood", col=3, row=1, marker="Threepeater"); place("Peashooter", col=0, row=1)
    targets = [target(row=r) for r in (0,1,2)]
    api("speed", index=4); api("pause", paused=False)
    wait("zombies", lambda v: all(next(z["native"]["mBodyHealth"] for z in v["items"]
        if z["native"]["DataID"] == t["DataID"]) < 1000000 for t in targets), 20)
    api("pause", paused=True)
    CHECKS.append(dict(torchthreeHits=zombies()))
    print("PASS Torchthree damage in all three rows", flush=True)

    # Real small peas cross the Hot Potato, use source graphics and deal 40 damage.
    clear(); place("Potatomine", col=2, marker="Torchwood"); place("Puffshroom", col=0, marker="Peashooter"); z=target()
    api("pause", paused=False); shot=pause_shot(-624010,"SmallFireProjectile")
    assert shot["mCobTargetRow"] == 40 and shot["mDamageRangeFlags"] == "Ground", shot
    api("pause", paused=False)
    wait("zombies", lambda v: next(q["native"]["mBodyHealth"] for q in v["items"] if q["native"]["DataID"]==z["DataID"]) < 1000000)
    api("pause", paused=True)
    print("PASS small fire pea graphics and damage", flush=True)

    # Potato peas count eight; the 13th ignition exceeds 100 and summons the shared Roasted Potato.
    clear(); place("Potatomine", col=2, marker="Torchwood"); place("Potatomine", col=0, marker="Peashooter")
    wall_col = next(c for c in range(7, 3, -1) if (c, 1) not in graves)
    api("debug.fertilize", board=board, col=0, row=1); place("Tallnut",col=wall_col); target(col=8)
    api("pause", paused=False); shot=pause_shot(-624011,"PotatoFireProjectile")
    assert shot["mCobTargetRow"] == 50, shot
    api("speed", index=6); api("pause", paused=False)
    summoned = wait("plants",lambda v: next((p for p in v["items"] if p["mSeedType"]=="Potatomine" and p["mImitaterType"]=="Cobcannon"),None),30)
    api("pause",paused=True); CHECKS.append(dict(roastedPotato=summoned))
    print("PASS potato fire graphics and eight-credit summon",flush=True)

    clear(); place("Potatomine",col=2,marker="Torchwood"); place("Peashooter",col=0,marker="Wallnut"); target()
    api("pause",paused=False)
    shot=pause_shot(-624013,"FireNutProjectile")
    assert shot["mCobTargetRow"]==40,shot
    CHECKS.append(dict(ignitedRollingNut=shot))
    print("PASS rolling-nut ignition graphics and doubled damage",flush=True)

    # An explosive cherry becomes a homing fire cherry, including its real visible model and blast.
    clear(); place("Torchwood", col=2, marker="Cherrybomb"); place("Peashooter", col=0, marker="ExplodeONut"); z=target(row=2)
    # The registered Explode-o-shooter emits explosive cherries; another row exercises global homing.
    straightTarget = target(row=1)
    api("pause",paused=False)
    shot=pause_shot(-624012,"FireCherryProjectile")
    assert shot["mCobTargetRow"]==300,shot
    api("zombie.remove",board=board,id=straightTarget["DataID"])
    api("pause",paused=False)
    wait("zombies",lambda v: any(q["native"]["DataID"]==z["DataID"] and q["native"]["mBodyHealth"]<1000000 for q in v["items"]),20)
    api("pause",paused=True)
    CHECKS.append(dict(fireCherryHits=zombies()))
    print("PASS explosive-cherry conversion, homing and blast",flush=True)

    # Slot order uses a common tile depth, despite different horizontal offsets.
    clear()
    for _ in range(3): place("Puffshroom",col=2,row=1,marker="Peashooter")
    ps=plants(); back=min(ps,key=lambda p:p["mY"])
    assert all(p["mRenderOrder"]>back["mRenderOrder"] for p in ps if p["DataID"]!=back["DataID"]),ps
    CHECKS.append(dict(shroomLayers=ps))
    print("PASS upper mushroom behind both lower slots",flush=True)

    clear()
    zombiesCatalog=api("catalog.zombies")
    assert all(z["type"]!="DuckyTube" for z in zombiesCatalog["debug"])
    assert len(zombiesCatalog["registered"])==4,zombiesCatalog
    for row,index in enumerate((0,1,2)):
        api("zombie.place",registered=index,col=4,row=row)
    api("pause",paused=False);time.sleep(.5);api("pause",paused=True)
    data=api("zombies",visuals=True)
    CHECKS.append(dict(armoredEyes=data))
    for item in data["items"]:
        z=item["native"] if "native" in item else item["state"]["native"]
        assert z["mGraveX"]==-521012,z
        if z["mZombieType"]!="Normal": assert z["mHelmHealth"]>0,z
    shutil.copy2(api("screenshot")["path"],OUT/"eyes-armor.png")
    print("PASS armored Eyes variants and duck-free chooser",flush=True)

    clear()
    if (4, 1) not in graves: api("gravestone.place",board=board,col=4,row=1)
    z=api("zombie.place",registered=1,col=7,row=1)["native"]
    api("debug.zombie.rise",board=board,id=z["DataID"],col=4,row=1)
    api("pause",paused=False);time.sleep(.3);api("pause",paused=True)
    rising=api("zombies",visuals=True);CHECKS.append(dict(graveRising=rising))
    shutil.copy2(api("screenshot")["path"],OUT/"grave-rise.png")
    api("pause",paused=False)
    wait("zombies",lambda v: any(q["native"]["mZombiePhase"]=="RisingFromGrave" and -70<q["native"]["mAltitude"]<-10 for q in v["items"]),10)
    api("pause",paused=True)
    CHECKS.append(dict(gravePartial=api("zombies",visuals=True)))
    shutil.copy2(api("screenshot")["path"],OUT/"grave-rise-partial.png")
    api("pause",paused=False)
    wait("zombies",lambda v: all(q["native"]["mZombiePhase"]!="RisingFromGrave" for q in v["items"]),10)
    api("pause",paused=True)
    CHECKS.append(dict(graveComplete=api("zombies",visuals=True)))
    print("PASS native grave rise lifecycle",flush=True)

if __name__=="__main__":
    OUT.mkdir(exist_ok=True)
    try: main()
    finally: (OUT/"runtime.json").write_text(json.dumps(dict(events=EVENTS,checks=CHECKS),indent=2)+"\n")
