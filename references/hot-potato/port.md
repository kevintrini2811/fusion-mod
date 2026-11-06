# Hot Potato port contract

Source: local Fusion 4.0.5.2, pinned by `research-inputs.json`. `TorchMine.OnTriggerEnter2D` at 0x489f90 accepts small peas, potato peas and rolling nuts. Small and potato peas gain 20 fire damage. Potato peas grant eight ignition credits; the other accepted projectiles grant one. The strict comparison is greater than 100. The counter is reset before attempting a summon. `SummonMine` at 0x48a1c0 scans forward from the next tile to the rightmost tile. It creates source plant 1422, the existing non-fusible Roasted Potato. `DieEvent` creates a 1800-damage potato explosion.

The port uses native Potatomine + Torchwood identity, edible Notready state and no contact fuse or buried growth. The saved target counter owns ignition charge. Conversions preserve small-pea and potato-pea trajectories, set explicit ground collision categories and use the original small/yellow fire-pea models. Rolling nuts double their saved damage and clear their previous-hit set. Fertilizer heals without arming or evolving this passive stump. Source model 44436 exports both `idle` and `fire` clips. The four existing Roasted forms remain the shared summon implementations.

Only one source recipe exists: Potato Mine + Torchwood. Both ingredient orders use the existing transaction. No native Replanted plant duplicates this result. Source ID: 1421.
