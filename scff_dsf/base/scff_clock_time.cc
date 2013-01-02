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

/// @file base/scff_clock_time.cc
/// SCFFClockTimeの定義

#include "base/scff_clock_time.h"

#include "base/debug.h"

//=====================================================================
// SCFFClockTime
//=====================================================================

SCFFClockTime::SCFFClockTime()
    : graph_clock_(nullptr),
      system_clock_(nullptr),
      target_frame_interval_(-1LL),   // ありえない値
      zero_(-1LL),                    // ありえない値
      graph_cursor_(-1LL),            // ありえない値
      system_cursor_(-1LL),           // ありえない値
      frame_counter_(-1LL),           // ありえない値
      last_(-1LL),                    // ありえない値
      last_end_(-1LL) {               // ありえない値
  // nop
}

SCFFClockTime::~SCFFClockTime() {
  if (system_clock_ != nullptr) {
    system_clock_->Release();
    system_clock_ = nullptr;
  }
  if (graph_clock_ != nullptr) {
    graph_clock_->Release();
    graph_clock_ = nullptr;
  }
}

void SCFFClockTime::Reset(double fps, CSource* parent) {
  if (system_clock_ != nullptr) {
    system_clock_->Release();
    system_clock_ = nullptr;
  }
  if (graph_clock_ != nullptr) {
    graph_clock_->Release();
    graph_clock_ = nullptr;
  }

  // システムクロックを取得
  HRESULT result_system_clock = CoCreateInstance(
    CLSID_SystemClock,
    NULL, // nullptr
    CLSCTX_INPROC_SERVER,
    IID_IReferenceClock,
    reinterpret_cast<void**>(&system_clock_));
  ASSERT(result_system_clock == S_OK);
  ASSERT(system_clock_ != nullptr);

  // グラフクロックを取得
  ASSERT(parent != nullptr);
  IReferenceClock *graph_clock;
  HRESULT result_graph_clock = parent->GetSyncSource(&graph_clock);
  ASSERT(result_graph_clock == S_OK);
  if (graph_clock != nullptr) {
    graph_clock_ = graph_clock;
  } else {
    // 参照カウント追加
    system_clock_->AddRef();
    graph_clock_ = system_clock_;
  }

  target_frame_interval_ = static_cast<REFERENCE_TIME>(UNITS / fps);
  graph_clock_->GetTime(&zero_);
  frame_counter_ = 0LL;
  last_end_ = 0LL;

  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("SCFFClockTime: RESET!!!!!!!!!!!!")));
}

REFERENCE_TIME SCFFClockTime::GetNow(REFERENCE_TIME filter_zero) {
  ASSERT(graph_clock_ != nullptr);
  ASSERT(system_clock_ != nullptr);

  REFERENCE_TIME system_now;
  system_clock_->GetTime(&system_now);
  const double system_delta_sec =
      static_cast<double>(system_now - system_cursor_) / UNITS;
  if (system_delta_sec > 60.0) {
    // 1分ごとにzero_を計算しなおす
    system_cursor_ = system_now;
    graph_clock_->GetTime(&graph_cursor_);
  }

  REFERENCE_TIME now = graph_cursor_ + (system_now - system_cursor_);
  if (now < last_) {
    // 巻き戻らないように調整
    now = last_;
  }
  last_ = now;

  // zero_がparentから得られる場合がある
  REFERENCE_TIME delta_zero = zero_ - filter_zero;
  if (delta_zero < 0LL) {
    delta_zero = -delta_zero;
  }
  if (delta_zero < UNITS) {
    // ずれが1秒以下
    return now - filter_zero;
  } else {
    return now - zero_;
  }
}

void SCFFClockTime::GetTimestamp(REFERENCE_TIME filter_zero,
                                 REFERENCE_TIME *start,
                                 REFERENCE_TIME *end) {
  // 現在のストリームタイムを取得
  const REFERENCE_TIME now_in_stream = GetNow(filter_zero);

#if defined(FOR_FFMPEG)
  // 最初の1回だけは正確な値を計算する
  if (frame_counter_ == 0LL) {
    REFERENCE_TIME tmp_start;
    REFERENCE_TIME tmp_end;

    int skip_count = 0;
    do {
        // 想定フレームを再計算＋フレームカウンタ更新
        tmp_start = frame_counter_ * target_frame_interval_;
        tmp_end = tmp_start + target_frame_interval_;
        ++frame_counter_;
        ++skip_count;
    } while (tmp_end < now_in_stream);
    // 現在時刻がフレームの終了時よりも前になるまでスキップ
    MyDbgLog((LOG_TRACE, kDbgImportant,
              TEXT("SCFFClockTime: Inital Skip Occured(%d)"),
              skip_count));
  }

  // 想定フレームを計算＋フレームカウンタ更新
  *start = frame_counter_ * target_frame_interval_;
  *end = *start + target_frame_interval_;
  last_end_ = *end;
  ++frame_counter_;
#else
  // 想定フレームを計算＋フレームカウンタ更新
  REFERENCE_TIME tmp_start = frame_counter_ * target_frame_interval_;
  REFERENCE_TIME tmp_end = tmp_start + target_frame_interval_;
  ++frame_counter_;

  // すでに現在時刻が次の想定フレームの中にある場合
  //    = フレームスキップが絶対発生する
  if (tmp_end + target_frame_interval_ < now_in_stream) {
    int skip_count = 0;
    do {
        // 想定フレームを再計算＋フレームカウンタ更新
        tmp_start = frame_counter_ * target_frame_interval_;
        tmp_end = tmp_start + target_frame_interval_;
        ++frame_counter_;
        ++skip_count;
    } while (tmp_end < now_in_stream);
    // 現在時刻がフレームの終了時よりも前になるまでスキップ
    MyDbgLog((LOG_TRACE, kDbgImportant,
              TEXT("SCFFClockTime: Frame Skip Occured(%d)"),
              skip_count));
  }

  /// @attention タイムスタンプは必ずしも１フレームに収める必要はない
  *start = tmp_start;
  //*start = last_end_+1;
  *end = tmp_end;
  last_end_ = tmp_end;
#endif
}

void SCFFClockTime::Sleep(REFERENCE_TIME filter_zero) {
  // 現在のストリームタイムを取得
  const REFERENCE_TIME now_in_stream = GetNow(filter_zero);
  const REFERENCE_TIME sleep_interval = last_end_ - now_in_stream;
  const DWORD sleep_interval_msec = static_cast<DWORD>(
      (sleep_interval * MILLISECONDS) / UNITS);

  if (last_end_ < now_in_stream) {
    // Sleepするべき時間がすでに過ぎてしまった
    return;
  } else {
    // Sleepしないとフレームを生成しすぎる
    //    = フレーム終了まで待つ
    ASSERT(sleep_interval_msec < 10000);   //10秒以上はさすがにバグだろう
    ::Sleep(sleep_interval_msec);
  }
}
