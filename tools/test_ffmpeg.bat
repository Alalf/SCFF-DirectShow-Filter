@echo off
SET ROOT_DIR=%~dp0..\
PUSHD "%ROOT_DIR%"

SET OUTPUT_DIR=tools\tmp
SET OUTPUT=%OUTPUT_DIR%\test_ffmpeg.flv
SET VIDEO=SCFF DirectShow Filter
SET AUDIO=Mixer (Creative SB X-Fi)
SET FFMPEG_EXE=ext\ffmpeg\x64\bin\ffmpeg.exe

MKDIR "%OUTPUT_DIR%"
DEL "%OUTPUT%"

REM [TUNED]
"%FFMPEG_EXE%" -rtbufsize 100MB -r 30 -s 640x360 -f dshow -i video="%VIDEO%":audio="%AUDIO%" -vf fps="fps=30" -r 30 -s 640x360 -pix_fmt yuv420p -maxrate 1200k -bufsize 2400k -crf 23 -qmin 10 -qmax 51 -vcodec libx264 -preset medium -x264opts b-adapt=2:direct=auto:keyint=300:me=umh:rc-lookahead=50:ref=6:subme=5  -async 100 -acodec libvo_aacenc -ar 48000 -ab 96k -ac 2 -vol 256 -threads 4 -metadata maxBitrate="1200k" -f flv "%OUTPUT%"
"%OUTPUT%"

REM [SIMPLE]
REM "%FFMPEG_EXE%" -rtbufsize 100MB -r 30 -s 640x360 -f dshow -i video="%VIDEO%":audio="%AUDIO%" -vf fps="fps=30" -r 30 -s 640x360 -pix_fmt yuv420p -maxrate 700k -bufsize 1400k -crf 30 -vcodec libx264 -preset slow -profile:v main -async 100 -acodec libvo_aacenc -ar 48000 -ab 96k -ac 2 -vol 256 -threads 4 -metadata maxBitrate="700k" -f flv "%OUTPUT%"
REM "%OUTPUT%"

POPD