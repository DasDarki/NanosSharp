#ifndef NANOSSHARP_PLATFORM_LINUX_ISOLATED_LIBRARIES_H_
#define NANOSSHARP_PLATFORM_LINUX_ISOLATED_LIBRARIES_H_

#include <string>

namespace nanossharp::host {

void PreloadIsolatedSystemLibraries();
void* LoadIsolatedLibrary(const char* path, std::string& error);

}

#endif
