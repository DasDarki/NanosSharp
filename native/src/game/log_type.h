#ifndef NANOSSHARP_GAME_LOG_TYPE_H_
#define NANOSSHARP_GAME_LOG_TYPE_H_

#include <cstdint>

namespace nanossharp::game {

enum class LogType : int32_t {
  kInfo = 0,
  kWarning = 1,
  kError = 2,
  kDebug = 3,
  kVerbose = 4,
  kScript = 5,
  kScriptWarning = 6,
  kScriptError = 7,
  kScriptDebug = 8,
  kChat = 9,
  kWebUI = 10,
  kSuccess = 11,
  kFatal = 12,
};

}

#endif
