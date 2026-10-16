#include "host/dotnet_host.h"

#include <coreclr_delegates.h>
#include <hostfxr.h>

#include <array>
#include <cstdio>
#include <utility>

#include "platform/platform.h"

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
  return reinterpret_cast<T>(platform::FindExport(library, name));
}

std::basic_string<char_t> ToHostString(const std::string& value) {
  return std::filesystem::path(value).native();
}

}

DotnetHost::DotnetHost(std::filesystem::path runtime_directory)
    : runtime_directory_(std::move(runtime_directory)) {}

bool DotnetHost::Start(std::string& error) {
  platform::HostfxrLocation location;
  if (!platform::LocateHostfxr(runtime_directory_, location, error)) {
    return false;
  }

  std::string load_error;
  void* hostfxr = platform::OpenLibrary(location.library, load_error);
  if (hostfxr == nullptr) {
    error = "unable to load hostfxr: " + load_error;
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
  hostfxr_initialize_parameters parameters{sizeof(hostfxr_initialize_parameters), nullptr,
                                           location.dotnet_root.c_str()};
  hostfxr_handle context = nullptr;
  int result = initialize(config_path.c_str(), location.dotnet_root.empty() ? nullptr : &parameters,
                          &context);
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
  int result = load(assembly_path.c_str(), ToHostString(qualified_type).c_str(),
                    ToHostString(method_name).c_str(), UNMANAGEDCALLERSONLY_METHOD, nullptr,
                    &entry_point);
  if (result != 0 || entry_point == nullptr) {
    error = "unable to load " + qualified_type + "::" + method_name + " from " +
            assembly_path.string() + " (" + HexCode(result) + ")";
    return nullptr;
  }
  return entry_point;
}

}
