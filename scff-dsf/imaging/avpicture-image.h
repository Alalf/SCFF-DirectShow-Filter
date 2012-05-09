
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

/// @file imaging/avpicture-image.h
/// @brief imaging::AVPictureImageの宣言

#ifndef SCFF_DSF_IMAGING_AVPICTURE_IMAGE_H_
#define SCFF_DSF_IMAGING_AVPICTURE_IMAGE_H_

#include "imaging/image.h"

namespace imaging {

/// @brief AVPicture(ffmpeg)の実体を管理するクラス
class AVPictureImage: public Image {
 public:
  /// @brief コンストラクタ
  AVPictureImage();
  /// @brief デストラクタ
  ~AVPictureImage();

  //-------------------------------------------------------------------
  /// @brief Create()などによって実体がまだ生成されていない場合
  bool IsEmpty() const;
  /// @brief AVPictureの実体を作成する
  ErrorCode Create(ImagePixelFormat pixel_format, int width, int height);
  //-------------------------------------------------------------------

  /// @brief Getter: AVPictureへのポインタ
  AVPicture* avpicture() const;

 private:
  //-------------------------------------------------------------------
  // (copy禁止)
  //-------------------------------------------------------------------
  /// @brief コピーコンストラクタ(copy禁止)
  AVPictureImage(const AVPictureImage&);
  /// @brief 代入演算子(copy禁止)
  void operator=(const AVPictureImage&);
  //-------------------------------------------------------------------

  /// @brief AVPictureへのポインタ
  AVPicture *avpicture_;
};
}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_AVPICTURE_IMAGE_H_
