#ifndef NANOSSHARP_PLATFORM_WINDOWS_FILE_TRANSFER_REDIRECT_H_
#define NANOSSHARP_PLATFORM_WINDOWS_FILE_TRANSFER_REDIRECT_H_

#include <windows.h>

#include <string>

namespace nanossharp::platform::file_transfer {

// Hooks the dedicated server's internal `/file` HTTP handler and answers eligible
// downloads with a 302 redirect to `<cdn_url>/file?...`, offloading large client
// files to a CDN. The client's downloader follows the redirect. Returns true and
// reports the hooked location in `detail` on success, or false with a reason when
// the handler (or the std::string / set_header helpers it needs) cannot be located
// uniquely. Windows dedicated server only.
bool InstallFileRedirect(HMODULE server, const std::string& cdn_url, std::string& detail);

}

#endif
