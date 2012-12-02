@ECHO OFF
SET ROOT_DIR=%~dp0..\
PUSHD "%ROOT_DIR%"

MKDIR "dist\Debug-x64\"
MKDIR "dist\Release-x64\"
MKDIR "dist\Debug-Win32\"
MKDIR "dist\Release-Win32\"

COPY /Y "ext\ffmpeg\x64\bin\avcodec*.dll" "dist\Debug-x64\"
COPY /Y "ext\ffmpeg\x64\bin\avutil*.dll" "dist\Debug-x64\"
COPY /Y "ext\ffmpeg\x64\bin\swscale*.dll" "dist\Debug-x64\"

COPY /Y "ext\ffmpeg\x64\bin\avcodec*.dll" "dist\Release-x64\"
COPY /Y "ext\ffmpeg\x64\bin\avutil*.dll" "dist\Release-x64\"
COPY /Y "ext\ffmpeg\x64\bin\swscale*.dll" "dist\Release-x64\"

COPY /Y "ext\ffmpeg\Win32\bin\avcodec*.dll" "dist\Debug-Win32\"
COPY /Y "ext\ffmpeg\Win32\bin\avutil*.dll" "dist\Debug-Win32\"
COPY /Y "ext\ffmpeg\Win32\bin\swscale*.dll" "dist\Debug-Win32\"

COPY /Y "ext\ffmpeg\Win32\bin\avcodec*.dll" "dist\Release-Win32\"
COPY /Y "ext\ffmpeg\Win32\bin\avutil*.dll" "dist\Release-Win32\"
COPY /Y "ext\ffmpeg\Win32\bin\swscale*.dll" "dist\Release-Win32\"

POPD
