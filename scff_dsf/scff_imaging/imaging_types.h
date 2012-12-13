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

/// @file scff_imaging/imaging_types.h
/// scff_imagingモジュール用型宣言

#ifndef SCFF_DSF_SCFF_IMAGING_IMAGING_TYPES_H_
#define SCFF_DSF_SCFF_IMAGING_IMAGING_TYPES_H_

extern "C" {
#include <libswscale/swscale.h>
}
#include <Windows.h>
#include <cstdint>

namespace scff_imaging {

//=====================================================================
// 定数
//=====================================================================

/// ProcessorのInput/Outputに設定できるImageの最大数
const int kMaxProcessorSize = 8;

//---------------------------------------------------------------------

/// 共通エラーコード
enum class ErrorCode {
  //-------------------------------------------------------------------
  // Common
  //-------------------------------------------------------------------
  /// エラーはない
  kNoError = 0,

  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------

  /// RawBitmapイメージのメモリ確保に失敗した
  kRawBitmapImageOutOfMemoryError = 1000,

  /// AVPictureイメージのメモリ確保に失敗した
  kAVPictureImageOutOfMemoryError = 1001,

  /// AVPictureWithFillイメージのメモリ確保に失敗した
  kAVPictureWithFillImageOutOfMemoryError = 1002,
  /// AVPictureWithFillイメージのAVPicture作成に失敗した
  kAVPictureWithFillImageCannotCreateAVPictureError = 1003,
  /// AVPictureWithFillイメージのfillに失敗した
  kAVPictureWithFillImageCannotFillError = 1004,

  /// WindowsDDBイメージ作成時のリソース画像のLoadImageに失敗した
  kWindowsDDBImageCannotLoadResourceImageError = 1005,
  /// WindowsDDBイメージ作成時にWindowからDCを得ることに失敗した
  kWindowsDDBImageCannotGetDCFromWindowError = 1006,
  /// 32bit以外のWindowからWindowsDDBイメージを作成しようとした
  kWindowsDDBImageNotRGB32WindowError = 1007,
  /// WindowsDDBイメージのメモリ確保に失敗した
  kWindowsDDBImageOutOfMemoryError = 1008,

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------

  /// 初期化されていないプロセッサである
  kProcessorUninitializedError = 2000,

  /// SWScaleコンテキストの作成に失敗した
  kScaleCannotGetContextError = 2001,
  /// SWScaleフィルタの作成に失敗した
  kScaleCannotGetDefaultFilterError = 2002,

  /// ScreenCapture時、取り込み対象のWindowが適切な状態ではなかった
  kScreenCaptureInvalidWindowError = 2003,
  /// ScreenCapture時、クリッピング領域の設定が適切ではなかった
  kScreenCaptureInvalidClippingRegionError = 2004,
  /// ScreenCapture時、画面の色深度が32bitではなかった
  kScreenCaptureNot32bitColorError= 2005,

  //-------------------------------------------------------------------
  // Layout
  //-------------------------------------------------------------------

  /// 複合レイアウトで出力画像より外の範囲に要素を配置しようとした
  kComplexLayoutBoundError = 3000,
  /// 複合レイアウトに対応していないピクセルフォーマット
  kComplexLayoutInvalidPixelFormatError = 3001,
};

//---------------------------------------------------------------------

/// イメージのピクセルフォーマット
enum class ImagePixelFormat {
  /// 不正なピクセルフォーマット
  kInvalidPixelFormat = -1,
  /// I420(12bit)
  kI420 = 0,
  /// IYUV(12bit)
  kIYUV,
  /// YV12(12bit)
  kYV12,
  /// UYVY(16bit)
  kUYVY,
  /// YUY2(16bit)
  kYUY2,
  /// RGB0(32bit)
  kRGB0,
  /// 対応ピクセルフォーマット数
  kSupportedPixelFormatsCount
};

//---------------------------------------------------------------------

/// 拡大縮小メソッドをあらわす定数
/// @sa libswscale/swscale.h
enum class SWScaleFlags {
  /// fast bilinear
  kFastBilinear = SWS_FAST_BILINEAR,
  /// bilinear
  kBilinear     = SWS_BILINEAR,
  /// bicubic
  kBicubic      = SWS_BICUBIC,
  /// experimental
  kX            = SWS_X,
  /// nearest neighbor
  kPoint        = SWS_POINT,
  /// averaging area
  kArea         = SWS_AREA,
  /// luma bicubic, chroma bilinear
  kBicublin     = SWS_BICUBLIN,
  /// gaussian
  kGauss        = SWS_GAUSS,
  /// sinc
  kSinc         = SWS_SINC,
  /// lanczos
  kLanczos      = SWS_LANCZOS,
  /// natural bicubic spline
  kSpline       = SWS_SPLINE
};

//---------------------------------------------------------------------

/// 回転方向を表す定数
enum class RotateDirection {
  /// 回転なし
  kNoRotate = 0,
  /// 時計回り90度
  k90Degrees,
  /// 時計回り180度
  k180Degrees,
  /// 時計回り270度
  k270Degrees
};

//=====================================================================
// タイプ
//=====================================================================

/// 拡大縮小メソッドの設定
// #define SWS_FULL_CHR_H_INT    0x2000
// #define SWS_FULL_CHR_H_INP    0x4000
// #define SWS_DIRECT_BGR        0x8000
// #define SWS_ACCURATE_RND      0x40000
// #define SWS_BITEXACT          0x80000
struct SWScaleConfig {
  //-------------------------------------------------------------------
  // 拡大縮小メソッド
  //-------------------------------------------------------------------
  /// 拡大縮小メソッド(Chroma/Luma共通)
  SWScaleFlags flags;

  /// 正確な丸め処理
  bool accurate_rnd;

  //-------------------------------------------------------------------
  // フィルタ
  //-------------------------------------------------------------------
  /// 変換前にフィルタをかけるか
  /// @attention false推奨
  bool is_filter_enabled;

  /// 輝度のガウスぼかし
  float luma_gblur;
  /// 色差のガウスぼかし
  float chroma_gblur;
  /// 輝度のシャープ化
  float luma_sharpen;
  /// 色差のシャープ化
  float chroma_sharpen;
  /// 水平方向のワープ
  float chroma_hshift;
  /// 垂直方向のワープ
  float chroma_vshift;

  //-------------------------------------------------------------------
  // 未使用
  //-------------------------------------------------------------------
  // bool full_chr_h_int;
  // bool full_chr_h_inp;
  // bool direct_bgr;
  // bool bitexact;       // リファレンスエンコーダとの比較用？
  //-------------------------------------------------------------------
};

/// レイアウトパラメータ
struct LayoutParameter {
  /// サンプル内の原点のX座標
  /// @warning NullLayout,NativeLayoutでは無視される
  int bound_x;
  /// サンプル内の原点のY座標
  /// @warning NullLayout,NativeLayoutでは無視される
  int bound_y;
  /// サンプル内の幅
  /// @warning NullLayout,NativeLayoutでは無視される
  int bound_width;
  /// サンプル内の高さ
  /// @warning NullLayout,NativeLayoutでは無視される
  int bound_height;
  /// キャプチャを行う対象となるウィンドウ
  HWND window;
  /// 取り込み範囲の開始X座標
  int clipping_x;
  /// 取り込み範囲の開始y座標
  int clipping_y;
  /// 取り込み範囲の幅
  int clipping_width;
  /// 取り込み範囲の高さ
  int clipping_height;
  /// マウスカーソルの表示
  bool show_cursor;
  /// レイヤードウィンドウの表示
  bool show_layered_window;
  /// 拡大縮小設定
  SWScaleConfig swscale_config;
  /// 取り込み範囲が出力サイズより小さい場合拡張
  bool stretch;
  /// アスペクト比の保持
  bool keep_aspect_ratio;
  /// 回転方向
  RotateDirection rotate_direction;
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_IMAGING_TYPES_H_
