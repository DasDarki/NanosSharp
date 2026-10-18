#include "platform/windows/inline_hook.h"

#include <windows.h>

#include <cstdint>
#include <cstring>

namespace nanossharp::platform::inline_hook {

namespace {

constexpr uint8_t kAbsoluteJump[] = {0xFF, 0x25, 0x00, 0x00, 0x00, 0x00};
constexpr size_t kAbsoluteJumpLength = sizeof(kAbsoluteJump) + sizeof(void*);
constexpr uint8_t kBreakpoint = 0xCC;

void WriteAbsoluteJump(uint8_t* at, const void* destination) {
  std::memcpy(at, kAbsoluteJump, sizeof(kAbsoluteJump));
  std::memcpy(at + sizeof(kAbsoluteJump), &destination, sizeof(destination));
}

}

void* Install(void* target, void* detour, size_t relocatable_prologue) {
  if (relocatable_prologue < kAbsoluteJumpLength) {
    return nullptr;
  }
  auto* trampoline = static_cast<uint8_t*>(VirtualAlloc(nullptr, relocatable_prologue + kAbsoluteJumpLength,
                                                        MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE));
  if (trampoline == nullptr) {
    return nullptr;
  }
  auto* code = static_cast<uint8_t*>(target);
  std::memcpy(trampoline, code, relocatable_prologue);
  WriteAbsoluteJump(trampoline + relocatable_prologue, code + relocatable_prologue);

  DWORD protection = 0;
  if (!VirtualProtect(code, relocatable_prologue, PAGE_EXECUTE_READWRITE, &protection)) {
    VirtualFree(trampoline, 0, MEM_RELEASE);
    return nullptr;
  }
  WriteAbsoluteJump(code, detour);
  std::memset(code + kAbsoluteJumpLength, kBreakpoint, relocatable_prologue - kAbsoluteJumpLength);
  VirtualProtect(code, relocatable_prologue, protection, &protection);
  FlushInstructionCache(GetCurrentProcess(), code, relocatable_prologue);
  FlushInstructionCache(GetCurrentProcess(), trampoline, relocatable_prologue + kAbsoluteJumpLength);
  return trampoline;
}

}
