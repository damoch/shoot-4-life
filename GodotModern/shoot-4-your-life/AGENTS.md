# Shoot 4 Your Life — Godot project rules

- This folder is the Godot 4.x migration target. `UnityLegacy/` is read-only reference material.
- Keep scenes explicit about ownership. Prefer scene references, typed variables, signals, and Resources over global state, autoloads, or brittle parent lookups.
- Keep gameplay changes incremental and reviewable. Do not start procedural generation, survivor AI, scoring, or save/load until the M1 vertical slice is stable.
- Use seeded randomness when randomness is introduced. Do not add randomness to the M0/M1 slice unless a test controls the seed.
- Keep core combat rules testable without rendering. Add focused headless tests under `tests/` for rule changes.
- Keep UI copy clean and minimal. Do not add descriptive suffixes, tutorial text, or gameplay advice to UI element labels unless explicitly requested; a health bar should be labeled only `Health bar` when a label is needed.
- Use the user's computer only after the user explicitly prompts an agent to run a test; otherwise use repository and terminal checks.
- Preserve the Unity project and avoid mass conversion or edits outside this Godot project and migration documentation.
