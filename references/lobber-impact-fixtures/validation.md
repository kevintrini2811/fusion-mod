# Lobber impact fixture review

Independent review performed on 7 November 2026 against baseline commit `b9f424375a613876c40a719c54018f2dd3ddffa4`.

## Reproduction and cause

The unmodified committed source and tests were extracted with `git archive HEAD src tests` into an isolated temporary directory. Running `ShootingPotsTests.csproj` in Release reproduced `hypno splash damage: expected 39, got 0` (exit 134). After correcting only that fixture, the same baseline reproduced `melon splash five per garlic point: expected 48, got 0` (exit 134).

These adapters call `ElementalLobbers.Impact` directly without advancing the native flight. Their projectile remains at its launch position (x=120, y=230), while the neighboring-row splash target occupies x=390..410, y=110..150. That target is outside the source-derived splash radius of 88.8889 pixels. Moving the projectile to the direct target rectangle center (x=400, y=230) places the neighboring target 80 pixels away, inside the same unchanged radius.

The Hypno Melon, Garlic-pult and Summer Melon cases now use a test-only `MoveToImpact` helper before calling Impact. Their expected damage and status assertions remain unchanged. No production behavior changes address this issue.

## Verification

1. The isolated baseline, with only those three fixture corrections, passes the complete `ShootingPotsTests` executable: `Shooting pot and lobber contracts passed.` (exit 0).
2. The current working source, including the new Bolt-pult contracts, independently passes the complete same executable (exit 0).
3. The change is confined to test setup; existing direct-hit, splash-damage, hypnosis, garlic and heat assertions remain active.

Command used for both successful runs:

```sh
rtk proxy /home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet run --project PATH/tests/ShootingPotsTests.csproj -c Release
```

The retained logs are [unmodified baseline](baseline.log), [baseline after the first correction](fixed.log), and [baseline after all three corrections](fixed-all.log). This is an offline fixture verification, not a claim about executed game behavior.
