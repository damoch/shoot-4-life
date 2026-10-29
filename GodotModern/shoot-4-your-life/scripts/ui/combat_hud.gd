class_name CombatHud
extends Control

@onready var status_label: Label = $MarginContainer/VBoxContainer/StatusLabel
@onready var message_label: Label = $MarginContainer/VBoxContainer/MessageLabel
@onready var restart_button: Button = $MarginContainer/VBoxContainer/RestartButton

var squad: SquadController
var operation: Operation

func _ready() -> void:
	restart_button.pressed.connect(_on_restart_pressed)

func configure(squad_controller: SquadController, operation_controller: Operation) -> void:
	squad = squad_controller
	operation = operation_controller
	refresh()

func refresh() -> void:
	if not is_instance_valid(squad) or not is_instance_valid(operation):
		return
	status_label.text = operation.get_status_text()
	message_label.text = "Selected: %d | WASD move | 1-4 toggle select | LMB fire | R restart" % squad.selected_actors.size()

func _on_restart_pressed() -> void:
	if is_instance_valid(operation):
		operation.restart_operation()
