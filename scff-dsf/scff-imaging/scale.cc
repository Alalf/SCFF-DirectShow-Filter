
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

/// @file scff-imaging/scale.cc
/// @brief scff_imaging::Scaleの定義

#include "scff-imaging/scale.h"

extern "C" {
#include <libswscale/swscale.h>
}

#include "scff-imaging/utilities.h"
#include "scff-imaging/avpicture-image.h"
#include "scff-imaging/avpicture-with-fill-image.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::Scale
//=====================================================================

/// @brief コンストラクタ
Scale::Scale(SWScaleFlags sws_flags)
    : Processor<AVPictureWithFillImage, AVPictureImage>(),
      sws_flags_(sws_flags),
      scaler_(0) {  // NULL
  // nop
}

/// @brief デストラクタ
Scale::~Scale() {
  if (scaler_ != 0) {   // NULL
    sws_freeContext(scaler_);
  }
}

//-------------------------------------------------------------------
// Processor::Init
ErrorCode Scale::Init() {
  // 入力はRGB0限定
  ASSERT(GetInputImage()->pixel_format() == kRGB0);

  // 拡大縮小用のコンテキストを作成
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
      GetInputImage()->width(),
      GetInputImage()->height(),
      input_pixel_format,
      GetOutputImage()->width(),
      GetOutputImage()->height(),
      GetOutputImage()->avpicture_pixel_format(),
      sws_flags_, NULL, NULL, NULL);
  if (scaler == NULL) {
    return ErrorOccured(kOutOfMemoryError);
  }
  scaler_ = scaler;

  // 初期化は成功
  return InitDone();
}

// Processor::Run
ErrorCode Scale::Run() {
  // SWScaleを使って拡大・縮小を行う
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

  // エラー発生なし
  return NoError();
}
//-------------------------------------------------------------------
}   // namespace scff_imaging
