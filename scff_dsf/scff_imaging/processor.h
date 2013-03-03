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

/// @file scff_imaging/processor.h
/// scff_imaging::Processorの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_PROCESSOR_H_
#define SCFF_DSF_SCFF_IMAGING_PROCESSOR_H_

#include "scff_imaging/imaging_types.h"
#include "scff_imaging/debug.h"

/// 画像処理を行うクラスをまとめたネームスペース
namespace scff_imaging {

class Request;
class AVPictureImage;
class AVPictureWithFillImage;
class RawBitmapImage;
class WindowsDDBImage;

/// 実際に処理を行う仮想クラス
template <class InputImageType, class OutputImageType>
class Processor {
 public:
  /// コンストラクタ
  explicit Processor(int size = 1)
      : error_code_(ErrorCodes::kProcessorUninitializedError),
        size_(size) {
    // nop
  }
  /// 仮想デストラクタ
  virtual ~Processor() {
    // nop
  }

  //-------------------------------------------------------------------
  /// 初期化
  /// @pre SetXXXImageで入出力の形式がすでに定まっていること
  /// @warning Init後にSetXXXImageを実行した場合、動作は不定だが
  ///          ピクセルフォーマットもサイズも変わらない場合は問題ない。
  /// @retval InitDone()      初期化成功
  /// @retval ErrorOccured()  エラーが発生した場合
  virtual ErrorCodes Init() = 0;
  /// 実際の処理を行う
  /// @retval GetCurrentError()   Accept成功
  /// @retval ErrorOccured()      エラーが発生した場合
  virtual ErrorCodes Run() = 0;

  /// リクエストに対する処理を行う
  /// @param[in] request          リクエスト
  /// @retval GetCurrentError()   Accept成功
  /// @retval ErrorOccured()      エラーが発生した場合
  virtual ErrorCodes Accept(Request *request) {
    ASSERT(false);
    return GetCurrentError();
  }
  //-------------------------------------------------------------------

  /// プロセッサに異常が発生している場合NoError以外を返す
  ErrorCodes GetCurrentError() const  {
    return error_code_;
  }
  /// Getter: 入出力のサイズ
  int size() const {
    return size_;
  }

  /// Setter: input_image_
  void SetInputImage(InputImageType *input_image, int index = 0) {
    ASSERT(0 <= index && index < size());
    ASSERT(GetCurrentError() == ErrorCodes::kProcessorUninitializedError);
    input_image_[index] = input_image;
  }
  /// Swap: input_image_
  InputImageType* SwapInputImage(InputImageType *input_image, int index = 0) {
    ASSERT(0 <= index && index < size());
    ASSERT(GetCurrentError() == ErrorCodes::kNoError);
    ASSERT(input_image_[index] != nullptr);
    InputImageType *original_image = input_image_[index];
    input_image_[index] = input_image;
    return original_image;
  }
  /// Getter: input_image_
  InputImageType* GetInputImage(int index = 0) const {
    ASSERT(0 <= index && index < size());
    return input_image_[index];
  }
  /// Setter: output_image_
  void SetOutputImage(OutputImageType *output_image, int index = 0) {
    ASSERT(0 <= index && index < size());
    ASSERT(GetCurrentError() == ErrorCodes::kProcessorUninitializedError);
    output_image_[index] = output_image;
  }
  /// Swap: output_image_
  OutputImageType* SwapOutputImage(OutputImageType *output_image,
                                   int index = 0) {
    ASSERT(0 <= index && index < size());
    ASSERT(GetCurrentError() == ErrorCodes::kNoError);
    ASSERT(output_image_[index] != nullptr);
    OutputImageType *original_image = output_image_[index];
    output_image_[index] = output_image;
    return original_image;
  }
  /// Getter: output_image_
  OutputImageType* GetOutputImage(int index = 0) const {
    ASSERT(0 <= index && index < size());
    return output_image_[index];
  }

 protected:
  //-------------------------------------------------------------------
  /// 唯一エラーコードをkNoErrorにできる関数
  /// @attention Initが成功したらこちら
  ErrorCodes InitDone() {
    ASSERT(error_code_ == ErrorCodes::kProcessorUninitializedError);
    if (error_code_ == ErrorCodes::kProcessorUninitializedError) {
      error_code_ = ErrorCodes::kNoError;
    }
    return error_code_;
  }
  /// エラーが発生したときに呼び出す。
  /// @return 発生したエラーコード。
  /// @attention エラーがいったんおきたら解除は不可能
  ErrorCodes ErrorOccured(ErrorCodes error_code) {
    if (error_code != ErrorCodes::kNoError) {
      // 後からkNoErrorにしようとしてもできない
      // ASSERT(false);
      DbgLog((kLogError, kError,
              TEXT("Processor: Error Occured(%d)"),
              error_code));
      error_code_ = error_code;
    }
    return error_code_;
  }
  //-------------------------------------------------------------------

 private:
  /// エラーコード
  ErrorCodes error_code_;
  /// 入出力のサイズ
  const int size_;

  /// 設定済みの入力
  InputImageType *input_image_[kMaxProcessorSize];
  /// 設定済みの出力
  OutputImageType *output_image_[kMaxProcessorSize];
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_PROCESSOR_H_
