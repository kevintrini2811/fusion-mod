# Catapult observation: fixture contamination, product path PASS

Initial same-row sequence ran Zamboni before Catapult. Removing vehicles/plants/projectiles did not remove the native Zamboni ice road. The next planted Torchweed disappeared while the Catapult and neighboring target remained unchanged. The screenshot confirms the prior ice road remained across that row. This first observation alone did not establish a CatapultDeath defect.

Repeated only Torchweed+Catapult on previously unused grass row1, at explicitly verified current-boardNormal3 and native1×. The same frozen candidate7b42a0e9a509af84549a759bec55ca17f1d83f11865a735403007d8043bacd63 passed: plant consumed, Catapult reaches native destruction, and the surviving same-event neighbor loses exactly40HP. No source/product fix is required for the contaminated first fixture.

Raw isolated evidence:vehicle-Torchwood-Catapult.json,vehicles-latest.json and vehicle-Torchwood-Catapult.png. Original failed same-row evidence remains under ../task1-two-fusions-22-3-4/vehicle-catapult-failure.json. Remaining vehicle cases must use separate previously unused rows, with Zamboni last. No new API or global ice-road suppression is needed.

The reviewer corrected the premature product-failure classification with the orchestrator and implementing worker. Board again empty, paused,1×; previous current-board difficulty restored. Candidate unchanged, no profile/default changes, no feature commit.
