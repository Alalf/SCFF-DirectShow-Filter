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

/// @file base/scff-quality-controlled-time.h
/// @brief SCFFQualityControlledTimeの宣言

#ifndef SCFF_DSF_BASE_SCFF_QUALITY_CONTROLLED_TIME_H_
#define SCFF_DSF_BASE_SCFF_QUALITY_CONTROLLED_TIME_H_

#include <streams.h>

/// @brief タイムマネージャ
class SCFFQualityControlledTime {
 public:
  /// @brief コンストラクタ
  SCFFQualityControlledTime();

  /// @brief デストラクタ
  ~SCFFQualityControlledTime();

  /// @brief ストリームタイムをリセット
  void Reset(double fps, IReferenceClock *graph_clock);

  /// @brief 現在のフレーム区間を取得
  REFERENCE_TIME frame_interval() const;

  /// @brief Qualityパラメータを利用して調整
  /// @return 発生した遅延時間
  REFERENCE_TIME Adjust(const Quality &quality);

  /// @brief sample->SetTime用のタイムスタンプを返す
  void GetTimestamp(
      REFERENCE_TIME filter_zero,
      REFERENCE_TIME *start, REFERENCE_TIME *end) const;

  /// @brief Nowを更新する。値はGetTimestampで取得したendにすること。
  void UpdateNow(REFERENCE_TIME end);

 private:
  /// @brief 現在のストリームタイム(100nSec)
  REFERENCE_TIME now_;

  /// @brief ストリームタイム基準時
  REFERENCE_TIME zero_;

  /// @brief 目標FPS
  double target_fps_;

  /// @brief 目標フレーム区間(100nSec)
  REFERENCE_TIME target_frame_interval_;

  /// @brief IQualityControl::Notifyによる指定フレーム区間(100nSec)
  REFERENCE_TIME frame_interval_;
};

#endif  // SCFF_DSF_BASE_SCFF_QUALITY_CONTROLLED_TIME_H_
