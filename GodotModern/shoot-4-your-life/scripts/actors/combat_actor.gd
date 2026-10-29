class_name CombatActor
extends CharacterBody2D

signal actor_died(actor: CombatActor)

@export var faction: int = Teams.Faction.HUMAN
@export var tuning: CombatTuning

@onready var health_component: HealthComponent = $HealthComponent

var is_alive: bool = true

func _ready() -> void:
	if tuning == null:
		tuning = CombatTuning.new()
	health_component.configure(tuning.max_health, tuning.armor)
	health_component.damaged.connect(_on_health_damaged)
	health_component.died.connect(_on_health_died)
	is_alive = true
	add_to_group("combat_actors")

func receive_damage(packet: DamagePacket) -> int:
	if not is_alive or packet == null:
		return 0
	if not CombatRules.can_damage(packet.source_team, faction, packet.friendly_fire):
		return 0
	return health_component.apply_damage(packet)

func set_selected(_selected: bool) -> void:
	# Concrete controllable actors override this. The no-op keeps enemy actors safe.
	pass

func _on_health_damaged(_applied_damage: int, _remaining_health: int, _armor_divider: int) -> void:
	queue_redraw()

func _on_health_died() -> void:
	is_alive = false
	velocity = Vector2.ZERO
	collision_layer = 0
	collision_mask = 0
	actor_died.emit(self)
	call_deferred("queue_free")
