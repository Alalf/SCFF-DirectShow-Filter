
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

/// @file scff-imaging/native-layout.h
/// @brief scff_imaging::NativeLayoutの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_NATIVE_LAYOUT_H_
#define SCFF_DSF_SCFF_IMAGING_NATIVE_LAYOUT_H_

#include "scff-imaging/common.h"
#include "scff-imaging/layout.h"
#include "scff-imaging/avpicture-with-fill-image.h"

namespace scff_imaging {

class ScreenCapture;
class Scale;
class Padding;

/// @brief スクリーンキャプチャ出力一つだけを処理するレイアウトプロセッサ
class NativeLayout : public Layout {
 public:
  /// @brief コンストラクタ
  explicit NativeLayout(const LayoutParameter &parameter);
  /// @brief デストラクタ
  ~NativeLayout();

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Run
  ErrorCode Run();
  //-------------------------------------------------------------------

 private:
  /// @brief 設定されたOutputImageはPadding可能か？
  bool CanUsePadding() const;

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  /// @brief スクリーンキャプチャ
  ScreenCapture *screen_capture_;
  /// @brief 拡大縮小ピクセルフォーマット変換
  Scale *scale_;
  /// @brief パディング
  Padding *padding_;
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  /// @brief ScreenCaptureから取得した変換処理前のイメージ
  AVPictureWithFillImage captured_image_;
  /// @brief SWScaleで拡大縮小ピクセルフォーマット変換を行った後のイメージ
  AVPictureImage converted_image_;
  //-------------------------------------------------------------------

  /// @brief レイアウトパラメータ
  const LayoutParameter parameter_;

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(NativeLayout);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_NATIVE_LAYOUT_H_
