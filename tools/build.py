# SCFF-DirectShow-Filter Build Script
#======================================================================

# option
OPTIONS = ['download_ffmpeg']
#OPTIONS = ['download_ffmpeg', 'msbuild']

#----------------------------------------------------------------------

# common
from os.path import abspath
ROOT_DIR = abspath('..')
TMP_DIR = ROOT_DIR + '\\tools\\tmp'
BIN_DIR = ROOT_DIR + '\\tools\\bin'

# dist.py
PACKAGE_32BIT_DIR = ROOT_DIR + '\\tools\\tmp\\package\\SCFF-DirectShow-Filter-x86'
PACKAGE_64BIT_DIR = ROOT_DIR + '\\tools\\tmp\\package\\SCFF-DirectShow-Filter-amd64'

#----------------------------------------------------------------------

def download_ffmpeg():
    from scripts import download_ffmpeg
    download_ffmpeg.TMP_DIR = TMP_DIR + '\\download_ffmpeg'
    download_ffmpeg.ROOT_DIR = ROOT_DIR

    download_ffmpeg.FFMPEG_32BIT_DIR = download_ffmpeg.TMP_DIR + '\\x86'
    download_ffmpeg.FFMPEG_64BIT_DIR = download_ffmpeg.TMP_DIR + '\\amd64'
    download_ffmpeg.EXT_FFMPEG_32BIT_DIR = ROOT_DIR + '\\ext\\ffmpeg\\x86'
    download_ffmpeg.EXT_FFMPEG_64BIT_DIR = ROOT_DIR + '\\ext\\ffmpeg\\amd64'

    download_ffmpeg.DOWNLOADS = [
        'http://ffmpeg.zeranoe.com/builds/win32/shared/ffmpeg-latest-win32-shared.7z',
        'http://ffmpeg.zeranoe.com/builds/win32/dev/ffmpeg-latest-win32-dev.7z',
        'http://ffmpeg.zeranoe.com/builds/win64/shared/ffmpeg-latest-win64-shared.7z',
        'http://ffmpeg.zeranoe.com/builds/win64/dev/ffmpeg-latest-win64-dev.7z']

    download_ffmpeg.EXTRACT_COMMAND = BIN_DIR + '\\7zr.exe'
    download_ffmpeg.EXTRACT_OPTIONS = 'x -o"%s"' % download_ffmpeg.TMP_DIR

    download_ffmpeg.PATCHES = [(
        'extern const AVPixFmtDescriptor av_pix_fmt_descriptors[];',
         'extern __declspec(dllimport) const AVPixFmtDescriptor av_pix_fmt_descriptors[];')]
    
    download_ffmpeg.init()
    download_ffmpeg.download()
    download_ffmpeg.extract()
    download_ffmpeg.relocate()
    download_ffmpeg.patch()
    download_ffmpeg.copy_dll()
    download_ffmpeg.make_tools_bat()

#----------------------------------------------------------------------

def msbuild():
    from sys import stderr
    from scripts import msbuild    
    msbuild.TMP_DIR = TMP_DIR + '\\msbuild'
    msbuild.BUILD_32BIT_BAT = msbuild.TMP_DIR + '\\build-x86.bat'
    msbuild.BUILD_64BIT_BAT = msbuild.TMP_DIR + '\\build-amd64.bat'

    msbuild.ENV_32BIT_BAT = 'D:\\Program Files\\MSVC2010\\VC\\bin\\vcvars32.bat'
    msbuild.ENV_64BIT_BAT = 'D:\\Program Files\\Microsoft SDKs\\Windows\\v7.1\\Bin\\SetEnv.cmd'
    msbuild.DSF_SLN = ROOT_DIR + '\\scff-dsf.sln'
    msbuild.APP_SLN = ROOT_DIR + '\\scff-app.sln'

    msbuild.init()
    msbuild.make_build_x86_bat()
    msbuild.make_build_amd64_bat()
    msbuild.build_x86()
    msbuild.build_amd64()

#----------------------------------------------------------------------

# main()
if __name__=='__main__':
    from sys import stderr
    from sys import exit
    
    print >>stderr, '=== SCFF-DirectShow-Filter Build Script ===\n'
    
    # download_ffmpeg.py
    if 'download_ffmpeg' in OPTIONS:
        download_ffmpeg()
    
    # msbuild.py
    if 'msbuild' in OPTIONS:
        msbuild()