#include "platform/windows/rtti.h"

#include <dbghelp.h>

#include <array>
#include <cstdint>
#include <cstring>
#include <vector>

namespace nanossharp::platform::rtti {

namespace {

constexpr uint32_t kLocatorSignature = 1;
constexpr size_t kDescriptorNameOffset = 16;
constexpr DWORD kUndecorateTypeFlags = UNDNAME_NO_ARGUMENTS | UNDNAME_32_BIT_DECODE;

struct CompleteObjectLocator {
  uint32_t signature;
  uint32_t offset;
  uint32_t constructor_displacement_offset;
  uint32_t type_descriptor;
  uint32_t class_descriptor;
  uint32_t self;
};

struct Section {
  uint8_t* start;
  size_t size;
  bool writable;
  bool executable;
};

std::vector<Section> Sections(HMODULE module) {
  auto* base = reinterpret_cast<uint8_t*>(module);
  auto* dos = reinterpret_cast<IMAGE_DOS_HEADER*>(base);
  auto* headers = reinterpret_cast<IMAGE_NT_HEADERS64*>(base + dos->e_lfanew);
  IMAGE_SECTION_HEADER* section = IMAGE_FIRST_SECTION(headers);
  std::vector<Section> sections;
  for (WORD index = 0; index < headers->FileHeader.NumberOfSections; ++index, ++section) {
    sections.push_back({base + section->VirtualAddress, section->Misc.VirtualSize,
                        (section->Characteristics & IMAGE_SCN_MEM_WRITE) != 0,
                        (section->Characteristics & IMAGE_SCN_MEM_EXECUTE) != 0});
  }
  return sections;
}

uint8_t* FindData(const std::vector<Section>& sections, const void* needle, size_t length) {
  for (const Section& section : sections) {
    if (section.executable || section.size < length) {
      continue;
    }
    for (size_t offset = 0; offset + length <= section.size; ++offset) {
      if (std::memcmp(section.start + offset, needle, length) == 0) {
        return section.start + offset;
      }
    }
  }
  return nullptr;
}

const CompleteObjectLocator* FindLocator(const std::vector<Section>& sections, uint8_t* base,
                                         uint32_t descriptor, bool is_primary = true) {
  for (const Section& section : sections) {
    if (section.writable || section.executable) {
      continue;
    }
    for (size_t offset = 0; offset + sizeof(CompleteObjectLocator) <= section.size; offset += 4) {
      auto* locator = reinterpret_cast<const CompleteObjectLocator*>(section.start + offset);
      auto self = static_cast<uint32_t>(reinterpret_cast<const uint8_t*>(locator) - base);
      if (locator->signature == kLocatorSignature && (locator->offset == 0) == is_primary &&
          locator->type_descriptor == descriptor && locator->self == self) {
        return locator;
      }
    }
  }
  return nullptr;
}

const CompleteObjectLocator* LocatorOf(const void* polymorphic_object) {
  if (polymorphic_object == nullptr) {
    return nullptr;
  }
  auto* const* vtable = *static_cast<void* const* const*>(polymorphic_object);
  auto* locator = static_cast<const CompleteObjectLocator*>(vtable[-1]);
  return locator != nullptr && locator->signature == kLocatorSignature ? locator : nullptr;
}

}

void** FindVtable(HMODULE module, const char* decorated_name) {
  auto* base = reinterpret_cast<uint8_t*>(module);
  std::vector<Section> sections = Sections(module);
  uint8_t* name = FindData(sections, decorated_name, std::strlen(decorated_name) + 1);
  if (name == nullptr) {
    return nullptr;
  }

  auto descriptor = static_cast<uint32_t>(name - kDescriptorNameOffset - base);
  const CompleteObjectLocator* locator = FindLocator(sections, base, descriptor);
  if (locator == nullptr) {
    return nullptr;
  }

  auto address = reinterpret_cast<uintptr_t>(locator);
  uint8_t* reference = FindData(sections, &address, sizeof(address));
  return reference != nullptr ? reinterpret_cast<void**>(reference + sizeof(void*)) : nullptr;
}

bool FindBaseOffset(HMODULE module, const char* decorated_name, uint32_t& offset) {
  auto* base = reinterpret_cast<uint8_t*>(module);
  std::vector<Section> sections = Sections(module);
  uint8_t* name = FindData(sections, decorated_name, std::strlen(decorated_name) + 1);
  if (name == nullptr) {
    return false;
  }

  auto descriptor = static_cast<uint32_t>(name - kDescriptorNameOffset - base);
  const CompleteObjectLocator* locator = FindLocator(sections, base, descriptor, false);
  if (locator == nullptr) {
    return false;
  }
  offset = locator->offset;
  return true;
}

const char* DecoratedTypeName(const void* polymorphic_object) {
  const CompleteObjectLocator* locator = LocatorOf(polymorphic_object);
  if (locator == nullptr) {
    return nullptr;
  }
  auto* base = reinterpret_cast<const uint8_t*>(locator) - locator->self;
  return reinterpret_cast<const char*>(base + locator->type_descriptor + kDescriptorNameOffset);
}

std::string ReadableTypeName(const void* polymorphic_object) {
  const char* decorated = DecoratedTypeName(polymorphic_object);
  if (decorated == nullptr) {
    return {};
  }
  std::array<char, 4096> buffer{};
  DWORD length = UnDecorateSymbolName(decorated + 1, buffer.data(), static_cast<DWORD>(buffer.size()),
                                      kUndecorateTypeFlags);
  return length != 0 ? std::string(buffer.data(), length) : std::string(decorated);
}

bool HasTypeName(const void* polymorphic_object, const char* decorated_name) {
  const char* name = DecoratedTypeName(polymorphic_object);
  return name != nullptr && std::strcmp(name, decorated_name) == 0;
}

bool HasTypePrefix(const void* polymorphic_object, const char* decorated_prefix) {
  const char* name = DecoratedTypeName(polymorphic_object);
  return name != nullptr && std::strncmp(name, decorated_prefix, std::strlen(decorated_prefix)) == 0;
}

}
