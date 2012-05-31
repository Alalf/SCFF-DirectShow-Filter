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
  AppImplementation impl_;

  /// @brief コンストラクタ
  public Form1() {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------

    // リサイズメソッドのコンボボックスのデータソースを設定
    resizeMethodList.DisplayMember = "Value";
    resizeMethodList.ValueMember = "Key";
    List<KeyValuePair<scff_interprocess.SWScaleFlags,string>> resize_method_list =
        new List<KeyValuePair<scff_interprocess.SWScaleFlags,string>>
            (data.SWScaleConfig.ResizeMethodList);
    resizeMethodList.DataSource = resize_method_list;

    // AppImplementationインスタンスを生成
    impl_ = new AppImplementation();
    
    // 初期設定
    impl_.UpdateDirectory(ref this.entries);

    // デフォルトの設定を書き込む
    this.layoutParameters.AddNew();
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

  private void aeroOn_Click(object sender, EventArgs e) {
    // Aeroの状態を切り替える
    impl_.DWMAPIFlip(aeroOn.Checked);

    aeroOn.Checked = !aeroOn.Checked;
  }

  //-------------------------------------------------------------------
  // OK/CANCEL
  //-------------------------------------------------------------------
  private void splash_Click(object sender, EventArgs e) {
    impl_.SendNull(ref entries, true);
  }

  private void apply_Click(object sender, EventArgs e) {
    impl_.SendMessage(ref entries,
                      ref layoutParameters, true);
  }

  //-------------------------------------------------------------------
  // Process
  //-------------------------------------------------------------------
  private void processRefresh_Click(object sender, EventArgs e) {
    // entiresを更新
    impl_.UpdateDirectory(ref this.entries);
  }

  //-------------------------------------------------------------------
  // Layout Profile
  //-------------------------------------------------------------------
  private void profileAdd_Click(object sender, EventArgs e) {
  }

  //-------------------------------------------------------------------
  // Target/Window
  //-------------------------------------------------------------------

  private void windowDragHere_MouseDown(object sender, MouseEventArgs e) {
    windowDragHere.BackColor = Color.Orange;
  }

  private void windowDragHere_MouseUp(object sender, MouseEventArgs e) {
    windowDragHere.BackColor = SystemColors.Control;

    Point screen_location = windowDragHere.PointToScreen(e.Location);
    impl_.SetWindowFromPoint(ref layoutParameters, screen_location.X, screen_location.Y);

    if (autoApply.Checked) {
      impl_.SendMessage(ref entries,
                        ref layoutParameters, false);
    }
  }

  private void windowDesktop_Click(object sender, EventArgs e) {
    impl_.SetDesktopWindow(ref layoutParameters);

    if (autoApply.Checked) {
      impl_.SendMessage(ref entries,
                        ref layoutParameters, false);
    }
  }

  //-------------------------------------------------------------------
  // Target/Area
  //-------------------------------------------------------------------
  private void areaFit_CheckedChanged(object sender, EventArgs e) {
    if (areaFit.Checked) {
      areaClippingX.Enabled = false;
      areaClippingY.Enabled = false;
      areaClippingWidth.Enabled = false;
      areaClippingHeight.Enabled = false;

      int window_width, window_height;
      impl_.GetWindowSize(((data.LayoutParameter)layoutParameters.Current).Window, out window_width, out window_height);

      ((data.LayoutParameter)layoutParameters.Current).ClippingX = 0;
      ((data.LayoutParameter)layoutParameters.Current).ClippingY = 0;
      ((data.LayoutParameter)layoutParameters.Current).ClippingWidth = window_width;
      ((data.LayoutParameter)layoutParameters.Current).ClippingHeight = window_height;
      ((data.LayoutParameter)layoutParameters.Current).Fit = true;
      layoutParameters.ResetCurrentItem();

      if (autoApply.Checked) {
        impl_.SendMessage(ref entries,
                          ref layoutParameters, false);
      }
    } else {
      areaClippingX.Enabled = true;
      areaClippingY.Enabled = true;
      areaClippingWidth.Enabled = true;
      areaClippingHeight.Enabled = true;
    }
  }

  private void targetAreaSelect_Click(object sender, EventArgs e) {
    // AreaSelectFormを利用してクリッピング領域を取得
    int raw_x, raw_y, raw_width, raw_height;
    Size bound_size = ((data.LayoutParameter)layoutParameters.Current).WindowSize;

    using (gui.AreaSelectForm form = new gui.AreaSelectForm(
        bound_size.Width, bound_size.Height,
        ((data.LayoutParameter)layoutParameters.Current).ClippingX,
        ((data.LayoutParameter)layoutParameters.Current).ClippingY,
        ((data.LayoutParameter)layoutParameters.Current).ClippingWidth,
        ((data.LayoutParameter)layoutParameters.Current).ClippingHeight)) {
      form.ShowDialog();
      form.GetResult(out raw_x, out raw_y, out raw_width, out raw_height);
    }

    // FitをはずしてからDesktopWindowに設定
    ((data.LayoutParameter)layoutParameters.Current).Fit = false;
    ((data.LayoutParameter)layoutParameters.Current).SetWindowFromPtr(ExternalAPI.GetDesktopWindow());
    
    int clipping_x, clipping_y, clipping_width, clipping_height;
    impl_.JustifyClippingRegion(
        bound_size.Width, bound_size.Height,
        raw_x, raw_y, raw_width, raw_height,
        out clipping_x, out clipping_y, out clipping_width, out clipping_height);

    ((data.LayoutParameter)layoutParameters.Current).ClippingX = clipping_x;
    ((data.LayoutParameter)layoutParameters.Current).ClippingY = clipping_y;
    ((data.LayoutParameter)layoutParameters.Current).ClippingWidth = clipping_width;
    ((data.LayoutParameter)layoutParameters.Current).ClippingHeight = clipping_height;

    layoutParameters.ResetCurrentItem();

    if (autoApply.Checked) {
      impl_.SendMessage(ref entries,
                        ref layoutParameters, false);
    }
  }

  //-------------------------------------------------------------------
  // Layout
  //-------------------------------------------------------------------

  private void layoutEdit_Click(object sender, EventArgs e) {
    int bound_width, bound_height;
    if (entries.Count == 0) {
      // 一応ダミーで調整できるように
      bound_width = 640;
      bound_height = 360;
    } else {
      bound_width = ((data.Entry)entries.Current).SampleWidth;
      bound_height = ((data.Entry)entries.Current).SampleHeight;
    }

    using (gui.LayoutForm layout_form = new gui.LayoutForm(layoutParameters, bound_width, bound_height)) {
      layout_form.ShowDialog();
    
      if (layout_form.GetResult()) {
        if (autoApply.Checked) {
          impl_.SendMessage(ref entries,
                            ref layoutParameters, false);
        }
      }
    }
  }

  private void resizeMethodIsFilterEnabled_CheckedChanged(object sender, EventArgs e) {
    bool state = resizeMethodIsFilterEnabled.Checked;
    resizeMethodLGBlur.Enabled = state;
    resizeMethodCGBlur.Enabled = state;
    resizeMethodLSharpen.Enabled = state;
    resizeMethodCSharpen.Enabled = state;
    resizeMethodCHShift.Enabled = state;
    resizeMethodCVShift.Enabled = state;
  }

  private void kLayoutParametersNavigator_RefreshItems(object sender, EventArgs e) {
    if (layoutParameters.Count <= 1) {
      // １個以下にはできない
      layoutBoundRelativeTop.Enabled = false;
      layoutBoundRelativeBottom.Enabled = false;
      layoutBoundRelativeLeft.Enabled = false;
      layoutBoundRelativeRight.Enabled = false;

      layoutAdd.Enabled = true;
      layoutRemove.Enabled = false;
      layoutEdit.Enabled = false;
    } else if (layoutParameters.Count <
        scff_interprocess.Interprocess.kMaxComplexLayoutElements) {
      // 最大値になるまでは要素を追加できるし削除もできる
      layoutBoundRelativeTop.Enabled = true;
      layoutBoundRelativeBottom.Enabled = true;
      layoutBoundRelativeLeft.Enabled = true;
      layoutBoundRelativeRight.Enabled = true;

      layoutEdit.Enabled = true;
      layoutAdd.Enabled = true;
      layoutRemove.Enabled = true;
    } else {
      // 最大値になったので追加はできない
      layoutAdd.Enabled = false;
      layoutRemove.Enabled = true;
      layoutEdit.Enabled = true;
    }
  }

  private void entries_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
    bool entry_exists = this.entries.Count != 0;

    processList.Enabled = entry_exists;
    splash.Enabled = entry_exists;
    apply.Enabled = entry_exists;
    autoApply.Enabled = entry_exists;
  }

  private void layoutParameters_CurrentItemChanged(object sender, EventArgs e) {
    MessageBox.Show("layoutParameters_CurrentItemChanged");
  }

  private void layoutParameters_DataMemberChanged(object sender, EventArgs e) {
    MessageBox.Show("layoutParameters_DataMemberChanged");
  }

  private void layoutParameters_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
    MessageBox.Show("layoutParameters_ListChanged");
  }

  private void layoutParameters_DataSourceChanged(object sender, EventArgs e) {
    MessageBox.Show("layoutParameters_DataSourceChanged");
  }
}
}   // namespace scff_app

