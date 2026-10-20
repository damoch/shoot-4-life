# 02 — Gameplay systems audit

## Status vocabulary

- Working scaffold: behavior exists and is visible in the prototype.
- Partial: a related mechanism exists, but it does not meet the intended game rule.
- Missing: no implementation was found.
- Migration risk: behavior exists but should not be ported literally.

## System matrix

| Intended system | Current evidence | Status | Migration interpretation |
| --- | --- | --- | --- |
| Four-person mercenary squad | Two playable actor instances and a two-entry controller list | Partial | Make squad size data-driven; first slice should prove four actors |
| Actor selection | Selected flag and number-key attempt | Partial | Replace with explicit squad selection state and input actions |
| Shared movement | Selected actors receive movement commands | Working scaffold | Preserve concept; add formation and collision-safe movement |
| Shared aim and fire | Selected actors aim at mouse and fire together | Working scaffold | Preserve concept; make command ownership explicit |
| Weapon swap | Main/secondary references and swap command | Working scaffold | Rebuild around WeaponData and actor loadout |
| Projectile weapon | Instantiated straight-line ammunition | Working scaffold | Add collision layers, attribution, pooling, and hit events |
| Melee weapon | Range check and timed melee ammunition | Working scaffold | Replace spawned melee object with an explicit attack window/hit query |
| Health and armor | Damage divided by armor value | Working scaffold | Preserve tuning, add damage packet and event ledger |
| Death | Flags, animation stop, collider trigger | Partial | Add dead state, cleanup, score event, and camera/AI removal |
| Friendly-fire rules | No team check in ammunition hit path | Missing/ambiguous | Decide and encode a single mission rule |
| Zombies | Room-scoped target chase and attack | Working scaffold | Rebuild as policy-driven state machine with perception |
| Survivor civilians | No civilian actor or behavior | Missing | New system; use reusable civilian policy resources |
| Room awareness | Trigger lists and notifications | Partial | Replace as one perception/navigation layer; triggers can remain implementation details |
| Navigation | AI Navigation package, no NavMeshData or API use | Missing | Add NavigationAgent2D or bounded grid/graph navigation |
| Hospital layout | Static room prefab and walls | Partial | Preserve fixed templates; add graph-based generation |
| Procedural generation | No generator, seed, validation, or retry logic | Missing | New deterministic generation subsystem |
| Explosive charges | No charge object or objective logic | Missing | New mission objective with planted/armed/detonated states |
| First charge event | No event or civilian reaction | Missing | MissionCoordinator signal/event must be the source of truth |
| Countdown | No countdown | Missing | New mission phase and HUD timer |
| Extraction | Exit zones exist as room transitions, not extraction | Partial | Reuse spatial idea; add objective completion and extraction rules |
| Score | No score ledger | Missing | New event-driven scoring service |
| Payout | No payout or result screen | Missing | New run result model and UI |
| Speedrun fixed layout | No mode | Missing | Add FixedLayoutProvider sharing room contracts with procedural mode |
| Difficulty modes | No mode model | Missing | Data-driven RunConfig and difficulty modifiers |
| Save/load | Throws NotImplementedException | Missing | Defer until run result/progression requirements are defined |
| Menus | Main menu, options, pause scaffold | Partial | Rebuild after gameplay shell; fix settings model |
| Audio/VFX feedback | No audio or particle assets detected | Missing | Add as content/polish after combat events exist |

## Current gameplay loop

The demonstrated loop is: load a test scene, select or attempt to select actors, move selected actors, aim at the mouse, fire a weapon, let zombies find targets in a room, apply damage on collision, and reload/swap weapons. A room trigger announces actor presence to zombie controllers.

There is no mission start state beyond scene loading, no objective state, no survivor population, no charge/extraction transition, no score, no payout, and no run completion state.

## Intended loop that must be designed

The target loop needs an explicit sequence:

1. Select operation and mode.
2. Generate or load a validated hospital layout from a seed.
3. Spawn four mercenaries, civilians, zombies, objectives, and extraction.
4. Move and fight while civilian policies react to squad behavior and mission events.
5. Plant charges and emit the first-charge event.
6. Resolve countdown, additional objectives, casualties, and extraction.
7. Compute score and payout from an event ledger.
8. Show the run result and optionally restart with another seed.

The Godot implementation should own this as a RunCoordinator/MissionCoordinator state machine rather than deriving state from scene presence or individual actors.

## Important rule decisions before implementation

The following decisions are currently unspecified and should be written as data or acceptance criteria before the relevant milestone:

- Whether friendly fire is always on, mode-dependent, or restricted to explosives.
- Whether civilians can be killed, panicked, armed, evacuated, or become hostile.
- What the first charge event does to civilians, zombies, spawn rates, and the timer.
- Whether all four mercenaries share commands or support independent selection.
- Whether a dead mercenary is removed from the squad, remains a body, or can affect extraction eligibility.
- What counts toward score: kills, rescued civilians, time, damage, casualties, charges, extraction, and penalties.
- Whether procedural generation may fail and retry, and what fallback is shown to the player.
- Whether fixed speedrun mode is only a layout lock or also fixes enemy/civilian spawns and difficulty.

## System boundary recommendation

Use gameplay events as the stable boundary:

- Combat emits damage_applied, actor_died, shot_fired, and objective damage events.
- Squad emits selection_changed, command_issued, and formation_changed.
- Mission emits objective_changed, first_charge_planted, countdown_started, extraction_state_changed, and run_finished.
- ScoreLedger listens to those events and never reaches into actor internals.
- UI listens to mission/combat/squad signals and does not decide rules.

This prevents the current Room and Actor classes from becoming a new set of Godot god objects.

