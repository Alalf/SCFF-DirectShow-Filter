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

/// @file base/scff-clock-time.h
/// @brief SCFFClockTimeの宣言

#ifndef SCFF_DSF_BASE_SCFF_CLOCK_TIME_H_
#define SCFF_DSF_BASE_SCFF_CLOCK_TIME_H_

#include <streams.h>
#include <cstdint>

/// @brief SCFFQualityControlledTimeが使えない場合の代替手段
///- 特にFFMpegでは全てのメディアタイムスタンプが無視されるため、
///  FillBufferの速度を自分で調整しなければならない
///- よってこのクラスの目的は、正確なタイムスタンプを求めることではなく
///  正確なSleepTimeを測定することが目的といえよう
class SCFFClockTime {
 public:
  /// @brief コンストラクタ
  SCFFClockTime();

  /// @brief デストラクタ
  ~SCFFClockTime();

  /// @brief ストリームタイムをリセット
  void Reset(double fps, IReferenceClock *graph_clock);

  /// @brief sample->SetTime用のストリームタイムを返す
  /// @attention フレームカウンタも更新しているのでconstではない
  void GetTimestamp(REFERENCE_TIME filter_zero,
                    REFERENCE_TIME *start, REFERENCE_TIME *end);

  /// @brief fpsが上限を超えないようにSleepをかける
  /// @attention 具体的には直前のGetTimestampのendまでSleep
  void Sleep(REFERENCE_TIME filter_zero);

 private:
  /// @brief 現在のストリームタイムを得る
  REFERENCE_TIME GetNow(REFERENCE_TIME filter_zero);

  /// @brief メディアサンプルに付加するタイムスタンプ計算用グラフクロック
  IReferenceClock *graph_clock_;

  /// @brief メディアサンプルに付加するタイムスタンプ計算用システムクロック
  IReferenceClock *system_clock_;

  /// @brief 目標フレーム区間(REFERENCE_TIME)
  REFERENCE_TIME target_frame_interval_;

  /// @brief ストリームタイム基準時
  REFERENCE_TIME zero_;

  /// @brief 補正用カーソル(グラフクロック)
  REFERENCE_TIME graph_cursor_;

  /// @brief 補正用カーソル(システムクロック)
  REFERENCE_TIME system_cursor_;

  /// @brief フレームカウンタ
  int64_t frame_counter_;

  /// @brief 巻き戻り監視用ストリームタイム(100nSec)
  REFERENCE_TIME last_;

  /// @brief 直前のGetTimestampのend
  REFERENCE_TIME last_end_;
};

#endif  // SCFF_DSF_BASE_SCFF_CLOCK_TIME_H_
