call "D:\Program Files\MSVC2010\VC\bin\vcvars32.bat"

msbuild /t:rebuild /p:Configuration=Release /p:Platform=x86 "..\scff-app.sln"
msbuild /t:rebuild /p:Configuration=Debug /p:Platform=x86 "..\scff-app.sln"
