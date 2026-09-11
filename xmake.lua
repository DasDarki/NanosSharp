set_project("NanosSharp")
set_version("0.1.0")
set_xmakever("3.0.0")

set_allowedplats("linux")
set_allowedarchs("x86_64")

add_rules("mode.debug", "mode.release")
set_defaultmode("release")

add_requires("funchook", {configs = {disasm = "distorm"}})

option("dotnet_root")
    set_default("/usr/share/dotnet")
    set_showmenu(true)
    set_description("Root directory of the .NET SDK providing nethost")
option_end()

option("server_dir")
    set_default(".server")
    set_showmenu(true)
    set_description("nanos world server directory used by the deploy target")
option_end()

target("nanossharp")
    set_kind("shared")
    set_languages("c++20")
    set_basename("nanossharp")
    set_symbols("hidden")
    set_warnings("all", "extra")
    add_files("native/src/**.cc")
    add_includedirs("native/src")
    add_defines("_GLIBCXX_USE_CXX11_ABI=0")
    add_cxflags("-fPIC", "-fno-plt")
    add_shflags("-Wl,-z,defs", {force = true})
    add_packages("funchook")
    add_syslinks("dl", "pthread")

    after_build(function (target)
        local symbols = os.iorunv("objdump", {"-T", target:targetfile()})
        for minor in symbols:gmatch("GLIBCXX_3%.4%.(%d+)") do
            if tonumber(minor) > 32 then
                raise("%s requires GLIBCXX_3.4.%s, nanos world only ships GLIBCXX_3.4.32", target:targetfile(), minor)
            end
        end
    end)

    on_load(function (target)
        import("core.base.semver")
        local nethost_dir
        local hosts = os.dirs(path.join(get_config("dotnet_root"), "packs", "Microsoft.NETCore.App.Host.linux-x64", "*"))
        table.sort(hosts, function (a, b)
            return semver.compare(path.filename(a), path.filename(b)) > 0
        end)
        for _, host in ipairs(hosts) do
            local native_dir = path.join(host, "runtimes", "linux-x64", "native")
            if not nethost_dir and os.isfile(path.join(native_dir, "libnethost.a")) then
                nethost_dir = native_dir
            end
        end
        if not nethost_dir then
            raise("nethost was not found, set --dotnet_root to a .NET SDK installation")
        end
        target:add("includedirs", nethost_dir)
        target:add("linkdirs", nethost_dir)
        target:add("links", "libnethost.a")
    end)
target_end()

task("deploy")
    set_category("plugin")
    on_run(function ()
        import("core.base.option")
        import("core.project.config")
        import("core.project.project")

        config.load()
        local root = os.projectdir()
        local server_dir = path.absolute(option.get("server") or config.get("server_dir") or ".server", root)
        local configuration = option.get("configuration")
        if not os.isfile(path.join(server_dir, "NanosWorldServer")) then
            raise("no nanos world server found in %s", server_dir)
        end

        os.execv("xmake", {"build", "nanossharp"})
        os.execv("dotnet", {"build", path.join(root, "managed", "NanosSharp.slnx"), "-c", configuration, "--nologo", "-v", "q"})

        local runtime_dir = path.join(server_dir, "NanosSharp", "runtime")
        os.tryrm(runtime_dir)
        os.mkdir(runtime_dir)
        os.cp(path.join(root, "build", "managed", "bin", "NanosSharp.Host", configuration, "net10.0", "*"), runtime_dir)
        os.cp(project.target("nanossharp"):targetfile(), runtime_dir)
        os.cp(path.join(root, "scripts", "NanosSharp.sh"), server_dir)

        for _, example_dir in ipairs(os.dirs(path.join(root, "managed", "examples", "*"))) do
            local name = path.filename(example_dir)
            local package_dir = path.join(server_dir, "Packages", "nanossharp-" .. name:lower())
            os.mkdir(path.join(package_dir, "Server"))
            os.cp(path.join(example_dir, "Package.toml"), package_dir)
            os.cp(path.join(root, "build", "managed", "bin", name, configuration, "net10.0", name .. ".*"), path.join(package_dir, "Server"))
            os.tryrm(path.join(package_dir, "Server", name .. ".deps.json"))
        end

        cprint("${color.success}NanosSharp deployed to %s, start the server with ./NanosSharp.sh", server_dir)
    end)
    set_menu {
        usage = "xmake deploy [options]",
        description = "Build NanosSharp and deploy it with the examples into a nanos world server.",
        options = {
            {"s", "server", "kv", nil, "The nanos world server directory."},
            {"c", "configuration", "kv", "Debug", "The .NET build configuration.", values = {"Debug", "Release"}}
        }
    }
task_end()
