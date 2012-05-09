
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

/// @file imaging/multi-processor.cc
/// @brief imaging::MultiProcessorの定義

#include "imaging/multi-processor.h"

#include "base/debug.h"

namespace imaging {

//=====================================================================
// imaging::MultiProcessor
//=====================================================================

// コンストラクタ。継承クラスは必ずこれを呼ぶこと。
MultiProcessor::MultiProcessor(
    ImagePixelFormat pixel_format,
    int size,
    int width[kMaxMultiProcessorSize],
    int height[kMaxMultiProcessorSize])
    : pixel_format_(pixel_format),
      size_(size),
      error_code_(kUninitialziedError) {
  // 配列の初期化
  for (int i = 0; i < kMaxMultiProcessorSize; i++) {
    width_[i] = width[i];
    height_[i] = height[i];
  }
}

// コンストラクタ。出力一つの場合こちらを利用すること。
MultiProcessor::MultiProcessor(
    ImagePixelFormat pixel_format,
    int width, int height)
    : pixel_format_(pixel_format),
      size_(1) {
  // 配列の初期化
  width_[0] = width;
  height_[0] = height;
  for (int i = 1; i < kMaxMultiProcessorSize; i++) {
    width_[i] = -1;   // ありえない値
    height_[i] = -1;  // ありえない値
  }
}

// 仮想デストラクタ
MultiProcessor::~MultiProcessor() {
  // nop
}

// プロセッサに異常が発生している場合NoError以外を返す
ErrorCode MultiProcessor::GetCurrentError() const {
  return error_code_;
}

/// @brief 内部ピクセルフォーマットへのアクセッサ
ImagePixelFormat MultiProcessor::pixel_format() const {
  return pixel_format_;
}

/// @brief 出力widthへのアクセッサ
int MultiProcessor::width(int index) const {
  ASSERT(0 <= index && index < size_);
  return width_[index];
}

/// @brief 出力heightへのアクセッサ
int MultiProcessor::height(int index) const {
  ASSERT(0 <= index && index < size_);
  return height_[index];
}

//---------------------------------------------------------------------

// 唯一エラーコードをkNoErrorにできるメソッド
ErrorCode MultiProcessor::InitDone() {
  ASSERT(error_code_ == kUninitialziedError);
  if (error_code_ == kUninitialziedError) {
    error_code_ = kNoError;
  }
  return error_code_;
}

// kNoErrorを返すだけのメソッド
ErrorCode MultiProcessor::NoError() const {
  return kNoError;
}

// エラーが発生したときに呼び出す。返り値は発生したエラーコード。
ErrorCode MultiProcessor::ErrorOccured(ErrorCode error_code) {
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

//---------------------------------------------------------------------
// MultiProcessor::PullAVPictureImages
ErrorCode MultiProcessor::PullAVPictureImages(
    AVPictureImage *images[kMaxMultiProcessorSize]) {
  ASSERT(false);
  return ErrorOccured(kImageTypeError);
}
// MultiProcessor::PullAVPictureWithFillImages
ErrorCode MultiProcessor::PullAVPictureWithFillImages(
    AVPictureWithFillImage *images[kMaxMultiProcessorSize]) {
  ASSERT(false);
  return ErrorOccured(kImageTypeError);
}
// MultiProcessor::PullRawBitmapImages
ErrorCode MultiProcessor::PullRawBitmapImages(
    RawBitmapImage *images[kMaxMultiProcessorSize]) {
  ASSERT(false);
  return ErrorOccured(kImageTypeError);
}
// MultiProcessor::PullWindowsDDBImages
ErrorCode MultiProcessor::PullWindowsDDBImages(
    WindowsDDBImage *images[kMaxMultiProcessorSize]) {
  ASSERT(false);
  return ErrorOccured(kImageTypeError);
}
//---------------------------------------------------------------------

}   // namespace imaging
