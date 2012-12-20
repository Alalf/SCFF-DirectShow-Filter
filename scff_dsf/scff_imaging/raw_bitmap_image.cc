// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

/// @file scff_imaging/raw_bitmap_image.cc
/// scff_imaging::RawBitmapImageの定義

#include "scff_imaging/raw_bitmap_image.h"

extern "C" {
#include <libavcodec/avcodec.h>
}

#include "scff_imaging/debug.h"
#include "scff_imaging/imaging_types.h"
#include "scff_imaging/utilities.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::RawBitmapImage
//=====================================================================

RawBitmapImage::RawBitmapImage()
    : Image(),
      raw_bitmap_(nullptr) {
  /// @attention raw_bitmap_そのものの構築はCreate()で行う
}

RawBitmapImage::~RawBitmapImage() {
  if (!IsEmpty()) {
    // av_freepで解放
    av_freep(raw_bitmap());
  }
}

bool RawBitmapImage::IsEmpty() const {
  return raw_bitmap_ == nullptr;
}

ErrorCodes RawBitmapImage::Create(ImagePixelFormats pixel_format,
                                  int width, int height) {
  // pixel_format, width, heightを設定する
  ErrorCodes error_create = Image::Create(pixel_format, width, height);
  if (error_create != ErrorCodes::kNoError) {
    return error_create;
  }

  // 取り込み用バッファを作成
  int size = utilities::CalculateDataSize(pixel_format, width, height);
  uint8_t *raw_bitmap = static_cast<uint8_t*>(av_malloc(size));
  if (raw_bitmap == nullptr) {
    return ErrorCodes::kRawBitmapImageOutOfMemoryError;
  }
  raw_bitmap_ = raw_bitmap;

  return ErrorCodes::kNoError;
}

uint8_t* RawBitmapImage::raw_bitmap() const {
  return raw_bitmap_;
}
}   // namespace scff_imaging
