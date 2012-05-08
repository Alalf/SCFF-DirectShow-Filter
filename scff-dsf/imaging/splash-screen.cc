
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

/// @file imaging/splash-screen.cc
/// @brief imaging::SplashScreenの定義

#include "imaging/splash-screen.h"

extern "C" {
#include <libswscale/swscale.h>
}
#include <libavfilter/drawutils.h>

#include "resource.h"   // NOLINT
#include "base/debug.h"
#include "imaging/utilities.h"
#include "imaging/avpicture-image.h"
#include "imaging/avpicture-with-fill-image.h"
#include "imaging/windows-ddb-image.h"

namespace imaging {

//=====================================================================
// imaging::SplashScreen
//=====================================================================

//コンストラクタ
SplashScreen::SplashScreen(ImagePixelFormat pixel_format,
                           int width, int height)
    : Processor(pixel_format, width, height) {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("SplashScreen: NEW(%d, %d, %d)"),
          pixel_format, width, height));
}

//-------------------------------------------------------------------

// デストラクタ
SplashScreen::~SplashScreen() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("SplashScreen: DELETE")));
}

// Processor::Initの実装
ErrorCode SplashScreen::Init() {
  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("SplashScreen: Init")));

  //-------------------------------------------------------------------
  // 初期化の順番はイメージ→プロセッサの順
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  // nop
  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  // nop
  //-------------------------------------------------------------------

  return InitDone();
}

// Processor::Acceptの実装
ErrorCode SplashScreen::Accept(Request *request) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("SplashScreen: Accept")));
  if (GetCurrentError() != kNoError) {
    // 何かエラーが発生している場合は何もしない
    return GetCurrentError();
  }

  /// @todo(me) ここでいろいろやる

  // No Child Processor
  return NoError();
}

// Processor::PullImageの実装
/// @todo(me) 全体的に汚すぎるので徹底的に書き直す
ErrorCode SplashScreen::PullAVPictureImage(AVPictureImage *image) {
  if (GetCurrentError() != kNoError) {
    // 何かエラーが発生している場合は何もしない
    return GetCurrentError();
  }

  //-------------------------------------------------------------------
  // リソースからのビットマップ読み込み
  //-------------------------------------------------------------------

  // リソース画像の情報
  const int resource_width = 128;
  const int resource_height = 64;
  const WORD resource_id = IDB_SPLASH;

  // リソースをDDB化
  WindowsDDBImage resource_image;
  const ErrorCode error_resource_image =
      resource_image.CreateFromResource(resource_width,
                                        resource_height,
                                        resource_id);
  if (error_resource_image != kNoError) {
    return ErrorOccured(error_resource_image);
  }

  // 取り込み用BITMAPINFOを作成
  BITMAPINFO resource_image_info =
      Utilities::ImageToWindowsBitmapInfo(resource_image);

  // ピクセルフォーマット変換用のイメージを作成
  AVPictureWithFillImage resource_image_for_swscale;
  const ErrorCode error_resource_image_for_swscale =
      resource_image_for_swscale.Create(kRGB0,
                                        resource_width,
                                        resource_height);
  if (error_resource_image_for_swscale != kNoError) {
    return ErrorOccured(error_resource_image_for_swscale);
  }

  // GetDIBitsを利用してビットマップデータを転送
  HDC resource_dc = CreateCompatibleDC(NULL);
  SelectObject(resource_dc, resource_image.windows_ddb());
  GetDIBits(resource_dc, resource_image.windows_ddb(),
            0, resource_height,
            resource_image_for_swscale.raw_bitmap(),
            &resource_image_info,
            DIB_RGB_COLORS);
  DeleteDC(resource_dc);

  //-------------------------------------------------------------------
  // パディング
  //-------------------------------------------------------------------
  int splash_width = width();
  int splash_height = height();
  int padding_top = 0;
  int padding_bottom = 0;
  int padding_left = 0;
  int padding_right = 0;
  FFDrawContext draw_context;
  FFDrawColor padding_color;

  /// @warning 2012/05/08現在drawutilsはPlaner Formatにしか対応していない
  bool can_use_drawutils = false;
  switch (pixel_format()) {
  case kI420:
  case kRGB0:
    can_use_drawutils = true;
    break;
  case kUYVY:
    can_use_drawutils = false;
    break;
  }
  if (can_use_drawutils) {
    // パディング用のコンテキスト・カラーの初期化
    const int error_init =
        ff_draw_init(&draw_context,
                     Utilities::ToAVPicturePixelFormat(pixel_format()),
                     0);
    ASSERT(error_init == 0);
    uint8_t rgba_padding_color[4] = {0,0,0,0};    // black
    ff_draw_color(&draw_context,
                  &padding_color,
                  rgba_padding_color);

    // パディングサイズの計算
    const bool no_error = Utilities::CalculatePaddingSize(
        width(), height(),
        resource_width, resource_height,
        false, true,
        &padding_top, &padding_bottom,
        &padding_left, &padding_right);
    ASSERT(no_error);

    // パディング分だけサイズを小さくする
    splash_width -= padding_left + padding_right;
    splash_height -= padding_top + padding_bottom;
  }

  //-------------------------------------------------------------------
  // ピクセルフォーマット変換＋拡大縮小
  //-------------------------------------------------------------------
  struct SwsContext *scaler = 0;  // NULL
  PixelFormat input_pixel_format = PIX_FMT_NONE;
  switch (pixel_format()) {
  case kI420:
  case kUYVY:
    // I420/UYVY: 入力:BGR0(32bit) 出力:I420(12bit)/UYVY(16bit)
    /// @attention RGB->YUV変換時にUVが逆になるのを修正
    ///- RGBデータをBGRデータとしてSwsContextに渡してあります
    input_pixel_format = PIX_FMT_BGR0;
    break;
  case kRGB0:
    // RGB0: 入力:RGB0(32bit) 出力:RGB0(32bit)
    input_pixel_format  = PIX_FMT_RGB0;
    break;
  }
  scaler = sws_getCachedContext(NULL,
      resource_width, resource_height, input_pixel_format,
      splash_width, splash_height,
      Utilities::ToAVPicturePixelFormat(pixel_format()),
      kLanczos, NULL, NULL, NULL);
  if (scaler == NULL) {
    return ErrorOccured(kOutOfMemoryError);
  }

  //-------------------------------------------------------------------
  // SWScaleによる拡大・縮小
  //-------------------------------------------------------------------

  /// @attention RGB->YUV変換時に上下が逆になるのを修正
  const bool need_horizontal_flip =
      pixel_format() == kI420 || pixel_format() == kUYVY;
  AVPicture flip_horizontal_image_for_swscale;
  if (need_horizontal_flip) {
    Utilities::FlipHorizontal(
        resource_image_for_swscale.avpicture(),
        resource_height,
        &flip_horizontal_image_for_swscale);
  }

  // パディング不可能な場合はそのまま出力
  if (!can_use_drawutils) {
    ASSERT(pixel_format() == kUYVY);
    const int scale_height =
        sws_scale(scaler,
                  flip_horizontal_image_for_swscale.data,
                  flip_horizontal_image_for_swscale.linesize,
                  0, resource_height,
                  image->avpicture()->data,
                  image->avpicture()->linesize);
    ASSERT(scale_height == splash_height);
    return NoError();
  }

  // パディング可能な場合は中間イメージを新たに作成
  AVPictureImage tmp_image_for_padding;
  const ErrorCode error_tmp_image_for_padding =
      tmp_image_for_padding.Create(pixel_format(),
                                    splash_width,
                                    splash_height);
  if (error_tmp_image_for_padding != kNoError) {
    return ErrorOccured(error_tmp_image_for_padding);
  }

  // 拡大縮小
  if (pixel_format() == kI420) {
    const int scale_height =
        sws_scale(scaler,
                  flip_horizontal_image_for_swscale.data,
                  flip_horizontal_image_for_swscale.linesize,
                  0, resource_height,
                  tmp_image_for_padding.avpicture()->data,
                  tmp_image_for_padding.avpicture()->linesize);
    ASSERT(scale_height == splash_height);
  } else {
    const int scale_height =
        sws_scale(scaler,
                  resource_image_for_swscale.avpicture()->data,
                  resource_image_for_swscale.avpicture()->linesize,
                  0, resource_height,
                  tmp_image_for_padding.avpicture()->data,
                  tmp_image_for_padding.avpicture()->linesize);
    ASSERT(scale_height == splash_height);
  }

  // パディング
  Utilities::PadImage(&draw_context,
                      &padding_color,
                      tmp_image_for_padding.avpicture(),
                      tmp_image_for_padding.width(),
                      tmp_image_for_padding.height(),
                      padding_left, padding_right,
                      padding_top, padding_bottom,
                      width(), height(),
                      image->avpicture());

  // エラー発生なし
  return NoError();
}

}   // namespace imaging
