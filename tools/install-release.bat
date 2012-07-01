@echo off
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
"tools\bin\regsvrex32.exe" /s "dist\Release-x86\scff-dsf-x86.ax"
"tools\bin\regsvrex64.exe" /s "dist\Release-amd64\scff-dsf-amd64.ax"
popd