@echo off
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
"tools\bin\regsvrex32.exe" /s "dist\Debug-x86\scff-dsf-x86.ax"
"tools\bin\regsvrex64.exe" /s "dist\Debug-amd64\scff-dsf-amd64.ax"
popd