@echo off
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
"tools\bin\regsvrex64.exe" /u /s "bin\Debug-x64\scff-dsf-x64.ax"
"tools\bin\regsvrex32.exe" /u /s "bin\Debug-Win32\scff-dsf-Win32.ax"
"tools\bin\regsvrex64.exe" /u /s "bin\Release-x64\scff-dsf-x64.ax"
"tools\bin\regsvrex32.exe" /u /s "bin\Release-Win32\scff-dsf-Win32.ax"
popd