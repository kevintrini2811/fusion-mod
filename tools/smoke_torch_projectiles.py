"""Verify real projectile conversions and before/after artwork in a temporary debug board."""
import json
from pathlib import Path
import shutil
import sys
import time

PROJECT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT.parent / "debug-mod/tools"))
from debug_api import call, GAME
ROOT = GAME / "UserData/DebugModApi"
OUT = PROJECT / "references/torch-projectiles-22-0-1"
events, checks = [], []

def api(command, **args):
    result = call(ROOT, command, args)
    if command not in ("projectiles", "zombies"):
        events.append(dict(command=command, args=args, response=result))
    assert result["ok"], result
    return result["result"]

def wait(command, predicate, seconds=20):
    end = time.monotonic() + seconds
    while time.monotonic() < end:
        value = api(command, **({"visuals": True} if command == "projectiles" and "--baseline" not in sys.argv else {}))
        match = predicate(value)
        if match: return match
        time.sleep(.025)
    raise AssertionError((command, value))

def run():
    state = api("status")
    assert state["debug"] and state["scene"] == "Playing"
    board = state["board"]
    api("difficulty", board=board, value=3)
    catalog = api("catalog.plants")["registered"]
    def place(seed, marker=None, col=0):
        args = dict(col=col, row=0)
        if marker:
            args["registered"] = next(x["index"] for x in catalog if x["type"] == seed and x["marker"] == marker)
        else: args["type"] = seed
        api("plant.place", **args)
    def clear():
        api("pause", paused=True)
        while live := api("zombies")["items"]:
            api("zombie.remove", board=board, id=live[-1]["native"]["DataID"])
        plants = api("plants")["items"]
        potato = any(p["mSeedType"] == "Potatomine" for p in plants)
        for p in plants: api("plant.remove", board=board, id=p["DataID"])
        api("pause", paused=False)
        time.sleep(11 if potato else 2)
        api("pause", paused=True)
    # Source 4.0.5.2 accepts plain, snow, yellow fire and potato in Scorchwood.
    # Stenchwood accepts only plain and garlic. Other families are negative controls.
    cases = [
        ("snow-thaw", "Snowpea", None, [(3,"Jalapeno")], "Pea", 0, None),
        ("snow-double", "Snowpea", None, [(3,"Jalapeno"),(4,"Jalapeno")], "Fireball", -622001, 50),
        ("yellow-fire", "Peashooter", None, [(1,None),(3,"Jalapeno")], "Fireball", -622001, 70),
        ("garlic-toxic", "Peashooter", "Garlic", [(3,"Garlic")], "PeashooterPea", -622003, 60),
        ("blaze-unchanged", "Peashooter", "Jalapeno", [(3,"Jalapeno")], "PeashooterPea", 0, None),
        ("stench-snow-unchanged", "Snowpea", None, [(3,"Garlic")], "Snowpea", 0, None),
        ("stench-yellow-unchanged", "Peashooter", None, [(1,None),(3,"Garlic")], "PeashooterFireball", 0, None),
        ("stench-blaze-unchanged", "Peashooter", "Jalapeno", [(3,"Garlic")], "PeashooterPea", 0, None),
        ("potato-scorch", "Potatomine", "Peashooter", [(3,"Jalapeno")], "Fireball", -622001, 60),
        ("stench-potato-unchanged", "Potatomine", "Peashooter", [(3,"Garlic")], "Pea", -5331240, None),
    ]
    cases = [
        ("blaze-control", "Peashooter", "Jalapeno", [], "PeashooterPea", 0, None),
        ("snow-control", "Snowpea", None, [], "Snowpea", 0, None),
        ("yellow-control", "Peashooter", None, [(1,None)], "PeashooterFireball", 0, None),
    ] + cases + [("potato-control", "Potatomine", "Peashooter", [], "Pea", -5331240, None)]
    for name,seed,marker,stumps,kind,tag,damage in cases:
        clear()
        place(seed, marker)
        for col,wood in stumps: place("Torchwood", wood, col)
        z = api("zombie.place", type="Normal", col=8, row=0)["native"]
        api("debug.zombie.set", board=board, id=z["DataID"], health=100000, freezeTicks=100000)
        api("pause", paused=False)
        result = dict(name=name, expected=dict(type=kind, marker=tag, damage=damage))
        try:
            first = wait("projectiles", lambda r: next((x for x in r["items"] if 135 < x["mPosX"] < 290 and x["mRow"] == 0), None))
            id = first["DataID"]
            api("pause", paused=True)
            result["before"] = next(x for x in api("projectiles", visuals=True)["items"] if x["DataID"] == id)
            shutil.copy2(api("screenshot")["path"],OUT/(name+"-before.png"))
            api("pause", paused=False)
            endX = 80 * max([col for col,_ in stumps] or [4]) + 125
            after = wait("projectiles", lambda r: next((x for x in r["items"] if x["DataID"] == id and x["mPosX"] > endX), None))
            api("pause", paused=True)
            result["after"] = next(x for x in api("projectiles", visuals=True)["items"] if x["DataID"] == id)
            shutil.copy2(api("screenshot")["path"],OUT/(name+"-after.png"))
            after = result["after"]
            # Yellow fire has a native per-shooter enum; compare semantically.
            assert kind is None or after["mProjectileType"] == kind or name == "snow-thaw" and after["mProjectileType"] == "PeashooterPea", after["mProjectileType"]
            assert after["mCobTargetX"] == tag, after["mCobTargetX"]
            assert damage is None or after["mCobTargetRow"] == damage, after["mCobTargetRow"]
            if "--baseline" not in sys.argv:
                visible = [v for v in after["visuals"] if v["enabled"] and v["active"]]
                assert visible, "No enabled projectile renderer"
                expected_art = "ScorchProjectile" if tag == -622001 else "StenchFireProjectile" if tag == -622003 else "PotatoProjectile" if tag == -5331240 else "BlazeProjectile" if "blaze" in name else None
                if expected_art:
                    assert any("Fusion-"+expected_art+"/" in v["path"] for v in visible), visible
                    assert all("Fusion-" in v["path"] or "/Shadow/" in v["path"] for v in visible), "Native artwork leaks through imported conversion"
                if expected_art == "ScorchProjectile":
                    assert any("/Bullet_firePea_orange/" in v["path"] for v in visible), "Wrong source prefab for orange fire pea"
                if "unchanged" in name:
                    before_paths = {v["path"] for v in result["before"]["visuals"] if v["enabled"] and v["active"] and "/Shadow/" not in v["path"]}
                    after_paths = {v["path"] for v in visible if "/Shadow/" not in v["path"]}
                    assert before_paths == after_paths, ("Unsupported conversion changed artwork",before_paths,after_paths)
                if name == "snow-thaw":
                    assert not any("snow" in (v["path"]+str(v["sprite"])).lower() for v in visible), visible
            api("pause", paused=False)
            impacted = wait("zombies", lambda r: next((x["native"] for x in r["items"] if x["native"]["DataID"] == z["DataID"] and x["native"]["mBodyHealth"] < 100000), None))
            api("pause", paused=True)
            result["impact"] = impacted
            dealt = 100000 - impacted["mBodyHealth"]
            expected_hit = {"snow-thaw":20,"snow-double":50,"yellow-fire":70,"blaze-unchanged":30,"stench-snow-unchanged":40,"stench-yellow-unchanged":40,"stench-blaze-unchanged":30,"potato-scorch":60,"stench-potato-unchanged":30}.get(name)
            result["damage"] = dealt
            if name == "garlic-toxic": assert dealt >= 60, dealt
            elif "unchanged" not in name and expected_hit is not None:
                assert dealt == expected_hit, ("impact damage", dealt, expected_hit)
            result["passed"] = True
            print("PASS", name, after["mProjectileType"],after["mCobTargetX"],after["mCobTargetRow"],flush=True)
        except Exception as error:
            result["passed"] = False; result["error"] = str(error)
            print("FAIL",name,str(error)[:160],flush=True)
        checks.append(result)
        (OUT/("baseline.json" if "--baseline" in sys.argv else "runtime.json")).write_text(json.dumps(dict(checks=checks,events=events),indent=2)+"\n")
    api("pause", paused=False)
    by_name = {c["name"]:c for c in checks}
    for name,control in [("blaze-unchanged","blaze-control"),("stench-blaze-unchanged","blaze-control"),("stench-snow-unchanged","snow-control"),("stench-yellow-unchanged","yellow-control"),("stench-potato-unchanged","potato-control")]:
        current,base = by_name[name],by_name[control]
        current["control"] = control
        if current.get("damage") != base.get("damage"):
            current["passed"] = False
            current["error"] = "Damage differs from native control"
        print("CONTROL",name,current.get("damage"),base.get("damage"),current["passed"],flush=True)
    return all(c["passed"] for c in checks)

if __name__ == "__main__":
    OUT.mkdir(parents=True, exist_ok=True)
    try: success=run()
    finally:
        (OUT/("baseline.json" if "--baseline" in sys.argv else "runtime.json")).write_text(json.dumps(dict(checks=checks,events=events),indent=2)+"\n")
    sys.exit(0 if success else 1)
