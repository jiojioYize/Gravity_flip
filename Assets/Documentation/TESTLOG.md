# Gravity Flip — Test Log

Record playtests here. The final report will reference this file for testing and iteration evidence.

**How to use:** Add a new row after each test session. Link to git commits when a fix is pushed.

---

## Template

| Date | Build / commit | What was tested | Issue | Fix | Retest |
|------|----------------|-----------------|-------|-----|--------|
| YYYY-MM-DD | e.g. `abc1234` or "Editor Play" | Brief scenario | What went wrong | What changed | Pass / Fail |

---

## Entries

| Date | Build / commit | What was tested | Issue | Fix | Retest |
|------|----------------|-----------------|-------|-----|--------|
| 2026-05-24 | Editor scripts only | Added `GravityController` and `PlayerController2D`; reviewed setup requirements | Not yet tested in Unity Play Mode because scene and prefab binding are not created | Documented required Inspector setup in `SETUP.md` | Pending Play Mode test |
| 2026-05-24 | Unity Editor Play Mode | Movement, grounded jump, no double jump, gravity flip to ceiling, ceiling movement, inverted jump, and flip back to normal gravity | No issues reported during first manual verification | No code change required | Pass |

---

## Known issues (open)

_None for the current movement and gravity-flip test scope._

---

## Resolved issues (summary)

_Use this section for quick reference when writing the report._

- Initial movement and gravity-flip implementation passed manual Unity Editor verification: left/right movement, grounded-only jump, gravity flip, ceiling movement, inverted jump, and return to normal gravity all worked as intended.
