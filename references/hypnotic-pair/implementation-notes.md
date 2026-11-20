# Trippy-shroom and Comet-shroom

REA 6.10.1 and Ghidra inspected PvZ Fusion English 4.0.5.2. GameAssembly SHA-256 is `48096b917ee6aaf6e35c95c98e666a4399a9e0c3d5adbab727cee941eccf2d42`. Native evidence and localized descriptions are in `../.test-runs/task1-20261120-pair-09` from the project root. Both recipes work in either ingredient order.

## Trippy-shroom

`ScaredyHypno.Shoot1` at `0x56f660` creates `Bullet_puffRandomColor` (13). The bullet selects one of seven colors uniformly. `HitZombie` at `0x821d60` records the color before applying damage. Hits before completion deal 40 damage, including repeated colors. The hit that completes the set hypnotizes the zombie and skips damage. `Zombie.EatEffect` at `0x653420` handles plant ID 1024 by hypnotizing the eater and placing plain Scaredy-shroom in the same cell.

The native Scaredy-shroom adapter retains the family fear, sleep and shooting schedule. A projectile marker carries the color. A hidden native grid item saves each zombie's seven-color mask under the full zombie DataID. The record expires when its owner dies, turns friendly or leaves the board. The imported Speedy-shroom body and spore are tinted for this port. PlantLibrary dispatches native Puff Splat and Splat at impact. The source's Charm-shroom synergy needs that plant's own shot behavior when it becomes available.

## Comet-shroom

`SunHypno.DieEvent` at `0x551290` instantiates `JumpSun` with the plant's attack damage. `Zombie.EatEffect` at `0x653420` handles plant ID 1326 by hypnotizing the eater before removing the plant. `JumpSun.Start` at `0x451920` snapshots the board's zombies and sorts hypnotized zombies first. `JumpSun.Update` at `0x451b90` pursues each target in order, removes each entry after impact and uses current allegiance to determine damage and sun. The constructor at `0x452140` provides 200 base damage, ten hits and four source units per second. Hits use shield-bypassing damage. `JumpSun.Die` at `0x451830` drops a final 15 sun.

Comet reuses the Sunflower production controller for 25 sun on the nominal 25-second cycle with the source producer's five-percent jitter. The flight retains a board-scoped queue of zombie DataIDs, so reused slots and later spawns cannot join. In-flight suns follow the existing transient Fusion projectile load rule. The imported Silver Hypno-shroom body and Solar Blover projectile provide the reused artwork. Impacts use the native Star Splat, Chime and Splat.
