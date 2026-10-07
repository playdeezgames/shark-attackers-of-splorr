# Shark Attackers of SPLORR!

For https://itch.io/jam/the-wacky-fun-game-jam-of-joy-and-whimsy

A Production of TheGrumpyGameDev

Play it: https://thegrumpygamedev.itch.io/shark-attackers-of-splorr

## Layout

- `odin/` is the current game: Odin compiled to `js_wasm32`, drawn as plain HTML.
- `src/` is the original VB.NET game (console and Blazor builds), kept for reference until the port ships.
- `docs/PORT_PLAN.md` is the plan and the verified behavior of the original; `docs/QUIRKS.md` lists oddities of the original left alone on purpose.

## Working on the Odin game

Needs Odin (the build uses `dev-2026-07-nightly`; set `ODIN` if it is not at `/home/yermom/ODIN/odin`).

```bash
odin/test.sh     # native tests
odin/build.sh    # builds odin/out (game.wasm, odin.js, index.html)
python3 -m http.server -d odin/out 8124   # then open http://localhost:8124/
```

`./shippit.sh` runs the tests, builds, and zips to `build/`. It uploads to itch.io only with `--push`.
