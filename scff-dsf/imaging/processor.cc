
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

/// @file imaging/processor.cc
/// @brief imaging::Processorの定義

#include "imaging/processor.h"

#include "base/debug.h"

namespace imaging {

//=====================================================================
// imaging::Processor
//=====================================================================

/// コンストラクタ。継承クラスは必ずこれを呼ぶこと。
Processor::Processor(ImagePixelFormat pixel_format, int width, int height)
    : pixel_format_(pixel_format),
      width_(width),
      height_(height),
      error_code_(kUninitialziedError) {
}

// 仮想デストラクタ
Processor::~Processor() {
  // nop
}

//-------------------------------------------------------------------

/// @brief プロセッサに異常が発生している場合NoError以外を返す
ErrorCode Processor::GetCurrentError() const {
  return error_code_;
}

// 内部ピクセルフォーマットへのアクセッサ
ImagePixelFormat Processor::pixel_format() const {
  return pixel_format_;
}

// 出力widthへのアクセッサ
int Processor::width() const {
  return width_;
}

// 出力heightへのアクセッサ
int Processor::height() const {
  return height_;
}

// 唯一エラーコードをkNoErrorにできるメソッド
ErrorCode Processor::InitDone() {
  ASSERT(error_code_ == kUninitialziedError);
  if (error_code_ == kUninitialziedError) {
    error_code_ = kNoError;
  }
  return error_code_;
}

// kNoErrorを返すだけのメソッド
ErrorCode Processor::NoError() const {
  return kNoError;
}

// エラーが発生したときに呼び出す。返り値は発生したエラーコード。
ErrorCode Processor::ErrorOccured(ErrorCode error_code) {
  if (error_code != kNoError) {
    // 後からkNoErrorにしようとしてもできない
    // ASSERT(false);
    MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("Processor: Error Occured(%d)"),
            error_code));
    error_code_ = error_code;
  }
  return error_code_;
}

//-------------------------------------------------------------------
// Processor::PullAVPictureImage
ErrorCode Processor::PullAVPictureImage(AVPictureImage *image) {
  ASSERT(false);
  return ErrorOccured(kImageTypeError);
}
// Processor::PullAVPictureWithFillImage
ErrorCode Processor::PullAVPictureWithFillImage(
    AVPictureWithFillImage *image) {
  ASSERT(false);
  return ErrorOccured(kImageTypeError);
}
// Processor::PullRawBitmapImage
ErrorCode Processor::PullRawBitmapImage(RawBitmapImage *image) {
  ASSERT(false);
  return ErrorOccured(kImageTypeError);
}
// Processor::PullWindowsDDBImage
ErrorCode Processor::PullWindowsDDBImage(WindowsDDBImage *image) {
  ASSERT(false);
  return ErrorOccured(kImageTypeError);
}
//-------------------------------------------------------------------

}   // namespace imaging
