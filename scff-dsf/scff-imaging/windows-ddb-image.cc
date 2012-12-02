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

/// @file scff-imaging/windows-ddb-image.cc
/// @brief scff_imaging::WindowsDDBImageの定義

#include "scff-imaging/windows-ddb-image.h"

#include <tchar.h>

#include "scff-imaging/debug.h"
#include "scff-imaging/imaging-types.h"
#include "scff-imaging/utilities.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::WindowsDDBImage
//=====================================================================

// コンストラクタ
WindowsDDBImage::WindowsDDBImage()
    : Image(),
      windows_ddb_(nullptr),
      from_(Source::kInvalidSource) {
  /// @attention windows_ddb_そのものの構築はCreateで行う
}

// デストラクタ
WindowsDDBImage::~WindowsDDBImage() {
  if (!IsEmpty()) {
    DeleteObject(windows_ddb_);
  }
}

// Create()などによって実体がまだ生成されていない場合
bool WindowsDDBImage::IsEmpty() const {
  return windows_ddb_ == nullptr;
}

// リソースから実体を作る
ErrorCode WindowsDDBImage::CreateFromResource(int width, int height,
                                              WORD resource_id) {
  // pixel_format, width, height, fromを設定する
  /// @attention WindowsビットマップはRGB0(32bit)限定
  ErrorCode error_create = Image::Create(ImagePixelFormat::kRGB0, width, height);
  if (error_create != ErrorCode::kNoError) {
    return error_create;
  }
  from_ = Source::kFromResource;

  // LoadImageをつかってDDBを読み込む
  /// @attention Bitmapイメージの形式はチェックしていない
  HBITMAP windows_ddb = static_cast<HBITMAP>(
      LoadImage(Utilities::dll_instance(),
                MAKEINTRESOURCE(resource_id),
                IMAGE_BITMAP,
                0, 0, 0));

  // ロード失敗
  if (windows_ddb == nullptr) {
    return ErrorCode::kWindowsDDBImageCannotLoadResourceImageError;
  }

  windows_ddb_ = windows_ddb;
  return ErrorCode::kNoError;
}

// 与えられたWindowからCompatibleBitmapを作成する
ErrorCode WindowsDDBImage::CreateFromWindow(int width, int height,
                                            HWND window) {
  // pixel_format, width, height, fromを設定する
  /// @attention WindowsビットマップはRGB0(32bit)限定
  ErrorCode error_create = Image::Create(ImagePixelFormat::kRGB0, width, height);
  if (error_create != ErrorCode::kNoError) {
    return error_create;
  }
  from_ = Source::kFromWindow;

  // ウィンドウから情報を得るためのDC
  HDC original_dc = GetDC(window);
  if (original_dc == nullptr) {
    return ErrorCode::kWindowsDDBImageCannotGetDCFromWindowError;
  }

  // 32Bitイメージ以外は作成できない
  const int bpp = GetDeviceCaps(original_dc, BITSPIXEL);
  if (bpp != 32) {
    ASSERT(false);
    return ErrorCode::kWindowsDDBImageNotRGB32WindowError;
  }

  HBITMAP windows_ddb =
      CreateCompatibleBitmap(original_dc, width, height);
  if (windows_ddb == nullptr) {
    ReleaseDC(window, original_dc);
    return ErrorCode::kWindowsDDBImageOutOfMemoryError;
  }
  ReleaseDC(window, original_dc);

  windows_ddb_ = windows_ddb;

  return ErrorCode::kNoError;
}

// Getter: Windowsビットマップハンドル
HBITMAP WindowsDDBImage::windows_ddb() const {
  return windows_ddb_;
}
}   // namespace scff_imaging
