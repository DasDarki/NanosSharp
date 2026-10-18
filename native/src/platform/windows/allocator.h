#ifndef NANOSSHARP_PLATFORM_WINDOWS_ALLOCATOR_H_
#define NANOSSHARP_PLATFORM_WINDOWS_ALLOCATOR_H_

namespace nanossharp::platform::allocator {

bool Resolve(void* create_environment, void* environment_destructor);
void Activate();

}

#endif
