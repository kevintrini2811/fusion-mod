# Transient projectile storage: validation and limits

Fusion 24.0.1 stores projectile provenance only in the current board's managed memory. Projectile lookup never falls back to native GridItem allocation. Board generation, full IDs, native pointer/array membership and birth invalidation protect recycled slots and stale handles. Successful native load clears projectile caches and uses native Pea initialization for explicitly marked Fusion shots. Status, crater and clock records remain native.

The obsolete companion, nonce and recovery implementation was removed after its complete source archive was verified. Existing companion files remain untouched. Old Fusion category headers require a byte-exact content-addressed original-file backup before in-memory translation to native format 14. New saves use ordinary native I/O semantics.

Release builds passed without warnings or errors. Focused linked-source checks covered 53 transient/load/migration cases, 1,073 native projectile/crater timing cases, 548 clock-save cases, 23 carrier-readiness cases and 56 Blover cases. These use native/Unity shims where documented; they do not establish runtime behavior by themselves. Independent source review accepted the implementation, the parameterless save hook and the bounded own-carrier scene-reference preparation.

Live observations were kept separate:

- Runs `110125` and `114612` observed 140 marked shots before pause and 135 stable shots afterward, with movement between observations. In `114612`, native GridItems increased from zero to five; those five records belonged to zombie statuses, with no projectile carrier.
- Run `124240` produced a real native save: SHA-256 `f874d11f018eefb5db66d0a0a10e24227ff4bd09adef00de27762b69e65377f4`, 5,009 compressed bytes, format 14, Survival category 2, level 85, positive board payload.
- Run `134057` used Fusion 24.0.1, DebugMod 1.3.32 and ZombieLibrary 1.4.8. It loaded all 35 Glacier cells and the saved clock counter 5028, reached paused Playing and saved again without a new deserialization exception. The final native file was 4,240 bytes, SHA-256 `691db1908f57c3dc68e9c65eb8fadd1983ec10e7c74715be7c6da8b67cec2c59`, format 14/category 2/level 85, with 69,342 payload bytes. Header inspection leaves board payload contents opaque.

The post-load projectile snapshots were taken at counter 6860 and contained 40 new marked Pea/Star shots, with no ID overlap with the 100 saved shots. They do not prove individual normalization. The user explicitly waived further runtime normalization proof. A later intro-only diagnostic was stopped, archived and removed from the product; DebugMod returned exactly to its reviewed 1.3.32 inputs. No additional test was required for that reversion.

Live legacy migration, foreign-projectile combinations and forced full-ID reuse remain unverified. The native roundtrip is a focused fixture result, not a general save-format or all-mod compatibility guarantee. Earlier reader and carrier failures remain preserved; the successful combined fixes do not establish sole causality for the earlier process exit.

Test DLLs and display changes were individually restored after process exit. All 13 installed DLL records, the eleven prior owner files and display state matched their before-images. The final stopped probe's unchanged temporary input was explicitly restored to its successful native before-version. Actual profile, registry, Steam and log deltas were retained. The known documented shutdown RenderTexture warning was not suppressed.

Local raw evidence and independent reports are retained under `pvzreplanted/.test-runs/plant-shooting-performance-20261112/run-20261112T081645Z/pool-transient-projectiles-20261116/`, including `griditem-read-20261116/targeted-reload-20261116T134057Z/practical-test.md` and `result-review.md`. Player files are not included in this source reference.
