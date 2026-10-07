#+build js
package sharks

import "base:runtime"
import "core:fmt"
import "core:math/rand"
import "core:time"

ctx: runtime.Context

main :: proc() {
	ctx = context
	rand.reset(u64(time.now()._nsec))
	fmt.println("Shark Attackers of SPLORR!! (odin scaffold)")
}

// Kept so odin.js does not end the program right after main.
@(export)
step :: proc(dt: f64, c: runtime.Context) -> bool {
	return true
}
