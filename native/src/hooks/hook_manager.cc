#include "hooks/hook_manager.h"

#include <funchook.h>

namespace nanossharp::hooks {

HookManager::HookManager() : funchook_(funchook_create()) {}

HookManager::~HookManager() {
  if (funchook_ != nullptr) {
    funchook_destroy(funchook_);
  }
}

bool HookManager::PrepareRaw(void** target, void* detour) {
  if (funchook_ == nullptr) {
    error_ = "funchook could not be created";
    return false;
  }
  if (funchook_prepare(funchook_, target, detour) != FUNCHOOK_ERROR_SUCCESS) {
    error_ = funchook_error_message(funchook_);
    return false;
  }
  return true;
}

bool HookManager::Install() {
  if (funchook_install(funchook_, 0) != FUNCHOOK_ERROR_SUCCESS) {
    error_ = funchook_error_message(funchook_);
    return false;
  }
  return true;
}

}
