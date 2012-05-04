
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

/// @file imaging/screen-capture.h
/// @brief imaging::ScreenCaptureの宣言

#ifndef SCFF_DSF_IMAGING_SCREEN_CAPTURE_H_
#define SCFF_DSF_IMAGING_SCREEN_CAPTURE_H_

#include <Windows.h>
extern "C" {
#include <libswscale/swscale.h>
}

#include "imaging/processor.h"
#include "imaging/windows-ddb-image.h"
#include "imaging/avpicture-with-fill-image.h"

namespace imaging {

/// @brief スクリーンキャプチャを行うプロセッサ
class ScreenCapture : public Processor {
 public:
  /// @brief コンストラクタ
  ScreenCapture(ImagePixelFormat pixel_format, int width, int height,
                const ScreenCaptureParameter &parameter);

  //-------------------------------------------------------------------
  /// @brief デストラクタ
  ~ScreenCapture();
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
  ScreenCapture(const ScreenCapture& screen_capture);
  /// @brief 代入演算子(copy禁止)
  void operator=(const ScreenCapture& screen_capture);
  //-------------------------------------------------------------------

  /// @brief 渡されたDCにカーソルを描画する
  void DrawCursor(HDC dc);

  /// @brief Windowがキャプチャに適切な状態になっているか判定する
  bool ValidateWindow();

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  // No Child Processor
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  /// @brief BitBlt用DDB
  WindowsDDBImage image_for_bitblt_;
  /// @brief SWScale用AVPicture
  AVPictureWithFillImage image_for_swscale_;
  //-------------------------------------------------------------------

  /// @brief BitBlt用DDBのデバイスコンテキスト
  HDC dc_for_bitblt_;
  /// @brief GetDIBits用BITMAPINFO
  BITMAPINFO info_for_getdibits_;

  /// @brief 拡大縮小用のコンテキスト
  struct SwsContext *scaler_;

  /// @brief Init呼び出し時のウィンドウの幅
  int window_width_;
  /// @brief Init呼び出し時のウィンドウの高さ
  int window_height_;

  /// @brief スクリーンキャプチャパラメータ
  const ScreenCaptureParameter parameter_;
};
}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_SCREEN_CAPTURE_H_
