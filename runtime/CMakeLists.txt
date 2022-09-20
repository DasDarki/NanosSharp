cmake_minimum_required(VERSION 3.15)

project(nanossharp_runtime LANGUAGES CXX)

set(CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS 1)

add_library(nanossharp_runtime SHARED src/main.cpp src/runtime.h src/icvalues.h)

target_include_directories(nanossharp_runtime PUBLIC deps/module-sdk/include)
target_include_directories(nanossharp_runtime PUBLIC deps/dotnet)
target_compile_features(nanossharp_runtime PRIVATE cxx_std_17)
set_target_properties(nanossharp_runtime PROPERTIES LINKER_LANGUAGE CXX)

if(WIN32)
    target_link_libraries(nanossharp_runtime PRIVATE "${CMAKE_SOURCE_DIR}/deps/module-sdk/lib/lua.lib")
elseif(UNIX AND NOT APPLE)
    target_link_libraries(nanossharp_runtime PRIVATE "${CMAKE_SOURCE_DIR}/deps/module-sdk/lib/liblua.a")
endif()