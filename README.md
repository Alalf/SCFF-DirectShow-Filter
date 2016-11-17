SCFF DirectShow Filter
=======================================================================

- SCFF directShow FilterはWindows用スクリーンキャプチャプログラム(DirectShowフィルタ)です。
- ffmpegやWindows Media Encoderの映像入力として使われることを想定しています。
- リンク:
    - GitHub: https://github.com/Alalf/SCFF-DirectShow-Filter
    - Nightly-Build: https://sites.google.com/site/scffdirectshowfilter/


v0.3.0-alpha.3インストール前の注意事項
-----------------------------------------------------------------------

- v0.3.0-alpha.3(2013/09/12版)から.ax/.dllファイルの置き場所が変わりました。
    - 旧版のuninstall*.batを実行/全てのファイルを削除したあと、最新版をインストールしてください。


必要動作環境
-----------------------------------------------------------------------

- Windows XP SP3/Windows 7
- 画面の色数: 32bit True Color


インストール方法
-----------------------------------------------------------------------

1. 以下のランタイムをインストールしてください  
    - 共通: [Microsoft .NET Framework 4 Client Profile]
      (http://www.microsoft.com/downloads/ja-jp/details.aspx?FamilyID=e5ad0459-cbcc-4b4f-97b6-fb17111cf544)
    - 32bit/64bit OS: [Visual Studio 2015 の Visual C++ 再頒布可能パッケージ (vc_redist.x86.exe)]
      (http://www.microsoft.com/ja-jp/download/details.aspx?id=48145)
    - 64bit OS: [Visual Studio 2015 の Visual C++ 再頒布可能パッケージ (vc_redist.x64.exe)]
      (http://www.microsoft.com/ja-jp/download/details.aspx?id=48145)
2. install.batを実行してください
    - ***重要！***
      インストール後にscff_dsf_Win32/x64.axを移動させた場合は再度install.batを実行してください。
    - vcredist_x86/x64をインストールしていない場合、LoadLibrary("XXX") failed.というエラーが表示されます。
3. (install.batでインストールできない場合) 以下の手順で手動インストールしてください:
    - install_regsvr.batをエクスプローラ上で右クリック＞管理者として実行
    - もしくは管理者として実行したコマンドプロンプトからbatファイルを実行してください


使用方法
-----------------------------------------------------------------------

1. キャプチャソフトで「SCFF DirectShow Filter」を選択します。
2. ***重要！***
   SCFF DirectShow Filterは取り込みサイズとフレームレートは
   出力サイズに自動的に合わせられます。
   各種エンコーダで、まずは出力サイズ、フレームレートを設定してください。
3. エンコードを開始し、ロゴが中央に表示されているかプレビューなどで確認してください。
4. SCFF.GUI.exeを実行してください。
5. 左上のプロセスメニューから目的のプロセスを選択し、
   取り込み設定後、Applyボタンを押してください。
   (Applyボタン横のAutoチェックボックスを押すと、
    一部設定が変更後自動でApplyされます)
6. SCFF.GUI.exe起動中にコマンドプロンプトからSCFF.GUI.exeを操作可能です。
   - SCFF.GUI.exe (PROFILE_PATH) [[/p (PID)|/pa] [/s|/a]]
   - /p (PID): プロファイルの書き込み先のプロセスを選択
   - /pa: /sおよび/aオプションを全てのプロセスを対象にする。プロセスの選択は保持される
   - /s: スプラッシュ画面を表示（Splashボタンを同じ動作）
   - /a: プロファイルを適用(Applyボタンと同じ動作)
7. 後はいろいろ触って覚えてください。


バージョンアップ方法
-----------------------------------------------------------------------

1. エンコーダおよびGUIクライアント(SCFF.GUI.exe)が実行されていないことを確認して下さい。
2. 確認後、ファイルを上書きしてください。


アンインストール方法
-----------------------------------------------------------------------

1. エンコーダおよびGUIクライアント(SCFF.GUI.exe)が実行されていないことを確認して下さい。
2. uninstall.batを実行してください。
3. (install_regsvr.batでインストールした場合) 以下の手順で手動アンインストールしてください:
    - uninstall_regsvr.batをエクスプローラ上で右クリック＞管理者として実行
    - もしくは管理者として実行したコマンドプロンプトからbatファイルを実行してください
4. フォルダ・ファイルを削除してください。


注意
-----------------------------------------------------------------------

- ***重要！***
  取り込み時に問題が発生すると、強制的にロゴが表示されます。
  たいていの場合はGUIクライアントで設定を変えてApplyしなおすと直ります。
- 現在判明している問題として、大きな取り込み領域(1920x1050)を
  32x32程度まで小さく縮小しようとすると取り込みに失敗します。
- 何度Applyしても直らない場合は、プロセスメニューをRefreshして
  エンコーダが起動しているかどうか確認してください。
- それでも直らない場合はSCFF DirectShow Filterに対応していない環境の可能性があります。
    - 以下のWebページの"Issue"に環境の詳細を書き込んでいただければ助かります。
    - https://github.com/Alalf/SCFF-DirectShow-Filter


開発者向け: ビルド+利用方法
-----------------------------------------------------------------------

0. Pythonに詳しい方はtools/build.pyを参照してください。下記の作業の一部を自動化できます。
1. [Zeranoe FFmpeg builds](http://ffmpeg.zeranoe.com/builds/) からSharedビルド及びDevビルドを取得する
    - [32bit Builds (Shared)]
      (https://ffmpeg.zeranoe.com/builds/win32/shared/ffmpeg-3.2-win32-shared.zip)
    - [32bit Builds (Dev)]
      (https://ffmpeg.zeranoe.com/builds/win32/dev/ffmpeg-3.2-win32-dev.zip)
    - [64bit Builds (Shared)]
      (https://ffmpeg.zeranoe.com/builds/win64/shared/ffmpeg-3.2-win64-shared.zip)
    - [64bit Builds (Dev)]
      (https://ffmpeg.zeranoe.com/builds/win64/dev/ffmpeg-3.2-win64-dev.zip)
    - ext/ffmpeg/x64に64bit版を、ext/ffmpeg/Win32に32bit版を展開する
    - SharedもDevも同じディレクトリに展開すること（数個のファイルが上書きされるが問題ない）
    - ext/ffmpeg/x64/README.txt, ext/ffmpeg/Win32/README.txtが存在するように確認すること
    - ソースコードは [GitHub: FFmpeg / FFmpeg](https://github.com/FFmpeg/FFmpeg) からダウンロード可能
2. Microsoft Ribbon for WPF October 2010をインストールする
    - [Microsoft Ribbon for WPF.msi]
      (http://www.microsoft.com/en-us/download/details.aspx?id=11877)
3. scff.slnソリューションを開き、全てのビルドが通ることを確認
    - Visual Studio Community 2015で確認済み
    - 必要ならばいくつかのプロジェクト設定を書き換えること
4. tools/copy_ffmpeg_dll.batを実行してbinディレクトリにffmpeg付属のdllをコピー
5. tools/install_debug.batかtools/install_release.batを実行
6. 各種エンコーダを起動しフィルタが認識されているかチェック
7. bin/Debugかbin/Releaseにあるヘルパーアプリケーションを起動し取り込み設定を行う
8. （scff_dsfのデバッグバージョンを利用する場合:）
    - プロジェクト設定からローカルWindowsデバッガーを選ぶ
    - コマンドにWME/KTE/FMEなどを選択すればデバッグ文字列などを見ることが出来る。


開発者向け: 「開発に参加したい！」
-----------------------------------------------------------------------

- 現在、SCFF DirectShow FilterはGitHub上で開発が進められています。
    - https://github.com/Alalf/SCFF-DirectShow-Filter
- パッチを作成したい場合やコードを追加したい場合、まずGoogle C++スタイルガイドを一読してください。
    - [Google C++ Style Guide (Revision 3.231)]
      (http://google-styleguide.googlecode.com/svn/trunk/cppguide.xml)
    - [Google C++スタイルガイド 日本語訳 (Revision 3.199)]
      (http://www.textdrop.net/google-styleguide-ja/cppguide.xml)
    - このガイドは単純に決め事ではなく、バグを減らすために役に立つテクニックもいくらか含まれているようです。
- scff_dsfにはdoxygenコメントをつけてあります
    - [Doxygen]
      (http://www.stack.nl/~dimitri/doxygen/index.html)
    - プログラムの全体的な構造を把握したい場合はぜひ利用してみてください。


各種エンコーダ対応情報
-----------------------------------------------------------------------

### エンコーダ共通
- YUV420P(I420)に加えて各種ピクセルフォーマット出力が利用可能です。
    - YUV420P: IYUV/YV12
    - RGB32
    - [暫定対応] YUV422: UYVY/YUY2
        - レイアウト機能、Keep Aspect Ratioなどの設定が使えません

### ffmpeg
- サンプル設定がtools/test_ffmpeg.batにあります。
- "real-time buffer XXX% full! frame dropped!"が表示された場合、フレームドロップが発生しています。
    - 出力先がストリーミングサイトの場合、混み合っている時間帯に起きやすいようです。
    - "-rtbufsize=100MB"オプションを追加すると改善される場合があります。
- -iオプションの前に-rオプションを指定してFPSを指定した場合、処理落ちなどでフレームドロップしてもタイムスタンプ(pts)が補正されず音ずれが発生します。
    - -rオプションの代わりにdshowデバイスのオプション(-f dshow -framerate)のみでFPSを指定してみてください。
    - 例: -f dshow -video_size 640x360 -framerate 24 -pixel_format yuv420p -i video="SCFF DirectShow Filter" ...

### xSplit
- xSplitと併用する場合、ffmpegの一部ライブラリが干渉することがあります。
    - SplitMediaLabs\XSplit\avutil-51.dllをSCFF付属のavutil-*.dllと置き換えるとよい、という報告がありました。

### Windows Media Encoder(WME)
- WMEのノンインターレース化処理の利用は非推奨です。
    - アマレコやpecatvのデインタレース機能を利用することをおすすめします。

### KoToEncoder(KTE)
- KoToEncoderのプレビュー機能をVideoRendererからVideo Mixing Renderer 9への切り替えを推奨します
    - Video Rendererを利用しているとSCFFの出力がRGB32(RGB0)に固定されます
    - Video Mixing Renderer 9が利用できない場合は、VMR-7を利用するかプレビュー機能を完全に停止してください
- KoToEncoderのプレビュー機能が停止する問題
    - 出力サイズを設定したあとKTEを再起動すればプレビューが表示されるようになります

### Expression Encoder(EE)
- 出力サイズを640x360から変更するにはプログラムの改変が必要です
    - 例: scff_dsf/base/constants.ccの{640, 360}を{960, 540}に変更
- ピクセルフォーマット出力をYUV422からYUV420Pに変更するには、ツール＞オプション＞互換性のリストから以下の項目***以外***を無効化してください:
    - i420 VFE デコーダ
    - iyuv VFW デコーダ

### WME/KTE/EE共通
- 最新のWindows OSやExpression Encoder付属のqasf.dll(DirectShow ASF Support)の方が画質が良いそうです(thx cavelis!)
    - WME/KTEのみを利用する場合でもExpression Encoderのインストールを推奨します

### アマレコ/PeCaTVなどのプレビュー機能付きキャプチャソフトとの併用
- キャプチャソフトの入力をRGB32(RGB0)に設定してください。
    - I420だとAVI Decompressorをプレビューに利用するため、パフォーマンスがかなり落ちるとの報告がありました。


関連するソフトウェア、ソースコードについて
-----------------------------------------------------------------------

- DirectShow base classes - efines class hierarchy for streams architecture.
    - Copyright (c) 1992-2001 Microsoft Corporation.  All rights reserved.
- ffmpegプロジェクト(http://ffmpeg.org)
    - 利用しているLGPLライブラリ:
        - libswscale:
          a library performing highly optimized image scaling and
          color space/pixel format conversion operations.
        - libavutil:
          a library containing functions for simplifying programming,
          including random number generators, data structures, mathematics routines,
          core multimedia utilities, and much more.
    - 利用しているLGPLライセンスのソースコード
        - libavutil/colorspace.h
        - libavfilter/drawutils.c
        - libavfilter/formats.c
        - libavfilter/drawutils.h
        - libavfilter/formats.h
        - libavfilter/all_channel_layouts.inc


注意
-----------------------------------------------------------------------
- SCFF DirectShow Filterは"フリーソフトウェア"です。
    - 作者は本ソフトウェアに関する一切の義務（サポート、恒久的アップデート）を持ちません。
    - また、本ソフトウェアの使用により生じた直接的、間接的損害に一切の責任を持ちません。
    - 本ソフトウェアの利用についてはLICENSE(LGPLv.3の詳細)も参照してください。


-----------------------------------------------------------------------

- https://github.com/Alalf/SCFF-DirectShow-Filter
- Copyright (C) 2012-2015 Alalf
