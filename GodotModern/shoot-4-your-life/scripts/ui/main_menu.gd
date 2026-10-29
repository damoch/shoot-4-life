extends Control

@export_file("*.tscn") var operation_scene: String = "res://scenes/operation.tscn"

@onready var start_button: Button = $CenterContainer/PanelContainer/MarginContainer/VBoxContainer/StartButton

func _ready() -> void:
	start_button.pressed.connect(_on_start_pressed)
	start_button.grab_focus()

func _on_start_pressed() -> void:
	get_tree().change_scene_to_file(operation_scene)
