# Rolling converter adapter regression

The unchanged source and relevant test projects at the commit recorded in `head.txt` were extracted with `git archive HEAD src tests` into an isolated temporary directory. ChomperFusionsTests failed compilation with five errors: Projectile lacked mCobTargetRow, and the TorchweedFusions/ChargeTorchFusions converter collaborators were absent. The same unchanged HEAD also reproduced the missing field and collaborators in SolarChomperTests and UmbrellaShellTests. Their shared Mine adapter also compiles PotatoProjectiles, which requires ChargeTorchFusions. The failure logs are stored beside this report.

The production code already has the field in the installed native references and both converter implementations. The defect belongs to stale offline test adapters, not the game or the new Hypno Chomper behavior. No production code changed for this issue.

The fix adds the native saved mCobTargetRow field to the Chomper and Mine Projectile doubles. A small shared TorchConverters adapter supplies neutral Convert collaborators for suites that isolate chompers, mines and rolling projectiles without torch plants. ChomperFusionsTests includes it directly; Adapters/Mine.projitems includes it for all three consumers: MineFusionsTests, SolarChomperTests and UmbrellaShellTests. No duplicate converter owner is introduced. TorchFusionsTests continues compiling both actual converters and actual RollingProjectiles; its existing integration tests verify the real movement path, ignition, saved damage and previous-target history.

Complete current regression runs passed:

- ChomperFusionsTests: 69 assertions.
- SolarChomperTests: 55 assertions.
- UmbrellaShellTests: 62 assertions.
- MineFusionsTests: 185 final assertions.
- TorchFusionsTests: 3084 checks.

The adapter-report tool also passes all seven synthetic regression tests. `affected-suites.json` records the actual evaluated MSBuild inputs for the changed adapters. Every affected suite listed there was executed above. Logs preserve compiler warnings instead of claiming warning-free builds. This adapter-only defect requires no mod patch-version change. The offline runs do not establish any new live-game behavior.
