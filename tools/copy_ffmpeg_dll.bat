@ECHO OFF
SET ROOT_DIR=%~dp0..\
PUSHD "%ROOT_DIR%"

MKDIR "bin\Debug-x64\"
MKDIR "bin\Release-x64\"
MKDIR "bin\Debug-Win32\"
MKDIR "bin\Release-Win32\"

COPY /Y "ext\ffmpeg\x64\bin\avcodec*.dll" "bin\Debug-x64\"
COPY /Y "ext\ffmpeg\x64\bin\avutil*.dll" "bin\Debug-x64\"
COPY /Y "ext\ffmpeg\x64\bin\swscale*.dll" "bin\Debug-x64\"

COPY /Y "ext\ffmpeg\x64\bin\avcodec*.dll" "bin\Release-x64\"
COPY /Y "ext\ffmpeg\x64\bin\avutil*.dll" "bin\Release-x64\"
COPY /Y "ext\ffmpeg\x64\bin\swscale*.dll" "bin\Release-x64\"

COPY /Y "ext\ffmpeg\Win32\bin\avcodec*.dll" "bin\Debug-Win32\"
COPY /Y "ext\ffmpeg\Win32\bin\avutil*.dll" "bin\Debug-Win32\"
COPY /Y "ext\ffmpeg\Win32\bin\swscale*.dll" "bin\Debug-Win32\"

COPY /Y "ext\ffmpeg\Win32\bin\avcodec*.dll" "bin\Release-Win32\"
COPY /Y "ext\ffmpeg\Win32\bin\avutil*.dll" "bin\Release-Win32\"
COPY /Y "ext\ffmpeg\Win32\bin\swscale*.dll" "bin\Release-Win32\"

POPD
