"""Verify four ports through the documented API in temporary debug levels."""
import json
from pathlib import Path
import shutil
import sys
import time

PROJECT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT.parent / "debug-mod/tools"))
from debug_api import call, GAME


def run():
    root = GAME / "UserData/DebugModApi"
    output = PROJECT / "references/fume-guard-21-2-0"
    results = []
    game_log = GAME / "MelonLoader/Latest.log"
    log_start = game_log.stat().st_size
    board = None

    def api(command, **args):
        value = call(root, command, args)
        results.append(dict(command=command, args=args, response=value))
        assert value["ok"], value
        return value["result"]

    def wait(command, predicate, seconds=30):
        deadline = time.monotonic() + seconds
        while time.monotonic() < deadline:
            response = call(root, command, {})
            results.append(dict(command=command, response=response))
            value = response.get("result")
            if response.get("ok") and predicate(value):
                return value
            time.sleep(.08)
        raise AssertionError(f"Timeout: {command}; last={value}")

    def ticks(count):
        first = api("fps")["simulationTick"]
        return wait("fps", lambda x: x["board"] == board and x["simulationTick"] - first >= count)

    def plants():
        return api("plants")["items"]

    def zombies():
        return [item["native"] for item in api("zombies")["items"]]

    def shots():
        return api("projectiles")["items"]

    def plant(seed, col=2, row=2):
        return api("plant.place", type=seed, col=col, row=row)["plants"]

    def direct(seed, marker, col=2, row=2):
        catalog = api("catalog.plants")["registered"]
        entry = next(x for x in catalog if x["type"] == seed and x["marker"] == marker)
        return api("plant.place", registered=entry["index"], col=col, row=row)["plants"][-1]

    def target(col, row=2, health=10000, frozen=True):
        z = api("zombie.place", type="Normal", col=col, row=row)["native"]
        api("debug.zombie.set", board=board, id=z["DataID"], health=health,
            freezeTicks=100000 if frozen else 0)
        return z["DataID"]

    def clean():
        api("pause", paused=True)
        for z in zombies():
            api("zombie.remove", board=board, id=z["DataID"])
        for p in plants():
            api("plant.remove", board=board, id=p["DataID"])
        api("pause", paused=False)
        ticks(300)

    def screenshot(name):
        value = api("screenshot")
        shutil.copy2(value["path"], output / (name + ".png"))

    def get_plant(identity):
        return next(x for x in plants() if x["DataID"] == identity)

    try:
        state = api("status")
        assert state["ready"] and not state["board"], "Main menu required"
        backgrounds = api("debug.backgrounds")
        night = next(x for x in backgrounds if x["background"] == "Night")
        api("debug.start", index=night["index"])
        wait("chooser", lambda x: x["ready"])
        api("chooser.select", indices=[0], confirm=True)
        wait("chooser", lambda x: x["ready"] and x["zombiePhase"])
        screenshot("chooser")
        api("chooser.select", indices=[0], confirm=True)
        board = wait("status", lambda x: x["scene"] == "Playing")["board"]
        difficulty = api("debug.difficulty")["value"]

        def damage(value):
            return value + 10 if difficulty == 1 else value // 2 if difficulty == 5 else value

        recipes = [("Fumeshroom", "Chomper", "Fumeshroom", "Chomper", 300),
                   ("Scaredyshroom", "Chomper", "Scaredyshroom", "Chomper", 300),
                   ("Fumeshroom", "Cherrybomb", "Fumeshroom", "Cherrybomb", 300),
                   ("Iceshroom", "Jalapeno", "Iceshroom", "Jalapeno", 32000)]
        for left, right, seed, marker, health in recipes:
            for reverse in (False, True):
                clean()
                api("pause", paused=True)
                for ingredient in ((right, left) if reverse else (left, right)):
                    plant(ingredient)
                current = plants()
                assert len(current) == 1 and current[0]["mSeedType"] == seed and current[0]["mImitaterType"] == marker, current
                p = current[0]
                assert p["mPlantMaxHealth"] == health
                api("debug.plant.set", board=board, id=p["DataID"], health=100)
                api("debug.fertilize", board=board, col=2, row=2)
                assert get_plant(p["DataID"])["mPlantHealth"] == health
                api("pause", paused=False)
                print(seed, marker, "reverse" if reverse else "forward", "recipe/healing OK", flush=True)

        clean()
        belch = direct("Fumeshroom", "Chomper")
        far = target(6)
        ticks(180)
        far_before = next(z for z in zombies() if z["DataID"] == far)["mBodyHealth"]
        assert far_before < 10000, "Belch ordinary smoke must hit"
        close = target(2)
        wait("zombies", lambda x: not any(z["native"]["DataID"] == close for z in x["items"]))
        assert get_plant(belch["DataID"])["mTargetX"] > 3000
        before = next(z for z in zombies() if z["DataID"] == far)
        ticks(170)
        after = next(z for z in zombies() if z["DataID"] == far)
        assert before["mBodyHealth"] - after["mBodyHealth"] >= damage(40)
        assert after["mPosX"] > before["mPosX"], "Digesting smoke must push"
        screenshot("belch-digesting")
        api("pause", paused=True)
        api("debug.fertilize", board=board, col=2, row=2)
        assert get_plant(belch["DataID"])["mTargetX"] <= 50
        api("pause", paused=False)
        ticks(70)
        assert get_plant(belch["DataID"])["mTargetX"] == 0
        print("Belch swallow, smoke, knockback, fertilizer OK", flush=True)

        clean()
        hungry = direct("Scaredyshroom", "Chomper")
        near = [target(2, row) for row in (1, 2, 3, 2)]
        ticks(160)
        remaining = [z for z in zombies() if z["DataID"] in near]
        assert len(remaining) == 1, remaining
        assert get_plant(hungry["DataID"])["mTargetX"] > 3000
        for z in remaining:
            api("zombie.remove", board=board, id=z["DataID"])
        far = target(7)
        second_far = target(6)
        wait("projectiles", lambda x: any(-31203 <= shot["mCobTargetX"] <= -31201 for shot in x["items"]))
        ticks(220)
        assert next(z for z in zombies() if z["DataID"] == far)["mBodyHealth"] < 10000
        assert next(z for z in zombies() if z["DataID"] == second_far)["mBodyHealth"] < 10000
        screenshot("hungry-chunks")
        api("pause", paused=True)
        api("debug.fertilize", board=board, col=2, row=2)
        assert get_plant(hungry["DataID"])["mTargetX"] <= 50
        api("pause", paused=False)
        ticks(80)
        assert get_plant(hungry["DataID"])["mTargetX"] == 0
        wait("projectiles", lambda x: any(shot["mCobTargetX"] == -425040 for shot in x["items"]))
        print("Hungry three swallows, chunks, ordinary spores, fertilizer OK", flush=True)

        clean()
        direct("Fumeshroom", "Cherrybomb", col=0)
        direct("Peashooter", "Cherrybomb", col=2)
        far = target(8)
        wait("projectiles", lambda x: any(shot["mCobTargetX"] == -41011 for shot in x["items"]))
        ticks(300)
        health = next(z for z in zombies() if z["DataID"] == far)["mBodyHealth"]
        assert 10000 - health >= damage(80), health
        ordinary = wait("projectiles", lambda x: any(shot["mCobTargetX"] == -41001 and shot["mPosX"] < 400 for shot in x["items"]))
        pending = {shot["DataID"] for shot in ordinary["items"] if shot["mCobTargetX"] == -41001}
        api("zombie.remove", board=board, id=far)
        wait("projectiles", lambda x: any(shot["DataID"] in pending and shot["mCobTargetX"] == -41011 for shot in x["items"]))
        assert not zombies(), "Conversion must also run without a zombie"
        # Existing airborne cherries must keep their converted saved identity.
        screenshot("trigger-cherries")
        print("Trigger conversion and explosive impact OK", flush=True)

        clean()
        api("pause", paused=True)
        guard = direct("Iceshroom", "Jalapeno", col=0, row=0)
        protected = plant("Wallnut", col=4, row=2)[-1]
        api("debug.plant.set", board=board, id=protected["DataID"], health=1)
        enemy = target(4, frozen=False)
        api("pause", paused=False)
        ticks(160)
        assert get_plant(protected["DataID"])["mPlantHealth"] == 1, "Global guard must absorb bites"
        assert get_plant(guard["DataID"])["mPlantHealth"] < 25600
        api("zombie.remove", board=board, id=enemy)
        row_target = target(7, row=0)
        other_target = target(7, row=4)
        api("pause", paused=True)
        api("debug.plant.set", board=board, id=guard["DataID"], health=100)
        api("debug.fertilize", board=board, col=0, row=0)
        healed = get_plant(guard["DataID"])
        assert healed["mPlantHealth"] == 32000 and healed["mTargetX"] <= 50
        screenshot("spicicle-guard")
        api("debug.plant.set", board=board, id=guard["DataID"], health=100)
        api("pause", paused=False)
        ticks(75)
        assert not any(p["DataID"] == guard["DataID"] for p in plants())
        current = {z["DataID"]: z for z in zombies()}
        assert 10000 - current[row_target]["mBodyHealth"] == damage(1800) + damage(20), current[row_target]
        assert 10000 - current[other_target]["mBodyHealth"] == damage(20), current[other_target]
        assert current[other_target]["mIceTrapCounter"] > 0
        screenshot("spicicle-fire-ice")
        print("Spicicle global guard, decay, fertilizer, ordered fire/ice OK", flush=True)
        current_log = game_log.read_bytes()[log_start:].decode("utf-8", errors="replace")
        (output / "runtime.log").write_text(current_log)
        assert "[ERROR]" not in current_log and "Exception:" not in current_log, "Game log contains errors"
        results.append(dict(passed=True, gameLogErrors=0))
    finally:
        try:
            final = api("status")
            if final.get("board"):
                api("pause", paused=False)
            if final.get("debug"):
                api("level.leave")
                wait("status", lambda x: not x["debug"] and not x["board"])
        finally:
            (output / "runtime.json").write_text(json.dumps(results, indent=2))


if __name__ == "__main__":
    run()
