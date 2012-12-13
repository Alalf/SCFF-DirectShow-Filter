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

/// @file scff_imaging/raw_bitmap_image.h
/// scff_imaging::RawBitmapImageの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_RAW_BITMAP_IMAGE_H_
#define SCFF_DSF_SCFF_IMAGING_RAW_BITMAP_IMAGE_H_

#include "scff_imaging/image.h"

namespace scff_imaging {

/// 各種ビットマップの実体を管理するクラス
class RawBitmapImage: public Image {
 public:
  /// コンストラクタ
  RawBitmapImage();
  /// デストラクタ
  ~RawBitmapImage();

  //-------------------------------------------------------------------
  /// @copydoc Image::IsEmpty
  bool IsEmpty() const;
  /// 必要な分だけメモリを確保する
  /// @sa Image::Create
  ErrorCode Create(ImagePixelFormat pixel_format, int width, int height);
  //-------------------------------------------------------------------

  /// Getter: 各種ビットマップ
  uint8_t* raw_bitmap() const;

 private:
  /// 各種ビットマップ
  uint8_t *raw_bitmap_;

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(RawBitmapImage);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_RAW_BITMAP_IMAGE_H_
