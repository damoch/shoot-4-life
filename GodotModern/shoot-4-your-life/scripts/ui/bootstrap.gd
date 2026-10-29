extends Node

@export_file("*.tscn") var next_scene: String = "res://scenes/main_menu.tscn"

func _ready() -> void:
	get_tree().call_deferred("change_scene_to_file", next_scene)
