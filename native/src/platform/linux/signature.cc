#include "platform/linux/signature.h"

#include <link.h>

#include <algorithm>
#include <functional>
#include <string_view>
#include <vector>

namespace nanossharp::platform::signature {

namespace {

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

struct ScanRequest {
  const Pattern* pattern;
  size_t anchor_start;
  size_t anchor_length;
  size_t total_length;
  bool require_unique;
  uint8_t* found;
  bool ambiguous;
};

bool Done(const ScanRequest& request) {
  return request.ambiguous || (request.found != nullptr && !request.require_unique);
}

void ScanRange(uint8_t* start, uint8_t* end, ScanRequest& request) {
  const uint8_t* anchor = request.pattern->bytes.data() + request.anchor_start;
  std::boyer_moore_horspool_searcher searcher(anchor, anchor + request.anchor_length);
  for (uint8_t* position = std::search(start, end, searcher); position != end;
       position = std::search(position + 1, end, searcher)) {
    uint8_t* candidate = position - request.anchor_start;
    if (candidate < start || candidate + request.total_length > end || !Matches(candidate, *request.pattern)) {
      continue;
    }
    if (request.found != nullptr) {
      request.ambiguous = true;
      return;
    }
    request.found = candidate;
    if (!request.require_unique) {
      return;
    }
  }
}

int ScanObject(dl_phdr_info* info, size_t, void* data) {
  auto* request = static_cast<ScanRequest*>(data);
  if (info->dlpi_name != nullptr && info->dlpi_name[0] != '\0') {
    return 0;
  }
  for (int index = 0; index < info->dlpi_phnum; ++index) {
    const ElfW(Phdr)& header = info->dlpi_phdr[index];
    if (header.p_type != PT_LOAD || (header.p_flags & PF_X) == 0) {
      continue;
    }
    auto* start = reinterpret_cast<uint8_t*>(info->dlpi_addr + header.p_vaddr);
    ScanRange(start, start + header.p_filesz, *request);
    if (Done(*request)) {
      break;
    }
  }
  return 1;
}

uint8_t* Find(const char* text, size_t& length, bool require_unique) {
  Pattern pattern = Parse(text);
  length = pattern.bytes.size();
  ScanRequest request{};
  request.pattern = &pattern;
  request.total_length = length;
  request.require_unique = require_unique;
  LongestFixedRun(pattern, request.anchor_start, request.anchor_length);
  if (request.anchor_length == 0) {
    return nullptr;
  }
  dl_iterate_phdr(&ScanObject, &request);
  return request.ambiguous ? nullptr : request.found;
}

}

uint8_t* FindUnique(const char* text, size_t& length) {
  return Find(text, length, true);
}

uint8_t* FindFirst(const char* text, size_t& length) {
  return Find(text, length, false);
}

}
