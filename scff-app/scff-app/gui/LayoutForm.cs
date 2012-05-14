
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

/// @file scff-app/gui/LayoutForm.cs
/// @brief レイアウトをGUIで編集するためのフォームの定義
/// @todo(progre) 移植未達部分が完了次第名称含め全体をリファクタリング
/// @todo(me) 全体的にBindingSourceをカスタムコントロールで利用する方法さえわかれば、
///           いろいろとエレガントに対応できそうではある

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace scff_app.gui {

/// @brief レイアウトをGUIで編集するためのフォーム
public partial class LayoutForm : Form {
  private BindingSource layoutParameterBindingSource_;
  private BindingSource entryBindingSource_;

  private List<PreviewControl> previews_; 

  /// @brief コンストラクタ
  public LayoutForm(BindingSource layoutParameterBindingSource, BindingSource entryBindingSource) {
    InitializeComponent();

    layoutParameterBindingSource_ = layoutParameterBindingSource;
    entryBindingSource_ = entryBindingSource;

    int bound_width, bound_height;
    Debug.Assert(entryBindingSource_.Current != null);

    bound_width = ((Entry)entryBindingSource_.Current).SampleWidth;
    bound_height = ((Entry)entryBindingSource_.Current).SampleHeight;

    layout_panel.Width = bound_width;
    layout_panel.Height = bound_height;

    // BindingSourceを見て必要な分だけ
    int index = 0;
    previews_ = new List<PreviewControl>();
    foreach (LayoutParameter i in layoutParameterBindingSource_.List) {
      PreviewControl preview = new PreviewControl(bound_width, bound_height, index, i);
      int x = (int)((i.BoundRelativeLeft * bound_width) / 100);
      int y = (int)((i.BoundRelativeTop * bound_height) / 100);
      int width = (int)(((i.BoundRelativeRight - i.BoundRelativeLeft) * bound_width) / 100);
      int height = (int)(((i.BoundRelativeBottom - i.BoundRelativeTop) * bound_height) / 100);
      preview.Location = new Point(x, y);
      preview.Size = new Size(width, height);
      previews_.Add(preview);
      layout_panel.Controls.Add(preview);
      preview.BringToFront();
      ++index;
    }
  }

  private void add_item_Click(object sender, System.EventArgs e) {
    /// @todo(me) 実装
  }

  private void remove_item_Click(object sender, System.EventArgs e) {
    /// @todo(me) 実装
  }

  private void apply_item_Click(object sender, System.EventArgs e) {
    int bound_width = ((Entry)entryBindingSource_.Current).SampleWidth;
    int bound_height = ((Entry)entryBindingSource_.Current).SampleHeight;

    // 値をPreviewControlから集めてBindingSourceに書き戻す
    foreach(PreviewControl i in previews_) {
      int index = i.IndexInLayoutParameterBindingSource;
      double bound_relative_left = ((double)i.Left * 100.0) / bound_width;
      double bound_relative_right = ((double)i.Right * 100.0) / bound_width;
      double bound_relative_top = ((double)i.Top * 100.0) / bound_height;
      double bound_relative_bottom = ((double)i.Bottom * 100.0) / bound_height;
      ((LayoutParameter)layoutParameterBindingSource_[index]).BoundRelativeLeft =
          bound_relative_left;
      ((LayoutParameter)layoutParameterBindingSource_[index]).BoundRelativeRight =
          bound_relative_right;
      ((LayoutParameter)layoutParameterBindingSource_[index]).BoundRelativeTop =
          bound_relative_top;
      ((LayoutParameter)layoutParameterBindingSource_[index]).BoundRelativeBottom =
          bound_relative_bottom;
    }
    // 更新を他のコントロールに伝える
    layoutParameterBindingSource_.ResetBindings(false);
    Close();
  }

  private void cancel_item_Click(object sender, System.EventArgs e) {
    Close();
  }
}
}
