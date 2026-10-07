#!/usr/bin/env bash
# Builds the browser game into odin/out (what gets shipped).
set -euo pipefail
cd "$(dirname "$0")"
ODIN="${ODIN:-/home/yermom/ODIN/odin}"
mkdir -p out
"$ODIN" build . -target:js_wasm32 -out:out/game.wasm -o:speed
cp "$("$ODIN" root)/core/sys/wasm/js/odin.js" out/
cp web/index.html out/
