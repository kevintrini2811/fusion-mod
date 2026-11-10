"""Live 100/120-Hz timing checks on an already running, empty Night debug board.

Does not start the game, load/save a profile, or enter an ordinary level. Root
must explicitly authorize execution after installation. Offline core tests are
separate evidence; this script reads the actual native snapshots and records.
"""
import json
import math
from pathlib import Path
import sys
import time

PROJECT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT.parent / "debug-mod/tools"))
from debug_api import GAME, call
API = GAME / "UserData/DebugModApi"
OUT = PROJECT / "references/tick-rate-migration/runtime"
EVENTS, CHECKS = [], []


def api(command, **args):
    response = call(API, command, args)
    EVENTS.append(dict(command=command, args=args, response=response))
    assert response["ok"], response
    return response["result"]


def wait(command, predicate, seconds=30, **args):
    end = time.monotonic() + seconds
    while time.monotonic() < end:
        value = api(command, **args)
        if result := predicate(value):
            return result
        time.sleep(.02)
    raise AssertionError(("timeout", command, value))


def main():
    state = api("status")
    assert state["debug"] and state["scene"] == "Playing", state
    assert not api("plants")["items"] and not api("zombies")["items"] and not api("projectiles")["items"], "Use an empty temporary debug board."
    board = state["board"]
    original = dict(speed=api("speed")["selected"], paused=state.get("paused", False), timing=api("debug.timing"), settings=api("settings"))
    assert not original["timing"]["overridden"], "Reset any previous debug timing test first."
    catalog = api("catalog.plants")["registered"]
    own_plants, own_zombies = set(), set()
    OUT.mkdir(parents=True, exist_ok=True)

    def plants():
        return api("plants")["items"]

    def zombies():
        return [z["native"] for z in api("zombies")["items"]]

    def place(seed, marker=None, col=0, row=0):
        args = dict(col=col, row=row)
        if marker:
            args["registered"] = next(p["index"] for p in catalog if (p["type"], p["marker"]) == (seed, marker))
        else:
            args["type"] = seed
        values = api("plant.place", **args)["plants"]
        found = next(p for p in values if (p["mSeedType"], p["mImitaterType"]) == (seed, marker or "None"))
        own_plants.add(found["DataID"])
        return found

    def target(col, row, frozen=False):
        z = api("zombie.place", type="Normal", col=col, row=row)["native"]
        own_zombies.add(z["DataID"])
        api("debug.zombie.set", board=board, id=z["DataID"], health=1000000, freezeTicks=1000000 if frozen else 0)
        return z["DataID"]

    def snapshot(identifier):
        return next(p for p in plants() if p["DataID"] == identifier)

    def run_ticks(count):
        start = api("fps")["simulationTick"]
        api("pause", paused=False)
        wait("fps", lambda f: f["board"] == board and f["simulationTick"] - start >= count)
        api("pause", paused=True)
        end = api("fps")["simulationTick"]
        return end - start

    def record(kind, identifier):
        return next(i for i in api("debug.items")["items"] if i["mSunCount"] == -521090-kind and (i["mGridX"] & 0xffffffff) == identifier)

    def shot_age(shot):
        r = record(2, shot["DataID"])
        return (r["mPosX"] + (shot["mProjectileAge"] - r["mPosY"]) * 600 / r["mGridItemCounter"]) / 600

    try:
        api("pause", paused=True)
        api("speed", index=0)
        blood = place("Sunflower", "Gravebuster", row=0)["DataID"]
        native = place("Sunflower", row=1)["DataID"]
        for hz in (100, 120):
            api("debug.timing", board=board, hz=hz)
            if snapshot(native)["mLaunchCounter"] <= hz:
                api("pause", paused=False)
                wait("plants", lambda p: next(x for x in p["items"] if x["DataID"] == native)["mLaunchCounter"] > 500)
                api("pause", paused=True)
            before = {identifier: snapshot(identifier) for identifier in (blood, native)}
            ticks = run_ticks(round(hz * .4))
            after = {identifier: snapshot(identifier) for identifier in (blood, native)}
            source_delta = before[blood]["mLaunchCounter"] - after[blood]["mLaunchCounter"]
            native_delta = before[native]["mLaunchCounter"] - after[native]["mLaunchCounter"]
            assert abs(source_delta - ticks * 100 / hz) <= 1.01, (hz, ticks, source_delta)
            assert native_delta == ticks, (hz, ticks, native_delta)
            CHECKS.append(dict(test="actual counters", hz=hz, nativeTicks=ticks, sourceDelta=source_delta, unchangedNativeDelta=native_delta))

        # Pausing must stop the native simulation and both timer domains.
        frozen = dict(fps=api("fps"), blood=snapshot(blood), native=snapshot(native))
        time.sleep(.3)
        stable = dict(fps=api("fps"), blood=snapshot(blood), native=snapshot(native))
        assert frozen["fps"]["simulationTick"] == stable["fps"]["simulationTick"]
        assert frozen["blood"]["mLaunchCounter"] == stable["blood"]["mLaunchCounter"]
        assert frozen["native"]["mLaunchCounter"] == stable["native"]["mLaunchCounter"]
        CHECKS.append(dict(test="pause", before=frozen, after=stable))

        api("debug.timing", board=board, hz=100)
        place("Puffshroom", "Cabbagepult", col=0, row=2)
        target(8, 2, frozen=True)
        api("pause", paused=False)
        first = wait("projectiles", lambda p: next((x for x in p["items"] if x["mMotionType"] == "Lobbed"), None))
        api("pause", paused=True)
        identifier = first["DataID"]
        current = next(p for p in api("projectiles")["items"] if p["DataID"] == identifier)
        old_hz = 100
        for hz in (120, 100):
            before, before_age = current, shot_age(current)
            api("debug.timing", board=board, hz=hz)
            ticks = run_ticks(1)
            current = next(p for p in api("projectiles")["items"] if p["DataID"] == identifier)
            after_age = shot_age(current)
            assert abs(current["mVelX"]*hz - before["mVelX"]*old_hz) < .02, (before, current)
            assert abs(current["mAccZ"]*hz*hz - before["mAccZ"]*old_hz*old_hz) < .1, (before, current)
            assert abs(after_age - before_age - ticks/hz) < .001, (before_age, after_age, ticks, hz)
            CHECKS.append(dict(test="actual ballistic rate handoff", oldHz=old_hz, hz=hz, nativeTicks=ticks, before=before, after=current, beforeAge=before_age, afterAge=after_age))
            old_hz = hz

        # A real bite removes the one-HP Frozen Cherry and triggers its own
        # explosion/freeze path. Debug freeze setters are not evidence for this.
        blocked = {(i["mGridX"], i["mGridY"]) for i in api("debug.items")["items"] if i["mGridItemType"] != "ZenTool"}
        freeze_col, freeze_row = next((col, 4) for col in (5, 4, 3, 6, 2) if (col, 4) not in blocked)
        cherry = place("Peashooter", "Iceshroom", col=freeze_col, row=freeze_row)["DataID"]
        api("debug.plant.set", board=board, id=cherry, health=1)
        victim = target(freeze_col, freeze_row)
        api("pause", paused=False)
        wait("zombies", lambda z: next((x["native"] for x in z["items"] if x["native"]["DataID"] == victim and x["native"]["mIceTrapCounter"] > 0), None), seconds=20)
        api("pause", paused=True)
        before = next(z for z in zombies() if z["DataID"] == victim)
        provenance = record(1, victim)
        assert provenance["mGridY"] & 2 and provenance["mPosY"] > 0, provenance
        api("debug.timing", board=board, hz=120)
        ticks = run_ticks(12)
        after = next(z for z in zombies() if z["DataID"] == victim)
        assert abs(before["mIceTrapCounter"]/100 - after["mIceTrapCounter"]/120 - ticks/120) < .025, (before, after, ticks)
        CHECKS.append(dict(test="actual owned freeze handoff", before=before, after=after, nativeTicks=ticks, provenance=provenance))
    finally:
        # This script owns only its temporary entities and timing override.
        cleanup_errors = []
        assert api("status")["board"] == board, "The test board changed; do not clean entities on a replacement board."
        for command, args in [("pause", dict(paused=True)), ("debug.timing", dict(board=board, reset=True))]:
            try: api(command, **args)
            except Exception as error: cleanup_errors.append(str(error))
        for z in zombies():
            if z["DataID"] in own_zombies:
                try: api("zombie.remove", board=board, id=z["DataID"])
                except Exception as error: cleanup_errors.append(str(error))
        for plant in plants():
            if plant["DataID"] in own_plants:
                try: api("plant.remove", board=board, id=plant["DataID"])
                except Exception as error: cleanup_errors.append(str(error))
        try:
            api("pause", paused=False)
            wait("projectiles", lambda p: not p["items"], seconds=20)
            api("speed", index=original["speed"])
            api("pause", paused=original["paused"])
            restored = dict(timing=api("debug.timing"), settings=api("settings"))
            assert restored["timing"]["hz"] == original["timing"]["hz"] and not restored["timing"]["overridden"]
            assert restored["settings"] == original["settings"]
        except Exception as error:
            cleanup_errors.append(str(error))
            restored = dict(error=str(error))
        (OUT/"timing-runtime.json").write_text(json.dumps(dict(checks=CHECKS, events=EVENTS, original=original, restored=restored, cleanupErrors=cleanup_errors), indent=2))
        assert not cleanup_errors, cleanup_errors
    print(json.dumps(dict(checks=len(CHECKS), evidence=str(OUT/"timing-runtime.json"))))


if __name__ == "__main__":
    main()
