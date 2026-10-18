#include "bridge/lua_interop.h"

#include <cstddef>
#include <cstring>
#include <exception>
#include <functional>
#include <memory>
#include <new>
#include <stdexcept>
#include <string>
#include <vector>

#include "platform/platform.h"

namespace nanossharp::game {

LuaRef::~LuaRef() { GetLuaSymbols().ref_destroy(this); }

LuaSymbols& GetLuaSymbols() {
  static LuaSymbols symbols;
  return symbols;
}

}

namespace nanossharp::bridge {

namespace {

constexpr int kEnvironmentUpvalue = 1;
constexpr int kTrampolineArguments = 2;
constexpr char kTrampolineSource[] = "local operation, context = ... return operation(context)";
constexpr char kTrampolineName[] = "=NanosSharp";
constexpr char kTrampolineMode[] = "t";
constexpr char kFunctionMetatableName[] = "NanosSharp.Function";
constexpr char kGarbageCollectField[] = "__gc";

using game::GetLuaSymbols;
using game::LuaPopVisited;
using game::LuaPushVisited;
using game::LuaRef;
using game::LuaState;
using game::LuaSymbols;
using game::ScriptValue;
using FunctionHolder = std::shared_ptr<game::IScriptFunctionRef>;

class LuaStackGuard {
 public:
  explicit LuaStackGuard(LuaState* state) : state_(state), top_(GetLuaSymbols().gettop(state)) {}
  ~LuaStackGuard() { GetLuaSymbols().settop(state_, top_); }

  LuaStackGuard(const LuaStackGuard&) = delete;
  LuaStackGuard& operator=(const LuaStackGuard&) = delete;

 private:
  LuaState* state_;
  int top_;
};

struct OperationContext {
  std::function<void(LuaState*)> operation;
  std::exception_ptr exception;
};

LuaRef& GetEnvironmentTable(game::Environment& environment) {
  LuaRef* table = platform::GetLuaEnvironmentTable(environment);
  if (table == nullptr) {
    throw std::runtime_error("the package environment is not a Lua environment");
  }
  return *table;
}

bool IsLuaFunction(const FunctionHolder& function) { return platform::IsLuaFunction(function.get()); }

int CollectFunction(LuaState* state) {
  auto* holder = static_cast<FunctionHolder*>(GetLuaSymbols().touserdata(state, 1));
  holder->~FunctionHolder();
  return 0;
}

bool InvokeFunction(LuaState* state, std::string& error) {
  const LuaSymbols& lua = GetLuaSymbols();
  try {
    auto* holder = static_cast<FunctionHolder*>(lua.touserdata(state, game::LuaUpvalueIndex(1)));
    int count = lua.gettop(state);
    std::vector<ScriptValue> arguments(count, ScriptValue{std::monostate{}});
    LuaPopVisited popped;
    for (int index = 0; index < count; ++index) {
      lua.pop_value(state, index + 1, arguments[index], popped, false);
    }
    ScriptValue result = (*holder)->Call(arguments);
    LuaPushVisited pushed;
    PushValueDetour(state, result, pushed);
    return true;
  } catch (const std::exception& exception) {
    error = exception.what();
  } catch (...) {
    error = "unknown native exception";
  }
  return false;
}

int CallFunction(LuaState* state) {
  std::string error;
  if (InvokeFunction(state, error)) {
    return 1;
  }
  const LuaSymbols& lua = GetLuaSymbols();
  lua.pushlstring(state, error.data(), error.size());
  error = std::string();
  return lua.error(state);
}

void PushFunction(LuaState* state, const FunctionHolder& function) {
  const LuaSymbols& lua = GetLuaSymbols();
  void* memory = lua.newuserdatauv(state, sizeof(FunctionHolder), 0);
  new (memory) FunctionHolder(function);
  if (lua.newmetatable(state, kFunctionMetatableName) != 0) {
    lua.pushcclosure(state, &CollectFunction, 0);
    lua.setfield(state, -2, kGarbageCollectField);
  }
  lua.setmetatable(state, -2);
  lua.pushcclosure(state, &CallFunction, 1);
}

int RunOperation(LuaState* state) {
  auto* context = *static_cast<OperationContext**>(GetLuaSymbols().touserdata(state, 1));
  try {
    context->operation(state);
  } catch (...) {
    context->exception = std::current_exception();
  }
  return 0;
}

void RunInEnvironment(game::Environment& environment, std::function<void(LuaState*)> operation) {
  std::string reason;
  if (!platform::IsLuaInteropAvailable(reason)) {
    throw std::runtime_error("Lua interop is not available: " + reason);
  }
  LuaState* state = platform::GetLuaState();
  LuaRef& table = GetEnvironmentTable(environment);
  LuaStackGuard guard(state);
  const LuaSymbols& lua = GetLuaSymbols();
  if (lua.loadbufferx(state, kTrampolineSource, sizeof(kTrampolineSource) - 1, kTrampolineName,
                      kTrampolineMode) != 0) {
    throw std::runtime_error(lua.tolstring(state, -1, nullptr));
  }
  lua.ref_push(&table, state);
  lua.setupvalue(state, -2, kEnvironmentUpvalue);

  OperationContext context{std::move(operation), nullptr};
  lua.pushcclosure(state, &RunOperation, 0);
  *static_cast<OperationContext**>(lua.newuserdatauv(state, sizeof(OperationContext*), 0)) = &context;
  if (lua.pcallk(state, kTrampolineArguments, 0, 0, 0, nullptr) != 0) {
    throw std::runtime_error(lua.tolstring(state, -1, nullptr));
  }
  if (context.exception) {
    std::rethrow_exception(context.exception);
  }
}

}

void PushValueDetour(LuaState* state, const ScriptValue& value, LuaPushVisited& visited) {
  if (const auto* function = std::get_if<FunctionHolder>(&value.value)) {
    if (*function != nullptr && !IsLuaFunction(*function)) {
      PushFunction(state, *function);
      return;
    }
  }
  GetLuaSymbols().push_value(state, value, visited);
}

ScriptValue GetLuaGlobal(game::Environment& environment, const std::string& name) {
  ScriptValue value{std::monostate{}};
  RunInEnvironment(environment, [&](LuaState* state) {
    LuaStackGuard guard(state);
    const LuaSymbols& lua = GetLuaSymbols();
    lua.getglobal(state, name.c_str());
    LuaPopVisited visited;
    lua.pop_value(state, -1, value, visited, false);
  });
  return value;
}

void SetLuaGlobal(game::Environment& environment, const std::string& name, const ScriptValue& value) {
  RunInEnvironment(environment, [&](LuaState* state) {
    LuaStackGuard guard(state);
    const LuaSymbols& lua = GetLuaSymbols();
    LuaPushVisited visited;
    PushValueDetour(state, value, visited);
    lua.setglobal(state, name.c_str());
  });
}

}
