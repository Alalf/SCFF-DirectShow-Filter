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

namespace scff_app {

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

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
    resize_method_combo.DisplayMember = "Value";
    resize_method_combo.ValueMember = "Key";
    resize_method_combo.DataSource = data.SWScaleConfig.ResizeMethodList;
    
    // 初期設定
    this.UpdateCurrentDirectory();

    // デフォルトの設定を書き込む
    layoutParametersBindingSource.AddNew();
  }

  /// @brief Processコンボボックスのデータソースを再設定
  private void UpdateCurrentDirectory() {
    data.Directory current_directory = impl_.GetCurrentDirectory();
    entriesBindingSource.Clear();

    if (current_directory.Entries.Count == 0) {
      process_combo.Enabled = false;
      splash.Enabled = false;
      apply.Enabled = false;
      auto_apply.Checked = false;
      auto_apply.Enabled = false;
    } else {
      foreach (data.Entry i in current_directory.Entries) {
        entriesBindingSource.Add(i);
      }

      process_combo.Enabled = true;
      splash.Enabled = true;
      apply.Enabled = true;
      auto_apply.Enabled = true;
    }
  }

  /// @brief 現在の設定をメッセージにして送信する
  private void SendRequest(bool show_message) {
    if (entriesBindingSource.Current == null) {
      return;
    }


    List<data.LayoutParameter> list = new List<data.LayoutParameter>();
    foreach (data.LayoutParameter i in layoutParametersBindingSource.List) {
      list.Add(i);
    }

    int bound_width = ((data.Entry)entriesBindingSource.Current).SampleWidth;
    int bound_height = ((data.Entry)entriesBindingSource.Current).SampleHeight;
    if (!impl_.ValidateParameters(list, bound_width, bound_height, show_message)) {
      return;
    }

    UInt32 process_id = ((data.Entry)entriesBindingSource.Current).ProcessID;
    impl_.SendLayoutRequest(process_id, list, bound_width, bound_height);
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // フォーム
  //-------------------------------------------------------------------
  private void Form1_Shown(object sender, EventArgs e) {
    // 起動時のチェック
    if (!impl_.CheckEnvironment()) {
      Close();
      return;
    }

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
    if (entriesBindingSource.Current == null) {
      return;
    }
    UInt32 process_id = ((data.Entry)entriesBindingSource.Current).ProcessID;
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

  //-------------------------------------------------------------------
  // Layout Profile
  //-------------------------------------------------------------------
  private void layout_profile_add_Click(object sender, EventArgs e) {
  }

  //-------------------------------------------------------------------
  // Target/Window
  //-------------------------------------------------------------------

  private void SetWindow(UIntPtr window) {
    ((data.LayoutParameter)layoutParametersBindingSource.Current).Window = window;
    int window_width, window_height;
    impl_.GetWindowSize(window, out window_width, out window_height);
    ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingX = 0;
    ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingY = 0;
    ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingWidth = window_width;
    ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingHeight = window_height;
    ((data.LayoutParameter)layoutParametersBindingSource.Current).Fit = true;
    layoutParametersBindingSource.ResetCurrentItem();
  }

  private void window_draghere_MouseDown(object sender, MouseEventArgs e) {
    window_draghere.BackColor = Color.Orange;
  }

  private void window_draghere_MouseUp(object sender, MouseEventArgs e) {
    window_draghere.BackColor = SystemColors.Control;

    Point screen_location = window_draghere.PointToScreen(e.Location);
    UIntPtr window = impl_.GetWindowFromPoint(screen_location.X, screen_location.Y);
    this.SetWindow(window);

    if (auto_apply.Checked) {
      SendRequest(false);
    }
  }

  private void window_desktop_Click(object sender, EventArgs e) {
    UIntPtr window = impl_.GetWindowFromDesktop();
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

      UIntPtr window = ((data.LayoutParameter)layoutParametersBindingSource.Current).Window;
      int window_width, window_height;
      impl_.GetWindowSize(window, out window_width, out window_height);
      ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingX = 0;
      ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingY = 0;
      ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingWidth = window_width;
      ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingHeight = window_height;
      ((data.LayoutParameter)layoutParametersBindingSource.Current).Fit = true;
      layoutParametersBindingSource.ResetCurrentItem();

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
    // デスクトップの情報を得る
    UIntPtr window = impl_.GetWindowFromDesktop();
    int window_width, window_height;
    impl_.GetWindowSize(window, out window_width, out window_height);

    // AreaSelectFormを利用してクリッピング領域を取得
    int raw_x, raw_y, raw_width, raw_height;
    using (gui.AreaSelectForm form =
        new gui.AreaSelectForm(window_width, window_height,
                           ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingX,
                           ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingY,
                           ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingWidth,
                           ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingHeight)) {
      form.ShowDialog();
      form.GetResult(out raw_x, out raw_y, out raw_width, out raw_height);
    }

    // デスクトップキャプチャに変更
    ((data.LayoutParameter)layoutParametersBindingSource.Current).Window = window;

    // FitをはずしてClippingを更新
    int clipping_x, clipping_y, clipping_width, clipping_height;
    impl_.JustifyClippingRegion(
        window,
        raw_x, raw_y, raw_width, raw_height,
        out clipping_x, out clipping_y, out clipping_width, out clipping_height);
    ((data.LayoutParameter)layoutParametersBindingSource.Current).Fit = false;
    ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingX = clipping_x;
    ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingY = clipping_y;
    ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingWidth = clipping_width;
    ((data.LayoutParameter)layoutParametersBindingSource.Current).ClippingHeight = clipping_height;

    layoutParametersBindingSource.ResetCurrentItem();

    if (auto_apply.Checked) {
      SendRequest(false);
    }
  }

  //-------------------------------------------------------------------
  // Layout
  //-------------------------------------------------------------------
  private void layout_add_Click(object sender, EventArgs e) {
  }

  private void layout_remove_Click(object sender, EventArgs e) {
  }

  private void layout_layout_Click(object sender, EventArgs e) {
    int bound_width, bound_height;
    if (entriesBindingSource.Current == null) {
      // 一応ダミーで調整できるように
      bound_width = 640;
      bound_height = 360;
    } else {
      bound_width = ((data.Entry)entriesBindingSource.Current).SampleWidth;
      bound_height = ((data.Entry)entriesBindingSource.Current).SampleHeight;
    }

    using (gui.LayoutForm layout_form = new gui.LayoutForm(layoutParametersBindingSource, bound_width, bound_height)) {
      layout_form.ShowDialog();
    
      if (layout_form.GetResult()) {
        if (auto_apply.Checked) {
          SendRequest(false);
        }
      }
    }
  }

  private void resize_method_is_filter_enabled_CheckedChanged(object sender, EventArgs e) {
    bool state = resize_method_is_filter_enabled.Checked;
    resize_method_lgblur.Enabled = state;
    resize_method_cgblur.Enabled = state;
    resize_method_lsharpen.Enabled = state;
    resize_method_csharpen.Enabled = state;
    resize_method_chshift.Enabled = state;
    resize_method_cvshift.Enabled = state;
  }

  private void layoutParametersBindingNavigator_RefreshItems(object sender, EventArgs e) {
    if (layoutParametersBindingSource.Count <= 1) {
      // １個以下にはできない
      layout_bound_relative_top.Enabled = false;
      layout_bound_relative_bottom.Enabled = false;
      layout_bound_relative_left.Enabled = false;
      layout_bound_relative_right.Enabled = false;
      layout_layout.Enabled = false;
      layout_remove.Enabled = false;
    } else if (layoutParametersBindingSource.Count <
        scff_interprocess.Interprocess.kMaxComplexLayoutElements) {
      // 最大値になるまでは要素を追加できるし削除もできる
      layout_bound_relative_top.Enabled = true;
      layout_bound_relative_bottom.Enabled = true;
      layout_bound_relative_left.Enabled = true;
      layout_bound_relative_right.Enabled = true;
      layout_layout.Enabled = true;
      layout_remove.Enabled = true;
    } else {
      // サイズオーバー
      layout_add.Enabled = false;
    }
  }
}
}   // namespace scff_app

