﻿// Copyright 2012-2013 Alalf <alalf.iQLc_at_gmail.com>
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

/// @file scff_imaging/avframe_bitmap_image.h
/// scff_imaging::AVFrameBitmapImageの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_AVFRAME_BITMAP_IMAGE_H_
#define SCFF_DSF_SCFF_IMAGING_AVFRAME_BITMAP_IMAGE_H_

#include "scff_imaging/common.h"
#include "scff_imaging/image.h"

namespace scff_imaging {

/// AVFrame(ffmpeg)の実体を管理するクラス
class AVFrameBitmapImage: public Image {
 public:
  /// コンストラクタ
  AVFrameBitmapImage();
  /// デストラクタ
  ~AVFrameBitmapImage();

  //-------------------------------------------------------------------
  /// @copydoc Image::IsEmpty
  bool IsEmpty() const;
  /// AVFrameと同時にRawBitmapの実体を作成する
  /// @sa Image::Create
  ErrorCodes Create(ImagePixelFormats pixel_format, int width, int height);
  //-------------------------------------------------------------------

  /// Getter: 各種ビットマップ
  uint8_t* bitmap() const;
  /// Getter: AVFrameへのポインタ
  AVFrame* avframe() const;

 private:
  /// 各種ビットマップ
  uint8_t *bitmap_;
  /// AVFrameへのポインタ
  AVFrame *avframe_;

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(AVFrameBitmapImage);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_AVFRAME_BITMAP_IMAGE_H_
