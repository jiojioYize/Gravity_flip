# Gravity Flip

A 2D puzzle platformer where you flip your character's gravity — not the world. Reach collectables and the exit by walking on floors *and* ceilings.

**Full design document:** [docs/GAME_CONCEPT.md](docs/GAME_CONCEPT.md)

---

## Development status

| Item | Status |
|------|--------|
| Unity project skeleton | Done |
| Game scripts | In progress — movement, gravity, level loop, HUD, P1 shuttle + P2 C2 (verified 2026-06-01), `CameraFollow2D` |
| HUD (in-game) | Done — progress, gravity direction, control hints |
| Audio and polish | Done — SFX, flip flash, `R` reset (Kenney CC0 clips in Unity) |
| Level 01 scene | In progress — C1 + P1 shuttle + P2 C2 verified; P3–P4 layout pending ([GAME_CONCEPT.md](docs/GAME_CONCEPT.md#11-level01--detailed-level-design-approved), [SETUP.md](SETUP.md#10-collectable-2-on-shuttle-platform--p2-implemented--requires-unity-binding)) |
| Playable build | Planned |

---

## Controls (planned)

| Action | Key |
|--------|-----|
| Move left / right | `A` / `D` or arrow keys |
| Jump | `Space` |
| Flip gravity | `Left Shift` |
| Pause | `Esc` *(stretch)* |
| Reset level | `R` |

Control hints will also appear in-game on the HUD.

---

## Requirements

- **Unity 2022.3 LTS** (project uses `2022.3.62f3`)
- Unity Hub with Personal license

Do **not** open this project in Unity 6 or newer.

---

## How to run

> Updated when Level01 exists.

1. Clone this repository.
2. Open the project folder in Unity Hub.
3. Open scene: `Assets/Scenes/Level01.unity` *(not created yet)*.
4. Press **Play**.

---

## Project structure (planned)

```
Assets/
  Scenes/           Level01, MainMenu
  Scripts/          Core, Player, Level, UI, Audio
  Documentation/    TESTLOG.md
docs/
  GAME_CONCEPT.md   Design and scope
SETUP.md            Unity Editor setup steps (updated during development)
```

---

## Testing log

Playtest notes: [Assets/Documentation/TESTLOG.md](Assets/Documentation/TESTLOG.md)

---

## Credits

**Audio:** Sound effects from [Kenney](https://kenney.nl/) (CC0): RPG Audio, Music Jingles, Digital Audio. Per-file list: [docs/AUDIO_SOURCING.md](docs/AUDIO_SOURCING.md).

**Sprites:** Placeholder shapes in `Level01`; Kenney 2D art planned for a later art pass (see [docs/GAME_CONCEPT.md](docs/GAME_CONCEPT.md)).

---

## License

Coursework project — see module guidelines for reuse and submission terms.
