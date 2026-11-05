"""Explicit live test of the two ports and zombie UI in an already running game.

Requires the main menu. Starts and leaves only temporary debug levels. Does not
start the game, alter settings, or request profile/save writes.
"""
import json
from pathlib import Path
import sys
import time

PROJECT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT.parent / "debug-mod/tools"))
from debug_api import call, GAME


def run():
    root = GAME / "UserData/DebugModApi"
    results = []
    output = PROJECT / "references/puff-duo-21-1-0/runtime.json"
    board = None

    def api(command, args=None, error=None):
        response = call(root, command, args or {})
        results.append(dict(command=command, args=args or {}, response=response))
        if error:
            assert response.get("error", {}).get("code") == error, response
        else:
            assert response["ok"], response
        return response.get("result")

    def wait(command, predicate, seconds=30):
        deadline = time.monotonic() + seconds
        while time.monotonic() < deadline:
            response = call(root, command, {})
            if response.get("ok") and predicate(response["result"]):
                results.append(dict(command=command, response=response))
                return response["result"]
            time.sleep(.05)
        results.append(dict(command=command, response=response, timeout=True))
        raise AssertionError(f"Timeout: {command}")

    def ticks(count):
        first = api("fps")
        return wait("fps", lambda value: value["board"] == first["board"]
                    and value["simulationTick"] - first["simulationTick"] >= count)

    def plant(seed, col=2, row=2):
        return api("plant.place", dict(type=seed, col=col, row=row))["plants"]

    def clean():
        api("pause", dict(paused=True))
        for zombie in api("zombies")["items"]:
            api("zombie.remove", dict(board=board, id=zombie["native"]["DataID"]))
        for value in api("plants")["items"]:
            api("plant.remove", dict(board=board, id=value["DataID"]))
        api("pause", dict(paused=False))
        ticks(200)

    def target(kind, col, row):
        value = api("zombie.place", dict(type=kind, col=col, row=row))["native"]
        return api("debug.zombie.set", dict(board=board, id=value["DataID"], health=10000, freezeTicks=100000))["native"]

    def zombie(identity):
        return next(item["native"] for item in api("zombies")["items"] if item["native"]["DataID"] == identity)

    def direct(marker, col, row):
        catalog = api("catalog.plants")["registered"]
        entry = next(item for item in catalog if item["type"] == "Puffshroom" and item["marker"] == marker)
        return api("plant.place", dict(registered=entry["index"], col=col, row=row))["plants"][-1]

    try:
        state = api("status")
        assert state["ready"] and not state["board"], "The main menu is required."
        backgrounds = api("debug.backgrounds")
        night = next(item for item in backgrounds if item["background"] == "Night")
        api("debug.start", dict(index=night["index"]))
        wait("chooser", lambda value: value["ready"])
        board = api("status")["board"]
        api("chooser.select", dict(indices=[0], confirm=True))
        chooser = wait("chooser", lambda value: value["zombiePhase"] and value["ready"])
        count = len(api("catalog.zombies")["native"])
        selected = [0] + [item["index"] for item in chooser["choices"] if item["index"] >= count][:3]
        api("chooser.select", dict(indices=selected, confirm=False))
        time.sleep(.3)
        ui = api("debug.ui")
        assert any((item["sprite"] or "").startswith("DebugZombiePacket.") for item in ui)
        api("screenshot")
        api("chooser.select", dict(indices=selected, confirm=True))
        state = wait("status", lambda value: value["scene"] == "Playing")
        board = state["board"]
        difficulty = api("debug.difficulty")["value"]
        def effective_damage(damage):
            return damage + 10 if difficulty == 1 else damage // 2 if difficulty == 5 else damage
        wait("debug.ui", lambda items: any("FusionDebugUI" in item["path"] and item["sprite"] ==
             "DebugZombieChooser.SPR_MainHud_SeedBank_SunAmount" for item in items))
        api("screenshot")

        for ingredient, marker in (("Cherrybomb", "Cherrybomb"), ("Cactus", "Cactus")):
            for reverse in (False, True):
                clean()
                api("pause", dict(paused=True))
                for seed in ((ingredient, "Puffshroom") if reverse else ("Puffshroom", ingredient)):
                    plant(seed)
                values = api("plants")["items"]
                assert len(values) == 1 and values[0]["mSeedType"] == "Puffshroom" and values[0]["mImitaterType"] == marker
                value = values[0]
                api("debug.plant.set", dict(board=board, id=value["DataID"], health=100))
                assert api("debug.fertilize", dict(board=board, col=2, row=2))["applied"]
                healed = api("plants")["items"][0]
                assert healed["mPlantHealth"] == 300 and healed["mImitaterType"] == marker
                api("debug.fertilize", dict(board="stale", col=2, row=2), error="STALE_BOARD")
                api("pause", dict(paused=False))
                ticks(100)
                assert len(api("plants")["items"]) == 1
                print(ingredient, "reverse" if reverse else "forward", "fertilizer OK", flush=True)

        for kind, row in (("Normal", 1), ("Balloon", 3)):
            clean()
            victim = target(kind, 6, row)
            direct("Cactus", 1, row)
            wait("zombies", lambda items: next(item["native"] for item in items["items"]
                 if item["native"]["DataID"] == victim["DataID"])["mBodyMaxHealth"] == 7000)
            ticks(200)
            after = zombie(victim["DataID"])
            assert after["mBodyMaxHealth"] == 7000 and after["mBodyHealth"] < 7000
            if kind == "Balloon":
                assert after["mFlyingMaxHealth"] == 14, "Shrink must include separate balloon armor."
                assert after["mFlyingHealth"] == 0 and after["mZombiePhase"] != "BalloonFlying", "Native balloon armor must pop after depletion."
            api("screenshot")
            print(kind, "damage, shrink and repeat-hit OK", flush=True)

        clean()
        api("pause", dict(paused=True))
        victim = target("Normal", 4, 2)
        pop = direct("Cherrybomb", 3, 2)
        api("plant.remove", dict(board=board, id=pop["DataID"]))
        assert zombie(victim["DataID"])["mBodyHealth"] == 10000 - effective_damage(900)
        bombs = [shot for shot in api("projectiles")["items"] if shot.get("mCobTargetX") == -41021]
        assert len(bombs) == 1 and bombs[0]["mCobTargetRow"] == 1125
        api("screenshot")
        api("pause", dict(paused=False))
        ticks(650)
        assert zombie(victim["DataID"])["mBodyHealth"] == 10000 - effective_damage(900) - effective_damage(1125)
        print("Cherry Pop initial blast, charge and delayed explosion OK", flush=True)
        clean()
        api("level.leave")
        wait("status", lambda value: value["ready"] and not value["board"])
        results.append(dict(passed=True))
    except Exception as error:
        results.append(dict(passed=False, error=str(error)))
        raise
    finally:
        if board is not None:
            try:
                state = api("status")
                if state["debug"] and state["board"] == board:
                    if state["scene"] == "Playing":
                        api("pause", dict(paused=False))
                    api("level.leave")
            except Exception:
                pass
        output.parent.mkdir(parents=True, exist_ok=True)
        output.write_text(json.dumps(results, ensure_ascii=False, indent=2) + "\n")


if __name__ == "__main__":
    run()
