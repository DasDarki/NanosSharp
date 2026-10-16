#ifndef NANOSSHARP_PLATFORM_PLATFORM_H_
#define NANOSSHARP_PLATFORM_PLATFORM_H_

#include <cstdint>
#include <filesystem>
#include <string>

#include "game/log_type.h"
#include "game/scripting.h"

namespace nanossharp::game {

class Package;

}

namespace nanossharp::platform {

struct HostfxrLocation {
  std::filesystem::path library;
  std::filesystem::path dotnet_root;
};

bool LocateHostfxr(const std::filesystem::path& runtime_directory, HostfxrLocation& location,
                   std::string& error);
void* OpenLibrary(const std::filesystem::path& path, std::string& error);
void* FindExport(void* library, const char* name);

game::Engine* AttachedEngine();
void Log(game::LogType type, const std::string& message);
char* DescribeType(const void* polymorphic_object);
void* LoadNativeLibrary(const char* path, std::string& error);
bool ResolveDeferredEntity(uint32_t network_id, game::ScriptValue& value);

game::ScriptValue GetLuaGlobal(game::Environment& environment, const std::string& name);
void SetLuaGlobal(game::Environment& environment, const std::string& name,
                  const game::ScriptValue& value);
void ReloadPackage(game::Package* package);
void* GetPackageEntity(game::Package* package);

}

#endif
