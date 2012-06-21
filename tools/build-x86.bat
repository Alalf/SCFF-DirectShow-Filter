call "D:\Program Files\MSVC2010\VC\bin\vcvars32.bat"

msbuild /t:rebuild /p:Configuration=Release /p:Platform=Win32 "..\scff-dsf.sln"
msbuild /t:rebuild /p:Configuration=Debug /p:Platform=Win32 "..\scff-dsf.sln"
