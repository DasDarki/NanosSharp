#!/usr/bin/env sh
cd "$(dirname "$(readlink -f "$0")")"
if [ -z "$DOTNET_ROOT" ] && [ -x "$HOME/.dotnet/dotnet" ]; then
    export DOTNET_ROOT="$HOME/.dotnet"
fi
if [ -x "$PWD/NanosSharp/nanossharp-install" ]; then
    "$PWD/NanosSharp/nanossharp-install" server --path "$PWD" --update --if-enabled --quiet --yes 2>/dev/null || true
fi
export LD_PRELOAD="$PWD/NanosSharp/runtime/libnanossharp.so${LD_PRELOAD:+:$LD_PRELOAD}"
exec ./NanosWorldServer.sh "$@"
