
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
  // プロセスIDの取得
  const DWORD process_id = GetCurrentProcessId();
  interprocess_.RemoveEntry(process_id);
}

// 初期化
bool SCFFMonitor::Init(scff_imaging::ImagePixelFormat pixel_format,
                       int width, int height, double fps) {
  // プロセスIDの取得
  const DWORD process_id = GetCurrentProcessId();

  // interprocessオブジェクトの初期化
  const bool success_directory = interprocess_.InitDirectory();
  const bool success_message = interprocess_.InitMessage(process_id);
  ASSERT(success_directory && success_message);

  // エントリの追加
  scff_interprocess::Entry entry;
  entry.process_id = process_id;
  GetModuleBaseNameA(GetCurrentProcess(), NULL, entry.process_name, scff_interprocess::kMaxPath);
  entry.sample_image_pixel_format = pixel_format;
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

static void MessageToLayoutParameter(
    const scff_interprocess::Message &message,
    int index,
    scff_imaging::LayoutParameter *parameter) {
  ASSERT(0 <= index && index < message.layout_element_count);
  
  parameter->bound_x =
      message.layout_parameters[index].bound_x;
  parameter->bound_y =
      message.layout_parameters[index].bound_y;
  parameter->bound_width =
      message.layout_parameters[index].bound_width;
  parameter->bound_height =
      message.layout_parameters[index].bound_height;
  parameter->window =
      reinterpret_cast<HWND>(message.layout_parameters[index].window);
  parameter->clipping_x =
      message.layout_parameters[index].clipping_x;
  parameter->clipping_y =
      message.layout_parameters[index].clipping_y;
  parameter->clipping_width =
      message.layout_parameters[index].clipping_width;
  parameter->clipping_height =
      message.layout_parameters[index].clipping_height;
  parameter->show_cursor =
      message.layout_parameters[index].show_cursor != 0;
  parameter->show_layered_window =
      message.layout_parameters[index].show_layered_window != 0;
  switch (message.layout_parameters[index].sws_flags) {
  case scff_interprocess::kFastBilinear:
    parameter->sws_flags = scff_imaging::kFastBilinear;
    break;
  case scff_interprocess::kBilinear:
    parameter->sws_flags = scff_imaging::kBilinear;
    break;
  case scff_interprocess::kBicubic:
    parameter->sws_flags = scff_imaging::kBicubic;
    break;
  case scff_interprocess::kX:
    parameter->sws_flags = scff_imaging::kX;
    break;
  case scff_interprocess::kPoint:
    parameter->sws_flags = scff_imaging::kPoint;
    break;
  case scff_interprocess::kArea:
    parameter->sws_flags = scff_imaging::kArea;
    break;
  case scff_interprocess::kBicublin:
    parameter->sws_flags = scff_imaging::kBicublin;
    break;
  case scff_interprocess::kGauss:
    parameter->sws_flags = scff_imaging::kGauss;
    break;
  case scff_interprocess::kSinc:
    parameter->sws_flags = scff_imaging::kSinc;
    break;
  case scff_interprocess::kLanczos:
    parameter->sws_flags = scff_imaging::kLanczos;
    break;
  case scff_interprocess::kSpline:
    parameter->sws_flags = scff_imaging::kSpline;
    break;
  case scff_interprocess::kHQFastBilinear:
    parameter->sws_flags = scff_imaging::kHQFastBilinear;
    break;
  case scff_interprocess::kHQBilinear:
    parameter->sws_flags = scff_imaging::kHQBilinear;
    break;
  case scff_interprocess::kHQBicubic:
    parameter->sws_flags = scff_imaging::kHQBicubic;
    break;
  case scff_interprocess::kHQX:
    parameter->sws_flags = scff_imaging::kHQX;
    break;
  case scff_interprocess::kHQPoint:
    parameter->sws_flags = scff_imaging::kHQPoint;
    break;
  case scff_interprocess::kHQArea:
    parameter->sws_flags = scff_imaging::kHQArea;
    break;
  case scff_interprocess::kHQBicublin:
    parameter->sws_flags = scff_imaging::kHQBicublin;
    break;
  case scff_interprocess::kHQGauss:
    parameter->sws_flags = scff_imaging::kHQGauss;
    break;
  case scff_interprocess::kHQSinc:
    parameter->sws_flags = scff_imaging::kHQSinc;
    break;
  case scff_interprocess::kHQLanczos:
    parameter->sws_flags = scff_imaging::kHQLanczos;
    break;
  case scff_interprocess::kHQSpline:
    parameter->sws_flags = scff_imaging::kHQSpline;
    break;
  default:
    ASSERT(false);
    parameter->sws_flags = scff_imaging::kFastBilinear;
    break;
  }
  parameter->stretch =
      message.layout_parameters[index].stretch != 0;
  parameter->keep_aspect_ratio =
      message.layout_parameters[index].keep_aspect_ratio != 0;
  switch (message.layout_parameters[index].rotate_direction) {
  case scff_interprocess::kNoRotate:
    parameter->rotate_direction = scff_imaging::kNoRotate;
    break;
  case scff_interprocess::k90Degrees:
    parameter->rotate_direction = scff_imaging::k90Degrees;
    break;
  case scff_interprocess::k180Degrees:
    parameter->rotate_direction = scff_imaging::k180Degrees;
    break;
  case scff_interprocess::k270Degrees:
    parameter->rotate_direction = scff_imaging::k270Degrees;
    break;
  default:
    ASSERT(false);
    parameter->rotate_direction = scff_imaging::kNoRotate;
    break;
  }
}

// リクエストがあるかどうか調べ、あれば実体を、なければNULLを返す
scff_imaging::Request* SCFFMonitor::CreateRequest() {
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

  // メッセージを取得(InitMessage済みなのでProcessIDを指定する必要はない)
  scff_interprocess::Message message;
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

  // メッセージの内容を処理する
  scff_imaging::Request *tmp_request = 0;

  switch (message.layout_type) {
  case scff_interprocess::kNullLayout:
    //-----------------------------------------------------------------
    // ResetLayoutRequest
    //-----------------------------------------------------------------
    MyDbgLog((LOG_TRACE, kDbgImportant,
              TEXT("SCFFMonitor: ResetLayoutRequest arrived(%lld)."),
              message.timestamp));
    tmp_request = new scff_imaging::ResetLayoutRequest();
    break;

  case scff_interprocess::kNativeLayout:
    //-----------------------------------------------------------------
    // SetNativeLayoutRequest
    //-----------------------------------------------------------------
    MyDbgLog((LOG_TRACE, kDbgImportant,
              TEXT("SCFFMonitor: SetNativeLayoutRequest arrived(%lld)."),
              message.timestamp));
    // パラメータをメッセージから抽出
    scff_imaging::LayoutParameter parameter;
    MessageToLayoutParameter(message, 0, &parameter);
    tmp_request = new scff_imaging::SetNativeLayoutRequest(parameter);
    break;

  case scff_interprocess::kComplexLayout:
    //-----------------------------------------------------------------
    // SetComplexLayoutRequest
    //-----------------------------------------------------------------
    MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("SCFFMonitor: SetComplexLayoutRequest arrived(%lld)."),
          message.timestamp));
    // パラメータをメッセージから抽出
    scff_imaging::LayoutParameter parameters[scff_imaging::kMaxProcessorSize];
    for (int i = 0; i < message.layout_element_count; i++) {
      MessageToLayoutParameter(message, i, &(parameters[i]));
    }
    tmp_request = new scff_imaging::SetComplexLayoutRequest(
        message.layout_element_count,
        parameters);
    break;
  }
  return tmp_request;
}

// 使い終わったリクエストを解放する
void SCFFMonitor::ReleaseRequest(scff_imaging::Request* request) {
  if (request == 0) {   // NULL
    // NULLなら何もしない
    return;
  } else {
    delete request;
    request = 0;        // NULL
  }
}
