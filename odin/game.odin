package sharks

// Pure game rules. No browser imports, so everything here runs under `odin test`.
// Ported from the VB.NET game in ../src (see ../docs/PORT_PLAN.md, section 1b).

import "core:fmt"
import "core:math"
import "core:strings"

MINIMUM_SHARK_DISTANCE :: 10.0
MOOR_DISTANCE :: 1.0
AD_BREAK_MS :: i64(2 * 60 * 1000)
MIN_HEADING :: 0.0
MAX_HEADING :: 360.0
MIN_SPEED :: 0.1
MAX_SPEED :: 1.0

PIER_NAME :: "Pier"
BOAT_NAME :: "Blue Boat"
SHARK_NAME :: "Shark"

Place :: enum {
	Pier,
	Boat,
}

// The numeric prompt the player is in the middle of answering.
Mode :: enum {
	None,
	Change_Heading,
	Change_Speed,
}

// Boat verbs, in the order the menu lists them.
Verb :: enum {
	Unmoor,
	Moor,
	Move,
	Set_Heading,
	Set_Speed,
}

Message :: struct {
	text: string,
	href: string, // non-empty for a link
}

Game :: struct {
	embarked:     bool,
	name:         string,
	at:           Place,
	moored:       bool,
	x, y:         f64,
	heading:      f64,
	speed:        f64,
	shark:        bool,
	dead:         bool,
	mode:         Mode,
	ad_active:    bool,
	ad_finish_ms: i64, // wall clock, ms since the epoch
	messages:     [dynamic]Message,
}

game_destroy :: proc(g: ^Game) {
	clear_messages(g)
	delete(g.messages)
	delete(g.name)
	g^ = {}
}

verb_name :: proc(v: Verb) -> string {
	switch v {
	case .Unmoor:      return "Unmoor"
	case .Moor:        return "Moor"
	case .Move:        return "Move"
	case .Set_Heading: return "Set Heading..."
	case .Set_Speed:   return "Set Speed..."
	}
	return ""
}

place_name :: proc(p: Place) -> string {
	return p == .Pier ? PIER_NAME : BOAT_NAME
}

// ---- messages ----------------------------------------------------------

clear_messages :: proc(g: ^Game) {
	for m in g.messages {
		delete(m.text)
		delete(m.href)
	}
	clear(&g.messages)
}

add_message :: proc(g: ^Game, format: string, args: ..any) {
	append(&g.messages, Message{text = fmt.aprintf(format, ..args)})
}

add_link :: proc(g: ^Game, text, href: string) {
	append(&g.messages, Message{text = strings.clone(text), href = strings.clone(href)})
}

// ---- math (TGGD.Extensions.Utility) -------------------------------------

distance_between :: proc(ax, ay, bx, by: f64) -> f64 {
	return math.sqrt(math.pow(bx - ax, 2) + math.pow(by - ay, 2))
}

// Degrees counter-clockwise from +x, normalised to 0..360.
heading_between :: proc(ax, ay, bx, by: f64) -> f64 {
	d := math.atan2(by - ay, bx - ax) * 180.0 / math.PI
	return d < 0 ? d + 360.0 : d
}

next_xy :: proc(x, y, heading, speed: f64) -> (f64, f64) {
	r := heading * math.PI / 180.0
	return x + math.cos(r) * speed, y + math.sin(r) * speed
}

// VB's CInt rounds half to even.
round_half_even :: proc(v: f64) -> int {
	f := math.floor(v)
	diff := v - f
	n := int(f)
	if diff > 0.5 || (diff == 0.5 && n % 2 != 0) {
		n += 1
	}
	return n
}

boat_distance_to_pier :: proc(g: ^Game) -> f64 {
	return distance_between(g.x, g.y, 0, 0)
}

boat_heading_to_pier :: proc(g: ^Game) -> f64 {
	return heading_between(g.x, g.y, 0, 0)
}

// ---- lifecycle -----------------------------------------------------------

// Wipes the world. Like the original's Clear, this also ends any ad break.
abandon :: proc(g: ^Game) {
	clear_messages(g)
	delete(g.name)
	g.embarked = false
	g.name = ""
	g.at = .Pier
	g.moored = false
	g.x, g.y = 0, 0
	g.heading, g.speed = 0, 0
	g.shark = false
	g.dead = false
	g.mode = .None
	g.ad_active = false
	g.ad_finish_ms = 0
}

embark :: proc(g: ^Game, name: string) {
	abandon(g)
	g.embarked = true
	g.name = strings.clone(name)
	g.at = .Pier
	g.moored = true
	g.x, g.y = 0, 0
	g.heading = MIN_HEADING
	g.speed = MAX_SPEED
	add_message(g, "Welcome to Shark Attackers of SPLORR!!!")
	look(g)
}

// ---- looking -------------------------------------------------------------

// The mooring feature at the avatar's place exists only while the boat is moored.
mooring_name :: proc(g: ^Game) -> (name: string, ok: bool) {
	if !g.moored {
		return "", false
	}
	return g.at == .Pier ? "Mooring from Pier to Blue Boat" : "Mooring from Blue Boat to Pier", true
}

// Appends to the message log without clearing it first (the original's Look).
look :: proc(g: ^Game) {
	add_message(g, "%s is at %s.", g.name, place_name(g.at))
	if g.at == .Boat {
		add_message(g, "Location: (%.2f,%.2f)", g.x, g.y)
		if g.moored {
			add_message(g, "Moored to pier")
		} else {
			add_message(g, "Current heading: %.2f°", g.heading)
			add_message(g, "Current speed: %.2f", g.speed)
			add_message(g, "Distance to pier: %.2f", boat_distance_to_pier(g))
			add_message(g, "Heading to pier: %.2f°", boat_heading_to_pier(g))
		}
	}
	if name, ok := mooring_name(g); ok {
		add_message(g, "Features:")
		add_message(g, "- %s", name)
	}
}

look_action :: proc(g: ^Game) {
	clear_messages(g)
	look(g)
}

show_status :: proc(g: ^Game) {
	clear_messages(g)
	add_message(g, "Status:")
}

// ---- state queries ---------------------------------------------------------

in_combat :: proc(g: ^Game) -> bool {
	return g.shark && g.at == .Boat
}

ad_in_progress :: proc(g: ^Game) -> bool {
	return g.ad_active
}

// ---- boat verbs ------------------------------------------------------------

// Only the boat has verbs; the pier has none.
can_perform :: proc(g: ^Game, v: Verb) -> bool {
	if g.at != .Boat {
		return false
	}
	switch v {
	case .Unmoor:
		return g.moored
	case .Moor:
		return !g.moored && boat_distance_to_pier(g) < MOOR_DISTANCE
	case .Move, .Set_Heading, .Set_Speed:
		return !g.moored
	}
	return false
}

// `shark_roll` is a uniform number in [0,1) used only by Move.
perform :: proc(g: ^Game, v: Verb, shark_roll: f64 = 0) {
	if !can_perform(g, v) {
		return
	}
	clear_messages(g)
	switch v {
	case .Unmoor:
		add_message(g, "%s unmoors %s.", g.name, BOAT_NAME)
		g.moored = false
		look(g)
	case .Moor:
		add_message(g, "%s moors %s to %s.", g.name, BOAT_NAME, PIER_NAME)
		g.moored = true
		look(g)
	case .Move:
		add_message(g, "%s moves.", BOAT_NAME)
		g.x, g.y = next_xy(g.x, g.y, g.heading, g.speed)
		spawn_shark(g, shark_roll)
		look(g)
	case .Set_Heading:
		g.mode = .Change_Heading
	case .Set_Speed:
		g.mode = .Change_Speed
	}
}

// Chance is weight/(10+weight) with weight = round-half-even(distance - 10),
// and zero closer than 10 to the pier, exactly as the original's weighted roll.
shark_weight :: proc(distance: f64) -> int {
	if distance < MINIMUM_SHARK_DISTANCE {
		return -1
	}
	return round_half_even(distance - MINIMUM_SHARK_DISTANCE)
}

spawn_shark :: proc(g: ^Game, roll: f64) -> bool {
	true_weight := shark_weight(boat_distance_to_pier(g))
	if true_weight < 0 {
		return false
	}
	total := int(MINIMUM_SHARK_DISTANCE) + true_weight
	pick := int(roll * f64(total))
	if pick < true_weight {
		g.shark = true
		add_message(g, "%s appears!", SHARK_NAME)
		return true
	}
	return false
}

set_heading :: proc(g: ^Game, heading: f64) {
	g.heading = clamp(heading, MIN_HEADING, MAX_HEADING)
	look(g)
	g.mode = .None
}

set_speed :: proc(g: ^Game, speed: f64) {
	g.speed = clamp(speed, MIN_SPEED, MAX_SPEED)
	look(g)
	g.mode = .None
}

// ---- features ----------------------------------------------------------------

describe_mooring :: proc(g: ^Game) {
	clear_messages(g)
	if name, ok := mooring_name(g); ok {
		add_message(g, "This is a %s.", name)
	}
}

// The mooring's Enter verb: walk between the pier and the boat while moored.
enter_mooring :: proc(g: ^Game) {
	name, ok := mooring_name(g)
	if !ok {
		return
	}
	clear_messages(g)
	add_message(g, "%s uses %s.", g.name, name)
	g.at = g.at == .Pier ? .Boat : .Pier
	look(g)
}

// ---- combat --------------------------------------------------------------------

// There is no way to win: the shark always kills the avatar.
fight :: proc(g: ^Game) {
	if !in_combat(g) {
		return
	}
	clear_messages(g)
	add_message(g, "%s attacks %s.", g.name, SHARK_NAME)
	add_message(g, "%s does not damage %s in any way.", g.name, SHARK_NAME)
	add_message(g, "%s attacks %s.", SHARK_NAME, g.name)
	add_message(g, "%s kills %s.", SHARK_NAME, g.name)
	g.dead = true
}

// ---- ads -----------------------------------------------------------------------

ad_start :: proc(g: ^Game, now_ms: i64) {
	g.ad_active = true
	g.ad_finish_ms = now_ms + AD_BREAK_MS
}

// `sponsor_roll` is uniform in [0,1) and picks between the two sponsors.
ad_show :: proc(g: ^Game, now_ms: i64, sponsor_roll: f64) {
	clear_messages(g)
	if g.ad_finish_ms > now_ms {
		secs := (g.ad_finish_ms - now_ms) / 1000
		add_message(g, "Time left in ad break: %02d:%02d", secs / 60, secs % 60)
		add_message(g, "(This is a turn based game. As such, this counter will not automatically change. You have to click the OK button to refresh.)")
		if int(sponsor_roll * 2) == 0 {
			add_link(g, "For all yer umlauting needs! umlaut.fyi", "https://umlaut.fyi/")
		} else {
			add_link(g, "Everybody loves Pen 15!", "https://pen15.site/")
		}
	} else {
		add_message(g, "Ad break is complete! You may return to yer metaphor!")
		g.ad_active = false
	}
}
