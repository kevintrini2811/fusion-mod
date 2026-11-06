# Balloon issue22.3.3: PASS

Verified candidate SHA256:1124a341a152ebbc7c059b3d6b747c5aba2305a38bf4e672b6949f88017caeb1. Verified the restored22.2.6 DLL hash before replacement and confirmed game process exit. Saved an original DLL backup before installing this exact candidate. Runtime log identifies Plant Fusion22.3.3.

Independent code review confirms OffGround is added only for a Torchweed owner targeting a Balloon in BalloonPopping. The helper and unrelated off-ground targets retain their existing masks. The previously reviewed first-flight-armor overflow scope remains unchanged. Independently executed TorchFusionsTests:3077 checks passed. git diff --check passed.

Live test used a temporary Pool Debug session at1×. Explicitly set and re-read current-board difficultyNormal3 before measuring damage; no saved default was changed. Controlled balloon position through existing zombie.move and recorded actual coordinates on each probe.

Observed native state sequence:

| Phase | FlyingHP | BodyHP | Result |
|---|---:|---:|---|
|Flying|20|1000|Initial flight armor present|
|Popping|0|1000|First40 hit consumes armor without body overflow|
|Popping|0|960|Next40 pulse reaches body during popping|
|Walking|0|920|Following40 pulse continues after landing|

The native phase progresses through popping into walking; no second pop or residual flight armor occurs. This matches the official source Fall behavior: status becomes ground-compatible immediately, primary armor is removed, and following hits may damage the body before the animation ends.

Raw API evidence:runtime.json. Runtime log copied to runtime.log; no new errors or exceptions. The full two-fusion feature matrix has not been started by this issue test, and no partial feature commit was made.

Cleanup: all temporary plants and zombies removed; board left empty, paused,1×. Previous current-board difficulty5 restored. Profiles, saves and saved presets unchanged. The verified22.3.3 candidate remains installed and running for the orchestrator's next authorized review stage.
