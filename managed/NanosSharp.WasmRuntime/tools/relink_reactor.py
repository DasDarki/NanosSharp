#!/usr/bin/env python3
"""Relink a Mono-wasi command build into a WASI reactor core module.

The .NET WASI SDK only emits command modules (crt1-command + a main() that runs
the managed entry once and exits). A reactor is needed so the native host can
call guest exports repeatedly against a live runtime. This takes the object
files the SDK already produced, recompiles the runtime main.c in
WASM_LIBRARY_MODE, and relinks with wasm-ld using crt1-reactor so the module
exports _initialize + initialize_runtime + the managed [UnmanagedCallersOnly]
entry points instead of _start.

With AOT enabled the SDK writes each assembly's object to a temporary path and
deletes it after the build, so those objects are regenerated here from the
compile commands captured in the publish log before relinking.

Usage: relink_reactor.py <link-cwd> <exports-csv> <output.wasm> <publish-log>
"""
import glob
import os
import re
import shlex
import subprocess
import sys

WASI = os.environ.get("WASI_SDK_PATH", os.path.expanduser("~/.wasi-sdk"))
RT_GLOB = "/usr/share/dotnet/packs/Microsoft.NETCore.App.Runtime.Mono.wasi-wasm/*/runtimes/wasi-wasm/native"

cwd = os.path.abspath(sys.argv[1])
exports = [e for e in sys.argv[2].split(",") if e]
out = os.path.abspath(sys.argv[3])
log_path = sys.argv[4]

runtime_native = sorted(glob.glob(RT_GLOB))[-1]

with open(log_path, errors="replace") as f:
    log_lines = f.read().splitlines()

link_line = next(l for l in reversed(log_lines)
                 if "wasm-component-ld" in l and "crt1-command.o" in l)
tokens = shlex.split(link_line)

wasm_ld = None
main_o = None
for i, t in enumerate(tokens):
    if t == "--wasm-ld-path":
        wasm_ld = tokens[i + 1]
    if t.endswith("/main.o"):
        main_o = t
assert wasm_ld and main_o, "could not find wasm-ld path or main.o in link command"

for_publish = os.path.dirname(main_o if os.path.isabs(main_o) else os.path.join(cwd, main_o))
scratch = os.path.join(for_publish, "reactor")
os.makedirs(scratch, exist_ok=True)


def clang(args):
    subprocess.run([os.path.join(WASI, "bin/clang")] + args, check=True)


# runtime main.c in reactor (library) mode
reactor_main = os.path.join(scratch, "reactor_main.o")
clang(["--target=wasm32-unknown-wasip2", f"--sysroot={WASI}/share/wasi-sysroot",
       "-O2", "-fvisibility=hidden", "-DWASM_LIBRARY_MODE=1",
       f"-I{runtime_native}/include/wasm", f"-I{runtime_native}/include/mono-2.0",
       "-c", f"{runtime_native}/src/main.c", "-o", reactor_main])

# the entrypoint stub (its temp object is already deleted)
entrypoint_src = glob.glob(os.path.join(for_publish, "entrypoint_*.dll.c"))[0]
entrypoint_o = os.path.join(scratch, "reactor_entrypoint.o")
clang(["--target=wasm32-unknown-wasip2", f"--sysroot={WASI}/share/wasi-sysroot",
       "-O2", "-fvisibility=hidden", "-c", entrypoint_src, "-o", entrypoint_o])

# regenerate any deleted temporary AOT objects from their compile commands
tmp_compiles = {}
for line in log_lines:
    m = re.search(r"-o (/tmp/[A-Za-z0-9_.-]+\.o) ", line)
    if m and "-x ir" in line and "clang" in line:
        tmp_compiles[m.group(1)] = line


def regenerate(tmp_o):
    line = tmp_compiles.get(tmp_o)
    if line is None:
        raise SystemExit(f"no compile command found for {tmp_o}")
    parts = shlex.split(line)
    dest = os.path.join(scratch, os.path.basename(tmp_o))
    parts = [dest if p == tmp_o else p for p in parts]
    subprocess.run(parts, check=True, cwd=cwd)
    return dest


link = [wasm_ld]
skip = False
i = 1
while i < len(tokens):
    t = tokens[i]
    if skip:
        skip = False; i += 1; continue
    if t in ("-m", "--wasm-ld-path", "--component-type"):
        skip = True; i += 1; continue
    if t == "-s":
        i += 1; continue
    if t.endswith("crt1-command.o"):
        link.append(t.replace("crt1-command.o", "crt1-reactor.o")); i += 1; continue
    if t.endswith("/main.o"):
        link.append(reactor_main); i += 1; continue
    if "/entrypoint_" in t and t.endswith(".o"):
        link.append(entrypoint_o); i += 1; continue
    if t.startswith("/tmp/") and t.endswith(".o"):
        link.append(regenerate(t)); i += 1; continue
    if t == "-o":
        link.append("-o"); link.append(out); skip = True; i += 1; continue
    link.append(t); i += 1

link.append("--no-entry")
for e in ["_initialize", "initialize_runtime"] + exports:
    link.append(f"--export={e}")

subprocess.run(link, check=True, cwd=cwd)
print(f"reactor core module -> {out}")
