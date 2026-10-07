#+build !js
package sharks

import "core:testing"

// Drives the state machine the way a front end would.
Session :: struct {
	g:   Game,
	ui:  Ui,
	env: Env,
}

session_start :: proc() -> Session {
	s: Session
	ui_start(&s.ui)
	s.env = Env{now_ms = 1_000_000}
	return s
}

session_end :: proc(s: ^Session) {
	game_destroy(&s.g)
	free_all(context.temp_allocator)
}

choice_texts :: proc(s: ^Session) -> []string {
	cs := current_choices(&s.ui, &s.g)
	out := make([]string, len(cs), context.temp_allocator)
	for ch, i in cs {
		out[i] = ch.text
	}
	return out
}

// Choose by visible text. Returns false when that choice is not offered.
pick :: proc(s: ^Session, text: string) -> bool {
	for ch, i in current_choices(&s.ui, &s.g) {
		if ch.text == text {
			choose(&s.ui, &s.g, s.env, i)
			return true
		}
	}
	return false
}

expect_choices :: proc(t: ^testing.T, s: ^Session, want: []string, loc := #caller_location) {
	got := choice_texts(s)
	if len(got) != len(want) {
		testing.expectf(t, false, "choices %v, want %v", got, want, loc = loc)
		return
	}
	for w, i in want {
		testing.expectf(t, got[i] == w, "choices %v, want %v", got, want, loc = loc)
	}
}

// Title -> Main Menu -> name -> at the pier.
begin :: proc(t: ^testing.T, s: ^Session) {
	testing.expect(t, pick(s, "OK"))
	testing.expect(t, pick(s, "Embark!"))
	submit_text(&s.ui, &s.g, s.env, "Tester")
}

// Aboard the boat, unmoored.
cast_off :: proc(t: ^testing.T, s: ^Session) {
	testing.expect(t, pick(s, "Mooring from Pier to Blue Boat..."))
	testing.expect(t, pick(s, "Enter"))
	testing.expect(t, pick(s, "Unmoor"))
}

@(test)
title_then_main_menu_then_name :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	v := make_view(&s.ui, &s.g)
	testing.expect_value(t, s.ui.screen, Screen.Title)
	testing.expect_value(t, v.elements[0].kind, Element_Kind.Title)
	testing.expect_value(t, len(v.elements), 8)
	testing.expect_value(t, v.prompt.title, "")
	expect_choices(t, &s, {"OK"})

	pick(&s, "OK")
	v = make_view(&s.ui, &s.g)
	testing.expect_value(t, v.prompt.title, "Main Menu:")
	testing.expect_value(t, len(v.elements), 0)
	expect_choices(t, &s, {"Embark!"}) // Quit is hidden in the browser build

	pick(&s, "Embark!")
	v = make_view(&s.ui, &s.g)
	testing.expect_value(t, v.prompt.kind, Prompt_Kind.Text)
	testing.expect_value(t, v.prompt.title, "What is your name?")

	submit_text(&s.ui, &s.g, s.env, "Olen")
	testing.expect_value(t, s.ui.screen, Screen.Nav_Menu)
	testing.expect_value(t, s.g.name, "Olen")
}

@(test)
pier_menu_order :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	begin(t, &s)
	expect_choices(t, &s, {"Status...", "Mooring from Pier to Blue Boat...", "Look", "Watch Ad...", "Gämë Mënü"})
	v := make_view(&s.ui, &s.g)
	testing.expect_value(t, v.prompt.title, "Now What?")
	testing.expect_value(t, v.elements[0].text, "Welcome to Shark Attackers of SPLORR!!!")
}

@(test)
mooring_menu_and_enter :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	begin(t, &s)
	pick(&s, "Mooring from Pier to Blue Boat...")
	testing.expect_value(t, s.ui.screen, Screen.Feature_Menu)
	v := make_view(&s.ui, &s.g)
	testing.expect_value(t, v.prompt.title, "Do what with Mooring from Pier to Blue Boat?")
	testing.expect_value(t, v.elements[0].text, "This is a Mooring from Pier to Blue Boat.")
	expect_choices(t, &s, {"Never Mind", "Enter"})

	pick(&s, "Never Mind")
	testing.expect_value(t, s.ui.screen, Screen.Nav_Menu)
	testing.expect_value(t, s.g.at, Place.Pier)
	// messages are not cleared by Never Mind
	v = make_view(&s.ui, &s.g)
	testing.expect_value(t, v.elements[0].text, "This is a Mooring from Pier to Blue Boat.")

	pick(&s, "Mooring from Pier to Blue Boat...")
	pick(&s, "Enter")
	testing.expect_value(t, s.g.at, Place.Boat)
	expect_choices(t, &s, {"Unmoor", "Status...", "Mooring from Blue Boat to Pier...", "Look", "Watch Ad...", "Gämë Mënü"})
}

@(test)
boat_menu_when_unmoored :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	begin(t, &s)
	cast_off(t, &s)
	// still within 1.0 of the pier, so Moor is offered
	expect_choices(t, &s, {"Moor", "Move", "Set Heading...", "Set Speed...", "Status...", "Look", "Watch Ad...", "Gämë Mënü"})
	s.g.x = 5
	expect_choices(t, &s, {"Move", "Set Heading...", "Set Speed...", "Status...", "Look", "Watch Ad...", "Gämë Mënü"})
}

@(test)
heading_and_speed_prompts :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	begin(t, &s)
	cast_off(t, &s)

	pick(&s, "Set Heading...")
	v := make_view(&s.ui, &s.g)
	testing.expect_value(t, s.ui.screen, Screen.Heading_Prompt)
	testing.expect_value(t, v.prompt.kind, Prompt_Kind.Number)
	testing.expect_value(t, v.prompt.title, "New Heading?")
	submit_number(&s.ui, &s.g, s.env, 90)
	testing.expect_value(t, s.ui.screen, Screen.Nav_Menu)
	testing.expect_value(t, s.g.heading, 90.0)

	pick(&s, "Set Speed...")
	v = make_view(&s.ui, &s.g)
	testing.expect_value(t, v.prompt.title, "New Speed?")
	submit_number(&s.ui, &s.g, s.env, 0.5)
	testing.expect_value(t, s.g.speed, 0.5)
	testing.expect_value(t, s.ui.screen, Screen.Nav_Menu)
}

@(test)
status_and_look_return_to_the_menu :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	begin(t, &s)
	pick(&s, "Status...")
	testing.expect_value(t, s.ui.screen, Screen.Nav_Menu)
	v := make_view(&s.ui, &s.g)
	testing.expect_value(t, len(v.elements), 5)
	testing.expect_value(t, v.elements[0].text, "Status:")
	pick(&s, "Look")
	v = make_view(&s.ui, &s.g)
	testing.expect_value(t, v.elements[0].text, "Tester is at Pier.")
}

@(test)
sailing_into_a_shark_and_dying :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	begin(t, &s)
	cast_off(t, &s)
	s.g.x = 49 // the next Move lands 50 from the pier
	s.env.shark_roll = 0
	pick(&s, "Move")
	testing.expect_value(t, s.ui.screen, Screen.Combat_Menu)
	expect_choices(t, &s, {"Fight!", "Watch Ad...", "Gämë Mënü"})
	v := make_view(&s.ui, &s.g)
	testing.expect_value(t, v.prompt.title, "Now What?")

	pick(&s, "Fight!")
	testing.expect_value(t, s.ui.screen, Screen.Dead_Menu)
	expect_choices(t, &s, {"Watch Ad...", "Gämë Mënü"})
	v = make_view(&s.ui, &s.g)
	testing.expect_value(t, v.elements[3].text, "Shark kills Tester.")
}

@(test)
no_shark_means_the_menu_stays_normal :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	begin(t, &s)
	cast_off(t, &s)
	s.env.shark_roll = 0.999
	for _ in 0 ..< 5 {
		testing.expect(t, pick(&s, "Move"))
		testing.expect_value(t, s.ui.screen, Screen.Nav_Menu)
	}
	testing.expect_value(t, s.g.x, 5.0)
}

@(test)
game_menu_continue_and_abandon :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	begin(t, &s)
	pick(&s, "Gämë Mënü")
	v := make_view(&s.ui, &s.g)
	testing.expect_value(t, v.prompt.title, "Game Menu:")
	testing.expect_value(t, len(v.elements), 0)
	expect_choices(t, &s, {"Continue", "Abandon"})
	pick(&s, "Continue")
	testing.expect_value(t, s.ui.screen, Screen.Nav_Menu)

	pick(&s, "Gämë Mënü")
	pick(&s, "Abandon")
	v = make_view(&s.ui, &s.g)
	testing.expect_value(t, v.prompt.title, "Are you sure you want to abandon?")
	expect_choices(t, &s, {"No", "Yes"})
	pick(&s, "No")
	testing.expect_value(t, s.ui.screen, Screen.Nav_Menu)
	testing.expect(t, s.g.embarked)

	pick(&s, "Gämë Mënü")
	pick(&s, "Abandon")
	pick(&s, "Yes")
	testing.expect_value(t, s.ui.screen, Screen.Main_Menu)
	testing.expect(t, !s.g.embarked)
	expect_choices(t, &s, {"Embark!"})
}

@(test)
ad_break_blocks_play_until_ok :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	begin(t, &s)
	pick(&s, "Watch Ad...")
	testing.expect_value(t, s.ui.screen, Screen.Ad_Menu)
	expect_choices(t, &s, {"Ok"})
	v := make_view(&s.ui, &s.g)
	testing.expect_value(t, v.prompt.title, "")
	testing.expect_value(t, v.elements[0].text, "Time left in ad break: 02:00")
	testing.expect_value(t, v.elements[2].kind, Element_Kind.Link)

	// Ok before the time is up refreshes the countdown
	s.env.now_ms += 30_000
	pick(&s, "Ok")
	testing.expect_value(t, s.ui.screen, Screen.Ad_Menu)
	v = make_view(&s.ui, &s.g)
	testing.expect_value(t, v.elements[0].text, "Time left in ad break: 01:30")

	// After the deadline: the completion text, still on the Ad menu with Ok
	s.env.now_ms += 100_000
	pick(&s, "Ok")
	testing.expect_value(t, s.ui.screen, Screen.Ad_Menu)
	v = make_view(&s.ui, &s.g)
	testing.expect_value(t, v.elements[0].text, "Ad break is complete! You may return to yer metaphor!")
	testing.expect(t, !ad_in_progress(&s.g))
	pick(&s, "Ok")
	testing.expect_value(t, s.ui.screen, Screen.Nav_Menu)
}

@(test)
ad_can_be_watched_when_dead_and_in_combat :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	begin(t, &s)
	cast_off(t, &s)
	s.g.shark = true
	enter_play(&s.ui, &s.g, s.env)
	testing.expect_value(t, s.ui.screen, Screen.Combat_Menu)
	pick(&s, "Watch Ad...")
	testing.expect_value(t, s.ui.screen, Screen.Ad_Menu)
	s.env.now_ms += AD_BREAK_MS
	pick(&s, "Ok") // shows "complete"
	pick(&s, "Ok")
	testing.expect_value(t, s.ui.screen, Screen.Combat_Menu)
	pick(&s, "Fight!")
	pick(&s, "Watch Ad...")
	s.env.now_ms += AD_BREAK_MS
	pick(&s, "Ok")
	pick(&s, "Ok")
	testing.expect_value(t, s.ui.screen, Screen.Dead_Menu)
}

@(test)
bad_input_is_ignored :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	choose(&s.ui, &s.g, s.env, 7)
	choose(&s.ui, &s.g, s.env, -1)
	testing.expect_value(t, s.ui.screen, Screen.Title)
	submit_text(&s.ui, &s.g, s.env, "x") // not on the name screen
	submit_number(&s.ui, &s.g, s.env, 1)
	testing.expect_value(t, s.ui.screen, Screen.Title)
	testing.expect(t, !s.g.embarked)
}

@(test)
full_run_back_to_main_menu_and_replay :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	begin(t, &s)
	for round in 0 ..< 2 {
		cast_off(t, &s)
		s.g.x = 49
		pick(&s, "Move")
		pick(&s, "Fight!")
		testing.expect_value(t, s.ui.screen, Screen.Dead_Menu)
		pick(&s, "Gämë Mënü")
		pick(&s, "Abandon")
		pick(&s, "Yes")
		testing.expect_value(t, s.ui.screen, Screen.Main_Menu)
		// after Abandon the world is clean: Embark again starts alive at the pier
		pick(&s, "Embark!")
		submit_text(&s.ui, &s.g, s.env, "Again")
		testing.expect(t, !s.g.dead)
		testing.expect(t, !s.g.shark)
		testing.expect_value(t, s.g.at, Place.Pier)
		testing.expect_value(t, s.g.name, "Again")
		testing.expect_value(t, s.ui.screen, Screen.Nav_Menu)
	}
}

@(test)
title_links_to_the_working_jam_address :: proc(t: ^testing.T) {
	s := session_start()
	defer session_end(&s)
	v := make_view(&s.ui, &s.g)
	found := false
	for e in v.elements {
		if e.kind == .Link && e.text == "The Wacky Fun Game Jam of Joy and Whimsy" {
			found = true
			testing.expect_value(t, e.href, "https://itch.io/jam/the-wacky-fun-game-jam-of-joy-and-whimsy")
		}
	}
	testing.expect(t, found)
}
