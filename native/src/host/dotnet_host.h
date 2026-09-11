#ifndef NANOSSHARP_HOST_DOTNET_HOST_H_
#define NANOSSHARP_HOST_DOTNET_HOST_H_

#include <filesystem>
#include <string>

namespace nanossharp::host {

class DotnetHost {
 public:
  explicit DotnetHost(std::filesystem::path runtime_directory);

  bool Start(std::string& error);
  void* GetEntryPoint(const char* type_name, const char* method_name,
                      std::string& error) const;

 private:
  std::filesystem::path runtime_directory_;
  void* load_assembly_and_get_function_pointer_ = nullptr;
};

}

#endif
