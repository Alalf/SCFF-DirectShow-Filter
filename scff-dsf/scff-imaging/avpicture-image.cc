
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

/// @file scff-imaging/avpicture-image.cc
/// @brief scff_imaging::AVPictureImageの定義

#include "scff-imaging/avpicture-image.h"

extern "C" {
#include <libavcodec/avcodec.h>
}

#include "scff-imaging/debug.h"
#include "scff-imaging/utilities.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::AVPictureImage
//=====================================================================

// コンストラクタ
AVPictureImage::AVPictureImage()
    : Image(),
      avpicture_(0) {   // NULL
  /// @attention avpicture_そのものの構築はCreateで行う
}

// デストラクタ
AVPictureImage::~AVPictureImage() {
  if (!IsEmpty()) {   // NULL
    avpicture_free(avpicture_);
  }
}

// Create()などによって実体がまだ生成されていない場合
bool AVPictureImage::IsEmpty() const {
  return avpicture_ == 0;   // NULL
}

/// @brief AVPictureのみ作成する
ErrorCode AVPictureImage::Create(ImagePixelFormat pixel_format,
                                 int width, int height) {
  // pixel_format, width, heightを設定する
  ErrorCode error_create = Image::Create(pixel_format, width, height);
  if (error_create != kNoError) {
    return error_create;
  }

  // 取り込み用AVPictureを作成
  AVPicture *avpicture = new AVPicture();
  int result_alloc =
      avpicture_alloc(avpicture,
                      avpicture_pixel_format(),
                      width, height);
  if (result_alloc != 0) {
    return kAVPictureImageOutOfMemoryError;
  }
  avpicture_ = avpicture;

  return kNoError;
}

// Getter: AVPictureへのポインタ
AVPicture* AVPictureImage::avpicture() const {
  return avpicture_;
}
}   // namespace scff_imaging
