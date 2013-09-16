@echo off
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"

if "%1"=="-sys" (
  rem System
  if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    "%systemroot%\syswow64\regsvr32.exe" /u "bin\Debug_Win32\scff_dsf_Win32.ax"
    "%systemroot%\syswow64\regsvr32.exe" /u "bin\Release_Win32\scff_dsf_Win32.ax"
    "%systemroot%\system32\regsvr32.exe" /u "bin\Debug_x64\scff_dsf_x64.ax"
    "%systemroot%\system32\regsvr32.exe" /u "bin\Release_x64\scff_dsf_x64.ax"
  ) else (
    "%systemroot%\system32\regsvr32.exe" /u "bin\Debug_Win32\scff_dsf_Win32.ax"
    "%systemroot%\system32\regsvr32.exe" /u "bin\Release_Win32\scff_dsf_Win32.ax"
  )
) else (
  rem Normal
  if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    "tools\bin\regsvrex32.exe" /u "bin\Debug_Win32\scff_dsf_Win32.ax"
    "tools\bin\regsvrex32.exe" /u "bin\Release_Win32\scff_dsf_Win32.ax"
    "tools\bin\regsvrex64.exe" /u "bin\Debug_x64\scff_dsf_x64.ax"
    "tools\bin\regsvrex64.exe" /u "bin\Release_x64\scff_dsf_x64.ax"
  ) else (
    "tools\bin\regsvrex32.exe" /u "bin\Debug_Win32\scff_dsf_Win32.ax"
    "tools\bin\regsvrex32.exe" /u "bin\Release_Win32\scff_dsf_Win32.ax"
  )
)

popd