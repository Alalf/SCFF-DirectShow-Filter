
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

/// @file imaging/native-layout.cc
/// @brief imaging::NativeLayoutの定義

#include "imaging/native-layout.h"

#include <math.h>
#include <algorithm>

#include "base/debug.h"
#include "imaging/image.h"
#include "imaging/avpicture-image.h"
#include "imaging/screen-capture.h"
#include "imaging/utilities.h"

namespace imaging {

//=====================================================================
// imaging::NativeLayout
//=====================================================================

// コンストラクタ
NativeLayout::NativeLayout(
    const ScreenCaptureParameter &parameter,
    bool stretch, bool keep_aspect_ratio)
    : Processor<AVPictureImage, AVPictureImage>(),
      parameter_(parameter),
      can_use_drawutils_(false),
      stretch_(stretch),
      keep_aspect_ratio_(keep_aspect_ratio),
      scaler_(0),           // NULL
      padding_left_(-1),    // ありえない値
      padding_right_(-1),   // ありえない値
      padding_top_(-1),     // ありえない値
      padding_bottom_(-1),  // ありえない値
      screen_capture_(0) {  // NULL
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("NativeLayout: NEW(stretch:%d, keep-aspect:%d)"),
          stretch, keep_aspect_ratio));
  // 配列の初期化
  rgba_padding_color_[0] = 0;
  rgba_padding_color_[1] = 0;
  rgba_padding_color_[2] = 0;
  rgba_padding_color_[3] = 0;
  // 明示的に初期化していない
  // captured_image_
  // converted_image_
}

// デストラクタ
NativeLayout::~NativeLayout() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("NativeLayout: DELETE")));
  // 管理しているインスタンスをすべて破棄
  // 破棄はプロセッサ→イメージの順
  if (screen_capture_ != 0) {
    delete screen_capture_;
  }
}

// Processor::Init
ErrorCode NativeLayout::Init() {
  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("NativeLayout: Init")));


  /// @warning 2012/05/08現在drawutilsはPlaner Formatにしか対応していない
  switch (GetOutputImage()->pixel_format()) {
  case kI420:
  case kRGB0:
    can_use_drawutils_ = true;
    break;
  case kUYVY:
    can_use_drawutils_ = false;
    break;
  }

  // パディング用の変数の設定
  int converted_width = GetOutputImage()->width();
  int converted_height = GetOutputImage()->height();
  if (can_use_drawutils_) {
    // パディング用のコンテキスト・カラーの初期化
    const int error_init =
        ff_draw_init(&draw_context_,
                     Utilities::ToAVPicturePixelFormat(GetOutputImage()->pixel_format()),
                     0);
    ASSERT(error_init == 0);
    rgba_padding_color_[0] = 0;
    rgba_padding_color_[1] = 0;
    rgba_padding_color_[2] = 0;
    rgba_padding_color_[3] = 0;
    ff_draw_color(&draw_context_,
                  &padding_color_,
                  rgba_padding_color_);

    // パディングサイズの計算
    const bool no_error = Utilities::CalculatePaddingSize(
        GetOutputImage()->width(), GetOutputImage()->height(),
        parameter_.clipping_width, parameter_.clipping_height,
        stretch_, keep_aspect_ratio_,
        &padding_top_, &padding_bottom_,
        &padding_left_, &padding_right_);
    ASSERT(no_error);

    // パディング分だけサイズを小さくする
    converted_width -= padding_left_ + padding_right_;
    converted_height -= padding_top_ + padding_bottom_;
  }

  // 拡大縮小用のコンテキストを作成
  const int captured_width = parameter_.clipping_width;
  const int captured_height = parameter_.clipping_height;

  struct SwsContext *scaler = 0;    // NULL
  PixelFormat input_pixel_format = PIX_FMT_NONE;
  switch (GetOutputImage()->pixel_format()) {
  case kI420:
  case kUYVY:
    // I420/UYVY: 入力:BGR0(32bit) 出力:I420(12bit)/UYVY(16bit)
    /// @attention RGB->YUV変換時にUVが逆になるのを修正
    ///- RGBデータをBGRデータとしてSwsContextに渡してあります
    input_pixel_format = PIX_FMT_BGR0;
    break;
  case kRGB0:
    // RGB0: 入力:RGB0(32bit) 出力:RGB0(32bit)
    input_pixel_format = PIX_FMT_RGB0;
    break;
  }
  scaler = sws_getCachedContext(NULL,
      captured_width, captured_height, input_pixel_format,
      converted_width, converted_height,
      Utilities::ToAVPicturePixelFormat(GetOutputImage()->pixel_format()),
      parameter_.sws_flags, NULL, NULL, NULL);
  if (scaler == NULL) {
    return ErrorOccured(kOutOfMemoryError);
  }
  scaler_ = scaler;

  //-------------------------------------------------------------------
  // 初期化の順番はイメージ→プロセッサの順
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  // GetDIBits用なのでRGB0で作成
  const ErrorCode error_captured_image =
      captured_image_.Create(kRGB0,
                             parameter_.clipping_width,
                             parameter_.clipping_height);
  if (error_captured_image != kNoError) {
    return ErrorOccured(error_captured_image);
  }
  // ピクセル形式変換＋拡大縮小後のイメージ
  const ErrorCode error_converted_image =
      converted_image_.Create(GetOutputImage()->pixel_format(),
                              converted_width,
                              converted_height);
  if (error_converted_image != kNoError) {
    return ErrorOccured(error_converted_image);
  }
  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  ScreenCaptureParameter parameter_array[kMaxMultiProcessorSize];
  parameter_array[0] = parameter_;
  // NativeLayoutなのでsize=1
  ScreenCapture *screen_capture = new ScreenCapture(1, parameter_array);
  screen_capture->SetOutputImage(&captured_image_);
  const ErrorCode error_screen_capture = screen_capture->Init();
  if (error_screen_capture != kNoError) {
    delete screen_capture;
    return ErrorOccured(error_screen_capture);
  }
  screen_capture_ = screen_capture;
  //-------------------------------------------------------------------

  return InitDone();
}

// Processor::Run
ErrorCode NativeLayout::Run() {
  if (GetCurrentError() != kNoError) {
    // 何かエラーが発生している場合は何もしない
    return GetCurrentError();
  }

  ASSERT(screen_capture_ != 0);   // NULL
  ASSERT(GetOutputImage() != 0);             // NULL

  // まずはスクリーンキャプチャを行って変換
  const ErrorCode error = screen_capture_->Run();
  if (error != kNoError) {
    return ErrorOccured(error);
  }

  int scale_height = -1;    // ありえない値
  switch (GetOutputImage()->pixel_format()) {
  case kI420:
  case kUYVY:
    /// @attention RGB->YUV変換時に上下が逆になるのを修正
    AVPicture flip_horizontal_image_for_swscale;
    Utilities::FlipHorizontal(
        GetInputImage()->avpicture(),
        GetInputImage()->height(),
        &flip_horizontal_image_for_swscale);

    // 拡大縮小
    scale_height =
        sws_scale(scaler_,
                  flip_horizontal_image_for_swscale.data,
                  flip_horizontal_image_for_swscale.linesize,
                  0, GetInputImage()->height(),
                  GetOutputImage()->avpicture()->data,
                  GetOutputImage()->avpicture()->linesize);
    ASSERT(scale_height == GetOutputImage()->height());
    break;
  case kRGB0:
    // 拡大縮小
    scale_height =
        sws_scale(scaler_,
                  GetInputImage()->avpicture()->data,
                  GetInputImage()->avpicture()->linesize,
                  0, GetInputImage()->height(),
                  GetOutputImage()->avpicture()->data,
                  GetOutputImage()->avpicture()->linesize);
    ASSERT(scale_height == GetOutputImage()->height());
    break;
  }

  // drawutilsがサポートしていないフォーマットはパディング非対応
  if (!can_use_drawutils_) {
    // スクリーンキャプチャ
    const ErrorCode error = screen_capture_->Run();
    if (error != kNoError) {
      return ErrorOccured(error);
    }
    return NoError();
  }

  // 以下drawutilsによるパディング

  // スクリーンキャプチャ
  const ErrorCode error_capture = screen_capture_->Run();
  if (error_capture != kNoError) {
    return ErrorOccured(error_capture);
  }

  // パディング
  Utilities::PadImage(&draw_context_,
                      &padding_color_,
                      GetInputImage()->avpicture(),
                      GetInputImage()->width(),
                      GetInputImage()->height(),
                      padding_left_, padding_right_,
                      padding_top_, padding_bottom_,
                      GetOutputImage()->width(),
                      GetOutputImage()->height(),
                      GetOutputImage()->avpicture());

  return NoError();
}
}   // namespace imaging
