#+build !js
package sharks

import "core:testing"

new_game :: proc() -> Game {
	g: Game
	embark(&g, "Tester")
	return g
}

// Walk aboard and cast off.
at_sea :: proc(g: ^Game) {
	enter_mooring(g)
	perform(g, .Unmoor)
}

text_at :: proc(g: ^Game, i: int) -> string {
	return g.messages[i].text
}

@(test)
embark_starts_moored_at_pier :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	testing.expect(t, g.embarked)
	testing.expect_value(t, g.at, Place.Pier)
	testing.expect(t, g.moored)
	testing.expect_value(t, g.heading, 0.0)
	testing.expect_value(t, g.speed, 1.0)
	testing.expect_value(t, len(g.messages), 4)
	testing.expect_value(t, text_at(&g, 0), "Welcome to Shark Attackers of SPLORR!!!")
	testing.expect_value(t, text_at(&g, 1), "Tester is at Pier.")
	testing.expect_value(t, text_at(&g, 2), "Features:")
	testing.expect_value(t, text_at(&g, 3), "- Mooring from Pier to Blue Boat")
}

@(test)
pier_has_no_verbs :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	for v in Verb {
		testing.expect(t, !can_perform(&g, v))
	}
}

@(test)
describe_and_enter_mooring :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	describe_mooring(&g)
	testing.expect_value(t, len(g.messages), 1)
	testing.expect_value(t, text_at(&g, 0), "This is a Mooring from Pier to Blue Boat.")

	enter_mooring(&g)
	testing.expect_value(t, g.at, Place.Boat)
	testing.expect_value(t, text_at(&g, 0), "Tester uses Mooring from Pier to Blue Boat.")
	testing.expect_value(t, text_at(&g, 1), "Tester is at Blue Boat.")
	testing.expect_value(t, text_at(&g, 2), "Location: (0.00,0.00)")
	testing.expect_value(t, text_at(&g, 3), "Moored to pier")
	testing.expect_value(t, text_at(&g, 4), "Features:")
	testing.expect_value(t, text_at(&g, 5), "- Mooring from Blue Boat to Pier")

	enter_mooring(&g)
	testing.expect_value(t, g.at, Place.Pier)
}

@(test)
verb_availability_follows_mooring :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	enter_mooring(&g)
	testing.expect(t, can_perform(&g, .Unmoor))
	testing.expect(t, !can_perform(&g, .Moor))
	testing.expect(t, !can_perform(&g, .Move))
	testing.expect(t, !can_perform(&g, .Set_Heading))
	testing.expect(t, !can_perform(&g, .Set_Speed))

	perform(&g, .Unmoor)
	testing.expect(t, !g.moored)
	testing.expect(t, !can_perform(&g, .Unmoor))
	testing.expect(t, can_perform(&g, .Move))
	testing.expect(t, can_perform(&g, .Set_Heading))
	testing.expect(t, can_perform(&g, .Set_Speed))
	// still at the pier, within 1.0
	testing.expect(t, can_perform(&g, .Moor))
}

@(test)
unmooring_removes_the_mooring_feature :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	at_sea(&g)
	_, ok := mooring_name(&g)
	testing.expect(t, !ok)
	testing.expect_value(t, text_at(&g, 0), "Tester unmoors Blue Boat.")
	testing.expect_value(t, text_at(&g, 1), "Tester is at Blue Boat.")
	testing.expect_value(t, text_at(&g, 2), "Location: (0.00,0.00)")
	testing.expect_value(t, text_at(&g, 3), "Current heading: 0.00°")
	testing.expect_value(t, text_at(&g, 4), "Current speed: 1.00")
	testing.expect_value(t, text_at(&g, 5), "Distance to pier: 0.00")
	testing.expect_value(t, len(g.messages), 7) // plus the heading-to-pier line
}

@(test)
move_advances_along_heading :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	at_sea(&g)
	perform(&g, .Move)
	testing.expect_value(t, g.x, 1.0)
	testing.expect(t, abs(g.y) < 1e-9)
	testing.expect_value(t, text_at(&g, 0), "Blue Boat moves.")

	set_heading(&g, 90)
	set_speed(&g, 0.5)
	perform(&g, .Move)
	testing.expect(t, abs(g.x - 1.0) < 1e-9)
	testing.expect(t, abs(g.y - 0.5) < 1e-9)
}

@(test)
moor_needs_the_pier_within_one :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	at_sea(&g)
	g.x = 1.0
	testing.expect(t, !can_perform(&g, .Moor)) // exactly 1.0 is not < 1.0
	g.x = 0.99
	testing.expect(t, can_perform(&g, .Moor))
	perform(&g, .Moor)
	testing.expect(t, g.moored)
	testing.expect_value(t, text_at(&g, 0), "Tester moors Blue Boat to Pier.")
}

@(test)
heading_and_speed_are_clamped :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	at_sea(&g)
	set_heading(&g, -50)
	testing.expect_value(t, g.heading, 0.0)
	set_heading(&g, 400)
	testing.expect_value(t, g.heading, 360.0)
	set_speed(&g, 0)
	testing.expect_value(t, g.speed, 0.1)
	set_speed(&g, 5)
	testing.expect_value(t, g.speed, 1.0)
}

@(test)
set_prompts_use_modes_and_keep_old_messages :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	at_sea(&g)
	perform(&g, .Set_Heading)
	testing.expect_value(t, g.mode, Mode.Change_Heading)
	testing.expect_value(t, len(g.messages), 0) // verbs clear messages
	perform(&g, .Set_Speed)
	testing.expect_value(t, g.mode, Mode.Change_Speed)
	set_speed(&g, 0.5)
	testing.expect_value(t, g.mode, Mode.None)
	before := len(g.messages)
	set_heading(&g, 45) // Look appends, it does not clear
	testing.expect(t, len(g.messages) > before)
}

@(test)
heading_to_pier :: proc(t: ^testing.T) {
	testing.expect(t, abs(heading_between(5, 0, 0, 0) - 180) < 1e-9)
	testing.expect(t, abs(heading_between(0, 5, 0, 0) - 270) < 1e-9)
	testing.expect(t, abs(heading_between(0, -5, 0, 0) - 90) < 1e-9)
	testing.expect(t, abs(heading_between(-5, 0, 0, 0) - 0) < 1e-9)
	testing.expect(t, abs(distance_between(3, 4, 0, 0) - 5) < 1e-9)
}

@(test)
banker_rounding_matches_cint :: proc(t: ^testing.T) {
	testing.expect_value(t, round_half_even(0.5), 0)
	testing.expect_value(t, round_half_even(1.5), 2)
	testing.expect_value(t, round_half_even(2.5), 2)
	testing.expect_value(t, round_half_even(2.4), 2)
	testing.expect_value(t, round_half_even(2.6), 3)
	testing.expect_value(t, round_half_even(0.0), 0)
}

@(test)
no_shark_closer_than_ten :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	at_sea(&g)
	g.x = 9.9
	testing.expect(t, !spawn_shark(&g, 0.999999))
	testing.expect(t, !g.shark)
	g.x = 10 // weight 0: never
	testing.expect(t, !spawn_shark(&g, 0.0))
}

@(test)
shark_odds_follow_the_weights :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	at_sea(&g)
	// distance 30: true weight 20 of 30
	g.x = 30
	testing.expect_value(t, shark_weight(30), 20)
	testing.expect(t, spawn_shark(&g, 0.0))
	g.shark = false
	testing.expect(t, spawn_shark(&g, 0.66)) // 19/30 < 20/30
	g.shark = false
	testing.expect(t, !spawn_shark(&g, 0.67)) // 20/30
	testing.expect(t, !g.shark)
}

@(test)
shark_weight_rounds_half_to_even :: proc(t: ^testing.T) {
	testing.expect_value(t, shark_weight(10.5), 0)
	testing.expect_value(t, shark_weight(11.5), 2)
	testing.expect_value(t, shark_weight(12.5), 2)
}

@(test)
shark_appearing_puts_avatar_in_combat :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	at_sea(&g)
	g.x = 49 // after the move: 50 from the pier, weight 40 of 50
	perform(&g, .Move, 0.0)
	testing.expect(t, g.shark)
	testing.expect(t, in_combat(&g))
	testing.expect_value(t, text_at(&g, 0), "Blue Boat moves.")
	testing.expect_value(t, text_at(&g, 1), "Shark appears!")
}

@(test)
fight_always_kills :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	at_sea(&g)
	g.shark = true
	fight(&g)
	testing.expect(t, g.dead)
	testing.expect_value(t, len(g.messages), 4)
	testing.expect_value(t, text_at(&g, 0), "Tester attacks Shark.")
	testing.expect_value(t, text_at(&g, 1), "Tester does not damage Shark in any way.")
	testing.expect_value(t, text_at(&g, 2), "Shark attacks Tester.")
	testing.expect_value(t, text_at(&g, 3), "Shark kills Tester.")
}

@(test)
fight_does_nothing_without_a_shark :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	at_sea(&g)
	fight(&g)
	testing.expect(t, !g.dead)
}

@(test)
ad_break_counts_down_and_finishes :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	ad_start(&g, 1_000_000)
	testing.expect(t, ad_in_progress(&g))

	ad_show(&g, 1_000_000, 0.0)
	testing.expect_value(t, text_at(&g, 0), "Time left in ad break: 02:00")
	testing.expect_value(t, g.messages[2].href, "https://umlaut.fyi/")
	testing.expect(t, ad_in_progress(&g))

	ad_show(&g, 1_000_000 + 75_500, 0.9)
	testing.expect_value(t, text_at(&g, 0), "Time left in ad break: 00:44")
	testing.expect_value(t, g.messages[2].href, "https://pen15.site/")

	ad_show(&g, 1_000_000 + AD_BREAK_MS, 0.0) // the deadline itself is over
	testing.expect_value(t, text_at(&g, 0), "Ad break is complete! You may return to yer metaphor!")
	testing.expect_value(t, len(g.messages), 1)
	testing.expect(t, !ad_in_progress(&g))
}

@(test)
abandon_resets_everything_including_the_ad :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	at_sea(&g)
	g.shark = true
	fight(&g)
	ad_start(&g, 5)
	abandon(&g)
	testing.expect(t, !g.embarked)
	testing.expect(t, !g.dead)
	testing.expect(t, !g.shark)
	testing.expect(t, !ad_in_progress(&g))
	testing.expect_value(t, len(g.messages), 0)
}

@(test)
status_and_look_clear_first :: proc(t: ^testing.T) {
	g := new_game()
	defer game_destroy(&g)
	show_status(&g)
	testing.expect_value(t, len(g.messages), 1)
	testing.expect_value(t, text_at(&g, 0), "Status:")
	look_action(&g)
	testing.expect_value(t, text_at(&g, 0), "Tester is at Pier.")
}
