#include "game/lua.h"

#include <cstddef>
#include <cstring>
#include <exception>
#include <functional>
#include <memory>
#include <new>
#include <stdexcept>
#include <string>
#include <vector>

#include "game/lua_symbols.h"
#include "game/validation.h"

namespace nanossharp::game {

namespace {

constexpr std::ptrdiff_t kEngineLuaStateOffset = 0xa0;
constexpr std::ptrdiff_t kEnvironmentTableOffset = 0xc8;
constexpr int kEnvironmentUpvalue = 1;
constexpr int kTrampolineArguments = 2;
constexpr char kTrampolineSource[] = "local operation, context = ... return operation(context)";
constexpr char kTrampolineName[] = "=NanosSharp";
constexpr char kTrampolineMode[] = "t";
constexpr char kLuaFunctionTypeName[] = "N2nw9scripting3lua14LuaFunctionRefE";
constexpr char kFunctionMetatableName[] = "NanosSharp.Function";
constexpr char kGarbageCollectField[] = "__gc";

using FunctionHolder = std::shared_ptr<IScriptFunctionRef>;

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

LuaState* GetLuaState(Engine& engine) {
  auto* state =
      *reinterpret_cast<LuaState**>(reinterpret_cast<char*>(&engine) + kEngineLuaStateOffset);
  if (state == nullptr) {
    throw std::runtime_error("the Lua state is not available");
  }
  return state;
}

LuaRef& GetEnvironmentTable(Environment& environment) {
  if (!IsLuaEnvironment(&environment)) {
    throw std::runtime_error("the package environment is not a Lua environment");
  }
  return *reinterpret_cast<LuaRef*>(reinterpret_cast<char*>(&environment) +
                                    kEnvironmentTableOffset);
}

bool IsLuaFunction(const FunctionHolder& function) {
  const char* name = GetTypeName(function.get());
  return name != nullptr && std::strcmp(name, kLuaFunctionTypeName) == 0;
}

int CollectFunction(LuaState* state) {
  auto* holder = static_cast<FunctionHolder*>(GetLuaSymbols().touserdata(state, 1));
  holder->~FunctionHolder();
  return 0;
}

bool InvokeFunction(LuaState* state, std::string& error) {
  const LuaSymbols& lua = GetLuaSymbols();
  try {
    auto* holder = static_cast<FunctionHolder*>(lua.touserdata(state, LuaUpvalueIndex(1)));
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
  auto* context = static_cast<OperationContext*>(GetLuaSymbols().touserdata(state, 1));
  try {
    context->operation(state);
  } catch (...) {
    context->exception = std::current_exception();
  }
  return 0;
}

void RunInEnvironment(Engine& engine, Environment& environment,
                      std::function<void(LuaState*)> operation) {
  LuaState* state = GetLuaState(engine);
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
  lua.pushlightuserdata(state, &context);
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

ScriptValue GetLuaGlobal(Engine& engine, Environment& environment, const std::string& name) {
  ScriptValue value{std::monostate{}};
  RunInEnvironment(engine, environment, [&](LuaState* state) {
    LuaStackGuard guard(state);
    const LuaSymbols& lua = GetLuaSymbols();
    lua.getglobal(state, name.c_str());
    LuaPopVisited visited;
    lua.pop_value(state, -1, value, visited, false);
  });
  return value;
}

void SetLuaGlobal(Engine& engine, Environment& environment, const std::string& name,
                  const ScriptValue& value) {
  RunInEnvironment(engine, environment, [&](LuaState* state) {
    LuaStackGuard guard(state);
    const LuaSymbols& lua = GetLuaSymbols();
    LuaPushVisited visited;
    PushValueDetour(state, value, visited);
    lua.setglobal(state, name.c_str());
  });
}

}
