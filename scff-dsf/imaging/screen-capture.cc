
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
#include "imaging/avpicture-with-fill-image.h"
#include "imaging/utilities.h"

namespace imaging {

//=====================================================================
// imaging::ScreenCapture
//=====================================================================

// コンストラクタ
ScreenCapture::ScreenCapture(int size,
                             ScreenCaptureParameter parameter[kMaxMultiProcessorSize])
    : Processor<void, AVPictureWithFillImage>(size) {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
            TEXT("ScreenCapture: NEW(%d)"),
            size));
  // 配列の初期化
  for (int i = 0; i < kMaxMultiProcessorSize; i++) {
    parameter_[i] = parameter[i];
    dc_for_bitblt_[i] = NULL;
    window_width_[i] = -1;    // ありえない値
    window_height_[i] = -1;   // ありえない値
    raster_operation_[i] = SRCCOPY;
  }
  // 以下のメンバは明示的に初期化していない
  // image_for_bitblt_
  // info_for_getdibits_
}

// デストラクタ
ScreenCapture::~ScreenCapture() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("ScreenCapture: DELETE")));
  // 管理しているインスタンスをすべて破棄
  // 破棄はプロセッサ→イメージの順
  for (int i = 0; i < kMaxMultiProcessorSize; i++) {
    if (dc_for_bitblt_[i] != NULL) {
      DeleteDC(dc_for_bitblt_[i]);
      dc_for_bitblt_[i] = NULL;
    }
  }
  // No Child Processor
}

// Windowがキャプチャに適切な状態になっているか判定する
bool ScreenCapture::ValidateWindow(int index) {
  ASSERT(0 <= index && index < size());

  // ウィンドウハンドル
  HWND window = parameter_[index].window;

  // windowハンドルがそもそもNULL
  if (window == NULL) {
    return false;
  }

  // windowが存在していない
  if (!IsWindow(window)) {
    return false;
  }

  // ウィンドウのサイズを取得
  RECT current_window_screen_rect;
  GetWindowRect(window, &current_window_screen_rect);
  const int current_window_width   =
      (current_window_screen_rect.right - current_window_screen_rect.left);
  const int current_window_height  =
      (current_window_screen_rect.bottom - current_window_screen_rect.top);

  // 初回のチェックの値を保存しておく
  if (window_width_[index] == -1) {
    window_width_[index] = current_window_width;
  }
  if (window_height_[index] == -1) {
    window_height_[index] = current_window_height;
  }

  // ウィンドウサイズは変更禁止(かなり厳しい条件)
  if (window_width_[index] != current_window_width ||
      window_height_[index] != current_window_height) {
    return false;
  }

  /// @todo(me) カラーチェック。オンスクリーンDCが必要なことに注意
  /// @code
  /// if (GetDeviceCaps(window_dc, BITSPIXEL) != 32) {
  ///   return ErrorOccured(kNot32bitColorError);
  /// }
  /// @endcode

  /// @todo(me) 可視領域についても考慮するべきかも

  return true;
}

// インデックスを指定して初期化
ErrorCode ScreenCapture::InitByIndex(int index) {
  ASSERT(0<= index && index < size());

  // RGB0以外の出力はできない
  ASSERT(GetOutputImage(index)->pixel_format() == kRGB0);

  if (!ValidateWindow(index)) {
    return kInvalidWindowError;
  }

  // クリッピング領域がウィンドウ内におさまっているか？
  if (Utilities::IsClippingRegionValid(window_width_[index],
                                       window_height_[index],
                                       parameter_[index].clipping_x,
                                       parameter_[index].clipping_y,
                                       parameter_[index].clipping_width,
                                       parameter_[index].clipping_height)) {
    // ok
  } else {
    return kInvalidClippingRegionError;
  }

  const int capture_width = parameter_[index].clipping_width;
  const int capture_height = parameter_[index].clipping_height;

  //-------------------------------------------------------------------
  // 初期化の順番はイメージ→プロセッサの順
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  // DDBはウィンドウのあるディスプレイと同じ形式=RGB0
  const ErrorCode error_image_for_bitblt =
      image_for_bitblt_[index].CreateFromWindow(
          capture_width, capture_height,
          parameter_[index].window);
  if (error_image_for_bitblt != kNoError) {
    return error_image_for_bitblt;
  }
  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  // nop
  //-------------------------------------------------------------------

  // 取り込み用BITMAPINFOを作成
  info_for_getdibits_[index] =
      Utilities::ImageToWindowsBitmapInfo(image_for_bitblt_[index]);

  // 取り込み用DCを作成 (SelectObjectで過去の値は放棄)
  HDC window_dc = GetDC(parameter_[index].window);
  dc_for_bitblt_[index] = CreateCompatibleDC(window_dc);
  SelectObject(dc_for_bitblt_[index], image_for_bitblt_[index].windows_ddb());
  ReleaseDC(parameter_[index].window, window_dc);

  // BitBltに渡すラスターオペレーションコードを作成
  if (parameter_[index].show_layered_window) {
    raster_operation_[index] = SRCCOPY | CAPTUREBLT;
  } else {
    raster_operation_[index] = SRCCOPY;
  }

  // エラーなし
  return kNoError;
}

// 初期化
ErrorCode ScreenCapture::Init() {
  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("ScreenCapture: Init")));

  for (int i = 0; i < size(); i++) {
    const ErrorCode error = InitByIndex(i);
    if (error != kNoError) {
      // 一つでもエラーが起きたら全てエラー扱いとする
      return ErrorOccured(error);
    }
  }

  // 初期化は成功
  return InitDone();
}

// 渡されたDCにカーソルを描画する
void ScreenCapture::DrawCursor(HDC dc, HWND window) {
  POINT cursor_screen_point;
  GetCursorPos(&cursor_screen_point);

  RECT window_screen_rect;
  GetWindowRect(window, &window_screen_rect);
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
ErrorCode ScreenCapture::Run() {
  // 何かエラーが発生している場合は何もしない
  if (GetCurrentError() != kNoError) {
    return GetCurrentError();
  }

  // 全てのウインドウのチェック
  for (int i = 0; i < size(); i++) {
    if (!ValidateWindow(i)) {
      return ErrorOccured(kInvalidWindowError);
    }
  }

  // まとめてスクリーンキャプチャ
  for (int i = 0; i < size(); i++) {
    // オンスクリーンDCの取得期間は最小限にすること！
    // なおVGAのキャッシュは取り込み画像に比べて小さすぎるので、
    // キャッシュミス関連で気をつけるべきことはない
    HDC window_dc = GetDC(parameter_[i].window);
    BitBlt(dc_for_bitblt_[i],
           0, 0,
           parameter_[i].clipping_width, parameter_[i].clipping_height,
           window_dc,
           parameter_[i].clipping_x, parameter_[i].clipping_y,
           raster_operation_[i]);
    ReleaseDC(parameter_[i].window, window_dc);
  }

  // 以下オフスクリーンビットマップに対する操作
  for (int i = 0; i < size(); i++) {
    if (parameter_[i].show_cursor) {
      DrawCursor(dc_for_bitblt_[i], parameter_[i].window);
    }

    // OutputImageへの書き込み
    GetDIBits(dc_for_bitblt_[i],
              image_for_bitblt_[i].windows_ddb(),
              0, parameter_[i].clipping_height,
              GetOutputImage(i)->raw_bitmap(),
              &(info_for_getdibits_[i]),
              DIB_RGB_COLORS);
  }

  // エラー発生なし
  return NoError();
}
}   // namespace imaging
