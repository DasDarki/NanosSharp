use anyhow::{anyhow, Result};
use wasmtime::{Caller, Engine, Extern, Linker, Module, Store};
use wasmtime_wasi::preview1::{self, WasiP1Ctx};
use wasmtime_wasi::{DirPerms, FilePerms, WasiCtxBuilder};

struct State {
    wasi: WasiP1Ctx,
}

fn memory_of(caller: &mut Caller<'_, State>) -> wasmtime::Memory {
    match caller.get_export("memory") {
        Some(Extern::Memory(memory)) => memory,
        _ => panic!("core module exported no memory"),
    }
}

fn put_str(blob: &mut Vec<u8>, text: &str) {
    blob.extend_from_slice(&(text.len() as u16).to_le_bytes());
    blob.extend_from_slice(text.as_bytes());
}

fn put_member(blob: &mut Vec<u8>, handle: i32, name: &str) {
    blob.extend_from_slice(&handle.to_le_bytes());
    put_str(blob, name);
}

fn build_schema_blob() -> Vec<u8> {
    let mut blob = Vec::new();

    blob.extend_from_slice(&2u32.to_le_bytes());

    blob.extend_from_slice(&1001u64.to_le_bytes());
    put_str(&mut blob, "Player");
    blob.extend_from_slice(&10i32.to_le_bytes());
    put_str(&mut blob, "Constructor");
    blob.extend_from_slice(&2u16.to_le_bytes());
    put_member(&mut blob, 11, "GetName");
    put_member(&mut blob, 12, "SetHealth");
    blob.extend_from_slice(&1u16.to_le_bytes());
    put_member(&mut blob, 13, "GetAll");
    blob.extend_from_slice(&1u16.to_le_bytes());
    put_member(&mut blob, 14, "Health");

    blob.extend_from_slice(&1002u64.to_le_bytes());
    put_str(&mut blob, "Vehicle");
    blob.extend_from_slice(&0i32.to_le_bytes());
    blob.extend_from_slice(&1u16.to_le_bytes());
    put_member(&mut blob, 21, "Honk");
    blob.extend_from_slice(&0u16.to_le_bytes());
    blob.extend_from_slice(&0u16.to_le_bytes());

    blob.extend_from_slice(&1u32.to_le_bytes());
    put_member(&mut blob, 31, "GetWorldTime");

    blob
}

fn main() -> Result<()> {
    let cli: Vec<String> = std::env::args().collect();
    let module_path = cli
        .get(1)
        .cloned()
        .ok_or_else(|| anyhow!("usage: devhost-core <core-module.wasm> [package-dir]"))?;

    let engine = Engine::default();
    let module = Module::from_file(&engine, &module_path)?;
    let mut linker: Linker<State> = Linker::new(&engine);

    preview1::add_to_linker_sync(&mut linker, |s: &mut State| &mut s.wasi)?;
    linker.func_wrap("wasi_snapshot_preview1", "adapter_close_badfd", |_c: Caller<'_, State>, _fd: i32| 0i32)?;

    linker.func_wrap("nanos", "log", |mut caller: Caller<'_, State>, level: i32, ptr: i32, len: i32| {
        let memory = memory_of(&mut caller);
        let data = memory.data(&caller);
        let text = String::from_utf8_lossy(&data[ptr as usize..(ptr + len) as usize]).into_owned();
        println!("[guest/{level}] {text}");
    })?;
    linker.func_wrap("nanos", "engine-is-attached", |_caller: Caller<'_, State>| 0i32)?;
    linker.func_wrap(
        "nanos",
        "function-call",
        |mut caller: Caller<'_, State>, _method: i32, env: i32, args: i32, count: i32, result_ptr: i32, _error: i32| -> i32 {
            let memory = memory_of(&mut caller);
            let base = result_ptr as usize;

            if count == 0 {
                let data = memory.data_mut(&mut caller);
                data[base] = 1; // ValueType.Integer
                data[base + 8..base + 16].copy_from_slice(&42i64.to_le_bytes());
                return 1;
            }

            let _ = env;
            let argument = args as usize;
            let data = memory.data(&caller);
            let argument_type = data[argument];
            let string_data = i32::from_le_bytes(data[argument + 8..argument + 12].try_into().unwrap()) as usize;
            let string_length = i64::from_le_bytes(data[argument + 16..argument + 24].try_into().unwrap());
            let received =
                String::from_utf8_lossy(&data[string_data..string_data + string_length as usize]).into_owned();
            println!("[host] string arg: type={argument_type} length={string_length} '{received}'");

            let data = memory.data_mut(&mut caller);
            data[base] = 1; // ValueType.Integer
            data[base + 8..base + 16].copy_from_slice(&string_length.to_le_bytes());
            1
        },
    )?;

    linker.func_wrap(
        "nanos",
        "schema-dump",
        |mut caller: Caller<'_, State>, buffer: i32, capacity: i32| -> i32 {
            let blob = build_schema_blob();
            let needed = blob.len() as i32;
            if needed <= capacity {
                let memory = memory_of(&mut caller);
                let data = memory.data_mut(&mut caller);
                let base = buffer as usize;
                data[base..base + blob.len()].copy_from_slice(&blob);
            }
            needed
        },
    )?;

    linker.define_unknown_imports_as_traps(&module)?;

    let mut builder = WasiCtxBuilder::new();
    builder.inherit_stdio().args(&["nanossharp"]);
    if let Some(dir) = cli.get(2) {
        builder.preopened_dir(dir, "/package", DirPerms::READ, FilePerms::READ)?;
    }

    let mut store = Store::new(&engine, State { wasi: builder.build_p1() });
    let instance = linker.instantiate(&mut store, &module)?;
    let start = instance.get_typed_func::<(), ()>(&mut store, "_start")?;
    start.call(&mut store, ())?;
    println!("devhost-core: guest returned cleanly");
    Ok(())
}
