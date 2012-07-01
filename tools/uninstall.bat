@echo off
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
"tools\bin\regsvrex64.exe" /u /s "dist\Debug-amd64\scff-dsf-amd64.ax"
"tools\bin\regsvrex32.exe" /u /s "dist\Debug-x86\scff-dsf-x86.ax"
"tools\bin\regsvrex64.exe" /u /s "dist\Release-amd64\scff-dsf-amd64.ax"
"tools\bin\regsvrex32.exe" /u /s "dist\Release-x86\scff-dsf-x86.ax"
popd