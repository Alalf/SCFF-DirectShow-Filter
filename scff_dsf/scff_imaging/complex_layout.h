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

/// @file scff_imaging/complex_layout.h
/// scff_imaging::ComplexLayoutの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_COMPLEX_LAYOUT_H_
#define SCFF_DSF_SCFF_IMAGING_COMPLEX_LAYOUT_H_

#include <libavfilter/drawutils.h>

#include "scff_imaging/common.h"
#include "scff_imaging/layout.h"
#include "scff_imaging/avpicture_with_fill_image.h"

namespace scff_imaging {

class ScreenCapture;
class Scale;
class Padding;

/// 複数のスクリーンキャプチャ領域を取り扱い可能なレイアウト
class ComplexLayout : public Layout {
 public:
  /// コンストラクタ
  ComplexLayout(
      int element_count,
      const LayoutParameter (&parameters)[kMaxProcessorSize]);
  /// デストラクタ
  ~ComplexLayout();

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Run
  ErrorCode Run();
  //-------------------------------------------------------------------

 private:
  /// インデックスを指定して初期化
  ErrorCode InitByIndex(int index);

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  /// スクリーンキャプチャ
  ScreenCapture *screen_capture_;
  /// 拡大縮小ピクセルフォーマット変換
  Scale *scale_[kMaxProcessorSize];
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  /// ScreenCaptureから取得した変換処理前のイメージ
  AVPictureWithFillImage captured_image_[kMaxProcessorSize];
  /// SWScaleで拡大縮小ピクセルフォーマット変換を行った後のイメージ
  AVPictureImage converted_image_[kMaxProcessorSize];
  //-------------------------------------------------------------------

  /// 描画用コンテキスト
  FFDrawContext draw_context_;
  /// 背景カラー
  FFDrawColor background_color_;

  /// レイアウト要素拡大縮小後の新しい原点のX座標
  int element_x_[kMaxProcessorSize];
  /// レイアウト要素拡大縮小後の新しい原点のY座標
  int element_y_[kMaxProcessorSize];

  /// レイアウト要素の数
  const int element_count_;

  /// レイアウトパラメータ
  LayoutParameter parameters_[kMaxProcessorSize];

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(ComplexLayout);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_COMPLEX_LAYOUT_H_
