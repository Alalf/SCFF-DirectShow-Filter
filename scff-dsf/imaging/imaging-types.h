
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

/// @file imaging/imaging-types.h
/// @brief imagingモジュール用型宣言

#ifndef SCFF_DSF_IMAGING_IMAGING_TYPES_H_
#define SCFF_DSF_IMAGING_IMAGING_TYPES_H_

#include <Windows.h>
#include <cstdint>

//=====================================================================
// 定数
//=====================================================================

namespace imaging {

//---------------------------------------------------------------------

/// @brief 共通エラーコード
enum ErrorCode {
  /// @brief エラーはない
  kNoError = 0,
  /// @brief 初期化されていない
  kUninitialziedError,
  /// @brief 対応してないType
  kImageTypeError,
  /// @brief メモリの確保に失敗した
  kOutOfMemoryError,
  /// @brief 一回しか初期化・生成は許されていない
  kMultipleCreateError,
  /// @brief 対応してないPixelFormat
  kPixelFormatError,
  /// @brief 不正なウィンドウハンドル
  kInvalidWindow,
  /// @brief クリッピング領域が不正
  kInvalidClippingRegion,
  /// @brief キャプチャ領域が動作中に不正になった
  kInvalidCaptureRegion,
  /// @brief 取り込み時に画面の色深度が32bitではなかった
  kNot32bitColor
};

//---------------------------------------------------------------------

/// @brief イメージタイプ
enum ImageType {
  /// @brief 不正なタイプ
  kInvalidImageType = -1,
  /// @brief 各種ビットマップへのポインタ
  kRawBitmap = 0,
  /// @brief ビットマップハンドル(HBITMAP)
  kWindowsDDB,
  /// @brief AVPicture(ffmepg)
  kAVPicture
};

//---------------------------------------------------------------------

/// @brief イメージのピクセルフォーマット
enum ImagePixelFormat {
  /// @brief 不正なピクセルフォーマット
  kInvalidPixelFormat = -1,
  /// @brief RGB0(32bit)
  kRGB0 = 0,
  /// @brief I420(12bit)
  kI420,
  /// @brief UYVY(16bit)
  kUYVY
};

//---------------------------------------------------------------------

/// @brief 拡大縮小メソッドをあらわす定数(from swscale.h)
enum SWScaleFlags {
  /// @brief fast bilinear
  kFastBilinear = 1,
  /// @brief bilinear
  kBilinear     = 2,
  /// @brief bicubic
  kBicubic      = 4,
  /// @brief experimental
  kX            = 8,
  /// @brief nearest neighbor
  kPoint        = 0x10,
  /// @brief averaging area
  kArea         = 0x20,
  /// @brief luma bicubic, chroma bilinear
  kBicublin     = 0x40,
  /// @brief gaussian
  kGauss        = 0x80,
  /// @brief sinc
  kSinc         = 0x100,
  /// @brief natural
  kLanczos      = 0x200,
  /// @brief natural bicubic spline
  kSpline       = 0x400
};

//---------------------------------------------------------------------

/// @brief スクリーンキャプチャパラメータ
struct ScreenCaptureParameter {
  /// @brief 初期化用コンストラクタ
  ScreenCaptureParameter()
      : window(0),        // NULL
        clipping_x(0),
        clipping_y(0),
        clipping_width(0),
        clipping_height(0),
        show_cursor(false),
        show_layered_window(false),
        sws_flags(kFastBilinear) {
    // nop
  }

  /// @brief キャプチャを行う対象となるウィンドウ
  HWND window;
  /// @brief 取り込み範囲の開始X座標
  int clipping_x;
  /// @brief 取り込み範囲の開始y座標
  int clipping_y;
  /// @brief 取り込み範囲の幅
  int clipping_width;
  /// @brief 取り込み範囲の高さ
  int clipping_height;
  /// @brief マウスカーソルの表示
  bool show_cursor;
  /// @brief レイヤードウィンドウの表示
  bool show_layered_window;
  /// @brief 拡大縮小アルゴリズムの選択
  SWScaleFlags sws_flags;
};
}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_IMAGING_TYPES_H_
