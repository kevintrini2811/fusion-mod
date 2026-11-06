# Native Eyes Zombie colours

Status: DONE. Fusion Mod 22.2.3 uses Zombie Library 1.3.1's shared native colour
material for Eyes Zombie bodies and armour. The former approximate cold tint
and sprite-lerp translation are removed. Animated source alpha reaches the
native shader once, including the current head alpha used by armour.

Release build and static checks pass. A live temporary Pool session compares
native Normal, Eyes, Conehead Eyes and Buckethead Eyes on land and in water,
before cold, during native chill/freeze and after full thaw. Texture details,
black outlines, pool clipping and normal restored colours pass visual review.
No profile mutation is requested, speed is restored, and the game exits
regularly.

[Shared native evidence, API snapshots and reviewed screenshots](https://github.com/kevintrini2811/zombie-library/blob/main/references/native-zombie-colors/runtime-validation.md).
