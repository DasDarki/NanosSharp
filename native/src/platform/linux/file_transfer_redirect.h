#ifndef NANOSSHARP_PLATFORM_LINUX_FILE_TRANSFER_REDIRECT_H_
#define NANOSSHARP_PLATFORM_LINUX_FILE_TRANSFER_REDIRECT_H_

#include <string>

namespace nanossharp::platform::file_transfer {

// Hooks the dedicated server's internal `/file` HTTP handler and answers eligible
// downloads with a 302 redirect to `<cdn_url>/file?...`, offloading large client files
// to a CDN. Returns true and reports the result in `detail` on success, or false with a
// reason when the handler or the std::string / set_header helpers cannot be located.
bool InstallFileRedirect(const std::string& cdn_url, std::string& detail);

}

#endif
