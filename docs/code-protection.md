# Client code protection

## Goal and threat model

Deter casual copying of client-side package code (C# and Lua), roughly the 95% of "script kiddies" who just read the
downloaded package folder. Perfect protection is impossible: client code must reach the client to run, so a determined
attacker can always extract it at runtime with a memory dump. The aim is to raise the effort bar.

Hard requirement: no global secret in the shipped binaries. Reverse-engineering `version.dll` or the server binary must
not yield a key that unlocks every server. Keys are per-server.

## Leverage

NanosSharp owns the native client host (`version.dll`), which sits between the downloaded files and both runtimes (the
Lua VM and the Mono-WASM runtime for C#). It can decrypt in memory so plaintext never touches disk. The server already
transforms client files during the handshake and `StoreClientFiles` step, so it can compile and encrypt before sending.

## Current state

The server sends `Client/` and `Shared/` C# as `.cs` source; the client host (Roslyn, `IsSandboxed = true`) compiles
it locally and runs it in the Mono-WASM sandbox. Lua client code ships as `.lua` source. Everything sits in cleartext
in `%LOCALAPPDATA%\...\Packages\<name>\`.

## Design

### C# precompilation

The server compiles each package's `Client/` and `Shared/` C# into a client assembly using its own Roslyn and ships
the DLL instead of the source. The client loads the prebuilt DLL into the Mono-WASM runtime and caches it, keyed by a
content hash, so it is reused across connects. Roslyn is removed from the client runtime, which becomes smaller and
faster and no longer carries package source on disk. A later phase obfuscates the IL before shipping.

### Per-server key without a global secret

The server generates a unique symmetric key `K` (AES-256) on first start, stored server-side only in its data directory
and never shipped. `K` encrypts the protected client artifacts. The key is delivered with an ephemeral ECIES exchange
(X25519): `version.dll` generates an ephemeral key pair per connection and sends its public key in the handshake, the
server wraps `K` to that public key, and the client unwraps `K` with its ephemeral private key and holds it in memory.

No secret lives in any shipped binary, so reverse-engineering `version.dll` or the server yields only code. To obtain
server X's key an attacker must connect to X and then dump memory or reimplement the exchange, which is per-server
effort and never a master key. Leaked at-rest files are useless without `K`. An optional MITM/server authentication can
use a non-secret public server-identity key embedded in `version.dll`, since a public key is not a secret.

### Lua

Defense in depth, in order of strength: minify and obfuscate the source; compile to Lua 5.4 bytecode if the nanos Lua VM
accepts it (needs verification); decrypt in memory through a native hook on the Lua load path.

### In-memory decryption

`version.dll` decrypts downloaded artifacts with `K` in memory and feeds them to the Mono-WASM runtime (C#) or the Lua
VM; plaintext is never written to disk.

## Honest ceiling

An attacker who connects to a server can still extract that server's code with a memory dump of `K` or the plaintext,
or by reimplementing the protocol. That is per-server effort and never a master key from reverse-engineering alone. The
design defeats casual copying, not a determined reverse engineer.

## Phases

1. **C# server-side precompile** (in progress): compile client C# to a DLL on the server, ship and cache it on the
   client, drop Roslyn from the client. No cryptography; already removes source on disk.
2. **C# IL obfuscation** of the shipped client assembly.
3. **Per-server key + ephemeral ECIES exchange + in-memory decryption** for C# and Lua.
4. **Lua bytecode or obfuscation** under the same encryption.

Protection is opt-in per package, for example `Package.toml [script] protect = true`.

## Open questions

- Does the nanos Lua VM accept bytecode (`load` mode `b`)? Needs a reverse-engineering check.
- The Lua load hook point for in-memory decryption, with the usual caution about fragile byte signatures.
- Key management details: rotation and storage permissions.
- Tradeoffs: client hot-reload of protected code, debuggability under obfuscation, and client startup cost.
