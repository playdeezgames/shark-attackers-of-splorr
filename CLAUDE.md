# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

"Shark Attackers of SPLORR!!", a text-adventure "Metaphor" by TheGrumpyGameDev for the [Wacky Fun Game Jam of Joy and Whimsy](https://itch.io/jam/the-wacky-fun-game-jam-of-joy-and-whimsy). Live at https://thegrumpygamedev.itch.io/shark-attackers-of-splorr.

The jam build was VB.NET; it has been ported to Odin compiled to `js_wasm32` (`odin/`), the stack the author now uses for all new games. The port shipped to itch.io on October 7, 2026 (`html` channel, build #2080898) and the VB.NET `src/` was then deleted (see the last section). Read `docs/PORT_PLAN.md` first: it records the decisions (exact port, plain DOM, browser only, saving added on purpose) and the verified behavior of the original. `docs/QUIRKS.md` lists oddities of the original that were deliberately kept; log new ones there rather than fixing them silently.

An Obsidian vault with the author's cross-game knowledge lives at `/home/yermom/git/bok-of-splorr/splorr/` (outside this repo). Start with `Home.md`, then `Tech/Odin wasm recipe.md`, `Gotchas.md`, `Tech/Shipping to itch.io.md` and `Concepts/Metaphor design.md`. The vault has no note for this game yet. Its standing rules: never `git push` or run a ship script (`./shippit.sh --push`) unless the user says so, and do not "fix" deliberate design (deadpan text, harsh difficulty, the always-fatal shark fight) as if it were a bug.

Devlog entries live in `devlog/YYYYMMDD/devlog.md`, plain text for copy and paste in the author's deadpan voice, ending with "I wrote this with Claude Code, which did most of the typing." and "Thanks for checking." (same pattern as the author's other repos).

## Commands

Odin game (toolchain `dev-2026-07-nightly` at `/home/yermom/ODIN/odin`, override with `ODIN`):

```bash
odin/test.sh                                 # native tests (single thread, tracks leaks)
odin/test.sh -define:ODIN_TEST_NAMES=sharks.fight_always_kills   # one test
odin/build.sh                                # js_wasm32 build into odin/out
python3 -m http.server -d odin/out 8124      # serve it; pick a port nothing else uses
./shippit.sh                                 # test + build + zip to build/, never uploads
```

`./shippit.sh --push` uploads to itch.io (public), so only with an explicit yes from the user. Always run the wasm build as well as the tests: native `int` is 64-bit but 32-bit on `js_wasm32`.

## Architecture (Odin port, `odin/`)

One package `sharks`. The rules never import browser code, so everything but `web.odin` runs under `odin test`.

- `game.odin`: all rules on one plain `Game` struct (place, boat position/heading/speed, shark, dead, ad deadline, message log). Randomness and the clock are passed in (`shark_roll`, `now_ms`), never read inside, so tests control them. The shark chance is the original's weighted integer roll (`round_half_even(distance-10)` vs 10), not a simple percentage.
- `screens.odin`: the menu state machine (`Screen`, `choose`, `submit_text`, `submit_number`) and `make_view`. Mirrors `InPlay.Run` priority: ad, dead, combat, heading/speed prompt, navigation. Unavailable choices are omitted, not disabled. `Env` carries the clock and rolls into transitions.
- `save.odin`: JSON save/load with strict validation (enums as ints, range and consistency checks). `load_from_string` leaves the game untouched on failure.
- `web.odin` (`#+build js`): the only browser glue. Imports `dom_*`, storage and `read_text`; exports `on_choice`, `on_submit_number`, `on_submit_text`. Every event ends in `settle()` = save, then redraw. On start, a saved game resumes straight into play.
- `web/index.html`: replays the view into real DOM (`textContent` only) and implements the imports.
- Tests (`*_test.odin`, `#+build !js`): `game_test` (rules), `screens_test` (walkthroughs through the menu API), `save_test` (round trips and refusals).

Gotchas that bit here: Odin string literals in the view are fine but dynamic strings use `context.temp_allocator` and are valid only until `render()` frees it; `core:encoding/json` prints floats with about 16 digits, so compare positions with a tolerance.

## The original VB.NET game

Removed from the working tree once the port shipped. It is still in git history: the last commit that has `src/` is `9a5685b` (for example `git show 9a5685b:src/Metaphor.Extensions/WorldExtensions.vb`). Layers were `Provision`, `Persistence`, `Extensions` (all rules, as extension methods), `Models`, `Presentation` (dialog state machine), `Platform`, then Spectre console and Blazor front ends.
