#include "host/toml_settings.h"

#include <fstream>

namespace nanossharp::host {

namespace {

void Trim(std::string& text) {
  size_t begin = text.find_first_not_of(" \t\r\n");
  size_t end = text.find_last_not_of(" \t\r\n");
  text = begin == std::string::npos ? std::string() : text.substr(begin, end - begin + 1);
}

void StripQuotes(std::string& text) {
  if (text.size() >= 2 && (text.front() == '"' || text.front() == '\'') && text.back() == text.front()) {
    text = text.substr(1, text.size() - 2);
  }
}

}

std::string ReadTomlValue(const std::filesystem::path& file, const std::string& section, const std::string& key) {
  std::ifstream stream(file);
  if (!stream) {
    return std::string();
  }
  std::string wanted_section = "[" + section + "]";
  std::string line;
  bool in_section = false;
  while (std::getline(stream, line)) {
    size_t comment = line.find('#');
    if (comment != std::string::npos) {
      line.erase(comment);
    }
    Trim(line);
    if (line.empty()) {
      continue;
    }
    if (line.front() == '[') {
      in_section = line == wanted_section;
      continue;
    }
    if (!in_section) {
      continue;
    }
    size_t equals = line.find('=');
    if (equals == std::string::npos) {
      continue;
    }
    std::string current_key = line.substr(0, equals);
    std::string value = line.substr(equals + 1);
    Trim(current_key);
    Trim(value);
    if (current_key == key) {
      StripQuotes(value);
      return value;
    }
  }
  return std::string();
}

}
