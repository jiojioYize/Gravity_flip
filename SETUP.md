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
Scripts/Core/     `GravityController`
Scripts/Player/   `PlayerController2D`
Scripts/Level/    `Collectible`, `ExitDoor`, `KillZone`
Scripts/UI/       *(pending)*
Scripts/Audio/    *(pending)*
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

- Main Camera: Orthographic, size ~5–6, background colour of choice

### Level geometry

Build with 2D sprites + BoxCollider2D (or Tilemap):

| Object | Notes |
|--------|-------|
| Ground | Bottom platform spanning level width |
| Ceiling | Top platform (walkable when gravity inverted) |
| Side walls | Optional; prevent falling off horizontally |
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
9. Drag to `Assets/Prefabs/Player.prefab`

---

## 5. UI canvas *(pending — Week 1 D5)*

1. GameObject → UI → Canvas (Screen Space Overlay)
2. Add Text/TMP elements:
   - Progress: `Keys 0/1` (top-left)
   - Gravity indicator (top-right)
   - Control hints (bottom)
3. Add `GameplayHUD` script *(when created)* and assign UI references

---

## 6. Managers *(core scripts ready — requires Unity binding)*

Empty GameObject `--- Managers ---` in scene with:

- `GravityController`
- `GameManager`
- `ProgressManager`
- `AudioManager` *(when audio added)*

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

The `Collectible` script sets its collider to trigger at runtime.

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

## 8. Build settings *(pending — Week 2)*

1. File → Build Settings
2. Add `Level01` (and `MainMenu` if implemented) to Scenes In Build
3. Platform: PC, Mac & Linux Standalone (or as module requires)

Document build output path in README when first build is exported.

---

## 9. Verification checklist

Before marking Level01 “playable”, confirm:

- [ ] Move and jump feel smooth; no air jump
- [ ] Shift flips gravity; player lands on ceiling/ground
- [ ] Level cannot be completed without flipping
- [ ] HUD shows gravity state and progress
- [ ] Death resets position, gravity, and progress
- [ ] No tunneling through platforms at normal play speed

For the current level-loop milestone, verify:

- [ ] Collectable disappears when touched by the player
- [ ] Exit remains locked before collecting all collectables
- [ ] Exit completes the level after all collectables are collected
- [ ] Kill zone respawns the player at `SpawnPoint`
- [ ] Kill zone resets gravity to normal
- [ ] Kill zone restores collectable progress

Log results in [Assets/Documentation/TESTLOG.md](Assets/Documentation/TESTLOG.md).
