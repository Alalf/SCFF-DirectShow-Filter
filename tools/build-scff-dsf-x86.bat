
call "D:\Program Files\MSVC2010\VC\bin\vcvars32.bat"
msbuild /t:build /p:Configuration=Debug /p:Platform=Win32 "..\scff-dsf.sln"
msbuild /t:build /p:Configuration=Release /p:Platform=Win32 "..\scff-dsf.sln"
