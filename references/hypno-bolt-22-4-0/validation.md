# Hypno Chomper and Bolt-pult validation

The final Plant Fusion 22.4.0 build was reviewed and tested on 7 November 2026 with Debug Mod 1.3.14 in PvZ Replanted. See [release.json](release.json) for the installed artifact checksums and installation rollback paths.

## Executed game verification

The reviewer first confirmed that no Replanted process or live Debug API session existed, then launched Steam as desktop user `kevin`. The native start button, main menu and temporary Night debug session were used. Screenshots confirmed the visible board without the startup overlay. No ordinary game or existing user profile was replaced.

The final complete run of `tools/smoke_hypno_bolt.py` exited successfully. Its command responses and assertions are retained in [runtime.json](runtime.json):

- Four seed placement cases and four Lawn Glove cases cover both ingredient orders for both recipes. Each transaction produces exactly the expected fusion and consumes its ingredients. A separate flowerpot survives the compatible recipe cases.
- Fertilizer restores Hypno Chomper to 300 HP and Bolt-pult to 2000 HP. The imported original bodies have active SpriteRenderers corresponding to their exported model nodes. Screenshots were taken after 400 simulation ticks so the fertilizer effect no longer covers the plant.
- Feeding an additional Bucket repairs Bolt-pult by exactly 500 HP without replacing the plant or changing its saved attack phase.
- Five isolated real Bolt-pult projectiles were observed before collision, including both random branches. Original Iron Kernel and Iron Butter renderer paths are active. Kernels remove exactly 80 of a Pail zombie's helmet HP with no body damage. Iron Butter removes all 1100 helmet HP and exactly 100 body HP. The observed remaining butter duration is 388 ticks immediately after the native 400-tick application. Removing the shooter before impact confirms that the saved projectile remains functional independently.
- A normal Hypno Chomper meal starts with 3993 remaining native digestion ticks and produces no Hypno-shroom packet. Fertilizer shortens the real digestion to at most 50 ticks, and native state progression finishes it.
- A zombie hypnotized by a real native Hypno-shroom bite is moved inside the native Chomper range. Hypno Chomper devours that allied zombie, begins with 1000 remaining digestion ticks and creates exactly one native Hypno-shroom seed packet.
- While digesting, Hypno Chomper receives a real enemy bite. The attacker becomes hypnotized, the original fusion disappears, an ordinary Chomper replaces it, and its separate flowerpot keeps the same DataID.

Clean visual evidence: [Hypno Chomper](HypnoChomper-body.png), [Bolt-pult](BoltPult-body.png), [Iron Kernel in flight](bolt-kernel.png), [Iron Butter in flight](bolt-butter.png), and [allied devour reward](hypno-allied-meal.png). Both the reviewer and the orchestrator inspected the body and projectile screenshots.

## Observed issue and regression proof

The first real allied meal exposed a product defect: the reward observer hooked only the standalone `UpdateChomper`, while the game normally executes an inlined Chomper state machine in `UpdateAbilities`. Its prefix also assumed a countdown of zero even though the actual caller decrements that countdown internally. The failed real meal, 4000-tick digestion and empty coin pool are recorded in [ally-range-diagnostic.json](ally-range-diagnostic.json).

The final observer covers both native entries and records an allied meal only after the native devour commits. It shortens only the saved allied meal's native countdown, clears that flag once, and preserves shorter countdowns. Nested meal and digestion postfixes cannot duplicate the reward or overwrite the shortened duration.

Independent offline execution passes 50 Hypno Chomper assertions, including the actual prefix/native-body/postfix boundary, the final one-to-zero countdown tick, both native entries, nested reward and digestion paths, missed targets, skipped originals, pool generations and failed allocations. The native assembly evidence shows the ordinary 4000-tick write in both entry points; the mod no longer writes ordinary digestion.

The isolated check against the final installed build observes 994 remaining allied digestion ticks and exactly one packet: [allied-meal-fixed.json](allied-meal-fixed.json) and [screenshot](allied-meal-fixed.png). A separate final normal meal observes 3993 ticks and no new packet: [final-normal-meal.json](final-normal-meal.json). The full final runtime suite independently repeats both cases successfully.

Earlier `fixture-attempt-*.json` and `focus-fixture-extreme.json` files retain test setup failures separately. They cover an invalid attempt to add a flowerpot underneath an existing plant, the Debug API's explicit rejection of freezing a mind-controlled zombie, an allied zombie initially outside native range, and attempting to prepare native hypnosis at the user's Extreme difficulty. Those setup failures are not classified as additional product defects. Successful combat tests explicitly use Normal difficulty and restore the original value.

## UI and localization

The final game session selects the new Bolzenapult and Hypnoschnapper debug cards through the native chooser and enters a fresh temporary board. [Chooser screenshot](new-cards-chooser.png), [seed bank screenshot](new-cards-bank.png), and [ui.json](ui.json) show the original portraits and costs of 300 and 225.

The actual active UI images use 128 by 176 pixel packet rectangles at (1152, 1936) and (1152, 1056). These match the generated Debug Mod atlas entries for source IDs 1211 and 1335 exactly; [ui-atlas-comparison.json](ui-atlas-comparison.json) retains both sides of the comparison.

The live Almanac API reports both registered plants with their full German names and descriptions, correct type/marker identities and `plantUnlocked: false`. English and German catalog content and translation hashes were inspected offline. No language setting was changed. The documented API provides Almanac data, not a command to open an individual detail panel; no live detail-panel interaction is claimed.

## Cleanup and limits

The full test restores the previous speed index 0, Sun Collector enabled state and Extreme difficulty value 5. Its original paused debug state is restored before the separate UI session. No profile mutation commands or persistent save operations are issued. Only temporary debug boards are created. The final game is closed through `game.shutdown`, and actual `Replanted.exe` process exit is confirmed.

[final-runtime.log](final-runtime.log) contains no new exceptions or errors during gameplay. Its three shutdown errors are the already documented IGNORED native RenderTexture issue. No suppression or global cleanup patch is added.

No persistent save/load round trip was executed. Native saved fields, identity reconstruction, pending reward state and pool lifetime guards are source-reviewed and covered by the stated offline tests; those checks do not claim an executed persistent save round trip.

Generated assembly dumps have trailing whitespace removed, and copied logs use LF line endings. The native evidence output hashes match these normalized text files; original binary input hashes remain unchanged.
