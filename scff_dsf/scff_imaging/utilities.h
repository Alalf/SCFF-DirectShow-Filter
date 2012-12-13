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

/// @file scff_imaging/utilities.h
/// scff_imaging::Utilitiesの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_UTILITIES_H_
#define SCFF_DSF_SCFF_IMAGING_UTILITIES_H_

#include <Windows.h>
extern "C" {
#include <libavcodec/avcodec.h>
}

namespace scff_imaging {

enum class ErrorCode;
enum class ImagePixelFormat;
class Image;
class AVPictureImage;

/// scff_imagingモジュールを使う上で便利な機能を集めた名前空間
namespace utilities {

//-------------------------------------------------------------------
// リソースの取得用DLLインスタンスハンドルの取得
//-------------------------------------------------------------------

/// Getter: リソースの取得用DLLインスタンスハンドル
HINSTANCE dll_instance();
/// Setter: リソースの取得用DLLインスタンスハンドル
void set_dll_instance(HINSTANCE dll_instance);

//-------------------------------------------------------------------
// イメージの操作
//-------------------------------------------------------------------

/// ピクセルフォーマットがTopdownか
bool IsTopdownPixelFormat(ImagePixelFormat pixel_format);

/// drawutilsが使用可能なピクセルフォーマットか
bool CanUseDrawUtils(ImagePixelFormat pixel_format);

//-------------------------------------------------------------------
// イメージのタイプ（サイズ、形式など）
//-------------------------------------------------------------------

/// イメージのサイズを求める
int CalculateDataSize(ImagePixelFormat pixel_format,
                      int width, int height);
/// イメージのサイズを直接求める
int CalculateImageSize(const Image &image);

/// AVPixelFormatを取得
AVPixelFormat ToAVPicturePixelFormat(ImagePixelFormat pixel_format);

/// BITMAPINFOHEADERを取得
void ToWindowsBitmapInfo(ImagePixelFormat pixel_format,
                         int width,
                         int height,
                         bool vertical_invert,
                         BITMAPINFO *info);
/// イメージからBITMAPINFOHEADERを取得
void ImageToWindowsBitmapInfo(const Image &image,
                              bool vertical_invert,
                              BITMAPINFO *info);

/// int(index)->enum(ImagePixelFormat)変換
/// @warning バグの元なので注意して使うこと
ImagePixelFormat IndexToPixelFormat(int index);

/// BITMAPINFOHEADERからImagePixelFormatを取得
ImagePixelFormat WindowsBitmapInfoHeaderToPixelFormat(
    const BITMAPINFOHEADER &info_header);

/// BITMAPINFOHEADERから対応ピクセルフォーマットかどうかを求める
bool IsSupportedPixelFormat(const BITMAPINFOHEADER &info_header);

//-------------------------------------------------------------------
// レイアウト
//-------------------------------------------------------------------

/// 指定された範囲（同じ座標系）が中に含まれているか
bool Contains(int bound_x, int bound_y,
              int bound_width, int bound_height,
              int x, int y, int width, int height);

/// 境界の座標系と同じ座標系の新しい配置を計算する
bool CalculateLayout(int bound_x, int bound_y,
                     int bound_width, int bound_height,
                     int input_width, int input_height,
                     bool stretch, bool keep_aspect_ratio,
                     int *new_x, int *new_y,
                     int *new_width, int *new_height);

/// 幅と高さから拡大縮小した場合のパディングサイズを求める
bool CalculatePaddingSize(int bound_width, int bound_height,
                          int input_width, int input_height,
                          bool stretch, bool keep_aspect_ratio,
                          int *padding_top, int *padding_bottom,
                          int *padding_left, int *padding_right);

/// マルチモニタを考慮してウィンドウ領域を求める
void GetWindowRectangle(HWND window, int *x, int *y,
                        int *width, int *height);
}   // namespace utilities
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_UTILITIES_H_
