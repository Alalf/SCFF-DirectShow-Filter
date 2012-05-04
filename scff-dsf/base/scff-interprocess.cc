
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

/// @file base/scff-interprocess.cc
/// @brief SCFFInterprocessの定義
/// @warning To me: このファイルの中から別のファイルへのIncludeは禁止！
///- 別の言語に移植する場合もh+srcで最大2ファイルでお願いします

#include "base/scff-interprocess.h"

#include <stdio.h>

//=====================================================================
// SCFFInterprocess
//=====================================================================

// コンストラクタ
SCFFInterprocess::SCFFInterprocess()
    : scff_directory_(NULL),
      view_of_scff_directory_(0),   // NULL
      mutex_scff_directory_(NULL),
      process_id_(0),               // NULL
      scff_message_(NULL),
      view_of_scff_message_(0),     // NULL
      mutex_scff_message_(NULL) {
  // nop
  OutputDebugString(TEXT("****SCFFInterprocess: NEW\n"));
}

// デストラクタ
SCFFInterprocess::~SCFFInterprocess() {
  OutputDebugString(TEXT("****SCFFInterprocess: DELETE\n"));
  // 解放忘れがないように
  ReleaseMessage();
  ReleaseDirectory();
}

// SCFFDirectory初期化
bool SCFFInterprocess::InitDirectory() {
  // 念のため解放
  ReleaseDirectory();

  // 仮想メモリ(SCFFDirectory)の作成
  HANDLE tmp_scff_directory =
      CreateFileMappingA(INVALID_HANDLE_VALUE,
                          NULL,
                          PAGE_READWRITE,
                          0,
                          sizeof(SCFFDirectory),
                          kSCFFDirectoryName);
  if (tmp_scff_directory == NULL) {
    // 仮想メモリ作成失敗
    return false;
  }
  DWORD error_create_file_mapping = GetLastError();

  // ビューの作成
  LPVOID tmp_view_of_scff_directory =
      MapViewOfFile(tmp_scff_directory,
                    FILE_MAP_ALL_ACCESS,
                    0, 0, 0);
  if (tmp_view_of_scff_directory == NULL) {
    // ビュー作成失敗
    CloseHandle(tmp_scff_directory);
    return false;
  }

  // 最初に共有メモリを作成した場合は0クリアしておく
  if (tmp_view_of_scff_directory != NULL &&
      error_create_file_mapping != ERROR_ALREADY_EXISTS) {
    ZeroMemory(tmp_view_of_scff_directory, sizeof(SCFFDirectory));
  }

  // Mutexの作成
  HANDLE tmp_mutex_scff_directory =
      CreateMutexA(NULL, false, kSCFFDirectoryMutexName);
  if (tmp_mutex_scff_directory == NULL) {
    // Mutex作成失敗
    UnmapViewOfFile(tmp_view_of_scff_directory);
    CloseHandle(tmp_scff_directory);
    return false;
  }
  DWORD error_create_mutex = GetLastError();

  // 最初にMutexを作成した場合は…なにもしなくていい
  if (tmp_mutex_scff_directory != NULL &&
      error_create_mutex != ERROR_ALREADY_EXISTS) {
    // nop
  }

  // メンバ変数に設定
  scff_directory_ = tmp_scff_directory;
  view_of_scff_directory_ = tmp_view_of_scff_directory;
  mutex_scff_directory_ = tmp_mutex_scff_directory;

  OutputDebugString(TEXT("****SCFFInterprocess: InitDirectory Done\n"));
  return true;
}

// SCFFDirectoryの初期化が成功したか
bool SCFFInterprocess::IsDirectoryInitialized() {
  return scff_directory_ != NULL &&
         view_of_scff_directory_ != NULL &&
         mutex_scff_directory_ != NULL;
}

// SCFFDirectory解放
void SCFFInterprocess::ReleaseDirectory() {
  if (mutex_scff_directory_ != NULL) {
    CloseHandle(mutex_scff_directory_);
    mutex_scff_directory_ = NULL;
  }
  if (view_of_scff_directory_ != 0) {   // NULL
    UnmapViewOfFile(view_of_scff_directory_);
    view_of_scff_directory_ = 0;
  }
  if (scff_directory_ != NULL) {
    CloseHandle(scff_directory_);
    scff_directory_ = NULL;
  }
}

// SCFFMessage初期化
bool SCFFInterprocess::InitMessage(uint32_t process_id) {
  // 念のため解放
  ReleaseMessage();

  // 仮想メモリの名前
  char scff_message_name[256];
  ZeroMemory(scff_message_name, sizeof(scff_message_name));
  sprintf_s(scff_message_name,
            256, "%s%d",
            kSCFFMessageNamePrefix, process_id);

  // 仮想メモリ(SCFFMessage<process_id>)の作成
  HANDLE tmp_scff_message =
      CreateFileMappingA(INVALID_HANDLE_VALUE,
                          NULL,
                          PAGE_READWRITE,
                          0,
                          sizeof(SCFFMessage),
                          scff_message_name);
  if (tmp_scff_message == NULL) {
    // 仮想メモリ作成失敗
    return false;
  }
  DWORD error_create_file_mapping = GetLastError();

  // ビューの作成
  LPVOID tmp_view_of_scff_message =
      MapViewOfFile(tmp_scff_message,
                    FILE_MAP_ALL_ACCESS,
                    0, 0, 0);
  if (tmp_view_of_scff_message == NULL) {
    // ビュー作成失敗
    CloseHandle(tmp_scff_message);
    return false;
  }

  // 最初に共有メモリを作成した場合は0クリアしておく
  if (tmp_view_of_scff_message != NULL &&
      error_create_file_mapping != ERROR_ALREADY_EXISTS) {
    ZeroMemory(tmp_view_of_scff_message, sizeof(SCFFMessage));
  }

  // Mutexの名前
  char scff_message_mutex_name[256];
  ZeroMemory(scff_message_mutex_name, sizeof(scff_message_mutex_name));
  sprintf_s(scff_message_mutex_name,
            256, "%s%d",
            kSCFFMessageMutexNamePrefix, process_id);

  // Mutexの作成
  HANDLE tmp_mutex_scff_message =
      CreateMutexA(NULL, false, scff_message_mutex_name);
  if (tmp_mutex_scff_message == NULL) {
    // Mutex作成失敗
    UnmapViewOfFile(tmp_view_of_scff_message);
    CloseHandle(tmp_scff_message);
    return false;
  }
  DWORD error_create_mutex = GetLastError();

  // 最初にMutexを作成した場合は…なにもしなくていい
  if (tmp_mutex_scff_message != NULL &&
      error_create_mutex != ERROR_ALREADY_EXISTS) {
    // nop
  }

  // メンバ変数に設定
  process_id_ = process_id;
  scff_message_ = tmp_scff_message;
  view_of_scff_message_ = tmp_view_of_scff_message;
  mutex_scff_message_ = tmp_mutex_scff_message;

  OutputDebugString(TEXT("****SCFFInterprocess: InitMessage Done\n"));
  return true;
}

// SCFFMessageの初期化が成功したか
bool SCFFInterprocess::IsMessageInitialized() {
  return process_id_ != 0 &&
         scff_message_ != NULL &&
         view_of_scff_message_ != NULL &&
         mutex_scff_message_ != NULL;
}

// SCFFMessage解放
void SCFFInterprocess::ReleaseMessage() {
  if (mutex_scff_message_ != NULL) {
    CloseHandle(mutex_scff_message_);
    mutex_scff_message_ = NULL;
  }
  if (view_of_scff_message_ != 0) {   // NULL
    UnmapViewOfFile(view_of_scff_message_);
    view_of_scff_message_ = 0;        // NULL
  }
  if (scff_message_ != NULL) {
    CloseHandle(scff_message_);
    scff_message_ = NULL;
  }
  process_id_ = 0;
}

//-------------------------------------------------------------------
// for SCFF DirectShow Filter
//-------------------------------------------------------------------

// エントリを作成する
bool SCFFInterprocess::AddEntry(const SCFFEntry &entry) {
  // 初期化されていなければ失敗
  if (!IsDirectoryInitialized()) {
    OutputDebugString(TEXT("****SCFFInterprocess: AddEntry FAILED\n"));
    return false;
  }

  OutputDebugString(TEXT("****SCFFInterprocess: AddEntry\n"));

  // ロック取得
  WaitForSingleObject(mutex_scff_directory_, INFINITE);

  SCFFDirectory *scff_directory =
      static_cast<SCFFDirectory*>(view_of_scff_directory_);
  bool success = false;
  for (int i = 0; i < kSCFFMaxEntry; i++) {
    /// @warning 重複する場合がある？
    if (scff_directory->entries[i].process_id == 0) {
      // PODなのでコピー可能（のはず）
      scff_directory->entries[i] = entry;
      success = true;
      break;
    }
  }

  // ロック解放
  ReleaseMutex(mutex_scff_directory_);

  if (success) {
    OutputDebugString(TEXT("****SCFFInterprocess: AddEntry SUCCESS\n"));
  }

  return success;
}

// エントリを削除する
bool SCFFInterprocess::RemoveEntry() {
  // 初期化されていなければ失敗
  if (!IsDirectoryInitialized()) {
    OutputDebugString(TEXT("****SCFFInterprocess: RemoveEntry FAILED\n"));
    return false;
  }

  OutputDebugString(TEXT("****SCFFInterprocess: RemoveEntry\n"));

  // ロック取得
  WaitForSingleObject(mutex_scff_directory_, INFINITE);

  SCFFDirectory *scff_directory =
      static_cast<SCFFDirectory*>(view_of_scff_directory_);
  for (int i = 0; i < kSCFFMaxEntry; i++) {
    if (scff_directory->entries[i].process_id == process_id_) {
      // 0クリア
      scff_directory->entries[i].process_id = 0;
      scff_directory->entries[i].process_name[0] = '\0';
      scff_directory->entries[i].sample_pixel_format = 0;
      scff_directory->entries[i].sample_width = 0;
      scff_directory->entries[i].sample_height = 0;
      scff_directory->entries[i].fps = 0;
      OutputDebugString(TEXT("****SCFFInterprocess: RemoveEntry SUCCESS\n"));
      break;
    }
  }

  // ロック解放
  ReleaseMutex(mutex_scff_directory_);

  return true;
}

// メッセージを受け取る
bool SCFFInterprocess::ReceiveMessage(SCFFMessage *message) {
  // 初期化されていなければ失敗
  if (!IsMessageInitialized()) {
    OutputDebugString(TEXT("****SCFFInterprocess: ReceiveMessage FAILED\n"));
    return false;
  }

  //OutputDebugString(TEXT("****SCFFInterprocess: ReceiveMessage\n"));

  // ロック取得
  WaitForSingleObject(mutex_scff_message_, INFINITE);

  // そのままコピー
  memcpy(message, view_of_scff_message_, sizeof(SCFFMessage));

  // ロック解放
  ReleaseMutex(mutex_scff_message_);

  return true;
}

//-------------------------------------------------------------------

// ディレクトリを取得する
bool SCFFInterprocess::GetDirectory(SCFFDirectory *directory) {
  // 初期化されていなければ失敗
  if (!IsDirectoryInitialized()) {
    OutputDebugString(TEXT("****SCFFInterprocess: GetDirectory FAILED\n"));
    return false;
  }

  OutputDebugString(TEXT("****SCFFInterprocess: GetDirectory\n"));

  // ロック取得
  WaitForSingleObject(mutex_scff_directory_, INFINITE);

  // そのままコピー
  memcpy(directory, view_of_scff_directory_, sizeof(SCFFDirectory));

  // ロック解放
  ReleaseMutex(mutex_scff_directory_);

  return true;
}

// メッセージを作成する
bool SCFFInterprocess::SendMessage(const SCFFMessage &message) {
  // 初期化されていなければ失敗
  if (!IsMessageInitialized()) {
    OutputDebugString(TEXT("****SCFFInterprocess: SendMessage FAILED\n"));
    return false;
  }

  OutputDebugString(TEXT("****SCFFInterprocess: SendMessage\n"));

  // ロック取得
  WaitForSingleObject(mutex_scff_message_, INFINITE);

  // そのままコピー
  memcpy(view_of_scff_message_, &message, sizeof(SCFFMessage));

  // ロック解放
  ReleaseMutex(mutex_scff_message_);

  return true;
}
