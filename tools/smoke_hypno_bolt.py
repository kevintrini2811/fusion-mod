"""Verify Hypno Chomper and Bolt-pult on an already running temporary Night board.

Never starts a game, changes profiles, or opens ordinary levels. Saves API evidence
and restores session settings even after a failed assertion.
"""
import json
from pathlib import Path
import shutil
import sys
import time

PROJECT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT.parent / "debug-mod/tools"))
from debug_api import GAME, call

API = GAME / "UserData/DebugModApi"
OUT = PROJECT / "references/hypno-bolt-22-4-0"
EVENTS, CHECKS = [], []


def api(command, **args):
    response = call(API, command, args)
    EVENTS.append(dict(command=command, args=args, response=response))
    assert response["ok"], response
    return response["result"]


def wait(command, predicate, seconds=30, **args):
    end = time.monotonic() + seconds
    last = None
    while time.monotonic() < end:
        last = api(command, **args)
        if result := predicate(last):
            return result
        time.sleep(.04)
    raise AssertionError(("Timed out", command, last))


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    state = api("status")
    assert state["debug"] and state["scene"] == "Playing", state
    board = state["board"]
    original_speed = api("speed")["selected"]
    original_pause = api("status").get("paused", False)
    difficulty = api("difficulty")
    collector = api("suncollector")["enabled"]
    catalog = api("catalog.plants")["registered"]
    identities = {
        "HypnoChomper": ("Chomper", "Hypnoshroom"),
        "BoltPult": ("Kernelpult", "Wallnut"),
        "KernelMorph": ("Kernelpult", "Magnetshroom"),
        "Bucket": ("Potatomine", "Magnetshroom"),
    }

    def plants():
        return api("plants")["items"]

    def zombies():
        return [item["native"] for item in api("zombies")["items"]]

    def at_identity(value, name):
        return (value["mSeedType"], value["mImitaterType"]) == identities[name]

    def place(name, col=2, row=1):
        args = dict(col=col, row=row)
        if name in identities:
            seed, marker = identities[name]
            args["registered"] = next(p["index"] for p in catalog if (p["type"], p["marker"]) == (seed, marker))
        else:
            args["type"] = name
        result = api("plant.place", **args)["plants"]
        return result

    def direct(name, col=2, row=1):
        return next(p for p in place(name, col, row) if at_identity(p, name))

    def snapshot(identifier):
        return next(p for p in plants() if p["DataID"] == identifier)

    def zombie(identifier):
        return next((z for z in zombies() if z["DataID"] == identifier), None)

    def target(kind="Normal", col=7, row=1, health=1000000, frozen=True):
        z = api("zombie.place", type=kind, col=col, row=row)["native"]
        api("debug.zombie.set", board=board, id=z["DataID"], health=health,
            freezeTicks=1000000 if frozen else 0)
        return zombie(z["DataID"])

    def ticks(count):
        first = api("fps")["simulationTick"]
        return wait("fps", lambda value: value["board"] == board and value["simulationTick"] - first >= count,
                    max(30, count / 50))

    def clear():
        api("pause", paused=True)
        for z in zombies():
            api("zombie.remove", board=board, id=z["DataID"])
        for p in plants():
            api("plant.remove", board=board, id=p["DataID"])
        api("speed", index=6)
        api("pause", paused=False)
        wait("projectiles", lambda value: not value["items"])
        api("pause", paused=True)
        api("speed", index=0)

    def shot_image(name):
        screenshot = api("screenshot")
        shutil.copy2(screenshot["path"], OUT / (name + ".png"))

    try:
        api("difficulty", board=board, value=3)
        api("suncollector", enabled=False)
        # Both seed orders and the remote glove transaction consume precisely
        # the two ingredients. Compatible supports retain their own identity.
        for result, pair in (("HypnoChomper", ("Chomper", "Hypnoshroom")),
                             ("BoltPult", ("KernelMorph", "Bucket"))):
            for first, second in (pair, tuple(reversed(pair))):
                clear()
                # Chomper and Kernel Morph can coexist with an ordinary pot.
                # Hypno-shroom + pot and Chomper + pumpkin are separate recipes.
                supported = first in ("Chomper", "KernelMorph")
                if supported:
                    place("Flowerpot")
                place(first)
                place(second)
                current = plants()
                fused = next(p for p in current if at_identity(p, result))
                assert len(current) == (2 if supported else 1), current
                if supported:
                    assert any(p["mSeedType"] == "Flowerpot" for p in current), current
                CHECKS.append(dict(test="recipe", order=[first, second], result=fused))
            for first, second in (pair, tuple(reversed(pair))):
                clear()
                place(first)
                source = place(second, col=0, row=3)[-1]
                api("plant.move", board=board, id=source["DataID"], col=2, row=1)
                current = plants()
                assert len(current) == 1 and at_identity(current[0], result), current
                assert current[0]["DataID"] != source["DataID"], current
                CHECKS.append(dict(test="glove", order=[first, second], result=current))
            print("PASS seed and glove recipes", result, flush=True)

        for name, expected_health in (("HypnoChomper", 300), ("BoltPult", 2000)):
            clear()
            p = direct(name)
            api("debug.plant.set", board=board, id=p["DataID"], health=100)
            api("debug.fertilize", board=board, col=2, row=1)
            healed = snapshot(p["DataID"])
            assert healed["mPlantHealth"] == healed["mPlantMaxHealth"] == expected_health, healed
            api("pause", paused=False)
            ticks(400)
            api("pause", paused=True)
            visual = next(q for q in api("plants", visuals=True)["items"] if q["DataID"] == p["DataID"])
            renderers = visual["visuals"]["renderers"]
            model_name = "hypno-chomper" if name == "HypnoChomper" else "bolt-pult"
            nodes = json.loads((PROJECT / "assets" / model_name / "model.json").read_text())["Nodes"]
            sprite_names = {n["Name"] for n in nodes if n["Sprite"] >= 0}
            assert any(r["name"] in sprite_names and r["renderer"] == "SpriteRenderer"
                       and r["enabled"] and r["active"] for r in renderers), visual
            CHECKS.append(dict(test="fertilizer-and-art", name=name, plant=healed, visual=visual))
            shot_image(name + "-body")
            print("PASS healing and original body", name, flush=True)

        # Bucket repair uses the actual ingredient path, preserving ability state.
        clear()
        p = direct("BoltPult")
        api("debug.plant.set", board=board, id=p["DataID"], health=100)
        place("Bucket")
        repaired = snapshot(p["DataID"])
        assert repaired["mPlantHealth"] == 600 and len(plants()) == 1, repaired
        assert repaired["mStartRow"] == p["mStartRow"], repaired
        CHECKS.append(dict(test="bucket-repair", plant=repaired))

        # Pause a real launched shot, then remove its owner so only one impact
        # contributes to the damage assertion. No projectile state is injected.
        seen = set()
        for attempt in range(40):
            clear()
            p = direct("BoltPult", col=0)
            z = target("Pail", col=7)
            api("pause", paused=False)
            shot = wait("projectiles", lambda value: next((q for q in value["items"]
                        if q["mCobTargetX"] in (-520400, -520420)), None), visuals=True)
            api("pause", paused=True)
            family = "kernel" if shot["mCobTargetX"] == -520400 else "butter"
            expected_art = "IronKernelProjectile" if family == "kernel" else "IronButterProjectile"
            # Give the render owner a frame if the birth and API request share a tick.
            if not any(expected_art in r["path"] and r["enabled"] and r["active"] for r in shot["visuals"]):
                time.sleep(.1)
                shot = next(q for q in api("projectiles", visuals=True)["items"] if q["DataID"] == shot["DataID"])
            assert any(expected_art in r["path"] and r["enabled"] and r["active"] for r in shot["visuals"]), shot
            assert shot["mCobTargetRow"] == (80 if family == "kernel" else 100), shot
            before = zombie(z["DataID"])
            api("plant.remove", board=board, id=p["DataID"])
            if family not in seen:
                shot_image("bolt-" + family)
            api("pause", paused=False)
            after = wait("zombies", lambda value: next((q["native"] for q in value["items"]
                         if q["native"]["DataID"] == z["DataID"] and
                         q["native"]["mHelmHealth"] < before["mHelmHealth"]), None))
            api("pause", paused=True)
            after = zombie(z["DataID"])
            if family == "kernel":
                assert before["mHelmHealth"] - after["mHelmHealth"] == 80, (before, after)
                assert after["mBodyHealth"] == before["mBodyHealth"], (before, after)
            else:
                assert after["mHelmHealth"] == 0, (before, after)
                assert before["mBodyHealth"] - after["mBodyHealth"] == 100, (before, after)
                assert 350 <= after["mButteredCounter"] <= 400, after
            CHECKS.append(dict(test="bolt-impact", family=family, shot=shot, before=before, after=after))
            seen.add(family)
            if seen == {"kernel", "butter"}:
                break
        assert seen == {"kernel", "butter"}, seen
        print("PASS both original projectiles, exact armor/body damage and butter", flush=True)
        # Normal meals use full digestion and do not create the allied reward.
        clear()
        p = direct("HypnoChomper")
        victim = target(col=3, health=270)
        before_coins = {c["DataID"] for c in api("coins")["items"]}
        api("pause", paused=False)
        digest = wait("plants", lambda value: next((q for q in value["items"]
                      if q["DataID"] == p["DataID"] and q["mState"] == "ChomperDigesting"), None))
        api("pause", paused=True)
        assert 3900 <= digest["mStateCountdown"] <= 4000, digest
        assert zombie(victim["DataID"]) is None or zombie(victim["DataID"])["mDead"], victim
        assert not [c for c in api("coins")["items"] if c["DataID"] not in before_coins and
                    c.get("mUsableSeedType") == "Hypnoshroom"], "Enemy meal must not reward a packet"
        CHECKS.append(dict(test="enemy-meal", digestion=digest))
        api("debug.fertilize", board=board, col=2, row=1)
        accelerated = snapshot(p["DataID"])
        assert accelerated["mStateCountdown"] <= 50, accelerated
        api("pause", paused=False)
        wait("plants", lambda value: next((q for q in value["items"] if q["DataID"] == p["DataID"]
             and q["mState"] != "ChomperDigesting"), None))
        print("PASS normal meal and real fertilizer digestion cap", flush=True)

        # Produce a friendly zombie through the native hypnosis bite, then move
        # that real unit into range. No mind-control flag is assigned by a test.
        clear()
        place("Hypnoshroom", col=5)
        friend = target(col=5, health=270, frozen=False)
        api("pause", paused=False)
        friend = wait("zombies", lambda value: next((q["native"] for q in value["items"]
                      if q["native"]["DataID"] == friend["DataID"] and q["native"]["mMindControlled"]), None))
        api("pause", paused=True)
        api("zombie.move", board=board, id=friend["DataID"], col=2, row=1)
        p = direct("HypnoChomper")
        before_coins = {c["DataID"] for c in api("coins")["items"]}
        api("pause", paused=False)
        digest = wait("plants", lambda value: next((q for q in value["items"]
                      if q["DataID"] == p["DataID"] and q["mState"] == "ChomperDigesting"), None))
        api("pause", paused=True)
        assert 900 <= digest["mStateCountdown"] <= 1000, digest
        packets = [c for c in api("coins")["items"] if c["DataID"] not in before_coins
                   and c.get("mUsableSeedType") == "Hypnoshroom"]
        assert len(packets) == 1, packets
        CHECKS.append(dict(test="allied-meal", hypnosis=friend, digestion=digest, packets=packets))
        shot_image("hypno-allied-meal")
        print("PASS allied meal, short digestion and exactly one native packet", flush=True)

        # A chewing Hypno Chomper can still be bitten. Allocate the replacement
        # before consumption and preserve the separate flowerpot support.
        clear()
        support = place("Flowerpot")[0]
        p = direct("HypnoChomper")
        target(col=3, health=270)
        api("pause", paused=False)
        wait("plants", lambda value: next((q for q in value["items"] if q["DataID"] == p["DataID"]
             and q["mState"] == "ChomperDigesting"), None))
        api("pause", paused=True)
        attacker = target(col=2, health=270, frozen=False)
        api("pause", paused=False)
        controlled = wait("zombies", lambda value: next((q["native"] for q in value["items"]
                          if q["native"]["DataID"] == attacker["DataID"] and q["native"]["mMindControlled"]), None))
        api("pause", paused=True)
        current = plants()
        assert not any(q["DataID"] == p["DataID"] for q in current), current
        assert any(q["mSeedType"] == "Chomper" and q["mImitaterType"] == "None" for q in current), current
        assert any(q["DataID"] == support["DataID"] for q in current), current
        CHECKS.append(dict(test="hypno-bite-and-replacement", zombie=controlled, plants=current))
        print("PASS real bite hypnosis and transactional replacement", flush=True)
        clear()
        CHECKS.append(dict(test="session-core", passed=True))
    finally:
        try:
            api("pause", paused=False)
            api("speed", index=original_speed)
            api("suncollector", enabled=collector)
            if isinstance(difficulty.get("value"), int):
                api("difficulty", board=board, value=difficulty["value"])
            api("pause", paused=original_pause)
        finally:
            (OUT / "runtime.json").write_text(json.dumps(dict(checks=CHECKS, events=EVENTS), indent=2))


if __name__ == "__main__":
    main()
