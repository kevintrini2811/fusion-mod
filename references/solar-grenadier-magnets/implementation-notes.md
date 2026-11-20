# Solar Magnet and Grenadier Magnet

The round 11 REA 6.10.1 receipts under `../.test-runs/task1-20261120-pair-11/` target the English 4.0.5.2 GameAssembly SHA-256 `48096b917ee6aaf6e35c95c98e666a4399a9e0c3d5adbab727cee941eccf2d42`. The source recipe archive lists Magnet-shroom plus Sunflower for Solar Magnet (1154) and Cherry Bomb plus Magnet-shroom for Grenadier Magnet (1166). Both ingredients already exist in Replanted. Each recipe accepts either order through the normal fusion transaction. The resulting native save identities are Sunflower/Magnetshroom and Magnetshroom/Cherrybomb. The existing Cherry Bomb ingredient explosion still happens when completing the Grenadier recipe.

## Solar Magnet

`SunMagnetShroom` initializes the attribute clock at 10 seconds at `0x1804c6790` and resets it to 30 seconds at `0x180551a80`. `0x180551410` selects one of six metal types with equal probability, then offers the item to an eligible immediate right-hand neighbor or leaves it loose. Grenadier receives it only when empty and ready. The source checks for Hypno Magnet and Ultimate Hypno as well. Those plants have no implemented recipient here. Both kinds of implemented delivery place the item at the right side in the same row. The rightmost cell constrains this placement to the board.

The shared Producer update sampled from `0x180550a00` gives one full 25-sun coin per nominal 25-second production period with 5-percent jitter. The independent metal timer uses `mTargetX` in centiseconds. Manual feeding consumes one item through the existing glove selection, gives one full sun and leaves both timers running. The source manual interaction is at `0x180551b00`.

## Grenadier Magnet

Equipment, placed ladders and unheld loose metal use the existing magnet acquisition boundary. The interval is 15 seconds with a four-source-unit range and two-row limit (`0x18049a6a0`). The manual acceptance guard at `0x18049a350` requires both an empty slot and an expired interval. A loaded item survives a cooldown expiry and empty target searches.

The attack searches start after one second loaded and repeat every three seconds (`0x1804990f0`, `0x18049a1c0`). The closest eligible zombie in the same row is chosen (`0x180499500`). The held item's native draw controller follows its saved world position. The item moves toward a fixed target snapshot at 40 source units per second. It detonates within one source unit for 300 explosive damage at the target position, then moves back at 80 percent of outbound speed. The port uses a fixed speed for the return leg and the native gear placement at the plant for rest. After five impacts, the item is removed and a cooldown above 0.5 seconds is capped (`0x180499b90`). Explosion damage runs after Zombie Library has restored the armor it temporarily exposes during magnet capture.

`mLaunchCounter` remains the attraction clock. `mShootingCounter` holds time until the next search and `mTargetX` counts impacts. Nonnegative `mStartRow` values retain the existing acquisition animation, with negative values denoting outbound and return. The serialized `MagnetItem` holds the current position and, during outbound flight, the fixed aim instead of a local resting offset. The holder and visible gear travel together through save and reload. Both fusions shorten a positive attribute timer above 0.5 seconds when fertilized, matching `Fertilize.Upgrade` at `0x180682810`.

The REA metadata maps `0x180499380` to `CherryMagnet.DieEventMustExecute`. Grenadier death destroys the held gear with the same source function used after the fifth impact, independent of how the gear was acquired.

## Metal and presentation

The shared loose-drop lifecycle persists generated gear with the type encoded in a distinct hidden GridItem marker. Its existing eight-second lifetime, highlight and controller pickup paths are reused. Pails and helmets retain their ordinary placement uses. Pickaxe, jack-in-the-box, the door representation for PortalHeart and the pogo representation for Machine are metal-only. The six variants have distinct native item types. The native metal controllers also provide drop and selection visuals and are disposed with their owner. Both plants reuse the existing magnet rig with distinct colors. The pair uses the existing native magnet and cherry cues and the shared cherry explosion particle.
