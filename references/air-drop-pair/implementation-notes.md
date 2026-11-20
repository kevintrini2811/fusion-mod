# Fall Melon and Squash Drone

Blover and Melon-pult make Fall Melon. Blover and Squash make Squash Drone. Both orders use the shared fusion transaction. Plant Library registers `Blover/Melonpult` and `Blover/Squash` with `UnlockPolicy.Never`. `AirPlants` places each result independently above the ground plant on its tile.

## Source behavior

REA 6.10.1 with Ghidra 12.2 examined English Fusion 4.0.5.2. The GameAssembly SHA-256 is `48096b917ee6aaf6e35c95c98e666a4399a9e0c3d5adbab727cee941eccf2d42`. The source descriptions and selected method decompilations are in `pvzreplanted/.test-runs/task1-20261120-pair-04/`.

- Fall Melon (1361) inherits the short-range air lobber. `MelonBlover.GetBulletType` at RVA `0x4ce7c0` selects the melon projectile. `CabbageBlover.SearchZombie` checks for ground zombies in the same lane within two source units. `Shoot1` emits the three ballistic velocities at −2, 0 and 2 source units per second. The source description gives 80 damage per melon, three melons per attack and a three-second interval. Normal melon impacts use the existing radius and splash budget.
- Squash Drone (1374) searches a 0.6-unit circle in `SquashBlover.FixedUpdate` at RVA `0x4e9990`. The animation event `AnimSquash` at RVA `0x4e9650` finds every eligible zombie in the same lane within 0.7 source units and applies crush damage. The corresponding source description gives 100 damage per victim and a two-second interval. Water landings emit a splash and ordinary landings play the stomp and shake the board.
- `Fertilize.Upgrade` checks ordinary healing and the separate attribute countdown. Neither plant defines a fertilizer-specific effect. The Squash Drone trigger uses the attribute count field in the source, which is separate from the countdown.

## Replanted behavior

Fall Melon shares the saved air-lobber projectile marker and native lobbed motion. Its typed model branch extends the renderer validation introduced in 24.4.1. The common melon hit handler owns impact damage, splash particles and sound. Existing Spring Melon art supplies the plant body and the existing melon projectile supplies the three shots.

Squash Drone tracks its descent and recovery in a saved plant counter. The port assigns 20 ticks to descent and 30 ticks to recovery. Replanted ground hit rectangles approximate the source collider in the active lane. At impact, zombie IDs are captured before damage callbacks and checked again before each hit. The air tile stays anchored throughout the attack. The plant uses the existing mini-squash body.

Pause and carrying suspend the owned attack timers. The attack branches consume canonical source ticks and use the existing native serialization fields.
