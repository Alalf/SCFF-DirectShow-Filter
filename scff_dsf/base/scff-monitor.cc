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
/// SCFFMonitorの定義

#include "base/scff-monitor.h"

#include <Psapi.h>

#include "base/debug.h"
#include "base/constants.h"

//=====================================================================
// SCFFMonitor
//=====================================================================

SCFFMonitor::SCFFMonitor()
    : last_polling_clock_(-1),        // ありえない値
      last_message_timestamp_(-1LL) {   // ありえない値
  MyDbgLog((LOG_MEMORY, kDbgNewDelete, TEXT("NEW SCFFMonitor")));
}

SCFFMonitor::~SCFFMonitor() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete, TEXT("DELETE SCFFMonitor")));
  // プロセスIDの取得
  const DWORD process_id = GetCurrentProcessId();
  interprocess_.RemoveEntry(process_id);
}

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
  GetModuleBaseNameA(
      GetCurrentProcess(),
      nullptr,
      entry.process_name,
      scff_interprocess::kMaxPath);
  /// @attention enum->int32_t
  entry.sample_image_pixel_format = static_cast<int32_t>(pixel_format);
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

namespace {

/// モジュール間のSWScaleConfigの変換
void ConvertSWScaleConfig(
    const scff_interprocess::SWScaleConfig &input,
    scff_imaging::SWScaleConfig *output) {
  // enumは無理にキャストせずswitchで変換
  switch (input.flags) {
    case scff_interprocess::SWScaleFlags::kFastBilinear: {
      output->flags = scff_imaging::SWScaleFlags::kFastBilinear;
      break;
    }
    case scff_interprocess::SWScaleFlags::kBilinear: {
      output->flags = scff_imaging::SWScaleFlags::kBilinear;
      break;
    }
    case scff_interprocess::SWScaleFlags::kBicubic: {
      output->flags = scff_imaging::SWScaleFlags::kBicubic;
      break;
    }
    case scff_interprocess::SWScaleFlags::kX: {
      output->flags = scff_imaging::SWScaleFlags::kX;
      break;
    }
    case scff_interprocess::SWScaleFlags::kPoint: {
      output->flags = scff_imaging::SWScaleFlags::kPoint;
      break;
    }
    case scff_interprocess::SWScaleFlags::kArea: {
      output->flags = scff_imaging::SWScaleFlags::kArea;
      break;
    }
    case scff_interprocess::SWScaleFlags::kBicublin: {
      output->flags = scff_imaging::SWScaleFlags::kBicublin;
      break;
    }
    case scff_interprocess::SWScaleFlags::kGauss: {
      output->flags = scff_imaging::SWScaleFlags::kGauss;
      break;
    }
    case scff_interprocess::SWScaleFlags::kSinc: {
      output->flags = scff_imaging::SWScaleFlags::kSinc;
      break;
    }
    case scff_interprocess::SWScaleFlags::kLanczos: {
      output->flags = scff_imaging::SWScaleFlags::kLanczos;
      break;
    }
    case scff_interprocess::SWScaleFlags::kSpline: {
      output->flags = scff_imaging::SWScaleFlags::kSpline;
      break;
    }
    default: {
      ASSERT(false);
      output->flags = scff_imaging::SWScaleFlags::kFastBilinear;
      break;
    }
  }

  output->accurate_rnd = input.accurate_rnd != 0;
  output->is_filter_enabled = input.is_filter_enabled != 0;
  output->luma_gblur = input.luma_gblur;
  output->chroma_gblur = input.chroma_gblur;
  output->luma_sharpen = input.luma_sharpen;
  output->chroma_sharpen = input.chroma_sharpen;
  output->chroma_hshift = input.chroma_hshift;
  output->chroma_vshift = input.chroma_vshift;
}

/// モジュール間のLayoutParameterの変換
void ConvertLayoutParameter(
    const scff_interprocess::LayoutParameter &input,
    scff_imaging::LayoutParameter *output) {
  output->bound_x = input.bound_x;
  output->bound_y = input.bound_y;
  output->bound_width = input.bound_width;
  output->bound_height = input.bound_height;
  output->window = reinterpret_cast<HWND>(input.window);
  output->clipping_x = input.clipping_x;
  output->clipping_y = input.clipping_y;
  output->clipping_width = input.clipping_width;
  output->clipping_height = input.clipping_height;
  output->show_cursor = input.show_cursor != 0;
  output->show_layered_window = input.show_layered_window != 0;

  //-------------------------------------------------------------------
  // SWScaleConfigの変換
  ConvertSWScaleConfig(input.swscale_config, &(output->swscale_config));
  //-------------------------------------------------------------------

  output->stretch = input.stretch != 0;
  output->keep_aspect_ratio = input.keep_aspect_ratio != 0;

  // enumは無理にキャストせずswitchで変換
  switch (input.rotate_direction) {
    case scff_interprocess::RotateDirection::kNoRotate: {
      output->rotate_direction = scff_imaging::RotateDirection::kNoRotate;
      break;
    }
    case scff_interprocess::RotateDirection::k90Degrees: {
      output->rotate_direction = scff_imaging::RotateDirection::k90Degrees;
      break;
    }
    case scff_interprocess::RotateDirection::k180Degrees: {
      output->rotate_direction = scff_imaging::RotateDirection::k180Degrees;
      break;
    }
    case scff_interprocess::RotateDirection::k270Degrees: {
      output->rotate_direction = scff_imaging::RotateDirection::k270Degrees;
      break;
    }
    default: {
      ASSERT(false);
      output->rotate_direction = scff_imaging::RotateDirection::kNoRotate;
      break;
    }
  }
}

/// MessageからLayoutParameterへの変換
void MessageToLayoutParameter(
    const scff_interprocess::Message &message,
    int index,
    scff_imaging::LayoutParameter *parameter) {
  ASSERT(0 <= index && index < message.layout_element_count);
  ConvertLayoutParameter(message.layout_parameters[index], parameter);
}

}   // namespace

scff_imaging::Request* SCFFMonitor::CreateRequest() {
  // 前回のCreateRequestからの経過時間(Sec)
  const clock_t now = clock();
  const double erapsed_time_from_last_polling =
      static_cast<double>(now - last_polling_clock_) / CLOCKS_PER_SEC;

  // ポーリングはkSCFFMonitorPollingInterval秒に1回である
  if (erapsed_time_from_last_polling < kSCFFMonitorPollingInterval) {
    return nullptr;
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
  if (message.timestamp <= 0LL) {
    return 0;
  }

  // 前回処理したタイムスタンプより前、あるいは同じ場合は何もしない
  if (message.timestamp <= last_message_timestamp_) {
    return 0;
  }

  // タイムスタンプを進めておく
  last_message_timestamp_ = message.timestamp;

  //-----------------------------------------------------------------
  /// @warning int32_t->enum
  scff_interprocess::LayoutType layout_type =
      static_cast<scff_interprocess::LayoutType>(message.layout_type);

  //-----------------------------------------------------------------
  // ResetLayoutRequest
  //-----------------------------------------------------------------
  if (layout_type == scff_interprocess::LayoutType::kNullLayout) {
    /// @todo(me) %lldではなく%"PRId64"が適切だがコンパイルエラーになる
    MyDbgLog((LOG_TRACE, kDbgImportant,
              TEXT("SCFFMonitor: ResetLayoutRequest arrived(%lld)."),
              message.timestamp));
    return new scff_imaging::ResetLayoutRequest();
  }

  //-----------------------------------------------------------------
  // SetLayoutRequest
  //-----------------------------------------------------------------
  ASSERT(layout_type == scff_interprocess::LayoutType::kNativeLayout ||
         layout_type == scff_interprocess::LayoutType::kComplexLayout);

  /// @todo(me) %lldではなく%"PRId64"が適切だがコンパイルエラーになる
  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("SCFFMonitor: SetLayoutRequest arrived(%d, %lld)."),
            message.layout_type,
            message.timestamp));

  // パラメータをメッセージから抽出
  scff_imaging::LayoutParameter parameters[scff_imaging::kMaxProcessorSize];
  for (int i = 0; i < message.layout_element_count; i++) {
    MessageToLayoutParameter(message, i, &(parameters[i]));
  }
  return new scff_imaging::SetLayoutRequest(
      message.layout_element_count,
      parameters);
}

void SCFFMonitor::ReleaseRequest(scff_imaging::Request *request) {
  if (request == nullptr) {
    // nullptrなら何もしない
    return;
  } else {
    delete request;
    request = nullptr;
  }
}
