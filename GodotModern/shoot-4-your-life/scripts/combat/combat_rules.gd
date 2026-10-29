class_name CombatRules
extends RefCounted

static func can_damage(source_team: int, target_team: int, friendly_fire: bool) -> bool:
	return friendly_fire or source_team != target_team

static func damage_after_armor(raw_damage: int, armor_divider: int) -> int:
	if raw_damage <= 0:
		return 0
	return maxi(1, ceili(float(raw_damage) / float(maxi(armor_divider, 1))))
