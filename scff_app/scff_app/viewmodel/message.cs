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

/// @file scff_app/viewmodel/message.cs
/// scff_app.viewmodel.Messageのメソッドの定義

namespace scff_app.viewmodel {

using System;
using System.ComponentModel;
using System.Text;

// scff_inteprocess.Messageのビューモデル
partial class Message {

  /// デフォルトコンストラクタ
  public Message() {
    this.Init();
  }

  /// 検証
  public bool IsValid() {
    bool result = true;
    foreach (LayoutParameter i in this.LayoutParameters) {
      result &= i.IsValid();
    }
    return result;
  }

  /// エラー
  public string Error {
    get {
      StringBuilder error_string = new StringBuilder();
      int index = 1;
      foreach (LayoutParameter i in this.LayoutParameters) {
        if (i.Error != string.Empty) {
          error_string.AppendLine("["+index+"] "+i.WindowText);
          error_string.AppendLine(i.Error);
        }
        ++index;
      }
      return error_string.ToString();
    }
  }

  /// scff_Interprocess用に変換
  public scff_interprocess.Message ToInterprocess(int bound_width, int bound_height, bool force_null_layout) {
    scff_interprocess.Message output = new scff_interprocess.Message();

    output.timestamp = this.Timestamp;
    if (force_null_layout) {
      output.layout_type = (Int32)scff_interprocess.LayoutTypes.kNullLayout;
    } else {
      output.layout_type = (Int32)this.LayoutType;
    }
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

  /// デフォルトパラメータを設定
  void Init() {
    this.LayoutParameters = new BindingList<LayoutParameter>();
  }
}
}
