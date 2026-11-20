# Cactus Drone and Cherry Drone

Blover and Cactus make Cactus Drone. Blover and Cherry Bomb make Cherry Drone. Both orders use the shared fusion transaction and the independent air slot. Replanted stores the plants as `Blover/Cactus` and `Blover/Cherrybomb`. Plant Library registers both with `UnlockPolicy.Never`.

## Source contract

REA 6.10.1 and Ghidra 12.2 examined English Fusion 4.0.5.2. The GameAssembly SHA-256 is `48096b917ee6aaf6e35c95c98e666a4399a9e0c3d5adbab727cee941eccf2d42`. Method entry points come from the earlier REA metadata evidence. The current method exports are in `pvzreplanted/.test-runs/task1-20261120-pair-03/`. The bounded text reads for 1082 and 1244 are in the previous run.

- Cactus Drone is source plant 1082. `CactusBlover.Start` at RVA `0x493dd0` plays the placement cue, calls the same balloon action as Blover and clears fog. `SearchZombie` at `0x493a00` selects Balloon Zombies ahead in the same row. `Shoot1` at `0x493c40` creates the same source cactus projectile used by Cactus with the air-only flag. The source description supplies two shots of 30 damage every 1.5 seconds. The port schedules the second thorn ten ticks after the first.
- Cherry Drone is source plant 1244. `CherryBlover.FixedUpdate` at `0x497f40` looks for ground targets within three source units and one row. `MoveUpdate` at `0x498190` moves the head toward its target with a five-per-second interpolation factor. At less than half a source unit it creates a cherry explosion and starts the 15-second attribute cooldown. It then returns to the original head position. The source description specifies 300 damage within one tile of impact. `OnMove` at `0x498740` clears the pending target.
- `Fertilize.Upgrade` at `0x682810` applies ordinary healing and caps positive attribute cooldowns at half a second. Cherry Drone's cooldown uses that field. Cactus Drone's ordinary firing interval uses its attack clock.

## Replanted behavior

Cactus Drone uses the existing Blover effect and the shared Cactus projectile owner. The saved projectile marker selects 30 damage with the native Flying damage range. The remaining salvo and firing phase use saved plant counters. Cherry Drone uses the target's full native DataID and saved board-relative movement offsets. Pause and carrying suspend both abilities. Relocating Cherry Drone clears its target and restarts the visual offset at the new tile.

Cherry Drone uses the current zombie hit rectangles and the existing small cherry explosion path. Completing a fusion with a fresh Cherry Bomb still runs the shared ingredient explosion. The drone's own 300-damage blast has a separate attack cooldown.

Cactus Drone uses the existing Ball Cactus body and spike model. Source `Bullet_cactus.HitZombie` calls the shared bullet impact sound. The port uses Replanted's throw and splat cues and pea-splat particles. Cherry Drone uses the existing flying cherry model. Both bodies lift above the ground layer.
