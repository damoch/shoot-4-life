extends SceneTree

var failures: int = 0

func _init() -> void:
	call_deferred("_run")

func _run() -> void:
	var scene_paths := [
		"res://scenes/bootstrap.tscn",
		"res://scenes/main_menu.tscn",
		"res://scenes/fixed_room.tscn",
		"res://scenes/operation.tscn",
		"res://scenes/mercenary.tscn",
		"res://scenes/zombie.tscn",
		"res://scenes/projectile.tscn",
	]
	for scene_path in scene_paths:
		var packed_scene := load(scene_path) as PackedScene
		_check(packed_scene != null, "scene loads: " + scene_path)

	var operation_scene := load("res://scenes/operation.tscn") as PackedScene
	if operation_scene != null:
		var operation := operation_scene.instantiate()
		get_root().add_child(operation)
		await process_frame
		_check(operation.get_node("Actors/Mercenary01") != null, "operation owns Mercenary01")
		_check(operation.get_node("Actors/Zombie01") != null, "operation owns Zombie01")
		_check(operation.get_node("SquadController").get_alive_count() == 4, "operation registers four living mercenaries")
		operation.queue_free()
		await process_frame

	if failures == 0:
		print("PASS: scene load tests")
	else:
		push_error("FAIL: %d scene test(s)" % failures)
	quit(failures)

func _check(condition: bool, description: String) -> void:
	if condition:
		return
	failures += 1
	push_error(description)
