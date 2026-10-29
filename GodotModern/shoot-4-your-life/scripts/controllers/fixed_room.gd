class_name FixedRoom
extends Node2D

@export var room_rect: Rect2 = Rect2(-600.0, -350.0, 1200.0, 700.0)
@export var wall_thickness: float = 24.0

func _ready() -> void:
	_create_boundary("TopWall", Vector2(room_rect.size.x, wall_thickness), Vector2(room_rect.position.x + room_rect.size.x / 2.0, room_rect.position.y))
	_create_boundary("BottomWall", Vector2(room_rect.size.x, wall_thickness), Vector2(room_rect.position.x + room_rect.size.x / 2.0, room_rect.end.y))
	_create_boundary("LeftWall", Vector2(wall_thickness, room_rect.size.y), Vector2(room_rect.position.x, room_rect.position.y + room_rect.size.y / 2.0))
	_create_boundary("RightWall", Vector2(wall_thickness, room_rect.size.y), Vector2(room_rect.end.x, room_rect.position.y + room_rect.size.y / 2.0))
	queue_redraw()

func _create_boundary(wall_name: String, size: Vector2, wall_position: Vector2) -> void:
	var body := StaticBody2D.new()
	body.name = wall_name
	body.position = wall_position
	body.collision_layer = 1
	body.collision_mask = 1
	var shape_node := CollisionShape2D.new()
	var shape := RectangleShape2D.new()
	shape.size = size
	shape_node.shape = shape
	body.add_child(shape_node)
	add_child(body)

func _draw() -> void:
	draw_rect(room_rect, Color("#1d2a35"), true)
	draw_rect(room_rect, Color("#607d8b"), false, 5.0)
	var grid_step := 50.0
	var x := room_rect.position.x
	while x <= room_rect.end.x:
		draw_line(Vector2(x, room_rect.position.y), Vector2(x, room_rect.end.y), Color(0.25, 0.34, 0.40, 0.22), 1.0)
		x += grid_step
	var y := room_rect.position.y
	while y <= room_rect.end.y:
		draw_line(Vector2(room_rect.position.x, y), Vector2(room_rect.end.x, y), Color(0.25, 0.34, 0.40, 0.22), 1.0)
		y += grid_step
	draw_circle(Vector2.ZERO, 70.0, Color(0.12, 0.20, 0.24, 0.6))
