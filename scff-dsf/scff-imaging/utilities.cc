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

/// @file scff-imaging/utilities.cc
/// scff_imaging::Utilitiesの定義

#include "scff-imaging/utilities.h"

extern "C" {
#include <libavcodec/avcodec.h>
}

#include "scff-imaging/debug.h"
#include "scff-imaging/imaging-types.h"
#include "scff-imaging/avpicture-image.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::Utilities
//=====================================================================

//-------------------------------------------------------------------
// リソースの取得用DLLインスタンスハンドルの取得
//-------------------------------------------------------------------

/// リソースの取得用DLLインスタンスハンドル
static HINSTANCE g_dll_instance;

// Getter: リソースの取得用DLLインスタンスハンドル
HINSTANCE Utilities::dll_instance() {
  return g_dll_instance;
}

// Setter: リソースの取得用DLLインスタンスハンドル
void Utilities::set_dll_instance(HINSTANCE dll_instance) {
  g_dll_instance = dll_instance;
}

//-------------------------------------------------------------------
// イメージの操作
//-------------------------------------------------------------------

// ピクセルフォーマットがTopdownか
bool Utilities::IsTopdownPixelFormat(ImagePixelFormat pixel_format) {
  switch (pixel_format) {
  case ImagePixelFormat::kRGB0:
    return true;
  case ImagePixelFormat::kI420:
  case ImagePixelFormat::kIYUV:
  case ImagePixelFormat::kYV12:
  case ImagePixelFormat::kUYVY:
  case ImagePixelFormat::kYUY2:
  default:
    return false;
  }
}

/// drawutilsが使用可能なピクセルフォーマットか
bool Utilities::CanUseDrawUtils(ImagePixelFormat pixel_format) {
  /// @warning 2012/05/08現在drawutilsはPlaner Formatにしか対応していない
  switch (pixel_format) {
  case ImagePixelFormat::kI420:
  case ImagePixelFormat::kIYUV:
  case ImagePixelFormat::kYV12:
  case ImagePixelFormat::kRGB0:
    return true;
  case ImagePixelFormat::kUYVY:
  case ImagePixelFormat::kYUY2:
  default:
    return false;
  }
}

//-------------------------------------------------------------------
// イメージのタイプ
//-------------------------------------------------------------------

// イメージのサイズを求める
int Utilities::CalculateDataSize(ImagePixelFormat pixel_format,
                            int width, int height) {
  return avpicture_get_size(ToAVPicturePixelFormat(pixel_format),
                            width, height);
}

// イメージのサイズを直接求める
int Utilities::CalculateImageSize(const Image &image) {
  return CalculateDataSize(image.pixel_format(),
                      image.width(),
                      image.height());
}

// AVPixelFormatを取得
/// @attention ピクセルフォーマットを追加するときはここを修正すること
AVPixelFormat Utilities::ToAVPicturePixelFormat(ImagePixelFormat pixel_format) {
  switch (pixel_format) {
  case ImagePixelFormat::kI420:
  case ImagePixelFormat::kIYUV:
  case ImagePixelFormat::kYV12:
    return AV_PIX_FMT_YUV420P;
    break;
  case ImagePixelFormat::kUYVY:
    return AV_PIX_FMT_UYVY422;
    break;
  case ImagePixelFormat::kYUY2:
    return AV_PIX_FMT_YUYV422;
    break;
  case ImagePixelFormat::kRGB0:
    return AV_PIX_FMT_RGB0;
    break;
  }

  ASSERT(false);
  return AV_PIX_FMT_NONE;
}

// BITMAPINFOHEADERを取得
/// @attention ピクセルフォーマットを追加するときはここを修正すること
void Utilities::ToWindowsBitmapInfo(ImagePixelFormat pixel_format,
                                    int width,
                                    int height,
                                    bool vertical_invert,
                                    BITMAPINFO *info) {
  ZeroMemory(info, sizeof(BITMAPINFO));
  // どの形式でもカラーテーブルはない
  info->bmiHeader.biSize          = sizeof(BITMAPINFOHEADER);
  info->bmiHeader.biWidth         = width;
  info->bmiHeader.biHeight        = vertical_invert ? -height : height;
  info->bmiHeader.biPlanes        = 1;
  /// @todo(me) GetBitmapSize(&video_info->bmiHeader)と異なるかも。要調査
  info->bmiHeader.biSizeImage     =
      static_cast<DWORD>(CalculateDataSize(pixel_format, width, height));
  info->bmiHeader.biXPelsPerMeter = 0;
  info->bmiHeader.biYPelsPerMeter = 0;
  info->bmiHeader.biClrUsed       = 0;
  info->bmiHeader.biClrImportant  = 0;
  switch (pixel_format) {
  case ImagePixelFormat::kI420:
    info->bmiHeader.biBitCount    = 12;
    info->bmiHeader.biCompression = MAKEFOURCC('I', '4', '2', '0');
    break;
  case ImagePixelFormat::kIYUV:
    info->bmiHeader.biBitCount    = 12;
    info->bmiHeader.biCompression = MAKEFOURCC('I', 'Y', 'U', 'V');
    break;
  case ImagePixelFormat::kYV12:
    info->bmiHeader.biBitCount    = 12;
    info->bmiHeader.biCompression = MAKEFOURCC('Y', 'V', '1', '2');
    break;
  case ImagePixelFormat::kUYVY:
    info->bmiHeader.biBitCount    = 16;
    info->bmiHeader.biCompression = MAKEFOURCC('U', 'Y', 'V', 'Y');
    break;
  case ImagePixelFormat::kYUY2:
    info->bmiHeader.biBitCount    = 16;
    info->bmiHeader.biCompression = MAKEFOURCC('Y', 'U', 'Y', '2');
    break;
  case ImagePixelFormat::kRGB0:
    info->bmiHeader.biBitCount    = 32;
    info->bmiHeader.biCompression = BI_RGB;
    break;
  }
}

// イメージからBITMAPINFOHEADERを取得
void Utilities::ImageToWindowsBitmapInfo(const Image &image,
                                         bool vertical_invert,
                                         BITMAPINFO *info) {
  Utilities::ToWindowsBitmapInfo(image.pixel_format(),
                                 image.width(),
                                 image.height(),
                                 vertical_invert,
                                 info);
}

// int(index)->enum(ImagePixelFormat)変換
ImagePixelFormat Utilities::IndexToPixelFormat(int index) {
  /// @attention enum->int
  ASSERT(0 <= index &&
      index < static_cast<int>(ImagePixelFormat::kSupportedPixelFormatsCount));
  /// @warning int->enum
  return static_cast<ImagePixelFormat>(index);
}

// BITMAPINFOHEADERからImagePixelFormatを取得
ImagePixelFormat Utilities::WindowsBitmapInfoHeaderToPixelFormat(
    const BITMAPINFOHEADER &info_header) {
  switch (info_header.biCompression) {
  case MAKEFOURCC('I', '4', '2', '0'):
    return ImagePixelFormat::kI420;
  case MAKEFOURCC('I', 'Y', 'U', 'V'):
    return ImagePixelFormat::kIYUV;
  case MAKEFOURCC('Y', 'V', '1', '2'):
    return ImagePixelFormat::kYV12;
  case MAKEFOURCC('U', 'Y', 'V', 'Y'):
    return ImagePixelFormat::kUYVY;
  case MAKEFOURCC('Y', 'U', 'Y', '2'):
    return ImagePixelFormat::kYUY2;
  case BI_RGB:
    if (info_header.biBitCount == 32) {
      return ImagePixelFormat::kRGB0;
    }
  }

  return ImagePixelFormat::kInvalidPixelFormat;
}

// BITMAPINFOHEADERから対応ピクセルフォーマットかどうかを求める
bool Utilities::IsSupportedPixelFormat(const BITMAPINFOHEADER &info_header) {
  return
      WindowsBitmapInfoHeaderToPixelFormat(info_header) !=
          ImagePixelFormat::kInvalidPixelFormat;
}

//-------------------------------------------------------------------
// レイアウト
//-------------------------------------------------------------------

/// 指定された範囲（同じ座標系）が中に含まれているか
bool Utilities::Contains(int bound_x, int bound_y,
                         int bound_width, int bound_height,
                         int x, int y, int width, int height) {
  ASSERT(bound_width >= 0 && bound_height >= 0 &&
         width >= 0 && height >= 0);

  // 境界の幅、高さのどちらかが0なら必ずfalse
  if (bound_width == 0 || bound_height == 0) {
    return false;
  }

  const int right = x + width;
  const int bottom = y + height;
  const int bound_right = bound_x + bound_width;
  const int bound_bottom = bound_y + bound_height;

  return bound_x <= x && x <= bound_right &&
         bound_y <= y && y <= bound_bottom &&
         right <= bound_right &&
         bottom <= bound_bottom;
}

/// 境界の座標系と同じ座標系の新しい配置を計算する
bool Utilities::CalculateLayout(int bound_x, int bound_y,
                                int bound_width, int bound_height,
                                int input_width, int input_height,
                                bool stretch, bool keep_aspect_ratio,
                                int *new_x, int *new_y,
                                int *new_width, int *new_height) {
  // 高さと幅はかならず0より上
  ASSERT(input_width > 0 && input_height > 0 &&
         bound_width > 0 && bound_height > 0);

  // 高さ、幅が境界と一致しているか？
  if (input_width == bound_width && input_height == bound_height) {
    // サイズが完全に同じならば何もしなくてもよい
    *new_x = bound_x;
    *new_y = bound_y;
    *new_width = bound_width;
    *new_height = bound_height;
    return true;
  }

  // 高さと幅の比率を求めておく
  const double bound_aspect = static_cast<double>(bound_width) / bound_height;
  const double input_aspect = static_cast<double>(input_width) / input_height;

  // inputのサイズがboundより完全に小さいかどうか
  const bool need_expand = input_width <= bound_width &&
                           input_height <= bound_height;

  // オプションごとに条件分岐
  if (!keep_aspect_ratio && need_expand && stretch ||
      !keep_aspect_ratio && !need_expand) {
    // 境界と一致させる
    *new_x = bound_x;
    *new_y = bound_y;
    *new_width = bound_width;
    *new_height = bound_height;
  } else if (keep_aspect_ratio && need_expand && stretch ||
             keep_aspect_ratio && !need_expand) {
    // アスペクト比維持しつつ拡大縮小:
    if (input_aspect >= bound_aspect) {
      // 入力のほうが横長
      //    = widthを境界にあわせる
      //    = heightの倍率はwidthの引き伸ばし比率で求められる
      *new_width = bound_width;
      *new_height = input_height * bound_width / input_width;
      ASSERT(*new_height <= bound_height);
      *new_x = bound_x;
      const int padding_height = (bound_height - *new_height) / 2;
      *new_y = bound_y + padding_height;
    } else {
      // 出力のほうが横長
      //    = heightを境界にあわせる
      //    = widthの倍率はheightの引き伸ばし比率で求められる
      *new_height = bound_height;
      *new_width = input_width * bound_height / input_height;
      ASSERT(*new_height <= bound_height);
      *new_y = bound_y;
      const int padding_width = (bound_width - *new_width) / 2;
      *new_x = bound_x + padding_width;
    }
  } else if (need_expand && !stretch) {
    // パディングを入れる
    const int padding_width = (bound_width - input_width) / 2;
    const int padding_height = (bound_height - input_height) / 2;
    *new_x = bound_x + padding_width;
    *new_y = bound_y + padding_height;
    *new_width = input_width;
    *new_height = input_height;
  } else {
    ASSERT(false);
    return false;
  }

  return true;
}

/// 幅と高さから拡大縮小した場合のパディングサイズを求める
bool Utilities::CalculatePaddingSize(int bound_width, int bound_height,
                                     int input_width, int input_height,
                                     bool stretch, bool keep_aspect_ratio,
                                     int *padding_top, int *padding_bottom,
                                     int *padding_left, int *padding_right) {
  int new_x, new_y, new_width, new_height;
  // 座標系はbound領域内
  const bool error =
      CalculateLayout(0, 0, bound_width, bound_height,
                      input_width, input_height,
                      stretch, keep_aspect_ratio,
                      &new_x, &new_y, &new_width, &new_height);
  if (error != true) {
    ASSERT(false);
    return error;
  }
  *padding_left = new_x;
  *padding_top = new_y;
  *padding_right = bound_width - (new_x + new_width);
  *padding_bottom = bound_height - (new_y + new_height);
  return true;
}

// マルチモニタを考慮してウィンドウ領域を求める
void Utilities::GetWindowRectangle(HWND window, int *x, int *y,
                                   int *width, int *height) {
  *x = 0;
  *y = 0;
  *width = 0;
  *height = 0;
  if (window == GetDesktopWindow()) {
    *x = GetSystemMetrics(SM_XVIRTUALSCREEN);
    *y = GetSystemMetrics(SM_YVIRTUALSCREEN);
    *width = GetSystemMetrics(SM_CXVIRTUALSCREEN);
    *height = GetSystemMetrics(SM_CYVIRTUALSCREEN);
  } else if (IsWindow(window)) {
    RECT window_rect;
    GetClientRect(window, &window_rect);
    *x = window_rect.left;
    *y = window_rect.top;
    *width = window_rect.right - window_rect.left;
    *height = window_rect.bottom - window_rect.top;
  }
}
}   // namespace scff_imaging
