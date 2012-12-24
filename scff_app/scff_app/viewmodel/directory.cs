// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
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

/// @file scff_app/viewmodel/directory.cs
/// scff_app.viewmodel.Directoryのメソッドの定義

namespace scff_app.viewmodel {

using System.ComponentModel;

// scff_interprocess.Directoryのビューモデル
partial class Directory {

  /// デフォルトコンストラクタ
  public Directory() {
    this.Init();
  }

  /// scff_interprocessから変換
  public void LoadFromInterprocess(scff_interprocess.Directory input) {
    this.Entries.Clear();
    const int kMaxEntry = scff_interprocess.Interprocess.kMaxEntry;
    for (int i = 0; i < kMaxEntry; i++) {
      if (input.entries[i].process_id == 0) {
        continue;
      }
      this.Entries.Add(new Entry(input.entries[i]));
    }
  }

  //-------------------------------------------------------------------

  /// デフォルトパラメータを設定
  void Init() {
    this.Entries = new BindingList<Entry>();
  }
}
}
