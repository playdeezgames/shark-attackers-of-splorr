#+build js
package sharks

// Browser glue: the only file with browser imports. The page owns the DOM; Odin
// describes each screen through the dom_* imports and the page replays it.
// Events come back through the exported on_* procs.

import "base:runtime"
import "core:math/rand"
import "core:time"

foreign import host "env"

@(default_calling_convention = "contextless")
foreign host {
	dom_clear   :: proc() ---
	// kind: 0 text, 1 title, 2 link
	dom_element :: proc(kind: i32, text: [^]u8, text_len: i32, href: [^]u8, href_len: i32, new_line: i32) ---
	// kind: 0 choice list, 1 number box, 2 text box
	dom_prompt  :: proc(kind: i32, title: [^]u8, title_len: i32) ---
	dom_choice  :: proc(index: i32, text: [^]u8, text_len: i32) ---
	dom_end     :: proc() ---
	// Copies the text box into buf and returns its byte length.
	read_text   :: proc(buf: [^]u8, cap: i32) -> i32 ---
	// localStorage. storage_get copies the value into buf and returns its byte
	// length, or -1 when absent or unreadable (also when it does not fit).
	storage_get :: proc(key: [^]u8, key_len: i32, buf: [^]u8, cap: i32) -> i32 ---
	storage_set :: proc(key: [^]u8, key_len: i32, val: [^]u8, val_len: i32) ---
}

game: Game
ui: Ui

NAME_CAP :: MAX_NAME_BYTES
name_buf: [NAME_CAP]u8

// A save is at most 64 messages of 512 bytes plus a little; escapes can double
// that. Anything bigger is treated as absent.
SAVE_CAP :: 96 * 1024
save_buf: [SAVE_CAP]u8

now_ms :: proc() -> i64 {
	return time.now()._nsec / 1_000_000
}

env :: proc() -> Env {
	return Env{now_ms = now_ms(), shark_roll = rand.float64(), sponsor_roll = rand.float64()}
}

@(private = "file")
ptr :: proc(s: string) -> [^]u8 {
	return raw_data(s)
}

render :: proc() {
	v := make_view(&ui, &game)
	dom_clear()
	for e in v.elements {
		text, href := e.text, e.href
		dom_element(i32(e.kind), ptr(text), i32(len(text)), ptr(href), i32(len(href)), e.new_line ? 1 : 0)
	}
	title := v.prompt.title
	dom_prompt(i32(v.prompt.kind), ptr(title), i32(len(title)))
	for ch, i in v.prompt.choices {
		text := ch.text
		dom_choice(i32(i), ptr(text), i32(len(text)))
	}
	dom_end()
	free_all(context.temp_allocator)
}

persist :: proc() {
	text := save_to_string(&game, context.temp_allocator)
	if len(text) == 0 || len(text) > SAVE_CAP {
		return
	}
	key := SAVE_KEY
	storage_set(ptr(key), i32(len(key)), ptr(text), i32(len(text)))
}

// Resume straight into play when a game was saved, else start at the title.
restore :: proc() {
	key := SAVE_KEY
	n := int(storage_get(ptr(key), i32(len(key)), raw_data(save_buf[:]), SAVE_CAP))
	if n > 0 && n <= SAVE_CAP && load_from_string(&game, string(save_buf[:n])) && game.embarked {
		enter_play(&ui, &game, env())
		return
	}
	ui_start(&ui)
}

// Every event ends the same way: save, then draw.
settle :: proc() {
	persist()
	render()
}

main :: proc() {
	rand.reset(u64(time.now()._nsec))
	restore()
	settle()
}

@(export)
on_choice :: proc "c" (index: i32) {
	context = runtime.default_context()
	choose(&ui, &game, env(), int(index))
	settle()
}

@(export)
on_submit_number :: proc "c" (value: f64) {
	context = runtime.default_context()
	submit_number(&ui, &game, env(), value)
	settle()
}

@(export)
on_submit_text :: proc "c" () {
	context = runtime.default_context()
	n := int(read_text(raw_data(name_buf[:]), NAME_CAP))
	if n <= 0 || n > NAME_CAP {
		return
	}
	submit_text(&ui, &game, env(), string(name_buf[:n]))
	settle()
}

// Kept so odin.js does not end the program right after main.
@(export)
step :: proc(dt: f64, c: runtime.Context) -> bool {
	return true
}
