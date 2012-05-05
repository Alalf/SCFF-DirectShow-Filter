@echo off
FOR /R ..\scff-dsf %%i in (*.cc) do (
cpplint.py --filter=-whitespace/comments "%%i"
)
FOR /R ..\scff-dsf %%i in (*.h) do (
cpplint.py --filter=-whitespace/comments "%%i"
)
rem Aggry generated code must be ....
rem FOR /R ..\scff-app-net %%i in (*.cc) do (
rem cpplint.py --filter=-whitespace/comments "%%i"
rem )
rem FOR /R ..\scff-app-net %%i in (*.h) do (
rem cpplint.py --filter=-whitespace/comments "%%i"
rem )
FOR /R ..\scff-tests %%i in (*.cc) do (
cpplint.py --filter=-whitespace/comments "%%i"
)
FOR /R ..\scff-tests %%i in (*.h) do (
cpplint.py --filter=-whitespace/comments "%%i"
)



