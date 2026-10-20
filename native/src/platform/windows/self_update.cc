#include "platform/windows/self_update.h"

namespace nanossharp::platform::self_update {

namespace {

constexpr wchar_t kStagingDirectory[] = L"update";
constexpr wchar_t kStagedDirectory[] = L"staged";
constexpr wchar_t kMarkerFile[] = L"pending";
constexpr wchar_t kModuleName[] = L"version.dll";
constexpr wchar_t kOldModuleSuffix[] = L".old";
constexpr const wchar_t* kReplacedEntries[] = {L"runtime", L"dotnet", L"VERSION", L"LICENSE"};

void Replace(const std::filesystem::path& from, const std::filesystem::path& to) {
  std::error_code error;
  if (!std::filesystem::exists(from, error)) {
    return;
  }
  std::filesystem::remove_all(to, error);
  std::filesystem::rename(from, to, error);
}

bool IsComplete(const std::filesystem::path& staged) {
  std::error_code error;
  return std::filesystem::exists(staged / kModuleName, error) &&
         std::filesystem::exists(staged / L"NanosSharp" / L"runtime", error) &&
         std::filesystem::exists(staged / L"NanosSharp" / L"dotnet", error);
}

}

bool Apply(const std::filesystem::path& nanossharp_directory, const std::wstring& module_path, const LogSink& log) {
  std::error_code error;
  std::filesystem::path old_module = std::filesystem::path(module_path).concat(kOldModuleSuffix);
  std::filesystem::remove(old_module, error);

  std::filesystem::path staging = nanossharp_directory / kStagingDirectory;
  std::filesystem::path staged = staging / kStagedDirectory;
  if (!std::filesystem::exists(staging / kMarkerFile, error)) {
    return false;
  }

  if (!IsComplete(staged)) {
    std::filesystem::remove_all(staging, error);
    log("a staged client update was incomplete and was discarded");
    return false;
  }

  std::filesystem::path staged_nanossharp = staged / L"NanosSharp";
  for (const wchar_t* entry : kReplacedEntries) {
    Replace(staged_nanossharp / entry, nanossharp_directory / entry);
  }

  std::filesystem::rename(module_path, old_module, error);
  std::filesystem::rename(staged / kModuleName, module_path, error);
  std::filesystem::remove_all(staging, error);
  log("a NanosSharp client update was applied, it starts fresh on the next game launch");
  return true;
}

}
