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

/// @file scff-interprocess/interprocess.cc
/// SCFFのプロセス間通信に関するクラスの定義
/// @warning To me: このファイルの中から別のファイルへのIncludeは禁止！
///- 別の言語に移植する場合も最大2ファイルでお願いします

#include "scff-interprocess/interprocess.h"

#include <stdio.h>

namespace scff_interprocess {

//=====================================================================
// scff-interprocess::Interprocess
//=====================================================================

// コンストラクタ
Interprocess::Interprocess()
    : directory_(nullptr),
      view_of_directory_(nullptr),
      mutex_directory_(nullptr),
      message_(nullptr),
      view_of_message_(nullptr),
      mutex_message_(nullptr) {
  // nop
  OutputDebugString(TEXT("****Interprocess: NEW\n"));
}

// デストラクタ
Interprocess::~Interprocess() {
  OutputDebugString(TEXT("****Interprocess: DELETE\n"));
  // 解放忘れがないように
  ReleaseMessage();
  ReleaseDirectory();
}

//---------------------------------------------------------------------

// Directory初期化
bool Interprocess::InitDirectory() {
  // 念のため解放
  ReleaseDirectory();

  // 仮想メモリ(Directory)の作成
  HANDLE tmp_directory =
      CreateFileMappingA(INVALID_HANDLE_VALUE,
                          nullptr,
                          PAGE_READWRITE,
                          0,
                          sizeof(Directory),
                          kDirectoryName);
  if (tmp_directory == nullptr) {
    // 仮想メモリ作成失敗
    return false;
  }
  DWORD error_create_file_mapping = GetLastError();

  // ビューの作成
  LPVOID tmp_view_of_directory =
      MapViewOfFile(tmp_directory,
                    FILE_MAP_ALL_ACCESS,
                    0, 0, 0);
  if (tmp_view_of_directory == nullptr) {
    // ビュー作成失敗
    CloseHandle(tmp_directory);
    return false;
  }

  // 最初に共有メモリを作成した場合は0クリアしておく
  if (tmp_view_of_directory != nullptr &&
      error_create_file_mapping != ERROR_ALREADY_EXISTS) {
    ZeroMemory(tmp_view_of_directory, sizeof(Directory));
  }

  // Mutexの作成
  HANDLE tmp_mutex_directory =
      CreateMutexA(nullptr, false, kDirectoryMutexName);
  if (tmp_mutex_directory == nullptr) {
    // Mutex作成失敗
    UnmapViewOfFile(tmp_view_of_directory);
    CloseHandle(tmp_directory);
    return false;
  }
  DWORD error_create_mutex = GetLastError();

  // 最初にMutexを作成した場合は…なにもしなくていい
  if (tmp_mutex_directory != nullptr &&
      error_create_mutex != ERROR_ALREADY_EXISTS) {
    // nop
  }

  // メンバ変数に設定
  directory_ = tmp_directory;
  view_of_directory_ = tmp_view_of_directory;
  mutex_directory_ = tmp_mutex_directory;

  OutputDebugString(TEXT("****Interprocess: InitDirectory Done\n"));
  return true;
}

// Directoryの初期化が成功したか
bool Interprocess::IsDirectoryInitialized() {
  return directory_ != nullptr &&
         view_of_directory_ != nullptr &&
         mutex_directory_ != nullptr;
}

// Directory解放
void Interprocess::ReleaseDirectory() {
  if (mutex_directory_ != nullptr) {
    CloseHandle(mutex_directory_);
    mutex_directory_ = nullptr;
  }
  if (view_of_directory_ != nullptr) {
    UnmapViewOfFile(view_of_directory_);
    view_of_directory_ = nullptr;
  }
  if (directory_ != nullptr) {
    CloseHandle(directory_);
    directory_ = nullptr;
  }
}

//---------------------------------------------------------------------

// Message初期化
bool Interprocess::InitMessage(uint32_t process_id) {
  // 念のため解放
  ReleaseMessage();

  // 仮想メモリの名前
  char message_name[256];
  ZeroMemory(message_name, sizeof(message_name));
  sprintf_s(message_name,
            256, "%s%d",
            kMessageNamePrefix, process_id);

  // 仮想メモリ(Message<process_id>)の作成
  HANDLE tmp_message =
      CreateFileMappingA(INVALID_HANDLE_VALUE,
                          nullptr,
                          PAGE_READWRITE,
                          0,
                          sizeof(Message),
                          message_name);
  if (tmp_message == nullptr) {
    // 仮想メモリ作成失敗
    return false;
  }
  DWORD error_create_file_mapping = GetLastError();

  // ビューの作成
  LPVOID tmp_view_of_message =
      MapViewOfFile(tmp_message,
                    FILE_MAP_ALL_ACCESS,
                    0, 0, 0);
  if (tmp_view_of_message == nullptr) {
    // ビュー作成失敗
    CloseHandle(tmp_message);
    return false;
  }

  // 最初に共有メモリを作成した場合は0クリアしておく
  if (tmp_view_of_message != nullptr &&
      error_create_file_mapping != ERROR_ALREADY_EXISTS) {
    ZeroMemory(tmp_view_of_message, sizeof(Message));
  }

  // Mutexの名前
  char message_mutex_name[256];
  ZeroMemory(message_mutex_name, sizeof(message_mutex_name));
  sprintf_s(message_mutex_name,
            256, "%s%d",
            kMessageMutexNamePrefix, process_id);

  // Mutexの作成
  HANDLE tmp_mutex_message =
      CreateMutexA(nullptr, false, message_mutex_name);
  if (tmp_mutex_message == nullptr) {
    // Mutex作成失敗
    UnmapViewOfFile(tmp_view_of_message);
    CloseHandle(tmp_message);
    return false;
  }
  DWORD error_create_mutex = GetLastError();

  // 最初にMutexを作成した場合は…なにもしなくていい
  if (tmp_mutex_message != nullptr &&
      error_create_mutex != ERROR_ALREADY_EXISTS) {
    // nop
  }

  // メンバ変数に設定
  message_ = tmp_message;
  view_of_message_ = tmp_view_of_message;
  mutex_message_ = tmp_mutex_message;

  OutputDebugString(TEXT("****Interprocess: InitMessage Done\n"));
  return true;
}

// Messageの初期化が成功したか
bool Interprocess::IsMessageInitialized() {
  return message_ != nullptr &&
         view_of_message_ != nullptr &&
         mutex_message_ != nullptr;
}

// Message解放
void Interprocess::ReleaseMessage() {
  if (mutex_message_ != nullptr) {
    CloseHandle(mutex_message_);
    mutex_message_ = nullptr;
  }
  if (view_of_message_ != nullptr) {
    UnmapViewOfFile(view_of_message_);
    view_of_message_ = nullptr;
  }
  if (message_ != nullptr) {
    CloseHandle(message_);
    message_ = nullptr;
  }
}

//-------------------------------------------------------------------
// for SCFF DirectShow Filter
//-------------------------------------------------------------------

// エントリを作成する
bool Interprocess::AddEntry(const Entry &entry) {
  // 初期化されていなければ失敗
  if (!IsDirectoryInitialized()) {
    OutputDebugString(TEXT("****Interprocess: AddEntry FAILED\n"));
    return false;
  }

  OutputDebugString(TEXT("****Interprocess: AddEntry\n"));

  // ロック取得
  WaitForSingleObject(mutex_directory_, INFINITE);

  Directory *directory =
      static_cast<Directory*>(view_of_directory_);
  bool success = false;
  for (int i = 0; i < kMaxEntry; i++) {
    /// @warning 重複する場合がある？
    if (directory->entries[i].process_id == 0) {
      // PODなのでコピー可能（のはず）
      directory->entries[i] = entry;
      success = true;
      break;
    }
  }

  // ロック解放
  ReleaseMutex(mutex_directory_);

  if (success) {
    OutputDebugString(TEXT("****Interprocess: AddEntry SUCCESS\n"));
  }

  return success;
}

// エントリを削除する
bool Interprocess::RemoveEntry(uint32_t process_id) {
  // 初期化されていなければ失敗
  if (!IsDirectoryInitialized()) {
    OutputDebugString(TEXT("****Interprocess: RemoveEntry FAILED\n"));
    return false;
  }

  OutputDebugString(TEXT("****Interprocess: RemoveEntry\n"));

  // ロック取得
  WaitForSingleObject(mutex_directory_, INFINITE);

  Directory *directory =
      static_cast<Directory*>(view_of_directory_);
  for (int i = 0; i < kMaxEntry; i++) {
    if (directory->entries[i].process_id == process_id) {
      ZeroMemory(&(directory->entries[i]), sizeof(directory->entries[i]));
      OutputDebugString(TEXT("****Interprocess: RemoveEntry SUCCESS\n"));
      break;
    }
  }

  // ロック解放
  ReleaseMutex(mutex_directory_);

  return true;
}

// メッセージを受け取る
bool Interprocess::ReceiveMessage(Message *message) {
  // 初期化されていなければ失敗
  if (!IsMessageInitialized()) {
    OutputDebugString(TEXT("****Interprocess: ReceiveMessage FAILED\n"));
    return false;
  }

  //OutputDebugString(TEXT("****Interprocess: ReceiveMessage\n"));

  // ロック取得
  WaitForSingleObject(mutex_message_, INFINITE);

  // そのままコピー
  memcpy(message, view_of_message_, sizeof(Message));

  // ロック解放
  ReleaseMutex(mutex_message_);

  return true;
}

//-------------------------------------------------------------------

// ディレクトリを取得する
bool Interprocess::GetDirectory(Directory *directory) {
  // 初期化されていなければ失敗
  if (!IsDirectoryInitialized()) {
    OutputDebugString(TEXT("****Interprocess: GetDirectory FAILED\n"));
    return false;
  }

  OutputDebugString(TEXT("****Interprocess: GetDirectory\n"));

  // ロック取得
  WaitForSingleObject(mutex_directory_, INFINITE);

  // そのままコピー
  memcpy(directory, view_of_directory_, sizeof(Directory));

  // ロック解放
  ReleaseMutex(mutex_directory_);

  return true;
}

// メッセージを作成する
bool Interprocess::SendMessage(const Message &message) {
  // 初期化されていなければ失敗
  if (!IsMessageInitialized()) {
    OutputDebugString(TEXT("****Interprocess: SendMessage FAILED\n"));
    return false;
  }

  OutputDebugString(TEXT("****Interprocess: SendMessage\n"));

  // ロック取得
  WaitForSingleObject(mutex_message_, INFINITE);

  // そのままコピー
  memcpy(view_of_message_, &message, sizeof(Message));

  // ロック解放
  ReleaseMutex(mutex_message_);

  return true;
}
}   // namespace scff_interprocess
