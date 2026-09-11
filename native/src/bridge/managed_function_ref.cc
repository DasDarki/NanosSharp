#include "bridge/managed_function_ref.h"

#include <memory>

#include "bridge/native_api.h"

namespace nanossharp::bridge {

ManagedFunctionRef::ManagedFunctionRef(int64_t function_id,
                                       game::Environment* environment)
    : environment_(environment), function_id_(function_id) {}

ManagedFunctionRef::~ManagedFunctionRef() {
  if (auto release = GetManagedCallbacks().release_function) {
    release(function_id_);
  }
}

game::ScriptValue ManagedFunctionRef::Call(
    const std::vector<game::ScriptValue>& args) {
  auto invoke = GetManagedCallbacks().invoke_function;
  if (!valid_ || invoke == nullptr) {
    return game::ScriptValue{std::monostate{}};
  }
  std::unique_ptr<game::ScriptValue> result(
      invoke(function_id_, args.data(), static_cast<int32_t>(args.size())));
  if (result == nullptr) {
    return game::ScriptValue{std::monostate{}};
  }
  return std::move(*result);
}

void* ManagedFunctionRef::GetRawPtr() const {
  return const_cast<ManagedFunctionRef*>(this);
}

void ManagedFunctionRef::Invalidate() { valid_ = false; }

}
