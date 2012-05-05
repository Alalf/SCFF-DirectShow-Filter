
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

/// @file base/scff-monitor.cc
/// @brief SCFFMonitorの定義

#include "base/scff-monitor.h"

#include <Psapi.h>

#include "base/debug.h"
#include "base/constants.h"

//=====================================================================
// SCFFMonitor
//=====================================================================

// コンストラクタ
SCFFMonitor::SCFFMonitor()
    : last_polling_clock_(-1),        // ありえない値
      last_message_timestamp_(-1) {   // ありえない値
  MyDbgLog((LOG_MEMORY, kDbgNewDelete, TEXT("NEW SCFFMonitor")));
}

// デストラクタ
SCFFMonitor::~SCFFMonitor() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete, TEXT("DELETE SCFFMonitor")));
  interprocess_.RemoveEntry();
}

// 初期化
bool SCFFMonitor::Init(imaging::ImagePixelFormat pixel_format,
                       int width, int height, double fps) {
  // プロセスIDを取得
  const DWORD process_id = GetCurrentProcessId();

  // interprocessオブジェクトの初期化
  const bool success_directory = interprocess_.InitDirectory();
  const bool success_message = interprocess_.InitMessage(process_id);
  ASSERT(success_directory && success_message);

  // エントリの追加
  SCFFEntry entry;
  entry.process_id = process_id;
  GetModuleBaseNameA(GetCurrentProcess(), NULL, entry.process_name, MAX_PATH);
  entry.sample_pixel_format = pixel_format;
  entry.sample_width = width;
  entry.sample_height = height;
  entry.fps = fps;
  interprocess_.AddEntry(entry);

  // タイムスタンプを念のため更新
  last_polling_clock_ = -1;
  last_message_timestamp_ = -1;

  return true;
}

//---------------------------------------------------------------------
// リクエスト
//---------------------------------------------------------------------

// リクエストがあるかどうか調べ、あれば実体を、なければNULLを返す
imaging::Request* SCFFMonitor::CreateRequest() {
  // 前回のCreateRequestからの経過時間(Sec)
  const clock_t now = clock();
  const double erapsed_time_from_last_polling =
      static_cast<double>(now - last_polling_clock_) / CLOCKS_PER_SEC;

  // ポーリングはkSCFFMonitorPollingInterval秒に1回である
  if (erapsed_time_from_last_polling < kSCFFMonitorPollingInterval) {
    return 0;   // NULL
  }

  // ポーリング記録の更新
  last_polling_clock_ = now;

  //-------------------------------------------------------------------
  // メッセージを確認する
  //-------------------------------------------------------------------

  // プロセスIDを取得
  const DWORD process_id = GetCurrentProcessId();
  SCFFMessage message;
  interprocess_.ReceiveMessage(&message);

  // タイムスタンプが0以下ならメッセージは空と同じ扱いとなる
  if (message.timestamp <= 0) {
    return 0;
  }

  // 前回処理したタイムスタンプより前、あるいは同じ場合は何もしない
  if (message.timestamp <= last_message_timestamp_) {
    return 0;
  }

  // タイムスタンプを進めておく
  last_message_timestamp_ = message.timestamp;

  /// @todo(me) ComplexLayoutのメッセージは来ても無視する

  // メッセージの内容を処理する
  imaging::Request *tmp_request = 0;
  imaging::ScreenCaptureParameter parameter;
  bool stretch;
  bool keep_aspect_ratio;

  switch (message.layout_type) {
  case kSCFFNullLayout:
    //-----------------------------------------------------------------
    // ResetLayoutRequest
    //-----------------------------------------------------------------
    MyDbgLog((LOG_TRACE, kDbgImportant,
              TEXT("SCFFMonitor: ResetLayoutRequest arrived.")));
    tmp_request = new imaging::ResetLayoutRequest();
    break;

  case kSCFFNativeLayout:
    //-----------------------------------------------------------------
    // SetNativeLayoutRequest
    //-----------------------------------------------------------------
    MyDbgLog((LOG_TRACE, kDbgImportant,
              TEXT("SCFFMonitor: SetNativeLayoutRequest arrived.")));

    // 以下のデータは無視される
    // message.layout_element_count
    // message.layout_parameters[0]以外の全てのエントリ
    // message.layout_parameters[0].bound_x
    // message.layout_parameters[0].bound_y
    // message.layout_parameters[0].bound_width
    // message.layout_parameters[0].bound_height

    // 取り込みパラメータを読み取る
    parameter.window =
        reinterpret_cast<HWND>(message.layout_parameters[0].window);
    parameter.clipping_x =
        message.layout_parameters[0].clipping_x;
    parameter.clipping_y =
        message.layout_parameters[0].clipping_y;
    parameter.clipping_width =
        message.layout_parameters[0].clipping_width;
    parameter.clipping_height =
        message.layout_parameters[0].clipping_height;
    parameter.show_cursor =
        message.layout_parameters[0].show_cursor != 0;
    parameter.show_layered_window =
        message.layout_parameters[0].show_layered_window != 0;
    switch (message.layout_parameters[0].sws_flags) {
    case kSCFFFastBilinear:
      parameter.sws_flags = imaging::kFastBilinear;
      break;
    case kSCFFBilinear:
      parameter.sws_flags = imaging::kBilinear;
      break;
    case kSCFFBicubic:
      parameter.sws_flags = imaging::kBicubic;
      break;
    case kSCFFX:
      parameter.sws_flags = imaging::kX;
      break;
    case kSCFFPoint:
      parameter.sws_flags = imaging::kPoint;
      break;
    case kSCFFArea:
      parameter.sws_flags = imaging::kArea;
      break;
    case kSCFFBicublin:
      parameter.sws_flags = imaging::kBicublin;
      break;
    case kSCFFGauss:
      parameter.sws_flags = imaging::kGauss;
      break;
    case kSCFFSinc:
      parameter.sws_flags = imaging::kSinc;
      break;
    case kSCFFLanczos:
      parameter.sws_flags = imaging::kLanczos;
      break;
    case kSCFFSpline:
      parameter.sws_flags = imaging::kSpline;
      break;
    default:
      parameter.sws_flags = imaging::kFastBilinear;
      break;
    }
    stretch = message.layout_parameters[0].stretch != 0;
    keep_aspect_ratio = message.layout_parameters[0].keep_aspect_ratio != 0;

    tmp_request = new imaging::SetNativeLayoutRequest(
        parameter,
        stretch,
        keep_aspect_ratio);
    break;

  case kSCFFComplexLayout:
    //-----------------------------------------------------------------
    // SetComplexLayoutRequest
    //-----------------------------------------------------------------
    MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("SCFFMonitor: SetComplexLayoutRequest is not implemented.")));
    ASSERT(false);
    tmp_request = 0;  // NULL
    break;
  }
  return tmp_request;
}

// 使い終わったリクエストを解放する
void SCFFMonitor::ReleaseRequest(imaging::Request* request) {
  if (request == 0) {   // NULL
    // NULLなら何もしない
    return;
  } else {
    delete request;
    request = 0;        // NULL
  }
}
