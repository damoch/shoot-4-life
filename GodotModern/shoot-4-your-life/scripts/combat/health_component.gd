class_name HealthComponent
extends Node

signal damaged(applied_damage: int, remaining_health: int, armor_divider: int)
signal died

@export var max_health: int = 100
@export var armor_divider: int = 1

var current_health: int = 100
var is_alive: bool = true

func _ready() -> void:
	current_health = max_health
	is_alive = current_health > 0

func configure(health_value: int, armor_value: int) -> void:
	max_health = maxi(health_value, 1)
	armor_divider = maxi(armor_value, 1)
	current_health = max_health
	is_alive = true

func apply_damage(packet: DamagePacket) -> int:
	if not is_alive or packet == null:
		return 0
	var applied_damage := CombatRules.damage_after_armor(packet.amount, armor_divider)
	current_health = maxi(current_health - applied_damage, 0)
	damaged.emit(applied_damage, current_health, armor_divider)
	if current_health == 0:
		is_alive = false
		died.emit()
	return applied_damage
