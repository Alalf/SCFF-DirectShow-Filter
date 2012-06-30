# FFmpeg Download Script
#======================================================================

from sys import stderr

# 設定
kTmpDirectory = 'tmp\\ffmpeg'
k32bitTmpDirectory = kTmpDirectory + '\\x86'
k64bitTmpDirectory = kTmpDirectory + '\\amd64'
k32bitExtDirectory = '..\\ext\\ffmpeg\\x86'
k64bitExtDirectory = '..\\ext\\ffmpeg\\amd64'

kDownloads = {}
kDownloads['ffmpeg-latest-win32-shared.7z'] = 'http://ffmpeg.zeranoe.com/builds/win32/shared/ffmpeg-latest-win32-shared.7z'
kDownloads['ffmpeg-latest-win32-dev.7z'] = 'http://ffmpeg.zeranoe.com/builds/win32/dev/ffmpeg-latest-win32-dev.7z'
kDownloads['ffmpeg-latest-win64-shared.7z'] = 'http://ffmpeg.zeranoe.com/builds/win64/shared/ffmpeg-latest-win64-shared.7z'
kDownloads['ffmpeg-latest-win64-dev.7z'] = 'http://ffmpeg.zeranoe.com/builds/win64/dev/ffmpeg-latest-win64-dev.7z'

k7zipCommand = '7zr.exe x -o%s %s\\' % (kTmpDirectory, kTmpDirectory)

kPixdescHeaderPatchSrc = 'extern const AVPixFmtDescriptor av_pix_fmt_descriptors[];'
kPixdescHeaderPatchDst = 'extern __declspec(dllimport) const AVPixFmtDescriptor av_pix_fmt_descriptors[];'

#-----------------------------------------------------------------------

def MakeTmp():
    from os import makedirs
    from shutil import rmtree

    print >>stderr, 'make-tmp:'
    
    rmtree(kTmpDirectory, True)
    makedirs(kTmpDirectory)

#-----------------------------------------------------------------------

def DownloadFFmpeg():
    from urllib import urlretrieve

    print >>stderr, 'download-ffmpeg:'

    # ffmpegのアーカイブをダウンロード
    for k, v in kDownloads.items():
        print >>stderr, '\t[download] ' + k
        r = urlretrieve(v, '%s/%s' % (kTmpDirectory, k))

#-----------------------------------------------------------------------

def ExtractFFmpeg():
    from subprocess import call
    
    print >>stderr, 'extract-ffmpeg:'

    # アーカイブをすべて解凍する
    for k in kDownloads.keys():
        print >>stderr, '\t[extract] ' + k
        call(k7zipCommand + k)

#-----------------------------------------------------------------------

def ArrangeFFmpeg():
    from os import listdir
    from os import makedirs
    from shutil import rmtree
    from shutil import copyfile
    from subprocess import call

    print >>stderr, 'arrange-ffmpeg:'

    # ファイル名を格納するディレクトリ
    ffmpeg_dirs = {}
    ffmpeg_dirs['win32-dev'] = ''
    ffmpeg_dirs['win32-shared'] = ''
    ffmpeg_dirs['win64-dev'] = ''
    ffmpeg_dirs['win64-shared'] = ''

    # tmpディレクトリを見て解凍したディレクトリの名前を得る
    files = listdir(kTmpDirectory)
    for i in files:
        for k in ffmpeg_dirs.keys():
            if i.endswith(k):
                ffmpeg_dirs[k] = kTmpDirectory + '\\' + i

    # ディレクトリを生成する
    rmtree(k32bitTmpDirectory, True)
    rmtree(k64bitTmpDirectory, True)
    makedirs(k32bitTmpDirectory)
    makedirs(k64bitTmpDirectory)

    # Xcopyでファイルを上書きコピーする
    for k, v in ffmpeg_dirs.items():
        try:
            print >>stderr, '\t[copy-%s] START' % k
            if k.startswith('win32'):
                retcode = call('xcopy /C /Y /R /E %s %s' % (v, k32bitTmpDirectory))
            else:
                retcode = call('xcopy /C /Y /R /E %s %s' % (v, k64bitTmpDirectory))
            if retcode < 0:
                print >>stderr, '\t[copy-%s] FAILED!' % k, -retcode
                sys.exit()
            else:
                print >>stderr, '\t[copy-%s] SUCCESS!' % k, retcode
        except OSError, e:
            print >>stderr, '\t[copy-%s] Execution failed:' % k, e
            sys.exit()

    # scffのext/ffmpeg/*ディレクトリからdummy.txtをコピーしてくる
    copyfile(k32bitExtDirectory + '\\dummy.txt', k32bitTmpDirectory+'\\dummy.txt')
    copyfile(k64bitExtDirectory + '\\dummy.txt', k64bitTmpDirectory+'\\dummy.txt')

#-----------------------------------------------------------------------

def PatchPixdescHeader():
    from shutil import move
    
    print >>stderr, 'patch-pixdesc-header:'
    
    # オリジナルをコピーして保存しておく
    target_32bit = k32bitTmpDirectory+'\\include\\libavutil\\pixdesc.h'
    orig_32bit = k32bitTmpDirectory+'\\include\\libavutil\\pixdesc.h.orig'
    target_64bit = k64bitTmpDirectory+'\\include\\libavutil\\pixdesc.h'
    orig_64bit = k64bitTmpDirectory+'\\include\\libavutil\\pixdesc.h.orig'
 
    move(target_32bit, orig_32bit)
    move(target_64bit, orig_64bit)

    # ファイルを開いて修正箇所を変更
    # 改行コードが変わってしまうが、多分大丈夫だろう
    print >>stderr, '\t[add-dllimport] ', target_32bit
    with open(orig_32bit, 'r') as src:
        with open(target_32bit, 'w') as dst:
            for line in src:
                replaced = line.replace(kPixdescHeaderPatchSrc, kPixdescHeaderPatchDst)
                dst.write(replaced)
                
    print >>stderr, '\t[add-dllimport] ', target_64bit
    with open(orig_64bit, 'r') as src:
        with open(target_64bit, 'w') as dst:
            for line in src:
                replaced = line.replace(kPixdescHeaderPatchSrc, kPixdescHeaderPatchDst)
                dst.write(replaced)

#-----------------------------------------------------------------------

def MoveToExt():
    from shutil import move
    from shutil import rmtree
    
    print >>stderr, 'move-to-ext:'
    
    # extの元あったディレクトリを削除する
    rmtree(k32bitExtDirectory, ignore_errors=True)
    rmtree(k64bitExtDirectory, ignore_errors=True)

    # 移動
    move(k32bitTmpDirectory, k32bitExtDirectory)
    move(k64bitTmpDirectory, k64bitExtDirectory)

#-----------------------------------------------------------------------

def CopyFFmpegDLL():
    from subprocess import call
    
    print >>stderr, 'copy-ffmpeg-dll:'
    
    bat_string = '''
mkdir "..\\dist\\Debug-amd64\\"
mkdir "..\\dist\\Release-amd64\\"
mkdir "..\\dist\\Debug-x86\\"
mkdir "..\\dist\\Release-x86\\"

copy /y "..\\ext\\ffmpeg\\amd64\\bin\\avcodec*.dll" "..\\dist\\Debug-amd64\\"
copy /y "..\\ext\\ffmpeg\\amd64\\bin\\avutil*.dll" "..\\dist\\Debug-amd64\\"
copy /y "..\\ext\\ffmpeg\\amd64\\bin\\swscale*.dll" "..\\dist\\Debug-amd64\\"

copy /y "..\\ext\\ffmpeg\\amd64\\bin\\avcodec*.dll" "..\\dist\\Release-amd64\\"
copy /y "..\\ext\\ffmpeg\\amd64\\bin\\avutil*.dll" "..\\dist\\Release-amd64\\"
copy /y "..\\ext\\ffmpeg\\amd64\\bin\\swscale*.dll" "..\\dist\\Release-amd64\\"

copy /y "..\\ext\\ffmpeg\\x86\\bin\\avcodec*.dll" "..\\dist\\Debug-x86\\"
copy /y "..\\ext\\ffmpeg\\x86\\bin\\avutil*.dll" "..\\dist\\Debug-x86\\"
copy /y "..\\ext\\ffmpeg\\x86\\bin\\swscale*.dll" "..\\dist\\Debug-x86\\"

copy /y "..\\ext\\ffmpeg\\x86\\bin\\avcodec*.dll" "..\\dist\\Release-x86\\"
copy /y "..\\ext\\ffmpeg\\x86\\bin\\avutil*.dll" "..\\dist\\Release-x86\\"
copy /y "..\\ext\\ffmpeg\\x86\\bin\\swscale*.dll" "..\\dist\\Release-x86\\"
'''

    # ファイル出力
    with open('copy-ffmpeg-dll.bat', 'w') as f:
        f.write(bat_string)

    # 出力したファイルを実行
    call('copy-ffmpeg-dll.bat')

#-----------------------------------------------------------------------

# main()
if __name__=='__main__':
    print >>stderr, '=== FFmpeg Download Script ===\n'
    MakeTmp()
    DownloadFFmpeg()
    ExtractFFmpeg()
    ArrangeFFmpeg()
    PatchPixdescHeader()
    MoveToExt()
    CopyFFmpegDLL()
