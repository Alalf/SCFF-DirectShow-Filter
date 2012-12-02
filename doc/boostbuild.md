Boostについて
=======================================================================

現在のバージョンではマルチスレッドをサポートしていないが、将来的にはBoostのThreadを使うかもしれない。

Boost Build
-----------------------------------------------------------------------
    b2 toolset=msvc-10.0 --build-type=complete --build-dir=build\Win32 --stagedir=stage\Win32 address-model=32 -j 4
    b2 toolset=msvc-10.0 --build-type=complete --without-python --build-dir=build\x64 --stagedir=stage\x64 address-model=64 -j 4

How to check libs
-----------------------------------------------------------------------
    dumpbin /headers libboost_thread-vc100-mt-gd-1_45.lib | findstr machine