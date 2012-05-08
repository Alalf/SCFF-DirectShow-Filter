
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
      can_use_drawutils_(false),
      stretch_(stretch),
      keep_aspect_ratio_(keep_aspect_ratio),
      padding_left_(-1),    // ありえない値
      padding_right_(-1),   // ありえない値
      padding_top_(-1),     // ありえない値
      padding_bottom_(-1),  // ありえない値
      screen_capture_(0),   // NULL
      capture_image_() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("NativeLayout: NEW(%d, %d, %d, stretch:%d, keep-aspect:%d)"),
          pixel_format, width, height, stretch, keep_aspect_ratio));
  // 配列の初期化
  rgba_padding_color_[0] = 0;
  rgba_padding_color_[1] = 0;
  rgba_padding_color_[2] = 0;
  rgba_padding_color_[3] = 0;
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

  /// @warning 2012/05/08現在drawutilsはPlaner Formatにしか対応していない
  switch (pixel_format()) {
  case kI420:
  case kRGB0:
    can_use_drawutils_ = true;
    break;
  case kUYVY:
    can_use_drawutils_ = false;
    break;
  }

  // パディング用の変数の設定
  int capture_width = width();
  int capture_height = height();
  if (can_use_drawutils_) {
    // パディング用のコンテキスト・カラーの初期化
    const int error_init =
        ff_draw_init(&draw_context_,
                     Utilities::ToAVPicturePixelFormat(pixel_format()),
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
        width(), height(),
        parameter_.clipping_width, parameter_.clipping_height,
        stretch_, keep_aspect_ratio_,
        &padding_top_, &padding_bottom_,
        &padding_left_, &padding_right_);
    ASSERT(no_error);

    // パディング分だけサイズを小さくする
    capture_width -= padding_left_ + padding_right_;
    capture_height -= padding_top_ + padding_bottom_;
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

  // drawutilsがサポートしていないフォーマットはパディング非対応
  if (!can_use_drawutils_) {
    // スクリーンキャプチャ
    const ErrorCode error =
        screen_capture_->PullAVPictureImage(image);
    if (error != kNoError) {
      return ErrorOccured(error);
    }
    return NoError();
  }

  // 以下drawutilsによるパディング

  // スクリーンキャプチャ
  const ErrorCode error = screen_capture_->PullAVPictureImage(&capture_image_);
  if (error != kNoError) {
    return ErrorOccured(error);
  }

  // 上の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    image->avpicture()->data,
                    image->avpicture()->linesize,
                    0, 0, width(), padding_top_);

  // 中央に画像を配置する
  ff_copy_rectangle2(&draw_context_,
                     image->avpicture()->data,
                     image->avpicture()->linesize,
                     capture_image_.avpicture()->data,
                     capture_image_.avpicture()->linesize,
                     padding_left_, padding_top_,
                     0, 0, 
                     capture_image_.width(),
                     capture_image_.height());

  // 下の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    image->avpicture()->data,
                    image->avpicture()->linesize,
                    0,
                    padding_top_ + capture_image_.height(),
                    width(),
                    padding_bottom_);

  // 左の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    image->avpicture()->data,
                    image->avpicture()->linesize,
                    0, padding_top_,
                    padding_left_, capture_image_.height());

  // 右の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    image->avpicture()->data,
                    image->avpicture()->linesize,
                    padding_left_ + capture_image_.width(), padding_top_,
                    padding_right_, capture_image_.height());

  return NoError();
}
}   // namespace imaging
