"""Verify base selections automatically use library ducks, including armored Eyes."""
import json
import shutil
import time
import sys
from smoke_charge_torches import api, wait, OUT, EVENTS, CHECKS, GAME


def main(native_only=False):
    status = api("status")
    assert status["debug"] and status["scene"] == "Playing"
    board = status["board"]
    api("pause", paused=True)
    while live := api("zombies")["items"]: api("zombie.remove", board=board, id=live[0]["native"]["DataID"])
    catalog = api("catalog.zombies")
    assert all(z["type"] != "DuckyTube" for z in catalog["debug"]), catalog
    assert len(catalog["registered"]) == (0 if native_only else 4), catalog
    sightings = GAME / "UserData/ZombieLibrary/sightings.json"
    before = sightings.read_bytes() if sightings.exists() else None
    kinds = ["Newspaper", "Door", "Bobsled", "Pogo", "Ladder", "Imp", "Target", "TrashCan",
             "Football", "JackInTheBox", "Dancer", "BackupDancer", "Yeti", "Gargantuar", "RedeyeGargantuar", "SquashHead"]
    ids = []
    for index, kind in enumerate(kinds):
        z = api("zombie.place", type=kind, col=2 + index % 6, row=2 + index % 2)["native"]
        ids.append(z["DataID"])
    api("pause", paused=False)
    data = wait("zombies", lambda v: v if all(any(z["native"]["DataID"] == id and z["native"]["mInPool"] for z in v["items"]) for id in ids) else None, 12)
    time.sleep(2)
    api("pause", paused=True)
    CHECKS.append(dict(nativeDucks=data))
    if native_only:
        visuals = api("zombies", visuals=True)
        CHECKS.append(dict(nativePoolVisuals=visuals))
        CHECKS.append(dict(nativeDuckRenderers=[dict(type=i["state"]["native"]["mZombieType"], renderers=i["rendering"]["renderers"]) for i in visuals["items"]]))
        for item in visuals["items"]:
            if item["state"]["native"]["DataID"] not in ids: continue
            graphics = item["rendering"]
            assert any(r["name"] == "Fusion-NativeDuckTube" and r["enabled"] and r["active"] for r in graphics["renderers"]) or any(
                "duckytube" in (s["attachment"] or "").lower() and s["alpha"] > 0 for s in graphics["slots"]), item["state"]
        shutil.copy2(api("screenshot")["path"], OUT / "library-ducks-without-fusion.png")
    while live := api("zombies")["items"]: api("zombie.remove", board=board, id=live[0]["native"]["DataID"])
    balloon = api("zombie.place", type="Balloon", col=5, row=3)["native"]
    api("debug.balloon.pop", board=board, id=balloon["DataID"])
    api("pause", paused=False)
    landed = wait("zombies", lambda v: v if any(z["native"]["DataID"] == balloon["DataID"] and z["native"]["mInPool"] for z in v["items"]) else None, 12)
    CHECKS.append(dict(balloonDuck=landed)); api("pause", paused=True)
    api("zombie.remove", board=board, id=balloon["DataID"])
    if native_only:
        assert (sightings.read_bytes() if sightings.exists() else None) == before
        print("PASS all 17 additional native duck types without Plant Fusion", flush=True)
        return
    for index in range(3): api("zombie.place", registered=index, col=3 + index * 2, row=2)
    api("pause", paused=False)
    wait("zombies", lambda v: len(v["items"]) == 3 and all(z["native"]["mInPool"] for z in v["items"]), 8)
    time.sleep(1); api("pause", paused=True)
    data = api("zombies", visuals=True)
    torso_heights = []
    for item in data["items"]:
        z = item["state"]["native"]
        assert z["mGraveX"] == -521012 and z["mInPool"], z
        if z["mZombieType"] != "Normal": assert z["mHelmHealth"] > 0, z
        renderers = item["rendering"]["renderers"]
        assert any(r["name"] == "anim_head1-sprite" and r["enabled"] and r["active"] for r in renderers), (z["mZombieType"], renderers)
        assert any(r["name"] == "Fusion-NativeDuckTube" and r["enabled"] and r["active"] for r in renderers), (z["mZombieType"], renderers)
        torso_heights.append(next(r["y"] for r in renderers if r["name"] == "Zombie_body-sprite"))
    assert max(torso_heights) - min(torso_heights) < 40, ("Imported pool bodies have inconsistent ground anchors", torso_heights)
    CHECKS.append(dict(eyesDucks=data))
    assert (sightings.read_bytes() if sightings.exists() else None) == before, "Debug spawning changed permanent sightings"
    shutil.copy2(api("screenshot")["path"], OUT / "eyes-ducks.png")
    print("PASS native base zombies automatically swim; all three Eyes variants retain armor and ducks", flush=True)


if __name__ == "__main__":
    native_only = "--native-only" in sys.argv
    try: main(native_only)
    finally: (OUT / ("pool-library-only-runtime.json" if native_only else "pool-runtime.json")).write_text(json.dumps(dict(events=EVENTS, checks=CHECKS), indent=2) + "\n")
