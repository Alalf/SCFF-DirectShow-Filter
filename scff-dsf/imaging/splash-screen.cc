
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
ErrorCode SplashScreen::PullAVPictureImage(AVPictureImage *image) {
  if (GetCurrentError() != kNoError) {
    // 何かエラーが発生している場合は何もしない
    return GetCurrentError();
  }

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

  // I420のみパディングも使うのでパディング幅、高さを計算しておく
  int padding_top = 0;
  int padding_bottom = 0;
  int padding_left = 0;
  int padding_right = 0;
  int splash_width = width();
  int splash_height = height();
  int no_error = true;
  switch (pixel_format()) {
  case kI420:
    no_error =
        Utilities::CalculatePaddingSize(width(), height(),
                                        resource_width, resource_height,
                                        false, true,
                                        &padding_top, &padding_bottom,
                                        &padding_left, &padding_right);
    ASSERT(no_error);
    splash_width = width() - padding_left - padding_right;
    splash_height = height() - padding_top - padding_bottom;
    break;
  case kUYVY:
  case kRGB0:
    // nop
    break;
  }

  // ピクセルフォーマット変換＋拡大縮小用のコンテキスト作成
  // I420のみパディングも使うので拡大縮小はなし
  struct SwsContext *scaler = 0;  // NULL
  switch (pixel_format()) {
  case kI420:
  case kUYVY:
    // I420/UYVY: 入力:BGR0(32bit) 出力:I420(12bit)/UYVY(16bit)
    /// @attention RGB->YUV変換時にUVが逆になるのを修正
    ///- RGBデータをBGRデータとしてSwsContextに渡してあります
    scaler = sws_getCachedContext(NULL,
        resource_width, resource_height, PIX_FMT_BGR0,
        splash_width, splash_height,
        Utilities::ToAVPicturePixelFormat(pixel_format()),
        kLanczos, NULL, NULL, NULL);
    break;
  case kRGB0:
    // RGB0: 入力:RGB0(32bit) 出力:RGB0(32bit)
    scaler = sws_getCachedContext(NULL,
        resource_width, resource_height, PIX_FMT_RGB0,
        splash_width, splash_height,
        Utilities::ToAVPicturePixelFormat(pixel_format()),
        kLanczos, NULL, NULL, NULL);
    break;
  }
  if (scaler == NULL) {
    return ErrorOccured(kOutOfMemoryError);
  }

  // DDBからビットマップ(配列)を取り出す
  HDC resource_dc = CreateCompatibleDC(NULL);
  SelectObject(resource_dc, resource_image.windows_ddb());
  GetDIBits(resource_dc, resource_image.windows_ddb(),
            0, resource_height,
            resource_image_for_swscale.raw_bitmap(),
            &resource_image_info,
            DIB_RGB_COLORS);
  DeleteDC(resource_dc);

  // SWScaleを使って拡大・縮小を行う
  int scale_height = -1;    // ありえない値

  ErrorCode error_tmp_image_for_padding = kNoError;
  static int padding_yuv_color[3] = {0, 128, 128};
  int error_pad = -1;
  AVPicture tmp_image_for_swscale;
  AVPictureImage tmp_image_for_padding;

  switch (pixel_format()) {
  case kI420:
    /// @attention RGB->YUV変換時に上下が逆になるのを修正
    ///- 取り込みデータの中身を操作せず、ポインタをいじるだけで対処してあります
    for (int i = 0; i < 8; i++) {
      tmp_image_for_swscale.data[i] =
          resource_image_for_swscale.avpicture()->data[i]
          + resource_image_for_swscale.avpicture()->linesize[i]
              * (resource_height - 1);
      tmp_image_for_swscale.linesize[i] =
          -resource_image_for_swscale.avpicture()->linesize[i];
    }
    // 拡大縮小用の中間データを作成
    error_tmp_image_for_padding =
        tmp_image_for_padding.Create(pixel_format(),
                                     splash_width,
                                     splash_height);
    if (error_tmp_image_for_padding != kNoError) {
      return ErrorOccured(error_tmp_image_for_padding);
    }
    // 拡大縮小
    scale_height =
        sws_scale(scaler,
                  tmp_image_for_swscale.data,
                  tmp_image_for_swscale.linesize,
                  0, resource_height,
                  tmp_image_for_padding.avpicture()->data,
                  tmp_image_for_padding.avpicture()->linesize);
    ASSERT(scale_height == splash_height);
    // パディング: YUV形式の黒色(PCスケール)で埋める
    error_pad =
        av_picture_pad(image->avpicture(), tmp_image_for_padding.avpicture(),
                       height(), width(),
                       Utilities::ToAVPicturePixelFormat(pixel_format()),
                       padding_top, padding_bottom,
                       padding_left, padding_right,
                       padding_yuv_color);
    ASSERT(error_pad == 0);
    break;
  case kUYVY:
    /// @attention RGB->YUV変換時に上下が逆になるのを修正
    ///- 取り込みデータの中身を操作せず、ポインタをいじるだけで対処してあります
    for (int i = 0; i < 8; i++) {
      tmp_image_for_swscale.data[i] =
          resource_image_for_swscale.avpicture()->data[i]
          + resource_image_for_swscale.avpicture()->linesize[i]
              * (resource_height - 1);
      tmp_image_for_swscale.linesize[i] =
          -resource_image_for_swscale.avpicture()->linesize[i];
    }
    // 拡大縮小
    scale_height =
        sws_scale(scaler,
                  tmp_image_for_swscale.data,
                  tmp_image_for_swscale.linesize,
                  0, resource_height,
                  image->avpicture()->data,
                  image->avpicture()->linesize);
    ASSERT(scale_height == splash_height);
    break;
  case kRGB0:
    // 拡大縮小
    scale_height =
        sws_scale(scaler,
                  resource_image_for_swscale.avpicture()->data,
                  resource_image_for_swscale.avpicture()->linesize,
                  0, resource_height,
                  image->avpicture()->data,
                  image->avpicture()->linesize);
    ASSERT(scale_height == splash_height);
    break;
  }

  // エラー発生なし
  return NoError();
}

}   // namespace imaging
