@echo off
if "%1"=="-sys" goto _MAIN_SYS
if "%PROCESSOR_ARCHITECTURE%"=="x86" goto _MAIN_X86
goto _MAIN_AMD64

rem ===================================================================
:_MAIN_X86
rem ===================================================================
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
"tools\bin\regsvrex32.exe" "bin\Release_Win32\scff_dsf_Win32.ax"
popd
exit /b

rem ===================================================================
:_MAIN_AMD64
rem ===================================================================
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
"tools\bin\regsvrex32.exe" "bin\Release_Win32\scff_dsf_Win32.ax"
"tools\bin\regsvrex64.exe" "bin\Release_x64\scff_dsf_x64.ax"
popd
exit /b

rem ===================================================================
:_MAIN_SYS
rem ===================================================================
if "%PROCESSOR_ARCHITECTURE%"=="x86" goto _MAIN_SYS_X86
goto _MAIN_SYS_AMD64

rem ===================================================================
:_MAIN_SYS_X86
rem ===================================================================
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
"%systemroot%\system32\regsvr32.exe" "bin\Release_Win32\scff_dsf_Win32.ax"
popd
exit /b

rem ===================================================================
:_MAIN_SYS_AMD64
rem ===================================================================
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
"%systemroot%\syswow64\regsvr32.exe" "bin\Release_Win32\scff_dsf_Win32.ax"
"%systemroot%\system32\regsvr32.exe" "bin\Release_x64\scff_dsf_x64.ax"
popd
exit /b
