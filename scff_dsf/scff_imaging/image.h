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

/// @file scff_imaging/image.h
/// scff_imaging::Imageの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_IMAGE_H_
#define SCFF_DSF_SCFF_IMAGING_IMAGE_H_

#include <Windows.h>
extern "C" {
#include <libavcodec/avcodec.h>
}

#include <cstdint>

#include "scff_imaging/common.h"

struct AVPicture;

namespace scff_imaging {

enum class ErrorCodes;
enum class ImagePixelFormats;

/// イメージの実体を管理する(union的な)クラス
/// @attention Imageのインスタンスはメモリ領域の作成、解放まで責任を持つ
class Image {
 public:
  /// 仮想デストラクタ
  virtual ~Image();

  /// Getter: イメージのピクセルフォーマット
  ImagePixelFormats pixel_format() const;
  /// イメージのピクセルフォーマットをAVPicture用に変換
  AVPixelFormat av_pixel_format() const;
  /// Getter: イメージの幅
  int width() const;
  /// Getter: イメージの高さ
  int height() const;

  //-------------------------------------------------------------------
  /// Create()などによって実体がまだ生成されていない場合true
  virtual bool IsEmpty() const = 0;
  //-------------------------------------------------------------------

 protected:
  //-------------------------------------------------------------------
  // (new禁止＝抽象クラス)
  /// コンストラクタ
  Image();
  /// 実体を生成する
  /// @attention 各継承クラスでは以下のようなpublicメソッドを作り、
  ///            内部でCreate()を呼び出すこと
  ///            (pixel_format_,width_,height_を設定する手段はこのメソッドのみ)
  /// @code
  /// ErrorCodes CreateWith___(ImagePixelFormats pixel_format,
  ///                          int width, int height, ___);
  /// ErrorCodes CreateFrom___(ImagePixelFormats pixel_format,
  ///                          int width, int height, ___);
  /// @endcode
  virtual ErrorCodes Create(ImagePixelFormats pixel_format,
                            int width, int height);
  //-------------------------------------------------------------------
 private:
  /// イメージのピクセルフォーマット
  ImagePixelFormats pixel_format_;
  /// イメージの幅
  int width_;
  /// イメージの高さ
  int height_;

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(Image);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_IMAGE_H_
