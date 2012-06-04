// Copyright 2012 Progre <djyayutto_at_gmail.com>
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

/// @file scff-app/view/LayoutForm.cs
/// @brief レイアウトをGUIで編集するためのフォームの定義
/// @todo(progre) 移植未達部分が完了次第名称含め全体をリファクタリング
/// @todo(me) 全体的にBindingSourceをカスタムコントロールで利用する方法さえわかれば、
///           いろいろとエレガントに対応できそうではある

namespace scff_app.view {

using System.Drawing;
using System.Windows.Forms;
using scff_app.viewmodel;

/// @brief レイアウトをGUIで編集するためのフォーム
partial class LayoutForm : Form {

  /// @brief コンストラクタ
  public LayoutForm(BindingSource entries, BindingSource layoutParameters) {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------

    entries_ = entries;
    layout_parameters_ = layoutParameters;
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  private void LayoutForm_Load(object sender, System.EventArgs e) {
    // Directoryから現在選択中のEntryを取得し、出力幅、高さを得る
    Entry current_entry = (Entry)entries_.Current;
    int bound_width = SCFFApp.kDefaultBoundWidth;
    int bound_height = SCFFApp.kDefaultBoundHeight;
    if (entries_.Count != 0) {
      // 現在選択中のプロセスの幅、高さで調整
      bound_width = current_entry.SampleWidth;
      bound_height = current_entry.SampleHeight;
    }

    // レイアウトパネルのサイズを調整
    this.layoutPanel.Size = new Size(bound_width, bound_height);
    this.layoutPanel.DataSource = layout_parameters_;
  }

  private void add_Click(object sender, System.EventArgs e) {
    /// @todo(me) 実装
  }

  private void remove_Click(object sender, System.EventArgs e) {
    /// @todo(me) 実装
  }

  private void apply_Click(object sender, System.EventArgs e) {
    this.layoutPanel.Apply();

    this.DialogResult = System.Windows.Forms.DialogResult.OK;
    Close();
  }

  private void cancel_Click(object sender, System.EventArgs e) {
    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    Close();
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  BindingSource entries_;
  BindingSource layout_parameters_;
}
}
