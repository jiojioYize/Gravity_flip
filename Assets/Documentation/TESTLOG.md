# Gravity Flip — Test Log

Record playtests here. The final report will reference this file for testing and iteration evidence.

**How to use:** Add a new row after each test session. Link to git commits when a fix is pushed.

**Date column:** Use the calendar day you ran the Play Mode test, not the day a row was later edited in the file.

---

## Template

| Date | Build / commit | What was tested | Issue | Fix | Retest |
|------|----------------|-----------------|-------|-----|--------|
| YYYY-MM-DD | e.g. `abc1234` or "Editor Play" | Brief scenario | What went wrong | What changed | Pass / Fail |

---

## Entries

| Date | Build / commit | What was tested | Issue | Fix | Retest |
|------|----------------|-----------------|-------|-----|--------|
| 2026-05-24 | Editor scripts only | Added `GravityController` and `PlayerController2D`; reviewed setup requirements | Not yet tested in Unity Play Mode because scene and prefab binding are not created | Documented required Inspector setup in `SETUP.md` | Pending Play Mode test |
| 2026-05-24 | Unity Editor Play Mode | Movement, grounded jump, no double jump, gravity flip to ceiling, ceiling movement, inverted jump, and flip back to normal gravity | No issues reported during first manual verification | No code change required | Pass |
| 2026-05-25 | Editor scripts only | Added `ProgressManager`, `GameManager`, `Collectible`, `ExitDoor`, and `KillZone`; reviewed setup requirements | Not yet tested in Unity Play Mode because scene binding is required | Documented required Inspector setup and verification checklist in `SETUP.md` | Pending Play Mode test |
| 2026-05-26 | Unity Editor Play Mode | Locked exit before collect, collectable pickup, level complete after collect, kill-zone respawn, gravity reset, collectable reset | No issues reported during manual verification | No code change required | Pass |
| 2026-05-26 | Unity Editor Play Mode (`2f240ea`) | Level01 layout: collectable unreachable from ground-only path; reachable via ceiling route after gravity flip | No issues reported during manual verification | No code change required | Pass |
| 2026-05-26 | Editor scripts only | Added `GameplayHUD` for progress, gravity direction, and control hints | Not yet tested in Unity Play Mode because canvas binding is required | Documented canvas setup in `SETUP.md` section 5 | Pending Play Mode test |
| 2026-05-26 | Unity Editor Play Mode | HUD progress, gravity label, control hints; updates on collect, flip, and kill-zone respawn | No issues reported during manual verification | No code change required | Pass |
| 2026-05-26 | Editor scripts only | Added `AudioManager`, `FlipScreenFlash`, and `R` level reset; wired SFX hooks to jump, flip, collect, death, door, and win | Not yet tested in Unity Play Mode; audio clips must be assigned in Inspector | Documented setup in `SETUP.md` section 8 | Pending Play Mode test |
| 2026-05-26 | Unity Editor Play Mode | SFX (jump, flip, collect, death, door unlock, level complete, level reset), flip screen flash, and `R` level reset | No issues reported during manual verification | No code change required | Pass |
| 2026-05-26 | Editor scripts only | P1 shuttle platform: `MovingPlatform2D`, `ShuttlePlatformController`, `PlatformCorridorExitTrigger`; player detach on reset | Not yet tested in Unity Play Mode — scene objects and Inspector binding required | Documented setup in `SETUP.md` section 9 | Pending Play Mode test |
| 2026-05-26 | Unity Editor Play Mode (P1) | Collect C1 → shuttle platform should appear | Platform did not appear after C1 | `ShuttlePlatformController` disabled itself via `SetActive(false)` on the same GameObject as the controller; hide now toggles renderers/colliders when controller and platform share one object | Pending retest |
| 2026-05-26 | Unity Editor Play Mode (P1) | Stand on moving shuttle platform after C1 | Player could not stay on platform; platform slid away | Parenting fought custom velocity; added `PlatformRider2D` + platform velocity/delta carry, removed parenting | Pending retest |
| 2026-05-26 | Unity Editor Play Mode (P1) | Move/jump/flip while riding shuttle platform | Player followed platform but could not move, jump, or flip | Velocity + delta carry conflicted with kinematic contact solver; riding mode uses parenting + gravity-only velocity + `MovePosition` for input; jump unparents | Pending retest |
| 2026-05-26 | Unity Editor Play Mode (P1) | Ride shuttle after parenting-only riding fix | Platform moved without carrying player again | Dynamic `Rigidbody2D` does not follow parent `MovePosition`; reverted to delta carry on platform + riding input without parenting or platform velocity on player | Pending retest |
| 2026-05-26 | Unity Editor Play Mode (P1) | Jump and flip gravity while on shuttle platform | Platform carry and strafe worked; jump and Shift flip did not | Riding mode stripped jump velocity each frame; flip kept platform carry/contact; jump before riding, leave-support check, `ReleaseFromPlatform` on jump/flip | Pending retest |
| 2026-05-26 | Unity Editor Play Mode (P1) | Jump/flip on shuttle; console errors | Jump and flip still failed; `NullReferenceException` in `PlatformRider2D.OnCollisionExit2D` | `null == null` entered exit handler after `ReleaseFromPlatform`; guard `activePlatform`; brief detach lock after jump/flip | Pending retest |
| 2026-05-26 | Unity Editor Play Mode (P1) | Jump and Shift flip on moving platform | A/D and carry OK; jump failed; Shift needed two presses and flipped twice | Platform FixedUpdate ran before player (re-carried jump); jump detach moved to Update; platform order 100; interaction block; flip uses `Fire3` only + cooldown | Pending retest |
| 2026-05-26 | Unity Editor Play Mode (P1) | Full shuttle controls after jump/flip fix | Jump and flip OK; A/D on platform stopped working | Riding used `MovePosition` for strafe + required ground cast; riding now uses horizontal velocity when on platform contact; platform carry still runs after player at order 100 | Pending retest |
| 2026-05-26 | Unity Editor Play Mode (P1) | A/D strafe on moving shuttle platform | Jump/flip OK; platform carry OK; A/D still dead on platform | Kinematic contact zeroed tangential velocity; strafe moved to `ApplyPlatformStrafeAfterCarry` right after platform delta; player order 150; `useFullKinematicContacts` | Pending retest |
| 2026-05-26 | Unity Editor Play Mode (P1) | Full shuttle platform loop (carry, strafe, jump, flip, despawn) | All core P1 checks passed | Added `using GravityFlip.Level` compile fix; side-contact carry to be refined | Pass |
| 2026-05-31 | Editor scripts only | Side contact should not stick to moving platform | P1 passed but player was carried when touching platform left/right edges | `MovingPlatformContact` uses collision normal vs gravity; register/carry only on walkable top/bottom | Pending Play Mode retest |
| 2026-05-31 | Unity Editor Play Mode | Walkable-only platform carry | After normal-based filter, platform no longer carried player on top | Contact normal sign differs per callback body; use abs alignment + gravity cast fallback onto platform collider | Pending retest |
| 2026-05-31 | Unity Editor Play Mode (`445cef3`) | Final shuttle platform: carry, strafe, jump, flip, despawn, walkable-only contact | No issues reported — stand on top carries; side bumps do not; inverted-gravity bottom contact OK | Abs normal alignment + gravity cast fallback on `MovingPlatformContact` | Pass |
| 2026-06-01 | Editor scripts only | `CameraFollow2D` on Main Camera for linear Level01 | Script and `SETUP.md` section 10 added; target bound in Inspector | Ready for Play Mode verification | Pass |
| 2026-06-01 | Unity Editor Play Mode | `CameraFollow2D` first version (follow X and Y) | Ceiling/ground framing shifted; scene felt vertically offset | Default to locked Y and relative horizontal follow; keep authored camera height | Pass |
| 2026-06-01 | Unity Editor Play Mode | `CameraFollow2D` locked Y + relative horizontal follow | Horizontal scroll OK; ceiling/ground framing matches authored layout | `lockVerticalPosition`, `useRelativeHorizontalFollow`, offset `(0,0)` on Main Camera | Pass |
| 2026-06-01 | Editor scripts only | P2 `PlatformBoundCollectible` + shuttle run events | C2 hidden until run; reappears each run if missed; top/bottom boarding unchanged from P1 | `Collectable2` child on `ShuttlePlatform` with `BoxCollider2D` per `SETUP.md` section 10 | Pass |
| 2026-06-01 | Unity Editor Play Mode | P2 Collectable 2 on shuttle platform | Hidden before C1; appears each run; collect on top or ceiling-drop boarding; miss then retry next run; reset/`R` clears C2 until C1 again | `PlatformBoundCollectible` + run events; P1 loop unchanged | Pass |
| 2026-06-01 | Editor / docs | P3 Collectable 3 + pit hazard blockout guide | C3 + `KillZone` reuse; SETUP section 15 | Split ground, `Collectable3`, `KillZone_C3Pit` | Pass |
| 2026-06-01 | Unity Editor Play Mode | P3 shuttle dismount, jump over pit, collect C3 | Collect/death SFX unified; forward route toward corridor | Ground gap + lips; P1/P2 unchanged | Pass |

---

## Known issues (open)

| Issue | Workaround until fixed | Planned fix |
|-------|------------------------|-------------|
| Walking past the left/right end of finite `Ground` / `Ceiling` lets the player fall out of the play space | Avoid hugging platform ends during testing | Invisible end-cap colliders after P4 blockout ([SETUP.md](../SETUP.md) section 14) |

---

## Resolved issues (summary)

_Use this section for quick reference when writing the report._

- Initial movement and gravity-flip implementation passed manual Unity Editor verification: left/right movement, grounded-only jump, gravity flip, ceiling movement, inverted jump, and return to normal gravity all worked as intended.
- First level-loop implementation passed manual Unity Editor verification: exit locked until collectable collected, collectable disappears on pickup, level completes at exit, kill zone respawns player at spawn point, gravity resets to normal, and collectable progress resets.
- Level01 reference puzzle layout verified: the collectable cannot be reached without using gravity flip; the ceiling route allows pickup and level completion.
- Gameplay HUD passed manual Unity Editor verification: progress `Keys 0/1` → `1/1`, gravity Down/Up on flip, controls visible, reset after kill zone matches gameplay state.
- Audio and polish feedback passed manual Unity Editor verification: jump, flip, collect, death, door unlock, level complete, and level reset sounds; flip screen flash on gravity change; `R` resets player state without using death SFX path incorrectly.
- P1 shuttle platform passed manual Unity Editor verification: spawns after C1, moves left→right, carries player on walkable surfaces only (not side bumps), A/D strafe, jump, single Shift flip (`Fire3`), corridor despawn and respawn loop, kill/`R` reset behaviour unchanged.
- P2 Collectable 2 on shuttle platform passed manual Unity Editor verification (2026-06-01): C2 hidden until C1 and an active run; pickup on moving platform; retry on next run if missed; progress and reset behaviour correct.
- P3 Collectable 3 and pit hazard passed manual Unity Editor verification (2026-06-01): split ground gap, pit kill zone, jump-from-shuttle route, unified collect/death SFX, progress toward corridor.
