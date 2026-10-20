# 04 — Asset inventory

## Inventory summary

The repository contains approximately:

- 46 PNG files.
- 35 C# source files.
- 15 core gameplay prefabs.
- 5 Unity scenes.
- 2 animation clips and 2 animator controllers.
- 1 material, 1 PSD, 1 TTF, 1 PDF, 1 ZIP, 4 XML files, and 2 text files.
- 5 DLL files.

No audio files, particle systems, or custom shader source files were found in the gameplay asset inventory.

All Unity asset paths in this document are relative to UnityLegacy/Assets/. The new Godot project is under GodotModern/shoot-4-your-life/ and currently contains only its default configuration/icon assets.

## Reuse classification

### Reuse directly after import

- Character sprite sheets: BloodyZombie and MassiveFrankWithRifle.
- Bullet sprite.
- Wall sprite.
- Room background/template artwork.
- Button pack artwork used by the menu.
- Font, subject to licensing and import verification.

These are the highest-confidence visual assets for the first Godot slice.

### Reuse as tuning/reference data

- PlayableActor prefab values: health 100, speed 10, Blue team, M9, dummy rifle, Kevlar.
- ZombieActor prefab values: health 100, speed 1, Red team at runtime, zombie weapon, ZombieArmor.
- Projectile values: damage 20, speed 60.
- Melee values: damage 50, minimum distance 2, melee length 1.
- Weapon magazine, reload, cooldown, and recoil settings.
- Room dimensions and wall placement from test.unity.

These values should be transcribed into Godot Resource files and reviewed, not hidden in scene scripts.

### Rebuild in Godot

- Unity prefabs, MonoBehaviour wiring, Animator controllers, Rigidbody settings, BoxCollider/child hierarchy, and Unity scene YAML.
- Unity NavMesh data and AI Navigation package configuration.
- Material references such as LaserPointer.mat unless the effect is explicitly needed.
- UI event wiring and PlayerPrefs settings behavior.

### Defer or exclude

- Mini Profiler demo content.
- Button-pack demo scenes not used by the gameplay flow.
- Json.NET content without gameplay source references.
- The unfinished SaveLoadManager and placeholder turn system.
- Imported third-party DLLs until a specific Godot integration need is proven.

## Texture and animation notes

BloodyZombie is an eight-frame 320x320 animation at 12 FPS and loops. MassiveFrankWithRifle is a four-frame 320x320 animation at 12 FPS and loops. The controllers contain one state each and no parameters, so a direct first-pass AnimatedSprite2D or a small AnimationTree is sufficient.

The infantry_human sprite is present but has no source references in the inspected project. Treat it as optional until a survivor visual is selected.

The room artwork is 2400x1200 and is referenced by the room prefab/scenes. It can serve as a fixed-layout visual reference, but procedural layouts will need modular room tiles, markers, or an alternative generated composition if the final game cannot use one large background.

## Import and licensing checklist

Before implementation uses these assets:

- Confirm source import dimensions, filtering, mipmaps, pixel-art settings, and animation frame order.
- Confirm sprite sheets are legally usable in the migrated project and preserve attribution requirements.
- Decide whether the current button pack is retained or replaced by Godot-native UI.
- Recreate collision shapes and hitbox offsets explicitly; do not infer them from Unity prefab hierarchy.
- Keep original files untouched and import copies or references through a documented asset pipeline.

## Missing content that affects the target game

- Survivor/civilian presentation variants.
- Charge/explosion visuals.
- Extraction and objective markers.
- Damage/death feedback.
- Muzzle flashes, impacts, blood, alarms, countdown, and score feedback.
- Audio for weapons, zombies, civilians, doors, objectives, and mission states.
- Loading, result, and speedrun UI.

Missing content should not block the first combat slice. It should be represented by simple placeholders and scheduled after gameplay contracts are validated.

