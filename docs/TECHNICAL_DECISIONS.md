# Gravity Flip — Technical Decisions

This file records important design and programming decisions made during development. It is intended to support the final report by keeping track of what was chosen, why it was chosen, and how the choice affected the game.

Update this document when a decision affects gameplay, architecture, testing, scope, accessibility, assets, or the final player experience.

---

## Decision Log

### 2026-05-24 — Build one polished vertical-slice level

**Decision:** Focus development on one complete level, `Level01`, before adding optional features.

**Reason:** The assessment rewards a coherent, playable vertical slice and steady improvement more than a large unfinished feature list. One polished level gives enough space to demonstrate the gravity-flip mechanic, player feedback, UI, hazards, win condition, testing, and iteration.

**Alternatives considered:** Multiple levels or extra mechanics early in development.

**Result / follow-up:** Baseline functionality must be completed first. Stretch goals such as menu, pause, extra collectables, BGM, or visual polish should only be added after the core level is stable and testable.

---

### 2026-05-24 — Flip player gravity, not the world

**Decision:** Gravity flipping changes the player's gravity direction and visual orientation only. Level geometry remains fixed.

**Reason:** This keeps the puzzle readable, matches the project requirement, and avoids unnecessary complexity from rotating or moving the entire scene. It also keeps collision, camera, UI, and level layout easier to test.

**Alternatives considered:** Rotating the whole world, rotating the camera, or physically moving platforms during a flip.

**Result / follow-up:** `GravityController` should own the current gravity direction, and player movement/jump logic should read from that state. Level objects should not be transformed when gravity changes.

---

### 2026-05-24 — Use Legacy Input Manager for baseline controls

**Decision:** Use Unity's built-in legacy input for the required controls: A/D or arrows for movement, Space for jump, and Left Shift for gravity flip.

**Reason:** The baseline controls are simple, the project timeline is short, and adding Unity's newer Input System would introduce setup and documentation overhead without improving the core assessed mechanic.

**Alternatives considered:** Installing and configuring the Unity Input System package.

**Result / follow-up:** Do not add the Input System package unless the project needs remapping, controller support, or the user explicitly requests it.

---

### 2026-05-24 — Keep process evidence in lightweight documents

**Decision:** Use `README.md`, `docs/GAME_CONCEPT.md`, `SETUP.md`, `Assets/Documentation/TESTLOG.md`, and this file as the main project evidence trail.

**Reason:** The assessment expects GitHub to show planning, development, testing, debugging, and improvement over time. Lightweight Markdown files are easy to update as features are implemented and provide clear material for the final report and presentation.

**Alternatives considered:** GitHub Issues/Projects or a more complex project-management setup.

**Result / follow-up:** Update `TESTLOG.md` for test sessions and bug fixes. Update this file for design and technical choices. Keep README and SETUP accurate as the project changes.

---

### 2026-05-24 — Apply custom gravity to the player Rigidbody

**Decision:** Disable the player's built-in `Rigidbody2D.gravityScale` and apply gravity manually in `PlayerController2D` using the direction stored by `GravityController`.

**Reason:** Unity's `Physics2D.gravity` is global. Changing it would affect every dynamic `Rigidbody2D` in the scene, but the design requires only the player gravity direction and player visual orientation to change. Manual player gravity keeps the world stable and makes the rule clear in code.

**Alternatives considered:** Flipping global `Physics2D.gravity`, using negative `gravityScale`, or rotating the whole scene.

**Result / follow-up:** The first implementation supports down/up gravity, screen-space left/right movement, grounded-only jumping, and visual inversion. Initial Unity Editor Play Mode verification passed for movement, grounded jump, no double jump, gravity flip to the ceiling, ceiling movement, inverted jump, and flipping back to normal gravity.

---

### 2026-05-25 — Use simple scene managers for the first level loop

**Decision:** Add `ProgressManager`, `GameManager`, `Collectible`, `ExitDoor`, and `KillZone` as small scene-level scripts instead of introducing a larger architecture or event framework.

**Reason:** The current goal is a reliable vertical-slice loop: collect the required item, unlock the exit, complete the level, and reset cleanly after death. Small scene managers are easy to wire in Unity, easy to explain in the report, and appropriate for a one-level coursework project.

**Alternatives considered:** A more generic objective system, ScriptableObject-driven level data, or a larger event bus.

**Result / follow-up:** Initial Unity Editor Play Mode verification passed: exit stays locked until the collectable is collected, collectable pickup unlocks completion, touching the exit after collection logs level complete, kill-zone respawn returns the player to the spawn point, gravity resets to normal, and collectable state resets on death.

---

### 2026-05-26 — Use legacy UI Text for the gameplay HUD

**Decision:** Implement `GameplayHUD` with Unity's built-in `UnityEngine.UI.Text` instead of TextMeshPro.

**Reason:** The baseline HUD only needs three simple text fields (progress, gravity direction, and control hints). Legacy UI Text avoids adding another package, keeps setup simple in the Editor, and is sufficient for the coursework vertical slice.

**Alternatives considered:** TextMeshPro for sharper text rendering.

**Result / follow-up:** The HUD listens to `ProgressManager` and `GravityController` events. Initial Unity Editor Play Mode verification passed: progress and gravity labels update on collect, flip, and kill-zone respawn; control hints remain visible.

---

### 2026-05-26 — Centralize SFX and add lightweight flip feedback

**Decision:** Add `AudioManager` with optional Inspector-assigned clips, `FlipScreenFlash` on the HUD canvas, and `R` key level reset via `GameManager.ResetLevel()`.

**Reason:** Baseline requirement #8 asks for simple SFX and visible flip feedback. A single audio component keeps wiring simple; empty clip slots fail silently so testing can continue before final assets are imported. Screen flash gives immediate feedback without moving level geometry. `R` reset improves puzzle iteration during playtesting and demo.

**Alternatives considered:** Per-object AudioSource components, particle-heavy flip effects, requiring audio files in the repository.

**Result / follow-up:** Kenney CC0 clips assigned in Unity under `Assets/Audio/` (RPG Audio, Music Jingles, Digital Audio). Initial Play Mode verification passed for all wired SFX, flip screen flash, and `R` level reset. Source mapping is recorded in [AUDIO_SOURCING.md](AUDIO_SOURCING.md).

---

### 2026-05-26 — Level01 layout: linear level + shuttle platform (Scheme B)

**Decision:** Document an approved four-collectable Level01 in [GAME_CONCEPT.md](GAME_CONCEPT.md) Section 11: fixed C1/C3/C4 visible at start; C2 on a shuttle platform that spawns after C1; platform moves left→right only, despawns after fully exiting the spike corridor, then respawns at the left for a new run.

**Reason:** Baseline mechanics are proven; the next increment is level design depth without unrelated systems. Scheme B (full left-to-right run per platform appearance) matches “next round = next run.” Collectable 4 uses spatial irreversibility: after the player passes the corridor without C4, they remain right of the corridor and cannot board in time to traverse the corridor on board again — `R` is the recovery tool, not another platform loop.

**Alternatives considered:** Order-based key puzzles; infinite two-way platform loops through the corridor; recoverable C4 on later runs from the right side.

**Result / follow-up:** Implementation deferred until documentation was complete. Build in phases P1–P4 per GAME_CONCEPT Section 11.13.

---

### 2026-05-26 — P1 shuttle platform: kinematic move + collision parenting

**Decision:** Implement the shuttle as `MovingPlatform2D` (kinematic `Rigidbody2D`, constant world-direction velocity) plus `ShuttlePlatformController` (spawn at left after a referenced C1 `Collectible`, despawn on `PlatformCorridorExitTrigger`, timed respawn for the next run). Carry the player by parenting on `OnCollision` with the platform; `ReleaseAllRiders()` on despawn and `PlayerController2D.ResetTo()` clears parent on death/`R`.

**Reason:** Matches Scheme B without moving level geometry. Parenting is reliable for a single rider on a kinematic platform in 2D and matches the design note that the player falls if still aboard at despawn. A dedicated exit trigger keeps “fully left the corridor” as a level-authoring choice instead of hard-coded distances.

**Alternatives considered:** `PlatformEffector2D` one-way tops only; moving the player by adding platform delta to velocity each frame; despawning at a fixed X coordinate.

**Result / follow-up:** P1 complete — Play Mode verified (see TESTLOG.md). C2 on platform and corridor blockout continue in P2–P4.

---

### 2026-05-26 — Moving platform carry: velocity + delta, not parenting

**Decision:** Drop transform parenting for the shuttle. `MovingPlatform2D` exposes per-frame `Velocity`, applies `MovePosition` delta to rider bodies, and runs before the player (`DefaultExecutionOrder`). `PlatformRider2D` on the player adds platform velocity in `PlayerController2D.ApplyHorizontalMovement` so custom gravity does not erase carry.

**Reason:** Playtest showed the platform moved but the player slipped off. Parenting a dynamic `Rigidbody2D` while rewriting `body.velocity` each `FixedUpdate` does not keep world motion in sync with a kinematic platform.

**Alternatives considered:** `PlatformEffector2D` one-way only; making the player kinematic while riding; friction-only coupling.

**Result / follow-up:** First retest: carry worked but player could not move/jump/flip on board. Second approach: re-parent while riding; player applies horizontal input via `MovePosition`, keeps only gravity-axis velocity, treats platform contact as grounded, unparents on jump.

---

### 2026-05-26 — Riding mode: parent for carry, separate input from world velocity

**Decision:** While `transform.parent` is a `MovingPlatform2D`, use `ApplyRidingMovement()` (horizontal `MovePosition`, strip non-gravity velocity) instead of adding platform velocity to `body.velocity`. Restore parenting on platform collision; `TryJump` unparents before impulse.

**Reason:** Kinematic platform + per-frame velocity rewrite caused the physics solver to cancel tangential input; double application of platform motion also blocked jumps (ground check failed on moving colliders).

**Result / follow-up:** Parenting-only riding failed retest (platform left player behind). Final P1 carry: platform `MovePosition` delta applied to rider bodies at execution order -50; player at +50 uses contact-based riding (horizontal `MovePosition`, gravity-axis velocity only, no extra platform velocity term).

---

### 2026-05-26 — Dynamic player cannot ride via transform parenting

**Decision:** Do not parent the player to the kinematic platform. Carry riders by applying the same `MovePosition` delta to the player `Rigidbody2D` each physics step.

**Reason:** A dynamic `Rigidbody2D` child does not reliably follow a kinematic parent moved with `Rigidbody2D.MovePosition`; parenting caused the platform to slide away from the player.

**Alternatives considered:** Make player kinematic while riding; friction joints; platform velocity added to `body.velocity` without delta (slip).

**Result / follow-up:** Combined with contact-based riding movement for input/jump/flip (see prior riding-mode entry). Pending P1 retest.

---

### 2026-05-26 — Platform carry only on walkable contact normals

**Decision:** Register riders and active platform contact only when a collision contact normal aligns with the current “up” (`-gravityDirection`), via `MovingPlatformContact.HasWalkableSupport`. Side bumps unregister the rider instead of carrying.

**Reason:** P1 playtest passed, but brushing the platform’s vertical edges pulled the player along unrealistically.

**Alternatives considered:** Layered child colliders (top-only); one-way platform effector; ignoring all side collisions globally.

**Result / follow-up:** Retest passed after abs-normal alignment and gravity cast fallback. P1 shuttle milestone complete in Play Mode.
