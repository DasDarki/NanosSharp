#include <windows.h>

#include <string>

#include "platform/windows/runtime.h"

extern "C" {
#define X(name) void* real_##name = nullptr;
#include "platform/windows/version_exports.inc"
#undef X
}

namespace {

constexpr wchar_t kSystemLibraryName[] = L"\\version.dll";

void LoadSystemVersionLibrary() {
  std::wstring path(MAX_PATH, L'\0');
  path.resize(GetSystemDirectoryW(path.data(), MAX_PATH));
  HMODULE library = LoadLibraryW((path + kSystemLibraryName).c_str());
  if (library == nullptr) {
    return;
  }
#define X(name) real_##name = reinterpret_cast<void*>(GetProcAddress(library, #name));
#include "platform/windows/version_exports.inc"
#undef X
}

}

BOOL WINAPI DllMain(HINSTANCE instance, DWORD reason, LPVOID) {
  if (reason == DLL_PROCESS_ATTACH) {
    DisableThreadLibraryCalls(instance);
    LoadSystemVersionLibrary();
    nanossharp::Runtime::Instance().Load(instance);
  }
  if (reason == DLL_PROCESS_DETACH) {
    nanossharp::Runtime::Instance().Unload();
  }
  return TRUE;
}
