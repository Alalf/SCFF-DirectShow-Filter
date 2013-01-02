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

/// @file scff_imaging/scale.cc
/// scff_imaging::Scaleの定義

#include "scff_imaging/scale.h"

extern "C" {
#include <libswscale/swscale.h>
}

#include "scff_imaging/utilities.h"
#include "scff_imaging/avpicture_image.h"
#include "scff_imaging/avpicture_with_fill_image.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::Scale
//=====================================================================

Scale::Scale(const SWScaleConfig &swscale_config)
    : Processor<AVPictureWithFillImage, AVPictureImage>(),
      swscale_config_(swscale_config),
      filter_(nullptr),
      scaler_(nullptr) {
  // nop
}

Scale::~Scale() {
  if (filter_ != nullptr) {
    sws_freeFilter(filter_);
  }
  if (scaler_ != nullptr) {
    sws_freeContext(scaler_);
  }
}

//-------------------------------------------------------------------

ErrorCodes Scale::Init() {
  // 入力はRGB0限定
  ASSERT(GetInputImage()->pixel_format() == ImagePixelFormats::kRGB0);

  // 拡大縮小時のフィルタを作成
  SwsFilter *filter = sws_getDefaultFilter(
      swscale_config_.luma_gblur,
      swscale_config_.chroma_gblur,
      swscale_config_.luma_sharpen,
      swscale_config_.chroma_sharpen,
      swscale_config_.chroma_hshift,
      swscale_config_.chroma_vshift,
      0);
  if (filter == nullptr) {
    return ErrorOccured(ErrorCodes::kScaleCannotGetDefaultFilterError);
  }
  filter_ = filter;

  //-------------------------------------------------------------------
  // 拡大縮小用のコンテキストを作成
  //-------------------------------------------------------------------
  struct SwsContext *scaler = nullptr;

  // ピクセルフォーマットの調整
  AVPixelFormat input_pixel_format = AV_PIX_FMT_NONE;
  switch (GetOutputImage()->pixel_format()) {
    case ImagePixelFormats::kI420:
    case ImagePixelFormats::kIYUV:
    case ImagePixelFormats::kUYVY:
    case ImagePixelFormats::kYUY2: {
      // IYUV/I420/YUY2/UYVY:
      //    入力:BGR0(32bit)
      //    出力:I420(12bit)/IYUV(12bit)/UYVY(16bit)/YUY2(16bit)
      /// @attention RGB->YUV変換時にUVが逆になるのを修正
      /// - RGBデータをBGRデータとしてSwsContextに渡してあります
      input_pixel_format = AV_PIX_FMT_BGR0;
      break;
    }
    case ImagePixelFormats::kYV12:
    case ImagePixelFormats::kRGB0: {
      // YV12/RGB0:
      //    入力:RGB0(32bit)
      //    出力:YV12(12bit)/RGB0(32bit)
      input_pixel_format = AV_PIX_FMT_RGB0;
      break;
    }
  }

  // フィルタの設定
  SwsFilter *src_filter = nullptr;
  if (swscale_config_.is_filter_enabled) {
    src_filter = filter_;
  }

  // 丸め処理
  /// @attention enum->int
  int flags = static_cast<int>(swscale_config_.flags);
  if (swscale_config_.accurate_rnd) {
    flags |= SWS_ACCURATE_RND;
  }

  // SWScalerの作成
  scaler = sws_getCachedContext(nullptr,
      GetInputImage()->width(),
      GetInputImage()->height(),
      input_pixel_format,
      GetOutputImage()->width(),
      GetOutputImage()->height(),
      GetOutputImage()->av_pixel_format(),
      flags, src_filter, nullptr, nullptr);
  if (scaler == nullptr) {
    return ErrorOccured(ErrorCodes::kScaleCannotGetContextError);
  }
  scaler_ = scaler;

  // 初期化は成功
  return InitDone();
}

ErrorCodes Scale::Run() {
  // SWScaleを使って拡大・縮小を行う
  int scale_height =
      sws_scale(scaler_,
                GetInputImage()->avpicture()->data,
                GetInputImage()->avpicture()->linesize,
                0, GetInputImage()->height(),
                GetOutputImage()->avpicture()->data,
                GetOutputImage()->avpicture()->linesize);
  ASSERT(scale_height == GetOutputImage()->height());

  // エラー発生なし
  return GetCurrentError();
}

}   // namespace scff_imaging
