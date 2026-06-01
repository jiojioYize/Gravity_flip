# Gravity Flip — Unity Editor Setup

Step-by-step instructions for building scenes in the Unity Editor.  
**This file is updated as features are implemented** — sections marked *(pending)* are not ready yet.

Prerequisites: Unity **2022.3 LTS**, project opened from repository root.

---

## 1. Project settings *(pending — do during Week 1 D1–2)*

### Tags and layers

Create layers (Edit → Project Settings → Tags and Layers):

| Layer | Use |
|-------|-----|
| `Ground` | Floor platforms |
| `Ceiling` | Top walkable surfaces |
| `Player` | Player collider |
| `Hazard` | Kill zones, spikes |

Alternatively use a single `Walkable` layer for ground + ceiling if ground check uses combined mask.

### Physics 2D

- Edit → Project Settings → Physics 2D
- Default gravity: `(0, -9.81)` — flipped at runtime by `GravityController`
- Ensure `Ground` / `Ceiling` / `Player` collision matrix allows player ↔ platform contact

---

## 2. Folder layout *(partially complete)*

Current / planned layout in `Assets/`:

```
Scenes/           *(pending)*
Scripts/Core/     `GravityController`, `CameraFollow2D`, `GameManager`, `ProgressManager`
Scripts/Player/   `PlayerController2D`
Scripts/Level/    `Collectible`, `PlatformBoundCollectible`, `ExitDoor`, `KillZone`, `MovingPlatform2D`, `ShuttlePlatformController`, `PlatformCorridorExitTrigger`
Scripts/UI/       `GameplayHUD`
Scripts/Audio/    `AudioManager`
Prefabs/          *(pending)*
Sprites/          *(pending)*
Audio/            *(pending)*
```

Create pending folders when their first asset or script is added.

---

## 3. Level01 scene *(pending — Week 1 D4+)*

### Create scene

1. File → New Scene → 2D
2. Save as `Assets/Scenes/Level01.unity`

### Camera

- Main Camera: Orthographic, size ~5–6 (or ~8 if the level feels tight before follow is added), background colour of choice
- Add `CameraFollow2D` — see [section 10](#10-camera-follow-2d-implemented--requires-unity-binding)

### Level geometry

Build with 2D sprites + BoxCollider2D (or Tilemap):

| Object | Notes |
|--------|-------|
| Ground | Bottom platform spanning level width |
| Ceiling | Top platform (walkable when gravity inverted) |
| Side walls | **Deferred** — invisible end caps after full blockout (see [section 14](#14-walkway-end-caps-deferred-until-blockout-complete)) |
| Mid platforms | Support reference puzzle layout |
| Spawn point | Empty GameObject at player start position |
| Collectable | Trigger collider; unreachable without flip puzzle |
| Exit door | Collider + script; locked until collect complete |
| Kill zone | Trigger below level or spike objects on `Hazard` layer |

### Reference puzzle layout

Align with [docs/GAME_CONCEPT.md](docs/GAME_CONCEPT.md) Section 2:

- Collectable above a high area — not reachable by ground jump alone
- Ceiling path lets inverted player walk under collectable
- Second flip drops player past collectable to pick it up

---

## 4. Player prefab *(script ready — requires Unity binding)*

1. Create empty `Player` GameObject
2. Add `SpriteRenderer` (placeholder square sprite)
3. Add `Rigidbody2D`: Dynamic, Freeze Rotation Z, Continuous collision
4. Add `CapsuleCollider2D` or `BoxCollider2D`
5. Add `PlayerController2D` script
6. Set `Walkable Layers` to include floor and ceiling layers (`Ground` + `Ceiling`, or a unified `Walkable` layer)
7. Assign the scene `GravityController` reference, or leave it empty if there is only one `GravityController` in the scene
8. Start with these tuning values, then adjust in Play Mode:
   - Move Speed: `7`
   - Jump Speed: `12`
   - Custom Gravity: `28`
   - Ground Check Distance: `0.08`
9. `PlatformRider2D` is added automatically via `PlayerController2D` (`RequireComponent`); no extra binding needed
10. Drag to `Assets/Prefabs/Player.prefab`

---

## 5. UI canvas *(implemented — bind if setting up a new scene)*

### Create canvas

1. In `Hierarchy`, right-click → `UI` → `Canvas`
2. Unity may auto-create `EventSystem`; keep it if present
3. Select `Canvas`; in `Inspector` confirm **Render Mode** = `Screen Space - Overlay`

### Progress text (top-left)

1. Right-click `Canvas` → `UI` → `Text`
2. Rename to `ProgressText`
3. Set **Anchor Preset**: top-left (hold `Alt` + click top-left preset)
4. Set **Pos X** `20`, **Pos Y** `-20`
5. Set **Text** to `Keys 0/1` (placeholder)
6. Set **Font Size** `24`, **Color** white (or readable on your background)

### Gravity text (top-right)

1. Right-click `Canvas` → `UI` → `Text`
2. Rename to `GravityText`
3. Anchor preset: top-right (`Alt` + top-right)
4. **Pos X** `-20`, **Pos Y** `-20`
5. **Text** `Gravity: Down` (placeholder)
6. **Font Size** `24`

### Controls text (bottom)

1. Right-click `Canvas` → `UI` → `Text`
2. Rename to `ControlsText`
3. Anchor preset: bottom-center (`Alt` + bottom-center)
4. **Pos X** `0`, **Pos Y** `20`
5. **Text** `A/D Move  |  Space Jump  |  Shift Flip Gravity` (placeholder)
6. **Font Size** `20`
7. **Alignment**: center

### GameplayHUD component

1. Select `Canvas`
2. `Add Component` → `GameplayHUD`
3. Assign references:
   - **Progress Manager**: drag `--- Managers ---` (or leave empty if only one in scene)
   - **Gravity Controller**: drag `--- Managers ---`
   - **Progress Text**: drag `ProgressText`
   - **Gravity Text**: drag `GravityText`
   - **Controls Text**: drag `ControlsText`
4. Save scene (`Ctrl + S`)

At runtime, progress updates when collectables are picked up or reset; gravity label updates on flip and after kill-zone respawn.

---

## 6. Managers *(core scripts ready — requires Unity binding)*

Empty GameObject `--- Managers ---` in scene with:

- `GravityController`
- `GameManager`
- `ProgressManager`
- `AudioManager`

Create a `SpawnPoint` empty GameObject at the player's starting position.

Assign `GameManager` references:

- Player: the `Player` object with `PlayerController2D`
- Spawn Point: the `SpawnPoint` object
- Gravity Controller: the scene `GravityController`
- Progress Manager: the scene `ProgressManager`

`GravityController` has no required Inspector references for the first movement test. Keep `Initial Gravity Direction` as `(0, -1)`.

`ProgressManager` has no required Inspector references. `Collectible` objects register themselves at runtime.

---

## 7. Collectable, exit, and death reset *(script ready — requires Unity binding)*

### Collectable

1. Create `2D Object > Sprites > Square` and name it `Collectable`
2. Set a visible colour, such as yellow
3. Add `BoxCollider2D`
4. Add `Collectible` script
5. Place it where the player must use gravity flip to reach it
6. Leave `Progress Manager` empty if there is only one `ProgressManager` in the scene, or assign it manually

The `Collectible` script sets its collider to trigger at runtime. For **C4 only**, enable **Require Active Shuttle Run** (see [section 16](#16-spike-corridor-collectable-4-and-exit--p4-implemented--requires-unity-blockout)).

### Exit door

1. Create `2D Object > Sprites > Square` and name it `ExitDoor`
2. Set its scale to look like a door, for example `(1, 2, 1)`
3. Add `BoxCollider2D`
4. Add `ExitDoor` script
5. Assign `Progress Manager`, `Game Manager`, and `Sprite Renderer` references, or leave them empty if there is only one of each in the scene

The door starts locked, changes colour when all collectables are collected, and calls `GameManager.CompleteLevel()` when the player reaches it.

### Kill zone

1. Create an empty GameObject named `KillZone`
2. Add `BoxCollider2D`
3. Set `Is Trigger` on the collider
4. Add `KillZone` script
5. Scale and place it below the level or near hazards
6. Assign `Game Manager`, or leave it empty if there is only one `GameManager` in the scene

When the player enters the kill zone, the player respawns, gravity resets to normal, and collectable progress resets.

---

## 8. Audio and polish feedback *(implemented — bind if setting up a new scene)*

### Import sound files

1. Create folder `Assets/Audio/`
2. Import short `.wav` / `.ogg` clips (Kenney impact/UI packs are fine)
3. Suggested mapping:

| Inspector field | Suggested use |
|---------------|---------------|
| Jump Clip | Jump |
| Flip Clip | Gravity flip |
| Collect Clip | **All** collectables (C1, C2, C3, C4) — one shared sound |
| Death Clip | **All** `KillZone` hazards — one shared sound |
| Door Unlock Clip | Exit turns green |
| Level Complete Clip | Touch exit after unlock |
| Level Reset Clip | Press `R` (optional; leave empty if none) |

### AudioManager on Managers

1. Select `--- Managers ---`
2. `Add Component` → `AudioManager`
3. Unity adds an `Audio Source` automatically if missing
4. Drag each imported clip into the matching field (empty slots are skipped at runtime)

### Flip screen flash

1. Select `Canvas`
2. Right-click `Canvas` → `UI` → `Image`
3. Rename to `FlipFlashOverlay`
4. Anchor: stretch full screen (hold `Alt`, click stretch-stretch preset)
5. Set **Left / Top / Right / Bottom** offsets to `0`
6. Set **Color** to white with alpha `0` (fully transparent)
7. Disable **Raycast Target** on the Image (optional, avoids blocking clicks)
8. Select `Canvas` → `Add Component` → `FlipScreenFlash`
9. Assign **Flash Image** → `FlipFlashOverlay`
10. **Gravity Controller** → `--- Managers ---` (or leave empty)

### Unified SFX policy

- **Collectables:** only assign **Collect Clip** on `AudioManager`. Every `Collectible` (C1, C2, …) plays that clip automatically — no per-object audio fields.
- **Hazards:** only assign **Death Clip** on `AudioManager`. Every `KillZone` uses it via `GameManager.RespawnPlayer()`.

### Verify polish

- [ ] Jump / flip / collect / death / door unlock / level complete play when clips are assigned
- [ ] **C1** and **C2** both play the same collect sound
- [ ] `R` resets player, progress, and gravity (reset sound optional)
- [ ] Flip shows a brief screen flash
- [ ] HUD bottom text mentions `R Reset`

---

## 9. Shuttle platform — P1 *(implemented — requires Unity binding)*

Implements [GAME_CONCEPT.md](docs/GAME_CONCEPT.md) Section 11.6 (Scheme B): platform inactive until Collectable 1 is taken, moves left→right, despawns after the corridor exit trigger, then respawns at the left for the next run. Player rides via collision parenting; riders detach and fall when the platform despawns.

### Create the platform object

1. In `Hierarchy`, create `2D Object > Sprites > Square`
2. Rename to `ShuttlePlatform`
3. Set a distinct colour (e.g. cyan) and scale to a platform size (e.g. `(3, 0.5, 1)`)
4. Add `Rigidbody2D`:
   - **Body Type**: `Kinematic` (script also forces this at runtime)
   - **Freeze Rotation** Z: on
5. Add `BoxCollider2D` — **not** a trigger (player stands on top or bottom)
6. Add `MovingPlatform2D`
7. Add `ShuttlePlatformController` on the same object (supported — controller stays enabled while the platform is hidden)
8. Set layer to `Ground` (or your walkable layer) so the player ground check and collisions work

**Optional hierarchy:** Put `ShuttlePlatformController` on a parent empty `ShuttlePlatformSystem` and the moving body on a child; the script can hide the child with `SetActive` if references point at the child.

### Spawn point (left dock)

1. Create empty GameObject `ShuttleSpawnPoint`
2. Place it where each run should start (left side of the platform track, before the C2/C4 corridor)
3. On `ShuttlePlatform` → `Shuttle Platform Controller`:
   - **Moving Platform**: drag `ShuttlePlatform` (self)
   - **Spawn Point**: drag `ShuttleSpawnPoint`
   - **Activation Collectible**: drag the **first** collectable (`Collectable` / C1)
   - **Progress Manager**: leave empty if only one manager exists
4. **Respawn Delay**: `0.5` (adjust if runs feel too fast)

At Play Mode start the platform should be **hidden** until C1 is collected.

### Corridor exit trigger (despawn gate)

Place this at the **right end** of the spike corridor so the platform only despawns after it has fully passed through the corridor (per design).

1. Create empty GameObject `PlatformCorridorExit`
2. Add `BoxCollider2D`, enable **Is Trigger**
3. Scale the collider to span the corridor height/width at the exit line (platform collider must enter this volume)
4. Add `PlatformCorridorExitTrigger`
5. Assign **Shuttle Controller** → `ShuttlePlatform`, or leave empty if only one controller exists

### Physics matrix

- `Player` ↔ platform layer: collision enabled
- Platform uses non-trigger collider so `MovingPlatform2D` receives `OnCollision` with the player

### Verify P1 (Play Mode)

- [ ] Before C1: `ShuttlePlatform` is inactive / not moving
- [ ] After C1: platform appears at `ShuttleSpawnPoint` and moves **right only**
- [ ] Standing on the platform while moving: player moves with it
- [ ] Jumping off: player leaves the platform normally
- [ ] When platform enters `PlatformCorridorExit` trigger: platform hides; player on board **falls** (parent released)
- [ ] After respawn delay: platform reappears at the left and starts another run
- [ ] Kill zone or `R` reset: progress clears → platform system stops until C1 is collected again
- [ ] Death/`R` respawn: player is not stuck parented to a hidden platform

P1 only needs movement, activation, despawn loop, and carry. **P2** adds Collectable 2 on the platform (section 10).

---

## 10. Collectable 2 on shuttle platform — P2 *(implemented — verified 2026-06-01)*

Implements [GAME_CONCEPT.md](docs/GAME_CONCEPT.md) Section 11.7: C2 rides on the shuttle, is **hidden at level start**, appears only during an active platform run, and respawns on the next run if missed. Top and bottom boarding use the same walkable-surface rules as P1 (`MovingPlatformContact`).

### Create C2 as a child of the platform

1. In `Hierarchy`, expand `ShuttlePlatform`
2. Create `2D Object > Sprites > Square` **as a child** of `ShuttlePlatform`
3. Rename to `Collectable2`
4. Set a distinct colour (e.g. gold) and scale (e.g. `(0.35, 0.35, 1)`)
5. Set **Transform** local position on top of the platform (e.g. `x: 0`, `y: 0.45`) — adjust in Scene view
6. Ensure **Box Collider 2D** is present (Square sprite usually adds one); `Collectible` sets **Is Trigger** at runtime
7. Add `Collectible` (registers with `ProgressManager` at load)
8. Add `PlatformBoundCollectible`:
   - **Shuttle Controller**: drag `ShuttlePlatform` (or leave empty)
   - **Pickup Renderer** / **Pickup Collider**: drag the components on `Collectable2` (or leave empty — auto-fills)
9. Keep the **GameObject active** in the hierarchy (script hides renderer/collider until a run starts)
10. **Audio:** no extra setup — C2 uses the same **Collect Clip** as C1 on `--- Managers ---` → `AudioManager` (see [section 8](#8-audio-and-polish-feedback-implemented--bind-if-setting-up-a-new-scene))

### Progress and naming

- Rename the first collectable in the Inspector to `Collectable1` (optional, for clarity); shuttle **Activation Collectible** must still reference C1.
- HUD should show `0/2` (or `0/4` when C3/C4 exist) once both collectables are in the scene and active at load.

### Verify P2 (Play Mode)

- [ ] Before C1: `Collectable2` is **not** visible and cannot be collected
- [ ] After C1: when the shuttle run starts, C2 appears on the moving platform
- [ ] Collect C2 while riding (normal gravity on top) or after flipping to ceiling and dropping onto the platform (underside boarding)
- [ ] Miss C2: after the platform despawns and respawns at the left, C2 appears again on the next run
- [ ] After collecting C2: it stays gone for later runs in the same attempt
- [ ] `R` or kill zone: progress resets; C2 hidden again until C1 is collected and a new run starts
- [ ] P1 loop still works (carry, strafe, jump, flip, corridor despawn)

**P3** adds Collectable 3 and the pit hazard (section 15). **P4** adds the spike corridor and C4 (section 16).

---

## 15. Collectable 3 and pit hazard — P3 *(implemented — verified 2026-06-01)*

Implements [GAME_CONCEPT.md](docs/GAME_CONCEPT.md) Section 11.8: **C3** is visible from level start, sits on the **forward route** after the shuttle segment and **before** the C4 corridor, above a **ground pit** the player clears with a planned jump (often: ride shuttle → dismount → jump right over the gap → collect C3 → continue toward the corridor).

**Scripts:** reuse `Collectible` (pickup + unified collect SFX) and `KillZone` (unified death SFX). No new gameplay code for P3.

### Layout goals (left → right)

```text
… shuttle track (C2) … → dismount zone → [C3 over pit] → safe ground → C4 corridor (P4) → door
```

Reference positions in the current `Level01` draft (tune in Scene view):

| Marker | Approx. world X | Notes |
|--------|-----------------|-------|
| `ShuttleSpawnPoint` | -18 | Platform run starts |
| C3 pit / collect zone | 10 – 14 | Between shuttle path and `PlatformCorridorExit` |
| `PlatformCorridorExit` | 17 | Move **further right** if the pit needs more space before the corridor |

### A. Split ground and add the pit

1. Duplicate or slice the bottom `Ground` into two walkable strips with a **gap** (no collider in the gap):
   - `Ground_C3_Left` — left lip (approach from shuttle / on foot)
   - `Ground_C3_Right` — right lip (landing after the jump)
2. Gap width: start around **3–5** units; player must **jump from the left lip or from a dismounted shuttle** and move **right** in the air to reach the right lip or C3.
3. Optional red spike sprites in the gap (**Layer** `Hazard`, **no** extra script) for readability only.

### B. Kill zone in the pit

1. `Create Empty` → `KillZone_C3Pit` (child under a `Hazards` empty if you like)
2. **Layer:** `Hazard` (or default — must still trigger player)
3. Add **Box Collider 2D**, enable **Is Trigger**
4. Stretch the box to fill the gap and extend **below** the play line (same idea as the existing `KillZone` under the level)
5. Add **`KillZone`** script (`Game Manager` can stay empty)
6. **Physics 2D:** `Player` must collide with triggers (default)

Falling into the pit plays **Death Clip** and respawns at `SpawnPoint` with progress reset.

### C. Collectable 3

1. `2D Object > Sprites > Square` → rename **`Collectable3`**
2. Place on or just above the **right lip** or mid-air over the pit so a **rightward jump** from the shuttle/left lip collects it while clearing the hazard (tune Y ≈ ground height + 1–2)
3. Distinct colour (e.g. green)
4. **Box Collider 2D** + **`Collectible`** (same as C1)
5. Leave active at load — **visible from start**; HUD should show **`Keys 0/3`** until C4 exists (**`0/4`** later)

### D. Shuttle / platform tuning for the jump

1. After C1, ride the shuttle and collect C2 (P2)
2. **Dismount** while the platform is still on the left/mid track (jump off) **before** the pit, or time a jump from the platform edge toward C3
3. If the jump is too hard: lower gap width, raise `Jump Speed` slightly on `Player`, or add a small `Ground_C3_Mid` lip (only if design allows)
4. Ensure the platform track still reaches `PlatformCorridorExit` for P4 (corridor may move right)

### E. One-way route (no backtracking)

Before final end caps ([section 14](#14-walkway-end-caps-deferred-until-blockout-complete)):

- Do **not** leave a flat walk back from the right lip to re-collect C1/C2 without `R`
- Options: raised right lip, low tunnel, or a **short blocker** past C3 so returning left is impossible
- Missing C3 after passing the pit is recovered with **`R`**, not another shuttle run from the wrong side

### Verify P3 (Play Mode)

- [ ] HUD **`0/3`** (or `0/4` with C4) at start; C3 visible before C1
- [ ] C3 pickup uses the same **Collect Clip** as C1/C2
- [ ] Pit triggers **Death Clip** and full respawn (gravity down, progress reset)
- [ ] Intended route: shuttle → C2 → dismount/jump → collect C3 over pit → can continue right toward corridor
- [ ] Cannot skip C3 and reach the corridor on foot without collecting (blockout check)
- [ ] P1 shuttle loop and P2 C2 still work

---

## 16. Spike corridor, Collectable 4, and exit — P4 *(implemented — requires Unity blockout)*

Implements [GAME_CONCEPT.md](docs/GAME_CONCEPT.md) Sections 11.9–11.10: a **spike corridor** reached during an **active shuttle run**; **C4** collected with a **timed jump** after flipping to the platform underside and entering the corridor, then **dismount** on safe ground and walk to the **exit** at `4/4`.

**Scripts:** `KillZone` for corridor hazards (unified **Death Clip**); `Collectible` with optional **Require Active Shuttle Run** for C4 (allows jump pickup while the run is active — not while standing on foot before C1); existing `PlatformCorridorExitTrigger` and `ExitDoor`.

### Layout (left → right)

```text
… C3 pit … → [corridor: floor+ceiling hazards, no on-foot floor] → corridor exit trigger → safe ground → ExitDoor
                              ↑ C4 (in jump range during active run)
```

| Marker | Draft X | Notes |
|--------|---------|-------|
| Corridor span | ~15 – 17 | No continuous walkable `Ground` through this span |
| `PlatformCorridorExit` | 17+ | Move right if the corridor is wider |
| `ExitDoor` | ~18+ | On safe ground **after** the corridor |
| `ShuttleSpawnPoint` | -18 | Unchanged |

### A. Spike corridor (blockout)

1. Create empty parent `CorridorHazards` (optional, for organisation)
2. **Remove or gap** walkable ground under the corridor — the player cannot run through on foot
3. **Floor spikes / kill:**
   - `KillZone_CorridorFloor` — **Box Collider 2D**, **Is Trigger**, **`KillZone`**
   - Cover the corridor floor band (e.g. `y` near `-9`, width ≈ corridor length)
   - Red spike sprites optional (**Layer** `Hazard`, no script)
4. **Ceiling spikes / kill:**
   - `KillZone_CorridorCeiling` — same pattern along the ceiling band (e.g. `y` near `4–5`)
   - Punishes trying to walk the corridor on the ceiling route without the platform
5. **Side bounds:** keep the corridor wide enough for the shuttle sprite + player on top or bottom

Touching either kill volume uses the same **Death Clip** as other hazards.

### B. Collectable 4

1. `2D Object > Sprites > Square` → **`Collectable4`**
2. Place **inside the corridor** at **jump height** relative to the underside route (tune X/Y in Scene view with Play Mode)
3. Visible at level start; distinct colour (e.g. purple)
4. **Box Collider 2D** + **`Collectible`**
5. Enable **`Require Active Shuttle Run`** on `Collectible` (C4 only — C1/C3 leave this off). This blocks pickup on foot when no run is active; it does **not** require staying on the platform collider when the jump connects.
6. **Audio:** unified **Collect Clip** on `AudioManager` (no per-object clip)

**Intended pickup:**

1. After C3, board the shuttle for a new run.
2. As the platform approaches the corridor, **flip gravity** (Shift) so you are on the **underside** of the platform.
3. Enter the **spike corridor** with the platform still moving (floor/ceiling kill zones active).
4. **Jump** (Space) when C4 is within range — timing matters; you may leave the platform surface briefly to touch the collectable trigger.
5. Land safely or remount if needed; clear the corridor before despawn at `PlatformCorridorExit`.

### C. Corridor exit and safe dismount

1. Confirm **`PlatformCorridorExit`** trigger spans the corridor height at the **right end**; platform collider must enter it before despawn
2. Beyond the exit, add **`Ground_Exit`** (walkable strip) so the player can land after despawn/fall
3. **Boarding rule (spatial design):** safe ground to the **right** of the corridor should not let the player re-board in time for another corridor pass after missing C4 (per GAME_CONCEPT §11.9). Use gap, height step, or later a blocker — full end caps in [section 14](#14-walkway-end-caps-deferred-until-blockout-complete)

### D. Exit door

1. Place **`ExitDoor`** on `Ground_Exit` to the right of the corridor
2. Stays **locked** until HUD shows **`Keys 4/4`**
3. Touch door → **Door Unlock** + **Level Complete** clips (if assigned)

### E. Progress and win

- HUD at load: **`Keys 0/4`** with C1–C4 all active in the hierarchy
- Win requires at least one gravity flip elsewhere in the level (existing design rule)
- Full route: C1 → shuttle/C2 → C3 → shuttle/C4 in corridor → dismount → door

### Verify P4 (Play Mode)

- [ ] On-foot entry to the corridor triggers death (floor/ceiling kill zones)
- [ ] C4 **cannot** be collected on foot before a shuttle run (`Require Active Shuttle Run` on)
- [ ] C4 **can** be collected with a **timed jump** in the corridor during an active run (underside gravity, inside spike zone)
- [ ] Missing C4, then leaving the corridor on the right, cannot be fixed by waiting for the next shuttle run (design check)
- [ ] `PlatformCorridorExit` still ends the run; player on board falls; loop from the left works
- [ ] At `4/4`, exit unlocks and level completes
- [ ] `R` / kill zone reset still clears all four keys and shuttle state
- [ ] P1–P3 behaviour unchanged

After P4 blockout is stable, add walkway **end caps** ([section 14](#14-walkway-end-caps-deferred-until-blockout-complete)) and run [section 13](#13-verification-checklist) for the full level.

---

## 11. Camera follow 2D *(implemented — requires Unity binding)*

Linear `Level01` is wider than one screen. `CameraFollow2D` scrolls **horizontally only** so your placed ceiling and floor heights stay framed; the camera does not chase the player on Y by default.

### Add the component

1. Select **Main Camera** in `Hierarchy`
2. **Add Component** → `CameraFollow2D`
3. Assign **Target** → `Player` (or leave empty — finds `PlayerController2D` at runtime)
4. Suggested starting values:

| Field | Suggested | Notes |
|-------|-----------|-------|
| Lock Vertical Position | on | Keeps **Main Camera** Y where you placed it in the scene (e.g. `y = -3`) |
| Follow Y | off | Turn on only if you intentionally want the view to move up/down with the player |
| Use Relative Horizontal Follow | on | At spawn, camera stays at your authored X; it moves only as the player moves left/right |
| Offset | `0, 0` first | Try X `2` later if you want more view to the right |
| Follow X | on | |
| Smooth Follow | on | |
| Smooth Time | `0.12` | Lower = snappier; raise if motion feels laggy |
| Use Bounds | off until level limits are set | Optional clamp so the camera does not show empty space past the level |

### Optional bounds (after blockout is stable)

1. Enable **Use Bounds**
2. Set **Min Position** / **Max Position** to the world X/Y range you want the camera centre to stay within (account for **Offset** when tuning)
3. Typical for Level01: min X near the left spawn area, max X near the exit door

### Verify

- [ ] Play Mode: moving right keeps the player in view; you can see platforms and hazards ahead
- [ ] Ceiling and floor still appear at the same heights as before follow was added (Lock Vertical Position on)
- [ ] No jitter worse than before (if so, try Smooth Time `0.15`–`0.2` or disable Smooth Follow)
- [ ] Kill-zone respawn / `R` reset: camera catches up to spawn

---

## 12. Build settings *(pending — Week 2)*

1. File → Build Settings
2. Add `Level01` (and `MainMenu` if implemented) to Scenes In Build
3. Platform: PC, Mac & Linux Standalone (or as module requires)

Document build output path in README when first build is exported.

---

## 13. Verification checklist

Before marking Level01 “playable”, confirm:

- [ ] Move and jump feel smooth; no air jump
- [ ] Shift flips gravity; player lands on ceiling/ground
- [ ] Level cannot be completed without flipping
- [ ] HUD shows gravity state and progress *(bind canvas per section 5, then verify)*
- [ ] Death resets position, gravity, and progress
- [ ] No tunneling through platforms at normal play speed

For the HUD milestone, verify:

- [ ] Progress shows `Keys 0/1` at start (or `0/0` briefly until collectables register, then `0/1`)
- [ ] Progress becomes `Keys 1/1` after collecting
- [ ] Gravity text shows `Gravity: Down` at start and after respawn
- [ ] Gravity text shows `Gravity: Up` after flipping
- [ ] Control hints stay visible at the bottom

For the current level-loop milestone, verify:

- [ ] Collectable disappears when touched by the player
- [ ] Exit remains locked before collecting all collectables
- [ ] Exit completes the level after all collectables are collected
- [ ] Kill zone respawns the player at `SpawnPoint`
- [ ] Kill zone resets gravity to normal
- [ ] Kill zone restores collectable progress

Log results in [Assets/Documentation/TESTLOG.md](Assets/Documentation/TESTLOG.md).

---

## 14. Walkway end caps *(deferred until blockout complete)*

**When:** After P3–P4 layout is in place and `Ground` / `Ceiling` final length is set (spawn through exit door).

**Why:** Finite ground/ceiling sprites let the player walk off the ends and fall out of the play space. Invisible vertical colliders at the **left and right edges** of the walkable strip block that without code changes.

**Not using for now:** Camera X bounds only move the view; kill zones on the sides punish with respawn instead of blocking.

### Place left and right caps

1. `Create Empty` → `LevelBounds_Left` and `LevelBounds_Right`
2. Each object: **Box Collider 2D** (not trigger), **Layer** = `Ground` (same as floor/ceiling walkables for player collision)
3. No sprite required (or disable `SpriteRenderer`)
4. Position each wall on the **outer edge** of the finished ground/ceiling span:
   - For a platform with centre `x`, scale `S`: left edge ≈ `x - S/2`, right edge ≈ `x + S/2`
   - Example (current placeholder): centre `0`, scale `38` → edges near `x = ±19`
5. Collider size: thin in X (e.g. `0.5`), tall in Y (e.g. `16`) so both floor and ceiling routes hit the wall when inverted
6. Nudge X slightly outside the platform edge (e.g. `±19.5`) so the player cannot slip past

### Verify (after blockout)

- [ ] Walking into the left/right end on **ground** stops movement; no fall off-screen
- [ ] Same on **ceiling** with inverted gravity
- [ ] Jump puzzles, pits, and shuttle platform path still reachable
- [ ] Left cap is not inside the spawn safe zone in a way that traps the player
