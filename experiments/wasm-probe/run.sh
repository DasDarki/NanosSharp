#!/usr/bin/env bash
# Reproduce the end-to-end probe: build the guest component (with the custom
# nanos imports) and run it under the Rust component host that supplies them.
#
# Requires: .NET 10 SDK + wasi-experimental workload, wasi-sdk (WASI_SDK_PATH),
# a Rust toolchain, and wasm-tools on PATH.
set -euo pipefail
cd "$(dirname "$0")"

: "${WASI_SDK_PATH:=$HOME/.wasi-sdk}"
export WASI_SDK_PATH

echo "== building guest component =="
dotnet publish guest/Guest.csproj -c Release -o out/guest >/dev/null
cp guest/bin/Release/net10.0/wasi-wasm/AppBundle/Guest.wasm out/guest.component.wasm

echo "== nanos import interface in the component =="
wasm-tools component wit out/guest.component.wasm | grep -A4 "import nanos"

echo "== running under the component host =="
cargo run --quiet --manifest-path host-rs/Cargo.toml -- out/guest.component.wasm
