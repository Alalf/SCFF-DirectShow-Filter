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

/// @file scff-app/viewmodel/message.cs
/// @brief scff_app.viewmodel.Messageのメソッドの定義

namespace scff_app.viewmodel {

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

// scff_inteprocess.Messageのビューモデル
partial class Message {

  /// @brief デフォルトコンストラクタ
  public Message() {
    this.Init();
  }

  /// @brief NullLayoutのMessageに設定する
  public void Reset() {
    this.Timestamp = DateTime.Now.Ticks;
    this.LayoutType = scff_interprocess.LayoutType.kNullLayout;
    this.LayoutElementCount = 0;
    this.LayoutParameters.Clear();
  }

  /// @brief IListから値を読み込んで設定
  public void Load(IList layout_parameters) {
    Debug.Assert(layout_parameters.Count >= 1);

    this.Timestamp = DateTime.Now.Ticks;

    if (layout_parameters.Count == 1) {
      this.LayoutType = scff_interprocess.LayoutType.kNativeLayout;
    } else {
      this.LayoutType = scff_interprocess.LayoutType.kComplexLayout;
    }

    this.LayoutElementCount = layout_parameters.Count;

    this.LayoutParameters.Clear();
    foreach (LayoutParameter i in layout_parameters) {
      this.LayoutParameters.Add(i);
    }
  }

  /// @brief 検証
  public bool Validate(bool show_message) {
    foreach (LayoutParameter i in this.LayoutParameters) {
      if (!i.Validate(show_message)) {
        return false;
      }
    }
    return true;
  }

  /// @brief scff_Interprocess用に変換
  public scff_interprocess.Message ToInterprocess(int bound_width, int bound_height) {
    scff_interprocess.Message output = new scff_interprocess.Message();

    output.timestamp = this.Timestamp;
    output.layout_type = (Int32)this.LayoutType;
    output.layout_element_count = this.LayoutElementCount;
    
    // Listの前から順番に書き込む
    const int kMaxComplexLayoutElements = scff_interprocess.Interprocess.kMaxComplexLayoutElements;
    output.layout_parameters = new scff_interprocess.LayoutParameter[kMaxComplexLayoutElements];
    for (int i = 0; i < kMaxComplexLayoutElements; i++) {
      if (i < this.LayoutParameters.Count) {
        output.layout_parameters[i] = this.LayoutParameters[i].ToInterprocess(bound_width, bound_height);
      } else {
        // C#はインスタンスは勝手にゼロクリアされる
        output.layout_parameters[i] = new scff_interprocess.LayoutParameter();
      }
    }

    return output;
  }

  //-------------------------------------------------------------------

  /// @brief デフォルトパラメータを設定
  void Init() {
    this.Timestamp = DateTime.Now.Ticks;
    this.LayoutType = scff_interprocess.LayoutType.kNullLayout;
    this.LayoutElementCount = 0;
    this.LayoutParameters = new List<LayoutParameter>();
  }
}
}
