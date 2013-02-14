@echo off
SET ROOT_DIR=%~dp0..\
PUSHD "%ROOT_DIR%"

SET OUTPUT_DIR=tools\tmp
SET OUTPUT=http://localhost:8080/publish/first?password=secret
SET OUTPUT_FILE=%OUTPUT_DIR%\test_ffmpeg_webm.mkv
SET VIDEO=SCFF DirectShow Filter
SET AUDIO=Mixer (Creative SB X-Fi)
SET FFMPEG_EXE=ext\ffmpeg\x64\bin\ffmpeg.exe

MKDIR "%OUTPUT_DIR%"
DEL "%OUTPUT%"

REM [WEBM]
"%FFMPEG_EXE%" -rtbufsize 100MB -r 30 -s 640x360 -f dshow -i video="%VIDEO%":audio="%AUDIO%" -vf fps="fps=30" -vb 700k -r 30 -s 640x360 -pix_fmt yuv420p -maxrate 700k -bufsize 1400k -crf 23 -qmin 10 -qmax 51 -vcodec libvpx -preset medium -async 100 -acodec libvorbis -ar 48000 -ab 96k -ac 2 -vol 256 -threads 4 -metadata maxBitrate="700k" -f webm "%OUTPUT%" "%OUTPUT_FILE%"

REM [MATROSKA]
REM "%FFMPEG_EXE%" -rtbufsize 100MB -r 30 -s 640x360 -f dshow -i video="%VIDEO%":audio="%AUDIO%" -vf fps="fps=30" -vb 700k -r 30 -s 640x360 -pix_fmt yuv420p -maxrate 700k -bufsize 1400k -crf 23 -qmin 10 -qmax 51 -vcodec libvpx -preset medium -async 100 -acodec libvorbis -ar 48000 -ab 96k -ac 2 -vol 256 -threads 4 -metadata maxBitrate="700k" -f matroska "%OUTPUT%" "%OUTPUT_FILE%"

POPD