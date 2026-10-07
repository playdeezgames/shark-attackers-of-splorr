#!/usr/bin/env bash
# Native tests. One thread: the tests are independent but the tracking allocator is per thread.
set -euo pipefail
cd "$(dirname "$0")"
"${ODIN:-/home/yermom/ODIN/odin}" test . -define:ODIN_TEST_THREADS=1
