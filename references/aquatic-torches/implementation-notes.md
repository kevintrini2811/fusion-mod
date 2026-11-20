# Torch-kelp and Torch Driftwood

Round 12 uses REA 6.10.1 against the English 4.0.5.2 GameAssembly SHA-256 `48096b917ee6aaf6e35c95c98e666a4399a9e0c3d5adbab727cee941eccf2d42`. Receipts are in the local round 12 test run. The source recipes are directed. Torch-kelp (1056) uses Tangle Kelp as the foundation and Torchwood as the incoming ingredient. Torch Driftwood (1430) uses Sea-shroom as the foundation and Torchwood as the incoming ingredient. Their saved identities use the native aquatic foundation with the Torchwood marker.

## Projectile conversion

`KelpTorch.OnTriggerEnter2D` at `0x180477920` and `TorchSeaShroom.OnTriggerEnter2D` at `0x180585f70` accept ordinary peas, small peas, snow peas, and kelp shots. `TorchWood.CheckFire` at `0x180515f50` filters projectile ownership and row according to source projectile mode. The port uses the existing row and swept crossing rule. The former two gain 20 damage through fireball conversion. Snow peas thaw without a damage bonus and still count. Fire conversion records the torch column to prevent a thawed pea from being counted again on the same crossing.

Kelp projectiles use their existing saved straight or burst flight, with a fire marker and the additional 20 damage. Direct and same-lane splash hits apply one kelp stack to water zombies and remove cold effects. Splash damage is divided across eligible secondary targets and capped at one third of the direct hit. REA records `Bullet_fireKelp.HitZombie` at `0x180706620`, `AttackOtherZombie` at `0x1807060d0`, and `HitLand` at `0x1807064f0`. The port uses a one-source-unit radius and Replanted's fireball impact and splat cues. The projectile keeps the shared kelp art with a warmer tint.

## Summons

Torch-kelp spends the counter after 101 conversions and searches to its right for the first legal aquatic cell (`0x180477b10`). It summons the source 1156 variant, represented by Tangle Kelp with the Cob Cannon marker. Native Kelp behaviour supplies the grab and drowning for this variant. The parent also retains the native Tangle Kelp grab.

Torch Driftwood spends the counter after 51 conversions. Its shipped description says over 100, while its trigger compares against `0x32` and calls the summon when strictly greater. The summon at `0x180586110` checks the immediate right cell. A legal empty cell gives the source 1431 variant, represented by Sea-shroom with the Cob Cannon marker. An existing child gains 20 shot damage and 4 percent visual scale, up to eleven upgrades. Its baseline shot is 20 with the shared 1.5-second Sea-shroom firing interval. A blocked cell or edge consumes the charge as in the source. The two distinct summoned identities have no recipe entries.

Each torch's native `mTargetX` saves its conversion count. The Sea-shroom child uses `mShootingCounter` for its level, with native shooting suppressed through the Sea family. Projectile type, damage and flight are in the native save fields. Formation and growth depend on conversion counts, so neither parent adds a fertilizer timer action. The plants reuse the existing Kelp, Tide-shroom and Minnow-shroom rig resources with warm coloring.
