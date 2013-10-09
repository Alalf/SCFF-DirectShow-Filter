@ECHO OFF
SET ROOT_DIR=%~dp0\
PUSHD "%ROOT_DIR%"

MKDIR "tmp\XSplit"
COPY /Y "..\ext\XSplit\avcodec*.dll" "tmp\XSplit\"
COPY /Y "..\ext\XSplit\avutil*.dll" "tmp\XSplit\"
COPY /Y "..\ext\XSplit\swscale*.dll" "tmp\XSplit\"

PUSHD "tmp\XSplit"
CALL "%VS110COMNTOOLS%vsvars32.bat" >NUL:
FOR %%f IN ("*.dll") DO dumpbin /EXPORTS "%%f" > "%%~nf.txt"
FOR %%f IN ("*.txt") DO python "..\..\scripts\dllexports2def.py" %%f > "%%~nf.def"
FOR %%f IN ("*.def") DO lib /MACHINE:X86 /DEF:"%%f"
POPD

MKDIR "..\ext\XSplit\lib"
COPY /Y "tmp\XSplit\*.lib" "..\ext\XSplit\lib\"
COPY /Y "tmp\XSplit\*.exp" "..\ext\XSplit\lib\"

POPD
