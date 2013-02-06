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

/// @file scff_interprocess/interprocess.cc
/// SCFFのプロセス間通信に関するクラスの定義
/// @warning To me: このファイルの中から別のファイルへのIncludeは禁止！
/// - 別の言語に移植する場合も最大2ファイルでお願いします

#include "scff_interprocess/interprocess.h"

#include <stdio.h>
#include <tchar.h>

namespace scff_interprocess {

//=====================================================================
// scff_interprocess::Interprocess
//=====================================================================

Interprocess::Interprocess()
    : directory_(nullptr),
      view_of_directory_(nullptr),
      mutex_directory_(nullptr),
      message_(nullptr),
      view_of_message_(nullptr),
      mutex_message_(nullptr),
      error_event_(nullptr),
      shutdown_event_(nullptr) {
  // nop
  OutputDebugString(TEXT("****Interprocess: NEW\n"));
}

Interprocess::~Interprocess() {
  OutputDebugString(TEXT("****Interprocess: DELETE\n"));
  // 解放忘れがないように
  ReleaseErrorEvent();
  ReleaseShutdownEvent();
  ReleaseMessage();
  ReleaseDirectory();
}

//---------------------------------------------------------------------

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

bool Interprocess::IsDirectoryInitialized() {
  return directory_ != nullptr &&
         view_of_directory_ != nullptr &&
         mutex_directory_ != nullptr;
}

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

bool Interprocess::IsMessageInitialized() {
  return message_ != nullptr &&
         view_of_message_ != nullptr &&
         mutex_message_ != nullptr;
}

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

//---------------------------------------------------------------------

bool Interprocess::InitErrorEvent(uint32_t process_id) {
  // 念のため解放
  ReleaseErrorEvent();

  // イベントの名前
  TCHAR error_event_name[256];
  ZeroMemory(error_event_name, sizeof(error_event_name));
  _stprintf_s(error_event_name,
              256, TEXT("%s%d"),
              kErrorEventNamePrefix, process_id);
            
  // イベント(ErrorEvent<process_id>)の作成
  HANDLE tmp_error_event = CreateEvent(nullptr, FALSE, FALSE, error_event_name);
  if (tmp_error_event == nullptr) {
    // イベント作成失敗
    return false;
  }
  DWORD error_create_event = GetLastError();

  // 最初にEventを作成した場合は…なにもしなくていい
  if (error_create_event != ERROR_ALREADY_EXISTS) {
    // nop
  }

  // メンバ変数に設定
  error_event_ = tmp_error_event;

  OutputDebugString(TEXT("****Interprocess: InitErrorEvent Done\n"));
  return true;
}

bool Interprocess::IsErrorEventInitialized() {
  return error_event_ != nullptr;
}

void Interprocess::ReleaseErrorEvent() {
  if (error_event_ != nullptr) {
    CloseHandle(error_event_);
    error_event_ = nullptr;
  }
}

//---------------------------------------------------------------------

bool Interprocess::InitShutdownEvent() {
  // プログラム全体で一回だけCreate/CloseすればよいのでCloseはしない
  if (IsShutdownEventInitialized()) return true;
            
  // イベント(ShutdownEvent)の作成
  HANDLE tmp_shutdown_event = CreateEvent(nullptr, TRUE, FALSE, nullptr);
  if (tmp_shutdown_event == nullptr) {
    // イベント作成失敗
    return false;
  }

  // メンバ変数に設定
  shutdown_event_ = tmp_shutdown_event;

  OutputDebugString(TEXT("****Interprocess: InitShutdownEvent Done\n"));
  return true;
}

bool Interprocess::IsShutdownEventInitialized() {
  return shutdown_event_ != nullptr;
}

void Interprocess::ReleaseShutdownEvent() {
  if (shutdown_event_ != nullptr) {
    CloseHandle(shutdown_event_);
    shutdown_event_ = nullptr;
  }
}

//-------------------------------------------------------------------
// for SCFF DirectShow Filter
//-------------------------------------------------------------------

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

bool Interprocess::RaiseErrorEvent() {
  // 初期化されていなければ失敗
  if (!IsErrorEventInitialized()) {
    OutputDebugString(TEXT("****Interprocess: RaiseErrorEvent FAILED\n"));
    return false;
  }

  OutputDebugString(TEXT("****Interprocess: RaiseErrorEvent\n"));

  // シグナルを送る
  BOOL error_set_event = SetEvent(error_event_);
  if (!error_set_event) {
    OutputDebugString(TEXT("****Interprocess: RaiseErrorEvent FAILED\n"));
    return false;
  }

  return true;
}

//-------------------------------------------------------------------

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

bool Interprocess::WaitUntilErrorEventOccured() {
  // 初期化されていなければ失敗
  if (!IsErrorEventInitialized()) {
    OutputDebugString(TEXT("****Interprocess: WaitUntilErrorEventOccured FAILED\n"));
    return false;
  }

  OutputDebugString(TEXT("****Interprocess: WaitUntilErrorEventOccured\n"));

  // シグナル状態になるまで待機
  const HANDLE events[] = {error_event_, shutdown_event_};
  const int signaled_event_index = WaitForMultipleObjects(2, events, FALSE, INFINITE);
  if (signaled_event_index != 0) {
    // エラー以外のイベントが起きた場合は失敗
    OutputDebugString(TEXT("****Interprocess: WaitUntilErrorEventOccured FAILED\n"));
    return false;
  }

  return true;
}

bool Interprocess::RaiseShutdownEvent() {
  // 初期化されていなければ失敗
  if (!IsShutdownEventInitialized()) {
    OutputDebugString(TEXT("****Interprocess: RaiseShutdownEvent FAILED\n"));
    return false;
  }

  OutputDebugString(TEXT("****Interprocess: RaiseShutdownEvent\n"));

  // シグナルを送る
  BOOL error_set_event = SetEvent(shutdown_event_);
  if (!error_set_event) {
    OutputDebugString(TEXT("****Interprocess: RaiseShutdownEvent FAILED\n"));
    return false;
  }

  return true;
}
}   // namespace scff_interprocess
