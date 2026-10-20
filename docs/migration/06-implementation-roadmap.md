# 06 — Implementation roadmap

This roadmap is intentionally milestone-based rather than calendar-based. Each milestone should be completed and reviewed before the next system expands the scope.

## M0 — Godot shell and conventions

### Objective

Create a bootable Godot 4.x project with a fixed operation scene, input actions, folder structure, test conventions, and project-level agent rules.

### Tasks

- Create project.godot and bootstrap/main-menu/operation scenes.
- Add InputMap actions for movement, aim, fire, selection, swap, hold, pause, and interact.
- Add SettingsStore only if settings are needed by the shell.
- Add the first AGENTS.md with scope, testing, scene ownership, and determinism rules.
- Create placeholder HUD and a fixed room with walls and spawn markers.

### Dependencies

None beyond Godot 4.x and approved asset import.

### Deliverables

Bootable Godot project, operation scene, input map, initial test harness, and documented scene ownership.

### Acceptance

The project starts headlessly, opens the main menu, enters the operation scene, responds to mapped input, and returns without errors.

### Automated validation

- Headless project boot.
- InputMap action existence test.
- Scene instantiation test.
- No unexpected autoloads or missing resource references.

### Manual validation

Open the operation scene, move the camera/player placeholder, pause/resume, and inspect the scene tree.

### Risks

Overbuilding the shell before a playable behavior exists. Keep all M0 systems replaceable.

## M1 — Four-person combat vertical slice

### Objective

Prove the central game feel with four mercenaries, explicit selection, shared aim/fire, one zombie, damage, death, cleanup, and a small fixed room.

### Tasks

- Build mercenary and zombie scenes with explicit collision layers.
- Create Actor/Health/Combatant components.
- Implement SquadController with four members and shared direct commands.
- Implement selection, formation slots, aim, fire, reload, and weapon swap.
- Implement projectile and melee damage packets with source/team attribution.
- Decide and implement friendly-fire behavior.
- Emit actor_died and remove dead actors from squad/camera/AI collections.
- Import the minimum character, wall, bullet, and room art.

### Dependencies

M0 and the four-person control decision.

### Deliverables

A playable combat scene that can be restarted repeatedly without stale references.

### Acceptance

Four mercenaries can be selected, moved, aimed, and fired as a group. One zombie can pursue and attack. Damage, armor, reload, death, and cleanup are visible and deterministic enough for repeated manual tests.

### Automated validation

- Damage and armor calculation.
- Magazine/reload behavior.
- Projectile ownership and friendly-fire rule.
- Death emitted exactly once.
- Squad and camera collections remove dead members.
- Four-member command fan-out.

### Manual validation

Run the slice with keyboard/mouse, inspect selection feedback, hit a zombie, allow a mercenary to die, restart, and repeat. Confirm no freed-node errors.

### Risks

The existing prototype mixes Transform.Translate and Rigidbody movement and uses parent-chain collision lookup. Recreating those patterns will hide bugs; keep movement and collision contracts explicit.

## M2 — Squad tactics and combat feedback

### Objective

Make the four-person squad readable and tactically controllable without expanding mission rules.

### Tasks

- Add formation slots and collision-aware destination resolution.
- Add hold/regroup/basic auto-engage toggles.
- Add selection rings, health, ammo, reload, and weapon indicators.
- Add camera framing, dead-member handling, and no-target behavior.
- Add muzzle, hit, damage, reload, and death placeholder feedback.
- Add basic obstacle/navigation behavior in the fixed room.

### Dependencies

M1.

### Deliverables

Readable tactical movement and combat feedback in the same vertical-slice room.

### Acceptance

The player can tell who is selected, who is firing/reloading, who is dead, and where the squad will move. The camera remains valid as members die or leave the room.

### Automated validation

- Formation slot assignment.
- Destination validity against walls.
- Camera target removal.
- Reload and weapon swap state transitions.

### Manual validation

Stress movement around walls, simultaneous firing/reload, a member death, and rapid selection changes.

### Risks

Formation logic can become an implicit second AI system. Keep it as a command-to-destination adapter, not a new behavior framework.

## M3 — Survivor civilians and reactive AI

### Objective

Introduce civilian actors with reusable reaction profiles and event-driven responses.

### Tasks

- Add survivor scene using shared health/movement/perception components.
- Create SurvivorPolicy Resources: ignore, warn, cooperate, flee, hide, hostile.
- Add perception of squad proximity, gunfire, damage, zombies, and mission events.
- Add civilian state transitions and safe fallback behavior.
- Emit and consume the first-charge event without direct cross-object mutation.

### Dependencies

M1 combat events and an agreed civilian rule set.

### Deliverables

A test room containing multiple survivor profiles that react differently to the squad and combat.

### Acceptance

Survivors respond according to policy, do not require player-only code paths, and survive/recover/reach safe states according to documented rules.

### Automated validation

- Policy-to-state mapping.
- Event reaction to first_charge_planted.
- Perception filtering by team and range.
- Civilian death/evacuation accounting.

### Manual validation

Trigger gunfire and the first charge near each policy profile. Confirm behavior is legible and does not freeze or oscillate indefinitely.

### Risks

Civilian behaviors can become an unbounded design surface. Keep profiles data-driven and ship only the reactions needed for the first mission.

## M4 — Mission objectives, charges, countdown, extraction

### Objective

Turn combat into a complete run with explicit mission progression.

### Tasks

- Add objective sites and charge planting interaction.
- Add MissionCoordinator phases and objective state.
- Emit first_charge_planted exactly once per relevant mission.
- Add countdown and HUD.
- Add extraction area, eligibility, failure, and completion rules.
- Add run start, abort, failure, and success result data.

### Dependencies

M1, M3, and written objective/extraction rules.

### Deliverables

A complete fixed-layout mission from start to extraction or failure.

### Acceptance

The run has a clear start, objective progression, first-charge event, countdown, extraction eligibility, and result. UI reflects the authoritative mission state.

### Automated validation

- Objective state machine.
- First-charge idempotence.
- Countdown pause/failure behavior.
- Extraction eligibility and result conditions.
- Restart clears all run state.

### Manual validation

Complete a run, fail one, lose a mercenary, trigger civilian reactions, pause/resume, and restart.

### Risks

Scoring or UI may start deciding mission state. Keep MissionCoordinator authoritative.

## M5 — Seeded procedural hospital

### Objective

Generate validated layouts from a seed while retaining a fixed-layout path.

### Tasks

- Define room template and door/socket resources.
- Build room graph generation with named random streams.
- Place objectives, extraction, civilians, and threats.
- Validate connectivity, reachable objectives, navigable paths, safe spawns, and difficulty budget.
- Add bounded retries and a documented fallback.
- Implement FixedLayoutProvider using the same scene contracts.

### Dependencies

M4 and room/navigation contracts from M2.

### Deliverables

A seeded procedural run and a fixed speedrun run that share mission/combat code.

### Acceptance

The same seed recreates the same layout/spawns within the documented determinism scope. Invalid layouts are rejected before play. Fixed mode does not fork mission rules.

### Automated validation

- Seed repeatability.
- Graph connectivity.
- Objective/extraction reachability.
- Spawn separation and room bounds.
- Retry/fallback behavior.
- Fixed-layout provider contract.

### Manual validation

Play several seeds, inspect layout readability, verify doors and objectives, and compare fixed-mode restarts.

### Risks

Generation can consume the project. Start with a small graph and a small template set; instrument failures before adding variety.

## M6 — Scoring, payout, and difficulty

### Objective

Add an event-driven result model for score, payout, and difficulty modes.

### Tasks

- Implement ScoreLedger and immutable score events.
- Define kills, rescue, time, damage, casualties, objectives, extraction, and penalties.
- Add RunConfig and difficulty modifiers.
- Build result screen and payout handoff.
- Add replay/retry seed display.

### Dependencies

M4 and M5.

### Deliverables

Deterministic run result data and readable result UI.

### Acceptance

Two runs with the same seed and rules produce the same score inputs. Score is independent from UI and freed scene nodes.

### Automated validation

- Score event accumulation.
- Difficulty modifier application.
- Result serialization in memory.
- Same-seed score consistency.

### Manual validation

Compare fast/slow, casualty/no-casualty, objective success/failure, and extraction outcomes.

### Risks

Undocumented scoring rules create balance churn. Keep formulas in data or a versioned rules resource.

## M7 — Speedrun and determinism hardening

### Objective

Make fixed-layout speedrun mode reliable enough for repeatable competition or internal timing.

### Tasks

- Lock layout, spawn tables, seed, and relevant modifiers.
- Remove gameplay-affecting uncontrolled random calls.
- Stabilize IDs and ordering.
- Add run seed/mode display.
- Add reset/restart path and timing instrumentation.
- Document determinism limits and platform assumptions.

### Dependencies

M5 and M6.

### Deliverables

Fixed-layout mode, repeatability tests, and a short determinism contract.

### Acceptance

Restarting the same mode and seed yields the same initial state and score inputs. Any known platform-dependent variation is documented.

### Automated validation

- Initial snapshot comparison.
- Seeded spawn order.
- Fixed-layout hash or canonical state comparison.
- Restart cleanup.

### Manual validation

Run repeated attempts and confirm no stale state, hidden random changes, or UI timing drift.

### Risks

Frame-perfect replay promises are premature. Scope first to same-platform, same-build repeatability.

## M8 — Content, polish, and release

### Objective

Make the validated game legible, tunable, and shippable.

### Tasks

- Add audio, VFX, survivor/zombie variation, objectives, rooms, and UI polish.
- Add accessibility and settings.
- Add performance profiling and pooling where measurements justify it.
- Add save/progression only if the product definition requires it.
- Test exports on target platforms.

### Dependencies

All prior milestones.

### Deliverables

Release candidate with content, QA checklist, and known-issues list.

### Acceptance

Core mission, modes, scoring, restart, and result flow are stable; assets load correctly; no known blocker remains in target exports.

### Automated validation

Full headless tests, scene/resource validation, export smoke tests, and repeatability checks.

### Manual validation

Play complete runs across modes and difficulty, inspect UI/audio/feedback, and test settings/restart/quit flows.

### Risks

Polish can mask core rule gaps. Do not move to M8 while M4 mission completion or M5 validation is unstable.

