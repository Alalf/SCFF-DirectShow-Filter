
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
/// @brief Form1のUIに関連するメソッドの定義
/// @todo(progre) 移植未達部分が完了次第名称含め全体をリファクタリング

using System;
using System.Drawing;
using System.Windows.Forms;

namespace scff_app {

/// @brief メインウィンドウ
public partial class Form1 : Form {

  /// @brief アプリケーションの実装(MVCパターンにおけるModel/Controller)
  private AppImplementation impl_;

  /// @brief コンストラクタ
  public Form1() {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------

    // AppImplementationインスタンスを生成
    impl_ = new AppImplementation();

    // リサイズメソッドのコンボボックスのデータソースを設定
    option_resize_method_combo.DisplayMember = "Item1";
    option_resize_method_combo.ValueMember = "Item2";
    option_resize_method_combo.DataSource = impl_.ResizeMethodList;

    // 初期設定
    this.UpdateCurrentDirectory();
    this.UpdateEditingLayoutIndex(0);

    area_fit.Checked = true;
  }

  /// @brief Processコンボボックスのデータソースを再設定
  private void UpdateCurrentDirectory() {
    process_combo.DataSource = null;
    process_combo.DisplayMember = "EntryInfo";
    process_combo.ValueMember = "ProcessID";
    impl_.UpdateCurrentDirectory();
    process_combo.DataSource = impl_.CurrentDirectory.Entries;
    
    if (impl_.CurrentDirectory.Entries.Count == 0) {
      process_combo.Enabled = false;
      splash.Enabled = false;
      apply.Enabled = false;
      target_auto_apply.Checked = false;
      target_auto_apply.Enabled = false;
    } else {
      process_combo.Enabled = true;
      splash.Enabled = true;
      apply.Enabled = true;
      target_auto_apply.Enabled = true;
    }
  }

  /// @brief DataBindingsを再設定
  private void UpdateEditingLayoutIndex(int editing_layout_index) {
    impl_.EditingLayoutIndex = editing_layout_index;
    layout_bound_x.DataBindings.Add(
        "Value", impl_.LayoutParameters[impl_.EditingLayoutIndex], "BoundX");
    layout_bound_y.DataBindings.Add(
        "Value", impl_.LayoutParameters[impl_.EditingLayoutIndex], "BoundY");
    layout_bound_width.DataBindings.Add(
        "Value", impl_.LayoutParameters[impl_.EditingLayoutIndex], "BoundWidth");
    layout_bound_height.DataBindings.Add(
        "Value", impl_.LayoutParameters[impl_.EditingLayoutIndex], "BoundHeight");
    window_handle.DataBindings.Add(
        "Text", impl_.LayoutParameters[impl_.EditingLayoutIndex], "WindowText");
    window_handle.DataBindings.Add(
        "Tag", impl_.LayoutParameters[impl_.EditingLayoutIndex], "Window");
    area_clipping_x.DataBindings.Add(
        "Value", impl_.LayoutParameters[impl_.EditingLayoutIndex], "ClippingX");
    area_clipping_y.DataBindings.Add(
        "Value", impl_.LayoutParameters[impl_.EditingLayoutIndex], "ClippingY");
    area_clipping_width.DataBindings.Add(
        "Value", impl_.LayoutParameters[impl_.EditingLayoutIndex], "ClippingWidth");
    area_clipping_height.DataBindings.Add(
        "Value", impl_.LayoutParameters[impl_.EditingLayoutIndex], "ClippingHeight");
    option_show_mouse_cursor.DataBindings.Add(
        "Checked", impl_.LayoutParameters[impl_.EditingLayoutIndex], "ShowCursor");
    option_show_layered_window.DataBindings.Add(
        "Checked", impl_.LayoutParameters[impl_.EditingLayoutIndex], "ShowLayeredWindow");
    option_resize_method_combo.DataBindings.Add(
        "SelectedValue", impl_.LayoutParameters[impl_.EditingLayoutIndex], "SwsFlags");
    option_enable_enlargement.DataBindings.Add(
        "Checked", impl_.LayoutParameters[impl_.EditingLayoutIndex], "Stretch");
    option_keep_aspect_ratio.DataBindings.Add(
        "Checked", impl_.LayoutParameters[impl_.EditingLayoutIndex], "KeepAspectRatio");
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // フォーム
  //-------------------------------------------------------------------
  private void Form1_Shown(object sender, EventArgs e) {
    // AeroをOffにしようと試みる
    impl_.DWMAPIOff();
  }
  private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
    // Aeroの状態を元に戻す
    impl_.DWMAPIRestore();
  }

  //-------------------------------------------------------------------
  // メニュー
  //-------------------------------------------------------------------
  private void aero_on_item_Click(object sender, EventArgs e) {
    // Aeroの状態を切り替える
    impl_.DWMAPIFlip(aero_on_item.Checked);
    aero_on_item.Checked = !aero_on_item.Checked;
  }

  //-------------------------------------------------------------------
  // Stripメニュー
  //-------------------------------------------------------------------
  private void layout_strip_ButtonClick(object sender, EventArgs e) {
    if (Width > 300) {
      Width = 300;
    } else {
      Width = 488;
    }
  }

  //-------------------------------------------------------------------
  // OK/CANCEL
  //-------------------------------------------------------------------
  private void splash_Click(object sender, EventArgs e) {
    impl_.SendNullLayoutRequest();
  }
  private void apply_Click(object sender, EventArgs e) {
    if (impl_.ValidateParameters(true)) {
      impl_.SendLayoutRequest();
    }
  }

  //-------------------------------------------------------------------
  // Process
  //-------------------------------------------------------------------
  private void process_refresh_Click(object sender, EventArgs e) {
    // ディレクトリから更新
    this.UpdateCurrentDirectory();
  }

  private void process_combo_SelectedIndexChanged(object sender, EventArgs e) {
    if (process_combo.SelectedValue != null) {
      impl_.EditingProcessID = (System.UInt32)process_combo.SelectedValue;
    }
  }

  //-------------------------------------------------------------------
  // Layout Profile
  //-------------------------------------------------------------------
  private void layout_profile_add_Click(object sender, EventArgs e) {
    MessageBox.Show(impl_.CurrentDirectory.Entries.ToString());
  }

  //-------------------------------------------------------------------
  // Target/Window
  //-------------------------------------------------------------------

  /// @brief クリッピング領域の値の更新
  private void UpdateClippingRegion() {
    impl_.ResetClippingRegion();
    area_clipping_x.DataBindings["Value"].ReadValue();
    area_clipping_y.DataBindings["Value"].ReadValue();
    area_clipping_width.DataBindings["Value"].ReadValue();
    area_clipping_height.DataBindings["Value"].ReadValue();
  }

  private void window_draghere_MouseDown(object sender, MouseEventArgs e) {
    window_draghere.BackColor = Color.Orange;
  }

  private void window_draghere_MouseUp(object sender, MouseEventArgs e) {
    window_draghere.BackColor = SystemColors.Control;

    Point screen_location = window_draghere.PointToScreen(e.Location);
    impl_.SetWindowFromPoint(screen_location.X, screen_location.Y);
    window_handle.DataBindings["Text"].ReadValue();
    window_handle.DataBindings["Tag"].ReadValue();

    UpdateClippingRegion();
    area_fit.Checked = true;

    if (target_auto_apply.Checked) {
      if (impl_.ValidateParameters(false)) {
        impl_.SendLayoutRequest();
      }
    }
  }

  private void window_desktop_Click(object sender, EventArgs e) {
    impl_.SetWindowToDesktop();
    window_handle.DataBindings["Text"].ReadValue();
    window_handle.DataBindings["Tag"].ReadValue();

    UpdateClippingRegion();
    area_fit.Checked = true;

    if (target_auto_apply.Checked) {
      if (impl_.ValidateParameters(false)) {
        impl_.SendLayoutRequest();
      }
    }
  }

  //-------------------------------------------------------------------
  // Target/Area
  //-------------------------------------------------------------------
  private void area_fit_CheckedChanged(object sender, EventArgs e) {
    if (area_fit.Checked) {
      area_clipping_x.Enabled = false;
      area_clipping_y.Enabled = false;
      area_clipping_width.Enabled = false;
      area_clipping_height.Enabled = false;
      this.UpdateClippingRegion();

      if (target_auto_apply.Checked) {
        if (impl_.ValidateParameters(false)) {
          impl_.SendLayoutRequest();
        }
      }
    } else {
      area_clipping_x.Enabled = true;
      area_clipping_y.Enabled = true;
      area_clipping_width.Enabled = true;
      area_clipping_height.Enabled = true;
    }
  }

  private void target_area_select_Click(object sender, EventArgs e) {
    // AreaSelectFormを利用してクリッピング領域を取得
    AreaSelectForm form = new AreaSelectForm();
    Point new_location =
        new Point((int)area_clipping_x.Value, (int)area_clipping_y.Value);
    form.Location = new_location;
    Size new_size =
        new Size((int)area_clipping_width.Value, (int)area_clipping_height.Value);
    form.Size = new_size;
    form.ShowDialog();
    int clipping_x, clipping_y, clipping_width, clipping_height;
    form.GetResult(out clipping_x, out clipping_y, out clipping_width, out clipping_height);

    // デスクトップキャプチャに変更
    impl_.SetWindowToDesktop();
    window_handle.DataBindings["Text"].ReadValue();
    window_handle.DataBindings["Tag"].ReadValue();

    // FitをはずしてClippingを更新
    area_fit.Checked = false;
    impl_.JustifyClippingRegion(clipping_x, clipping_y, clipping_width, clipping_height);
    area_clipping_x.DataBindings["Value"].ReadValue();
    area_clipping_y.DataBindings["Value"].ReadValue();
    area_clipping_width.DataBindings["Value"].ReadValue();
    area_clipping_height.DataBindings["Value"].ReadValue();

    if (target_auto_apply.Checked) {
      if (impl_.ValidateParameters(false)) {
        impl_.SendLayoutRequest();
      }
    }
  }

  //-------------------------------------------------------------------
  // Layout
  //-------------------------------------------------------------------
  private void layout_add_Click(object sender, EventArgs e) {
    // nop
  }

  private void layout_remove_Click(object sender, EventArgs e) {
    // nop
  }

  private void layout_list_SelectedIndexChanged(object sender, EventArgs e) {
    // nop  
  }

  private void layout_layout_Click(object sender, EventArgs e) {
    // nop
  }
}
}   // namespace scff_app

