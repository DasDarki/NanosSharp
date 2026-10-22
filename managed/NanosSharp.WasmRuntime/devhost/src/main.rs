use std::collections::VecDeque;

use anyhow::{anyhow, Result};
use wasmtime::component::{Component, Linker, ResourceTable};
use wasmtime::{Engine, Store};
use wasmtime_wasi::{DirPerms, FilePerms, WasiCtx, WasiCtxBuilder, WasiView};
use wasmtime_wasi_http::{WasiHttpCtx, WasiHttpView};

struct HostState {
    wasi: WasiCtx,
    http: WasiHttpCtx,
    table: ResourceTable,
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

fn main() -> Result<()> {
    let module_path = std::env::args()
        .nth(1)
        .ok_or_else(|| anyhow!("usage: devhost <component.wasm>"))?;

    let engine = Engine::default();
    let component = Component::from_file(&engine, &module_path)?;
    let mut linker: Linker<HostState> = Linker::new(&engine);
    wasmtime_wasi::add_to_linker_sync(&mut linker)?;
    wasmtime_wasi_http::add_only_http_to_linker_sync(&mut linker)?;

    let mut nanos = linker.instance("nanos")?;
    nanos.func_wrap(
        "engine-is-attached",
        |_caller, (): ()| {
            eprintln!("[devhost] engine-is-attached called");
            Ok((0i32,))
        },
    )?;
    nanos.func_wrap(
        "log",
        |_caller, (level, message): (i32, Vec<u8>)| {
            println!("[guest/{level}] {}", String::from_utf8_lossy(&message));
            Ok(())
        },
    )?;

    let mut wasi_builder = WasiCtxBuilder::new();
    wasi_builder.inherit_stdio().args(&["nanossharp"]);
    if let Some(package_dir) = std::env::args().nth(2) {
        wasi_builder.preopened_dir(&package_dir, "/package", DirPerms::READ, FilePerms::READ)?;
    }

    let mut store = Store::new(
        &engine,
        HostState {
            wasi: wasi_builder.build(),
            http: WasiHttpCtx::new(),
            table: ResourceTable::new(),
            events: VecDeque::new(),
        },
    );

    let instance = linker.instantiate(&mut store, &component)?;
    let run_instance = instance
        .get_export(&mut store, None, "wasi:cli/run@0.2.0")
        .ok_or_else(|| anyhow!("no wasi:cli/run export"))?;
    let run_export = instance
        .get_export(&mut store, Some(&run_instance), "run")
        .ok_or_else(|| anyhow!("no run func"))?;
    let run = instance.get_typed_func::<(), (Result<(), ()>,)>(&mut store, &run_export)?;
    match run.call(&mut store, ())? {
        (Ok(()),) => {
            println!("devhost: guest returned cleanly");
            Ok(())
        }
        (Err(()),) => Err(anyhow!("guest run returned an error")),
    }
}
