
// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF DSF.
//
// SCFF DSF is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCFF DSF is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SCFF DSF.  If not, see <http://www.gnu.org/licenses/>.

/// @file scff-interprocess/interprocess.cs
/// @brief SCFFのプロセス間通信に関するクラス、定数、型の宣言
/// @warning To me: このファイルの中から別のファイルへのusingは禁止！

using System.Runtime.InteropServices;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Diagnostics;

/// @brief scff-interprocessモジュールのC#版(オリジナルはC++)
namespace scff_interprocess {

//=====================================================================
// SCFF Messaging Protocol v0 (by 2012/05/02 Alalf)
//
// [全体的な注意点]
// - Windows固有の型名はビットサイズが分かりにくいので
///  System.***で置き換える
//   - 対応表
//     - DWORD        = System.UInt32 32bit
//     - HWND(void*)  = System.UInt64 64bit
//       - SCFHから変更: 念のため32bitから64bitに
//     - bool         = System.Byte 8bit
// - 不動小数点数はdoubleに統一すること
//   - System.Double: 64bit
// - すべての構造体はPOD(Plain Old Data)であること
//   - 基本型、コンストラクタ、デストラクタ、仮想関数を持たない構造体のみ
//=====================================================================

/// @brief プロセス間通信を担当するクラス
class Interprocess {

  /// @brief 共有メモリ名: SCFFエントリを格納するディレクトリ
  const string kDirectoryName = "scff-v0-directory";

  /// @brief Directoryの保護用Mutex名
  const string kDirectoryMutexName = "mutex-scff-v0-directory";

  /// @brief 共有メモリ名の接頭辞: SCFFで使うメッセージを格納する
  const string kMessageNamePrefix = "scff-v0-message-";

  /// @brief Messageの保護用Mutex名の接頭辞
  const string kMessageMutexNamePrefix = "mutex-scff-v0-message-";

  //-------------------------------------------------------------------

  /// @brief Directoryに格納されるEntryの最大の数
  public const int kMaxEntry = 8;

  /// @brief ComplexLayout利用時の最大の要素数
  /// @sa imaging::kMaxProcessorSize
  public const int kMaxComplexLayoutElements = 8;

  //-------------------------------------------------------------------

  /// @brief レイアウトの種類
  public enum LayoutType {
    /// @brief 何も表示しない
    kNullLayout = 0,
    /// @brief 取り込み範囲1個で、境界は出力に強制的に合わせられる
    kNativeLayout,
    /// @brief 取り込み範囲が複数ある
    kComplexLayout
  }

  //-------------------------------------------------------------------

  /// @brief ピクセルフォーマットの種類
  /// @sa scff-imaging/imaging-types.h
  /// @sa scff_imaging::ImagePixelFormat
  public enum ImagePixelFormat {
    /// @brief 不正なピクセルフォーマット
    kInvalidPixelFormat = -1,
    /// @brief I420(12bit)
    kI420 = 0,
    /// @brief UYVY(16bit)
    kUYVY,
    /// @brief RGB0(32bit)
    kRGB0,
    /// @brief 対応ピクセルフォーマット数
    kSupportedPixelFormatsCount
  }

  //-------------------------------------------------------------------

  /// @brief 拡大縮小メソッドをあらわす定数
  /// @sa scff-imaging/imaging-types.h
  /// @sa scff_imaging::SWScaleFlags
  public enum SWScaleFlags {
    /// @brief fast bilinear
    kFastBilinear = 1,
    /// @brief bilinear
    kBilinear = 2,
    /// @brief bicubic
    kBicubic = 4,
    /// @brief experimental
    kX = 8,
    /// @brief nearest neighbor
    kPoint = 0x10,
    /// @brief averaging area
    kArea = 0x20,
    /// @brief luma bicubic, chroma bilinear
    kBicublin = 0x40,
    /// @brief gaussian
    kGauss = 0x80,
    /// @brief sinc
    kSinc = 0x100,
    /// @brief natural
    kLanczos = 0x200,
    /// @brief natural bicubic spline
    kSpline = 0x400
  }

  /// @brief 回転方向を表す定数
  /// @sa scff-imaging/imaging-types.h
  /// @sa scff_imaging::RotateDirection
  enum RotateDirection {
    /// @brief 回転なし
    kNoRotate = 0,
    /// @brief 時計回り90度
    k90Degrees,
    /// @brief 時計回り180度
    k180Degrees,
    /// @brief 時計回り270度
    k270Degrees
  }

  //-------------------------------------------------------------------

  // アラインメントをコンパイラに変えられないように
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  /// @brief 共有メモリ(Directory)に格納する構造体のエントリ
  public struct Entry {
    /// @brief SCFF DSFのDLLが使われれているプロセスID
    public System.UInt32 process_id;
    /// @brief SCFF DSFのDLLが使われているプロセス名
    /// @warning 長さが260バイトに制限されているので注意！
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string process_name;
    /// @brief サンプルの出力width
    public System.Int32 sample_width;
    /// @brief サンプルの出力height
    public System.Int32 sample_height;
    /// @brief サンプルの出力ピクセルフォーマット
    /// @attention ImagePixelFormatを操作に使うこと
    public System.Int32 sample_pixel_format;
    /// @brief 目標fps
    public System.Double fps;
  }

  /// @brief 共有メモリ(Directory)に格納する構造体
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct Directory {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst=kMaxEntry)]
    public Entry[] entries;
  }

  /// @brief レイアウトパラメータ
  /// @sa scff_imaging::ScreenCaptureParameter
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct LayoutParameter {
    /// @brief サンプル内の原点のX座標
    /// @warning NullLayout,NativeLayoutでは無視される
    public System.Int32 bound_x;
    /// @brief サンプル内の原点のY座標
    /// @warning NullLayout,NativeLayoutでは無視される
    public System.Int32 bound_y;
    /// @brief サンプル内の幅
    /// @warning NullLayout,NativeLayoutでは無視される
    public System.Int32 bound_width;
    /// @brief サンプル内の高さ
    /// @warning NullLayout,NativeLayoutでは無視される
    public System.Int32 bound_height;
    /// @brief キャプチャを行う対象となるウィンドウ
    public System.UInt64 window;
    /// @brief 取り込み範囲の開始X座標
    public System.Int32 clipping_x;
    /// @brief 取り込み範囲の開始y座標
    public System.Int32 clipping_y;
    /// @brief 取り込み範囲の幅
    public System.Int32 clipping_width;
    /// @brief 取り込み範囲の高さ
    public System.Int32 clipping_height;
    /// @brief マウスカーソルの表示
    public System.Byte show_cursor;
    /// @brief レイヤードウィンドウの表示
    public System.Byte show_layered_window;
    /// @brief 拡大縮小アルゴリズムの選択
    /// @attention SWScaleFlagsを操作に使うこと
    public System.Int32 sws_flags;
    /// @brief 取り込み範囲が出力サイズより小さい場合拡張
    public System.Byte stretch;
    /// @brief アスペクト比の保持
    public System.Byte keep_aspect_ratio;
    /// @brief 回転方向
    /// @attention RotateDirectionを操作に使うこと
    public System.Int32 rotate_direction;
  }

  /// @brief 共有メモリ(Message)に格納する構造体
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct Message {
    /// @brief タイムスタンプ(time()で求められたものを想定)
    /// @warning 1ではじまり単調増加が必須条件
    /// @warning (0および負数は無効なメッセージを示す)
    public System.Int64 timestamp;
    /// @brief レイアウトの種類
    /// @attention LayoutTypeを操作に使うこと
    public System.Int32 layout_type;
    /// @brief 有効なレイアウト要素の数
    public System.Int32 layout_element_count;
    /// @brief レイアウトパラメータの配列
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = kMaxComplexLayoutElements)] 
    public LayoutParameter[] layout_parameters;
  }

  /// @brief Directoryのサイズ
  int size_of_directory_;
  /// @brief Messageのサイズ
  int size_of_message_;

  //-------------------------------------------------------------------

  /// @brief コンストラクタ
  public Interprocess() {
    directory_ = null;
    view_of_directory_ = null;
    mutex_directory_ = null;
    message_ = null;
    view_of_message_ = null;
    mutex_message_ = null;

    size_of_directory_ = Marshal.SizeOf(typeof(Directory));
    size_of_message_ = Marshal.SizeOf(typeof(Message));

    Trace.WriteLine("****Interprocess: NEW");
  }
  /// @brief デストラクタ
  ~Interprocess() {
    // 解放忘れがないように
    ReleaseDirectory();
    ReleaseMessage();
    Trace.WriteLine("****Interprocess: DELETE");
  }

  /// @brief Directory初期化
  /// @todo(me) 全体的に例外処理を考慮していないコード。要注意。
  public bool InitDirectory() {
    // 念のため解放
    ReleaseDirectory();

    // 仮想メモリ(Directory)の作成
    /// @todo(me) あまりにも邪悪な例外処理の使い方。なんとかしたい。
    MemoryMappedFile tmp_directory;
    bool already_exists = true;
    try {
      tmp_directory = MemoryMappedFile.OpenExisting(kDirectoryName);
    } catch (FileNotFoundException) {
      tmp_directory =
          MemoryMappedFile.CreateNew(kDirectoryName, size_of_directory_);
      already_exists = false;
    }

    // ビューの作成
    MemoryMappedViewStream tmp_view_of_directory =
        tmp_directory.CreateViewStream();

    // 最初に共有メモリを作成した場合は0クリアしておく
    if (!already_exists) {
      for (int i = 0; i < size_of_directory_; i++) {
        tmp_view_of_directory.Seek(i, SeekOrigin.Begin);
        tmp_view_of_directory.WriteByte(0);
      }
    }

    // Mutexの作成
    Mutex tmp_mutex_directory = new Mutex(false, kDirectoryMutexName);

    // メンバ変数に設定
    directory_ = tmp_directory;
    view_of_directory_ = tmp_view_of_directory;
    mutex_directory_ = tmp_mutex_directory;

    Trace.WriteLine("****Interprocess: InitDirectory Done");
    return true;
  }
  /// @brief Directoryの初期化が成功したか
  public bool IsDirectoryInitialized() {
    return directory_ != null &&
           view_of_directory_ != null &&
           mutex_directory_ != null;
  }

  /// @brief Message初期化
  public bool InitMessage(System.UInt32 process_id) {
    // 念のため解放
    ReleaseMessage();

    // 仮想メモリの名前
    string message_name = kMessageNamePrefix + process_id;

    // 仮想メモリ(Message<process_id>)の作成
    MemoryMappedFile tmp_message;
    bool already_exists = true;
    try {
      tmp_message = MemoryMappedFile.OpenExisting(message_name);
    } catch (FileNotFoundException) {
      tmp_message =
          MemoryMappedFile.CreateNew(message_name, size_of_message_);
      already_exists = false;
    }

    // ビューの作成
    MemoryMappedViewStream tmp_view_of_message =
        tmp_message.CreateViewStream();

    // 最初に共有メモリを作成した場合は0クリアしておく
    if (!already_exists) {
      for (int i = 0; i < size_of_message_; i++) {
        tmp_view_of_message.Seek(i, SeekOrigin.Begin);
        tmp_view_of_message.WriteByte(0);
      }
    }

    // Mutexの名前
    string message_mutex_name = kMessageMutexNamePrefix + process_id;

    // Mutexの作成
    Mutex tmp_mutex_message = new Mutex(false, message_mutex_name);

    // メンバ変数に設定
    message_ = tmp_message;
    view_of_message_ = tmp_view_of_message;
    mutex_message_ = tmp_mutex_message;

    Trace.WriteLine("****Interprocess: InitMessage Done");
    return true;
  }
  /// @brief Messageの初期化が成功したか
  public bool IsMessageInitialized() {
    return message_ != null &&
           view_of_message_ != null &&
           mutex_message_ != null;
  }

  //-------------------------------------------------------------------
  // for SCFF DirectShow Filter
  //-------------------------------------------------------------------
  /// @brief エントリを作成する
  public bool AddEntry(Entry entry) {
    // 初期化されていなければ失敗
    if (!IsDirectoryInitialized()) {
      Trace.WriteLine("****Interprocess: AddEntry FAILED");
      return false;
    }

    Trace.WriteLine("****Interprocess: AddEntry");

    // ロック取得
    mutex_directory_.WaitOne();
    
    // コピー取得
    Directory directory = ReadDirectory();

    bool success = false;
    for (int i = 0; i < kMaxEntry; i++) {
      /// @warning 重複する場合がある？
      if (directory.entries[i].process_id == 0) {
        // PODなのでコピー可能（のはず）
        directory.entries[i] = entry;
        success = true;
        break;
      }
    }

    // 変更を適用
    WriteDirectory(directory);

    // ロック解放
    mutex_directory_.ReleaseMutex();

    if (success) {
      Trace.WriteLine("****Interprocess: AddEntry SUCCESS");
    }

    return success;
  }
  /// @brief エントリを削除する
  public bool RemoveEntry(System.UInt32 process_id) {
    // 初期化されていなければ失敗
    if (!IsDirectoryInitialized()) {
      Trace.WriteLine("****Interprocess: RemoveEntry FAILED");
      return false;
    }

    Trace.WriteLine("****Interprocess: RemoveEntry");

    // ロック取得
    mutex_directory_.WaitOne();

    // コピー取得
    Directory directory = ReadDirectory();

    for (int i = 0; i < kMaxEntry; i++) {
      if (directory.entries[i].process_id == process_id) {
        // zero clear
        directory.entries[i].process_id = 0;
        directory.entries[i].process_name = "";
        directory.entries[i].sample_pixel_format = 0;
        directory.entries[i].sample_width = 0;
        directory.entries[i].sample_height = 0;
        directory.entries[i].fps = 0.0;
        Trace.WriteLine("****Interprocess: RemoveEntry SUCCESS");
        break;
      }
    }

    // 変更を適用
    WriteDirectory(directory);

    // ロック解放
    mutex_directory_.ReleaseMutex();

    return true;
  }
  /// @brief メッセージを受け取る
  /// @pre 事前にInitMessageが実行されている必要がある
  public bool ReceiveMessage(out Message message) {
    // 初期化されていなければ失敗
    if (!IsMessageInitialized()) {
      Trace.WriteLine("****Interprocess: ReceiveMessage FAILED");
      message = new Message();
      return false;
    }

    //Trace.WriteLine("****Interprocess: ReceiveMessage");

    // ロック取得
    mutex_message_.WaitOne();

    // そのままコピー
    message = ReadMessage();

    // ロック解放
    mutex_message_.ReleaseMutex();

    return true;
  }
  //-------------------------------------------------------------------

  /// @brief ディレクトリを取得する
  public bool GetDirectory(out Directory directory) {
    // 初期化されていなければ失敗
    if (!IsDirectoryInitialized()) {
      Trace.WriteLine("****Interprocess: GetDirectory FAILED");
      directory = new Directory();
      return false;
    }

    Trace.WriteLine("****Interprocess: GetDirectory");

    // ロック取得
    mutex_directory_.WaitOne();

    // そのままコピー
    directory = ReadDirectory();

    // ロック解放
    mutex_directory_.ReleaseMutex();

    return true;
  }
  /// @brief メッセージを作成する
  /// @pre 事前にInitMessageが実行されている必要がある
  public bool SendMessage(Message message) {
    // 初期化されていなければ失敗
    if (!IsMessageInitialized()) {
      Trace.WriteLine("****Interprocess: SendMessage FAILED");
      return false;
    }

    Trace.WriteLine("****Interprocess: SendMessage");

    // ロック取得
    mutex_message_.WaitOne();

    // そのままコピー
    WriteMessage(message);

    // ロック解放
    mutex_message_.ReleaseMutex();

    return true;
  }

  //-------------------------------------------------------------------
  // Private
  //-------------------------------------------------------------------

  /// @brief Directory解放
  private void ReleaseDirectory() {
    if (mutex_directory_ != null) {
      mutex_directory_.Dispose();
      mutex_directory_ = null;
    }
    if (view_of_directory_ != null) {
      view_of_directory_.Dispose();
      view_of_directory_ = null;
    }
    if (directory_ != null) {
      directory_.Dispose();
      directory_ = null;
    }
  }
  /// @brief Message解放
  private void ReleaseMessage() {
    if (mutex_message_ != null) {
      mutex_message_.Dispose();
      mutex_message_ = null;
    }
    if (view_of_message_ != null) {
      view_of_message_.Dispose();
      view_of_message_ = null;
    }
    if (message_ != null) {
      message_.Dispose();
      message_ = null;
    }
  }
  /// @brief Messageを読み込む
  private Message ReadMessage() {
    byte[] buffer = new byte[size_of_message_];
    view_of_message_.Read(buffer, 0, size_of_message_);
    GCHandle gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
    return (Message)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(Message));
  }
  /// @brief Messageを書き込む
  private void WriteMessage(Message message) {
    byte[] buffer = new byte[size_of_message_];
    System.IntPtr ptr = Marshal.AllocHGlobal(size_of_message_);
    Marshal.StructureToPtr(message, ptr, false);
    Marshal.Copy(ptr, buffer, 0, size_of_message_);
    Marshal.FreeHGlobal(ptr);
    view_of_message_.Write(buffer, 0, size_of_message_);
  }
  /// @brief Directoryを読み込む
  private Directory ReadDirectory() {
    byte[] buffer = new byte[size_of_directory_];
    view_of_directory_.Read(buffer, 0, size_of_directory_);
    GCHandle gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
    return (Directory)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(Directory));
  }
  /// @brief Directoryを書き込む
  private void WriteDirectory(Directory directory) {
    byte[] buffer = new byte[size_of_directory_];
    System.IntPtr ptr = Marshal.AllocHGlobal(size_of_directory_);
    Marshal.StructureToPtr(directory, ptr, false);
    Marshal.Copy(ptr, buffer, 0, size_of_directory_);
    Marshal.FreeHGlobal(ptr);
    view_of_directory_.Write(buffer, 0, size_of_directory_);
  }

  /// @brief 共有メモリ: Directory
  MemoryMappedFile directory_;
  /// @brief ビュー: Directory
  MemoryMappedViewStream view_of_directory_;
  /// @brief Mutex: Directory
  Mutex mutex_directory_;

  /// @brief 共有メモリ: Message
  MemoryMappedFile message_;
  /// @brief ビュー: Message
  MemoryMappedViewStream view_of_message_;
  /// @brief Mutex: Message
  Mutex mutex_message_;
}
}
