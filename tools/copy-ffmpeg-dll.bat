@ECHO OFF
SET ROOT_DIR=%~dp0..\
PUSHD "%ROOT_DIR%"

MKDIR "dist\Debug-amd64\"
MKDIR "dist\Release-amd64\"
MKDIR "dist\Debug-x86\"
MKDIR "dist\Release-x86\"

COPY /Y "ext\ffmpeg\amd64\bin\avcodec*.dll" "dist\Debug-amd64\"
COPY /Y "ext\ffmpeg\amd64\bin\avutil*.dll" "dist\Debug-amd64\"
COPY /Y "ext\ffmpeg\amd64\bin\swscale*.dll" "dist\Debug-amd64\"

COPY /Y "ext\ffmpeg\amd64\bin\avcodec*.dll" "dist\Release-amd64\"
COPY /Y "ext\ffmpeg\amd64\bin\avutil*.dll" "dist\Release-amd64\"
COPY /Y "ext\ffmpeg\amd64\bin\swscale*.dll" "dist\Release-amd64\"

COPY /Y "ext\ffmpeg\x86\bin\avcodec*.dll" "dist\Debug-x86\"
COPY /Y "ext\ffmpeg\x86\bin\avutil*.dll" "dist\Debug-x86\"
COPY /Y "ext\ffmpeg\x86\bin\swscale*.dll" "dist\Debug-x86\"

COPY /Y "ext\ffmpeg\x86\bin\avcodec*.dll" "dist\Release-x86\"
COPY /Y "ext\ffmpeg\x86\bin\avutil*.dll" "dist\Release-x86\"
COPY /Y "ext\ffmpeg\x86\bin\swscale*.dll" "dist\Release-x86\"

POPD
