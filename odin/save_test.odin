#+build !js
package sharks

import "core:strings"
import "core:testing"

saved :: proc(g: ^Game) -> string {
	return save_to_string(g, context.temp_allocator)
}

@(test)
round_trip_at_sea :: proc(t: ^testing.T) {
	defer free_all(context.temp_allocator)
	a := new_game()
	defer game_destroy(&a)
	at_sea(&a)
	set_heading(&a, 123.456)
	set_speed(&a, 0.25)
	perform(&a, .Move)
	text := saved(&a)

	b: Game
	defer game_destroy(&b)
	testing.expect(t, load_from_string(&b, text))
	testing.expect_value(t, b.name, a.name)
	testing.expect_value(t, b.at, a.at)
	testing.expect_value(t, b.moored, a.moored)
	testing.expect(t, abs(b.x - a.x) < 1e-12) // JSON keeps about 16 digits
	testing.expect(t, abs(b.y - a.y) < 1e-12) // JSON keeps about 16 digits
	testing.expect(t, abs(b.heading - a.heading) < 1e-12) // JSON keeps about 16 digits
	testing.expect(t, abs(b.speed - a.speed) < 1e-12) // JSON keeps about 16 digits
	testing.expect_value(t, len(b.messages), len(a.messages))
	for m, i in a.messages {
		testing.expect_value(t, b.messages[i].text, m.text)
	}
	testing.expect_value(t, saved(&b), text) // saving what was loaded is byte-identical
}

@(test)
round_trip_dead_with_an_ad_running :: proc(t: ^testing.T) {
	defer free_all(context.temp_allocator)
	a := new_game()
	defer game_destroy(&a)
	at_sea(&a)
	a.shark = true
	fight(&a)
	ad_start(&a, 1_790_000_000_123)
	ad_show(&a, 1_790_000_000_123, 0.9)
	text := saved(&a)

	b: Game
	defer game_destroy(&b)
	testing.expect(t, load_from_string(&b, text))
	testing.expect(t, b.dead)
	testing.expect(t, b.shark)
	testing.expect(t, b.ad_active)
	testing.expect_value(t, b.ad_finish_ms, i64(1_790_000_000_123) + AD_BREAK_MS) // a big i64 survives
	testing.expect_value(t, b.messages[2].href, "https://pen15.site/")
}

@(test)
round_trip_in_a_numeric_prompt :: proc(t: ^testing.T) {
	defer free_all(context.temp_allocator)
	a := new_game()
	defer game_destroy(&a)
	at_sea(&a)
	perform(&a, .Set_Heading)
	b: Game
	defer game_destroy(&b)
	testing.expect(t, load_from_string(&b, saved(&a)))
	testing.expect_value(t, b.mode, Mode.Change_Heading)
}

@(test)
unicode_names_survive :: proc(t: ^testing.T) {
	defer free_all(context.temp_allocator)
	a: Game
	defer game_destroy(&a)
	embark(&a, "Zoë \"Ünï\" \\ ☃")
	b: Game
	defer game_destroy(&b)
	testing.expect(t, load_from_string(&b, saved(&a)))
	testing.expect_value(t, b.name, "Zoë \"Ünï\" \\ ☃")
}

@(test)
abandoned_game_saves_an_empty_marker :: proc(t: ^testing.T) {
	defer free_all(context.temp_allocator)
	a := new_game()
	defer game_destroy(&a)
	abandon(&a)
	text := saved(&a)
	testing.expect(t, strings.contains(text, "\"empty\":true"))
	b: Game
	defer game_destroy(&b)
	testing.expect(t, load_from_string(&b, text))
	testing.expect(t, !b.embarked)
}

@(test)
long_message_logs_keep_the_latest :: proc(t: ^testing.T) {
	defer free_all(context.temp_allocator)
	a := new_game()
	defer game_destroy(&a)
	at_sea(&a)
	for i in 0 ..< 100 {
		set_heading(&a, f64(i)) // each Look appends and nothing clears
	}
	b: Game
	defer game_destroy(&b)
	testing.expect(t, load_from_string(&b, saved(&a)))
	testing.expect_value(t, len(b.messages), MAX_SAVED_MESSAGES)
	testing.expect_value(t, b.messages[MAX_SAVED_MESSAGES - 1].text, a.messages[len(a.messages) - 1].text)
}

@(test)
refuses_missing_corrupt_and_foreign_text :: proc(t: ^testing.T) {
	defer free_all(context.temp_allocator)
	good := new_game()
	defer game_destroy(&good)
	text := saved(&good)

	bad := [?]string{
		"",
		"not json",
		"{",
		text[:len(text) / 2],
		"{}",
		`{"worldData":{"x":1}}`,
		`{"version":2,"empty":true}`,
		`{"version":1,"embarked":false}`,
		`{"version":1,"empty":true,"embarked":true}`,
	}
	for s in bad {
		g: Game
		testing.expectf(t, !load_from_string(&g, s), "accepted %q", s)
		testing.expect(t, !g.embarked)
		game_destroy(&g)
	}
}

@(test)
refuses_impossible_states :: proc(t: ^testing.T) {
	defer free_all(context.temp_allocator)
	a := new_game()
	defer game_destroy(&a)
	at_sea(&a)
	good := saved(&a)

	// Each edit turns a valid save into a state the game cannot reach.
	edits := [?][2]string{
		{`"at":1`, `"at":2`},
		{`"at":1`, `"at":-1`},
		{`"mode":0`, `"mode":3`},
		{`"speed":1`, `"speed":5`},
		{`"speed":1`, `"speed":0`},
		{`"heading":0`, `"heading":361`},
		{`"heading":0`, `"heading":-1`},
		{`"name":"Tester"`, `"name":""`},
		{`"dead":false`, `"dead":true`}, // dead without a shark
		{`"ad_finish_ms":0`, `"ad_finish_ms":-5`},
		{`"x":0`, `"x":1e999`},
	}
	for e in edits {
		text, _ := strings.replace(good, e[0], e[1], 1, context.temp_allocator)
		if text == good {
			testing.expectf(t, false, "edit %q did not apply to %s", e[0], good)
			continue
		}
		g: Game
		ok := load_from_string(&g, text)
		testing.expectf(t, !ok, "accepted %s", text)
		game_destroy(&g)
	}

	// Pier states that cannot happen: unmoored at the pier, or away from the origin.
	p := new_game()
	defer game_destroy(&p)
	ptext := saved(&p)
	for e in ([?][2]string{{`"moored":true`, `"moored":false`}, {`"x":0`, `"x":3`}}) {
		text, _ := strings.replace(ptext, e[0], e[1], 1, context.temp_allocator)
		g: Game
		testing.expectf(t, !load_from_string(&g, text), "accepted %s", text)
		game_destroy(&g)
	}
}

@(test)
oversized_text_is_refused :: proc(t: ^testing.T) {
	defer free_all(context.temp_allocator)
	a: Game
	defer game_destroy(&a)
	long, _ := strings.repeat("n", MAX_NAME_BYTES + 1, context.temp_allocator)
	embark(&a, long)
	b: Game
	defer game_destroy(&b)
	testing.expect(t, !load_from_string(&b, saved(&a)))
}

@(test)
a_failed_load_leaves_a_running_game_alone :: proc(t: ^testing.T) {
	defer free_all(context.temp_allocator)
	g := new_game()
	defer game_destroy(&g)
	testing.expect(t, !load_from_string(&g, "garbage"))
	testing.expect_value(t, g.name, "Tester")
	testing.expect(t, g.embarked)
}
