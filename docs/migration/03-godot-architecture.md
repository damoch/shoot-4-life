# 03 — Proposed Godot 4.x architecture

## Design principles

The architecture should be explicit, testable, and small enough to understand while the game rules are still changing.

- Use GDScript by default. Introduce C# only if a measured bottleneck or existing team constraint justifies it.
- Prefer scenes for composition, scripts for behavior, and Resource files for tunable data.
- Use signals for lifecycle and mission events.
- Keep player control, AI policy, combat resolution, mission rules, and UI separate.
- Use explicit references or dependency injection through scene setup; do not create a global event bus by default.
- Use seeded random streams for generation and gameplay randomness.
- Keep the first Godot scene fixed and hand-authored; add procedural generation only after the combat contract is stable.

## Suggested project layout

Recommended folders:

- scenes/bootstrap — startup and application shell.
- scenes/menus — main menu, options, pause, result screens.
- scenes/operation — operation root, hospital, mission, extraction.
- scenes/actors — mercenary, zombie, survivor, shared actor components.
- scenes/combat — projectile, hit effect, weapon view.
- scenes/world — room, doors, walls, objective sites, templates.
- scripts/core — run coordination, settings, IDs, signals, utilities.
- scripts/actors — actor lifecycle, movement, selection, squad commands.
- scripts/combat — damage, weapons, projectiles, hit resolution.
- scripts/ai — perception, blackboards, state machines, policies.
- scripts/mission — objectives, charges, extraction, scoring.
- scripts/generation — room graphs, templates, validators, seed streams.
- scripts/ui — HUD, menus, mission status, result display.
- data/actors, data/weapons, data/missions, data/difficulty — Resource assets.
- tests — headless unit and integration tests.
- art, audio, vfx — imported and authored content.

## Scene ownership

### bootstrap.tscn

Owns application startup, SettingsStore access, scene transitions, and the initial menu. It should not own combat or mission rules.

### operation.tscn

Owns the current run. Suggested children are RunCoordinator, MissionCoordinator, SquadController, HospitalRoot, Actors, Projectiles, Objectives, CameraRig, and HUD. The operation scene receives a RunConfig and a seed, then creates or loads a layout.

### actor scenes

Use a common Actor scene contract with composition:

- CharacterBody2D movement root.
- CollisionShape2D.
- Visual/animation node.
- Hitbox and hurtbox areas with explicit collision layers.
- Combatant component.
- Health component.
- Selection marker where applicable.
- Perception sensor for AI actors.

Mercenary, zombie, and survivor scenes should share components without sharing player/AI ownership flags. Control policy belongs outside the combatant.

### weapon and projectile scenes

Weapon is a component or child owned by an actor. WeaponData is a Resource containing damage, cooldown, magazine, reload, spread/recoil, range, ammunition type, and friendly-fire policy. Projectile is an Area2D with an explicit damage packet, owner ID, team, penetration count, speed, and lifetime.

Melee should be an attack window or shape query rather than a spawned projectile unless a specific animation requires a persistent hitbox.

### room and objective scenes

Room is a spatial template with spawn markers, doors, navigation boundaries, objective sockets, and connectivity metadata. It should not contain global mission logic. Charge sites, extraction, and civilians are placed by MissionCoordinator from generated layout data.

## Core runtime objects

### RunCoordinator

Owns RunConfig, the seed, operation phase, scene-level dependencies, and run lifecycle. It provides the narrow root API for start, finish, retry, and result handoff.

### SquadController

Owns the four mercenary members, current selection, formation slots, shared command, and selection changes. It exposes commands such as move, aim, fire, hold, swap, and regroup. It does not own health or weapon implementation.

Recommended squad approach: shared direct command plus formation slots. Each selected mercenary receives the same intention but resolves an individual destination from its slot. This keeps the current “selected actors aim and fire together” feel while scaling to four actors. A leader/follower queue is a possible later mode, but it is less suitable for immediate tactical control.

### Combatant and Health

Combatant accepts a DamagePacket with source ID, team, amount, damage type, armor interaction, and hit context. Health resolves damage and emits damage_applied and died exactly once. ScoreLedger and AI listeners consume events instead of querying private fields.

### AgentBrain and PerceptionSensor

PerceptionSensor collects visible or audible candidates using areas, raycasts, or an explicit query service. AgentBrain owns a small state machine and a blackboard. Zombies and survivors provide different policy Resources to the same reusable behavior framework.

Suggested states: Idle, Hold, Investigate, Pursue, Attack, Flee, Panic, Cooperate, Hostile, Dead.

### MissionCoordinator

Owns objective phases, the first-charge event, countdown, extraction eligibility, civilian reactions, and run completion. It is the only authority for mission progression.

### ScoreLedger

Subscribes to stable gameplay signals and records immutable score events. It should calculate score at run end from events or from a documented accumulator; it should not inspect scene nodes after they are freed.

### HospitalGenerator

Consumes a RunConfig and seed, then chooses a room graph, templates, spawn points, objectives, and extraction. It validates connectivity, navigation availability, spawn separation, objective reachability, and a bounded difficulty budget before committing the layout.

### CameraRig

Follows active squad members and removes them through signals. It should handle dead members and no-target cases without relying on freed-node references.

## Signals

Minimum useful signals:

- Actor: health_changed, damage_applied, actor_died, team_changed.
- Combat: shot_fired, reload_started, reload_finished, projectile_hit.
- Squad: selection_changed, command_issued, formation_changed.
- Mission: objective_changed, first_charge_planted, countdown_started, extraction_state_changed, run_finished.
- Generation: layout_generation_started, layout_validated, layout_failed.

Signals are for facts that happened. Commands should be method calls or typed command objects. Do not use signals as an untyped command bus.

## Collision and physics contract

Use a documented collision-layer table. A starting contract might be:

| Layer | Purpose |
| --- | --- |
| 1 | World walls and static obstacles |
| 2 | Actor bodies |
| 3 | Hurtboxes |
| 4 | Projectiles |
| 5 | Perception sensors |
| 6 | Objective/extraction areas |

CharacterBody2D should own actor movement. StaticBody2D should own walls. Area2D should own sensors, objective sites, extraction, and projectile hit detection. Collision queries should return typed nodes or components, not depend on a parent chain.

Friendly fire must be decided centrally by CombatResolver or WeaponData. The current prototype has no team check, so the Godot implementation should not inherit that ambiguity.

## AI architecture

Use a hybrid finite-state machine plus shared perception/blackboard:

1. Perception updates candidate facts.
2. Policy Resource selects priorities.
3. AgentBrain changes state.
4. State produces an intention such as move, attack, flee, warn, cooperate, or investigate.
5. Actor movement/combat components execute the intention.

Zombie policy can prioritize nearby enemies, noise, and line of sight. Survivor policy can vary by profile: ignore, warn, cooperate, flee, hide, or become hostile. The first-charge signal can change the policy input for all nearby civilians without directly mutating every civilian from the charge script.

Use NavigationAgent2D only where pathfinding is needed. Rooms should provide navigable bounds and door connectivity. Do not require a global navigation mesh for every actor if a bounded room graph and local avoidance are sufficient.

## Procedural generation and determinism

Generation should be graph-first:

1. Create a seeded room graph.
2. Select room templates and door orientations.
3. Place start, objectives, and extraction.
4. Check connectivity and route budgets.
5. Place actors, civilians, and threats from separate deterministic substreams.
6. Build navigation/collision.
7. Validate reachability, spawn safety, and mission rules.
8. Retry with a bounded attempt count, then use a documented fallback.

Use a stable seed, stable sorted IDs, and named random streams for layout, combat spawns, civilians, and cosmetic variation. Avoid global random calls and iteration order over unordered collections when output affects gameplay. Determinism should mean the same seed and rules produce the same run outcome within the documented simulation scope; it need not promise frame-perfect replay in the first version.

Fixed speedrun mode should use the same room and mission contracts through a FixedLayoutProvider. It should lock layout, spawn tables, and relevant modifiers rather than introduce a separate code path.

## Autoload policy

Start with no global gameplay event bus. Acceptable autoload candidates are:

- SettingsStore for user preferences.
- An optional RunSession only if scene transitions require persistent run results.

RunCoordinator, SquadController, MissionCoordinator, ScoreLedger, and HospitalGenerator should belong to the operation scene. This keeps tests and restart behavior local.

