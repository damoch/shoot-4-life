# 05 — Migration strategy

## Options

### A. Direct port

Recreate the current Unity hierarchy and behavior one-to-one in Godot. This is initially familiar and can preserve visible behavior quickly, but it carries over brittle parent lookups, mixed movement semantics, hidden selection state, room-trigger coupling, and missing lifecycle events. It also risks spending time porting unfinished systems.

### B. Clean rewrite

Design the Godot game from first principles. This gives the cleanest architecture, but it discards useful tuning and behavior knowledge and creates a large design gap before a playable result exists.

### C. Hybrid rebuild — recommended

Preserve assets, tuning, naming, and behavior concepts. Rebuild engine-dependent code and all missing game systems around explicit Godot scene contracts, Resources, signals, seeded generation, and a vertical slice.

This option is the best fit because the current prototype is valuable as a behavior reference but is not a complete version of the intended game.

## What the hybrid approach preserves

- Sprite and UI art where licensing/import checks pass.
- Character, weapon, ammunition, armor, wall, and room concepts.
- Health, speed, damage, magazine, reload, and recoil tuning as initial defaults.
- Shared selected-squad aim/fire feel.
- Basic zombie target and attack behavior.
- Static test room geometry as a first migration map.

## What it deliberately replaces

- Unity actor god-object design.
- Legacy Input Manager and string-based numeric selection.
- Rigidbody body type as the meaning of selection.
- Parent-chain collision/room lookups.
- Room trigger notifications as the AI architecture.
- Direct projectile instantiation without ownership/cleanup policy.
- Unity NavMesh package assumptions.
- Placeholder save/load and turn-number code.
- PlayerPrefs type mismatch and scene-name-only mission start.

## Migration order

1. Establish the Godot project shell, input actions, data folders, test conventions, and a fixed operation scene.
2. Build a four-mercenary combat vertical slice in a small hand-authored room.
3. Add squad formations, selection feedback, reload/swap feedback, camera cleanup, and basic tactical commands.
4. Add survivors with policy resources and event-driven reactions.
5. Add charges, first-charge event, countdown, extraction, and mission completion.
6. Add seeded procedural hospital generation with validation and a fixed-layout provider.
7. Add score, payout, difficulty, and speedrun rules.
8. Add audio, VFX, content breadth, menus, result presentation, and release checks.

## Porting rules

- Port behavior and values, not object hierarchy.
- Port one system at a time behind an explicit Godot contract.
- Keep the Unity prototype under UnityLegacy/ as a read-only reference until Godot behavior is accepted.
- Use one source of truth for mission state, damage, score, and seed.
- Keep new placeholder content replaceable.
- Do not delete or mass-convert Unity assets during the analysis phase.
- Do not introduce persistence until the run-result model is stable.

## Verification gates

Each migration step should have:

- A small automated test for rules or data.
- A headless boot or scene-load check where practical.
- A manual playable check for feel and readability.
- A diff review focused on scope and ownership.
- A short update to the relevant migration document when a design decision changes.

## Exit criteria for starting implementation

Implementation can begin after approval when the team agrees on:

- Four-mercenary control model.
- Friendly-fire policy.
- Survivor reaction profiles.
- Charge, countdown, extraction, score, and payout rules.
- Seed/determinism scope.
- Whether Godot GDScript is the default implementation language.
- The first vertical-slice acceptance criteria in 06-implementation-roadmap.md.

