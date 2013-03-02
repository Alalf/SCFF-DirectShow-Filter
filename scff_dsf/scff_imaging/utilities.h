// Copyright 2012-2013 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter(SCFF DSF).
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

enum class ErrorCodes;
enum class ImagePixelFormats;
class Image;
class AVPictureImage;

/// scff_imagingモジュールを使う上で便利な機能を集めた名前空間
namespace utilities {

//-------------------------------------------------------------------
// イメージの操作
//-------------------------------------------------------------------

/// ピクセルフォーマットがTopdownか
bool IsTopdownPixelFormat(ImagePixelFormats pixel_format);

/// drawutilsが使用可能なピクセルフォーマットか
bool CanUseDrawUtils(ImagePixelFormats pixel_format);

//-------------------------------------------------------------------
// イメージのタイプ（サイズ、形式など）
//-------------------------------------------------------------------

/// イメージのサイズを求める
int CalculateDataSize(ImagePixelFormats pixel_format,
                      int width, int height);
/// イメージのサイズを直接求める
int CalculateImageSize(const Image &image);

/// AVPixelFormatを取得
AVPixelFormat ToAVPicturePixelFormat(ImagePixelFormats pixel_format);

/// BITMAPINFOHEADERを取得
void ToWindowsBitmapInfo(ImagePixelFormats pixel_format,
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
ImagePixelFormats IndexToPixelFormat(int index);

/// BITMAPINFOHEADERからImagePixelFormatを取得
ImagePixelFormats WindowsBitmapInfoHeaderToPixelFormat(
    const BITMAPINFOHEADER &info_header);

/// BITMAPINFOHEADERから対応ピクセルフォーマットかどうかを求める
bool IsSupportedPixelFormat(const BITMAPINFOHEADER &info_header);

//-------------------------------------------------------------------
// レイアウト
//-------------------------------------------------------------------

/// 配置計算用の許容誤差
extern const double kEpsilon;

/// 指定された範囲（同じ座標系）が中に含まれているか
bool Contains(int bound_x, int bound_y,
              int bound_width, int bound_height,
              int x, int y, int width, int height);

/// 境界の座標系と同じ座標系の新しい配置を計算する
void CalculateLayout(int bound_x, int bound_y,
                     int bound_width, int bound_height,
                     int input_width, int input_height,
                     bool stretch, bool keep_aspect_ratio,
                     int *new_x, int *new_y,
                     int *new_width, int *new_height);

/// 幅と高さから拡大縮小した場合のパディングサイズを求める
void CalculatePaddingSize(int bound_width, int bound_height,
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
