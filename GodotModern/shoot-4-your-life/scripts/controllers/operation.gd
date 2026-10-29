class_name Operation
extends Node2D

@onready var squad: SquadController = $SquadController
@onready var actors_root: Node2D = $Actors
@onready var projectiles_root: Node2D = $Projectiles
@onready var camera_follow: CameraFollow = $Camera2D
@onready var hud: CombatHud = $HUD/CombatHud

var defeated_zombies: int = 0

func _ready() -> void:
	for actor_node in actors_root.get_children():
		var mercenary := actor_node as MercenaryActor
		if mercenary != null:
			mercenary.projectile_parent = projectiles_root
			mercenary.actor_died.connect(_on_actor_died)
			squad.register_actor(mercenary)
		var zombie := actor_node as ZombieActor
		if zombie != null:
			zombie.actor_died.connect(_on_actor_died)
	squad.selection_changed.connect(_on_selection_changed)
	hud.configure(squad, self)
	_on_selection_changed(squad.selected_actors.size())

func _process(_delta: float) -> void:
	camera_follow.set_targets(squad.get_alive_selected())
	if Input.is_action_just_pressed("restart"):
		restart_operation()

func restart_operation() -> void:
	get_tree().reload_current_scene()

func get_status_text() -> String:
	return "%d / 4 mercenaries active   |   %d zombie remaining" % [squad.get_alive_count(), _get_alive_zombie_count()]

func _get_alive_zombie_count() -> int:
	if not is_inside_tree():
		return 0
	var count := 0
	for zombie_node in get_tree().get_nodes_in_group("zombies"):
		var zombie := zombie_node as ZombieActor
		if zombie != null and zombie.is_alive:
			count += 1
	return count

func _on_selection_changed(_selected_count: int) -> void:
	if not is_inside_tree():
		return
	if is_instance_valid(camera_follow):
		camera_follow.set_targets(squad.get_alive_selected())
	if is_instance_valid(hud):
		hud.refresh()

func _on_actor_died(actor: CombatActor) -> void:
	if actor is ZombieActor:
		defeated_zombies += 1
	if is_instance_valid(hud):
		hud.refresh()
