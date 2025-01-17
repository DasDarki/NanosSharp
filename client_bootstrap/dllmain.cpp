#pragma once

#pragma comment(linker,"/export:GetFileVersionInfoA=c:\\windows\\system32\\version.GetFileVersionInfoA,@1")
#pragma comment(linker,"/export:GetFileVersionInfoByHandle=c:\\windows\\system32\\version.GetFileVersionInfoByHandle,@2")
#pragma comment(linker,"/export:GetFileVersionInfoExA=c:\\windows\\system32\\version.GetFileVersionInfoExA,@3")
#pragma comment(linker,"/export:GetFileVersionInfoExW=c:\\windows\\system32\\version.GetFileVersionInfoExW,@4")
#pragma comment(linker,"/export:GetFileVersionInfoSizeA=c:\\windows\\system32\\version.GetFileVersionInfoSizeA,@5")
#pragma comment(linker,"/export:GetFileVersionInfoSizeExA=c:\\windows\\system32\\version.GetFileVersionInfoSizeExA,@6")
#pragma comment(linker,"/export:GetFileVersionInfoSizeExW=c:\\windows\\system32\\version.GetFileVersionInfoSizeExW,@7")
#pragma comment(linker,"/export:GetFileVersionInfoSizeW=c:\\windows\\system32\\version.GetFileVersionInfoSizeW,@8")
#pragma comment(linker,"/export:GetFileVersionInfoW=c:\\windows\\system32\\version.GetFileVersionInfoW,@9")
#pragma comment(linker,"/export:VerFindFileA=c:\\windows\\system32\\version.VerFindFileA,@10")
#pragma comment(linker,"/export:VerFindFileW=c:\\windows\\system32\\version.VerFindFileW,@11")
#pragma comment(linker,"/export:VerInstallFileA=c:\\windows\\system32\\version.VerInstallFileA,@12")
#pragma comment(linker,"/export:VerInstallFileW=c:\\windows\\system32\\version.VerInstallFileW,@13")
#pragma comment(linker,"/export:VerLanguageNameA=c:\\windows\\system32\\version.VerLanguageNameA,@14")
#pragma comment(linker,"/export:VerLanguageNameW=c:\\windows\\system32\\version.VerLanguageNameW,@15")
#pragma comment(linker,"/export:VerQueryValueA=c:\\windows\\system32\\version.VerQueryValueA,@16")
#pragma comment(linker,"/export:VerQueryValueW=c:\\windows\\system32\\version.VerQueryValueW,@17")

#include "windows.h"
#include "iostream"
#include "fstream"

#include <filesystem>
#include <thread>

HMODULE hModule = LoadLibrary(L"c:\\windows\\system32\\version.dll");

HANDLE g_LockFileHandle = nullptr;
HANDLE g_JobHandle = nullptr;
HANDLE g_ProcessHandle = nullptr;

VOID DebugToFile(LPCSTR szInput)
{
    std::ofstream log("proxy-version.log", std::ios_base::app | std::ios_base::out);
    log << szInput;
    log << "\n";
}

BOOL StartProcessInAttachedMode(const wchar_t* exePath)
{
    // Create a job object
    g_JobHandle = CreateJobObject(nullptr, nullptr);
    if (!g_JobHandle)
    {
        std::wcerr << L"Failed to create job object. Error: " << GetLastError() << std::endl;
        return FALSE;
    }

    // Configure the job object to terminate processes on job close
    JOBOBJECT_EXTENDED_LIMIT_INFORMATION jobInfo = {};
    jobInfo.BasicLimitInformation.LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;
    if (!SetInformationJobObject(g_JobHandle, JobObjectExtendedLimitInformation, &jobInfo, sizeof(jobInfo)))
    {
        std::wcerr << L"Failed to set job object limits. Error: " << GetLastError() << std::endl;
        CloseHandle(g_JobHandle);
        g_JobHandle = nullptr;
        return FALSE;
    }

    // Set up the process start info
    STARTUPINFO si = { sizeof(STARTUPINFO) };
    PROCESS_INFORMATION pi = {};

    // Create the child process
    if (!CreateProcess(
        exePath,
        nullptr,      // Command line
        nullptr,      // Process security attributes
        nullptr,      // Thread security attributes
        FALSE,        // Inherit handles
        CREATE_SUSPENDED | CREATE_BREAKAWAY_FROM_JOB, // Start suspended to attach to the job
        nullptr,      // Environment
        std::filesystem::current_path().c_str(),      // Current directory
        &si,          // Startup info
        &pi))         // Process info
    {
        std::wcerr << L"Failed to create process. Error: " << GetLastError() << std::endl;
        CloseHandle(g_JobHandle);
        g_JobHandle = nullptr;
        return FALSE;
    }

    g_ProcessHandle = pi.hProcess;

    // Assign the process to the job
    if (!AssignProcessToJobObject(g_JobHandle, pi.hProcess))
    {
        std::wcerr << L"Failed to assign process to job object. Error: " << GetLastError() << std::endl;
        TerminateProcess(pi.hProcess, 1);
        CloseHandle(pi.hThread);
        CloseHandle(pi.hProcess);
        CloseHandle(g_JobHandle);
        g_JobHandle = nullptr;
        return FALSE;
    }

    // Resume the suspended process
    ResumeThread(pi.hThread);
    CloseHandle(pi.hThread);

    return TRUE;
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    std::filesystem::path lock_file = std::filesystem::current_path() / ".nanos-sharp.lock";
    std::filesystem::path exe_path = std::filesystem::current_path() / "dotnet" / "NanosSharp.Client.exe";

    if (ul_reason_for_call == DLL_PROCESS_ATTACH || ul_reason_for_call == DLL_THREAD_ATTACH) 
    {
        g_LockFileHandle = CreateFile(lock_file.c_str(),
            GENERIC_READ | GENERIC_WRITE,
            0, // Prevent sharing
            nullptr,
            CREATE_ALWAYS,
            FILE_ATTRIBUTE_NORMAL,
            nullptr);

        if (g_LockFileHandle == INVALID_HANDLE_VALUE)
        {
            // Lock file already in use or other error
            return TRUE;
        }

        // Start the process in attached mode
        if (!StartProcessInAttachedMode(exe_path.c_str()))
		{
			return TRUE;
		}
    }
    else if (ul_reason_for_call == DLL_PROCESS_ATTACH || ul_reason_for_call == DLL_THREAD_ATTACH)
    {
        // Close the file handle
        if (g_LockFileHandle)
        {
            CloseHandle(g_LockFileHandle);
            g_LockFileHandle = nullptr;
            DeleteFile(lock_file.c_str());
        }

        if (g_JobHandle)
        {
            CloseHandle(g_JobHandle);
            g_JobHandle = nullptr;
        }
    }

    return TRUE;
}

