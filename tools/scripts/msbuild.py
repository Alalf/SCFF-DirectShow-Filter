# msbuild.py
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

    print('init:', file=stderr)

    rmtree(TMP_DIR, True)
    makedirs(TMP_DIR)

#----------------------------------------------------------------------

def make_build_Win32_bat():
    from sys import stderr

    print('make_build_Win32_bat:', file=stderr)

    # ビルドスクリプト生成
    build_script = '''@ECHO OFF
IF NOT DEFINED DevEnvDir CALL "%s" >NUL:
msbuild /verbosity:m /t:build /p:Configuration=Debug /p:Platform=Win32 "%s"
msbuild /verbosity:m /t:build /p:Configuration=Release /p:Platform=Win32 "%s"
''' % (ENV_32BIT_BAT, DSF_SLN, DSF_SLN)

    # ファイルに書き込み
    with open(BUILD_32BIT_BAT, 'w') as f:
        print('\t[write] ' + BUILD_32BIT_BAT, file=stderr)
        f.write(build_script)

#----------------------------------------------------------------------

def make_build_x64_bat():
    from sys import stderr

    print('make_build_x64_bat:', file=stderr)

    # ビルドスクリプト生成
    build_script = '''@ECHO OFF
IF NOT DEFINED DevEnvDir CALL "%s" >NUL:
msbuild /verbosity:m /t:build /p:Configuration=Debug /p:Platform=x64 "%s"
msbuild /verbosity:m /t:build /p:Configuration=Release /p:Platform=x64 "%s"
''' % (ENV_64BIT_BAT, DSF_SLN, DSF_SLN)

    # ファイルに書き込み
    with open(BUILD_64BIT_BAT, 'w') as f:
        print('\t[write] ' + BUILD_64BIT_BAT, file=stderr)
        f.write(build_script)

#----------------------------------------------------------------------

def build_Win32():
    from sys import stderr, exit
    from subprocess import call

    print('build_Win32:', file=stderr)

    # 32bit版ビルド
    try:
        print('\t[build_Win32] START', file=stderr)
        retcode = call(BUILD_32BIT_BAT)
        if retcode < 0:
            print('\t[build_Win32] FAILED! %d' % -retcode, file=stderr)
            exit()
        else:
            print('\t[build_Win32] SUCCESS! %d' % retcode, file=stderr)
    except OSError as e:
        print('\t[build_Win32] Execution failed: %s' % e.strerror, file=stderr)
        exit()

#----------------------------------------------------------------------

def build_x64():
    from sys import stderr, exit
    from subprocess import call

    print('build_x64:', file=stderr)

    # 64bit版ビルド
    try:
        print('\t[build_x64] START', file=stderr)
        retcode = call(BUILD_64BIT_BAT)
        if retcode < 0:
            print('\t[build_x64] FAILED! %d' % -retcode, file=stderr)
            exit()
        else:
            print('\t[build_x64] SUCCESS! %d' % retcode, file=stderr)
    except OSError as e:
        print('\t[build_x64] Execution failed: %s' % e.strerror, file=stderr)
        exit()

#----------------------------------------------------------------------
