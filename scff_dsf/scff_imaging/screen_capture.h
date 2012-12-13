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

/// @file scff-imaging/screen-capture.h
/// scff_imaging::ScreenCaptureの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_SCREEN_CAPTURE_H_
#define SCFF_DSF_SCFF_IMAGING_SCREEN_CAPTURE_H_

#include <Windows.h>

#include "scff-imaging/processor.h"
#include "scff-imaging/windows-ddb-image.h"
#include "scff-imaging/avpicture-with-fill-image.h"

namespace scff_imaging {

/// スクリーンキャプチャを行うプロセッサ
class ScreenCapture : public Processor<void, AVPictureWithFillImage> {
 public:
  /// コンストラクタ
  ScreenCapture(
      bool vertical_invert,
      int count,
      const LayoutParameter (&parameters)[kMaxProcessorSize]);
  /// デストラクタ
  ~ScreenCapture();

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Run
  ErrorCode Run();
  //-------------------------------------------------------------------

 private:
  /// 渡されたDCにカーソルを描画する
  void DrawCursor(HDC dc, HWND window, int clipping_x, int clipping_y);

  /// キャプチャ設定を検証する
  ErrorCode ValidateParameter(int index);

  /// インデックスを指定して初期化
  ErrorCode InitByIndex(int index);

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  // No Child Processor
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  /// BitBlt用DDB
  WindowsDDBImage image_for_bitblt_[kMaxProcessorSize];
  //-------------------------------------------------------------------

  /// BitBlt用DDBのデバイスコンテキスト
  HDC dc_for_bitblt_[kMaxProcessorSize];
  /// GetDIBits用BITMAPINFO
  BITMAPINFO info_for_getdibits_[kMaxProcessorSize];

  /// BitBltに渡すラスターオペレーションコード
  DWORD raster_operation_[kMaxProcessorSize];

  /// レイアウトパラメータ
  LayoutParameter parameters_[kMaxProcessorSize];

  /// 取り込み時に上下反転を行うか
  const bool vertical_invert_;

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(ScreenCapture);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_SCREEN_CAPTURE_H_
