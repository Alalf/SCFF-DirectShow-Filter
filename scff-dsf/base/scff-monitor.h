
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

/// @file base/scff-monitor.h
/// @brief SCFFMonitorの宣言

#ifndef SCFF_DSF_BASE_SCFF_MONITOR_H_
#define SCFF_DSF_BASE_SCFF_MONITOR_H_

#include <cstdint>
#include <ctime>

#include "base/scff-interprocess.h"
#include "imaging/imaging.h"

/// @brief SCFF DirectShow Filterの外部インターフェースを担当するクラス
class SCFFMonitor {
 public:
  /// @brief コンストラクタ
  SCFFMonitor();
  /// @brief デストラクタ
  ~SCFFMonitor();

  /// @brief 初期化
  bool Init(imaging::ImagePixelFormat pixel_format,
            int width, int height, double fps);

  /// @brief リクエストがあるかどうか調べ、あれば実体を、なければNULLを返す
  imaging::Request* CreateRequest();
  /// @brief 使い終わったリクエストを解放する
  void ReleaseRequest(imaging::Request* request);

 private:
  /// @brief プロセス間通信のためのインスタンス
  SCFFInterprocess interprocess_;

  /// @brief 内部時刻を保持するための変数
  clock_t last_polling_clock_;

  /// @brief 最後に受信したSCFFMessageのタイムスタンプ
  int32_t last_message_timestamp_;
};

#endif  // SCFF_DSF_BASE_SCFF_MONITOR_H_
