# Magnet Blover and Lumos Magnet: Fusion 4.0.5.2 contract

## Evidence and limits

The existing research tool validates the GameAssembly SHA against `fusionen-quellen.json`. The raw method ranges, recipe records, source almanac records, root prefab components and controllers are archived in `../magnet-blover/` and `../lumos-magnet/`. The source root prefabs are `MagnetBloverPrefab` 42478 and `LanternMagnetPrefab` 45021. The actual script classes are `MagnetBlover` and `LanternMagnet`, both inheriting `Magnetshroom`.

Only these recipes produce the selected outputs: Blover + Magnet-shroom -> Magnet Blover (1102); Magnet-shroom + Plantern -> Lumos Magnet (1100). Both source registrations explicitly accept both orders. Both ingredients are available native Replanted plants. Neither output is a native Replanted plant.

The source collider enumeration cannot be reproduced byte-for-byte across engines. It selects the first eligible result in the physics result array, with no nearest-distance sort. Replanted should preserve its existing deterministic eligible-candidate order rather than introduce nearest selection.

## Shared attraction and digestion

`Magnetshroom.FixedUpdate` (0x4cb5a0) calls the ordinary Plant update, requires active gameplay and nonzero animator speed, then invokes the virtual `MagnetUpdate`. `Magnetshroom.MagnetUpdate` (0x4cb640) searches only when AttributeCountdown is exactly zero. `SearchZombieWithIron` (0x4cb810) performs a circular overlap centered at the plant position with the source vertical offset +0.5. It checks the separate absolute row-distance limit and excludes mind-controlled zombies before calling `TryAttrackZombie`. A successful attraction returns immediately. A physical loose item is also eligible if it is not already being attracted and satisfies the same row-distance restriction. No stack of simultaneously processed items is specified.

`TryAttrackZombie` (0x4cbbe0) prioritizes first armor, then second armor, then unique metal items. Source first-armor branches include Bucket and FootballHelmet; second-armor branches include Door and Ladder; unique-item branches include Jackbox, Jumper and Pickaxe. Source-only armor/enemy variants have no invented Replanted equivalent. Each recognized equipment branch removes the actual equipment through its responsible source path and invokes virtual AttrackItem. Loose ladders are also detected through the physical-item branch in SearchZombieWithIron. Native Replanted attraction should own the corresponding equipment transition.

`AttrackItem` (0x4ca7d0) owns the actual captured object, starts its flight, triggers the attraction animation, sets AttributeCountdown to attrackMaxTime and marks usable bucket objects by setting `Bucket.isLand` true and `Bucket.disappear` false. `AttributeEvent` (0x4cadb0) destroys the processed target object, clears its reference, then offers the captured item to connected system consumers. The source runs one digestion interval per attraction; there is no separate post-arrival interval in this path.

Both `MoveIronItem` variants pass exactly 0.3 seconds to `MoveObject` (literal reads at 0x4cb6ef and 0x4c79ee). The actual coroutine `<MoveObject>d__16.MoveNext` (0x4e2dd0, archived in `../magnet-flight-source/`) linearly interpolates elapsed/duration from the captured origin and explicitly sets the destination at completion. Repeated exponential smoothing is not the original flight contract. A port may reconstruct the same linear progression from saved current position and the remaining flight ticks.

`DieEventMustExecute` (0x4cb420) releases an actual physical captured item when appropriate, with an explicit exception for source type 1102. The physical Bucket age advances only when its `disappear` flag is true (Bucket.Update 0x9d536b); captured physical items keep their remaining age. Synthetic equipment graphics are not automatically treated as a new gameplay drop. Preserve native item ownership and avoid creating duplicate drops.

## Magnet Blover

`Start` (0x4c7db0) calls Plant.Start, sets attrackMaxTime to exactly 1 second, triggers `blow`, and initializes AttributeCountdown to 0.5 seconds. It does not install a hard maximum-item counter.

The original `blow` clip 3869 has duration 1.26666665 seconds. Its sole event is `AnimBlow` at 0.383333325 seconds. Controller 6186 starts from idle, transitions to blow on its trigger, then transitions to blowloop. The original blowloop state has speed 0.3. Clip 3870 is blowloop; clip 3871 is idle.

`AnimBlow` (0x4c76b0) invokes virtual BlowZombie, invokes `FogMgr.Blown` if the Board fog object exists, and schedules DelayDie in exactly 10 seconds. The ten-second lifetime starts at this event, not at plant creation. The first regular attraction remains controlled by the initial 0.5-second countdown. `BlowZombie` (0x4c77d0) traverses zombies and applies the original balloon blow action to source BalloonZombie. It does not apply global wind damage to unrelated zombies.

`SearchZombieWithIron` (0x4c7a60) uses inherited attrackRange multiplied by 10. The inherited constructor sets range 4 and row distance 2, producing radius 40 source world units and maximum row distance 2. The large horizontal radius does not remove the explicit row restriction. The source almanac describes field-wide attraction, but the actual row guard still exists. It processes one available item every 1 second during its lifetime. There is no independent hard cap of 10. Do not invent a count-based removal.

The source almanac also describes an on-entry bucket offer to Hypno Magnet or the ultimate hypno consumer in its connected MagnetSystem. These consumers, plus MagnetBox, are absent from ingredient availability. The shared AttributeEvent only dispatches to these unported consumers; no implemented plant is an eligible recipient. This unavailable synergy must be explicitly documented, not represented as implemented or expanded into a future MagnetSystem feature.

## Lumos Magnet

`LanternMagnet.MagnetUpdate` (0x4bf570) updates range only when AttributeCountdown is zero, then invokes SearchZombieWithIron. A light change does not rewrite a running digestion timer.

`SetRange` (0x4bf5c0) writes the following exact values. At LL 0: radius 2 source world units, row distance 2, digestion 15 seconds. At LL 1: radius 5, row distance 3, digestion 11 seconds. At LL 2: radius 6, row distance 4, digestion 7.5 seconds. At LL >=3: radius 7, row distance 5, digestion 5 seconds. Convert source world distance with the existing factor 80/1.35. The almanac's 1.5/3.7/4.5/5.2 tile figures are rounded descriptions, not the exact distance constants.

`AttrackItem` (0x4bf150) first checks a valid target. At LL >1 it performs a radius-1-world-unit overlap around the target position. It affects only non-mind-controlled zombies in the passed attraction row, subject to Plant.AttackLandZombie. It calls the ordinary damage dispatcher using plant.attackDamage, then creates the original local effect. Only after this damage pass does it invoke the base AttrackItem (0x4ca7d0), which starts captured-object graphics and digestion. Equipment removal occurs earlier in TryAttrackZombie, before the virtual LanternMagnet.AttrackItem call: the bucket path clears first-armor reference, health and type at 0x4cbcd2–0x4cbceb, then dispatches the virtual call at 0x4cbcfb–0x4cbd0c. Other recognized equipment paths likewise commit their corresponding equipment transition before dispatch. The required order is therefore equipment removal, local damage, captured-object flight/digestion. The removed armor cannot absorb the local damage. A port that delegates the bundled native equipment-and-graphics transition first may apply the local damage immediately afterward, provided the gameplay equipment transition and callback generation checks preserve this order. The source almanac states 200 damage. No area damage occurs below LL 2. Callback generation checks are required across both removal and damage callbacks.

The entire LanternMagnet class contains only Awake, MagnetUpdate, SetRange, AttrackItem and its constructor. It has no light-provider OnCreate/OnMove/Die override. Awake resolves a local lightShine object, and SetRange enables this graphic at LL >=2. No self-contributed light level or persistent fog-clearing ability is established. The shared existing light query should measure external light providers.

## Fertilizer, sleeping and adaptation

The ordinary Fertilize.Upgrade branch (0x682b1c–0x682b6d) calls the plant recovery method and shortens a positive AttributeCountdown to at most 0.5 seconds. Its literal at 0x682a63 is 0.5. It does not increase Lumos light level, reset Blover lifetime or create a separate instant area attack. The short interval completes normal digestion, after which normal target acquisition resumes.

Fusion 4.0.5.2 does not expose a native Replanted sleeping state in these classes. The project workflow retains native mushroom/terrain/coffee rules for ports. If both outputs use native Magnetshroom as backing identity, preserve those established native rules rather than silently invent a source sleep timer. This is an explicit platform adaptation, not a claimed source sleep branch.

No projectile is produced by either selected class. Metal flight is a captured-object presentation and ownership transition, not an attack projectile. Preserve original plant artwork, action clips, lightShine and captured equipment presentation using the appropriate existing owners.

## Captured item selection

Bucket.OnClick (0x9d4900) invokes its virtual Pick method. Bucket.Pick (0x9d4920) marks isLand true, updates the Mouse-held item and hides the relevant object/collider. It does not clear Bucket.magnetShroom, Magnetshroom.targetItem or AttributeCountdown. The source magnet digestion and Blover lifetime therefore continue while the captured item is selected. A port must not inherit a vanilla-only whole-ability hold pause for these variants. If digestion or death removes the saved slot, the existing selection validation must cancel that selection safely. Successful slot consumption must also clear physical-origin ownership markers so later death cannot recreate the consumed item.
