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
