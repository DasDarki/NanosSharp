# Orb Rush

A small arena game mode written entirely in C#, split across the server, client and shared
sides of a single NanosSharp package. It is meant as a realistic example that actually plays,
and it exercises the pieces that matter for the WebAssembly client sandbox.

## How it plays

- Eight glowing orbs are scattered around the arena.
- Every player spawns as a mannequin and runs around collecting orbs. Each orb is worth one point.
- Walking into an orb scores it and the orb immediately reappears somewhere else.
- A round lasts 90 seconds. When it ends, the winner is announced and a new round starts after 5 seconds.
- Press `F` to ping the nearest orb; the HUD tells you the direction and distance.

## What each side does

- `Shared/Protocol.cs` - event names and tuning values shared by both sides.
- `Server/GameMode.cs` - the `Package` entry point: spawns players, scatters orbs, scores pickups,
  runs the round loop with timers and answers ping requests.
- `Server/ScoreBoard.cs` - a `Service` that is dependency injected into the game mode and tracks scores.
- `Server/Orb.cs` - a light paired with an invisible overlap trigger.
- `Client/Hud.cs` - a chat based HUD, a local round clock driven by the per frame tick and the `F` key binding.

## Why it is a good sandbox test

The client side touches every path the sandboxed runtime has to carry:

- The package lifecycle (`OnStart` / `OnStop`).
- Dependency injection on the server (`[Inject] ScoreBoard`).
- Custom remote events in both directions (`CallRemote` client to server, `BroadcastRemote` server to client).
- Engine callbacks relayed into the sandbox: the per frame `Client.OnTick` and `Input.OnKeyPress`.
- Timers (`ScriptTimer.SetTimeout`).
- Entity spawning and manipulation on the server (`Character`, `Light`, `Trigger`).

## Running it

Drop the `OrbRush` folder into a server's `Packages/` directory and load it, then connect with a
NanosSharp client. On a WebAssembly client the whole `Client/Hud.cs` logic runs inside the sandbox.
