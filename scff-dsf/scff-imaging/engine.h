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

/// @file scff-imaging/engine.h
/// @brief scff_imaging::Engineの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_ENGINE_H_
#define SCFF_DSF_SCFF_IMAGING_ENGINE_H_

#include "scff-imaging/common.h"
#include "scff-imaging/layout.h"

/// @brief 画像処理を行うクラスをまとめたネームスペース
namespace scff_imaging {

/// @brief 画像処理スレッドを管理する
/// @todo(me) まずはシングルバッファで実装してみる
class Engine : public CAMThread, public Layout {
 public:
  /// @brief コンストラクタ
  Engine(ImagePixelFormat output_pixel_format,
         int output_width, int output_height, double output_fps);
  /// @brief デストラクタ
  ~Engine();

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Accept
  ErrorCode Accept(Request *request);
  //-------------------------------------------------------------------

  /// @brief フロントイメージをサンプルにコピー
  ErrorCode CopyFrontImage(BYTE *sample, DWORD data_size);

  //-------------------------------------------------------------------
  // ダブルディスパッチ用
  //-------------------------------------------------------------------
  /// @brief 現在のレイアウトを解放してスプラッシュを表示する
  void ResetLayout();
  /// @brief 現在のレイアウトを新しいNativeLayoutに設定する
  void SetNativeLayout();
  /// @brief 現在のレイアウトを新しいComplexLayoutに設定する
  void SetComplexLayout();
  //-------------------------------------------------------------------
  /// @brief スレッド間で共有: レイアウトパラメータの設定
  void SetLayoutParameters(
      int element_count,
      const LayoutParameter (&parameters)[kMaxProcessorSize]);
  //-------------------------------------------------------------------

 private:
  //===================================================================
  // キャプチャスレッド関連
  //===================================================================
  enum class RequestType : DWORD {
    kInvalid,
    kResetLayout,
    kSetNativeLayout,
    kSetComplexLayout,
    kRun,
    kStop,
    kExit,
  };

  /// @brief 現在のレイアウトを解放する（スプラッシュを表示する）
  /// @attention レイアウトエラーコードをkUninitializedErrorにする
  void DoResetLayout();
  /// @brief 現在のレイアウトを新しいNativeLayoutに設定する
  void DoSetNativeLayout();
  /// @brief 現在のレイアウトを新しいComplexLayoutに設定する
  void DoSetComplexLayout();
  /// @brief バッファにキャプチャ結果を格納する
  void DoLoop();

  /// @brief CAMThread::ThreadProc()の実装
  DWORD ThreadProc();

  /// @copydoc Processor::Run
  ErrorCode Run();

  /// @brief バッファを更新
  void Update();

  /// @brief 更新中のバッファを表すインデックス
  /// @attention あえてLockしない
  enum class ImageIndex {
    kFront,
    kBack,
  } last_update_image_;

  /// @brief レイアウト
  Layout *layout_;

  //-------------------------------------------------------------------
  // スレッド間で共有
  // front/back_image_はあえてロックしない
  //-------------------------------------------------------------------

  /// @brief 唯一レイアウトエラーコードをkNoErrorにできる関数
  /// @attention Initが成功したらこちら
  ErrorCode LayoutInitDone();
  /// @brief レイアウトエラーが発生したときに呼び出す
  /// @return 発生したエラーコード
  /// @attention エラーがいったんおきたら解除は不可能
  ErrorCode LayoutErrorOccured(ErrorCode error_code);
  /// @brief レイアウトプロセッサに異常が発生している場合NoError以外を返す
  ErrorCode GetCurrentLayoutError();

  /// @brief レイアウトパラメータの要素数
  int element_count_;
  /// @biref レイアウトパラメータ
  LayoutParameter parameters_[kMaxProcessorSize];
  /// @brief レイアウトのエラーコード
  ErrorCode layout_error_code_;

  //===================================================================

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

  /// @brief イメージのピクセルフォーマット
  const ImagePixelFormat output_pixel_format_;
  /// @brief イメージの幅
  const int output_width_;
  /// @brief イメージの高さ
  const int output_height_;
  /// @brief fps
  /// @todo(me) マルチスレッド化する場合、FPSに合わせて処理を行う
  const double output_fps_;

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(Engine);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_ENGINE_H_
