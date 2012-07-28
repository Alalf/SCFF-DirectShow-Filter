@echo off
SET ROOT_DIR=%~dp0..\
PUSHD "%ROOT_DIR%"

SET OUTPUT_DIR=tools\tmp
SET OUTPUT=%OUTPUT_DIR%\test-ffmpeg.flv
SET VIDEO=SCFF DirectShow Filter
SET AUDIO=Mixer (Creative SB X-Fi)
SET FFMPEG_EXE=ext\ffmpeg\amd64\bin\ffmpeg.exe

MKDIR "%OUTPUT_DIR%"
DEL "%OUTPUT%"
"%FFMPEG_EXE%" -rtbufsize 100MB -r 30 -f dshow -s 640x360 -i video="%VIDEO%":audio="%AUDIO%" -r 30 -s 640x360 -vcodec libx264 -preset medium -x264opts b-adapt=2:direct=auto:keyint=300:me=umh:rc-lookahead=50:ref=6:subme=5 -maxrate 1200k -bufsize 2400k -crf 23 -qmin 10 -qmax 51 -vf fps="fps=30" -acodec libvo_aacenc -ar 44100 -ab 96k -ac 2 -threads 4 -f flv "%OUTPUT%"
"%OUTPUT%"

POPD