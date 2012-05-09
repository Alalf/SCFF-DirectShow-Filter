
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

/// @file imaging/engine.h
/// @brief imaging::Engineの宣言

#ifndef SCFF_DSF_IMAGING_ENGINE_H_
#define SCFF_DSF_IMAGING_ENGINE_H_

#include "imaging/processor.h"
#include "imaging/avpicture-image.h"

/// @brief 画像処理を行うクラスをまとめたネームスペース
namespace imaging {

/// @brief 画像処理スレッドを管理する
/// @todo(me) まずはシングルバッファで実装してみる
class Engine : public Processor {
 public:
  /// @brief コンストラクタ
  Engine(ImagePixelFormat pixel_format, int width, int height, double fps);
  /// @brief デストラクタ
  ~Engine();

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Accept
  ErrorCode Accept(Request *request);
  //-------------------------------------------------------------------

  /// @brief フロントイメージをサンプルにコピー
  ErrorCode PullFrontImage(BYTE *sample, DWORD data_size);

  //-------------------------------------------------------------------
  // リクエストハンドラ
  //-------------------------------------------------------------------
  /// @brief 現在のプロセッサを解放してスプラッシュを表示する
  void DoResetLayout();
  /// @brief 現在のプロセッサを新しいNativeLayoutに設定する
  void DoSetNativeLayout(const ScreenCaptureParameter &parameter,
                         bool stretch, bool keep_aspect_ratio);

 private:
  //-------------------------------------------------------------------
  // (copy禁止)
  //-------------------------------------------------------------------
  /// @brief コピーコンストラクタ
  Engine(const Engine& engine);
  /// @brief 代入演算子(copy禁止)
  void operator=(const Engine& engine);
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  /// @copydoc Processor::PullAVPictureImage
  ErrorCode PullAVPictureImage(AVPictureImage *image);
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // レイアウト操作
  //-------------------------------------------------------------------
  /// @brief レイアウトを削除する
  /// @attention レイアウトエラーコードをkUninitializedErrorにする
  void ReleaseLayout();
  /// @brief 唯一レイアウトエラーコードをkNoErrorにできる関数
  /// @attention Initが成功したらこちら
  ErrorCode LayoutInitDone();
  /// @brief レイアウトエラーが発生したときに呼び出す
  /// @return 発生したエラーコード
  /// @attention エラーがいったんおきたら解除は不可能
  ErrorCode LayoutErrorOccured(ErrorCode error_code);
  /// @brief レイアウトプロセッサに異常が発生している場合NoError以外を返す
  ErrorCode GetCurrentLayoutError() const;
  /// @brief レイアウトのエラーコード
  ErrorCode layout_error_code_;
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  /// @brief レイアウト
  Processor *layout_;
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  /// @brief フロントイメージ
  AVPictureImage front_image_;
  /// @brief バックイメージ
  AVPictureImage back_image_;
  /// @brief スプラッシュイメージ
  AVPictureImage splash_image_;
  //-------------------------------------------------------------------

  /// @brief fps
  /// @todo(me) マルチスレッド化する場合、FPSに合わせて処理を行う
  const double fps_;
};
}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_ENGINE_H_
