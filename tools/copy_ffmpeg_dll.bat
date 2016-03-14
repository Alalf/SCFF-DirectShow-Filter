@ECHO OFF
SET ROOT_DIR=%~dp0..\
PUSHD "%ROOT_DIR%"

MKDIR "bin\Debug_x64\"
MKDIR "bin\Release_x64\"
MKDIR "bin\Debug_Win32\"
MKDIR "bin\Release_Win32\"

COPY /Y "ext\ffmpeg\x64\bin\avutil*.dll" "bin\Debug_x64\"
COPY /Y "ext\ffmpeg\x64\bin\swscale*.dll" "bin\Debug_x64\"

COPY /Y "ext\ffmpeg\x64\bin\avutil*.dll" "bin\Release_x64\"
COPY /Y "ext\ffmpeg\x64\bin\swscale*.dll" "bin\Release_x64\"

COPY /Y "ext\ffmpeg\Win32\bin\avutil*.dll" "bin\Debug_Win32\"
COPY /Y "ext\ffmpeg\Win32\bin\swscale*.dll" "bin\Debug_Win32\"

COPY /Y "ext\ffmpeg\Win32\bin\avutil*.dll" "bin\Release_Win32\"
COPY /Y "ext\ffmpeg\Win32\bin\swscale*.dll" "bin\Release_Win32\"

POPD
