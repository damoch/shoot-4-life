class_name SquadController
extends Node2D

signal selection_changed(selected_count: int)

const MERCENARY_SLOT_COUNT := 4

var actors: Array[MercenaryActor] = []
var selected_actors: Array[MercenaryActor] = []
var next_selection_slot: int = 0

func register_actor(actor: MercenaryActor) -> void:
	if actor == null or actors.has(actor):
		return
	actor.selection_slot = next_selection_slot
	next_selection_slot += 1
	actors.append(actor)
	actor.tree_exited.connect(_on_actor_tree_exited.bind(actor), CONNECT_ONE_SHOT)

func toggle_actor_by_index(index: int) -> void:
	_prune_invalid_actors()
	if index < 0 or index >= MERCENARY_SLOT_COUNT:
		return
	for actor in actors:
		if actor.selection_slot == index and is_instance_valid(actor) and actor.is_alive:
			if selected_actors.has(actor):
				selected_actors.erase(actor)
				actor.set_selected(false)
			else:
				selected_actors.append(actor)
				actor.set_selected(true)
			selection_changed.emit(selected_actors.size())
			return

func get_alive_selected() -> Array[MercenaryActor]:
	_prune_invalid_actors()
	return selected_actors.duplicate()

func get_alive_count() -> int:
	_prune_invalid_actors()
	return actors.size()

func _process(_delta: float) -> void:
	_prune_invalid_actors()
	for index in MERCENARY_SLOT_COUNT:
		if Input.is_action_just_pressed("select_mercenary_%d" % (index + 1)):
			toggle_actor_by_index(index)
	var movement := Input.get_vector("move_left", "move_right", "move_up", "move_down")
	var target := get_global_mouse_position()
	var should_fire := Input.is_action_pressed("fire")
	for actor in selected_actors:
		if is_instance_valid(actor) and actor.is_alive:
			actor.set_command(movement, target, should_fire)

func _prune_invalid_actors() -> void:
	var removed := false
	for index in range(actors.size() - 1, -1, -1):
		var actor := actors[index]
		if not is_instance_valid(actor) or not actor.is_alive:
			actors.remove_at(index)
			removed = true
	for index in range(selected_actors.size() - 1, -1, -1):
		var selected := selected_actors[index]
		if not is_instance_valid(selected) or not selected.is_alive:
			selected_actors.remove_at(index)
			removed = true
	if removed:
		selection_changed.emit(selected_actors.size())

func _on_actor_tree_exited(actor: MercenaryActor) -> void:
	actors.erase(actor)
	selected_actors.erase(actor)
	if is_inside_tree():
		selection_changed.emit(selected_actors.size())
