# Hypno Chomper

This port uses the original Fusion 4.0.5.2 distribution under `pvzfusion/english-4.0.5.2/Game Files`. `research-inputs.json` pins the source checksums. The unofficial expanded sources contain an older, different Hypno Chomper and are not evidence for this port.

## Identity and recipe

The original identity is 1335, class `::HypnoChomper`, prefab `HypnoChomperPrefab` at PathID 34837. The sole source recipe is Chomper plus Hypno-shroom, supported in both orders. Replanted stores the variant as `(SeedType.Chomper, SeedType.Hypnoshroom)`; the original numeric identity is never passed to a native enum.

## Source behavior

The translated almanac states that ordinary devours digest for 40 seconds, while a hypnotized zombie yields a Hypno-shroom packet and digests for 10 seconds. When bitten, the plant hypnotizes the attacker and becomes an ordinary Chomper.

The binary confirms the digestion and reward contract. `Chomper..ctor` at RVA `0x48e710` initializes the swallow duration to the float `40`. `HypnoChomper.Chomp` at `0x4b16c0` first delegates to native source Chomper consumption. Its allied branch at `0x4b16dd` reads the consumed zombie's control flag, creates seed type 8 at `0x4b1727`, then applies the source countdown multiplied by `0.25` at `0x4b1737`. The multiplier lives at RVA `0x297bb30`. The inherited bite and search rules retain the source's exclusion of invulnerable or incompatible zombie states.

`HypnoChomper.Start`, `CheckZombie`, `BiteEvent`, `Chomp`, the base Chomper methods, prefab components and original almanac are archived beside this note. The bite transformation is part of the original Hypno interaction contract described by the source almanac; Replanted uses the existing Fusion `Zombie.EatPlant` integration to perform it.

## Native ownership and persistence

Replanted's native Chomper retains target geometry, target exclusions, bite animation, failed bite, devour, swallow and digestion. To include allies, the port makes a second native target query with `DamageRangeFlags.OnlyMindcontrolled`; the context applies only to the same plant generation and board. It then compares native target rectangle X coordinates using the native integer priority. Ordinary targeting is unchanged when it wins or ties. No zombie allegiance field is temporarily modified.

The native target and damage-range paths are pinned in `../hypno-chomper-native/native-inputs.json`. `Zombie.EffectedByDamage` at `0x6f12f0` uses flag 128 only to select the hypnotized side and preserves its normal state exclusions. The native Chomper calls `DieWithLoot` and commits `ChomperBitingGotOne` after a devour. The port requires that state plus the captured zombie's death, plant generation, zombie generation and board before recording a reward.

Replanted runs an inlined copy of the Chomper state machine inside `Plant.UpdateAbilities`, rather than calling the standalone `UpdateChomper` entry. `UpdateAbilities` decrements the saved countdown at RVA `0x6b62bd` before its inlined bite commit and assigns the ordinary digestion duration at `0x6b76c6`. The reward observer hooks both native entries, captures an active bite without assuming a zero countdown at entry, and commits only after an actual devour. The saved allied-meal flag also makes nested calls idempotent: an inner standalone Chomper postfix cannot cause an outer abilities postfix to award another packet. The original implementation observed only the standalone entry and missed the live devour; `../hypno-bolt-22-4-0/ally-range-diagnostic.json` records the failing runtime case. Both native entry traces are now included in the checksum-pinned native evidence.

`mTargetX` stores the allied-meal flag between the committed bite and entry into digestion. Native code assigns its ordinary 4000-tick duration. Only an allied flag allows the port to shorten that duration to at most 1000 ticks, then clear the flag. A shorter countdown is preserved. A second outer postfix consequently leaves the inner shortening unchanged when both native entries are nested. `mTargetY` stores a pending reward packet when the native coin pool cannot allocate one. Allocation reserves the flag before callbacks, and a failed allocation keeps the reward for a later active tick. Both fields are serialized and deserialized by native Plant; neither is otherwise used by native Chomper. Loading an ongoing digestion does not restart it. Fresh pooled plants clear both fields during initialization.

The packet is a native `UsableSeedPacket` with both saved seed type and controller seed type set to `Hypnoshroom`. Native collection, placement and persistence own the packet afterward. The port adds no projectile or projectile artwork because this plant shoots none.

On an enemy bite, the ordinary replacement is allocated before the original is consumed. Failed allocation leaves the original and the attacker's allegiance unchanged. Generation checks prevent consuming a reused original or controlling a reused attacker. The existing consumption scope preserves supporting plants and pumpkins.

Fertilizer uses the existing healing contract and caps positive active digestion at 50 ticks. It does not shorten an uncommitted attack, lengthen a countdown below 50, or create additional reward packets.

## Verification

`tests/HypnoChomperTests.csproj` compiles the actual port and Chomper fertilizer code against offline boundary doubles. Its 50 assertions check allied search scope cleanup, native flag composition, target priority, committed devour, pending packet retry, packet identity, digestion transitions, saved-state reconstruction, generation reuse, pause, replacement failure and fertilizer. Regression cases invoke the actual Harmony prefix and postfix around simulated native bodies, including the inlined one-to-zero countdown, standalone zero-countdown entry, earlier attack ticks, failed bite, skipped original body and nested entries with exactly one packet. Nested digestion preserves the allied 1000-tick duration, a previously shortened duration, and ordinary native 4000-tick digestion. These checks do not claim native animation or gameplay success. Live verification and visual evidence belong to the shared task runtime report.

The native ordinary digestion writer is independently confirmed in both entry points: UpdateAbilities at RVA `0x6b76c6` and UpdateChomper at `0x6b8950` store `0xfa0` (4000) into native `mStateCountdown` at offset `0xa0`, immediately after selecting ChomperDigesting. The port therefore leaves ordinary digestion entirely native. The original native traces are `../hypno-chomper-native/replanted-0x6001a26.asm` and `replanted-0x6001a3f.asm`.
