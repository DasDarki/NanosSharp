#ifndef NANOSSHARP_PLATFORM_WINDOWS_SIGNATURE_H_
#define NANOSSHARP_PLATFORM_WINDOWS_SIGNATURE_H_

#include <windows.h>

#include <cstddef>
#include <cstdint>

namespace nanossharp::platform::signature {

uint8_t* FindUnique(HMODULE module, const char* pattern, size_t& length);
uint8_t* CallTarget(uint8_t* call);

}

#endif
