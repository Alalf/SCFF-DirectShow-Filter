
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
using System.Collections.Generic;

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

    // デフォルトの設定を書き込む
    layoutParameterBindingSource.AddNew();
  }

  /// @brief Processコンボボックスのデータソースを再設定
  private void UpdateCurrentDirectory() {
    Directory current_directory = impl_.GetCurrentDirectory();
    entryBindingSource.Clear();

    if (current_directory.Entries.Count == 0) {
      process_combo.Enabled = false;
      splash.Enabled = false;
      apply.Enabled = false;
      auto_apply.Checked = false;
      auto_apply.Enabled = false;
    } else {
      foreach (Entry i in current_directory.Entries) {
        entryBindingSource.Add(i);
      }

      process_combo.Enabled = true;
      splash.Enabled = true;
      apply.Enabled = true;
      auto_apply.Enabled = true;
    }
  }

  /// @brief 現在の設定をメッセージにして送信する
  private void SendRequest(bool show_message) {
    if (entryBindingSource.Current == null) {
      return;
    }


    List<LayoutParameter> list = new List<LayoutParameter>();
    foreach (LayoutParameter i in layoutParameterBindingSource.List) {
      list.Add(i);
    }
    if (!impl_.ValidateParameters(list, show_message)) {
      return;
    }

    UInt32 process_id = ((Entry)entryBindingSource.Current).ProcessID;
    int bound_width = ((Entry)entryBindingSource.Current).SampleWidth;
    int bound_height = ((Entry)entryBindingSource.Current).SampleHeight;

    impl_.SendLayoutRequest(process_id, list, bound_width, bound_height);
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
    if (Width > 310) {
      Width = 310;
    } else {
      Width = 500;
    }
  }

  //-------------------------------------------------------------------
  // OK/CANCEL
  //-------------------------------------------------------------------
  private void splash_Click(object sender, EventArgs e) {
    if (entryBindingSource.Current == null) {
      return;
    }
    UInt32 process_id = ((Entry)entryBindingSource.Current).ProcessID;
    impl_.SendNullLayoutRequest(process_id);
  }
  private void apply_Click(object sender, EventArgs e) {
    this.SendRequest(true);
  }

  //-------------------------------------------------------------------
  // Process
  //-------------------------------------------------------------------
  private void process_refresh_Click(object sender, EventArgs e) {
    // ディレクトリから更新
    this.UpdateCurrentDirectory();
  }

  private void process_combo_SelectedIndexChanged(object sender, EventArgs e) {
  }

  //-------------------------------------------------------------------
  // Layout Profile
  //-------------------------------------------------------------------
  private void layout_profile_add_Click(object sender, EventArgs e) {
  }

  //-------------------------------------------------------------------
  // Target/Window
  //-------------------------------------------------------------------

  private void SetWindow(UInt64 window) {
    ((LayoutParameter)layoutParameterBindingSource.Current).Window = window;
    int clipping_x, clipping_y, clipping_width, clipping_height;
    impl_.GetClippingRegion(window,
        out clipping_x, out clipping_y, out clipping_width, out clipping_height);
    ((LayoutParameter)layoutParameterBindingSource.Current).ClippingX = clipping_x;
    ((LayoutParameter)layoutParameterBindingSource.Current).ClippingY = clipping_y;
    ((LayoutParameter)layoutParameterBindingSource.Current).ClippingWidth = clipping_width;
    ((LayoutParameter)layoutParameterBindingSource.Current).ClippingHeight = clipping_height;
    ((LayoutParameter)layoutParameterBindingSource.Current).Fit = true;
    layoutParameterBindingSource.ResetCurrentItem();
  }

  private void window_draghere_MouseDown(object sender, MouseEventArgs e) {
    window_draghere.BackColor = Color.Orange;
  }

  private void window_draghere_MouseUp(object sender, MouseEventArgs e) {
    window_draghere.BackColor = SystemColors.Control;

    Point screen_location = window_draghere.PointToScreen(e.Location);
    UInt64 window = impl_.GetWindowFromPoint(screen_location.X, screen_location.Y);
    this.SetWindow(window);

    if (auto_apply.Checked) {
      SendRequest(false);
    }
  }

  private void window_desktop_Click(object sender, EventArgs e) {
    UInt64 window = impl_.GetWindowFromDesktop();
    this.SetWindow(window);

    if (auto_apply.Checked) {
      SendRequest(false);
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

      UInt64 window = ((LayoutParameter)layoutParameterBindingSource.Current).Window;
      int clipping_x, clipping_y, clipping_width, clipping_height;
      impl_.GetClippingRegion(window,
          out clipping_x, out clipping_y, out clipping_width, out clipping_height);
      ((LayoutParameter)layoutParameterBindingSource.Current).ClippingX = clipping_x;
      ((LayoutParameter)layoutParameterBindingSource.Current).ClippingY = clipping_y;
      ((LayoutParameter)layoutParameterBindingSource.Current).ClippingWidth = clipping_width;
      ((LayoutParameter)layoutParameterBindingSource.Current).ClippingHeight = clipping_height;
      ((LayoutParameter)layoutParameterBindingSource.Current).Fit = true;
      layoutParameterBindingSource.ResetCurrentItem();

      if (auto_apply.Checked) {
        SendRequest(false);
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
    int raw_x, raw_y, raw_width, raw_height;
    form.GetResult(out raw_x, out raw_y, out raw_width, out raw_height);

    // デスクトップキャプチャに変更
    UInt64 window = impl_.GetWindowFromDesktop();
    ((LayoutParameter)layoutParameterBindingSource.Current).Window = window;

    // FitをはずしてClippingを更新
    int clipping_x, clipping_y, clipping_width, clipping_height;
    impl_.JustifyClippingRegion(
        window,
        raw_x, raw_y, raw_width, raw_height,
        out clipping_x, out clipping_y, out clipping_width, out clipping_height);
    ((LayoutParameter)layoutParameterBindingSource.Current).Fit = false;
    ((LayoutParameter)layoutParameterBindingSource.Current).ClippingX = clipping_x;
    ((LayoutParameter)layoutParameterBindingSource.Current).ClippingY = clipping_y;
    ((LayoutParameter)layoutParameterBindingSource.Current).ClippingWidth = clipping_width;
    ((LayoutParameter)layoutParameterBindingSource.Current).ClippingHeight = clipping_height;

    layoutParameterBindingSource.ResetCurrentItem();

    if (auto_apply.Checked) {
      SendRequest(false);
    }
  }

  //-------------------------------------------------------------------
  // Layout
  //-------------------------------------------------------------------
  private void layout_add_Click(object sender, EventArgs e) {
    if (layoutParameterBindingSource.Count <
        scff_interprocess.Interprocess.kMaxComplexLayoutElements) {
      layoutParameterBindingSource.AddNew();
      layout_bound_relative_top.Enabled = true;
      layout_bound_relative_bottom.Enabled = true;
      layout_bound_relative_left.Enabled = true;
      layout_bound_relative_right.Enabled = true;
    }
  }

  private void layout_remove_Click(object sender, EventArgs e) {
    if (layoutParameterBindingSource.Count > 1) {
      layoutParameterBindingSource.RemoveCurrent();
    }
    if (layoutParameterBindingSource.Count == 1) {
      layout_bound_relative_top.Enabled = false;
      layout_bound_relative_bottom.Enabled = false;
      layout_bound_relative_left.Enabled = false;
      layout_bound_relative_right.Enabled = false;
    }
  }

  private void layout_list_SelectedIndexChanged(object sender, EventArgs e) {
    // nop  
  }

  private void layout_layout_Click(object sender, EventArgs e) {
    // nop
  }
}
}   // namespace scff_app

