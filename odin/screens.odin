package sharks

// The menu/dialog state machine, ported from Metaphor.Presentation. Pure: it
// reads and drives `Game` and produces a `View` for a front end to draw.
// Choices that are not available are left out of the list (the original's
// DialogPrompt drops disabled choices), they are never shown greyed out.

import "core:fmt"

Screen :: enum {
	Title,
	Main_Menu,
	Choose_Name,
	Game_Menu,
	Confirm_Abandon,
	Feature_Menu,
	// What InPlay routes to; picked once on entry, in this priority order.
	Ad_Menu,
	Dead_Menu,
	Combat_Menu,
	Heading_Prompt,
	Speed_Prompt,
	Nav_Menu,
}

Action :: enum {
	Title_Ok,
	Embark,
	Continue,
	Abandon,
	Confirm_No,
	Confirm_Yes,
	Never_Mind,
	Enter,
	Verb,
	Status,
	Look,
	Mooring,
	Watch_Ad,
	Game_Menu,
	Fight,
	Ad_Ok,
}

Choice :: struct {
	text:   string,
	action: Action,
	verb:   Verb, // only for .Verb
}

Prompt_Kind :: enum {
	Choice,
	Number,
	Text,
}

Prompt :: struct {
	kind:    Prompt_Kind,
	title:   string,
	choices: []Choice, // for .Choice
}

Element_Kind :: enum {
	Text,
	Title,
	Link,
}

Element :: struct {
	kind:     Element_Kind,
	text:     string,
	href:     string,
	new_line: bool,
}

// Elements and prompt text live in the temp allocator or in `Game` messages.
// Draw it before the next transition, then `free_all(context.temp_allocator)`.
View :: struct {
	elements: []Element,
	prompt:   Prompt,
}

Ui :: struct {
	screen: Screen,
}

// Things a transition may need from outside: the clock and two uniform rolls
// in [0,1) (the shark roll for Move, the sponsor pick for an ad break).
Env :: struct {
	now_ms:       i64,
	shark_roll:   f64,
	sponsor_roll: f64,
}

TITLE_TEXT :: "Shark Attackers of SPLORR!!"

ui_start :: proc(ui: ^Ui) {
	ui.screen = .Title
}

// ---- choices ------------------------------------------------------------------

@(private = "file")
c :: proc(text: string, action: Action) -> Choice {
	return Choice{text = text, action = action}
}

current_choices :: proc(ui: ^Ui, g: ^Game) -> []Choice {
	out := make([dynamic]Choice, context.temp_allocator)
	switch ui.screen {
	case .Title:
		append(&out, c("OK", .Title_Ok))
	case .Main_Menu:
		append(&out, c("Embark!", .Embark)) // Quit is disabled in the browser build
	case .Choose_Name, .Heading_Prompt, .Speed_Prompt:
	case .Game_Menu:
		append(&out, c("Continue", .Continue), c("Abandon", .Abandon))
	case .Confirm_Abandon:
		append(&out, c("No", .Confirm_No), c("Yes", .Confirm_Yes))
	case .Feature_Menu:
		append(&out, c("Never Mind", .Never_Mind), c("Enter", .Enter))
	case .Ad_Menu:
		append(&out, c("Ok", .Ad_Ok))
	case .Dead_Menu:
		append(&out, c("Watch Ad...", .Watch_Ad), c("Gämë Mënü", .Game_Menu))
	case .Combat_Menu:
		append(&out, c("Fight!", .Fight), c("Watch Ad...", .Watch_Ad), c("Gämë Mënü", .Game_Menu))
	case .Nav_Menu:
		for v in Verb {
			if can_perform(g, v) {
				append(&out, Choice{text = verb_name(v), action = .Verb, verb = v})
			}
		}
		append(&out, c("Status...", .Status))
		// Ground and Inventory are hidden: no items exist.
		if name, ok := mooring_name(g); ok {
			append(&out, c(fmt.tprintf("%s...", name), .Mooring))
		}
		append(&out, c("Look", .Look), c("Watch Ad...", .Watch_Ad), c("Gämë Mënü", .Game_Menu))
	}
	return out[:]
}

// ---- view ---------------------------------------------------------------------

@(private = "file")
message_elements :: proc(out: ^[dynamic]Element, g: ^Game) {
	for m in g.messages {
		append(out, Element{kind = m.href != "" ? .Link : .Text, text = m.text, href = m.href, new_line = true})
	}
}

make_view :: proc(ui: ^Ui, g: ^Game) -> View {
	elements := make([dynamic]Element, context.temp_allocator)
	v: View
	v.prompt.kind = .Choice
	v.prompt.choices = current_choices(ui, g)
	switch ui.screen {
	case .Title:
		append(&elements, Element{kind = .Title, text = TITLE_TEXT})
		append(&elements, Element{kind = .Text, text = "A Production of "})
		append(&elements, Element{kind = .Link, text = "TheGrumpyGameDev", href = "https://thegrumpygamedev.itch.io/", new_line = true})
		append(&elements, Element{kind = .Text, text = "For: "})
		append(&elements, Element{kind = .Link, text = "The Wacky Fun Game Jam of Joy and Whimsy", href = "https://itch.io/jam/the-wacky-fun-game-jam-of-joy-and-whimsy", new_line = true})
		append(&elements, Element{kind = .Text, text = "Sponsored by: ", new_line = true})
		append(&elements, Element{kind = .Link, text = "UMLAUT.FYI!", href = "https://umlaut.fyi/", new_line = true})
		append(&elements, Element{kind = .Link, text = "Pen 15!", href = "https://pen15.site/", new_line = true})
	case .Main_Menu:
		v.prompt.title = "Main Menu:"
	case .Choose_Name:
		v.prompt.kind = .Text
		v.prompt.title = "What is your name?"
	case .Game_Menu:
		v.prompt.title = "Game Menu:"
	case .Confirm_Abandon:
		v.prompt.title = "Are you sure you want to abandon?"
	case .Feature_Menu:
		message_elements(&elements, g)
		name, _ := mooring_name(g)
		v.prompt.title = fmt.tprintf("Do what with %s?", name)
	case .Ad_Menu:
		message_elements(&elements, g)
	case .Dead_Menu, .Combat_Menu, .Nav_Menu:
		message_elements(&elements, g)
		v.prompt.title = "Now What?"
	case .Heading_Prompt:
		v.prompt.kind = .Number
		v.prompt.title = "New Heading?"
	case .Speed_Prompt:
		v.prompt.kind = .Number
		v.prompt.title = "New Speed?"
	}
	v.elements = elements[:]
	return v
}

// ---- transitions ----------------------------------------------------------------

// InPlay.Run: decide which in-play screen to show. An ad break that is still
// running shows its countdown; one that has run out says so, and keeps its Ok
// button for this screen even though the break is already over.
enter_play :: proc(ui: ^Ui, g: ^Game, env: Env) {
	switch {
	case ad_in_progress(g):
		ad_show(g, env.now_ms, env.sponsor_roll)
		ui.screen = .Ad_Menu
	case g.dead:
		ui.screen = .Dead_Menu
	case in_combat(g):
		ui.screen = .Combat_Menu
	case g.mode == .Change_Heading:
		ui.screen = .Heading_Prompt
	case g.mode == .Change_Speed:
		ui.screen = .Speed_Prompt
	case:
		ui.screen = .Nav_Menu
	}
}

// Pick choice `index` of the current screen. Out-of-range indexes are ignored.
choose :: proc(ui: ^Ui, g: ^Game, env: Env, index: int) {
	choices := current_choices(ui, g)
	if index < 0 || index >= len(choices) {
		return
	}
	ch := choices[index]
	switch ch.action {
	case .Title_Ok:
		ui.screen = .Main_Menu
	case .Embark:
		ui.screen = .Choose_Name
	case .Continue, .Confirm_No, .Never_Mind, .Ad_Ok:
		enter_play(ui, g, env)
	case .Abandon:
		ui.screen = .Confirm_Abandon
	case .Confirm_Yes:
		abandon(g)
		ui.screen = .Main_Menu
	case .Enter:
		enter_mooring(g)
		enter_play(ui, g, env)
	case .Verb:
		perform(g, ch.verb, env.shark_roll)
		enter_play(ui, g, env)
	case .Status:
		show_status(g)
		enter_play(ui, g, env)
	case .Look:
		look_action(g)
		enter_play(ui, g, env)
	case .Mooring:
		describe_mooring(g)
		ui.screen = .Feature_Menu
	case .Watch_Ad:
		ad_start(g, env.now_ms)
		enter_play(ui, g, env)
	case .Game_Menu:
		ui.screen = .Game_Menu
	case .Fight:
		fight(g)
		enter_play(ui, g, env)
	}
}

submit_text :: proc(ui: ^Ui, g: ^Game, env: Env, text: string) {
	if ui.screen != .Choose_Name {
		return
	}
	embark(g, text)
	enter_play(ui, g, env)
}

submit_number :: proc(ui: ^Ui, g: ^Game, env: Env, value: f64) {
	#partial switch ui.screen {
	case .Heading_Prompt:
		set_heading(g, value)
		enter_play(ui, g, env)
	case .Speed_Prompt:
		set_speed(g, value)
		enter_play(ui, g, env)
	}
}
