﻿// Copyright 2012-2013 Alalf <alalf.iQLc_at_gmail.com>
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

/// @file scff_imaging/engine.h
/// scff_imaging::Engineの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_ENGINE_H_
#define SCFF_DSF_SCFF_IMAGING_ENGINE_H_

#include "scff_imaging/common.h"
#include "scff_imaging/layout.h"

/// 画像処理を行うクラスをまとめたネームスペース
namespace scff_imaging {

/// 画像処理スレッドを管理する
class Engine : public CAMThread, public Layout {
 public:
  /// コンストラクタ
  Engine(ImagePixelFormats output_pixel_format,
         int output_width, int output_height, double output_fps);
  /// デストラクタ
  ~Engine();

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCodes Init();
  /// @copydoc Processor::Accept
  ErrorCodes Accept(Request *request);
  //-------------------------------------------------------------------

  /// カレントイメージをサンプルにコピー
  ErrorCodes CopyCurrentImage(BYTE *sample, DWORD data_size);

  //-------------------------------------------------------------------
  // ダブルディスパッチ用
  //-------------------------------------------------------------------
  /// 現在のレイアウトを解放してスプラッシュを表示する
  void ResetLayout();
  /// 現在のレイアウトを新しいNativeLayoutに設定する
  void SetNativeLayout();
  /// 現在のレイアウトを新しいComplexLayoutに設定する
  void SetComplexLayout();
  //-------------------------------------------------------------------
  /// スレッド間で共有: レイアウトパラメータの設定
  void SetLayoutParameters(
      int element_count,
      const LayoutParameter (&parameters)[kMaxProcessorSize]);
  /// レイアウトプロセッサに異常が発生している場合NoError以外を返す
  ErrorCodes GetCurrentLayoutError();
  //-------------------------------------------------------------------

 private:
  //===================================================================
  // キャプチャスレッド関連
  //===================================================================
  enum class RequestTypes : DWORD {
    kInvalid,
    kResetLayout,
    kSetNativeLayout,
    kSetComplexLayout,
    kRun,
    kStop,
    kExit,
  };

  /// 現在のレイアウトを解放する（スプラッシュを表示する）
  /// @attention レイアウトエラーコードをkUninitializedErrorにする
  void DoResetLayout();
  /// 現在のレイアウトを新しいNativeLayoutに設定する
  void DoSetNativeLayout();
  /// 現在のレイアウトを新しいComplexLayoutに設定する
  void DoSetComplexLayout();
  /// バッファにキャプチャ結果を格納する
  void DoLoop();

  /// CAMThread::ThreadProc()の実装
  DWORD ThreadProc();

  /// @copydoc Processor::Run
  ErrorCodes Run();

  /// バッファを更新
  void Update();

  /// 更新中のバッファを表すインデックス
  /// @attention あえてLockしない
  enum class ImageIndexes {
    kFront,
    kBack,
  } last_update_image_;

  /// レイアウト
  Layout *layout_;

  //-------------------------------------------------------------------
  // スレッド間で共有
  // front/back_image_はあえてロックしない
  //-------------------------------------------------------------------

  /// 唯一レイアウトエラーコードをkNoErrorにできる関数
  /// @attention Initが成功したらこちら
  ErrorCodes LayoutInitDone();
  /// レイアウトエラーが発生したときに呼び出す
  /// @return 発生したエラーコード
  /// @attention エラーがいったんおきたら解除は不可能
  ErrorCodes LayoutErrorOccured(ErrorCodes error_code);

  /// レイアウトパラメータの要素数
  int element_count_;
  /// レイアウトパラメータ
  LayoutParameter parameters_[kMaxProcessorSize];
  /// レイアウトのエラーコード
  ErrorCodes layout_error_code_;

  //===================================================================

  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  /// フロントイメージ
  AVFrameImage front_image_;
  /// バックイメージ
  AVFrameImage back_image_;
  /// スプラッシュイメージ
  AVFrameImage splash_image_;
  //-------------------------------------------------------------------

  /// フロントイメージの消去が必要
  bool need_clear_front_image_;
  /// バックイメージの消去が必要
  bool need_clear_back_image_;

  /// イメージのピクセルフォーマット
  const ImagePixelFormats output_pixel_format_;
  /// イメージの幅
  const int output_width_;
  /// イメージの高さ
  const int output_height_;
  /// fps
  const double output_fps_;

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(Engine);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_ENGINE_H_
