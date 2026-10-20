# 07 — AI-assisted development workflow

## Purpose

AI assistance should accelerate small, reviewable Godot changes without allowing duplicated gameplay logic, hidden global state, or large speculative rewrites.

## Project rules for AGENTS.md

The future Godot project should include repository-local rules covering:

- Godot 4.x and default language choice.
- Scene ownership and component boundaries.
- Signal naming and command/event distinction.
- Data-driven tuning through Resources.
- Explicit collision layers and typed references.
- No unnecessary autoloads or global event buses.
- All gameplay-affecting randomness receives a seed or named random stream.
- Every new system has a small automated test and a playable/manual validation path.
- A change must preserve the current milestone's acceptance criteria.
- Existing Unity files under UnityLegacy/ are references until the migration is approved; do not delete or mass-convert them.

## Task request format

Every AI implementation task should state:

1. Context: current milestone and the relevant scenes/scripts.
2. Desired behavior: player-visible or testable rule.
3. Scope: files or systems allowed to change.
4. Non-goals: what must not be redesigned.
5. Acceptance criteria: observable outcomes.
6. Validation: exact tests, headless commands, and manual scenario.
7. Report format: files changed, tests run, known limitations, and follow-up.

Example task shape:

- Add four-member selection to SquadController.
- Do not change mission state or procedural generation.
- Selection must be explicit, stable after death, and visible.
- Add unit coverage for selection/death and a manual four-actor scene check.

## Agent loop

### 1. Inspect

Read the relevant scene, script, Resource, test, and current milestone document. Search for existing signals, ownership, and duplicate behavior before writing code.

### 2. State assumptions

If a rule is unspecified, identify the assumption in the task report. Do not silently choose mission, scoring, civilian, friendly-fire, or determinism rules.

### 3. Make the smallest coherent change

Prefer one component or one contract at a time. Avoid broad renames, mass scene rewrites, and framework creation before a concrete use case exists.

### 4. Test the rule

Add or update a focused automated test. Test the rule at the lowest useful level before relying only on a scene test.

### 5. Run headless validation

Use Godot headless project boot, script/resource tests, and scene-load checks. Keep a known-good command list in the repository once Godot is available.

### 6. Review the diff

Check scene ownership, signal direction, resource references, collision layers, random sources, and whether a change introduced a second source of truth.

### 7. Perform a manual scenario

Play the smallest scene that exercises the behavior. Verify readability, feel, restart behavior, and freed-node safety.

### 8. Commit narrowly

Use a branch name such as codex/squad-selection or codex/mission-countdown. Keep commits focused and describe behavior, tests, and known limitations.

### 9. Update documents

If a design rule changes, update the relevant migration document or a decision record in the same change. Do not let implementation silently redefine the plan.

## Safe automation

Good candidates for automation:

- Resource and scene validation.
- InputMap/action existence checks.
- Damage, score, objective, and generation unit tests.
- Headless boot and scene-load smoke tests.
- Seed repeatability and canonical-state comparisons.
- Duplicate signal or missing-reference searches.
- Import metadata checks.

Require human judgment for:

- Whether squad control feels readable.
- Whether formations are tactically useful.
- Whether survivor reactions communicate intent.
- Whether procedural layouts are fair and legible.
- Whether scoring rules match the intended experience.
- Whether an asset is visually and legally appropriate.

## Safeguards

### Duplicate logic

Search before adding behavior. One owner for damage resolution, mission state, score, generation, and selection. Components emit facts; coordinators own progression.

### Hidden globals

Reject new autoloads unless the task documents why scene-local ownership cannot work. Settings and a run session are the default exceptions.

### Player/AI coupling

Do not use actor type or a player flag to branch health, combat, or movement behavior. Player input and AI policy should both produce intentions consumed by shared components.

### Dead actor leaks

Every actor-died path must remove references from squads, camera targets, perception lists, room registries, and score subscriptions as appropriate. Add restart tests.

### Uncontrolled randomness

Search for random calls in gameplay code. Require an injected RandomNumberGenerator or named stream for every gameplay-affecting decision.

### Massive rewrites

Set a changed-file and changed-system scope in the task. Pause and request review if acceptance requires touching unrelated systems.

## Review gates

Require a human review before merging changes that affect:

- Objective rules, countdown, extraction, score, or payout.
- Autoloads, global services, or framework-level abstractions.
- Procedural generation, seeds, retry/fallback, or navigation dependencies.
- Asset deletion, broad import changes, or licensing-sensitive files.
- Determinism guarantees or engine/platform behavior.
- Actor lifecycle, friendly fire, or any rule that can invalidate a run.

## Definition of done for an AI task

A task is done only when:

- The requested behavior exists in the intended owner.
- The acceptance criteria are checked.
- Automated validation passes or a failure is reported.
- A manual scenario was run when the change is player-visible.
- No unrelated behavior was changed.
- The final report lists changed files, validation, assumptions, and remaining risks.

