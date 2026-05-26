# Gravity Flip

A 2D puzzle platformer where you flip your character's gravity — not the world. Reach collectables and the exit by walking on floors *and* ceilings.

**Full design document:** [docs/GAME_CONCEPT.md](docs/GAME_CONCEPT.md)

---

## Development status

| Item | Status |
|------|--------|
| Unity project skeleton | Done |
| Game scripts | Started — player movement, gravity, collectable, exit, and reset foundation |
| Level 01 scene | Started — first movement test scene exists |
| Playable build | Planned |

---

## Controls (planned)

| Action | Key |
|--------|-----|
| Move left / right | `A` / `D` or arrow keys |
| Jump | `Space` |
| Flip gravity | `Left Shift` |
| Pause | `Esc` *(stretch)* |
| Reset level | `R` *(stretch)* |

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

External assets (sprites, audio) will be listed here with license links before final submission.

**Placeholder:** royalty-free assets from [Kenney.nl](https://kenney.nl/) (CC0) are planned; credits will be updated when assets are imported.

---

## License

Coursework project — see module guidelines for reuse and submission terms.
