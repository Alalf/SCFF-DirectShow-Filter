// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
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

/// @file scff_imaging/windows_ddb_image.cc
/// scff_imaging::WindowsDDBImageの定義

#include "scff_imaging/windows_ddb_image.h"

#include <tchar.h>

#include "scff_imaging/debug.h"
#include "scff_imaging/imaging_types.h"
#include "scff_imaging/utilities.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::WindowsDDBImage
//=====================================================================

WindowsDDBImage::WindowsDDBImage()
    : Image(),
      windows_ddb_(nullptr),
      from_(Sources::kInvalidSource) {
  /// @attention windows_ddb_そのものの構築はCreateで行う
}

WindowsDDBImage::~WindowsDDBImage() {
  if (!IsEmpty()) {
    DeleteObject(windows_ddb_);
  }
}

bool WindowsDDBImage::IsEmpty() const {
  return windows_ddb_ == nullptr;
}

ErrorCodes WindowsDDBImage::CreateFromResource(int width, int height,
                                               WORD resource_id) {
  // pixel_format, width, height, fromを設定する
  /// @attention WindowsビットマップはRGB0(32bit)限定
  ErrorCodes error_create =
      Image::Create(ImagePixelFormats::kRGB0, width, height);
  if (error_create != ErrorCodes::kNoError) {
    return error_create;
  }
  from_ = Sources::kFromResource;

  // LoadImageをつかってDDBを読み込む
  /// @attention Bitmapイメージの形式はチェックしていない
  HBITMAP windows_ddb = static_cast<HBITMAP>(
      LoadImage(utilities::dll_instance(),
                MAKEINTRESOURCE(resource_id),
                IMAGE_BITMAP,
                0, 0, 0));

  // ロード失敗
  if (windows_ddb == nullptr) {
    return ErrorCodes::kWindowsDDBImageCannotLoadResourceImageError;
  }

  windows_ddb_ = windows_ddb;
  return ErrorCodes::kNoError;
}

ErrorCodes WindowsDDBImage::CreateFromWindow(int width, int height,
                                            HWND window) {
  // pixel_format, width, height, fromを設定する
  /// @attention WindowsビットマップはRGB0(32bit)限定
  ErrorCodes error_create =
      Image::Create(ImagePixelFormats::kRGB0, width, height);
  if (error_create != ErrorCodes::kNoError) {
    return error_create;
  }
  from_ = Sources::kFromWindow;

  // ウィンドウから情報を得るためのDC
  HDC original_dc = GetDC(window);
  if (original_dc == nullptr) {
    return ErrorCodes::kWindowsDDBImageCannotGetDCFromWindowError;
  }

  // 32Bitイメージ以外は作成できない
  const int bpp = GetDeviceCaps(original_dc, BITSPIXEL);
  if (bpp != 32) {
    ASSERT(false);
    return ErrorCodes::kWindowsDDBImageNotRGB32WindowError;
  }

  HBITMAP windows_ddb =
      CreateCompatibleBitmap(original_dc, width, height);
  if (windows_ddb == nullptr) {
    ReleaseDC(window, original_dc);
    return ErrorCodes::kWindowsDDBImageOutOfMemoryError;
  }
  ReleaseDC(window, original_dc);

  windows_ddb_ = windows_ddb;

  return ErrorCodes::kNoError;
}

HBITMAP WindowsDDBImage::windows_ddb() const {
  return windows_ddb_;
}
}   // namespace scff_imaging
