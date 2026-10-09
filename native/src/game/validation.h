#ifndef NANOSSHARP_GAME_VALIDATION_H_
#define NANOSSHARP_GAME_VALIDATION_H_

#include <string>

#include "game/package.h"
#include "game/scripting.h"

namespace nanossharp::game {

const char* GetTypeName(const void* polymorphic_object);
bool ValidateEngine(const Engine* engine, std::string& error);
bool ValidatePackage(const Package* package, std::string& error);
bool IsLuaEnvironment(const Environment* environment);

}

#endif
