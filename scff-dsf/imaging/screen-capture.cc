
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

/// @file imaging/screen-capture.cc
/// @brief imaging::ScreenCaptureの定義

#include "imaging/screen-capture.h"

#include <Windows.h>

#include "base/debug.h"
#include "imaging/windows-ddb-image.h"
#include "imaging/avpicture-image.h"
#include "imaging/utilities.h"

namespace imaging {

//=====================================================================
// imaging::ScreenCapture
//=====================================================================

// コンストラクタ
ScreenCapture::ScreenCapture(ImagePixelFormat pixel_format,
                             int width, int height,
                             const ScreenCaptureParameter &parameter)
    : Processor(pixel_format, width, height),
      parameter_(parameter),
      image_for_bitblt_(),
      image_for_swscale_(),
      dc_for_bitblt_(0),      // NULL
      info_for_getdibits_(),
      scaler_(0),             // NULL
      window_width_(-1),      // ありえない値
      window_height_(-1) {    // ありえない値
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("ScreenCapture: NEW(%d, %d, %d)"),
          pixel_format, width, height));
}

// デストラクタ
ScreenCapture::~ScreenCapture() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("ScreenCapture: DELETE")));
  // 管理しているインスタンスをすべて破棄
  // 破棄はプロセッサ→イメージの順
  if (dc_for_bitblt_ != NULL) {
    DeleteDC(dc_for_bitblt_);
  }
  if (scaler_ != 0) {   // NULL
    sws_freeContext(scaler_);
  }
  // No Child Processor
}

// Windowがキャプチャに適切な状態になっているか判定する
bool ScreenCapture::ValidateWindow() {
  // windowハンドルがそもそもNULL
  if (parameter_.window == NULL) {
    return false;
  }

  // windowが存在していない
  if (!IsWindow(parameter_.window)) {
    return false;
  }

  // ウィンドウのサイズを取得
  RECT current_window_screen_rect;
  GetWindowRect(parameter_.window, &current_window_screen_rect);
  const int current_window_width   =
      (current_window_screen_rect.right - current_window_screen_rect.left);
  const int current_window_height  =
      (current_window_screen_rect.bottom - current_window_screen_rect.top);

  // 初回のチェックの値を保存しておく
  if (window_width_ == -1) {
    window_width_ = current_window_width;
  }
  if (window_height_ == -1) {
    window_height_ = current_window_height;
  }

  // ウィンドウサイズは変更禁止(かなり厳しい条件)
  if (window_width_ != current_window_width ||
      window_height_ != current_window_height) {
    return false;
  }

  /// @todo(me) そのほかにも条件はいろいろありそうだが・・・要調査

  return true;
}

// 初期化
ErrorCode ScreenCapture::Init() {
  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("ScreenCapture: Init")));

  // ウィンドウのチェック
  if (!ValidateWindow()) {
    return ErrorOccured(kInvalidWindow);
  }

  // クリッピング領域がウィンドウ内におさまっているか？
  if (Utilities::IsClippingRegionValid(window_width_, window_height_,
                                       parameter_.clipping_x,
                                       parameter_.clipping_y,
                                       parameter_.clipping_width,
                                       parameter_.clipping_height)) {
    // ok
  } else {
    return ErrorOccured(kInvalidClippingRegion);
  }

  const int capture_width = parameter_.clipping_width;
  const int capture_height = parameter_.clipping_height;

  //-------------------------------------------------------------------
  // 初期化の順番はイメージ→プロセッサの順
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  // DDBはウィンドウのあるディスプレイと同じ形式=RGB0
  const ErrorCode error_image_for_bitblt =
      image_for_bitblt_.CreateFromWindow(capture_width, capture_height,
                                         parameter_.window);
  if (error_image_for_bitblt != kNoError) {
    return ErrorOccured(error_image_for_bitblt);
  }
  // GetDIBits用なのでRGB0で作成
  const ErrorCode error_image_for_swscale =
      image_for_swscale_.Create(kRGB0, capture_width, capture_height);
  if (error_image_for_swscale != kNoError) {
    return ErrorOccured(error_image_for_swscale);
  }
  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  // nop
  //-------------------------------------------------------------------

  // 取り込み用BITMAPINFOを作成
  info_for_getdibits_ =
      Utilities::ImageToWindowsBitmapInfo(image_for_bitblt_);

  // 取り込み用DCを作成 (SelectObjectで過去の値は放棄)
  HDC window_dc = GetDC(parameter_.window);
  dc_for_bitblt_ = CreateCompatibleDC(window_dc);
  SelectObject(dc_for_bitblt_, image_for_bitblt_.windows_ddb());
  ReleaseDC(parameter_.window, window_dc);

  // 拡大縮小用のコンテキストを作成
  struct SwsContext *scaler = 0;    // NULL
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
    input_pixel_format = PIX_FMT_RGB0;
    break;
  }
  scaler = sws_getCachedContext(NULL,
      capture_width, capture_height, input_pixel_format,
      width(), height(),
      Utilities::ToAVPicturePixelFormat(pixel_format()),
      parameter_.sws_flags, NULL, NULL, NULL);
  if (scaler == NULL) {
    return ErrorOccured(kOutOfMemoryError);
  }
  scaler_ = scaler;

  // 初期化は成功
  return InitDone();
}

// リクエストに対する処理を行う
ErrorCode ScreenCapture::Accept(Request *request) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("ScreenCapture: Accept")));
  if (GetCurrentError() != kNoError) {
    // 何かエラーが発生している場合は何もしない
    return GetCurrentError();
  }

  /// @todo(me) ここでいろいろやる

  // No Child Processor
  return NoError();
}

// 渡されたDCにカーソルを描画する
void ScreenCapture::DrawCursor(HDC dc) {
  POINT cursor_screen_point;
  GetCursorPos(&cursor_screen_point);

  RECT window_screen_rect;
  GetWindowRect(parameter_.window, &window_screen_rect);
  const int cursor_x = cursor_screen_point.x - window_screen_rect.left;
  const int cursor_y = cursor_screen_point.y - window_screen_rect.top;

  CURSORINFO cursor_info;
  ZeroMemory(&cursor_info, sizeof(cursor_info));
  cursor_info.cbSize = sizeof(cursor_info);
  cursor_info.flags = CURSOR_SHOWING;

  ICONINFO icon_info;

  if (GetCursorInfo(&cursor_info) &&
      GetIconInfo(cursor_info.hCursor, &icon_info)) {
    DrawIcon(dc,
             cursor_x - icon_info.xHotspot,
             cursor_y - icon_info.yHotspot,
             cursor_info.hCursor);

    DeleteObject(icon_info.hbmColor);
    DeleteObject(icon_info.hbmMask);
  }
}

// 渡されたポインタにデータを設定する
ErrorCode ScreenCapture::PullAVPictureImage(AVPictureImage *image) {
  if (GetCurrentError() != kNoError) {
    // 何かエラーが発生している場合は何もしない
    return GetCurrentError();
  }

  // ウィンドウのチェック
  if (!ValidateWindow()) {
    return ErrorOccured(kInvalidWindow);
  }

  /// @todo(me) カラーチェック。重いのでどこでやるか考え中
  //if (GetDeviceCaps(window_dc, BITSPIXEL) != 32) {
  //  ReleaseDC(parameter_.window, window_dc);
  //  return ErrorOccured(kNot32bitColor);
  //}

  DWORD raster_operation = SRCCOPY;
  if (parameter_.show_layered_window) {
    raster_operation = SRCCOPY | CAPTUREBLT;
  }

  /// @todo(me) 可視領域についてまったく考慮していない
  const int capture_width = parameter_.clipping_width;
  const int capture_height = parameter_.clipping_height;

  /// @attention スライス機能に関するテストの結果
  ///- BitBltを分割してオンスクリーンウィンドウに対する操作を分けてみた
  ///  - スループットの向上がなかったのを確認
  ///- マウスフリッカリングは少し減った？
  ///  - 本当に気になるならレイヤードウィンドウ取り込みを切る方がよい
  ///- 出力品質に問題発生
  ///  - 画面操作は重くならなくてもエンコーダ側でかなりひどい
  ///    フレームスキップを確認
  ///- 結論: 分割キャプチャは特に意味なし
  ///  - マルチスレッド化しても結局オンスクリーンウィンドウへの
  ///    アクセスが減らせるわけではないのでかわらなそう
  const int slice_height = 100000;
  const int slice_count = capture_height / slice_height;
  const int slice_odd   = capture_height % slice_height;

  for (int i = 0; i < slice_count; i++) {
    const int i_y       = i * slice_height;

    // スクリーンキャプチャ
    HDC window_dc = GetDC(parameter_.window);
    BitBlt(dc_for_bitblt_, 0, i_y, capture_width, slice_height,
           window_dc, parameter_.clipping_x, parameter_.clipping_y + i_y,
           raster_operation);
    ReleaseDC(parameter_.window, window_dc);
  }
  // 余りがあればもう一回
  if (slice_odd > 0) {
    const int y           = slice_count * slice_height;
    const int height_odd  = capture_height - y;

    // スクリーンキャプチャ
    HDC window_dc = GetDC(parameter_.window);
    BitBlt(dc_for_bitblt_, 0, y, capture_width, height_odd,
           window_dc, parameter_.clipping_x, parameter_.clipping_y + y,
           raster_operation);
    ReleaseDC(parameter_.window, window_dc);
  }

  if (parameter_.show_cursor) {
    DrawCursor(dc_for_bitblt_);
  }

  // capture_bitmap_への書き込み
  GetDIBits(dc_for_bitblt_,
            image_for_bitblt_.windows_ddb(),
            0, capture_height,
            image_for_swscale_.raw_bitmap(),
            &info_for_getdibits_,
            DIB_RGB_COLORS);

  // SWScaleを使って拡大・縮小を行う
  int scale_height = -1;    // ありえない値
  switch (pixel_format()) {
  case kI420:
  case kUYVY:
    /// @attention RGB->YUV変換時に上下が逆になるのを修正
    AVPicture flip_horizontal_image_for_swscale;
    Utilities::FlipHorizontal(
        image_for_swscale_.avpicture(),
        capture_height,
        &flip_horizontal_image_for_swscale);

    // 拡大縮小
    scale_height =
        sws_scale(scaler_,
                  flip_horizontal_image_for_swscale.data,
                  flip_horizontal_image_for_swscale.linesize,
                  0, capture_height,
                  image->avpicture()->data,
                  image->avpicture()->linesize);
    ASSERT(scale_height == height());
    break;
  case kRGB0:
    // 拡大縮小
    scale_height =
        sws_scale(scaler_,
                  image_for_swscale_.avpicture()->data,
                  image_for_swscale_.avpicture()->linesize,
                  0, capture_height,
                  image->avpicture()->data,
                  image->avpicture()->linesize);
    ASSERT(scale_height == height());
    break;
  }

  // エラー発生なし
  return NoError();
}
}   // namespace imaging
