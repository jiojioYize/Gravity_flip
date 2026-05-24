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

## 2. Folder layout *(pending)*

In `Assets/` create:

```
Scenes/
Scripts/Core/
Scripts/Player/
Scripts/Level/
Scripts/UI/
Scripts/Audio/
Prefabs/
Sprites/
Audio/
```

Scripts will be added by development tasks; create folders when first script is added.

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

## 4. Player prefab *(pending — Week 1 D1–3)*

1. Create empty `Player` GameObject
2. Add `SpriteRenderer` (placeholder square sprite)
3. Add `Rigidbody2D`: Dynamic, Freeze Rotation Z, Continuous collision
4. Add `CapsuleCollider2D` or `BoxCollider2D`
5. Add `PlayerController2D` script *(when created)*
6. Wire references in Inspector per script tooltips
7. Drag to `Assets/Prefabs/Player.prefab`

---

## 5. UI canvas *(pending — Week 1 D5)*

1. GameObject → UI → Canvas (Screen Space Overlay)
2. Add Text/TMP elements:
   - Progress: `Keys 0/1` (top-left)
   - Gravity indicator (top-right)
   - Control hints (bottom)
3. Add `GameplayHUD` script *(when created)* and assign UI references

---

## 6. Managers *(pending)*

Empty GameObject `--- Managers ---` in scene with:

- `GameManager`
- `GravityController`
- `ProgressManager`
- `AudioManager` *(when audio added)*

Assign scene references (spawn point, door, HUD) in Inspector.

---

## 7. Build settings *(pending — Week 2)*

1. File → Build Settings
2. Add `Level01` (and `MainMenu` if implemented) to Scenes In Build
3. Platform: PC, Mac & Linux Standalone (or as module requires)

Document build output path in README when first build is exported.

---

## 8. Verification checklist

Before marking Level01 “playable”, confirm:

- [ ] Move and jump feel smooth; no air jump
- [ ] Shift flips gravity; player lands on ceiling/ground
- [ ] Level cannot be completed without flipping
- [ ] HUD shows gravity state and progress
- [ ] Death resets position, gravity, and progress
- [ ] No tunneling through platforms at normal play speed

Log results in [Assets/Documentation/TESTLOG.md](Assets/Documentation/TESTLOG.md).
