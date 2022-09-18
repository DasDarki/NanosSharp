//
// Created by DasDarki on 18.09.2022.
//

#ifndef NANOSSHARP_RUNTIME_NATIVES_H
#define NANOSSHARP_RUNTIME_NATIVES_H

#include <string>
#include <vector>

#ifdef __cplusplus
#define EXTERN extern "C"
#else
#define EXTERN
#endif

#ifdef _WIN32
#define EXPORT EXTERN __declspec(dllexport)
#define IMPORT EXTERN __declspec(dllimport)
#else
#define EXPORT EXTERN __attribute__((visibility("default")))
#define IMPORT
#endif

const char* allocate_string(const std::string& str, int32_t& size) {
    size_t stringSize = str.size();
    size = (int32_t) stringSize;
    char* writable = new char[stringSize + 1];
    std::copy(str.begin(), str.end(), writable);
    writable[stringSize] = '\0';
    return writable;
}

const char** allocate_string_array(std::vector<std::string> arr, uint32_t& size) {
    size = arr.size();
    auto out = new const char*[size];
    for (auto i = 0; i < size; i++) {
        auto el = arr[i];
        auto elSize = el.size();
        auto str = el.c_str();
        auto allocStr = new char[elSize + 1];
        for (auto j = 0; j < elSize; j++) allocStr[j] = str[j];
        allocStr[elSize] = '\0';
        out[i] = allocStr;
    }

    return out;
}

EXPORT void FreeString(const char* string) {
    delete[] string;
}

EXPORT void FreeStringArray(const char** stringArray, uint32_t size) {
    for (int i = 0; i < size; i++) delete[] stringArray[i];
    delete[] stringArray;
}

// <% AUTOGENERATE(FUNC_DECLARE, 0) %>

#endif //NANOSSHARP_RUNTIME_NATIVES_H
