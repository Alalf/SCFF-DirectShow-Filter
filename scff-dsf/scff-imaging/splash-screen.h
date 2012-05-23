
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

/// @file scff-imaging/splash-screen.h
/// @brief scff_imaging::SplashScreenの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_SPLASH_SCREEN_H_
#define SCFF_DSF_SCFF_IMAGING_SPLASH_SCREEN_H_

#include "scff-imaging/layout.h"
#include "scff-imaging/avpicture-with-fill-image.h"
#include "scff-imaging/windows-ddb-image.h"

namespace scff_imaging {

class Scale;
class Padding;

/// @brief スプラッシュスクリーンを表示する
class SplashScreen : public Layout {
 public:
  /// @brief コンストラクタ
  SplashScreen();
  /// @brief デストラクタ
  ~SplashScreen();

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Run
  ErrorCode Run();
  //-------------------------------------------------------------------

 private:
  /// @brief 設定されたOutputImageはPadding可能か？
  bool CanUsePadding() const;

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  /// @brief 拡大縮小ピクセルフォーマット変換
  Scale *scale_;
  /// @brief パディング
  Padding *padding_;
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  /// @brief リソースのビットマップ読み込み用
  WindowsDDBImage resource_ddb_;
  /// @brief GetDIBits用
  AVPictureWithFillImage resource_image_;
  /// @brief 変換後
  AVPictureImage converted_image_;
  //-------------------------------------------------------------------

  /// @brief 取り込み用BITMAPINFO
  BITMAPINFO resource_ddb_info_;

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(SplashScreen);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_SPLASH_SCREEN_H_
