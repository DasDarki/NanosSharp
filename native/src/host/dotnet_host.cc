#include "host/dotnet_host.h"

#include <coreclr_delegates.h>
#include <dlfcn.h>
#include <hostfxr.h>
#include <nethost.h>

#include <array>
#include <cstdio>
#include <utility>

namespace nanossharp::host {

namespace {

constexpr char kHostAssemblyName[] = "NanosSharp.Host";

std::string HexCode(int32_t code) {
  std::array<char, 16> buffer{};
  std::snprintf(buffer.data(), buffer.size(), "0x%x", static_cast<uint32_t>(code));
  return buffer.data();
}

template <typename T>
T LoadExport(void* library, const char* name) {
  return reinterpret_cast<T>(dlsym(library, name));
}

}

DotnetHost::DotnetHost(std::filesystem::path runtime_directory)
    : runtime_directory_(std::move(runtime_directory)) {}

bool DotnetHost::Start(std::string& error) {
  std::array<char_t, 4096> hostfxr_path{};
  size_t hostfxr_path_size = hostfxr_path.size();
  int result = get_hostfxr_path(hostfxr_path.data(), &hostfxr_path_size, nullptr);
  if (result != 0) {
    error = "unable to locate hostfxr (" + HexCode(result) +
            "), is the .NET runtime installed?";
    return false;
  }

  void* hostfxr = dlopen(hostfxr_path.data(), RTLD_NOW | RTLD_LOCAL);
  if (hostfxr == nullptr) {
    error = std::string("unable to load hostfxr: ") + dlerror();
    return false;
  }

  auto initialize = LoadExport<hostfxr_initialize_for_runtime_config_fn>(
      hostfxr, "hostfxr_initialize_for_runtime_config");
  auto get_delegate = LoadExport<hostfxr_get_runtime_delegate_fn>(
      hostfxr, "hostfxr_get_runtime_delegate");
  auto close = LoadExport<hostfxr_close_fn>(hostfxr, "hostfxr_close");
  if (initialize == nullptr || get_delegate == nullptr || close == nullptr) {
    error = "hostfxr exports are missing";
    return false;
  }

  std::filesystem::path config_path =
      runtime_directory_ / (std::string(kHostAssemblyName) + ".runtimeconfig.json");
  hostfxr_handle context = nullptr;
  result = initialize(config_path.c_str(), nullptr, &context);
  if (result != 0 || context == nullptr) {
    error = "unable to initialize the .NET runtime with " + config_path.string() +
            " (" + HexCode(result) + ")";
    if (context != nullptr) {
      close(context);
    }
    return false;
  }

  result = get_delegate(context, hdt_load_assembly_and_get_function_pointer,
                        &load_assembly_and_get_function_pointer_);
  close(context);
  if (result != 0 || load_assembly_and_get_function_pointer_ == nullptr) {
    error = "unable to get the .NET runtime delegate (" + HexCode(result) + ")";
    return false;
  }
  return true;
}

void* DotnetHost::GetEntryPoint(const char* type_name, const char* method_name,
                                std::string& error) const {
  auto load = reinterpret_cast<load_assembly_and_get_function_pointer_fn>(
      load_assembly_and_get_function_pointer_);
  std::filesystem::path assembly_path =
      runtime_directory_ / (std::string(kHostAssemblyName) + ".dll");
  std::string qualified_type = std::string(type_name) + ", " + kHostAssemblyName;

  void* entry_point = nullptr;
  int result = load(assembly_path.c_str(), qualified_type.c_str(), method_name,
                    UNMANAGEDCALLERSONLY_METHOD, nullptr, &entry_point);
  if (result != 0 || entry_point == nullptr) {
    error = "unable to load " + qualified_type + "::" + method_name + " from " +
            assembly_path.string() + " (" + HexCode(result) + ")";
    return nullptr;
  }
  return entry_point;
}

}
