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

/// @file scff_app/view/LayoutPanel.cs
/// LayoutPanelコントロールの定義

namespace scff_app.view {

using System.ComponentModel;
using System.Windows.Forms;
using scff_app.viewmodel;

[ComplexBindingProperties("DataSource", "DataMember")]
partial class LayoutPanel : UserControl {

  public LayoutPanel() {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------
  }

  public void Apply() {
    foreach (PreviewControl i in this.Controls) {
      i.Apply();
    }
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  private void LayoutPanel_Load(object sender, System.EventArgs e) {
    // nop
  }

  //===================================================================
  // プロパティ
  //===================================================================

  public object DataSource {
    get { return data_source_; }
    set {
      data_source_ = value;

      if (data_source_ != null) {
        BuildControls();
      }
    }
  }
  object data_source_;

  public string DataMember {
    get{ return data_member_; }
    set{ data_member_ = value; }
  }
  string data_member_;

  //-------------------------------------------------------------------

  void BuildControls() {
    int index = 0;
    foreach (LayoutParameter i in (BindingSource)data_source_) {
      // PreviewControlの生成
      PreviewControl preview = new PreviewControl((BindingSource)data_source_, index);
      this.Controls.Add(preview);

      // 後から追加したものは前面へ
      preview.BringToFront();

      ++index;
    }
  }

  //===================================================================
  // メンバ変数
  //===================================================================
}
}
