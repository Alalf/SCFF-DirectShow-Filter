@echo off
set ROOT_DIR=%~dp0..\
pushd "%ROOT_DIR%"
FOR /R "scff_dsf" %%i in (*.cc) do (
"tools\bin\cpplint.py" --filter=-whitespace/comments "%%i"
)
FOR /R "scff_dsf" %%i in (*.h) do (
"tools\bin\cpplint.py" --filter=-whitespace/comments "%%i"
)
FOR /R "scff_tests" %%i in (*.cc) do (
"tools\bin\cpplint.py" --filter=-whitespace/comments "%%i"
)
FOR /R "scff_tests" %%i in (*.h) do (
"tools\bin\cpplint.py" --filter=-whitespace/comments "%%i"
)
popd
PAUSE