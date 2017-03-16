@echo off
if "%PROCESSOR_ARCHITECTURE%"=="x86" goto _MAIN_X86
goto _MAIN_AMD64

rem ===================================================================
:_MAIN_X86
rem ===================================================================
set ROOT_DIR=%~dp0
pushd "%ROOT_DIR%"
regsvrex32 "Win32\scff_dsf_Win32.ax"
popd
exit /b

rem ===================================================================
:_MAIN_AMD64
rem ===================================================================
set ROOT_DIR=%~dp0
pushd "%ROOT_DIR%"
regsvrex32 "Win32\scff_dsf_Win32.ax"
regsvrex64 "x64\scff_dsf_x64.ax"
popd
exit /b