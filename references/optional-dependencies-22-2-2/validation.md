# Optional Speed Mod and Difficulty Mod

The review checks the dependency boundary in Fusion Mod 22.2.2. Speed Mod and
Difficulty Mod are optional Melon dependencies, and the Fusion assembly no
longer has a project reference to Speed Mod. Difficulty's existing reflection
integration remains unchanged.

## Offline checks

- Release build: no errors or warnings.
- `tools/fusion_checks.py check`: 383 active recipes, 286 pending recipes,
  no errors.
- `GoldenPlantsTests`: passed.
- Code review: all direct Speed Mod type references in imported plant, zombie,
  hypno-head, smoke, and projectile visuals now use the optional adapter.
  Absent Speed Mod returns native animation time and disables reduced posing.
- Golden Bean uses the existing Speed Mod slowdown anchor when available and
  the native `TimeCycler` button otherwise.
- Fusion release no longer builds, installs, replaces, or checksums Speed Mod.

## Live procedure

Each configuration starts a fresh PvZ Replanted process through Steam with
`--debug-level=Night`. For an absent mod, its DLL is moved completely outside
the game's `Mods` directory while the game is closed. The original optional
DLLs are restored after the matrix. The official installer backs up the
previous Fusion DLL before installing 22.2.2.

The Debug Mod API places a registered Flame Pea (`Peashooter`, marker
`Jalapeno`) and a stationary native zombie. It checks the imported plant and
projectile renderers, projectile movement, real damage, and active Golden Bean
UI images. Screenshots verify Golden Bean placement beside the relevant speed
control. The Speed Mod configuration additionally switches to 20x, advances
300 simulation ticks, queries imported visual state, then restores 1x.

No profile mutations, campaign levels, or permanent resource changes are
requested. Runtime changes stay inside temporary debug sessions.

## Separate teardown finding

The dependency checks do not establish clean teardown. The initial direct
`game.shutdown` from the debug level logs three render-target release errors
and Almanac Addressables load errors. The process exits regularly after about
nine seconds. Leaving the debug level before shutdown also logs three
render-target release errors. These findings are recorded separately in the
workspace `ISSUES.md`; this dependency fix does not modify teardown.

Raw API responses and logs are retained locally in
`/home/kevin/modding/pvzreplanted/.test-runs/optional-dependencies-20261106-133331/`.
The adjacent matrix JSON retains the selected verification evidence.

## Results

All four configurations pass:

| Speed Mod | Difficulty Mod | Result |
| --- | --- | --- |
| Installed | Installed | Fusion, imported visuals, projectiles, damage, and Golden Bean UI pass. |
| Absent | Absent | Native timing and Golden Bean anchor pass; difficulty falls back to Normal. |
| Installed | Absent | Visuals and Golden Bean pass at 1x and 20x; difficulty falls back to Normal. |
| Absent | Installed | Native timing and Golden Bean anchor pass; Extreme difficulty remains active. |

The loaded assembly list confirms each missing optional mod is actually absent.
Both original optional DLLs are restored. Fusion 22.2.2 remains installed with
its previous binary retained in the installer backup. No game process remains.

Status: DONE for the optional-dependencies issue. The separately documented
teardown issue remains open.
