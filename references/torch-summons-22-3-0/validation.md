# Verification checkpoints

Current frozen candidate: Plant Fusion **22.3.4**, SHA256 `7b42a0e9a509af84549a759bec55ca17f1d83f11865a735403007d8043bacd63`.

The combined live session uses DebugMod **1.3.13**, SHA256 `6443f8c604ae3b83a48e787fc5b0246a23d38d316919ec1ec9e217a01746eef5`. Its card atlas contains 334 entries; all 331 existing cards remain pixel-identical. Installation hashes are recorded in `runtime-checkpoints/rolling-installed.json`.

## Offline verification

The Release build completed without warnings or errors. `TorchFusionsTests` passed 3084 checks against the actual TorchFusions, TorchweedFusions, RollingProjectiles, ChargeTorchFusions and shared index source. `WeedFusionsTests` passed 116 checks. Registration generation is consistent; all three original model/resource sets and all 331 almanac entries pass the static resource checks. The exporter suite passed 38 checks. After the full runtime matrix passed and the orchestrator approved completion, the two completed recipe rows were removed from `fusionen.txt`; its German header was retained. The resulting list contains 284 open recipes with 226 result plants, alongside 385 active recipes.

Build and test commands use the local SDK at `/home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet`; `/usr/bin/dotnet` provides no SDK in this environment.

Final `registrations.py --check` passed. `fusion_checks.py check` reported **385 active recipes, 284 pending recipes, 0 errors**. The read-only documentation audit proposed only translating the retained German pending-list header; there were no unexpected recipe, list or README changes. Both tested DLL hashes remained unchanged after documentation finalization.

Relevant tests include separate 50/51 and 100/101 thresholds, both full-row charge rules, duplicate-crossing prevention, rejection of already ignited source232, pause, transaction-context restoration, exact 119/120/240 native fire-tick boundaries, helper 108–132 tick jitter bounds, the 80-tick original helper clip and 40-tick damage event, vehicle-specific death, and ordinary neighbor damage before plant removal. Balloon tests cover first-hit armor absorption without body overflow, 100-point modified armor, receiver damage scaling, additional helmets, recursive hits and exception cleanup. The target adapter now models native BalloonPopping as OffGround, with an explicit positive case and negative cases for unrelated off-ground targets.

## Independent live timing checkpoint

The reviewer measured the native plant clock at 120 board ticks per simulated second. On 22.3.1, the new Torchweed fire clock and helper attack/event timing passed the narrow live timing review. Raw traces remain under `../../../.test-runs/task1-timing-22-3-1/`; the review, compact measured summary and installed-candidate record are copied under `runtime-checkpoints/timing-*`. Later balloon corrections leave those clocks unchanged.

## Independent live balloon checkpoint

On the frozen 22.3.3 candidate, the reviewer observed the exact native state sequence:

| Phase | Flying armor | Body health |
| --- | ---: | ---: |
| Flying before hit | 20 | 1000 |
| First pop | 0 | 1000 |
| Next hit while Popping | 0 | 960 |
| Later hit while Walking | 0 | 920 |

The reviewer independently reran all 3077 offline checks and found no new game-log errors. The archived live review and compact native-state trace are `runtime-checkpoints/balloon-review.md` and `runtime-checkpoints/balloon-runtime.json`. Earlier failing candidates are retained in the local test-run archive as diagnostics, not passing evidence.

## Independent live rolling checkpoint

The 22.3.3 live review found that real PeaNut rolling motion bypasses native `CheckForCollision`, so the new converter was never called. Candidate 22.3.4 invokes Torchweed conversion before the existing ChargeTorch conversion in the shared rolling movement owner, matching their collision-patch priorities. Offline integration now calls actual `Move` and the motion hook, checking fire damage 40, once-only charge, cleared prior-hit history, pause/resume and existing Hot Potato conversion.

The independent 22.3.4 live review passed using naturally fired PeaNut rollers at current-board Normal 3. Torchweed produced the saved fire-nut marker and fire graphics, with exactly 40 damage. A second Torchweed neither re-ignited nor charged again. One roller pierced two targets, damaging each exactly once for 40. Native Torchwood retained 20 damage; existing Hot Potato retained 40. The reviewer independently reran all 3084 Torch checks. Review and installation records are archived as `runtime-checkpoints/rolling-review.md` and `rolling-installed.json`; detailed API traces and screenshots remain in `../../../.test-runs/task1-rolling-22-3-4/`. The orchestrator accepted the rolling issue as resolved and resumed the remaining task matrix.

## Full task matrix

The independent reviewer completed the full matrix, and the orchestrator explicitly accepted PASS. The consolidated report is archived as `runtime-checkpoints/final-review.md`. It includes both orders of both recipes, original bodies and cards, pea/snow/small-fire/rolling projectile behavior, native timing at 1× and 2×, balloon armor and popping, strict 51/101 thresholds, different full-row charge behavior, forward placement and helper identities 1057/1155, fertilizer without charge or clock resets, and both Zamboni and Catapult branches for Torchweed and the helper. Final helper checks include both helpers' non-fusible identity, the 1800-damage Roasted Squash crush and its existing Ice Pot conversion; Roasted Spikeweed's exclusion from Ice Pot is a source/code contract. Detailed evidence is retained under `../../../.test-runs/task1-two-fusions-22-3-4/`, `task1-vehicles-final-22-3-4/`, `task1-helpers-final-22-3-4/` and `task1-two-fusions-final/`.

One initial Catapult fixture reused a row after Zamboni; the remaining native ice road removed the plant before its intended pulse. An isolated check on previously unused grass at verified current-board Normal 3 passed native vehicle destruction and 40 neighbor damage with the unchanged candidate. The initial observation was fixture contamination, not a product defect. Later vehicle cases used fresh rows and Zamboni last. The independent explanation is archived in `runtime-checkpoints/catapult-fixture-review.md`, with raw isolated evidence under `../../../.test-runs/task1-catapult-abgrenzung-22-3-4/`.

These results establish the exercised runtime contracts. They do not claim an exhaustive test of every level, difficulty or third-party mod combination, or a new save/reload test for every counter. Native saved-field ownership and lifecycle guards are documented in the port notes and source review; offline doubles alone do not establish engine behavior. Existing ignored native shutdown and VersusBot issues remain separately documented by the orchestrator.

The final list and documentation changes do not alter version, gameplay, assets or either tested DLL. No worker installation, rebuild, commit or push was performed for this finalization. Release packaging and installation checks remain with the orchestrator and reviewer.
