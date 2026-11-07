# Bolt-pult port

Bolt-pult uses source ID 1211 (`IronCorn`) from Fusion 4.0.5.2 in the supplied `english-4.0.5.2` directory. Its sole source recipe is **Kernel Morph + Bucket**. Both ingredients are implemented and usable before this port. Kernel Morph is a full plant variant; Bucket comes from the existing selectable metal drops and native captured magnet items. The shared unordered recipe resolver also accepts the reverse input order. Bolt-pult has no native Replanted equivalent.

## Source contract

The original Chinese almanac in `almanac.json` gives 2000 toughness, 80 damage every three seconds, and a 25% chance of throwing iron butter instead. Iron butter deals 100 damage plus the target’s remaining type-1 armor health, then applies four seconds of butter stun. Ordinary iron kernels do not inherit this armor bonus. Neither attack gains bonus damage from held type-2 shields.

`IronCorn.GetBulletType` at RVA `0x55f040` returns 121, `Bullet_kernal_iron`; `GetBulletType2` at `0x55f030` returns 122, `Bullet_butter_iron`. The kernel prefab 75726 uses the ordinary `Bullet_kernal` script. Its actual `HitZombie` body at `0x70d8e0` passes the projectile’s damage directly to the zombie. The butter subclass at `0x6fb5a0` adds the current first-armor health before damage and applies the four-second stun afterward. The corresponding method snapshots are in this directory and [bolt-iron-kernel](../bolt-iron-kernel/).

The plant prefab is 46066 with `IronCorn` script, animator controller 6254, and clips 4081/4082/4083. The exporter retains the original idle, kernel throw, and butter throw clips, sprites, hierarchy and transforms. The two projectile prefabs are 75726 and 42994. The full model and projectile exports are fingerprinted in their respective `visual-assets.json` files. Source numeric bullet IDs are never passed as Replanted projectile enums.

## Implementation and lifetime

`FusionPlant.BoltPult` uses the unique saved native pair `(Kernelpult, Wallnut)`. `LobberFusions` owns initialization, targeting, attack interval and the once-per-cycle butter choice. The existing source timing is retained: a one-second action clip and the existing tick-34 throw event. The common interval is 285–315 simulation ticks, with 100 ticks per second. The Butter Pot changes the existing butter draw from one in four to one in two.

`LobberProjectiles` owns saved projectile families 16 and 17. Kernels use native `ProjectileType.Kernel`, marker `-520400`, damage 80. Butter uses native `ProjectileType.Butter`, marker `-520420`, damage 100. The native saved lob motion owns travel and collision. The saved projectile identity survives producer death and does not require a transient owner lookup.

On impact, iron butter snapshots the target’s remaining native `mHelmHealth` into its damage before the native armor/body writer runs. Existing `BypassesShield` lob rules preserve type-2 shield handling. A surviving target receives four seconds of native butter; an existing longer butter effect remains longer. Board and full DataID checks reject recycled targets before the status callback. Ground misses release the projectile, inactive or friendly targets cannot take the hit, and already-consumed projectiles cannot apply damage twice.

The existing Bucket repair path heals Bolt-pult by 500 up to its maximum without replacing the plant or resetting its saved attack phase. No MagneticSystem or MagneticNetwork owner exists in the current ports. Bolt-pult adds no independent magnetic-network attack; it retains the implemented metal repair behavior. Unported external magnet-network synergies remain unavailable with those external systems.

## Fertilizer

The original `Fertilize.Upgrade` is independently archived in [bolt-fertilizer](../bolt-fertilizer/). Its general branch heals and caps a positive attribute timer at half a second. Source ID 1211 has no special evolution branch, and IronCorn introduces no attribute timer. Fertilizer therefore heals this plant without evolving it, resetting its attack phase, or forcing an extra shot. The existing fertilizer owner provides that behavior.

## Verification

The complete `ShootingPotsTests.csproj` and `BucketPlantsTests.csproj` programs pass. `BoltPultCases.cs` exercises both saved projectile identities, exact armored and unarmored damage, butter duration, longer existing butter, producer death, target recycling, friendly rejection, misses, duplicate impact rejection, source shooting time, single butter choice, pause/sleep, and failed allocation. The Bucket suite verifies repair eligibility, the 500-health cap and preserved attack phase.

During verification, an existing splash fixture defect was independently reproduced from unchanged HEAD and corrected without product changes. Evidence: [lobber-impact-fixtures](../lobber-impact-fixtures/validation.md).

These offline tests do not establish runtime rendering, native armor depletion, live projectile impacts, actual save/load round trips, fertilizer UI, or recipe input behavior. The orchestrator’s separate runtime review records those results before this port is marked complete.
