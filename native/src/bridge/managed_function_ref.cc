#include "bridge/managed_function_ref.h"

#include <array>
#include <memory>
#include <vector>

#include "bridge/native_api.h"

namespace nanossharp::bridge {

namespace {

constexpr size_t kStackArgumentCount = 8;

}

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
  std::array<NativeArgument, kStackArgumentCount> stack_views;
  std::vector<NativeArgument> heap_views;
  NativeArgument* views = stack_views.data();
  if (args.size() > stack_views.size()) {
    heap_views.resize(args.size());
    views = heap_views.data();
  }
  for (size_t i = 0; i < args.size(); ++i) {
    ToArgumentView(args[i], views[i]);
  }
  std::unique_ptr<game::ScriptValue> result(
      invoke(function_id_, views, static_cast<int32_t>(args.size())));
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
