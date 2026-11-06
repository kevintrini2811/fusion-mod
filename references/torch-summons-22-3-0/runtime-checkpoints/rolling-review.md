# Rolling-motion issue22.3.4: PASS

Installed and verified frozen Fusion22.3.4 SHA2567b42a0e9a509af84549a759bec55ca17f1d83f11865a735403007d8043bacd63 and Debug1.3.13 SHA2566443f8c604ae3b83a48e787fc5b0246a23d38d316919ec1ec9e217a01746eef5. Original Fusion22.2.6 and Debug1.3.12 DLLs were independently hash-checked and backed up before replacement while no game process was running.

Independent source review confirms one Torchweed conversion call was added to the existing RollingProjectiles.Move owner before its existing ChargeTorch conversion, matching the native patch priorities. No parallel movement sweep was added. Independently executed TorchFusionsTests:3084 checks passed, covering actual Torch/Rolling/Charge code together.

Temporary Pool session at1×; explicitly set and re-read current-boardNormal3 before source damage checks. Real registered PeaNut fired real rolling nuts. Runtime checks passed:

- Torchweed converts a natural ground rolling nut to marker-624013 and live fire graphics; target loses exactly40HP.
- A second Torchweed does not re-ignite or double-charge that already ignited nut; damage stays40 and ignition owner remains the first stump.
- One ignited rolling nut pierces two frozen targets and damages each exactly once for40HP.
- Native Torchwood leaves an ordinary rolling nut at20 damage.
- Existing HotPotato still converts that nut to40 damage through the existing charge path.

Raw API evidence is split into01-real-rolling.json through05-hit-history.json, with complete latest.json and screenshots. Test entities removed, projectiles drained, speed restored to1×, previous current-board difficulty restored, board paused. No profile/default setting changed, no feature commit made. The full task matrix remains gated on orchestrator acceptance of this issue review.
