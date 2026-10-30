#ifndef NANOSSHARP_HOST_TOML_SETTINGS_H_
#define NANOSSHARP_HOST_TOML_SETTINGS_H_

#include <filesystem>
#include <string>

namespace nanossharp::host {

// Reads a single key's value from a section of a minimal TOML file, scanning line by
// line without a full parser. Surrounding quotes are stripped from the value. Returns
// an empty string when the file, the section or the key is absent.
std::string ReadTomlValue(const std::filesystem::path& file, const std::string& section, const std::string& key);

}

#endif
