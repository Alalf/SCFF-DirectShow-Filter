
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

#include <libavfilter/drawutils.h>

#include "imaging/processor.h"
#include "imaging/avpicture-image.h"

namespace imaging {

class ScreenCapture;

/// @brief スクリーンキャプチャ出力一つだけを処理するレイアウトプロセッサ
class NativeLayout : public Processor {
 public:
  /// @brief コンストラクタ
  NativeLayout(ImagePixelFormat pixel_format, int width, int height,
               const ScreenCaptureParameter &parameter,
               bool stretch, bool keep_aspect_ratio);

  //-------------------------------------------------------------------
  /// @brief デストラクタ
  ~NativeLayout();
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Accept
  ErrorCode Accept(Request *request);
  /// @copydoc Processor::PullAVPictureImage
  ErrorCode PullAVPictureImage(AVPictureImage *image);
  //-------------------------------------------------------------------

 private:
  //-------------------------------------------------------------------
  // (copy禁止)
  //-------------------------------------------------------------------
  /// @brief コピーコンストラクタ
  NativeLayout(const NativeLayout& layout);
  /// @brief 代入演算子(copy禁止)
  void operator=(const NativeLayout& layout);
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  /// @brief スクリーンキャプチャ
  ScreenCapture *screen_capture_;
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  /// @brief キャプチャ用イメージ
  AVPictureImage capture_image_;
  //-------------------------------------------------------------------

  /// @brief スクリーンキャプチャパラメータ
  const ScreenCaptureParameter parameter_;

  //-------------------------------------------------------------------
  /// @brief ピクセルフォーマットがdrawutilsに対応しているかどうか
  bool can_use_drawutils_;
  /// @brief 描画用コンテキスト
  FFDrawContext draw_context_;
  /// @brief 枠描画用カラー
  FFDrawColor padding_color_;
  /// @brief 枠描画用カラー(RGBA)
  uint8_t rgba_padding_color_[4];
  //-------------------------------------------------------------------

  /// @brief 取り込み範囲が出力サイズより小さい場合拡張
  const bool stretch_;
  /// @brief アスペクト比の保持
  const bool keep_aspect_ratio_;

  /// @brief パディング(left)
  int padding_left_;
  /// @brief パディング(right)
  int padding_right_;
  /// @brief パディング(top)
  int padding_top_;
  /// @brief パディング(bottom)
  int padding_bottom_;
};
}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_NATIVE_LAYOUT_H_
