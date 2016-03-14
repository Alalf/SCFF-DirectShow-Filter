# download_ffmpeg.py
#======================================================================

# config

# TMP_DIR
# FFMPEG_32BIT_DIR
# FFMPEG_64BIT_DIR
# EXT_FFMPEG_32BIT_DIR
# EXT_FFMPEG_64BIT_DIR
# DOWNLOADS
# EXTRACT_COMMAND
# EXTRACT_OPTIONS
# DLLIMPORT_PATCH_SRC
# DLLIMPORT_PATCH_DST

#-----------------------------------------------------------------------

def init():
    from sys import stderr
    from os import makedirs
    from shutil import rmtree

    print >>stderr, 'init:'

    rmtree(TMP_DIR, True)
    makedirs(TMP_DIR)

#-----------------------------------------------------------------------

def download():
    from sys import stderr
    from urllib import urlretrieve

    print >>stderr, 'download:'

    # ffmpegのアーカイブをダウンロード
    for url in DOWNLOADS:
        print >>stderr, '\t[download] ' + url
        filename = url.split('/')[-1]
        path = TMP_DIR + '\\' + filename
        urlretrieve(url, path)

#-----------------------------------------------------------------------

def extract():
    from sys import stderr
    from subprocess import call

    print >>stderr, 'extract:'

    # アーカイブをすべて解凍する
    for url in DOWNLOADS:
        filename = url.split('/')[-1]
        path = TMP_DIR + '\\' + filename
        print >>stderr, '\t[extract] ' + path
        command = '"%s" %s "%s"' % (EXTRACT_COMMAND, EXTRACT_OPTIONS, path)
        call(command)

#-----------------------------------------------------------------------

def relocate():
    from sys import stderr
    from os import listdir
    from os import makedirs
    from shutil import copyfile
    from subprocess import call

    print >>stderr, 'relocate:'

    # ファイル名を格納するディレクトリ
    ffmpeg_dirs = {}
    ffmpeg_dirs['win32-dev'] = ''
    ffmpeg_dirs['win32-shared'] = ''
    ffmpeg_dirs['win64-dev'] = ''
    ffmpeg_dirs['win64-shared'] = ''

    # tmpディレクトリを見て解凍したディレクトリの名前を得る
    files = listdir(TMP_DIR)
    for i in files:
        for k in ffmpeg_dirs.keys():
            if i.endswith(k):
                ffmpeg_dirs[k] = TMP_DIR + '\\' + i

    # ディレクトリを生成する
    makedirs(FFMPEG_32BIT_DIR)
    makedirs(FFMPEG_64BIT_DIR)

    # Xcopyでファイルを上書きコピーする
    for k, v in ffmpeg_dirs.items():
        try:
            print >>stderr, '\t[copy_%s] START' % k
            if k.startswith('win32'):
                retcode = call('XCOPY /Q /C /Y /R /E "%s" "%s"' % (v, FFMPEG_32BIT_DIR))
            else:
                retcode = call('XCOPY /Q /C /Y /R /E "%s" "%s"' % (v, FFMPEG_64BIT_DIR))
            if retcode < 0:
                print >>stderr, '\t[copy_%s] FAILED!' % k, -retcode
                sys.exit()
            else:
                print >>stderr, '\t[copy_%s] SUCCESS!' % k, retcode
        except OSError, e:
            print >>stderr, '\t[copy_%s] Execution failed:' % k, e
            sys.exit()

    # scffのext/ffmpeg/*ディレクトリからdummy.txtをコピーしてくる
    copyfile(EXT_FFMPEG_32BIT_DIR + '\\dummy.txt', FFMPEG_32BIT_DIR + '\\dummy.txt')
    copyfile(EXT_FFMPEG_64BIT_DIR + '\\dummy.txt', FFMPEG_64BIT_DIR + '\\dummy.txt')

#-----------------------------------------------------------------------

def move_to_ext():
    from sys import stderr
    from shutil import move
    from shutil import rmtree

    print >>stderr, 'move_to_ext:'

    # extの元あったディレクトリを削除する
    rmtree(EXT_FFMPEG_32BIT_DIR, True)
    rmtree(EXT_FFMPEG_64BIT_DIR, True)

    move(FFMPEG_32BIT_DIR, EXT_FFMPEG_32BIT_DIR)
    move(FFMPEG_64BIT_DIR, EXT_FFMPEG_64BIT_DIR)

#-----------------------------------------------------------------------

def copy_dll():
    from sys import stderr
    from subprocess import call

    print >>stderr, 'copy_dll:'

    bat_string = '''@ECHO OFF
SET ROOT_DIR=%s
PUSHD "%%ROOT_DIR%%"

MKDIR "bin\\Debug_x64\\"
MKDIR "bin\\Release_x64\\"
MKDIR "bin\\Debug_Win32\\"
MKDIR "bin\\Release_Win32\\"

COPY /Y "ext\\ffmpeg\\x64\\bin\\avutil*.dll" "bin\\Debug_x64\\"
COPY /Y "ext\\ffmpeg\\x64\\bin\\swscale*.dll" "bin\\Debug_x64\\"

COPY /Y "ext\\ffmpeg\\x64\\bin\\avutil*.dll" "bin\\Release_x64\\"
COPY /Y "ext\\ffmpeg\\x64\\bin\\swscale*.dll" "bin\\Release_x64\\"

COPY /Y "ext\\ffmpeg\\Win32\\bin\\avutil*.dll" "bin\\Debug_Win32\\"
COPY /Y "ext\\ffmpeg\\Win32\\bin\\swscale*.dll" "bin\\Debug_Win32\\"

COPY /Y "ext\\ffmpeg\\Win32\\bin\\avutil*.dll" "bin\\Release_Win32\\"
COPY /Y "ext\\ffmpeg\\Win32\\bin\\swscale*.dll" "bin\\Release_Win32\\"

POPD
''' % ROOT_DIR

    # ファイル出力
    bat = TMP_DIR + '\\copy_ffmpeg_dll.bat'
    with open(bat, 'w') as f:
        f.write(bat_string)

    # 出力したファイルを実行
    call(bat)

#-----------------------------------------------------------------------

def make_tools_bat():
    from sys import stderr

    print >>stderr, 'make_tools_bat:'

    src_bat = TMP_DIR + '\\copy_ffmpeg_dll.bat'
    dst_bat = ROOT_DIR + '\\tools\\copy_ffmpeg_dll.bat'
    with open(src_bat, 'r') as src:
        with open(dst_bat, 'w') as dst:
            for line in src:
                replaced = line.replace(ROOT_DIR, '%~dp0..\\')
                dst.write(replaced)

#-----------------------------------------------------------------------