#ifndef NANOSSHARP_BRIDGE_MANAGED_FUNCTION_REF_H_
#define NANOSSHARP_BRIDGE_MANAGED_FUNCTION_REF_H_

#include <cstdint>

#include "game/scripting.h"

namespace nanossharp::bridge {

class ManagedFunctionRef final : public game::IScriptFunctionRef {
 public:
  ManagedFunctionRef(int64_t function_id, game::Environment* environment);
  ~ManagedFunctionRef() override;

  game::ScriptValue Call(const std::vector<game::ScriptValue>& args) override;
  void* GetRawPtr() const override;
  void Invalidate() override;

 private:
  game::Environment* environment_;
  int64_t function_id_;
  bool valid_ = true;
};

}

#endif
