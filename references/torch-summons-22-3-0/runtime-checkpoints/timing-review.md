# Targeted22.3.1 runtime review

Installed candidate SHA256:a02d0e3e317944795b089b205c064fb5498f0fa95a664e1c6ab3b3f01d6513b1. Original installed22.2.6 SHA256:ae4e7293fc0d593205b50167c450f95242ed727164d9c1ce2505bf6cfb98547a. The previous DLL and log are backed up here. Installation followed regular level.leave/game.shutdown and confirmed process exit. No other DLL was replaced.

## Timing issue: PASS

Measured in a temporary Pool Debug session. Explicitly set and re-read current-board difficultyNormal3 before source damage measurements. The saved default remains unchanged. Torchweed's target was repeatedly returned through the existing zombie.move API to cell(4,0), because fire removes its freeze. Every probe records actual target position. Torch targetX stayed355.99–360, within contact. Other targets remained frozen.

| Native speed | Simulated seconds | TorchweedDPS | Helper1155DPS | Native SpikeweedDPS |
|---|---:|---:|---:|---:|
|1×|30.050|39.933|19.967|23.960|
|2×|30.158|39.790|19.895|23.211|

All observed Torchweed health changes were exactly40 and helper changes exactly20. Torchweed delivered30 hits in each window; its mean observed interval was0.9997/1.0011 simulated seconds. Helper also delivered30 hits. Native's2× window excludes one initial native pulse at the sampling boundary; the observed rate remains within two boundary hits of its established native24DPS cadence. Polling is sequential, so individual observed intervals include sampling uncertainty; exact clock/event constants are verified separately offline.

Evidence:runtime.json and summary.json. A first aborted fixture run inherited Extreme5 from the fresh board and therefore halved all incoming damage. fixture-extreme5.json preserves that diagnostic evidence; it was not treated as a product failure. The corrected run explicitly verifiesNormal3.

## Separate balloon follow-up

The requested narrow check preserved the same frozen build and used current-boardNormal3. Flying balloon before first fire:Body1000/FlyingHealth20. First pop:Popping,Body1000/FlyingHealth20. Landing:Walking,Body1000/FlyingHealth20. First ground hit:Body960/FlyingHealth20. Thus no20-point absorption on the walking hit was observed, but the flight armor field is retained after landing. At the next fire pulse while still Popping, IsFlying remains true and LandFlyer is called again without body damage.

The official source BalloonZombie.Fall at0x5bff10 explicitly clears firstArmorHP+0xb0 at0x5bffab. Independent metadata confirms BalloonZombie has no BodyTakeDamage or GetDamage override. Source Fall calls ChangeStatus(0) immediately; inherited BodyTakeDamage at0x64f500 directly subtracts body HP without an animation gate. Therefore skipping the next fire tick during native Popping is also a confirmed port discrepancy. These findings were reported separately to the orchestrator and worker; no silent change was applied to the measured candidate. Evidence:balloon-flying-health.json and the independent source export under ../task1-two-fusions-source-review/.

## Cleanup

All test plants and zombies removed. Board left empty, paused, at1×. Prior current-board difficulty5 restored after each completed check. No profile/save/default setting changed. Runtime log contains no new red errors or exceptions. Known native RenderTexture warnings occurred during the previous process's regular shutdown and remain covered by the existing IGNORED report.

The full two-fusion feature matrix has not been run or approved by this timing review. No feature commit was made.
