#ifndef NANOSSHARP_PLATFORM_WINDOWS_LUA_SYMBOLS_H_
#define NANOSSHARP_PLATFORM_WINDOWS_LUA_SYMBOLS_H_

#include <windows.h>

#include <cstddef>
#include <string>

namespace nanossharp::platform::lua {

bool Resolve(HMODULE module, std::string& error);
bool IsResolved(std::string& reason);
std::ptrdiff_t EnvironmentTableOffset();

}

#endif
