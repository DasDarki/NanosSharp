#ifndef NANOSSHARP_PLATFORM_LINUX_SIGNATURE_H_
#define NANOSSHARP_PLATFORM_LINUX_SIGNATURE_H_

#include <cstddef>
#include <cstdint>

namespace nanossharp::platform::signature {

// Scans the executable segments of the main program for a byte pattern. The pattern is
// space separated hexadecimal bytes with `??` wildcards. FindUnique returns the single
// match, or nullptr when the pattern is absent or occurs more than once. FindFirst returns
// the lowest-address match, for patterns that identify several identical COMDAT copies.
uint8_t* FindUnique(const char* pattern, size_t& length);
uint8_t* FindFirst(const char* pattern, size_t& length);

}

#endif
