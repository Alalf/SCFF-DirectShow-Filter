@echo off
set ROOT_DIR=%~dp0
pushd "%ROOT_DIR%"

if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
  "%systemroot%\syswow64\regsvr32.exe" /u "Win32\scff_dsf_Win32.ax"
  "%systemroot%\system32\regsvr32.exe" /u "x64\scff_dsf_x64.ax"
) else (
  "%systemroot%\system32\regsvr32.exe" /u "Win32\scff_dsf_Win32.ax"
)

popd