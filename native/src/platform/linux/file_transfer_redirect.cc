#include "platform/linux/file_transfer_redirect.h"

#include <cstdint>
#include <cstdio>
#include <string>

#include "platform/linux/hook_manager.h"
#include "platform/linux/signature.h"

namespace nanossharp::platform::file_transfer {

namespace {

// The dedicated server's internal `/file` HTTP handler and the libstdc++ std::string /
// header helpers it calls. The response keeps its header map at response+0x18 and its
// status as a uint32 at response+0x8; the request's parameter vector spans
// request+0x18..+0x20 with 16-byte entries holding a C string key at +0 and value at +8.
constexpr char kHandlerPattern[] =
    "41 57 66 0F EF C0 41 56 49 89 FE 41 55 41 54 55 53 48 81 EC ?? ?? ?? ?? 48 89 74 24 10";
constexpr char kSetHeaderPattern[] =
    "F3 0F 1E FA 48 8B 06 4C 8B 02 48 8B 48 E8 48 85 C9 0F 84 ?? ?? ?? ??";
constexpr char kStringConstructorPattern[] =
    "F3 0F 1E FA 41 55 41 54 55 53 48 83 EC 08 48 85 F6 0F 84 ?? ?? ?? ?? 49 89 FC 48 89 F7 48 89 F3";
constexpr char kStringDestructorPattern[] =
    "F3 0F 1E FA 48 8B 07 48 8D 15 ?? ?? ?? ?? 48 8D 78 E8 48 39 D7 75 09 C3 0F 1F 84 00 00 00 00 00 "
    "80 3D ?? ?? ?? ?? 00 74 ?? 8B 50 F8 8D 4A FF 89 48 F8 85 D2 7F ?? 48 8B 70 F0";

constexpr size_t kRequestParametersBeginOffset = 0x18;
constexpr size_t kRequestParametersEndOffset = 0x20;
constexpr size_t kParameterStride = 16;
constexpr size_t kParameterValueOffset = 8;
constexpr size_t kResponseHeadersOffset = 0x18;
constexpr size_t kResponseStatusOffset = 0x8;
constexpr int kRedirectStatus = 302;

using HandlerFunction = void (*)(const uint8_t* request, uint8_t* response);
using StringConstructor = void (*)(void* self, const char* text, const void* allocator);
using StringDestructor = void (*)(void* self);
using HeaderSetter = void (*)(void* headers, void* key, void* value);

struct Redirect {
  std::string cdn_url;
  StringConstructor construct_string = nullptr;
  StringDestructor destroy_string = nullptr;
  HeaderSetter set_header = nullptr;
  bool installed = false;
};

Redirect g_redirect;

struct alignas(8) StringStorage {
  uint8_t bytes[32] = {};

  void* Pointer() {
    return bytes;
  }
};

std::string PercentEncode(const std::string& text) {
  static const char kHex[] = "0123456789ABCDEF";
  std::string encoded;
  encoded.reserve(text.size());
  for (unsigned char character : text) {
    bool safe = (character >= '0' && character <= '9') || (character >= 'A' && character <= 'Z') ||
                (character >= 'a' && character <= 'z') || character == '-' || character == '_' ||
                character == '.' || character == '~' || character == '/';
    if (safe) {
      encoded.push_back(static_cast<char>(character));
      continue;
    }
    encoded.push_back('%');
    encoded.push_back(kHex[character >> 4]);
    encoded.push_back(kHex[character & 0x0F]);
  }
  return encoded;
}

std::string BuildQuery(const uint8_t* request) {
  auto begin = *reinterpret_cast<const uintptr_t*>(request + kRequestParametersBeginOffset);
  auto end = *reinterpret_cast<const uintptr_t*>(request + kRequestParametersEndOffset);
  if (begin == 0 || end <= begin) {
    return std::string();
  }
  size_t count = (end - begin) / kParameterStride;
  std::string query;
  for (size_t index = 0; index < count; ++index) {
    const auto* entry = reinterpret_cast<const uint8_t*>(begin + index * kParameterStride);
    const char* key_pointer = *reinterpret_cast<const char* const*>(entry);
    const char* value_pointer = *reinterpret_cast<const char* const*>(entry + kParameterValueOffset);
    if (key_pointer == nullptr || value_pointer == nullptr) {
      continue;
    }
    std::string key(key_pointer);
    if (key != "path" && key != "type" && key != "folder") {
      continue;
    }
    if (!query.empty()) {
      query.push_back('&');
    }
    query.append(key);
    query.push_back('=');
    query.append(PercentEncode(value_pointer));
  }
  return query;
}

void Detour(const uint8_t* request, uint8_t* response) {
  if (request == nullptr || response == nullptr) {
    return;
  }
  std::string query = BuildQuery(request);
  if (query.empty()) {
    return;
  }
  std::string url = g_redirect.cdn_url + "/file?" + query;

  StringStorage key;
  StringStorage value;
  const char kLocation[] = "Location";
  char allocator = 0;
  g_redirect.construct_string(key.Pointer(), kLocation, &allocator);
  g_redirect.construct_string(value.Pointer(), url.c_str(), &allocator);

  g_redirect.set_header(response + kResponseHeadersOffset, key.Pointer(), value.Pointer());
  *reinterpret_cast<uint32_t*>(response + kResponseStatusOffset) = kRedirectStatus;

  g_redirect.destroy_string(value.Pointer());
  g_redirect.destroy_string(key.Pointer());
}

}

bool InstallFileRedirect(const std::string& cdn_url, std::string& detail) {
  if (g_redirect.installed) {
    detail = "the file redirect is already installed";
    return false;
  }
  if (cdn_url.empty()) {
    detail = "no CDN URL is configured";
    return false;
  }

  // The handler and set_header are unique, but the std::string constructor and destructor
  // are emitted as several identical COMDAT copies; the lowest-address one is the copy the
  // thunks call, so those two take the first match rather than requiring a unique one.
  size_t length = 0;
  uint8_t* handler = signature::FindUnique(kHandlerPattern, length);
  uint8_t* set_header = signature::FindUnique(kSetHeaderPattern, length);
  uint8_t* string_constructor = signature::FindFirst(kStringConstructorPattern, length);
  uint8_t* string_destructor = signature::FindFirst(kStringDestructorPattern, length);
  if (handler == nullptr) {
    detail = "the /file handler was not found; the server build is unsupported";
    return false;
  }
  if (set_header == nullptr || string_constructor == nullptr || string_destructor == nullptr) {
    detail = "the HTTP response helpers were not found; the server build is unsupported";
    return false;
  }

  std::string normalized = cdn_url;
  while (!normalized.empty() && normalized.back() == '/') {
    normalized.pop_back();
  }

  g_redirect.cdn_url = normalized;
  g_redirect.construct_string = reinterpret_cast<StringConstructor>(string_constructor);
  g_redirect.destroy_string = reinterpret_cast<StringDestructor>(string_destructor);
  g_redirect.set_header = reinterpret_cast<HeaderSetter>(set_header);

  static hooks::HookManager manager;
  HandlerFunction handler_function = reinterpret_cast<HandlerFunction>(handler);
  if (!manager.Prepare(handler_function, &Detour) || !manager.Install()) {
    detail = "the /file handler hook could not be installed: " + manager.error();
    return false;
  }

  g_redirect.installed = true;
  detail = "redirecting to " + normalized;
  return true;
}

}
