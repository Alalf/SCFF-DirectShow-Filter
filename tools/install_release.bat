@echo off
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
"tools\bin\regsvrex32.exe" /s "bin\Release_Win32\scff_dsf_Win32.ax"
"tools\bin\regsvrex64.exe" /s "bin\Release_x64\scff_dsf_x64.ax"
popd