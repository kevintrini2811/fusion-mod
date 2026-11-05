"""Verify the four pumpkin/glacier ports through the documented debug API."""
import json
from pathlib import Path
import shutil
import sys
import time

PROJECT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT.parent / "debug-mod/tools"))
from debug_api import call, GAME


def run(glacier_only=False):
    root = GAME / "UserData/DebugModApi"
    output = PROJECT / "references/pumpkin-glacier-21-3-0"
    results = []
    game_log = GAME / "MelonLoader/Latest.log"
    log_start = game_log.stat().st_size
    board = None
    old_speed = 0

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
        return wait("fps", lambda x: x["board"] == board and x["simulationTick"] - first >= count, seconds=max(30, count / 100 + 10))

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

        speed = api("speed")
        old_speed = speed["selected"]
        print("Speed:", speed, flush=True)
        # Use the native documented speed control and restore its prior index.
        api("speed", index=4)
        if not glacier_only:
            recipes = [
                ("Pumpkinshell", "Chomper", "Pumpkinshell", "Chomper", 4000),
                ("Pumpkinshell", "Potatomine", "Pumpkinshell", "Potatomine", 4000),
                ("Pumpkinshell", "Sunflower", "Pumpkinshell", "Sunflower", 4000),
                (("Doomshroom", "Iceshroom"), "Fumeshroom", "Fumeshroom", "Wintermelon", 300),
                ("Doomshroom", ("Fumeshroom", "Iceshroom"), "Fumeshroom", "Wintermelon", 300),
                ("Iceshroom", ("Fumeshroom", "Doomshroom"), "Fumeshroom", "Wintermelon", 300)]
            for left, right, seed, marker, health in recipes:
                for reverse in (False, True):
                    clean()
                    api("pause", paused=True)
                    for ingredient in ((right, left) if reverse else (left, right)):
                        if isinstance(ingredient, tuple): direct(*ingredient)
                        else: plant(ingredient)
                    current = plants()
                    assert len(current) == 1 and current[0]["mSeedType"] == seed and current[0]["mImitaterType"] == marker, current
                    p = current[0]
                    assert p["mPlantMaxHealth"] == health
                    api("debug.plant.set", board=board, id=p["DataID"], health=100)
                    api("debug.fertilize", board=board, col=2, row=2)
                    healed = get_plant(p["DataID"])
                    assert healed["mPlantHealth"] == health
                    if marker == "Chomper": assert healed["mLaunchCounter"] <= 50
                    if marker == "Sunflower": assert healed["mLaunchCounter"] == p["mLaunchCounter"]
                    api("pause", paused=False)
                    print(left, right, reverse, "recipe and fertilizer OK", flush=True)

            clean()
            chompkin = direct("Pumpkinshell", "Chomper")
            ticks(760)
            assert get_plant(chompkin["DataID"])["mTargetX"] == 1
            # A real native swallow starts digestion inside the stocked shell.
            chomper = plant("Chomper")
            chomper = next(p for p in chomper if p["mSeedType"] == "Chomper")
            prey = target(3, health=100)
            wait("plants", lambda x: any(p["DataID"] == chomper["DataID"] and p["mState"] == "ChomperDigesting" for p in x["items"]))
            wait("plants", lambda x: any(p["DataID"] == chompkin["DataID"] and p["mTargetX"] == 0 for p in x["items"]))
            ticks(80)
            assert get_plant(chomper["DataID"])["mState"] != "ChomperDigesting"
            screenshot("chompkin")
            ticks(12000)
            assert get_plant(chompkin["DataID"])["mTargetX"] == 3
            api("pause", paused=True)
            full = get_plant(chompkin["DataID"])
            api("debug.fertilize", board=board, col=2, row=2)
            assert get_plant(chompkin["DataID"])["mTargetY"] == full["mTargetY"]
            api("pause", paused=False)
            api("speed", index=0)
            api("plant.remove", board=board, id=chomper["DataID"])
            chompzilla = direct("Chomper", "Tallnut")
            target(3, health=100000)
            wait("plants", lambda x: any(p["DataID"] == chompzilla["DataID"] and p["mStateCountdown"] > 1000 for p in x["items"]))
            wait("plants", lambda x: any(p["DataID"] == chompkin["DataID"] and p["mTargetX"] < 3 for p in x["items"]))
            assert get_plant(chompzilla["DataID"])["mStateCountdown"] <= 50
            api("speed", index=4)
            print("Chompkin production, cap, native digestion, Chompzilla recharge and fertilizer OK", flush=True)

            clean()
            mine = direct("Pumpkinshell", "Potatomine")
            enemy = target(2)
            api("debug.plant.set", board=board, id=mine["DataID"], health=2600)
            ticks(5)
            first = next(z for z in zombies() if z["DataID"] == enemy)["mBodyHealth"]
            assert 10000 - first == damage(500), first
            api("debug.plant.set", board=board, id=mine["DataID"], health=1200)
            ticks(5)
            second = next(z for z in zombies() if z["DataID"] == enemy)["mBodyHealth"]
            assert first - second == damage(500)
            api("debug.fertilize", board=board, col=2, row=2)
            api("debug.plant.set", board=board, id=mine["DataID"], health=1200)
            ticks(5)
            assert next(z for z in zombies() if z["DataID"] == enemy)["mBodyHealth"] == second
            api("plant.remove", board=board, id=mine["DataID"])
            after = next(z for z in zombies() if z["DataID"] == enemy)["mBodyHealth"]
            assert second - after == damage(1800)
            screenshot("pumpkin-mine")
            print("Pumpkin Mine thresholds, fertilizer and death explosion OK", flush=True)

            clean()
            api("pause", paused=True)
            support = plant("Flowerpot")[-1]
            shooter = plant("Peashooter")[-1]
            sunny = direct("Pumpkinshell", "Sunflower")
            api("pause", paused=False)
            def shields():
                return {item["mGridX"]: item["mGridItemCounter"] for item in api("debug.items")["items"] if item.get("mSunCount") == -613250}
            ticks(2560)
            values = shields()
            for p in [support, shooter, sunny]: assert values[p["DataID"]] == 50, values
            ticks(2700)
            values = shields()
            for p in [support, shooter, sunny]: assert values[p["DataID"]] == 100, values
            api("plant.remove", board=board, id=sunny["DataID"])
            api("pause", paused=True)
            assert shields()[shooter["DataID"]] == 100
            eater = target(2, health=100000, frozen=False)
            api("speed", index=0)
            api("pause", paused=False)
            wait("debug.items", lambda x: any(item.get("mSunCount") == -613250 and item["mGridX"] == shooter["DataID"] and 0 < item["mGridItemCounter"] < 100 for item in x["items"]))
            assert shields().get(shooter["DataID"], 0) < 100
            assert get_plant(shooter["DataID"])["mPlantHealth"] == 300
            wait("plants", lambda x: any(p["DataID"] == shooter["DataID"] and p["mPlantHealth"] < 300 for p in x["items"]))
            screenshot("sunny-shield")
            print("Sunny Pumpkin production, stacked shields and real bite absorption OK", flush=True)

        api("speed", index=4)
        clean()
        api("speed", index=0)
        api("pause", paused=True)
        glacier = direct("Fumeshroom", "Wintermelon", col=0)
        frozen = target(5)
        api("pause", paused=False)
        ticks(100)
        api("pause", paused=True)
        burst = [shot for shot in shots() if -614002 <= shot["mCobTargetX"] <= -614000]
        assert len(burst) == 4, burst
        api("plant.remove", board=board, id=glacier["DataID"])
        api("pause", paused=False)
        ticks(220)
        loss = 10000 - next(z for z in zombies() if z["DataID"] == frozen)["mBodyHealth"]
        assert loss == 4 * (damage(80) + damage(10)), loss
        clean()
        api("pause", paused=True)
        glacier = direct("Fumeshroom", "Wintermelon", col=0)
        targets = [target(col) for col in (5,6,7,8)]
        api("pause", paused=False)
        ticks(100)
        api("pause", paused=True)
        burst = [shot for shot in shots() if -614002 <= shot["mCobTargetX"] <= -614000]
        assert len(burst) == 4, burst
        screenshot("glacier-shroom")
        api("plant.remove", board=board, id=glacier["DataID"])
        api("pause", paused=False)
        ticks(320)
        state = {z["DataID"]: z for z in zombies()}
        for identity in targets[:3]: assert 10000 - state[identity]["mBodyHealth"] >= 4 * damage(80)
        assert 10000 - state[targets[3]]["mBodyHealth"] <= 4 * damage(10)
        print("Glacier four shots, frozen multiplier, three contacts and half-damage splash OK", flush=True)
        api("speed", index=old_speed)
        current_log = game_log.read_bytes()[log_start:].decode("utf-8", errors="replace")
        (output / "runtime.log").write_text(current_log)
        assert "[ERROR]" not in current_log and "Exception:" not in current_log, "Game log contains errors"
        results.append(dict(passed=True, scope="glacier" if glacier_only else "all", gameLogErrors=0))
    finally:
        try:
            final = api("status")
            if final.get("board"):
                api("speed", index=old_speed)
                api("pause", paused=False)
            if final.get("debug"):
                api("level.leave")
                wait("status", lambda x: not x["debug"] and not x["board"])
        finally:
            (output / ("runtime-glacier.json" if glacier_only else "runtime.json")).write_text(json.dumps(results, indent=2))


if __name__ == "__main__":
    run(glacier_only="--glacier-only" in sys.argv)
