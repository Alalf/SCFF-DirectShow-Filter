
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

/// @file imaging/native-layout.h
/// @brief imaging::NativeLayoutの宣言

#ifndef SCFF_DSF_IMAGING_NATIVE_LAYOUT_H_
#define SCFF_DSF_IMAGING_NATIVE_LAYOUT_H_

#include "imaging/processor.h"
#include "imaging/avpicture-with-fill-image.h"
#include "imaging/avpicture-image.h"

namespace imaging {

class ScreenCapture;
class Scale;
class Padding;

/// @brief スクリーンキャプチャ出力一つだけを処理するレイアウトプロセッサ
class NativeLayout : public Processor<void, AVPictureImage> {
 public:
  /// @brief コンストラクタ
  NativeLayout(const ScreenCaptureParameter &parameter,
               bool stretch, bool keep_aspect_ratio);
  /// @brief デストラクタ
  ~NativeLayout();

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
  NativeLayout(const NativeLayout&);
  /// @brief 代入演算子(copy禁止)
  void operator=(const NativeLayout&);
  //-------------------------------------------------------------------

  /// @brief 設定されたImageはPadding可能か？
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

  /// @brief スクリーンキャプチャパラメータ
  const ScreenCaptureParameter parameter_;
  /// @brief 取り込み範囲が出力サイズより小さい場合拡張
  const bool stretch_;
  /// @brief アスペクト比の保持
  const bool keep_aspect_ratio_;
};
}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_NATIVE_LAYOUT_H_
