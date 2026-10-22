#!/usr/bin/env python3
import re, sys, pathlib

SRC = pathlib.Path("managed/NanosSharp/Interop/NativeApi.cs")
OUT_CS = pathlib.Path("managed/NanosSharp/Interop/NativeApi.Wasm.g.cs")
OUT_WIT = pathlib.Path("managed/NanosSharp.WasmRuntime/wit/nanos.wit")

# wasm-only batching imports that are declared by hand on the wasm side rather
# than mirrored from a NativeApi field. They must stay in the generated WIT.
WASM_ONLY_WIT = [
    "        schema-dump: func(buffer: s32, capacity: s32) -> s32;",
    "        set-result-scratch: func(buffer: s32, capacity: s32);",
]

text = SRC.read_text()
# capture: public delegate* unmanaged<TYPES> Name;
fields = re.findall(r"public delegate\* unmanaged<([^>]+)>\s+(\w+);", text)

def split_types(s):
    return [t.strip() for t in s.split(",")]

def kebab(name):
    out = []
    for i, c in enumerate(name):
        if c.isupper() and i > 0:
            out.append("-")
        out.append(c.lower())
    return "".join(out)

def wit_type(cs):
    cs = cs.strip()
    if cs in ("long", "ulong"):
        return "s64"
    if cs == "double":
        return "f64"
    if cs == "float":
        return "f32"
    # byte/sbyte/int/uint/nint and every pointer lower to i32 in wasm32
    return "s32"

methods = []
imports = []
wit_funcs = []
for types_s, name in fields:
    types = split_types(types_s)
    ret = types[-1]
    params = types[:-1]
    pnames = [f"a{i}" for i in range(len(params))]
    sig_params = ", ".join(f"{p} {n}" for p, n in zip(params, pnames))
    call_args = ", ".join(pnames)
    methods.append(f"    public {ret} {name}({sig_params}) => Host{name}({call_args});")
    imports.append(
        f"    [LibraryImport(\"nanos\", EntryPoint = \"{kebab(name)}\")]\n"
        f"    [WasmImportLinkage]\n"
        f"    [MethodImpl(MethodImplOptions.AggressiveOptimization)]\n"
        f"    private static partial {ret} Host{name}({sig_params});")
    wit_params = ", ".join(f"{n.replace('a','p')}: {wit_type(p)}" for p, n in zip(params, pnames))
    wit_ret = "" if ret == "void" else f" -> {wit_type(ret)}"
    wit_funcs.append(f"        {kebab(name)}: func({wit_params}){wit_ret};")

cs = "#if NANOSSHARP_WASM\n"
cs += "using System.Runtime.CompilerServices;\n"
cs += "using System.Runtime.InteropServices;\n"
cs += "using NanosSharp.Interop.Arguments;\n\n"
cs += "namespace NanosSharp.Interop;\n\n"
cs += "internal unsafe partial struct NativeApi\n{\n"
cs += "\n".join(methods) + "\n\n"
cs += "\n".join(imports) + "\n"
cs += "}\n#endif\n"
OUT_CS.write_text(cs)

wit = "package nanos:runtime@0.1.0;\n\nworld sandbox {\n    import nanos: interface {\n"
wit += "\n".join(wit_funcs + WASM_ONLY_WIT) + "\n    }\n}\n"
OUT_WIT.write_text(wit)
print(f"generated {len(fields)} functions -> {OUT_CS} + {OUT_WIT}")
