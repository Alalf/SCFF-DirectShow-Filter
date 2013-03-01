@echo off
set ROOT_DIR=%~dp0
pushd "%ROOT_DIR%"

if exist "scff_dsf_Win32.ax" (
  if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    "%systemroot%\syswow64\regsvr32.exe" /u scff_dsf_Win32.ax
  ) else (
    "%systemroot%\system32\regsvr32.exe" /u scff_dsf_Win32.ax
  )
)
if exist "scff_dsf_x64.ax" (
  if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    "%systemroot%\system32\regsvr32.exe" /u scff_dsf_x64.ax
  ) else (
    echo "nop"
  )
)

popd