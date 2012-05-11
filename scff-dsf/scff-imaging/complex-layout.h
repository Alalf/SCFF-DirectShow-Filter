
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

/// @file scff-imaging/complex-layout.h
/// @brief scff_imaging::ComplexLayoutの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_COMPLEX_LAYOUT_H_
#define SCFF_DSF_SCFF_IMAGING_COMPLEX_LAYOUT_H_

#include <libavfilter/drawutils.h>

#include "scff-imaging/common.h"
#include "scff-imaging/processor.h"
#include "scff-imaging/avpicture-with-fill-image.h"
#include "scff-imaging/avpicture-image.h"

namespace scff_imaging {

class ScreenCapture;
class Scale;
class Padding;

/// @brief 複数のスクリーンキャプチャ領域を取り扱い可能なレイアウト
class ComplexLayout : public Processor<void, AVPictureImage> {
 public:
  /// @brief コンストラクタ
  ComplexLayout(int element_count, LayoutParameter parameter[kMaxProcessorSize]);
  /// @brief デストラクタ
  ~ComplexLayout();

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Run
  ErrorCode Run();
  //-------------------------------------------------------------------

 private:
  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(ComplexLayout);

  /// @brief インデックスを指定して初期化
  ErrorCode InitByIndex(int index);

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  /// @brief スクリーンキャプチャ
  ScreenCapture *screen_capture_;
  /// @brief 拡大縮小ピクセルフォーマット変換
  Scale *scale_[kMaxProcessorSize];
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  /// @brief ScreenCaptureから取得した変換処理前のイメージ
  AVPictureWithFillImage captured_image_[kMaxProcessorSize];
  /// @brief SWScaleで拡大縮小ピクセルフォーマット変換を行った後のイメージ
  AVPictureImage converted_image_[kMaxProcessorSize];
  //-------------------------------------------------------------------

  /// @brief 描画用コンテキスト
  FFDrawContext draw_context_;
  /// @brief 背景カラー
  FFDrawColor background_color_;

  /// @brief レイアウト要素拡大縮小後の新しい原点のX座標
  int element_x_[kMaxProcessorSize];
  /// @brief レイアウト要素拡大縮小後の新しい原点のY座標
  int element_y_[kMaxProcessorSize];

  /// @brief レイアウト要素の数
  const int element_count_;

  /// @brief レイアウトパラメータ
  LayoutParameter parameter_[kMaxProcessorSize];
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_COMPLEX_LAYOUT_H_
