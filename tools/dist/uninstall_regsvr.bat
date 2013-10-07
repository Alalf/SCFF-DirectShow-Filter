@echo off
if "%PROCESSOR_ARCHITECTURE%"=="x86" goto _MAIN_X86
goto _MAIN_AMD64

rem ===================================================================
:_MAIN_X86
rem ===================================================================
set ROOT_DIR=%~dp0
pushd "%ROOT_DIR%"
%systemroot%\system32\regsvr32.exe" /u "Win32\scff_dsf_Win32.ax"
popd
exit /b

rem ===================================================================
:_MAIN_AMD64
rem ===================================================================
set ROOT_DIR=%~dp0
pushd "%ROOT_DIR%"
"%systemroot%\syswow64\regsvr32.exe" /u "Win32\scff_dsf_Win32.ax"
"%systemroot%\system32\regsvr32.exe" /u "x64\scff_dsf_x64.ax"
popd
exit /b
