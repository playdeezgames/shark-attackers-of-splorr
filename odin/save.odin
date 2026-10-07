package sharks

// Save and load as JSON text. Pure: the page stores the text. Everything the
// player can see is saved (the original never saved at all), so a reload
// resumes exactly where they were. Loading validates every field; anything
// odd is refused and the caller starts a fresh game.

import "core:encoding/json"
import "core:math"
import "core:strings"

SAVE_KEY :: "sao:save"
SAVE_VERSION :: 1

MAX_NAME_BYTES :: 64
MAX_SAVED_MESSAGES :: 64
MAX_MESSAGE_BYTES :: 512

@(private = "file")
Save_Message :: struct {
	text: string,
	href: string,
}

// Enums are stored as their integer values and range-checked on load, because
// unknown enum names would unmarshal silently.
@(private = "file")
Save_Data :: struct {
	version:      int,
	empty:        bool, // Abandon writes this marker instead of removing the key
	embarked:     bool,
	name:         string,
	at:           int,
	moored:       bool,
	x:            f64,
	y:            f64,
	heading:      f64,
	speed:        f64,
	shark:        bool,
	dead:         bool,
	mode:         int,
	ad_active:    bool,
	ad_finish_ms: i64,
	messages:     []Save_Message,
}

// Text for the page to store. Caller frees with `delete`.
save_to_string :: proc(g: ^Game, allocator := context.allocator) -> string {
	d := Save_Data{version = SAVE_VERSION}
	if !g.embarked {
		d.empty = true
		return marshal_save(&d, allocator)
	}
	d.embarked = true
	d.name = g.name
	d.at = int(g.at)
	d.moored = g.moored
	d.x, d.y, d.heading, d.speed = g.x, g.y, g.heading, g.speed
	d.shark = g.shark
	d.dead = g.dead
	d.mode = int(g.mode)
	d.ad_active = g.ad_active
	d.ad_finish_ms = g.ad_finish_ms

	// A long run of Set Heading / Set Speed piles up messages; keep the latest.
	first := max(0, len(g.messages) - MAX_SAVED_MESSAGES)
	msgs := make([]Save_Message, len(g.messages) - first, context.temp_allocator)
	for m, i in g.messages[first:] {
		msgs[i] = Save_Message{text = m.text, href = m.href}
	}
	d.messages = msgs
	return marshal_save(&d, allocator)
}

@(private = "file")
marshal_save :: proc(d: ^Save_Data, allocator := context.allocator) -> string {
	data, err := json.marshal(d^, {}, allocator)
	if err != nil {
		return ""
	}
	return string(data)
}

@(private = "file")
valid_text :: proc(s: string, max_bytes: int) -> bool {
	return len(s) <= max_bytes
}

// Fills `g` from saved text. Returns false and leaves `g` untouched when the
// text is missing, corrupt, from another version or fails validation.
// A valid "empty" marker loads fine and leaves `g` not embarked.
load_from_string :: proc(g: ^Game, text: string) -> bool {
	if len(text) == 0 {
		return false
	}
	d: Save_Data
	// The parsed form is thrown away with the temp allocator.
	if err := json.unmarshal_string(text, &d, allocator = context.temp_allocator); err != nil {
		return false
	}
	if d.version != SAVE_VERSION {
		return false
	}
	if d.empty {
		return !d.embarked
	}
	if !d.embarked {
		return false
	}
	// name
	if len(d.name) == 0 || !valid_text(d.name, MAX_NAME_BYTES) {
		return false
	}
	// enums
	if d.at < int(min(Place)) || d.at > int(max(Place)) {
		return false
	}
	if d.mode < int(min(Mode)) || d.mode > int(max(Mode)) {
		return false
	}
	// numbers
	for v in ([4]f64{d.x, d.y, d.heading, d.speed}) {
		if math.is_nan(v) || math.is_inf(v) {
			return false
		}
	}
	if d.heading < MIN_HEADING || d.heading > MAX_HEADING || d.speed < MIN_SPEED || d.speed > MAX_SPEED {
		return false
	}
	if d.ad_finish_ms < 0 {
		return false
	}
	// rules the game itself keeps
	place := Place(d.at)
	if place == .Pier && !d.moored { // the avatar can only be at the pier while moored
		return false
	}
	if place == .Pier && (d.x != 0 || d.y != 0) {
		return false
	}
	if d.dead && !d.shark {
		return false
	}
	if d.shark && place != .Boat {
		return false
	}
	if d.moored && (d.x != 0 || d.y != 0) {
		return false
	}
	mode := Mode(d.mode)
	if mode != .None && (place != .Boat || d.moored || d.dead || d.shark) {
		return false
	}
	// messages
	if len(d.messages) > MAX_SAVED_MESSAGES {
		return false
	}
	for m in d.messages {
		if !valid_text(m.text, MAX_MESSAGE_BYTES) || !valid_text(m.href, MAX_MESSAGE_BYTES) {
			return false
		}
	}

	// Everything checks out: commit.
	abandon(g)
	g.embarked = true
	g.name = strings.clone(d.name)
	g.at = place
	g.moored = d.moored
	g.x, g.y, g.heading, g.speed = d.x, d.y, d.heading, d.speed
	g.shark = d.shark
	g.dead = d.dead
	g.mode = mode
	g.ad_active = d.ad_active
	g.ad_finish_ms = d.ad_finish_ms
	for m in d.messages {
		append(&g.messages, Message{text = strings.clone(m.text), href = strings.clone(m.href)})
	}
	return true
}
