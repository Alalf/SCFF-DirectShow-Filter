﻿
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

/// @file imaging/padding.cc
/// @brief imaging::Paddingの定義

#include "imaging/padding.h"

#include "imaging/avpicture-image.h"

namespace imaging {

//=====================================================================
// imaging::Padding
//=====================================================================

// コンストラクタ
Padding::Padding(int padding_left, int padding_right,
                 int padding_top, int padding_bottom)
    : padding_left_(padding_left),        // ありえない値
      padding_right_(padding_right),      // ありえない値
      padding_top_(padding_top),          // ありえない値
      padding_bottom_(padding_bottom) {   // ありえない値 
  // 配列の初期化
  rgba_padding_color_[0] = 0;
  rgba_padding_color_[1] = 0;
  rgba_padding_color_[2] = 0;
  rgba_padding_color_[3] = 0;
  // 明示的に初期化していない
  // draw_context_
  // padding_color_
}

// デストラクタ
Padding::~Padding() {
  // nop
}

// Padding機能が利用できるかどうか
bool Padding::CanUsePadding() const {
  ASSERT(GetInputImage()->pixel_format() == GetOutputImage()->pixel_format());

  /// @warning 2012/05/08現在drawutilsはPlaner Formatにしか対応していない
  switch (GetOutputImage()->pixel_format()) {
  case kI420:
  case kRGB0:
    return true;
    break;
  case kUYVY:
  default:
    return false;
    break;
  }
}

//-------------------------------------------------------------------
// Processor::Init
ErrorCode Padding::Init() {
  ASSERT(GetInputImage()->pixel_format() == GetOutputImage()->pixel_format());

  // パディング用のコンテキスト・カラーの初期化
  const int error_init =
      ff_draw_init(&draw_context_,
                   GetOutputImage()->avpicture_pixel_format(),
                   0);
  ASSERT(error_init == 0);

  rgba_padding_color_[0] = 0;
  rgba_padding_color_[1] = 0;
  rgba_padding_color_[2] = 0;
  rgba_padding_color_[3] = 0;
  ff_draw_color(&draw_context_,
                &padding_color_,
                rgba_padding_color_);

  return InitDone();
}

// Processor::Run
ErrorCode Padding::Run() {
  // 左の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    GetOutputImage()->avpicture()->data,
                    GetOutputImage()->avpicture()->linesize,
                    0,
                    padding_top_,
                    padding_left_,
                    GetInputImage()->height());

  // 右の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    GetOutputImage()->avpicture()->data,
                    GetOutputImage()->avpicture()->linesize,
                    padding_left_ + GetInputImage()->width(),
                    padding_top_,
                    padding_right_,
                    GetInputImage()->height());

  // 上の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    GetOutputImage()->avpicture()->data,
                    GetOutputImage()->avpicture()->linesize,
                    0,
                    0,
                    GetOutputImage()->width(),
                    padding_top_);

  // 中央に画像を配置する
  ff_copy_rectangle2(&draw_context_,
                     GetOutputImage()->avpicture()->data,
                     GetOutputImage()->avpicture()->linesize,
                     GetInputImage()->avpicture()->data,
                     GetInputImage()->avpicture()->linesize,
                     padding_left_,
                     padding_top_,
                     0,
                     0, 
                     GetInputImage()->width(),
                     GetInputImage()->height());

  // 下の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    GetOutputImage()->avpicture()->data,
                    GetOutputImage()->avpicture()->linesize,
                    0,
                    padding_top_ + GetInputImage()->height(),
                    GetOutputImage()->width(),
                    padding_bottom_);

  return NoError();
}
//-------------------------------------------------------------------

}   // namespace imaging
