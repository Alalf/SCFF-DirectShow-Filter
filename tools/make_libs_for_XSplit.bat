@echo off
IF NOT DEFINED DevEnvDir CALL "%VS110COMNTOOLS%vsvars32.bat" >NUL:

SET ROOT_DIR=%~dp0\
SET DLL2DEF_PY=%ROOT_DIR%scripts\dllexports2def.py

PUSHD "%ROOT_DIR%\..\ext\XSplit\"
FOR %%f IN ("*.dll") DO dumpbin /EXPORTS "%%f" > "%%~nf.tmp"
FOR %%f IN ("*.tmp") DO python "%DLL2DEF_PY%" %%f > "%%~nf.def"
FOR %%f IN ("*.def") DO lib /MACHINE:X86 /DEF:"%%f"
DEL "*.tmp"
POPD
