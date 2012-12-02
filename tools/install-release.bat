@echo off
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
"tools\bin\regsvrex32.exe" /s "dist\Release-Win32\scff-dsf-Win32.ax"
"tools\bin\regsvrex64.exe" /s "dist\Release-x64\scff-dsf-x64.ax"
popd