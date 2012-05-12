
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

/// @file scff-interprocess/interprocess-class.cs
/// @brief SCFFのプロセス間通信に関するクラスの宣言
/// @warning To me: このファイルの中から別のファイルへのusingは禁止！
///- 別の言語に移植する場合も最大2ファイルでお願いします

using System.Runtime.InteropServices;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Diagnostics;

namespace scff_interprocess {

// プロセス間通信を担当するクラス
partial class Interprocess {
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

  //-------------------------------------------------------------------
  // Directory
  //-------------------------------------------------------------------

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

  //-------------------------------------------------------------------
  // Message
  //-------------------------------------------------------------------

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
  // for SCFF GUI Client
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

  //-------------------------------------------------------------------
  // メンバ変数
  //-------------------------------------------------------------------

  /// @brief Directoryのサイズ
  int size_of_directory_;
  /// @brief Messageのサイズ
  int size_of_message_;

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
}   // namespace scff_interprocess