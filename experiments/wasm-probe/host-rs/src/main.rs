// Component host standing in for the native game client.
//
// It embeds wasmtime, supplies the `nanos` import interface (the slice of the
// future NativeApi table) and the WASI imports, then drives the guest's
// `wasi:cli/run` export. If this prints the guest's log lines and the integer
// round trip, managed <-> host works in both directions over the real
// toolchain: guest -> host (the nanos imports) and host -> guest (run).

use std::collections::{HashMap, VecDeque};

use anyhow::{anyhow, Result};
use wasmtime::component::{Component, Linker, ResourceTable};
use wasmtime::{Engine, Store};
use wasmtime_wasi::{WasiCtx, WasiCtxBuilder, WasiView};
use wasmtime_wasi_http::{WasiHttpCtx, WasiHttpView};

struct HostState {
    wasi: WasiCtx,
    http: WasiHttpCtx,
    table: ResourceTable,
    values: HashMap<i32, i64>,
    next_handle: i32,
    events: VecDeque<i32>,
}

impl WasiView for HostState {
    fn ctx(&mut self) -> &mut WasiCtx {
        &mut self.wasi
    }

    fn table(&mut self) -> &mut ResourceTable {
        &mut self.table
    }
}

impl WasiHttpView for HostState {
    fn ctx(&mut self) -> &mut WasiHttpCtx {
        &mut self.http
    }

    fn table(&mut self) -> &mut ResourceTable {
        &mut self.table
    }
}

const TAG_BOOL: u8 = 0;
const TAG_INTEGER: u8 = 1;
const TAG_DOUBLE: u8 = 2;
const TAG_STRING: u8 = 3;
const RECORD_SIZE: usize = 32;

fn i32_at(buffer: &[u8], offset: usize) -> i32 {
    i32::from_le_bytes(buffer[offset..offset + 4].try_into().unwrap())
}

fn i64_at(buffer: &[u8], offset: usize) -> i64 {
    i64::from_le_bytes(buffer[offset..offset + 8].try_into().unwrap())
}

fn unpack_and_reduce(buffer: &[u8]) -> i64 {
    let count = i32_at(buffer, 0) as usize;
    let mut accumulator: i64 = 0;
    for index in 0..count {
        let record = 4 + index * RECORD_SIZE;
        match buffer[record] {
            TAG_BOOL => accumulator += buffer[record + 8] as i64,
            TAG_INTEGER => accumulator += i64_at(buffer, record + 8),
            TAG_DOUBLE => accumulator += f64::from_bits(i64_at(buffer, record + 8) as u64) as i64,
            TAG_STRING => {
                let offset = i32_at(buffer, record + 8) as usize;
                let length = i32_at(buffer, record + 12) as usize;
                let text = String::from_utf8_lossy(&buffer[offset..offset + length]);
                println!("[host] function-call string arg: {:?}", text);
                accumulator += length as i64;
            }
            other => println!("[host] function-call unknown tag {other}"),
        }
    }
    accumulator
}

fn main() -> Result<()> {
    let cli: Vec<String> = std::env::args().collect();
    let module_path = cli
        .get(1)
        .cloned()
        .unwrap_or_else(|| "../out/guest.component.wasm".to_string());
    let mut guest_args = vec!["guest".to_string()];
    guest_args.extend(cli.iter().skip(2).cloned());

    let engine = Engine::default();
    let component = Component::from_file(&engine, &module_path)?;

    let mut linker: Linker<HostState> = Linker::new(&engine);
    wasmtime_wasi::add_to_linker_sync(&mut linker)?;
    wasmtime_wasi_http::add_only_http_to_linker_sync(&mut linker)?;

    // The nanos import interface: the WebAssembly form of NativeApi.
    let mut nanos = linker.instance("nanos")?;

    nanos.func_wrap(
        "log",
        |_caller, (message,): (Vec<u8>,)| {
            println!("[guest] {}", String::from_utf8_lossy(&message));
            Ok(())
        },
    )?;

    nanos.func_wrap(
        "value-create-integer",
        |mut caller: wasmtime::StoreContextMut<'_, HostState>, (value,): (i64,)| {
            let state = caller.data_mut();
            let handle = state.next_handle;
            state.next_handle += 1;
            state.values.insert(handle, value);
            Ok((handle,))
        },
    )?;

    nanos.func_wrap(
        "value-get-integer",
        |caller: wasmtime::StoreContextMut<'_, HostState>, (handle,): (i32,)| {
            let value = caller.data().values.get(&handle).copied().unwrap_or(0);
            Ok((value,))
        },
    )?;

    nanos.func_wrap(
        "function-call",
        |_caller, (_method, arguments): (i32, Vec<u8>)| Ok((unpack_and_reduce(&arguments),)),
    )?;

    nanos.func_wrap(
        "next-event",
        |mut caller: wasmtime::StoreContextMut<'_, HostState>, (): ()| {
            let state = caller.data_mut();
            Ok((state.events.pop_front().unwrap_or(-1),))
        },
    )?;

    nanos.func_wrap("nop", |_caller, (value,): (i32,)| Ok((value.wrapping_add(1),)))?;

    let mut store = Store::new(
        &engine,
        HostState {
            wasi: WasiCtxBuilder::new().inherit_stdio().args(&guest_args).build(),
            http: WasiHttpCtx::new(),
            table: ResourceTable::new(),
            values: HashMap::new(),
            next_handle: 1,
            events: VecDeque::from(vec![10, 20, 30]),
        },
    );

    let instance = linker.instantiate(&mut store, &component)?;

    let run_instance = instance
        .get_export(&mut store, None, "wasi:cli/run@0.2.0")
        .ok_or_else(|| anyhow!("component does not export wasi:cli/run"))?;
    let run_export = instance
        .get_export(&mut store, Some(&run_instance), "run")
        .ok_or_else(|| anyhow!("wasi:cli/run has no run function"))?;
    let run = instance.get_typed_func::<(), (Result<(), ()>,)>(&mut store, &run_export)?;

    match run.call(&mut store, ())? {
        (Ok(()),) => {
            println!("host: guest returned cleanly");
            Ok(())
        }
        (Err(()),) => Err(anyhow!("guest run returned an error")),
    }
}
