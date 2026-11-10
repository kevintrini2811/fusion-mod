"""Exercise Boomwood charge, saved drone slots and original artwork through real shots."""
import json
import shutil
import time
from smoke_charge_torches import api, wait, OUT, EVENTS, CHECKS


def main():
    board = api("status")["board"]
    row = 3
    catalog = api("catalog.plants")["registered"]
    def plants(): return api("plants")["items"]
    def clear():
        api("pause", paused=True)
        for z in api("zombies")["items"]: api("zombie.remove", board=board, id=z["native"]["DataID"])
        for p in plants(): api("plant.remove", board=board, id=p["DataID"])
        api("speed", index=6); api("pause", paused=False)
        wait("projectiles", lambda v: not v["items"], 15)
        api("pause", paused=True); api("speed", index=0)
    def place(seed, col, marker=None):
        args = dict(col=col, row=row)
        if marker: args["registered"] = next(p["index"] for p in catalog if p["type"] == seed and p["marker"] == marker)
        else: args["type"] = seed
        return api("plant.place", **args)["plants"]
    def target():
        z = api("zombie.place", type="Normal", col=8, row=row)["native"]
        api("debug.zombie.set", board=board, id=z["DataID"], health=1000000, freezeTicks=1000000)
        return z

    clear(); place("Torchwood", 2, "Cherrybomb"); place("Repeater", 0); place("Tallnut", 7); target()
    owner = next(p for p in plants() if p["mSeedType"] == "Torchwood")
    api("speed", index=4); api("pause", paused=False)
    charges = set()
    end = time.monotonic() + 35
    while time.monotonic() < end:
        p = next(p for p in plants() if p["DataID"] == owner["DataID"])
        charges.add(p["mTargetX"])
        drones = [p for p in api("projectiles")["items"] if p["mCobTargetX"] == -624012]
        if drones:
            assert 30 in charges and all(p["mCobTargetRow"] == 300 for p in drones), (charges, drones)
            CHECKS.append(dict(ordinaryCharge=sorted(charges), drones=drones)); break
    else: raise AssertionError(("No natural 31-conversion summon", charges))
    print("PASS ordinary pea charge exceeds 30 and summons 300-damage cherry", flush=True)

    # Four real cherries are already in flight when their target is removed.
    # Three become stored drones; the fourth must pass the full rack unchanged.
    clear()
    for col in range(6): place("Peashooter", col, "ExplodeONut")
    z = target(); api("pause", paused=False)
    wait("projectiles", lambda v: len([p for p in v["items"] if p["mCobTargetX"] == -41010 and p["mPosX"] < 600]) >= 4)
    api("pause", paused=True)
    api("zombie.remove", board=board, id=z["DataID"])
    for p in plants():
        if p["mSeedType"] == "Peashooter": api("plant.remove", board=board, id=p["DataID"])
    place("Torchwood", 7, "Cherrybomb")
    api("pause", paused=False)
    def full(v):
        drones = [p for p in v["items"] if p["mCobTargetX"] == -624012]
        passed = [p for p in v["items"] if p["mCobTargetX"] == -41010 and p["mPosX"] > 690]
        return v if len(drones) == 3 and passed else None
    rack = wait("projectiles", full, 12, visuals=True)
    api("pause", paused=True)
    drones = [p for p in rack["items"] if p["mCobTargetX"] == -624012]
    assert {p["mHitTorchwoodGridX"] for p in drones} == {0, 1, 2}, drones
    assert all(p["mMotionType"] == "Puff" and any("CherryDroneProjectile" in r["path"] and r["enabled"] for r in p["visuals"]) for p in drones), drones
    CHECKS.append(dict(fullRack=rack))
    shutil.copy2(api("screenshot")["path"], OUT / "cherry-storage.png")
    print("PASS three saved drone slots, original stored graphics, full-rack passthrough", flush=True)
    z = target(); api("pause", paused=False)
    wait("zombies", lambda v: any(p["native"]["DataID"] == z["DataID"] and p["native"]["mBodyHealth"] <= 999100 for p in v["items"]), 15)
    api("pause", paused=True)
    CHECKS.append(dict(storedDroneHits=api("zombies")))
    print("PASS stored drones acquire new target and deal their saved damage", flush=True)


if __name__ == "__main__":
    try: main()
    finally: (OUT / "storage-runtime.json").write_text(json.dumps(dict(events=EVENTS, checks=CHECKS), indent=2) + "\n")
