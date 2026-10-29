extends SceneTree

var failures: int = 0

func _init() -> void:
	call_deferred("_run")

func _run() -> void:
	var operation_scene := load("res://scenes/operation.tscn") as PackedScene
	_check(operation_scene != null, "operation scene loads for runtime test")
	if operation_scene == null:
		quit(failures)
		return
	var operation := operation_scene.instantiate() as Operation
	get_root().add_child(operation)
	for _frame in 5:
		await process_frame
	var squad := operation.squad
	var mercenary := operation.get_node("Actors/Mercenary01") as MercenaryActor
	var zombie := operation.get_node("Actors/Zombie01") as ZombieActor
	_check(squad.get_alive_count() == 4, "runtime operation registers four mercenaries")
	_check(squad.selected_actors.is_empty(), "runtime operation starts with no mercenaries selected")
	_check(InputMap.has_action("select_mercenary_1"), "number 1 selection action exists")
	_check(InputMap.has_action("select_mercenary_2"), "number 2 selection action exists")
	_check(InputMap.has_action("select_mercenary_3"), "number 3 selection action exists")
	_check(InputMap.has_action("select_mercenary_4"), "number 4 selection action exists")
	_check(not InputMap.has_action("select_all") and not InputMap.has_action("select_next"), "legacy selection actions are unbound")
	var fire_events := InputMap.action_get_events("fire")
	var left_mouse_is_only_fire_binding := fire_events.size() == 1 and fire_events[0] is InputEventMouseButton and (fire_events[0] as InputEventMouseButton).button_index == MOUSE_BUTTON_LEFT
	_check(left_mouse_is_only_fire_binding, "left mouse is the only fire binding")
	squad.toggle_actor_by_index(0)
	_check(squad.selected_actors.size() == 1 and squad.selected_actors[0].name == "Mercenary01", "number 1 selects the first mercenary")
	squad.toggle_actor_by_index(1)
	_check(squad.selected_actors.size() == 2 and squad.selected_actors.has(operation.get_node("Actors/Mercenary02")), "number 2 adds the second mercenary")
	squad.toggle_actor_by_index(1)
	_check(squad.selected_actors.size() == 1 and squad.selected_actors[0].name == "Mercenary01", "pressing number 2 again deselects only the second mercenary")

	mercenary.aim_target = zombie.global_position
	mercenary.projectile_parent = operation.projectiles_root
	_check(mercenary.try_fire(), "mercenary can spawn a projectile")
	for _frame in 90:
		await physics_frame
	_check(not is_instance_valid(zombie) or zombie.health_component.current_health < zombie.health_component.max_health, "projectile reaches and damages the zombie")

	if is_instance_valid(zombie):
		for _hit in 25:
			zombie.receive_damage(DamagePacket.create(20, Teams.Faction.HUMAN, DamagePacket.DamageType.PROJECTILE))
		await process_frame
	_check(not is_instance_valid(zombie), "dead zombie is freed")

	var mercenary_to_remove := operation.get_node("Actors/Mercenary02") as MercenaryActor
	mercenary_to_remove.receive_damage(DamagePacket.create(200, Teams.Faction.ZOMBIE, DamagePacket.DamageType.MELEE))
	await process_frame
	_check(not is_instance_valid(mercenary_to_remove), "dead mercenary is freed")
	_check(squad.get_alive_count() == 3, "dead mercenary is removed from squad")
	_check(operation.camera_follow.targets.size() <= 3, "dead mercenary is removed from camera targets")
	squad.toggle_actor_by_index(2)
	_check(squad.selected_actors.has(operation.get_node("Actors/Mercenary03")), "number 3 keeps its mercenary identity after another actor dies")

	operation.queue_free()
	await process_frame
	if failures == 0:
		print("PASS: operation runtime tests")
	else:
		push_error("FAIL: %d operation runtime test(s)" % failures)
	quit(failures)

func _check(condition: bool, description: String) -> void:
	if condition:
		return
	failures += 1
	push_error(description)
