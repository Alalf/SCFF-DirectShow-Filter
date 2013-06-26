# build.py - SCFF-DirectShow-Filter Build Script
#======================================================================

# option
#OPTIONS = ['download_ffmpeg', 'msbuild', 'dist', 'upload']
OPTIONS = ['download_ffmpeg', 'msbuild', 'dist']
#OPTIONS = ['download_ffmpeg']

#----------------------------------------------------------------------

# common
from os.path import abspath
ROOT_DIR = abspath('..')
TMP_DIR = ROOT_DIR + '\\tools\\tmp'
BIN_DIR = ROOT_DIR + '\\tools\\bin'

#----------------------------------------------------------------------

def download_ffmpeg():
    from sys import stderr
    print >>stderr, '--- download_ffmpeg ---\n'

    from scripts import download_ffmpeg
    download_ffmpeg.TMP_DIR = TMP_DIR + '\\download_ffmpeg'
    download_ffmpeg.ROOT_DIR = ROOT_DIR

    download_ffmpeg.FFMPEG_32BIT_DIR = download_ffmpeg.TMP_DIR + '\\Win32'
    download_ffmpeg.FFMPEG_64BIT_DIR = download_ffmpeg.TMP_DIR + '\\x64'
    download_ffmpeg.EXT_FFMPEG_32BIT_DIR = ROOT_DIR + '\\ext\\ffmpeg\\Win32'
    download_ffmpeg.EXT_FFMPEG_64BIT_DIR = ROOT_DIR + '\\ext\\ffmpeg\\x64'

    download_ffmpeg.DOWNLOADS = [
        'http://ffmpeg.zeranoe.com/builds/win32/shared/ffmpeg-latest-win32-shared.7z',
        'http://ffmpeg.zeranoe.com/builds/win32/dev/ffmpeg-latest-win32-dev.7z',
        'http://ffmpeg.zeranoe.com/builds/win64/shared/ffmpeg-latest-win64-shared.7z',
        'http://ffmpeg.zeranoe.com/builds/win64/dev/ffmpeg-latest-win64-dev.7z']

    download_ffmpeg.EXTRACT_COMMAND = BIN_DIR + '\\7zr.exe'
    download_ffmpeg.EXTRACT_OPTIONS = 'x -o"%s"' % download_ffmpeg.TMP_DIR

    download_ffmpeg.init()
    download_ffmpeg.download()
    download_ffmpeg.extract()
    download_ffmpeg.relocate()
    download_ffmpeg.move_to_ext()
    download_ffmpeg.copy_dll()
    download_ffmpeg.make_tools_bat()

#----------------------------------------------------------------------

def msbuild():
    from sys import stderr
    print >>stderr, '--- msbuild ---\n'

    from scripts import msbuild
    msbuild.TMP_DIR = TMP_DIR + '\\msbuild'
    msbuild.BUILD_32BIT_BAT = msbuild.TMP_DIR + '\\build_Win32.bat'
    msbuild.BUILD_64BIT_BAT = msbuild.TMP_DIR + '\\build_x64.bat'

    msbuild.ENV_32BIT_BAT = 'D:\\Program Files\\MSVS2012\\VC\\vcvarsall.bat'
    msbuild.ENV_64BIT_BAT = 'D:\\Program Files\\MSVS2012\\VC\\vcvarsall.bat'
    msbuild.DSF_SLN = ROOT_DIR + '\\scff.sln'

    msbuild.init()
    msbuild.make_build_Win32_bat()
    msbuild.make_build_x64_bat()
    msbuild.build_Win32()
    msbuild.build_x64()

#----------------------------------------------------------------------

def dist():
    from sys import stderr
    print >>stderr, '--- dist ---\n'

    from scripts import dist
    dist.TMP_DIR = TMP_DIR + '\\dist'

    dist.BASENAME_32BIT_DIR = 'SCFF-DirectShow-Filter-Win32'
    dist.BASENAME_64BIT_DIR = 'SCFF-DirectShow-Filter-x64'
    dist.DIST_32BIT_DIR = dist.TMP_DIR + '\\' + dist.BASENAME_32BIT_DIR
    dist.DIST_64BIT_DIR = dist.TMP_DIR + '\\' + dist.BASENAME_64BIT_DIR

    dist.FILES_32BIT = [
        ROOT_DIR + '\\README.md',
        ROOT_DIR + '\\LICENSE',
        ROOT_DIR + '\\bin\\Release\\scff_app.exe',
        ROOT_DIR + '\\bin\\Release\\SCFF.GUI.exe',
        ROOT_DIR + '\\bin\\Release\\Microsoft.Windows.Shell.dll',
        ROOT_DIR + '\\bin\\Release\\SCFF.Common.dll',
        ROOT_DIR + '\\bin\\Release\\SCFF.Interprocess.dll',
        ROOT_DIR + '\\bin\\Release_Win32\\*.dll',
        ROOT_DIR + '\\bin\\Release_Win32\\*.ax',
        ROOT_DIR + '\\tools\\bin\\regsvrex32.exe',
        ROOT_DIR + '\\tools\\dist\\Microsoft .NET Framework 4 Client Profile.url',
        ROOT_DIR + '\\tools\\dist\\Visual C++ Redistributable for Visual Studio 2012 Update 3.url',
        ROOT_DIR + '\\tools\\dist\\install.bat',
        ROOT_DIR + '\\tools\\dist\\uninstall.bat',
        ROOT_DIR + '\\tools\\dist\\install_regsvr.bat',
        ROOT_DIR + '\\tools\\dist\\uninstall_regsvr.bat',
        ]
    dist.FILES_64BIT = [
        ROOT_DIR + '\\README.md',
        ROOT_DIR + '\\LICENSE',
        ROOT_DIR + '\\bin\\Release\\scff_app.exe',
        ROOT_DIR + '\\bin\\Release\\SCFF.GUI.exe',
        ROOT_DIR + '\\bin\\Release\\Microsoft.Windows.Shell.dll',
        ROOT_DIR + '\\bin\\Release\\SCFF.Common.dll',
        ROOT_DIR + '\\bin\\Release\\SCFF.Interprocess.dll',
        ROOT_DIR + '\\bin\\Release_x64\\*.dll',
        ROOT_DIR + '\\bin\\Release_x64\\*.ax',
        ROOT_DIR + '\\tools\\bin\\regsvrex64.exe',
        ROOT_DIR + '\\tools\\dist\\Microsoft .NET Framework 4 Client Profile.url',
        ROOT_DIR + '\\tools\\dist\\Visual C++ Redistributable for Visual Studio 2012 Update 3.url',
        ROOT_DIR + '\\tools\\dist\\install.bat',
        ROOT_DIR + '\\tools\\dist\\uninstall.bat',
        ROOT_DIR + '\\tools\\dist\\install_regsvr.bat',
        ROOT_DIR + '\\tools\\dist\\uninstall_regsvr.bat',
        ]
    dist.ARCHIVE_COMMAND = BIN_DIR + '\\7zr.exe'
    dist.ARCHIVE_OPTIONS = 'a'

    dist.init()
    dist.make_dist()
    dist.make_archive()

#----------------------------------------------------------------------

def upload():
    from sys import stderr
    print >>stderr, '--- upload ---\n'

    from scripts import upload
    upload.TMP_DIR = TMP_DIR + '\\upload'
    upload.ARCHIVES = TMP_DIR + '\\dist\\*.7z'
    upload.UPLOAD_COMMAND = 'curl.exe'
    upload.UPLOAD_OPTIONS = ''

    upload.AUTH = ('Alalf', raw_input('GitHub Password: '))
    upload.DOWNLOADS_URL = 'https://api.github.com/repos/Alalf/SCFF-DirectShow-Filter/downloads'

    upload.init()
    upload.upload()

#----------------------------------------------------------------------

# main()
if __name__=='__main__':
    from sys import stderr

    print >>stderr, '=== SCFF-DirectShow-Filter Build Script ===\n'

    # download_ffmpeg.py
    if 'download_ffmpeg' in OPTIONS:
        download_ffmpeg()

    # msbuild.py
    if 'msbuild' in OPTIONS:
        msbuild()

    # dist.py
    if 'dist' in OPTIONS:
        dist()

    # upload.py
    if 'upload' in OPTIONS:
        upload()