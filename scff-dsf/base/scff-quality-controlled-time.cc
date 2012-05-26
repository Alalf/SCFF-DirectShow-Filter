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

/// @file base/scff-quality-controlled-time.cc
/// @brief SCFFQualityControlledTimeの定義

#include "base/scff-quality-controlled-time.h"

#include "base/debug.h"
#include "base/constants.h"

//=====================================================================
// ユーティリティ関数
//=====================================================================

// 秒指定のFPSをフレーム区間の長さ(100nSec)に変換
static REFERENCE_TIME FPSToFrameInterval(double fps) {
  return static_cast<REFERENCE_TIME>(UNITS / fps);
}

//=====================================================================
// SCFFQualityControlledTime
//=====================================================================

// コンストラクタ
SCFFQualityControlledTime::SCFFQualityControlledTime()
    : now_(-1),                     // ありえない値
      target_fps_(-1),              // ありえない値
      target_frame_interval_(-1),   // ありえない値
      frame_interval_(-1) {         // ありえない値
  // nop
}

// デストラクタ
SCFFQualityControlledTime::~SCFFQualityControlledTime() {
  // nop
}

// ストリームタイムをリセット
void SCFFQualityControlledTime::Reset(double fps) {
  target_fps_ = fps;
  target_frame_interval_ = FPSToFrameInterval(fps);
  frame_interval_ = target_frame_interval_;

  now_ = 0;

  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("SCFFQualityControlledTime: RESET!!!!!!!!!!!!")));
}

// 現在のフレーム区間を取得
REFERENCE_TIME SCFFQualityControlledTime::frame_interval() const {
  return frame_interval_;
}

// Qualityパラメータを利用して調整
/// @warning ロックをかけずに使っているので、各メンバ変数への書き込みは1回まで!
REFERENCE_TIME SCFFQualityControlledTime::Adjust(const Quality &quality) {
  const REFERENCE_TIME min_frame_interval = target_frame_interval_;
  const REFERENCE_TIME max_frame_interval = kMaxFrameInterval;      // 0.1fps

  if (quality.Proportion <= 0) {
    // フレーム区間の長さが永遠に長くならないように
    frame_interval_ = max_frame_interval;
  } else {
    // フレーム区間の長さをクオリティレート(100% = 1000)にあわせて伸張
    // Proptionは1000以上にはならないので、1000以外ではframe_interval_が長くなる
    frame_interval_ *= 1000 / quality.Proportion;

    // 長さがオーバーしないように
    if (max_frame_interval < frame_interval_) {
      frame_interval_ = max_frame_interval;
    } else if (frame_interval_ < min_frame_interval) {
      frame_interval_ = min_frame_interval;
    }
  }

  // フレームスキップ発生
  if (quality.Late > 0) {
    now_ += quality.Late;
    return quality.Late;
  } else {
    return 0;
  }
}

// sample->SetTime用のタイムスタンプを返す
void SCFFQualityControlledTime::GetTimestamp(REFERENCE_TIME *start,
                                             REFERENCE_TIME *end) const {
  *start = now_;
  *end = *start + frame_interval_;
}

// Nowを更新する。値はGetTimestampで取得したendにすること。
void SCFFQualityControlledTime::UpdateNow(REFERENCE_TIME end) {
  now_ = end;
}
