set_project("NanosSharp")
set_version("0.1.0")
set_xmakever("3.0.0")

set_allowedplats("linux")
set_allowedarchs("x86_64")

add_rules("mode.debug", "mode.release", "mode.releasedbg")
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
    add_files("native/src/bridge/*.cc", "native/src/host/*.cc", "native/src/platform/linux/*.cc")
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
        for minor in symbols:gmatch("GLIBC_2%.(%d+)") do
            if tonumber(minor) > 34 then
                raise("%s requires GLIBC_2.%s, nanos world only requires GLIBC_2.34", target:targetfile(), minor)
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

task("bundle")
    set_category("plugin")
    on_run(function ()
        import("core.base.option")
        import("core.project.config")
        import("core.project.project")

        config.load()
        local root = os.projectdir()
        local version = option.get("release") or project.version()
        local release_dir = path.join(root, "build", "release")
        local bundle_dir = path.join(release_dir, "bundle")
        local runtime_dir = path.join(bundle_dir, "NanosSharp", "runtime")
        local archive = path.join(release_dir, "nanossharp-" .. version .. "-linux-x64.tar.gz")

        os.execv("xmake", {"build", "nanossharp"})
        os.execv("dotnet", {"build", path.join(root, "managed", "NanosSharp.Host", "NanosSharp.Host.csproj"), "-c", "Release",
            "-p:Version=" .. version, "--nologo", "-v", "q"})

        os.tryrm(release_dir)
        os.mkdir(runtime_dir)
        os.cp(path.join(root, "build", "managed", "bin", "NanosSharp.Host", "Release", "net10.0", "*"), runtime_dir)
        os.cp(project.target("nanossharp"):targetfile(), runtime_dir)
        os.cp(path.join(root, "scripts", "NanosSharp.sh"), bundle_dir)
        os.cp(path.join(root, "LICENSE"), path.join(bundle_dir, "NanosSharp"))
        os.execv("tar", {"-czf", archive, "-C", bundle_dir, "."})

        cprint("${color.success}NanosSharp %s bundled into %s", version, archive)
    end)
    set_menu {
        usage = "xmake bundle [-r version]",
        description = "Build the NanosSharp runtime and pack it into a release archive for nanos world servers.",
        options = {
            {"r", "release", "kv", nil, "The version of the release, the project version by default."}
        }
    }
task_end()

task("client")
    set_category("plugin")
    on_run(function ()
        import("core.base.option")
        import("core.project.config")
        import("lib.detect.find_tool")
        import("net.http")
        import("utils.archive")

        config.load()
        local root = os.projectdir()
        local is_windows_host = os.host() == "windows"
        local output_dir = path.join(root, "build", "client")
        local object_dir = path.join(root, "build", "windows", "obj")
        local package_dir = path.join(output_dir, "NanosSharp")
        local runtime_version = option.get("runtime")
        local configuration = option.get("configuration")

        local dotnet_root = get_config("dotnet_root")
        local header_dirs = os.dirs(path.join(dotnet_root, "packs", "Microsoft.NETCore.App.Host.*", "*", "runtimes", "*", "native"))
        local header_dir = nil
        for _, dir in ipairs(header_dirs) do
            if os.isfile(path.join(dir, "hostfxr.h")) then
                header_dir = dir
            end
        end
        if not header_dir then
            raise("hostfxr.h was not found in the .NET SDK packs of %s", dotnet_root)
        end

        local system_flags = {}
        local library_flags = {}
        if not is_windows_host then
            local xwin = option.get("xwin") or path.join(os.getenv("HOME"), ".xwin")
            if not os.isdir(xwin) then
                raise("the MSVC CRT and Windows SDK were not found in %s, install them with xwin", xwin)
            end
            for _, dir in ipairs({"crt/include", "sdk/include/ucrt", "sdk/include/um", "sdk/include/shared"}) do
                table.insert(system_flags, "/imsvc" .. path.join(xwin, dir))
            end
            for _, dir in ipairs({"crt/lib/x86_64", "sdk/lib/um/x86_64", "sdk/lib/ucrt/x86_64"}) do
                table.insert(library_flags, "/libpath:" .. path.join(xwin, dir))
            end
        end

        os.tryrm(object_dir)
        os.mkdir(object_dir)
        local sources = {}
        for _, pattern in ipairs({"bridge/*.cc", "host/*.cc", "platform/windows/*.cc"}) do
            table.join2(sources, os.files(path.join(root, "native", "src", pattern)))
        end

        local objects = {}
        for _, source in ipairs(sources) do
            local object = path.join(object_dir, path.basename(source) .. ".obj")
            local arguments = {"/nologo", "/c", "/std:c++20", "/EHsc", "/MD", "/O2", "/W3", "/DNOMINMAX",
                "/DWIN32_LEAN_AND_MEAN", "/I" .. path.join(root, "native", "src"), "/imsvc" .. header_dir}
            table.join2(arguments, system_flags)
            table.join2(arguments, {source, "/Fo" .. object})
            os.execv("clang-cl", arguments)
            table.insert(objects, object)
        end

        local stubs = path.join(object_dir, "version_stubs.obj")
        os.execv("clang", {"--target=x86_64-pc-windows-msvc", "-c",
            path.join(root, "native", "src", "platform", "windows", "version_stubs.S"), "-o", stubs})
        table.insert(objects, stubs)

        os.tryrm(output_dir)
        os.mkdir(package_dir)
        local link_arguments = {"/nologo", "/DLL", "/DEF:" .. path.join(root, "native", "src", "platform", "windows", "version.def"),
            "/OUT:" .. path.join(output_dir, "version.dll"), "/IMPLIB:" .. path.join(object_dir, "version.lib"), "dbghelp.lib"}
        table.join2(link_arguments, library_flags)
        table.join2(link_arguments, objects)
        os.execv("lld-link", link_arguments)

        os.execv("dotnet", {"build", path.join(root, "managed", "NanosSharp.Host", "NanosSharp.Host.csproj"), "-c", configuration,
            "-p:NanosSharpSide=Client", "--nologo", "-v", "q"})
        os.mkdir(path.join(package_dir, "runtime"))
        os.cp(path.join(root, "build", "managed", "Client", "bin", "NanosSharp.Host", configuration, "net10.0", "*"), path.join(package_dir, "runtime"))

        local cache_dir = path.join(root, "build", "cache")
        local runtime_archive = path.join(cache_dir, "dotnet-runtime-" .. runtime_version .. "-win-x64.zip")
        if not os.isfile(runtime_archive) then
            os.mkdir(cache_dir)
            http.download("https://builds.dotnet.microsoft.com/dotnet/Runtime/" .. runtime_version .. "/dotnet-runtime-" ..
                runtime_version .. "-win-x64.zip", runtime_archive)
        end
        archive.extract(runtime_archive, path.join(package_dir, "dotnet"))
        os.cp(path.join(root, "LICENSE"), package_dir)

        cprint("${color.success}NanosSharp client built into %s, copy its content into NanosWorld/Binaries/Win64", output_dir)
    end)
    set_menu {
        usage = "xmake client [options]",
        description = "Build the NanosSharp client runtime for the Windows game client.",
        options = {
            {"c", "configuration", "kv", "Release", "The .NET build configuration.", values = {"Debug", "Release"}},
            {nil, "runtime", "kv", "10.0.13", "The .NET runtime version shipped with the client."},
            {nil, "xwin", "kv", nil, "Directory of the MSVC CRT and Windows SDK downloaded with xwin."}
        }
    }
task_end()

task("test")
    set_category("plugin")
    on_run(function ()
        import("core.base.option")
        import("core.project.config")

        config.load()
        local root = os.projectdir()
        local server_dir = path.absolute(option.get("server") or config.get("server_dir") or ".server", root)
        local deploy_arguments = {"deploy", "-s", server_dir, "-c", option.get("configuration")}
        os.execv("xmake", deploy_arguments)

        local packages = {}
        for _, test_dir in ipairs(os.dirs(path.join(root, option.get("directory"), "*"))) do
            local name = path.filename(test_dir)
            local package_dir = path.join(server_dir, "Packages", name)
            os.tryrm(package_dir)
            os.cp(test_dir, package_dir)
            table.insert(packages, name)
        end

        local log_file = path.join(root, "build", "test", option.get("directory") .. "-" .. option.get("port") .. ".log")
        os.mkdir(path.directory(log_file))
        local timeout = tostring(option.get("timeout"))
        local port = tonumber(option.get("port"))
        local server_arguments = {"-s", "INT", timeout, path.join(server_dir, "NanosSharp.sh"),
            "--packages", table.concat(packages, ","), "--port", tostring(port), "--query_port", tostring(port + 1), "--announce", "0", "--enable_http_server"}
        for _, argument in ipairs((option.get("arguments") or ""):split(" ")) do
            table.insert(server_arguments, argument)
        end
        cprint("${color.success}server is running on port %d (timeout %ss), follow the output with: tail -f %s", port, timeout, log_file)
        os.execv("timeout", server_arguments, {stdin = os.nuldev(), stdout = log_file, stderr = log_file, try = true})

        local passed = 0
        local failed = 0
        local summary = nil
        for line in io.readfile(log_file):gmatch("[^\n]+") do
            if line:find("[TEST] PASS", 1, true) then
                passed = passed + 1
                cprint("${color.success}%s", line)
            elseif line:find("[TEST] FAIL", 1, true) then
                failed = failed + 1
                cprint("${color.failure}%s", line)
            elseif line:find("[TEST] SUMMARY", 1, true) then
                summary = line
            elseif line:find("[BENCH]", 1, true) or line:find("[PROBE]", 1, true) then
                print(line)
            elseif line:find("FATAL", 1, true) or line:find("NanosSharp is disabled", 1, true) then
                cprint("${color.failure}%s", line)
            end
        end

        if summary == nil then
            raise("tests did not finish, see %s", log_file)
        end
        if failed > 0 then
            raise("%d of %d tests failed, see %s", failed, passed + failed, log_file)
        end
        cprint("${color.success}all %d tests passed", passed)
    end)
    set_menu {
        usage = "xmake test [options]",
        description = "Run the NanosSharp integration tests inside a nanos world server.",
        options = {
            {"s", "server", "kv", nil, "The nanos world server directory."},
            {"c", "configuration", "kv", "Release", "The .NET build configuration.", values = {"Debug", "Release"}},
            {"t", "timeout", "kv", "120", "Seconds until the server is stopped."},
            {"d", "directory", "kv", "tests", "Directory with the packages to run, for example benchmarks."},
            {"a", "arguments", "kv", nil, "Additional server arguments, for example '--steam_app game'."},
            {"p", "port", "kv", "7797", "Game port of the test server, the query port is the next one."}
        }
    }
task_end()

local api_definitions_repository = "https://github.com/nanos-world/api.git"
local api_definitions_revision = "47f4540c3d2bc452cb6d47979df747342c9c1fb7"

task("dump-api")
    set_category("plugin")
    on_run(function ()
        import("core.base.option")
        import("core.project.config")

        config.load()
        local root = os.projectdir()
        local server_dir = path.absolute(option.get("server") or config.get("server_dir") or ".server", root)
        local output = path.join(root, "managed", "tools", "NanosSharp.Generator", "Data", "native-api.json")
        os.execv("xmake", {"deploy", "-s", server_dir})
        os.tryrm(output)
        os.execv("timeout", {"-s", "INT", "20", path.join(server_dir, "NanosSharp.sh"),
            "--packages", "", "--port", "7797", "--query_port", "7798", "--announce", "0"},
            {envs = {NANOSSHARP_DUMP_API = output}, stdout = os.nuldev(), stderr = os.nuldev(), try = true})
        if not os.isfile(output) then
            raise("the server did not write %s", output)
        end
        cprint("${color.success}native API dumped to %s", output)
    end)
    set_menu {
        usage = "xmake dump-api [options]",
        description = "Dump the native scripting API of the nanos world server for the generator.",
        options = {
            {"s", "server", "kv", nil, "The nanos world server directory."}
        }
    }
task_end()

task("generate")
    set_category("plugin")
    on_run(function ()
        import("core.base.option")

        local root = os.projectdir()
        local definitions_dir = path.join(root, "build", "generator", "nanos-api")
        if not os.isdir(path.join(definitions_dir, ".git")) then
            os.execv("git", {"clone", "--quiet", api_definitions_repository, definitions_dir})
        end
        os.execv("git", {"-C", definitions_dir, "fetch", "--quiet", "origin"})
        os.execv("git", {"-C", definitions_dir, "checkout", "--quiet", "--detach", api_definitions_revision})

        local generator = path.join(root, "managed", "tools", "NanosSharp.Generator")
        os.execv("dotnet", {"run", "--project", generator, "-c", "Release", "--",
            "--native", path.join(generator, "Data", "native-api.json"),
            "--native-client", path.join(generator, "Data", "native-api-client.json"),
            "--docs", definitions_dir,
            "--supplements", path.join(generator, "Data", "Supplements"),
            "--output", path.join(root, "managed", "NanosSharp", "Generated"),
            "--report", path.join(root, "build", "generator", "report.md"),
            "--revision", api_definitions_revision})
    end)
    set_menu {
        usage = "xmake generate",
        description = "Generate the typed C# API from the native dump and the nanos world API definitions.",
        options = {}
    }
task_end()
