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

/// @file scff-imaging/raw-bitmap-image.cc
/// scff_imaging::RawBitmapImageの定義

#include "scff-imaging/raw-bitmap-image.h"

extern "C" {
#include <libavcodec/avcodec.h>
}

#include "scff-imaging/debug.h"
#include "scff-imaging/imaging-types.h"
#include "scff-imaging/utilities.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::RawBitmapImage
//=====================================================================

// コンストラクタ
RawBitmapImage::RawBitmapImage()
    : Image(),
      raw_bitmap_(nullptr) {
  /// @attention raw_bitmap_そのものの構築はCreate()で行う
}

// デストラクタ
RawBitmapImage::~RawBitmapImage() {
  if (!IsEmpty()) {
    // av_freepで解放
    av_freep(raw_bitmap());
  }
}

// Create()などによって実体がまだ生成されていない場合
bool RawBitmapImage::IsEmpty() const {
  return raw_bitmap_ == nullptr;
}

// 実態を作る
ErrorCode RawBitmapImage::Create(ImagePixelFormat pixel_format,
                                 int width, int height) {
  // pixel_format, width, heightを設定する
  ErrorCode error_create = Image::Create(pixel_format, width, height);
  if (error_create != ErrorCode::kNoError) {
    return error_create;
  }

  // 取り込み用バッファを作成
  int size = Utilities::CalculateDataSize(pixel_format, width, height);
  uint8_t *raw_bitmap = static_cast<uint8_t*>(av_malloc(size));
  if (raw_bitmap == nullptr) {
    return ErrorCode::kRawBitmapImageOutOfMemoryError;
  }
  raw_bitmap_ = raw_bitmap;

  return ErrorCode::kNoError;
}

// Getter: 各種ビットマップ
uint8_t* RawBitmapImage::raw_bitmap() const {
  return raw_bitmap_;
}
}   // namespace scff_imaging
