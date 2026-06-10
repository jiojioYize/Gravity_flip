# Gravity Flip — Technical Decisions

This file records important design and programming decisions made during development. It is intended to support the final report by keeping track of what was chosen, why it was chosen, and how the choice affected the game.

Update this document when a decision affects gameplay, architecture, testing, scope, accessibility, assets, or the final player experience.

**Entry dates:** Use the day the decision was **finalized** (usually when Play Mode verification passed or the change was pushed). If an approach was tried and replaced later, keep the earlier date on the first entry and add a later-dated entry for the final approach — do not leave misleading “pending” follow-ups on old entries.

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

**Result / follow-up:** Superseded by the 2026-05-31 P1 implementation entry after Play Mode iteration (parenting and early velocity-only carry were insufficient).

---

### 2026-05-31 — P1 shuttle platform: final carry, riding controls, and walkable-only contact

**Decision:** Ship P1 with:

- `MovingPlatform2D` + `ShuttlePlatformController` + `PlatformCorridorExitTrigger` (spawn after C1, left→right, despawn at corridor exit, respawn at left).
- **Delta carry:** apply the platform’s `MovePosition` delta to rider `Rigidbody2D` bodies each step — **no** transform parenting (dynamic player does not follow a kinematic parent reliably).
- **Script order:** platform `FixedUpdate` at 100, player at 150; horizontal strafe via `PlayerController2D.ApplyPlatformStrafeAfterCarry()` immediately after the platform moves.
- **Riding:** gravity-axis velocity only while on board; jump/flip call `ReleaseFromPlatform()` and a short interaction block; flip input uses legacy **`Fire3`** (Left Shift) with cooldown in `GravityController` to avoid double-flip.
- **Walkable-only contact:** `MovingPlatformContact.HasWalkableSupport` uses `|dot(contactNormal, up)|` plus a gravity-direction cast fallback so top/bottom boarding works but side bumps do not stick.

**Reason:** Play Mode iteration showed parenting and velocity-only coupling both failed (slip, no strafe, blocked jump/flip, or side-edge carry). Kinematic contact normals differ per callback object; abs alignment and cast fallback fix false negatives on the player side.

**Alternatives considered:** Parenting; platform velocity added to `body.velocity`; `MovePosition` strafe before platform move; side collisions ignored globally; top-only child colliders.

**Result / follow-up:** P1 milestone verified in Unity Editor Play Mode (commit `445cef3`). See [TESTLOG.md](../Assets/Documentation/TESTLOG.md). C2 implemented in P2; corridor blockout continues in P3–P4.

---

### 2026-06-01 — Horizontal camera follow for linear Level01

**Decision:** Add `CameraFollow2D` on `Main Camera`: **horizontal scroll only** by default (`lockVerticalPosition` uses the camera’s placed world Y; `useRelativeHorizontalFollow` moves X by player delta from start so spawn framing is preserved). Optional `SmoothDamp`, offset, and min/max bounds.

**Reason:** After P1 verification, the level is wider than one screen. Following both X and Y pulled the authored ceiling/floor out of frame. Relative X follow keeps the original vertical composition while the player progresses right.

**Alternatives considered:** Cinemachine package; fixed wide orthographic size only; full X+Y follow to player position (rejected after playtest feedback).

**Result / follow-up:** Verified in Play Mode with locked Y and relative horizontal follow (see [TESTLOG.md](../Assets/Documentation/TESTLOG.md), SETUP.md section 11).

---

### 2026-06-01 — P2 Collectable 2 bound to shuttle runs

**Decision:** Add `PlatformBoundCollectible` on a child of `ShuttlePlatform` with `Collectible` + `BoxCollider2D`. GameObject stays active at load for progress registration; renderer and trigger toggle on `PlatformRunStarted` / `PlatformRunEnded`. Platform visibility toggles root renderers/colliders only so C2 is not forced visible with the shuttle body.

**Reason:** Design requires C2 only while a platform instance is active, with another chance on the next run if missed, without breaking HUD totals from an inactive collectible `Awake`.

**Alternatives considered:** Per-run spawn prefab; whole-object `SetActive` on C2 at load; child included in platform `GetComponentsInChildren` hide (rejected — would show C2 whenever shuttle shows).

**Result / follow-up:** Verified in Unity Editor Play Mode on 2026-06-01. See [TESTLOG.md](../Assets/Documentation/TESTLOG.md). P3 C3 placement and hazard clearance next.

---

### P3 — Collectable 3 and pit hazard (Editor blockout)

**Decision:** P3 uses existing `Collectible` and `KillZone` only — fixed **Collectable3** visible at load, ground gap with **KillZone_C3Pit** trigger, split ground lips for jump-from-shuttle / jump-right clearance. Unified **Collect Clip** and **Death Clip** on `AudioManager`.

**Reason:** No new systems needed; P3 is level design and verification of shuttle dismount → aerial clearance → forward-only route before the P4 corridor.

**Alternatives considered:** Per-hazard audio overrides (rejected — unified SFX policy); custom pit script (rejected — `KillZone` is sufficient).

**Result / follow-up:** Verified in Unity Editor Play Mode on 2026-06-01. See [TESTLOG.md](../Assets/Documentation/TESTLOG.md). P4 spike corridor and C4 next.

---

### P4 — Spike corridor, Collectable 4, and level complete

**Decision:** P4 is Editor blockout plus **`requireActiveShuttleRun`** on C4’s `Collectible`: pickup only while `ShuttlePlatformController.IsRunActive`. Corridor uses `KillZone` triggers on floor/ceiling bands (unified death SFX); no walkable ground through the span. C4 is a **timed jump** collect after flipping to the platform underside and entering the spike corridor — not passive trigger overlap while riding.

**Reason:** Playtest clarified the route: flip under the platform, enter the hazard corridor during the run, then jump into C4. Requiring platform **contact** at pickup blocked that jump. Active-run gating still prevents collecting C4 on foot at level start while allowing mid-air pickup.

**Alternatives considered:** `requireMovingPlatformContact` (rejected after playtest — conflicts with jump timing); child C4 on platform (rejected — fixed position per design).

**Result / follow-up:** Blockout in [SETUP.md](../SETUP.md) section 16; [GAME_CONCEPT.md](GAME_CONCEPT.md) §11.9 updated. End caps (section 14) after final dimensions.

---

### Visual framing — backdrop and optional camera fit

**Decision:** Add `LevelBackdrop2D` (scaled sprite rectangle behind gameplay) and optional `CameraFollow2D.fitPlayAreaOnStart` to match orthographic size to the floor–ceiling band. Document tuning in SETUP section 17.

**Reason:** Horizontal follow exposes finite ground/ceiling ends and default orthographic size (~8.9) shows camera clear colour above/below the play strip — breaks immersion.

**Alternatives considered:** Only shrink orthographic size manually (still need side cover); UI letterbox bars; infinite ground sprites (rejected — collider scope).

**Result / follow-up:** `LevelBackdrop2D` + Min/Max tuning verified in Play Mode; left gap fixed by lowering **Min X** (not Max X). See SETUP section 17.

---

### 2026-06-03 — HUD screen-space lock and art-ready panels

**Decision:** Keep HUD on **Screen Space - Overlay** with corner/bottom **`HudScreenAnchor`** (or equivalent RectTransform anchors). Optional panel wrapper (Image + Text) per block so UI sprites swap without touching `GameplayHUD` logic.

**Reason:** World camera scroll and vertical framing must not move gameplay hints; Overlay UI is independent of `CameraFollow2D`. Panel wrapper separates data (`GameplayHUD` strings) from presentation (sprites).

**Alternatives considered:** World Space HUD parented to player (rejected); Screen Space - Camera (rejected for this project); immediate TextMeshPro migration (deferred).

**Result / follow-up:** Verified in Play Mode (2026-06-03). `StatusPanel` stack and layout fix documented in the 2026-05-26 entry below. See [SETUP.md](../SETUP.md) sections 5 and 18.

---

### 2026-06-03 — UI reference resolution 600×700

**Decision:** Use **Canvas Scaler → Scale With Screen Size** with reference resolution **600×700** and **Match 0.5** on `Level01` gameplay HUD and on runtime flow UI created by `OverlayUiBuilder` (main menu, pause, instructions, win panels).

**Reason:** One reference size keeps margins and font tuning consistent between scene HUD and code-built overlays. The value matches the Game view proportions used during Level01 layout and playtesting; `Scale With Screen Size` still adapts to the actual display at runtime.

**Alternatives considered:** `1920×1080` (common PC default — rejected for this slice because HUD margins were tuned at 600×700); constant pixel scale (rejected — poor fit across aspect ratios).

**Result / follow-up:** Documented in [SETUP.md](../SETUP.md) sections 5 and 18. No change required while full acceptance passes on target Game view sizes.

---

### 2026-06-08 — Kenney backdrop sprite; editor-only backdrop tint

**Decision:** Assign Kenney `tile_0000` as `Assets/Tiles/Background.png` on `LevelBackdrop` (`LevelBackdrop2D` bounds scaling). Remove script-driven `backdropColor` from `LevelBackdrop2D` — backdrop tint is **Editor-only** via **Sprite Renderer → Color**.

**Reason:** Art pass should not overwrite hand-tuned scene colours on rebuild/play. Separating bounds scaling (script) from palette (Inspector) matches the project rule that level visuals are Editor-owned after the 2026-05-26 art-incident recovery.

**Alternatives considered:** Procedural full-scene colour restore (rejected); script-set colour every `OnValidate` (rejected — fights manual Kenney tint).

**Result / follow-up:** Verified in Play Mode (2026-06-08). Mapping in [SPRITE_SOURCING.md](SPRITE_SOURCING.md); setup in [SETUP.md](../SETUP.md) section 17.

---

### 2026-06-09 — Final build uses 1920×900 windowed presentation

**Decision:** Export the final Windows Standalone build as a **windowed 1920×900** build instead of native fullscreen or a small 600×700 window.

**Reason:** Native/default fullscreen displayed too much horizontal space and made the authored Level01 composition feel wrong. The earlier 600×700 window preserved the UI scale but felt too small and narrow for players. A 1920×900 window keeps the wide final composition readable while avoiding monitor-dependent fullscreen stretching.

**Alternatives considered:** Native fullscreen 16:9 (rejected after build preview); 600×700 or 900×1050 windowed builds (rejected after testing because they did not present the final level comfortably); camera letterboxing/aspect-lock scripts (rejected after preview because the result was too narrow).

**Result / follow-up:** User-confirmed Windows build test passed on 2026-06-09. Documented in [SETUP.md](../SETUP.md) and [TESTLOG.md](../Assets/Documentation/TESTLOG.md).

---

### 2026-05-26 — `StatusPanel` stacked HUD layout

**Decision:** Group **ProgressText** and **GravityText** under a **`StatusPanel`** child of `Canvas`. Apply **`HudScreenAnchor` (Top Left)** only on the panel (margin e.g. `(48, 56)`). Child text uses top-left anchor/pivot with local positions **`(0, 0)`** and **`(0, -32)`** — no `HudScreenAnchor` on the text objects. Apply anchors in **`HudScreenAnchor.Start()`** after **`Canvas.ForceUpdateCanvases()`**. Do not save the scene in Play Mode (avoids baked child scales).

**Reason:** Play Mode testing showed progress/gravity labels clipped in Game view when children kept stale canvas-relative offsets and non-`(1,1,1)` scales from an earlier hierarchy. `ControlsText` worked as a direct `Canvas` child with manual anchors; the panel stack needed explicit child-local layout rules.

**Alternatives considered:** Separate `HudScreenAnchor` per text with different margins (works but duplicates margin tuning); World Space HUD (rejected).

**Result / follow-up:** Verified in Play Mode (2026-05-26 full acceptance). See [TESTLOG.md](../Assets/Documentation/TESTLOG.md) and [SETUP.md](../SETUP.md) §18 StatusPanel stack.

---

### 2026-06-01 — Walkway end caps deferred until blockout

**Decision:** Fix “walk off finite ground/ceiling ends” with **invisible BoxCollider2D end walls** on the `Ground` layer at the left/right edges of the final walkway span. **Do not add** until P3–P4 blockout is done and `Ground` / `Ceiling` length is final.

**Reason:** End positions depend on total level width; placing caps on the current short platforms would need moving again when geometry grows. Editor-only colliders match project conventions (no position-clamp script).

**Alternatives considered:** Extend platforms only (still need end caps at level extremities); side kill zones (respawn punishment); `CameraFollow2D` bounds (camera only); runtime X clamp on player.

**Result / follow-up:** Verified in Play Mode (2026-06-03). See [SETUP.md](../SETUP.md) section 14 and [TESTLOG.md](../Assets/Documentation/TESTLOG.md).

---

### 2026-06-04 — Scoped game flow UI (UX-1–3)

**Decision:** After Level01 baseline (P1–P4) verification, add a **trimmed** player journey in three implementation steps (no art assets required for first pass):

1. **UX-1 — `MainMenu` scene:** Short story blurb (2–4 sentences) + **Start** button → `SceneManager.LoadScene("Level01")`. No auto-skip-only entry; button required for demos and clarity.
2. **UX-2 — Pause overlay in Level01:** `Esc` toggles pause (`Time.timeScale = 0`); panels: **Resume**, **Instructions** (full controls + level goal), **Return to main menu**. Legacy Input Manager only.
3. **UX-3 — Win overlay:** On `GameManager.CompleteLevel()`, show a simple panel: **Play again** (reload `Level01` or reset state) and **Main menu**.

**Death / failure UX:** Keep **quiet respawn** — existing kill-zone behaviour (death SFX, respawn at `SpawnPoint`, gravity down, progress reset). **No** full-screen failure or Game Over modal. Optional future: one-line HUD hint only; not in scope for UX-1–3.

**Explicitly out of this stretch slice:** Auto-enter level without Start; full-screen death popup; `Application.Quit` as primary exit (optional later); BGM; multi-page story cinematics; art-dependent blocking.

**Reason:** Matches [GAME_CONCEPT.md](GAME_CONCEPT.md) Stretch A and assessment focus: baseline gameplay is proven; flow UI improves presentation and report narrative without changing puzzle rules. Quiet death aligns with teaching-level loops and existing `R` reset (death SFX vs reset SFX).

**Alternatives considered:** Full ideal flow in one milestone (menu + pause + exit + story + win + fail popups + art) — rejected as too large (~3–5 extra days) and asset-dependent; hard fail Game Over — rejected by team preference.

**Architecture:** `GameFlowController` on `GameManager` for panel visibility, pause gate on player input, and scene loads — separate from `ProgressManager` / `GameplayHUD` data paths. Runtime Canvas panels via `OverlayUiBuilder` and `MainMenuController` (placeholder UI until optional Kenney swap).

**Result / follow-up:** Verified in Play Mode (2026-06-04, `09d678e`). See [SETUP.md](../SETUP.md) section 19 and [TESTLOG.md](../Assets/Documentation/TESTLOG.md).

---

### 2026-05-26 — Reject procedural scene-wide art automation; document incident

**Decision:** Do **not** ship editor menus that bulk-assign generated sprites into `Level01` (or any hand-tuned scene). Kenney or manual per-object swaps only. Level visuals stay **Editor-only** (scene `SpriteRenderer` colors/sprites); gameplay scripts do not override hazard appearance.

**Reason:** A procedural art pass wrote sprite references under `Assets/Resources/Art/`, then those assets were deleted on revert. The scene kept broken GUIDs (Hierarchy intact, Scene view empty). Emergency repair restored visibility with default white squares but **wiped hand-set `SpriteRenderer` colors** and made **KillZone** volumes look like solid white platforms (colliders were still triggers; gameplay feel and level read changed).

**Alternatives considered:** Re-commit generated art (rejected by user); hand-edit entire scene YAML (rejected — fragile GUIDs); only document manual fix (insufficient — added palette restore from last verified baseline commit `09d678e`).

**Process change:** Before any tool that modifies open scenes, deletes asset folders, or runs “apply to all,” state risks explicitly: missing references, lost tints, need for Git/scene backup.

**Result / follow-up:** Art scripts, temporary repair menus, and `GravityFlipSceneSetup` editor menu removed after user recovery (2026-05-26). `KillZone` reverted to respawn + runtime `isTrigger` only. MainMenu/Build Settings are maintained as scene assets in the repo; manual steps in SETUP §19 if recovery is needed.
