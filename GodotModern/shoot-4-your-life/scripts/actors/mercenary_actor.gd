class_name MercenaryActor
extends CombatActor

@export var projectile_scene: PackedScene

var projectile_parent: Node2D
var is_selected: bool = false
var aim_target: Vector2
var movement_input: Vector2 = Vector2.ZERO
var fire_timer: float = 0.0
var selection_slot: int = -1

func _ready() -> void:
	super._ready()
	add_to_group("mercenaries")
	aim_target = global_position + Vector2.RIGHT * 100.0

func _physics_process(delta: float) -> void:
	if not is_alive:
		return
	fire_timer = maxf(fire_timer - delta, 0.0)
	velocity = movement_input.normalized() * tuning.move_speed
	move_and_slide()
	queue_redraw()

func set_command(move_input: Vector2, target: Vector2, should_fire: bool) -> void:
	if not is_alive:
		movement_input = Vector2.ZERO
		return
	movement_input = move_input
	aim_target = target
	if should_fire:
		try_fire()

func set_selected(selected: bool) -> void:
	is_selected = selected and is_alive
	queue_redraw()

func try_fire() -> bool:
	if not is_alive or fire_timer > 0.0 or projectile_scene == null or projectile_parent == null:
		return false
	var direction := global_position.direction_to(aim_target)
	if direction.length_squared() < 0.01:
		return false
	var projectile := projectile_scene.instantiate() as Projectile
	if projectile == null:
		return false
	projectile_parent.add_child(projectile)
	var packet := DamagePacket.create(
		tuning.projectile_damage,
		faction,
		DamagePacket.DamageType.PROJECTILE,
		self,
		false
	)
	projectile.launch(global_position + direction * 23.0, direction, packet, tuning.projectile_speed)
	fire_timer = tuning.fire_cooldown
	return true

func _draw() -> void:
	var body_color := Color("#6bd6ff") if is_alive else Color("#394b5c")
	draw_circle(Vector2.ZERO, 18.0, body_color)
	draw_circle(Vector2.ZERO, 18.0, Color("#d9f5ff"), false, 2.0)
	if is_selected:
		draw_arc(Vector2.ZERO, 26.0, 0.0, TAU, 32, Color("#f5d76e"), 3.0)
	var direction := global_position.direction_to(aim_target)
	draw_line(Vector2.ZERO, direction * 27.0, Color("#f0f4f8"), 4.0)
	var health_ratio := float(health_component.current_health) / float(maxi(health_component.max_health, 1))
	draw_rect(Rect2(-21.0, -32.0, 42.0, 5.0), Color("#1c2630"))
	draw_rect(Rect2(-21.0, -32.0, 42.0 * health_ratio, 5.0), Color("#65e08a"))
