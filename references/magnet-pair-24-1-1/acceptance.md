# Magnet Blover and Lumos Magnet acceptance

Fusion 24.1.1 implements both source recipes in both ingredient orders. Its installed and tested DLL SHA-256 is `8aa81ad9c6d8c6c635c96ebf2d5ccb75c3e5edf2ce14b0daa88889c409cb6620`. The accepted FPS/startup/pool base is `ea0bcb0a32eda10b1c1a9957d61b59b8c12befc7`.

The release build has zero warnings and errors. Offline checks passed: 81 Magnet cases, 133 Bucket/controller cases, 2722 clock cases, 548 clock-save cases, 70 Football cases and 38 physical-drop snapshots. The ScaredContent suite and registration validation also passed. Offline test doubles retain existing nullable/default-field warnings.

Live V4–V6 used Fusion 24.1.0 and verified both recipe orders, native day sleep/coffee, Blover wind/balloons/fog/metal/fertilizer/pause, Lumos light levels 0–3, shield-bypassing 200 damage at LL2, light-provider removal, fertilizer, pause and a 100→120 Hz transition. Real physical bucket creation, capture, release and Blover discard were observed. The ability and imported-visual source hashes are unchanged in 24.1.1.

Live V8 used Fusion 24.1.1. Native controller confirm selected captured metal from `PlantFromBank`; cancel released the selection while preserving the magnet and metal. Confirm selected it again. During at least 100 natural ticks, the cooldown decreased from 880 at tick 1692 to 765 at tick 1807. Confirm placed one Bucket (`Potatomine`/`Magnetshroom`) at (3,0). Removing the original magnet produced no duplicate drop. No mouse input or debug cursor-state mutation performed these interactions.

Controller confirm retains valid planting, repair and fusion priority. Only the handled metal interaction skips native post-confirm cursor cleanup. Cancellation clears this mod's selection with the native controller-aware cursor reset and consumes the shared tool release, preventing an accidental shovel action. No global input or game patch is added.

The V8 run ended with exit code 0. Fresh inventory verified all 13 DLLs, mod entries, display and processes. Test profile and configuration contents were hash-equal to baseline; native logs, API metadata and Wine registry changes were retained. Both guarded joint installations passed all 14 postchecks. The original unpublished Fusion worktree and existing library checksum files remain protected.

Limits: no Almanac test was performed. Blover's expected 1039 source-cs expiry lies in the observed interval (1005,1049.5]; the exact death tick is unmeasured. Two inspected Lumos images show the body, metal and sustained white particles, but do not establish exact rendered particle curves. Native MagnetItem slot values were not directly observed, and no separate sound recording was taken. Source MagnetSystem recipients remain unported. Pointer diagnosis stays paused.

The [practical report](practical-test-report.md) identifies raw local evidence. [Machine-readable evidence](acceptance.json) pins the source, manifest and review hashes. Raw captures remain under `pvzreplanted/.test-runs/magnet-pair-resume-20261117/`; they are local investigation artifacts.
