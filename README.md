# SCFF DirectShow Filter Ver 0.1.0 (2012/5/14)

SCFF directShow FilterはWindows用スクリーンキャプチャプログラム(DirectShowフィルタ)です。
ffmpegやWindows Media Encoderの映像入力として使われることを想定しています。



## 必要動作環境
* (開発中のため調査中です)
* Windows XP SP3
* 画面の色数: 32bit True Color

## 最適動作環境
* Windows 7
* CPU: Intel Sandy Bridge/Ivy Bridge
  * Lucid Virtu (MVP) I-ModeをONにした場合、最高のパフォーマンスを期待できます。



## インストール方法
1. 以下のランタイムをインストールしてください:
  * 共通:
    * http://www.microsoft.com/downloads/ja-jp/details.aspx?FamilyID=e5ad0459-cbcc-4b4f-97b6-fb17111cf544
  * 32bit OSの場合:
    * http://www.microsoft.com/downloads/ja-jp/details.aspx?familyid=c32f406a-f8fc-4164-b6eb-5328b8578f03
  * 64bit OSの場合:
    * http://www.microsoft.com/downloads/ja-jp/details.aspx?FamilyID=C68CCBB6-75EF-4C9D-A326-879EAB4FCDF8

2. install-*.batを実行してください
  * *******************************************************************
    * 重要！                                                          *
    *******************************************************************
    インストール後にscff-*.axを移動させた場合は再度install-*.batを実行してください。



## 使用方法
1. キャプチャソフトで「SCFF DirectShow Filter」を選択します。

2. *******************************************************************
   * 重要！                                                          *
   *******************************************************************
   SCFF DirectShow Filterは取り込みサイズとフレームレートは
   出力サイズに自動的に合わせられます。
   各種エンコーダで、まずは出力サイズ、フレームレートを設定してください。

3. プレビュー画面を確認し、ロゴが中央に表示されることを確認してください。

4. scff-app.exeを実行してください。

5. 左上のプロセスメニューから目的のプロセスを選択し、
   取り込み設定後、Applyボタンを押してください。
   (Applyボタン横のAutoチェックボックスを押すと、
    一部設定が変更後自動でApplyされます)

6. 後はいろいろ触って覚えてください。



## バージョンアップ方法
1. エンコーダおよびGUIクライアント(scff-app.exe)が実行されていないことを確認して下さい。

2. 確認後、ファイルを上書きしてください。



## アンインストール方法
1. エンコーダおよびGUIクライアント(scff-app.exe)が実行されていないことを確認して下さい。

2. uninstall-*.batを実行してください。

3. フォルダ・ファイルを削除してください。
   * レジストリは使用していませんので、これだけで完全にアンインストールが可能です。



## 注意
*  *******************************************************************
   * 重要！                                                          *
   *******************************************************************
   取り込み時に問題が発生すると、強制的にロゴが表示されます。
   たいていの場合はGUIクライアントで設定を変えてApplyしなおすと直ります。

* 現在判明している問題として、大きな取り込み領域(1920x1050)を
  32x32程度まで小さく縮小しようとすると取り込みに失敗します。

* 何度Applyしても直らない場合は、プロセスメニューをRefreshして
  エンコーダが起動しているかどうか確認してください。

* それでも直らない場合はSCFF DirectShow Filterに対応していない環境の可能性があります。
  * 以下のWebページの"Issue"に環境の詳細を書き込んでいただければ助かります。
  * https://github.com/Alalf/SCFF-DirectShow-Filter



## 開発者向け: ビルド+利用方法

1. http://ffmpeg.zeranoe.com/builds/ からSharedビルド及びDevビルドを取得する
   * ext/ffmpeg/amd64に64bit版を、ext/ffmpeg/x86に32bit版を展開する
   * SharedもDevも同じディレクトリに展開すること（数個のファイルが上書きされるが問題ない）
   * ext/ffmpeg/amd64/README.txt, ext/ffmpeg/x86/README.txtが存在するように確認すること
2. (重要！) ext/ffmpeg/amd64/include/libavutil/pixdesc.hおよび
   ext/ffmpeg/x86/include/libavutil/pixdesc.hの以下の部分:
> extern const AVPixFmtDescriptor av_pix_fmt_descriptors[];
   を
> extern __declspec(dllimport) const AVPixFmtDescriptor av_pix_fmt_descriptors[];
   のように書き換えてください。一応tools/pixdesc.patchも添付してあります。
3. scff-dsf.slnソリューションを開き、全てのビルドが通ることを確認
   * Microsoft Visual C++ 2010 Express Edition + Windows SDK 7.1で確認済み
   * 必要ならばいくつかのプロジェクト設定を書き換えること
4. tools/copy-binaries.batを実行してdistディレクトリにdllなどをコピー
5. tools/install-debug.batかtools/install-release.batを実行
6. 各種エンコーダを起動しフィルタが認識されているかチェック
7. scff-app.slnソリューションを開き、全てのビルドが通ることを確認
   * Microsoft Visual C# 2010 Express Editionで確認済み
8. dist/Debugかdist/Releaseにあるヘルパーアプリケーションを起動し取り込み設定を行う
9. （scff-dsfのデバッグバージョンを利用する場合:）
   * プロジェクト設定からローカルWindowsデバッガーを選ぶ
   * コマンドにWME/KTE/FMEなどを選択すればデバッグ文字列などを見ることが出来る。



## 開発者向け: 「開発に参加したい！」

* 現在、SCFF DirectShow FilterはGitHub上で開発が進められています。
  * https://github.com/Alalf/SCFF-DirectShow-Filter

* パッチを作成したい場合やコードを追加したい場合、まずGoogle C++スタイルガイドを一読してください。
  * Google C++スタイルガイド 日本語訳
    * http://www.textdrop.net/google-styleguide-ja/cppguide.xml
  * このガイドは単純に決め事ではなく、バグを減らすために役に立つテクニックもいくらか含まれているようです。

* scff-dsfにはdoxygenコメントをつけてあります
  * Doxygen(http://www.stack.nl/~dimitri/doxygen/index.html)
    * プログラムの全体的な構造を把握したい場合はぜひ利用してみてください。



## 各種エンコーダー対応情報

### Windows Media Encoder etc.
* YUV420P(I420)に加えてRGB 32(RGB0),YUV422(UYVY)フォーマット出力が利用可能です。
  * （暫定対応なのでYUV422(UYVY)はかなり機能が制限されます）
  *  WMEのプロパティ > 処理 > ビデオ > ピクセルの形式から「RGB 32」「UYVY」を選択することで利用できます。

### KoToEncoder(KTE)
* KTEでYUV420P出力を利用する場合、scff-dsf/base/constants.hの以下の部分：
> // #define FOR_KOTOENCODER
  を
> #define FOR_KOTOENCODER
  のように書き換えてください。
* KTEはRGB32出力をサポートしたフィルタを利用する場合、フィルタの出力をRGB32に固定するようです。
  * 書き換え量は増えますが、YUV422も利用可能です。
    * 詳しくはソリューション内検索で"FOR_KOTOENCODER"で検索してみてください。
* KoToEncoderのプレビュー機能はI420出力利用時にうまく動かないようです。
  * 出力サイズを設定したあとKTEを再起動すればプレビューが表示されるようになります。



## 関連するソフトウェア、ソースコードについて

* DirectShow base classes - efines class hierarchy for streams architecture.
  * Copyright (c) 1992-2001 Microsoft Corporation.  All rights reserved.
* ISO C9x  compliant inttypes.h for Microsoft Visual Studio
  * Copyright (c) 2006 Alexander Chemeris

* ffmpegプロジェクト(http://ffmpeg.org)
  * 利用しているLGPLライブラリ:
    * libavutil: a library containing functions for simplifying programming, including random number generators, data structures, mathematics routines, core multimedia utilities, and much more.
    * libavcodec: a library containing decoders and encoders for audio/video codecs.
    * libswscale: a library performing highly optimized image scaling and color space/pixel format conversion operations.
    * libavfilter: a library containing media filters.
    * libavformat: a library containing demuxers and muxers for multimedia container formats.
    * libswresample: a library performing highly optimized audio resampling, rematrixing and sample format conversion operations.
  * 利用しているLGPLライセンスのソースコード
    * libavutil/colorspace.h
    * libavfilter/drawutils.c
    * libavfilter/drawutils.h



## 注意
* SCFF DirectShow Filterは"フリーソフトウェア"です。
* 作者は本ソフトウェアに関する一切の義務（サポート、恒久的アップデート）を持ちません。
* また、本ソフトウェアの使用により生じた直接的、間接的損害に一切の責任を持ちません。
* 本ソフトウェアの利用についてはLICENSE(LGPLv.3の詳細)も参照してください。

https://github.com/Alalf/SCFF-DirectShow-Filter
Copyright (C) 2012 Alalf