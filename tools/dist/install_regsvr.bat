@echo off
set ERROR_VC2012_RUNTIME=0
set VC2012_REGKEY=HKLM\SOFTWARE\Microsoft\DevDiv\VC\Servicing\11.0\RuntimeMinimum
set VC2012_WOW6432_REGKEY=HKLM\SOFTWARE\WOW6432Node\Microsoft\DevDiv\VC\Servicing\11.0\RuntimeMinimum

reg query "%VC2012_REGKEY%" /v "Install" >NUL 2>&1
if "%ErrorLevel%"=="1" (
  echo "Please install Visual C++ Redistributable for Visual Studio 2012 at first"
  set ERROR_VC2012_RUNTIME=1
)
if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
  reg query "%VC2012_WOW6432_REGKEY%" /v "Install" >NUL 2>&1
  if "%ErrorLevel%"=="1" (
    echo "Please install Visual C++ Redistributable for Visual Studio 2012 (32bit) at first"
    set ERROR_VC2012_RUNTIME=1
  )
)
if "%ERROR_VC2012_RUNTIME%"=="1" (
  pause
  exit
)

set ROOT_DIR=%~dp0
pushd "%ROOT_DIR%"

if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
  "%systemroot%\syswow64\regsvr32.exe" "Win32\scff_dsf_Win32.ax"
  "%systemroot%\system32\regsvr32.exe" "x64\scff_dsf_x64.ax"
) else (
  "%systemroot%\system32\regsvr32.exe" "Win32\scff_dsf_Win32.ax"
)

popd