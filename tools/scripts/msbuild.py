﻿# msbuild.py
#======================================================================

# config

# TMP_DIR
# ENV_32BIT_BAT
# ENV_64BIT_BAT
# DSF_SLN
# BUILD_32BIT_BAT
# BUILD_64BIT_BAT

#----------------------------------------------------------------------

def init():
    from sys import stderr
    from os import makedirs
    from shutil import rmtree

    print >>stderr, 'init:'

    rmtree(TMP_DIR, True)
    makedirs(TMP_DIR)

#----------------------------------------------------------------------

def make_build_Win32_bat():
    from sys import stderr

    print >>stderr, 'make_build_Win32_bat:'

    # ビルドスクリプト生成
    build_script = '''@ECHO OFF
IF NOT DEFINED DevEnvDir CALL "%s" >NUL:
msbuild /verbosity:m /t:build /p:Configuration=Debug /p:Platform=Win32 "%s"
msbuild /verbosity:m /t:build /p:Configuration=Release /p:Platform=Win32 "%s"
''' % (ENV_32BIT_BAT, DSF_SLN, DSF_SLN)

    # ファイルに書き込み
    with open(BUILD_32BIT_BAT, 'w') as f:
        print >>stderr, '\t[write] ' + BUILD_32BIT_BAT
        f.write(build_script)

#----------------------------------------------------------------------

def make_build_x64_bat():
    from sys import stderr

    print >>stderr, 'make_build_x64_bat:'

    # ビルドスクリプト生成
    build_script = '''@ECHO OFF
IF NOT DEFINED DevEnvDir CALL "%s" >NUL:
msbuild /verbosity:m /t:build /p:Configuration=Debug /p:Platform=x64 "%s"
msbuild /verbosity:m /t:build /p:Configuration=Release /p:Platform=x64 "%s"
''' % (ENV_64BIT_BAT, DSF_SLN, DSF_SLN)

    # ファイルに書き込み
    with open(BUILD_64BIT_BAT, 'w') as f:
        print >>stderr, '\t[write] ' + BUILD_64BIT_BAT
        f.write(build_script)

#----------------------------------------------------------------------

def build_Win32():
    from sys import stderr
    from sys import exit
    from subprocess import call

    print >>stderr, 'build_Win32:'

    # 32bit版ビルド
    try:
        print >>stderr, '\t[build_Win32] START'
        retcode = call(BUILD_32BIT_BAT)
        if retcode < 0:
            print >>stderr, '\t[build_Win32] FAILED!', -retcode
            exit()
        else:
            print >>stderr, '\t[build_Win32] SUCCESS!', retcode
    except OSError, e:
        print >>stderr, '\t[build_Win32] Execution failed:', e
        exit()

#----------------------------------------------------------------------

def build_x64():
    from sys import stderr
    from sys import exit
    from subprocess import call

    print >>stderr, 'build_x64:'

    # 64bit版ビルド
    try:
        print >>stderr, '\t[build_x64] START'
        retcode = call(BUILD_64BIT_BAT)
        if retcode < 0:
            print >>stderr, '\t[build_x64] FAILED!', -retcode
            exit()
        else:
            print >>stderr, '\t[build_x64] SUCCESS!', retcode
    except OSError, e:
        print >>stderr, '\t[build_x64] Execution failed:', e
        exit()

#----------------------------------------------------------------------
