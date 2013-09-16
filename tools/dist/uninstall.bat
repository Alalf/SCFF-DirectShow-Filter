@echo off
set ROOT_DIR=%~dp0
pushd "%ROOT_DIR%"

regsvrex32 /u "Win32\scff_dsf_Win32.ax"
if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
  regsvrex64 /u "x64\scff_dsf_x64.ax"
)

popd