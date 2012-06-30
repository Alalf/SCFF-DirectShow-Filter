@echo off
FOR /R ..\scff-dsf %%i in (*.cc) do (
scripts\cpplint.py --filter=-whitespace/comments "%%i"
)
FOR /R ..\scff-dsf %%i in (*.h) do (
scripts\cpplint.py --filter=-whitespace/comments "%%i"
)
FOR /R ..\scff-tests %%i in (*.cc) do (
scripts\cpplint.py --filter=-whitespace/comments "%%i"
)
FOR /R ..\scff-tests %%i in (*.h) do (
scripts\cpplint.py --filter=-whitespace/comments "%%i"
)



