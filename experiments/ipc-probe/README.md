# Process-sandbox IPC probe

Tests the performance-relevant core of the "separate low-privilege process"
sandbox idea: the cost of a synchronous crossing between the game process and
the sandbox process over shared memory. The OS confinement itself (Windows
restricted token / Job Object / AppContainer) is not measured here - it is
Windows-specific, not available on this Linux dev machine, and not a performance
factor. What matters for performance is the crossing, and that is measured with
equivalent primitives (POSIX shared memory + atomics / semaphores) that map
qualitatively onto Windows (named shared memory + events).

## Build and run

```
gcc -O2 -pthread ipc_bench.c -o ipc_bench
./ipc_bench 1000000
```

## Results (1,000,000 roundtrips, this machine, stable across runs)

| mechanism                         | ns/roundtrip | cost                         |
|-----------------------------------|--------------|------------------------------|
| process sandbox, busy-poll (spin) |      ~37     | pegs a full CPU core         |
| process sandbox, blocking (sem)   |    ~3400     | no spin, context-switch/call |

For comparison (release hosts, from `../wasm-probe/crossing-bench.sh`):

| wasm core crossing                |       ~6     | no spin                      |
| Lua 5.4 -> C call                 |       ~14    | in-process                   |
| wasm component crossing           |       ~23    | no spin                      |

## Reading it

The process sandbox loses to wasm on the synchronous crossing, outright:

```
in-process ~1-2 ns  <  wasm core ~6 ns  <  Lua C ~14 ns  <  wasm component ~23 ns  <  process spin ~37 ns (burns a core)  <<  process blocking ~3400 ns
```

- **Busy-poll (~37 ns)** does not even beat a wasm component crossing (~23 ns),
  and it pegs a full CPU core to do it - a game client cannot usually spare one,
  and the number assumes a *free* core.
- **Blocking (~3400 ns)** is the CPU-friendly option and is ~150x a wasm component
  crossing and ~570x a wasm core crossing - a context switch per call, unusable
  for fine-grained synchronous script -> engine calls.

So a separate-process sandbox buys OS-level confinement at a crossing cost that
is strictly worse than the in-process wasm sandbox, which already provides
isolation without a second process or a burned core. (Note: this C IPC bench is
compiled -O2; the earlier wasm crossing numbers it was first compared against
were wrong - measured through a debug-built host - and have been corrected to the
release numbers above. The conclusion is unchanged in direction and stronger in
degree.)

## Implication for the two-sandbox idea

The reason to build a separate Windows-native sandbox was performance (native C#
speed + shared memory, avoiding a supposed WASM interop tax). But the WASM
interop tax was a measurement artifact - in release it is ~6-23 ns/crossing,
cheaper than this process IPC. So the process sandbox has no performance case
over in-process wasm. If OS-level confinement is still wanted on Windows, wrap
the wasm host process in a restricted token for defense-in-depth - that costs
nothing per crossing - rather than moving script code into a second process.
