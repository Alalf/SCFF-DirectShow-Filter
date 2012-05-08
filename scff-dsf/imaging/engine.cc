
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

/// @file imaging/engine.cc
/// @brief imaging::Engineの定義

#include "imaging/engine.h"

#include <Windows.h>

extern "C" {
#include <libavcodec/avcodec.h>
}

#include "base/debug.h"
#include "imaging/avpicture-image.h"
#include "imaging/native-layout.h"
#include "imaging/splash-screen.h"
#include "imaging/request.h"
#include "imaging/utilities.h"

namespace imaging {

//=====================================================================
// imaging::Engine
//=====================================================================

// コンストラクタ
Engine::Engine(ImagePixelFormat pixel_format, int width, int height, double fps)
    : Processor(pixel_format, width, height),
      fps_(fps),
      layout_(0),       // NULL
      layout_error_code_(kUninitialziedError),
      front_image_(),
      back_image_() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("Engine: NEW(%d, %d, %d, %.1f)"),
          pixel_format, width, height, fps));
}

// デストラクタ
Engine::~Engine() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("Engine: DELETE")));
  // 管理しているインスタンスをすべて破棄
  if (layout_ != 0) {       // NULL
    delete layout_;
  }
}

//-------------------------------------------------------------------
// レイアウト操作
//-------------------------------------------------------------------

// レイアウトとイメージを削除
void Engine::ReleaseLayout() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("Engine: Release Layout")));

  // 解放+0クリア
  if (layout_ != 0) {       // NULL
    delete layout_;
    layout_ = 0;            // NULL
  }
  // 未初期化
  layout_error_code_ = kUninitialziedError;
}

// 唯一レイアウトエラーコードをkNoErrorにできるメソッド
ErrorCode Engine::LayoutInitDone() {
  ASSERT(layout_error_code_ == kUninitialziedError);
  if (layout_error_code_ == kUninitialziedError) {
    layout_error_code_ = kNoError;
  }
  return layout_error_code_;
}

// レイアウトエラーが発生したときに呼び出す
ErrorCode Engine::LayoutErrorOccured(ErrorCode error_code) {
  if (error_code != kNoError) {
    // 後からkNoErrorにしようとしてもできない
    // ASSERT(false);
    MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("Engine: Layout Error Occured(%d)"),
            error_code));
    layout_error_code_ = error_code;
  }
  return layout_error_code_;
}

// レイアウトプロセッサに異常が発生している場合NoError以外を返す
ErrorCode Engine::GetCurrentLayoutError() const {
  return layout_error_code_;
}

//---------------------------------------------------------------------
// Processor
//---------------------------------------------------------------------

// 初期化
ErrorCode Engine::Init() {
  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("Engine: Init")));

  //-------------------------------------------------------------------
  // 初期化の順番はイメージ→プロセッサの順
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  // フロントイメージ
  const ErrorCode error_front_image =
      front_image_.Create(pixel_format(), width(), height());
  if (error_front_image != kNoError) {
    return ErrorOccured(error_front_image);
  }
  // バックイメージ
  // const ErrorCode error_back_image =
  //     back_image_.Create(pixel_format(), width(), height());
  // if (error_back_image != kNoError) {
  //   return ErrorOccured(error_back_image);
  // }
  // スプラッシュイメージ
  const ErrorCode error_splash_image =
      splash_image_.Create(pixel_format(), width(), height());
  if (error_splash_image != kNoError) {
    return ErrorOccured(error_splash_image);
  }

  // 一時的にスプラッシュスクリーンプロセッサを作ってイメージを生成しておく
  SplashScreen splash_screen(pixel_format(), width(), height());
  const ErrorCode error_splash_screen = splash_screen.Init();
  if (error_splash_screen != kNoError) {
    return ErrorOccured(error_splash_screen);
  }
  const ErrorCode error_splash_image_pull =
      splash_screen.PullAVPictureImage(&splash_image_);
  if (error_splash_image_pull != kNoError) {
    return ErrorOccured(error_splash_image_pull);
  }

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  ReleaseLayout();

  //-------------------------------------------------------------------

  // 作成成功
  return InitDone();
}

// リクエストに対する処理を行う
ErrorCode Engine::Accept(Request *request) {
  // 何かエラーが発生している場合は何もしない
  if (GetCurrentError() != kNoError) {
    return GetCurrentError();
  }

  // NULLリクエスト(何もしない)ならば、何もしない
  if (request == 0) {   // NULL
    return GetCurrentError();
  }

  MyDbgLog((LOG_TRACE, kDbgImportant,
          TEXT("Engine: Accept")));

  // リクエストが送られてきているのならば、
  // thisを渡して処理を任せる(ダブルディスパッチ)
  request->SendTo(this);
  // レイアウトエラーの設定はリクエストハンドラの中で行われているので
  // ここではチェックしない

  /// @attention 現状、Chain of Resiposibilityはない＝
  /// @attention 下位のプロセッサへリクエストは送らない
  return NoError();
}

// 渡されたポインタにビットマップデータを設定する
ErrorCode Engine::PullAVPictureImage(AVPictureImage *image) {
  if (GetCurrentError() != kNoError) {
    // 何かエラーが発生している場合は何もしない
    return GetCurrentError();
  }

  // イメージにlayoutの出力をつめる
  ASSERT(layout_ != 0);   // NULL
  ASSERT(image != 0);     // NULL
  const ErrorCode error = layout_->PullAVPictureImage(image);
  if (error != kNoError) {
    LayoutErrorOccured(error);
    /// @attention layout_でエラーが発生してもEngine自体はエラー状態ではない
  }

  return NoError();
}

//-------------------------------------------------------------------

// フロントイメージをサンプルにコピー
/// @attention エラー発生中に追加の処理を行うのはEngineだけ
ErrorCode Engine::PullFrontImage(BYTE *sample, DWORD data_size) {
  /// @attention processorのポインタがNULLであることはエラーではない

  // Engine自体にエラーが発生していたら0クリア
  if (GetCurrentError() != kNoError) {
    // Splashすら表示できない状態である可能性がある
    ZeroMemory(sample, data_size);
    return GetCurrentError();
  }

  // layout_にエラーが発生していたらスプラッシュを書く
  if (GetCurrentLayoutError() != kNoError) {
    ASSERT(data_size == Utilities::CalculateImageSize(splash_image_));
    avpicture_layout(splash_image_.avpicture(),
                      Utilities::ToAVPicturePixelFormat(pixel_format()),
                      width(), height(), sample, data_size);
    return GetCurrentError();
  }

  // フロントイメージにデータを詰める
  /// @todo(me) マルチスレッド化した場合はダブルバッファ処理にする
  PullAVPictureImage(&front_image_);

  // フロントイメージからsampleにそのままコピー
  ASSERT(data_size == Utilities::CalculateImageSize(front_image_));
  avpicture_layout(front_image_.avpicture(),
                    Utilities::ToAVPicturePixelFormat(pixel_format()),
                    width(), height(), sample, data_size);

  return GetCurrentError();
}

//-------------------------------------------------------------------
// リクエストハンドラ
//-------------------------------------------------------------------

/// @brief 現在のプロセッサを解放してスプラッシュを表示する
void Engine::DoResetLayout() {
  /// @attention ReleaseLayoutはレイアウトエラーコードを変更する
  ReleaseLayout();
}

/// @brief 現在のプロセッサを新しいNativeLayoutに設定する
void Engine::DoSetNativeLayout(
    const ScreenCaptureParameter &parameter,
    bool stretch, bool keep_aspect_ratio) {
  // 現在のプロセッサは必要ないので削除
  ReleaseLayout();

  //-------------------------------------------------------------------
  NativeLayout *native_layout =
      new NativeLayout(pixel_format(), width(), height(),
                       parameter, stretch, keep_aspect_ratio);
  const ErrorCode error_layout = native_layout->Init();
  if (error_layout != kNoError) {
    delete native_layout;
    LayoutErrorOccured(error_layout);
    // エラーコードを設定して終了
    return;
  }

  // 成功
  layout_ = native_layout;
  LayoutInitDone();
}
}   // namespace imaging
