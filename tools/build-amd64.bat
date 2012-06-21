call "D:\Program Files\Microsoft SDKs\Windows\v7.1\Bin\SetEnv.cmd"

msbuild /t:rebuild /p:Configuration=Release /p:Platform=x64 "..\scff-dsf.sln"
msbuild /t:rebuild /p:Configuration=Debug /p:Platform=x64 "..\scff-dsf.sln"
