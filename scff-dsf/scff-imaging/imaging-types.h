
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
  //-------------------------------------------------------------------
  // Common
  //-------------------------------------------------------------------
  /// @brief エラーはない
  kNoError = 0,

  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------

  /// @brief RawBitmapイメージのメモリ確保に失敗した
  kRawBitmapImageOutOfMemoryError = 1000,

  /// @brief AVPictureイメージのメモリ確保に失敗した
  kAVPictureImageOutOfMemoryError = 1001,

  /// @brief AVPictureWithFillイメージのメモリ確保に失敗した
  kAVPictureWithFillImageOutOfMemoryError = 1002,
  /// @brief AVPictureWithFillイメージのAVPicture作成に失敗した
  kAVPictureWithFillImageCannotCreateAVPictureError = 1003,
  /// @brief AVPictureWithFillイメージのfillに失敗した
  kAVPictureWithFillImageCannotFillError = 1004,

  /// @brief WindowsDDBイメージ作成時のリソース画像のLoadImageに失敗した
  kWindowsDDBImageCannotLoadResourceImageError = 1005,
  /// @brief WindowsDDBイメージ作成時にWindowからDCを得ることに失敗した
  kWindowsDDBImageCannotGetDCFromWindowError = 1006,
  /// @brief 32bit以外のWindowからWindowsDDBイメージを作成しようとした
  kWindowsDDBImageNotRGB32WindowError = 1007,
  /// @brief WindowsDDBイメージのメモリ確保に失敗した
  kWindowsDDBImageOutOfMemoryError = 1008,

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------

  /// @brief 初期化されていないプロセッサである
  kProcessorUninitializedError = 2000,

  /// @brief SWScaleコンテキストの作成に失敗した
  kScaleCannotGetContextError = 2001,

  /// @brief ScreenCapture時、取り込み対象のWindowが適切な状態ではなかった
  kScreenCaptureInvalidWindowError = 2002,
  /// @brief ScreenCapture時、クリッピング領域の設定が適切ではなかった
  kScreenCaptureInvalidClippingRegionError = 2003,
  /// @brief ScreenCapture時、画面の色深度が32bitではなかった
  kScreenCaptureNot32bitColorError= 2004,

  //-------------------------------------------------------------------
  // Layout
  //-------------------------------------------------------------------

  /// @brief 複合レイアウトで出力画像より外の範囲に要素レイアウトを配置しようとした
  kComplexLayoutBoundError = 3000,
  /// @brief 複合レイアウトに対応していないピクセルフォーマット
  kComplexLayoutInvalidPixelFormatError = 3001,
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
