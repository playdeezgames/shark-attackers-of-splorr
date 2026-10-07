# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

"Shark Attackers of SPLORR!!", a text-adventure "Metaphor" by TheGrumpyGameDev for the [Wacky Fun Game Jam of Joy and Whimsy](https://itch.io/jam/the-wacky-fun-game-jam-of-joy-and-whimsy). Live at https://thegrumpygamedev.itch.io/shark-attackers-of-splorr. The jam build is finished (VB.NET). The current goal is to **plan, then carry out, a port to Odin compiled to `js_wasm32`**, the stack the author now uses for all new games. No port code exists yet; the VB.NET code is the reference for behavior.

An Obsidian vault with the author's cross-game knowledge lives at `/home/yermom/git/bok-of-splorr/splorr/` (outside this repo). Start with `Home.md`, then `Tech/Odin wasm recipe.md` (build command, frame loop, 2D-canvas JS shim, keeping logic native-testable with `#+build js` / `!js`), `Gotchas.md`, `Tech/Shipping to itch.io.md` and `Concepts/Metaphor design.md`. The vault has no note for this game yet. Its standing rules: never `git push` or run a ship script unless the user says so, and do not "fix" deliberate design (deadpan text, harsh difficulty) as if it were a bug.

## Commands

The legacy VB.NET game (.NET 10 SDK; builds cleanly):

```bash
dotnet build src/Metaphor.Spectre/Metaphor.Spectre.vbproj   # console build
dotnet run --project src/Metaphor.Spectre                    # play in the terminal (Spectre.Console)
dotnet run --project src/Metaphor.Blazor                     # browser build (dev server)
```

There are no tests and no linter. `shippit.sh` publishes self-contained linux/windows/mac single-file builds plus the Blazor site, then `butler push`es all four to itch.io. It publishes publicly, so only run it when asked. It is not executable (`bash shippit.sh`).

## Architecture (legacy VB.NET, `src/`)

`Metaphor.slnx` lays the projects out as numbered layers. Each layer has a generic `TGGD.*` project (reusable framework) and a `Metaphor.*` project (this game) that builds on it. References run strictly downward: Provision, Persistence, Extensions, Models, Presentation, Platform, then the front ends (Spectre, Blazor).

- **Provision** (`WorldData`, `EntityData`, `MessageData`): plain serializable data. The whole world is one `WorldData` holding a `Dictionary(Of Guid, EntityData)` plus a message log.
- **Persistence**: entity wrappers over that data (`Entity`, `World`, `Location`, `Character`, `Feature`, `Item`, `Inventory`, `Verb`). Everything is a generic entity with a subtype string and bags of properties (counters, dimensions, tags, metadata, "yokes" = named links to other entity ids). `IPersister` (`SaveAsync`/`LoadAsync` by filename) is the only I/O seam. Spectre implements it with files, Blazor with `localStorage` through `wwwroot/persister.js`.
- **Extensions**: all game rules, written as VB `<Extension>` methods on the persistence interfaces, grouped per entity kind (`Character/`, `Location/`, `Verbs/`, `WorldExtensions`). Names for properties and subtypes are string constants in `_Enum/` (`Counters`, `Tags`, `Yokes`, `CharacterSubtypes` with N00B and SHARK, `LocationSubtypes` with PIER and BOAT). `Grimoire` holds tuning constants.
- **Models** (namespace `Metaphor.Processing`): `IWorldModel`, `AvatarModel`, `AvatarCombatModel`, `AdModel`, etc., the facade the UI talks to. The player is the "avatar" and moves by heading and speed, so the world is navigated like a boat on open water (see the commit log: "avatar navigation model and sharks are enemies").
- **Presentation**: UI-agnostic dialog state machine. Each screen is a `Dialog` whose `Run()` returns an `IDialogPrompt` (choose, integer, double or string). `InPlay.Run()` is the router: ad in progress, then dead, then combat, then heading/speed prompt, else the navigation menu. Responding to a prompt calls the next dialog (`DialogSource` is a `Function() -> Dialog` continuation). Output is a list of `IDisplayElement`s carrying hints (title, link, newline).
- **Platform**: `Display` / `MetaphorDisplay` expose `Elements` + `Prompt` + `Running` to a front end and wrap prompts so a response swaps in the next dialog.
- **Front ends** (`Metaphor.Spectre`, `Metaphor.Blazor`) loop: render `Elements`, read the `Prompt`, respond, repeat. Spectre is the complete one. Blazor's `Home.razor` is rough (for example the grid cells render the literal text `gridCell.Text`, missing an `@`).

Implication for the port: the dialog/prompt loop is a menu-driven text UI. The rules in `Metaphor.Extensions` and `Metaphor.Models` are the part to translate, and the entity-bag persistence is VB/JSON-specific and can be replaced by plain structs.

`ss/cover.png` is the itch.io cover image.
