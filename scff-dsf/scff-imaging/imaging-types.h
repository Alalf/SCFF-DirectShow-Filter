
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

/// @file scff-imaging/imaging-types.h
/// @brief scff-imagingモジュール用型宣言

#ifndef SCFF_DSF_SCFF_IMAGING_IMAGING_TYPES_H_
#define SCFF_DSF_SCFF_IMAGING_IMAGING_TYPES_H_

#include <Windows.h>
#include <cstdint>

//=====================================================================
// 定数
//=====================================================================

namespace scff_imaging {

/// @brief ProcessorのInput/Outputに設定できるImageの最大数
const int kMaxProcessorSize = 8;

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
  kInvalidWindowError,
  /// @brief クリッピング領域が不正
  kInvalidClippingRegionError,
  /// @brief キャプチャ領域が動作中に不正になった
  kInvalidCaptureRegionError,
  /// @brief 取り込み時に画面の色深度が32bitではなかった
  kNot32bitColorError,
  /// @brief 複合レイアウトで出力画像より外の範囲に要素レイアウトを配置しようとした
  kComplexLayoutBoundError,
  /// @brief 複合レイアウトに対応していないピクセルフォーマット
  kComplexLayoutInvalidPixelFormatError,
};

//---------------------------------------------------------------------

/// @brief イメージのピクセルフォーマット
enum ImagePixelFormat {
  /// @brief 不正なピクセルフォーマット
  kInvalidPixelFormat = -1,
  /// @brief I420(12bit)
  kI420 = 0,
  /// @brief UYVY(16bit)
  kUYVY,
  /// @brief RGB0(32bit)
  kRGB0,
  /// @brief 対応ピクセルフォーマット数
  kSupportedPixelFormatsCount
};

//---------------------------------------------------------------------

/// @brief 拡大縮小メソッドをあらわす定数
/// @sa libswscale/swscale.h
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

/// @brief 回転方向を表す定数
enum RotateDirection {
  /// @brief 回転なし
  kNoRotate = 0,
  /// @brief 時計回り90度
  k90Degrees,
  /// @brief 時計回り180度
  k180Degrees,
  /// @brief 時計回り270度
  k270Degrees
};

//---------------------------------------------------------------------

/// @brief レイアウトパラメータ
struct LayoutParameter {
  /// @brief サンプル内の原点のX座標
  /// @warning NullLayout,NativeLayoutでは無視される
  int bound_x;
  /// @brief サンプル内の原点のY座標
  /// @warning NullLayout,NativeLayoutでは無視される
  int bound_y;
  /// @brief サンプル内の幅
  /// @warning NullLayout,NativeLayoutでは無視される
  int bound_width;
  /// @brief サンプル内の高さ
  /// @warning NullLayout,NativeLayoutでは無視される
  int bound_height;
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
  /// @attention SWScaleFlagsを操作に使うこと
  SWScaleFlags sws_flags;
  /// @brief 取り込み範囲が出力サイズより小さい場合拡張
  bool stretch;
  /// @brief アスペクト比の保持
  bool keep_aspect_ratio;
  /// @brief 回転方向
  RotateDirection rotate_direction;
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_IMAGING_TYPES_H_
