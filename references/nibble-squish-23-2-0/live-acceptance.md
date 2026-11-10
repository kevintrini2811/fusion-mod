# Nibble-shroom / Squish acceptance

Fusion 23.2.6 and Debug 1.3.28 complete the focused acceptance for the directed Puff-shroom + Chomper and Puff-shroom + Squash recipes. The original source/asset contracts and documented native adaptations remain in the [Nibble notes](../nibble-shroom-contract-20261108/port-notes.md), [Squish notes](../squish-contract-20261108/port-notes.md), [product review](product-review.md) and [native lifecycle analysis](native-puff-jumper-lifecycle.md). Their earlier incomplete-live statements describe the state at those historical reviews.

| Evidence | Actual result |
| --- | --- |
| [UI, Day, Nibble, Squish and timing](runtime/run-20261110T120525Z/independent-five-group-complete-raw-review.md) |51/51 independent checks; actual CLI exits 0. Recipes, slots, sleep/Coffee, fertilizer, abilities, native damage callers and 100/120 Hz/pause verified. |
| [Water, controller tools and lifecycle](runtime/run-20261110T134949Z/) |35/35 independent raw checks and restoration 28/28; Shovel removes only HomeLily, Glove picks and restores the same Lily ID. |
| [Native Bungee selection/drop](runtime/run-20261110T151213Z/independent-bungee-outcome-review.md) |Bungee 10 selects Ground 3 while Squish 1 remains airborne; native drop restores the same Ground 3 at home. Restoration 24/24. |
| [Ordinary offhome Squish bite](runtime/run-20261110T153253Z/independent-bite-outcome-review.md) |Manually accepted original raw proof: Squish 1 Ground 0, 100 px offhome, Normal 10 eating, HP 300→296→292; Wallnut 3 unchanged. Original CLI exit 1/result.failed remains unchanged: its post-pause equality assumption was wrong. Restoration 24/24. |
| [Unobscured Hover/Fall art](runtime/run-20261110T154649Z/independent-art-outcome-review.md) |Same Squish full ID 1 in actual phases 3/4, active original renderers and Shadow_1 disabled; clear upright elevated Hover and wider, flatter, lower Fall. Root and reviewer directly viewed both PNGs. Capture CLI exit 0, restoration 24/24. |

The accepted native/offline owner split remains unchanged. No actual disk save/load roundtrip, direct Catapult selector return or broad Boss live suite is claimed. Their absence adds no new requirement to this accepted contract. Existing IGNORED native/foreign findings remain present and are not described as fixed. Protected user files, settings, source/DLL bytes and original display geometry/DPMS Off were preserved or restored after each final probe.

Current Release/build/install evidence is retained under `../../../.test-runs/controller-shovel-query-fix-23-2-6/`; later Debug fixes and the actual installed pair are bound by the final runtime source/DLL snapshots. No product/version change was needed for final acceptance. Recipe-list removal affects only the two completed directed entries; unimplemented future recipes remain open.
