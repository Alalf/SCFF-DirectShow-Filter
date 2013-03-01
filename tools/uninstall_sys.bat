@echo off
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
"%systemroot%\syswow64\regsvr32.exe" /u "bin\Debug_x64\scff_dsf_x64.ax"
"%systemroot%\system32\regsvr32.exe" /u "bin\Debug_Win32\scff_dsf_Win32.ax"
"%systemroot%\syswow64\regsvr32.exe" /u "bin\Release_x64\scff_dsf_x64.ax"
"%systemroot%\system32\regsvr32.exe" /u "bin\Release_Win32\scff_dsf_Win32.ax"
popd