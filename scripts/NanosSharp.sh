#!/usr/bin/env sh
cd "$(dirname "$(readlink -f "$0")")"
if [ -z "$DOTNET_ROOT" ] && [ -x "$HOME/.dotnet/dotnet" ]; then
    export DOTNET_ROOT="$HOME/.dotnet"
fi
export LD_PRELOAD="$PWD/NanosSharp/runtime/libnanossharp.so${LD_PRELOAD:+:$LD_PRELOAD}"
exec ./NanosWorldServer.sh "$@"
