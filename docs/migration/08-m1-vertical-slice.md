# M1 vertical slice decisions

This note records the decisions made while the Godot project was bootstrapped. The checkout did not contain the referenced `docs/migration/README.md` through `07-ai-development-workflow.md`, so the handoff requirements and the Unity assets were used as the migration constraints.

## Friendly fire

Friendly fire is disabled for the M1 slice. A projectile carries its source faction in a `DamagePacket`; the damageable actor rejects packets from the same faction unless a future mode explicitly enables friendly fire. This keeps shared squad fire safe while the selection and aim controls are being validated.

## Legacy tuning carried forward

- Mercenaries: 100 health, Kevlar armor value 2, projectile damage 20, 0.1 second fire cooldown.
- Zombie: 100 health, zombie armor value 4.
- The Unity code treats armor as a damage divider (`damage / armor`), rather than consuming armor points. Godot preserves that rule and rounds up the result.
- The Unity movement/projectile speeds were authored in a smaller world-unit scale. The Godot Resource keeps the legacy values as comments and uses pixel-space equivalents (180 px/s movement and 600 px/s projectile travel) for a playable 2D room.

## Scope boundary

The zombie has only the minimum chase/melee behavior required to exercise damage and cleanup. Procedural rooms, survivor AI, scoring, and save/load remain out of scope until this slice is stable.

## Controls

The operation starts with no mercenaries selected. Keys `1` through `4` toggle an individual mercenary in the selection. `WASD` moves the selected mercenary or mercenaries. The left mouse button fires toward the cursor. The right mouse button is currently unbound. `R` restarts the operation.
