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

/// @file scff_imaging/avframe_image.cc
/// scff_imaging::AVFrameImageの定義

#include "scff_imaging/avframe_image.h"

extern "C" {
#include <libavutil/frame.h>
#include <libavutil/imgutils.h>
}

#include "scff_imaging/debug.h"
#include "scff_imaging/imaging_types.h"
#include "scff_imaging/utilities.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::AVFrameImage
//=====================================================================

AVFrameImage::AVFrameImage()
    : Image(),
      avframe_(nullptr) {
  /// @attention avframe_そのものの構築はCreateで行う
}

AVFrameImage::~AVFrameImage() {
  if (!IsEmpty()) {
    if (avframe_->data) {
      av_freep(&avframe_->data[0]);
    }
    av_frame_free(&avframe_);
  }
}

bool AVFrameImage::IsEmpty() const {
  return avframe_ == nullptr;
}

ErrorCodes AVFrameImage::Create(ImagePixelFormats pixel_format,
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

  const int result_alloc = av_image_alloc(avframe->data,
                                          avframe->linesize,
                                          width, height, av_pixel_format(), 1);
  if (result_alloc == 0) {
    return ErrorCodes::kAVFrameImageOutOfMemoryError;
  }

  avframe_ = avframe;
  return ErrorCodes::kNoError;
}

AVFrame* AVFrameImage::avframe() const {
  return avframe_;
}
}   // namespace scff_imaging
