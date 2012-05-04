@echo off
FOR /R ..\scff-dsf %%i in (*.cc) do (
cpplint.py --filter=-whitespace/comments "%%i"
)
FOR /R ..\scff-dsf %%i in (*.h) do (
cpplint.py --filter=-whitespace/comments "%%i"
)
FOR /R ..\scff-app-net %%i in (*.cc) do (
cpplint.py --filter=-whitespace/comments "%%i"
)
FOR /R ..\scff-app-net %%i in (*.h) do (
cpplint.py --filter=-whitespace/comments "%%i"
)
FOR /R ..\scff-tests %%i in (*.cc) do (
cpplint.py --filter=-whitespace/comments "%%i"
)
FOR /R ..\scff-tests %%i in (*.h) do (
cpplint.py --filter=-whitespace/comments "%%i"
)



