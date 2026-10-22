# WASM feasibility probe

Goal: prove the single assumption the whole client migration rests on, before
touching the real codebase.

The claim under test: the existing NanosSharp package model (real C#, reflection,
attribute scanning, DI, collectible load contexts) can run unchanged inside a
WebAssembly sandbox, where the native boundary that is today a table of
`delegate* unmanaged` function pointers (`NanosSharp/Interop/NativeApi.cs`)
becomes a set of host-supplied imports.

## Layout

- `guest/` - stand-in for the client runtime. Compiled to `wasi-wasm`. Declares
  host imports (`nanos.log`, `nanos.value-create-integer`,
  `nanos.value-get-integer`) that mirror the shape of `NativeApi`, and exercises
  reflection + a tiny DI container + attribute scanning inside the sandbox.
- `wit/nanos.wit` - the import table slice as a WIT world, injected into the
  component at link time.
- `host-rs/` - stand-in for the native game client. A Rust wasmtime component
  host that supplies the `nanos` import table plus WASI and drives the guest.
  This is the embedding the real client performs through the wasmtime C API.
- `run.sh` - builds the guest and runs it under the host, end to end.

## Prerequisites

```
sudo dotnet workload install wasi-experimental
```

## Two questions, tested separately

### Question 1 (PASSED): does real .NET run in the sandbox?

Does real .NET - reflection, attribute scanning, constructor-injection DI -
execute inside a WASI sandbox under a real standalone host?

```
dotnet publish guest/Guest.csproj -c Release -o out/guest
cd out/guest && wasmtime run -S http=y --dir . dotnet.wasm Guest
```

Observed output (2026-10-21, .NET 10.0.303, wasmtime 29.0.1):

```
[guest] guest runtime entered
[guest] reflection discovered 2 hook method(s)
[guest] hello nanos from inside wasm
[guest] static hook fired
[guest] instance hook fired
[guest] guest runtime finished
```

Reflection, attribute scanning and DI all work inside the sandbox, so the
package-facing API does not need cuts for those features.

### Question 2 (PASSED): can the host supply the import table, both directions?

End-to-end proven. `run.sh` builds the guest component with the custom `nanos`
imports and runs it under the Rust component host (`host-rs/`) that supplies
them. Observed output:

```
[guest] guest runtime entered
[guest] host value round trip: handle 1 -> 1337
[guest] function-call sum(40, 2) = 42
[host] function-call string arg: "hello "
[host] function-call string arg: "wasm"
[guest] function-call string bytes("hello ", "wasm") = 10
[guest] reflection discovered 2 hook method(s)
[guest] hello nanos from inside wasm
[guest] static hook fired
[guest] handled event 10
[guest] handled event 20
[guest] handled event 30
[guest] pumped 3 host event(s)
[guest] guest runtime finished
host: guest returned cleanly
```

- `host value round trip` = guest called `nanos.value-create-integer(1337)`,
  the host stored it and returned handle 1, guest called
  `nanos.value-get-integer(1)` and got 1337 back. Guest -> host -> guest over the
  custom table.
- the `[guest]` lines arrived through `nanos.log(list<u8>)`: the host received
  the raw bytes (the raw-bytes ABI, honoured within the component model).
- `handled event 10/20/30` = the host -> guest direction by guest-pull: the host
  queued events and the guest pumped them through the `next-event` import until
  it returned -1. This stands in for ManagedCallbacks (Tick/events).
- `host: guest returned cleanly` = host -> guest, driving the `wasi:cli/run`
  export.
- `function-call sum(40, 2) = 42` and `string bytes("hello ", "wasm") = 10` =
  the real `FunctionCall` argument-packing path: the guest packs a
  count-prefixed `NativeArgument[]` (32-byte records + a scratch blob for
  strings, mirroring `ArgumentPacker`) into a `list<u8>`, and the host unpacks
  it faithfully, reading the strings through their scratch offsets.

Finding: in interpreter mode a `function-call` import returning `i64` traps
(`aot-runtime-wasm.c:188`, missing interp-to-native trampoline); returning `i32`
works. Arbitrary import signatures need AOT. See docs/wasm-migration.md.

### AOT (PASSED, the central-tension check)

The guest is built with `RunAOTCompilation=true`. AOT requires
`PublishTrimmed=true`; the Guest assembly is kept with `<TrimmerRootAssembly>`
so reflection/DI survive trimming. Under AOT the `i64` `function-call` signature
runs (the interpreter trampoline limit is gone), reflection still finds the
hooks, DI still resolves, and the module shrinks 32 MB -> 17 MB. The trimmer
emits IL2026/IL2070/IL2072/IL2075 on the reflection paths - the production
runtime must be made trim-safe (root package assemblies / annotate).

Note: the committed csproj has AOT on, so the build is slow. Set
`RunAOTCompilation=false` and `PublishTrimmed=false` for a fast interpreter
build.

Reproduce:

```
./run.sh
```

#### Details

The host (`host-rs/`) is a Rust wasmtime component host, because .NET 10 WASI
only emits a wasip2 component (no preview1 core module), so a plain core-module
host is not available. It adds WASI (`wasmtime_wasi` + `wasmtime_wasi_http`) and
defines the three `nanos` functions on the component linker, then calls the
`wasi:cli/run` export. The native game client embeds the equivalent through the
wasmtime C API.

An earlier `host/` project (wasmtime-dotnet) was a dead end: that NuGet only
loads core modules, not the WASI 0.2 component the SDK emits. It was removed in
favour of `host-rs/`.

What made the custom import build work (see docs/wasm-migration.md): the imports
force a native relink, so `WASI_SDK_PATH` must point at the wasi-sdk; a second
`--component-type` pointing at `wit/nanos.wit` is appended via `<LinkerArg>` so
`wasm-component-ld` merges it with the built-in WASI world; the WIT interface is
named `nanos` to match `[DllImport("nanos")]`; and the guest's `EntryPoint`s are
kebab-case to match the WIT function names.

## Findings that shape the real migration

- Output is a **WASI 0.2 component**, always: `WasiApp.targets` unconditionally
  links `--component-type`. There is no simple core-module switch.
- Default build is the **Mono interpreter**: `dotnet.wasm` is ~12 MB plus ~200
  sidecar BCL assemblies loaded from a preopened directory. AOT is a separate,
  later step and is what makes the performance case over Lua.
- The component imports `wasi:http/types@0.2.0`, so even a smoke run needs the
  host to wire wasi-http (`-S http=y`).

## What this probe does NOT yet cover

- Entities across the boundary (classId/instance/networkId -> handle indices)
  and the result direction (the single `NativeArgument` returned by value).
- Batching the chatty value API.
- Threading model constraints.
- Collectible `AssemblyLoadContext` hot reload inside wasm.
- A real reactor export path (the host -> guest direction is demonstrated by
  guest-pull inversion instead; see docs/wasm-migration.md).
- Performance measurement against Lua.
