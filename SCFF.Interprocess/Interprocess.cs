// Copyright 2012-2013 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter(SCFF DSF).
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

/// @file SCFF.Interprocess/Interprocess.cs
/// SCFFのプロセス間通信に関するクラスの宣言
/// @warning このファイルの中から別のファイルへのusingは禁止!
///          (別の言語に移植する場合も最大2ファイルでお願いします)

/// scff_interprocessモジュールの C# 版(オリジナルは C++ )
namespace SCFF.Interprocess {

using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;

// =====================================================================
/// @page smp SCFF Messaging Protocol v1 (by 2012/05/22 Alalf)
/// SCFF_DSFおよびそのクライアントで共有する共有メモリ内のデータ配置の仕様
///
/// ## C# 版の実装
/// - Windows固有の型名はビットサイズが分かりにくいのでSystem.***で置き換える
///   - 対応表
///     - DWORD        = UInt32 (32bit)
///     - HWND(void*)  = UInt64 (64bit)
///       - SCFHから変更: 念のため32bitから64bitに
///     - bool         = Byte (8bit)
/// - 不動小数点数は基本的にはdoubleで(floatも利用可)
///   - 対応表
///     - double       = Double (64bit)
///     - float        = Single (32bit)
/// - すべての構造体はPOD(Plain Old Data)であること
///   - 基本型、コンストラクタ、デストラクタ、仮想関数を持たない構造体のみ
// =====================================================================

/// レイアウトの種類
public enum LayoutTypes {
  NullLayout = 0,   ///< 何も表示しない
  NativeLayout,     ///< 取り込み範囲1個で、境界は出力に強制的に合わせられる
  ComplexLayout     ///< 取り込み範囲が複数ある
}

//-------------------------------------------------------------------

/// ピクセルフォーマットの種類
/// @sa scff_imaging/imaging_types.h
/// @sa scff_imaging::ImagePixelFormats
public enum ImagePixelFormats {
  InvalidPixelFormat = -1,    ///< 不正なピクセルフォーマット
  I420 = 0,                   ///< I420(12bit)
  IYUV,                       ///< IYUV(12bit)
  YV12,                       ///< YV12(12bit)
  UYVY,                       ///< UYVY(16bit)
  YUY2,                       ///< YUY2(16bit)
  RGB0,                       ///< RGB0(32bit)
  SupportedPixelFormatsCount  ///< 対応ピクセルフォーマット数
}

//-------------------------------------------------------------------

/// 拡大縮小メソッドをあらわす定数
/// @sa scff_imaging/imaging_types.h
/// @sa scff_imaging::SWScaleFlags
public enum SWScaleFlags {
  FastBilinear = 1, ///< fast bilinear
  Bilinear = 2,     ///< bilinear
  Bicubic = 4,      ///< bicubic
  X = 8,            ///< experimental
  Point = 0x10,     ///< nearest neighbor
  Area = 0x20,      ///< averaging area
  Bicublin = 0x40,  ///< luma bicubic, chroma bilinear
  Gauss = 0x80,     ///< gaussian
  Sinc = 0x100,     ///< sinc
  Lanczos = 0x200,  ///< lanczos
  Spline = 0x400    ///< natural bicubic spline
}

/// 回転方向を表す定数
/// @sa scff_imaging/imaging_types.h
/// @sa scff_imaging::RotateDirections
public enum RotateDirections {
  NoRotate = 0, ///< 回転なし
  Degrees90,    ///< 時計回り90度
  Degrees180,   ///< 時計回り180度
  Degrees270    ///< 時計回り270度
}

/// 共有メモリ(Directory)に格納する構造体のエントリ
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Entry {
  /// SCFF DSFのDLLが使われれているプロセスID
  internal UInt32 PrrocessID;
  /// SCFF DSFのDLLが使われているプロセス名
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Interprocess.MaxPath)]
  internal string ProcessName;
  /// サンプルの出力width
  internal Int32 SampleWidth;
  /// サンプルの出力height
  internal Int32 SampleHeight;
  /// サンプルの出力ピクセルフォーマット
  /// @attention ImagePixelFormatを操作に使うこと
  internal Int32 SamplePixelFormat;
  /// 目標fps
  internal Double FPS;
}

/// 共有メモリ(Directory)に格納する構造体
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Directory {
  /// エントリの配列
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = Interprocess.MaxEntry)]
  public Entry[] Entries;
}

/// 拡大縮小設定
/// @sa scff_imaging::SWScaleConfig
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SWScaleConfig {
  /// 拡大縮小メソッド(Chroma/Luma共通)
  /// @attention 操作にはSWScaleFlagsを使うこと！
  public Int32 Flags;
  /// 正確な丸め処理
  public Byte AccurateRnd;
  /// 変換前にフィルタをかけるか
  public Byte IsFilterEnabled;
  /// 輝度のガウスぼかし
  public Single LumaGblur;
  /// 色差のガウスぼかし
  public Single ChromaGblur;
  /// 輝度のシャープ化
  public Single LumaSharpen;
  /// 色差のシャープ化
  public Single ChromaSharpen;
  /// 水平方向のワープ
  public Single ChromaHshift;
  /// 垂直方向のワープ
  public Single ChromaVshift;
}

/// レイアウトパラメータ
/// @sa scff_imaging::ScreenCaptureParameter
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LayoutParameter {
  /// サンプル内の原点のX座標
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 BoundX;
  /// サンプル内の原点のY座標
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 BoundY;
  /// サンプル内の幅
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 BoundWidth;
  /// サンプル内の高さ
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 BoundHeight;
  /// キャプチャを行う対象となるWindow
  public UInt64 Window;
  /// 取り込み範囲の開始X座標
  public Int32 ClippingX;
  /// 取り込み範囲の開始y座標
  public Int32 ClippingY;
  /// 取り込み範囲の幅
  public Int32 ClippingWidth;
  /// 取り込み範囲の高さ
  public Int32 ClippingHeight;
  /// マウスカーソルの表示
  public Byte ShowCursor;
  /// レイヤードウィンドウの表示
  public Byte ShowLayeredWindow;
  /// 拡大縮小設定
  public SWScaleConfig SWScaleConfig;
  /// 取り込み範囲が出力サイズより小さい場合拡張
  public Byte Stretch;
  /// アスペクト比の保持
  public Byte KeepAspectRatio;
  /// 回転方向
  /// @attention RotateDirectionを操作に使うこと
  public Int32 RotateDirection;
}

/// 共有メモリ(Message)に格納する構造体
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Message {
  /// タイムスタンプ(time()で求められたものを想定)
  /// @warning 1ではじまり単調増加が必須条件
  /// @warning (0および負数は無効なメッセージを示す)
  public Int64 Timestamp;
  /// レイアウトの種類
  /// @attention LayoutTypesを操作に使うこと
  public Int32 LayoutType;
  /// 有効なレイアウト要素の数
  public Int32 LayoutElementCount;
  /// レイアウトパラメータの配列
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = Interprocess.MaxComplexLayoutElements)]
  public LayoutParameter[] LayoutParameters;
}

/// プロセス間通信を担当するクラス
public class Interprocess {
  //===================================================================
  // 定数
  //===================================================================

  /// Path文字列の長さ
  public const int MaxPath = 260;

  /// Directoryに格納されるEntryの最大の数
  public const int MaxEntry = 8;

  /// ComplexLayout利用時の最大の要素数
  /// @sa scff_imaging::kMaxProcessorSize
  public const int MaxComplexLayoutElements = 8;

  //-------------------------------------------------------------------

  /// 共有メモリ名: SCFFエントリを格納するディレクトリ
  private const string DirectoryName = "scff_v1_directory";

  /// Directoryの保護用Mutex名
  private const string DirectoryMutexName = "mutex_scff_v1_directory";

  /// 共有メモリ名の接頭辞: SCFFで使うメッセージを格納する
  private const string MessageNamePrefix = "scff_v1_message_";

  /// Messageの保護用Mutex名の接頭辞
  private const string MessageMutexNamePrefix = "mutex_scff_v1_message_";

  //===================================================================
  // コンストラクタ/デストラクタ
  //===================================================================

  /// コンストラクタ
  public Interprocess() {
    this.directory = null;
    this.viewOfDirectory = null;
    this.mutexDirectory = null;
    this.message = null;
    this.viewOfMessage = null;
    this.mutexMessage = null;

    this.sizeOfDirectory = Marshal.SizeOf(typeof(Directory));
    this.sizeOfMessage = Marshal.SizeOf(typeof(Message));

    Trace.WriteLine("****Interprocess: NEW");
  }

  /// デストラクタ
  ~Interprocess() {
    // 解放忘れがないように
    this.ReleaseDirectory();
    this.ReleaseMessage();
    Trace.WriteLine("****Interprocess: DELETE");
  }

  //===================================================================
  // Directory
  //===================================================================

  /// Directory初期化
  /// @todo(me) 全体的に例外処理を考慮していないコード。要注意。
  /// @return 初期化が成功したか
  public bool InitDirectory() {
    // 念のため解放
    this.ReleaseDirectory();

    // 仮想メモリ(Directory)の作成
    /// @todo(me) あまりにも邪悪な例外処理の使い方。なんとかしたい。
    MemoryMappedFile tmpDirectory;
    bool alreadyExists = true;
    try {
      tmpDirectory = MemoryMappedFile.OpenExisting(DirectoryName);
    } catch (FileNotFoundException) {
      tmpDirectory =
          MemoryMappedFile.CreateNew(DirectoryName, this.sizeOfDirectory);
      alreadyExists = false;
    }

    // ビューの作成
    MemoryMappedViewStream tmpViewOfDirectory =
        tmpDirectory.CreateViewStream();

    // 最初に共有メモリを作成した場合は0クリアしておく
    if (!alreadyExists) {
      for (int i = 0; i < this.sizeOfDirectory; i++) {
        tmpViewOfDirectory.Seek(i, SeekOrigin.Begin);
        tmpViewOfDirectory.WriteByte(0);
      }
    }

    // Mutexの作成
    Mutex tmpMutexDirectory = new Mutex(false, DirectoryMutexName);

    // フィールドに設定
    this.directory = tmpDirectory;
    this.viewOfDirectory = tmpViewOfDirectory;
    this.mutexDirectory = tmpMutexDirectory;

    Trace.WriteLine("****Interprocess: InitDirectory Done");
    return true;
  }

  /// Directoryの初期化が成功したか
  public bool IsDirectoryInitialized() {
    return this.directory != null &&
           this.viewOfDirectory != null &&
           this.mutexDirectory != null;
  }

  /// Directory解放
  private void ReleaseDirectory() {
    if (this.mutexDirectory != null) {
      this.mutexDirectory.Dispose();
      this.mutexDirectory = null;
    }
    if (this.viewOfDirectory != null) {
      this.viewOfDirectory.Dispose();
      this.viewOfDirectory = null;
    }
    if (this.directory != null) {
      this.directory.Dispose();
      this.directory = null;
    }
  }

  /// Directoryを読み込む
  private Directory ReadDirectory() {
    byte[] buffer = new byte[this.sizeOfDirectory];
    this.viewOfDirectory.Read(buffer, 0, this.sizeOfDirectory);
    GCHandle gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
    return (Directory)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(Directory));
  }

  /// Directoryを書き込む
  private void WriteDirectory(Directory directory) {
    byte[] buffer = new byte[this.sizeOfDirectory];
    IntPtr ptr = Marshal.AllocHGlobal(this.sizeOfDirectory);
    Marshal.StructureToPtr(directory, ptr, false);
    Marshal.Copy(ptr, buffer, 0, this.sizeOfDirectory);
    Marshal.FreeHGlobal(ptr);
    this.viewOfDirectory.Write(buffer, 0, this.sizeOfDirectory);
  }

  //===================================================================
  // Message
  //===================================================================

  /// Message初期化
  public bool InitMessage(UInt32 processID) {
    // 念のため解放
    this.ReleaseMessage();

    // 仮想メモリの名前
    string messageName = MessageNamePrefix + processID;

    // 仮想メモリ(Message<PrrocessID>)の作成
    MemoryMappedFile tmpMessage;
    bool alreadyExists = true;
    try {
      tmpMessage = MemoryMappedFile.OpenExisting(messageName);
    } catch (FileNotFoundException) {
      tmpMessage =
          MemoryMappedFile.CreateNew(messageName, this.sizeOfMessage);
      alreadyExists = false;
    }

    // ビューの作成
    MemoryMappedViewStream tmpViewOfMessage =
        tmpMessage.CreateViewStream();

    // 最初に共有メモリを作成した場合は0クリアしておく
    if (!alreadyExists) {
      for (int i = 0; i < this.sizeOfMessage; i++) {
        tmpViewOfMessage.Seek(i, SeekOrigin.Begin);
        tmpViewOfMessage.WriteByte(0);
      }
    }

    // Mutexの名前
    string messageMutexName = MessageMutexNamePrefix + processID;

    // Mutexの作成
    Mutex tmpMutexMessage = new Mutex(false, messageMutexName);

    // フィールドに設定
    this.message = tmpMessage;
    this.viewOfMessage = tmpViewOfMessage;
    this.mutexMessage = tmpMutexMessage;

    Trace.WriteLine("****Interprocess: InitMessage Done");
    return true;
  }

  /// Messageの初期化が成功したか
  public bool IsMessageInitialized() {
    return this.message != null &&
           this.viewOfMessage != null &&
           this.mutexMessage != null;
  }


  /// Message解放
  private void ReleaseMessage() {
    if (this.mutexMessage != null) {
      this.mutexMessage.Dispose();
      this.mutexMessage = null;
    }
    if (this.viewOfMessage != null) {
      this.viewOfMessage.Dispose();
      this.viewOfMessage = null;
    }
    if (this.message != null) {
      this.message.Dispose();
      this.message = null;
    }
  }

  /// Messageを読み込む
  private Message ReadMessage() {
    byte[] buffer = new byte[this.sizeOfMessage];
    this.viewOfMessage.Read(buffer, 0, this.sizeOfMessage);
    GCHandle gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
    return (Message)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(Message));
  }

  /// Messageを書き込む
  private void WriteMessage(Message message) {
    byte[] buffer = new byte[this.sizeOfMessage];
    IntPtr ptr = Marshal.AllocHGlobal(this.sizeOfMessage);
    Marshal.StructureToPtr(message, ptr, false);
    Marshal.Copy(ptr, buffer, 0, this.sizeOfMessage);
    Marshal.FreeHGlobal(ptr);
    this.viewOfMessage.Write(buffer, 0, this.sizeOfMessage);
  }

  //===================================================================
  // for SCFF DirectShow Filter
  //===================================================================

  /// エントリを追加する
  /// @param entry 追加されるエントリ
  /// @return 追加が成功したか
  public bool AddEntry(Entry entry) {
    // 初期化されていなければ失敗
    if (!this.IsDirectoryInitialized()) {
      Trace.WriteLine("****Interprocess: AddEntry FAILED");
      return false;
    }

    Trace.WriteLine("****Interprocess: AddEntry");

    // ロック取得
    this.mutexDirectory.WaitOne();

    // コピー取得
    Directory directory = this.ReadDirectory();

    bool success = false;
    for (int i = 0; i < MaxEntry; i++) {
      /// @warning 重複する場合がある？
      if (directory.Entries[i].PrrocessID == 0) {
        // PODなのでコピー可能（のはず）
        directory.Entries[i] = entry;
        success = true;
        break;
      }
    }

    // 変更を適用
    this.WriteDirectory(directory);

    // ロック解放
    this.mutexDirectory.ReleaseMutex();

    if (success) {
      Trace.WriteLine("****Interprocess: AddEntry SUCCESS");
    }

    return success;
  }

  /// エントリを削除する
  public bool RemoveEntry(UInt32 processID) {
    // 初期化されていなければ失敗
    if (!this.IsDirectoryInitialized()) {
      Trace.WriteLine("****Interprocess: RemoveEntry FAILED");
      return false;
    }

    Trace.WriteLine("****Interprocess: RemoveEntry");

    // ロック取得
    this.mutexDirectory.WaitOne();

    // コピー取得
    Directory directory = this.ReadDirectory();

    for (int i = 0; i < MaxEntry; i++) {
      if (directory.Entries[i].PrrocessID == processID) {
        // zero clear
        directory.Entries[i].PrrocessID = 0;
        directory.Entries[i].ProcessName = String.Empty;
        directory.Entries[i].SamplePixelFormat = 0;
        directory.Entries[i].SampleWidth = 0;
        directory.Entries[i].SampleHeight = 0;
        directory.Entries[i].FPS = 0.0;
        Trace.WriteLine("****Interprocess: RemoveEntry SUCCESS");
        break;
      }
    }

    // 変更を適用
    this.WriteDirectory(directory);

    // ロック解放
    this.mutexDirectory.ReleaseMutex();

    return true;
  }

  /// メッセージを受け取る
  /// @pre 事前にInitMessageが実行されている必要がある
  /// @param[out] message 受けとったメッセージ
  /// @return メッセージ受け取りに成功
  public bool ReceiveMessage(out Message message) {
    // 初期化されていなければ失敗
    if (!this.IsMessageInitialized()) {
      Trace.WriteLine("****Interprocess: ReceiveMessage FAILED");
      message = new Message();
      return false;
    }

    // Trace.WriteLine("****Interprocess: ReceiveMessage");

    // ロック取得
    this.mutexMessage.WaitOne();

    // そのままコピー
    message = this.ReadMessage();

    // ロック解放
    this.mutexMessage.ReleaseMutex();

    return true;
  }

  //===================================================================
  // for SCFF GUI Client
  //===================================================================
  /// ディレクトリを取得する
  public bool GetDirectory(out Directory directory) {
    // 初期化されていなければ失敗
    if (!this.IsDirectoryInitialized()) {
      Trace.WriteLine("****Interprocess: GetDirectory FAILED");
      directory = new Directory();
      return false;
    }

    Trace.WriteLine("****Interprocess: GetDirectory");

    // ロック取得
    this.mutexDirectory.WaitOne();

    // そのままコピー
    directory = this.ReadDirectory();

    // ロック解放
    this.mutexDirectory.ReleaseMutex();

    return true;
  }

  /// メッセージを作成する
  /// @pre 事前にInitMessageが実行されている必要がある
  /// @param message 作成されるmessage
  /// @return 作成が成功したか
  public bool SendMessage(Message message) {
    // 初期化されていなければ失敗
    if (!this.IsMessageInitialized()) {
      Trace.WriteLine("****Interprocess: SendMessage FAILED");
      return false;
    }

    Trace.WriteLine("****Interprocess: SendMessage");

    // ロック取得
    this.mutexMessage.WaitOne();

    // そのままコピー
    this.WriteMessage(message);

    // ロック解放
    this.mutexMessage.ReleaseMutex();

    return true;
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// Directoryのサイズ
  private int sizeOfDirectory;
  /// Messageのサイズ
  private int sizeOfMessage;

  /// 共有メモリ: Directory
  private MemoryMappedFile directory;
  /// ビュー: Directory
  private MemoryMappedViewStream viewOfDirectory;
  /// Mutex: Directory
  private Mutex mutexDirectory;

  /// 共有メモリ: Message
  private MemoryMappedFile message;
  /// ビュー: Message
  private MemoryMappedViewStream viewOfMessage;
  /// Mutex: Message
  private Mutex mutexMessage;
}
}   // namespace SCFF.Interprocess