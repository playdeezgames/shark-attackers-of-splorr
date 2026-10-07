#!/usr/bin/env bash
# Tests, builds and zips the Odin browser game. Uploads to itch.io only with --push.
#   ./shippit.sh          test + build + zip, prints "not pushed"
#   ./shippit.sh --push   the same, then butler push (public!)
set -euo pipefail
cd "$(dirname "$0")"

TARGET="thegrumpygamedev/shark-attackers-of-splorr:html"
ZIP="build/shark-attackers-html5.zip"

./odin/test.sh
./odin/build.sh

rm -f odin/out/*.pdb
mkdir -p build
rm -f "$ZIP"
(cd odin/out && zip -qr "../../$ZIP" .)
echo "built $ZIP ($(du -h "$ZIP" | cut -f1)):"
unzip -l "$ZIP" | tail -n +4 | head -n -2

if [ "${1:-}" = "--push" ]; then
	butler push odin/out "$TARGET"
else
	echo "not pushed (run with --push to upload to $TARGET)"
fi
