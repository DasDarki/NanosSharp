#include "platform/windows/file_transfer_patch.h"

#include <cstdio>

#include "platform/windows/signature.h"

namespace nanossharp::platform::file_transfer {

namespace {

// The per-file backend decision caps the configured threshold and shifts it left
// by ten to form CoreClient::MaxSocketFileSize. The clamp reads the threshold,
// takes the minimum with 8192 (8 MiB once shifted), keeps it when positive and
// stores it back:
//   mov ecx,0x2000 ; cmp [rdx+0x5c],ecx ; cmovl ecx,[rdx+0x5c] ;
//   test ecx,ecx ; cmovg esi,ecx ; mov [rdx+0x5c],esi
// The leading 0x2000 immediate is the KiB cap; lowering it re-enables HTTP.
constexpr char kSizeClampPattern[] = "B9 00 20 00 00 39 4A 5C 0F 4C 4A 5C 85 C9 0F 4F F1 89 72 5C";
constexpr size_t kImmediateOffset = 1;
constexpr uint32_t kDisableThresholdKib = 8192;

}

bool PatchSizeThreshold(HMODULE game, uint32_t threshold_kib, std::string& detail) {
  if (threshold_kib == 0 || threshold_kib >= kDisableThresholdKib) {
    detail = "threshold must be between 1 and " + std::to_string(kDisableThresholdKib - 1) + " KiB";
    return false;
  }

  size_t length = 0;
  uint8_t* match = signature::FindUnique(game, kSizeClampPattern, length);
  if (match == nullptr) {
    detail = "the size threshold clamp was not found; the client build is unsupported";
    return false;
  }

  uint8_t* immediate = match + kImmediateOffset;
  DWORD protection = 0;
  if (!VirtualProtect(immediate, sizeof(uint32_t), PAGE_EXECUTE_READWRITE, &protection)) {
    detail = "VirtualProtect failed";
    return false;
  }
  *reinterpret_cast<uint32_t*>(immediate) = threshold_kib;
  VirtualProtect(immediate, sizeof(uint32_t), protection, &protection);
  FlushInstructionCache(GetCurrentProcess(), immediate, sizeof(uint32_t));

  char text[96];
  std::snprintf(text, sizeof(text), "%u KiB threshold at %s+0x%llx", threshold_kib,
                "client", static_cast<unsigned long long>(immediate - reinterpret_cast<uint8_t*>(game)));
  detail = text;
  return true;
}

}
