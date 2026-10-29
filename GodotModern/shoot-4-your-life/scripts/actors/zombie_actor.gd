class_name ZombieActor
extends CombatActor

var target: MercenaryActor
var melee_timer: float = 0.0

func _ready() -> void:
	super._ready()
	add_to_group("zombies")

func _physics_process(delta: float) -> void:
	if not is_alive:
		return
	melee_timer = maxf(melee_timer - delta, 0.0)
	if not is_instance_valid(target) or not target.is_alive:
		target = _find_nearest_mercenary()
	if not is_instance_valid(target):
		velocity = Vector2.ZERO
		return
	var distance := global_position.distance_to(target.global_position)
	if distance > tuning.melee_range:
		velocity = global_position.direction_to(target.global_position) * tuning.move_speed
		move_and_slide()
	else:
		velocity = Vector2.ZERO
		try_melee_attack()
	queue_redraw()

func _find_nearest_mercenary() -> MercenaryActor:
	var nearest: MercenaryActor
	var nearest_distance := INF
	for candidate_node in get_tree().get_nodes_in_group("mercenaries"):
		var candidate := candidate_node as MercenaryActor
		if candidate == null or not candidate.is_alive:
			continue
		var candidate_distance := global_position.distance_to(candidate.global_position)
		if candidate_distance < nearest_distance:
			nearest = candidate
			nearest_distance = candidate_distance
	if nearest != null and not nearest.tree_exited.is_connected(_on_target_exited):
		nearest.tree_exited.connect(_on_target_exited, CONNECT_ONE_SHOT)
	return nearest

func try_melee_attack() -> bool:
	if not is_instance_valid(target) or melee_timer > 0.0:
		return false
	var packet := DamagePacket.create(
		tuning.melee_damage,
		faction,
		DamagePacket.DamageType.MELEE,
		self,
		false
	)
	target.receive_damage(packet)
	melee_timer = tuning.melee_cooldown
	return true

func _on_target_exited() -> void:
	target = null

func _draw() -> void:
	var body_color := Color("#d45d67") if is_alive else Color("#4a3037")
	draw_circle(Vector2.ZERO, 21.0, body_color)
	draw_circle(Vector2.ZERO, 21.0, Color("#f5b0a8"), false, 2.0)
	var health_ratio := float(health_component.current_health) / float(maxi(health_component.max_health, 1))
	draw_rect(Rect2(-23.0, -35.0, 46.0, 5.0), Color("#1c2630"))
	draw_rect(Rect2(-23.0, -35.0, 46.0 * health_ratio, 5.0), Color("#ff7984"))
