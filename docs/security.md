# Security model

This document describes what NanosSharp protects, from whom, and where the boundaries are. The component layout it
refers to is in [architecture.md](architecture.md).

## Who is trusted

NanosSharp runs code from two sources with very different trust:

- **Server packages** are chosen and installed by the server operator, exactly like Lua packages. They are trusted and
  run in process with full rights. Protecting the server operator from their own packages is a non-goal, as it is for
  Lua.
- **Client packages** are a different matter. In nanos world the server pushes a package's client code to every player
  who joins. That code then runs on the player's machine. The player did not choose it and cannot vet it. A malicious
  or compromised server could therefore ship client code that reads the player's files, opens network connections, or
  runs native code on their machine.

The entire client security model exists to contain that second case: **untrusted client code must not get ambient
access to the player's machine.**

## Why the previous model was not a boundary

Earlier the client ran C# packages in process, the same as the server, with a compile-time IL allow-list meant to
restrict what the code could do. That was never a real boundary:

- It was enforced at compile time, by inspecting IL against a list. Reflection, `unsafe`, dynamic loading and ordinary
  gaps in the list all defeat a compile-time check. Once the assembly is loaded in process, the list enforces nothing.
- Code running in process has the full rights of the game process regardless of what its IL looked like.

An allow-list on untrusted code that then runs with full ambient authority is a lint, not a sandbox.

## The boundary today

Client packages run inside a WebAssembly sandbox (Wasmtime, Mono interpreter). The guarantees come from the sandbox,
not from inspecting the code:

- **Memory isolation.** The package runs in the reactor's own linear memory. It cannot read or write the host process
  memory, the engine, or other packages' host-side state. It holds engine objects only as opaque integer handles that
  mean nothing without the trusted host's tables.
- **No ambient authority.** The reactor is given no capabilities it is not explicitly handed. It has no file system
  access beyond the single directory the trusted host stages its own assemblies into, no network, no process creation,
  no environment, no native calls, no foreign function interface. The WASI surface is a minimal preview1 subset and the
  calls that would reach the outside world are stubbed.
- **The compiler stays outside.** Roslyn and NuGet run in the trusted host, never in the sandbox. The sandbox has no
  compiler, no dependency resolver and no way to fetch or emit new native code. Package assemblies are compiled outside
  and staged in read-only.
- **One brokered door.** The only way out of the sandbox is the `nanos` import seam. Every import is implemented by the
  trusted host, which reads the arguments out of guest memory, validates and translates them, and calls the engine on
  the guest's behalf. There is no second channel.

A client package that tried to open a file, make an HTTP request, spawn a process or call into a native library has no
API to do so and no ambient capability to fall back on. The worst it can reach is the engine, through the one door.

## What the sandbox does and does not restrict

The sandbox restricts **ambient authority**, not **API use**. Inside the sandbox a package can still call any engine
function the client API exposes, because that is what a package is for, exactly as a Lua client package can. The
sandbox guarantees the package cannot step outside that API to touch the machine; it does not, and is not meant to,
decide whether a given engine call is "allowed". Engine-level abuse (spawning entities, spamming the chat, reading
game state the client already has) is a game-design and rate-limiting concern, the same as it is for Lua, and belongs
to the package and server authors, not to the isolation layer.

This is the right split. The isolation layer has one job, keep untrusted code off the machine, and it does that with a
real mechanism instead of a guess about intent.

## Trust boundaries, summarized

```
 trusted                                   | untrusted
 ------------------------------------------+---------------------------
 nanos world engine (native)               |
 native host (version.dll)                 |
 trusted host (CoreCLR): Roslyn, staging,  |
   handle tables, callback relay           |
   WasmHost `nanos` import implementations  <----  the one boundary
                                           | reactor (Wasmtime guest):
                                           |   client package code
                                           |   NanosSharp.WasmRuntime
```

Everything to the left is trusted and runs with process rights. Everything to the right is untrusted client code. The
boundary is the `nanos` import seam, crossed only through handles and blittable records.

## Release integrity

Isolation only matters if the player runs the real NanosSharp. Releases are signed with an ECDSA key whose public half
is built into the installer and the updater:

- Every release ships a `SHA256SUMS` signed with the release key.
- The installer and the self-updater verify every download, manual or automatic, against the pinned public key and
  reject anything that is not a genuine, untampered release. A hijacked download, mirror or man-in-the-middle cannot
  slip in a different build.
- The server updates itself only to newer, verified releases and keeps the previous version as a backup. The client
  updates the same way and applies on the next game start. Updates can be turned off (`[update]` in `NanosSharp.toml`,
  `NANOSSHARP_NO_UPDATE=1` on the client).

The private signing key is never in the repository and never in CI logs; it lives only where releases are cut.

## Residual risks and honest limitations

- **The engine API is the attack surface that remains.** The sandbox contains the machine, not the engine. A hostile
  client package can still misuse the client API within the engine. That is the same exposure Lua client packages
  have, and it is handled at the game and server layer, not by isolation.
- **Packages share one reactor.** For the session there is a single reactor; packages are separated inside it by
  collectible load contexts and per-package scopes, which is logical isolation, not a separate sandbox per package. A
  package cannot reach another package's host-side handles, but they share the guest runtime. Hardening toward
  per-package isolation is possible future work.
- **The trusted host compiles untrusted source.** Roslyn compiles the downloaded client source in the trusted host.
  Compilation itself runs code (source generators, analyzers) only if referenced, and client packages reference only
  the NanosSharp client API, but treating the compiler as part of the trusted computing base is a deliberate choice.
- **Interpreter, for now.** The reactor runs on the Mono interpreter. This is a performance characteristic, not a
  security one; an ahead-of-time reactor is tracked as a follow-up.
- **Defense in depth is not yet layered on.** Wrapping the client host process in a restricted OS token, on top of the
  sandbox, is possible future hardening and not currently applied.

## Reporting

Security issues should be reported privately to the maintainer rather than through public issues.
