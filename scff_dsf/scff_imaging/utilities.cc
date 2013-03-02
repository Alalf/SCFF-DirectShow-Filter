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

/// @file scff_imaging/utilities.cc
/// scff_imaging::Utilitiesの定義

#include "scff_imaging/utilities.h"

extern "C" {
#include <libavcodec/avcodec.h>
}

#include <cmath>

#include "scff_imaging/debug.h"
#include "scff_imaging/imaging_types.h"
#include "scff_imaging/avpicture_image.h"

// DLLインスタンスハンドル(DirectShow BaseClassesで定義済み)
extern HINSTANCE g_hInst;

namespace scff_imaging {

//=====================================================================
// scff_imaging::utilities
//=====================================================================
namespace utilities {

//-------------------------------------------------------------------
// リソースの取得用DLLインスタンスハンドルの取得
//-------------------------------------------------------------------

HINSTANCE dll_instance() {
  return g_hInst;
}

//-------------------------------------------------------------------
// イメージの操作
//-------------------------------------------------------------------

bool IsTopdownPixelFormat(ImagePixelFormats pixel_format) {
  switch (pixel_format) {
    case ImagePixelFormats::kRGB0: {
      return true;
    }
    case ImagePixelFormats::kI420:
    case ImagePixelFormats::kIYUV:
    case ImagePixelFormats::kYV12:
    case ImagePixelFormats::kUYVY:
    case ImagePixelFormats::kYUY2:
    default: {
      return false;
    }
  }
}

bool CanUseDrawUtils(ImagePixelFormats pixel_format) {
  /// @warning 2012/05/08現在drawutilsはPlaner Formatにしか対応していない
  switch (pixel_format) {
    case ImagePixelFormats::kI420:
    case ImagePixelFormats::kIYUV:
    case ImagePixelFormats::kYV12:
    case ImagePixelFormats::kRGB0: {
      return true;
    }
    case ImagePixelFormats::kUYVY:
    case ImagePixelFormats::kYUY2:
    default: {
      return false;
    }
  }
}

//-------------------------------------------------------------------
// イメージのタイプ
//-------------------------------------------------------------------

int CalculateDataSize(ImagePixelFormats pixel_format,
                      int width, int height) {
  return avpicture_get_size(ToAVPicturePixelFormat(pixel_format),
                            width, height);
}

int CalculateImageSize(const Image &image) {
  return CalculateDataSize(image.pixel_format(),
                      image.width(),
                      image.height());
}

/// @attention ピクセルフォーマットを追加するときはここを修正すること
AVPixelFormat ToAVPicturePixelFormat(ImagePixelFormats pixel_format) {
  switch (pixel_format) {
    case ImagePixelFormats::kI420:
    case ImagePixelFormats::kIYUV:
    case ImagePixelFormats::kYV12: {
      return AV_PIX_FMT_YUV420P;
    }
    case ImagePixelFormats::kUYVY: {
      return AV_PIX_FMT_UYVY422;
    }
    case ImagePixelFormats::kYUY2: {
      return AV_PIX_FMT_YUYV422;
    }
    case ImagePixelFormats::kRGB0: {
      return AV_PIX_FMT_RGB0;
    }
  }

  ASSERT(false);
  return AV_PIX_FMT_NONE;
}

/// @attention ピクセルフォーマットを追加するときはここを修正すること
void ToWindowsBitmapInfo(ImagePixelFormats pixel_format,
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
    case ImagePixelFormats::kI420: {
      info->bmiHeader.biBitCount    = 12;
      info->bmiHeader.biCompression = MAKEFOURCC('I', '4', '2', '0');
      break;
    }
    case ImagePixelFormats::kIYUV: {
      info->bmiHeader.biBitCount    = 12;
      info->bmiHeader.biCompression = MAKEFOURCC('I', 'Y', 'U', 'V');
      break;
    }
    case ImagePixelFormats::kYV12: {
      info->bmiHeader.biBitCount    = 12;
      info->bmiHeader.biCompression = MAKEFOURCC('Y', 'V', '1', '2');
      break;
    }
    case ImagePixelFormats::kUYVY: {
      info->bmiHeader.biBitCount    = 16;
      info->bmiHeader.biCompression = MAKEFOURCC('U', 'Y', 'V', 'Y');
      break;
    }
    case ImagePixelFormats::kYUY2: {
      info->bmiHeader.biBitCount    = 16;
      info->bmiHeader.biCompression = MAKEFOURCC('Y', 'U', 'Y', '2');
      break;
    }
    case ImagePixelFormats::kRGB0: {
      info->bmiHeader.biBitCount    = 32;
      info->bmiHeader.biCompression = BI_RGB;
      break;
    }
  }
}

void ImageToWindowsBitmapInfo(const Image &image,
                              bool vertical_invert,
                              BITMAPINFO *info) {
  ToWindowsBitmapInfo(image.pixel_format(),
                      image.width(),
                      image.height(),
                      vertical_invert,
                      info);
}

ImagePixelFormats IndexToPixelFormat(int index) {
  /// @attention enum->int
  ASSERT(0 <= index &&
      index < static_cast<int>(ImagePixelFormats::kSupportedPixelFormatsCount));
  /// @warning int->enum
  return static_cast<ImagePixelFormats>(index);
}

ImagePixelFormats WindowsBitmapInfoHeaderToPixelFormat(
    const BITMAPINFOHEADER &info_header) {
  switch (info_header.biCompression) {
    case MAKEFOURCC('I', '4', '2', '0'): {
      return ImagePixelFormats::kI420;
    }
    case MAKEFOURCC('I', 'Y', 'U', 'V'): {
      return ImagePixelFormats::kIYUV;
    }
    case MAKEFOURCC('Y', 'V', '1', '2'): {
      return ImagePixelFormats::kYV12;
    }
    case MAKEFOURCC('U', 'Y', 'V', 'Y'): {
      return ImagePixelFormats::kUYVY;
    }
    case MAKEFOURCC('Y', 'U', 'Y', '2'): {
      return ImagePixelFormats::kYUY2;
    }
    case BI_RGB: {
      if (info_header.biBitCount == 32) {
        return ImagePixelFormats::kRGB0;
      }
    }
  }

  return ImagePixelFormats::kInvalidPixelFormat;
}

bool IsSupportedPixelFormat(const BITMAPINFOHEADER &info_header) {
  return
      WindowsBitmapInfoHeaderToPixelFormat(info_header) !=
          ImagePixelFormats::kInvalidPixelFormat;
}

//-------------------------------------------------------------------
// レイアウト
//-------------------------------------------------------------------

/// 配置計算用の許容誤差
const double kEpsilon = 0.0001;

bool Contains(int bound_x, int bound_y,
              int bound_width, int bound_height,
              int x, int y, int width, int height) {
  const int right = x + width;
  const int bottom = y + height;
  const int bound_right = bound_x + bound_width;
  const int bound_bottom = bound_y + bound_height;

  if (bound_width < 0 || bound_height < 0 ||
      width < 0 || height < 0) return false;

  // 開始点
  if (x < bound_x || y < bound_y) return false;
  // Right
  if (right <= x) {
    // widthが負の数
    if (bound_x <= bound_right || bound_right < right) return false;
  } else {
    if (bound_x <= bound_right && bound_right < right) return false;
  }
  // Bottom
  if (bottom <= y) {
    // heightが負の数
    if (bound_y <= bound_bottom || bound_bottom < bottom) return false;
  } else {
    if (bound_y <= bound_bottom && bound_bottom < bottom) return false;
  }
  return true;
}

/// 境界に合わせる
static void Fit(int bound_x, int bound_y, int bound_width, int bound_height,
                int *new_x, int *new_y, int *new_width, int *new_height) {
  *new_x = bound_x;
  *new_y = bound_y;
  *new_width = bound_width;
  *new_height = bound_height;
}

/// 比率を維持したまま拡大・縮小する
static void Letterbox(int bound_x, int bound_y,
                      int bound_width, int bound_height,
                      int input_width, int input_height,
                      int *new_x, int *new_y,
                      int *new_width, int *new_height) {
  // アスペクト比の計算
  const double bound_aspect = static_cast<double>(bound_width) / bound_height;
  const double input_aspect = static_cast<double>(input_width) / input_height;
  const bool is_letterboxing = (input_aspect >= bound_aspect);

  if (is_letterboxing) {
    // A. 境界よりも元が横長(is_letterboxing)
    //  - widthを境界にあわせる
    //  - heightの倍率はwidthの引き伸ばし比率で求められる
    *new_x = bound_x;
    *new_width = bound_width;
    const double actual_height = bound_width / input_aspect;
    const double actual_padding_height = (bound_height - actual_height) / 2.0;
    // floor
    *new_y = bound_y + static_cast<int>(actual_padding_height + kEpsilon);
    // ceil
    *new_height = static_cast<int>(std::ceil(actual_height - kEpsilon));
  } else {
    // B. 境界よりも元が縦長(!is_letterboxing = isPillarboxing)
    //  - heightを境界にあわせる
    //  - widthの倍率はheightの引き伸ばし比率で求められる
    *new_y = bound_y;
    *new_height = bound_height;
    const double actual_width = bound_height * input_aspect;
    const double actual_padding_width = (bound_width - actual_width) / 2.0;
    // floor
    *new_x = bound_x + static_cast<int>(actual_padding_width + kEpsilon);
    // ceil
    *new_width = static_cast<int>(std::ceil(actual_width - kEpsilon));
  }
  ASSERT(bound_x <= *new_x && *new_x + *new_width <= bound_x + bound_width &&
         bound_y <= *new_y && *new_y + *new_height <= bound_y + bound_height);
}

/// 拡大縮小はせずパディングだけ行う
static void Pad(int bound_x, int bound_y,
                int bound_width, int bound_height,
                int input_width, int input_height,
                int *new_x, int *new_y,
                int *new_width, int *new_height) {
  const double actual_padding_width = (bound_width - input_width) / 2.0;
  const double actual_padding_height = (bound_height - input_height) / 2.0;
  // floor
  *new_x = bound_x + static_cast<int>(actual_padding_width + kEpsilon);
  *new_y = bound_y + static_cast<int>(actual_padding_height + kEpsilon);
  *new_width = input_width;
  *new_height = input_height;
  ASSERT(bound_x <= *new_x && *new_x + *new_width <= bound_x + bound_width &&
         bound_y <= *new_y && *new_y + *new_height <= bound_y + bound_height);
}

void CalculateLayout(int bound_x, int bound_y,
                     int bound_width, int bound_height,
                     int input_width, int input_height,
                     bool stretch, bool keep_aspect_ratio,
                     int *new_x, int *new_y,
                     int *new_width, int *new_height) {
  // 高さと幅はかならず0より上
  ASSERT(input_width > 0 && input_height > 0 &&
         bound_width > 0 && bound_height > 0);

  // - 1. 高さと幅が同じ(same_size)
  // - 2. 高さと幅が共に境界より小さい(!same_size && need_expand)
  //   - 2.1 拡大しない(need_expand && !stretch)
  //   - 2.2 拡大する(need_expand && stretch)
  //     - 2.2.1 アスペクト比維持しない(... && !keep_aspect_ratio)
  //     - 2.2.2 アスペクト比維持(... && keep_aspect_ratio)
  //       - 2.2.2.1 境界よりも元が横長(... && is_letterboxing)
  //       - 2.2.2.2 境界よりも元が縦長(... && !is_letterboxing)
  // - 3. 上記以外(!same_size && !need_expand)
  //   - 3.1 アスペクト比維持しない(... && !keep_aspect_ratio)
  //   - 3.2 アスペクト比維持(... && keep_aspect_ratio)
  //     - 3.2.1 境界よりも元が横長(... && is_letterboxing)
  //     - 3.2.2 境界よりも元が縦長(... && !is_letterboxing)

  // 条件分岐用変数
  const bool same_size = (input_width == bound_width &&
                          input_height == bound_height);
  const bool need_expand = (input_width <= bound_width &&
                            input_height <= bound_height);

  // 1. 高さと幅が同じ(same_size)
  if (same_size) {
    Fit(bound_x, bound_y, bound_width, bound_height,
        new_x, new_y, new_width, new_height);
    return;
  }

  // 2. 高さと幅が共に境界より小さい
  if (need_expand) {
    // 2.1. 拡大しない(!stretch)
    if (!stretch) {
      Pad(bound_x, bound_y, bound_width, bound_height,
          input_width, input_height,
          new_x, new_y, new_width, new_height);
      return;
    }
    // 2.2. 拡大する(else)
    if (!keep_aspect_ratio) {
      // 2.2.1. アスペクト比維持しない(!keep_aspect_ratio)
      Fit(bound_x, bound_y, bound_width, bound_height,
          new_x, new_y, new_width, new_height);
      return;
    }
    // 2.2.2. アスペクト比維持(else)
    // 2.2.2.1 境界よりも元が横長(... && is_letterboxing)
    // 2.2.2.2 境界よりも元が縦長(... && !is_letterboxing)
    Letterbox(bound_x, bound_y, bound_width, bound_height,
              input_width, input_height,
              new_x, new_y, new_width, new_height);
    return;
  }

  // 3. 上記以外(else=高さか幅のどちらかが境界より大きい)
  if (!keep_aspect_ratio) {
    // 3.1. アスペクト比維持しない(!keep_aspect_ratio)
    Fit(bound_x, bound_y, bound_width, bound_height,
        new_x, new_y, new_width, new_height);
    return;
  }
  // 3.2. アスペクト比維持(else)
  // 3.2.1 境界よりも元が横長(... && is_letterboxing)
  // 3.2.2 境界よりも元が縦長(... && !is_letterboxing)
  Letterbox(bound_x, bound_y, bound_width, bound_height,
            input_width, input_height,
            new_x, new_y, new_width, new_height);
}

void CalculatePaddingSize(int bound_width, int bound_height,
                          int input_width, int input_height,
                          bool stretch, bool keep_aspect_ratio,
                          int *padding_top, int *padding_bottom,
                          int *padding_left, int *padding_right) {
  // 領域の計算
  int new_x, new_y, new_width, new_height;
  CalculateLayout(0, 0, bound_width, bound_height,
                  input_width, input_height,
                  stretch, keep_aspect_ratio,
                  &new_x, &new_y, &new_width, &new_height);

  // パディングサイズの計算
  *padding_left = new_x;
  *padding_top = new_y;
  *padding_right = bound_width - (new_x + new_width);
  *padding_bottom = bound_height - (new_y + new_height);
}

void GetWindowRectangle(HWND window, int *x, int *y,
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
  } else if (IsWindow(window) && !IsIconic(window)) {
    RECT window_rect;
    GetClientRect(window, &window_rect);
    *x = window_rect.left;
    *y = window_rect.top;
    *width = window_rect.right - window_rect.left;
    *height = window_rect.bottom - window_rect.top;
  }
}
}   // namespace utilities
}   // namespace scff_imaging
