
call "D:\Program Files\Microsoft SDKs\Windows\v7.1\Bin\SetEnv.cmd"
msbuild /t:build /p:Configuration=Debug /p:Platform=x64 "..\scff-dsf.sln"
msbuild /t:build /p:Configuration=Release /p:Platform=x64 "..\scff-dsf.sln"
