# Shoot 4 Your Life — Unity to Godot Migration Package

This package is an analysis-only audit and migration plan for the current prototype. It does not modify, refactor, migrate, or delete the existing Unity project.

## Executive summary

The repository is a Unity 2022.3.62f2 prototype with a small but usable combat scaffold. It contains playable actors, zombie actors, weapons, projectile and melee ammunition, armor, room-trigger bookkeeping, camera following, pause/menu UI, and a minimal test suite.

The current implementation is not yet the intended four-person tactical speedrun game. The build scenes wire two playable actors and four zombies into a static room. Survivor civilians, explosive charges, countdown/extraction rules, scoring, payout, procedural hospital generation, fixed speedrun mode, and the broader mission state machine are absent.

The recommended migration is a hybrid rebuild: preserve art, tuning, names, and proven gameplay concepts, but rebuild engine-dependent gameplay in Godot 4.x with explicit composition, signals, data resources, seeded generation, and a vertical-slice-first workflow.

## Repository layout after cleanup

- UnityLegacy/ — the complete existing Unity project, including Assets, Packages, ProjectSettings, scenes, prefabs, source, and generated Unity workspace data.
- GodotModern/shoot-4-your-life/ — the new Godot 4.7 project, currently containing the default project configuration and icon.
- docs/migration/ — this audit and migration plan.
- .git/ — repository metadata retained at the repository root.

The migration documents remain at the repository root as shared planning material for both engine directories. Unity paths cited in the audit should now be read as relative to UnityLegacy/, and Godot paths as relative to GodotModern/shoot-4-your-life/.

## Read this package in order

1. 01-project-audit.md — repository, engine, build, code, scene, and test evidence.
2. 02-gameplay-systems.md — current behavior versus the intended game systems.
3. 03-godot-architecture.md — proposed Godot scene, script, data, signal, and AI architecture.
4. 04-asset-inventory.md — asset classification, reuse guidance, and gaps.
5. 05-migration-strategy.md — options, recommendation, sequencing, and non-porting rules.
6. 06-implementation-roadmap.md — milestone plan with acceptance and validation criteria.
7. 07-ai-development-workflow.md — rules for safe AI-assisted implementation.

## Decision summary

### Preserve

- Existing character, zombie, weapon, ammunition, armor, wall, room, and button art.
- Tunable values such as health, speed, magazine size, cooldown, reload duration, damage, armor divisor, and recoil.
- The room concept, selectable actors, shared squad aiming, weapon swap, projectile/melee distinction, and the core zombie chase/attack behavior.
- The two playable test scenes as references for the first playable Godot slice.

### Rebuild

- Actor composition and lifecycle.
- Player input and selection.
- Weapon and projectile ownership.
- Damage, death, attribution, friendly-fire rules, and cleanup.
- AI perception, target selection, navigation, civilian reactions, and mission events.
- Room generation, objective/extraction flow, scoring, run modes, and deterministic seeds.
- Menu/settings persistence and save/load only after the core run is stable.

### Do not port directly

- Unity YAML scene/prefab serialization and GUID relationships.
- Unity package configuration, NavMesh assets, legacy Input Manager assumptions, and editor-only runtime code.
- The unfinished SaveLoadManager, duplicate GameController concepts, and the minimal turn-number implementation.
- Parent-chain lookups as gameplay contracts, hidden selection through Rigidbody body type, or random behavior without an explicit seed.

## Main migration risks

1. The prototype's two-player test wiring must become an intentional four-person squad model without duplicating input, camera, or combat logic.
2. Civilian reactions, the first-charge event, extraction, and scoring need one explicit mission/event model rather than more room-trigger side effects.
3. Procedural generation and fixed-layout speedrun mode must share validated templates while remaining reproducible from a seed.
4. The existing Unity runtime assembly contains editor-only references and has not been validated by a fresh headless build in this audit.

## First implementation task after approval

Create a Godot shell and one four-mercenary combat vertical slice: explicit selection, shared aim/fire, one zombie, damage/death cleanup, and a small fixed room. This gives the project a playable architecture boundary before procedural generation or mission complexity is introduced.

