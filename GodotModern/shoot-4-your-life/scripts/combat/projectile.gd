class_name Projectile
extends Area2D

@export var max_lifetime: float = 2.5

var velocity: Vector2 = Vector2.ZERO
var damage_packet: DamagePacket
var lifetime: float = 0.0

func _ready() -> void:
	body_entered.connect(_on_body_entered)
	queue_redraw()

func launch(start_position: Vector2, direction: Vector2, packet: DamagePacket, projectile_speed: float) -> void:
	global_position = start_position
	velocity = direction.normalized() * projectile_speed
	damage_packet = packet
	rotation = velocity.angle()

func _physics_process(delta: float) -> void:
	var next_position := global_position + velocity * delta
	if _try_hit_along_segment(next_position):
		return
	global_position = next_position
	lifetime += delta
	if lifetime >= max_lifetime:
		queue_free()

func _on_body_entered(body: Node2D) -> void:
	_impact(body)

func _try_hit_along_segment(next_position: Vector2) -> bool:
	if damage_packet == null or not is_inside_tree():
		return false
	var query := PhysicsRayQueryParameters2D.create(global_position, next_position)
	query.collision_mask = 1
	if damage_packet.source is CollisionObject2D:
		query.exclude = [damage_packet.source.get_rid()]
	var result := get_world_2d().direct_space_state.intersect_ray(query)
	if result.is_empty():
		return false
	var collider := result.get("collider") as Node2D
	if collider == null:
		return false
	_impact(collider)
	return true

func _impact(body: Node2D) -> void:
	if damage_packet == null:
		queue_free()
		return
	if body == damage_packet.source:
		return
	var actor := body as CombatActor
	if actor != null:
		actor.receive_damage(damage_packet)
	queue_free()

func _draw() -> void:
	draw_circle(Vector2.ZERO, 4.0, Color("#f7e27d"))
	draw_line(Vector2(-9.0, 0.0), Vector2(-3.0, 0.0), Color("#ff9f43"), 2.0)
