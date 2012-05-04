
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

/// @brief 画像処理を行うクラスをまとめたネームスペース
namespace imaging {

class Request;
class AVPictureImage;
class AVPictureWithFillImage;
class RawBitmapImage;
class WindowsDDBImage;

/// @brief 実際に処理を行うクラス
class Processor {
 public:
  /// @brief コンストラクタ。継承クラスは必ずこれを呼ぶこと。
  Processor(ImagePixelFormat pixel_format, int width, int height);

  //-------------------------------------------------------------------
  /// @brief 仮想デストラクタ
  virtual ~Processor();
  /// @brief 初期化
  /// @retval InitDone()      初期化成功
  /// @retval ErrorOccured()  エラーが発生した場合
  virtual ErrorCode Init() = 0;
  /// @brief リクエストに対する処理を行う
  /// @param[in] request      リクエスト
  /// @retval NoError()       Accept成功
  /// @retval ErrorOccured()  エラーが発生した場合
  virtual ErrorCode Accept(Request *request) = 0;
  /// @brief 渡されたImageにイメージを設定する
  /// @param[in,out] image    Imageインスタンス
  /// @retval NoError()       Pull成功
  /// @retval ErrorOccured()  エラーが発生した場合
  virtual ErrorCode PullAVPictureImage(AVPictureImage *image);
  /// @copydoc PullAVPictureImage
  virtual ErrorCode PullAVPictureWithFillImage(
      AVPictureWithFillImage *image);
  /// @copydoc PullAVPictureImage
  virtual ErrorCode PullRawBitmapImage(RawBitmapImage *image);
  /// @copydoc PullAVPictureImage
  virtual ErrorCode PullWindowsDDBImage(WindowsDDBImage *image);
  //-------------------------------------------------------------------

  /// @brief プロセッサに異常が発生している場合NoError以外を返す
  ErrorCode GetCurrentError() const;

  /// @brief 内部ピクセルフォーマットへのアクセッサ
  ImagePixelFormat pixel_format() const;
  /// @brief 出力widthへのアクセッサ
  int width() const;
  /// @brief 出力heightへのアクセッサ
  int height() const;

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
  Processor(const Processor& processor);
  /// @brief 代入演算子(copy禁止)
  void operator=(const Processor& processor);
  //-------------------------------------------------------------------

  /// @brief エラーコード
  ErrorCode error_code_;

  /// @brief 出力width
  const int width_;
  /// @brief 出力height
  const int height_;
  /// @brief 内部ピクセルフォーマット
  const ImagePixelFormat pixel_format_;
};
}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_PROCESSOR_H_
