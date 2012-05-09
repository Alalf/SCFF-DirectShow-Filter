
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

/// @file imaging/padding.h
/// @brief imaging::Paddingの宣言

#ifndef SCFF_DSF_IMAGING_PADDING_H_
#define SCFF_DSF_IMAGING_PADDING_H_

#include <libavfilter/drawutils.h>

#include "imaging/processor.h"

namespace imaging {

/// @brief drawutilsを利用してパディングを行う
class Padding : public Processor<AVPictureImage, AVPictureImage> {
 public:
  /// @brief コンストラクタ
  Padding(int padding_left, int padding_right,
          int padding_top, int padding_bottom);
  /// @brief デストラクタ
  ~Padding();

  /// @brief Padding機能が利用できるかどうか
  bool CanUsePadding() const;

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Run
  ErrorCode Run();
  //-------------------------------------------------------------------

 private:
  //-------------------------------------------------------------------
  // (copy禁止)
  //-------------------------------------------------------------------
  /// @brief コピーコンストラクタ
  Padding(const Padding&);
  /// @brief 代入演算子(copy禁止)
  void operator=(const Padding&);
  //-------------------------------------------------------------------

  /// @brief 描画用コンテキスト
  FFDrawContext draw_context_;
  /// @brief 枠描画用カラー
  FFDrawColor padding_color_;
  /// @brief 枠描画用カラー(RGBA)
  uint8_t rgba_padding_color_[4];

  /// @brief パディング(left)
  const int padding_left_;
  /// @brief パディング(right)
  const int padding_right_;
  /// @brief パディング(top)
  const int padding_top_;
  /// @brief パディング(bottom)
  const int padding_bottom_;
};
}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_PADDING_H_
