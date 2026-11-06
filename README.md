# Fusion Mod 22.2.6

Version 22.2.6 renders armored Eyes portraits from the imported walk pose and the same native helmet fit used on the live zombie. Placement previews and generated cards no longer use manually positioned portrait helmets.

Version 22.2.5 indexes potential torch-conversion hosts once per board tick instead of scanning every plant for every projectile. Plant lifecycle and board changes invalidate the index; conversion rules, live positions, damage, visuals and simulation timing are preserved.

Version 22.2.4 forwards custom zombie bite events through native Hypno-shroom consumption and hypnosis. Zombie Library validates registered actors and the actual native bite target; Eyes calls the shared helper at its own bite event. Native AI and saved identities remain unchanged.

Version 22.2.3 uses native character colour state for imported zombie sprites, including cold blue, armour and pool ducks. Zombie Library owns the shared colour adapter; source alpha and pool clipping remain intact.

Version 22.2.2 makes Speed Mod and Difficulty Mod optional. Imported animations use Speed Mod when installed; the golden bean remains available beside the native speed controls without it. Fusion releases leave optional mods untouched. [Review and live dependency matrix](references/optional-dependencies-22-2-2/validation.md).

## Dependencies

- Game/runtime: PvZ Replanted (Windows/IL2CPP), MelonLoader 0.7.3, and .NET 6.
- Required mods: `BloomEngine.dll`, `LawnGlove.dll (Glove Mod)`, `PlantLibrary.dll`, `ZombieLibrary.dll`.
- Optional integrations: `DebugMod`, `SpeedMod`, `DifficultyMod`.
- Building: .NET SDK with `net6.0` support and the game/MelonLoader reference assemblies declared in `FusionMod.csproj`.

## Hot Potato, Boomwood and zombie fixes (22.2.0)

Hot Potato and Boomwood include both source recipe orders, original models and projectile graphics, charge-based summons, death explosions and fertilizer behavior. Hot Potato reuses the existing non-fusible Roasted Potato. Boomwood preserves explosive-cherry damage and limits each stump to three drones. See [Hot Potato source contract](references/hot-potato/port.md), [Boomwood source contract](references/boomwood/port.md) and [runtime validation](references/charge-torches-22-2-0/validation.md).

Zombie Library 1.3.0 owns all additional native duck variants and their cards. Plant Fusion retains its fusion-specific zombies and adds Conehead and Buckethead Eyes Zombies, including automatic duck rendering and the same difficulty-dependent replacement chance as ordinary Eyes Zombies. Imported bodies use the library's native displacement and clipping during grave rises. Debug Mod 1.3.5 hides duck-only selections and spawns the base zombie's water appearance automatically.

The upper mushroom in each three-slot tile renders behind both lower slots, including after native depth recalculation. Torchthree fireballs now carry ground damage categories and hit zombies in all three lanes. Speed Mod 1.0.5 removes VersusBot's unrelated 1.5× cap outside Bot vs Bot while retaining its own mode's behavior.

## Four new Torch Fusions (22.1.0)

Walnut, Triple Torch, Sparklewood, and Hypnofire use original models, animations, and projectile graphics from Fusion 4.0.5.2. All four recipes work in either order. Fertilizer heals and maintains stored inflammation counters. [Sources and Implementation](references/torch-four-22-1-0/implementation.md), [Testing and Limitations](references/torch-four-22-1-0/validation.md).

Sea mushroom fusions are checked based on their result before the native water lily requirement. Peatato Mine shoots from the original animated muzzle point. The global splash hook on native damage areas is replaced with local targeting and landing rules; As a result, the reproduced cactus crash on water lilies passes the counter-test. Zombie Library 1.2.1 corrects duck size, arm order and shield bearer in water. Debug Mod 1.3.4 adds missing seed package backgrounds and leaves the start button with its native state images.

The long-term freeze could be reproduced as a self-deadlock in Wine's display management under Proton Experimental. PvZ now uses the installed stable Proton 11.0 locally. Pass two six-minute selection tests, including regular Steam launch with overlay. Peatato Mine and a Zombie also ran stable for six minutes. Details and limited significance are contained in the test report.

## Fixed projectile conversions (22.0.1)

Scorchwood natively defrosts snow peas including their graphics and correctly transforms yellow fire peas and potato peas. The source calls for orange fire peas; the previously used purple model has been replaced. Potato peas switch from their own movement to the native movement when crossing. Unsupported Blaze Peas remain unchanged for both Firewood fusions. [Test matrix with before/after graphics and hits](references/torch-projectiles-22-0-1/notes.md).

## New fusions and water rules in 22.0.0

Tide-shroom, Blacklight Plantern, Scorchwood and Stenchwood are supplemented with original graphics, available recipes and fertilizer rules. Nine additional native zombies receive duck variants. Balloons survive water and land after bursting; Pogo loses his staff when he enters the water. Because of the changed water rules, the first version position increases. Debug Mod 1.3.2 prevents persistent almanac sightings from debug levels. [Port contract, sources and test documents](references/support-ducks-22-0-0/notes.md).

## New fusions in 21.5.0

Umbrella Cabbage (Cabbage Pult + Umbrella Leaf), Umbrella Bark (Melon Apult + Umbrella Leaf), Light Umbrella (Plantern + Umbrella Leaf), and Ice Pot (Flower Pot + Ice Mushroom) use original models, animations, and both recipe orders. The ice pot contains the four original, non-fusible roasted auxiliary forms. Glacier projectiles retain their original graphics. Debug Mod 1.3.1 adds map frames and controller leaves to the horizontal map edges. [Port contract and inspection documents](references/umbrella-ice-21-5-0/notes.md).

## New fusions in 21.4.0

Threetato Mine (Potato Mine + Threepeater) and Marigold Mine (Potato Mine + Marigold) are ported with original graphics, animations, projectile logic, fertilization and both recipe orders. The second version position increases because of two new plants. [Original receipts, port contract and playtests](references/treasure-three-mines-21-4-0/port-notes.md).

Debug Mod 1.3.0 adds direct start with `--debug-level=Day`, game end, difficulty and sun collector controls, golden coffee bean, mode unlocks, full status queries, profile management, gravestones, lawnmower and virtual Xbox 360 input. The [API documentation](../debug-mod/src/documentation.md) describes commands and security limits.

## New fusions in 21.3.0

Chompkin, Pumpkin Mine, Sunny Pumpkin and Glacier-shroom are ported with original art, animations and all six available recipes. The second version position increases because of the four new plants. Fertilizer takes into account the respective ability. [Port notes and API credentials](references/pumpkin-glacier-21-3-0/port-notes.md).

## Cherry Pop and Ball Cactus (21.1.0)

Two new fusions with natively available ingredients: Puff-shroom + Cherry Bomb creates Cherry Pop; Puff-shroom + Cactus makes Ball Cactus. Both recipe orders use the existing merger transaction. There are no other recipes for these results in source 4.0.5.2.

Cherry Pop explodes for 900 damage upon contact with the ground or death, before attaching a half-ripe cherry to a random suitable target without a sticky cherry. The first explosion already charges this cherry. Ball Cactus shoots an original cactus spike across the line for 30 damage every about a second. After the hit, it reduces the size of both ground and air targets according to the actual swelling behavior. Imps use the original death exception. Original models, animations, sticky cherries, impact effects, daytime rest and three subfields are connected. Fertilization heals without additional development.

[Sources and references](references/puff-duo-21-1-0/port-notes.md). Debug Mod 1.2.1 uses the same gray roster as zombie selection and uniform seed packs for registered zombies.

## Puff-shroom spreads (21.0.0)

`Puff-shroom (Unterlage) + Fume-shroom` consumes one to three common puff-shrooms from the same cell. With one, two or three ingredients, `1`, `2/1` or `3/2/1` puff-shrooms are created. A native smoke shroom remains in the original cell.

`Puff-shroom (Unterlage) + Scaredy-shroom` creates a puff-shroom forward on an ingredient. With two ingredients, one is created forward, up and down; if there are three ingredients, two each. A native scary shroom remains in the original cell. Locked cells are skipped; free subfields remain usable. Mixed fungal groups remain protected. Cards and Glove use the same transaction path.

Both recipes end in existing Replanted plants. Abilities, graphics, projectiles, impact effects, day rest, fertilization and almanac entries remain native. No additional plant types or unlocks. The first version location increases because of the new propagation mechanics. [Sources, Tests and Limits](references/spreading-21-0-0/port-notes.md).

The separate [Debug Mod 1.2.0](../debug-mod/README.md) shows gray UI frames and brains instead of suns during zombie selection. After selection or cancellation, the native graphics are restored.

## Standalone Debug Mod (20.1.0)

Debug level operation is now in [Debug Mod 1.0.0](../debug-mod/README.md). Without `DebugMod.dll`, Fusion will not start a debug level. With Debug Mod, fusion ingredients, finished fusions, placement rules, and previews remain available. The debug selection additionally takes over all registered entries of other mods from Plant Library and Zombie Library, including normally blocked variants. The seven custom mods and Debug Mod declare each other as optional dependencies; Existing dependencies between the other mods are retained. This release includes Plant Library 1.0.3 and Zombie Library 1.1.3 for catalog connectivity. Game and test programs did not start.



## Hypnohead Direction and Debug Almanac (20.0.4)

The animated head of the hypnopea zombie is mirrored horizontally in the native head anchor. Its position takes into account the mirrored center point; Native direction changes remain responsible. The screenshot `Bildschirmfoto_20261103_121240.png` shows the previous wrong direction opposite the projectile flying to the right.

Full zombie prey lock in debug mode is back on at user request. She skips `Zombie.DropLoot` and sets `mDroppedLoot`; Coins and native level rewards remain there. Normal levels keep their loot.

Zombie Library 1.1.2 respects temporary debug clearance on native Zombie entries, Zombotany and mod tabs including duck variants. Outside debug mode, the actual sightings apply again. No artificial sightings or native profile activations are saved. [Details and manual acceptance](references/feedback-20-0-4/report.md).

Game and test programs did not start. The user is responsible for the runtime check.


## Graphics, flight targets and zombie loot (20.0.3)

Gatling Fusions with native Gatling wearer adjust their full height to the actual native mesh pose. The static preview graphic and the shadow width are not reliable measurements for this. Invalid spine coordinates are excluded. Butter Fly and Cabbage Dropper use the existing conversion of the source models instead of increasing their narrow shadow to tile width; This means both will be around 35 percent smaller.

`Zombie.CanTargetPlant` excludes plants from the flight layer during candidate selection. Zombies walk past it and can continue to eat ground plants underneath. The full debug lock on `Zombie.DropLoot` is removed: Native loot including coins is running again, with unchanged odds and profile credit. Only native level rewards remain locked in debug mode.

In almanacs, cards and duck cards, the hypnopea zombie only gets the head including the mouthpiece. Export removes stems, leaves and shadows. The animated head representation also takes the separately created mouthpiece into account. No new fusions; `fusionen.txt` remains unchanged.

[Causes, Evidence and Manual Checkpoints](references/feedback-20-0-3/report.md). Game and test programs did not start. Compilation and file checks are not a substitute for play testing; the user is responsible for acceptance.

## Standalone speed control (20.0.2)

Fast forward and slow motion are now part of the standalone [Speed ​​Mod](../speed-mod/README.md). Speed Mod and Difficulty Mod are optional. When Speed Mod is installed, Fusion uses its UI anchors and limits visible imported animations to 2× from game speed 10×. At those speeds, imported projectiles skip pose calculation and visual rotations. Without Speed Mod, the golden bean uses the native speed button as its anchor and animations retain native timing. Simulation, hits and projectile flight remain unchanged. Build and static native analysis do not replace playtesting; The user is responsible for manual acceptance.

## Debug level transition (20.0.1)

After zombie selection, the native `SeedBank.UpdateWidth` sets the card number again from `Board.GetNumSeedsInBank`. The previous debug patch continued to deliver ten even if only one sunflower was selected. The actual number of cards previously set was thereby overwritten. The debug chooser now saves the confirmed plant count before native completion. Ten spots remain available during selection; in the level, the same capacity owner provides the actual number chosen. A new width build-up can no longer fill the bank with empty cards.

The golden coffee bean continues to use the slowdown container as a position anchor. Your button template is again from the actual native speed button: the container itself does not have a `Button` component. This regression from 20.0.0 could break the HUD build.

Plant Library and Zombie Library were checked, including their selection and bank patches, and recompiled together with Fusion. No change to their API or source code was required. Native profiles and saves remain unchanged. [Secured logs, native analysis and audit limit](references/debug-crash-20-0-1/report.md).

The logs do not contain any fatal stack trace. The exact native crash point and freedom from crashes have therefore not been proven. Game and test programs did not start. The user checks the transition again with a sunflower and a normal zombie, then several cards and the coffee bean button.

## Changes 20.0.0

- New fusions: **Blover + Cabbage-pult = Cabbage Dropper** and **Blover + Kernel-pult = Butter Fly**, also vice versa. Full original models, animations, three ballistic drops, butter effect, impact effects and almanac text are included. [Port documents and manual verification](references/air-lobbers-20-0-0.md).
- The golden coffee bean is above the slowdown button. The level coin counter is to the left of the coffee bean.
- Plant coins only go into the level supply; Zombie loot and all coins in the Zen Garden into the profile supply. Origin and special payout values ​​are retained for new coins via saving/loading. Unmarked coins already created before this update retain the profile path. [Behavior and native evidence](references/currency-20-0-0/port-notes.md).
- With the difficulty mod installed, the eye zombie replaces **Extremely 33%** of the suitable normal zombies. Otherwise it remains **1%**. Difficulty mod remains optional; Fusion does not contain any assembly dependency on it.
- The mod menu uses Chompzilla on light native lawn. The Hypnopea Zombie also has the imported Hypnopea Head in the Zombie Almanac and its cards.

The first version position increases due to the changed coin and extreme game rules. Both new fusions are removed from `fusionen.txt`. Game and test programs did not start. Compilation and static testing do not replace playtesting; the user takes care of this.

## Native Zombie Cards (19.1.1)

Zombie Library 1.0.1 replaces gray card areas with native Zombie frames and original almanac close-ups. The additional green names under cards are omitted. Names and descriptions remain in the native detail view. Own preview images differentiate between normal and rubber duck variants. Hover and controller selection use native marking. The changes are based on the screenshots `Bildschirmfoto_20261103_070315.png` and `Bildschirmfoto_20261103_070325.png`.

Game and testing will continue to be run solely by the user.

## Zombie Library (19.1.0)

Fusion now also requires `ZombieLibrary.dll`. Hypnopea Zombie and Eye Zombie register their existing saved identities in this library. Initialization and summoning take place via the library; Special behavior remains in fusion. The eye zombie's wake-up sound uses common sound registration.

The library adopts the previous native pool representation including waterline clipping, arm order and dancer stand-up phase. Both special zombies receive rubber duck variants. In addition, the eight previous additional native pool types are added as their own variants in the Zombie Almanac. The Plant Fusion tab contains a total of twelve entries: two special zombies and ten duck variants. Native zombie maps, backgrounds and detailed views are used.

Source code, API, optional example and manual checkpoints: [Zombie Library](../zombie-library/README.md). The library installer and Fusion release path install Plant Library, Zombie Library, and Fusion along with DLL backups. Save markers remain unchanged. Game and tests have not been run.

## Change 19.0.0

Plant Fusion now requires `PlantLibrary.dll` in addition to `LawnGlove.dll`. Plant registration, normal selection pages, mod tabs in the Almanac, and shared registration of projectiles, impact effects, and sounds are located in the [Plant Library](../plant-library/README.md). Recipes, ingredient usage, terrain rules, and fusion effects remain in Plant Fusion. Existing plant and projectile identifiers are retained. Registered plants from other mods are not treated as unchanged ingredients from their native host plant.

Fusion variants are regularly blocked. The Blood Sunflower becomes permanently selectable for the active profile after a merger has actually been completed. Direct debug spawning, infection and loading do not unlock them. A real fusion at the debug level also counts. The debug catalog remains complete. New activations will only be displayed the next time you open the regular plant selection. The library stores them separately under `UserData/PlantLibrary/unlocks.json`.

Build and install library and fusion together with `../plant-library/tools/install.py`. Example mod is located at `../plant-library/example-mod/`; their installation is optional. Game and test programs did not start. Compiling against the installed game references does not replace runtime checking. [Current Workflow](workflow.md).

## Change 18.1.4

Fix for red errors when continuing a level: The patch on `Zombie.Deserialize(ref ReloadedBinaryReader, int)` is no longer applicable. This means that the native binary zombie reader runs without the IL2CPP harmony trampoline. Volatile poison/freeze conditions will instead be cleared before `SerializationService.Deserialize()` for the affected board. Saved status effects remain unchanged and are restored as before with the first `UpdateGame`; Overheating remains stored in their native GridItems. No saves will be deleted, rewritten or reset. Release build and static release checks occur without starting the game. Game tests and test programs remain unexecuted at the user's request.

## Change 18.1.3

Fix release for the too big brain and the open left edge in the debug zombie roster. The brain is given a limited area at a distance from the counter slip and the top edge. The left background edge mirrors the closed right end of the native texture. [Details](references/roster-18-1-3/report.md). No new fusions. Game launch and play testing remain with the user.

## Change 18.1.2

Fix release for the debug zombie roster: card width, card height, horizontal position and spacing follow the visible native plant cards and their layout group. The position of the brain counter follows the sun label; The grid area now also includes the sun counter area. The brown background of the counter slip is removed when loading. Maps and click areas use the same geometry. No new plants, zombies or recipes. The game and test programs will not start; The user carries out the visual inspection. [Details](references/roster-18-1-2/report.md).

## Change 18.1.1

Fix release for the November 2nd response: The debug zombie rust replaces the orange-brown color with desaturated gray-purple. Its surface takes on the width, height and horizontal position of the actually visible plant grid; Map spacing and click lock follow the new geometry. The Eyed Zombie Duck is sized based on the visible imported torso rather than the hidden native duck pose. In the concentrated shot mode, six peas were present as two overlapping groups of three. Now the peas within both groups follow at an interval of 0.08 seconds. Number, base damage and attack rate remain the same. [Receipts and User Verification](references/feedback-18-1-1/report.md). Game and test programs will not start.

## Change 18.1.0

Two new fusions: **Multiple Pea Shooter** (Peashooter + Threepeater) with three saved shooting modes and **Seaflowerpot** (Flower Pot + Sea-shroom) with water check and non-stackable 50 percent speed bonus for the available kelp hybrids. Original models, all animation clips, fertilizer treatment and German/English almanac are added. Both completed recipes are removed from `fusionen.txt`.

The debug zombie roster uses the native plant roster background with purple coloring, an original brain sprite and a ∞ counter. Map names are dark, conform to the card area, and receive ellipses when necessary. The white information text is omitted. With a zombie or plant card selected, the entire zombie roster has 20 percent opacity and does not block clicks. Placement and Cancel restore the display. The native sun display also shows ∞ in debug mode. [Receipts and User Verification](references/multipeater-sea-pot-18-1-0/report.md). Game and test programs will not start.

## Change 18.0.6

Fixed release for the severed head of the carried Imp on Gargantuar and Giga-Gargantuar in the pool. The native floating rectangle now extends a full screen height above the waterline on these two types. Its bottom edge remains unchanged; Body trimming, duck and arm order, and imp litter remain responsible. No new fusions; `fusionen.txt` remains unchanged. [Cause and user verification](references/imp-clip-18-0-6/report.md). Game and test programs will not start.

## Change 18.0.5

Fix release for the pool graphics. Only the left arm is sorted in front of the rubber duck; the right arm remains behind. The remaining submeshes retain their native character order within a common SortingGroup, so that the football helmet, head and Gargantuar body in particular are not swapped with the same sorting values. The duck reflection takes over the sign of the current pose even when the dancer turns around. The Yeti duck is reduced in size by 15 percent. In the case of the eye zombie, the duck lies within its imported sprite group between the body and the left arm. No new fusions; `fusionen.txt` remains unchanged. [Causes and User Verification](references/pool-layers-18-0-5/report.md). Game and test programs will not start.

## Change 18.0.4

Fix release for remaining UI, flight and pool bugs. Golden bean monetary amount and slowdown label inherit the appearance and actual font size of the native fast-forward display. Flytrapper/Flying Chomper uses the same flight layer as the other flying plants, including map, debug map and glove placement. Spine arm boundaries now start in the correct slot and follow draw order changes. Ducks and imported sprites use the actual native cutting edge including controller and screen offset instead of a shadow estimate. No new fusions; `fusionen.txt` remains unchanged. [Evidence and user verification](references/feedback-18-0-4/report.md). Game and test programs did not start.

## Change 18.0.3

Fixed release for the golden coffee bean, slowdown, flying plants, pot placement with the glove and pool graphics. The bean receives the native button background and is centered over fast-forward; The slowdown text is on the left and only appears when slowdown is active. The flight layer uses an explicit mod identity instead of the native coffee bean classification. Ground plants are retained when placed. The gauntlet is allowed to place normal plants on empty native and fused pots. Pool cropping uses rectangle pixels; synthetic ducks swim at the waterline. Existing spine separators are expanded and their arm order is set according to the actual mesh construction. No new fusions; `fusionen.txt` remains unchanged. [Causes and User Verification](references/feedback-18-0-3/report.md). Game and test programs did not start.

## Change 18.0.2

Fix release for flying plants, debug glove and pool graphics. Flight placement also uses the native Blover rules for fear drones and takes native flying plants into account when gauntleting. Ground plants are preserved; Ingredient fusions take priority. The debug glove recharges immediately after use. Gargantuars, Giga-Gargantuars, jack-in-the-box, dancers, group dancers and football zombies get the actual body transfer into the water. The previous pure trimming offset is no longer necessary. New rubber ducks retain the original aspect ratio; native arm parts are rendered in front of it. No new fusions; `fusionen.txt` remains unchanged. [Evidence and user verification](references/pool-air-18-0-2/report.md). Game and test programs did not start.

## Change 18.0.1

Fix release for debug almanac and pool zombies: Already bound almanac models are updated upon entry and exit of the debug session. Called group dancers first complete their native surfacing movement and then begin normal water entry. Gargantuars, Giga-Gargantuars, dancers, group dancers and jack-in-the-box are depicted more deeply. Rubber ducks use the running spine pose and a native sprite material; their size follows the actual body graphic. Imported sprites are cropped at the waterline, including the eye zombie. No new fusions; `fusionen.txt` remains unchanged. [Causes, limitations and manual testing](references/pool-almanac-18-0-1/report.md). Game and test programs did not start.

## Change 18.0.0

Two new fusions: **Wegblastopf** (Flower Pot + Blover) and **Angsttopf** (Flower Pot + Scaredy-shroom), each in both card and glove directions. Original graphics, almanac, fertilizer treatment and saved attack/projectile bonuses added. Football, jack-in-the-box, dance, group dancer, Yeti zombies and both Gargantuars are also allowed to use water rows in existing pool levels. These zombies and the eye zombie automatically receive an original rubber duck and the native water transition. Debug almanac including previously blocked entries released, without profile change. [Receipts, adjustments and user verification](references/support-pots-pool-18-0-0/report.md). Game and test programs did not start.

## Change 17.0.4

Status **17.0.4**: Grasping algae no longer pushes zombies out of native bite range after bite contact is achieved. Ranged recoil, damage and water grass remain. The check also takes into account zombies whose next native feeding tick is still pending. [Cause and user verification](references/grapple-bite-17-0-4/report.md). Game and test programs did not start.

## Change 17.0.3

As of **17.0.3**: The common fear check is separated from damage categories. Even nearby enemies outside the ground category interrupt shots in both neighboring rows and trigger the fully charged Frenzy Mushroom explosion. Grasping algae and algae nut use a neutral, edible aquatic plant identity in native target testing. The Squash Zombie gets the native pool transition. Scharfertang uses the correct original prefab and plays its handle even with natively initialized or loaded target coordinates. [Receipts and User Verification](references/feedback-17-0-3/report.md). Game and test programs did not start.

## Change 17.0.2

As of **17.0.2**: Frenzy Mushroom triggers its charged Fear Explosion immediately upon proximity, including neighboring rows. Scaredy Fusions continue to interrupt their shots on nearby zombies. Slowdown takes the actual calculated size and the native button background. Grapple algae is eatable by zombies. The original test confirms that the triple tang has a consumable grip; three denotes his firing lines. [Causes and User Verification](references/feedback-17-0-2/report.md). Game and test programs will not start.

## Change 17.0.1

As of **17.0.1**: Fixes to gray debug fusion maps, fusion previews, Frenzy Mushroom targeting rows, slowdown background, original Gloom smoke, Grasping Algae geometry, and Sea Sun Mushroom size. Triple tang is used up after the one-time tang grip as in the original. Hypnoweher only works on already hypnotized zombies. Sea Nut and Sea Pumpkin remain unchanged after user confirmation. [Causes, testing and dismantling](references/feedback-17-0-1/report.md). Game and test programs will not start.

## Change 17.0.0

As of **17.0.0**: User fixes to shooting/status effects, corruption synergy, pumpkin gauntlet placement, line fire, seaweed, head physics, and aquatic plant graphics. Sea squash can now stand on water without a water lily; therefore the gameplay number increases. [Changes, existing fixes and user review](references/feedback-17-0-0/report.md). Game and test programs did not start.

## Change 16.0.0

Status **16.0.0**: With the native threepeater, the triple pot now fires its two additional projectiles 0.2 seconds after the sixth main pea. This means that the additional projectiles fly behind the normal volley. The delay uses saved game ticks and pauses the game. Shot limit and half additional damage remain. The user log confirms that the additional floors were already created in 15.0.7. [Behavior, Evidence and User Verification](references/tri-pot-delay-16-0-0/report.md). Game and test programs did not start.

## Change 15.0.4

As of **15.0.4**: Enflamed turns orange on the actual native controller color recorder. The later drawing path can no longer overwrite the status color with its base color. Old color state cache no longer applies; Poison uses the same corrected path. [Cause and user verification](references/enflamed-tint-15-0-4/report.md). Game and test programs did not start.

## Change 15.0.3

As of **15.0.3**: The additional night pool without fog now uses the native ClearAlpha character path instead of a permanently set blover countdown. Normal night pool remains with fog. [Cause and user verification](references/debug-no-fog-15-0-3/report.md). Game and test programs did not start.

## Change 15.0.2

Status **15.0.2**: Large debug zombie maps are again using their visible image as a clickable button area. Orange selection coloring, preview and spawn are given a selected zombie identity again. [Cause and user verification](references/debug-zombie-cards-15-0-2/report.md). Game and test programs did not start.

## Change 15.0.1

As of **15.0.1**: Debug input in the main menu uses the native level selection state instead of the GameObject activity. Keypresses are read at the beginning of LateUpdate; parallel menu flags are no longer required. [Cause and test instructions](references/debug-input-15-0-1/report.md). Game and test programs did not start.

## Change 15.0.0

Status **15.0.0**: Extensive fixes to debug input and placement, flying plants, mines, seaweed, shot counters, projectiles, original effects, status colors, beam muzzles and rendering orders. Additional night pool without fog; Fast-forward with 5x, 10x, 20x and right click to downshift. No new plants. [Changes, Evidence and User Review](references/bugfixes-15-0-0/report.md). Game and test programs did not start.

## Change 14.0.6

As of **14.0.6**: The Eye Zombie size reference now includes the full standard native head including jaw and hair. An early pose without a head again resulted in a target height that was too small, despite the correct source measurement. The measurement reads the native head attachments without changes to the game state. [Cause and evidence](references/eyes-complete-height-14-0-6/report.md). The game and test programs remain with the user.

## Change 14.0.5

Status **14.0.5**: The height measurement of the eye zombie only takes visible body parts into account. Disabled effect and head sprites had increased the measured source height and reduced the visible model. The scale still follows the true native body height. [Measurement evidence and user verification](references/eyes-visible-height-14-0-5/report.md). The game and test programs remain with the user.

## Change 14.0.4

As of **14.0.4**: The eye zombie uses the non-generic Unity interface when accessing the native spine renderer. This bypasses the IL2CPP constraint error documented in the user log, which caused the original model to be discarded. Native height measurement including data asset scaling is retained. [Error receipt and testing](references/eyes-interop-14-0-4/report.md). The game and test programs remain with the user.

## Change 14.0.3

As of **14.0.3**: Eye Zombie measures the current native spine geometry including the previously overlooked data asset factor of 0.564. No more dependency on readable mesh buffers and no more unscaled size replacement value. [Cause, interfaces and user testing](references/eyes-scale-14-0-3/report.md). The game and test programs remain with the user.

## Change 14.0.2

As of **14.0.2**: Pot speed bonuses receive ongoing shot animations and save unused credit. Protection for all Ember/Cherry Shooters including Split and Gatling; own fusion salvos additionally checked. Eye Zombie measures visible native mesh triangles instead of renderer bounds corrupted by invisible locator points; its original model is preserved for unreadable meshes with an occupied body reference. [Causes, affected plants and user testing](references/repeater-eyes-14-0-2/report.md). The game and test programs remain with the user.

## Change 14.0.1

Status **14.0.1**: Eye Zombie measures body heights in the local sprite layer and limits invalid scaling. Iron star, chili star, star nut and star caps start above the proven native soil limit; Chilistar displays native line fire and melts ice trails. Common final hit path damage ratio: 20/20 = 1.0, including area attacks, beams, and poison. Firepot Almanac lists all fire recipients and explains why Rotten Mushroom and Triple Mud Pumpkin do not receive a bonus. [Causes, sources and testing](references/regressions-14-0-1/report.md). The game and test programs remain with the user.

## Change 14.0.0

As of **14.0.0**: Triple Lantern Leaf and Aurora Borealis Lantern Leaf with original assets, light/laser abilities, fertilizer and DE/EN almanac. Fixed: Bite damage scale, star clocks/chilli star death, fire pot jalapeno, horizontal original lasers, foul mushroom smoke/eyes, eye zombie size/frost, catapult impact, and bouncy explosion graphics. Debug plant names without leading `$`. [Receipts, damage comparison and user verification](references/fusion-repairs-14-0-0/report.md). Game and test programs did not start.

## Change 13.1.0: Phoenix Triplinator, Solar Catapult and bug fixes

- New: Charred Threepeater + Jalapeno = Phoenix Threepeater; Cabbage desk + Sunflower = solar desk. Both source recipes are fully registered, including original models, bullets, abilities, storage, fertilizer and German/English almanac.
- Pot selection in the lower third of the field added. Tempo/Quitter Mushroom use original green/blue spores. Catapults share a corrected flight calculation.
- Eye zombie retains visible butter and maintains normal body size. Hochwall-Nuss uses her real, original portrait. Apocalyptoblatt shows original DoomSplat instead of FumeCloud.
- Ashen Triplinator awakens as a Phoenix through a recipe or fire series. Chilistar completes his one-time star volley on the ignition timer. Light peas use sprite lasers without the LineRenderer error documented in the user log. Dual solar nut retains its solar ball graphic.

[Sources, limitations, compilation evidence and manual checklist](references/fusion-repairs-13-1-0/report.md). Game and test programs were not started by the agent. Workflow remains unchanged.

## Change 13.0.9: native zombie variants and correct grid lane

- The six plant head zombies use images rendered offline from native head skeletons, including Zombotany textures, skins and prefab offsets. Whole plants with stems are no longer placed on trimmed bodies.
- Giga Gargantuar gets its real Red-Eyes skin; Target Zombie its full native target model with arms and shield intact. Eight corrected variants are available in maps and grid previews from the same atlas. The personal Zombatar remains with the common standard static body.
- Grid previews use the foot line of their actual row instead of the top edge. The line height comes from the board: 100 or 85 pixels. Roof pitch remains natively taken into account. Eye zombie and native zombie sprites use the same bottom image anchor.

Fix release **13.0.9**. [Sources, display limits, testing and release](references/zombie-accuracy-13-0-9/report.md). Offline asset conversion, file viewing and compilation only; The agent did not start the game or test programs. The already running user session will only load this DLL after a complete restart.

## Change 13.0.8: Preserve card graphics across scene changes

Unity's scene cleanup was able to remove the preloaded Atlas sprites and textures, although their C# cache entries continued to exist. This left fusions/zombies without graphics; The eye zombie then aborted the model build with a NullReferenceException when reading the destroyed background texture.

All self-created card resources now receive DontUnloadUnusedAsset immediately upon creation. The session owner still explicitly shares them. This includes fusion atlas, zombie atlas, all sprite areas, card background and eye zombie graphics. Atlas/selection architecture and the removal of unstable live recordings are retained.

Fix release **13.0.8**. [Log receipts, lifecycle and manual review](references/chooser-assets-13-0-8/report.md). Game and test programs did not start.

## Change 13.0.7: Zombie cards without live capture

The second abort occurred again immediately after normal recording began, this time with the selection fully visible. The later release of 13.0.6 did not fix the error. The live capture path is now completely removed, including its initialization/reanimation patches. Maps no longer create Zombies, Almanac Binders, Cameras, or RenderTextures.

All 37 native zombie types have pre-exported raster images and full maps in the embedded atlas. The display is available immediately and does not require an initial cache fill. Variants without their own static preview art have an explicit limit: six plant head zombies combine original body and original plant art; Red-Eyed Gargantuar uses the Gargantuar image, Zombatar uses the normal body image. Names and actual spawn identities remain separate. Eye Zombie retains its own original artwork.

Fix release **13.0.7**. [Abort findings, sources, representation limits and testing](references/chooser-crash-13-0-7/report.md). Game and test programs did not start. The suspicious native generation path is removed; Term confirmation remains with the user.

## Change 13.0.6: Portrait capture only after menu initialization has been completed

The user run log with 13.0.5 ends right after the first portrait capture, just 82 ms after the board ready message. An existing board is not sufficient as a release for native display objects. Portrait shots now wait for the native selection panel to be bound, active, and fully displayed in the normal selection state; they do not run during transitions. Preloading is still possible in the started game. Cache, map atlas and preview functions are retained.

Fix release **13.0.6**. [Abort documents, change and test limit](references/chooser-crash-13-0-6/report.md). Without a crash dump, the exact native cause of the crash is not proven. Game and test programs did not start.

## Change 13.0.5: Card cache, full frames and fusion preview

- All 291 fusion/object maps are included in a pre-generated atlas with a complete map frame. Individual portrait PNGs are no longer decoded when browsing. The previous mesh callback no longer creates UI objects.
- Select/Remove retain the current tab's grid list. The seed bar reports its complete change once instead of after every packet.
- Zombie portraits are prepared during plant selection and saved to UserData/FusionMod/zombie-portraits-v2-<native SHA256>/. Further game starts use the cache. A first generation remains necessary. Invalid recordings will not be saved. Zombie cards receive the same frame; the grid preview uses the image without a frame.
- Leading dollar signs are removed from the localized zombie name components.
- Merge/object previews on mouse and grid use exact map identity and existing imported model renderer. Reused controllers are also updated when switching between fusions of the same basic type. There are no toy plants or abilities created for this.
- Further log corrections: Release focus before releasing the binder models; Evaluate three-head alpha via shoot1/2/3 instead of a missing shoot clip; Obtain portrait context from the living debug board.

Fix release **13.0.5**. [Evidence, Limits and Manual Verification](references/chooser-fix-13-0-5/report.md). Game and test programs did not start. After changes to the almanac portraits, tools/build_debug_cards.py rebuilds the atlas; the static check detects outdated sources.

## Change 13.0.4: Card identity, seed bar and page performance

- The selection uses the specific card instead of its base plant type used multiple times. As a result, fusions end up in the native seed bar. Remove runs without recursively calling the two native overloads; the remaining cards are completely reassigned via the native bank allocator. Debug cards are assigned immediately; The previous map flights are omitted from this selection.
- Selected cards belong exclusively to the seed bank and are not reinserted into the selection grid when scrolling. The map models persist between tabs. Changes to the native model lists are reported in bulk; identical lists do not create rebinding. The unchanged impersonator list is not recreated for every tab.
- Fusion portraits are decoded at most individually per frame, instead of collected in a page click. Until then, the native card background remains visible. Fusions and Zombies get the same native blank card background, space for costs, and undistorted portraits. This also applies to your cards in the seed bank.

Fix release **13.0.4**. [Causal evidence, changes and manual review](references/chooser-fix-13-0-4/report.md). Game and test programs remain closed.

## Change 13.0.3: stable selection and accessible card arrows

- The debug selection only sets map positions during page changes and state transitions. Native binders maintain control of screen coordinates; The previous feedback with the mod continued to cause the map to be constantly rebuilt despite the index error being fixed.
- The side arrows directly use the native acceleration button including its loaded prefab instances. You no longer need the button cache that was set in `Awake`. Both stand in the appropriate canvas size above the stable imitator frame. Panel and map anchors are resolved using their own model key.
- Invisible additional pages do not create card mockups or portrait subscriptions. Selected cards are retained. Additional maps receive the same flight time update and native landing completion as the original 49 maps.

Fix release **13.0.3**. [Findings, native evidence and manual checklist](references/chooser-fix-13-0-3/report.md). Game and test programs did not start.

## Change 13.0.2: Re-enable selection menu after log error

The debug selector now consistently uses `NumSeedsInChooser` (49) for native maps, impersonator, page start, and switch to zombie selector. `NumSeedTypes` (53) also contains four special plants without a selection card. The previous imitator path therefore read outside of its 49 entries; the aborted model tick left the selection dirty and constantly rebuilt cards and tweens. The first four additional cards now also have their correct tab position. Native maps, tabs and impersonator are retained.

Fix release **13.0.2**. [Causal Evidence, Testing and Remaining Limits](references/chooser-fix-13-0-2/report.md). Game and test programs did not start.

## Change 13.0.1: native debug card tabs and log fixes

- The additional green plant selection and the separate fusion bar are no longer required. All cards now appear in the existing native **Choose Your Plants** space and land in the same native seed bank. Tab 1 contains the normal plants; From Tab 2 onwards, objects and fusions follow with original portraits and their calculated solar costs.
- Two directional buttons derived from the native fast-forward button are located above the impersonator. Page forward/backward receives the selected cards and waits for ongoing card flights. Confirmation and subsequent zombie selection remain native. There are still a maximum of ten plant/fusion/object cards.
- The `ModelBase being disposed multiple times` log error is addressed at its source: the filtered card list uses common references instead of a second owner of the same card models.
- Zombie portraits use the actual generated native zombie controller instead of the unoccupied binder field. Planthead/Zombatar initialization gets limited app context; Head reanimations created in this way remain under the isolated almanac binder. Presentation items will be removed without the invalid gameplay death path for them.
- RenderTextures are also separated from cameras that are still connected at their actual release boundary; the existing treatment at exit remains intact.

Fix release **13.0.1**. [Log findings, native evidence, limits and manual checklist](references/native-debug-cards-13-0-1/report.md). Game and test programs did not start. Workflow, fusion list and recipe source remain unchanged.


## Change 13.0.0: Debug tabs, glow clover, frenzy mushroom and mushroom fixes

- New: **Luminous Clover / Glowver** and **Frenzy Mushroom / Frenzy-shroom** with all available source recipes, original models and animations, abilities, projectiles, fertilizer treatment and complete German/English almanac. Both completed entries are removed from `fusionen.txt`. Frenzy Mushroom uses the minimum occupied interval of **0.2 seconds** and takes the damage of an Ominous Pea immediately to the right.
- Debug plant selection has the **Plants**, **Fusions** and **Objects** tabs. All 288 almanac results are available as fusion cards. Fusion cost uses the cheapest complete prescription route available; Loose ingredients cost **100** each. Supported objects include bucket, football helmet and eye zombie head. A total of ten plant/fusion/object cards are possible; Zombies are then chosen separately. Gold upgrades take your first bean payment into account. Fight money and sun remain unlimited in the debug level.
- Mini Ice Mushroom checks contact in skill cycle and freezes its ground target without additional damage. Mini Hypno Mushroom is triggered by the Eye Zombie's own bite timer. Shrunken zombies get a native vertical bitmap shift fix; the imported eye zombie graphic follows the reduced silhouette.
- Replaced mushroom skill paths retain the coffee wake-up counter including wake-up tone. This particularly applies to sun storage mushrooms and ice cream mushrooms.
- Soot Mushroom uses the original black smoke **FumeDoom**, including its particle size, lifespan, velocity and emission rate. Icicle Mushroom has the full row range of its true source parent type and enforces its ice projectile graphic upon labeling.
- Doom Spores grow around their ground anchor instead of the scene position of the exported prefab. The display is smaller. Hypnopea zombies wear the original animated Hypnopea head. Zombie placement previews lose the additional 65 pixels vertical offset. The eye zombie adopts cold as a structure-preserving color tint.

New debug game rules use the first version location: **13.0.0**. [Sources, exact limits, build receipts and manual checklist](references/debug-fusions-13-0-0/report.md). Game and test programs did not start. Workflow unchanged.


## Change 12.0.0: Perfume mushroom, seaweed stinger and fixes

- New: **Perfume-shroom** and **Tangstachel / Tangle Weed**, with all available source recipes, original models/animations, smoke profile, abilities, fertilizer treatment and German/English almanac. Both completed entries are removed from `fusionen.txt`.
- Coffee awakens all three mushrooms in the same cell. For pot recipes, the bottom third fuses the pot; The top is planted normally or the resident is merged. Pot fusions appear smaller.
- Speed/Quittershroom projectiles use the animated firing point and appropriate bullet graphics, also after Torchwood. Giant Wall-nut no longer suppresses normal zombie death animations.
- Debug zombie selection: only zombie maps, native full variant portraits, undistorted images and names from the current game translation.
- Last red log error `Releasing render texture that is set as Camera.targetTexture!`: Camera targets are already separated in native `Application.quitting` before renderers release textures.

Gameplay class: **12.0.0**. [Sources, Capabilities, Build Receipts and Manual Checklist](references/fusions-fixes-12-0-0/report.md). Game and test programs were not started by the agent; In particular, new debug portraits and shutdown sequence remain to be checked at runtime. Workflow unchanged.

## Change 11.0.0: four mine fusions, slowdown and HUD

- New: **Tangle Mine**, **Kelp Mine**, **Sea-mine** and **Potato Masher** with all available source recipes, original graphics/animations, skills, fertilizer treatment and German/English almanac. Four completed entries are removed from `fusionen.txt`. None of these fusions shoot projectiles; Potato Mash is a native Squash spin-off with original graphics.
- Note direction: Schlingwrasse on Potato Mine produces Tangle Mine; reversed kelp mine. Potato mine on sea mushroom makes sea-mine; conversely, land remains mine. Potato Masher works both ways.
- Money display and gold bean are above Fast Forward. In the debug level, fighting money is infinite, both displays show ∞; Profile/shop money remains intact.
- New slowdown next to Fast Forward: mirrored original 1.5× symbol, toggle with visible marking, exactly **0.2×**. Switching off takes over the currently selected fast forward speed. Pause remains native.
- Marigold and Flowerpot projectile errors: own production counters no longer enter the native firing path. Failed debug zombies are cleanly aborted including partial instances; Special types that are not available show a message. When exiting, camera RenderTextures are separated before release.

Gameplay class: **11.0.0**. [Sources, exact capabilities, directions, build receipts and manual checklist](references/mines-11-0-0/report.md). Game and test programs were not started by the agent. Workflow unchanged.

## Change 10.0.0: Gold bean, two gold upgrades and native debug minigame view

- The original gold bean icon is next to the money counter. Collected coins also fill a saved level supply; Profile/shop money is not spent. Select bean, then click silver plant or gold plant. Right click and escape abort without consumption.
- The two confirmed upgrades are Golden Nut and Golden Sunflower. Silver Nut costs 1000 including the first healing/boosting ability. Silver Sunflower initially costs 3000, doubled for each Gold Sunflower present. Additional abilities cost 1000: Nut strengthens and heals; Sunflowers collectively pay out their stored sun. In the debug level, these actions are free. Original models, animations, portraits and English/German almanac texts are included. Both have no projectiles.
- Fertilizer cures both gold plants. With the golden sunflower, it also shortens the next production to a maximum of 0.5 seconds; he does not buy any ability or convert any silver plant into gold.
- Debug uses the original native minigame template with scroll grid and background thumbnails. Plant/zombie selection and intro remain in the existing native flow. Template and profile data are not changed.

New gameplay rule through paid gold upgrades: **10.0.0**. `fusionen.txt` remains correctly unchanged because these tool upgrades are not there as plant pair recipes. Open further fusions with gold ingredients remain. [Sources, operation, storage contract and manual checklist](references/golden-bean-10-0-0/report.md). Game and test programs did not start. Workflow remains unchanged. Previous DLL: `backup/FusionMod-9.0.0.dll`; previous sources: `backup/golden-bean-9.0.0.tar.gz`.

## Change 9.0.0: Fusion effects, mushroom graphics and reliable eye zombie bites

- If a real Ice-shroom is consumed during a successful fusion, its global ice effect occurs. Card and Gauntlet Fusions use the same finisher. If there are multiple small results, an incoming Ice Shroom card works exactly once. Existing Ice Storage in Cryo Pumpkin and Frost Nut Healing use their existing rules. Loading and failed fusions do not trigger anything.
- Courage smoke originates from the original animated `Shoot` point instead of the offset native controller origin. All direct Stink Mushroom smoke variants now use original sprites and emission profiles: including Stink Mine, Ice, Fire, Magnet, Iron and Sting Smoke. Multi-row beams receive the appropriate row offset. Damage remains in the existing simulation path.
- All eight Fear Mushroom Fusions use the same conversion from original dimensions to playing field dimensions. Speed, fear bomb, potato, sun, star, nut and camouflage variants are thus aligned with the already suitable ice variant. The associated pea, cherry, potato and ice projectiles use the same scale; Courage fungus spores too.
- With the Trembling Mushroom and the other Fear Mushroom shooters, an already valid general pea view is no longer sufficient: the view must match the saved projectile type. General pea graphics do not claim marked fear mushroom projectiles.
- The Eye Zombie no longer loses bite events on the native target check at intervals of four or eight ticks. Only during this test is the age cycle set appropriately and then restored. Damage remains 50 per bite; quick bite animation, pause, cold, butter and ice retain their responsibility.
- The Frost Snapper begins a bite against unfrozen targets only within the actually damaging ice pulse. Frozen swallowable targets remain permitted within the longer feeding range. Attack Start and Ice Pulse use the same geometric check.
- Mine Fusions use the native Potato Mine explosion effect instead of the cherry graphic. This also applies to other abilities that use the same mine detonator.

Gameplay change caused by the Ice-shroom fusion effect: **9.0.0**. No new plants, zombies or recipes. [Receipts and manual checklist](references/fusion-effects-9-0-0-native/report.md). Game and test programs did not start. Previous DLL: `backup/FusionMod-8.0.0.dll`; Source and resource archives are located at `backup/fusion-effects-8.0.0.tar.gz` and `backup/fume-mine-effects-8.0.0.tar.gz`. Supplementary original files: `backup/ScaredContent-8.0.0.cs` and `backup/ChomperVariants-8.0.0.cs`.

## Change 8.0.0: Debug placement and eye zombie graphics

- In the debug level there is no automatic sun falling from the sky. Plant sun remains part of the respective plant function. This gameplay change increases the first version location to **8.0.0**.
- Zombie placement shows a semi-transparent zombie image at the current grid. The preview does not create a real unit and disappears outside valid fields when canceling, choosing a plant, pausing and at the end of the level.
- The debug zombie bar is hidden during pause and does not process board clicks there.
- After zombie selection, the native plant models including image references are explicitly restored. This means that identical numeric plant and zombie slots do not retain a zombie image in the plant bar.
- Emptying the selection immediately resets empty cards. `SetPacketType(None, None)` previously called `Plant.GetRefreshTime` without a plant definition and produced 40 `NullReferenceException` messages in the last log. Hidden chooser slots including impersonators remain available for the native binders; This addresses the missing `all.48` models logged there.
- The eye zombie's sloping head uses its imported head art on top of the native head effect. Movement, physics and lifespan remain native. The separate 1% head drop remains unchanged.
- The imported body graphics adopt native color and additive material effects for hit flash and cold; the shadow receives no hit flash.

No new plants, zombies or recipes. [Causes and manual checklist](references/debug-eyes-8-0-0-native/report.md). Static testing and compilation only; Game and test programs did not start. Previous sources: `backup/debug-eyes-7.0.0.tar.gz`; previous DLL: `backup/FusionMod-7.0.0.dll`.

## Change 7.0.0: Debug without plant cooldown and robust selection switching

- Plant cards can be immediately reused in the current debug level. The release also applies to initial load times and impersonators. Native planting, consumption meters, and fusion events continue; normal levels retain their loading times.
- Switching to zombie selection finds the panel actually connected to the current chooser via its native data binding. There is no need to search for names across all loaded panels, including inactive ones.
- The waiting point takes effective visibility into account: If the native state tree hides a parent container, its deactivated hierarchy is sufficient. A panel within it does not have to change its own state to hidden. This means that the change no longer blocks in this state.
- Zombie maps start at their native grid positions. Previously selected plants may have already moved their map position to the top bar; these positions are no longer used as starting points for zombie selection. Two log entries mark plant confirmation and opening of the zombie selection.

Gameplay change due to lifted loading times: **7.0.0**. No new plants, zombies or recipes. [Receipts and manual checklist](references/debug-cooldown-7-0-0-native/report.md). Game and test programs did not start. Previous project files: `backup/debug-transition-cooldown-6.0.0.tar.gz`; previous DLL: `backup/FusionMod-6.0.0.dll`.

## Change 6.0.0: Native debug selection and coin-free zombies

- After selecting the background, the native level path starts with the original plant selection. The additional self-built plant view is no longer necessary. Zombie selection then uses the same native chooser with its map flight, selection bar, removal and controller operation.
- The phase change uses the native panel animation and waits for it to end. After zombie selection, the native chooser ends the selection; The normal level intro with tracking shot and “Ready, Set, Plant” continues. The previous direct call to StartPlaying is no longer necessary.
- Debug offers ten places regardless of profile. One to ten plants and zero to ten zombies are allowed. Imitater keeps the native selection dialog and the actual selected imitation type. Profile and scores will not be changed.
- Zombie maps in the level use the blank native card background instead of the colored rectangular box. Zombie images and variants remain.
- Zombie.DropLoot is locked to the debug board only. With this, killed zombies including Yeti do not generate coins or other native loot.

Gameplay Change: **6.0.0**. No new plants, zombies or recipes. `fusionen.txt` remains unchanged. [Native evidence, limits and manual checklist](references/debug-native-6-0-0/report.md). Game and test programs did not start. Previous project files: `backup/debug-native-5.0.1.tar.gz`; previous DLL: `backup/FusionMod-5.0.1.dll`.


## Change 5.0.1: Load preview images of the debug cards

- Plant and Zombie cards now assign their image reference via `AddressableSpriteValueModel.Reference`, like the normal native card choice. The previously used constructor ignores its sprite argument in this version of the game and therefore does not start a loading process. That explains the empty cards without error messages.
- The existing UI continues to take over loaded images asynchronously. The same fix applies to the selection grid, top selection bar, plant head icons and zombie cards in the level. Eye Zombie image and resource sharing remain unchanged.

Fix version **5.0.1**. [Native evidence and manual checklist](references/debug-card-images-5-0-1/report.md). Game and test programs did not start. Previous DLL: `backup/FusionMod-5.0.0.dll`; previous project files: `backup/debug-card-images-5.0.0.tar.gz`.

## Change 5.0.0: Debug image cards, tools and quick eye zombie

- Debug uses the same face card view with a top selection bar for plants and zombies as a normal card selector. First select and confirm plants, then select zombies. Both selections are limited to ten; a full selection does not allow additional cards. Selected cards can be removed again at the top. It is still possible to go back to choosing plants.
- Zombie maps also show their previews in the current debug level. All ten fit into the bar at the same time. Native plant/zombie preview images load asynchronously and are released per session. The eye zombie uses a full body image exported from its original model. For native variants without their own preview image, the native body image is used; Zombotany cards also receive a plant head symbol.
- Debug unlocks the native shovel. The existing Lawn Glove mod uses the same shovel button: right-click switches between shovel and glove; Left click activates the selected tool. Profile activations will not be changed.
- After opening its eyes and completing the wake-up animation, the Eye Zombie runs twice as fast. His feeding clip and damaging bite events run twice as fast as before. Damage remains 50 per bite. Break, butter, ice cream, cold and hypnosis remain native rules. Temporarily scaled motion values ​​are restored immediately after the native run call.

Gameplay changes increase the first version location to **5.0.0**. No new plants, zombies or recipes; `fusionen.txt` remains unchanged. [Implementation, sources and manual checklist](references/debug-cards-rage-5-0-0/report.md). Game and test programs did not start. Previous DLL: `backup/FusionMod-4.0.0.dll`; previous project status: `backup/debug-cards-rage-4.0.0.tar.gz`. The previous version of the resource check is also under `backup/debug-cards-rage-4.0.0-checker.py`.

## Change 4.0.0: Debug map bar, manual zombies and eye zombie graphics

- The debug plant bar is explicitly made visible after native launch and published as changed. The selected plants remain in the native map bar; the existing data model path updates visibility and entries.
- In the debug level, automatic wave, grave, water and sky spawns are blocked. Zombie maps still use native generation. Normal levels retain their spawns. Special abilities of already placed zombies remain part of their native behavior.
- The Scared animation adapter now uses the angle matrix populated from the original shader. The previous axes and signs twisted body parts. The jointly used correction also applies to the blood sunflower.
- The eye zombie gets size and ground anchor from the actual native shadow instead of the skin origin. The native double shadow is hidden and restored when released. The imported body and its shadow use the same hierarchy and ground point; Native skin animations do not change this hierarchy.

The changed debug spawn rule increases the first position to **4.0.0** according to the version scheme. No new plants, zombies or recipes; `fusionen.txt` remains unchanged. [Sources and manual checklist](references/debug-roster-eyes-4-0-0/report.md). Game and test programs did not start. Dismantling when the game is closed: Copy `backup/FusionMod-3.0.3.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/debug-roster-eyes-3.0.3.tar.gz`.

## Change 3.0.3: Fusion results and operable debug selection

- Right-clicking on a native almanac plant now shows the **achievable fusion results** instead of the ingredient partners. Each result appears once with its name and picture. Its detailed view lists the appropriate ingredients and recipes for the plant you clicked on. Alternative recipes are collected under the same result.
- The debug dialog and its zombie cards use native Unity UI buttons with the existing EventSystem instead of IMGUI. Background click opens the map selection; You can choose up to ten plants there and zombie cards in a separate tab. Only “Start 3rd debug level” starts the level with the selected environment. All native plants can be selected regardless of profile activations. Imitators and debug-only activations continue to use the existing session path.
- Background selection, start request and finished debug board each write a log entry. Failed actions show their error message in the dialog. The level copy, the native start path and the existing memory locks remain responsible.

Fix version **3.0.3**. No new plants, zombies or recipes; `fusionen.txt` remains unchanged. [Causes, Limitations and Manual Checklist](references/almanac-results-debug-ui-3-0-3/report.md). Game and test programs did not start. Previous DLL: `backup/FusionMod-3.0.2.dll`; previous project status: `backup/almanac-results-debug-ui-3.0.2.tar.gz`.

## Change 3.0.2: Background selection in the debug dialog

- The background selection uses ten labeled radio buttons per page as well as forward/backward pages and a quantity display. `GUI.BeginScrollView` is not available in this replanted version and stopped displaying immediately after “Select Background”. The dialog now only uses the buttons and labels already used for card selection.
- After the background selection, the card selection begins again on page one. An empty background list shows an explicit message. The selection offers text buttons, not image previews.

Fix version **3.0.2**, no new plants, zombies or gameplay rules. [Cause and manual checklist](references/debug-background-picker-3-0-2/report.md). The game and test programs remain unstarted. Previous DLL: `backup/FusionMod-3.0.1.dll`; Project files: `backup/debug-background-picker-3.0.1.tar.gz`.

## Change 3.0.1: Debug entry, free partial slots and almanac images

- `debug` in the main menu now uses the existing gameplay chooser after service injection from the active main menu. The previously silent cancellation due to a lack of services is no longer necessary without using the normal save file loading path.
- Small plants move to the first free subspace in the same field without a suitable fusion if the clicked subspace is occupied. A free clicked part remains preferred. Cards, Glove and Preview use the same selection. Matching fusions take priority; full fields, terrain and non-small plants retain their barriers.
- Native almanac partners use their plant miniature instead of the grass background. Asynchronously loaded thumbnails automatically update maps and detailed views; the native models retain resource management.

The final release point increases to **3.0.1** for these fixes. No new plants, zombies or recipes; `fusionen.txt` remains unchanged. [Causes and manual checklist](references/menu-slots-almanac-3-0-1/report.md). Game and test programs did not start. Static testing and compilation are not a substitute for runtime testing. Dismantling when the game is closed: Copy `backup/FusionMod-3.0.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/menu-slots-almanac-3.0.0.tar.gz`.

## Change 3.0.0: Debug levels and bug fixes

- Type `debug` in the main menu: select background, then select up to ten plants and in the second tab select any number of zombie cards. All native plants are available regardless of profile. Imitator copies the first other plant selected, alternatively Peashooter.
- In the debug level, normal plant maps remain in the native map bar. Selected zombie cards appear below. Click on a zombie card, then click on a grass field: The zombie appears there as an enemy. Right click or Esc cancels zombie placement. The eye zombie is also selectable.
- Native waves repeat without level ending. Sun can be spent indefinitely; the display remains at 9990 and also shows `∞`. Zombies on the left edge do not complete the debug level. Zombie selection, unlocking, and sun rules apply to this session only. Debug saves are not loaded, saved, or deleted as a replacement for existing saves.
- The Almanac reads fusion partners from the full native plant model rather than just the visible maps. This means that lanterns, gold flowers and other plants that are outside the current section are no longer missing.
- The Eye Zombie only opens its eyes when it encounters an enemy target within a space in the same row. Projectiles no longer trigger opening. The original animation follows the board controller independently of the native skin animation; Team changes reflect the imported graphics.
- The lantern gains the Fusion recharge time of 15 seconds. The already suitable native fog area of ​​5×7 without corners, 25 sun and the existing additive light level 1 in the area 3×3 remain responsible.
- Courage fungus spores maintain a flight altitude of 40 pixels before each native collision check. Brave Mushroom Smoke and Ice Smoke use their original sprites and archived emission, speed, and fade values ​​instead of the native default smoke. Damage rules remain in the existing fume damage route.

The change to existing gameplay rules increases the first version point from 2.35.0 to 3.0.0. No new fusions; `fusionen.txt` remains unchanged. [Receipts and manual checklist](references/debug-fixes-3-0-0/report.md). Only data was checked and programs were compiled. Game and test programs did not start. Dismantling when the game is closed: Copy `backup/FusionMod-2.35.0.dll` to `Mods/FusionMod.dll`. Previous project files are in `backup/debug-fixes-2.35.0.tar.gz`.

## Change 2.35.0: Four seaweed and mud gourd fusions

- **Tangle Kelp + Squash = Strangle Kelp** ([Strangle Tang](references/strangle-kelp/port-notes.md)): three drowns, 600 additional area damage each, one second recovery between grabs.
- **Tangle Kelp + Threepeater = Kelp-spreader** ([Triple Tang](references/kelp-spreader/port-notes.md)): three Tang projectiles every 1.5 seconds, 30 damage and Tang points each; retains one-time drowning.
- **Squash + Threepeater = Squash-spreader** ([Triple Squash Pumpkin](references/squash-spreader/port-notes.md)): three bouncing mini squid pumpkins every three seconds; 40 damage on contact and landing on the ground.
- **Spikeweed + Squash = Squashweed** ([Mud Pumpkin Grass](references/squashweed/port-notes.md)): 20 shield-piercing contact damage every 1.5 seconds, knockback and mini mud pumpkin.

All four source recipes use available native ingredients. Both card orders and glove directions are active. Four original plant models with all clips, the original bullet, original portraits and complete German/English almanac are included. Fertilizer heals without evolving and gains spent grips and running attacks. The outdated English Kelp damage value is corrected using PlantData and original code.

[Source, condition and test report](references/kelp-squash-2-35-0/report.md), [Adapter and user commands](references/kelp-squash-2-35-0/adapters.json). Play and testing programs remain closed; The user is responsible for testing. Workflow and source archive remain unchanged. Dismantling when the game is closed: Copy `backup/FusionMod-2.34.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/support-pots-2.34.0.tar.gz`.


## Change 2.34.0: Four ground and water fusions

- **Spikeweed + Kernel-pult = Butterweed** ([Buttergrass](references/butterweed/port-notes.md)): half movement speed on contact, one second of butter every three seconds, no damage and no tire destruction.
- **Spikeweed + Melon-pult = Springweed** ([Spikeweed](references/springweed/port-notes.md)): 80 shield piercing damage every 1.5 seconds; Throw up, weaker effect on giants, instant imp death, and inherited vehicle contact.
- **Sea-shroom + Squash = Splash** ([Water Pumpkin](references/splash/port-notes.md)): aquatic mud pumpkin with 1800 landing damage, even against submerged targets.
- **Sea-shroom + Wall-nut = Sea-nut** ([Sea-shroom](references/sea-nut/port-notes.md)): 2000/4000/6000 HP at 0/10/20 seconds; Rolls invulnerable after 30 seconds and deals 600 damage every 0.02 seconds on contact.

All four full source recipes use native ingredients already available. Both card orders and glove directions are active. No outcome identity exists natively. Four original models with eleven clips, original portraits and complete German/English almanac are included. All skills are implemented; the four sources do not produce their own damage projectiles. Fertilizer heals, speeds up butter or growth timers to a maximum of 0.5 seconds, and maintains ongoing attacks, jumps, and rolling states. Sea-mine remains open due to collision with the existing disordered land-mine recipe.

[Common source/condition report](references/ground-fusions-2-34-0/report.md), [Adapters and user commands](references/ground-fusions-2-34-0/adapters.json). Play and testing programs remain closed; the user takes over testing. Workflow and source archive remain unchanged. Dismantling when the game is closed: Copy `backup/FusionMod-2.33.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/pot-fusions-2.33.0.tar.gz`.


## Change 2.33.0: Four more cactus fusions

- **Cactus + Starfruit = Starthorn** ([Star Cactus](references/starthorn/port-notes.md)): five homing original stars against aerial targets on the entire field.
- **Cactus + Doom-shroom = Doom Cactus** ([Doom Cactus](references/doom-cactus/port-notes.md)): three-hit radiating thorns and special fan made of 90 original thorns; Damage and crush protection during special animation.
- **Cactus + Plantern = Lumos Cactus** ([Light Cactus](references/lumos-cactus/port-notes.md)): Air priority, double air attacks, light thorns, and unlimited piercing lasers with light bonus.
- **Cactus + Spikeweed = Thornweed** ([Cactus Spikeweed](references/thornweed/port-notes.md)): shield-piercing contact against ground and air; each attack shoots up three original spikes.

All four full source recipes use native ingredients already available; both card orders and glove directions remain supported. No outcome identity exists natively. All abilities and projectile paths, original models with all clips, original portraits and complete German/English almanac are included. Fertilizer heals without development and maintains lot supply and attack. Source contradictions are expressly stated in the Almanac: Doom Cactus starts with 1/21 special chance, light level 3 doubles laser damage.

[Joint report](references/cactus-fusions-2-33-0/report.md), [Adapters and user commands](references/cactus-fusions-2-33-0/adapters.json), [Installation receipt](references/release-2.33.0.json). Play and testing programs remain closed; the user takes over testing. Workflow and source archive remain unchanged. Dismantling when the game is closed: Copy `backup/FusionMod-2.32.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/cactus-fusions-2.32.0.tar.gz`.


## Change 2.32.0: Four laser shooters

- **Peashooter + Plantern = Beamshooter** ([Lichterbse](references/beamshooter/port-notes.md)): a permanently locked ground target.
- **Repeater + Plantern = Rebeamer** ([Double Light Pea](references/rebeamer/port-notes.md)): two different targets.
- **Split Pea + Plantern = Split Beam** ([Relay Light Pea](references/split-beam/port-notes.md)): receives energy from left; the right head attacks a target.
- **Gatling Pea + Plantern = Gatling Beam** ([MG-Light Pea](references/gatling-beam/port-notes.md)): four different targets.

All four basic recipes use native ingredients that are already available. In total, all ten source recipes including the alternative pea upgrades are implemented, each in both card orders and glove directions. No outcome identity exists natively. Each beam deals 10 base shield piercing damage every 0.2 seconds. Light creates energy; Consumption increases damage. Front relay light peas can instead receive energy every 0.1 seconds, starting from two columns apart. One click captures new targets. Energy 1 is retained according to the original code; A transmission consumes energy once, then the number of beams multiplies the amount transmitted.

Four original models with twelve clips, original portraits and complete German/English almanac are included. Lasers are procedural and do not produce moving damage projectiles. Native memory fields receive energy, all four target identifiers, relays, independent attack timers and animation. Fertilizer heals without special development; Energy and goals remain intact. Non-existent Odyssey/Rogue upgrades are ring-fenced in the Almanac.

[Common source/condition report](references/beam-fusions-2-32-0/report.md), [Adapter and user commands](references/beam-fusions-2-32-0/adapters.json), [Installation receipt](references/release-2.32.0.json). Play and testing programs remain closed; the user takes over testing. Workflow and `fusionen-quellen.json` remain unchanged. Dismantling when the game is closed: Copy `backup/FusionMod-2.31.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/beam-fusions-2.31.0.tar.gz`.



## Change 2.31.0: Four more smoke family fusions

All four recipes use the already native **Fume-shroom**:

- **Plantern = Ray-shroom** ([Laser mushroom](references/ray-shroom/port-notes.md)): unlimited penetrating laser with distance and light bonus, every 1.5 seconds.
- **Kernel-pult = Blubber-shroom** ([Butterpushing mushroom](references/blubber-shroom/port-notes.md)): Click or hit throws original butter at nearby targets; 40 damage, four second stun, 30 current and maximum HP per attack.
- **Sunflower = Sunrise-shroom** ([Sun Storage Mushroom](references/sunrise-shroom/port-notes.md)): draws in new suns in the 5×5 area, stores their rounded 1.3x value and pays out in units of 25 upon production.
- **Umbrella Leaf = Bush-shroom** ([Umbrella Smoke Mushroom](references/bush-shroom/port-notes.md)): Damage reduction, stored energy for smoke waves, recoil, and native bungee/basketball protection.

All ingredients are available; every result has exactly this complete source recipe. No result identity exists natively in Replanted. Original models include eight clips; the complete original butter model is reused. The laser, like its source, is procedural. Complete German/English almanac with original portraits is included. Fertilizer heals all four; He receives Buttersqueezing Mushroom's reduced HP maximum as well as energy and solar storage. With the solar storage mushroom, it limits the positive production timer to 0.5 seconds. The source code only pays out additional sun when memory is over 25; this special feature has been implemented and described.

[Source and condition report](references/fume-support-2-31-0/report.md), [Adapter and user commands](references/fume-support-2-31-0/adapters.json), [Installation receipt](references/release-2.31.0.json). Play and testing programs remain closed; the user takes over testing. Workflow and `fusionen-quellen.json` remain unchanged. Dismantling when the game is closed: Copy `backup/FusionMod-2.30.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/fume-support-2.30.0.tar.gz`.


## Change 2.30.0: Four more fusions

- **Garlic + Umbrella Leaf = Umbrella Clove** ([Garlic Umbrella](references/umbrella-clove/port-notes.md)): 40 shield piercing damage, knockback, and native line switching.
- **Cactus + Umbrella Leaf = Umbrella Thorn** ([Thorn Umbrella](references/umbrella-thorn/port-notes.md)): only knocks back aerial targets and bursts balloons without direct damage.
- **Cabbage-pult + Doom-shroom = Damage-pult** ([Ominous Cabbage Pult](references/damage-pult/port-notes.md)): 1800 damage every 15 seconds; Radiated hits deal 600 area damage to other nearby zombies.
- **Fume-shroom + Starfruit = Starburst-shroom** ([Starcap Mushroom](references/starburst-shroom/port-notes.md)): damage-free cap with three different contacts; each contact creates five original stars, each dealing 20 damage.

All ingredients were already available natively. No result identity exists natively in Replanted. All four full source recipes are active in both card orders and glove directions. Garlic and Thorn Umbrella has 1000 HP, a one second contact cooldown, and its original health cost of 100/150/250 HP for 1/2-4/5+ targets. Native protection against bungees and catapult projectiles is retained. Fertilizer heals all four without special evolution or resetting attack counters. Original airships are not present in Replanted; this limit is named in the almanac.

Four original plant models and two additional full projectile models with a total of eleven clips are included, as well as the existing original model of normal stars. The almanac has original portraits as well as complete German/English descriptions and accompanying texts. **309 recipes** are active. **360 recipes with 294 results** remain open. The almanac contains **258 results**. The four complete lines are omitted from `fusionen.txt`; Source archive and workflow remain unchanged.

[Common source/condition report](references/mixed-fusions-2-30-0/report.md), [Adapter and user commands](references/mixed-fusions-2-30-0/adapters.json), [Installation receipt](references/release-2.30.0.json). Game and test programs do not start; the user takes over testing. Dismantling when the game is closed: Copy `backup/FusionMod-2.29.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/umbrella-fusions-2.29.0.tar.gz`.


## Change 2.29.0: Four mischief shooters

- **Peashooter + Doom-shroom = Doom Pea** ([Corruption](references/doom-pea/port-notes.md)): Shield-piercing peas. Each shot increases damage by one to 100; the first one already deals 41 damage.
- **Gatling Pea + Doom-shroom = Gatling Doom** ([MG-Mischief](references/gatling-doom/port-notes.md)): four projectiles of 300 damage each. Every sixteenth deals 1800 direct and additional area damage; then 4.5 second cooldown. Alternative recipe: **Doom Pea + Split Pea**.
- **Threepeater + Doom-shroom = Doom-spreader** ([Doom triple pea](references/doom-spreader/port-notes.md)): three peas of 40 damage each. Every seventh volley uses large projectiles with 120 direct and area damage. At the edge, a delayed additional floor replaces the missing row.
- **Starfruit + Doom-shroom = Death Star** ([Death Star](references/death-star/port-notes.md)): after three seconds, a falling baleful meteor with 1800 area damage and 180 original spore stars of 20 damage each. No crater.

All four results have available native base recipes. A total of five new recipes are active, each in both card orders and glove directions. No variant exists natively in Replanted. Gatling Frenzy-shroom as an additional ingredient remains open because this plant has not yet been implemented. Doom-spreader gets its own memory pair; Charred Threepeater retains its existing identifier.

Fertilizer heals all four and limits Death Star's positive activation timer to 0.5 seconds. Damage bonus, bullet/burst counter and pause are retained. According to the source code, large doom projectiles deal area damage before the direct hit. Three source units of radius correspond to approximately 2.22 lawn fields. Non-existent Frenzy synergies and Odyssey upgrades are explicitly delineated in the Almanac.

Four original plant models and four additional full bullet models with a total of 19 clips are included, as well as the existing original spore star model. English and German almanacs have complete texts and original portraits. **305 recipes** are active. **364 recipes with 298 results** remain open. The almanac contains **254 results**. The five complete recipe lines are omitted from `fusionen.txt`; Source archive and workflow remain unchanged.

[Common source/condition report](references/doom-shooters-2-29-0/report.md), [Adapter and user commands](references/doom-shooters-2-29-0/adapters.json), [Installation receipt](references/release-2.29.0.json). Game and test programs do not start; the user takes over testing. Dismantling when the game is closed: Copy `backup/FusionMod-2.28.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/support-pots-2.28.0.tar.gz`.

## Change 2.28.0: Four more metal fusions

- **Kernel-pult + Magnet-shroom = Kernel Morph** ([Magnetkornapult](references/kernel-morph/port-notes.md)): 60 damage every three seconds; 25 percent magnetic butter with 80 damage and a four second stun.
- **Morph-shroom + Bucket = Rust-shroom** ([Rust Mushroom](references/rust-shroom/port-notes.md)): every 1.5 seconds, 50 shield piercing damage, 150 against remaining Type 1 armor.
- **Melon Morph + Bucket = Metal-pult** ([Iron Melonapult](references/metal-pult/port-notes.md)): 200 direct damage plus melon splash damage every three seconds; Double damage against each recipient's existing Type 1 armor.
- **Starmorph + Bucket = Iron Star** ([Iron Star](references/iron-star/port-notes.md)): five stars with 80 damage each every 1.5 seconds; Adds existing Type 2 armor before native damage processing.

All the ingredients were already available. Each result has exactly one complete source recipe; both orders and glove directions use the existing resolver. Pick-up buckets can upgrade the three basic metal plants and then heal 500 HP each up to a maximum of 2000. Kernel Morph has 300 LP. Fertilizer heals all four without special development. Chrono/helmet upgrades that have not been implemented remain open. No variant exists natively in Replanted.

Four original plant models and four complete original projectile models with a total of 13 clips as well as original portraits and complete German/English almanac are included. The smoke family uses their existing native FumeCloud effect. **300 recipes** are active. **369 recipes with 301 results** remain open. The almanac contains **250 results**. The four complete lines are omitted from `fusionen.txt`; Source archive and workflow remain unchanged.

[Common source/condition report](references/shooter-fusions-2-28-0/report.md), [Adapter and user commands](references/shooter-fusions-2-28-0/adapters.json), [Installation receipt](references/release-2.28.0.json). Game and test programs do not start; the user takes over testing. Dismantling when the game is closed: Copy `backup/FusionMod-2.27.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/shooter-fusions-2.27.0.tar.gz`.


## Change 2.27.0: Four supporting pumpkin fusions

All four recipes use **Pumpkin** and the native ingredient mentioned:

- **Peashooter = Pumpkin Pod** ([Peashooter](references/pumpkin-pod/port-notes.md)): copies the trapped peashooter's projectile type, damage, and volleys. Double basic interval, at least three seconds.
- **Melon-pult = Melon Shell** ([Melon Shell](references/melon-shell/port-notes.md)): per actual throw, an additional original melon with 80 direct damage and melon splash damage.
- **Ice-shroom = Cryo Pumpkin** ([Frost Pumpkin](references/cryo-pumpkin/port-notes.md)): stores up to three ice effects from the field directly in front of it; Click and auto release with four seconds of freezing. Bites cause four seconds of cold and five freezing points.
- **Jalapeno = Pyro Pumpkin** ([Emerge Pumpkin](references/pyro-pumpkin/port-notes.md)): Heat from lines of fire and superheated deaths, bite reflection and overheating. Click or die from 1800 heat consumes all the energy for a line of fire.

Each result has a fully available source recipe; both card orders and glove directions use the existing resolver. No variant exists natively. All shells have 4000 health points. Four original models with a total of six clips and existing full projectile models are included. Fertilizer heals; for peas and frost squash, it limits positive ability timers to 0.5 seconds. Charges and heat are retained. Overheat is stored in native GridItems for true save/load. English and German almanac texts name documented deviations: Contrary to the description, Cryo does not reach its own field, Pumpkin Pod does not have a six-second upper limit in the source code, damaged Pyro shells reflect less damage.

There are **296 recipes** active. Remain open **373 recipes with 305 results**; the four complete lines are omitted from `fusionen.txt`. The almanac contains **246 results**. Workflow and source archive remain unchanged. Game and test programs do not start; the user takes over testing.

[Common source/condition report](references/pumpkin-support-2-27-0/report.md), [Adapter and user commands](references/pumpkin-support-2-27-0/adapters.json), [Installation receipt](references/release-2.27.0.json). Dismantling when the game is closed: Copy `backup/FusionMod-2.26.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/pumpkin-support-2.26.0.tar.gz`.


## Change 2.26.0: Four Calamity Fusions

All four recipes combine the already native **Doom-shroom** with an equally native ingredient:

- **Garlic = Doom Garlic** ([Doom Garlic](references/doom-garlic/port-notes.md)): Bites radiate and poison; One-time corpse explosion with 300 damage. Poison remains active when irradiated.
- **Wall-nut = Doom-nut** ([Doom Nut](references/doom-nut/port-notes.md)): 4000 HP, protection against non-physical explosions, healing on nearby radiation deaths and doom fungus effect on your own death.
- **Sunflower = Doomflower** ([Doom Sunflower](references/doomflower/port-notes.md)): every 50 seconds, 25 Sun and a Doom Spore in 3×3. Spores grow for 30 seconds and explode when close to an enemy.
- **Potato Mine = Crater-dead** ([Crater Potato](references/crater-tot/port-notes.md)): Potato and ominous explosion, followed by five original fragments. After 1.5 seconds of flight, native mines spawn with 0.5 seconds remaining before appearing.

Each result has exactly one source recipe, valid for both card orders and glove directions. No variant exists natively. Five original plant models including the generated spore and the full fragment model with a total of ten clips are included. The almanac contains complete German/English texts and original portraits. Fertilizer heals; Sun/spore timers will be limited to 0.5 seconds or less, Crater-dead will arm immediately. Replanted has no Doom-shroom Zombies; their separate nut suppression has no target type.

There are **292 recipes** active. Remain open **377 recipes with 309 results**; the four complete lines are omitted from `fusionen.txt`. The almanac contains **242 results**. Workflow and source archive remain unchanged. Game and test programs do not start; the user takes over testing.

[Common source/condition report](references/doom-fusions-2-26-0/report.md), [Adapter and user commands](references/doom-fusions-2-26-0/adapters.json), [Installation receipt](references/release-2.26.0.json). Dismantling when the game is closed: Copy `backup/FusionMod-2.25.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/support-pots-2.25.0.tar.gz`.


## Change 2.25.0: Three cacti and a glow nut

- **Fume-shroom + Cactus = Saguaro-shroom** ([Columnar Cactus Mushroom](references/saguaro-shroom/port-notes.md)): shield-piercing smoke, low ground/air mode and high air mode with quadruple damage. Click changes mode.
- **Cactus + Ice-shroom = Ice Cactus** ([Ice Cactus](references/ice-cactus/port-notes.md)): Air priority, ten frost points, quadruple damage against frozen targets and additionally against air; up to three different aerial hits.
- **Cactus + Magnet-shroom = Magnet Cactus** ([Magnetic Cactus](references/magnet-cactus/port-notes.md)): Air priority, 40 base damage, double damage against existing metal armor/shields.
- **Plantern + Wall-nut = Illumi-nut** ([Luminous Nut](references/illumi-nut/port-notes.md)): 4000 LP, light level one in 3×3, healing from additional light every 15 seconds.

Each result has exactly one fully available source recipe. Both card orders and glove directions use the existing resolver. No variant exists natively. Four original plant models with all 19 clips are included, as well as both complete original projectile models. The almanac contains original portraits and complete German/English texts. Fertilizer heals all four; only the positive Illumi healing timer is limited to a maximum of 0.5 seconds. Non-existent iron/portal balloons and airships do not provide a target type for the additional magnetic source branches.

There are **288 recipes** active. Remain open **381 recipes with 313 results**; four complete lines are omitted from `fusionen.txt`. The almanac contains **238 results**. Workflow and source archive remain unchanged. Play and testing programs remain closed; the user takes over testing.

[Common source/condition report](references/cactus-fusions-2-25-0/report.md), [Adapter and user commands](references/cactus-fusions-2-25-0/adapters.json), [Installation receipt](references/release-2.25.0.json). Dismantling when the game is closed: Copy `backup/FusionMod-2.24.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/cactus-fusions-2.24.0.tar.gz`.


## Change 2.24.0: Four metal fusions

All recipes use **Magnet-shroom** as well as the native ingredient mentioned:

- **Doom-shroom = Dormant-shroom** ([Slumber Mushroom](references/dormant-shroom/port-notes.md)): remains passive and does not explode.
- **Fume-shroom = Morph-shroom** ([Metal Smoke Mushroom](references/morph-shroom/port-notes.md)): 40 shield piercing damage every 1.5 seconds; double against metal armor/shields and Zamboni.
- **Melon-pult = Melon Morph** ([Metalmelon](references/melon-morph/port-notes.md)): 120 direct damage every three seconds with melon splash damage; Double damage if each recipient has Type 1 armor.
- **Starfruit = Starmorph** ([Metalstar](references/starmorph/port-notes.md)): five stars with 40 damage each every 1.5 seconds; double damage against metal armor/shields.

Each result has exactly one source recipe, valid for both card orders and glove directions. Four original plant models and two complete projectile models including clips, original portraits and complete German/English almanac texts are included. Fertilizer heals without special development. Later metal upgrades remain open; they are different results. No new variant exists natively in Replanted.

There are **284 recipes** active. Remain open **385 recipes with 317 results**; four complete lines are omitted from `fusionen.txt`. The almanac contains **234 results**. Workflow and source archive remain unchanged. Game and test programs do not start; the user takes over testing.

[Common source/condition report](references/metal-morphs-2-24-0/report.md), [Adapter and user commands](references/metal-morphs-2-24-0/adapters.json), [Installation receipt](references/release-2.24.0.json). Dismantling when the game is closed: Copy `backup/FusionMod-2.23.0.dll` to `Mods/FusionMod.dll`. Previous project status: `backup/support-pots-2.23.0.tar.gz`.


## Change 2.23.0: Four new throw fusions

- **Cabbage-pult + Squash = Lettuce-pult** ([Salatquetschapult](references/lettuce-pult/port-notes.md)): Squeeze bullets with two subsequent generations, 60/30/15 damage.
- **Kernel-pult + Squash = Butternut-pult** ([Butterquetschapult](references/butternut-pult/port-notes.md)): 60 Squash Damage; 25 percent butter selection, four seconds of butter for up to four targets according to actual source code.
- **Melon-pult + Squash = Gourd-pult** ([Melon Squashapult](references/gourd-pult/port-notes.md)): 120 Squash Damage; after a second of additional melon splash damage.
- **Fume-shroom + Melon-pult = Spring Melon** ([Spring Melon](references/spring-melon/port-notes.md)): stored charge up to 30 seconds, up to 1920 direct damage. Sourced: three second base interval, seven source units of range, and more than 1.5 seconds of charge required. These code values ​​replace conflicting English almanac information.

Each result has exactly one source recipe. All ingredients were already available natively; no new variant exists natively. Both card orders and glove directions use the existing resolution. Four original plant models and five new projectile/ground models with all clips are included; Spring Melon uses the existing full melon projectile. All four receive English and German almanac texts including original portraits. Fertilizer heals; with Spring Melon it only limits the positive attribute timer to 0.5 seconds and maintains the charge.

There are **280 recipes** active. Remain open **389 recipes with 321 results**; four fully implemented lines are omitted from `fusionen.txt`. The almanac contains **230 results**. Workflow and source archive remain unchanged. Game and test programs do not start; the user takes over testing.

[Common source/condition report](references/squash-lobbers-2-23-0/report.md), [Adapter and user commands](references/squash-lobbers-2-23-0/adapters.json), [Installation receipt](references/release-2.23.0.json). Dismantling when the game is closed: Copy `backup/FusionMod-2.22.0.dll` to `Mods/FusionMod.dll`. Project status before: `backup/squash-lobbers-2.22.0.tar.gz`.

## Change 2.22.0: Four pumpkin fusions and bug fixes

- **Pumpkin + Blover = Windy Pumpkin** ([Wind Pumpkin](references/windy-pumpkin/port-notes.md)): 4000 LP; creates a native blover every 30 seconds in the first permitted free infield from your own field to the right.
- **Pumpkin + Spikeweed = Spiky Pumpkin** ([Spiky Pumpkin](references/spiky-pumpkin/port-notes.md)): 4000 LP; low, does not block zombies. Takes crushing for itself and prickly plants/prickly nuts in the same field. Losing a tire costs 500 LP, a maximum of once per simulation stick.
- **Pumpkin + Plantern = Jack O' Plantern** ([Lantern Pumpkin](references/jack-o-plantern/port-notes.md)): 4000 LP; Light level 1 and fog clearing in 3×3. Heal for 150 × max(0, light level in field − 1) HP every 15 seconds. Implemented light sources add up.
- **Pumpkin + Sea-shroom = Sea-shell** ([Sea Pumpkin](references/sea-shell/port-notes.md)): 4000 LP; Repair sea mushroom cards, worn sea mushrooms and swarm sea mushrooms immediately to the left 1333 LP. The indoor plant and rootstock remain intact.

All four results have exactly one source recipe, supported in both card orders and glove directions. No variant exists natively. Four original models with all four original clips, damage views and complete English/German almanac texts including original portraits are included. These four plants do not have projectiles of their own; Wind squash produces a complete native blover. Fertilizer heals all four; for the Wind and Lantern Pumpkins, it limits the running ability timer to a maximum of 0.5 seconds.

When you right-click on a native map, the Almanac first resolves the full binder key. Cattail and Melon Pot patches both honor native `AddProjectile` overloads without ambiguous Harmony target. Chompzilla's stored fragment identifier explicitly determines the exact original model; another valid floor view is not sufficient. Both native pea types receive the fragment identifier. All three original fragments are re-exported.

There are **276 recipes** active; remain open **393 recipes with 325 results**. The almanac contains **226 results**. Four completely completed lines are omitted from `fusionen.txt`. Source archive and workflow remain unchanged. Game and test programs do not start; the user takes over testing.

[Common source/condition report](references/pumpkin-support-2-22-0/report.md), [Adapter and user commands](references/pumpkin-support-2-22-0/adapters.json), [Installation receipt](references/release-2.22.0.json). Dismantling when the game is closed: Copy `backup/FusionMod-2.21.0.dll` to `Mods/FusionMod.dll` and load a save game without new IDs. Project status before: `backup/pumpkin-support-2.21.0.tar.gz`.



## Change 2.21.0: Four pumpkin fusions and graphics/hit fixes

- **Pumpkin + Cherry Bomb = Cherry Pumpkin** ([Cherry Pumpkin](references/cherry-pumpkin/port-notes.md)): 4000 LP; absorbs explosive cherries before their area damage. Absorption and existing cherry area effects heal 200 HP.
- **Pumpkin + Garlic = Stinky Pumpkin** ([Stink Pumpkin](references/stinky-pumpkin/port-notes.md)): 4000 LP; Actual bites give ten seconds of poison, one point of poison, and the native delayed row change.
- **Pumpkin + Scaredy-shroom = Scareless Pumpkin** ([Mutpumpkin](references/scareless-pumpkin/port-notes.md)): 2000 LP; native and implemented fear mushrooms in the same field stay brave and keep shooting.
- **Pumpkin + Wall-nut = Nut Shell** ([Nut Shell](references/nut-shell/port-notes.md)): 8000 LP; Approximately every 50 seconds a total of 1000 HP healing for other pumpkins in the surrounding 3x3 area. Without a healing target, a usable walnut card is created, a maximum of three times. Healing remains active afterwards. The source grows the nut from 0.01 to 1 at 0.02/s: approximately 49.5 seconds.

All four source recipes apply to both card orders and glove directions. The indoor plant and rootstock remain intact. Four original models with all four clips, damage views, visible nut growth and complete German/English almanac texts including original portraits are included. None of these variants exist natively in Replanted. Fertilizer heals without special development; Nut growth and used cards are retained.

Chompzilla's three original fragments are re-exported from the source prefabs. Graphics views now check the exact fragment identifier and are restored to the existing motion path. All Fume-Fusions and Shivery-shroom use the source scale instead of inconsistent shadow widths. Homemade Scaredy bullets receive the required ground target mask; Older saved shots will be repaired before the next collision. Frost-shroom's smoke is given the blue of the original FumeIce sprite.

There are **272 recipes** active; remain open **397 recipes with 329 results**. The almanac contains **222 results**. Four completed lines are omitted from `fusionen.txt`. Source archive and workflow remain unchanged. Game and test programs do not start; the user takes over testing.

[Source, condition and test report](references/pumpkin-elemental-2-21-0/report.md), [Adapter and user commands](references/pumpkin-elemental-2-21-0/adapters.json), [Installation receipt](references/release-2.21.0.json). Dismantling when the game is closed: Copy `backup/FusionMod-2.20.0.dll` to `Mods/FusionMod.dll`. Project status before: `backup/pumpkin-elemental-2.20.0.tar.gz`.



## Change 2.20.0: Four pumpkin fusions, Chompzilla and Boost-shroom

- **Pumpkin + Starfruit = Starry Pumpkin** ([Star Pumpkin](references/starry-pumpkin/port-notes.md)): enables automatic Solarfruit shots. The attraction synergies require the Starblover/Pulsar not yet implemented.
- **Pumpkin + Cactus = Thorny Pumpkin** ([Thorn Pumpkin](references/thorny-pumpkin/port-notes.md)): 4000 LP; reflects 50 damage for each actual bite taken.
- **Pumpkin + Puff-shroom = Puff-kin** ([Sprout Pumpkin](references/puff-kin/port-notes.md)): starts with 1000/4000 HP, heals 50 HP every three seconds. Above a third, numerical damage halved; above two-thirds protection of the indoor plant.
- **Pumpkin + Fume-shroom = Fuming Pumpkin** ([Smoke Pumpkin](references/fuming-pumpkin/port-notes.md)): three vertical, shield-piercing 20-damage hits every 1.5 seconds, own column up to three rows above/below.

All four source recipes apply to both card orders and glove directions. Pumpkin dissolution receives indoor plant and rootstock. Four original models, all five clips, damage views and German/English almanac texts with original portraits are included. Fertilizer heals; with sprouting pumpkin, it limits the remaining healing time to a maximum of 0.5 seconds.

Chompzilla now uses all three original zombie fragments instead of ChompShooter graphics. Spitting follows the original event after the bite. Weak unarmored targets do not consume Special Bite. Special digestion lasts 40 seconds; Fertilizer limits their remaining time to a maximum of 0.5 seconds. Bullet ID and hit history are retained when saving. Boost shrooms smoke gets original green and extends across the entire front row; Working damage and boost remain with the existing owner.

There are **268 recipes** active, **401 recipes with 333 results** that remain open. The almanac contains **218 results**. Four completed lines are omitted from `fusionen.txt`. Source archive and workflow remain unchanged. Game and test programs do not start; The user is responsible for play testing.

[Common source/condition report](references/pumpkin-fusions-2-20-0/report.md), [Adapter and user commands](references/pumpkin-fusions-2-20-0/adapters.json), [Installation receipt](references/release-2.20.0.json). Dismantling when the game is closed: Copy `backup/FusionMod-2.19.0.dll` to `Mods/FusionMod.dll`. The previous project status is below `backup/pumpkin-fusions-2.19.0.tar.gz`.


## Change 2.19.0: Four water support fusions

- **Sea-shroom + Plantern = Beacon-shroom** ([Luminous Sea Mushroom](references/beacon-shroom/port-notes.md)): clears 3×3 fields; after 30 seconds 5×5.
- **Sea-shroom + Hypno-shroom = Jelly-shroom** ([Jellyfish Hypno Mushroom](references/jelly-shroom/port-notes.md)): Sea Mushroom Food summons a hypnotized floating tire zombie after the eating animation. Bites hypnotize the attacker and consume the awake plant.
- **Sea-shroom + Ice-shroom = Swell-shroom** ([Frost Puffer Mushroom](references/swell-shroom/port-notes.md)): Sea Mushroom Food deals 20 shield-piercing damage after 1.4 seconds and four seconds of close-range frost.
- **Sea-shroom + Scaredy-shroom = Hermit-shroom** ([Hermit Sea Mushroom](references/hermit-shroom/port-notes.md)): Sea Mushroom Food gives ten seconds of courage, including rearing time. Shoots 20-damage spores every 0.5 seconds; then crouches again.

All four source recipes apply to both card orders and glove directions. Feeding works with map, carried sea mushroom and swarm sea mushroom immediately left. Four original models, all 13 original clips, existing original spurs and complete German/English almanac texts including portraits are included. Native water, sleep, coffee and storage rules apply. Fertilizer heals and limits remaining Beacon growth or ongoing Hermit Courage to a maximum of 0.5 seconds. He does not activate Hidden Hermits and gains Jelly/Swell action phases.

The almanac contains **214 results**. There are **264 recipes** active. Four completed entries are omitted from `fusionen.txt`; **405 recipes with 337 results** remain open. Existing plants are not duplicated. Workflow and source archive remain unchanged.

[Common source/condition report](references/sea-support-2-19-0/report.md), [Adapter and user commands](references/sea-support-2-19-0/adapters.json), [Installation receipt](references/release-2.19.0.json). Game and test programs remain closed. The user checks abilities, food costs, courage/growth times, fertilizer, water movement of summoned zombies, original animations, gauntlet, pause/death, true save/load, and both almanac languages.

Dismantling when the game is closed: Copy `backup/FusionMod-2.18.0.dll` to `Mods/FusionMod.dll`. Then load a save game without new IDs. The original project status is below `backup/sea-support-2.18.0.tar.gz`.


## Change 2.18.0: Four water fusions

Four new results use only previously available native ingredients. Each has exactly one source recipe, registered for both card orders and glove directions. No variant already exists natively.

- **Sea-shroom + Puff-shroom = Minnow-shroom** ([Swarm Sea Mushroom](references/minnow-shroom/port-notes.md)): two 20-damage spores every 1.5 seconds; after 15 seconds three. After that, every 15 seconds, apply a Sea Mushroom to the immediately right plant or issue a usable Sea Mushroom card.
- **Sea-shroom + Cactus = Urchin-shroom** ([Sea Urchin Mushroom](references/urchin-shroom/port-notes.md)): row-bound tracking ground spores or spines that are effective only against aerial targets, each dealing 20 damage. Air targets have priority. After 120 seconds, four shots per attack.
- **Sea-shroom + Fume-shroom = Shark-shroom** ([Shark Mushroom](references/shark-shroom/port-notes.md)): 20 shield-piercing smoke damage every 1.5 seconds. Sea mushroom food increases the range for ten seconds across the entire row. Card, Glove and Minnow immediately to the left can feed. The source Kraken knockback synergy remains unattainable due to the lack of Kraken shroom.
- **Sea-shroom + Starfruit = Sea Star** ([Sea Star Mushroom](references/sea-star/port-notes.md)): five water stars, each dealing 20 damage every 1.5 seconds; after 60 seconds, two full volleys with a total of ten stars.

Four original models, all 17 original clips, three original bullets and complete German/English almanac texts with original portraits are included. All four are aquatic plants and use native sleep/coffee rules. The three low variants let opposing straight peas fly past; the high shark mushroom and basketball hits are excluded. Native memory fields maintain growth, proliferation, range enhancement, attack mode and flight states. Fertilizer heals and limits positive ability timers to a maximum of 0.5 seconds, including subsequent propagation and ongoing range buffs. Shorter timers and shooting phases are retained.

The almanac contains **210 results**. There are **260 recipes** active. Four completed lines are omitted from `fusionen.txt`; **409 recipes with 341 results** remain open. The middle version point increases once to **2.18.0**. Workflow and source archive remain unchanged; no time savings have been proven.

[Common source, condition and test report](references/sea-fusions-2-18-0/report.md), [Adapter and user commands](references/sea-fusions-2-18-0/adapters.json), [Installation receipt](references/release-2.18.0.json). Game and test programs will not start. The user checks water terrain, both recipe orders, gauntlet, food cost/card usage, growth, all multishots, balloon targets, propagation, fertilizer, pause/death, true save/load, and both almanac languages.

Dismantling when the game is closed: Copy `backup/FusionMod-2.17.0.dll` to `Mods/FusionMod.dll`. Then load a save game without new IDs. The previous project status is below `backup/sea-fusions-2.17.0.tar.gz`.

## Change 2.17.0: Four cat tail and hypnosis fusions

Four new results use only native ingredients. Each has exactly one source recipe, registered for both card orders and glove directions. No variant already exists natively.

- **Cattail + Ice-shroom = Frost Cattail** ([Frost Cattail](references/frost-cattail/port-notes.md)): two guided missiles with 40 damage each every 1.5 seconds; Double damage against chilled targets, quadruple against frozen targets.
- **Cattail + Jalapeno = Flame Cattail** ([Flame Cat Tail](references/flame-cattail/port-notes.md)): two guided projectiles with 40 damage each every 1.5 seconds; Triple base damage against already overheated targets before their existing damage bonus.
- **Melon-pult + Hypno-shroom = Hypno Melon** ([Hypnomelone](references/hypno-melon/port-notes.md)): 120 direct damage every three seconds and melon area damage. Any surviving target below half body and helmet health has a 25% Hypnosis chance after damage. Bites hypnotize and transform the plant back into an ordinary melon pulp.
- **Hypno-shroom + Jalapeno = Hypno Pepper** ([Hypnochili](references/hypno-pepper/port-notes.md)): explodes after about 0.84 seconds or when crushed. Hypnotizes targets of the same series under 900 body and helmet health; other targets and failed hypnosis receive 1800 ash damage.

Four original models, all original clips, three original bullets and complete English/German almanac texts with original portraits are included. Native storage fields maintain attack phases, ignition progress, bullet families and damage. Fertilizer heals without special development or resetting current phases. Cattail bullets take advantage of existing status conditions; they do not produce cold or overheating themselves. Adventure upgrades are excluded.

The almanac contains **206 results**. There are **256 recipes** active. Four completed lines are omitted from `fusionen.txt`; **413 recipes with 345 results** remain open. The middle version point increases once to **2.17.0**. Workflow and source archive remain unchanged; no time savings have been proven.

[Common source, condition and test report](references/mixed-fusions-2-17-0/report.md), [Adapter and user commands](references/mixed-fusions-2-17-0/adapters.json), [Installation receipt](references/release-2.17.0.json). Game and test programs will not start. The user checks skills, all-row homing, trajectories, hypnosis thresholds, bite retransformation, both recipe orders, gauntlet, fertilizer, true save/load, pause/death, and both almanac languages.

Dismantling when the game is closed: Copy `backup/FusionMod-2.16.0.dll` to `Mods/FusionMod.dll`. Then load a save game without new IDs. The previous project status is below `backup/mixed-fusions-2.16.0.tar.gz`.


## Change 2.16.0: Four star fusions

Four new results use only native ingredients. Each has exactly one source recipe; both card orders and glove directions use the same recipe table. No variant already exists in Replanted.

- **Starfruit + Garlic = Starlic** ([Garlic Star](references/starlic/port-notes.md)): five stars with 30 damage each every 1.5 seconds. Five additional damage per existing poison point, then one new poison point. Bites trigger a delayed line change.
- **Starfruit + Ice-shroom = Winter Starfruit** ([Winterstern](references/winter-starfruit/port-notes.md)): five spiral ice stars every 1.5 seconds. 20 damage each, ten frost points and quadruple damage against frozen targets.
- **Starfruit + Jalapeno = Spicy Starfruit** ([Chilistern](references/spicy-starfruit/port-notes.md)): explodes after one second or upon death. Creates a line of fire and 25 stars with 100 damage each. No instant burst of fire upon fusion.
- **Peashooter + Starfruit = Starshooter** ([Star Pea Cannon](references/starshooter/port-notes.md)): a 20-damage pea every 1.5 seconds. As long as she is flying, she releases five 20-damage stars every 0.2/0.7/1.2/1.7/2.2 seconds.

Four original models, all original clips, four original bullets and complete German/English almanac texts with original portraits are included. Native storage fields gain attack phases, fuse, bullet families, damage, spiral origin, and emission counters. Existing status holders save poison and frost. Fertilizer heals without special development. With the Chilistern it limits the remaining ignition time to a maximum of 0.5 seconds; Shorter ignition times and firing phases are retained.

The almanac contains **202 results**. There are **252 recipes** active. Four completed recipe lines are omitted from `fusionen.txt`; **417 recipes with 349 results** remain open. The middle version point increases once to **2.16.0**. Workflow and source archive remain unchanged; no time savings have been proven.

[Common source, condition and test report](references/star-fusions-2-16-0/report.md), [Adapter and user commands](references/star-fusions-2-16-0/adapters.json), [Installation receipt](references/release-2.16.0.json). Game and test programs will not start. The user checks abilities, trajectories, both recipe orders, gauntlet, fertilizer during ongoing phases, true save/load, pause/death, and both almanac languages.

Dismantling when the game is closed: Copy `backup/FusionMod-2.15.0.dll` to `Mods/FusionMod.dll`. Then load a save game without new IDs. The previous project status is below `backup/star-fusions-2.15.0.tar.gz`.


## Change 2.15.0: Four garlic and melon fusions

Four new results use only ingredients already available. All four source recipes are registered in both card orders and glove directions. None of these plants exist natively in Replanted.

- **Cabbage-pult + Garlic = Garbage-pult** ([Garlic Cabbage Pult](references/garbage-pult/port-notes.md)): 40 damage every three seconds and 20 additional damage per poison point present. Then award a poison point. An extra rebound; Targets with poison points present do not consume ricochet.
- **Kernel-pult + Garlic = Clove-pult** ([Garlic Kornapult](references/clove-pult/port-notes.md)): 40 damage, same poison bonus. 25% chance of garlic butter with 60 base damage, four seconds of butter and ten seconds of poisoning.
- **Melon-pult + Garlic = Garlic-pult** ([Garlic Melonapult](references/garlic-pult/port-notes.md)): 100 direct damage with 20 additional damage per point of poison. Area damage receives five additional damage per point of poison. Survivors then receive one poison point each.
- **Melon-pult + Jalapeno = Summer Melon** ([Summer Melon](references/summer-melon/port-notes.md)): 120 direct damage and melon area damage. Overheats surviving direct and area targets after damage.

Original graphics, all action clips, five original bullets, original portraits and complete German/English almanac texts are included. Fertilizer heals without special evolution, new butter choice, or resetting ongoing attacks. Native plant and projectile fields store phases, butter choice, damage and ricochet. The existing saved status records receive poison points/poison timers and the overheating caused by direct or area hits. Passed-on overheating from the existing death explosion retains the previous storage limit.

The almanac contains **198 results**. There are **248 recipes** active. Four completed recipe lines are omitted from `fusionen.txt`; **421 recipes with 353 results** remain open. The middle version point increases once to **2.15.0**. Workflow and source archive remain unchanged; no time savings have been proven.

[Common source, condition and test report](references/garlic-lobbers-2-15-0/report.md), [Adapter and user commands](references/garlic-lobbers-2-15-0/adapters.json), [Installation receipt](references/release-2.15.0.json). Game and test programs will not start. The user checks abilities, projectiles, both recipe orders, gauntlet, fertilizer, true save/load, pause/death, and both almanac languages.

Dismantling when the game is closed: Copy `backup/FusionMod-2.14.0.dll` to `Mods/FusionMod.dll`. Then load a save game without the new IDs. The previous project status is below `backup/garlic-lobbers-2.14.0.tar.gz`.


## Change 2.14.0: Four smoke mushroom fusions

Four new results use already available ingredients. All four source recipes are registered in both card orders and glove directions. None of these variants exist natively in Replanted.

- **Fume-shroom + Peashooter = Boost-shroom** ([Fume-shroom](references/boost-shroom/port-notes.md)): 20 piercing damage every 1.5 seconds across the entire front row. During the initial smoke window, the damage and speed of straight-line projectiles in front of him increase every 0.02 seconds. The bonuses are stackable and are saved.
- **Fume-shroom + Torchwood = Blowtorch-shroom** ([Burner Mushroom](references/blowtorch-shroom/port-notes.md)): Absorbs common peas up to 300 stored energy and discharges them as a piercing flame hit no more than once per second. Snow peas are thawed and fly through. The actual source range is seven source units.
- **Fume-shroom + Wall-nut = Wallop-shroom** ([Schschlagpilz](references/wallop-shroom/port-notes.md)): 4000 HP. Consumes an available Nut Mushroom within 3 × 3 spaces. His original projectile receives 20 plus his current health as damage, bypasses shields and penetrates weaker enemies with correspondingly decreasing residual damage.
- **Fume-shroom + Spikeweed = Caltrop-shroom** ([Nail Fungus](references/caltrop-shroom/port-notes.md)): Low, non-blocking plant. 40 piercing smoke damage every 1.5 seconds; Common vehicles receive an additional 30% of their maximum body and armor health. Vehicle contact punctures tires and consumes the plant.

Original models, all action clips, the new nut mushroom bullet, original portraits and complete German/English almanac texts are included. Native smoke particles replace the original smoke/flame particles. Fertilizer heals without special evolution or resetting ongoing attacks, stored ammo, and energy. The middle version point increases once to **2.14.0**.

The almanac contains **194 results**. Four completed recipe lines are omitted from `fusionen.txt`; **425 recipes with 357 results** remain open. Workflow and source archive remain unchanged. No time savings have been proven.

[Common source, condition and test report](references/fume-expansion-2-14-0/report.md), [affected adapters and user commands](references/fume-expansion-2-14-0/adapters.json), [installation receipt](references/release-2.14.0.json). Game and test programs will not start. The user checks both recipe orders, glove, terrain/pads/pumpkin, all abilities and projectiles, true save/load, rest/death, fertilizer, and both almanac languages.

Dismantling when the game is closed: Copy `backup/FusionMod-2.13.0.dll` to `Mods/FusionMod.dll`. Then load a save game without new IDs. The previous project status is below `backup/fume-expansion-2.13.0.tar.gz`.



## Change 2.13.0: Four Weepingleaf Fusions

Four new results use only native ingredients. Each result has exactly one source recipe; both card orders and glove directions are active. None of these variants already exist natively in Replanted.

- **Blover + Blover = Cyclover** ([Whirlblade](references/cyclover/port-notes.md)): Pushes back up to three closest enemies in the same row in the original clip. Thrust takes into account flight, throwing up, vehicles and both native giants. Only the first attack clears away fog and balloon zombies.
- **Blover + Garlic = Disguster** ([Garlic Weeder](references/disguster/port-notes.md)): Awards at least five poison points, then another point and five seconds of poisoning. Forces permissible row changes and also reacts to bites. Disappears two seconds after blowing.
- **Blover + Ice-shroom = Breeze Blover** ([Frostweher](references/breeze-blover/port-notes.md)): Ten cold pulses with one damage each and 20 frost points. Converts existing and newly created Snow Peas into full polar missiles: preserved trajectory and damage bonuses, 20 Frost points, quadruple damage against frozen or cold-immune targets, no conversion from common torchwood.
- **Blover + Hypno-shroom = Mind-blover** ([Hypnoweher](references/mind-blover/port-notes.md)): Pulls hypnotized zombies back for five seconds. At start, this costs a one-time 10% of your maximum body and helmet health. The left margin column remains excluded. Flight, vehicles and giants have the proven speed factors.

Original models, all original clips, polar bullets, original portraits and full English/German almanac texts are included. Native plant and missile fields store phases, lifespan, and missile damage. Invisible stored GridItems gain the poison and freeze states created by these fusions, including increasing freeze threshold. The existing status holders remain responsible for the effect. Fertilizer heals without evolution, additional attacks, or resetting current conditions.

The almanac contains **190 results**. Four completed recipe lines are omitted from `fusionen.txt`; **429 recipes with 361 results** remain open. Workflow and source archive remain unchanged. No time savings have been proven. The middle version point increases once to **2.13.0**.

[Common source, condition and test report](references/blover-fusions-2-13-0/report.md), [affected adapters and user commands](references/blover-fusions-2-13-0/adapters.json) and [installation receipt](references/release-2.13.0.json). Static tests and compilation are to be distinguished from runtime tests. Game and test programs will not start. The user checks both recipe orders, gauntlet, pads/pumpkin, cost/cooldown, all abilities and polar missiles, pause/death, true save/load, fertilizer, and both almanac languages. Wind particles use native FumeCloud effects; Original graphics and action clips are retained.

Dismantling when the game is closed: Copy `backup/FusionMod-2.12.0.dll` to `Mods/FusionMod.dll`. Then load a save game without the new IDs. Code, tools, tests, project file, README, recipe list and checksums before change are under `backup/blover-fusions-2.12.0.tar.gz`.

## Change 2.12.0: Eye Zombie and Blood Sunflower

The Eye Zombie from PvZ-Scared randomly replaces 1% of freshly generated unprotected Browncoats. It has 270 HP, wakes up when there are nearby enemy plants, zombies or projectiles and then deals 50 damage per bite according to the original animation rate. Ordinary death has an independent head chance of 1%; Instakills don't produce heads. The expected overall rate is therefore 0.01% of the original brown coats with only permissible types of death.

**Eye Zombie Head + Regular Sunflower = Blood Sunflower.** The head is a loose glove ingredient and cannot be planted in an open field. The Blood Sunflower has 300 HP and generates 25 Sun every 25 seconds. As specifically requested, after ten uninterrupted seconds, she transforms the plant directly to her right into another blood sunflower. Rootstocks and pumpkins remain; Changing goals resets progress. The first plant needs the head fusion.

Original artwork, original animations, original portrait and English/German almanac texts are included. Original sound is not used at the user's request; Awakening uses a native Replanted rage scream. Zombie phases, drop decision, loose heads and infection progression use native memory fields. The almanac contains 186 results. There are 236 active recipes; 433 recipes with 365 results remain open. Introducing new units increases the median version location once to **2.12.0**.

[Source, condition and test report](references/scared-2-12-0/report.md), [adapter commands](references/scared-2-12-0/adapters.json), [latest adapter builds](references/scared-2-12-0/adapter-build-final.log) and [installation receipt](references/release-2.12.0.json). Play and testing programs remain closed; the user undertakes all runtime checks. Particularly important are types of death, true loose head score, bite/protection interactions, both glove directions, and infection. Dismantling when the game is closed: Copy `backup/FusionMod-2.11.1.dll` to `Mods/FusionMod.dll` and load a save game without new IDs.

## Change 2.11.1: Merger partner via right click

Right-clicking on an activated normal plant entry opens the additional almanac tab “With: Plant name”. It shows all the different ingredient plants that can fuse with this plant via a currently implemented recipe. Fusion plants, self-fusions, buckets and football helmets are also taken into account. The detailed view shows the original plant, the selected ingredient and the result. This refers to ingredient partners; the existing Fusions tab still contains all 185 results.

The list comes directly from `FusionRecipes`, the same unordered recipe table as Cards and Glove. Native cards are identified by their bound `AlmanacEntryModel`, regardless of grid position or translated name. Native names and thumbnails as well as existing fusion portraits are reused. Bucket and football helmet use their original body sprites already embedded. Recipes that have not been implemented are not displayed. Without suitable recipes, an explicit empty message appears. Another right click in the normal plant view replaces the context of the additional tab.

“Plants” restores the normal view; the additional tab reopens the same partner list. The existing fusion catalog, page breaks, English/German labels, image sharing, native close actions, and controller neighbor targets remain in the shared view. The change only affects the almanac service; neither new plants nor gameplay changes are introduced. Therefore the last version point increases to **2.11.1**.

Static consistency check and release build; no game start and no test programs. Installation and backup: [Release receipt](references/release-2.11.1.json). Dismantling when the game is closed: Copy `backup/FusionMod-2.11.0.dll` to `Mods/FusionMod.dll`. Previous modified project files: `backup/almanac-partners-2.11.0/`.

Manual User Check: Right-click on Peashooter (37 different partners including Peashooter and Bucket), Puff-shroom (20 partners), Tall-nut (Football Helmet), and a plant without an active recipe. Card choice must give appropriate results. Left click, switch to “Plants” and “Fusions”, right click again on another plant, reopen/close, long names, language change and cross check controller navigation. Actual representation and input behavior are not confirmed by compilation.

## Change 2.11.0: Four water grass fusions

Four new results were implemented one after the other with their required capabilities. All ingredients were already available natively; Each plant has exactly one source recipe. Both card orders and glove directions use the existing fusion path. None of the four variants exist natively in Replanted.

- **Tangle Kelp + Puff-shroom = Tingle Kelp** ([Tingling Alge](references/tingle-kelp/port-notes.md)): When approached, it bursts after the original clip and creates 30 original ballistic projectiles, each dealing 20 damage. Crushing triggers the volley immediately. Water Zombies get Water Grass; Deadly aquatic weed applications can create new tingling algae in permitted free water fields.
- **Tangle Kelp + Fume-shroom = Grapple Kelp** ([Grapple Kelp](references/grapple-kelp/port-notes.md)): Three piercing hits for 30 damage each, additional water grass, point-based bonus damage, and knockback. The actual source radius is seven source units. The original controller sequence, fades, 1.5x speed of the hit clip, all bone meshes and sprite masks are included.
- **Tangle Kelp + Wall-nut = Tangle-nut** ([Algennuss](references/tangle-nut/port-notes.md)): 4000 HP and true blocking in the water. Bites are given by water grass. Crushing them doesn't kill them instantly; a crushing water zombie costs them 50 HP every 0.02 seconds and gains water grass. Damaged bodies and lost arms follow current health.
- **Tangle Kelp + Spikeweed = Spike-kelp** ([Spiky Algae](references/spike-kelp/port-notes.md)): 20 piercing contact damage per second including water grass. Vehicle hits cost you 10 HP. The original target states, contact phase and water animation remain taken into account; Vehicle contact is not instant destruction.

Water Grass halves movement speed, adds casting times, and retains points after the slow expires. From the seventh use onwards, 1% of the maximum body and helmet health follows as additional damage. From the 31st there is also a separate hit with the entire maximum health. Bosses are immune to water grass. Plant, missile and invisible native GridItem fields store phases, flight, points and remaining duration. Fertilizer heals all layers without special development or resetting of these conditions. Greed-kelp conversion requires plants not yet implemented; both almanac languages ​​name this limit.

Original models, all clips, bullet sprites, original portraits and full English/German almanac texts are included. The almanac contains **185 results**. Four completed recipe lines are omitted from `fusionen.txt`; **433 recipes with 365 results** remain open. Workflow and source archive remain unchanged. No time savings have been proven.

[Source, storage and release report](references/kelp-fusions-2-11-0/report.md), [affected adapters including user commands](references/kelp-fusions-2-11-0/adapters.json) and [installation receipt](references/release-2.11.0.json). Game and test programs did not start. The scope of testing includes static consistency, original export, visual previews and compilation. Specifically, the user checks water terrain, both recipe/glove directions, rootstock/pumpkin, all attacks, thresholds and propagation, bones/masks, true save, pause/death, fertilizer, and both almanac languages.

Dismantling when the game is closed: `backup/FusionMod-2.10.0.dll`, then load a save game without new plant, projectile or status entries. The previous project status is `backup/kelp-fusions-2.10.0.tar.gz`. The middle version position increases once to **2.11.0** for four new plants.

## Change 2.10.0: Four active nut fusions

Four new results use only native ingredients. Each has exactly one source recipe; both card orders and glove directions are active:

- **Wall-nut + Cabbage-pult = Cabbage-nut** ([Nusssalat](references/cabbage-nut/port-notes.md)): 4000 HP, stores actual health damage with the source multipliers and throws an equally powerful cabbage when more than 1000 energy. An equally strong, decaying shield absorbs numerical injuries.
- **Wall-nut + Melon-pult = Melon-nut** ([Melonuss](references/melon-nut/port-notes.md)): 8000 HP, 1.5x energy, limit 32000. Casts the energy as a melon including full area damage and gains the same shield.
- **Wall-nut + Threepeater = Three-nuts** ([Triple-Nut](references/three-nuts/port-notes.md)): 12000 HP, high blocking and rolling nuts with 80/60/40 damage for a total of 3/5/7 different targets. Lost minds stay lost; Fertilizer also heals to a maximum of 8000 or 4000 HP afterwards.
- **Wall-nut + Squash = Squash-nut** ([Mudnut](references/squash-nut/port-notes.md)): 4000 HP, jumps on your own field and 160 crush damage when landing. No blocking or bites in the air. The original clip lasts 1.666667 s; the translated almanac value of 1.5 s is corrected.

All skills, original models, all clips, damaged bodies, existing original projectiles and complete English/German almanac texts with original portraits are included. No new variant exists natively in Replanted. Saved plant and missile spaces gain energy, shield, phases, and head loss. Invisible native GridItems store the full target handles of rolling projectiles and prevent repeated hits on overlapping zombies even after loading.

Fertilizer heals all layers without special evolution, energy recharge, shield replenishment, phase reset, or additional shots. Triple Nut respects the permanent healing limit. The almanac contains **181 results**. Four completed lines are omitted from `fusionen.txt`; remain open **437 recipes with 369 results**. Workflow and source archive remain unchanged.

[Common source, health and status agreement](references/nut-fusions-2-10-0/report.md), [affected adapters and user commands](references/nut-fusions-2-10-0/adapters.json) and [build protocol](references/nut-fusions-2-10-0/adapter-build.log) document the implementation. Scope of testing: static consistency, original export, visual previews and compilation. **Game and test programs will not start.** User checks recipes and glove, cost/cooldown, roof/pads/pumpkin, shields, damage and area damage, head losses including fertilizer, penetrations, jump blocking, true save, pause/death and both almanac languages.

Installation receipt upon completion: `references/release-2.10.0.json`. Dismantling with closed game: `backup/FusionMod-2.9.0.dll`; previous project status: `backup/nut-fusions-2.9.0.tar.gz`. Then load a save game without new plants, bullet IDs or hit histories. The middle version location increases to **2.10.0** for four new plants.

## Change 2.9.0: Frosted cabbage, kimchi, iced corn and salad tosser

Four new results with all six available source recipes are implemented:

- **Cabbage-pult + Ice-shroom = Coldslaw** ([Frostkohl](references/coldslaw/port-notes.md)): 40 damage, ten freezing points before the icing test, quadruple damage and additional ricochet on iced hits. The living spawner gains triple animation speed for three seconds and attack intervals of 0.2 seconds; This is followed by a blocking time of 1.5 seconds.
- **Cabbage-pult + Jalapeno = Kimchi-pult** ([Kimchi Launcher](references/kimchi-pult/port-notes.md)): 60 damage and overheat before hit. Direct hits produce up to nine small projectiles, each dealing 20 base damage in a 3x3 area. Splinters retain superheat and do not divide again.
- **Kernel-pult + Ice-shroom = Cobsicle** ([Ice Corn](references/cobsicle/port-notes.md)): 20 ice grains or 40 ice butter, 25% butter chance, quadruple damage against already iced targets. Corn assigns freezing points based on damage; Butter then freezes for four seconds.
- **Cabbage-pult + Cob-pult**, **Kernel-pult + Overripe Melon** or **Melon-pult + Taco-pult = Salad-pult** ([Salatwerfer](references/salad-pult/port-notes.md)): 140 damage including area damage, 42% chance of chainable follow-up attacks. A direct hit creates an Overripe Melon and a Cornmelon or Buttermelon with all of the ricochet, area, and butter abilities present.

All ingredients were already implemented. Both card orders and glove directions are active. None of the four plants exist natively in Replanted. Kohltopf supports the new cabbage variants and salad tossers; the active frost cabbage cycle has priority. Butter pot increases iced corn and salad butter to 50%. Melon pot strengthens salad and both successors. Fertilizer heals all layers without special development and does not change phases, frost cabbage bonus, lock time, butter choice or flying missiles.

Four original plants and five new original floor models with all clips, layers, shadows and rotations are included. Salad successors also use three existing original models. Complete English and German almanac texts contain original portraits, accompanying texts and documented deviations from inaccurate source descriptions. The almanac includes **177 results**. Six completed lines are omitted from `fusionen.txt`; remain open **441 recipes with 373 results**. Workflow and source archive remain unchanged.

[Common source and status contract](references/elemental-lobbers-2-9-0/report.md), [affected adapters and user commands](references/elemental-lobbers-2-9-0/adapters.json) and [build protocol](references/elemental-lobbers-2-9-0/adapter-build.log) document the implementation. New attack and projectile states are saved natively. The existing volatile freezing points/thresholds and overheating still do not survive a game restart; both almanac languages ​​name this limit.

Scope of testing: static consistency, original export, visual previews and compilation. **Game and test programs are not started.** The user checks in particular all recipes and glove directions, roof/pads/pumpkin, costs/cooldown, ice and fire effects, frost cabbage phases, all small and large projectiles, lettuce chains and successors, pot bonuses, fertilizer, target loss, pause/death, real save/load and almanac in German/English.

Installation receipt upon completion: `references/release-2.9.0.json`. Dismantling with closed game: `backup/FusionMod-2.8.0.dll`; previous project status: `backup/elemental-lobbers-2.8.0.tar.gz`, additional previous almanac data: `backup/elemental-lobbers-almanac-2.8.0.tar.gz`. Then load a save game without new plant and floor IDs. The middle version location increases to **2.9.0** for four new plants.

## Change 2.8.0: Four more launcher fusions

New are **Cabbage-pult + Kernel-pult = Taco-pult**, **Kernel-pult + Melon-pult = Cob-pult**, **Cabbage-pult + Melon-pult = Overripe Melon** and **Cabbage-pult + Threepeater = Cabbage Tri-pult**. All ingredients were already available natively. Each result has exactly this one source recipe. Both card orders and glove directions are active. None of the four plants exist natively in Replanted; the existing corn cannon remains its own native result.

- [Taco-pult](references/taco-pult/port-notes.md) throws tacos for 40 damage or butter tacos for 60 damage every 3 s. Both hit up to three different zombies. Butter lasts four seconds. Each bounce halves the flight speed; the landing ends the projectile.
- [Cob-pult](references/cob-pult/port-notes.md) throws Cornmelons with 140 damage or Buttermelons with 160 damage and Melon area damage every 3 s. Butter holds the direct target for four seconds; the duration in the environment is max(0.5; integer 4/target number) seconds. The original code uses 140/160 instead of the outdated English translation values ​​of 120/140.
- [Overripe Melon](references/overripe-melon/port-notes.md) throws melons every 3s, dealing 80 direct damage, a ricochet, and area damage on each hit and on the final landing.
- [Cabbage Tri-pult](references/cabbage-tri-pult/port-notes.md) throws cabbage for 40 damage across three rows every 1.5s, with edge replacement. After hits, the next target in a neighboring row follows exactly. Side projectiles jump once, the middle one twice; Follow-up projectiles fly 0.75 s. The English translation describes the single target rule inaccurately.

Cabbage Pot Supports Taco, Overripe Melon and Triple Cabbage; Butter pot supports taco and cornmelon. Melon pot strengthens the two new melons with a proven factor of 0.67. Triple Pot increases triple cabbage damage including ricochet to 48. The previous family values ​​remain. The new launchers use the original interval fluctuation and shot events at 0.316667/0.333333/0.25 s, mapped to native hundredths of a second ticks. Fertilizer heals all layers; None of the four variants has a special development. It does not change phases, butter selection or flying projectiles.

Four plant models and seven bullet models including all butter and final variants come from Fusion 4.0.5.2. Original clips, additive layer, rotation, shadows, original portraits and full English/German almanac texts are included. Saved native fields have phases, damage, pot bonus and target handles already hit. The four completed recipe lines are omitted from `fusionen.txt`. Remain open **447 recipes with 377 results**; the almanac includes **173 results**. Workflow and `fusionen-quellen.json` remain unchanged.

[Common source and state contract](references/lobber-fusions-2-8-0/report.md), [Adapter and user commands](references/lobber-fusions-2-8-0/adapters.json) and [Build protocol](references/lobber-fusions-2-8-0/adapter-build.log) document the evidence. Scope of testing: static consistency, original export, previews and compilation. **Game and test programs will not start.** Manually, cards/glove in both directions, costs/cooldown, terrain/pads/pumpkin, all bullet variants and pot bonuses, target loss, butter immunity, edge rows, real save, pause/death, animations and almanac remain to be checked in both languages.

Installation receipt: `references/release-2.8.0.json`. Dismantling with closed game: `backup/FusionMod-2.7.0.dll`; previous project status including tests and documentation: `backup/lobber-fusions-2.7.0.tar.gz`. Then load a save game without new plant and projectile IDs. The middle version location increases to **2.8.0** for four new plants.

## Change 2.7.0: Two shooter pots and two triple launchers

New are **Flower Pot + Peashooter = Pea Pot**, **Flower Pot + Threepeater = Tri-pot**, **Kernel-pult + Threepeater = Kernel Tri-pult** and **Melon-pult + Threepeater = Melon Tri-pult**. All the ingredients were already available. Each result has exactly one source recipe; Both card orders and glove directions are supported. None of these four variants exist natively in Replanted.

- [Pea Pot](references/pea-pot/port-notes.md) generates five additional attacks from its legal pea plants after 30 primary attack events. These count themselves; After the first trigger, 25 more normal events are sufficient. The original projectiles retain all abilities and receive the original random muzzle offset.
- [Tri-pot](references/tri-pot/port-notes.md) refunds 100 sun once, increases the attack power of the existing triple archer line by 20%, and creates two side attacks with half attack damage after six primary pea attacks. Edge rows are replaced in their own row. Torchwood, Ice, Poison, Hypnosis, Iron and Cherry Charge are still taken into account; Fixed explosions retain their fixed damage.
- [Kernel Tri-pult](references/kernel-tri-pult/port-notes.md) throws corn or butter over three rows every 1.5 s. Non-butter series increase his additional bullets; Butter then resets the series. The exact counting rule delivered is in the port notes and both almanac languages. Butter pot and triple pot work on the new variant.
- [Melon Tri-pult](references/melon-tri-pult/port-notes.md) throws three melons every 1.5 s for 80 direct damage and full area damage. Rim replacement, triple pot and melon pot are included. Both throwers adopt the original interval fluctuation, the shot event at 0.25 s and the original throwing trajectory.

Fertilizer heals the new plants including all layers. The source does not contain any special development for them; Fertilizer does not change pot counts, non-butter series or butter selection and does not award sun or additional salves. Saved plant and projectile fields gain counters, chosen salvos, and attack damage across loads. The two pots preserve the existing generation protection of 0.5s. Card costs, cooldown, documents and pumpkins use the existing owners.

Original models, all required clips, three new original floor models, and original portraits come from Fusion 4.0.5.2. All four results have complete English and German almanac texts including accompanying text. Remain open **451 recipes with 381 results**; the almanac includes **169 results**. `fusionen-quellen.json` and workflow remain unchanged. There was no documented need for a workflow change.

Test limit: Static consistency and compilation, no test programs running and no game starting. The **27 affected offline suites** and their user commands are in [adapters.json](references/support-pots-2-7-0/adapters.json). The release build additionally checks the actual native types and renderers. Manually, both card orders and glove directions, cost/cooldown, terrain and documents, animations, all receivers/projectiles, edge replacement, fertilizer, pause, death, move, real save/load and the complete almanac in German/English remain to be checked.

Installation receipt: `references/release-2.7.0.json`. Dismantling with closed game: `backup/FusionMod-2.6.0.dll`; previous sources `backup/support-pots-2.6.0.tar.gz`, previous offline suites `backup/tests-support-pots-2.6.0.tar.gz`. Then load a save game without new plant and projectile IDs.

## Change 2.6.0: Four reactive pot fusions

Four results from the local Fusion source 4.0.5.2 have been implemented. Both ingredients were already available natively; each result has exactly this one source recipe:

- Flower Pot + Cherry Bomb = **Cherry Pot**: Cherry explosions heal all plants on the same field for 20 HP. Cherry Archers gain 50% increased attack speed for five seconds. Additional explosions renew the duration without stacking.
- Flower Pot + Jalapeno = **Hot Pot** (Fire Pot): Jalapeno row effects heal Fire Archers above them for 20 HP and grant 50% increased attack speed for five seconds. There are at least 0.2 seconds between triggers. Bonuses stack additively and expire independently.
- Flower Pot + Hypno-shroom = **Hypno Pot**: Passing hypnotized zombies permanently receive a third less numerical damage. Within a Source World unit radius and the same row, the pot heals one Body HP every 0.04 seconds. When bitten, it hypnotizes the attacker and becomes a normal flowerpot; an upper plant remains.
- Flower Pot + Doom-shroom = **Doom Pot**: prevents doom craters on your own field. This costs 75 maximum and current HP and then 20% of the reduced maximum HP. The prevention also applies in the event of a fatal victim. Craters recover twice as fast when the pot is present, in 90 seconds instead of 180, and heals the doom pot with the lowest HP on the same row by 2% of its maximum HP every second.

All four pots start with 300 HP and the existing generation protection of 0.5 seconds. Fertilizer heals without development. In Doom Pot, it does not restore sacrificed maximum HP. Original models, all original clips and portraits are included; the fire pot also has its continuous additive fire animation. No new projectiles are needed. Reinforced Riflemen use all existing projectiles and abilities. Native Repeater, Split Pea, and Gatling volleys keep their events at remaining counts of 25 and 50, respectively.

The cherry pot supports the five existing cherry shooter variants, the cherry thrower and the firecracker. The Fire Pot supports the four Ember Archers, the Scorched Triple Archer, the Flame Mushroom, and the Flame Gloom Mushroom. Source plants that have not yet been ported will not be listed as available ingredients or recipients. Attack bonuses only change the remaining attack count; Shooting animations and projectile damage remain with the respective family. Hypnosis protection affects numerical damage; direct special distance without damage value remains fatal. Fractions maintain the effect in small bites.

Time-limited bonuses and permanent hypnosis protection are stored as invisible native GridItems with full recipient DataID. They follow the recipient as they are relocated and survive the removal of the donating pot. Death or pool reuse removes orphaned effects. The Crater Healing Counter uses a separate saved field; the existing ice duration of Demise Shroom Crater is preserved. The native Doom-shroom has embedded stack destruction before crater creation. The new protection therefore specifically takes over this branch before other plants are lost; Doom's own fusions use the same sacrificial contract before their collective crater creation.

Both card orders and glove directions use existing recipe resolution, placement check, and generation before ingredient consumption. Top plants and pumpkins remain. All four completed recipe lines are removed from `fusionen.txt`; the source archive remains unchanged. 455 recipes with 385 results remain open. The bilingual fusion almanac includes 165 results. The middle version location increases to **2.6.0** for four new plants.

Technical sources, behavior, storage, adjustments and concrete user tests are available at [Kirschtopf](references/cherry-pot/port-notes.md), [Feuertopf](references/hot-pot/port-notes.md), [Hypnotopf](references/hypno-pot/port-notes.md) and [Untergangstopf](references/doom-pot/port-notes.md). Static consistency, original export, calibration and C# compilation are checked. All 26 adapters with a common fusion identity are compiled exclusively; Protocols: `references/reactive-pots-validation/`. The extended suite `DefensivePotsTests` contains the actual new behavior class. They isolate the remaining family adapters with a common collaborator double.

**Game and all test programs remain with the user.** Check: all recipe and glove directions, roof, existing top plants/pumpkins, fertilizer, original animations, attack bursts with full volleys, separate bonus processes and duration renewal, hypnosis healing and permanent protection, bite conversion, fatal health sacrifice, crater healing and repair time, demise ice duration, pause, and true save/load during all effects. Commands: `../.tools/dotnet/dotnet run --project tests/DefensivePotsTests.csproj -c Release` and the other suites mentioned in the adapter report. Builds do not replace these checks.

Installation receipt: `references/release-2.6.0.json`. Dismantling with closed game: `backup/FusionMod-2.5.0.dll`; previous sources and documentation: `backup/reactive-pots-2.5.0.tar.gz`. To restore, use a save game without new pot IDs and without saved new effects. Workflow, complete recipe archive, profiles and saves remain unchanged. A necessary workflow change with proven time savings did not arise.

## Change 2.5.0: Four more pot fusions

Four results with all available source recipes have been implemented. Each has exactly one recipe; both ingredients are already available natively:

- Flower Pot + Wall-nut = **Nut Pot**: 20% reduced numerical combat damage in your cell, including stored damage fractions.
- Flower Pot + Garlic = **Garlic Pot**: exactly 10 HP per successful bite on garlic plants in the same cell; The biter moves to the middle row if possible. Supports garlic, silver garlic, hazelnut garlic, rotten mushroom, tripli stinker and the four poison pea varieties. Hypno garlic maintains its transformation and hypnosis with the same row preference.
- Flower Pot + Spikeweed = **Spike Pot**: 20 damage to nearby ground zombies on the same row approximately every second, tire burst with pot consumption, and 50% reduced numeric damage to the spikeweed above. Their existing vehicle contact costs 250 instead of 500 HP. Biting zombies ignore the low ground.
- Flower Pot + Melon-pult = **Melon Pot** (melon pot): one-time 150 sun upon successful fusion. Melona Pult, Winter Melon, and Silver Melon above receive two-thirds of secondary target damage without any quantity reduction (53 and 66, respectively). Direct damage and winter cold remain. Missiles already in flight retain their stored bonus after pot movement or death.

All four pots have 300 HP, native pot placement, and the existing 0.5 second generation protection. Fertilizer heals the existing layers; none of the four source IDs have a fertilizer development. The spiked pot does not receive any additional fertilizer reload. Original models, all required clips and portraits come from the local 4.0.5.2 source. The bilingual almanac includes 161 results. Calibration sheets document fixed sizes and anchors; they do not replace a visual inspection in the game.

Native instant destruction and crushing without numerical damage remain deadly. Replanted does not have any lava fields for the Nutpot's lava immunity. The additional synergies with the unavailable Obsidian Thornnut, Chronomelon, Maelonstorm, Calamity Melonapult, and Melon Shell remain inactive. The Melon Twill does not have an amplifying AoE path. Capabilities, source addresses, storage and specific user tests are available at [Nusstopf](references/nut-pot/port-notes.md), [Garliktopf](references/garlic-pot/port-notes.md), [Stacheltopf](references/spike-pot/port-notes.md) and [Melonentopf](references/melon-pot/port-notes.md).

Version 2.5.0 increases the middle spot for new plants. Static consistency, original export, calibration and C# compilation are checked; The game and all test programs remain unstarted. Build logs: `references/defensive-pots-validation/`; Installation receipt: `references/release-2.5.0.json`. In particular, the user checks both recipe and glove directions, terrain, fertilizer, lethal boundary hits, row changes, spike attack and tire burst, enhanced melons, and true save/load. The prepared suites `DefensivePotsTests`, `ShroomSlotsTests`, `SunflowerFusionsTests`, `PuffLobbersTests`, `WeedFusionsTests` and `SupportFusionsTests` can be run with `../.tools/dotnet/dotnet run --project tests/<Name>.csproj -c Release`.

Previous sources and documentation: `backup/defensive-pots-2.4.0.tar.gz`. Previous installed DLL: `backup/FusionMod-2.4.0.dll`. To restore the game, use a save game without new pot IDs. Workflow, complete recipe archive, profiles and saves remain unchanged.

## Change 2.4.0: Three pot fusions and native corn cannon recipe

Four more results from `fusionen.txt` have been implemented. All ingredients were already fully available; each result has exactly one source recipe:

- Flower Pot + Cabbage-pult = **Cabbage Pot** (cabbage pot): shortens the attack intervals of Kohlapult, Salat-Zwille and Silberkohl by a third, i.e. approximately 50% more attacks.
- Flower Pot + Kernel-pult = **Butter Pot**: increases the butter chance of Kornapult, Mais-Zwille and Silberkornapult from 25 to 50%. Butter Animation, Shrinking, Damage, Stun, Money, and Stored Missiles remain with their existing families.
- Flower Pot + Umbrella Leaf = **Umbrella Pot** (umbrella pot): increases the recoil of the umbrella nut, umbrella cover and silver umbrella from 160 to 240 board pixels. Foxglove screen also receives 50% more recoil (approximately 118.52 to 177.78 board pixels at its existing small plant scale). Umbrella cover numbs with butter for four seconds instead of two. The pot itself does not repel basketballs or bungees.
- Kernel-pult + Cob Cannon = **Cob Cannon** (corn cannon): recipe for the existing native plant only. A cannon card still requires two adjacent cornapets. Kornapult on existing cannon replaces it within its space. The same requirements apply to gloves. There is no additional corn cannon implementation.

When comparing sources, a swapped zero comparison was found in the previous screen porting. Without a pot, the umbrella nut, umbrella cover and silver umbrella correctly repel two source units instead of the previously used three. Three units apply with an umbrella pot. The actual equality helper and PE constants called are archived at `references/support-pot-effects/`. The short thimble umbrella already had the correct basic value; he now receives the documented bonus. Unported Cabbage, Garlic, and Reflection Screens and Super Abilities remain specifically unavailable.

All three pots have 300 health and the shared saved generation protection of 0.5 seconds. Original models, all idle clips, sprites, portraits and full English/German almanac entries are included. The pots do not produce their own projectiles. Native and existing imported projectiles are fully reused. Fertilizer heals the pots and the native cannon; none of these result IDs receive a new evolution, additional salvo, or instant reload. The fertilizer development of two corn pulps, which is only available in the other source version, is not additionally introduced because the corn cannon remains expressly native.

Pot effects are determined from the living plants of the same cell. Moving, removing and loading do not leave a permanent bonus. The cabbage pot only changes newly scheduled intervals; a remaining counter that is already running remains. Butter choice and matching animation are saved once per attack. Cards and Glove produce results before ingredient consumption. The Corn Cannon Path protects and consumes both corn pulps together; both result fields are checked. Existing plants above a potted ingredient and pumpkins are retained. Cannon Fusion reinitializes health and reload phase; the condition of a spent cannon is not transferred.

Technical sources, storage, customizations and user reviews are available at `references/cabbage-pot/port-notes.md`, `references/butter-pot/port-notes.md`, `references/umbrella-pot/port-notes.md` and `references/native-cob-cannon/port-notes.md`. The four completed lines are removed; the source archive remains unchanged. 463 recipes with 393 results remain open. The Fusion Almanac includes 157 results. For three new plants, the median version location increases to **2.4.0**. Workflow and its linkage remain unchanged; A necessary process change with a measured time gain did not arise.

Static consistency, original export, calibration and release build are checked. All 25 affected C# adapters were exclusively compiled; Protocol `references/support-pots-adapter-builds-2.4.0.txt`. The game and all test programs remain with the user. Installation receipt: `references/release-2.4.0.json`. Dismantling with closed game: `backup/FusionMod-2.3.0.dll`; previous sources and documentation: `backup/support-pots-2.3.0.tar.gz`. Remove new pot variants from savegames that can be reused before permanent dismantling.

Check manually: all four recipes in both map/glove directions, pots under existing plants, roof and pumpkins, actual throwing intervals, butter selection including animation and already flying projectiles, basic recoil and pot bonus of all four umbrella families, four seconds of butter and immunities, generation protection and fertilizer, pause, move/remove, save/load and both almanac languages. For the cannon, check in particular the second front sight, edge gap, occupied right field, production error without loss of ingredients, reloading and manual firing. Prepared regressions: `PuffLobbersTests`, `PuffSupportTests`, `UmbrellaShellTests`, `SunflowerFusionsTests` and `ShroomSlotsTests`. Builds do not replace these checks.

## Change 2.3.0: Silver pot, silver umbrella, silver grain pulpit and light pot

Four more results from the local Fusion source 4.0.5.2 have been implemented:

- Flower Pot + Marigold = **Silver Pot**: 300 life points, support for another plant, money production approximately every 25 seconds.
- Marigold + Umbrella Leaf = **Silver Umbrella**: 1000 life points, native protection in the area of ​​3 × 3 fields, 40 contact damage with knockback and 25 coins per basketball defended. With this source variant, contact attacks cost no life points and do not bring any money; Bungees don’t make any money either.
- Kernel-pult + Marigold = **Silver Kernel**: 300 life points, thrown approximately every three seconds, 40 grain damage. With 25% chance, the plant will throw silver butter for 60 damage, four seconds of native stun, and 25 coins. Own row has priority; without a suitable target there, other rows are taken into account. Butter also pays for ground hits.
- Flower Pot + Plantern = **Lumos Pot**: 300 life points, pad with light for exactly your own field. The native fog mask is brightened at the two vertical field edges; There is no additional system for fusion light levels.

All ingredients were already fully available. Every result has exactly this one source recipe. Both card orders and glove directions use the existing paths; That doesn't prove it in playtests. Pot fusions preserve existing plants above the rootstock. Cost and card cooldown still come from the incoming ingredient. The four completed lines are removed from `fusionen.txt`; the complete source archive remains unchanged.

Original plants, both silver corn projectiles, animations, event receipts and complete English/German almanac entries are included. Silvercorn uses the original events at 0.333333 seconds; the saved butter choice also selects the appropriate `shoot2` clip. Native shadows, flight movement and saving remain in the existing renderer and projectile format. The calibration shows the high swinging limb partially outside the fixed comparison image; This is an image limitation of the offline preview and not a cropped runtime graphic.

Replanted stores money in increments of ten. The new 25 coins therefore alternately pay out 20 and 30 coins as native silver coins. The remaining five coins remain stored in `Plant.mTargetY`; for silver butter, the already determined payout value is also stored in the saved projectile field `mCobTargetRow`. A single yield is therefore not exactly 25; after two returns the total is correct. If a plant with remainder dies, this remainder expires. Coins and flying butter that have already been generated remain stored natively. There are no direct profile or inventory write accesses; Only the normal collection of coins uses the native money route.

Fertilizer heals all four plants to their maximum health. None of these source IDs have fertilizer development. Both new pots receive the common 0.5 second protection of the source. It blocks bites, crushes, enemy projectiles and explosions, or enemy line fire. The remaining period is saved; Fertilizer doesn't shorten it any further. Shovel, ingredient consumption and board dismantling remain possible. Fertilizer therefore neither triggers money production nor an additional attack. The separately paid gold plants and super abilities are not part of these four normal recipe results and are not activated. The current source methods replace outdated English information about a loss of life points upon contact with the silver screen.

Technical evidence and concrete user tests: `references/silver-pot/port-notes.md`, `references/silver-umbrella/port-notes.md`, `references/silver-kernel/port-notes.md`, `references/lumos-pot/port-notes.md`. The most complex part was matching the original abilities with the native callers and memory fields, especially silver corn homing, butter hitting, basketball defense and short-term pot protection. There are no separate time measurements for all work steps. The workflow remains unchanged because no necessary process changes have been made that would result in a proven time gain.

The middle version location increases to **2.3.0** for four new plants. Static consistency, original export and release build are checked; Ten affected C# test projects were exclusively compiled. The game and all test programs remain with the user. Build log: `references/silver-support-adapter-builds-2.3.0.txt`; Installation: `references/release-2.3.0.json`. Dismantling with closed game: `backup/FusionMod-2.2.0.dll`. Previous sources and documentation: `backup/silver-support-2.2.0.tar.gz`. Remove new variants from savegames that can be used before permanent dismantling.

Check manually: all four recipes in both directions with cards and glove, occupied pots and roof, visible original animations, row change of silver corn targets, butter on single target and ground, four second stuns and immunities, basketball defense against bungee/contact attacks, sums of money over multiple returns, light field and re-closing of the fog after distance, fertilizer, pause, death and save/load during throw, flight and production. For both pots, also check for hits immediately after they are generated and after the first half second has elapsed. Check English and German almanacs and language changes. Appropriate prepared tests are available in `PuffLobbersTests`, `SunflowerFusionsTests` and `UmbrellaShellTests`; the full adapter list is in the build log.

## Change 2.2.0: Four more silver fusions

New are Doom-shroom + Marigold = Silver Doom-shroom, Hypno-shroom + Marigold = Silver Hypno-shroom, Cabbage-pult + Marigold = Silver Cabbage and Marigold + Melon-pult = Silver Melon. All the ingredients were already available; every result has exactly this source recipe. Both card orders and glove directions use the existing fusion path. The four completed recipe lines are removed from `fusionen.txt`.

Silver Doom-shroom remains completely stable according to the original: no explosion, no crater and no coin production. Shatter removes it without explosion. Silver Hypno-shroom uses the native hypnosis while eating and drops a total of 100 money after successful hypnosis. Two native gold coins of 50 each retain the monetary value even when saving and loading. Both mushrooms keep Replanted's sleep and coffee rules.

Silver Cabbage throws silver cabbages with 60 direct damage, Silver Melon throws silver melons with 100 direct damage and up to 33 area damage. Both attack approximately every three seconds; their original clips trigger the cabbage toss after 0.316667 and the melon toss after 0.333333 seconds. The native ballistic movement takes over the 1.5 second long trajectory. Melons also cause area damage when hit on the ground within 1.5 source world units and at most one neighboring row. For very large groups, this damage drops to at least 1 according to the original quantity limit. Shields that are being carried are thrown over. Silver carbon bullets rotate at 300 degrees per second.

All four plants have 300 life points. Fertilizer heals them; it does not activate an explosion, a second coin reward, or a gold plant upgrade. The gold plants purchased separately for money and their additional abilities are independent results that are still not available. Original models, all clips, both projectile models and the portraits are from the local 4.0.5.2 source. The bilingual fusion almanac now includes 149 results. Sizes and anchors are documented in static calibration sheets.

Capabilities, sources, storage, deviations and specific user checks are available at `references/silver-doom-shroom/port-notes.md`, `references/silver-hypno-shroom/port-notes.md`, `references/silver-cabbage/port-notes.md` and `references/silver-melon/port-notes.md`. The version number increases to 2.2.0 for four new plants. Static consistency and release build are checked; The game and all test programs remain closed. The extended existing C# test projects are only compiled. Release document: `references/release-2.2.0.json`. Dismantling the DLL when the game is closed: `backup/FusionMod-2.1.6.dll`. Previous sources and documentation: `backup/silver-combat-2.1.6.tar.gz`. Workflow, complete recipe archive, profiles and saves remain unchanged.

## Change 2.1.6: Technical documentation of capabilities and limitations

The new [Capabilities and Limitations Overview](faehigkeiten-und-grenzen.md) describes common fusion rules, recipe status, storage, and the power of porting tools. An existing Cherry spreader port shows the separation of ability, numbers with units, adjustment and limit. [Workflow, Section 7.2](workflow.md#72-fachliche-dokumentation-der-fähigkeiten-und-grenzen), also accessible via `worklfow.md`, specifies the technical notes per result: original documents, replanted implementation, missing capabilities, life cycle and concrete user tests.

`python tools/fusion_checks.py notes --result "Cherry-spreader"` produces an explicitly unchecked Markdown draft to stdout. The existing helper takes over the source ID, confirmed result identity, stored availability document and all recipe alternatives from the existing project files. Active, blocked and still unregistered recipes remain separated. Input checksums record the status; Conflicting identities, inconsistent registrations, or input changes during generation will result in an error. Capabilities, native callers, graphics and memory behavior are still subject to technical documentation. Existing notes will not be overwritten. There is no new data maintenance for skills and no re-export of sources. A time saving for the entire porting was not measured.

For the ordered release the fix number increases to 2.1.6. Gameplay, plants, recipes, save identifiers and graphics assets do not change. Python syntax, actual documentation output, static consistency and release build are checked; Regression testing and play testing remain with the user. Supplemented synthetic tests are available in `tools/test_workflow.py`. Installation and build receipt: `references/release-2.1.6.json`. Removing the mod when the game is closed: `backup/FusionMod-2.1.5.dll`. The previous tool, version and documentation files are under `backup/capability-docs-2.1.5.tar.gz`.

## Change 2.1.5: Targeted initial export without full index

Graphics export continues to load the source bundle once per batch, but no longer builds a full source index. It checks the selected prefabs, root transforms and AnimatorController directly. This eliminates the need for additional decoding of all GameObjects, Controllers and AnimationClips for the global name search during the first targeted export without a cache. The source index remains available for research. Sprite image and metadata now come from a UnityPy object read once. Streamed frames use standard library batch decoding; invalid frame sizes and channel indexes are rejected. PNG compression and model format are preserved.

The CLI export first creates and checks the entire selected batch in a temporary project folder. The supported source DLL is checked in advance; Input checksums are checked again before acceptance. Then, renames on the same file system replace the changed files while preserving byte-equal targets. Evidence of success is adopted last. An error in decoding or a changed source leaves the previous outputs unchanged. If the final transfer fails, files may already have been replaced; missing documents require a new export. A file lock denies concurrent CLI exports. The temporary folder requires space for the selected batch. Running times for loading, decoding with output and acceptance appear separately; There is no measured comparison or overall time gain.

Guidance and limits are available in `workflow.md`, Section 5, also accessible via `worklfow.md`. The existing call with repeated `--plant`, `--all` and the standard model remain available. For the ordered release the fix number increases to 2.1.5. Gameplay, plants, recipes, save identifiers and existing graphics assets do not change. Python syntax, static consistency and release build are checked. Game, export runs and regression testing remain with the user; the supplemented synthetic tests are in `tools/test_export_snow_repeater.py`. Installation and build receipt: `references/release-2.1.5.json`. Removing the mod when the game is closed: `backup/FusionMod-2.1.4.dll`. The previous tool and documentation files are under `backup/source-export-2.1.4.tar.gz`.

## Change 2.1.4: Common C# test adapters

The offline test projects now share their related mod sources across five MSBuild source groups under `tests/Adapters/`. An additional group includes the true fusion identity and a full native `SeedType` definition; 16 previously separate enum copies are no longer necessary. The previous standard identities of the test doubles are explicitly set. Different behavioral doubles stay with their respective families. A new shared dependency is added once in the appropriate group, instead of in each consumer individually.

`python tools/csharp_adapters.py --changed src/ShroomSlots.cs` shows all affected suites and their manual test commands. The script queries the sources actually evaluated by MSBuild and detects common stub files, imported source groups and central project settings. Missing or duplicate entries and files without a connected suite generate an error. Without file selection, all 27 projects appear. `--build` compiles the selected projects exclusively; Test programs are never started. Instructions and limitations are available in `workflow.md`, Section 7.1, also accessible via `worklfow.md`.

For the ordered release, the last version point increases to 2.1.4. Gameplay, plants, recipes and save identifiers do not change. All 27 offline projects are just being built; Regression testing and play testing remain with the user. The new synthetic tool checks are below `tools/test_csharp_adapters.py` and have not been run. Static migration documents and build outputs are below `references/csharp-adapters-*-2.1.4.*`; the release document is under `references/release-2.1.4.json`. Removing the mod when the game is closed: `backup/FusionMod-2.1.3.dll`. The previous test, tool and documentation files are under `backup/csharp-adapters-2.1.3.tar.gz`. An acceleration of the overall porting was not measured.

## Change 2.1.3: Shared graphics calibration

`tools/preview_snow_repeater.py --calibrate` adds a fixed scale comparison sheet and JSON measurement report to the existing original preview. Plant size and ground anchors are calculated once from the real sprite boundaries of the initial pose. A common grid makes size differences and animation offsets visible. Optional projectiles use the same scale, an explicitly assigned reference width and, if necessary, the factor or root correction of their runtime adapter. Known puff variants require full graphics measurement and a measured native target width. The report includes boundaries, node origins, scaling, anchors, and input and image checksums.

The existing animation, PNG, and binding caches remain shared. The preview now takes into account the reflection around the sprite origin even for off-center pivots; Zero scales do not draw a surface. The previous original preview and the special sun sphere comparison remain available. Instructions and examples are available in `workflow.md`, Section 5.1, also accessible via `worklfow.md`. A time saving during the entire porting process has not yet been measured. The preview does not replace testing native controllers, trajectories, or capabilities.

For the ordered release, the last version point increases to 2.1.3. Gameplay, plants, recipes and save identifiers do not change. Game, regression testing and initial preview generation remain with the user; prepared are `tools/test_preview_snow_repeater.py`. Syntax, static consistency and release build are checked. Installation and build receipt: `references/release-2.1.3.json`. Dismantling with closed game: `backup/FusionMod-2.1.2.dll`. The previous tool and documentation files are under `backup/visual-calibration-2.1.2/`.

## Change 2.1.2: Shared native behavior analysis

`tools/research_native.py` replaces the repeated Replanted one-time scripts for new research. A SHA-256 validated caller index is reused between results. The tool archives direct calls and jumps including `jmp`, lists indirect calls as open, and adds target and lifecycle methods as search candidates. For selected native fields, it collects read, write and address candidates including covering reset writes. Token-based filenames get overloads; Source checks, output checksums, file locks and a last written proof of success protect the research output.

Instructions: `workflow.md`, section 4.1, also accessible via `worklfow.md`. Static sample document: `references/native-analysis-potato/native-report.md`. It shows, among other things, the actual `jmp` from `Zombie.CheckIfPreyCaught` to `Zombie.EatPlant` and memory/lifecycle candidates for `Plant.mStateCountdown`. Method areas can contain additional native helpers; Offset hits do not occupy an object type without register analysis. Virtual calls, third-party field writers, embedded implementations and actual capabilities remain subject to technical review. An overall time saving when porting was not measured.

For the expressly requested release, the last version point increases to 2.1.2. Gameplay, plants, recipes and save identifiers do not change. The regression tests in `tools/test_research_native.py` are prepared and remain with the user along with game tests. Python syntax and actual static research are examined; Game and test programs will not start. Installation and build receipt: `references/release-2.1.2.json`. Dismantling with closed game: `backup/FusionMod-2.1.1.dll`.

## Change 2.1.1: Generate C# registrations together

The porting workflow now maintains identity, `FusionPlant.From`, families and plant renderers together in `tools/plant-registrations.json`. `tools/registrations.py` uses this to create a testable patch for the previous C# files. For a new imported plant, its memory pair and the technically appropriate family assignments are sufficient. Model name, recognition and `HasImportedVisuals` follow automatically. Behavior, Recipes, Sources, Exports, Resources, Ingredient Availability and Almanac remain within their existing responsibilities.

135 imported plants, 31 families and a reserved synergy are adopted. The 134 stored variant pairs, the pairs recognized by `From`, family overlaps as well as renderer order and resource names remain unchanged according to the static source comparison. Gloom-shroom remains native; Starry Pumpkin remains reserved. Unknown markers and native imitators retain their native identity. There is no new plant and no intended gameplay change. The configuration file is only required for development; the mod continues to only load C# registrations.

The existing release check denies outdated generated C# registrations. Instructions and user testing are available in `workflow.md`, Section 6.1, also accessible via `worklfow.md`. Source files before the conversion: `backup/csharp-registrations-2.1.0/`. Static comparison: `references/csharp-registrations/migration-2.1.1.json`. New regression tests are prepared in `tools/test_registrations.py`; The existing workflow check takes outdated family and renderer entries into account. Tests and game start remain with the user. There is no measured time gain.

2.1.1 increases the final version location for consolidating existing registrations. Installation and build receipt: `references/release-2.1.1.json`. Dismantling with closed game: `backup/FusionMod-2.1.0.dll`.

## Change 2.1.0: Four silver fusions

New are Marigold + Wall-nut = Silver-nut, Garlic + Marigold = Silver Garlic, Ice-shroom + Marigold = Silver Ice-shroom and Marigold + Sunflower = Silver Sunflower. All ingredients were already available natively. Each result has exactly this source recipe; Both card orders and glove directions are supported. The four completed recipe lines are removed from the open list.

Silver-nut has 6000 life points, Silver Garlic 600 with native garlic row change. Silver Ice-shroom activates a 20-damage global freeze and five seconds of freeze after 1.5 seconds of wakefulness; Shatter also activates it. Coffee remains natively processed. Silver Sunflower produces 25 suns each about 15 seconds including original dispersion and glow phase. Fertilizer cures the variants; with the silver ice mushroom it shortens a longer rest to 0.5 seconds. None of these plants require new attack projectiles. Separate gold plant upgrades for coins remain outside of these four silver outcomes and are specifically noted in the almanac.

Original models, all original clips, damage images and original portraits are from the local 4.0.5.2 source. The Fusion Almanac now includes 145 results with full English and German texts. Sources, saved states, abilities, fertilizers, limits and manual checkpoints: `references/silver-plants/port-notes.md`. Workflow and complete recipe archive remain unchanged.

2.1.0 increases the middle version location for four new plants. Release testing and build take place via the existing release path; Game and test programs will not start. Installation: `references/release-2.1.0.json`. Dismantling with closed game: `backup/FusionMod-2.0.0.dll`. The user is responsible for game testing and prepared regression tests.

## Change 2.0.0: Four fusion recipes for native upgrade plants

New are Ice-shroom + Melon-pult = Winter Melon, Magnet-shroom + Marigold = Gold Magnet, Lily Pad + Cattail = Cattail and Spikeweed + Spikerock = Spikerock. Every result has exactly this source recipe. Both card orders and glove directions use the existing fusion path; all ingredients are already available natively. The four completed entries are removed from the open list.

All four plants already exist in Replanted and are therefore only produced natively. Graphics, animations, abilities, projectiles and storage are reused. Gold Magnet natively collects coins and diamonds; The projectile attraction and shot control from PvZ Fusion are not added. Winter Melon retains native cold and area damage without fusion cryo points or quadruple freeze damage. Fertilizer cures the results without further development. The previous fertilizer upgrades of spikeweed and exposed lily pad remain in place.

The Fusion Almanac now contains 141 results with original portraits as well as full English and German entries. Sources, native identities, behavior, deviations and manual checkpoints: `references/native-upgrade-recipes/port-notes.md`. Workflow and complete source archive remain unchanged.

2.0.0 increases the gameplay location for additional recipes without new plant implementations. Release build and static testing take place via the existing release path; Test programs and game tests remain with the user. Installation: `references/release-2.0.0.json`. Dismantling with closed game: `backup/FusionMod-1.8.4.dll`. Profiles, scores and LawnGlove remain unchanged. Manually check in particular both recipe directions, cat tail on water and occupied water lilies, roof and underlay, preview, fertilizer, native abilities and projectiles, save/load as well as almanac and language change.


## Change 1.8.4: Button geometry and click area in Fusion Almanac

The cloned native button template contains a zero-width serialized `Layout` container. Normally a `ContentSizeFitter` calculates the width based on the localized text. Merge View disabled this resizer without giving the inner container its own width. As a result, the background and click area had no usable width; the text was displayed very small and one below the other.

Fusion Switches and Side Buttons now share a common fixed button size setup. The inner container and native background fill the button area. The text is centered on one line with side spacing and uses 40 to 55 units of font size. The background is explicitly the raycast target; the text leaves clicks to the button. The existing click actions remain responsible. Native Animator components are only removed from your own copies; Unity color transitions inherit their selection and click states without overwriting fixed geometry. The original native buttons remain unchanged.

1.8.4 increases the fix number. Geometry documents are under `references/fusion-almanac/buttons-1.8.4/`, the release document under `references/release-1.8.4.json`. Dismantling with closed game with `backup/FusionMod-1.8.3.dll`. Build and static release testing do not replace play testing. Game and test programs will not start. The user checks background, readable single-line caption, text and margin click, switching to fusion almanac and back, as well as page buttons and controller navigation.

## Change 1.8.3: Missing fusion button

The existing game log from 1.8.2 shows the cause of the missing button: `Localizer.Instance` already triggers a `TypeLoadException` when loading the generated interop type. The generated `Localizer` inherits from `Singleton<Localizer, ILocalizer>`, but does not implement `ILocalizer` in the managed wrapper. This causes the voice query to fail before the almanac view and the “Fusions” button are built.

The voice query now calls the existing native getters `Singleton.get_Instance` and `Localizer.get_LanguageLabel` via Il2CppInterop. It does not load the incorrect managed type `Localizer`. Classes and methods are resolved by name, without fixed memory addresses; missing metadata and native exceptions remain errors. Only method metadata is cached, instance and language value are reread with each query. The comparison with the native key for `Language.German`, the English fallback and the update on language change are preserved. Gameplay, crops and profiles do not change.

1.8.3 increases the fixed number. Error statement and interop document are available under `references/fusion-almanac/visibility-1.8.3/`. Release document: `references/release-1.8.3.json`. Dismantling with closed game with `backup/FusionMod-1.8.2.dll`. Build and static release testing do not confirm any visible runtime representation. The game and all test programs will not start. The user checks the button in the plant dictionary, opening and closing the merger page as well as German, English and language switching.

## Change 1.8.2: Full English and German Fusion Almanac

All 137 merger entries now have separate English and fully translated German versions. Names, abilities, numerical values, accompanying texts, Replanted notes and all 185 active recipe alternatives appear in the selected version. The previously only English German plant names have also been translated. The English view does not adopt any German texts from the previous mixed version. The embedded display texts do not contain Chinese characters; The original Chinese documents remain exclusively in the source archive.

**German is only displayed if the game language is German.** Replanted's active localization is decisive: `Localizer.Instance.LanguageLabel` is compared with the key for `Language.German` provided by the game itself. For English and all other game languages, the Fusion Almanac uses English. Operating system and Steam language do not change this selection. A language change also updates an already constructed view; The selection, page and scroll position are retained when updating the texts. Your own page buttons, headings, cost information and recipe names also change. The explicitly requested entry button is still called “Fusions”.

The German version is under `references/fusion-almanac/translations.de.json`. It translates the current English source texts, including documented porting adjustments, and replaces the incomplete or numerically outdated German source version. For each entry, `EnglishSha256` binds the checked German translation to the name, description, accompanying text and notes from the English version. If this content changes, the export requires a new translation check. The release check checks both complete versions, localized ingredients, current recipes, missing translations and remaining Chinese texts. According to the workflow, new plants require both versions in the same release.

1.8.2 increases the fix number. Sources and static language evidence are available at `references/fusion-almanac/localization-1.8.2/`; Release and backup under `references/release-1.8.2.json`. Reset when game is closed via `backup/FusionMod-1.8.1.dll`. Starting the game and all test programs are left to the user. Check manually: English, German, backswitch and a game language other than English fallback; all four pages, long accompanying texts, ingredients and instructions; Closing and reopening as well as selection and scroll position when changing languages. Build and static testing do not confirm runtime representation.

## Change 1.8.1: Fusion almanac

In the Plant Almanac there is a new button **Fusions** next to “Lexicon Contents”. The page contains all **137 implemented fusion results**, including native results such as Repeater, Sun-shroom and Twin Sunflower. Eight columns with five rows and the “Back” and “Next” buttons distribute the entries over four pages. Clicking on a card shows the original portrait, description, values, accompanying text and all actually available recipe alternatives. The same button switches back to the normal plant view as “Plants”; the native buttons for lexicon content and close remain available.

Text and images come from PvZ Fusion 4.0.5.2. The English and German translations are committed to the same Git version as the existing recipe archive; the original Chinese local entries remain as evidence. Original German names and accompanying texts are adopted. The German translation is partly still English and partly contains older numerical values; therefore the behavioral descriptions come from the appropriate English version. This is not a completely new German translation. Replanted uses its native font, card frames, background graphics and detail view. The foreign fixed font sizes, alignments and color markings are removed; the texts receive Replanted's almanac colors and a scrollable detail area. The portraits are static original sprites, not recreated game animations.

Proven deviations are taken into account in the entry: including 1800 contact damage from the Giant Wall-nut, native Sun-shroom and Twin Sunflower production, new Nugget-shroom propagation after a successful cycle and 1.5 field radius of the attached Cherry-spreader bomb. Snow/Cherry click upgrades and unavailable Starry Pumpkin/Infernowood synergies that have not yet been implemented will not be offered as abilities. Cost and recharge time describe the incoming ingredient card and existing gauntlet rules. Locked alternative recipes do not appear as executable recipes.

The view only uses the embedded text and images. It does not register new native plant types, does not create plants and does not change unlocks or profiles. The existing native plant views are hidden during viewing and restored when switching back or closing. Mouse, keyboard and controllers use native Unity buttons, explicit navigation and the existing scroll path. No source archives or internet connections are required at runtime.

`workflow.md` now requires **every new plant** to have the appropriate almanac entry in the same release. `tools/almanac.py` creates the entries from the active recipes and the checked ingredient identities. The existing release check controls full result coverage, unique source IDs, active recipe alternatives, embedded original images and export checksums. Sources, adaptations and image identities are available at `references/fusion-almanac/`.

1.8.1 increases the fixed number for the previously missing almanac. Gameplay and crops will not receive any changes. Release and backup are documented under `references/release-1.8.1.json`; Dismantling when the game is closed via `backup/FusionMod-1.8.0.dll`. The game won't start. The user runs test programs and game tests. Check manually: position of the new button, all four pages including last card, names and images, long texts and all recipe alternatives, mouse/controller navigation, scrolling and switching between plants, fusion page and lexicon content, closing and reopening. Build and static source checking do not confirm visible game output.

## Change 1.8.0: Cherry spreader and proven porting workflow

`Cherry Bomb | Threepeater | Cherry-spreader` is fully ported and applies in both card orders and both glove directions. The plant has 300 HP and shoots into its own row and existing neighboring rows every 1.5 seconds. On the sidelines, the missing third shot follows after 0.2 seconds in your own row. Card cost and cooldown still come from the incoming ingredient; Documents, terrain and generation before ingredient consumption use the common fusion path.

The first half-cherry ball attaches exactly one bomb with 20 base damage to a living enemy target and does not deal an additional direct hit. Additional half-cherry bullets hit for 20 damage and charge the existing bomb by 5. During maturation, existing cherry bullets and cherry bombs increase bomb damage by a quarter of their incoming damage, truncated by an integer per hit. Uninvolved or nested other damage is not counted as cherry damage. The bomb follows its target and physically explodes after six seconds within the original 1.5 source unit radius; at board scale that's about 88.9 pixels. It preserves cold states and bodies, does not harm plants and does not leave a crater. Separate selection of flying targets applies to a flying carrier. If the carrier disappears or dies, the bomb is removed without an explosion. Torchwood does not convert half-cherry balls.

The new plant identity `Threepeater/Cherrybomb` as well as target handle, maturity time, charge and short charge animation are in native saved fields. There is no volatile bomb table. Pause stops maturation; Death, target loss and pool reuse are protected against old instances. New original models and animations for plant, flying projectile and attached TimeBomb are exported and embedded. Source identity, actual methods, numerical values, memory fields, callers and boundaries are available at `references/cherry-spreader/port-notes.md` and the documents linked there.

This port tests the optimized tools with a real new fusion: shared batch research, export only with prefab IDs, shared source caches, auto-completed project settings, auto-offered previews and co-generated documentation changes. Commands, actual runtimes and offline checks are available under `references/workflow-trial-cherry-spreader/`. Measured individual running times do not show any comparison with the old tool version. The completed recipe line is removed from `fusionen.txt`; the complete source archive remains unchanged.

Version 1.8.0 increases the middle spot for a new plant. Installation and backup are documented in `references/release-1.8.0.json`; Dismantling when the game is closed via `backup/FusionMod-1.7.3.dll`. Savegames with new plants or cherry identifiers require this or a compatible newer version. Game didn't start; Profiles, scores and LawnGlove unchanged. The offline test reconstructs saved fields against native doubles and is not an actual save/load or Unity playtest. Check manually: maps and glove in both directions, preview and marking, roof and water underlays, sizes and anchors, all three heads, attachment and charging by previous cherry families, six second ripening time, death and hypnosis of the wearer, pause and real save/load.


## Change 1.7.3: Restore visibility of puff fusions

The new size adjustment from 1.7.2 used mesh data, which also contains the extreme spine coordinates. The game log shows a target box of about 302,795,300 × 302,795,200 units and a scaling factor of about 382 million. As a result, the fusions were displayed far outside of their visible position. The synthetic geometry tests from 1.7.2 did not represent this actual mesh content.

The incorrect mesh measurement and the associated tests have been removed. The previously visible sprite path is restored. All imported Puff Fusions are uniformly increased in size by 50 percent from this path; Aspect ratio, ground anchor and horizontal center are retained. World graphics and preview use the same magnification, with the existing preview parent scale applied exactly once. The previously recorded target box of 100 × 99 creates 150 × 148.5 units. Native mesh coordinates are no longer included in the display. Abilities, projectiles, recipes and plant conditions remain unchanged.

20 offline checks in `tests/PuffArtworkSizingTests.csproj` check the previously logged size reference, zoomed target values, aspect ratio, world/preview scales and anchors. Release build, resource/recipe testing and workflow testing supplement the evidence. This does not confirm any visible game output. Game launch and play testing remain with the user; Check visibility, size next to a native puff shroom, all subfields, preview, glove and save/load. The exact visual correspondence with native puff-shrooms can still be assessed in the game.

Version 1.7.3 increases the fix number. Installation: `references/release-1.7.3.json`. Previous version backup: `backup/FusionMod-1.7.2.dll`; This previous version contains the known visibility error. The last visibly working older version is under `backup/FusionMod-1.7.1.dll`. Dismantling only when the game is closed. Profiles, saves, LawnGlove, assets and workflow files are not changed. Proof: `references/puff-size-1.7.3/verification.json`.


## Change 1.7.2: Actual size of puff fusions

**This change has been reverted due to the proven visibility issue in 1.7.3. The associated geometry tests and mesh helper are removed.**

The size fix from 1.6.1 only changed the aspect ratio limit. The world graphics continued to use the separately created preview sprite as a size reference. In the present game log from 1.7.1, this still results in a target box of 100 × 99 units for Pea-shroom and Repeat-shroom; the user confirms that the display is still too small.

All imported Puff-Fusions now measure their world size by the actual drawn triangles of the native Puff-shroom mesh. The measurement transforms the mesh points into the same local space as the import graphic and adopts native width, horizontal center and ground position. It does not use the oversized spine culling limits, unused mesh vertices, or a blanket magnification factor. This means that already effective parent scaling, roof and glove positions are not applied twice. If the native mesh geometry is still missing, the native graphics remain visible; the import is waiting for them to be built. The preview still uses its own native sprite size reference. Abilities, projectiles, recipes and plant identifiers remain unchanged.

`tests/NativePlantArtworkTests.csproj` uses productive measurement code and synthetic geometry to check native mesh size instead of preview size, oversized culling limits, unused mesh capacity, parent scaling, offset, mirroring, and not yet available or invalid mesh data. These offline checks do not measure a running character. Pass release build, resource/recipe testing, and existing workflow tests. Game launch and visual playtests remain with the user; especially size next to ordinary puff-shrooms, check all three subfields, roof, glove, preview and loading.

1.7.2 only increases the fixed number. Installation: `references/release-1.7.2.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.7.1.dll` to `Mods/FusionMod.dll`. LawnGlove, assets and both workflow files remain unchanged. Proof: `references/puff-size-1.7.2/verification.json`.


## Change 1.7.1: Own co-op selection and placement preview

- The controller correction for small plant fields now also applies to the controller player in co-op. Each controller uses its own pointer coordinates and its own recenter timer.
- Each placement preview uses its own player for subfield position, validation and card selection. Mouse player movements no longer move or block the controller player's puff preview.
- Planting clicks retain their player index. Tool hits use the player and coordinates of the actual call. The impersonator exception checks the card of the responsible player.

The game already has separate preview items per player; these are reused. Temporary player context connects native preview checks to common subfield selection and also resets after errors. The installed LawnGlove mod remains unchanged.

Evidence and limits: `references/coop-cursors-1.7.1/notes.md`. Release build and static consistency check; existing subfield test project only compiled. No test programs executed, game not started. The user performs game testing: move both previews at the same time, select different subfields of the same cell, release the controller stick, place and merge; Counter-check occupied subfields, shovel, card cancellation and single player.

Installation: `references/release-1.7.1.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.7.0.dll` to `Mods/FusionMod.dll`.


## Change 1.7.0: Four Sunflower fusions

```text
Blover | Sunflower | Solar Blover
Flower Pot | Sunflower | Solar Pot
Jalapeno | Sunflower | Solar Flare
Sea-shroom | Sunflower | Sunset-shroom
```

These are all four source recipes from the four results. Both ingredients are already available natively. All recipes apply in both card orders and glove directions. The incoming card determines the cost and cooldown. The median version number increases for four new plants. A total of 184 recipes are active; 484 recipes for 414 results remain open. The four completed lines are removed from `fusionen.txt`; `fusionen-quellen.json` remains unchanged.

- **Solar Pot:** 300 HP. Rootstock with 15 sun per production cycle; Basic interval 25 seconds, original deviation ±10 percent. After the brightening begins, the payout occurs half a second later. A normal resident remains intact when the underlying pot fuses. If the normal plant also has a suitable recipe, this takes precedence: a card only produces one result. Existing other documents will not be replaced. The native pot function and its terrain rules remain responsible.
- **Sunset Shroom:** 300 HP. Aquatic plant without shoots. Initially produces 15, after 20 active seconds 25 and after another 40 seconds 50 sun. Production interval 15 seconds with ±5 percent deviation and half a second brightening until payout. Both original growth clips and all three age representations are included. Sleep, coffee and water placement remain native. Fertilizer heals and shortens the current growth stage to a maximum of 0.5 active seconds after the feeding animation; he doesn't skip any steps. A second application accelerates the second stage. This behavior follows `Fertilize.Upgrade` and `SunSeaShroom.AttributeEvent` of the local source.
- **Solar Flare:** 300 HP. Explodes to the original event at 2/3 second, even if previously destroyed. Deals 1800 ash damage in your row, plus 500 for each plant you capture. Each enemy zombie instance hit generates 25 sun. Afterwards, your own plants in the same row each receive 1800 damage; 50 suns are created per plant. The source leaves exactly 1 HP with probability 1/3, provided that the health becomes strictly negative as a result of the hit. If the HP is exactly 0, this exception does not apply. Other ranks and hypnotized zombies are spared. Consuming ingredients and cleaning up a board do not trigger an explosion. **The ability deliberately destroys your own plants, including affected rootstocks and pumpkins.**
- **Solar Blover:** 300 HP. Original blowing event after around 0.49 seconds. Removes enemy balloon zombies and clears fog for 30 seconds. Then fire 72 small solar projectiles in two spirals at angles of 10 to 720 degrees, one per 0.02 seconds. Each projectile deals 10 damage and leaves behind a collectible pickup with 5 sun when hit by a zombie. No Torchwood conversion. After the last coroutine interval, the plant disappears.

Original graphics and animations come exclusively from the local 4.0.5.2 bundle. Native particles and sounds replace source particles. All five models, 13 clips and 27 sprites are exported and embedded; the offline previews are visually controlled. Source descriptions, data values, recipes, script identities and checksums are under `references/solar-pot/`, `sunset-shroom/`, `solar-flare/`, `solar-blover/` and `blover-sun-projectile/`. Behavioral paths, native storage, terrain queries and boundaries explained in `references/solar-fusions-native/port-notes.md`.

New memory pairs are Flowerpot/Sunflower, Seashroom/Sunflower, Jalapeno/Sunflower and Blover/Sunflower. Production, growth and firing phases as well as projectile identification are in native saved fields. Loading reconstructs identity, phase and representation. The existing sun aid only temporarily saves the special value of 5-sun pickups that have already been paid out: such a pickup that is loaded before collection falls back to the native SmallSun value. The regular 15/25/50 Sun pickups remain natively saved. Savegames with the new variants require this or a compatible newer version.

Checked offline: productive skills, fertilizer completion and DataID protection, production values, both growth stages, rest and sleep, 72 shots, hits, friendly fire, death, saved intermediate phases, card/glove paths and document preservation and atomic generation. Recipe/asset consistency, workflow checks, affected regressions and release build are documented separately. Game didn't start; Profiles and saves not changed. Both workflow files remain unchanged. Installation: `references/release-1.7.0.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.6.2.dll` to `Mods/FusionMod.dll`.

Check manually: all four recipes in both orders with cards and glove, documents, occupied pots, preview, marking, costs and cooldown; water, roof, day/night and coffee; Sun values, growth and fertilizing twice; Solar Flares friendly fire and bonus; Balloons, nebula, and all 72 Solar Blover projectiles. Check pause, carry, bungee, death and save/load during production, growth, feeding, spiral and projectile flight.

## Change 1.6.2: Controller cursor and subfield selection

- Fertilizer removes the tool cursor after successful use even with controller. Record, Cancel, Save, and Cleanup also use native cursor deletion regardless of control type. If the targets are invalid, the bag remains available.
- There is no automatic re-centering of the controller pointer in small plant fields. The position selected using the stick is retained for the glove, shovel and placement. Outside these fields, native recentering remains active; D-pad navigation remains unchanged.
- The shovel breaks up overlapping small plants based on the selected subfield. Empty subfields do not select neighboring small plants. Native priority of pumpkin and rootstocks is preserved.

Native evidence and technical limits: `references/controller-fixes-1.6.2/notes.md`. Release build and static consistency check; no test programs executed. Game didn't start. The user carries out play tests: select all three subfields with the stick, release, use glove/shovel, plant free subfields and use fertilizer and cancel.

Installation: `references/release-1.6.2.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.6.1.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged.


## Change 1.6.1: Projectiles, sizes, sun-shroom and fertilizer

- Peashooter Mine (Peatato Mine) now retains the original potato bullet graphics. The import renderer's validation assigns these bullets to their own projectile family; previously it fell back on the Cherry Shooter test and removed the graphics immediately. Normal shots and all 36 radial killshots use the same original asset from 4.0.5.2. Damage, speed and collision remain with the existing potato projectile path.
- Giant Wall-Nut uses size 1.5 instead of 1.8 and a matching 120 pixel contact box. The ground animation runs at 5.4 instead of 12 frames per second, even with an animation already running or loaded. Motion normalizes size and animation rate separately; Floating point movement avoids standstill and jumps by cutting off small steps.
- Puff-shroom + Sunflower only creates the native Replanted Sun-shroom. The additional porting, sun production and embedded sun shroom assets are removed. Native skills, coffee, growth, fertilizer and representation are responsible. The old storage pair Puffshroom/Sunflower remains reserved for the changeover and will not be reassigned. Before controller generation, old ported plants are converted to native Sun-shrooms; Health, position, ripening time and production distance are maintained. The old ported whitening phase is ended. The information about the separate porting in the historical section 1.5.0 no longer applies.
- A fertilizer bag heals all living plants of the same cell: all three mushroom subfields, pumpkin, rootstock, and other layers. The actual plant list replaces the shift query limited to a normal resident. Each viable plant receives its own saved job with DataID in the serialized tool counter. All young native Sun-shrooms grow after fertilization animation ends; Removed, moved or carried plants do not receive foreign growth. A replacement plant that has now been used does not take over the old order. Before healing, all orders are reserved; If allocation errors occur, plants remain unchanged. Fertilizer jobs saved before 1.6.1 continue to use their previous resolution.
- Puff fusions are scaled based on the native puff shroom width. The previous additional height limit shrunk tall fusions below the native width. Aspect ratio, ground anchors and existing preview scaling are retained.

No new plants or recipes will be added. 1.6.1 only increments the latest version number for bug fixes. Original sources and assets continue to come from the local `english-4.0.5.2`; archived Sun-shroom sources remain as evidence. `fusionen.txt`, `fusionen-quellen.json`, `workflow.md` and `worklfow.md` remain unchanged. The existing workflow already covers these corrections.

Offline Evidence: `tests/TileFixesTests.csproj` checks complete field fertilization, three stored growth orders, animation, replacement targets, movement, glove, allocation error, pumpkin preservation, mine arming, old stand conversion and walnut movement. Existing puff, subfield and mine regressions as well as resource/recipe checking, workflow checks and release build complement the proof. Native receipts and boundaries are available at `references/fixes-1.6.1/`. No game launch and no playtests; Profiles and scores are not edited.

Check manually: Potato bullets forward and after mine death, size of puff fusions next to common puff-shrooms including preview and gauntlet, size and rotation of the giant wall-nut, sun-shroom recipe in both directions and existing older sun-shrooms after loading. Apply fertilizer to three young sun-shrooms, mixed mushrooms, pumpkin with plant and mines; Check healing, growth only after animation, pause and save/load during fertilization. Actual visual match and controller recovery of old Sun-shrooms need this game check.

Installation: `references/release-1.6.1.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.6.0.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged. A Sun-shroom saved after conversion is a common native plant.


## Change 1.6.0: Buck-shroom, Sprout-pult, Baby Corn and Slice-pult

```text
Pea-shroom | Bucket | Buck-shroom
Puff-shroom | Buckshooter | Buck-shroom
Puff-shroom | Cabbage-pult | Sprout-pult
Puff-shroom | Kernel-pult | Baby Corn
Puff-shroom | Melon-pult | Slice-pult
```

All ingredients were already fully available. These five recipes are all source recipes for the four results. The common recipe resolution supports both ingredient orders, cards and glove, one to three matching puff subfields, terrain, underlays, marking and preview. The results occur before the ingredients are consumed; The cost and cooldown remain with the incoming ingredient. The five completed lines are removed from `fusionen.txt`. A total of 180 recipes are active; 488 recipes for 418 results remain open. Subsequent fusions and `fusionen-quellen.json` remain unchanged.

- **Buck Shroom:** 1000 HP. Shoots small original iron projectiles across the line every 2.25 seconds. Each hit deals 80 damage in addition to current shield health and knocks back eligible enemies via the iron adapter provided. Heavy enemies retain its knockback immunity. The plant counts as an iron plant in the existing bucket path: Another bucket heals up to 500 HP, at most up to the previous maximum health. Both source recipes have been implemented. The iron projectiles are not ignited by Torchwood.
- **Sprout Pult:** 300 HP. Throws small cabbages every three seconds for 40 direct damage. The projectiles can ricochet twice, hitting a total of three different consecutive targets. Horizontal speed after a hit: 40 percent; reflected vertical speed: 50 percent. Contact with the ground ends the flight. If a cabbage hits a shrunken zombie, this projectile receives the shrinking effect. Subsequent hits shrink eligible targets once to 70 percent size and current/maximum body, helmet, and shield health before damage. Without prior contact with a shrunken opponent, the cabbage has no shrinking effect.
- **Baby Corn:** 300 HP. Throws small kernels of corn for 20 damage every three seconds, with a 25 percent chance of throwing small butter for 40 damage instead. First, look for the next non-shrunk ground zombie in front of you in the same row; if none is present, the next shrunken enemy. Small Butter shrinks a suitable unshrunk target once. For already shrunken targets, it instead immobilizes for 1.5 seconds; Butter that has been present for a longer period of time is retained. Little Butter Kills Imps. Native butter immunity and the existing boss immunity to shrink remain.
- **Slice desk:** 300 HP. Throws original melon pieces every three seconds for 80 direct damage, against shrunken enemies 160. These pieces do not deal area damage.

All attack intervals use five percent random variation. The throwing plants seek ground targets forward in the same row across the full row. The original event of the three throwing clips is 0.616667 seconds, for the buck-shroom it is 0.383333 seconds. Baby Corn saves the butter decision at the beginning of the associated original clip. Native throwing motion and collision represent the source flight of 1.5 seconds. Direct damage thrown bypasses hand shields; Body, helmet, death and loot rules remain with the native health writer. Sleep/Coffee, Break, Glove, Bungee, and Death use the existing lifecycle rules. Original animations do not produce second shots.

Original prefabs: IronPuffPrefab 57323, CabbagePuffPrefab 33774, CornPuffPrefab 33773, and MelonPuffPrefab 33775. All nine plant clips and five actual original projectile prefabs are exported. Buck-shroom specifically uses Bullet_puffIronPea 34018 with the Bullet_ironPea script, not the similarly named AA type Bullet_ironPea_air. Corn, Butter, Cabbage and Melon use 75723, 42997, 48867 and 72616. Original sprites, pivots, hierarchy, shadows and controllers are preserved; the existing import renderer uses native puff size references. Projectiles remain attached to the native movement anchor and their shadows remain on the ground. Native hit particles replace source particles. No foreign Unity game scripts are loaded and no unknown animation bindings are missed.

New storage pairs: Puffshroom/Magnetshroom, Puffshroom/Cabbagepult, Puffshroom/Kernelpult and Puffshroom/Melonpult. The throwing plants store distance, clip phase and butter choice in `mLaunchCounter`, `mStartRow` and `mShootingCounter`. The projectiles store family and with Kohl also number of hits and shrinkage effect in `mCobTargetX`; `mTargetZombieID` prevents the same target from being hit again immediately. Native velocity and acceleration fields maintain trajectory when loading. The existing stored size value identifies shrunken zombies. There is no new fleeting ability state. Save games with these variants require this or a compatible newer version.

Source descriptions, all recipes, methods, data values, script identities, asset checksums and previews: `references/buck-shroom/`, `sprout-pult/`, `baby-corn/`, `slice-pult/`, `buck-shroom-projectile/`, `puff-lob-source/`, `puff-lob-base/` and `puff-lob-native/`. The shared behavior and storage document is in `references/puff-lob-native/port-notes.md`.

Checked offline: productive skills, full recipes in both orders, original event phases, protected timers, saved butter choice, native flight parameters, cabbage ricochet and shrink pass, 20/40/80/160 direct damage, missing melon area damage, imp special case, bucket heal, shield bonus and Torchwood exception. Existing Puff, Subfield, Bucket, Football, Scaredy and Squash regressions as well as recipe/asset consistency, workflow checks and release build are checked. Original animation previews are controlled offline. This evidence does not confirm playtesting. The game didn't start; Profiles and scores remain unchanged. Version 1.6.0 increases the middle number for new plants. Installation: `references/release-1.6.0.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.5.0.dll` to `Mods/FusionMod.dll`. LawnGlove and both workflow files remain unchanged.

Check manually by the user: all five recipes with both ingredient orders, cards and glove, one to three and mixed subfields, documents, terrain including roof/pool, cost, cooldown, preview, mark and cancel. Buck-shrooms 1000 HP, check shield bonus, knockback and bucket healing. Three types of throws, small butter including Imp special case, cabbage ricochet and shrink pass as well as melon pieces without area damage. Sleep/Coffee, Pause, Speed, Gauntlet, Bungee, Death and Save/Load before and after the shooting event, cross-check during Butterclip, Flight, Bounce and after Shrinking.


## Change 1.5.0: Sun-shroom, Gnome-shroom and Umbrella Cap

```text
Puff-shroom | Sunflower | Sun-shroom
Puff-shroom | Hypno-shroom | Gnome-shroom
Puff-shroom | Umbrella Leaf | Umbrella Cap
```

All three results each have exactly this one source recipe. Both ingredients are fully available. Common recipe resolution supports both card orders and glove directions, one to three puff subfields, atomic generation before ingredient consumption, terrain, underlays, preview and highlighting. Cost and cooldown come from the incoming ingredient. The three completed lines are removed from `fusionen.txt`. A total of 175 recipes are active; 493 recipes for 422 results remain open. Subsequent fusions and `fusionen-quellen.json` remain unchanged.

- **Sun Shroom:** 300 HP. Produces 15 suns every 25 seconds with five percent random deviation, then 25 suns after 120 active seconds. The original lightening begins with the production cycle; the payout occurs half a second later. Original growth animation and both age representations are included. Native SmallSun/Sun pickups preserve value, collection and storage.
- **Gnome-shroom:** 300 HP, no shots. A bite into the awake mushroom hypnotizes the attacking zombie and consumes the mushroom. The zombie shrinks once to 70 percent of its previous size. Current and maximum body, helmet and shield health are each multiplied by 0.7 and rounded down. Zombies that have already shrunk will not shrink again. Native special cases without plant bite or hypnosis remain excluded.
- **Umbrella Cap:** 300 HP. Multiplies every 30 active seconds in the first free subfield of the same cell. Even after the field is full, the parent timer is started again. Close ground contact in the same row starts the original half-second block clip. At the original event at one-sixth of a second, eligible enemies on the same row within a source unit radius are pushed back two source units without damage. An unshrunken enemy consumes the mushroom. If only shrunken enemies are present, the mushroom loses 50 HP instead. The actual method `SmallUmbrella.AnimBlock` requires this 50 HP even if all enemies have left the radius by the time of the event. This small version does not protect neighboring plants from catapults or bungees.

All abilities use native simulation sticks; Sleep, rest and carrying stop them. The three original types do not produce projectiles. Native puff spores are suppressed for these variants. The original prefabs SunShroom 50766 / Transform 116220 / Controller 6495, HypnoPuff 56884 / 95285 / 6605 and SmallUmbrella 53646 / 90910 / 6554 come exclusively from the local 4.0.5.2 bundle. All 15 sprites and six clips are included, with no missed bindings. Shadows, hierarchy, pivots, puff size reference and marker use the existing renderer. Solar pickups, hypnosis and recoil use native Replanted adapters; Unity game scripts not loading.

Memory pairs: Puffshroom/Sunflower, Puffshroom/Hypnoshroom and Puffshroom/Umbrella. Production, lightening, age, growth clip, propagation and block phase lie in the existing saved plant fields. Shrink uses the saved native `Zombie.mScaleZombie` as well as the six health fields. The three native output sizes 1, 0.8 and 0.5 result in clearly recognizable shrinkage sizes 0.7, 0.56 and 0.35. Loading reconstructs appearance and condition without re-shrinking or additional sun. Third-party mods with other zombie sizes are not tested. Save games with these variants require this or a compatible newer version.

Original descriptions, full recipes, actual methods, raw data, asset checksums and previews are at `references/sun-shroom/`, `gnome-shroom/` and `umbrella-cap/`. Call, save and port documents: `references/puff-support-1-5/port-notes.md`.

Passed offline: 155 skill checks and 423 subfield checks, including both recipe orders, glove directions, one to three subfields, growth, production timing, saved intermediate phases, shrinking of all six health slots, recoil, mixed enemy sizes, and multiply at full field. Additionally passed: 91 Puff special tests, 195 Puff marksman tests, 139 Scaredy tests and 13 Workflow tests. Original animation previews are visually controlled; Recipe/asset consistency and release build without warnings or errors are checked. These checks do not validate playtesting. The game won't start; Profiles and scores remain unchanged. Version 1.5.0 increases the middle number for new plants. Installation: `references/release-1.5.0.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.4.0.dll` to `Mods/FusionMod.dll`. LawnGlove and both workflow files remain unchanged.

Check manually by the user: all three recipes with cards and glove in both directions, partially occupied and mixed subfields, terrain, documents, costs, cooldown, preview and marking. Sun-shrooms 15/25 Check sun, growth and payout; Check Gnome's bite hypnosis, height and armor; Umbrella Caps Check proliferation, knockback, lack of damage, consumption on large enemies and 50 HP on small enemies. Cross-check sleep/coffee, pause, speed, death and save/load during growth, lightening, block phase and after shrinking.


## Change 1.4.0: Hail-shroom, Icicle-shroom, Twinkle-shroom and Nugget-shroom

```text
Hail-shroom | Peashooter | Icicle-shroom
Pea-shroom | Ice-shroom | Icicle-shroom
Puff-shroom | Ice-shroom | Hail-shroom
Puff-shroom | Potato Mine | Nugget-shroom
Puff-shroom | Snow Pea | Icicle-shroom
Puff-shroom | Starfruit | Twinkle-shroom
```

All four results are ported with ingredients already available. Hail-shroom becomes available first; This means that all three source recipes for Icicle-shroom are also permitted. All six recipes use the common resolution for both card orders and glove directions. Matching puff subfields fuse together; Subfields, mismatched plants, water lilies, pots and pumpkins remain. The cost and cooldown are determined by the incoming ingredient. Results occur before ingredients are consumed. The six completed lines are removed from `fusionen.txt`. A total of 172 recipes are active; 496 recipes for 425 results remain open. Subsequent fusions and `fusionen-quellen.json` remain unchanged.

- **Hail Shroom:** 300 HP. A nearby enemy ground zombie on the same row applies four seconds of freeze followed by ten seconds of cold. Contacts that have already frozen will not trigger again. The plant disappears after the effect. No damage and no projectile; native frost immunity remains responsible.
- **Icicle Shroom:** 300 HP. Search forward in the same row to 2.5 source units, approximately 1.85 squares. Shoots original small ice projectiles with 20 damage, cold and ten frost points at a base interval of 2.25 seconds. A frozen or cold-immune target takes quadruple damage. The actual local method `Bullet_snowPea.HitZombie` adds ten frost points and removes the projectile after the first hit. The almanac statements “five frost points” and “penetrate twice” do not correspond to this delivered implementation; the port follows the actual code. The independent random upgrade through an additional source ability is not part of the base plant.
- **Twinkle Shroom:** 300 HP. Native star homing activates five original stars every 2.25 seconds, each dealing 20 damage. Directions: 30, -30, 180, 90 and -90 degrees. The two oblique and two vertical stars fly at five source units per second, the backward star at six. Original shot event at 0.25 seconds; Row changes, armor and collision remain native.
- **Nugget Shroom:** 300 HP. Shoots original potato spores for 20 damage at the actual base interval of two seconds. The target range is also 2.5 source units. After about 20 active seconds, another nugget shroom with its own timer is created in the first free subfield of the same cell. The actual method `PotatoPuff.AttributeEvent` also starts the parent timer again after successful propagation. If the placement fails, for example when three subfields are occupied, this timer stops permanently. This is different from the almanac formulation “once per plant.” Ordinary Death deals 300 Potato Blast damage to eligible ground targets on the same row within a source unit. Ingredient consumption, failed fusion, and board removal do not trigger an explosion. No crater; Potato spores do not cause an additional explosion.

All firing intervals and the propagation timer retain the original random deviation of five percent. Icicle and Nugget projectiles travel at six source units per second; 40 board pixels flight altitude keep them above the native ground hit limit. Regular Torchwood does not convert both types of bullets. Sleep, break, glove and bungee stop plant abilities. There are no additional fusion bonuses or second shots from animation events.

Original sprites and all seven plant clips are from the local 4.0.5.2 bundle: SmallIceShroomPrefab 56882 / Transform 95283 / Controller 6631; IcePuffPrefab 42855 / 98233 / 6457; StarPuffPrefab 42857 / 98235 / 6460; PotatoPuffPrefab 42859 / 98237 / 6454. The new original bullet Bullet_snowPuff 73650 / 95613 actually has the script `Bullet_snowPea`. Nugget uses the existing identical export Bullet_puffPotato 73652; Twinkle uses Bullet_star 33973. Hierarchy, shadows, pivots, animations and the native puff size reference are preserved. All bindings are exported; no new exception is introduced. Native frost, hit and explosion particles and audio replace corresponding source components. Unity game scripts not loading.

Memory pairs: Puffshroom/Iceshroom, Puffshroom/Snowpea, Puffshroom/Starfruit and Puffshroom/Potatomine. `mLaunchCounter` stores the shot distance, `mStartRow` the clip phase and `mTargetX` the nugget propagation timer including the stopped state zero. Descendants are created with their result identifier, regardless of cursor and player fusion context. Store small ice bullets or potato spores -470018/-470092 in `Projectile.mCobTargetX`; Movement, damage, and visuals are retained after loading. The shared existing SnowPea management for untriggered frost points and increasing frost thresholds is still volatile and resets upon loading. Active frost and cold stores natively. Savegames with these variants require this or a compatible newer version.

Sources, unmodified original descriptions, complete recipes, actual methods, original data, asset checksums and previews are in `references/hail-shroom/`, `icicle-shroom/`, `twinkle-shroom/` and `nugget-shroom/`. Detailed behavior and invocation document: `references/puff-special-native/port-notes.md`.

Checked offline: productive skills, six recipes in both orders, card and gauntlet fusions, one to three subfields, propagation without cursor dependency, full fields, failed generation, saved timers, death, dismantling, and existing puff/scaredy/star/cold regressions. Original animation previews, recipe/asset consistency, workflow checks and release build are checked. This does not confirm playtesting. The game didn't start; Profiles and scores remain unchanged. Version 1.4.0 increases the middle number for new plants. Installation: `references/release-1.4.0.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.3.0.dll` to `Mods/FusionMod.dll`. LawnGlove and both workflow files remain unchanged.

Check manually by the user: all six recipes with card and glove in both directions; mixed and partially occupied subfields, costs, cooldown, terrain, documents, cancellation, preview and highlighting. Hail contact freeze, cold and immunity; Icicle range, 20/80 damage, ten points of frost and kills on first hit; Twinkle's five directions and distant destinations; Nuggets 20 second proliferation, free subfields, reactivated parent timer, stop at full field, and 300 death damage. Cross-check sleep/coffee, rest, speed, gauntlet, bungee, death and save/load during shot clips, multiplication and projectile flight.


## Change 1.3.0: Four more Puff-shroom fusions

```text
Puff-shroom | Wall-nut | Nut-shroom
Puff-shroom | Jalapeno | Bell Pepper
Puff-shroom | Doom-shroom | Boom-shroom
Puff-shroom | Marigold | Penny-shroom
```

All four results are ported with ingredients already available. These four recipes are all source recipes for these results. Both map orders and glove directions use the existing atomic generation, ingredient consumption, terrain, preview and marker paths. An incoming ingredient fuses all matching Puff-shrooms in the field; their subfields and unsuitable plants are retained. Card cost and cooldown remain with the incoming ingredient. A total of 166 recipes are active; 502 open recipes for 429 results remain. The four completed lines are removed from `fusionen.txt`; the source archive remains unchanged.

- **Nut-shroom:** 2000 HP, low edible blocker with no shooting ability.
- **Bell Pepper:** 300 HP. An enemy ground zombie within a source unit in the same row will start the original one-second explosion animation. The plant then disappears, dealing 600 Ash damage to its row via the common fire row adapter, including removing cold effects. Crushing triggers instantly; Ordinary death alone does not trigger an additional explosion.
- **Boom Shroom:** 300 HP. Contact with an enemy ground zombie or common death triggers once. Each enemy ground zombie on the same row within a source unit, approximately 0.74 squares, takes 1800 Ash damage. No crater. The actual method `DoomPuff.Explode` is decisive: The loop call is virtual slot 29, `Zombie.Charred`. It causes one hit per target, not an area explosion multiplied by the number of enemies. The almanac formulation about repeated explosions is correspondingly imprecise.
- **Penny Shroom:** 300 HP. Produces 100 money at the base interval of 25 seconds with five percent random variation; after 120 active seconds 300 money. Original brightening and half-second growth animation are included. Production occurs 0.5 seconds after lightening begins. Two or six ordinary gold coins represent the source values ​​100/300; whose native value and collection survive save/load without volatile special allocation.

These four source types do not produce projectiles. Native puff spores and their shot counters are suppressed for these variants only. The four original prefabs, eleven sprites and seven original clips are from the local 4.0.5.2 bundle. All bindings are fully exported; there are no missed tracks. Size, shadows, pivots, hierarchy, and existing highlighting use the existing import renderer. Native particles, audio, fire row display, and coins replace corresponding source effects. No foreign Unity game scripts are loaded.

The reserved memory pairs are Puffshroom/Wallnut, Puffshroom/Jalapeno, Puffshroom/Doomshroom, and Puffshroom/Marigold. `mStartRow` stores the Bell Pepper ignition phase; Penny-shroom uses `mLaunchCounter` for production, `mShootingCounter` for lightening and delayed payout, `mTargetX` for maturity and `mTargetY` for growth clip. All states use native simulation sticks and saved plant fields. Rest, sleep and carrying stop abilities. Ingredient consumption, deconstruction and board end do not trigger a death explosion. Loading reconstructs identity and appearance without fusion bonuses. Savegames with new variants require this or a compatible newer version.

Sources, data values, original descriptions, full recipes, methods and asset checksums: `references/nut-shroom/`, `bell-pepper/`, `boom-shroom/`, `penny-shroom/`, `puff-effects/` and `puff-zombie-source/`. Full behavior and storage receipt: `references/puff-support/port-notes.md`.

Checked offline: productive skills, both recipe orders, card and glove fusions on one to three subfields, growth, payout, saved timers, target limits, death, dismantling, pool reuse and existing puff/scaredy regressions. Original animation previews visually inspected; Recipe/asset consistency, workflow checks and release build checked. These are not confirmed playtests. Game didn't start; Profiles and saves unchanged. Version 1.3.0 increases the middle number for new plants. Installation: `references/release-1.3.0.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.2.1.dll` to `Mods/FusionMod.dll`. LawnGlove, `workflow.md` and `worklfow.md` remain unchanged.

Check manually: all four recipes with cards and glove in both directions; one, two, three and mixed puff subfields; Cost, cooldown, cancel, terrain, pot/waterlily/pumpkin, preview and highlighting. Check Nut-shroom with 2000 HP and no shots. Check Bell Pepper for ground proximity, ignition animation, 600 row damage, cold removal, normal death and crush. Boom-shroom check for close contact, 1800 damage per target, multiple targets, death and missing crater. Penny-shroom to 100/300 money, growth after 120 active seconds and check coin collection. Sleep/coffee, break, game speed, glove, save/load during ignition phase, growth, lightening and cross-check before coin collection.


## Change 1.2.1: Puff group fusion, pea flight and size

Puff-shrooms and their four shooter variants now fuse all matching mushrooms from the same field, even if only one or two subfields are occupied. The previous selection restriction to a subfield caused the reported error. A card or glove ingredient is used once; Unsuitable plants are retained. Nuclear generation and dismantling in the event of errors remain responsible. Other small plants retain their previous subfield selection.

The small peas were only twelve pixels away from the simulated ground height. The actual native motion method produces a ground hit immediately for peas under 28 pixels. All four puff shooters now use 40 pixels altitude. Muzzle position, original projectile, 20 damage, shot sequences, rebound, native collision and save are retained.

The display now follows the actual sprite boundaries and preview scaling of a normal native puff-shroom. All visible original sprites of the merger determine their initial size; the uniform scaling maintains the aspect ratio. A foreign plant at the cursor does not distort the measurement. The fixed normalization against a 40-pixel shadow is no longer necessary.

Evidence, screenshot, native methods and limits: `references/puff-fixes-1.2.1/port-notes.md`. Passed offline: 195 puff checks including first flight ticks and removed hits, 171 subfield checks including partial card/glove fusions and deconstruction, 139 scary regressions, 13 workflow checks, recipe/asset consistency and release build with no warnings or errors. These are not confirmed playtests.

The user closed their current game session before installing. Codex didn't start the game; Profiles and scores remain unchanged. Version 1.2.1 increases the final number for fixes. Installation: `references/release-1.2.1.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.2.0.dll` to `Mods/FusionMod.dll`. Recipe list, source archive, LawnGlove and both workflow files remain unchanged.

Check manually: one, two and three Puff-shrooms per field, card and gauntlet fusion, mixed variants, size next to regular Puff-shroom and both previews. Cross-check all firing sequences, distant targets, split rebound, armor, pause, terrain, and save/load.


## Change 1.2.0: Celebrates the Puff-shroom Fusion

Pea-shroom, Repeat-shroom, Split Pea-shroom and Gatling Pea-shroom are fully ported. All ten source recipes are registered, including step-by-step alternatives from Pea-shroom and Repeat-shroom. Both card orders and glove directions use the existing common recipe resolution. The ten completed lines are removed from `fusionen.txt`. A total of 162 recipes are active; 506 open recipes for 433 results remain.

All four plants have 300 HP and shoot small peas for 20 damage each. The base interval is 2.25 seconds with the original random deviation of five percent. Pea-shroom shoots once, Repeat-shroom twice, Gatling Pea-shroom four times. Split Pea-shroom shoots forward once and backward twice; the rear projectiles bounce back once at the left edge and become larger according to the source. The target search extends across your own row; Split Pea-shroom also reacts to targets behind it. Torchwood does not modify these projectiles. All skills, original bullets and shooting animations are included. There are no additional immediate fusion bonuses.

The original plants and Bullet_puffPea are exclusively from the local 4.0.5.2 bundle. The four controllers, all eight clips and all sprites are embedded. Two demonstrably orphaned visibility traces of the Pea shot clip are precisely omitted. Native hit particles and audio replace the corresponding source effects. The small plants continue to use three subfields per cell; the world graphics receive a 40 board pixel footprint. Native sleep/coffee, terrain, lifecycle, armor and storage rules remain in charge.

Memory pairs: Puffshroom/Peashooter, Puffshroom/Repeater, Puffshroom/Splitpea and Puffshroom/Gatlingpea. Cooldown and shooting phase are in `mLaunchCounter` and `mStartRow`. Projectiles store their identifier -460010/-460017/-460018 in `mCobTargetX`, which is unused in Peas; Native position and speed preserve flight direction and rebound state. The representation is reconstructed after loading. Savegames with these variants require this or a compatible newer version.

Sources, full recipe lists, descriptions, original data, asset checksums, shot events, previews and port limits are available at `references/pea-shroom/`, `repeat-shroom/`, `split-pea-shroom/`, `gatling-pea-shroom/`, `puff-projectile/`, `puff-shooter-base/` and `puff-movement/`. Detailed behavior and storage receipt: `references/pea-shroom/port-notes.md`.

Tested offline: 146 new production plant, recipe, projectile and rebound code checks, 128 subfield checks including published three-way merges, scaredy and squash regressions, workflow checks, recipe/asset consistency and release build. The four original animation previews were visually inspected. No game starts and no changes to profiles or savegames. Installation: `references/release-1.2.0.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.1.0.dll` to `Mods/FusionMod.dll`. LawnGlove, `fusionen-quellen.json`, `workflow.md` and `worklfow.md` remain unchanged. The average version number increases because of new plants.

Manual user checking: all ten recipes with card and glove in both directions; singles, triples and mixed fields; Cost, cooldown, cancellation, terrain, documents, sleep/coffee and marking. Shot counts 1/2/3/4, rear split targets, two rebound bullets, their one-time size change, 20 damage and check Torchwood. Cross-check pause, game speed, death, pool reuse and save/load during shot sequence and rebound.


## Change 1.1.0: Football Helmet, Football-Nut and Football Gatling

```text
Tall-nut | Football Helmet | Football-nut
Gatling Pea | Football Helmet | Football Gatling
```

Football zombies have a 50 percent chance of losing an original football helmet if they lose their helmet due to normal damage. The dice are rolled exactly once for each loss of helmet. Instant kills, ash damage, already helmetless zombies, friendly zombies, and magnet extraction do not produce an additional drop. A common fatal hit may leave a helmet; Instant kill paths remain excluded. The existing bucket probability and its exclusion of fatal hits remain unchanged.

One click picks up the loose helmet. Another click plants it as a low, immobile plant with 4000 HP or fuses it with Tall-nut or Gatling Pea. FootballHelmet1/2/3 collected from the magnet shroom can also be clicked on and used. Abort, invalid terrain, and failed creation keep the item. Loose helmets use the existing simulated item movement, flashing after four seconds and disappearing after eight active seconds; selected helmets do not expire. The gauntlet can move or fuse planted helmets. Both recipe sequences are registered. Costs, cooldowns, highlighting, previews and documentation remain shared responsibilities.

Football-Nut has 16000 HP and the native high aim and jump block of a Tall-nut. Another Football Helmet heals 5000 HP to the maximum, according to the actual method `Helmet.Use` and its constant read from the source DLL. Football Gatling has 3000 HP and shoots four staggered bullets in the native Gatling cycle with 200 ticks base interval including native random shortening. The first eleven bullets each deal 80 damage plus the current HP of the second piece of armor, with the existing iron recoil around 16 board pixels. Every twelfth ball actually produced is the original red football ball. According to the actual `Bullet_helmetPea.HitZombie` implementation, this only deals 80 direct damage, with no additional iron shield damage or iron knockback. It sends enemy football zombies fleeing from damage. Escaping football zombies remain hostile, turn around, run to the right, do not eat plants and disappear along the right edge without loot. The source only doubles speed below three source units per second; This limit is converted to board pixels and simulation sticks. Pause, cold, butter, freezing, death and hypnosis are still taken into account. Torchwood does not change either type of bullet.

All original plant sprites, damaged bodies, controllers and animations, as well as the red ball are from the local 4.0.5.2 bundle. The normal iron ball is reused from the existing original export. Plant Prefabs: HelmetPlantPrefab 40918 / Transform 116192 / Controller 6196; TallNutFootballPrefab 49272 / 106170 / 6497; HelmetGatlingPrefab 38417 / 107621 / 6294. Red bullet: Bullet_helmetPea 34022 / 117795. The two orphaned visibility traces of the Gatling shot clip correspond to the already documented Gatling hypno case and are precisely omitted. Native effects replace the original hit particles. No Unity game scripts are loaded. As with existing iron plants, there is no additional general blast immunity: in Replanted, your own Cherry Bomb explosions do not damage plants; Jack-in-the-Box and Crushing retain their own rules.

Memory identifiers: Potatomine/GoldMagnet, Tallnut/GoldMagnet and Gatlingpea/GoldMagnet. Identity and HP save natively. Gatling uses the unused stored `mTargetZombieID` for this type as a counter 0-11; native Gatling homing passes its target separately to `Fire`. Flying balls store their football iron ID -450011 and -450168 respectively in `mCobTargetX`; Appearance and abilities are reconstructed after loading. Escape uses the football unused stored `mSummonCounter` with -450168 as well as the native speed. Loose floor drops and cursor selection, like existing buckets, remain fleeting and disappear when the board is reloaded. Planted helmets and native magnet items remain saved. Savegames with these plants require this or a compatible newer mod version.

Sources, almanac descriptions, methods, original assets and static call receipts are located at `references/football-helmet/`, `football-nut/`, `football-gatling/`, `football-projectiles/`, `football-base/` and `football-native/`. The mod now contains 152 active recipes; 516 open recipes for 437 results remain. The two completed lines are removed from `fusionen.txt`. Football-shroom and Buckshot Commando remain open. `fusionen-quellen.json`, `workflow.md` and `worklfow.md` remain unchanged. Version 1.1.0 increases the middle number for new plants.

Tested offline: 70 new production helmet, recipe, projectile and escape code checks, 75 bucket regressions, 117 subfield checks, 13 workflow checks, recipe/asset consistency and release build. The three original animation previews were visually inspected. No game start, no playtests, no changes to profiles or savegames. Installation: `references/release-1.1.0.json`. Dismantling when the game is closed: Copy `backup/FusionMod-1.0.1.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged.

User Manual Check: Helmet loss on normal hits and instant kills, both dice results over many zombies, magnet extraction without double drop, pick up/cancel, standalone planting, and both recipes with card and gauntlet. Four-shot, twelfth red ball, shield bonus for iron balls only, recoil, torchwood, escape/reversal without hypnosis and check exit on the right. Check Football Nut with 16000 HP, Jump Block, 5000 HP Heal and Damaged Body. Cross-check terrain, pot/water lily/pumpkin, pause, game speed, preview/mark, pool reuse and save/load during firing sequence, flying bullets and escape.


## Change 1.0.1: Glove marks the selected subfield

The native hover marker uses `Board.ToolHitTest`, while the glove calls `Board.GetTopPlantAt` when shooting. Therefore, if the left mushroom was correctly selected, the right mushroom could still light up. The new empty glove limited postfix for `ToolHitTest` uses the same existing subfield selection as the recording. Native coloring remains responsible. Empty subfields do not mark a sibling plant. Other tools and carried fusion ingredients retain their previous hit test; Recording, movement, recipes, costs and storage remain unchanged.

The screenshot `Bildschirmfoto_20261020_115736.png` proves the visual error. Static call documents are available under `references/shroom-slots/Board-HighlightPlantsForMouse.asm` and `Board-ToolHitTest.asm`. Tested offline: 117 subfield checks including eight new checks for hover/pickup, left/right/up, empty subfield, tool change, and carried ingredients; also workflow checks, recipe/asset consistency and release build without warnings or errors. The game didn't start. The user checks the visible marking in the game.

Installation: `references/release-1.0.1.json`. Backup/restore when game is closed: Copy `backup/FusionMod-1.0.0.dll` to `Mods/FusionMod.dll`. LawnGlove and both workflow files remain unchanged. The latest version number increases because only a display error is corrected.


## Change 1.0.0: Three mushrooms per field

Sun-shroom, Puff-shroom and variants with these native basic types occupy up to three subfields of the same cell. This includes the already existing Sundae shroom variant. The mouse selects the nearest subfield: top center, bottom right, or bottom left. The normal grid preview and the glove preview show the same position as the subsequent placement. An occupied subfield does not allow further normal planting; A free subfield can still be planted individually with one or two plants. Existing mushrooms that are still in the middle belong to the upper subfield and are not rearranged when loading.

With three mushrooms, an incoming plant checks all three recipe identities against the original ingredient. For each suitable recipe, a separate result is created in the previous subfield. Plants without a suitable recipe are preserved. For one or two plants, only the selected subfield is fused. The card or glove is used once per successful action; native card accounting and glove cooldown remain responsible. All results occur before the ingredients are consumed. If a result allocation fails, the mod removes the results that have already been generated and preserves all the ingredients.

The glove only picks up the plant in the selected subfield. He can place this plant in a selected free subfield of another cell or carry out appropriate fusions there. Sibling plants in the original field are retained. Existing cancel, preview, save and cooldown responsibilities remain in the unchanged LawnGlove; FusionMod extends its selection, placement and grid preview via Harmony. Movements within the same cell remain an abort as before. Water lilies, pots and pumpkins remain. Imitation cards continue to use native placement and are not interpreted as a new subfield or fusion action.

The stored native coordinates `mX` and `mY` determine the subfield. There is no new storage identifier, no additional plant data field and no ephemeral subfield catalog. Loaded variants retain their existing identity. Scripted `AddPlant` calls and loads do not trigger fusion. The condition exceptions for terrain checks are also reset in the event of errors. Native health, growth, sleep, damage and sun production remain responsible.

This release adds placement behavior, not new plants or recipes. The 150 active recipes and the 518 open recipes for 439 results remain unchanged. In particular, open puff-shroom fusions are not made available through invented replacement recipes. The offline batch tests use an explicitly isolated test recipe that is not registered in the mod. `fusionen.txt`, `fusionen-quellen.json`, `workflow.md` and `worklfow.md` remain unchanged. Version **1.0.0** increments the first number for a gameplay change according to the desired scheme; the other two numbers start again at zero.

References: `references/shroom-slots-source/` contains the actual 4.0.5.2 methods of CreatePlant, Mouse, SmallPuff, and SunShroom, as well as the Puff-shroom almanac. The triangle arrangement is adjusted for native replanted sprites. The mouse selection required by the user replaces the automatic assignment of the first free source subfield. `references/shroom-slots/` documents native placement, preview, coordinate storage and porting limits.

Checked offline: 109 checks of production subfield, card, glove, preview, marking and batch code; 75 bucket regressions; 13 workflow checks; Recipe/asset consistency and release build. These checks do not load game code. The game didn't start; Profiles and saves have not been changed. Installation and binary version check: `references/release-1.0.0.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.3.0.dll` to `Mods/FusionMod.dll`. The previous version does not offer subfield selection. LawnGlove remains unchanged.

Manual checking by the user:

- Plant Sun-shroom, Puff-shroom and Sundae-shroom in all three subfields, check mixed fields and have a fourth plant rejected.
- Move the mouse between the three subfields; Compare grid preview, actual position, foreground/background and grown sun-shrooms. Check old mushrooms in the middle.
- Pick up each individual plant with your glove, place it in a specific free subfield of another cell, break it off and check the original field. Only the selected plant is allowed to move.
- Check terrain, water lily, pot, pumpkin, pause, death, board change and save/load. Countercheck normal single plant fusions and costs/cooldown.
- As soon as suitable Sun/Puff recipes have been ported, check full fields with three suitable or mixed suitable ingredients: merge suitable ingredients, unsuitable plants are retained. The release itself does not add such recipes.


## Change 0.3.0: Fallout Blover, Solarfruit, Hypno Garlic and Flytrapper

```text
Blover | Chomper | Flytrapper
Blover | Doom-shroom | Fallout Blover
Garlic | Hypno-shroom | Hypno Garlic
Starfruit | Sunflower | Solarfruit
```

All ingredients were already fully usable as native plants. The local source archive contains exactly these four recipes for the four results. All source recipes are implemented, in both card orders and glove directions. Joint resolution, preview, marking, terrain checking and generation before ingredient consumption remains responsible. The incoming card determines the cost and cooldown. Water lilies, pots and pumpkins remain. A total of 150 recipes are active; 518 recipes for 439 results remain open. The four completed lines are removed from `fusionen.txt`; Subsequent fusions and `fusionen-quellen.json` remain unchanged.

- **Fallout Blover:** deals 180 Ash damage per pulse worldwide, including against flying targets. The actual source method generates a pulse at the initial blowing event after about 0.49 seconds. This is followed by ten more pulses every second over ten seconds; then the plant disappears. That's eleven impulses including the first event. Friendly, already dying and invalid targets as well as bosses, tombstones and target objects are excluded. The inherited bubble reaction expels fog for 30 seconds. Balloon zombies receive damage but are not additionally removed by the effect of a regular blover. Neither craters nor additional projectiles are created.
- **Solarfruit:** shoots five original stars in the directions 30, -30, 180, 90 and -90 degrees only after a normal click. Each star deals 100 damage. A successful attack costs 25 sun. Both actual source entrances strictly require more than 25 available sun; Therefore exactly 25 sun is not enough. The original controller plays the shot clip at speed two; the shooting event occurs after approximately 0.21 seconds. With a Starry Pumpkin in the same cell, a paid attack occurs every 0.35 seconds. This condition is implemented and checked offline; Starry Pumpkin itself remains an open, standalone fusion and will not be available through this release. Without this pumpkin there is no automatic homing or automatic shooting. Tool, glove and release clicks do not trigger an attack.
- **Hypno Garlic:** Redirects eligible friendly ground zombies within a source unit on the same row every 0.5 active seconds. An enemy bite first creates a fresh common Garlic with 300 HP, only then consumes the variant, hypnotizes the walker and triggers it to change ranks. Native row selection maintains valid rows, water/land compatibility, movement and storage. An ongoing row change is not restarted. If the creation of the Garlic fails, the variant and the enemy zombie remain unchanged. Damaged original bodies and hypno caps are embedded.
- **Flytrapper:** Floats with its original animations, is excluded from the native ground attack selection, and can devour nearby ground and air targets. The actual source radius is one source unit in front of the offset search origin. Native Chomper AI handles bite, unsuccessful bites against large targets, devour, 40 second digestion and storage. Vehicles, Bungee, Boss, Tombstone and Target Object are not swallow targets; friendly targets are not attacked.

All four plants have 300 HP. The reserved stored pairs are `Blover/Doomshroom`, `Starfruit/Sunflower`, `Garlic/Hypnoshroom` and `Chomper/Blover`. Active timers, fusion phases and pulse number are in native saved plant fields. Solar Stars retain their 100 damage through stored ID -440100 in `Projectile.mCobTargetX`, even after loading flying projectiles. Normal stars retain their normal damage. Native star movement, armor, collision, lifespan and the already imported original star graphics remain responsible. Pause, carry, bungee, death and board removal stop the new abilities. Animations do not create additional game events.

Original graphics and all ten plant clips come exclusively from the local `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: DoomBloverPrefab 42486 / Transform 119636 / Controller 6177; SunStarPrefab 52553 / 106215 / 6485; HypnoGarlicPrefab 43588 / 108762 / 6291; ChomperBloverPrefab 53765 / 100920 / 6240. The stars use the already embedded original Bullet_star 33973 object. SunStar has no visible shadow in the original; his body provides the graphic anchor. The three demonstrably orphaned transform tracks of the Hypno-Garlic clip are precisely documented and left out. Native cloud particles replace the original fallout particles. Audio, hypnosis and row movement remain native; Unity game scripts not loading.

Source recipes, descriptions, methods, script identities, export checksums, and offline previews are located in the four results folders under `references/`. Shared behavior and memory receipt: `references/fallout-blover/port-notes.md`; Source base classes and constants: `references/support-source-base/`; Fog source: `references/fallout-fog-source/`; native call and save paths: `references/support-native/` and the complementary `support-native-garlic*` folders. The workflow including `worklfow.md` remains unchanged.

Tested offline: recipe/asset consistency, 13 workflow checks, all 18 C# test suites including 92 new capability checks, all ten original clips and release build. This does not confirm playtesting. Installation: `references/release-0.3.0.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.2.0.dll` to `Mods/FusionMod.dll`. Savegames with these variants require 0.3.0 or a compatible newer version. LawnGlove remains unchanged. The median version number increases because of new plants; the patch number starts again at zero.

Check manually by user: all four recipes in both card orders and glove directions; Cost/cooldown, demolition, marking, roof/water and documentation. Fallout's starting pulse, ten follow-up pulses, air targets, armor, fog and check sequence. Solarfruits click, limit at 25/26 sun, five directions, 100 damage and check saved stars. Hypno Garlic with enemy bites and friendly contacts, check row edges and water boundaries. Flytrapper with ground, balloon and large targets, check digestion and lack of ground vulnerability. Check original animations, pause, gauntlet, bungee, death, and save/load in the middle of all stages. The game won't start; Profiles and scores will not be changed.

## Change 0.2.0: Hypno Pea, Hypno Repeater, Split Hypno and Gatling Hypno

```text
Gatling Pea | Hypno-shroom | Gatling Hypno
Hypno Pea | Peashooter | Hypno Repeater
Hypno Pea | Repeater | Split Hypno
Hypno Pea | Split Pea | Gatling Hypno
Hypno Repeater | Peashooter | Split Hypno
Hypno Repeater | Repeater | Gatling Hypno
Hypno-shroom | Peashooter | Hypno Pea
Hypno-shroom | Repeater | Hypno Repeater
Hypno-shroom | Split Pea | Split Hypno
Peashooter | Split Hypno | Gatling Hypno
```

All four results have a direct recipe path with fully usable native ingredients. The additional paths will become available in the order of Hypno Pea, Hypno Repeater, Split Hypno and Gatling Hypno. This means that all ten source recipes from these results have been implemented. Both map orders and glove directions use the existing common resolution, terrain checking and generation before ingredient consumption. The incoming card determines the cost and cooldown. Water lilies, pots and pumpkins remain. A total of 146 recipes are active; 522 recipes for 443 results remain open. The ten completed lines are removed from `fusionen.txt`; Subsequent fusions and `fusionen-quellen.json` remain intact.

- **Hypno Pea:** Shoots an original hypno pea for 30 damage per 1.5 seconds.
- **Hypno Repeater:** Shoots two hypno peas, each dealing 30 damage per 1.5 seconds. After 15 active seconds, a normal click can evolve the plant into Split Hypno for free.
- **Split Hypno:** shoots one hypno pea forward and two backwards; each deals 30 damage. The rear projectiles ricochet off the left edge of the screen. A target on one side activates both minds. After 15 active seconds, a normal click will evolve the plant into Gatling Hypno for free.
- **Gatling Hypno:** Shoots four hypno peas, each dealing 30 damage per 1.5 seconds.

A fatal Hypnopea hit creates exactly one friendly PeaHead zombie with 270 HP at the position and row of the defeated zombie. This shoots original hypno peas every 1.5 seconds for 20 damage. His deadly projectiles can also spawn more friendly PeaHead zombies; the actual source method selects the same bullet type 153. The first shot randomly waits 0.5 to 1.5 seconds according to `PeaShooterZ.Awake`. Survivors, armor-only hits, already dying or friendly targets, as well as bosses, tombstones and target objects do not create allies. Regular Torchwood does not alter Hypnopeas. The special fire pea is part of the independent Hypnowood upgrade and is not a projectile of these four basic plants.

All plants have 300 HP. The stored pairs are `Peashooter/Hypnoshroom`, `Repeater/Hypnoshroom`, `Splitpea/Hypnoshroom` and `Gatlingpea/Hypnoshroom`. Native shooter AI controls target selection, rows, number of shots and staggered volleys; the existing common growth logic stores the 15 seconds in `Plant.mStartRow`. Break, glove, bungee and death stop growth. Evolution takes remaining HP and does not generate a fusion reward. Bullet origin and 30 or 20 damage remain stored in `Projectile.mCobTargetX`. Allies' special ammo will be marked in their unused native saved `mSummonCounter`. Rebound and special effect also work after loading rear projectiles that are already flying. No additional volatile provenance tables are created.

Plant and bullet graphics as well as all eight original clips come from the local `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: HypnoPeashooterPrefab 37181 / Transform 120499 / Controller 6360; HypnoRepeaterPrefab 54444 / 110839 / 6267; HypnoSplitPrefab 37189 / 120507 / 6363; GatlingPeaPrefab 38424 / 107628 / 6294; Bullet_hypnoPea 63610 / 114455. The misleadingly named GatlingPeaPrefab actually has the HypnoPeashooter script. SuperHypnoGatlingPrefab 38420 belongs to another upgrade and is not used. Original hierarchies, pivots, shadows and additive animations are preserved. Provably orphaned controller bindings are individually documented. Summoned zombies use native PeaHead rendering, walking animation, hypnosis, bite, armor, and death logic; their bullets use the original graphics. Hit particles and audio remain native. Unity game scripts are not carried over.

Source recipes, original descriptions, methods, script identities, export checksums and offline previews are available at `references/hypno-pea/`, `references/hypno-repeater/`, `references/split-hypno/`, `references/gatling-hypno/` and `references/hypno-projectile/`. Common Behavioral Document: `references/hypno-pea/port-notes.md`; Source base classes: `references/hypno-source-base/` and `references/hypno-torch/`; native call and save paths: `references/hypno-native/`. In particular, `Zombie.UpdateActions` contains an embedded copy of the PeaHead shot logic. Both actual firing paths are taken into account. The workflow including `worklfow.md` remains unchanged.

Tested offline: recipe/asset consistency, 13 workflow checks, all 17 C# test suites including 112 new Hypno checks, original animation previews and release build. This does not confirm playtesting. Installation: `references/release-0.2.0.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.61.dll` to `Mods/FusionMod.dll`. Savegames with these variants require 0.2.0 or a compatible newer version. LawnGlove remains unchanged. The median version number increases because of new plants; the patch number starts again at zero.

Check manually by user: all ten recipes in both card orders and glove directions; Cost/cooldown, demolition, marking, roof/water and documentation. shot numbers and both split heads; rebound on the left edge; 30 damage, armor and lethal hits; Position, 270 HP, hypnosis and 20 damage ally missiles including other summons. Both click upgrades after 15 seconds, check damaged plants and demolition with glove. Original animations, Torchwood, pause, bungee, death, and save/load during bursts, growth, missile flight, and check for summons. The game won't start; Profiles and scores will not be changed.

## Change 0.1.61: Sunny-shroom, Starry-shroom, Sturdy-shroom and Camo-shroom

```text
Blover | Scaredy-shroom | Camo-shroom
Scaredy-shroom | Starfruit | Starry-shroom
Scaredy-shroom | Sunflower | Sunny-shroom
Scaredy-shroom | Wall-nut | Sturdy-shroom
```

All ingredients were already fully usable as native plants. The local source archive contains exactly these four recipes for the four results; there are no other alternatives. Both map orders and gauntlet directions use common recipe resolution, terrain checking, and generation before ingredient consumption. Cost and cooldown come from the incoming card. Water lilies, pots and pumpkins remain. A total of 136 recipes are active; 532 recipes for 447 results remain open. The four completed entries are removed from `fusionen.txt`. Subsequent fusions and `fusionen-quellen.json` remain unchanged.

- **Sunny-shroom:** produces 25 sun each and continues to produce even during fear. The production interval starts at 25 seconds, continuously decreases by half a second per active second, and reaches at least ten seconds. In fact, `ScaredySun.GetNearZombies` continuously resets the interval to 25 seconds for nearby zombies. An already running production process will not be restarted. The first pre-run is four to seven seconds, further pre-runs use the current interval with five percent chance. The original brightness increases from one to four within three seconds; the sun is formed at its maximum. It then fades away within 0.6 seconds.
- **Starry-shroom:** Generates five original stars every 0.75 seconds with no target requirement, each dealing 20 damage. The directions are 30, -30, 180, 90 and -90 degrees. Nearby zombies interrupt the attack and trigger the original fear sequence. The shooting event is at 0.25 seconds in the 0.416667 second original clip. Native star movement, row switching, collision, armor, lifespan and storage remain responsible. The existing original star `Bullet_star` is reused; the source method explicitly produces bullet type 39.
- **Sturdy-shroom:** has 4000 HP and only takes half the incoming HP damage while crouching. Bites, zombie peas, basketballs and fatal bites are included; integer damage is halved as in the source. An initially fatal bite can be survived by halving it and then does not increase the count of eaten plants. Native Jack in the Box distance translates to 1800 damage or 900 damage while crouching. After more than ten seconds of continuous threat, the plant straightens up. After another 0.5 seconds, it deals 20 damage to shield and body in the original area and knocks back eligible targets by two source units. No new fear occurs for five seconds. Squeezing and ordinary removal retain their native ways of death.
- **Camo-shroom:** Floats with its original animation and does not attack. One's flying identity is excluded from the native selection for ground attacks. In a 3x3 cell environment, an active Camo-shroom prevents bites on ducking Scaredy-shrooms, including the native basal plant and all ported variants. The protection ends when the Camo-shroom is upright, sleeping, being carried or disappearing. Protecting neighboring plants solely prevents biting; other types of attacks remain possible.

Sunny, Starry and Camo have 300 HP. All four variants retain the native mushroom, coffee, terrain and glove life cycle. The reserved stored pairs are `Scaredyshroom/Sunflower`, `Scaredyshroom/Starfruit`, `Scaredyshroom/Wallnut` and `Scaredyshroom/Blover`. Production advance, half interval ticks, brightening, fear phase, delayed defensive shock and fear immunity lie exclusively in native saved fields. Stars that are already flying do not require additional volatile provenance management. Pause, Sleep, Carry, Bungee and Death stop the simulation; Original animations do not create any additional effects. Card or glove previews preserve the original graphics via the existing import renderer.

All original graphics and 14 clips come exclusively from `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: ScaredySunPrefab 75230 / Transform 100428 / Controller 6420; ScaredyStarPrefab 75229 / 100427 / 6424; ScaredyNutPrefab 75226 / 100424 / 6422; ScaredyBloverPrefab 37300 / 91125 / 6421. Stars use the already embedded original object 33973. Unity game scripts are not loaded. Source recipes, almanac descriptions, actual methods, script identities, export checksums, and offline previews are located in the four results folders under `references/`. Shared behavior and memory receipt: `references/sunny-shroom/port-notes.md`; native caller: `references/scaredy-support-native/`. The workflow including `worklfow.md` remains unchanged.

Tested offline: all 16 existing C# test suites, including 139 Scaredy tests with the new capabilities, actual damage paths, nested damage contexts, pool reuse and saved phase resumption; Recipe/asset consistency, 13 workflow checks, all 14 new original clips and release build. No confirmed playtests. Installation: `references/release-0.1.61.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.60.dll` to `Mods/FusionMod.dll`. Savegames with these variants require 0.1.61 or a compatible newer version. LawnGlove remains unchanged.

Check manually by user: both card orders and glove directions, cost/cooldown, cancel, mark, water/roof, underlays and pumpkins; Sunny's accelerating production and continued production while ducking; Starry's Five Directions and Fear Interruption; Sturdy's damage halving, fatal bites, jack blasts, delayed defensive blast, and five seconds of fear immunity; Camos flight and bite protection inside and outside the 3x3 environment. Check original animations, armor, pause, sleep/coffee, gauntlet, bungee, death and save/load in the middle of clear, shot, duck or save. The game won't start; Profiles and scores will not be changed.

## Change 0.1.60: Speedy-shroom, Blasty-shroom, Shivery-shroom and Spuddy-shroom

```text
Peashooter | Scaredy-shroom | Speedy-shroom
Cherry Bomb | Scaredy-shroom | Blasty-shroom
Ice-shroom | Scaredy-shroom | Shivery-shroom
Potato Mine | Scaredy-shroom | Spuddy-shroom
```

All four pairs of ingredients were previously completely usable. The local source archive contains exactly these four recipes for these results; All alternatives are covered. Both map orders and glove directions utilize the existing common recipe resolution, terrain checking, and generation before ingredient consumption. Cost and cooldown come from the incoming card. Water lilies, pots and pumpkins remain. A total of 132 recipes are active; 536 recipes for 451 results remain open. The four completed lines are removed from `fusionen.txt`; `fusionen-quellen.json` and subsequent fusions remain unchanged.

- **Speedy-shroom:** five percent more attack speed and one additional point of damage per shot, maximum 150 percent and 50 damage. The basic interval is 1.5 seconds and decreases to 0.6 seconds. Fear resets the bonus. Flying peas retain their damage. Normal Torchwood adds 20 damage and gains native area damage.
- **Blasty-shroom:** an original cherry spore that deals 40 direct damage every 1.5 seconds. Crouching deals 600 cherry damage in the original radius once. The effect is 0.833333 seconds in the original clip; constant crouching does not repeat it. The general cherry effect of consuming a Cherry Bomb remains.
- **Shivery-shroom:** an original ice missile with 40 damage every 1.5 seconds, cold, ten freezing points and quadruple damage against frozen targets. When crouching, nearby zombies receive 20 damage and four seconds of freezing. Native immunities, longer existing freeze times, and Torchwood thaws are retained.
- **Spuddy-shroom:** one original potato spore every three seconds. In the first 20 active seconds it deals 600 damage and costs the plant 75 HP; Afterwards it deals 60 damage without any HP consumption. Without healing, the fourth early shot kills the 300 HP plant; the last spore flies on. Death creates a potato explosion with 600 damage and the original radius of a source unit. Ingredient consumption and board removal do not trigger a death explosion.

All four plants have 300 HP and retain native Mushroom, Coffee, Terrain and Glove rules. The stored identifiers are `Scaredyshroom/Peashooter`, `Scaredyshroom/Cherrybomb`, `Scaredyshroom/Iceshroom` and `Scaredyshroom/Potatomine`. Native fields store bonus, advance, shooting phase, crouching and rearing. Even flying projectiles retain their damage, special effect and original graphics when loaded via a reserved identifier in `Projectile.mCobTargetX`. The existing freezing points on zombies remain fleeting.

Original art and all 20 plant clips are from local `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: PeaScaredyPrefab 75222 / Transform 100420 / Controller 6418; CherryScaredyPrefab 75219 / 100417 / 6416; IceScaredyShroomPrefab 75227 / 100425 / 6612; ScaredyPotatoPrefab 75220 / 100418 / 6419. Original bullets are Bullet_pea 33962, Bullet_puff_cherry 73654, Bullet_snowPea 62921 and Bullet_puffPotato 73652. All five clips per plant, original hierarchy, pivots, shadows and alpha curves are embedded. Hit, ice, explosion particles, and audio use native effects. Unity game scripts not loading.

Sources, descriptions, script identities, method excerpts, asset checksums, and offline previews are located in the four results folders under `references/`. The shared document is in `references/speedy-shroom/port-notes.md`; native callers and serialization in `references/scaredy-native/`. The existing workflow including `worklfow.md` remains unchanged.

Offline checking: recipe/asset consistency, 13 workflow checks, targeted scary regressions, affected existing regressions, original animations and release build. No confirmed playtests. Installation: `references/release-0.1.60.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.59.dll` to `Mods/FusionMod.dll`. Savegames with these variants require 0.1.60 or a compatible newer version. LawnGlove remains unchanged.

Manually checked by the user: all four recipes and both directions, cost/cooldown, cancellation, marking, water/roof and documents. Speedy's Increase and Fear Reset; Blasty's one-time explosion; Shivery's projectiles and local freezing effect; Spuddy's HP consumption, 20 second limit, final shot and death explosion. Original animations, Torchwood, Armor, Pause, Sleep/Coffee, Gauntlet, Bungee and Save/Load in the middle of Skills and Check Missile Flight. The game won't start; Profiles and scores will not be changed.

## Change 0.1.59: Black Cherry, Frozen Cherry, Demise-shroom and Sundae-shroom

```text
Cherry Bomb | Doom-shroom | Black Cherry
Cherry Bomb | Ice-shroom | Frozen Cherry
Doom-shroom | Ice-shroom | Demise-shroom
Ice-shroom | Sunflower | Sundae-shroom
```

The local source archive contains exactly these four recipes for the four results. All ingredients were completely usable beforehand. Both map orders and glove directions use common recipe resolution, terrain checking, and generation before ingredient consumption. Cost and cooldown come from the incoming card. Rootstocks and pumpkins are retained when fused. A total of 128 recipes are active; 540 recipes for 455 results remain open. The four completed entries are removed from `fusionen.txt`; Subsequent fusions and `fusionen-quellen.json` remain unchanged.

- **Black Cherry:** Successful fusion creates a one-time cherry blast dealing 1800 ash damage, counting each damaged zombie, including survivors. After 1.5 seconds, a doom explosion follows with 1800 + 300 damage per previously damaged zombie. Premature crushing also triggers the follow-up explosion; ordinary removal does not repeat them. The Doom explosion has a source radius of five world units and leaves the existing native 180 second crater. The crater removes the plant stack from his cell as before. Loading does not repeat the first explosion.
- **Frozen Cherry:** Stands still and absorbs appropriate friendly cherry bullets at the same height and row. Pink cherries provide one point, fire cherries two, explosive cherries ten. From 100 points onwards, a native, removable Cherry Bomb card is created; the source counter is reset to zero. Normal peas and foreign bullet types remain unaffected. Death deals 1800 cherry damage and freezes for ten seconds in the blast radius. The source radius is two world units, around 118.52 replanted pixels. The general cherry effect when consuming a Cherry Bomb is retained.
- **Demise-shroom:** uses the original explosion clip at speed 0.7; its event is 2/3 clip seconds, i.e. after around 0.96 simulation seconds. Afterwards, eligible zombies across the board receive 1800 ash damage and five seconds of freezing. The native crater remains for 180 seconds. Regular wave generation and its timer pause for ten seconds. Existing zombies and plants continue to move or act; Scripted generation by other mods remains responsible.
- **Sundae-shroom:** generates 25 sun three times each. Zombies that are already frozen before the respective ice effect drop an additional 15 sun each. The first two ice effects each deal 20 damage and cold without any new freezing. The third also deals 20 damage, freezes for four seconds and removes the plant. The first production run begins after four to seven seconds, further runs after 25 seconds with five percent chance. Original lightening from factor one to four and subsequent darkening are included; the sun is formed at its maximum brightness.

All four results have 300 HP. Native terrain, mushroom/coffee, glove, pause and bungee rules remain. Frozen Cherry uses the regular saved base `Peashooter/Iceshroom` to keep the permanent plant regularly vulnerable including vehicle contact. Sundae-shroom uses `Sunshroom/Iceshroom` for the same normal mushroom life cycle. Black Cherry uses `Cherrybomb/Doomshroom`, Demise-shroom `Doomshroom/Iceshroom`. `mLaunchCounter`, `mStartRow`, `mTargetX` and `mTargetY` store advance, production level, hit/absorption counter and brightening phase. Native special and shooting events do not produce additional effects. Native freeze immunities and longer existing freeze times are retained.

Demise-shroom's ten second spawn lock is in the crater unused and natively saved fields `GridItem.mSeedType = Iceshroom` and `mSunCount`; Reset removes both. Several locks work at the same time, their times do not add up. The bullet ID of pink and explosive cherries is now at the exclusive values ​​`-41001` and `-41010` in `Projectile.mCobTargetX`. Native initialization, reset and death clean them up. This means that the damage, original graphics and absorbability of already flying cherry bullets are preserved after loading. The existing remaining volatile zombie effects and solar value helpers retain their previous storage limits.

Original art and all six clips come exclusively from the local `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: DoomCherryPrefab 37633 / Transform 119536 / Controller 6234; IceCherryPrefab 66980 / 113301 / 6235; IceDoomPrefab 39392 / 113189 / 6606; SunIceShroomPrefab 55289 / 92994 / 6315. Black Cherry retains the additive shaking level with weight one. No unknown animation path is discarded. These four plants do not produce new flying projectiles. Frozen Cherry uses the already ported cherry bullets and native card mount; Explosions, cold particles and sounds use existing native effects. Unity game scripts are not running.

Source recipes, almanac texts, prefab/script identities, actual methods, and export checksums are in `references/black-cherry/`, `frozen-cherry/`, `demise-shroom/`, and `sundae-shroom/`. Shared behavior and memory receipt: `references/black-cherry/port-notes.md`. Production documents: `references/sundae-production/` and `ice-producer-base/`. Native callers and serialization: `references/ice-instant-native/`. The research helper can now select nested coroutines with the same name via their actual owner. The workflow only complements this time-saving selection; `worklfow.md` remains the existing link to `workflow.md`.

Recipe/asset consistency, workflow checks, targeted regressions and affected existing regressions as well as the release build are checked offline. There are no confirmed playtests. Installation: `references/release-0.1.59.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.58.dll` to `Mods/FusionMod.dll`. New saves require 0.1.59 or a compatible newer version. LawnGlove remains unchanged.

Manual user review: all four recipes in both card orders and glove directions; Cost, Cooldown, Cancel, Preview, Mark, Water, Roof, Pads and Pumpkins. Check Black Cherry for two explosions, hit count against survivors, increased follow-up damage, early crush, removal and crater. Check Frozen Cherry for suitable and inappropriate projectiles, 1/2/10 points, card generation, card capture and death with ten seconds of freezing. Check demise for global effect, immunities and exactly ten seconds of wave blocking. Sundae on three productions, 15 additional sun exclusively for already frozen zombies, check cold on the first two productions and freeze on the last process. Check all plants during break, sleep/coffee, glove, bungee, vehicle contact, death and save/load in the middle of skills and missile flight. The game won't start; Profiles and scores will not be changed.

## Change 0.1.58: Gloom fusions and Gutsy-shroom

```text
Fume-shroom | Gloom-shroom | Gloom-shroom
Frost-shroom | Gloom-shroom | Frost Gloom-shroom
Gloom-shroom | Ice-shroom | Frost Gloom-shroom
Flame-shroom | Gloom-shroom | Flame Gloom-shroom
Gloom-shroom | Jalapeno | Flame Gloom-shroom
Fume-shroom | Scaredy-shroom | Gutsy-shroom
```

All six source recipes for these four results are implemented. All ingredients were previously usable as native plants or fully ported fusions. Gloom-shroom is already native; What is new is his still open source recipe including original representation and source behavior. Its native identity remains `Gloomshroom` (42), with no variant marker. The two elementary variants use `Gloomshroom/Iceshroom` and `Gloomshroom/Jalapeno`; Gutsy-shroom uses `Fumeshroom/Scaredyshroom`. Both card orders and glove directions share common recipe resolution, terrain checking, and generation before ingredient consumption. Card cost and cooldown come from the incoming ingredient. Water lilies, pots and pumpkins remain. A total of 124 recipes are active; 544 recipes for 459 results remain open. The six completed lines are removed from `fusionen.txt`. Subsequent fusions and `fusionen-quellen.json` remain intact. The existing workflow including the `worklfow.md` link does not require any change.

- **Gloom-shroom:** four piercing smoke pulses for 20 damage every 1.9 seconds. The attack reaches suitable ground targets in front of, behind and next to the plant, at most one neighboring row away. Source physics uses a radius of two source units, about 1.48 columns; the port checks for overlap with native target rectangles. The original events at 0.6 / 0.8667 / 1.1333 / 1.4 clip seconds are executed at speed 1.4.
- **Frost Gloom-shroom:** the same four pulses each with 40 damage, cold and five points of freezing each over the existing common freeze helper. The original speed is 1.3. Native cold immunity and the previous freezing system remain responsible.
- **Flame Gloom-shroom:** four pulses of 40 base damage each and overheating via the existing shared system. Targets that are already overheated receive an additional 50 percent damage. Cold Removal, Death Blast and Overheat Passing are included. The original additional fire layer remains active at all times; Attack speed is 1.3.
- **Gutsy-shroom:** When attacking, choose the next suitable target in front of you in your own row. When within seven source units of distance, he uses piercing smoke for 30 damage. Otherwise, it creates two staggered original spores, each dealing 30 direct damage. The cycle is 1.5 seconds. The smoke animation hits after 0.75 seconds; the double-fast spore animation produces projectiles at 0.375 and 0.5417 seconds. The plant doesn't hide even when there are nearby zombies. Spores fly across the row at six source units per second, use native movement and collision, and are not ignited by Torchwood.

All four results have 300 HP and retain native Mushroom, Coffee, Terrain and Gloves rules. Attack phase and cooldown are saved in `mStartRow` and `mLaunchCounter`. Gutsy-shroom additionally saves the mode selected when attacking in `mTargetX`; loading between the two shots produces only the remaining shot. Original animations do not produce additional hits. Pause, Sleep, Carry, Bungee, Death and Board End stop skills. Native `Fire` calls are suppressed for these identities. Gutsy spores reserve the value `-40030` in `Projectile.mCobTargetX` exclusively for native pea types. Native serialization preserves identifier and flight movement; Initialization and pool reset delete the identifier. This means that damage, Torchwood lock and original graphics are retained even for spores that are already flying after loading. The existing volatile freeze points and overheat states on Zombies retain their previous storage limits.

Original graphics and all ten plant clips come from `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: GloomShroomPrefab 33647 / Transform 108374 / Controller 6300; IceGloomPrefab 33651 / 108378 / 6301; FireGloomPrefab 33650 / 108377 / 6299; ScaredFumePrefab 48132 / 92173 / 6628. The original spore Bullet_puff 73653 / Transform 95616 does not have an animator. The export preserves all levels, speeds, reference poses and bindings. Gutsy-shroom's alternative spore and smoke animations each remain their own additive head movement. No unknown binding path is ignored. The three Gloom Plants do not produce flying collision projectiles. Smoke Particles, Hit Particles, Cold Particles, and Sounds use existing native effects. Unity game scripts are not running.

Source recipes, Chinese almanac descriptions, actual disassembled methods, prefab/script identities, original controllers, and asset checksums are under `references/gloom-shroom/`, `references/frost-gloom-shroom/`, `references/flame-gloom-shroom/`, and `references/gutsy-shroom/`. The floor documents are under `references/gutsy-projectile/` and `references/gutsy-bullet-base/`; the shared behavior and storage document is in `references/gloom-shroom/port-notes.md`. Recipe and asset consistency, 13 workflow checks, all 14 existing regression test programs, the original animations and the release build are checked offline. There are no confirmed playtests. Installation and checksums: `references/release-0.1.58.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.57.dll` to `Mods/FusionMod.dll`. Savegames with the new Elemental or Gutsy variants require 0.1.58 or a compatible newer version. LawnGlove remains unchanged.

Manual user review: all six recipes in both card orders and glove directions, cost, cooldown, cancel, highlight, preview, water, roof, underlays and pumpkins. Gloom pulses for four hits, 1.9 second cycle, check targets back/front/side and radius limit. Frost for cold, five freezing points each and check immunities. Check flame for overheating, increased consequential damage and fatal consequences. Check Gutsy for smoke on close targets, two long range spores, mode switching between volleys, targets behind the plant, lack of fear and Torchwood lock. For all variants, check original animations, pause, sleep/coffee, glove, bungee, death and save/load during attack and projectile flight. The game won't start; Profiles and scores will not be changed.

## Change 0.1.57: four threepeater fusions

```text
Garlic | Threepeater | Stench-spreader
Ice-shroom | Threepeater | Snow Threepeater
Jalapeno | Threepeater | Scorched Threepeater
Jalapeno | Scorched Threepeater | Charred Threepeater
```

The local source archive contains exactly these four recipes for the four new results. The first three recipes use usable native ingredients. Afterwards, the fully implemented Scorched Threepeater becomes available as an ingredient for Charred Threepeater. Both map orders and glove directions use the same recipe resolution, terrain checking, and generation before ingredient consumption. The cost and cooldown still come from the incoming card. Follow-up fusions, particularly Phoenix Threepeater, remain open.

- **Stench-spreader:** three garlic peas with 30 damage each every 1.5 seconds in your own and neighboring rows. At the edge, an additional shot in your own line replaces the missing side shot after 0.2 seconds. All projectiles use the existing poison levels, poison and poison clouds. `Miasma.Update` counts this plant with three shots for the cloud damage calculation in its stand row; Side shots do not increase the number of plants in other rows.
- **Snow Threepeater:** three ice projectiles per second. He chooses up to three different rows based on the accessible zombie that is furthest to the left. He supplements free firing positions with his own, previous and next row without doubling. The native threepeater trajectory is scaled for distant target rows. The existing snow projectile implementation deals 20 damage, cold, ten freezing, and quadruple damage against frozen targets.
- **Scorched Threepeater:** three common peas for 40 damage each every 1.5 seconds; Edge replacement like the Stench spreader. The successful fusion triggers the Jalapeno effect with 1800 ash damage, removal of the cold and the existing healing of the Flame Tall-nut in its own and adjacent rows. Loading or simply creating the saved variant does not repeat this effect. Common Torchwood remains permitted: `BoardAction.FirePeas` adds 20 damage, i.e. 60 direct damage after ignition; the native area damage is adjusted accordingly. Fertilizer only converts this variant back into a regular threepeater after the saved feeding process is complete.
- **Charred Threepeater:** the fully burned variant has 300 HP and the original rest animation, but no attack. Native shooting events also remain suppressed. Another Jalapeno fusion to Phoenix Threepeater will not be available until its full implementation.

All four variants retain 300 HP, native threepeater terrain rules, rootstocks, pumpkins, gauntlet ability, and saved variant identity. The centrally reserved memory pairs are `Threepeater/Garlic`, `Threepeater/Iceshroom`, `Threepeater/Jalapeno` and `Threepeater/Doomshroom`. `mStartRow` saves the attack phase and `mLaunchCounter` saves the cooldown. The original medium shot clip triggers the attack at 0.5 seconds. Native animation events do not create second floors. Pause, Carry, Bungee, Sleep and Death stop the simulation via the existing activity path. The special pea damage is stored as a unique negative value `-40040` in the native `Projectile.mCobTargetX` unused in peas. Native initialization and pool reset clear the field; Serialize/Deserialize preserves it, Torchwood doesn't change it. This means that the damage of scorched bullets that are already flying is retained when loading. Existing poison levels, poison clouds and freezing points remain volatile as before; their boundaries will not be changed by this expansion.

Original graphics and all 22 clips come from `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: GarlicThreePeaterPrefab 63448 / Transform 110668 / Controller 6512; IceThreePeaterPrefab 63450 / 110670 / 6512; DarkThreePeaterPrefab 63449 / 110669 / 6512; AshThreePeaterPrefab 63452 / 110672 / 6506. The three active plants have a body plane and three independently additive head planes, each with alternative resting and shooting clips. The importer preserves all layers, reference poses, sprite hierarchies and visibility bindings. Charred Threepeater only uses its original resting clip. Stench bullets use the original 34023 projectile already exported; Scorched bullets the already exported original common pea model 33962. Snow and ignited fire peas use the existing native Replanted controllers and effects. Jalapeno, cold, poison particles and sounds use the existing native effects. No Unity game scripts are running.

Sources, almanac descriptions, recipe credits, full gameplay prefabs, script identities, animation data, checksums and offline previews are at `references/stench-spreader/`, `snow-threepeater/`, `scorched-threepeater/` and `charred-threepeater/`. The evidence for poison clouds is under `references/stench-spreader-cloud/`, and for fusion and fertilization events under `references/scorched-threepeater-effects/`. The shared behavior and storage document is in `references/stench-spreader/port-notes.md`. The recipe and asset consistency, workflow checks, targeted threepeater regressions and affected existing poison, fire, nut and squash regressions as well as the release build are checked offline. The game won't start; Profiles and scores will not be changed. The installation is documented under `references/release-0.1.57.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.56.dll` to `Mods/FusionMod.dll`. Savegames with the new variants require 0.1.57 or a compatible newer version.

Manual user review: all four recipes in both card orders and glove directions; cost, cooldown, cancellation, preview and highlighting; Water, roof, pads and pumpkins. Check stench spreaders for poison levels, poisoning, clouds and their damage in your own and other rows. Check Snow Threepeater for prioritization of multiple zombies per row, distant target rows, five and six rows, free firing slots and freeze effects. Check Scorched Threepeater for the three unique fire rows, 40 fire pea damage, 60 fire pea damage including area damage and fertilizer reconversion. Charred Threepeater may not fire even after loading or carrying. For all variants, check edge replacement shots, pause, bungee, death and save/load during attack and projectile flight.

## Change 0.1.56: Icerock, Hearthrock, Foul-shroom and Starch-shroom

```text
Spikerock | Ice-shroom | Icerock
Iceweed | Spikerock | Icerock
Spikerock | Jalapeno | Hearthrock
Scorchweed | Spikerock | Hearthrock
Fume-shroom | Garlic | Foul-shroom
Fume-shroom | Potato Mine | Starch-shroom
```

The complete local source archive contains exactly these six recipes for the four new results. All ingredients were already present as native replants or complete fusions. Native Spikerock (SeedType 46) is now also included in the tested ingredients list; his own fusion recipe, which is still open, remains unchanged. Both card orders and glove directions use common recipe resolution, placement checking, and generation before ingredient consumption. Native terrain rules, cost, cooldown, preview, highlighting, water lilies, pots and pumpkins remain. The six completed lines are removed from `fusionen.txt`; Follow-up fusions remain open. There are now 114 recipes active; 554 recipes for 467 results remain open. `fusionen-quellen.json`, `workflow.md` and `worklfow.md` remain unchanged; a workflow change is not necessary.

Icerock and Hearthrock retain native Spikerock AI, 450 HP, low target category, vehicle contact, hammer reactions and save. They deal two hits per second with 40 base damage each. Native vehicle contacts continue to deal 1800 damage and utilize the existing attrition path. After a hit, Icerock applies 15 seconds of cold and five freezing points over the existing common freezing aid. Native immunities apply. Hearthrock sets the existing overheat condition after a hit, including damage buff, death blast, and passing. Already overheated targets receive its existing damage boost. The original graphics lose the first two major peaks at a maximum of 300 and 150 HP respectively. Idle and both attack moves are from the original controller. The two original events are adapted to the native contact phases and produce no additional damage.

Foul-shroom and Starch-shroom retain 300 HP, native mushroom/coffee bean rules, and 1.5 second attack cycle. The hit occurs in the original event after 350 ms and penetrates suitable ground targets in the own row up to seven source units forward, approximately 414,815 replanted pixels. Foul-shroom deals 30 damage and then one level in the existing common poison condition. Its poison damage begins at sixth level; a regular smoke attack does not create a cloud of miasma. Starch-shroom also usually deals 30 damage. Its charge starts at 15 seconds. The first wave fired afterward replaces normal damage for each target hit with a 300-damage potato explosion. The actual call to `AoeDamage.SmallBombPotato` has radius 0 and row spacing 0: the port uses the existing explode helper as a point overlap in the native target rectangle. Multiple overlapping target rectangles can receive multiple explosions. There is no additional normal 30 damage hit. Each charged wave consumes the charge even if there are no targets left when the hit event occurs. After that, charging starts again at 15 seconds.

The stored identities are `Spikerock/Iceshroom`, `Spikerock/Jalapeno`, `Fumeshroom/Garlic` and `Fumeshroom/Potatomine`. As before, the attack phase and timing of the mushrooms are in `mStartRow` and `mLaunchCounter`. Starch-shroom uses the `mTargetX`, `mTargetY` and `mShootingCounter` fields unused for native Fume-shroom outside the suppressed fire path for charge, blink distance and blink phase. Initialization, reset, native field recorders and serialization/loading are checked. The original flashing uses the distance max(0.5 seconds, remaining charge / 5). The original additive layer has weight 0.97 and only discrete sprite bindings. Charging and flashing phase are retained when saving. Pause, sleep, carry, bungee, death and board end stop skills over the existing activity path. The pre-existing poison and freeze states on zombies remain volatile and are reset upon loading.

None of these four plants produce their own flying collision projectiles. Cold, freezing points, overheating, poison, piercing smoke and all potato explosions are implemented. Original graphics and all nine clips come exclusively from the local original bundle `english-4.0.5.2/PlantsVsZombiesRH_Data/data.unity3d`: IceSpikerockPrefab 60659 / Transform 89928 / Controller 6220; FireSpikerockPrefab 60661 / 89930 / 6220; GarlicfumePrefab 70211 / 100382 / 6276; PotatoFumePrefab 70226 / 100397 / 6284. There are no discarded unknown bindings. For the rock variants, the original shadow object is empty; the original body serves as a scaling footprint. Smoke particles, explosion particles, cold particles and sounds use native Replanted effects. Unity game scripts and animation events are not running.

Sources, almanac descriptions, recipe completeness, prefab/script identities, asset checksums, and offline previews are under `references/icerock/`, `references/hearthrock/`, `references/foul-shroom/`, and `references/starch-shroom/`. The shared behavior and storage document is in `references/icerock/port-notes.md`; native paths are occupied under `references/weed-fusions/` and `references/fume-native/`. Offline Evidence: Recipe/Asset Check, 13 Workflow Checks, Affected Fume, Weed, Poison, Mine and Elemental Nut Regressions, Original Asset Previews and Release Build. The remaining existing regressions test extended shared identity and prescription registration. No game start, no playtests, no changes to profiles or savegames. Installation and version proof: `references/release-0.1.56.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.55.dll` to `Mods/FusionMod.dll`. Savegames with these variants require version 0.1.56 or a compatible newer version. LawnGlove remains unchanged.

Manual user review: all six recipes in both card orders and glove directions, cost, cooldown, cancel, terrain, pads and pumpkins, highlighting and preview. Check Icerock on both contacts, 15 seconds cold, freezing points, vehicles, giants and graphics at 300/150 HP. Check Hearthrock for both contacts, overheating and fatalities. Check Foul-shroom for 30-damage pungent smoke, range, poison levels, and shared effect with Poison Shooters. Check starch-shroom for first 15 seconds, normal and charged waves, overlapping targets, empty waves, charging and original flashing. For all variants, check pause, gauntlet movement, bungee, death, pool reuse, and save/load during attack, charge, and blink phases.

## Change 0.1.55: Doom Squash, Jawbreaker, Doom Chomper and Frostbiter

```text
Chomper | Doom-shroom | Doom Chomper
Chomper | Ice-shroom | Frostbiter
Chomper | Squash | Jawbreaker
Doom-shroom | Squash | Doom Squash
```

The complete local source archive contains exactly one recipe per result. All the ingredients were already available. All four recipes use the common flow for both card orders and glove directions: native terrain check, preview, highlight, cost and cooldown, generation before ingredient consumption, and obtaining water lilies, pots, and pumpkins upon fusion. Instant ingredients are not delayed. Exactly four completed lines were removed from `fusionen.txt`; Follow-up fusions remain open. There are now 108 recipes active; 560 recipes for 471 results remain open. `fusionen-quellen.json`, `workflow.md` and `worklfow.md` remain unchanged. There was no necessary workflow change that would save additional time.

Doom Squash and Jawbreaker use native squash states, landing, 1800 crush damage, movement and save. Doom Squash is looking in its own row. The actual source `DoomSquash.get_Range` provides a box of eight source units, the center of which moves the base method two source units forward. With a grid width of 1.35, there are approximately 118,519 pixels to the rear and 355,556 pixels to the front; the almanac shortens this to two or three fields. Upon landing, a doom explosion follows with 1800 ash damage and five source units of radius (approximately 296,296 pixels). The explosion destroys the plant stack on the actual landing field, creating a natively stored crater there for 180 seconds. An existing crater is refreshed. Neighboring plants are not removed by this crater. Damage and native explode use the common Doom-Kelp damage path. The doom kelp radius, which was previously set too large at 400 pixels, is corrected to the same conversion with 1.35 source units per column. Doom Kelp still does not create a crater.

Jawbreaker only looks for zombie columns further to the right in its own row. The actual source area is four source units wide and offset two units forward, so to about 237,037 pixels forward. The Almanac refers to this as the right neighbor cell. When landing, Jawbreaker will devour at most one suitable zombie instead of the crushing hit. The remaining targets receive the usual crush hit. Giants, bosses and target dummies are not devoured. Common vehicles remain devourable by non-giants, as in native Chomper and the source rule. Successful Devouring sets a saved return marker. Upon subsequent death, Jawbreaker returns to the current landing field for free, provided its native terrain check is successful. Like `ChomperSquash.DieEvent`, the port excludes the last column. Shovel, ingredient consumption, bungee, loading and board end do not trigger returns. Without successful devouring there is no return. The otherwise unused native Squash `mLaunchCounter` stores the marker; `mStartRow` remains reserved for the jump track.

Doom Chomper adopts native Chomper targeting filters, devour and missed bites. The actual source cast begins one source unit in front of the shadow and extends two more source units forward. This position and the range of approximately 118,519 pixels are transferred to the attack area called up by the native target seeker. Only the successful transition `ChomperBiting` to `ChomperBitingGotOne` once creates a Doom explosion a column in front of the plant. Damage, radius, loss of the plant stack there and craters correspond to Doom Squash. In the last column, as in the source, there is no explosion field outside the board. Digestion takes 60 seconds instead of the native 40 seconds and is within the saved native countdown. A bad bite, unengaged large target or load will not trigger an explosion.

Frostbiter uses the same source cast origin and one source unit increased range from `IceChomper.get_ChompRange`: three source units or approximately 177,778 pixels. Native target filters are retained. Unfrozen targets can trigger an attack, but are not devoured in the actual bite event. Successfully devouring a frozen eligible target creates a board-wide Ice Shroom effect for 20 damage and one second of freezing; Any freezing that has been present for a longer period of time is retained. The native freeze function preserves immunities and representation. After each bite, the actual method `IceChomper.BiteEvent` additionally generates a local pulse, even after successful devouring: 40 damage, 15 seconds of cold and 51 points in the existing joint freezing state, radius two source units (about 118,519 pixels) and at most one neighboring row. This expands on the abbreviated almanac description, which only mentions the pulse on failure. The almanac gives 50 points; the actual call to `Zombie.AddfreezeLevel` gets `0x33`, i.e. 51, and adds this value unchanged. The native single 40 damage bite against giants is replaced so that there is no double hit. Digestion remains 40 seconds. Targets that are already frozen or immune do not receive any additional points over the existing freezing aid. The one-second global effect heals existing Frost Tall-nuts via the existing helper.

All four variants have 300 HP. Centrally reserved memory pairs are `Squash/Doomshroom`, `Squash/Chomper`, `Chomper/Doomshroom` and `Chomper/Iceshroom`. Native types remain for terrain, AI, gauntlet, and storage; Markers distinguish variants. Synchronous contexts are restored with finalizers or `finally`. Targets are captured before splash damage with board and `DataID`. The existing freezing points remain volatile as before and are reset when charging; Plant identity, digestion, jump and return markers are saved natively. Source upgrades and unimplemented follow-up fusions are not additional core abilities.

These four plants do not produce their own flying collision projectiles. Devour, Crush, Doom Explosions, Freeze and the Cold Pulse are implemented. Original graphics, hierarchies, pivots, shadows and all 18 controller clips come exclusively from the local bundle `english-4.0.5.2/PlantsVsZombiesRH_Data/data.unity3d`: DoomSquashPrefab 68176 / Transform 91104 / Controller 6477; ChomperSquashPrefab 68169 / 91097 / 6477; DoomChomperPrefab 34840 / 93310 / 6239; IceChomperPrefab 34843 / 93313 / 6239. Two gaze clips from Jawbreaker and the Doom Chomper controller are proven to contain curves for missing transform paths; It is precisely these bonds that were ignored by the original itself that are documented. The remaining curves remain completely intact. Unity game scripts and animation events are not running. Native world movement remains the only anchor of movement. Explosions, ice particles, crater graphics and sounds use native Replanted effects.

Source documents, almanac texts, recipe completeness, prefab/script identity and asset checksums are available in the four result directories under `references/`, additionally in `doom-crater/`, `chomper-squash-native/`, `chomper-squash-animation-bindings.json` and the existing `squash-kelp-effects/` documents. Offline Evidence: Release build, recipe/asset check, workflow check, isolated squash/chomper regressions, and affected existing regressions. No game start, no playtests, no changes to profiles or savegames. Installation and version proof: `references/release-0.1.55.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.54.dll` to `Mods/FusionMod.dll`. Savegames with these variants require version 0.1.55 or a compatible newer version. LawnGlove remains unchanged.

Manual user review: all four recipes in both card orders and glove directions, cost, cooldown, terrain, underlays, pumpkins, preview, highlighting and cancel. Check Doom Squash for target area, landing position, both damage events, plant loss at crater field, and crater duration. Check Jawbreaker for one-time devour, large targets, crushing of additional targets, return to landing space, occupied landing spaces, last column, and exclusion of shovel/ingredient consumption. Doom Chomper for successful and unsuccessful bites, explode one column ahead and check digestion for 60 seconds. Frostbiter for longer range, frozen/unfrozen/immune targets, 40 damage pulse, actual 51 freeze, cold, global 20 damage and one second freeze, and check large targets without double bite. Original art and all animations, pause, gauntlet, bungee, death, pool reuse and save/load during jump, digest and check after devour.

## Change 0.1.54: Frost-shroom, Flame-shroom, Soot-shroom and Fume-spreader

```text
Fume-shroom | Ice-shroom | Frost-shroom
Fume-shroom | Jalapeno | Flame-shroom
Doom-shroom | Fume-shroom | Soot-shroom
Fume-shroom | Threepeater | Fume-spreader
```

The complete local source archive contains exactly one recipe for each of these four results. All the ingredients were already available. Common recipe resolution supports both card orders and glove directions. Placement check, cost, cooldown, preview, highlighting, generation before ingredient consumption, and obtaining water lilies, pots, and pumpkins remain in the existing flow. The four fully completed lines are removed from `fusionen.txt`. There are now 104 recipes active; 564 recipes for 475 results remain open. `fusionen-quellen.json`, `workflow.md` and `worklfow.md` remain unchanged; A workflow change does not save any necessary time here.

All four variants have 300 HP and use the native Fume-shroom identity for terrain, sleep, coffee bean, bungee, glove and save. The pairs `Fumeshroom/Iceshroom`, `Fumeshroom/Jalapeno`, `Fumeshroom/Doomshroom` and `Fumeshroom/Threepeater` are centrally reserved. `mLaunchCounter` saves the attack rate or reloading. The `mStartRow`, unused at Fume-shroom, saves the running original animation. Initialization, reset, save and load are checked using the native methods. The variants only replace `UpdateShooter`; the common sleep, wake, and bungee checks in `UpdateAbilities` remain in effect. Native `Fire` callbacks do not produce a double hit. Rest, sleep and carrying stop the attack.

Frost-shroom deals 40 piercing damage every 1.5 seconds. Each hit causes native cold and five freezing points in the existing common freezing state. The threshold starts at 100 points, increases by 100 after a freeze is triggered, and ends at 1000. Native immunities apply; Targets that are already frozen do not receive any additional points. There is no additional Ice Shroom damage or Snow Shoot damage multipliers. Frost-shroom and Flame-shroom use the actual source range from `shadow.x` to `shadow.x + 7`, The actual grid function `Mouse.GetColumnFromX` divides by 1.35 source units per column. At 80 pixels per replanted column, this results in about 414,815 pixels forward. The document is under `references/fume-grid/`. Dead zombies, friendly zombies, flying targets, tombstones and dummy targets are excluded; the native fume-shroom damage category determines further target states.

Flame-shroom creates 33 individual fire hits per attack, each with base damage. The sequence begins at 350 ms from the original event and runs at 50 Hz until the end of the one-second attack clip. Each hit removes cold and sets the existing overheat condition: subsequent damage increases by 50 percent; the existing death path creates its explosion and transmission. Integer native damage values ​​are preserved. The flame is not an additional flying projectile. Original clips `idle`, `shoot` and the independent sprite animation `fire` are embedded.

Soot-shroom only attacks with a regular left click. The hit occurs after 350 ms and deals 1800 piercing ash damage to all eligible zombies in front of the plant in the same row. There is no upper range limit, no crater and no plant damage. Once triggered, the plant recharges for 30 seconds; `rest` shows this condition. Clicking again while attacking or reloading does not produce another hit. Shovels, gloves and foreign players do not trigger an attack. The full original clip lasts 1.55 seconds.

Fume spreader hits your own and both neighboring rows forward over their entire length. Each hit deals 40 piercing damage. In the top or bottom existing row, your own row deals 80 damage, the adjacent row still deals 40. The original code `ThreeFume.DelayShoot` waits an additional 200 ms after the 350 ms animation event; Damage therefore occurs after 550 ms. The attack cycle is 1.5 seconds. The full original clip lasts approximately 1,017 seconds and is mapped to 102 simulation sticks. The edge rule uses the actual number of rows and also applies in the pool.

None of these four plants produce a flying collision projectile. Smoke and flames deal direct penetrating hits; their abilities and status effects are implemented. Native FumeCloud particles and Fume sounds replace the foreign Unity particle systems. The three documented particle emissions in the fume spreader clip are expressly replaced by the three native smoke clouds. The original plant sprites, hierarchies, pivots, shadows and all ten controller clips come from `english-4.0.5.2/PlantsVsZombiesRH_Data/data.unity3d`: IceFumeShroomPrefab 70222 / Transform 100393 / Controller 6610; FireFumePrefab 70230 / 100401 / 6274; DoomFumePrefab 70203 / 100374 / 6594; ThreeFumePrefab 70217 / 100388 / 6283. The existing import renderer handles both additive and replacement shot poses. Unity game scripts and animation events are not running.

Evidence: Almanac, all result recipes, actual methods, prefab/script identities and export checksums under `references/frost-shroom/`, `flame-shroom/`, `soot-shroom/` and `fume-spreader/`; additionally `fume-base/`, `fume-shooter/`, `fume-spreader-sequence/` and `fume-native/`. The freezing points and overheating use the existing volatile zombie states and are not restored when loading as before. Plant identity, attack phase and reload are saved natively. Global source upgrades are not additional base abilities of these recipe variants.

Offline Evidence: Release build, recipe/asset check, workflow checks, isolated fume regression check, and existing poison, fire, and freeze regressions. No game start, no playtests, no changes to profiles or savegames. Installation and version proof: `references/release-0.1.54.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.53.dll` to `Mods/FusionMod.dll`. Savegames with these variants require version 0.1.54 or a compatible newer version. LawnGlove remains unchanged.

Manual user review: all four recipes in both map orders and glove directions, cost, cooldown, terrain, support plants and pumpkins, preview, highlighting and cancel. Check Frost-shroom for 40 damage, cold, five freeze points and immunities. Check flame-shroom for complete flame sequence, overheating, cold removal and death explosion. Soot-shroom on left click, 1800 damage, entire front row, 30 second reload and check shovel/glove separation. Check fume spreader on three rows, delay, entire front row and 80/40 damage on both edges on five and six rows. Check original art, sleep/coffee bean, pause, bungee, death, pool reuse and save/load during attack and reload.

## Change 0.1.53: Starry Squash, Solar Squash, Spicy Kelp and Doom Kelp

```text
Squash | Starfruit | Starry Squash
Squash | Sunflower | Solar Squash
Tangle Kelp | Jalapeno | Spicy Kelp
Tangle Kelp | Doom-shroom | Doom Kelp
```

The complete local source archive contains exactly one recipe per result. All the necessary ingredients were already available. All four recipes support both card orders and glove directions via common recipe resolution. The result identifier is set before controller creation; Ingredients are only consumed after successful production. Card cost, cooldown, glove cost, preview and highlighting will remain in the existing common flow. Rootstocks and pumpkins remain. Kelp results use Tangle Kelp native water testing; the recipe does not bypass any terrain testing. Immediate plants are not artificially delayed. Exactly four completed lines are removed from `fusionen.txt`. There are now 100 recipes active; 568 recipes for 479 results remain open. `fusionen-quellen.json`, `workflow.md` and `worklfow.md` remain unchanged.

Starry Squash uses the native squash target search in the occupied area from half a square on the left to one square on the right, as well as the saved native jumping sequence. The central crushing hit deals 900 damage instead of 1800. In the same landing event, four circles follow, each with a source field radius and 900 damage to the occupied offsets (+/-1.5; +/-0.5) source fields. The additional hits affect at most the adjacent rows; a large target can touch multiple circles. Flight targets and allied zombies remain excluded. Native shields and damage rules remain. The port does not generate an additional fire wave. The original controller contains the five bodies and all five idle, gaze, jump and landing clips. Native world movement and original pose are applied only once.

Solar Squash deals the usual 1800 crush damage. After successful native death, the plant attempts to return to its current land column. The first return costs 50 sun, each subsequent return costs 25 more. The original planting crevice is not mistakenly used as a landing position. The target area is checked natively again. A failed generation refunds the reserved costs; Even in the event of an exception, the reservation will not be lost. If no return is successful, two normal sun pickups with a total of 50 sun are created. As in the actual `SunSquash.DieEvent`, this last branch also applies to shoveling; Shoveling does not trigger a return. Fusion consumption, carry, bungee, loading, repeat death, and board end do not trigger return or refund. Death by crushing is also recorded. `mLaunchCounter` stores the return generation; `mStartRow` remains expressly reserved for the native jump trajectory. All six original clips are embedded; the continuous additive solar rotation runs in addition to the respective squash pose.

Spicy Kelp and Doom Kelp retain native aquatic placement, targeting rules, grabbing, drowning, and removal. Spicy Kelp deals an additional 1800 ash damage across your entire row upon death, removes cold, and heals existing Flame Nuts via the shared Fire Helper. Doom Kelp deals an additional 1800 ash damage in a radius of five source squares. This radius comes from the actual `BoardAction.SetDoom` method, not the smaller native Replanted Doom radius. There are no craters or plant damage. Dead targets, allied targets and targets belonging to foreign boards are excluded. Both variants use stable targeting snapshots with `DataID` to prevent damage callbacks from hitting reused zombies. Only successfully completed death triggers the effect; Fusion consumption, loading, carrying and board end are excluded. The saved native type/marker pairs reconstruct behavior and graphics after loading. There is no such thing as a fleeting skill state.

None of these four plants shoot their own flying projectiles. The five crush areas, recurrence and kelp death blasts are fully implemented as abilities. Grabbing and Drowning remain native abilities. Original graphics and all controller clips come exclusively from `english-4.0.5.2/PlantsVsZombiesRH_Data/data.unity3d`: StarSquashPrefab 68167 / Transform 91095 / Controller 6478; SunSquashPrefab 68180 / Transform 91108 / Controller 6479; FirekelpPrefab 75434 / Transform 93164 / Controller 6503; DoomKelpPrefab 75433 / Transform 93163 / Controller 6503. FirekelpPrefab actually carries the base script `Tanglekelp`; the separate fire death path is occupied in `Jalakelp.DieEvent`. This different prefab and behavioral identity is documented. Unity game scripts are not running.

Kelp uses its original water surface as a footprint because its shadow object does not contain a sprite. The second additive controller layer controls the original gripper arms; their independent group follows the actual stored native grab target instead of the random scene position in the prefab. The native drowning process handles water/zombie effects; the foreign SpriteMask game object is not instantiated. Explosion particles, water/drowning effects, and sounds use native Replanted effects. This is an effect replacement; the original plant sprites, hierarchies, pivots and curves remain embedded. Documents and asset checksums are located in the four result directories as well as `references/squash-kelp-effects/`, `references/spicy-kelp-factory/` and `references/squash-kelp-native/`.

Offline Evidence: Recipe/Asset Audit, Workflow Audits, Affected Isolated Regressions, Visual Audit of Original Exported Assets and Release Build. Research now opens PE files with `fast_load=True`; Metadata and code areas remain identical, without unnecessary PE directory analysis. No workflow change is required for this. No game start, no playtests, no changes to profiles or savegames. Installation and version proof: `references/release-0.1.53.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.52.dll` to `Mods/FusionMod.dll`. New saves with these variants require version 0.1.53 or a compatible newer version. LawnGlove remains unchanged.

Manual user review: all four recipes in both map and glove directions, cost, cooldown, terrain, underlays, pumpkins, highlighting and canceling. Check Starry Squash for five hit areas, big targets and all jumping phases. Solar Squash on landing position, cost sequence 50/75/100, missing sun, occupied target spaces, shovel refund and check death by crushing. Check both kelp variants in the water for grabbing, drowning, death from other causes, fire series or doom radius, missing crater and undamaged plants. Check original graphics, additive animations, pause, glove movement, loading during a jump or grab, and pool reuse.

## Change 0.1.52: Butter-nut, Umbrella Husk, Torchflower and Land-mine

```text
Kernel-pult | Wall-nut | Butter-nut
Kernel-pult | Umbrella Leaf | Umbrella Husk
Sunflower | Torchwood | Torchflower
Potato Mine | Sea-shroom | Land-mine
```

For each result, the complete source archive contains exactly that one recipe. All ingredients were already available before this change. Common recipe resolution supports both card orders and glove directions as long as the terrain allows for result identity. Cost and cooldown still belong to the incoming card or glove. The result identifier is set before controller creation; Only a successfully produced result uses the ingredients. Rootstocks and pumpkins remain. In particular, land-mine remains a landmine: the sea-shroom recipe does not bypass the native water test of the result. There are no blocked alternative recipes. Exactly four completed lines were removed from `fusionen.txt`; Follow-up fusions remain open. Now 96 recipes are active; 572 recipes for 483 results remain open. `fusionen-quellen.json`, `workflow.md` and `worklfow.md` remain unchanged.

Butter-nut has 4000 HP. Each actual bite butters the attacker for two seconds, including the final bite. Skipped bites don't trigger anything. The shared feature `ButterEffects.Apply` first uses the native butter feature for immunities, graphics, and suspended zombie actions. Only an actually accepted stun receives the duration of 200 simulation ticks; an already long anesthesia is not shortened. The original art includes all three damage levels and both controller clips.

Umbrella Husk has 1000 HP and retains native protection from bungee zombies and catapult projectiles. Contact with a suitable zombie on the same row triggers a pulse within a radius of two source squares no more than once per second: 40 damage, two seconds of butter and immediate knockback of three source squares. Boss and vehicles retain the existing exclusions. Without the Umbrella Pot, which has not yet been implemented, exactly these basic values ​​apply; its additional reinforcement is not anticipated. As in the actual base class `CornUmbrella.DamageHealth`, a pulse costs 100 HP for one, 150 HP for two to four, or 250 HP for at least five zombies hit. These costs already apply to Umbrella Shell and remain there. Idle and full block animation are from the original controller.

Torchflower has 300 HP and produces 25 suns at a nominal interval of 25 seconds, with the existing documented spread of 23.75 to 26.25 seconds and native first production delay. Production occurs 500 ms after the start of the brightness pulse. With each production, all living plant projectiles within two source spaces also disappear; For each one, a physical pickup with exactly five suns is created at its position. The actual method `TorchSunflower.ProduceSun` uses radius 2, although the almanac states radius 1.5. Zombie peas, basketball bullets and projectiles from other boards are not converted. The altitude counts towards the distance. Pause, Carrying, Garden and I-Zombie stop production. The existing `SolarShooter.DropSun` route maintains collection through Mouse and Sun Collector. Torchflower does not have an additional Torchwood ignition.

Land-mine has 300 HP and is instantly armed. After every 29 to 31 seconds the mine grows, at most twice. The starting level deals 900 damage, both higher levels do 1800 damage. The first two stages use the original small mine radius from a source square and only hit the same row. The final stage uses `MidBombPotato` with radius two and is allowed to hit adjacent rows. These actual code values ​​go beyond the abbreviated almanac rating of 0.74 and 1.5, respectively. Contact and crushing trigger the explosion; a regular death, shoveling, and ingredient consumption do not produce an additional explosion. The original size grows from 0.15 to 0.20 and 0.25 in 250 ms. The native placement remains the movement anchor; the contact area grows with the original collider. The original light flashes every two seconds for 100 ms. Fertilizer does not reset the independent growth timer of this already armed mine.

None of these four plants shoot their own flying projectiles. Butter and mine explosions use native effects; the solar transformation consumes existing projectiles. That's why there's no missing new bullet type. The full original models and all controller clips were exported from the local 4.0.5.2 bundle and embedded: `CornNutPrefab` 49492, Transform 100805, Controller 6582; `CornUmbrellaPrefab` 40547, Transform 117220, Controller 6546; `TorchSunflowerPrefab` 46022, Transform 96764, Controller 6493; `SeaMine_landPrefab` 73148, Transform 96844, Controller 6388. Pivot, hierarchy, shadow and sorting are preserved. Unity game scripts not loading. Source recipes, Chinese almanac, prefab/script identity, current methods and export checksums are under `references/butter-nut/`, `umbrella-husk/`, `torchflower/`, `land-mine/`, `butter-nut-behavior/`, `torchflower-base/`, `torchflower-producer/` and `butter-torch-native/`.

The new saved pairs are `Wallnut/Kernelpult`, `Umbrella/Kernelpult`, `Sunflower/Torchwood` and `Potatomine/Seashroom`. Health, identity, butter state and production timers remain native memory fields. Land-mine stores growth level in `mStartRow`, remaining size change in `mTargetX`, growth timer in `mStateCountdown`, blink interval in `mLaunchCounter` and blink duration in `mShootingCounter`. Initialization, reset, serialization and loading of these fields as well as the actual callers are documented under `references/butter-torch-native/`. After loading, the representation reconstructs this identity and the current state. As with the existing Solar Shooter, the identifier of 5-sun pickups that have already been dropped remains fleeting: after saving/loading, such a pickup reverts to the native SmallSun value. The plants then produce correctly marked 5-sun pickups again.

Offline Evidence: Recipe/Asset Review, Tool Regressions, Affected Isolated C# Regressions, Original Asset Previews and Release Build. No game start, no runtime tests and no changes to profiles or savegames. Installation: `Mods/FusionMod.dll`, version 0.1.52, over `tools/release.py`; Version and binary proof in `references/release-0.1.52.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.51.dll` to `Mods/FusionMod.dll`. New variants require version 0.1.52 or a compatible newer version when loading. LawnGlove remains unchanged.

Manual user review: all four recipes in both map orders and glove directions, cost/cooldown, preview, highlight, cancel, underlays and terrain. Check Butter-nut with normal, skip and lethal bite as well as butter immunity and prolonged stun. Check Umbrella Husk for contact radius, recoil, butter, out-of-pocket costs, bungee and catapult protection. Check Torchflower for production rhythm, radius limits, flight altitude, enemy projectiles, exactly five suns per converted projectile and Sun Collector. Check Land-mine immediately after fusion and after both growth stages for damage, radius, contact, crushing and common death. Original art, check all damage levels, block animation, growth, pause, carry, death and save/load.



## Change 0.1.51: Foul-nut, Star-nut, Pea Squash and Iron Squash

All four source recipes are implemented: Garlic + Wall-nut, Starfruit + Wall-nut, Peashooter + Squash and Squash + Bucket. Both sequences work via the existing recipe resolution; Bucket comes from magnet shroom or a discarded iron find. All ingredients were available prior to this change. There are no locked alternative recipes for these results. The four completed lines are removed from `fusionen.txt`; Follow-up fusions remain open. The source archive and both workflow files remain unchanged.

Foul-nut has 4000 HP. Each actual bite poisons the attacker for ten additional seconds and increases their poison value by one, up to a maximum of 30. The native saved disgust animation takes over the subsequent row change including terrain check. Poison deals 20 damage per poison value per second and bypasses the second armor. It uses the same condition as Poison Pea, but does not create a cloud of miasma when bitten. The joint implementation now takes the documented `SetPoison` call into account: The sixth and every subsequent pea hit also increases the poison value instead of stopping at five. As before, toxic conditions remain volatile and are not restored upon loading.

Star-nut has 4000 HP and doesn't look for targets. A successful bite, zombie pea, basketball hit, burning zombie jalapeno head, crush, or area explosion triggers five stars of 20 damage each. The original angles are 30°, −30°, 180°, 90° and −90°; the backward star flies at six instead of five source cells per second. Shovel, Fusion Consumption, Damage Suppressed, and later removing an already fatally struck specimen do not trigger an additional shot. The mod observes the actual native health writers. With this non-shooting wall-nut base identity, `mLaunchCounter` saves the health value that has already been acknowledged and prevents double fatal hits.

Pea Squash uses the native squash jump and deals 1800 crush damage when landing. Three complete pea rings are then created with a total of 108 projectiles, each dealing 20 damage. The rings each contain 36 angles ten degrees apart and fly with five, six or seven source cells per second. Native `ProjectileMotion.Star` handles flight, row switching, collision and storage. The peas retain native Torchwood conversion; additional vertical boundaries also remove vertical and converted peas outside the field of play.

Iron Squash has 1000 HP and deals 3240 crush damage. A bucket heals 500 HP via the existing magnet/found/glove paths, up to maximum. The native jack-in-the-box blast removes plants immediately; for Iron Squash, this exact path is mapped to 900 damage, equivalent to 50 percent of the original cherry damage. The first hit at full health leaves 100 HP remaining and does not count the surviving plant as eaten. Other distances and crush damage remain responsible. Both new squash variants capture targets half a cell to the left to one cell to the right and receive native jump states, terrain rules and save fields.

The stored pairs are `Wallnut/Garlic`, `Wallnut/Starfruit`, `Squash/Peashooter` and `Squash/Magnetshroom`. They are set before controller creation. The existing map, glove, preview, highlighting and consumption paths remain shared responsibility. Water lilies, pots and pumpkins remain. Flying Peas and Stars exclusively use native saved simulation fields; The original representation is reconstructed after loading from type and movement. There is no volatile damage or ownership table for these projectiles.

Original assets and behavior come exclusively from the local 4.0.5.2 inventory: GarlicNutPrefab 49500 (WallNut), StarNutPrefab 49516 (StarNut, not Tower_starNut), PeaSquashPrefab 68174 (PeaSquash), IronSquashPrefab 68173 (Squash), Bullet_pea 33962 and Bullet_star 33973. Both nuts use controller 6582 with two clips; Both Squash models use Controller 6477 with all five idle, glance, jump and landing clips. The original hierarchy, sprites, pivots, damage bodies and shadows are embedded. Stars additionally receive the original scripted rotation of their visible body. Native hit particles and sounds replace Unity game scripts and particle effects. Asset checksums and complete source documents are located in the `references/` directories of the same name, under `references/retaliation-projectiles/` and `references/nut-squash-native/`.

Offline evidence: static recipe/asset check, workflow checks, affected isolated regressions and release build. No game start, no playtests and no changes to profiles or savegames. Installation and version proof: `references/release-0.1.51.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.50.dll` to `Mods/FusionMod.dll`. Savegames with the new variants require version 0.1.51 or a compatible newer version. LawnGlove remains unchanged.

Manual user review: all four recipes in both map and glove directions, bucket selection and feeding, cost/cooldown, cancellation, highlighting, terrain, and support plants. Check foul-nut including last bite, row changes, poison stacking and interaction with poison pea. Check Star-nut against bites, zombie peas, basketball, explosion and crush; Shovels and Fusion are not allowed to trigger stars. Pea Squash for 108 peas, damage, all directions, torchwood and check distance on edge. Check Iron Squash for 3240 damage, 500 HP healing and 900 damage from Jack-in-the-Box. Check original art, all damage levels, jump animations, pause, death, pool reuse, and save/load.

## Change 0.1.50: Poison Pea, Poison Repeater, Split Poison and Gatling Poison

```text
Garlic | Peashooter | Poison Pea
Garlic | Repeater | Poison Repeater
Peashooter | Poison Pea | Poison Repeater
Garlic | Split Pea | Split Poison
Peashooter | Poison Repeater | Split Poison
Poison Pea | Repeater | Split Poison
Garlic | Gatling Pea | Gatling Poison
Peashooter | Split Poison | Gatling Poison
Poison Pea | Split Pea | Gatling Poison
Poison Repeater | Repeater | Gatling Poison
```

All ten source recipes have been implemented. First, Poison Pea was available from existing native ingredients; after that, Poison Repeater, Split Poison and Gatling Poison were implemented in this dependency order. This means that all alternatives for these four results are available. Both orders apply to cards and glove. The common fusion path produces the result with its memory identifier before ingredient consumption, receives documents and pumpkins, and uses the cost or cooldown of the incoming card or gauntlet. Loading and scripted creation do not trigger fusion. The ten completed lines are removed from `fusionen.txt`. There are now 88 recipes active; 580 recipes for 491 results remain open. Source archive and workflow remain unchanged.

### Abilities and missiles

All four plants have 300 health and use native homing and the normal firing cycle of nominally 1.5 seconds. Poison Pea shoots one, Poison Repeater shoots two, and Gatling Poison shoots four garlic peas. Split Poison uses the same pre-existing coupled homing as the other Split variants: one front shot and two rear shots; rear peas rebound on the left edge. The original animations follow the native shooting phase. They do not generate additional projectiles.

Each garlic pea deals 30 direct damage and then gives the target a poison level. The actual source `Zombie.AddPoisonLevel` starts with five stages; only the sixth hit starts five seconds of poisoning. Additional hits extend this duration by five seconds each. Poisoning causes 20 damage per poison level immediately on the next simulation tick and every second thereafter, usually 100. Freezing lasts the remaining poison duration, but not the poison damage. After expiry, the stages are reset. The poison damage and cloud damage bypass shields; Helmets continue to take damage. This corresponds to source path `ApplyDamage` for damage type 4, which skips the second armor and deals with the first armor first. Native damage states and the existing overheat gain are retained. Allies, dead targets, and alien boards do not receive new poison levels. Boss, tombstone, and target dummy retain direct hit damage, but no poisoning.

Hitting a poisoned enemy creates a stationary poison cloud at their location. This also applies to the hit that causes the poisoning and to a fatal hit on an already poisoned opponent. There is at most one cloud per row. Additional hits do not move them or reset their lifespan. The cloud lasts six seconds. Every second, eligible zombies on the same row within 0.74 spaces gain a poison level and damage equal to twenty times the number of shots fired by all active Poison Pea Shooters on that row: 20/40/60/80 per Poison Pea/Poison Repeater/Split Poison/Gatling Poison. A cloud remains after the death of its creator; without active Poison Pea Shooters, it does not deal additional cloud damage, but still provides poison levels.

Poison Repeater can grow into Split Poison after 15 seconds with a normal left click. Split Poison grows into Gatling Poison in the same way after another 15 seconds. The shared owner `ShooterGrowth` now includes this flow already present for the Blaze family; the existing Blaze rules remain. The countdown is in the natively stored field `mStartRow`. Break, carrying, bungee, garden and loading phases stop it. Only a successfully created and placed result consumes the previous plant; The new plant retains any damage sustained.

The original bullet is `Bullet_pea_garlic`, source bullet ID 177. Common Torchwood does not convert it: the actual source path only accepts common and ice peas. `Bullet_firePea_garlic` belongs to another, not yet available Torchwood fusion and is not incorrectly created here by ordinary Torchwood. This means that no species of these four plants is missing. Your moving projectiles use the original sprites and native collision, movement and rebound controls. Poison clouds are area effects, not additional flying projectiles.

### Original assets, sources and storage

Graphics and animations come exclusively from the local bundle of `english-4.0.5.2`. Original objects: Poison Pea `GarlicPeaPrefab` 37179, Transform 120497, Controller 6360; Poison Repeater `GarlicRepeaterPrefab` 54442, Transform 110837, Controller 6267; Split Poison `GarlicSplitPrefab` 37180, Transform 120498, Controller 6363; Gatling Poison `GarlicGatlingPrefab` 38419, Transform 107623, Controller 6294; Bullet `Bullet_pea_garlic` 34023, Transform 117796. Repeater and Gatling actually also use the `GarlicPea` script; Split uses `GarlicSplit`. All sprites and both original clips of each plant are embedded. The occupied orphaned bindings of controllers shared with Blaze are exported with the same narrow exceptions. The asset previews were visually inspected offline.

Recipes, local Chinese almanac, current method addresses, disassembly, prefab/script identities and export checksums are below `references/poison-pea/`, `poison-repeater/`, `split-poison/`, `gatling-poison/`, `poison-projectile/`, `poison-base/`, `poison-effects/`, `poison-status/` and `poison-miasma/`. The miasma lambdas were resolved via the actual metadata of the local DLL. Your row check and serialized cloud values ​​are archived. Foreign Unity game scripts will not run. The original cloud uses a Unity particle emitter; in the port, the native replanted effect `FumeCloud` visualizes the cloud during generation and each wave of damage. Impact and noises also use the native effects. This is a documented effect replacement, not a carryover of the original particle system.

The new unique memory pairs are `Peashooter/Garlic`, `Repeater/Garlic`, `Splitpea/Garlic` and `Gatlingpea/Garlic`. Identity, health, shot phase and growth countdown remain natively saved. After loading, the representation is reconstructed from these identifiers. Poison levels, ongoing poisoning, poison clouds and the origin of poison bullets already flying are volatile mod states and are not saved. Shots generated after reloading have full poison functionality again; Previously saved flying projectiles lose their special poison detection. Board and `DataID` protect against pool reuse. Death, reset, board removal and mod deinitialization clean up the associated state; a reset board counter discards old toxic states.

### Offline proof, installation and manual testing

Release build, recipe/asset matching, 13 tool tests, and all twelve isolated C# regressions run without game code. The new regressions check all ten recipes in both orders, save identities, direct damage, poison levels and time limits, freezing, poison clouds and row scaling, radius limits, lethal hits, native projectiles, context recovery, pool reuse, board removal, and both click upgrades. The existing blaze regressions secure the common growth path.

The game didn't start. Profiles and saves have not been changed. The user is responsible for play testing: all recipe alternatives in both card orders and glove directions, cost/cooldown, preview/marking, documents on water/roof, shot numbers and split rebound, shields/helmets, poisoning including freezing and overheating, cloud radius and scaling, fatal hits, click growth, carry/pause as well as the documented save/load limits.

Installation: `Mods/FusionMod.dll`, version 0.1.50, over `tools/release.py`. Proof: `references/release-0.1.50.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.49.dll` to `Mods/FusionMod.dll`. Only load saves with the new IDs again with a suitable mod version. LawnGlove remains unchanged. `worklfow.md` still refers to the unchanged `workflow.md`.


## Change 0.1.49: Thorn-nut, Ice Mine, Hypno Mine and Chip Mine

```text
Cactus | Wall-nut | Thorn-nut
Ice-shroom | Potato Mine | Ice Mine
Hypno-shroom | Potato Mine | Hypno Mine
Jalapeno | Potato Mine | Chip Mine
```

The complete source archive contains exactly one recipe for each of these four results. All the necessary ingredients were already available. Cards and Glove both use orders over the existing recipe resolution. Result and memory identifier are generated before ingredient consumption; Water lilies, pots and pumpkins remain. The cost and cooldown still come from the incoming card or gauntlet. Loading and scripted mine scatter do not trigger player fusion. Only these four completed recipe lines were removed from `fusionen.txt`; Follow-up fusions remain open. The source archive remains unchanged.

### skills

- **Thorn-nut:** 4000 life points. A normal 50-HP source bite reflects 50 damage. Replanted splits eating into native 4-HP writes; the port reflects the actual HP removed by each write, including the fatal write. Skipped bites do not deal counter damage. When flying balloon zombies are within 1.5 spaces of the same row, the plant extends its original stem. Contact destroys the balloon via native landing. Allies, dead zombies and other ranks are excluded. The original clips `idle`, `rise`, `idle2` and `unrise` and all three damage levels are used. Spike-nut's special vehicle protection is not transferred.
- **Hypno Mine:** 300 health, low Potato Mine target category. The mine grows out of the ground according to the existing mine timer of 14 to 16 seconds with the original animation. Armed contact or crushing triggers the explosion. Eligible enemies within a field radius of the same row are hypnotized; Only if the hypnosis is unsuccessful does 1800 damage occur. Boss, tombstone and target dummy are not hypnotized. An ordinary death by bites underground does not cause an explosion. The original graphic including the flashing layer is integrated.
- **Ice Mine:** 300 health, same low target category and growth time. While surfacing, an ice explosion occurs after 500 ms: 600 damage and four seconds of freezing in the 3x3 cell area. Armed contact or crushing creates another ice blast and the usual 1800 damage mine blast. The actual source `IceMine.Freeze` goes beyond the short almanac description: after every 150 ms, the ice explosion continues to spread across neighboring zombie cells. Each enemy is hit a maximum of once per ice explosion. Separate mines remain independent. Native freezing immunity is retained; Even prolonged freezing is not shortened. Fertilizer triggers the same ice ability when it spawns immediately, but not again on a mine that is already armed.
- **Chip Mine:** 300 life points. `JalaMine` inherits directly from `Plant` in the source: the plant remains edible and has no emergence or contact ignition. Actual death creates an 1800 damage mine explosion and up to seven common Potato Mines in the same row. The search begins on the right, then on the left, each time with increasing distance; the original cell remains free. Occupied and unsuitable cells are skipped. Mines created take five seconds to appear. Mine Scatter uses the same placement and context protection as Cluster Mine; its previous 3×3 area remains intact.

**None of these four source plants produce projectiles of their own.** Mine scatter produces plants. Ice and mine explosions use native `IceSparkle`/`Powie` effects and native explosion sounds. There is therefore no lack of new types of flying bullets.

### Sources, representation and storage

`english-4.0.5.2/Game Files/GameAssembly.dll`, the original bundle and the local almanac remain relevant. Recipes, Chinese description, method addresses, disassembly, prefab/script identity, controllers and export checksums are under `references/thorn-nut/`, `references/hypno-mine/`, `references/ice-mine/`, `references/ice-mine-coroutines/` and `references/chip-mine/`.

Original objects: Thorn-nut `CactusNutPrefab` 72702, Transform 103542, Controller 6567; Hypno Mine `HypnoMinePrefab` 44432, Transform 105736, Controller 6380; Ice Mine `IceMinePrefab` 44437, Transform 105741, Controller 6381; Chip Mine `JalaMinePrefab` 44155, Transform 114709, Controller 6382. All sprites and clips are embedded. Ice Mine's additional 200 ms blinking uses the two verified original sprites from serialized script object 195419 (renderer 139533, sprites 20092/11880); The export shows this script flow as a data clip. Only proven orphaned blindfolds of the Potato Mine controller shared by Hypno Mine are skipped: `PotatoMine_body/PotatoMine_eyes` is missing from the original Hypno prefab, which has `PotatoMine_eye1` and `PotatoMine_eyes2` instead. The specific exception is in the export manifesto. The individual previews under the reference folders are exclusively asset checks.

New unique memory pairs: `Wallnut/Cactus`, `Potatomine/Iceshroom`, `Potatomine/Hypnoshroom` and `Potatomine/Jalapeno`. Health, Condition, Emergence Time, Flashing and Thorn-nuts Elevation Changes use serialized native plant fields. After loading, the original graphic is reconstructed from these identifiers. Board and `DataID` ensure synchronous bite, explosion and target contexts; Ingredient consumption and board removal do not trigger additional explosions. Rest and carrying stop skills.

The ongoing ice chain waves are volatile: Damage and freezing that have already been saved are retained natively, and outstanding 150 ms follow-up waves are not saved. Board dismantling and mod deinitialization remove these waves; Board counter jumps discard old waves. Saving/loading during a chain shaft therefore remains an explicit manual check point.

### Testing and installation

Build, static recipe/asset checks, workflow checks, and affected isolated C# regressions run without loading game code. Additional regressions examine both recipe orders, memory identity, real and skipped bites, balloon contact, hypnosis and damage relapse, armed crush, the ice chain with pause and replay protection, and actual mine spread with occupied cells and context recovery on errors.

The game didn't start. Playtesting is done by the user: both card orders and glove directions, preview and marking, land/water/roof with underlays, balloons and damage levels, hypnosis including gargantuars, mine growth, fertilizer, chain waves, death/shovel, and save/load. Installation takes place as version 0.1.49 via `tools/release.py`; Proof: `references/release-0.1.49.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.48.dll` to `Mods/FusionMod.dll`. Only load saves with the new IDs again with a suitable mod version. LawnGlove remains unchanged. The content of the existing workflow was not changed; `worklfow.md` refers to `workflow.md`.

## Change 0.1.48: Solarweed, Iceweed, Scorchweed and Spike-nut

```text
Spikeweed | Sunflower | Solarweed
Ice-shroom | Spikeweed | Iceweed
Jalapeno | Spikeweed | Scorchweed
Spikeweed | Wall-nut | Spike-nut
```

All four results each have exactly this one source recipe. All ingredients were already available as native replanted plants. All four recipes apply to cards and gloves in both orders. Joint recipe resolution produces the result before ingredient consumption; The water lily, pot and pumpkin are retained during the merger. Cost and cooldown still belong to the incoming card or glove. Loading and scripted creation do not trigger fusion. The four completed lines have been removed from `fusionen.txt`. There are now 64 recipes active; 604 recipes for 503 results remain open. The source archive remains unchanged.

### Capabilities and native lifecycles

- **Solarweed:** 300 health, 20 contact damage per second. Each attack made creates exactly one collectible pickup with five suns, regardless of the number of zombies hit. An attack where the plant dies on a vehicle's tire will still spawn a pickup. Collection and Sun Collector use the existing small sun adapter; there is no direct credit for the attack.
- **Iceweed:** 300 health, 40 contact damage per second. Hits cause ten seconds of cold and ten freezing points. The existing common freezing mechanic applies: initially 100 points, then 100 more for each freeze triggered, up to a maximum of 1000. Freezing lasts four seconds; native immunities apply. No further points are created during freezing. There is no additional Ice Shroom damage and no additional Snow Shoot damage multiplier.
- **Scorchweed:** 300 health, 40 contact damage per second. After damage, surviving eligible targets gain the already implemented Overheat: 50% more incoming damage, nullified by cold, explosion on death. Native status exclusions and the existing chain explosions apply. This plant is `JalaSpike`, not the similarly named `CaltropTorch` of Torchweed; it does not alter flying peas.
- **Spike-nut:** 4000 health, normal high wall-nut target category. A normal 50-HP source bite reflects 50 damage. Replanted splits eating into native 4-HP writes; the port reflects the actual HP removed by each write, including the fatal write. Skipped bites do not deal counter damage. If the plant is rolled over natively by Zamboni or Catapult, the plant loses 500 life points and causes native tire damage of 1800 to the vehicle. She stays there as long as health is present. Normal vehicles die as a result. Gargantuar hammers and boss attacks retain their normal destruction rules. The source also halves damage to a spike pot on the same square (`CaltropNut.GetDamage`, Lambda checks source ID 1372). Spike Pot is not yet implemented and will not be replaced by a regular Flower Pot.

All four plants produce **no projectiles of their own**. The three ground plants retain `SeedType.Spikeweed`: low target category, native attack rectangles, range, enemy target selection, shield penetration, normal health and vehicle contact. Native spikeweeds and spikerocks remain unchanged. `UpdateAbilities` includes the Spikeweed flow already inline; both this path and direct `UpdateSpikeweed` calls reach `DoRowAreaDamage`. That's exactly where the 40 damage and synchronous contact context come in. Only enemies actually hit by this spike attack receive the additional effect. The own context is restored in case of nesting and errors.

Spike-nuts vehicle protection attacks `Zombie.SquishAllInSquare` and `Plant.Squish`, after native checking of row, rectangle overlap and allowed attack. Board and pool ID protect the attacker. Protection does not replace scoop, ingredient consumption, common death, or other destruction. Bite Retaliation expands the existing owner for Nut counterattacks.

### Original assets, storage and limits

Graphics, hierarchy, pivots and all clips come from `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`:

- Solarweed: `SunCaltropPrefab` 64168, Transform 99074, Controller 6222; eight nodes, five sprites, `idle` 3975 and `attack` 3974.
- Iceweed: `IceCaltropPrefab` 64165, Transform 99071, Controller 6213; seven nodes, five sprites, `idle` 3980 and `attack` 3979.
- Scorchweed: `JalaSpikePrefab` 64162, Transform 99068, Controller 6216; eight nodes, six sprites, `idle` 3964 and `attack` 3963.
- Spike nut: `CaltropNutPrefab` 67954, Transform 98078, Controller 6582; nine nodes, four sprites, `idle` 4880 and `nothing` 4882. Health selects the three original damage bodies; Healing restores the appropriate body.

All four controllers have exactly one normal base layer without a skeleton mask. The original ground plant attack clips last two-thirds of a second; event `AnimAttack` is one third. In the port, this clip runs during the last half of the native one-second cycle, so the original event corresponds to the actual native contact at countdown 25. The native simulation clock still has the hits. Foreign Unity game scripts and animation events will not run.

The three ground plants have empty original `Shadow` nodes. The visible original body therefore serves as a size reference; no external shadow graphics are added. The new static check detects empty size references before installation. Only this proven time saving was included as a brief note in the workflow.

Identities are in the native stored pairs `Spikeweed + Sunflower/Iceshroom/Jalapeno` and `Wallnut + Spikeweed`. Health, attack phase and countdown remain natively saved. The existing freezing points and overheating marks are fleeting; your previous storage limits apply. Five suns that have already been created use the same volatile origin adapter as the previous mod suns and lose the special value identifier when reloaded. There is no new permanent mod state. Break, carry, bungee, garden, loading and board removal lock their own triggers.

Source documents are under `references/solarweed/`, `iceweed/`, `scorchweed/`, `spike-nut/`, `spike-nut-effects/`, `spike-nut-lambda/` and `weed-fusions/`: all source recipes, local almanac, current method addresses and disassembly, prefab/script identities, controllers, Asset checksums and offline previews. The spike pot dependency was checked using the actual metadata lambda 918 and comparing it to 1372; Outdated proxy addresses are not proof of behavior.

Tested offline: 67 new assertions with real C# files and test doubles; 531 existing assertions; 13 tool tests; full recipe/asset matching and release build. The four original previews were visually inspected. These proofs do not load game code or confirm playtesting. No game starts; no changes to profiles or savegames.

Manually, please check all four recipes in both card orders and glove directions, cost/cooldown, low target category, contact against groups, five suns with collection, cold/freeze/immunities, overheat/chain explosion, successful and skipped bites, normal and fatal vehicle contact, gargantuar/boss, damage graphics, water/roof/documents and saving/loading during an attack.

Installation: `Mods/FusionMod.dll`, version 0.1.48. Release document: `references/release-0.1.48.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.47.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged.


## Change 0.1.47: Flame Wall-nut, Frost Wall-nut and both Tall-nut variants

```text
Jalapeno | Wall-nut | Flame Wall-nut
Ice-shroom | Wall-nut | Frost Wall-nut
Flame Wall-nut | Tall-nut | Flame Tall-nut
Tall-nut | Jalapeno | Flame Tall-nut
Frost Wall-nut | Tall-nut | Frost Tall-nut
Tall-nut | Ice-shroom | Frost Tall-nut
```

All six source recipes are implemented, in both orders for cards and gloves. First, the two wall-nut variants were implemented from available native ingredients, then the two tall-nut variants. There are no locked alternative recipes for these four results. The six completed lines are removed from `fusionen.txt`. There are now 60 recipes active; 608 recipes for 507 results remain open. Source archive and workflow remain unchanged.

The common recipe resolution still has placement, highlighting and glove. The result is created before the ingredients are consumed; The pot, water lily and pumpkin remain. Cost and cooldown come from the incoming card and existing gauntlet rules, respectively. Loading and scripted creation do not trigger fusion.

### Skills and original presentation

- **Flame Wall-nut:** 4000 life points. Each native bite actually performed deals 20 counter damage to the attacker and subsequently overheats any surviving attackers.
- **Frost Wall-nut:** 4000 life points. Each successful bite causes ten seconds of cold and ten freezing points. The existing common freezing mechanic applies: 100 points initially, 100 more after each freeze triggered, a maximum of 1000. Freezing lasts four seconds. No further points are created during freezing; native immunities remain effective. There is no additional ice shroom damage.
- **Flame Tall-nut:** 8000 health and native high block, including jump defense. Same bite retaliation as Flame Wall-nut. A Jalapeno Fire effect in her row heals for 3000 health, capped at maximum health. This also applies to the existing Spicy Squash.
- **Frost Tall-nut:** 8000 health and native high block. Same biting cold as Frost Wall-nut. An Ice Shroom effect heals 1000 health points. The actual native `DoSpecial` already includes the Ice Blast inline; this path and direct `IceZombies` calls are taken into account. Common Snow Bullets and Icy Squash do not trigger this heal.

Overheat follows local mechanic text and actual `JalaEffect` implementation: 50% more incoming damage, unlimited duration, canceled by successfully applied cold or freeze. It removes previous cold and freezing. Upon death, an explosion occurs with a radius of one field, at most one neighboring row, 100 damage and air hits. The source first deals damage and then overheats survivors. An already overheated victim therefore receives 150 damage and can trigger another explosion upon death; a victim who was not previously overheated and killed immediately does not receive a subsequent death explosion. Allied and invalid targets are excluded. Source special zombies and poison states do not exist in Replanted; Bosses, targets and tombstones will not receive new overheating. The effects use native damage, cold and explosion rendering; a separate red zombie skin is not included.

Missing native bites, for example due to ladder or cold tick, do not trigger retaliation. A fatal plant bite triggers it. Board and pool ID ensure delayed feedback. All four plants do not have their own projectiles: all intended effects are bite, damage, death or instant plant effects. Fertilizer continues to heal all layers and evolves both Elemental Wall-nuts into the appropriate Tall-nut variant after the feeding animation ends. The saved feeding identifier receives the variant; Plants that have been replaced or carried in the meantime will not be developed.

Graphics, hierarchy, pivots, damage bodies and animations come entirely from `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`:

- Flame Wall-nut: `FireNutPrefab` 49511, Transform 100824, Controller 6568; eleven nodes, nine sprites; `idle` 4856 and `fire` 4855.
- Frost Wall-nut: `IceNutPrefab` 77109, Transform 110503, Controller 6572; seven nodes, four sprites; `idle` 4861.
- Flame Tall-nut: `TallFireNutPrefab` 34405, Transform 88766, Controller 6496; twelve nodes, twelve sprites; `idle_fire` 4690 and `fire` 4687.
- Frost Tall-nut: `TallIceNutPrefab` 49276, Transform 106174, Controller 6498; seven nodes, four sprites; `iceidle` 4688.

The two different idle names only receive an export alias. The fire animation, as in the original, runs continuously on an additive layer with weight 1, without a skeleton mask, speed 1. Your real sprite reference curves are applied. Health chooses between the three original damage bodies; When healed the display changes back. Foreign Unity game scripts will not run.

### Storage, evidence and dismantling

Identities reside in the native stored pairs `Wallnut/Tallnut + Jalapeno/Iceshroom`. Health and feeding remain natively stored. Overheat markings as well as the existing freezing points are fleeting and are lost when reloading; Native cold and freezing times are saved. Death, reset, pool change, board end and mod end clean up overheating. A canceled native death call receives the mark of the still living zombie. Breaks, carrying, bungee, gardening and board dismantling do not trigger their own planting actions.

Source documents are at `references/flame-wall-nut/`, `frost-wall-nut/`, `flame-tall-nut/`, `frost-tall-nut/`, `elemental-nut-effects/`, `elemental-nut-board/` and `elemental-nuts/`: all recipes, local almanac and mechanics text, actual method addresses/disassembly, prefab script identities, Controller layers, asset checksums and offline previews. In particular, the fire plants carry the source scripts `WallNut` and `TallNutFootball` respectively; Bite Retribution is centrally located in `Zombie.EatEffect`, Fire Healing is centrally located in `BoardAction.CreateFireLine`. Outdated proxy addresses are not used as evidence of behavior.

Tested offline: 88 new assertions and 443 existing assertions, twelve tool tests, new recipe, identity, health, bite, cold, overheat, explosion, healing and lifecycle checks with the real C# files and test doubles; all existing offline regressions, tool checks, asset reconciliation and release build. These checks do not load game code. Original previews were reviewed separately. No game start, no change to profiles or savegames. Manually, please check both card orders, glove directions, sun/cooldown, normal and fatal bites, ladder, freeze/immunities, chain explosions, both healing effects, fertilizer, damage graphics, high blockage, roof/water/underlays, and save/load.

Installation: `Mods/FusionMod.dll`, version 0.1.47. Release document: `references/release-0.1.47.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.46.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged.

## Change 0.1.46: Cherry Squash, Spicy Squash, Icy Squash and Hypno Squash

```text
Cherry Bomb | Squash | Cherry Squash
Jalapeno | Squash | Spicy Squash
Ice-shroom | Squash | Icy Squash
Hypno-shroom | Squash | Hypno Squash
```

All four results each have exactly this one source recipe. All ingredients are native, usable replanted plants. Cards and glove support both orders via common recipe resolution. The result is created before the ingredients are consumed; The pot, water lily and pumpkin remain. Costs and cooldowns are still part of the incoming card or the existing glove rules. Scripted creation and loading do not trigger fusion. The four completed lines are removed from `fusionen.txt`; 54 recipes are active, 614 recipes for 511 results remain open. `fusionen-quellen.json` and the workflow remain unchanged.

All four variants retain `SeedType.Squash`, the native 300 health, jump movement, hit timing, 1800 crush damage and subsequent removal. The source adds the following skills:

- **Cherry Squash:** Advanced target search, two spaces behind to three spaces in front of the plant. When the impact occurs, the existing cherry area explosion with 1800 damage, a radius of two fields and a maximum of one neighboring row is added. As with other fusions with a fresh Cherry Bomb, completing the merger together also triggers the previous instant cherry effect. An existing Cherry-nut in the explosion area receives its existing healing.
- **Spicy Squash:** same advanced target search. Upon impact, eligible enemy zombies across the line receive an additional 1800 explosion damage. Cold and ice shackles on these targets are removed via `RemoveColdEffects` beforehand. The port uses limited damage; `BurnRow` would be unsuitable because its native call `ApplyBurn` can also instantly kill zombies with significantly more health.
- **Icy Squash:** normal target search, half a field behind to one field in front of the plant. After the normal crush damage, suitable zombies within a radius of two squares and a maximum of one neighboring row receive an additional 600 damage and four seconds of freezing. The actual method `IceSquash.AttackZombie` uses radius 2, a third of the attack value and four seconds; the almanac describes the area in abbreviated terms as 3×3. Native freezing immunity still applies. Longer existing freezing times are retained. The common freezer adapter then provides cold for ten seconds. `HitIceTrap`'s standalone 20 damage is suppressed in the narrow synchronous context; Only the planned 600 additional damage occurs.
- **Hypno Squash:** normal homing. Any eligible zombie actually hit by the native crush path will be converted with `StartMindControlled` and will then receive no crush damage. If the conversion fails or the target is excluded, the native 1800 damage remains active. Common Gargantuars are not generally immune: they also inherit the normal conversion route in the Source. Bosses, targets and tombstone zombies are not converted. Special immunized Fusion zombie classes do not have an additional Replanted equivalent.

These four plants **do not produce projectiles of their own**. All intended attacks occur on the native landing hit. Additional homing uses native zombie rectangles and damage categories instead of Unity 2D colliders. Allied, dead, unreachable targets or targets belonging to other boards are excluded. The target distances follow the field information in the local almanac. The original physics search uses square source boxes with edge lengths 3 and 8, respectively; these Unity coordinates are not interpreted unchanged as replanted fields. Terrain, jump, tracking the target, roof and water remain in the native squash flow. Explosion particles and audio are from Replanted; the foreign Unity physics and foreign audio management are not loaded.

### Original illustration and sources

All graphics, hierarchies, pivots and five original clips come from `../pvzfusion/english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`:

- Cherry Squash: `CherrySquashPrefab` 68165, Transform 91093, Animator 125697; ten nodes, eight sprites.
- Spicy Squash: `JalaSquashPrefab` 68166, Transform 91094, Animator 125698; six nodes, five sprites.
- Icy Squash: `IceSquashPrefab` 68179, Transform 91107, Animator 125711; seven nodes, five sprites.
- Hypno Squash: `HypnoSquashPrefab` 68164, Transform 91092, Animator 125696; eight nodes, six sprites.

Common original controller 6477: `idle` 4645, `lookleft` 4648, `lookright` 4649, `jump` 4646, `jumppop` 4647. The single normal base layer has no skeleton mask and no additive mix. All bindings are exported. The `jump` clip also includes a real `SpriteRenderer.m_Enabled` curve for the shadow. Exporter, Appearance and Offline Preview now directly support this curve and restore the initial visibility before each new pose. Existing models without this optional data field retain their previous representation.

The saved native phases determine the viewing direction and clip progress. The source event `AnimMove` at 0.55 seconds and `AttackZombie` at one-third of a second are associated with the native takeoff and landing locations. Native state counters continue to trigger the simulation; External Unity events are not executed. The native movement has the spatial jump trajectory. Therefore, the additional vertical body displacement of the source clip `jumppop` is not added again to this path; Original scales, face and stem curves remain active. The shadow remains hidden after the jump. The source assets are embedded unchanged; the runtime representation remains to be checked manually.

Source documents are at `references/cherry-squash/`, `spicy-squash/`, `icy-squash/` and `hypno-squash/`: local almanac, full source recipes, current method addresses, disassembly, prefab/script identities, animator, asset checksums and previews. `references/squash-effects/` and `squash-explosions/` complement `Zombie.SetMindControl`, explosion and fire line. What matters is the actual DLL and the current metadata table, not outdated addresses of generated proxy assemblies. `references/squash-fusions/` archives the native call and save paths as well as the complete original clip data. `UpdateAbilities` actually calls `UpdateSquash`; its landing hit at countdown 5 calls `DoSquashDamage`. This specific path is being expanded.

### Storage, testing and dismantling

The four identities lie in `mSeedType/mImitaterType` with markers `Cherrybomb`, `Jalapeno`, `Iceshroom` and `Hypnoshroom`. Health, Aim, Jump Phase and Countdown remain in the native saved fields. Converted zombies and freeze times are also saved natively. There is no additional permanent mod state. Synchronous landing contexts are restored on nested calls and errors. Board and pool ID prevent stale hits. Pause, carry, bungee, load and board removal lock their own triggers. Shovel, ingredient consumption, and common death do not trigger landing effects. Without the mod, saved variants will be treated like normal squashes again.

Tested offline: 81 new assertions for all recipe orders, saved identities, clearances and exclusions, target distances, area damage, full fire series, thawing, freeze immunity and extended freeze times, conversion and damage rollback, errors/nesting, pool switching, and animation phases. Additionally 362 existing assertions, tool checks, recipe/asset matching and release build. Original jump previews were visually inspected. This is purely static and offline evidence, not confirmed playtests. No game starts; no changes to profiles or savegames.

Please manually check both ingredient orders with card and glove, sun/cost/cooldown, line of sight and jump display, landing hits, extended target search in front of and behind the plant, every additional effect against individual targets and groups, immunized targets, lawn/roof/water, pot/water lily/pumpkin and saving/loading during the jump. In Hypno Squash also check a common Gargantuar and the boss; in Spicy Squash, a frozen zombie and a surviving zombie with over 1800 health outside the squish area.

Installation: `Mods/FusionMod.dll`, version 0.1.46. Proof: `references/release-0.1.46.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.45.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged.

## Change 0.1.45: Twin Marigold

```text
Marigold | Marigold | Twin Marigold
```

The only source recipe has been implemented. Both ingredients are already native, usable Marigolds. Cards and Glove use the common recipe resolution. Card cost and cooldown come from the incoming Marigold; the existing glove rules apply unchanged. The result is generated before the ingredients are consumed. The water lily, pot and pumpkin remain. Scripted spawns and loading do not trigger fusion.

Twin Marigold has 300 life points and produces two independently rolled coins per cycle. Per coin, 75% silver with a value of 100, 20% gold with a value of 300 and 5% diamond with a value of 1000 apply. The actual source method uses rolls from 0 to 99: 0-19 gold, 20-94 silver, 95-99 diamond. The interval is 20 seconds with a deviation of ±2 seconds. A cycle begins with lighting; both coins appear after 0.5 seconds. In the port, the first cycle begins with the first active skill tick. The plant has no attacks and no projectiles. The normal native Marigolds retain their original production and its limitations; Twin Marigold uses the source production rules with no native Marigold wave limit.

Replanted stores money in tens. The port therefore only credits 10, 30 or 100 internal units when collecting its own coins and corrects the board statistics accordingly. Normal coins and diamonds retain their values. The actual `Coin.ScoreCoin` path first destroys the pickup and then uses an inline monetary value. The mod records the origin before this death and limits the conversion to the synchronous `UserService.AddCoins` call. Finalizers restore nested contexts even in the event of errors. Pool ID, board and coin type prevent takeover by third-party pickups; Death, reset, end of board and end of mod clear up the origin. No money will be credited during installation and no profile will be changed.

Original rendering from `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`: `TwinMarigoldPrefab` 50942, Transform 114329, Animator 126529, Controller 6540 and original clip `idle` 4789. All 31 nodes, 15 sprites and the full original animation are embedded. The normal base layer replaces the pose; no additive layer, no skeleton mask and no ignored curves. The lighting is a separate brightness gradient corresponding to `Marigold.<Bright>d__2`; in the port it follows the simulation clock instead of the frame rate. The offline preview was visually checked. It does not confirm any depiction in the game.

The stored identity is `Marigold + Marigold` in `mSeedType/mImitaterType`. Health, production interval and pending distribution use native saved fields. Charging continues the cycle instead of reinitializing the plant. Pause, Carry, Bungee, Board Dismantling, Garden, Loading Screen and I-Zombie lock production. **The additional values ​​of coins already distributed are fleeting:** After reloading, these become normal native pickups with normal values. Collect your own coins before saving. Plant identity and pending production are saved. Coin graphics, flight movement and collection sounds are from Replanted; the original plant and animation comes from PvZ Fusion. Without a mod or after dismantling, a saved Twin Marigold is treated like a normal Marigold.

Sources and Evidence: `references/twin-marigold/` includes Almanac, Full Recipes, Prefab/Script Identity, Original Controllers, Asset Checksums, Data Assets and Previews. `PlantDataAsset` 187343 occupies 20 seconds and 300 health. `Marigold.Update/MarigoldUpdate/Produce` at 0x54dba0/0x54d760/0x54d9d0 occupy interval, both calls and probabilities. Both deferred calls use the same proven string `Produce`. `references/twin-marigold-bright/`, `twin-marigold-data/`, `twin-marigold-producer/` and `twin-marigold-native/` complement coroutine, base plant and actual replanted call/store paths. The continuous source deviation of ±2 seconds is mapped to simulation ticks of 10 ms.

Tested offline: 37 scenario assertions covering recipe, saved identity, health, probability limits, two independent coins, interval limits, flashing, continued distribution after loading, pauses and exclusions, normal Marigold, inlined credit, statistics, pool/board switching, cleanup, and nested error contexts. Additionally existing regressions, tool checks, source/asset matching and release build. No game launch and no playtests; no changes to profiles or savegames.

Manually, please check card fusion and glove fusion, cost/cooldown, original appearance and flash, both coins per cycle, values ​​when collecting including Gold Magnet, Pause/Carry/Bungee, Water Lily/Pot/Pumpkin and Save/Load before distribution. Monetary values ​​and time display are documented offline, but not yet confirmed in the game.

50 recipes are active; 618 open recipes for 515 results remain. Only the completed recipe line was removed from `fusionen.txt`. `fusionen-quellen.json` and workflow remain unchanged. Installation: `Mods/FusionMod.dll`, version 0.1.45. Proof: `references/release-0.1.45.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.44.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged.


## Change 0.1.44: Umbrella Shell

```text
Umbrella Leaf | Wall-nut | Umbrella Shell
```

The only source recipe is fully implemented. Both ingredients are native, already available plants. Both sequences work via shared card and glove processing. The cost and cooldown still come from the incoming card or the existing glove rules. The result is created before the ingredients are consumed; The water lily, pot and pumpkin remain. Scripted creation and loading do not trigger fusion.

Umbrella Shell has 4000 life points. The native type `Umbrella` protects your own and eight adjacent spaces against catapult projectiles and bungees. When it comes into contact with a suitable ground zombie on the same row, the plant will trigger an area blast within a radius of two squares no more than once per second. Each eligible zombie within takes 40 shield damage and is immediately reset three spaces, bounded by the right edge of the board. Vehicles and bosses, allies, dead or unreachable zombies are excluded. The blow costs the plant itself 100 life points for one target, 150 for two to four targets, 250 for five or more targets. These out-of-pocket costs can kill the plant. The cost is inherited behavior from `CornUmbrella.DamageHealth`, not mentioned in the short almanac text. The variant does not generate its own projectiles. The normal native screen protection also remains active; Normal Umbrella Leaves do not gain any additional health or burst attacks.

Original prefab `NutUmbrellaPrefab` 40549, Transform 117222, Animator 125774 and Controller 6553 are from `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`. 33 nodes, 23 original sprites and both `idle`/`block` 4824/4823 clips are embedded. The seven sheets alternate between their three original depictions of damage. The block animation uses the original clip with source speed 1.2. Native defensive actions use the progression of native body animation. The original controller has a normal base layer without a skeleton mask; its transitions are archived in the export document.

The `Umbrella + Wallnut` identity, health, and both ability timers reside in native storage fields. Pool reuse and board end do not require additional state management. Break, carrying, bungee hijacking, board removal and other people's boards lock the shock. Healing restores the appropriate leaf representation. Without this mod or after dismantling, a saved result behaves like a normal Umbrella Leaf.

Sources: `references/umbrella-shell/` contains almanac, all recipes, prefab components, actual method addresses, disassembly, export checksums and previews. The actual behavior path goes through `CornUmbrella.OnTriggerStay2D/BlockZombies/DamageHealth` and `NutUmbrella.BlockEffect/TakeDamage/ReplaceSprite`. These inherited methods are also under `references/umbrella-shell-combat/`; Basic plant and zombie movement under `references/umbrella-shell-base/`. The generated C# signatures are only a hierarchy aid and have outdated addresses; Only the methods determined from the current DLL are relevant. `BungeeLanding` calls native `FindUmbrellaPlant` search; `UpdateAbilities` contains the native screen flow already inline. The native search checks the type `Umbrella`, which this merger keeps permanently.

Porting Boundaries: Contact checking and area range use native Replanted target rectangles and board coordinates instead of Unity 2D colliders. The contact area corresponds to the width and X-displacement of the original collider; the source physics are not fully simulated. Source recoil is applied immediately as in `Zombie.RealKnockBack`; the port limits it to the native right edge of the board. The basic almanac gives rounded 1.5 field values; the actual methods use Radius 2 and Recoil 3. The additional source path for `UmbrellaPot` remains inactive as this support does not exist in Replanted. Native screen effects and audio replace source audio. Original animation and sprites are retained; an identical runtime representation has not yet been confirmed.

Tested offline: 36 scenario assertions for recipe order, saved identity and timer, area effect, knockback, own cost including lethal cost, target exclusions, normal screens, and pause/carry/board end. Additionally source/asset checks, tool checks and release build. No game start, no playtests, no changes to profiles or savegames. Please check both orders with card and glove, catapult/bungee protection in the 3x3 area, push against single and multiple zombies, damage sheets, healing, pot/water lily/pumpkin and save/load.

49 recipes are active; 619 open recipes for 516 results remain. Only the completed row was removed from `fusionen.txt`. `fusionen-quellen.json` and workflow remain unchanged. Installation: `Mods/FusionMod.dll`, version 0.1.44. Proof: `references/release-0.1.44.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.43.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged.


## Change 0.1.43: Display of the summoned sun sphere

User reports a graphical discrepancy between Solar-nut summoned by Twin Solar-nut versus PvZ Fusion. In 0.1.42, the correct original prefab was included, but its root motion clip was incorrectly applied to the native projectile anchor. `Round` moves the root from X=0 to X=8 within two seconds. The source animator explicitly uses `m_ApplyRootMotion=true`. The port played this shift as a local graphics offset in addition to the native motion and reset it with each clip repeat. At the scale used, this corresponds to an additional offset of approximately 422 board pixels per loop. Additionally, when transitioning from waiting to rolling, the scene position Y=1.189 dropped to Y=0. The original asset-only preview obscured these errors because its camera followed the shadow.

The sun sphere now uses a solid ground anchor: its original shadow follows the native `mShadowY`, even during reinforcement and terrain changes. The scene position and the additional root shift are not reflected as a graphic offset. The body rotation is still from the original `Round` clip. A separate animation time based on the game simulation only starts at zero when rolling; the age of the projectile including waiting time no longer determines the rolling phase. The source parameter `Speed` is taken into account: initially 2, then 0.1 less for each gain up to the lower limit of 1. There is no additional manual projectile rotation for this sphere.

Original sprites and both original clips are retained. The fix affects rendering and animation time, including ground anchoring during growth. The movement, damage, recipe and storage limits documented in 0.1.42 still apply; in particular, existing sun orbs lose their mod assignment when reloading. Other plants and rolling projectiles retain their previous appearance. Complete identical running times with PvZ Fusion have not yet been confirmed.

Credits: `references/solar-nut-ball/rendering-0.1.43.json`, original animator 126525, controller 6566 and `BigSunNut.Awake/Larger`. `tools/preview_snow_repeater.py --plant solar-nut-ball --runtime-projectile` produces an offline comparison with fixed projectile anchor under `references/solar-nut-ball/runtime-adapter-comparison.png` and a preview GIF. This preview recreates the adapter and is not a playtest. The new scenario checks animation start, projectile age independence, gain, minimum speed and terrain anchor. The tool test also verifies the root motion and speed parameters actually present in the original.

No game start, no playtests, no changes to savegames or profiles. Please check the summoned solar groove at a standstill, when rolling for the first time, over several rolling loops and after several quick reinforcements; additionally once on the roof. For further visible deviations, the exact location and type of deviation are still helpful.

Installation: `Mods/FusionMod.dll`, version 0.1.43. Proof: `references/release-0.1.43.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.42.dll` to `Mods/FusionMod.dll`. The installed LawnGlove DLL remains unchanged. Recipes and open list remain at 48 active and 620 open recipes for 517 results.


## Change 0.1.42: Chomp-mine and solar fusions, instant potato growth

Four more recipes have been implemented; Cards and Glove use the same recipe resolution and support both ingredient orders:

```text
Chomper | Potato Mine | Chomp-mine
Chomper | Sunflower | Solar Chomper
Sunflower | Wall-nut | Solar-nut
Solar-nut | Wall-nut | Twin Solar-nut
```

Solar-nut will be made available before Twin Solar-nut. The second source recipe `Twin Inferno-nut | Sunflower | Twin Solar-nut` remains expressly open: Twin Inferno-nut is not yet implemented. No substitute plant will be accepted for this ingredient. Only the four completed recipe lines were removed. There are currently 48 recipes active; 620 recipes for 517 results remain open. The source archive remains unchanged.

**Chomp-mine:** 300 health, 75 sun immediately after successful fusion, native Chomper target selection and swallow with limited range to one field. Digestion takes 20 seconds. After digestion is complete, a normal, immediately upgraded Potato Mine is created one space ahead, as long as this space is valid and free. No mine is created on the right edge, on illegal terrain or when the field is occupied; Existing plants are not removed. On regular death, Chomp-mine explodes with 1800 damage in a radius of one field. Ingredient consumption and board removal do not trigger an explosion. Chomp-mine itself is already active and does not have an underground upgrade phase.

**Solar Chomper:** 300 health, native Chomper attacks, and 40 seconds of digestion. Only the successful completion of digestion generates four normal suns, for a total of 100. Missed bites, large targets that cannot be swallowed, and simply starting digestion do not generate this bonus.

**Solar-nut:** 4000 health, 25 sun every approximately 15 seconds; After the first production, the interval fluctuates by ±2 seconds, as with existing solar mines. The middle and lower damage levels each give an additional 25 sun, death gives 50 sun. The original three damage bodies follow the current health. The source limits are strict: middle level below 2666 and above 1333, lower level below 1333. Skipped levels are not counted. Healing does not reset used bonuses.

**Twin Solar-nut:** inherits production, damage levels, and death bonus from Solar-nut. A successful native bite, zombie pea hit, crush, or explosion via the native plant radius path instead deals 50 damage and creates a value 15 sun. A failed damage check produces no bonus. Native bite and projectile paths write health directly; The conversion therefore protects against premature native death and restores reserved health even in the event of errors. Other direct distances, such as shovel and bungee, remain native distances.

A normal left-click on Twin Solar-nut costs 500 Sun, heals for 4000 health, even beyond maximum health, and creates a large sun sphere one space ahead. Before the costs are deducted, the sphere must be able to be successfully created or strengthened. If the front space is occupied or invalid and on the right edge, sun and health remain unchanged. Further clicks within a second reinforce the same stationary sphere and start this waiting period again. She then rolls straight ahead using the existing roll adapter (1.25 board pixels per tick after the first movement tick) and hits suitable ground zombies every two simulation ticks: initially 600 damage per hit. Buffs increase damage by `max(60, floor(600 * exp(-0.3 * (n - 1))))` and the original 0.3 scale by `0.1 * exp(-2 * Maßstab)`. Native Pea Collision and Torchwood Conversion are turned off for these balls. The original clips `idle_big` and `Round` represent waiting and rolling. The additional travel upgrades for 10 instead of 50 damage and 200 instead of 500 sun costs are not activated; the port does not have a corresponding upgrade status.

**Half Height:** Chomp-mine remains a Chomper for AI, swallowing, and storage. For `ZombiePea` homing only, it is temporarily treated as a low-level Potato Mine; a finalizer restores their type with board/DataID checking. The native search, collision boundary and pumpkin priority are preserved. Peashooter/Gatling Zombie Peas can't hit Chomp-mine without Pumpkin. Solar Chomper and both Sun Nuts remain at normal height. The existing low potato mines retain their previous category.

**Fertilizer:** In the selected field, buried normal Potato Mines, Peatato Mines and Solar Mines are upgraded immediately, even while standing up and under a pumpkin. Healing and growth occurs upon application, not after fertilizer animation. For normal potato mines, the native completion of getting up is called: This swaps the buried controller and sets up the armed controller including light. Simply setting the state `PotatoArmed` would leave the buried representation behind. Fusions retain their identity and ability timers. Bucket, Cluster Mine, Mine-nut, and Chomp-mine do not have a corresponding burrowed growth state and do not receive invented growth. Other fertilizer upgrades remain as they were.

### Original assets, storage and auditing

The representation comes exclusively from `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`, file `resources.assets`. Prefab/Transform/Controller: Chomp-mine `PotatoChomperPrefab` 56013/107987/6625, Solar Chomper `SunChomperPrefab` 32211/94180/6634, Solar-nut `SunNutPrefab` 54016/102846/6636, Twin Solar-nut `SuperSunNutPrefab` 42902/116351/6534, sun ball `BigSunNutPrefab` 43517/105463/6566. So the English name Twin Solar-nut denotes `SuperSunNut`, while `BigSunNut` is its sphere. All original plant clips, original sprites, damage bodies and both bullet clips are embedded. The sphere has two orphaned transform paths in the original clip 4881; Only these proven paths are ignored during export. Source almanac, full recipes, method addresses, disassembly, components, export checksums and offline previews are at `references/chomp-mine/`, `solar-chomper/`, `solar-nut/`, `twin-solar-nut/`, `solar-nut-ball/` and `new-fusions-native/`.

The stored identities are `Chomper + Potatomine`, `Chomper + Sunflower`, `Wallnut + Sunflower` and `Wallnut + Twinsunflower`. Health, digestion, production, and unique damage flags are in native fields; the storage format remains unchanged. Flying or waiting sun balls use the existing volatile rolling adapter: when reloaded, they lose origin, gain and waiting time and become native peas. Do not save while waiting or flying a sun orb. The new 15-series sun uses the same value and collection path, which is protected against board/pool changes, as the existing 5-series sun. Original audio and additional source particles are replaced with existing replanted effects. Terrain and target rectangles come from Replanted instead of Unity 2D physics; this port does not claim an identical physics simulation.

Tested offline: source/asset matching, 49 new scenario assertions with test doubles, 87 existing mine assertions, 57 existing Chomper assertions, ten tool checks and release build. No game start, no playtests, no changing of profiles or savegames. Manually, please check both orders with card and glove, costs and reimbursement, all four representations, damage levels and healing, completion of digestion, mine formation in front of an open/occupied field and on the right edge, fertilizer before/during upgrade, pot/water lily/pumpkin and saving/loading. Additionally check Chomp-mine against zombie peas without/with pumpkin as well as Twin Solar-nuts bite, projectile, crush, explosion and click behavior.

Installation: `Mods/FusionMod.dll`, version 0.1.42. Proof: `references/release-0.1.42.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.41.dll` to `Mods/FusionMod.dll`. The installed LawnGlove DLL remains unchanged.

## Change 0.1.41: Melee damage to low mines

Solar Mine and Mine-nut could be bitten endlessly in 0.1.40 without losing health. Peatato Mine had the same lockdown after the stand-up began. The cause was in the native `Zombie.EatPlant`: For `SeedType.Potatomine`, Replanted ends the bite before the health penalty as soon as the plant state is no longer `Notready`. The previous test of target selection and projectile collision alone did not capture this second lock.

All three fusion mines now take regular native bite damage. The edible state `Notready` is only provided for these variants during `Zombie.EatPlant`. A finalizer then restores the previous state even in the event of errors. `DataID` and board protect against recovery on reused objects. Health, damage amount, bite frequency and fatal bites remain in the native flow. This means that Mine-nut's damage levels and the respective death effects can also be achieved regularly. Upgrades, skill timers, and saved identities will not be reset. Existing saves do not require conversion; The patch also applies to existing mines.

The plant species remains `Potatomine` throughout the bite. Plant zombie peas continue to fly through these low plants. Normal Potato Mines and other plants retain their previous bite rules. The previous contact explosions from upgraded Peatato Mine and Solar Mine remain intact.

Source: `references/mine-bite-fix/`. `Zombie.CheckIfPreyCaught` actually jumps to `Zombie.EatPlant` (`0x6f0eb0`) at `0x6ea274`. Its termination test is at `0x6f0fd8`–`0x6f0fe5`; The regular deduction of four life points only follows with `0x6f10d6`. The research checks the real tail jump, not just ordinary `call` instructions. The offline check first reproduces the native lock and then checks all three fusions before, during and after the upgrade, state retention, fatal bites, damage levels, normal potato mines, nested calls, errors, and pool and board changes.

No game start and no playtests. Please recheck Solar Mine, Mine-nut and Peatato Mine against normal zombies, including Mine-nut's damage patterns and explosions. Additionally, check the passage of Peashooter/Gatling zombie peas without pumpkin. Static tests and an offline test with a simulated native lock do not replace this runtime test.

Installation: `Mods/FusionMod.dll`, version 0.1.41. Proof: `references/release-0.1.41.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.40.dll` to `Mods/FusionMod.dll`. Recipes and open list remain at 44 active and 624 open recipes for 520 open results.


## Change 0.1.40: Peatato Mine, Solar Mine and Mine-nut

The three resulting fusions are fully accessible via their single recipe registration from Fusion 4.0.5.2. Both orders work with ingredient cards and glove. All ingredients are already native Replanted plants:

```text
Peashooter | Potato Mine | Peatato Mine
Potato Mine | Sunflower | Solar Mine
Potato Mine | Wall-nut | Mine-nut
```

**Peatato Mine:** 300 life points; If the merger is successful, 50 sun will be credited immediately. She already shoots potato peas underground for 30 damage. The distance to the frontmost suitable zombie in the same row determines the interval: 1.5 seconds from six fields, linear to 0.5 seconds with a maximum of one field. One source unit corresponds to 80 board pixels. After 14-16 seconds, the original 0.833-second stand-up movement begins. Subsequent contact with the ground detonates the mine with 1800 damage in a radius of one field. Each regular death scatters an additional 36 potato peas ten degrees apart in all directions; these can hit other rows. The peas do not penetrate, cause an additional area explosion, and are not converted by Torchwood.

**Solar Mine:** 300 life points; If the merger is successful, 25 sun will be credited immediately. Even without a zombie it produces normal 25mm suns. The first production occurs after 15 seconds, further intervals vary by ±2 seconds as in `SunMine.ProducerUpdate`. Upgrade and contact explosion correspond to Peatato Mine. Each regular death gives an additional two normal suns, for a total of 50. If eaten before upgrading, there are these suns, but no contact explosion.

**Mine-nut:** 4000 health, immediately active as an edible low barrier. Zombie contact does not trigger self-destruction. The first time you stay in the medium damage level and the first time you fall below the lower damage limit, an explosion with 500 damage occurs. The limits are strict, as in the source code: at 4000 maximum health, the middle level is below 2666 and above 1333, the lower level is below 1333. A large hit directly into the lower level only triggers its explosion; the source does not count skipped steps. Healing does not reset triggered levels. Death causes an explosion with 1800 damage. The three original body representations follow current health.

**Half Height:** All three results explicitly use `SeedType.Potatomine`, also Mine-nut. The markers `Peashooter`, `Sunflower` and `Wallnut` distinguish the stored identities. The native collision of `ZombiePea` skips Potato Mines based on this plant species. A Peashooter/Gatling zombie can therefore shoot over it. The normal melee, rootstock and pumpkin treatment remains native; an existing pumpkin is itself still a lofty goal. No resizing of the sprite is used as a replacement for this target category.

The joint fusion processing produces the result before the ingredient consumption. Cards retain the costs and cooldowns of their incoming ingredient. The glove keeps its rules. Invalid terrain does not change ingredients. Pot, water lily and pumpkin are preserved. Ingredient consumption, failed generation, pool reset, and board removal do not trigger Death Peas, Death Explosions, or Death Suns. Refunds are solely for successful merger completion, not for plant initialization or loading.

### Original assets and evidence

Graphics, hierarchy, pivots, sprite order and original curves come exclusively from `../pvzfusion/english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`:

- Peatato Mine: `PeaMinePrefab` 71626, Transform 99523, Controller 6621; 19 nodes, 18 sprites; Clips `idle`, `rise`, `armed`, `mashed`, `shoot` 4973/4975/4972/4974/4976. The additive shot plane has weight 1 and no skeleton mask.
- Solar Mine: `SunMinePrefab` 33164, Transform 113828, Controller 6635; 21 nodes, 22 sprites; Clips `idle`, `rise`, `armed`, `mashed`, `flash` 5017/5019/5015/5018/5016. Original flashing layer sprite changes are applied separately and then reset.
- Mine-nut: `PotatoNutPrefab` 46252, Transform 98297, Controller 6626; 20 nodes, 18 sprites; Clips `armed`/`flash` 4985/4986. `idle` in the exported model is just an alias of the original `armed` clip.
- Potato pea: `Bullet_potato` 57125, Transform 118943; two nodes, two original sprites, no animator. The actual script is called `Bullet_puffPotato`. The object of the same name 60844, on the other hand, is a `PotatoSeed` and not a projectile from this merger. `Bullet_potato_explode` 57126 is also a different variant and is not included.

Sources, original Chinese almanac, actual method addresses, prefab/script mapping, export checksums and offline previews are under `references/peatato-mine/`, `references/solar-mine/`, `references/mine-nut/` and `references/potato-projectile/`. Supplementary base classes and the variant seed identity are archived at `references/peatato-bullets/` and `references/potato-projectile-identity/`. Prescription registrations: `0x47cffa`, `0x47d0f6`, `0x47d10e`.

### Storage, limits and manual review

Identity, health, upgrades, production/shot counters, and unique mine-nut damage flags are stored in existing native plant fields. The mod does not change the storage format. The marking requires this or a compatible newer mod version. Already flying potato peas lose their volatile mod origin when loaded and become native peas; new shots and death rings again use the potato behavior. At death, pool reset, board end and mod end the bullet assignment is cleaned up.

The port uses replanted zombie rectangles and board terrain instead of the source's Unity 2D physics. The potato peas move with 4.8 board pixels per simulation tick derived from six source units per second. Shots occur during the simulation cycle of firing the shot; the Unity animation event at 0.667 seconds does not run. Shooting and flashing animations are presentation data. Explosion particles and audio come from Replanted; no additional animated mashed potato object remains. The original clip `mashed` remains archived and embedded. Fusion's own zombie types and their special mine reactions are not added.

Tested offline: 51 new scenario assertions covering recipes, low saved identity, refund, upgrade, pause, target selection, firing rate, production, damage limits, healing, flag save, death ring, line switching, bullet consumption, and pool/board cleanup; 147 existing assertions; ten tool tests; static recipe/asset checking and release build. No game start, no playtests. Please check both ingredient orders with card and glove, sun/cost, grass/roof/water, pot/water lily/pumpkin, shot flight and death ring, original animations, mine-nut damage patterns and healing, save/load and peashooter/gatling zombies. When testing the height, check once without the pumpkin and once with it.

The three completed result rows are removed from `fusionen.txt`; later recipes using these fusions as ingredients remain open. Current status: 44 active recipes, 624 open recipes for 520 results. `fusionen-quellen.json` remains unchanged.

Installation: `Mods/FusionMod.dll`, version 0.1.40. Proof: `references/release-0.1.40.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.39.dll` to `Mods/FusionMod.dll`. The installed LawnGlove DLL is not changed.


## Change 0.1.39: Bucket drops when helmet is lost

Normal Bucketheads now have a **50% chance of a Bucket** if a damaging hit removes their remaining Bucket Helmet and they survive the full hit, per the specifically requested rule. The same helmet loss only rolls once. Mere partial damage and subsequent hits on the already unprotected zombie do not produce a drop. Instant kills, choking, crushing, and burning do not produce such a bucket; Even an ordinary hit that kills the helmet and body at the same time is impossible. Other helmet types and friendly zombies do not generate buckets.

**Magnetic deprivation remains:** Magnet-shroom continues to pick up its native bucket exactly. Click on this magnet shroom and plant, fuse or use the bucket for healing as before. Magnet Drain does not create a second floor drop or an additional random roll. The native magnetic path removes the helmet directly and does not invoke the observed damage hit.

A successful damage drop appears on the zombie, flies up briefly and falls to the ground. The visible graphic is the original sprite 15928 of the loose Fusion bucket prefab 59960. The bucket remains in place for eight simulation seconds and flashes after four seconds. Clicking it takes it to the existing bucket cursor. Neither life nor flight continues during selection. Right-clicking or changing tools resets the same bucket with its remaining lifespan. Successful planting, fusing, or healing consumes it exactly once. Invalid targets and failed creation keep the bucket. Pause and game speed follow the native simulation.

The 50% rule for normal bucketheads comes from the user instructions. The local Fusion 4.0.5.2 Zombie Almanac describes magnet withdrawal for normal Bucketheads (4/13); Express death drops belong to special iron plant zombies. These additional zombie types are not introduced here. The original research and the new game rules are documented separately in `references/bucket-drops/behavior.json`.

The hook belongs to the complete `Zombie.TakeDamage` flow. `TakeHelmDamage` removes the helmet before the same hit can damage the body; A drop directly into `DropHelm` would therefore occur even in the event of a fatal overflow. Board and `DataID` secure transition observation against pool reuse. No general death or loot hook is used. The native board randomness determines the 50% chance.

Loose buckets use their own temporary display anchors. These are not tied to the mouse. Before saving, selections and representations are removed; Within the same session, unused drops will then reappear with their remaining lifespan. **Loose drops are not saved in the save game and will disappear when reloading a board.** Planted buckets, fusions and native magnet items will remain saved as before. The end of the board and the end of the mod clear up the drops. Recipes, open list, existing plants and LawnGlove DLL remain unchanged.

Tested offline: 72 scenario assertions including both random outcomes, one-time rolls, helmet partial damage, lethal overflow, instant kill flags, magnetic drain without double drop, pool reuse, pick/cancel, failed placement, consumption, pause, landing, and expiration; existing Chomper regressions; static recipe/asset testing and tool testing; Release build. No game start and no playtests. User Testing: common pea hits to helmet loss, approximate drop frequency across multiple bucketheads, no drops on instant kills, magnet drain with exactly one usable bucket, ground position/click area on grass and roof, blink/expire, select/cancel, fusion/heal, and pause and save/load.

Installation: `Mods/FusionMod.dll`, version 0.1.39. Proof: `references/release-0.1.39.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.38.dll` to `Mods/FusionMod.dll`.

## Change 0.1.38: Bucket, Buck-nut and Buckshooter

Bucket is available as a standalone low plant with 2000 health. To obtain one, first have a regular magnet shroom attract a bucket from a buckethead. Click on the magnet shroom with the bucket you picked up; The original graphic of the bucket appears on the glove cursor. A free valid field plants bucket. A peashooter or a wall-nut immediately creates the respective fusion. Right-click or tool change clears the selection. The stored magnet item is only consumed after successful placement, fusion, or healing; invalid terrain and failed generation keep it. There is no additional seed card and no sun cost for the bucket picked up.

```text
Peashooter | Bucket | Buckshooter
Wall-nut | Bucket | Buck-nut
```

Both recipes also support planted buckets: the native ingredient card on bucket or both glove directions. Cards retain their normal costs and cooldowns; the glove retains its existing rules. Result occurs before ingredients are consumed. Rootstocks and pumpkins remain. Appropriate fusion and healing targets shine through the existing highlight path.

Buck-nut has 8000 health and the three original damage diagrams. Buckshooter has 2000 health and shoots an original iron pea every 1.5 seconds for 80 damage. Additionally, the hit deals damage equal to the current second armor, in Replanted `mShieldHealth`; the helmet does not count. Surviving moving ground targets are pushed back by 16 board pixels. This corresponds to the occupied 0.2 source units with 80 board pixels per field. Bosses, vehicles and Gargantuars will not be moved. Native homing, flight, hit authority and hit effects remain responsible. Torchwood does not turn iron peas into fire peas.

Another bucket heals Buck-nut and Buckshooter for 500 health, up to the normal cap. Excess health already present is not reduced. To do this, either place the magnetic bucket you have picked up on the iron plant or carry a planted bucket there with your glove. The iron plant is healed and retains its identity and shot condition. Bucket itself is not an iron fusion and does not accept such healing. Fertilizer continues to heal all three plants via the existing path, allowing their identities to persist.

### Original assets and sources

All graphics, hierarchies, pivots, sprite orders and original clips come from the local `english-4.0.5.2/Game Files/PlantsVsZombiesRH_Data/data.unity3d`. Foreign Unity game scripts will not run.

- Bucket: `BucketPlantPrefab` 40920, Transform 116194, Controller 6196; five nodes, four sprites, original clip `idle` 3897. Damaged bodies are displayed according to health.
- Buck-nut: `IronNutPrefab` 64886, Transform 98626, Controller 6582; seven nodes, four sprites, clips `idle` and `nothing` 4880/4882. The source here explicitly uses the common script `WallNut`, not a class `IronNut`.
- Buckshooter: `IronPeaPrefab` 37176, Transform 120494, Controller 6613; 16 nodes, 15 sprites, clips `idle`/`shoot` 4950/4951. The additive shot plane is retained.
- Iron Pea: `Bullet_ironPea` 33960, Transform 117733; two nodes and two original sprites. The source prefab does not have an animator.

Evidence resides at `references/bucket/`, `references/buck-nut/`, `references/buckshooter/`, and `references/iron-projectile/`, including actual method addresses, original almanac, compressed enum constants, prefab/script mapping, export checksums, and offline previews. The recipes are assigned registrations `0x9d5846` and `0x9d591a`. Source ID 224 denotes the base ingredient and does not have a fusion recipe itself. The research assistant now supports `--source-id` and checks name and ID against the unchanged recipe archive.

### Storage and porting limits

Saved pairs `mSeedType + mImitaterType`: Bucket `Potatomine + Magnetshroom`, Buck-nut `Wallnut + GoldMagnet`, Buckshooter `Peashooter + Magnetshroom`. The markers are identities, not additional ingredients. The Potato Mine type only provides the low native target/terrain category; its mining behavior is switched off for Bucket. Identity, health and firing rate are saved natively. A magnetic bucket is deselected before saving; The item that has not yet been used remains stored in the Magnet-shroom. Death, pool reuse, board change and mod end clean selection and projectile origin.

Procurement uses the native Replanted magnet flow including attraction, charge, and item duration. The fusion objects lying around with their own flight physics, flashing after four seconds and disappearing after eight seconds are not additionally recreated. Other metal objects remain in the native magnetic drain; they do not become new ingredients in this run. Iron plants can be cured using the implemented buckets.

The Source halves enemy Cherry blast damage to Iron Fusions. Replanted has no enemy cherry explosions; custom cherry bombs and previous mod cherry explosions do not damage plants at all. This native rule remains. Jack-in-the-Box and Crush are other damage types and are not nerfed for these three plants. This is not general blast immunity.

As with previous mod bullets, the origin of flying iron peas remains fleeting: after saving/loading, already flying bullets become native peas. New shots work as iron peas again. Audio and hit particles are from Replanted. Savegames with the new plants require this or a compatible newer mod version.

Tested offline: 49 scenario assertions covering recipes, identity storage, stats, selection, cancellation, failure cases, healing, gauntlet, projectile origin, shield damage, and recoil; existing Chomper exams; tool testing; static completion and release build. No game start and no playtests. User Check: Bucket pickup and cursor, all recipe orders with cards/gloves, costs, pads/pumpkins, roof/water, damage patterns, shield hits and knockback, healing, rest, save/load, board swap, and unchanged normal plants.

The two completed result rows are removed from `fusionen.txt`. Recipes with Bucket/Buckshooter/Buck-nut for further results remain open. Current status: 41 active recipes, 627 open recipes for 523 results. Source archive unchanged.

Installation: `Mods/FusionMod.dll`, version 0.1.38. Proof: `references/release-0.1.38.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.37.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged.

## Change 0.1.37: Healing before fertilization, growth after

A successful use of fertilizer now first heals all living plants in the selected cell and updates their damage display. Only then does the native fertilization animation begin. Wall-nut, free Lily Pad and Spikeweed will only be replaced after this animation actually ends. Sun-shrooms also only begin their growth afterwards. There is no estimated animation timer and no second healing after the animation. Damage during fertilization remains possible; normal replacement production continues to provide full health of the new plant.

The pending growth operation uses the already existing native `GridItemType.ZenTool` with `ZenToolFertilizer`, target field, expected plant species and native reanimation ID. This data is saved and loaded regularly by the game. A patch on the actual call path `Board.UpdateGridItems -> GridItem.Update` only handles fertilizer tools outside of the Zen Garden. It waits for the native animation to end, checks the plant, occupancy and terrain again and executes the growth. The Zen Garden inventory and profile path is not called up. Empty cells, removed or worn plants, fusion variants and now occupied water lilies are not converted. If replacement production fails, the original plant remains. After the end, the native tool is cleaned up. The previously documented loading limit of unused drop bags still applies; Growth that has already started is now saved natively.

The drop bag uses the original scaling of its native animation controller. The previous additional factor `0.45` is no longer applicable. The mouse area is expanded to fit the larger bag. Cursor size and fertilization animation remain unchanged.

Source documents under `references/fertilizer/`: native `GridItem.Init`, `GridItem.Update`, `GridItem.Serialize`, `GridItem.Deserialize`, `GridItem.GridItemDie` and the direct call in `Board.UpdateGridItems`; additional original data of the fertilizer cursor. The serialization contains both the expected plant species and the reanimation ID. Release build and static resource/recipe check successful. No game start and no playtests. The user checks visible instant healing, growth only after the fertilization animation, larger drops including click area, pause/game speed and saving/loading during the animation.

Installation: `Mods/FusionMod.dll`, version 0.1.37. Proof: `references/release-0.1.37.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.36.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged.

## Change 0.1.36: Fertilizer in combat

A bag of fertilizer regularly falls down on the house. The first drop occurs after 70.5 simulation seconds, further drops every 72 seconds. This corresponds to the actual Fusion 4.0.5.2 controller `Bg1` with its eight second clip `motion1`, the event at 6.5 seconds and `GiveFertilize.AnimGive` after every nine events. An uncollected bag will disappear after 30 seconds; During the last five seconds it flashes. The 30 seconds are taken directly from the actual `Fertilize.Update` method and its float constant. The additional flashing display creates the desired sun impression. Pause and game speed follow native 100Hz simulation.

Operation: Click on the bag on the left side of the house, then click on an occupied plant cell. Invalid targets keep the fertilizer on the cursor. Right-clicking or changing tools will reset the bag with its remaining lifespan. The time does not expire while wearing it. A successful use consumes exactly one bag and plays the native Zen Garden fertilization animation. Graphics and cursors come from Replanted; Foreign Fusion game scripts will not be executed. The display on the house uses the frozen beginning of this animation and a short falling motion. Any changed properties of the pooled animation controller are restored before it is returned.

The fertilizer fully heals all living plants in the selected cell, including fusions, rootstocks and pumpkins. Any existing health above the normal maximum is retained. Normal Wall-nuts grow into Tall-nuts regardless of previous damage. Free Lily Pads Grow into Cattails; In the case of a covered water lily, the rootstock and existing plant are preserved and healed. Spikeweed grows into spikerock. Small or already growing Sun-shrooms immediately begin the native growth animation. Fully grown Sun-shrooms are cured. Fusion variants are recognized via their common identity resolution and retain their identity. Replacement plants are created prior to removing the original plant. Terrain tests, rootstocks and pumpkins remain considered; Costs, cards and cooldowns are not consumed.

Fertilizer is available on running game boards, not in the Zen Garden. The Fusion progress lock and other non-ordered Fusion special upgrades will not be covered. No Zen Garden inventory is consumed or a profile is changed. When saving, the selection is canceled; Running bags and timers are retained during the session. Their temporary representations are removed before native serialization and then rebuilt. When loading a new board, the drop timer and bag inventory restart. Plant healing, replacement plants, and sun-shroom states use native saved plant fields.

Sources: `references/fertilizer/behavior.json`, actual fusion methods, original prefab and original clip, and native replanted methods. Release build and static resource/recipe check successful. No game start, no playtests. The user checks drop and mouse position, 72 second cycle, expiration/flashing, selection/cancel, healing and all four growth paths, damaged nuts, occupied lily pads, pumpkins, glove, pause/game speed, and save/load.

Installation: `Mods/FusionMod.dll`, version 0.1.36. Proof: `references/release-0.1.36.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.35.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged.

Target game: `/mnt/FastGaming1/SteamLibrary/steamapps/common/PVZ Replanted/`

Working directory: `/home/kevin/modding/pvzreplanted/fusion-mod/`

## Change 0.1.35: Pea-nut falling movement and proper Chompzilla graphics

Pea-nut now throws the roll nut from mouth height. For a normal 80 pixel plant, it starts 45 board pixels above its ground position, falls with 0.1 pixels per tick additional fall speed, and lands without jumping or sinking. As it falls, it moves forward at 2.5 pixels per tick; After landing, it rolls through the row at 1.25 instead of the previous 3.33 pixels per tick. The original shadow remains on the ground during the fall. Native `mPosZ`/`mVelZ`/`mAccZ` describe the height; `Projectile.Update` and the controller maintain positioning. Damage, penetration, targeting privileges and roll nut size are retained. The rolling nut only disappears when it comes into contact with the ground on a rising edge of the terrain. Chomp-shooter and Chompzilla chunks retain their previous movement behavior.

Chompzilla's graphics were mapped incorrectly in 0.1.34. `UltimateChomper` denotes Cherrizilla (ID 903) in this source; The commissioned Chompzilla has ID 1013 and is called internally `SuperChomper`. The two values ​​are taken from the compressed enum constants of the actual 4.0.5.2 metadata. The import now uses `SuperChomperPrefab` 56150, Transform 111852, Script 194393 (`globalgamemanagers.assets:163 SuperChomper`), Animator 126182 and Controller 6640. All 32 nodes, 28 sprites and the two original clips 5031 `idle` and 5030 `bite` are embedded. The head and body switch between the three original damage levels. The previously selected Cherrizilla graphic and its additional clip are no longer included in the active model. Orphaned original curves are specifically assigned using the correct prefab; old unused sprite files are removed after a successful export.

Recipes, saved plant IDs, and Chompzilla's existing abilities will not change. The description of the Cherrizilla assets used in 0.1.34 remains below as historical proof of the bug. Current identity, prefab, controller and method documents are in `references/chompzilla-0-1-35/`; the current export manifest is still below `references/chompzilla/visual-assets.json`. The previous manifest status is preserved as `visual-assets-0.1.34.json`. Supplementary roll nut sources are under `references/pea-nut-0-1-35/`.

Tested offline: 57 scenario assertions including fall movement, landing, slower roll and unchanged chomp-shooter; nine tool checks including source ID, script and imported prefab matching; static resource/recipe completion; Release build. The asset preview shows the corrected original graphic. No game start and no playtests. The user checks drop, shadow and landing on grass/roof, slower roll, multiple hits, pause/game speed, Chompzilla's appearance and damage levels, as well as unchanged chunks of the two Chomper Fusions. The known storage limit of flying mod bullets remains.

Installation: `Mods/FusionMod.dll`, version 0.1.35. Proof: `references/release-0.1.35.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.34.dll` to `Mods/FusionMod.dll`. Savegames, profiles and installed LawnGlove DLL remain unchanged.

## Change 0.1.34: Pea-nut, Chomp-shooter, Chomp-nut and Chompzilla

All six source recipes are implemented. The three dual fusions will become available first; After that, all three paths to Chompzilla are possible with the ingredients already fully implemented. Cards and Gauntlet support both ingredient orders via the common placement, cost, highlighting, and consumption paths:

```text
Peashooter | Wall-nut | Pea-nut
Chomper | Peashooter | Chomp-shooter
Chomper | Wall-nut | Chomp-nut
Chomp-nut | Peashooter | Chompzilla
Chomp-shooter | Wall-nut | Chompzilla
Chomper | Pea-nut | Chompzilla
```

**Pea-nut** has 4000 life points. Native Peashooter target search and firing are retained. A rolling nut with 20 damage is created every 1.5 seconds. It hits every eligible ground zombie in its row once and penetrates any number of targets. Crack stages use the original bodies. **Chomp-shooter** uses native chomping and native constraints for large targets. Digestion takes 30 seconds. Only during this digestion does he spit out a rolling chunk every three seconds for 80 damage and the same penetration. The first chunk appears about 0.83 seconds into the event of the original clip. **Chomp-nut** has 4000 health, swallows natively, digests for 40 seconds and heals for 1000 health at the end of digestion up to a maximum of 4000. Both original crack levels for head and back are re-selected for damage and healing.

**Chompzilla** has 4000 base health and bites every 1.75 seconds at close range. The bite hits at the corresponding time in the original clip; a target that has now disappeared results in a bad bite. The ready special bite deals 1,000,000 damage, heals up to the cap of 16000, and then gains a 40 second cooldown. During this time, Chompzilla will continue to bite for 200 damage and heal for the amount of damage dealt. Targets without a helmet remaining and with 500 or less Body Health instead take 500 damage and provide 400 healing. Each successful bite spits out a piercing chunk for 80 damage. The chunk of the enhanced normal bite knocks back living, moving ground targets by 40 board pixels. Bosses, vehicles and Gargantuars will not be moved. Life points above 4000 decrease by 200 per second of simulation; the upper limit is 16000.

Chompzilla's damage control converts native crush and the Jack-in-the-Box's plant blast into a maximum of `4000 / 3 = 1333` damage per hit. Low health can still lead to death. Ordinary numerical zombie and catapult hits are already below this limit. The native radius helper determines the blast receivers; only its synchronous `Plant.Die` path is reinterpreted. Shovel, fusion consumption, bungee and board removal remain real distances. The actual callers are statically assigned: `SquishAllInSquare` calls `Plant.Squish`; Jack-in-the-Box calls the float radius helper, which calls `Plant.Die`. Native counters for crushed/lost plants can be increased despite Chompzilla surviving; this stat difference does not affect health or prescriptions.

### Original assets, storage and limits

Graphics, hierarchies, pivots, sprite order and all clips come from the local 4.0.5.2 bundle. No foreign game scripts or colliders are executed:

- Pea-nut: `NutShooterPrefab` 41474, Transform 116621, Controller 6618; two clips, 15 nodes, eight sprites.
- Chomp shooter: `PeaChomperPrefab` 65657, Transform 92526, Controller 6619; three clips, 26 nodes, 24 sprites. The shoot clip remains additive to the base layer.
- Chomp-nut: `NutChomperPrefab` 45123, Transform 94296, Controller 6616; four clips, 20 nodes, 23 sprites including four separately loaded crack graphics. Their original ResourceManager paths are `plants/_mixer/nutchomper/cracked1_back`, `cracked2_back`, `cracked1_head`, `cracked2_head`.
- Chompzilla: `UltimateChomperPrefab` 56149, Transform 111851, Controller 6638; three clips, 42 nodes, 29 sprites. The normal bite runs over `bite`. The additional original clip `supershoot` is also exported, but is part of the additional source special attack and is not triggered for the normal recipe plant.
- Rollnut: `Bullet_nut` 50823, Transform 116654. Chunks: `Bullet_pea_chomper` 33967, Transform 117740. Both original hierarchies, each with three nodes and two sprites, are embedded. The movement rotates the visible body, the shadow remains below. Rolling motion uses 3.33 board pixels per simulation stick. The projectiles disappear on a rising edge of the terrain. Torchwood doesn't turn them into firepeas; Shields do not prevent their penetration.

As with the previous import, replanted states and simulation cycles represent the original animations; their events do not generate additional attacks. Chompzilla uses its own process on the actual `UpdateAbilities` owner for the missing native repeated close bite. Native Chomper targeting continues to determine range and targeting permissions. Source options for travel upgrades, legendary abilities, and manually activated additional attacks are not part of these normal recipe plants. Plants and bullets are imported; Audio and particles remain assigned to Replanted. The new rolling projectiles do not have any additional source sound or hit particles of their own.

Reserved Saved Pairs (`mSeedType + mImitaterType`): Pea-nut `Peashooter + Wallnut`, Chomp-shooter `Chomper + Peashooter`, Chomp-nut `Chomper + Wallnut`, Chompzilla `Chomper + Tallnut`. The Tallnut marker identifies Chompzilla and is not another ingredient. Health, identity, bite phase and special/digestion time are in native saved fields. Original representation will be rebuilt after loading. There is no additional plant timer in a volatile list. The origin and targets that have already been hit by flying rolling projectiles, on the other hand, are fleeting: When loading, existing projectiles are reconstructed as native peas. Save games preferably without flying mod projectiles; This limit corresponds to the previous origin management of other mod bullets. Death, pool reset, board end and mod end clean up runtime management.

Documents: `references/pea-nut/`, `references/chomp-shooter/`, `references/chomp-nut/`, `references/chompzilla/`, `references/nut-projectile/` and `references/chomp-projectile/`. They contain all source recipes with registration addresses, original almanac, actual methods, prefabs, controllers, export checksums and offline previews. Orphaned transform curves of the original clips are populated with full prefab path list; only these known curves are skipped. The source archive remains unchanged. The six completed result rows are removed from `fusionen.txt`; Recipes with these plants as ingredients for results that are still open are retained. Status: 39 active recipes, 629 open recipes for 525 results.

Tested offline: 51 scenario assertions against the real recipe, identity, capability and rolling bullet files with test doubles, existing Cherry Chomper regression, eight tool checks, full static completion and release build. These are not playtests. The user is solely responsible for starting the game and checking the runtime. Savegames and profiles will not be opened or changed while you are working.

Check manually: all six recipes in both map/glove directions, cost/cooldown, cancellation, terrain and documents/pumpkins obtained; Original artwork including previews and crack stages; Rolling projectiles against multiple zombies, shields, flying targets, allies, roof and rising terrain; 30/40 second digestion, chunks only during digestion, healing limits; Chompzilla against large targets, 40 second special distance on continued normal bites, weak targets, knockback, excess health, Gargantuar/Zamboni/Jack-in-the-Box; Pause, game speed, carry, bungee, death, board change, and save/load during digestion and bite phase. Cross-check common chompers/peashooters and previous fusions.

Installation: `Mods/FusionMod.dll`, version 0.1.34. Proof: `references/release-0.1.34.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.33.dll` to `Mods/FusionMod.dll`. Savegames with the new variants require 0.1.34 or a compatible newer version. LawnGlove remains unchanged.

## Change 0.1.33: Cherry Bomb fusion explosion and size adjustments

Every successful fusion with a normal Cherry Bomb as an ingredient causes a normal cherry explosion on the target field. This applies to both card orders and both glove directions, including the fusion into the Explode-o-shooter. First the result is created, then the ingredients are used up, then the target field explodes. Abort, invalid placement, preview, loading, and scripted spawns do not produce a fusion explosion. Cherry variants without a new Cherry Bomb as an ingredient also do not trigger it.

The explosion uses the native damage helper with the same parameters as Cherry Bombs `Plant.DoSpecial`: radius 115 board pixels, at most one neighboring row, burn damage and native target categories `0x7f`. There is also the normal Powie effect, both native explosion tones, board vibration and the existing cherry-nut healing. The resulting plant retains its own behavior; in particular, Solar Bomb retains its subsequent detonator. Ingredient consumption still does not trigger additional death effects.

The graphical explosion of the Explode-o-shooter projectile is 10% smaller. Only the created particle instance is scaled; 300 damage and radius 80 remain. `TodParticleSystem.OverrideScale` natively scales the associated effect controller without changing the shared particle definition.

Giant Wall-nut is 10% smaller: scaling 1.8 instead of 2.0 and logical size 144 instead of 160 board pixels. The rolling speed drops by 10% regardless of the display: factor 0.45 instead of 0.5. Contact rules, 1800 damage and roll animation remain.

Build and static checks only; no game start and no playtests. The user is responsible for the runtime check: cards and glove in both directions, explosion only on the target field, no explosion on cancellation, damage and cherry-nut healing, smaller project effect as well as size and rolling speed of the Giant Wall-nut. Release/Installation Proof: `references/release-0.1.33.json`. Dismantling when the game is closed: Copy `backup/FusionMod-0.1.32.dll` to `Mods/FusionMod.dll`. Scores, profiles and LawnGlove remain unchanged.

## Change 0.1.32: Cherry Chomper special ability in the actual game path

If swallowed successfully, Cherry Chomper creates a cherry explosion one space ahead like in PvZ Fusion: 1800 damage, radius 160 board pixels with at most one neighboring row. The existing common blast path inherits targeting permissions, shield/body damage, particles, sound, and cherry-nut healing. Native homing, missed bites, bites on large targets and 40 seconds of digestion are retained.

The user reported for 0.1.31 that the special ability was missing. The previous patch was on `Plant.UpdateChomper`. However, the actual replanted DLL also has its logic compiled directly into `Plant.UpdateAbilities`: `Plant.Update` calls `UpdateAbilities`; this contains the successful swallowing process and does not call `UpdateChomper`. As a result, normal play did not reach the previous patch. That was a mistake in the first port.

The patch now captures the native state before and after `UpdateAbilities`. Only the successful transition `ChomperBiting` to `ChomperBitingGotOne` produces the explosion once. The native owner of the ability remains; there is no additional timer and no global swallow registration. Digestion, loading an already achieved swallowing state, missed bites, common chompers, previews, and board dismantling do not produce an additional explosion. Recipes, original graphics, animations and memory identification do not change. The completed result row remains removed from `fusionen.txt`.

Static proof: `references/cherry-chomper/replanted-Update.asm`, `replanted-UpdateAbilities.asm` and `fix-0.1.32.json`. The actual source `CherryChomper.Chomp` at RVA `0x475f80` and the local almanac continue to support the capability. Offline regression testing compiles the real patch file with small test doubles, checks the responsible patch point, successful swallow, uniqueness, explosion position and excluded cases. It doesn't load any game code. Tool tests and static completion remain additionally required.

Release/Installation Proof: `references/release-0.1.32.json`. Previous version: `backup/FusionMod-0.1.31.dll`. No game starts and no changes to profiles or savegames. The runtime confirmation remains with the user: let a zombie be swallowed and check the explosion including area damage for further zombies; Cross-check malocclusion, large targets, digestion, and save/load.

## Change 0.1.31: Automated workflow and Cherry Chomper

`tools/source_index.py` captures Prefabs/GameObjects, Classes, MonoScripts, Animator Controllers and Clips once. The JSON cache under `.cache/source-index.json` is checked against SHA-256 for bundle, DLL, metadata and index script before each use. Changed sources discard the cache; the DLL must still match the complete source archive. The research uses the class index. The exporter accepts repeated `--plant` and `--all`; The bundle is loaded exactly once per call.

`tools/fusion_checks.py recipes` shows all source recipes or all alternatives of a result. `tools/ingredient-availability.json` explicitly records checked availability with replanted identity and justification. Ingredients that are not recorded or marked as unavailable will lock a recipe; Registration does not unlock any ingredient. `--strict` also reports blocked alternatives with an error status. The static final check checks memory identifiers, recipe/availability comparison, resources and export checksums, current version information as well as the complete open list and its numbers. Historical version and number information remain historical evidence.

By default, `tools/release.py` increases the patch version together, checks, builds release, saves the installed previous version without overwriting a different backup and replaces the DLL atomically. Binary equality and DLL file version are checked; `SHA256SUMS` and a release document are updated. An exclusive installation lock prevents parallel releases. In case of an error after installation, DLL, checksums and edited version files are restored. LawnGlove is not installed or modified. Details and reproducible calls are in `vorgehen-neue-fusionen.md`.

### Cherry Chomper as the first complete playthrough

The only source recipe is `Cherry Bomb | Chomper | Cherry Chomper` (Result ID 1016). Both ingredients are expressly recorded as native, available plants. Both card orders and both glove directions use the existing central placement, cost, prominence, and consumption paths. The result is created before the ingredients are consumed; Water lilies, pots and pumpkins remain.

The local almanac requires immediate devouring, 40 seconds of digestion, and a cherry explosion one space in front of the plant upon successful ingestion. The actual source method `CherryChomper.Chomp` at RVA `0x475f80` confirms this explosion path. The native replanted chomper handles target search, allowed targets, unsuccessful bites, bites on targets that are too large and the stored digestion counter of 4000 simulation ticks. Only the native transition `ChomperBiting` to `ChomperBitingGotOne` generates the common cherry explosion once: 1800 damage, center point 80 board pixels in front of the plant, radius 160 pixels, at most one neighboring row. The explosion uses native targeting permissions and hit rectangles, damaging shield and body and healing Cherry-nuts for 200 up to their maximum. Missed bites, large targets not swallowed, digestion, death, and loading do not trigger an additional explosion. Native bite rules and timing are preserved; the original animation does not produce any additional attack events.

Original model: `CherryChomperPrefab` 47417, Transform 120633, Script `globalgamemanagers.assets:2779 CherryChomper`, Animator 126815, Controller 6590. All 24 nodes, 22 sprites and four original clips are embedded: 4900 `idle`, 4898 `bite`, 4899 `chew`, 4901 `swallow`. The controller has a replacing base layer without a mask; the renderer combines the clips with native state and animation progress. The source file contains 18 transform bindings on six paths missing from the prefab in `bite`/`chew`. Only these exact bindings are skipped and listed in the export manifest, as in Unity; unknown bindings remain errors. Documents including full prefab path list are at `references/cherry-chomper/`. Explosion particles and audio remain native Replanted assets.

Memory identifier: `Chomper + Cherrybomb` to `mImitaterType`. Native fields store identity, state, and digestion time; When loading, the original display is restored. No new volatile status list and no own real-time timer. The result row was removed from `fusionen.txt` after implementation. Cherry Chomper as an ingredient for Cherrizilla remains an open question. Currently: 33 active recipes, 635 open recipes for 529 results. `fusionen-quellen.json` remains unchanged.

Tested offline: cache reuse and checksum invalidation, bulk export of Cherry Chomper and existing cherry projectile, full recipe check, static final check and eight tool checks including error detection and release rollback. The original animation is also created as an offline preview. Release/Installation Proof: `references/release-0.1.31.json`. These are not playtests. Game start, saves and profiles remain untouched.

Check manually by the user: both card/glove orders within the cherry bomb firing window, cost/cooldown, cancellation, valid/invalid mark, documents/pumpkin, successful swallow with exactly one explosion one space ahead, edge spaces, armor, cherry-nut healing, missing explosion on malocclusion and large targets, 40 second digestion, original clips and previews, pause/game speed, death, bungee, board change and Save/load during bite/digestion. Countercheck regular chomper and existing explosions.

Dismantling when the game is closed: Copy `backup/FusionMod-0.1.30.dll` to `Mods/FusionMod.dll`. Saves with Cherry Chomper require 0.1.31 or a compatible newer version.

## Change 0.1.30: Cherry-nut, Solar Bomb and Cluster Mine

All three results have exactly one recipe each in the full 4.0.5.2 source archive. Both ingredients are native, already available plants. The recipes apply to cards and glove in both orders:

```text
Cherry Bomb | Wall-nut | Cherry-nut
Cherry Bomb | Sunflower | Solar Bomb
Cherry Bomb | Potato Mine | Cluster Mine
```

Cherry-nut has 4000 life points. Upon death, she triggers a cherry explosion for 1800 damage. Friendly cherry explosions heal surviving Cherry-nuts in the area of ​​effect by 200, up to a maximum of 4000: normal Cherry Bomb, Solar Bomb, other Cherry-nuts and the small Explode-o-shooter explosions are connected. Ordinary cherry balls are not explosions. Friendly explosions already do not natively cause plant damage; there is no general immunity to zombie attacks or jack-in-the-box. The original bodies alternate between undamaged and two crack levels at the native damage thresholds. Healing restores the appropriate representation.

Solar Bomb uses the saved native Cherry Bomb detonator of one second. The explosion deals 1800 damage and creates a true sun with 25 sun value for each zombie hit, even if the zombie survives. Shields do not count as additional zombies. Destruction before the detonator expires triggers the same explosion. The mod counts valid damage targets itself: The native helper `KillAllZombiesInRadius` delivers killed zombies and would be wrong for this sun rule.

Cluster Mine is an edible, low plant. The actual source class `CherryMine` inherits from `Plant`, not from `PotatoMine`; your controller only has `idle`. Therefore, it does not trigger by touch like a normal sharp Potato Mine and does not have its own excavation cycle. When she dies, she deals 1800 mine damage to her row and distributes regular Potato Mines to the permitted spaces in the 3x3 area, including the vacated center space. Each mine created receives a five second saved excavation countdown; Then your native get-up animation runs until it is armed. Occupied spaces, invalid terrain and spaces outside the board are skipped. Existing plants, water lilies, pots and pumpkins will not be replaced. Spawned mines use the normal Replanted representation and do not trigger further fusions.

New cherry explosions use a circle radius of two squares (160 board pixels) with at most one neighboring row; the mine explosion has a radius of one square (80 pixels) in the same row. Native targeting permissions and zombie hit rectangles remain relevant. Explode-o-shooter retains its existing radius and damage. The normal Cherry Bomb stays at its native radius of 115 pixels; their new Nut Heal uses the same radius. Damage hits shield and body, suppressing remaining bodies. The new explosions don't clear ladders. Death effects only run after a successful native death. Ingredient consumption, previews, carried plants, and board mining do not trigger explosions or mine distribution. However, a normal removal of a living plant on the board, for example with a shovel, counts as death; this must be checked manually.

### Original assets and storage

The plant models are entirely from the local `PlantsVsZombiesRH_Data/data.unity3d`:

- Cherry nut: `CherryNutPrefab` 36039, Transform 114130, Script `globalgamemanagers.assets:445 CherryNut`, Controller 6592; Clips 4905 `idle` and 4906 `nothing`; 12 nodes, 9 sprites.
- Solar Bomb: `SunBombPrefab` 50022, Transform 116955, Script `globalgamemanagers.assets:3758 SunBomb`, Controller 6633; Clips 5008 `Bomb`, 5009 `idle`, 5010 `shake`; 11 nodes, 10 sprites. The bomb clip follows the native detonator; the additive shake level remains intact. Animation events do not create a second explosion.
- Cluster Mine: `CherryMinePrefab` 57083, Transform 103897, Script `globalgamemanagers.assets:3088 CherryMine`, Controller 6378; Clip 4356 `idle`; 29 nodes, 29 sprites including the animated red flashing light.

Original pivots, hierarchies, sprite order, curves and shadow reference use the existing import path including glove and grid previews. All models and sprites are embedded. Blast Flash, Sound, Suns, and Generated Normal Mines use Replanted Assets; Original particles and original audio are not imported. `nothing` is the original attack blank state; The port uses the original idle and native damage indicators without introducing any additional source attack system.

Persisted identifiers in `mImitaterType`: `Wallnut + Cherrybomb`, `Cherrybomb + Sunflower`, `Potatomine + Cherrybomb`. These valid enum pairs are centrally reserved and may not be reused. Native fields store identity, health, detonators and mine countdowns. Display will be rebuilt after loading. There is no additional volatile game state created by these three plants; Savegames with these variants require at least this mod version. Existing storage limits for flying mod projectiles remain in place.

Source recipes with registration addresses, original Chinese almanac, method evidence, prefab/animator data, asset checksums and offline previews are located at `references/cherry-nut/`, `references/solar-bomb/` and `references/cluster-mine/`. Common current Plant/PotatoMine life cycles are archived at `references/cluster-mine-lifecycle/`. This description translates the relevant almanac features. Export and preview support the `--plant` values ​​of the same name. The complete source archive remains unchanged. Currently: 32 active recipes; 636 open recipes for 530 results.

### Manual user testing and dismantling

Release build: 0 warnings, 0 errors. All 51 new embedded asset files match the export files. Version attributes and binary equality of the installation are statically checked; Proof: `references/release-0.1.30.json`. These exams are not playtests. The game didn't start; Savegames and profiles have not been changed. The user checks:

- All three recipes in both card orders and both glove directions within the native Cherry Bomb firing window; Cost, cooldown, cancellation, invalid targets and visible highlighting.
- Cherry-nut: 4000 health, both rift levels, death/shovel/crush, 1800 explosion damage, 200 healing from each connected cherry blast, healing limit and return from rift levels. No healing effect from ordinary cherry balls.
- Solar Bomb: one detonator, one explosion, 25 sun per zombie hit including survivors, no additional suns for shields, normal sun collection and sun collector, explosion on early destruction.
- Cluster Mine: no independent arming, explosion on death, free 3x3 fields including center, edge fields, occupied fields, water/roof, retained support plants/pumpkins, five second countdown followed by native get-up animation.
- Original graphics, flashing light, firing animation, height/flooring, both previews, pause/game speed, carry, bungee, death, board swap and save/load. Regular Wall-nut, Cherry Bomb and Potato Mine as well as cross-check existing fusions.

Installation: `Mods/FusionMod.dll`, version 0.1.30. Dismantling when the game is closed: copy back `backup/FusionMod-0.1.29.dll`. Only load saves with the new variants again with 0.1.30 or a compatible newer version. LawnGlove remains unchanged.

## List

`fusionen.txt` contains **286 open recipes** with **228 result plants** in the format:

```text
Plant1 | Plant2 | Fusion name
```

Sorted alphabetically by plant1, then plant2 and fusion name, case-insensitive. Names correspond to the English translation. Already fused plants are included as ingredients; each line describes an immediate fusion step.

The order of ingredients in the porting list corresponds to the original source registrations. In this mod, implemented recipes generally apply in both ingredient orders. Items like Bucket or Football Helmet also occupy the second column.

Included are normal recipes, travel/special recipes, and item fusions. Seven recipes retain an existing resulting plant, including the six registered base plant and associated upgrade combinations. These are also included in the list for complete compatibility. Activations, mode conditions and additional requirements are not reflected in the desired three-column scheme. The list describes registered recipes, not a guarantee that every combination is available in every mode. Other transformations outside of recipe registration are not supplemented as two-plant fusions.

## Origin

The ingredients and results were **statically read from the local `GameAssembly.dll`** under `pvzfusion/english-4.0.5.2/Game Files/`. The basis is the calls of `MixData.AddOrderedRecipe` (RVA `0x477d20`), `MixData.AddRecipe` (RVA `0x477f30`) and the 27 entries of `Bucket.Recipes` (initialization from RVA `0x9d5630`). The seven data-dependent registries in `FirstMix` inherit these item recipes. Reverse registrations of the same prescriptions were combined.

The existing generated C# assemblies contain deprecated method addresses. Therefore, the actual call locations and functional limits of the local DLL were evaluated. The local almanac dump contains 720 entries; The DLL also contains a recipe with result ID 1488. This recipe is also recorded.

The included translation mod loads the English names from the `Teyliu/PVZF-Translation` project. For this list, the [name source is recorded on commit 43982fb929e7](https://raw.githubusercontent.com/Teyliu/PVZF-Translation/43982fb929e71ce51911015fb7b01831c939694e/PvZ_Fusion_Translator/Localization/English/Almanac/LawnStringsTranslate.json). The online source is for names only; the recipes come from the local DLL.

`fusionen-quellen.json` archives the original inventory of 668 recipes and documents the plant IDs, permitted sequences and registration addresses as well as SHA-256 checksums of the sources. For item recipes, the address points to the corresponding entry in `Bucket.Recipes`.

## Status

The basic framework is implemented. Active recipes:

```text
Potato Mine | Torchwood | Hot Potato
Torchwood | Cherry Bomb | Boomwood
Blover | Sunflower | Solar Blover
Flower Pot | Sunflower | Solar Pot
Jalapeno | Sunflower | Solar Flare
Sea-shroom | Sunflower | Sunset-shroom
Pea-shroom | Bucket | Buck-shroom
Puff-shroom | Buckshooter | Buck-shroom
Puff-shroom | Cabbage-pult | Sprout-pult
Puff-shroom | Kernel-pult | Baby Corn
Puff-shroom | Melon-pult | Slice-pult
Puff-shroom | Sunflower | Sun-shroom
Puff-shroom | Hypno-shroom | Gnome-shroom
Puff-shroom | Umbrella Leaf | Umbrella Cap
Hail-shroom | Peashooter | Icicle-shroom
Pea-shroom | Ice-shroom | Icicle-shroom
Puff-shroom | Ice-shroom | Hail-shroom
Puff-shroom | Potato Mine | Nugget-shroom
Puff-shroom | Snow Pea | Icicle-shroom
Puff-shroom | Starfruit | Twinkle-shroom
Puff-shroom | Doom-shroom | Boom-shroom
Puff-shroom | Jalapeno | Bell Pepper
Puff-shroom | Marigold | Penny-shroom
Puff-shroom | Wall-nut | Nut-shroom
Pea-shroom | Peashooter | Repeat-shroom
Pea-shroom | Repeater | Split Pea-shroom
Pea-shroom | Split Pea | Gatling Pea-shroom
Puff-shroom | Gatling Pea | Gatling Pea-shroom
Puff-shroom | Peashooter | Pea-shroom
Puff-shroom | Repeater | Repeat-shroom
Puff-shroom | Split Pea | Split Pea-shroom
Repeat-shroom | Peashooter | Split Pea-shroom
Repeat-shroom | Repeater | Gatling Pea-shroom
Split Pea-shroom | Peashooter | Gatling Pea-shroom
Tall-nut | Football Helmet | Football-nut
Gatling Pea | Football Helmet | Football Gatling
Blover | Chomper | Flytrapper
Blover | Doom-shroom | Fallout Blover
Garlic | Hypno-shroom | Hypno Garlic
Starfruit | Sunflower | Solarfruit
Gatling Pea | Hypno-shroom | Gatling Hypno
Hypno Pea | Peashooter | Hypno Repeater
Hypno Pea | Repeater | Split Hypno
Hypno Pea | Split Pea | Gatling Hypno
Hypno Repeater | Peashooter | Split Hypno
Hypno Repeater | Repeater | Gatling Hypno
Hypno-shroom | Peashooter | Hypno Pea
Hypno-shroom | Repeater | Hypno Repeater
Hypno-shroom | Split Pea | Split Hypno
Peashooter | Split Hypno | Gatling Hypno
Blover | Scaredy-shroom | Camo-shroom
Scaredy-shroom | Starfruit | Starry-shroom
Scaredy-shroom | Sunflower | Sunny-shroom
Scaredy-shroom | Wall-nut | Sturdy-shroom
Cherry Bomb | Scaredy-shroom | Blasty-shroom
Ice-shroom | Scaredy-shroom | Shivery-shroom
Peashooter | Scaredy-shroom | Speedy-shroom
Potato Mine | Scaredy-shroom | Spuddy-shroom
Cherry Bomb | Doom-shroom | Black Cherry
Cherry Bomb | Ice-shroom | Frozen Cherry
Doom-shroom | Ice-shroom | Demise-shroom
Ice-shroom | Sunflower | Sundae-shroom
Flame-shroom | Gloom-shroom | Flame Gloom-shroom
Frost-shroom | Gloom-shroom | Frost Gloom-shroom
Fume-shroom | Gloom-shroom | Gloom-shroom
Fume-shroom | Scaredy-shroom | Gutsy-shroom
Gloom-shroom | Ice-shroom | Frost Gloom-shroom
Gloom-shroom | Jalapeno | Flame Gloom-shroom
Garlic | Threepeater | Stench-spreader
Ice-shroom | Threepeater | Snow Threepeater
Jalapeno | Scorched Threepeater | Charred Threepeater
Jalapeno | Threepeater | Scorched Threepeater
Fume-shroom | Garlic | Foul-shroom
Fume-shroom | Potato Mine | Starch-shroom
Iceweed | Spikerock | Icerock
Scorchweed | Spikerock | Hearthrock
Spikerock | Ice-shroom | Icerock
Spikerock | Jalapeno | Hearthrock
Chomper | Doom-shroom | Doom Chomper
Chomper | Ice-shroom | Frostbiter
Chomper | Squash | Jawbreaker
Doom-shroom | Squash | Doom Squash
Fume-shroom | Ice-shroom | Frost-shroom
Fume-shroom | Jalapeno | Flame-shroom
Doom-shroom | Fume-shroom | Soot-shroom
Fume-shroom | Threepeater | Fume-spreader
Squash | Starfruit | Starry Squash
Squash | Sunflower | Solar Squash
Tangle Kelp | Doom-shroom | Doom Kelp
Tangle Kelp | Jalapeno | Spicy Kelp
Kernel-pult | Umbrella Leaf | Umbrella Husk
Kernel-pult | Wall-nut | Butter-nut
Potato Mine | Sea-shroom | Land-mine
Sunflower | Torchwood | Torchflower
Garlic | Wall-nut | Foul-nut
Peashooter | Squash | Pea Squash
Squash | Bucket | Iron Squash
Starfruit | Wall-nut | Star-nut
Garlic | Gatling Pea | Gatling Poison
Garlic | Peashooter | Poison Pea
Garlic | Repeater | Poison Repeater
Garlic | Split Pea | Split Poison
Peashooter | Poison Pea | Poison Repeater
Peashooter | Poison Repeater | Split Poison
Peashooter | Split Poison | Gatling Poison
Poison Pea | Repeater | Split Poison
Poison Pea | Split Pea | Gatling Poison
Poison Repeater | Repeater | Gatling Poison
Cactus | Wall-nut | Thorn-nut
Hypno-shroom | Potato Mine | Hypno Mine
Ice-shroom | Potato Mine | Ice Mine
Jalapeno | Potato Mine | Chip Mine
Ice-shroom | Spikeweed | Iceweed
Jalapeno | Spikeweed | Scorchweed
Spikeweed | Sunflower | Solarweed
Spikeweed | Wall-nut | Spike-nut
Flame Wall-nut | Tall-nut | Flame Tall-nut
Frost Wall-nut | Tall-nut | Frost Tall-nut
Ice-shroom | Wall-nut | Frost Wall-nut
Jalapeno | Wall-nut | Flame Wall-nut
Tall-nut | Ice-shroom | Frost Tall-nut
Tall-nut | Jalapeno | Flame Tall-nut
Cherry Bomb | Squash | Cherry Squash
Hypno-shroom | Squash | Hypno Squash
Ice-shroom | Squash | Icy Squash
Jalapeno | Squash | Spicy Squash
Marigold | Marigold | Twin Marigold
Umbrella Leaf | Wall-nut | Umbrella Shell
Chomper | Potato Mine | Chomp-mine
Chomper | Sunflower | Solar Chomper
Sunflower | Wall-nut | Solar-nut
Solar-nut | Wall-nut | Twin Solar-nut
Peashooter | Potato Mine | Peatato Mine
Potato Mine | Sunflower | Solar Mine
Potato Mine | Wall-nut | Mine-nut
Peashooter | Bucket | Buckshooter
Wall-nut | Bucket | Buck-nut
Peashooter | Wall-nut | Pea-nut
Chomper | Peashooter | Chomp-shooter
Chomper | Wall-nut | Chomp-nut
Chomp-nut | Peashooter | Chompzilla
Chomp-shooter | Wall-nut | Chompzilla
Chomper | Pea-nut | Chompzilla
Cherry Bomb | Chomper | Cherry Chomper
Cherry Bomb | Wall-nut | Cherry-nut
Cherry Bomb | Sunflower | Solar Bomb
Cherry Bomb | Potato Mine | Cluster Mine
Cherry Bomb | Peashooter | Cherryshooter
Cherry Bomb | Cherryshooter | Explode-o-shooter
Cherry Bomb | Split Pea | Split Cherry
Cherrypeater | Peashooter | Split Cherry
Cherryshooter | Repeater | Split Cherry
Cherry Bomb | Gatling Pea | Gatling Cherry
Cherrypeater | Repeater | Gatling Cherry
Cherryshooter | Split Pea | Gatling Cherry
Peashooter | Split Cherry | Gatling Cherry
Cherry Bomb | Repeater | Cherrypeater
Cherryshooter | Peashooter | Cherrypeater
Peashooter | Sunflower | Solar-shooter
Sunflower | Sunflower | Twin Sunflower
Peashooter | Peashooter | Repeater
Peashooter | Repeater | Split Pea
Peashooter | Split Pea | Gatling Pea
Repeater | Repeater | Gatling Pea
Wall-nut | Wall-nut | Giant Wall-nut
Wall-nut | Tall-nut | Tall-nut
Ice-shroom | Peashooter | Snow Pea
Ice-shroom | Repeater | Snow Repeater
Peashooter | Snow Pea | Snow Repeater
Ice-shroom | Split Pea | Split Snow
Peashooter | Snow Repeater | Split Snow
Repeater | Snow Pea | Split Snow
Gatling Pea | Ice-shroom | Gatling Snow
Peashooter | Split Snow | Gatling Snow
Repeater | Snow Repeater | Gatling Snow
Snow Pea | Split Pea | Gatling Snow
Blaze Pea | Peashooter | Blaze Repeater
Blaze Pea | Repeater | Split Blaze
Blaze Pea | Split Pea | Gatling Blaze
Blaze Repeater | Peashooter | Split Blaze
Blaze Repeater | Repeater | Gatling Blaze
Gatling Pea | Jalapeno | Gatling Blaze
Jalapeno | Peashooter | Blaze Pea
Jalapeno | Repeater | Blaze Repeater
Jalapeno | Split Pea | Split Blaze
Peashooter | Split Blaze | Gatling Blaze
Cherry Bomb | Threepeater | Cherry-spreader
Ice-shroom | Melon-pult | Winter Melon
Lily Pad | Cattail | Cattail
Magnet-shroom | Marigold | Gold Magnet
Spikeweed | Spikerock | Spikerock
Garlic | Marigold | Silver Garlic
Ice-shroom | Marigold | Silver Ice-shroom
Marigold | Sunflower | Silver Sunflower
Marigold | Wall-nut | Silver-nut
Cabbage-pult | Marigold | Silver Cabbage
Doom-shroom | Marigold | Silver Doom-shroom
Hypno-shroom | Marigold | Silver Hypno-shroom
Marigold | Melon-pult | Silver Melon
Flower Pot | Marigold | Silver Pot
Flower Pot | Plantern | Lumos Pot
Kernel-pult | Marigold | Silver Kernel
Marigold | Umbrella Leaf | Silver Umbrella
Flower Pot | Cabbage-pult | Cabbage Pot
Flower Pot | Kernel-pult | Butter Pot
Flower Pot | Umbrella Leaf | Umbrella Pot
Kernel-pult | Cob Cannon | Cob Cannon
Flower Pot | Garlic | Garlic Pot
Flower Pot | Melon-pult | Melon Pot
Flower Pot | Spikeweed | Spike Pot
Flower Pot | Wall-nut | Nut Pot
Flower Pot | Cherry Bomb | Cherry Pot
Flower Pot | Doom-shroom | Doom Pot
Flower Pot | Hypno-shroom | Hypno Pot
Flower Pot | Jalapeno | Hot Pot
Flower Pot | Peashooter | Pea Pot
Flower Pot | Threepeater | Tri-pot
Kernel-pult | Threepeater | Kernel Tri-pult
Melon-pult | Threepeater | Melon Tri-pult
Cabbage-pult | Kernel-pult | Taco-pult
Cabbage-pult | Melon-pult | Overripe Melon
Cabbage-pult | Threepeater | Cabbage Tri-pult
Kernel-pult | Melon-pult | Cob-pult
Cabbage-pult | Cob-pult | Salad-pult
Cabbage-pult | Ice-shroom | Coldslaw
Cabbage-pult | Jalapeno | Kimchi-pult
Ice-shroom | Kernel-pult | Cobsicle
Kernel-pult | Overripe Melon | Salad-pult
Melon-pult | Taco-pult | Salad-pult
Cabbage-pult | Wall-nut | Cabbage-nut
Melon-pult | Wall-nut | Melon-nut
Squash | Wall-nut | Squash-nut
Threepeater | Wall-nut | Three-nuts
Tangle Kelp | Puff-shroom | Tingle Kelp
Tangle Kelp | Fume-shroom | Grapple Kelp
Tangle Kelp | Wall-nut | Tangle-nut
Tangle Kelp | Spikeweed | Spike-kelp
Eyes Zombie Head | Sunflower | Blood Sunflower
Blover | Blover | Cyclover
Blover | Garlic | Disguster
Blover | Hypno-shroom | Mind-blover
Blover | Ice-shroom | Breeze Blover
Fume-shroom | Peashooter | Boost-shroom
Fume-shroom | Spikeweed | Caltrop-shroom
Fume-shroom | Torchwood | Blowtorch-shroom
Fume-shroom | Wall-nut | Wallop-shroom
Cabbage-pult | Garlic | Garbage-pult
Garlic | Kernel-pult | Clove-pult
Garlic | Melon-pult | Garlic-pult
Jalapeno | Melon-pult | Summer Melon
Garlic | Starfruit | Starlic
Ice-shroom | Starfruit | Winter Starfruit
Jalapeno | Starfruit | Spicy Starfruit
Peashooter | Starfruit | Starshooter
Cattail | Ice-shroom | Frost Cattail
Cattail | Jalapeno | Flame Cattail
Hypno-shroom | Jalapeno | Hypno Pepper
Hypno-shroom | Melon-pult | Hypno Melon
Sea-shroom | Cactus | Urchin-shroom
Sea-shroom | Fume-shroom | Shark-shroom
Sea-shroom | Puff-shroom | Minnow-shroom
Sea-shroom | Starfruit | Sea Star
Sea-shroom | Hypno-shroom | Jelly-shroom
Sea-shroom | Ice-shroom | Swell-shroom
Sea-shroom | Plantern | Beacon-shroom
Sea-shroom | Scaredy-shroom | Hermit-shroom
Pumpkin | Cactus | Thorny Pumpkin
Pumpkin | Fume-shroom | Fuming Pumpkin
Pumpkin | Puff-shroom | Puff-kin
Pumpkin | Starfruit | Starry Pumpkin
Pumpkin | Cherry Bomb | Cherry Pumpkin
Pumpkin | Garlic | Stinky Pumpkin
Pumpkin | Scaredy-shroom | Scareless Pumpkin
Pumpkin | Wall-nut | Nut Shell
Pumpkin | Blover | Windy Pumpkin
Pumpkin | Plantern | Jack O' Plantern
Pumpkin | Sea-shroom | Sea-shell
Pumpkin | Spikeweed | Spiky Pumpkin
Cabbage-pult | Squash | Lettuce-pult
Fume-shroom | Melon-pult | Spring Melon
Kernel-pult | Squash | Butternut-pult
Melon-pult | Squash | Gourd-pult
Doom-shroom | Magnet-shroom | Dormant-shroom
Fume-shroom | Magnet-shroom | Morph-shroom
Magnet-shroom | Melon-pult | Melon Morph
Magnet-shroom | Starfruit | Starmorph
Cactus | Fume-shroom | Saguaro-shroom
Cactus | Ice-shroom | Ice Cactus
Cactus | Magnet-shroom | Magnet Cactus
Plantern | Wall-nut | Illumi-nut
Doom-shroom | Garlic | Doom Garlic
Doom-shroom | Potato Mine | Crater-tot
Doom-shroom | Sunflower | Doomflower
Doom-shroom | Wall-nut | Doom-nut
Pumpkin | Ice-shroom | Cryo Pumpkin
Pumpkin | Jalapeno | Pyro Pumpkin
Pumpkin | Melon-pult | Melon Shell
Pumpkin | Peashooter | Pumpkin Pod
Kernel-pult | Magnet-shroom | Kernel Morph
Melon Morph | Bucket | Metal-pult
Morph-shroom | Bucket | Rust-shroom
Starmorph | Bucket | Iron Star
Doom Pea | Split Pea | Gatling Doom
Doom-shroom | Gatling Pea | Gatling Doom
Doom-shroom | Peashooter | Doom Pea
Doom-shroom | Starfruit | Death Star
Doom-shroom | Threepeater | Doom-spreader
Cabbage-pult | Doom-shroom | Damage-pult
Cactus | Umbrella Leaf | Umbrella Thorn
Fume-shroom | Starfruit | Starburst-shroom
Garlic | Umbrella Leaf | Umbrella Clove
Fume-shroom | Kernel-pult | Blubber-shroom
Fume-shroom | Plantern | Ray-shroom
Fume-shroom | Sunflower | Sunrise-shroom
Fume-shroom | Umbrella Leaf | Bush-shroom
Beamshooter | Peashooter | Rebeamer
Beamshooter | Repeater | Split Beam
Beamshooter | Split Pea | Gatling Beam
Gatling Pea | Plantern | Gatling Beam
Peashooter | Plantern | Beamshooter
Peashooter | Rebeamer | Split Beam
Peashooter | Split Beam | Gatling Beam
Plantern | Repeater | Rebeamer
Plantern | Split Pea | Split Beam
Rebeamer | Repeater | Gatling Beam
Cactus | Doom-shroom | Doom Cactus
Cactus | Plantern | Lumos Cactus
Cactus | Spikeweed | Thornweed
Cactus | Starfruit | Starthorn
Kernel-pult | Spikeweed | Butterweed
Melon-pult | Spikeweed | Springweed
Sea-shroom | Squash | Splash
Sea-shroom | Wall-nut | Sea-nut
Spikeweed | Squash | Squashweed
Squash | Threepeater | Squash-spreader
Tangle Kelp | Squash | Strangle Kelp
Tangle Kelp | Threepeater | Kelp-spreader
Potato Mine | Squash | Potato Masher
Potato Mine | Tangle Kelp | Tangle Mine
Sea-shroom | Potato Mine | Sea-mine
Tangle Kelp | Potato Mine | Kelp Mine
Fume-shroom | Hypno-shroom | Perfume-shroom
Spikeweed | Tangle Kelp | Tangle Weed
Blover | Plantern | Glowver
Doom-shroom | Scaredy-shroom | Frenzy-shroom
Cabbage-pult | Sunflower | Solar-pult
Charred Threepeater | Jalapeno | Phoenix Threepeater
Ice-shroom | Plantern | Aurora Plantern
Plantern | Threepeater | Triplantern
Flower Pot | Blover | Blover Pot
Flower Pot | Scaredy-shroom | Scaredy Pot
Flower Pot | Sea-shroom | Sea Pot
Peashooter | Threepeater | Multipeater
Blover | Cabbage-pult | Cabbage Dropper
Blover | Kernel-pult | Butter Fly
Puff-shroom | Fume-shroom | Puff-shroom Spreading: Fume-shroom
Puff-shroom | Scaredy-shroom | Puff-shroom Spreading: Scaredy-shroom
Puff-shroom | Cactus | Ball Cactus
Puff-shroom | Cherry Bomb | Cherry Pop
Cherry Bomb | Fume-shroom | Trigger-shroom
Chomper | Fume-shroom | Belch-shroom
Chomper | Scaredy-shroom | Hungry-shroom
Ice-shroom | Jalapeno | Spicicle
Demise-shroom | Fume-shroom | Glacier-shroom
Doom-shroom | Frost-shroom | Glacier-shroom
Ice-shroom | Soot-shroom | Glacier-shroom
Pumpkin | Chomper | Chompkin
Pumpkin | Potato Mine | Pumpkin Mine
Pumpkin | Sunflower | Sunny Pumpkin
Marigold | Potato Mine | Marigold Mine
Potato Mine | Threepeater | Threetato Mine
Cabbage-pult | Umbrella Leaf | Umbrella Kale
Flower Pot | Ice-shroom | Ice Pot
Melon-pult | Umbrella Leaf | Umbrella Rind
Plantern | Umbrella Leaf | Lumos Umbrella
Doom-shroom | Plantern | Blacklight Plantern
Garlic | Torchwood | Stenchwood
Jalapeno | Torchwood | Scorchwood
Sea-shroom | Blover | Tide-shroom
Hypno-shroom | Torchwood | Hypnowood
Starfruit | Torchwood | Sparkwood
Threepeater | Torchwood | Torchthree
Torchwood | Wall-nut | Nutwood
```

Plant a sunflower from the seed card onto an existing sunflower. The game continues to calculate the cost of the incoming Sunflower (usually 50 Sun) and its card cooldown. The merger uses the existing native Twin Sunflower with its normal behavior; no additional Twin Sunflower card or shop activation is necessary. A sunflower in an open field is planted as normal, unchanged. The original Twin Sunflower upgrade card remains usable.

Alternatively, pick up a sunflower with the glove and place it on another sunflower. Both ingredients are replaced by a native Twin Sunflower at the target space; the source field becomes free. The glove does not consume any additional sun and receives the previous 10-second cooldown. Invalid targets and aborts retain the previous glove behavior. The grid preview shows the supported starting plant.

Native placement rules of the resulting plant apply, including water lily/pot, craters, special fields, and mode restrictions. The water lily or pot under an ingredient as well as existing pumpkins are not consumed. Damaged ingredients yield a freshly initialized Twin Sunflower; Health points and production counters are initialized like a new native plant. A third Sunflower on a Twin Sunflower does not trigger another fusion. Zen Garden and unregistered combinations remain unchanged. Mimic seed cards are not implemented as an additional planting route in this first recipe; an already transformed Sunflower can be used with the gauntlet.

## Structure and other recipes

- `src/FusionPlant.cs`: Recipe identity from native plant type and optional variant identifier; `From(plant)` distinguishes existing fusions from their native base types.
- `src/FusionRecipes.cs`: Central assignment `(Zutat A, Zutat B) -> Ergebnis`. Further fusions to existing native plants, such as Repeater + Repeater -> Gatlingpea, require another entry here. Enter each recipe once; the query automatically accepts both ingredient orders. Use Replanted-`SeedType`, not the Fusion IDs from `fusionen-quellen.json`.
- `src/FusionService.cs`: Common Target Check and Gauntlet Fusion. Result is generated before removing ingredients. The placement is validated via the native check of the result; A recursion lock prevents the recipe from being dissolved again.
- `src/PlantingPatches.cs`: Expands native target checking and changes the type of plant produced within a player planting operation only. Costs, card cooldowns, planting effects, and cursor management remain with the game. Scripted `AddPlant` calls and savegame restore do not trigger fusion.
- `glove-mod/src/OccupiedTilePlacement.cs`: Public events `CanPlace` and `TryPlace` for occupied glove targets. The fusion mod reports its common logic there. The Glove Mod doesn't know any recipes and doesn't need the Fusion Mod. Without a registered extension, occupied destinations remain blocked.

The current adapter supports ingredients and results at the normal plant level. Native upgrades are checked with this if the native base plant is suitable. Otherwise, the synchronous native terrain checking ingredient to be consumed is temporarily hidden and restored in the finally block; The common terrain plant type from the Glove mod then applies to upgrades. New plant types, other levels or special multi-field rules also require their own placement/creation logic in the service; A recipe list entry alone is not enough. The porting list does not automatically activate any additional recipes.

## Building and installation

```bash
../.tools/dotnet/dotnet build FusionMod.csproj -c Release
```

The Fusion Mod builds the Glove Mod as a project dependency. Target: .NET 6, existing Replanted/MelonLoader interop assemblies. If necessary, specify another game path with `-p:GameDir=...`.

Installed files:

- `Mods/FusionMod.dll` (0.1.29)
- `Mods/LawnGlove.dll` (1.2.2, required for new interface)

The previous installed Glove DLL is backed up at `backup/LawnGlove-before-fusion.dll`. `SHA256SUMS` documents the built DLLs. To restore, remove `Mods/FusionMod.dll` and, if necessary, copy the saved Glove DLL back as `Mods/LawnGlove.dll`. Already created Twin Sunflowers are native plants and do not require their own mod data structure to store them.

## Testing and open play testing

Release build of both projects: 0 warnings, 0 errors. The native processes were statically reproduced: `Board.CanPlantAt` (RVA `0x6565b0`), `Board.MouseDownWithPlant` (`0x665440`), `Board.AddPlant` (`0x6553e0`), `Board.NewPlant` (`0x668e90`) and `Plant.IsUpgradableTo` (`0x6b2450`). The native sun deduction and card usage continue to occur for the incoming Sunflower before and after the resulting plant is created, respectively.

The game did not start, no automatic tests were run, and no save/profile files were changed. Runtime behavior and display have not yet been confirmed. The user carries out the play tests: card fusion with/without enough sun, glove fusion and cooldown, cancellation/invalid targets, placement on water lily/pot or under pumpkin and saving/loading the Twin Sunflower.


## Change 0.1.1: Fusion targets glow

While a Sunflower is held from the roster or on the gauntlet, all valid Sunflower fusion targets glow with the same native color as the shovel/glove hover. The plant itself is not marked. Other plants, invalid target fields and unimplemented recipes do not get an additional glow. When placing, canceling or changing tools, the next native color update recalculates the normal display. Fusion glow is not requested during pause.

`src/FusionHighlightPatch.cs` extends `Plant.UpdateReanimColor`: The native `mHighlighted` flag is only set during the color update for a valid fusion target and is reset to its original value afterwards. This means that existing hover markers are retained; no permanent flags are saved in the save game. The native animation update calls the color update regularly (statically confirmed, call location RVA `0x6bb922`). The merger check uses the same recipes and placement rules as the commit. The Glove Mod 1.2.1 provides a read-only query of the currently worn plant with `LawnGloveMod.GetCarriedPlant(Board)`.

Both release builds: 0 warnings, 0 errors. No game starting and no playtests running. In particular, please check multiple possible targets, unchanged normal shovel/glove hover, pick/cancel, tool change and successful fusion. Previous installed DLLs: `backup/FusionMod-0.1.0.dll` and `backup/LawnGlove-1.2.0.dll`.


## Change 0.1.2: Peashooter + Peashooter -> Repeater

Two Peashooters merge into a freshly initialized native repeater. This works via the Peashooter seed card (normal Peashooter cost, usually 100 Sun, and card cooldown) as well as with the Gauntlet (no additional Sun cost, normal Gauntlet cooldown). Matching Peashooter targets light up when holding a Peashooter. The Sunflower merger remains active.

Repeater is not a native upgrade. Therefore, with normal results, the `CheckFusionPlacement` joint test uses the same temporary exclusion flags as the Glove mod: only the target plant to be consumed is excluded during the synchronous native terrain test with `mSquished=true` and `mIsOnBoard=false`. Both values ​​are restored even in the event of errors. Supporting plants and squashes remain visible for native testing and are not consumed. For native upgrade results, the base plant remains in place for their upgrade testing. All three paths (Roster, Glove, Lights) use this common test.

Release build: 0 warnings, 0 errors. No game starting and no playtests running. Still to be checked by the user: Roster/Glove Fusion, Target Lights, Sun Cost/Card Cooldown, Invalid Targets, Water Lily/Pot/Pumpkin and the existing Sunflower Fusion. Previous version: `backup/FusionMod-0.1.1.dll`.


## Change 0.1.3: Split pea and interchangeable ingredients

Peashooter + Repeater and Repeater + Peashooter result in a native split pea. All recipe queries generally accept both orders, even for future recipes: a single entry in `FusionRecipes.cs` is sufficient. Roster, Glove, and Target Lights share this query. With the roster, the card actually added is paid for and reloaded (Peashooter usually 100, Repeater usually 200 Sun); As before, the glove does not consume any additional sun.

`SplitPeaProjectiles.cs` only captures projectiles generated from a Split Pea's secondary weapon during `Plant.Fire`. Their left shots are mirrored on the left edge of the screen in board coordinates (`-board.mX`) and switched to `ProjectileMotion.Straight`. This maintains the native speed of 3.33 units per tick; `mVelX` alone would not change the fixed native reverse movement. The check runs after the movement, before the native collision/offscreen check. Collision damage and Torchwood conversion remain in play. Right split pea shots and alien reverse projectiles are not marked. This applies to all native split peas, including existing plants.

The association is removed upon projectile death, pool reset, and board disposal so that reused projectiles do not receive the old marking. Limitation: Native projectile serialization does not save a shooter. After loading a save, projectiles that have already been saved and are still flying backwards do not have a rebound marker; Newly fired split pea projectiles will be received as normal. Projectiles that have already been reversed remain stored natively flying forward.

The entry `Peashooter | Repeater | Split Pea` has been removed from `fusionen.txt`. Other open recipes that use split pea as an ingredient will remain. The source archive remains complete.

Release build: 0 warnings, 0 errors. No game starting and no playtests running. Open user test: both orders by roster and glove, matching target markers, left shot at the rim (also after Torchwood), unchanged right shots and other plants. Previous version: `backup/FusionMod-0.1.2.dll`.


## Change 0.1.4: All Gatling Pea recipes

The two recipes registered in the source archive with result Gatling Pea are implemented:

```text
Peashooter | Split Pea | Gatling Pea
Repeater | Repeater | Gatling Pea
```

Both apply regardless of ingredient order, across Roster and Glove, with the same target marking as all other recipes. The result is a freshly initialized native Gatling Pea. The roster uses the cost and card cooldown of the actual ingredient added. The gauntlet uses its previous cooldown with no additional sun cost.

The native Gatling Pea upgrade rule only accepts repeaters as a base plant. `CheckFusionPlacement` uses this rule when the existing ingredient is natively upgradable to the result. For the other fusion ingredients, with the target plant temporarily excluded, the terrain is checked against the normal base type of the resulting plant. The mapping comes from the existing Glove logic, now shared under `glove-mod/src/PlantPlacementRules.cs`; it was not duplicated. Supporting plants and pumpkins are retained during testing and fusion. Roster, Glove and Lights use the same test.

The two completed entries have been removed from `fusionen.txt`. Other Gatling variants and recipes that require Gatling Pea as an ingredient remain open. The source archive remains complete.

Release build of both DLLs: 0 warnings, 0 errors. No game starting and no playtests running. User review: both recipes, both ingredient orders for the Peashooter/Split Pea recipe, Roster/Glove and Target Lights, Terrain/Underplants, and previous fusions. Previous versions: `backup/FusionMod-0.1.3.dll` and `backup/LawnGlove-1.2.1.dll`.


## Change 0.1.5: Giant Wall-nut and coupled split-pea heads

`Wall-nut + Wall-nut -> Giant Wall-nut` is implemented via Roster and Glove. Both ingredients must be in the visually undamaged stage. The test corresponds exactly to the native `AnimateNuts` limit: `health >= floor(2 * maxHealth / 3)`. With the normal 4000 life points, this is allowed from 2666 life points. Minor bite damage without visible cracks is permitted. A cracked target wall-nut and a cracked glove ingredient prevent fusion and fusion lights. The normal native repair of a damaged wall-nut using a seed card remains possible and does not create a giant wall-nut.

`FusionRecipes.AcceptsIngredient` has the condition-dependent recipe condition; `FusionService` applies it to target plants and carried ingredients. This means that placement, commit and lighting check the same damage limit.

`GiantWallNut.cs` uses the existing native plant type `GiantWallnut`, its controller and the native bowling ground animation `_ground`. The size is set to absolute double, including 160×160 logical size. The movement uses the native ground speed at normal bowling reference rate 12 fps, removes the visual size factor and halves the normal integer movement step. This means that the larger display does not double the speed. The Giant Wall-nut rolls to the right, does not change row or vertical position, updates its current column and disappears behind the right edge of the board.

A swept collision rectangle checks all zombies in the same lane instead of damaging just one target per tick. Any living zombie hit dies via the native `DieWithLoot` function, regardless of health, helmet or shield. The native bowling hitting tone is played on hits. It does not move while taking a break or wearing the glove. The implementation only replaces the Giant Wall Nut type ability update outside of Zen Garden; other bowling nuts retain their previous behavior. Identity, position and controllers use native save, with no additional mod save file.

The Giant Wall Nut entry has been removed from the open port list; the source archive remains intact.

`SplitPeaLinkedFire.cs` couples both split-pea weapons: with every native shot, the corresponding counter shot is triggered directly, even without a target on this side. The native muzzle positions and projectile types are retained. A recursion lock prevents an endless chain of shots. One mask per `UpdateShooting` prevents double counter shots if the game were to trigger both weapons in the same update. Multiple game updates per rendered frame are treated separately. The left split pea shot rebound marker also works for the additional fired shots.

Release build: 0 warnings, 0 errors. No game launches or play testing by the agent. User test: Giant-Fusion made from undamaged or slightly bitten nuts; Exclusion of both cracked glove/target ingredients; normal repair; Glow, size, speed, fixed lane, hit against multiple armored/strong zombies, pause and save/load. Split Pea: left only/right only/targets on both sides as well as rebound of the newly coupled left shots. Previous version: `backup/FusionMod-0.1.4.dll`.


## Fix 0.1.6: Native split pea volley and shooting animations

The direct shot link from 0.1.5 has been replaced. The Split Pea once again fully uses its native shot plan: **one shot forward, two timed shots backwards**, with both native head animations. This also applies if there is only a target on the left or right. Without a target on both sides, no new volley begins.

`SplitPeaLinkedFire.cs` now only extends target search within `Plant.FindTargetAndFire`. If a split pea head doesn't find a target there, a native target on the other side can activate its regular firing sequence. The additional direction query has a recursion lock and remains limited to the specific plant and this method call. Other target queries retain their native meaning. The previous fire hooks for additional counter shots and the UpdateShooting mask have been removed; Additional projectiles or custom volley timers are no longer generated.

Statistically tracked: Native `UpdateShooter` starts the rear head at the beginning of the cycle and later starts the front and rear heads together (LaunchCounter 25). `FindTargetAndFire` starts the animations `shooting` (Head) and `splitpea_shooting` (Head2). `UpdateShooting` creates the respective projectiles at the native animation time. The existing marking and direction reversal of the left projectiles remains active.

Release build: 0 warnings, 0 errors. No game launch or playtest executed. User testing: target right only, target left only, targets on both sides; 1 projectile at the front and 2 at the back per cycle, visible head animations and left rebound. Previous version: `backup/FusionMod-0.1.5.dll`.


## Fix 0.1.7: Wall-nut repair used up the fusion ingredient prematurely

Cause of missing Giant Wall-nut spawn: After a successful placement check, the native `MouseDownWithPlant` flow calls the repair logic at Wall-nut on Wall-nut. This removes the existing nut above `Plant.Die` (RVA `0x665e92`) before reaching `Board.AddPlant`. As a result, the previous fusion search did not find any living starting plants and allowed a normal wall-nut to be created again.

`PlayerPlantingScope` now remembers the already validated fusion ingredient for the specific planting click. The new `DeferFusionIngredientDeathPatch` only postpones their removal within this synchronous process. The existing AddPlant hook thereby finds the ingredient, produces the result and only then removes the source plant via an explicitly released Consume call. The operation context is restored by a Harmony finalizer even in the event of errors. Uninvolved plants, normal damaged nut repairs, and the Glove expiration retain their previous death/use logic. Card costs and card cooldown remain native.

The no visible damage condition still applies. Cracked Wall-nuts are not marked as fusion ingredients and can still be repaired normally.

Release build: 0 warnings, 0 errors. Repair path statically traced using the local GameAssembly; no game start or playtest executed. User check after full reboot: Wall-nut seed card for an intact wall-nut, slightly bitten but crack-free nut, normal cracked nut repair, and glove fusion. Previous version: `backup/FusionMod-0.1.6.dll`.


## Fix 0.1.8: Giant Wall-nut stays on the field and deals contact damage

The previous cancellation check used `board.mWidth`, the widget width. Instead, the same logical right limit now applies as with the native `Plant.UpdateBowling`: X = 800 (comparison with RVA `0x6b7df5`). This means that the rolling nut is no longer prematurely removed simply because of an inappropriate widget width. Double size, half normal bowling speed and fixed lane remain.

The instant kill from 0.1.5 has been replaced with **1800 damage per directly touched zombie and nut**. `Zombie.TakeDamage(1800, HitsShieldAndBody | DoesntLeaveBody)` uses the native explosion damage value and its damage flags (0x12) without triggering an explosion, a burn effect, or a radius search. A Gargantuar with full health survives the hit. A quantity of Zombie DataIDs hit per nut prevents further hits against the same surviving zombie while continuing to roll. Plant/board identity and pool IDs delineate this mapping; Death, reset and board disposal clean them up.

Restriction when saving/loading: The hit history only lives in the main memory. If a rolling nut with a surviving zombie still overlapping is saved and later loaded, it can be hit again. Position and plant type are still saved natively.

Release build: 0 warnings, 0 errors. Native edge check and damage call statically tracked; no game start or playtest executed. User check after full reboot: visible rolling to the right edge, fixed lane, size/speed, multiple contacts and exactly one 1800 hit per nut against a fully healed Gargantuar. Previous version: `backup/FusionMod-0.1.7.dll`.


## Change 0.1.9: Wall-nut + Tall-nut → Tall-nut

Exactly one more recipe from the open list is implemented:

```text
Wall-nut | Tall-nut | Tall-nut
```

Both ingredients are native, already usable replanted plants (`SeedType.Wallnut = 3`, `SeedType.Tallnut = 23`). The fusion creates a fresh native Tall-nut with 8000 health and her native lock against normal pole vault, pogo and dolphin jumps. Damaged ingredients may be used; the special condition without visible cracks remains limited to Giant Wall-nut. Score, controllers, animations, damage levels and saving are already native; an additional behavior patch is not required for this recipe.

Like all mod recipes, it applies in either order: Tall-nut card to Wall-nut or Wall-nut card to Tall-nut, as well as both directions with the gauntlet. Matching targets use the existing fusion glow. Card cost and card cooldown belong to the incoming card (usually Tall-nut 125 and Wall-nut 50 Sun, respectively). The glove does not consume additional sun and retains its cooldown. Both ingredients are consumed, supporters/pumpkins are obtained. Normal planting in open fields remains possible. The existing synchronous terrain checking and generation before ingredient consumption will be reused unchanged.

### Source and assets

The local build `../pvzfusion/english-4.0.5.2/Game Files/` is relevant. The unmodified source archive proves the registration with RVAs `0x47ec9d` and `0x480fd5`: Wall-nut (Fusion-ID 3) + Tall-nut (Fusion-ID 1027) → Tall-nut (1027). Do not confuse these IDs with the Replanted types. The local `Mods/PvZ_Fusion_Translator/Dumps/LawnStrings.json` entry 1027 describes 8000 life points, the high jump lock and wall-nut as a base. The relevant original entry including the German functional description, source checksums and recipe proof is under `references/tall-nut/`.

The original textures `Tallnut_body`, `Tallnut_cracked1` and `Tallnut_cracked2` are stored there as reference assets from `PlantsVsZombiesRH_Data/data.unity3d`. For the game, this native result uses the already appropriate Replanted controller and its graphics; Fusion Unity textures do not replace replanted animations. The representation is therefore the native replanted tall nut, not a pixel-perfect copy of the Fusion artwork. There are no additional asset files to install.

Conscious existing mod rules compared to the source: Both ingredient orders are allowed; Tall-nut can also be planted natively in open fields. The Fusion source describes the wall-nut as an upgrade base. Your one-page registration will be preserved unchanged in the source archive.

### Audit and manual playtest

Release build successful: 0 warnings, 0 errors. The installed FusionMod DLL matches the build via SHA-256; Previous version 0.1.8 is secured. No automatic tests, no game start and no save/profile changes; The user checks the runtime and display. After a complete restart, please check both card orders and both glove directions, as well as target lights, sun costs/cooldowns, damaged ingredients (fresh result), valid water lily/pot fields with pumpkin, invalid targets and cancellation, save/load as well as the previous Wall-nut/Giant-Wall-nut recipes. For the function, check in particular life points and the jump lock.

The completed entry has been removed from `fusionen.txt`; the source archive remains complete. The new mandatory workflow is in [procedure-new-fusions.md](vorgehen-neue-fusionen.md), including source version, asset, ID, and repair pitfalls. Previous version/removal: Copy `backup/FusionMod-0.1.8.dll` back to `Mods/FusionMod.dll`. LawnGlove remains at 1.2.2.


## Change 0.1.10: Snow Pea and freezing points

`Ice-shroom | Peashooter | Snow Pea` is implemented. Both ingredients are already native Replanted plants (`Iceshroom = 14`, `Peashooter = 0`); Result is `Snowpea = 5`. Do not confuse with Fusion source IDs 10, 0 and 1034. Both map orders and both glove directions use the existing central recipe resolution, terrain checking and objective marking. The result is created before the ingredients are consumed. Water lilies, pots and pumpkins remain. Cost and cooldown are part of the incoming card; the glove retains its previous rules. A dormant Ice-shroom can serve as an existing ingredient; During the day, she does not activate her pre-fusion effect. At night, an existing Ice-shroom must be fused before its native release.

Snow Pea uses the native controller, its shooting animations, and ice projectiles (20 base damage, approximately 1.5 seconds per shot). `src/SnowPea.cs` adds to the freezing mechanism described in the local 4.0.5.2 almanac: each ice pea hit gives a freezeable zombie 10 freezing points. At an initial 100 points it freezes for four seconds. The threshold increases by 100 to a maximum of 1000 after each point-triggered freeze. Hits during a freeze do not extend it or carry points into the next cycle. When thawing or removing the cold, remaining points are discarded; the increased threshold remains for this zombie. Already frozen zombies receive quadruple Ice Pea damage, even on hits that reach the freezing threshold (normally 80 instead of 20). Other projectiles do not receive a damage bonus. The native armor, shield and immunity rules remain in effect.

The native `Zombie.HitIceTrap` produces freeze, ice controller, noise and animation freeze. His own additional 20 Ice Shroom damage is only suppressed by freezing points when summoned; the pea continues to deal exactly its own hit. The freezing duration of this common path is now four seconds plus ten seconds of subsequent cold, also for the normal Ice-shroom. Effects that have existed for a long time are not shortened. Normal ice pea hits still use the native ten second slow. The depiction remains the native blue cold coloring with ice shackle; This does not involve a pixel-precise transfer of the fusion ice cream.

This adjustment applies to all native Snow Peas, including directly planted, pre-existing and loaded plants. The decisive factor is the type of projectile at impact. A bullet that has already been transformed into an ordinary pea by Torchwood does not give any freezing points or the bonus. The change does not require additional marking of the plant or stored projectiles.

### Sources and assets

`references/snow-pea/` contains local almanac entries 1034 and 10, the unchanged source recipe (registration RVA `0x47cece`), original textures for Snow Pea, Ice Pea and Ice Shackle, as well as provenance and checksums. The static evaluation of the actual local DLL places `Bullet_snowPea.HitZombie` at `0x826ce0`, freezing points at `0x64dd50` and the initial threshold at `0x65fbf8`. The current source increases the threshold to **1000**; the older unofficial Unity project mentions 400 and was not adopted for this. Actual method tokens were resolved from the local metadata file; generated Fusion Interop tokens are partially moved.

As with other native results, the already matching replanted assets are used at runtime. The extracted Fusion textures are reference assets and not replaceable Replanted animations. Additional asset files do not need to be installed into the game.

### Memory limits and manual checking

Freezing points and increased thresholds are exclusively in the working memory. After save/load they start again at 0/100. Running native cold/freeze counters and native Snow Pea plants are saved by the game. Zombie death, pool reset, and board end clean up the extra state; Board and DataID protect against reused objects. No saves or profiles are changed by installing the mod.

Release build successful: 0 warnings, 0 errors. Installed DLL and build are binary identical via SHA-256; Assembly and MelonInfo version were statically checked as 0.1.10. Previous version 0.1.9 is secured. No automatic tests and no game start; The user carries out the runtime check. Please check after a complete restart:

- Both card orders and glove directions, target lights, cost/cooldown, cancellation and invalid targets; Water lily/pot/pumpkin.
- Ice-shroom sleeping during the day as an ingredient; at night there is a short period of time until it is triggered.
- Against a sufficiently resilient, freezeable zombie: first freeze after ten ice pea hits, four seconds of standstill, then slowing; second freeze after twenty more hits after thawing.
- Quadruple damage during freeze and triggering hit; no additional 20 damage from the Ice Shackle. Normal Peashooters remain at normal damage.
- Ice-shroom alone: ​​four seconds of freezing, ten seconds of subsequent cold and continuing its normal damage. Frost Immune Zombies, Shields and Helmets, Fire and Torchwood, Pause and Save/Load.
- Existing fusions, notably Split Pea and Giant Wall-nut.

The completed entry has been removed from `fusionen.txt`; 660 recipes with 542 results remain open. The source archive remains unchanged. Previous version/removal: Copy back `backup/FusionMod-0.1.9.dll` to `Mods/FusionMod.dll`. LawnGlove remains at 1.2.2.


## Change 0.1.11: Snow Repeater with both recipes

The native replacement representation described here only applied to 0.1.11. As of 0.1.12, original graphics and original animations are included; see the following amendment section.

All two recipes registered in the local source archive for this result are implemented:

```text
Ice-shroom | Repeater | Snow Repeater
Peashooter | Snow Pea | Snow Repeater
```

All four ingredients are already usable. Both recipes work in either order via cards and gauntlet; Target lights, terrain checking, water lilies/pots/pumpkins, and generation before ingredient consumption continue to use the common fusion path. Cards only pay for the incoming ingredient and use its cooldown. The gauntlet retains its previous cost and cooldown rules. Damaged ingredients result in a freshly initialized plant with 300 health points. The restriction on an ice shroom that is triggered immediately at night remains in place; An Ice-shroom that sleeps during the day can normally be used as an ingredient.

Snow Repeater uses the native Repeater AI: two time-staggered shots per volley, approximately every 1.5 seconds. Only his projectiles are switched to ice peas **before** native projectile initialization. Each deals 20 base damage and uses the existing Snow Pea logic: cold, 10 freezing points, four seconds of freezing when the threshold is reached, and quadruple damage against frozen zombies including the triggering hit. Both floors contribute freezing points separately. Immunities, armor, torchwood, pause, and targeting remain native. Normal Repeaters, Snow Peas, and Split Peas do not receive additional shots or foreign projectile types.

### Variant identification, representation and storage

A custom source type 1105 is not passed to native Replanted APIs. `FusionPlant.SnowRepeater` instead refers to the pair of `mSeedType = Repeater` (7) and `mImitaterType = Snowpea` (5). This pair is not created by native impersonator transformation, whose identifier is 48. Native initialization sets both fields before controller creation. Native serialization saves both, deserialization reads them before rebuilding the controller, and pool reset resets the identifier to None. There is no additional variant storage in memory and no change to the binary storage format. With the mod active, Snow Repeater should remain intact even after loading and moving the glove; the playtest is pending.

The central recipe resolution always uses `FusionPlant.From(plant)` for existing and worn plants. This means that Snow Repeater is not consumed as a normal repeater in its recipes. For example, Peashooter on Snow Repeater cannot produce a normal split pea; the still open result Split Snow remains locked. Native upgrade cards retain their own game rules.

`SnowRepeater.cs` selects the existing Replanted Snow Pea controller when creating the controller, including when loading. He has the right shooting animations for the native pea shooter; the repeater AI delivers the double salvo. The representation is therefore a native blue Snow Pea with a double shot, not a pixel-perfect copy of the Fusion artwork. When worn, the existing glove uses the native base type as a preview: green repeater; After putting it down, the ice cream controller appears again. No additional asset files are required in the game.

Freezing points and increased zombie thresholds retain the memory limit documented in 0.1.10: they start again at 0/100 after loading. The variant identifier itself remains stored natively. Without FusionMod active, the saved plant behaves like a regular repeater; reverting to an older mod version also does not provide a snow repeater effect. Use the saved DLL to restore; Saved games are not touched during the installation.

### Source and limits

Only `../pvzfusion/english-4.0.5.2/Game Files/` is relevant. The source archive records Ice-shroom (10) + Repeater (1030) for RVA `0x47cee6` and Peashooter (0) + Snow Pea (1034) for `0x47ce47`, each result 1105. These are source IDs, not replanted seed types.

`references/snow-repeater/` contains the unmodified local almanac entry 1105, the original image `DoubleSnow` (Texture2D 2190, resources.assets), `DoubleSnowPrefab` (GameObject 35603) extracted from `data.unity3d`, along with components, source checksums, and static behavior/memory receipts. The actual DLL addresses were resolved using the current metadata and CodeGenModule table: `DoubleSnowPeaShooter.Awake` at `0x4d3d40`, `GetBulletType` at `0x54c2d0`. The latter regularly selects ice pea type 15; the special branch with another source plant not implemented here will not be ported. The existing common freezing logic is occupied in `references/snow-pea/`.

The almanac also mentions a manual click upgrade to **Split Snow** after 15 seconds. This other result and its recipes remain open; the click upgrade is not available in this version. The selected fusion step only creates Snow Repeaters. Both completed Snow Repeater lines have been removed from `fusionen.txt`; 658 recipes with 541 results remain open. The complete source archive remains unchanged. The workflow now explicitly requires all valid recipes of a selected result to be considered together and documents variant identification, projectiles, storage and these porting limits.

### Audit and manual playtest

Release build successful: 0 warnings, 0 errors. Installed FusionMod DLL and build are binary identical via SHA-256; Assembly and MelonInfo version 0.1.11 were statically checked. Previous version 0.1.10 is secured. LawnGlove remains unchanged at 1.2.2. No automatic tests, no game start and no save/profile changes. The user checks the runtime and display after a complete restart:

- Both recipes, both card orders and both glove directions; Target glows, cost/cooldown, damaged ingredients, cancellation and invalid targets.
- Valid water lily/pot fields and pumpkins; Ice-shroom sleeping during the day and their short period of time until they are released at night.
- Blue display, two time-delayed ice peas per volley, normal target behavior; normal repeaters and Snow Peas in comparison.
- Cold and freezing points of each projectile, quadruple damage when frozen, immunities, shields, torchwood and pause.
- Move Snow Repeater and save/load: preserve variant identification, display and double shot; Continue to use native shot counters saved during a volley.
- Snow Repeater may not offer normal Repeater fusion recipes via cards, gauntlets, or lights. Further review existing repeater/split pea/giant wall nut fusions.

Dismantling: Copy `backup/FusionMod-0.1.10.dll` back to `Mods/FusionMod.dll`.


## Fix 0.1.12: Original graphics and animations for Snow Repeater

Snow Repeater now uses the graphics **and both animation clips from the local PVZ Fusion 4.0.5.2**. The native replacement representation from 0.1.11 is removed. This applies to newly merged and loaded Snow Repeaters, as well as both gauntlet previews. Both recipes, cost rules, variant identifier storage, double shot and freezing logic are retained.

### Original data and integration

From `PlantsVsZombiesRH_Data/data.unity3d`, file `resources.assets`, the original `DoubleSnowPrefab` (GameObject 35603), its AnimatorController `DoubleSnow` (6442), its **19 hierarchy objects with 17 sprites** and the clips **idle (4541, 2 seconds)** and **shoot (4542, approx. 1.333 seconds)** adopted. These include individual head, mouth, eye, eyebrow, ice crystal, stem, leaf and shadow sprites. The static almanac image `DoubleSnow.png` is not output as an animated plant.

`assets/snow-repeater/` contains the original cut sprites and `model.json` with original pivot points, pixels-per-unit, transform hierarchy, visibility, draw order and full cubic animation curves. This data is embedded into the FusionMod DLL as 18 resources; additional files do not need to be copied to the game folder. `references/snow-repeater/visual-assets.json` documents original objects and SHA-256 checksums. The extraction comes only from the prescribed local build, not from the unofficial Unity project.

`SnowRepeaterVisuals.cs` builds Unity-SpriteRenderer from this pure representation data. The native repeater controller remains active for AI, shot timing and save, its graphics and shadow are hidden. The original shadow is part of the imported representation. The original curves are evaluated directly, including the visibility channels for the flashing parts. Original proportions are retained; The native display limit is used exclusively for conversion into replanted sizes and ground position. The original internal sprite order is preserved in a SortingGroup, whose external sorting follows the native controller.

The idle clip follows the native board simulation counter with a stable cosmetic DataID phase shift and the speed 1.3 specified in the source controller. The plant's short native ping-pong frame counter is not used as a clock for this. The shot clip follows the phase of the native `shooting` animation and is timed to the existing native double shot sequence. His movements come from the original curves. The two original `AnimShoot` events (approx. 0.533 and 1.067 seconds) are archived as source evidence, but are not executed as additional game calls: the native repeater remains solely responsible for the two projectiles. Projectile flight, ice pea/hit effects, and freeze rendering remain native as in 0.1.11; this correction replaces the plant and preview display.

Both glove previews also use the original composite sprites and the original idle. The native preview remains responsible for cursor/grid position, transparency and sorting. Pause pauses the animation phase. The plant display continues to follow the native color values ​​for marking and damage. Native controller data is not expanded to include additional GameObjects; After loading, the representation is created again from the saved variant identifier. Death, pool reset, controller end, board end and mod end clean up the display. Construction errors are logged and restore the native display instead of leaving the plant invisible.

### Reproduction and asset preview

`tools/export_snow_repeater.py` requires Python with UnityPy 1.25.4 and Pillow. It reads the original local bundle and writes the embedded data and proof of provenance. The original clips do not contain the usual float/position curves in this player build; they were completely reconstructed from `m_StreamedClip` including the constant channels. Unknown bindings lead to the export being aborted.

`tools/preview_snow_repeater.py` renders the extracted hierarchy and its curves for offline visual inspection. Results: [Original animation as GIF](references/snow-repeater/original-animation-preview.gif) and [Frame overview](references/snow-repeater/original-animation-preview.png). Both show the source data, no footage from Replanted and no confirmed playtest.

### Exam and open play test

Release build successful: 0 warnings, 0 errors. Embedded asset files and resources have been statically aligned. Installed DLL and build are identical via SHA-256; Assembly and MelonInfo version are 0.1.12. The previous version is under `backup/FusionMod-0.1.11.dll`. LawnGlove remains unchanged at 1.2.2.

The game didn't start. No automatic playtests and no save/profile changes. After a complete restart, please check in particular:

- Both recipes and both ingredient orders via card and glove; Original artwork with ice crystals and eyebrows.
- Original idle and firing movement, exactly two ice peas per volley, pause and previous cold/freeze function.
- Cursor and grid preview while carrying, hidden/invalid grid targets, transparency, drop, cancel and reshoot.
- Ground orientation and size ratio, water lily/pot/pumpkin, shadows, target glows and damage flashes; no additional visible native repeater graphics.
- Save/load at idle and during a volley, scoop/death and new board rebuild; no leftover graphic parts or double shadows.

The split snow click upgrade remains open as in 0.1.11. The open recipe list remains at 658 recipes with 541 results. Dismantling: Copy `backup/FusionMod-0.1.11.dll` to `Mods/FusionMod.dll`; all original assets of this correction are in the new DLL.


## Fix 0.1.13: Render material of the original graphics

According to user playtesting, Snow Repeater was invisible with 0.1.12. There is no error loading the original assets in the existing MelonLoader runtime log; however, the existing Unity player log occupies the active Universal Render Pipeline (URP). The import created SpriteRenderer without explicit material assignment. This meant that the connection to Replanted's pipeline was incomplete; Decoded sprites and valid animation curves alone do not guarantee a visible display.

All imported plant parts and both glove previews now receive a common material with the `Universal Render Pipeline/2D/Sprite-Unlit-Default` occupied in the installed shader bundle. The shader must be present and supported before the native replacement representation is hidden. The material has a white basic color value; Original textures, alpha channels, sprite colors and animations remain unchanged. The container also adopts the same camera layer as the native renderer. The footage will be released with the imported GPU assets at mod end. Shader origin and passport data are documented at `references/snow-repeater/replanted-sprite-shader.json`.

For each representation created, `Snow Repeater original art ready` reports shaders, sprite count, camera layer, scale, native boundaries, visibility/active flags and the native color value once. If the shader/asset initialization fails, the native representation is retained and the error appears in the log. The material correction addresses the statically determined missing render contract; their effect in the game has not yet been confirmed.

Release build: 0 warnings, 0 errors. Version 0.1.13 and the binary equality of build and installed DLL were statically checked. The previous installed DLL is under `backup/FusionMod-0.1.12.dll`; LawnGlove remains unchanged. No changes to recipes, original asset files, savegames or profiles. The game did not launch and no automatic playtests were run.

After a full reboot, please check the visibility of a newly merged and a loaded Snow Repeater plant, along with the original idle/firing motion and both gauntlet previews. If the plant remains invisible, `MelonLoader/Latest.log` now contains the display values ​​mentioned for limitation. Dismantling: Copy `backup/FusionMod-0.1.12.dll` to `Mods/FusionMod.dll`.


## Fix 0.1.14: Build original graphics after shader loading

The user playtest for 0.1.13 showed ordinary repeaters. The current log now explicitly proves the reason: `Required Snow Repeater shader is unavailable: Universal Render Pipeline/2D/Sprite-Unlit-Default`. Both loaded plants already failed in the `Loading` scene. Restoring the native display worked correctly; However, the original representation was no longer created afterwards.

`Shader.Find` does not load a shader from an addressables bundle that has not yet been loaded. The mod therefore now determines the actually registered shader via the native ResourceLocators and loads its IResourceLocation with `Addressables.LoadAssetAsync<Shader>`. No bundle will be opened parallel to game management. The internal asset path `Packages/com.unity.render-pipelines.universal/Shaders/2D/Sprite-Unlit-Default.shader` is compared; the catalog keys themselves can be GUIDs, labels or any addresses. Origin and asset path are occupied in the existing installed shader bundle.

The construction only begins in `Playing`. As long as the asynchronous loading process is running, the display remains registered and the native graphic remains visible as a temporary placeholder. After successfully loading and checking the shader name and GPU support, the unchanged original sprites and animations are built and the native graphics are hidden. Waiting is no longer an error path. The load handle remains as long as the shared materials and is released via addressables when the mod ends. A board change during loading does not require orphaned callbacks; Newly registered plants can continue to use the current or already completed charging process.

The log initially says `Loading Snow Repeater shader through Addressables`, after the graphics setup is complete `Snow Repeater original art ready`. Actual loading errors are still reported explicitly, leaving the native appearance intact. The playtest is still pending.

Release build: 0 warnings, 0 errors. Version 0.1.14, binary equality of build and installed DLL and all 18 unchanged original asset resources were statically checked. Previous version: `backup/FusionMod-0.1.13.dll`. LawnGlove remains unchanged; no additional DLLs or asset files need to be installed. The game didn't start; no automatic playtests and no save/profile changes.

After a complete restart, please load the Snow Repeaters that have already been saved and check their original graphics after the short shader loading process. Then do a new fusion and check both gauntlet previews, including idle/shooting animation, pause, and reload/board swap. To dismantle, copy `backup/FusionMod-0.1.13.dll` to `Mods/FusionMod.dll`.


## Fix 0.1.15: Removed catalog access after reported crash

The user reported a game crash with 0.1.14. The saved logs end after board initialization before the new message `Loading Snow Repeater shader through Addressables` appears. There is no clear native crash stack trace in it; The exact crash site has therefore not been proven. The time limitation makes the IL2CPP catalog access introduced in 0.1.14 suspect. Logs, DLL checksum and an image of the former display source code are saved unchanged under `references/crash-0.1.14/`.

All new catalog path is removed: no ResourceLocator/Key iteration, no typed generic shader requests, no `Shader.Find` and no custom bundle loads. Instead, the mod adopts its own material copy from the native plant preview. For the glove preview, this comes directly from your PreviewController, otherwise from the existing Cursor PreviewController. If this is missing, the same native factory as in LawnGlove provides a temporary hidden preview that is released again after copying. Third-party cursor controllers and their materials are neither changed nor released. The material copy is cleaned up with the rest of your own display assets.

This means that the sprite material that is part of the actual game pipeline and is already loaded by the game is used instead of resolving a shader via additional IL2CPP generics. The 17 original sprites, source hierarchy, two original animations, recipes, double shot, freeze logic and saved variant identifier are retained. The build is still waiting for `Playing`. The message `Snow Repeater uses native preview material` names the actual material/shader adopted; `Snow Repeater original art ready` records the remaining display values.

Release build: 0 warnings, 0 errors. Version 0.1.15, installed binary equality and all 18 embedded original asset resources are statically checked. The removed catalog/shader loading path was also checked for absence in the decompiled release build. Previous version: `backup/FusionMod-0.1.14.dll`; This previous version is the version reported by the user as having crashed and is not a recommended dismantling version. The older backups are retained; LawnGlove is unchanged.

The game didn't start. No saves or profiles were changed and no automatic playtests were carried out. **Crashfreedom and visible display have not yet been confirmed.** The next user playtest should first load the saved game and check whether the board starts without crashing; then check original graphics/animations and both glove previews.


## Fix 0.1.16: Skin anchors and size calculation

The 0.1.15 user log confirms successfully loaded original assets and supported material `Sprites/DefaultColourblend`. It also shows native mesh limits of around 302 million units and a calculated scale of 213,707,280. These limits are not suitable for determining the size of the plant graphic; the previous adjustment scaled and shifted the artwork extremely.

`completecollection2_ef281/PvZReCoreLib.dll` shows the responsible anchor in `SpriteRendererSkin.ApplySkin` and `SpriteRendererPlantSkin.ApplySkin`: additional sprites are attached locally to the CharacterSkinController, with explicit scale. The Fire Peashooter assets use 1 pixel per unit, a center ground pivot, and unchanged prefab transforms. FusionMod adopts this anchoring principle without any new DLL dependencies. It maintains its own SortingGroup, which takes the native sorting level and order and maintains the original order within the plant.

The original representation is now attached to the skin ground anchor. Their proportions are fitted into a fixed 80x80 plant field with a native conversion factor of 2.82 (225.6 render units). Glove previews only use the real `Sprite.bounds` of their native preview graphics. Native spine mesh boundaries are not used for waiting, scale, or position. Graphics, animations and recipes are retained. Evidence, reference sources and the previous user log are at `references/snow-repeater/rendering-0.1.16/`.

Release build: 0 warnings, 0 errors. Installed DLL and version as well as the 18 embedded original asset files are statically checked. Previous version: `backup/FusionMod-0.1.15.dll`. LawnGlove remains unchanged. The game didn't start; Visibility and exact size/ground adjustment are not yet confirmed in-game. After restarting, please check both recipes, idle/shooting animation, glove pickup/grid preview/drop, water lily/pot/pumpkin and save/load.


## Change 0.1.17: Snow Split Pea / Split Snow

**Split Snow** is the name in the recipe list; the desired **Snow Split Pea** corresponds to the original plant `SnowSplit`, source ID 1107. All three recipes registered in the local archive are now implemented, in both orders:

```text
Ice-shroom | Split Pea | Split Snow
Peashooter | Snow Repeater | Split Snow
Repeater | Snow Pea | Split Snow
```

All ingredients are already native or implemented plants. Card Fusion, Gauntlet Fusion, and Aim Glow use the same central recipe resolution. A card still pays the cost of the incoming ingredient and uses its cooldown; The existing glove rules apply to gloves. Variants are recognized via `FusionPlant.From`. Notably, Peashooter on Snow Repeater now gives Split Snow; Peashooter on Split Snow may not spawn a common Gatling Pea. Gatling Snow remains open.

### Behavior and storage

A volley includes one ice pea to the right and two staggered ice peas to the left: 20 × 3 damage each about 1.5 seconds according to the local almanac. Native split-pea AI and the existing coupled targeting control both heads even with a target on only one side. The existing rebound management reverses left shots on the left edge of the screen. `SnowShooters.cs` converts projectiles for both ice variants before their native initialization; `SnowPea.cs` remains solely responsible for cold, 10 freezing points per hit and quadruple damage against frozen zombies. Torchwood still uses the actual native projectile type after conversion.

The secure native identity is `mSeedType = Splitpea` (28), `mImitaterType = Snowpea` (5). The same initialization, serialization, deserialization and reset paths already used as with the Snow Repeater receive the variant identifier without changing the storage format. The controller and the original representation are rebuilt from this identifier when loaded. Without this mod version the plant appears as a native Split Pea.

Known memory limits remain: freezing points and increased zombie thresholds start again at 0/100 after loading. Projectiles that have already been stored and are flying backwards lose their shooter assignment and thus their rebound; Newly fired projectiles after loading are recorded as normal. Already reversed projectiles retain their native forward motion.

The original almanac mentions a manual click upgrade to Gatling Snow after 15 seconds. This result is not yet implemented; the upgrade remains blocked. The previously unported click upgrade of the Snow Repeater is also not supplemented by these three recipes. Both limits affect additional transformations outside of the selected two-plant recipes.

### Original assets and evidence

`references/snow-split/` contains the local almanac entry 1107, the almanac image `SnowSplit` (Texture2D 2406), the original prefab and its rendering components, static DLL evidence, and source checksums. The actual v31 metadata and CodeGenModule method table occupy `SnowSplit.GetBulletType` for RVA `0x574b10`, `Shoot1` for `0x574c90` and `Shoot2` for `0x574f20`. The regular projectile type is 15 (snowPea). The special case with another source plant that has not yet been implemented is not anticipated.

From the local `data.unity3d`, `resources.assets`, `SnowSplitPrefab` (GameObject 37188, Transform 120506), controller 6363 as well as **20 hierarchy objects, 16 sprites and both animation clips** are integrated: `idle` (4335, 2 seconds) and `shoot` (4336, about 1.333 seconds). Original pivots, transforms, visibility channels, and cubic animation curves are retained. The source events `AnimShoot`/`AnimShoot2` are documented and not additionally executed. The native shooting logic remains responsible.

The render path confirmed since January 01, 2016 is now merged into `src/ImportedPeaVisuals.cs` for both ice variants. Each variant has its own embedded models and GPU assets; Same sprite IDs cannot mix models. Native preview material copy, skin anchors, sorting, shadow replacement, glove previews and cleanup are managed together. The size conversion is based on the native 80x80 plant dimensions from `PlantInitialize` and 2.82 render units per board pixel. Head animations take both native head tracks into account in Split Snow. Exact appearance and phase consistency remain to be checked in the game.

Export and offline-only asset preview now support `--plant snow-split`:

```bash
/tmp/pvz-fusion-extract-venv/bin/python tools/export_snow_repeater.py --plant snow-split
/tmp/pvz-fusion-extract-venv/bin/python tools/preview_snow_repeater.py --plant snow-split
```

Without parameters, the export for Snow Repeater remains. The preview is not a playtest. The source archive remains unchanged. The three completed result rows are removed from `fusionen.txt`; **655 open recipes with 540 results** will remain, including all open recipes with Split Snow as an ingredient.

### Build, installation and manual testing

The first release build uncovered an existing project problem: Decompiled `.cs` documents under `references/` were automatically compiled by the SDK. `FusionMod.csproj` now explicitly compiles only `src/**/*.cs`. The documents are retained; additional third-party libraries are not required.

Release build: **0 warnings, 0 errors**. Version **0.1.17** is equal in project, MelonInfo and README. Installation target is `Mods/FusionMod.dll` in the replanted directory mentioned above. The previous installed **0.1.16** is backed up under `backup/FusionMod-0.1.16.dll`. The new DLL is installed; Binary equality, all 35 embedded asset files and version metadata are statically checked (`references/snow-split/verification-0.1.17.json`); `SHA256SUMS` contains the installed DLL checksums. LawnGlove will not be replaced. Dismantling: copy back the saved 0.1.16 as `Mods/FusionMod.dll`. A saved Split Snow plant then behaves like the normal Split Pea.

The game didn't start. No playtests or automated runtime tests were run; Savegames and profiles remain untouched. Please check manually:

- All three recipes with ingredients, cards and both directions swapped; enough/too little sun, cooldown, cancellation and invalid fields.
- Ice-shroom as the second ingredient and daytime sleep as the first ingredient. At night, an Ice-shroom placed first can trigger before the second click.
- A zombie on the right only, only on the left and on both sides: one right and two left ice peas, left rebound, cold/freeze points, quadruple freeze damage and Torchwood conversion.
- Both original heads, idle/shot animation, size/ground, sorting, both glove previews, water lily, roof/pot and pumpkin. Check Snow Repeater as a regression.
- Save/load while idle and during a volley, then new reverse shots; Death, shovel, pool reuse and board changes without leftover graphics.
- Not your usual repeater/split pea recipe on ice cream variants; no yet open Gatling-Snow fusion or click transformation.


## Change 0.1.18: Split Snow shows native Split Pea

The user reports an ordinary split pea representation for 0.1.17. The secured MelonLoader log shows the successful Splitpea merger, but no SnowSplit asset build and no loading error. The original static prefab proof explains the silent wait state: `SplitPea.m_skinController` is empty. The previous rendering path waited for this optional skin controller and therefore never got to the point where the original assets were loaded.

If the skin controller is present, `ImportedPeaVisuals.cs` continues to use its transform. If this is missing, the native mesh transform serves as an anchor. In the original prefab, `m_renderOffset` and `m_meshRenderer` point to the same object `SplitPea_Render`; its animations and native offsets are retained. Shaders, original assets and size conversion remain with the confirmed render path. The back shot animation is also queried with its actual name `splitpea_shooting`, instead of searching the back head track again `shooting`.

Documents: `references/snow-split/rendering-0.1.18/Latest-0.1.17.log` and `native-splitpea-prefab.json` with bundle checksum. Version 0.1.18 increases Project, MelonInfo and README together. The previously installed 0.1.17 is saved under `backup/FusionMod-0.1.17.dll`; 0.1.18 is installed. Release build: 0 warnings, 0 errors. Version metadata and corrected render path are checked in the release artifact; installed and built DLL are binary equal (`rendering-0.1.18/verification.json`). To restore, copy this file back as `Mods/FusionMod.dll`. Recipes and open list remain unchanged.

Release build and static artifact checking only; the game won't start. The user checks visibility of both ice variants, both heads and shooting directions, glove previews, placement on pot/water lily or under pumpkin and loading a saved split snow. The previous storage limits continue to apply. Visibility is only confirmed after this playtest.


## Change 0.1.19: Common rocking, additive shot animation and same scale

The user confirms the visibility of Split Snow for 0.1.18. However, the head remained static while the stem bobbed; Split Snow was smaller than Snow Repeater. On the Snow Repeater, the head visually separated from the handle while shooting. The source check shows two common causes in the display adapter.

Both original AnimatorControllers have an additive `Shoot` layer (`m_LayerBlendingMode = 1`, weight 1). So far their head position and scale curves have been written absolutely above the idle pose. This means that the head lost the idle movement of the handle during the shot presentation. `ImportedPeaVisuals.cs` now adopts the original composition: first the full idle pose, then position differences, scale ratios and rotation differences of the shot clip compared to its initial pose. Blink visibility curves remain discrete. Even an unchanged native shooting phase can no longer hold the bobbing head in an absolute position. The source has both heads under `PeaShooter_Head`; this hierarchy remains intact. Neither artificial head connections nor additional game shots will be introduced.

The size was previously calculated from the entire visible graphic per plant. The wider rear head resulted in a smaller scale. Both original prefabs have the same ground area: `Shadow`, sprite 15757, 88x36 pixels, PPU 100, local scale 7 and root scale 0.23. Its width is 1.4168 source units. Both plants now use this common reference and its ground position instead of the entire head width. With the native field width of 80×2.82, this results in the same local scale of approximately 159.2321 for both. Split Snow's back head extends the silhouette to the left; The body and front head are not reduced in size. The previously visible Snow Repeater remains the size reference. Glove previews also use the common source reference within the true native sprite boundaries.

The export records the additive plane and the ground reference in `model.json` and `visual-assets.json`. Unsupported layer modes, masks, or zero reference scales are not silently adopted. The offline preview uses the same additive composition and the same camera for both plants. New PNG/GIF previews at `references/snow-repeater/` and `references/snow-split/` show head/stem movement and shot recoil. They are asset checks, not playtests. Original controller and user log are saved under `references/animation-0.1.19/`.

Version 0.1.19 exclusively replaces FusionMod. Recipes, projectile logic, saved variant identifier and open recipe list remain unchanged. The previously installed 0.1.18 is saved under `backup/FusionMod-0.1.18.dll`. 0.1.19 is installed: Release build with 0 warnings and 0 errors, version metadata and 35 embedded asset files statically checked, installed and built DLL binary identical. Proof: `references/animation-0.1.19/verification.json`. To restore, copy this backup back as `Mods/FusionMod.dll`. The game won't start; Scores and profiles will not be changed.

To be checked manually: both plants next to each other in idle, continuous fire and firing pauses, split snows both firing directions, head/stem contact throughout the entire movement, same body/head scale, glove mount and both previews, roof/pot/water lily/pumpkin and save/load. The visibility confirmation for 0.1.18 does not yet confirm these new motion fixes.


## Change 0.1.20: Gatling Snow

All four original recipes are implemented. Each ingredient is already available natively or as a full mod variant:

```text
Gatling Pea | Ice-shroom | Gatling Snow
Peashooter | Split Snow | Gatling Snow
Repeater | Snow Repeater | Gatling Snow
Snow Pea | Split Pea | Gatling Snow
```

All recipes apply in both orders, via existing seed card or gauntlet and the same common target marker. Cards still only provide the respective native ingredient. Snow Repeater and Split Snow are recognized as existing plants or on the glove; Your variant identifier is retained when the recipe is queried. Cost and card cooldown belong to the incoming card; the glove uses its previous cost rules. Terrain, water lilies, pots, pumpkins and production before ingredient consumption remain with the existing service.

Gatling Snow fires four staggered ice peas to the right per volley, dealing 20 base damage each, with the native Gatling volley timing of approximately 1.5 seconds. Common ice pea hits slow, deliver 10 freeze points, and deal quadruple damage against frozen zombies. Torchwood conversion remains native. The native Gatling controller is the AI ​​and memory adapter; the ice projectile selection occurs before its initialization. Additional volleys or original animation events will not occur.

The local almanac describes a Gatling shooter for Ice Peas (20×4/1.5 seconds). Its accompanying text explains that the hot barrels keep the otherwise cold shooter from freezing. Original description, four recipe registrations, actual method addresses and binary documents are under `references/gatling-snow/`. The complete source archive remains unchanged.

Original art and animations are from the local 4.0.5.2 bundle: `SnowGatlingPrefab` (38426), Transform 107630, Controller 6294, Clips 4186/4187. 24 transform nodes and 20 sprites are embedded under `assets/gatling-snow/`. The weight 1 additive shot plane uses the same composition as the existing ice variants. The ground reference has the same width 1.4168 but a different source position. The render adapter normalizes this position to the occupied native `GatlingPea_Render` anchor. Native graphics are only hidden after successful construction. Glove and grid previews use the same asset path.

`FusionPlant.GatlingSnow` uses `Gatlingpea` (40) and `Snowpea` (5) as the saved variant identifier. Load rebuilds the imported representation. The common cleanup at death, pool reset, controller and board end also applies to this variant. Zombie freezing points and resistance thresholds are still not saved. The previously missing click upgrades of Snow Repeater and Split Snow are not part of this recipe port; Gatling Snow is created using the four recipes.

The four completed result rows have been removed from `fusionen.txt`. The list now contains 651 open recipes with 539 resulting plants.

Manual user checking: all four recipes in both orders, card fusion as long as the incoming ingredient is present as a card, both glove directions, target marking and unchanged native recipe identities. Also check four Ice Peas per volley, Cold/Freeze/Quad Damage, Torchwood, Head/Stem Contact and Size, Glove/Grid Preview, Pause, Water Lily/Pot/Pumpkin, and Save/Load and Board Switch. Split Snow should not be considered a regular split pea in these recipes; Gatling Snow may not trigger a native Gatling Pea recipe.

Release build and offline asset preview are static proofs. The game didn't start; Play testing and runtime confirmation remain with the user. Previous installed DLL: `backup/FusionMod-0.1.19.dll`. To restore, copy this backup to `Mods/FusionMod.dll`. Before loading a save game with Gatling Snow, reinstall version 0.1.20 so that its variant identifier is recognized correctly.


## Change 0.1.21: Solar-shooter

The only registered recipe for Solar-shooter (Source ID 1000) is fully available via the common fusion path:

```text
Peashooter | Sunflower | Solar-shooter
```

Both ingredients are native. Both orders work via seed card and glove; Target marking, terrain checking, ingredient consumption and obtaining water lily, pot and pumpkin use the existing service. The incoming card determines sun cost and cooldown; the glove retains its cost rules. The result has its own recipe identity and does not trigger normal Peashooter recipes. Source archive and Glove DLL remain unchanged. The completed result row is removed from `fusionen.txt`; 650 recipes with 538 results remain open.

### Description and behavior

Solar Shooter produces sun and shoots solar projectiles: 30 direct damage per shot at approximately 1.5 seconds. Sun production delivers 25 suns, first after 4-7 seconds, then after 25 seconds with the original random deviation of −2 to +2 seconds. It doesn't need a target. Every third **fired** shot drops a collectible sun worth 5 when hitting a zombie. Missed shots count, but do not cause sun to fall. Armor and shields still use the native direct hit path. Torchwood does not alter these solar projectiles; the original TorchWood trigger only converts regular or ice peas.

The local description explains that the sunflower behind the archer is his sister: she generates sun while he protects her with solar projectiles. What is relevant is the Chinese Almanac entry 1000 and the actual local 4.0.5.2 DLL, not the older unofficial Unity project. Documents and German description are available at `references/solar-shooter/`.

`SolarShooter.cs` only complements the variant `Peashooter` + saved `Sunflower` marker. Native target search, shot sequence and 300 life points come from the Peashooter. A dedicated production timer avoids conflicts with its shot counters and pauses when worn, paused and during bungee states. Production lightening is approximated using the existing color path; the incompatible original shader is not loaded. Additional original animation events do not produce second shots.

The hit sun is a native `SmallSun` with individually corrected value. Replanted normally counts `SmallSun` as 15. A patch of `GetSunValue` alone is not enough: `Coin.ScoreCoin` includes the calculation inline and calls `Die` before crediting. A synchronous context restored via finalizer therefore captures the allocation before death and sets exactly this credit in `Board.AddSunMoney` to 5. Other small suns remain at 15. Manual collection and the existing Sun Collector mod use the same native collection path.

### Original graphics and animations

The existing import path adopts `PeaSunFlowerPrefab` 46095, Transform 117484 and Controller 6624: 24 nodes, 18 original sprites, idle clip 4979 and shot clip 4980. The shot layer is additive, weight 1, without a mask. Material, skin anchors, sorting, glove and grid preview, and cleanup remain jointly responsible. The ground reference uses the actual solar shadow: sprite 15757, width 0.88, local scale 7.4329915 and root scale 0.23 gives 1.5044375 source units. Don't adopt the ice cream variant standard without checking.

The original shot clip contains two orphaned position/scale bindings for Path-CRC32 `2349996250`. This transform is missing in the delivered prefab; Unity can't apply the bindings either. The export only logs this documented exception in `visual-assets.json`; all other unknown bindings remain errors. The similarly named tower controller 6654 belongs to a different game script and is not used as a replacement.

The solar projectiles also use original assets: `Bullet_normalSun` 63439, controller 6142, clip `Solar` 3765, four nodes and two sprites. `ImportedSolarProjectileVisuals.cs` uses the same model, material and animation helpers. Native movement, hit and save remain with the projectile. Native graphics are only hidden after successful construction and restored in the event of errors. All required runtime assets are embedded in the DLL.

Reproducible export and offline preview:

```bash
/tmp/pvz-fusion-extract-venv/bin/python tools/export_snow_repeater.py --plant solar-shooter
/tmp/pvz-fusion-extract-venv/bin/python tools/export_snow_repeater.py --plant solar-projectile
/tmp/pvz-fusion-extract-venv/bin/python tools/preview_snow_repeater.py --plant solar-shooter
```

### Storage and user verification

Plant identity remains stored via the native type/marker fields; Loading produces solar shooter again with original representation. **Additional data is not yet saved:** Production timer, brightening and three-shot counter restart after loading. After loading, already flying sun projectiles become ordinary native peas with their graphics and damage, without a sun bonus. 5-sun pickups that have not yet been credited lose their assignment after loading and then count 15 natively. Before saving, collect all hit suns completely and wait for ongoing shots. This limit also applies during the collection animation. Native storage formats, profiles and existing savegames are not changed by additional fields or own write access.

Check manually: both card orders and glove directions; Costs, cooldowns, objective marking, cancellation and invalid fields; 30 damage, every third shot with 5-sun hit bonus, misses, shields and torchwood; 25-sun production without goal, break and wear; Original representation, head contact, brightening, muzzle position and projectile size; Glove/grid preview, water lily/pot/pumpkin; Save/load, board change and existing ice variants. Regular Peashooters and Sunflowers must keep their previous recipes.

Release build: 0 warnings, 0 errors. Plant offline preview visually checked; Build resources and installed DLL statically checked (see `references/solar-shooter/verification-0.1.21.json`). The game didn't start; Playtests remain entirely with the user. Previous version/removal: Copy `backup/FusionMod-0.1.20.dll` to `Mods/FusionMod.dll`. Before loading a save game with Solar Shooter, reinstall at least version 0.1.21.


## Change 0.1.22: Register solar projectiles to the native initialization point

The user reports on 0.1.21 common peas instead of solar projectiles. The secured log under `references/solar-shooter/shooting-0.1.22/Latest-0.1.21.log` shows the successful construction of the solar plant, but neither material construction nor an error in the solar projectile graphic. The previous recording via return values ​​of the two `Board.AddProjectile` overloads therefore does not provide any runtime proof of a successful assignment.

The assignment now takes place directly after `Projectile.ProjectileInitialize`. Statically confirmed: Both native `AddProjectile` overloads set app and board and call the same initializer (`0x6c8e60`). After it returns, the type, board and projectile state are initialized. The shared synchronous context includes `Plant.UpdateShooting` and direct `Plant.Fire` calls; nested calls restore the previous shooter via finalizer. The old solar patch on the overcharge returns is no longer applicable. Ordinary Pea Shooters without a Solar marker do not receive a Solar assignment.

The first context and registration messages are issued once per solar plant, the successful original graphic structure once per board. This makes it possible to distinguish between missing shooter context, missing projectile assignment and missing representation during user testing. Diagnostic status is cleaned at reset, death, board and mod end. Original assets, 30 damage, three-shot bonus, production, recipes and the storage limits documented in 0.1.21 remain unchanged.

Release build: 0 warnings, 0 errors. Version and embedded original assets statically checked, build and installed DLL are binary identical. Previous version: `backup/FusionMod-0.1.21.dll`; Dismantling by copying to `Mods/FusionMod.dll`. The game didn't start. Check for yourself after a full restart: visible sun projectiles, 30 damage, 5 sun on hit every third shot, common peashooters and ice variants. The cause of the runtime cannot be conclusively confirmed without this user check.


## Change 0.1.23: Accept correct Peashooter projectile type

The new user findings for 0.1.22 and its secured log limit the error to the registration: The solar shooter context is reached, but the projectile registration is not. The actual cause lies in the type filter of `SolarShooter.Track`: Replanted produces `ProjectileType.PeashooterPea` (14) for the native Peashooter plant, while the filter only accepted `ProjectileType.Pea` (0). This meant that each solar projectile was discarded before assignment; Damage, three-shot bonus and original presentation were also not allocated. Moving the registry point in 0.1.22 did not resolve this filter error.

Static evidence: `Plant.Fire` checks the plant type for RVA `0x6b06e7` to 0 and sets the projectile type to `0x0e` for `0x6b06eb`. At `0x6b0cc8` it passes this value to `Board.AddProjectile`, called at `0x6b0cd4`. The enum values ​​from the actually installed interop assembly are recorded under `references/solar-shooter/shooting-0.1.23/`.

The filter now accepts the two common pea types `Pea` and `PeashooterPea`, still only in the synchronous shooting context of a Solar plant on the same board. The additional outer interception `UpdateShooting` from 0.1.22 is removed; the statically assigned direct `Plant.Fire` context is sufficient. The common initialization point, three-shot counter, life cycle and original assets are retained. Diagnostic messages now output numeric DataIDs. Normal Peashooters are not affected by their missing solar marker.

Release build: 0 warnings, 0 errors. Version, type filters and embedded assets statically checked; installed and built DLL binary identical. Previous version is `backup/FusionMod-0.1.22.dll`; Dismantling by copying to `Mods/FusionMod.dll`. Game not started, no playtests run. User check after restart: Sun projectiles, 30 damage, every third shot with 5 sun hit bonus, Torchwood and normal peashooters. The known storage limits remain.


## Change 0.1.24: Cherryshooter

The only source recipe for Cherryshooter (ID 1001) is fully registered:

```text
Cherry Bomb | Peashooter | Cherryshooter
```

Both ingredients are native, usable replanted plants. Both orders work via seed card and glove; Target markers use the same centralized check. Card cost and card cooldown are part of the incoming ingredient (usually Cherry Bomb: 150 Sun, Peashooter: 100 Sun). For the glove, the previous costs and the previous cooldown apply. The result is created before the ingredients are consumed. The water lily, pot and pumpkin remain.

Cherryshooter shoots pink cherry projectiles with **40 direct damage at approximately 1.5 seconds of firing distance**. The native Peashooter takes care of target selection, shot distance and collision; only projectiles from the Cherryshooter synchronous shooting context receive cherry behavior. Ordinary Torchwood does not transform these projectiles. The normal cherry ball does not cause an area explosion. Description and values ​​are from the local 4.0.5.2 almanac. The description text describes Cherryshooter's dislike of jokes about his red head and his special cherry balls. Source RVAs, Almanac and Assets are documented at `references/cherry-shooter/`.

The plant identity is `Peashooter` + `Cherrybomb` in `mImitaterType`. It remains separate from regular Peashooters and Solar Shooters, including as a glove ingredient and after loading. The existing native field is saved; no foreign IDs or non-relevant counters are used. The result therefore does not trigger recipes for a normal peashooter. Further developments using cherry shooter as an ingredient remain open.

Original graphics and animations from `data.unity3d` are embedded: **CherryshooterPrefab 37187**, Transform 120505, Controller 6593, Clips 4907 (`idle`) and 4908 (`shoot`), 21 nodes and 19 sprites. The shot plane is additive, weight 1, without mask. The similarly named **CherryShooterPrefab 47422 is of the Tower class and is not used**. Original projectile `Bullet_cherry` 61266 contains three nodes and two sprites, but no animator. It remains static true to the original; the empty runtime time window `idle` does not contain any invented animation. Plant and projectile use the occupied shadow reference 0.88 × 7 × 0.23 = 1.4168 source units. Material, previews, sorting and additive animation use the existing import path. Solar and Cherry projectiles now share its display adapter; the damage and production rules remain with their plants. Hit sounds and impact effects are still from Replanted.

Storage Limit: Already flying cherry projectiles lose their volatile assignment, original appearance, 40 damage and Torchwood lock when loaded; they become common native peas. Newly fired projectiles regain cherry behavior. Plant identity and plant animation are recovered from the native fields. The additional random explosion in `Bullet_cherry.HitZombie` is explicitly tied to a Fusion travel upgrade; This foreign mode system is not part of the normal Cherryshooter Fusion and will not be ported.

The completed result entry is removed from `fusionen.txt`; the complete source archive remains unchanged. There are now 649 recipes open with 537 results. Release build: 0 warnings, 0 errors. Original assets were exported offline and the additive animation preview was visually inspected. This does not confirm any representation or function in the game. Game didn't start; no playtests, savegames or profile changes. Previous version: `backup/FusionMod-0.1.23.dll`. Dismantling by copying this DLL to `Mods/FusionMod.dll`; Before loading a save game with Cherryshooter, install a suitable mod version again.

Manual user verification:

- Both card orders; Immediately place Cherry Bomb on existing Peashooter. A Cherry Bomb planted alone may still explode normally.
- Both glove directions as far as Cherry Bomb can be picked up before the native explosion; whose detonators do not delay for more convenient fusion. Check abort, invalid targets and target marking.
- Check card cost/cooldown, glove cooldown, water lily/pot/pumpkin and ingredient usage.
- Plant animation, previews, size/ground location, muzzle position, pink projectiles, 40 direct damage, shot distance and check Torchwood. Countercheck common peashooters as well as solar and ice plants.
- Check pause, carry, death, board change and save/load; Flying cherry projectiles have the storage limit described.


## Change 0.1.25: Explode-o-shooter

The only source recipe for result ID 1005 is fully implemented:

```text
Cherry Bomb | Cherryshooter | Explode-o-shooter
```

Cherryshooter has been implemented since 0.1.24; Cherry Bomb is available natively. Both orders apply via the common recipe resolution and the glove. The seed card allows Cherry Bomb on Cherryshooter. A Cherryshooter seed card is not invented. The opposite direction uses the Gauntlet and an unexploded Cherry Bomb; their detonator is not delayed. Card cost and glove cooldown remain native. Common terrain checking, objective marking, generation before ingredient consumption, and preservation of water lilies, pots, and pumpkins remain.

### Description and behavior

The original local almanac describes an extremely irritable cherry shooter who could actually shoot the cherry stuck in his throat; his red face betrays the effort. It shoots exploding cherries **every 3 seconds** with **300 area damage**, **circular radius of a field** and **no distance decay**. The port uses native zombie permissions and hit rectangles, a circle radius of 80 board pixels, and a maximum of one adjacent row. The primary hit receives no additional pea damage. Native blast flags deal with shields and bodies. The native large explosion is avoided because it also deletes ladders. Regular Torchwood does not transform the projectiles.

The **Cherry Bomb card used for this successful fusion will receive 85% of its cooldown immediately**: normally about **7.5 seconds remain instead of 50**. The native reload counter stores the credit. The normal card duration remains unchanged; subsequent regular Cherry Bomb uses reload normally. Abandonment, Invalid Placement, Cherry Shooter Generation, and Gauntlet Fusion do not receive a card bonus. The successfully generated result is captured in the synchronous `PlayerPlantingScope` before the game calls `SeedPacket.WasPlanted`.

The almanac also lists immunity to cherry blast damage. Friendly Cherry Bombs and these projectiles don't damage plants in Replanted anyway. Do not add general invulnerability to zombie attacks, jack-in-the-box, or catapult damage. The source game's enemy cherry systems and travel upgrades will not be introduced as additional systems.

### Original assets and storage

Original plant from `data.unity3d`: **SuperCherryShooterPrefab 47421**, Transform 105493, Script `globalgamemanagers.assets:690 SuperCherryShooter`, Controller 6637, Clips 5022 (`idle`, 2 seconds) and 5023 (`shoot`, 1 second), **12 nodes and 11 Sprites**. Shot level: additive, weight 1, without mask. In the idle clip, three transform targets are already missing from the delivered prefab. As in Unity, only these documented position bindings are ignored and documented in the export manifest; other unknown bindings remain errors.

Original projectile **Bullet_superCherry 61265**, transform 104370, script `globalgamemanagers.assets:2218 Bullet_superCherry`: **three nodes, two sprites, no animator**. Original pivots, hierarchy, sorting and animations use the existing import path, including glove and grid previews. Measured shadow width: 0.88 × 6.70833397 × 0.24 = **1.4168 source units**. Plants and projectiles use this size reference. All runtime assets are embedded. **Asset Limit:** Explosion Flash (`Powie`) and Sound (`Cherrybomb`) are from Replanted; Source particles and source audio data are not loaded.

The reserved memory identifier is **Peashooter + ExplodeONut (49)** in the native `mImitaterType`. This valid enum identifier is neither impersonator marker 48 nor a source ID. The pair may not be used for other fusions. `FusionPlant.From` separates Explode-o-shooter from Peashooter, Cherryshooter and Solar-shooter. Plant identity, shot interval and running shot counter use the existing native memory fields. Loading restores the original appearance.

**Storage limit:** Already flying explosive cherries lose their volatile mapping, original graphics, explosion and Torchwood lock when loading; they become common native peas. New shots will have the correct behavior again. Wait for shots in progress before saving. Death, pool reset, board end and mod end use the common cleanup paths.

Evidence, translated description, original almanac, method extracts, export manifest and offline preview are at `references/explode-o-shooter/`. Reproducible tools:

```bash
/tmp/pvz-fusion-extract-venv/bin/python tools/research_explode_o_shooter.py
/tmp/pvz-fusion-extract-venv/bin/python tools/export_snow_repeater.py --plant explode-o-shooter
/tmp/pvz-fusion-extract-venv/bin/python tools/export_snow_repeater.py --plant explode-projectile
/tmp/pvz-fusion-extract-venv/bin/python tools/preview_snow_repeater.py --plant explode-o-shooter
```

The completed result entry is removed from `fusionen.txt`. **648 recipes with 536 results** remain open. The header does not count as a recipe. `fusionen-quellen.json` remains unchanged; Gatling Cherrybomber as a result remains open.

Release build: 0 warnings, 0 errors. Original animation visually checked offline; Installation version, embedded assets and binary equality statically checked. No game start, no playtests, no savegame or profile changes. Previous version/removal: Copy `backup/FusionMod-0.1.24.dll` to `Mods/FusionMod.dll`. Before loading a save game with Explode-o-shooter, use at least 0.1.25 again. LawnGlove remains unchanged at 1.2.2.

Manual user verification:

- Cherry Bomb on Cherry Shooter; both glove directions in the native ignition window. Check abort, invalid targets, target marking and obtaining ingredients if placement fails.
- Check costs and approximately 7.5 seconds remaining cooldown after successful card fusion. Without Fusion and with Cherry Shooter generation, Cherry Bomb must reload normally; Glove retains its own cooldown.
- Check 300 area damage, circle edge, multiple zombies, adjacent rows, armor/shields, lack of additional direct hit and three second firing distance. Torchwood may not produce Fire Pea.
- Check original art, animations, muzzle position, size/ground position, both previews, explosion flash and water lily/pot/pumpkin.
- Check pause, carry, bungee, death, board change and save/load; Flying projectiles have the stated storage limit. Check normal cherry bombs, cherry shooters, solar shooters and ice variants.


## Change 0.1.26: Visible fusion marker on imported graphics

Cherryshooter is supposed to glow as a valid target for Explode-o-shooter when holding a Cherry Bomb card. The recipe and its central objective test were already in place. The error was in the imported representation: The native color update sets the additive highlight color, while `ImportedPeaVisuals` previously only interpolated the white multiplication color value against white. `Lerp(white, white, Stärke)` does not change texture color; the visible original graphic received no highlighting.

The common plant renderer now handles additive colors over the existing `_BlendColor` and `_BlendStrength` properties of the already used native shader `Sprites/DefaultColourblend`. The existing variant `_BLEND_ON` is activated exclusively on your own plant material copy. One `MaterialPropertyBlock` maintains the current color and strength per renderer; other plants with the same material remain independent. Strength is reset with each appearance update and resets to zero when the effect is disabled. The original shadow remains unchanged. Native hover markers and damage whitening use the same path. Recipe resolution, target testing, costs and game behavior remain unchanged; there is no additional Cherryshooter special stage.

Static evidence is below `references/highlight-0.1.26/`: `Plant.UpdateReanimColor` from RVA `0x6bb0f0` sets `mHighlighted` over `ReloadedController.SetExtraAdditiveColor` (`0x7cad50`) white with alpha `196/255`. The previous import path could not show this brightening. The shader archive documents color blend properties and keyword; the secured user log documents the imported representation and successful Cherryshooter follow-up fusions in 0.1.25.

Release build: 0 warnings, 0 errors. Version and installed DLL statically checked; Installation and build are binary identical. Previous version/removal: Copy `backup/FusionMod-0.1.25.dll` to `Mods/FusionMod.dll`. LawnGlove remains unchanged. Game didn't start, no playtests, no changes to savegames or profiles.

Check manually: Hold Cherry Bomb map, view multiple Cherry Shooters as targets, cancel map and change tools. Valid cherry shooters should visibly brighten; Unsuitable Solar Shooters and Explode-o-Shooters may not receive a fusion marker. Cherry Bomb on the gauntlet, normal hover, damage whitening, pause and other imported plants cross-check. Brightening must not spread to other plants with shared material.


## Change 0.1.27: Smoother highlighting

The highlighting of imported plants now uses half the previous glare strength: for the normal white highlighting, around 0.384 instead of 0.769. This preserves more original color and details. Fusion targets and normal hover markers use the same damping; Damage whitening without simultaneous marking remains unchanged.

`FusionHighlightPatch` captures the marker after the native color update before its finalizer resets the temporary fusion flag. The existing display instance will maintain this cosmetic state until the next color update. No additional recipe check or separate global state is necessary. Previews, shadows and recipe behavior remain unchanged.

Release build: 0 warnings, 0 errors. Version and binary equality checked statically. Previous version/removal: Copy `backup/FusionMod-0.1.26.dll` to `Mods/FusionMod.dll`. Game didn't start; User testing includes brightness on Cherryshooter, cancel/tool ​​change, and unchanged damage brightening.


## Change 0.1.28: Cherrypeater and source automation

All two source recipes for result ID 1008 are implemented, in both orders:

```text
Cherry Bomb | Repeater | Cherrypeater
Cherryshooter | Peashooter | Cherrypeater
```

Both pairs of ingredients are already available. Card fusions use the existing native ingredient cards; Cherryshooter is recognized as an existing plant. The glove supports both directions. Joint target checking, highlighting, terrain checking and generation before ingredient consumption remain within existing responsibilities.

The local 4.0.5.2 almanac describes two pink cherry balls, each dealing 40 direct damage per volley at a 1.5 second source interval. Cherrypeater uses the native repeater for homing, double shot, shot counter and collision. Its native clocking, including random shortening by 0-14 simulation ticks, is retained. Both bullets use the existing Cherryshooter hit path: 40 direct damage, no area explosion, and no Torchwood conversion. The described click upgrade after 15 seconds creates Split Cherry; this still open result and its recipes are left out.

Original representation: `DoubleCherryPrefab` 37191, Transform 120509, Script `globalgamemanagers.assets:2357 DoubleCherry`, Animator 6596, Clips 4913/4914. All 22 nodes, 20 sprites and both animations are from the original local bundle. The additive firing plane and idle speed 1.3 are adopted. Both original shooting events are evidence; Projectiles are only generated by the native AI. The original cherry ball 61266 is reused from the existing resource. The floor width is 0.88 × 7 × 0.23 = 1.4168 source units. Origin, description, recipes, disassembly and offline preview are under `references/cherrypeater/`.

The persisted identity is `Repeater` + `Cherrybomb` in `mImitaterType`. `FusionPlant.From` prevents ordinary repeater subsequent fusions. Native plant fields store this identifier; the mod rebuilds graphics and shot type after loading. Cherry balls that are already flying lose their volatile special assignment when loaded and then become native peas. The appropriate mod version is required for saves with Cherrypeater.

### Reusable source research

`tools/research_fusion.py` replaces copying and adapting plant-specific research programs for further fusions. A call archives all source recipes of a result name, the local almanac entry, the actual method addresses and complete method excerpts of selected classes. With `--prefab` he additionally secures the root components, the script identity resolved via file ID as well as animator and clip IDs. Input checksums prove the sources. The DLL checksum must match the source archive; another version requires a recheck of the specified metadata offsets and method table. Unknown or ambiguous classes abort. Method tokens in the filename prevent overwriting of overloads of the same name.

Dependencies: Python with UnityPy 1.25.4, Pillow, pefile 2024.8.26 and capstone 5.0.9. For example, an environment can be created at `/tmp/pvz-fusion-research-venv`. Temporary environments do not exist permanently and must be set up again if necessary.

```bash
python3 -m venv /tmp/pvz-fusion-research-venv
/tmp/pvz-fusion-research-venv/bin/pip install UnityPy==1.25.4 Pillow pefile==2024.8.26 capstone==5.0.9
/tmp/pvz-fusion-research-venv/bin/python tools/research_fusion.py --result Cherrypeater --slug cherrypeater --type DoubleCherry --type Shooter --type Bullet_cherry --prefab 37191
/tmp/pvz-fusion-research-venv/bin/python tools/export_snow_repeater.py --plant cherrypeater
/tmp/pvz-fusion-research-venv/bin/python tools/preview_snow_repeater.py --plant cherrypeater
```

The automation was executed on Cherrypeater: two recipes, almanac ID 1008, 18 methods and script DoubleCherry recorded correctly. It does not automatically assess ingredient availability or behavioral consistency. These decisions and the registration of the asset export remain part of the workflow.

Release build: 0 warnings, 0 errors. Visually checked offline preview; embedded Cherrypeater resources compared with the export files. No game start, no playtests and no changes to profiles or savegames.

Manual user review: both recipes and both glove directions, Cherry Bomb before its native detonator expires, card cost and cooldown, valid and invalid highlighting, two Cherry Bullets with 40 damage each, Torchwood, original animation and muzzle position, pause/game speed, water lily/pot/pumpkin, save/load, and differentiation from Repeater and Snow Repeater. Split Cherry click upgrade is not offered.

Installation: Version 0.1.28 is under `Mods/FusionMod.dll`; Binary equality and version attributes were checked statically. The previous version is unchanged under `backup/FusionMod-0.1.27.dll`. To restore, copy this DLL back with the game closed; Only load saves with Cherrypeater again with 0.1.28 or a compatible newer version. LawnGlove remains unchanged. `fusionen.txt` now contains 646 open recipes for 535 results; the complete source archive remains unchanged.


## Change 0.1.29: Split Cherry and Gatling Cherry

All three source recipes for Split Cherry (1108) and all four for Gatling Cherry (1017) are implemented. The active recipes are at the top; Both ingredient orders apply. First, Split Cherry was supplemented with already available ingredients; subsequently Peashooter + Split Cherry is also available for Gatling Cherry. No locked alternative recipes for these two results remain open. A total of 29 recipes are active; the open list contains 639 recipes for 533 results. The complete source archive remains unchanged.

Split Cherry shoots pink cherry balls: one forward, two staggered backwards, each dealing 40 direct damage. Backward balls bounce back to the right on the left edge of the field. The existing coupled split-pea homing allows both heads to fire as soon as one side has a target. Gatling Cherry shoots four staggered cherry balls, each dealing 40 direct damage. The local almanac states 1.5 seconds each; The port receives native split pea/gatling pea clocking including random shortening by 0-14 simulation ticks. Both use the common Cherry hit path without AoE explosion or Torchwood conversion. There is no additional shot generation from original animations.

Original representation from the local 4.0.5.2 bundle:

- Split Cherry: CherrySplitPrefab 37192, Transform 120510, Script globalgamemanagers.assets:3373 CherrySplit, Animator 6362, Clips 4333/4334; 26 Knoten, 22 Sprites.
- Gatling Cherry: CherryGatlingPrefab 63745, Transform 92857, Script globalgamemanagers.assets:3282 CherryGatling, Animator 6591, Clips 4902/4903/4904; 22 Knoten, 18 Sprites.

Both models include original hierarchy, pivots, sprite ordering, and additive animation curves. Idle speed is 1.3. The common shadow reference is again 0.88 × 7 × 0.23 = 1.4168 source units; the existing original cherry ball 61266 is reused. Gatling Cherry's additional clip `shooting` is fully exported and embedded. This clip belongs to the special rapid fire state, the normal four-shot uses `shoot`. Source mode upgrades and the previously unported Cherrypeater and Split Cherry 15-second click upgrades remain outside of this recipe port. Both results are available through all source recipes; Gatling Cherrybomber remains open.

The memory identifiers are Splitpea + Cherrybomb and Gatlingpea + Cherrybomb respectively. Native plant fields store these pairs; When loading, the mod rebuilds the display and cherry behavior. Recipe queries always use FusionPlant.From and do not trigger regular split pea/gatling pea recipes on variants. Native terrain, map, glove, highlight and cleanup paths remain in charge. For split displays, the shared animation now also takes the second header track into account based on the native type.

Storage limit: Cherry balls that are already flying lose their volatile cherry assignment when loading; Backward balls also lose their rebound assignment. New shots work again via the variant marker. Savegames with these variants require 0.1.29 or a compatible newer mod version.

Source documents, all recipe registrations with RVAs, almanac texts, actual method excerpts, animator controllers, asset checksums and offline previews are under `references/split-cherry/` and `references/gatling-cherry/`. The research tool now supports `--type Namespace::Type`; `--type ::CherryGatling` explicitly chooses the global type instead of the class of the same name in GameLevel.RogueShooting. Unqualified ambiguities continue to break down.

Release build: 0 warnings, 0 errors. Offline animation sheets visually checked; embedded models and sprites compared with export files. Installed DLL and build are binary same; Version 0.1.29 is statically tested. Previous version/removal: Copy `backup/FusionMod-0.1.28.dll` to `Mods/FusionMod.dll` when the game is closed. LawnGlove remains unchanged. No game starts, playtests or changes to savegames and profiles.

Manual user review: all seven recipes, both possible card orders, and both glove directions; Use the glove on two mod ingredients. Cherry Bomb fuse in front of her native detonator. Check card cost/cooldown, cancellation, invalid targets, visible highlighting, water lily/pot/pumpkin, differentiation from normal and ice variants. Split Cherry with targets only on the left, only on the right and check on both sides: three cherry balls, 40 damage each, rebound and damage after rebound. Check Gatling Cherry for four bullets with 40 damage each. Torchwood is not allowed to change either type of ball. Check size/flooring, both head animations, muzzle position, glove preview, pause/game speed, death and pool reuse, and save/load. Don't expect click upgrades.

## New fusions in 21.2.0

Belch-shroom, Hungry-shroom, Trigger-shroom and Spicicle use the original graphics and animations from Fusion 4.0.5.2. Behavior, projectiles, fertilizers and source variations are described in [Port Evidence](references/fume-guard-21-2-0/port-notes.md).
