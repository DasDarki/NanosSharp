#ifndef NANOSSHARP_BRIDGE_NATIVE_ARGUMENT_H_
#define NANOSSHARP_BRIDGE_NATIVE_ARGUMENT_H_

#include <cstdint>

#include "game/script_value.h"

namespace nanossharp::bridge {

inline constexpr uint8_t kHandleArgument = 0xff;
inline constexpr uint8_t kBorrowedHandleArgument = 0xfe;

struct NativeArgument {
  uint8_t type;
  uint8_t padding[7];
  union {
    uint8_t boolean;
    int64_t integer;
    double number;
    struct {
      const char* data;
      int64_t length;
    } string;
    double components[3];
    float color[4];
    struct {
      uint64_t class_id;
      void* instance;
      uint32_t network_id;
    } entity;
    game::ScriptValue* handle;
  };
};

static_assert(sizeof(NativeArgument) == 32);

game::ScriptValue FromArgument(const NativeArgument& argument);
void ToArgument(game::ScriptValue&& value, NativeArgument& argument);
void ToArgumentView(const game::ScriptValue& value, NativeArgument& argument);

}

#endif
