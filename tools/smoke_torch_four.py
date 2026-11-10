"""Exercise four torch fusions and regression cases using the real debug API."""
import json
from pathlib import Path
import shutil
import sys
import time

PROJECT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT.parent / "debug-mod/tools"))
from debug_api import call, GAME, ApiFailure
ROOT = GAME / "UserData/DebugModApi"
OUT = PROJECT / "references/torch-four-22-1-0"
events, checks = [], []

def api(command, **args):
    response = call(ROOT, command, args)
    if command not in ("projectiles", "plants", "zombies", "status"):
        events.append(dict(command=command, args=args, response=response))
    assert response["ok"], response
    return response["result"]

def wait(command, predicate, seconds=30, **args):
    end = time.monotonic() + seconds
    while time.monotonic() < end:
        try:
            value = api(command, **args)
        except ApiFailure:
            if command != "status": raise
            time.sleep(.5)
            continue
        result = predicate(value)
        if result: return result
        time.sleep(.04)
    raise AssertionError((command, value))

def main():
    state = wait("status", lambda s: s if s["debug"] and s["scene"] == "Playing" else None, 120)
    board = state["board"]
    api("difficulty", board=board, value=3)
    catalog = api("catalog.plants")["registered"]
    def plants(): return api("plants")["items"]
    def place(seed, col=2, row=0, marker=None):
        args = dict(col=col, row=row)
        if marker: args["registered"] = next(x["index"] for x in catalog if x["type"] == seed and x["marker"] == marker)
        else: args["type"] = seed
        return api("plant.place", **args)["plants"]
    def clear():
        api("pause", paused=True)
        while live := api("zombies")["items"]:
            api("zombie.remove", board=board, id=live[-1]["native"]["DataID"])
        for p in plants(): api("plant.remove", board=board, id=p["DataID"])
        api("speed", index=5)
        api("pause", paused=False)
        wait("projectiles", lambda value: not value["items"], 15)
        api("pause", paused=True)
        api("speed", index=0)
    def target(row=0, col=8, health=1000000):
        z = api("zombie.place", type="Normal", col=col, row=row)["native"]
        api("debug.zombie.set", board=board, id=z["DataID"], health=health, freezeTicks=1000000)
        return z
    for ingredient in (() if "--combat" in sys.argv else ("Wallnut", "Threepeater", "Starfruit", "Hypnoshroom")):
        for a, b in (("Torchwood", ingredient), (ingredient, "Torchwood")):
            clear(); place(a); result = place(b)
            p = next(p for p in result if p["mSeedType"] == "Torchwood" and p["mImitaterType"] == ingredient)
            api("debug.plant.set", board=board, id=p["DataID"], health=100)
            api("debug.fertilize", board=board, col=2, row=0)
            healed = next(q for q in plants() if q["DataID"] == p["DataID"])
            assert healed["mPlantHealth"] == healed["mPlantMaxHealth"], healed
            checks.append(dict(recipe=[a,b], result=healed))
        print("PASS recipe both orders and fertilizer", ingredient, flush=True)
    # Native land cards must be allowed to fuse on an unsupported sea mushroom.
    for ingredient in (() if "--combat" in sys.argv else ("Blover", "Plantern", "Hypnoshroom", "Iceshroom", "Scaredyshroom", "Puffshroom", "Cactus", "Fumeshroom", "Starfruit", "Sunflower", "Wallnut", "Potatomine", "Squash")):
        clear(); place("Seashroom", row=2); result = place(ingredient, row=2)
        assert any(p["mImitaterType"] != "None" for p in result), result
        checks.append(dict(water=ingredient, result=result))
    print("PASS water fusions" if "--combat" not in sys.argv else "Combat-only run", flush=True)
    # Observe complete 1/2/3 ignition cycle, not just a fireball enum.
    clear(); place("Torchwood", col=3, marker="Threepeater"); place("Peashooter", col=0); target()
    api("pause", paused=False)
    phases = set(); lanes = set(); fire = []
    end = time.monotonic() + 12
    while time.monotonic() < end:
        for p in plants():
            if p["mImitaterType"] == "Threepeater": phases.add(p["mTargetX"])
        for shot in api("projectiles", visuals=True)["items"]:
            if shot["mProjectileType"] == "Fireball": lanes.add(shot["mRow"]); fire.append(shot)
        if len(phases) == 3 and 1 in lanes: break
        time.sleep(.04)
    assert phases == {0,1,2} and 1 in lanes, (phases,lanes)
    checks.append(dict(torchthreePhases=sorted(phases), lanes=sorted(lanes), projectiles=fire[-6:]))
    print("PASS torchthree ignition cycle", flush=True)
    # Real firing builds summon credit. High simulation speed shortens waiting.
    for marker, seed, expected in (("Wallnut", "Gatlingpea", "nut"), ("Hypnoshroom", "Gatlingpea", "ally")):
        clear(); place("Torchwood",col=3,marker=marker); place(seed,col=0); target()
        api("speed",index=5); api("pause",paused=False)
        if expected == "nut":
            summoned = wait("plants",lambda v: next((p for p in v["items"] if p["mSeedType"] == "Wallnut" and p["mImitaterType"] == "Cobcannon"),None),60)
        else:
            summoned = wait("zombies",lambda v: next((p["native"] for p in v["items"] if p["native"]["mZombieType"] == "GatlingHead" and p["native"]["mMindControlled"]),None),60)
        api("pause",paused=True); api("speed",index=0)
        checks.append(dict(summon=expected,state=summoned))
        print("PASS summon",expected,flush=True)
    clear(); place("Torchwood",col=3,marker="Hypnoshroom"); place("Peashooter",col=0,marker="Hypnoshroom"); target()
    api("pause",paused=False)
    shot=wait("projectiles",lambda v: next((p for p in v["items"] if p["mCobTargetX"] == -430060
        and any("HypnoFireProjectile" in (r["path"] or "") and r["enabled"] for r in p["visuals"])),None),20,visuals=True)
    assert shot["mCobTargetRow"] == 60 and any("HypnoFireProjectile" in (r["path"] or "") and r["enabled"] for r in shot["visuals"]),shot
    checks.append(dict(hypnoFire=shot));print("PASS hypno fire artwork",flush=True)
    clear(); place("Torchwood",col=2,row=1,marker="Starfruit"); place("Starfruit",col=2); target(row=1,col=4)
    api("pause",paused=False)
    shot=wait("projectiles",lambda v: next((p for p in v["items"] if p["mCobTargetX"] == -540009
        and any("FireStarProjectile" in (r["path"] or "") and r["enabled"] for r in p["visuals"])),None),20,visuals=True)
    assert shot["mCobTargetRow"] == 40 and any("FireStarProjectile" in r["path"] and r["enabled"] for r in shot["visuals"]),shot
    checks.append(dict(fireStar=shot));print("PASS fire star artwork",flush=True)
    shutil.copy2(api("screenshot")["path"],OUT/"sparkwood-shot.png")
    api("pause",paused=True)

if __name__ == "__main__":
    OUT.mkdir(parents=True, exist_ok=True)
    try: main()
    finally: (OUT/"runtime.json").write_text(json.dumps(dict(events=events,checks=checks),indent=2)+"\n")
