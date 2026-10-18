#ifndef NANOSSHARP_PLATFORM_WINDOWS_RTTI_H_
#define NANOSSHARP_PLATFORM_WINDOWS_RTTI_H_

#include <windows.h>

#include <cstdint>
#include <string>

namespace nanossharp::platform::rtti {

void** FindVtable(HMODULE module, const char* decorated_name);
bool FindBaseOffset(HMODULE module, const char* decorated_name, uint32_t& offset);
const char* DecoratedTypeName(const void* polymorphic_object);
std::string ReadableTypeName(const void* polymorphic_object);
bool HasTypeName(const void* polymorphic_object, const char* decorated_name);
bool HasTypePrefix(const void* polymorphic_object, const char* decorated_prefix);

}

#endif
