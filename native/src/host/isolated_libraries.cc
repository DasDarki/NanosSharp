#include "host/isolated_libraries.h"

#include <dlfcn.h>

#include <array>

namespace nanossharp::host {

namespace {

constexpr int kIsolatedFlags = RTLD_NOW | RTLD_LOCAL | RTLD_DEEPBIND;

constexpr std::array<std::array<const char*, 2>, 2> kOpenSslLibraries{{
    {"libcrypto.so.3", "libssl.so.3"},
    {"libcrypto.so.1.1", "libssl.so.1.1"},
}};

}

void PreloadIsolatedSystemLibraries() {
  for (const auto& libraries : kOpenSslLibraries) {
    void* crypto = dlopen(libraries[0], kIsolatedFlags);
    if (crypto == nullptr) {
      continue;
    }
    dlopen(libraries[1], kIsolatedFlags);
    return;
  }
}

void* LoadIsolatedLibrary(const char* path, std::string& error) {
  void* handle = dlopen(path, kIsolatedFlags);
  if (handle == nullptr) {
    const char* message = dlerror();
    error = message != nullptr ? message : "unknown dlopen error";
  }
  return handle;
}

}
