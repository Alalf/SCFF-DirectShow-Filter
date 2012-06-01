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

/// @file scff-app/data/directory-interprocess.cs
/// @brief 共有メモリから現在のDirectoryを取得するメソッドの定義

namespace scff_app.data {

using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

// scff_interprocess.Directoryをマネージドクラス化したクラス
partial class Directory {

  /// @brief 共有メモリから現在のDirectoryを取得する
  public void Update(scff_interprocess.Interprocess interprocess) {
    // 共有メモリへのアクセス準備
    interprocess.InitDirectory();

    // 共有メモリからデータを取得
    scff_interprocess.Directory interprocess_directory;
    interprocess.GetDirectory(out interprocess_directory);

    // 取得したデータから値を設定
    InitFromInterprocess(interprocess_directory);
  }

  /// @brief BindingSourceに対応する値を設定する
  public void UpdateBindingSource(BindingSource entries) {
    entries.Clear();
    foreach (Entry i in this.Entries) {
      entries.Add(i);
    }
  }
}
}
