#ifndef NANOSSHARP_HOOKS_HOOK_MANAGER_H_
#define NANOSSHARP_HOOKS_HOOK_MANAGER_H_

#include <string>

typedef struct funchook funchook_t;

namespace nanossharp::hooks {

class HookManager {
 public:
  HookManager();
  ~HookManager();

  HookManager(const HookManager&) = delete;
  HookManager& operator=(const HookManager&) = delete;

  template <typename T>
  bool Prepare(T& target, T detour) {
    void* function = reinterpret_cast<void*>(target);
    if (!PrepareRaw(&function, reinterpret_cast<void*>(detour))) {
      return false;
    }
    target = reinterpret_cast<T>(function);
    return true;
  }

  bool Install();
  const std::string& error() const { return error_; }

 private:
  bool PrepareRaw(void** target, void* detour);

  funchook_t* funchook_;
  std::string error_;
};

}

#endif
