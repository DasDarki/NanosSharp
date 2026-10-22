#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
: "${WASI_SDK_PATH:=$HOME/.wasi-sdk}"; export WASI_SDK_PATH
export PATH="$HOME/.local/bin:$HOME/.cargo/bin:$PATH"
N="${1:-20000000}"

extract() { sed -n 's/.*elapsed_ms=\([0-9.]*\).*/\1/p' | tail -1; }
nsper() { awk -v ms="$1" -v n="$N" 'BEGIN{ if (ms=="") {print "n/a"} else {printf "%.2f", ms*1e6/n} }'; }
best() { sort -n | head -1; }

echo "== build native (JIT) =="
dotnet publish native/Native.csproj -c Release -o native/out >/dev/null 2>&1
echo "== build wasm interpreter =="
dotnet publish wasm/Wasm.csproj -c Release -o wasm/out-interp >/dev/null 2>&1
echo "== build wasm AOT (slow) =="
dotnet publish wasm/Wasm.csproj -c Release -o wasm/out-aot \
  -p:RunAOTCompilation=true -p:PublishTrimmed=true -p:WasmSingleFileBundle=true -p:WasmMainAssemblyFileName=Wasm.dll >/dev/null 2>&1

INTERP_DIR="$(dirname "$(find wasm/out-interp wasm/bin -name dotnet.wasm | head -1)")"
AOT_WASM="$(find wasm/out-aot wasm/bin -name 'Wasm.wasm' | head -1)"

echo "N=$N"; echo

ms_native=$(for i in 1 2 3; do dotnet native/out/Native.dll "$N"; done | sed -n 's/.*elapsed_ms=\([0-9.]*\).*/\1/p' | best)
ms_lua54=$(lua5.4 compute.lua "$N" | extract)
ms_luajit=$(luajit compute.lua "$N" | extract)
ms_interp=$(wasmtime run -S http=y --dir "$INTERP_DIR"::/ "$INTERP_DIR/dotnet.wasm" Wasm "$N" 2>/dev/null | extract)
ms_aot=$(wasmtime run -S http=y "$AOT_WASM" "$N" 2>/dev/null | extract)

printf "%-22s %12s %14s\n" "environment" "elapsed_ms" "ns/iter"
printf "%-22s %12s %14s\n" "---------------------" "----------" "------------"
printf "%-22s %12s %14s\n" "C# native (JIT)"      "$ms_native" "$(nsper "$ms_native")"
printf "%-22s %12s %14s\n" "C# wasm AOT"          "$ms_aot"    "$(nsper "$ms_aot")"
printf "%-22s %12s %14s\n" "C# wasm interpreter"  "$ms_interp" "$(nsper "$ms_interp")"
printf "%-22s %12s %14s\n" "Lua 5.4 (nanos)"      "$ms_lua54"  "$(nsper "$ms_lua54")"
printf "%-22s %12s %14s\n" "LuaJIT 2.1"           "$ms_luajit" "$(nsper "$ms_luajit")"
