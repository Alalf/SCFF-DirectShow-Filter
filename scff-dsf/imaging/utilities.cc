
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

/// @file imaging/utilities.cc
/// @brief imaging::Utilitiesの定義

#include "imaging/utilities.h"

extern "C" {
#include <libavcodec/avcodec.h>
}

#include "base/debug.h"
#include "imaging/imaging-types.h"
#include "imaging/avpicture-image.h"

namespace imaging {

//=====================================================================
// imaging::Utilities
//=====================================================================

//-------------------------------------------------------------------
// リソースの取得用DLLインスタンスハンドルの取得
//-------------------------------------------------------------------

/// @brief リソースの取得用DLLインスタンスハンドル
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

// 矩形を指定してビットマップをコピー
// outputイメージに収まらない場合、クリッピングされる
/// @todo(me) libavfilter/drawutils.cのff_copy_rectangle/2を移植したい
void Utilities::BitBlt(const AVPictureImage &input,
                       AVPictureImage *output, int output_x, int output_y) {
  /// @todo(me) 実装
  ASSERT(false);
}

// brief 矩形を指定したRGBカラーで塗りつぶし
// outputイメージに収まらない場合、クリッピングされる
/// @todo(me) libavfilter/drawutils.cのff_fill_rectangleを移植したい
void Utilities::FillRect(AVPictureImage *output, int output_x, int output_y,
                         int output_width, int output_height) {
  /// @todo(me) 実装
  ASSERT(false);
}

/// @brief イメージ全体を黒で塗りつぶす
/// @todo(me) FillRectがあればそれを利用して実装する
void Utilities::Clear(AVPictureImage *output) {
  /// @todo(me) 実装
  ASSERT(false);

  // RGB0限定ならZeroMemoryでよい
  if (output->pixel_format() == kRGB0) {
    const int data_size = CalcImageSize(*output);
    ZeroMemory(output->avpicture()->data[0], data_size);
    return;
  }
}

//-------------------------------------------------------------------
// イメージのタイプ
//-------------------------------------------------------------------

// イメージのサイズを求める
int Utilities::CalcDataSize(ImagePixelFormat pixel_format,
                            int width, int height) {
  return avpicture_get_size(ToAVPicturePixelFormat(pixel_format),
                            width, height);
}

// イメージのサイズを直接求める
int Utilities::CalcImageSize(const Image &image) {
  return CalcDataSize(image.pixel_format(),
                      image.width(),
                      image.height());
}

// AVPicture用PixelFormatを取得
/// @attention ピクセルフォーマットを追加するときはここを修正すること
PixelFormat Utilities::ToAVPicturePixelFormat(ImagePixelFormat pixel_format) {
  switch (pixel_format) {
  case kI420:
    return PIX_FMT_YUV420P;
    break;
  case kUYVY:
    return PIX_FMT_UYVY422;
    break;
  case kRGB0:
    return PIX_FMT_RGB0;
    break;
  }

  ASSERT(false);
  return PIX_FMT_NONE;
}

// BITMAPINFOHEADERを取得
/// @attention ピクセルフォーマットを追加するときはここを修正すること
BITMAPINFO Utilities::ToWindowsBitmapInfo(ImagePixelFormat pixel_format,
                                          int width,
                                          int height) {
  BITMAPINFO info;
  ZeroMemory(&info, sizeof(BITMAPINFO));
  // どの形式でもカラーテーブルはない
  info.bmiHeader.biSize          = sizeof(BITMAPINFOHEADER);
  info.bmiHeader.biWidth         = width;
  info.bmiHeader.biHeight        = height;
  info.bmiHeader.biPlanes        = 1;
  info.bmiHeader.biSizeImage     = static_cast<DWORD>(
      CalcDataSize(pixel_format, width, height));
  info.bmiHeader.biXPelsPerMeter = 0;
  info.bmiHeader.biYPelsPerMeter = 0;
  info.bmiHeader.biClrUsed       = 0;
  info.bmiHeader.biClrImportant  = 0;
  switch (pixel_format) {
  case kI420:
    info.bmiHeader.biBitCount      = 12;
    info.bmiHeader.biCompression   = MAKEFOURCC('I', '4', '2', '0');
    break;
  case kUYVY:
    info.bmiHeader.biBitCount      = 16;
    info.bmiHeader.biCompression   = MAKEFOURCC('U', 'Y', 'V', 'Y');
    break;
  case kRGB0:
    info.bmiHeader.biBitCount      = 32;
    info.bmiHeader.biCompression   = BI_RGB;
    break;
  }
  return info;
}

// イメージからBITMAPINFOHEADERを取得
BITMAPINFO Utilities::ImageToWindowsBitmapInfo(const Image &image) {
  return Utilities::ToWindowsBitmapInfo(image.pixel_format(),
                                        image.width(),
                                        image.height());
}

//-------------------------------------------------------------------
// レイアウト
//-------------------------------------------------------------------

/// @brief 指定された範囲（同じ座標系）が中に含まれているか
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
         right <= bound_right && bottom <= bound_bottom;
}

/// @brief クリッピング範囲（ローカル座標系）が正しいかどうか
bool Utilities::IsClippingRegionValid(int bound_width, int bound_height,
                                      int clipping_x, int clipping_y,
                                      int clipping_width,
                                      int clipping_height) {
  ASSERT(bound_width >= 0 && bound_height >= 0);
  return clipping_x >= 0 && clipping_y >= 0 &&
         clipping_width <= bound_width && clipping_height <= bound_height;
}

/// @brief 境界の座標系と同じ座標系の新しい配置を計算する
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

/// @brief 幅と高さから拡大縮小した場合のパディングサイズを求める
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

}   // namespace imaging
