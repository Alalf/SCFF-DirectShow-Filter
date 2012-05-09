
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

/// @file imaging/multi-processor.h
/// @brief imaging::MultiProcessorの宣言

#ifndef SCFF_DSF_IMAGING_MULTI_PROCESSOR_H_
#define SCFF_DSF_IMAGING_MULTI_PROCESSOR_H_

#include "imaging/imaging-types.h"

namespace imaging {

class Request;
class AVPictureImage;
class AVPictureWithFillImage;
class RawBitmapImage;
class WindowsDDBImage;

/// @brief 複数のイメージの入出力に対応したプロセッサ
class MultiProcessor {
 public:
  /// @brief コンストラクタ。継承クラスは必ずこれを呼ぶこと。
  MultiProcessor(ImagePixelFormat pixel_format,
                 int size,
                 int width[kMaxMultiProcessorSize],
                 int height[kMaxMultiProcessorSize]);
  /// @brief 仮想デストラクタ
  virtual ~MultiProcessor();

  //-------------------------------------------------------------------
  /// @brief 初期化
  /// @retval InitDone()      初期化成功
  /// @retval ErrorOccured()  エラーが発生した場合
  virtual ErrorCode Init() = 0;
  /// @brief リクエストに対する処理を行う
  /// @param[in] request      リクエスト
  /// @retval NoError()       Accept成功
  /// @retval ErrorOccured()  エラーが発生した場合
  virtual ErrorCode Accept(Request *request) = 0;
  /// @brief 渡されたImage配列にイメージを設定する
  /// @param[in,out] images   Imageインスタンスの配列
  /// @retval NoError()       Pull成功
  /// @retval ErrorOccured()  エラーが発生した場合
  virtual ErrorCode PullAVPictureImages(
      AVPictureImage *images[kMaxMultiProcessorSize]);
  /// @copydoc PullAVPictureImages
  virtual ErrorCode PullAVPictureWithFillImages(
      AVPictureWithFillImage *images[kMaxMultiProcessorSize]);
  /// @copydoc PullAVPictureImages
  virtual ErrorCode PullRawBitmapImages(
      RawBitmapImage *images[kMaxMultiProcessorSize]);
  /// @copydoc PullAVPictureImages
  virtual ErrorCode PullWindowsDDBImages(
      WindowsDDBImage *images[kMaxMultiProcessorSize]);
  //-------------------------------------------------------------------

  /// @brief プロセッサに異常が発生している場合NoError以外を返す
  ErrorCode GetCurrentError() const;

  /// @brief 内部ピクセルフォーマットへのアクセッサ
  ImagePixelFormat pixel_format() const;
  /// @brief 出力widthへのアクセッサ
  int width(int index) const;
  /// @brief 出力heightへのアクセッサ
  int height(int index) const;

 protected:
  //-------------------------------------------------------------------
  /// @brief 唯一エラーコードをkNoErrorにできる関数
  /// @attention Initが成功したらこちら
  ErrorCode InitDone();
  /// @brief kNoErrorを返すだけのメソッド
  ErrorCode NoError() const;
  /// @brief エラーが発生したときに呼び出す。
  /// @return 発生したエラーコード。
  /// @attention エラーがいったんおきたら解除は不可能
  ErrorCode ErrorOccured(ErrorCode error_code);
  //-------------------------------------------------------------------

 private:
  //-------------------------------------------------------------------
  // (copy禁止)
  //-------------------------------------------------------------------
  /// @brief コピーコンストラクタ
  MultiProcessor(const MultiProcessor& multi_processor);
  /// @brief 代入演算子(copy禁止)
  void operator=(const MultiProcessor& multi_processor);
  //-------------------------------------------------------------------

  /// @brief 内部ピクセルフォーマット
  const ImagePixelFormat pixel_format_;
  /// @brief 出力イメージの個数
  const int size_;

  /// @brief エラーコード
  ErrorCode error_code_;

  /// @brief 出力widthの配列
  int width_[kMaxMultiProcessorSize];
  /// @brief 出力heightの配列
  int height_[kMaxMultiProcessorSize];
};

}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_MULTI_PROCESSOR_H_
