@echo off
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"

mkdir "dist\Debug-amd64\"
mkdir "dist\Release-amd64\"
mkdir "dist\Debug-x86\"
mkdir "dist\Release-x86\"

copy /y "ext\ffmpeg\amd64\bin\avcodec*.dll" "dist\Debug-amd64\"
copy /y "ext\ffmpeg\amd64\bin\avutil*.dll" "dist\Debug-amd64\"
copy /y "ext\ffmpeg\amd64\bin\swscale*.dll" "dist\Debug-amd64\"

copy /y "ext\ffmpeg\amd64\bin\avcodec*.dll" "dist\Release-amd64\"
copy /y "ext\ffmpeg\amd64\bin\avutil*.dll" "dist\Release-amd64\"
copy /y "ext\ffmpeg\amd64\bin\swscale*.dll" "dist\Release-amd64\"

copy /y "ext\ffmpeg\x86\bin\avcodec*.dll" "dist\Debug-x86\"
copy /y "ext\ffmpeg\x86\bin\avutil*.dll" "dist\Debug-x86\"
copy /y "ext\ffmpeg\x86\bin\swscale*.dll" "dist\Debug-x86\"

copy /y "ext\ffmpeg\x86\bin\avcodec*.dll" "dist\Release-x86\"
copy /y "ext\ffmpeg\x86\bin\avutil*.dll" "dist\Release-x86\"
copy /y "ext\ffmpeg\x86\bin\swscale*.dll" "dist\Release-x86\"

popd
