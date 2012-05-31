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

/// @file scff-app/data/message-factory.cs
/// @brief scff_*.Message生成・変換用メソッドの定義

namespace scff_app.data {

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

// scff_inteprocess.Messageをマネージドクラス化したクラス
partial class Message {

  /// @brief NullLayoutのMessageに設定する
  public void Reset() {
    this.Timestamp = DateTime.Now.Ticks;
    this.LayoutType = scff_interprocess.LayoutType.kNullLayout;
    this.LayoutElementCount = 0;
    this.LayoutParameters.Clear();
  }

  /// @brief BindingSourceから値を読み込んで設定
  public void Uppate(ref BindingSource layoutParameters) {
    Debug.Assert(layoutParameters.Count >= 1);

    this.Timestamp = DateTime.Now.Ticks;

    if (layoutParameters.Count == 1) {
      this.LayoutType = scff_interprocess.LayoutType.kNativeLayout;
    } else {
      this.LayoutType = scff_interprocess.LayoutType.kComplexLayout;
    }

    this.LayoutElementCount = layoutParameters.Count;

    this.LayoutParameters.Clear();
    foreach (LayoutParameter i in layoutParameters) {
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

  /// @brief 共有メモリに現在編集中のMessageを書き込む
  public void Send(ref scff_interprocess.Interprocess interprocess,
                   ref BindingSource entries,
                   bool show_message) {
    if (entries.Count == 0) {
      // 書き込み先が存在しない
      if (show_message) {
        MessageBox.Show("No process to send message.");
      }
      return;
    }

    try {
      /// @warning DWORD->int変換！オーバーフローの可能性あり
      Process.GetProcessById((int)((Entry)entries.Current).ProcessID);
    } catch {
      // プロセスが存在しない場合
      if (show_message) {
        MessageBox.Show("Cannot find process(" + ((Entry)entries.Current).ProcessID + ").");
      }
      return;
    }
    
    // 共有メモリへのアクセス準備
    interprocess.InitMessage(((Entry)entries.Current).ProcessID);

    // 共有メモリへデータを書き込む
    interprocess.SendMessage(
        this.ToInterprocess(((Entry)entries.Current).SampleWidth,
                            ((Entry)entries.Current).SampleHeight));
  }
}
}
