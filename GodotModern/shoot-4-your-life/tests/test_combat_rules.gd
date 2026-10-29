extends SceneTree

var failures: int = 0

func _init() -> void:
	call_deferred("_run")

func _run() -> void:
	_check(CombatRules.can_damage(Teams.Faction.HUMAN, Teams.Faction.ZOMBIE, false), "different factions can damage")
	_check(not CombatRules.can_damage(Teams.Faction.HUMAN, Teams.Faction.HUMAN, false), "friendly fire is disabled by default")
	_check(CombatRules.can_damage(Teams.Faction.HUMAN, Teams.Faction.HUMAN, true), "friendly fire can be explicitly enabled")
	_check(CombatRules.damage_after_armor(20, 2) == 10, "legacy armor divider reduces 20 damage to 10")
	_check(CombatRules.damage_after_armor(20, 4) == 5, "legacy zombie armor divider reduces 20 damage to 5")
	_check(CombatRules.damage_after_armor(1, 10) == 1, "positive damage always deals at least one point")

	var health := HealthComponent.new()
	get_root().add_child(health)
	health.configure(100, 2)
	await process_frame
	var packet := DamagePacket.create(20, Teams.Faction.ZOMBIE, DamagePacket.DamageType.PROJECTILE)
	_check(health.apply_damage(packet) == 10, "health applies armor-divided damage")
	_check(health.current_health == 90, "health subtracts applied damage")
	var death_signal_count: Array[int] = [0]
	health.died.connect(func() -> void: death_signal_count[0] += 1)
	health.apply_damage(DamagePacket.create(200, Teams.Faction.ZOMBIE, DamagePacket.DamageType.MELEE))
	_check(death_signal_count[0] == 1, "health emits died exactly when health reaches zero")
	_check(not health.is_alive, "health marks actor dead")
	health.queue_free()

	if failures == 0:
		print("PASS: combat rule tests")
	else:
		push_error("FAIL: %d combat rule test(s)" % failures)
	quit(failures)

func _check(condition: bool, description: String) -> void:
	if condition:
		return
	failures += 1
	push_error(description)
