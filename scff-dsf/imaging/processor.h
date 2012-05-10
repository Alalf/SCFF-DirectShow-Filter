
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

/// @file imaging/processor.h
/// @brief imaging::Processorの宣言

#ifndef SCFF_DSF_IMAGING_PROCESSOR_H_
#define SCFF_DSF_IMAGING_PROCESSOR_H_

#include "imaging/imaging-types.h"
#include "base/debug.h"

/// @brief 画像処理を行うクラスをまとめたネームスペース
namespace imaging {

class Request;
class AVPictureImage;
class AVPictureWithFillImage;
class RawBitmapImage;
class WindowsDDBImage;

/// @brief 実際に処理を行う仮想クラス
template <class InputImageType, class OutputImageType>
class Processor {
 public:
  /// @brief コンストラクタ
  explicit Processor(int size = 1)
      : error_code_(kUninitialziedError),
        size_(size) {
    // nop
  }
  /// @brief 仮想デストラクタ
  virtual ~Processor() {
    // nop
  }

  //-------------------------------------------------------------------
  /// @brief 初期化
  /// @pre SetXXXImageで入出力の形式がすでに定まっていること
  /// @warning Init後にSetXXXImageを実行した場合、動作は不定だが
  /// @warning ピクセルフォーマットもサイズも変わらない場合は問題ない。
  /// @retval InitDone()      初期化成功
  /// @retval ErrorOccured()  エラーが発生した場合
  virtual ErrorCode Init() = 0;
  /// @brief 実際の処理を行う
  /// @retval NoError()       Accept成功
  /// @retval ErrorOccured()  エラーが発生した場合
  virtual ErrorCode Run() = 0;

  /// @brief リクエストに対する処理を行う
  /// @param[in] request      リクエスト
  /// @retval NoError()       Accept成功
  /// @retval ErrorOccured()  エラーが発生した場合
  virtual ErrorCode Accept(Request *request) {
    ASSERT(false);
    return NoError();
  }
  //-------------------------------------------------------------------

  /// @brief プロセッサに異常が発生している場合NoError以外を返す
  ErrorCode GetCurrentError() const  {
    return error_code_;
  }
  /// @brief Getter: 入出力のサイズ
  int size() const {
    return size_;
  }

  /// @brief Setter: input_image_
  void SetInputImage(InputImageType *input_image, int index = 0) {
    ASSERT(0 <= index && index < size());
    input_image_[index] = input_image;
  }
  /// @brief Getter: input_image_
  InputImageType* GetInputImage(int index = 0) const {
    ASSERT(0 <= index && index < size());
    return input_image_[index];
  }
  /// @brief Setter: output_image_
  void SetOutputImage(OutputImageType* output_image, int index = 0) {
    ASSERT(0 <= index && index < size());
    output_image_[index] = output_image;
  }
  /// @brief Getter: output_image_
  OutputImageType* GetOutputImage(int index = 0) const {
    ASSERT(0 <= index && index < size());
    return output_image_[index];
  }

 protected:
  //-------------------------------------------------------------------
  /// @brief 唯一エラーコードをkNoErrorにできる関数
  /// @attention Initが成功したらこちら
  ErrorCode InitDone() {
    ASSERT(error_code_ == kUninitialziedError);
    if (error_code_ == kUninitialziedError) {
      error_code_ = kNoError;
    }
    return error_code_;
  }
  /// @brief kNoErrorを返すだけのメソッド
  ErrorCode NoError() const {
    return kNoError;
  }
  /// @brief エラーが発生したときに呼び出す。
  /// @return 発生したエラーコード。
  /// @attention エラーがいったんおきたら解除は不可能
  ErrorCode ErrorOccured(ErrorCode error_code) {
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

 private:
  /// @brief エラーコード
  ErrorCode error_code_;
  /// @brief 入出力のサイズ
  const int size_;

  /// @brief 設定済みの入力
  InputImageType *input_image_[kMaxProcessorSize];
  /// @brief 設定済みの出力
  OutputImageType *output_image_[kMaxProcessorSize];
};
}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_PROCESSOR_H_
