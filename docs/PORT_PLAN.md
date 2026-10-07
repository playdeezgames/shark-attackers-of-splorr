# Port plan: VB.NET to Odin / `js_wasm32`

Status: draft for review. Nothing in `src/` changes until the open questions at the end are answered.

## 1. What the game actually is

The VB.NET code is about 5,000 lines, but the game is small. The size comes from a generic entity/dialog framework (`TGGD.*`). The game itself is:

- **Title screen** with sponsor links, then **Main Menu**: Embark!, Quit (Quit is disabled in the browser build).
- **Embark**: ask for a name (the default constant `Olen Kyrpa` is never used). The world is created: a Pier at (0,0), a Blue Boat at (0,0) moored to the pier, the avatar. Message "Welcome to Shark Attackers of SPLORR!!!", then a Look.
- **Navigation menu** (`Now What?`), in order: the boat's verbs (Unmoor, Moor, Move, Set Heading..., Set Speed...), Status, Ground, Inventory, characters here, features here, Look, Watch Ad, Game Menu. Disabled choices are **filtered out, not shown** (`DialogPrompt` drops `Enabled = False`), so Ground and Inventory never appear (no items exist) and characters never appear outside combat.
- **Boat rules**
  - Heading is 0 to 360, speed is 0.1 to 1.0, starting at 0 and 1.0.
  - Move, Set Heading and Set Speed need an unmoored boat; Unmoor needs a moored one; Moor needs an unmoored boat within 1.0 of the pier.
  - Move advances `(x,y)` by `cos/sin(heading°) * speed`, then `SpawnShark`, then Look.
  - Look shows position, and when unmoored heading, speed, distance and heading to the pier.
- **Sharks**: after each Move, if the boat is at least 10 from the pier, a shark appears with probability `(d-10)/d` (weights: false=10, true=d-10). It is an enemy character in the same location.
- **Combat**: with an enemy present the menu is only Fight!, Watch Ad, Game Menu. Fight always ends the same way: no damage dealt, the shark kills the avatar. This is the point of the Metaphor, so do not add a way to win.
- **Death**: the menu is only Watch Ad and Game Menu. Game Menu: Continue, Abandon (confirm), which clears the world and returns to the Main Menu.
- **Ads**: Watch Ad starts a 2-minute real-time break, available at any time. While it runs, InPlay shows only the ad screen with an OK button that refreshes the countdown (turn-based, no timer) and a random sponsor link (umlaut.fyi or pen15.site, equal weight). When the time is up it says so and clears the break. This is the sponsor mechanic, so it stays.
- **Persistence**: none in practice. `World.Save` exists but nothing ever calls it, so the original never writes a save; on start `Load` reads `save.json` (absent), fails, and an empty world is used. See section 1b.

## 1b. Verified behavior (read from the VB code; supersedes section 1 where they differ)

**The avatar starts at the Pier, not on the boat.** Embark builds the Pier and a moored Blue Boat, both at (0,0). Each has a "Mooring from X to Y" feature with an `Enter` verb, a pair that are twins (removing one removes the other).

Flow:
1. Embark: world cleared, Pier/avatar/boat created, message `Welcome to Shark Attackers of SPLORR!!!`, then Look: `<name> is at Pier.`, `Features:`, `- Mooring from Pier to Blue Boat`. (Pier has no describer, so no location line.)
2. Pier menu (`Now What?`): `Status...`, `Mooring from Pier to Blue Boat...`, `Look`, `Watch Ad...`, `Game Menu`. Choosing the mooring clears messages, prints `This is a Mooring from Pier to Blue Boat.`, and shows `Do what with Mooring from Pier to Blue Boat?`: `Never Mind`, `Enter` (`Items...` is hidden, empty). Enter prints `<name> uses <feature>.`, moves the avatar to the boat, then Look (boat describer: `Location: (0.00,0.00)`, `Moored to pier`, features list).
3. Boat menu, moored: `Unmoor`, `Status...`, the boat's mooring `...`, `Look`, `Watch Ad...`, `Game Menu`. Unmoored: `Move`, `Set Heading...`, `Set Speed...` (and `Moor` only within 1.0 of the pier), `Status...`, `Look`, `Watch Ad...`, `Game Menu`. Order is location verbs in creation order (Unmoor, Moor, Move, Set Heading, Set Speed), then avatar verbs (none), Status, Ground, Inventory, characters, features, Look, Watch Ad, Game Menu.
4. Unmoor removes the boat's mooring (and its twin on the pier), so the avatar cannot return to the pier until moored again. Moor creates both again.
5. Set Heading / Set Speed: a mode prompt `New Heading?` / `New Speed?` (number). The raw value is stored and every read **clamps** (heading 0..360, speed 0.1..1.0), so out-of-range input is silently clamped. Afterwards it calls Look **without clearing messages**, so earlier messages stay and the new look is appended.
6. Message clearing is exactly: location/feature/character verb Perform, Status, Look (menu action), Fight, feature Describe, ad Show, Embark/Abandon. Everything else appends. Messages render above every `PickerMenu`, so they persist across Never Mind and the like.
7. Move: `Blue Boat moves.`, advance, shark roll (may add `Shark appears!`), then Look. Look text order: `<name> is at <location>.`, the location line(s), then `Features:` list if any.
8. Combat, death, ads, abandon: as in section 1. `DonePrompt` (an "Ok" leading to Abandon) is dead code. Status prints only `Status:`.
9. Browser build is created with `quittable = false`; the Spectre build with `true`.

**There is no save or load.** `World.Save` is never called anywhere, so an Embarked game and the ad deadline vanish on reload in both front ends. Under the exact-port rule (decision 2) the port should therefore **not save**. Decision 3 (new save key) only matters if saving is added; recorded in `QUIRKS.md` as a candidate for after the port. Phase 4 below is revised accordingly.

Remaining unverified: exact Spectre/Blazor rendering differences, and whether the first Look after Embark is rendered before or after the welcome line (it is after: Welcome is added first).

## 2. Target architecture

Follow the vault's "Roomba Rights" and "Check Yer Butthole" variants, since this game is a menu-driven text game with real buttons:

```
odin/                         (new top-level dir; src/ stays until the port ships)
  game.odin                   pure rules and state, no browser imports
  screens.odin                menu/dialog state machine, builds a View
  save.odin                   JSON (de)serialization of the state
  web.odin        #+build js  foreign imports, exports, step
  game_test.odin  #+build !js tests
  web/index.html, shim JS     DOM renderer, storage, link handling
  build.sh, shippit.sh
```

Key decisions (recommended, change if you disagree):

1. **Presentation: plain DOM, event driven** (the "Check Yer Butthole" variant). The game is a list of text lines plus a menu of buttons plus an optional text input and a number input. Real HTML links work for the sponsor ads and the title, and the page has no frame loop beyond a trivial `step`. The alternatives are a canvas text grid (the bus game's variant; the unused `IGrid` and the "text grid forerunner" commit hint at this idea) and a pure-text terminal clone. The grid is a bigger presentation change than a port needs; revisit only if you want the CoCo look.
2. **Collapse the entity framework into plain structs.** Replace generic entities, tags, yokes and dimensions with a fixed `Game` struct: `phase` (title, main menu, in play, ...), `avatar_name`, `boat {x, y, heading, speed, moored}`, `shark_present`, `avatar_dead`, `ad_finish` (ms since epoch), `messages`. Keep the same rule order and wording. The pier is the origin and needs no entity.
3. **Dialogs become an enum state plus a `view(game) -> View` proc and `choose(game, index)` / `submit_text` / `submit_number`.** The `DialogSource` continuation chain and the `InPlay.Run()` router (ad, then dead, then combat, then heading/speed prompt, else navigation) map directly to a `switch` on state, with the same priority order.
4. **Randomness**: seed in `main` as the vault recommends, and give the shark roll a proc that takes the roll as a parameter so tests can force it.
5. **Time**: the ad deadline needs wall-clock milliseconds, imported from JS as `f64` (see the `int` is 32 bits gotcha). `i64` in the state, narrowed carefully. Pass "now" into the rules so tests control it.
6. **Save**: the original never saves (section 1b), so the exact port does not either. If saving is added later, use a namespaced key such as `sao:save` and an explicit `empty` marker (itch.io games share one `localStorage`).
7. **Numeric prompts**: Heading and Speed are text-entry doubles in the original. In the DOM build use `<input type="number">` with `min`/`max`/`step`, and clamp in Odin as `SetDimension` did. Verify that clamping first.
8. **Delete the Spectre console front end and the three native publishes** only after the web port ships. The old `shippit.sh` pushes `windows`, `linux`, `mac` and `html` channels; the new one should push only a new `html` channel (see open question 4).

## 3. Phases

Each phase ends with something runnable. Do them in order.

**Phase 0: scaffold (small)**
- `odin/` directory, `build.sh` (`odin build odin -target:js_wasm32 -out:out/game.wasm -o:speed`, copy `odin.js`), a local static server note (pick a port that is not 8765), `.gitignore` entries for `out/` and the stray `odin` test binary.
- Confirm the toolchain at `/home/yermom/ODIN/odin` builds an empty `js_wasm32` program that logs a line.

**Phase 1: pure rules + tests (the core of the port)**
- Port `Utility` math (distance, heading in degrees normalized to 0..360, next position), boat state, mooring rules, `can_perform` per verb, move + shark roll, fight and death, ad start/show/finish.
- Tests (native, `ODIN_TEST_THREADS=1`): can't move while moored; can only moor within 1.0 of the pier; no shark under 10; shark probability at several distances with a forced roll; fight always kills; ad countdown and finish; heading and speed clamping; heading-to-pier values.
- Also pin the exact message text for Look/Status/Move against the VB build's output (run the Spectre build once and transcribe).

**Phase 2: screens as data**
- `View` type (kind, id, text, href), the state machine and the menu ordering from section 1, including disabled-but-shown items.
- Tests that walk a whole run through the menu API: title, embark, unmoor, set heading, move until a shark, fight, dead menu, watch ad, abandon. Bound every loop.

**Phase 3: web shell**
- `index.html` plus shim: `dom_clear`/`dom_add`, a click export, a text/number submit export, storage get/set imports, `Date.now()` as `f64`, and links that open in a new tab. Build elements with `textContent`, never `innerHTML`. Match the original look (plain page, the title as `h1`).
- Real-browser playtest in Chrome (not just the in-app pane), including a save, reload, resume, and an ad break that survives a reload.

**Phase 4: polish**
- No save/load (see section 1b). Check wasm vs native differences (32-bit `int`) by always running the wasm build as well as the tests.

**Phase 5: ship (only when you say so)**
- New `shippit.sh` in the style of the other repos: tests, build, zip with `index.html` at the root, and push only with `--push`. Update README and write `ITCH_DESCRIPTION.md`. Remove the VB.NET projects, `.slnx` and old publishes in a separate commit. Add a vault note under `Games/` and link it from `Home.md`.

## 4. Risks

- **Behavior drift from the original.** The game is tiny, so any rule change is obvious. Mitigation: transcribe VB output first, then keep tests that mirror it.
- **Hidden state in the entity framework** (message clearing, when `Look` runs, save timing). Mitigation: read `World.vb`, `Entity.vb` and `MetaphorDialog` before phase 2.
- **Ad timing in tests and across reloads.** Mitigation: inject "now", persist the deadline, test a reload mid-break.
- **Old itch.io channels.** Pushing a new `html` channel next to the existing Blazor `html` channel replaces it; check what is live first.

## 5. Decisions (settled)

1. **Presentation**: plain DOM, event driven.
2. **Fidelity**: exact port of rules, menu order and wording. Questionable quirks are not fixed; they are logged in `docs/QUIRKS.md` as they are found, for decisions after the port ships. Known so far: the unused default name `Olen Kyrpa`, the never-set pronouns, the disabled Quit in the browser build, the Blazor grid rendering literal `gridCell.Text`, and Ground/Inventory entries that can never appear.
3. **Saves**: no migration of old saves (there are none). Whether to add saving is deferred, see 1b and `QUIRKS.md`.
4. **Native builds**: browser only. The Spectre project and the Windows/Linux/Mac publishes are dropped; the old itch.io uploads are removed by hand.
5. **Layout**: new `odin/` beside `src/`, delete `src/` in a separate commit at ship time.
6. **Scope**: jam game only. No item, inventory, ground or feature structures beyond what the game uses.

Remaining before phase 2: the verification list at the end of section 1.
