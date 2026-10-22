#!/usr/bin/env bash
# Builds the wasm client runtime as a WASI reactor core module.
#
# The .NET WASI SDK only emits command modules (run once via _start, then the
# runtime is dead). The native host needs to call guest exports repeatedly, so
# we publish normally, then relink the SDK's object files into a reactor with
# tools/relink_reactor.py.
#
# AOT is disabled for now: the relink can rebuild an AOT module (relink_reactor.py
# regenerates the per-assembly objects the SDK deletes), but the hand-built
# reactor init does not fully set up the AOT native-to-interp dispatch, so an AOT
# reactor traps at the first managed export (uninitialized element). The
# interpreter reactor works; AOT+reactor init is the open hardening follow-up. See
# docs/wasm-migration.md "Stage 3, slice 2".
set -euo pipefail

REPO="$(cd "$(dirname "$0")/../../.." && pwd)"
PROJECT="$REPO/managed/NanosSharp.WasmRuntime"
TOOLS="$PROJECT/tools"
OUT="${1:-$REPO/build/managed/Client/wasm/nanossharp-reactor.wasm}"
EXPORTS="nanos_boot,nanos_sync_packages,nanos_tick,nanos_engine_attached,nanos_engine_detaching,nanos_invoke_function,nanos_release_function,nanos_entity_created,nanos_entity_destroyed,bench_import,bench_value,bench_funccall,bench_compute"

: "${WASI_SDK_PATH:=$HOME/.wasi-sdk}"
export WASI_SDK_PATH

LOG="$(mktemp)"
trap 'rm -f "$LOG"' EXIT

echo "publishing wasm client (command module) ..."
rm -rf "$REPO/build/managed/Client/obj/NanosSharp.WasmRuntime" \
       "$REPO/build/managed/Client/bin/NanosSharp.WasmRuntime"
dotnet publish "$PROJECT" -c Release \
    -p:NanosSharpSide=Client -p:NanosSharpClientRuntime=Wasm \
    -p:RunAOTCompilation=false > "$LOG" 2>&1

mkdir -p "$(dirname "$OUT")"
echo "relinking to a reactor core module ..."
python3 "$TOOLS/relink_reactor.py" "$PROJECT" "$EXPORTS" "$OUT" "$LOG"
