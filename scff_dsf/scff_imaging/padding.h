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

/// @file scff_imaging/padding.h
/// scff_imaging::Paddingの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_PADDING_H_
#define SCFF_DSF_SCFF_IMAGING_PADDING_H_

#include <libavfilter/drawutils.h>

#include "scff_imaging/common.h"
#include "scff_imaging/processor.h"

namespace scff_imaging {

/// drawutilsを利用してパディングを行う
class Padding : public Processor<AVPictureImage, AVPictureImage> {
 public:
  /// コンストラクタ
  Padding(int padding_left, int padding_right,
          int padding_top, int padding_bottom);
  /// デストラクタ
  ~Padding();

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Run
  ErrorCode Run();
  //-------------------------------------------------------------------

 private:
  /// 描画用コンテキスト
  FFDrawContext draw_context_;
  /// 枠描画用カラー
  FFDrawColor padding_color_;

  /// パディング(left)
  const int padding_left_;
  /// パディング(right)
  const int padding_right_;
  /// パディング(top)
  const int padding_top_;
  /// パディング(bottom)
  const int padding_bottom_;

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(Padding);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_PADDING_H_
