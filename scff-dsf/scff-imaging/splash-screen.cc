
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

/// @file scff-imaging/splash-screen.cc
/// @brief scff_imaging::SplashScreenの定義

#include "scff-imaging/splash-screen.h"

#include "resource.h"   // NOLINT
#include "scff-imaging/debug.h"
#include "scff-imaging/utilities.h"
#include "scff-imaging/scale.h"
#include "scff-imaging/padding.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::SplashScreen
//=====================================================================

//コンストラクタ
SplashScreen::SplashScreen()
    : Layout(),
      scale_(0),      // NULL
      padding_(0) {   // NULL
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
            TEXT("SplashScreen: NEW")));
  // 明示的に初期化していない
  // resource_ddb_
  // resource_image_
  // converted_image_
  // resource_ddb_info_;
}

// デストラクタ
SplashScreen::~SplashScreen() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("SplashScreen: DELETE")));
  // 管理しているインスタンスをすべて破棄
  // 破棄はプロセッサ→イメージの順
  if (scale_ != 0) {  // NULL
    delete scale_;
  }
  if (padding_ != 0) {  // NULL
    delete padding_;
  }
}

// 設定されたOutputImageはPadding可能か？
bool SplashScreen::CanUsePadding() const {
  /// @warning 2012/05/08現在drawutilsはPlaner Formatにしか対応していない
  switch (GetOutputImage()->pixel_format()) {
  case kI420:
  case kRGB0:
    return true;
  case kUYVY:
  default:
    return false;
  }
}

//-------------------------------------------------------------------

// Processor::Init
ErrorCode SplashScreen::Init() {
  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("SplashScreen: Init")));

  // あらかじめイメージのサイズを計算しておく
  const int resource_width = 128;
  const int resource_height = 64;
  int converted_width = GetOutputImage()->width();
  int converted_height = GetOutputImage()->height();
  int padding_top = 0;
  int padding_bottom = 0;
  int padding_left = 0;
  int padding_right = 0;

  if (CanUsePadding()) {
    // パディングサイズの計算
    const bool no_error = Utilities::CalculatePaddingSize(
        GetOutputImage()->width(),
        GetOutputImage()->height(),
        resource_width,
        resource_height,
        false, true,
        &padding_top, &padding_bottom,
        &padding_left, &padding_right);
    ASSERT(no_error);

    // パディング分だけサイズを小さくする
    converted_width -= padding_left + padding_right;
    converted_height -= padding_top + padding_bottom;
  }

  //-------------------------------------------------------------------
  // 初期化の順番はイメージ→プロセッサの順
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  // リソースのビットマップ読み込み用
  const WORD resource_id = IDB_SPLASH;
  const ErrorCode error_resource_ddb =
      resource_ddb_.CreateFromResource(resource_width,
                                       resource_height,
                                       resource_id);
  if (error_resource_ddb != kNoError) {
    return ErrorOccured(error_resource_ddb);
  }

  // GetDIBits用
  const ErrorCode error_resource_image =
      resource_image_.Create(kRGB0,
                             resource_width,
                             resource_height);
  if (error_resource_image != kNoError) {
    return ErrorOccured(error_resource_image);
  }

  // 変換後パディング用
  if (CanUsePadding()) {
    const ErrorCode error_converted_image =
        converted_image_.Create(GetOutputImage()->pixel_format(),
                                converted_width,
                                converted_height);
    if (error_converted_image != kNoError) {
      return ErrorOccured(error_converted_image);
    }
  }
  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  // 拡大縮小ピクセルフォーマット変換
  SWScaleConfig config;
  ZeroMemory(&config, sizeof(config));
  config.flags = kLanczos;

  Scale *scale = new Scale(config);
  scale->SetInputImage(&resource_image_);
  if (CanUsePadding()) {
    // パディング可能ならバッファをはさむ
    scale->SetOutputImage(&converted_image_);
  } else {
    scale->SetOutputImage(GetOutputImage());
  }
  const ErrorCode error_scale_init = scale->Init();
  if (error_scale_init != kNoError) {
    delete scale;
    return ErrorOccured(error_scale_init);
  }
  scale_ = scale;

  // パディング
  if (CanUsePadding()) {
    Padding *padding =
        new Padding(padding_left, padding_right, padding_top, padding_bottom);
    padding->SetInputImage(&converted_image_);
    padding->SetOutputImage(GetOutputImage());
    const ErrorCode error_padding_init = padding->Init();
    if (error_padding_init != kNoError) {
      delete padding;
      return ErrorOccured(error_padding_init);
    }
    padding_ = padding;
  }
  //-------------------------------------------------------------------

  // 取り込み用BITMAPINFOを作成
  resource_ddb_info_ = Utilities::ImageToWindowsBitmapInfo(resource_ddb_);

  return InitDone();
}

// Processor::Run
ErrorCode SplashScreen::Run() {
  if (GetCurrentError() != kNoError) {
    // 何かエラーが発生している場合は何もしない
    return GetCurrentError();
  }

  // GetDIBitsを利用してビットマップデータを転送
  HDC resource_dc = CreateCompatibleDC(NULL);
  SelectObject(resource_dc, resource_ddb_.windows_ddb());
  GetDIBits(resource_dc, resource_ddb_.windows_ddb(),
            0, resource_ddb_.height(),
            resource_image_.raw_bitmap(),
            &resource_ddb_info_,
            DIB_RGB_COLORS);
  DeleteDC(resource_dc);

  // Scaleを利用して変換
  const ErrorCode error_scale = scale_->Run();
  if (error_scale != kNoError) {
    return ErrorOccured(error_scale);
  }

  // Paddingを利用してパディングを行う
  if (CanUsePadding()) {
    const ErrorCode error_padding = padding_->Run();
    if (error_padding != kNoError) {
      return ErrorOccured(error_padding);
    }
  }

  // エラー発生なし
  return GetCurrentError();
}

}   // namespace scff_imaging
