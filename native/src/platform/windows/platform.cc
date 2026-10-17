#include "platform/platform.h"

#include <windows.h>

#include <algorithm>
#include <cstdlib>
#include <cstring>
#include <stdexcept>
#include <vector>

#include "platform/windows/rtti.h"
#include "platform/windows/runtime.h"

namespace nanossharp::platform {

namespace {

constexpr wchar_t kDotnetDirectoryName[] = L"dotnet";
constexpr wchar_t kHostfxrFileName[] = L"hostfxr.dll";
constexpr char kUnsupportedFeature[] = "this feature is not supported on Windows yet";

char* CopyString(const std::string& value) {
  char* copy = static_cast<char*>(std::malloc(value.size() + 1));
  std::memcpy(copy, value.c_str(), value.size() + 1);
  return copy;
}

std::string LastErrorMessage() {
  return "Windows error " + std::to_string(GetLastError());
}

}

bool LocateHostfxr(const std::filesystem::path&, HostfxrLocation& location, std::string& error) {
  std::filesystem::path dotnet_root = Runtime::Instance().base_directory() / kDotnetDirectoryName;
  std::filesystem::path fxr_directory = dotnet_root / L"host" / L"fxr";
  std::error_code code;
  std::vector<std::filesystem::path> versions;
  for (const auto& entry : std::filesystem::directory_iterator(fxr_directory, code)) {
    if (std::filesystem::exists(entry.path() / kHostfxrFileName)) {
      versions.push_back(entry.path());
    }
  }
  if (versions.empty()) {
    error = "no .NET runtime found in " + fxr_directory.string();
    return false;
  }
  std::sort(versions.begin(), versions.end());
  location.library = versions.back() / kHostfxrFileName;
  location.dotnet_root = dotnet_root;
  return true;
}

void* OpenLibrary(const std::filesystem::path& path, std::string& error) {
  HMODULE library = LoadLibraryW(path.c_str());
  if (library == nullptr) {
    error = LastErrorMessage();
  }
  return library;
}

void* FindExport(void* library, const char* name) {
  return reinterpret_cast<void*>(GetProcAddress(static_cast<HMODULE>(library), name));
}

game::Engine* AttachedEngine() { return Runtime::Instance().engine(); }

void Log(game::LogType type, const std::string& message) { Runtime::Instance().Log(type, message); }

char* DescribeType(const void* polymorphic_object) {
  return CopyString(rtti::ReadableTypeName(polymorphic_object));
}

void* LoadNativeLibrary(const char* path, std::string& error) {
  return OpenLibrary(std::filesystem::path(reinterpret_cast<const char8_t*>(path)), error);
}

bool ResolveDeferredEntity(uint32_t, game::ScriptValue&) { return false; }

game::ScriptValue GetLuaGlobal(game::Environment&, const std::string&) {
  throw std::runtime_error(kUnsupportedFeature);
}

void SetLuaGlobal(game::Environment&, const std::string&, const game::ScriptValue&) {
  throw std::runtime_error(kUnsupportedFeature);
}

void ReloadPackage(game::Package*) { throw std::runtime_error(kUnsupportedFeature); }

void* GetPackageEntity(game::Package*) { return nullptr; }

}
