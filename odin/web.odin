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
}

game: Game
ui: Ui

NAME_CAP :: 64
name_buf: [NAME_CAP]u8

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

main :: proc() {
	rand.reset(u64(time.now()._nsec))
	ui_start(&ui)
	render()
}

@(export)
on_choice :: proc "c" (index: i32) {
	context = runtime.default_context()
	choose(&ui, &game, env(), int(index))
	render()
}

@(export)
on_submit_number :: proc "c" (value: f64) {
	context = runtime.default_context()
	submit_number(&ui, &game, env(), value)
	render()
}

@(export)
on_submit_text :: proc "c" () {
	context = runtime.default_context()
	n := int(read_text(raw_data(name_buf[:]), NAME_CAP))
	if n <= 0 || n > NAME_CAP {
		return
	}
	submit_text(&ui, &game, env(), string(name_buf[:n]))
	render()
}

// Kept so odin.js does not end the program right after main.
@(export)
step :: proc(dt: f64, c: runtime.Context) -> bool {
	return true
}
