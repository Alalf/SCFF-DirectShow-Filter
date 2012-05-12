
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

/// @file Form1.cs
/// @brief Form1のイベントハンドラの定義

// TODO(progre): 移植未達部分が完了次第名称含め全体をリファクタリング

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;

namespace scff_app {

/// @brief メインウィンドウ
public partial class Form1 : Form {

  private AppImplementation impl;

  public Form1() {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------

    impl = new AppImplementation();

    // DataSoruce/DataBindingsの設定
  }

  /// @brief Event: ウィンドウが閉じられた
  protected override void OnClosed(EventArgs e) {
    base.OnClosed(e);
    impl.DWMAPIRestore();
  }

  private void aero_on_item_Click(object sender, EventArgs e) {
    impl.DWMAPIFlip(aero_on_item.Checked);
  }

  private void window_draghere_MouseDown(object sender, MouseEventArgs e) {
    window_draghere.BackColor = Color.Orange;
  }

  private void process_refresh_Click(object sender, EventArgs e) {
    // ディレクトリから更新
    impl.UpdateDirectory();
  }

  [DllImport("user32.dll")]
  private static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

  private void window_draghere_MouseUp(object sender, MouseEventArgs e) {
    window_draghere.BackColor = SystemColors.Control;

    Point location = window_draghere.PointToScreen(e.Location);

    IntPtr window_handle = WindowFromPoint(location.X, location.Y);
    if (window_handle != IntPtr.Zero) {
      // 見つかった場合
      impl.SetWindow(window_handle);
    } else {
      // nop
    }
  }
  private void splash_Click(object sender, EventArgs e) {
    impl.SendNullLayoutRequest();
  }
  private void apply_Click(object sender, EventArgs e) {
    if (impl.ValidateParameters()) {
      impl.SendNativeLayoutRequest();
    }
  }
  private void process_combo_SelectedIndexChanged(object sender, EventArgs e) {
  }
  private void area_fit_CheckedChanged(object sender, EventArgs e) {
    if (area_fit.Checked) {
      area_clipping_x.Enabled = false;
      area_clipping_y.Enabled = false;
      area_clipping_width.Enabled = false;
      area_clipping_height.Enabled = false;

      impl.ResetClippingRegion();
    } else {
      area_clipping_x.Enabled = true;
      area_clipping_y.Enabled = true;
      area_clipping_width.Enabled = true;
      area_clipping_height.Enabled = true;
    }
  }
  private void window_desktop_Click(object sender, EventArgs e) {
    impl.DoCaptureDesktopWindow();
  }
  private void area_clipping_x_ValueChanged(object sender, EventArgs e) {
    impl.LayoutParameters[impl.EditingLayoutIndex].ClippingX =
        (int)area_clipping_x.Value;
  }
  private void area_clipping_y_ValueChanged(object sender, EventArgs e) {
    impl.LayoutParameters[impl.EditingLayoutIndex].ClippingY =
        (int)area_clipping_y.Value;
  }
  private void area_clipping_width_ValueChanged(object sender, EventArgs e) {
    impl.LayoutParameters[impl.EditingLayoutIndex].ClippingWidth =
        (int)area_clipping_width.Value;
  }
  private void area_clipping_height_ValueChanged(object sender, EventArgs e) {
    impl.LayoutParameters[impl.EditingLayoutIndex].ClippingHeight =
        (int)area_clipping_height.Value;
  }
  private void option_show_mouse_cursor_CheckedChanged(object sender, EventArgs e) {
    impl.LayoutParameters[impl.EditingLayoutIndex].ShowCursor =
        option_show_mouse_cursor.Checked;
  }
  private void option_show_layered_window_CheckedChanged(object sender, EventArgs e) {
    impl.LayoutParameters[impl.EditingLayoutIndex].ShowLayeredWindow =
        option_show_layered_window.Checked;
  }
  private void option_keep_aspect_ratio_CheckedChanged(object sender, EventArgs e) {
    impl.LayoutParameters[impl.EditingLayoutIndex].KeepAspectRatio =
        option_keep_aspect_ratio.Checked;
  }
  private void option_enable_enlargement_CheckedChanged(object sender, EventArgs e) {
    impl.LayoutParameters[impl.EditingLayoutIndex].Stretch =
        option_enable_enlargement.Checked;
  }
  private void option_over_sampling_CheckedChanged(object sender, EventArgs e) {
    /// @todo(me) 実装
  }
  private void option_thread_num_ValueChanged(object sender, EventArgs e) {
    /// @todo(me) 実装
  }
  private void option_resize_method_combo_SelectedIndexChanged(object sender, EventArgs e) {
    impl.LayoutParameters[impl.EditingLayoutIndex].SwsFlags =
        (scff_interprocess.SWScaleFlags)option_resize_method_combo.SelectedValue;
  }
  private void target_area_select_Click(object sender, EventArgs e) {
    AreaSelectForm form = new AreaSelectForm();
    Point new_loc = new Point((int)area_clipping_x.Value,
                  (int)area_clipping_y.Value);
    form.Location = new_loc;
    Size new_size = new Size((int)area_clipping_width.Value,
                                    (int)area_clipping_height.Value);
    form.Size = new_size;
    form.ShowDialog();

    // デスクトップキャプチャに変更
    impl.DoCaptureDesktopWindow();
    // FitをはずしてClippingをかく
    area_fit.Checked = false;
    area_clipping_x.Value = Math.Max(form.clipping_x, area_clipping_x.Minimum);
    area_clipping_y.Value = Math.Max(form.clipping_y, area_clipping_y.Minimum);
    area_clipping_width.Value = Math.Min(form.clipping_width, area_clipping_width.Maximum - area_clipping_x.Value);
    area_clipping_height.Value = Math.Min(form.clipping_height, area_clipping_height.Maximum - area_clipping_y.Value);
  }
  private void Form1_Shown(object sender, EventArgs e) {
    impl.DWMAPIOff();
  }
  private void layout_layout_Click(object sender, EventArgs e) {
    if (impl.ValidateParameters()) {
      impl.SendComplexLayoutRequest();
    }
  }
  private void layout_add_Click(object sender, EventArgs e) {
  }
  private void layout_remove_Click(object sender, EventArgs e) {}
  private void layout_strip_ButtonClick(object sender, EventArgs e) {
    if (Width > 300) {
      Width = 300;
    } else {
      Width = 488;
    }
  }
  private void layout_list_SelectedIndexChanged(object sender, EventArgs e) {}
}
}   // namespace scff_app

