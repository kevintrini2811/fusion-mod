# Hoverleaf and Drone-nut

Blover and Umbrella Leaf make Hoverleaf. Blover and Wall-nut make Drone-nut. Both recipes accept both ingredient orders. Their Replanted identities are `Blover/Umbrella` and `Blover/Wallnut`. Existing AirPlants placement owns the independent air slot on land, pool and roof boards. Plant Library registers both results with `UnlockPolicy.Never`.

## Source contract

The local English 4.0.5.2 build was investigated through [REA](https://github.com/morluto/rea) 6.10.1 and its Ghidra 12.2 provider. The analyzed GameAssembly SHA-256 is `48096b917ee6aaf6e35c95c98e666a4399a9e0c3d5adbab727cee941eccf2d42`. The analysis reused the preceding metadata-linked PE project with method-address seeds for the selected bodies.

`BloverUmbrella`, plant 1204, is a flying utility with own-tile protection. `Block` at RVA `0x4916a0` redirects incoming target projectile motion and `BlockBungi` at `0x4915d0` rejects the bungee. Source-specific special projectile effects use types absent from this port's native umbrella route. Replanted retains its basketball and bungee deflection and its Foley events. This variant is an exact-tile fallback to `Board.FindUmbrellaPlant`. Native umbrella coverage and priority remain available.

`NutBlover`, plant 1255, has 4000 toughness and takes damage in place of other plants on its tile. Its own `Instead` at `0x404b10` declines a second substitution. The `FixedUpdate` body at `0x4d2990` requires the plant to be active and its last-damage age to exceed ten seconds. It then restores one health point per fixed source step. The source description specifies 20 milliseconds per point. `Fertilize.Upgrade` applies ordinary healing and operates on the separate attribute countdown. The no-damage clock receives no special fertilizer reset.

REA results are local under `pvzreplanted/.test-runs/task1-20261120-pair-02/`. `rea-air-BloverUmbrella.json` and `rea-air-NutBlover.json` hold the selected code. `rea-candidate-read-1204-0.json` and `rea-candidate-read-1255-0.json` are bounded reads of the original source descriptions. The project metadata is retained in the preceding run.

## Replanted behavior

The native bite and hostile-projection damage observers redirect actual numerical loss. A lethal incoming hit still protects its original recipient if the drone was eligible when damage resolved. Generation checks prevent recycled defenders taking the hit. Shovel, fusion consumption and direct native destruction remain separate from numerical hit transfer. One native saved clock stores the ten-second delay and two-tick regeneration cadence. Pause and carrying suspend the clock. Both plants have no attack projectile of their own.

Hoverleaf uses the existing Umbrella Cap artwork and block clip. Drone-nut uses the existing Frost Wall-nut artwork and damage stages. Their reused models are scaled and lifted above the ground layer. The two presentation entries use original English and German descriptions.
