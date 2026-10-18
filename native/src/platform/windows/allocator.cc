#include "platform/windows/allocator.h"

#include <windows.h>

#include <atomic>
#include <cstdint>
#include <cstdlib>
#include <cstring>
#include <functional>
#include <new>
#include <unordered_set>

namespace nanossharp::platform::allocator {

namespace {

using AllocateFunction = void* (*)(size_t size);
using FreeFunction = void (*)(void* pointer, size_t size);

constexpr size_t kScanLength = 128;
constexpr uint8_t kMoveEcxOpcode = 0xB9;
constexpr uint8_t kMoveEdxOpcode = 0xBA;
constexpr uint8_t kCallOpcode = 0xE8;
constexpr size_t kImmediateLength = 5;
constexpr size_t kCallLength = 5;
constexpr size_t kMaximumCallDistance = 4;
constexpr uint8_t kAllocatePrologue[] = {0x48, 0x85, 0xC9};
constexpr uint8_t kFreePrologue[] = {0x48, 0x83, 0xEC, 0x28, 0x48, 0x85, 0xC9};

template <typename T>
struct MallocAllocator {
  using value_type = T;

  MallocAllocator() = default;
  template <typename U>
  MallocAllocator(const MallocAllocator<U>&) {}

  T* allocate(size_t count) {
    void* pointer = std::malloc(count * sizeof(T));
    if (pointer == nullptr) {
      throw std::bad_alloc();
    }
    return static_cast<T*>(pointer);
  }

  void deallocate(T* pointer, size_t) { std::free(pointer); }

  template <typename U>
  bool operator==(const MallocAllocator<U>&) const {
    return true;
  }
};

using LocalAllocations = std::unordered_set<void*, std::hash<void*>, std::equal_to<void*>, MallocAllocator<void*>>;

AllocateFunction game_allocate = nullptr;
FreeFunction game_free = nullptr;
std::atomic<bool> active = false;
std::atomic<size_t> local_count = 0;
SRWLOCK lock = SRWLOCK_INIT;

LocalAllocations& Locals() {
  static auto* allocations = new (std::malloc(sizeof(LocalAllocations))) LocalAllocations();
  return *allocations;
}

uint8_t* CallTarget(uint8_t* call) {
  int32_t displacement = 0;
  std::memcpy(&displacement, call + 1, sizeof(displacement));
  return call + kCallLength + displacement;
}

uint8_t* FindCallAfterImmediate(void* function, uint8_t opcode) {
  auto* bytes = static_cast<uint8_t*>(function);
  for (size_t index = 0; index < kScanLength; ++index) {
    if (bytes[index] != opcode) {
      continue;
    }
    for (size_t distance = 0; distance <= kMaximumCallDistance; ++distance) {
      uint8_t* call = bytes + index + kImmediateLength + distance;
      if (*call == kCallOpcode) {
        return CallTarget(call);
      }
    }
  }
  return nullptr;
}

bool StartsWith(const uint8_t* function, const uint8_t* prologue, size_t length) {
  return function != nullptr && std::memcmp(function, prologue, length) == 0;
}

void* AllocateLocal(size_t size) {
  void* pointer = std::malloc(size == 0 ? 1 : size);
  if (pointer == nullptr) {
    return nullptr;
  }
  AcquireSRWLockExclusive(&lock);
  if (active.load(std::memory_order_relaxed)) {
    ReleaseSRWLockExclusive(&lock);
    std::free(pointer);
    return game_allocate(size);
  }
  Locals().insert(pointer);
  local_count.fetch_add(1, std::memory_order_relaxed);
  ReleaseSRWLockExclusive(&lock);
  return pointer;
}

bool FreeLocal(void* pointer) {
  if (local_count.load(std::memory_order_acquire) == 0) {
    return false;
  }
  AcquireSRWLockExclusive(&lock);
  bool found = Locals().erase(pointer) != 0;
  if (found) {
    local_count.fetch_sub(1, std::memory_order_relaxed);
  }
  ReleaseSRWLockExclusive(&lock);
  if (found) {
    std::free(pointer);
  }
  return found;
}

void* Allocate(size_t size) {
  if (active.load(std::memory_order_acquire)) {
    return game_allocate(size);
  }
  return AllocateLocal(size);
}

void Free(void* pointer) {
  if (pointer == nullptr || FreeLocal(pointer)) {
    return;
  }
  game_free(pointer, 0);
}

void* AllocateOrThrow(size_t size) {
  void* pointer = Allocate(size);
  if (pointer == nullptr) {
    throw std::bad_alloc();
  }
  return pointer;
}

}

bool Resolve(void* create_environment, void* environment_destructor) {
  uint8_t* allocate = FindCallAfterImmediate(create_environment, kMoveEcxOpcode);
  uint8_t* deallocate = FindCallAfterImmediate(environment_destructor, kMoveEdxOpcode);
  if (!StartsWith(allocate, kAllocatePrologue, sizeof(kAllocatePrologue)) ||
      !StartsWith(deallocate, kFreePrologue, sizeof(kFreePrologue))) {
    return false;
  }
  game_allocate = reinterpret_cast<AllocateFunction>(allocate);
  game_free = reinterpret_cast<FreeFunction>(deallocate);
  return true;
}

void Activate() {
  if (game_allocate == nullptr || game_free == nullptr) {
    return;
  }
  AcquireSRWLockExclusive(&lock);
  active.store(true, std::memory_order_release);
  ReleaseSRWLockExclusive(&lock);
}

}

using nanossharp::platform::allocator::AllocateOrThrow;

void* operator new(size_t size) { return AllocateOrThrow(size); }

void* operator new[](size_t size) { return AllocateOrThrow(size); }

void* operator new(size_t size, const std::nothrow_t&) noexcept {
  try {
    return AllocateOrThrow(size);
  } catch (...) {
    return nullptr;
  }
}

void* operator new[](size_t size, const std::nothrow_t&) noexcept {
  try {
    return AllocateOrThrow(size);
  } catch (...) {
    return nullptr;
  }
}

void operator delete(void* pointer) noexcept { nanossharp::platform::allocator::Free(pointer); }

void operator delete[](void* pointer) noexcept { nanossharp::platform::allocator::Free(pointer); }

void operator delete(void* pointer, size_t) noexcept { nanossharp::platform::allocator::Free(pointer); }

void operator delete[](void* pointer, size_t) noexcept { nanossharp::platform::allocator::Free(pointer); }

void operator delete(void* pointer, const std::nothrow_t&) noexcept { nanossharp::platform::allocator::Free(pointer); }

void operator delete[](void* pointer, const std::nothrow_t&) noexcept { nanossharp::platform::allocator::Free(pointer); }
