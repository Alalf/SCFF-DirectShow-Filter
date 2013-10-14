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

/// @file scff_imaging/engine.cc
/// scff_imaging::Engineの定義

#include "scff_imaging/engine.h"

#include "scff_imaging/debug.h"
#include "scff_imaging/avpicture_image.h"
#include "scff_imaging/splash_screen.h"
#include "scff_imaging/native_layout.h"
#include "scff_imaging/complex_layout.h"
#include "scff_imaging/request.h"
#include "scff_imaging/utilities.h"

extern OSVERSIONINFO g_osInfo;

namespace {

/// 指定されたAVPictureImageを黒で塗りつぶす
void Clear(scff_imaging::AVPictureImage *image) {
  if (!scff_imaging::utilities::CanUseDrawUtils(image->pixel_format())) {
    // 塗りつぶせなければなにもしない
    return;
  }

  FFDrawContext draw_context;
  FFDrawColor padding_color;

  // パディング用のコンテキストの初期化
  const int error_init =
      ff_draw_init(&draw_context,
                   image->av_pixel_format(),
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
}   // namespace

namespace scff_imaging {

//=====================================================================
// scff_imaging::Engine
//=====================================================================

Engine::Engine(ImagePixelFormats output_pixel_format,
               int output_width, int output_height, double output_fps)
    : CAMThread(),
      Layout(),
      need_clear_front_image_(false),
      need_clear_back_image_(false),
      output_pixel_format_(output_pixel_format),
      output_width_(output_width),
      output_height_(output_height),
      output_fps_(output_fps),
      layout_(nullptr),
      layout_error_code_(ErrorCodes::kProcessorUninitializedError),
      last_update_image_(ImageIndexes::kFront) {
  DbgLog((kLogMemory, kTrace,
          TEXT("Engine: NEW(%d, %d, %d, %.1f)"),
          output_pixel_format, output_width, output_height, output_fps));
  // 明示的に初期化していない
  // front_image_
  // back_image_
  // splash_image_
}

Engine::~Engine() {
  DbgLog((kLogMemory, kTrace,
          TEXT("Engine: DELETE")));

  /// @attention enum->DWORD
  CallWorker(static_cast<DWORD>(RequestTypes::kStop));
  CallWorker(static_cast<DWORD>(RequestTypes::kResetLayout));
  CallWorker(static_cast<DWORD>(RequestTypes::kExit));
}

//---------------------------------------------------------------------
// Processor
//---------------------------------------------------------------------

ErrorCodes Engine::Init() {
  DbgLog((kLogTrace, kTraceInfo,
          TEXT("Engine: Init(%d, %d, %d, %.1f)"),
          output_pixel_format_, output_width_, output_height_, output_fps_));

  //-------------------------------------------------------------------
  // 初期化の順番はイメージ→プロセッサの順
  //-------------------------------------------------------------------
  // Image
  //-------------------------------------------------------------------
  // フロントイメージ
  const ErrorCodes error_front_image =
      front_image_.Create(output_pixel_format_,
                          output_width_,
                          output_height_);
  if (error_front_image != ErrorCodes::kNoError) {
    return ErrorOccured(error_front_image);
  }
  // バックイメージ
  const ErrorCodes error_back_image =
      back_image_.Create(output_pixel_format_,
                         output_width_,
                         output_height_);
  if (error_back_image != ErrorCodes::kNoError) {
    return ErrorOccured(error_back_image);
  }
  // スプラッシュイメージ
  const ErrorCodes error_splash_image =
      splash_image_.Create(output_pixel_format_,
                           output_width_,
                           output_height_);
  if (error_splash_image != ErrorCodes::kNoError) {
    return ErrorOccured(error_splash_image);
  }

  // すべてのイメージをクリア
  Clear(&front_image_);
  Clear(&back_image_);
  Clear(&splash_image_);

  // 一時的にスプラッシュスクリーンプロセッサを作ってイメージを生成しておく
  SplashScreen splash_screen;
  splash_screen.SetOutputImage(&splash_image_);
  const ErrorCodes error_splash_screen = splash_screen.Init();
  if (error_splash_screen != ErrorCodes::kNoError) {
    return ErrorOccured(error_splash_screen);
  }
  const ErrorCodes error_splash_image_pull = splash_screen.Run();
  if (error_splash_image_pull != ErrorCodes::kNoError) {
    return ErrorOccured(error_splash_image_pull);
  }

  //-------------------------------------------------------------------
  // Processor
  //-------------------------------------------------------------------
  // nop
  //-------------------------------------------------------------------

  // スレッド作成
  Create();
  CallWorker(static_cast<DWORD>(RequestTypes::kResetLayout));

  // 作成成功
  return InitDone();
}

ErrorCodes Engine::Accept(Request *request) {
  // 何かエラーが発生している場合は何もしない
  if (GetCurrentError() != ErrorCodes::kNoError) {
    return GetCurrentError();
  }

  // NULLリクエスト(何もしない)ならば、何もしない
  if (request == nullptr) {
    return GetCurrentError();
  }

  DbgLog((kLogTrace, kTraceInfo,
          TEXT("Engine: Accept")));

  // リクエストが送られてきているのならば、
  // thisを渡して処理を任せる(ダブルディスパッチ)
  // レイアウトエラーの設定はリクエストハンドラの中で行われているので
  // ここではチェックしない
  request->SendTo(this);

  /// @attention 現状、Chain of Resiposibilityはない＝
  ///            下位のプロセッサへリクエストは送らない
  return GetCurrentError();
}

//-------------------------------------------------------------------

/// @attention エラー発生中に追加の処理を行うのはEngineだけ
ErrorCodes Engine::CopyCurrentImage(BYTE *sample, DWORD data_size) {
  /// @attention processorのポインタがnullptrであることはエラーではない

  // Engine自体にエラーが発生していたら0クリア
  if (GetCurrentError() != ErrorCodes::kNoError) {
    // Splashすら表示できない状態である可能性がある
    ZeroMemory(sample, data_size);
    return GetCurrentError();
  }

  // layout_にエラーが発生していたらスプラッシュを書く
  if (GetCurrentLayoutError() != ErrorCodes::kNoError) {
    ASSERT(data_size == utilities::CalculateImageSize(splash_image_));
    avpicture_layout(splash_image_.avpicture(),
                     splash_image_.av_pixel_format(),
                     splash_image_.width(),
                     splash_image_.height(),
                     sample, data_size);
    return GetCurrentError();
  }

  // sampleにコピー
  if (last_update_image_ == ImageIndexes::kFront) {
    ASSERT(data_size == utilities::CalculateImageSize(front_image_));
    avpicture_layout(front_image_.avpicture(),
                     front_image_.av_pixel_format(),
                     front_image_.width(),
                     front_image_.height(),
                     sample, data_size);
  } else if (last_update_image_ == ImageIndexes::kBack) {
    ASSERT(data_size == utilities::CalculateImageSize(back_image_));
    avpicture_layout(back_image_.avpicture(),
                     back_image_.av_pixel_format(),
                     back_image_.width(),
                     back_image_.height(),
                     sample, data_size);
  }

  return GetCurrentError();
}


//-------------------------------------------------------------------
// リクエストハンドラ
//-------------------------------------------------------------------

void Engine::ResetLayout() {
  /// @attention enum->DWORD
  CallWorker(static_cast<DWORD>(RequestTypes::kStop));
  CallWorker(static_cast<DWORD>(RequestTypes::kResetLayout));
  CallWorker(static_cast<DWORD>(RequestTypes::kRun));
}

void Engine::SetNativeLayout() {
  /// @attention enum->DWORD
  CallWorker(static_cast<DWORD>(RequestTypes::kStop));
  CallWorker(static_cast<DWORD>(RequestTypes::kSetNativeLayout));
  CallWorker(static_cast<DWORD>(RequestTypes::kRun));
}

void Engine::SetComplexLayout() {
  /// @attention enum->DWORD
  CallWorker(static_cast<DWORD>(RequestTypes::kStop));
  CallWorker(static_cast<DWORD>(RequestTypes::kSetComplexLayout));
  CallWorker(static_cast<DWORD>(RequestTypes::kRun));
}

void Engine::SetLayoutParameters(
    int element_count,
    const LayoutParameter (&parameters)[kMaxProcessorSize]) {
  CAutoLock lock(&m_WorkerLock);
  element_count_ = element_count;
  for (int i = 0; i < kMaxProcessorSize; i++) {
    parameters_[i] = parameters[i];
    if (utilities::IsTopdownPixelFormat(output_pixel_format_)) {
      // * Topdownピクセルフォーマットの場合はbound_yの値を補正する
      // まずbound_yは左上のy座標になっているので、左下のy座標にする(y+height)
      // 左下のy座標は左上原点の座標系になっているので、左下原点の座標に直す
      parameters_[i].bound_y =
          output_height_ -
              (parameters_[i].bound_y + parameters_[i].bound_height);
    }
  }
}

//===================================================================
// キャプチャスレッド関連
//===================================================================

void Engine::DoResetLayout() {
  DbgLog((kLogTrace, kTraceInfo,
          TEXT("Engine: Reset Layout")));

  // 解放+0クリア
  if (layout_ != nullptr) {
    delete layout_;
    layout_ = nullptr;
  }
  // 未初期化
  CAutoLock lock(&m_WorkerLock);
  layout_error_code_ = ErrorCodes::kProcessorUninitializedError;
}

void Engine::DoSetNativeLayout() {
  // 現在のプロセッサは必要ないので削除
  DoResetLayout();

  //-------------------------------------------------------------------
  NativeLayout *native_layout = new NativeLayout(parameters_[0]);
  native_layout->SetOutputImage(&front_image_);
  const ErrorCodes error_layout = native_layout->Init();
  if (error_layout != ErrorCodes::kNoError) {
    // 失敗
    delete native_layout;
    LayoutErrorOccured(error_layout);
  } else {
    // 成功
    layout_ = native_layout;
    LayoutInitDone();
  }
}

void Engine::DoSetComplexLayout() {
  // 現在のプロセッサは必要ないので削除
  DoResetLayout();

  //-------------------------------------------------------------------
  ComplexLayout *complex_layout =
      new ComplexLayout(element_count_, parameters_);
  complex_layout->SetOutputImage(&front_image_);
  const ErrorCodes error_layout = complex_layout->Init();
  if (error_layout != ErrorCodes::kNoError) {
    // 失敗
    delete complex_layout;
    LayoutErrorOccured(error_layout);
  } else {
    // 成功
    layout_ = complex_layout;
    LayoutInitDone();
  }
}

void Engine::DoLoop() {
  // システムクロックを取得
  IReferenceClock *system_clock = nullptr;
  HRESULT result_system_clock = CoCreateInstance(
    CLSID_SystemClock,
    NULL, // nullptr
    CLSCTX_INPROC_SERVER,
    IID_IReferenceClock,
    reinterpret_cast<void**>(&system_clock));
  ASSERT(result_system_clock == S_OK);
  ASSERT(system_clock != nullptr);

  // 初期化
  DWORD request;
  const REFERENCE_TIME output_frame_interval =
      static_cast<REFERENCE_TIME>(UNITS / output_fps_);
  REFERENCE_TIME zero = 0LL;
  system_clock->GetTime(&zero);
  REFERENCE_TIME now = zero;
  int64_t frame_counter = 0LL;

  do {
    while (!CheckRequest(&request)) {
      Update();
      system_clock->GetTime(&now);
      now -= zero;

      // 想定フレームを計算＋フレームカウンタ更新
      REFERENCE_TIME tmp_start = frame_counter * output_frame_interval;
      REFERENCE_TIME tmp_end = tmp_start + output_frame_interval;
      ++frame_counter;

      // すでに現在時刻が次の想定フレームの中にある場合
      //    = フレームスキップが絶対発生する
      if (tmp_end + output_frame_interval < now) {
        int skip_count = 0;
        do {
          // 想定フレームを再計算＋フレームカウンタ更新
          tmp_start = frame_counter * output_frame_interval;
          tmp_end = tmp_start + output_frame_interval;
          ++frame_counter;
          ++skip_count;
        } while (tmp_end < now);
        // 現在時刻がフレームの終了時よりも前になるまでスキップ
        DbgLog((kLogError, kErrorWarn,
                TEXT("Engine: Frame Skip Occured(%d)"),
                skip_count));
      }

      // Sleep時間を計算
      const REFERENCE_TIME sleep_interval = tmp_end - now;
      const DWORD sleep_interval_msec =
          static_cast<DWORD>((sleep_interval * MILLISECONDS) / UNITS);

      if (tmp_end < now) {
        // Sleepするべき時間がすでに過ぎてしまった
        DbgLog((kLogError, kErrorWarn, TEXT("Engine: Drop Frame")));
      } else {
        // Sleepしないとフレームを生成しすぎる
        //    = フレーム終了まで待つ
        ::Sleep(sleep_interval_msec);
      }
    }

    /// @attention enum->DWORD
    if (request == static_cast<DWORD>(RequestTypes::kRun)) {
      Reply(NOERROR);
    }
  } while (request != static_cast<DWORD>(RequestTypes::kStop));

  // システムクロック解放
  if (system_clock != nullptr) {
    system_clock->Release();
    system_clock = nullptr;
  }
}

DWORD Engine::ThreadProc() {
  HRESULT result = ERROR;
  RequestTypes request = RequestTypes::kInvalid;

  do {
    DWORD actual_request = GetRequest();
    /// @warning DWORD->enum
    request = static_cast<RequestTypes>(actual_request);

    switch (request) {
      case RequestTypes::kResetLayout: {
        DoResetLayout();
        Reply(NOERROR);
        break;
      }
      case RequestTypes::kSetNativeLayout: {
        DoSetNativeLayout();
        Reply(NOERROR);
        break;
      }
      case RequestTypes::kSetComplexLayout: {
        DoSetComplexLayout();
        Reply(NOERROR);
        break;
      }
      case RequestTypes::kRun: {
        Reply(NOERROR);
        DoLoop();
        break;
      }
      case RequestTypes::kStop:
      case RequestTypes::kExit: {
        Reply(NOERROR);
        break;
      }
    }
  } while (request != RequestTypes::kExit);

  return 0;
}

ErrorCodes Engine::Run() {
  ASSERT(layout_ != nullptr);
  const ErrorCodes error = layout_->Run();
  if (error != ErrorCodes::kNoError) {
    /// @attention layout_でエラーが発生してもEngine自体はエラー状態ではない
    LayoutErrorOccured(error);
  }
  return GetCurrentError();
}

void Engine::Update() {
  if (GetCurrentLayoutError() != ErrorCodes::kNoError) {
    return;
  }

  if (last_update_image_ == ImageIndexes::kFront) {
    layout_->SwapOutputImage(&back_image_);
    {
      CAutoLock lock(&m_WorkerLock);
      if (need_clear_back_image_) {
        Clear(&back_image_);
        need_clear_back_image_ = false;
      }
    }
    Run();
    last_update_image_ = ImageIndexes::kBack;
  } else if (last_update_image_ == ImageIndexes::kBack) {
    layout_->SwapOutputImage(&front_image_);
    {
      CAutoLock lock(&m_WorkerLock);
      if (need_clear_front_image_) {
        Clear(&front_image_);
        need_clear_front_image_ = false;
      }
    }
    Run();
    last_update_image_ = ImageIndexes::kFront;
  }
}

ErrorCodes Engine::LayoutInitDone() {
  CAutoLock lock(&m_WorkerLock);
  ASSERT(layout_error_code_ == ErrorCodes::kProcessorUninitializedError);
  if (layout_error_code_ == ErrorCodes::kProcessorUninitializedError) {
    layout_error_code_ = ErrorCodes::kNoError;

    // 次回更新時に一回クリアする
    need_clear_front_image_ = true;
    need_clear_back_image_ = true;
  }
  return layout_error_code_;
}

ErrorCodes Engine::LayoutErrorOccured(ErrorCodes error_code) {
  CAutoLock lock(&m_WorkerLock);
  if (error_code != ErrorCodes::kNoError) {
    // 後からkNoErrorにしようとしてもできない
    // ASSERT(false);
    DbgLog((kLogError, kError,
            TEXT("Engine: Layout Error Occured(%d)"),
            error_code));
    layout_error_code_ = error_code;
  }
  return layout_error_code_;
}

ErrorCodes Engine::GetCurrentLayoutError() {
  CAutoLock lock(&m_WorkerLock);
  return layout_error_code_;
}

}   // namespace scff_imaging
