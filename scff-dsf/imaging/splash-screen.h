
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

/// @file imaging/splash-screen.h
/// @brief imaging::SplashScreenの宣言

#ifndef SCFF_DSF_IMAGING_SPLASH_SCREEN_H_
#define SCFF_DSF_IMAGING_SPLASH_SCREEN_H_

#include "imaging/processor.h"
#include "imaging/avpicture-image.h"

namespace imaging {

/// @brief スプラッシュスクリーンを表示する
class SplashScreen : public Processor {
 public:
  /// @brief コンストラクタ
  SplashScreen(ImagePixelFormat pixel_format, int width, int height);

  //-------------------------------------------------------------------
  /// @brief デストラクタ
  ~SplashScreen();
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Accept
  ErrorCode Accept(Request* request);
  /// @copydoc Processor::PullAVPictureImage
  ErrorCode PullAVPictureImage(AVPictureImage *image);
  //-------------------------------------------------------------------

 private:
  //-------------------------------------------------------------------
  // (copy禁止)
  //-------------------------------------------------------------------
  /// @brief コピーコンストラクタ
  SplashScreen(const SplashScreen& splash_screen);
  /// @brief 代入演算子(copy禁止)
  void operator=(const SplashScreen& splash_screen);
  //-------------------------------------------------------------------
};
}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_SPLASH_SCREEN_H_
