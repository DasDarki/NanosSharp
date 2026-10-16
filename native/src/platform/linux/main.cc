#include "platform/linux/runtime.h"

__attribute__((constructor)) static void OnLibraryLoad() {
  nanossharp::Runtime::Instance().Load();
}
