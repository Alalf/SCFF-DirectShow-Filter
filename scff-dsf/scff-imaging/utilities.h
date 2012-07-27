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

/// @file scff-imaging/utilities.h
/// @brief scff_imaging::Utilitiesの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_UTILITIES_H_
#define SCFF_DSF_SCFF_IMAGING_UTILITIES_H_

#include <Windows.h>
extern "C" {
#include <libavcodec/avcodec.h>
}

#include "scff-imaging/imaging-types.h"

namespace scff_imaging {

class Image;
class AVPictureImage;

/// @brief scff_imagingモジュールを使う上で便利な機能を集めたクラス
class Utilities {
 public:
  //-------------------------------------------------------------------
  // リソースの取得用DLLインスタンスハンドルの取得
  //-------------------------------------------------------------------

  /// @brief Getter: リソースの取得用DLLインスタンスハンドル
  static HINSTANCE dll_instance();
  /// @brief Setter: リソースの取得用DLLインスタンスハンドル
  static void set_dll_instance(HINSTANCE dll_instance);

  //-------------------------------------------------------------------
  // イメージの操作
  //-------------------------------------------------------------------

  /// @brief 縦方向に回転したAVPictureを返す。実体は共通なので解放の必要はない。
  static void FlipHorizontal(const AVPicture *input, int input_height,
                             AVPicture *output);

  /// @brief drawutilsが使用可能なピクセルフォーマットか
  static bool CanUseDrawUtils(ImagePixelFormat pixel_format);

  //-------------------------------------------------------------------
  // イメージのタイプ（サイズ、形式など）
  //-------------------------------------------------------------------

  /// @brief イメージのサイズを求める
  static int CalculateDataSize(ImagePixelFormat pixel_format,
                               int width, int height);
  /// @brief イメージのサイズを直接求める
  static int CalculateImageSize(const Image &image);

  /// @brief AVPicture用PixelFormatを取得
  static PixelFormat ToAVPicturePixelFormat(ImagePixelFormat pixel_format);

  /// @brief BITMAPINFOHEADERを取得
  static void ToWindowsBitmapInfo(ImagePixelFormat pixel_format,
                                  int width,
                                  int height,
                                  bool vertical_invert,
                                  BITMAPINFO *info);
  /// @brief イメージからBITMAPINFOHEADERを取得
  static void ImageToWindowsBitmapInfo(const Image &image,
                                       bool vertical_invert,
                                       BITMAPINFO *info);

  /// @brief int(index)->enum(ImagePixelFormat)変換
  /// @warning バグの元なので注意して使うこと
  static ImagePixelFormat IndexToPixelFormat(int index);

  /// @brief BITMAPINFOHEADERからImagePixelFormatを取得
  static ImagePixelFormat WindowsBitmapInfoHeaderToPixelFormat(
      const BITMAPINFOHEADER &info_header);

  /// @brief BITMAPINFOHEADERから対応ピクセルフォーマットかどうかを求める
  static bool IsSupportedPixelFormat(const BITMAPINFOHEADER &info_header);

  //-------------------------------------------------------------------
  // レイアウト
  //-------------------------------------------------------------------

  /// @brief 指定された範囲（同じ座標系）が中に含まれているか
  static bool Contains(int bound_x, int bound_y,
                       int bound_width, int bound_height,
                       int x, int y, int width, int height);

  /// @brief 境界の座標系と同じ座標系の新しい配置を計算する
  static bool CalculateLayout(int bound_x, int bound_y,
                              int bound_width, int bound_height,
                              int input_width, int input_height,
                              bool stretch, bool keep_aspect_ratio,
                              int *new_x, int *new_y,
                              int *new_width, int *new_height);

  /// @brief 幅と高さから拡大縮小した場合のパディングサイズを求める
  static bool CalculatePaddingSize(int bound_width, int bound_height,
                                   int input_width, int input_height,
                                   bool stretch, bool keep_aspect_ratio,
                                   int *padding_top, int *padding_bottom,
                                   int *padding_left, int *padding_right);

  /// @brief マルチモニタを考慮してウィンドウ領域を求める
  static void GetWindowRectangle(HWND window, int *x, int *y,
                                 int *width, int *height);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_UTILITIES_H_
