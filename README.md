# Gravity Flip

A 2D puzzle platformer where you flip your character's gravity — not the world. Reach collectables and the exit by walking on floors *and* ceilings.

**Full design document:** [docs/GAME_CONCEPT.md](docs/GAME_CONCEPT.md)

---

## Development status

| Item | Status |
|------|--------|
| Unity project (2022.3 LTS) | Done |
| Core gameplay scripts | Done — gravity, movement, jump, level loop, hazards, `R` reset |
| Level01 vertical slice (P1–P4) | Done — C1→C4→door verified (2026-06-03). See [GAME_CONCEPT.md](docs/GAME_CONCEPT.md) §11.14 |
| HUD | Done — progress, gravity direction, control hints (screen-space overlay) |
| Audio and polish | Done — Kenney CC0 SFX, flip flash (`docs/AUDIO_SOURCING.md`) |
| Camera and framing | Done — `CameraFollow2D`, bounds, `LevelBackdrop2D` |
| Game flow UI (UX-1–3) | Done — `MainMenu`, Esc pause/instructions, win panel; quiet death (2026-06-04) |
| Level gameplay sprites | Done — Kenney *1-Bit Platformer Pack* in `Assets/Tiles/` (see [SPRITE_SOURCING.md](docs/SPRITE_SOURCING.md)) |
| Level backdrop / HUD / flow UI art | Placeholders — optional Kenney UI or background swap |
| Playable build export | Build Settings ready (`MainMenu` → `Level01`); standalone export when required for submission |

---

## Controls

| Action | Key |
|--------|-----|
| Move left / right | `A` / `D` or arrow keys |
| Jump | `Space` |
| Flip gravity | `Left Shift` |
| Pause | `Esc` |
| Reset level | `R` |

Control hints also appear on the in-game HUD.

---

## Requirements

- **Unity 2022.3 LTS** (project uses `2022.3.62f3`)
- Unity Hub with Personal license

Do **not** open this project in Unity 6 or newer.

---

## How to run

1. Clone this repository.
2. Open the project folder in Unity Hub and wait for scripts to compile.
3. **Recommended:** Open `Assets/Scenes/MainMenu.unity` and press **Play** → **Start** loads Level01.
4. **Quick test:** Open `Assets/Scenes/Level01.unity` directly and press **Play** (skips menu; pause/win overlays still work).

Build order is **MainMenu (0)** → **Level01 (1)** in File → Build Settings (preconfigured in repo).

---

## Project structure

```
Assets/
  Scenes/           MainMenu.unity, Level01.unity
  Scripts/          Core, Player, Level, UI, Audio
  Audio/            Kenney CC0 clips
  Documentation/    TESTLOG.md
docs/
  GAME_CONCEPT.md   Design and scope
  TECHNICAL_DECISIONS.md
  AUDIO_SOURCING.md
  SPRITE_SOURCING.md
SETUP.md            Unity Editor setup and verification checklists
```

---

## Evidence trail (assessment)

| What | Where |
|------|--------|
| Design | [docs/GAME_CONCEPT.md](docs/GAME_CONCEPT.md) |
| Technical choices | [docs/TECHNICAL_DECISIONS.md](docs/TECHNICAL_DECISIONS.md) |
| Playtests | [Assets/Documentation/TESTLOG.md](Assets/Documentation/TESTLOG.md) |
| Editor reproduction | [SETUP.md](SETUP.md) |
| Version history | Git commits on `main` |

---

## Testing log

Playtest notes: [Assets/Documentation/TESTLOG.md](Assets/Documentation/TESTLOG.md)

---

## Credits

**Audio:** Sound effects from [Kenney](https://kenney.nl/) (CC0): RPG Audio, Music Jingles, Digital Audio. Per-file list: [docs/AUDIO_SOURCING.md](docs/AUDIO_SOURCING.md).

**Sprites:** [Kenney *1-Bit Platformer Pack*](https://kenney.nl/assets/1-bit-platformer-pack) (CC0) — gameplay tiles in `Assets/Tiles/`; per-object mapping in [docs/SPRITE_SOURCING.md](docs/SPRITE_SOURCING.md). Backdrop, HUD, and runtime menu UI still placeholders.

---

## License

Coursework project — see module guidelines for reuse and submission terms.
