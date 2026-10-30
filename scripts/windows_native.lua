function _header_dir(dotnet_root)
    local dirs = os.dirs(path.join(dotnet_root, "packs", "Microsoft.NETCore.App.Host.*", "*", "runtimes", "*", "native"))
    local header_dir = nil
    for _, dir in ipairs(dirs) do
        if os.isfile(path.join(dir, "hostfxr.h")) then
            header_dir = dir
        end
    end
    return header_dir
end

function main(root, output_dir, object_dir, dotnet_root, xwin_override)
    local header_dir = _header_dir(dotnet_root)
    if not header_dir then
        raise("hostfxr.h was not found in the .NET SDK packs of %s", dotnet_root)
    end

    local system_flags = {}
    local library_flags = {}
    if os.host() ~= "windows" then
        local xwin = xwin_override or path.join(os.getenv("HOME"), ".xwin")
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

    os.mkdir(output_dir)
    local dll = path.join(output_dir, "version.dll")
    local link_arguments = {"/nologo", "/DLL", "/DEF:" .. path.join(root, "native", "src", "platform", "windows", "version.def"),
        "/OUT:" .. dll, "/IMPLIB:" .. path.join(object_dir, "version.lib"), "/MAP:" .. path.join(object_dir, "version.map"), "dbghelp.lib"}
    table.join2(link_arguments, library_flags)
    table.join2(link_arguments, objects)
    os.execv("lld-link", link_arguments)
    return dll
end
