class_name CameraFollow
extends Camera2D

@export var follow_lerp: float = 7.0
var targets: Array[Node2D] = []

func set_targets(new_targets: Array[MercenaryActor]) -> void:
	targets.clear()
	for target in new_targets:
		if is_instance_valid(target) and target.is_alive:
			targets.append(target)

func _process(delta: float) -> void:
	_prune_targets()
	if targets.is_empty():
		return
	var center := Vector2.ZERO
	for target in targets:
		center += target.global_position
	center /= targets.size()
	global_position = global_position.lerp(center, minf(delta * follow_lerp, 1.0))

func _prune_targets() -> void:
	for index in range(targets.size() - 1, -1, -1):
		var target := targets[index]
		if not is_instance_valid(target):
			targets.remove_at(index)
