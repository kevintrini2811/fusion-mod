# Rolling converter test adapter review

Independent review performed on 7 November 2026 against baseline commit `b9f424375a613876c40a719c54018f2dd3ddffa4`.

The unmodified committed `src` and `tests` directories were extracted into an isolated temporary directory. `ChomperFusionsTests` reproduces five compiler errors: the projectile adapter lacks `mCobTargetRow`, and the suite lacks the two converter collaborators referenced by `RollingProjectiles.Move`. The baseline log is [retained here](baseline.log).

The correction adds the native saved damage field to both affected projectile doubles and supplies a shared `TorchConverters.cs` collaborator for suites without torch plants. `ChomperFusionsTests` references it explicitly; the three consumers of the existing Mine adapter receive it through `Mine.projitems`. Production code is unchanged for this issue. The real converters remain covered by `TorchFusionsTests`; these suites do not replace them.

Only these five adapter/project files were copied into the isolated baseline:

- `tests/ChomperFusionsStubs.cs`
- `tests/MineFusionsStubs.cs`
- `tests/Adapters/TorchConverters.cs`
- `tests/Adapters/Mine.projitems`
- `tests/ChomperFusionsTests.csproj`

Both the isolated baseline with those corrections and the current working source pass all five relevant executables, run sequentially in Release: ChomperFusionsTests (69 assertions), SolarChomperTests (55), UmbrellaShellTests (62), MineFusionsTests (185), and TorchFusionsTests (3084). Logs are retained as `baseline-fixed-SUITE.log` and `current-SUITE.log` in this directory. Existing unused callback warnings in some doubles do not affect the successful exit codes.

No product fix or game runtime result is claimed by this adapter verification.
