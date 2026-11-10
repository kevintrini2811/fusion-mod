"""Exercise the four ports against the actual, temporary debug board."""
import json
from pathlib import Path
import shutil
import sys
import time

PROJECT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT.parent / "debug-mod/tools"))
from debug_api import call, GAME, ApiFailure

OUT = PROJECT / "references/umbrella-ice-21-5-0"
ROOT = GAME / "UserData/DebugModApi"
results = []
board = None
checks = []


def api(command, **args):
    response = call(ROOT, command, args)
    results.append(dict(command=command, args=args, response=response))
    assert response["ok"], response
    return response["result"]


def wait(command, predicate, seconds=35):
    deadline = time.monotonic() + seconds
    while time.monotonic() < deadline:
        value = api(command)
        if predicate(value):
            return value
        time.sleep(.08)
    raise AssertionError((command, value))


def check(condition, name):
    assert condition, name
    checks.append(name)
    print("PASS", name, flush=True)


def ticks(count):
    first = api("fps")["simulationTick"]
    return wait("fps", lambda x: x["board"] == board and x["simulationTick"] - first >= count, max(35, count / 50 + 10))


def plants():
    return api("plants")["items"]


def zombies():
    return [x["native"] for x in api("zombies")["items"]]


def get_plant(id):
    return next(p for p in plants() if p["DataID"] == id)


def place(seed=None, marker=None, col=2, row=2, offsetY=40):
    args = dict(col=col, row=row, offsetY=offsetY)
    if marker:
        entry = next(x for x in api("catalog.plants")["registered"] if x["type"] == seed and x["marker"] == marker)
        args["registered"] = entry["index"]
    else:
        args["type"] = seed
    before = {p["DataID"] for p in plants()}
    api("plant.place", **args)
    return next(p for p in plants() if p["DataID"] not in before)


def enemy(type="Normal", col=7, row=2, frozen=True, health=100000):
    z = api("zombie.place", type=type, col=col, row=row)["native"]
    api("debug.zombie.set", board=board, id=z["DataID"], health=health, freezeTicks=100000 if frozen else 0)
    return z["DataID"]


def clean():
    api("pause", paused=True)
    for z in zombies():
        api("zombie.remove", board=board, id=z["DataID"])
    for p in plants():
        api("plant.remove", board=board, id=p["DataID"])
    api("pause", paused=False)
    ticks(250)


def screenshot(name):
    value = api("screenshot")
    shutil.copy2(value["path"], OUT / (name + ".png"))


def run():
    global board
    deadline = time.monotonic() + 120
    while True:
        try:
            state = api("status")
            if state["debug"] and state["scene"] == "Playing":
                break
            if state["ready"] and not state["board"]:
                api("startup.continue")
        except ApiFailure:
            pass
        if time.monotonic() > deadline:
            raise AssertionError("debug startup timeout")
        time.sleep(.5)
    board = state["board"]
    api("difficulty", board=board, value=3)
    api("speed", index=4)
    for left, right, seed, marker, health in [
        ("Umbrella", "Cabbagepult", "Umbrella", "Cabbagepult", 1000),
        ("Umbrella", "Melonpult", "Umbrella", "Melonpult", 2000),
        ("Umbrella", "Plantern", "Umbrella", "Plantern", 1000),
        ("Flowerpot", "Iceshroom", "Flowerpot", "Iceshroom", 300),
    ]:
        for reverse in (False, True):
            clean()
            api("pause", paused=True)
            for ingredient in ((right, left) if reverse else (left, right)):
                place(ingredient, offsetY=70 if seed == "Flowerpot" else 40)
            current = plants()
            check(len(current) == 1 and current[0]["mSeedType"] == seed and current[0]["mImitaterType"] == marker,
                  f"recipe {left}+{right}, reverse={reverse}")
            p = current[0]
            check(p["mPlantMaxHealth"] == health, f"health {marker} {health}")
            api("debug.plant.set", board=board, id=p["DataID"], health=100)
            api("debug.fertilize", board=board, col=2, row=2)
            healed = get_plant(p["DataID"])
            check(healed["mPlantHealth"] == health, f"fertilizer {marker}")
            check((healed["mTargetX"], healed["mTargetY"]) == (p["mTargetX"], p["mTargetY"]), f"fertilizer preserves saved state {marker}")
            api("pause", paused=False)
    for seed in ["Wallnut", "Pumpkinshell", "Potatomine", "Squash"]:
        clean()
        pot = place("Flowerpot", "Iceshroom")
        roasted = place(seed, "Cobcannon")
        if seed == "Potatomine":
            api("debug.plant.set", board=board, id=roasted["DataID"], health=123)
        ticks(1050)
        occupants = plants()
        transformed = next(p for p in occupants if p["mSeedType"] == seed)
        check(transformed["mImitaterType"] == "None" and transformed["DataID"] != roasted["DataID"], f"Ice Pot converts {seed}")
        if seed == "Potatomine":
            check(transformed["mState"] == "PotatoArmed" and transformed["mPlantHealth"] == 123, "Ice Pot preserves mine health and arms it")
        screenshot("ice-pot-" + seed)
    clean()
    rind = place("Umbrella", "Melonpult")
    shooter = enemy("PeaHead", frozen=False)
    wait("plants", lambda x: any(p["DataID"] == rind["DataID"] and p["mTargetX"] > 0 for p in x["items"]))
    api("pause", paused=True)
    stored = get_plant(rind["DataID"])["mTargetX"]
    check(stored >= 20, "Rind stores energy from a real enemy pea")
    screenshot("rind-reflection")
    api("debug.zombie.set", board=board, id=shooter, freezeTicks=100000)
    api("zombie.move", board=board, id=shooter, col=2, row=2)
    old = next(z for z in zombies() if z["DataID"] == shooter)["mBodyHealth"]
    api("pause", paused=False)
    ticks(3)
    after = next(z for z in zombies() if z["DataID"] == shooter)["mBodyHealth"]
    check(old - after == 80 + stored and get_plant(rind["DataID"])["mTargetX"] == 0, "Rind contact releases and clears all energy")
    clean()
    light = place("Umbrella", "Plantern")
    eater = enemy(col=2, frozen=False)
    wait("plants", lambda x: any(p["DataID"] == light["DataID"] and p["mTargetX"] < 1500 for p in x["items"]))
    api("pause", paused=True)
    p = get_plant(light["DataID"])
    check(p["mPlantHealth"] == 1000, "Lumos shields real contact/bite damage before HP")
    energy = p["mTargetX"]
    api("debug.plant.set", board=board, id=light["DataID"], health=100)
    api("debug.fertilize", board=board, col=2, row=2)
    p = get_plant(light["DataID"])
    check(p["mPlantHealth"] == 1000 and p["mTargetX"] == energy, "Lumos fertilizer heals without energy refill")
    api("zombie.remove", board=board, id=eater)
    api("pause", paused=False)
    ticks(100)
    check(get_plant(light["DataID"])["mTargetX"] >= energy + 25, "Lumos regenerates from its actual light level")
    screenshot("lumos")
    clean()
    kale = place("Umbrella", "Cabbagepult", col=4)
    place("Cabbagepult", col=1)
    prey = enemy(col=7)
    wait("zombies", lambda x: any(z["native"]["DataID"] == prey and z["native"]["mBodyHealth"] < 100000 for z in x["items"]))
    health = next(z for z in zombies() if z["DataID"] == prey)["mBodyHealth"]
    check((100000 - health) % 70 == 0, "Kale increases native cabbage damage from 40 to 70")
    screenshot("kale-relay")
    clean()
    glacier = place("Fumeshroom", "Wintermelon")
    # Wake through the native tile coffee path on the day board.
    place("InstantCoffee")
    enemy(col=7)
    wait("projectiles", lambda x: any(-614002 <= p["mCobTargetX"] <= -614000 for p in x["items"]))
    screenshot("glacier-projectile")
    check(True, "Glacier creates its saved projectile family")
    clean()


if __name__ == "__main__":
    OUT.mkdir(parents=True, exist_ok=True)
    try:
        run()
    finally:
        if board is not None:
            try:
                state = api("status")
                if state["debug"] and state["board"] == board:
                    api("pause", paused=False)
                    api("level.leave")
            except Exception as error:
                results.append(dict(cleanup_error=str(error)))
        (OUT / "runtime.json").write_text(json.dumps(dict(checks=checks, calls=results), indent=2) + "\n")
