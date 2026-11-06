# Three-mushroom depth correction, 22.2.1

The user's screenshot `Bildschirmfoto_20261106_121950.png` showed an upper native Sun-shroom obscuring both lower Sun-shrooms. Read-only inspection of the running debug board confirmed that the saved logical orders were already distinct: 302685 for the upper plant and 302686 for both lower plants. The previous regression test covered those logical values only.

The native path is `ReloadedController.UpdateSorting`, `RenderSortedComponentRenderer.UpdateLayer`, then `SortingUtils.RemapSortingLayers`. The remapping truncates `(logicalOrder - 100000) * 65525f / 800000f` and subtracts 32763. Consequently, the observed 302685 and 302686 both become Unity order -16162. A depth tie lets renderer creation order or distance put the upper plant in front. [compression.json](compression.json) records the counterexample and corrected values. Native instructions are archived under `../shroom-sorting-components/`.

The existing slot correction still cancels the horizontal component of native plant sorting. Both lower slots now receive a 16-step logical gap, sufficient to remain distinct after the native remapping. No render ownership, position, plant identity, growth state or gameplay changes. Native and imported plants continue to use the same native sorting pipeline.

## Verification

- Fusion release build 22.2.1 and Debug Mod release build 1.3.6 succeeded without warnings or errors.
- The mushroom regression suite passed 50,622 assertions and 50,703 checks including spreading. The added regression checks all 50,000 logical baselines between 300000 and 349999 after the actual native floating-point remapping.
- Static validation passed: 383 active recipes, 286 pending recipes, zero errors. Debug API client tests passed all three cases.
- The live test `tools/smoke_shroom_depth.py` placed eight three-plant tiles in two rows: native Sun-shrooms, native Puff-shrooms, imported Pea-shrooms and mixed native/imported trios. It checked actual active Unity body renderer orders or imported SortingGroup orders, rather than saved logical order alone.
- The test passed before growth, after every native Sun-shroom matured, and through five further pause/resume and renderer-update cycles. [runtime.json](runtime.json) contains the observations. For the mature native trio in row 0, the upper body order is -16156 and both lower bodies use -16154. Row 1 uses -15337 versus -15335.
- [Young plants](young-and-imported.png) and [mature plants](grown-and-imported.png) were visually inspected. The lower caps cover the upper plant's stem, as required.

Debug Mod's read-only `plants {"visuals":true}` option exposes renderer and SortingGroup orders for this verification. The default plant response stays unchanged. The user's existing debug session was closed with explicit permission; the replacement test session was closed after verification. Native profiles and saves were not edited.
