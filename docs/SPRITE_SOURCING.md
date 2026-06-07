# Gravity Flip — Sprite Sourcing

This document records where Level01 gameplay sprites come from, how they map to scene objects, and the license terms. It supports the module requirements for legal/ethical use of assets and README credits.

**Gameplay sprites in `Assets/Tiles/` are from Kenney *1-Bit Platformer Pack* (CC0).** Background, HUD panels, and runtime-built menu/pause/win UI still use placeholders until swapped manually.

---

## Pack used

| Pack | Author | License | URL |
|------|--------|---------|-----|
| Kenney *1-Bit Platformer Pack* | Kenney | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/1-bit-platformer-pack |

Kenney assets use **CC0**: attribution is not legally required; this project credits Kenney in the README and lists mappings here for transparency.

---

## Original tile → project file → Level01 use

Imported PNGs are stored under `Assets/Tiles/` with readable names. Each file was taken from the pack using the tile filename shown in the pack (e.g. `tile_0241.png`).

| Kenney tile (source) | Project file (`Assets/Tiles/`) | Level01 object(s) |
|----------------------|--------------------------------|-------------------|
| `tile_0241` | `Player.png` | `Player` |
| `tile_0116` | `Ceiling_and_Floor.png` | `Ceiling`, `Ground_C3_Left`, `Ground_C3_Right`, and other walkable ground/ceiling platforms |
| `tile_0009` | `ShuttlePlatform.png` | `ShuttlePlatform` (visual on platform root or child) |
| `tile_0096` | `Keys.png` | `Collectable`, `Collectable2`, `Collectable3`, `Square` (C4) |
| `tile_0056` | `door_closed.png` | `ExitDoor` — **Locked Sprite** on `ExitDoor` script |
| `tile_0058` | `door_open.png` | `ExitDoor` — **Open Sprite** on `ExitDoor` script |
| `tile_0033` | `KillZone.png` | `KillZone` (level-bottom hazard) |
| `tile_0105` | `KillZone_C3Pit.png` | `KillZone_C3Pit` |
| `tile_0166` | `KillZone_Corridor.png` | `KillZone_CorridorFloor`, `KillZone_CorridorCeiling` |

### Hazard visuals

Kill zones keep **Box Collider 2D → Is Trigger** and the `KillZone` script. Long hazard strips may use **Sprite Renderer → Draw Mode: Tiled** so one spike tile repeats across the volume.

### Collectables

All four keys share `Keys.png`; distinguish them in the Editor with **Rotation Z** or slight **Position** offsets if desired.

### Exit door

When **Locked Sprite** and **Open Sprite** are both assigned on `ExitDoor`, the script swaps art at `Keys 4/4` instead of using red/green colour tint.

---

## Not yet swapped (placeholders)

| Area | Current state | Notes |
|------|---------------|-------|
| `LevelBackdrop` | Editor placeholder / colour | Optional pack background or solid tint |
| HUD (`Canvas` panels) | Text + optional panel colour | See `SETUP.md` §18; Kenney UI Pack optional |
| Main menu / pause / win UI | Runtime-built in `MainMenuController`, `OverlayUiBuilder` | Code-driven; swap requires script or sprite hooks |

---

## Import settings (project convention)

- **Texture Type:** Sprite (2D and UI)
- **Pixels Per Unit:** unified per level (tune once on `Player`, then match other tiles)
- **Filter Mode:** Point (fits 1-bit pixel look) or Bilinear (softer)

Swap sprites per object in the Inspector on `Level01` — do not use scene-wide bulk automation (see `TECHNICAL_DECISIONS.md`, 2026-05-26 art incident).

---

## If you change imports

Update this table when tile choices or filenames change. Keep README **Credits** in sync.
