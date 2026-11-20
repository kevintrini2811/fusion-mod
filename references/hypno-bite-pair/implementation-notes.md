# Curse-shroom and Zodiac-shroom

Round 13 uses REA 6.10.1 against the PvZ Fusion 4.0.5.2 GameAssembly SHA-256 `48096b917ee6aaf6e35c95c98e666a4399a9e0c3d5adbab727cee941eccf2d42`. The source archive records one recipe per result, each in both orders. Curse-shroom (1045) combines Hypno-shroom and Doom-shroom. Zodiac-shroom (1365) combines Hypno-shroom and Starfruit. The variants reuse the existing Hypno-shroom renderer and the ordinary mushroom placement and sleep rules.

## Curse-shroom

`Zombie.EatEffect` at `0x180653420` sets `isDoom` for plant ID `0x415` before the hypnosis action. `Zombie.Die` at `0x180650f80` clears the flag before using `BoardAction.SetDoom` with 1800 damage. The source exit handler `0x180659ba0` invokes the same death path. The zombie constructor enables `doomWithPit`.

The port extends the established native Hypno-shroom bite owner. It binds the effect to the existing zombie's complete saved ID and consumes the plant after hypnosis succeeds. The Replanted death and removal callbacks take a position snapshot and retire the record before triggering the existing Doom damage and crater owner. Board disposal retires the board state. The burst and crater use existing native resources.

## Zodiac-shroom

`StarHypno.OnEat` at `0x18045b9d0` schedules 60 salvos with delays of `0.1 * index` seconds. Its callback at `0x18045fa30` checks that the zombie actor is still active, then creates five sea stars at the actor. The rotation at `0x18045fc5f` uses `20 * (index + 3 * star)` degrees. The port reuses the existing sea-star projectile family and schedules the five 20-damage stars in canonical source ticks. The record stores its pending salvo count and delay. A dead or released owner ends the schedule. The native game owns the stars' movement, with the existing family supplying damage and impact presentation.

The two saved records are distinct from the Ember and Trippy effects. Failed or resisted hypnosis leaves the plant and retires the pending record. Neither source variant has a separate fertilizer action in this port's source mode.
