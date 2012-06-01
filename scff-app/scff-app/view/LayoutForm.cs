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

using System.Collections.Generic;
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

    // Directoryから現在選択中のEntryを取得し、出力幅、高さを得る
    Entry current_entry = (Entry)entries_.Current;
    if (entries_.Count != 0) {
      // 現在選択中のプロセスの幅、高さで調整
      bound_width_ = current_entry.SampleWidth;
      bound_height_ = current_entry.SampleHeight;
    } else {
      // プロセスがなくても一応ダミーの長さで調整可能
      bound_width_ = SCFFApp.kDefaultBoundWidth;
      bound_height_ = SCFFApp.kDefaultBoundHeight;
    }

    previews_ = new List<PreviewControl>();
  }

  //===================================================================
  // オーバーライド
  //===================================================================

  protected override void OnPaintBackground(PaintEventArgs pevent) {
    // 何もしない
    // base.OnPaintBackground(pevent);
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  private void LayoutForm_Load(object sender, System.EventArgs e) {
    /// @todo(me) 100%以外でも調整できるように
    this.layoutPanel.Size = new Size(bound_width_, bound_height_);

    int index = 0;
    foreach (LayoutParameter i in layout_parameters_.List) {
      // PreviewControlの生成
      PreviewControl preview = new PreviewControl(bound_width_, bound_height_, index, i);
      int x = (int)((i.BoundRelativeLeft * bound_width_) / 100);
      int y = (int)((i.BoundRelativeTop * bound_height_) / 100);
      int width = (int)(((i.BoundRelativeRight - i.BoundRelativeLeft) * bound_width_) / 100);
      int height = (int)(((i.BoundRelativeBottom - i.BoundRelativeTop) * bound_height_) / 100);
      preview.Location = new Point(x, y);
      preview.Size = new Size(width, height);

      // リストに追加しておく
      previews_.Add(preview);

      // パネルに配置(後から追加したものは前面へ)
      this.layoutPanel.Controls.Add(preview);
      preview.BringToFront();

      ++index;
    }
  }

  private void add_Click(object sender, System.EventArgs e) {
    /// @todo(me) 実装
  }

  private void remove_Click(object sender, System.EventArgs e) {
    /// @todo(me) 実装
  }

  private void apply_Click(object sender, System.EventArgs e) {
    // 値をPreviewControlから集めてBindingSourceに書き戻す
    foreach(PreviewControl i in previews_) {
      int index = i.Index;
      double bound_relative_left = ((double)i.Left * 100.0) / bound_width_;
      double bound_relative_right = ((double)i.Right * 100.0) / bound_width_;
      double bound_relative_top = ((double)i.Top * 100.0) / bound_height_;
      double bound_relative_bottom = ((double)i.Bottom * 100.0) / bound_height_;
      ((LayoutParameter)layout_parameters_[index]).BoundRelativeLeft =
          bound_relative_left;
      ((LayoutParameter)layout_parameters_[index]).BoundRelativeRight =
          bound_relative_right;
      ((LayoutParameter)layout_parameters_[index]).BoundRelativeTop =
          bound_relative_top;
      ((LayoutParameter)layout_parameters_[index]).BoundRelativeBottom =
          bound_relative_bottom;
    }

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

  readonly int bound_width_;
  readonly int bound_height_;

  List<PreviewControl> previews_;
}
}
