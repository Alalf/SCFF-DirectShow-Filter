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

/// @file scff-imaging/avpicture-with-fill-image.cc
/// scff_imaging::AVPictureWithFillImageの定義

#include "scff-imaging/avpicture-with-fill-image.h"

extern "C" {
#include <libavcodec/avcodec.h>
}

#include "scff-imaging/debug.h"
#include "scff-imaging/imaging-types.h"
#include "scff-imaging/utilities.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::AVPictureWithFillImage
//=====================================================================

AVPictureWithFillImage::AVPictureWithFillImage()
    : Image(),
      raw_bitmap_(nullptr),
      avpicture_(nullptr) {
  /// @attention avpicture_そのものの構築はCreateで行う
}

AVPictureWithFillImage::~AVPictureWithFillImage() {
  if (!IsEmpty()) {
    /// @attention avpicture_fillによって
    ///            関連付けられたメモリ領域も解放してくれる
    avpicture_free(avpicture_);
  }
}

bool AVPictureWithFillImage::IsEmpty() const {
  return avpicture_ == nullptr;
}

ErrorCode AVPictureWithFillImage::Create(ImagePixelFormat pixel_format,
                                       int width, int height) {
  // pixel_format, width, heightを設定する
  ErrorCode error_create = Image::Create(pixel_format, width, height);
  if (error_create != ErrorCode::kNoError) {
    return error_create;
  }

  // RawBitmapを作成
  int size = Utilities::CalculateDataSize(pixel_format, width, height);
  uint8_t *raw_bitmap = static_cast<uint8_t*>(av_malloc(size));
  if (raw_bitmap == nullptr) {
    return ErrorCode::kAVPictureWithFillImageOutOfMemoryError;
  }

  // 取り込み用AVPictureを作成
  AVPicture *avpicture = new AVPicture();
  if (avpicture == nullptr) {
    av_freep(raw_bitmap);
    return ErrorCode::kAVPictureWithFillImageCannotCreateAVPictureError;
  }

  // 取り込みバッファとAVPictureを関連付け
  int result_fill =
      avpicture_fill(avpicture, raw_bitmap,
                     av_pixel_format(),
                     width, height);
  if (result_fill != size) {
    av_freep(raw_bitmap);
    return ErrorCode::kAVPictureWithFillImageCannotFillError;
  }

  avpicture_ = avpicture;
  raw_bitmap_ = raw_bitmap;

  return ErrorCode::kNoError;
}

AVPicture* AVPictureWithFillImage::avpicture() const {
  return avpicture_;
}

uint8_t* AVPictureWithFillImage::raw_bitmap() const {
  return raw_bitmap_;
}
}   // namespace scff_imaging
