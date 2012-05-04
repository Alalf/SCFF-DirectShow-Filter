
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

/// @file imaging/image.h
/// @brief imaging::Imageの宣言

#ifndef SCFF_DSF_IMAGING_IMAGE_H_
#define SCFF_DSF_IMAGING_IMAGE_H_

#include <Windows.h>
#include <cstdint>
#include "imaging/imaging-types.h"

struct AVPicture;

namespace imaging {

/// @brief イメージの実体を管理する(union的な)クラス
/// @attention Imageのインスタンスはメモリ領域の作成、解放まで責任を持つ
class Image {
 public:
  /// @brief Getter: イメージのピクセルフォーマット
  ImagePixelFormat pixel_format() const;
  /// @brief Getter: イメージの幅
  int width() const;
  /// @brief Getter: イメージの高さ
  int height() const;

  //-------------------------------------------------------------------
  /// @brief 仮想デストラクタ
  virtual ~Image();
  /// @brief Create()などによって実体がまだ生成されていない場合
  virtual bool IsEmpty() const = 0;
  //-------------------------------------------------------------------

 protected:
  //-------------------------------------------------------------------
  // (new禁止＝抽象クラス)
  /// @brief コンストラクタ
  Image();
  /// @brief 実体を生成する
  /// @attention 各継承クラスでは以下のようなpublicメソッドを作り、
  /// @attention 内部でCreate()を呼び出すこと
  /// @attention (pixel_format_,width_,height_を設定する手段はこのメソッドのみ)
  /// @code
  /// ErrorCode CreateWith___(ImagePixelFormat pixel_format,
  ///                         int width, int height, ___);
  /// ErrorCode CreateFrom___(ImagePixelFormat pixel_format,
  ///                         int width, int height, ___);
  /// @endcode
  virtual ErrorCode Create(ImagePixelFormat pixel_format,
                           int width, int height);
  //-------------------------------------------------------------------
 private:
  //-------------------------------------------------------------------
  // (copy禁止)
  //-------------------------------------------------------------------
  /// @brief コピーコンストラクタ
  Image(const Image& image);
  /// @brief 代入演算子(copy禁止)
  void operator=(const Image& image);
  //-------------------------------------------------------------------

  /// @brief イメージのピクセルフォーマット
  ImagePixelFormat pixel_format_;
  /// @brief イメージの幅
  int width_;
  /// @brief イメージの高さ
  int height_;
};
}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_IMAGE_H_
