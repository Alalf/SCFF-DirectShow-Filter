# SCFF-DirectShow-Filter Build Script
#======================================================================

from sys import stderr

# 設定
k32bitDSFBuildScript = 'build-scff-dsf-x86.bat'
k64bitDSFBuildScript = 'build-scff-dsf-amd64.bat'
kAPPBuildScript = 'build-scff-app.bat'

kTmpDirectory = 'tmp'
k32bitDirectory = '%s/SCFF-DirectShow-Filter-x86' % kTmpDirectory
k64bitDirectory = '%s/SCFF-DirectShow-Filter-amd64' % kTmpDirectory

#----------------------------------------------------------------------

def MakeBuildScript():
    print >>stderr, 'make-build-script:'
    
    # ビルドスクリプト生成
    build_script = {}
    build_script[k32bitDSFBuildScript] = '''
call "D:\\Program Files\\MSVC2010\\VC\\bin\\vcvars32.bat"
msbuild /t:build /p:Configuration=Debug /p:Platform=Win32 "..\\scff-dsf.sln"
msbuild /t:build /p:Configuration=Release /p:Platform=Win32 "..\\scff-dsf.sln"
'''
    build_script[k64bitDSFBuildScript] = '''
call "D:\\Program Files\\Microsoft SDKs\\Windows\\v7.1\\Bin\\SetEnv.cmd"
msbuild /t:build /p:Configuration=Debug /p:Platform=x64 "..\\scff-dsf.sln"
msbuild /t:build /p:Configuration=Release /p:Platform=x64 "..\\scff-dsf.sln"
'''
    build_script[kAPPBuildScript] = '''
call "D:\\Program Files\\MSVC2010\\VC\\bin\\vcvars32.bat"
msbuild /t:build /p:Configuration=Debug /p:Platform=x86 "..\\scff-app.sln"
msbuild /t:build /p:Configuration=Release /p:Platform=x86 "..\\scff-app.sln"
'''

    # ファイルに書き込み
    for k, v in build_script.items():
        with open(k, 'w') as f:
            print >>stderr, '\t[write] '+k
            f.write(v)

#----------------------------------------------------------------------

def BuildSCFF():
    from subprocess import call

    print >>stderr, 'build-scff:'
    
    # 32bit版dsfのビルド
    try:
        print >>stderr, '\t[build-32bit-dsf] START'
        retcode = call(k32bitDSFBuildScript)
        if retcode < 0:
            print >>stderr, '\t[build-32bit-dsf] FAILED!', -retcode
            sys.exit()
        else:
            print >>stderr, '\t[build-32bit-dsf] SUCCESS!', retcode
    except OSError, e:
        print >>stderr, '\t[build-32bit-dsf] Execution failed:', e
        sys.exit()

    # 64bit版dsfのビルド
    try:
        print >>stderr, '\t[build-64bit-dsf] START'
        retcode = call(k64bitDSFBuildScript)
        if retcode < 0:
            print >>stderr, '\t[build-64bit-dsf] FAILED!', -retcode
            sys.exit()
        else:
            print >>stderr, '\t[build-64bit-dsf] SUCCESS!', retcode
    except OSError, e:
        print >>stderr, '\t[build-64bit-dsf] Execution failed:', e
        sys.exit()

    # appのビルド
    try:
        print >>stderr, '\t[build-app] START'
        retcode = call(kAPPBuildScript)
        if retcode < 0:
            print >>stderr, '\t[build-app] FAILED!', -retcode
            sys.exit()
        else:
            print >>stderr, '\t[build-app] SUCCESS!', retcode
    except OSError, e:
        print >>stderr, '\t[build-app] Execution failed:', e
        sys.exit()

#----------------------------------------------------------------------

def MakePackage():
    from glob import iglob
    from shutil import rmtree
    from os import mkdir
    
    print >>stderr, 'make-package:'
    
    # テンポラリディレクトリを作成
    rmtree(kTmpDirectory, True)
    mkdir(kTmpDirectory)

    # ディレクトリを作成
    mkdir(k32bitDirectory)
    mkdir(k64bitDirectory)

#----------------------------------------------------------------------

# main()
if __name__=='__main__':
    print >>stderr, '=== SCFF-DirectShow-Filter Build Script ===\n'
    MakeBuildScript()
    BuildSCFF()
    MakePackage()

    
