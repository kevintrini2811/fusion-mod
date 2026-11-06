# Plant Fusion 22.2.0 validation

Tested on PvZ Replanted 1.5.1469_Steam through the documented Debug Mod API in temporary Night and Pool boards. Installed versions: Plant Fusion 22.2.0, Zombie Library 1.3.0, Debug Mod 1.3.5 and Speed Mod 1.0.5. Native profiles and board saves were not edited.

## Source coverage

The [Hot Potato contract](../hot-potato/port.md) and [Boomwood contract](../boomwood/port.md) pin Fusion 4.0.5.2 methods, recipes, prefab metadata and original artwork. Each result has one available source recipe, accepted in both ingredient orders. Roasted Potato already exists and is reused. Both passive plants heal when fertilized; neither has a separate fertilizer evolution or ultimate in the source.

## Live combat and rendering

`tools/smoke_charge_torches.py` passed. Its final observations are in [runtime.json](runtime.json):

- Both ingredient orders produce each new identity. Fertilizer restores 300 HP and preserves charge.
- Removing either new plant triggers its 1800-damage death explosion.
- Torchthree fireballs damage targets in all three rows.
- Small and potato peas use their original fire artwork and saved 40/50 damage. Potato peas award eight credits; the thirteenth conversion summons the existing Roasted Potato ahead.
- Rolling nuts use the original Fire Nut artwork and doubled saved damage.
- Explosive cherries become visible Fire Cherries, acquire another row after their original target is removed and deal explosion damage.
- The upper small mushroom sorts behind both lower mushrooms.
- All three Eyes Zombie identities retain their native armor state. [Land armor](eyes-armor.png) was inspected after replacing guessed offsets with the native helmet-to-head geometry, including raster padding.
- Native grave rise progresses from altitude -200 through a [partially emerged body](grave-rise-partial.png) to altitude 0 and the normal phase. The saved Eyes identity survives the complete rise. Imported artwork uses the native displacement and clipping plane.
- The actual native speed button reaches every installed stage through 20× and retains each selection over subsequent render frames.

`tools/smoke_charge_storage.py` passed; [storage-runtime.json](storage-runtime.json) records ordinary conversion charge reaching 30 before a 300-damage drone appears, all three saved drone slots, original stored artwork, unchanged explosive cherries passing a full rack and stored drones attacking a newly introduced target for their saved damage. [Stored drones](cherry-storage.png) were inspected.

`tools/smoke_pool_eyes.py` passed; [pool-runtime.json](pool-runtime.json) records automatic pool entry for the additional native types, including a popped Balloon Zombie. All three Eyes variants have an active imported head and an active duck renderer. Their torso heights agree within 40 world units, catching the stale pooled shadow that previously displaced Buckethead artwork. The [pool screenshot](eyes-ducks.png) confirms the imported Buckethead Eyes Zombie and all three ducks. Debug spawning leaves persistent sightings unchanged. Duck-only choices and native DuckyTube are absent from the debug selection.

The no-Speed-Mod VersusBot reproduction and English issue report are archived in `speed-mod/references/versusbot-speed-issue.md`. Native 2× and 2.5× reset to 1× with both Speed Mod and Fusion disabled.

The final isolation run disabled FusionMod.dll and passed all 17 additional native duck types, including Balloon after popping. [Library-only observations](pool-library-only-runtime.json) include live native slots and renderers. The original DLL was restored and its SHA-256 verified. This exposed Pogo's zero mesh Width/Height metadata: selecting the torso from actual posed vertices fixes its missing tube. Rigs without a swim clip retain their native walk animation. [Library-only screenshot](library-ducks-without-fusion.png) records the pool appearances.

## Offline checks

- Release builds of Fusion, Debug Mod and the Zombie Library example succeeded with no warnings or errors.
- Registration generation check passed; static validation reports 383 active recipes, 286 pending recipes and zero errors.
- Torch regression suite: 23 checks passed. Mushroom placement suite: 622 assertions, 703 checks including native spreading.
- Eyes replacement contracts cover all 100 random rolls for Normal, TrafficCone and Pail with the same difficulty-dependent chance.
- Zombie Library registry and grave displacement contracts passed.
- Original-art exporter: 38 tests passed. Debug API transport: three tests passed. Speed visual-clock and simulation-budget regression suites passed.

## Test conditions

Night boards contain random graves. The potato-summon check chooses an available barrier tile so that a zombie cannot eat the test plants or immediately detonate the summon. A dancer can remove its followers during cleanup; pool cleanup rereads live IDs after every removal. These are test setup constraints.

With ReplantAPI installed, direct debug startup can report a playable board while the splash image still covers rendering. Final visual evidence uses a normal startup followed by the documented menu and chooser commands. The workflow now records this observed fallback to avoid repeating unusable screenshot runs.
