use std::collections::HashMap;

use anyhow::{anyhow, Result};
use wasmtime::{Caller, Engine, Extern, Linker, Module, Store};
use wasmtime_wasi::preview1::{self, WasiP1Ctx};
use wasmtime_wasi::WasiCtxBuilder;

struct State {
    wasi: WasiP1Ctx,
    values: HashMap<i32, i64>,
    next_handle: i32,
}

const TAG_INTEGER: u8 = 1;
const TAG_STRING: u8 = 3;
const RECORD_SIZE: usize = 32;

fn memory_of(caller: &mut Caller<'_, State>) -> wasmtime::Memory {
    match caller.get_export("memory") {
        Some(Extern::Memory(memory)) => memory,
        _ => panic!("core module exported no memory"),
    }
}

fn reduce(buffer: &[u8]) -> i64 {
    let count = i32::from_le_bytes(buffer[0..4].try_into().unwrap()) as usize;
    let mut accumulator: i64 = 0;
    for index in 0..count {
        let record = 4 + index * RECORD_SIZE;
        match buffer[record] {
            TAG_INTEGER => {
                accumulator += i64::from_le_bytes(buffer[record + 8..record + 16].try_into().unwrap())
            }
            TAG_STRING => {
                let offset = i32::from_le_bytes(buffer[record + 8..record + 12].try_into().unwrap()) as usize;
                let length = i32::from_le_bytes(buffer[record + 12..record + 16].try_into().unwrap()) as usize;
                accumulator += length as i64;
                let _ = &buffer[offset..offset + length];
            }
            _ => {}
        }
    }
    accumulator
}

fn main() -> Result<()> {
    let cli: Vec<String> = std::env::args().collect();
    let module_path = cli
        .get(1)
        .cloned()
        .ok_or_else(|| anyhow!("usage: host <core-module.wasm> [guest args...]"))?;
    let mut guest_args = vec!["guest".to_string()];
    guest_args.extend(cli.iter().skip(2).cloned());

    let engine = Engine::default();
    let module = Module::from_file(&engine, &module_path)?;
    let mut linker: Linker<State> = Linker::new(&engine);

    preview1::add_to_linker_sync(&mut linker, |s: &mut State| &mut s.wasi)?;
    linker.func_wrap("wasi_snapshot_preview1", "adapter_close_badfd", |_c: Caller<'_, State>, _fd: i32| 0i32)?;

    linker.func_wrap("nanos", "nop", |_c: Caller<'_, State>, value: i32| value.wrapping_add(1))?;
    linker.func_wrap("nanos", "value-create-integer", |mut c: Caller<'_, State>, value: i64| {
        let s = c.data_mut();
        let handle = s.next_handle;
        s.next_handle += 1;
        s.values.insert(handle, value);
        handle
    })?;
    linker.func_wrap("nanos", "value-get-integer", |c: Caller<'_, State>, handle: i32| {
        c.data().values.get(&handle).copied().unwrap_or(0)
    })?;
    linker.func_wrap("nanos", "next-event", |_c: Caller<'_, State>| -1i32)?;
    linker.func_wrap("nanos", "log", |mut c: Caller<'_, State>, ptr: i32, len: i32| {
        let memory = memory_of(&mut c);
        let data = memory.data(&c);
        let text = String::from_utf8_lossy(&data[ptr as usize..(ptr + len) as usize]).into_owned();
        println!("[guest] {text}");
    })?;
    linker.func_wrap("nanos", "function-call", |mut c: Caller<'_, State>, _method: i32, ptr: i32, len: i32| {
        let memory = memory_of(&mut c);
        let data = memory.data(&c);
        reduce(&data[ptr as usize..(ptr + len) as usize])
    })?;

    linker.define_unknown_imports_as_traps(&module)?;

    let mut store = Store::new(
        &engine,
        State {
            wasi: WasiCtxBuilder::new().inherit_stdio().args(&guest_args).build_p1(),
            values: HashMap::new(),
            next_handle: 1,
        },
    );

    let instance = linker.instantiate(&mut store, &module)?;
    let start = instance.get_typed_func::<(), ()>(&mut store, "_start")?;
    start.call(&mut store, ())?;
    println!("host: guest returned cleanly");
    Ok(())
}
