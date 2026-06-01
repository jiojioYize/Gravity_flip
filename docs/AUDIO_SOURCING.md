# Gravity Flip — Audio Sourcing

This document records where gameplay sound effects come from, how they map to `AudioManager`, and the license terms. It supports the module requirements for legal/ethical use of assets and README credits.

**All current SFX are from [Kenney](https://kenney.nl/) (CC0).** Clip choices may change during polish; update this file when imports change.

---

## Do I need a source for every sound?

| Situation | What to document |
|-----------|------------------|
| **All clips from Kenney CC0 packs** | Each pack: name, license, URL. Per-file mapping (below) is recommended for the report. |
| **Clips from more than one author/site** | Each clip needs source and license. |
| **Edited or trimmed clips** | Note original filename and that the file was edited for this project. |

Kenney assets use **CC0**: attribution is not legally required; this project credits Kenney in the README and lists files here for transparency.

---

## Packs used

| Pack | Author | License | URL |
|------|--------|---------|-----|
| Kenney *RPG Audio* | Kenney | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | https://kenney.nl/assets/rpg-audio |
| Kenney *Music Jingles* | Kenney | CC0 | https://kenney.nl/assets/music-jingles |
| Kenney *Digital Audio* | Kenney | CC0 | https://kenney.nl/assets/digital-audio |

---

## In-project files → gameplay use

Files live under `Assets/Audio/`. Project filenames were chosen for clarity in Unity; they differ from the original download names in the tables below.

### RPG Audio

| Project file | `AudioManager` field | Gameplay use | Original file |
|--------------|----------------------|--------------|---------------|
| `Collect.ogg` | Collect Clip | Collectable pickup | `handleCoins.ogg` |
| `Jump.ogg` | Jump Clip | Player jump (grounded) | `dropLeather.ogg` |
| `doorOpen.ogg` | Door Unlock Clip | Exit door unlocks | `doorOpen_1.ogg` |

### Music Jingles

| Project file | `AudioManager` field | Gameplay use | Original file |
|--------------|----------------------|--------------|---------------|
| `Reset.ogg` | Level Reset Clip | `R` key level reset | `jingles_PIZZI00.ogg` |
| `Complete.ogg` | Level Complete Clip | Player reaches exit after unlock | `jingles_NES03.ogg` |
| `Death.ogg` | Death Clip | Kill zone respawn | `jingles_SAX07.ogg` |

### Digital Audio

| Project file | `AudioManager` field | Gameplay use | Original file |
|--------------|----------------------|--------------|---------------|
| `Flip.ogg` | Flip Clip | Gravity flip | `highUp.ogg` |

---

## README credit (short)

```text
Sound effects from Kenney (CC0): RPG Audio, Music Jingles, Digital Audio.
https://kenney.nl
```

---

## Import notes (Unity)

- Clips are assigned on `--- Managers ---` → `AudioManager` in `Level01`.
- Empty `AudioManager` slots are skipped at runtime (no error).
- When replacing a clip, update this document and Inspector assignments together.
- **Unified policy:** all `Collectible` objects share **Collect Clip**; all `KillZone` objects share **Death Clip** on `AudioManager`.

---

## Document history

| Date | Notes |
|------|-------|
| 2026-05-26 | Initial sourcing doc |
| 2026-05-26 | Filled Kenney pack list and per-file mapping (RPG Audio, Music Jingles, Digital Audio) |
