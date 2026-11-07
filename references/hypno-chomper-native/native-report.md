# Native behavior analysis: Replanted

This report collects static evidence. It does not confirm gameplay behavior.

## Limitations

- Callers: direct calls and external jumps from Assembly-CSharp method ranges, including `jmp`.
- Ranges end at the next method pointer or section boundary. Interior addresses are only attribution candidates.
- Ranges may contain additional native helpers; in that case the reported caller name is also only a candidate.
- Indirect calls, native functions outside these ranges, and embedded copies still require manual tracing.
- Aliases share the same address. Automatic target and lifecycle selection relies on method names.
- The display limits aliases to three and per-method lists to 20 entries. The JSON contains the complete lists.
- Field evidence consists of candidate offsets without proof of the object type. `lea` only establishes an address.
- Field searches cover methods of the declared field owner. External writers and register aliases require manual review.
- Missing matches do not prove unused helpers, available fields, or absent persistence.

## Coverage

5779 unique ranges; 692 methods without an analyzable range.
18 ranges where decoding ended early.

## Persistence and field candidates

### Reloaded.Gameplay.Plant.mTargetX at 0xcc

- PlantInitialize [0x6001a0e] `0x6b3ab2` write: `mov qword ptr [rbx + 0xcc], 0xffffffffffffffff`.
- PlantInitialize [0x6001a0e] `0x6b3f5b` read: `mov rcx, qword ptr [rax + 0xc8]`.
- Fire [0x6001a16] `0x6b0db1` read: `mov eax, dword ptr [rbx + 0xcc]`.
- Fire [0x6001a16] `0x6b0dc1` write: `movss dword ptr [rdi + 0xcc], xmm0`.
- Fire [0x6001a16] `0x6b0de0` read: `mov edx, dword ptr [rbx + 0xcc]`.
- UpdateProductionPlant [0x6001a19] `0x6bae7b` read: `mov rcx, qword ptr [rax + 0xc8]`.
- UpdateSquash [0x6001a2d] `0x6bd3be` read: `movd xmm1, dword ptr [rbx + 0xcc]`.
- UpdateSquash [0x6001a2d] `0x6bd3dc` read: `mov r12d, dword ptr [rbx + 0xcc]`.
- UpdateSquash [0x6001a2d] `0x6bd81d` write: `mov dword ptr [rbx + 0xcc], ecx`.
- UpdateSquash [0x6001a2d] `0x6bd98a` write: `mov dword ptr [rbx + 0xcc], ecx`.
- UpdateBlink [0x6001a41] `0x6b7b80` read: `mov rcx, qword ptr [rax + 0xc8]`.
- CobCannonFire [0x6001a5f] `0x6ac56d` write: `mov dword ptr [rbx + 0xcc], eax`.
- Reset [0x6001a78] `0x6b4fb0` write: `mov qword ptr [rbx + 0xcc], rdi`.
- Serialize [0x6001a79] `0x6b526a` read: `mov edx, dword ptr [rdi + 0xcc]`.
- Deserialize [0x6001a7a] `0x6ac789` address: `lea rdx, [rbx + 0xcc]`.

### Reloaded.Gameplay.Plant.mTargetY at 0xd0

- PlantInitialize [0x6001a0e] `0x6b3ab2` write: `mov qword ptr [rbx + 0xcc], 0xffffffffffffffff`.
- Draw [0x6001a13] `0x6aef23` read: `mov rcx, qword ptr [rax + 0xd0]`.
- Fire [0x6001a16] `0x6b0dd6` read: `mov r8d, dword ptr [rbx + 0xd0]`.
- Fire [0x6001a16] `0x6b0deb` write: `mov dword ptr [rdi + 0xd0], eax`.
- GetRefreshTime [0x6001a21] `0x6b1b4f` read: `mov rax, qword ptr [rbx + 0xd0]`.
- UpdateTanglekelp [0x6001a59] `0x6bdf0c` read: `mov rcx, qword ptr [rsi + 0xd0]`.
- CobCannonFire [0x6001a5f] `0x6ac573` write: `mov dword ptr [rbx + 0xd0], esi`.
- Reset [0x6001a78] `0x6b4fb0` write: `mov qword ptr [rbx + 0xcc], rdi`.
- Serialize [0x6001a79] `0x6b527b` read: `mov edx, dword ptr [rdi + 0xd0]`.
- Deserialize [0x6001a7a] `0x6ac79b` address: `lea rdx, [rbx + 0xd0]`.

## Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]

requested, lifecycle/target candidate

RVA `0x6b0040`; range end `0x6b0580`; evidence [replanted-0x6001a17.asm](replanted-0x6001a17.asm).

Callers:

- `0x6afbab` `call`: Reloaded.Gameplay.Plant.FindTargetAndFire [0x6001a1b] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b274c` `call`: Reloaded.Gameplay.Plant.LaunchThreepeater [0x6001a1c] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b277c` `call`: Reloaded.Gameplay.Plant.LaunchThreepeater [0x6001a1c] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b27b0` `call`: Reloaded.Gameplay.Plant.LaunchThreepeater [0x6001a1c] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b6472` `call`: Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b65c6` `call`: Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b771f` `call`: Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b7828` `call`: Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b7eb5` `call`: Reloaded.Gameplay.Plant.UpdateBowling [0x6001a54] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b852e` `call`: Reloaded.Gameplay.Plant.UpdateCactus [0x6001a57] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b85fa` `call`: Reloaded.Gameplay.Plant.UpdateCactus [0x6001a57] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b89b9` `call`: Reloaded.Gameplay.Plant.UpdateChomper [0x6001a3f] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6b8add` `call`: Reloaded.Gameplay.Plant.UpdateChomper [0x6001a3f] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6ba75c` `call`: Reloaded.Gameplay.Plant.UpdatePotato [0x6001a48] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6bc95d` `call`: Reloaded.Gameplay.Plant.UpdateShooting [0x6001a4c] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6bcac5` `call`: Reloaded.Gameplay.Plant.UpdateShooting [0x6001a4c] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6bd22a` `call`: Reloaded.Gameplay.Plant.UpdateSpikeweed [0x6001a51] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).
- `0x6bde53` `call`: Reloaded.Gameplay.Plant.UpdateTanglekelp [0x6001a59] to `0x6b0040` (Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17]).

Targets:

- `0x6b006e` `call` to `0x4feb70`: unresolved native address.
- `0x6b007a` `call` to `0x4feb70`: unresolved native address.
- `0x6b0086` `call` to `0x4feb70`: unresolved native address.
- `0x6b010c` `call` to `0x6b12f0`: Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29].
- `0x6b0126` `call` to `0x6b1660`: Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b].
- `0x6b01aa` `call` to `0x2449330`: unresolved native address.
- `0x6b01d8` `call` to `0x6f3830`: Reloaded.Gameplay.Zombie.IsTangleKelpTarget [0x6001d95].
- `0x6b0211` `call` to `0x7e3010`: Reloaded.TreeStateActivities.GameplayActivity.get_GameMode [0x6000ea6].
- `0x6b0270` `call` to `0x67db70`: Reloaded.Gameplay.Challenge.CanTargetZombieWithPortals [0x6001837].
- `0x6b0297` `call` to `0x6f12f0`: Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7].
- `0x6b03bb` `call` to `0x6f2d70`: Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0].
- `0x6b040b` `call` to `0x4feeb0`: unresolved native address.
- `0x6b041d` `call` to `0x6cbaa0`: Reloaded.Gameplay.Common.GetRectOverlap [0x6001bd8].
- `0x6b0473` `call` to `0x4feeb0`: unresolved native address.
- `0x6b04ad` `call` to `0x6cb840`: Reloaded.Gameplay.Common.Distance2D [0x6001bcd].
- `0x6b04bd` `call` to `0x6f3650`: Reloaded.Gameplay.Zombie.IsFlying [0x6001d2a].
- `0x6b0572` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26]

requested

RVA `0x6b6170`; range end `0x6b7ac0`; evidence [replanted-0x6001a26.asm](replanted-0x6001a26.asm).

Callers:

- `0x6b9b4b` `call`: Reloaded.Gameplay.Plant.UpdateInternal [0x6001a11] to `0x6b6170` (Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26]).
- `0x6be26c` `call`: Reloaded.Gameplay.Plant.Update [0x6001a10] to `0x6b6170` (Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26]).

Targets:

- `0x6b618c` `call` to `0x4feb70`: unresolved native address.
- `0x6b6198` `call` to `0x4feb70`: unresolved native address.
- `0x6b61e8` `call` to `0x7e3010`: Reloaded.TreeStateActivities.GameplayActivity.get_GameMode [0x6000ea6].
- `0x6b6205` `call` to `0x7e3010`: Reloaded.TreeStateActivities.GameplayActivity.get_GameMode [0x6000ea6].
- `0x6b6277` `call` to `0x2040`: unresolved native address.
- `0x6b628d` `call` to `0x6b5700`: Reloaded.Gameplay.Plant.SetSleeping [0x6001a4b].
- `0x6b62af` `call` to `0x6bc710`: Reloaded.Gameplay.Plant.UpdateShooting [0x6001a4c].
- `0x6b62d7` `call` to `0x4feeb0`: unresolved native address.
- `0x6b62e4` `call` to `0x3832900`: unresolved native address.
- `0x6b62fc` `call` to `0x7dfa20`: Reloaded.TreeStateActivities.GameplayActivity.IsWallnutBowlingLevel [0x6000ef0].
- `0x6b630a` `call` to `0x6b7d50`: Reloaded.Gameplay.Plant.UpdateBowling [0x6001a54].
- `0x6b6428` `call` to `0x6bbef0`: Reloaded.Gameplay.Plant.UpdateScaredyShroom [0x6001a4f].
- `0x6b6437` `call` to `0x6bdc80`: Reloaded.Gameplay.Plant.UpdateTanglekelp [0x6001a59].
- `0x6b6451` `call` to `0x4feb70`: unresolved native address.
- `0x6b6472` `call` to `0x6b0040`: Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17].
- `0x6b6485` `call` to `0x6b5b90`: Reloaded.Gameplay.Plant.SpikeweedAttack [0x6001a68].
- `0x6b64ca` `call` to `0x6ad520`: Reloaded.Gameplay.Plant.DoRowAreaDamage [0x6001a28].
- `0x6b64ee` `call` to `0x7c7a30`: Source.Controllers.ReloadedCharacterController.GetLoopCount [0x6000c4c].
- `0x6b650b` `call` to `0x4feeb0`: unresolved native address.
- `0x6b6523` `call` to `0x6cbd40`: Reloaded.Gameplay.Common.RandRangeFloat [0x6001bde].
- Another 155 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]

requested

RVA `0x6b12f0`; range end `0x6b13b0`; evidence [replanted-0x6001a29.asm](replanted-0x6001a29.asm).

Callers:

- `0x6ac126` `call`: Reloaded.Gameplay.Plant.BurnRow [0x6001a30] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6ad127` `call`: Reloaded.Gameplay.Plant.DistanceToClosestZombie [0x6001a50] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6ad59f` `call`: Reloaded.Gameplay.Plant.DoRowAreaDamage [0x6001a28] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6ad814` `call`: Reloaded.Gameplay.Plant.DoSpecial [0x6001a15] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6ae274` `call`: Reloaded.Gameplay.Plant.DoSquashDamage [0x6001a2f] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6af450` `call`: Reloaded.Gameplay.Plant.FindSquashTarget [0x6001a2c] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6af814` `call`: Reloaded.Gameplay.Plant.FindStarFruitTarget [0x6001a3e] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6b010c` `call`: Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6b0cee` `call`: Reloaded.Gameplay.Plant.Fire [0x6001a16] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6b0d80` `call`: Reloaded.Gameplay.Plant.Fire [0x6001a16] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6b5f85` `call`: Reloaded.Gameplay.Plant.StarFruitFire [0x6001a58] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6b83c3` `call`: Reloaded.Gameplay.Plant.UpdateBowling [0x6001a54] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).
- `0x6ba7fd` `call`: Reloaded.Gameplay.Plant.UpdatePotato [0x6001a48] to `0x6b12f0` (Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29]).

Targets:

- No direct external targets found.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b]

requested, lifecycle/target candidate

RVA `0x6b1660`; range end `0x6b1a00`; evidence [replanted-0x6001a2b.asm](replanted-0x6001a2b.asm).

Callers:

- `0x6ad13d` `call`: Reloaded.Gameplay.Plant.DistanceToClosestZombie [0x6001a50] to `0x6b1660` (Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b]).
- `0x6ad5b5` `call`: Reloaded.Gameplay.Plant.DoRowAreaDamage [0x6001a28] to `0x6b1660` (Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b]).
- `0x6ae289` `call`: Reloaded.Gameplay.Plant.DoSquashDamage [0x6001a2f] to `0x6b1660` (Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b]).
- `0x6af466` `call`: Reloaded.Gameplay.Plant.FindSquashTarget [0x6001a2c] to `0x6b1660` (Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b]).
- `0x6b0126` `call`: Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17] to `0x6b1660` (Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b]).
- `0x6b695a` `call`: Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26] to `0x6b1660` (Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b]).
- `0x6ba813` `call`: Reloaded.Gameplay.Plant.UpdatePotato [0x6001a48] to `0x6b1660` (Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b]).
- `0x6bdfc4` `call`: Reloaded.Gameplay.Plant.UpdateTorchwood [0x6001a3c] to `0x6b1660` (Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b]).
- `0x6c8b97` `call`: Reloaded.Gameplay.Projectile.PeaAboutToHitTorchwood [0x6001aa6] to `0x6b1660` (Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b]).

Targets:

- `0x6b168d` `call` to `0x7dfa20`: Reloaded.TreeStateActivities.GameplayActivity.IsWallnutBowlingLevel [0x6000ef0].
- `0x6b19eb` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.UpdateChomper [0x6001a3f]

requested

RVA `0x6b8760`; range end `0x6b8b90`; evidence [replanted-0x6001a3f.asm](replanted-0x6001a3f.asm).

Callers:

- No direct callers found in the analyzed range.

Targets:

- `0x6b8779` `call` to `0x4feb70`: unresolved native address.
- `0x6b8785` `call` to `0x4feb70`: unresolved native address.
- `0x6b8791` `call` to `0x4feb70`: unresolved native address.
- `0x6b879d` `call` to `0x4feb70`: unresolved native address.
- `0x6b880a` `call` to `0x7c7a30`: Source.Controllers.ReloadedCharacterController.GetLoopCount [0x6000c4c].
- `0x6b8821` `call` to `0x6b4c50`: Reloaded.Gameplay.Plant.PlayIdleAnim [0x6001a72].
- `0x6b8867` `call` to `0x60f880`: SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072]).
- `0x6b889f` `call` to `0x7c8270`: Source.Controllers.ReloadedCharacterController.PlayAnimationOnTrack [0x6000c2c].
- `0x6b88d3` `call` to `0x7c7a30`: Source.Controllers.ReloadedCharacterController.GetLoopCount [0x6000c4c].
- `0x6b890b` `call` to `0x7c8210`: Source.Controllers.ReloadedCharacterController.PlayAnimationOnTrack [0x6000c2d].
- `0x6b891f` `call` to `0x7df5d0`: Reloaded.TreeStateActivities.GameplayActivity.IsIZombieLevel [0x6000edf].
- `0x6b8941` `call` to `0x7c8740`: Source.Controllers.ReloadedCharacterController.SetAnimationRateOnTrack [0x6000c31].
- `0x6b89a8` `call` to `0x2040`: unresolved native address.
- `0x6b89b9` `call` to `0x6b0040`: Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17].
- `0x6b89f0` `call` to `0x6f3670`: Reloaded.Gameplay.Zombie.IsImmobilizied [0x6001d41].
- `0x6b89fe` `call` to `0x6f35e0`: Reloaded.Gameplay.Zombie.IsBouncingPogo [0x6001d47].
- `0x6b8a37` `call` to `0x6ec320`: Reloaded.Gameplay.Zombie.DieWithLoot [0x6001ccc].
- `0x6b8a96` `call` to `0x2040`: unresolved native address.
- `0x6b8ab2` `call` to `0x6f8f60`: Reloaded.Gameplay.Zombie.TakeDamage [0x6001cd1].
- `0x6b8add` `call` to `0x6b0040`: Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17].
- Another 4 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.DieWithLoot [0x6001ccc]

requested, lifecycle/target candidate

RVA `0x6ec320`; range end `0x6ec340`; evidence [replanted-0x6001ccc.asm](replanted-0x6001ccc.asm).

Callers:

- `0x686085` `call`: Reloaded.Gameplay.Challenge.MouseDownWhackAZombie [0x6001823] to `0x6ec320` (Reloaded.Gameplay.Zombie.DieWithLoot [0x6001ccc]).
- `0x6acdbf` `call`: Reloaded.Gameplay.Plant.Die [0x6001a18] to `0x6ec320` (Reloaded.Gameplay.Zombie.DieWithLoot [0x6001ccc]).
- `0x6b7798` `call`: Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26] to `0x6ec320` (Reloaded.Gameplay.Zombie.DieWithLoot [0x6001ccc]).
- `0x6b8a37` `call`: Reloaded.Gameplay.Plant.UpdateChomper [0x6001a3f] to `0x6ec320` (Reloaded.Gameplay.Zombie.DieWithLoot [0x6001ccc]).
- `0x6bde32` `call`: Reloaded.Gameplay.Plant.UpdateTanglekelp [0x6001a59] to `0x6ec320` (Reloaded.Gameplay.Zombie.DieWithLoot [0x6001ccc]).
- `0x6c43c4` `jmp`: Reloaded.Gameplay.LawnMower.MowZombie [0x60019e3] to `0x6ec320` (Reloaded.Gameplay.Zombie.DieWithLoot [0x6001ccc]).

Targets:

- `0x6ec32b` `call` to `0x6ebf90`: Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca].
- `0x6ec33a` `jmp` to `0x6f0500`: Reloaded.Gameplay.Zombie.DropLoot [0x6001ce9].

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]

requested

RVA `0x6f12f0`; range end `0x6f1600`; evidence [replanted-0x6001cd7.asm](replanted-0x6001cd7.asm).

Callers:

- `0x664d4f` `call`: Reloaded.Gameplay.Board.KillAllZombiesInRadius [0x6001762] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6ac1b6` `call`: Reloaded.Gameplay.Plant.BurnRow [0x6001a30] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6ad1cb` `call`: Reloaded.Gameplay.Plant.DistanceToClosestZombie [0x6001a50] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6ad669` `call`: Reloaded.Gameplay.Plant.DoRowAreaDamage [0x6001a28] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6ae346` `call`: Reloaded.Gameplay.Plant.DoSquashDamage [0x6001a2f] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6af563` `call`: Reloaded.Gameplay.Plant.FindSquashTarget [0x6001a2c] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6af8ea` `call`: Reloaded.Gameplay.Plant.FindStarFruitTarget [0x6001a3e] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6b0297` `call`: Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6ba89b` `call`: Reloaded.Gameplay.Plant.UpdatePotato [0x6001a48] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6c2699` `call`: Reloaded.Gameplay.GridItem.RakeFindZombie [0x60019d1] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6c4ce7` `call`: Reloaded.Gameplay.LawnMower.Update [0x60019de] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6c6d3e` `call`: Reloaded.Gameplay.Projectile.CheckForCollision [0x6001a97] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6c853d` `call`: Reloaded.Gameplay.Projectile.FindCollisionTarget [0x6001a98] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6c8a0e` `call`: Reloaded.Gameplay.Projectile.IsZombieHitBySplash [0x6001aa5] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).
- `0x6c9f5a` `call`: Reloaded.Gameplay.Projectile.UpdateNormalMotion [0x6001a9f] to `0x6f12f0` (Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7]).

Targets:

- `0x6f130f` `call` to `0x4feb70`: unresolved native address.
- `0x6f132b` `call` to `0x4feeb0`: unresolved native address.
- `0x6f136e` `call` to `0x4feeb0`: unresolved native address.
- `0x6f13e5` `call` to `0x7c78e0`: Source.Controllers.ReloadedCharacterController.GetAnimationTimeOnTrack [0x6000c35].
- `0x6f140a` `call` to `0x7c78e0`: Source.Controllers.ReloadedCharacterController.GetAnimationTimeOnTrack [0x6000c35].
- `0x6f1439` `call` to `0x6f1f20`: Reloaded.Gameplay.Zombie.GetBobsledPosition [0x6001d48].
- `0x6f14cf` `call` to `0x6f2d70`: Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0].
- `0x6f1519` `call` to `0x4feeb0`: unresolved native address.
- `0x6f1541` `call` to `0x4feeb0`: unresolved native address.
- `0x6f1561` `call` to `0x4feeb0`: unresolved native address.
- `0x6f1588` `call` to `0x4feeb0`: unresolved native address.
- `0x6f15d9` `call` to `0x4feeb0`: unresolved native address.
- `0x6f15f5` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]

requested

RVA `0x6f2d70`; range end `0x6f2e30`; evidence [replanted-0x6001cf0.asm](replanted-0x6001cf0.asm).

Callers:

- `0x664d7c` `call`: Reloaded.Gameplay.Board.KillAllZombiesInRadius [0x6001762] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x66515d` `call`: Reloaded.Gameplay.Board.MouseDownButterUpZombie [0x60017bb] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x677310` `call`: Reloaded.Gameplay.Board.ZombiesWon [0x6001702] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x685f67` `call`: Reloaded.Gameplay.Challenge.MouseDownWhackAZombie [0x6001823] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x690d19` `call`: Reloaded.Gameplay.Challenge.UpdatePortal [0x6001835] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6ac02f` `call`: Reloaded.Gameplay.Plant.BlowAwayFliers [0x6001a32] / Reloaded.Gameplay.Plant.BlowAwayFliers [0x6001a33] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6ad1df` `call`: Reloaded.Gameplay.Plant.DistanceToClosestZombie [0x6001a50] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6ad681` `call`: Reloaded.Gameplay.Plant.DoRowAreaDamage [0x6001a28] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6ae176` `call`: Reloaded.Gameplay.Plant.DoSpecial [0x6001a15] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6ae35a` `call`: Reloaded.Gameplay.Plant.DoSquashDamage [0x6001a2f] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6af57b` `call`: Reloaded.Gameplay.Plant.FindSquashTarget [0x6001a2c] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6af8d5` `call`: Reloaded.Gameplay.Plant.FindStarFruitTarget [0x6001a3e] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6b03bb` `call`: Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6b0f00` `call`: Reloaded.Gameplay.Plant.Fire [0x6001a16] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6b9e15` `call`: Reloaded.Gameplay.Plant.UpdateMagnetShroom [0x6001a43] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6ba8af` `call`: Reloaded.Gameplay.Plant.UpdatePotato [0x6001a48] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6bc025` `call`: Reloaded.Gameplay.Plant.UpdateScaredyShroom [0x6001a4f] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6c26ad` `call`: Reloaded.Gameplay.GridItem.RakeFindZombie [0x60019d1] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6c4cfb` `call`: Reloaded.Gameplay.LawnMower.Update [0x60019de] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- `0x6c6d6e` `call`: Reloaded.Gameplay.Projectile.CheckForCollision [0x6001a97] to `0x6f2d70` (Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0]).
- Another 8 entries are available in full in native-analysis.json.

Targets:

- `0x6f2d95` `call` to `0x6f3a20`: Reloaded.Gameplay.Zombie.IsWalkingBackwards [0x6001d00].
- `0x6f2dbe` `call` to `0x6f2170`: Reloaded.Gameplay.Zombie.GetDrawPos [0x6001ce5].

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]

requested, lifecycle/target candidate

RVA `0x6f3600`; range end `0x6f3620`; evidence [replanted-0x6001d53.asm](replanted-0x6001d53.asm).

Callers:

- `0x655eb9` `call`: Reloaded.Gameplay.Board.AreEnemyZombiesOnScreen [0x60016ac] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x6561e1` `call`: Reloaded.Gameplay.Board.BungeeIsTargetingCell [0x6001749] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x658d62` `call`: Reloaded.Gameplay.Board.CountZombiesOnScreen [0x600177e] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x65e332` `call`: Reloaded.Gameplay.Board.GetLiveGargantuarCount [0x60016c6] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x66318f` `call`: Reloaded.Gameplay.Board.KeyChar [0x60016ce] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x664e20` `call`: Reloaded.Gameplay.Board.KillAllZombiesInRadius [0x6001762] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x665145` `call`: Reloaded.Gameplay.Board.MouseDownButterUpZombie [0x60017bb] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x66dc70` `call`: Reloaded.Gameplay.Board.RemoveAllZombies [0x60016c2] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x66decd` `call`: Reloaded.Gameplay.Board.RemoveZombiesForRepick [0x60017b6] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x6725d7` `call`: Reloaded.Gameplay.Board.TotalZombiesHealthInWave [0x600170c] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x674696` `call`: Reloaded.Gameplay.Board.UpdateLevelEndSequence [0x6001742] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x6753a1` `call`: Reloaded.Gameplay.Board.UpdateProgressMeter [0x600172b] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x677344` `call`: Reloaded.Gameplay.Board.ZombiesWon [0x6001702] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x6842cb` `call`: Reloaded.Gameplay.Challenge.IZombieUpdate [0x6001859] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x685f4f` `call`: Reloaded.Gameplay.Challenge.MouseDownWhackAZombie [0x6001823] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x68b9c3` `call`: Reloaded.Gameplay.Challenge.SpawnLevelAward [0x600182a] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x6ac01b` `call`: Reloaded.Gameplay.Plant.BlowAwayFliers [0x6001a32] / Reloaded.Gameplay.Plant.BlowAwayFliers [0x6001a33] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x6ae162` `call`: Reloaded.Gameplay.Plant.DoSpecial [0x6001a15] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x6b9e67` `call`: Reloaded.Gameplay.Plant.UpdateMagnetShroom [0x6001a43] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- `0x6bc04c` `call`: Reloaded.Gameplay.Plant.UpdateScaredyShroom [0x6001a4f] to `0x6f3600` (Reloaded.Gameplay.Zombie.IsDeadOrDying [0x6001d53]).
- Another 3 entries are available in full in native-analysis.json.

Targets:

- No direct external targets found.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant..ctor [0x6001a0b]

lifecycle/target candidate

RVA `0x6be2d0`; range end `0x6be7c0`; evidence [replanted-0x6001a0b.asm](replanted-0x6001a0b.asm).

Callers:

- `0x6a77fc` `call`: Reloaded.Gameplay.CutScene.UpdateUpsell [0x600191c] to `0x6be2d0` (Reloaded.Gameplay.Plant..ctor [0x6001a0b]).
- `0x6a788e` `call`: Reloaded.Gameplay.CutScene.UpdateUpsell [0x600191c] to `0x6be2d0` (Reloaded.Gameplay.Plant..ctor [0x6001a0b]).
- `0x6a7920` `call`: Reloaded.Gameplay.CutScene.UpdateUpsell [0x600191c] to `0x6be2d0` (Reloaded.Gameplay.Plant..ctor [0x6001a0b]).
- `0x6c53fe` `call`: Reloaded.Gameplay.MessageWidget.Deserialize [0x60019fd] to `0x6be470` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x1a0).
- `0x6c540d` `call`: Reloaded.Gameplay.MessageWidget.Deserialize [0x60019fd] to `0x6be470` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x1a0).
- `0x6c54b3` `call`: Reloaded.Gameplay.MessageWidget.Deserialize [0x60019fd] to `0x6be470` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x1a0).
- `0x6c54c5` `call`: Reloaded.Gameplay.MessageWidget.Deserialize [0x60019fd] to `0x6be470` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x1a0).
- `0x6c5627` `call`: Reloaded.Gameplay.MessageWidget.Serialize [0x60019fc] to `0x6be580` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x2b0).
- `0x6c5640` `call`: Reloaded.Gameplay.MessageWidget.Serialize [0x60019fc] to `0x6be580` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x2b0).
- `0x6c56d9` `call`: Reloaded.Gameplay.MessageWidget.Serialize [0x60019fc] to `0x6be580` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x2b0).
- `0x6c56f5` `call`: Reloaded.Gameplay.MessageWidget.Serialize [0x60019fc] to `0x6be580` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x2b0).
- `0x6c9fe3` `call`: Reloaded.Gameplay.Projectile.UpdateNormalMotion [0x6001a9f] to `0x6be690` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x3c0).
- `0x6ca02e` `call`: Reloaded.Gameplay.Projectile.UpdateNormalMotion [0x6001a9f] to `0x6be690` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x3c0).
- `0x6ca630` `call`: Reloaded.Gameplay.Reanimation.Deserialize [0x6001ad8] to `0x6be470` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x1a0).
- `0x6ca666` `call`: Reloaded.Gameplay.Reanimation.Deserialize [0x6001ad8] to `0x6be470` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x1a0).
- `0x6ca68a` `call`: Reloaded.Gameplay.Reanimation.Deserialize [0x6001ad8] to `0x6be470` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x1a0).
- `0x6cab28` `call`: Reloaded.Gameplay.Reanimation.Serialize [0x6001ad7] to `0x6be580` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x2b0).
- `0x6cab67` `call`: Reloaded.Gameplay.Reanimation.Serialize [0x6001ad7] to `0x6be580` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x2b0).
- `0x6cab95` `call`: Reloaded.Gameplay.Reanimation.Serialize [0x6001ad7] to `0x6be580` (range candidate: Reloaded.Gameplay.Plant..ctor [0x6001a0b] + 0x2b0).
- `0x78ee37` `call`: Source.UI.CreditsSunflowerController.Setup [0x60008dd] to `0x6be2d0` (Reloaded.Gameplay.Plant..ctor [0x6001a0b]).
- Another 15 entries are available in full in native-analysis.json.

Targets:

- `0x6be2ed` `call` to `0x4feb70`: unresolved native address.
- `0x6be2f9` `call` to `0x4feb70`: unresolved native address.
- `0x6be305` `call` to `0x4feb70`: unresolved native address.
- `0x6be311` `call` to `0x4feb70`: unresolved native address.
- `0x6be338` `call` to `0x4fdf20`: unresolved native address.
- `0x6be34e` `call` to `0x4fdea0`: unresolved native address.
- `0x6be35f` `call` to `0x4fdf20`: unresolved native address.
- `0x6be375` `call` to `0x4fdea0`: unresolved native address.
- `0x6be37f` `call` to `0x6ce700`: Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e].
- `0x6be38b` `call` to `0x1137af0`: unresolved native address.
- `0x6be3a1` `call` to `0x4fdea0`: unresolved native address.
- `0x6be3d3` `call` to `0x4fedd0`: unresolved native address.
- `0x6be3e0` `call` to `0x60f020`: AudioSequence/DelayedAudioSequenceData..ctor [0x600000d] / ByteWrite..ctor [0x600008b] / UnitySourceGeneratedAssemblyMonoScriptTypes_v1..ctor [0x60000b8] / 184 more aliases (JSON aliases[6352928]).
- `0x6be3f9` `call` to `0x4fdec0`: unresolved native address.
- `0x6be41a` `call` to `0x4fdea0`: unresolved native address.
- `0x6be44f` `call` to `0x4fee20`: unresolved native address.
- `0x6be455` `call` to `0x4fe790`: unresolved native address.
- `0x6be45f` `call` to `0x4fede0`: unresolved native address.
- `0x6be465` `call` to `0x4fee10`: unresolved native address.
- `0x6be49e` `call` to `0x4feb70`: unresolved native address.
- Another 21 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.PlantInitialize [0x6001a0e]

lifecycle/target candidate, field-offset candidate

RVA `0x6b3890`; range end `0x6b4b80`; evidence [replanted-0x6001a0e.asm](replanted-0x6001a0e.asm).

Callers:

- `0x668f5f` `call`: Reloaded.Gameplay.Board.NewPlant [0x600177a] to `0x6b3890` (Reloaded.Gameplay.Plant.PlantInitialize [0x6001a0e]).
- `0x69e475` `call`: Reloaded.Gameplay.CutScene.AddFlowerPots [0x6001902] to `0x6b3890` (Reloaded.Gameplay.Plant.PlantInitialize [0x6001a0e]).
- `0x78efaa` `call`: Source.UI.CreditsSunflowerController.Setup [0x60008dd] to `0x6b3890` (Reloaded.Gameplay.Plant.PlantInitialize [0x6001a0e]).
- `0x7bb23a` `call`: Source.Binders.AlmanacPlantBinder.SetupPlant [0x6000d26] to `0x6b3890` (Reloaded.Gameplay.Plant.PlantInitialize [0x6001a0e]).

Targets:

- `0x6b38be` `call` to `0x4feb70`: unresolved native address.
- `0x6b38ca` `call` to `0x4feb70`: unresolved native address.
- `0x6b38d6` `call` to `0x4feb70`: unresolved native address.
- `0x6b38e2` `call` to `0x4feb70`: unresolved native address.
- `0x6b38ee` `call` to `0x4feb70`: unresolved native address.
- `0x6b38fa` `call` to `0x4feb70`: unresolved native address.
- `0x6b3906` `call` to `0x4feb70`: unresolved native address.
- `0x6b3912` `call` to `0x4feb70`: unresolved native address.
- `0x6b391e` `call` to `0x4feb70`: unresolved native address.
- `0x6b392a` `call` to `0x4feb70`: unresolved native address.
- `0x6b3936` `call` to `0x4feb70`: unresolved native address.
- `0x6b3942` `call` to `0x4feb70`: unresolved native address.
- `0x6b394e` `call` to `0x4feb70`: unresolved native address.
- `0x6b395a` `call` to `0x4feb70`: unresolved native address.
- `0x6b3966` `call` to `0x4feb70`: unresolved native address.
- `0x6b3972` `call` to `0x4feb70`: unresolved native address.
- `0x6b397e` `call` to `0x4feb70`: unresolved native address.
- `0x6b398a` `call` to `0x4feb70`: unresolved native address.
- `0x6b3996` `call` to `0x4feb70`: unresolved native address.
- `0x6b39a2` `call` to `0x4feb70`: unresolved native address.
- Another 114 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- `0x6b419e`: `call rax`; target unresolved.

## Reloaded.Gameplay.Plant.Draw [0x6001a13]

field-offset candidate

RVA `0x6aede0`; range end `0x6af140`; evidence [replanted-0x6001a13.asm](replanted-0x6001a13.asm).

Callers:

- `0x6b9b94` `jmp`: Reloaded.Gameplay.Plant.UpdateInternal [0x6001a11] to `0x6aede0` (Reloaded.Gameplay.Plant.Draw [0x6001a13]).
- `0x6be2b5` `jmp`: Reloaded.Gameplay.Plant.Update [0x6001a10] to `0x6aede0` (Reloaded.Gameplay.Plant.Draw [0x6001a13]).
- `0x78f2b6` `call`: Source.UI.CreditsSunflowerController.Update [0x60008dc] to `0x6aede0` (Reloaded.Gameplay.Plant.Draw [0x6001a13]).
- `0x7bb425` `call`: Source.Binders.AlmanacPlantBinder.Update [0x6000d25] to `0x6aede0` (Reloaded.Gameplay.Plant.Draw [0x6001a13]).

Targets:

- `0x6aee02` `call` to `0x4feb70`: unresolved native address.
- `0x6aee32` `call` to `0x6b33c0`: Reloaded.Gameplay.Plant.PlantDrawHeightOffset [0x6001a0d].
- `0x6aee89` `call` to `0x65eba0`: Reloaded.Gameplay.Board.GetPumpkinAt [0x600172f].
- `0x6aeeb2` `call` to `0x65f2a0`: Reloaded.Gameplay.Board.GetTopPlantAt [0x60016fb].
- `0x6aeec9` `call` to `0x63cd70`: GRPC.NET.PushPullStream.get_Position [0x6000857] / Source.Controllers.ReloadedCharacterController.GetFrameCount [0x6000c3b] / Source.Controllers.ReloadedCharacterController.FindTrackIndex [0x6000c5e] / 45 more aliases (JSON aliases[6540656]).
- `0x6aeef2` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6aef0c` `call` to `0x2448850`: unresolved native address.
- `0x6aef9b` `call` to `0x6ae840`: Reloaded.Gameplay.Plant.DrawShadow [0x6001a4e].
- `0x6aefc3` `call` to `0x3840220`: unresolved native address.
- `0x6aefd8` `call` to `0x4feb70`: unresolved native address.
- `0x6aeff3` `call` to `0x4feeb0`: unresolved native address.
- `0x6af016` `call` to `0x7e3010`: Reloaded.TreeStateActivities.GameplayActivity.get_GameMode [0x6000ea6].
- `0x6af02f` `call` to `0x7e3010`: Reloaded.TreeStateActivities.GameplayActivity.get_GameMode [0x6000ea6].
- `0x6af048` `call` to `0x7df5d0`: Reloaded.TreeStateActivities.GameplayActivity.IsIZombieLevel [0x6000edf].
- `0x6af074` `call` to `0x67fd40`: Reloaded.Gameplay.Challenge.IZombieDrawPlant [0x600185a].
- `0x6af07d` `call` to `0x63cd70`: GRPC.NET.PushPullStream.get_Position [0x6000857] / Source.Controllers.ReloadedCharacterController.GetFrameCount [0x6000c3b] / Source.Controllers.ReloadedCharacterController.FindTrackIndex [0x6000c5e] / 45 more aliases (JSON aliases[6540656]).
- `0x6af0ec` `call` to `0x6ab080`: range candidate: Reloaded.Gameplay.Board/ZombiePicker..ctor [0x60017d3] + 0x310.
- `0x6af107` `call` to `0x6ae500`: Reloaded.Gameplay.Plant.DrawMagnetItems [0x6001a45].
- `0x6af12c` `call` to `0x4fee10`: unresolved native address.
- `0x6af132` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- `0x6aef8d`: `call rax`; target unresolved.

## Reloaded.Gameplay.Plant.Fire [0x6001a16]

field-offset candidate

RVA `0x6b0580`; range end `0x6b1120`; evidence [replanted-0x6001a16.asm](replanted-0x6001a16.asm).

Callers:

- `0x6afdf6` `call`: Reloaded.Gameplay.Plant.FindTargetAndFire [0x6001a1b] to `0x6b0580` (Reloaded.Gameplay.Plant.Fire [0x6001a16]).
- `0x6bc8ea` `call`: Reloaded.Gameplay.Plant.UpdateShooting [0x6001a4c] to `0x6b0580` (Reloaded.Gameplay.Plant.Fire [0x6001a16]).
- `0x6bc974` `call`: Reloaded.Gameplay.Plant.UpdateShooting [0x6001a4c] to `0x6b0580` (Reloaded.Gameplay.Plant.Fire [0x6001a16]).
- `0x6bc9b1` `call`: Reloaded.Gameplay.Plant.UpdateShooting [0x6001a4c] to `0x6b0580` (Reloaded.Gameplay.Plant.Fire [0x6001a16]).
- `0x6bc9f0` `call`: Reloaded.Gameplay.Plant.UpdateShooting [0x6001a4c] to `0x6b0580` (Reloaded.Gameplay.Plant.Fire [0x6001a16]).
- `0x6bca32` `call`: Reloaded.Gameplay.Plant.UpdateShooting [0x6001a4c] to `0x6b0580` (Reloaded.Gameplay.Plant.Fire [0x6001a16]).
- `0x6bca6a` `call`: Reloaded.Gameplay.Plant.UpdateShooting [0x6001a4c] to `0x6b0580` (Reloaded.Gameplay.Plant.Fire [0x6001a16]).
- `0x6bcaa6` `call`: Reloaded.Gameplay.Plant.UpdateShooting [0x6001a4c] to `0x6b0580` (Reloaded.Gameplay.Plant.Fire [0x6001a16]).
- `0x6bcba6` `call`: Reloaded.Gameplay.Plant.UpdateShooting [0x6001a4c] to `0x6b0580` (Reloaded.Gameplay.Plant.Fire [0x6001a16]).

Targets:

- `0x6b05aa` `call` to `0x4feb70`: unresolved native address.
- `0x6b05b6` `call` to `0x4feb70`: unresolved native address.
- `0x6b075b` `call` to `0x2040`: unresolved native address.
- `0x6b0810` `call` to `0x4fbc00`: unresolved native address.
- `0x6b089a` `call` to `0x2040`: unresolved native address.
- `0x6b0a29` `call` to `0x6b15d0`: Reloaded.Gameplay.Plant.GetPeaHeadOffset [0x6001a37].
- `0x6b0a89` `call` to `0x6b15d0`: Reloaded.Gameplay.Plant.GetPeaHeadOffset [0x6001a37].
- `0x6b0ab3` `call` to `0x6b15d0`: Reloaded.Gameplay.Plant.GetPeaHeadOffset [0x6001a37].
- `0x6b0ada` `call` to `0x6b15d0`: Reloaded.Gameplay.Plant.GetPeaHeadOffset [0x6001a37].
- `0x6b0bb6` `call` to `0x65de10`: Reloaded.Gameplay.Board.GetFlowerPotAt [0x6001730].
- `0x6b0bf8` `call` to `0x665070`: Reloaded.Gameplay.Board.MakeRenderOrder [0x6001737].
- `0x6b0c32` `call` to `0x665070`: Reloaded.Gameplay.Board.MakeRenderOrder [0x6001737].
- `0x6b0c6f` `call` to `0x665070`: Reloaded.Gameplay.Board.MakeRenderOrder [0x6001737].
- `0x6b0ca7` `call` to `0x7d9b50`: Reloaded.TreeStateActivities.GameplayActivity.AddTodParticle [0x6000f16].
- `0x6b0cd4` `call` to `0x655650`: Reloaded.Gameplay.Board.AddProjectile [0x60016b6].
- `0x6b0cee` `call` to `0x6b12f0`: Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29].
- `0x6b0d80` `call` to `0x6b12f0`: Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29].
- `0x6b0de6` `call` to `0x66be90`: Reloaded.Gameplay.Board.PixelToGridYKeepOnBoard [0x60016f2].
- `0x6b0e1a` `call` to `0x676eb0`: Reloaded.Gameplay.Board.ZombieGetID [0x6001713].
- `0x6b0f00` `call` to `0x6f2d70`: Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0].
- Another 16 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- `0x6b065d`: `jmp rax`; target unresolved.
- `0x6b081f`: `call r8`; target unresolved.

Decoder ends early at `0x6b10f1`. Inspect the remainder manually.

## Reloaded.Gameplay.Plant.Die [0x6001a18]

lifecycle/target candidate

RVA `0x6acd40`; range end `0x6ad0c0`; evidence [replanted-0x6001a18.asm](replanted-0x6001a18.asm).

Callers:

- `0x663cb5` `call`: Reloaded.Gameplay.Board.KeyChar [0x60016ce] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x664abf` `call`: Reloaded.Gameplay.Board.KillAllPlantsInRadius [0x600172e] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x664c1f` `call`: Reloaded.Gameplay.Board.KillAllPlantsInRadius [0x600172d] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x665e45` `call`: Reloaded.Gameplay.Board.MouseDownWithPlant [0x60016f5] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x665e92` `call`: Reloaded.Gameplay.Board.MouseDownWithPlant [0x60016f5] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x665f40` `call`: Reloaded.Gameplay.Board.MouseDownWithPlant [0x60016f5] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x665f5b` `call`: Reloaded.Gameplay.Board.MouseDownWithPlant [0x60016f5] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x666267` `call`: Reloaded.Gameplay.Board.MouseDownWithPlant [0x60016f5] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x666299` `call`: Reloaded.Gameplay.Board.MouseDownWithPlant [0x60016f5] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x666e15` `call`: Reloaded.Gameplay.Board.MouseDownWithTool [0x60016f6] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x66db38` `call`: Reloaded.Gameplay.Board.RemoveAllPlants [0x60016c0] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x67b91f` `call`: Reloaded.Gameplay.Challenge.BeghouledPacketClicked [0x600183d] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x67ba3f` `call`: Reloaded.Gameplay.Challenge.BeghouledPacketClicked [0x600183d] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x67bb5f` `call`: Reloaded.Gameplay.Challenge.BeghouledPacketClicked [0x600183d] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x67c24c` `call`: Reloaded.Gameplay.Challenge.BeghouledRemoveHorizontalMatch [0x6001808] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x67c48c` `call`: Reloaded.Gameplay.Challenge.BeghouledRemoveVerticalMatch [0x6001809] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x67cb48` `call`: Reloaded.Gameplay.Challenge.BeghouledShuffle [0x600183e] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x67fa6a` `call`: Reloaded.Gameplay.Challenge.GraveDangerSpawnGraveAt [0x6001829] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x67fccf` `call`: Reloaded.Gameplay.Challenge.GraveDangerSpawnRandomGrave [0x6001828] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- `0x687629` `call`: Reloaded.Gameplay.Challenge.PuzzleNextStageClear [0x6001852] to `0x6acd40` (Reloaded.Gameplay.Plant.Die [0x6001a18]).
- Another 45 entries are available in full in native-analysis.json.

Targets:

- `0x6acd5d` `call` to `0x4feb70`: unresolved native address.
- `0x6acd69` `call` to `0x4feb70`: unresolved native address.
- `0x6acd75` `call` to `0x4feb70`: unresolved native address.
- `0x6acd81` `call` to `0x4feb70`: unresolved native address.
- `0x6acdb0` `call` to `0x677050`: Reloaded.Gameplay.Board.ZombieTryToGet [0x6001715].
- `0x6acdbf` `call` to `0x6ec320`: Reloaded.Gameplay.Zombie.DieWithLoot [0x6001ccc].
- `0x6acdd1` `call` to `0x63cd70`: GRPC.NET.PushPullStream.get_Position [0x6000857] / Source.Controllers.ReloadedCharacterController.GetFrameCount [0x6000c3b] / Source.Controllers.ReloadedCharacterController.FindTrackIndex [0x6000c5e] / 45 more aliases (JSON aliases[6540656]).
- `0x6acdee` `call` to `0x4feeb0`: unresolved native address.
- `0x6acdfb` `call` to `0x3832900`: unresolved native address.
- `0x6ace1a` `call` to `0x7e14a0`: Reloaded.TreeStateActivities.GameplayActivity.RemoveReanimation [0x6000f29].
- `0x6ace21` `call` to `0x63cd70`: GRPC.NET.PushPullStream.get_Position [0x6000857] / Source.Controllers.ReloadedCharacterController.GetFrameCount [0x6000c3b] / Source.Controllers.ReloadedCharacterController.FindTrackIndex [0x6000c5e] / 45 more aliases (JSON aliases[6540656]).
- `0x6ace69` `call` to `0x9c0bc0`: unresolved native address.
- `0x6acea3` `call` to `0x24484c0`: unresolved native address.
- `0x6aceac` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6aceca` `call` to `0x657e60`: Reloaded.Gameplay.Board.ClearCursor [0x60016aa].
- `0x6acee6` `call` to `0x6b4e30`: Reloaded.Gameplay.Plant.RemoveEffects [0x6001a62].
- `0x6acf13` `call` to `0x65e170`: Reloaded.Gameplay.Board.GetLadderAt [0x6001783].
- `0x6acf22` `call` to `0x6c2390`: Reloaded.Gameplay.GridItem.GridItemDie [0x60019c3].
- `0x6acf59` `call` to `0x65f2a0`: Reloaded.Gameplay.Board.GetTopPlantAt [0x60016fb].
- `0x6acf78` `call` to `0x65de10`: Reloaded.Gameplay.Board.GetFlowerPotAt [0x6001730].
- Another 13 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.UpdateProductionPlant [0x6001a19]

field-offset candidate

RVA `0x6bacd0`; range end `0x6bb0f0`; evidence [replanted-0x6001a19.asm](replanted-0x6001a19.asm).

Callers:

- `0x6b6d2c` `call`: Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26] to `0x6bacd0` (Reloaded.Gameplay.Plant.UpdateProductionPlant [0x6001a19]).
- `0x6b6eaf` `call`: Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26] to `0x6bacd0` (Reloaded.Gameplay.Plant.UpdateProductionPlant [0x6001a19]).
- `0x6bdc5f` `call`: Reloaded.Gameplay.Plant.UpdateSunShroom [0x6001a53] to `0x6bacd0` (Reloaded.Gameplay.Plant.UpdateProductionPlant [0x6001a19]).

Targets:

- `0x6bace9` `call` to `0x4feb70`: unresolved native address.
- `0x6bacf5` `call` to `0x4feb70`: unresolved native address.
- `0x6bad37` `call` to `0x7e3010`: Reloaded.TreeStateActivities.GameplayActivity.get_GameMode [0x6000ea6].
- `0x6bad54` `call` to `0x7e3010`: Reloaded.TreeStateActivities.GameplayActivity.get_GameMode [0x6000ea6].
- `0x6bad71` `call` to `0x7df5d0`: Reloaded.TreeStateActivities.GameplayActivity.IsIZombieLevel [0x6000edf].
- `0x6bad8d` `call` to `0x7e3010`: Reloaded.TreeStateActivities.GameplayActivity.get_GameMode [0x6000ea6].
- `0x6badaa` `call` to `0x7e3010`: Reloaded.TreeStateActivities.GameplayActivity.get_GameMode [0x6000ea6].
- `0x6badc7` `call` to `0x65f8a0`: Reloaded.Gameplay.Board.HasLevelAwardDropped [0x600172a].
- `0x6bae2f` `call` to `0x7e3010`: Reloaded.TreeStateActivities.GameplayActivity.get_GameMode [0x6000ea6].
- `0x6baeb5` `call` to `0x4feeb0`: unresolved native address.
- `0x6baedc` `call` to `0x6cc120`: Reloaded.Gameplay.Common.TodAnimateCurve [0x6001be4].
- `0x6baf1a` `call` to `0x4feeb0`: unresolved native address.
- `0x6baf2a` `call` to `0x6cbdf0`: Reloaded.Gameplay.Common.RandRangeInt [0x6001bdd].
- `0x6baf66` `call` to `0x2040`: unresolved native address.
- `0x6bafb3` `call` to `0x654ea0`: Reloaded.Gameplay.Board.AddCoin [0x60016b8].
- `0x6bb006` `call` to `0x654ea0`: Reloaded.Gameplay.Board.AddCoin [0x60016b8].
- `0x6bb021` `call` to `0x4feeb0`: unresolved native address.
- `0x6bb02d` `call` to `0x6cbe80`: Reloaded.Gameplay.Common.Rand [0x6001bdf].
- `0x6bb063` `call` to `0x654ea0`: Reloaded.Gameplay.Board.AddCoin [0x60016b8].
- `0x6bb073` `call` to `0x7e3010`: Reloaded.TreeStateActivities.GameplayActivity.get_GameMode [0x6000ea6].
- Another 2 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.FindTargetAndFire [0x6001a1b]

lifecycle/target candidate

RVA `0x6afb20`; range end `0x6b0040`; evidence [replanted-0x6001a1b.asm](replanted-0x6001a1b.asm).

Callers:

- `0x6bc4c7` `call`: Reloaded.Gameplay.Plant.UpdateShooter [0x6001a1a] to `0x6afb20` (Reloaded.Gameplay.Plant.FindTargetAndFire [0x6001a1b]).
- `0x6bc4f6` `call`: Reloaded.Gameplay.Plant.UpdateShooter [0x6001a1a] to `0x6afb20` (Reloaded.Gameplay.Plant.FindTargetAndFire [0x6001a1b]).
- `0x6bc50f` `call`: Reloaded.Gameplay.Plant.UpdateShooter [0x6001a1a] to `0x6afb20` (Reloaded.Gameplay.Plant.FindTargetAndFire [0x6001a1b]).
- `0x6bc6b5` `call`: Reloaded.Gameplay.Plant.UpdateShooter [0x6001a1a] to `0x6afb20` (Reloaded.Gameplay.Plant.FindTargetAndFire [0x6001a1b]).
- `0x6bc6e1` `call`: Reloaded.Gameplay.Plant.UpdateShooter [0x6001a1a] to `0x6afb20` (Reloaded.Gameplay.Plant.FindTargetAndFire [0x6001a1b]).
- `0x6bc6fa` `call`: Reloaded.Gameplay.Plant.UpdateShooter [0x6001a1a] to `0x6afb20` (Reloaded.Gameplay.Plant.FindTargetAndFire [0x6001a1b]).

Targets:

- `0x6afb4c` `call` to `0x4feb70`: unresolved native address.
- `0x6afb58` `call` to `0x4feb70`: unresolved native address.
- `0x6afb64` `call` to `0x4feb70`: unresolved native address.
- `0x6afb70` `call` to `0x4feb70`: unresolved native address.
- `0x6afb7c` `call` to `0x4feb70`: unresolved native address.
- `0x6afb88` `call` to `0x4feb70`: unresolved native address.
- `0x6afb94` `call` to `0x4feb70`: unresolved native address.
- `0x6afbab` `call` to `0x6b0040`: Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17].
- `0x6afbce` `call` to `0x7c7650`: Source.Controllers.ReloadedCharacterController.DisableBlink [0x6000c33].
- `0x6afbff` `call` to `0x60f880`: SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072]).
- `0x6afc3a` `call` to `0x7c8270`: Source.Controllers.ReloadedCharacterController.PlayAnimationOnTrack [0x6000c2c].
- `0x6afc72` `call` to `0x7c8940`: Source.Controllers.ReloadedCharacterController.TrackExistsOnTrack [0x6000c29].
- `0x6afcc6` `call` to `0x60f880`: SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072]).
- `0x6afd01` `call` to `0x7c8270`: Source.Controllers.ReloadedCharacterController.PlayAnimationOnTrack [0x6000c2c].
- `0x6afd4d` `call` to `0x7c8740`: Source.Controllers.ReloadedCharacterController.SetAnimationRateOnTrack [0x6000c31].
- `0x6afd84` `call` to `0x7c8740`: Source.Controllers.ReloadedCharacterController.SetAnimationRateOnTrack [0x6000c31].
- `0x6afdd8` `call` to `0x7c8940`: Source.Controllers.ReloadedCharacterController.TrackExistsOnTrack [0x6000c29].
- `0x6afdf6` `call` to `0x6b0580`: Reloaded.Gameplay.Plant.Fire [0x6001a16].
- `0x6afe2c` `call` to `0x6b4b80`: Reloaded.Gameplay.Plant.PlayBodyReanim [0x6001a42].
- `0x6afea1` `call` to `0x4feeb0`: unresolved native address.
- Another 7 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.GetRefreshTime [0x6001a21]

field-offset candidate

RVA `0x6b1aa0`; range end `0x6b1bb0`; evidence [replanted-0x6001a21.asm](replanted-0x6001a21.asm).

Callers:

- `0x6d2ebd` `call`: Reloaded.Gameplay.SeedChooserScreen.<CloseSeedChooser>b__66_0 [0x6001b8d] to `0x6b1aa0` (Reloaded.Gameplay.Plant.GetRefreshTime [0x6001a21]).
- `0x6d6918` `call`: Reloaded.Gameplay.SeedPacket.SetPacketType [0x6001bbf] to `0x6b1aa0` (Reloaded.Gameplay.Plant.GetRefreshTime [0x6001a21]).
- `0x6d6974` `call`: Reloaded.Gameplay.SeedPacket.SetPacketType [0x6001bbf] to `0x6b1aa0` (Reloaded.Gameplay.Plant.GetRefreshTime [0x6001a21]).
- `0x6d6b97` `call`: Reloaded.Gameplay.SeedPacket.Update [0x6001baf] to `0x6b1aa0` (Reloaded.Gameplay.Plant.GetRefreshTime [0x6001a21]).
- `0x6d6c2b` `call`: Reloaded.Gameplay.SeedPacket.Update [0x6001baf] to `0x6b1aa0` (Reloaded.Gameplay.Plant.GetRefreshTime [0x6001a21]).
- `0x6d6e5b` `call`: Reloaded.Gameplay.SeedPacket.WasPlanted [0x6001bbc] to `0x6b1aa0` (Reloaded.Gameplay.Plant.GetRefreshTime [0x6001a21]).
- `0x6d983e` `call`: Reloaded.Gameplay.VersusMode.UpdateGameplay [0x6001b14] to `0x6b1aa0` (Reloaded.Gameplay.Plant.GetRefreshTime [0x6001a21]).
- `0x6d98c7` `call`: Reloaded.Gameplay.VersusMode.UpdateGameplay [0x6001b14] to `0x6b1aa0` (Reloaded.Gameplay.Plant.GetRefreshTime [0x6001a21]).

Targets:

- `0x6b1abb` `call` to `0x6859a0`: Reloaded.Gameplay.Challenge.IsZombieSeedType [0x6001855].
- `0x6b1ad2` `call` to `0x7df9f0`: Reloaded.TreeStateActivities.GameplayActivity.IsVersusMode [0x6000ed8].
- `0x6b1b10` `call` to `0x7dd8a0`: Reloaded.TreeStateActivities.GameplayActivity.GetPlantDefinition [0x6000ec3].
- `0x6b1b1d` `call` to `0x7df9f0`: Reloaded.TreeStateActivities.GameplayActivity.IsVersusMode [0x6000ed8].
- `0x6b1b9d` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.IsFlying [0x6001a24]

lifecycle/target candidate

RVA `0x6b21e0`; range end `0x6b21f0`; evidence [replanted-0x6001a24.asm](replanted-0x6001a24.asm).

Callers:

- `0x656831` `call`: Reloaded.Gameplay.Board.CanPlantAt [0x60016c8] to `0x6b21e0` (Reloaded.Gameplay.Plant.IsFlying [0x6001a24]).
- `0x656c4f` `call`: Reloaded.Gameplay.Board.CanPlantAt [0x60016c8] to `0x6b21e0` (Reloaded.Gameplay.Plant.IsFlying [0x6001a24]).
- `0x656c6a` `call`: Reloaded.Gameplay.Board.CanPlantAt [0x60016c8] to `0x6b21e0` (Reloaded.Gameplay.Plant.IsFlying [0x6001a24]).
- `0x65ae89` `call`: Reloaded.Gameplay.Board.DoPlantingEffects [0x600177b] to `0x6b21e0` (Reloaded.Gameplay.Plant.IsFlying [0x6001a24]).
- `0x65ea70` `call`: Reloaded.Gameplay.Board.GetPlantsOnLawn [0x60016fc] to `0x6b21e0` (Reloaded.Gameplay.Plant.IsFlying [0x6001a24]).
- `0x66913d` `call`: Reloaded.Gameplay.Board.OffsetYForPlanting [0x60017aa] to `0x6b21e0` (Reloaded.Gameplay.Plant.IsFlying [0x6001a24]).
- `0x66c774` `call`: Reloaded.Gameplay.Board.PlantingPixelToGridX [0x600174a] to `0x6b21e0` (Reloaded.Gameplay.Plant.IsFlying [0x6001a24]).
- `0x66c804` `call`: Reloaded.Gameplay.Board.PlantingPixelToGridY [0x600174b] to `0x6b21e0` (Reloaded.Gameplay.Plant.IsFlying [0x6001a24]).
- `0x67174c` `call`: Reloaded.Gameplay.Board.SpecialPlantHitTest [0x6001750] to `0x6b21e0` (Reloaded.Gameplay.Plant.IsFlying [0x6001a24]).
- `0x69bafe` `call`: Reloaded.Gameplay.CursorObject.UpdateCursor [0x60018df] to `0x6b21e0` (Reloaded.Gameplay.Plant.IsFlying [0x6001a24]).

Targets:

- No direct external targets found.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.FindSquashTarget [0x6001a2c]

lifecycle/target candidate

RVA `0x6af3d0`; range end `0x6af750`; evidence [replanted-0x6001a2c.asm](replanted-0x6001a2c.asm).

Callers:

- `0x6bd7f2` `call`: Reloaded.Gameplay.Plant.UpdateSquash [0x6001a2d] to `0x6af3d0` (Reloaded.Gameplay.Plant.FindSquashTarget [0x6001a2c]).
- `0x6bd926` `call`: Reloaded.Gameplay.Plant.UpdateSquash [0x6001a2d] to `0x6af3d0` (Reloaded.Gameplay.Plant.FindSquashTarget [0x6001a2c]).

Targets:

- `0x6af3f4` `call` to `0x4feb70`: unresolved native address.
- `0x6af400` `call` to `0x4feb70`: unresolved native address.
- `0x6af40c` `call` to `0x4feb70`: unresolved native address.
- `0x6af450` `call` to `0x6b12f0`: Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29].
- `0x6af466` `call` to `0x6b1660`: Reloaded.Gameplay.Plant.GetPlantAttackRect [0x6001a2b].
- `0x6af50a` `call` to `0x2449330`: unresolved native address.
- `0x6af54d` `call` to `0x6f3830`: Reloaded.Gameplay.Zombie.IsTangleKelpTarget [0x6001d95].
- `0x6af563` `call` to `0x6f12f0`: Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7].
- `0x6af57b` `call` to `0x6f2d70`: Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0].
- `0x6af605` `call` to `0x6f35c0`: Reloaded.Gameplay.Zombie.IsBobsledTeamWithSled [0x6001d4c].
- `0x6af631` `call` to `0x4feeb0`: unresolved native address.
- `0x6af643` `call` to `0x6cbaa0`: Reloaded.Gameplay.Common.GetRectOverlap [0x6001bd8].
- `0x6af687` `call` to `0x7df7e0`: Reloaded.TreeStateActivities.GameplayActivity.IsScaryPotterLevel [0x6000edc].
- `0x6af6a0` `call` to `0x6f3a20`: Reloaded.Gameplay.Zombie.IsWalkingBackwards [0x6001d00].
- `0x6af6c9` `call` to `0x676eb0`: Reloaded.Gameplay.Board.ZombieGetID [0x6001713].
- `0x6af740` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.UpdateSquash [0x6001a2d]

field-offset candidate

RVA `0x6bd300`; range end `0x6bda80`; evidence [replanted-0x6001a2d.asm](replanted-0x6001a2d.asm).

Callers:

- `0x6b7a5d` `call`: Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26] to `0x6bd300` (Reloaded.Gameplay.Plant.UpdateSquash [0x6001a2d]).

Targets:

- `0x6bd319` `call` to `0x4feb70`: unresolved native address.
- `0x6bd325` `call` to `0x4feb70`: unresolved native address.
- `0x6bd331` `call` to `0x4feb70`: unresolved native address.
- `0x6bd33d` `call` to `0x4feb70`: unresolved native address.
- `0x6bd349` `call` to `0x4feb70`: unresolved native address.
- `0x6bd355` `call` to `0x4feb70`: unresolved native address.
- `0x6bd3d1` `call` to `0x66bd30`: Reloaded.Gameplay.Board.PixelToGridXKeepOnBoard [0x60016ef].
- `0x6bd3f5` `call` to `0x65f630`: Reloaded.Gameplay.Board.GridToPixelY [0x60016ee].
- `0x6bd42a` `call` to `0x6acd40`: Reloaded.Gameplay.Plant.Die [0x6001a18].
- `0x6bd475` `call` to `0x4feeb0`: unresolved native address.
- `0x6bd49b` `call` to `0x6cc120`: Reloaded.Gameplay.Common.TodAnimateCurve [0x6001be4].
- `0x6bd4ba` `call` to `0x6ae200`: Reloaded.Gameplay.Plant.DoSquashDamage [0x6001a2f].
- `0x6bd4e1` `call` to `0x662110`: Reloaded.Gameplay.Board.IsPoolSquare [0x6001725].
- `0x6bd527` `call` to `0x6709c0`: Reloaded.Gameplay.Board.ShakeBoard [0x600171a].
- `0x6bd558` `call` to `0x2040`: unresolved native address.
- `0x6bd574` `call` to `0x6719c0`: Reloaded.Gameplay.Board.StageHasRoof [0x6001767].
- `0x6bd5bc` `call` to `0x7d9b50`: Reloaded.TreeStateActivities.GameplayActivity.AddTodParticle [0x6000f16].
- `0x6bd601` `call` to `0x7d9b50`: Reloaded.TreeStateActivities.GameplayActivity.AddTodParticle [0x6000f16].
- `0x6bd632` `call` to `0x2040`: unresolved native address.
- `0x6bd663` `call` to `0x2040`: unresolved native address.
- Another 27 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]

lifecycle/target candidate

RVA `0x6b3380`; range end `0x6b33c0`; evidence [replanted-0x6001a2e.asm](replanted-0x6001a2e.asm).

Callers:

- `0x65d7f7` `call`: Reloaded.Gameplay.Board.FindUmbrellaPlant [0x600174c] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x65de9c` `call`: Reloaded.Gameplay.Board.GetFlowerPotAt [0x6001730] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x65ea63` `call`: Reloaded.Gameplay.Board.GetPlantsOnLawn [0x60016fc] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x65ec2c` `call`: Reloaded.Gameplay.Board.GetPumpkinAt [0x600172f] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x673263` `call`: Reloaded.Gameplay.Board.UpdateFog [0x600173d] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x6c2e35` `call`: Reloaded.Gameplay.GridItem.UpdateScaryPot [0x60019cd] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x6c30b5` `call`: Reloaded.Gameplay.GridItem.Update [0x60019c8] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x6c8b75` `call`: Reloaded.Gameplay.Projectile.PeaAboutToHitTorchwood [0x6001aa6] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x6e70dd` `call`: Reloaded.Gameplay.Zombie.BossCanStompRow [0x6001d6f] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x6e89ce` `call`: Reloaded.Gameplay.Zombie.BossStompAttack [0x6001d6e] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x6e94e9` `call`: Reloaded.Gameplay.Zombie.BungeeStealTarget [0x6001d08] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x6e975c` `call`: Reloaded.Gameplay.Zombie.CanTargetPlant [0x6001d2c] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x6f1b46` `call`: Reloaded.Gameplay.Zombie.FindCatapultTarget [0x6001d2e] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x6fbe57` `call`: Reloaded.Gameplay.Zombie.UpdateActions [0x6001d7c] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x700663` `call`: Reloaded.Gameplay.Zombie.UpdateZombieBungee [0x6001cd5] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).
- `0x7032f3` `call`: Reloaded.Gameplay.Zombie.UpdateZombieJalapenoHead [0x6001d8d] to `0x6b3380` (Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e]).

Targets:

- No direct external targets found.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.FindStarFruitTarget [0x6001a3e]

lifecycle/target candidate

RVA `0x6af750`; range end `0x6afb20`; evidence [replanted-0x6001a3e.asm](replanted-0x6001a3e.asm).

Callers:

- `0x6b261a` `call`: Reloaded.Gameplay.Plant.LaunchStarFruit [0x6001a3d] to `0x6af750` (Reloaded.Gameplay.Plant.FindStarFruitTarget [0x6001a3e]).
- `0x6bc5cc` `call`: Reloaded.Gameplay.Plant.UpdateShooter [0x6001a1a] to `0x6af750` (Reloaded.Gameplay.Plant.FindStarFruitTarget [0x6001a3e]).

Targets:

- `0x6af76c` `call` to `0x4feb70`: unresolved native address.
- `0x6af778` `call` to `0x4feb70`: unresolved native address.
- `0x6af784` `call` to `0x4feb70`: unresolved native address.
- `0x6af814` `call` to `0x6b12f0`: Reloaded.Gameplay.Plant.GetDamageRangeFlags [0x6001a29].
- `0x6af8b9` `call` to `0x2449330`: unresolved native address.
- `0x6af8d5` `call` to `0x6f2d70`: Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0].
- `0x6af8ea` `call` to `0x6f12f0`: Reloaded.Gameplay.Zombie.EffectedByDamage [0x6001cd7].
- `0x6af937` `call` to `0x4feeb0`: unresolved native address.
- `0x6af971` `call` to `0x6cb840`: Reloaded.Gameplay.Common.Distance2D [0x6001bcd].
- `0x6af987` `call` to `0x70a110`: Reloaded.Gameplay.Zombie.ZombieTargetLeadX [0x6001d43].
- `0x6af9ee` `call` to `0x6ab010`: range candidate: Reloaded.Gameplay.Board/ZombiePicker..ctor [0x60017d3] + 0x2a0.
- `0x6afa06` `call` to `0x4feeb0`: unresolved native address.
- `0x6afa10` `call` to `0x6cbd30`: Reloaded.Gameplay.Common.RadToDeg [0x6001bd7].
- `0x6afb17` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.UpdateBlink [0x6001a41]

field-offset candidate

RVA `0x6b7ac0`; range end `0x6b7ca0`; evidence [replanted-0x6001a41.asm](replanted-0x6001a41.asm).

Callers:

- `0x6abe06` `call`: Reloaded.Gameplay.Plant.Animate [0x6001a12] to `0x6b7ac0` (Reloaded.Gameplay.Plant.UpdateBlink [0x6001a41]).

Targets:

- `0x6b7ad9` `call` to `0x4feb70`: unresolved native address.
- `0x6b7ae5` `call` to `0x4feb70`: unresolved native address.
- `0x6b7b0e` `call` to `0x7c7a30`: Source.Controllers.ReloadedCharacterController.GetLoopCount [0x6000c4c].
- `0x6b7b29` `call` to `0x7c7650`: Source.Controllers.ReloadedCharacterController.DisableBlink [0x6000c33].
- `0x6b7b59` `call` to `0x4feeb0`: unresolved native address.
- `0x6b7b66` `call` to `0x3832900`: unresolved native address.
- `0x6b7ba5` `call` to `0x63cd70`: GRPC.NET.PushPullStream.get_Position [0x6000857] / Source.Controllers.ReloadedCharacterController.GetFrameCount [0x6000c3b] / Source.Controllers.ReloadedCharacterController.FindTrackIndex [0x6000c5e] / 45 more aliases (JSON aliases[6540656]).
- `0x6b7bcc` `call` to `0x7c7e60`: Source.Controllers.ReloadedCharacterController.IsAnimationPlaying [0x6000c59].
- `0x6b7c15` `call` to `0x7d96d0`: Reloaded.TreeStateActivities.GameplayActivity.AddReanimation [0x6000f2c].
- `0x6b7c29` `call` to `0x7e13a0`: Reloaded.TreeStateActivities.GameplayActivity.ReanimationGetID [0x6000f2f].
- `0x6b7c47` `call` to `0x63cd70`: GRPC.NET.PushPullStream.get_Position [0x6000857] / Source.Controllers.ReloadedCharacterController.GetFrameCount [0x6000c3b] / Source.Controllers.ReloadedCharacterController.FindTrackIndex [0x6000c5e] / 45 more aliases (JSON aliases[6540656]).
- `0x6b7c55` `call` to `0x6b4e30`: Reloaded.Gameplay.Plant.RemoveEffects [0x6001a62].
- `0x6b7c5c` `call` to `0x63cd70`: GRPC.NET.PushPullStream.get_Position [0x6000857] / Source.Controllers.ReloadedCharacterController.GetFrameCount [0x6000c3b] / Source.Controllers.ReloadedCharacterController.FindTrackIndex [0x6000c5e] / 45 more aliases (JSON aliases[6540656]).
- `0x6b7c88` `jmp` to `0x6ad260`: Reloaded.Gameplay.Plant.DoBlink [0x6001a40].
- `0x6b7c8d` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.UpdateTanglekelp [0x6001a59]

field-offset candidate

RVA `0x6bdc80`; range end `0x6bdf60`; evidence [replanted-0x6001a59.asm](replanted-0x6001a59.asm).

Callers:

- `0x6b6437` `call`: Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26] to `0x6bdc80` (Reloaded.Gameplay.Plant.UpdateTanglekelp [0x6001a59]).

Targets:

- `0x6bdc99` `call` to `0x4feb70`: unresolved native address.
- `0x6bdca5` `call` to `0x4feb70`: unresolved native address.
- `0x6bdce7` `call` to `0x677050`: Reloaded.Gameplay.Board.ZombieTryToGet [0x6001715].
- `0x6bdcf9` `call` to `0x6ec590`: Reloaded.Gameplay.Zombie.DragUnder [0x6001d3d].
- `0x6bdd06` `call` to `0x6f5850`: Reloaded.Gameplay.Zombie.PoolSplash [0x6001cfa].
- `0x6bdd29` `call` to `0x665070`: Reloaded.Gameplay.Board.MakeRenderOrder [0x6001737].
- `0x6bdd69` `call` to `0x7d96d0`: Reloaded.TreeStateActivities.GameplayActivity.AddReanimation [0x6000f2c].
- `0x6bdd88` `call` to `0x6ca790`: Reloaded.Gameplay.Reanimation.OverrideScale [0x6001ac3].
- `0x6bddc4` `call` to `0x7d9b50`: Reloaded.TreeStateActivities.GameplayActivity.AddTodParticle [0x6000f16].
- `0x6bddf5` `call` to `0x2040`: unresolved native address.
- `0x6bde08` `call` to `0x6acd40`: Reloaded.Gameplay.Plant.Die [0x6001a18].
- `0x6bde23` `call` to `0x677050`: Reloaded.Gameplay.Board.ZombieTryToGet [0x6001715].
- `0x6bde32` `call` to `0x6ec320`: Reloaded.Gameplay.Zombie.DieWithLoot [0x6001ccc].
- `0x6bde53` `call` to `0x6b0040`: Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17].
- `0x6bde8c` `call` to `0x2040`: unresolved native address.
- `0x6bdead` `call` to `0x6f5850`: Reloaded.Gameplay.Zombie.PoolSplash [0x6001cfa].
- `0x6bdec4` `call` to `0x7c8d30`: Source.Controllers.ReloadedController.ApplyDefaultMaterialToOverrides [0x6000c96].
- `0x6bdeef` `call` to `0x7d96d0`: Reloaded.TreeStateActivities.GameplayActivity.AddReanimation [0x6000f2c].
- `0x6bdf01` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6bdf29` `call` to `0x7c7500`: Source.Controllers.ReloadedCharacterController.AttachToBone [0x6000c46].
- Another 2 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.CobCannonFire [0x6001a5f]

field-offset candidate

RVA `0x6ac470`; range end `0x6ac5c0`; evidence [replanted-0x6001a5f.asm](replanted-0x6001a5f.asm).

Callers:

- `0x6653fa` `call`: Reloaded.Gameplay.Board.MouseDownCobcannonFire [0x6001760] to `0x6ac470` (Reloaded.Gameplay.Plant.CobCannonFire [0x6001a5f]).
- `0x667654` `call`: Reloaded.Gameplay.Board.MouseDown [0x60016cb] to `0x6ac470` (Reloaded.Gameplay.Plant.CobCannonFire [0x6001a5f]).

Targets:

- `0x6ac49c` `call` to `0x4feb70`: unresolved native address.
- `0x6ac4d3` `call` to `0x4feb70`: unresolved native address.
- `0x6ac4df` `call` to `0x4feb70`: unresolved native address.
- `0x6ac508` `call` to `0x2a24e50`: unresolved native address.
- `0x6ac52e` `call` to `0x60f880`: SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072]).
- `0x6ac55e` `call` to `0x7c8270`: Source.Controllers.ReloadedCharacterController.PlayAnimationOnTrack [0x6000c2c].
- `0x6ac597` `call` to `0x60f880`: SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072]).
- `0x6ac5b1` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.RemoveEffects [0x6001a62]

lifecycle/target candidate

RVA `0x6b4e30`; range end `0x6b4f50`; evidence [replanted-0x6001a62.asm](replanted-0x6001a62.asm).

Callers:

- `0x6acee6` `call`: Reloaded.Gameplay.Plant.Die [0x6001a18] to `0x6b4e30` (Reloaded.Gameplay.Plant.RemoveEffects [0x6001a62]).
- `0x6b5ddf` `call`: Reloaded.Gameplay.Plant.Squish [0x6001a27] to `0x6b4e30` (Reloaded.Gameplay.Plant.RemoveEffects [0x6001a62]).
- `0x6b7c55` `call`: Reloaded.Gameplay.Plant.UpdateBlink [0x6001a41] to `0x6b4e30` (Reloaded.Gameplay.Plant.RemoveEffects [0x6001a62]).

Targets:

- `0x6b4e4d` `call` to `0x4feb70`: unresolved native address.
- `0x6b4e6d` `call` to `0x4feeb0`: unresolved native address.
- `0x6b4e7a` `call` to `0x3832770`: unresolved native address.
- `0x6b4e9d` `call` to `0x60f880`: SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072]).
- `0x6b4eb8` `call` to `0x7e14a0`: Reloaded.TreeStateActivities.GameplayActivity.RemoveReanimation [0x6000f29].
- `0x6b4ecf` `call` to `0x7e14a0`: Reloaded.TreeStateActivities.GameplayActivity.RemoveReanimation [0x6000f29].
- `0x6b4ee6` `call` to `0x7e14a0`: Reloaded.TreeStateActivities.GameplayActivity.RemoveReanimation [0x6000f29].
- `0x6b4efd` `call` to `0x7e14a0`: Reloaded.TreeStateActivities.GameplayActivity.RemoveReanimation [0x6000f29].
- `0x6b4f14` `call` to `0x7e14a0`: Reloaded.TreeStateActivities.GameplayActivity.RemoveReanimation [0x6000f29].
- `0x6b4f2b` `call` to `0x7e14a0`: Reloaded.TreeStateActivities.GameplayActivity.RemoveReanimation [0x6000f29].
- `0x6b4f3b` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.FindGoldMagnetTarget [0x6001a67]

lifecycle/target candidate

RVA `0x6af170`; range end `0x6af3d0`; evidence [replanted-0x6001a67.asm](replanted-0x6001a67.asm).

Callers:

- `0x6b1d73` `call`: Reloaded.Gameplay.Plant.GoldMagnetFindTargets [0x6001a75] to `0x6af170` (Reloaded.Gameplay.Plant.FindGoldMagnetTarget [0x6001a67]).
- `0x6b94c0` `call`: Reloaded.Gameplay.Plant.UpdateGoldMagnetShroom [0x6001a60] to `0x6af170` (Reloaded.Gameplay.Plant.FindGoldMagnetTarget [0x6001a67]).

Targets:

- `0x6af19a` `call` to `0x4feb70`: unresolved native address.
- `0x6af1a6` `call` to `0x4feb70`: unresolved native address.
- `0x6af1b2` `call` to `0x4feb70`: unresolved native address.
- `0x6af26f` `call` to `0x2449330`: unresolved native address.
- `0x6af300` `call` to `0x4feeb0`: unresolved native address.
- `0x6af33d` `call` to `0x6cb840`: Reloaded.Gameplay.Common.Distance2D [0x6001bcd].
- `0x6af3c4` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.GoldMagnetFindTargets [0x6001a75]

lifecycle/target candidate

RVA `0x6b1c60`; range end `0x6b1e90`; evidence [replanted-0x6001a75.asm](replanted-0x6001a75.asm).

Callers:

- `0x6b95c5` `call`: Reloaded.Gameplay.Plant.UpdateGoldMagnetShroom [0x6001a60] to `0x6b1c60` (Reloaded.Gameplay.Plant.GoldMagnetFindTargets [0x6001a75]).

Targets:

- `0x6b1c8f` `call` to `0x4feb70`: unresolved native address.
- `0x6b1d73` `call` to `0x6af170`: Reloaded.Gameplay.Plant.FindGoldMagnetTarget [0x6001a67].
- `0x6b1dac` `call` to `0x4feeb0`: unresolved native address.
- `0x6b1dbb` `call` to `0x6cbd40`: Reloaded.Gameplay.Common.RandRangeFloat [0x6001bde].
- `0x6b1dcf` `call` to `0x6cbd40`: Reloaded.Gameplay.Common.RandRangeFloat [0x6001bde].
- `0x6b1e04` `call` to `0x6972a0`: Reloaded.Gameplay.Coin.Die [0x60018ae].
- `0x6b1e1a` `call` to `0x6972a0`: Reloaded.Gameplay.Coin.Die [0x60018ae].
- `0x6b1e30` `call` to `0x6972a0`: Reloaded.Gameplay.Coin.Die [0x60018ae].
- `0x6b1e3a` `call` to `0x4fee20`: unresolved native address.
- `0x6b1e47` `call` to `0x4feb90`: unresolved native address.
- `0x6b1e4f` `call` to `0x4fedd0`: unresolved native address.
- `0x6b1e5e` `call` to `0x4feb90`: unresolved native address.
- `0x6b1e6c` `call` to `0x2c223d0`: unresolved native address.
- `0x6b1e78` `call` to `0x4feb90`: unresolved native address.
- `0x6b1e83` `call` to `0x4fede0`: unresolved native address.
- `0x6b1e89` `call` to `0x4fee10`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.Reset [0x6001a78]

lifecycle/target candidate, field-offset candidate

RVA `0x6b4f50`; range end `0x6b50b0`; evidence [replanted-0x6001a78.asm](replanted-0x6001a78.asm).

Callers:

- No direct callers found in the analyzed range.

Targets:

- `0x6b4f5f` `call` to `0x6ce570`: Reloaded.Gameplay.ReloadedObject.Reset [0x6001b32].
- `0x6b4fbd` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6b4fd2` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6b4fe1` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6b4ff0` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6b4fff` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6b500e` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6b501d` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6b502c` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6b5050` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6b508a` `call` to `0x4fdea0`: unresolved native address.
- `0x6b50a9` `jmp` to `0x4fdea0`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Plant.Serialize [0x6001a79]

lifecycle/target candidate, field-offset candidate

RVA `0x6b50b0`; range end `0x6b5700`; evidence [replanted-0x6001a79.asm](replanted-0x6001a79.asm).

Callers:

- No direct callers found in the analyzed range.

Targets:

- `0x6b50d6` `call` to `0x4feb70`: unresolved native address.
- `0x6b50e2` `call` to `0x4feb70`: unresolved native address.
- `0x6b50ee` `call` to `0x4feb70`: unresolved native address.
- `0x6b50fa` `call` to `0x4feb70`: unresolved native address.
- `0x6b5106` `call` to `0x4feb70`: unresolved native address.
- `0x6b513a` `call` to `0x6ce5a0`: Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35].
- `0x6b5148` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b5156` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b5164` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b5172` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b5183` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b5194` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b51a5` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b51b6` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b51c7` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b51d8` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b51e9` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b51fa` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b520b` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6b521c` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- Another 46 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- `0x6b5488`: `call rax`; target unresolved.

## Reloaded.Gameplay.Plant.Deserialize [0x6001a7a]

lifecycle/target candidate, field-offset candidate

RVA `0x6ac5c0`; range end `0x6acd40`; evidence [replanted-0x6001a7a.asm](replanted-0x6001a7a.asm).

Callers:

- No direct callers found in the analyzed range.

Targets:

- `0x6ac5f5` `call` to `0x4feb70`: unresolved native address.
- `0x6ac601` `call` to `0x4feb70`: unresolved native address.
- `0x6ac60d` `call` to `0x4feb70`: unresolved native address.
- `0x6ac619` `call` to `0x4feb70`: unresolved native address.
- `0x6ac625` `call` to `0x4feb70`: unresolved native address.
- `0x6ac63d` `call` to `0x6ce400`: Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36].
- `0x6ac653` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac668` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac677` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac686` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac698` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac6aa` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac6bd` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac6d8` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac6ea` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac6fd` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac718` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac72a` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac73c` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ac74e` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- Another 57 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- `0x6acaa2`: `call rax`; target unresolved.

## Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]

lifecycle/target candidate

RVA `0x6ce700`; range end `0x6ce740`; evidence [replanted-0x6001b2e.asm](replanted-0x6001b2e.asm).

Callers:

- `0x678ac3` `call`: Reloaded.Gameplay.Board..ctor [0x60016a6] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).
- `0x69a961` `jmp`: Reloaded.Gameplay.Coin..ctor [0x60018cb] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).
- `0x69ca0e` `call`: Reloaded.Gameplay.CursorObject..ctor [0x60018dd] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).
- `0x69e16d` `call`: Reloaded.Gameplay.CursorPreview..ctor [0x60018eb] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).
- `0x6be37f` `call`: Reloaded.Gameplay.Plant..ctor [0x6001a0b] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).
- `0x6c0ee9` `call`: Reloaded.Gameplay.GamepadCursor..ctor [0x6001af4] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).
- `0x6c3391` `call`: Reloaded.Gameplay.GridItem..ctor [0x60019be] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).
- `0x6c50a2` `jmp`: Reloaded.Gameplay.LawnMower..ctor [0x60019e9] / Reloaded.Gameplay.Projectile..ctor [0x6001aaa] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).
- `0x6cad9f` `call`: Reloaded.Gameplay.Reanimation..ctor [0x6001ab1] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).
- `0x70a5cd` `call`: Reloaded.Gameplay.Zombie..ctor [0x6001cc0] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).
- `0x790e0d` `call`: Source.Debug.DebugCrazyDave.Awake [0x60009e8] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).
- `0x7f4800` `call`: Reloaded.Services.CrazyDaveService..ctor [0x6001071] to `0x6ce700` (Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e]).

Targets:

- `0x6ce719` `call` to `0x60f880`: SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072]).

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.ReloadedObject.Reset [0x6001b32]

lifecycle/target candidate

RVA `0x6ce570`; range end `0x6ce5a0`; evidence [replanted-0x6001b32.asm](replanted-0x6001b32.asm).

Callers:

- `0x699023` `call`: Reloaded.Gameplay.Coin.Reset [0x60018c8] to `0x6ce570` (Reloaded.Gameplay.ReloadedObject.Reset [0x6001b32]).
- `0x6b4f5f` `call`: Reloaded.Gameplay.Plant.Reset [0x6001a78] to `0x6ce570` (Reloaded.Gameplay.ReloadedObject.Reset [0x6001b32]).
- `0x6c2749` `call`: Reloaded.Gameplay.GridItem.Reset [0x60019d6] to `0x6ce570` (Reloaded.Gameplay.ReloadedObject.Reset [0x6001b32]).
- `0x6c43ef` `call`: Reloaded.Gameplay.LawnMower.Reset [0x60019e6] to `0x6ce570` (Reloaded.Gameplay.ReloadedObject.Reset [0x6001b32]).
- `0x6c928b` `call`: Reloaded.Gameplay.Projectile.Reset [0x6001aa7] to `0x6ce570` (Reloaded.Gameplay.ReloadedObject.Reset [0x6001b32]).
- `0x6ca9cf` `call`: Reloaded.Gameplay.Reanimation.Reset [0x6001ad6] to `0x6ce570` (Reloaded.Gameplay.ReloadedObject.Reset [0x6001b32]).
- `0x6f5d9f` `call`: Reloaded.Gameplay.Zombie.Reset [0x6001da0] to `0x6ce570` (Reloaded.Gameplay.ReloadedObject.Reset [0x6001b32]).

Targets:

- No direct external targets found.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]

lifecycle/target candidate

RVA `0x6ce5a0`; range end `0x6ce700`; evidence [replanted-0x6001b35.asm](replanted-0x6001b35.asm).

Callers:

- `0x699379` `call`: Reloaded.Gameplay.Coin.Serialize [0x60018c9] to `0x6ce5a0` (Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]).
- `0x69ad28` `call`: Reloaded.Gameplay.CursorObject.Serialize [0x60018e8] to `0x6ce5a0` (Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]).
- `0x69cf23` `call`: Reloaded.Gameplay.CursorPreview.Serialize [0x60018ef] to `0x6ce5a0` (Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]).
- `0x6b513a` `call`: Reloaded.Gameplay.Plant.Serialize [0x6001a79] to `0x6ce5a0` (Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]).
- `0x6c28a0` `call`: Reloaded.Gameplay.GridItem.Serialize [0x60019d7] to `0x6ce5a0` (Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]).
- `0x6c4498` `call`: Reloaded.Gameplay.LawnMower.Serialize [0x60019e7] to `0x6ce5a0` (Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]).
- `0x6c9389` `call`: Reloaded.Gameplay.Projectile.Serialize [0x6001aa8] to `0x6ce5a0` (Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]).
- `0x6caaac` `call`: Reloaded.Gameplay.Reanimation.Serialize [0x6001ad7] to `0x6ce5a0` (Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]).
- `0x6cf046` `call`: Reloaded.Gameplay.SeedBank.Serialize [0x6001b4e] to `0x6ce5a0` (Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]).
- `0x6d6583` `call`: Reloaded.Gameplay.SeedPacket.Serialize [0x6001bc0] to `0x6ce5a0` (Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]).
- `0x6f6456` `call`: Reloaded.Gameplay.Zombie.Serialize [0x6001da1] to `0x6ce5a0` (Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35]).

Targets:

- `0x6ce5c0` `call` to `0x4feb70`: unresolved native address.
- `0x6ce5d7` `call` to `0x60dae0`: unresolved native address.
- `0x6ce5e7` `call` to `0x60dae0`: unresolved native address.
- `0x6ce5f5` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6ce603` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6ce612` `call` to `0x63bab0`: range candidate: Source.Serialization.ReloadedBinaryWriter.BoolArray [0x6000988] + 0x180.
- `0x6ce620` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6ce62e` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6ce63d` `call` to `0x63bab0`: range candidate: Source.Serialization.ReloadedBinaryWriter.BoolArray [0x6000988] + 0x180.
- `0x6ce64d` `call` to `0x60dae0`: unresolved native address.
- `0x6ce65d` `call` to `0x60dae0`: unresolved native address.
- `0x6ce66d` `call` to `0x60dae0`: unresolved native address.
- `0x6ce67d` `call` to `0x60dae0`: unresolved native address.
- `0x6ce68d` `call` to `0x60dae0`: unresolved native address.
- `0x6ce69d` `call` to `0x60dae0`: unresolved native address.
- `0x6ce6ab` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6ce6b9` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6ce6c7` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6ce6d7` `call` to `0x60dae0`: unresolved native address.
- `0x6ce6f1` `jmp` to `0x60dae0`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]

lifecycle/target candidate

RVA `0x6ce400`; range end `0x6ce570`; evidence [replanted-0x6001b36.asm](replanted-0x6001b36.asm).

Callers:

- `0x696f03` `call`: Reloaded.Gameplay.Coin.Deserialize [0x60018ca] to `0x6ce400` (Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]).
- `0x69aa72` `call`: Reloaded.Gameplay.CursorObject.Deserialize [0x60018e9] to `0x6ce400` (Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]).
- `0x69cec3` `call`: Reloaded.Gameplay.CursorPreview.Deserialize [0x60018f0] to `0x6ce400` (Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]).
- `0x6ac63d` `call`: Reloaded.Gameplay.Plant.Deserialize [0x6001a7a] to `0x6ce400` (Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]).
- `0x6c14f1` `call`: Reloaded.Gameplay.GridItem.Deserialize [0x60019d8] to `0x6ce400` (Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]).
- `0x6c34d9` `call`: Reloaded.Gameplay.LawnMower.Deserialize [0x60019e8] to `0x6ce400` (Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]).
- `0x6c738d` `call`: Reloaded.Gameplay.Projectile.Deserialize [0x6001aa9] to `0x6ce400` (Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]).
- `0x6ca5ad` `call`: Reloaded.Gameplay.Reanimation.Deserialize [0x6001ad8] to `0x6ce400` (Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]).
- `0x6ceab4` `call`: Reloaded.Gameplay.SeedBank.Deserialize [0x6001b4f] to `0x6ce400` (Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]).
- `0x6d5c67` `call`: Reloaded.Gameplay.SeedPacket.Deserialize [0x6001bc1] to `0x6ce400` (Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]).
- `0x6eaed8` `call`: Reloaded.Gameplay.Zombie.Deserialize [0x6001da2] to `0x6ce400` (Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36]).

Targets:

- `0x6ce420` `call` to `0x4feb70`: unresolved native address.
- `0x6ce436` `call` to `0x60d8c0`: unresolved native address.
- `0x6ce445` `call` to `0x60d8c0`: unresolved native address.
- `0x6ce454` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ce463` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ce472` `call` to `0x63b2a0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x220.
- `0x6ce481` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ce490` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ce49f` `call` to `0x63b2a0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x220.
- `0x6ce4ae` `call` to `0x60d8c0`: unresolved native address.
- `0x6ce4bd` `call` to `0x60d8c0`: unresolved native address.
- `0x6ce4cc` `call` to `0x60d8c0`: unresolved native address.
- `0x6ce4db` `call` to `0x60d8c0`: unresolved native address.
- `0x6ce4ea` `call` to `0x60d8c0`: unresolved native address.
- `0x6ce4f9` `call` to `0x60d8c0`: unresolved native address.
- `0x6ce508` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ce520` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ce536` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6ce545` `call` to `0x60d8c0`: unresolved native address.
- `0x6ce55e` `jmp` to `0x60d8c0`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.ReloadedObject.PostDeserialize [0x6001b37]

lifecycle/target candidate

RVA `0x60f880`; range end `0x60f890`; evidence [replanted-0x6001b37.asm](replanted-0x6001b37.asm).

Callers:

- `0x60e8fd` `call`: Eadp.Nexus.Connect.Grpc.V1.AccessToken.Clone [0x60001d6] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x60efb2` `call`: Eadp.Nexus.Connect.Grpc.V1.AccessToken..ctor [0x60001d5] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x60f022` `jmp`: AudioSequence/DelayedAudioSequenceData..ctor [0x600000d] / ByteWrite..ctor [0x600008b] / UnitySourceGeneratedAssemblyMonoScriptTypes_v1..ctor [0x60000b8] / 184 more aliases (JSON aliases[6352928]) to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x6107e6` `call`: UI.Scripts.AlmanacArchive.Start [0x60000c6] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x6107f9` `call`: UI.Scripts.AlmanacArchive.Start [0x60000c6] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x61671d` `call`: Eadp.Nexus.Connect.Grpc.V1.FieldFilter.Clone [0x60001f2] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x616cf2` `call`: Eadp.Nexus.Connect.Grpc.V1.FieldFilter..ctor [0x60001f1] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x61810d` `call`: Eadp.Nexus.Connect.Grpc.V1.GetAuthForEpicClientId3Request.Clone [0x600020c] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x618f22` `call`: Eadp.Nexus.Connect.Grpc.V1.GetAuthForEpicClientId3Request..ctor [0x600020b] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x61c492` `call`: Eadp.Nexus.Connect.Grpc.V1.GetAuthForMobileXplayClientId3Request..ctor [0x6000239] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x6204aa` `call`: UI.Scripts.LevelSelectScreen.HideCarousel [0x6000105] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x620703` `call`: UI.Scripts.LevelSelectScreen.OnEnterLevelSelect [0x60000fc] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x620716` `call`: UI.Scripts.LevelSelectScreen.OnEnterLevelSelect [0x60000fc] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x620762` `call`: UI.Scripts.LevelSelectScreen.OnEnterLevelSelect [0x60000fc] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x620891` `call`: UI.Scripts.LevelSelectScreen.OnLeaveLevelSelect [0x60000fd] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x6208a4` `call`: UI.Scripts.LevelSelectScreen.OnLeaveLevelSelect [0x60000fd] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x620c7d` `call`: UI.Scripts.LevelSelectScreen.ShowCarouselAfterXSeconds [0x60000fa] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x62114e` `call`: UI.Scripts.LevelSelectScreen.Start [0x60000f9] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x621a36` `call`: PauseGameActivity.ActiveStarted [0x600004d] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- `0x621b86` `call`: PauseGameActivity.InactiveStarted [0x600004e] to `0x60f880` (SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072])).
- Another 723 entries are available in full in native-analysis.json.

Targets:

- No direct external targets found.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie..ctor [0x6001cc0]

lifecycle/target candidate

RVA `0x70a570`; range end `0x70a700`; evidence [replanted-0x6001cc0.asm](replanted-0x6001cc0.asm).

Callers:

- `0x770f3a` `call`: Source.UI.AspectRatioEnforcer.EnforceAspectRatio [0x60008a8] to `0x70a600` (range candidate: Reloaded.Gameplay.Zombie..ctor [0x6001cc0] + 0x90).
- `0x7bbe77` `call`: Source.Binders.AlmanacZombieBinder.SetupZombie [0x6000d32] to `0x70a570` (Reloaded.Gameplay.Zombie..ctor [0x6001cc0]).

Targets:

- `0x70a589` `call` to `0x4feb70`: unresolved native address.
- `0x70a595` `call` to `0x4feb70`: unresolved native address.
- `0x70a5ad` `call` to `0x4fdf20`: unresolved native address.
- `0x70a5c3` `call` to `0x4fdea0`: unresolved native address.
- `0x70a5cd` `call` to `0x6ce700`: Reloaded.Gameplay.ReloadedObject..ctor [0x6001b2e].
- `0x70a5d9` `call` to `0x1137af0`: unresolved native address.
- `0x70a5f4` `jmp` to `0x4fdea0`: unresolved native address.
- `0x70a61c` `call` to `0x4feb70`: unresolved native address.
- `0x70a638` `call` to `0x4feeb0`: unresolved native address.
- `0x70a652` `call` to `0x5d9360`: unresolved native address.
- `0x70a693` `call` to `0x5d9710`: unresolved native address.
- `0x70a6a6` `call` to `0x5d9360`: unresolved native address.
- `0x70a6e7` `call` to `0x5d9c80`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.ZombieInitialize [0x6001cc1]

lifecycle/target candidate

RVA `0x706f70`; range end `0x709ec0`; evidence [replanted-0x6001cc1.asm](replanted-0x6001cc1.asm).

Callers:

- `0x655903` `call`: Reloaded.Gameplay.Board.AddZombieAtCell [0x60016be] to `0x706f70` (Reloaded.Gameplay.Zombie.ZombieInitialize [0x6001cc1]).
- `0x655afa` `call`: Reloaded.Gameplay.Board.AddZombieInRow [0x6001724] to `0x706f70` (Reloaded.Gameplay.Zombie.ZombieInitialize [0x6001cc1]).
- `0x655c5b` `call`: Reloaded.Gameplay.Board.AddZombieInRow [0x6001724] to `0x706f70` (Reloaded.Gameplay.Zombie.ZombieInitialize [0x6001cc1]).
- `0x655c83` `call`: Reloaded.Gameplay.Board.AddZombieInRow [0x6001724] to `0x706f70` (Reloaded.Gameplay.Zombie.ZombieInitialize [0x6001cc1]).
- `0x655cab` `call`: Reloaded.Gameplay.Board.AddZombieInRow [0x6001724] to `0x706f70` (Reloaded.Gameplay.Zombie.ZombieInitialize [0x6001cc1]).
- `0x6560d3` `call`: Reloaded.Gameplay.Board.BungeeDropZombie [0x6001774] to `0x706f70` (Reloaded.Gameplay.Zombie.ZombieInitialize [0x6001cc1]).
- `0x7bbfee` `call`: Source.Binders.AlmanacZombieBinder.SetupZombie [0x6000d32] to `0x706f70` (Reloaded.Gameplay.Zombie.ZombieInitialize [0x6001cc1]).

Targets:

- `0x706f9c` `call` to `0x4feb70`: unresolved native address.
- `0x706fa8` `call` to `0x4feb70`: unresolved native address.
- `0x706fb4` `call` to `0x4feb70`: unresolved native address.
- `0x706fc0` `call` to `0x4feb70`: unresolved native address.
- `0x706fcc` `call` to `0x4feb70`: unresolved native address.
- `0x706fd8` `call` to `0x4feb70`: unresolved native address.
- `0x706fe4` `call` to `0x4feb70`: unresolved native address.
- `0x706ff0` `call` to `0x4feb70`: unresolved native address.
- `0x706ffc` `call` to `0x4feb70`: unresolved native address.
- `0x707008` `call` to `0x4feb70`: unresolved native address.
- `0x707014` `call` to `0x4feb70`: unresolved native address.
- `0x707020` `call` to `0x4feb70`: unresolved native address.
- `0x70702c` `call` to `0x4feb70`: unresolved native address.
- `0x707038` `call` to `0x4feb70`: unresolved native address.
- `0x707044` `call` to `0x4feb70`: unresolved native address.
- `0x707050` `call` to `0x4feb70`: unresolved native address.
- `0x70705c` `call` to `0x4feb70`: unresolved native address.
- `0x707068` `call` to `0x4feb70`: unresolved native address.
- `0x707074` `call` to `0x4feb70`: unresolved native address.
- `0x707080` `call` to `0x4feb70`: unresolved native address.
- Another 287 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- `0x7079fc`: `call r9`; target unresolved.
- `0x707a5b`: `call r9`; target unresolved.
- `0x707b9f`: `call r9`; target unresolved.
- `0x707bfe`: `call r9`; target unresolved.
- `0x707d36`: `call r9`; target unresolved.
- `0x707d95`: `call r9`; target unresolved.
- `0x707f0d`: `call r9`; target unresolved.
- `0x707f6c`: `call r9`; target unresolved.
- `0x7080c0`: `call r9`; target unresolved.
- `0x70811f`: `call r9`; target unresolved.
- `0x708295`: `call r9`; target unresolved.
- `0x7082f4`: `call r9`; target unresolved.
- `0x7088ae`: `call r9`; target unresolved.
- `0x70897c`: `call rax`; target unresolved.
- `0x709e2c`: `call r8`; target unresolved.

## Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]

lifecycle/target candidate

RVA `0x6ebf90`; range end `0x6ec320`; evidence [replanted-0x6001cca.asm](replanted-0x6001cca.asm).

Callers:

- `0x66dc7e` `call`: Reloaded.Gameplay.Board.RemoveAllZombies [0x60016c2] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x66dd21` `call`: Reloaded.Gameplay.Board.RemoveCutsceneZombies [0x60016c4] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x66deed` `call`: Reloaded.Gameplay.Board.RemoveZombiesForRepick [0x60017b6] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6746b6` `call`: Reloaded.Gameplay.Board.UpdateLevelEndSequence [0x6001742] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x67732e` `call`: Reloaded.Gameplay.Board.ZombiesWon [0x6001702] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x677358` `call`: Reloaded.Gameplay.Board.ZombiesWon [0x6001702] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6875a9` `call`: Reloaded.Gameplay.Challenge.PuzzleNextStageClear [0x6001852] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6b6b34` `call`: Reloaded.Gameplay.Plant.UpdateAbilities [0x6001a26] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6b977d` `call`: Reloaded.Gameplay.Plant.UpdateGraveBuster [0x6001a34] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6e5cf4` `call`: Reloaded.Gameplay.Zombie.ApplyBurn [0x6001d33] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6e6b1d` `call`: Reloaded.Gameplay.Zombie.BobsledDie [0x6001d4a] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6e6b68` `call`: Reloaded.Gameplay.Zombie.BobsledDie [0x6001d4a] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6e6dc7` `call`: Reloaded.Gameplay.Zombie.BossBungeeLeave [0x6001d6d] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6e757f` `call`: Reloaded.Gameplay.Zombie.BossDie [0x6001d70] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6e8cf2` `call`: Reloaded.Gameplay.Zombie.BungeeDie [0x6001d55] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6e9a4d` `call`: Reloaded.Gameplay.Zombie.CatapultDeath [0x6001d57] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6e9bbc` `jmp`: Reloaded.Gameplay.Zombie.CheckForBoardEdge [0x6001d7d] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6e9d18` `call`: Reloaded.Gameplay.Zombie.CheckForBoardEdge [0x6001d7d] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6ec0be` `call`: Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- `0x6ec118` `call`: Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca] to `0x6ebf90` (Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca]).
- Another 25 entries are available in full in native-analysis.json.

Targets:

- `0x6ebfad` `call` to `0x4feb70`: unresolved native address.
- `0x6ebfb9` `call` to `0x4feb70`: unresolved native address.
- `0x6ebfca` `call` to `0x6f7dd0`: Reloaded.Gameplay.Zombie.StopZombieSound [0x6001cec].
- `0x6ebfd7` `call` to `0x60f880`: SentryCliConfiguration.Configure [0x6000005] / AudioSourceWrapper.SetPitch [0x600001c] / RandomAnimation_Controller/<DelayedLoopStart>d__11.System.IDisposable.Dispose [0x60000a2] / 345 more aliases (JSON aliases[6355072]).
- `0x6ebff0` `call` to `0x4feeb0`: unresolved native address.
- `0x6ec007` `call` to `0x3832900`: unresolved native address.
- `0x6ec026` `call` to `0x7e14a0`: Reloaded.TreeStateActivities.GameplayActivity.RemoveReanimation [0x6000f29].
- `0x6ec037` `call` to `0x6f9de0`: Reloaded.Gameplay.Zombie.TrySpawnLevelAward [0x6001cea].
- `0x6ec04b` `call` to `0x6f1f20`: Reloaded.Gameplay.Zombie.GetBobsledPosition [0x6001d48].
- `0x6ec07b` `call` to `0x63cd70`: GRPC.NET.PushPullStream.get_Position [0x6000857] / Source.Controllers.ReloadedCharacterController.GetFrameCount [0x6000c3b] / Source.Controllers.ReloadedCharacterController.FindTrackIndex [0x6000c5e] / 45 more aliases (JSON aliases[6540656]).
- `0x6ec09a` `call` to `0x676f10`: Reloaded.Gameplay.Board.ZombieGet [0x6001714].
- `0x6ec0be` `call` to `0x6ebf90`: Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca].
- `0x6ec0fc` `call` to `0x676f10`: Reloaded.Gameplay.Board.ZombieGet [0x6001714].
- `0x6ec118` `call` to `0x6ebf90`: Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca].
- `0x6ec143` `call` to `0x4feb70`: unresolved native address.
- `0x6ec154` `call` to `0x6e8d10`: Reloaded.Gameplay.Zombie.BungeeDropPlant [0x6001d99].
- `0x6ec174` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6ec18e` `call` to `0x2448850`: unresolved native address.
- `0x6ec1b0` `call` to `0x6acd40`: Reloaded.Gameplay.Plant.Die [0x6001a18].
- `0x6ec1cb` `call` to `0x677050`: Reloaded.Gameplay.Board.ZombieTryToGet [0x6001715].
- Another 14 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.DieDeserialize [0x6001ccb]

lifecycle/target candidate

RVA `0x6ebef0`; range end `0x6ebf90`; evidence [replanted-0x6001ccb.asm](replanted-0x6001ccb.asm).

Callers:

- `0x6a039b` `call`: Reloaded.Gameplay.CutScene.ClearUpsellBoard [0x600191d] to `0x6ebef0` (Reloaded.Gameplay.Zombie.DieDeserialize [0x6001ccb]).
- `0x6aa805` `jmp`: Reloaded.Gameplay.Board/<>c.<Deserialize>b__459_2 [0x60017e8] to `0x6ebef0` (Reloaded.Gameplay.Zombie.DieDeserialize [0x6001ccb]).

Targets:

- `0x6ebf0d` `call` to `0x4feb70`: unresolved native address.
- `0x6ebf37` `call` to `0x4feeb0`: unresolved native address.
- `0x6ebf44` `call` to `0x3832900`: unresolved native address.
- `0x6ebf5b` `call` to `0x7c9970`: Source.Controllers.ReloadedController.Die [0x6000c90].
- `0x6ebf74` `call` to `0x4fdea0`: unresolved native address.
- `0x6ebf84` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.PickBungeeZombieTarget [0x6001cdb]

lifecycle/target candidate

RVA `0x6f46e0`; range end `0x6f4b50`; evidence [replanted-0x6001cdb.asm](replanted-0x6001cdb.asm).

Callers:

- `0x6e6f42` `call`: Reloaded.Gameplay.Zombie.BossBungeeSpawn [0x6001d68] to `0x6f46e0` (Reloaded.Gameplay.Zombie.PickBungeeZombieTarget [0x6001cdb]).
- `0x6fd0b2` `call`: Reloaded.Gameplay.Zombie.UpdateBoss [0x6001d62] to `0x6f46e0` (Reloaded.Gameplay.Zombie.PickBungeeZombieTarget [0x6001cdb]).
- `0x709604` `call`: Reloaded.Gameplay.Zombie.ZombieInitialize [0x6001cc1] to `0x6f46e0` (Reloaded.Gameplay.Zombie.PickBungeeZombieTarget [0x6001cdb]).

Targets:

- `0x6f4703` `call` to `0x4feb70`: unresolved native address.
- `0x6f470f` `call` to `0x4feb70`: unresolved native address.
- `0x6f472b` `call` to `0x4feb70`: unresolved native address.
- `0x6f4737` `call` to `0x4feb70`: unresolved native address.
- `0x6f47bb` `call` to `0x2449330`: unresolved native address.
- `0x6f4819` `call` to `0x65f2a0`: Reloaded.Gameplay.Board.GetTopPlantAt [0x60016fb].
- `0x6f4828` `call` to `0x6b3190`: Reloaded.Gameplay.Plant.MakesSun [0x6001a38].
- `0x6f4852` `call` to `0x658b70`: Reloaded.Gameplay.Board.CountSunFlowers [0x60016fd].
- `0x6f486c` `call` to `0x4fdf20`: unresolved native address.
- `0x6f48cc` `call` to `0x65def0`: Reloaded.Gameplay.Board.GetGraveStoneAt [0x6001782].
- `0x6f4940` `call` to `0x65f2a0`: Reloaded.Gameplay.Board.GetTopPlantAt [0x60016fb].
- `0x6f4962` `call` to `0x6b3190`: Reloaded.Gameplay.Plant.MakesSun [0x6001a38].
- `0x6f4997` `call` to `0x656160`: Reloaded.Gameplay.Board.BungeeIsTargetingCell [0x6001749].
- `0x6f4a45` `call` to `0x4feeb0`: unresolved native address.
- `0x6f4a52` `call` to `0x6ccf70`: Reloaded.Gameplay.Common.TodPickFromWeightedGridArray [0x6001bd5].
- `0x6f4a85` `call` to `0x665070`: Reloaded.Gameplay.Board.MakeRenderOrder [0x6001737].
- `0x6f4ab9` `call` to `0x65f2a0`: Reloaded.Gameplay.Board.GetTopPlantAt [0x60016fb].
- `0x6f4af1` `call` to `0x65f590`: Reloaded.Gameplay.Board.GridToPixelX [0x60016ed].
- `0x6f4b0b` `call` to `0x6f29d0`: Reloaded.Gameplay.Zombie.GetPosYBasedOnRow [0x6001cd3].
- `0x6f4b3b` `call` to `0x6ebf90`: Reloaded.Gameplay.Zombie.DieNoLoot [0x6001cca].
- Another 2 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.CountBungeesTargetingSunFlowers [0x6001cdc]

lifecycle/target candidate

RVA `0x6eacf0`; range end `0x6eae30`; evidence [replanted-0x6001cdc.asm](replanted-0x6001cdc.asm).

Callers:

- No direct callers found in the analyzed range.

Targets:

- `0x6ead17` `call` to `0x4feb70`: unresolved native address.
- `0x6ead23` `call` to `0x4feb70`: unresolved native address.
- `0x6ead89` `call` to `0x2449330`: unresolved native address.
- `0x6eaddf` `call` to `0x65f2a0`: Reloaded.Gameplay.Board.GetTopPlantAt [0x60016fb].
- `0x6eadee` `call` to `0x6b3190`: Reloaded.Gameplay.Plant.MakesSun [0x6001a38].
- `0x6eae22` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]

lifecycle/target candidate

RVA `0x6f1bd0`; range end `0x6f1d40`; evidence [replanted-0x6001cdd.asm](replanted-0x6001cdd.asm).

Callers:

- `0x6e4e21` `call`: Reloaded.Gameplay.Zombie.AnimateChewEffect [0x6001d7b] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x6e4f78` `call`: Reloaded.Gameplay.Zombie.AnimateChewSound [0x6001d7a] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x6ea25c` `call`: Reloaded.Gameplay.Zombie.CheckIfPreyCaught [0x6001cc5] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x6fb69e` `call`: Reloaded.Gameplay.Zombie.UpdateActions [0x6001d7c] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x6fb720` `call`: Reloaded.Gameplay.Zombie.UpdateActions [0x6001d7c] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x6feb20` `call`: Reloaded.Gameplay.Zombie.UpdateLadder [0x6001d5c] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x6febab` `call`: Reloaded.Gameplay.Zombie.UpdateLadder [0x6001d5c] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x701b2c` `call`: Reloaded.Gameplay.Zombie.UpdateZombieDolphinRider [0x6001cda] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x701d13` `call`: Reloaded.Gameplay.Zombie.UpdateZombieDolphinRider [0x6001cda] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x70225b` `call`: Reloaded.Gameplay.Zombie.UpdateZombieGargantuar [0x6001d31] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x702614` `call`: Reloaded.Gameplay.Zombie.UpdateZombieGargantuar [0x6001d31] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x703a9b` `call`: Reloaded.Gameplay.Zombie.UpdateZombiePogo [0x6001cfc] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x703b72` `call`: Reloaded.Gameplay.Zombie.UpdateZombiePogo [0x6001cfc] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x703c64` `call`: Reloaded.Gameplay.Zombie.UpdateZombiePolevaulter [0x6001cd9] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).
- `0x703e59` `call`: Reloaded.Gameplay.Zombie.UpdateZombiePolevaulter [0x6001cd9] to `0x6f1bd0` (Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd]).

Targets:

- `0x6f1bf4` `call` to `0x4feb70`: unresolved native address.
- `0x6f1c00` `call` to `0x4feb70`: unresolved native address.
- `0x6f1c0c` `call` to `0x4feb70`: unresolved native address.
- `0x6f1c38` `call` to `0x6f2c90`: Reloaded.Gameplay.Zombie.GetZombieAttackRect [0x6001cf1].
- `0x6f1c99` `call` to `0x2449330`: unresolved native address.
- `0x6f1cbd` `call` to `0x6b1a00`: Reloaded.Gameplay.Plant.GetPlantRect [0x6001a2a].
- `0x6f1cda` `call` to `0x4feeb0`: unresolved native address.
- `0x6f1cec` `call` to `0x6cbaa0`: Reloaded.Gameplay.Common.GetRectOverlap [0x6001bd8].
- `0x6f1d02` `call` to `0x6e9710`: Reloaded.Gameplay.Zombie.CanTargetPlant [0x6001d2c].
- `0x6f1d39` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.GetZombieAttackRect [0x6001cf1]

lifecycle/target candidate

RVA `0x6f2c90`; range end `0x6f2d70`; evidence [replanted-0x6001cf1.asm](replanted-0x6001cf1.asm).

Callers:

- `0x67fdc5` `call`: Reloaded.Gameplay.Challenge.IZombieEatBrain [0x600186a] to `0x6f2c90` (Reloaded.Gameplay.Zombie.GetZombieAttackRect [0x6001cf1]).
- `0x67fefc` `call`: Reloaded.Gameplay.Challenge.IZombieGetBrainTarget [0x600186b] to `0x6f2c90` (Reloaded.Gameplay.Zombie.GetZombieAttackRect [0x6001cf1]).
- `0x6ea358` `call`: Reloaded.Gameplay.Zombie.CheckSquish [0x6001cde] to `0x6f2c90` (Reloaded.Gameplay.Zombie.GetZombieAttackRect [0x6001cf1]).
- `0x6f1c38` `call`: Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd] to `0x6f2c90` (Reloaded.Gameplay.Zombie.GetZombieAttackRect [0x6001cf1]).
- `0x6f1dac` `call`: Reloaded.Gameplay.Zombie.FindZombieTarget [0x6001d36] to `0x6f2c90` (Reloaded.Gameplay.Zombie.GetZombieAttackRect [0x6001cf1]).

Targets:

- `0x6f2cdc` `call` to `0x6f3a20`: Reloaded.Gameplay.Zombie.IsWalkingBackwards [0x6001d00].
- `0x6f2d05` `call` to `0x6f2170`: Reloaded.Gameplay.Zombie.GetDrawPos [0x6001ce5].

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.BungeeStealTarget [0x6001d08]

lifecycle/target candidate

RVA `0x6e9450`; range end `0x6e9560`; evidence [replanted-0x6001d08.asm](replanted-0x6001d08.asm).

Callers:

- No direct callers found in the analyzed range.

Targets:

- `0x6e946d` `call` to `0x4feb70`: unresolved native address.
- `0x6e9479` `call` to `0x4feb70`: unresolved native address.
- `0x6e94b0` `call` to `0x6f55d0`: Reloaded.Gameplay.Zombie.PlayZombieReanim [0x6001d37].
- `0x6e94d7` `call` to `0x65f2a0`: Reloaded.Gameplay.Board.GetTopPlantAt [0x60016fb].
- `0x6e94e9` `call` to `0x6b3380`: Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e].
- `0x6e9514` `call` to `0x24484c0`: unresolved native address.
- `0x6e951d` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6e9540` `call` to `0x665070`: Reloaded.Gameplay.Board.MakeRenderOrder [0x6001737].
- `0x6e9553` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.GetTargetShieldDamageIndex [0x6001d0e]

lifecycle/target candidate

RVA `0x6f2b90`; range end `0x6f2bc0`; evidence [replanted-0x6001d0e.asm](replanted-0x6001d0e.asm).

Callers:

- No direct callers found in the analyzed range.

Targets:

- No direct external targets found.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.IsFlying [0x6001d2a]

lifecycle/target candidate

RVA `0x6f3650`; range end `0x6f3670`; evidence [replanted-0x6001d2a.asm](replanted-0x6001d2a.asm).

Callers:

- `0x6ac039` `call`: Reloaded.Gameplay.Plant.BlowAwayFliers [0x6001a32] / Reloaded.Gameplay.Plant.BlowAwayFliers [0x6001a33] to `0x6f3650` (Reloaded.Gameplay.Zombie.IsFlying [0x6001d2a]).
- `0x6ae180` `call`: Reloaded.Gameplay.Plant.DoSpecial [0x6001a15] to `0x6f3650` (Reloaded.Gameplay.Zombie.IsFlying [0x6001d2a]).
- `0x6b04bd` `call`: Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17] to `0x6f3650` (Reloaded.Gameplay.Zombie.IsFlying [0x6001d2a]).

Targets:

- No direct external targets found.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.CanTargetPlant [0x6001d2c]

lifecycle/target candidate

RVA `0x6e9710`; range end `0x6e99a0`; evidence [replanted-0x6001d2c.asm](replanted-0x6001d2c.asm).

Callers:

- `0x6e9906` `call`: Reloaded.Gameplay.Zombie.CanTargetPlant [0x6001d2c] to `0x6e9710` (Reloaded.Gameplay.Zombie.CanTargetPlant [0x6001d2c]).
- `0x6ea422` `call`: Reloaded.Gameplay.Zombie.CheckSquish [0x6001cde] to `0x6e9710` (Reloaded.Gameplay.Zombie.CanTargetPlant [0x6001d2c]).
- `0x6f1d02` `call`: Reloaded.Gameplay.Zombie.FindPlantTarget [0x6001cdd] to `0x6e9710` (Reloaded.Gameplay.Zombie.CanTargetPlant [0x6001d2c]).

Targets:

- `0x6e973c` `call` to `0x7dfa20`: Reloaded.TreeStateActivities.GameplayActivity.IsWallnutBowlingLevel [0x6000ef0].
- `0x6e975c` `call` to `0x6b3380`: Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e].
- `0x6e9792` `call` to `0x662110`: Reloaded.Gameplay.Board.IsPoolSquare [0x6001725].
- `0x6e97ae` `call` to `0x6b2430`: Reloaded.Gameplay.Plant.IsSpiky [0x6001a6e].
- `0x6e983c` `call` to `0x65e170`: Reloaded.Gameplay.Board.GetLadderAt [0x6001783].
- `0x6e98a7` `call` to `0x65f2a0`: Reloaded.Gameplay.Board.GetTopPlantAt [0x60016fb].
- `0x6e98e3` `call` to `0x65f2a0`: Reloaded.Gameplay.Board.GetTopPlantAt [0x60016fb].
- `0x6e9906` `call` to `0x6e9710`: Reloaded.Gameplay.Zombie.CanTargetPlant [0x6001d2c].
- `0x6e9945` `call` to `0x662110`: Reloaded.Gameplay.Board.IsPoolSquare [0x6001725].
- `0x6e9965` `call` to `0x65de10`: Reloaded.Gameplay.Board.GetFlowerPotAt [0x6001730].
- `0x6e998e` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.FindCatapultTarget [0x6001d2e]

lifecycle/target candidate

RVA `0x6f1a70`; range end `0x6f1bd0`; evidence [replanted-0x6001d2e.asm](replanted-0x6001d2e.asm).

Callers:

- `0x700818` `call`: Reloaded.Gameplay.Zombie.UpdateZombieCatapult [0x6001d2d] to `0x6f1a70` (Reloaded.Gameplay.Zombie.FindCatapultTarget [0x6001d2e]).
- `0x700a0c` `call`: Reloaded.Gameplay.Zombie.UpdateZombieCatapult [0x6001d2d] to `0x6f1a70` (Reloaded.Gameplay.Zombie.FindCatapultTarget [0x6001d2e]).
- `0x700b8e` `call`: Reloaded.Gameplay.Zombie.UpdateZombieCatapult [0x6001d2d] to `0x6f1a70` (Reloaded.Gameplay.Zombie.FindCatapultTarget [0x6001d2e]).

Targets:

- `0x6f1a98` `call` to `0x4feb70`: unresolved native address.
- `0x6f1aa4` `call` to `0x4feb70`: unresolved native address.
- `0x6f1b19` `call` to `0x2449330`: unresolved native address.
- `0x6f1b46` `call` to `0x6b3380`: Reloaded.Gameplay.Plant.NotOnGround [0x6001a2e].
- `0x6f1b54` `call` to `0x6b2430`: Reloaded.Gameplay.Plant.IsSpiky [0x6001a6e].
- `0x6f1b85` `call` to `0x65f2a0`: Reloaded.Gameplay.Board.GetTopPlantAt [0x60016fb].
- `0x6f1bc2` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.FindZombieTarget [0x6001d36]

lifecycle/target candidate

RVA `0x6f1d40`; range end `0x6f1f20`; evidence [replanted-0x6001d36.asm](replanted-0x6001d36.asm).

Callers:

- `0x6ea242` `call`: Reloaded.Gameplay.Zombie.CheckIfPreyCaught [0x6001cc5] to `0x6f1d40` (Reloaded.Gameplay.Zombie.FindZombieTarget [0x6001d36]).

Targets:

- `0x6f1d59` `call` to `0x4feb70`: unresolved native address.
- `0x6f1d65` `call` to `0x4feb70`: unresolved native address.
- `0x6f1d71` `call` to `0x4feb70`: unresolved native address.
- `0x6f1dac` `call` to `0x6f2c90`: Reloaded.Gameplay.Zombie.GetZombieAttackRect [0x6001cf1].
- `0x6f1e09` `call` to `0x2449330`: unresolved native address.
- `0x6f1e9c` `call` to `0x6f2d70`: Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0].
- `0x6f1eb9` `call` to `0x4feeb0`: unresolved native address.
- `0x6f1ecb` `call` to `0x6cbaa0`: Reloaded.Gameplay.Common.GetRectOverlap [0x6001bd8].
- `0x6f1f19` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.ZombieTargetLeadX [0x6001d43]

lifecycle/target candidate

RVA `0x70a110`; range end `0x70a1a0`; evidence [replanted-0x6001d43.asm](replanted-0x6001d43.asm).

Callers:

- `0x6af987` `call`: Reloaded.Gameplay.Plant.FindStarFruitTarget [0x6001a3e] to `0x70a110` (Reloaded.Gameplay.Zombie.ZombieTargetLeadX [0x6001d43]).
- `0x6b0f17` `call`: Reloaded.Gameplay.Plant.Fire [0x6001a16] to `0x70a110` (Reloaded.Gameplay.Zombie.ZombieTargetLeadX [0x6001d43]).
- `0x6bd80a` `call`: Reloaded.Gameplay.Plant.UpdateSquash [0x6001a2d] to `0x70a110` (Reloaded.Gameplay.Zombie.ZombieTargetLeadX [0x6001d43]).
- `0x6bd95e` `call`: Reloaded.Gameplay.Plant.UpdateSquash [0x6001a2d] to `0x70a110` (Reloaded.Gameplay.Zombie.ZombieTargetLeadX [0x6001d43]).
- `0x6c9f89` `call`: Reloaded.Gameplay.Projectile.UpdateNormalMotion [0x6001a9f] to `0x70a110` (Reloaded.Gameplay.Zombie.ZombieTargetLeadX [0x6001d43]).

Targets:

- `0x70a141` `call` to `0x6f3a20`: Reloaded.Gameplay.Zombie.IsWalkingBackwards [0x6001d00].
- `0x70a156` `call` to `0x709ec0`: Reloaded.Gameplay.Zombie.ZombieNotWalking [0x6001d35].
- `0x70a16d` `call` to `0x6f2d70`: Reloaded.Gameplay.Zombie.GetZombieRect [0x6001cf0].

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.RemoveIceTrap [0x6001d46]

lifecycle/target candidate

RVA `0x6f5ce0`; range end `0x6f5d40`; evidence [replanted-0x6001d46.asm](replanted-0x6001d46.asm).

Callers:

- `0x6e5878` `call`: Reloaded.Gameplay.Zombie.ApplyBurn [0x6001d33] to `0x6f5ce0` (Reloaded.Gameplay.Zombie.RemoveIceTrap [0x6001d46]).
- `0x6f421b` `call`: Reloaded.Gameplay.Zombie.MowDown [0x6001d81] to `0x6f5ce0` (Reloaded.Gameplay.Zombie.RemoveIceTrap [0x6001d46]).

Targets:

- `0x6f5d01` `call` to `0x7b3d70`: Source.Controllers.IceTrapController.ClearIceTrap [0x6000b8b].
- `0x6f5d14` `call` to `0x6e6750`: Reloaded.Gameplay.Zombie.BalloonPropellerHatSpin [0x6001d9b].
- `0x6f5d1e` `call` to `0x6fbf10`: Reloaded.Gameplay.Zombie.UpdateAnimSpeed [0x6001d16].
- `0x6f5d2d` `jmp` to `0x6f7c70`: Reloaded.Gameplay.Zombie.StartZombieSound [0x6001ceb].

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.RemoveColdEffects [0x6001d88]

lifecycle/target candidate

RVA `0x6f5c60`; range end `0x6f5ce0`; evidence [replanted-0x6001d88.asm](replanted-0x6001d88.asm).

Callers:

- `0x6ac1c4` `call`: Reloaded.Gameplay.Plant.BurnRow [0x6001a30] to `0x6f5c60` (Reloaded.Gameplay.Zombie.RemoveColdEffects [0x6001d88]).
- `0x6c7817` `call`: Reloaded.Gameplay.Projectile.DoImpact [0x6001a95] to `0x6f5c60` (Reloaded.Gameplay.Zombie.RemoveColdEffects [0x6001d88]).
- `0x6e6c66` `call`: Reloaded.Gameplay.Zombie.BossBungeeAttack [0x6001d6a] to `0x6f5c60` (Reloaded.Gameplay.Zombie.RemoveColdEffects [0x6001d88]).
- `0x6e7da6` `call`: Reloaded.Gameplay.Zombie.BossRVAttack [0x6001d6b] to `0x6f5c60` (Reloaded.Gameplay.Zombie.RemoveColdEffects [0x6001d88]).
- `0x6e8512` `call`: Reloaded.Gameplay.Zombie.BossSpawnAttack [0x6001d69] to `0x6f5c60` (Reloaded.Gameplay.Zombie.RemoveColdEffects [0x6001d88]).
- `0x6e8922` `call`: Reloaded.Gameplay.Zombie.BossStompAttack [0x6001d6e] to `0x6f5c60` (Reloaded.Gameplay.Zombie.RemoveColdEffects [0x6001d88]).
- `0x6fd54c` `call`: Reloaded.Gameplay.Zombie.UpdateBoss [0x6001d62] to `0x6f5c60` (Reloaded.Gameplay.Zombie.RemoveColdEffects [0x6001d88]).

Targets:

- `0x6f5c8a` `call` to `0x7b3d70`: Source.Controllers.IceTrapController.ClearIceTrap [0x6000b8b].
- `0x6f5c9d` `call` to `0x6e6750`: Reloaded.Gameplay.Zombie.BalloonPropellerHatSpin [0x6001d9b].
- `0x6f5ca7` `call` to `0x6fbf10`: Reloaded.Gameplay.Zombie.UpdateAnimSpeed [0x6001d16].
- `0x6f5cb1` `call` to `0x6f7c70`: Reloaded.Gameplay.Zombie.StartZombieSound [0x6001ceb].
- `0x6f5cd3` `jmp` to `0x6fbf10`: Reloaded.Gameplay.Zombie.UpdateAnimSpeed [0x6001d16].

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.IsTanglekelpTarget_Lower [0x6001d93]

lifecycle/target candidate

RVA `0x6f3930`; range end `0x6f3a20`; evidence [replanted-0x6001d93.asm](replanted-0x6001d93.asm).

Callers:

- No direct callers found in the analyzed range.

Targets:

- `0x6f394d` `call` to `0x4feb70`: unresolved native address.
- `0x6f3959` `call` to `0x4feb70`: unresolved native address.
- `0x6f39ad` `call` to `0x2449330`: unresolved native address.
- `0x6f39d5` `call` to `0x676eb0`: Reloaded.Gameplay.Board.ZombieGetID [0x6001713].
- `0x6f3a11` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.IsTangleKelpTarget [0x6001d95]

lifecycle/target candidate

RVA `0x6f3830`; range end `0x6f3930`; evidence [replanted-0x6001d95.asm](replanted-0x6001d95.asm).

Callers:

- `0x6af54d` `call`: Reloaded.Gameplay.Plant.FindSquashTarget [0x6001a2c] to `0x6f3830` (Reloaded.Gameplay.Zombie.IsTangleKelpTarget [0x6001d95]).
- `0x6b01d8` `call`: Reloaded.Gameplay.Plant.FindTargetZombie [0x6001a17] to `0x6f3830` (Reloaded.Gameplay.Zombie.IsTangleKelpTarget [0x6001d95]).
- `0x6c4ccf` `call`: Reloaded.Gameplay.LawnMower.Update [0x60019de] to `0x6f3830` (Reloaded.Gameplay.Zombie.IsTangleKelpTarget [0x6001d95]).
- `0x6e5fc8` `call`: Reloaded.Gameplay.Zombie.ApplyButter [0x6001d42] to `0x6f3830` (Reloaded.Gameplay.Zombie.IsTangleKelpTarget [0x6001d95]).
- `0x6ea1de` `call`: Reloaded.Gameplay.Zombie.CheckIfPreyCaught [0x6001cc5] to `0x6f3830` (Reloaded.Gameplay.Zombie.IsTangleKelpTarget [0x6001d95]).
- `0x7017d2` `call`: Reloaded.Gameplay.Zombie.UpdateZombieDolphinRider [0x6001cda] to `0x6f3830` (Reloaded.Gameplay.Zombie.IsTangleKelpTarget [0x6001d95]).

Targets:

- `0x6f3849` `call` to `0x4feb70`: unresolved native address.
- `0x6f3855` `call` to `0x4feb70`: unresolved native address.
- `0x6f38bd` `call` to `0x2449330`: unresolved native address.
- `0x6f38e2` `call` to `0x676eb0`: Reloaded.Gameplay.Board.ZombieGetID [0x6001713].
- `0x6f391e` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.RemoveButter [0x6001d9a]

lifecycle/target candidate

RVA `0x6f5af0`; range end `0x6f5c60`; evidence [replanted-0x6001d9a.asm](replanted-0x6001d9a.asm).

Callers:

- `0x7b1da8` `call`: Source.Controllers.GamepadCursorController._updateTertiaryAction [0x6000b5f] to `0x6f5af0` (Reloaded.Gameplay.Zombie.RemoveButter [0x6001d9a]).

Targets:

- `0x6f5b09` `call` to `0x4feb70`: unresolved native address.
- `0x6f5b2d` `call` to `0x6e6750`: Reloaded.Gameplay.Zombie.BalloonPropellerHatSpin [0x6001d9b].
- `0x6f5b75` `call` to `0x7e1430`: Reloaded.TreeStateActivities.GameplayActivity.ReanimationTryToGet [0x6000f30].
- `0x6f5bac` `call` to `0x7c7e10`: Source.Controllers.ReloadedCharacterController.IsAnimationPlayingOnTrack [0x6000c5a].
- `0x6f5be5` `call` to `0x7c7e10`: Source.Controllers.ReloadedCharacterController.IsAnimationPlayingOnTrack [0x6000c5a].
- `0x6f5c06` `call` to `0x6caee0`: Reloaded.Gameplay.Reanimation.set_mAnimRate [0x6001aae].
- `0x6f5c1e` `call` to `0x6ca910`: Reloaded.Gameplay.Reanimation.ReanimationDie [0x6001ab5].
- `0x6f5c37` `call` to `0x4fdea0`: unresolved native address.
- `0x6f5c41` `call` to `0x6fbf10`: Reloaded.Gameplay.Zombie.UpdateAnimSpeed [0x6001d16].
- `0x6f5c55` `jmp` to `0x6f7c70`: Reloaded.Gameplay.Zombie.StartZombieSound [0x6001ceb].
- `0x6f5c5a` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.BungeeLiftTarget [0x6001d9e]

lifecycle/target candidate

RVA `0x6e9240`; range end `0x6e9450`; evidence [replanted-0x6001d9e.asm](replanted-0x6001d9e.asm).

Callers:

- `0x7005b9` `call`: Reloaded.Gameplay.Zombie.UpdateZombieBungee [0x6001cd5] to `0x6e9240` (Reloaded.Gameplay.Zombie.BungeeLiftTarget [0x6001d9e]).

Targets:

- `0x6e9259` `call` to `0x4feb70`: unresolved native address.
- `0x6e9265` `call` to `0x4feb70`: unresolved native address.
- `0x6e9271` `call` to `0x4feb70`: unresolved native address.
- `0x6e927d` `call` to `0x4feb70`: unresolved native address.
- `0x6e92bd` `call` to `0x6f55d0`: Reloaded.Gameplay.Zombie.PlayZombieReanim [0x6001d37].
- `0x6e92db` `call` to `0x6be7d0`: Reloaded.Gameplay.DataID.op_Implicit [0x6001935] / Reloaded.Gameplay.DataID.op_Implicit [0x6001936] / Reloaded.Gameplay.DataID.op_Implicit [0x6001937] / 46 more aliases (JSON aliases[7071696]).
- `0x6e92f5` `call` to `0x2448850`: unresolved native address.
- `0x6e933c` `call` to `0x2040`: unresolved native address.
- `0x6e9358` `call` to `0x4feeb0`: unresolved native address.
- `0x6e9365` `call` to `0x3832900`: unresolved native address.
- `0x6e9388` `call` to `0x7c8740`: Source.Controllers.ReloadedCharacterController.SetAnimationRateOnTrack [0x6000c31].
- `0x6e93b9` `call` to `0x65f2a0`: Reloaded.Gameplay.Board.GetTopPlantAt [0x60016fb].
- `0x6e93f7` `call` to `0x668e90`: Reloaded.Gameplay.Board.NewPlant [0x600177a].
- `0x6e9407` `call` to `0x7df5d0`: Reloaded.TreeStateActivities.GameplayActivity.IsIZombieLevel [0x6000edf].
- `0x6e942b` `call` to `0x683a60`: Reloaded.Gameplay.Challenge.IZombiePlantDropRemainingSun [0x6001871].
- `0x6e9440` `call` to `0x4fee20`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.Reset [0x6001da0]

lifecycle/target candidate

RVA `0x6f5d80`; range end `0x6f5fc0`; evidence [replanted-0x6001da0.asm](replanted-0x6001da0.asm).

Callers:

- No direct callers found in the analyzed range.

Targets:

- `0x6f5d9f` `call` to `0x6ce570`: Reloaded.Gameplay.ReloadedObject.Reset [0x6001b32].
- `0x6f5e9e` `call` to `0x63cd70`: GRPC.NET.PushPullStream.get_Position [0x6000857] / Source.Controllers.ReloadedCharacterController.GetFrameCount [0x6000c3b] / Source.Controllers.ReloadedCharacterController.FindTrackIndex [0x6000c5e] / 45 more aliases (JSON aliases[6540656]).
- `0x6f5f1d` `call` to `0x4fdea0`: unresolved native address.
- `0x6f5f4d` `call` to `0x4fdea0`: unresolved native address.
- `0x6f5f62` `call` to `0x4fdea0`: unresolved native address.
- `0x6f5fb1` `call` to `0x4fee20`: unresolved native address.
- `0x6f5fb7` `call` to `0x4fee10`: unresolved native address.

Indirect calls and jumps:

- None found in the decoded range.

## Reloaded.Gameplay.Zombie.Serialize [0x6001da1]

lifecycle/target candidate

RVA `0x6f63b0`; range end `0x6f6de0`; evidence [replanted-0x6001da1.asm](replanted-0x6001da1.asm).

Callers:

- No direct callers found in the analyzed range.

Targets:

- `0x6f63ea` `call` to `0x4feb70`: unresolved native address.
- `0x6f63f6` `call` to `0x4feb70`: unresolved native address.
- `0x6f6402` `call` to `0x4feb70`: unresolved native address.
- `0x6f640e` `call` to `0x4feb70`: unresolved native address.
- `0x6f641a` `call` to `0x4feb70`: unresolved native address.
- `0x6f6426` `call` to `0x4feb70`: unresolved native address.
- `0x6f6432` `call` to `0x4feb70`: unresolved native address.
- `0x6f643e` `call` to `0x4feb70`: unresolved native address.
- `0x6f6456` `call` to `0x6ce5a0`: Reloaded.Gameplay.ReloadedObject.Serialize [0x6001b35].
- `0x6f6464` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6f6472` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6f6482` `call` to `0x60dae0`: unresolved native address.
- `0x6f6492` `call` to `0x60dae0`: unresolved native address.
- `0x6f64a3` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6f64b4` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6f64c7` `call` to `0x60dae0`: unresolved native address.
- `0x6f64d8` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6f64e9` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6f64fa` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- `0x6f650b` `call` to `0x63bd40`: range candidate: Source.Serialization.ReloadedBinaryWriter.Int32Array [0x6000981] + 0x180.
- Another 107 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- `0x6f6cff`: `call rax`; target unresolved.

## Reloaded.Gameplay.Zombie.Deserialize [0x6001da2]

lifecycle/target candidate

RVA `0x6eae30`; range end `0x6ebb50`; evidence [replanted-0x6001da2.asm](replanted-0x6001da2.asm).

Callers:

- No direct callers found in the analyzed range.

Targets:

- `0x6eae60` `call` to `0x4feb70`: unresolved native address.
- `0x6eae6c` `call` to `0x4feb70`: unresolved native address.
- `0x6eae78` `call` to `0x4feb70`: unresolved native address.
- `0x6eae84` `call` to `0x4feb70`: unresolved native address.
- `0x6eae90` `call` to `0x4feb70`: unresolved native address.
- `0x6eae9c` `call` to `0x4feb70`: unresolved native address.
- `0x6eaea8` `call` to `0x4feb70`: unresolved native address.
- `0x6eaeb4` `call` to `0x4feb70`: unresolved native address.
- `0x6eaec0` `call` to `0x4feb70`: unresolved native address.
- `0x6eaed8` `call` to `0x6ce400`: Reloaded.Gameplay.ReloadedObject.Deserialize [0x6001b36].
- `0x6eaeee` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6eaf07` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6eaf1c` `call` to `0x60d8c0`: unresolved native address.
- `0x6eaf2b` `call` to `0x60d8c0`: unresolved native address.
- `0x6eaf3d` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6eaf4f` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6eaf61` `call` to `0x60d8c0`: unresolved native address.
- `0x6eaf73` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6eaf85` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- `0x6eaf97` `call` to `0x63b5f0`: range candidate: Source.Serialization.ReloadedBinaryReader.BoolArray [0x60009af] + 0x570.
- Another 139 entries are available in full in native-analysis.json.

Indirect calls and jumps:

- `0x6eb88e`: `call rax`; target unresolved.

## Technical handoff

1. Trace relevant callers and targets to the code that actually writes state. Select additional methods with `--method`.
2. Check targeting rules at the actual branches: lanes, distance, flight state, friend/foe, death, and terrain.
3. For each field, document object type, initialization, writers, Serialize/Deserialize, reset, and pool reuse.
4. Check board changes, DataID, pause, cancellation, death, and cleanup. Document state that is not saved.
5. Record technical findings in a separate port-notes.md; this report is replaced when the command is run again.
