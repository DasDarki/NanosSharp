.intel_syntax noprefix
.text
.globl Proxy_GetFileVersionInfoA
Proxy_GetFileVersionInfoA:
    jmp qword ptr [rip + real_GetFileVersionInfoA]
.globl Proxy_GetFileVersionInfoByHandle
Proxy_GetFileVersionInfoByHandle:
    jmp qword ptr [rip + real_GetFileVersionInfoByHandle]
.globl Proxy_GetFileVersionInfoExA
Proxy_GetFileVersionInfoExA:
    jmp qword ptr [rip + real_GetFileVersionInfoExA]
.globl Proxy_GetFileVersionInfoExW
Proxy_GetFileVersionInfoExW:
    jmp qword ptr [rip + real_GetFileVersionInfoExW]
.globl Proxy_GetFileVersionInfoSizeA
Proxy_GetFileVersionInfoSizeA:
    jmp qword ptr [rip + real_GetFileVersionInfoSizeA]
.globl Proxy_GetFileVersionInfoSizeExA
Proxy_GetFileVersionInfoSizeExA:
    jmp qword ptr [rip + real_GetFileVersionInfoSizeExA]
.globl Proxy_GetFileVersionInfoSizeExW
Proxy_GetFileVersionInfoSizeExW:
    jmp qword ptr [rip + real_GetFileVersionInfoSizeExW]
.globl Proxy_GetFileVersionInfoSizeW
Proxy_GetFileVersionInfoSizeW:
    jmp qword ptr [rip + real_GetFileVersionInfoSizeW]
.globl Proxy_GetFileVersionInfoW
Proxy_GetFileVersionInfoW:
    jmp qword ptr [rip + real_GetFileVersionInfoW]
.globl Proxy_VerFindFileA
Proxy_VerFindFileA:
    jmp qword ptr [rip + real_VerFindFileA]
.globl Proxy_VerFindFileW
Proxy_VerFindFileW:
    jmp qword ptr [rip + real_VerFindFileW]
.globl Proxy_VerInstallFileA
Proxy_VerInstallFileA:
    jmp qword ptr [rip + real_VerInstallFileA]
.globl Proxy_VerInstallFileW
Proxy_VerInstallFileW:
    jmp qword ptr [rip + real_VerInstallFileW]
.globl Proxy_VerLanguageNameA
Proxy_VerLanguageNameA:
    jmp qword ptr [rip + real_VerLanguageNameA]
.globl Proxy_VerLanguageNameW
Proxy_VerLanguageNameW:
    jmp qword ptr [rip + real_VerLanguageNameW]
.globl Proxy_VerQueryValueA
Proxy_VerQueryValueA:
    jmp qword ptr [rip + real_VerQueryValueA]
.globl Proxy_VerQueryValueW
Proxy_VerQueryValueW:
    jmp qword ptr [rip + real_VerQueryValueW]
