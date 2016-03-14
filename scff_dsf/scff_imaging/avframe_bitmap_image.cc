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

/// @file scff_imaging/avframe_bitmap_image.cc
/// scff_imaging::AVFrameBitmapImageの定義

#include "scff_imaging/avframe_bitmap_image.h"

extern "C" {
#include <libavutil/frame.h>
#include <libavutil/imgutils.h>
}

#include "scff_imaging/debug.h"
#include "scff_imaging/imaging_types.h"
#include "scff_imaging/utilities.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::AVFrameBitmapImage
//=====================================================================

AVFrameBitmapImage::AVFrameBitmapImage()
    : Image(),
      bitmap_(nullptr),
      avframe_(nullptr) {
  /// @attention avframe_そのものの構築はCreateで行う
}

AVFrameBitmapImage::~AVFrameBitmapImage() {
  if (!IsEmpty()) {
    /// @attention av_image_fill_arraysによって
    ///            関連付けられたメモリ領域も解放してくれる
    av_frame_free(&avframe_);
  }
}

bool AVFrameBitmapImage::IsEmpty() const {
  return avframe_ == nullptr;
}

ErrorCodes AVFrameBitmapImage::Create(ImagePixelFormats pixel_format,
                                      int width, int height) {
  // pixel_format, width, heightを設定する
  ErrorCodes error_create = Image::Create(pixel_format, width, height);
  if (error_create != ErrorCodes::kNoError) {
    return error_create;
  }

  // 取り込み用AVFrameを作成
  AVFrame *avframe = av_frame_alloc();
  avframe->width = width;
  avframe->height = height;
  avframe->format = av_pixel_format();

  // RawBitmapを作成
  const int size = utilities::CalculateDataSize(pixel_format, width, height);
  uint8_t *bitmap = static_cast<uint8_t*>(av_malloc(size));
  if (bitmap == nullptr) {
    av_frame_free(&avframe);
    return ErrorCodes::kAVFrameBitmapImageOutOfMemoryError;
  }

  // 取り込みバッファとAVFrameを関連付け
  const int result = av_image_fill_arrays(avframe->data, avframe->linesize,
                                          bitmap, av_pixel_format(),
                                          width, height, 1);
  if (result != size) {
    av_freep(bitmap);
    av_frame_free(&avframe);
    return ErrorCodes::kAVFrameBitmapImageCannotFillError;
  }

  avframe_ = avframe;
  bitmap_ = bitmap;

  return ErrorCodes::kNoError;
}

AVFrame* AVFrameBitmapImage::avframe() const {
  return avframe_;
}

uint8_t* AVFrameBitmapImage::bitmap() const {
  return bitmap_;
}
}   // namespace scff_imaging
