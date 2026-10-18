#include "platform/windows/signature.h"

#include <algorithm>
#include <functional>
#include <string_view>
#include <vector>

namespace nanossharp::platform::signature {

namespace {

constexpr uint8_t kCallOpcode = 0xE8;
constexpr size_t kCallLength = 5;

struct Pattern {
  std::vector<uint8_t> bytes;
  std::vector<bool> fixed;
};

int HexValue(char character) {
  if (character >= '0' && character <= '9') {
    return character - '0';
  }
  if (character >= 'A' && character <= 'F') {
    return character - 'A' + 10;
  }
  return -1;
}

Pattern Parse(const char* text) {
  Pattern pattern;
  std::string_view view(text);
  for (size_t index = 0; index < view.size(); index += 3) {
    if (view[index] == '?') {
      pattern.bytes.push_back(0);
      pattern.fixed.push_back(false);
      continue;
    }
    pattern.bytes.push_back(static_cast<uint8_t>(HexValue(view[index]) * 16 + HexValue(view[index + 1])));
    pattern.fixed.push_back(true);
  }
  return pattern;
}

bool Matches(const uint8_t* candidate, const Pattern& pattern) {
  for (size_t index = 0; index < pattern.bytes.size(); ++index) {
    if (pattern.fixed[index] && candidate[index] != pattern.bytes[index]) {
      return false;
    }
  }
  return true;
}

void LongestFixedRun(const Pattern& pattern, size_t& start, size_t& length) {
  start = 0;
  length = 0;
  size_t current = 0;
  for (size_t index = 0; index <= pattern.bytes.size(); ++index) {
    if (index < pattern.bytes.size() && pattern.fixed[index]) {
      ++current;
      continue;
    }
    if (current > length) {
      length = current;
      start = index - current;
    }
    current = 0;
  }
}

}

uint8_t* FindUnique(HMODULE module, const char* text, size_t& length) {
  Pattern pattern = Parse(text);
  length = pattern.bytes.size();
  size_t anchor_start = 0;
  size_t anchor_length = 0;
  LongestFixedRun(pattern, anchor_start, anchor_length);
  if (anchor_length == 0) {
    return nullptr;
  }

  auto* base = reinterpret_cast<uint8_t*>(module);
  auto* dos = reinterpret_cast<IMAGE_DOS_HEADER*>(base);
  auto* headers = reinterpret_cast<IMAGE_NT_HEADERS64*>(base + dos->e_lfanew);
  IMAGE_SECTION_HEADER* section = IMAGE_FIRST_SECTION(headers);
  const uint8_t* anchor = pattern.bytes.data() + anchor_start;
  std::boyer_moore_horspool_searcher searcher(anchor, anchor + anchor_length);
  uint8_t* found = nullptr;
  for (WORD index = 0; index < headers->FileHeader.NumberOfSections; ++index, ++section) {
    if ((section->Characteristics & IMAGE_SCN_MEM_EXECUTE) == 0) {
      continue;
    }
    uint8_t* start = base + section->VirtualAddress;
    uint8_t* end = start + section->Misc.VirtualSize;
    for (uint8_t* position = std::search(start, end, searcher); position != end;
         position = std::search(position + 1, end, searcher)) {
      uint8_t* candidate = position - anchor_start;
      if (candidate < start || candidate + length > end || !Matches(candidate, pattern)) {
        continue;
      }
      if (found != nullptr) {
        return nullptr;
      }
      found = candidate;
    }
  }
  return found;
}

uint8_t* CallTarget(uint8_t* call) {
  if (call == nullptr || *call != kCallOpcode) {
    return nullptr;
  }
  int32_t displacement = 0;
  std::copy_n(call + 1, sizeof(displacement), reinterpret_cast<uint8_t*>(&displacement));
  return call + kCallLength + displacement;
}

}
