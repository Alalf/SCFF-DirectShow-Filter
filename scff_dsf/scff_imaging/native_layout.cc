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

/// @file scff_imaging/native_layout.cc
/// scff_imaging::NativeLayoutの定義

#include "scff_imaging/native_layout.h"

#include "scff_imaging/debug.h"
#include "scff_imaging/utilities.h"
#include "scff_imaging/screen_capture.h"
#include "scff_imaging/scale.h"
#include "scff_imaging/padding.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::NativeLayout
//=====================================================================

NativeLayout::NativeLayout(
    const LayoutParameter &parameter)
    : Layout(),
      parameter_(parameter),
      screen_capture_(nullptr),
      scale_(nullptr),
      padding_(nullptr) {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("NativeLayout: NEW(%dx%d)"),
          parameter_.clipping_width,
          parameter_.clipping_height));
  // 明示的に初期化していない
  // captured_image_
  // converted_image_
}

NativeLayout::~NativeLayout() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("NativeLayout: DELETE")));
  // 管理しているインスタンスをすべて破棄
  // 破棄はプロセッサ→イメージの順
  if (screen_capture_ != nullptr) {
    delete screen_capture_;
  }
  if (scale_ != nullptr) {
    delete scale_;
  }
  if (padding_ != nullptr) {
    delete padding_;
  }
}

//-------------------------------------------------------------------

ErrorCodes NativeLayout::Init() {
  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("NativeLayout: Init")));

  // あらかじめイメージのサイズを計算しておく
  const int captured_width = parameter_.clipping_width;
  const int captured_height = parameter_.clipping_height;
  int converted_width = GetOutputImage()->width();
  int converted_height = GetOutputImage()->height();
  int padding_top = 0;
  int padding_bottom = 0;
  int padding_left = 0;
  int padding_right = 0;

  if (utilities::CanUseDrawUtils(GetOutputImage()->pixel_format())) {
    // パディングサイズの計算
    const bool no_error = utilities::CalculatePaddingSize(
        GetOutputImage()->width(),
        GetOutputImage()->height(),
        captured_width,
        captured_height,
        parameter_.stretch,
        parameter_.keep_aspect_ratio,
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
  // GetDIBits用
  const ErrorCodes error_captured_image =
      captured_image_.Create(ImagePixelFormats::kRGB0,
                             captured_width,
                             captured_height);
  if (error_captured_image != ErrorCodes::kNoError) {
    return ErrorOccured(error_captured_image);
  }

  // 変換後パディング用
  if (utilities::CanUseDrawUtils(GetOutputImage()->pixel_format())) {
    const ErrorCodes error_converted_image =
        converted_image_.Create(GetOutputImage()->pixel_format(),
                                converted_width,
                                converted_height);
    if (error_converted_image != ErrorCodes::kNoError) {
      return ErrorOccured(error_converted_image);
    }
  }
  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  // スクリーンキャプチャ
  LayoutParameter parameter_array[kMaxProcessorSize];
  parameter_array[0] = parameter_;
  // NativeLayoutなのでsize=1
  ScreenCapture *screen_capture = new ScreenCapture(
      !utilities::IsTopdownPixelFormat(GetOutputImage()->pixel_format()),
      1, parameter_array);
  screen_capture->SetOutputImage(&captured_image_);
  const ErrorCodes error_screen_capture = screen_capture->Init();
  if (error_screen_capture != ErrorCodes::kNoError) {
    delete screen_capture;
    return ErrorOccured(error_screen_capture);
  }
  screen_capture_ = screen_capture;

  // 拡大縮小ピクセルフォーマット変換
  Scale *scale = new Scale(parameter_.swscale_config);
  scale->SetInputImage(&captured_image_);
  if (utilities::CanUseDrawUtils(GetOutputImage()->pixel_format())) {
    // パディング可能ならバッファをはさむ
    scale->SetOutputImage(&converted_image_);
  } else {
    scale->SetOutputImage(GetOutputImage());
  }
  const ErrorCodes error_scale_init = scale->Init();
  if (error_scale_init != ErrorCodes::kNoError) {
    delete scale;
    return ErrorOccured(error_scale_init);
  }
  scale_ = scale;

  // パディング
  if (utilities::CanUseDrawUtils(GetOutputImage()->pixel_format())) {
    Padding *padding =
        new Padding(padding_left, padding_right, padding_top, padding_bottom);
    padding->SetInputImage(&converted_image_);
    padding->SetOutputImage(GetOutputImage());
    const ErrorCodes error_padding_init = padding->Init();
    if (error_padding_init != ErrorCodes::kNoError) {
      delete padding;
      return ErrorOccured(error_padding_init);
    }
    padding_ = padding;
  }
  //-------------------------------------------------------------------

  return InitDone();
}

ErrorCodes NativeLayout::Run() {
  if (GetCurrentError() != ErrorCodes::kNoError) {
    // 何かエラーが発生している場合は何もしない
    return GetCurrentError();
  }

  // OutputImageを設定しなおす
  if (utilities::CanUseDrawUtils(GetOutputImage()->pixel_format())) {
    padding_->SwapOutputImage(GetOutputImage());
  } else {
    scale_->SwapOutputImage(GetOutputImage());
  }

  // スクリーンキャプチャ
  const ErrorCodes error_screen_capture = screen_capture_->Run();
  if (error_screen_capture != ErrorCodes::kNoError) {
    return ErrorOccured(error_screen_capture);
  }

  // Scaleを利用して変換
  const ErrorCodes error_scale = scale_->Run();
  if (error_scale != ErrorCodes::kNoError) {
    return ErrorOccured(error_scale);
  }

  // Paddingを利用してパディングを行う
  if (utilities::CanUseDrawUtils(GetOutputImage()->pixel_format())) {
    const ErrorCodes error_padding = padding_->Run();
    if (error_padding != ErrorCodes::kNoError) {
      return ErrorOccured(error_padding);
    }
  }

  // エラー発生なし
  return GetCurrentError();
}
}   // namespace scff_imaging
