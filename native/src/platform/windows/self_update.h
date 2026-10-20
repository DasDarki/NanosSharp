#ifndef NANOSSHARP_PLATFORM_WINDOWS_SELF_UPDATE_H_
#define NANOSSHARP_PLATFORM_WINDOWS_SELF_UPDATE_H_


#include <filesystem>
#include <functional>
#include <string>

namespace nanossharp::platform::self_update {

using LogSink = std::function<void(const std::string&)>;

bool Apply(const std::filesystem::path& nanossharp_directory, const std::wstring& module_path, const LogSink& log);

}

#endif
