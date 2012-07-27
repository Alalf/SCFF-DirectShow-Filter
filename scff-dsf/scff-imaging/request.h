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

/// @file scff-imaging/request.h
/// @brief scff_imaging::Requestの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_REQUEST_H_
#define SCFF_DSF_SCFF_IMAGING_REQUEST_H_

#include "scff-imaging/engine.h"

namespace scff_imaging {

/// @brief エンジンに対するリクエストをカプセル化したクラス
class Request {
 public:
  /// @brief デストラクタ
  virtual ~Request() {
    // nop
  }
  /// @brief ダブルディスパッチ用
  virtual void SendTo(Engine *engine) const = 0;
 protected:
  /// @brief コンストラクタ
  Request() {
    // nop
  }
};

/// @brief リクエスト: ResetLayout
class ResetLayoutRequest : public Request {
 public:
  /// @brief コンストラクタ
  ResetLayoutRequest() : Request() {
    // このリクエストは特に情報を必要としない
  }
  /// @brief デストラクタ
  ~ResetLayoutRequest() {
    // nop
  }
  /// @brief ダブルディスパッチ用
  void SendTo(Engine *engine) const {
    engine->ResetLayout();
  }
};

/// @brief リクエスト: SetLayout
class SetLayoutRequest : public Request {
 public:
  /// @brief コンストラクタ
  SetLayoutRequest(
      int element_count,
      const LayoutParameter (&parameters)[kMaxProcessorSize])
      : Request(),
        element_count_(element_count) {
    // 配列の初期化
    for (int i = 0; i < kMaxProcessorSize; i++) {
      parameters_[i] = parameters[i];
    }
  }
  /// @brief デストラクタ
  ~SetLayoutRequest() {
    // nop
  }
  /// @brief ダブルディスパッチ用
  void SendTo(Engine *engine) const {
    engine->SetLayoutParameters(element_count_, parameters_);
    if (element_count_ == 1) {
      engine->SetNativeLayout();
    } else {
      engine->SetComplexLayout();
    }
  }

 private:
  /// @brief レイアウト要素数
  const int element_count_;

  /// @brief レイアウトパラメータ
  LayoutParameter parameters_[kMaxProcessorSize];
};

}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_REQUEST_H_
