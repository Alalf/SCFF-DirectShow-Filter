// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

/// @file base/scff_monitor.h
/// SCFFMonitorの宣言

#ifndef SCFF_DSF_BASE_SCFF_MONITOR_H_
#define SCFF_DSF_BASE_SCFF_MONITOR_H_

#include <cstdint>
#include <ctime>

#include "scff_interprocess/interprocess.h"
#include "scff_imaging/imaging.h"

/// SCFF DirectShow Filterの外部インターフェースを担当するクラス
class SCFFMonitor {
 public:
  /// コンストラクタ
  SCFFMonitor();
  /// デストラクタ
  ~SCFFMonitor();

  /// 初期化
  bool Init(scff_imaging::ImagePixelFormat pixel_format,
            int width, int height, double fps);

  /// リクエストがあるかどうか調べ、あれば実体を、なければnullptrを返す
  scff_imaging::Request* CreateRequest();
  /// 使い終わったリクエストを解放する
  void ReleaseRequest(scff_imaging::Request *request);

 private:
  /// プロセス間通信のためのインスタンス
  scff_interprocess::Interprocess interprocess_;

  /// 内部時刻を保持するための変数
  clock_t last_polling_clock_;

  /// 最後に受信したSCFFMessageのタイムスタンプ
  int64_t last_message_timestamp_;
};

#endif  // SCFF_DSF_BASE_SCFF_MONITOR_H_
