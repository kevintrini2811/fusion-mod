# Catalyst-shroom and Cryonic-shroom

REA 6.10.1 and Ghidra inspected PvZ Fusion English 4.0.5.2. GameAssembly SHA-256 is `48096b917ee6aaf6e35c95c98e666a4399a9e0c3d5adbab727cee941eccf2d42`. The receipts are in `../.test-runs/task1-20261120-pair-10` from the project root. Each result has one source recipe that accepts either ingredient order.

`Zombie.EatEffect` at `0x653420` consumes plant 1333 or 1041 and replaces the biter with a new ally of the appropriate type. The biter's water tile selects the Buckethead Ducky Tube fallback for either plant. Cryonic invokes `BoardAction.CreateFreeze` before removing the plant with a four-second freeze. Replanted uses the native Hypno-shroom's chew target and sleep rules. The replacement is allocated and placed before consuming the plant. Native hypnosis allows the difficulty mod to resist a transformation. Cryonic reuses the native Ice-shroom effect and the existing Ice Pumpkin interception.

Catalyst's Super Cherry Zombie uses a ZombieLibrary PeaHead definition with 720 body HP. `SuperCherryZ.Awake` at `0x62ed00` starts the first shot at three seconds. `AttributeEvent` at `0x62ece0` resets the interval to six seconds. The virtual bullet selector returns `Bullet_superCherry` (3), and `GetBulletDamage` at `0x62ed20` multiplies the ordinary pea damage by 15. The friendly shot inherits the existing exploding cherry impact and artwork. The native head firing path retains aiming, movement and the saved phase. The existing clock adapter measures the interval in source ticks.

Cryonic's land replacement uses the native Tall-nut zombie rig, 270 body HP and 2200 helmet HP. `TallIceNutZ.FirstArmorBroken` at `0x636bd0` switches armor sprites at two-thirds and one-third HP. The port reuses the Frost Tall-nut model for those stages and the native body for locomotion and death. The saved marker grants immunity to ordinary chill and freeze. A native burn attempt is passed to body damage as one-third of the source maximum health.

The plant views reuse Silver Hypno-shroom with separate tints. The Catalyst head uses the existing Explode-o-shooter head model. REA found no fusion-specific fertilizer behavior in `Fertilize.Upgrade`.
