# Projectile performance at normal speed

The issue was reported against Speed Mod, including 1x. Profiling isolates the dominant cost in Fusion's three torch-conversion hooks; Speed Mod's scheduler and visual behavior remain unchanged.

## Evidence before the fix

Reproducible temporary Day debug board, Difficulty 3, original graphics settings, five frozen native Normal targets, native Gatlingpeas in 0/7/21/35-plant stages. Measurements use native frame and simulation-counter deltas against wall time, with warm-up before each window. Original raw data are under `pvzreplanted/.test-runs/projectile-performance-20261106/`.

Uninstrumented baseline with Fusion 22.2.4:

| Gatlings | FPS | Simulation ticks/s | Live projectiles |
| --- | ---: | ---: | ---: |
| 7 | 59.04 | 120.12 | 12-24 |
| 21 | 2.77 | 110.60 | 53-65 |
| 35 | 1.275 | 51.00 | 91-102 |

Control with Fusion temporarily removed, otherwise unchanged:

| Gatlings | FPS | Simulation ticks/s | Live projectiles |
| --- | ---: | ---: | ---: |
| 21 | 59.997 | 119.994 | 78-96 |
| 35 | 59.991 | 119.983 | 139-149 |

A separate temporary diagnostic mod measures inclusive method time. In the targeted 35-Gatling window (5.70 seconds, 31,517 pea collision checks):

| Method | Inclusive milliseconds |
| --- | ---: |
| Projectile.CheckForCollision | 4560 |
| ChargeTorchFusions.Convert | 1173 |
| TorchFusions.Convert | 1168 |
| SupportTorchFusions.Convert | 1913 |
| SupportTorchFusions.Stumps (included above) | 1869 |
| Native FindCollisionTarget | 109 |

The three conversion hooks account for about 93% of collision time. Each repeatedly scanned the entire native plant population for every ordinary pea, even without conversion hosts. This creates a plant-count times projectile-count cost. Crossing the CPU threshold also causes the native accumulator to hit its 40-tick-per-frame ceiling; this amplifies frame stalls at 1x. Extending a scheduler budget would hide this cost rather than remove it.

## Change and invariants

`TorchPlantIndex` indexes the coarse native host types Torchwood/Potatomine once per board tick. No marker or mutable gameplay state is cached. All existing identity, active-state, position, row, conversion order, damage and graphics checks remain in their owners. SupportTorch now filters identity before its more expensive active-state test.

Plant initialization and death invalidate before and after callbacks; board disposal clears the cache. Population-count changes and a new board/tick also refresh it. Candidate DataID and board checks reject pooled reuse. Position and eligibility remain live, so glove movement and detachment cannot reuse stale coordinates. Coarse membership accommodates fusion marker writes after initialization. Rebuilding replaces arrays so an in-flight enumeration survives nested lifecycle callbacks.

Release build succeeds with zero warnings/errors. The existing torch contracts and new index regression pass 3036 checks. In particular, 3000 converter queries against 35 ordinary plants perform 35 population reads within one tick, then refresh exactly once on the next tick. Tests also cover late marker assignment, same-tick addition/replacement, movement, death, pooled generations, board changes and retained in-flight snapshots.

## Final validation

The temporary profiler was removed before the final comparison. Windowed 1920x1026, VSync enabled, five frozen targets, eight seconds of warm-up and twenty seconds per measurement window were preserved. Only Fusion changed from 22.2.4 to 22.2.5; Speed remained 1.0.6.

| Gatlings | Before FPS | After FPS | Before ticks/s | After ticks/s | After projectiles |
| --- | ---: | ---: | ---: | ---: | ---: |
| 0 | 59.94 | 59.95 | 119.78 | 119.96 | 0 |
| 7 | 59.04 | 60.00 | 120.12 | 119.95 | 16-20 |
| 21 | 2.77 | 60.02 | 110.60 | 120.04 | 58-62 |
| 35 | 1.275 | 54.67 | 51.00 | 119.95 | 96-102 |

The heaviest stage improves frame throughput about 43-fold and restores real-time simulation (120 native ticks per second). The remaining frame cost is visible in this workload; the result does not claim unlimited projectile capacity or a locked 60 FPS.

Live Debug API checks verify Nutwood (40 damage), Hot Potato small fire peas (40), Scorchwood (50), Stenchwood (30), and deleted/recreated Nutwood (40). Every case observes active visible projectile renderers; Hot Potato additionally verifies its source SmallFireProjectile model. All five cases pass again after leaving and recreating the debug board. The last case also moves Nutwood through another row and back using the glove placement implementation before verifying ignition and damage. No errors or exceptions appear in the final session log before shutdown.

`runtime.json` contains settings, counter samples and projectile populations for uninstrumented before/after and the no-Fusion control. `profile-before.jsonl` contains the separate diagnostic timings; `conversions.json` records both live conversion rounds. Full original API traces remain in the raw-data directory above. `benchmark.py` and `verify-converters.py` preserve the exact local harnesses; run the benchmark only on a disposable Day debug board, and keep profiler runs separate from FPS comparisons.

No profiler or new persistent setting is shipped.
