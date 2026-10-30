#ifndef NANOSSHARP_PLATFORM_WINDOWS_FILE_TRANSFER_PATCH_H_
#define NANOSSHARP_PLATFORM_WINDOWS_FILE_TRANSFER_PATCH_H_

#include <windows.h>

#include <cstdint>
#include <string>

namespace nanossharp::platform::file_transfer {

// Lowers the client's file-transfer size threshold so files of at least
// `threshold_kib` KiB become eligible for HTTP delivery (a CDN) instead of the
// game socket. The stock client clamps this threshold to 8192 KiB, the sentinel
// value that disables HTTP file transfer entirely; patching the clamp immediate
// re-enables the HTTP path. Returns true and reports the patched location in
// `detail` on success, or false with a reason when the clamp cannot be located
// uniquely or the value is out of range. Windows client only.
bool PatchSizeThreshold(HMODULE game, uint32_t threshold_kib, std::string& detail);

}

#endif
