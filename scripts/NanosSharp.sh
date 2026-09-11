#!/usr/bin/env sh
cd "$(dirname "$(readlink -f "$0")")"
export LD_PRELOAD="$PWD/NanosSharp/runtime/libnanossharp.so${LD_PRELOAD:+:$LD_PRELOAD}"
exec ./NanosWorldServer.sh "$@"
