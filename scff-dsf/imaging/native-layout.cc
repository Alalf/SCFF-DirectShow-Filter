
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
NativeLayout::NativeLayout(ImagePixelFormat pixel_format,
              int width, int height,
              const ScreenCaptureParameter &parameter,
              bool stretch, bool keep_aspect_ratio)
    : Processor(pixel_format, width, height),
      parameter_(parameter),
      stretch_(stretch),
      keep_aspect_ratio_(keep_aspect_ratio),
      padding_left_(-1),    // ありえない値
      padding_right_(-1),   // ありえない値
      padding_top_(-1),     // ありえない値
      padding_bottom_(-1),  // ありえない値
      screen_capture_(0),   // NULL
      capture_image_() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("NativeLayout: NEW(%d,%d,stretch:%d,keep-aspect:%d)"),
          width, height, stretch, keep_aspect_ratio));
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

// 初期化
ErrorCode NativeLayout::Init() {
  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("NativeLayout: Init")));

  /// @todo(me) 他の形式でもPadding対応する
  int capture_width = width();
  int capture_height = height();
  padding_top_ = 0;
  padding_bottom_ = 0;
  padding_left_ = 0;
  padding_right_ = 0;

  /// @warning YUV420Pのみに対応
  bool error = false;
  switch (pixel_format()) {
  case kI420:
    error = Utilities::CalculatePaddingSize(
        width(), height(),
        parameter_.clipping_width, parameter_.clipping_height,
        stretch_, keep_aspect_ratio_,
        &padding_top_, &padding_bottom_,
        &padding_left_, &padding_right_);
    ASSERT(error == true);

    capture_width = width() - padding_left_ - padding_right_;
    capture_height = height() - padding_top_ - padding_bottom_;
    break;
  case kUYVY:
  case kRGB0:
    // nop
    break;
  }

  //-------------------------------------------------------------------
  // 初期化の順番はイメージ→プロセッサの順
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  const ErrorCode error_image =
      capture_image_.Create(pixel_format(), capture_width, capture_height);
  if (error_image != kNoError) {
    return ErrorOccured(error_image);
  }
  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  ScreenCapture *screen_capture =
      new ScreenCapture(pixel_format(), capture_width,
                        capture_height, parameter_);
  const ErrorCode error_screen_capture = screen_capture->Init();
  if (error_screen_capture != kNoError) {
    delete screen_capture;
    return ErrorOccured(error_screen_capture);
  }
  screen_capture_ = screen_capture;
  //-------------------------------------------------------------------

  return InitDone();
}

// リクエストに対する処理を行う
ErrorCode NativeLayout::Accept(Request *request) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("NativeLayout: Accept")));

  if (GetCurrentError() != kNoError) {
    // 何かエラーが発生している場合は何もしない
    return GetCurrentError();
  }

  /// @todo(me) ここでいろいろやる

  ASSERT(screen_capture_ != 0);   // NULL
  const ErrorCode error = screen_capture_->Accept(request);
  if (error != kNoError) {
    return ErrorOccured(error);
  } else {
    return NoError();
  }
}

// 渡されたポインタにビットマップデータを設定する
ErrorCode NativeLayout::PullAVPictureImage(AVPictureImage *image) {
  if (GetCurrentError() != kNoError) {
    // 何かエラーが発生している場合は何もしない
    return GetCurrentError();
  }

  ASSERT(screen_capture_ != 0);   // NULL
  ASSERT(image != 0);             // NULL

  /// @warning YUV420Pのみに対応
  ErrorCode error = kNoError;
  int error_pad = -1;
  static int padding_yuv_color[3] = {0, 128, 128};
  switch (pixel_format()) {
  case kI420:
    error = screen_capture_->PullAVPictureImage(&capture_image_);
    if (error != kNoError) {
      return ErrorOccured(error);
    }
    // YUV形式の黒色(PCスケール)で埋める
    error_pad =
        av_picture_pad(image->avpicture(), capture_image_.avpicture(),
                       height(), width(),
                       Utilities::ToAVPicturePixelFormat(pixel_format()),
                       padding_top_, padding_bottom_,
                       padding_left_, padding_right_,
                       padding_yuv_color);
    ASSERT(error_pad == 0);
    break;
  case kUYVY:
  case kRGB0:
    error = screen_capture_->PullAVPictureImage(image);
    if (error != kNoError) {
      return ErrorOccured(error);
    }
    break;
  }

  return NoError();
}
}   // namespace imaging
