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

/// @file scff_imaging/splash_screen.h
/// scff_imaging::SplashScreenの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_SPLASH_SCREEN_H_
#define SCFF_DSF_SCFF_IMAGING_SPLASH_SCREEN_H_

#include "scff_imaging/layout.h"
#include "scff_imaging/avframe_bitmap_image.h"
#include "scff_imaging/windows_ddb_image.h"

namespace scff_imaging {

class Scale;
class Padding;

/// スプラッシュスクリーンを表示する
class SplashScreen : public Layout {
 public:
  /// コンストラクタ
  SplashScreen();
  /// デストラクタ
  ~SplashScreen();

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCodes Init();
  /// @copydoc Processor::Run
  ErrorCodes Run();
  //-------------------------------------------------------------------

 private:
  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  /// 拡大縮小ピクセルフォーマット変換
  Scale *scale_;
  /// パディング
  Padding *padding_;
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  /// リソースのビットマップ読み込み用
  WindowsDDBImage resource_ddb_;
  /// GetDIBits用
  AVFrameBitmapImage resource_image_;
  /// 変換後
  AVFrameImage converted_image_;
  //-------------------------------------------------------------------

  /// 取り込み用BITMAPINFO
  BITMAPINFO resource_ddb_info_;

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(SplashScreen);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_SPLASH_SCREEN_H_
