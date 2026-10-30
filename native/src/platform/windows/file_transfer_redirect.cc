#include "platform/windows/file_transfer_redirect.h"

#include <cstdint>
#include <cstdio>
#include <string>

#include "platform/windows/inline_hook.h"
#include "platform/windows/signature.h"

namespace nanossharp::platform::file_transfer {

namespace {

// The dedicated server's internal `/file` HTTP handler and the std::string / header
// helpers it calls. The handler's prologue (two register saves plus pushes) relocates
// cleanly at 15 bytes, past the absolute jump the inline hook writes. set_header takes
// the response's header container at response+0x48, the status is an int at
// response+0x20, and the request's parameter vector lives at request+0x60..+0x68 with
// 64-byte entries holding an MSVC std::string key at +0 and value at +0x20.
constexpr char kHandlerPattern[] =
    "48 89 5C 24 08 48 89 74 24 20 57 41 54 41 55 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? "
    "48 33 C4 48 89 84 24 ?? ?? ?? ?? 4D 8B F8 4C 8B EA";
constexpr char kSetHeaderPattern[] =
    "48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 20 49 8B F0 48 8B EA 48 8B F9 48 8B CA E8 ?? ?? ?? ??";
constexpr char kStringConstructorPattern[] =
    "48 89 5C 24 10 48 89 6C 24 18 48 89 74 24 20 57 48 83 EC ?? 48 BF FF FF FF FF FF FF FF 7F 49 8B D8 48 8B EA 48 8B F1";
constexpr char kStringDestructorPattern[] =
    "40 53 48 83 EC ?? 48 8B 51 18 48 8B D9 48 83 FA 0F 76 ?? 48 8B 09 48 FF C2 48 81 FA 00 10 00 00";

constexpr size_t kHandlerPrologueLength = 15;
constexpr size_t kRequestParametersBeginOffset = 0x60;
constexpr size_t kRequestParametersEndOffset = 0x68;
constexpr size_t kParameterStride = 64;
constexpr size_t kParameterValueOffset = 0x20;
constexpr size_t kResponseHeadersOffset = 0x48;
constexpr size_t kResponseStatusOffset = 0x20;
constexpr size_t kStringSizeOffset = 0x10;
constexpr size_t kStringCapacityOffset = 0x18;
constexpr size_t kStringSmallCapacity = 15;
constexpr size_t kMaximumStringLength = 0x10000;
constexpr int kRedirectStatus = 302;

using StringConstructor = void* (*)(void* destination, const char* text, size_t length);
using StringDestructor = void (*)(void* string);
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

std::string ReadString(const uint8_t* string) {
  size_t length = *reinterpret_cast<const size_t*>(string + kStringSizeOffset);
  size_t capacity = *reinterpret_cast<const size_t*>(string + kStringCapacityOffset);
  if (length > kMaximumStringLength) {
    return std::string();
  }
  const char* data = capacity > kStringSmallCapacity
                         ? *reinterpret_cast<const char* const*>(string)
                         : reinterpret_cast<const char*>(string);
  if (data == nullptr) {
    return std::string();
  }
  return std::string(data, length);
}

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
    std::string key = ReadString(entry);
    if (key != "path" && key != "type" && key != "folder") {
      continue;
    }
    std::string value = ReadString(entry + kParameterValueOffset);
    if (!query.empty()) {
      query.push_back('&');
    }
    query.append(key);
    query.push_back('=');
    query.append(PercentEncode(value));
  }
  return query;
}

void Detour(void* /*functor*/, const uint8_t* request, uint8_t* response) {
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
  g_redirect.construct_string(key.Pointer(), kLocation, sizeof(kLocation) - 1);
  g_redirect.construct_string(value.Pointer(), url.c_str(), url.size());

  g_redirect.set_header(response + kResponseHeadersOffset, key.Pointer(), value.Pointer());
  *reinterpret_cast<int*>(response + kResponseStatusOffset) = kRedirectStatus;

  g_redirect.destroy_string(value.Pointer());
  g_redirect.destroy_string(key.Pointer());
}

}

bool InstallFileRedirect(HMODULE server, const std::string& cdn_url, std::string& detail) {
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
  uint8_t* handler = signature::FindUnique(server, kHandlerPattern, length);
  uint8_t* set_header = signature::FindUnique(server, kSetHeaderPattern, length);
  uint8_t* string_constructor = signature::FindFirst(server, kStringConstructorPattern, length);
  uint8_t* string_destructor = signature::FindFirst(server, kStringDestructorPattern, length);
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

  if (inline_hook::Install(handler, reinterpret_cast<void*>(&Detour), kHandlerPrologueLength) == nullptr) {
    detail = "the /file handler hook could not be installed";
    return false;
  }

  g_redirect.installed = true;
  char text[160];
  std::snprintf(text, sizeof(text), "redirecting to %s at server+0x%llx", normalized.c_str(),
                static_cast<unsigned long long>(handler - reinterpret_cast<uint8_t*>(server)));
  detail = text;
  return true;
}

}
