# 01 — Project audit

## Scope and method

This audit is read-only. It inspected the repository tree, Git history/status, Unity project metadata, package manifests, C# source, scenes, prefabs, animation/material metadata, build settings, and tests. No Unity editor run, headless test run, export, dependency installation, or Godot implementation was performed. After the repository cleanup, all Unity project paths in this document are relative to UnityLegacy/.

The source of truth is the current checkout at branch codex/work-setup, commit 069136a. The remote default reference is origin/master. The repository root now contains the Unity project under UnityLegacy/ and the new Godot project under GodotModern/shoot-4-your-life/.

## Repository and engine

| Area | Finding |
| --- | --- |
| Engine | Unity 2022.3.62f2, revision 7670c08855a9 |
| Project format | Unity project under UnityLegacy/ with Assets, Packages, ProjectSettings, Library, Temp, Logs, and generated IDE files |
| Godot state | GodotModern/shoot-4-your-life/project.godot exists; it identifies a Godot 4.7 Forward Plus project with no gameplay scenes or scripts yet |
| Godot executable | Not available on PATH during the audit |
| Unity executable | Installed at C:\Program Files\Unity\Hub\Editor\2022.3.62f2\Editor\Unity.exe |
| Build scenes | mainMenu.unity and test.unity |
| Additional scene | testNoZombies.unity exists but is not in EditorBuildSettings |
| Package era | Unity 2022 LTS packages, including AI Navigation, UGUI, Timeline, ads, analytics, purchasing, and test framework |
| Input | Legacy Input Manager is active; no numeric player-selection actions were found |

The Git history shows the project was originally created around Unity 2018.3.7f1 and later upgraded to Unity 2022.3.62f2. This matters because serialized Unity behavior and package assumptions should not be treated as a clean design baseline.

## Current runtime structure

### Actors

Actor is a large MonoBehaviour that combines health, selection, movement, weapons, armor, animation state, infection, team, and death behavior. It requires a Rigidbody2D and BoxCollider2D. Selection changes the Rigidbody body type between Dynamic and Kinematic. Player movement uses Transform.Translate, while non-player movement uses Rigidbody2D.MovePosition.

The actor has two weapon object references, a selected flag, health, speed, team, actor type, armor, and a target position. Death disables selection, sets speed to zero, makes the body dynamic, stops animation, and changes the root collider to a trigger. There is no death signal, score event, cleanup service, or removal from room/camera collections.

### Player controller

PlayableActorController maps legacy key codes to commands, sends movement commands to selected actors, rotates selected actors toward the mouse while aim is held, and fires all selected actors on the left mouse button.

Selection attempts to use Input.GetKeyDown with strings such as 1, 2, and so on. The Input Manager does not define matching numeric button entries, so this is a fragile or non-functional selection path. The serialized scene list contains two playable actors in both test scenes, not four.

### Zombie controller

NonPlayableZombieActorController assigns a zombie team, tracks potential targets notified by Room, selects an available target, rotates toward it, moves toward it, and fires when the weapon reports that an attack is possible. It is a simple room-scoped chase/attack loop with no navigation mesh use, shared blackboard, perception model, behavior state, or survivor policy.

### Weapons and ammunition

Weapon supports projectile and melee ammunition, magazines, cooldown, reload duration, optional recoil, and weapon swapping. Projectile shooting instantiates ammunition objects directly. The code itself notes that pooling should be added later.

Ammunition moves projectiles in a straight line, checks Actor through a parent transform lookup, applies damage divided by armor, and destroys non-penetrating ammunition after a hit. There is no damage event, attacker attribution, explicit friendly-fire policy, hit feedback, or robust collision contract.

### Rooms and camera

Room is a trigger-based registry of actors in the room and neighboring rooms. It notifies zombie controllers about actors entering and removes actors on exit through RoomExitZone. It does not generate rooms, create navigation, own mission state, or validate a connected layout.

CameraController follows a list of target transforms and adjusts zoom from target bounds. It has no actor-death cleanup signal and therefore depends on scene references remaining valid.

### UI, settings, and persistence

MenuController pauses by changing Time.timeScale and activates menu states. MainMenuState always loads the scene named test for a new game. OptionsState persists fullscreen using PlayerPrefs strings, while GameStartupManager reads a fullscreen integer. This is a settings persistence mismatch.

SaveLoadManager contains only NotImplementedException methods. There is no persistent campaign, payout, score ledger, or run result model.

## Scene and prefab evidence

UnityLegacy/Assets/Scenes/test.unity contains:

- Four ZombieActor prefab instances.
- Two PlayableActor prefab instances.
- Four wall instances.
- One room prefab, two exit zones, a Canvas, and a camera controller.
- A playable actor list containing exactly two actor references.
- Pause/menu wiring.

testNoZombies.unity contains the same two playable actor references, no zombies, and similar room/wall/UI wiring. It is useful as a combat setup reference but is not a build scene.

The PlayableActor prefab uses 100 health, speed 10, Blue team, a main M9, a secondary dummy rifle, and Kevlar armor. The ZombieActor prefab uses 100 health, speed 1, a zombie weapon, ZombieArmor, and a controller that changes the team to Red. Projectile damage is 20 with speed 60. Melee damage is 50 with a minimum distance of 2. Kevlar divides damage by 2 and zombie flesh divides damage by 4.

The scenes contain NavMeshSettings with no NavMeshData, and source searches found no gameplay use of Unity NavMesh APIs. The installed AI Navigation package is therefore not evidence of working navigation.

## Build and code risks

- GameStartupManager in a runtime source folder imports UnityEditor and references EditorApplication. It is included in the generated Assembly-CSharp project, so a player build should be explicitly validated before relying on it.
- Legacy Input Manager behavior is relied upon, while numeric selection is not configured.
- Transform movement and Rigidbody movement are mixed.
- Parent transform lookups are used as collision and room-registration contracts.
- Actor death does not publish lifecycle events or remove references from camera/room/controller collections.
- The origin Vector2 is used as an invalid NPC target sentinel, so the world origin cannot be a valid target position.
- Weapon and ammunition spawning are allocation-heavy and have no pooling boundary.
- Settings read/write types differ.
- Multiple game controller concepts exist: a pause controller and a separate namespaced turn-number controller.
- The save/load implementation and turn logic are placeholders rather than migration targets.

## Tests and verification

There are two test files:

- GameControllerTests checks that NextTurn increments a turn number.
- MainMenuTests checks component creation and state lookup with mocks.

These tests do not cover movement, input, selection, firing, damage, death, AI targeting, room entry/exit, camera cleanup, settings persistence, scene loading, or build compatibility.

The audit intentionally did not run Unity because opening the editor, running tests, or exporting can mutate Library, Temp, Logs, and other generated state. A fresh Unity build and test pass should be a separate approval-gated baseline task.

## Audit conclusion

The project is a useful behavior and asset reference, not a complete implementation of the intended game. The migration should retain tuning and the basic combat vocabulary while replacing the Unity-specific composition, input, lifecycle, AI, mission, generation, and persistence layers with explicit Godot systems.

