#ifndef NANOSSHARP_PLATFORM_WINDOWS_INLINE_HOOK_H_
#define NANOSSHARP_PLATFORM_WINDOWS_INLINE_HOOK_H_

#include <cstddef>

namespace nanossharp::platform::inline_hook {

void* Install(void* target, void* detour, size_t relocatable_prologue);

}

#endif
