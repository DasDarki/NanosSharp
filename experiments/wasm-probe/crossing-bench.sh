#!/usr/bin/env bash
# Crossing-cost benchmark. MUST build the hosts in RELEASE - a debug host build
# inflates these numbers by 10-150x (the host-side closures dominate in debug).
set -euo pipefail
cd "$(dirname "$0")"
export PATH="$HOME/.cargo/bin:$PATH"
N="${1:-20000000}"
BV="${2:-2441}"   # batch crossings; 4096 values each

cargo build --release --quiet --manifest-path host-rs/Cargo.toml
cargo build --release --quiet --manifest-path host-core-rs/Cargo.toml
HR=host-rs/target/release/nanos-wasm-host
HC=host-core-rs/target/release/nanos-wasm-core-host

ms() { grep -o 'elapsed_ms=[0-9.]*' | cut -d= -f2; }
row() {
  local name="$1" host="$2" mod="$3"
  local c b
  c=$("$host" "$mod" chatty "$N" 2>/dev/null | ms)
  b=$("$host" "$mod" batch 4096 "$BV" 2>/dev/null | ms)
  awk -v n="$name" -v c="$c" -v b="$b" -v N="$N" -v V="$((4096*BV))" \
    'BEGIN{printf "%-26s %8.1f %10.2f\n", n, c*1e6/N, b*1e6/V}'
}

printf "%-26s %8s %10s\n" "mechanism" "ns/call" "ns/value"
row "wasm component (Mono)"  "$HR" out/guest.aot.wasm
row "wasm core (Mono)"       "$HC" out/core/unbundled-module0.wasm
row "wasm core (NativeAOT)"  "$HC" naot/ncore/unbundled-module0.wasm
echo "(Lua 5.4 -> C ~14 ns/call; process spin ~37 ns; process blocking ~3400 ns)"
