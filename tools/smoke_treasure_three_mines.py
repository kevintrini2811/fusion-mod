"""Exercise both mine ports through the real, temporary debug board."""
import json
from pathlib import Path
import shutil
import sys
import time

PROJECT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT.parent / "debug-mod/tools"))
from debug_api import GAME, call


def run():
    root = GAME / "UserData/DebugModApi"
    output = PROJECT / "references/treasure-three-mines-21-4-0"
    output.mkdir(parents=True, exist_ok=True)
    records = []
    checks = []
    board = None
    speed = None
    collector = None

    def api(command, **args):
        response = call(root, command, args)
        records.append(dict(command=command, args=args, response=response))
        assert response["ok"], response
        return response["result"]

    def wait(command, predicate, timeout=40):
        deadline = time.monotonic() + timeout
        while time.monotonic() < deadline:
            result = api(command)
            if predicate(result):
                return result
            time.sleep(.08)
        raise AssertionError(f"Timed out: {command}")

    def ticks(count):
        first = api("fps")["simulationTick"]
        wait("fps", lambda v: v["board"] == board and v["simulationTick"] - first >= count, max(30, count / 50))

    def plants():
        return api("plants")["items"]

    def zombies():
        return [z["native"] for z in api("zombies")["items"]]

    def clean():
        api("pause", paused=True)
        for z in zombies():
            api("zombie.remove", board=board, id=z["DataID"])
        for p in plants():
            api("plant.remove", board=board, id=p["DataID"])
        api("pause", paused=False)
        ticks(250)

    def place(seed, col=2, row=2):
        return api("plant.place", type=seed, col=col, row=row)["plants"]

    def direct(marker, col=2, row=2):
        item = next(x for x in api("catalog.plants")["registered"] if x["type"] == "Potatomine" and x["marker"] == marker)
        return api("plant.place", registered=item["index"], col=col, row=row)["plants"][-1]

    def target(col, row=2):
        z = api("zombie.place", type="Normal", col=col, row=row)["native"]
        api("debug.zombie.set", board=board, id=z["DataID"], health=10000, freezeTicks=100000)
        return z["DataID"]

    def screenshot(name):
        image = api("screenshot")
        shutil.copy2(image["path"], output / (name + ".png"))

    try:
        state = wait("status", lambda s: s["ready"] and s["debug"] and s["scene"] == "Playing", 90)
        board = state["board"]
        screenshot("direct-start")
        assert not plants(), "Start with a fresh --debug-level=Day session."
        api("difficulty", board=board, value=3)
        speed = api("speed")
        print("Speed:", speed, flush=True)
        api("speed", index=1)
        collector = api("suncollector")["enabled"]
        api("suncollector", enabled=False)
        for marker in ("Threepeater", "Marigold"):
            for pair in (("Potatomine", marker), (marker, "Potatomine")):
                clean()
                place(pair[0])
                result = place(pair[1])
                assert any(p["mSeedType"] == "Potatomine" and p["mImitaterType"] == marker for p in result)
                assert len(plants()) == 1
                checks.append(f"recipe {pair}")

        clean()
        mine = direct("Threepeater")
        api("debug.plant.set", board=board, id=mine["DataID"], health=100)
        api("debug.fertilize", board=board, col=2, row=2)
        value = next(p for p in plants() if p["DataID"] == mine["DataID"])
        assert value["mPlantHealth"] == 300 and value["mStateCountdown"] <= 50
        ticks(270)
        assert next(p for p in plants() if p["DataID"] == mine["DataID"])["mState"] == "PotatoArmed"
        checks.append("Threetato fertilizer heals and preserves animated rising")
        ids = [target(7, row) for row in (1, 2, 3)]
        transition = wait("projectiles", lambda v: any(p["mCobTargetX"] == -5331240 and p["mRow"] != 2
            and p["mMotionType"] == "Threepeater" and 0 < p["mProjectileAge"] < 12 for p in v["items"]))
        outer = next(p for p in transition["items"] if p["mCobTargetX"] == -5331240 and p["mRow"] != 2
            and p["mMotionType"] == "Threepeater" and 0 < p["mProjectileAge"] < 12)
        birth_y = mine["mY"] + 20
        assert 0 < abs(outer["mPosY"] - birth_y) < 40 and 0 < abs(outer["mVelY"]) < 3, outer
        screenshot("lane-transition")
        checks.append("outer potato projectiles start at the plant and use animated native lane transitions")
        ticks(350)
        damaged = {z["DataID"]: z["mBodyHealth"] for z in zombies()}
        assert len(plants()) == 1, "Unexpected plant appeared during controlled projectile test."
        assert all(0 < damaged[i] < 10000 and (10000 - damaged[i]) % 30 == 0 for i in ids), damaged
        screenshot("threetato-three-lanes")
        checks.append("30-damage potato projectiles hit all three lanes")
        victim = ids[1]
        api("pause", paused=True)
        api("debug.zombie.set", board=board, id=victim, health=10000)
        moved = api("zombie.move", board=board, id=victim, col=2, row=2)["native"]
        api("pause", paused=False)
        ticks(10)
        value = next(z for z in zombies() if z["DataID"] == victim)
        assert 4500 <= value["mBodyHealth"] <= 4600, value
        assert value["mPosX"] > moved["mPosX"] + 60, value
        value = next(p for p in plants() if p["DataID"] == mine["DataID"])
        assert value["mState"] == "Notready" and 1300 <= value["mStateCountdown"] <= 1500, value
        checks.append("5400-damage contact, one-tile knockback, 15-second reburial")
        clean()
        edge = direct("Threepeater", row=0)
        api("debug.fertilize", board=board, col=2, row=0)
        ticks(270)
        own, adjacent = target(7, row=0), target(7, row=1)
        ticks(500)
        edge_health = {z["DataID"]: z["mBodyHealth"] for z in zombies()}
        assert 0 < edge_health[own] < edge_health[adjacent] < 10000, edge_health
        assert all((10000 - edge_health[i]) % 30 == 0 for i in (own, adjacent)), edge_health
        checks.append("edge lane returns its third projectile after a saved 0.2-second delay")
        clean()

        mine = direct("Marigold")
        api("debug.plant.set", board=board, id=mine["DataID"], health=100)
        api("debug.fertilize", board=board, col=2, row=2)
        value = next(p for p in plants() if p["DataID"] == mine["DataID"])
        assert value["mPlantHealth"] == 300 and value["mLaunchCounter"] <= 50, value
        ticks(250)
        coins = api("coins")["items"]
        assert coins or zombies(), "Treasure event produced no reward."
        checks.append("Marigold fertilizer, full rise/give animation and real reward")
        screenshot("marigold-rewards")
        # Mark the native saved tile and observe its 1.5x cooldown rate.
        api("debug.treasure.set", board=board, col=2, row=2, enabled=True)
        first = next(p for p in plants() if p["DataID"] == mine["DataID"])
        t0 = api("fps")["simulationTick"]
        ticks(100)
        last = next(p for p in plants() if p["DataID"] == mine["DataID"])
        elapsed = api("fps")["simulationTick"] - t0
        delta = first["mLaunchCounter"] - last["mLaunchCounter"]
        assert 1.2 * elapsed <= delta <= 1.8 * elapsed, (delta, elapsed)
        checks.append("Treasure tile cooldown is 50% faster")
        # Exercise random real drops until the third packet destroys the mine.
        for _ in range(100):
            if not any(p["DataID"] == mine["DataID"] for p in plants()):
                break
            api("debug.fertilize", board=board, col=2, row=2)
            ticks(450)
        else:
            raise AssertionError("No third packet after 100 treasure cycles.")
        checks.append("Third real seed packet removes Marigold Mine")
        clean()
        mine = direct("Marigold")
        victim = target(2)
        api("plant.remove", board=board, id=mine["DataID"])
        assert next(z for z in zombies() if z["DataID"] == victim)["mBodyHealth"] == 8200
        checks.append("Marigold death explosion deals 1800 damage")
        print("PASS:", "; ".join(checks), flush=True)
    finally:
        try:
            if board:
                api("pause", paused=False)
                if collector is not None:
                    api("suncollector", enabled=collector)
                if speed:
                    api("speed", index=speed.get("selected", 0))
                api("level.leave")
        finally:
            (output / "runtime.json").write_text(json.dumps(dict(checks=checks, records=records), indent=2) + "\n")


if __name__ == "__main__":
    run()
