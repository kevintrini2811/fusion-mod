"""Verify actual Unity depth, including grown native Sun-shrooms and imported mini fusions."""
import json
from pathlib import Path
import shutil
import sys
import time

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT.parent / "debug-mod/tools"))
from debug_api import call, GAME

OUT = ROOT / "references/shroom-depth-22-2-1"
events = []


def api(command, **args):
    response = call(GAME / "UserData/DebugModApi", command, args)
    assert response["ok"], response
    if command != "plants" or args.get("visuals"):
        events.append(dict(command=command, args=args, result=response["result"]))
    return response["result"]


def body_order(plant):
    graphics = plant["visuals"]
    groups = [g for g in graphics["groups"] if g["enabled"] and g["active"] and g["name"].startswith("Fusion-")]
    if groups:
        assert len(groups) == 1, groups
        return groups[0]["layer"], groups[0]["order"]
    bodies = [r for r in graphics["renderers"] if r["enabled"] and r["active"] and r["renderer"] == "MeshRenderer"]
    assert bodies, plant
    return bodies[0]["layer"], bodies[0]["order"]


def main():
    status = api("status")
    assert status["debug"] and status["scene"] == "Playing"
    assert not api("plants")["items"], "Use an empty temporary test board"
    catalog = api("catalog.plants")["registered"]
    pea = next(p["index"] for p in catalog if p["type"] == "Puffshroom" and p["marker"] == "Peashooter")
    cells = []
    api("pause", paused=True)
    # Different columns cover differing native rounding phases. Two rows also
    # cover the row contribution; mix native and imported owners on one tile.
    for row in range(2):
        for col, kinds in enumerate((("Sunshroom",) * 3, ("Puffshroom",) * 3,
                                     (pea,) * 3, ("Puffshroom", pea, "Sunshroom"))):
            for kind in kinds:
                api("plant.place", col=col, row=row, **({"type": kind} if isinstance(kind, str) else {"registered": kind}))
            cells.append((col, row))
    def verify(label):
        snapshot = api("plants", visuals=True)
        for col, row in cells:
            plants = [p for p in snapshot["items"] if p["mPlantCol"] == col and p["mRow"] == row]
            assert len(plants) == 3, plants
            back = min(plants, key=lambda p: p["mY"])
            layer, depth = body_order(back)
            for front in plants:
                if front["DataID"] == back["DataID"]: continue
                front_layer, front_depth = body_order(front)
                assert layer == front_layer and depth < front_depth, (label, back, front)
        print("PASS actual Unity depth:", label, flush=True)
        shutil.copy2(api("screenshot")["path"], OUT / (label + ".png"))
    api("pause", paused=False); time.sleep(.5); api("pause", paused=True)
    verify("young-and-imported")
    api("speed", index=6); api("pause", paused=False)
    end = time.monotonic() + 45
    while time.monotonic() < end:
        native = [p for p in api("plants")["items"] if p["mSeedType"] == "Sunshroom" and p["mImitaterType"] == "None"]
        if native and all(p["mState"] == "SunshroomBig" for p in native): break
        time.sleep(.3)
    else: raise AssertionError("Native Sun-shrooms did not mature")
    api("speed", index=0); time.sleep(.5); api("pause", paused=True)
    verify("grown-and-imported")
    for _ in range(5):
        api("pause", paused=False); time.sleep(.2); api("pause", paused=True)
        verify("grown-and-imported")


if __name__ == "__main__":
    OUT.mkdir(exist_ok=True)
    try: main()
    finally: (OUT / "runtime.json").write_text(json.dumps(events, indent=2) + "\n")
