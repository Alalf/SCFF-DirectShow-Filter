@echo off
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
mkdir "tools\tmp"
"tools\bin\stylecopcli\StyleCopCLI.exe" -out "tools\tmp\stylecop_SCFF.Common.xml" -set "Settings.StyleCop" -proj "SCFF.Common\SCFF.Common.csproj"
"tools\bin\stylecopcli\StyleCopCLI.exe" -out "tools\tmp\stylecop_SCFF.GUI.xml" -set "Settings.StyleCop" -proj "SCFF.GUI\SCFF.GUI.csproj"
popd
PAUSE