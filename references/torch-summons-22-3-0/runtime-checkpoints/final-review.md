# Two-fusion release review: PASS

Final frozen gameplay candidate: Fusion22.3.4, SHA2567b42a0e9a509af84549a759bec55ca17f1d83f11865a735403007d8043bacd63. Debug card integration: Debug1.3.13, SHA2566443f8c604ae3b83a48e787fc5b0246a23d38d316919ec1ec9e217a01746eef5. No partial feature commit or fusionen.txt removal was performed during testing.

## Verified behavior

- Both actual native ingredient orders work for Squashwood(Squash+Torchwood) and Torchweed(Spikeweed+Torchwood). Each transaction leaves exactly one correctly marked300-HP result. Live imported model renderers use original prefab40367/49464; helper1155 uses64169, independently bound through correct official metadata constants.
- Real ordinary peas become40-damage fireballs through Squashwood. Real snow peas thaw to20-damage peas and earn charge. Real small puff peas become40-damage small fire peas through Torchweed. Real PeaNut rolling nuts become40-damage fire nuts. All have live renderer and screenshot evidence.
- Rolling motion reaches the existing conversion owner. An already ignited nut crosses a second Torchweed without another ignition/charge; one nut pierces two enemies exactly once each. Existing native Torchwood rolling damage remains20, and existing HotPotato conversion remains40.
- Squashwood has no summon at exactly50 natural conversions and summons1057 at51. Torchweed has no summon at exactly100 and summons1155 at101. Both choose the first legal cell to the right.
- A genuinely full forward row retains Squashwood charge51; freeing a cell permits the next conversion to summon and reset0. Torchweed consumes the blocked101st charge, resets0, then begins at1 after a cell is freed without an immediate retry.
- Both helpers have300HP and cannot be fused; rejected real placement attempts preserve their DataID/identity atomically. RoastedSquash1057 crushes for exactly1800 and its existing IcePot conversion replaces it with ordinary Squash while preserving support. RoastedSpikeweed1155 retains its low native Spikeweed base and source20-per-second attack behavior.
- Fertilizer heals the three identities to300 without changing saved charge/fire/attack counters, including Squashwood50 and Torchweed100 boundaries. It also preserves an already active helper attack phase and its independent jitter clock.
- Torchweed and helper each destroy native Zamboni/Catapult through native vehicle death presentation, consume the plant, and finish same-event neighboring enemy damage40/20 respectively. Tests use clean separate grass rows. Native destruction states and screenshots were recorded.
- Targeted timing review measured30 simulated seconds at1× and2×: Torchweed39.93/39.79DPS, helper19.97/19.89DPS, with exact40/20 individual hits. Native Spikeweed's existing legacy cadence remained unchanged. Source clock constants use actual native tick duration; helper jitter and animation event are separately pinned offline.
- Balloon flight armor20 is consumed to0 without body overflow on the first hit. The next pulse deals40 during Popping, then40 during Walking. The OffGround exception is limited to Torchweed versus BalloonPopping.
- Three actual new cards1059/1061/1155 use exact embedded atlas rectangles in both native chooser and bank. All331 original atlas resources are independently pixel-identical; total334 cards. Screenshots visually reviewed.

## Evidence map

- ../task1-two-fusions-source-review/: independent official callers, damage/armor dispatch, fertilizer, native120Hz measurement and source contract notes.
- ../task1-two-fusions-22-3-3/: four actual ingredient orders, model renderers, initial fertilizer and ordinary/snow/small projectile checks. Initial rolling failure retained separately; final rolling fix below supersedes it.
- ../task1-timing-22-3-1/: independent30-second1×/2× damage windows and native control.
- ../task1-balloon-22-3-3/: final flight-armor→popping→walking chain.
- ../task1-rolling-22-3-4/: final real rolling conversion, duplicate-ignition guard, two-target hit history and native/HotPotato regressions.
- ../task1-debug-cards-1-3-13/: exact atlas card use in chooser/bank and screenshots.
- ../task1-two-fusions-22-3-4/: natural50/51 and100/101 boundaries, full-row differences, charged fertilizer and Torchweed/Zamboni proof.
- ../task1-catapult-abgrenzung-22-3-4/: isolated clean Torchweed/Catapult proof.
- ../task1-vehicles-final-22-3-4/: helper/Catapult, helper/Zamboni and active-helper fertilizer.
- ../task1-helpers-final-22-3-4/:300HP, nonfusible atomic rejection,1800 squash crush and existing IcePot conversion.

## Fixture controls and limits

Every source-damage block explicitly sets and re-reads current-boardNormal3. Saved defaults remain unchanged; original current-board difficulty and1× are restored after each block. Fire warms targets, so threshold/timing fixtures control target position with existing zombie.move. Full-row blockers are inedible native Spikeweeds. Prior Zamboni ice roads are not removed by entity cleanup; vehicle cases therefore use separate fresh rows. The premature first Catapult failure classification was corrected after an isolated clean-row PASS; it is a fixture finding, not a fixed product defect.

Testing used protected temporary Debug sessions only. No persistent player-save roundtrip, normal-wave distribution study, or global re-audit of older port clocks is claimed. New saved-counter ownership and identity/pool guards were reviewed statically. Re-export still requires the separately documented local original game inputs; no DLL/raw bundle archive should be committed as evidence.

Independent final offline review ran3084 actual Torch/Rolling/Charge integration checks and the Weed regression suite. Both repository diffs pass git diff --check. Runtime log has no new errors/exceptions. Existing IGNORED native shutdown/foreign-mod findings retain their prior classifications.

## Final cleanup

All test entities removed, projectiles drained,1× and prior current-board difficulty restored. The temporary Debug level was then left regularly so persistent board-local ice roads and particles are also disposed. The game was then shut down regularly and process exit confirmed. All13 installed DLL hashes remained byte-identical across shutdown/finalization. Profiles, saves and saved presets were not changed. Final verified Fusion/Debug DLLs remain installed; original DLL backups are retained locally. Release documentation, source provenance selection, fusionen.txt removal and the single reviewed feature commit remain the orchestrator's integration step.
