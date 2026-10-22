# Compute benchmark: C# (native / wasm) vs Lua

Answers the one remaining decision-relevant question: is C# on wasm actually
faster than the Lua the client runs today?

## Method

The identical algorithm (a 64-bit xorshift + multiply-accumulate loop, N
iterations) runs in five environments. Only the loop is timed (`Stopwatch` /
`os.clock`), so startup is excluded - this is steady-state compute. The same
source compiles for native and wasm (`Bench.cs`); `compute.lua` is the line-for-
line Lua port.

```
./bench.sh [N]        # default N = 20_000_000
```

## Results (N = 20,000,000, this machine)

| environment            | ns/iter | vs Lua 5.4   | result            |
|------------------------|---------|--------------|-------------------|
| C# native (JIT)        |   1.46  | 9.3x faster  | 4c61c276d8196399  |
| **C# wasm AOT**        | **1.83**| **7.4x faster** | 4c61c276d8196399 |
| Lua 5.4 (nanos)        |  13.55  | 1.0x         | 4c61c276d8196399  |
| C# wasm interpreter    |  18.04  | 0.75x slower | 4c61c276d8196399  |

The native, wasm-AOT, Lua 5.4 and wasm-interpreter runs all produce the same
result, so they compute the same work.

## Reading it

- **C# wasm AOT is ~7.4x faster than Lua 5.4**, the interpreter nanos ships on
  the client. That is the headline: moving client scripting to AOT C# on wasm is
  a performance win over the status quo, not a regression, while adding the
  sandbox.
- The wasm sandbox costs about 25% over native JIT (1.83 vs 1.46 ns/iter) - the
  price of isolation on compute.
- The **wasm interpreter is slower than Lua 5.4**, confirming from the
  performance side what the trampoline finding showed structurally: AOT is
  required, interpreter mode is not a shipping option.

## LuaJIT caveat

LuaJIT 2.1 ran the loop in ~1.1 ns/iter but produced a different result
(`8000000000000000`): LuaJIT has no 64-bit integer type (numbers are doubles),
so it does not compute the same work and is not a valid comparison here. nanos
runs Lua 5.4, which matched, so 5.4 is the relevant baseline.

## Chatty axis: per-crossing cost

> CORRECTION (supersedes an earlier version of this file): the first pass
> measured the crossing through a **debug-built** Rust host, which inflated every
> number 10-150x and produced a wrong conclusion ("~305 ns/call, ~188 ns/value,
> batching useless, the component copy is the killer"). With the host built in
> **release** (`crossing-bench.sh` enforces this) the real picture is the
> opposite: wasm interop is cheap and competitive with Lua. The numbers below are
> the corrected release numbers.

A tight loop of a trivial call into native (`nop(x) -> x+1`) measures the
per-crossing overhead; a batched `function-call` carrying a packed buffer
measures bulk throughput. Hosts built in release. Reproduce:

```
./crossing-bench.sh            # builds hosts in release, prints the table
LUA_CPATH="./?.so" lua5.4 chatty.lua 20000000   # Lua 5.4 -> C baseline
```

Results (release hosts, 20M calls / 10M batched values, stable):

| mechanism                     | ns/call | ns/value (batch) |
|-------------------------------|---------|------------------|
| wasm core (NativeAOT)         |  ~6.0   |      ~0.60       |
| wasm core (Mono)              |  ~6.4   |      ~0.60       |
| Lua 5.4 -> C (nanos baseline) |  ~14    |        -         |
| wasm component (Mono AOT)     |  ~23    |      ~1.27       |
| process sandbox, spin         |  ~37    |        -         |
| process sandbox, blocking     | ~3400   |        -         |

## Reading it (both axes)

- **Compute**: wasm AOT ~7.4x faster than Lua 5.4 (unchanged; measured in-process,
  never touched the host build).
- **Crossing**: wasm interop is cheap. A core-module crossing (~6 ns) is faster
  than a Lua C call (~14 ns); a component crossing (~23 ns) is ~1.7x Lua. Not the
  15-22x penalty the debug measurement wrongly showed.
- **Batching works**: ~0.6 ns/value (core) and ~1.3 ns/value (component). The
  component canonical-ABI copy is real but ~0.6 ns/value in release - negligible.
- **Component vs core**: the component adds ~17 ns/call and ~0.6 ns/value over a
  raw core module. Real but tiny; not a dealbreaker. At 60 fps even 50k component
  crossings/frame cost ~1.1 ms (~7%); typical frames are far below that.
- **NativeAOT vs Mono**: ~identical crossing cost (~6 ns). NativeAOT's advantage
  is elsewhere - module size (2.6 MB vs Mono's 32 MB, ~12x) and no bundled Mono -
  not per-call speed.

## Methodology note

Crossing cost is measured by a `Stopwatch` inside the guest around a loop of host
calls, so it includes the host-side closure. Build the host in release or the
closure dominates. The compute benchmark and the Lua numbers are in-process and
were never affected. The core-module runs use the pre-canonical module extracted
via `wasm-tools component unbundle` (a toolchain hack); a supported core-module
output is a separate production question (see docs/wasm-migration.md).
