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

/// @file scff-app/data/directory-factory.cs
/// @brief scff_*.Directory生成・変換用メソッドの定義

namespace scff_app.data {

using System.Collections.Generic;

// scff_interprocess.Directoryをマネージドクラス化したクラス
partial class Directory {

  /// @brief デフォルトコンストラクタ
  public Directory() {
    // nop
  }

  /// @brief 変換コンストラクタ
  public Directory(scff_interprocess.Directory input) {
    InitByInterprocess(input);
  }

  /// @brief scff_interprocessモジュールのパラメータを生成
  public scff_interprocess.Directory ToInterprocessDirectory() {
    scff_interprocess.Directory output = new scff_interprocess.Directory();

    // Listの前から順番に書き込む
    const int kMaxEntry = scff_interprocess.Interprocess.kMaxEntry;
    output.entries = new scff_interprocess.Entry[kMaxEntry];
    for (int i = 0; i < kMaxEntry; i++) {
      if (i < this.Entries.Count) {
        output.entries[i] = this.Entries[i].ToInterprocessEntry();
      } else {
        // C#はインスタンスは勝手にゼロクリアされる
        output.entries[i] = new scff_interprocess.Entry();
      }
    }

    return output;
  }

  //-------------------------------------------------------------------

  /// @brief scff_interprocessモジュールのパラメータから生成
  void InitByInterprocess(scff_interprocess.Directory input) {
    const int kMaxEntry = scff_interprocess.Interprocess.kMaxEntry;
    for (int i = 0; i < kMaxEntry; i++) {
      if (input.entries[i].process_id == 0) {
        continue;
      }
      Entry entry = new Entry(input.entries[i]);
      this.Entries.Add(entry);
    }
  }
}
}
