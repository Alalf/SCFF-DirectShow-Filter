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

/// @file scff-imaging/engine.cc
/// @brief scff_imaging::Engineの定義

#include "scff-imaging/engine.h"

#include <ctime>

#include "scff-imaging/debug.h"
#include "scff-imaging/avpicture-image.h"
#include "scff-imaging/splash-screen.h"
#include "scff-imaging/native-layout.h"
#include "scff-imaging/complex-layout.h"
#include "scff-imaging/request.h"
#include "scff-imaging/utilities.h"

namespace scff_imaging {

// 指定されたAVPictureImageを黒で塗りつぶす
static void Clear(AVPictureImage *image) {
  if (!Utilities::CanUseDrawUtils(image->pixel_format())) {
    // 塗りつぶせなければなにもしない
    return;
  }

  FFDrawContext draw_context;
  FFDrawColor padding_color;

  // パディング用のコンテキストの初期化
  const int error_init =
      ff_draw_init(&draw_context,
                   image->avpicture_pixel_format(),
                   0);
  ASSERT(error_init == 0);

  // パディング用のカラーを真っ黒に設定
  uint8_t rgba_padding_color[4] = {0};
  ff_draw_color(&draw_context,
                &padding_color,
                rgba_padding_color);

  ff_fill_rectangle(&draw_context, &padding_color,
                    image->avpicture()->data,
                    image->avpicture()->linesize,
                    0,
                    0,
                    image->width(),
                    image->height());
}

//=====================================================================
// scff_imaging::Engine
//=====================================================================

// コンストラクタ
Engine::Engine(ImagePixelFormat output_pixel_format,
               int output_width, int output_height, double output_fps)
    : CAMThread(),
      Layout(),
      output_pixel_format_(output_pixel_format),
      output_width_(output_width),
      output_height_(output_height),
      output_fps_(output_fps),
      layout_(0),       // NULL
      layout_error_code_(kProcessorUninitializedError),
      which_image_(0) {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("Engine: NEW(%d, %d, %d, %.1f)"),
          output_pixel_format, output_width, output_height, output_fps));
  // 明示的に初期化していない
  // front_image_
  // back_image_
  // splash_image_
}

// デストラクタ
Engine::~Engine() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("Engine: DELETE")));

  CallWorker(kRequestStop);
  CallWorker(kRequestResetLayout);
  CallWorker(kRequestExit);
}

//---------------------------------------------------------------------
// Processor
//---------------------------------------------------------------------

// Processor::Init
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
      front_image_.Create(output_pixel_format_,
                          output_width_,
                          output_height_);
  if (error_front_image != kNoError) {
    return ErrorOccured(error_front_image);
  }
  // バックイメージ
  const ErrorCode error_back_image =
      back_image_.Create(output_pixel_format_,
                         output_width_,
                         output_height_);
  if (error_back_image != kNoError) {
    return ErrorOccured(error_back_image);
  }
  // スプラッシュイメージ
  const ErrorCode error_splash_image =
      splash_image_.Create(output_pixel_format_,
                           output_width_,
                           output_height_);
  if (error_splash_image != kNoError) {
    return ErrorOccured(error_splash_image);
  }

  // 一時的にスプラッシュスクリーンプロセッサを作ってイメージを生成しておく
  SplashScreen splash_screen;
  splash_screen.SetOutputImage(&splash_image_);
  const ErrorCode error_splash_screen = splash_screen.Init();
  if (error_splash_screen != kNoError) {
    return ErrorOccured(error_splash_screen);
  }
  const ErrorCode error_splash_image_pull = splash_screen.Run();
  if (error_splash_image_pull != kNoError) {
    return ErrorOccured(error_splash_image_pull);
  }

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  // nop
  //-------------------------------------------------------------------

  // スレッド作成
  Create();
  CallWorker(kRequestResetLayout);

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
  // レイアウトエラーの設定はリクエストハンドラの中で行われているので
  // ここではチェックしない
  request->SendTo(this);

  /// @attention 現状、Chain of Resiposibilityはない＝
  /// @attention 下位のプロセッサへリクエストは送らない
  return GetCurrentError();
}

//-------------------------------------------------------------------

// フロントイメージをサンプルにコピー
/// @attention エラー発生中に追加の処理を行うのはEngineだけ
ErrorCode Engine::CopyFrontImage(BYTE *sample, DWORD data_size) {
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
                     splash_image_.avpicture_pixel_format(),
                     splash_image_.width(),
                     splash_image_.height(),
                     sample, data_size);
    return GetCurrentError();
  }

  // sampleにコピー
  if (which_image_ == 0) {
    ASSERT(data_size == Utilities::CalculateImageSize(front_image_));
    avpicture_layout(front_image_.avpicture(),
                     front_image_.avpicture_pixel_format(),
                     front_image_.width(),
                     front_image_.height(),
                     sample, data_size);
  } else {
    ASSERT(data_size == Utilities::CalculateImageSize(back_image_));
    avpicture_layout(back_image_.avpicture(),
                     back_image_.avpicture_pixel_format(),
                     back_image_.width(),
                     back_image_.height(),
                     sample, data_size);    
  }

  return GetCurrentError();
}


//-------------------------------------------------------------------
// リクエストハンドラ
//-------------------------------------------------------------------

// 現在のプロセッサを解放してスプラッシュを表示する
void Engine::ResetLayout() {
  CallWorker(kRequestStop);
  CallWorker(kRequestResetLayout);
  CallWorker(kRequestRun);
}

// 現在のプロセッサを新しいNativeLayoutに設定する
void Engine::SetNativeLayout() {
  CallWorker(kRequestStop);
  CallWorker(kRequestSetNativeLayout);
  CallWorker(kRequestRun);
}

// 現在のプロセッサを新しいComplexLayoutに設定する
void Engine::SetComplexLayout() {
  CallWorker(kRequestStop);
  CallWorker(kRequestSetComplexLayout);
  CallWorker(kRequestRun);
}

// スレッド間で共有: レイアウトパラメータの設定
void Engine::SetLayoutParameters(int element_count, const LayoutParameter (&parameters)[kMaxProcessorSize]) {
  CAutoLock lock(&m_WorkerLock);
  element_count_ = element_count;
  for (int i = 0; i < kMaxProcessorSize; i++) {
    parameters_[i] = parameters[i];
    if (Utilities::IsTopdownPixelFormat(output_pixel_format_)) {
      // * Topdownピクセルフォーマットの場合はbound_yの値を補正する
      // まずbound_yは左上のy座標になっているので、左下のy座標にする(y+height)
      // 左下のy座標は左上原点の座標系になっているので、左下原点の座標に直す
      parameters_[i].bound_y = output_height_ - (parameters_[i].bound_y + parameters_[i].bound_height);
    }
  }
}

//===================================================================
// キャプチャスレッド関連
//===================================================================

// 現在のレイアウトを解放する（スプラッシュを表示する）
// レイアウトエラーコードをkUninitializedErrorにする
void Engine::DoResetLayout() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
          TEXT("Engine: Reset Layout")));

  // 解放+0クリア
  if (layout_ != 0) {       // NULL
    delete layout_;
    layout_ = 0;            // NULL
  }
  // 未初期化
  CAutoLock lock(&m_WorkerLock);
  layout_error_code_ = kProcessorUninitializedError;
}

// 現在のプロセッサを新しいNativeLayoutに設定する
void Engine::DoSetNativeLayout() {
  // 現在のプロセッサは必要ないので削除
  DoResetLayout();

  //-------------------------------------------------------------------
  NativeLayout *native_layout = new NativeLayout(parameters_[0]);
  native_layout->SetOutputImage(&front_image_);
  const ErrorCode error_layout = native_layout->Init();
  if (error_layout != kNoError) {
    // 失敗
    delete native_layout;
    LayoutErrorOccured(error_layout);
  } else {
    // 成功
    layout_ = native_layout;
    LayoutInitDone();
  }
}

// 現在のプロセッサを新しいComplexLayoutに設定する
void Engine::DoSetComplexLayout() {
  // 現在のプロセッサは必要ないので削除
  DoResetLayout();

  //-------------------------------------------------------------------
  ComplexLayout *complex_layout = new ComplexLayout(element_count_, parameters_);
  complex_layout->SetOutputImage(&front_image_);
  const ErrorCode error_layout = complex_layout->Init();
  if (error_layout != kNoError) {
    // 失敗
    delete complex_layout;
    LayoutErrorOccured(error_layout);
  } else {
    // 成功
    layout_ = complex_layout;
    LayoutInitDone();
  }
}

// バッファにキャプチャ結果を格納する
void Engine::DoLoop() {
  DWORD request;

  do {
    while (!CheckRequest(&request)) {
      const clock_t start_update = ::clock();
      Update();
      const clock_t end_update = ::clock();
      const double update_interval = static_cast<double>(end_update - start_update) / CLOCKS_PER_SEC;
      const double delta = (1 / output_fps_) - update_interval;
      if (delta > 0.0) {
        ::Sleep(static_cast<DWORD>(delta * MILLISECONDS));
      }
    }

    if (request == kRequestRun) {
      Reply(NOERROR);
    }
  } while (request != kRequestStop);
}

// CAMThread::ThreadProc()の実装
DWORD Engine::ThreadProc() {
  HRESULT result = ERROR;
  DWORD request = kRequestInvalid;

  do {
    request = GetRequest();

    switch(request) {
    case kRequestResetLayout:
      DoResetLayout();
      Reply(NOERROR);
      break;
    case kRequestSetNativeLayout:
      DoSetNativeLayout();
      Reply(NOERROR);
      break;
    case kRequestSetComplexLayout:
      DoSetComplexLayout();
      Reply(NOERROR);
      break;
    case kRequestRun:
      Reply(NOERROR);
      DoLoop();
      break;
    case kRequestStop:
    case kRequestExit:
      Reply(NOERROR);
      break;
    }
  } while (request != kRequestExit);

  return 0;
}

// Processor::Run
ErrorCode Engine::Run() {
  ASSERT(layout_ != 0);   // NULL
  const ErrorCode error = layout_->Run();
  if (error != kNoError) {
    /// @attention layout_でエラーが発生してもEngine自体はエラー状態ではない
    LayoutErrorOccured(error);
  }
  return GetCurrentError();
}

// バッファを更新
void Engine::Update() {
  if (GetCurrentLayoutError() != kNoError) {
    return;
  }

  if (which_image_ == 0) {
    layout_->SetOutputImage(&front_image_);
    which_image_ = 1;
  } else {
    layout_->SetOutputImage(&back_image_);
    which_image_ = 0;
  }

  Run();
}

// 唯一レイアウトエラーコードをkNoErrorにできるメソッド
ErrorCode Engine::LayoutInitDone() {
  CAutoLock lock(&m_WorkerLock);
  ASSERT(layout_error_code_ == kProcessorUninitializedError);
  if (layout_error_code_ == kProcessorUninitializedError) {
    layout_error_code_ = kNoError;

    // ここで一回FrontImage/BackImageを黒で塗りつぶす
    Clear(&front_image_);
    Clear(&back_image_);
  }
  return layout_error_code_;
}

// レイアウトエラーが発生したときに呼び出す
ErrorCode Engine::LayoutErrorOccured(ErrorCode error_code) {
  CAutoLock lock(&m_WorkerLock);
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
ErrorCode Engine::GetCurrentLayoutError() {
  CAutoLock lock(&m_WorkerLock);
  return layout_error_code_;
}

}   // namespace scff_imaging
