class_name DamagePacket
extends Resource

enum DamageType {
	PROJECTILE,
	MELEE,
}

var amount: int = 0
var source_team: int = Teams.Faction.HUMAN
var damage_type: DamageType = DamageType.PROJECTILE
var friendly_fire: bool = false
var source: Node = null

static func create(
	damage_amount: int,
	source_faction: int,
	type: DamageType,
	packet_source: Node = null,
	allow_friendly_fire: bool = false
) -> DamagePacket:
	var packet := DamagePacket.new()
	packet.amount = damage_amount
	packet.source_team = source_faction
	packet.damage_type = type
	packet.source = packet_source
	packet.friendly_fire = allow_friendly_fire
	return packet
