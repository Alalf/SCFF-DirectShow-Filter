
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

/// @file imaging/avpicture-with-fill-image.cc
/// @brief imaging::AVPictureWithFillImageの定義

#include "imaging/avpicture-with-fill-image.h"

extern "C" {
#include <libavcodec/avcodec.h>
}

#include "base/debug.h"
#include "imaging/utilities.h"

namespace imaging {

//=====================================================================
// imaging::AVPictureWithFillImage
//=====================================================================

// コンストラクタ
AVPictureWithFillImage::AVPictureWithFillImage()
    : Image(),
      raw_bitmap_(0),   // NULL
      avpicture_(0) {   // NULL
  /// @attention avpicture_そのものの構築はCreateで行う
}

// デストラクタ
AVPictureWithFillImage::~AVPictureWithFillImage() {
  if (!IsEmpty()) {   // NULL
    /// @attention avpicture_fillによって
    /// @attention 関連付けられたメモリ領域も解放してくれる
    avpicture_free(avpicture_);
  }
}

// Create()などによって実体がまだ生成されていない場合
bool AVPictureWithFillImage::IsEmpty() const {
  return avpicture_ == 0;   // NULL
}

// AVPictureと同時にRawBitmapの実体を作成する
ErrorCode AVPictureWithFillImage::Create(ImagePixelFormat pixel_format,
                                       int width, int height) {
  // pixel_format, width, heightを設定する
  ErrorCode error_create = Image::Create(pixel_format, width, height);
  if (error_create != kNoError) {
    return error_create;
  }

  // RawBitmapを作成
  int size = Utilities::CalcDataSize(pixel_format, width, height);
  uint8_t *raw_bitmap = static_cast<uint8_t*>(av_mallocz(size));
  if (raw_bitmap == NULL) {
    return kOutOfMemoryError;
  }

  // 取り込み用AVPictureを作成
  AVPicture *avpicture = new AVPicture();
  if (avpicture == 0) {    // NULL
    av_freep(raw_bitmap);
    return kOutOfMemoryError;
  }

  // 取り込みバッファとAVPictureを関連付け
  int result_fill =
      avpicture_fill(avpicture, raw_bitmap,
                     Utilities::ToAVPicturePixelFormat(pixel_format),
                     width, height);
  if (result_fill != size) {
    av_freep(raw_bitmap);
    return kOutOfMemoryError;
  }

  avpicture_ = avpicture;
  raw_bitmap_ = raw_bitmap;

  return kNoError;
}

// Getter: AVPictureへのポインタ
AVPicture* AVPictureWithFillImage::avpicture() const {
  return avpicture_;
}

// Getter: 各種ビットマップ
uint8_t* AVPictureWithFillImage::raw_bitmap() const {
  return raw_bitmap_;
}
}   // namespace imaging
